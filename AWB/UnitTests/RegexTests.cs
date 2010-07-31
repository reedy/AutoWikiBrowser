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

            TestMatch(WikiRegexes.SimpleWikiLink, "[[foo|bar]]");

            TestMatch(WikiRegexes.SimpleWikiLink, "[[Image:Foo.jpg]]");
            TestMatch(WikiRegexes.SimpleWikiLink, "[[Image:Bar.jpg|Bar]]");
            TestMatch(WikiRegexes.SimpleWikiLink, "[[Image:Bar.jpg|Bar|20px]]");

            TestMatch(WikiRegexes.SimpleWikiLink, "[[Category:Foo]]");
            TestMatch(WikiRegexes.SimpleWikiLink, "[[Category:Foo|Bar]]");
        }

        [Test]
        public void RedirectTest()
        {
            TestMatch(WikiRegexes.Redirect, "#redirect [[Foo]]", "#redirect [[Foo]]", @"Foo");
            TestMatch(WikiRegexes.Redirect, "#redirect [[Foo|bar]]", "#redirect [[Foo|bar]]", @"Foo");
            TestMatch(WikiRegexes.Redirect, "#Redirect : [[Foo#bar]]", "#Redirect : [[Foo#bar]]", @"Foo#bar");
            TestMatch(WikiRegexes.Redirect, "#REDIRECT[[Foo]]", "#REDIRECT[[Foo]]", @"Foo");
            TestMatch(WikiRegexes.Redirect, "#redirect[[:Foo bar ]]", "#redirect[[:Foo bar ]]", @"Foo bar");

            TestMatches(WikiRegexes.Redirect, "[foo]]", 0);
        }

        [Test]
        public void TalkpageHeader()
        {
            TestMatches(WikiRegexes.TalkHeaderTemplate, @"{{talk header|foo}}", 1);
            TestMatches(WikiRegexes.TalkHeaderTemplate, @"{{ talk header}}", 1);
            TestMatches(WikiRegexes.TalkHeaderTemplate, @"{{Talk header|foo}}", 1);
            TestMatches(WikiRegexes.TalkHeaderTemplate, @"{{Talkheader|foo}}", 1);
            TestMatches(WikiRegexes.TalkHeaderTemplate, @"{{Talkpageheader|foo}}", 1);
            TestMatches(WikiRegexes.TalkHeaderTemplate, @"{{talkheader|foo}}", 1);
            TestMatches(WikiRegexes.TalkHeaderTemplate, @"{{talkheader
|foo|bar
|here}}", 1);
            TestMatches(WikiRegexes.TalkHeaderTemplate, @"{{Talkheader}}", 1);
            TestMatches(WikiRegexes.TalkHeaderTemplate, @"{{talk header|search=yes}}", 1);
            TestMatches(WikiRegexes.TalkHeaderTemplate, @"{{talk header|noarchive=yes}}", 1);

            Assert.AreEqual(@"Talkpageheader", WikiRegexes.TalkHeaderTemplate.Match(@"{{ Talkpageheader  |foo}}").Groups[1].Value);

            // no match
            TestMatches(WikiRegexes.TalkHeaderTemplate, @"{{talkarchivenav|noredlinks=yes}}", 0);

            TestMatches(WikiRegexes.TalkHeaderTemplate, @"{{talkheadernav}}", 0);
        }

        [Test]
        public void SkipTOCTemplateRegex()
        {
            TestMatches(WikiRegexes.SkipTOCTemplateRegex, @"{{Skip to talk}}", 1);
            TestMatches(WikiRegexes.SkipTOCTemplateRegex, @"{{ skiptotoctalk}}", 1);
            TestMatches(WikiRegexes.SkipTOCTemplateRegex, @"{{skiptotoc}}", 1);
        }

        [Test]
        public void WikiProjectBannerShellTemplate()
        {
            TestMatches(WikiRegexes.WikiProjectBannerShellTemplate, @"{{WPBS|1=foo}}", 1);
            TestMatches(WikiRegexes.WikiProjectBannerShellTemplate, @"{{WikiProjectBannerShell}}", 1);
            TestMatches(WikiRegexes.WikiProjectBannerShellTemplate, @"{{WikiProjectBanners|foo}}", 1);
            TestMatches(WikiRegexes.WikiProjectBannerShellTemplate, @"{{WikiProjectBanners|1=foo|bar}}", 1);
            TestMatches(WikiRegexes.WikiProjectBannerShellTemplate, @"{{WikiProjectBanners|blp=yes|activepol=yes|1=foo}}", 1);
            TestMatches(WikiRegexes.WikiProjectBannerShellTemplate, @"{{WikiProjectBanners|blp=yes|1=
            {{WPBiography|living=yes|class=}}
            }}", 1);
            TestMatches(WikiRegexes.WikiProjectBannerShellTemplate, @"{{WikiProjectBanners|blp=yes|1=
            {{WPBiography|living=yes|class=}}
            {{WikiProject Greece}}
            |activepol=yes
            }}", 1);
        }

        [Test]
        public void BLPUnsourced()
        {
            TestMatches(WikiRegexes.BLPSources, @"{{BLP unsourced|foo}}", 1);
            TestMatches(WikiRegexes.BLPSources, @"{{UnsourcedBLP|foo}}", 1);
            TestMatches(WikiRegexes.BLPSources, @"{{BLPunreferenced|foo}}", 1);
            TestMatches(WikiRegexes.BLPSources, @"{{Unreferencedblp|foo}}", 1);
            TestMatches(WikiRegexes.BLPSources, @"{{Blpunsourced|foo}}", 1);
            TestMatches(WikiRegexes.BLPSources, @"{{BLPunsourced|foo}}", 1);
            TestMatches(WikiRegexes.BLPSources, @"{{Unsourcedblp|foo}}", 1);
            TestMatches(WikiRegexes.BLPSources, @"{{BLPUnreferenced|foo}}", 1);
            TestMatches(WikiRegexes.BLPSources, @"{{Unsourced BLP|foo}}", 1);
            TestMatches(WikiRegexes.BLPSources, @"{{BLP unreferenced|foo}}", 1);
            TestMatches(WikiRegexes.BLPSources, @"{{Blpunref|foo}}", 1);
            TestMatches(WikiRegexes.BLPSources, @"{{Unreferenced BLP|foo}}", 1);
            TestMatches(WikiRegexes.BLPSources, @"{{Blpunreferenced|foo}}", 1);
            TestMatches(WikiRegexes.BLPSources, @"{{UnreferencedBLP|foo}}", 1);
            TestMatches(WikiRegexes.BLPSources, @"{{BLPUnsourced|foo}}", 1);
            TestMatches(WikiRegexes.BLPSources, @"{{Unreferenced blp|foo}}", 1);
            TestMatches(WikiRegexes.BLPSources, @"{{ BLP Unreferenced | foo}}", 1);
            TestMatches(WikiRegexes.BLPSources, @"{{bLP Unreferenced}}", 1);
        }

        [Test]
        public void NamedReferences()
        {
            Assert.IsTrue(WikiRegexes.NamedReferences.IsMatch(@"<ref name = ""foo"">text</ref>"));
            Assert.IsTrue(WikiRegexes.NamedReferences.IsMatch(@"<ref name =foo>text</ref>"));
            Assert.IsTrue(WikiRegexes.NamedReferences.IsMatch(@"<ref name=foo>text< / ref >"));
            Assert.IsTrue(WikiRegexes.NamedReferences.IsMatch(@"<ref name = 'foo'>text</ref>"));
            Assert.IsTrue(WikiRegexes.NamedReferences.IsMatch(@"< REF NAME = ""foo"">text</ref>"));
            Assert.IsTrue(WikiRegexes.NamedReferences.IsMatch(@"< REF NAME = ""foo"">
{{ cite web|
title = text |
work = text
}}</ref>"), "matches multiple line ref");
            Assert.IsTrue(WikiRegexes.NamedReferences.IsMatch(@"< REF NAME = ""foo"">text <br>more</ref>"), "case insensitive matching");

            Assert.AreEqual(@"<ref name=""Shul726"">Shul, p. 726</ref>", WikiRegexes.NamedReferences.Match(@"<ref name=""vietnam.ttu.edu""/><ref name=""Shul726"">Shul, p. 726</ref>").Value, "match is not across consecutive references – first condensed");
            Assert.AreEqual(@"<ref name=""Shul726"">Shul, p. 726</ref>", WikiRegexes.NamedReferences.Match(@"<ref name=""Shul726"">Shul, p. 726</ref><ref name=""Foo"">foo text</ref>").Value, "match is not across consecutive references – first full");
            Assert.AreEqual(@"Shul726", WikiRegexes.NamedReferences.Match(@"<ref name=""vietnam.ttu.edu""/><ref name=""Shul726"">Shul, p. 726</ref>").Groups[2].Value, "ref name is group 2");
            Assert.AreEqual(@"Shul, p. 726", WikiRegexes.NamedReferences.Match(@"<ref name=""vietnam.ttu.edu""/><ref name=""Shul726"">Shul, p. 726</ref>").Groups[3].Value, "ref value is group 3");
            Assert.AreEqual(@"Shul726", WikiRegexes.NamedReferences.Match(@"<ref name=""vietnam.ttu.edu""/><ref name=""Shul726"">
Shul, p. 726    </ref>").Groups[2].Value, "ref value doesn't include leading/trailing whitespace");
        }

        [Test]
        public void UnformattedTextTests()
        {
            Assert.IsTrue(WikiRegexes.UnformattedText.IsMatch(@"<pre>{{abc}}</pre>"));
            Assert.IsTrue(WikiRegexes.UnformattedText.IsMatch(@"<math>{{abc}}</math>"));
            Assert.IsTrue(WikiRegexes.UnformattedText.IsMatch(@"<nowiki>{{abc}}</nowiki>"));
            Assert.IsTrue(WikiRegexes.UnformattedText.IsMatch(@"now hello {{bye}} <pre>{now}}</pre>"));
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
            TestMatch(WikiRegexes.WikiLinksOnly, "[[:foo]]", "[[:foo]]");
            TestMatch(WikiRegexes.WikiLinksOnly, "[[a:foo]]", "[[a:foo]]");
            TestMatch(WikiRegexes.WikiLinksOnly, "[[FOO:BAR]]", "[[FOO:BAR]]");
            TestMatch(WikiRegexes.WikiLinksOnly, "[[foo bar:world series]]", "[[foo bar:world series]]");
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

            // don't consider Categories, Images and IW to be "WikiLinks Only"
            TestMatches(WikiRegexes.WikiLinksOnly, "[[Category:Test]]", 0);
            TestMatches(WikiRegexes.WikiLinksOnly, "[[de:Test]]", 0);
            TestMatches(WikiRegexes.WikiLinksOnly, "[[Image:Test,]]", 0);

            //Assert.AreEqual("Test", WikiRegexes.WikiLinksOnly.Matches("[[Test]]")[0].Groups[1].Value);
        }

        [Test]
        public void WikiLinksOnlyPossiblePipe()
        {
            TestMatch(WikiRegexes.WikiLinksOnlyPossiblePipe, "[[foo]]", "[[foo]]");
            TestMatch(WikiRegexes.WikiLinksOnlyPossiblePipe, "[[:foo]]", "[[:foo]]");
            TestMatch(WikiRegexes.WikiLinksOnlyPossiblePipe, "[[a:foo]]", "[[a:foo]]");
            TestMatch(WikiRegexes.WikiLinksOnlyPossiblePipe, "[[FOO:BAR]]", "[[FOO:BAR]]");
            TestMatch(WikiRegexes.WikiLinksOnlyPossiblePipe, "[[foo bar:world series]]", "[[foo bar:world series]]");
            TestMatch(WikiRegexes.WikiLinksOnlyPossiblePipe, "[[foo bar]]", "[[foo bar]]");
            TestMatches(WikiRegexes.WikiLinksOnlyPossiblePipe, "[[foo\r\nbar]]", 0);
            TestMatches(WikiRegexes.WikiLinksOnlyPossiblePipe, "[foo]]", 0);

            TestMatch(WikiRegexes.WikiLinksOnlyPossiblePipe, "[[foo|bar]]", "[[foo|bar]]");
            TestMatch(WikiRegexes.WikiLinksOnlyPossiblePipe, "[[foo]] [[bar]]", "[[foo]]");
            TestMatches(WikiRegexes.WikiLinksOnlyPossiblePipe, "[[foo]] [[bar]]", 2);

            TestMatch(WikiRegexes.WikiLinksOnlyPossiblePipe, "[[foo]]]", "[[foo]]");
            TestMatch(WikiRegexes.WikiLinksOnlyPossiblePipe, "[[[foo]]", "[[foo]]");

            TestMatches(WikiRegexes.WikiLinksOnlyPossiblePipe, "[[foo[]]", 0);
            TestMatch(WikiRegexes.WikiLinksOnlyPossiblePipe, "[[foo [[bar]] here]", "[[bar]]");

            // don't consider Categories, Images and IW to be "WikiLinks Only"
            TestMatches(WikiRegexes.WikiLinksOnlyPossiblePipe, "[[Category:Test]]", 0);
            TestMatches(WikiRegexes.WikiLinksOnlyPossiblePipe, "[[de:Test]]", 0);
            TestMatches(WikiRegexes.WikiLinksOnlyPossiblePipe, "[[Image:Test,]]", 0);

            Assert.AreEqual("foo", WikiRegexes.WikiLinksOnlyPossiblePipe.Match("[[foo|bar]]").Groups[1].Value);
            Assert.AreEqual("foo", WikiRegexes.WikiLinksOnlyPossiblePipe.Match("[[foo]]").Groups[1].Value);
            Assert.AreEqual("foo smith:the great", WikiRegexes.WikiLinksOnlyPossiblePipe.Match("[[foo smith:the great|bar]]").Groups[1].Value);
            Assert.AreEqual("bar here", WikiRegexes.WikiLinksOnlyPossiblePipe.Match("[[foo|bar here]]").Groups[2].Value);
        }

        [Test]
        public void DeadLinkTests()
        {
            TestMatch(WikiRegexes.DeadLink, "{{dead link}}", "{{dead link}}");
            TestMatch(WikiRegexes.DeadLink, "{{Dead link}}", "{{Dead link}}");
            TestMatch(WikiRegexes.DeadLink, "{{Deadlink}}", "{{Deadlink}}");
            TestMatch(WikiRegexes.DeadLink, "{{brokenlink}}", "{{brokenlink}}");
            TestMatch(WikiRegexes.DeadLink, "{{Dl}}", "{{Dl}}");
            TestMatch(WikiRegexes.DeadLink, "{{Dead link}}", "{{Dead link}}");
            TestMatch(WikiRegexes.DeadLink, "{{dead link|date=May 2009}}", "{{dead link|date=May 2009}}");
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

            Assert.AreEqual("foo", WikiRegexes.PipedWikiLink.Match("[[foo|bar]]").Groups[1].Value);
            Assert.AreEqual("bar", WikiRegexes.PipedWikiLink.Match("[[foo|bar]]").Groups[2].Value);
        }

        [Test]
        public void Blockquote()
        {
            // one line
            TestMatch(WikiRegexes.Blockquote, "<blockquote>foo bar< /blockquote>", "<blockquote>foo bar< /blockquote>");

            // multiple lines
            TestMatch(WikiRegexes.Blockquote, "< Blockquote >foo\r\nbar</ BLOCKQUOTE>", "< Blockquote >foo\r\nbar</ BLOCKQUOTE>");
        }

        //        [Test]
        //        public void Poem()
        //        {
        //            // one line
        //            TestMatch(WikiRegexes.Poem, "<poem>foo bar< /poem>", "<poem>foo bar< /poem>");
        //
        //            // multiple lines
        //            TestMatch(WikiRegexes.Poem, @"< Poem >foo
        //bar</ POEM>", @"< Poem >foo
        //bar</ POEM>");
        //        }

        [Test]
        public void Persondata()
        {
            string pd1 = @"{{Persondata
|NAME= Skipworth, Alison
|ALTERNATIVE NAMES=
|SHORT DESCRIPTION=British actress
|DATE OF BIRTH= 25 July 1863
|PLACE OF BIRTH= [[London, England]], England
|DATE OF DEATH= 5 July 1952
|PLACE OF DEATH= {{city-state|Los Angeles|California}}, U.S.
}}", pd2 = @"{{Persondata
|NAME= Skipworth, Alison
|ALTERNATIVE NAMES=
|SHORT DESCRIPTION=British actress
|DATE OF BIRTH= 25 July 1863
|PLACE OF BIRTH= [[London, England]], England
|DATE OF DEATH= 5 July 1952
|PLACE OF DEATH= Los Angeles, California, U.S.
}}";
            TestMatch(WikiRegexes.Persondata, pd1, pd1);
            TestMatch(WikiRegexes.Persondata, pd2, pd2);
        }

        [Test]
        public void UseDatesTemplateTests()
        {
            TestMatch(WikiRegexes.UseDatesTemplate, @"{{use mdy dates}}", true);
            TestMatch(WikiRegexes.UseDatesTemplate, @"{{mdy}}", true);
            TestMatch(WikiRegexes.UseDatesTemplate, @"{{Mdy}}", true);
            TestMatch(WikiRegexes.UseDatesTemplate, @"{{ Use dmy dates}}", true);
            TestMatch(WikiRegexes.UseDatesTemplate, @"{{dmy}}", true);
            TestMatch(WikiRegexes.UseDatesTemplate, @"{{DMY}}", true);
            TestMatch(WikiRegexes.UseDatesTemplate, @"{{use ymd dates}}", true);
            TestMatch(WikiRegexes.UseDatesTemplate, @"{{ISO}}", true);

            Assert.AreEqual(WikiRegexes.UseDatesTemplate.Match(@"{{use mdy dates}}").Groups[2].Value, "use mdy dates");
        }

        [Test]
        public void ISODates()
        {
            TestMatch(WikiRegexes.ISODates, @"on 2009-12-11 a", true);
            TestMatch(WikiRegexes.ISODates, @"on 2009-12-01 a", true);
            TestMatch(WikiRegexes.ISODates, @"on 2009-12-21 a", true);
            TestMatch(WikiRegexes.ISODates, @"on 2009-08-31 a", true);
            TestMatch(WikiRegexes.ISODates, @"BC]] |date=2003-10-19 }}", true);

            TestMatch(WikiRegexes.ISODates, @"on 1009-12-11 a", false);
            TestMatch(WikiRegexes.ISODates, @"on 2209-12-11 a", false);
            TestMatch(WikiRegexes.ISODates, @"on 2009-14-11 a", false);
            TestMatch(WikiRegexes.ISODates, @"on 2009-2-11 a", false);
            TestMatch(WikiRegexes.ISODates, @"on 2009-08-33 a", false);
            TestMatch(WikiRegexes.ISODates, @"on 2009/08/31 a", false);
        }

        [Test]
        public void ImageMapTests()
        {
            // one line
            TestMatch(WikiRegexes.ImageMap, "<imagemap>foo bar< /imagemap>", "<imagemap>foo bar< /imagemap>");
            TestMatch(WikiRegexes.ImageMap, "<Imagemap>foo bar< /Imagemap>", "<Imagemap>foo bar< /Imagemap>");
            TestMatch(WikiRegexes.ImageMap, "<imagemap id=hello>foo bar< /imagemap>", "<imagemap id=hello>foo bar< /imagemap>");

            // multiple lines
            TestMatch(WikiRegexes.ImageMap, @"< imagemap >foo
bar</ IMAGEMAP>", @"< imagemap >foo
bar</ IMAGEMAP>");
        }

        [Test]
        public void NoincludeTests()
        {
            // one line
            TestMatch(WikiRegexes.Noinclude, "<noinclude>foo bar< /noinclude>", "<noinclude>foo bar< /noinclude>");
            TestMatch(WikiRegexes.Noinclude, "<Noinclude>foo bar< /Noinclude>", "<Noinclude>foo bar< /Noinclude>");

            // multiple lines
            TestMatch(WikiRegexes.Noinclude, @"< noinclude >foo
bar</ NOINCLUDE>", @"< noinclude >foo
bar</ NOINCLUDE>");
        }

        [Test]
        public void IncludeonlyTests()
        {
            // one line
            TestMatch(WikiRegexes.Includeonly, "<includeonly>foo bar< /includeonly>", "<includeonly>foo bar< /includeonly>");
            TestMatch(WikiRegexes.Includeonly, "<Includeonly>foo bar< /Includeonly>", "<Includeonly>foo bar< /Includeonly>");
            TestMatch(WikiRegexes.Includeonly, "<onlyInclude>foo bar< /onlyInclude>", "<onlyInclude>foo bar< /onlyInclude>");

            // multiple lines
            TestMatch(WikiRegexes.Includeonly, @"< includeonly >foo
bar</ INCLUDEONLY>", @"< includeonly >foo
bar</ INCLUDEONLY>");
        }

        [Test]
        public void Template()
        {
            RegexAssert.Matches("{{foo}}", WikiRegexes.TemplateMultiline, "{{foo}}");
            RegexAssert.Matches("{{foo}}", WikiRegexes.TemplateMultiline, "123{{foo}}test");
            RegexAssert.Matches("{{foo|bar}}", WikiRegexes.TemplateMultiline, "{{foo|bar}}");
            RegexAssert.Matches("{{foo\r\n|bar=test}}", WikiRegexes.TemplateMultiline, "{{foo\r\n|bar=test}}");

            RegexAssert.Matches("Should match distinct templates", WikiRegexes.TemplateMultiline, "{{foo}}{{bar}}", "{{foo}}", "{{bar}}");

            // regex won't match if nested template or curly-bracketed stuff
            RegexAssert.NoMatch(WikiRegexes.TemplateMultiline, "{{foo| {bar} }}");
        }

        [Test]
        public void NestedTemplates()
        {
            RegexAssert.Matches("{{foo}}", WikiRegexes.NestedTemplates, "{{foo}}");
            RegexAssert.Matches("{{foo}}", WikiRegexes.NestedTemplates, "123{{foo}}test");
            RegexAssert.Matches("{{foo|bar}}", WikiRegexes.NestedTemplates, "{{foo|bar}}");
            RegexAssert.Matches("{{foo\r\n|bar=test}}", WikiRegexes.NestedTemplates, "{{foo\r\n|bar=test}}");
            RegexAssert.Matches("Should match distinct templates", WikiRegexes.NestedTemplates, "{{foo}}{{bar}}", "{{foo}}", "{{bar}}");
            RegexAssert.Matches("{{foo| {bar} }}", WikiRegexes.NestedTemplates, "{{foo| {bar} }}");
            RegexAssert.Matches("{{foo {{bar}} end}}", WikiRegexes.NestedTemplates, "{{foo {{bar}} end}}");

            RegexAssert.Matches("{{ foo |bar}}", WikiRegexes.NestedTemplates, "{{ foo |bar}}");
            RegexAssert.Matches("{{foo<!--comm-->|bar}}", WikiRegexes.NestedTemplates, "{{foo<!--comm-->|bar}}");
            RegexAssert.Matches("", WikiRegexes.NestedTemplates, "{{foo");
        }

        [Test]
        public void TemplateName()
        {
            Assert.IsTrue(WikiRegexes.TemplateName.Match(@"{{Start date and age|1833|7|11}}").Groups[1].Value == "Start date and age");

            // whitespace handling
            Assert.IsTrue(WikiRegexes.TemplateName.Match(@"{{ Start date and age |1833|7|11}}").Groups[1].Value == "Start date and age");
            Assert.IsTrue(WikiRegexes.TemplateName.Match(@"{{
Start date and age
|1833|7|11}}").Groups[1].Value == "Start date and age");

            // embedded comments
            Assert.IsTrue(WikiRegexes.TemplateName.Match(@"{{start date and age <!--comm--> |1833|7|11}}").Groups[1].Value == "start date and age");
            Assert.IsTrue(WikiRegexes.TemplateName.Match(@"{{start date and age <!--comm-->}}").Groups[1].Value == "start date and age");

            // works on part templates
            Assert.IsTrue(WikiRegexes.TemplateName.Match(@"{{Start date and age|1833|7|").Groups[1].Value == "Start date and age");
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

            RegexAssert.Matches(WikiRegexes.Refs, "<REF NAME=\"foo\" >bar</ref >", "<REF NAME=\"foo\" >bar</ref >");
            RegexAssert.Matches(WikiRegexes.Refs, "<REF  NAME=foo>bar< /ref>", "<REF  NAME=foo>bar< /ref>");
            RegexAssert.Matches(WikiRegexes.Refs, "<ReF Name=foo/>", "<ReF Name=foo/>");
            RegexAssert.Matches(WikiRegexes.Refs, "<ReF Name = 'foo'/>", "<ReF Name = 'foo'/>");
            RegexAssert.Matches(WikiRegexes.Refs, "<ReF Name = \"foo\"/>", "<ReF Name = \"foo\"/>");
            RegexAssert.Matches(WikiRegexes.Refs, "< ref>foo</ ref>", "< ref>foo</ ref>");
            RegexAssert.Matches(WikiRegexes.Refs, @"<ref name= ""foo/bar""/>", @"<ref name= ""foo/bar""/>");
            RegexAssert.Matches(WikiRegexes.Refs, @"<ref name= ""foo/bar"">a</ref>", @"<ref name= ""foo/bar"">a</ref>");

            RegexAssert.NoMatch(WikiRegexes.Refs, "<refname=foo>bar</ref>");
            RegexAssert.NoMatch(WikiRegexes.Refs, "<refname=foo/>");

            RegexAssert.Matches(WikiRegexes.Refs, "<ref group=a name=foo/>", "<ref group=a name=foo/>");
            RegexAssert.Matches(WikiRegexes.Refs, "<ref name=foo group=a />", "<ref name=foo group=a />");

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_10#.3Cp.3E_deletion_in_references_and_notes
            RegexAssert.Matches(WikiRegexes.Refs, "<ref>foo<!-- bar --></ref>", "<ref>foo<!-- bar --></ref>");
            // shouldn't eat too much
            RegexAssert.Matches(WikiRegexes.Refs, "<ref>foo<!-- bar --></ref> <ref>foo</ref>", "<ref>foo<!-- bar --></ref>", "<ref>foo</ref>");

            TestMatches(WikiRegexes.Refs, @"Article<ref>A</ref> <ref>B</ref> <ref>C</ref> <ref>B</ref> <ref>E</ref>", 5);

            // this is why the <DEPTH> business is needed in WikiRegexes.Refs
            RegexAssert.Matches(new Regex(WikiRegexes.Refs + @"\."), "Foo.<ref>bar</ref>. The next Foo.<ref>bar <br> other</ref> The next Foo.<ref>bar</ref> The next Foo.<ref>bar</ref> The nextFoo<ref>bar2</ref>. The next", "<ref>bar</ref>.", "<ref>bar2</ref>.");
        }

        [Test]
        public void Small()
        {
            RegexAssert.Matches(WikiRegexes.Small, "<small>foo</small>", "<small>foo</small>");
            RegexAssert.Matches(WikiRegexes.Small, "<small  >foo</small >", "<small  >foo</small >");
            RegexAssert.Matches(WikiRegexes.Small, @"<small>
foo
</small>", @"<small>
foo
</small>");

            RegexAssert.Matches(WikiRegexes.Small, "<SMALL>foo</SMALL>", "<SMALL>foo</SMALL>");
            RegexAssert.Matches(WikiRegexes.Small, "<small>a<small>foo</small>b</small>", "<small>a<small>foo</small>b</small>");
            RegexAssert.Matches(WikiRegexes.Small, @"<small>..<small>...</small>", @"<small>...</small>");
        }

        [Test]
        public void Nowiki()
        {
            RegexAssert.Matches(WikiRegexes.Nowiki, "<nowiki>foo</nowiki>", "<nowiki>foo</nowiki>");
            RegexAssert.Matches(WikiRegexes.Nowiki, "<nowiki >foo</nowiki >", "<nowiki >foo</nowiki >");
        }

        [Test]
        public void ExternalLink()
        {
            RegexAssert.Matches(WikiRegexes.ExternalLinks, "http://google.co.uk", "http://google.co.uk");
            RegexAssert.Matches(WikiRegexes.ExternalLinks, "http://google-here.co.uk", "http://google-here.co.uk");
            RegexAssert.Matches(WikiRegexes.ExternalLinks, "https://google.co.uk", "https://google.co.uk");
            RegexAssert.Matches(WikiRegexes.ExternalLinks, "http://foo.com/asdfasdf/asdf.htm", "http://foo.com/asdfasdf/asdf.htm");
            RegexAssert.Matches(WikiRegexes.ExternalLinks, "http://www.google.co.uk", "http://www.google.co.uk");

            RegexAssert.Matches(WikiRegexes.ExternalLinks, "lol http://www.google.co.uk lol", "http://www.google.co.uk");

            RegexAssert.Matches(WikiRegexes.ExternalLinks, "[http://www.google.co.uk]", "[http://www.google.co.uk]");
            RegexAssert.Matches(WikiRegexes.ExternalLinks, "[http://www.google.co.uk google]", "[http://www.google.co.uk google]");

            RegexAssert.Matches(WikiRegexes.ExternalLinks, "lol [http://www.google.co.uk] lol", "[http://www.google.co.uk]");
            RegexAssert.Matches(WikiRegexes.ExternalLinks, "lol [http://www.google.co.uk google] lol", "[http://www.google.co.uk google]");

            //http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_14#Regex_problem
            RegexAssert.Matches(WikiRegexes.ExternalLinks, "http://www.google.co.uk google}}", "http://www.google.co.uk");
            RegexAssert.Matches(WikiRegexes.ExternalLinks, "http://www.google.co.uk}}", "http://www.google.co.uk");

            RegexAssert.Matches(WikiRegexes.ExternalLinks, @"date=April 2010|url=http://w/010111a.html}}", "http://w/010111a.html");

            // incorrect brackets
            RegexAssert.Matches(WikiRegexes.ExternalLinks, "lol [http://www.google.co.uk lol", "http://www.google.co.uk");

            RegexAssert.NoMatch(WikiRegexes.ExternalLinks, "Google");

            // protocol is group 1
            Assert.AreEqual("http", WikiRegexes.ExternalLinks.Match(@"http://google.co.uk").Groups[1].Value);
            Assert.AreEqual("svn", WikiRegexes.ExternalLinks.Match(@"svn://google.co.uk Google}}").Groups[1].Value);
            Assert.AreEqual("Http", WikiRegexes.ExternalLinks.Match(@"Http://google.co.uk").Groups[1].Value);
            Assert.AreEqual("HTTP", WikiRegexes.ExternalLinks.Match(@"HTTP://google.co.uk").Groups[1].Value);

            // not when in external link brackets
            Assert.AreEqual("", WikiRegexes.ExternalLinks.Match(@"[http://google.co.uk Google]").Groups[1].Value);
        }

        [Test]
        public void PossibleInterwikis()
        {
            RegexAssert.Matches(WikiRegexes.PossibleInterwikis, "foo[[en:bar]]", "[[en:bar]]");
            RegexAssert.Matches(WikiRegexes.PossibleInterwikis, "[[en:bar]][[ru:", "[[en:bar]]");
            RegexAssert.Matches(WikiRegexes.PossibleInterwikis, "foo[[en:bar:quux]][[ru:boz test]]", "[[en:bar:quux]]", "[[ru:boz test]]");

            RegexAssert.NoMatch(WikiRegexes.PossibleInterwikis, "[[:en:foo]]");
            RegexAssert.NoMatch(WikiRegexes.PossibleInterwikis, "[[:foo]]");
            RegexAssert.NoMatch(WikiRegexes.PossibleInterwikis, "[[File:foo]]");

            Assert.AreEqual("en", WikiRegexes.PossibleInterwikis.Match("[[ en :bar]]").Groups[1].Value);
            Assert.AreEqual("bar", WikiRegexes.PossibleInterwikis.Match("[[en: bar ]]").Groups[2].Value);

            // length outside range
            RegexAssert.NoMatch(WikiRegexes.PossibleInterwikis, "[[e:foo]]");
            RegexAssert.NoMatch(WikiRegexes.PossibleInterwikis, "[[abcdefghijlkmnop:foo]]");
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
        public void RFromModification()
        {
            Assert.IsTrue(Tools.NestedTemplateRegex(WikiRegexes.RFromModificationList).IsMatch(@"{{R from modification}}"));
            Assert.IsTrue(Tools.NestedTemplateRegex(WikiRegexes.RFromModificationList).IsMatch(@"{{ r from modification}}"));
            Assert.IsTrue(Tools.NestedTemplateRegex(WikiRegexes.RFromModificationList).IsMatch(@"{{R mod }}"));
            Assert.IsTrue(Tools.NestedTemplateRegex(WikiRegexes.RFromModificationList).IsMatch(@"{{R from alternate punctuation}}"));
            Assert.IsTrue(Tools.NestedTemplateRegex(WikiRegexes.RFromModificationList).IsMatch(@"{{R from alternative punctuation}}"));
        }

        [Test]
        public void RFromTitleWithoutDiacritics()
        {
            Assert.IsTrue(Tools.NestedTemplateRegex(WikiRegexes.RFromTitleWithoutDiacriticsList).IsMatch(@"{{R to accents}}"));
            Assert.IsTrue(Tools.NestedTemplateRegex(WikiRegexes.RFromTitleWithoutDiacriticsList).IsMatch(@"{{Redirects from title without diacritics}}"));
            Assert.IsTrue(Tools.NestedTemplateRegex(WikiRegexes.RFromTitleWithoutDiacriticsList).IsMatch(@"{{RDiacr}}"));
            Assert.IsTrue(Tools.NestedTemplateRegex(WikiRegexes.RFromTitleWithoutDiacriticsList).IsMatch(@"{{r to unicode name}}"));
            Assert.IsTrue(Tools.NestedTemplateRegex(WikiRegexes.RFromTitleWithoutDiacriticsList).IsMatch(@"{{ R to unicode}}"));
            Assert.IsTrue(Tools.NestedTemplateRegex(WikiRegexes.RFromTitleWithoutDiacriticsList).IsMatch(@"{{R to unicode  }}"));
            Assert.IsTrue(Tools.NestedTemplateRegex(WikiRegexes.RFromTitleWithoutDiacriticsList).IsMatch(@"{{R to title with diacritics}}"));
            Assert.IsTrue(Tools.NestedTemplateRegex(WikiRegexes.RFromTitleWithoutDiacriticsList).IsMatch(@"{{R to diacritics}}"));
            Assert.IsTrue(Tools.NestedTemplateRegex(WikiRegexes.RFromTitleWithoutDiacriticsList).IsMatch(@"{{R from name without diacritics}}"));
            Assert.IsTrue(Tools.NestedTemplateRegex(WikiRegexes.RFromTitleWithoutDiacriticsList).IsMatch(@"{{R from title without diacritics}}"));
            Assert.IsTrue(Tools.NestedTemplateRegex(WikiRegexes.RFromTitleWithoutDiacriticsList).IsMatch(@"{{R from original name without diacritics}}"));
            Assert.IsTrue(Tools.NestedTemplateRegex(WikiRegexes.RFromTitleWithoutDiacriticsList).IsMatch(@"{{R without diacritics}}"));
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
            Assert.IsTrue(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello<ref>Fred</ref> {{reflist|2}}"));
            Assert.IsTrue(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello<ref>Fred</ref> {{Reflist}}"));
            Assert.IsTrue(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello<ref>Fred</ref> {{ref-list}}"));
            Assert.IsTrue(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello<ref>Fred</ref> {{Reflink}}"));
            Assert.IsTrue(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello<ref>Fred</ref> {{reflink}}"));
            Assert.IsTrue(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello<ref>Fred</ref> {{References}}"));
            Assert.IsTrue(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello<ref>Fred</ref> {{references}}"));
            Assert.IsTrue(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello<ref>Fred</ref> <references/>"));
            Assert.IsTrue(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello<ref>Fred</ref> <references />"));
            Assert.IsTrue(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello<ref>Fred</ref> {{Listaref|2}}"));
            Assert.IsTrue(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello<ref>Fred</ref> {{ listaref | 2}}"));
            Assert.IsTrue(WikiRegexes.ReferencesTemplate.IsMatch(@"<references>
<ref name =Fred>Fred</ref> </references>"));
            Assert.IsTrue(WikiRegexes.ReferencesTemplate.IsMatch(@"<  references >
<ref name =Fred>Fred</ref> </  references  >"));

            Assert.IsFalse(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello<ref>Fred</ref>"));
            Assert.IsFalse(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello<ref name=""F"">Fred</ref>"));
            Assert.IsFalse(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello world"));

            Assert.IsTrue(WikiRegexes.ReferencesTemplate.IsMatch(@"{{Reflist|refs=
<ref name=modern>{{cite news |first=William }}
        }}"));
        }

        [Test]
        public void CiteTemplate()
        {
            Assert.IsTrue(WikiRegexes.CiteTemplate.IsMatch(@"{{cite web|url=a|title=b}}"));
            Assert.IsTrue(WikiRegexes.CiteTemplate.IsMatch(@"{{cite web}}"));
            Assert.IsTrue(WikiRegexes.CiteTemplate.IsMatch(@"{{ cite web|url=a|title=b}}"));
            Assert.IsTrue(WikiRegexes.CiteTemplate.IsMatch(@"{{Citeweb|url=a|title=b}}"));
            Assert.IsTrue(WikiRegexes.CiteTemplate.IsMatch(@"{{Citeweb|url=a|title=b and {{foo}} there}}"));

            // name derivation
            Assert.AreEqual(WikiRegexes.CiteTemplate.Match(@"{{cite web|url=a|title=b}}").Groups[2].Value, "cite web");
            Assert.AreEqual(WikiRegexes.CiteTemplate.Match(@"{{ cite web |url=a|title=b}}").Groups[2].Value, "cite web");
            Assert.AreEqual(WikiRegexes.CiteTemplate.Match(@"{{Cite web
|url=a|title=b}}").Groups[2].Value, "Cite web");
            Assert.AreEqual(WikiRegexes.CiteTemplate.Match(@"{{cite press release|url=a|title=b}}").Groups[2].Value, "cite press release");
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

            string bug1 = @"
==References==
<references />

{{Northampton County, Pennsylvania}}

[[Category:Boroughs in Pennsylvania]]
[[Category:Northampton County, Pennsylvania]]
[[Category:Settlements established in 1790]]

[[ht:Tatamy, Pennsilvani]]
[[nl:Tatamy]]
[[pt:Tatamy]]
[[vo:Tatamy]]";

            Assert.IsFalse(WikiRegexes.RefAfterReflist.IsMatch(bug1));
        }

        [Test]
        public void IbidOpCitation()
        {
            Assert.IsTrue(WikiRegexes.IbidOpCitation.IsMatch(@"ibid"));
            Assert.IsTrue(WikiRegexes.IbidOpCitation.IsMatch(@"Ibid"));
            Assert.IsTrue(WikiRegexes.IbidOpCitation.IsMatch(@"IBID"));
            Assert.IsTrue(WikiRegexes.IbidOpCitation.IsMatch(@"op cit"));
            Assert.IsTrue(WikiRegexes.IbidOpCitation.IsMatch(@"Op.cit"));
            Assert.IsTrue(WikiRegexes.IbidOpCitation.IsMatch(@"Op. cit"));
            Assert.IsTrue(WikiRegexes.IbidOpCitation.IsMatch(@"Op
cit"));

            Assert.IsFalse(WikiRegexes.IbidOpCitation.IsMatch(@"Libid was"));
            Assert.IsFalse(WikiRegexes.IbidOpCitation.IsMatch(@"The op was later cit"));
        }

        [Test]
        public void Ibid()
        {
            Assert.IsTrue(WikiRegexes.Ibid.IsMatch(@"{{ibid}}"));
            Assert.IsTrue(WikiRegexes.Ibid.IsMatch(@"{{ Ibid }}"));
            Assert.IsTrue(WikiRegexes.Ibid.IsMatch(@"{{ibid|date=May 2009}}"));
            Assert.IsTrue(WikiRegexes.Ibid.IsMatch(@"{{ibid | date=May 2009}}"));
            Assert.IsTrue(WikiRegexes.Ibid.IsMatch(@"{{Ibid|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"));
            Assert.IsTrue(WikiRegexes.Ibid.IsMatch(@"{{ibid|date=May 2009|foo=bar}}"));
            Assert.IsTrue(WikiRegexes.Ibid.IsMatch(@"{{ibid|}}"));

            Assert.IsFalse(WikiRegexes.Ibid.IsMatch(@"Libid was"));
            Assert.IsFalse(WikiRegexes.Ibid.IsMatch(@"{{IBID}}"));
            Assert.IsFalse(WikiRegexes.Ibid.IsMatch(@"{{Ibidate}}"));
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
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{Otheruses|something}}"));
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{Otheruses2|something}}"));
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{otheruse|something}}"));
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{otheruses}}"));
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{otheruse
|something}}"));
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{2otheruses|something}}"));

            Assert.IsFalse(WikiRegexes.Dablinks.IsMatch(@"{{For fake template|Fred the dancer|Fred(dancer)}}"));
            Assert.IsFalse(WikiRegexes.Dablinks.IsMatch(@"{{REDIRECT2|Fred the dancer|Fred Smith (dancer)}}"));
            Assert.IsFalse(WikiRegexes.Dablinks.IsMatch(@"{{Otheruse2|something}}")); //non-existent
        }

        [Test]
        public void Unreferenced()
        {
            Assert.IsTrue(WikiRegexes.Unreferenced.IsMatch(@"{{unreferenced}}"));
            Assert.IsTrue(WikiRegexes.Unreferenced.IsMatch(@"{{  Unreferenced}}"));
            Assert.IsTrue(WikiRegexes.Unreferenced.IsMatch(@"{{unreferenced  }}"));
            Assert.IsTrue(WikiRegexes.Unreferenced.IsMatch(@"{{unreferenced|date=May 2009}}"));
            Assert.IsTrue(WikiRegexes.Unreferenced.IsMatch(@"{{No refs}}"));

            Assert.IsFalse(WikiRegexes.Unreferenced.IsMatch(@"{{unreferenced-stub}}"));
        }

        [Test]
        public void PortalTemplateTests()
        {
            Assert.IsTrue(WikiRegexes.PortalTemplate.IsMatch(@"{{portal}}"));
            Assert.IsTrue(WikiRegexes.PortalTemplate.IsMatch(@"{{port|Foo}}"));
            Assert.IsTrue(WikiRegexes.PortalTemplate.IsMatch(@"{{ portal}}"));
            Assert.IsTrue(WikiRegexes.PortalTemplate.IsMatch(@"{{Portal}}"));
            Assert.IsTrue(WikiRegexes.PortalTemplate.IsMatch(@"{{Portalpar}}"));
            Assert.IsTrue(WikiRegexes.PortalTemplate.IsMatch(@"{{portal|Science}}"));
            Assert.IsTrue(WikiRegexes.PortalTemplate.IsMatch(@"{{portal|Spaceflight|RocketSunIcon.svg|break=yes}}"));

            Assert.IsFalse(WikiRegexes.PortalTemplate.IsMatch(@"{{PORTAL}}"));
            Assert.IsFalse(WikiRegexes.PortalTemplate.IsMatch(@"{{portalos}}"));
            Assert.IsFalse(WikiRegexes.PortalTemplate.IsMatch(@"{{portalparity}}"));
            Assert.IsFalse(WikiRegexes.PortalTemplate.IsMatch(@"{{Spanish portal|game}}"));
        }

        [Test]
        public void InfoboxTests()
        {
            TestMatch(WikiRegexes.InfoBox, @" {{Infobox hello| bye}} ", @"{{Infobox hello| bye}}", @"Infobox hello");
            TestMatch(WikiRegexes.InfoBox, @" {{ Infobox hello | bye}} ", @"{{ Infobox hello | bye}}", @"Infobox hello");
            TestMatch(WikiRegexes.InfoBox, @" {{infobox hello| bye}} ", @"{{infobox hello| bye}}", @"infobox hello");
            TestMatch(WikiRegexes.InfoBox, @" {{Infobox hello| bye {{a}} was}} ", @"{{Infobox hello| bye {{a}} was}}", @"Infobox hello");
            TestMatch(WikiRegexes.InfoBox, @" {{hello Infobox| bye {{a}} was}} ", @"{{hello Infobox| bye {{a}} was}}", @"hello Infobox");
            TestMatch(WikiRegexes.InfoBox, @" {{hello_Infobox| bye {{a}} was}} ", @"{{hello_Infobox| bye {{a}} was}}", @"hello_Infobox");
            Assert.IsTrue(WikiRegexes.InfoBox.IsMatch(@" {{infobox hello| bye}} "));
            Assert.IsTrue(WikiRegexes.InfoBox.IsMatch(@" {{infobox hello
| bye}} "));
            Assert.IsTrue(WikiRegexes.InfoBox.IsMatch(@" {{Infobox_play| bye}} "));
            Assert.IsTrue(WikiRegexes.InfoBox.IsMatch(@" {{some infobox| hello| bye}} "));
            Assert.IsTrue(WikiRegexes.InfoBox.IsMatch(@" {{Some Infobox| hello| bye}} "));
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
        public void TemplateEndTests()
        {
            Assert.AreEqual(WikiRegexes.TemplateEnd.Match(@"{{foo}}").Value, @"}}");
            Assert.AreEqual(WikiRegexes.TemplateEnd.Match(@"{{foo }}").Value, @" }}");
            Assert.AreEqual(WikiRegexes.TemplateEnd.Match(@"{{foo
}}").Value, @"
}}");
            Assert.AreEqual(WikiRegexes.TemplateEnd.Match(@"{{foo
}}").Groups[1].Value, "\r\n");
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
            Assert.AreEqual("heading", WikiRegexes.HeadingLevelTwo.Match(@"article
==heading==
a").Groups[1].Value);
            Assert.IsTrue(WikiRegexes.HeadingLevelTwo.IsMatch(@"article
== heading ==
a"));
            Assert.AreEqual(" heading ", WikiRegexes.HeadingLevelTwo.Match(@"article
== heading ==
a").Groups[1].Value);
            Assert.IsTrue(WikiRegexes.HeadingLevelTwo.IsMatch(@"article
== heading ==
words"));
            Assert.IsTrue(WikiRegexes.HeadingLevelTwo.IsMatch(@"article
==H==
a"));
            Assert.IsTrue(WikiRegexes.HeadingLevelTwo.IsMatch(@"article
==Hi==
a"));
            Assert.IsTrue(WikiRegexes.HeadingLevelTwo.IsMatch(@"article
==Here and=there==
a"));

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
        public void HeadingLevelThreeTests()
        {
            Assert.IsTrue(WikiRegexes.HeadingLevelThree.IsMatch(@"article
===heading===
a"));
            Assert.AreEqual("heading", WikiRegexes.HeadingLevelThree.Match(@"article
===heading===
a").Groups[1].Value);
            Assert.IsTrue(WikiRegexes.HeadingLevelThree.IsMatch(@"article
=== heading ===
a"));
            Assert.AreEqual(" heading ", WikiRegexes.HeadingLevelThree.Match(@"article
=== heading ===
a").Groups[1].Value);
            Assert.IsTrue(WikiRegexes.HeadingLevelThree.IsMatch(@"article
=== heading ===
words"));
            Assert.IsTrue(WikiRegexes.HeadingLevelThree.IsMatch(@"article
===H===
a"));
            Assert.IsTrue(WikiRegexes.HeadingLevelThree.IsMatch(@"article
===Hi===
a"));
            Assert.IsTrue(WikiRegexes.HeadingLevelThree.IsMatch(@"article
===Here and=there===
a"));

            // no matches
            Assert.IsFalse(WikiRegexes.HeadingLevelThree.IsMatch(@"article ===
heading==="));
            Assert.IsFalse(WikiRegexes.HeadingLevelThree.IsMatch(@"article
===heading=== words"));
            Assert.IsFalse(WikiRegexes.HeadingLevelThree.IsMatch(@"article ====heading==="));
            Assert.IsFalse(WikiRegexes.HeadingLevelThree.IsMatch(@"article
=====heading====
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
        public void ArticleIssuesTests()
        {
            Assert.IsTrue(WikiRegexes.MultipleIssues.IsMatch(@"{{Article issues|wikify=May 2008|a=b|c=d}}"));
            Assert.IsTrue(WikiRegexes.MultipleIssues.IsMatch(@"{{Articleissues|wikify=May 2008|a=b|c=d}}"));
            Assert.IsTrue(WikiRegexes.MultipleIssues.IsMatch(@"{{articleissues|wikify=May 2008|a=b|c=d}}"));
            Assert.IsTrue(WikiRegexes.MultipleIssues.IsMatch(@"{{article issues|wikify=May 2008|a=b|c=d}}"));
            Assert.IsTrue(WikiRegexes.MultipleIssues.IsMatch(@"{{article issues | wikify=May 2008|a=b|c=d}}"));
            Assert.IsTrue(WikiRegexes.MultipleIssues.IsMatch(@"{{article issues
           | wikify=May 2008|a=b|c=d}}"));
            Assert.IsTrue(WikiRegexes.MultipleIssues.IsMatch(@"{{article issues|}}"));
            Assert.IsTrue(WikiRegexes.MultipleIssues.IsMatch(@"{{Article issues|}}"));
            Assert.IsTrue(WikiRegexes.MultipleIssues.IsMatch(@"{{articleissues}}"));
            Assert.IsTrue(WikiRegexes.MultipleIssues.IsMatch(@"{{Articleissues}}"));
            Assert.IsTrue(WikiRegexes.MultipleIssues.IsMatch(@"{{Articleissues }}"));
            Assert.IsTrue(WikiRegexes.MultipleIssues.IsMatch(@"{{Multiple issues|wikify=May 2008|a=b|c=d}}"));
            Assert.IsTrue(WikiRegexes.MultipleIssues.IsMatch(@"{{ multiple issues|wikify=May 2008|a=b|c=d}}"));

            Assert.IsTrue(WikiRegexes.MultipleIssues.IsMatch(@"{{ multiple issues|wikify={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|orphan={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|c=d}}"));

            // no matches
            Assert.IsFalse(WikiRegexes.MultipleIssues.IsMatch(@"{{ARTICLEISSUES }}"));
            Assert.IsFalse(WikiRegexes.MultipleIssues.IsMatch(@"{{Bert|Articleissues }}"));
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
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{dead end|date =  April 2009}}"));
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
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Inappropriate tone|date =  April 2009}}"));
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{inappropriate tone|date =  April 2009}}"));
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
            Assert.IsTrue(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{Original research|date =  April 2009}}"));
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

            Assert.IsFalse(WikiRegexes.ArticleIssuesTemplates.IsMatch(@"{{OR}}"));

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
        public void WordApostropheTests()
        {
            Assert.IsTrue(WikiRegexes.RegexWordApostrophes.IsMatch(@"Rachel"));
            Assert.IsTrue(WikiRegexes.RegexWordApostrophes.IsMatch(@"Rachel's"));
            Assert.IsTrue(WikiRegexes.RegexWordApostrophes.IsMatch(@"Kwakwaka'wakw"));

            Assert.AreEqual("", WikiRegexes.RegexWordApostrophes.Replace(@"Kwakwaka'wakw", ""));
        }

        [Test]
        public void DeathsOrLivingCategoryTests()
        {
            Assert.IsTrue(WikiRegexes.DeathsOrLivingCategory.IsMatch(@"[[Category:653 deaths|Honorius]]"));
            Assert.IsTrue(WikiRegexes.DeathsOrLivingCategory.IsMatch(@"[[Category:839 deaths]]"));
            Assert.IsTrue(WikiRegexes.DeathsOrLivingCategory.IsMatch(@"[[Category:5th-century BC deaths]]"));
            Assert.IsTrue(WikiRegexes.DeathsOrLivingCategory.IsMatch(@"[[Category:1209 deaths]]"));
            Assert.IsTrue(WikiRegexes.DeathsOrLivingCategory.IsMatch(@"[[Category:Living people]]"));
            Assert.IsTrue(WikiRegexes.DeathsOrLivingCategory.IsMatch(@"[[Category:221 BC deaths]]"));
            Assert.IsTrue(WikiRegexes.DeathsOrLivingCategory.IsMatch(@"[[Category:Year of death missing]]"));
            Assert.IsTrue(WikiRegexes.DeathsOrLivingCategory.IsMatch(@"[[Category:Year of death unknown]]"));
            Assert.IsTrue(WikiRegexes.DeathsOrLivingCategory.IsMatch(@"[[Category:Date of death unknown]]"));

            // no matches
            Assert.IsFalse(WikiRegexes.DeathsOrLivingCategory.IsMatch(@""));
            Assert.IsFalse(WikiRegexes.DeathsOrLivingCategory.IsMatch(@"[[Category:strange deaths]]"));
            Assert.IsFalse(WikiRegexes.DeathsOrLivingCategory.IsMatch(@"1990 deaths"));
        }

        [Test]
        public void BirthsCategoryTests()
        {

            Assert.IsTrue(WikiRegexes.BirthsCategory.IsMatch(@"[[Category:12th-century births]]"));
            Assert.IsTrue(WikiRegexes.BirthsCategory.IsMatch(@"[[Category:1299 births]]"));
            Assert.IsTrue(WikiRegexes.BirthsCategory.IsMatch(@"[[Category:110 BC births]]"));

            //no matches
            Assert.IsFalse(WikiRegexes.BirthsCategory.IsMatch(@"[[Category:strange births]]"));
            Assert.IsFalse(WikiRegexes.BirthsCategory.IsMatch(@"1960 births"));
            Assert.IsFalse(WikiRegexes.BirthsCategory.IsMatch(@""));
        }

        [Test]
        public void DateBirthAndAgeTests()
        {
            Assert.IsTrue(WikiRegexes.DateBirthAndAge.IsMatch(@"{{Birth date|1972|02|18}}"));
            Assert.IsTrue(WikiRegexes.DateBirthAndAge.IsMatch(@"{{Birth date and age|1972|02|18}}"));
            Assert.IsTrue(WikiRegexes.DateBirthAndAge.IsMatch(@"{{Birth date| 1972 |02|18}}"));

            Assert.IsTrue(WikiRegexes.DateBirthAndAge.IsMatch(@"{{birth date and age|mf=yes|1980|3|9}}"));
            Assert.IsTrue(WikiRegexes.DateBirthAndAge.IsMatch(@"{{bda|mf=yes|1980|3|9}}"));

            Assert.AreEqual("1975", WikiRegexes.DateBirthAndAge.Match(@"{{birth-date|1975}}").Groups[1].Value);
            Assert.AreEqual("1975", WikiRegexes.DateBirthAndAge.Match(@"{{birth-date|   1975}}").Groups[1].Value);
            Assert.AreEqual("1984", WikiRegexes.DateBirthAndAge.Match(@"{{birth date and age|year=1984|month=2|day=6}}").Groups[1].Value);
        }

        [Test]
        public void DateDeathAndAgeTests()
        {
            Assert.IsTrue(WikiRegexes.DeathDate.IsMatch(@"{{Death date|1972|02|18}}"));
            Assert.IsTrue(WikiRegexes.DeathDate.IsMatch(@"{{Death date and age|1972|02|18}}"));
            Assert.IsTrue(WikiRegexes.DeathDate.IsMatch(@"{{Death date| 1972 |02|18}}"));

            Assert.IsTrue(WikiRegexes.DeathDate.IsMatch(@"{{death date and age|mf=yes|1980|3|9}}"));

            Assert.AreEqual("1975", WikiRegexes.DeathDate.Match(@"{{death-date|1975}}").Groups[1].Value);
            Assert.AreEqual("1975", WikiRegexes.DeathDate.Match(@"{{death-date|   1975}}").Groups[1].Value);
            Assert.AreEqual("1984", WikiRegexes.DeathDate.Match(@"{{death date and age|year=1984|month=2|day=6}}").Groups[1].Value);
            Assert.AreEqual("1911", WikiRegexes.DeathDate.Match(@"{{death date and age|1911|12|12|1821|10|02}}").Groups[1].Value);
        }

        [Test]
        public void DeathDateAndAge()
        {
            Assert.AreEqual("1911", WikiRegexes.DeathDateAndAge.Match(@"{{death date and age|1911|12|12|1821|10|02}}").Groups[1].Value);
            Assert.AreEqual("1911", WikiRegexes.DeathDateAndAge.Match(@"{{death-date and age|1911|12|12|1821|10|02}}").Groups[1].Value);
            Assert.AreEqual("1911", WikiRegexes.DeathDateAndAge.Match(@"{{dda|1911|12|12|1821|10|02}}").Groups[1].Value);
            Assert.AreEqual("1821", WikiRegexes.DeathDateAndAge.Match(@"{{death date and age|1911|12|12|1821|10|02}}").Groups[2].Value);
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
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(@"{{Article issues|dead end=May 2008|a=b|c=d}}"));
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(@"{{Multiple issues|deadend=May 2008|a=b|c=d}}"));
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(@"{{ Article issues|dead end=May 2008|a=b|c=d}}"));

            Assert.IsFalse(WikiRegexes.DeadEnd.IsMatch(@"{{deadend|}}"));
        }

        [Test]
        public void BareExternalLinkTests()
        {
            Assert.IsTrue(WikiRegexes.BareExternalLink.IsMatch(@"* http://www.site.com
"));
            Assert.IsTrue(WikiRegexes.BareExternalLink.IsMatch(@"* http://www.site.com
"));
            Assert.IsTrue(WikiRegexes.BareExternalLink.IsMatch(@"*   http://www.site.com
"));
            Assert.IsTrue(WikiRegexes.BareExternalLink.IsMatch(@"* http://www.site.com/great/a.htm
"));

            Assert.IsFalse(WikiRegexes.BareExternalLink.IsMatch(@"* [http://www.site.com text]
"));
            Assert.IsFalse(WikiRegexes.BareExternalLink.IsMatch(@"<ref>http://www.site.com</ref>
"));
        }

        [Test]
        public void BareRefExternalLink()
        {
            TestMatch(WikiRegexes.BareRefExternalLink, @"<ref>[http://news.bbc.co.uk/hi/England/story4384.htm]</ref>");
            TestMatch(WikiRegexes.BareRefExternalLink, @"<ref>[http://news.bbc.co.uk/hi/England/story4384.htm]]</ref>");
            TestMatch(WikiRegexes.BareRefExternalLink, @"< REF > [   http://news.bbc.co.uk/hi/England/story4384.htm]
< / ref  >");
            TestMatch(WikiRegexes.BareRefExternalLink, @"<ref name=hello>[http://news.bbc.co.uk/hi/England/story4384.htm]</ref>");
            TestMatch(WikiRegexes.BareRefExternalLink, @"<ref>[   http://news.bbc.co.uk/hi/England/story4384.htm   ]   </ref>");

            TestMatch(WikiRegexes.BareRefExternalLink, @"<ref>[http://news.bbc.co.uk/hi/England/story4384.htm title here]</ref>", false);

            Assert.AreEqual(WikiRegexes.BareRefExternalLink.Match(@"<ref>[ http://news.bbc.co.uk/hi/England/story4384.htm ] </ref>").Groups[1].Value, @"http://news.bbc.co.uk/hi/England/story4384.htm");
            Assert.AreEqual(WikiRegexes.BareRefExternalLink.Match(@"<ref>[ http://news.bbc.co.uk/hi/England/story4384.htm] </ref>").Groups[1].Value, @"http://news.bbc.co.uk/hi/England/story4384.htm");
            Assert.AreEqual(WikiRegexes.BareRefExternalLink.Match(@"<ref>[ http://news.bbc.co.uk/hi/England/story4384.htm]. </ref>").Groups[1].Value, @"http://news.bbc.co.uk/hi/England/story4384.htm");
        }

        [Test]
        public void BareRefExternalLinkBotGenTitle()
        {
            TestMatch(WikiRegexes.BareRefExternalLinkBotGenTitle, @"<ref>[http://news.bbc.co.uk/hi/England/story4384.htm]</ref>");
            TestMatch(WikiRegexes.BareRefExternalLinkBotGenTitle, @"<ref>[http://www.independent.co.uk/news/people/bo-johnson-30403.html Boris Johnson: People - The Independent<!-- Bot generated title -->]</ref>");

            TestMatch(WikiRegexes.BareRefExternalLinkBotGenTitle, @"<ref>Smith, Fred [http://www.independent.co.uk/news/people/bo-johnson-30403.html Boris Johnson: People - The Independent<!-- Bot generated title -->]</ref>", false);
            TestMatch(WikiRegexes.BareRefExternalLinkBotGenTitle, @"<ref>[http://www.independent.co.uk/news/people/bo-johnson-30403.html Boris Johnson: People - The Independent]</ref>", false);
            Assert.IsTrue(WikiRegexes.BareRefExternalLinkBotGenTitle.IsMatch(@"attack<ref>http://www.news.com.au/heraldsun/story/0,21985,23169580-5006022,00.html</ref> was portrayed"));
            
            Assert.AreEqual(@"http://news.bbc.co.uk/hi/England/story4384.htm", WikiRegexes.BareRefExternalLinkBotGenTitle.Match(@"<ref>[http://news.bbc.co.uk/hi/England/story4384.htm]</ref>").Groups[1].Value);
            Assert.AreEqual(@"http://news.bbc.co.uk/hi/England/story4384.htm", WikiRegexes.BareRefExternalLinkBotGenTitle.Match(@"<ref> [ http://news.bbc.co.uk/hi/England/story4384.htm]. </ref>").Groups[1].Value);
        }

        [Test]
        public void BoldItalicTests()
        {
            Assert.AreEqual(WikiRegexes.Bold.Match(@"'''foo'''").Groups[1].Value, @"foo");
            Assert.AreEqual(WikiRegexes.Bold.Match(@"'''foo bar'''").Groups[1].Value, @"foo bar");
            Assert.AreEqual(WikiRegexes.Bold.Match(@"'''foo's bar'''").Groups[1].Value, @"foo's bar");

            Assert.AreEqual(WikiRegexes.Italics.Match(@"''foo''").Groups[1].Value, @"foo");
            Assert.AreEqual(WikiRegexes.Italics.Match(@"''foo bar''").Groups[1].Value, @"foo bar");
            Assert.AreEqual(WikiRegexes.Italics.Match(@"''foo's bar''").Groups[1].Value, @"foo's bar");

            Assert.AreEqual(WikiRegexes.BoldItalics.Match(@"''''' foo'''''").Groups[1].Value, @" foo");
            Assert.AreEqual(WikiRegexes.BoldItalics.Match(@"'''''foo bar'''''").Groups[1].Value, @"foo bar");
            Assert.AreEqual(WikiRegexes.BoldItalics.Match(@"'''''foo's bar'''''").Groups[1].Value, @"foo's bar");
        }

        [Test]
        public void StarRowsTests()
        {
            Assert.AreEqual(WikiRegexes.StarRows.Match(@"*foo bar
Bert").Groups[1].Value, @"*");
            Assert.AreEqual(WikiRegexes.StarRows.Match(@"*foo bar
Bert").Groups[2].Value, "foo bar\r");

            Assert.AreEqual(WikiRegexes.StarRows.Match(@"    *foo bar").Groups[1].Value, @"*");
            Assert.AreEqual(WikiRegexes.StarRows.Match(@" *foo bar").Groups[2].Value, @"foo bar");
        }

        [Test]
        public void CircaTemplate()
        {
            Assert.IsTrue(WikiRegexes.CircaTemplate.IsMatch(@"{{circa}}"));
            Assert.IsTrue(WikiRegexes.CircaTemplate.IsMatch(@"{{ circa}}"));
            Assert.IsTrue(WikiRegexes.CircaTemplate.IsMatch(@"{{Circa}}"));
            Assert.IsTrue(WikiRegexes.CircaTemplate.IsMatch(@"{{circa|foo=yes}}"));
        }
        
        [Test]
        public void ReferenceList()
        {
            Assert.IsTrue(WikiRegexes.ReferenceList.IsMatch(@"{{reflist}}"));
            Assert.IsTrue(WikiRegexes.ReferenceList.IsMatch(@"{{references-small}}"));
            Assert.IsTrue(WikiRegexes.ReferenceList.IsMatch(@"{{references-2column}}"));
        }
    }
}
