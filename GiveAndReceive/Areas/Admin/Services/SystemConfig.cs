namespace GiveAndReceive.Areas.Admin.Services
{
    public class SystemConfig
    {
        public string SystemConfigId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string ValueType { get; set; }
        public class EnumValueType
        {
            public const string STRING = "STRING";
            public const string BOOLEAN = "BOOLEAN";
            public const string NUMBER = "NUMBER";
            public const string IMAGE = "IMAGE";
        }
    }
}