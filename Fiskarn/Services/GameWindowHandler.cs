using Fiskarn.Models;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Fiskarn.Services
{
    public class GameWindowHandler
    {
        public GameWindow GameWindow { get; private set; }

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        
        public void ReinitializeGameWindow()
        {
            var processes = Process.GetProcessesByName("wow");
            var windowWidth = Screen.PrimaryScreen.Bounds.Width / 2;
            SetWindowSizeAndPosition(processes[0], windowWidth);
        }
        
        private void SetWindowSizeAndPosition(Process process, int width)
        {
            var height = (int)(width * 0.75);
            var rectangle = new Rectangle(0, 0, width, height);

            MoveWindow(
                process.MainWindowHandle, 
                rectangle.X, 
                rectangle.Y, 
                rectangle.Width, 
                rectangle.Height, 
                true);

            GameWindow = new GameWindow
            {
                GameProcess = process,
                WindowRectangle = rectangle
            };
        }
    }
}
