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
        public void FixSyntaxRedirectsBrackets()
        {
            Assert.AreEqual(@"#REDIRECT[[Foo]] {{R from move}}", Parsers.FixSyntaxRedirects(@"#REDIRECT[[Foo]] {{Template:R from move"), "Missing }}");
            Assert.AreEqual(@"#REDIRECT[[Foo]] {{R from move}}", Parsers.FixSyntaxRedirects(@"#REDIRECT[[Foo]] {{Template:R from move" + "\r\n"), "Missing }} with newline");
            Assert.AreEqual(@"#REDIRECT[[Foo]] {{R from move}}", Parsers.FixSyntaxRedirects(@"#REDIRECT[[Foo]] {{R from move}"), "Missing }");
            Assert.AreEqual(@"#REDIRECT[[Foo]] {{R from move}}", Parsers.FixSyntaxRedirects(@"#REDIRECT[[Foo]] {{R from move}]"), "Has }]");
            Assert.AreEqual(@"#REDIRECT[[Foo]] {{R from move}}", Parsers.FixSyntaxRedirects(@"#REDIRECT[[Foo]] {{R from move]}"), "Has ]}");
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
		public void TestFixSyntaxDisambigCat()
		{
			Assert.AreEqual(@"Foo {{Disambiguation}}", Parsers.FixSyntax(@"Foo [[Category:Disambiguation pages]]"),"do nothing if everything is OK");
		}

        [Test]
        public void TestFixSyntaxRemoveEmptyTags()
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
            Assert.AreEqual("", Parsers.FixSyntax(@"<sup></sup>"));
            Assert.AreEqual("", Parsers.FixSyntax(@"<sub></sub>"));
            Assert.AreEqual(@"[[hello]] and [[bye]]", Parsers.FixSyntax(@"[[hello]] <sub></sub>and<sub></sub> [[bye]]<sub></sub>"));
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
        public void  CiteTemplateWithSquareBracketsTests()
        {
        	Assert.AreEqual(@"<ref name=Test>{{cite web|url=http://foo.com|accessdate=2015-07-07}}</ref>", Parsers.FixSyntax(@"<ref name=Test>[[cite web|url=http://foo.com|accessdate=2015-07-07]]</ref>"));
        	Assert.AreEqual(@"<ref>[[cite web | url = http://collection.whitney.org/object/11823 | accessdate: 25 June 2015 | publisher = [[Whitney Museum of American Art]] | title = Houston, Texas, 1977 from Women are Better than Men]].</ref>", Parsers.FixSyntax(@"<ref>[[cite web | url = http://collection.whitney.org/object/11823 | accessdate: 25 June 2015 | publisher = [[Whitney Museum of American Art]] | title = Houston, Texas, 1977 from Women are Better than Men]].</ref>"));
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

        [Test]
        public void FixDeadlinkOutsideRef()
        {
        	Assert.AreEqual("<ref>foo {{dead link|date=July 2014}}</ref> boo", Parsers.FixSyntax(@"<ref>foo</ref> {{dead link|date=July 2014}} boo"), "only {{dead link}} taken inside ref");
        	Assert.AreEqual("<ref>foo {{Dead link | date=July 2014 }}</ref> boo", Parsers.FixSyntax(@"<ref>foo</ref> {{Dead link | date=July 2014 }} boo"), "only {{dead link}} taken inside ref");
        	Assert.AreEqual("<ref>foo {{Dead link|date=July 2014}}</ref> boo", Parsers.FixSyntax(@"<ref>foo</ref> {{Dead link|date=July 2014}} boo"), "only {{dead link}} taken inside ref");        	Assert.AreEqual("<ref>{{cite web | url=http://www.site.com/article100.html | title=Foo }} {{dead link|date=July 2014}}</ref>", Parsers.FixSyntax(@"<ref>{{cite web | url=http://www.site.com/article100.html | title=Foo }}</ref> {{dead link|date=July 2014}}"), "{{dead link}} taken inside ref");
        }

        [Test]
        public void FixImagesBr()
        {
            Assert.AreEqual(@"[[File:Foo.png|description]]", Parsers.FixSyntax(@"[[File:Foo.png|description<br>]]"));
            Assert.AreEqual(@"[[File:Foo.png|description]]", Parsers.FixSyntax(@"[[File:Foo.png|description<br >]]"));
            Assert.AreEqual(@"[[File:Foo.png|description]]", Parsers.FixSyntax(@"[[File:Foo.png|description<br/>]]"));
            Assert.AreEqual(@"[[File:Foo.png|description]]", Parsers.FixSyntax(@"[[File:Foo.png|description<BR>]]"));
            Assert.AreEqual(@"[[File:Foo.png|description]]", Parsers.FixSyntax(@"[[File:Foo.png|description<br />]]"));
            Assert.AreEqual(@"[[File:Foo.png|description]]", Parsers.FixSyntax(@"[[File:Foo.png|description<br />
]]"));

            const string nochange1 = @"[[File:Foo.png|description<br>here]]";
            Assert.AreEqual(nochange1, Parsers.FixSyntax(nochange1));
        }

        [Test]
        public void FixSyntaxDEFAULTSORT()
        {
            Assert.AreEqual(@"{{DEFAULTSORT:Foo}}", Parsers.FixSyntax(@"{{DEFAULTSORT:
Foo}}"));

            Assert.AreEqual(@"{{DEFAULTSORT:Foo}}", Parsers.FixSyntax(@"{{DEFAULTSORT: Foo }}"));
            Assert.AreEqual(@"{{DEFAULTSORT:Foo}}", Parsers.FixSyntax(@"{{DEFAULTSORT:Foo
}}"));
        }
        
        [Test]
        public void FixSyntaxFontTags()
        {
            Assert.AreEqual("hello", Parsers.FixSyntax(@"<font>hello</font>"));
            Assert.AreEqual("hello", Parsers.FixSyntax(@"<font>hello</FONT>"));
            Assert.AreEqual(@"hello
world", Parsers.FixSyntax(@"<font>hello
world</font>"));

            // only changing font tags without properties
            Assert.AreEqual(@"<font name=ab>hello</font>", Parsers.FixSyntax(@"<font name=ab>hello</font>"));
        }
        
       [Test]
        public void FixSyntaxHTTPFormat()
        {
            Assert.AreEqual("<ref>http://www.site.com</ref>", Parsers.FixSyntax(@"<ref>http//www.site.com</ref>"),"missing colon");
            Assert.AreEqual("<ref>https://www.site.com</ref>", Parsers.FixSyntax(@"<ref>https//www.site.com</ref>"),"missing colon");
            Assert.AreEqual("<ref>http://www.site.com</ref>", Parsers.FixSyntax(@"<ref>http:://www.site.com</ref>"),"double colon");
            Assert.AreEqual("<ref>http://www.site.com</ref>", Parsers.FixSyntax(@"<ref>http:www.site.com</ref>"),"missing slashes");
            Assert.AreEqual("<ref>http://www.site.com</ref>", Parsers.FixSyntax(@"<ref>http:///www.site.com</ref>"),"triple slashes");
            Assert.AreEqual("<ref>http://www.site.com</ref>", Parsers.FixSyntax(@"<ref>http:////www.site.com</ref>"),"four slashes");
            Assert.AreEqual("at http://www.site.com", Parsers.FixSyntax(@"at http//www.site.com"));
            Assert.AreEqual("<ref>[http://www.site.com a website]</ref>",
                            Parsers.FixSyntax(@"<ref>[http:/www.site.com a website]</ref>"),"missing a slash");
            Assert.AreEqual("*[http://www.site.com a website]", Parsers.FixSyntax(@"*[http//www.site.com a website]"));
            Assert.AreEqual("|url=http://www.site.com", Parsers.FixSyntax(@"|url=http//www.site.com"));
            Assert.AreEqual("|url = http://www.site.com", Parsers.FixSyntax(@"|url = http:/www.site.com"));
            Assert.AreEqual("[http://www.site.com]", Parsers.FixSyntax(@"[http/www.site.com]"));

            // these strings should not change
            string bug1 = @"now http://members.bib-arch.org/nph-proxy.pl/000000A/http/www.basarchive.org/bswbSearch was";
            Assert.AreEqual(bug1, Parsers.FixSyntax(bug1));

            string bug2 = @"now http://sunsite.utk.edu/math_archives/.http/contests/ was";
            Assert.AreEqual(bug2, Parsers.FixSyntax(bug2));

            Assert.AreEqual("the HTTP/0.9 was", Parsers.FixSyntax("the HTTP/0.9 was"));
            Assert.AreEqual("the HTTP/1.0 was", Parsers.FixSyntax("the HTTP/1.0 was"));
            Assert.AreEqual("the HTTP/1.1 was", Parsers.FixSyntax("the HTTP/1.1 was"));
            Assert.AreEqual("the HTTP/1.2 was", Parsers.FixSyntax("the HTTP/1.2 was"));

            string a = @"the HTTP/FTP was";
            Assert.AreEqual(a, Parsers.FixSyntax(a));

            Assert.AreEqual("the HTTP/1.2 protocol", Parsers.FixSyntax("the HTTP/1.2 protocol"));
            Assert.AreEqual(@"<ref>[http://cdiac.esd.ornl.gov/ftp/cdiac74/a.pdf chapter 5]</ref>",
                            Parsers.FixSyntax(@"<ref>[http://cdiac.esd.ornl.gov/ftp/cdiac74/a.pdf chapter 5]</ref>"));
        }

        [Test]
        public void FixSyntaxMagicWords()
        {
            Assert.AreEqual(@"{{FULLPAGENAME:Foo}}", Parsers.FixSyntax(@"{{Fullpagename|Foo}}"));
        }

        [Test]
        public void FixSyntaxMagicWordsBehaviourSwitches()
        {
            Assert.AreEqual(@"__TOC__", Parsers.FixSyntax(@"__ToC__"));
            Assert.AreEqual(@"__TOC__", Parsers.FixSyntax(@"__toC__"));
            Assert.AreEqual(@"__TOC__", Parsers.FixSyntax(@"__TOC__"));
            Assert.AreEqual(@"__NOTOC__", Parsers.FixSyntax(@"__NoToC__"));
        }

        [Test]
        public void FixSyntaxPipesInExternalLinks()
        {
            Assert.AreEqual("[http://www.site.com ''my cool site'']", Parsers.FixSyntax("[http://www.site.com|''my cool site'']"));
            Assert.AreEqual("[http://www.site.com/here/there.html ''my cool site'']", Parsers.FixSyntax("[http://www.site.com/here/there.html|''my cool site'']"));

            Assert.AreEqual(@"port [http://www.atoc.org/general/ConnectingCommunitiesReport_S10.pdf ""Connecting Communities - Expanding Access to the Rail Network""] consid",
                            Parsers.FixSyntax(@"port [[http://www.atoc.org/general/ConnectingCommunitiesReport_S10.pdf |""Connecting Communities - Expanding Access to the Rail Network""]] consid"));

            const string nochange1 = @"[http://www.site.com|''my cool site''", nochange2 = @"{{Infobox Singapore School
| name = Yuan Ching Secondary School
| established = 1978
| city/town = [[Jurong]]
| enrolment = over 1,300
| homepage = [http://schools.moe.edu.sg/ycss/
| border_color = #330066
| uniform_color = #66CCFF
}}";
            Assert.AreEqual(nochange1, Parsers.FixSyntax(nochange1));
            Assert.AreEqual(nochange2, Parsers.FixSyntax(nochange2));
        }

        [Test]
        public void FixUnbalancedBracketsChineseBrackets()
        {
        	#if DEBUG
        	const string CB = @"now （there) was";

        	Variables.SetProjectLangCode("fr");
        	Assert.AreEqual(CB, Parsers.CiteTemplateDates(CB));

        	Variables.SetProjectLangCode("en");
        	Assert.AreEqual(@"now (there) was", Parsers.FixSyntax(CB));

        	const string CB2 = @"now （there） was";
        	Assert.AreEqual(CB2, Parsers.FixSyntax(CB2), "No change when brackets are balanced");
        	#endif
        }

        [Test]
        public void FixUnbalancedBracketsCiteTemplates()
        {
            Assert.AreEqual(@"<ref>{{cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref>{{cite web|url=a|title=b}</ref>"));
            Assert.AreEqual(@"<ref>{{cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref>{{cite web|url=a|title=b}]</ref>"));
            Assert.AreEqual(@"<ref>{{cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref>{{cite web|url=a|title=b))</ref>"));
            Assert.AreEqual(@"<ref> {{cite web|url=a|title=b}} </ref>", Parsers.FixSyntax(@"<ref> {{cite web|url=a|title=b} </ref>"));
            Assert.AreEqual(@"<ref name=Fred>{{cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref name=Fred>{{cite web|url=a|title=b}</ref>"));
            Assert.AreEqual(@"<ref name=""Fred"">{{cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref name=""Fred"">{{cite web|url=a|title=b}</ref>"));

            Assert.AreEqual(@"<ref>{{cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref>{cite web|url=a|title=b}}</ref>"));
            Assert.AreEqual(@"<ref> {{cite web|url=a|title=b}} </ref>", Parsers.FixSyntax(@"<ref> {cite web|url=a|title=b}} </ref>"));
            Assert.AreEqual(@"<ref name=Fred>{{Cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref name=Fred>{Cite web|url=a|title=b}}</ref>"));
            Assert.AreEqual(@"<ref name=""Fred"">{{cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref name=""Fred"">{cite web|url=a|title=b}}</ref>"));
            Assert.AreEqual(@"<ref>{{citation|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref>{citation|url=a|title=b}}</ref>"));

            Assert.AreEqual(@"<ref> {{Citation|title=}}", Parsers.FixSyntax(@"<ref> {Citation|title=}}"));
            Assert.AreEqual(@"<ref> {{cite web|title=}}", Parsers.FixSyntax(@"<ref> {cite web|title=}}"));
            Assert.AreEqual(@"* {{Citation|title=}}", Parsers.FixSyntax(@"* {Citation|title=}}"));

            Assert.AreEqual(@"<ref>{{cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref>{{cite web|url=a|title=b}}}}</ref>"));
            Assert.AreEqual(@"<ref> {{cite web|url=a|title=b}} </ref>", Parsers.FixSyntax(@"<ref> {{cite web|url=a|title=b}}}} </ref>"));
            Assert.AreEqual(@"<ref name=Fred>{{Cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref name=Fred>{{Cite web|url=a|title=b}}}}</ref>"));
            Assert.AreEqual(@"<ref name=""Fred"">{{cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref name=""Fred"">{{cite web|url=a|title=b}}}}</ref>"));
            Assert.AreEqual(@"<ref>{{citation|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref>{{citation|url=a|title=b}}}}</ref>"));
            Assert.AreEqual(@"<ref>{{cite web|url=a|title=b {{rp}}}}</ref>", Parsers.FixSyntax(@"<ref>{{cite web|url=a|title=b {{rp}}}}</ref>"));

            Assert.AreEqual(@"<ref>{{cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref>{[cite web|url=a|title=b}}</ref>"));
            Assert.AreEqual(@"<ref> {{cite web|url=a|title=b}} </ref>", Parsers.FixSyntax(@"<ref> {[cite web|url=a|title=b}} </ref>"));
            Assert.AreEqual(@"<ref name=Fred>{{Cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref name=Fred>{[Cite web|url=a|title=b}}</ref>"));
            Assert.AreEqual(@"<ref name=""Fred"">{{cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref name=""Fred"">{[cite web|url=a|title=b}}</ref>"));
            Assert.AreEqual(@"<ref>{{citation|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref>{[citation|url=a|title=b}}</ref>"));

            Assert.AreEqual(@"<ref>{{cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref>{{cite web|url=a|title=b{}</ref>"));
            Assert.AreEqual(@"<ref> {{cite web|url=a|title=b}} </ref>", Parsers.FixSyntax(@"<ref> {{cite web|url=a|title=b}] </ref>"));
            Assert.AreEqual(@"<ref name=Fred>{{Cite web|url=a|title=[[b]]}}</ref>", Parsers.FixSyntax(@"<ref name=Fred>{{Cite web|url=a|title=[[b]]}</ref>"));
            Assert.AreEqual(@"<ref name=""Fred"">{{cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref name=""Fred"">{{cite web|url=a|title=b{}</ref>"));
            Assert.AreEqual(@"<ref>{{citation|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref>{{citation|url=a|title=b}]</ref>"));

            Assert.AreEqual(@"<ref>{{cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref>cite web|url=a|title=b}}</ref>"));
            Assert.AreEqual(@"<ref name=""Smith"">{{cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref name=""Smith"">cite web|url=a|title=b}}</ref>"));
            Assert.AreEqual(@"<ref name=""Smith"">{{cite web|
url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref name=""Smith"">cite web|
url=a|title=b}}</ref>"));
            Assert.AreEqual(@"<ref>{{cite book|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref>cite book|url=a|title=b}}</ref>"));
            Assert.AreEqual(@"<ref>{{Citation|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref>Citation|url=a|title=b}}</ref>"));
            Assert.AreEqual(@"<ref>{{cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref>((cite web|url=a|title=b}}</ref>"));

            Assert.AreEqual(@"<ref>{{cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref>{{cite web|url=a|title=b}}}</ref>"), "fixes cite ending in three closing braces");
            Assert.AreEqual(@"<ref>{{cite web|url=a|title=b}}
			</ref>", Parsers.FixSyntax(@"<ref>{{cite web|url=a|title=b}}}
			</ref>"), "fixes cite ending in three closing braces, newline before ref end");

            Assert.AreEqual(@"<ref>{{cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref>{{{cite web|url=a|title=b}}</ref>"), "fixes cite starting in three opening braces");
            Assert.AreEqual(@"<ref name=""foo"">{{cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref name=""foo"">{{{cite web|url=a|title=b}}</ref>"), "fixes cite starting in three opening braces");

            string CiteDeadLink = @"<ref>{{cite web|url=a|title=b}} {{dead link|date=May 2011}}</ref>";
            Assert.AreEqual(CiteDeadLink, Parsers.FixSyntax(CiteDeadLink));

            CiteDeadLink = @"<ref>{{cite web|url=a|title=b {{dead link|date=May 2011}}}}</ref>";
            Assert.AreEqual(CiteDeadLink, Parsers.FixSyntax(CiteDeadLink));
        }

        [Test]
        public void FixUnbalancedBracketsGeneral()
        {
            const string CorrectCat = @"[[Category:Foo]]
[[Category:Foo2]]";

            Assert.AreEqual(CorrectCat, Parsers.FixSyntax(@"[[Category:Foo
[[Category:Foo2]]"), "closes unclosed cats");
            Assert.AreEqual(CorrectCat, Parsers.FixSyntax(CorrectCat));

            Assert.AreEqual(@"[[es:Foo]]
[[fr:Foo2]]", Parsers.FixSyntax(@"[[es:Foo
[[fr:Foo2]]"), "closes unclosed interwikis");

            const string CorrectFileLink = @"[[File:foo.jpeg|eflkjfdslkj]]";

            Assert.AreEqual(CorrectFileLink, Parsers.FixSyntax(@"{{File:foo.jpeg|eflkjfdslkj]]"));
            Assert.AreEqual(CorrectFileLink, Parsers.FixSyntax(CorrectFileLink));

            const string CorrectFileLink2 = @"[[Image:foo.jpeg|eflkjfdslkj]]";

            Assert.AreEqual(CorrectFileLink2, Parsers.FixSyntax(@"{{Image:foo.jpeg|eflkjfdslkj]]"));
            Assert.AreEqual(CorrectFileLink2, Parsers.FixSyntax(CorrectFileLink2));

            const string CorrectFileLink3 = @"[[file:foo.jpeg|eflkjfdslkj]]";

            Assert.AreEqual(CorrectFileLink3, Parsers.FixSyntax(@"{{file:foo.jpeg|eflkjfdslkj]]"));
            Assert.AreEqual(CorrectFileLink3, Parsers.FixSyntax(CorrectFileLink3));

            const string NoFix1 = @"==Charact==
[[Image:X.jpg|thumb
|alt=
|xx.]]

Japanese classification systemJapanese classification systemJapanese classification systemJapanese classification systemJapanese classification systemJapanese classification systemJapanese classification system

<gallery>
Image:X.JPG|Japanese classification systemJapanese classification systemJapanese classification system]]
</gallery>";

            Assert.AreEqual(NoFix1, Parsers.FixSyntax(NoFix1));
            
            Assert.AreEqual(@"[[Foo]]", Parsers.FixSyntax(@"[ [Foo]]"), "fixes link spacing");
            
            Assert.AreEqual(@"[[Foo]]", Parsers.FixSyntax(@"[[Foo]]]]"), "fixes excess link bracketss");
            
            Assert.AreEqual(@"[[Foo]],", Parsers.FixSyntax(@"[[Foo],]"), "fixes links broken by punctuation");
            Assert.AreEqual(@"[[Foo]].", Parsers.FixSyntax(@"[[Foo].]"), "fixes links broken by punctuation");
            Assert.AreEqual(@"[[Foo]]:", Parsers.FixSyntax(@"[[Foo]:]"), "fixes links broken by punctuation");
            Assert.AreEqual(@"[[Foo]]  bar", Parsers.FixSyntax(@"[[Foo] ] bar"), "fixes links broken by punctuation");
            Assert.AreEqual(@"[[Foo]]''", Parsers.FixSyntax(@"[[Foo]'']"), "fixes links broken by punctuation");

            Assert.AreEqual(@"[[panka Smith]] (Local national)", Parsers.FixLinkWhitespace(Parsers.FixSyntax(@"[panka  Smith]] (Local national)"), "Test"), "bracket and whitespace fix in one");

            const string Football = @"{{Infobox football biography
| playername     = D
| image          = 
| dateofdeath    = 1940 {aged 57)<ref name=A/>
| cityofdeath    = [[S]]
| years3         = 1911–19?? }}";

            Assert.AreEqual(Football.Replace(@"{aged", @"(aged"), Parsers.FixSyntax(Football));
            Assert.AreEqual(@"{{DEFAULTSORT:Foo}}", Parsers.FixSyntax(@"{{DEFAULTSORT:Foo
"), "fixes DEFAULTSORT ending");
            
            Assert.AreEqual(@"{{foo|par=[[Bar]]|par2=Bar2}}", Parsers.FixSyntax(@"{{foo|par=[[Bar[[|par2=Bar2}}"), "reversed brackets");

            Assert.AreEqual(@"{{foo|par=[[Bar]]|par2=Bar2}}", Parsers.FixSyntax(@"{{foo|par=[{Bar]]|par2=Bar2}}"));

            const string Unfixable1 = @"Ruth singled and Meusel [[bunt (baseball)|ed him over, but Ruth split his pants sliding into second, [[announcer|Radio announcer]] [[Graham McNamee]]";
            Assert.AreEqual(Unfixable1, Parsers.FixSyntax(Unfixable1));

             Assert.AreEqual(@"{{DEFAULTSORT:Foo}}", Parsers.FixSyntax(@"{{DEFAULTSORT:Foo))"), "fixes Template )) ending");
             const string Unfixable2 = @"Ruth [[File:One.JPEG|A [http://www.site.com/a]]]
XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
Now [[Fred was";
             Assert.AreEqual(Unfixable2, Parsers.FixSyntax(Unfixable2));

             Assert.AreEqual(Unfixable2 + @"
==Heading==
Now [[A]], was.", Parsers.FixSyntax(Unfixable2 + @"
==Heading==
Now [[A],] was."));
        }

        [Test]
        public void FixUnbalancedBracketsMathSetNotation()
        {
            const string MathSet1 = @"{[0], [1], [2]}", Foo = @"Foo { ...";
            Assert.AreEqual(Foo + MathSet1, Parsers.FixSyntax(Foo + MathSet1));
        }

        [Test]
        public void FixUnbalancedBracketsPersondataEnd()
        {
            const string PD = @"{{Persondata<!--Metadata: see [[Wikipedia:Persondata]].-->
|NAME=Orbe, Georgy
|ALTERNATIVE NAMES=
|SHORT DESCRIPTION=
|DATE OF BIRTH=1873
|PLACE OF BIRTH=[[Tbilisi]]
|DATE OF DEATH=1944
|PLACE OF DEATH=Paris

{{DEFAULTSORT:Orbe, Georgy}}";
            
            Assert.AreEqual(PD.Replace(@"Paris", @"Paris}}"), Parsers.FixSyntax(PD));
        }

        [Test]
        public void FixUnbalancedBracketsRef()
        {
            Assert.AreEqual(@"now <ref>foo</ref>", Parsers.FixSyntax(@"now <ref>>foo</ref>"));
            Assert.AreEqual(@"now <ref>[http://foo.com/bar/ text here]</ref>", Parsers.FixSyntax(@"now <ref>[http://foo.com/bar/ text here[</ref>"));

            Assert.AreEqual(@"<ref>[http://www.foo.com bar]</ref>", Parsers.FixSyntax(@"<ref>]http://www.foo.com bar]</ref>"));
            Assert.AreEqual(@"<ref name=A>[http://www.foo.com bar]</ref>", Parsers.FixSyntax(@"<ref name=A>]http://www.foo.com bar]</ref>"));
            Assert.AreEqual(@"<ref>[http://www.foo.com bar] this one</ref>", Parsers.FixSyntax(@"<ref>]http://www.foo.com bar] this one</ref>"));
        }

        [Test]
        public void TestCellpaddingTypo()
        {
            Assert.AreEqual(@"now {| class=""wikitable"" cellpadding=2", Parsers.FixSyntax(@"now {| class=""wikitable"" celpadding=2"));
            Assert.AreEqual(@"now {| class=""wikitable"" cellpadding=2", Parsers.FixSyntax(@"now {| class=""wikitable"" cellpading=2"));
            Assert.AreEqual(@"now {| class=""wikitable"" cellpadding=2", Parsers.FixSyntax(@"now {| class=""wikitable"" Celpading=2"));

            // no Matches
            Assert.AreEqual("now the cellpading of the", Parsers.FixSyntax("now the cellpading of the"));
            Assert.AreEqual(@"now {| class=""wikitable"" cellpadding=2", Parsers.FixSyntax(@"now {| class=""wikitable"" cellpadding=2"));
        }

        [Test]
        public void TestFixObsoleteBrAttributes()
        {
            Assert.AreEqual("{{clear}}", Parsers.FixSyntax(@"<br clear=both />"));
            Assert.AreEqual("{{clear}}", Parsers.FixSyntax(@"<Br clear=both />"));
            Assert.AreEqual("{{clear}}", Parsers.FixSyntax(@"<br clear=""both"" />"));
            Assert.AreEqual("{{clear}}", Parsers.FixSyntax(@"<br clear=all />"));
            Assert.AreEqual("{{clear}}", Parsers.FixSyntax(@"<br clear=""all"" />"));
            Assert.AreEqual("{{clear|left}}", Parsers.FixSyntax(@"<br clear=left />"));
            Assert.AreEqual("{{clear|left}}", Parsers.FixSyntax(@"<br clear=""left"" />"));
            Assert.AreEqual("{{clear|right}}", Parsers.FixSyntax(@"<br clear=right />"));
            Assert.AreEqual("{{clear|right}}", Parsers.FixSyntax(@"<br clear=""right"" />"));
        }

        [Test]
        public void TestFixSyntaxUnbalancedBrackets()
        {
            Assert.AreEqual(@"<ref>[http://www.site.com]</ref>", Parsers.FixSyntax(@"<ref>{{http://www.site.com}}</ref>"));
            Assert.AreEqual(@"<ref>[http://www.site.com cool site]</ref>", Parsers.FixSyntax(@"<ref>{{http://www.site.com cool site}}</ref>"));
            Assert.AreEqual(@"<ref name=""Fred"">[http://www.site.com]</ref>", Parsers.FixSyntax(@"<ref name=""Fred"">{{http://www.site.com}}</ref>"));
            Assert.AreEqual(@"<ref> [http://www.site.com] </ref>", Parsers.FixSyntax(@"<ref> {{http://www.site.com}} </ref>"));

            Assert.AreEqual(@"{{hello}}", Parsers.FixSyntax(@"{[hello}}"));
            Assert.AreEqual(@"{{hello}}", Parsers.FixSyntax(@"[{hello}}"));

            Assert.AreEqual(@"<ref>[http://site.com]</ref>", Parsers.FixSyntax(@"<ref>http://site.com]</ref>"));
            Assert.AreEqual(@"<ref>[http://site.com]</ref>", Parsers.FixSyntax(@"<ref>[http://site.com</ref>"));
            Assert.AreEqual(@"<REF>[http://site.com]</ref>", Parsers.FixSyntax(@"<REF>[http://site.com</ref>"));
            Assert.AreEqual(@"<ref>[http://site.com Smith, 2004]</ref>", Parsers.FixSyntax(@"<ref>Smith, 2004 http://site.com]</ref>"));
            Assert.AreEqual(@"<ref> [http://site.com] </ref>", Parsers.FixSyntax(@"<ref> http://site.com] </ref>"));
            Assert.AreEqual(@"<ref name=Fred>[http://site.com cool]</ref>", Parsers.FixSyntax(@"<ref name=Fred>http://site.com cool]</ref>"));
            Assert.AreEqual(@"<ref name=""Fred"">[http://site.com]</ref>", Parsers.FixSyntax(@"<ref name=""Fred"">http://site.com]</ref>"));
            Assert.AreEqual(@"<ref>[http://site.com great site-here]</ref>", Parsers.FixSyntax(@"<ref>http://site.com great site-here]</ref>"));

            Assert.AreEqual(@"<ref>Antara News [http://www.antara.co.id/arc/2009/1/9/kri-lebanon/ KRI Lebanon]</ref>", Parsers.FixSyntax(@"<ref>Antara News [http://www.antara.co.id/arc/2009/1/9/kri-lebanon/ KRI Lebanon</ref>"));
            Assert.AreEqual(@"<ref>Antara News [ftp://www.antara.co.id/arc/2009/1/9/kri-lebanon/ KRI Lebanon]</ref>", Parsers.FixSyntax(@"<ref>Antara News [ftp://www.antara.co.id/arc/2009/1/9/kri-lebanon/ KRI Lebanon</ref>"), "handles FTP protocol");
            Assert.AreEqual(@"<ref name=Smith>Antara News [http://www.antara.co.id/arc/2009/1/9/kri-lebanon/ KRI Lebanon]</ref>", Parsers.FixSyntax(@"<ref name=Smith>Antara News [http://www.antara.co.id/arc/2009/1/9/kri-lebanon/ KRI Lebanon</ref>"));
            Assert.AreEqual(@"<ref name=Smith>Antara News [http://www.antara.co.id/arc/2009/1/9/kri-lebanon/ KRI Lebanon]</ref>", Parsers.FixSyntax(@"<ref name=Smith>Antara News [http://www.antara.co.id/arc/2009/1/9/kri-lebanon/ KRI Lebanon]</ref>"));

            // no completion of template braces on non-template ref
            const string a = @"<ref>Smith and Jones, 2005, p46}}</ref>";
            Assert.AreEqual(a, Parsers.FixSyntax(a));

            Assert.AreEqual(@"{{DEFAULTSORT:Astaire, Fred}}", Parsers.FixSyntax(@"{{DEFAULTSORT:Astaire, Fred]}}"));
            Assert.AreEqual(@"{{DEFAULTSORT:Astaire, Fred}}", Parsers.FixSyntax(@"{{DEFAULTSORT:Astaire, Fred]]}}"));
            Assert.AreEqual(@"{{DEFAULTSORT:Astaire, Fred}}", Parsers.FixSyntax(@"{{DEFAULTSORT:Astaire, Fred]]]}}"));
            Assert.AreEqual(@"{{DEFAULTSORT:Astaire, Fred}}", Parsers.FixSyntax(@"{{DEFAULTSORT:Astaire, Fred[]}}"));

            // completes curly brackets where it would make all brackets balance
            Assert.AreEqual(@"Great.{{fact|date=May 2008}} Now", Parsers.FixSyntax(@"Great.{{fact|date=May 2008} Now"));
            Assert.AreEqual(@"Great.{{fact|date=May 2008}} Now", Parsers.FixSyntax(@"Great.{{fact|date=May 2008]} Now"));

            // don't change what could be wikitable
            Assert.AreEqual(@"Great.{{foo|} Now", Parsers.FixSyntax(@"Great.{{foo|} Now"));

            // set single curly bracket to normal bracket if that makes all brackets balance
            Assert.AreEqual(@"Great (not really) now", Parsers.FixSyntax(@"Great (not really} now"));
            Assert.AreEqual(@"# [[Herbert H. H. Fox]] ([[1934 - 1939]])<br>", Parsers.FixSyntax(@"# [[Herbert H. H. Fox]] ([[1934 - 1939]]}<br>"));
            // can't fix these
            Assert.AreEqual(@"Great { but (not really} now", Parsers.FixSyntax(@"Great { but (not really} now"));
            Assert.AreEqual(@"Great (not really)} now", Parsers.FixSyntax(@"Great (not really)} now"));
            // don't touch when it could be a table
            Assert.AreEqual(@"great (in 2001 | blah |} now", Parsers.FixSyntax(@"great (in 2001 | blah |} now"));

            // Fixes } to |
            const string cite1 = @"Great.<ref>{{cite web | url=http://www.site.com | title=abc } year=2009}}</ref>";
            Assert.AreEqual(@"Great.<ref>{{cite web | url=http://www.site.com | title=abc | year=2009}}</ref>", Parsers.FixSyntax(cite1));

            // set double round bracket to single
            Assert.AreEqual(@"then (but often) for", Parsers.FixSyntax(@"then ((but often) for"));

            // only applies changes if brackets then balance out
            Assert.AreEqual(@"then ((but often)) for", Parsers.FixSyntax(@"then ((but often)) for"));
            Assert.AreEqual(@"then ((but often)} for", Parsers.FixSyntax(@"then ((but often)} for"));
            Assert.AreEqual(@"then ((but often) for(", Parsers.FixSyntax(@"then ((but often) for("));

            // an unbalanced bracket is fixed if there are enough characters until the next one to be confident that the next one is indeed
            // a separate incident
            Assert.AreEqual(@"# [[Daniel Sylvester Tuttle|Daniel S. Tuttle]], (1867 - 1887)
# Ethelbert Talbot, (1887 - 1898),
# James B. Funsten, (1899 - 1918)
# Herman Page, (1919 - 1919)
# Frank H. Touret, (1919 - 1924)
# Herbert H. H. Fox, (1925 - 1926)
# [[Middleton S. Barnwell]], (1926 - 1935)
# Frederick B. Bartlett, (1935 - 1941)
# Frak A. Rhea, (1942 - 1968)
# Norman L. Foote, (1957 - 1972)
# Hanford L. King, Jr., (1972 - 1981)
# David B. Birney, IV, (1982 - 1989)
# John S. Thornton. (1990 - 1998
# Harry B. Bainbridge, III, (1998 - 2008)
# Brian J. Thom  (2008 - Present)", Parsers.FixSyntax(@"# [[Daniel Sylvester Tuttle|Daniel S. Tuttle]], (1867 - 1887)
# Ethelbert Talbot, (1887 - 1898),
# James B. Funsten, (1899 - 1918)
# Herman Page, (1919 - 1919}
# Frank H. Touret, (1919 - 1924)
# Herbert H. H. Fox, (1925 - 1926)
# [[Middleton S. Barnwell]], (1926 - 1935)
# Frederick B. Bartlett, (1935 - 1941)
# Frak A. Rhea, (1942 - 1968)
# Norman L. Foote, (1957 - 1972)
# Hanford L. King, Jr., (1972 - 1981)
# David B. Birney, IV, (1982 - 1989)
# John S. Thornton. (1990 - 1998
# Harry B. Bainbridge, III, (1998 - 2008)
# Brian J. Thom  (2008 - Present)"));

            Assert.AreEqual(@"The '''Zach Feuer Gallery''' is a [[contemporary art]] gallery located in [[Chelsea, Manhattan|Chelsea]], [[New York City|New York]].

==History==

The Zach Feuer Gallery was founded in 2000 as the LFL Gallery, by Nick Lawrence, Russell LaMontagne and Zach Feuer.  It was originally located on a fourth floor space on 26th Street.    In 2002 the gallery moved to a first floor space on 24th Street, briefly sharing space with an art book gallery owned by one of the partners.  In 2004  Zach Feuer purchased the gallery from his partners and changed the gallery name to Zach Feuer Gallery.

Some artists represented by Zach Feuer Gallery are [[Phoebe Washburn]], [[Jules de Balincourt]], [[Nathalie Djurberg]], [[Justin Lieberman]], [[Stuart Hawkins]],  [[Johannes Vanderbeek]], [[Sister Corita Kent]], [[Tamy Ben Tor]], [[Anton Henning]], [[Dana Schutz]], and [[Mark Flood]].

==External links==
* [http://www.zachfeuer.com Zach Feuer Gallery] website

{{coord missing|New York}}

[[Category:Contemporary art galleries]]
[[Category:2000 establishments]]
[[Category:Art galleries in Manhattan]]", Parsers.FixSyntax(@"The '''Zach Feuer Gallery''' is a [[contemporary art]] gallery located in [[Chelsea, Manhattan|Chelsea]], [[New York City|New York]].

==History==

The Zach Feuer Gallery was founded in 2000 as the LFL Gallery, by Nick Lawrence, Russell LaMontagne and Zach Feuer.  It was originally located on a fourth floor space on 26th Street.    In 2002 the gallery moved to a first floor space on 24th Street, briefly sharing space with an art book gallery owned by one of the partners.  In 2004  Zach Feuer purchased the gallery from his partners and changed the gallery name to Zach Feuer Gallery.

Some artists represented by Zach Feuer Gallery are [[Phoebe Washburn]], [[Jules de Balincourt]], [[Nathalie Djurberg]]], [[Justin Lieberman]], [[Stuart Hawkins]],  [[Johannes Vanderbeek]], [[Sister Corita Kent]], [[Tamy Ben Tor]], [[Anton Henning]], [[Dana Schutz]], and [[Mark Flood]].

==External links==
* [http://www.zachfeuer.com Zach Feuer Gallery] website

{{coord missing|New York}}

[[Category:Contemporary art galleries]]
[[Category:2000 establishments]]
[[Category:Art galleries in Manhattan]]"));

            Assert.AreEqual(@"<ref>[http://www.findagrave.com/cgi-bin/fg.cgi?page=gr&GRid=5194 Find A Grave]</ref>", Parsers.FixSyntax(@"<ref>{http://www.findagrave.com/cgi-bin/fg.cgi?page=gr&GRid=5194 Find A Grave]</ref>"));

            // convert [[[[link]] to [[link]] if that balances it all out
            Assert.AreEqual(@"hello [[link]] there", Parsers.FixSyntax(@"hello [[[[link]] there"));
            Assert.AreEqual(@"hello [[[[link]] there]]", Parsers.FixSyntax(@"hello [[[[link]] there]]"));

            // convert {blah) to (blah) if that balances it all out, not wikitables, templates
            Assert.AreEqual(@"hello (link) there", Parsers.FixSyntax(@"hello {link) there"));
            Assert.AreEqual(@"hello {|table|blah) there", Parsers.FixSyntax(@"hello {|table|blah) there"));
            Assert.AreEqual(@"{{cite web|title=a|url=http://www.site.com|publisher=ABC)|year=2006}", Parsers.FixSyntax(@"{{cite web|title=a|url=http://www.site.com|publisher=ABC)|year=2006}"));

            // convert [blah} to [blah] if that balances it all out
            Assert.AreEqual(@"*[http://aeiou.visao.pt/Pages/Lusa.aspx?News=200808068620453 Obituary notice] here", Parsers.FixSyntax(@"*[http://aeiou.visao.pt/Pages/Lusa.aspx?News=200808068620453 Obituary notice} here"));

            // don't touch template ends
            Assert.AreEqual(@"[http://aeiou.visao.pt/Pages/Lusa.aspx?News=200808068620453 Obituary notice}}", Parsers.FixSyntax(@"[http://aeiou.visao.pt/Pages/Lusa.aspx?News=200808068620453 Obituary notice}}"));

            // correct [[blah blah}word]] to [[blah blah|word]]
            Assert.AreEqual(@"[[blah blah|word]]", Parsers.FixSyntax(@"[[blah blah}word]]"));
            Assert.AreEqual(@"[[blah|word]]", Parsers.FixSyntax(@"[[blah}word]]"));

            // []foo]] --> [[foo]]
            Assert.AreEqual(@"now [[link]] was", Parsers.FixSyntax(@"now []link]] was"));
            Assert.AreEqual(@"now [[link|a]] was", Parsers.FixSyntax(@"now []link|a]] was"));

            // not if unbalanced brackets remain
            Assert.AreEqual(@"{ here [[blah blah}word]]", Parsers.FixSyntax(@"{ here [[blah blah}word]]"));

            // correct {[link]] or {[[link]] or [[[link]] or [[{link]]
            Assert.AreEqual(@"now [[link]] was", Parsers.FixSyntax(@"now {[link]] was"));
            Assert.AreEqual(@"now [[link]] was", Parsers.FixSyntax(@"now {[[link]] was"));
            Assert.AreEqual(@"now [[link]] was", Parsers.FixSyntax(@"now [[[link]] was"));
            Assert.AreEqual(@"now [[link]] was", Parsers.FixSyntax(@"now [[{link]] was"));

            // not if unbalanced brackets remain nearby
            Assert.AreEqual(@"now {[[link]]} was", Parsers.FixSyntax(@"now {[link]]} was"));
            Assert.AreEqual(@"now [[[link]] was]", Parsers.FixSyntax(@"now [[[link]] was]"));

            // convert [[link]]]] to [[link]] IFF that balances it all out
            Assert.AreEqual(@"hello [[link]] there", Parsers.FixSyntax(@"hello [[link]]]] there"));
            Assert.AreEqual(@"[[hello [[link]]]] there", Parsers.FixSyntax(@"[[hello [[link]]]] there"));

            //Unbalanced bracket and double pipe [[foo||bar] inside a table
            Assert.AreEqual(@"{|
			|-
			| [[foo|bar]]
			|}", Parsers.FixSyntax(@"{|
			|-
			| [[foo||bar]
			|}"));

            // external links missing brackets
            Assert.AreEqual(@"blah
* [http://www.site.com a site]
* [http://www.site2.com another]", Parsers.FixSyntax(@"blah
* [http://www.site.com a site
* [http://www.site2.com another]"));
            Assert.AreEqual(@"blah
* [http://www.site.com a site]
* [http://www.site2.com another]", Parsers.FixSyntax(@"blah
* http://www.site.com a site]
* [http://www.site2.com another]"));

            Assert.AreEqual(@"now ({{lang-el|foo}}) was", Parsers.FixSyntax(@"now ({lang-el|foo}}) was"));

            //  IndexOutOfRangeException bug
            Assert.AreEqual(@"] now", Parsers.FixSyntax(@"] now"));

            Assert.AreEqual(@"{{DEFAULTSORT:hello}}
now", Parsers.FixSyntax(@"{{DEFAULTSORT:hello
now"));

            Assert.AreEqual(@"|[[Belmont (Durham) railway station|Belmont]] ([[Durham]])
|[[North Eastern Railway (UK)|NER]]
|1857", Parsers.FixSyntax(@"|[[Belmont (Durham) railway station|Belmont]] {[[Durham]])
|[[North Eastern Railway (UK)|NER]]
|1857"));

            const string Choisir = @"{{Thoroughbred
| horsename = Choisir
| image =
| caption =
| sire = [[Danehill Dancer]]
| grandsire = [[Danehill (horse)|Danehill]]
| dam = [[Great Selection]]
| damsire =
| sex =
| foaled =
| country = [[Australia|Australian]]}
| colour =
| breeder =
| owner = T
| trainer = [[Paul Perry]]
}}
'''Choisir'''";

            Assert.AreEqual(Choisir, Parsers.FixSyntax(Choisir));

            const string Nochange = @"** >> {[[Sei Young Animation Co., Ltd.|Animação Retrô]]}";

            Assert.AreEqual(Nochange, Parsers.FixSyntax(Nochange));
            
            Assert.AreEqual(@"<ref>{{cite web|url=a|title=b (ABC) }}</ref>", Parsers.FixSyntax(@"<ref>{{cite web|url=a|title=b (ABC} }}</ref>"));
        }

        [Test, Category("Incomplete")]
        public void TestFixSyntax()
        {
            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_3#NEsted_square_brackets_again.
            Assert.AreEqual("[[Image:foo.jpg|Some [http://some_crap.com]]]",
                            Parsers.FixSyntax("[[Image:foo.jpg|Some [http://some_crap.com]]]"));

            Assert.AreEqual("[[File:foo.jpg|Some [http://some_crap.com]]]",
                            Parsers.FixSyntax("[[File:foo.jpg|Some [http://some_crap.com]]]"));

            Assert.AreEqual("Image:foo.jpg|{{{some_crap}}}]]", Parsers.FixSyntax("Image:foo.jpg|{{{some_crap}}}]]"));

            Assert.AreEqual("[[somelink]]", Parsers.FixSyntax("[somelink]]"));
            Assert.AreEqual("[[somelink]]", Parsers.FixSyntax("[[somelink]"));
            Assert.AreNotEqual("[[somelink]]", Parsers.FixSyntax("[somelink]"));
            Assert.AreEqual("[[somelink]]", Parsers.FixSyntax("[[somelink|]"));

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_7#Erroneously_removing_pipe
            Assert.AreEqual("[[|foo]]", Parsers.FixSyntax("[[|foo]]"));

            bool noChange;
            //TODO: move it to parts testing specific functions, when they're covered
            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_4#Bug_encountered_when_perusing_Sonorous_Susurrus
            Parsers.CanonicalizeTitle("[[|foo]]"); // shouldn't throw exceptions
            Assert.AreEqual("[[|foo]]", Parsers.FixLinks("[[|foo]]", "bar", out noChange));

            Assert.AreEqual(@"[[foo|bar]]", Parsers.FixSyntax(@"[[foo||bar]]"));
            Assert.AreEqual("[[somelink#a]]", Parsers.FixLinkWhitespace("[[somelink_#a]]", "Test"));
        }
 
        [Test]
        public void TestFixSyntaxExternalLinkSpacing()
        {
            Assert.AreEqual(@"their new [http://www.site.com site]", Parsers.FixSyntax(@"their new[http://www.site.com site]"));
            Assert.AreEqual(@"their new [http://www.site.com site] was", Parsers.FixSyntax(@"their new [http://www.site.com site]was"));
            Assert.AreEqual(@"their new [http://www.site.com site] was", Parsers.FixSyntax(@"their new[http://www.site.com site]was"));

            Assert.AreEqual(@"their new [http://www.site.com site]", Parsers.FixSyntax(@"their new [http://www.site.com site]"));
            Assert.AreEqual(@"their new [http://www.site.com site] was", Parsers.FixSyntax(@"their new [http://www.site.com site] was"));

            Assert.AreEqual(@"their new [http://www.site.com site] was [[blog]]ger then", Parsers.FixSyntax(@"their new [http://www.site.com site] was [[blog]]ger then"));

            const string nochange1 = @"their [[play]]ers", nochange2 = @"ts borders.<ref>[http://cyber.law.harvard.edu/filtering/a/ Saudi Arabia]</ref> A Saudi";
            Assert.AreEqual(nochange1, Parsers.FixSyntax(nochange1));
            Assert.AreEqual(nochange2, Parsers.FixSyntax(nochange2));

            // https
            Assert.AreEqual(@"their new [https://www.site.com site]", Parsers.FixSyntax(@"their new[https://www.site.com site]"));
            Assert.AreEqual(@"their new [https://www.site.com site] was", Parsers.FixSyntax(@"their new [https://www.site.com site]was"));
            Assert.AreEqual(@"their new [https://www.site.com site] was", Parsers.FixSyntax(@"their new[https://www.site.com site]was"));

            Assert.AreEqual(@"their new [https://www.site.com site]", Parsers.FixSyntax(@"their new [https://www.site.com site]"));
            Assert.AreEqual(@"their new [https://www.site.com site] was", Parsers.FixSyntax(@"their new [https://www.site.com site] was"));

            Assert.AreEqual(@"their new [https://www.site.com site] was [[blog]]ger then", Parsers.FixSyntax(@"their new [https://www.site.com site] was [[blog]]ger then"));

            const string nochange3 = @"ts borders.<ref>[https://cyber.law.harvard.edu/filtering/a/ Saudi Arabia]</ref> A Saudi";
            Assert.AreEqual(nochange3, Parsers.FixSyntax(nochange3));

#if debug
			// In Chinese Wikipedia  the text inside and outside of the link should be directly connected
            Variables.SetProjectLangCode("zh");
            Assert.AreEqual(@"their new[http://www.site.com site]", Parsers.FixSyntax(@"their new[http://www.site.com site]"));
            Assert.AreEqual(@"their new [http://www.site.com site]was", Parsers.FixSyntax(@"their new [http://www.site.com site]was"));
            Assert.AreEqual(@"their new[http://www.site.com site]was", Parsers.FixSyntax(@"their new[http://www.site.com site]was"));
            
            Variables.SetProjectLangCode("en");
            Assert.AreEqual(@"their new [http://www.site.com site]", Parsers.FixSyntax(@"their new[http://www.site.com site]"));
#endif

        }

        [Test]
        public void TestFixSyntaxReferencesWithNoHttp()
        {
            Assert.AreEqual(@"<ref>http://www.foo.com</ref>", Parsers.FixSyntax(@"<ref>www.foo.com</ref>"),"missing http");
            Assert.AreEqual(@"<ref>[http://www.foo.com bar]</ref>", Parsers.FixSyntax(@"<ref>[www.foo.com bar]</ref>"),"missing http inside brackets");
            Assert.AreEqual(@"<ref name=test>http://www.foo.com</ref>", Parsers.FixSyntax(@"<ref name=test>www.foo.com</ref>"), "missing http inside named ref");
            Assert.AreEqual(@"<ref>http://www.foo.com</ref>", Parsers.FixSyntax(@"<ref>       www.foo.com</ref>"));
            Assert.AreEqual(@"Visit www.foo.com", Parsers.FixSyntax(@"Visit www.foo.com"), "no changes outside references");
            Assert.AreEqual(@"<ref>[www-foo.a.com bar]</ref>", Parsers.FixSyntax(@"<ref>[www-foo.a.com bar]</ref>"),"No change for www-");
        }

        [Test]
        public void TestFixIncorrectBr()
        {
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"<br.>"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"<BR.>"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"<br\>"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"<BR\>"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"<\br>"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"<br./>"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"<br /a>"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"<br /v>"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"<br /r>"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"<br /s>"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"<br /t>"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"<br /z>"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"<br /0>"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"<br /1>"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"<br /2>"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"<br /9>"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"<br /•>"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"<br/•>"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"<br /br>"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"<br ?>"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"<br?>"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"<br//>"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"</br>"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"</br >"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"</br />"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"</br/>"));

            // these are already correct
            Assert.AreEqual("<br/>", Parsers.FixSyntax(@"<br/>"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"<br />"));
        }

    }
}