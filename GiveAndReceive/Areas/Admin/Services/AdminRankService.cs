using Dapper;
using GiveAndReceive.Models;
using GiveAndReceive.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace GiveAndReceive.Areas.Admin.Services
{
    public class AdminRankService : BaseService
    {
        public AdminRankService() : base() { }
        public AdminRankService(IDbConnection db) : base(db) { }

        public void InsertRank(Rank model, IDbTransaction transaction = null)
        {
            string query = "INSERT INTO [dbo].[rank]([Name],[CommissionPercent],[MaximumPinLimit]) VALUES (@Name,@CommissionPercent,@MaximumPinLimit)";
            int status = this._connection.Execute(query, model, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
        public void UpdateRank(Rank model, IDbTransaction transaction = null)
        {
            string query = "UPDATE [dbo].[rank] SET [Name] = @Name,[CommissionPercent] = @CommissionPercent,[MaximumPinLimit] = @MaximumPinLimit WHERE RankId = @RankId";
            int status = this._connection.Execute(query, model, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public void DeleteRank(int RankId, IDbTransaction transaction = null)
        {
            string query = "Delete [rank] where RankId = @RankId";
            int status = this._connection.Execute(query, new { RankId }, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public Rank GetRankByRankId(int RankId, IDbTransaction transaction = null)
        {
            string query = "select TOP(1)* from [rank] where RankId = @RankId";
            return this._connection.Query<Rank>(query, new { RankId }, transaction).FirstOrDefault();
        }
        public List<Rank> GetListAllRank(IDbTransaction transaction = null)
        {
            string query = "select* from [rank] order by RankId desc";
            return this._connection.Query<Rank>(query, null, transaction).ToList();
        }
    }
}