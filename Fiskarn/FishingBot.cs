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

        public FishingBot(ScreenShotService screenshotService, GameWindow gameWindow)
        {
            _gameWindow = gameWindow;

            LoadScanSize();

            _screenshotService = screenshotService;

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
        
        public void Update(Bitmap screenShot)
        {
            if (_currentState == BotState.FindBaitLocation)
            {
                if (_tries > 5)
                {
                    _tries = 0;
                    HandleKeyboardPress("1");
                    Thread.Sleep(1500);
                }
                FindBaitLocation(screenShot);
            }
            else if (_currentState == BotState.WaitForBait)
            {
                _tries = 0;
                WaitForBait(screenShot);
            }
            else if (_currentState == BotState.Loot)
            {
                Thread.Sleep(1000);
                Console.WriteLine("Clicking at " + CurrentBaitLocation.X + " " + CurrentBaitLocation.Y);
                HandleMouseClick(CurrentBaitLocation);
                _currentState = BotState.FindBaitLocation;
                Thread.Sleep(1000);
                HandleKeyboardPress("1");
                Thread.Sleep(1500);
            }
            Thread.Sleep(100);
        }

        private void WaitForBait(Bitmap screenShot)
        {
            var rect = _screenshotService.CreateRectangleFromCenterPoint(CurrentBaitLocation, _baitScanSize);
            var bait = _screenshotService.GetScreenshotFromImage(screenShot, rect);
            var baitStill = false;
            for (var x = 0; x < rect.Width; x++)
            {
                for (var y = 0; y < rect.Height; y++)
                {
                    var pixel = bait.GetPixel(x, y);
                    if (pixel.B < 120 && pixel.G < 120 && pixel.R > 150 && pixel.R < 250)
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

        private void FindBaitLocation(Bitmap screenShot)
        {
            var screen = _screenshotService.GetScreenshotFromImage(screenShot, ScanArea);
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
            InteractionHandler.QueueInteraction(() =>
            {
                SetForegroundWindow(_gameWindow.GameProcess.MainWindowHandle);
                SendKeys.SendWait(key);
            });
        }

        private void HandleMouseClick(Point screenPoint)
        {
            InteractionHandler.QueueInteraction(() => 
            { 
                InputHandler.RightMouseClick(screenPoint.X, screenPoint.Y);
            });
        }
    }
}
