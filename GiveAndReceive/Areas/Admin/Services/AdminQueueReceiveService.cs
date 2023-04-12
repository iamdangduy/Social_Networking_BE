using Dapper;
using GiveAndReceive.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Razor.Tokenizer.Symbols;
using static GiveAndReceive.Models.JsonResult;

namespace GiveAndReceive.Areas.Admin.Services
{
    public class AdminQueueReceiveService : BaseService
    {
        public AdminQueueReceiveService() : base() { }
        public AdminQueueReceiveService(IDbConnection db) : base(db) { }

        public object GetListQueueReceive(int PageIndex = 1, string Status = "")
        {
            string queryCount = "select COUNT(*) ";
            string querySelect = "select * ";
            string queryWhere = "from [queue_receive] where 1=1 ";

            if (!string.IsNullOrEmpty(Status))
            {
                Status = "%" + Status.Replace(" ", "%") + "%";
                queryWhere += "and Status like @Status ";
            }
            int TotalRow = this._connection.Query<int>(queryCount + queryWhere, new { Status }).FirstOrDefault();
            int TotalPage = 0;
            if (TotalRow > 0)
            {
                TotalPage = (int)Math.Ceiling((decimal)TotalRow / Constant.PAGE_SIZE);
            }
            int skip = (PageIndex - 1) * Constant.PAGE_SIZE;

            queryWhere += " order by CreateTime desc offset " + skip + " rows fetch next " + Constant.PAGE_SIZE + " rows only";
            List<object> ListData = this._connection.Query<object>(querySelect + queryWhere, new { PageIndex, Status }).ToList();
            return new
            {
                TotalPage,
                ListData,
            };
        }
    }
}