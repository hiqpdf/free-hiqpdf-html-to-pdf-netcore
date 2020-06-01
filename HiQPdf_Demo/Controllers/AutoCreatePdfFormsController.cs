using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using HiQPdf;

namespace HiQPdf_Demo.Controllers
{
    public class AutoCreatePdfFormsController : Controller
    {
        // GET: AutoCreatePdfForms
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

            htmlToPdfConverter.BrowserWidth = 800;

            // automatically create a PDF form from HTML form
            htmlToPdfConverter.Document.AutoPdfForm.AutoCreatePdfForm = collection["checkBoxCreateForm"].Count > 0;

            // convert the HTML code to PDF
            byte[] pdfBuffer = htmlToPdfConverter.ConvertHtmlToMemory(collection["textBoxHtmlCode"], null);

            FileResult fileResult = new FileContentResult(pdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "AutoPdfForms.pdf";

            return fileResult;
        }
    }
}