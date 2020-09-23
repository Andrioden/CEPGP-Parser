using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace CepgpParser.Parser.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static bool IsInteger(this string value)
        {
            if (value.IsNullOrEmpty())
                return false;

            return int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out int _);
        }

        public static int ToInteger(this string value)
        {
            int result;
            bool success = int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
            if (!success)
                throw new ArgumentException($"Was not able to convert '{value}' to integer");

            return result;
        }

        public static bool IsDateTime(this string value, string format)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            return DateTime.TryParseExact(value, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime _);
        }

        public static DateTime ToDateTime(this string value, string format)
        {
            return DateTime.ParseExact(value, format, CultureInfo.InvariantCulture);
        }
    }
}
