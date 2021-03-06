﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using FA.Buiness;
using FA.Common;
using FA.DB;
using GODInventory.MyLinq;
using WeifenLuo.WinFormsUI.Docking;
using Excel = Microsoft.Office.Interop.Excel;
namespace FA_datawork_HRB
{
    public partial class frmFapiaoInfo : DockContent
    {
        int RowRemark = 0;
        int cloumn = 0;
        List<clsFAinfo> Result;
        public log4net.ILog ProcessLogger;
        public log4net.ILog ExceptionLogger;
        private SortableBindingList<clsFAinfo> sortablePendingOrderList;
        private string IDclick;
        List<clsFAinfo> Flter_Result;
        List<clsFAinfo> dav1_Flter_Result;
        List<clsuserinfo> userlist_Server;
        public frmFapiaoInfo(string username)
        {
            InitializeComponent();

            ReadScope(username);

            InitialSystemInfo();
            ProcessLoggerInitialSystemInfo();
            if (userlist_Server != null && userlist_Server.Count != 0)
            {
                if (userlist_Server[0].jigoudaima != null && userlist_Server[0].jigoudaima != "" && userlist_Server[0].jigoudaima != "所有")
                {
                    comboBox1.Text = userlist_Server[0].jigoudaima;
                    comboBox1.Enabled = false;
                }
                else
                {
                    this.comboBox1.SelectedIndex = 0;
                    this.comboBox2.SelectedIndex = 0;
                }
            }



            ProcessLogger.Fatal("print Initial" + DateTime.Now.ToString());
        }

        private void ReadScope(string username)
        {
            userlist_Server = new List<clsuserinfo>();
            clsAllnew BusinessHelp = new clsAllnew();
            userlist_Server = BusinessHelp.findUser(username);
            //   comboBox1.Text = 

        }
        public class SortableBindingList<T> : BindingList<T>
        {
            private bool isSortedCore = true;
            private ListSortDirection sortDirectionCore = ListSortDirection.Ascending;
            private PropertyDescriptor sortPropertyCore = null;
            private string defaultSortItem;

            public SortableBindingList() : base() { }

            public SortableBindingList(IList<T> list) : base(list) { }

            protected override bool SupportsSortingCore
            {
                get { return true; }
            }

            protected override bool SupportsSearchingCore
            {
                get { return true; }
            }

            protected override bool IsSortedCore
            {
                get { return isSortedCore; }
            }

            protected override ListSortDirection SortDirectionCore
            {
                get { return sortDirectionCore; }
            }

            protected override PropertyDescriptor SortPropertyCore
            {
                get { return sortPropertyCore; }
            }

            protected override int FindCore(PropertyDescriptor prop, object key)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    if (Equals(prop.GetValue(this[i]), key)) return i;
                }
                return -1;
            }

            protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
            {
                isSortedCore = true;
                sortPropertyCore = prop;
                sortDirectionCore = direction;
                Sort();
            }

            protected override void RemoveSortCore()
            {
                if (isSortedCore)
                {
                    isSortedCore = false;
                    sortPropertyCore = null;
                    sortDirectionCore = ListSortDirection.Ascending;
                    Sort();
                }
            }

            public string DefaultSortItem
            {
                get { return defaultSortItem; }
                set
                {
                    if (defaultSortItem != value)
                    {
                        defaultSortItem = value;
                        Sort();
                    }
                }
            }

            private void Sort()
            {
                List<T> list = (this.Items as List<T>);
                list.Sort(CompareCore);
                ResetBindings();
            }

            private int CompareCore(T o1, T o2)
            {
                int ret = 0;
                if (SortPropertyCore != null)
                {
                    ret = CompareValue(SortPropertyCore.GetValue(o1), SortPropertyCore.GetValue(o2), SortPropertyCore.PropertyType);
                }
                if (ret == 0 && DefaultSortItem != null)
                {
                    PropertyInfo property = typeof(T).GetProperty(DefaultSortItem, BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.IgnoreCase, null, null, new Type[0], null);
                    if (property != null)
                    {
                        ret = CompareValue(property.GetValue(o1, null), property.GetValue(o2, null), property.PropertyType);
                    }
                }
                if (SortDirectionCore == ListSortDirection.Descending) ret = -ret;
                return ret;
            }

