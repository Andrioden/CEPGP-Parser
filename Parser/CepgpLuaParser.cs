using System;
using System.Linq;
using NLua;
using System.Collections;
using System.Collections.Generic;
using CepgpParser.Parser.Extensions;
using System.Text;
using System.IO;
using CepgpParser.Parser.Utils;
using System.Threading.Tasks;

namespace CepgpParser.Parser
{
    public class CepgpLuaParser
    {
        public List<CepgpParserLog> Logs = new List<CepgpParserLog>();
        public List<CepgpRecord> Records;
        public List<CepgpTrafficEntry> Traffic;

        public void Parse(string filePath)
        {
            using (Lua lua = new Lua())
            {
                lua.State.Encoding = Encoding.UTF8;
                lua.DoFile(filePath);

                Parse(lua);
            }
        }

        public async Task ParseAsync(Stream stream)
        {
            byte[] fileContent = await StreamUtils.ToByteArrayAsync(stream);

            using (Lua lua = new Lua())
            {
                lua.State.Encoding = Encoding.UTF8;
                lua.DoString(fileContent);

                Parse(lua);
            }
        }

        private void Parse(Lua lua)
        {
            Records = ParseRecords((LuaTable)lua["RECORDS"]);
            Traffic = ParseTraffic((LuaTable)lua["TRAFFIC"]);
        }

        private List<CepgpRecord> ParseRecords(LuaTable recordsTable)
        {
            List<CepgpRecord> cepgpRecords = new List<CepgpRecord>();

            foreach (string recordKey in recordsTable.Keys)
            {
                LuaTable recordTable = ((LuaTable)recordsTable[recordKey]);

                CepgpRecord cepgpRecord = new CepgpRecord
                {
                    Name = recordKey,
                    Date = recordKey.IsDateTime("yyyyMMdd") ? recordKey.ToDateTime("yyyyMMdd") : (DateTime?)null,
                    Entries = new List<CepgpRecordEntry>()
                };

                foreach (string entryKey in recordTable.Keys)
                {
                    string entryValue = (string)recordTable[entryKey];

                    cepgpRecord.Entries.Add(new CepgpRecordEntry
                    {
                        Nick = entryKey.Split("-")[0],
                        Server = entryKey.Split("-")[1],
                        EP = entryValue.Split(",")[0].ToInteger(),
                        GP = entryValue.Split(",")[1].ToInteger()
                    });
                }

                cepgpRecords.Add(cepgpRecord);
            }

            return cepgpRecords;
        }

        private List<CepgpTrafficEntry> ParseTraffic(LuaTable trafficTable)
        {
            List<CepgpTrafficEntry> cepgpTraffic = new List<CepgpTrafficEntry>();

            foreach(long entryKey in trafficTable.Keys)
            {
                LuaTable entry = (LuaTable)trafficTable[entryKey];

                cepgpTraffic.Add(new CepgpTrafficEntry
                {
                    Key = entryKey,
                    Date = ParseDateTime(entry[9]),
                    Player = ParseString(entry[1]),
                    IssuedBy = ParseString(entry[2]),
                    Action = ParseString(entry[3]),
                    Item = ParseItem(entry[8], entryKey),
                    EpBefore = ParseInt(entry[4]),
                    EpAfter = ParseInt(entry[5]),
                    GpBefore = ParseInt(entry[6]),
                    GpAfter = ParseInt(entry[7])
                });
            }

            return cepgpTraffic;
        }

        private string ParseString(object value)
        {
            return (string)value;
        }

        private int? ParseInt(object value)
        {
            if (value == null)
                return null;
            else if (value.GetType() == typeof(string) && ((string)value).IsInteger())
                return ((string)value).ToInteger();
            else if (value.GetType() == typeof(long))
                return Convert.ToInt32(value);
            else
                return null;
        }

        private CepgpTrafficEntryItem ParseItem(object value, long entryKey)
        {
            if (value == null)
                return null;
            else if (value.GetType() == typeof(long) && (long)value == 0)
                return null;
            else if (value.GetType() != typeof(string))
            {
                AddLog(CepgpParserLogLevel.Warning_ParseIgnore, $"Ignoring entry {entryKey} item value. Is unknown non-string value: '{value.ToString()}'");
                return null;
            }

            string strValue = ((string)value);

            if (strValue.IsNullOrEmpty())
                return null;
            if (!strValue.Contains("[") || !strValue.Contains("]"))
            {
                AddLog(CepgpParserLogLevel.Warning_ParseIgnore, $"Ignoring entry {entryKey} item value. Bad format: '{value.ToString()}'");
                return null;
            }

            return new CepgpTrafficEntryItem
            {
                Id = strValue.Between("Hitem:", "::::::::").ToInteger(),
                Name = strValue.Between("[", "]")
            };
        }

        private DateTime? ParseDateTime(object value)
        {
            if (value == null)
                return null;

            long epoch = value.GetType() == typeof(long) ? (long)value : ((string)value).ToLong();
            
            return DateTimeOffset.FromUnixTimeSeconds(epoch).UtcDateTime;
        }

        public void AddLog(CepgpParserLogLevel level, string message)
        {
            Logs.Add(new CepgpParserLog { Level = level, Message = message });
        }
    }

    public class CepgpParserLog
    {
        public CepgpParserLogLevel Level;
        public string Message;
    }

    public enum CepgpParserLogLevel
    {
        Info,
        Warning,
        Warning_ParseIgnore,
        Error
    }
}
