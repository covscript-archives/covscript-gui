using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft;
using Microsoft.Win32;

namespace Covariant_Script
{
    public partial class MainForm : Form
    {
        private ProgramSettings settings = new ProgramSettings();
        private string TmpPath;
        private string FilePath = "";
        private string DefaultArgs = "--wait-before-exit";
        private bool FileChanged = false;
        private Process CsProcess = null;

        internal ProgramSettings Settings { get => settings; set => settings = value; }

        public MainForm()
        {
            InitializeComponent();
            ReadRegistry();
            TmpPath = Path.GetTempFileName();
            textBox1.Font = new System.Drawing.Font(textBox1.Font.Name, settings.font_size);
            Application.DoEvents();
        }

        private void DownloadFile(string URL, string filename, ToolStripProgressBar prog, ToolStripLabel label)
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
            catch (Exception e)
            {
                label.Text = "错误";
                if (prog != null)
                    prog.Value = 0;
                throw e;
            }
            label.Text = "就绪";
            if (prog != null)
                prog.Value = 0;
        }

        private void ReadRegistry()
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(Configs.Names.CsRegistry);
            Object bin_path = key.GetValue(Configs.RegistryKey.BinPath);
            Object ipt_path = key.GetValue(Configs.RegistryKey.ImportPath);
            Object log_path = key.GetValue(Configs.RegistryKey.LogPath);
            Object font_size = key.GetValue(Configs.RegistryKey.FontSize);
            if (bin_path == null || ipt_path == null || log_path == null || font_size == null)
            {
                key.Close();
                settings.InitDefault();
                SaveRegistry();
            }
            else
            {
                settings.program_path = bin_path.ToString();
                settings.import_path = ipt_path.ToString();
                settings.log_path = log_path.ToString();
                settings.font_size = int.Parse(font_size.ToString());
                key.Close();
            }
        }

        private void SaveRegistry()
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(Configs.Names.CsRegistry);
            key.SetValue(Configs.RegistryKey.BinPath, settings.program_path);
            key.SetValue(Configs.RegistryKey.ImportPath, settings.import_path);
            key.SetValue(Configs.RegistryKey.LogPath, settings.log_path);
            key.SetValue(Configs.RegistryKey.FontSize, settings.font_size);
        }

        private string ComposeDefaultArguments()
        {
            string args = DefaultArgs;
            args += " --import-path \"" + Settings.import_path + "\" --log-path \"" + Settings.log_path + Configs.Names.CsLog + "\"";
            return args;
        }

        private string ComposeArguments(string file_path,bool compile_only,string program_args)
        {
            string args = ComposeDefaultArguments();
            if (compile_only)
                args += " --compile-only";
            args += " \"" + file_path + "\" " + program_args;
            return args;
        }

        private void DownloadCompoents()
        {
            try
            {
                DownloadFile(Configs.Urls.Cs, Settings.program_path + Configs.Names.CsBin, toolStripProgressBar1, toolStripStatusLabel1);
                DownloadFile(Configs.Urls.CsRepl, Settings.program_path + Configs.Names.CsReplBin, toolStripProgressBar1, toolStripStatusLabel1);
            }
            catch (Exception)
            {
                MessageBox.Show("下载失败", "Covariant Script GUI", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            MessageBox.Show("下载完成", "Covariant Script GUI", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void StartProcess(string bin_name,string args)
        {
            try
            {
                CsProcess = new Process();
                CsProcess.StartInfo.FileName = bin_name;
                CsProcess.StartInfo.Arguments = args;
                CsProcess.StartInfo.UseShellExecute = true;
                CsProcess.StartInfo.WorkingDirectory = Configs.Paths.WorkPath;
                CsProcess.Start();
            }
            catch (System.ComponentModel.Win32Exception)
            {
                CsProcess = null;
                if (MessageBox.Show("缺少必要组件，是否下载？", "Covariant Script GUI", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                    DownloadCompoents();
            }
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
            File.Delete(Settings.log_path+Configs.Names.CsLog);
            StartProcess(Settings.program_path + Configs.Names.CsBin, ComposeArguments(TmpPath,false,""));
        }

        public void RunWithArgs(bool compile_only, string args)
        {
            File.WriteAllText(TmpPath, textBox1.Text);
            File.Delete(Settings.log_path + Configs.Names.CsLog);
            StartProcess(Settings.program_path + Configs.Names.CsBin, ComposeArguments(TmpPath,compile_only,args));
        }

        private void RunRepl()
        {
            File.WriteAllText(TmpPath, textBox1.Text);
            File.Delete(Settings.log_path + Configs.Names.CsLog);
            StartProcess(Settings.program_path + Configs.Names.CsReplBin, ComposeDefaultArguments());
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
                new Error(File.ReadAllText(Settings.log_path + Configs.Names.CsLog)).Show();
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
            new InfoForm().ShowDialog();
        }

        private void toolStripMenuItem17_Click(object sender, System.EventArgs e)
        {
            Run();
        }

        private void toolStripMenuItem18_Click(object sender, EventArgs e)
        {
            new RunForm(this).ShowDialog();
        }

        private void toolStripMenuItem19_Click(object sender, EventArgs e)
        {
            DownloadCompoents();
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
            new RunForm(this).ShowDialog();
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
            if (CsProcess != null)
            {
                if (!CsProcess.HasExited)
                {
                    CsProcess.Kill();
                    CsProcess.WaitForExit();
                }
            }
            SaveRegistry();
            File.Delete(TmpPath);
        }

        private void textBox1_TextChanged(object sender, System.EventArgs e)
        {
            FileChanged = true;
            Text = FilePath + "(未保存) - Covariant Script GUI";
        }

        private void toolStripMenuItem21_Click(object sender, EventArgs e)
        {
            RunRepl();
        }

        private void toolStripMenuItem28_Click(object sender, EventArgs e)
        {
            new Settings(settings).ShowDialog();
            SaveRegistry();
            textBox1.Font = new System.Drawing.Font(textBox1.Font.Name, settings.font_size);
            Application.DoEvents();
        }

        private void toolStripMenuItem29_Click(object sender, EventArgs e)
        {
            Process.Start(Configs.Urls.WebSite);
        }
    }
}
