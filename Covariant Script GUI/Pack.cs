using System;
using System.Diagnostics;
using System.IO;
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
            File.WriteAllText(tmp_path + "\\program.csc", code);
            File.Copy(settings.program_path + "\\cs.exe", tmp_path + "\\cs.exe");
            string path = Application.StartupPath;
            string opt_files = "";
            foreach (string name in textBox2.Text.Split(';'))
                opt_files += "\"" + name + "\" ";
            ProcessStartInfo info0 = new ProcessStartInfo(path + "\\7zr.exe")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                Arguments = "a \"" + tmp_path + "\\out.7z\" \"" + settings.import_path + "\" \"" + tmp_path + "\\*\" " + opt_files + "-r -mx -mf=BCJ2"
            };
            Process proc0 = Process.Start(info0);
            proc0.WaitForExit();
            StreamWriter file = File.CreateText(tmp_path + "\\config.txt");
            file.WriteLine(";!@Install@!UTF-8!");
            file.WriteLine("ExecuteFile=\"cs.exe\"");
            file.WriteLine("ExecuteParameters=\"--import-path .\\imports .\\program.csc " + textBox1.Text + "\"");
            file.WriteLine(";!@InstallEnd@!");
            file.Close();
            ProcessStartInfo info1 = new ProcessStartInfo("cmd.exe")
            {
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            Process proc1 = Process.Start(info1);
            proc1.StandardInput.WriteLine("copy /b 7zS.sfx + \"" + tmp_path + "\\config.txt\" + \"" + tmp_path + "\\out.7z\" \"" + tmp_path + "\\out.exe");
            proc1.StandardInput.WriteLine("exit");
            proc1.WaitForExit();
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(saveFileDialog1.FileName))
                    File.Delete(saveFileDialog1.FileName);
                File.Move(tmp_path + "\\out.exe", saveFileDialog1.FileName);
            }
            Close();
        }
    }
}
