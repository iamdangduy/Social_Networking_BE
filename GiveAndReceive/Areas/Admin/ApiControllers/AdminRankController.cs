using GiveAndReceive.Areas.Admin.Services;
using GiveAndReceive.Filters;
using GiveAndReceive.Models;
using GiveAndReceive.Providers;
using GiveAndReceive.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GiveAndReceive.Areas.Admin.ApiControllers
{
    public class AdminRankController : ApiAdminBaseController
    {
        [HttpPost]
        [ApiAdminTokenRequire]
        public JsonResult AdminCreateRank(Rank model)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {

                        UserAdmin userAdmin = SecurityProvider.GetUserAdminByToken(Request);
                        if (userAdmin == null) return Unauthorized();
                        AdminRankService adminRankService = new AdminRankService(connect);

                        if (model.Name == null || model.CommissionPercent.Equals(null) || model.MaximumPinLimit.Equals(null)) throw new Exception("Vui lòng nhập đủ thông tin");
                        adminRankService.InsertRank(model, transaction);
                        transaction.Commit();
                        return Success();
                    }
                }
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
        [HttpPost]
        [ApiAdminTokenRequire]
        public JsonResult AdminUpdateRank(Rank model)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        UserAdmin userAdmin = SecurityProvider.GetUserAdminByToken(Request);
                        if (userAdmin == null) return Unauthorized();
                        AdminRankService adminRankService = new AdminRankService(connect);

                        Rank rank = adminRankService.GetRankByRankId(model.RankId, transaction);
                        if (rank == null) throw new Exception("Không tìm thấy rank");
                        if (model.Name == null || model.CommissionPercent.Equals(null) || model.MaximumPinLimit.Equals(null)) throw new Exception("Vui lòng nhập đủ thông tin");
                        adminRankService.UpdateRank(model, transaction);

                        transaction.Commit();
                        return Success();
                    }
                }
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
        [HttpGet]
        [ApiAdminTokenRequire]
        public JsonResult AdminDeleteRank(int rankId)
        {
            try
            {
                using (var connect = BaseService.Connect())
                {
                    connect.Open();
                    using (var transaction = connect.BeginTransaction())
                    {
                        UserAdmin userAdmin = SecurityProvider.GetUserAdminByToken(Request);
                        if (userAdmin == null) return Unauthorized();
                        AdminRankService adminRankService = new AdminRankService(connect);

                        Rank rank = adminRankService.GetRankByRankId(rankId, transaction);
                        if (rank == null) throw new Exception("Không tìm thấy rank");

                        adminRankService.DeleteRank(rank.RankId, transaction);
                        transaction.Commit();
                        return Success();
                    }
                }
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
        [HttpGet]
        [ApiAdminTokenRequire]
        public JsonResult GetListAllRank()
        {
            try
            {
                UserAdmin userAdmin = SecurityProvider.GetUserAdminByToken(Request);
                if (userAdmin == null) return Unauthorized();
                AdminRankService adminRankService = new AdminRankService();

                List<Rank> lsRank = adminRankService.GetListAllRank();
                return Success(lsRank);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
        [HttpGet]
        [ApiAdminTokenRequire]
        public JsonResult GetRankByRankId(int RankId)
        {
            try
            {
                UserAdmin userAdmin = SecurityProvider.GetUserAdminByToken(Request);
                if (userAdmin == null) return Unauthorized();
                AdminRankService adminRankService = new AdminRankService();

                Rank rank = adminRankService.GetRankByRankId(RankId);
                if (rank == null) throw new Exception("Không tìm thấy hạng");
                return Success(rank);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}
