using System;
using System.IO;
using System.Reflection;

namespace LogHelper
{
    public class LogHelperUtils
    {
        /// <summary>
        /// LOG文件夹
        /// </summary>
        const string LogPathName = "Logs";
        string LogFilePath;
        string LogFileName;
        string FunctionName;
        string FunctionTypeName;

        /// <summary>
        /// Log记录类
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="fun"></param>
        public LogHelperUtils(string Type, string fun)
        {
            FunctionName = fun;
            FunctionTypeName = Type;
            CreateFile();
        }

        private void CreateFile()
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory;

                LogFilePath = path.Substring(0, path.LastIndexOf('\\')) + "\\" + LogPathName + "\\"+ FunctionTypeName + "\\" + FunctionName;

                if (!Directory.Exists(LogFilePath))
                    Directory.CreateDirectory(LogFilePath);
                LogFileName = GetName();
                lock (this)
                {
                    using (FileStream file = new FileStream(LogFileName, FileMode.OpenOrCreate))
                    {
                        file.Close();
                    }
                }
            }
            catch (Exception)
            { }
        }

        private string GetName()
        {
            return LogFilePath + "\\" + DateTime.Now.ToString("yyyyMMdd_HH") + ".txt";
        }

        private void LogWrite(string value)
        {
            if (LogFileName != GetName())
                CreateFile();

            try
            {
                lock (this)
                {
                    using (StreamWriter file = new StreamWriter(LogFileName, true))
                    {
                        file.WriteLine(value);
                        file.Close();
                    }
                }
            }
            catch (Exception)
            { }
        }

        public void WriteLog(string sLogInfo)
        {
            string writeStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "[    ]  " + sLogInfo;
            LogWrite(writeStr);
        }

        public void WriteLog(string fun, string sLogInfo)
        {
            string writeStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "[" + fun + "]  " + sLogInfo;
            LogWrite(writeStr);
        }

        public void WriteLog(string fun, string[] sLogInfo)
        {
            string writeStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "[" + fun + "]  ";
            foreach (var item in sLogInfo)
            {
                writeStr += item + ",";
            }
            LogWrite(writeStr.TrimEnd(',') + "]");
        }

        public void WriteLog(string fun, byte[] sLogInfo)
        {
            string writeStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "[" + fun + "]  ";
            foreach (var item in sLogInfo)
            {
                writeStr += item + ",";
            }
            LogWrite(writeStr.TrimEnd(',') + "]");
        }

        public void WriteLog(string fun, int[] sLogInfo)
        {
            string writeStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "[" + fun + "]  ";
            foreach (var item in sLogInfo)
            {
                writeStr += item + ",";
            }
            LogWrite(writeStr.TrimEnd(',') + "]");
        }

        public void WriteLog(string fun, char[] sLogInfo)
        {
            string writeStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + "[" + fun + "]  ";
            foreach (var item in sLogInfo)
            {
                writeStr += item + ",";
            }
            LogWrite(writeStr.TrimEnd(',') + "]");
        }

        public void WriteLogStart()
        {
            WriteLog("********************Start****************************************");
        }

        public void WriteLogEnd()
        {
            WriteLog("********************End******************************************");
        }
    }
}
