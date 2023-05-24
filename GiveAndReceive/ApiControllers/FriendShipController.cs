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
    public class FriendShipController : ApiBaseController
    {
        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetListFriendShipPending()
        {
            try
            {
                UserService userService = new UserService();
                string token = Request.Headers.Authorization.ToString();
                User user = userService.GetUserByToken(token);
                if (user == null) return Unauthorized();

                FriendShipService friendShipService = new FriendShipService();
                return Success(friendShipService.GetListFriendShipPending(user.UserId), "Lấy dữ liệu thành công");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetListFriendShipAccepted()
        {
            try
            {
                UserService userService = new UserService();
                string token = Request.Headers.Authorization.ToString();
                User user = userService.GetUserByToken(token);
                if (user == null) return Unauthorized();

                FriendShipService friendShipService = new FriendShipService();
                return Success(friendShipService.GetListFriendShipAccepted(user.UserId), "Lấy dữ liệu thành công");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetNearlistFriendShipPending()
        {
            try
            {
                UserService userService = new UserService();
                string token = Request.Headers.Authorization.ToString();
                User user = userService.GetUserByToken(token);
                if (user == null) return Unauthorized();

                FriendShipService friendShipService = new FriendShipService();
                return Success(friendShipService.GetNearlistFriendShipPending(user.UserId), "Lấy dữ liệu thành công");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult InsertFriendShip(FriendShip model)
        {
            try
            {
                UserService userService = new UserService();
                string token = Request.Headers.Authorization.ToString();
                User user = userService.GetUserByToken(token);
                if (user == null) return Unauthorized();

                FriendShipService friendShipService = new FriendShipService();
                FriendShip friendShip = new FriendShip();
                friendShip.FriendShipId = Guid.NewGuid().ToString();
                friendShip.UserId = user.UserId;
                friendShip.FriendId = model.FriendId;
                friendShip.Status = FriendShip.EnumStatus.PENDING;

                friendShipService.InsertFriendShip(friendShip);
                return Success();
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult AcceptFriendShip(string FriendShipId)
        {
            try
            {
                FriendShipService friendShipService = new FriendShipService();
                friendShipService.AcceptFriendShip(FriendShipId);
                return Success();
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}
