using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FA.DB;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace FA.Buiness
{

    public class clsAllnew
    {
        private string ipadress;
        public clsAllnew()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "System\\IP.txt";

            string[] fileText = File.ReadAllLines(path);
            ipadress = "mongodb://" + fileText[0];


        }
        public void createUser_Server(List<clsuserinfo> AddMAPResult)
        {
            string connectionString = "mongodb://127.0.0.1";

            connectionString = ipadress;
            MongoServer server = MongoServer.Create(connectionString);
            MongoDatabase db1 = server.GetDatabase("FA_datawork_HRB");
            MongoCollection collection1 = db1.GetCollection("FA_datawork_HRB_User");
            MongoCollection<BsonDocument> employees1 = db1.GetCollection<BsonDocument>("FA_datawork_HRB_User");

            //  collection1.RemoveAll();
            if (AddMAPResult == null)
            {
                MessageBox.Show("No Data  input Sever", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            foreach (clsuserinfo item in AddMAPResult)
            {

                QueryDocument query = new QueryDocument("name", item.name);
                collection1.Remove(query);
                BsonDocument fruit_1 = new BsonDocument
                 { 
                 { "name", item.name },
                 { "password", item.password },
                 { "Createdate", DateTime.Now.ToString("yyyy/MM/dd/HH")}, 
                 { "Btype", item.Btype} ,
                  { "denglushijian", item.denglushijian} ,
                   { "jigoudaima", item.jigoudaima} ,
                 { "AdminIS", item.AdminIS} 
                 };
                collection1.Insert(fruit_1);
            }
        }
        public void changeUserpassword_Server(List<clsuserinfo> AddMAPResult)
        {
            string connectionString = "mongodb://127.0.0.1";
            connectionString = ipadress;
            MongoServer server = MongoServer.Create(connectionString);
            MongoDatabase db1 = server.GetDatabase("FA_datawork_HRB");
            MongoCollection collection1 = db1.GetCollection("FA_datawork_HRB_User");
            MongoCollection<BsonDocument> employees1 = db1.GetCollection<BsonDocument>("FA_datawork_HRB_User");

            //  collection1.RemoveAll();
            if (AddMAPResult == null)
            {
                MessageBox.Show("No Data  input Sever", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            foreach (clsuserinfo item in AddMAPResult)
            {
                QueryDocument query = new QueryDocument("name", item.name);
                var update = Update.Set("password", item.password.Trim());
                collection1.Update(query, update);
            }
        }

        public void lock_Userpassword_Server(List<clsuserinfo> AddMAPResult)
        {
            string connectionString = "mongodb://127.0.0.1";
            connectionString = ipadress;
            MongoServer server = MongoServer.Create(connectionString);
            MongoDatabase db1 = server.GetDatabase("FA_datawork_HRB");
            MongoCollection collection1 = db1.GetCollection("FA_datawork_HRB_User");
            MongoCollection<BsonDocument> employees1 = db1.GetCollection<BsonDocument>("FA_datawork_HRB_User");

            //  collection1.RemoveAll();
            if (AddMAPResult == null)
            {
                MessageBox.Show("No Data  input Sever", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            foreach (clsuserinfo item in AddMAPResult)
            {
                QueryDocument query = new QueryDocument("name", item.name);
                var update = Update.Set("Btype", item.Btype.Trim());
                collection1.Update(query, update);
            }
        }

        public void updateLoginTime_Server(List<clsuserinfo> AddMAPResult)
        {
            string connectionString = "mongodb://127.0.0.1";
            connectionString = ipadress;
            MongoServer server = MongoServer.Create(connectionString);
            MongoDatabase db1 = server.GetDatabase("FA_datawork_HRB");
            MongoCollection collection1 = db1.GetCollection("FA_datawork_HRB_User");
            MongoCollection<BsonDocument> employees1 = db1.GetCollection<BsonDocument>("FA_datawork_HRB_User");

            //  collection1.RemoveAll();
            if (AddMAPResult == null)
            {
                MessageBox.Show("No Data  input Sever", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            foreach (clsuserinfo item in AddMAPResult)
            {
                QueryDocument query = new QueryDocument("name", item.name);
                var update = Update.Set("denglushijian", item.denglushijian.Trim());
                collection1.Update(query, update);
            }
        }

        public List<clsuserinfo> ReadUserlistfromServer()
        {

            #region Read  database info server
            try
            {
                List<clsuserinfo> ClaimReport_Server = new List<clsuserinfo>();

                string connectionString = "mongodb://127.0.0.1";
                connectionString = ipadress;
                MongoServer server = MongoServer.Create(connectionString);
                MongoDatabase db1 = server.GetDatabase("FA_datawork_HRB");
                MongoCollection collection1 = db1.GetCollection("FA_datawork_HRB_User");
                MongoCollection<BsonDocument> employees = db1.GetCollection<BsonDocument>("FA_datawork_HRB_User");

                foreach (BsonDocument emp in employees.FindAll())
                {
                    clsuserinfo item = new clsuserinfo();

                    #region 数据
                    if (emp.Contains("_id"))
                        item._id = (emp["_id"].ToString());
                    if (emp.Contains("name"))
                        item.name = (emp["name"].AsString);
                    if (emp.Contains("password"))
                        item.password = (emp["password"].ToString());
                    if (emp.Contains("Btype"))
                        item.Btype = (emp["Btype"].AsString);
                    if (emp.Contains("denglushijian"))
                        item.denglushijian = (emp["denglushijian"].AsString);
                    if (emp.Contains("Createdate"))
                        item.Createdate = (emp["Createdate"].AsString);
                    if (emp.Contains("AdminIS"))
                        item.AdminIS = (emp["AdminIS"].AsString);

                    if (emp.Contains("jigoudaima"))
                        item.jigoudaima = (emp["jigoudaima"].AsString);

                    #endregion
                    ClaimReport_Server.Add(item);
                }
                return ClaimReport_Server;

            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex);
                return null;

                throw ex;
            }
            #endregion
        }
        public void deleteUSER(string name)
        {
            string connectionString = "mongodb://127.0.0.1";
            connectionString = ipadress;
            MongoServer server = MongoServer.Create(connectionString);
            MongoDatabase db1 = server.GetDatabase("FA_datawork_HRB");
            MongoCollection collection1 = db1.GetCollection("FA_datawork_HRB_User");
            MongoCollection<BsonDocument> employees = db1.GetCollection<BsonDocument>("FA_datawork_HRB_User");

            if (name == null)
            {
                MessageBox.Show("No Data  input Sever", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            QueryDocument query = new QueryDocument("name", name);
            //var dd = Query.And(Query.EQ("QiHao", name), Query.EQ("Caipiaomingcheng", Caipiaomingcheng));//同时满足多个条件

            collection1.Remove(query);
        }

        public List<clsuserinfo> findUser(string findtext)
        {

            #region Read  database info server
            try
            {
                List<clsuserinfo> ClaimReport_Server = new List<clsuserinfo>();

                string connectionString = "mongodb://127.0.0.1";
                connectionString = ipadress;
                MongoServer server = MongoServer.Create(connectionString);
                MongoDatabase db1 = server.GetDatabase("FA_datawork_HRB");
                MongoCollection collection1 = db1.GetCollection("FA_datawork_HRB_User");
                MongoCollection<BsonDocument> employees = db1.GetCollection<BsonDocument>("FA_datawork_HRB_User");

                var query = new QueryDocument("name", findtext);

                foreach (BsonDocument emp in employees.Find(query))
                {
                    clsuserinfo item = new clsuserinfo();

                    #region 数据
                    if (emp.Contains("_id"))
                        item._id = (emp["_id"].ToString());
                    if (emp.Contains("name"))
                        item.name = (emp["name"].AsString);
                    if (emp.Contains("password"))
                        item.password = (emp["password"].ToString());
                    if (emp.Contains("Btype"))
                        item.Btype = (emp["Btype"].AsString);
                    if (emp.Contains("denglushijian"))
                        item.denglushijian = (emp["denglushijian"].AsString);
                    if (emp.Contains("Createdate"))
                        item.Createdate = (emp["Createdate"].AsString);
                    if (emp.Contains("AdminIS"))
                        item.AdminIS = (emp["AdminIS"].AsString);

                    if (emp.Contains("jigoudaima"))
                        item.jigoudaima = (emp["jigoudaima"].AsString);
                    #endregion
                    ClaimReport_Server.Add(item);
                }
                return ClaimReport_Server;

            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex);
                return null;

                throw ex;
            }
            #endregion
        }

        public void createFapiao_Server(List<clsFAinfo> AddMAPResult)
        {
            string connectionString = "mongodb://127.0.0.1";
            connectionString = ipadress;
            MongoServer server = MongoServer.Create(connectionString);
            MongoDatabase db1 = server.GetDatabase("FA_datawork_HRB");
            MongoCollection collection1 = db1.GetCollection("FA_datawork_HRB_FA");
            MongoCollection<BsonDocument> employees1 = db1.GetCollection<BsonDocument>("FA_datawork_HRB_FA");

            // collection1.RemoveAll();
            if (AddMAPResult == null)
            {
                MessageBox.Show("No Data  input Sever", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            foreach (clsFAinfo item in AddMAPResult)
            {

                //QueryDocument query = new QueryDocument("name", item.name);
                var dd = Query.And(Query.EQ("fapiaohao", item.fapiaohao), Query.EQ("jigoudaima", item.jigoudaima), Query.EQ("fapiaoleixing", item.fapiaoleixing));//同时满足多个条件

                collection1.Remove(dd);
                BsonDocument fruit_1 = new BsonDocument
                 { 
                 { "fapiaohao", item.fapiaohao },
                 { "danganhao", item.danganhao },                 
                 { "bianhao", item.bianhao} ,
                 { "guidangrenzhanghao", item.guidangrenzhanghao} ,
                 { "jigoudaima", item.jigoudaima} ,
                 { "fapiaoleixing", item.fapiaoleixing} ,
                 { "Input_Date", item.Input_Date} ,
                 { "Lurushijian", item.Input_Date.Substring(0,8)} 
                 };
                collection1.Insert(fruit_1);
            }
        }

        public List<clsFAinfo> findAll_Fapiao()
        {

            #region Read  database info server
            try
            {
                List<clsFAinfo> ClaimReport_Server = new List<clsFAinfo>();

                string connectionString = "mongodb://127.0.0.1";
                connectionString = ipadress;
                MongoServer server = MongoServer.Create(connectionString);
                MongoDatabase db1 = server.GetDatabase("FA_datawork_HRB");
                MongoCollection collection1 = db1.GetCollection("FA_datawork_HRB_FA");
                MongoCollection<BsonDocument> employees = db1.GetCollection<BsonDocument>("FA_datawork_HRB_FA");

                foreach (BsonDocument emp in employees.FindAll())
                {
                    clsFAinfo item = new clsFAinfo();

                    #region 数据
                    if (emp.Contains("_id"))
                        item.R_id = (emp["_id"].ToString());
                    if (emp.Contains("fapiaohao"))
                        item.fapiaohao = (emp["fapiaohao"].ToString());
                    if (emp.Contains("danganhao"))
                        item.danganhao = (emp["danganhao"].ToString());
                    if (emp.Contains("bianhao"))
                        item.bianhao = (emp["bianhao"].ToString());
                    if (emp.Contains("guidangrenzhanghao"))
                        item.guidangrenzhanghao = (emp["guidangrenzhanghao"].AsString);
                    if (emp.Contains("Input_Date"))
                        item.Input_Date = (emp["Input_Date"].AsString);
                    if (emp.Contains("jigoudaima"))
                        item.jigoudaima = (emp["jigoudaima"].AsString);
                    if (emp.Contains("fapiaoleixing"))
                        item.fapiaoleixing = (emp["fapiaoleixing"].AsString);


                    #endregion
                    ClaimReport_Server.Add(item);
                }
                return ClaimReport_Server;

            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex);
                return null;

                throw ex;
            }
            #endregion
        }

        public List<clsFAinfo> findFapiao(string jigoudaima, string fapiaoleixing)
        {

            #region Read  database info server
            try
            {
                List<clsFAinfo> ClaimReport_Server = new List<clsFAinfo>();

                string connectionString = "mongodb://127.0.0.1";
                connectionString = ipadress;
                MongoServer server = MongoServer.Create(connectionString);
                MongoDatabase db1 = server.GetDatabase("FA_datawork_HRB");
                MongoCollection collection1 = db1.GetCollection("FA_datawork_HRB_FA");
                MongoCollection<BsonDocument> employees = db1.GetCollection<BsonDocument>("FA_datawork_HRB_FA");

                var query = new QueryDocument("jigoudaima", jigoudaima);

                var dd = Query.And(Query.EQ("jigoudaima", jigoudaima), Query.EQ("fapiaoleixing", fapiaoleixing));//同时满足多个条件

                foreach (BsonDocument emp in employees.Find(dd))
                {
                    clsFAinfo item = new clsFAinfo();

                    #region 数据
                    if (emp.Contains("_id"))
                        item.R_id = (emp["_id"].ToString());
                    if (emp.Contains("fapiaohao"))
                        item.fapiaohao = (emp["fapiaohao"].ToString());
                    if (emp.Contains("danganhao"))
                        item.danganhao = (emp["danganhao"].ToString());
                    if (emp.Contains("bianhao"))
                        item.bianhao = (emp["bianhao"].ToString());
                    if (emp.Contains("guidangrenzhanghao"))
                        item.guidangrenzhanghao = (emp["guidangrenzhanghao"].AsString);
                    if (emp.Contains("Input_Date"))
                        item.Input_Date = (emp["Input_Date"].AsString);
                    if (emp.Contains("jigoudaima"))
                        item.jigoudaima = (emp["jigoudaima"].AsString);
                    if (emp.Contains("fapiaoleixing"))
                        item.fapiaoleixing = (emp["fapiaoleixing"].AsString);


                    #endregion
                    ClaimReport_Server.Add(item);
                }
                return ClaimReport_Server;

            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex);
                return null;

                throw ex;
            }
            #endregion
        }
        public List<clsFAinfo> findFapiao_user(string jigoudaima, string fapiaoleixing,string guidangren)
        {

            #region Read  database info server
            try
            {
                List<clsFAinfo> ClaimReport_Server = new List<clsFAinfo>();

                string connectionString = "mongodb://127.0.0.1";
                connectionString = ipadress;
                MongoServer server = MongoServer.Create(connectionString);
                MongoDatabase db1 = server.GetDatabase("FA_datawork_HRB");
                MongoCollection collection1 = db1.GetCollection("FA_datawork_HRB_FA");
                MongoCollection<BsonDocument> employees = db1.GetCollection<BsonDocument>("FA_datawork_HRB_FA");

                var query = new QueryDocument("danganhao", jigoudaima);

                //    var dd = Query.And(Query.EQ("jigoudaima", jigoudaima), Query.EQ("fapiaoleixing", fapiaoleixing));//同时满足多个条件

                foreach (BsonDocument emp in employees.Find(query))
                {
                    clsFAinfo item = new clsFAinfo();

                    #region 数据
                    if (emp.Contains("_id"))
                        item.R_id = (emp["_id"].ToString());
                    if (emp.Contains("fapiaohao"))
                        item.fapiaohao = (emp["fapiaohao"].ToString());
                    if (emp.Contains("danganhao"))
                        item.danganhao = (emp["danganhao"].ToString());
                    if (emp.Contains("bianhao"))
                        item.bianhao = (emp["bianhao"].ToString());
                    if (emp.Contains("guidangrenzhanghao"))
                        item.guidangrenzhanghao = (emp["guidangrenzhanghao"].AsString);
                    if (emp.Contains("Input_Date"))
                        item.Input_Date = (emp["Input_Date"].AsString);
                    if (emp.Contains("jigoudaima"))
                        item.jigoudaima = (emp["jigoudaima"].AsString);
                    if (emp.Contains("fapiaoleixing"))
                        item.fapiaoleixing = (emp["fapiaoleixing"].AsString);


                    #endregion
                    ClaimReport_Server.Add(item);
                }
                // collection1.RemoveAll();
                query = new QueryDocument("fapiaohao", fapiaoleixing);
                //  IMongoQuery query1 = Query.EQ("fapiaohao", new ObjectId(fapiaoleixing));
                // var dd = Query.And(Query.EQ("fapiaohao", fapiaoleixing));
                foreach (BsonDocument emp in employees.Find(query))
                {
                    clsFAinfo item = new clsFAinfo();

                    #region 数据
                    if (emp.Contains("_id"))
                        item.R_id = (emp["_id"].ToString());
                    if (emp.Contains("fapiaohao"))
                        item.fapiaohao = (emp["fapiaohao"].ToString());
                    if (emp.Contains("danganhao"))
                        item.danganhao = (emp["danganhao"].ToString());
                    if (emp.Contains("bianhao"))
                        item.bianhao = (emp["bianhao"].ToString());
                    if (emp.Contains("guidangrenzhanghao"))
                        item.guidangrenzhanghao = (emp["guidangrenzhanghao"].AsString);
                    if (emp.Contains("Input_Date"))
                        item.Input_Date = (emp["Input_Date"].AsString);
                    if (emp.Contains("jigoudaima"))
                        item.jigoudaima = (emp["jigoudaima"].AsString);
                    if (emp.Contains("fapiaoleixing"))
                        item.fapiaoleixing = (emp["fapiaoleixing"].AsString);


                    #endregion
                    ClaimReport_Server.Add(item);
                }
                query = new QueryDocument("guidangrenzhanghao", guidangren);              
                foreach (BsonDocument emp in employees.Find(query))
                {
                    clsFAinfo item = new clsFAinfo();

                    #region 数据
                    if (emp.Contains("_id"))
                        item.R_id = (emp["_id"].ToString());
                    if (emp.Contains("fapiaohao"))
                        item.fapiaohao = (emp["fapiaohao"].ToString());
                    if (emp.Contains("danganhao"))
                        item.danganhao = (emp["danganhao"].ToString());
                    if (emp.Contains("bianhao"))
                        item.bianhao = (emp["bianhao"].ToString());
                    if (emp.Contains("guidangrenzhanghao"))
                        item.guidangrenzhanghao = (emp["guidangrenzhanghao"].AsString);
                    if (emp.Contains("Input_Date"))
                        item.Input_Date = (emp["Input_Date"].AsString);
                    if (emp.Contains("jigoudaima"))
                        item.jigoudaima = (emp["jigoudaima"].AsString);
                    if (emp.Contains("fapiaoleixing"))
                        item.fapiaoleixing = (emp["fapiaoleixing"].AsString);


                    #endregion
                    ClaimReport_Server.Add(item);
                }
              //  query = new QueryDocument("guidangrenzhanghao", guidangren);
                var query1 = Query.And(Query.GTE("Lurushijian", jigoudaima.Replace("/", "")), Query.LTE("Lurushijian", fapiaoleixing.Replace("/", "")));
                foreach (BsonDocument emp in employees.Find(query1))
                {
                    clsFAinfo item = new clsFAinfo();

                    #region 数据
                    if (emp.Contains("_id"))
                        item.R_id = (emp["_id"].ToString());
                    if (emp.Contains("fapiaohao"))
                        item.fapiaohao = (emp["fapiaohao"].ToString());
                    if (emp.Contains("danganhao"))
                        item.danganhao = (emp["danganhao"].ToString());
                    if (emp.Contains("bianhao"))
                        item.bianhao = (emp["bianhao"].ToString());
                    if (emp.Contains("guidangrenzhanghao"))
                        item.guidangrenzhanghao = (emp["guidangrenzhanghao"].AsString);
                    if (emp.Contains("Input_Date"))
                        item.Input_Date = (emp["Input_Date"].AsString);
                    if (emp.Contains("jigoudaima"))
                        item.jigoudaima = (emp["jigoudaima"].AsString);
                    if (emp.Contains("fapiaoleixing"))
                        item.fapiaoleixing = (emp["fapiaoleixing"].AsString);


                    #endregion
                    ClaimReport_Server.Add(item);
                }

                return ClaimReport_Server;

            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex);
                return null;

                throw ex;
            }
            #endregion
        }

        public void deletefapiao(clsFAinfo item)
        {
            string connectionString = "mongodb://127.0.0.1";
            connectionString = ipadress;
            MongoServer server = MongoServer.Create(connectionString);
            MongoDatabase db1 = server.GetDatabase("FA_datawork_HRB");
            MongoCollection collection1 = db1.GetCollection("FA_datawork_HRB_FA");
            MongoCollection<BsonDocument> employees = db1.GetCollection<BsonDocument>("FA_datawork_HRB_FA");

            if (item == null)
            {
                MessageBox.Show("No Data  input Sever", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //QueryDocument query = new QueryDocument("name", name);
            var dd = Query.And(Query.EQ("fapiaohao", item.fapiaohao), Query.EQ("danganhao", item.danganhao), Query.EQ("jigoudaima", item.jigoudaima));//同时满足多个条件

            collection1.Remove(dd);
        }
        public void deletefapiao_danganhao(string item)
        {
            string connectionString = "mongodb://127.0.0.1";
            connectionString = ipadress;
            MongoServer server = MongoServer.Create(connectionString);
            MongoDatabase db1 = server.GetDatabase("FA_datawork_HRB");
            MongoCollection collection1 = db1.GetCollection("FA_datawork_HRB_FA");
            MongoCollection<BsonDocument> employees = db1.GetCollection<BsonDocument>("FA_datawork_HRB_FA");

            if (item == null)
            {
                MessageBox.Show("No Data  input Sever", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            QueryDocument query = new QueryDocument("danganhao", item);
            //var dd = Query.And(Query.EQ("fapiaohao", item.danganhao));//同时满足多个条件

            collection1.Remove(query);
        }
        public void updateFA_Server(List<clsFAinfo> AddMAPResult)
        {
            string connectionString = "mongodb://127.0.0.1";
            connectionString = ipadress;
            MongoServer server = MongoServer.Create(connectionString);
            MongoDatabase db1 = server.GetDatabase("FA_datawork_HRB");
            MongoCollection collection1 = db1.GetCollection("FA_datawork_HRB_FA");
            MongoCollection<BsonDocument> employees = db1.GetCollection<BsonDocument>("FA_datawork_HRB_FA");

            //  collection1.RemoveAll();
            if (AddMAPResult == null)
            {
                MessageBox.Show("No Data  input Sever", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            foreach (clsFAinfo item in AddMAPResult)
            {
                IMongoQuery query = Query.EQ("_id", new ObjectId(item.R_id));
                //collection.Remove(query);

                //QueryDocument query = new QueryDocument("name", item.name);
                //var query = Query.And(Query.EQ("fapiaohao", item.fapiaohao), Query.EQ("danganhao", item.danganhao), Query.EQ("jigoudaima", item.jigoudaima));//同时满足多个条件

                var update = Update.Set("jigoudaima", item.jigoudaima.Trim());
                collection1.Update(query, update);
                update = Update.Set("fapiaoleixing", item.fapiaoleixing);
                collection1.Update(query, update);
                update = Update.Set("fapiaohao", item.fapiaohao);
                collection1.Update(query, update);
                update = Update.Set("bianhao", item.bianhao);
                collection1.Update(query, update);
                update = Update.Set("guidangrenzhanghao", item.guidangrenzhanghao);
                collection1.Update(query, update);
                update = Update.Set("Input_Date", item.Input_Date);
                collection1.Update(query, update);
                update = Update.Set("danganhao", item.danganhao);
                collection1.Update(query, update);
                update = Update.Set("Lurushijian", item.Input_Date.Substring(0,8));
                collection1.Update(query, update);
                
            }
        }


    }
}
