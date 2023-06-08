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

        public List<object> GetListPost(string UserId)
        {
            string query = "select p.PostId, p.Title, p.Image, p.Love, p.Comment, p.CreateTime, u.Name, u.Avatar, COUNT( CASE when l.UserId = @UserId then 1 end) as Loved " +
                "from [post] p left join [user] u on u.UserId = p.UserId left join [love] l on l.PostId = p.PostId " +
                "group by p.PostId, p.Title, p.Image, p.Love, p.Comment, p.CreateTime, u.Name, u.Avatar order by CreateTime desc";
            return this._connection.Query<object>(query, new { UserId }).ToList();
        }

        public object GetPostByPostId(string PostId)
        {
            string queryPost = "select p.*, u.Name, u.Avatar from [post] p left join [user] u on u.UserId = p.UserId where p.PostId = @PostId";
            string queryComment = "select c.CommentId, c.CommentContent, c.CreateTime, u.Name, u.Avatar from [comment] c left join [user] u on u.UserId = c.UserId where c.PostId = @PostId order by c.CreateTime desc";
            var Post = this._connection.Query<object>(queryPost, new { PostId }).FirstOrDefault();
            var Comments = this._connection.Query<object>(queryComment, new { PostId }).ToList();

            return new
            {
                Post,
                Comments,
            };
        }

        public Post GetPostById(string PostId)
        {
            string query = "select * from Post where PostId = @PostId";
            return this._connection.Query<Post>(query, new { PostId }).FirstOrDefault();
        }

        public List<object> GetListPostByUserId(string UserId, string FriendId)
        {
            string query = "select p.PostId, p.Title, p.Image, p.Love, p.Comment, p.CreateTime, u.Name, u.Avatar, COUNT( CASE when l.UserId = @FriendId then 1 end) as Loved " +
                "from [post] p left join [user] u on u.UserId = p.UserId left join [love] l on l.PostId = p.PostId where p.UserId = @UserId " +
                "group by p.PostId, p.Title, p.Image, p.Love, p.Comment, p.CreateTime, u.Name, u.Avatar order by p.CreateTime desc";
            return this._connection.Query<object>(query, new { UserId, FriendId }).ToList();
        }

        public void PlusNumberCommentPost(string PostId)
        {
            string query = "update [post] set Comment = Comment + 1 where PostId = @PostId";
            var status = this._connection.Execute(query, new { PostId });
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public void MinusNumberCommentPost(string PostId)
        {
            string query = "update [post] set Comment = Comment - 1 where PostId = @PostId";
            var status = this._connection.Execute(query, new { PostId });
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public void PlusNumberLovePost(string PostId)
        {
            string query = "update [post] set Love = Love + 1 where PostId = @PostId";
            var status = this._connection.Execute(query, new { PostId });
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public void MinusNumberLovePost(string PostId)
        {
            string query = "update [post] set Love = Love - 1 where PostId = @PostId";
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