using System;

namespace Fiskarn
{
    class Program
    {
        static void Main(string[] args)
        {
            var bot = new FishingBot();
            bot.Start();

            Console.ReadLine();
        }
    }
}
