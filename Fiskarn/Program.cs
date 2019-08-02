using Fiskarn.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fiskarn
{
    class Program
    {
        private static GameWindowHandler WindowHandler = new GameWindowHandler();

        static void Main(string[] args)
        {
            WindowHandler.ReinitializeGameWindow();

            var bot = new FishingBot(WindowHandler.GameWindow);

            Task.Run(() => RunBot(bot));

            InitializeOverlay(bot);
        }

        private static void RunBot(FishingBot bot)
        {
            Console.WriteLine("Starting in 5 seconds...");
            Thread.Sleep(3000);
            while (true)
            {
                try
                {
                    bot.Update();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private static void InitializeOverlay(FishingBot bot)
        {
            var overlay = new Overlay(bot);

            Application.EnableVisualStyles();

            Application.Run(overlay);
        }
    }
}
