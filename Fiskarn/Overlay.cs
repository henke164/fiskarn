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

            var button = new Button
            {
                Location = new Point(0, 0),
                Text = "Exit",
                BackColor = Color.Pink
            };

            button.Click += (object sender, EventArgs e) => {
                Application.Exit();
            };

            Controls.Add(button);

            _timer = new Timer();
            _timer.Tick += Update;
            _timer.Interval = 10;
            //_timer.Start();
        }

        private void Update(object sender, EventArgs e)
        {
            using (var g = CreateGraphics())
            {
                g.Clear(Color.White);
                g.DrawRectangle(Pens.Orange, new Rectangle(_bot.CurrentBaitLocation, new Size(20, 20)));
                g.DrawRectangle(Pens.Pink, new Rectangle(_bot.CurrentBaitLocation, new Size(20, 20)));
                g.DrawRectangle(Pens.Red, _bot.ScanArea);
            }
        }
    }
}
