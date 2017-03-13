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
    public partial class frmEdidUser : Form
    {
        List<clsuserinfo> userlist_Server;
        private string logname;

        public frmEdidUser(string name)
        {
            InitializeComponent();
            InitialSystemInfo(name);

        }

        private void InitialSystemInfo(string name)
        {
            clsAllnew BusinessHelp = new clsAllnew();
            userlist_Server = new List<clsuserinfo>();
            userlist_Server = BusinessHelp.findUser(name);

            #region page1 信息录入
            if (userlist_Server.Count != 0)
            {
                this.textBox1.Text = userlist_Server[0].name;
                this.textBox2.Text = userlist_Server[0].password;
                this.textBox3.Text = userlist_Server[0].password;
                if (userlist_Server[0].Btype == "lock")
                    this.radioButton2.Checked = true;
                else
                    this.radioButton1.Checked = true;

                if (userlist_Server[0].AdminIS == "true")
                    checkBox1.Checked = true;


                this.textBox6.Text = userlist_Server[0].name;
                this.textBox4.Text = userlist_Server[0].password;
                this.textBox5.Text = userlist_Server[0].password;
            }
            #endregion
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
            userlist_Server.Add(item);
            clsAllnew BusinessHelp = new clsAllnew();

            BusinessHelp.createUser_Server(userlist_Server);

            MessageBox.Show("创建用户成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            userlist_Server = new List<clsuserinfo>();
            clsuserinfo item = new clsuserinfo();

            if (textBox1.Text == "")
            {
                MessageBox.Show("请填写完整信息然后锁定或解锁！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (this.radioButton1.Checked == false && this.radioButton2.Checked == false)
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

        private void button2_Click(object sender, EventArgs e)
        {
            this.textBox1.Text = "";

            this.textBox2.Text = "";

            this.textBox3.Text = "";
            this.radioButton1.Checked = false;
            this.radioButton2.Checked = false;
            checkBox1.Checked = false;
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
    }
}
