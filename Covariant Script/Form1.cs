using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Covariant_Script
{
    public partial class Form1 : Form
    {
        private string FilePath = "";
        private bool FileChanged = false;
        private Process MainProc = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void OpenFile(string path)
        {
            FilePath = path;
            FileChanged = false;
            Text = path + " - Covariant Script";
            richTextBox1.LoadFile(path, RichTextBoxStreamType.PlainText);
        }

        private void SaveFile(string path)
        {
            FilePath = path;
            FileChanged = false;
            Text = path + " - Covariant Script";
            richTextBox1.SaveFile(path, RichTextBoxStreamType.PlainText);
        }

        private void Run()
        {
            richTextBox1.SaveFile(Application.StartupPath + "/init.csc", RichTextBoxStreamType.PlainText);
            MainProc = Process.Start(Application.StartupPath + "/cs.exe");
        }

        private void toolStripMenuItem5_Click(object sender, System.EventArgs e)
        {
            FilePath = null;
            FileChanged = false;
            Text = "Covariant Script";
            richTextBox1.Clear();
        }

        private void toolStripMenuItem6_Click(object sender, System.EventArgs e)
        {
            if(openFileDialog1.ShowDialog()==DialogResult.OK)
                OpenFile(openFileDialog1.FileName);
        }

        private void toolStripMenuItem7_Click(object sender, System.EventArgs e)
        {
            if(FilePath=="")
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    SaveFile(saveFileDialog1.FileName);
            }
            else
                SaveFile(FilePath);
        }

        private void toolStripMenuItem8_Click(object sender, System.EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                SaveFile(saveFileDialog1.FileName);
        }

        private void toolStripMenuItem9_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        private void toolStripMenuItem10_Click(object sender, System.EventArgs e)
        {
            richTextBox1.Undo();
        }

        private void toolStripMenuItem11_Click(object sender, System.EventArgs e)
        {
            richTextBox1.Redo();
        }

        private void toolStripMenuItem12_Click(object sender, System.EventArgs e)
        {
            richTextBox1.Cut();
        }

        private void toolStripMenuItem13_Click(object sender, System.EventArgs e)
        {
            richTextBox1.Copy();
        }

        private void toolStripMenuItem14_Click(object sender, System.EventArgs e)
        {
            richTextBox1.Paste();
        }

        private void toolStripMenuItem15_Click(object sender, System.EventArgs e)
        {
            richTextBox1.SelectAll();
        }

        private void toolStripMenuItem17_Click(object sender, System.EventArgs e)
        {
            Run();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (MainProc != null)
            {
                if (!MainProc.HasExited)
                {
                    MainProc.Kill();
                    MainProc.WaitForExit();
                }
                File.Delete(Application.StartupPath + "/init.csc");
            }
        }

        private void richTextBox1_TextChanged(object sender, System.EventArgs e)
        {
            FileChanged = true;
            Text = FilePath + "(未保存) - Covariant Script";
        }

        private void toolStripMenuItem20_Click(object sender, System.EventArgs e)
        {
            richTextBox1.Undo();
        }

        private void toolStripMenuItem21_Click(object sender, System.EventArgs e)
        {
            richTextBox1.Redo();
        }

        private void toolStripMenuItem22_Click(object sender, System.EventArgs e)
        {
            richTextBox1.Cut();
        }

        private void toolStripMenuItem23_Click(object sender, System.EventArgs e)
        {
            richTextBox1.Copy();
        }

        private void toolStripMenuItem24_Click(object sender, System.EventArgs e)
        {
            richTextBox1.Paste();
        }

        private void toolStripMenuItem25_Click(object sender, System.EventArgs e)
        {
            richTextBox1.SelectAll();
        }

        private void toolStripMenuItem26_Click(object sender, System.EventArgs e)
        {
            Run();
        }
    }
}
