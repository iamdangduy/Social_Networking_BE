using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GiveAndReceive.Controllers
{
    public class UserQuestController : Controller
    {
        [Route("nguoi-dung/nhiem-vu")]
        public ActionResult Index()
        {
            return View();
        }
    }
}