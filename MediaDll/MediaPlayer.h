// MediaPlayer.h
// 使用MMF实现的播放类
#pragma once

#include "pch.h"
#include "MFException.h"
#include "PlayState.h"
#include "CreateMediaSource.h"
#include "SafeRelease.h"
#include "CreatePlaybackTopology.h"

// 使用MMF实现的播放类
class MediaPlayer : public IMFAsyncCallback
{
public:
    // 创建播放器实例，失败则抛出MFException异常
    static void CreateInstance(HWND hVideo, HWND hEvent, MediaPlayer **ppPlayer);

    // IUnknown 方法
    STDMETHODIMP QueryInterface(REFIID iid, void **ppv);
    STDMETHODIMP_(ULONG)
    AddRef();
    STDMETHODIMP_(ULONG)
    Release();

    // IMFAsyncCallback 方法
    STDMETHODIMP GetParameters(DWORD *, DWORD *)
    {
        // 此方法的实现是可选地
        return E_NOTIMPL;
    }

    STDMETHODIMP Invoke(IMFAsyncResult *pAsyncResult);

    // 控制方法，错误则抛出MFException异常
    void OpenURL(const WCHAR *sURL);
    void Play();
    void Pause();
    void Stop();
    void Shutdown();
    HRESULT HandleEvent(UINT_PTR pUnkPtr);
    PlayerState GetState() const { return m_state; }

    // 视频方法
    void Repaint();
    void ResizeVideo(WORD width, WORD height);

    BOOL HasVideo() const
    {
        return (m_pVideoDisplay != NULL);
    };

protected:
    // 构造是私有的。使用静态CreateInstance方法实例化.
    MediaPlayer(HWND hVideo, HWND hEvent);

    // 析构是私有的，使用静态CreateInstance方法实例化.
    virtual ~MediaPlayer();

    //错误则抛出MFException异常
    void Initialize();//初始化
    void CreateSession();//创建会话
    void CloseSession();//关闭会话
    void StartPlayback();//开始播放

    // Media event handlers
    virtual HRESULT OnTopologyStatus(IMFMediaEvent *pEvent);
    virtual HRESULT OnPresentationEnded(IMFMediaEvent *pEvent);
    virtual HRESULT OnNewPresentation(IMFMediaEvent *pEvent);

    // Override to handle additional session events.
    virtual HRESULT OnSessionEvent(IMFMediaEvent *, MediaEventType)
    {
        return S_OK;
    }

protected:
    long m_nRefCount; // Reference count.

    IMFMediaSession *m_pSession;
    IMFMediaSource *m_pSource;
    IMFVideoDisplayControl *m_pVideoDisplay;

    HWND m_hwndVideo;     // 视频窗口句柄.
    HWND m_hwndEvent;     // 接收事件的应用窗口.
    PlayerState m_state;  // 媒体状态.
    HANDLE m_hCloseEvent; // 关闭时的事件.
};