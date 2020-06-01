using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace HiQPdf_Demo.Controllers
{
    public class WelcomeHtmlToPdfController : Controller
    {
        // GET: WelcomeHtmlToPdf
        public ActionResult Index()
        {
            return View();
        }
    }
}