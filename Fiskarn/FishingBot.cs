using Fiskarn.Models;
using Fiskarn.Services;
using System;
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

        public int ProcessId;
        public SoundDetector SoundDetector;
        private bool _shouldAbort = false;

        private GameWindow _gameWindow;
        private BotState _currentState;
        private CursorDetector _cursorDetector = new CursorDetector();
        private DateTime _fishingStarted = DateTime.MinValue;

        public FishingBot(GameWindow gameWindow, int audioDeviceIndex)
        {
            SoundDetector = new SoundDetector(audioDeviceIndex);

            _gameWindow = gameWindow;

            _currentState = BotState.FindBaitLocation;

            ProcessId = _gameWindow.GameProcess.Id;

            ScanArea = GetScanArea();
        }

        public Rectangle GetScanArea()
        {
            var screenCenter = new Point(
                _gameWindow.WindowRectangle.X + (_gameWindow.WindowRectangle.Width / 2),
                _gameWindow.WindowRectangle.Y + (_gameWindow.WindowRectangle.Height / 2));

            var size = new Size(400, 100);

            return new Rectangle(
                screenCenter.X - (size.Width / 2),
                screenCenter.Y - (size.Height / 8),
                size.Width,
                size.Height);
        }

        public void Update()
        {
            _shouldAbort = false;

            switch (_currentState)
            {
                case BotState.FindBaitLocation:
                    _fishingStarted = DateTime.Now;
                    _currentState = BotState.IsFindingBaitLocation;
                    TaskQueue.QueueTask(FindBaitLocation);
                    break;

                case BotState.WaitForBait:
                    var time = DateTime.Now - _fishingStarted;
                    if (time.TotalSeconds > 30)
                    {
                        Console.WriteLine("Restart");
                        _currentState = BotState.FindBaitLocation;
                    }
                    WaitForBait();
                    break;

                case BotState.Loot:
                    _currentState = BotState.IsLooting;
                    TaskQueue.QueuePriorityTask(Loot);
                    break;
            }

            Thread.Sleep(100);
        }

        public void Abort()
        {
            _shouldAbort = true;
            _currentState = BotState.FindBaitLocation;
        }

        private void WaitForBait()
        {
            if (!SoundDetector.HasVolume())
            {
                return;
            }

            Console.WriteLine("Sound from " + SoundDetector.DeviceIndex);
            _currentState = BotState.Loot;
        }

        private void FindBaitLocation()
        {
            HandleKeyboardPress(Keys.D1);
            Thread.Sleep(1500);

            CurrentBaitLocation = Point.Empty;

            var offset = new Point(5, 5);
            for (var y = 0; y < ScanArea.Height && CurrentBaitLocation == Point.Empty; y += 25)
            {
                for (var x = 0; x < ScanArea.Width && CurrentBaitLocation == Point.Empty; x += 25)
                {
                    if (_shouldAbort)
                    {
                        return;
                    }

                    if (IsBaitLocation(ScanArea.X + x, ScanArea.Y + y))
                    {
                        CurrentBaitLocation = new Point(ScanArea.X + x + offset.X, ScanArea.Y + y + offset.Y);
                    }
                }
            }

            if (CurrentBaitLocation != Point.Empty)
            {
                _currentState = BotState.WaitForBait;
                Console.WriteLine("Found bait location! at: " + CurrentBaitLocation.X + " " + CurrentBaitLocation.Y);
            }
            else
            {
                _currentState = BotState.FindBaitLocation;
            }
        }

        private void Loot()
        {
            Console.WriteLine("Clicking at " + CurrentBaitLocation.X + " " + CurrentBaitLocation.Y);
            HandleMouseClick(CurrentBaitLocation);
            _currentState = BotState.FindBaitLocation;
        }

        private void HandleKeyboardPress(Keys key)
        {
            SetForegroundWindow(_gameWindow.GameProcess.MainWindowHandle);
            InputHandler.PressKey(_gameWindow.GameProcess.MainWindowHandle, key);
        }

        private bool IsBaitLocation(int x, int y)
        {
            InputHandler.SetMousePosition(x, y);
            Thread.Sleep(5);
            return _cursorDetector.IsFishingCursor();
        }

        private void HandleMouseClick(Point screenPoint)
        {
            Thread.Sleep(50);
            InputHandler.RightMouseClick(screenPoint.X, screenPoint.Y);
        }
    }
}
