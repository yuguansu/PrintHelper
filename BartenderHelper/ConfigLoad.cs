using System;
using System.Drawing.Printing;
using System.IO;
using System.Reflection;

namespace BartenderHelper
{
    public class ConfigLoad
    {
        /// <summary>
        /// 获取标签的完整路径
        /// </summary>
        /// <param name="sLabelName">文件名，带后缀，默认为BC_DEFAULT.btw</param>
        /// <returns>返回标签的完整路径</returns>
        public static string GetLabelNameFull(string sLabelName)
        {
            string sPathFolder;
            try
            {
                //获取当前程序集的路径
                sPathFolder = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
                if (string.IsNullOrEmpty(sLabelName))
                {
                    return sPathFolder + "\\" + "BC_DEFAULT.btw";
                }
                else
                {
                    return (sPathFolder + "\\" + sLabelName);
                }
            }
            catch (Exception)
            {
                return "";
            }
        }
        
        /// <summary>
        /// 获取默认打印机名称
        /// </summary>
        /// <returns></returns>
        public static string GetDefaultPrinterName()
        {
            PrintDocument fPrintDoc = new PrintDocument();
            return fPrintDoc.PrinterSettings.PrinterName;
        }

        /// <summary>
        /// 获取本地打印机列表
        /// </summary>
        /// <returns></returns>
        public static string[] GetLocalPrinterList()
        {
            string[] ListPrinter = new string[PrinterSettings.InstalledPrinters.Count];
            //获取当前打印机
            for (int i = 0; i < PrinterSettings.InstalledPrinters.Count; i++)
            {
                ListPrinter[i] = (PrinterSettings.InstalledPrinters[i].ToString());
            }
            return ListPrinter;
        }
    }
}
