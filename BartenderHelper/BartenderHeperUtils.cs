using System;
using System.Collections.Generic;
using Seagull.BarTender.Print;

/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
/// 日期：2017-12-23
/// * 版本：1.0.0.0
/// * 人员：铅笔
/// * 内容：使用bartender 2016 sdk开发
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
namespace BartenderHelper
{
    /// <summary>
    /// 直接调用静态方法打印，无需外部初始化，自带日志记录
    /// * 注意：必须选择x86模式编译
    /// * 使用方式：打印参数设置好后，直接调用静态函数打印
    /// * 弊端：1.打印engine启动和停止时间较长，调试机启动大约6秒。
    ///         2.无法作为服务调用。
    /// </summary>
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
        public static int iQtyLabel { get; set; }
        /// <summary>
        /// 整体重复次数
        /// </summary>
        public static int iQtyLot { get; set; }

        /// <summary>
        /// 记录日志
        /// </summary>
        public static LogHelper.LogHelperUtils Log = new LogHelper.LogHelperUtils("Print", "BartenderHeperUtils");
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
                Log.WriteLogStart();
                //启动打印引擎
                btEngine.Start();
                Log.WriteLog("--Print Engine Start OK--");

                //获取标签完整路径
                sLabelNameFull = ConfigLoad.GetLabelNameFull(sLabel);
                Log.WriteLog("--Get full label path : " + sLabelNameFull + "--");
                //获取打印机名称
                sPrinterName = string.IsNullOrEmpty(sPrinter) ? ConfigLoad.GetDefaultPrinterName() : sPrinter;
                Log.WriteLog("--Printer : " + sPrinterName);
                //加载标签模板，指定打印机
                btFormatDoc = btEngine.Documents.Open(sLabelNameFull, sPrinterName);
                Log.WriteLog("--FormatDoc opened--");
                return true;
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex.Message);
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
                Log.WriteLog("--FormatDocument closed--");
                //停止打印引擎
                btEngine.Stop();
                Log.WriteLog("--Print Engine Stop--");
                Log.WriteLogEnd();
                //打印任务结束
                return true;
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex.Message);
                return false;
            }
        }
        
        /// <summary>
        /// 打印单个标签，直接打印一次，无参数
        /// </summary>
        /// <param name="sLabel">标签名称，可为空，默认值为BC_DEFAULT.btw</param>
        /// <param name="sPrinter">打印机名称，可为空，默认值为系统默认打印机</param>
        public static bool PrintLabel(string sLabel,string sPrinter)
        {
            Log.WriteLog("--Call Function [ PrintLabel(string sLabel, string sPrinter,int iRepeatNumberOfSingleLabel) ]--");
            return PrintLabel(sLabel, sPrinter,1);
        }

        /// <summary>
        /// 打印单个标签，无参数，指定打印次数
        /// </summary>
        /// <param name="sLabel">标签名称，可为空，默认值为BC_DEFAULT.btw</param>
        /// <param name="sPrinter">打印机名称，可为空，默认值为系统默认打印机</param>
        /// <param name="iRepeatNumberOfSingleLabel">单个标签的连续打印数量</param>
        public static bool PrintLabel(string sLabel, string sPrinter,int iRepeatNumberOfSingleLabel)
        {
            //记录参数
            Log.WriteLog("-- Params : Label = " + sLabel + "; Print = " + sPrinter + ";");
            try
            {
                //开始打印
                if (!PrintLabelStart(sLabel, sPrinter))
                {
                    return false;
                }
                try
                {
                    iQtyLabel = iRepeatNumberOfSingleLabel > 0 ? iRepeatNumberOfSingleLabel : 1;
                }
                catch (Exception)
                {
                    iQtyLabel = 1;
                }
                //设置重复打印数量
                btFormatDoc.PrintSetup.IdenticalCopiesOfLabel = iQtyLabel;
                //打印标签
                Log.WriteLog("--Print Start : PrintJob1--");
                btFormatDoc.Print("PrintJob1");
                Log.WriteLog("--Print Label OK : " + sLabelNameFull);

                //结束打印
                PrintLabelEnd();

                return true;
            }
            catch (Exception ex)
            {
                Log.WriteLog("--[Execute error]--public static bool PrintLabel(string sLabel,string sPrinter)--");
                Log.WriteLog("--[System Error Msg]--" + ex.Message);
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
            Log.WriteLog("--Call Function [ PrintLabel("+ sLabel+" , "+ sPrinter + ", "+ sSubstringName + " , "+ sSubstringValue + " , 1 ) ]--");
            return PrintLabel(sLabel, sPrinter, sSubstringName, sSubstringValue, 1);
        }

        /// <summary>
        /// 打印单个标签内容，标签中只有一个变量，指定单个标签内容的重复打印数量
        /// </summary>
        /// <param name="sLabel">标签名称，可为空，默认值为BC_DEFAULT.btw</param>
        /// <param name="sPrinter">打印机名称，可为空，默认值为系统默认打印机</param>
        /// <param name="sSubstringName">标签内的变量名称</param>
        /// <param name="sSubstringValue">标签内的变量值</param>
        /// <param name="iRepeatNumberOfSingleLabel">单个标签内容的连续打印数量</param>
        /// <returns></returns>
        public static bool PrintLabel(string sLabel, string sPrinter, string sSubstringName, string sSubstringValue,int iRepeatNumberOfSingleLabel)
        {
            Log.WriteLog("public static bool PrintLabel(string sLabel, string sPrinter, string sSubstringName, List<string> lstValue,int iRepeatNumberOfSingleLabel)");
            List<string> lstValue = new List<string> { };
            lstValue.Add(sSubstringValue);
            return PrintLabel(sLabel, sPrinter, sSubstringName, lstValue, iRepeatNumberOfSingleLabel);
        }

        /// <summary>
        /// 打印多个不同标签，标签模板中只有一个变量
        /// </summary>
        /// <param name="sLabel">标签名称，可为空，默认值为BC_DEFAULT.btw</param>
        /// <param name="sPrinter">打印机名称，可为空，默认值为系统默认打印机</param>
        /// <param name="sSubstringName">标签内的变量名称</param>
        /// <param name="lstValue">标签内的变量值</param>
        /// <returns></returns>
        public static bool PrintLabel(string sLabel, string sPrinter, string sSubstringName, List<string> lstValue)
        {
            Log.WriteLog("public static bool PrintLabel(string sLabel, string sPrinter, string sSubstringName, List<string> lstValue,int iRepeatNumberOfSingleLabel)");
            return PrintLabel(sLabel, sPrinter, sSubstringName, lstValue, 1);
        }

        /// <summary>
        /// 打印多个不同标签，标签模板中只有一个变量，指定重复打印数量
        /// </summary>
        /// <param name="sLabel">标签名称，可为空，默认值为BC_DEFAULT.btw</param>
        /// <param name="sPrinter">打印机名称，可为空，默认值为系统默认打印机</param>
        /// <param name="sSubstringName">标签内的变量名称</param>
        /// <param name="lstValue">标签内的变量值</param>
        /// <param name="iRepeatNumberOfSingleLabel">单个标签内容的连续打印数量</param>
        /// <returns></returns>
        public static bool PrintLabel(string sLabel, string sPrinter, string sSubstringName, List<string> lstValue,int iRepeatNumberOfSingleLabel)
        {
            Log.WriteLog("--Call Function [ PrintLabel(string sLabel, string sPrinter, List<string> lstSubstringName, List<List<string>> lstValue, int iRepeatNumberOfSingleLabel) ]--");
            List<string> lstSubstringName = new List<string> { };
            lstSubstringName.Add(sSubstringName);
            List<List<string>> lstValue2 = new List<List<string>> { };
            foreach (var value in lstValue)
            {
                List<string> newValue = new List<string> { };
                newValue.Add(value);
                lstValue2.Add(newValue);
            }
            return PrintLabel(sLabel, sPrinter, lstSubstringName, lstValue, 1);
        }

        /// <summary>
        /// 打印单个标签内容，标签模板中有多个变量
        /// </summary>
        /// <param name="sLabel">标签名称，可为空，默认值为BC_DEFAULT.btw</param>
        /// <param name="sPrinter">打印机名称，可为空，默认值为系统默认打印机</param>
        /// <param name="lstSubstringName">标签内的变量名称</param>
        /// <param name="lstValue">标签内的变量值</param>
        /// <returns></returns>
        public static bool PrintLabel(string sLabel, string sPrinter, List<string> lstSubstringName, List<string> lstValue)
        {
            Log.WriteLog("PrintLabel(string sLabel, string sPrinter, List<string> lstSubstringName, List<string> lstValue, int iRepeatNumberOfSingleLabel)");
            return PrintLabel(sLabel, sPrinter, lstSubstringName, lstValue, 1);
        }

        /// <summary>
        /// 打印单个标签内容，标签模板中有多个变量，指定单个标签内容的重复打印数量
        /// </summary>
        /// <param name="sLabel">标签名称，可为空，默认值为BC_DEFAULT.btw</param>
        /// <param name="sPrinter">打印机名称，可为空，默认值为系统默认打印机</param>
        /// <param name="lstSubstringName">标签内的变量名称</param>
        /// <param name="lstValue">标签内的变量值</param>
        /// <param name="iRepeatNumberOfSingleLabel">单个标签内容的连续打印数量</param>
        /// <returns></returns>
        public static bool PrintLabel(string sLabel, string sPrinter, List<string> lstSubstringName, List<string> lstValue, int iRepeatNumberOfSingleLabel)
        {
            Log.WriteLog("--Call Function [ PrintLabel(string sLabel, string sPrinter, List<string> lstSubstringName, List<List<string>> lstValue, int iRepeatNumberOfSingleLabel) ]--");
            List<List<string>> lstValue2 = new List<List<string>> { };
            lstValue2.Add(lstValue);
            return PrintLabel(sLabel, sPrinter, lstSubstringName, lstValue2, 1);
        }

        /// <summary>
        /// 打印多个不同标签内容，标签模板中有多个变量
        /// </summary>
        /// <param name="sLabel">标签名称，可为空，默认值为BC_DEFAULT.btw</param>
        /// <param name="sPrinter">打印机名称，可为空，默认值为系统默认打印机</param>
        /// <param name="lstSubstringName">标签内的变量名称</param>
        /// <param name="lstValue">标签内的变量值</param>
        /// <returns></returns>
        public static bool PrintLabel(string sLabel, string sPrinter,List<string> lstSubstringName, List<List<string>> lstValue)
        {
            Log.WriteLog("--Call Function [PrintLabel(string sLabel, string sPrinter, List<string> lstSubstringName, List<List<string>> lstValue, int iRepeatNumberOfSingleLabel) ]--");
            return PrintLabel(sLabel, sPrinter, lstSubstringName, lstValue, 1);
        }

        /// <summary>
        /// 打印多个标签内容，标签中有多个变量，指定单个标签内容的重复打印数量
        /// </summary>
        /// <param name="sLabel">标签名称，可为空，默认值为BC_DEFAULT.btw</param>
        /// <param name="sPrinter">打印机名称，可为空，默认值为系统默认打印机</param>
        /// <param name="lstSubstringName">标签内的变量名称</param>
        /// <param name="lstValue">标签内的变量值</param>
        /// <param name="iRepeatNumberOfSingleLabel">单个标签的连续打印数量</param>
        /// <returns></returns>
        public static bool PrintLabel(string sLabel, string sPrinter, List<string> lstSubstringName, List<List<string>> lstValue, int iRepeatNumberOfSingleLabel)
        {
            Log.WriteLog("--Call Function [ PrintLabel(string sLabel, string sPrinter, List<string> lstSubstringName, List<List<string>> lstValue, int iRepeatNumberOfSingleLabel,int iRepeatNumberOfLot) ]--");
            return PrintLabel(sLabel, sPrinter, lstSubstringName, lstValue, 1,1);
        }

        /// <summary>
        /// 打印多个标签内容，标签中有多个变量，指定单个标签重复打印数量，指定批次重复数量
        /// </summary>
        /// <param name="sLabel">标签名称，可为空，默认值为BC_DEFAULT.btw</param>
        /// <param name="sPrinter">打印机名称，可为空，默认值为系统默认打印机</param>
        /// <param name="lstSubStringName">标签内的变量名称</param>
        /// <param name="lstValue">标签内的变量值</param>
        /// <param name="iRepeatNumberOfSingleLabel">单个标签的连续打印数量</param>
        /// <param name="iRepeatNumberOfLot">整体标签批次重复的次数</param>
        /// <returns></returns>
        public static bool PrintLabel(string sLabel, string sPrinter, List<string> lstSubStringName, List<List<string>> lstValue, int iRepeatNumberOfSingleLabel,int iRepeatNumberOfLot)
        {
            #region //记录参数
            string sSubstringName = string.Empty;
            foreach (var item in lstSubStringName)
            {
                sSubstringName += "{";
                sSubstringName += item + ",";
                sSubstringName += "}";
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
            //参数写LOG
            Log.WriteLog("--Params : "
                + "Label=" + sLabel + ";Printer=" + sPrinter + ";"
                + "SubstringName=" + sSubstringName + ";SubstringValue=" + slistValue + ";"
                + "Qty=" + iRepeatNumberOfSingleLabel + ";"
                + " --");
            #endregion

            try
            {
                //开始打印
                if (!PrintLabelStart(sLabel, sPrinter)) return false;

                //更改单个标签打印数量，默认为1
                try
                {
                    iQtyLabel = iRepeatNumberOfSingleLabel > 0 ? iRepeatNumberOfSingleLabel : 1;
                }
                catch (Exception)
                {
                    iQtyLabel = 1;
                }
                btFormatDoc.PrintSetup.IdenticalCopiesOfLabel = iQtyLabel;
                Log.WriteLog("-- Label repeat Qty = " + iQtyLabel + "; --");

                //更改打印批次数量，默认为1
                try
                {
                    iQtyLot = iRepeatNumberOfLot > 0 ? iRepeatNumberOfLot : 1;
                }
                catch (Exception)
                {
                    iQtyLot = 1;
                }
                Log.WriteLog("-- Lot repeat qty = " + iQtyLot + "; --");
                for (int iLot = 0; iLot < iQtyLot; iLot++)
                {
                    //更改标签内容并打印
                    for (int iValue = 0; iValue < lstValue.Count; iValue++)
                    {
                        string sLabelValue = "[";
                        for (int iName = 0; iName < lstSubStringName.Count; iName++)
                        {
                            btFormatDoc.SubStrings[lstSubStringName[iName]].Value = lstValue[iValue][iName];
                            sLabelValue += lstSubStringName[iName] + "=" + lstValue[iValue][iName] + ";";
                        }
                        sLabelValue += "]";
                        if (iLot == 0)
                        {
                            Log.WriteLog("-- Label " + iValue + ":" + sLabelValue + " --");
                            Log.WriteLog("--PrintJob start : PrintJob" + iValue + " --");
                        }
                        btFormatDoc.Print("PrintJob" + iValue);
                        if (iLot == 0)
                        {
                            Log.WriteLog("--PrintJob finish : PrintJob" + iValue + " --");
                        }
                    }
                }

                //结束打印
                if (!PrintLabelEnd()) return false;

                return true;
            }
            catch (Exception ex)
            {
                Log.WriteLog("--[Execute error] PrintLabel(string sLabel, string sPrinter,List<string> lstSubstringName, List<List<string>> lstValue) --");
                Log.WriteLog("--[System Error Msg] " + ex.Message + " --");

                return false;
            }
        }

        #endregion

    }
}
