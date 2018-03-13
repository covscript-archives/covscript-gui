using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Covariant_Script
{
    public partial class Pack : Form
    {
        string code;
        ProgramSettings settings;
        public Pack(ProgramSettings programSettings,string text)
        {
            InitializeComponent();
            settings = programSettings;
            code = text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string tmp_path = Path.GetTempPath() + "\\" + Path.GetRandomFileName();
            Directory.CreateDirectory(tmp_path);
            StreamWriter file = File.CreateText(tmp_path + "\\config.txt");
            file.WriteLine(";!@Install@!UTF-8!");
            file.WriteLine("ExecuteFile=\"cs.exe\"");
            file.WriteLine("ExecuteParameters=\"--import-path .\\imports .\\program.csc " + textBox1.Text + "\"");
            file.WriteLine(";!@InstallEnd@!");
            file.Flush();
            File.WriteAllText(tmp_path + "\\program.csc", code);
            File.Copy(settings.program_path + "\\cs.exe", tmp_path + "\\cs.exe");
            string path = Application.StartupPath;
            string args = "a \"" + tmp_path + "\\out.7z\" \"" + settings.import_path + "\" \"" + tmp_path + "\\*\" -r -mx -mf=BCJ2";
            ProcessStartInfo info = new ProcessStartInfo(path + "\\7zr.exe", args);
            info.RedirectStandardOutput = true;
            info.UseShellExecute = false;
            Process proc = Process.Start(info);
            proc.WaitForExit();
            MessageBox.Show(proc.StandardOutput.ReadToEnd());
            ProcessStartInfo startInfo = new ProcessStartInfo("cmd.exe");
            startInfo.RedirectStandardInput = true;
            startInfo.UseShellExecute = false;
            proc = Process.Start(startInfo);
            proc.StandardInput.WriteLine("copy /b 7zSD.sfx + \"" + tmp_path + "\\config.txt\" + \"" + tmp_path + "\\out.7z\" \"" + tmp_path + "\\out.exe");
            proc.StandardInput.WriteLine("exit");
            saveFileDialog1.ShowDialog();
            File.Copy(tmp_path + "\\out.exe", saveFileDialog1.FileName, true);
            this.Close();
        }
    }
}
