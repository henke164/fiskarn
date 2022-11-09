using Fiskarn.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Fiskarn.UI
{
    public partial class Form1 : Form
    {
        private List<FishingBot> _fishingBots = new List<FishingBot>();
        private static GameWindowHandler WindowHandler = new GameWindowHandler();

        public Form1()
        {
            InitializeComponent();

            panel1.AutoScroll = false;
            panel1.HorizontalScroll.Enabled = false;
            panel1.HorizontalScroll.Visible = false;
            panel1.HorizontalScroll.Maximum = 0;
            panel1.AutoScroll = true;

            textBox1.Text = SoundDetector.Sensitivity.ToString();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ReloadBots();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            ReloadBots();
        }

        private void ReloadBots()
        {
            WindowHandler.ReinitializeGameWindows();

            var existingBots = _fishingBots.Select(f => f.ProcessId);
            foreach (var gameWindow in WindowHandler.GameWindows)
            {
                if (!existingBots.Contains(gameWindow.GameProcess.Id))
                {
                    _fishingBots.Add(new FishingBot(gameWindow, 0));
                }
            }

            var runningGames = WindowHandler.GameWindows.Select(f => f.GameProcess.Id);
            for (var x = 0; x < _fishingBots.Count; x++)
            {
                if (!runningGames.Contains(_fishingBots[x].ProcessId))
                {
                    _fishingBots.RemoveAt(x);
                    x--;
                }
            }

            RenderBotRows();
        }

        private void RenderBotRows()
        {
            panel1.Controls.Clear();

            var yPos = 0;
            foreach (var bot in _fishingBots)
            {
                var botrow = new BotRow(bot);
                botrow.Location = new Point(0, yPos);
                panel1.Controls.Add(botrow);
                yPos += 53;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            RunBots();
        }

        private void RunBots()
        {
            foreach (var bot in _fishingBots)
            {
                bot.Restart();
            }
            timer1.Start();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            foreach (var bot in _fishingBots)
            {
                bot.Abort();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            foreach (var bot in _fishingBots)
            {
                bot.Update();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            var sens = textBox1.Text;
            if (float.TryParse(sens, out float sensitivity))
            {
                SoundDetector.Sensitivity = sensitivity;
            }
        }
    }
}
