using CepgpParser.Parser.Common;
using CepgpParser.Parser.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace CepgpParser.Parser.Standings
{
    public class CepgpStandingsParser
    {
        public List<CepgpParserLog> Logs = new List<CepgpParserLog>();
        public CepgpStandings Standings;

        public void ParseFile(string filePath)
        {
            string standingsJson = File.ReadAllText(filePath);
            Parse(standingsJson);
        }

        public void Parse(string standingsJson)
        {
            AddLog(CepgpParserLogLevel.Info, "Parsing input json...");

            CepgpStandingsRaw raw = ParseToRaw(standingsJson);
            if (raw == null)
                return;

            if (raw.Roster.Count == 0)
            {
                AddLog(CepgpParserLogLevel.Warning, "Missing roster data, empty array");
                return;
            }

            Standings = new CepgpStandings
            {
                Roster = raw.Roster
                    .Select(re => ParsePlayer(re))
                    .Where(x => x != null)
                    .ToList(),
                Timestamp = raw.Timestamp
            };
        }

        private CepgpStandingsRaw ParseToRaw(string standingsJson)
        {
            try
            {
                var json = JsonSerializer.Deserialize<CepgpStandingsRaw>(standingsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (json == null)
                    AddLog(CepgpParserLogLevel.Error, "Unable to parse, is the string empty?");
                else
                    return json;
            }
            catch (JsonException)
            {
                AddLog(CepgpParserLogLevel.Error, "Unable to parse, is it in json format?");
            }

            return null;
        }

        private CepgpStandingsPlayer ParsePlayer(List<object> rosterEntry)
        {
            if (rosterEntry.Count != 6)
            {
                AddLog(CepgpParserLogLevel.Error, $"Invalid roster entry: {rosterEntry.ToCsv()}");
                return null;
            }

            return new CepgpStandingsPlayer
            {
                Nick = ((JsonElement)rosterEntry[0]).GetString(),
                Class = ((JsonElement)rosterEntry[1]).GetString(),
                Rank = ((JsonElement)rosterEntry[2]).GetString(),
                EP = ((JsonElement)rosterEntry[3]).GetInt32(),
                GP = ((JsonElement)rosterEntry[4]).GetInt32(),
                Ratio = ((JsonElement)rosterEntry[5]).GetDecimal()
            };
        }

        private void AddLog(CepgpParserLogLevel level, string message)
        {
            Logs.Add(new CepgpParserLog { Level = level, Message = message });
        }
    }
}