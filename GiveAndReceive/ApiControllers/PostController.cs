using GiveAndReceive.Filters;
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
    public class PostController : ApiBaseController
    {
        [HttpGet]
        public JsonResult GetListPost()
        {
            try
            {
                UserService userService = new UserService();
                string token = Request.Headers.Authorization.ToString();
                User user = userService.GetUserByToken(token);
                if (user == null) return Unauthorized();

                PostService postService = new PostService();
                return Success(postService.GetListPost(user.UserId), "Lấy dữ liệu thành công!");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetPostByPostId(string PostId)
        {
            try
            {
                UserService userService = new UserService();
                string token = Request.Headers.Authorization.ToString();
                User user = userService.GetUserByToken(token);
                if (user == null) return Unauthorized();

                PostService postService = new PostService();
                return Success(postService.GetPostByPostId(PostId), "Lấy dữ liệu thành công!");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetListPostByUserId(string UserId)
        {
            try
            {
                PostService postService = new PostService();
                return Success(postService.GetListPostByUserId(UserId), "Lấy dữ liệu thành công!");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult PlusNumberCommentPost(string PostId)
        {
            try
            {
                PostService postService = new PostService();
                postService.PlusNumberCommentPost(PostId);
                return Success();
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult PlusNumberLovePost(string PostId)
        {
            try
            {
                PostService postService = new PostService();
                postService.PlusNumberLovePost(PostId);
                return Success();
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult InsertPost(Post model)
        {
            try
            {
                UserService userService = new UserService();
                string token = Request.Headers.Authorization.ToString();
                User user = userService.GetUserByToken(token);
                if (user == null) return Unauthorized();
                PostService postService = new PostService();
                Post post = new Post();
                post.PostId = Guid.NewGuid().ToString();
                post.UserId = user.UserId;
                post.Title = model.Title;
                if (!string.IsNullOrEmpty(model.Image))
                {
                    string filename = Guid.NewGuid().ToString() + ".jpg";
                    var path = System.Web.HttpContext.Current.Server.MapPath(Constant.PATH.POST_IMAGE_PATH + filename);
                    HelperProvider.Base64ToImage(model.Image, path);
                    //if (!HelperProvider.DeleteFile(user.Avatar)) return Error();
                    post.Image = Constant.PATH.POST_IMAGE_URL + filename;
                }
                post.Love = 0;
                post.Comment = 0;
                post.CreateTime = HelperProvider.GetSeconds();
                postService.InsertPost(post);

                FriendShipService friendShipService = new FriendShipService();
                NotificationService notificationService = new NotificationService();
                var ListFriend = friendShipService.GetListFriendByUserId(user.UserId);
                foreach(var Friend in ListFriend)
                {
                    Notification notification = new Notification();
                    notification.NotificationId = Guid.NewGuid().ToString();
                    notification.UserId = Friend;
                    notification.Message = $"{user.Name} vừa đăng tải một trạng thái mới!";
                    notification.IsRead = false;
                    notification.CreateTime = HelperProvider.GetSeconds();
                    notification.MessageShort = $"{user.Name} vừa đăng tải một trạng thái mới!";

                    notificationService.CreateNotification(notification);
                }
                return Success();
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult DeletePost(string PostId)
        {
            try
            {
                UserService userService = new UserService();
                string token = Request.Headers.Authorization.ToString();
                User user = userService.GetUserByToken(token);
                if (user == null) return Unauthorized();

                PostService postService = new PostService();
                postService.DeletePost(PostId, user.UserId);
                LoveService loveService = new LoveService();
                loveService.DeleteLoveInPost(PostId);
                CommentService commentService = new CommentService();
                commentService.DeleteComment(PostId);
                return Success(null, "Xoá dữ liệu thành công!");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}
