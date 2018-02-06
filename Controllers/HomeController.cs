using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Whirlpool_logistics.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }


        public FileStreamResult GetPdf()
        {
            return new FileStreamResult(Response.OutputStream, "application/pdf") { FileDownloadName = "download.pdf" };
        }
        

    }
}
