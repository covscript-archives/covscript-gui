using System;
using System.IO;

namespace Covariant_Script
{
    /// <summary>
    /// 程序设置
    /// </summary>
    public class ProgramSettings
    {
        public string work_path;
        public string import_path;
        public string program_path;
        public string log_path;
        public int font_size = 12;
        public ProgramSettings()
        {
            work_path = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\CovScriptGUI";
            import_path = work_path + "\\Imports";
            program_path = work_path + "\\Bin";
            log_path = work_path + "\\Logs";
            Directory.CreateDirectory(import_path);
            Directory.CreateDirectory(program_path);
            Directory.CreateDirectory(log_path);
        }
    }
}
