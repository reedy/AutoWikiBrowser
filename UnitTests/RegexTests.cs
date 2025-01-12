﻿using System.Text.RegularExpressions;
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
        }

        [Test]
        public void NamedReferences()
        {
            Assert.IsTrue(WikiRegexes.NamedReferences.IsMatch(@"<ref name = ""foo"">text</ref>"));
            Assert.IsTrue(WikiRegexes.NamedReferences.IsMatch(@"<ref name = ""foo""></ref>"));
            Assert.IsTrue(WikiRegexes.NamedReferences.IsMatch(@"<ref name =foo>text</ref>"));
            Assert.IsTrue(WikiRegexes.NamedReferences.IsMatch(@"<ref name=foo>text< / ref >"));
            Assert.IsTrue(WikiRegexes.NamedReferences.IsMatch(@"<ref name=foo>te<taga</tag>xt</ref>"), "nested tag support");
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

            Assert.IsTrue(WikiRegexes.NamedReferencesIncludingCondensed.IsMatch(@"<ref name = ""foo"">text</ref>"));
            Assert.IsTrue(WikiRegexes.NamedReferencesIncludingCondensed.IsMatch(@"<ref name = ""fo/o"">text</ref>"));
            Assert.IsTrue(WikiRegexes.NamedReferencesIncludingCondensed.IsMatch(@"<ref name = ""foo"">text<tag>a</tag></ref>"), "matches with nested tags");
            Assert.IsTrue(WikiRegexes.NamedReferencesIncludingCondensed.IsMatch(@"<ref name = ""foo""></ref>"));
            Assert.IsTrue(WikiRegexes.NamedReferencesIncludingCondensed.IsMatch(@"<ref Name =foo>text</ref>"));
            Assert.IsTrue(WikiRegexes.NamedReferencesIncludingCondensed.IsMatch(@"<ref name=foo>text< / ref >"));
            Assert.IsTrue(WikiRegexes.NamedReferencesIncludingCondensed.IsMatch(@"<ref name=foo>te<taga</tag>xt</ref>"), "nested tag support");
            Assert.IsTrue(WikiRegexes.NamedReferencesIncludingCondensed.IsMatch(@"<ref name = 'foo'>text</ref>"));
            Assert.IsTrue(WikiRegexes.NamedReferencesIncludingCondensed.IsMatch(@"< ref NAME = ""foo"">text</ref>"));
            Assert.IsTrue(WikiRegexes.NamedReferencesIncludingCondensed.IsMatch(@"< ref NAME = ""foo"" />"), "matches condensed named ref");
            Assert.IsTrue(WikiRegexes.NamedReferencesIncludingCondensed.IsMatch(@"< ref NAME = ""foo"">
{{ cite web|
title = text |
work = text
}}</ref>"), "matches multiple line ref");
            Assert.IsTrue(WikiRegexes.NamedReferencesIncludingCondensed.IsMatch(@"< ref NAME = ""foo"">text <br>more</ref>"), "case insensitive matching");

            Assert.AreEqual(@"<ref name=""vietnam.ttu.edu""/>", WikiRegexes.NamedReferencesIncludingCondensed.Match(@"<ref name=""vietnam.ttu.edu""/><ref name=""Shul726"">Shul, p. 726</ref>").Value, "match is not across consecutive references – first condensed");
            Assert.AreEqual(@"Shul726", WikiRegexes.NamedReferencesIncludingCondensed.Match(@"<ref name=""Shul726"">Shul, p. 726</ref>").Groups[2].Value, "ref name is group 2");
            Assert.AreEqual(@"Shul, p. 726", WikiRegexes.NamedReferencesIncludingCondensed.Match(@"<ref name=""Shul726"">Shul, p. 726</ref>").Groups[3].Value, "ref value is group 3");
            Assert.AreEqual(@"Shul726", WikiRegexes.NamedReferencesIncludingCondensed.Match(@"<ref name=""Shul726"">
Shul, p. 726    </ref>").Groups[2].Value, "ref value doesn't include leading/trailing whitespace");
        }

        [Test]
        public void UnformattedTextTests()
        {
            Assert.IsTrue(WikiRegexes.UnformattedText.IsMatch(@"<pre>{{abc}}</pre>"));
            Assert.IsTrue(WikiRegexes.UnformattedText.IsMatch(@"<math>{{abc}}</math>"));
            Assert.IsTrue(WikiRegexes.UnformattedText.IsMatch(@"<math chem>{{abc}}</math>"));
            Assert.IsTrue(WikiRegexes.UnformattedText.IsMatch(@"<chem>{{abc}}</chem>"));
            Assert.IsTrue(WikiRegexes.UnformattedText.IsMatch(@"<nowiki>{{abc}}</nowiki>"));
            Assert.IsTrue(WikiRegexes.UnformattedText.IsMatch(@"now hello {{bye}} <pre>{now}}</pre>"));
            Assert.IsTrue(WikiRegexes.UnformattedText.IsMatch(@"<!--{{abc}}-->"));
            Assert.IsFalse(WikiRegexes.UnformattedText.IsMatch(@"<pre>{{abc}}</nowiki>"), "Does not match unbalanced tags");
        }
        
        [Test]
        public void AllTags()
        {
            Assert.IsTrue(WikiRegexes.AllTags.IsMatch(@"<pre>{{abc}}</pre>"));
            Assert.IsTrue(WikiRegexes.AllTags.IsMatch(@"<math>{{abc}}</math>"));
            Assert.IsTrue(WikiRegexes.AllTags.IsMatch(@"<nowiki>{{abc}}</nowiki>"));
            Assert.IsTrue(WikiRegexes.AllTags.IsMatch(@"<timeline>{{abc}}</timeline>"));
            Assert.IsTrue(WikiRegexes.AllTags.IsMatch(@"<cite>{{abc}}</cite>"));
            Assert.IsTrue(WikiRegexes.AllTags.IsMatch(@"<blockquote>{{abc}}</blockquote>"));
            Assert.IsTrue(WikiRegexes.AllTags.IsMatch(@"<BLOCKQUOTE>{{abc}}</BLOCKQUOTE>"));
            Assert.IsTrue(WikiRegexes.AllTags.IsMatch(@"<poem>{{abc}}</poem>"));
            Assert.IsTrue(WikiRegexes.AllTags.IsMatch(@"<imagemap>{{abc}}</imagemap>"));
            Assert.IsTrue(WikiRegexes.AllTags.IsMatch(@"<noinclude>{{abc}}</noinclude>"));
            Assert.IsTrue(WikiRegexes.AllTags.IsMatch(@"<includeonly>{{abc}}</includeonly>"));
            Assert.IsTrue(WikiRegexes.AllTags.IsMatch(@"<onlyinclude>{{abc}}</onlyinclude>"));
            Assert.IsTrue(WikiRegexes.AllTags.IsMatch(@"<hiero>abc</hiero>"));
            Assert.IsTrue(WikiRegexes.AllTags.IsMatch(@"<score>{a,, c, e, a, c e a c' e' a' c'' e'' a'' c''' e''' g''' \bar</score>"));
            
            Assert.IsTrue(WikiRegexes.AllTags.IsMatch(@"now hello {{bye}} <pre>{now}}</pre>"));
            Assert.IsFalse(WikiRegexes.AllTags.IsMatch(@"<!--{{abc}}-->"));
            Assert.IsFalse(WikiRegexes.AllTags.IsMatch(@"<pre>{{abc}}</nowiki>"), "Does not match unbalanced tags");
            
            Assert.IsTrue(WikiRegexes.AllTags.IsMatch(@"<pre>{{abc}}</pre>"));
            Assert.IsTrue(WikiRegexes.AllTags.IsMatch(@"<code>{{abc}}</code>"));
            Assert.IsTrue(WikiRegexes.AllTags.IsMatch(@"<source lang=xml>{{abc}}</source>"));
            Assert.IsTrue(WikiRegexes.AllTags.IsMatch(@"<syntaxhighlight lang=xml>{{abc}}</syntaxhighlight>"));
            Assert.IsTrue(WikiRegexes.AllTags.IsMatch(@"<source>{{abc}}</source>"));
            Assert.IsTrue(WikiRegexes.AllTags.IsMatch(@"<syntaxhighlight>{{abc}}</syntaxhighlight>"));
            Assert.IsTrue(WikiRegexes.AllTags.IsMatch(@"now hello {{bye}} <pre>{now}}</pre>"));
            Assert.IsTrue(WikiRegexes.AllTags.IsMatch(@"<math>{{abc}}</math>"));
            Assert.IsTrue(WikiRegexes.AllTags.IsMatch(@"<chem>{{abc}}</chem>"));
            Assert.IsTrue(WikiRegexes.AllTags.IsMatch(@"<math chem>{{abc}}</math>"));

            Assert.IsTrue(WikiRegexes.AllTags.IsMatch(@"<pre>now <br> now </pre>"));
            
            Assert.IsTrue(WikiRegexes.AllTags.IsMatch(@"< pre >{{abc}}< / pre >"));
            Assert.IsTrue(WikiRegexes.AllTags.IsMatch(@"< pre > {{abc}} < / pre >"));
            Assert.IsTrue(WikiRegexes.AllTags.IsMatch(@"< pre >
{{abc}}
< / pre >"));
            Assert.IsTrue(WikiRegexes.AllTags.IsMatch(@"<
 pre > {{abc}} < / pre
 >"));
            Assert.AreEqual(WikiRegexes.AllTags.Match(@"<nowiki>now <math>{{abc}}</math> now </nowiki>").Value, "<nowiki>now <math>{{abc}}</math> now </nowiki>");
        }

        [Test]
        public void SourceCodeTests()
        {
            Assert.IsTrue(WikiRegexes.SourceCode.IsMatch(@"<code>{{abc}}</code>"));
            Assert.IsTrue(WikiRegexes.SourceCode.IsMatch(@"<source lang=xml>{{abc}}</source>"));
            Assert.IsTrue(WikiRegexes.SourceCode.IsMatch(@"<syntaxhighlight lang=xml>{{abc}}</syntaxhighlight>"));            
            Assert.IsTrue(WikiRegexes.SourceCode.IsMatch(@"<syntaxhighlight lang=""xml"">{{abc}}</syntaxhighlight>"));
            Assert.IsTrue(WikiRegexes.SourceCode.IsMatch(@"<syntaxhighlight lang=""javascript"">\r\n var x; //defines the variable x</syntaxhighlight>"));
            Assert.IsTrue(WikiRegexes.SourceCode.IsMatch(@"<source>{{abc}}</source>"));
            Assert.IsTrue(WikiRegexes.SourceCode.IsMatch(@"<syntaxhighlight>{{abc}}</syntaxhighlight>"));
            Assert.IsTrue(WikiRegexes.SourceCode.IsMatch(@"<tt>{{abc}}</tt>"));

            Assert.IsFalse(WikiRegexes.SourceCode.IsMatch(@"<math>{{abc}}</math>"));
            Assert.IsFalse(WikiRegexes.SourceCode.IsMatch(@"<pre>{{abc}}</pre>"));
        }
        
        [Test]
        public void MathPreSourceCodeTests()
        {
            Assert.IsTrue(WikiRegexes.MathPreSourceCodeComments.IsMatch(@"<pre>{{abc}}</pre>"));
            Assert.IsTrue(WikiRegexes.MathPreSourceCodeComments.IsMatch(@"<!--{{abc}}-->"));
            Assert.IsTrue(WikiRegexes.MathPreSourceCodeComments.IsMatch(@"<code>{{abc}}</code>"));
            Assert.IsTrue(WikiRegexes.MathPreSourceCodeComments.IsMatch(@"<source lang=xml>{{abc}}</source>"));
            Assert.IsTrue(WikiRegexes.MathPreSourceCodeComments.IsMatch(@"<syntaxhighlight lang=xml>{{abc}}</syntaxhighlight>"));            
            Assert.IsTrue(WikiRegexes.MathPreSourceCodeComments.IsMatch(@"<syntaxhighlight lang=""xml"">{{abc}}</syntaxhighlight>"));
            Assert.IsTrue(WikiRegexes.SourceCode.IsMatch(@"<syntaxhighlight lang=""javascript"">\r\n var x; //defines the variable x</syntaxhighlight>"));
            Assert.IsTrue(WikiRegexes.MathPreSourceCodeComments.IsMatch(@"<source>{{abc}}</source>"));
            Assert.IsTrue(WikiRegexes.MathPreSourceCodeComments.IsMatch(@"<syntaxhighlight>{{abc}}</syntaxhighlight>"));
            Assert.IsTrue(WikiRegexes.MathPreSourceCodeComments.IsMatch(@"now hello {{bye}} <pre>{now}}</pre>"));
            Assert.IsTrue(WikiRegexes.MathPreSourceCodeComments.IsMatch(@"<math>{{abc}}</math>"));
            Assert.IsTrue(WikiRegexes.MathPreSourceCodeComments.IsMatch(@"<chem>{{abc}}</chem>"));
            Assert.IsTrue(WikiRegexes.MathPreSourceCodeComments.IsMatch(@"<math chem>{{abc}}</math>"));
            Assert.IsTrue(WikiRegexes.SourceCode.IsMatch(@"<tt>{{abc}}</tt>"));

            Assert.IsFalse(WikiRegexes.MathPreSourceCodeComments.IsMatch(@"<nowiki>{{abc}}</nowiki>"));
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
            Assert.AreEqual(WikiRegexes.WikiLink.Match(@"[[foo]]").Groups[1].Value, @"foo");
            Assert.AreEqual(WikiRegexes.WikiLink.Match(@"[[foo|bar]]").Groups[1].Value, @"foo");
            Assert.AreEqual(WikiRegexes.WikiLink.Match(@"[[foo bar]]").Groups[1].Value, @"foo bar");
            Assert.AreEqual(WikiRegexes.WikiLink.Match(@"[[Foo]]").Groups[1].Value, @"Foo");
            Assert.AreEqual(WikiRegexes.WikiLink.Match(@"[[foo bar|word here]]").Groups[1].Value, @"foo bar");
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

            Assert.AreEqual("foo", WikiRegexes.PipedWikiLink.Match("[[foo|bar]]").Groups[1].Value);
            Assert.AreEqual("bar", WikiRegexes.PipedWikiLink.Match("[[foo|bar]]").Groups[2].Value);
        }

        [Test]
        public void UnPipedWikiLink()
        {
            TestMatch(WikiRegexes.UnPipedWikiLink, "[[foo]]");
            TestMatch(WikiRegexes.UnPipedWikiLink, "a [[foo boo ]] !one", "[[foo boo ]]");
            TestMatch(WikiRegexes.UnPipedWikiLink, "[[foo|bar]]", false);
            Assert.AreEqual("foo", WikiRegexes.UnPipedWikiLink.Match("[[foo]]").Groups[1].Value);
            Assert.AreEqual("foo bar", WikiRegexes.UnPipedWikiLink.Match("[[foo bar]]").Groups[1].Value);
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
        public void Headings()
        {
            RegexAssert.NoMatch(WikiRegexes.Headings, "");
            RegexAssert.IsMatch(WikiRegexes.Headings, "=Foo=");
            RegexAssert.IsMatch(WikiRegexes.Headings, "=Foo=<!--comm-->");
            RegexAssert.IsMatch(WikiRegexes.Headings, "==Foo==");
            RegexAssert.IsMatch(WikiRegexes.Headings, "======Foo======");
            Assert.AreEqual(WikiRegexes.Headings.Match("======Foo======").Groups[1].Value, "Foo");
            Assert.AreEqual(WikiRegexes.Headings.Match("== Foo == ").Groups[1].Value, "Foo");

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
            Assert.AreEqual("Foo", WikiRegexes.UnnamedReferences.Match("<ref>Foo</ref>").Groups[1].Value);
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
            Assert.AreEqual(WikiRegexes.Small.Match("<small>foo</small>").Groups[1].Value, "foo");
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
            Assert.AreEqual("http", WikiRegexes.ExternalLinks.Match(@"http://google.co.uk").Groups[1].Value);
            Assert.AreEqual("https", WikiRegexes.ExternalLinks.Match(@"https://google.co.uk").Groups[1].Value);
            Assert.AreEqual("svn", WikiRegexes.ExternalLinks.Match(@"svn://google.co.uk Google}}").Groups[1].Value);
            Assert.AreEqual("Http", WikiRegexes.ExternalLinks.Match(@"Http://google.co.uk").Groups[1].Value);
            Assert.AreEqual("HTTP", WikiRegexes.ExternalLinks.Match(@"HTTP://google.co.uk").Groups[1].Value);

            // not when in external link brackets
            Assert.AreEqual("", WikiRegexes.ExternalLinks.Match(@"[http://google.co.uk Google]").Groups[1].Value);
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
            Assert.AreEqual("http", WikiRegexes.ExternalLinksHTTPOnly.Match(@"http://google.co.uk").Groups[1].Value);
            Assert.AreEqual("Http", WikiRegexes.ExternalLinksHTTPOnly.Match(@"Http://google.co.uk").Groups[1].Value);
            Assert.AreEqual("HTTP", WikiRegexes.ExternalLinksHTTPOnly.Match(@"HTTP://google.co.uk").Groups[1].Value);

            // not when in external link brackets
            Assert.AreEqual("", WikiRegexes.ExternalLinksHTTPOnly.Match(@"[http://google.co.uk Google]").Groups[1].Value);
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

            Assert.AreEqual("en", WikiRegexes.PossibleInterwikis.Match("[[ en :bar]]").Groups[1].Value);
            Assert.AreEqual("bar", WikiRegexes.PossibleInterwikis.Match("[[en: bar ]]").Groups[2].Value);
            Assert.AreEqual(" <!--comm-->", WikiRegexes.PossibleInterwikis.Match("[[en: bar ]] <!--comm-->").Groups[3].Value);

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

            Assert.IsFalse(WikiRegexes.UntemplatedQuotes.Replace(@" ""very fast"" ", "1").Contains(@"""very fast"""));
            Assert.IsFalse(WikiRegexes.UntemplatedQuotes.Replace(@"""very fast"" ", "1").Contains(@"""very fast"""));
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
        public void CurlyDoubleQuotes()
        {
            Assert.IsTrue(WikiRegexes.CurlyDoubleQuotes.IsMatch(@" “ very fast ”"));
            Assert.IsTrue(WikiRegexes.CurlyDoubleQuotes.IsMatch(@"very fast „"));
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
            Assert.IsTrue(Tools.NestedTemplateRegex(WikiRegexes.RFromTitleWithoutDiacriticsList).IsMatch(@"{{R to diacritic}}"));
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
            Assert.IsTrue(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello<ref>Fred</ref> <references responsive/>"));
            Assert.IsTrue(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello<ref>Fred</ref> <references responsive=""0""/>"));
            Assert.IsTrue(WikiRegexes.ReferencesTemplate.IsMatch(@"Hello<ref>Fred</ref> <References />"));
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
            Assert.IsTrue(WikiRegexes.CiteTemplate.IsMatch(@"{{Cite web|url=a|title=b and {{foo}} there}}"));
            Assert.IsTrue(WikiRegexes.CiteTemplate.IsMatch(@"{{cite news|url=a|title=b}}"));
            Assert.IsTrue(WikiRegexes.CiteTemplate.IsMatch(@"{{cite book|url=a|title=b}}"));
            Assert.IsTrue(WikiRegexes.CiteTemplate.IsMatch(@"{{cite conference|url=a|title=b}}"));
            Assert.IsTrue(WikiRegexes.CiteTemplate.IsMatch(@"{{cite manual|url=a|title=b}}"));
            Assert.IsTrue(WikiRegexes.CiteTemplate.IsMatch(@"{{cite paper|url=a|title=b}}"));
            Assert.IsTrue(WikiRegexes.CiteTemplate.IsMatch(@"{{cite press release|url=a|title=b}}"));
            Assert.IsTrue(WikiRegexes.CiteTemplate.IsMatch(@"{{cite encyclopedia|url=a|title=b}}"));
            Assert.IsTrue(WikiRegexes.CiteTemplate.IsMatch(@"{{cite AV media|url=a|title=b}}"));
            Assert.IsTrue(WikiRegexes.CiteTemplate.IsMatch(@"{{vcite2 journal}}"));
            Assert.IsTrue(WikiRegexes.CiteTemplate.IsMatch(@"{{cite magazine|url=a|title=b}}"));
            Assert.IsTrue(WikiRegexes.CiteTemplate.IsMatch(@"{{cite report|url=a|title=b}}"));
            
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
            Assert.IsTrue(WikiRegexes.IbidLocCitation.IsMatch(@"ibid"));
            Assert.IsTrue(WikiRegexes.IbidLocCitation.IsMatch(@"Ibid"));
            Assert.IsTrue(WikiRegexes.IbidLocCitation.IsMatch(@"IBID"));
            Assert.IsTrue(WikiRegexes.IbidLocCitation.IsMatch(@"loc cit"));
            Assert.IsTrue(WikiRegexes.IbidLocCitation.IsMatch(@"loc.cit"));
            Assert.IsTrue(WikiRegexes.IbidLocCitation.IsMatch(@"Loc. cit"));
            Assert.IsTrue(WikiRegexes.IbidLocCitation.IsMatch(@"Loc
cit"));

            Assert.IsFalse(WikiRegexes.IbidLocCitation.IsMatch(@"Libid was"));
            Assert.IsFalse(WikiRegexes.IbidLocCitation.IsMatch(@"The loc was later cit"));
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
        public void WikipediaBooks()
        {
            Assert.IsTrue(WikiRegexes.WikipediaBooks.IsMatch(@"{{Wikipedia-Books}}"));
            Assert.IsTrue(WikiRegexes.WikipediaBooks.IsMatch(@"{{ Wikipedia books }}"));
            Assert.IsTrue(WikiRegexes.WikipediaBooks.IsMatch(@"{{Wikipedia books|1=Academy Awards|3=Academy Awards for Best Picture}}"));
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
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{other persons}}"));
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{otheruse
|something}}"));
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{selfref|something}}"));
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{Redirect-distinguish|something}}"));
            
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{About-distinguish2}}"), @"{{About-distinguish2}}");
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{About-distinguish}}"), @"{{About-distinguish}}");
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{about2}}"), @"{{about2}}");
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{about}}"), @"{{about}}");
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{distinguish-otheruses2}}"), @"{{distinguish-otheruses2}}");
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{distinguish-otheruses}}"), @"{{distinguish-otheruses}}");
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{distinguish2}}"), @"{{distinguish2}}");
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{distinguish}}"), @"{{distinguish}}");
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{for2}}"), @"{{for2}}");
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{for}}"), @"{{for}}");
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{further2}}"), @"{{further2}}");
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{further}}"), @"{{further}}");
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{hatnote}}"), @"{{hatnote}}");
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{other hurricanes}}"), @"{{other hurricanes}}");
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{other people2}}"), @"{{other people2}}");
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{other people3}}"), @"{{other people3}}");
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{other people}}"), @"{{other people}}");
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{other places3}}"), @"{{other places3}}");
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{other places}}"), @"{{other places}}");
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{other ships}}"), @"{{other ships}}");
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{other uses of}}"), @"{{other uses of}}");
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{other uses2}}"), @"{{other uses2}}");
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{other uses}}"), @"{{other uses}}");
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{outline}}"), @"{{outline}}");
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{redirect-distinguish2}}"), @"{{redirect-distinguish2}}");
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{redirect-distinguish}}"), @"{{redirect-distinguish}}");
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{redirect-several}}"), @"{{redirect-several}}");
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{redirect-multi}}"), @"{{redirect-multi}}");
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{redirect2}}"), @"{{redirect2}}");
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{redirect3}}"), @"{{redirect3}}");
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{redirect}}"), @"{{redirect}}");
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{see also}}"), @"{{see also}}");
            Assert.IsTrue(WikiRegexes.Dablinks.IsMatch(@"{{selfref}}"), @"{{selfref}}");

            Assert.IsFalse(WikiRegexes.Dablinks.IsMatch(@"{{For fake template|Fred the dancer|Fred(dancer)}}"));
            Assert.IsFalse(WikiRegexes.Dablinks.IsMatch(@"{{REDIRECT2|Fred the dancer|Fred Smith (dancer)}}"));
            Assert.IsFalse(WikiRegexes.Dablinks.IsMatch(@"{{Otheruse2|something}}")); // non-existent
        }

        [Test]
        public void SisterLinksTests()
        {
            Assert.IsTrue(WikiRegexes.SisterLinks.IsMatch(@"{{wiktionary}}"));
            Assert.IsTrue(WikiRegexes.SisterLinks.IsMatch(@"{{ wiktionary }}"));
            Assert.IsTrue(WikiRegexes.SisterLinks.IsMatch(@"{{sisterlinks}}"));
            Assert.IsTrue(WikiRegexes.SisterLinks.IsMatch(@"{{sister links}}"));
            Assert.IsTrue(WikiRegexes.SisterLinks.IsMatch(@"{{sister project links}}"));
            Assert.IsTrue(WikiRegexes.SisterLinks.IsMatch(@"{{wikibooks}}"));
            Assert.IsTrue(WikiRegexes.SisterLinks.IsMatch(@"{{wikimedia}}"));
            Assert.IsTrue(WikiRegexes.SisterLinks.IsMatch(@"{{wikiversity}}"));
        }

        [Test]
        public void Unreferenced()
        {
            Assert.IsTrue(WikiRegexes.Unreferenced.IsMatch(@"{{unreferenced}}"));
            Assert.IsTrue(WikiRegexes.Unreferenced.IsMatch(@"{{  Unreferenced}}"));
            Assert.IsTrue(WikiRegexes.Unreferenced.IsMatch(@"{{unreferenced  }}"));
            Assert.IsTrue(WikiRegexes.Unreferenced.IsMatch(@"{{unreferenced|date=May 2009}}"));
            Assert.IsTrue(WikiRegexes.Unreferenced.IsMatch(@"{{unreferenced stub|date=May 2009}}"));

            Assert.IsFalse(WikiRegexes.Unreferenced.IsMatch(@"{{unreferenced-stub}}"));
        }

        [Test]
        public void PortalTemplateTests()
        {
            Assert.IsTrue(WikiRegexes.PortalTemplate.IsMatch(@"{{portal}}"));
            Assert.IsTrue(WikiRegexes.PortalTemplate.IsMatch(@"{{portal|Foo}}"));
            Assert.IsTrue(WikiRegexes.PortalTemplate.IsMatch(@"{{ portal}}"));
            Assert.IsTrue(WikiRegexes.PortalTemplate.IsMatch(@"{{Portal}}"));
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
            Assert.IsTrue(WikiRegexes.InfoBox.IsMatch(@" {{Template:infobox hello| bye}} "));
            Assert.IsTrue(WikiRegexes.InfoBox.IsMatch(@" {{infobox hello
| bye}} "));
            Assert.IsTrue(WikiRegexes.InfoBox.IsMatch(@" {{Infobox_play| bye}} "));
            Assert.IsTrue(WikiRegexes.InfoBox.IsMatch(@" {{some infobox| hello| bye}} "));
            Assert.IsTrue(WikiRegexes.InfoBox.IsMatch(@" {{Some Infobox| hello| bye}} "));

            Assert.AreEqual(WikiRegexes.InfoBox.Match(@" {{Infobox
| hello| bye}} ").Groups[1].Value, "Infobox");
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
            Assert.AreEqual(WikiRegexes.TemplateEnd.Match(@"{{foo
 }}").Value, "\r\n }}");
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
        public void MultipleIssuesTests()
        {
            Assert.IsTrue(WikiRegexes.MultipleIssues.IsMatch(@"{{Multiple issues|wikify=May 2008|a=b|c=d}}"), "with space");
            Assert.IsTrue(WikiRegexes.MultipleIssues.IsMatch(@"{{Multipleissues|wikify=May 2008|a=b|c=d}}"), "unspaced");
            Assert.IsTrue(WikiRegexes.MultipleIssues.IsMatch(@"{{multipleissues|wikify=May 2008|a=b|c=d}}"), "unspaced no capitals");
            Assert.IsTrue(WikiRegexes.MultipleIssues.IsMatch(@"{{Multiple issues | wikify=May 2008|a=b|c=d}}"), "with spaces in parameters");
            Assert.IsTrue(WikiRegexes.MultipleIssues.IsMatch(@"{{Multiple issues
           | wikify=May 2008|a=b|c=d}}"), "with break lines");
            Assert.IsTrue(WikiRegexes.MultipleIssues.IsMatch(@"{{Multiple issues|}}"), "empty");
            Assert.IsTrue(WikiRegexes.MultipleIssues.IsMatch(@"{{Multipleissues}}"), "empty too");

            Assert.IsTrue(WikiRegexes.MultipleIssues.IsMatch(@"{{ multiple issues|wikify=May 2008|a=b|c=d}}"));

            Assert.IsTrue(WikiRegexes.MultipleIssues.IsMatch(@"{{ multiple issues|wikify={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|orphan={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|c=d}}"));

            // no matches
            Assert.IsFalse(WikiRegexes.MultipleIssues.IsMatch(@"{{ARTICLEISSUES }}"));
            Assert.IsFalse(WikiRegexes.MultipleIssues.IsMatch(@"{{Bert|Multipleissues }}"));
        }

        [Test]
        public void NonDeadEndPageTemplatesTests()
        {
            Assert.IsTrue(WikiRegexes.NonDeadEndPageTemplates.IsMatch(@"{{Events by year for decade|31}}"));
            Assert.IsTrue(WikiRegexes.NonDeadEndPageTemplates.IsMatch(@"{{Events by year for decade BC|31}}"));
            Assert.IsTrue(WikiRegexes.NonDeadEndPageTemplates.IsMatch(@"{{SCOTUSRow | case name = Arizona v. Inter Tribal Council of Ariz., Inc. | docket = 12-71 | decision date = June 17 | decision year = 2013}}"));
            Assert.IsTrue(WikiRegexes.NonDeadEndPageTemplates.IsMatch(@"{{Portal:Current events/Month Inclusion|2009 February}}"));
        }

        [Test]
        public void WordApostropheTests()
        {
            Assert.IsTrue(WikiRegexes.RegexWordApostrophes.IsMatch(@"Rachel"));
            Assert.IsTrue(WikiRegexes.RegexWordApostrophes.IsMatch(@"Rachel's"));
            Assert.IsTrue(WikiRegexes.RegexWordApostrophes.IsMatch(@"Rachel’s"), "curly apostrophe");
            Assert.IsTrue(WikiRegexes.RegexWordApostrophes.IsMatch(@"Kwakwaka'wakw"));

            Assert.AreEqual("", WikiRegexes.RegexWordApostrophes.Replace(@"Kwakwaka'wakw", ""));
        }

        [Test]
        public void DeathsOrLivingCategoryTests()
        {
            Assert.IsTrue(WikiRegexes.DeathsOrLivingCategory.IsMatch(@"[[Category:653 deaths|Honorius]]"), "xxx deaths with sortkey");
            Assert.IsTrue(WikiRegexes.DeathsOrLivingCategory.IsMatch(@"[[Category:839 deaths]]"), "xxx deaths");
            Assert.IsTrue(WikiRegexes.DeathsOrLivingCategory.IsMatch(@"[[Category:5th-century BC deaths]]"), "centure BC deaths");
            Assert.IsTrue(WikiRegexes.DeathsOrLivingCategory.IsMatch(@"[[Category:1209 deaths]]"), "xxxx deaths");
            Assert.IsTrue(WikiRegexes.DeathsOrLivingCategory.IsMatch(@"[[Category:2018 suicides]]"), "xxxx suicides");
            Assert.IsTrue(WikiRegexes.DeathsOrLivingCategory.IsMatch(@"[[Category:Living people]]"), "living people");
            Assert.IsTrue(WikiRegexes.DeathsOrLivingCategory.IsMatch(@"[[Category:Possibly living people]]"), "possibly living people");
            Assert.IsTrue(WikiRegexes.DeathsOrLivingCategory.IsMatch(@"[[Category:221 BC deaths]]"), "xxx BC deaths");
            Assert.IsTrue(WikiRegexes.DeathsOrLivingCategory.IsMatch(@"[[Category:Year of death missing]]"), "YOD missing");
            Assert.IsTrue(WikiRegexes.DeathsOrLivingCategory.IsMatch(@"[[Category:Year of death unknown]]"), "YOD unknown");
            Assert.IsTrue(WikiRegexes.DeathsOrLivingCategory.IsMatch(@"[[Category:Date of death unknown]]"), "DOD unknown");
            Assert.IsTrue(WikiRegexes.DeathsOrLivingCategory.IsMatch(@"[[Category:Year of death uncertain]]"), "YOD uncertain");
            Assert.IsTrue(WikiRegexes.DeathsOrLivingCategory.IsMatch(@"[[Category:Missing people]]"), "Mising people");
            Assert.IsTrue(WikiRegexes.DeathsOrLivingCategory.IsMatch(@"[[Category:People declared dead in absentia]]"), "People declared dead in absentia");
            Assert.IsTrue(WikiRegexes.DeathsOrLivingCategory.IsMatch(@"[[Category:839 deaths ]]"), "xxx deaths with space");

            // no matches
            Assert.IsFalse(WikiRegexes.DeathsOrLivingCategory.IsMatch(@""));
            Assert.IsFalse(WikiRegexes.DeathsOrLivingCategory.IsMatch(@"[[Category:strange deaths]]"));
            Assert.IsFalse(WikiRegexes.DeathsOrLivingCategory.IsMatch(@"[[Category:Missing people organizations]]"));
            Assert.IsFalse(WikiRegexes.DeathsOrLivingCategory.IsMatch(@"1990 deaths"));
        }

        [Test]
        public void BirthsCategoryTests()
        {
            Assert.IsTrue(WikiRegexes.BirthsCategory.IsMatch(@"[[Category:12th-century births]]"));
            Assert.IsTrue(WikiRegexes.BirthsCategory.IsMatch(@"[[Category:1299 births]]"));
            Assert.IsTrue(WikiRegexes.BirthsCategory.IsMatch(@"[[Category:110 BC births]]"));
            Assert.IsTrue(WikiRegexes.BirthsCategory.IsMatch(@"[[Category:1st-century births]]"));

            // no matches
            Assert.IsFalse(WikiRegexes.BirthsCategory.IsMatch(@"[[Category:strange births]]"));
            Assert.IsFalse(WikiRegexes.BirthsCategory.IsMatch(@"1960 births"));
            Assert.IsFalse(WikiRegexes.BirthsCategory.IsMatch(@""));
        }

        [Test]
        public void PeopleFromCategoryTests()
        {
            Assert.IsTrue(WikiRegexes.PeopleFromCategory.IsMatch(@"[[Category:People from x]]"));
            Assert.IsTrue(WikiRegexes.PeopleFromCategory.IsMatch(@"[[Category:People from x|y]]"));
            Assert.IsFalse(WikiRegexes.PeopleFromCategory.IsMatch(@"[[Category:People who x]]"));
        }

        [Test]
        public void DateBirthAndAgeTests()
        {
            Assert.IsTrue(WikiRegexes.DateBirthAndAge.IsMatch(@"{{Birth date|1972|02|18}}"));
            Assert.IsTrue(WikiRegexes.DateBirthAndAge.IsMatch(@"{{Birth date and age|1972|02|18}}"));
            Assert.IsTrue(WikiRegexes.DateBirthAndAge.IsMatch(@"{{Birth date| 1972 |02|18}}"));

            Assert.IsTrue(WikiRegexes.DateBirthAndAge.IsMatch(@"{{birth date and age|mf=yes|1980|3|9}}"));
            Assert.IsTrue(WikiRegexes.DateBirthAndAge.IsMatch(@"{{bda|mf=yes|1980|3|9}}"));

            Assert.AreEqual("1975", WikiRegexes.DateBirthAndAge.Match(@"{{birth-date|1975}}").Groups[1].Value, "extract year from birth-date");
            Assert.AreEqual("1975", WikiRegexes.DateBirthAndAge.Match(@"{{birth-date|   1975}}").Groups[1].Value, "spacing");
            Assert.AreEqual("1984", WikiRegexes.DateBirthAndAge.Match(@"{{birth date and age|year=1984|month=2|day=6}}").Groups[1].Value);

            Assert.AreEqual("{{Birth date|1972|02|18}}", WikiRegexes.DateBirthAndAge.Match(@"{{Birth date|1972|02|18}}").Value);
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

            Assert.IsTrue(WikiRegexes.BareExternalLink.IsMatch(@"* https://www.site.com
"));
            Assert.IsTrue(WikiRegexes.BareExternalLink.IsMatch(@"* ftp://www.site.com
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
            TestMatch(WikiRegexes.BareRefExternalLink, @"<ref>http://news.bbc.co.uk/hi/England/story4384.htm</ref>");
            TestMatch(WikiRegexes.BareRefExternalLink, @"< REF > [   http://news.bbc.co.uk/hi/England/story4384.htm]
< / ref  >");
            TestMatch(WikiRegexes.BareRefExternalLink, @"<ref name=hello>[http://news.bbc.co.uk/hi/England/story4384.htm]</ref>");
            TestMatch(WikiRegexes.BareRefExternalLink, @"<ref>[   http://news.bbc.co.uk/hi/England/story4384.htm   ]   </ref>");

            TestMatch(WikiRegexes.BareRefExternalLink, @"<ref>[http://news.bbc.co.uk/hi/England/story4384.htm title here]</ref>", false);

            TestMatches(WikiRegexes.BareRefExternalLink, @"<ref>[http://news.bbc.co.uk/hi/England/story4384.htm</ref>", 0); // no matches for unbalanced braces

            Assert.AreEqual(WikiRegexes.BareRefExternalLink.Match(@"<ref>[ http://news.bbc.co.uk/hi/England/story4384.htm ] </ref>").Groups[1].Value, @"http://news.bbc.co.uk/hi/England/story4384.htm");
            Assert.AreEqual(WikiRegexes.BareRefExternalLink.Match(@"<ref>[ http://news.bbc.co.uk/hi/England/story4384.htm] </ref>").Groups[1].Value, @"http://news.bbc.co.uk/hi/England/story4384.htm");
            Assert.AreEqual(WikiRegexes.BareRefExternalLink.Match(@"<ref>[ http://news.bbc.co.uk/hi/England/story4384.htm]. </ref>").Groups[1].Value, @"http://news.bbc.co.uk/hi/England/story4384.htm");
        }

        [Test]
        public void BareRefExternalLinkBotGenTitle()
        {
            TestMatch(WikiRegexes.BareRefExternalLinkBotGenTitle, @"<ref>[http://news.bbc.co.uk/hi/England/story4384.htm]</ref>");
            TestMatch(WikiRegexes.BareRefExternalLinkBotGenTitle, @"<ref>http://news.bbc.co.uk/hi/England/story4384.htm</ref>");
            TestMatch(WikiRegexes.BareRefExternalLinkBotGenTitle, @"<ref>[http://www.independent.co.uk/news/people/bo-johnson-30403.html Boris Johnson: People - The Independent<!-- Bot generated title -->]</ref>");

            TestMatch(WikiRegexes.BareRefExternalLinkBotGenTitle, @"<ref>Smith, Fred [http://www.independent.co.uk/news/people/bo-johnson-30403.html Boris Johnson: People - The Independent<!-- Bot generated title -->]</ref>", false);
            TestMatch(WikiRegexes.BareRefExternalLinkBotGenTitle, @"<ref>[http://www.independent.co.uk/news/people/bo-johnson-30403.html Boris Johnson: People - The Independent]</ref>", false);
            Assert.IsTrue(WikiRegexes.BareRefExternalLinkBotGenTitle.IsMatch(@"attack<ref>http://www.news.com.au/heraldsun/story/0,21985,23169580-5006022,00.html</ref> was portrayed"));

            Assert.AreEqual(@"http://news.bbc.co.uk/hi/England/story4384.htm", WikiRegexes.BareRefExternalLinkBotGenTitle.Match(@"<ref>[http://news.bbc.co.uk/hi/England/story4384.htm]</ref>").Groups[1].Value);
            Assert.AreEqual(@"http://news.bbc.co.uk/hi/England/story4384.htm", WikiRegexes.BareRefExternalLinkBotGenTitle.Match(@"<ref>[http://news.bbc.co.uk/hi/England/story4384.htm]""</ref>").Groups[1].Value);
            Assert.AreEqual(@"http://news.bbc.co.uk/hi/England/story4384.htm", WikiRegexes.BareRefExternalLinkBotGenTitle.Match(@"<ref> [ http://news.bbc.co.uk/hi/England/story4384.htm]. </ref>").Groups[1].Value);
            Assert.AreEqual(@"Foo", WikiRegexes.BareRefExternalLinkBotGenTitle.Match(@"<ref> [ http://news.bbc.co.uk/hi/England/story4384.htm Foo<!--bot generated title-->]. </ref>").Groups[2].Value);
        }

        [Test]
        public void BoldItalicTests()
        {
            Assert.AreEqual(WikiRegexes.Bold.Match(@"'''foo'''").Groups[1].Value, @"foo");
            Assert.AreEqual(WikiRegexes.Bold.Match(@"'''foo bar'''").Groups[1].Value, @"foo bar");
            Assert.AreEqual(WikiRegexes.Bold.Match(@"'''foo's bar'''").Groups[1].Value, @"foo's bar");
            Assert.AreEqual(WikiRegexes.Bold.Match(@"'''''foo's bar'''''").Groups[1].Value, "", "no match on bold italics");

            Assert.AreEqual(WikiRegexes.Italics.Match(@"''foo''").Groups[1].Value, @"foo");
            Assert.AreEqual(WikiRegexes.Italics.Match(@"''foo bar''").Groups[1].Value, @"foo bar");
            Assert.AreEqual(WikiRegexes.Italics.Match(@"''foo's bar''").Groups[1].Value, @"foo's bar");
            Assert.AreEqual(WikiRegexes.Italics.Match(@"'''foo's bar'''").Groups[1].Value, "", "no match on bold");
            Assert.AreEqual(WikiRegexes.Italics.Match(@"'''''foo's bar'''''").Groups[1].Value, "", "no match on bold italics");
            Assert.AreEqual(WikiRegexes.Italics.Match(@"'''Tyrone Station''' is an by Amtrak's ''[[foo]]'', which").Groups[1].Value, "[[foo]]");

            Assert.AreEqual(WikiRegexes.BoldItalics.Match(@"''''' foo'''''").Groups[1].Value, @" foo");
            Assert.AreEqual(WikiRegexes.BoldItalics.Match(@"'''''foo bar'''''").Groups[1].Value, @"foo bar");
            Assert.AreEqual(WikiRegexes.BoldItalics.Match(@"'''''foo's bar'''''").Groups[1].Value, @"foo's bar");
            Assert.AreEqual(WikiRegexes.Italics.Match(@"''f''").Groups[1].Value, @"f");
            Assert.AreEqual(WikiRegexes.Italics.Match(@"''f'' nar ''abc''").Groups[1].Value, @"f");
            Assert.AreEqual(WikiRegexes.Bold.Match(@"'''f''' nar '''abc'''").Groups[1].Value, @"f");

            Assert.IsFalse(WikiRegexes.Italics.IsMatch(@"'''foo'''"));
            Assert.IsFalse(WikiRegexes.Italics.IsMatch(@"'''''foo'''''"));
            Assert.IsFalse(WikiRegexes.Bold.IsMatch(@"''foo'''"));
            Assert.IsFalse(WikiRegexes.Bold.IsMatch(@"''foo''"));
            Assert.IsFalse(WikiRegexes.BoldItalics.IsMatch(@"''foo''"));
            Assert.IsFalse(WikiRegexes.BoldItalics.IsMatch(@"'''foo'''"));
            Assert.IsFalse(WikiRegexes.BoldItalics.IsMatch(@"'''''foo''"));
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

#if DEBUG
            Variables.SetProjectLangCode("fr");
            WikiRegexes.MakeLangSpecificRegexes();
            Assert.IsTrue(WikiRegexes.ReferenceList.IsMatch(@"{{références}}"));
            Assert.IsTrue(WikiRegexes.ReferenceList.IsMatch(@"{{references}}"));
            Assert.IsTrue(WikiRegexes.ReferenceList.IsMatch(@"{{reflist}}"));

            Variables.SetProjectLangCode("en");
            WikiRegexes.MakeLangSpecificRegexes();
            Assert.IsTrue(WikiRegexes.ReferenceList.IsMatch(@"{{reflist}}"));
            Assert.IsTrue(WikiRegexes.ReferenceList.IsMatch(@"{{references-small}}"));
            Assert.IsTrue(WikiRegexes.ReferenceList.IsMatch(@"{{references-2column}}"));
#endif
        }

        [Test]
        public void SimpleWiki()
        {
            #if DEBUG
            Variables.SetProjectLangCode("simple");
            WikiRegexes.MakeLangSpecificRegexes();
            Assert.IsTrue(WikiRegexes.SeeAlso.IsMatch(@"==Related pages=="));
            Assert.IsTrue(WikiRegexes.SeeAlso.IsMatch(@"==related pages=="));
            Assert.IsTrue(WikiRegexes.SeeAlso.IsMatch(@"==See also=="));

            Assert.IsTrue(WikiRegexes.ExternalLinksHeader.IsMatch(@"==Other websites=="));
            Assert.IsTrue(WikiRegexes.ExternalLinksHeader.IsMatch(@"==External links=="));

            Variables.SetProjectLangCode("en");
            WikiRegexes.MakeLangSpecificRegexes();
            Assert.IsFalse(WikiRegexes.SeeAlso.IsMatch(@"==Related pages=="));
            Assert.IsTrue(WikiRegexes.SeeAlso.IsMatch(@"==See also=="));
            Assert.IsTrue(WikiRegexes.ExternalLinksHeader.IsMatch(@"==External links=="));
            #endif
        }

        [Test]
        public void SurnameClarificationTemplates()
        {
            Assert.IsTrue(WikiRegexes.SurnameClarificationTemplates.IsMatch(@"{{Malay name}}"));
            Assert.IsTrue(WikiRegexes.SurnameClarificationTemplates.IsMatch(@"{{Malay_name}}"));
            Assert.IsTrue(WikiRegexes.SurnameClarificationTemplates.IsMatch(@"{{Chinese name}}"));
            Assert.IsFalse(WikiRegexes.SurnameClarificationTemplates.IsMatch(@"{{Eastern name order}}"));
        }
        
        [Test]
        public void ReversedItalics()
        {
            Assert.IsTrue(WikiRegexes.ReversedItalics.IsMatch(@"</i>foo<i>"));
            Assert.IsTrue(WikiRegexes.ReversedItalics.IsMatch(@"</i>foo< i >"));
            Assert.IsTrue(WikiRegexes.ReversedItalics.IsMatch(@"< /i >foo<i>"));
            Assert.AreEqual("foo", WikiRegexes.ReversedItalics.Match(@"</i>foo<i>").Groups[1].Value);
            
            Assert.IsFalse(WikiRegexes.ReversedItalics.IsMatch(@"<i>foo</i>"));
            Assert.IsFalse(WikiRegexes.ReversedItalics.IsMatch(@"<i>foo<i>"));
            Assert.IsFalse(WikiRegexes.ReversedItalics.IsMatch(@"<i>foo</i> and <i>foo</i>"));
        }

        [Test]
        public void ShortPagesMonitor()
        {
            const string LC = @"{{Short pages monitor}}<!-- This long comment was added to the page to prevent it from being listed on Special:Shortpages. It and the accompanying monitoring template were generated via Template:Long comment. Please do not remove the monitor template without removing the comment as well.-->";

            Assert.IsTrue(WikiRegexes.ShortPagesMonitor.IsMatch(LC));
            Assert.IsTrue(WikiRegexes.ShortPagesMonitor.IsMatch(LC.Replace("{{S", "{{s")), "handles template name first letter case insensitive");
            Assert.IsTrue(WikiRegexes.ShortPagesMonitor.IsMatch(@"{{Short pages monitor}}<!-- any old comment-->"));
        }
    }
}
