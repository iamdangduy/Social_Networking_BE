using Dapper;
using GiveAndReceive.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace GiveAndReceive.Services
{
    public class LoveService : BaseService
    {
        public LoveService() : base() { }
        public LoveService(IDbConnection db) : base(db) { }

        public void InsertLove(Love model)
        {
            string query = "insert into [love] (LoveId, PostId, UserId, CreateTime) values (@LoveId, @PostId, @UserId, @CreateTime)";
            int status = this._connection.Execute(query, model);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public void DeleteLoveInPost(string PostId)
        {
            string query = "delete from [love] where PostId = @PostId";
            int status = this._connection.Execute(query, new { PostId });
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public Love GetExistLove(string PostId, string UserId)
        {
            string query = "select * from love where PostId = @PostId and UserId = @UserId";
            return this._connection.Query<Love>(query, new { PostId, UserId }).FirstOrDefault();
        }
    }
}