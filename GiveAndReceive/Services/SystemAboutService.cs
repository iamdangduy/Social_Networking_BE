using Dapper;
using GiveAndReceive.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace GiveAndReceive.Services
{
    public class SystemAboutService : BaseService
    {
        public SystemAboutService() : base() { }
        public SystemAboutService(IDbConnection db) : base(db) { }

        public SystemAbout GetSystemAbout(IDbTransaction transaction = null)
        {
            string query = "select * from system_about";
            return this._connection.Query<SystemAbout>(query, null, transaction).FirstOrDefault();
        }
        public bool UpdateSystemAbout(SystemAbout model, IDbTransaction transaction = null)
        {
            string query = "UPDATE [dbo].[system_about] SET [HtmlContent]=@HtmlContent WHERE SystemAboutId=@SystemAboutId";
            int status = this._connection.Execute(query, model, transaction);
            return status > 0;
        }
    }
}