using System;
using System.Linq;
using NLua;
using System.Collections;
using System.Collections.Generic;
using CepgpParser.Parser.Extensions;
using System.Text;

namespace CepgpParser.Parser
{
    public class CepgpParser
    {
        //public List<CepgpParserLog> Logs = new List<CepgpParserLog>();
        public List<CepgpRecord> Records;

        public void Parse(string filePath)
        {
            using (Lua lua = new Lua())
            {
                lua.State.Encoding = Encoding.UTF8;
                lua.DoFile(filePath);

                Records = ParseRecords((LuaTable)lua["RECORDS"]);
            }
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

        //public void AddLog(string message)
        //{
        //    Logs.Add(new CepgpParserLog { Message = message });
        //}
    }

    public class CepgpParserLog
    {
        public string Message;
    }
}
