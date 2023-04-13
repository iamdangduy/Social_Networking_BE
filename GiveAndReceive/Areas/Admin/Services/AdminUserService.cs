using Dapper;
using GiveAndReceive.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using static GiveAndReceive.Models.JsonResult;

namespace GiveAndReceive.Areas.Admin.Services
{
    public class AdminUserService : BaseService
    {
        public AdminUserService() : base() { }
        public AdminUserService(IDbConnection db) : base(db) { }

        public object GetListUser(int PageIndex = 1, string Keyword = "")
        {
            string queryCount = "select COUNT(*) ";
            string querySelect = "select UserId, Name, Avatar, Email, Account ";
            string queryWhere = "from [user] where 1=1";
            if (!string.IsNullOrEmpty(Keyword))
            {
                Keyword = "%" + Keyword.Replace(" ", "%") + "%";
                queryWhere += "and Name like @Keyword ";
            }
            int TotalRow = this._connection.Query<int>(queryCount + queryWhere, new { Keyword }).FirstOrDefault();
            int TotalPage = 0;
            if (TotalRow > 0)
            {
                TotalPage = (int)Math.Ceiling((decimal)TotalRow / Constant.PAGE_SIZE);
            }
            int skip = (PageIndex - 1) * Constant.PAGE_SIZE;
            queryWhere += " order by CreateTime desc offset " + skip + " rows fetch next " + Constant.PAGE_SIZE + " rows only";
            List<object> ListData = this._connection.Query<object>(querySelect + queryWhere, new { PageIndex, Keyword }).ToList();
            return new
            {
                TotalPage,
                ListData,
            };
        }

        public object GetUserByUserId(string UserId, IDbTransaction transaction = null)
        {
            string query = "select [Name], [Avatar], [Account], [Phone], [Email], [CreateTime], [ShareCode], [ParentCode] from [user] where UserId = @UserId";
            return this._connection.Query<object>(query, new { UserId }, transaction).FirstOrDefault();
        }

        public int GetTotalUser()
        {
            string query = "select COUNT(UserId) from [user]";
            return this._connection.Query<int>(query).FirstOrDefault();
        }
    }
}