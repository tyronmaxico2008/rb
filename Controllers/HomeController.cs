﻿using System;
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

        public ActionResult userReportView()
        {
            return View();
        }

        public ActionResult searchDoc()
        {
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.RemoveAll();
            return RedirectToAction("Login");
        }


    }
}
