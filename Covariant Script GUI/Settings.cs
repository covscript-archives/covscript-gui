using System;
using System.Windows.Forms;

namespace Covariant_Script
{
    public partial class Settings : Form
    {
        private ProgramSettings parent_settings;
        public Settings(ProgramSettings s)
        {
            InitializeComponent();
            parent_settings = s;
            InitSettings();
        }

        private void InitSettings()
        {
            textBox1.Text = parent_settings.import_path;
            textBox2.Text = parent_settings.program_path;
            textBox3.Text = parent_settings.log_path;
            numericUpDown1.Value = parent_settings.font_size;
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            parent_settings.InitDefault();
            InitSettings();
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            parent_settings.import_path = textBox1.Text;
            parent_settings.program_path = textBox2.Text;
            parent_settings.log_path = textBox3.Text;
            parent_settings.font_size = (int)numericUpDown1.Value;
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Settings_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    button2_Click(this, EventArgs.Empty);
                    break;
            }
        }
    }
}
