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
        private string sConnectionString = "Data Source=.;initial Catalog=whirlpool_logistics;user id=sa;password=writer#2015";
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult getMantagList()
        {
            SqlDataAdapter ad = new SqlDataAdapter("select * from Maintag", sConnectionString);

            DataTable t = new DataTable();


            ad.Fill(t);

            string sResult = Newtonsoft.Json.JsonConvert.SerializeObject(t);

            //System.Threading.Thread.Sleep(3000);

            return Content(sResult, "application/json");


        }
        


    }
}
