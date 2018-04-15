using System;
using System.Collections.Generic;
using System.Windows.Forms;
using BartenderHelper;

namespace TestForHelper
{
    public partial class fTestForHelper : Form
    {
        public fTestForHelper()
        {
            InitializeComponent();
        }

        private void fTestForHelper_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Initial
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInitial_Click(object sender, EventArgs e)
        {
            cmbLabel.Items.AddRange(ConfigLoad.GetLocalPrinterList());
            cmbPrinter.Items.AddRange(ConfigLoad.GetLocalPrinterList());
        }

        /// <summary>
        /// 选择标签模板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbLabel_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        /// <summary>
        /// 选择打印机
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbPrinter_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 打印条码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrint_Click(object sender, EventArgs e)
        {
            List<string> listSN = new List<string> { };
            for (int i = 0; i < lstSNNew.Items.Count; i++)
            {
                listSN.Add(lstSNNew.Items[i].ToString());
            }
            try
            {
                BartenderHeperUtils.PrintLabel("", "","SERIAL_NUMBER", listSN,2);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
    }
}
