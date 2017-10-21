using System.Windows.Forms;

namespace Covariant_Script
{
    public partial class Error : Form
    {
        public Error(string info)
        {
            InitializeComponent();
            textBox1.Text = info;
        }
    }
}
