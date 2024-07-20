// MediaPlayer.cpp

#include "MediaPlayer.h"

HRESULT MediaPlayer::QueryInterface(REFIID riid, void **ppv)
{
    static const QITAB qit[] =
        {
            QITABENT(MediaPlayer, IMFAsyncCallback),
            {0}};
    return QISearch(this, qit, riid, ppv);
}

ULONG MediaPlayer::AddRef()
{
    return InterlockedIncrement(&m_nRefCount);
}

ULONG MediaPlayer::Release()
{
    ULONG uCount = InterlockedDecrement(&m_nRefCount);
    if (uCount == 0)
    {
        delete this;
    }
    return uCount;
}

//  静态类方法创建MediaPlayer实例

void MediaPlayer::CreateInstance(
    HWND hVideo,
    HWND hEvent,
    MediaPlayer **ppPlayer)
{
    HRESULT hr;
    if (ppPlayer == NULL)
    {
        hr = E_POINTER;
        throw MFException<HRESULT>("无效的参数", hr);
    }

    MediaPlayer *pPlayer = new (std::nothrow) MediaPlayer(hVideo, hEvent);
    if (pPlayer == NULL)
    {
        hr = E_OUTOFMEMORY;
        throw MFException<HRESULT>("创建实例失败", hr);
    }

    try
    {
        pPlayer->Initialize();
    }
    catch (MFException<HRESULT> &e)
    {
        hr = e.GetParam();
        e.Show();
        pPlayer->Release();
        throw MFException<HRESULT>("初始化失败", hr);
    }
    *ppPlayer = pPlayer;
}

void MediaPlayer::Initialize()
{
    // 启动MMF.
    HRESULT hr = MFStartup(MF_VERSION);
    if (SUCCEEDED(hr))
    {
        m_hCloseEvent = CreateEvent(NULL, FALSE, FALSE, NULL);
        if (m_hCloseEvent == NULL)
        {
            hr = HRESULT_FROM_WIN32(GetLastError());
            throw MFException<HRESULT>("创建关闭事件失败", hr);
        }
    }
    throw MFException<HRESULT>("初始化失败", hr);
}

MediaPlayer::MediaPlayer(HWND hVideo, HWND hEvent) : m_pSession(NULL),
                                                     m_pSource(NULL),
                                                     m_pVideoDisplay(NULL),
                                                     m_hwndVideo(hVideo),
                                                     m_hwndEvent(hEvent),
                                                     m_state(Closed),
                                                     m_hCloseEvent(NULL),
                                                     m_nRefCount(1)
{
}

MediaPlayer::~MediaPlayer()
{
    // 如果为FALSE，则应用程序未调用Shutdown（）。
    // 当MediaPlayer在上调用IMediaEventGenerator:：BeginGetEvent时
    // 媒体会话，它使媒体会话保持引用
    // 依靠MediaPlayer。
    // 这将在MediaPlayer和
    // 媒体会议。调用Shutdown会打破循环引用
    // 计数。
    // 如果CreateInstance失败，应用程序将不会调用
    // 关机。要处理这种情况，请在析构函数中调用Shutdown.

    Shutdown();
}

//  打开一个URL用于播放.
void MediaPlayer::OpenURL(const WCHAR *sURL)
{
    // 1.创建新的媒体会话。
    // 2.创建媒体源。
    // 3.创建拓扑。
    // 4.将拓扑排队[异步]
    // 5.开始播放[异步-此方法中不会发生。]

    IMFTopology* pTopology = NULL;
    IMFPresentationDescriptor* pSourcePD = NULL;

    try
    {
        // 创建媒体会话。
        CreateSession();

        // 创建媒体源.
        CreateMediaSource(sURL, &m_pSource);

        // 为媒体源创建演示描述符
        m_pSource->CreatePresentationDescriptor(&pSourcePD);

        // 创建局部拓扑
        CreatePlaybackTopology(m_pSource, pSourcePD, m_hwndVideo, &pTopology);

        // 在媒体会话上设置拓扑
        m_pSession->SetTopology(0, pTopology);

        m_state = OpenPending;

        // 如果SetTopology成功，媒体会话将排队
        // MESessionTopologySet事件。

    }
    catch (MFException<HRESULT> &e)
    {
        HRESULT hr=e.GetParam();
        if (FAILED(hr))
        {
            m_state = Closed;
        }

        SafeRelease(&pSourcePD);
        SafeRelease(&pTopology);
        e.Show();
    }
}

