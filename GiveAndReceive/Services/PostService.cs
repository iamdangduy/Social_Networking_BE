using Dapper;
using GiveAndReceive.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace GiveAndReceive.Services
{
    public class PostService : BaseService
    {
        public PostService() : base() { }
        public PostService(IDbConnection db) : base(db) { }

        public List<object> GetListPost()
        {
            string query = "select p.*, u.Name, u.Avatar from [post] p left join [user] u on u.UserId = p.UserId order by p.CreateTime desc";
            return this._connection.Query<object>(query).ToList();
        }

        public List<object> GetListPostByUserId(string UserId)
        {
            string query = "select p.*, u.Name, u.Avatar from [post] p left join [user] u on u.UserId = p.UserId where p.UserId = @UserId order by p.CreateTime desc";
            return this._connection.Query<object>(query, new { UserId }).ToList();
        }

        public void PlusNumberCommentPost(string PostId)
        {
            string query = "update [post] set Comment = Comment - 1 where PostId = @PostId";
            var status = this._connection.Execute(query, new { PostId });
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public void InsertPost(Post post)
        {
            string query = "insert into [post] (PostId, UserId, Title, Image, Love, Comment, CreateTime) values (@PostId, @UserId, @Title, @Image, @Love, @Comment, @CreateTime)";
            var status = this._connection.Execute(query, post);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public void DeletePost(string PostId, string UserId)
        {
            string query = "delete from [post] where PostId = @PostId and UserId = @UserId";
            var status = this._connection.Execute(query, new { PostId, UserId });
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
    }
}