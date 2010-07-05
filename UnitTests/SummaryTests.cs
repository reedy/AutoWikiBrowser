using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using WikiFunctions;

namespace UnitTests
{
    [TestFixture]
    public class SummaryTests : RequiresInitialization
    {
        [Test, Ignore("Known Failing")]
        public void IsCorrect()
        {
            // too long
            StringBuilder sb = new StringBuilder(300);
            for (int i = 0; i < 256; i++) sb.Append('x');
            Assert.IsFalse(Summary.IsCorrect(sb.ToString()));

            // no wikilinks
            Assert.IsTrue(Summary.IsCorrect(""));
            Assert.IsTrue(Summary.IsCorrect("test"));
            Assert.IsTrue(Summary.IsCorrect("["));
            Assert.IsTrue(Summary.IsCorrect("]"));
            Assert.IsTrue(Summary.IsCorrect("[test]"));
            Assert.IsTrue(Summary.IsCorrect("[test]]"));
            Assert.IsTrue(Summary.IsCorrect("[[]]"));

            // correctly (sort of..) terminated wikilinks
            Assert.IsTrue(Summary.IsCorrect("[[test]]"));
            Assert.IsTrue(Summary.IsCorrect("[[test]] [[foo]]"));
            Assert.IsTrue(Summary.IsCorrect("[[foo[[]]]"));

            //broken wikilinks, should be found to be invalid
            Assert.IsFalse(Summary.IsCorrect("[["));
            Assert.IsFalse(Summary.IsCorrect("[[["));
            Assert.IsFalse(Summary.IsCorrect("[[test]"));
            Assert.IsFalse(Summary.IsCorrect("[[test]] [["));

            Assert.IsFalse(Summary.IsCorrect("[[123456789 123456789 123456789 1[[WP:AWB]]"));
        }


        [Test]
        public void Trim()
        {
            Assert.AreEqual("test", Summary.Trim("test"));

            const string bug1 = @"replaced category 'Actual event ballads' → 'Songs based on actual events' per [[Wikipedia:Categories for discussion/Log/2009 November 6|CfD 2009 Nov 6]]";
            const string waffle = @"some waffle here to make the edit summary too long";

            Assert.AreEqual(bug1, Summary.Trim(bug1));
            Assert.AreEqual(waffle + bug1, Summary.Trim(waffle + bug1));
            Assert.AreEqual(waffle + waffle + @"replaced category 'Actual event ballads' → 'Songs based on actual events' per...", Summary.Trim(waffle + waffle + bug1));

            #if DEBUG
            // debug only - length affected by revision number being in edit summary for debug builds
            Assert.AreEqual("clean upclean upclean up, typos fixed: dissapointment → disappointment (2), attatched → attached (2), begining → beginning, Expiditionary → Expeditionary, manuever → maneuver (5), thier → their (7), indep",
                            Summary.Trim(
                                "clean upclean upclean up, typos fixed: dissapointment → disappointment (2), attatched → attached (2), begining → beginning, Expiditionary → Expeditionary, manuever → maneuver (5), thier → their (7), independant → independent"));
            #endif
        }

        [Test]
        public void ModifiedSection()
        {
            Assert.AreEqual("", Summary.ModifiedSection("", ""));
            Assert.AreEqual("", Summary.ModifiedSection("foo", "foo"));
            Assert.AreEqual("", Summary.ModifiedSection("foo", "bar"));

            Assert.AreEqual("foo", Summary.ModifiedSection("==foo==\r\n123", "==foo==\r\ntest"));
            Assert.AreEqual("foo", Summary.ModifiedSection("rawr\r\n==foo==\r\n123", "rawr\r\n==foo==\r\ntest"));
            Assert.AreEqual("foo", Summary.ModifiedSection("==foo==\r\n123", "== foo ==\r\ntest"));
            Assert.AreEqual("bar", Summary.ModifiedSection("==foo==\r\n123", "==bar==\r\ntest"));

            // http://en.wikipedia.org/w/index.php?diff=prev&oldid=338360216
            Assert.AreEqual("", Summary.ModifiedSection("==foo==\r\nbar", "test\r\n==foo==\r\nbar"));
        }
    }
}
