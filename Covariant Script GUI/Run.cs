using System;
using System.Windows.Forms;

namespace Covariant_Script_GUI
{
    public partial class RunForm : Form
    {
        Form1 parent = null;
        public RunForm(string args,Form1 p)
        {
            InitializeComponent();
            textBox1.Text = args;
            parent = p;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            parent.Run(textBox1.Text);
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
