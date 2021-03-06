﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Fiskarn.Services
{
    public class CursorDetector
    {
        [StructLayout(LayoutKind.Sequential)]
        struct CURSORINFO
        {
            public Int32 cbSize;
            public Int32 flags;
            public IntPtr hCursor;
            public POINTAPI ptScreenPos;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct POINTAPI
        {
            public int x;
            public int y;
        }

        [DllImport("user32.dll")]
        static extern bool GetCursorInfo(out CURSORINFO pci);

        [DllImport("user32.dll")]
        static extern bool DrawIcon(IntPtr hDC, int X, int Y, IntPtr hIcon);

        private Bitmap _fishingCursorIcon;

        public CursorDetector()
        {
            _fishingCursorIcon = (Bitmap)Bitmap.FromFile("cursor.png");
        }

        public bool IsFishingCursor()
        {
            try
            {
                using (var currentCursor = GetCurrentCursorIcon())
                {
                    for (var x = 0; x < currentCursor.Width; x += 2)
                    {
                        for (var y = 0; y < currentCursor.Height; y += 2)
                        {
                            var color1 = currentCursor.GetPixel(x, y);
                            var color2 = _fishingCursorIcon.GetPixel(x, y);
                            if (color1.R != color2.R || color1.G != color2.G || color1.B != color2.B)
                            {
                                return false;
                            }
                        }
                    }
                    return true;
                }
            }
            catch
            {
                Application.Restart();
                Application.Exit();
            }
            return false;
        }

        public Bitmap GetCurrentCursorIcon()
        {
            var result = new Bitmap(50, 50);

            var g = Graphics.FromImage(result);
            CURSORINFO pci;
            pci.cbSize = Marshal.SizeOf(typeof(CURSORINFO));

            if (GetCursorInfo(out pci))
            {
                var hdc = g.GetHdc();
                var hCursor = pci.hCursor;
                DrawIcon(hdc, 0, 0, hCursor);
                g.ReleaseHdc(hdc);
            }

            return result;
        }
    }
}
