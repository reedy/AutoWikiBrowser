using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using WikiFunctions;
using System.IO;

namespace UnitTests
{
    [TestFixture]
    public class SiteInfoTests
    {
        SiteInfoCache c;

        string fileName = Environment.GetEnvironmentVariable("TEMP") + "/$Tests.xml";

        public SiteInfoTests()
        {
            TearDown();
        }

        [SetUp]
        public void SetUp()
        {
            c = new SiteInfoCache(fileName);
        }

        [TearDown]
        public void TearDown()
        {
            //if (File.Exists("$Tests.xml")) File.Delete("$Tests.xml");
        }

        [Test]
        public void Normalisation()
        {
            Assert.AreEqual("http://foo/", SiteInfo.NormalizeURL("http://foo"));
            Assert.AreEqual("http://foo/", SiteInfo.NormalizeURL("http://foo/"));
        }

        [Test]
        public void Creation()
        {
            SiteInfo si = new SiteInfo("http://foo/", Variables.enLangNamespaces);
            c.Add(si);
            Assert.AreEqual(si, c.Find(si.ScriptPath));
            Assert.AreEqual(si, c.Find("http://foo"));
            c.SaveCache();
        }
    }
}
