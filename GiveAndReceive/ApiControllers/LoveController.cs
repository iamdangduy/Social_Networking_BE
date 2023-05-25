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
    [AllowAnonymous]
    public class LoveController : ApiBaseController
    {
        [HttpPost]
        public JsonResult InsertLove(Love model)
        {
            try
            {
                UserService userService = new UserService();
                string token = Request.Headers.Authorization.ToString();
                User user = userService.GetUserByToken(token);
                if (user == null) return Unauthorized();

                Love love = new Love();
                love.LoveId = Guid.NewGuid().ToString();
                love.PostId = model.PostId;
                love.UserId = user.UserId;
                love.CreateTime = HelperProvider.GetSeconds();

                LoveService loveService = new LoveService();
                PostService postService = new PostService();
                postService.PlusNumberCommentPost(love.PostId);
                loveService.InsertLove(love);

                return Success();
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}
