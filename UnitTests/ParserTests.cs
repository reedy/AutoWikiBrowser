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

        [Test]
        public void PrecededByEqualSign()
        {
            Assert.That(parser.FixFootnotes("a=<ref>b</ref>"), Text.DoesNotContain("\n"));
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
        public void DontFixTrailedLinks()
        {
            bool dummy;

            Assert.AreEqual("[[a ]]b", parser.FixLinks("[[a ]]b", out dummy));
        }

        [Test]
        public void TestStickyLinks()
        {
            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_1#Link_de-piping_false_positive
            Assert.AreEqual("[[Sacramento, California|Sacramento]], California's [[capital city]]",
                parser.StickyLinks("[[Sacramento, California|Sacramento]], California's [[capital city]]"));
        }

        [Test]
        public void TestSimplifyLinks()
        {
            Assert.AreEqual("[[dog]]s", parser.SimplifyLinks("[[dog|dogs]]"));

            //http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_2#Inappropriate_link_compression
            Assert.AreEqual("[[foo|foo3]]", parser.SimplifyLinks("[[foo|foo3]]"));
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

        [Test]
        //TODO: unfinished
        public void TestLinkRepairs()
        {
            Assert.AreEqual("[http://example.com]", parser.FixSyntax("[http://example.com]"));
            Assert.AreEqual("[http://example.com]", parser.FixSyntax("[[http://example.com]"));
            Assert.AreEqual("[http://example.com]", parser.FixSyntax("[http://example.com]]"));
            Assert.AreEqual("[http://example.com]", parser.FixSyntax("[[http://example.com]]"));

            Assert.AreEqual("[[Image:foo.jpg|Some [http://some_crap.com]]]",
                parser.FixSyntax("[[Image:foo.jpg|Some [[http://some_crap.com]]]]"));

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs#NEsted_square_brackets_again.
            Assert.AreEqual("[[Image:foo.jpg|Some [http://some_crap.com]]]",
                parser.FixSyntax("[[Image:foo.jpg|Some [http://some_crap.com]]]"));

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
            string s = parser.BulletExternalLinks(@"==External links==
http://example.com/foo
[http://example.com foo]
{{aTemplate|url=
http://example.com }}");

            StringAssert.Contains("* http://example.com/foo", s);
            StringAssert.Contains("* [http://example.com foo]", s);

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_2#Incorrect_bulleting
            StringAssert.Contains("\r\nhttp://example.com }}", s);
        }
    }

    [TestFixture]
    public class ImageTests
    {
        [SetUp]
        public void SetUp()
        {
            Globals.UnitTestMode = true;
        }

        [Test, Category("Incomplete")]
        public void BasicImprovements()
        {
            Parsers parser = new Parsers();

            Assert.AreEqual("[[Image:foo.jpg|thumb|200px|Bar]]",
                parser.FixImages("[[ image : foo.jpg|thumb|200px|Bar]]"));

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_6#URL_underscore_regression
            Assert.AreEqual("[[Image:foo|thumb]] # [http://a_b c] [[link]]",
                parser.FixImages("[[Image:foo|thumb]] # [http://a_b c] [[link]]"));

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_2#Removing_underscore_in_URL_in_Ref_in_Description_in_Image....
            //Assert.AreEqual("[[Image:foo_bar|[http://some_link]]]",
            //    parser.FixImages("[[image:foo_bar|http://some_link]]"));
        }
    }

    [TestFixture]
    public class BoldTitleTests
    {
        Parsers p = new Parsers();

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
        Parsers p = new Parsers();

        [Test]
        public void BasicBehaviour()
        {
            Assert.AreEqual("{{main|Foo}}", p.FixMainArticle("Main article: [[Foo]]"));
            Assert.AreEqual("{{main|Foo}}", p.FixMainArticle("Main article: [[Foo]]."));
            Assert.AreEqual("Main article:\r\n [[Foo]]", p.FixMainArticle("Main article:\r\n [[Foo]]"));
        }

        [Test]
        // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_3#Fixing_Main_Article_to_.7B.7Bmain.7D.7D
        public void PipedLinks()
        {
            Assert.AreEqual("{{main|Foo|l1=Bar}}", p.FixMainArticle("Main article: [[Foo|Bar]]"));
        }

        [Test]
        public void SupportIndenting()
        {
            Assert.AreEqual("{{main|Foo}}", p.FixMainArticle(":Main article: [[Foo]]"));
            Assert.AreEqual("{{main|Foo}}", p.FixMainArticle(":Main article: [[Foo]]."));
            Assert.AreEqual("{{main|Foo}}", p.FixMainArticle(":''Main article: [[Foo]]''"));
            Assert.AreEqual("'':Main article: [[Foo]]''", p.FixMainArticle("'':Main article: [[Foo]]''"));
        }

        [Test]
        public void SupportBoldAndItalic()
        {
            Assert.AreEqual("{{main|Foo}}", p.FixMainArticle("Main article: '[[Foo]]'"));
            Assert.AreEqual("{{main|Foo}}", p.FixMainArticle("Main article: ''[[Foo]]''"));
            Assert.AreEqual("{{main|Foo}}", p.FixMainArticle("Main article: '''[[Foo]]'''"));
            Assert.AreEqual("{{main|Foo}}", p.FixMainArticle("Main article: '''''[[Foo]]'''''"));

            Assert.AreEqual("{{main|Foo}}", p.FixMainArticle("'Main article: [[Foo]]'"));
            Assert.AreEqual("{{main|Foo}}", p.FixMainArticle("''Main article: [[Foo]]''"));
            Assert.AreEqual("{{main|Foo}}", p.FixMainArticle("'''Main article: [[Foo]]'''"));
            Assert.AreEqual("{{main|Foo}}", p.FixMainArticle("'''''Main article: [[Foo]]'''''"));

            Assert.AreEqual("{{main|Foo}}", p.FixMainArticle("''Main article: '''[[Foo]]'''''"));
            Assert.AreEqual("{{main|Foo}}", p.FixMainArticle("'''Main article: ''[[Foo]]'''''"));
        }

        [Test]
        public void CaseInsensitivity()
        {
            Assert.AreEqual("{{main|foo}}", p.FixMainArticle("main Article: [[foo]]"));
        }

        [Test]
        // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_4#Problem_with_reverse_subst_of_.7B.7Bmain.7D.7D
        public void DontEatTooMuch()
        {
            Assert.AreEqual("Foo is a bar, see main article: [[Foo]]",
                p.FixMainArticle("Foo is a bar, see main article: [[Foo]]"));
            Assert.AreEqual("Main article: [[Foo]], bar", p.FixMainArticle("Main article: [[Foo]], bar"));
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
    }
}
