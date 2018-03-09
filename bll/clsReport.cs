using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Whirlpool_logistics.bll
{
    class clsReport
    {

        public string getConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString;
        }


        //private class clsGenericAdapter
        //{
        //    string _con = "";

        //    public clsGenericAdapter(string sCon)
        //    {
        //        _con = sCon;
        //    }

        //    public DataTable getData(string sql)
        //    {

        //        SqlDataAdapter ad = new SqlDataAdapter(sql, _con);

        //        DataTable t = new DataTable();
        //        ad.Fill(t);

        //        return t;
        //    }

        //    public string exec(string q)
        //    {
        //        using (SqlConnection conn = new SqlConnection(_con))
        //        {
        //            conn.Open();
        //            var cmd = conn.CreateCommand();
        //            cmd.CommandText = q;

        //            cmd.ExecuteNonQuery();
        //        }
        //    }
        //}

        public DataTable getData(string sql)
        {

            SqlDataAdapter ad = new SqlDataAdapter(sql, getConnectionString());

            DataTable t = new DataTable();
            ad.Fill(t);

            return t;
        }



        public void Update_LRNo()
        {

            //Temporary Table Creation for LRNO and invoice no

            DataTable t = new DataTable();

            t.Columns.Add("LRNo", typeof(string));
            t.Columns.Add("InvoiceNo", typeof(string));

            string[] arrColumns = { "LRNo", "InvoiceNo" };

            //t.Constraints.Add(new UniqueConstraint("compositeKey1", arrColumns, false));

            //////////////////
            //Required table creation
            DataTable tFile = getData("select * from mFile");
            DataTable tIndexFieldData = getData("select * from vpageIndexData");

            //Function 
            Func<int, string, string> getIndexFieldVal = delegate(int iFileID, string sField)
            {

                DataRow[] rows = tIndexFieldData.Select(" mFileID = " + iFileID + " and indexVal = '" + sField + "'");

                if (rows.Length > 0)
                {

                    return rows[0]["val"].ToString();

                }

                return "";
            };

            /////////////////////////////////////////////////////////////////////////

            using (SqlConnection _conn = new SqlConnection(getConnectionString()))
            {
                _conn.Open();

                foreach (DataRow r in tFile.Rows)
                {
                    int iID = Convert.ToInt32(r["id"].ToString());
                    string sLRNo = getIndexFieldVal(iID, "LR No");
                    string sSTNNo = getIndexFieldVal(iID, "STN No");


                    if (!string.IsNullOrWhiteSpace(sLRNo) && !string.IsNullOrWhiteSpace(sSTNNo))
                    {

                        string[] sInvoiceNos = sSTNNo.Split(',');

                        foreach (string sInvoiceNo in sInvoiceNos)
                        {
                            InsertLRInMaster(_conn, sLRNo, sInvoiceNo);


                            Console.Write("LR NO : {0}  , STN No : {1} \n ", sLRNo, sInvoiceNo);
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(sSTNNo) || !string.IsNullOrWhiteSpace(sLRNo))
                        Update_mFile(_conn, iID, sLRNo, sSTNNo);
                }

                finalUpdate(_conn);
            }


            //Console.ReadLine();

        }

        private static void Update_mFile(SqlConnection _conn
            , int iFileID
            , string sLRNo
            , string sInvoiceNo)
        {
            SqlCommand cmd = _conn.CreateCommand();
            cmd.CommandText = "update mFile set LRNo = @LRNo , InvoiceNo = @InvoiceNo where id = " + iFileID;
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("LRNo", sLRNo);
            cmd.Parameters.AddWithValue("InvoiceNo", sInvoiceNo);
            cmd.ExecuteNonQuery();
        }

        private static void InsertLRInMaster(SqlConnection _conn, string sLRNo, string sInvoiceNo)
        {
            SqlCommand cmd = _conn.CreateCommand();
            cmd.CommandText = "mLR_insert";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("LRNo", sLRNo);
            cmd.Parameters.AddWithValue("InvoiceNo", sInvoiceNo);
            cmd.ExecuteNonQuery();
        }


        private void finalUpdate(SqlConnection _conn)
        {
            SqlCommand cmd = _conn.CreateCommand();
            cmd.CommandText = @"update mFile 
            set mFile.LRNO = mLR.LRNo
            from mFile left join mLR on mFile.InvoiceNo = mLR.InvoiceNo ";

            cmd.ExecuteNonQuery();

        }

        public void test()
        {
            Update_LRNo();
        }


    }
}