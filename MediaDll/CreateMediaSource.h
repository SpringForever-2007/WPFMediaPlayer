//CreateMediaSource.h
#pragma once
#include "pch.h"
#include "MFException.h"

void CreateMediaSource(PCWSTR sURL, IMFMediaSource** ppSource)
{
    try
    {
        MF_OBJECT_TYPE ObjectType = MF_OBJECT_INVALID;

        IMFSourceResolver* pSourceResolver = NULL;
        IUnknown* pSource = NULL;

        // Create the source resolver.
        HRESULT hr = MFCreateSourceResolver(&pSourceResolver);
        if (FAILED(hr))
            throw MFException<HRESULT>("创建源解决程序", hr);
        // Use the source resolver to create the media source.

        // Note: For simplicity this sample uses the synchronous method to create 
        // the media source. However, creating a media source can take a noticeable
        // amount of time, especially for a network source. For a more responsive 
        // UI, use the asynchronous BeginCreateObjectFromURL method.

        hr = pSourceResolver->CreateObjectFromURL(
            sURL,                       // URL of the source.
            MF_RESOLUTION_MEDIASOURCE,  // Create a source object.
            NULL,                       // Optional property store.
            &ObjectType,        // Receives the created object type. 
            &pSource            // Receives a pointer to the media source.
        );

        if (FAILED(hr))
            throw MFException<HRESULT>("创建URL对象失败", hr);

        // Get the IMFMediaSource interface from the media source.
        hr = pSource->QueryInterface(IID_PPV_ARGS(ppSource));

        if (FAILED(hr))
            throw MFException<HRESULT>("QueryInterface失败", hr);
    }
    catch (MFException<HRESULT>& e)
    {
        throw e;
    }
}