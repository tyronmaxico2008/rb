using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Whirlpool_logistics.Models;

namespace Whirlpool_logistics.Controllers
{
    public class HomeController : myBaseController
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            if (string.IsNullOrWhiteSpace(Session["username"].ToString()))
            {
                Session.Clear();
                Session.RemoveAll();
                return RedirectToAction("Login");
            }
            return View();
        }
        public ActionResult Login()
        {
            Session.Clear();
            Session.RemoveAll();
            return View();
        }

        public ActionResult ReportView()
        {
            return View();
        }
<<<<<<< HEAD
<<<<<<< HEAD
=======
=======

>>>>>>> de9b9b78bca133217e5447c2ab096b5b596b6313


        public ActionResult searchDoc()
        {
            return View();
        }
<<<<<<< HEAD
>>>>>>> v2
=======

>>>>>>> de9b9b78bca133217e5447c2ab096b5b596b6313

        public ActionResult Logout()
        {
            Session.Clear();
            Session.RemoveAll();
            return RedirectToAction("Login");
        }


    }
}
