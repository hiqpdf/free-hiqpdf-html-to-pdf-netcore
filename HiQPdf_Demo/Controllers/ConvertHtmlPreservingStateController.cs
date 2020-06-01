using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.Rendering;

using System.IO;
using HiQPdf;

namespace HiQPdf_Demo.Controllers
{
    public class ConvertHtmlPreservingStateController : Controller
    {
        private ICompositeViewEngine m_viewEngine;

        public ConvertHtmlPreservingStateController(ICompositeViewEngine viewEngine)
        {
            m_viewEngine = viewEngine;
        }

        // GET: ConvertHtmlPreservingState
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ConvertThisViewToPdf(IFormCollection formCollection)
        {
            // add the custom value to view data
            ViewDataDictionary viewData = new ViewDataDictionary(ViewData);
            viewData.Clear();

            viewData.Add("MyTextValue", formCollection["textBoxText"]);
            viewData.Add("MyDropDownListValue", formCollection["dropDownListValues"]);

            // get the HTML code of this view
            string htmlToConvert = RenderViewAsString("Index", viewData);

            // the base URL to resolve relative images and css
            HttpRequest httpRequest = this.ControllerContext.HttpContext.Request;
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = httpRequest.Scheme;
            uriBuilder.Host = httpRequest.Host.Host;
            if (httpRequest.Host.Port != null)
                uriBuilder.Port = (int)httpRequest.Host.Port;
            uriBuilder.Path = httpRequest.PathBase.ToString() + httpRequest.Path.ToString();
            uriBuilder.Query = httpRequest.QueryString.ToString();

            String thisViewUrl = uriBuilder.Uri.AbsoluteUri;
            String baseUrl = thisViewUrl.Substring(0, thisViewUrl.Length - "ConvertHtmlPreservingState/ConvertThisViewToPdf".Length);

            // instantiate the HiQPdf HTML to PDF converter
            HtmlToPdf htmlToPdfConverter = new HtmlToPdf();

            // render the HTML code as PDF in memory
            byte[] pdfBuffer = htmlToPdfConverter.ConvertHtmlToMemory(htmlToConvert, baseUrl);

            FileResult fileResult = new FileContentResult(pdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "ThisViewToPdf.pdf";

            return fileResult;
        }

        public string RenderViewAsString(string viewName, ViewDataDictionary viewData)
        {
            // create a string writer to receive the HTML code
            StringWriter stringWriter = new StringWriter();

            // get the view to render
            ViewEngineResult viewResult = m_viewEngine.FindView(ControllerContext, viewName, true);
            // create a context to render a view based on a model
            ViewContext viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    viewData,
                    TempData,
                    stringWriter,
                    new HtmlHelperOptions()
                    );

            // render the view to a HTML code
            viewResult.View.RenderAsync(viewContext).Wait();

            // return the HTML code
            return stringWriter.ToString();
        }

        [HttpPost]
        public ActionResult ConvertAnotherViewToPdf(IFormCollection formCollection)
        {
            // set a session variable to be used in the the converted view
            ViewDataDictionary viewData = new ViewDataDictionary(ViewData);
            viewData.Clear();

            viewData["MySessionVariable"] = formCollection["textBoxAnotherSessionVariable"];

            // get the About view HTML code
            string htmlToConvert = RenderViewAsString("AnotherView", viewData);

            // the base URL to resolve relative images and css
            HttpRequest httpRequest = this.ControllerContext.HttpContext.Request;
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = httpRequest.Scheme;
            uriBuilder.Host = httpRequest.Host.Host;
            if (httpRequest.Host.Port != null)
                uriBuilder.Port = (int)httpRequest.Host.Port;
            uriBuilder.Path = httpRequest.PathBase.ToString() + httpRequest.Path.ToString();
            uriBuilder.Query = httpRequest.QueryString.ToString();

            String thisViewUrl = uriBuilder.Uri.AbsoluteUri;
            String baseUrl = thisViewUrl.Substring(0, thisViewUrl.Length - "ConvertHtmlPreservingState/ConvertAnotherViewToPdf".Length);

            // instantiate the HiQPdf HTML to PDF converter
            HtmlToPdf htmlToPdfConverter = new HtmlToPdf();

            // render the HTML code as PDF in memory
            byte[] pdfBuffer = htmlToPdfConverter.ConvertHtmlToMemory(htmlToConvert, baseUrl);

            // send the PDF document to browser
            FileResult fileResult = new FileContentResult(pdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "AnotherViewToPdf.pdf";

            return fileResult;
        }
    }
}