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
            Text = "请稍候....";
            textBox1.Enabled = false;
            textBox2.Enabled = false;
            string tmp_path = Path.GetTempPath() + "\\" + Path.GetRandomFileName();
            Directory.CreateDirectory(tmp_path);
            File.WriteAllText(tmp_path + "\\program.csc", code);
            File.Copy(settings.program_path + "\\cs.exe", tmp_path + "\\cs.exe");
            string opt_files = "";
            if (textBox2.Text.Length > 0)
            {
                foreach (string name in textBox2.Text.Split(';'))
                    if (File.Exists(name))
                        opt_files += "\"" + name + "\" ";
            }
            ProcessStartInfo info0 = new ProcessStartInfo(settings.program_path + "\\7zr.exe")
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = tmp_path,
                Arguments = "a out.7z \"" + settings.import_path + "\" * " + opt_files + "-r -mx -mf=BCJ2"
            };
            Process proc0 = Process.Start(info0);
            proc0.WaitForExit();
            File.Copy(settings.program_path + "\\7zS.sfx", tmp_path + "\\7zS.sfx");
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
                CreateNoWindow = true,
                WorkingDirectory = tmp_path
            };
            Process proc1 = Process.Start(info1);
            proc1.StandardInput.WriteLine("copy /b \"" + settings.program_path + "\\7zS.sfx\" + config.txt + out.7z out.exe");
            proc1.StandardInput.WriteLine("exit");
            proc1.WaitForExit();
            Close();
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(saveFileDialog1.FileName))
                    File.Delete(saveFileDialog1.FileName);
                File.Move(tmp_path + "\\out.exe", saveFileDialog1.FileName);
                MessageBox.Show("输出成功: " + saveFileDialog1.FileName, "Covariant Script GUI", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
