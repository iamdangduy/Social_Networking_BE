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

        [Route("dang-ky")]
        public ActionResult Register()
        {
            return View();
        }
        [Route("quen-mat-khau")]
        public ActionResult ForgotPassword()
        {
            return View();
        }

    }
}