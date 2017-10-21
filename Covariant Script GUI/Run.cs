using System;
using System.Windows.Forms;

namespace Covariant_Script
{
    public partial class RunForm : Form
    {
        MainForm parent = null;
        public RunForm(MainForm p)
        {
            InitializeComponent();
            parent = p;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            parent.RunWithArgs(checkBox1.Checked, textBox1.Text);
            Close();
        }

        private void RunForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                case Keys.Enter:
                    button1_Click(this, EventArgs.Empty);
                    break;
            }
        }
    }
}
