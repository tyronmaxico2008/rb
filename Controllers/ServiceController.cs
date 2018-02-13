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
    public class ServiceController : myBaseController
    {
        //
        // GET: /Service/
        

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
        public ActionResult getTotalDoc()
        {
            DataTable t = getData("Select count(*) as rec_count from dump where flag is null and docLocation='" + Request.Form["docLocation"] + "'");
            string sResult = Newtonsoft.Json.JsonConvert.SerializeObject(t);
            return Content(sResult, "application/json");

        }

        [HttpPost]
        public ActionResult userlogin()
        {
            //var a = Request.Form["username"];
            //var b = Request.Form["password"];
            //var c = "Select Count(uname) from users where uname='" + a + "' and pass='" + b + "'";
            var t = getData("Select * from users where uname='" + Request.Form["username"] + "' and pass='" + Request.Form["password"] + "'");
            if (t != null && t.Rows.Count > 0)
            {
                string sFirstName = t.Rows[0]["uname"].ToString();
                string uid = t.Rows[0]["uid"].ToString();
                Session["username"] = sFirstName;
                Session["userid"] = uid;
                string sResult = Newtonsoft.Json.JsonConvert.SerializeObject(t);
                return Json(new { username = sFirstName, result = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { username = "", result = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult getFileBarCodeData()
        {
            string sBarcode = Request.Form["getbarcode"];
            string doclocation = Request.Form["docLocation"];

            StringBuilder sb1 = new StringBuilder();

            sb1.Append("select top 1 * from dump where flag is null and docLocation='" + doclocation + "'");

            if (!string.IsNullOrWhiteSpace(sBarcode)) sb1.AppendFormat(" and FileBarcode='{0}'", sBarcode);
            DataTable t = getData(sb1.ToString());
            if (t.Rows.Count > 0)
            {
                using (SqlConnection conn = new SqlConnection(getConnectionString()))
                {
                    conn.Open();
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = "update dump set flag='y' where FileBarcode='" + t.Rows[0]["FileBarcode"].ToString() + "'";
                    cmd.ExecuteNonQuery();
                }
            }


            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(t), "application/json");

        }



        public ActionResult fetchPDF()
        {
            string sPath = ConfigurationManager.AppSettings["docPath"] + Request.QueryString["fileBarCode"] + ".pdf";


            byte[] data = System.IO.File.ReadAllBytes(sPath);


            return File(data, "application/pdf");
        }

        [HttpPost]
        public ActionResult save_indexField()
        {

            if (string.IsNullOrWhiteSpace(Request.Form["maintagid"]) || string.IsNullOrWhiteSpace(Request.Form["subtagid"]))
            {
                return Json(new { msg = "Please specify Main Tag and Sub Tag !", result = false }, JsonRequestBehavior.AllowGet);
            }


            string dumpid = Request.Form["dumpid"];
            string fileBarCode = Request.Form["fileBarCode"];
            string currentPage = Request.Form["currentPage"];
            int iMainTagID = Convert.ToInt32(Request.Form["maintagid"]);
            int iSubTagID = Convert.ToInt32(Request.Form["subtagid"]);
            string imageName = fileBarCode + "_" + currentPage;
            string json_data = Request.Form["indexField"];
            string docLocation = Request.Form["docLocation"];

            //Duplicate validation
            DataTable tFile = getData("select * from mFile where imageName = '" + imageName + "'");

            if (tFile.Rows.Count > 0)
            {
                string sMsg = string.Format("The image already exists [{0}] !", imageName);
                return Json(new { msg = sMsg, result = false }, JsonRequestBehavior.AllowGet);
            }

            //Duplicate validation
            DataTable tFile2 = getData("select * from docDelete where imageName = '" + imageName + "'");

            if (tFile2.Rows.Count > 0)
            {
                string sMsg = string.Format("The image already exists [{0}] !", imageName);
                return Json(new { msg = sMsg, result = false }, JsonRequestBehavior.AllowGet);
            }


            using (SqlConnection conn = new SqlConnection(getConnectionString()))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "insert into mfile (dumpid,fileBarCode,imageName,maintagid,subtagid,docLocation) Values(@dumpid,@fileBarCode,@imageName,@maintagid,@subtagid,@docLocation)";
                cmd.CommandText += "\r\n  select SCOPE_IDENTITY() ";

                cmd.Parameters.AddWithValue("@dumpid", dumpid);
                cmd.Parameters.AddWithValue("@fileBarCode", fileBarCode);
                cmd.Parameters.AddWithValue("@imageName", imageName);
                cmd.Parameters.AddWithValue("maintagid", iMainTagID);
                cmd.Parameters.AddWithValue("subtagid", iSubTagID);
                cmd.Parameters.AddWithValue("docLocation", docLocation);

                int iID = Convert.ToInt32(cmd.ExecuteScalar());

                /////////mFile Record updated


                cmd.Parameters.Clear();
                cmd.CommandText = "exec mFile_inserIndexField " + iID;
                cmd.ExecuteNonQuery();

                DataTable tblIndexField = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(json_data);

                foreach (DataRow r in tblIndexField.Rows)
                {
                    var cmd2 = conn.CreateCommand();
                    cmd2.CommandText = "update pageIndexData set val = @val where mFileID = @mFileID and index_id = @index_id ";
                    cmd2.Parameters.AddWithValue("index_id", r["indexid"]);
                    cmd2.Parameters.AddWithValue("mFileID", iID);
                    cmd2.Parameters.AddWithValue("val", r["val"].ToString());
                    cmd2.ExecuteNonQuery();
                }

                var oPDF = new bll.clsPDF();
                oPDF.savePage(fileBarCode, imageName, Convert.ToInt32(currentPage));
            }


            return Json(new { username = "", result = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult delete()
        {
            string dumpid = Request.Form["dumpid"];
            string fileBarCode = Request.Form["fileBarCode"];
            string currentPage = Request.Form["currentPage"];
            //int iMainTagID = 0;
            //int iSubTagID = 0;
            string imageName = fileBarCode + "_" + currentPage;
            //string json_data = Request.Form["indexField"];
            string docLocation = Request.Form["docLocation"];

            //Duplicate validation
            DataTable tFile = getData("select * from docDelete where imageName = '" + imageName + "'");

            if (tFile.Rows.Count > 0)
            {
                string sMsg = string.Format("The image already exists [{0}] !", imageName);
                return Json(new { msg = sMsg, result = false }, JsonRequestBehavior.AllowGet);
            }

            //Duplicate validation
            DataTable tFile2 = getData("select * from mfile where imageName = '" + imageName + "'");

            if (tFile2.Rows.Count > 0)
            {
                string sMsg = string.Format("The image already exists [{0}] !", imageName);
                return Json(new { msg = sMsg, result = false }, JsonRequestBehavior.AllowGet);
            }

            using (SqlConnection conn = new SqlConnection(getConnectionString()))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "insert into docDelete (dumpid,fileBarCode,imageName,docLocation) Values(@dumpid,@fileBarCode,@imageName,@docLocation)";
                cmd.CommandText += "\r\n  select SCOPE_IDENTITY() ";

                cmd.Parameters.AddWithValue("@dumpid", dumpid);
                cmd.Parameters.AddWithValue("@fileBarCode", fileBarCode);
                cmd.Parameters.AddWithValue("@imageName", imageName);
                //cmd.Parameters.AddWithValue("maintagid", iMainTagID);
                //cmd.Parameters.AddWithValue("subtagid", iSubTagID);
                cmd.Parameters.AddWithValue("docLocation", docLocation);
                int iID = Convert.ToInt32(cmd.ExecuteScalar());

                /////////mFile Record updated


                //cmd.Parameters.Clear();
                //cmd.CommandText = "exec mFile_inserIndexField " + iID;
                cmd.ExecuteNonQuery();

                //DataTable tblIndexField = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(json_data);

                //foreach (DataRow r in tblIndexField.Rows)
                //{
                //    var cmd2 = conn.CreateCommand();
                //    cmd2.CommandText = "update pageIndexData set val = @val where mFileID = @mFileID and index_id = @index_id ";
                //    cmd2.Parameters.AddWithValue("index_id", r["indexid"]);
                //    cmd2.Parameters.AddWithValue("mFileID", iID);
                //    cmd2.Parameters.AddWithValue("val", r["val"].ToString());
                //    cmd2.ExecuteNonQuery();
                //}

                var oPDF = new bll.clsPDF();
                oPDF.deletePage(fileBarCode, imageName, Convert.ToInt32(currentPage));
            }


            return Json(new { username = "", result = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult setViewData()
        {
            if (string.IsNullOrWhiteSpace(Request.Form["maintagid"]) || string.IsNullOrWhiteSpace(Request.Form["subtagid"]))
            {
                return Json(new { msg = "Please specify Main Tag and Sub Tag !", result = false }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                string s = "Select * from vpageIndexData where maintagid='" + Request.Form["maintagid"] + "' and subtagid='" + Request.Form["subtagid"] + "'";
                DataTable t = getData(s);
                Session["reportData"] = t;
                if (t.Rows.Count > 0)
                    return Json(new { msg = "", result = true }, JsonRequestBehavior.AllowGet);
                else {
                    return Json(new { msg = "", result = false }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { msg = ex.Message, result = false }, JsonRequestBehavior.AllowGet);
            }

        }


        public ActionResult downloadReport()
        {
            string sFileType = "Excel";


            var rpt = new Microsoft.Reporting.WebForms.LocalReport();

            //rpt.ReportPath = "Rpt01.rdlc";
            rpt.EnableExternalImages = true;

            //var t = getData("select * from vpageIndexData");
            var t = Session["reportData"] as DataTable;
            var o = new Microsoft.Reporting.WebForms.ReportDataSource("DataSet1", t);
            rpt.DataSources.Add(o);

            //string sReportName = Request.QueryString["ReportName"];
            string sReportPath = Server.MapPath("~/Report/Report1.rdl");
            System.IO.MemoryStream ms = new System.IO.MemoryStream(System.IO.File.ReadAllBytes(sReportPath));
            rpt.LoadReportDefinition(ms);


            Byte[] results = rpt.Render(sFileType);

            return File(results, "application/unknown", "report.xls");
            //return results;

        }
    }
}
