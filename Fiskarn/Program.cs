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
        private static IList<Bitmap> _images;

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
                _images = new List<Bitmap>();
                for (var x = 0; x < Bots.Count; x++)
                {
                    _images.Add(new Bitmap(100, 100));
                }

                while (true)
                {
                    var img = ScreenshotService.CaptureScreenShot();
                    for (var x = 0; x < Bots.Count; x++)
                    {
                        _images[x] = (Bitmap)img.Clone();
                    } 
                    Thread.Sleep(1000 / 100);
                }
            });

            foreach (var bot in Bots)
            {
                Task.Run(() => {
                    while (true)
                    {
                        try
                        {
                            bot.Update((Bitmap)_images[Bots.IndexOf(bot)]);
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
