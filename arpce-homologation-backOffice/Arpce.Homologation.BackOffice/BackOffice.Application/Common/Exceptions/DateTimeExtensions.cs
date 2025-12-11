using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackOffice.Application.Common.Exceptions
{
    public static class DateTimeExtensions
    {
        // Convertit DateTime? en long?
        public static long? ToUnixTimeMilliseconds(this DateTime? dateTime)
        {
            if (!dateTime.HasValue) return null;
            return new DateTimeOffset(dateTime.Value.ToUniversalTime()).ToUnixTimeMilliseconds();
        }

        // Convertit DateTime en long
        public static long ToUnixTimeMilliseconds(this DateTime dateTime)
        {
            return new DateTimeOffset(dateTime.ToUniversalTime()).ToUnixTimeMilliseconds();
        }

        // Convertit long? en DateTime?
        public static DateTime? FromUnixTimeMilliseconds(this long? unixTimeMilliseconds)
        {
            if (!unixTimeMilliseconds.HasValue) return null;
            return DateTimeOffset.FromUnixTimeMilliseconds(unixTimeMilliseconds.Value).DateTime;
        }

       
        public static DateTime FromUnixTimeMilliseconds(this long unixTimeMilliseconds)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(unixTimeMilliseconds).DateTime;
        }
    }
}
