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
        /// <summary>
        /// Checks if a regex matches a string
        /// </summary>
        /// <param name="r">Regex to test</param>
        /// <param name="text">Text to match</param>
        /// <param name="isMatch">If the regex should match the text</param>
        protected static void TestMatch(Regex r, string text, bool isMatch)
        {
            Assert.AreEqual(isMatch, r.IsMatch(text));
        }

        /// <summary>
        /// Checks if a regex matches a string
        /// </summary>
        /// <param name="r">Regex to test</param>
        /// <param name="text">Text to match</param>
        protected static void TestMatch(Regex r, string text)
        {
            TestMatch(r, text, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <param name="text"></param>
        /// <param name="expectedMatches"></param>
        protected static void TestMatches(Regex r, string text, int expectedMatches)
        {
            Assert.AreEqual(expectedMatches, r.Matches(text).Count);
        }

        protected static void TestMatch(Match m, params string[] groups)
        {
            Assert.GreaterOrEqual(m.Groups.Count, groups.Length, "Too few groups matched");

            for (int i = 0; i < groups.Length; i++)
            {
                if (groups[i] == null) continue;
                Assert.AreEqual(groups[i], m.Groups[i].Value);
            }
        }

        protected static void TestMatch(Regex r, string text, params string[] groups)
        {
            TestMatch(r.Match(text), groups);
        }
    }

    [TestFixture]
    // Don't place tests that require switching to non-default locale here
    public class RegexTests : RegexTestsBase
    {
        [Test]
        public void GenerateNamespaceRegex()
        {
            Assert.AreEqual("", WikiRegexes.GenerateNamespaceRegex(0));

            Assert.AreEqual("User", WikiRegexes.GenerateNamespaceRegex(Namespace.User));
            Assert.AreEqual("User[ _]talk", WikiRegexes.GenerateNamespaceRegex(Namespace.UserTalk));
            Assert.AreEqual("File|Image", WikiRegexes.GenerateNamespaceRegex(Namespace.File));

            Assert.AreEqual("Wikipedia|Project", WikiRegexes.GenerateNamespaceRegex(Namespace.Project));

            Assert.AreEqual("Media|File|Image",
                WikiRegexes.GenerateNamespaceRegex(Namespace.Media, Namespace.File));
        }

        [Test]
        public void SimpleWikiLink()
        {
            TestMatch(WikiRegexes.SimpleWikiLink, "[[foo]]", "[[foo]]", "foo");
            TestMatches(WikiRegexes.SimpleWikiLink, "[[foo[]]", 0);
            TestMatch(WikiRegexes.SimpleWikiLink, "[[foo bar]]", "[[foo bar]]", "foo bar");
            TestMatches(WikiRegexes.SimpleWikiLink, "[[foo\r\nbar]]", 0);
            TestMatches(WikiRegexes.SimpleWikiLink, "[foo]]", 0);

            TestMatch(WikiRegexes.SimpleWikiLink, "[[foo|bar]]", "[[foo|bar]]", "foo|bar");
            TestMatch(WikiRegexes.SimpleWikiLink, "[[foo]] [[bar]]", "[[foo]]", "foo");
            TestMatches(WikiRegexes.SimpleWikiLink, "[[foo]] [[bar]]", 2);

            TestMatch(WikiRegexes.SimpleWikiLink, "[[foo]]]", "[[foo]]", "foo");


            TestMatch(WikiRegexes.SimpleWikiLink, "[[foo [[bar]] here]]", "[[foo [[bar]] here]]", "foo [[bar]] here");
            TestMatch(WikiRegexes.SimpleWikiLink, "[[[foo]]", "[[foo]]", "foo");
        }

        [Test]
        public void WikiLinksOnly()
        {
            TestMatch(WikiRegexes.WikiLinksOnly, "[[foo]]", "[[foo]]");
            TestMatch(WikiRegexes.WikiLinksOnly, "[[foo bar]]", "[[foo bar]]");
            TestMatches(WikiRegexes.WikiLinksOnly, "[[foo\r\nbar]]", 0);
            TestMatches(WikiRegexes.WikiLinksOnly, "[foo]]", 0);

            TestMatch(WikiRegexes.WikiLinksOnly, "[[foo|bar]]", "[[foo|bar]]");
            TestMatch(WikiRegexes.WikiLinksOnly, "[[foo]] [[bar]]", "[[foo]]");
            TestMatches(WikiRegexes.WikiLinksOnly, "[[foo]] [[bar]]", 2);

            TestMatch(WikiRegexes.WikiLinksOnly, "[[foo]]]", "[[foo]]");
            TestMatch(WikiRegexes.WikiLinksOnly, "[[[foo]]", "[[foo]]");

            TestMatches(WikiRegexes.WikiLinksOnly, "[[foo[]]", 0);
            TestMatch(WikiRegexes.WikiLinksOnly, "[[foo [[bar]] here]", "[[bar]]");
        }

        [Test]
        public void PipedWikiLink()
        {
            TestMatch(WikiRegexes.PipedWikiLink, "[[foo|bar]]");
            TestMatch(WikiRegexes.PipedWikiLink, "a [[foo boo | bar bar ]] !one", "[[foo boo | bar bar ]]");
            TestMatch(WikiRegexes.PipedWikiLink, "[[foo]]", false);

            // shouldn't eat too much
            TestMatch(WikiRegexes.PipedWikiLink, "[[foo]] [[foo|bar]]", "[[foo|bar]]");
            TestMatch(WikiRegexes.PipedWikiLink, "[[foo|bar]] [[foo]]", "[[foo|bar]]");
            
            TestMatch(WikiRegexes.PipedWikiLink, "[[foo|\r\nbar]]", false);
            TestMatch(WikiRegexes.PipedWikiLink, "[[foo\r\n|bar]]", false);
            TestMatch(WikiRegexes.PipedWikiLink, "[[foo]] | bar]]", false);
            TestMatch(WikiRegexes.PipedWikiLink, "[[foo | [[bar]]", false);
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
        public void Poem()
        {
            // one line
            TestMatch(WikiRegexes.Poem, "<poem>foo bar< /poem>", "<poem>foo bar< /poem>");

            // multiple lines
            TestMatch(WikiRegexes.Poem, @"< Poem >foo
bar</ POEM>", @"< Poem >foo
bar</ POEM>");
        }

        [Test]
        public void Template()
        {
            RegexAssert.Matches("{{foo}}", WikiRegexes.TemplateMultiLine, "{{foo}}");
            RegexAssert.Matches("{{foo}}", WikiRegexes.TemplateMultiLine, "123{{foo}}test");
            RegexAssert.Matches("{{foo|bar}}", WikiRegexes.TemplateMultiLine, "{{foo|bar}}");
            RegexAssert.Matches("{{foo\r\n|bar=test}}", WikiRegexes.TemplateMultiLine, "{{foo\r\n|bar=test}}");

            RegexAssert.Matches("Should match distinct templates", WikiRegexes.TemplateMultiLine, "{{foo}}{{bar}}", "{{foo}}", "{{bar}}");

            // regex won't match if nested template or curly-bracketed stuff
            RegexAssert.NoMatch(WikiRegexes.TemplateMultiLine, "{{foo| {bar} }}");
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

        [Test, Category("Unarchived bugs")]
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

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs#.3Cp.3E_deletion_in_references_and_notes
            RegexAssert.Matches(WikiRegexes.Refs, "<ref>foo<!-- bar --></ref>", "<ref>foo<!-- bar --></ref>");
            // shouldn't eat too much
            RegexAssert.Matches(WikiRegexes.Refs, "<ref>foo<!-- bar --></ref> <ref>foo</ref>", "<ref>foo<!-- bar --></ref>", "<ref>foo</ref>");
        }

        [Test]
        ///
        public void PossibleInterwikis()
        {
            RegexAssert.Matches(WikiRegexes.PossibleInterwikis, "foo[[en:bar]]", "[[en:bar]]");
            RegexAssert.Matches(WikiRegexes.PossibleInterwikis, "[[en:bar]][[ru:", "[[en:bar]]");
            RegexAssert.Matches(WikiRegexes.PossibleInterwikis, "foo[[en:bar:quux]][[ru:boz test]]", "[[en:bar:quux]]", "[[ru:boz test]]");

            RegexAssert.NoMatch(WikiRegexes.PossibleInterwikis, "[[:en:foo]]");
            RegexAssert.NoMatch(WikiRegexes.PossibleInterwikis, "[[:foo]]");
        }

        [Test]
        public void UntemplatedQuotes()
        {
            // RegexAssert doesn't seem to work, so do tests this way

            // be careful about condensing any of these unit tests, as some of the different quote characters *look* the same, but in fact are different Unicode characters

           Assert.IsFalse(WikiRegexes.UntemplatedQuotes.Replace(@" ""very fast"" ", "1").Contains(@"""very fast"""));
           Assert.IsFalse(WikiRegexes.UntemplatedQuotes.Replace(@" « very fast » ", "1").Contains(@"« very fast »"));
           Assert.IsFalse(WikiRegexes.UntemplatedQuotes.Replace(@" ‘very fast‘ ", "1").Contains(@"‘very fast‘"));
           Assert.IsFalse(WikiRegexes.UntemplatedQuotes.Replace(@" ’ very fast ’ ", "").Contains(@"’ very fast ’"));
           Assert.IsFalse(WikiRegexes.UntemplatedQuotes.Replace(@" “ very fast “ ", "").Contains(@"“ very fast “"));
           Assert.IsFalse(WikiRegexes.UntemplatedQuotes.Replace(@" ” very fast ” ", "").Contains(@"” very fast ”"));
           Assert.IsFalse(WikiRegexes.UntemplatedQuotes.Replace(@" ‛ very fast ‛ ", "").Contains(@"‛ very fast ‛"));
           Assert.IsFalse(WikiRegexes.UntemplatedQuotes.Replace(@" ‟ very fast‟ ", " ").Contains(@"‟ very fast‟"));
           Assert.IsFalse(WikiRegexes.UntemplatedQuotes.Replace(@" ‹ very fast › ", "").Contains(@"‹ very fast ›"));
           Assert.IsFalse(WikiRegexes.UntemplatedQuotes.Replace(@" “ very fast ” ", "").Contains(@"“ very fast ”"));
           Assert.IsFalse(WikiRegexes.UntemplatedQuotes.Replace(@" „ very fast „ ", "").Contains(@"„ very fast „"));
           Assert.IsFalse(WikiRegexes.UntemplatedQuotes.Replace(@" „ very fast „ ", "").Contains(@"„ very fast „"));
           Assert.IsFalse(WikiRegexes.UntemplatedQuotes.Replace(@" ‘ very fast ‘ ", "").Contains(@"‘ very fast ‘"));
           Assert.IsFalse(WikiRegexes.UntemplatedQuotes.Replace(@" ’ very fast ’ ", "").Contains(@"’ very fast ’"));
           Assert.IsFalse(WikiRegexes.UntemplatedQuotes.Replace(@" “ very fast ” ", "").Contains(@"“ very fast ”"));
           Assert.IsFalse(WikiRegexes.UntemplatedQuotes.Replace(@" ` very fast ` ", "").Contains(@"` very fast `"));
           Assert.IsFalse(WikiRegexes.UntemplatedQuotes.Replace(@" ’ very fast ‘ ", "").Contains(@"’ very fast ‘"));

           Assert.IsTrue(WikiRegexes.UntemplatedQuotes.Replace(@" “ very fast ” but not pretty ", "").Contains("but not pretty"));
           Assert.IsTrue(WikiRegexes.UntemplatedQuotes.Replace(@" “ very fast ” but not pretty“ ", "").Contains("but not pretty“"));
           Assert.IsTrue(WikiRegexes.UntemplatedQuotes.Replace(@" “ very fast ” and “ very well ” ", "").Contains(" and "));
           Assert.IsTrue(WikiRegexes.UntemplatedQuotes.Replace(@"not pretty but “ very fast ” ", "").Contains("not pretty but "));

           // don't match single quotes, no quotes, quotes over two paragraphs
           Assert.IsTrue(WikiRegexes.UntemplatedQuotes.Replace(@" very fast ", "").Contains(@"very fast"));
           Assert.IsTrue(WikiRegexes.UntemplatedQuotes.Replace(@" 'very fast' ", "").Contains(@"'very fast'"));
           Assert.IsTrue(WikiRegexes.UntemplatedQuotes.Replace(@" ''very fast'' ", "").Contains(@"''very fast''"));
           Assert.IsTrue(WikiRegexes.UntemplatedQuotes.Replace(@" ''very fast'' ", "").Contains(@"''very fast''"));
           Assert.IsTrue(WikiRegexes.UntemplatedQuotes.Replace(@" ,very fast, ", "").Contains(@",very fast,")); // commas
           Assert.IsTrue(WikiRegexes.UntemplatedQuotes.Replace(@" „very 
fast„", "@@").Contains(@" „very 
fast„"));
            // don't match apostrophes when used within words
           Assert.IsTrue(WikiRegexes.UntemplatedQuotes.Replace(@"Now it’s a shame as it’s a", "").Contains(@"Now it’s a shame as it’s a"));
        }

        [Test]
        public void SicTagTests()
        {
            Assert.IsTrue(WikiRegexes.SicTag.IsMatch("now helo [sic] there"));
            Assert.IsTrue(WikiRegexes.SicTag.IsMatch("now helo[sic] there"));
            Assert.IsTrue(WikiRegexes.SicTag.IsMatch("now helo (sic) there"));
            Assert.IsTrue(WikiRegexes.SicTag.IsMatch("now helo {sic} there"));
            Assert.IsTrue(WikiRegexes.SicTag.IsMatch("now helo [Sic] there"));
            Assert.IsTrue(WikiRegexes.SicTag.IsMatch("now {{sic|helo}} there"));
            Assert.IsTrue(WikiRegexes.SicTag.IsMatch("now {{sic|hel|o}} there"));
            Assert.IsTrue(WikiRegexes.SicTag.IsMatch("now {{typo|helo}} there"));

            Assert.IsFalse(WikiRegexes.SicTag.IsMatch("now sickened by"));
            Assert.IsFalse(WikiRegexes.SicTag.IsMatch("now helo sic there"));
        }

        [Test]
        public void MoreNoFootnotesTests()
        {
            Assert.IsTrue(WikiRegexes.MoreNoFootnotes.IsMatch(@"{{morefootnotes}}"));
            Assert.IsTrue(WikiRegexes.MoreNoFootnotes.IsMatch(@"{{Morefootnotes}}"));
            Assert.IsTrue(WikiRegexes.MoreNoFootnotes.IsMatch(@"{{Nofootnotes}}"));
            Assert.IsTrue(WikiRegexes.MoreNoFootnotes.IsMatch(@"{{nofootnotes}}"));

            Assert.IsFalse(WikiRegexes.MoreNoFootnotes.IsMatch(@"{{NOFOOTNOTES}}"));
            Assert.IsFalse(WikiRegexes.MoreNoFootnotes.IsMatch(@"{{no footnotes}}"));
        }

        [Test]
        public void ReferencesTemplateTests()
        {
            Assert.IsTrue(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello<ref>Fred</ref> {{reflist}}"));
            Assert.IsTrue(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello<ref>Fred</ref> {{ref-list}}"));
            Assert.IsTrue(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello<ref>Fred</ref> {{reflink}}"));
            Assert.IsTrue(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello<ref>Fred</ref> {{references}}"));
            Assert.IsTrue(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello<ref>Fred</ref> <references/>"));

            Assert.IsFalse(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello<ref>Fred</ref>"));
            Assert.IsFalse(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello<ref name=""F"">Fred</ref>"));
            Assert.IsFalse(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello world"));
        }

        [Test]
        public void DablinksTests()
        {
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{For|Fred the dancer|Fred (dancer)}}"));
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{for|Fred the dancer|Fred (dancer)}}"));
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{otherpeople1|Fred the dancer|Fred Smith (dancer)}}"));
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{For|Fred the dancer|Fred Smith (dancer)}}"));
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{redirect2|Fred the dancer|Fred Smith (dancer)}}"));
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{For
    |Fred the dancer
    |Fred (dancer)}}"));

            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{For|
    Fred the dancer|
    Fred (dancer)}}"));

            Assert.IsFalse(WikiRegexes.Dablinks.IsMatch(@"{{For fake template|Fred the dancer|Fred(dancer)}}"));
            Assert.IsFalse(WikiRegexes.Dablinks.IsMatch(@"{{REDIRECT2|Fred the dancer|Fred Smith (dancer)}}"));
        }

        [Test]
        public void OrphanArticleIssuesTests()
        {
            Assert.IsTrue(WikiRegexes.OrphanArticleIssues.IsMatch(@"{{Article issues|orphan=May 2008|cleanup=May 2008|story=May 2008}}"));
            Assert.IsTrue(WikiRegexes.OrphanArticleIssues.IsMatch(@"{{Articleissues|orphan=May 2008|cleanup=May 2008|story=May 2008}}"));
            Assert.IsTrue(WikiRegexes.OrphanArticleIssues.IsMatch(@"{{ Articleissues|orphan=May 2008|cleanup=May 2008|story=May 2008}}"));
            Assert.IsTrue(WikiRegexes.OrphanArticleIssues.IsMatch(@"{{Article issues
            |orphan=May 2008|cleanup=May 2008|story=May 2008}}"));
            Assert.IsTrue(WikiRegexes.OrphanArticleIssues.IsMatch(@"{{Article issues|orphan = May 2008|cleanup=May 2008|story=May 2008}}"));
            Assert.IsTrue(WikiRegexes.OrphanArticleIssues.IsMatch(@"{{Article issues|Orphan=May 2008|cleanup=May 2008|story=May 2008}}"));
            Assert.IsTrue(WikiRegexes.OrphanArticleIssues.IsMatch(@"{{Article issues| orphan |cleanup=May 2008|story=May 2008}}"));
            Assert.IsFalse(WikiRegexes.OrphanArticleIssues.IsMatch(@"{{Article issues|cleanup=May 2008|story=May 2008}}"));
        }

        [Test]
        public void InfoboxTests()
        {
            Assert.IsTrue(WikiRegexes.Infobox.IsMatch(@" {{Infobox hello| bye}} "));
            Assert.IsTrue(WikiRegexes.Infobox.IsMatch(@" {{infobox hello| bye}} "));
            Assert.IsTrue(WikiRegexes.Infobox.IsMatch(@" {{infobox hello| bye {{a}} was}} "));
            Assert.IsTrue(WikiRegexes.Infobox.IsMatch(@" {{infobox hello
| bye}} "));
            Assert.IsTrue(WikiRegexes.Infobox.IsMatch(@" {{some infobox| hello| bye}} "));
            Assert.IsTrue(WikiRegexes.Infobox.IsMatch(@" {{Some Infobox| hello| bye}} "));
        }
    }
}
