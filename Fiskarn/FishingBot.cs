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
        
        private BotState _currentState;

        private SoundDetector _soundDetector;

        private CursorDetector _cursorDetector = new CursorDetector();

        public FishingBot(GameWindow gameWindow, int audioDeviceIndex)
        {
            _soundDetector = new SoundDetector(audioDeviceIndex);

            _gameWindow = gameWindow;

            _currentState = BotState.FindBaitLocation;

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
            switch (_currentState)
            {
                case BotState.FindBaitLocation:
                    TaskQueue.QueueTask(FindBaitLocation);
                    _currentState = BotState.IsFindingBaitLocation;
                    break;

                case BotState.WaitForBait:
                    WaitForBait();
                    break;

                case BotState.Loot:
                    TaskQueue.QueuePriorityTask(Loot);
                    _currentState = BotState.IsLooting;
                    break;
            }

            Thread.Sleep(100);
        }

        private void WaitForBait()
        {
            if (!_soundDetector.HasVolume())
            {
                return;
            }

            _currentState = BotState.Loot;
        }

        private void FindBaitLocation()
        {
            HandleKeyboardPress("1");
            Thread.Sleep(1500);

            CurrentBaitLocation = Point.Empty;

            for (var y = 0; y < ScanArea.Height && CurrentBaitLocation == Point.Empty; y += 20)
            {
                for (var x = 0; x < ScanArea.Width && CurrentBaitLocation == Point.Empty; x += 20)
                {
                    if (IsBaitLocation(ScanArea.X + x, ScanArea.Y + y))
                    {
                        CurrentBaitLocation = new Point(ScanArea.X + x, ScanArea.Y + y);
                    }
                }
            }

            if (CurrentBaitLocation != Point.Empty)
            {
                _currentState = BotState.WaitForBait;
                Console.WriteLine("Found bait location! at: " + CurrentBaitLocation.X + " " + CurrentBaitLocation.Y);
            }
        }

        private void Loot()
        {
            Console.WriteLine("Clicking at " + CurrentBaitLocation.X + " " + CurrentBaitLocation.Y);
            HandleMouseClick(CurrentBaitLocation);
            _currentState = BotState.FindBaitLocation;
        }

        private void HandleKeyboardPress(string key)
        {
            SetForegroundWindow(_gameWindow.GameProcess.MainWindowHandle);
            Thread.Sleep(50);
            SendKeys.SendWait(key);
        }

        private bool IsBaitLocation(int x, int y)
        {
            InputHandler.SetMousePosition(x, y);
            Thread.Sleep(50);
            return _cursorDetector.IsFishingCursor();
        }

        private void HandleMouseClick(Point screenPoint)
        {
            InputHandler.RightMouseClick(screenPoint.X, screenPoint.Y);
        }
    }
}
