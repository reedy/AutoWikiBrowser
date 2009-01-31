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

using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using WikiFunctions;
using WikiFunctions.Parse;
using NUnit.Framework.SyntaxHelpers;

namespace UnitTests
{
    [TestFixture]
    public class FootnotesTests
    {
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

        [Test]
        // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_7#References-2column_not_replaced_with_2_argument_to_reflist
        public void TestFixReferenceListTags()
        {
            Assert.AreEqual("<references/>", Parsers.FixReferenceListTags("<references/>"));
            Assert.AreEqual("<div><references/></div>", Parsers.FixReferenceListTags("<div><references/></div>"));

            Assert.AreEqual("{{reflist}}", Parsers.FixReferenceListTags("<div class=\"references-small\"><references/>\r\n</div>"));
            Assert.AreEqual("{{reflist|2}}", Parsers.FixReferenceListTags("<div class=\"references-2column\"><references/></div>"));
            Assert.AreEqual("{{reflist|2}}",
                Parsers.FixReferenceListTags(@"<div class=""references-2column""><div class=""references-small"">
<references/></div></div>"));
            Assert.AreEqual("{{reflist|2}}",
                Parsers.FixReferenceListTags(@"<div class=""references-small""><div class=""references-2column""> <references/>
</div></div>"));

            // evil don't do's
            Assert.That(Parsers.FixReferenceListTags(@"<div class=""references-small""><div class=""references-2column"">
<references/></div>* some other ref</div>"), Is.Not.Contains("{{reflist"));
            Assert.That(Parsers.FixReferenceListTags(@"<div class=""references-small""><div class=""references-2column"">
<references/></div>"), Is.Not.Contains("{{reflist"));
        }

        [Test]
        public void TestFixReferenceTags()
        {
            // whitespace cleaning
            Assert.AreEqual(@"now <ref>[http://www.site.com a site]</ref> was", Parsers.FixReferenceTags(@"now < ref>[http://www.site.com a site]</ref> was"));
            Assert.AreEqual(@"now <ref>[http://www.site.com a site]</ref> was", Parsers.FixReferenceTags(@"now < ref   >[http://www.site.com a site]</ref> was"));
            Assert.AreEqual(@"now <ref>[http://www.site.com a site]</ref> was", Parsers.FixReferenceTags(@"now <ref >[http://www.site.com a site]</ref> was"));
            Assert.AreEqual(@"now <ref>[http://www.site.com a site]</ref> was", Parsers.FixReferenceTags(@"now < ref>[http://www.site.com a site]< /ref> was"));
            Assert.AreEqual(@"now <ref>[http://www.site.com a site]</ref> was", Parsers.FixReferenceTags(@"now <ref>[http://www.site.com a site]</ ref> was"));
            Assert.AreEqual(@"now <ref>[http://www.site.com a site]</ref> was", Parsers.FixReferenceTags(@"now <ref>[http://www.site.com a site]</ref > was"));

            // <ref name=foo bar> --> <ref name="foo bar">
            Assert.AreEqual(@"now <ref name=""foo bar""> and", Parsers.FixReferenceTags(@"now <ref name=foo bar> and"));
            Assert.AreEqual(@"now <ref name=""foo bar"" /> and", Parsers.FixReferenceTags(@"now <ref name=foo bar /> and"));
            Assert.AreEqual(@"now <ref name = ""foo bar"" > and", Parsers.FixReferenceTags(@"now <ref name = foo bar > and"));

            // <ref name=foo bar"> --> <ref name="foo bar">
            Assert.AreEqual(@"now <ref name=""foo bar""> and", Parsers.FixReferenceTags(@"now <ref name=foo bar""> and"));
            Assert.AreEqual(@"now <ref name=""foo bar"" /> and", Parsers.FixReferenceTags(@"now <ref name=foo bar"" /> and"));
            Assert.AreEqual(@"now <ref name = ""foo bar"" > and", Parsers.FixReferenceTags(@"now <ref name = foo bar"" > and"));

            // <ref name="foo bar> --> <ref name="foo bar">
            Assert.AreEqual(@"now <ref name=""foo bar""> and", Parsers.FixReferenceTags(@"now <ref name=""foo bar> and"));
            Assert.AreEqual(@"now <ref name=""foo bar"" /> and", Parsers.FixReferenceTags(@"now <ref name=""foo bar /> and"));
            Assert.AreEqual(@"now <ref name = ""foo bar"" > and", Parsers.FixReferenceTags(@"now <ref name = ""foo bar > and"));

            // <ref name = ''Fred'> --> <ref name="Fred"> (two apostrophes)
            Assert.AreEqual(@"now <ref name=""foo bar""> and", Parsers.FixReferenceTags(@"now <ref name=''foo bar'> and"));
            Assert.AreEqual(@"now <ref name=""foo bar"" /> and", Parsers.FixReferenceTags(@"now <ref name='foo bar'' /> and"));
            Assert.AreEqual(@"now <ref name = ""foo bar"" > and", Parsers.FixReferenceTags(@"now <ref name = ''foo bar'' > and"));

            // <ref name "foo bar"> --> <ref name="foo bar">
            Assert.AreEqual(@"now <ref name =""foo bar""> and", Parsers.FixReferenceTags(@"now <ref name ""foo bar""> and"));
            Assert.AreEqual(@"now <ref name =""foo bar"" /> and", Parsers.FixReferenceTags(@"now <ref name ""foo bar"" /> and"));

            Assert.AreEqual(@"now <ref name =""foo bar""> and", Parsers.FixReferenceTags(@"now <ref name -""foo bar""> and"));
            Assert.AreEqual(@"now <ref name =""foo bar"" /> and", Parsers.FixReferenceTags(@"now <ref name -""foo bar"" /> and"));
            Assert.AreEqual(@"now <ref name =  ""foo bar"" > and", Parsers.FixReferenceTags(@"now <ref name -  ""foo bar"" > and"));
            Assert.AreEqual(@"now <ref name =""foo bar""> and", Parsers.FixReferenceTags(@"now <ref name +""foo bar""> and"));
            Assert.AreEqual(@"now <ref name =""foo bar"" /> and", Parsers.FixReferenceTags(@"now <ref name +""foo bar"" /> and"));
            Assert.AreEqual(@"now <ref name =  ""foo bar"" > and", Parsers.FixReferenceTags(@"now <ref name +  ""foo bar"" > and"));

            // <ref "foo bar"> --> <ref name="foo bar">
            Assert.AreEqual(@"now <ref name=""foo bar""> and", Parsers.FixReferenceTags(@"now <ref ""foo bar""> and"));
            Assert.AreEqual(@"now <ref name=""foo bar"" /> and", Parsers.FixReferenceTags(@"now <ref ""foo bar"" /> and"));

            // <ref name="Fred" /ref> --> <ref name="Fred"/>
            Assert.AreEqual(@"now <ref name=""Fred""/> was", Parsers.FixReferenceTags(@"now <ref name=""Fred"" /ref> was"));
            Assert.AreEqual(@"now <ref name=""Fred A""/> was", Parsers.FixReferenceTags(@"now <ref name=""Fred A"" /ref> was"));

            // <ref name="Fred".> --> <ref name="Fred"/>
            Assert.AreEqual(@"now <ref name=""Fred"".> was", Parsers.FixReferenceTags(@"now <ref name=""Fred"".> was"));
            Assert.AreEqual(@"now <ref name=""Fred""?> was", Parsers.FixReferenceTags(@"now <ref name=""Fred""?> was"));

            // <ref>...<ref/> --> <ref>...</ref>
            Assert.AreEqual(@"now <ref>[http://www.site.com a site]</ref> was", Parsers.FixReferenceTags(@"now <ref>[http://www.site.com a site]<ref/> was"));
            Assert.AreEqual(@"now <ref> [http://www.site.com a site]</ref> was", Parsers.FixReferenceTags(@"now <ref> [http://www.site.com a site]<ref/> was"));

            // <ref>...</red> --> <ref>...</ref>
            Assert.AreEqual(@"now <ref>[http://www.site.com a site]</ref> was", Parsers.FixReferenceTags(@"now <ref>[http://www.site.com a site]</red> was"));
            Assert.AreEqual(@"now <ref> [http://www.site.com a site]</ref> was", Parsers.FixReferenceTags(@"now <ref> [http://www.site.com a site]</red> was"));

            // no matches
            Assert.AreEqual(@"now <ref name=""foo bar""> and", Parsers.FixReferenceTags(@"now <ref name=""foo bar""> and"));
            Assert.AreEqual(@"now <ref name=""foo bar"" /> and", Parsers.FixReferenceTags(@"now <ref name=""foo bar"" /> and"));
            Assert.AreEqual(@"now <ref name = ""foo bar"" > and", Parsers.FixReferenceTags(@"now <ref name = ""foo bar"" > and"));
            Assert.AreEqual(@"now <ref name = 'foo bar' > and", Parsers.FixReferenceTags(@"now <ref name = 'foo bar' > and"));
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

        [Test, Category("Unarchived bugs")]
        public void TestSimplifyLinks()
        {
            Assert.AreEqual("[[dog]]s", Parsers.SimplifyLinks("[[dog|dogs]]"));

            // case insensitivity of the first char
            Assert.AreEqual("[[dog]]s", Parsers.SimplifyLinks("[[Dog|dogs]]"));
            Assert.AreEqual("[[Dog]]s", Parsers.SimplifyLinks("[[dog|Dogs]]"));

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_8#Wrong_link_simplification_capitalisation
            Assert.AreEqual("[[dog]]", Parsers.SimplifyLinks("[[Dog|dog]]"));
            Assert.AreEqual("[[Dog]]", Parsers.SimplifyLinks("[[dog|Dog]]"));
            Assert.AreEqual("[[Dog]]", Parsers.SimplifyLinks("[[Dog|Dog]]"));

            Assert.AreEqual("[[dog]]s", Parsers.SimplifyLinks("[[Dog|dogs]]"));
            Assert.AreEqual("[[Dog]]s", Parsers.SimplifyLinks("[[dog|Dogs]]"));
            Assert.AreEqual("[[Dog]]s", Parsers.SimplifyLinks("[[Dog|Dogs]]"));

            // ...and sensitivity of others
            Assert.AreEqual("[[dog|dOgs]]", Parsers.SimplifyLinks("[[dog|dOgs]]"));

            //http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_2#Inappropriate_link_compression
            Assert.AreEqual("[[foo|foo3]]", Parsers.SimplifyLinks("[[foo|foo3]]"));

            // don't touch suffixes with caps to avoid funky results like
            // http://en.wikipedia.org/w/index.php?diff=195760456
            Assert.AreEqual("[[FOO|FOOBAR]]", Parsers.SimplifyLinks("[[FOO|FOOBAR]]"));
            Assert.AreEqual("[[foo|fooBAR]]", Parsers.SimplifyLinks("[[foo|fooBAR]]"));

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_8#Only_one_spurious_space_removed_from_link
            Assert.AreEqual("[[Elizabeth Gunn]]", Parsers.SimplifyLinks("[[Elizabeth Gunn | Elizabeth Gunn]]"));
            Assert.AreEqual("[[Big Bend, Texas|Big Bend]]", Parsers.SimplifyLinks("[[Big Bend, Texas | Big Bend]]"));

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_8#SVN:_general_fixes_removes_whitespace_around_pipes_within_citation_templates
            Assert.AreEqual("{{foo|[[bar]] | boz}}]]", Parsers.SimplifyLinks("{{foo|[[bar]] | boz}}]]"));
            Assert.AreEqual("{{foo|[[bar]]\r\n| boz}}]]", Parsers.SimplifyLinks("{{foo|[[bar]]\r\n| boz}}]]"));

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_9#General_fixes_remove_spaces_from_category_sortkeys
            Assert.AreEqual("[[foo|bar]]", Parsers.SimplifyLinks("[[foo| bar]]"));
            Assert.AreEqual("[[foo|bar]]", Parsers.SimplifyLinks("[[foo| bar ]]"));
            Assert.AreEqual("[[Category:foo| bar]]", Parsers.SimplifyLinks("[[Category:foo| bar]]"));
            Assert.AreEqual("[[Category:foo| bar]]", Parsers.SimplifyLinks("[[Category:foo| bar ]]"));
        }

        [Test]
        public void FixDates()
        {
            Assert.AreEqual("the later 1990s", Parsers.FixDates("the later 1990's"));

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_1#Title_bolding
            Assert.AreEqual("the later A1990's", Parsers.FixDates("the later A1990's"));

            //http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_1#Breaking_a_template
            Assert.AreEqual("{{the later 1990's}}", Parsers.FixDates("{{the later 1990's}}"));

            // replace <br> and <p> HTML tags tests
            Assert.AreEqual("\r\n\r\nsome text", Parsers.FixDates("<p>some text"));
            Assert.AreEqual("\r\nsome text", Parsers.FixDates("<br><br>some text"));
            Assert.AreEqual("some text\r\n\r\n", Parsers.FixDates("some text<p>"));
            Assert.AreEqual("some text\r\n", Parsers.FixDates("some text<br><br>"));

            // don't match when in table or blockquote
            Assert.AreEqual("|<p>some text", Parsers.FixDates("|<p>some text"));
            Assert.AreEqual("|<br><br>some text", Parsers.FixDates("|<br><br>some text"));
            Assert.AreEqual("!<p>some text", Parsers.FixDates("!<p>some text"));
            Assert.AreEqual("!<br><br>some text", Parsers.FixDates("!<br><br>some text"));

            Assert.AreEqual("<blockquote><p>some text</blockquote>", Parsers.FixDates("<blockquote><p>some text</blockquote>"));
            Assert.AreEqual("<blockquote>|<br><br>some text</blockquote>", Parsers.FixDates("<blockquote>|<br><br>some text</blockquote>"));
        }

        [Test]
        public void FixLivingThingsRelatedDates()
        {
            Assert.AreEqual("test text", Parsers.FixLivingThingsRelatedDates("test text"));
            Assert.AreEqual("'''John Doe''' (born [[21 February]] [[2008]])", Parsers.FixLivingThingsRelatedDates("'''John Doe''' (b. [[21 February]] [[2008]])"));
            Assert.AreEqual("'''John Doe''' (died [[21 February]] [[2008]])", Parsers.FixLivingThingsRelatedDates("'''John Doe''' (d. [[21 February]] [[2008]])"));
            Assert.AreEqual("'''Willa Klug Baum''' ([[October 4]], [[1926]] – May 18, 2006)", Parsers.FixLivingThingsRelatedDates("'''Willa Klug Baum''' (born [[October 4]], [[1926]], died May 18, 2006)"));
        }

        [Test, Category("Incomplete")]
        public void TestFixSyntax()
        {
            bool noChange;

            Assert.AreEqual("[http://example.com] site", Parsers.FixSyntax("[http://example.com] site"));
            Assert.AreEqual("[http://example.com] site", Parsers.FixSyntax("[[http://example.com] site"));
            Assert.AreEqual("[http://example.com] site", Parsers.FixSyntax("[http://example.com]] site"));
            Assert.AreEqual("[http://example.com] site", Parsers.FixSyntax("[[http://example.com]] site"));

            Assert.AreEqual("[http://test.com]", Parsers.FixSyntax("[http://test.com]"));
            Assert.AreEqual("[http://test.com]", Parsers.FixSyntax("[http://http://test.com]"));
            Assert.AreEqual("[http://test.com]", Parsers.FixSyntax("[http://http://http://test.com]"));

            Assert.AreEqual("[http://www.site.com ''my cool site'']",
                            Parsers.FixSyntax("[http://www.site.com|''my cool site'']"));

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_3#NEsted_square_brackets_again.
            Assert.AreEqual("[[Image:foo.jpg|Some [http://some_crap.com]]]",
                            Parsers.FixSyntax("[[Image:foo.jpg|Some [http://some_crap.com]]]"));

            Assert.AreEqual("Image:foo.jpg|{{{some_crap}}}]]", Parsers.FixSyntax("Image:foo.jpg|{{{some_crap}}}]]"));

            Assert.AreEqual("[[somelink]]", Parsers.FixSyntax("[somelink]]"));
            Assert.AreEqual("[[somelink]]", Parsers.FixSyntax("[[somelink]"));
            Assert.AreNotEqual("[[somelink]]", Parsers.FixSyntax("[somelink]"));

            Assert.AreEqual("'''foo''' bar", Parsers.FixSyntax("<b>foo</b> bar"));
            Assert.AreEqual("'''foo''' bar", Parsers.FixSyntax("< b >foo</b> bar"));
            Assert.AreEqual("'''foo''' bar", Parsers.FixSyntax("<b>foo< /b > bar"));
            Assert.AreEqual("<b>foo<b> bar", Parsers.FixSyntax("<b>foo<b> bar"));

            Assert.AreEqual("''foo'' bar", Parsers.FixSyntax("<i>foo</i> bar"));
            Assert.AreEqual("''foo'' bar", Parsers.FixSyntax("< i >foo</i> bar"));
            Assert.AreEqual("''foo'' bar", Parsers.FixSyntax("<i>foo< /i > bar"));
            Assert.AreEqual("<i>foo<i> bar", Parsers.FixSyntax("<i>foo<i> bar"));

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_7#Erroneously_removing_pipe
            Assert.AreEqual("[[|foo]]", Parsers.FixSyntax("[[|foo]]"));

            //Double Spaces
            Assert.AreEqual("[[Foo]]", Parsers.FixSyntax("[[Foo]]"));
            Assert.AreEqual("[[Foo Bar]]", Parsers.FixSyntax("[[Foo Bar]]"));
            Assert.AreEqual("[[Foo Bar]]", Parsers.FixSyntax("[[Foo  Bar]]"));
            Assert.AreEqual("[[Foo Bar|Bar]]", Parsers.FixSyntax("[[Foo  Bar|Bar]]"));

            //TODO: move it to parts testing specific functions, when they're covered
            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_4#Bug_encountered_when_perusing_Sonorous_Susurrus
            Parsers.CanonicalizeTitle("[[|foo]]"); // shouldn't throw exceptions
            Assert.AreEqual("[[|foo]]", Parsers.FixLinks("[[|foo]]", out noChange));

            // string before and after
            Assert.AreEqual("<ref>http://www.site.com</ref>", Parsers.FixSyntax(@"<ref>http//www.site.com</ref>"));
            Assert.AreEqual("at http://www.site.com", Parsers.FixSyntax(@"at http//www.site.com"));
            Assert.AreEqual("<ref>[http://www.site.com a website]</ref>",
                            Parsers.FixSyntax(@"<ref>[http:/www.site.com a website]</ref>"));
            Assert.AreEqual("*[http://www.site.com a website]", Parsers.FixSyntax(@"*[http//www.site.com a website]"));
            Assert.AreEqual("|url=http://www.site.com", Parsers.FixSyntax(@"|url=http//www.site.com"));
            Assert.AreEqual("|url = http://www.site.com", Parsers.FixSyntax(@"|url = http:/www.site.com"));
            Assert.AreEqual("[http://www.site.com]", Parsers.FixSyntax(@"[http/www.site.com]"));

            // these strings should not change
            Assert.AreEqual("http://members.bib-arch.org/nph-proxy.pl/000000A/http/www.basarchive.org/bswbSearch",
                            Parsers.FixSyntax(
                                "http://members.bib-arch.org/nph-proxy.pl/000000A/http/www.basarchive.org/bswbSearch"));
            Assert.AreEqual("http://sunsite.utk.edu/math_archives/.http/contests/",
                            Parsers.FixSyntax("http://sunsite.utk.edu/math_archives/.http/contests/"));
            Assert.AreEqual("HTTP/0.9", Parsers.FixSyntax("HTTP/0.9"));
            Assert.AreEqual("HTTP/1.0", Parsers.FixSyntax("HTTP/1.0"));
            Assert.AreEqual("HTTP/1.1", Parsers.FixSyntax("HTTP/1.1"));
            Assert.AreEqual("HTTP/1.2", Parsers.FixSyntax("HTTP/1.2"));
            Assert.AreEqual("the HTTP/1.2 protocol", Parsers.FixSyntax("the HTTP/1.2 protocol"));
            Assert.AreEqual(@"<ref>[http://cdiac.esd.ornl.gov/ftp/cdiac74/a.pdf chapter 5]</ref>", Parsers.FixSyntax(@"<ref>[http://cdiac.esd.ornl.gov/ftp/cdiac74/a.pdf chapter 5]</ref>"));
        }

        [Test, Ignore]
        public void ExtraBracketInExternalLink()
        {
            //http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_9#Bug_in_regex_to_correct_double_bracketed_external_links
            Assert.AreEqual("now [http://www.site.com a [[a]] site] was", Parsers.FixSyntax("now [http://www.site.com a [[a]] site] was"));  // valid syntax
            Assert.AreEqual("now [http://www.site.com a site [cool] here] was", Parsers.FixSyntax("now [http://www.site.com a site [cool] here] was"));         // valid syntax
            Assert.AreEqual("now [http://www.site.com a b site]] was", Parsers.FixSyntax("now [http://www.site.com a b site] was"));
            Assert.AreEqual("now [http://www.site.com a [[b]] site]] was", Parsers.FixSyntax("now [http://www.site.com a [[b]] site] was"));
            Assert.AreEqual("now [[http://www.site.com a c site] was", Parsers.FixSyntax("now [http://www.site.com a c site] was"));
            Assert.AreEqual("now [[http://www.site.com a [[c]] site] was", Parsers.FixSyntax("now [http://www.site.com a [[c]] site] was"));
            Assert.AreEqual("now [[http://www.site.com a [[d]] or [[d2]] site]] was", Parsers.FixSyntax("now [http://www.site.com a [[d]] or [[d2]] site] was"));
            Assert.AreEqual("now [[http://www.site.com a d3 site]] was", Parsers.FixSyntax("now [http://www.site.com a d3 site] was"));
            Assert.AreEqual("now [[Image:Fred1211212.JPG| here [http://www.site.com a [[e]] site]]] was", Parsers.FixSyntax("now [[Image:Fred1211212.JPG| here [http://www.site.com a [[e]] site]]] was"));   // valid wiki syntax
            Assert.AreEqual("now [[Image:Fred12.JPG| here [http://www.site.com a [[f]] site]]]] was", Parsers.FixSyntax("now [[Image:Fred12.JPG| here [http://www.site.com a [[f]] site]]] was"));
            Assert.AreEqual("now [[Image:Fred12.JPG| here [http://www.site.com a g site]]]] was", Parsers.FixSyntax("now [[Image:Fred12.JPG| here [http://www.site.com a g site]]] was"));
            Assert.AreEqual("now [[Image:Fred12.JPG| here [[http://www.site.com a g site]]] was", Parsers.FixSyntax("now [[Image:Fred12.JPG| here [http://www.site.com a g site]]] was"));
            Assert.AreEqual("now [[Image:Fred12.JPG| here [[http://www.site.com a g site]]]] was", Parsers.FixSyntax("now [[Image:Fred12.JPG| here [http://www.site.com a g site]]] was"));
        }

        [Test, Ignore] // TODO, fix this failing unit test
        public void FailingLinkRepair()
        {
            Assert.AreEqual("[[Image:foo.jpg|Some [http://some_crap.com]]]",
                Parsers.FixSyntax("[[Image:foo.jpg|Some [[http://some_crap.com]]]]"));
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

            Assert.AreEqual(@"His [[Tiger Woods#Career]] was", Parsers.FixLinkWhitespace(@"His [[Tiger Woods# Career]] was", "Tiger Woods"));

            // don't fix when bit before # is not article name
            Assert.AreNotSame(@"Fred's [[Smith#Career]] was", Parsers.FixLinkWhitespace(@"Fred's [[Smith# Career]] was", "Fred"));

            Assert.AreEqual(@"[[Category:London| ]]", Parsers.FixLinkWhitespace(@"[[Category:London| ]]")); // leading space NOT removed from cat sortkey
            Assert.AreEqual(@"[[Category:Slam poetry| ]] ", Parsers.FixLinkWhitespace(@"[[Category:Slam poetry| ]] ")); // leading space NOT removed from cat sortkey

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
            Assert.AreEqual("[[Category:Foo bar|quux]]", Parsers.FixCategories("[[category : foo_bar%20|quux]]"));

            // http://en.wikipedia.org/w/index.php?title=Wikipedia_talk:AutoWikiBrowser/Bugs&oldid=262844859#General_fixes_remove_spaces_from_category_sortkeys
            Assert.AreEqual(@"[[Category:Public transport in Auckland| Public transport in Auckland]]", Parsers.FixCategories(@"[[Category:Public transport in Auckland| Public transport in Auckland]]"));
            Assert.AreEqual(@"[[Category:Actors|Fred Astaire]]", Parsers.FixCategories(@"[[Category:Actors|Fred Astaire ]]")); // trailing space IS removed
            Assert.AreEqual(@"[[Category:Actors| Fred Astaire]]", Parsers.FixCategories(@"[[Category:Actors| Fred Astaire ]]")); // trailing space IS removed
            Assert.AreEqual(@"[[Category:London| ]]", Parsers.FixCategories(@"[[Category:London| ]]")); // leading space NOT removed
            Assert.AreEqual(@"[[Category:Slam poetry| ]] ", Parsers.FixCategories(@"[[Category:Slam poetry| ]] ")); // leading space NOT removed

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Archive_18#.2Fdoc_pages_and_includeonly_sections
            Assert.AreEqual("[[Category:Foo bar|boz_quux]]", Parsers.FixCategories("[[Category: foo_bar |boz_quux]]"));
            Assert.AreEqual("[[Category:Foo bar|{{boz_quux}}]]", Parsers.FixCategories("[[Category: foo_bar|{{boz_quux}}]]"));
            StringAssert.Contains("{{{boz_quux}}}", Parsers.FixCategories("[[CategorY : foo_bar{{{boz_quux}}}]]"));
            Assert.AreEqual("[[Category:Foo bar|{{{boz_quux}}}]]", Parsers.FixCategories("[[CategorY : foo_bar|{{{boz_quux}}}]]"));

            // diacritics removed from sortkeys
            Assert.AreEqual(@"[[Category:World Scout Committee members|Laine, Juan]]", Parsers.FixCategories(@"[[Category:World Scout Committee members|Lainé, Juan]]"));
        }

        [Test]
        public void TestFixEmptyLinksAndTemplates()
        {
            Assert.AreEqual("", Parsers.FixEmptyLinksAndTemplates(""));

            Assert.AreEqual("Test", Parsers.FixEmptyLinksAndTemplates("Test"));

            Assert.AreEqual("{{Test}}", Parsers.FixEmptyLinksAndTemplates("{{Test}}"));

            Assert.AreEqual("Test\r\n{{Test}}", Parsers.FixEmptyLinksAndTemplates("Test\r\n{{Test}}"));

            Assert.AreEqual("", Parsers.FixEmptyLinksAndTemplates("{{}}"));
            Assert.AreEqual("", Parsers.FixEmptyLinksAndTemplates("{{ }}"));
            Assert.AreEqual("", Parsers.FixEmptyLinksAndTemplates("{{|}}"));
            Assert.AreEqual("", Parsers.FixEmptyLinksAndTemplates("{{||||||||||||||||}}"));
            Assert.AreEqual("", Parsers.FixEmptyLinksAndTemplates("{{|||| |||||||                      ||  |||}}"));

            Assert.AreEqual("", Parsers.FixEmptyLinksAndTemplates("{{Template:}}"));
            Assert.AreEqual("", Parsers.FixEmptyLinksAndTemplates("{{Template:     |||}}"));

            Assert.AreEqual("{{Test}}", Parsers.FixEmptyLinksAndTemplates("{{  }}{{Test}}{{Template: ||}}"));

            Assert.AreEqual("[[Test]]", Parsers.FixEmptyLinksAndTemplates("[[Test]]"));
            Assert.AreEqual("[[Test|Bar]]", Parsers.FixEmptyLinksAndTemplates("[[Test|Bar]]"));
            Assert.AreEqual("[[|Bar]]", Parsers.FixEmptyLinksAndTemplates("[[|Bar]]"));

            Assert.AreEqual("", Parsers.FixEmptyLinksAndTemplates("[[]]"));
            Assert.AreEqual("", Parsers.FixEmptyLinksAndTemplates("[[  ]]"));

            Assert.AreEqual("[[Category:Test]]", Parsers.FixEmptyLinksAndTemplates("[[Category:Test]]"));
            Assert.AreEqual("Text [[Category:Test]]", Parsers.FixEmptyLinksAndTemplates("Text [[Category:Test]]"));

            Assert.AreEqual("", Parsers.FixEmptyLinksAndTemplates("[[Category:]]"));
            Assert.AreEqual("Text ", Parsers.FixEmptyLinksAndTemplates("Text [[Category:]]"));
            Assert.AreEqual("", Parsers.FixEmptyLinksAndTemplates("[[Category:  ]]"));

            Assert.AreEqual("[[Image:Test]]", Parsers.FixEmptyLinksAndTemplates("[[Image:Test]]"));
            Assert.AreEqual("Text [[Image:Test]]", Parsers.FixEmptyLinksAndTemplates("Text [[Image:Test]]"));
            Assert.AreEqual("[[File:Test]]", Parsers.FixEmptyLinksAndTemplates("[[File:Test]]"));
            Assert.AreEqual("Text [[File:Test]]", Parsers.FixEmptyLinksAndTemplates("Text [[File:Test]]"));

            Assert.AreEqual("", Parsers.FixEmptyLinksAndTemplates("[[Image:]]"));
            Assert.AreEqual("Text ", Parsers.FixEmptyLinksAndTemplates("Text [[Image:]]"));
            Assert.AreEqual("", Parsers.FixEmptyLinksAndTemplates("[[Image:  ]]"));

            Assert.AreEqual("", Parsers.FixEmptyLinksAndTemplates("[[File:]]"));
            Assert.AreEqual("Text ", Parsers.FixEmptyLinksAndTemplates("Text [[File:]]"));
            Assert.AreEqual("", Parsers.FixEmptyLinksAndTemplates("[[File:  ]]"));

            Assert.AreEqual("", Parsers.FixEmptyLinksAndTemplates("[[File:]][[Image:]]"));
            Assert.AreEqual("[[File:Test]]", Parsers.FixEmptyLinksAndTemplates("[[File:Test]][[Image:]]"));
        }

        [Test]
        public void TestFixNonBreakingSpaces()
        {
            Assert.AreEqual(@"a 50&nbsp;km road", Parsers.FixNonBreakingSpaces(@"a 50 km road"));
            Assert.AreEqual(@"a 50&nbsp;km road", Parsers.FixNonBreakingSpaces(@"a 50km road"));
            Assert.AreEqual(@"a 50&nbsp;kg dog", Parsers.FixNonBreakingSpaces(@"a 50 kg dog"));
            Assert.AreEqual(@"a 50&nbsp;kg dog", Parsers.FixNonBreakingSpaces(@"a 50kg dog"));
            Assert.AreEqual(@"a 50&nbsp;cm road", Parsers.FixNonBreakingSpaces(@"a 50 cm road"));
            Assert.AreEqual(@"a 50&nbsp;cm road", Parsers.FixNonBreakingSpaces(@"a 50cm road"));
            Assert.AreEqual(@"a 50.247&nbsp;cm road", Parsers.FixNonBreakingSpaces(@"a 50.247cm road"));
            Assert.AreEqual(@"a 50.247&nbsp;nm laser", Parsers.FixNonBreakingSpaces(@"a 50.247nm laser"));
            Assert.AreEqual(@"a 50.247&nbsp;nm laser", Parsers.FixNonBreakingSpaces(@"a 50.247  nm laser"));
            Assert.AreEqual(@"a 50.247&nbsp;cd light", Parsers.FixNonBreakingSpaces(@"a 50.247 cd light"));
            Assert.AreEqual(@"a 50.247&nbsp;cd light", Parsers.FixNonBreakingSpaces(@"a 50.247cd light"));
            Assert.AreEqual(@"a 50.247&nbsp;mmol solution", Parsers.FixNonBreakingSpaces(@"a 50.247mmol solution"));
            Assert.AreEqual(@"a 0.3&nbsp;mol solution", Parsers.FixNonBreakingSpaces(@"a 0.3mol solution"));

            // no changes for these
            Assert.AreEqual(@"nearly 5m people", Parsers.FixNonBreakingSpaces(@"nearly 5m people"));
            Assert.AreEqual(@"a 3CD set", Parsers.FixNonBreakingSpaces(@"a 3CD set"));
            Assert.AreEqual(@"http://site.com/View/3356 A show", Parsers.FixNonBreakingSpaces(@"http://site.com/View/3356 A show"));
            Assert.AreEqual(@"a 50&nbsp;km road", Parsers.FixNonBreakingSpaces(@"a 50&nbsp;km road"));
            Assert.AreEqual(@"over $200K in cash", Parsers.FixNonBreakingSpaces(@"over $200K in cash"));
            Assert.AreEqual(@"now {{a 50kg dog}} was", Parsers.FixNonBreakingSpaces(@"now {{a 50kg dog}} was"));
            Assert.AreEqual(@"now a [[50kg dog]] was", Parsers.FixNonBreakingSpaces(@"now a [[50kg dog]] was"));
            Assert.AreEqual(@"now “a 50kg dog” was", Parsers.FixNonBreakingSpaces(@"now “a 50kg dog” was"));
            Assert.AreEqual(@"now <!--a 50kg dog--> was", Parsers.FixNonBreakingSpaces(@"now <!--a 50kg dog--> was"));
            Assert.AreEqual(@"now <nowiki>a 50kg dog</nowiki> was", Parsers.FixNonBreakingSpaces(@"now <nowiki>a 50kg dog</nowiki> was"));
        }
    }

    [TestFixture]
    public class FormattingTests
    {
        public FormattingTests()
        {
            Globals.UnitTestMode = true;
        }

        [Test]
        public void TestBrConverter()
        {
            Assert.AreEqual("*a\r\nb", Parsers.FixSyntax("*a<br>\r\nb"));
            Assert.AreEqual("*a\r\nb", Parsers.FixSyntax("\r\n*a<br>\r\nb"));
            Assert.AreEqual("foo\r\n*a\r\nb", Parsers.FixSyntax("foo\r\n*a<br>\r\nb"));

            Assert.AreEqual("*a", Parsers.FixSyntax("*a<br>\r\n")); // \r\n\ trimmed

            Assert.AreEqual("*a", Parsers.FixSyntax("*a<br\\>\r\n"));
            Assert.AreEqual("*a", Parsers.FixSyntax("*a<br/>\r\n"));
            Assert.AreEqual("*a", Parsers.FixSyntax("*a <br\\> \r\n"));

            // leading (back)slash is hack for incorrectly formatted breaks per
            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_7#br_tags_are_not_always_removed
            Assert.AreEqual("*a", Parsers.FixSyntax("*a </br/> \r\n"));
            Assert.AreEqual("*a", Parsers.FixSyntax("*a<br\\> \r\n"));
            Assert.AreEqual("*a", Parsers.FixSyntax("*a <\\br\\>\r\n"));
            Assert.AreEqual("*a", Parsers.FixSyntax("*a     <br\\>    \r\n"));

            Assert.AreEqual("*:#;a\r\n*b", Parsers.FixSyntax("*:#;a<br>\r\n*b"));
            Assert.AreEqual("###;;;:::***a\r\nb", Parsers.FixSyntax("###;;;:::***a<br />\r\nb"));
            Assert.AreEqual("*&a\r\nb", Parsers.FixSyntax("*&a<br/>\r\nb"));

            Assert.AreEqual("&*a<br>\r\nb", Parsers.FixSyntax("&*a<br>\r\nb"));
            Assert.AreEqual("*a\r\n<br>\r\nb", Parsers.FixSyntax("*a\r\n<br>\r\nb"));
        }

        [Test]
        public void TestFixHeadings()
        {
            // breaks if article title is empty
            Assert.AreEqual("==foo==", Parsers.FixHeadings("=='''foo'''==", "test"));
            StringAssert.StartsWith("==foo==", Parsers.FixHeadings("=='''foo'''==\r\n", "test"));
            Assert.AreEqual("quux\r\n==foo==\r\nbar", Parsers.FixHeadings("quux\r\n=='''foo'''==\r\nbar", "test"));
            Assert.AreEqual("quux\r\n==foo==\r\n\r\nbar", Parsers.FixHeadings("quux\r\n=='''foo'''==\r\n\r\nbar", "test"));

            Assert.AreEqual("==foo==", Parsers.FixHeadings("==foo==", "test"));

            // following unit tests appear messy due to need to include whitespace and newlines
            Assert.AreEqual(@"hi.
===News===
Some news here.", Parsers.FixHeadings(@"hi.
 ===News===
Some news here.", "test"));
            Assert.AreEqual(@"hi.
==News place==
Some news here.", Parsers.FixHeadings(@"hi.
 ==News place==
Some news here.", "test"));
            Assert.AreEqual(@"hi.
==News place==
Some news here.", Parsers.FixHeadings(@"hi.
    ==News place==  
Some news here.", "test"));
            Assert.AreEqual(@"hi.
==News place==
Some news here.", Parsers.FixHeadings(@"hi.
==News place==  
Some news here.", "test"));

            // tests for regexRemoveHeadingsInLinks
            Assert.AreEqual("==foo==", Parsers.FixHeadings("==[[foo]]==", "test"));
            Assert.AreEqual("== foo ==", Parsers.FixHeadings("== [[foo]] ==", "test"));
            Assert.AreEqual("==bar==", Parsers.FixHeadings("==[[foo|bar]]==", "test"));

            // match
            Assert.AreEqual("==hello world ==", Parsers.FixHeadings("==hello [[world]] ==", "a"));
            Assert.AreEqual("==Hello world==", Parsers.FixHeadings("==[[Hello world]]==", "a"));
            Assert.AreEqual("==world==", Parsers.FixHeadings("==[[hello|world]]==", "a"));
            Assert.AreEqual("== world ==", Parsers.FixHeadings("== [[hello|world]] ==", "a"));
            Assert.AreEqual("===world===", Parsers.FixHeadings("===[[hello now|world]]===", "a"));
            Assert.AreEqual("==world now==", Parsers.FixHeadings("==[[hello|world now]]==", "a"));
            Assert.AreEqual("==now world==", Parsers.FixHeadings("==now [[hello|world]]==", "a"));
            Assert.AreEqual("==now world here==", Parsers.FixHeadings("==now [[hello|world]] here==", "a"));
            Assert.AreEqual("==now world def here==", Parsers.FixHeadings("==now [[hello abc|world def]] here==", "a"));
            Assert.AreEqual("==now world here==", Parsers.FixHeadings("==now [[ hello |world]] here==", "a"));
            Assert.AreEqual("==now world==", Parsers.FixHeadings("==now [[hello#world|world]]==", "a"));
            Assert.AreEqual("==now world and moon==", Parsers.FixHeadings("==now [[hello|world]] and [[bye|moon]]==", "a"));
            // no match
            Assert.AreEqual("===hello [[world]] ==", Parsers.FixHeadings("===hello [[world]] ==", "a"));
            Assert.AreEqual("==hello [[world]] ===", Parsers.FixHeadings("==hello [[world]] ===", "a"));
            Assert.AreEqual("== hello world ==", Parsers.FixHeadings("== hello world ==", "a"));
            Assert.AreEqual("==hello==", Parsers.FixHeadings("==hello==", "a"));
            Assert.AreEqual("==now [[hello|world] ] here==", Parsers.FixHeadings("==now [[hello|world] ] here==", "a"));
            Assert.AreEqual("==hello[http://example.net]==", Parsers.FixHeadings("==hello[http://example.net]==", "a"));
            Assert.AreEqual("hello [[world]]", Parsers.FixHeadings("hello [[world]]", "a"));
            Assert.AreEqual("now == hello [[world]] == here", Parsers.FixHeadings("now == hello [[world]] == here", "a"));
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
    public class MOSTests
    {
        public MOSTests()
        {
            Globals.UnitTestMode = true;
        }

        [Test]
        public void TestFixDateOrdinalsAndOf()
        {
            // 'of' between month and year
            Assert.AreEqual(@"Now in July 2007 a new", Parsers.FixDateOrdinalsAndOf(@"Now in July of 2007 a new", "test"));
            Assert.AreEqual(@"Now ''Plaice'' in July 2007 a new", Parsers.FixDateOrdinalsAndOf(@"Now ''Plaice'' in July of 2007 a new", "test"));
            Assert.AreEqual(@"Now in January 1907 a new", Parsers.FixDateOrdinalsAndOf(@"Now in January of 1907 a new", "test"));
            Assert.AreEqual(@"Now in January 1807 a new", Parsers.FixDateOrdinalsAndOf(@"Now in January of 1807 a new", "test"));
            Assert.AreEqual(@"Now in January 1807 and May 1804 a new", Parsers.FixDateOrdinalsAndOf(@"Now in January of 1807 and May of 1804 a new", "test"));

            // no matches
            Assert.AreEqual(@"Now ""in July of 2007"" a new", Parsers.FixDateOrdinalsAndOf(@"Now ""in July of 2007"" a new", "test"));
            Assert.AreEqual(@"Now {{quote|in July of 2007}} a new", Parsers.FixDateOrdinalsAndOf(@"Now {{quote|in July of 2007}} a new", "test"));
            Assert.AreEqual(@"Now ""in July of 1707"" a new", Parsers.FixDateOrdinalsAndOf(@"Now ""in July of 1707"" a new", "test"));
            Assert.AreEqual(@"Now a march of 2007 resulted", Parsers.FixDateOrdinalsAndOf(@"Now a march of 2007 resulted", "test"));
            Assert.AreEqual(@"Now the June of 2007 was", Parsers.FixDateOrdinalsAndOf(@"Now the June of 2007 was", "test"));

            // no ordinals on dates
            Assert.AreEqual(@"On 14 March elections were", Parsers.FixDateOrdinalsAndOf(@"On 14th March elections were", "test"));
            Assert.AreEqual(@"On 14 June elections were", Parsers.FixDateOrdinalsAndOf(@"On 14th June elections were", "test"));
            Assert.AreEqual(@"On March 14 elections were", Parsers.FixDateOrdinalsAndOf(@"On March 14th elections were", "test"));
            Assert.AreEqual(@"On June 21 elections were", Parsers.FixDateOrdinalsAndOf(@"On June 21st elections were", "test"));
            Assert.AreEqual(@"On 14 March 2008 elections were", Parsers.FixDateOrdinalsAndOf(@"On 14th March 2008 elections were", "test"));
            Assert.AreEqual(@"On 14 June 2008 elections were", Parsers.FixDateOrdinalsAndOf(@"On 14th June 2008 elections were", "test"));
            Assert.AreEqual(@"On March 14, 2008 elections were", Parsers.FixDateOrdinalsAndOf(@"On March 14th, 2008 elections were", "test"));
            Assert.AreEqual(@"On June 21, 2008 elections were", Parsers.FixDateOrdinalsAndOf(@"On June   21st, 2008 elections were", "test"));

            // date ranges
            Assert.AreEqual(@"On 14-15 June elections were", Parsers.FixDateOrdinalsAndOf(@"On 14th-15th June elections were", "test"));
            Assert.AreEqual(@"On 14 - 15 June elections were", Parsers.FixDateOrdinalsAndOf(@"On 14th - 15th June elections were", "test"));
            Assert.AreEqual(@"On 14 to 15 June elections were", Parsers.FixDateOrdinalsAndOf(@"On 14th to 15th June elections were", "test"));
            Assert.AreEqual(@"On 14,15 June elections were", Parsers.FixDateOrdinalsAndOf(@"On 14th,15th June elections were", "test"));
            Assert.AreEqual(@"On 3 and 15 June elections were", Parsers.FixDateOrdinalsAndOf(@"On 3rd and 15th June elections were", "test"));

            // no matches, particularly dates with 'the' before where fixing the ordinal may leave 'on the 11 May' which wouldn't read well
            Assert.AreEqual(@"On 14th march was", Parsers.FixDateOrdinalsAndOf(@"On 14th march was", "test"));
            Assert.AreEqual(@"On 14th of February was", Parsers.FixDateOrdinalsAndOf(@"On 14th of February was", "test"));
            Assert.AreEqual(@"Now the 14th February was", Parsers.FixDateOrdinalsAndOf(@"Now the 14th February was", "test"));
            Assert.AreEqual(@"Now the February 14th was", Parsers.FixDateOrdinalsAndOf(@"Now the February 14th was", "test"));
            Assert.AreEqual(@"'''6th October City''' is", Parsers.FixDateOrdinalsAndOf(@"'''6th October City''' is", "6th October City"));
        }
    }

    [TestFixture]
    public class ImageTests
    {
        public ImageTests()
        {
            Globals.UnitTestMode = true;
            if (WikiRegexes.LooseImage == null)
                WikiRegexes.MakeLangSpecificRegexes();
        }

        [Test, Category("Incomplete")]
        public void BasicImprovements()
        {
            Assert.AreEqual("[[File:foo.jpg|thumb|200px|Bar]]",
                Parsers.FixImages("[[ file : foo.jpg|thumb|200px|Bar]]"));

            Assert.AreEqual("[[Image:foo.jpg|thumb|200px|Bar]]",
                Parsers.FixImages("[[ image : foo.jpg|thumb|200px|Bar]]"));

            //TODO: decide if such improvements really belong here
            //Assert.AreEqual("[[Media:foo]]",
            //    Parsers.FixImages("[[ media : foo]]"));

            //Assert.AreEqual("[[:Media:foo]]",
            //    Parsers.FixImages("[[ : media : foo]]"));

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_6#URL_underscore_regression
            Assert.AreEqual("[[File:foo|thumb]] # [http://a_b c] [[link]]",
                Parsers.FixImages("[[File:foo|thumb]] # [http://a_b c] [[link]]"));

            Assert.AreEqual("[[Image:foo|thumb]] # [http://a_b c] [[link]]",
                Parsers.FixImages("[[Image:foo|thumb]] # [http://a_b c] [[link]]"));

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_2#Removing_underscore_in_URL_in_Ref_in_Description_in_Image....
            //Assert.AreEqual("[[Image:foo_bar|[http://some_link]]]",
            //    Parsers.FixImages("[[image:foo_bar|http://some_link]]"));

            // no changes should be made to this one
            Assert.AreEqual(@"[[Image:Diamminesilver(I)-3D-balls.png|thumb|right|200px|Ball-and-stick model of the diamminesilver(I) cation, [Ag(NH<sub>3</sub>)<sub>2</sub>]<sup>+</sup>]]",
                Parsers.FixImages(@"[[Image:Diamminesilver(I)-3D-balls.png|thumb|right|200px|Ball-and-stick model of the diamminesilver(I) cation, [Ag(NH<sub>3</sub>)<sub>2</sub>]<sup>+</sup>]]"));
        }

        [Test]
        public void Removal()
        {
            bool noChange;

            Assert.AreEqual("", Parsers.RemoveImage("Foo.jpg", "[[Image:Foo.jpg]]", false, "", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("", Parsers.RemoveImage("Foo.jpg", "[[:Image:Foo.jpg]]", false, "", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("", Parsers.RemoveImage("foo.jpg", "[[Image: foo.jpg]]", false, "", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("", Parsers.RemoveImage("Foo, bar", "[[File:foo%2C_bar|quux]]", false, "", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("", Parsers.RemoveImage("Foo%2C_bar", "[[File:foo, bar|quux]]", false, "", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("", Parsers.RemoveImage("foo.jpg", "[[Media:foo.jpg]]", false, "", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("", Parsers.RemoveImage("foo.jpg", "[[:media : foo.jpg]]", false, "", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("{{infobox|image=}}",
                Parsers.RemoveImage("foo", "{{infobox|image=foo}}", false, "", out noChange));
            Assert.IsFalse(noChange);
        }

        [Test]
        public void Replacement()
        {
            bool noChange;

            // just in case...
            Assert.AreEqual("", Parsers.ReplaceImage("", "", "", out noChange));
            Assert.IsTrue(noChange);

            Assert.AreEqual("[[File:bar]]", Parsers.ReplaceImage("foo", "bar", "[[File:Foo]]", out noChange));
            Assert.IsFalse(noChange);

            // preserve namespace
            Assert.AreEqual("[[Image:bar]]", Parsers.ReplaceImage("foo", "bar", "[[image:Foo]]", out noChange));
            Assert.IsFalse(noChange);

            // pipes, non-canonical NS casing
            Assert.AreEqual("[[File:bar|boz!|666px]]",
                Parsers.ReplaceImage("Foo%2C_bar", "bar", "[[FIle:foo, bar|boz!|666px]]", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("[[Media:bar]]", Parsers.ReplaceImage("foo", "bar", "[[Media:foo]]", out noChange));
            Assert.IsFalse(noChange);

            // normalising Media: is not yet supported, see TODO in BasicImprovements()
            Assert.AreEqual("[[:media:bar]]", Parsers.ReplaceImage("foo", "bar", "[[:media : foo]]", out noChange));
            Assert.IsFalse(noChange);
        }
    }

    [TestFixture]
    // tests have to have long strings due to logic in BoldTitle looking at bolding in first 5% of article only
    public class BoldTitleTests
    {
        bool noChangeBack;

        public BoldTitleTests()
        {
            Globals.UnitTestMode = true;
            if (WikiRegexes.Category == null) WikiRegexes.MakeLangSpecificRegexes();
        }

        [Test]
        //http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_1#Title_bolding
        public void DontEmboldenImagesAndTemplates()
        {
            Assert.That(Parsers.BoldTitle("[[Image:Foo.jpg]]", "Foo", out noChangeBack), Is.Not.Contains("'''Foo'''"));
            Assert.That(Parsers.BoldTitle("{{Foo}}", "Foo", out noChangeBack), Is.Not.Contains("'''Foo'''"));
            Assert.That(Parsers.BoldTitle("{{template| Foo is a bar}}", "Foo", out noChangeBack), Is.Not.Contains("'''Foo'''"));
        }

        [Test]
        public void DatesNotChanged()
        {
            Assert.AreEqual(@"May 31 is a great day", Parsers.BoldTitle(@"May 31 is a great day", "May 31", out noChangeBack));
            Assert.IsTrue(noChangeBack);
            Assert.AreEqual(@"March 1 is a great day", Parsers.BoldTitle(@"March 1 is a great day", "March 1", out noChangeBack));
            Assert.IsTrue(noChangeBack);
            Assert.AreEqual(@"31 May is a great day", Parsers.BoldTitle(@"31 May is a great day", "31 May", out noChangeBack));
            Assert.IsTrue(noChangeBack);
        }

        [Test]
        public void SimilarLinksWithDifferentCaseNotChanged()
        {
            Assert.AreEqual("'''Foo''' is this one, now [[FOO]] is another While remaining upright may be the primary goal of beginning riders", Parsers.BoldTitle("Foo is this one, now [[FOO]] is another While remaining upright may be the primary goal of beginning riders", "Foo", out noChangeBack));
            Assert.IsFalse(noChangeBack);
            Assert.AreEqual("'''Foo''' is this one, now [[FOO]] is another While remaining upright may be the primary goal of beginning riders", Parsers.BoldTitle("'''Foo''' is this one, now [[FOO]] is another While remaining upright may be the primary goal of beginning riders", "Foo", out noChangeBack));
            Assert.IsTrue(noChangeBack);
        }

        [Test]
        public void DontChangeIfAlreadyBold()
        {
            Assert.AreEqual("'''Foo''' is this one", Parsers.BoldTitle("'''Foo''' is this one", "Foo", out noChangeBack));
            Assert.IsTrue(noChangeBack);
            Assert.AreEqual("Foo is a bar, '''Foo''' moar", Parsers.BoldTitle("Foo is a bar, '''Foo''' moar", "Foo", out noChangeBack));
            Assert.IsTrue(noChangeBack);
            Assert.AreEqual(@"{{Infobox | name = Foo | age=11}} '''Foo''' is a bar", Parsers.BoldTitle(@"{{Infobox | name = Foo | age=11}} '''Foo''' is a bar", "Foo", out noChangeBack));
            Assert.IsTrue(noChangeBack);
            Assert.AreEqual(@"{{Infobox
| age=11}} '''John David Smith''' is a bar", Parsers.BoldTitle(@"{{Infobox
| age=11}} '''John David Smith''' is a bar", "John Smith", out noChangeBack));
            Assert.IsTrue(noChangeBack);

            // bold earlier in body of article
            Assert.AreEqual(@"{{Infobox| age=11}} '''John David Smith''' is a bar, John Smith", Parsers.BoldTitle(@"{{Infobox| age=11}} '''John David Smith''' is a bar, John Smith", "John Smith", out noChangeBack));
            Assert.IsTrue(noChangeBack);
            Assert.AreEqual(@"{{Infobox
| age=11}}{{box2}}} '''John David Smith''' is a bar", Parsers.BoldTitle(@"{{Infobox
| age=11}}{{box2}}} '''John David Smith''' is a bar", "John Smith", out noChangeBack));
            Assert.IsTrue(noChangeBack);

            // won't change if italics either
            Assert.AreEqual("''Foo'' is this one", Parsers.BoldTitle("''Foo'' is this one", "Foo", out noChangeBack));
            Assert.IsTrue(noChangeBack);

            Assert.AreEqual(@"{{Infobox_martial_art| website      = }}
{{Nihongo|'''Aikido'''|合気道|aikidō}} is a. Aikido was", Parsers.BoldTitle(@"{{Infobox_martial_art| website      = }}
{{Nihongo|'''Aikido'''|合気道|aikidō}} is a. Aikido was", "Aikido", out noChangeBack));
            Assert.IsTrue(noChangeBack);
        }

        [Test]
        public void StandardCases()
        {
            Assert.AreEqual("'''Foo''' is a bar While remaining upright may be the primary goal of beginning riders", Parsers.BoldTitle("Foo is a bar While remaining upright may be the primary goal of beginning riders", "Foo", out noChangeBack));
            Assert.IsFalse(noChangeBack);
            Assert.AreEqual("'''Foo in the wild''' is a bar While remaining upright may be the primary goal of beginning riders While remaining upright may be the primary goal of beginning riders", Parsers.BoldTitle("Foo in the wild is a bar While remaining upright may be the primary goal of beginning riders While remaining upright may be the primary goal of beginning riders", "Foo in the wild", out noChangeBack));
            Assert.IsFalse(noChangeBack);
            Assert.AreEqual("'''Foo''' is a bar, Foo moar While remaining upright may be the primary goal of beginning riders", Parsers.BoldTitle("Foo is a bar, Foo moar While remaining upright may be the primary goal of beginning riders", "Foo", out noChangeBack));
            Assert.IsFalse(noChangeBack);
            Assert.AreEqual("'''F^o^o''' is a bar While remaining upright may be the primary goal of beginning riders", Parsers.BoldTitle("F^o^o is a bar While remaining upright may be the primary goal of beginning riders", "F^o^o", out noChangeBack));
            Assert.IsFalse(noChangeBack);
            Assert.AreEqual(@"{{Infobox | name = Foo | age=11}}
'''Foo''' is a bar While remaining upright may be the primary goal of beginning riders While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders", Parsers.BoldTitle(@"{{Infobox | name = Foo | age=11}}
Foo is a bar While remaining upright may be the primary goal of beginning riders While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders", "Foo", out noChangeBack));
            Assert.IsFalse(noChangeBack);

            // brackets excluded from bolding
            Assert.AreEqual("'''Foo''' (Band album) is a CD While remaining upright may be the primary goal of beginning riders", Parsers.BoldTitle("Foo (Band album) is a CD While remaining upright may be the primary goal of beginning riders", "Foo (Band album)", out noChangeBack));
            Assert.IsFalse(noChangeBack);

            // non-changes
            Assert.AreEqual("Fooo is a bar", Parsers.BoldTitle("Fooo is a bar", "Foo", out noChangeBack));
            Assert.IsTrue(noChangeBack);

            Assert.AreEqual(@"Foo is a '''bar''' While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders", Parsers.BoldTitle(@"Foo is a '''bar''' While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders", "Foo", out noChangeBack)); // bold within first 5% of article
            Assert.IsTrue(noChangeBack);
        }

        [Test]
        public void WithDelinking()
        {
            Assert.AreEqual("'''Foo''' is a bar While remaining upright may be the primary goal of beginning riders", Parsers.BoldTitle("[[Foo]] is a bar While remaining upright may be the primary goal of beginning riders", "Foo", out noChangeBack));
            Assert.IsFalse(noChangeBack);
            Assert.AreEqual("'''Foo''' is a bar, Foo moar While remaining upright may be the primary goal of beginning riders", Parsers.BoldTitle("[[Foo]] is a bar, Foo moar While remaining upright may be the primary goal of beginning riders", "Foo", out noChangeBack));
            Assert.IsFalse(noChangeBack);
            Assert.AreEqual("'''Foo''' is a bar, now Foo here While remaining upright may be the primary goal of beginning riders", Parsers.BoldTitle("Foo is a bar, now [[Foo]] here While remaining upright may be the primary goal of beginning riders", "Foo", out noChangeBack));
            Assert.IsFalse(noChangeBack);
            Assert.AreEqual("'''Foo''' is a bar, now foo here While remaining upright may be the primary goal of beginning riders", Parsers.BoldTitle("Foo is a bar, now [[foo]] here While remaining upright may be the primary goal of beginning riders", "Foo", out noChangeBack));
            Assert.IsFalse(noChangeBack);
            Assert.AreEqual("'''Foo''' is a [[bar]] While remaining upright may be the primary goal of beginning riders", Parsers.BoldTitle("[[Foo]] is a [[bar]] While remaining upright may be the primary goal of beginning riders", "Foo", out noChangeBack));
            Assert.IsFalse(noChangeBack);

            // removal of self links in iteslf are not a 'change'
            Assert.AreEqual("'''Foo''' is a bar, now [[Foo]] here While remaining upright may be the primary goal of beginning riders", Parsers.BoldTitle("'''Foo''' is a bar, now [[Foo]] here While remaining upright may be the primary goal of beginning riders", "Foo", out noChangeBack));
            Assert.IsTrue(noChangeBack);
        }

        [Test]
        // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_9#Bold_letters
        public void ExamplesFromBugReport()
        {
            Assert.AreEqual(@"'''Michael Bavaro''' is a [[filmmaker]] based in [[Manhattan]]. While remaining upright may be the primary goal of beginning riders, a bike must lean in order to maintain balance", Parsers.BoldTitle(@"[[Michael Bavaro]] is a [[filmmaker]] based in [[Manhattan]]. While remaining upright may be the primary goal of beginning riders, a bike must lean in order to maintain balance", "Michael Bavaro", out noChangeBack));
            Assert.IsFalse(noChangeBack);

            Assert.AreEqual(@"{{Unreferenced|date=October 2007}}
'''Steve Cook''' is a songwriter for Sovereign Grace. While remaining upright may be the primary goal of beginning riders. While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders While remaining upright may be the primary goal of beginning riders", Parsers.BoldTitle(@"{{Unreferenced|date=October 2007}}
Steve Cook is a songwriter for Sovereign Grace. While remaining upright may be the primary goal of beginning riders. While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders While remaining upright may be the primary goal of beginning riders", "Steve Cook", out noChangeBack));
            Assert.IsFalse(noChangeBack);
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

        [Test]
        // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_7#Problem_with_.22Main_article.22_fixup
        public void Newlines()
        {
            Assert.AreEqual("test\r\n{{main|Foo}}\r\ntest", Parsers.FixMainArticle("test\r\nMain article: [[Foo]]\r\ntest"));
            Assert.AreEqual("test\r\n\r\n{{main|Foo}}\r\n\r\ntest", Parsers.FixMainArticle("test\r\n\r\nMain article: [[Foo]]\r\n\r\ntest"));
        }
    }

    [TestFixture]
    public class UnicodifyTests
    {
        [Test]
        public void PreserveTM()
        {
            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_1#AWB_corrupts_the_trademark_.28TM.29_special_character_.
            Assert.AreEqual("test™", Parsers.Unicodify("test™"));
        }

        [Test]
        public void DontChangeCertainEntities()
        {
            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_3#.26emsp.3B
            Assert.AreEqual("&emsp;&#013;", Parsers.Unicodify("&emsp;&#013;"));

            Assert.AreEqual("The F&#x2011;22 plane", Parsers.Unicodify("The F&#x2011;22 plane"));
        }

        [Test]
        public void IgnoreMath()
        {
            Assert.AreEqual("<math>&laquo;</math>", Parsers.Unicodify("<math>&laquo;</math>"));
        }
    }

    [TestFixture]
    public class UtilityFunctionTests
    {
        public UtilityFunctionTests()
        {
            Globals.UnitTestMode = true;
            Variables.SetToEnglish();
        }

        [Test]
        public void IsCorrectEditSummary()
        {
            // too long
            StringBuilder sb = new StringBuilder(300);
            for (int i = 0; i < 300; i++) sb.Append('x');
            Assert.IsFalse(Parsers.IsCorrectEditSummary(sb.ToString()));

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
        public void ChangeToDefaultSort()
        {
            bool noChange;

            // don't change sorting for single categories
            Assert.AreEqual("[[Category:Test1|Foooo]]",
                            Parsers.ChangeToDefaultSort("[[Category:Test1|Foooo]]", "Foo", out noChange));
            Assert.IsTrue(noChange);

            // should work
            Assert.AreEqual("[[Category:Test1]][[Category:Test2]]\r\n{{DEFAULTSORT:Foooo}}",
                            Parsers.ChangeToDefaultSort("[[Category:Test1|Foooo]][[Category:Test2|Foooo]]", "Bar",
                                                  out noChange));
            Assert.IsFalse(noChange);

            // ...but don't add DEFAULTSORT if the key equals page title
            Assert.AreEqual("[[Category:Test1]][[Category:Test2]]",
                            Parsers.ChangeToDefaultSort("[[Category:Test1|Foooo]][[Category:Test2|Foooo]]", "Foooo",
                                                  out noChange));
            Assert.IsFalse(noChange, "Should detect a change even if it hasn't added a DEFAULTSORT");

            // don't change if key is 3 chars or less
            Assert.AreEqual("[[Category:Test1|Foo]][[Category:Test2|Foo]]",
                            Parsers.ChangeToDefaultSort("[[Category:Test1|Foo]][[Category:Test2|Foo]]", "Bar", out noChange));
            Assert.IsTrue(noChange);

            // Remove explicit keys equal to page title
            Assert.AreEqual("[[Category:Test1]][[Category:Test2]]",
                            Parsers.ChangeToDefaultSort("[[Category:Test1|Foooo]][[Category:Test2]]", "Foooo", out noChange));
            Assert.IsFalse(noChange);
            // swap
            Assert.AreEqual("[[Category:Test1]][[Category:Test2]]",
                            Parsers.ChangeToDefaultSort("[[Category:Test1]][[Category:Test2|Foooo]]", "Foooo", out noChange));
            Assert.IsFalse(noChange);

            // Borderline condition
            Assert.AreEqual("[[Category:Test1|Fooooo]][[Category:Test2]]",
                            Parsers.ChangeToDefaultSort("[[Category:Test1|Fooooo]][[Category:Test2]]", "Foooo", out noChange));
            Assert.IsTrue(noChange);

            // Don't change anything if there's ambiguity
            Assert.AreEqual("[[Category:Test1|Foooo]][[Category:Test2|Baaar]]",
                            Parsers.ChangeToDefaultSort("[[Category:Test1|Foooo]][[Category:Test2|Baaar]]", "Teeest",
                                                  out noChange));
            Assert.IsTrue(noChange);
            // same thing
            Assert.AreEqual("[[Category:Test1|Foooo]][[Category:Test2|Baaar]]",
                            Parsers.ChangeToDefaultSort("[[Category:Test1|Foooo]][[Category:Test2|Baaar]]", "Foooo",
                                                  out noChange));
            Assert.IsTrue(noChange);

            // remove diacritics when generating a key
            Assert.AreEqual("[[Category:Test1]][[Category:Test2]]\r\n{{DEFAULTSORT:Foooo}}",
                            Parsers.ChangeToDefaultSort("[[Category:Test1|Foooô]][[Category:Test2|Foooô]]", "Bar",
                                                  out noChange));
            Assert.IsFalse(noChange);

            // should also fix diacritics in existing defaultsort's and remove leading spaces
            // also support mimicking templates
            Assert.AreEqual("{{DEFAULTSORT:Test}}",
                            Parsers.ChangeToDefaultSort("{{defaultsort| Tést}}", "Foo", out noChange));
            Assert.IsFalse(noChange);

            // shouldn't change whitespace-only sortkeys
            Assert.AreEqual("{{DEFAULTSORT: \t}}", Parsers.ChangeToDefaultSort("{{DEFAULTSORT: \t}}", "Foo", out noChange));
            Assert.IsTrue(noChange);

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_6#DEFAULTSORT_with_spaces
            // DEFAULTSORT doesn't treat leading spaces the same way as categories do
            Assert.AreEqual("[[Category:Test1| Foooo]][[Category:Test2| Foooo]]",
                            Parsers.ChangeToDefaultSort("[[Category:Test1| Foooo]][[Category:Test2| Foooo]]", "Bar",
                                                  out noChange));
            Assert.IsTrue(noChange);

            // {{lifetime}} and crap like that is not supported
            Parsers.ChangeToDefaultSort("{{lifetime|shite}}[[Category:Test1|Foooo]][[Category:Test2|Foooo]]",
                                  "Bar", out noChange);
            Assert.IsTrue(noChange);

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_7#AWB_needs_to_handle_lifetime_template_correctly
            // pages with multiple sort specifiers shouldn't be changed
            Parsers.ChangeToDefaultSort("{{DEFAULTSORT:Foo}}{{lifetime|Bar}}",
                                  "Foo", out noChange);
            Assert.IsTrue(noChange);
            // continued...
            Parsers.ChangeToDefaultSort("{{defaultsort| Tést}}{{DEFAULTSORT: Tést}}", "Foo", out noChange);
            Assert.IsTrue(noChange);

            //Remove explicitally defined sort keys from categories when the page has defaultsort
            Assert.AreEqual("{{DEFAULTSORT:Test}}[[Category:Test]]",
                            Parsers.ChangeToDefaultSort("{{DEFAULTSORT:Test}}[[Category:Test|Test]]", "Foo", out noChange));
            Assert.IsFalse(noChange);

            //Case difference of above
            Assert.AreEqual("{{DEFAULTSORT:Test}}[[Category:Test]]",
                Parsers.ChangeToDefaultSort("{{DEFAULTSORT:Test}}[[Category:Test|TEST]]", "Foo", out noChange));
            Assert.IsFalse(noChange);

            //No change due to different key
            Assert.AreEqual("{{DEFAULTSORT:Test}}[[Category:Test|Not a Test]]",
    Parsers.ChangeToDefaultSort("{{DEFAULTSORT:Test}}[[Category:Test|Not a Test]]", "Foo", out noChange));
            Assert.IsTrue(noChange);

            //Multiple to be removed
            Assert.AreEqual("{{DEFAULTSORT:Test}}[[Category:Test]][[Category:Foo]][[Category:Bar]]",
    Parsers.ChangeToDefaultSort("{{DEFAULTSORT:Test}}[[Category:Test|TEST]][[Category:Foo|Test]][[Category:Bar|test]]", "Foo", out noChange));
            Assert.IsFalse(noChange);

            //Multiple with 1 no key
            Assert.AreEqual("{{DEFAULTSORT:Test}}[[Category:Test]][[Category:Foo]][[Category:Bar]]",
Parsers.ChangeToDefaultSort("{{DEFAULTSORT:Test}}[[Category:Test|TEST]][[Category:Foo]][[Category:Bar|test]]", "Foo", out noChange));
            Assert.IsFalse(noChange);

            //Multiple with 1 different key
            Assert.AreEqual("{{DEFAULTSORT:Test}}[[Category:Test]][[Category:Foo|Bar]][[Category:Bar]]",
Parsers.ChangeToDefaultSort("{{DEFAULTSORT:Test}}[[Category:Test|TEST]][[Category:Foo|Bar]][[Category:Bar|test]]", "Foo", out noChange));
            Assert.IsFalse(noChange);

            // just removing diacritics in categories is useful
            Assert.AreEqual(@"[[Category:Bronze Wolf awardees|Laine, Juan]]", Parsers.ChangeToDefaultSort(@"[[Category:Bronze Wolf awardees|Lainé, Juan]]", "Hi", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual(@"[[Category:Bronze Wolf awardees|Laine, Juan]]", Parsers.ChangeToDefaultSort(@"[[Category:Bronze Wolf awardees|Laine, Juan]]", "Hi", out noChange));
            Assert.IsTrue(noChange);
        }

        [Test, Ignore("Unused"), Category("Incomplete")]
        public void ExternalURLToInternalLink()
        {
            //TODO:MOAR
            Assert.AreEqual("", Parsers.ExternalURLToInternalLink(""));
            Assert.AreEqual("%20", Parsers.ExternalURLToInternalLink("%20"));

            // ExtToInt1
            Assert.AreEqual("https://secure.wikimedia.org/otrs/index.pl?Action=AgentTicketQueue",
                Parsers.ExternalURLToInternalLink("https://secure.wikimedia.org/otrs/index.pl?Action=AgentTicketQueue"));

            // ExtToInt2
            Assert.AreEqual("[[:ru:Яипу|Foo]]", Parsers.ExternalURLToInternalLink("[http://ru.wikipedia.org/wiki/Яипу Foo]"));
        }

        [Test]
        public void RemoveEmptyComments()
        {
            Assert.AreEqual("", Parsers.RemoveEmptyComments("<!---->"));
            Assert.AreEqual("", Parsers.RemoveEmptyComments("<!-- -->"));

            // newline comments are used to split wikitext to lines w/o breaking formatting,
            // they should not be removed
            Assert.AreEqual("<!--\r\n\r\n-->", Parsers.RemoveEmptyComments("<!--\r\n\r\n-->"));

            Assert.AreEqual("", Parsers.RemoveEmptyComments("<!----><!---->"));
            Assert.AreEqual("<!--\r\n\r\n-->", Parsers.RemoveEmptyComments("<!--\r\n\r\n--><!---->"));
            Assert.AreEqual("<!--Test-->", Parsers.RemoveEmptyComments("<!----><!--Test-->"));
            Assert.AreEqual(" <!--Test-->", Parsers.RemoveEmptyComments("<!----> <!--Test-->"));
            Assert.AreEqual("<!--Test\r\nfoo--> <!--Test-->", Parsers.RemoveEmptyComments("<!--Test\r\nfoo--> <!--Test-->"));

            Assert.AreEqual("<!--Test-->", Parsers.RemoveEmptyComments("<!--Test-->"));

            Assert.AreEqual("", Parsers.RemoveEmptyComments(""));
            Assert.AreEqual("test", Parsers.RemoveEmptyComments("test"));
        }

        [Test]
        public void HasSicTagTests()
        {
            Assert.IsTrue(Parsers.HasSicTag("now helo [sic] there"));
            Assert.IsTrue(Parsers.HasSicTag("now helo[sic] there"));
            Assert.IsTrue(Parsers.HasSicTag("now helo (sic) there"));
            Assert.IsTrue(Parsers.HasSicTag("now helo {sic} there"));
            Assert.IsTrue(Parsers.HasSicTag("now helo [Sic] there"));
            Assert.IsTrue(Parsers.HasSicTag("now helo [ Sic ] there"));
            Assert.IsTrue(Parsers.HasSicTag("now {{sic|helo}} there"));
            Assert.IsTrue(Parsers.HasSicTag("now {{sic|hel|o}} there"));
            Assert.IsTrue(Parsers.HasSicTag("now {{typo|helo}} there"));
            Assert.IsTrue(Parsers.HasSicTag("now helo <!--[sic]-->there"));

            Assert.IsFalse(Parsers.HasSicTag("now sickened by"));
            Assert.IsFalse(Parsers.HasSicTag("now helo sic there"));
            Assert.IsFalse(Parsers.HasSicTag("The Sound Information Company (SIC) is"));
        }

        [Test]
        public void HasInUseTagTests()
        {
            Assert.IsTrue(Parsers.IsInUse("{{inuse}} Hello world"));
            Assert.IsTrue(Parsers.IsInUse("{{ inuse  }} Hello world"));
            Assert.IsTrue(Parsers.IsInUse("{{Inuse}} Hello world"));
            Assert.IsTrue(Parsers.IsInUse("Hello {{inuse}} Hello world"));
            Assert.IsTrue(Parsers.IsInUse("{{inuse|5 minutes}} Hello world"));

            // ignore commented inuse
            Assert.IsFalse(Parsers.IsInUse("<!--{{inuse}}--> Hello world"));
            Assert.IsTrue(Parsers.IsInUse("<!--{{inuse}}--> {{inuse|5 minutes}} Hello world"));

            Assert.IsFalse(Parsers.IsInUse("{{INUSE}} Hello world")); // no such template
        }

        [Test]
        public void IsMissingReferencesDisplayTests()
        {
            Assert.IsTrue(Parsers.IsMissingReferencesDisplay(@"Hello<ref>Fred</ref>"));
            Assert.IsTrue(Parsers.IsMissingReferencesDisplay(@"Hello<ref name=""F"">Fred</ref>"));

            Assert.IsFalse(Parsers.IsMissingReferencesDisplay(@"Hello"));
            Assert.IsFalse(Parsers.IsMissingReferencesDisplay(@"Hello<ref>Fred</ref> {{reflist}}"));
            Assert.IsFalse(Parsers.IsMissingReferencesDisplay(@"Hello<ref>Fred</ref> {{ref-list}}"));
            Assert.IsFalse(Parsers.IsMissingReferencesDisplay(@"Hello<ref>Fred</ref> {{reflink}}"));
            Assert.IsFalse(Parsers.IsMissingReferencesDisplay(@"Hello<ref>Fred</ref> {{references}}"));
            Assert.IsFalse(Parsers.IsMissingReferencesDisplay(@"Hello<ref>Fred</ref> <references/>"));
        }

        [Test]
        public void InfoboxTests()
        {
            Assert.IsTrue(Parsers.HasInfobox(@"{{Infobox fish | name = Bert }} ''Bert'' is a good fish."));
            Assert.IsTrue(Parsers.HasInfobox(@"{{Infobox 
fish | name = Bert }} ''Bert'' is a good fish."));
            Assert.IsTrue(Parsers.HasInfobox(@"{{infobox fish | name = Bert }} ''Bert'' is a good fish."));
            Assert.IsTrue(Parsers.HasInfobox(@"{{ infobox fish | name = Bert }} ''Bert'' is a good fish."));
            Assert.IsTrue(Parsers.HasInfobox(@"{{ infobox fish | name = Bert }} ''Bert'' is a good <!--fish-->."));

            Assert.IsFalse(Parsers.HasInfobox(@"{{INFOBOX fish | name = Bert }} ''Bert'' is a good fish."));
            Assert.IsFalse(Parsers.HasInfobox(@"{{infoboxfish | name = Bert }} ''Bert'' is a good fish."));
            Assert.IsFalse(Parsers.HasInfobox(@"<!--{{infobox fish | name = Bert }}--> ''Bert'' is a good fish."));
            Assert.IsFalse(Parsers.HasInfobox(@"<nowiki>{{infobox fish | name = Bert }}</nowiki> ''Bert'' is a good fish."));
        }

        [Test]
        public void NoBotsTests()
        {
            Assert.IsTrue(Parsers.CheckNoBots("", ""));
            Assert.IsTrue(Parsers.CheckNoBots("{{test}}", ""));
            Assert.IsTrue(Parsers.CheckNoBots("lol, test", ""));

            Assert.IsTrue(Parsers.CheckNoBots("{{bots}}", ""));
            Assert.IsTrue(Parsers.CheckNoBots("{{bots|allow=awb}}", ""));
            Assert.IsTrue(Parsers.CheckNoBots("{{bots|allow=test}}", "test"));
            Assert.IsTrue(Parsers.CheckNoBots("{{bots|allow=user,test}}", "test"));
            Assert.IsTrue(Parsers.CheckNoBots("{{bots|deny=none}}", ""));

            Assert.IsFalse(Parsers.CheckNoBots("{{nobots}}", ""));
            Assert.IsFalse(Parsers.CheckNoBots("{{bots|deny=all}}", ""));
            Assert.IsFalse(Parsers.CheckNoBots("{{bots|deny=awb}}", ""));
            Assert.IsFalse(Parsers.CheckNoBots("{{bots|deny=awb,test}}", ""));
            Assert.IsFalse(Parsers.CheckNoBots("{{bots|deny=awb,test}}", "test"));
            Assert.IsFalse(Parsers.CheckNoBots("{{bots|deny=test}}", "test"));
            Assert.IsFalse(Parsers.CheckNoBots("{{bots|allow=none}}", ""));
        }
    }

    [TestFixture]
    public class RecategorizerTests
    {
        readonly Parsers p = new Parsers();

        public RecategorizerTests()
        {
            Globals.UnitTestMode = true;
        }

        [Test]
        public void Addition()
        {
            bool noChange;

            Assert.AreEqual("\r\n\r\n[[Category:Foo]]\r\n", p.AddCategory("Foo", "", "bar", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("bar\r\n\r\n[[Category:Foo]]\r\n", p.AddCategory("Foo", "bar", "bar", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("test\r\n\r\n[[Category:Foo|bar]]\r\n[[Category:Bar]]\r\n",
                p.AddCategory("Bar", "test[[Category:Foo|bar]]", "foo", out noChange));
            Assert.IsFalse(noChange);

            // shouldn't add if category already exists
            Assert.AreEqual("[[Category:Foo]]", p.AddCategory("Foo", "[[Category:Foo]]", "bar", out noChange));
            Assert.IsTrue(noChange);
            Assert.That(p.AddCategory("Foo bar", "[[Category:Foo_bar]]", "bar", out noChange),
                Text.Matches(@"\[\[Category:Foo[ _]bar\]\]"));
            Assert.IsTrue(noChange);

            Assert.AreEqual("[[category : foo_bar%20|quux]]", p.AddCategory("Foo bar", "[[category : foo_bar%20|quux]]", "bar", out noChange));
            Assert.IsTrue(noChange);

            Assert.AreEqual("test<noinclude>\r\n[[Category:Foo]]\r\n</noinclude>",
                p.AddCategory("Foo", "test", "Template:foo", out noChange));
            Assert.IsFalse(noChange);
        }

        [Test]
        public void Replacement()
        {
            bool noChange;

            Assert.AreEqual("[[Category:Bar]]", Parsers.ReCategoriser("Foo", "Bar", "[[Category:Foo]]", out noChange));
            Assert.IsFalse(noChange);

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_7#Replacing_Arabic_categories
            // addresses special case in Tools.CaseInsensitive
            Assert.AreEqual("[[Category:Bar]]", Parsers.ReCategoriser("-Foo bar-", "Bar", "[[Category:-Foo bar-]]", out noChange));
            Assert.IsFalse(noChange);
            Assert.AreEqual("[[Category:-Bar II-]]", Parsers.ReCategoriser("Foo", "-Bar II-", "[[Category:Foo]]", out noChange));
            Assert.IsFalse(noChange);


            Assert.AreEqual("[[Category:Bar]]", Parsers.ReCategoriser("Foo", "Bar", "[[ catEgory: Foo]]", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("[[Category:Bar]]", Parsers.ReCategoriser("Foo", "Bar", "[[Category:foo]]", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("[[Category:Bar|boz]]", Parsers.ReCategoriser("Foo", "Bar", "[[Category:Foo|boz]]", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("[[Category:Bar| boz]]", Parsers.ReCategoriser("foo? Bar!", "Bar", "[[ category:Foo?_Bar! | boz]]", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual(@"[[Category:Boz]]
[[Category:Bar]]
[[Category:Quux]]", Parsers.ReCategoriser("Foo", "Bar", @"[[Category:Boz]]
[[Category:foo]]
[[Category:Quux]]", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("test[[Category:Bar]]test", Parsers.ReCategoriser("Foo", "Bar", "test[[Category:Foo]]test", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("[[Category:Fooo]]", Parsers.ReCategoriser("Foo", "Bar", "[[Category:Fooo]]", out noChange));
            Assert.IsTrue(noChange);
        }

        [Test]
        public void Removal()
        {
            bool noChange;

            Assert.AreEqual("", Parsers.RemoveCategory("Foo", "[[Category:Foo]]", out noChange));
            Assert.IsFalse(noChange);

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_7#Replacing_Arabic_categories
            // addresses special case in Tools.CaseInsensitive
            Assert.AreEqual("", Parsers.RemoveCategory("-Foo bar-", "[[Category:-Foo bar-]]", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("", Parsers.RemoveCategory("Foo", "[[ category: foo | bar]]", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("", Parsers.RemoveCategory("Foo", "[[Category:Foo|]]", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("  ", Parsers.RemoveCategory("Foo", " [[Category:Foo]] ", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("", Parsers.RemoveCategory("Foo", "[[Category:Foo]]\r\n", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("\r\n", Parsers.RemoveCategory("Foo", "[[Category:Foo]]\r\n\r\n", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("", Parsers.RemoveCategory("Foo? Bar!", "[[Category:Foo?_Bar!|boz]]", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("[[Category:Fooo]]", Parsers.RemoveCategory("Foo", "[[Category:Fooo]]", out noChange));
            Assert.IsTrue(noChange);
        }
    }

    [TestFixture]
    public class TaggerTests
    {
        private bool noChange;
        private string summary;

        private readonly Parsers p = new Parsers(500, true);
        public TaggerTests()
        {
            Globals.UnitTestMode = true;
            WikiRegexes.MakeLangSpecificRegexes();
        }

        private const string uncat = "{{Uncategorized|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}",
                       uncatstub = "{{Uncategorizedstub|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}",
                       orphan = "{{orphan|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}",
                       wikify = "{{Wikify|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}",
                       deadend = "{{deadend|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}",
                       stub = "{{stub}}";

        [Test]
        public void Add()
        {
            Globals.UnitTestIntValue = 0;
            Globals.UnitTestBoolValue = true;

            string text = p.Tagger(shortText, "Test", out noChange, ref summary, true, false); 
            //Stub, no existing stub tag. Needs all tags
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text));
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(text));
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(text));
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text));

            Assert.IsFalse(text.Contains(uncatstub));

            text = p.Tagger(shortText + stub + uncat + wikify + orphan + deadend, "Test", out noChange, ref summary, true, false);
            //Tagged article, dupe tags shouldn't be added
            Assert.AreEqual(1, Tools.RegexMatchCount(Regex.Escape(stub), text));
            Assert.AreEqual(1, Tools.RegexMatchCount(Regex.Escape(uncat), text));
            Assert.AreEqual(1, Tools.RegexMatchCount(Regex.Escape(wikify), text));
            Assert.AreEqual(1, Tools.RegexMatchCount(Regex.Escape(orphan), text));
            Assert.AreEqual(1, Tools.RegexMatchCount(Regex.Escape(deadend), text));

            text = p.Tagger(shortText + stub, "Test", out noChange, ref summary, true, false);
            //Stub, existing stub tag
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text));
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(text));
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text));
            Assert.IsTrue(text.Contains(uncatstub));
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text));

            Assert.IsFalse(text.Contains(uncat));

            Assert.AreEqual(1, Tools.RegexMatchCount(Regex.Escape(stub), text));

            text = p.Tagger(shortText + shortText, "Test", out noChange, ref summary, true, false);
            //Not a stub
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text));
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(text));
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(text));

            Assert.IsFalse(text.Contains(uncatstub));
            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text));

            Globals.UnitTestIntValue = 5;

            text = p.Tagger(shortText, "Test", out noChange, ref summary, true, false);
            //Categorised Stub
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text));
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(text));
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text));
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text));

            Assert.IsFalse(text.Contains(uncatstub));
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text));

            text = p.Tagger(shortText + shortText, "Test", out noChange, ref summary, true, false);
            //Categorised Page
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text));
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(text));
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text));

            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text));
            Assert.IsFalse(text.Contains(uncatstub));
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text));

            Globals.UnitTestBoolValue = false;

            text = p.Tagger(shortText, "Test", out noChange, ref summary, true, false);
            //Non orphan categorised stub
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(text));
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text));
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text));

            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));
            Assert.IsFalse(text.Contains(uncatstub));
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text));

            text = p.Tagger(shortText + shortText, "Test", out noChange, ref summary, true, false);
            //Non orphan categorised page
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(text));
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text));

            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text));
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));
            Assert.IsFalse(text.Contains(uncatstub));
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text));

            text = p.Tagger(shortText.Replace("consectetur", "[[consectetur]]"), "Test", out noChange, ref summary, true, false);
            //Non deadend stub
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text));

            Assert.IsFalse(WikiRegexes.Wikify.IsMatch(text));
            Assert.IsFalse(WikiRegexes.DeadEnd.IsMatch(text));
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));
            Assert.IsFalse(text.Contains(uncatstub));
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text));

            text = p.Tagger(Regex.Replace(shortText, @"(\w+)", "[[$1]]"), "Test", out noChange, ref summary, true, false);
            //very wikified stub
            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text));
            Assert.IsFalse(WikiRegexes.Wikify.IsMatch(text));
            Assert.IsFalse(WikiRegexes.DeadEnd.IsMatch(text));
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));
            Assert.IsFalse(text.Contains(uncatstub));
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text));
        }

        private const string shortText =
            @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Curabitur sit amet tortor nec neque faucibus pharetra. Fusce lorem arcu, tempus et, imperdiet a, commodo a, pede. Nulla sit amet turpis gravida elit dictum cursus. Praesent tincidunt velit eu urna.";

        private const string longText =
            @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse dictum ultrices augue. Fusce sem diam, vestibulum sit amet, vehicula id, congue a, nisl. Phasellus pulvinar posuere purus. Donec elementum justo mattis nulla. Sed a purus dictum lacus pharetra adipiscing. Nam non dui non ante viverra iaculis. Fusce euismod lacus id nulla vulputate gravida. Suspendisse lectus pede, tempus sed, tristique id, pharetra eget, urna. Integer mattis libero vel quam accumsan suscipit. Vivamus molestie dapibus est. Quisque quis metus eget nisl accumsan aliquet. Donec tempus pellentesque tellus. Aliquam lacinia gravida justo. Aliquam erat volutpat. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Mauris ultricies suscipit urna. Ut mollis tempor leo. Pellentesque fringilla mattis enim. Proin sapien enim, congue non, aliquet et, sollicitudin nec, mauris. Sed porta. 

