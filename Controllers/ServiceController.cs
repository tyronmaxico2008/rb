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
using System.IO;
using System.Globalization;


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

        public ActionResult getUserList()
        {
            DataTable t = getData("select * from Users ");
            string sResult = Newtonsoft.Json.JsonConvert.SerializeObject(t);
            return Content(sResult, "application/json");
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

        [HttpPost]
        public ActionResult releaseDoc()
        {
            using (SqlConnection conn = new SqlConnection(getConnectionString()))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "update dump set flag=null where FileBarcode='" + Request.Form["FileBarcode"] + "'";
                cmd.ExecuteNonQuery();
            }
            return Content("", "application/json");
        }

        public ActionResult fetchPDF()
        {
            string sPath = ConfigurationManager.AppSettings["docPath"] + Request.QueryString["fileBarCode"] + ".pdf";


            byte[] data = System.IO.File.ReadAllBytes(sPath);


            return File(data, "application/pdf");
        }

        public ActionResult fetchPDFSearch()
        {
            string sPath = ConfigurationManager.AppSettings["uploadPath"] + Request.QueryString["imageName"] + ".pdf";


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
                cmd.CommandText = "insert into mfile (dumpid,fileBarCode,imageName,maintagid,subtagid,docLocation,userid) Values(@dumpid,@fileBarCode,@imageName,@maintagid,@subtagid,@docLocation,@userid)";
                cmd.CommandText += "\r\n  select SCOPE_IDENTITY() ";

                cmd.Parameters.AddWithValue("@dumpid", dumpid);
                cmd.Parameters.AddWithValue("@fileBarCode", fileBarCode);
                cmd.Parameters.AddWithValue("@imageName", imageName);
                cmd.Parameters.AddWithValue("maintagid", iMainTagID);
                cmd.Parameters.AddWithValue("subtagid", iSubTagID);
                cmd.Parameters.AddWithValue("docLocation", docLocation);
                cmd.Parameters.AddWithValue("userid", Session["userid"].ToString());

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
                cmd.CommandText = "insert into docDelete (dumpid,fileBarCode,imageName,docLocation,userid) Values(@dumpid,@fileBarCode,@imageName,@docLocation,@userid)";
                cmd.CommandText += "\r\n  select SCOPE_IDENTITY() ";

                cmd.Parameters.AddWithValue("@dumpid", dumpid);
                cmd.Parameters.AddWithValue("@fileBarCode", fileBarCode);
                cmd.Parameters.AddWithValue("@imageName", imageName);
                //cmd.Parameters.AddWithValue("maintagid", iMainTagID);
                //cmd.Parameters.AddWithValue("subtagid", iSubTagID);
                cmd.Parameters.AddWithValue("docLocation", docLocation);
                cmd.Parameters.AddWithValue("userid", Session["userid"].ToString());


                cmd.ExecuteNonQuery();

                var oPDF = new bll.clsPDF();
                oPDF.deletePage(fileBarCode, imageName, Convert.ToInt32(currentPage));
            }


            return Json(new { username = "", result = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult setViewData()
        {
            if (string.IsNullOrWhiteSpace(Request.Form["maintagid"]))
            {
                return Json(new { msg = "Please specify Main Tag !", result = false }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                string s = "Select * from vpageIndexData where maintagid='" + Request.Form["maintagid"] + "'and docLocation='" + Request.Form["docLocation"] + "'";
                DataTable t = getData(s);
                Session["reportData"] = t;
                if (t.Rows.Count > 0)
                    return Json(new { msg = "", result = true }, JsonRequestBehavior.AllowGet);
                else
                {
                    return Json(new { msg = "", result = false }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { msg = ex.Message, result = false }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult setViewDataUserWise()
        {
            if (string.IsNullOrWhiteSpace(Request.Form["maintagid"]))
            {
                return Json(new { msg = "Please specify Main Tag !", result = false }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                string s = "Select * from vpageIndexData where maintagid='" + Request.Form["maintagid"] + "'and uid='" + Request.Form["uid"] + "'";
                DataTable t = getData(s);
                Session["reportData"] = t;
                if (t.Rows.Count > 0)
                    return Json(new { msg = "", result = true }, JsonRequestBehavior.AllowGet);
                else
                {
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
            string filename = "Report" + DateTime.Now.ToString("yyyy-MMM-dd-HHmmss") + ".xls";
            return File(results, "application/unknown", filename);
            //return results;

        }

        [HttpPost]
        public ActionResult getData_filelist()
        {

            StringBuilder sb1 = new StringBuilder();

            sb1.AppendFormat("select * from vmfile where 1 = 1 ");

            if (!string.IsNullOrWhiteSpace(Request.Form["maintagid"]))
                sb1.AppendFormat(" and maintagid = '" + Request.Form["maintagid"] + "'");

            //if (!string.IsNullOrWhiteSpace(Request.Form["subtagid"]))
            //    sb1.AppendFormat(" and subtagid = '" + Request.Form["subtagid"] + "'");


            DataTable tIndexField = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(Request.Form["indexFieldData"]);

            if (!tIndexField.Columns.Contains("val"))
            {
                tIndexField.Columns.Add("val", typeof(string));
            }


            if (tIndexField.Rows.Count > 0)
            {
                StringBuilder sbIn = new StringBuilder();

                sbIn.AppendLine("select distinct mFileID  from dbo.vpageIndexData where 1=1 ");
                foreach (DataRow r in tIndexField.Rows)
                {
                    if (!string.IsNullOrWhiteSpace(r["val"].ToString()))
                    {
                        //sbIn.AppendFormat(" and subtagid = {0} and val Like '%{1}%' ", r["subtagid"], r["val"].ToString());
                        sbIn.AppendFormat("  and val Like '%{0}%' ", r["val"].ToString());
                    }

                }

                sb1.AppendFormat(" and  id in ({0})", sbIn.ToString());

            }




            DataTable t = getData(sb1.ToString());


            string sResult = Newtonsoft.Json.JsonConvert.SerializeObject(t);

            return Content(sResult, "application/json");
        }

        [HttpPost]
        public ActionResult fetchIndexField()
        {
            StringBuilder sb1 = new StringBuilder();

            sb1.AppendFormat("select * from vpageIndexData where 1 = 1 and mFileId=" + Request.Form["id"]);
            DataTable t = getData(sb1.ToString());

            string sResult = Newtonsoft.Json.JsonConvert.SerializeObject(t);

            return Content(sResult, "application/json");
        }

        


        [HttpPost]
        public ActionResult update_indexField()
        {

            if (string.IsNullOrWhiteSpace(Request.Form["maintagid"]) || string.IsNullOrWhiteSpace(Request.Form["subtagid"]))
            {
                return Json(new { msg = "Please specify Main Tag and Sub Tag !", result = false }, JsonRequestBehavior.AllowGet);
            }


            string id = Request.Form["id"];
            int iMainTagID = Convert.ToInt32(Request.Form["maintagid"]);
            int iSubTagID = Convert.ToInt32(Request.Form["subtagid"]);
            string json_data = Request.Form["indexField"];


            using (SqlConnection conn = new SqlConnection(getConnectionString()))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "update mfile set maintagid=@maintagid, subtagid=@subtagid, userid=@userid where id=@id";
                //cmd.CommandText += "\r\n  select SCOPE_IDENTITY() ";

                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("maintagid", iMainTagID);
                cmd.Parameters.AddWithValue("subtagid", iSubTagID);
                cmd.Parameters.AddWithValue("userid", Session["userid"].ToString());

                int iID = Convert.ToInt32(cmd.ExecuteScalar());

                /////////mFile Record updated

                //update index
                var cmd3 = conn.CreateCommand();
                cmd3.CommandText = "delete from pageIndexData where mFileID=@id";
                cmd3.Parameters.AddWithValue("@id", id);
                cmd3.ExecuteNonQuery();

                //update ends

                cmd.Parameters.Clear();
                cmd.CommandText = "exec mFile_inserIndexField " + id;
                cmd.ExecuteNonQuery();

                DataTable tblIndexField = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(json_data);

                foreach (DataRow r in tblIndexField.Rows)
                {
                    var cmd2 = conn.CreateCommand();
                    cmd2.CommandText = "update pageIndexData set val = @val where mFileID = @mFileID and index_id = @index_id ";
                    cmd2.Parameters.AddWithValue("index_id", r["indexid"]);
                    cmd2.Parameters.AddWithValue("mFileID", id);
                    cmd2.Parameters.AddWithValue("val", r["val"].ToString());
                    cmd2.ExecuteNonQuery();
                }

            }


            return Json(new { username = "", result = true }, JsonRequestBehavior.AllowGet);
        }


        public DataTable getCSVData(string sPath)
        {
            string CSVFilePathName = sPath;
            string[] Lines = System.IO.File.ReadAllLines(CSVFilePathName);
            string[] Fields;
            Fields = Lines[0].Split(new char[] { ',' });
            int Cols = Fields.GetLength(0);
            DataTable dt = new DataTable();
            //1st row must be column names; force lower case to ensure matching later on.
            for (int i = 0; i < Cols; i++)
                dt.Columns.Add(Fields[i].ToLower(), typeof(string));
            DataRow Row;
            for (int i = 1; i < Lines.GetLength(0); i++)
            {
                Fields = Lines[i].Split(new char[] { ',' });
                Row = dt.NewRow();
                for (int f = 0; f < Cols; f++)
                    Row[f] = Fields[f];
                dt.Rows.Add(Row);
            }

            return dt;
        }

        [HttpPost]
        public ActionResult bulkUpload()
        {
            if (Request.Files.Count > 0)
            {
                //string tempPath = Path.GetTempPath();
                string tempPath = Server.MapPath("~/tmp") + "\\dump" + DateTime.Now.ToString("yyyy-MMM-dd-HHmmss") + ".csv";
                Request.Files[0].SaveAs(tempPath);
                DataTable t = getCSVData(tempPath);

                using (SqlConnection conn = new SqlConnection(getConnectionString()))
                {
                    //conn.Open();
                    //var cmd = conn.CreateCommand();
                    //cmd.CommandText = "insert into dump (BoxBarcode,FileBarcode,InvoiceNo,InvoiceDate,SRNNo,SRNDate,ARADocNo,ARACity,STNNo,STNDate,STNFrom,STNTo,LRNo,Comment,BillingLocation,CustomerName,SANNo,InventoryAdj,SANA,docLocation) values (@BoxBarcode,@FileBarcode,@InvoiceNo,@InvoiceDate,@SRNNo,@SRNDate,@ARADocNo,@ARACity,@STNNo,@STNDate,@STNFrom,@STNTo,@LRNo,@Comment,@BillingLocation,@CustomerName,@SANNo,@InventoryAdj,@SANA,@docLocation)";

                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(conn))
                    {
                        //Set the database table name
                        sqlBulkCopy.DestinationTableName = "dbo.Dump";

                        //[OPTIONAL]: Map the DataTable columns with that of the database table
                        sqlBulkCopy.ColumnMappings.Add("Box Barcode", "BoxBarcode");
                        sqlBulkCopy.ColumnMappings.Add("FileBarcode", "FileBarcode");
                        sqlBulkCopy.ColumnMappings.Add("Sales Invoice No", "InvoiceNo");
                        sqlBulkCopy.ColumnMappings.Add("Sales Invoice Date", "InvoiceDate");
                        sqlBulkCopy.ColumnMappings.Add("SRN No", "SRNNo");
                        sqlBulkCopy.ColumnMappings.Add("SRN Date", "SRNDate");
                        sqlBulkCopy.ColumnMappings.Add("ARA Doc No", "ARADocNo");
                        sqlBulkCopy.ColumnMappings.Add("ARA City", "ARACity");
                        sqlBulkCopy.ColumnMappings.Add("STN No", "STNNo");
                        sqlBulkCopy.ColumnMappings.Add("STN Date", "STNDate");
                        sqlBulkCopy.ColumnMappings.Add("STN From", "STNFrom");
                        sqlBulkCopy.ColumnMappings.Add("STN To", "STNTo");
                        sqlBulkCopy.ColumnMappings.Add("LRNo", "LRNo");
                        sqlBulkCopy.ColumnMappings.Add("Comment", "Comment");
                        sqlBulkCopy.ColumnMappings.Add("Billing Location", "BillingLocation");
                        sqlBulkCopy.ColumnMappings.Add("Customer Name", "CustomerName");
                        sqlBulkCopy.ColumnMappings.Add("Inventory Adj", "InventoryAdj");
                        sqlBulkCopy.ColumnMappings.Add("SAN A", "SANA");
                        sqlBulkCopy.ColumnMappings.Add("Location", "docLocation");
                        conn.Open();
                        sqlBulkCopy.WriteToServer(t);
                        conn.Close();
                    }


                }

            }

            return Json(new { bulkdata = "", result = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult setViewDataDocumentWise()
        {
            string frmdate = (Request.Form["frmdatepicker"].ToString());
            string todate = Request.Form["todatepicker"].ToString();

            StringBuilder sb1 = new StringBuilder();

            sb1.AppendFormat("select users.uname,COUNT(distinct mfileID) as pagecount, COUNT (DISTINCT filebarcode) as totaldoc,entrydate from vpageIndexData left join users on vpageIndexData.uid=users.uid where entrydate between '" + frmdate + "' and '" + todate + "' group by users.uname,vpageindexdata.entrydate");
            DataTable t = getData(sb1.ToString());

            string sResult = Newtonsoft.Json.JsonConvert.SerializeObject(t);

            return Content(sResult, "application/json");
        }

    }
}
