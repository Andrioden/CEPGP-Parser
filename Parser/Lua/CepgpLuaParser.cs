using CepgpParser.Parser.Common;
using CepgpParser.Parser.Extensions;
using CepgpParser.Parser.Utils;
using NLua;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CepgpParser.Parser.Lua
{
    public class CepgpLuaParser
    {
        public List<CepgpParserLog> Logs = new List<CepgpParserLog>();
        public List<CepgpRecord> Records;
        public List<CepgpTrafficEntry> Traffic;
        public List<CepgpItemCostOverride> Overrides;
        public CepgpAlt Alt;

        private readonly Regex RecordEntryValueRegex = new Regex(@"^\d+,\d+$");

        public void ParseFile(string filePath)
        {
            using (NLua.Lua lua = new NLua.Lua())
            {
                lua.State.Encoding = Encoding.UTF8;
                lua.DoFile(filePath);

                Parse(lua);
            }
        }

        public async Task ParseAsync(Stream stream)
        {
            byte[] fileContent = await StreamUtils.ToByteArrayAsync(stream);

            using (NLua.Lua lua = new NLua.Lua())
            {
                lua.State.Encoding = Encoding.UTF8;
                lua.DoString(fileContent);

                Parse(lua);
            }
        }

        private void Parse(NLua.Lua lua)
        {
            Records = ParseRecords((LuaTable)lua["CEPGP.Backups"]);
            Traffic = ParseTraffic((LuaTable)lua["CEPGP.Traffic"]);
            Overrides = ParseOverrides((LuaTable)lua["CEPGP.Overrides"]);
            Alt = ParseAlt((LuaTable)lua["CEPGP.Alt"]);
        }

        private List<CepgpRecord> ParseRecords(LuaTable recordsTable)
        {
            List<CepgpRecord> cepgpRecords = new List<CepgpRecord>();

            foreach (object recordKey in recordsTable.Keys)
            {
                LuaTable recordTable = (LuaTable)recordsTable[recordKey];
                string recordName = (string)recordKey;

                CepgpRecord cepgpRecord = new CepgpRecord
                {
                    Name = recordName,
                    Date = recordName.IsDateTime("yyyyMMdd") ? recordName.ToDateTime("yyyyMMdd") : null,
                    Entries = new List<CepgpRecordEntry>()
                };

                foreach (object entryKey in recordTable.Keys)
                {
                    string entryValue = (string)recordTable[entryKey];

                    if (entryValue.IsNullOrEmpty())
                        entryValue = "0,1";
                    if (RecordEntryValueRegex.IsMatch(entryValue) == false)
                    {
                        AddLog(CepgpParserLogLevel.Warning_ParseIgnore, $"Ignoring record entry, bad format. Record '{recordName}', entry key '{entryKey.ToString()}', value '{entryValue}'");
                        continue;
                    }

                    cepgpRecord.Entries.Add(new CepgpRecordEntry
                    {
                        Player = entryKey.ToString().Split("-")[0],
                        Server = entryKey.ToString().Split("-")[1],
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

            foreach (long entryKey in trafficTable.Keys)
            {
                LuaTable entry = (LuaTable)trafficTable[entryKey];

                cepgpTraffic.Add(new CepgpTrafficEntry
                {
                    Key = entryKey,
                    Date = ParseDateTime(entry[9]),
                    Player = ParseString(entry[1]),
                    IssuedBy = ParseString(entry[2]),
                    Action = ParseString(entry[3]),
                    Item = ParseItem(entry[8], $"Traffic entry {entryKey}"),
                    EpBefore = ParseInt(entry[4]),
                    EpAfter = ParseInt(entry[5]),
                    GpBefore = ParseInt(entry[6]),
                    GpAfter = ParseInt(entry[7])
                });
            }

            return cepgpTraffic;
        }

        private List<CepgpItemCostOverride> ParseOverrides(LuaTable overridesTable)
        {
            List<CepgpItemCostOverride> overrides = new List<CepgpItemCostOverride>();

            foreach (string overrideKey in overridesTable.Keys)
            {
                int? value = ParseInt(overridesTable[overrideKey]);

                if (value == null)
                    continue;

                overrides.Add(new CepgpItemCostOverride
                {
                    Key = overrideKey,
                    Item = ParseItem(overrideKey),
                    GP = value.Value
                });
            }

            return overrides;
        }

        private CepgpAlt ParseAlt(LuaTable overridesTable)
        {
            if (overridesTable == null)
                return null;

            return new CepgpAlt
            {
                Links = ParseStringToStringListDict(overridesTable["Links"]),
                SyncEP = ParseBool(overridesTable["SyncEP"]),
                SyncGP = ParseBool(overridesTable["SyncGP"]),
                BlockAwards = ParseBool(overridesTable["BlockAwards"]),
            };
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

        private bool? ParseBool(object value)
        {
            if (value == null)
                return null;
            else if (value.GetType() == typeof(bool))
                return (bool)value;
            else
                return null;
        }

        private CepgpItem ParseItem(object value, string logParseContext = null)
        {
            if (value == null)
                return null;
            else if (value.GetType() == typeof(long) && (long)value == 0)
                return null;
            else if (value.GetType() != typeof(string))
            {
                if (logParseContext != null)
                    AddLog(CepgpParserLogLevel.Warning_ParseIgnore, $"Ignoring {logParseContext} item value. Is unknown non-string value: '{value.ToString()}'");
                return null;
            }

            string strValue = (string)value;

            if (strValue.IsNullOrEmpty())
                return null;
            if (!strValue.Contains("[") || !strValue.Contains("]"))
            {
                if (logParseContext != null)
                    AddLog(CepgpParserLogLevel.Warning_ParseIgnore, $"Ignoring {logParseContext} item value. Bad format: '{value.ToString()}'");
                return null;
            }

            string itemId = strValue.Between("Hitem:", "::::::::");
            if (itemId.IsInteger() == false)
            {
                if (logParseContext != null)
                    AddLog(CepgpParserLogLevel.Warning_ParseIgnore, $"Ignoring {logParseContext} item value. Bad item id: '{itemId}'");
                return null;
            }

            return new CepgpItem
            {
                Id = itemId.ToInteger(),
                Name = strValue.Between("[", "]"),
                Quality = EnumUtils.GetEnumValueFromDescription<CepgpItemQuality>(strValue.Between("|", "|Hitem"))
            };
        }

        private DateTime? ParseDateTime(object value)
        {
            if (value == null)
                return null;

            long epoch = value.GetType() == typeof(long) ? (long)value : ((string)value).ToLong();

            return DateTimeOffset.FromUnixTimeSeconds(epoch).UtcDateTime;
        }

        private Dictionary<string, List<string>> ParseStringToStringListDict(object value)
        {
            if (value == null)
                return null;

            LuaTable table = (LuaTable)value;

            Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();
            foreach (string key in table.Keys)
                dict[key] = ((LuaTable)table[key]).Values.Cast<string>().ToList();

            return dict;
        }

        private void AddLog(CepgpParserLogLevel level, string message)
        {
            Logs.Add(new CepgpParserLog { Level = level, Message = message });
        }
    }
}
