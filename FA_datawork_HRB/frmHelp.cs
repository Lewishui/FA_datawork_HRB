using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FA_datawork_HRB
{
    public partial class frmHelp : Form
    {
        public frmHelp()
        {
            InitializeComponent();
            string message = " Version ： " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            string listinfo = "PA 财务系统" + message + "\r\n" + " Copyrighted : QQ: 512250428，All rights reserved" + "\r\n"+ " Security mode:Default";
            this.label6.Text = listinfo;
        }
    }
}
