using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class MovableTransparentWindow : MonoBehaviour
{
	[DllImport("user32.dll")]
	static extern IntPtr GetActiveWindow();

	[DllImport("user32.dll")]
	static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

	[DllImport("user32.dll")]
	static extern int SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

	[DllImport("user32.dll")]
	static extern int SetLayeredWindowAttributes(IntPtr hWnd, int crKey, byte bAlpha, uint dwFlags);

	struct RECT { public int left, top, right, bottom; }

	[DllImport("user32.dll")]
	static extern int GetWindowRect(IntPtr hWnd, out RECT lpRect);

	[DllImport("user32.dll")]
	static extern int MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, int bRepaint);

	struct POINT { public int x, y; }

	[DllImport("user32.dll")]
	static extern int GetCursorPos(out POINT lpPoint);

	const int GWL_STYLE = -16;
	const uint WS_VISIBLE = 0x10000000;
	const uint WS_POPUP = 0x80000000;
	const int HWND_TOPMOST = -1;
	const uint SWP_NOMOVE = 0x0002;
	const int GWL_EXSTYLE = -20;
	const uint WS_EX_LAYERED = 0x00080000;
	const int LWA_COLORKEY = 1;

	void Start()
	{
#if !UNITY_EDITOR
		int width = Screen.width;
		int height = Screen.height;
		var hWnd = GetActiveWindow();
		SetWindowLong(hWnd, GWL_STYLE, WS_VISIBLE | WS_POPUP);
		SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, width, height, SWP_NOMOVE);

		SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED);
		SetLayeredWindowAttributes(hWnd, 0x000000, 0, LWA_COLORKEY);
#endif
	}

	bool movable = false;
	POINT cursorPosition;
	void Update()
	{
		if (Input.GetKey(KeyCode.Escape))
		{
			//Application.Quit();
		}

		if (Input.GetMouseButtonDown(0))
		{
			movable = true;
			GetCursorPos(out cursorPosition);
		}
		else if (Input.GetMouseButtonUp(0))
		{
			movable = false;
		}

		if (movable)
		{
			GetCursorPos(out POINT position);
			int dx = position.x - cursorPosition.x;
			int dy = position.y - cursorPosition.y;
			if (dx != 0 || dy != 0)
			{
#if !UNITY_EDITOR
				var hWnd = GetActiveWindow();
				GetWindowRect(hWnd, out RECT rect);

				int x = rect.left + dx;
				int y = rect.top + dy;
				int width = rect.right - rect.left;
				int height = rect.bottom - rect.top;
				MoveWindow(hWnd, x, y, width, height, 0);
#endif
			}
			cursorPosition = position;
		}
	}
}