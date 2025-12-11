using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontOffice.Application.Common.Exceptions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Convertit un DateTime? nullable en un timestamp Unix (long?) en millisecondes.
        /// </summary>
        public static long? ToUnixTimeMilliseconds(this DateTime? dateTime)
        {
            if (!dateTime.HasValue)
            {
                return null;
            }
            // S'assurer que la date est en UTC avant la conversion pour éviter les problèmes de fuseau horaire
            return new DateTimeOffset(dateTime.Value.ToUniversalTime()).ToUnixTimeMilliseconds();
        }

        /// <summary>
        /// Convertit un timestamp Unix (long?) en millisecondes en un DateTime? nullable.
        /// </summary>
        public static DateTime? FromUnixTimeMilliseconds(this long? unixTimeMilliseconds)
        {
            if (!unixTimeMilliseconds.HasValue)
            {
                return null;
            }
            return DateTimeOffset.FromUnixTimeMilliseconds(unixTimeMilliseconds.Value).DateTime;
        }
    }
}
