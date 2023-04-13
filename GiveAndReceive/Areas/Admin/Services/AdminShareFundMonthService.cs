using Dapper;
using GiveAndReceive.Models;
using GiveAndReceive.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using static GiveAndReceive.Models.JsonResult;
using System.Web.Razor.Tokenizer.Symbols;

namespace GiveAndReceive.Areas.Admin.Services
{
    public class AdminShareFundMonthService : BaseService
    {
        public AdminShareFundMonthService() : base() { }
        public AdminShareFundMonthService(IDbConnection db) : base(db) { }

        public ListShareFundMonthView GetListShareFundMonth(int page,IDbTransaction transaction = null)
        {
            ListShareFundMonthView listShareFundMonthView = new ListShareFundMonthView();
            listShareFundMonthView.List = new List<ShareFundMonth>();
            listShareFundMonthView.TotalPage = 0;

            string querySelect = "select *";
            string queryCount = "select count(*)";
            string query = " from [share_fund_month] sfm";


            int totalRow = _connection.Query<int>(queryCount + query, transaction).FirstOrDefault();
            if (totalRow > 0)
            {
                listShareFundMonthView.TotalPage = (int)Math.Ceiling((decimal)totalRow / Constant.ADMIN_PAGE_SIZE);
            }

            int skip = (page - 1) * Constant.ADMIN_PAGE_SIZE;
            query += " order by sfm.Month desc, sfm.Year desc offset " + skip + " rows fetch next " + Constant.ADMIN_PAGE_SIZE + " rows only";
            listShareFundMonthView.List = this._connection.Query<ShareFundMonth>(querySelect + query, transaction).ToList();
            return listShareFundMonthView;
        }

        public long GetCurrentBalance(int Month, int Year)
        {
            string query = "select Balance from [share_fund_month] where Month = @Month and Year = @Year";
            return this._connection.Query<long>(query, new { Month, Year }).FirstOrDefault();
        }
    }
}