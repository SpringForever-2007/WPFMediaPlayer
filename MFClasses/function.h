#pragma once
#include "pch.h"

template<class T>void MFFUNCTION SafeRelease(T** Obj);
void MFFUNCTION HeapSetInformation();
HRESULT CoInitialize();
HRESULT MFFUNCTION CreateMediaSource(PCWSTR sURL, IMFMediaSource** ppSource);
