using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

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

        public MainForm(string[] args)
        {
            InitializeComponent();
            settings.InitDefault();
            ReadRegistry();
            if (args.Length == 1)
            {
                if (Path.GetExtension(args[0]) == ".cse")
                {
                    if (MessageBox.Show("是否要安装扩展?\n文件路径:" + args[0], "Covariant Script GUI", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        File.Copy(args[0], Settings.import_path + "/" + Path.GetFileName(args[0]), true);
                        MessageBox.Show("安装完毕", "Covariant Script GUI", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    Environment.Exit(0);
                }
                else
                    OpenFile(args[0]);
            }
            if (File.Exists(Settings.log_path + Configs.Names.CsLog))
                File.Delete(Settings.log_path + Configs.Names.CsLog);
            TmpPath = Path.GetTempFileName();
            textBox1.Font = new System.Drawing.Font(textBox1.Font.Name, settings.font_size);
            Application.DoEvents();
        }

        private void ReadRegistry()
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(Configs.Names.CsRegistry);
            Object bin_path = key.GetValue(Configs.RegistryKey.BinPath);
            Object ipt_path = key.GetValue(Configs.RegistryKey.ImportPath);
            Object log_path = key.GetValue(Configs.RegistryKey.LogPath);
            Object font_size = key.GetValue(Configs.RegistryKey.FontSize);
            Object tab_width = key.GetValue(Configs.RegistryKey.TabWidth);
            if (bin_path == null || ipt_path == null || log_path == null || font_size == null || tab_width == null)
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
                settings.tab_width = int.Parse(tab_width.ToString());
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
            key.SetValue(Configs.RegistryKey.TabWidth, settings.tab_width);
        }

        private string ComposeDefaultArguments()
        {
            string args = DefaultArgs;
            args += " --import-path \"" + Settings.import_path + "\" --log-path \"" + Settings.log_path + Configs.Names.CsLog + "\"";
            return args;
        }

        private string ComposeArguments(string file_path, bool compile_only, string program_args)
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
                Process.Start(settings.program_path + Configs.Names.CsInstBin);
            }
            catch (Exception)
            {
                MessageBox.Show("未找到Covariant Script安装程序", "Covariant Script GUI", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void StartProcess(string bin_name, string args)
        {
            if (CsProcess != null && !CsProcess.HasExited)
            {
                MessageBox.Show("不能同时运行两个进程", "Covariant Script GUI", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                CsProcess = new Process();
                CsProcess.StartInfo.FileName = bin_name;
                CsProcess.StartInfo.Arguments = args;
                CsProcess.StartInfo.UseShellExecute = true;
                CsProcess.StartInfo.WorkingDirectory = Settings.work_path;
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
            textBox1.Text = File.ReadAllText(path).Replace("\r\n", "\n");
            textBox1.Text = textBox1.Text.Replace("\n", "\r\n");
            textBox1.Select(0, 0);
            FilePath = path;
            FileChanged = false;
            toolStripStatusLabel2.Text = "";
            toolStripStatusLabel1.Text = path;
        }

        private void SaveFile(string path)
        {
            File.WriteAllText(path, textBox1.Text);
            FilePath = path;
            FileChanged = false;
            toolStripStatusLabel2.Text = "";
            toolStripStatusLabel1.Text = path;
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
            File.WriteAllText(TmpPath, textBox1.Text);
            File.Delete(Settings.log_path + Configs.Names.CsLog);
            StartProcess(Settings.program_path + Configs.Names.CsBin, ComposeArguments(TmpPath, false, ""));
        }

        public void RunWithArgs(bool compile_only, string args)
        {
            File.WriteAllText(TmpPath, textBox1.Text);
            File.Delete(Settings.log_path + Configs.Names.CsLog);
            StartProcess(Settings.program_path + Configs.Names.CsBin, ComposeArguments(TmpPath, compile_only, args));
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
            toolStripStatusLabel2.Text = "";
            toolStripStatusLabel1.Text = "新文件";
        }

        private void toolStripMenuItem6_Click(object sender, System.EventArgs e)
        {
            CheckUnsave();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                OpenFile(openFileDialog1.FileName);
        }

        private void toolStripMenuItem7_Click(object sender, System.EventArgs e)
        {
            if (FilePath == "")
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
            catch (FileNotFoundException)
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
            switch (e.KeyCode)
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
                    }
                    else if ((int)e.Modifiers == ((int)Keys.Control + (int)Keys.Shift))
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
            toolStripStatusLabel2.Text = "(未保存)";
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
            textBox1.Select(0, 0);
            Application.DoEvents();
        }

        private void toolStripMenuItem29_Click(object sender, EventArgs e)
        {
            Process.Start(Configs.Urls.WebSite);
        }

        private void toolStripMenuItem30_Click(object sender, EventArgs e)
        {
            try
            {
                if (openFileDialog2.ShowDialog() == DialogResult.OK)
                {
                    File.Copy(openFileDialog2.FileName, Settings.import_path + "/" + Path.GetFileName(openFileDialog2.FileName), true);
                    MessageBox.Show("安装完毕", "Covariant Script GUI", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("安装失败", "Covariant Script GUI", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void toolStripMenuItem31_Click(object sender, EventArgs e)
        {
            textBox1.Text = textBox1.Text.Replace("\t", new string(' ', settings.tab_width));
        }

        private void toolStripMenuItem32_Click(object sender, EventArgs e)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = Settings.program_path + Configs.Names.CsReplBin,
                Arguments = "--silent",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                UseShellExecute = false
            };
            try
            {
                Process p = Process.Start(psi);
                p.StandardInput.WriteLine("runtime.info()");
                p.StandardInput.WriteLine("system.exit(0)");
                p.WaitForExit();
                MessageBox.Show(Regex.Match(p.StandardOutput.ReadToEnd(), "Version: (.*)").Value, "Covariant Script GUI", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (System.ComponentModel.Win32Exception)
            {
                if (MessageBox.Show("缺少必要组件，是否下载？", "Covariant Script GUI", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                    DownloadCompoents();
            }
        }

        private void toolStripMenuItem33_Click(object sender, EventArgs e)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = Settings.program_path + Configs.Names.CsReplBin,
                Arguments = "--silent",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                UseShellExecute = false
            };
            try
            {
                Process p = Process.Start(psi);
                p.StandardInput.WriteLine(textBox1.SelectedText);
                p.StandardInput.WriteLine("system.exit(0)");
                p.WaitForExit();
                if (p.StandardError.EndOfStream)
                {
                    string output = p.StandardOutput.ReadToEnd();
                    output = output.TrimEnd('\n', '\r');
                    if (output.Length != 0)
                        MessageBox.Show(output, "Covariant Script GUI", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("无输出", "Covariant Script GUI", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    MessageBox.Show(p.StandardError.ReadToEnd(), "Covariant Script GUI", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (System.ComponentModel.Win32Exception)
            {
                if (MessageBox.Show("缺少必要组件，是否下载？", "Covariant Script GUI", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                    DownloadCompoents();
            }
        }

        private void toolStripMenuItem34_Click(object sender, EventArgs e)
        {
            try
            {
                string result = Regex.Match(File.ReadAllText(Settings.log_path + Configs.Names.CsLog), "File \\\"(.*)\\\", line ([0-9]+)").Groups[2].Value;
                if (result != null && result.Length != 0)
                {
                    int line = int.Parse(result) - 1;
                    int pos = 0;
                    for (int i = 0; i < line; ++i)
                        pos += textBox1.Lines[i].Length + 2;
                    textBox1.Select(pos, textBox1.Lines[line].Length);
                    textBox1.ScrollToCaret();
                }
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("无错误信息", "Covariant Script GUI", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void toolStripMenuItem35_Click(object sender, EventArgs e)
        {
            textBox1.Text = textBox1.Text.Replace("\r\n", "\n");
            textBox1.Text = textBox1.Text.Replace("\n", "\r\n");
            textBox1.Select(0, 0);
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\t')
            {
                SendKeys.SendWait(new string(' ', settings.tab_width));
                e.Handled = true;
            }
        }
    }
}