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
    [AllowAnonymous]
    public class FriendShipController : ApiBaseController
    {
        [HttpGet]
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

                FriendShip friendShip1 = new FriendShip();
                friendShip1.FriendShipId = Guid.NewGuid().ToString();
                friendShip1.UserId = model.FriendId;
                friendShip1.FriendId = user.UserId;
                friendShip1.Status = FriendShip.EnumStatus.ACCEPTED;

                friendShipService.InsertFriendShip(friendShip);
                friendShipService.InsertFriendShip(friendShip1);
                return Success();
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
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

        [HttpGet]
        public JsonResult RejectFriendShip(string FriendShipId)
        {
            try
            {
                UserService userService = new UserService();
                string token = Request.Headers.Authorization.ToString();
                User user = userService.GetUserByToken(token);
                if (user == null) return Unauthorized();


                FriendShipService friendShipService = new FriendShipService();
                var FriendShip = friendShipService.GetFriendShipById(FriendShipId);
                friendShipService.RejectFriendShip(FriendShip.FriendId, FriendShip.UserId);
                return Success();
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult CheckFriendShipExist(string FriendId)
        {
            try
            {
                UserService userService = new UserService();
                string token = Request.Headers.Authorization.ToString();
                User user = userService.GetUserByToken(token);
                if (user == null) return Unauthorized();

                FriendShipService friendShipService = new FriendShipService();
                return Success(friendShipService.CheckFriendShipExist(user.UserId, FriendId), "");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}
