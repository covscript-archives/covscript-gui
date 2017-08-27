using Covariant_Script;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Covariant_Script_GUI
{
    public partial class Form1 : Form
    {
        private string FileUrl = "http://ldc.atd3.cn/cs.exe";
        private string TempPath = "C:\\Temp\\CovScriptGUI";
        private string FilePath = "";
        private bool FileChanged = false;
        private Process MainProc = null;
        private string LastArgs = "";
        public Form1()
        {
            InitializeComponent();
            Directory.CreateDirectory(TempPath);
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
                    label.Text = "下载中 " + ((int)percent).ToString() + "%";
                    System.Windows.Forms.Application.DoEvents();
                }
                so.Close();
                st.Close();
            }
            catch (System.Exception)
            {
                MessageBox.Show("下载失败", "Covariant Script GUI", MessageBoxButtons.OK, MessageBoxIcon.Error);
                label.Text = "错误";
                if (prog != null)
                    prog.Value = 0;
                return;
            }
            MessageBox.Show("下载完成", "Covariant Script GUI", MessageBoxButtons.OK, MessageBoxIcon.Information);
            label.Text = "就绪";
            if (prog != null)
                prog.Value = 0;
        }

        private void OpenFile(string path)
        {
            textBox1.Text = File.ReadAllText(path);
            FilePath = path;
            FileChanged = false;
            Text = path + " - Covariant Script GUI";
        }

        private void SaveFile(string path)
        {
            File.WriteAllText(path, textBox1.Text);
            FilePath = path;
            FileChanged = false;
            Text = path + " - Covariant Script GUI";
        }

        private void CheckUnsave()
        {
            if (FileChanged)
            {
                if (MessageBox.Show("文件已修改，是否保存？", "Covariant Script GUI", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
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
            File.WriteAllText(TempPath + "\\temp.csc",textBox1.Text);
            try
            {
                MainProc = Process.Start(Application.StartupPath + "\\cs.exe", TempPath + "\\temp.csc");
            }catch(System.ComponentModel.Win32Exception)
            {
                MainProc = null;
                if(MessageBox.Show("缺少必要组件，是否下载？", "Covariant Script GUI", MessageBoxButtons.YesNo,MessageBoxIcon.Error)==DialogResult.Yes)
                {
                    DownloadFile(FileUrl, Application.StartupPath + "\\cs.exe", toolStripProgressBar1, toolStripStatusLabel1);
                }
            }
        }

        public void Run(string args)
        {
            LastArgs = args;
            File.WriteAllText(TempPath + "\\temp.csc", textBox1.Text);
            try
            {
                MainProc = Process.Start(Application.StartupPath + "\\cs.exe", TempPath + "\\temp.csc " + args);
            }
            catch (System.ComponentModel.Win32Exception)
            {
                MainProc = null;
                if (MessageBox.Show("缺少必要组件，是否下载？", "Covariant Script GUI", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                {
                    DownloadFile(FileUrl, Application.StartupPath + "\\cs.exe", toolStripProgressBar1, toolStripStatusLabel1);
                }
            }
        }

        private void toolStripMenuItem5_Click(object sender, System.EventArgs e)
        {
            CheckUnsave();
            textBox1.Clear();
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
            textBox1.Undo();
        }

        private void toolStripMenuItem12_Click(object sender, System.EventArgs e)
        {
            textBox1.Cut();
        }

        private void toolStripMenuItem13_Click(object sender, System.EventArgs e)
        {
            textBox1.Copy();
        }

        private void toolStripMenuItem14_Click(object sender, System.EventArgs e)
        {
            textBox1.Paste();
        }

        private void toolStripMenuItem15_Click(object sender, System.EventArgs e)
        {
            textBox1.SelectAll();
        }

        private void toolStripMenuItem16_Click(object sender, EventArgs e)
        {
            new InfoForm().Show();
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
            DownloadFile(FileUrl, Application.StartupPath + "/cs.exe", toolStripProgressBar1, toolStripStatusLabel1);
        }
        
        private void toolStripMenuItem20_Click(object sender, System.EventArgs e)
        {
            textBox1.Undo();
        }

        private void toolStripMenuItem22_Click(object sender, System.EventArgs e)
        {
            textBox1.Cut();
        }

        private void toolStripMenuItem23_Click(object sender, System.EventArgs e)
        {
            textBox1.Copy();
        }

        private void toolStripMenuItem24_Click(object sender, System.EventArgs e)
        {
            textBox1.Paste();
        }

        private void toolStripMenuItem25_Click(object sender, System.EventArgs e)
        {
            textBox1.SelectAll();
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
                case Keys.A:
                    if (e.Modifiers == Keys.Control)
                    {
                        toolStripMenuItem25_Click(this, EventArgs.Empty);
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
            }
            Directory.Delete(TempPath, true);
        }

        private void textBox1_TextChanged(object sender, System.EventArgs e)
        {
            FileChanged = true;
            Text = FilePath + "(未保存) - Covariant Script GUI";
        }
    }
}