//  Create a new instance of the media session.
void MediaPlayer::CreateSession()
{
    try
    {
        // Close the old session, if any.
        CloseSession();

        // Create the media session.
        MFCreateMediaSession(NULL, &m_pSession);

        // Start pulling events from the media session
        m_pSession->BeginGetEvent((IMFAsyncCallback*)this, NULL);

        m_state = Ready;
    }
    catch (MFException<HRESULT>& e)
    {
        e.Show();
    }
}

//  Callback for the asynchronous BeginGetEvent method.

HRESULT MediaPlayer::Invoke(IMFAsyncResult* pResult)
{
    MediaEventType meType = MEUnknown;  // Event type

    IMFMediaEvent* pEvent = NULL;

    // Get the event from the event queue.
    HRESULT hr = m_pSession->EndGetEvent(pResult, &pEvent);
    if (FAILED(hr))
    {
        goto done;
    }

    // Get the event type. 
    hr = pEvent->GetType(&meType);
    if (FAILED(hr))
    {
        goto done;
    }

    if (meType == MESessionClosed)
    {
        // The session was closed. 
        // The application is waiting on the m_hCloseEvent event handle. 
        SetEvent(m_hCloseEvent);
    }
    else
    {
        // For all other events, get the next event in the queue.
        hr = m_pSession->BeginGetEvent(this, NULL);
        if (FAILED(hr))
        {
            goto done;
        }
    }

    // Check the application state. 

    // If a call to IMFMediaSession::Close is pending, it means the 
    // application is waiting on the m_hCloseEvent event and
    // the application's message loop is blocked. 

    // Otherwise, post a private window message to the application. 

    if (m_state != Closing)
    {
        // Leave a reference count on the event.
        pEvent->AddRef();

        PostMessage(m_hwndEvent, WM_APP_PLAYER_EVENT,
            (WPARAM)pEvent, (LPARAM)meType);
    }

done:
    SafeRelease(&pEvent);
    return S_OK;
}

HRESULT MediaPlayer::HandleEvent(UINT_PTR pEventPtr)
{
    HRESULT hrStatus = S_OK;
    MediaEventType meType = MEUnknown;

    IMFMediaEvent* pEvent = (IMFMediaEvent*)pEventPtr;

    if (pEvent == NULL)
    {
        return E_POINTER;
    }

    // Get the event type.
    HRESULT hr = pEvent->GetType(&meType);
    if (FAILED(hr))
    {
        goto done;
    }

    // Get the event status. If the operation that triggered the event 
    // did not succeed, the status is a failure code.
    hr = pEvent->GetStatus(&hrStatus);

    // Check if the async operation succeeded.
    if (SUCCEEDED(hr) && FAILED(hrStatus))
    {
        hr = hrStatus;
    }
    if (FAILED(hr))
    {
        goto done;
    }

    switch (meType)
    {
    case MESessionTopologyStatus:
        hr = OnTopologyStatus(pEvent);
        break;

    case MEEndOfPresentation:
        hr = OnPresentationEnded(pEvent);
        break;

    case MENewPresentation:
        hr = OnNewPresentation(pEvent);
        break;

    default:
        hr = OnSessionEvent(pEvent, meType);
        break;
    }

done:
    SafeRelease(&pEvent);
    return hr;
}

