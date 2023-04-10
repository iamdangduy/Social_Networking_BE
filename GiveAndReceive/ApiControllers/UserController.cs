using GiveAndReceive.Models;
using GiveAndReceive.Providers;
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
                        string token = Request.Headers.Authorization.ToString();
                        UserService userService = new UserService(connect);
                        User user = userService.GetUserByToken(token, transaction);
                        if (user == null) return Unauthorized();

                        user.Name = model.Name.Trim();
                        if(!string.IsNullOrEmpty(model.Avatar))
                        {
                            string filename = Guid.NewGuid().ToString() + ".jpg";
                            /*var path = System.Web.HttpContext.Current.Server.MapPath(Constant.AVATAR_USER_PATH + filename);
                            HelperProvider.Base64ToImage(model.Avatar, path);
                            if (!HelperProvider.DeleteFile(user.Avatar)) return Error();
                            user.Avatar = Constant.AVATAR_USER_URL + filename;*/
                        }

                        if(!string.IsNullOrEmpty(model.Account))
                        {
                            user.Account = model.Account.Trim();
                            userService.CheckAccountExist(user.Account, transaction);
                        }

                        if(!string.IsNullOrEmpty (model.Email))
                        {
                            user.Email = model.Email.Trim();
                            userService.CheckEmailExist(user.Email, transaction);
                        }

                        user.Phone = model.Phone.Trim();

                        userService.UpdateUser(user,transaction);

                        transaction.Commit();
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
