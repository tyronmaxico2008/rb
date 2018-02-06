using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;



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
            DataTable t = getData("select * from subtag where maintagid = " + Request.Form["maintagid"]);
            string sResult = Newtonsoft.Json.JsonConvert.SerializeObject(t);
            return Content(sResult, "application/json");
        }

        [HttpPost]
        public ActionResult getIndexFieldList()
        {
            DataTable t = getData("select * from IndexFields where subtagid = " + Request.Form["subtagid"]);
            string sResult = Newtonsoft.Json.JsonConvert.SerializeObject(t);
            return Content(sResult, "application/json");
        }

    }
}
