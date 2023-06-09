﻿using Dapper;
using GiveAndReceive.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using GiveAndReceive.Models;

namespace GiveAndReceive.Areas.Admin.Services
{
    public class AdminQueueGiveService : BaseService
    {
        public AdminQueueGiveService() : base() { }
        public AdminQueueGiveService(IDbConnection db) : base(db) { }

        public object GetListQueueGive(int PageIndex = 1)
        {
            string queryCount = "select COUNT(*) ";
            string querySelect = "select qg.*, u.Name ";
            string queryWhere = "from [queue_give] qg left join [user] u on u.UserId = qg.UserId where qg.Status = 'PENDING' or qg.Status = 'IN-DUTY' ";

            
            int TotalRow = this._connection.Query<int>(queryCount + queryWhere).FirstOrDefault();
            int TotalPage = 0;
            if (TotalRow > 0)
            {
                TotalPage = (int)Math.Ceiling((decimal)TotalRow / Constant.NUMBER.PAGE_SIZE);
            }
            int skip = (PageIndex - 1) * Constant.NUMBER.PAGE_SIZE;

            queryWhere += " order by qg.CreateTime desc offset " + skip + " rows fetch next " + Constant.NUMBER.PAGE_SIZE + " rows only";
            List<object> ListData = this._connection.Query<object>(querySelect + queryWhere, new { PageIndex }).ToList();
            return new
            {
                TotalPage,
                ListData,
            };
        }

        public int GetTotalQueueGive()
        {
            string query = "select COUNT(*) from [queue_give] where Status = 'PENDING' or Status = 'IN-DUTY' ";
            return this._connection.Query<int>(query).FirstOrDefault();
        }
    }
}