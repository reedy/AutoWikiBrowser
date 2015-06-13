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

using System.Text.RegularExpressions;
using NUnit.Framework;
using WikiFunctions;
using WikiFunctions.Parse;
using System.Collections.Generic;

namespace UnitTests
{
    [TestFixture]
    public class FixSyntaxTests : RequiresParser
    {
        public GenfixesTestsBase genFixes  = new GenfixesTestsBase();

        [Test]
        public void FixISBNFormat()
        {
            Assert.AreEqual(@"ISBN 1245781549", Parsers.FixSyntax(@"ISBN: 1245781549"), "removes colon after ISBN");
            Assert.AreEqual(@"ISBN 1245781549", Parsers.FixSyntax(@"[[ISBN]] 1245781549"), "removes wikilink around ISBN");
            Assert.AreEqual(@"ISBN 1245781549", Parsers.FixSyntax(@"ISBN-10: 1245781549"), "removes colon after ISBN");
            Assert.AreEqual(@"ISBN 9781245781549", Parsers.FixSyntax(@"ISBN-13: 9781245781549"), "removes colon after ISBN");
            Assert.AreEqual(@"ISBN 1245781549", Parsers.FixSyntax(@"ISBN 1245781549"), "no change if already correct");
            Assert.AreEqual(@"ISBN 1245781549", Parsers.FixSyntax(@"ISBN:1245781549"), "removes colon after ISBN");
            Assert.AreEqual(@"ISBN 1245781549", Parsers.FixSyntax(@"ISBN	1245781549"), "ISBN with tab");

            Assert.AreEqual(@"ISBN 1245781549", Parsers.FixSyntax(@"ISBN-1245781549"), "removes minus after ISBN");
            Assert.AreEqual(@"ISBN 1045781549", Parsers.FixSyntax(@"ISBN-1045781549"), "removes minus after ISBN");
            Assert.AreEqual(@"ISBN-10 12345781549", Parsers.FixSyntax(@"ISBN-10 12345781549"), "do nothing");
            Assert.AreEqual(@"ISBN-13 12345781549", Parsers.FixSyntax(@"ISBN-13 12345781549"), "do nothing");

            //{{ISBN-10}} and {{ISBN-13}} have been deleted
            //Assert.AreEqual(@"{{ISBN-10|1245781549}}", Parsers.FixSyntax(@"{{ISBN-10|1245781549}}"), "no change if already correct – ISBN-10 template");
            //Assert.AreEqual(@"{{ISBN-13|9781245781549}}", Parsers.FixSyntax(@"{{ISBN-13|9781245781549}}"), "no change if already correct – ISBN-13 template");

            Assert.AreEqual(@"[http://www.hup.harvard.edu/catalog.php?isbn=9780674372993 example]", Parsers.FixSyntax(@"[http://www.hup.harvard.edu/catalog.php?isbn=9780674372993 example]"), "no change inside url");
            Assert.AreEqual(@"foo<ref name=""isbn0-19-517234-5"" />", Parsers.FixSyntax(@"foo<ref name=""isbn0-19-517234-5"" />"), "no change inside ref");

            Assert.AreEqual(@"ISBN 9711005522", Parsers.FixSyntax(@"[[ISBN]] [[Special:BookSources/9711005522|9711005522]]"), "ISBNT substed");
            Assert.AreEqual(@"ISBN 9780881925166", Parsers.FixSyntax(@"[[ISBN]] [[Special:BookSources/9780881925166|9780881925166]]"), "ISBNT substed");
        }

        [Test]
        public void FixPMIDFormat()
        {
            Assert.AreEqual(@"PMID 1245781549", Parsers.FixSyntax(@"PMID: 1245781549"), "removes colon after PMID");
            Assert.AreEqual(@"PMID 1245781549", Parsers.FixSyntax(@"PMID:1245781549"), "removes colon after PMID");
            Assert.AreEqual(@"PMID 1245781549", Parsers.FixSyntax(@"PMID:    1245781549"), "removes colon after PMID");
            Assert.AreEqual(@"PMID 1245781549", Parsers.FixSyntax(@"PMID 1245781549"), "No change if alrady correct");
        }

