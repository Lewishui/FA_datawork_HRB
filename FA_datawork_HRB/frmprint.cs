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

        public frmprint(List<clsFAinfo> Result1)
        {
            InitializeComponent();
            Result = new List<clsFAinfo>();
            Result = Result1;
        }
        private void frmprint_Load(object sender, EventArgs e)
        {          
            reportViewer1.LocalReport.ReportPath = Application.StartupPath + "\\Report1.rdlc";
            //指定数据集,数据集名称后为表,不是DataSet类型的数据集
            this.reportViewer1.LocalReport.DataSources.Clear();
            this.reportViewer1.LocalReport.DataSources.Add(new Microsoft.Reporting.WinForms.ReportDataSource("DataSet1", Result));
            //显示报表
            this.reportViewer1.RefreshReport();
        }
    }
}
