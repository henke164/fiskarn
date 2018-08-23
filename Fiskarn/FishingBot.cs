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
        private Size _screenScanSize = new Size(400, 300);
        private Size _baitScanSize = new Size(200, 200);
        private BotState _currentState;
        private readonly Bitmap _baitImage = (Bitmap)Bitmap.FromFile("bait.png");

        private ScreenShotService _screenshotService;
        private ImageLocator _imageLocator;
        private Point _screenCenter;
        private Point _baitLocation;

        private int _tries = 0;

        public FishingBot()
        {
            _screenCenter = new Point(Screen.PrimaryScreen.Bounds.Width / 2, Screen.PrimaryScreen.Bounds.Height / 2);
            _screenshotService = new ScreenShotService();
            _imageLocator = new ImageLocator();
            _currentState = BotState.FindBaitLocation;
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
                InputHandler.RightMouseClick(_baitLocation.X + 10, _baitLocation.Y + 10);
                _currentState = BotState.FindBaitLocation;
                Thread.Sleep(1000);
                SendKeys.SendWait("1");
                Thread.Sleep(1500);
            }
        }

        private void WaitForBait()
        {
            var bait = _screenshotService.CaptureScreenShot(_baitLocation, _baitScanSize);
            var baitLocation = _imageLocator.FindInImage(_baitImage, bait);
            if (baitLocation == Point.Empty)
            {
                _currentState = BotState.Loot;
            }
        }

        private void FindBaitLocation()
        {
            var screen = _screenshotService.CaptureScreenShot(_screenCenter, _screenScanSize);
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

            _baitLocation = ToScreenPosition(baitLocation);
            _currentState = BotState.WaitForBait;
            Console.WriteLine("Found bait location!");
            Thread.Sleep(1500);
        }

        private Point ToScreenPosition(Point p)
            => new Point((Screen.PrimaryScreen.Bounds.Width / 2) - (_screenScanSize.Width / 2) + p.X,
                (Screen.PrimaryScreen.Bounds.Height / 2) - (_screenScanSize.Height / 2) + p.Y);
    }
}
