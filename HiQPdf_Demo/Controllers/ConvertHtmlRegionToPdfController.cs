using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using HiQPdf;

namespace HiQPdf_Demo.Controllers
{
    public class ConvertHtmlRegionToPdfController : Controller
    {
        // GET: ConvertHtmlRegionToPdf
        public ActionResult Index()
        {
            SetCrtPageUri();

            return View();
        }

        [HttpPost]
        public ActionResult ConvertToPdf(IFormCollection collection)
        {
            // create the HTML to PDF converter
            HtmlToPdf htmlToPdfConverter = new HtmlToPdf();

            // set a demo serial number
            htmlToPdfConverter.SerialNumber = "YCgJMTAE-BiwJAhIB-EhlWTlBA-UEBRQFBA-U1FOUVJO-WVlZWQ==";

            // select the HTML element to be converted to PDF
            htmlToPdfConverter.ConvertedHtmlElementSelector = collection["textBoxConvertedHtmlElementSelector"];

            // optionally wait an additional time before starting the conversion
            // it is not necessary to set this property if the HTML page is not updated after initial load
            htmlToPdfConverter.WaitBeforeConvert = 2;

            // convert URL to a PDF memory buffer
            string url = collection["textBoxUrl"];

            byte[] pdfBuffer = htmlToPdfConverter.ConvertUrlToMemory(url);

            FileResult fileResult = new FileContentResult(pdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "ConvertHtmlPart.pdf";

            return fileResult;
        }

        private void SetCrtPageUri()
        {
            HttpRequest httpRequest = this.ControllerContext.HttpContext.Request;
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = httpRequest.Scheme;
            uriBuilder.Host = httpRequest.Host.Host;
            if (httpRequest.Host.Port != null)
                uriBuilder.Port = (int)httpRequest.Host.Port;
            uriBuilder.Path = httpRequest.PathBase.ToString() + httpRequest.Path.ToString();
            uriBuilder.Query = httpRequest.QueryString.ToString();

            ViewData["CrtPageUri"] = uriBuilder.Uri.AbsoluteUri;
        }
    }
}