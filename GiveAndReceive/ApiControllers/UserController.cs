using GiveAndReceive.Models;
using GiveAndReceive.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GiveAndReceive.ApiControllers
{
    public class UserController : ApiBaseController
    {
        [HttpPost]
        public JsonResult UpdateInfoUser(User model)
        {
            try
            {
                using(var connect = BaseService.Connect())
                {
                    connect.Open();
                    using(var transaction = connect.BeginTransaction())
                    {
                        
                        UserService userService = new UserService(connect);
                        return Success();
                    }
                }
            }
            catch(Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}