        [Test]
        public void FixHtmlTagsSyntax()
        {
            const string corr = @"Foo<small>bar</small> was the 1<sub>st</sub> to drink H<sup>2</sup>O";
            Assert.AreEqual(corr, Parsers.FixSyntax(@"Foo<small>bar<small/> was the 1<sub>st<sub/> to drink H<sup>2<sup/>O"));
            Assert.AreEqual(corr, Parsers.FixSyntax(@"Foo<small>bar<small/> was the 1<sub>st</sub/> to drink H<sup>2</sup/>O"));
            Assert.AreEqual(corr, Parsers.FixSyntax(corr));
 
            Assert.AreEqual(@"<center>Centered text</center>", Parsers.FixSyntax(@"<center>Centered text<center/>"));
        }

        [Test]
        public void FixSyntaxHorizontalRule()
        {
            Assert.AreEqual(@"----", Parsers.FixSyntax(@"<hr>"));
            Assert.AreEqual(@"----", Parsers.FixSyntax(@"-----"));
            Assert.AreEqual(@"A
----
B", Parsers.FixSyntax(@"A
<hr>
B"));
            string Nochange = @"A<hr>";
            Assert.AreEqual(Nochange, Parsers.FixSyntax(Nochange));
            Nochange = @"A----";
            Assert.AreEqual(Nochange, Parsers.FixSyntax(Nochange));
        }

        [Test]
        public void FixSyntaxRedirects()
        {
            Assert.AreEqual(@"#REDIRECT [[Foo]]", Parsers.FixSyntaxRedirects(@"#REDIRECT
[[Foo]]"),"newline");
            Assert.AreEqual(@"#REDIRECT [[Foo]]", Parsers.FixSyntaxRedirects(@"#REDIRECT [[[Foo]]]"),"extra opening/closing bracket");
            Assert.AreEqual(@"#REDIRECT [[Foo]]", Parsers.FixSyntaxRedirects(@"#REDIRECT [[Foo]]]"),"one extra closing bracket");
            Assert.AreEqual(@"#REDIRECT [[Foo]]", Parsers.FixSyntaxRedirects(@"#REDIRECT [[[Foo]]"),"one extra opening bracket");
            Assert.AreEqual(@"#REDIRECT [[Foo]]", Parsers.FixSyntaxRedirects(@"#REDIRECT [[[[Foo]]"),"two extra opening brackets");
            Assert.AreEqual(@"#REDIRECT [[Foo]]", Parsers.FixSyntaxRedirects(@"#REDIRECT [[Foo]]]]"),"two extra closing brackets");
            Assert.AreEqual(@"#REDIRECT [[Foo]]", Parsers.FixSyntaxRedirects(@"#REDIRECT [[[[Foo]]"),"two extra opening/closing brackets");
            Assert.AreEqual(@"#REDIRECT [[Foo]]", Parsers.FixSyntaxRedirects(@"#REDIRECT
[[[Foo]]]"),"extra brackets and newline");
            Assert.AreEqual(@"#REDIRECT [[Foo]]", Parsers.FixSyntaxRedirects(@"#REDIRECT:[[Foo]]"),"double dot unspaced");
            Assert.AreEqual(@"#REDIRECT [[Foo]]", Parsers.FixSyntaxRedirects(@"#REDIRECT: [[Foo]]"),"double dot with space");
            Assert.AreEqual(@"#REDIRECT [[Foo]]", Parsers.FixSyntaxRedirects(@"#REDIRECT=[[Foo]]"),"equal sign unspaced");
            Assert.AreEqual(@"#REDIRECT [[Foo]]", Parsers.FixSyntaxRedirects(@"#REDIRECT= [[Foo]]"),"equal sihn with space");
            Assert.AreEqual(@"#REDIRECT [[Foo]]", Parsers.FixSyntaxRedirects(@"#REDIRECT=
[[[Foo]]]"),"extra brackets, equal sign and newline");

            Assert.AreEqual(@"#REDIRECT[[Foo]] {{R from move}}", Parsers.FixSyntaxRedirects(@"#REDIRECT[[Foo]] {{Template:R from move}}"),"template prefix");

        }

        [Test]
        public void ExternalLinksNewline()
        {
            Assert.AreEqual(@"here [http://www.site.com text here]", Parsers.FixSyntax(@"here [http://www.site.com text
here]"), "newline removed");
            Assert.AreEqual(@"here [http://www.site.com text here2 ]", Parsers.FixSyntax(@"here [http://www.site.com text here2
]"), "newline removed2");
            Assert.AreEqual(@"here [http://www.site.com text here there]", Parsers.FixSyntax(@"here [http://www.site.com text
here
there]"), "multiple newlines removed");
            Assert.AreEqual(@"here [http://www.site.com text here]", Parsers.FixSyntax(@"here [http://www.site.com |text
here]"), "newline removed3");

            Assert.AreEqual(@"here [http://www.site.com text here]", Parsers.FixSyntax(@"here [http://www.site.com text here]"), "no change if no new line");
        }

        [Test]
        public void UnbalancedBrackets()
        {
            int bracketLength = 0;
            Assert.AreEqual(18, Parsers.UnbalancedBrackets(@"now hello {{bye}} {{now}", out bracketLength));
            Assert.AreEqual(2, bracketLength);
            Assert.AreEqual(18, Parsers.UnbalancedBrackets(@"now hello {{bye}} {{now} abc", out bracketLength));
            Assert.AreEqual(2, bracketLength);
            Assert.AreEqual(18, Parsers.UnbalancedBrackets(@"now hello {{bye}} {{now} abc {{def}}", out bracketLength));
            Assert.AreEqual(2, bracketLength);
            Assert.AreEqual(18, Parsers.UnbalancedBrackets(@"now hello {{bye}} [[now]", out bracketLength));
            Assert.AreEqual(2, bracketLength);
            Assert.AreEqual(18, Parsers.UnbalancedBrackets(@"now hello [[bye]] {{now}", out bracketLength));
            Assert.AreEqual(2, bracketLength);
            Assert.AreEqual(18, Parsers.UnbalancedBrackets(@"now hello {{bye}} [now words [here]", out bracketLength));
            Assert.AreEqual(1, bracketLength);
            Assert.AreEqual(21, Parsers.UnbalancedBrackets(@"now hello {{bye}} now] words [here]", out bracketLength));
            Assert.AreEqual(1, bracketLength);
            Assert.AreEqual(22, Parsers.UnbalancedBrackets(@"now hello {{bye}} {now}}", out bracketLength));
            Assert.AreEqual(2, bracketLength);
            Assert.AreEqual(33, Parsers.UnbalancedBrackets(@"[http://www.site.com a link [cool]]", out bracketLength)); // FixSyntax replaces with &#93;
            Assert.AreEqual(2, bracketLength);
            Assert.AreEqual(18, Parsers.UnbalancedBrackets(@"now hello {{bye}} {now", out bracketLength));
            Assert.AreEqual(1, bracketLength);
            Assert.AreEqual(18, Parsers.UnbalancedBrackets(@"now hello {abyea} {now", out bracketLength));
            Assert.AreEqual(1, bracketLength);
            Assert.AreEqual(18, Parsers.UnbalancedBrackets(@"now hello {{bye}} {now {{here}} a", out bracketLength));
            Assert.AreEqual(1, bracketLength);
            Assert.AreEqual(18, Parsers.UnbalancedBrackets(@"now hello {{bye}} {now {here} a {{a}}", out bracketLength));
            Assert.AreEqual(1, bracketLength);
            Assert.AreEqual(36, Parsers.UnbalancedBrackets(@"now hello {{bye}} now {here} a {{a}}}", out bracketLength));
            Assert.AreEqual(1, bracketLength);
            Assert.AreEqual(0, Parsers.UnbalancedBrackets(@"{bye", out bracketLength));
            Assert.AreEqual(1, bracketLength);
            Assert.AreEqual(0, Parsers.UnbalancedBrackets(@"<bye", out bracketLength));
            Assert.AreEqual(1, bracketLength);
            Assert.AreEqual(36, Parsers.UnbalancedBrackets(@"now hello [words [here&#93; end] now]", out bracketLength));
            Assert.AreEqual(1, bracketLength);

            // only first reported
            Assert.AreEqual(18, Parsers.UnbalancedBrackets(@"now hello {{bye}} {{now} or {{now} was", out bracketLength));
            Assert.AreEqual(18, Parsers.UnbalancedBrackets(@"now hello {{bye}} {{now} or [[now] was", out bracketLength));

            Assert.AreEqual(115, Parsers.UnbalancedBrackets(@"==External links==
*[http://www.transfermarkt.de/profil.html]&section=p&teamid=458 Profile] at Transfermarkt.de
*[http://www.vi.nl/Spelers
", out bracketLength));
            Assert.AreEqual(1, bracketLength);
            
            Assert.AreEqual(0, Parsers.UnbalancedBrackets(@"{{Infobox|foo=bar (OMG} }}", out bracketLength));

            Assert.AreEqual(4, Parsers.UnbalancedBrackets(@"now [[link],] at", out bracketLength));
            Assert.AreEqual(2, bracketLength);
        }

        [Test]
        public void UnbalancedBracketsNone()
        {
            int bracketLength = 0;
            // brackets all okay
            Assert.AreEqual(-1, Parsers.UnbalancedBrackets(@"now hello {{bye}} {{now}}", out bracketLength));
            Assert.AreEqual(-1, Parsers.UnbalancedBrackets(@"now hello [[bye]] {{now}}", out bracketLength));
            Assert.AreEqual(-1, Parsers.UnbalancedBrackets(@"now hello {{bye}} [now]", out bracketLength));
            Assert.AreEqual(-1, Parsers.UnbalancedBrackets(@"now hello", out bracketLength));
            Assert.AreEqual(-1, Parsers.UnbalancedBrackets(@"<ref>[http://www.pubmedcentral.nih.gov/articlerender.fcgi?artid=32159 Message to
complementary and alternative medicine: evidence is a better friend than power. Andrew J Vickers]</ref>", out bracketLength));
            Assert.AreEqual(-1, Parsers.UnbalancedBrackets(@"[http://www.site.com a link [cool&#93;]", out bracketLength)); // displays as valid syntax
            Assert.AreEqual(-1, Parsers.UnbalancedBrackets(@"[http://www.site.com a link &#91;cool&#93; here]", out bracketLength)); // displays as valid syntax
            Assert.AreEqual(-1, Parsers.UnbalancedBrackets(@"*[http://external.oneonta.edu/html A&#91;lbert&#93; T. 1763]", out bracketLength));

            // don't consider stuff in <math> or <pre> tags etc.
            Assert.AreEqual(-1, Parsers.UnbalancedBrackets(@"now hello {{bye}} <pre>{now}}</pre>", out bracketLength));
            Assert.AreEqual(-1, Parsers.UnbalancedBrackets(@"now hello {{bye}} <math>{a{b}}</math>", out bracketLength));
            Assert.AreEqual(-1, Parsers.UnbalancedBrackets(@"now hello {{bye}} <code>{now}}</code>", out bracketLength));
            // ignore in certain templates
            Assert.AreEqual(-1, Parsers.UnbalancedBrackets(@"now hello {{LSJ|foo(bar}}", out bracketLength));
            Assert.AreEqual(-1, Parsers.UnbalancedBrackets(@"now hello {{)!}}", out bracketLength));
            Assert.AreEqual(-1, Parsers.UnbalancedBrackets(@"now hello {{!(}}", out bracketLength));
        }

        [Test]
        public void UnbalancedTags()
        {
            int bracketLength = 0;
            // unbalanced tags
            Assert.AreEqual(15, Parsers.UnbalancedBrackets(@"now <b>hello /b>", out bracketLength));
            Assert.AreEqual(1, bracketLength);

            Assert.AreEqual(27, Parsers.UnbalancedBrackets(@"<a>asdf</a> now <b>hello /b>", out bracketLength));
            Assert.AreEqual(1, bracketLength);

            // not unbalanced
            Assert.AreEqual(-1, Parsers.UnbalancedBrackets(@"now was < 50 cm long", out bracketLength));
        }

        [Test]
        public void TestWordingIntoBareExternalLinks()
        {
            Assert.AreEqual(@"<ref>[http://www.nps.gov/history/nr/travel/cumberland/ber.htm B'er Chayim Temple, National Park Service]</ref>", Parsers.FixSyntax(@"<ref>B'er Chayim Temple, National Park Service, [ http://www.nps.gov/history/nr/travel/cumberland/ber.htm]</ref>"));

            // don't catch two bare links
            Assert.AreEqual(@"<ref>[http://www.nps.gov/history] [http://www.nps.gov/history/nr/travel/cumberland/ber.htm]</ref>", Parsers.FixSyntax(@"<ref>[http://www.nps.gov/history] [http://www.nps.gov/history/nr/travel/cumberland/ber.htm]</ref>"));
        }

        private static readonly System.Globalization.CultureInfo BritishEnglish = new System.Globalization.CultureInfo("en-GB");

        [Test]
        public void FixSyntaxSubstRefTags()
        {
            Assert.IsFalse(Parsers.FixSyntax(@"<ref>{{cite web | title=foo| url=http://www.site.com }} {{dead link|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}</ref>").Contains(@"{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}"), "subst converted within ref tags");
            Assert.AreEqual(@"<ref>{{cite web | title=foo| url=http://www.site.com }} {{dead link|date=" + System.DateTime.UtcNow.ToString("MMMM yyyy", BritishEnglish) + @"}}</ref>", Parsers.FixSyntax(@"<ref>{{cite web | title=foo| url=http://www.site.com }} {{dead link|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}</ref>"));
            Assert.IsTrue(Parsers.FixSyntax(@"* {{cite web | title=foo| url=http://www.site.com }} {{dead link|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}").Contains(@"{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}"), "subst not converted when outside ref tags");

            Assert.AreEqual(@"<gallery>
 Foo.JPG |Foo great{{citation needed|date=" + System.DateTime.UtcNow.ToString("MMMM yyyy", BritishEnglish) + @"}}
</gallery>", Parsers.FixSyntax(@"<gallery>
 Foo.JPG |Foo great{{citation needed|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}
</gallery>"), "subst converted within gallery tags");
        }

        [Test]
        public void FixSyntaxTemplateNamespace()
        {
            Assert.AreEqual(@"{{foo}}", Parsers.FixSyntax(@"{{Template:foo}}"));
            Assert.AreEqual(@"{{ foo}}", Parsers.FixSyntax(@"{{ Template:foo}}"));
            Assert.AreEqual(@"{{ foo}}", Parsers.FixSyntax(@"{{ template :foo}}"));
            Assert.AreEqual(@"{{
foo}}", Parsers.FixSyntax(@"{{
Template:foo}}"));
            Assert.AreEqual(@"{{foo
|bar=yes}}", Parsers.FixSyntax(@"{{Template:foo
|bar=yes}}"));
        }

        [Test]
        public void FixSyntaxExternalLinkBrackets()
        {
            Assert.AreEqual("[http://example.com] site", Parsers.FixSyntax("[http://example.com] site"));
            Assert.AreEqual("[http://example.com] site2", Parsers.FixSyntax("[[http://example.com] site2"));
            Assert.AreEqual("[http://example.com] site3", Parsers.FixSyntax("[[[http://example.com] site3"));
            Assert.AreEqual("[http://example.com] site4", Parsers.FixSyntax("[http://example.com]] site4"));
            Assert.AreEqual("[http://example.com] site5", Parsers.FixSyntax("[[http://example.com]] site5"));
            Assert.AreEqual("[http://example.com] site6", Parsers.FixSyntax("[[[http://example.com]]] site6"));
            Assert.AreEqual("[http://example.com] site7", Parsers.FixSyntax(Parsers.FixLinkWhitespace("[[ http://example.com]] site7", "Test")));
            Assert.AreEqual(@"[http://example.com]
* List 2", Parsers.FixSyntax(@"[[http://example.com
* List 2"));

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_10#second_pair_of_brackets_added_to_https_links
            Assert.AreEqual("[https://example.com] site", Parsers.FixSyntax("[https://example.com]] site"));
            Assert.AreEqual("[https://example.com] site", Parsers.FixSyntax("[[https://example.com] site"));

            Assert.AreEqual("<ref>[http://test.com]</ref>", Parsers.FixSyntax("<ref>[http://test.com}</ref>"));
            Assert.AreEqual("<ref>[http://test.com]</ref>", Parsers.FixSyntax("<ref>[http://test.com</ref>"));

            Assert.AreEqual("<ref>[http://test.com foo bar]</ref>", Parsers.FixSyntax("<ref>[http://test.com foo" + "\r\n" +
                                                                                      @"bar</ref>"));

            Assert.AreEqual("{{cite web | url = http://test.com |title=a }}", Parsers.FixSyntax("{{cite web | url = http:http://test.com |title=a }}"));
            Assert.AreEqual(@"* [http://www.site.com text]", Parsers.FixSyntax(@"* [http://www.site.com text)"));
            Assert.AreEqual(@"* [http://www.site.com text]", Parsers.FixSyntax(@"* [http://www.site.com text) "));

            const string Correct = @"* [http://www.site.com text (here) there]";
            Assert.AreEqual(Correct, Parsers.FixSyntax(Correct));
        }

        [Test]
        public void FixSyntaxExternalLinks()
        {
            Assert.AreEqual("[http://test.com]", Parsers.FixSyntax("[http://test.com]"),"do nothing if everything is OK");
            Assert.AreEqual("[http://test.com]", Parsers.FixSyntax("[http://http://test.com]"),"double http");
            Assert.AreEqual("[http://test.com]", Parsers.FixSyntax("[http:// http://test.com]"),"double http");
            Assert.AreEqual("[http://test.com]", Parsers.FixSyntax("[http:http://test.com]"),"double http with first lacking slashes");
            Assert.AreEqual("[http://test.com]", Parsers.FixSyntax("[http://http://http://test.com]"),"more than two");
            Assert.AreEqual("[https://test.com]", Parsers.FixSyntax("[https://https://test.com]"),"double https");
            Assert.AreEqual("[ftp://test.com]", Parsers.FixSyntax("[ftp://ftp://test.com]"),"double ftp");
            Assert.AreEqual("[ftp://test.com]", Parsers.FixSyntax("[ftp://ftp://ftp://test.com]"),"triple ftp");

            Assert.AreEqual("{{url|http://test.com}}", Parsers.FixSyntax("{{url|http://http://test.com}}"),"double http inside url template");
            Assert.AreEqual("{{official website|http://test.com}}", Parsers.FixSyntax("{{official website|http://http://test.com}}"),"double http inside official website template");
            Assert.AreEqual("{{foo|http://http://test.com}}", Parsers.FixSyntax("{{foo|http://http://test.com}}"),"no change in a random template");

            Assert.AreEqual("[http://test.com/a.png]", Parsers.FixSyntax("[[Image:http://test.com/a.png]]"), "Image http 1");
            Assert.AreEqual("[http://test.com/a.png]", Parsers.FixSyntax("[Image:http://test.com/a.png]"), "Image http 2");
        }
 

        [Test]
        public void TestFixSyntaxRemoveEmptyGallery()
        {
            Assert.AreEqual("", Parsers.FixSyntax(@"<gallery></gallery>"));
            Assert.AreEqual("", Parsers.FixSyntax(@"<gallery>
</gallery>"));
            Assert.AreEqual("", Parsers.FixSyntax(@"<gallery>   </gallery>"));
            Assert.AreEqual("", Parsers.FixSyntax(@"< Gallery >   </gallery>"));
            Assert.AreEqual("", Parsers.FixSyntax(@"< GALLERY >   </gallery>"));

            const string Gallery = @"<gallery>Image1.jpeg</gallery>";

            Assert.AreEqual(Gallery, Parsers.FixSyntax(Gallery));
            Assert.AreEqual("", Parsers.FixSyntax(@"<gallery> <gallery><gallery>   </gallery></gallery></gallery>"), "Cleans nested empty gallery tags");
            Assert.AreEqual("", Parsers.FixSyntax(@"<center></center>"));
            Assert.AreEqual("", Parsers.FixSyntax(@"<gallery> <center></center> </gallery>"));
            Assert.AreEqual("", Parsers.FixSyntax(@"<blockquote></blockquote>"));
        }

        [Test]
        public void FixSyntaxHTMLTags()
        {
            Assert.AreEqual("'''foo''' bar", Parsers.FixSyntax("<b>foo</b> bar"));
            Assert.AreEqual("'''foo''' bar", Parsers.FixSyntax("<B>foo</B> bar"));
            Assert.AreEqual("'''foo''' bar", Parsers.FixSyntax("< b >foo</b> bar"));
            Assert.AreEqual("'''foo''' bar", Parsers.FixSyntax("<b>foo< /b > bar"));
            Assert.AreEqual("<b>foo<b> bar", Parsers.FixSyntax("<b>foo<b> bar"));
            Assert.AreEqual("'''foobar'''", Parsers.FixSyntax("<b>foo</b><b>bar</b>"));

            Assert.AreEqual("''foo'' bar", Parsers.FixSyntax("<i>foo</i> bar"));
            Assert.AreEqual("''foo'' bar", Parsers.FixSyntax("< i >foo</i> bar"));
            Assert.AreEqual("''foo'' bar", Parsers.FixSyntax("< i >foo< / i   > bar"));
            Assert.AreEqual("''foo'' bar", Parsers.FixSyntax("<i>foo< /i > bar"));
            Assert.AreEqual("''foo'' bar", Parsers.FixSyntax("<i>foo<i /> bar"));
            Assert.AreEqual("''foo'' bar", Parsers.FixSyntax(@"<i>foo<i\> bar"));
            Assert.AreEqual("<i>foo<i> bar", Parsers.FixSyntax("<i>foo<i> bar"));
            Assert.AreEqual("''foo'' bar", Parsers.FixSyntax("<em>foo</em> bar"));
            Assert.AreEqual("''foobar''", Parsers.FixSyntax("<i>foo</i><i>bar</i>"));
            Assert.AreEqual("''foobar''", Parsers.FixSyntax("<i>foo<i/><i>bar</i>"));
            Assert.AreEqual("'''''foo''''' bar", Parsers.FixSyntax("<b><i>foo</i></b> bar"));

            const string EmTemplate = @"{{em|foo}}";
            Assert.AreEqual(EmTemplate, Parsers.FixSyntax(EmTemplate));
        }

        [Test]
        public void FixSyntaxStrike()
        {
            Assert.AreEqual("<s>hello</s>", Parsers.FixSyntax(@"<strike>hello</strike>"));
        }

        [Test]
        public void ExtraBracketInExternalLink()
        {
            //https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_9#Bug_in_regex_to_correct_double_bracketed_external_links
            const string valid = "now [http://www.site.com a [[a]] site] was";
            Assert.AreEqual(valid, Parsers.FixSyntax(valid));  // valid syntax
            Assert.AreEqual("now [http://www.site.com a b site] was", Parsers.FixSyntax("now [http://www.site.com a b site]] was"));
            Assert.AreEqual("now [http://www.site.com a c site] was", Parsers.FixSyntax("now [[http://www.site.com a c site] was"));
            Assert.AreEqual("now [http://www.site.com a d3 site] was", Parsers.FixSyntax("now [[http://www.site.com a d3 site]] was"));
            Assert.AreEqual("now [[Image:Fred1211212.JPG| here [http://www.site.com a [[e]] site]]] was", Parsers.FixSyntax("now [[Image:Fred1211212.JPG| here [http://www.site.com a [[e]] site]]] was"));   // valid wiki syntax
            Assert.AreEqual("now [[Image:Fred12.JPG| here [http://www.site.com a g site]]] was", Parsers.FixSyntax("now [[Image:Fred12.JPG| here [[http://www.site.com a g site]]] was"));
            Assert.AreEqual("now [http://www.site.com a [[b]] site] was", Parsers.FixSyntax("now [http://www.site.com a [[b]] site]] was"));
            Assert.AreEqual("now [http://www.site.com a [[c]] site] was", Parsers.FixSyntax("now [[http://www.site.com a [[c]] site] was"));
            Assert.AreEqual("now [http://www.site.com a [[d]] or [[d2]] site] was", Parsers.FixSyntax("now [[http://www.site.com a [[d]] or [[d2]] site]] was"));
            Assert.AreEqual("now [[Image:Fred12.JPG| here [http://www.site.com a [[f]] site]]] was", Parsers.FixSyntax("now [[Image:Fred12.JPG| here [http://www.site.com a [[f]] site]]]] was"));
            Assert.AreEqual("now [[Image:Fred12.JPG| here [http://www.site.com a g site]]] was", Parsers.FixSyntax("now [[Image:Fred12.JPG| here [http://www.site.com a g site]]]] was"));
            Assert.AreEqual("now [[Image:Fred12.JPG| here [http://www.site.com a g site]]] was", Parsers.FixSyntax("now [[Image:Fred12.JPG| here [[http://www.site.com a g site]]]] was"));
            Assert.AreEqual("[[Image:foo.jpg|Some [http://some_crap.com]]]", Parsers.FixSyntax("[[Image:foo.jpg|Some [[http://some_crap.com]]]]"));
            Assert.AreEqual("[[File:foo.jpg|Some [https://some_crap.com]]]", Parsers.FixSyntax("[[File:foo.jpg|Some [[https://some_crap.com]]]]"));
        }

        [Test]
        public void SquareBracketsInExternalLinksTests()
        {
            Assert.AreEqual(@"[http://www.site.com some stuff &#91;great&#93;]", Parsers.FixSyntax(@"[http://www.site.com some stuff [great]]"));
            Assert.AreEqual(@"[http://www.site.com some stuff &#91;great&#93; free]", Parsers.FixSyntax(@"[http://www.site.com some stuff [great] free]"));
            Assert.AreEqual(@"[http://www.site.com some stuff &#91;great&#93; free &#91;here&#93;]", Parsers.FixSyntax(@"[http://www.site.com some stuff [great] free [here]]"));
            Assert.AreEqual(@"[http://www.site.com some stuff &#91;great&#93; free [[now]]]", Parsers.FixSyntax(@"[http://www.site.com some stuff [great] free [[now]]]"));

            Assert.AreEqual(@"*This: [http://www.privacyinternational.org/article.shtml?cmd&#91;347&#93;=x-347-359656&als&#91;theme&#93;=AT+Law+and+Policy Terrorism Profile - Uganda] Privacy International",
                            Parsers.FixSyntax(@"*This: [http://www.privacyinternational.org/article.shtml?cmd[347]=x-347-359656&als[theme]=AT+Law+and+Policy Terrorism Profile - Uganda] Privacy International"));

            // no delinking needed
            Assert.AreEqual(@"[http://www.site.com some great stuff]", Parsers.FixSyntax(@"[http://www.site.com some great stuff]"));
            Assert.AreEqual(@"now [http://www.site.com some great stuff] here", Parsers.FixSyntax(@"now [http://www.site.com some great stuff] here"));
            Assert.AreEqual(@"[http://www.site.com some [[great]] stuff]", Parsers.FixSyntax(@"[http://www.site.com some [[great]] stuff]"));
            Assert.AreEqual(@"[http://www.site.com some great [[stuff]]]", Parsers.FixSyntax(@"[http://www.site.com some great [[stuff]]]"));
            Assert.AreEqual(@"[http://www.site.com [[some great stuff|loads of stuff]]]", Parsers.FixSyntax(@"[http://www.site.com [[some great stuff|loads of stuff]]]"));
            Assert.AreEqual(@"[http://www.site.com [[some great stuff|loads of stuff]] here]", Parsers.FixSyntax(@"[http://www.site.com [[some great stuff|loads of stuff]] here]"));

            // don't match beyond </ref> tags
            Assert.AreEqual(@"[http://www.site.com some </ref> [stuff] here]", Parsers.FixSyntax(@"[http://www.site.com some </ref> [stuff] here]"));

            // exception handling
            Assert.AreEqual(@"* [http://www.site.com some stuff
* [http://site2.com stuff 2]
Other
]", Parsers.FixSyntax(@"* [http://www.site.com some stuff
* [http://site2.com stuff 2]
Other
]"));
        }


	}
}