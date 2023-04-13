using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GiveAndReceive.Controllers
{
    public class UserBankInfoController : Controller
    {
        // GET: UserBankInfo
        [Route("nguoi-dung/tai-khoan-ngan-hang")]
        public ActionResult Index()
        {
            return View();
        }
    }
}