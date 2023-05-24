﻿using Dapper;
using GiveAndReceive.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Services.Description;

namespace GiveAndReceive.Services
{
    public class FriendShipService : BaseService
    {
        public FriendShipService() : base() { }
        public FriendShipService(IDbConnection db) : base(db) { }

        public List<object> GetListFriendShipPending(string FriendId)
        {
            string query = $"select fs.*, u.Name, u.Avatar from [friendship] fs left join [user] u on u.UserId = fs.UserId where fs.FriendId = @FriendId and fs.Status = '{FriendShip.EnumStatus.PENDING}'";
            return this._connection.Query<object>(query, new { FriendId }).ToList();
        }

        public object GetNearlistFriendShipPending(string FriendId)
        {
            string query = $"select TOP(1) fs.*, u.Name, u.Avatar from [friendship] fs left join [user] u on u.UserId = fs.UserId where fs.FriendId = @FriendId and fs.Status = '{FriendShip.EnumStatus.PENDING}' ORDER BY NEWID()";
            return this._connection.Query<object>(query, new { FriendId }).FirstOrDefault();
        }

        public List<object> GetListFriendShipAccepted(string FriendId)
        {
            string query = $"select fs.*, u.Name, u.Avatar from [friendship] fs left join [user] u on u.UserId = fs.UserId where fs.FriendId = @FriendId and fs.Status = '{FriendShip.EnumStatus.ACCEPTED}'";
            return this._connection.Query<object>(query, new { FriendId }).ToList();
        }

        public void InsertFriendShip(FriendShip model)
        {
            string query = "insert into [friendship] (FriendshipId, UserId, FriendId, Status) values (@FriendshipId, @UserId, @FriendId, @Status)";
            var status = this._connection.Execute(query, model);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public void AcceptFriendShip(string FriendShipId)
        {
            string query = $"update [friendship] set Status = '{FriendShip.EnumStatus.ACCEPTED}' where FriendShipId = @FriendShipId";
            var status = this._connection.Execute(query, new { FriendShipId });
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
    }
}