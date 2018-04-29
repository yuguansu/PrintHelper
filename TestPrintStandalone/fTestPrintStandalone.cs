using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Seagull.BarTender.Print;
using System.Drawing.Printing;
using System.Collections.Generic;

/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 2017/12/22
 * 调用bartender sdk(Seagull.Bartender.Print.dll)
 * 需要设置文件print.xml
 *     设置文件<Print>
 *              <Bartender > 
 *       	        <Config Templet_Default="BS_DEFAULT.btw" Templet_Test= "BS_TEST.btw"/>
 *       	    </Bartender>
 *       	  </Print>
 * 需要bartender模板，模板名称需要与xml设置中相同，模板中的标签名称需要设置成程序中的"SERIAL_NUMBER"。
 * 打印时，如果电脑性能差，会有一些延迟（几秒到几十秒不等），是正常现象
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

namespace TestPrintStandalone
{
    public partial class fTestPrintStandalone : Form
    {
        public fTestPrintStandalone()
        {
            InitializeComponent();
        }

        LogHelper.LogHelper Log = new LogHelper.LogHelper("Print","Bartender");
        private string sPathFolder = string.Empty;
        private string sPathConfig = string.Empty;
        private string sPathTemplet = string.Empty;
        private string sLabelTemplet = string.Empty;
        List<string> listLabel = new List<string> { };

        private void Form1_Load(object sender, EventArgs e)
        {
            cmbPrinter.Items.AddRange(GetLocalPrinter());
            cmbPrinter.Text = GetDefaultPrinterName();

            btnInitial_Click(sender, e);
        }

        /// <summary>
        /// Initial Label ,初始化条码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInitial_Click(object sender, EventArgs e)
        {
            //获取当前程序集的路径
            sPathFolder = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            
            sPathTemplet = sPathFolder + "\\" + "BC_DEFAULT.btw";
            txtLabel.Text = sPathTemplet;
        }

        /// <summary>
        /// Print Label ,打印条码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                Log.WriteLogStart();
                Log.WriteLog("Get Print List");
                listLabel = GetListPrintLabel(lstSNNew);
                if (listLabel.Count<=0)
                {
                    Log.WriteLog("Get Print List Error , no label");
                    return;
                }
                Log.WriteLog("Get Print List OK");
                // Calling constructor with 'true' automatically starts engine. 
                using (Engine btEngine = new Engine(true))
                {
                    //定义标签变量，初始化标签模板，指定打印机
                    LabelFormatDocument btFormat = btEngine.Documents.Open(sPathTemplet, cmbPrinter.Text);
                    
                    Log.WriteLog("Print Engine Start.");

                    // Application specific code 
                    // Explicitly start the engine 
                    btEngine.Start();

                    Log.WriteLog("Print Engine Start OK.");

                    #region //更改打印参数
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
                    #endregion
                    for (int i = 0; i < listLabel.Count; i++)
                    {
                        Log.WriteLog("Open Print Label Start.");

                        //Reading Named Substrings ,读取标签字段
                        txtSNOld.Text = btFormat.SubStrings["SERIAL_NUMBER"].Value;
                        btFormat.SubStrings["SERIAL_NUMBER"].Value = listLabel[i];

                        Log.WriteLog("Open Print Label OK.");

                        Log.WriteLog("PrintJob Start.");

                        //Print Label ,打印标签
                        Result result = btFormat.Print("PrintJob1");

                        Log.WriteLog("PrintJob Finish.");
                    }

                    #region //五种打印方法
                    //Print Label ,Print() 直接打印btw文件 , Print(string printJobName) ,自定义打印事件名称
                    //Result result = btFormat.Print();

                    //Print Label ,Print(string printJobName, out messages) ,自定义打印事件名称
                    //Messages messages = null;
                    //Result result = btFormat.Print("PrintJob1", out messages);

                    //Print Label ,Print(string printJobName, out Messages message)

                    //Print Label ,Print(string printJobName, int waitForCompletionTimeout)

                    //Print Label ,Print(string printJobName, int waitForCompletionTimeout, out Messages messages)
                    #endregion

                    #region //打印多个模板
                    //打印多个模板
                    //int i = 0;
                    //foreach (LabelFormatDocument format in btEngine.Documents)
                    //{
                    //    i++;
                    //    format.Print("PrintJob" + i);
                    //}
                    #endregion

                    Log.WriteLog("Close Print Label.");

                    //Close Label Format ,关闭标签模板
                    btFormat.Close(SaveOptions.DoNotSaveChanges);

                    Log.WriteLog("Close Print Label OK.");
                    Log.WriteLog("Print Engine Stop.");

                    // Application-specific code 
                    // Assuming the application wants to save changes, 
                    //     it can be easily done at Stop time. 
                    btEngine.Stop(SaveOptions.SaveChanges);

                    Log.WriteLog("Print Engine Stop OK.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Log.WriteLog(ex.Message);
            }
            finally
            {
                Log.WriteLogEnd();
            }
        }

        private void cmbLabel_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnInitial_Click(sender,e);
        }

        private List<string> GetListPrintLabel(ListBox listbox)
        {
            List<string> list = new List<string> { };
            if (listbox.Items.Count>0)
            {
                for (int i = 0; i < listbox.Items.Count; i++)
                {
                    list.Add(listbox.Items[i].ToString());
                }
            }
            return list;
        }

        /// <summary>
        /// Get Default Printer Name
        /// </summary>
        /// <returns>Default Printer Name ,type of string</returns>
        public string GetDefaultPrinterName()
        {
            //获取默认打印机的方法
            PrintDocument fPrintDocument = new PrintDocument();
            return fPrintDocument.PrinterSettings.PrinterName;
        }

        /// <summary>
        /// Get Local Printer List
        /// </summary>
        /// <returns>List of Printer Name ,type of List ,menber type is string</returns>
        public string[] GetLocalPrinter()
        {
            string[] ListPrinter = new string[PrinterSettings.InstalledPrinters.Count];
            //获取当前打印机
            for (int i = 0; i < PrinterSettings.InstalledPrinters.Count; i++)
            {
                ListPrinter[i]=(PrinterSettings.InstalledPrinters[i].ToString());
            }
            return ListPrinter;
        }
    }
}
