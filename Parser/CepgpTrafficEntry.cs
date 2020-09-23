using System;
using System.Collections.Generic;
using System.Text;

namespace CepgpParser.Parser
{
    public class CepgpTrafficEntry
    {
        public long Key;
        public DateTime? Date;
        public string Player;
        public string IssuedBy;
        public string Action;
        public string Item;
        public int? EpBefore;
        public int? EpAfter;
        public int? GpBefore;
        public int? GpAfter;
    }
}
