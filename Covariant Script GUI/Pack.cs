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

        public static void merge_files(string dest, string[] source)
        {
            FileStream stream = new FileStream(dest, FileMode.OpenOrCreate);
            byte[] buffer = new byte[1024];
            foreach (string src in source)
            {
                FileStream file = new FileStream(src, FileMode.Open);
                int count = 0;
                while ((count = file.Read(buffer, 0, buffer.Length)) > 0)
                    stream.Write(buffer, 0, count);
            }
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
            ProcessStartInfo info = new ProcessStartInfo(settings.program_path + "\\7zr.exe")
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = tmp_path,
                Arguments = "a out.7z \"" + settings.import_path + "\" * " + opt_files + "-r -mx -mf=BCJ2"
            };
            Process.Start(info).WaitForExit();
            File.Copy(settings.program_path + "\\7zS.sfx", tmp_path + "\\7zS.sfx");
            StreamWriter file = File.CreateText(tmp_path + "\\config.txt");
            file.WriteLine(";!@Install@!UTF-8!");
            file.WriteLine("ExecuteFile=\"cs.exe\"");
            file.WriteLine("ExecuteParameters=\"--import-path .\\imports .\\program.csc " + textBox1.Text + "\"");
            file.WriteLine(";!@InstallEnd@!");
            file.Close();
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(saveFileDialog1.FileName))
                    File.Delete(saveFileDialog1.FileName);
                string[] vs = { settings.program_path + "\\7zS.sfx", tmp_path + "\\config.txt", tmp_path + "\\out.7z" };
                merge_files(saveFileDialog1.FileName, vs);
                MessageBox.Show("输出成功: " + saveFileDialog1.FileName, "Covariant Script GUI", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            Close();
        }
    }
}
