using CepgpParser.Parser.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CepgpParser.Parser.Lua.Tests
{
    [TestClass]
    public class CepgpLuaParserTests
    {
        [TestMethod]
        public void CepgpLuaParser_ParseFile_new_file()
        {
            CepgpLuaParser parser = new CepgpLuaParser();
            parser.ParseFile("Lua/CEPGP.lua");

            // Records
            Assert.IsTrue(parser.Records.Count > 10);
            Assert.IsTrue(parser.Records.Any(r => r.Name == "20191025")); // From RECORDS but also in backups
            Assert.IsTrue(parser.Records.Any(r => r.Name == "20200501")); // From RECORDS

            // Record - 20200501
            CepgpRecord record = parser.Records.First(r => r.Name == "20200501");
            Assert.AreEqual(new DateTime(2020, 5, 1), record.Date);
            Assert.AreEqual(139, record.Entries.Count);

            // Record - 20200501 - Castortoy-ZandalarTribe
            CepgpRecordEntry entry = record.Entries.First(r => r.Player == "Castortoy");
            Assert.AreEqual("ZandalarTribe", entry.Server);
            Assert.AreEqual(307, entry.EP);
            Assert.AreEqual(302, entry.GP);


            // Traffic
            Assert.IsTrue(parser.Traffic.Count > 100);

            // Traffic - Newest format
            CepgpTrafficEntry newTraffic = parser.Traffic.First(e => e.Key == 5034);
            Assert.AreEqual("Fosen", newTraffic.Player);
            Assert.AreEqual("Andriod", newTraffic.IssuedBy);
            Assert.AreEqual("Add GP 450 (Full Price)", newTraffic.Action);
            Assert.AreEqual(409, newTraffic.EpBefore);
            Assert.AreEqual(409, newTraffic.EpAfter);
            Assert.AreEqual(164, newTraffic.GpBefore);
            Assert.AreEqual(614, newTraffic.GpAfter);
            Assert.AreEqual(21581, newTraffic.Item.Id);
            Assert.AreEqual("Gauntlets of Annihilation", newTraffic.Item.Name);
            Assert.AreEqual(CepgpItemQuality.Epic, newTraffic.Item.Quality);
            Assert.AreEqual(1600635769, newTraffic.Date.Value.ToEpoch());

            // Traffic - Legdendary item
            CepgpTrafficEntry ledgTraffic = parser.Traffic.First(e => e.Key == 1656);
            Assert.AreEqual(CepgpItemQuality.Legendary, ledgTraffic.Item.Quality);

            // Traffic - Old format
            CepgpTrafficEntry oldTraffic1 = parser.Traffic.First(e => e.Key == 1);
            Assert.AreEqual(null, oldTraffic1.Item);
            Assert.AreEqual(null, oldTraffic1.Date);

            // Traffic - Old format with string ep/gp
            CepgpTrafficEntry oldTraffic2 = parser.Traffic.First(e => e.Key == 174);
            Assert.AreEqual(95, oldTraffic2.EpBefore);
            Assert.AreEqual(95, oldTraffic2.EpAfter);
            Assert.AreEqual(291, oldTraffic2.GpBefore);
            Assert.AreEqual(491, oldTraffic2.GpAfter);

            // Traffic - Old format with string date epoch
            CepgpTrafficEntry oldTraffic3 = parser.Traffic.First(e => e.Key == 1030);
            Assert.AreEqual(1581885547, oldTraffic3.Date.Value.ToEpoch());

            // Traffic - Old format bugged date
            CepgpTrafficEntry oldTraffic4 = parser.Traffic.First(e => e.Key == 903);
            Assert.AreEqual(1580676719, oldTraffic4.Date.Value.ToEpoch());


            // Overrides
            Assert.IsTrue(parser.Overrides.Count > 100);

            // Override - only item name
            CepgpItemCostOverride over1 = parser.Overrides.First(o => o.Key == "Boots of Epiphany");
            Assert.AreEqual(null, over1.Item);
            Assert.AreEqual(150, over1.GP);

            // Override - full
            CepgpItemCostOverride over2 = parser.Overrides.First(o => o.Item?.Name == "Felheart Gloves");
            Assert.AreEqual("|cffa335ee|Hitem:16805:::::::::::::|h[Felheart Gloves]|h|r", over2.Key);
            Assert.AreEqual(16805, over2.Item.Id);
            Assert.AreEqual(CepgpItemQuality.Epic, over2.Item.Quality);
            Assert.AreEqual(75, over2.GP);
        }

        [TestMethod]
        public void CepgpLuaParser_ParseFile_old_file()
        {
            CepgpLuaParser parser = new CepgpLuaParser();
            parser.ParseFile("Lua/CEPGP_old.lua");

            Assert.IsTrue(parser.Records.Count > 10);
            Assert.IsTrue(parser.Traffic.Count > 100);
            Assert.IsTrue(parser.Overrides.Count > 100);
        }

        [TestMethod]
        public void CepgpLuaParser_ParseFile_traffic_bad_value()
        {
            CepgpLuaParser parser = new CepgpLuaParser();
            parser.ParseFile("Lua/CEPGP_traffic_bad_value.lua");

            Assert.AreEqual(2, parser.Traffic.Count(t => t.Item != null));
            Assert.AreEqual(1, parser.Traffic.Count(t => t.Item == null));
        }

        [TestMethod]
        public void CepgpLuaParser_ParseFile_record_entity_bad_value()
        {
            CepgpLuaParser parser = new CepgpLuaParser();
            parser.ParseFile("Lua/CEPGP_record_entity_bad_value.lua");

            Assert.AreEqual(1, parser.Records.Count);
            Assert.AreEqual(2, parser.Records[0].Entries.Count());
        }

        [TestMethod]
        public void CepgpLuaParser_ParseFile_alt_linking()
        {
            CepgpLuaParser parser = new CepgpLuaParser();
            parser.ParseFile("Lua/CEPGP_alt_linking.lua");

            Assert.AreEqual("Main", parser.Alt.Links.Keys.First());
            CollectionAssert.AreEqual(new List<string> { "Alt1", "Alt2" }, parser.Alt.Links["Main"]);
            Assert.AreEqual(true, parser.Alt.SyncEP);
            Assert.AreEqual(true, parser.Alt.SyncGP);
            Assert.AreEqual(false, parser.Alt.BlockAwards);
        }

        [TestMethod]
        public async Task CepgpLuaParser_Parse_stream()
        {
            using (FileStream file = new FileStream("Lua/CEPGP.lua", FileMode.Open, FileAccess.Read))
            {
                CepgpLuaParser parser = new CepgpLuaParser();
                await parser.ParseAsync(file);

                Assert.IsTrue(parser.Records.Count > 10);
                Assert.IsTrue(parser.Traffic.Count > 100);
            }
        }
    }
}