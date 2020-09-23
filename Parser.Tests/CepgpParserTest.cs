using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace CepgpParser.Parser.Tests
{
    [TestClass]
    public class CepgpParserTest
    {
        [TestMethod]
        public void CepgpParser_full_test()
        {
            CepgpParser parser = new CepgpParser();
            parser.Parse("CEPGP.lua");

            // Records
            Assert.IsTrue(parser.Records.Count > 10);
            Assert.IsTrue(parser.Records.Any(r => r.Name == "20191025")); // From RECORDS but also in backups
            Assert.IsTrue(parser.Records.Any(r => r.Name == "20200501")); // From RECORDS

            // Record: 20200501
            CepgpRecord record = parser.Records.First(r => r.Name == "20200501");
            Assert.AreEqual(new DateTime(2020, 5, 1), record.Date);
            Assert.AreEqual(139, record.Entries.Count);

            // Record: 20200501 : Castortoy-ZandalarTribe
            CepgpRecordEntry entry = record.Entries.First(r => r.Nick == "Castortoy");
            Assert.AreEqual("ZandalarTribe", entry.Server);
            Assert.AreEqual(307, entry.EP);
            Assert.AreEqual(302, entry.GP);
        }
    }
}