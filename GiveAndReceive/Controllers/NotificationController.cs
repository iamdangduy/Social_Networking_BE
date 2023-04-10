using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GiveAndReceive.Controllers
{
    public class NotificationController : Controller
    {
        [Route("thong-bao")]
        public ActionResult Index()
        {
            return View();
        }
    }
}