using System;
using System.Collections.Generic;
using Seagull.BarTender.Print;
/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 日期：2017-12-23
 * 版本：1.0.0.0
 * 人员：铅笔
 * 内容：使用bartender 2016 sdk开发
 * 注意：必须选择x86模式编译
 * 
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
namespace BartenderHelper
{
    public class BartenderHeperUtils
    {
        #region 构造方法
        /// <summary>
        /// 构造函数
        /// </summary>
        private BartenderHeperUtils()
        {

        }
        #endregion

        #region 变量定义

        //定义必要的变量
        /// <summary>
        /// 打印引擎
        /// </summary>
        private static Engine btEngine = new Engine(true);
        /// <summary>
        /// 标签文档
        /// </summary>
        private static LabelFormatDocument btFormatDoc { get; set; }
        /// <summary>
        /// 标签路径
        /// </summary>
        public static string sLabelNameFull { get; set; }
        /// <summary>
        /// 打印机名称
        /// </summary>
        public static string sPrinterName { get; set; }
        /// <summary>
        /// 单个标签内容连续打印次数
        /// </summary>
        public static int iQtySingleLabel { get; set; }
        /// <summary>
        /// 整体重复次数
        /// </summary>
        public static int iQtyLot { get; set; }

        /// <summary>
        /// 记录日志
        /// </summary>
        public static WriteLogs.WriteLog Log = new WriteLogs.WriteLog("Print","BartenderHelper");
        #endregion

        #region 打印方法
        
