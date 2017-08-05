using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace Covariant_Script
{
    public partial class Form1 : Form
    {
        private string FilePath = "";
        private bool FileChanged = false;
        private Process MainProc = null;
        private string LastArgs = "";
        public Form1()
        {
            InitializeComponent();
        }

        public void DownloadFile(string URL, string filename, System.Windows.Forms.ToolStripProgressBar prog, System.Windows.Forms.ToolStripLabel label)
        {
            double percent = 0;
            try
            {
                System.Net.HttpWebRequest Myrq = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(URL);
                System.Net.HttpWebResponse myrp = (System.Net.HttpWebResponse)Myrq.GetResponse();
                long totalBytes = myrp.ContentLength;
                if (prog != null)
                    prog.Maximum = (int)totalBytes;
                System.IO.Stream st = myrp.GetResponseStream();
                System.IO.Stream so = new System.IO.FileStream(filename, System.IO.FileMode.Create);
                long totalDownloadedByte = 0;
                byte[] by = new byte[1024];
                int osize = st.Read(by, 0, (int)by.Length);
                while (osize > 0)
                {
                    totalDownloadedByte = osize + totalDownloadedByte;
                    System.Windows.Forms.Application.DoEvents();
                    so.Write(by, 0, osize);
                    if (prog != null)
                        prog.Value = (int)totalDownloadedByte;
                    osize = st.Read(by, 0, (int)by.Length);

                    percent = (double)totalDownloadedByte / (double)totalBytes * 100;
                    label.Text = "Downloading cs.exe " + ((int)percent).ToString() + "%";
                    System.Windows.Forms.Application.DoEvents();
                }
                so.Close();
                st.Close();
            }
            catch (System.Exception)
            {
                MessageBox.Show("Download failed", "Covariant Script GUI", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            MessageBox.Show("Download finished", "Covariant Script GUI", MessageBoxButtons.OK, MessageBoxIcon.Information);
            label.Text = "Ready";
            if (prog != null)
                prog.Value = 0;
        }

        private void OpenFile(string path)
        {
            richTextBox1.LoadFile(path, RichTextBoxStreamType.PlainText);
            FilePath = path;
            FileChanged = false;
            Text = path + " - Covariant Script GUI";
        }

        private void SaveFile(string path)
        {
            richTextBox1.SaveFile(path, RichTextBoxStreamType.PlainText);
            FilePath = path;
            FileChanged = false;
            Text = path + " - Covariant Script GUI";
        }

        private void CheckUnsave()
        {
            if (FileChanged)
            {
                if (MessageBox.Show("File has unsaved changes,do you wang to save them?", "Covariant Script GUI", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (FilePath == "")
                    {
                        if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                            SaveFile(saveFileDialog1.FileName);
                    }
                    else
                        SaveFile(FilePath);
                }
            }
        }

        private void Run()
        {
            richTextBox1.SaveFile(Application.StartupPath + "/temp.csc", RichTextBoxStreamType.PlainText);
            try
            {
                MainProc = Process.Start(Application.StartupPath + "/cs.exe","temp.csc");
            }catch(System.ComponentModel.Win32Exception)
            {
                MainProc = null;
                if(MessageBox.Show("cs.exe not found,do you want to download?", "Covariant Script GUI", MessageBoxButtons.YesNo,MessageBoxIcon.Error)==DialogResult.Yes)
                {
                    DownloadFile("https://github.com/mikecovlee/covscript-gui/raw/master/exe/cs.exe", Application.StartupPath + "/cs.exe", toolStripProgressBar1, toolStripStatusLabel1);
                }
            }
        }

        public void Run(string args)
        {
            LastArgs = args;
            richTextBox1.SaveFile(Application.StartupPath + "/temp.csc", RichTextBoxStreamType.PlainText);
            try
            {
                MainProc = Process.Start(Application.StartupPath + "/cs.exe", "temp.csc " + args);
            }
            catch (System.ComponentModel.Win32Exception)
            {
                MainProc = null;
                if (MessageBox.Show("cs.exe not found,do you want to download?", "Covariant Script GUI", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                {
                    DownloadFile("https://github.com/mikecovlee/covscript-gui/raw/master/exe/cs.exe", Application.StartupPath + "/cs.exe", toolStripProgressBar1, toolStripStatusLabel1);
                }
            }
        }

        private void toolStripMenuItem5_Click(object sender, System.EventArgs e)
        {
            CheckUnsave();
            richTextBox1.Clear();
            FilePath = "";
            FileChanged = false;
            Text = "Covariant Script GUI";
        }

        private void toolStripMenuItem6_Click(object sender, System.EventArgs e)
        {
            CheckUnsave();
            if (openFileDialog1.ShowDialog()==DialogResult.OK)
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

        private void toolStripMenuItem16_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Covariant Script GUI 1.0.0", "About", MessageBoxButtons.OK);
        }

        private void toolStripMenuItem17_Click(object sender, System.EventArgs e)
        {
            Run();
        }

        private void toolStripMenuItem18_Click(object sender, EventArgs e)
        {
            new RunForm(LastArgs, this).Show();
        }

        private void toolStripMenuItem19_Click(object sender, EventArgs e)
        {
            DownloadFile("https://github.com/mikecovlee/covscript-gui/raw/master/exe/cs.exe", Application.StartupPath + "/cs.exe", toolStripProgressBar1, toolStripStatusLabel1);
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

        private void toolStripMenuItem27_Click(object sender, EventArgs e)
        {
            new RunForm(LastArgs,this).Show();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                case Keys.N:
                    if (e.Modifiers == Keys.Control)
                    {
                        toolStripMenuItem5_Click(this, EventArgs.Empty);
                        e.Handled = true;
                    }
                    break;
                case Keys.O:
                    if (e.Modifiers == Keys.Control)
                    {
                        toolStripMenuItem6_Click(this, EventArgs.Empty);
                        e.Handled = true;
                    }
                    break;
                case Keys.S:
                    if (e.Modifiers == Keys.Control)
                    {
                        toolStripMenuItem7_Click(this, EventArgs.Empty);
                        e.Handled = true;
                    }else if ((int)e.Modifiers == ((int)Keys.Control + (int)Keys.Shift))
                    {
                        toolStripMenuItem8_Click(this, EventArgs.Empty);
                        e.Handled = true;
                    }
                    break;
                case Keys.R:
                    if (e.Modifiers == Keys.Control)
                    {
                        toolStripMenuItem26_Click(this, EventArgs.Empty);
                        e.Handled = true;
                    }
                    break;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            CheckUnsave();
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
                File.Delete(Application.StartupPath + "/temp.csc");
            }
        }

        private void richTextBox1_TextChanged(object sender, System.EventArgs e)
        {
            FileChanged = true;
            Text = FilePath + "(Unsaved) - Covariant Script GUI";
        }
    }
}
