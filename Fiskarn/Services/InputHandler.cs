using System.Runtime.InteropServices;
using System.Threading;

namespace Fiskarn.Services
{
    public static class InputHandler
    {
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public const int MOUSEEVENTF_RIGHTDOWN = 0x08;

        public const int MOUSEEVENTF_RIGHTUP = 0x10;

        public static void RightMouseClick(int xpos, int ypos)
        {
            SetCursorPos(xpos, ypos);
            Thread.Sleep(500);
            mouse_event(MOUSEEVENTF_RIGHTDOWN, xpos, ypos, 0, 0);
            Thread.Sleep(500);
            mouse_event(MOUSEEVENTF_RIGHTUP, xpos, ypos, 0, 0);
            Thread.Sleep(500);
        }

        public static void SetMousePosition(int xpos, int ypos)
        {
            SetCursorPos(xpos, ypos);
        }
    }
}
