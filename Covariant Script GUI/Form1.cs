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
        private string WorkPath = "C:\\Temp\\CovScriptGUI";
        private string ExePath = "";
        private string TmpPath = "";
        private string FilePath = "";
        private string LogPath = "";
        private bool FileChanged = false;
        private Process MainProc = null;
        public Form1()
        {
            InitializeComponent();
            ExePath = Application.StartupPath + "\\cs.exe";
            LogPath = WorkPath + "\\cs_runtime.log";
            TmpPath = WorkPath + "\\temp.csc";
            Directory.CreateDirectory(WorkPath);
        }

        public void DownloadFile(string URL, string filename, ToolStripProgressBar prog, ToolStripLabel label)
        {
            File.Delete(filename);
            double percent = 0;
            try
            {
                System.Net.HttpWebRequest Myrq = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(URL);
                System.Net.HttpWebResponse myrp = (System.Net.HttpWebResponse)Myrq.GetResponse();
                long totalBytes = myrp.ContentLength;
                if (prog != null)
                    prog.Maximum = (int)totalBytes;
                Stream st = myrp.GetResponseStream();
                Stream so = new FileStream(filename, FileMode.Create);
                long totalDownloadedByte = 0;
                byte[] by = new byte[1024];
                int osize = st.Read(by, 0, by.Length);
                while (osize > 0)
                {
                    totalDownloadedByte = osize + totalDownloadedByte;
                    Application.DoEvents();
                    so.Write(by, 0, osize);
                    if (prog != null)
                        prog.Value = (int)totalDownloadedByte;
                    osize = st.Read(by, 0, by.Length);
                    percent = totalDownloadedByte / (double)totalBytes * 100;
                    label.Text = "下载中 " + ((int)percent).ToString() + "%";
                    Application.DoEvents();
                }
                so.Close();
                st.Close();
            }
            catch (Exception)
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
            File.WriteAllText(TmpPath,textBox1.Text);
            File.Delete(LogPath);
            try
            {
                MainProc = new Process();
                MainProc.StartInfo.FileName = ExePath;
                MainProc.StartInfo.Arguments = "--log-path " + LogPath + " " + TmpPath;
                MainProc.StartInfo.UseShellExecute = true;
                MainProc.StartInfo.WorkingDirectory = WorkPath;
                MainProc.Start();
            }catch(System.ComponentModel.Win32Exception)
            {
                MainProc = null;
                if(MessageBox.Show("缺少必要组件，是否下载？", "Covariant Script GUI", MessageBoxButtons.YesNo,MessageBoxIcon.Error)==DialogResult.Yes)
                {
                    DownloadFile(FileUrl, ExePath, toolStripProgressBar1, toolStripStatusLabel1);
                }
            }
        }

        public void Run(string args,bool compile_only)
        {
            File.WriteAllText(TmpPath, textBox1.Text);
            File.Delete(LogPath);
            try
            {
                MainProc = new Process();
                MainProc.StartInfo.FileName = ExePath;
                MainProc.StartInfo.Arguments = (compile_only?"--compile-only ":"") + "--log-path " + LogPath + " " + TmpPath + " " + args;
                MainProc.StartInfo.UseShellExecute = true;
                MainProc.StartInfo.WorkingDirectory = WorkPath;
                MainProc.Start();
            }
            catch (System.ComponentModel.Win32Exception)
            {
                MainProc = null;
                if (MessageBox.Show("缺少必要组件，是否下载？", "Covariant Script GUI", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                {
                    DownloadFile(FileUrl, ExePath, toolStripProgressBar1, toolStripStatusLabel1);
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

        private void toolStripMenuItem11_Click(object sender, EventArgs e)
        {
            try
            {
                new Error(File.ReadAllText(LogPath)).Show();
            }
            catch(FileNotFoundException)
            {
                MessageBox.Show("无错误信息", "Covariant Script GUI", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
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
            new RunForm(this).Show();
        }

        private void toolStripMenuItem19_Click(object sender, EventArgs e)
        {
            DownloadFile(FileUrl, ExePath, toolStripProgressBar1, toolStripStatusLabel1);
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
            new RunForm(this).Show();
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
            Directory.Delete(WorkPath, true);
        }

        private void textBox1_TextChanged(object sender, System.EventArgs e)
        {
            FileChanged = true;
            Text = FilePath + "(未保存) - Covariant Script GUI";
        }
    }
}
