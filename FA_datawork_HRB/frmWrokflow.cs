using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using FA.Buiness;
using FA.DB;
using GODInventory.MyLinq;
using MongoDB.Bson;
using MongoDB.Driver;
using WeifenLuo.WinFormsUI.Docking;

namespace FA_datawork_HRB
{
    public partial class frmWrokflow : DockContent
    {
        List<clsFAinfo> Result;
        private string hezihao;
        string guidangren;
        int RowRemark = 0;
        int cloumn = 0;
        private SortableBindingList<clsFAinfo> sortablePendingOrderList;
        private string IDclick;
        int logis;
        private string ipadress;
        public frmWrokflow(string username)
        {
            InitializeComponent();
            string path = AppDomain.CurrentDomain.BaseDirectory + "System\\IP.txt";

            string[] fileText = File.ReadAllLines(path);
            ipadress = "mongodb://" + fileText[0];
            InitialSystemInfo();
            Result = new List<clsFAinfo>();
            this.comboBox1.SelectedIndex = 0;
            this.comboBox2.SelectedIndex = 0;
            guidangren = username;
            NewMethoduserFind(username);
           


        }
        private void InitialSystemInfo()
        {

            clsAllnew BusinessHelp = new clsAllnew();
            Result = BusinessHelp.findFapiao(comboBox1.Text, comboBox2.Text);
            //if (Result)
            this.dataGridView1.AutoGenerateColumns = false;
            sortablePendingOrderList = new SortableBindingList<clsFAinfo>(Result);
            this.bindingSource1.DataSource = sortablePendingOrderList;
            this.dataGridView1.DataSource = this.bindingSource1;
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

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            List<clsFAinfo> Result = new List<clsFAinfo>();
            int sss = 0;
            if (textBox1.Text != "" && this.textBox2.Text != "")
            {
                int len = Convert.ToInt32(textBox2.Text) - Convert.ToInt32(textBox1.Text);
                if (len < 0)
                    return;

                for (int i = 0; i <= len; i++)
                {
                    clsFAinfo item = new clsFAinfo();
                    int ssl= Convert.ToInt32(textBox1.Text) + i;

                    item.fapiaohao = ssl.ToString();
                    item.jigoudaima = comboBox1.Text;
                    item.fapiaoleixing = comboBox2.Text;
                    item.danganhao = stockNOTextBox.Text;
                    item.Input_Date = DateTime.Now.ToString("yyyyMMdd-HHmmss");
                    item.guidangrenzhanghao = guidangren;
                    sss = i + 1;
                    item.bianhao = sss.ToString().PadLeft(4, '0');

                    Result.Add(item);


                }

            }
            if (this.textBox3.Text != "")
            {
                clsFAinfo item = new clsFAinfo();
               
                item.fapiaohao = textBox3.Text;
                item.jigoudaima = comboBox1.Text;
                item.fapiaoleixing = comboBox2.Text;
                item.danganhao = stockNOTextBox.Text;
                item.Input_Date = DateTime.Now.ToString("yyyyMMdd-HHmmss");
                item.guidangrenzhanghao = guidangren;
                sss = sss + 1;
                item.bianhao = sss.ToString().PadLeft(4, '0');
                Result.Add(item);
            }
            if (Result.Count != 0)
            {
                clsAllnew BusinessHelp = new clsAllnew();
                BusinessHelp.createFapiao_Server(Result);
                toolStripLabel1.Text = "已保存发票条目：" + Result.Count;
                InitialSystemInfo();

            }


        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            InitialSystemInfo();
            BuildStockNO();
    

        }
        private void BuildStockNO()
        {
            try
            {
                //读取-发票盒号 最大值
                if (Result != null && Result.Count != 0)
                {
                  //  var changeList = this.Result.FindAll(s => s.jigoudaima == comboBox1.Text && s.fapiaoleixing == comboBox2.Text);
                    var sss = Result.Select(s => new MockEntity { ShortName = s.danganhao, FullName = s.danganhao }).Distinct().OrderBy(s => s.ShortName).ToList(); ;
                    
                    string[] temptong = System.Text.RegularExpressions.Regex.Split(sss[sss.Count - 1].ShortName, "-");
                    int ssl = Convert.ToInt32("1" + temptong[3]) + 1;
                    hezihao = ssl.ToString().Substring(1, Convert.ToInt32(ssl.ToString().Length - 1));
                }
                else
                    hezihao = "00001";
                this.stockNOTextBox.Text = comboBox1.Text + "-" + comboBox2.Text + "-" + DateTime.Now.ToString("yyyyMMdd") + "-" + hezihao;

            }
            catch (Exception ex)
            {
                MessageBox.Show("错误:" + ex, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;


                throw;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            int len = Convert.ToInt32(textBox2.Text) - Convert.ToInt32(textBox1.Text) + 1;
            if (textBox3.Text != "")
                len++;

            if (len < 0)
                errorProvider1.SetError(textBox2, String.Format("结束发票号不能小于起始发票号！"));
            else
            {
                toolStripLabel1.Text = "共计发票条目：" + len;
                errorProvider1.SetError(textBox2, String.Format(""));
            }

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            int len = 0;

            if (textBox1.Text != "" && this.textBox2.Text != "")
                len = Convert.ToInt32(textBox2.Text) - Convert.ToInt32(textBox1.Text) + 1;
            if (textBox3.Text != "")
                len++;

            if (len < 0)
                errorProvider1.SetError(textBox2, String.Format("结束发票号不能小于起始发票号！"));
            else
            {
                toolStripLabel1.Text = "共计发票条目：" + len;
                errorProvider1.SetError(textBox2, String.Format(""));
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {

            string QiHao = this.dataGridView1.Rows[RowRemark].Cells["档案号"].EditedFormattedValue.ToString();

            var form = new frmDeleteFapiaoAdmin(QiHao);
            if (form.ShowDialog() == DialogResult.OK)
            {

            }


        }
        //获取识别是否是管理员权限

        private bool NewMethoduserFind(string user)
        {

            string connectionString = "mongodb://127.0.0.1";
            connectionString = ipadress;
            MongoServer server = MongoServer.Create(connectionString);
            MongoDatabase db1 = server.GetDatabase("FA_datawork_HRB");
            MongoCollection collection1 = db1.GetCollection("FA_datawork_HRB_User");
            MongoCollection<BsonDocument> employees = db1.GetCollection<BsonDocument>("FA_datawork_HRB_User");

            ///精确查找
            var query = new QueryDocument { { "name", user } };
            //   foreach (var emp in data)
            logis = 0;
            foreach (BsonDocument emp in employees.Find(query))
            {
                string Useramin = "";
                string lockis = "";
                string Pass = (emp["password"].AsString);
                string User = (emp["name"].AsString);
                if (emp.Contains("AdminIS"))
                    Useramin = (emp["AdminIS"].AsString);
                if (emp.Contains("Btype"))
                    lockis = (emp["Btype"].AsString);
                if (lockis == "lock")
                {
                    MessageBox.Show("登录失败,账户已被锁定，请重试或联系系统管理员，谢谢", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                if ( User.ToString().Trim() == user.Trim())
                    if (Useramin == "true")
                    {
                        toolStripButton2.Enabled = true;
                    }
                    else
                    {
                        toolStripButton2.Enabled = false;                 
                                   
                    }
            } 
            return false;

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            this.comboBox1.SelectedIndex = 0;
            this.comboBox2.SelectedIndex = 0;
            this.textBox1.Text = "";
            this.textBox2.Text = "";
            this.textBox3.Text = "";
            stockNOTextBox.Text = "";



        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {

            if (this.dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("Sorry , No Data Output !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = ".csv";
            saveFileDialog.Filter = "csv|*.csv";
            string strFileName = "PINGAN  FA System Info" + "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
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
            MessageBox.Show("Dear User, Down File  Successful ！", "System", MessageBoxButtons.OK, MessageBoxIcon.Information);



        }

        private void notifyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (RowRemark >= dataGridView1.Rows.Count)
            {
                RowRemark = RowRemark - 1;
            }
            clsFAinfo item = new clsFAinfo();

            item.fapiaohao =this.dataGridView1.Rows[RowRemark].Cells["发票号"].EditedFormattedValue.ToString();
            item.danganhao = this.dataGridView1.Rows[RowRemark].Cells["档案号"].EditedFormattedValue.ToString();
            item.jigoudaima = this.dataGridView1.Rows[RowRemark].Cells["机构代码"].EditedFormattedValue.ToString();

            clsAllnew BusinessHelp = new clsAllnew();

            BusinessHelp.deletefapiao(item);
            InitialSystemInfo();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            RowRemark = e.RowIndex;
            cloumn = e.ColumnIndex;
            clsFAinfo item = new clsFAinfo();
            item.fapiaohao =this.dataGridView1.Rows[RowRemark].Cells["发票号"].EditedFormattedValue.ToString();
            item.danganhao = this.dataGridView1.Rows[RowRemark].Cells["档案号"].EditedFormattedValue.ToString();
            item.bianhao = this.dataGridView1.Rows[RowRemark].Cells["编号"].EditedFormattedValue.ToString();
            item.jigoudaima = this.dataGridView1.Rows[RowRemark].Cells["机构代码"].EditedFormattedValue.ToString();

            //var pendingorder = Result.Find(o => o.QiHao == id.ToString());
            clsFAinfo stock = this.Result.Find(o => (o.fapiaohao == item.fapiaohao && o.danganhao == item.danganhao && o.jigoudaima == item.jigoudaima));


            IDclick = stock.R_id;

        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            InitialSystemInfo();

        }

        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (RowRemark >= dataGridView1.RowCount)
                return;
            List<clsFAinfo> SaveResult = new List<clsFAinfo>();
            clsFAinfo item = new clsFAinfo();
            item.fapiaohao =  this.dataGridView1.Rows[RowRemark].Cells["发票号"].EditedFormattedValue.ToString() ;
            item.danganhao = this.dataGridView1.Rows[RowRemark].Cells["档案号"].EditedFormattedValue.ToString();
            item.bianhao = this.dataGridView1.Rows[RowRemark].Cells["编号"].EditedFormattedValue.ToString();
            item.jigoudaima = this.dataGridView1.Rows[RowRemark].Cells["机构代码"].EditedFormattedValue.ToString();
            item.fapiaoleixing = this.dataGridView1.Rows[RowRemark].Cells["发票类型"].EditedFormattedValue.ToString();
            item.Input_Date = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            item.R_id = IDclick;
            item.guidangrenzhanghao = guidangren;

            SaveResult.Add(item);


            clsAllnew BusinessHelp = new clsAllnew();

            BusinessHelp.updateFA_Server(SaveResult);
            InitialSystemInfo();

        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (RowRemark >= dataGridView1.RowCount)
                return;
            List<clsFAinfo> SaveResult = new List<clsFAinfo>();
            clsFAinfo item = new clsFAinfo();
            item.fapiaohao = this.dataGridView1.Rows[RowRemark].Cells["发票号"].EditedFormattedValue.ToString();
            item.danganhao = this.dataGridView1.Rows[RowRemark].Cells["档案号"].EditedFormattedValue.ToString();
            item.bianhao = this.dataGridView1.Rows[RowRemark].Cells["编号"].EditedFormattedValue.ToString();
            item.jigoudaima = this.dataGridView1.Rows[RowRemark].Cells["机构代码"].EditedFormattedValue.ToString();
            item.fapiaoleixing = this.dataGridView1.Rows[RowRemark].Cells["发票类型"].EditedFormattedValue.ToString();
            item.Input_Date = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            item.R_id = IDclick;
            item.guidangrenzhanghao = guidangren;

            SaveResult.Add(item);


            clsAllnew BusinessHelp = new clsAllnew();

            BusinessHelp.updateFA_Server(SaveResult);
            InitialSystemInfo();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            InitialSystemInfo();
            BuildStockNO();
        }
    }
}
