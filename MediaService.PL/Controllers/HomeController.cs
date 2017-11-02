﻿using System.Web.Mvc;

namespace MediaService.PL.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        public ActionResult Index() => View();

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