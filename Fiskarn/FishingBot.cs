using Fiskarn.Services;
using System;
using System.Configuration;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fiskarn
{
    public class FishingBot
    {
        public Point CurrentBaitLocation { get; set; }
        public Rectangle ScanArea { get; set; }

        private Size _baitScanSize = new Size(5, 5);
        private BotState _currentState;

        private ScreenShotService _screenshotService;
        private ImageLocator _imageLocator;

        private int _tries = 0;

        public FishingBot()
        {
            var scanSize = int.Parse(ConfigurationSettings.AppSettings.Get("scansize"));
            _baitScanSize = new Size(scanSize, scanSize);
            _screenshotService = new ScreenShotService();
            _imageLocator = new ImageLocator();
            _currentState = BotState.FindBaitLocation;

            var screenCenter = new Point(Screen.PrimaryScreen.Bounds.Width / 2, Screen.PrimaryScreen.Bounds.Height / 2);
            var screenScanSize = new Size(400, 100);
            ScanArea = _screenshotService.CreateRectangleFromCenterPoint(screenCenter, screenScanSize);
        }

        public void Start()
        {
            Task.Run(() => {
                while (true)
                {
                    Update();
                }
            });
        }
        
        private void Update()
        {
            if (_currentState == BotState.FindBaitLocation)
            {
                if (_tries > 5)
                {
                    _tries = 0;
                    SendKeys.SendWait("1");
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
                InputHandler.RightMouseClick(CurrentBaitLocation.X + 10, CurrentBaitLocation.Y + 10);
                _currentState = BotState.FindBaitLocation;
                Thread.Sleep(1000);
                SendKeys.SendWait("1");
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

        private Point ToScreenPosition(Point center, Point innerPoint, Size scanSize)
            => new Point(center.X - (scanSize.Width / 2) + innerPoint.X,
                (center.Y - (scanSize.Height / 2) + innerPoint.Y));

        private Point GetScreenCenter()
        {
            var bounds = Screen.PrimaryScreen.Bounds;
            return new Point(bounds.Width / 2, bounds.Height / 2);
        }
    }
}
