using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using CepgpParser.Parser.Extensions;

namespace CepgpParser.Parser.Tests
{
    [TestClass]
    public class LuaCepgpParserTest
    {
        [TestMethod]
        public void CepgpParser_full_test()
        {
            LuaCepgpParser parser = new LuaCepgpParser();
            parser.Parse("CEPGP.lua");

            // Records
            Assert.IsTrue(parser.Records.Count > 10);
            Assert.IsTrue(parser.Records.Any(r => r.Name == "20191025")); // From RECORDS but also in backups
            Assert.IsTrue(parser.Records.Any(r => r.Name == "20200501")); // From RECORDS

            // Record - 20200501
            CepgpRecord record = parser.Records.First(r => r.Name == "20200501");
            Assert.AreEqual(new DateTime(2020, 5, 1), record.Date);
            Assert.AreEqual(139, record.Entries.Count);

            // Record - 20200501 - Castortoy-ZandalarTribe
            CepgpRecordEntry entry = record.Entries.First(r => r.Nick == "Castortoy");
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
            Assert.AreEqual("Gauntlets of Annihilation", newTraffic.Item);
            Assert.AreEqual(1600635769, newTraffic.Date.Value.ToEpoch());

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
        }
    }
}