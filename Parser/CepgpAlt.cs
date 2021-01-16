using System;
using System.Collections.Generic;
using System.Text;

namespace CepgpParser.Parser
{
    public class CepgpAlt
    {
        public Dictionary<string, List<string>> Links = new Dictionary<string, List<string>>();
        public bool? SyncEP;
        public bool? SyncGP;
        public bool? BlockAwards;
    }
}