Curabitur luctus mollis massa. Nullam consectetur mollis lacus. Suspendisse turpis. Fusce velit. Morbi egestas dui. Donec commodo ornare lorem. Vestibulum sodales. Curabitur egestas libero ut metus. Sed eget orci a ligula consectetur vestibulum. Cras sapien. 

Sed libero. Ut volutpat massa. Donec nulla pede, porttitor eu, sodales et, consectetur nec, quam. Pellentesque vestibulum hendrerit est. Nulla facilisi. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Duis et nibh eu lacus iaculis pretium. Fusce sed turpis. In cursus. Etiam interdum augue. Morbi commodo auctor ligula. In imperdiet, neque nec hendrerit consequat, lacus purus tristique turpis, eu hendrerit ipsum ligula at libero. Duis varius nunc vel tortor. Praesent tempor. Nunc non pede at velit congue feugiat. Curabitur gravida, nisl quis mattis porttitor, purus nulla viverra dui, non suscipit augue nunc ac libero. Donec lacinia est non augue. 

Nulla quam dui, tristique id, condimentum sed, sodales in, ante. Vestibulum vitae diam. Integer placerat ante non orci. Nulla gravida. Integer magna enim, iaculis ut, ornare dignissim, ultrices a, urna. Donec urna. Fusce fringilla, pede vitae pulvinar ullamcorper, est nisi eleifend ipsum, ac adipiscing odio massa vehicula neque. Sed blandit est. Morbi faucibus, nisl vel commodo vulputate, mi ipsum tincidunt sem, id ornare orci orci et velit. Morbi commodo sollicitudin ligula. Pellentesque vitae urna. Duis massa arcu, accumsan id, euismod eu, tincidunt et, odio. Phasellus purus leo, rhoncus sed, condimentum nec, vestibulum vel, lacus. In egestas, lectus vitae lacinia tristique, elit magna consequat risus, id sodales metus nulla ac pede. Suspendisse potenti. 

