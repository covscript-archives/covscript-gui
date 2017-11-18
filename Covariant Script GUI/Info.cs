using System.Windows.Forms;

namespace Covariant_Script
{
    public partial class InfoForm : Form
    {
        public InfoForm()
        {
            InitializeComponent();
            label1.Text = "Covariant Script GUI v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}
