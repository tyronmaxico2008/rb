using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp.text.pdf;
using iTextSharp;
using iTextSharp.text;
using System.Configuration;

namespace Whirlpool_logistics.bll
{
    public class clsPDF
    {

        private string getPDFPath(string fileBarcode)
        {
            string sPath = ConfigurationManager.AppSettings["docPath"] + fileBarcode + ".pdf";
            return sPath;
        }
        
        public void savePage(string fileBarCode
            ,string ImageName, int iPageNum)
        {


            string sSource = getPDFPath(fileBarCode);
            string sDestination  = ConfigurationManager.AppSettings["uploadPath"] + ImageName +".pdf";

            //string finalImagePage = ConfigurationManager.AppSettings["uploadPath"] + fileBarcode "_" iPage + ".pdf";
            ExtractPages(sSource,sDestination,iPageNum,iPageNum);

        }



        public  void ExtractPages(string sourcePDFpath, string outputPDFpath, int startpage, int endpage)
        {
            PdfReader reader = null;
            Document sourceDocument = null;
            PdfCopy pdfCopyProvider = null;
            PdfImportedPage importedPage = null;

            reader = new PdfReader(sourcePDFpath);
            sourceDocument = new Document(reader.GetPageSizeWithRotation(startpage));
            pdfCopyProvider = new PdfCopy(sourceDocument, new System.IO.FileStream(outputPDFpath, System.IO.FileMode.Create));

            sourceDocument.Open();

            for (int i = startpage; i <= endpage; i++)
            {
                importedPage = pdfCopyProvider.GetImportedPage(reader, i);
                pdfCopyProvider.AddPage(importedPage);
            }
            sourceDocument.Close();
            reader.Close();
        }
    }
}