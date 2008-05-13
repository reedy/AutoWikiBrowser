/*
AWB unit tests
Copyright (C) 2008 Max Semenik

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

Portions Copyright © 2002-2007 Charlie Poole or 
Copyright © 2002-2004 James W. Newkirk, Michael C. Two, Alexei A. Vorontsov or 
Copyright © 2000-2002 Philip A. Craig

*/

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using WikiFunctions;
using WikiFunctions.Parse;
using NUnit.Framework.SyntaxHelpers;

namespace UnitTests
{
    [TestFixture]
    public class FootnotesTests
    {
        Parsers parser = new Parsers();

        public FootnotesTests()
        {
            Globals.UnitTestMode = true;
        }

        [Test]
        public void PrecededByEqualSign()
        {
            Assert.That(Parsers.FixFootnotes("a=<ref>b</ref>"), Text.DoesNotContain("\n"));
        }

        [Test]
        // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_6#Unexpected_modification
        public void TestTagBoundaries()
        {
            Assert.AreEqual("<ref name=\"foo\"><br></ref>", Parsers.SimplifyReferenceTags("<ref name=\"foo\"><br></ref>"));
        }

        [Test]
        public void TestSimplifyReferenceTags()
        {
            Assert.AreEqual("<ref name=\"foo\" />", Parsers.SimplifyReferenceTags("<ref name=\"foo\"></ref>"));
            Assert.AreEqual("<ref name=\"foo\" />", Parsers.SimplifyReferenceTags("<ref name=\"foo\" >< / ref >"));
            Assert.AreEqual("<ref name=\"foo\" />", Parsers.SimplifyReferenceTags("<ref name=\"foo\" ></ref>"));
            Assert.AreEqual("<ref name=\"foo\" />", Parsers.SimplifyReferenceTags("<ref name=\"foo\"></ref>"));
            Assert.AreEqual("<ref name=\"foo\" />", Parsers.SimplifyReferenceTags("<ref name=\"foo\"></ref>"));

            // don't use * quantifier for \s
            Assert.AreEqual("<refname=\"foo\"></ref>", Parsers.SimplifyReferenceTags("<refname=\"foo\"></ref>"));
        }
    }

    [TestFixture]
    public class LinkTests
    {
        [SetUp]
        public void SetUp()
        {
            Globals.UnitTestMode = true;
        }

        Parsers parser = new Parsers();

        [Test]
        public void TestStickyLinks()
        {
            Assert.AreEqual("[[Russian literature]]", Parsers.StickyLinks("[[Russian literature|Russian]] literature"));
            Assert.AreEqual("[[Russian literature]]", Parsers.StickyLinks("[[Russian literature|Russian]]  literature"));
            Assert.AreEqual("[[Russian literature]]", Parsers.StickyLinks("[[russian literature|Russian]] literature"));
            Assert.AreEqual("[[russian literature]]", Parsers.StickyLinks("[[Russian literature|russian]] literature"));
            Assert.AreEqual("[[Russian literature|Russian]]\nliterature", Parsers.StickyLinks("[[Russian literature|Russian]]\nliterature"));
            Assert.AreEqual("   [[Russian literature]]  ", Parsers.StickyLinks("   [[Russian literature|Russian]] literature  "));

            Assert.AreEqual("[[Russian literature|Russian]] Literature", Parsers.StickyLinks("[[Russian literature|Russian]] Literature"));

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_1#Link_de-piping_false_positive
            Assert.AreEqual("[[Sacramento, California|Sacramento]], California's [[capital city]]",
                Parsers.StickyLinks("[[Sacramento, California|Sacramento]], California's [[capital city]]"));
        }

        [Test]
        public void TestSimplifyLinks()
        {
            Assert.AreEqual("[[dog]]s", Parsers.SimplifyLinks("[[dog|dogs]]"));

            // case insensitivity of the first char
            Assert.AreEqual("[[dog]]s", Parsers.SimplifyLinks("[[Dog|dogs]]"));
            Assert.AreEqual("[[Dog]]s", Parsers.SimplifyLinks("[[dog|Dogs]]"));

            // ...and sensitivity of others
            Assert.AreEqual("[[dog|dOgs]]", Parsers.SimplifyLinks("[[dog|dOgs]]"));

            //http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_2#Inappropriate_link_compression
            Assert.AreEqual("[[foo|foo3]]", Parsers.SimplifyLinks("[[foo|foo3]]"));

            // don't touch suffices with caps to avoid funky results like
            // http://en.wikipedia.org/w/index.php?diff=195760456
            Assert.AreEqual("[[FOO|FOOBAR]]", Parsers.SimplifyLinks("[[FOO|FOOBAR]]"));
            Assert.AreEqual("[[foo|fooBAR]]", Parsers.SimplifyLinks("[[foo|fooBAR]]"));
        }

