using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using WikiFunctions;
using NUnit.Framework;

namespace UnitTests
{
    /// <summary>
    /// This is the base class for every regex test. It contains helper functions that simplify checks.
    /// </summary>
    public class RegexTestsBase
    {
        public RegexTestsBase()
        {
            Globals.UnitTestMode = true;
            WikiRegexes.MakeLangSpecificRegexes();
        }

        /// <summary>
        /// Checks if a regex matches a string
        /// </summary>
        /// <param name="r">Regex to test</param>
        /// <param name="text">Text to match</param>
        /// <param name="isMatch">If the regex should match the text</param>
        protected void TestMatch(Regex r, string text, bool isMatch)
        {
            Assert.AreEqual(isMatch, r.IsMatch(text));
        }

        /// <summary>
        /// Checks if a regex matches a string
        /// </summary>
        /// <param name="r">Regex to test</param>
        /// <param name="text">Text to match</param>
        protected void TestMatch(Regex r, string text)
        {
            TestMatch(r, text, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <param name="text"></param>
        /// <param name="expectedMatches"></param>
        protected void TestMatches(Regex r, string text, int expectedMatches)
        {
            Assert.AreEqual(expectedMatches, r.Matches(text).Count);
        }

        protected void TestMatch(Match m, params string[] groups)
        {
            Assert.GreaterOrEqual(m.Groups.Count, groups.Length, "Too few groups matched");

            for (int i = 0; i < groups.Length; i++)
            {
                if (groups[i] == null) continue;
                Assert.AreEqual(groups[i], m.Groups[i].Value);
            }
        }

        protected void TestMatch(Regex r, string text, params string[] groups)
        {
            TestMatch(r.Match(text), groups);
        }
    }

    [TestFixture]
    // Don't place tests that require switching to non-default locale here
    public class RegexTests : RegexTestsBase
    {
        [Test]
        public void SimpleWikiLink()
        {
            TestMatch(WikiRegexes.SimpleWikiLink, "[[foo]]", "[[foo]]", "foo");
            TestMatch(WikiRegexes.SimpleWikiLink, "[[foo[]]", "[[foo[]]", "foo[");
            TestMatch(WikiRegexes.SimpleWikiLink, "[[foo bar]]", "[[foo bar]]", "foo bar");
            TestMatches(WikiRegexes.SimpleWikiLink, "[[foo\r\nbar]]", 0);
            TestMatches(WikiRegexes.SimpleWikiLink, "[foo]]", 0);

            TestMatch(WikiRegexes.SimpleWikiLink, "[[foo|bar]]", "[[foo|bar]]", "foo|bar");
            TestMatch(WikiRegexes.SimpleWikiLink, "[[foo]] [[bar]]", "[[foo]]", "foo");
            TestMatches(WikiRegexes.SimpleWikiLink, "[[foo]] [[bar]]", 2);

            TestMatch(WikiRegexes.SimpleWikiLink, "[[foo]]]", "[[foo]]", "foo");
            TestMatch(WikiRegexes.SimpleWikiLink, "[[[foo]]", "[[foo]]", "foo");
        }

        [Test]
        public void WikiLinksOnly()
        {
            TestMatch(WikiRegexes.WikiLinksOnly, "[[foo]]", "[[foo]]");
            TestMatch(WikiRegexes.WikiLinksOnly, "[[foo[]]", "[[foo[]]");
            TestMatch(WikiRegexes.WikiLinksOnly, "[[foo bar]]", "[[foo bar]]");
            TestMatches(WikiRegexes.WikiLinksOnly, "[[foo\r\nbar]]", 0);
            TestMatches(WikiRegexes.WikiLinksOnly, "[foo]]", 0);

            TestMatch(WikiRegexes.WikiLinksOnly, "[[foo|bar]]", "[[foo|bar]]");
            TestMatch(WikiRegexes.WikiLinksOnly, "[[foo]] [[bar]]", "[[foo]]");
            TestMatches(WikiRegexes.WikiLinksOnly, "[[foo]] [[bar]]", 2);

            TestMatch(WikiRegexes.WikiLinksOnly, "[[foo]]]", "[[foo]]");
            TestMatch(WikiRegexes.WikiLinksOnly, "[[[foo]]", "[[foo]]");
        }

        [Test]
        public void Blockquote()
        {
            // one line
            TestMatch(WikiRegexes.Blockquote, "<blockquote>foo bar< /blockquote>", "<blockquote>foo bar< /blockquote>");

            // multiple lines
            TestMatch(WikiRegexes.Blockquote, "< Blockquote >foo\r\nbar</ BLOCKQUOTE>", "< Blockquote >foo\r\nbar</ BLOCKQUOTE>");
        }

        [Test]
        public void Template()
        {
            RegexAssert.Matches("{{foo}}", WikiRegexes.TemplateMultiLine, "{{foo}}");
            RegexAssert.Matches("{{foo}}", WikiRegexes.TemplateMultiLine, "123{{foo}}test");
            RegexAssert.Matches("{{foo|bar}}", WikiRegexes.TemplateMultiLine, "{{foo|bar}}");
            RegexAssert.Matches("{{foo\r\n|bar=test}}", WikiRegexes.TemplateMultiLine, "{{foo\r\n|bar=test}}");

            RegexAssert.Matches("Should match distinct templates", WikiRegexes.TemplateMultiLine, "{{foo}}{{bar}}", "{{foo}}", "{{bar}}");

            RegexAssert.Matches("{{foo| {bar} }}", WikiRegexes.TemplateMultiLine, "{{foo| {bar} }}");
        }

        [Test]
        public void BulletedText()
        {
            RegexAssert.NoMatch(WikiRegexes.BulletedText, "");
            RegexAssert.Matches(WikiRegexes.BulletedText, ":foo", ":foo");
            RegexAssert.Matches(WikiRegexes.BulletedText, ":foo\r\n", ":foo\r");
            RegexAssert.Matches(WikiRegexes.BulletedText, "#foo\r\n*:bar", "#foo\r", "*:bar");
            RegexAssert.Matches(WikiRegexes.BulletedText, "#foo\r\ntest\r\n*:bar", "#foo\r", "*:bar");
            RegexAssert.Matches(WikiRegexes.BulletedText, " foo\r\nfoo bar", " foo\r");
        }

        [Test]
        public void Refs()
        {
            RegexAssert.Matches(WikiRegexes.Refs, "<ref>foo</ref>", "<ref>foo</ref>");

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_6#Typo_fixes_being_applied_to_a_reference_name
            RegexAssert.Matches(WikiRegexes.Refs, "<REF NAME=\"foo\" >bar</ref >", "<REF NAME=\"foo\" >bar</ref >");
            RegexAssert.Matches(WikiRegexes.Refs, "<REF  NAME=foo>bar< /ref>", "<REF  NAME=foo>bar< /ref>");
            //RegexAssert.Matches(WikiRegexes.Refs, "<ref/>", "<ref/>");
            RegexAssert.Matches(WikiRegexes.Refs, "<ReF Name=foo/>", "<ReF Name=foo/>");
            RegexAssert.Matches(WikiRegexes.Refs, "<ReF Name = 'foo'/>", "<ReF Name = 'foo'/>");
            RegexAssert.Matches(WikiRegexes.Refs, "<ReF Name = \"foo\"/>", "<ReF Name = \"foo\"/>");

            RegexAssert.NoMatch(WikiRegexes.Refs, "<refname=foo>bar</ref>");
            RegexAssert.NoMatch(WikiRegexes.Refs, "<refname=foo/>");
        }
    }
}
