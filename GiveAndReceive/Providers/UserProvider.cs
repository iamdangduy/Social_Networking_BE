using GiveAndReceive.Models;
using Dapper;
using System.Data;
using GiveAndReceive.Services;
using System;
using System.Net.Http;
using System.Linq;

namespace GiveAndReceive.Providers
{
    public class UserProvider
    {

        public static User GetUserFromRequestHeader(HttpRequestMessage request, IDbConnection connect = null, IDbTransaction transaction = null)
        {
            if (request == null) return null;
            string token = request.Headers.GetValues("Authorization").FirstOrDefault();
            if (string.IsNullOrEmpty(token)) return null;

            UserService userService = new UserService(connect);
            User user = userService.GetUserByToken(token, transaction);

            return user;
        }

        

        public static void TransferMoney(string userGiveId, string userReceiveId, decimal moneyTransfer, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            UserService userService = new UserService(connection);
            User userGive = userService.GetUserById(userGiveId, transaction);
            if (userGive == null) throw new Exception("Không tìm thấy thông tin người cho");
            User userReceive = userService.GetUserById(userReceiveId, transaction);
            if (userReceive == null) throw new Exception("Không tìm thấy thông tin người nhận");

            userService.UpdateUserPoint(userGive.UserId,-moneyTransfer,transaction);
            userService.UpdateUserPoint(userReceive.UserId, moneyTransfer, transaction);

            //systemupdate

        }


        /*public static void UpdateUserPoint(string userId, long point, string note = "", IDbConnection connection = null, IDbTransaction transaction = null)
        {
            UserService userService = new UserService(connection);
            UserWallet userWallet = userService.GetUserWallet(userId, transaction);
            if (userWallet == null) throw new Exception(JsonResult.Message.ERROR_SYSTEM);

            if (userWallet.Balance + point < 0) throw new Exception("Số dư tài khoản không đủ để thực hiện thao tác này");
            userService.UpdateUserPoint(userId, point, transaction);

            userService.InsertUserTransaction(new UserTransaction()
            {
                Amount = point,
                CreateTime = HelperProvider.GetSeconds(),
                Note = note,
                UserId = userId,
                UserTransactionId = Guid.NewGuid().ToString()
            }, transaction);
        }*/


    }
}