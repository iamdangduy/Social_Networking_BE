using GiveAndReceive.ApiControllers;
using GiveAndReceive.Areas.Admin.Services;
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
    public class AdminProductController : ApiBaseController
    {
        [HttpGet]
        public JsonResult GetListProduct(int PageIndex = 1, string Keyword = "")
        {
            try
            {
                UserAdmin userAdmin = SecurityProvider.GetUserAdminByToken(Request);
                if (userAdmin == null) return Unauthorized();

                AdminProductService adminProductService = new AdminProductService();
                return Success(adminProductService.GetListProduct(PageIndex, Keyword), "Lấy dữ liệu thành công1");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult InsertProduct(Product model)
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

                        Product product = new Product();
                        product.ProductId = Guid.NewGuid().ToString();
                        product.Name = model.Name;
                        product.Price = model.Price;
                        product.Description = model.Description;

                        AdminProductService adminProductService = new AdminProductService(connect);
                        adminProductService.InsertProduct(product, transaction);

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
        public JsonResult UpdateProduct(Product model)
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

                        Product product = new Product();
                        product.ProductId = model.ProductId;
                        product.Name = model.Name;
                        product.Price = model.Price;
                        product.Description = model.Description;

                        AdminProductService adminProductService = new AdminProductService(connect);
                        adminProductService.UpdateProduct(product, transaction);

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
        public JsonResult DeleteProduct(string ProductId)
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

                        AdminProductService adminProductService = new AdminProductService(connect);
                        var product = adminProductService.GetProductByProductId(ProductId);
                        if (product == null) return Error();

                        adminProductService.DeleteProduct(ProductId, transaction);

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
    }
}
