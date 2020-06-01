using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using HiQPdf;

namespace HiQPdf_Demo.Controllers
{
    public class AutoOutlinesAndLinksController : Controller
    {
        // GET: AutoOutlinesAndLinks
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ConvertToPdf(IFormCollection collection)
        {
            // create the HTML to PDF converter
            HtmlToPdf htmlToPdfConverter = new HtmlToPdf();

            // set a demo serial number
            htmlToPdfConverter.SerialNumber = "YCgJMTAE-BiwJAhIB-EhlWTlBA-UEBRQFBA-U1FOUVJO-WVlZWQ==";

            if (collection["checkBoxHierarchical"].Count > 0)
            {
                // automatically outline the H1 to H6 tags in a hierarchy
                htmlToPdfConverter.Document.Outlines.OutlineHeadingTags = true;
            }
            else
            {
                // create outlines for each chapter and for table of contents
                // the CSS selectors mean all the H1 and H2 elements with the CSS class name 'pdf_outlines'
                htmlToPdfConverter.Document.Outlines.OutlinedElements = new string[] { "H1[class=\"pdf_outlines\"]", "H2[class=\"pdf_outlines\"]" };
            }

            // make the outlines visible in viewer
            htmlToPdfConverter.Document.Viewer.PageMode = PdfPageMode.Outlines;

            // convert the HTML code to PDF
            byte[] pdfBuffer = htmlToPdfConverter.ConvertHtmlToMemory(collection["textBoxHtmlCode"], null);

            FileResult fileResult = new FileContentResult(pdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "AutoOutlines.pdf";

            return fileResult;
        }
    }
}