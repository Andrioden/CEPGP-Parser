using System;

namespace CepgpParser.Parser.Extensions
{
    public static class DateTimeExtensions
    {
        public static long ToEpoch(this DateTime value)
        {
            return new DateTimeOffset(value).ToUnixTimeSeconds();
        }
    }
}