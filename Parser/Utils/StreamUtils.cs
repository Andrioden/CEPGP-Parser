using System.IO;
using System.Threading.Tasks;

namespace CepgpParser.Parser.Utils
{
    public static class StreamUtils
    {
        public async static Task<byte[]> ToByteArrayAsync(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                await input.CopyToAsync(ms);
                return ms.ToArray();
            }
        }
    }
}