            private static int CompareValue(object o1, object o2, Type type)
            {
                if (o1 == null) return o2 == null ? 0 : -1;
                else if (o2 == null) return 1;
                else if (type.IsPrimitive || type.IsEnum) return Convert.ToDouble(o1).CompareTo(Convert.ToDouble(o2));
                else if (type == typeof(DateTime)) return Convert.ToDateTime(o1).CompareTo(o2);
                else return String.Compare(o1.ToString().Trim(), o2.ToString().Trim());
            }
        }

        private void InitialSystemInfo()
        {
            Flter_Result = new List<clsFAinfo>();
            clsAllnew BusinessHelp = new clsAllnew();
            Result = BusinessHelp.findAll_Fapiao();
            if (userlist_Server != null && userlist_Server.Count != 0 && userlist_Server[0].jigoudaima != null && userlist_Server[0].jigoudaima != "" && userlist_Server[0].jigoudaima != "所有")
                Result = Result.FindAll(o => o.jigoudaima == userlist_Server[0].jigoudaima);

            Flter_Result = Result.Distinct(new ProductNoComparer()).ToList();
            var PMHZ = Flter_Result.OrderBy(s => s.danganhao).ToList();
            //总结发票数量
            int totalfapiaoshuliang = 0;

            foreach (var emp in PMHZ)
            {
                var coutfapiaoshuliang = Result.FindAll(o => o.danganhao == emp.danganhao);
                emp.fapiaoshuliang = coutfapiaoshuliang.Count.ToString();
                totalfapiaoshuliang = totalfapiaoshuliang + coutfapiaoshuliang.Count;
            }
            //if (Result)
            this.dataGridView2.AutoGenerateColumns = false;
            sortablePendingOrderList = new SortableBindingList<clsFAinfo>(PMHZ);
            this.bindingSource1.DataSource = sortablePendingOrderList;
            this.dataGridView2.DataSource = this.bindingSource1;
            this.toolStripLabel1.Text = "档号汇总量:" + PMHZ.Count + "  件数汇总量:" + totalfapiaoshuliang;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilter4(comboBox1.Text, comboBox2.Text);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            ApplyFilter4(comboBox1.Text, comboBox2.Text);

        }
        private void ApplyFilter4(string shipper = "", string county = "")
        {
            var originalSortOrder = this.dataGridView2.SortOrder;
            var originalSortedColumn = this.dataGridView2.SortedColumn;

            bindingSource1.DataSource = null;
            var filteredOrderList = Flter_Result;


            if (shipper.Length > 0 && shipper != "所有")
            {
                filteredOrderList = filteredOrderList.FindAll(o => o.jigoudaima == shipper);
            }
            if (county.Length > 0 && county != "所有")
            {
                filteredOrderList = filteredOrderList.FindAll(o => o.fapiaoleixing == county.Substring(0, 2));
            }

            sortablePendingOrderList = new SortableBindingList<clsFAinfo>(filteredOrderList);

            this.bindingSource1.DataSource = sortablePendingOrderList;

            var direction = ListSortDirection.Ascending;
            if (originalSortOrder == System.Windows.Forms.SortOrder.Descending)
            {
                direction = ListSortDirection.Descending;
            }
            if (originalSortedColumn != null)
            {
                this.dataGridView2.Sort(originalSortedColumn, direction);
            }
        }
        private void ApplyFilter3(string shipper = "", string county = "")
        {
            var originalSortOrder = this.dataGridView1.SortOrder;
            var originalSortedColumn = this.dataGridView1.SortedColumn;

            bindingSource2.DataSource = null;
            var filteredOrderList = Result;


            if (shipper.Length > 0 && shipper != "所有")
            {
                filteredOrderList = filteredOrderList.FindAll(o => o.danganhao == shipper);
            }
            if (county.Length > 0 && county != "所有")
            {
                filteredOrderList = filteredOrderList.FindAll(o => o.guidangrenzhanghao == county);
            }

            sortablePendingOrderList = new SortableBindingList<clsFAinfo>(filteredOrderList);

            this.bindingSource2.DataSource = sortablePendingOrderList;

            var direction = ListSortDirection.Ascending;
            if (originalSortOrder == System.Windows.Forms.SortOrder.Descending)
            {
                direction = ListSortDirection.Descending;
            }
            if (originalSortedColumn != null)
            {
                this.dataGridView1.Sort(originalSortedColumn, direction);
            }
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.DataSource = this.bindingSource2;
            dav1_Flter_Result = new List<clsFAinfo>();

            dav1_Flter_Result = filteredOrderList;
            this.toolStripLabel2.Text = "数量：" + bindingSource2.Count.ToString();

        }
        //集合去重复

        class ProductNoComparer : IEqualityComparer<clsFAinfo>
        {
            public bool Equals(clsFAinfo p1, clsFAinfo p2)
            {
                if (p1 == null)
                    return p2 == null;
                return p1.danganhao == p2.danganhao;
            }

            public int GetHashCode(clsFAinfo p)
            {
                if (p == null)
                    return 0;
                return p.danganhao.GetHashCode();
            }
        }
        //显示的边框线颜色
        private void dataGridView1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(Pens.Black, new Rectangle(0, 0, this.dataGridView1.Width - 1, this.dataGridView1.Height - 1));

        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            RowRemark = e.RowIndex;
            cloumn = e.ColumnIndex;
            if (RowRemark != -1)
            {
                clsFAinfo item = new clsFAinfo();
                item.danganhao = this.dataGridView2.Rows[RowRemark].Cells["档号列表"].EditedFormattedValue.ToString();
                item.jigoudaima = this.dataGridView2.Rows[RowRemark].Cells["归档人"].EditedFormattedValue.ToString();
                ApplyFilter3(item.danganhao, item.jigoudaima);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var form = new frmprint(dav1_Flter_Result);
            if (form.ShowDialog() == DialogResult.OK)
            {

            }
            return;

            DownLoadExcel_Crosscheckfile(dav1_Flter_Result);



        }

        private string DownLoadExcel_Crosscheckfile(List<clsFAinfo> Result)
        {
            try
            {

                ProcessLogger.Fatal("8948 Down Initial" + DateTime.Now.ToString());
                #region 获取模板路径

                string fullPath = AppDomain.CurrentDomain.BaseDirectory + "System\\confing.xls";
                SaveFileDialog sfdDownFile = new SaveFileDialog();
                sfdDownFile.OverwritePrompt = false;
                string DesktopPath = Convert.ToString(System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
                sfdDownFile.Filter = "Excel files (*.xls,*.xlsx)|*.xls;*.xlsx";

                string fileinfo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\");
                sfdDownFile.FileName = Path.Combine(fileinfo, "print" + "_" + DateTime.Now.ToString("yyyyMMdd_mmss") + ".xls");
                string strExcelFileName = string.Empty;
                string ResaveName = AppDomain.CurrentDomain.BaseDirectory + "Resources\\" + "print" + "_" + DateTime.Now.ToString("yyyyMMdd_mmss") + ".xls";

                #endregion

                #region 导出前校验模板信息
                if (string.IsNullOrEmpty(sfdDownFile.FileName))
                {
                    MessageBox.Show("File name can't be empty, please Check, thanks!");
                    return "";
                }
                if (!File.Exists(fullPath))
                {
                    MessageBox.Show("Template file does not exist, please Check, thanks!");
                    return "";
                }
                else
                {
                    strExcelFileName = sfdDownFile.FileName;
                }
                #endregion

                #region Excel 初始化
                System.Globalization.CultureInfo CurrentCI = System.Threading.Thread.CurrentThread.CurrentCulture;
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                Microsoft.Office.Interop.Excel.Range rng;

                Microsoft.Office.Interop.Excel.ApplicationClass ExcelApp = new Microsoft.Office.Interop.Excel.ApplicationClass();
                System.Reflection.Missing missingValue = System.Reflection.Missing.Value;
                Microsoft.Office.Interop.Excel._Workbook ExcelBook =
                ExcelApp.Workbooks.Open(fullPath, missingValue, missingValue, missingValue, missingValue, missingValue, missingValue, missingValue, missingValue, missingValue, missingValue, missingValue, missingValue, missingValue, missingValue);
                #endregion
                ProcessLogger.Fatal("8949  Input Initial" + DateTime.Now.ToString());

                #region 导入
                try
                {
                    #region Sheet 初始化
                    Microsoft.Office.Interop.Excel._Worksheet ExcelSheet = (Microsoft.Office.Interop.Excel.Worksheet)ExcelBook.Worksheets[1];


                    #region 填充数据
                    int RowIndex = 6;
                    string fapiaoleixingin = "";
                    string guidangren = "";
                    string danghao = "";
                    string guidangshijian = "";
                    foreach (clsFAinfo item in Result)
                    {
                        fapiaoleixingin = item.fapiaoleixing;
                        guidangren = item.guidangrenzhanghao;
                        danghao = item.danganhao;
                        if (item.Input_Date != null && item.Input_Date.Length > 8)
                            guidangshijian = item.Input_Date.Substring(0, 8);

                        RowIndex++;


                        ExcelSheet.Cells[RowIndex, 1] = "'" + item.fapiaohao;
                        ExcelSheet.Cells[RowIndex, 2] ="'"+ item.bianhao;
                        //ExcelSheet.Cells[RowIndex, 3] = item.fapiaohao;

                    }
                    //ExcelApp.Visible = true;
                    //ExcelApp.ScreenUpdating = true;

                    ExcelSheet.Cells[1, 1] = "总件数：" + Result.Count.ToString();
                    ExcelSheet.Cells[2, 1] = "发票类型：" + fapiaoleixingin;
                    ExcelSheet.Cells[3, 1] = "归档人：" + guidangren;
                    ExcelSheet.Cells[4, 1] = "档号：" + danghao;
                    ExcelSheet.Cells[5, 1] = "归档时间：" + guidangshijian;

                    //直接打印
                    //ExcelApp.ActiveWindow.View = Excel.XlWindowView.xlPageBreakPreview;

                    //ExcelSheet.PageSetup.PaperSize =Excel.XlPaperSize.xlPaperA4;

                    //ExcelBook.PrintOut();
                    #region 写入文件
                    ProcessLogger.Fatal("8950  Output Initial" + DateTime.Now.ToString());

                    ExcelApp.DisplayAlerts = false;
                    ExcelApp.ScreenUpdating = true;
                    ExcelBook.SaveAs(ResaveName, missingValue, missingValue, missingValue, missingValue, missingValue, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, missingValue, missingValue, missingValue, missingValue, missingValue);
                    ExcelApp.DisplayAlerts = false;

                    ExcelBook.Close(false, missingValue, missingValue);
                    ExcelBook = null;
                    #endregion
                    //转换成PDF 文件
                    //if (XLSConvertToPDF(ResaveName, ResaveName.Replace("xlsx", "pdf")))
                    //{
                    //    // FilePath.Add(ResaveName.Replace("xlsx", "pdf"));
                    //}

                    return ResaveName;
                    #endregion

                    #endregion
                }
                #endregion

                #region 异常处理
                catch (Exception ex)
                {
                    ExcelApp.DisplayAlerts = false;
                    ExcelApp.Quit();
                    ExcelBook = null;
                    ExcelApp = null;
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    throw ex;
                }
                #endregion

                #region Finally垃圾回收
                finally
                {
                    //ExcelBook.Close(false, missingValue, missingValue);
                    //ExcelBook = null;
                    ExcelApp.DisplayAlerts = true;
                    ExcelApp.Quit();
                    clsKeyMyExcelProcess.Kill(ExcelApp);

                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool XLSConvertToPDF(string sourcePath, string targetPath)
        {
            bool result = false;
            Excel.XlFixedFormatType targetType = Excel.XlFixedFormatType.xlTypePDF;
            object missing = Type.Missing;
            Microsoft.Office.Interop.Excel.ApplicationClass ExcelApp = null;
            Microsoft.Office.Interop.Excel._Workbook ExcelBook = null;
            try
            {

                object target = targetPath;
                object type = targetType;
                //workBook = application.Workbooks.Open(sourcePath, missing, missing, missing, missing, missing,
                //        missing, missing, missing, missing, missing, missing, missing, missing, missing);
                // sourcePath = "C:\\Users\\IBM_ADMIN\\Desktop\\newadd.xlsx";
                System.Globalization.CultureInfo CurrentCI = System.Threading.Thread.CurrentThread.CurrentCulture;
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                ExcelApp = new Microsoft.Office.Interop.Excel.ApplicationClass();
                System.Reflection.Missing missingValue = System.Reflection.Missing.Value;
                ExcelBook = ExcelApp.Workbooks.Open(sourcePath, missingValue, missingValue, missingValue, missingValue, missingValue, missingValue, missingValue, missingValue, missingValue, missingValue, missingValue, missingValue, missingValue, missingValue);

                //ExcelApp.Visible = true;
                //ExcelApp.ScreenUpdating = true;

                ////ActiveWindow.SmallScroll Down:=6;
                //ExcelApp.ActiveWindow.View = Excel.XlWindowView.xlPageBreakPreview;
                ////ExcelApp.ActiveWindow.SmallScroll = Excel.;
                //ExcelApp.ActiveWindow.Zoom = 80;
                ////ActiveWindow.SmallScroll Down:=-3
                //// excelRange.WrapText = true;
                ////ActiveSheet.VPageBreaks(1).DragOff Direction:=xlToRight, RegionIndex:=1
                //ActiveWindow.SmallScroll Down:=30
                //Set ActiveSheet.HPageBreaks(1).Location = Range("A67")
                //ActiveWindow.SmallScroll Down:=-75

                //Microsoft.Office.Interop.Excel.Worksheet WS2 = (Microsoft.Office.Interop.Excel.Worksheet)ExcelBook.Worksheets[2];

                ////上边距   
                //double top = 0;
                ////左边距   
                //double left = 0;
                ////右边距   
                //double right = 0;
                ////下边距   
                //double footer = 0;
                //WS2.DisplayAutomaticPageBreaks = false;//显示分页线      
                //WS2.PageSetup.CenterFooter = "第   &P   页，共   &N   页";
                //WS2.PageSetup.TopMargin = ExcelApp.InchesToPoints(top / 2.54);//上   
                //WS2.PageSetup.BottomMargin = ExcelApp.InchesToPoints(footer / 15.54);//下   
                //WS2.PageSetup.LeftMargin = ExcelApp.InchesToPoints(left / 2.54);//左   
                //WS2.PageSetup.RightMargin = ExcelApp.InchesToPoints(right / 2.54);//右   
                //WS2.PageSetup.CenterHorizontally = true;//水平居中   xlSheet.PageSetup.PrintTitleRows = "$1:$3";//顶端标题行      
                //WS2.PageSetup.PaperSize = Excel.XlPaperSize.xlPaperA3;//A3纸张大小   xlSheet.PageSetup.Orientation = Excel.XlPageOrientation.xlLandscape;//纸张方向.横向  


                //Excel.Range excelRange = WS2.get_Range(WS2.Cells[1, 1], WS2.Cells[64, 24]);
                //自动调整列宽   
                ////  excelRange.EntireColumn.AutoFit();
                ////   excelRange.WrapText = false;     //文本自动换行   
                //excelRange.ShrinkToFit = false;
                ////设置字体在单元格内的对其方式    
                //excelRange.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                //// 文本水平居中方式
                //excelRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                //////设置为横向打印 
                ////WS2.PageSetup.Orientation = Excel.XlPageOrientation.xlLandscape;


                ////ExcelApp.ActiveWindow.FreezePanes = true;

                ////excelRange.EntireColumn.AutoFit();
                ////WS2.PageSetup.Orientation = 2;
                //WS2.PageSetup.PaperSize = Excel.XlPaperSize.xlPaperA4;

                //WS2.PageSetup.LeftMargin = ExcelApp.InchesToPoints(0.0);
                //WS2.PageSetup.RightMargin = ExcelApp.InchesToPoints(0.0);
                //WS2.PageSetup.TopMargin = ExcelApp.InchesToPoints(0.0);
                //WS2.PageSetup.BottomMargin = ExcelApp.InchesToPoints(0.0);
                //WS2.PageSetup.HeaderMargin = ExcelApp.InchesToPoints(0.0);
                //WS2.PageSetup.FooterMargin = ExcelApp.InchesToPoints(0.0);
                //WS2.PageSetup.CenterHorizontally = true;
                ////  WS2.PageSetup.PaperSize = Excel.XlPaperSize.xlPaperA4;
                //WS2.PageSetup.Orientation = Excel.XlPageOrientation.xlLandscape;
                //excelRange = WS2.get_Range(WS2.Cells[1, 1], WS2.Cells[2, 20]);
                //WS2.PageSetup.PrintTitleRows = excelRange.get_Address(excelRange.Row, excelRange.Column, Excel.XlReferenceStyle.xlA1, 1, 1);


                ExcelBook.ExportAsFixedFormat(targetType, target, Excel.XlFixedFormatQuality.xlQualityStandard, true, false, missing, missing, missing, missing);
                result = true;


            }

            catch
            {
                result = false;
            }
            finally
            {
                if (ExcelBook != null)
                {
                    ExcelBook.Close(true, missing, missing);
                    ExcelBook = null;
                }
                if (ExcelApp != null)
                {
                    ExcelApp.Quit();
                    ExcelApp = null;
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            return result;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" && textBox2.Text == "" && textBox3.Text == "")
            {
                MessageBox.Show("请至少填写一个查找的信息！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            clsAllnew BusinessHelp = new clsAllnew();
            Result = BusinessHelp.findFapiao_user(textBox1.Text, textBox2.Text, textBox3.Text);
            //只能读自己的发票信息
            if (userlist_Server != null && userlist_Server.Count != 0 && userlist_Server[0].jigoudaima != null && userlist_Server[0].jigoudaima != "" && userlist_Server[0].jigoudaima != "所有")
                Result = Result.FindAll(o => o.jigoudaima == userlist_Server[0].jigoudaima);

            Flter_Result = Result.Distinct(new ProductNoComparer()).ToList();
            var PMHZ = Flter_Result.OrderBy(s => s.danganhao).ToList();

            //if (Result)
            this.dataGridView2.AutoGenerateColumns = false;
            sortablePendingOrderList = new SortableBindingList<clsFAinfo>(PMHZ);
            this.bindingSource1.DataSource = sortablePendingOrderList;
            this.dataGridView2.DataSource = this.bindingSource1;
            //总结发票数量
            int totalfapiaoshuliang = 0;

            foreach (var emp in PMHZ)
            {
                var coutfapiaoshuliang = Result.FindAll(o => o.danganhao == emp.danganhao);
                emp.fapiaoshuliang = coutfapiaoshuliang.Count.ToString();
                totalfapiaoshuliang = totalfapiaoshuliang + coutfapiaoshuliang.Count;
            }
            this.toolStripLabel1.Text = "档号汇总量:" + PMHZ.Count + "  件数汇总量:" + totalfapiaoshuliang;


            bindingSource2.DataSource = null;
            this.dataGridView1.AutoGenerateColumns = false;
            sortablePendingOrderList = new SortableBindingList<clsFAinfo>(Result);
            this.bindingSource2.DataSource = sortablePendingOrderList;
            this.dataGridView1.DataSource = this.bindingSource2;
            this.toolStripLabel2.Text = "数量：" + bindingSource2.Count.ToString();


        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dav1_Flter_Result != null)
            {
                DownLoadExcel_Crosscheckfile(dav1_Flter_Result);

                string ZFCEPath = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources"), "");
                System.Diagnostics.Process.Start("explorer.exe", ZFCEPath);
            }
            else
                MessageBox.Show("请读取数据后再次下载数据！");
            return;
            //下载两个 全屏信息

            string strFileName = "";
            string strFileName2 = "";
            #region dav 1
            {
                {
                    if (this.dataGridView1.Rows.Count == 0)
                    {
                        MessageBox.Show("Sorry , No Data Output !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    var saveFileDialog = new SaveFileDialog();
                    saveFileDialog.DefaultExt = ".csv";
                    saveFileDialog.Filter = "csv|*.csv";
                    strFileName = "System  Info" + "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    strFileName2 = strFileName;

                    saveFileDialog.FileName = strFileName;
                    if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                    {
                        strFileName = saveFileDialog.FileName.ToString();
                    }
                    else
                    {
                        return;
                    }
                    FileStream fa = new FileStream(strFileName, FileMode.Create);
                    StreamWriter sw = new StreamWriter(fa, Encoding.Unicode);
                    string delimiter = "\t";
                    string strHeader = "";
                    for (int i = 0; i < this.dataGridView1.Columns.Count; i++)
                    {
                        strHeader += this.dataGridView1.Columns[i].HeaderText + delimiter;
                    }
                    sw.WriteLine(strHeader);

                    //output rows data
                    for (int j = 0; j < this.dataGridView1.Rows.Count; j++)
                    {
                        string strRowValue = "";

                        for (int k = 0; k < this.dataGridView1.Columns.Count; k++)
                        {
                            if (this.dataGridView1.Rows[j].Cells[k].Value != null)
                            {
                                strRowValue += this.dataGridView1.Rows[j].Cells[k].Value.ToString().Replace("\r\n", " ").Replace("\n", "") + delimiter;
                                if (this.dataGridView1.Rows[j].Cells[k].Value.ToString() == "LIP201507-35")
                                {

                                }

                            }
                            else
                            {
                                strRowValue += this.dataGridView1.Rows[j].Cells[k].Value + delimiter;
                            }
                        }
                        sw.WriteLine(strRowValue);
                    }
                    sw.Close();
                    fa.Close();

                }

            }
            #endregion

            #region dav 2
            {
                {
                    if (this.dataGridView2.Rows.Count == 0)
                    {
                        MessageBox.Show("Sorry , No Data Output !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    var saveFileDialog = new SaveFileDialog();
                    saveFileDialog.DefaultExt = ".csv";
                    saveFileDialog.Filter = "csv|*.csv";
                    string strFileName1 = "System 2 Info" + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "";
                    saveFileDialog.FileName = strFileName;
                    //if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                    //{
                    //    strFileName = saveFileDialog.FileName.ToString();
                    //}
                    //else
                    //{
                    //    return;
                    //}
                    strFileName = strFileName.Replace(strFileName2, strFileName1);

                    FileStream fa = new FileStream(strFileName, FileMode.Create);
                    StreamWriter sw = new StreamWriter(fa, Encoding.Unicode);
                    string delimiter = "\t";
                    string strHeader = "";
                    for (int i = 0; i < this.dataGridView2.Columns.Count; i++)
                    {
                        strHeader += this.dataGridView2.Columns[i].HeaderText + delimiter;
                    }
                    sw.WriteLine(strHeader);

                    //output rows data
                    for (int j = 0; j < this.dataGridView2.Rows.Count; j++)
                    {
                        string strRowValue = "";

                        for (int k = 0; k < this.dataGridView2.Columns.Count; k++)
                        {
                            if (this.dataGridView2.Rows[j].Cells[k].Value != null)
                                strRowValue += this.dataGridView2.Rows[j].Cells[k].Value.ToString().Replace("\r\n", " ") + delimiter;
                            else
                                strRowValue += this.dataGridView2.Rows[j].Cells[k].Value + delimiter;
                        }
                        sw.WriteLine(strRowValue);
                    }

                    sw.Close();
                    fa.Close();
                    MessageBox.Show("下载成功 ！", "System", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }

            #endregion



        }

        private void button4_Click(object sender, EventArgs e)
        {
            InitialSystemInfo();
        }

        private void ProcessLoggerInitialSystemInfo()
        {
            #region 初始化配置
            ProcessLogger = log4net.LogManager.GetLogger("ProcessLogger");
            ExceptionLogger = log4net.LogManager.GetLogger("SystemExceptionLogger");

            #endregion
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            datetimepicker();

        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {

            datetimepicker();

        }

        private void datetimepicker()
        {
            string start_time = clsCommHelp.objToDateTime1(dateTimePicker1.Text);
            string end_time = clsCommHelp.objToDateTime1(dateTimePicker2.Text);
            clsAllnew BusinessHelp = new clsAllnew();
            Result = BusinessHelp.findFapiao_user(start_time, end_time, "");
            //只能读自己的发票信息
            if (userlist_Server != null && userlist_Server.Count != 0 && userlist_Server[0].jigoudaima != null && userlist_Server[0].jigoudaima != "" && userlist_Server[0].jigoudaima != "所有")
                Result = Result.FindAll(o => o.jigoudaima == userlist_Server[0].jigoudaima);

            Flter_Result = Result.Distinct(new ProductNoComparer()).ToList();
            var PMHZ = Flter_Result.OrderBy(s => s.danganhao).ToList();

            //if (Result)
            this.dataGridView2.AutoGenerateColumns = false;
            sortablePendingOrderList = new SortableBindingList<clsFAinfo>(PMHZ);
            this.bindingSource1.DataSource = sortablePendingOrderList;
            this.dataGridView2.DataSource = this.bindingSource1;
            //总结发票数量
            int totalfapiaoshuliang = 0;

            foreach (var emp in PMHZ)
            {
                var coutfapiaoshuliang = Result.FindAll(o => o.danganhao == emp.danganhao);
                emp.fapiaoshuliang = coutfapiaoshuliang.Count.ToString();
                totalfapiaoshuliang = totalfapiaoshuliang + coutfapiaoshuliang.Count;
            }
            this.toolStripLabel1.Text = "档号汇总量:" + PMHZ.Count + "  件数汇总量:" + totalfapiaoshuliang;


            bindingSource2.DataSource = null;
            this.dataGridView1.AutoGenerateColumns = false;
            sortablePendingOrderList = new SortableBindingList<clsFAinfo>(Result);
            this.bindingSource2.DataSource = sortablePendingOrderList;
            this.dataGridView1.DataSource = this.bindingSource2;
            this.toolStripLabel2.Text = "数量：" + bindingSource2.Count.ToString();
        }
    }
}
