# DAL.PrintHelper Bartender打印

使用Bartender SDK(Seagull.BarTender.Print.dll)进行打印  
需要安装Bartender Automation或以上版本  
自带日志记录。  

* <b>推荐使用BartenderHeperUtils。</b>  
* <b>必须选择x86模式编译</b>  
* 打印参数设置好后，直接调用静态函数打印  
* 弊端：打印时engine启动和停止时间较长，本机启动engine约6秒（配置：i5-8300H，8G）。每次调用打印方法均会启动、停止engine。所以批量打印时，请一次性将数据全部传入，不要频繁多次调用。  

---

### 提供2个打印SDK调用示例程序

1. TestClient_BartenderAlone  
直接调用Seagull.BarTender.Print.dll进行打印。

2. TestClient_BartenderHelper  
将打印方法封装成BartenderHelper进行调用打印。  

---

### BartenderHelper封装说明
做了两个封装：
1. BartenderHeper：第一次写的封装，示例2也是使用该封装编写，仅提供一种打印选择。
2. <b>推荐使用</b>BartenderHeperUtils：充分使用打印engine参数配置，定义多种不同参数的打印方法。  

---

### Bartender SDK(Seagull.BarTender.Print.dll) 使用说明  

1. 启动engine  
btEngine.Start();
2. 初始化标签模板，指定打印机  
LabelFormatDocument btFormat = btEngine.Documents.Open(sPathTemplet, cmbPrinter.Text);  
3. 打印参数设置  
    //指定打印机  
    //Result result = btFormat.Print("PrintJob1");  
    //// Open a label format specifying a different printer  
    //btFormat = btEngine.Documents.Open(sPathTemplet, "OurPrinter_HX3000");  

    //更改打印机  
    //btFormat.PrintSetup.PrinterName = @"OurPrinter_HX3000";  

    //设置打印数量  
    //// Change the number of identical labels and serialized labels to print  
    //btFormat.PrintSetup.NumberOfSerializedLabels = 4;  
    //btFormat.PrintSetup.IdenticalCopiesOfLabel = 10;  


4. 打印标签  
Result result = btFormat.Print("PrintJob1");  

    //五种打印方法  

    //第一种，直接打印  
    //Print() 直接打印btw文件  
    //Print(string printJobName) ,自定义打印事件名称  
    //Result result = btFormat.Print();  

    //第二种，指定打印任务  
    //Print Label ,Print(string printJobName, out messages) ,自定义打印事件名称  
    //Messages messages = null;  
    //Result result = btFormat.Print("PrintJob1", out messages);  

    //第三种  
    //Print Label ,Print(string printJobName, out Messages message)  

    //第四种  
    //Print Label ,Print(string printJobName, int waitForCompletionTimeout)  

    //第五种  
    //Print Label ,Print(string printJobName, int waitForCompletionTimeout, out Messages messages)  
5. 打印多个模板  
    //打印多个模板  
    //int i = 0;  
    //foreach (LabelFormatDocument format in btEngine.Documents)  
    //{  
    //    i++;  
    //    format.Print("PrintJob" + i);  
    //}  
---

### BartenderHeperUtils  核心打印方法  

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
        #region 
        //记录参数  
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
#### 功能说明
1. 提供多种不同参数传入的打印方法，可根据自己的参数内容，选择不同的方法。  
    * 支持指定打印机
    * 支持指定bartender打印模板
    * 支持打印一个标签  
    * 支持打印多个标签
    * 支持指定打印的份数  
    * 支持多个标签的整体连续打印（标签1、标签2、标签1、标签2、标签1、标签2）和单张连续打印（标签1、标签1、标签1、标签2、标签2、标签2）
2. 每次调用打印，只能对同一标签格式的内容进行批量打印。如需打印不同格式的标签，请多次分开调用。  
3. 支持自定义日志记录  
自动在LogHelper.dll所在文件夹中建立日志文件夹“.\Logs\Print\BartenderHeperUtils”。  
自动以小时为单位记录日志到文本文件 “yyyyMMdd_HH.txt”。  
如不需要记录日志，可自行删除相关代码。  

#### 参数设定  
1. 指定标签模板 sLabelNameFull  
默认文件夹路径为 BartenderHelperUtils.dll 所在文件夹路径，不可更改，标签模板必须和dll在同一路径。  
指定 sLabelName 为 bartender 标签模板文件名称(带后缀.btw)，如果不传则默认为"BC_DEFAULT.btw"。  
2. 指定打印机名称  sPrinterName  
可自定义打印机名称，如不自定义，则指定为系统默认打印机。  
也可从本地打印机列表中进行选择 ConfigLoad.GetLocalPrinterList()。  
3. 标签中的变量列表使用 List<string> 表示，多个标签使用 List<List<string>> 表示。标签变量名称和bartender模板变量名称需要一一对应。  
4. 指定批量打印中，单个标签连续打印数量 iRepeatNumberOfSingleLabel  
例如设置为3，则标签打印效果为“标签1、标签1、标签1、标签2、标签2、标签2”。
5. 指定批量打印中，整批标签连续打印数量 iRepeatNumberOfLot。  
例如设置为3，则标签打印效果为“标签1、标签2、标签1、标签2、标签1、标签2”。  