HRESULT MediaPlayer::OnTopologyStatus(IMFMediaEvent* pEvent)
{
    UINT32 status;

    HRESULT hr = pEvent->GetUINT32(MF_EVENT_TOPOLOGY_STATUS, &status);
    if (SUCCEEDED(hr) && (status == MF_TOPOSTATUS_READY))
    {
        SafeRelease(&m_pVideoDisplay);

        // Get the IMFVideoDisplayControl interface from EVR. This call is
        // expected to fail if the media file does not have a video stream.

        MFGetService(m_pSession, MR_VIDEO_RENDER_SERVICE,
            IID_PPV_ARGS(&m_pVideoDisplay));

        StartPlayback();
    }
    return hr;
}

//  Handler for MEEndOfPresentation event.
HRESULT MediaPlayer::OnPresentationEnded(IMFMediaEvent* pEvent)
{
    // The session puts itself into the stopped state automatically.
    m_state = Stopped;
    return S_OK;
}

//  Handler for MENewPresentation event.
//
//  This event is sent if the media source has a new presentation, which 
//  requires a new topology. 

HRESULT MediaPlayer::OnNewPresentation(IMFMediaEvent* pEvent)
{
    HRESULT hr;
    IMFPresentationDescriptor* pPD = NULL;
    IMFTopology* pTopology = NULL;
    try
    {
        // Get the presentation descriptor from the event.
        GetEventObject(pEvent, &pPD);

        // Create a partial topology.
        CreatePlaybackTopology(m_pSource, pPD, m_hwndVideo, &pTopology);

        // Set the topology on the media session.
        hr = m_pSession->SetTopology(0, pTopology);
        if (FAILED(hr))
        {
            throw MFException<HRESULT>("设置拓扑失败", hr);
        }

        m_state = OpenPending;
        return S_OK;
    }
    catch (MFException<HRESULT> &e)
    {
        SafeRelease(&pTopology);
        SafeRelease(&pPD);
        return e.GetParam();
    }
}

//  Start playback from the current position. 
void MediaPlayer::StartPlayback()
{
    PROPVARIANT varStart;
    PropVariantInit(&varStart);

    HRESULT hr = m_pSession->Start(&GUID_NULL, &varStart);
    if (SUCCEEDED(hr))
    {
        // Note: Start is an asynchronous operation. However, we
        // can treat our state as being already started. If Start
        // fails later, we'll get an MESessionStarted event with
        // an error code, and we will update our state then.
        m_state = Started;
    }
    PropVariantClear(&varStart);
}

//  Start playback from paused or stopped.
void MediaPlayer::Play()
{
    if (m_state != Paused && m_state != Stopped)
    {
        throw MFException<HRESULT>("无法暂停", MF_E_INVALIDREQUEST);
    }
    if (m_pSession == NULL || m_pSource == NULL)
    {
        throw MFException<HRESULT>("不存在源", E_UNEXPECTED);
    }
    StartPlayback();
}

//  Pause playback.
void MediaPlayer::Pause()
{
    if (m_state != Started)
    {
        throw MFException<HRESULT>("没有开始", MF_E_INVALIDREQUEST);
    }
    if (m_pSession == NULL || m_pSource == NULL)
    {
        throw MFException<HRESULT>("没有源", E_UNEXPECTED);
    }

    HRESULT hr = m_pSession->Pause();
    if (SUCCEEDED(hr))
    {
        m_state = Paused;
    }

    else throw MFException<HRESULT>("暂停失败", E_UNEXPECTED);
}

// Stop playback.
void MediaPlayer::Stop()
{
    if (m_state != Started && m_state != Paused)
    {
        throw MFException<HRESULT>("无法停止", MF_E_INVALIDREQUEST);
    }
    if (m_pSession == NULL)
    {
        throw MFException<HRESULT>("没有源", E_UNEXPECTED);
    }

    HRESULT hr = m_pSession->Stop();
    if (SUCCEEDED(hr))
    {
        m_state = Stopped;
    }
    else throw MFException<HRESULT>("停止失败", E_UNEXPECTED);
}

//  Repaint the video window. Call this method on WM_PAINT.

void MediaPlayer::Repaint()
{
    if (m_pVideoDisplay)
    {
        m_pVideoDisplay->RepaintVideo();
    }
}