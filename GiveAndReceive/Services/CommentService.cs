using Dapper;
using GiveAndReceive.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace GiveAndReceive.Services
{
    public class CommentService : BaseService
    {
        public CommentService() : base() { }
        public CommentService(IDbConnection db) : base(db) { }

        public List<object> GetListCommentByPostId(string PostId)
        {
            string query = "select c.*, u.Name, u.Avatar from [comment] c left join [user] u on c.UserId = u.UserId where c.PostId = @PostId";
            return this._connection.Query<object>(query, new { PostId }).ToList();
        }

        public void InsertComment(Comment model)
        {
            string query = "insert into [comment] (CommentId, CommentContent, UserId, PostId, CreateTime) values (@CommentId, @CommentContent, @UserId, @PostId, @CreateTime)";
            int status = this._connection.Execute(query, model);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
    }
}