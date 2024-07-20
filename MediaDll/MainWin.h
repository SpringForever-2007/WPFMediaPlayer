//MainWin.h
#pragma once

#include "pch.h"
#include "MediaPlayer.h"
#include <functional>
#include <list>

using std::function;
using std::list;

struct WinEventItem
{
	UINT msg;
	function<LRESULT(WPARAM, LPARAM)>& fn;
};

class MainWin
{
public:
	MainWin();
	~MainWin();
	void Create();
private:
	static list<WinEventItem> WinEventList;
	LRESULT CALLBACK WndProc(HWND hwnd, UINT msg, WPARAM wp, LPARAM lp);
protected:
	template<typename fn>static void AddWinEventItem(UINT msg,fn)
};
