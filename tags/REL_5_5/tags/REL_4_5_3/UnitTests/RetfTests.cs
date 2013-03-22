using System;
using System.Collections.Generic;
using WikiFunctions.Parse;
using NUnit.Framework;

namespace UnitTests
{
    public class TypoList : ITyposProvider
    {
        readonly Dictionary<string, string> typos;

        public TypoList(Dictionary<string, string> list)
        {
            typos = list;
        }

        public TypoList(IEnumerable<KeyValuePair<string, string>> list)
        {
            typos = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> p in list)
            {
                typos.Add(p.Key, p.Value);
            }
        }

        public Dictionary<string, string> GetTypos()
        {
            return typos;
        }
    }

    [TestFixture]
    public class RetfTests : RequiresInitialization
    {
        #region Preparations
        RegExTypoFix retf;
        readonly Dictionary<string, string> typos = new Dictionary<string,string>();
        string Summary;

        [SetUp]
        public void SetUp()
        {
            typos.Clear();
            retf = null;
        }

        private string FixTypos(string ArticleText)
        {
            return FixTypos(ArticleText, "Test");
        }

        private string FixTypos(string ArticleText, string ArticleTitle)
        {
            if (retf == null)
            {
                if (typos.Count == 0) throw new Exception("You forgot to provide a list of typos!");
                retf = new RegExTypoFix(false, new TypoList(typos));
            }

            bool noChange;
            return retf.PerformTypoFixes(ArticleText, out noChange, out Summary, ArticleTitle);
        }

        private void AssertFix(string expected, string ArticleText, string ArticleTitle)
        {
            Assert.AreEqual(expected, FixTypos(ArticleText, ArticleTitle));
        }

        private void AssertFix(string expected, string ArticleText)
        {
            Assert.AreEqual(expected, FixTypos(ArticleText));
        }

        private void AssertNoFix(string ArticleText, string ArticleTitle)
        {
            Assert.AreEqual(ArticleText, FixTypos(ArticleText, ArticleTitle));
        }

        private void AssertNoFix(string ArticleText)
        {
            Assert.AreEqual(ArticleText, FixTypos(ArticleText));
        }
        #endregion

        [Test]
        public void SimpleTypos()
        {
            typos["foo"] = "bar";

            AssertNoFix("");
            AssertNoFix("test");

            AssertFix("the bars are cute", FixTypos("the foos are cute"));
        }

        [Test]
        public void CaseSensitivity()
        {
            typos["foo"] = "bar";
            typos["[Bb]oz"] = "quux";
            typos["(?i:fubar)"] = "fur";

            // fixes matching case
            AssertFix("bars", "foos");
            // ...but not mismatching
            AssertNoFix("Foobar");

            // should handle case-insensitive regex constructs properly
            AssertFix("quux", "boz");
            AssertFix("quux", "Boz");
            AssertNoFix("BOZ");
            AssertFix("furbag", "FuBaRbag");
        }

        [Test]
        // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_6#Rules_in_Finnish_Regex_TypoFix_list_not_always_applied
        public void Backreferences()
        {
            typos["(taht)"] = "that";
            typos[@"\b(a|the)\b\s+\1\b"] = "$1";

            // all rules must function separately
            AssertFix("that", "taht");
            AssertFix("the", "the the");
            AssertNoFix("a the"); // just in case

            // matches from other rules must not pollute backreferences
            AssertFix("that the", "taht the the");
        }

        [Test]
        public void SummaryTests()
        {
            typos["f[oO0]{2}"] = "foo";

            AssertFix("foo foo", "foo fO0");
            Assert.That(!Summary.Contains("foo → foo"));
            Assert.That(!Summary.Contains("2"));
        }

        [Test]
        public void FixLinkFaces()
        {
            typos["foo"] = "bar";

            AssertNoFix("[[foo]]");
            AssertFix("[[foo|bar]]", "[[foo|foo]]");
        }

        [Test]
        public void AllThreeGroups()
        {
            typos[@"\booze\b"] = "booze!";
            typos[@"foo(\w*)\b"] = "bar$1";
            typos[@"\b(a|the)\b\s+\1\b"] = "$1";

            AssertFix("barwards viva the booze!", "foowards viva the the ooze");
        }

        [Test]
        public void DontFixCertainStuff()
        {
            typos["(T|t)eh"] = "the";

            AssertNoFix(@"{{teh}} http://tehcrappe.org/TehCrappe [[teh]] [http://tehcrappe.org/TehCrappe]");
        }

        [Test]
        public void DontChangeTitle()
        {
            typos["teh"] = "the";
            typos["foo"] = "bar";

            AssertFix("teh bar", "teh foo", "teh");
            AssertFix("teh bar", "teh foo", "teh crap");
        }
    }
}