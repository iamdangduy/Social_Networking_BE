using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GiveAndReceive.Controllers
{
    public class ConnectMemberController : Controller
    {
        // GET: ConnectMember
        [Route("ketnoi")]
        public ActionResult Index()
        {
            return View();
        }
    }
}