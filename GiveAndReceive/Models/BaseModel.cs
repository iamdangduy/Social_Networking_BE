using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GiveAndReceive.Models
{
    public class JsonResult
    {
        public string status { get; set; }
        public object data { get; set; }
        public string message { get; set; }
        public override string ToString()
        {
            string content = "{\"status\":\"" + this.status + "\",\"data\":null ,\"message\":\"" + this.message + "\"  }";
            return content;
        }
        public static class Status
        {
            public const string SUCCESS = "success";
            public const string ERROR = "error";
            public const string UNAUTHORIZED = "unauthorized";
            public const string UNAUTHENTICATED = "unauthenticated";
        }
        public static class Message
        {
            public const string ERROR_SYSTEM = "Có lỗi xảy ra trong quá trình xử lý.";
            public const string TOKEN_EXPIRED = "Token đã hết hạn.";
            public const string NO_PERMISSION = "Không có quyền truy cập.";
            public const string UPDATED_SUCCESS = "Cập nhật thành công.";
            public const string DELETED_SUCCESS = "Xóa thành công.";

            public const string LOGIN_ACCOUNT_OR_PASSWORD_EMPTY = "Số điện thoại/Email/Tên tài khoản hoặc mật khẩu không được để trống.";
            public const string LOGIN_ACCOUNT_OR_PASSWORD_INCORRECT = "Số điện thoại/Email/Tên tài khoản hoặc mật khẩu không chính xác.";
        }
    }
}