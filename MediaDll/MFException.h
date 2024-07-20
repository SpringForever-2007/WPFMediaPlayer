//MFException.h
//多媒体异常类
#pragma once

#include <exception>
#include "pch.h"

template<typename ParamType>
class MFException : protected std::exception
{
public:
    MFException(const char* msg, ParamType param) :
        std::exception(msg)
    {
        ErrCode = GetLastError();
        Param = param;
    }

    int GetErrCode() { return ErrCode; }//获取错误码
    ParamType GetParam() { return Param; }//获取参数
    virtual const char* what() const throw() { return std::exception::what(); }//获取错误信息
    //显示错误信息
    void Show(HWND parent = NULL)
    {
        MessageBoxA(parent, what(), "错误", MB_OK | MB_ICONERROR);
    }
private:
    int ErrCode;
    ParamType Param;
};