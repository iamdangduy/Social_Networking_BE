using GiveAndReceive.Filters;
using GiveAndReceive.Models;
using GiveAndReceive.Providers;
using GiveAndReceive.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Web.Http;
using static GiveAndReceive.Models.JsonResult;

namespace GiveAndReceive.ApiControllers
{
    public class ProductController : ApiBaseController
    {
        [HttpGet]
        public JsonResult GetListProduct()
        {
            try
            {
                ProductService productService = new ProductService();
                return Success(productService.GetListProduct());
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

    }
}