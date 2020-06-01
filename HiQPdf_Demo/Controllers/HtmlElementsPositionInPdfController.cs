using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

using HiQPdf;

namespace HiQPdf_Demo.Controllers
{
    public class HtmlElementsPositionInPdfController : Controller
    {
        IHostingEnvironment m_hostingEnvironment;
        public HtmlElementsPositionInPdfController(IHostingEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        // GET: HtmlElementsPositionInPdf
        public ActionResult Index()
        {
            SetCrtPageUri();

            return View();
        }

        [HttpPost]
        public ActionResult ConvertToPdf(IFormCollection collection)
        {
            HtmlToPdf htmlToPdfConverter = new HtmlToPdf();

            // set a demo serial number
            htmlToPdfConverter.SerialNumber = "YCgJMTAE-BiwJAhIB-EhlWTlBA-UEBRQFBA-U1FOUVJO-WVlZWQ==";

            htmlToPdfConverter.SelectedHtmlElements = new string[] { collection["textBoxImageSelector"] };
            htmlToPdfConverter.HiddenHtmlElements = new string[] { collection["textBoxImageSelector"] };

            PdfDocument document = null;
            try
            {
                document = htmlToPdfConverter.ConvertUrlToPdfDocument(collection["textBoxUrl"]);

                foreach (HtmlElementInfo htmlImageInfo in htmlToPdfConverter.ConversionInfo.SelectedHtmlElementsInfo)
                {
                    PdfPageRegion imagePdfRegion = htmlImageInfo.PdfRegions[0];
                    PdfPage imagePdfPage = document.Pages[imagePdfRegion.PageIndex];

                    // create the image element
                    PdfImage pdfImage = new PdfImage(imagePdfRegion.Rectangle, m_hostingEnvironment.ContentRootPath + @"\wwwroot" + @"\DemoFiles\Images\HiQPdfLogo_Modified.png");
                    pdfImage.ClipRectangle = imagePdfRegion.Rectangle;

                    imagePdfPage.Layout(pdfImage);
                }

                // write the PDF document to a memory buffer
                byte[] pdfBuffer = document.WriteToMemory();

                FileResult fileResult = new FileContentResult(pdfBuffer, "application/pdf");
                fileResult.FileDownloadName = "ReplaceHtmlElements.pdf";

                return fileResult;

            }
            finally
            {
                if (document != null)
                    document.Close();
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