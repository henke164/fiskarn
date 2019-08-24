using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Fiskarn.Services
{
    public static class InputHandler
    {
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private const int WM_KEYDOWN = 0x100;

        private const int WM_KEYUP = 0x101;

        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;

        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        public static void RightMouseClick(int xpos, int ypos)
        {
            SetCursorPos(xpos, ypos);
            Thread.Sleep(100);
            mouse_event(MOUSEEVENTF_RIGHTDOWN, xpos, ypos, 0, 0);
            Thread.Sleep(150);
            mouse_event(MOUSEEVENTF_RIGHTUP, xpos, ypos, 0, 0);
            Thread.Sleep(1000);
        }

        public static void SetMousePosition(int xpos, int ypos)
        {
            SetCursorPos(xpos, ypos);
        }

        public static void PressKey(IntPtr hWnd, Keys key, int ms = 0)
        {
            SendMessage(hWnd, WM_KEYDOWN, Convert.ToInt32(key), 0);
            Thread.Sleep(ms);
            SendMessage(hWnd, WM_KEYUP, Convert.ToInt32(key), 0);
        }
    }
}
