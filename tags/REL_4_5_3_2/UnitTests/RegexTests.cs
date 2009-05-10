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
        public void UnformattedTextTests()
        {
            Assert.IsTrue(WikiRegexes.UnFormattedText.IsMatch(@"<pre>{{abc}}</pre>"));
            Assert.IsTrue(WikiRegexes.UnFormattedText.IsMatch(@"<math>{{abc}}</math>"));
            Assert.IsTrue(WikiRegexes.UnFormattedText.IsMatch(@"<nowiki>{{abc}}</nowiki>"));
            Assert.IsTrue(WikiRegexes.UnFormattedText.IsMatch(@"now hello {{bye}} <pre>{now}}</pre>"));
        }

        [Test]
        public void MathPreSourceCodeTests()
        {
            Assert.IsTrue(WikiRegexes.MathPreSourceCodeComments.IsMatch(@"<pre>{{abc}}</pre>"));
            Assert.IsTrue(WikiRegexes.MathPreSourceCodeComments.IsMatch(@"<!--{{abc}}-->"));
            Assert.IsTrue(WikiRegexes.MathPreSourceCodeComments.IsMatch(@"<code>{{abc}}</code>"));
            Assert.IsTrue(WikiRegexes.MathPreSourceCodeComments.IsMatch(@"<source lang=xml>{{abc}}</source>"));
            Assert.IsTrue(WikiRegexes.MathPreSourceCodeComments.IsMatch(@"<source>{{abc}}</source>"));
            Assert.IsTrue(WikiRegexes.MathPreSourceCodeComments.IsMatch(@"now hello {{bye}} <pre>{now}}</pre>"));
            Assert.IsTrue(WikiRegexes.MathPreSourceCodeComments.IsMatch(@"<math>{{abc}}</math>"));

            Assert.IsFalse(WikiRegexes.MathPreSourceCodeComments.IsMatch(@"<nowiki>{{abc}}</nowiki>"));
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
        public void WikiLinkTests()
        {
            Assert.AreEqual(WikiRegexes.WikiLink.Match(@"[[foo]]").Groups[1].Value, @"foo");
            Assert.AreEqual(WikiRegexes.WikiLink.Match(@"[[foo|bar]]").Groups[1].Value, @"foo");
            Assert.AreEqual(WikiRegexes.WikiLink.Match(@"[[foo bar]]").Groups[1].Value, @"foo bar");
            Assert.AreEqual(WikiRegexes.WikiLink.Match(@"[[Foo]]").Groups[1].Value, @"Foo");
            Assert.AreEqual(WikiRegexes.WikiLink.Match(@"[[foo bar|word here]]").Groups[1].Value, @"foo bar");
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
            Assert.IsFalse(WikiRegexes.UntemplatedQuotes.Replace(@" „very 
fast„ ", "").Contains(@" „very 
fast„ "));

            Assert.IsTrue(WikiRegexes.UntemplatedQuotes.Replace(@" “ very fast ” but not pretty ", "").Contains("but not pretty"));
            Assert.IsTrue(WikiRegexes.UntemplatedQuotes.Replace(@" “ very fast ” but not pretty“ ", "").Contains("but not pretty“"));
            Assert.IsTrue(WikiRegexes.UntemplatedQuotes.Replace(@" “ very fast ” and “ very well ” ", "").Contains(" and "));
            Assert.IsTrue(WikiRegexes.UntemplatedQuotes.Replace(@"not pretty but “ very fast ” ", "").Contains("not pretty but "));

            // don't match single quotes, no quotes
            Assert.IsTrue(WikiRegexes.UntemplatedQuotes.Replace(@" very fast ", "").Contains(@"very fast"));
            Assert.IsTrue(WikiRegexes.UntemplatedQuotes.Replace(@" 'very fast' ", "").Contains(@"'very fast'"));
            Assert.IsTrue(WikiRegexes.UntemplatedQuotes.Replace(@" ''very fast'' ", "").Contains(@"''very fast''"));
            Assert.IsTrue(WikiRegexes.UntemplatedQuotes.Replace(@" ''very fast'' ", "").Contains(@"''very fast''"));
            Assert.IsTrue(WikiRegexes.UntemplatedQuotes.Replace(@" ,very fast, ", "").Contains(@",very fast,")); // commas

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
            Assert.IsTrue(WikiRegexes.MoreNoFootnotes.IsMatch(@"{{More footnotes}}"));
            Assert.IsTrue(WikiRegexes.MoreNoFootnotes.IsMatch(@"{{Nofootnotes}}"));
            Assert.IsTrue(WikiRegexes.MoreNoFootnotes.IsMatch(@"{{No footnotes}}"));
            Assert.IsTrue(WikiRegexes.MoreNoFootnotes.IsMatch(@"{{nofootnotes}}"));
            Assert.IsTrue(WikiRegexes.MoreNoFootnotes.IsMatch(@"{{nofootnotes|section}}"));

            Assert.IsFalse(WikiRegexes.MoreNoFootnotes.IsMatch(@"{{NOFOOTNOTES}}"));
        }

        [Test]
        public void ReferencesTemplateTests()
        {
            Assert.IsTrue(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello<ref>Fred</ref> {{reflist}}"));
            Assert.IsTrue(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello<ref>Fred</ref> {{Reflist}}"));
            Assert.IsTrue(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello<ref>Fred</ref> {{ref-list}}"));
            Assert.IsTrue(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello<ref>Fred</ref> {{Reflink}}"));
            Assert.IsTrue(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello<ref>Fred</ref> {{reflink}}"));
            Assert.IsTrue(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello<ref>Fred</ref> {{References}}"));
            Assert.IsTrue(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello<ref>Fred</ref> {{references}}"));
            Assert.IsTrue(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello<ref>Fred</ref> <references/>"));

            Assert.IsFalse(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello<ref>Fred</ref>"));
            Assert.IsFalse(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello<ref name=""F"">Fred</ref>"));
            Assert.IsFalse(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello world"));
        }

        [Test]
        public void RefAfterReflist()
        {
            Assert.IsTrue(WikiRegexes.RefAfterReflist.IsMatch(@"blah <ref>a</ref> ==references== {{reflist}} <ref>b</ref>"));
            Assert.IsTrue(WikiRegexes.RefAfterReflist.IsMatch(@"blah <ref>a</ref> ==references== <references/> <ref>b</ref>"));
            Assert.IsTrue(WikiRegexes.RefAfterReflist.IsMatch(@"blah <ref>a</ref> ==references== {{Reflist}} <ref>b</ref>"));
            Assert.IsTrue(WikiRegexes.RefAfterReflist.IsMatch(@"blah <ref>a</ref> ==references== {{reflink}} <ref>b</ref>"));
            Assert.IsTrue(WikiRegexes.RefAfterReflist.IsMatch(@"blah <ref>a</ref> ==references== {{ref-list}} <ref>b</ref>"));
            Assert.IsTrue(WikiRegexes.RefAfterReflist.IsMatch(@"blah <ref>a</ref> 
==references== {{reflist}} <ref>b</ref>"));
            Assert.IsTrue(WikiRegexes.RefAfterReflist.IsMatch(@"blah <ref>a</ref> ==references== {{reflist}} blah
<ref>b</ref>"));
            Assert.IsTrue(WikiRegexes.RefAfterReflist.IsMatch(@"blah <ref>a</ref> ==references== {{reflist}} <ref name=""b"">b</ref>"));

            // {{GR}} with argument as simple decimal provides embedded <ref></ref>
            Assert.IsTrue(WikiRegexes.RefAfterReflist.IsMatch(@"blah <ref>a</ref> ==references== {{reflist}} {{GR|4}}"));
            Assert.IsFalse(WikiRegexes.RefAfterReflist.IsMatch(@"blah <ref>a</ref> ==references== {{reflist}} {{GR|r4}}"));
            Assert.IsFalse(WikiRegexes.RefAfterReflist.IsMatch(@"blah <ref>a</ref> ==references== {{reflist}} {{GR|India}}"));

            // this is correct syntax
            Assert.IsFalse(WikiRegexes.RefAfterReflist.IsMatch(@"blah <ref>a</ref> ==references== {{reflist}}"));
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

            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{Otheruse|something}}"));
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{Otheruses2|something}}"));
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{otheruse|something}}"));
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{otheruse
|something}}"));
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{2otheruses|something}}"));

            Assert.IsFalse(WikiRegexes.Dablinks.IsMatch(@"{{For fake template|Fred the dancer|Fred(dancer)}}"));
            Assert.IsFalse(WikiRegexes.Dablinks.IsMatch(@"{{REDIRECT2|Fred the dancer|Fred Smith (dancer)}}"));
            Assert.IsFalse(WikiRegexes.Dablinks.IsMatch(@"{{Otheruse2|something}}")); //non-existent
        }
        
        [Test]
        public void PortalTemplateTests()
        {
          Assert.IsTrue(WikiRegexes.PortalTemplate.IsMatch(@"{{portal}}"));
          Assert.IsTrue(WikiRegexes.PortalTemplate.IsMatch(@"{{Portal}}"));
          Assert.IsTrue(WikiRegexes.PortalTemplate.IsMatch(@"{{portal|Science}}"));
          Assert.IsTrue(WikiRegexes.PortalTemplate.IsMatch(@"{{portal|Spaceflight|RocketSunIcon.svg|break=yes}}"));
          
          Assert.IsFalse(WikiRegexes.PortalTemplate.IsMatch(@"{{PORTAL}}"));
          Assert.IsFalse(WikiRegexes.PortalTemplate.IsMatch(@"{{portalos}}"));
          Assert.IsFalse(WikiRegexes.PortalTemplate.IsMatch(@"{{Spanish portal|game}}"));
          Assert.IsFalse(WikiRegexes.PortalTemplate.IsMatch(@"{{portal|Bert|{{here}}}}"));
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
            Assert.IsTrue(WikiRegexes.Infobox.IsMatch(@" {{Infobox_play| bye}} "));
            Assert.IsTrue(WikiRegexes.Infobox.IsMatch(@" {{some infobox| hello| bye}} "));
            Assert.IsTrue(WikiRegexes.Infobox.IsMatch(@" {{Some Infobox| hello| bye}} "));
        }

        [Test]
        public void WikifyTests()
        {
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(@"{{wikify}}"));
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(@"{{wikify|date=March 2009}}"));
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(@"{{Wikify}}"));
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(@"{{Wikify|date=March 2009}}"));
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(@"{{wikify|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"));
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(@"{{Article issues|wikify=May 2008|a=b|c=d}}"));
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(@"{{Articleissues|wikify=May 2008|a=b|c=d}}"));
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(@"{{articleissues|wikify=May 2008|a=b|c=d}}"));
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(@"{{article issues|wikify=May 2008|a=b|c=d}}"));
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(@"{{Article issues|a=b|c=d| wikify = May 2008|a=b|c=d}}"));

            // don't remove the whole of an {{article issues}} template if removing wikify tag
            Assert.IsTrue(WikiRegexes.Wikify.Replace(@"{{Article issues|a=b|c=d| wikify = May 2008|a=b|c=d}}", "").Contains(@"{{Article issues|a=b|c=d|"));

            Assert.IsFalse(WikiRegexes.Wikify.IsMatch(@"{{wikifyworldblah}}"));
        }

        [Test]
        public void ExpandTests()
        {
            Assert.IsTrue(WikiRegexes.Expand.IsMatch(@"now {{expand}} here"));
            Assert.IsTrue(WikiRegexes.Expand.IsMatch(@"now {{Expand}} here"));
            Assert.IsTrue(WikiRegexes.Expand.IsMatch(@"now {{Expand|date=May 2009}} here"));
            Assert.IsTrue(WikiRegexes.Expand.IsMatch(@"now {{expandarticle}} here"));
            Assert.IsTrue(WikiRegexes.Expand.IsMatch(@"now {{Expandarticle}} here"));
            Assert.IsTrue(WikiRegexes.Expand.IsMatch(@"now {{expand-article}} here"));
            Assert.IsTrue(WikiRegexes.Expand.IsMatch(@"now {{Expand-article}} here"));
            Assert.IsTrue(WikiRegexes.Expand.IsMatch(@"now {{expansion}} here"));
            Assert.IsTrue(WikiRegexes.Expand.IsMatch(@"now {{Expansion|date=subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}} here"));
            Assert.IsTrue(WikiRegexes.Expand.IsMatch(@"now {{Develop}} here"));
            Assert.IsTrue(WikiRegexes.Expand.IsMatch(@"now {{develop}} here"));

            Assert.IsFalse(WikiRegexes.Expand.IsMatch(@"now {{developers}} here"));

            Assert.AreEqual(WikiRegexes.Expand.Replace(@"now {{expand}} here", ""), @"now  here");
            Assert.AreEqual(WikiRegexes.Expand.Replace(@"now {{expandarticle}} here", ""), @"now  here");
            Assert.AreEqual(WikiRegexes.Expand.Replace(@"{{article issues|wikify=May 2009|COI=March 2009|expand=May 2008}}", ""), @"{{article issues|wikify=May 2009|COI=March 2009}}");
            Assert.AreEqual(WikiRegexes.Expand.Replace(@"{{article issues|wikify=May 2009| expand = May 2008|COI=March 2009}}", ""), @"{{article issues|wikify=May 2009|COI=March 2009}}");
        }

        [Test]
        public void OrphanTests()
        {
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(@"{{orphan}}"));
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(@"{{Orphan}}"));
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(@"{{orphan|date=May 2008}}"));
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(@"{{Orphan| date = May 2008}}"));
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(@"{{orphan|date=subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"));
            
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(@"{{orphanblahblah}}"));
        }

        [Test]
        public void PstylesTests()
        {
            Assert.IsTrue(WikiRegexes.Pstyles.IsMatch(@"<p style=""margin:0px;font-size:100%""><span style=""color:#00ff00"">▪</span> <small>Francophone minorities</small></p>"));
            Assert.IsTrue(WikiRegexes.Pstyles.IsMatch(@"<p style=""font-family:monospace; line-height:130%"">hello</p>"));
            Assert.IsTrue(WikiRegexes.Pstyles.IsMatch(@"<p style=""font-family:monospace; line-height:130%"">hello</P>"));
            Assert.IsTrue(WikiRegexes.Pstyles.IsMatch(@"<p style=""font-family:monospace; line-height:130%"">hello</P>"));
            Assert.IsTrue(WikiRegexes.Pstyles.IsMatch(@"<p style = ""font-family:monospace; line-height:130%"">
hello</P>"));

        }

        [Test]
        public void FirstSection()
        {
            Assert.IsTrue(WikiRegexes.ZerothSection.IsMatch(@"article"));
            Assert.IsTrue(WikiRegexes.ZerothSection.IsMatch(@"article ==heading=="));
            Assert.IsTrue(WikiRegexes.ZerothSection.IsMatch(@"article ===heading==="));
            Assert.AreEqual("==heading==", WikiRegexes.ZerothSection.Replace(@"article ==heading==", ""));
            Assert.AreEqual(@"==heading== words ==another heading==", WikiRegexes.ZerothSection.Replace(@"{{wikify}}
{{Infobox hello | bye=yes}}
article words, '''bold''' blah.
==heading== words ==another heading==", ""));

            Assert.IsFalse(WikiRegexes.ZerothSection.IsMatch(@""));
        }

        [Test]
        public void HeadingLevelTwoTests()
        {
            Assert.IsTrue(WikiRegexes.HeadingLevelTwo.IsMatch(@"article 
==heading==
a"));
            Assert.IsTrue(WikiRegexes.HeadingLevelTwo.IsMatch(@"article 
== heading ==
a"));
            Assert.IsTrue(WikiRegexes.HeadingLevelTwo.IsMatch(@"article 
== heading ==
words"));

            // no matches
            Assert.IsFalse(WikiRegexes.HeadingLevelTwo.IsMatch(@"article ==
heading=="));
            Assert.IsFalse(WikiRegexes.HeadingLevelTwo.IsMatch(@"article 
==heading== words"));
            Assert.IsFalse(WikiRegexes.HeadingLevelTwo.IsMatch(@"article ===heading=="));
            Assert.IsFalse(WikiRegexes.HeadingLevelTwo.IsMatch(@"article 
====heading===
words"));
        }

        [Test]
        public void SectionLevelTwoTests()
        {
            Assert.IsTrue(WikiRegexes.SectionLevelTwo.IsMatch(@"== heading a ==
words
=== subsection ===
words
words2
== heading b ==
"));

            Assert.IsFalse(WikiRegexes.SectionLevelTwo.IsMatch(@"=== subsection ===
words
words2"));
            Assert.IsFalse(WikiRegexes.SectionLevelTwo.IsMatch(@"=== heading a ==
words
=== subsection ===
words
words2"));
            Assert.IsFalse(WikiRegexes.SectionLevelTwo.IsMatch(@"= heading a =
words
=== subsection ===
words
words2"));
        }

        [Test]
        public void ArticleToFirstLevelTwoHeadingTests()
        {
            Assert.IsTrue(WikiRegexes.ArticleToFirstLevelTwoHeading.IsMatch(@"words
== heading a ==
"));

            Assert.IsFalse(WikiRegexes.ArticleToFirstLevelTwoHeading.IsMatch(@"words
=== heading a ===
"));
        }

        [Test]
        public void UncatTests()
        {
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Uncategorized}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Uncategorised}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{uncategorized}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{uncategorised}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{uncat}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Uncat}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Uncat|date=January 2009}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{uncategorised|date=May 2008}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Uncategorised|date=May 2008}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Uncategorisedstub|date=May 2008}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Uncategorisedstub|date = May 2008}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{uncategorised|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"));

            // all the other redirects
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Classify}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{CatNeeded}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Catneeded}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Uncategorised}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Uncat}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Categorize}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Categories needed}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Categoryneeded}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Category needed}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Category requested}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Categories requested}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Nocats}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Categorise}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Nocat}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Uncat-date}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Uncategorized-date}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Needs cat}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Needs cats}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Cats needed}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Cat needed}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{classify}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{catneeded}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{catneeded}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{uncategorised}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{uncat}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{categorize}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{categories needed}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{categoryneeded}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{category needed}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{category requested}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{categories requested}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{nocats}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{categorise}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{nocat}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{uncat-date}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{uncategorized-date}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{needs cat}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{needs cats}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{cats needed}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{cat needed}}"));

            // no match
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(@"{{Uncategorized other template}}"));
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(@"{{Uncategorized other template|foo=bar}}"));
        }

        [Test]
        public void ArticleIssuesTests()
        {
            Assert.IsTrue(WikiRegexes.ArticleIssues.IsMatch(@"{{Article issues|wikify=May 2008|a=b|c=d}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssues.IsMatch(@"{{Articleissues|wikify=May 2008|a=b|c=d}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssues.IsMatch(@"{{articleissues|wikify=May 2008|a=b|c=d}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssues.IsMatch(@"{{article issues|wikify=May 2008|a=b|c=d}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssues.IsMatch(@"{{article issues | wikify=May 2008|a=b|c=d}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssues.IsMatch(@"{{article issues
           | wikify=May 2008|a=b|c=d}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssues.IsMatch(@"{{article issues|}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssues.IsMatch(@"{{Article issues|}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssues.IsMatch(@"{{articleissues}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssues.IsMatch(@"{{Articleissues}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssues.IsMatch(@"{{Articleissues }}"));

            // no matches
            Assert.IsFalse(WikiRegexes.ArticleIssues.IsMatch(@"{{ARTICLEISSUES }}"));
            Assert.IsFalse(WikiRegexes.ArticleIssues.IsMatch(@"{{Bert|Articleissues }}"));
        }

        [Test]
        public void ArticleIssuesTemplatesTests()
        {
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{advert|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{advert|date =  {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{autobiography|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{biased|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{blpdispute|date =  April 2008}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{citations missing|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{citationstyle|date =  May 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{citecheck|date=April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{cleanup|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{COI|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{colloquial|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{confusing|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{context|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{contradict|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{copyedit|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{criticisms|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{crystal|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{deadend|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{disputed|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{do-attempt|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{essay|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{examplefarm|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{expand|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{expert| topic name}}")); // not dated
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{external links|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{fancruft|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{fansite|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{fiction|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{gameguide|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{globalize|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{grammar|date= April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{histinfo|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{hoax|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{howto|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{inappropriate person|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{in-universe|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{importance|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{incomplete|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{intro length|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{intromissing|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{introrewrite|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{jargon|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{laundrylists|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{likeresume|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{long|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{newsrelease|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{notable|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{notability|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{onesource|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{OR|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{orphan|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{peacock|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{plot|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{POV|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{primarysources|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{prose|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{proseline|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{quotefarm|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{recent|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{refimprove|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{restructure|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{review|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{rewrite|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{roughtranslation|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{sections|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{self-published|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{spam|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{story|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{synthesis|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{tone|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{tooshort|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{travelguide|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{trivia|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{unbalanced|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{unencyclopedic|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{unreferenced|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{update|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{weasel|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{wikify|date =  April 2009}}"));

            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{advert}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{autobiography}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{biased}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{blpdispute}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{citations missing}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{citationstyle}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{citecheck}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{cleanup}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{COI}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{colloquial}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{confusing}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{context}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{contradict}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{copyedit}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{criticisms}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{crystal}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{deadend}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{disputed}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{do-attempt}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{essay}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{examplefarm}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{expand}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{external links}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{fancruft}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{fansite}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{fiction}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{gameguide}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{globalize}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{grammar}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{histinfo}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{hoax}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{howto}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{inappropriate person}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{in-universe}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{importance}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{incomplete}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{intro length}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{intromissing}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{introrewrite}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{jargon}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{laundrylists}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{likeresume}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{long}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{newsrelease}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{notable}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{onesource}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{OR}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{orphan}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{peacock}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{plot}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{POV}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{primarysources}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{prose}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{proseline}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{quotefarm}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{recent}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{refimprove}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{restructure}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{review}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{rewrite}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{roughtranslation}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{sections}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{self-published}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{spam}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{story}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{synthesis}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{tone}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{tooshort}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{travelguide}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{trivia}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{unbalanced}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{unencyclopedic}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{unreferenced}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{update}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{weasel}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{wikify}}"));

            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Advert}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Autobiography}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Biased}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Blpdispute}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Citations missing}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Citationstyle}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Citecheck}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Cleanup}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Colloquial}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Confusing}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Context}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Contradict}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Copyedit}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Criticisms}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Crystal}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Deadend}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Disputed}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Do-attempt}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Essay}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Examplefarm}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Expand}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{External links}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Fancruft}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Fansite}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Fiction}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Gameguide}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Globalize}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Grammar}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Histinfo}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Hoax}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Howto}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Inappropriate person}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{In-universe}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Importance}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Incomplete}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Intro length}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Intromissing}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Introrewrite}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Jargon}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Laundrylists}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Likeresume}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Long}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Newsrelease}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Notable}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Onesource}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Orphan}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Peacock}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Plot}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Primarysources}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Prose}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Proseline}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Quotefarm}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Recent}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Refimprove}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Restructure}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Review}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Rewrite}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Roughtranslation}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Sections}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Self-published}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Spam}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Story}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Synthesis}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Tone}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Tooshort}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Travelguide}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Trivia}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Unbalanced}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Unencyclopedic}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Unreferenced}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Update}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Weasel}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Wikify}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Coi}}"));

            Assert.IsFalse(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Article issues|wikify=May 2008|a=b|c=d}}"));
            Assert.IsFalse(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{WIKIFY}}"));
            
            // no support for handling templates with multiple parameters
            Assert.IsFalse(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{notability|Biographies|date=December 2008}}"));
        }

        [Test]
        public void LifetimeSortkeyTests()
        {
            Assert.IsTrue(WikiRegexes.LifetimeSortkey.IsMatch(@"{{Lifetime|1833|1907|Bisson, Elie-Hercule}}"));
            Assert.IsTrue(WikiRegexes.LifetimeSortkey.IsMatch(@"{{BD|1833|1907|Bisson, Elie-Hercule}}"));
            Assert.IsTrue(WikiRegexes.LifetimeSortkey.IsMatch(@"{{BIRTH-DEATH-SORT|1833|1907|Bisson, Elie-Hercule}}"));
            Assert.IsTrue(WikiRegexes.LifetimeSortkey.IsMatch(@"{{Lifetime||1907|Bisson, Elie-Hercule}}"));
            Assert.IsTrue(WikiRegexes.LifetimeSortkey.IsMatch(@"{{Lifetime|MISSING|1907|Bisson, Elie-Hercule}}"));
            Assert.IsTrue(WikiRegexes.LifetimeSortkey.IsMatch(@"{{Lifetime|1833||Bisson, Elie-Hercule}}"));
            Assert.IsTrue(WikiRegexes.LifetimeSortkey.IsMatch(@"{{Lifetime|1833|MISSING|Bisson, Elie-Hercule}}"));
            Assert.IsTrue(WikiRegexes.LifetimeSortkey.IsMatch(@"{{lifetime|||Bisson, Elie-Hercule}}"));
            Assert.IsTrue(WikiRegexes.LifetimeSortkey.IsMatch(@"{{lifetime |||Bisson, Elie-Hercule}}"));
            Assert.IsTrue(WikiRegexes.LifetimeSortkey.IsMatch(@"{{lifetime
          |||Bisson, Elie-Hercule}}"));

            Assert.IsTrue(WikiRegexes.LifetimeSortkey.Match(@"{{Lifetime|1833|1907|Bisson, Elie-Hercule}}").Groups[1].Value.Equals("Bisson, Elie-Hercule"));

            // ignores whitespace when getting sortkey
            Assert.IsTrue(WikiRegexes.LifetimeSortkey.Match(@"{{Lifetime|1833|1907|Bisson, Elie-Hercule }}").Groups[1].Value.Equals("Bisson, Elie-Hercule"));
            Assert.IsTrue(WikiRegexes.LifetimeSortkey.Match(@"{{Lifetime|1833|1907|
          Bisson, Elie-Hercule }}").Groups[1].Value.Equals("Bisson, Elie-Hercule"));
            Assert.IsTrue(WikiRegexes.LifetimeSortkey.Match(@"{{Lifetime|1833|1907| Bisson, Elie-Hercule }}").Groups[1].Value.Equals("Bisson, Elie-Hercule"));

            // case sensitive
            Assert.IsFalse(WikiRegexes.LifetimeSortkey.IsMatch(@"{{LIFETIME|1833|1907|Bisson, Elie-Hercule}}"));
            Assert.IsFalse(WikiRegexes.LifetimeSortkey.IsMatch(@"{{bd|1833|1907|Bisson, Elie-Hercule}}"));

            // no match if no sortkey
            Assert.IsFalse(WikiRegexes.LifetimeSortkey.IsMatch(@"{{Lifetime|1833|1907|}}"));
        }

        [Test]
        public void LifetimeTests()
        {
            Assert.IsTrue(WikiRegexes.Lifetime.IsMatch(@"{{Lifetime|1833|1907|Bisson, Elie-Hercule}}"));
            Assert.IsTrue(WikiRegexes.Lifetime.IsMatch(@"{{BD|1833|1907|Bisson, Elie-Hercule}}"));
            Assert.IsTrue(WikiRegexes.Lifetime.IsMatch(@"{{BIRTH-DEATH-SORT|1833|1907|Bisson, Elie-Hercule}}"));
            Assert.IsTrue(WikiRegexes.Lifetime.IsMatch(@"{{Lifetime||1907|Bisson, Elie-Hercule}}"));
            Assert.IsTrue(WikiRegexes.Lifetime.IsMatch(@"{{Lifetime|MISSING|1907|Bisson, Elie-Hercule}}"));
            Assert.IsTrue(WikiRegexes.Lifetime.IsMatch(@"{{Lifetime|1833||Bisson, Elie-Hercule}}"));
            Assert.IsTrue(WikiRegexes.Lifetime.IsMatch(@"{{Lifetime|1833|MISSING|Bisson, Elie-Hercule}}"));
            Assert.IsTrue(WikiRegexes.Lifetime.IsMatch(@"{{lifetime|||Bisson, Elie-Hercule}}"));
            Assert.IsTrue(WikiRegexes.Lifetime.IsMatch(@"{{lifetime |||Bisson, Elie-Hercule}}"));
            Assert.IsTrue(WikiRegexes.Lifetime.IsMatch(@"{{lifetime
          |||Bisson, Elie-Hercule}}"));

            // case sensitive
            Assert.IsFalse(WikiRegexes.Lifetime.IsMatch(@"{{LIFETIME|1833|1907|Bisson, Elie-Hercule}}"));
            Assert.IsFalse(WikiRegexes.Lifetime.IsMatch(@"{{bd|1833|1907|Bisson, Elie-Hercule}}"));
        }

        [Test]
        public void DeadEndTests()
        {
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(@"{{Deadend}}"));
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(@"{{deadend}}"));
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(@"{{dead end}}"));
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(@"{{deadend|date=May 2008}}"));
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(@"{{Dead end}}"));
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(@"{{internal links}}"));
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(@"{{internallinks}}"));
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(@"{{Internal links}}"));
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(@"{{Internal links|date=May 2008}}"));
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(@"{{nuevointernallinks}}"));
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(@"{{Nuevointernallinks}}"));
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(@"{{dep}}"));
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(@"{{dep|date=May 2008|Foobar}}"));
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(@"{{Article issues|deadend=May 2008|a=b|c=d}}"));
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(@"{{articleissues|deadend=May 2008|a=b|c=d}}"));

            Assert.IsFalse(WikiRegexes.DeadEnd.IsMatch(@"{{deadend|}}"));
        }
    }
}
