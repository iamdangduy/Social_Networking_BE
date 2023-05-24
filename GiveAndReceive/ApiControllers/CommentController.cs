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
    public class CommentController : ApiBaseController
    {
        [HttpPost]
        public JsonResult InsertComment(Comment model)
        {
            try
            {
                UserService userService = new UserService();
                string token = Request.Headers.Authorization.ToString();
                User user = userService.GetUserByToken(token);
                if (user == null) return Unauthorized();

                Comment comment = new Comment();
                comment.CommentId = Guid.NewGuid().ToString();
                comment.CommentContent = model.CommentContent;
                comment.UserId = user.UserId;
                comment.PostId = model.PostId;
                comment.CreateTime = HelperProvider.GetSeconds();
                CommentService commentService = new CommentService();
                commentService.InsertComment(comment);

                PostService postService = new PostService();
                postService.PlusNumberCommentPost(model.PostId);

                return Success();
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}
