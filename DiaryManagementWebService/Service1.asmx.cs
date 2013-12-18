using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Serialization;

namespace DiaryManagementWebService
{

    public class user
    {
        string username;
        string password;
    }

    /// <summary>
    /// Service1 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。
    // [System.Web.Script.Services.ScriptService]
    public class Service1 : System.Web.Services.WebService
    {
        private void XmlWriter<T>(T t, TextWriter tw)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            xs.Serialize(tw, t);
        }
        static Management loadmandata(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            XmlReader xr = XmlReader.Create(fs);
            XmlSerializer xs = new XmlSerializer(typeof(Management));
            //Console.WriteLine(((user)xs.Deserialize(xr)).username);
            //Console.WriteLine(((user)xs.Deserialize(fs)).username);
            return (Management)xs.Deserialize(xr);
        }

        private DataSet checkDiaryId(int id)
        {
            DBFunction dbf = new DBFunction();
            DataSet ds = dbf.GetDataSet(
                @"SELECT tbl_diary.*
FROM tbl_diary INNER JOIN tbl_diarylist ON tbl_diary.[ID] = tbl_diarylist.[id_diary]
WHERE (((tbl_diarylist.id_diary)=" + id + ") AND ((tbl_diarylist.id_user)=" + Session["userid"] + "))");
            if (ds.Tables[0].Rows.Count == 0)
                throw new Exception("你是不是想偷看别人的日记？？？？？");
            return ds;
        }
        [WebMethod(EnableSession = true)]
        public int uploadDiaryById(int id, Diary d)
        {
            DataSet ds = checkDiaryId(id);
            throw new NotImplementedException();
            return 0;
            

        }
        [WebMethod(EnableSession = true)]
        public Diary getNewestDiary()
        {
            DBFunction dbf = new DBFunction();
            DataSet ds = dbf.GetDataSet(
                @"SELECT TOP 1 tbl_diary.*, tbl_diarylist.id_user
FROM tbl_diary INNER JOIN tbl_diarylist ON tbl_diary.[ID] = tbl_diarylist.[id_diary]
WHERE (((tbl_diarylist.id_user)=" + Session["userid"] + @"))
ORDER BY tbl_diary.dt_lastmodified;
"
                );
            return getDiaryFromDataSet(ds);
        }

        [WebMethod(EnableSession=true)]
        public Diary getDiaryById(int id)
        {
            DBFunction dbf = new DBFunction();
            /*DataSet ds = dbf.ExecuteNonQuery("select * from tbl_diary, tbl_diarylist where tbl_diary.ID = "+id
                +"and tbl_diarylist.id_diary=tbl_diary.ID and  
             */
            DataSet ds = checkDiaryId(id);
            Diary d = getDiaryFromDataSet(ds);
            return d;
            //(string)ds.Tables[0].Rows[0]["str_management_name"];


                //"select * from tbl_diarylist where id_diary = "+id +"and id_user"); 
        }

        private static Diary getDiaryFromDataSet(DataSet ds)
        {
            Diary d = new Diary();
            d.content = (string)ds.Tables[0].Rows[0]["str_content_path"];
            d.modified = (DateTime)ds.Tables[0].Rows[0]["dt_lastmodified"];
            d.title = (string)ds.Tables[0].Rows[0]["str_title"];
            d.created = (DateTime)ds.Tables[0].Rows[0]["dt_created"];
            d.management = null;
            try
            {
                d.management = loadmandata((string)ds.Tables[0].Rows[0]["str_management_name"]);
            }
            catch (Exception)
            {
            }
            return d;
        }

        [WebMethod(EnableSession = true)]
        public int[] getDiaryList()
        {
            DBFunction db = new DBFunction();
            DataSet ds = db.GetDataSet("select id_diary from tbl_diarylist where id_user = "+Session["userid"]);
            int[] ret = new int[ds.Tables[0].Rows.Count];
           for(int i=0;i<ds.Tables[0].Rows.Count;++i)
               ret[i] = (int)ds.Tables[0].Rows[i]["id_diary"];
           return ret;
        }

        [WebMethod(EnableSession = true)]
        public void logout()
        {
            checkLoggedIn();
            Session.Abandon();
        }

        private bool isLoggedIn()
        {
            System.Diagnostics.Debug.WriteLine(Session.SessionID);
            if(Session==null)
                return false;
            if (Session.IsNewSession)
                return false;
            else if (Session.Count == 0)
                return false;
            else return (bool)Session["Logged"];
                
        }

        [WebMethod(EnableSession = true)]
        public void throwsException()
        {
            //throw new Exception("1111");
            throw new SoapException("1111", SoapException.ClientFaultCode, (string)null, (Exception)null);
        }
        [WebMethod(EnableSession=true)]
        private void checkLoggedIn()
        {
            if (!isLoggedIn())
                throw new SoapException("You have not logged in.",SoapException.ClientFaultCode);
        }
        [WebMethod(EnableSession = true)]
        public string sessionedHelloWorld()
        {
            checkLoggedIn();
            //throw new Exception("...");
            return "hello world!";
        }

        [WebMethod(EnableSession = true)]
        public bool userRegister(string username, string password)
        {
            //Session.Contents;
            
            if (isLoggedIn())
            {
                throw new SoapException("You have already logged in. Please log out first.",SoapException.ClientFaultCode);
            }
            DBFunction dbf = new DBFunction();
            DataSet ds = dbf.GetDataSet("select * from tbl_userlist where username = '" + username + "'and password = '" + password + "'");
            if (ds.Tables[0].Rows.Count == 1)
                return false;
            else
            {
                dbf.ExecuteNonQuery("insert into [tbl_userlist]([username],[password]) values('"+username+"','"+password+"')");
                return true;
            }

        }
        [WebMethod]
        public string[] return1()
        {
            return new string[] { "1","2","3"};
        }

        
        [WebMethod(EnableSession = true)]
        public string authenticate(string username, string password)
        {
            DBFunction dbf = new DBFunction();
            DataSet ds = dbf.GetDataSet("select * from tbl_userlist where username = '" + username + "' and password = '" + password + "'");
            if (ds.Tables[0].Rows.Count == 1)
            {
                Session["userid"] = ds.Tables[0].Rows[0]["ID"];
                System.Diagnostics.Debug.WriteLine(Session["userid"]);
                Session["Logged"] = true;
                System.Diagnostics.Debug.WriteLine(Session.SessionID);
                return Session.SessionID;
            }
            else
            {
                Session.Abandon();
                return null;
            }
        }

        /*[WebMethod]
        public bool upload(diary d)
        {
            return true;
        }*/
        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

    }
}