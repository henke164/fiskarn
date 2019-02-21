using Fiskarn.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Fiskarn.Services
{
    public class GameWindowHandler
    {
        public IList<GameWindow> GameWindows { get; private set; }

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        
        public void ReinitializeGameWindows()
        {
            GameWindows = new List<GameWindow>();
            var processes = Process.GetProcessesByName("wow");
            var windowsPerRow = processes.Length > 3 ? 3 : processes.Length;
            var windowWidth = Screen.PrimaryScreen.Bounds.Width / windowsPerRow;
            SetWindowSizeAndPosition(processes, windowWidth);
        }
        
        private void SetWindowSizeAndPosition(Process[] processes, int width)
        {
            var height = (int)(width * 0.75);
            var left = 0;
            var top = 0;
            for (var i = 0; i < processes.Length; i++)
            {
                var rectangle = new Rectangle(left * width, top * height, width, height);

                MoveWindow(
                    processes[i].MainWindowHandle, 
                    rectangle.X, 
                    rectangle.Y, 
                    rectangle.Width, 
                    rectangle.Height, 
                    true);

                GameWindows.Add(new GameWindow
                {
                    GameProcess = processes[i],
                    WindowRectangle = rectangle
                });

                if (left == 3)
                {
                    top++;
                    left = 0;
                }
                else
                {
                    left++;
                }
            }
        }
    }
}
