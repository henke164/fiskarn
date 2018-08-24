using System.Windows.Forms;

namespace Fiskarn
{
    class Program
    {
        static void Main(string[] args)
        {
            var bot = new FishingBot();

            bot.Start();

            var overlay = new Overlay(bot);

            Application.EnableVisualStyles();

            Application.Run(overlay);
        }
    }
}
