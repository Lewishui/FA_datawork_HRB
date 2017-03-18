using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FA.DB
{
    public class clsuserinfo
    {
        public string _id { get; set; }
        public string name { get; set; }
        public string password { get; set; }
        public string Btype { get; set; }
        public string denglushijian { get; set; }
        public string Createdate { get; set; }
        public string AdminIS { get; set; }
        public string jigoudaima { get; set; }
    }

    public class clsFAinfo
    {
        public string R_id { get; set; }
        public string fapiaohao { get; set; }
        public string danganhao { get; set; }
        public string bianhao { get; set; }
        public string guidangrenzhanghao { get; set; }
        public string Input_Date { get; set; }
        public string jigoudaima { get; set; }
        public string fapiaoleixing { get; set; }
        public string fapiaoshuliang { get; set; }
    }
    public class clslen_listinfo
    {
        public string R_id { get; set; }
        public string Start_No { get; set; }
        public string End_No { get; set; }
        public int len { get; set; }
    }
}
