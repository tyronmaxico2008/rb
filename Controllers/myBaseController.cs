using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Text;
using Whirlpool_logistics.Models;


namespace Whirlpool_logistics.Controllers
{
    public abstract class myBaseController : Controller
    {
        //
        // GET: /myBase/

        public ActionResult Index()
        {
            return View();
        }


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

    }
}
