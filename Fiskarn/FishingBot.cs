using Fiskarn.Services;
using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fiskarn
{
    public class FishingBot
    {
        public Point FirstBaitLocation { get; set; }
        public Point CurrentBaitLocation { get; set; }
        public Rectangle ScanArea { get; set; }

        private Size _baitScanSize = new Size(100, 100);
        private BotState _currentState;
        private readonly Bitmap _baitImage = (Bitmap)Bitmap.FromFile("bait.png");

        private ScreenShotService _screenshotService;
        private ImageLocator _imageLocator;
        private Point _screenCenter;

        private int _tries = 0;

        public FishingBot()
        {
            _screenshotService = new ScreenShotService();
            _imageLocator = new ImageLocator();
            _currentState = BotState.FindBaitLocation;

            var screenCenter = new Point(Screen.PrimaryScreen.Bounds.Width / 2, Screen.PrimaryScreen.Bounds.Height / 2);
            var screenScanSize = new Size(400, 300);
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
        }

        private void WaitForBait()
        {
            var rect = _screenshotService.CreateRectangleFromCenterPoint(CurrentBaitLocation, _baitScanSize);
            var bait = _screenshotService.CaptureScreenShot(rect);
            var newBaitLocation = _imageLocator.FindInImage(_baitImage, bait);
            CurrentBaitLocation = ToScreenPosition(CurrentBaitLocation, newBaitLocation, _baitScanSize);

            var dist = GetBaitChangeDistance();
            if (dist > 15)
            {
                _currentState = BotState.Loot;
            }
        }

        private double GetBaitChangeDistance()
            => Math.Sqrt(Math.Pow((FirstBaitLocation.X - CurrentBaitLocation.X), 2) + Math.Pow((FirstBaitLocation.Y - CurrentBaitLocation.Y), 2));
        
        private void FindBaitLocation()
        {
            var screen = _screenshotService.CaptureScreenShot(ScanArea);
            var baitLocation = _imageLocator.FindInImage(_baitImage, screen);

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

            CurrentBaitLocation = ToScreenPosition(GetScreenCenter(), baitLocation, ScanArea.Size);
            FirstBaitLocation = CurrentBaitLocation;
            _currentState = BotState.WaitForBait;
            Console.WriteLine("Found bait location!");
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