        [Test]
        public void FixDates()
        {
            Assert.AreEqual("the later 1990s", parser.FixDates("the later 1990's"));

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_1#Title_bolding
            Assert.AreEqual("the later A1990's", parser.FixDates("the later A1990's"));

            //http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_1#Breaking_a_template
            Assert.AreEqual("{{the later 1990's}}", parser.FixDates("{{the later 1990's}}"));
        }

        [Test, Category("Incomplete")]
        public void TestLinkRepairs()
        {
            Assert.AreEqual("[http://example.com]", parser.FixSyntax("[http://example.com]"));
            Assert.AreEqual("[http://example.com]", parser.FixSyntax("[[http://example.com]"));
            Assert.AreEqual("[http://example.com]", parser.FixSyntax("[http://example.com]]"));
            Assert.AreEqual("[http://example.com]", parser.FixSyntax("[[http://example.com]]"));

            Assert.AreEqual("[[Image:foo.jpg|Some [http://some_crap.com]]]",
                parser.FixSyntax("[[Image:foo.jpg|Some [[http://some_crap.com]]]]"));

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_3#NEsted_square_brackets_again.
            Assert.AreEqual("[[Image:foo.jpg|Some [http://some_crap.com]]]",
                parser.FixSyntax("[[Image:foo.jpg|Some [http://some_crap.com]]]"));

            Assert.AreEqual("Image:foo.jpg|{{{some_crap}}}]]", parser.FixSyntax("Image:foo.jpg|{{{some_crap}}}]]"));

            Assert.AreEqual("[[somelink]]", parser.FixSyntax("[somelink]]"));
            Assert.AreEqual("[[somelink]]", parser.FixSyntax("[[somelink]"));
            Assert.AreNotEqual("[[somelink]]", parser.FixSyntax("[somelink]"));

            Assert.AreEqual("'''foo''' bar", parser.FixSyntax("<b>foo</b> bar"));
            Assert.AreEqual("'''foo''' bar", parser.FixSyntax("< b >foo</b> bar"));
            Assert.AreEqual("'''foo''' bar", parser.FixSyntax("<b>foo< /b > bar"));
            Assert.AreEqual("<b>foo<b> bar", parser.FixSyntax("<b>foo<b> bar"));

            Assert.AreEqual("''foo'' bar", parser.FixSyntax("<i>foo</i> bar"));
            Assert.AreEqual("''foo'' bar", parser.FixSyntax("< i >foo</i> bar"));
            Assert.AreEqual("''foo'' bar", parser.FixSyntax("<i>foo< /i > bar"));
            Assert.AreEqual("<i>foo<i> bar", parser.FixSyntax("<i>foo<i> bar"));

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_4#Bug_encountered_when_perusing_Sonorous_Susurrus
            Assert.AreEqual("[[foo]]", parser.FixSyntax("[[|foo]]"));
            Parsers.CanonicalizeTitle("[[|foo]]"); // shouldn't throw exceptions
        }

