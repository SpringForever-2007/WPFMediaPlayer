#pragma once

#define WIN32_LEAN_AND_MEAN             // 从 Windows 头文件中排除极少使用的内容
// Windows 头文件
#include <windows.h>
#include <mfapi.h>
#include <mfidl.h>
#include <mfplay.h>
#include <Audiopolicy.h>
#include <Mmdeviceapi.h>
#include <new>

#ifdef MFCLASSES_EXPORT
#define MFCLASSES		__declspec(dllexport)
#define MFFUNCTION		__declspec(dllexport)
#else
#define MFCLASSES __declspec(dllimport)
#define MFFUNCTION		__declspec(dllimport)
#endif // !MFCLASSES_EXPORT