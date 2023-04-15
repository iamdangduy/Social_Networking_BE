using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GiveAndReceive.Models
{
    public class SystemConfig
    {
        public string SystemConfigId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string ValueType { get; set; }

        public class EnumSystemConfigId
        {
            public const string AVATAR = "AVATAR";
            public const string LIMIT_GIVE = "LIMIT_GIVE";
            public const string LIMIT_RECEIVE = "LIMIT_RECEIVE";
        }
        public class EnumValueType
        {
            public const string STRING = "STRING";
            public const string BOOLEAN = "BOOLEAN";
            public const string NUMBER = "NUMBER";
            public const string IMAGE = "IMAGE";
        }
    }

}