        /// <summary>
        /// 打印开始，公用程序，启动打印引擎/获取标签路径/打印机名称/加载标签文档
        /// </summary>
        /// <param name="sLabel">标签名称，可为空，默认值为BC_DEFAULT.btw</param>
        /// <param name="sPrinter">打印机名称，可为空，默认值为系统默认打印机</param>
        private static bool PrintLabelStart(string sLabel, string sPrinter)
        {
            try
            {
                //打印任务开始
                Log.WriteLogsStart();
                //启动打印引擎
                btEngine.Start();
                Log.WriteLogs("--Print Engine Start OK--");

                //获取标签完整路径
                sLabelNameFull = ConfigLoad.GetLabelNameFull(sLabel);
                Log.WriteLogs("--Get full label path : " + sLabelNameFull + "--");
                //获取打印机名称
                sPrinterName = string.IsNullOrEmpty(sPrinter) ? ConfigLoad.GetDefaultPrinterName() : sPrinter;
                Log.WriteLogs("--Printer : " + sPrinterName);
                //加载标签模板，指定打印机
                btFormatDoc = btEngine.Documents.Open(sLabelNameFull, sPrinterName);
                Log.WriteLogs("--FormatDoc opened--");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 打印结束，公用程序，关闭标签文档/停止打印引擎
        /// </summary>
        private static bool PrintLabelEnd()
        {
            try
            {
                //关闭标签模板
                btFormatDoc.Close(SaveOptions.DoNotSaveChanges);
                Log.WriteLogs("--FormatDocument closed--");
                //停止打印引擎
                btEngine.Stop();
                Log.WriteLogs("--Print Engine Stop--");
                Log.WriteLogsEnd();
                //打印任务结束
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        /// <summary>
        /// 打印单个标签
        /// </summary>
        /// <param name="sLabel">标签名称，可为空，默认值为BC_DEFAULT.btw</param>
        /// <param name="sPrinter">打印机名称，可为空，默认值为系统默认打印机</param>
        public static bool PrintLabel(string sLabel,string sPrinter)
        {
            //记录参数
            Log.WriteLogs("--Params : " + sLabel + ";" + sPrinter + ";");
            try
            {
                //开始打印
                if (!PrintLabelStart(sLabel, sPrinter))
                {
                    return false;
                }

                //打印标签
                Log.WriteLogs("--Print Start : PrintJob1--");
                btFormatDoc.Print("PrintJob1");
                Log.WriteLogs("--Print Label OK : " + sLabelNameFull);

                //结束打印
                PrintLabelEnd();

                return true;
            }
            catch (Exception ex)
            {
                Log.WriteLogs("--[Execute error]--public static bool PrintLabel(string sLabel,string sPrinter)--");
                Log.WriteLogs("--[System Error Msg]--" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 打印单个标签内容，标签中只有一个变量
        /// </summary>
        /// <param name="sLabel">标签名称，可为空，默认值为BC_DEFAULT.btw</param>
        /// <param name="sPrinter">打印机名称，可为空，默认值为系统默认打印机</param>
        /// <param name="sSubstringName">标签内的变量名称</param>
        /// <param name="sSubstringValue">标签内的变量值</param>
        public static bool PrintLabel(string sLabel, string sPrinter,string sSubstringName, string sSubstringValue)
        {
            Log.WriteLogsStart();
            Log.WriteLogs("--Call Function [public static bool PrintLabel("+ sLabel+" , "+ sPrinter + ", "+ sSubstringName + " , "+ sSubstringValue + " , 1 )]--");
            return PrintLabel(sLabel, sPrinter, sSubstringName, sSubstringValue, 1);
        }

        /// <summary>
        /// 打印单个标签内容，标签中只有一个变量，指定单个标签内容的重复打印数量
        /// </summary>
        /// <param name="sLabel">标签名称，可为空，默认值为BC_DEFAULT.btw</param>
        /// <param name="sPrinter">打印机名称，可为空，默认值为系统默认打印机</param>
        /// <param name="sSubstringName">标签内的变量名称</param>
        /// <param name="sSubstringValue">标签内的变量值</param>
        /// <param name="iLabelQty">单个标签内容的连续打印数量</param>
        /// <returns></returns>
        public static bool PrintLabel(string sLabel, string sPrinter, string sSubstringName, string sSubstringValue,int iLabelQty)
        {
            //记录参数
            Log.WriteLogs("--Params : " + sLabel + ";" + sPrinter + ";" + sSubstringName + ";" + sSubstringValue + ";" + iLabelQty + ";" + "--");
            try
            {
                //开始打印
                if (!PrintLabelStart(sLabel, sPrinter))
                {
                    return false;
                }

                //更改标签内容
                btFormatDoc.SubStrings[sSubstringName].Value = sSubstringValue;
                Log.WriteLogs("--Value in label : " + sSubstringName + " = " + sSubstringValue);

                //更改打印数量
                try
                {
                    iQtySingleLabel = iLabelQty > 0 ? iLabelQty : 1;
                }
                catch (Exception)
                {
                    iQtySingleLabel = 1;
                }
                btFormatDoc.PrintSetup.IdenticalCopiesOfLabel = iQtySingleLabel;
                Log.WriteLogs("--Qty of single label : "+ iQtySingleLabel);

                //打印标签
                Log.WriteLogs("--Print Start : PrintJob1--");
                btFormatDoc.Print("PrintJob1");
                Log.WriteLogs("--Print Label OK : " + sLabelNameFull);

                //结束打印
                PrintLabelEnd();

                return true;
            }
            catch (Exception ex)
            {
                Log.WriteLogs("--[Execute error]--public static bool PrintLabel(string sLabel, string sPrinter, string sSubstringName, string sSubstringValue,int iLabelQty)--");
                Log.WriteLogs("--[System Error Msg]--" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 打印多个标签内容，标签中只有一个变量
        /// </summary>
        /// <param name="sLabel">标签名称，可为空，默认值为BC_DEFAULT.btw</param>
        /// <param name="sPrinter">打印机名称，可为空，默认值为系统默认打印机</param>
        /// <param name="sSubstringName">标签内的变量名称</param>
        /// <param name="lstValue">标签内的变量值</param>
        /// <returns></returns>
        public static bool PrintLabel(string sLabel, string sPrinter, string sSubstringName, List<string> lstValue)
        {
            string slistValue = string.Empty;
            foreach (var item in lstValue)
            {
                slistValue += "{";
                slistValue += item + ",";
                slistValue += "}";
            }
            Log.WriteLogsStart();
            Log.WriteLogs("--Call Function [public static bool PrintLabel(" + sLabel + " , " + sPrinter + " , " + sSubstringName + " , " + "1 )]--");
            return PrintLabel(sLabel, sPrinter, sSubstringName, lstValue, 1);
        }

        /// <summary>
        /// 打印多个标签内容，标签中只有一个变量，指定单个标签内容的重复打印数量
        /// </summary>
        /// <param name="sLabel">标签名称，可为空，默认值为BC_DEFAULT.btw</param>
        /// <param name="sPrinter">打印机名称，可为空，默认值为系统默认打印机</param>
        /// <param name="sSubstringName">标签内的变量名称</param>
        /// <param name="lstValue">标签内的变量值</param>
        /// <param name="iLabelQty">单个标签内容的连续打印数量</param>
        /// <returns></returns>
        public static bool PrintLabel(string sLabel, string sPrinter, string sSubstringName, List<string> lstValue,int iLabelQty)
        {
            string listValue = string.Empty;
            foreach (var item in lstValue)
            {
                listValue += "{";
                listValue += item + ",";
                listValue += "}";
            }
            //记录参数
            Log.WriteLogs("--Params : " + sLabel + ";" + sPrinter + ";" + sSubstringName + ";" + listValue + ";" + iLabelQty + ";" + "--");
            try
            {
                //开始打印
                if (!PrintLabelStart(sLabel, sPrinter))
                {
                    return false;
                }

                //更改打印数量
                try
                {
                    iQtySingleLabel = iLabelQty > 0 ? iLabelQty : 1;
                }
                catch (Exception)
                {
                    iQtySingleLabel = 1;
                }
                btFormatDoc.PrintSetup.IdenticalCopiesOfLabel = iQtySingleLabel;
                Log.WriteLogs("--Qty of single label : " + iQtySingleLabel);

                //更改标签内容
                for (int iValue = 0; iValue < lstValue.Count; iValue++)
                {
                    btFormatDoc.SubStrings[sSubstringName].Value = lstValue[iValue];
                    Log.WriteLogs("--Value in label : " + sSubstringName + " = " + lstValue[iValue]);
                    //打印标签
                    Log.WriteLogs("--PrintJob Start : PrintJob" + iValue + "--");
                    btFormatDoc.Print("PrintJob" + iValue);
                    Log.WriteLogs("--PrintJob finish : PrintJob" + iValue + "--");
                }

                //结束打印
                PrintLabelEnd();

                return true;
            }
            catch (Exception ex)
            {
                Log.WriteLogs("--[Execute error]--public static bool PrintLabel(string sLabel, string sPrinter, string sSubstringName, List<string> lstValue,int iLabelQty)--");
                Log.WriteLogs("--[System Error Msg]--" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 打印单个标签内容，标签中有多个变量
        /// </summary>
        /// <param name="sLabel">标签名称，可为空，默认值为BC_DEFAULT.btw</param>
        /// <param name="sPrinter">打印机名称，可为空，默认值为系统默认打印机</param>
        /// <param name="lstSubstringName">标签内的变量名称</param>
        /// <param name="lstValue">标签内的变量值</param>
        /// <returns></returns>
        public static bool PrintLabel(string sLabel, string sPrinter, List<string> lstSubstringName, List<string> lstValue)
        {
            //整理参数
            string sSubstringName = string.Empty;
            foreach (var item in lstSubstringName)
            {
                sSubstringName += item + ",";
            }
            string slistValue = string.Empty;
            foreach (var item in lstValue)
            {
                slistValue += item + ",";
            }

            Log.WriteLogsStart();
            Log.WriteLogs("--Call Function [public static bool PrintLabel(" + sLabel + " , " + sPrinter + " , " + sSubstringName + " , " + slistValue + ", 1]--");
            return PrintLabel(sLabel, sPrinter, lstSubstringName, lstValue, 1);
        }

        /// <summary>
        /// 打印单个标签内容，标签中有多个变量，指定单个标签内容的重复打印数量
        /// </summary>
        /// <param name="sLabel">标签名称，可为空，默认值为BC_DEFAULT.btw</param>
        /// <param name="sPrinter">打印机名称，可为空，默认值为系统默认打印机</param>
        /// <param name="lstSubstringName">标签内的变量名称</param>
        /// <param name="lstValue">标签内的变量值</param>
        /// <param name="iLabelQty">单个标签内容的连续打印数量</param>
        /// <returns></returns>
        public static bool PrintLabel(string sLabel, string sPrinter, List<string> lstSubstringName, List<string> lstValue, int iLabelQty)
        {
            #region //记录参数
            string sSubstringName = string.Empty;
            foreach (var item in lstSubstringName)
            {
                sSubstringName += item + ",";
            }
            string slistValue = string.Empty;
            foreach (var item in lstValue)
            {
                slistValue += item + ",";
            }
            Log.WriteLogs("--Params : " + sLabel + ";" + sPrinter + ";" + sSubstringName + ";" + slistValue + ";" + iLabelQty + "--");
            #endregion
            try
            {
                //开始打印
                if (!PrintLabelStart(sLabel, sPrinter))
                {
                    return false;
                }

                //更改打印数量
                try
                {
                    iQtySingleLabel = iLabelQty > 0 ? iLabelQty : 1;
                }
                catch (Exception)
                {
                    iQtySingleLabel = 1;
                }
                btFormatDoc.PrintSetup.IdenticalCopiesOfLabel = iQtySingleLabel;
                Log.WriteLogs("--Qty of single label : " + iQtySingleLabel);

                //更改标签内容
                for (int iValue = 0; iValue < lstSubstringName.Count; iValue++)
                {
                    btFormatDoc.SubStrings[lstSubstringName[iValue]].Value = lstValue[iValue];
                    Log.WriteLogs("--Value in label : " + sSubstringName + " = " + lstValue[iValue]);
                }
                //打印标签
                Log.WriteLogs("--PrintJob Start : PrintJob1" + "--");
                btFormatDoc.Print("PrintJob1");
                Log.WriteLogs("--PrintJob finish : PrintJob1");

                //结束打印
                PrintLabelEnd();

                return true;
            }
            catch (Exception ex)
            {
                Log.WriteLogs("--[Execute error]--public static bool PrintLabel(string sLabel, string sPrinter, List<string> lstSubstringName, List<string> lstValue, int iLabelQty)--");
                Log.WriteLogs("--[System Error Msg]--" + ex.Message);

                return false;
            }

        }

        /// <summary>
        /// 打印多个标签内容，标签中有多个变量
        /// </summary>
        /// <param name="sLabel">标签名称，可为空，默认值为BC_DEFAULT.btw</param>
        /// <param name="sPrinter">打印机名称，可为空，默认值为系统默认打印机</param>
        /// <param name="lstSubstringName">标签内的变量名称</param>
        /// <param name="lstValue">标签内的变量值</param>
        /// <returns></returns>
        public static bool PrintLabel(string sLabel, string sPrinter,List<string> lstSubstringName, List<List<string>> lstValue)
        {
            //整理参数
            string sSubstringName = string.Empty;
            foreach (var item in lstSubstringName)
            {
                sSubstringName += item + ",";
            }
            string slistValue = string.Empty;
            foreach (var item1 in lstValue)
            {
                slistValue += "{";
                foreach (var item2 in item1)
                {
                    slistValue += item2 + ",";
                }
                slistValue += "}";
            }
            Log.WriteLogs("--Params : " + sLabel + ";" + sPrinter + ";" + sSubstringName + ";" + slistValue + ";" + "--");

            Log.WriteLogsStart();
            Log.WriteLogs("--Call Function [public static bool PrintLabel(" + sLabel + " , " + sPrinter + " , " + sSubstringName + " , " + slistValue + ", 1]--");
            return PrintLabel(sLabel, sPrinter, lstSubstringName, lstValue, 1);
        }

        /// <summary>
        /// 打印多个标签内容，标签中有多个变量，指定单个标签内容的重复打印数量
        /// </summary>
        /// <param name="sLabel">标签名称，可为空，默认值为BC_DEFAULT.btw</param>
        /// <param name="sPrinter">打印机名称，可为空，默认值为系统默认打印机</param>
        /// <param name="lstSubstringName">标签内的变量名称</param>
        /// <param name="lstValue">标签内的变量值</param>
        /// <param name="iLabelQty">单个标签内容的连续打印数量</param>
        /// <returns></returns>
        public static bool PrintLabel(string sLabel, string sPrinter, List<string> lstSubstringName, List<List<string>> lstValue, int iLabelQty)
        {
            #region //记录参数
            string sSubstringName = string.Empty;
            foreach (var item in lstSubstringName)
            {
                sSubstringName += item + ",";
            }
            string slistValue = string.Empty;
            foreach (var item1 in lstValue)
            {
                slistValue += "{";
                foreach (var item2 in item1)
                {
                    slistValue += item2 + ",";
                }
                slistValue += "}";
            }

            Log.WriteLogs("--Params : " + sLabel + ";" + sPrinter + ";" + sSubstringName + ";" + slistValue + ";" + iLabelQty + ";" + "--");
            #endregion

            try
            {
                //开始打印
                if (!PrintLabelStart(sLabel, sPrinter))
                {
                    return false;
                }

                //更改打印数量
                try
                {
                    iQtySingleLabel = iLabelQty > 0 ? iLabelQty : 1;
                }
                catch (Exception)
                {
                    iQtySingleLabel = 1;
                }
                btFormatDoc.PrintSetup.IdenticalCopiesOfLabel = iQtySingleLabel;
                Log.WriteLogs("--Qty of single label : " + iQtySingleLabel);

                //更改标签内容并打印
                for (int iValue = 0; iValue < lstValue.Count; iValue++)
                {
                    for (int iName = 0; iName < lstSubstringName.Count; iName++)
                    {
                        btFormatDoc.SubStrings[lstSubstringName[iName]].Value = lstValue[iValue][iName];
                        Log.WriteLogs("--Value in label : " + lstSubstringName[iName] + " = " + lstValue[iValue][iName]);
                    }
                    Log.WriteLogs("--PrintJob Start : PrintJob" + iValue + "--");
                    btFormatDoc.Print("PrintJob" + iValue);
                    Log.WriteLogs("--PrintJob finish : PrintJob" + iValue);
                }

                //结束打印
                PrintLabelEnd();

                return true;
            }
            catch (Exception ex)
            {
                Log.WriteLogs("--[Execute error]--public static bool PrintLabel(string sLabel, string sPrinter,List<string> lstSubstringName, List<List<string>> lstValue)--");
                Log.WriteLogs("--[System Error Msg]--" + ex.Message);

                return false;
            }
        }

        #endregion

    }
}
