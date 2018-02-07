using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using Whirlpool_logistics.Models;


namespace Whirlpool_logistics.Controllers
{
    public class ServiceController : Controller
    {
        //
        // GET: /Service/
        public string getConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString;
        }

        public DataTable getData(string sql)
        {

            SqlDataAdapter ad = new SqlDataAdapter(sql, getConnectionString());

            DataTable t = new DataTable();


            ad.Fill(t);

            return t;

        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult getMaintagList()
        {
            DataTable t = getData("select * from Maintag ");
            string sResult = Newtonsoft.Json.JsonConvert.SerializeObject(t);
            return Content(sResult, "application/json");
        }

        [HttpPost]
        public ActionResult getSubTagList()
        {
            if (Request.Form["maintagid"] != null)
            {
                DataTable t = getData("select * from subtag where maintagid = " + Request.Form["maintagid"]);
                string sResult = Newtonsoft.Json.JsonConvert.SerializeObject(t);
                return Content(sResult, "application/json");
            }
            return Content("", "application/json");
        }

        [HttpPost]
        public ActionResult getIndexFieldList()
        {
            if (Request.Form["subtagid"] != null)
            {
                DataTable t = getData("select * from IndexFields where subtagid = " + Request.Form["subtagid"]);
                string sResult = Newtonsoft.Json.JsonConvert.SerializeObject(t);
                return Content(sResult, "application/json");
            }
            return Content("", "application/json");
        }


        [HttpPost]
        public ActionResult userlogin()
        {
            var a = Request.Form["username"];
            var b = Request.Form["password"];
            var c = "Select Count(uname) from users where uname='" + a + "' and pass='" + b + "'";
            var t = getData("Select * from users where uname='" + Request.Form["username"] + "' and pass='" + Request.Form["password"] + "'");
            if (t != null && t.Rows.Count  >0)
            {
                string sFirstName = t.Rows[0]["uname"].ToString();
                
                string sResult = Newtonsoft.Json.JsonConvert.SerializeObject(t);
                return Json( new { username  = sFirstName , result = true},JsonRequestBehavior.AllowGet);
            }
            return Json( new { username  = "" , result = false},JsonRequestBehavior.AllowGet);
        }


        //public int userlogin(user us)
        //{
        //    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString);


        //    SqlCommand com = new SqlCommand("Sp_User_login", con);

        //    com.CommandType = CommandType.StoredProcedure;

        //    com.Parameters.AddWithValue("@Username", us.uname);

        //    com.Parameters.AddWithValue("@Password", us.pass);

        //    SqlParameter oblogin = new SqlParameter();

        //    oblogin.ParameterName = "@Isvalid";

        //    oblogin.Direction = ParameterDirection.Output;

        //    oblogin.SqlDbType = SqlDbType.Bit;

        //    com.Parameters.Add(oblogin);

        //    con.Open();

        //    com.ExecuteNonQuery();

        //    int res = Convert.ToInt32(oblogin.Value);

        //    con.Close();

        //    return res;

        //}
    }
}
