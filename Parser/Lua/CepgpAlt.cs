using System.Collections.Generic;

namespace CepgpParser.Parser.Lua
{
    public class CepgpAlt
    {
        public Dictionary<string, List<string>> Links = new Dictionary<string, List<string>>();
        public bool? SyncEP;
        public bool? SyncGP;
        public bool? BlockAwards;
    }
}
