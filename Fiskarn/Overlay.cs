using System;
using System.Drawing;
using System.Windows.Forms;

namespace Fiskarn
{
    public class Overlay : Form
    {
        private FishingBot _bot;
        private Timer _timer;

        public Overlay(FishingBot bot)
        {
            _bot = bot;

            TransparencyKey = Color.White;
            BackColor = Color.White;
            FormBorderStyle = FormBorderStyle.None;
            Bounds = Screen.PrimaryScreen.Bounds;
            TopMost = true;
            
            var exitButton = new Button
            {
                Location = new Point(0, 20),
                Text = "Exit",
                BackColor = Color.Pink
            };

            exitButton.Click += (object sender, EventArgs e) => {
                Application.Exit();
            };

            Controls.Add(exitButton);

            _timer = new Timer();
            _timer.Tick += Update;
            _timer.Interval = 1000;
            _timer.Start();
        }

        private void Update(object sender, EventArgs e)
        {
            using (var g = CreateGraphics())
            {
                g.Clear(Color.White);
                g.DrawRectangle(Pens.Green, _bot.ScanArea);
            }
        }
    }
}
