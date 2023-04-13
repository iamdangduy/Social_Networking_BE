using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GiveAndReceive.Areas.Admin.Controllers
{
    public class ManageShareFundMonthController : Controller
    {
        // GET: Admin/ManageShareFundMonth
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ShareFundUserMonth(string id)
        {
            ViewBag.Id = id;
            return View();
        }
    }
}