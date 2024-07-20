//CreatePlaybackTopology.h
#pragma once

#include "pch.h"
#include "MFException.h"
#include "SafeRelease.h"
#include "AddBranchToPartialTopology.h"

//  Create a playback topology from a media source.
void CreatePlaybackTopology(
    IMFMediaSource* pSource,          // Media source.
    IMFPresentationDescriptor* pPD,   // Presentation descriptor.
    HWND hVideoWnd,                   // Video window.
    IMFTopology** ppTopology)         // Receives a pointer to the topology.
{
    IMFTopology* pTopology = NULL;
    DWORD cSourceStreams = 0;

    try
    {
        // Create a new topology.
        HRESULT hr = MFCreateTopology(&pTopology);
        throw MFException<HRESULT>("创建拓扑失败", hr);




        // Get the number of streams in the media source.
        hr = pPD->GetStreamDescriptorCount(&cSourceStreams);
        throw MFException<HRESULT>("获取流描述书失败", hr);

        // For each stream, create the topology nodes and add them to the topology.
        for (DWORD i = 0; i < cSourceStreams; i++)
        {
            hr = AddBranchToPartialTopology(pTopology, pSource, pPD, i, hVideoWnd);
            throw MFException<HRESULT>("添加", hr);
        }

        // Return the IMFTopology pointer to the caller.
        *ppTopology = pTopology;
        (*ppTopology)->AddRef();
    }
    catch (MFException<HRESULT>& e)
    {
        SafeRelease(&pTopology);
        throw e;
    }
}
