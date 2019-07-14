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
        public int font_size;
        public int tab_width;
        public int time_over;
        public void InitDefault()
        {
            work_path = Environment.GetEnvironmentVariable("COVSCRIPT_HOME");
            if (work_path == null)
                work_path = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\CovScript";
            import_path = work_path + "\\imports";
            program_path = work_path + "\\bin";
            log_path = work_path + "\\logs";
            font_size = 12;
            tab_width = 4;
            time_over = 500;
            Directory.CreateDirectory(import_path);
            Directory.CreateDirectory(program_path);
            Directory.CreateDirectory(log_path);
        }
    }
    namespace Configs
    {
        class Urls
        {
            public static string WebSite = "http://covscript.org.cn/";
        }
        class Names
        {
            public static string CsBin = "\\cs.exe";
            public static string CsReplBin = "\\cs_repl.exe";
            public static string CsDbgBin = "\\cs_dbg.exe";
            public static string CsInstBin = "\\cs_inst.exe";
            public static string CsLog = "\\cs_gui.log";
            public static string CsRegistry = "software\\CovScriptGUI";
        }
        class RegistryKey
        {
            public static string BinPath = "BinPath";
            public static string ImportPath = "ImportPath";
            public static string LogPath = "LogPath";
            public static string FontSize = "FontSize";
            public static string TabWidth = "TabWidth";
            public static string TimeOver = "TimeOver";
        }
    }
}
