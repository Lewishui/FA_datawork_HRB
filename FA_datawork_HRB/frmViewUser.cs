using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using FA.Buiness;
using FA.DB;

namespace FA_datawork_HRB
{
    public partial class frmViewUser : Form
    {
        int RowRemark = 0;
        int cloumn = 0;
        List<clsuserinfo> Result_Server;
        private SortableBindingList<clsuserinfo> sortablePendingOrderList;
        public frmViewUser()
        {
            InitializeComponent();
          
           
       
          InitialSystemInfo();
     
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
            clsAllnew BusinessHelp = new clsAllnew();
            Result_Server = new List<clsuserinfo>();

            Result_Server = BusinessHelp.ReadUserlistfromServer();
            this.dataGridView1.AutoGenerateColumns = false;
            sortablePendingOrderList = new SortableBindingList<clsuserinfo>(Result_Server);
            this.bindingSource1.DataSource = sortablePendingOrderList;
            this.dataGridView1.DataSource = this.bindingSource1;

        }

        private void notifyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (RowRemark >= dataGridView1.Rows.Count)
            {
                RowRemark = RowRemark - 1;
            }
            string QiHao = this.dataGridView1.Rows[RowRemark].Cells[0].EditedFormattedValue.ToString();
        
            clsAllnew BusinessHelp = new clsAllnew();

            BusinessHelp.deleteUSER(QiHao );
            InitialSystemInfo();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            RowRemark = e.RowIndex;
            cloumn = e.ColumnIndex;
             
            return;
        }

        private void 编辑ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string QiHao = this.dataGridView1.Rows[RowRemark].Cells[0].EditedFormattedValue.ToString();

            var form = new frmEdidUser(QiHao);
            if (form.ShowDialog() == DialogResult.OK)
            {

            }


        }
    }
}
