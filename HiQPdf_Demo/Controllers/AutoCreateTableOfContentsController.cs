﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using HiQPdf;

namespace HiQPdf_Demo.Controllers
{
    public class AutoCreateTableOfContentsController : Controller
    {
        // GET: AutoCreateTableOfContents
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

            // automatically create a table of contents from H1 to H6 tags
            htmlToPdfConverter.Document.TableOfContents.TocEntriesForHeadingTags = collection["checkBoxCreateTOC"].Count > 0;

            // make the outlines visible in viewer
            htmlToPdfConverter.Document.Viewer.PageMode = PdfPageMode.Outlines;

            // convert the HTML code to PDF
            byte[] pdfBuffer = htmlToPdfConverter.ConvertHtmlToMemory(collection["textBoxHtmlCode"], null);

            FileResult fileResult = new FileContentResult(pdfBuffer, "application/pdf");
            fileResult.FileDownloadName = "AutoTableOfContents.pdf";

            return fileResult;
        }
    }
}