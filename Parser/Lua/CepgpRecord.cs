using System;
using System.Collections.Generic;

namespace CepgpParser.Parser.Lua
{
    public class CepgpRecord
    {
        public string Name;
        public DateTime? Date;
        public List<CepgpRecordEntry> Entries;
    }

    public class CepgpRecordEntry
    {
        public string Player;
        public string Server;
        public int EP;
        public int GP;

        public override string ToString()
        {
            return $"{Player}-{Server} : {EP},{GP}";
        }
    }
}
