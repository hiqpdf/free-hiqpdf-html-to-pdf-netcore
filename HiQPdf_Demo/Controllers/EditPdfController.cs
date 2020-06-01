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
    public class EditPdfController : Controller
    {
        IHostingEnvironment m_hostingEnvironment;
        public EditPdfController(IHostingEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        // GET: EditPdf
        public ActionResult Index()
        {
            SetCrtPageUri();

            return View();
        }

        [HttpPost]
        public ActionResult CreatePdf(IFormCollection collection)
        {
            // the path PDF document to edit
            string pdfDocumentToEdit = m_hostingEnvironment.ContentRootPath + @"\wwwroot" + @"\DemoFiles\Pdf\WikiPdf.pdf";

            // load the PDF document to edit
            PdfDocument document = PdfDocument.FromFile(pdfDocumentToEdit);

            // set a demo serial number
            document.SerialNumber = "YCgJMTAE-BiwJAhIB-EhlWTlBA-UEBRQFBA-U1FOUVJO-WVlZWQ==";

            #region Add an orange border to each page of the loaded PDF document

            // add an orange border to each PDF page in the loaded PDF document
            foreach (PdfPage pdfPage in document.Pages)
            {
                float crtPdfPageWidth = pdfPage.Size.Width;
                float crtPdfPageHeight = pdfPage.Size.Height;

                // create a PdfRectangle object
                PdfRectangle pdfRectangle = new PdfRectangle(2, 2, crtPdfPageWidth - 4, crtPdfPageHeight - 4);
                pdfRectangle.LineStyle.LineWidth = 2;
                pdfRectangle.ForeColor = System.Drawing.Color.OrangeRed;

                // layout the rectangle in PDF page
                pdfPage.Layout(pdfRectangle);
            }

            #endregion Add an orange border to each page of the loaded PDF document

            #region Layout HTML in a canvas to be repeated on each page of the loaded PDF document

            PdfPage pdfPage1 = document.Pages[0];
            float pdfPageWidth = pdfPage1.Size.Width;
            float pdfPageHeight = pdfPage1.Size.Height;

            // the width of the HTML logo in pixels
            int htmlLogoWidthPx = 400;
            // the width of the HTML logo in points
            float htmlLogoWidthPt = PdfDpiTransform.FromPixelsToPoints(htmlLogoWidthPx);
            float htmlLogoHeightPt = 100;

            // create a canvas to be repeated in the center of each PDF page
            // the canvas is a PDF container that can contain PDF objects ( HTML, text, images, etc )
            PdfRepeatCanvas repeatedCanvas = document.CreateRepeatedCanvas(new System.Drawing.RectangleF((pdfPageWidth - htmlLogoWidthPt) / 2, (pdfPageHeight - htmlLogoHeightPt) / 2,
                        htmlLogoWidthPt, htmlLogoHeightPt));

            // the HTML file giving the content to be placed in the repeated canvas
            string htmlFile = m_hostingEnvironment.ContentRootPath + @"\wwwroot" + @"\DemoFiles\Html\Logo.Html";

            // the HTML object to be laid out in repeated canvas
            PdfHtml htmlLogo = new PdfHtml(0, 0, repeatedCanvas.Width, repeatedCanvas.Height, htmlFile);
            // the browser width when rendering the HTML
            htmlLogo.BrowserWidth = htmlLogoWidthPx;
            // the HTML content will fit the destination hight which is the same with repeated canvas height
            htmlLogo.FitDestHeight = true;

            // layout the HTML object in the repeated canvas
            PdfLayoutInfo htmlLogLayoutInfo = repeatedCanvas.Layout(htmlLogo);

            #endregion Layout HTML in a canvas to be repeated on each page of the loaded PDF document

            try
            {
                // write the PDF document to a memory buffer
                byte[] pdfBuffer = document.WriteToMemory();

                FileResult fileResult = new FileContentResult(pdfBuffer, "application/pdf");
                fileResult.FileDownloadName = "EditPdf.pdf";

                return fileResult;
            }
            finally
            {
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