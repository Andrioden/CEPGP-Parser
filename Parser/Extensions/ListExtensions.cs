using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
