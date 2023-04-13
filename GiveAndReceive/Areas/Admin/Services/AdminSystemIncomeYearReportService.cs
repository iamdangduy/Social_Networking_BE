using Dapper;
using GiveAndReceive.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace GiveAndReceive.Areas.Admin.Services
{
    public class AdminSystemIncomeYearReportService : BaseService
    {
        public AdminSystemIncomeYearReportService() : base() { }
        public AdminSystemIncomeYearReportService(IDbConnection db) : base(db) { }

        public long GetTotalRevenue()
        {
            string query = "select SUM(Revenue) from [system_income_year_report]";
            return this._connection.Query<long>(query).FirstOrDefault();
        }
    }
}