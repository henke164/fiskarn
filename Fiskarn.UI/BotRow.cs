using Fiskarn.Services;
using System.Windows.Forms;

namespace Fiskarn.UI
{
    public partial class BotRow : UserControl
    {
        public FishingBot Bot { get; set; }
        public BotRow(FishingBot bot)
        {
            Bot = bot;

            InitializeComponent();
            label2.Text = Bot.ProcessId.ToString();

            comboBox1.Items.Clear();
            var names = SoundDetector.GetAllDevices();
            foreach (var name in names)
            {
                comboBox1.Items.Add(name);
            }

            comboBox1.SelectedItem = Bot.SoundDetector.GetDeviceName();
        }

        private void comboBox1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            Bot.SoundDetector = new SoundDetector(comboBox1.SelectedIndex);
        }
    }
}
