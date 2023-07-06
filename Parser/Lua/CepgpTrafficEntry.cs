using System;
using System.ComponentModel;

namespace CepgpParser.Parser.Lua
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
        [Description("cff9d9d9d")]
        Poor = 1,
        [Description("cffffffff")]
        Common = 1,
        [Description("cff1eff00")]
        Uncommon = 2,
        [Description("cff0070dd")]
        Rare = 3,
        [Description("cffa335ee")]
        Epic = 4,
        [Description("cffff8000")]
        Legendary = 5
    }
}
