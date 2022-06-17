using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;

namespace SCKRM.Window
{
    public static class WindowManager
    {
        public static Process currentProcess { get; } = Process.GetCurrentProcess();
        public static IntPtr currentHandle { get; } = currentProcess.MainWindowHandle;

#if (UNITY_STANDALONE_WIN && !UNITY_EDITOR) || UNITY_EDITOR_WIN
        static float lerpX = 0;
        static float lerpY = 0;

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int sizeX, int sizeY, int uFlags);

        const int SWP_NOSIZE = 0x0001;
        const int SWP_NOMOVE = 0x0002;
        const int SWP_NOZORDER = 0x0004;
        const int SWP_SHOWWINDOW = 0x0040;

        [DllImport("user32.dll")]
        [Obsolete("You can now get the main window handle from the process class. Use only when you can't bring it")]
        static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }

        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
#endif



        [Obsolete("You can now get the main window handle from the process class. Use only when you can't bring it")]
        public static IntPtr GetWindowHandle()
        {
#if (UNITY_STANDALONE_WIN && !UNITY_EDITOR) || UNITY_EDITOR_WIN
            return GetActiveWindow();
#else
            throw new NotSupportedException();
#endif
        }

        public static Vector2Int GetWindowPos() => GetWindowPos(Vector2.zero, Vector2.zero);
        public static Vector2Int GetWindowPos(Vector2 windowDatumPoint, Vector2 screenDatumPoint)
        {
#if (UNITY_STANDALONE_WIN && !UNITY_EDITOR) || UNITY_EDITOR_WIN
            GetWindowRect(currentHandle, out RECT rect);
            return new Vector2Int((rect.Left + ((rect.Right - rect.Left) * windowDatumPoint.x) - (ScreenManager.currentResolution.width * screenDatumPoint.x)).RoundToInt(), (rect.Top + ((rect.Bottom - rect.Top) * windowDatumPoint.y) - (ScreenManager.currentResolution.height * screenDatumPoint.y)).RoundToInt());
#else
            throw new NotSupportedException();
#endif
        }

        public static Vector2Int GetWindowSize()
        {
#if (UNITY_STANDALONE_WIN && !UNITY_EDITOR) || UNITY_EDITOR_WIN
            GetWindowRect(currentHandle, out RECT rect);
            return new Vector2Int(rect.Right - rect.Left, rect.Bottom - rect.Top);
#else
            throw new NotSupportedException();
#endif
        }

        public static Vector2Int GetClientSize()
        {
#if (UNITY_STANDALONE_WIN && !UNITY_EDITOR) || UNITY_EDITOR_WIN
            GetClientRect(currentHandle, out RECT rect);
            return new Vector2Int(rect.Right, rect.Bottom);
#else
            throw new NotSupportedException();
#endif
        }


        /// <summary>
        /// The editor ignores this function.
        /// </summary>
        public static void SetWindowRect(Rect rect, Vector2 windowDatumPoint, Vector2 screenDatumPoint, bool clientDatum = true, bool Lerp = false, float time = 0.05f)
        {
#if (UNITY_STANDALONE_WIN && !UNITY_EDITOR) || UNITY_EDITOR_WIN
            if (Screen.fullScreen)
                Screen.SetResolution(ScreenManager.currentResolution.width, ScreenManager.currentResolution.height, false);
            
            Vector2 border = GetWindowSize() - GetClientSize();

            if (clientDatum)
                SetWindowPos(currentHandle, IntPtr.Zero, 0, 0, (rect.width + border.x).RoundToInt(), (rect.height + border.y).RoundToInt(), SWP_NOZORDER | SWP_NOMOVE | SWP_SHOWWINDOW);
            else
                SetWindowPos(currentHandle, IntPtr.Zero, 0, 0, (rect.width).RoundToInt(), (rect.height).RoundToInt(), SWP_NOZORDER | SWP_NOMOVE | SWP_SHOWWINDOW);

            Vector2 size = GetWindowSize();

            rect.x -= size.x * windowDatumPoint.x;
            rect.y -= size.y * windowDatumPoint.y;

            rect.x += ScreenManager.currentResolution.width * screenDatumPoint.x;
            rect.y += ScreenManager.currentResolution.height * screenDatumPoint.y;

            if (!Lerp)
            {
                lerpX = rect.x;
                lerpY = rect.y;
            }
            else
            {
                lerpX = lerpX.Lerp(rect.x, time);
                lerpY = lerpY.Lerp(rect.y, time);
            }

            if (!Lerp)
                SetWindowPos(currentHandle, IntPtr.Zero, (rect.x).RoundToInt(), (rect.y).RoundToInt(), 0, 0, SWP_NOZORDER | SWP_NOSIZE | SWP_SHOWWINDOW);
            else
                SetWindowPos(currentHandle, IntPtr.Zero, (lerpX).RoundToInt(), (lerpY).RoundToInt(), 0, 0, SWP_NOZORDER | SWP_NOSIZE | SWP_SHOWWINDOW);
#else
            throw new NotSupportedException();
#endif
        }
    }
}