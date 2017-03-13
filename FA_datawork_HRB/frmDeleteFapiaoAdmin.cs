using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FA.Buiness;
using FA.DB;
using GODInventory.MyLinq;

namespace FA_datawork_HRB
{
    public partial class frmDeleteFapiaoAdmin : Form
    {
        public frmDeleteFapiaoAdmin(string  danganhao)
        {
            InitializeComponent();
            this.textBox1.Text = danganhao;
            InitialSystemInfo(danganhao);

        }
        private void InitialSystemInfo(string shipNO)
        {

            clsAllnew BusinessHelp = new clsAllnew();
            List<clsFAinfo> Result = BusinessHelp.findAll_Fapiao();
           var sss = Result.Select(s => new MockEntity { ShortName = s.danganhao, FullName = s.danganhao }).Distinct().OrderBy(s => s.ShortName).ToList(); ;

           this.comboBox1.DisplayMember = "FullName";
           this.comboBox1.ValueMember = "ShortName";
           this.comboBox1.DataSource = sss;
           if (shipNO.Length > 0)
           {
               this.comboBox1.SelectedValue = shipNO;
           }
           else
           {
               this.comboBox1.SelectedItem = null;
           }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text == "")
            {
                MessageBox.Show("请输入档案号！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            
            }
            if (MessageBox.Show("确认要清空当前档案号的数据 ?", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {

            }
            else
                return;
            clsAllnew BusinessHelp = new clsAllnew();

            BusinessHelp.deletefapiao_danganhao(this.textBox1.Text.Trim());
            MessageBox.Show("删除成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
              
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.textBox1.Text = comboBox1.Text;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();

        }
    }
}
