using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

using System.Text;

using HiQPdf;

namespace HiQPdf_Demo.Controllers
{
    public class PdfToHtmlConverterController : Controller
    {
        IHostingEnvironment m_hostingEnvironment;
        public PdfToHtmlConverterController(IHostingEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        // GET: PdfToHtmlConverter
        public ActionResult Index()
        {
            SetCrtPageUri();

            return View();
        }

        [HttpPost]
        public ActionResult ConvertPdfToHtml(IFormCollection collection)
        {
            // get the PDF file
            string pdfFile = m_hostingEnvironment.ContentRootPath + @"\wwwroot" + @"\DemoFiles\Pdf\Presentation.pdf";

            // create the PDF to HTML converter
            PdfToHtml pdfToHtmlConverter = new PdfToHtml();

            // set a demo serial number
            pdfToHtmlConverter.SerialNumber = "YCgJMTAE-BiwJAhIB-EhlWTlBA-UEBRQFBA-U1FOUVJO-WVlZWQ==";

            // set the conversion resolution in DPI
            pdfToHtmlConverter.DPI = int.Parse(collection["textBoxDPI"]);

            // set the zoom level
            pdfToHtmlConverter.Zoom = int.Parse(collection["textBoxZoom"]);

            int fromPdfPageNumber = int.Parse(collection["textBoxFromPage"]);
            int toPdfPageNumber = collection["textBoxToPage"][0].Length > 0 ? int.Parse(collection["textBoxToPage"]) : 0;

            // convert a range of pages of the PDF document to HTML documents in memory
            // the HTML can also be produced to a folder using the ConvertToHtmlFiles method
            // or they can be produced one by one using the RaisePageConvertedToHtmlEvent method
            PdfPageHtmlCode[] htmlDocuments = pdfToHtmlConverter.ConvertToHtml(pdfFile, fromPdfPageNumber, toPdfPageNumber);

            // return if no page was converted
            if (htmlDocuments.Length == 0)
                return Redirect("/PdfToHtmlConverter");

            // get the first page HTML bytes in a buffer
            byte[] htmlDocumentBuffer = null;
            try
            {
                // get the HTML document UTF-8 bytes
                htmlDocumentBuffer = Encoding.UTF8.GetBytes(htmlDocuments[0].HtmlCode);
            }
            finally
            {
                // dispose the HTML documents
                for (int i = 0; i < htmlDocuments.Length; i++)
                    htmlDocuments[i].Dispose();
            }

            FileResult fileResult = new FileContentResult(htmlDocumentBuffer, "text/html; charset=UTF-8");
            fileResult.FileDownloadName = "Page.html";

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