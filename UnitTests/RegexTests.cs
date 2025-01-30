using System.Text.RegularExpressions;
using WikiFunctions;
using NUnit.Framework;
using NUnit.Framework.Legacy;

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
            Assert.That(r.IsMatch(text), Is.EqualTo(isMatch));
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
            Assert.That(r.Matches(text).Count, Is.EqualTo(expectedMatches));
        }

        protected static void TestMatch(Match m, params string[] groups)
        {
            ClassicAssert.GreaterOrEqual(m.Groups.Count, groups.Length, "Too few groups matched");

            for (int i = 0; i < groups.Length; i++)
            {
                if (groups[i] == null) continue;
                Assert.That(m.Groups[i].Value, Is.EqualTo(groups[i]));
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
            Assert.That(WikiRegexes.GenerateNamespaceRegex(0), Is.Empty);

            Assert.That(WikiRegexes.GenerateNamespaceRegex(Namespace.User), Is.EqualTo("User"));
            Assert.That(WikiRegexes.GenerateNamespaceRegex(Namespace.UserTalk), Is.EqualTo("User[ _]talk"));
            Assert.That(WikiRegexes.GenerateNamespaceRegex(Namespace.File), Is.EqualTo("File|Image"));

            Assert.That(WikiRegexes.GenerateNamespaceRegex(Namespace.Project), Is.EqualTo("Wikipedia|Project"));

            Assert.That(WikiRegexes.GenerateNamespaceRegex(Namespace.Media, Namespace.File),
                            Is.EqualTo("Media|File|Image"));
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
            TestMatch(WikiRegexes.Redirect, @"#REDIRECT
[[Foo]]", @"#REDIRECT
[[Foo]]", @"Foo");
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

            Assert.That(WikiRegexes.TalkHeaderTemplate.Match(@"{{ Talkpageheader  |foo}}").Groups[1].Value, Is.EqualTo(@"Talkpageheader"));

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
            TestMatches(WikiRegexes.WikiProjectBannerShellTemplate, @"{{WikiProject banner shell}}", 1);
            TestMatches(WikiRegexes.WikiProjectBannerShellTemplate, @"{{WikiProject Banner Shell}}", 1);
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
            TestMatches(WikiRegexes.BLPSources, @"{{ BLP unsourced section| foo}}", 1);
            TestMatches(WikiRegexes.BLPSources, @"{{bLP Unreferenced}}", 1);
            TestMatches(WikiRegexes.BLPSources, @"{{BLP unreferenced section| foo}}", 1);
        }

        [Test]
        public void NamedReferences()
        {
            ClassicAssert.IsTrue(WikiRegexes.NamedReferences.IsMatch(@"<ref name = ""foo"">text</ref>"));
            ClassicAssert.IsTrue(WikiRegexes.NamedReferences.IsMatch(@"<ref name = ""foo""></ref>"));
            ClassicAssert.IsTrue(WikiRegexes.NamedReferences.IsMatch(@"<ref name =foo>text</ref>"));
            ClassicAssert.IsTrue(WikiRegexes.NamedReferences.IsMatch(@"<ref name=foo>text< / ref >"));
            ClassicAssert.IsTrue(WikiRegexes.NamedReferences.IsMatch(@"<ref name=foo>te<taga</tag>xt</ref>"), "nested tag support");
            ClassicAssert.IsTrue(WikiRegexes.NamedReferences.IsMatch(@"<ref name = 'foo'>text</ref>"));
            ClassicAssert.IsTrue(WikiRegexes.NamedReferences.IsMatch(@"< REF NAME = ""foo"">text</ref>"));
            ClassicAssert.IsTrue(WikiRegexes.NamedReferences.IsMatch(@"< REF NAME = ""foo"">
{{ cite web|
title = text |
work = text
}}</ref>"), "matches multiple line ref");
            ClassicAssert.IsTrue(WikiRegexes.NamedReferences.IsMatch(@"< REF NAME = ""foo"">text <br>more</ref>"), "case insensitive matching");

            Assert.That(WikiRegexes.NamedReferences.Match(@"<ref name=""vietnam.ttu.edu""/><ref name=""Shul726"">Shul, p. 726</ref>").Value, Is.EqualTo(@"<ref name=""Shul726"">Shul, p. 726</ref>"), "match is not across consecutive references – first condensed");
            Assert.That(WikiRegexes.NamedReferences.Match(@"<ref name=""Shul726"">Shul, p. 726</ref><ref name=""Foo"">foo text</ref>").Value, Is.EqualTo(@"<ref name=""Shul726"">Shul, p. 726</ref>"), "match is not across consecutive references – first full");
            Assert.That(WikiRegexes.NamedReferences.Match(@"<ref name=""vietnam.ttu.edu""/><ref name=""Shul726"">Shul, p. 726</ref>").Groups[2].Value, Is.EqualTo(@"Shul726"), "ref name is group 2");
            Assert.That(WikiRegexes.NamedReferences.Match(@"<ref name=""vietnam.ttu.edu""/><ref name=""Shul726"">Shul, p. 726</ref>").Groups[3].Value, Is.EqualTo(@"Shul, p. 726"), "ref value is group 3");
            Assert.That(WikiRegexes.NamedReferences.Match(@"<ref name=""vietnam.ttu.edu""/><ref name=""Shul726"">
Shul, p. 726    </ref>").Groups[2].Value, Is.EqualTo(@"Shul726"), "ref value doesn't include leading/trailing whitespace");

            ClassicAssert.IsTrue(WikiRegexes.NamedReferencesIncludingCondensed.IsMatch(@"<ref name = ""foo"">text</ref>"));
            ClassicAssert.IsTrue(WikiRegexes.NamedReferencesIncludingCondensed.IsMatch(@"<ref name = ""fo/o"">text</ref>"));
            ClassicAssert.IsTrue(WikiRegexes.NamedReferencesIncludingCondensed.IsMatch(@"<ref name = ""foo"">text<tag>a</tag></ref>"), "matches with nested tags");
            ClassicAssert.IsTrue(WikiRegexes.NamedReferencesIncludingCondensed.IsMatch(@"<ref name = ""foo""></ref>"));
            ClassicAssert.IsTrue(WikiRegexes.NamedReferencesIncludingCondensed.IsMatch(@"<ref Name =foo>text</ref>"));
            ClassicAssert.IsTrue(WikiRegexes.NamedReferencesIncludingCondensed.IsMatch(@"<ref name=foo>text< / ref >"));
            ClassicAssert.IsTrue(WikiRegexes.NamedReferencesIncludingCondensed.IsMatch(@"<ref name=foo>te<taga</tag>xt</ref>"), "nested tag support");
            ClassicAssert.IsTrue(WikiRegexes.NamedReferencesIncludingCondensed.IsMatch(@"<ref name = 'foo'>text</ref>"));
            ClassicAssert.IsTrue(WikiRegexes.NamedReferencesIncludingCondensed.IsMatch(@"< ref NAME = ""foo"">text</ref>"));
            ClassicAssert.IsTrue(WikiRegexes.NamedReferencesIncludingCondensed.IsMatch(@"< ref NAME = ""foo"" />"), "matches condensed named ref");
            ClassicAssert.IsTrue(WikiRegexes.NamedReferencesIncludingCondensed.IsMatch(@"< ref NAME = ""foo"">
{{ cite web|
title = text |
work = text
}}</ref>"), "matches multiple line ref");
            ClassicAssert.IsTrue(WikiRegexes.NamedReferencesIncludingCondensed.IsMatch(@"< ref NAME = ""foo"">text <br>more</ref>"), "case insensitive matching");

            Assert.That(WikiRegexes.NamedReferencesIncludingCondensed.Match(@"<ref name=""vietnam.ttu.edu""/><ref name=""Shul726"">Shul, p. 726</ref>").Value, Is.EqualTo(@"<ref name=""vietnam.ttu.edu""/>"), "match is not across consecutive references – first condensed");
            Assert.That(WikiRegexes.NamedReferencesIncludingCondensed.Match(@"<ref name=""Shul726"">Shul, p. 726</ref>").Groups[2].Value, Is.EqualTo(@"Shul726"), "ref name is group 2");
            Assert.That(WikiRegexes.NamedReferencesIncludingCondensed.Match(@"<ref name=""Shul726"">Shul, p. 726</ref>").Groups[3].Value, Is.EqualTo(@"Shul, p. 726"), "ref value is group 3");
            Assert.That(WikiRegexes.NamedReferencesIncludingCondensed.Match(@"<ref name=""Shul726"">
Shul, p. 726    </ref>").Groups[2].Value, Is.EqualTo(@"Shul726"), "ref value doesn't include leading/trailing whitespace");
        }

        [Test]
        public void UnformattedTextTests()
        {
            ClassicAssert.IsTrue(WikiRegexes.UnformattedText.IsMatch(@"<pre>{{abc}}</pre>"));
            ClassicAssert.IsTrue(WikiRegexes.UnformattedText.IsMatch(@"<math>{{abc}}</math>"));
            ClassicAssert.IsTrue(WikiRegexes.UnformattedText.IsMatch(@"<math chem>{{abc}}</math>"));
            ClassicAssert.IsTrue(WikiRegexes.UnformattedText.IsMatch(@"<chem>{{abc}}</chem>"));
            ClassicAssert.IsTrue(WikiRegexes.UnformattedText.IsMatch(@"<nowiki>{{abc}}</nowiki>"));
            ClassicAssert.IsTrue(WikiRegexes.UnformattedText.IsMatch(@"now hello {{bye}} <pre>{now}}</pre>"));
            ClassicAssert.IsTrue(WikiRegexes.UnformattedText.IsMatch(@"<!--{{abc}}-->"));
            ClassicAssert.IsFalse(WikiRegexes.UnformattedText.IsMatch(@"<pre>{{abc}}</nowiki>"), "Does not match unbalanced tags");
        }

        [Test]
        public void AllTags()
        {
            ClassicAssert.IsTrue(WikiRegexes.AllTags.IsMatch(@"<pre>{{abc}}</pre>"));
            ClassicAssert.IsTrue(WikiRegexes.AllTags.IsMatch(@"<math>{{abc}}</math>"));
            ClassicAssert.IsTrue(WikiRegexes.AllTags.IsMatch(@"<nowiki>{{abc}}</nowiki>"));
            ClassicAssert.IsTrue(WikiRegexes.AllTags.IsMatch(@"<timeline>{{abc}}</timeline>"));
            ClassicAssert.IsTrue(WikiRegexes.AllTags.IsMatch(@"<cite>{{abc}}</cite>"));
            ClassicAssert.IsTrue(WikiRegexes.AllTags.IsMatch(@"<blockquote>{{abc}}</blockquote>"));
            ClassicAssert.IsTrue(WikiRegexes.AllTags.IsMatch(@"<BLOCKQUOTE>{{abc}}</BLOCKQUOTE>"));
            ClassicAssert.IsTrue(WikiRegexes.AllTags.IsMatch(@"<poem>{{abc}}</poem>"));
            ClassicAssert.IsTrue(WikiRegexes.AllTags.IsMatch(@"<imagemap>{{abc}}</imagemap>"));
            ClassicAssert.IsTrue(WikiRegexes.AllTags.IsMatch(@"<noinclude>{{abc}}</noinclude>"));
            ClassicAssert.IsTrue(WikiRegexes.AllTags.IsMatch(@"<includeonly>{{abc}}</includeonly>"));
            ClassicAssert.IsTrue(WikiRegexes.AllTags.IsMatch(@"<onlyinclude>{{abc}}</onlyinclude>"));
            ClassicAssert.IsTrue(WikiRegexes.AllTags.IsMatch(@"<hiero>abc</hiero>"));
            ClassicAssert.IsTrue(WikiRegexes.AllTags.IsMatch(@"<score>{a,, c, e, a, c e a c' e' a' c'' e'' a'' c''' e''' g''' \bar</score>"));

            ClassicAssert.IsTrue(WikiRegexes.AllTags.IsMatch(@"now hello {{bye}} <pre>{now}}</pre>"));
            ClassicAssert.IsFalse(WikiRegexes.AllTags.IsMatch(@"<!--{{abc}}-->"));
            ClassicAssert.IsFalse(WikiRegexes.AllTags.IsMatch(@"<pre>{{abc}}</nowiki>"), "Does not match unbalanced tags");

            ClassicAssert.IsTrue(WikiRegexes.AllTags.IsMatch(@"<pre>{{abc}}</pre>"));
            ClassicAssert.IsTrue(WikiRegexes.AllTags.IsMatch(@"<code>{{abc}}</code>"));
            ClassicAssert.IsTrue(WikiRegexes.AllTags.IsMatch(@"<source lang=xml>{{abc}}</source>"));
            ClassicAssert.IsTrue(WikiRegexes.AllTags.IsMatch(@"<syntaxhighlight lang=xml>{{abc}}</syntaxhighlight>"));
            ClassicAssert.IsTrue(WikiRegexes.AllTags.IsMatch(@"<source>{{abc}}</source>"));
            ClassicAssert.IsTrue(WikiRegexes.AllTags.IsMatch(@"<syntaxhighlight>{{abc}}</syntaxhighlight>"));
            ClassicAssert.IsTrue(WikiRegexes.AllTags.IsMatch(@"now hello {{bye}} <pre>{now}}</pre>"));
            ClassicAssert.IsTrue(WikiRegexes.AllTags.IsMatch(@"<math>{{abc}}</math>"));
            ClassicAssert.IsTrue(WikiRegexes.AllTags.IsMatch(@"<chem>{{abc}}</chem>"));
            ClassicAssert.IsTrue(WikiRegexes.AllTags.IsMatch(@"<math chem>{{abc}}</math>"));

            ClassicAssert.IsTrue(WikiRegexes.AllTags.IsMatch(@"<pre>now <br> now </pre>"));

            ClassicAssert.IsTrue(WikiRegexes.AllTags.IsMatch(@"< pre >{{abc}}< / pre >"));
            ClassicAssert.IsTrue(WikiRegexes.AllTags.IsMatch(@"< pre > {{abc}} < / pre >"));
            ClassicAssert.IsTrue(WikiRegexes.AllTags.IsMatch(@"< pre >
{{abc}}
< / pre >"));
            ClassicAssert.IsTrue(WikiRegexes.AllTags.IsMatch(@"<
 pre > {{abc}} < / pre
 >"));
            Assert.That(WikiRegexes.AllTags.Match(@"<nowiki>now <math>{{abc}}</math> now </nowiki>").Value, Is.EqualTo("<nowiki>now <math>{{abc}}</math> now </nowiki>"));
        }

        [Test]
        public void SourceCodeTests()
        {
            ClassicAssert.IsTrue(WikiRegexes.SourceCode.IsMatch(@"<code>{{abc}}</code>"));
            ClassicAssert.IsTrue(WikiRegexes.SourceCode.IsMatch(@"<source lang=xml>{{abc}}</source>"));
            ClassicAssert.IsTrue(WikiRegexes.SourceCode.IsMatch(@"<syntaxhighlight lang=xml>{{abc}}</syntaxhighlight>"));
            ClassicAssert.IsTrue(WikiRegexes.SourceCode.IsMatch(@"<syntaxhighlight lang=""xml"">{{abc}}</syntaxhighlight>"));
            ClassicAssert.IsTrue(WikiRegexes.SourceCode.IsMatch(@"<syntaxhighlight lang=""javascript"">\r\n var x; //defines the variable x</syntaxhighlight>"));
            ClassicAssert.IsTrue(WikiRegexes.SourceCode.IsMatch(@"<source>{{abc}}</source>"));
            ClassicAssert.IsTrue(WikiRegexes.SourceCode.IsMatch(@"<syntaxhighlight>{{abc}}</syntaxhighlight>"));
            ClassicAssert.IsTrue(WikiRegexes.SourceCode.IsMatch(@"<tt>{{abc}}</tt>"));

            ClassicAssert.IsFalse(WikiRegexes.SourceCode.IsMatch(@"<math>{{abc}}</math>"));
            ClassicAssert.IsFalse(WikiRegexes.SourceCode.IsMatch(@"<pre>{{abc}}</pre>"));
        }

        [Test]
        public void MathPreSourceCodeTests()
        {
            ClassicAssert.IsTrue(WikiRegexes.MathPreSourceCodeComments.IsMatch(@"<pre>{{abc}}</pre>"));
            ClassicAssert.IsTrue(WikiRegexes.MathPreSourceCodeComments.IsMatch(@"<!--{{abc}}-->"));
            ClassicAssert.IsTrue(WikiRegexes.MathPreSourceCodeComments.IsMatch(@"<code>{{abc}}</code>"));
            ClassicAssert.IsTrue(WikiRegexes.MathPreSourceCodeComments.IsMatch(@"<source lang=xml>{{abc}}</source>"));
            ClassicAssert.IsTrue(WikiRegexes.MathPreSourceCodeComments.IsMatch(@"<syntaxhighlight lang=xml>{{abc}}</syntaxhighlight>"));
            ClassicAssert.IsTrue(WikiRegexes.MathPreSourceCodeComments.IsMatch(@"<syntaxhighlight lang=""xml"">{{abc}}</syntaxhighlight>"));
            ClassicAssert.IsTrue(WikiRegexes.SourceCode.IsMatch(@"<syntaxhighlight lang=""javascript"">\r\n var x; //defines the variable x</syntaxhighlight>"));
            ClassicAssert.IsTrue(WikiRegexes.MathPreSourceCodeComments.IsMatch(@"<source>{{abc}}</source>"));
            ClassicAssert.IsTrue(WikiRegexes.MathPreSourceCodeComments.IsMatch(@"<syntaxhighlight>{{abc}}</syntaxhighlight>"));
            ClassicAssert.IsTrue(WikiRegexes.MathPreSourceCodeComments.IsMatch(@"now hello {{bye}} <pre>{now}}</pre>"));
            ClassicAssert.IsTrue(WikiRegexes.MathPreSourceCodeComments.IsMatch(@"<math>{{abc}}</math>"));
            ClassicAssert.IsTrue(WikiRegexes.MathPreSourceCodeComments.IsMatch(@"<chem>{{abc}}</chem>"));
            ClassicAssert.IsTrue(WikiRegexes.MathPreSourceCodeComments.IsMatch(@"<math chem>{{abc}}</math>"));
            ClassicAssert.IsTrue(WikiRegexes.SourceCode.IsMatch(@"<tt>{{abc}}</tt>"));

            ClassicAssert.IsFalse(WikiRegexes.MathPreSourceCodeComments.IsMatch(@"<nowiki>{{abc}}</nowiki>"));
        }

        [Test]
        public void WikiLinksOnly()
        {
            TestMatch(WikiRegexes.WikiLinksOnly, "[[foo]]", "[[foo]]");
            TestMatch(WikiRegexes.WikiLinksOnly, "[[foo#bar]]", "[[foo#bar]]");
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

            // Assert.AreEqual("Test", WikiRegexes.WikiLinksOnly.Matches("[[Test]]")[0].Groups[1].Value);
        }

        [Test]
        public void WikiLinksOnlyPossiblePipe()
        {
            TestMatch(WikiRegexes.WikiLinksOnlyPossiblePipe, "[[foo]]", "[[foo]]");
            TestMatch(WikiRegexes.WikiLinksOnlyPossiblePipe, "[[:foo]]", "[[:foo]]");
            TestMatch(WikiRegexes.WikiLinksOnlyPossiblePipe, "[[foo#bar]]", "[[foo#bar]]");
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

            Assert.That(WikiRegexes.WikiLinksOnlyPossiblePipe.Match("[[foo|bar]]").Groups[1].Value, Is.EqualTo("foo"));
            Assert.That(WikiRegexes.WikiLinksOnlyPossiblePipe.Match("[[foo]]").Groups[1].Value, Is.EqualTo("foo"));
            Assert.That(WikiRegexes.WikiLinksOnlyPossiblePipe.Match("[[foo smith:the great|bar]]").Groups[1].Value, Is.EqualTo("foo smith:the great"));
            Assert.That(WikiRegexes.WikiLinksOnlyPossiblePipe.Match("[[foo|bar here]]").Groups[2].Value, Is.EqualTo("bar here"));
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
        public void TargetLessLinkTests()
        {
            TestMatch(WikiRegexes.TargetLessLink, "[[|foo]]", "[[|foo]]");
            TestMatch(WikiRegexes.TargetLessLink, "[[|Foo]]", "[[|Foo]]");
            TestMatch(WikiRegexes.TargetLessLink, "[[|Foo ]]", "[[|Foo ]]");
            TestMatch(WikiRegexes.TargetLessLink, "[[|foo bar]]", "[[|foo bar]]");
            TestMatch(WikiRegexes.TargetLessLink, "[[|linktext]]", "[[|linktext]]");
            TestMatch(WikiRegexes.TargetLessLink, "[[|linktext (foo)]]", "[[|linktext (foo)]]");
            TestMatch(WikiRegexes.TargetLessLink, "[[|link.text (foo)]]", "[[|link.text (foo)]]");
            TestMatch(WikiRegexes.TargetLessLink, "[[|link!?]]", "[[|link!?]]");
            TestMatch(WikiRegexes.TargetLessLink, "[[|Foo (1971–1985)]]", "[[|Foo (1971–1985)]]");
            TestMatch(WikiRegexes.TargetLessLink, "[[|Victor + Victoria]]", "[[|Victor + Victoria]]");
            TestMatch(WikiRegexes.TargetLessLink, "[[foo|bar]]", false);
        }

        [Test]
        public void DoublePipeLinkTests()
        {
            TestMatch(WikiRegexes.DoublePipeLink, "[[text|text2|text3]]", "[[text|text2|text3]]");
            TestMatch(WikiRegexes.DoublePipeLink, "[[Text|Text2|Text3]]", "[[Text|Text2|Text3]]");
            TestMatch(WikiRegexes.DoublePipeLink, "[[text||text3]]", "[[text||text3]]");
            TestMatch(WikiRegexes.DoublePipeLink, "[[text|(text2)|text3]]", "[[text|(text2)|text3]]");
            TestMatch(WikiRegexes.DoublePipeLink, "[[text (1)|text2|text3]]", "[[text (1)|text2|text3]]");
            TestMatch(WikiRegexes.DoublePipeLink, "[[Saint-Georges-du-Bois|Saint-Georges-du-Bois|Saint-Georges-du-Bois]]", "[[Saint-Georges-du-Bois|Saint-Georges-du-Bois|Saint-Georges-du-Bois]]");
            TestMatch(WikiRegexes.DoublePipeLink, "[[text, bar|text2, bar2|text3, bar3]]", "[[text, bar|text2, bar2|text3, bar3]]");
            TestMatch(WikiRegexes.DoublePipeLink, "[[First Australian Imperial Force|Australian Imperial Force|A.I.F.]]", "[[First Australian Imperial Force|Australian Imperial Force|A.I.F.]]");
            TestMatch(WikiRegexes.DoublePipeLink, "[[Ric (f)|Ric (f)|o.g.]]", "[[Ric (f)|Ric (f)|o.g.]]");
            TestMatch(WikiRegexes.DoublePipeLink, "[[text|text2|text3 & text4]]", "[[text|text2|text3 & text4]]");
            TestMatch(WikiRegexes.DoublePipeLink, "[[Bairnsdale|, Victoria|Bairnsdale]]", "[[Bairnsdale|, Victoria|Bairnsdale]]");
            TestMatch(WikiRegexes.DoublePipeLink, "[[Bairnsdale!|Victoria|Bairnsdale]]", "[[Bairnsdale!|Victoria|Bairnsdale]]");
            TestMatch(WikiRegexes.DoublePipeLink, "[[Bairnsdale!|Victoria?|Bairns, dale]]", "[[Bairnsdale!|Victoria?|Bairns, dale]]");
            TestMatch(WikiRegexes.DoublePipeLink, "[[Foo (1971–1985)|Victoria?|Bairns, dale]]", "[[Foo (1971–1985)|Victoria?|Bairns, dale]]");
            TestMatch(WikiRegexes.DoublePipeLink, "[[Foo (1971–1985)|Victor + Victoria|Bairns, dale]]", "[[Foo (1971–1985)|Victor + Victoria|Bairns, dale]]");
            TestMatch(WikiRegexes.DoublePipeLink, "[[test's test|test1|test2]]", "[[test's test|test1|test2]]");
            TestMatch(WikiRegexes.DoublePipeLink, @"[[Vlach ""Roman Legion""|The Roman Legion|The so called ""Roman Legion""]]");

            TestMatch(WikiRegexes.DoublePipeLink, "[[text|foo bar]]", false);
            TestMatch(WikiRegexes.DoublePipeLink, "[[|linktext]]", false);
            TestMatch(WikiRegexes.DoublePipeLink, "[[foo|bar]]", false);
            TestMatch(WikiRegexes.DoublePipeLink, "[[text!@?()&123456789|foo bar]]", false);
            TestMatch(WikiRegexes.DoublePipeLink, @"[[""text""|foo bar]]", false);
        }

        [Test]
        public void WikiLinkTests()
        {
            Assert.That(WikiRegexes.WikiLink.Match(@"[[foo]]").Groups[1].Value, Is.EqualTo(@"foo"));
            Assert.That(WikiRegexes.WikiLink.Match(@"[[foo|bar]]").Groups[1].Value, Is.EqualTo(@"foo"));
            Assert.That(WikiRegexes.WikiLink.Match(@"[[foo bar]]").Groups[1].Value, Is.EqualTo(@"foo bar"));
            Assert.That(WikiRegexes.WikiLink.Match(@"[[Foo]]").Groups[1].Value, Is.EqualTo(@"Foo"));
            Assert.That(WikiRegexes.WikiLink.Match(@"[[foo bar|word here]]").Groups[1].Value, Is.EqualTo(@"foo bar"));
            TestMatch(WikiRegexes.WikiLink, "[[foo|'''bar''']]");
            TestMatch(WikiRegexes.WikiLink, "[[foo|''bar'']]");
        }

        [Test]
        public void PipedWikiLink()
        {
            TestMatch(WikiRegexes.PipedWikiLink, "[[foo|bar]]");
            TestMatch(WikiRegexes.PipedWikiLink, "a [[foo boo | bar bar ]] !one", "[[foo boo | bar bar ]]");
            TestMatch(WikiRegexes.PipedWikiLink, "[[foo]]", false);
            TestMatch(WikiRegexes.PipedWikiLink, "[[foo|bar|]]");

            // shouldn't eat too much
            TestMatch(WikiRegexes.PipedWikiLink, "[[foo]] [[foo|bar]]", "[[foo|bar]]");
            TestMatch(WikiRegexes.PipedWikiLink, "[[foo|bar]] [[foo]]", "[[foo|bar]]");

            TestMatch(WikiRegexes.PipedWikiLink, "[[foo|\r\nbar]]", false);
            TestMatch(WikiRegexes.PipedWikiLink, "[[foo\r\n|bar]]", false);
            TestMatch(WikiRegexes.PipedWikiLink, "[[foo]] | bar]]", false);
            TestMatch(WikiRegexes.PipedWikiLink, "[[foo | [[bar]]", false);

            Assert.That(WikiRegexes.PipedWikiLink.Match("[[foo|bar]]").Groups[1].Value, Is.EqualTo("foo"));
            Assert.That(WikiRegexes.PipedWikiLink.Match("[[foo|bar]]").Groups[2].Value, Is.EqualTo("bar"));
        }

        [Test]
        public void UnPipedWikiLink()
        {
            TestMatch(WikiRegexes.UnPipedWikiLink, "[[foo]]");
            TestMatch(WikiRegexes.UnPipedWikiLink, "a [[foo boo ]] !one", "[[foo boo ]]");
            TestMatch(WikiRegexes.UnPipedWikiLink, "[[foo|bar]]", false);
            Assert.That(WikiRegexes.UnPipedWikiLink.Match("[[foo]]").Groups[1].Value, Is.EqualTo("foo"));
            Assert.That(WikiRegexes.UnPipedWikiLink.Match("[[foo bar]]").Groups[1].Value, Is.EqualTo("foo bar"));
        }

        [Test]
        public void Blockquote()
        {
            // one line
            TestMatch(WikiRegexes.Blockquote, "<blockquote>foo bar< /blockquote>", "<blockquote>foo bar< /blockquote>");

            TestMatch(WikiRegexes.Blockquote, "<blockquote style=x>foo bar< /blockquote>", "<blockquote style=x>foo bar< /blockquote>");

            // multiple lines
            TestMatch(WikiRegexes.Blockquote, "< Blockquote >foo\r\nbar</ BLOCKQUOTE>", "< Blockquote >foo\r\nbar</ BLOCKQUOTE>");
        }

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

            Assert.That(WikiRegexes.UseDatesTemplate.Match(@"{{use mdy dates}}").Groups[2].Value, Is.EqualTo("use mdy dates"));
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

            TestMatch(WikiRegexes.ISODatesQuick, @"on 2009-12-11 a", true);
            TestMatch(WikiRegexes.ISODatesQuick, @"on 2009-12-01 a", true);
            TestMatch(WikiRegexes.ISODatesQuick, @"on 2009-12-21 a", true);
            TestMatch(WikiRegexes.ISODatesQuick, @"on 2009-08-31 a", true);
            TestMatch(WikiRegexes.ISODatesQuick, @"BC]] |date=2003-10-19 }}", true);

            TestMatch(WikiRegexes.ISODatesQuick, @"on 1009-12-11 a", false);
            TestMatch(WikiRegexes.ISODatesQuick, @"on 2209-12-11 a", false);
            TestMatch(WikiRegexes.ISODatesQuick, @"on 2009-14-11 a", false);
            TestMatch(WikiRegexes.ISODatesQuick, @"on 2009-2-11 a", false);
            TestMatch(WikiRegexes.ISODatesQuick, @"on 2009-08-33 a", false);
            TestMatch(WikiRegexes.ISODatesQuick, @"on 2009/08/31 a", false);
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
        public void GalleryTagTests()
        {
            string g1 = @"< gallery >
File:foo.JPG
Image:Bar.png|description
</ gallery>", g2 = @"<GALLERY>
File:foo.JPG
Image:Bar.png|description
</ gallery>", g3 = @"<gallery param=""value"">
File:foo.JPG
Image:Bar.png|description
</ gallery>";
            TestMatch(WikiRegexes.GalleryTag, g1, g1);
            TestMatch(WikiRegexes.GalleryTag, g2, g2);
            TestMatch(WikiRegexes.GalleryTag, g3, g3);
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
            RegexAssert.Matches("{{foo {{bar}} {{bar2}} end}}", WikiRegexes.NestedTemplates, "{{foo {{bar}} {{bar2}} end}}");
            RegexAssert.Matches("{{foo {bar} end}}", WikiRegexes.NestedTemplates, "{{foo {bar} end}}");

            RegexAssert.Matches("{{ foo |bar}}", WikiRegexes.NestedTemplates, "{{ foo |bar}}");
            RegexAssert.Matches("{{foo<!--comm-->|bar}}", WikiRegexes.NestedTemplates, "{{foo<!--comm-->|bar}}");
            RegexAssert.Matches("", WikiRegexes.NestedTemplates, "{{foo");
            RegexAssert.Matches("{{{foo}}}", WikiRegexes.NestedTemplates, "{{{foo}}}");
        }

        [Test]
        public void TemplateName()
        {
            ClassicAssert.IsTrue(WikiRegexes.TemplateName.Match(@"{{Start date and age|1833|7|11}}").Groups[1].Value == "Start date and age");

            // whitespace handling
            ClassicAssert.IsTrue(WikiRegexes.TemplateName.Match(@"{{ Start date and age |1833|7|11}}").Groups[1].Value == "Start date and age");
            ClassicAssert.IsTrue(WikiRegexes.TemplateName.Match(@"{{
Start date and age
|1833|7|11}}").Groups[1].Value == "Start date and age");

            // embedded comments
            ClassicAssert.IsTrue(WikiRegexes.TemplateName.Match(@"{{start date and age <!--comm--> |1833|7|11}}").Groups[1].Value == "start date and age");
            ClassicAssert.IsTrue(WikiRegexes.TemplateName.Match(@"{{start date and age <!--comm-->}}").Groups[1].Value == "start date and age");

            // works on part templates
            ClassicAssert.IsTrue(WikiRegexes.TemplateName.Match(@"{{Start date and age|1833|7|").Groups[1].Value == "Start date and age");
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
        public void Headings()
        {
            RegexAssert.NoMatch(WikiRegexes.Headings, "");
            RegexAssert.IsMatch(WikiRegexes.Headings, "=Foo=");
            RegexAssert.IsMatch(WikiRegexes.Headings, "=Foo=<!--comm-->");
            RegexAssert.IsMatch(WikiRegexes.Headings, "==Foo==");
            RegexAssert.IsMatch(WikiRegexes.Headings, "======Foo======");
            Assert.That(WikiRegexes.Headings.Match("======Foo======").Groups[1].Value, Is.EqualTo("Foo"));
            Assert.That(WikiRegexes.Headings.Match("== Foo == ").Groups[1].Value, Is.EqualTo("Foo"));

            RegexAssert.IsMatch(WikiRegexes.Headings, "==Foo=", "matches unbalanced headings");
            RegexAssert.IsMatch(WikiRegexes.Headings, @"=='''Header with bold'''==<br/>
");
        }

        [Test]
        public void SpecificLevel2Headings()
        {
            RegexAssert.IsMatch(WikiRegexes.ReferencesHeader, "==References==");
            RegexAssert.IsMatch(WikiRegexes.ReferencesHeader, "== References ==");
            RegexAssert.IsMatch(WikiRegexes.ReferencesHeader, "==references==");
            RegexAssert.IsMatch(WikiRegexes.NotesHeader, "==Notes==");
            RegexAssert.IsMatch(WikiRegexes.NotesHeader, "== Notes ==");
            RegexAssert.IsMatch(WikiRegexes.NotesHeader, "==notes==");
            RegexAssert.IsMatch(WikiRegexes.NotesHeader, "== notes ==");
            RegexAssert.IsMatch(WikiRegexes.ExternalLinksHeader, "==External links==");
            RegexAssert.IsMatch(WikiRegexes.ExternalLinksHeader, "== External links ==");
            RegexAssert.IsMatch(WikiRegexes.ExternalLinksHeader, "==External link==");
            RegexAssert.IsMatch(WikiRegexes.ExternalLinksHeader, "== External link ==");
            RegexAssert.IsMatch(WikiRegexes.SeeAlso, "==See also==");
            RegexAssert.IsMatch(WikiRegexes.SeeAlso, "== See also ==");
        }

        [Test]
        public void Refs()
        {
            RegexAssert.Matches(WikiRegexes.Refs, "<ref>foo</ref>", "<ref>foo</ref>");
            RegexAssert.Matches(WikiRegexes.Refs, "<ref>foo<br></ref>", "<ref>foo<br></ref>");
            RegexAssert.Matches(WikiRegexes.Refs, "<ref>foo<br>bar</ref>", "<ref>foo<br>bar</ref>");

            RegexAssert.IsMatch(WikiRegexes.Refs, @"<ref>{{cite web
|url=http://www.h.com/.php?tmi=5177|title=Season-by-season record |accessdate = 2008-12-01}}</ref>");

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

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_10#.3Cp.3E_deletion_in_references_and_notes
            RegexAssert.Matches(WikiRegexes.Refs, "<ref>foo<!-- bar --></ref>", "<ref>foo<!-- bar --></ref>");
            // shouldn't eat too much
            RegexAssert.Matches(WikiRegexes.Refs, "<ref>foo<!-- bar --></ref> <ref>foo</ref>", "<ref>foo<!-- bar --></ref>", "<ref>foo</ref>");

            TestMatches(WikiRegexes.Refs, @"Article<ref>A</ref> <ref>B</ref> <ref>C</ref> <ref>B</ref> <ref>E</ref>", 5);

            // this is why the <DEPTH> business is needed in WikiRegexes.Refs
            RegexAssert.Matches(new Regex(WikiRegexes.Refs + @"\."), "Foo.<ref>bar</ref>. The next Foo.<ref>bar <br> other</ref> The next Foo.<ref>bar</ref> The next Foo.<ref>bar</ref> The nextFoo<ref>bar2</ref>. The next", "<ref>bar</ref>.", "<ref>bar2</ref>.");
        }

        [Test]
        public void UnnamedReferences()
        {
            RegexAssert.IsMatch(WikiRegexes.UnnamedReferences, @"<ref>Foo</ref>");
            RegexAssert.IsMatch(WikiRegexes.UnnamedReferences, @"< ref >Foo< / ref >");
            RegexAssert.IsMatch(WikiRegexes.UnnamedReferences, @"<ref> Foo </ref>");
            RegexAssert.IsMatch(WikiRegexes.UnnamedReferences, @"<ref>
Foo
</ref>");
            RegexAssert.IsMatch(WikiRegexes.UnnamedReferences, @"<ref>Foo<small>bar</small>Here</ref>");
            Assert.That(WikiRegexes.UnnamedReferences.Match("<ref>Foo</ref>").Groups[1].Value, Is.EqualTo("Foo"));
        }

        [Test]
        public void RefsGrouped()
        {
            RegexAssert.Matches(WikiRegexes.RefsGrouped, "<ref group=X>foo</ref>", "<ref group=X>foo</ref>");
            RegexAssert.Matches(WikiRegexes.RefsGrouped, "<ref group=X>foo<br></ref>", "<ref group=X>foo<br></ref>");
            RegexAssert.Matches(WikiRegexes.RefsGrouped, "<ref group=X>foo<br>bar</ref>", "<ref group=X>foo<br>bar</ref>");
            const string M1 = @"<ref group=X>{{cite web
|url=http://www.h.com/.php?tmi=5177|title=Season-by-season record |accessdate = 2008-12-01}}</ref>";
            RegexAssert.Matches(WikiRegexes.RefsGrouped, M1, M1);

            RegexAssert.Matches(WikiRegexes.RefsGrouped, "<REF group=\"foo\" >bar</ref >", "<REF group=\"foo\" >bar</ref >");
            RegexAssert.Matches(WikiRegexes.RefsGrouped, "<REF  group=foo>bar< /ref>", "<REF  group=foo>bar< /ref>");
            RegexAssert.Matches(WikiRegexes.RefsGrouped, "<ReF Group=foo/>", "<ReF Group=foo/>");
            RegexAssert.Matches(WikiRegexes.RefsGrouped, "<ReF Group = 'foo'/>", "<ReF Group = 'foo'/>");
            RegexAssert.Matches(WikiRegexes.RefsGrouped, "<ReF Group = \"foo\"/>", "<ReF Group = \"foo\"/>");
            RegexAssert.Matches(WikiRegexes.RefsGrouped, "< ref group=X>foo</ ref>", "< ref group=X>foo</ ref>");
            RegexAssert.Matches(WikiRegexes.RefsGrouped, @"<ref group= ""foo/bar""/>", @"<ref group= ""foo/bar""/>");
            RegexAssert.Matches(WikiRegexes.RefsGrouped, @"<ref group= ""foo/bar"">a</ref>", @"<ref group= ""foo/bar"">a</ref>");
            RegexAssert.Matches(WikiRegexes.RefsGrouped, "<ref name=A group=foo/>", "<ref name=A group=foo/>");
            RegexAssert.Matches(WikiRegexes.RefsGrouped, @"<ref group=""ref"" name=""Cannon""/>", @"<ref group=""ref"" name=""Cannon""/>");
            RegexAssert.Matches(WikiRegexes.RefsGrouped, "<ref group=X name=Y>foo</ref>", "<ref group=X name=Y>foo</ref>");
            RegexAssert.Matches(WikiRegexes.RefsGrouped, "<ref name=Y group=X>foo</ref>", "<ref name=Y group=X>foo</ref>");

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_10#.3Cp.3E_deletion_in_references_and_notes
            RegexAssert.Matches(WikiRegexes.RefsGrouped, "<ref group=X>foo<!-- bar --></ref>", "<ref group=X>foo<!-- bar --></ref>");
            // shouldn't eat too much
            RegexAssert.Matches(WikiRegexes.RefsGrouped, "<ref group=X>foo<!-- bar --></ref> <ref group=Y>foo</ref>", "<ref group=X>foo<!-- bar --></ref>", "<ref group=Y>foo</ref>");

            // this is why the <DEPTH> business is needed in WikiRegexes.Refs
            RegexAssert.Matches(new Regex(WikiRegexes.Refs + @"\."), "Foo.<ref group=X>bar</ref>. The next Foo.<ref>bar <br> other</ref> The next Foo.<ref>bar</ref> The next Foo.<ref>bar</ref> The nextFoo<ref group=Y>bar2</ref>. The next", "<ref group=X>bar</ref>.", "<ref group=Y>bar2</ref>.");

            RegexAssert.NoMatch(WikiRegexes.RefsGrouped, "<ref>foo</ref>", "<ref>foo</ref>");
            RegexAssert.NoMatch(WikiRegexes.RefsGrouped, "<ref>foo<br></ref>", "<ref>foo<br></ref>");
            RegexAssert.NoMatch(WikiRegexes.RefsGrouped, "<ref>foo<br>bar</ref>", "<ref>foo<br>bar</ref>");

            RegexAssert.NoMatch(WikiRegexes.RefsGrouped, @"<ref>{{cite web
|url=http://www.h.com/.php?tmi=5177|title=Season-by-season record |accessdate = 2008-12-01}}</ref>");

            RegexAssert.NoMatch(WikiRegexes.RefsGrouped, "<REF NAME=\"foo\" >bar</ref >", "<REF NAME=\"foo\" >bar</ref >");
            RegexAssert.NoMatch(WikiRegexes.RefsGrouped, "<REF  NAME=foo>bar< /ref>", "<REF  NAME=foo>bar< /ref>");
            RegexAssert.NoMatch(WikiRegexes.RefsGrouped, "<ReF Name=foo/>", "<ReF Name=foo/>");
            RegexAssert.NoMatch(WikiRegexes.RefsGrouped, "<ReF Name = 'foo'/>", "<ReF Name = 'foo'/>");
            RegexAssert.NoMatch(WikiRegexes.RefsGrouped, "<ReF Name = \"foo\"/>", "<ReF Name = \"foo\"/>");
            RegexAssert.NoMatch(WikiRegexes.RefsGrouped, "< ref>foo</ ref>", "< ref>foo</ ref>");
            RegexAssert.NoMatch(WikiRegexes.RefsGrouped, @"<ref name= ""foo/bar""/>", @"<ref name= ""foo/bar""/>");
            RegexAssert.NoMatch(WikiRegexes.RefsGrouped, @"<ref name= ""foo/bar"">a</ref>", @"<ref name= ""foo/bar"">a</ref>");

            RegexAssert.NoMatch(WikiRegexes.RefsGrouped, "<refname=foo>bar</ref>");
            RegexAssert.NoMatch(WikiRegexes.RefsGrouped, "<refname=foo/>");

            RegexAssert.Matches(WikiRegexes.RefsGrouped, "<ref group=a name=foo/>", "<ref group=a name=foo/>");
            RegexAssert.Matches(WikiRegexes.RefsGrouped, "<ref name=foo group=a />", "<ref name=foo group=a />");

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_10#.3Cp.3E_deletion_in_references_and_notes
            RegexAssert.NoMatch(WikiRegexes.RefsGrouped, "<ref>foo<!-- bar --></ref>", "<ref>foo<!-- bar --></ref>");
            // shouldn't eat too much
            RegexAssert.NoMatch(WikiRegexes.RefsGrouped, "<ref>foo<!-- bar --></ref> <ref>foo</ref>");

            // this is why the <DEPTH> business is needed in WikiRegexes.Refs
            RegexAssert.Matches(new Regex(WikiRegexes.RefsGrouped + @"\."), "Foo.<ref group=X>bar</ref>. The next Foo.<ref group=X>bar <br> other</ref> The next Foo.<ref group=X>bar</ref> The next Foo.<ref group=X>bar</ref> The nextFoo<ref group=X>bar2</ref>. The next",
                                @"<ref group=X>bar</ref>.", @"<ref group=X>bar2</ref>.");
        }

        [Test]
        public void Small()
        {
            RegexAssert.Matches(WikiRegexes.Small, "<small>foo</small>", "<small>foo</small>");
            Assert.That(WikiRegexes.Small.Match("<small>foo</small>").Groups[1].Value, Is.EqualTo("foo"));
            RegexAssert.Matches(WikiRegexes.Small, "<small  >foo</small >", "<small  >foo</small >");
            RegexAssert.Matches(WikiRegexes.Small, @"<small>
foo
</small>", @"<small>
foo
</small>");

            RegexAssert.Matches(WikiRegexes.Small, "<SMALL>foo</SMALL>", "<SMALL>foo</SMALL>");
            RegexAssert.Matches(WikiRegexes.Small, "<small>a<small>foo</small>b</small>", "<small>a<small>foo</small>b</small>");
            RegexAssert.Matches(WikiRegexes.Small, @"<small>..<small>...</small>", @"<small>...</small>");
            RegexAssert.Matches(WikiRegexes.Small, "<small>foo<br>bar</small>", "<small>foo<br>bar</small>");
        }

        [Test]
        public void Big()
        {
            RegexAssert.Matches(WikiRegexes.Big, "<big>foo</big>", "<big>foo</big>");
            RegexAssert.Matches(WikiRegexes.Big, "<big  >foo</big >", "<big  >foo</big >");
            RegexAssert.Matches(WikiRegexes.Big, @"<big>
foo
</big>", @"<big>
foo
</big>");

            RegexAssert.Matches(WikiRegexes.Big, "<big>foo</big>", "<big>foo</big>");
            RegexAssert.Matches(WikiRegexes.Big, "<big>a<big>foo</big>b</big>", "<big>a<big>foo</big>b</big>");
            RegexAssert.Matches(WikiRegexes.Big, @"<big>..<big>...</big>", @"<big>...</big>");
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
            RegexAssert.Matches(WikiRegexes.ExternalLinks, "//google.co.uk", "//google.co.uk");

            // protocol coverage
            RegexAssert.Matches(WikiRegexes.ExternalLinks, "http://foo.com/asdfasdf/asdf.htm", "http://foo.com/asdfasdf/asdf.htm");
            RegexAssert.Matches(WikiRegexes.ExternalLinks, "https://www.foo.com/asdfasdf/asdf.htm", "https://www.foo.com/asdfasdf/asdf.htm");
            RegexAssert.Matches(WikiRegexes.ExternalLinks, "ftp://www.foo.com/asdfasdf/asdf.htm", "ftp://www.foo.com/asdfasdf/asdf.htm");
            RegexAssert.Matches(WikiRegexes.ExternalLinks, "mailto://www.foo.com/asdfasdf/asdf.htm", "mailto://www.foo.com/asdfasdf/asdf.htm");
            RegexAssert.Matches(WikiRegexes.ExternalLinks, "irc://www.foo.com/asdfasdf/asdf.htm", "irc://www.foo.com/asdfasdf/asdf.htm");
            RegexAssert.Matches(WikiRegexes.ExternalLinks, "gopher://www.foo.com/asdfasdf/asdf.htm", "gopher://www.foo.com/asdfasdf/asdf.htm");
            RegexAssert.Matches(WikiRegexes.ExternalLinks, "telnet://www.foo.com/asdfasdf/asdf.htm", "telnet://www.foo.com/asdfasdf/asdf.htm");
            RegexAssert.Matches(WikiRegexes.ExternalLinks, "nntp://www.foo.com/asdfasdf/asdf.htm", "nntp://www.foo.com/asdfasdf/asdf.htm");
            RegexAssert.Matches(WikiRegexes.ExternalLinks, "worldwind://www.foo.com/asdfasdf/asdf.htm", "worldwind://www.foo.com/asdfasdf/asdf.htm");
            RegexAssert.Matches(WikiRegexes.ExternalLinks, "news://www.foo.com/asdfasdf/asdf.htm", "news://www.foo.com/asdfasdf/asdf.htm");
            RegexAssert.Matches(WikiRegexes.ExternalLinks, "svn://www.foo.com/asdfasdf/asdf.htm", "svn://www.foo.com/asdfasdf/asdf.htm");

            RegexAssert.Matches(WikiRegexes.ExternalLinks, "lol http://www.google.co.uk lol", "http://www.google.co.uk");

            RegexAssert.Matches(WikiRegexes.ExternalLinks, "[http://www.google.co.uk]", "[http://www.google.co.uk]");
            RegexAssert.Matches(WikiRegexes.ExternalLinks, "[http://www.google.co.uk google]", "[http://www.google.co.uk google]");

            RegexAssert.Matches(WikiRegexes.ExternalLinks, "lol [http://www.google.co.uk] lol", "[http://www.google.co.uk]");
            RegexAssert.Matches(WikiRegexes.ExternalLinks, "lol [http://www.google.co.uk google] lol", "[http://www.google.co.uk google]");

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_14#Regex_problem
            RegexAssert.Matches(WikiRegexes.ExternalLinks, "http://www.google.co.uk google}}", "http://www.google.co.uk");
            RegexAssert.Matches(WikiRegexes.ExternalLinks, "http://www.google.co.uk}}", "http://www.google.co.uk");

            RegexAssert.Matches(WikiRegexes.ExternalLinks, @"date=April 2010|url=http://w/010111a.html}}", "http://w/010111a.html");
            RegexAssert.Matches(WikiRegexes.ExternalLinks, @"date=April 2010|url=http://w/010111a.html|location=London}}", "http://w/010111a.html");
            RegexAssert.Matches(WikiRegexes.ExternalLinks, @"date=April 2010|url=https://w/010111a.html|location=London}}", "https://w/010111a.html");

            // incorrect brackets
            RegexAssert.Matches(WikiRegexes.ExternalLinks, "lol [http://www.google.co.uk lol", "http://www.google.co.uk");

            RegexAssert.NoMatch(WikiRegexes.ExternalLinks, "Google");

            // protocol is group 1
            Assert.That(WikiRegexes.ExternalLinks.Match(@"http://google.co.uk").Groups[1].Value, Is.EqualTo("http"));
            Assert.That(WikiRegexes.ExternalLinks.Match(@"https://google.co.uk").Groups[1].Value, Is.EqualTo("https"));
            Assert.That(WikiRegexes.ExternalLinks.Match(@"svn://google.co.uk Google}}").Groups[1].Value, Is.EqualTo("svn"));
            Assert.That(WikiRegexes.ExternalLinks.Match(@"Http://google.co.uk").Groups[1].Value, Is.EqualTo("Http"));
            Assert.That(WikiRegexes.ExternalLinks.Match(@"HTTP://google.co.uk").Groups[1].Value, Is.EqualTo("HTTP"));

            // not when in external link brackets
            Assert.That(WikiRegexes.ExternalLinks.Match(@"[http://google.co.uk Google]").Groups[1].Value, Is.Empty);
        }

        [Test]
        public void NonHTTPProtocols()
        {
            RegexAssert.NoMatch(WikiRegexes.NonHTTPProtocols, "http://foo.com/asdfasdf/asdf.htm");
            RegexAssert.NoMatch(WikiRegexes.NonHTTPProtocols, "https://www.foo.com/asdfasdf/asdf.htm");
            RegexAssert.IsMatch(WikiRegexes.NonHTTPProtocols, "ftp://www.foo.com/asdfasdf/asdf.htm", "ftp://www.foo.com/asdfasdf/asdf.htm");
            RegexAssert.IsMatch(WikiRegexes.NonHTTPProtocols, "mailto://www.foo.com/asdfasdf/asdf.htm", "mailto://www.foo.com/asdfasdf/asdf.htm");
            RegexAssert.IsMatch(WikiRegexes.NonHTTPProtocols, "irc://www.foo.com/asdfasdf/asdf.htm", "irc://www.foo.com/asdfasdf/asdf.htm");
            RegexAssert.IsMatch(WikiRegexes.NonHTTPProtocols, "gopher://www.foo.com/asdfasdf/asdf.htm", "gopher://www.foo.com/asdfasdf/asdf.htm");
            RegexAssert.IsMatch(WikiRegexes.NonHTTPProtocols, "telnet://www.foo.com/asdfasdf/asdf.htm", "telnet://www.foo.com/asdfasdf/asdf.htm");
            RegexAssert.IsMatch(WikiRegexes.NonHTTPProtocols, "nntp://www.foo.com/asdfasdf/asdf.htm", "nntp://www.foo.com/asdfasdf/asdf.htm");
            RegexAssert.IsMatch(WikiRegexes.NonHTTPProtocols, "worldwind://www.foo.com/asdfasdf/asdf.htm", "worldwind://www.foo.com/asdfasdf/asdf.htm");
            RegexAssert.IsMatch(WikiRegexes.NonHTTPProtocols, "news://www.foo.com/asdfasdf/asdf.htm", "news://www.foo.com/asdfasdf/asdf.htm");
            RegexAssert.IsMatch(WikiRegexes.NonHTTPProtocols, "svn://www.foo.com/asdfasdf/asdf.htm", "svn://www.foo.com/asdfasdf/asdf.htm");
            RegexAssert.IsMatch(WikiRegexes.NonHTTPProtocols, "SVN://www.foo.com/asdfasdf/asdf.htm", "SVN://www.foo.com/asdfasdf/asdf.htm");
        }

        [Test]
        public void ExternalLinksHTTPOnly()
        {
            RegexAssert.Matches(WikiRegexes.ExternalLinksHTTPOnly, "http://google.co.uk", "http://google.co.uk");
            RegexAssert.Matches(WikiRegexes.ExternalLinksHTTPOnly, "http://google-here.co.uk", "http://google-here.co.uk");
            RegexAssert.Matches(WikiRegexes.ExternalLinksHTTPOnly, "https://google.co.uk", "https://google.co.uk");
            RegexAssert.Matches(WikiRegexes.ExternalLinksHTTPOnly, "http://foo.com/asdfasdf/asdf.htm", "http://foo.com/asdfasdf/asdf.htm");
            RegexAssert.Matches(WikiRegexes.ExternalLinksHTTPOnly, "http://www.google.co.uk", "http://www.google.co.uk");

            // protocol coverage
            RegexAssert.Matches(WikiRegexes.ExternalLinksHTTPOnly, "http://foo.com/asdfasdf/asdf.htm", "http://foo.com/asdfasdf/asdf.htm");
            RegexAssert.Matches(WikiRegexes.ExternalLinksHTTPOnly, "https://www.foo.com/asdfasdf/asdf.htm", "https://www.foo.com/asdfasdf/asdf.htm");
            RegexAssert.NoMatch(WikiRegexes.ExternalLinksHTTPOnly, "ftp://www.foo.com/asdfasdf/asdf.htm");
            RegexAssert.NoMatch(WikiRegexes.ExternalLinksHTTPOnly, "mailto://www.foo.com/asdfasdf/asdf.htm");
            RegexAssert.NoMatch(WikiRegexes.ExternalLinksHTTPOnly, "irc://www.foo.com/asdfasdf/asdf.htm");
            RegexAssert.NoMatch(WikiRegexes.ExternalLinksHTTPOnly, "gopher://www.foo.com/asdfasdf/asdf.htm");
            RegexAssert.NoMatch(WikiRegexes.ExternalLinksHTTPOnly, "telnet://www.foo.com/asdfasdf/asdf.htm");
            RegexAssert.NoMatch(WikiRegexes.ExternalLinksHTTPOnly, "nntp://www.foo.com/asdfasdf/asdf.htm");
            RegexAssert.NoMatch(WikiRegexes.ExternalLinksHTTPOnly, "worldwind://www.foo.com/asdfasdf/asdf.htm");
            RegexAssert.NoMatch(WikiRegexes.ExternalLinksHTTPOnly, "news://www.foo.com/asdfasdf/asdf.htm");
            RegexAssert.NoMatch(WikiRegexes.ExternalLinksHTTPOnly, "svn://www.foo.com/asdfasdf/asdf.htm");

            RegexAssert.Matches(WikiRegexes.ExternalLinksHTTPOnly, "lol http://www.google.co.uk lol", "http://www.google.co.uk");

            RegexAssert.Matches(WikiRegexes.ExternalLinksHTTPOnly, "[http://www.google.co.uk]", "[http://www.google.co.uk]");
            RegexAssert.Matches(WikiRegexes.ExternalLinksHTTPOnly, "[http://www.google.co.uk google]", "[http://www.google.co.uk google]");

            RegexAssert.Matches(WikiRegexes.ExternalLinksHTTPOnly, "lol [http://www.google.co.uk] lol", "[http://www.google.co.uk]");
            RegexAssert.Matches(WikiRegexes.ExternalLinksHTTPOnly, "lol [http://www.google.co.uk google] lol", "[http://www.google.co.uk google]");

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_14#Regex_problem
            RegexAssert.Matches(WikiRegexes.ExternalLinksHTTPOnly, "http://www.google.co.uk google}}", "http://www.google.co.uk");
            RegexAssert.Matches(WikiRegexes.ExternalLinksHTTPOnly, "http://www.google.co.uk}}", "http://www.google.co.uk");

            RegexAssert.Matches(WikiRegexes.ExternalLinksHTTPOnly, @"date=April 2010|url=http://w/010111a.html}}", "http://w/010111a.html");
            RegexAssert.Matches(WikiRegexes.ExternalLinksHTTPOnly, @"date=April 2010|url=http://w/010111a.html|location=London}}", "http://w/010111a.html");

            // incorrect brackets
            RegexAssert.Matches(WikiRegexes.ExternalLinksHTTPOnly, "lol [http://www.google.co.uk lol", "http://www.google.co.uk");

            RegexAssert.NoMatch(WikiRegexes.ExternalLinksHTTPOnly, "Google");

            // protocol is group 1
            Assert.That(WikiRegexes.ExternalLinksHTTPOnly.Match(@"http://google.co.uk").Groups[1].Value, Is.EqualTo("http"));
            Assert.That(WikiRegexes.ExternalLinksHTTPOnly.Match(@"Http://google.co.uk").Groups[1].Value, Is.EqualTo("Http"));
            Assert.That(WikiRegexes.ExternalLinksHTTPOnly.Match(@"HTTP://google.co.uk").Groups[1].Value, Is.EqualTo("HTTP"));

            // not when in external link brackets
            Assert.That(WikiRegexes.ExternalLinksHTTPOnly.Match(@"[http://google.co.uk Google]").Groups[1].Value, Is.Empty);
        }

        [Test]
        public void PossibleInterwikis()
        {
            RegexAssert.Matches(WikiRegexes.PossibleInterwikis, "foo[[en:bar]]", "[[en:bar]]");
            RegexAssert.Matches(WikiRegexes.PossibleInterwikis, "[[en:bar]][[ru:", "[[en:bar]]");
            RegexAssert.Matches(WikiRegexes.PossibleInterwikis, "foo[[en:bar:quux]][[ru:boz test]]", "[[en:bar:quux]]", "[[ru:boz test]]");
            RegexAssert.Matches(WikiRegexes.PossibleInterwikis, "foo[[en::bar]]", "[[en::bar]]");

            RegexAssert.NoMatch(WikiRegexes.PossibleInterwikis, "[[:en:foo]]");
            RegexAssert.NoMatch(WikiRegexes.PossibleInterwikis, "[[:ru:foo]]");
            RegexAssert.NoMatch(WikiRegexes.PossibleInterwikis, "[[:foo]]");
            RegexAssert.NoMatch(WikiRegexes.PossibleInterwikis, "[[File:foo]]");

            Assert.That(WikiRegexes.PossibleInterwikis.Match("[[ en :bar]]").Groups[1].Value, Is.EqualTo("en"));
            Assert.That(WikiRegexes.PossibleInterwikis.Match("[[en: bar ]]").Groups[2].Value, Is.EqualTo("bar"));
            Assert.That(WikiRegexes.PossibleInterwikis.Match("[[en: bar ]] <!--comm-->").Groups[3].Value, Is.EqualTo(" <!--comm-->"));

            // length outside range
            RegexAssert.NoMatch(WikiRegexes.PossibleInterwikis, "[[e:foo]]");
            RegexAssert.NoMatch(WikiRegexes.PossibleInterwikis, @"[[Something:
[[other]]
]]");
            RegexAssert.NoMatch(WikiRegexes.PossibleInterwikis, "[[abcdefghijlkmnop:foo]]");

            RegexAssert.Matches(WikiRegexes.PossibleInterwikis, "foo[[en:bar]] <!--comm-->", "[[en:bar]] <!--comm-->");
        }

        [Test]
        public void UntemplatedQuotes()
        {
            // RegexAssert doesn't seem to work, so do tests this way

            // be careful about condensing any of these unit tests, as some of the different quote characters *look* the same, but in fact are different Unicode characters

            ClassicAssert.IsFalse(WikiRegexes.UntemplatedQuotes.Replace(@" ""very fast"" ", "1").Contains(@"""very fast"""));
            ClassicAssert.IsFalse(WikiRegexes.UntemplatedQuotes.Replace(@"""very fast"" ", "1").Contains(@"""very fast"""));
            ClassicAssert.IsFalse(WikiRegexes.UntemplatedQuotes.Replace(@" « very fast » ", "1").Contains(@"« very fast »"));
            ClassicAssert.IsFalse(WikiRegexes.UntemplatedQuotes.Replace(@" ‘very fast‘ ", "1").Contains(@"‘very fast‘"));
            ClassicAssert.IsFalse(WikiRegexes.UntemplatedQuotes.Replace(@" ’ very fast ’ ", "").Contains(@"’ very fast ’"));
            ClassicAssert.IsFalse(WikiRegexes.UntemplatedQuotes.Replace(@" “ very fast “ ", "").Contains(@"“ very fast “"));
            ClassicAssert.IsFalse(WikiRegexes.UntemplatedQuotes.Replace(@" ” very fast ” ", "").Contains(@"” very fast ”"));
            ClassicAssert.IsFalse(WikiRegexes.UntemplatedQuotes.Replace(@" ‛ very fast ‛ ", "").Contains(@"‛ very fast ‛"));
            ClassicAssert.IsFalse(WikiRegexes.UntemplatedQuotes.Replace(@" ‟ very fast‟ ", " ").Contains(@"‟ very fast‟"));
            ClassicAssert.IsFalse(WikiRegexes.UntemplatedQuotes.Replace(@" ‹ very fast › ", "").Contains(@"‹ very fast ›"));
            ClassicAssert.IsFalse(WikiRegexes.UntemplatedQuotes.Replace(@" “ very fast ” ", "").Contains(@"“ very fast ”"));
            ClassicAssert.IsFalse(WikiRegexes.UntemplatedQuotes.Replace(@" „ very fast „ ", "").Contains(@"„ very fast „"));
            ClassicAssert.IsFalse(WikiRegexes.UntemplatedQuotes.Replace(@" „ very fast „ ", "").Contains(@"„ very fast „"));
            ClassicAssert.IsFalse(WikiRegexes.UntemplatedQuotes.Replace(@" ‘ very fast ‘ ", "").Contains(@"‘ very fast ‘"));
            ClassicAssert.IsFalse(WikiRegexes.UntemplatedQuotes.Replace(@" ’ very fast ’ ", "").Contains(@"’ very fast ’"));
            ClassicAssert.IsFalse(WikiRegexes.UntemplatedQuotes.Replace(@" “ very fast ” ", "").Contains(@"“ very fast ”"));
            ClassicAssert.IsFalse(WikiRegexes.UntemplatedQuotes.Replace(@" ` very fast ` ", "").Contains(@"` very fast `"));
            ClassicAssert.IsFalse(WikiRegexes.UntemplatedQuotes.Replace(@" ’ very fast ‘ ", "").Contains(@"’ very fast ‘"));
            ClassicAssert.IsFalse(WikiRegexes.UntemplatedQuotes.Replace(@" „very
fast„ ", "").Contains(@" „very
fast„ "));

            ClassicAssert.IsTrue(WikiRegexes.UntemplatedQuotes.Replace(@" “ very fast ” but not pretty ", "").Contains("but not pretty"));
            ClassicAssert.IsTrue(WikiRegexes.UntemplatedQuotes.Replace(@" “ very fast ” but not pretty“ ", "").Contains("but not pretty“"));
            ClassicAssert.IsTrue(WikiRegexes.UntemplatedQuotes.Replace(@" “ very fast ” and “ very well ” ", "").Contains(" and "));
            ClassicAssert.IsTrue(WikiRegexes.UntemplatedQuotes.Replace(@"not pretty but “ very fast ” ", "").Contains("not pretty but "));

            // don't match single quotes, no quotes
            ClassicAssert.IsTrue(WikiRegexes.UntemplatedQuotes.Replace(@" very fast ", "").Contains(@"very fast"));
            ClassicAssert.IsTrue(WikiRegexes.UntemplatedQuotes.Replace(@" 'very fast' ", "").Contains(@"'very fast'"));
            ClassicAssert.IsTrue(WikiRegexes.UntemplatedQuotes.Replace(@" ''very fast'' ", "").Contains(@"''very fast''"));
            ClassicAssert.IsTrue(WikiRegexes.UntemplatedQuotes.Replace(@" ''very fast'' ", "").Contains(@"''very fast''"));
            ClassicAssert.IsTrue(WikiRegexes.UntemplatedQuotes.Replace(@" ,very fast, ", "").Contains(@",very fast,")); // commas

            // don't match apostrophes when used within words
            ClassicAssert.IsTrue(WikiRegexes.UntemplatedQuotes.Replace(@"Now it’s a shame as it’s a", "").Contains(@"Now it’s a shame as it’s a"));
        }

        [Test]
        public void CurlyDoubleQuotes()
        {
            ClassicAssert.IsTrue(WikiRegexes.CurlyDoubleQuotes.IsMatch(@" “ very fast ”"));
            ClassicAssert.IsTrue(WikiRegexes.CurlyDoubleQuotes.IsMatch(@"very fast „"));
        }

        [Test]
        public void RFromModification()
        {
            ClassicAssert.IsTrue(Tools.NestedTemplateRegex(WikiRegexes.RFromModificationList).IsMatch(@"{{R from modification}}"));
            ClassicAssert.IsTrue(Tools.NestedTemplateRegex(WikiRegexes.RFromModificationList).IsMatch(@"{{ r from modification}}"));
            ClassicAssert.IsTrue(Tools.NestedTemplateRegex(WikiRegexes.RFromModificationList).IsMatch(@"{{R mod }}"));
            ClassicAssert.IsTrue(Tools.NestedTemplateRegex(WikiRegexes.RFromModificationList).IsMatch(@"{{R from alternate punctuation}}"));
            ClassicAssert.IsTrue(Tools.NestedTemplateRegex(WikiRegexes.RFromModificationList).IsMatch(@"{{R from alternative punctuation}}"));
        }

        [Test]
        public void RFromTitleWithoutDiacritics()
        {
            ClassicAssert.IsTrue(Tools.NestedTemplateRegex(WikiRegexes.RFromTitleWithoutDiacriticsList).IsMatch(@"{{R to accents}}"));
            ClassicAssert.IsTrue(Tools.NestedTemplateRegex(WikiRegexes.RFromTitleWithoutDiacriticsList).IsMatch(@"{{Redirects from title without diacritics}}"));
            ClassicAssert.IsTrue(Tools.NestedTemplateRegex(WikiRegexes.RFromTitleWithoutDiacriticsList).IsMatch(@"{{RDiacr}}"));
            ClassicAssert.IsTrue(Tools.NestedTemplateRegex(WikiRegexes.RFromTitleWithoutDiacriticsList).IsMatch(@"{{r to unicode name}}"));
            ClassicAssert.IsTrue(Tools.NestedTemplateRegex(WikiRegexes.RFromTitleWithoutDiacriticsList).IsMatch(@"{{ R to unicode}}"));
            ClassicAssert.IsTrue(Tools.NestedTemplateRegex(WikiRegexes.RFromTitleWithoutDiacriticsList).IsMatch(@"{{R to unicode  }}"));
            ClassicAssert.IsTrue(Tools.NestedTemplateRegex(WikiRegexes.RFromTitleWithoutDiacriticsList).IsMatch(@"{{R to title with diacritics}}"));
            ClassicAssert.IsTrue(Tools.NestedTemplateRegex(WikiRegexes.RFromTitleWithoutDiacriticsList).IsMatch(@"{{R to diacritics}}"));
            ClassicAssert.IsTrue(Tools.NestedTemplateRegex(WikiRegexes.RFromTitleWithoutDiacriticsList).IsMatch(@"{{R to diacritic}}"));
            ClassicAssert.IsTrue(Tools.NestedTemplateRegex(WikiRegexes.RFromTitleWithoutDiacriticsList).IsMatch(@"{{R from name without diacritics}}"));
            ClassicAssert.IsTrue(Tools.NestedTemplateRegex(WikiRegexes.RFromTitleWithoutDiacriticsList).IsMatch(@"{{R from title without diacritics}}"));
            ClassicAssert.IsTrue(Tools.NestedTemplateRegex(WikiRegexes.RFromTitleWithoutDiacriticsList).IsMatch(@"{{R from original name without diacritics}}"));
            ClassicAssert.IsTrue(Tools.NestedTemplateRegex(WikiRegexes.RFromTitleWithoutDiacriticsList).IsMatch(@"{{R without diacritics}}"));
        }

        [Test]
        public void MoreNoFootnotesTests()
        {
            ClassicAssert.IsTrue(WikiRegexes.MoreNoFootnotes.IsMatch(@"{{morefootnotes}}"));
            ClassicAssert.IsTrue(WikiRegexes.MoreNoFootnotes.IsMatch(@"{{Morefootnotes}}"));
            ClassicAssert.IsTrue(WikiRegexes.MoreNoFootnotes.IsMatch(@"{{More footnotes}}"));
            ClassicAssert.IsTrue(WikiRegexes.MoreNoFootnotes.IsMatch(@"{{Nofootnotes}}"));
            ClassicAssert.IsTrue(WikiRegexes.MoreNoFootnotes.IsMatch(@"{{No footnotes}}"));
            ClassicAssert.IsTrue(WikiRegexes.MoreNoFootnotes.IsMatch(@"{{nofootnotes}}"));
            ClassicAssert.IsTrue(WikiRegexes.MoreNoFootnotes.IsMatch(@"{{nofootnotes|section}}"));

            ClassicAssert.IsFalse(WikiRegexes.MoreNoFootnotes.IsMatch(@"{{NOFOOTNOTES}}"));
        }

        [Test]
        public void ReferencesTemplateTests()
        {
            ClassicAssert.IsTrue(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello<ref>Fred</ref> {{reflist}}"));
            ClassicAssert.IsTrue(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello<ref>Fred</ref> {{reflist|2}}"));
            ClassicAssert.IsTrue(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello<ref>Fred</ref> {{Reflist}}"));
            ClassicAssert.IsTrue(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello<ref>Fred</ref> {{ref-list}}"));
            ClassicAssert.IsTrue(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello<ref>Fred</ref> {{Reflink}}"));
            ClassicAssert.IsTrue(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello<ref>Fred</ref> {{reflink}}"));
            ClassicAssert.IsTrue(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello<ref>Fred</ref> {{References}}"));
            ClassicAssert.IsTrue(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello<ref>Fred</ref> {{references}}"));
            ClassicAssert.IsTrue(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello<ref>Fred</ref> <references/>"));
            ClassicAssert.IsTrue(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello<ref>Fred</ref> <references />"));
            ClassicAssert.IsTrue(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello<ref>Fred</ref> <references responsive/>"));
            ClassicAssert.IsTrue(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello<ref>Fred</ref> <references responsive=""0""/>"));
            ClassicAssert.IsTrue(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello<ref>Fred</ref> <References />"));
            ClassicAssert.IsTrue(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello<ref>Fred</ref> {{Listaref|2}}"));
            ClassicAssert.IsTrue(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello<ref>Fred</ref> {{ listaref | 2}}"));
            ClassicAssert.IsTrue(WikiRegexes.ReferencesTemplate.IsMatch(@"<references>
<ref name =Fred>Fred</ref> </references>"));
            ClassicAssert.IsTrue(WikiRegexes.ReferencesTemplate.IsMatch(@"<  references >
<ref name =Fred>Fred</ref> </  references  >"));

            ClassicAssert.IsFalse(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello<ref>Fred</ref>"));
            ClassicAssert.IsFalse(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello<ref name=""F"">Fred</ref>"));
            ClassicAssert.IsFalse(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello world"));

            ClassicAssert.IsTrue(WikiRegexes.ReferencesTemplate.IsMatch(@"{{Reflist|refs=
<ref name=modern>{{cite news |first=William }}
        }}"));
        }

        [Test]
        public void CiteTemplate()
        {
            ClassicAssert.IsTrue(WikiRegexes.CiteTemplate.IsMatch(@"{{cite web|url=a|title=b}}"));
            ClassicAssert.IsTrue(WikiRegexes.CiteTemplate.IsMatch(@"{{cite web}}"));
            ClassicAssert.IsTrue(WikiRegexes.CiteTemplate.IsMatch(@"{{ cite web|url=a|title=b}}"));
            ClassicAssert.IsTrue(WikiRegexes.CiteTemplate.IsMatch(@"{{Cite web|url=a|title=b and {{foo}} there}}"));
            ClassicAssert.IsTrue(WikiRegexes.CiteTemplate.IsMatch(@"{{cite news|url=a|title=b}}"));
            ClassicAssert.IsTrue(WikiRegexes.CiteTemplate.IsMatch(@"{{cite book|url=a|title=b}}"));
            ClassicAssert.IsTrue(WikiRegexes.CiteTemplate.IsMatch(@"{{cite conference|url=a|title=b}}"));
            ClassicAssert.IsTrue(WikiRegexes.CiteTemplate.IsMatch(@"{{cite manual|url=a|title=b}}"));
            ClassicAssert.IsTrue(WikiRegexes.CiteTemplate.IsMatch(@"{{cite paper|url=a|title=b}}"));
            ClassicAssert.IsTrue(WikiRegexes.CiteTemplate.IsMatch(@"{{cite press release|url=a|title=b}}"));
            ClassicAssert.IsTrue(WikiRegexes.CiteTemplate.IsMatch(@"{{cite encyclopedia|url=a|title=b}}"));
            ClassicAssert.IsTrue(WikiRegexes.CiteTemplate.IsMatch(@"{{cite AV media|url=a|title=b}}"));
            ClassicAssert.IsTrue(WikiRegexes.CiteTemplate.IsMatch(@"{{vcite2 journal}}"));
            ClassicAssert.IsTrue(WikiRegexes.CiteTemplate.IsMatch(@"{{cite magazine|url=a|title=b}}"));
            ClassicAssert.IsTrue(WikiRegexes.CiteTemplate.IsMatch(@"{{cite report|url=a|title=b}}"));

            // name derivation
            Assert.That(WikiRegexes.CiteTemplate.Match(@"{{cite web|url=a|title=b}}").Groups[2].Value, Is.EqualTo("cite web"));
            Assert.That(WikiRegexes.CiteTemplate.Match(@"{{ cite web |url=a|title=b}}").Groups[2].Value, Is.EqualTo("cite web"));
            Assert.That(WikiRegexes.CiteTemplate.Match(@"{{Cite web
|url=a|title=b}}").Groups[2].Value, Is.EqualTo("Cite web"));
            Assert.That(WikiRegexes.CiteTemplate.Match(@"{{cite press release|url=a|title=b}}").Groups[2].Value, Is.EqualTo("cite press release"));
        }

        [Test]
        public void RefAfterReflist()
        {
            ClassicAssert.IsTrue(WikiRegexes.RefAfterReflist.IsMatch(@"blah <ref>a</ref> ==references== {{reflist}} <ref>b</ref>"));
            ClassicAssert.IsTrue(WikiRegexes.RefAfterReflist.IsMatch(@"blah <ref>a</ref> ==references== <references/> <ref>b</ref>"));
            ClassicAssert.IsTrue(WikiRegexes.RefAfterReflist.IsMatch(@"blah <ref>a</ref> ==references== {{Reflist}} <ref>b</ref>"));
            ClassicAssert.IsTrue(WikiRegexes.RefAfterReflist.IsMatch(@"blah <ref>a</ref> ==references== {{reflink}} <ref>b</ref>"));
            ClassicAssert.IsTrue(WikiRegexes.RefAfterReflist.IsMatch(@"blah <ref>a</ref> ==references== {{ref-list}} <ref>b</ref>"));
            ClassicAssert.IsTrue(WikiRegexes.RefAfterReflist.IsMatch(@"blah <ref>a</ref>
==references== {{reflist}} <ref>b</ref>"));
            ClassicAssert.IsTrue(WikiRegexes.RefAfterReflist.IsMatch(@"blah <ref>a</ref> ==references== {{reflist}} blah
<ref>b</ref>"));
            ClassicAssert.IsTrue(WikiRegexes.RefAfterReflist.IsMatch(@"blah <ref>a</ref> ==references== {{reflist}} <ref name=""b"">b</ref>"));

            // this is correct syntax
            ClassicAssert.IsFalse(WikiRegexes.RefAfterReflist.IsMatch(@"blah <ref>a</ref> ==references== {{reflist}}"));

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

            ClassicAssert.IsFalse(WikiRegexes.RefAfterReflist.IsMatch(bug1));
        }

        [Test]
        public void IbidOpCitation()
        {
            ClassicAssert.IsTrue(WikiRegexes.IbidLocCitation.IsMatch(@"ibid"));
            ClassicAssert.IsTrue(WikiRegexes.IbidLocCitation.IsMatch(@"Ibid"));
            ClassicAssert.IsTrue(WikiRegexes.IbidLocCitation.IsMatch(@"IBID"));
            ClassicAssert.IsTrue(WikiRegexes.IbidLocCitation.IsMatch(@"loc cit"));
            ClassicAssert.IsTrue(WikiRegexes.IbidLocCitation.IsMatch(@"loc.cit"));
            ClassicAssert.IsTrue(WikiRegexes.IbidLocCitation.IsMatch(@"Loc. cit"));
            ClassicAssert.IsTrue(WikiRegexes.IbidLocCitation.IsMatch(@"Loc
cit"));

            ClassicAssert.IsFalse(WikiRegexes.IbidLocCitation.IsMatch(@"Libid was"));
            ClassicAssert.IsFalse(WikiRegexes.IbidLocCitation.IsMatch(@"The loc was later cit"));
        }

        [Test]
        public void Ibid()
        {
            ClassicAssert.IsTrue(WikiRegexes.Ibid.IsMatch(@"{{ibid}}"));
            ClassicAssert.IsTrue(WikiRegexes.Ibid.IsMatch(@"{{ Ibid }}"));
            ClassicAssert.IsTrue(WikiRegexes.Ibid.IsMatch(@"{{ibid|date=May 2009}}"));
            ClassicAssert.IsTrue(WikiRegexes.Ibid.IsMatch(@"{{ibid | date=May 2009}}"));
            ClassicAssert.IsTrue(WikiRegexes.Ibid.IsMatch(@"{{Ibid|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"));
            ClassicAssert.IsTrue(WikiRegexes.Ibid.IsMatch(@"{{ibid|date=May 2009|foo=bar}}"));
            ClassicAssert.IsTrue(WikiRegexes.Ibid.IsMatch(@"{{ibid|}}"));

            ClassicAssert.IsFalse(WikiRegexes.Ibid.IsMatch(@"Libid was"));
            ClassicAssert.IsFalse(WikiRegexes.Ibid.IsMatch(@"{{IBID}}"));
            ClassicAssert.IsFalse(WikiRegexes.Ibid.IsMatch(@"{{Ibidate}}"));
        }

        [Test]
        public void WikipediaBooks()
        {
            ClassicAssert.IsTrue(WikiRegexes.WikipediaBooks.IsMatch(@"{{Wikipedia-Books}}"));
            ClassicAssert.IsTrue(WikiRegexes.WikipediaBooks.IsMatch(@"{{ Wikipedia books }}"));
            ClassicAssert.IsTrue(WikiRegexes.WikipediaBooks.IsMatch(@"{{Wikipedia books|1=Academy Awards|3=Academy Awards for Best Picture}}"));
        }

        [Test]
        public void DablinksTests()
        {
            ClassicAssert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{For|Fred the dancer|Fred (dancer)}}"));
            ClassicAssert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{for|Fred the dancer|Fred (dancer)}}"));
            ClassicAssert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{otherpeople1|Fred the dancer|Fred Smith (dancer)}}"));
            ClassicAssert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{For|Fred the dancer|Fred Smith (dancer)}}"));
            ClassicAssert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{redirect2|Fred the dancer|Fred Smith (dancer)}}"));
            ClassicAssert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{For
    |Fred the dancer
    |Fred (dancer)}}"));

            ClassicAssert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{For|
    Fred the dancer|
    Fred (dancer)}}"));

            ClassicAssert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{Otheruse|something}}"));
            ClassicAssert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{Otheruses|something}}"));
            ClassicAssert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{Otheruses2|something}}"));
            ClassicAssert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{otheruse|something}}"));
            ClassicAssert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{otheruses}}"));
            ClassicAssert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{other persons}}"));
            ClassicAssert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{otheruse
|something}}"));
            ClassicAssert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{selfref|something}}"));
            ClassicAssert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{Redirect-distinguish|something}}"));

            ClassicAssert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{About-distinguish2}}"), @"{{About-distinguish2}}");
            ClassicAssert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{About-distinguish}}"), @"{{About-distinguish}}");
            ClassicAssert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{about2}}"), @"{{about2}}");
            ClassicAssert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{about}}"), @"{{about}}");
            ClassicAssert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{distinguish-otheruses2}}"), @"{{distinguish-otheruses2}}");
            ClassicAssert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{distinguish-otheruses}}"), @"{{distinguish-otheruses}}");
            ClassicAssert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{distinguish2}}"), @"{{distinguish2}}");
            ClassicAssert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{distinguish}}"), @"{{distinguish}}");
            ClassicAssert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{for2}}"), @"{{for2}}");
            ClassicAssert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{for}}"), @"{{for}}");
            ClassicAssert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{further2}}"), @"{{further2}}");
            ClassicAssert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{further}}"), @"{{further}}");
            ClassicAssert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{hatnote}}"), @"{{hatnote}}");
            ClassicAssert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{other hurricanes}}"), @"{{other hurricanes}}");
            ClassicAssert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{other people2}}"), @"{{other people2}}");
            ClassicAssert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{other people3}}"), @"{{other people3}}");
            ClassicAssert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{other people}}"), @"{{other people}}");
            ClassicAssert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{other places3}}"), @"{{other places3}}");
            ClassicAssert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{other places}}"), @"{{other places}}");
            ClassicAssert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{other ships}}"), @"{{other ships}}");
            ClassicAssert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{other uses of}}"), @"{{other uses of}}");
            ClassicAssert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{other uses2}}"), @"{{other uses2}}");
            ClassicAssert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{other uses}}"), @"{{other uses}}");
            ClassicAssert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{outline}}"), @"{{outline}}");
            ClassicAssert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{redirect-distinguish2}}"), @"{{redirect-distinguish2}}");
            ClassicAssert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{redirect-distinguish}}"), @"{{redirect-distinguish}}");
            ClassicAssert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{redirect-several}}"), @"{{redirect-several}}");
            ClassicAssert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{redirect-multi}}"), @"{{redirect-multi}}");
            ClassicAssert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{redirect2}}"), @"{{redirect2}}");
            ClassicAssert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{redirect3}}"), @"{{redirect3}}");
            ClassicAssert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{redirect}}"), @"{{redirect}}");
            ClassicAssert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{see also}}"), @"{{see also}}");
            ClassicAssert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{selfref}}"), @"{{selfref}}");

            ClassicAssert.IsFalse(WikiRegexes.Dablinks.IsMatch(@"{{For fake template|Fred the dancer|Fred(dancer)}}"));
            ClassicAssert.IsFalse(WikiRegexes.Dablinks.IsMatch(@"{{REDIRECT2|Fred the dancer|Fred Smith (dancer)}}"));
            ClassicAssert.IsFalse(WikiRegexes.Dablinks.IsMatch(@"{{Otheruse2|something}}")); // non-existent
        }

        [Test]
        public void SisterLinksTests()
        {
            ClassicAssert.IsTrue(WikiRegexes.SisterLinks.IsMatch(@"{{wiktionary}}"));
            ClassicAssert.IsTrue(WikiRegexes.SisterLinks.IsMatch(@"{{ wiktionary }}"));
            ClassicAssert.IsTrue(WikiRegexes.SisterLinks.IsMatch(@"{{sisterlinks}}"));
            ClassicAssert.IsTrue(WikiRegexes.SisterLinks.IsMatch(@"{{sister links}}"));
            ClassicAssert.IsTrue(WikiRegexes.SisterLinks.IsMatch(@"{{sister project links}}"));
            ClassicAssert.IsTrue(WikiRegexes.SisterLinks.IsMatch(@"{{wikibooks}}"));
            ClassicAssert.IsTrue(WikiRegexes.SisterLinks.IsMatch(@"{{wikimedia}}"));
            ClassicAssert.IsTrue(WikiRegexes.SisterLinks.IsMatch(@"{{wikiversity}}"));
        }

        [Test]
        public void Unreferenced()
        {
            ClassicAssert.IsTrue(WikiRegexes.Unreferenced.IsMatch(@"{{unreferenced}}"));
            ClassicAssert.IsTrue(WikiRegexes.Unreferenced.IsMatch(@"{{  Unreferenced}}"));
            ClassicAssert.IsTrue(WikiRegexes.Unreferenced.IsMatch(@"{{unreferenced  }}"));
            ClassicAssert.IsTrue(WikiRegexes.Unreferenced.IsMatch(@"{{unreferenced|date=May 2009}}"));
            ClassicAssert.IsTrue(WikiRegexes.Unreferenced.IsMatch(@"{{unreferenced stub|date=May 2009}}"));

            ClassicAssert.IsFalse(WikiRegexes.Unreferenced.IsMatch(@"{{unreferenced-stub}}"));
        }

        [Test]
        public void PortalTemplateTests()
        {
            ClassicAssert.IsTrue(WikiRegexes.PortalTemplate.IsMatch(@"{{portal}}"));
            ClassicAssert.IsTrue(WikiRegexes.PortalTemplate.IsMatch(@"{{portal|Foo}}"));
            ClassicAssert.IsTrue(WikiRegexes.PortalTemplate.IsMatch(@"{{ portal}}"));
            ClassicAssert.IsTrue(WikiRegexes.PortalTemplate.IsMatch(@"{{Portal}}"));
            ClassicAssert.IsTrue(WikiRegexes.PortalTemplate.IsMatch(@"{{portal|Science}}"));
            ClassicAssert.IsTrue(WikiRegexes.PortalTemplate.IsMatch(@"{{portal|Spaceflight|RocketSunIcon.svg|break=yes}}"));

            ClassicAssert.IsFalse(WikiRegexes.PortalTemplate.IsMatch(@"{{PORTAL}}"));
            ClassicAssert.IsFalse(WikiRegexes.PortalTemplate.IsMatch(@"{{portalos}}"));
            ClassicAssert.IsFalse(WikiRegexes.PortalTemplate.IsMatch(@"{{portalparity}}"));
            ClassicAssert.IsFalse(WikiRegexes.PortalTemplate.IsMatch(@"{{Spanish portal|game}}"));
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
            ClassicAssert.IsTrue(WikiRegexes.InfoBox.IsMatch(@" {{infobox hello| bye}} "));
            ClassicAssert.IsTrue(WikiRegexes.InfoBox.IsMatch(@" {{Template:infobox hello| bye}} "));
            ClassicAssert.IsTrue(WikiRegexes.InfoBox.IsMatch(@" {{infobox hello
| bye}} "));
            ClassicAssert.IsTrue(WikiRegexes.InfoBox.IsMatch(@" {{Infobox_play| bye}} "));
            ClassicAssert.IsTrue(WikiRegexes.InfoBox.IsMatch(@" {{some infobox| hello| bye}} "));
            ClassicAssert.IsTrue(WikiRegexes.InfoBox.IsMatch(@" {{Some Infobox| hello| bye}} "));

            Assert.That(WikiRegexes.InfoBox.Match(@" {{Infobox
| hello| bye}} ").Groups[1].Value, Is.EqualTo("Infobox"));
        }

        [Test]
        public void DisplayLowerCaseItalicTitleTests()
        {
            ClassicAssert.IsTrue(WikiRegexes.DisplayLowerCaseItalicTitle.IsMatch(@"{{DISPLAYTITLE:foo}}"), "DISPLAYTITLE");
            ClassicAssert.IsTrue(WikiRegexes.DisplayLowerCaseItalicTitle.IsMatch(@"{{Display title:foo}}"), "Display title");
            ClassicAssert.IsTrue(WikiRegexes.DisplayLowerCaseItalicTitle.IsMatch(@"{{Displaytitle:foo}}"), "Displaytitle");
            ClassicAssert.IsTrue(WikiRegexes.DisplayLowerCaseItalicTitle.IsMatch(@"{{italic title}}"), "italic title");
            ClassicAssert.IsTrue(WikiRegexes.DisplayLowerCaseItalicTitle.IsMatch(@"{{lowercase title}}"), "lowercase title");
        }

        [Test]
        public void TemplateEndTests()
        {
            Assert.That(WikiRegexes.TemplateEnd.Match(@"{{foo}}").Value, Is.EqualTo(@"}}"));
            Assert.That(WikiRegexes.TemplateEnd.Match(@"{{foo }}").Value, Is.EqualTo(@" }}"));
            Assert.That(WikiRegexes.TemplateEnd.Match(@"{{foo
}}").Value, Is.EqualTo(@"
}}"));
            Assert.That(WikiRegexes.TemplateEnd.Match(@"{{foo
}}").Groups[1].Value, Is.EqualTo("\r\n"));
            Assert.That(WikiRegexes.TemplateEnd.Match(@"{{foo
 }}").Value, Is.EqualTo("\r\n }}"));
        }

        [Test]
        public void PstylesTests()
        {
            ClassicAssert.IsTrue(WikiRegexes.Pstyles.IsMatch(@"<p style=""margin:0px;font-size:100%""><span style=""color:#00ff00"">▪</span> <small>Francophone minorities</small></p>"));
            ClassicAssert.IsTrue(WikiRegexes.Pstyles.IsMatch(@"<p style=""font-family:monospace; line-height:130%"">hello</p>"));
            ClassicAssert.IsTrue(WikiRegexes.Pstyles.IsMatch(@"<p style=""font-family:monospace; line-height:130%"">hello</P>"));
            ClassicAssert.IsTrue(WikiRegexes.Pstyles.IsMatch(@"<p style=""font-family:monospace; line-height:130%"">hello</P>"));
            ClassicAssert.IsTrue(WikiRegexes.Pstyles.IsMatch(@"<p style = ""font-family:monospace; line-height:130%"">
hello</P>"));

        }

        [Test]
        public void FirstSection()
        {
            ClassicAssert.IsTrue(WikiRegexes.ZerothSection.IsMatch(@"article"));
            ClassicAssert.IsTrue(WikiRegexes.ZerothSection.IsMatch(@"article ==heading=="));
            ClassicAssert.IsTrue(WikiRegexes.ZerothSection.IsMatch(@"article ===heading==="));
            Assert.That(WikiRegexes.ZerothSection.Replace(@"article ==heading==", ""), Is.EqualTo("==heading=="));
            Assert.That(WikiRegexes.ZerothSection.Replace(@"{{wikify}}
{{Infobox hello | bye=yes}}
article words, '''bold''' blah.
==heading== words ==another heading==", ""), Is.EqualTo(@"==heading== words ==another heading=="));

            ClassicAssert.IsFalse(WikiRegexes.ZerothSection.IsMatch(@""));
        }

        [Test]
        public void HeadingLevelTwoTests()
        {
            ClassicAssert.IsTrue(WikiRegexes.HeadingLevelTwo.IsMatch(@"article
==heading==
a"));
            Assert.That(WikiRegexes.HeadingLevelTwo.Match(@"article
==heading==
a").Groups[1].Value, Is.EqualTo("heading"));
            ClassicAssert.IsTrue(WikiRegexes.HeadingLevelTwo.IsMatch(@"article
== heading ==
a"));
            Assert.That(WikiRegexes.HeadingLevelTwo.Match(@"article
== heading ==
a").Groups[1].Value, Is.EqualTo(" heading "));
            ClassicAssert.IsTrue(WikiRegexes.HeadingLevelTwo.IsMatch(@"article
== heading ==
words"));
            ClassicAssert.IsTrue(WikiRegexes.HeadingLevelTwo.IsMatch(@"article
==H==
a"));
            ClassicAssert.IsTrue(WikiRegexes.HeadingLevelTwo.IsMatch(@"article
==Hi==
a"));
            ClassicAssert.IsTrue(WikiRegexes.HeadingLevelTwo.IsMatch(@"article
==Here and=there==
a"));

            // no matches
            ClassicAssert.IsFalse(WikiRegexes.HeadingLevelTwo.IsMatch(@"article ==
heading=="));
            ClassicAssert.IsFalse(WikiRegexes.HeadingLevelTwo.IsMatch(@"article
==heading== words"));
            ClassicAssert.IsFalse(WikiRegexes.HeadingLevelTwo.IsMatch(@"article ===heading=="));
            ClassicAssert.IsFalse(WikiRegexes.HeadingLevelTwo.IsMatch(@"article
====heading===
words"));
        }

        [Test]
        public void HeadingLevelThreeTests()
        {
            ClassicAssert.IsTrue(WikiRegexes.HeadingLevelThree.IsMatch(@"article
===heading===
a"));
            Assert.That(WikiRegexes.HeadingLevelThree.Match(@"article
===heading===
a").Groups[1].Value, Is.EqualTo("heading"));
            ClassicAssert.IsTrue(WikiRegexes.HeadingLevelThree.IsMatch(@"article
=== heading ===
a"));
            Assert.That(WikiRegexes.HeadingLevelThree.Match(@"article
=== heading ===
a").Groups[1].Value, Is.EqualTo(" heading "));
            ClassicAssert.IsTrue(WikiRegexes.HeadingLevelThree.IsMatch(@"article
=== heading ===
words"));
            ClassicAssert.IsTrue(WikiRegexes.HeadingLevelThree.IsMatch(@"article
===H===
a"));
            ClassicAssert.IsTrue(WikiRegexes.HeadingLevelThree.IsMatch(@"article
===Hi===
a"));
            ClassicAssert.IsTrue(WikiRegexes.HeadingLevelThree.IsMatch(@"article
===Here and=there===
a"));

            // no matches
            ClassicAssert.IsFalse(WikiRegexes.HeadingLevelThree.IsMatch(@"article ===
heading==="));
            ClassicAssert.IsFalse(WikiRegexes.HeadingLevelThree.IsMatch(@"article
===heading=== words"));
            ClassicAssert.IsFalse(WikiRegexes.HeadingLevelThree.IsMatch(@"article ====heading==="));
            ClassicAssert.IsFalse(WikiRegexes.HeadingLevelThree.IsMatch(@"article
=====heading====
words"));
        }

        [Test]
        public void SectionLevelTwoTests()
        {
            ClassicAssert.IsTrue(WikiRegexes.SectionLevelTwo.IsMatch(@"== heading a ==
words
=== subsection ===
words
words2
== heading b ==
"));

            ClassicAssert.IsFalse(WikiRegexes.SectionLevelTwo.IsMatch(@"=== subsection ===
words
words2"));
            ClassicAssert.IsFalse(WikiRegexes.SectionLevelTwo.IsMatch(@"=== heading a ==
words
=== subsection ===
words
words2"));
            ClassicAssert.IsFalse(WikiRegexes.SectionLevelTwo.IsMatch(@"= heading a =
words
=== subsection ===
words
words2"));
        }

        [Test]
        public void ArticleToFirstLevelTwoHeadingTests()
        {
            ClassicAssert.IsTrue(WikiRegexes.ArticleToFirstLevelTwoHeading.IsMatch(@"words
== heading a ==
"));

            ClassicAssert.IsFalse(WikiRegexes.ArticleToFirstLevelTwoHeading.IsMatch(@"words
=== heading a ===
"));
        }

        [Test]
        public void MultipleIssuesTests()
        {
            ClassicAssert.IsTrue(WikiRegexes.MultipleIssues.IsMatch(@"{{Multiple issues|wikify=May 2008|a=b|c=d}}"), "with space");
            ClassicAssert.IsTrue(WikiRegexes.MultipleIssues.IsMatch(@"{{Multipleissues|wikify=May 2008|a=b|c=d}}"), "unspaced");
            ClassicAssert.IsTrue(WikiRegexes.MultipleIssues.IsMatch(@"{{multipleissues|wikify=May 2008|a=b|c=d}}"), "unspaced no capitals");
            ClassicAssert.IsTrue(WikiRegexes.MultipleIssues.IsMatch(@"{{Multiple issues | wikify=May 2008|a=b|c=d}}"), "with spaces in parameters");
            ClassicAssert.IsTrue(WikiRegexes.MultipleIssues.IsMatch(@"{{Multiple issues
           | wikify=May 2008|a=b|c=d}}"), "with break lines");
            ClassicAssert.IsTrue(WikiRegexes.MultipleIssues.IsMatch(@"{{Multiple issues|}}"), "empty");
            ClassicAssert.IsTrue(WikiRegexes.MultipleIssues.IsMatch(@"{{Multipleissues}}"), "empty too");

            ClassicAssert.IsTrue(WikiRegexes.MultipleIssues.IsMatch(@"{{ multiple issues|wikify=May 2008|a=b|c=d}}"));

            ClassicAssert.IsTrue(WikiRegexes.MultipleIssues.IsMatch(@"{{ multiple issues|wikify={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|orphan={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|c=d}}"));

            // no matches
            ClassicAssert.IsFalse(WikiRegexes.MultipleIssues.IsMatch(@"{{ARTICLEISSUES }}"));
            ClassicAssert.IsFalse(WikiRegexes.MultipleIssues.IsMatch(@"{{Bert|Multipleissues }}"));
        }

        [Test]
        public void NonDeadEndPageTemplatesTests()
        {
            ClassicAssert.IsTrue(WikiRegexes.NonDeadEndPageTemplates.IsMatch(@"{{Events by year for decade|31}}"));
            ClassicAssert.IsTrue(WikiRegexes.NonDeadEndPageTemplates.IsMatch(@"{{Events by year for decade BC|31}}"));
            ClassicAssert.IsTrue(WikiRegexes.NonDeadEndPageTemplates.IsMatch(@"{{SCOTUSRow | case name = Arizona v. Inter Tribal Council of Ariz., Inc. | docket = 12-71 | decision date = June 17 | decision year = 2013}}"));
            ClassicAssert.IsTrue(WikiRegexes.NonDeadEndPageTemplates.IsMatch(@"{{Portal:Current events/Month Inclusion|2009 February}}"));
        }

        [Test]
        public void WordApostropheTests()
        {
            ClassicAssert.IsTrue(WikiRegexes.RegexWordApostrophes.IsMatch(@"Rachel"));
            ClassicAssert.IsTrue(WikiRegexes.RegexWordApostrophes.IsMatch(@"Rachel's"));
            ClassicAssert.IsTrue(WikiRegexes.RegexWordApostrophes.IsMatch(@"Rachel’s"), "curly apostrophe");
            ClassicAssert.IsTrue(WikiRegexes.RegexWordApostrophes.IsMatch(@"Kwakwaka'wakw"));

            Assert.That(WikiRegexes.RegexWordApostrophes.Replace(@"Kwakwaka'wakw", ""), Is.Empty);
        }

        [Test]
        public void DeathsOrLivingCategoryTests()
        {
            ClassicAssert.IsTrue(WikiRegexes.DeathsOrLivingCategory.IsMatch(@"[[Category:653 deaths|Honorius]]"), "xxx deaths with sortkey");
            ClassicAssert.IsTrue(WikiRegexes.DeathsOrLivingCategory.IsMatch(@"[[Category:839 deaths]]"), "xxx deaths");
            ClassicAssert.IsTrue(WikiRegexes.DeathsOrLivingCategory.IsMatch(@"[[Category:5th-century BC deaths]]"), "centure BC deaths");
            ClassicAssert.IsTrue(WikiRegexes.DeathsOrLivingCategory.IsMatch(@"[[Category:1209 deaths]]"), "xxxx deaths");
            ClassicAssert.IsTrue(WikiRegexes.DeathsOrLivingCategory.IsMatch(@"[[Category:2018 suicides]]"), "xxxx suicides");
            ClassicAssert.IsTrue(WikiRegexes.DeathsOrLivingCategory.IsMatch(@"[[Category:Living people]]"), "living people");
            ClassicAssert.IsTrue(WikiRegexes.DeathsOrLivingCategory.IsMatch(@"[[Category:Possibly living people]]"), "possibly living people");
            ClassicAssert.IsTrue(WikiRegexes.DeathsOrLivingCategory.IsMatch(@"[[Category:221 BC deaths]]"), "xxx BC deaths");
            ClassicAssert.IsTrue(WikiRegexes.DeathsOrLivingCategory.IsMatch(@"[[Category:Year of death missing]]"), "YOD missing");
            ClassicAssert.IsTrue(WikiRegexes.DeathsOrLivingCategory.IsMatch(@"[[Category:Year of death unknown]]"), "YOD unknown");
            ClassicAssert.IsTrue(WikiRegexes.DeathsOrLivingCategory.IsMatch(@"[[Category:Date of death unknown]]"), "DOD unknown");
            ClassicAssert.IsTrue(WikiRegexes.DeathsOrLivingCategory.IsMatch(@"[[Category:Year of death uncertain]]"), "YOD uncertain");
            ClassicAssert.IsTrue(WikiRegexes.DeathsOrLivingCategory.IsMatch(@"[[Category:Missing people]]"), "Mising people");
            ClassicAssert.IsTrue(WikiRegexes.DeathsOrLivingCategory.IsMatch(@"[[Category:People declared dead in absentia]]"), "People declared dead in absentia");
            ClassicAssert.IsTrue(WikiRegexes.DeathsOrLivingCategory.IsMatch(@"[[Category:839 deaths ]]"), "xxx deaths with space");

            // no matches
            ClassicAssert.IsFalse(WikiRegexes.DeathsOrLivingCategory.IsMatch(@""));
            ClassicAssert.IsFalse(WikiRegexes.DeathsOrLivingCategory.IsMatch(@"[[Category:strange deaths]]"));
            ClassicAssert.IsFalse(WikiRegexes.DeathsOrLivingCategory.IsMatch(@"[[Category:Missing people organizations]]"));
            ClassicAssert.IsFalse(WikiRegexes.DeathsOrLivingCategory.IsMatch(@"1990 deaths"));
        }

        [Test]
        public void BirthsCategoryTests()
        {
            ClassicAssert.IsTrue(WikiRegexes.BirthsCategory.IsMatch(@"[[Category:12th-century births]]"));
            ClassicAssert.IsTrue(WikiRegexes.BirthsCategory.IsMatch(@"[[Category:1299 births]]"));
            ClassicAssert.IsTrue(WikiRegexes.BirthsCategory.IsMatch(@"[[Category:110 BC births]]"));
            ClassicAssert.IsTrue(WikiRegexes.BirthsCategory.IsMatch(@"[[Category:1st-century births]]"));

            // no matches
            ClassicAssert.IsFalse(WikiRegexes.BirthsCategory.IsMatch(@"[[Category:strange births]]"));
            ClassicAssert.IsFalse(WikiRegexes.BirthsCategory.IsMatch(@"1960 births"));
            ClassicAssert.IsFalse(WikiRegexes.BirthsCategory.IsMatch(@""));
        }

        [Test]
        public void PeopleFromCategoryTests()
        {
            ClassicAssert.IsTrue(WikiRegexes.PeopleFromCategory.IsMatch(@"[[Category:People from x]]"));
            ClassicAssert.IsTrue(WikiRegexes.PeopleFromCategory.IsMatch(@"[[Category:People from x|y]]"));
            ClassicAssert.IsFalse(WikiRegexes.PeopleFromCategory.IsMatch(@"[[Category:People who x]]"));
        }

        [Test]
        public void DateBirthAndAgeTests()
        {
            ClassicAssert.IsTrue(WikiRegexes.DateBirthAndAge.IsMatch(@"{{Birth date|1972|02|18}}"));
            ClassicAssert.IsTrue(WikiRegexes.DateBirthAndAge.IsMatch(@"{{Birth date and age|1972|02|18}}"));
            ClassicAssert.IsTrue(WikiRegexes.DateBirthAndAge.IsMatch(@"{{Birth date| 1972 |02|18}}"));

            ClassicAssert.IsTrue(WikiRegexes.DateBirthAndAge.IsMatch(@"{{birth date and age|mf=yes|1980|3|9}}"));
            ClassicAssert.IsTrue(WikiRegexes.DateBirthAndAge.IsMatch(@"{{bda|mf=yes|1980|3|9}}"));

            Assert.That(WikiRegexes.DateBirthAndAge.Match(@"{{birth-date|1975}}").Groups[1].Value, Is.EqualTo("1975"), "extract year from birth-date");
            Assert.That(WikiRegexes.DateBirthAndAge.Match(@"{{birth-date|   1975}}").Groups[1].Value, Is.EqualTo("1975"), "spacing");
            Assert.That(WikiRegexes.DateBirthAndAge.Match(@"{{birth date and age|year=1984|month=2|day=6}}").Groups[1].Value, Is.EqualTo("1984"));

            Assert.That(WikiRegexes.DateBirthAndAge.Match(@"{{Birth date|1972|02|18}}").Value, Is.EqualTo("{{Birth date|1972|02|18}}"));
        }

        [Test]
        public void DateDeathAndAgeTests()
        {
            ClassicAssert.IsTrue(WikiRegexes.DeathDate.IsMatch(@"{{Death date|1972|02|18}}"));
            ClassicAssert.IsTrue(WikiRegexes.DeathDate.IsMatch(@"{{Death date and age|1972|02|18}}"));
            ClassicAssert.IsTrue(WikiRegexes.DeathDate.IsMatch(@"{{Death date| 1972 |02|18}}"));

            ClassicAssert.IsTrue(WikiRegexes.DeathDate.IsMatch(@"{{death date and age|mf=yes|1980|3|9}}"));

            Assert.That(WikiRegexes.DeathDate.Match(@"{{death-date|1975}}").Groups[1].Value, Is.EqualTo("1975"));
            Assert.That(WikiRegexes.DeathDate.Match(@"{{death-date|   1975}}").Groups[1].Value, Is.EqualTo("1975"));
            Assert.That(WikiRegexes.DeathDate.Match(@"{{death date and age|year=1984|month=2|day=6}}").Groups[1].Value, Is.EqualTo("1984"));
            Assert.That(WikiRegexes.DeathDate.Match(@"{{death date and age|1911|12|12|1821|10|02}}").Groups[1].Value, Is.EqualTo("1911"));
        }

        [Test]
        public void DeathDateAndAge()
        {
            Assert.That(WikiRegexes.DeathDateAndAge.Match(@"{{death date and age|1911|12|12|1821|10|02}}").Groups[1].Value, Is.EqualTo("1911"));
            Assert.That(WikiRegexes.DeathDateAndAge.Match(@"{{death-date and age|1911|12|12|1821|10|02}}").Groups[1].Value, Is.EqualTo("1911"));
            Assert.That(WikiRegexes.DeathDateAndAge.Match(@"{{dda|1911|12|12|1821|10|02}}").Groups[1].Value, Is.EqualTo("1911"));
            Assert.That(WikiRegexes.DeathDateAndAge.Match(@"{{death date and age|1911|12|12|1821|10|02}}").Groups[2].Value, Is.EqualTo("1821"));
        }

        [Test]
        public void BareExternalLinkTests()
        {
            ClassicAssert.IsTrue(WikiRegexes.BareExternalLink.IsMatch(@"* http://www.site.com
"));
            ClassicAssert.IsTrue(WikiRegexes.BareExternalLink.IsMatch(@"* http://www.site.com
"));
            ClassicAssert.IsTrue(WikiRegexes.BareExternalLink.IsMatch(@"*   http://www.site.com
"));
            ClassicAssert.IsTrue(WikiRegexes.BareExternalLink.IsMatch(@"* http://www.site.com/great/a.htm
"));

            ClassicAssert.IsTrue(WikiRegexes.BareExternalLink.IsMatch(@"* https://www.site.com
"));
            ClassicAssert.IsTrue(WikiRegexes.BareExternalLink.IsMatch(@"* ftp://www.site.com
"));


            ClassicAssert.IsFalse(WikiRegexes.BareExternalLink.IsMatch(@"* [http://www.site.com text]
"));
            ClassicAssert.IsFalse(WikiRegexes.BareExternalLink.IsMatch(@"<ref>http://www.site.com</ref>
"));
        }

        [Test]
        public void BareRefExternalLink()
        {
            TestMatch(WikiRegexes.BareRefExternalLink, @"<ref>[http://news.bbc.co.uk/hi/England/story4384.htm]</ref>");
            TestMatch(WikiRegexes.BareRefExternalLink, @"<ref>[http://news.bbc.co.uk/hi/England/story4384.htm]]</ref>");
            TestMatch(WikiRegexes.BareRefExternalLink, @"<ref>http://news.bbc.co.uk/hi/England/story4384.htm</ref>");
            TestMatch(WikiRegexes.BareRefExternalLink, @"< REF > [   http://news.bbc.co.uk/hi/England/story4384.htm]
< / ref  >");
            TestMatch(WikiRegexes.BareRefExternalLink, @"<ref name=hello>[http://news.bbc.co.uk/hi/England/story4384.htm]</ref>");
            TestMatch(WikiRegexes.BareRefExternalLink, @"<ref>[   http://news.bbc.co.uk/hi/England/story4384.htm   ]   </ref>");

            TestMatch(WikiRegexes.BareRefExternalLink, @"<ref>[http://news.bbc.co.uk/hi/England/story4384.htm title here]</ref>", false);

            TestMatches(WikiRegexes.BareRefExternalLink, @"<ref>[http://news.bbc.co.uk/hi/England/story4384.htm</ref>", 0); // no matches for unbalanced braces

            Assert.That(WikiRegexes.BareRefExternalLink.Match(@"<ref>[ http://news.bbc.co.uk/hi/England/story4384.htm ] </ref>").Groups[1].Value, Is.EqualTo(@"http://news.bbc.co.uk/hi/England/story4384.htm"));
            Assert.That(WikiRegexes.BareRefExternalLink.Match(@"<ref>[ http://news.bbc.co.uk/hi/England/story4384.htm] </ref>").Groups[1].Value, Is.EqualTo(@"http://news.bbc.co.uk/hi/England/story4384.htm"));
            Assert.That(WikiRegexes.BareRefExternalLink.Match(@"<ref>[ http://news.bbc.co.uk/hi/England/story4384.htm]. </ref>").Groups[1].Value, Is.EqualTo(@"http://news.bbc.co.uk/hi/England/story4384.htm"));
        }

        [Test]
        public void BareRefExternalLinkBotGenTitle()
        {
            TestMatch(WikiRegexes.BareRefExternalLinkBotGenTitle, @"<ref>[http://news.bbc.co.uk/hi/England/story4384.htm]</ref>");
            TestMatch(WikiRegexes.BareRefExternalLinkBotGenTitle, @"<ref>http://news.bbc.co.uk/hi/England/story4384.htm</ref>");
            TestMatch(WikiRegexes.BareRefExternalLinkBotGenTitle, @"<ref>[http://www.independent.co.uk/news/people/bo-johnson-30403.html Boris Johnson: People - The Independent<!-- Bot generated title -->]</ref>");

            TestMatch(WikiRegexes.BareRefExternalLinkBotGenTitle, @"<ref>Smith, Fred [http://www.independent.co.uk/news/people/bo-johnson-30403.html Boris Johnson: People - The Independent<!-- Bot generated title -->]</ref>", false);
            TestMatch(WikiRegexes.BareRefExternalLinkBotGenTitle, @"<ref>[http://www.independent.co.uk/news/people/bo-johnson-30403.html Boris Johnson: People - The Independent]</ref>", false);
            ClassicAssert.IsTrue(WikiRegexes.BareRefExternalLinkBotGenTitle.IsMatch(@"attack<ref>http://www.news.com.au/heraldsun/story/0,21985,23169580-5006022,00.html</ref> was portrayed"));

            Assert.That(WikiRegexes.BareRefExternalLinkBotGenTitle.Match(@"<ref>[http://news.bbc.co.uk/hi/England/story4384.htm]</ref>").Groups[1].Value, Is.EqualTo(@"http://news.bbc.co.uk/hi/England/story4384.htm"));
            Assert.That(WikiRegexes.BareRefExternalLinkBotGenTitle.Match(@"<ref>[http://news.bbc.co.uk/hi/England/story4384.htm]""</ref>").Groups[1].Value, Is.EqualTo(@"http://news.bbc.co.uk/hi/England/story4384.htm"));
            Assert.That(WikiRegexes.BareRefExternalLinkBotGenTitle.Match(@"<ref> [ http://news.bbc.co.uk/hi/England/story4384.htm]. </ref>").Groups[1].Value, Is.EqualTo(@"http://news.bbc.co.uk/hi/England/story4384.htm"));
            Assert.That(WikiRegexes.BareRefExternalLinkBotGenTitle.Match(@"<ref> [ http://news.bbc.co.uk/hi/England/story4384.htm Foo<!--bot generated title-->]. </ref>").Groups[2].Value, Is.EqualTo(@"Foo"));
        }

        [Test]
        public void BoldItalicTests()
        {
            Assert.That(WikiRegexes.Bold.Match(@"'''foo'''").Groups[1].Value, Is.EqualTo(@"foo"));
            Assert.That(WikiRegexes.Bold.Match(@"'''foo bar'''").Groups[1].Value, Is.EqualTo(@"foo bar"));
            Assert.That(WikiRegexes.Bold.Match(@"'''foo's bar'''").Groups[1].Value, Is.EqualTo(@"foo's bar"));
            Assert.That(WikiRegexes.Bold.Match(@"'''''foo's bar'''''").Groups[1].Value, Is.EqualTo(""), "no match on bold italics");

            Assert.That(WikiRegexes.Italics.Match(@"''foo''").Groups[1].Value, Is.EqualTo(@"foo"));
            Assert.That(WikiRegexes.Italics.Match(@"''foo bar''").Groups[1].Value, Is.EqualTo(@"foo bar"));
            Assert.That(WikiRegexes.Italics.Match(@"''foo's bar''").Groups[1].Value, Is.EqualTo(@"foo's bar"));
            Assert.That(WikiRegexes.Italics.Match(@"'''foo's bar'''").Groups[1].Value, Is.EqualTo(""), "no match on bold");
            Assert.That(WikiRegexes.Italics.Match(@"'''''foo's bar'''''").Groups[1].Value, Is.EqualTo(""), "no match on bold italics");
            Assert.That(WikiRegexes.Italics.Match(@"'''Tyrone Station''' is an by Amtrak's ''[[foo]]'', which").Groups[1].Value, Is.EqualTo("[[foo]]"));

            Assert.That(WikiRegexes.BoldItalics.Match(@"''''' foo'''''").Groups[1].Value, Is.EqualTo(@" foo"));
            Assert.That(WikiRegexes.BoldItalics.Match(@"'''''foo bar'''''").Groups[1].Value, Is.EqualTo(@"foo bar"));
            Assert.That(WikiRegexes.BoldItalics.Match(@"'''''foo's bar'''''").Groups[1].Value, Is.EqualTo(@"foo's bar"));
            Assert.That(WikiRegexes.Italics.Match(@"''f''").Groups[1].Value, Is.EqualTo(@"f"));
            Assert.That(WikiRegexes.Italics.Match(@"''f'' nar ''abc''").Groups[1].Value, Is.EqualTo(@"f"));
            Assert.That(WikiRegexes.Bold.Match(@"'''f''' nar '''abc'''").Groups[1].Value, Is.EqualTo(@"f"));

            ClassicAssert.IsFalse(WikiRegexes.Italics.IsMatch(@"'''foo'''"));
            ClassicAssert.IsFalse(WikiRegexes.Italics.IsMatch(@"'''''foo'''''"));
            ClassicAssert.IsFalse(WikiRegexes.Bold.IsMatch(@"''foo'''"));
            ClassicAssert.IsFalse(WikiRegexes.Bold.IsMatch(@"''foo''"));
            ClassicAssert.IsFalse(WikiRegexes.BoldItalics.IsMatch(@"''foo''"));
            ClassicAssert.IsFalse(WikiRegexes.BoldItalics.IsMatch(@"'''foo'''"));
            ClassicAssert.IsFalse(WikiRegexes.BoldItalics.IsMatch(@"'''''foo''"));
        }

        [Test]
        public void StarRowsTests()
        {
            Assert.That(WikiRegexes.StarRows.Match(@"*foo bar
Bert").Groups[1].Value, Is.EqualTo(@"*"));
            Assert.That(WikiRegexes.StarRows.Match(@"*foo bar
Bert").Groups[2].Value, Is.EqualTo("foo bar\r"));

            Assert.That(WikiRegexes.StarRows.Match(@"    *foo bar").Groups[1].Value, Is.EqualTo(@"*"));
            Assert.That(WikiRegexes.StarRows.Match(@" *foo bar").Groups[2].Value, Is.EqualTo(@"foo bar"));
        }

        [Test]
        public void CircaTemplate()
        {
            ClassicAssert.IsTrue(WikiRegexes.CircaTemplate.IsMatch(@"{{circa}}"));
            ClassicAssert.IsTrue(WikiRegexes.CircaTemplate.IsMatch(@"{{ circa}}"));
            ClassicAssert.IsTrue(WikiRegexes.CircaTemplate.IsMatch(@"{{Circa}}"));
            ClassicAssert.IsTrue(WikiRegexes.CircaTemplate.IsMatch(@"{{circa|foo=yes}}"));
        }

        [Test]
        public void ReferenceList()
        {

#if DEBUG
            Variables.SetProjectLangCode("fr");
            WikiRegexes.MakeLangSpecificRegexes();
            ClassicAssert.IsTrue(WikiRegexes.ReferenceList.IsMatch(@"{{références}}"));
            ClassicAssert.IsTrue(WikiRegexes.ReferenceList.IsMatch(@"{{references}}"));
            ClassicAssert.IsTrue(WikiRegexes.ReferenceList.IsMatch(@"{{reflist}}"));

            Variables.SetProjectLangCode("en");
            WikiRegexes.MakeLangSpecificRegexes();
            ClassicAssert.IsTrue(WikiRegexes.ReferenceList.IsMatch(@"{{reflist}}"));
            ClassicAssert.IsTrue(WikiRegexes.ReferenceList.IsMatch(@"{{references-small}}"));
            ClassicAssert.IsTrue(WikiRegexes.ReferenceList.IsMatch(@"{{references-2column}}"));
#endif
        }

        [Test]
        public void SimpleWiki()
        {
#if DEBUG
            Variables.SetProjectLangCode("simple");
            WikiRegexes.MakeLangSpecificRegexes();
            ClassicAssert.IsTrue(WikiRegexes.SeeAlso.IsMatch(@"==Related pages=="));
            ClassicAssert.IsTrue(WikiRegexes.SeeAlso.IsMatch(@"==related pages=="));
            ClassicAssert.IsTrue(WikiRegexes.SeeAlso.IsMatch(@"==See also=="));

            ClassicAssert.IsTrue(WikiRegexes.ExternalLinksHeader.IsMatch(@"==Other websites=="));
            ClassicAssert.IsTrue(WikiRegexes.ExternalLinksHeader.IsMatch(@"==External links=="));

            Variables.SetProjectLangCode("en");
            WikiRegexes.MakeLangSpecificRegexes();
            ClassicAssert.IsFalse(WikiRegexes.SeeAlso.IsMatch(@"==Related pages=="));
            ClassicAssert.IsTrue(WikiRegexes.SeeAlso.IsMatch(@"==See also=="));
            ClassicAssert.IsTrue(WikiRegexes.ExternalLinksHeader.IsMatch(@"==External links=="));
#endif
        }

        [Test]
        public void SurnameClarificationTemplates()
        {
            ClassicAssert.IsTrue(WikiRegexes.SurnameClarificationTemplates.IsMatch(@"{{Malay name}}"));
            ClassicAssert.IsTrue(WikiRegexes.SurnameClarificationTemplates.IsMatch(@"{{Malay_name}}"));
            ClassicAssert.IsTrue(WikiRegexes.SurnameClarificationTemplates.IsMatch(@"{{Chinese name}}"));
            ClassicAssert.IsFalse(WikiRegexes.SurnameClarificationTemplates.IsMatch(@"{{Eastern name order}}"));
        }

        [Test]
        public void ReversedItalics()
        {
            ClassicAssert.IsTrue(WikiRegexes.ReversedItalics.IsMatch(@"</i>foo<i>"));
            ClassicAssert.IsTrue(WikiRegexes.ReversedItalics.IsMatch(@"</i>foo< i >"));
            ClassicAssert.IsTrue(WikiRegexes.ReversedItalics.IsMatch(@"< /i >foo<i>"));
            Assert.That(WikiRegexes.ReversedItalics.Match(@"</i>foo<i>").Groups[1].Value, Is.EqualTo("foo"));

            ClassicAssert.IsFalse(WikiRegexes.ReversedItalics.IsMatch(@"<i>foo</i>"));
            ClassicAssert.IsFalse(WikiRegexes.ReversedItalics.IsMatch(@"<i>foo<i>"));
            ClassicAssert.IsFalse(WikiRegexes.ReversedItalics.IsMatch(@"<i>foo</i> and <i>foo</i>"));
        }

        [Test]
        public void ShortPagesMonitor()
        {
            const string LC = @"{{Short pages monitor}}<!-- This long comment was added to the page to prevent it from being listed on Special:Shortpages. It and the accompanying monitoring template were generated via Template:Long comment. Please do not remove the monitor template without removing the comment as well.-->";

            ClassicAssert.IsTrue(WikiRegexes.ShortPagesMonitor.IsMatch(LC));
            ClassicAssert.IsTrue(WikiRegexes.ShortPagesMonitor.IsMatch(LC.Replace("{{S", "{{s")), "handles template name first letter case insensitive");
            ClassicAssert.IsTrue(WikiRegexes.ShortPagesMonitor.IsMatch(@"{{Short pages monitor}}<!-- any old comment-->"));
        }

        [Test]
        public void GoodFeaturedArticleTemplates()
        {
            ClassicAssert.IsTrue(WikiRegexes.GoodFeaturedArticleTemplates.IsMatch(@"{{Featured list}}"));
            ClassicAssert.IsTrue(WikiRegexes.GoodFeaturedArticleTemplates.IsMatch(@"{{Featured article}}"));
            ClassicAssert.IsTrue(WikiRegexes.GoodFeaturedArticleTemplates.IsMatch(@"{{Good article}}"));
        }

        [Test]
        public void UseDatesEnglishTemplates()
        {
            ClassicAssert.IsTrue(WikiRegexes.UseDatesEnglishTemplates.IsMatch("{{Use American English}}"));
            ClassicAssert.IsTrue(WikiRegexes.UseDatesEnglishTemplates.IsMatch("{{Use Antiguan and Barbudan English}}"));
            ClassicAssert.IsTrue(WikiRegexes.UseDatesEnglishTemplates.IsMatch("{{Use Australian English}}"));
            ClassicAssert.IsTrue(WikiRegexes.UseDatesEnglishTemplates.IsMatch("{{Use Bangladeshi English}}"));
            ClassicAssert.IsTrue(WikiRegexes.UseDatesEnglishTemplates.IsMatch("{{Use British English}}"));
            ClassicAssert.IsTrue(WikiRegexes.UseDatesEnglishTemplates.IsMatch("{{Use Oxford spelling}}"));
            ClassicAssert.IsTrue(WikiRegexes.UseDatesEnglishTemplates.IsMatch("{{Use Canadian English}}"));
            ClassicAssert.IsTrue(WikiRegexes.UseDatesEnglishTemplates.IsMatch("{{Use Ghanaian English}}"));
            ClassicAssert.IsTrue(WikiRegexes.UseDatesEnglishTemplates.IsMatch("{{Use Hiberno-English}}"));
            ClassicAssert.IsTrue(WikiRegexes.UseDatesEnglishTemplates.IsMatch("{{Use Hong Kong English}}"));
            ClassicAssert.IsTrue(WikiRegexes.UseDatesEnglishTemplates.IsMatch("{{Use Indian English}}"));
            ClassicAssert.IsTrue(WikiRegexes.UseDatesEnglishTemplates.IsMatch("{{Use Jamaican English}}"));
            ClassicAssert.IsTrue(WikiRegexes.UseDatesEnglishTemplates.IsMatch("{{Use Kenyan English}}"));
            ClassicAssert.IsTrue(WikiRegexes.UseDatesEnglishTemplates.IsMatch("{{Use Liberian English}}"));
            ClassicAssert.IsTrue(WikiRegexes.UseDatesEnglishTemplates.IsMatch("{{Use Malaysian English}}"));
            ClassicAssert.IsTrue(WikiRegexes.UseDatesEnglishTemplates.IsMatch("{{Use New Zealand English}}"));
            ClassicAssert.IsTrue(WikiRegexes.UseDatesEnglishTemplates.IsMatch("{{Use Nigerian English}}"));
            ClassicAssert.IsTrue(WikiRegexes.UseDatesEnglishTemplates.IsMatch("{{Use Pakistani English}}"));
            ClassicAssert.IsTrue(WikiRegexes.UseDatesEnglishTemplates.IsMatch("{{Use Philippine English}}"));
            ClassicAssert.IsTrue(WikiRegexes.UseDatesEnglishTemplates.IsMatch("{{Use Singapore English}}"));
            ClassicAssert.IsTrue(WikiRegexes.UseDatesEnglishTemplates.IsMatch("{{Use South African English}}"));
            ClassicAssert.IsTrue(WikiRegexes.UseDatesEnglishTemplates.IsMatch("{{Use Sri Lankan English}}"));
            ClassicAssert.IsTrue(WikiRegexes.UseDatesEnglishTemplates.IsMatch("{{Use Tanzanian English}}"));
            ClassicAssert.IsTrue(WikiRegexes.UseDatesEnglishTemplates.IsMatch("{{Use Trinidad and Tobago English}}"));
            ClassicAssert.IsTrue(WikiRegexes.UseDatesEnglishTemplates.IsMatch("{{Use Ugandan English}}"));
        }
    }
}
