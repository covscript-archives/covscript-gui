using System;
using System.Windows.Forms;

namespace Covariant_Script
{
    public partial class RunForm : Form
    {
        MainForm parent = null;
        bool last_status = false;
        public RunForm(MainForm p)
        {
            InitializeComponent();
            parent = p;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
                parent.DumpAST();
            else
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

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                last_status = checkBox1.Checked;
                checkBox1.Checked = true;
                checkBox1.Enabled = false;
            }
            else
            {
                checkBox1.Checked = last_status;
                checkBox1.Enabled = true;
            }
        }
    }
}
