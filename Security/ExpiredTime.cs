using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GNT_server.Security
{
    public class ExpiredTime
    {
        public static DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        public static long ETime = 0;
    }
}