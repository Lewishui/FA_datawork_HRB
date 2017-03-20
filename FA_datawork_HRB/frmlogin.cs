using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using FA.Buiness;
using FA.Common;
using FA.DB;
using Microsoft.Win32;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FA_datawork_HRB
{
    public partial class frmlogin : Form
    {
        Sunisoft.IrisSkin.SkinEngine se = null;
        public log4net.ILog ProcessLogger;
        public log4net.ILog ExceptionLogger;
        private TextBox txtSAPPassword;
        private CheckBox chkSaveInfo;
        int logis = 0;
        private frmFapiaoInfo frmFapiaoInfo;
        private System.Timers.Timer timerAlter1;
        private bool IsRun1 = false;
        private Thread GetDataforRawDataThread;
        private frmWrokflow frmWrokflow;
        private int alterisrun;
        private string ipadress;
        private frmHelp frmHelp;

        public frmlogin()
        {
            InitializeComponent();
            InitialSystemInfo();
            se = new Sunisoft.IrisSkin.SkinEngine();
            se.SkinAllForm = true;
            se.SkinFile = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ""), "GlassOrange.ssk");

            InitialPassword();
            ProcessLogger.Fatal("login" + DateTime.Now.ToString());
            string path = AppDomain.CurrentDomain.BaseDirectory + "System\\IP.txt";
              
            string[] fileText = File.ReadAllLines(path);
            ipadress = "mongodb://"+fileText[0];
            this.Text = "PA 财务系统 " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
          
        }
        private void InitialSystemInfo()
        {
            #region 初始化配置
            ProcessLogger = log4net.LogManager.GetLogger("ProcessLogger");
            ExceptionLogger = log4net.LogManager.GetLogger("SystemExceptionLogger");
            ProcessLogger.Fatal("System Start " + DateTime.Now.ToString());
            #endregion
        }
        private void InitialPassword()
        {
            try
            {
                txtSAPPassword = new TextBox();
                txtSAPPassword.PasswordChar = '*';
                ToolStripControlHost t = new ToolStripControlHost(txtSAPPassword);
                t.Width = 100;
                t.AutoSize = false;
                t.Alignment = ToolStripItemAlignment.Right;
                this.toolStrip1.Items.Insert(this.toolStrip1.Items.Count - 4, t);

                chkSaveInfo = new CheckBox();
                chkSaveInfo.Text = "";
                chkSaveInfo.Padding = new Padding(5, 2, 0, 0);
                ToolStripControlHost t1 = new ToolStripControlHost(chkSaveInfo);
                t1.AutoSize = true;

                t1.ToolTipText = clsShowMessage.MSG_002;
                t1.Alignment = ToolStripItemAlignment.Right;
                this.toolStrip1.Items.Insert(this.toolStrip1.Items.Count - 5, t1);
                getUserAndPassword();
                chkSaveInfo.Checked = false;

            }
            catch (Exception ex)
            {
                //clsLogPrint.WriteLog("<frmMain> InitialPassword:" + ex.Message);
                throw ex;
            }
        }
        private void getUserAndPassword()
        {
            try
            {
                RegistryKey rkLocalMachine = Registry.LocalMachine;
                RegistryKey rkSoftWare = rkLocalMachine.OpenSubKey(clsConstant.RegEdit_Key_SoftWare);
                RegistryKey rkAmdape2e = rkSoftWare.OpenSubKey(clsConstant.RegEdit_Key_AMDAPE2E);
                if (rkAmdape2e != null)
                {
                    this.txtSAPUserId.Text = clsCommHelp.encryptString(clsCommHelp.NullToString(rkAmdape2e.GetValue(clsConstant.RegEdit_Key_User)));
                    this.txtSAPPassword.Text = clsCommHelp.encryptString(clsCommHelp.NullToString(rkAmdape2e.GetValue(clsConstant.RegEdit_Key_PassWord)));
                    if (clsCommHelp.NullToString(rkAmdape2e.GetValue(clsConstant.RegEdit_Key_Date)) != "")
                    {
                        this.chkSaveInfo.Checked = true;
                    }
                    else
                    {
                        this.chkSaveInfo.Checked = false;
                    }
                    rkAmdape2e.Close();
                }
                rkSoftWare.Close();
                rkLocalMachine.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw ex;
            }
        }
        private void saveUserAndPassword()
        {
            try
            {
                RegistryKey rkLocalMachine = Registry.LocalMachine;
                RegistryKey rkSoftWare = rkLocalMachine.OpenSubKey(clsConstant.RegEdit_Key_SoftWare, true);
                RegistryKey rkAmdape2e = rkSoftWare.CreateSubKey(clsConstant.RegEdit_Key_AMDAPE2E);
                if (rkAmdape2e != null)
                {
                    rkAmdape2e.SetValue(clsConstant.RegEdit_Key_User, clsCommHelp.encryptString(this.txtSAPUserId.Text.Trim()));
                    rkAmdape2e.SetValue(clsConstant.RegEdit_Key_PassWord, clsCommHelp.encryptString(this.txtSAPPassword.Text.Trim()));
                    rkAmdape2e.SetValue(clsConstant.RegEdit_Key_Date, DateTime.Now.ToString("yyyMMdd"));
                }
                rkAmdape2e.Close();
                rkSoftWare.Close();
                rkLocalMachine.Close();

            }
            catch (Exception ex)
            {
                //ClsLogPrint.WriteLog("<frmMain> saveUserAndPassword:" + ex.Message);
                throw ex;
            }
        }

        private void pBBToolStripMenuItem_Click(object sender, EventArgs e)
        {

            var form = new frmUserManger(this.txtSAPUserId.Text.Trim(), "Admin");
            //var form1 = form.ShowDialog();

            if (form.ShowDialog() == DialogResult.OK)
            {

            }

        }

        private void tsbLogin_Click(object sender, EventArgs e)
        {
            try
            {
            
                NewMethoduserFind(txtSAPUserId.Text.Trim(), txtSAPPassword.Text.Trim());
                if (logis != 0)
                {
                    this.WindowState = FormWindowState.Maximized;
                    if (chkSaveInfo.Checked == true)
                        saveUserAndPassword();

                    #region 更新登录时间
                    List<clsuserinfo> userlist_Server = new List<clsuserinfo>();
                    clsuserinfo item = new clsuserinfo();
                    item.name = txtSAPUserId.Text.Trim();

                    item.denglushijian = DateTime.Now.ToString("yyyyMMdd-HH:mm:ss");


                    userlist_Server.Add(item);
                    clsAllnew BusinessHelp = new clsAllnew();

                    BusinessHelp.updateLoginTime_Server(userlist_Server);

                    #endregion
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("登录失败，请查看根目录下的System文件夹中IP.txt中服务器IP 地址是否正确！","错误",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;

                throw ex;
            }
        }
        private bool NewMethoduserFind(string user, string pass)
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
                if (Pass.ToString().Trim() == pass.Trim() && User.ToString().Trim() == user.Trim())
                    if (Useramin == "true")
                    {
                        toolStripDropDownButton1.Enabled = true;
                        toolStripDropDownButton3.Enabled = true;
                        toolStripDropDownButton2.Enabled = true;
                        //一键配置ToolStripMenuItem.Enabled = true;
                        pBBToolStripMenuItem.Enabled = true;
                        修改登录信息ToolStripMenuItem.Enabled = true;
                        logis++;
                    }
                    else
                    {
                        toolStripDropDownButton1.Enabled = true;
                        toolStripDropDownButton3.Enabled = true;
                        toolStripDropDownButton2.Enabled = true;

                        修改登录信息ToolStripMenuItem.Enabled = true;
                        logis++;
                        // return false;
                    }
            }
            if (logis == 0)
            {
                MessageBox.Show("登录失败，请重试或联系系统管理员，谢谢", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;

            }
            else
                this.WindowState = FormWindowState.Maximized;

            return false;



        }

        private void 修改登录信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            var form = new frmUserManger(this.txtSAPUserId.Text.Trim(), "User");

            if (form.ShowDialog() == DialogResult.OK)
            {

            }
        }

        private void 导入彩票数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {


            if (frmWrokflow == null)
            {
                frmWrokflow = new frmWrokflow(this.txtSAPUserId.Text.Trim());
                frmWrokflow.FormClosed += new FormClosedEventHandler(FrmOMS_FormClosed);
            }
            if (frmWrokflow == null)
            {
                frmWrokflow = new frmWrokflow(this.txtSAPUserId.Text.Trim());
            }
            frmWrokflow.Show(this.dockPanel2);
        }
        void FrmOMS_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (sender is frmWrokflow)
            {
                frmWrokflow = null;
            }
            if (sender is frmFapiaoInfo)
            {
                frmFapiaoInfo = null;
            }
            if (sender is frmHelp)
            {
                frmHelp = null;
            }

            
        }

        private void 查询信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {



            if (frmFapiaoInfo == null)
            {
                frmFapiaoInfo = new frmFapiaoInfo(this.txtSAPUserId.Text.Trim());
                frmFapiaoInfo.FormClosed += new FormClosedEventHandler(FrmOMS_FormClosed);
            }
            if (frmFapiaoInfo == null)
            {
                frmFapiaoInfo = new frmFapiaoInfo(this.txtSAPUserId.Text.Trim());
            }
            frmFapiaoInfo.Show(this.dockPanel2);
        }

        private void 一键配置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
          
        }
        private void NewMethod1()
        {

            List<clsuserinfo> userlist_Server = new List<clsuserinfo>();
            clsuserinfo item = new clsuserinfo();
            item.name = "Admin";
            item.password = "123";
            item.Btype = "Normal";
            item.AdminIS = "true";
            userlist_Server.Add(item);
            clsAllnew BusinessHelp = new clsAllnew();
            BusinessHelp.createUser_Server(userlist_Server);
        }
        private void systemin()
        {

            try
            {
                #region 创建文件夹和 log 记事本

                ProcessLogger.Fatal("1001--Create Folder txt" + DateTime.Now.ToString());
                string spath = @"C:\Program Files\mongodb\bin";

                if (Directory.Exists(spath))
                {
                }
                else
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(spath);
                    directoryInfo.Create();
                }


                spath = @"C:\Program Files\mongodb\data\db";

                if (Directory.Exists(spath))
                {
                }
                else
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(spath);
                    directoryInfo.Create();
                }

                spath = @"C:\Program Files\mongodb\data\log";

                if (Directory.Exists(spath))
                {
                }
                else
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(spath);
                    directoryInfo.Create();
                }
                spath = @"C:\Program Files\mongodb\data\log\MongoDB.log";

                if (File.Exists(spath))
                {
                }
                else
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(spath);

                    System.IO.File.Create(spath);
                }

                #endregion
                #region 复制文件BIN 到指定目录
                ProcessLogger.Fatal("1002--copy bin" + DateTime.Now.ToString());
                string srcdir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "System\\bin");
                string todir = @"C:\Program Files\mongodb\";
                string dstdir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "System\\bin");
                bool overwrite = true;
                CopyDirIntoDestDirectory(srcdir, todir, overwrite);


                #endregion

                #region 调用CMD 命令
                ProcessLogger.Fatal("1003--install db Start" + DateTime.Now.ToString());
                string cmd = @"C:&cd C:\Program Files\mongodb\bin&&mongod --dbpath ""C:\Program Files\mongodb\data\db""";
                string output = "";
                //cmd = @"ipconfig/all";
                RunCmd(cmd, out output);
                //  MessageBox.Show(output);

                ProcessLogger.Fatal("1004--install servers" + DateTime.Now.ToString());
                timerAlter1 = new System.Timers.Timer(200000);
                timerAlter1.Elapsed += new System.Timers.ElapsedEventHandler(TimeControl1);
                timerAlter1.AutoReset = true;
                timerAlter1.Start();
                cmd = @"C:&cd C:\Program Files\mongodb\bin&&mongod --dbpath ""C:\Program Files\mongodb\data\db"" --logpath ""C:\Program Files\mongodb\data\log\MongoDB.log"" --install --serviceName ""MongoDB""";
                RunCmd(cmd, out output);
                #endregion

                //MessageBox.Show("运行结束 ，后台数据配置成功 ", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                if (ex.Message.ToString().Contains("AccessException"))
                {
                    string dstdir = "";
                    Version ver = System.Environment.OSVersion.Version;
                    if (ver.Major.ToString().Contains("10"))
                    {
                        dstdir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "System\\win10Admin.reg");
                    }
                    else if (ver.Major.ToString().Contains("6"))
                    {
                        dstdir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "System\\win7Admin.reg");
                    }
                    Process.Start(dstdir);

                }
                MessageBox.Show("由于您未获得管理员权限，请尝试取得管理员权限\r\n（系统(仅支持Window10，win7版本)已自动尝试获取权限，如重试启动系统还未正常运行则请手动获取windows 的权限） ！" + ex, "安装错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

                throw;
            }

        }
        public static void CopyDirIntoDestDirectory(string srcdir, string dstdir, bool overwrite)
        {
            string todir = Path.Combine(dstdir,
                                        Path.GetFileName(srcdir)
                                        );

            if (!Directory.Exists(todir))
                Directory.CreateDirectory(todir);

            foreach (var s in Directory.GetFiles(srcdir))
            {
                string news = s.Replace(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "System\\bin"), todir);
                if (File.Exists(news))
                {
                }
                else
                {
                    File.Copy(s, Path.Combine(todir, Path.GetFileName(s)), overwrite);
                }
            }
            foreach (var s in Directory.GetDirectories(srcdir))
                CopyDirIntoDestDirectory(s, todir, overwrite);
        }


        public static void RunCmd(string cmd, out string output)
        {
            try
            {
                string CmdPath = @"C:\Windows\System32\cmd.exe";
                cmd = cmd.Trim().TrimEnd('&') + "&exit";//说明：不管命令是否成功均执行exit命令，否则当调用ReadToEnd()方法时，会处于假死状态
                using (Process p = new Process())
                {
                    p.StartInfo.FileName = CmdPath;
                    p.StartInfo.UseShellExecute = false;        //是否使用操作系统shell启动
                    p.StartInfo.RedirectStandardInput = true;   //接受来自调用程序的输入信息
                    p.StartInfo.RedirectStandardOutput = true;  //由调用程序获取输出信息
                    p.StartInfo.RedirectStandardError = true;   //重定向标准错误输出
                    p.StartInfo.CreateNoWindow = true;          //不显示程序窗口
                    p.Start();//启动程序

                    //向cmd窗口写入命令
                    p.StandardInput.WriteLine(cmd);
                    p.StandardInput.AutoFlush = true;

                    //获取cmd窗口的输出信息
                    output = p.StandardOutput.ReadToEnd();
                    p.WaitForExit();//等待程序执行完退出进程
                    p.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("EX:数据库配置失败 ：" + ex);


                throw;
            }
        }

        private void TimeControl1(object sender, EventArgs e)
        {
            if (!IsRun1)
            {
                IsRun1 = true;
                GetDataforRawDataThread = new Thread(TimeMethod1);
                GetDataforRawDataThread.Start();
            }
        }
        private void TimeMethod1()
        {
            string output = "";
            string cmd = @"C:&cd C:\Program Files\mongodb\bin&&mongod --dbpath ""C:\Program Files\mongodb\data\db"" --logpath ""C:\Program Files\mongodb\data\log\MongoDB.log"" --install --serviceName ""MongoDB""";
            RunCmd(cmd, out output);

            alterisrun = 0;
            IsRun1 = false;
            MessageBox.Show("运行结束 ，后台数据配置成功 ,系统即将关闭，请自行重启即可", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();

        }

        private void 打开本地ToolStripMenuItem_Click(object sender, EventArgs e)
        {
          
        }

        private void 追踪分析ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string ZFCEPath = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ""), "");
            System.Diagnostics.Process.Start("系统使用说明.xls", ZFCEPath);
        }

        private void 一键配置初始化信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {

                //安装数据库
                systemin();


                NewMethod1();

                MessageBox.Show("导入成功,可以使用了！");

            }
            catch (Exception ex)
            {
                MessageBox.Show("导入数据错误，请确认本地文件包解压是否正常" + ex, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

                throw;
            }
        }

        private void 打开本地目录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string ZFCEPath = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "System"), "");
            System.Diagnostics.Process.Start("explorer.exe", ZFCEPath);

        }

        private void 关于系统ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (frmHelp == null)
            {
                frmHelp = new frmHelp();
                frmHelp.FormClosed += new FormClosedEventHandler(FrmOMS_FormClosed);
            }
            if (frmHelp == null)
            {
                frmHelp = new frmHelp();
            }

            frmHelp.Show(this.dockPanel2);
        }
    }
}
