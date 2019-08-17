﻿using Fiskarn.Services;
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
        private static GameWindowHandler WindowHandler = new GameWindowHandler();
        private static IList<FishingBot> Bots;

        static void Main(string[] args)
        {
            WindowHandler.ReinitializeGameWindows();

            Bots = new List<FishingBot>();
            var idx = 0;
            foreach (var gameWindow in WindowHandler.GameWindows)
            {
                Bots.Add(new FishingBot(gameWindow, idx++));
            }

            Console.WriteLine("Starting in 5 sec");
            Thread.Sleep(5000);

            RunBots();

            InitializeOverlay();
        }

        private static void RunBots()
        {
            foreach (var bot in Bots)
            {
                Task.Run(() => {
                    while (true)
                    {
                        try
                        {
                            bot.Update();
                        }
                        catch (Exception ex)
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