        [Test]
        public void TestBulletExternalLinks()
        {
            string s = Parsers.BulletExternalLinks(@"==External links==
http://example.com/foo
[http://example.com foo]
{{aTemplate|url=
http://example.com }}");

            StringAssert.Contains("* http://example.com/foo", s);
            StringAssert.Contains("* [http://example.com foo]", s);

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_2#Incorrect_bulleting
            StringAssert.Contains("\r\nhttp://example.com }}", s);
        }

        [Test]
        public void TestFixLinkWhitespace()
        {
            Assert.AreEqual("b [[a]] c", Parsers.FixLinkWhitespace("b[[ a ]]c")); // regexes 1 & 2
            Assert.AreEqual("b   [[a]]  c", Parsers.FixLinkWhitespace("b   [[ a ]]  c")); // 4 & 5

            Assert.AreEqual("[[a]] b", Parsers.FixLinkWhitespace("[[a ]]b"));

            Assert.AreEqual("[[foo bar]]", Parsers.FixLinkWhitespace("[[foo  bar]]"));
            Assert.AreEqual("[[foo bar]]", Parsers.FixLinkWhitespace("[[foo     bar]]"));
            Assert.AreEqual("dat is [[foo bar]] show!", Parsers.FixLinkWhitespace("dat is [[ foo   bar ]] show!"));
            Assert.AreEqual("dat is [[foo bar]] show!", Parsers.FixLinkWhitespace("dat is[[ foo   bar ]]show!"));

            // shouldn't fix - not enough information
            //Assert.AreEqual("[[ a ]]", Parsers.FixLinkWhitespace("[[ a ]]"));
            //disabled for the time being to avoid unnecesary clutter
        }

        [Test, Category("Incomplete")]
        public void TestCanonicalizeTitle()
        {
            Assert.AreEqual("foo (bar)", Parsers.CanonicalizeTitle("foo_%28bar%29"));

            // it may or may not fix it, but shouldn't break anything
            StringAssert.Contains("{{bar_boz}}", Parsers.CanonicalizeTitle("foo_bar{{bar_boz}}"));
        }

        [Test]
        public void TestFixCategories()
        {
            Assert.AreEqual("[[Category:Foo bar]]", Parsers.FixCategories("[[ categOry : Foo_bar]]"));
            Assert.AreEqual("[[Category:Foo bar|boz]]", Parsers.FixCategories("[[ categOry : Foo_bar|boz]]"));

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Archive_18#.2Fdoc_pages_and_includeonly_sections
            Assert.AreEqual("[[Category:foo bar|boz_quux]]", Parsers.FixCategories("[[Category: foo_bar|boz_quux]]"));
            Assert.AreEqual("[[Category:foo bar|{{boz_quux}}]]", Parsers.FixCategories("[[Category: foo_bar|{{boz_quux}}]]"));
            StringAssert.Contains("{{{boz_quux}}}", Parsers.FixCategories("[[CategorY : foo_bar{{{boz_quux}}}]]"));
            Assert.AreEqual("[[Category:foo bar|{{{boz_quux}}}]]", Parsers.FixCategories("[[CategorY : foo_bar|{{{boz_quux}}}]]"));
        }
    }

    [TestFixture]
    public class FormattingTests
    {
        Parsers p = new Parsers();

        public FormattingTests()
        {
            Globals.UnitTestMode = true;
        }

        [Test]
        public void TestBrConverter()
        {
            Assert.AreEqual("*a\r\nb", p.FixSyntax("*a<br>\r\nb"));
            Assert.AreEqual("*a\r\nb", p.FixSyntax("\r\n*a<br>\r\nb"));
            Assert.AreEqual("foo\r\n*a\r\nb", p.FixSyntax("foo\r\n*a<br>\r\nb"));
        
            Assert.AreEqual("*a", p.FixSyntax("*a<br>\r\n")); // \r\n\ trimmed
            Assert.AreEqual("*:#;a\r\n*b", p.FixSyntax("*:#;a<br>\r\n*b"));
            Assert.AreEqual("###;;;:::***a\r\nb", p.FixSyntax("###;;;:::***a<br />\r\nb"));
            Assert.AreEqual("*&a\r\nb", p.FixSyntax("*&a<br/>\r\nb"));

            Assert.AreEqual("&*a<br>\r\nb", p.FixSyntax("&*a<br>\r\nb"));
            Assert.AreEqual("*a\r\n<br>\r\nb", p.FixSyntax("*a\r\n<br>\r\nb"));
        }

        [Test]
        public void TestFixHeadings()
        {
            // breaks if article title is empty
            Assert.AreEqual("==foo==", p.FixHeadings("=='''foo'''==", "test"));
            StringAssert.StartsWith("==foo==", p.FixHeadings("=='''foo'''==\r\n", "test"));
            Assert.AreEqual("quux\r\n==foo==\r\nbar", p.FixHeadings("quux\r\n=='''foo'''==\r\nbar", "test"));
            Assert.AreEqual("quux\r\n==foo==\r\n\r\nbar", p.FixHeadings("quux\r\n=='''foo'''==\r\n\r\nbar", "test"));
        }

        [Test, Category("Unarchived bugs")]
        public void TestParagraphFormatter()
        {
            Assert.AreEqual("", p.FixSyntax("<p>")); // trimmed by whitespace optimiser
            Assert.AreEqual("a\r\n\r\nb", p.FixSyntax("a</p>b"));
            Assert.AreEqual("a\r\n\r\nb", p.FixSyntax("<p>a</p>b"));
            Assert.AreEqual("a\r\n\r\n\r\nb", p.FixSyntax("a\r\n<p>b"));

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs#Clean_up_deformats_tables_removing_.3C_p_.3E_tags
            Assert.AreEqual("| a<p>b", p.FixSyntax("| a<p>b"));
            Assert.AreEqual("! a<p>b", p.FixSyntax("! a<p>b"));
            Assert.AreEqual("foo\r\n| a<p>b", p.FixSyntax("foo\r\n| a<p>b"));
            Assert.AreEqual("foo\r\n! a<p>b", p.FixSyntax("foo\r\n! a<p>b"));
            Assert.AreEqual("foo! a\r\n\r\nb", p.FixSyntax("foo! a<p>b"));
            Assert.AreEqual("{{foo|bar}} a\r\n\r\nb", p.FixSyntax("{{foo|bar}} a<p>b"));
            Assert.AreEqual("!<p>", p.FixSyntax("!<p>"));
            Assert.AreEqual("|<p>", p.FixSyntax("|<p>"));
        }

        [Test, Category("Incomplete")]
        //TODO: cover everything
        public void TestFixWhitespace()
        {
            Assert.AreEqual("", Parsers.RemoveWhiteSpace("     "));
            Assert.AreEqual("a\r\n\r\n b", Parsers.RemoveWhiteSpace("a\r\n\r\n\r\n b"));
            //Assert.AreEqual(" a", Parsers.RemoveWhiteSpace(" a")); // fails, but it doesn't seem harmful, at least for
                                                                     // WMF projects with their design guidelines
            //Assert.AreEqual(" a", Parsers.RemoveWhiteSpace("\r\n a \r\n")); // same as above
            Assert.AreEqual("a", Parsers.RemoveWhiteSpace("\r\na \r\n")); // the above errors have effect only on the first line
            Assert.AreEqual("", Parsers.RemoveWhiteSpace("\r\n"));
            Assert.AreEqual("", Parsers.RemoveWhiteSpace("\r\n\r\n"));
            Assert.AreEqual("a\r\nb", Parsers.RemoveWhiteSpace("a\r\nb"));
            Assert.AreEqual("a\r\n\r\nb", Parsers.RemoveWhiteSpace("a\r\n\r\nb"));
            Assert.AreEqual("a\r\n\r\nb", Parsers.RemoveWhiteSpace("a\r\n\r\n\r\nb"));

            Assert.AreEqual("== foo ==\r\n==bar", Parsers.RemoveWhiteSpace("== foo ==\r\n==bar"));
            Assert.AreEqual("== foo ==\r\n==bar", Parsers.RemoveWhiteSpace("== foo ==\r\n\r\n==bar"));
            Assert.AreEqual("== foo ==\r\n==bar", Parsers.RemoveWhiteSpace("== foo ==\r\n\r\n\r\n==bar"));

            Assert.AreEqual("{|\r\n| foo\r\n|\r\nbar\r\n|}", Parsers.RemoveWhiteSpace("{|\r\n| foo\r\n\r\n|\r\n\r\nbar\r\n|}"));

            // eh? should we fix such tables too?
            //Assert.AreEqual("{|\r\n! foo\r\n!\r\nbar\r\n|}", Parsers.RemoveWhiteSpace("{|\r\n! foo\r\n\r\n!\r\n\r\nbar\r\n|}"));
        }
    }

    [TestFixture]
    public class ImageTests
    {
        public ImageTests()
        {
            Globals.UnitTestMode = true;
        }

        [Test, Category("Incomplete")]
        public void BasicImprovements()
        {
            Parsers parser = new Parsers();

            Assert.AreEqual("[[Image:foo.jpg|thumb|200px|Bar]]",
                Parsers.FixImages("[[ image : foo.jpg|thumb|200px|Bar]]"));

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_6#URL_underscore_regression
            Assert.AreEqual("[[Image:foo|thumb]] # [http://a_b c] [[link]]",
            Parsers.FixImages("[[Image:foo|thumb]] # [http://a_b c] [[link]]"));

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_2#Removing_underscore_in_URL_in_Ref_in_Description_in_Image....
            //Assert.AreEqual("[[Image:foo_bar|[http://some_link]]]",
            //    parser.FixImages("[[image:foo_bar|http://some_link]]"));
        }
    }

    [TestFixture]
    public class BoldTitleTests
    {
        Parsers p = new Parsers();

        public BoldTitleTests()
        {
            Globals.UnitTestMode = true;
        }

        [Test]
        //http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_1#Title_bolding
        public void DontEmboldenImages()
        {
            bool dummy;
            Assert.That(p.BoldTitle("[[Image:Foo.jpg]]", "Foo", out dummy), Is.Not.Contains("'''Foo'''"));
        }
    }

    [TestFixture]
    public class FixMainArticleTests
    {
        [Test]
        public void BasicBehaviour()
        {
            Assert.AreEqual("{{main|Foo}}", Parsers.FixMainArticle("Main article: [[Foo]]"));
            Assert.AreEqual("{{main|Foo}}", Parsers.FixMainArticle("Main article: [[Foo]]."));
            Assert.AreEqual("Main article:\r\n [[Foo]]", Parsers.FixMainArticle("Main article:\r\n [[Foo]]"));
        }

        [Test]
        // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_3#Fixing_Main_Article_to_.7B.7Bmain.7D.7D
        public void PipedLinks()
        {
            Assert.AreEqual("{{main|Foo|l1=Bar}}", Parsers.FixMainArticle("Main article: [[Foo|Bar]]"));
        }

        [Test]
        public void SupportIndenting()
        {
            Assert.AreEqual("{{main|Foo}}", Parsers.FixMainArticle(":Main article: [[Foo]]"));
            Assert.AreEqual("{{main|Foo}}", Parsers.FixMainArticle(":Main article: [[Foo]]."));
            Assert.AreEqual("{{main|Foo}}", Parsers.FixMainArticle(":''Main article: [[Foo]]''"));
            Assert.AreEqual("'':Main article: [[Foo]]''", Parsers.FixMainArticle("'':Main article: [[Foo]]''"));
        }

        [Test]
        public void SupportBoldAndItalic()
        {
            Assert.AreEqual("{{main|Foo}}", Parsers.FixMainArticle("Main article: '[[Foo]]'"));
            Assert.AreEqual("{{main|Foo}}", Parsers.FixMainArticle("Main article: ''[[Foo]]''"));
            Assert.AreEqual("{{main|Foo}}", Parsers.FixMainArticle("Main article: '''[[Foo]]'''"));
            Assert.AreEqual("{{main|Foo}}", Parsers.FixMainArticle("Main article: '''''[[Foo]]'''''"));

            Assert.AreEqual("{{main|Foo}}", Parsers.FixMainArticle("'Main article: [[Foo]]'"));
            Assert.AreEqual("{{main|Foo}}", Parsers.FixMainArticle("''Main article: [[Foo]]''"));
            Assert.AreEqual("{{main|Foo}}", Parsers.FixMainArticle("'''Main article: [[Foo]]'''"));
            Assert.AreEqual("{{main|Foo}}", Parsers.FixMainArticle("'''''Main article: [[Foo]]'''''"));

            Assert.AreEqual("{{main|Foo}}", Parsers.FixMainArticle("''Main article: '''[[Foo]]'''''"));
            Assert.AreEqual("{{main|Foo}}", Parsers.FixMainArticle("'''Main article: ''[[Foo]]'''''"));
        }

        [Test]
        public void CaseInsensitivity()
        {
            Assert.AreEqual("{{main|foo}}", Parsers.FixMainArticle("main Article: [[foo]]"));
        }

        [Test]
        // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_4#Problem_with_reverse_subst_of_.7B.7Bmain.7D.7D
        public void DontEatTooMuch()
        {
            Assert.AreEqual("Foo is a bar, see main article: [[Foo]]",
                Parsers.FixMainArticle("Foo is a bar, see main article: [[Foo]]"));
            Assert.AreEqual("Main article: [[Foo]], bar", Parsers.FixMainArticle("Main article: [[Foo]], bar"));
        }

        [Test]
        // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_6#Main_and_See_also_templates
        public void SingleLinkOnly()
        {
            Assert.AreEqual(":Main article: [[Foo]] and [[Bar]]", Parsers.FixMainArticle(":Main article: [[Foo]] and [[Bar]]"));
            Assert.AreEqual(":Main article: [[Foo|f00]] and [[Bar]]", Parsers.FixMainArticle(":Main article: [[Foo|f00]] and [[Bar]]"));
        }
    }

    [TestFixture]
    public class UnicodifyTests
    {
        Parsers parser = new Parsers();

        [Test]
        public void PreserveTM()
        {
            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_1#AWB_corrupts_the_trademark_.28TM.29_special_character_.
            Assert.AreEqual("test™", parser.Unicodify("test™"));
        }

        [Test]
        public void DontChangeCertainEntities()
        {
            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_3#.26emsp.3B
            Assert.AreEqual("&emsp;", parser.Unicodify("&emsp;"));
        }

        [Test]
        public void IgnoreMath()
        {
            Assert.AreEqual("<math>&laquo;</math>", parser.Unicodify("<math>&laquo;</math>"));
        }
    }

    [TestFixture]
    public class UtilityFunctionTests
    {
        Parsers p = new Parsers();

        public UtilityFunctionTests()
        {
            Globals.UnitTestMode = true;
            Variables.SetToEnglish();
        }

        [Test]
        public void TestIsCorrectEditSummary()
        {
            // no wikilinks
            Assert.IsTrue(Parsers.IsCorrectEditSummary(""));
            Assert.IsTrue(Parsers.IsCorrectEditSummary("test"));
            Assert.IsTrue(Parsers.IsCorrectEditSummary("["));
            Assert.IsTrue(Parsers.IsCorrectEditSummary("]"));
            Assert.IsTrue(Parsers.IsCorrectEditSummary("[test]"));
            Assert.IsTrue(Parsers.IsCorrectEditSummary("[test]]"));
            Assert.IsTrue(Parsers.IsCorrectEditSummary("[[]]"));

            // correctly (sort of..) terminated wikilinks
            Assert.IsTrue(Parsers.IsCorrectEditSummary("[[test]]"));
            Assert.IsTrue(Parsers.IsCorrectEditSummary("[[test]] [[foo]]"));
            Assert.IsTrue(Parsers.IsCorrectEditSummary("[[foo[[]]]"));

            //broken wikilinks, should be found to be invalid
            Assert.IsFalse(Parsers.IsCorrectEditSummary("[["));
            Assert.IsFalse(Parsers.IsCorrectEditSummary("[[["));
            Assert.IsFalse(Parsers.IsCorrectEditSummary("[[test]"));
            Assert.IsFalse(Parsers.IsCorrectEditSummary("[[test]] [["));
        }

        [Test]
        public void TestChangeToDefaultSort()
        {
            bool noChange;

            // don't change sorting for single categories
            Assert.AreEqual("[[Category:Test1|Foooo]]",
                p.ChangeToDefaultSort("[[Category:Test1|Foooo]]", "Foo", out noChange));
            Assert.IsTrue(noChange);

            // should work
            Assert.AreEqual("[[Category:Test1]][[Category:Test2]]\r\n{{DEFAULTSORT:Foooo}}",
                p.ChangeToDefaultSort("[[Category:Test1|Foooo]][[Category:Test2|Foooo]]", "Bar", out noChange));
            Assert.IsFalse(noChange);

            // ...but don't add DEFAULTSORT if the key equals page title
            Assert.AreEqual("[[Category:Test1]][[Category:Test2]]",
                p.ChangeToDefaultSort("[[Category:Test1|Foooo]][[Category:Test2|Foooo]]", "Foooo", out noChange));
            Assert.IsFalse(noChange, "Should detect a change even if it hasn't added a DEFAULTSORT");

            // don't change if key is 3 chars or less
            Assert.AreEqual("[[Category:Test1|Foo]][[Category:Test2|Foo]]",
                p.ChangeToDefaultSort("[[Category:Test1|Foo]][[Category:Test2|Foo]]", "Bar", out noChange));
            Assert.IsTrue(noChange);

            // Remove explicit keys equal to page title
            Assert.AreEqual("[[Category:Test1]][[Category:Test2]]",
                p.ChangeToDefaultSort("[[Category:Test1|Foooo]][[Category:Test2]]", "Foooo", out noChange));
            Assert.IsFalse(noChange);
            // swap
            Assert.AreEqual("[[Category:Test1]][[Category:Test2]]",
                p.ChangeToDefaultSort("[[Category:Test1]][[Category:Test2|Foooo]]", "Foooo", out noChange));
            Assert.IsFalse(noChange);

            // Borderline condition
            Assert.AreEqual("[[Category:Test1|Fooooo]][[Category:Test2]]",
                p.ChangeToDefaultSort("[[Category:Test1|Fooooo]][[Category:Test2]]", "Foooo", out noChange));
            Assert.IsTrue(noChange);

            // Don't change anything if there's ambiguity
            Assert.AreEqual("[[Category:Test1|Foooo]][[Category:Test2|Baaar]]",
                p.ChangeToDefaultSort("[[Category:Test1|Foooo]][[Category:Test2|Baaar]]", "Teeest", out noChange));
            Assert.IsTrue(noChange);
            // same thing
            Assert.AreEqual("[[Category:Test1|Foooo]][[Category:Test2|Baaar]]",
                p.ChangeToDefaultSort("[[Category:Test1|Foooo]][[Category:Test2|Baaar]]", "Foooo", out noChange));
            Assert.IsTrue(noChange);

            // remove diacritics when generating a key
            Assert.AreEqual("[[Category:Test1]][[Category:Test2]]\r\n{{DEFAULTSORT:Foooo}}",
                p.ChangeToDefaultSort("[[Category:Test1|Foooô]][[Category:Test2|Foooô]]", "Bar", out noChange));
            Assert.IsFalse(noChange);

            // should also fix diacritics in existing defaultsort's and remove leading spaces
            // also support mimicking templates
            Assert.AreEqual("{{DEFAULTSORT:Test}}",
                p.ChangeToDefaultSort("{{defaultsort| Tést}}", "Foo", out noChange));
            Assert.IsFalse(noChange);

            // shouldn't change whitespace-only sortkeys
            Assert.AreEqual("{{DEFAULTSORT: \t}}", p.ChangeToDefaultSort("{{DEFAULTSORT: \t}}", "Foo", out noChange));
            Assert.IsTrue(noChange);

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_6#DEFAULTSORT_with_spaces
            // DEFAULTSORT doesn't treat leading spaces the same way as categories do
            Assert.AreEqual("[[Category:Test1| Foooo]][[Category:Test2| Foooo]]",
                p.ChangeToDefaultSort("[[Category:Test1| Foooo]][[Category:Test2| Foooo]]", "Bar", out noChange));
            Assert.IsTrue(noChange);
        }
    }

    [TestFixture]
    public class RecategorizerTests
    {
        Parsers p = new Parsers();

        public RecategorizerTests()
        {
            Globals.UnitTestMode = true;
        }

        [Test]
        public void Replacement()
        {
            bool noChange;

            Assert.AreEqual("[[Category:Bar]]", p.ReCategoriser("Foo", "Bar", "[[Category:Foo]]", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("[[Category:Bar]]", p.ReCategoriser("Foo", "Bar", "[[ catEgory: Foo]]", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("[[Category:Bar]]", p.ReCategoriser("Foo", "Bar", "[[Category:foo]]", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("[[Category:Bar|boz]]", p.ReCategoriser("Foo", "Bar", "[[Category:Foo|boz]]", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("[[Category:Bar| boz]]", p.ReCategoriser("foo? Bar!", "Bar", "[[ category:Foo? Bar! | boz]]", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual(@"[[Category:Boz]]
[[Category:Bar]]
[[Category:Quux]]", p.ReCategoriser("Foo", "Bar", @"[[Category:Boz]]
[[Category:foo]]
[[Category:Quux]]", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("test[[Category:Bar]]test", p.ReCategoriser("Foo", "Bar", "test[[Category:Foo]]test", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("[[Category:Fooo]]", p.ReCategoriser("Foo", "Bar", "[[Category:Fooo]]", out noChange));
            Assert.IsTrue(noChange);
        }

        [Test]
        public void Removal()
        {
            bool noChange;

            Assert.AreEqual("", p.RemoveCategory("Foo", "[[Category:Foo]]", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("", p.RemoveCategory("Foo", "[[ category: foo | bar]]", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("", p.RemoveCategory("Foo", "[[Category:Foo|]]", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("  ", p.RemoveCategory("Foo", " [[Category:Foo]] ", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("", p.RemoveCategory("Foo", "[[Category:Foo]]\r\n", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("\r\n", p.RemoveCategory("Foo", "[[Category:Foo]]\r\n\r\n", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("", p.RemoveCategory("Foo? Bar!", "[[Category:Foo? Bar!|boz]]", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("[[Category:Fooo]]", p.RemoveCategory("Foo", "[[Category:Fooo]]", out noChange));
            Assert.IsTrue(noChange);
        }
    }
}
