using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Whirlpool_logistics.Controllers
{
    public class UserController : Controller
    {
        //
        // GET: /User/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CreateUser()
        {
            return View();
        }

        public ActionResult CreateRole()
        {
            return View();
        }
        
        public ActionResult BulkUpload()
        {
            return View();
        }
    }
}
