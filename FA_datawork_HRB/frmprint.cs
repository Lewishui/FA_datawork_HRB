using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FA.DB;

namespace FA_datawork_HRB
{
    public partial class frmprint : Form
    {
        List<clsFAinfo> Result;
        public log4net.ILog ProcessLogger;
        public log4net.ILog ExceptionLogger;
        public frmprint(List<clsFAinfo> Result1)
        {
            InitializeComponent();
            Result = new List<clsFAinfo>();
            Result = Result1;
            InitialSystemInfo();
            ProcessLogger.Fatal("print Initial" + DateTime.Now.ToString());
         
        }
        private void InitialSystemInfo()
        {
            #region 初始化配置
            ProcessLogger = log4net.LogManager.GetLogger("ProcessLogger");
            ExceptionLogger = log4net.LogManager.GetLogger("SystemExceptionLogger");
          
            #endregion
        }
        private void frmprint_Load(object sender, EventArgs e)
        {
            try
            {

                reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Report1.rdlc";
                ProcessLogger.Fatal("109723 load file" + DateTime.Now.ToString());
                //指定数据集,数据集名称后为表,不是DataSet类型的数据集
                this.reportViewer1.LocalReport.DataSources.Clear();
                this.reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet1", Result));
                ProcessLogger.Fatal("109724 Add file" + DateTime.Now.ToString());
                //显示报表
                this.reportViewer1.RefreshReport();
                ProcessLogger.Fatal("109724 display file" + DateTime.Now.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("异常" + ex);
                return;

                throw;
            }
        }
    }
}
