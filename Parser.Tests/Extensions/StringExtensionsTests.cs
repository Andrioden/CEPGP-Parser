using CepgpParser.Parser.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace CepgpParser.Parser.Extensions.Tests
{
    [TestClass]
    public class StringExtensionsTests
    {
        [TestMethod]
        public void StringExtensions_Between()
        {
            Assert.AreEqual("21581", "|cffa335ee|Hitem:21581:::::::::::::|h[Gauntlets of Annihilation]|h|r".Between("Hitem:", "::::::::"));
            Assert.AreEqual("16819", "|cffa335ee|Hitem:16819::::::::60:::::::|h[Vambraces of Prophecy]|h|r".Between("Hitem:", "::::::::"));
        }
    }
}
