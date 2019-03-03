using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Fiskarn
{
    public class Overlay : Form
    {
        private IList<FishingBot> _bots;
        private Timer _timer;

        public Overlay(IList<FishingBot> bots)
        {
            _bots = bots;

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
            //_timer.Start();
        }

        private void Update(object sender, EventArgs e)
        {
            using (var g = CreateGraphics())
            {
                g.Clear(Color.White);
                foreach (var b in _bots)
                {
                    g.DrawRectangle(Pens.Green, b.ScanArea);
                }
            }
        }
    }
}
