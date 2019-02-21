using Fiskarn.Services;
using System.Windows.Forms;

namespace Fiskarn
{
    class Program
    {
        static void Main(string[] args)
        {
            var windowHandler = new GameWindowHandler();

            var bot = new FishingBot();

            bot.Start();

            var overlay = new Overlay(bot, windowHandler);

            Application.EnableVisualStyles();

            Application.Run(overlay);
        }
    }
}
