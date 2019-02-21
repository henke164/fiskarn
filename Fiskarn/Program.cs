using Fiskarn.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fiskarn
{
    class Program
    {
        private static GameWindowHandler WindowHandler = new GameWindowHandler();
        private static IList<FishingBot> Bots;

        static void Main(string[] args)
        {
            WindowHandler.ReinitializeGameWindows();

            Bots = new List<FishingBot>();
            foreach (var gameWindow in WindowHandler.GameWindows)
            {
                Bots.Add(new FishingBot(gameWindow));
            }

            InitializeOverlay();

            RunBots();

            while (true) ;
        }

        private static void RunBots()
        {
            Task.Run(() => {
                while (true)
                {
                    foreach (var bot in Bots)
                    {
                        bot.Update();
                    }
                }
            });
        }

        private static void InitializeOverlay()
        {
            var overlay = new Overlay(Bots);

            Application.EnableVisualStyles();

            Application.Run(overlay);
        }
    }
}