Fusce massa. Nullam lacinia purus nec ipsum. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Suspendisse potenti. Proin augue. Donec mi magna, interdum a, elementum quis, bibendum sit amet, felis. Donec vel libero eget magna hendrerit ultrices. Suspendisse potenti. Sed scelerisque lacinia nisi. Quisque elementum, nunc nec luctus iaculis, ante quam aliquet orci, et ullamcorper dui ipsum at mi. Vestibulum a dolor id tortor posuere elementum. Sed mauris nisl, ultrices a, malesuada non, convallis ac, velit. Sed aliquam elit id metus. Donec malesuada, lorem ut pharetra auctor, mi risus viverra enim, vitae pulvinar urna metus at lorem. Vivamus id lorem. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Nulla facilisi. Ut vel odio. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Pellentesque lobortis sem. 

Proin in odio. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Vivamus bibendum arcu nec risus. Nulla iaculis ligula in purus. Etiam vulputate nibh sit amet lectus. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Suspendisse potenti. Suspendisse eleifend. Donec blandit nibh hendrerit turpis. Integer accumsan posuere odio. Ut commodo augue malesuada risus. Curabitur augue. Praesent volutpat nunc a diam. Nulla lobortis interdum dolor. Nunc imperdiet, ipsum ac tempor iaculis, nunc. 
";

        [Test]
        public void Remove()
        {
            string text = p.Tagger(shortText + stub, "Test", out noChange, ref summary, false, true);
            //Stub, tag not removed
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text));

            text = p.Tagger(longText + stub, "Test", out noChange, ref summary, false, true);
            //stub tag removed
            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text));

            text = p.Tagger("{{wikify}}" + Regex.Replace(longText, @"(\w+)", "[[$1]]"), "Test", out noChange, ref summary, false, true);
            //wikify tag removed
            Assert.IsFalse(WikiRegexes.Wikify.IsMatch(text));


            Globals.UnitTestIntValue = 4;
            text = p.Tagger("{{uncategorised}}", "Test", out noChange, ref summary, false, true);
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text));

            text = p.Tagger("{{uncategorised|date=January 2009}}", "Test", out noChange, ref summary, false, true);
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text));

            text = p.Tagger("{{uncategorised|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}", "Test", out noChange, ref summary, false, true);
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text));
            Assert.IsTrue(string.IsNullOrEmpty(text));

            Globals.UnitTestBoolValue = false;

            text = p.Tagger("{{orphan}}", "Test", out noChange, ref summary, false, true);
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));

            Globals.UnitTestBoolValue = true;

            text = p.Tagger("{{orphan}}", "Test", out noChange, ref summary, false, true);
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text));
        }

        [Test]
        public void Update()
        {
            //Test of updating some of the non dated tags
            string text = p.Tagger("{{fact}}", "Test", out noChange, ref summary, false, false);

            Assert.IsTrue(text.Contains("{{Fact|date={{subst:CURRENTMONTHNAME}}"));
            Assert.IsFalse(text.Contains("{{fact}}"));

            text = p.Tagger("{{template:fact}}", "Test", out noChange, ref summary, false, false);

            Assert.IsTrue(text.Contains("{{Fact|date={{subst:CURRENTMONTHNAME}}"));
            Assert.IsFalse(text.Contains("{{fact}}"));
        }

        [Test]
        public void General()
        {
            Assert.AreEqual("#REDIRECT [[Test]]", p.Tagger("#REDIRECT [[Test]]", "Test", out noChange, ref summary, true, true));
            Assert.IsTrue(noChange);

            Assert.AreEqual(shortText, p.Tagger(shortText, "Talk:Test", out noChange, ref summary, true, true));
            Assert.IsTrue(noChange);

            //No change as no add/remove
            Assert.AreEqual(shortText, p.Tagger(shortText, "Test", out noChange, ref summary, false, false));
            Assert.IsTrue(noChange);

            Assert.AreEqual("{{Test Template}}", p.Tagger("{{Test Template}}", "Test", out noChange, ref summary, true, true));
            Assert.IsTrue(noChange);
        }
    }
}
