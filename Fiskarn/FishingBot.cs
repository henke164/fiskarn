using Fiskarn.Models;
using Fiskarn.Services;
using System;
using System.Configuration;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Fiskarn
{
    public class FishingBot
    {
        [DllImport("User32.dll")]
        static extern int SetForegroundWindow(IntPtr point);

        public Point CurrentBaitLocation { get; set; }

        public Rectangle ScanArea { get; set; }

        private GameWindow _gameWindow;

        private Size _baitScanSize = new Size(5, 5);

        private BotState _currentState;

        private ScreenShotService _screenshotService;

        private ImageLocator _imageLocator;

        private int _tries = 0;

        public FishingBot(GameWindow gameWindow)
        {
            _gameWindow = gameWindow;

            LoadScanSize();

            _screenshotService = new ScreenShotService();

            _imageLocator = new ImageLocator();

            _currentState = BotState.FindBaitLocation;

            var screenCenter = new Point(
                _gameWindow.WindowRectangle.X + (_gameWindow.WindowRectangle.Width / 2),
                _gameWindow.WindowRectangle.Y + (_gameWindow.WindowRectangle.Height / 2));
            
            ScanArea = _screenshotService.CreateRectangleFromCenterPoint(screenCenter, new Size(400, 100));
        }

        private void LoadScanSize()
        {
            var scanSize = int.Parse(ConfigurationSettings.AppSettings.Get("scansize"));
            _baitScanSize = new Size(scanSize, scanSize);
        }
        
        public void Update()
        {
            if (_currentState == BotState.FindBaitLocation)
            {
                if (_tries > 5)
                {
                    _tries = 0;
                    HandleKeyboardPress("1");
                    Thread.Sleep(1500);
                }
                FindBaitLocation();
            }
            else if (_currentState == BotState.WaitForBait)
            {
                _tries = 0;
                WaitForBait();
            }
            else if (_currentState == BotState.Loot)
            {
                Thread.Sleep(1000);
                Console.WriteLine("Clicking");
                HandleMouseClick(CurrentBaitLocation);
                _currentState = BotState.FindBaitLocation;
                Thread.Sleep(1000);
                HandleKeyboardPress("1");
                Thread.Sleep(1500);
            }
            Thread.Sleep(100);
        }

        private void WaitForBait()
        {
            var rect = _screenshotService.CreateRectangleFromCenterPoint(CurrentBaitLocation, _baitScanSize);
            var bait = _screenshotService.CaptureScreenShot(rect);
            var baitStill = false;
            for (var x = 0; x < rect.Width; x++)
            {
                for (var y = 0; y < rect.Height; y++)
                {
                    var pixel = bait.GetPixel(x, y);
                    if (pixel.R > pixel.G && pixel.R > pixel.B && pixel.R < 250)
                    {
                        baitStill = true;
                    }
                }
            }

            if (!baitStill)
            {
                _currentState = BotState.Loot;
            }
        }

        private void FindBaitLocation()
        {
            var screen = _screenshotService.CaptureScreenShot(ScanArea);
            var baitLocation = _imageLocator.FindInImage(screen);

            if (baitLocation == Point.Empty)
            {
                if (_tries == 0)
                {
                    Console.WriteLine("Looking for bait...");
                }

                _tries++;
                Thread.Sleep(200);
                return;
            }

            CurrentBaitLocation = new Point(ScanArea.X + baitLocation.X, ScanArea.Y + baitLocation.Y);
            _currentState = BotState.WaitForBait;
            Console.WriteLine("Found bait location! at: " + CurrentBaitLocation.X + " " + CurrentBaitLocation.Y);
            Thread.Sleep(1500);
        }

        private void HandleKeyboardPress(string key)
        {
            SetForegroundWindow(_gameWindow.GameProcess.MainWindowHandle);
            SendKeys.SendWait(key);
        }

        private void HandleMouseClick(Point screenPoint)
            => InputHandler.RightMouseClick(
                _gameWindow.WindowRectangle.X + screenPoint.X + 10,
                _gameWindow.WindowRectangle.Y + screenPoint.Y + 10);
    }
}
