using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Text;

namespace Whirlpool_logistics.Controllers
{
    public class Service2Controller : myBaseController
    {
        //
        // GET: /Service2/



        int _start = 0;
        int _length = 10;
        int _draw = 1;

        protected void setPageSize(int iPageSize)
        {
            _length = iPageSize;
        }

        public int start
        {
            get
            {
                //get start to skip data
                if (Request["start"] != null && Request["start"] != "")
                    _start = Convert.ToInt32(Request["start"]);

                return _start;
            }
        }


        public int length
        {
            get
            {
                if (Request["length"] != null && Request["length"] != "")
                    _length = Convert.ToInt32(Request["length"]);

                return _length;
            }
        }

        public int draw
        {
            get
            {
                if (Request["draw"] != null && Request["draw"] != "")
                    _draw = Convert.ToInt32(Request["draw"]);

                return _draw;
            }
        }

        
        
        public ActionResult Index()
        {
            return View();
        }

        public string test1()
        {
            return "aj";
        }


        private ContentResult getPagingData(DataTable t,int _Start,int _Length)
        {

            var result = new { recordsTotal = t.Rows.Count, data = g.GetTableRows(t, _Start * _Length, _Length) };

            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(result), "application/json");

        }


        public ContentResult sysobjects(FormCollection frm)
        {
         
   
            DataTable t = getData("select * from sysObjects");
            //var t = _oBL.getGridJson(sModuleName, sSubModuleName, sSortType, draw, start, length, cmd);
            return getPagingData(t, start, length);
            
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

            return getPagingData(t, start, length);
           
        }


    }
}
