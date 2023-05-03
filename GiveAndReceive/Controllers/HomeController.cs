using GiveAndReceive.Models;
using GiveAndReceive.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GiveAndReceive.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        [Route("dang-nhap")]
        public ActionResult Login() {
            return View();
        }

        [Route("dang-ky/{shareCode?}")]
        public ActionResult Register(string shareCode)
        {
            ViewBag.ShareCode = shareCode;
            return View();
        }
        [Route("quen-mat-khau")]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [Route("gioi-thieu")]
        public ActionResult AboutUs()
        {
            return View();
        }

    }
}