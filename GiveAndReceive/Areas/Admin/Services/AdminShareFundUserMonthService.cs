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
using System.Web.UI;

namespace GiveAndReceive.Areas.Admin.Services
{
    public class AdminShareFundUserMonthService : BaseService
    {
        public AdminShareFundUserMonthService() : base() { }
        public AdminShareFundUserMonthService(IDbConnection db) : base(db) { }

        public ListShareFundUserMonthView GetListShareFundUserMonthByMonthYear(int month, int year, string keyword, int page, IDbTransaction transaction = null)
        {
            ListShareFundUserMonthView listShareFundUserMonthView = new ListShareFundUserMonthView();
            listShareFundUserMonthView.List = new List<ShareFundUserMonth>();
            listShareFundUserMonthView.TotalPage = 0;

            string querySelect = "select u.Name, u.Phone, u.Email, sfum.Amount, sfum.Month, sfum.Year, sfum.Status";
            string queryCount = "select count(*)";
            string query = " from [share_fund_user_month] sfum left join [user] u on sfum.UserId = u.UserId where 1=1";

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = "%" + keyword.Replace(' ', '%') + "%";
                query += " and (u.Phone like @keyword or u.Email like @keyword)";
            }

            int totalRow = _connection.Query<int>(queryCount + query, new { keyword = keyword }, transaction).FirstOrDefault();
            if (totalRow > 0)
            {
                listShareFundUserMonthView.TotalPage = (int)Math.Ceiling((decimal)totalRow / Constant.ADMIN_PAGE_SIZE);
            }

            int skip = (page - 1) * Constant.ADMIN_PAGE_SIZE;
            query += " order by sfum.Month desc, sfum.Year desc offset " + skip + " rows fetch next " + Constant.ADMIN_PAGE_SIZE + " rows only";
            listShareFundUserMonthView.List = this._connection.Query<ShareFundUserMonth>(querySelect + query, new { keyword = keyword }, transaction).ToList();
            return listShareFundUserMonthView;
        }
    }
}