using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sampleapp.Tests.UnitTests.Helpers
{
    public static class DocModNumConvertersFactory
    {
        public const string TimeFormat = "yyyyMMddHHmmssfff";
        /// <summary>
        /// Helper function to convert a long generated from a datetime back into a datetime
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this long value)
        {
            return DateTime.ParseExact(value.ToString(), TimeFormat, System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Helper funciton to convert a string generated from a datetime back into a datetime
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string value)
        {
            return DateTime.ParseExact(value, TimeFormat, System.Globalization.CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// Helper function to convert a DateTime to a long.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long ToLong(this DateTime value)
        {
            return long.Parse(value.ToString(TimeFormat));
        }
    }
}
