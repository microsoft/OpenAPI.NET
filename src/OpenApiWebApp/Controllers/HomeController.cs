using OpenApiWebApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tavis.OpenApi;
using Tavis.OpenApi.Model;

namespace OpenApiWebApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(HomeViewModel model)
        {
            if (!String.IsNullOrWhiteSpace(model.Input))
            {
                var openApiParser = new OpenApiParser();
                OpenApiDocument doc = null;
                try
                {
                    var context = OpenApiParser.Parse(model.Input);
                    doc = context.OpenApiDocument;
                    model.Errors = String.Join("\r\n", context.ParseErrors);
                }
                catch (Exception ex)
                {
                    model.Errors = ex.Message;
                    model.Output = string.Empty;
                }
                if (doc != null)
                {
                    var outputstream = new MemoryStream();
                    doc.Save(outputstream);
                    outputstream.Position = 0;

                    model.Output = new StreamReader(outputstream).ReadToEnd();
                }

            }
            return View("Index", model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}