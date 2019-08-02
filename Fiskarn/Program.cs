using Fiskarn.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fiskarn
{
    class Program
    {
        private static ScreenShotService ScreenshotService = new ScreenShotService();
        private static GameWindowHandler WindowHandler = new GameWindowHandler();
        private static IList<FishingBot> Bots;
        private static Bitmap _currentScreenShot;
        static void Main(string[] args)
        {
            WindowHandler.ReinitializeGameWindows();

            Bots = new List<FishingBot>();
            foreach (var gameWindow in WindowHandler.GameWindows)
            {
                Bots.Add(new FishingBot(ScreenshotService, gameWindow));
            }

            RunBots();

            InitializeOverlay();
        }

        private static void RunBots()
        {
            Task.Run(() => {
                while (true)
                {
                    _currentScreenShot = ScreenshotService.CaptureScreenShot();
                    Thread.Sleep(500);
                    _currentScreenShot.Dispose();
                }
            });

            foreach (var bot in Bots)
            {
                Task.Run(() => {
                    while (true)
                    {
                        try
                        {
                            bot.Update(_currentScreenShot);
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                });
            }
        }

        private static void InitializeOverlay()
        {
            var overlay = new Overlay(Bots);

            Application.EnableVisualStyles();

            Application.Run(overlay);
        }
    }
}
