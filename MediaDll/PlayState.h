//PlayState.h
//定义播放状态
#pragma once
#include "pch.h"
#include "MFException.h"

typedef unsigned int UINT;
const UINT WM_APP_PLAYER_EVENT = WM_APP + 1;   

    // WPARAM = IMFMediaEvent*, WPARAM = MediaEventType

enum PlayerState
{
    Closed = 0,      // 无会话.
    Ready,         // 会话被创建，准备好打开一个文件. 
    OpenPending,    // 会话即将打开一个文件.
    Started,        // 会话正在播放一个文件.
    Paused,         // 会话暂停.
    Stopped,        // 会话停止. 
    Closing         // 应用即将关闭会话，但是等待关闭会话.
};

// Encoding mode
enum ENCODING_MODE {
    NONE = 0x00000000,
    CBR = 0x00000001,
    VBR = 0x00000002,
};

// Video buffer window
const INT32 VIDEO_WINDOW_MSEC = 3000;

