using GiveAndReceive.ApiControllers;
using GiveAndReceive.Areas.Admin.Services;
using GiveAndReceive.Filters;
using GiveAndReceive.Models;
using GiveAndReceive.Providers;
using GiveAndReceive.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using static GiveAndReceive.Models.JsonResult;

namespace GiveAndReceive.Areas.Admin.ApiControllers
{
    public class AdminProductController : ApiBaseController
    {
        [HttpGet]
        [ApiAdminTokenRequire]
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

        [HttpGet]
        [ApiAdminTokenRequire]
        public JsonResult GetProductByProductId(string ProductId)
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
                        return Success(adminProductService.GetProductByProductId(ProductId, transaction), "Lấy dữ liệu thành công!");
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

                        if (!Directory.Exists(HttpContext.Current.Server.MapPath("~" + String.Format(Constant.PRODUCT_THUMBNAIL_PATH))))
                        {
                            Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~" + String.Format(Constant.PRODUCT_THUMBNAIL_PATH)));
                        }

                        if (!string.IsNullOrEmpty(model.Thumbnail))
                        {
                            string filename = Guid.NewGuid().ToString() + ".jpg";
                            var path = System.Web.HttpContext.Current.Server.MapPath(Constant.PRODUCT_THUMBNAIL_PATH + filename);
                            HelperProvider.Base64ToImage(model.Thumbnail, path);
                            product.Thumbnail = Constant.PRODUCT_THUMBNAIL_URL + filename;
                        }

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
        [ApiAdminTokenRequire]
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
                        if (!string.IsNullOrEmpty(model.Thumbnail))
                        {
                            //xóa file cũ
                            if (!HelperProvider.DeleteFile(product.Thumbnail)) return Error(JsonResult.Message.ERROR_SYSTEM);
                            //tạo file mới
                            string filename = Guid.NewGuid().ToString() + ".jpg";
                            var path = System.Web.HttpContext.Current.Server.MapPath(Constant.PRODUCT_THUMBNAIL_PATH + filename);
                            HelperProvider.Base64ToImage(model.Thumbnail, path);
                            product.Thumbnail = Constant.PRODUCT_THUMBNAIL_URL + filename;
                        }
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
        [ApiAdminTokenRequire]
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
                        var product = adminProductService.GetProductByProductId(ProductId,transaction);
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
