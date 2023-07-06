using System;
using System.Collections.Generic;
using System.Linq;

namespace CepgpParser.Parser.Extensions
{
    internal static class ListExtensions
    {
        public static string ToCsv(this List<object> list)
        {
            return String.Join(",", list.Select(x => x.ToString()));
        }
    }
}
