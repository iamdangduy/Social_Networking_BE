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
    public class AdminQueueGiveService : BaseService
    {
        public AdminQueueGiveService() : base() { }
        public AdminQueueGiveService(IDbConnection db) : base(db) { }

        public object GetListQueueGive(int PageIndex = 1)
        {
            string queryCount = "select COUNT(*) ";
            string querySelect = "select * ";
            string queryWhere = "from [queue_give] where Status = 'PENDING' or Status = 'IN-DUTY' ";

            
            int TotalRow = this._connection.Query<int>(queryCount + queryWhere).FirstOrDefault();
            int TotalPage = 0;
            if (TotalRow > 0)
            {
                TotalPage = (int)Math.Ceiling((decimal)TotalRow / Constant.PAGE_SIZE);
            }
            int skip = (PageIndex - 1) * Constant.PAGE_SIZE;

            queryWhere += " order by CreateTime desc offset " + skip + " rows fetch next " + Constant.PAGE_SIZE + " rows only";
            List<object> ListData = this._connection.Query<object>(querySelect + queryWhere, new { PageIndex }).ToList();
            return new
            {
                TotalPage,
                ListData,
            };
        }
    }
}