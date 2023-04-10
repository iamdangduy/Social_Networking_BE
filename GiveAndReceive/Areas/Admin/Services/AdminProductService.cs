using Dapper;
using GiveAndReceive.Models;
using GiveAndReceive.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using static GiveAndReceive.Models.JsonResult;

namespace GiveAndReceive.Areas.Admin.Services
{
    public class AdminProductService : BaseService
    {
        public AdminProductService() : base() { }
        public AdminProductService(IDbConnection db) : base(db) { }

        public object GetListProduct(int PageIndex = 1, string Keyword = "")
        {
            string queryCount = "select COUNT(*) ";
            string querySelect = "select * ";
            string queryWhere = "from [product] where 1=1 ";

            if (!string.IsNullOrEmpty(Keyword))
            {
                Keyword = "%" + Keyword.Replace(" ", "%") + "%";
                queryWhere += "and Name like @Keyword ";
            }

            int TotalRow = this._connection.Query<int>(queryCount + queryWhere, new { Keyword }).FirstOrDefault();
            int TotalPage = 0;
            if (TotalRow > 0)
            {
                TotalPage = (int)Math.Ceiling((decimal)TotalRow / Constant.PAGE_SIZE);
            }
            int skip = (PageIndex - 1) * Constant.PAGE_SIZE;
            queryWhere += " order by u.CreateTime desc offset " + skip + " rows fetch next " + Constant.PAGE_SIZE + " rows only";
            List<object> ListData = this._connection.Query<object>(querySelect + queryWhere, new { PageIndex, Keyword }).ToList();

            return new
            {
                TotalPage,
                ListData,
            };
        }

        public Product GetProductByProductId(string ProductId, IDbTransaction transaction = null)
        {
            string query = "select * from [product] where ProductId = @ProductId";
            return this._connection.Query<Product>(query, new { ProductId }, transaction).FirstOrDefault();
        }

        public void InsertProduct(Product model, IDbTransaction transaction = null)
        {
            string query = "insert into [product] ([ProductId], [Name], [Price], [Description]) values (@ProductId, @Name, @Price, @Description)";
            int status = this._connection.Execute(query, model, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public void UpdateProduct(Product model, IDbTransaction transaction = null)
        {
            string query = "update [product] set [Name] = @Name, [Price] = @Price, [Description] = @Description where ProductId = @ProductId";
            int status = this._connection.Execute(query, model, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }

        public void DeleteProduct(string ProductId, IDbTransaction transaction = null)
        {
            string query = "delete from [product] where ProductId = @ProductId";
            int status = this._connection.Execute(query, new { ProductId }, transaction);
            if (status <= 0) throw new Exception(JsonResult.Message.ERROR_SYSTEM);
        }
    }
}