using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GiveAndReceive.Providers
{
    public class HelperProvider
    {
        public static long GetSeconds(DateTime? dateTime = null)
        {
            try
            {
                if (!dateTime.HasValue) dateTime = DateTime.Now;
                var Timestamp = new DateTimeOffset(dateTime.Value).ToUnixTimeMilliseconds();
                return Timestamp;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}