using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public CepgpItem Item;
        public int? EpBefore;
        public int? EpAfter;
        public int? GpBefore;
        public int? GpAfter;
    }

    public class CepgpItem
    {
        public int Id;
        public string Name;
        public CepgpItemQuality Quality;
    }

    public enum CepgpItemQuality
    {
        [Description("cff1eff00")]
        Uncommon,
        [Description("cff0070dd")]
        Rare,
        [Description("cffa335ee")]
        Epic,
        [Description("cffff8000")]
        Legendary
    }
}
