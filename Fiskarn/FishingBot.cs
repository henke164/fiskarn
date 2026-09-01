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

        public Rectangle ScanArea { get; set; }

        public int ProcessId;
        public SoundDetector SoundDetector;

        private GameWindow _gameWindow;
        private BotState _currentState;
        private DateTime _fishingStarted = DateTime.MinValue;

        public FishingBot(GameWindow gameWindow, int audioDeviceIndex)
        {
            SoundDetector = new SoundDetector(audioDeviceIndex);

            _gameWindow = gameWindow;

            _currentState = BotState.CastBait;

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
            switch (_currentState)
            {
                case BotState.CastBait:
                    _fishingStarted = DateTime.Now;
                    TaskQueue.QueueTask(CastBait);
                    break;

                case BotState.WaitForBait:
                    var time = DateTime.Now - _fishingStarted;
                    if (time.TotalSeconds > 30)
                    {
                        Console.WriteLine("Restart");
                        _currentState = BotState.CastBait;
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
            _currentState = BotState.CastBait;
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

        private void CastBait()
        {
            HandleKeyboardPress(Keys.D1);
            _currentState = BotState.WaitForBait;
        }

        private void Loot()
        {
            HandleKeyboardPress(Keys.F);
            Thread.Sleep(1500);
            _currentState = BotState.CastBait;
        }

        private void HandleKeyboardPress(Keys key)
        {
            SetForegroundWindow(_gameWindow.GameProcess.MainWindowHandle);
            InputHandler.PressKey(_gameWindow.GameProcess.MainWindowHandle, key);
        }
    }
}
