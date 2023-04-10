using GiveAndReceive.Models;
using GiveAndReceive.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace GiveAndReceive.Filters
{
    public class APIAuthenticationFilterAttribute : ActionFilterAttribute
    {
        private HttpResponseMessage UnauthorizedResponse()
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
            httpResponseMessage.StatusCode = HttpStatusCode.OK;
            httpResponseMessage.Content = new StringContent(new JsonResult() { message = JsonResult.Message.TOKEN_EXPIRED, status = JsonResult.Status.UNAUTHENTICATED }.ToString(), System.Text.Encoding.UTF8, "application/json");
            return httpResponseMessage;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            try
            {
                if (actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Count > 0) { base.OnActionExecuting(actionContext); return; }
                if (actionContext.Request.Headers.Authorization == null) throw new Exception();

                User user = UserProvider.GetUserFromRequestHeader(actionContext.Request);
                if (user == null) throw new Exception();
            }
            catch (Exception ex)
            {
                actionContext.Response = UnauthorizedResponse();
            }

            base.OnActionExecuting(actionContext);
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);
        }
    }
}