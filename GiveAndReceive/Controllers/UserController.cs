using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GiveAndReceive.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        [Route("nguoi-dung/thong-tin")]
        public ActionResult UserInfo() {
            return View();
        }

        [Route("nguoi-dung/vi")]
        public ActionResult UserWalletInfo() {
            return View();
        }

        [Route("nguoi-dung/lich-su-giao-dich")]
        public ActionResult UserTransaction() {
            return View();
        }
        [Route("nguoi-dung/xac-thuc-tai-khoan")]
        public ActionResult AccountVerification()
        {
            return View();
        }
    }
}