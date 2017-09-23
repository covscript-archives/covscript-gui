using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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
