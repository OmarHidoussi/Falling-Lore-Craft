using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class WindowManagement : MonoBehaviour
{
    const int GWL_STYLE = -16;
    const int WS_BORDER = 0x00800000; // Window with a thin-line border
    const int WS_SYSMENU = 0x00080000; // System menu

    [DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    const uint SWP_NOSIZE = 0x0001;
    const uint SWP_NOMOVE = 0x0002;
    const uint SWP_NOZORDER = 0x0004;
    const uint SWP_NOREDRAW = 0x0008;
    const uint SWP_NOACTIVATE = 0x0010;
    const uint SWP_FRAMECHANGED = 0x0020;
    const uint SWP_SHOWWINDOW = 0x0040;
    const uint SWP_HIDEWINDOW = 0x0080;
    const uint SWP_NOCOPYBITS = 0x0100;
    const uint SWP_NOOWNERZORDER = 0x0200;
    const uint SWP_NOSENDCHANGING = 0x0400;

    void Start()
    {
        HideWindowFrame();
    }

    private void HideWindowFrame()
    {
        IntPtr hwnd = GetForegroundWindow();
        int style = GetWindowLong(hwnd, GWL_STYLE);

        // Remove border and system menu
        style &= ~(WS_BORDER | WS_SYSMENU);

        // Apply the new style
        SetWindowLong(hwnd, GWL_STYLE, style);

        // Force the window to redraw
        SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_NOACTIVATE | SWP_FRAMECHANGED);
    }
}
