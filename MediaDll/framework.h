#pragma once

#define WIN32_LEAN_AND_MEAN             // 从 Windows 头文件中排除极少使用的内容
// Windows 头文件
#include <Windows.h>
#include <new>
#include <mfidl.h>            // Media Foundation interfaces
#include <mfapi.h>            // Media Foundation platform APIs
#include <mferror.h>        // Media Foundation error codes
#include <wmcontainer.h>    // ASF-specific components
#include <wmcodecdsp.h>        // Windows Media DSP interfaces
#include <Dmo.h>            // DMO objects
#include <uuids.h>            // Definition for FORMAT_VideoInfo
#include <propvarutil.h>
#include <shlwapi.h>
#include <mfmediaengine.h>
#include <evr.h>

// The required link libraries 
#pragma comment(lib, "mfplat")
#pragma comment(lib, "mf")
#pragma comment(lib, "mfuuid")
#pragma comment(lib, "msdmo")
#pragma comment(lib, "strmiids")
#pragma comment(lib, "propsys")
