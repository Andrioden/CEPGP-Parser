using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using CepgpParser.Parser;
using CepgpParser.Parser.Standings;
using CepgpParser.Parser.Common;

namespace CepgpParser.Parser.Standings.Tests
{
    [TestClass]
    public class CepgpStandingsParserTests
    {
        [TestMethod]
        public void CepgpStandingsParser_Parse_simple()
        {
            var parser = new CepgpStandingsParser();
            parser.Parse("{\"roster\": [[\"Dude\",\"Druid\",\"Alt\",20,10,2]], \"timestamp\":1688053483 }");

            Assert.AreEqual(1, parser.Standings.Roster.Count);
            Assert.AreEqual("Dude", parser.Standings.Roster[0].Nick);
            Assert.AreEqual("Druid", parser.Standings.Roster[0].Class);
            Assert.AreEqual("Alt", parser.Standings.Roster[0].Rank);
            Assert.AreEqual(20, parser.Standings.Roster[0].EP);
            Assert.AreEqual(10, parser.Standings.Roster[0].GP);
            Assert.AreEqual(2, parser.Standings.Roster[0].Ratio);

            Assert.IsFalse(parser.Logs.Any(x => x.Level == CepgpParserLogLevel.Error));
        }

        [TestMethod]
        public void CepgpStandingsParser_ParseFile_complex()
        {
            var parser = new CepgpStandingsParser();
            parser.ParseFile("Standings/CEPGP_standings_wrath.json");

            Assert.AreEqual(524, parser.Standings.Roster.Count);
            Assert.IsFalse(parser.Logs.Any(x => x.Level == CepgpParserLogLevel.Error));
        }

        [TestMethod]
        public void CepgpStandingsParser_Parse_empty()
        {
            var parser = new CepgpStandingsParser();
            parser.Parse("");

            Assert.AreEqual(null, parser.Standings);
            Assert.IsTrue(parser.Logs.Any(x => x.Level == CepgpParserLogLevel.Error));
        }

        [TestMethod]
        public void CepgpStandingsParser_Parse_not_json()
        {
            var parser = new CepgpStandingsParser();
            parser.Parse("Airplane,Priest,Alt,0,1,0,");

            Assert.AreEqual(null, parser.Standings);
            Assert.IsTrue(parser.Logs.Any(x => x.Level == CepgpParserLogLevel.Error));
        }
    }
}