using GiveAndReceive.Filters;
using GiveAndReceive.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GiveAndReceive.ApiControllers
{
    [APIAuthenticationFilter]
    public class ApiBaseController : ApiController
    {
        [HttpGet]
        public JsonResult Success(object data = null, string message = "")
        {
            return new JsonResult { status = JsonResult.Status.SUCCESS, data = data, message = message };
        }


        [HttpGet]
        
        public JsonResult Error(string message = "")
        {
            return new JsonResult { status = JsonResult.Status.ERROR, data = null, message = message };
        }


        [HttpGet]
        
        public JsonResult Unauthorized()
        {
            return new JsonResult { status = JsonResult.Status.UNAUTHORIZED, data = null, message = JsonResult.Message.NO_PERMISSION };
        }

        [HttpGet]
        
        public JsonResult Unauthenticated()
        {
            return new JsonResult { status = JsonResult.Status.UNAUTHENTICATED, data = null, message = JsonResult.Message.TOKEN_EXPIRED };
        }

    }
}
