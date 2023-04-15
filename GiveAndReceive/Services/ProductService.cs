using Dapper;
using GiveAndReceive.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace GiveAndReceive.Services
{
    public class ProductService : BaseService
    {
        public ProductService() : base() { }
        public ProductService(IDbConnection db) : base(db) { }

        public List<Product> GetListProduct(IDbTransaction transaction = null)
        {
            string query = "select * from [product]";
            return this._connection.Query<Product>(query, transaction).ToList();
        }
    }
}