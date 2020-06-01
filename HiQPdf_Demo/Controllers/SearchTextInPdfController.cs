using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

using System.Drawing;

using HiQPdf;

namespace HiQPdf_Demo.Controllers
{
    public class SearchTextInPdfController : Controller
    {
        IHostingEnvironment m_hostingEnvironment;
        public SearchTextInPdfController(IHostingEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        // GET: SearchTextInPdf
        public ActionResult Index()
        {
            SetCrtPageUri();

            return View();
        }

        [HttpPost]
        public ActionResult SearchText(IFormCollection collection)
        {
            // get the PDF file
            string pdfFile = m_hostingEnvironment.ContentRootPath + @"\wwwroot" + @"\DemoFiles\Pdf\InputPdf.pdf";

            // get the text to search
            string textToSearch = collection["textBoxTextToSearch"];

            // create the PDF text extractor
            PdfTextExtract pdfTextExtract = new PdfTextExtract();

            // set a demo serial number
            pdfTextExtract.SerialNumber = "YCgJMTAE-BiwJAhIB-EhlWTlBA-UEBRQFBA-U1FOUVJO-WVlZWQ==";

            int fromPdfPageNumber = int.Parse(collection["textBoxFromPage"]);
            int toPdfPageNumber = collection["textBoxToPage"][0].Length > 0 ? int.Parse(collection["textBoxToPage"]) : 0;

            // search the text in PDF document
            PdfTextSearchItem[] searchTextInstances = pdfTextExtract.SearchText(pdfFile, textToSearch,
                        fromPdfPageNumber, toPdfPageNumber, collection["checkBoxMatchCase"].Count > 0, collection["checkBoxMatchWholeWord"].Count > 0);

            // load the PDF file to highlight the searched text
            PdfDocument pdfDocument = PdfDocument.FromFile(pdfFile);

            // set a demo serial number
            pdfDocument.SerialNumber = "YCgJMTAE-BiwJAhIB-EhlWTlBA-UEBRQFBA-U1FOUVJO-WVlZWQ==";

            // highlight the searched text in PDF document
            foreach (PdfTextSearchItem searchTextInstance in searchTextInstances)
            {
                PdfRectangle pdfRectangle = new PdfRectangle(searchTextInstance.BoundingRectangle);

                // set rectangle color and opacity
                pdfRectangle.BackColor = Color.Yellow;
                pdfRectangle.Opacity = 30;

                // highlight the text
                pdfDocument.Pages[searchTextInstance.PdfPageNumber - 1].Layout(pdfRectangle);
            }

            // write the modified PDF document
            try
            {
                // write the PDF document to a memory buffer
                byte[] pdfBuffer = pdfDocument.WriteToMemory();

                FileResult fileResult = new FileContentResult(pdfBuffer, "application/pdf");
                fileResult.FileDownloadName = "SearchText.pdf";

                return fileResult;
            }
            finally
            {
                pdfDocument.Close();
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