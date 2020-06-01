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
    public class ExtractTextFromPdfController : Controller
    {
        IFormCollection m_formCollection;
        IHostingEnvironment m_hostingEnvironment;
        public ExtractTextFromPdfController(IHostingEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        // GET: ExtractTextFromPdf
        public ActionResult Index()
        {
            SetCrtPageUri();

            return View();
        }

        [HttpPost]
        public ActionResult ExtractText(IFormCollection collection)
        {
            m_formCollection = collection;

            // get the PDF file
            string pdfFile = m_hostingEnvironment.ContentRootPath + @"\wwwroot" + @"\DemoFiles\Pdf\InputPdf.pdf";

            // create the PDF text extractor
            PdfTextExtract pdfTextExtract = new PdfTextExtract();

            // set a demo serial number
            pdfTextExtract.SerialNumber = "YCgJMTAE-BiwJAhIB-EhlWTlBA-UEBRQFBA-U1FOUVJO-WVlZWQ==";

            // set the text extraction mode
            pdfTextExtract.TextExtractMode = GetTextExtractMode();

            int fromPdfPageNumber = int.Parse(collection["textBoxFromPage"]);
            int toPdfPageNumber = collection["textBoxToPage"][0].Length > 0 ? int.Parse(collection["textBoxToPage"]) : 0;

            // extract the text from a range of pages of the PDF document
            string text = pdfTextExtract.ExtractText(pdfFile, fromPdfPageNumber, toPdfPageNumber);

            // get UTF-8 bytes
            byte[] utf8Bytes = Encoding.UTF8.GetBytes(text);

            // the UTF-8 marker
            byte[] utf8Marker = new byte[] { 0xEF, 0xBB, 0xBF };

            // the text document bytes with UTF-8 marker followed by UTF-8 bytes
            byte[] bytes = new byte[utf8Bytes.Length + utf8Marker.Length];
            Array.Copy(utf8Marker, 0, bytes, 0, utf8Marker.Length);
            Array.Copy(utf8Bytes, 0, bytes, utf8Marker.Length, utf8Bytes.Length);

            FileResult fileResult = new FileContentResult(bytes, "text/plain; charset=UTF-8");
            fileResult.FileDownloadName = "ExtractedText.txt";

            return fileResult;
        }

        private PdfTextExtractMode GetTextExtractMode()
        {
            switch (m_formCollection["dropDownListExtractMode"])
            {
                case "Keep Positioning":
                    return PdfTextExtractMode.KeepPositioning;
                case "Keep Reading Order":
                    return PdfTextExtractMode.KeepReadingOrder;
                default:
                    return PdfTextExtractMode.KeepPositioning;
            }
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