using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace KOASampleCS
{
    class LogManager
    {
        private const string _logFileName = "log.txt";
        private const string _logBackupFileName = "log1.txt";
        private static string _SaveFileName = "";
        private static string _baseDir = "";
        private static Object thisLock = new Object();

        public static string BaseDir
        {
            get 
            {
                if (_baseDir.Length == 0)
                {
                    return System.AppDomain.CurrentDomain.BaseDirectory;
                }
                return _baseDir;
            }

            set
            {
                _baseDir = value;
                if (_baseDir[_baseDir.Length - 1] != '\\')
                {
                    _baseDir += '\\';
                }
            }
        }

        static public void WriteLine(string msg)
        {
            _SaveFileName = System.DateTime.Now.ToString("yyyyMMdd") + ".txt";

            writeFile(msg + "\r");
        }

        static void writeFile(string msg) 
        {
            lock (thisLock)
            {
                msg = "[" + DateTime.Now.ToString() + "] " + msg;
                Trace.WriteLine(msg);

                StreamWriter sw = new StreamWriter(BaseDir + _SaveFileName, true);
                sw.WriteLine(msg);
                sw.Flush();
                sw.Close();
            }
        }

        static public void backupLogFile()
        {
            string logFilePath = BaseDir + _logFileName;
            string logBackupFilePath = BaseDir + _logBackupFileName;

            FileInfo fi = new FileInfo(logFilePath);
            if (fi.Exists == true && fi.Length > 5000000)    // 5MB
            {
                if (File.Exists(logBackupFilePath) == true)
                {
                    File.Delete(logBackupFilePath);
                }

                File.Move(logFilePath, logBackupFilePath);
            }
        }
    }
}
