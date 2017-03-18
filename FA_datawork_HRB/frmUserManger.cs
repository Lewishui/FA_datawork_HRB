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

namespace FA_datawork_HRB
{
    public partial class frmUserManger : Form
    {
        List<clsuserinfo> userlist_Server;
        private string logname;

        public frmUserManger(string name,string typelogin)
        {
            InitializeComponent();
            logname = name;
            this.textBox6.Text = name;
            if (typelogin == "User")
            {
                this.tabControl1.SelectedIndex = 1;
                tabControl1.TabPages[0].Parent = null;
                toolStripButton3.Visible = false;
                toolStripButton2.Visible = false;
                toolStripButton1.Visible = false;

            }

        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedIndex = 0;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedIndex = 1;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            var form = new frmViewUser( );        
            if (form.ShowDialog() == DialogResult.OK)
            {

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            userlist_Server = new List<clsuserinfo>();
            clsuserinfo item = new clsuserinfo();

            if (textBox1.Text == "" || textBox2.Text == "")
            {
                MessageBox.Show("请填写完整信息然后创建！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;


            }
            if (textBox2.Text.Trim() != textBox3.Text.Trim())
            {
                MessageBox.Show("两次输入的用户密码不一致，请重新输入！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }

            item.name = textBox1.Text.Trim();
            item.password = textBox2.Text.Trim();
            if (this.radioButton1.Checked == true)
                item.Btype = "Normal";
            else if (this.radioButton2.Checked == true)
                item.Btype = "lock";
            if (checkBox1.Checked == true)
                item.AdminIS = "true";
            else
                item.AdminIS = "false";

            item.jigoudaima = this.comboBox1.Text.Trim();

            userlist_Server.Add(item);
            clsAllnew BusinessHelp = new clsAllnew();

            BusinessHelp.createUser_Server(userlist_Server);

            MessageBox.Show("创建用户成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.textBox4.Text = "";

            this.textBox5.Text = "";

            this.textBox6.Text = "";

        }

        private void button4_Click(object sender, EventArgs e)
        {
            userlist_Server = new List<clsuserinfo>();
            clsuserinfo item = new clsuserinfo();

            if (textBox4.Text == "" || textBox5.Text == "" || textBox6.Text == "")
            {
                MessageBox.Show("请填写完整信息然后创建！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;


            }
            if (textBox4.Text.Trim() != textBox5.Text.Trim())
            {
                MessageBox.Show("两次输入的用户密码不一致，请重新输入！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }

            item.name = textBox6.Text.Trim();
            item.password = textBox5.Text.Trim();
           

            userlist_Server.Add(item);
            clsAllnew BusinessHelp = new clsAllnew();

            BusinessHelp.changeUserpassword_Server(userlist_Server);

            MessageBox.Show("密码修改成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.textBox1.Text = "";

            this.textBox2.Text = "";

            this.textBox3.Text = "";
            this.radioButton1.Checked  = false;
            this.radioButton2.Checked = false;
            checkBox1.Checked = false;

        }

        private void button5_Click(object sender, EventArgs e)
        {
            userlist_Server = new List<clsuserinfo>();
            clsuserinfo item = new clsuserinfo();

            if (textBox1.Text == ""  )
            {
                MessageBox.Show("请填写完整信息然后锁定或解锁！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (this.radioButton1.Checked == false &&this.radioButton2.Checked == false)
            {
                MessageBox.Show("账户状态缺失，请重新输入！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            item.name = textBox1.Text.Trim();
            if (this.radioButton1.Checked == true)
                item.Btype = "Normal";
            else if (this.radioButton2.Checked == true)
                item.Btype = "lock";

            userlist_Server.Add(item);
            clsAllnew BusinessHelp = new clsAllnew();

            BusinessHelp.lock_Userpassword_Server(userlist_Server);

            MessageBox.Show("账户状态修改成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }
    }
}
