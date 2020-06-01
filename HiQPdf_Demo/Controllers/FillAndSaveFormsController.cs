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
    public class FillAndSaveFormsController : Controller
    {
        IHostingEnvironment m_hostingEnvironment;
        public FillAndSaveFormsController(IHostingEnvironment hostingEnvironment)
        {
            m_hostingEnvironment = hostingEnvironment;
        }

        // GET: FillAndSaveForms
        public ActionResult Index()
        {
            SetCrtPageUri();

            return View();
        }

        [HttpPost]
        public ActionResult CreatePdf(IFormCollection collection)
        {
            string pdfFileWithForm = m_hostingEnvironment.ContentRootPath + @"\wwwroot" + @"\DemoFiles\Pdf\PdfWithForm.pdf";

            // load the PDF document with form from file
            PdfDocument document = PdfDocument.FromFile(pdfFileWithForm);

            // set a demo serial number
            document.SerialNumber = "YCgJMTAE-BiwJAhIB-EhlWTlBA-UEBRQFBA-U1FOUVJO-WVlZWQ==";

            // get the Check Box field by name from form fields collection and set its value
            PdfFormField checkBoxField = document.Form.Fields["cb"];
            if (checkBoxField != null)
                checkBoxField.Value = collection["checkBoxChecked"].Count > 0;

            // get the Text Box field by name from form fields collection and set its value
            PdfFormField textBoxField = document.Form.Fields["tb"];
            if (textBoxField != null)
                textBoxField.Value = collection["textBoxText"][0];

            // get the List Box field by name from form fields collection and set its value
            PdfFormField listBoxField = document.Form.Fields["lb"];
            if (listBoxField != null)
                listBoxField.Value = collection["dropDownListListBoxValue"][0];

            // get the Combo Box field by name from form fields collection and set its value
            PdfFormField comboBoxField = document.Form.Fields["combo"];
            if (comboBoxField != null)
                comboBoxField.Value = collection["dropDownListComboBoxValue"][0];

            // get the Radio Buttons Group field by name from form fields collection and set its value
            PdfFormField radioGroupField = document.Form.Fields["rg"];
            if (radioGroupField != null)
                radioGroupField.Value = collection["SelectedRadionButton"] == "radioButton1" ? "rb1" : "rb2";

            try
            {
                // write the PDF document to a memory buffer
                byte[] pdfBuffer = document.WriteToMemory();

                FileResult fileResult = new FileContentResult(pdfBuffer, "application/pdf");
                fileResult.FileDownloadName = "FillForms.pdf";

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