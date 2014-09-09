using System;
using System.Collections.Generic;
using WikiFunctions.Parse;
using NUnit.Framework;

namespace UnitTests
{
    public class TypoList : ITyposProvider
    {
        readonly Dictionary<string, string> Typos;

        public TypoList(Dictionary<string, string> list)
        {
            Typos = list;
        }

        public Dictionary<string, string> GetTypos()
        {
            return Typos;
        }
    }

    [TestFixture]
    public class RetfTests : RequiresInitialization
    {
        #region Preparations
        private RegExTypoFix Retf;
        private readonly Dictionary<string, string> Typos = new Dictionary<string, string>();
        private string Summary;

        [SetUp]
        public void SetUp()
        {
            Typos.Clear();
            Retf = null;
        }

        private string FixTypos(string articleText)
        {
            return FixTypos(articleText, "Test");
        }

        private string FixTypos(string articleText, string articleTitle)
        {
            if (Retf == null)
            {
                if (Typos.Count == 0) throw new Exception("You forgot to provide a list of typos!");
                Retf = new RegExTypoFix(false, new TypoList(Typos));
            }

            bool noChange;
            return Retf.PerformTypoFixes(articleText, out noChange, out Summary, articleTitle);
        }

        private bool DetectTypo(string articleText)
        {
            return DetectTypo(articleText, "Test");
        }

        private bool DetectTypo(string articleText, string articleTitle)
        {
            if (Retf == null)
            {
                if (Typos.Count == 0) throw new Exception("You forgot to provide a list of typos!");
                Retf = new RegExTypoFix(false, new TypoList(Typos));
            }

            return Retf.DetectTypo(articleText, articleTitle);
        }

        private void AssertFix(string expected, string articleText, string articleTitle)
        {
            Assert.AreEqual(expected, FixTypos(articleText, articleTitle));
        }

        private void AssertFix(string expected, string articleText)
        {
            Assert.AreEqual(expected, FixTypos(articleText));
        }

        private void AssertNoFix(string articleText, string articleTitle)
        {
            Assert.AreEqual(articleText, FixTypos(articleText, articleTitle));
        }

        private void AssertNoFix(string articleText)
        {
            Assert.AreEqual(articleText, FixTypos(articleText));
        }
        #endregion

        [Test]
        public void SimpleTypos()
        {
            Typos["foo"] = "bar";

            AssertNoFix("");
            AssertNoFix("test");

            AssertFix("the bars are cute", FixTypos("the foos are cute"));
            AssertFix("the bars are cute bars", FixTypos("the foos are cute foos"));
        }
        
        [Test]
        public void NoTypos()
        {
            Typos["foo"] = "bar";
            AssertNoFix("spellfixno foos are");
        }

        [Test]
        public void DetectTypo()
        {
            Typos["foo"] = "bar";
            Assert.IsTrue(DetectTypo("The foo was"));
            Assert.IsTrue(DetectTypo("The foo was "));
            Assert.IsFalse(DetectTypo("The x was"));
            Assert.IsFalse(DetectTypo("The foo {{sic}} was"));
        }

        [Test]
        public void CaseSensitivity()
        {
            Typos["foo"] = "bar";
            Typos["[Bb]oz"] = "quux";
            Typos["(?i:fubar)"] = "fur";

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
        public void DontChangeURLs()
        {
            Typos["foo"] = "bar";
            AssertNoFix("http://foo.com/asdfasdf/asdf.htm");
            AssertNoFix(@"[http://foo.com/asdfasdf/asdf.htm Foo's best]");
            AssertNoFix("http://foo.com/asdfasdf/foo.htm");
        }

        [Test]
        // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_6#Rules_in_Finnish_Regex_TypoFix_list_not_always_applied
        public void Backreferences()
        {
            Typos["(taht)"] = "that";
            Typos[@"\b(a|the)\b\s+\1\b"] = "$1";

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
            Typos["f[oO0]{2}"] = "foo";
            Typos[@"(?<=ten )bar"] = "BAR";

            AssertFix("foo foo", "foo fO0");
            Assert.IsFalse(Summary.Contains("foo → foo"));
            Assert.IsFalse(Summary.Contains("2"));            
            
            AssertFix("ten BAR", "ten bar");
            Assert.IsTrue(Summary.Contains(@"bar → BAR"), "regex with lookbehind such that regex does not match its own match value still generates edit summary based on match value");
            Assert.IsFalse(Summary.Contains("ten"), "for regex with lookbehind, edit summary based on match value without groups outside match value");
        }

        [Test]
        public void FixLinkFaces()
        {
            Typos["foo"] = "bar";

            AssertNoFix("[[foo]]");
            AssertFix("[[Fred|bar]]", "[[Fred|foo]]");
        }

        [Test]
        public void DontChangeWhenWikilinked()
        {
            Typos["foo"] = "bar";

            AssertNoFix("now [[foo]] was foo here"); // No change: typo matches wikilink
            AssertNoFix("now [[foo|bo]] was foo here"); // No change: typo matches piped wikilink
            AssertNoFix("now [[Mr foo oft]] was foo here"); // No change: typo matches within wikilink
            AssertNoFix("now [[Mr foo oft]] was foo here [[Image:foo.png]] a"); // No change: typo matches within wikilink

            AssertFix("now [[Image:foo.png]] was bar here", "now [[Image:foo.png]] was foo here"); // Change: typo matches image name which doesn't matter
            AssertFix("now [[sv:foo]] was bar here", "now [[sv:foo]] was foo here");  // Change: typo matches interwiki name which doesn't matter
            AssertFix("now [[Category:A foo]] was bar here", "now [[Category:A foo]] was foo here");  // Change: typo matches category name which doesn't matter

            AssertFix("now bar [[Foo]] was", "now foo [[Foo]] was");
        }

        [Test]
        public void AllThreeGroups()
        {
            Typos[@"\booze\b"] = "booze!";
            Typos[@"foo(\w*)\b"] = "bar$1";
            Typos[@"\b(a|the)\b\s+\1\b"] = "$1";

            AssertFix("barwards viva the booze!", "foowards viva the the ooze");
        }

        [Test]
        public void DontFixCertainStuff()
        {
            Typos["(T|t)eh"] = "the";

            AssertNoFix(@"{{teh}} http://tehcrappe.org/TehCrappe [[teh]] [http://tehcrappe.org/TehCrappe]");
        }

        [Test]
        public void DontChangeTitle()
        {
            Typos["teh"] = "the";
            Typos["foo"] = "bar";

            AssertFix("teh bar", "teh foo", "teh");
            AssertFix("teh bar", "teh foo", "teh crap");
            AssertNoFix("teh home", "teh");
        }
    }
}