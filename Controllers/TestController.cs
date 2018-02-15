using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Whirlpool_logistics.Controllers
{
    public class TestController : myBaseController
    {
        //
        // GET: /Test/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult grid()
        {
            return View();
        }

    }
}
