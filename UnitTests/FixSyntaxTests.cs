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

using NUnit.Framework;
using WikiFunctions;
using WikiFunctions.Parse;

namespace UnitTests
{
    [TestFixture]
    public class FixSyntaxTests : RequiresParser
    {
        public GenfixesTestsBase genFixes  = new GenfixesTestsBase();

        [Test]
        public void FixISBNFormat()
        {
            Assert.That(Parsers.FixSyntax(@"ISBN: 1245781549"), Is.EqualTo(@"ISBN 1245781549"), "removes colon after ISBN");
            Assert.That(Parsers.FixSyntax(@"[[ISBN]] 1245781549"), Is.EqualTo(@"ISBN 1245781549"), "removes wikilink around ISBN");
            Assert.That(Parsers.FixSyntax(@"ISBN-10: 1245781549"), Is.EqualTo(@"ISBN 1245781549"), "removes colon after ISBN");
            Assert.That(Parsers.FixSyntax(@"ISBN-13: 9781245781549"), Is.EqualTo(@"ISBN 9781245781549"), "removes colon after ISBN");
            Assert.That(Parsers.FixSyntax(@"ISBN 1245781549"), Is.EqualTo(@"ISBN 1245781549"), "no change if already correct");
            Assert.That(Parsers.FixSyntax(@"ISBN:1245781549"), Is.EqualTo(@"ISBN 1245781549"), "removes colon after ISBN");
            Assert.That(Parsers.FixSyntax(@"ISBN	1245781549"), Is.EqualTo(@"ISBN 1245781549"), "ISBN with tab");
            Assert.That(Parsers.FixSyntax(@"ISBN 	1245781549"), Is.EqualTo(@"ISBN 1245781549"), "ISBN with space and tab");

            Assert.That(Parsers.FixSyntax(@"ISBN-1245781549"), Is.EqualTo(@"ISBN 1245781549"), "removes minus after ISBN");
            Assert.That(Parsers.FixSyntax(@"ISBN-1045781549"), Is.EqualTo(@"ISBN 1045781549"), "removes minus after ISBN");
            Assert.That(Parsers.FixSyntax(@"ISBN–1245781549"), Is.EqualTo(@"ISBN 1245781549"), "removes dash after ISBN");
            Assert.That(Parsers.FixSyntax(@"ISBN–13 9781245781549"), Is.EqualTo(@"ISBN-13 9781245781549"), "Fix ISBN–13 endash to dash");
            Assert.That(Parsers.FixSyntax(@"ISBN–10 12345781549"), Is.EqualTo(@"ISBN-10 12345781549"), "Fix ISBN–10 endash to dash");
            Assert.That(Parsers.FixSyntax(@"ISBN-10 12345781549"), Is.EqualTo(@"ISBN-10 12345781549"), "do nothing");
            Assert.That(Parsers.FixSyntax(@"ISBN-13 9781245781549"), Is.EqualTo(@"ISBN-13 9781245781549"), "do nothing");

            Assert.That(Parsers.FixSyntax(@"ISBN 0-9752298-0-x"), Is.EqualTo(@"ISBN 0-9752298-0-X"), "capitalise X");
            Assert.That(Parsers.FixSyntax(@"ISBN 097522980x"), Is.EqualTo(@"ISBN 097522980X"), "capitalise X");

            Assert.That(Parsers.FixSyntax(@"ISBN 978–92–0–103709–1"), Is.EqualTo(@"ISBN 978-92-0-103709-1"), "endash to hyphen within ISBN");
            Assert.That(Parsers.FixSyntax(@"ISBN 978–92–0–103709–X"), Is.EqualTo(@"ISBN 978-92-0-103709-X"), "endash to hyphen within ISBN");

            // {{ISBN-10}} and {{ISBN-13}} have been deleted
            // Assert.AreEqual(@"{{ISBN-10|1245781549}}", Parsers.FixSyntax(@"{{ISBN-10|1245781549}}"), "no change if already correct – ISBN-10 template");
            // Assert.AreEqual(@"{{ISBN-13|9781245781549}}", Parsers.FixSyntax(@"{{ISBN-13|9781245781549}}"), "no change if already correct – ISBN-13 template");

            Assert.That(Parsers.FixSyntax(@"[http://www.hup.harvard.edu/catalog.php?isbn=9780674372993 example]"), Is.EqualTo(@"[http://www.hup.harvard.edu/catalog.php?isbn=9780674372993 example]"), "no change inside url");
            Assert.That(Parsers.FixSyntax(@"foo<ref name=""isbn0-19-517234-5"" />"), Is.EqualTo(@"foo<ref name=""isbn0-19-517234-5"" />"), "no change inside ref");

            Assert.That(Parsers.FixSyntax(@"[[ISBN]] [[Special:BookSources/9711005522|9711005522]]"), Is.EqualTo(@"ISBN 9711005522"), "ISBNT substed");
            Assert.That(Parsers.FixSyntax(@"[[ISBN]] [[Special:BookSources/9780881925166|9780881925166]]"), Is.EqualTo(@"ISBN 9780881925166"), "ISBNT substed");
            Assert.That(Parsers.FixSyntax(@"[[ISBN]] [[Special:BookSources/9780881925166|<bdi>9780881925166</bdi>]]"), Is.EqualTo(@"ISBN 9780881925166"), "ISBNT substed bdi tags");
            Assert.That(Parsers.FixSyntax(@"[[International Standard Book Number|ISBN]] [[Special:BookSources/9780881925166|9780881925166]]"), Is.EqualTo(@"ISBN 9780881925166"), "ISBNT substed");
            Assert.That(Parsers.FixSyntax(@"[[International Standard Book Number|ISBN]] [[Special:BookSources/9780881925166|<bdi>9780881925166</bdi>]]"), Is.EqualTo(@"ISBN 9780881925166"), "ISBNT substed bdi tags");

            Assert.That(Parsers.FixSyntax(@"[http://www.site.com info ISBN 978-92-0-103709-X]"), Is.EqualTo(@"[http://www.site.com info] ISBN 978-92-0-103709-X"), "ISBN out of end of http external link");
            Assert.That(Parsers.FixSyntax(@"[http://www.site.com info ISBN 978-92-0-103709-X.]"), Is.EqualTo(@"[http://www.site.com info] ISBN 978-92-0-103709-X."), "ISBN out of end of http external link");
            Assert.That(Parsers.FixSyntax(@"[http://www.site.com info ISBN 978-92-0-103709-X. ]"), Is.EqualTo(@"[http://www.site.com info] ISBN 978-92-0-103709-X."), "ISBN out of end of http external link");
            Assert.That(Parsers.FixSyntax(@"[http://www.site.com info ISBN 978920103709X]"), Is.EqualTo(@"[http://www.site.com info] ISBN 978920103709X"), "ISBN out of end of http external link, ISBN ends X");
            Assert.That(Parsers.FixSyntax(@"[http://www.site.com info ISBN 9789201037091]"), Is.EqualTo(@"[http://www.site.com info] ISBN 9789201037091"), "ISBN out of end of http external link, ISBN ends number");
            Assert.That(Parsers.FixSyntax(@"[http://www.site.com info, ISBN 9789201037091]"), Is.EqualTo(@"[http://www.site.com info] ISBN 9789201037091"), "ISBN out of end of http external link, comma");
            Assert.That(Parsers.FixSyntax(@"[http://www.site.com info; ISBN 9789201037091]"), Is.EqualTo(@"[http://www.site.com info] ISBN 9789201037091"), "ISBN out of end of http external link, semicolon");
            Assert.That(Parsers.FixSyntax(@"[http://www.site.com info ISBN 978920103709X ISBN 9789201037091]"), Is.EqualTo(@"[http://www.site.com info] ISBN 978920103709X ISBN 9789201037091"), "ISBN out of end of http external link, 2x");

            const string nochange = @"[http://www.site.com ISBN 978-92-0-103709-X]";
            Assert.That(Parsers.FixSyntax(nochange), Is.EqualTo(nochange), "No change to ISBN at end of external link when no other link text");

        }

        [Test]
        public void FixISBNInfobox()
        {
            Assert.That(Parsers.FixSyntax(@"{{infobox foo|isbn=ISBN 9711005522 }}"), Is.EqualTo(@"{{infobox foo|isbn=9711005522 }}"), "ISBN removed in infobox isbn parameter");
            Assert.That(Parsers.FixSyntax(@"{{infobox foo|isbn=ISBN: 9711005522 }}"), Is.EqualTo(@"{{infobox foo|isbn=9711005522 }}"), "ISBN: removed in infobox isbn parameter");
            Assert.That(Parsers.FixSyntax(@"{{infobox foo|isbn=ISBN : 9711005522 }}"), Is.EqualTo(@"{{infobox foo|isbn=9711005522 }}"), "ISBN : removed in infobox isbn parameter");
            Assert.That(Parsers.FixSyntax(@"{{infobox foo|isbn=ISBN  9711005522 }}"), Is.EqualTo(@"{{infobox foo|isbn=9711005522 }}"), "ISBN removed in infobox isbn parameter, extra space");
            Assert.That(Parsers.FixSyntax(@"{{infobox foo|isbn=ISBN 9711005522 }} {{infobox foo|isbn=ISBN 9744005522 }}"), Is.EqualTo(@"{{infobox foo|isbn=9711005522 }} {{infobox foo|isbn=9744005522 }}"), "ISBN removed in infobox isbn parameter, multiple infoxboxes");
            Assert.That(Parsers.FixSyntax(@"{{infobox foo|isbn=9711005522 }}"), Is.EqualTo(@"{{infobox foo|isbn=9711005522 }}"), "No change when isbn= already correct");
            Assert.That(Parsers.FixSyntax(@"{{infobox foo|foo=ISBN 9711005522 }}"), Is.EqualTo(@"{{infobox foo|foo=ISBN 9711005522 }}"), "No change to non-isbn parameter");
        }

        [Test]
        public void FixPMIDFormat()
        {
            Assert.That(Parsers.FixSyntax(@"PMID: 1245781549"), Is.EqualTo(@"PMID 1245781549"), "removes colon after PMID");
            Assert.That(Parsers.FixSyntax(@"PMID:1245781549"), Is.EqualTo(@"PMID 1245781549"), "removes colon after PMID");
            Assert.That(Parsers.FixSyntax(@"PMID:    1245781549"), Is.EqualTo(@"PMID 1245781549"), "removes colon after PMID");
            Assert.That(Parsers.FixSyntax(@"PMID 1245781549"), Is.EqualTo(@"PMID 1245781549"), "No change if alrady correct");
            Assert.That(Parsers.FixSyntax(@"[[PMID:1245781549]]"), Is.EqualTo(@"[[PMID:1245781549]]"), "No change if alrady correct - magic link");

#if DEBUG
            // PMID: as magig word is used on hu-wiki
            Variables.SetProjectLangCode("hu");
            Assert.That(Parsers.FixSyntax(@"PMID:1245781549"), Is.EqualTo(@"PMID:1245781549"));

            Variables.SetProjectLangCode("en");
            Assert.That(Parsers.FixSyntax(@"PMID: 1245781549"), Is.EqualTo(@"PMID 1245781549"), "removes colon after PMID");
            #endif
        }

        [Test]
        public void FixHtmlTagsSyntax()
        {
            const string corr = @"Foo<small>bar</small> was the 1<sub>st</sub> to drink H<sup>2</sup>O";
            Assert.That(Parsers.FixSyntax(@"Foo<small>bar<small/> was the 1<sub>st<sub/> to drink H<sup>2<sup/>O"), Is.EqualTo(corr));
            Assert.That(Parsers.FixSyntax(@"Foo<small>bar<small/> was the 1<sub>st</sub/> to drink H<sup>2</sup/>O"), Is.EqualTo(corr));
            Assert.That(Parsers.FixSyntax(corr), Is.EqualTo(corr));

            Assert.That(Parsers.FixSyntax(@"<center>Centered text<center/>"), Is.EqualTo(@"<center>Centered text</center>"));
        }

        [Test]
        public void FixSyntaxHorizontalRule()
        {
            Assert.That(Parsers.FixSyntax(@"<hr>"), Is.EqualTo(@"----"));
            Assert.That(Parsers.FixSyntax(@"-----"), Is.EqualTo(@"----"));
            Assert.That(Parsers.FixSyntax(@"A
<hr>
B"), Is.EqualTo(@"A
----
B"));
            string Nochange = @"A<hr>";
            Assert.That(Parsers.FixSyntax(Nochange), Is.EqualTo(Nochange));
            Nochange = @"A----";
            Assert.That(Parsers.FixSyntax(Nochange), Is.EqualTo(Nochange));
        }

        [Test]
        public void FixSyntaxRedirects()
        {
            Assert.That(Parsers.FixSyntaxRedirects(@"#REDIRECT
[[Foo]]"), Is.EqualTo(@"#REDIRECT [[Foo]]"), "newline");
            Assert.That(Parsers.FixSyntaxRedirects(@"#REDIRECT [[[Foo]]]"), Is.EqualTo(@"#REDIRECT [[Foo]]"), "extra opening/closing bracket");
            Assert.That(Parsers.FixSyntaxRedirects(@"#REDIRECT [[Foo]]]"), Is.EqualTo(@"#REDIRECT [[Foo]]"), "one extra closing bracket");
            Assert.That(Parsers.FixSyntaxRedirects(@"#REDIRECT [[[Foo]]"), Is.EqualTo(@"#REDIRECT [[Foo]]"), "one extra opening bracket");
            Assert.That(Parsers.FixSyntaxRedirects(@"#REDIRECT [[[[Foo]]"), Is.EqualTo(@"#REDIRECT [[Foo]]"), "two extra opening brackets");
            Assert.That(Parsers.FixSyntaxRedirects(@"#REDIRECT [[Foo]]]]"), Is.EqualTo(@"#REDIRECT [[Foo]]"), "two extra closing brackets");
            Assert.That(Parsers.FixSyntaxRedirects(@"#REDIRECT [[[[Foo]]"), Is.EqualTo(@"#REDIRECT [[Foo]]"), "two extra opening/closing brackets");
            Assert.That(Parsers.FixSyntaxRedirects(@"#REDIRECT
[[[Foo]]]"), Is.EqualTo(@"#REDIRECT [[Foo]]"), "extra brackets and newline");
            Assert.That(Parsers.FixSyntaxRedirects(@"#REDIRECT:[[Foo]]"), Is.EqualTo(@"#REDIRECT [[Foo]]"), "double dot unspaced");
            Assert.That(Parsers.FixSyntaxRedirects(@"#REDIRECT: [[Foo]]"), Is.EqualTo(@"#REDIRECT [[Foo]]"), "double dot with space");
            Assert.That(Parsers.FixSyntaxRedirects(@"#REDIRECT=[[Foo]]"), Is.EqualTo(@"#REDIRECT [[Foo]]"), "equal sign unspaced");
            Assert.That(Parsers.FixSyntaxRedirects(@"#REDIRECT= [[Foo]]"), Is.EqualTo(@"#REDIRECT [[Foo]]"), "equal sihn with space");
            Assert.That(Parsers.FixSyntaxRedirects(@"#REDIRECT=
[[[Foo]]]"), Is.EqualTo(@"#REDIRECT [[Foo]]"), "extra brackets, equal sign and newline");

            Assert.That(Parsers.FixSyntaxRedirects(@"#REDIRECT[[Foo]] {{Template:R from move}}"), Is.EqualTo(@"#REDIRECT[[Foo]] {{R from move}}"), "template prefix");
        }

        [Test]
        public void FixSyntaxRedirectsBrackets()
        {
            Assert.That(Parsers.FixSyntaxRedirects(@"#REDIRECT[[Foo]] {{Template:R from move"), Is.EqualTo(@"#REDIRECT[[Foo]] {{R from move}}"), "Missing }}");
            Assert.That(Parsers.FixSyntaxRedirects(@"#REDIRECT[[Foo]] {{Template:R from move" + "\r\n"), Is.EqualTo(@"#REDIRECT[[Foo]] {{R from move}}"), "Missing }} with newline");
            Assert.That(Parsers.FixSyntaxRedirects(@"#REDIRECT[[Foo]] {{R from move}"), Is.EqualTo(@"#REDIRECT[[Foo]] {{R from move}}"), "Missing }");
            Assert.That(Parsers.FixSyntaxRedirects(@"#REDIRECT[[Foo]] {{R from move}]"), Is.EqualTo(@"#REDIRECT[[Foo]] {{R from move}}"), "Has }]");
            Assert.That(Parsers.FixSyntaxRedirects(@"#REDIRECT[[Foo]] {{R from move]}"), Is.EqualTo(@"#REDIRECT[[Foo]] {{R from move}}"), "Has ]}");
        }

        [Test]
        public void ExternalLinksNewline()
        {
            Assert.That(Parsers.FixSyntax(@"here [http://www.site.com text
here]"), Is.EqualTo(@"here [http://www.site.com text here]"), "newline removed");
            Assert.That(Parsers.FixSyntax(@"here [http://www.site.com text here2
]"), Is.EqualTo(@"here [http://www.site.com text here2 ]"), "newline removed2");
            Assert.That(Parsers.FixSyntax(@"here [http://www.site.com text
here
there]"), Is.EqualTo(@"here [http://www.site.com text here there]"), "multiple newlines removed");
            Assert.That(Parsers.FixSyntax(@"here [http://www.site.com |text
here]"), Is.EqualTo(@"here [http://www.site.com text here]"), "newline removed3");

            Assert.That(Parsers.FixSyntax(@"here [http://www.site.com text here]"), Is.EqualTo(@"here [http://www.site.com text here]"), "no change if no new line");
        }

        [Test]
        public void UnbalancedBrackets()
        {
            int bracketLength = 0;
            Assert.That(Parsers.UnbalancedBrackets(@"now hello {{bye}} {{now}", out bracketLength), Is.EqualTo(18));
            Assert.That(bracketLength, Is.EqualTo(2));
            Assert.That(Parsers.UnbalancedBrackets(@"now hello {{bye}} {{now} abc", out bracketLength), Is.EqualTo(18));
            Assert.That(bracketLength, Is.EqualTo(2));
            Assert.That(Parsers.UnbalancedBrackets(@"now hello {{bye}} {{now} abc {{def}}", out bracketLength), Is.EqualTo(18));
            Assert.That(bracketLength, Is.EqualTo(2));
            Assert.That(Parsers.UnbalancedBrackets(@"now hello {{bye}} [[now]", out bracketLength), Is.EqualTo(18));
            Assert.That(bracketLength, Is.EqualTo(2));
            Assert.That(Parsers.UnbalancedBrackets(@"now hello [[bye]] {{now}", out bracketLength), Is.EqualTo(18));
            Assert.That(bracketLength, Is.EqualTo(2));
            Assert.That(Parsers.UnbalancedBrackets(@"now hello {{bye}} [now words [here]", out bracketLength), Is.EqualTo(18));
            Assert.That(bracketLength, Is.EqualTo(1));
            Assert.That(Parsers.UnbalancedBrackets(@"now hello {{bye}} now] words [here]", out bracketLength), Is.EqualTo(21));
            Assert.That(bracketLength, Is.EqualTo(1));
            Assert.That(Parsers.UnbalancedBrackets(@"now hello {{bye}} {now}}", out bracketLength), Is.EqualTo(22));
            Assert.That(bracketLength, Is.EqualTo(2));
            Assert.That(Parsers.UnbalancedBrackets(@"[http://www.site.com a link [cool]]", out bracketLength), Is.EqualTo(33)); // FixSyntax replaces with &#93;
            Assert.That(bracketLength, Is.EqualTo(2));
            Assert.That(Parsers.UnbalancedBrackets(@"now hello {{bye}} {now", out bracketLength), Is.EqualTo(18));
            Assert.That(bracketLength, Is.EqualTo(1));
            Assert.That(Parsers.UnbalancedBrackets(@"now hello {abyea} {now", out bracketLength), Is.EqualTo(18));
            Assert.That(bracketLength, Is.EqualTo(1));
            Assert.That(Parsers.UnbalancedBrackets(@"now hello {{bye}} {now {{here}} a", out bracketLength), Is.EqualTo(18));
            Assert.That(bracketLength, Is.EqualTo(1));
            Assert.That(Parsers.UnbalancedBrackets(@"now hello {{bye}} {now {here} a {{a}}", out bracketLength), Is.EqualTo(18));
            Assert.That(bracketLength, Is.EqualTo(1));
            Assert.That(Parsers.UnbalancedBrackets(@"now hello {{bye}} now {here} a {{a}}}", out bracketLength), Is.EqualTo(36));
            Assert.That(bracketLength, Is.EqualTo(1));
            Assert.That(Parsers.UnbalancedBrackets(@"{bye", out bracketLength), Is.EqualTo(0));
            Assert.That(bracketLength, Is.EqualTo(1));
            Assert.That(Parsers.UnbalancedBrackets(@"<bye", out bracketLength), Is.EqualTo(0));
            Assert.That(bracketLength, Is.EqualTo(1));
            Assert.That(Parsers.UnbalancedBrackets(@"now hello [words [here&#93; end] now]", out bracketLength), Is.EqualTo(36));
            Assert.That(bracketLength, Is.EqualTo(1));

            // only first reported
            Assert.That(Parsers.UnbalancedBrackets(@"now hello {{bye}} {{now} or {{now} was", out bracketLength), Is.EqualTo(18));
            Assert.That(Parsers.UnbalancedBrackets(@"now hello {{bye}} {{now} or [[now] was", out bracketLength), Is.EqualTo(18));

            Assert.That(Parsers.UnbalancedBrackets(@"==External links==
*[http://www.transfermarkt.de/profil.html]&section=p&teamid=458 Profile] at Transfermarkt.de
*[http://www.vi.nl/Spelers
", out bracketLength), Is.EqualTo(115));
            Assert.That(bracketLength, Is.EqualTo(1));

            Assert.That(Parsers.UnbalancedBrackets(@"{{Infobox|foo=bar (OMG} }}", out bracketLength), Is.EqualTo(0));

            Assert.That(Parsers.UnbalancedBrackets(@"now [[link],] at", out bracketLength), Is.EqualTo(4));
            Assert.That(bracketLength, Is.EqualTo(2));
        }

        [Test]
        public void UnbalancedBracketsNone()
        {
            int bracketLength = 0;

            // brackets all okay
            Assert.That(Parsers.UnbalancedBrackets(@"now hello {{bye}} {{now}}", out bracketLength), Is.EqualTo(-1));
            Assert.That(Parsers.UnbalancedBrackets(@"now hello [[bye]] {{now}}", out bracketLength), Is.EqualTo(-1));
            Assert.That(Parsers.UnbalancedBrackets(@"now hello {{bye}} [now]", out bracketLength), Is.EqualTo(-1));
            Assert.That(Parsers.UnbalancedBrackets(@"now hello", out bracketLength), Is.EqualTo(-1));
            Assert.That(Parsers.UnbalancedBrackets(@"<ref>[http://www.pubmedcentral.nih.gov/articlerender.fcgi?artid=32159 Message to
complementary and alternative medicine: evidence is a better friend than power. Andrew J Vickers]</ref>", out bracketLength), Is.EqualTo(-1));
            Assert.That(Parsers.UnbalancedBrackets(@"[http://www.site.com a link [cool&#93;]", out bracketLength), Is.EqualTo(-1)); // displays as valid syntax
            Assert.That(Parsers.UnbalancedBrackets(@"[http://www.site.com a link &#91;cool&#93; here]", out bracketLength), Is.EqualTo(-1)); // displays as valid syntax
            Assert.That(Parsers.UnbalancedBrackets(@"*[http://external.oneonta.edu/html A&#91;lbert&#93; T. 1763]", out bracketLength), Is.EqualTo(-1));

            // don't consider stuff in <math> or <pre> tags etc.
            Assert.That(Parsers.UnbalancedBrackets(@"now hello {{bye}} <pre>{now}}</pre>", out bracketLength), Is.EqualTo(-1));
            Assert.That(Parsers.UnbalancedBrackets(@"now hello {{bye}} <math>{a{b}}</math>", out bracketLength), Is.EqualTo(-1));
            Assert.That(Parsers.UnbalancedBrackets(@"now hello {{bye}} <code>{now}}</code>", out bracketLength), Is.EqualTo(-1));
            // ignore in certain templates
            Assert.That(Parsers.UnbalancedBrackets(@"now hello {{LSJ|foo(bar}}", out bracketLength), Is.EqualTo(-1));
            Assert.That(Parsers.UnbalancedBrackets(@"now hello {{)!}}", out bracketLength), Is.EqualTo(-1));
            Assert.That(Parsers.UnbalancedBrackets(@"now hello {{!(}}", out bracketLength), Is.EqualTo(-1));
            Assert.That(Parsers.UnbalancedBrackets(@"now hello {{Lisp2|foo(bar}}", out bracketLength), Is.EqualTo(-1));
            Assert.That(Parsers.UnbalancedBrackets(@"now hello {{C sharp|foo(bar}}", out bracketLength), Is.EqualTo(-1));
        }

        [Test]
        public void UnbalancedTags()
        {
            int bracketLength = 0;

            // unbalanced tags
            Assert.That(Parsers.UnbalancedBrackets(@"now <b>hello /b>", out bracketLength), Is.EqualTo(15));
            Assert.That(bracketLength, Is.EqualTo(1));

            Assert.That(Parsers.UnbalancedBrackets(@"<a>asdf</a> now <b>hello /b>", out bracketLength), Is.EqualTo(27));
            Assert.That(bracketLength, Is.EqualTo(1));

            // not unbalanced
            Assert.That(Parsers.UnbalancedBrackets(@"now was < 50 cm long", out bracketLength), Is.EqualTo(-1));
        }

        [Test]
        public void TestWordingIntoBareExternalLinks()
        {
            Assert.That(Parsers.FixSyntax(@"<ref>B'er Chayim Temple, National Park Service, [ http://www.nps.gov/history/nr/travel/cumberland/ber.htm]</ref>"), Is.EqualTo(@"<ref>[http://www.nps.gov/history/nr/travel/cumberland/ber.htm B'er Chayim Temple, National Park Service]</ref>"));

            // don't catch two bare links
            Assert.That(Parsers.FixSyntax(@"<ref>[http://www.nps.gov/history] [http://www.nps.gov/history/nr/travel/cumberland/ber.htm]</ref>"), Is.EqualTo(@"<ref>[http://www.nps.gov/history] [http://www.nps.gov/history/nr/travel/cumberland/ber.htm]</ref>"));
        }

        private static readonly System.Globalization.CultureInfo BritishEnglish = new System.Globalization.CultureInfo("en-GB");

        [Test]
        public void FixSyntaxSubstRefTags()
        {
            Assert.IsFalse(Parsers.FixSyntax(@"<ref>{{cite web | title=foo| url=http://www.site.com }} {{dead link|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}</ref>").Contains(@"{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}"), "subst converted within ref tags");
            Assert.That(Parsers.FixSyntax(@"<ref>{{cite web | title=foo| url=http://www.site.com }} {{dead link|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}</ref>"), Is.EqualTo(@"<ref>{{cite web | title=foo| url=http://www.site.com }} {{dead link|date=" + System.DateTime.UtcNow.ToString("MMMM yyyy", BritishEnglish) + @"}}</ref>"));
            Assert.IsTrue(Parsers.FixSyntax(@"* {{cite web | title=foo| url=http://www.site.com }} {{dead link|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}").Contains(@"{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}"), "subst not converted when outside ref tags");

            Assert.That(Parsers.FixSyntax(@"<gallery>
 Foo.JPG |Foo great{{citation needed|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}
</gallery>"), Is.EqualTo(@"<gallery>
 Foo.JPG |Foo great{{citation needed|date=" + System.DateTime.UtcNow.ToString("MMMM yyyy", BritishEnglish) + @"}}
</gallery>"), "subst converted within gallery tags");
        }

        [Test]
        public void FixSyntaxTemplateNamespace()
        {
            Assert.That(Parsers.FixSyntax(@"{{Template:foo}}"), Is.EqualTo(@"{{foo}}"));
            Assert.That(Parsers.FixSyntax(@"{{ Template:foo}}"), Is.EqualTo(@"{{ foo}}"));
            Assert.That(Parsers.FixSyntax(@"{{ template :foo}}"), Is.EqualTo(@"{{ foo}}"));
            Assert.That(Parsers.FixSyntax(@"{{
Template:foo}}"), Is.EqualTo(@"{{
foo}}"));
            Assert.That(Parsers.FixSyntax(@"{{Template:foo
|bar=yes}}"), Is.EqualTo(@"{{foo
|bar=yes}}"));
        }

        [Test]
        public void FixSyntaxExternalLinkBrackets()
        {
            Assert.That(Parsers.FixSyntax("[http://example.com] site"), Is.EqualTo("[http://example.com] site"));
            Assert.That(Parsers.FixSyntax("[[http://example.com] site2"), Is.EqualTo("[http://example.com] site2"));
            Assert.That(Parsers.FixSyntax("[[[http://example.com] site3"), Is.EqualTo("[http://example.com] site3"));
            Assert.That(Parsers.FixSyntax("[http://example.com]] site4"), Is.EqualTo("[http://example.com] site4"));
            Assert.That(Parsers.FixSyntax("[[http://example.com]] site5"), Is.EqualTo("[http://example.com] site5"));
            Assert.That(Parsers.FixSyntax("[[[http://example.com]]] site6"), Is.EqualTo("[http://example.com] site6"));
            Assert.That(Parsers.FixSyntax(Parsers.FixLinkWhitespace("[[ http://example.com]] site7", "Test")), Is.EqualTo("[http://example.com] site7"));
            Assert.That(Parsers.FixSyntax(@"[[http://example.com
* List 2"), Is.EqualTo(@"[http://example.com]
* List 2"));

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_10#second_pair_of_brackets_added_to_https_links
            Assert.That(Parsers.FixSyntax("[https://example.com]] site"), Is.EqualTo("[https://example.com] site"));
            Assert.That(Parsers.FixSyntax("[[https://example.com] site"), Is.EqualTo("[https://example.com] site"));

            Assert.That(Parsers.FixSyntax("<ref>[http://test.com}</ref>"), Is.EqualTo("<ref>[http://test.com]</ref>"));
            Assert.That(Parsers.FixSyntax("<ref>[http://test.com</ref>"), Is.EqualTo("<ref>[http://test.com]</ref>"));

            Assert.That(Parsers.FixSyntax("<ref>[http://test.com foo" + "\r\n" +
                                                                                      @"bar</ref>"), Is.EqualTo("<ref>[http://test.com foo bar]</ref>"));

            Assert.That(Parsers.FixSyntax("{{cite web | url = http:http://test.com |title=a }}"), Is.EqualTo("{{cite web | url = http://test.com |title=a }}"));
            Assert.That(Parsers.FixSyntax(@"* [http://www.site.com text)"), Is.EqualTo(@"* [http://www.site.com text]"));
            Assert.That(Parsers.FixSyntax(@"* [http://www.site.com text) "), Is.EqualTo(@"* [http://www.site.com text]"));

            const string Correct = @"* [http://www.site.com text (here) there]";
            Assert.That(Parsers.FixSyntax(Correct), Is.EqualTo(Correct));
        }

        [Test]
        public void FixSyntaxExternalLinks()
        {
            Assert.That(Parsers.FixSyntax("[http://test.com]"), Is.EqualTo("[http://test.com]"), "do nothing if everything is OK");
            Assert.That(Parsers.FixSyntax("[http://http://test.com]"), Is.EqualTo("[http://test.com]"), "double http");
            Assert.That(Parsers.FixSyntax("[http:// http://test.com]"), Is.EqualTo("[http://test.com]"), "double http");
            Assert.That(Parsers.FixSyntax("[http:// www.test.com]"), Is.EqualTo("[http://www.test.com]"), "http www spacing");
            Assert.That(Parsers.FixSyntax("[https:// www.test.com]"), Is.EqualTo("[https://www.test.com]"), "https www spacing");
            Assert.That(Parsers.FixSyntax("[http%3A//www.test.com]"), Is.EqualTo("[http://www.test.com]"), "http colon encoding");
            Assert.That(Parsers.FixSyntax("[https%3A//www.test.com]"), Is.EqualTo("[https://www.test.com]"), "https colon encoding");
            Assert.That(Parsers.FixSyntax("[http:http://test.com]"), Is.EqualTo("[http://test.com]"), "double http with first lacking slashes");
            Assert.That(Parsers.FixSyntax("[http://http://http://test.com]"), Is.EqualTo("[http://test.com]"), "more than two");
            Assert.That(Parsers.FixSyntax("[https://https://test.com]"), Is.EqualTo("[https://test.com]"), "double https");
            Assert.That(Parsers.FixSyntax("[ftp://ftp://test.com]"), Is.EqualTo("[ftp://test.com]"), "double ftp");
            Assert.That(Parsers.FixSyntax("[ftp://ftp://ftp://test.com]"), Is.EqualTo("[ftp://test.com]"), "triple ftp");

            Assert.That(Parsers.FixSyntax("{{url|http://http://test.com}}"), Is.EqualTo("{{url|http://test.com}}"), "double http inside url template");
            Assert.That(Parsers.FixSyntax("{{official website|http://http://test.com}}"), Is.EqualTo("{{official website|http://test.com}}"), "double http inside official website template");
            Assert.That(Parsers.FixSyntax("{{foo|http://http://test.com}}"), Is.EqualTo("{{foo|http://http://test.com}}"), "no change in a random template");

            Assert.That(Parsers.FixSyntax("[[Image:http://test.com/a.png]]"), Is.EqualTo("[http://test.com/a.png]"), "Image http 1");
            Assert.That(Parsers.FixSyntax("[Image:http://test.com/a.png]"), Is.EqualTo("[http://test.com/a.png]"), "Image http 2");

            Assert.That(Parsers.FixSyntax("[https:http://test.com]"), Is.EqualTo("[https://test.com]"), "https and http");
        }
 
        [Test]
        public void TestFixSyntaxDisambigCat()
        {
            Assert.That(Parsers.FixSyntax(@"Foo [[Category:Disambiguation pages]]"), Is.EqualTo(@"Foo {{Disambiguation}}"), "Cat to template");
            Assert.That(Parsers.FixSyntax(@"Foo {{Disambiguation}}
[[Category:Disambiguation pages]]"), Is.EqualTo(@"Foo {{Disambiguation}}"), "Remove cat when template also present");
        }

        [Test]
        public void TestFixSyntaxRemoveEmptyTags()
        {
            Assert.That(Parsers.FixSyntax(@"<gallery></gallery>"), Is.Empty);
            Assert.That(Parsers.FixSyntax(@"<gallery>
</gallery>"), Is.Empty);
            Assert.That(Parsers.FixSyntax(@"<gallery>   </gallery>"), Is.Empty);
            Assert.That(Parsers.FixSyntax(@"< Gallery >   </gallery>"), Is.Empty);
            Assert.That(Parsers.FixSyntax(@"< GALLERY >   </gallery>"), Is.Empty);
            Assert.That(Parsers.FixSyntax(@"<gallery mode=a></gallery>"), Is.Empty);
            Assert.That(Parsers.FixSyntax(@"<gallery mode=a foo=b></gallery>"), Is.Empty);
            Assert.That(Parsers.FixSyntax(@"<blockquote mode=a foo=b></blockquote>"), Is.Empty);
            Assert.That(Parsers.FixSyntax(@"<center mode=a foo=b></center>"), Is.Empty);
            Assert.That(Parsers.FixSyntax(@"<sup mode=a foo=b></sup>"), Is.Empty);
            Assert.That(Parsers.FixSyntax(@"<sub mode=a foo=b></sub>"), Is.Empty);

            const string Gallery = @"<gallery>Image1.jpeg</gallery>";

            Assert.That(Parsers.FixSyntax(Gallery), Is.EqualTo(Gallery));
            Assert.That(Parsers.FixSyntax(@"<gallery> <gallery><gallery>   </gallery></gallery></gallery>"), Is.Empty, "Cleans nested empty gallery tags");
            Assert.That(Parsers.FixSyntax(@"<center></center>"), Is.Empty);
            Assert.That(Parsers.FixSyntax(@"<gallery> <center></center> </gallery>"), Is.Empty);
            Assert.That(Parsers.FixSyntax(@"<blockquote></blockquote>"), Is.Empty);
            Assert.That(Parsers.FixSyntax(@"<sup></sup>"), Is.Empty);
            Assert.That(Parsers.FixSyntax(@"<sub></sub>"), Is.Empty);
            Assert.That(Parsers.FixSyntax(@"[[hello]] <sub></sub>and<sub></sub> [[bye]]<sub></sub>"), Is.EqualTo(@"[[hello]] and [[bye]]"));
        }

        [Test]
        public void FixSyntaxHTMLTags()
        {
            Assert.That(Parsers.FixSyntax("<b>foo</b> bar"), Is.EqualTo("'''foo''' bar"));
            Assert.That(Parsers.FixSyntax("<B>foo</B> bar"), Is.EqualTo("'''foo''' bar"));
            Assert.That(Parsers.FixSyntax("< b >foo</b> bar"), Is.EqualTo("'''foo''' bar"));
            Assert.That(Parsers.FixSyntax("<b>foo< /b > bar"), Is.EqualTo("'''foo''' bar"));
            Assert.That(Parsers.FixSyntax("<b>foo<b> bar"), Is.EqualTo("<b>foo<b> bar"));
            Assert.That(Parsers.FixSyntax("<b>foo</b><b>bar</b>"), Is.EqualTo("'''foobar'''"));

            Assert.That(Parsers.FixSyntax("<i>foo</i> bar"), Is.EqualTo("''foo'' bar"));
            Assert.That(Parsers.FixSyntax("< i >foo</i> bar"), Is.EqualTo("''foo'' bar"));
            Assert.That(Parsers.FixSyntax("< i >foo< / i   > bar"), Is.EqualTo("''foo'' bar"));
            Assert.That(Parsers.FixSyntax("<i>foo< /i > bar"), Is.EqualTo("''foo'' bar"));
            Assert.That(Parsers.FixSyntax("<i>foo<i /> bar"), Is.EqualTo("''foo'' bar"));
            Assert.That(Parsers.FixSyntax(@"<i>foo<i\> bar"), Is.EqualTo("''foo'' bar"));
            Assert.That(Parsers.FixSyntax("<i>foo<i> bar"), Is.EqualTo("<i>foo<i> bar"));
            Assert.That(Parsers.FixSyntax("<i>foo</i><i>bar</i>"), Is.EqualTo("''foobar''"));
            Assert.That(Parsers.FixSyntax("<i>foo<i/><i>bar</i>"), Is.EqualTo("''foobar''"));
            Assert.That(Parsers.FixSyntax("<b><i>foo</i></b> bar"), Is.EqualTo("'''''foo''''' bar"));

            const string EmTemplate = @"{{em|foo}}";
            Assert.That(Parsers.FixSyntax(EmTemplate), Is.EqualTo(EmTemplate));
        }

        [Test]
        public void FixSyntaxStrike()
        {
            Assert.That(Parsers.FixSyntax(@"<strike>hello</strike>"), Is.EqualTo("<s>hello</s>"));
        }

        [Test]
        public void ExtraBracketInExternalLink()
        {
            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_9#Bug_in_regex_to_correct_double_bracketed_external_links
            const string valid = "now [http://www.site.com a [[a]] site] was";
            Assert.That(Parsers.FixSyntax(valid), Is.EqualTo(valid));  // valid syntax
            Assert.That(Parsers.FixSyntax("now [http://www.site.com a b site]] was"), Is.EqualTo("now [http://www.site.com a b site] was"));
            Assert.That(Parsers.FixSyntax("now [[http://www.site.com a c site] was"), Is.EqualTo("now [http://www.site.com a c site] was"));
            Assert.That(Parsers.FixSyntax("now [[http://www.site.com a d3 site]] was"), Is.EqualTo("now [http://www.site.com a d3 site] was"));
            Assert.That(Parsers.FixSyntax("now [[Image:Fred1211212.JPG| here [http://www.site.com a [[e]] site]]] was"), Is.EqualTo("now [[Image:Fred1211212.JPG| here [http://www.site.com a [[e]] site]]] was"));   // valid wiki syntax
            Assert.That(Parsers.FixSyntax("now [[Image:Fred12.JPG| here [[http://www.site.com a g site]]] was"), Is.EqualTo("now [[Image:Fred12.JPG| here [http://www.site.com a g site]]] was"));
            Assert.That(Parsers.FixSyntax("now [http://www.site.com a [[b]] site]] was"), Is.EqualTo("now [http://www.site.com a [[b]] site] was"));
            Assert.That(Parsers.FixSyntax("now [[http://www.site.com a [[c]] site] was"), Is.EqualTo("now [http://www.site.com a [[c]] site] was"));
            Assert.That(Parsers.FixSyntax("now [[http://www.site.com a [[d]] or [[d2]] site]] was"), Is.EqualTo("now [http://www.site.com a [[d]] or [[d2]] site] was"));
            Assert.That(Parsers.FixSyntax("now [[Image:Fred12.JPG| here [http://www.site.com a [[f]] site]]]] was"), Is.EqualTo("now [[Image:Fred12.JPG| here [http://www.site.com a [[f]] site]]] was"));
            Assert.That(Parsers.FixSyntax("now [[Image:Fred12.JPG| here [http://www.site.com a g site]]]] was"), Is.EqualTo("now [[Image:Fred12.JPG| here [http://www.site.com a g site]]] was"));
            Assert.That(Parsers.FixSyntax("now [[Image:Fred12.JPG| here [[http://www.site.com a g site]]]] was"), Is.EqualTo("now [[Image:Fred12.JPG| here [http://www.site.com a g site]]] was"));
            Assert.That(Parsers.FixSyntax("[[Image:foo.jpg|Some [[http://some_crap.com]]]]"), Is.EqualTo("[[Image:foo.jpg|Some [http://some_crap.com]]]"));
            Assert.That(Parsers.FixSyntax("[[File:foo.jpg|Some [[https://some_crap.com]]]]"), Is.EqualTo("[[File:foo.jpg|Some [https://some_crap.com]]]"));
        }

        [Test]
        public void CiteTemplateWithSquareBracketsTests()
        {
            Assert.That(Parsers.FixSyntax(@"<ref name=Test>[[cite web|url=http://foo.com|accessdate=2015-07-07]]</ref>"), Is.EqualTo(@"<ref name=Test>{{cite web|url=http://foo.com|accessdate=2015-07-07}}</ref>"));
            Assert.That(Parsers.FixSyntax(@"<ref>[[cite web|url=http://foo.com|accessdate=2015-07-07]]</ref>"), Is.EqualTo(@"<ref>{{cite web|url=http://foo.com|accessdate=2015-07-07}}</ref>"));
            Assert.That(Parsers.FixSyntax(@"<ref>[[cite web | url = http://collection.whitney.org/object/11823 | accessdate: 25 June 2015 | publisher = [[Whitney Museum of American Art]] | title = Houston, Texas, 1977 from Women are Better than Men]].</ref>"), Is.EqualTo(@"<ref>[[cite web | url = http://collection.whitney.org/object/11823 | accessdate: 25 June 2015 | publisher = [[Whitney Museum of American Art]] | title = Houston, Texas, 1977 from Women are Better than Men]].</ref>"));
        }

        [Test]
        public void SquareBracketsInExternalLinksTests()
        {
            Assert.That(Parsers.FixSyntax(@"[http://www.site.com some stuff [great]]"), Is.EqualTo(@"[http://www.site.com some stuff &#91;great&#93;]"));
            Assert.That(Parsers.FixSyntax(@"[http://www.site.com some stuff [great] free]"), Is.EqualTo(@"[http://www.site.com some stuff &#91;great&#93; free]"));
            Assert.That(Parsers.FixSyntax(@"[http://www.site.com some stuff [great] free [here]]"), Is.EqualTo(@"[http://www.site.com some stuff &#91;great&#93; free &#91;here&#93;]"));
            Assert.That(Parsers.FixSyntax(@"[http://www.site.com some stuff [great] free [[now]]]"), Is.EqualTo(@"[http://www.site.com some stuff &#91;great&#93; free [[now]]]"));

            Assert.That(Parsers.FixSyntax(@"*This: [http://www.privacyinternational.org/article.shtml?cmd[347]=x-347-359656&als[theme]=AT+Law+and+Policy Terrorism Profile - Uganda] Privacy International"),
                            Is.EqualTo(@"*This: [http://www.privacyinternational.org/article.shtml?cmd&#91;347&#93;=x-347-359656&als&#91;theme&#93;=AT+Law+and+Policy Terrorism Profile - Uganda] Privacy International"));

            // no delinking needed
            Assert.That(Parsers.FixSyntax(@"[http://www.site.com some great stuff]"), Is.EqualTo(@"[http://www.site.com some great stuff]"));
            Assert.That(Parsers.FixSyntax(@"now [http://www.site.com some great stuff] here"), Is.EqualTo(@"now [http://www.site.com some great stuff] here"));
            Assert.That(Parsers.FixSyntax(@"[http://www.site.com some [[great]] stuff]"), Is.EqualTo(@"[http://www.site.com some [[great]] stuff]"));
            Assert.That(Parsers.FixSyntax(@"[http://www.site.com some great [[stuff]]]"), Is.EqualTo(@"[http://www.site.com some great [[stuff]]]"));
            Assert.That(Parsers.FixSyntax(@"[http://www.site.com [[some great stuff|loads of stuff]]]"), Is.EqualTo(@"[http://www.site.com [[some great stuff|loads of stuff]]]"));
            Assert.That(Parsers.FixSyntax(@"[http://www.site.com [[some great stuff|loads of stuff]] here]"), Is.EqualTo(@"[http://www.site.com [[some great stuff|loads of stuff]] here]"));

            // don't match beyond </ref> tags
            Assert.That(Parsers.FixSyntax(@"[http://www.site.com some </ref> [stuff] here]"), Is.EqualTo(@"[http://www.site.com some </ref> [stuff] here]"));

            // exception handling
            Assert.That(Parsers.FixSyntax(@"* [http://www.site.com some stuff
* [http://site2.com stuff 2]
Other
]"), Is.EqualTo(@"* [http://www.site.com some stuff
* [http://site2.com stuff 2]
Other
]"));
        }

        [Test]
        public void FixDeadlinkOutsideRef()
        {
            Assert.That(Parsers.FixSyntax(@"<ref>foo</ref> {{dead link|date=July 2014}} boo"), Is.EqualTo("<ref>foo {{dead link|date=July 2014}}</ref> boo"), "only {{dead link}} taken inside ref");
            Assert.That(Parsers.FixSyntax(@"<ref>foo</ref> {{Dead link | date=July 2014 }} boo"), Is.EqualTo("<ref>foo {{Dead link | date=July 2014 }}</ref> boo"), "only {{dead link}} taken inside ref");
            Assert.That(Parsers.FixSyntax(@"<ref>foo</ref> {{Dead link|date=July 2014}} boo"), Is.EqualTo("<ref>foo {{Dead link|date=July 2014}}</ref> boo"), "only {{dead link}} taken inside ref");
            Assert.That(Parsers.FixSyntax(@"<ref>{{cite web | url=http://www.site.com/article100.html | title=Foo }}</ref> {{dead link|date=July 2014}}"), Is.EqualTo("<ref>{{cite web | url=http://www.site.com/article100.html | title=Foo }} {{dead link|date=July 2014}}</ref>"), "{{dead link}} taken inside ref");
        }

        [Test]
        public void FixImagesBr()
        {
            Assert.That(Parsers.FixSyntax(@"[[File:Foo.png|description<br>]]"), Is.EqualTo(@"[[File:Foo.png|description]]"));
            Assert.That(Parsers.FixSyntax(@"[[File:Foo.png|description<br >]]"), Is.EqualTo(@"[[File:Foo.png|description]]"));
            Assert.That(Parsers.FixSyntax(@"[[File:Foo.png|description<br/>]]"), Is.EqualTo(@"[[File:Foo.png|description]]"));
            Assert.That(Parsers.FixSyntax(@"[[File:Foo.png|description<BR>]]"), Is.EqualTo(@"[[File:Foo.png|description]]"));
            Assert.That(Parsers.FixSyntax(@"[[File:Foo.png|description<br />]]"), Is.EqualTo(@"[[File:Foo.png|description]]"));
            Assert.That(Parsers.FixSyntax(@"[[File:Foo.png|description<br />
]]"), Is.EqualTo(@"[[File:Foo.png|description]]"));

            const string nochange1 = @"[[File:Foo.png|description<br>here]]";
            Assert.That(Parsers.FixSyntax(nochange1), Is.EqualTo(nochange1));
        }

        [Test]
        public void FixSyntaxDEFAULTSORT()
        {
            Assert.That(Parsers.FixSyntax(@"{{DEFAULTSORT:
Foo}}"), Is.EqualTo(@"{{DEFAULTSORT:Foo}}"));

            Assert.That(Parsers.FixSyntax(@"{{DEFAULTSORT: Foo }}"), Is.EqualTo(@"{{DEFAULTSORT:Foo}}"));
            Assert.That(Parsers.FixSyntax(@"{{DEFAULTSORT:Foo
}}"), Is.EqualTo(@"{{DEFAULTSORT:Foo}}"));
            Assert.That(Parsers.FixSyntax(@"{{DEFAULTSORT:Jones,Andy}}"), Is.EqualTo(@"{{DEFAULTSORT:Jones, Andy}}"), "Add space after comma");
            Assert.That(Parsers.FixSyntax(@"{{DEFAULTSORT:Jones,}}"), Is.EqualTo(@"{{DEFAULTSORT:Jones,}}"), "No change: comma at end");
        }
        
        [Test]
        public void FixSyntaxFontTags()
        {
            Assert.That(Parsers.FixSyntax(@"<font>hello</font>"), Is.EqualTo("hello"));
            Assert.That(Parsers.FixSyntax(@"<font>hello</FONT>"), Is.EqualTo("hello"));
            Assert.That(Parsers.FixSyntax(@"<font>hello
world</font>"), Is.EqualTo(@"hello
world"));

            // only changing font tags without properties
            Assert.That(Parsers.FixSyntax(@"<font name=ab>hello</font>"), Is.EqualTo(@"<font name=ab>hello</font>"));
        }
        
       [Test]
        public void FixSyntaxHTTPFormat()
        {
            Assert.That(Parsers.FixSyntax(@"<ref>http//www.site.com</ref>"), Is.EqualTo("<ref>http://www.site.com</ref>"), "missing colon");
            Assert.That(Parsers.FixSyntax(@"<ref>https//www.site.com</ref>"), Is.EqualTo("<ref>https://www.site.com</ref>"), "missing colon");
            Assert.That(Parsers.FixSyntax(@"<ref>http:://www.site.com</ref>"), Is.EqualTo("<ref>http://www.site.com</ref>"), "double colon");
            Assert.That(Parsers.FixSyntax(@"<ref>http:www.site.com</ref>"), Is.EqualTo("<ref>http://www.site.com</ref>"), "missing slashes");
            Assert.That(Parsers.FixSyntax(@"<ref>http:///www.site.com</ref>"), Is.EqualTo("<ref>http://www.site.com</ref>"), "triple slashes");
            Assert.That(Parsers.FixSyntax(@"<ref>http:////www.site.com</ref>"), Is.EqualTo("<ref>http://www.site.com</ref>"), "four slashes");
            Assert.That(Parsers.FixSyntax(@"at http//www.site.com"), Is.EqualTo("at http://www.site.com"));
            Assert.That(Parsers.FixSyntax(@"<ref>[http:/www.site.com a website]</ref>"),
                            Is.EqualTo("<ref>[http://www.site.com a website]</ref>"), "missing a slash");
            Assert.That(Parsers.FixSyntax(@"*[http//www.site.com a website]"), Is.EqualTo("*[http://www.site.com a website]"));
            Assert.That(Parsers.FixSyntax(@"|url=http//www.site.com"), Is.EqualTo("|url=http://www.site.com"));
            Assert.That(Parsers.FixSyntax(@"|url = http:/www.site.com"), Is.EqualTo("|url = http://www.site.com"));
            Assert.That(Parsers.FixSyntax(@"[http/www.site.com]"), Is.EqualTo("[http://www.site.com]"));

            // these strings should not change
            string bug1 = @"now http://members.bib-arch.org/nph-proxy.pl/000000A/http/www.basarchive.org/bswbSearch was";
            Assert.That(Parsers.FixSyntax(bug1), Is.EqualTo(bug1));

            string bug2 = @"now http://sunsite.utk.edu/math_archives/.http/contests/ was";
            Assert.That(Parsers.FixSyntax(bug2), Is.EqualTo(bug2));

            Assert.That(Parsers.FixSyntax("the HTTP/0.9 was"), Is.EqualTo("the HTTP/0.9 was"));
            Assert.That(Parsers.FixSyntax("the HTTP/1.0 was"), Is.EqualTo("the HTTP/1.0 was"));
            Assert.That(Parsers.FixSyntax("the HTTP/1.1 was"), Is.EqualTo("the HTTP/1.1 was"));
            Assert.That(Parsers.FixSyntax("the HTTP/1.2 was"), Is.EqualTo("the HTTP/1.2 was"));

            string a = @"the HTTP/FTP was";
            Assert.That(Parsers.FixSyntax(a), Is.EqualTo(a));

            Assert.That(Parsers.FixSyntax("the HTTP/1.2 protocol"), Is.EqualTo("the HTTP/1.2 protocol"));
            Assert.That(Parsers.FixSyntax(@"<ref>[http://cdiac.esd.ornl.gov/ftp/cdiac74/a.pdf chapter 5]</ref>"),
                            Is.EqualTo(@"<ref>[http://cdiac.esd.ornl.gov/ftp/cdiac74/a.pdf chapter 5]</ref>"));
        }

        [Test]
        public void FixSyntaxMagicWords()
        {
            Assert.That(Parsers.FixSyntax(@"{{Fullpagename|Foo}}"), Is.EqualTo(@"{{FULLPAGENAME:Foo}}"));
        }

        [Test]
        public void FixSyntaxMagicWordsBehaviourSwitches()
        {
            Assert.That(Parsers.FixSyntax(@"__ToC__"), Is.EqualTo(@"__TOC__"));
            Assert.That(Parsers.FixSyntax(@"__toC__"), Is.EqualTo(@"__TOC__"));
            Assert.That(Parsers.FixSyntax(@"__TOC__"), Is.EqualTo(@"__TOC__"));
            Assert.That(Parsers.FixSyntax(@"__NoToC__"), Is.EqualTo(@"__NOTOC__"));
        }

        [Test]
        public void FixSyntaxPipesInExternalLinks()
        {
            Assert.That(Parsers.FixSyntax("[http://www.site.com|''my cool site'']"), Is.EqualTo("[http://www.site.com ''my cool site'']"));
            Assert.That(Parsers.FixSyntax("[http://www.site.com/here/there.html|''my cool site'']"), Is.EqualTo("[http://www.site.com/here/there.html ''my cool site'']"));

            Assert.That(Parsers.FixSyntax(@"port [[http://www.atoc.org/general/ConnectingCommunitiesReport_S10.pdf |""Connecting Communities - Expanding Access to the Rail Network""]] consid"),
                            Is.EqualTo(@"port [http://www.atoc.org/general/ConnectingCommunitiesReport_S10.pdf ""Connecting Communities - Expanding Access to the Rail Network""] consid"));

            const string nochange1 = @"[http://www.site.com|''my cool site''", nochange2 = @"{{Infobox Singapore School
| name = Yuan Ching Secondary School
| established = 1978
| city/town = [[Jurong]]
| enrolment = over 1,300
| homepage = [http://schools.moe.edu.sg/ycss/
| border_color = #330066
| uniform_color = #66CCFF
}}";
            Assert.That(Parsers.FixSyntax(nochange1), Is.EqualTo(nochange1));
            Assert.That(Parsers.FixSyntax(nochange2), Is.EqualTo(nochange2));
        }

        [Test]
        public void FixUnbalancedBracketsChineseBrackets()
        {
            #if DEBUG
            const string CB = @"now （there) was";

            Variables.SetProjectLangCode("fr");
            Assert.That(Parsers.CiteTemplateDates(CB), Is.EqualTo(CB));

            Variables.SetProjectLangCode("en");
            Assert.That(Parsers.FixSyntax(CB), Is.EqualTo(@"now (there) was"));

            const string CB2 = @"now （there） was";
            Assert.That(Parsers.FixSyntax(CB2), Is.EqualTo(CB2), "No change when brackets are balanced");
            #endif
        }

        [Test]
        public void FixUnbalancedBracketsCiteTemplates()
        {
            Assert.That(Parsers.FixSyntax(@"<ref>{{cite web|url=a|title=b}</ref>"), Is.EqualTo(@"<ref>{{cite web|url=a|title=b}}</ref>"));
            Assert.That(Parsers.FixSyntax(@"<ref>{{cite web|url=a|title=b}]</ref>"), Is.EqualTo(@"<ref>{{cite web|url=a|title=b}}</ref>"));
            Assert.That(Parsers.FixSyntax(@"<ref>{{cite web|url=a|title=b))</ref>"), Is.EqualTo(@"<ref>{{cite web|url=a|title=b}}</ref>"));
            Assert.That(Parsers.FixSyntax(@"<ref> {{cite web|url=a|title=b} </ref>"), Is.EqualTo(@"<ref> {{cite web|url=a|title=b}} </ref>"));
            Assert.That(Parsers.FixSyntax(@"<ref name=Fred>{{cite web|url=a|title=b}</ref>"), Is.EqualTo(@"<ref name=Fred>{{cite web|url=a|title=b}}</ref>"));
            Assert.That(Parsers.FixSyntax(@"<ref name=""Fred"">{{cite web|url=a|title=b}</ref>"), Is.EqualTo(@"<ref name=""Fred"">{{cite web|url=a|title=b}}</ref>"));

            Assert.That(Parsers.FixSyntax(@"<ref>{cite web|url=a|title=b}}</ref>"), Is.EqualTo(@"<ref>{{cite web|url=a|title=b}}</ref>"));
            Assert.That(Parsers.FixSyntax(@"<ref> {cite web|url=a|title=b}} </ref>"), Is.EqualTo(@"<ref> {{cite web|url=a|title=b}} </ref>"));
            Assert.That(Parsers.FixSyntax(@"<ref name=Fred>{Cite web|url=a|title=b}}</ref>"), Is.EqualTo(@"<ref name=Fred>{{Cite web|url=a|title=b}}</ref>"));
            Assert.That(Parsers.FixSyntax(@"<ref name=""Fred"">{cite web|url=a|title=b}}</ref>"), Is.EqualTo(@"<ref name=""Fred"">{{cite web|url=a|title=b}}</ref>"));
            Assert.That(Parsers.FixSyntax(@"<ref>{citation|url=a|title=b}}</ref>"), Is.EqualTo(@"<ref>{{citation|url=a|title=b}}</ref>"));

            Assert.That(Parsers.FixSyntax(@"<ref> {Citation|title=}}"), Is.EqualTo(@"<ref> {{Citation|title=}}"));
            Assert.That(Parsers.FixSyntax(@"<ref> {cite web|title=}}"), Is.EqualTo(@"<ref> {{cite web|title=}}"));
            Assert.That(Parsers.FixSyntax(@"* {Citation|title=}}"), Is.EqualTo(@"* {{Citation|title=}}"));

            Assert.That(Parsers.FixSyntax(@"<ref>{{cite web|url=a|title=b}}}}</ref>"), Is.EqualTo(@"<ref>{{cite web|url=a|title=b}}</ref>"));
            Assert.That(Parsers.FixSyntax(@"<ref> {{cite web|url=a|title=b}}}} </ref>"), Is.EqualTo(@"<ref> {{cite web|url=a|title=b}} </ref>"));
            Assert.That(Parsers.FixSyntax(@"<ref name=Fred>{{Cite web|url=a|title=b}}}}</ref>"), Is.EqualTo(@"<ref name=Fred>{{Cite web|url=a|title=b}}</ref>"));
            Assert.That(Parsers.FixSyntax(@"<ref name=""Fred"">{{cite web|url=a|title=b}}}}</ref>"), Is.EqualTo(@"<ref name=""Fred"">{{cite web|url=a|title=b}}</ref>"));
            Assert.That(Parsers.FixSyntax(@"<ref>{{citation|url=a|title=b}}}}</ref>"), Is.EqualTo(@"<ref>{{citation|url=a|title=b}}</ref>"));
            Assert.That(Parsers.FixSyntax(@"<ref>{{cite web|url=a|title=b {{rp}}}}</ref>"), Is.EqualTo(@"<ref>{{cite web|url=a|title=b {{rp}}}}</ref>"));

            Assert.That(Parsers.FixSyntax(@"<ref>{[cite web|url=a|title=b}}</ref>"), Is.EqualTo(@"<ref>{{cite web|url=a|title=b}}</ref>"));
            Assert.That(Parsers.FixSyntax(@"<ref> {[cite web|url=a|title=b}} </ref>"), Is.EqualTo(@"<ref> {{cite web|url=a|title=b}} </ref>"));
            Assert.That(Parsers.FixSyntax(@"<ref name=Fred>{[Cite web|url=a|title=b}}</ref>"), Is.EqualTo(@"<ref name=Fred>{{Cite web|url=a|title=b}}</ref>"));
            Assert.That(Parsers.FixSyntax(@"<ref name=""Fred"">{[cite web|url=a|title=b}}</ref>"), Is.EqualTo(@"<ref name=""Fred"">{{cite web|url=a|title=b}}</ref>"));
            Assert.That(Parsers.FixSyntax(@"<ref>{[citation|url=a|title=b}}</ref>"), Is.EqualTo(@"<ref>{{citation|url=a|title=b}}</ref>"));

            Assert.That(Parsers.FixSyntax(@"<ref>{{cite web|url=a|title=b{}</ref>"), Is.EqualTo(@"<ref>{{cite web|url=a|title=b}}</ref>"));
            Assert.That(Parsers.FixSyntax(@"<ref> {{cite web|url=a|title=b}] </ref>"), Is.EqualTo(@"<ref> {{cite web|url=a|title=b}} </ref>"));
            Assert.That(Parsers.FixSyntax(@"<ref name=Fred>{{Cite web|url=a|title=[[b]]}</ref>"), Is.EqualTo(@"<ref name=Fred>{{Cite web|url=a|title=[[b]]}}</ref>"));
            Assert.That(Parsers.FixSyntax(@"<ref name=""Fred"">{{cite web|url=a|title=b{}</ref>"), Is.EqualTo(@"<ref name=""Fred"">{{cite web|url=a|title=b}}</ref>"));
            Assert.That(Parsers.FixSyntax(@"<ref>{{citation|url=a|title=b}]</ref>"), Is.EqualTo(@"<ref>{{citation|url=a|title=b}}</ref>"));

            Assert.That(Parsers.FixSyntax(@"<ref>cite web|url=a|title=b}}</ref>"), Is.EqualTo(@"<ref>{{cite web|url=a|title=b}}</ref>"));
            Assert.That(Parsers.FixSyntax(@"<ref name=""Smith"">cite web|url=a|title=b}}</ref>"), Is.EqualTo(@"<ref name=""Smith"">{{cite web|url=a|title=b}}</ref>"));
            Assert.That(Parsers.FixSyntax(@"<ref name=""Smith"">cite web|
url=a|title=b}}</ref>"), Is.EqualTo(@"<ref name=""Smith"">{{cite web|
url=a|title=b}}</ref>"));
            Assert.That(Parsers.FixSyntax(@"<ref>cite book|url=a|title=b}}</ref>"), Is.EqualTo(@"<ref>{{cite book|url=a|title=b}}</ref>"));
            Assert.That(Parsers.FixSyntax(@"<ref>Citation|url=a|title=b}}</ref>"), Is.EqualTo(@"<ref>{{Citation|url=a|title=b}}</ref>"));
            Assert.That(Parsers.FixSyntax(@"<ref>((cite web|url=a|title=b}}</ref>"), Is.EqualTo(@"<ref>{{cite web|url=a|title=b}}</ref>"));

            Assert.That(Parsers.FixSyntax(@"<ref>{{cite web|url=a|title=b}}}</ref>"), Is.EqualTo(@"<ref>{{cite web|url=a|title=b}}</ref>"), "fixes cite ending in three closing braces");
            Assert.That(Parsers.FixSyntax(@"<ref>{{cite web|url=a|title=b}}}
			</ref>"), Is.EqualTo(@"<ref>{{cite web|url=a|title=b}}
			</ref>"), "fixes cite ending in three closing braces, newline before ref end");

            Assert.That(Parsers.FixSyntax(@"<ref>{{{cite web|url=a|title=b}}</ref>"), Is.EqualTo(@"<ref>{{cite web|url=a|title=b}}</ref>"), "fixes cite starting in three opening braces");
            Assert.That(Parsers.FixSyntax(@"<ref name=""foo"">{{{cite web|url=a|title=b}}</ref>"), Is.EqualTo(@"<ref name=""foo"">{{cite web|url=a|title=b}}</ref>"), "fixes cite starting in three opening braces");

            string CiteDeadLink = @"<ref>{{cite web|url=a|title=b}} {{dead link|date=May 2011}}</ref>";
            Assert.That(Parsers.FixSyntax(CiteDeadLink), Is.EqualTo(CiteDeadLink));

            CiteDeadLink = @"<ref>{{cite web|url=a|title=b {{dead link|date=May 2011}}}}</ref>";
            Assert.That(Parsers.FixSyntax(CiteDeadLink), Is.EqualTo(CiteDeadLink));

            string citeReprint = @"{{cite book |last=S |first=G |title=Freedom of the press {reprint) |publisher=New York, Da Capo Press |year=1971 }}";
            Assert.That(Parsers.FixSyntax(citeReprint), Is.EqualTo(citeReprint.Replace("{r", "(r")));

            citeReprint = @"{{cite book |last=S |first=G |title=Freedom of the press (in three volumes} |publisher=New York, Da Capo Press |year=1971 }}";
            Assert.That(Parsers.FixSyntax(citeReprint), Is.EqualTo(citeReprint.Replace("s}", "s)")));

            citeReprint = @"* {{cite book |last=S |first=G |title=Freedom of the press {reprint) |publisher=New York, Da Capo Press |year=1971 }}
* {{cite book |last=S |first=G |title=Freedom of the press (reprint} |publisher=New York, Da Capo Press |year=1971 }}";

            Assert.That(Parsers.FixSyntax(citeReprint), Is.EqualTo(citeReprint.Replace("t}", "t)").Replace("{r", "(r")));
        }

        [Test]
        public void FixUnbalancedBracketsGeneral()
        {
            const string CorrectCat = @"[[Category:Foo]]
[[Category:Foo2]]";

            Assert.That(Parsers.FixSyntax(@"[[Category:Foo
[[Category:Foo2]]"), Is.EqualTo(CorrectCat), "closes unclosed cats");
            Assert.That(Parsers.FixSyntax(CorrectCat), Is.EqualTo(CorrectCat));

            Assert.That(Parsers.FixSyntax(@"[[es:Foo
[[fr:Foo2]]"), Is.EqualTo(@"[[es:Foo]]
[[fr:Foo2]]"), "closes unclosed interwikis");

            const string CorrectFileLink = @"[[File:foo.jpeg|eflkjfdslkj]]";

            Assert.That(Parsers.FixSyntax(@"{{File:foo.jpeg|eflkjfdslkj]]"), Is.EqualTo(CorrectFileLink));
            Assert.That(Parsers.FixSyntax(CorrectFileLink), Is.EqualTo(CorrectFileLink));

            const string CorrectFileLink2 = @"[[Image:foo.jpeg|eflkjfdslkj]]";

            Assert.That(Parsers.FixSyntax(@"{{Image:foo.jpeg|eflkjfdslkj]]"), Is.EqualTo(CorrectFileLink2));
            Assert.That(Parsers.FixSyntax(CorrectFileLink2), Is.EqualTo(CorrectFileLink2));

            const string CorrectFileLink3 = @"[[file:foo.jpeg|eflkjfdslkj]]";

            Assert.That(Parsers.FixSyntax(@"{{file:foo.jpeg|eflkjfdslkj]]"), Is.EqualTo(CorrectFileLink3));
            Assert.That(Parsers.FixSyntax(CorrectFileLink3), Is.EqualTo(CorrectFileLink3));

            const string NoFix1 = @"==Charact==
[[Image:X.jpg|thumb
|alt=
|xx.]]

Japanese classification systemJapanese classification systemJapanese classification systemJapanese classification systemJapanese classification systemJapanese classification systemJapanese classification system

<gallery>
Image:X.JPG|Japanese classification systemJapanese classification systemJapanese classification system]]
</gallery>";

            Assert.That(Parsers.FixSyntax(NoFix1), Is.EqualTo(NoFix1));

            Assert.That(Parsers.FixSyntax(@"[ [Foo]]"), Is.EqualTo(@"[[Foo]]"), "fixes link spacing");

            Assert.That(Parsers.FixSyntax(@"[[Foo]]]]"), Is.EqualTo(@"[[Foo]]"), "fixes excess link bracketss");

            Assert.That(Parsers.FixSyntax(@"[[Foo],]"), Is.EqualTo(@"[[Foo]],"), "fixes links broken by punctuation");
            Assert.That(Parsers.FixSyntax(@"[[Foo].]"), Is.EqualTo(@"[[Foo]]."), "fixes links broken by punctuation");
            Assert.That(Parsers.FixSyntax(@"[[Foo]:]"), Is.EqualTo(@"[[Foo]]:"), "fixes links broken by punctuation");
            Assert.That(Parsers.FixSyntax(@"[[Foo] ] bar"), Is.EqualTo(@"[[Foo]]  bar"), "fixes links broken by punctuation");
            Assert.That(Parsers.FixSyntax(@"[[Foo]'']"), Is.EqualTo(@"[[Foo]]''"), "fixes links broken by punctuation");

            Assert.That(Parsers.FixLinkWhitespace(Parsers.FixSyntax(@"[panka  Smith]] (Local national)"), "Test"), Is.EqualTo(@"[[panka Smith]] (Local national)"), "bracket and whitespace fix in one");

            const string Football = @"{{Infobox football biography
| playername     = D
| image          = 
| dateofdeath    = 1940 {aged 57)<ref name=A/>
| cityofdeath    = [[S]]
| years3         = 1911–19?? }}";

            Assert.That(Parsers.FixSyntax(Football), Is.EqualTo(Football.Replace(@"{aged", @"(aged")));
            Assert.That(Parsers.FixSyntax(@"{{DEFAULTSORT:Foo
"), Is.EqualTo(@"{{DEFAULTSORT:Foo}}"), "fixes DEFAULTSORT ending");

            Assert.That(Parsers.FixSyntax(@"{{foo|par=[[Bar[[|par2=Bar2}}"), Is.EqualTo(@"{{foo|par=[[Bar]]|par2=Bar2}}"), "reversed brackets");

            Assert.That(Parsers.FixSyntax(@"{{foo|par=[{Bar]]|par2=Bar2}}"), Is.EqualTo(@"{{foo|par=[[Bar]]|par2=Bar2}}"));

            const string Unfixable1 = @"Ruth singled and Meusel [[bunt (baseball)|ed him over, but Ruth split his pants sliding into second, [[announcer|Radio announcer]] [[Graham McNamee]]";
            Assert.That(Parsers.FixSyntax(Unfixable1), Is.EqualTo(Unfixable1));

            Assert.That(Parsers.FixSyntax(@"{{DEFAULTSORT:Foo))"), Is.EqualTo(@"{{DEFAULTSORT:Foo}}"), "fixes Template )) ending");
             const string Unfixable2 = @"Ruth [[File:One.JPEG|A [http://www.site.com/a]]]
XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
Now [[Fred was";
            Assert.That(Parsers.FixSyntax(Unfixable2), Is.EqualTo(Unfixable2));

            Assert.That(Parsers.FixSyntax(Unfixable2 + @"
==Heading==
Now [[A],] was."), Is.EqualTo(Unfixable2 + @"
==Heading==
Now [[A]], was."));

            const string MI = @"{{Multiple issues|
{{autobiography|date=April 2015}}
{{notability|Biographies|date=April 2015}}
", Infobox = @"{{Infobox foo
|a1=a
|a2=b
}}";
            Assert.That(Parsers.FixSyntax(MI), Is.EqualTo(MI + "}}"));

            Assert.That(Parsers.FixSyntax(MI + "\r\n" + Infobox), Is.EqualTo(MI + "}}" + "\r\n\r\n" + Infobox));

            const string BrokenWikiLinkInExternalLink = @"[http://a.site.edu/text/ [[ABC] {{wayback|url=http://a.site.edu/text/ |date=20100522200011 }}] Words here]";
            Assert.That(Parsers.FixSyntax(BrokenWikiLinkInExternalLink), Is.EqualTo(BrokenWikiLinkInExternalLink), "No change when broken wikilink in external link");
        }

        [Test]
        public void FixUnbalancedBracketsMathSetNotation()
        {
            const string MathSet1 = @"{[0], [1], [2]}", Foo = @"Foo { ...";
            Assert.That(Parsers.FixSyntax(Foo + MathSet1), Is.EqualTo(Foo + MathSet1));
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

            Assert.That(Parsers.FixSyntax(PD), Is.EqualTo(PD.Replace(@"Paris", @"Paris}}")));
        }

        [Test]
        public void FixUnbalancedBracketsRef()
        {
            Assert.That(Parsers.FixSyntax(@"now <ref>>foo</ref>"), Is.EqualTo(@"now <ref>foo</ref>"));
            Assert.That(Parsers.FixSyntax(@"now <ref>[http://foo.com/bar/ text here[</ref>"), Is.EqualTo(@"now <ref>[http://foo.com/bar/ text here]</ref>"));

            Assert.That(Parsers.FixSyntax(@"<ref>]http://www.foo.com bar]</ref>"), Is.EqualTo(@"<ref>[http://www.foo.com bar]</ref>"));
            Assert.That(Parsers.FixSyntax(@"<ref name=A>]http://www.foo.com bar]</ref>"), Is.EqualTo(@"<ref name=A>[http://www.foo.com bar]</ref>"));
            Assert.That(Parsers.FixSyntax(@"<ref>]http://www.foo.com bar] this one</ref>"), Is.EqualTo(@"<ref>[http://www.foo.com bar] this one</ref>"));
        }

        [Test]
        public void TestCellpaddingTypo()
        {
            Assert.That(Parsers.FixSyntax(@"now {| class=""wikitable"" celpadding=2"), Is.EqualTo(@"now {| class=""wikitable"" cellpadding=2"));
            Assert.That(Parsers.FixSyntax(@"now {| class=""wikitable"" cellpading=2"), Is.EqualTo(@"now {| class=""wikitable"" cellpadding=2"));
            Assert.That(Parsers.FixSyntax(@"now {| class=""wikitable"" Celpading=2"), Is.EqualTo(@"now {| class=""wikitable"" cellpadding=2"));

            // no Matches
            Assert.That(Parsers.FixSyntax("now the cellpading of the"), Is.EqualTo("now the cellpading of the"));
            Assert.That(Parsers.FixSyntax(@"now {| class=""wikitable"" cellpadding=2"), Is.EqualTo(@"now {| class=""wikitable"" cellpadding=2"));
        }

        [Test]
        public void TestFixObsoleteBrAttributes()
        {
            Assert.That(Parsers.FixSyntax(@"<br clear=both />"), Is.EqualTo("{{clear}}"));
            Assert.That(Parsers.FixSyntax(@"<Br clear=both />"), Is.EqualTo("{{clear}}"));
            Assert.That(Parsers.FixSyntax(@"<br clear=""both"" />"), Is.EqualTo("{{clear}}"));
            Assert.That(Parsers.FixSyntax(@"<br clear=all />"), Is.EqualTo("{{clear}}"));
            Assert.That(Parsers.FixSyntax(@"<br clear=""all"" />"), Is.EqualTo("{{clear}}"));
            Assert.That(Parsers.FixSyntax(@"<br clear=left />"), Is.EqualTo("{{clear|left}}"));
            Assert.That(Parsers.FixSyntax(@"<br clear=""left"" />"), Is.EqualTo("{{clear|left}}"));
            Assert.That(Parsers.FixSyntax(@"<br clear=right />"), Is.EqualTo("{{clear|right}}"));
            Assert.That(Parsers.FixSyntax(@"<br clear=""right"" />"), Is.EqualTo("{{clear|right}}"));

            Assert.That(Parsers.FixSyntax(@"<br style=""clear:both;"" clear=""all"" />"), Is.EqualTo("{{clear}}"));
            Assert.That(Parsers.FixSyntax(@"<br style=""clear:left;"" clear=""all"" />"), Is.EqualTo("{{clear|left}}"));
            Assert.That(Parsers.FixSyntax(@"<br style=""clear:right;"" clear=""all"" />"), Is.EqualTo("{{clear|right}}"));

            Assert.That(Parsers.FixSyntax(@"<br style=""clear:both;"" />"), Is.EqualTo("{{clear}}"));
            Assert.That(Parsers.FixSyntax(@"<br style=""clear:left;"" />"), Is.EqualTo("{{clear|left}}"));
            Assert.That(Parsers.FixSyntax(@"<br style=""clear:right;"" />"), Is.EqualTo("{{clear|right}}"));

            Assert.That(Parsers.FixSyntax(@"<br style=clear:both; />"), Is.EqualTo("{{clear}}"));
            Assert.That(Parsers.FixSyntax(@"<br style=clear:left; />"), Is.EqualTo("{{clear|left}}"));
            Assert.That(Parsers.FixSyntax(@"<br style=clear:right; />"), Is.EqualTo("{{clear|right}}"));
        }

        [Test]
        public void TestFixSyntaxUnbalancedBrackets()
        {
            Assert.That(Parsers.FixSyntax(@"<ref>{{http://www.site.com}}</ref>"), Is.EqualTo(@"<ref>[http://www.site.com]</ref>"));
            Assert.That(Parsers.FixSyntax(@"<ref>{{http://www.site.com }}</ref>"), Is.EqualTo(@"<ref>[http://www.site.com]</ref>"));
            Assert.That(Parsers.FixSyntax(@"<ref>{{http://www.site.com cool site}}</ref>"), Is.EqualTo(@"<ref>[http://www.site.com cool site]</ref>"));
            Assert.That(Parsers.FixSyntax(@"<ref name=""Fred"">{{http://www.site.com}}</ref>"), Is.EqualTo(@"<ref name=""Fred"">[http://www.site.com]</ref>"));
            Assert.That(Parsers.FixSyntax(@"<ref> {{http://www.site.com}} </ref>"), Is.EqualTo(@"<ref>[http://www.site.com]</ref>"));

            Assert.That(Parsers.FixSyntax(@"{[hello}}"), Is.EqualTo(@"{{hello}}"));
            Assert.That(Parsers.FixSyntax(@"[{hello}}"), Is.EqualTo(@"{{hello}}"));
            Assert.That(Parsers.FixSyntax(@"{hello}}"), Is.EqualTo(@"{{hello}}"));

            Assert.That(Parsers.FixSyntax(@"<ref>http://site.com]</ref>"), Is.EqualTo(@"<ref>[http://site.com]</ref>"));
            Assert.That(Parsers.FixSyntax(@"<ref>[http://site.com</ref>"), Is.EqualTo(@"<ref>[http://site.com]</ref>"));
            Assert.That(Parsers.FixSyntax(@"<REF>[http://site.com</ref>"), Is.EqualTo(@"<REF>[http://site.com]</ref>"));
            Assert.That(Parsers.FixSyntax(@"<ref>Smith, 2004 http://site.com]</ref>"), Is.EqualTo(@"<ref>[http://site.com Smith, 2004]</ref>"));
            Assert.That(Parsers.FixSyntax(@"<ref> http://site.com] </ref>"), Is.EqualTo(@"<ref> [http://site.com] </ref>"));
            Assert.That(Parsers.FixSyntax(@"<ref name=Fred>http://site.com cool]</ref>"), Is.EqualTo(@"<ref name=Fred>[http://site.com cool]</ref>"));
            Assert.That(Parsers.FixSyntax(@"<ref name=""Fred"">http://site.com]</ref>"), Is.EqualTo(@"<ref name=""Fred"">[http://site.com]</ref>"));
            Assert.That(Parsers.FixSyntax(@"<ref>http://site.com great site-here]</ref>"), Is.EqualTo(@"<ref>[http://site.com great site-here]</ref>"));

            Assert.That(Parsers.FixSyntax(@"<ref>Antara News [http://www.antara.co.id/arc/2009/1/9/kri-lebanon/ KRI Lebanon</ref>"), Is.EqualTo(@"<ref>Antara News [http://www.antara.co.id/arc/2009/1/9/kri-lebanon/ KRI Lebanon]</ref>"));
            Assert.That(Parsers.FixSyntax(@"<ref>Antara News [ftp://www.antara.co.id/arc/2009/1/9/kri-lebanon/ KRI Lebanon</ref>"), Is.EqualTo(@"<ref>Antara News [ftp://www.antara.co.id/arc/2009/1/9/kri-lebanon/ KRI Lebanon]</ref>"), "handles FTP protocol");
            Assert.That(Parsers.FixSyntax(@"<ref name=Smith>Antara News [http://www.antara.co.id/arc/2009/1/9/kri-lebanon/ KRI Lebanon</ref>"), Is.EqualTo(@"<ref name=Smith>Antara News [http://www.antara.co.id/arc/2009/1/9/kri-lebanon/ KRI Lebanon]</ref>"));
            Assert.That(Parsers.FixSyntax(@"<ref name=Smith>Antara News [http://www.antara.co.id/arc/2009/1/9/kri-lebanon/ KRI Lebanon]</ref>"), Is.EqualTo(@"<ref name=Smith>Antara News [http://www.antara.co.id/arc/2009/1/9/kri-lebanon/ KRI Lebanon]</ref>"));

            // no completion of template braces on non-template ref
            const string a = @"<ref>Smith and Jones, 2005, p46}}</ref>";
            Assert.That(Parsers.FixSyntax(a), Is.EqualTo(a));

            Assert.That(Parsers.FixSyntax(@"{{DEFAULTSORT:Astaire, Fred]}}"), Is.EqualTo(@"{{DEFAULTSORT:Astaire, Fred}}"));
            Assert.That(Parsers.FixSyntax(@"{{DEFAULTSORT:Astaire, Fred]]}}"), Is.EqualTo(@"{{DEFAULTSORT:Astaire, Fred}}"));
            Assert.That(Parsers.FixSyntax(@"{{DEFAULTSORT:Astaire, Fred]]]}}"), Is.EqualTo(@"{{DEFAULTSORT:Astaire, Fred}}"));
            Assert.That(Parsers.FixSyntax(@"{{DEFAULTSORT:Astaire, Fred[]}}"), Is.EqualTo(@"{{DEFAULTSORT:Astaire, Fred}}"));

            // completes curly brackets where it would make all brackets balance
            Assert.That(Parsers.FixSyntax(@"Great.{{fact|date=May 2008} Now"), Is.EqualTo(@"Great.{{fact|date=May 2008}} Now"));
            Assert.That(Parsers.FixSyntax(@"Great.{{fact|date=May 2008]} Now"), Is.EqualTo(@"Great.{{fact|date=May 2008}} Now"));

            // don't change what could be wikitable
            Assert.That(Parsers.FixSyntax(@"Great.{{foo|} Now"), Is.EqualTo(@"Great.{{foo|} Now"));

            // set single curly bracket to normal bracket if that makes all brackets balance
            Assert.That(Parsers.FixSyntax(@"Great (not really} now"), Is.EqualTo(@"Great (not really) now"));
            Assert.That(Parsers.FixSyntax(@"# [[Herbert H. H. Fox]] ([[1934 - 1939]]}<ref>foo</ref>"), Is.EqualTo(@"# [[Herbert H. H. Fox]] ([[1934 - 1939]])<ref>foo</ref>"));

            // can't fix these
            Assert.That(Parsers.FixSyntax(@"Great { but (not really} now"), Is.EqualTo(@"Great { but (not really} now"));
            Assert.That(Parsers.FixSyntax(@"Great (not really)} now"), Is.EqualTo(@"Great (not really)} now"));

            // don't touch when it could be a table
            Assert.That(Parsers.FixSyntax(@"great (in 2001 | blah |} now"), Is.EqualTo(@"great (in 2001 | blah |} now"));

            // Fixes } to |
            string cite1 = @"Great.<ref>{{cite web | url=http://www.site.com | title=abc } year=2009}}</ref>";
            Assert.That(Parsers.FixSyntax(cite1), Is.EqualTo(@"Great.<ref>{{cite web | url=http://www.site.com | title=abc | year=2009}}</ref>"));

            cite1 = cite1.Replace("cite web", "vcite web");
            Assert.That(Parsers.FixSyntax(cite1), Is.EqualTo(@"Great.<ref>{{vcite web | url=http://www.site.com | title=abc | year=2009}}</ref>"));

            // set double round bracket to single
            Assert.That(Parsers.FixSyntax(@"then ((but often) for"), Is.EqualTo(@"then (but often) for"));

            // only applies changes if brackets then balance out
            Assert.That(Parsers.FixSyntax(@"then ((but often)) for"), Is.EqualTo(@"then ((but often)) for"));
            Assert.That(Parsers.FixSyntax(@"then ((but often)} for"), Is.EqualTo(@"then ((but often)} for"));
            Assert.That(Parsers.FixSyntax(@"then ((but often) for("), Is.EqualTo(@"then ((but often) for("));

            // an unbalanced bracket is fixed if there are enough characters until the next one to be confident that the next one is indeed
            // a separate incident
            Assert.That(Parsers.FixSyntax(@"# [[Daniel Sylvester Tuttle|Daniel S. Tuttle]], (1867 - 1887)
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
# Brian J. Thom  (2008 - Present)"), Is.EqualTo(@"# [[Daniel Sylvester Tuttle|Daniel S. Tuttle]], (1867 - 1887)
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
# Brian J. Thom  (2008 - Present)"));

            Assert.That(Parsers.FixSyntax(@"The '''Zach Feuer Gallery''' is a [[contemporary art]] gallery located in [[Chelsea, Manhattan|Chelsea]], [[New York City|New York]].

==History==

The Zach Feuer Gallery was founded in 2000 as the LFL Gallery, by Nick Lawrence, Russell LaMontagne and Zach Feuer.  It was originally located on a fourth floor space on 26th Street.    In 2002 the gallery moved to a first floor space on 24th Street, briefly sharing space with an art book gallery owned by one of the partners.  In 2004  Zach Feuer purchased the gallery from his partners and changed the gallery name to Zach Feuer Gallery.

Some artists represented by Zach Feuer Gallery are [[Phoebe Washburn]], [[Jules de Balincourt]], [[Nathalie Djurberg]]], [[Justin Lieberman]], [[Stuart Hawkins]],  [[Johannes Vanderbeek]], [[Sister Corita Kent]], [[Tamy Ben Tor]], [[Anton Henning]], [[Dana Schutz]], and [[Mark Flood]].

==External links==
* [http://www.zachfeuer.com Zach Feuer Gallery] website

{{coord missing|New York}}

[[Category:Contemporary art galleries]]
[[Category:2000 establishments]]
[[Category:Art galleries in Manhattan]]"), Is.EqualTo(@"The '''Zach Feuer Gallery''' is a [[contemporary art]] gallery located in [[Chelsea, Manhattan|Chelsea]], [[New York City|New York]].

==History==

The Zach Feuer Gallery was founded in 2000 as the LFL Gallery, by Nick Lawrence, Russell LaMontagne and Zach Feuer.  It was originally located on a fourth floor space on 26th Street.    In 2002 the gallery moved to a first floor space on 24th Street, briefly sharing space with an art book gallery owned by one of the partners.  In 2004  Zach Feuer purchased the gallery from his partners and changed the gallery name to Zach Feuer Gallery.

Some artists represented by Zach Feuer Gallery are [[Phoebe Washburn]], [[Jules de Balincourt]], [[Nathalie Djurberg]], [[Justin Lieberman]], [[Stuart Hawkins]],  [[Johannes Vanderbeek]], [[Sister Corita Kent]], [[Tamy Ben Tor]], [[Anton Henning]], [[Dana Schutz]], and [[Mark Flood]].

==External links==
* [http://www.zachfeuer.com Zach Feuer Gallery] website

{{coord missing|New York}}

[[Category:Contemporary art galleries]]
[[Category:2000 establishments]]
[[Category:Art galleries in Manhattan]]"));

            Assert.That(Parsers.FixSyntax(@"<ref>{http://www.findagrave.com/cgi-bin/fg.cgi?page=gr&GRid=5194 Find A Grave]</ref>"), Is.EqualTo(@"<ref>[http://www.findagrave.com/cgi-bin/fg.cgi?page=gr&GRid=5194 Find A Grave]</ref>"));

            // convert [[[[link]] to [[link]] if that balances it all out
            Assert.That(Parsers.FixSyntax(@"hello [[[[link]] there"), Is.EqualTo(@"hello [[link]] there"));
            Assert.That(Parsers.FixSyntax(@"hello [[[[link]] there]]"), Is.EqualTo(@"hello [[[[link]] there]]"));

            // convert {blah) to (blah) if that balances it all out, not wikitables, templates
            Assert.That(Parsers.FixSyntax(@"hello {link) there"), Is.EqualTo(@"hello (link) there"));
            Assert.That(Parsers.FixSyntax(@"hello {|table|blah) there"), Is.EqualTo(@"hello {|table|blah) there"));
            Assert.That(Parsers.FixSyntax(@"{{cite web|title=a|url=http://www.site.com|publisher=ABC)|year=2006}"), Is.EqualTo(@"{{cite web|title=a|url=http://www.site.com|publisher=ABC)|year=2006}"));

            // convert [blah} to [blah] if that balances it all out
            Assert.That(Parsers.FixSyntax(@"*[http://aeiou.visao.pt/Pages/Lusa.aspx?News=200808068620453 Obituary notice} here"), Is.EqualTo(@"*[http://aeiou.visao.pt/Pages/Lusa.aspx?News=200808068620453 Obituary notice] here"));

            // don't touch template ends
            Assert.That(Parsers.FixSyntax(@"[http://aeiou.visao.pt/Pages/Lusa.aspx?News=200808068620453 Obituary notice}}"), Is.EqualTo(@"[http://aeiou.visao.pt/Pages/Lusa.aspx?News=200808068620453 Obituary notice}}"));

            // correct [[blah blah}word]] to [[blah blah|word]]
            Assert.That(Parsers.FixSyntax(@"[[blah blah}word]]"), Is.EqualTo(@"[[blah blah|word]]"));
            Assert.That(Parsers.FixSyntax(@"[[blah}word]]"), Is.EqualTo(@"[[blah|word]]"));

            // []foo]] --> [[foo]]
            Assert.That(Parsers.FixSyntax(@"now []link]] was"), Is.EqualTo(@"now [[link]] was"));
            Assert.That(Parsers.FixSyntax(@"now []link|a]] was"), Is.EqualTo(@"now [[link|a]] was"));

            // not if unbalanced brackets remain
            Assert.That(Parsers.FixSyntax(@"{ here [[blah blah}word]]"), Is.EqualTo(@"{ here [[blah blah}word]]"));

            // correct {[link]] or {[[link]] or [[[link]] or [[{link]]
            Assert.That(Parsers.FixSyntax(@"now {[link]] was"), Is.EqualTo(@"now [[link]] was"));
            Assert.That(Parsers.FixSyntax(@"now {[[link]] was"), Is.EqualTo(@"now [[link]] was"));
            Assert.That(Parsers.FixSyntax(@"now [[[link]] was"), Is.EqualTo(@"now [[link]] was"));
            Assert.That(Parsers.FixSyntax(@"now [[{link]] was"), Is.EqualTo(@"now [[link]] was"));

            // not if unbalanced brackets remain nearby
            Assert.That(Parsers.FixSyntax(@"now {[link]]} was"), Is.EqualTo(@"now {[[link]]} was"));
            Assert.That(Parsers.FixSyntax(@"now [[[link]] was]"), Is.EqualTo(@"now [[[link]] was]"));

            // convert [[link]]]] to [[link]] IFF that balances it all out
            Assert.That(Parsers.FixSyntax(@"hello [[link]]]] there"), Is.EqualTo(@"hello [[link]] there"));
            Assert.That(Parsers.FixSyntax(@"[[hello [[link]]]] there"), Is.EqualTo(@"[[hello [[link]]]] there"));

            // Unbalanced bracket and double pipe [[foo||bar] inside a table
            Assert.That(Parsers.FixSyntax(@"{|
			|-
			| [[foo||bar]
			|}"), Is.EqualTo(@"{|
			|-
			| [[foo|bar]]
			|}"));

            // external links missing brackets
            Assert.That(Parsers.FixSyntax(@"blah
* [http://www.site.com a site
* [http://www.site2.com another]"), Is.EqualTo(@"blah
* [http://www.site.com a site]
* [http://www.site2.com another]"));
            Assert.That(Parsers.FixSyntax(@"blah
* http://www.site.com a site]
* [http://www.site2.com another]"), Is.EqualTo(@"blah
* [http://www.site.com a site]
* [http://www.site2.com another]"));

            // template missing opening {
            Assert.That(Parsers.FixSyntax(@"now ({lang-el|foo}}) was"), Is.EqualTo(@"now ({{lang-el|foo}}) was"));

            const string table = @"{| class=""wikitable""
|-
|Test
|Drive
|-
|1
|2
|}}";

            Assert.That(Parsers.FixSyntax(table), Is.EqualTo(table), "No change to table with excess closing }");

            // IndexOutOfRangeException bug
            Assert.That(Parsers.FixSyntax(@"] now"), Is.EqualTo(@"] now"));

            Assert.That(Parsers.FixSyntax(@"{{DEFAULTSORT:hello
now"), Is.EqualTo(@"{{DEFAULTSORT:hello}}
now"));

            Assert.That(Parsers.FixSyntax(@"|[[Belmont (Durham) railway station|Belmont]] {[[Durham]])
|[[North Eastern Railway (UK)|NER]]
|1857"), Is.EqualTo(@"|[[Belmont (Durham) railway station|Belmont]] ([[Durham]])
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

            Assert.That(Parsers.FixSyntax(Choisir), Is.EqualTo(Choisir));

            const string Nochange = @"** >> {[[Sei Young Animation Co., Ltd.|Animação Retrô]]}";

            Assert.That(Parsers.FixSyntax(Nochange), Is.EqualTo(Nochange));

            Assert.That(Parsers.FixSyntax(@"<ref>{{cite web|url=a|title=b (ABC} }}</ref>"), Is.EqualTo(@"<ref>{{cite web|url=a|title=b (ABC) }}</ref>"));
        }

        [Test, Category("Incomplete")]
        public void TestFixSyntax()
        {
            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_3#NEsted_square_brackets_again.
            Assert.That(Parsers.FixSyntax("[[Image:foo.jpg|Some [http://some_crap.com]]]"),
                            Is.EqualTo("[[Image:foo.jpg|Some [http://some_crap.com]]]"));

            Assert.That(Parsers.FixSyntax("[[File:foo.jpg|Some [http://some_crap.com]]]"),
                            Is.EqualTo("[[File:foo.jpg|Some [http://some_crap.com]]]"));

            Assert.That(Parsers.FixSyntax("Image:foo.jpg|{{{some_crap}}}]]"), Is.EqualTo("Image:foo.jpg|{{{some_crap}}}]]"));

            Assert.That(Parsers.FixSyntax("[somelink]]"), Is.EqualTo("[[somelink]]"));
            Assert.That(Parsers.FixSyntax("[[somelink]"), Is.EqualTo("[[somelink]]"));
            Assert.AreNotEqual("[[somelink]]", Parsers.FixSyntax("[somelink]"));
            Assert.That(Parsers.FixSyntax("[[somelink|]"), Is.EqualTo("[[somelink]]"));

            // double pipe
            Assert.That(Parsers.FixSyntax(@"[[foo||bar]]"), Is.EqualTo(@"[[foo|bar]]"));

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_7#Erroneously_removing_pipe
            Assert.That(Parsers.FixSyntax("[[|foo]]"), Is.EqualTo("[[|foo]]"));

            bool noChange;
            // TODO: move it to parts testing specific functions, when they're covered
            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_4#Bug_encountered_when_perusing_Sonorous_Susurrus
            Parsers.CanonicalizeTitle("[[|foo]]"); // shouldn't throw exceptions
            Assert.That(Parsers.FixLinks("[[|foo]]", "bar", out noChange), Is.EqualTo("[[|foo]]"));

            Assert.That(Parsers.FixSyntax(@"[[foo||bar]]"), Is.EqualTo(@"[[foo|bar]]"));
            Assert.That(Parsers.FixLinkWhitespace("[[somelink_#a]]", "Test"), Is.EqualTo("[[somelink#a]]"));

            Assert.That(Parsers.FixSyntax(@"Foo<ref>a</ref>
==References==
<<reflist>>"), Is.EqualTo(@"Foo<ref>a</ref>
==References==
{{reflist}}"));
        }

        [Test]
        public void FixSyntaxOpeningRefTag()
        {
            Assert.That(Parsers.FixSyntax(@"Now.ref>foo</ref>"), Is.EqualTo(@"Now.<ref>foo</ref>"), "Simple case invalid opening tag");

            Assert.That(Parsers.FixSyntax(@"Now. ref>foo</ref>"), Is.EqualTo(@"Now.<ref>foo</ref>"), "Simple case invalid opening tag, space");

            Assert.That(Parsers.FixSyntax(@"Now. ref name=boo>foo</ref>"), Is.EqualTo(@"Now.<ref name=boo>foo</ref>"), "Simple case invalid opening named tag, space");
            Assert.That(Parsers.FixSyntax(@"Now.ref name=boo>foo</ref>"), Is.EqualTo(@"Now.<ref name=boo>foo</ref>"), "Simple case invalid opening named tag");
            Assert.That(Parsers.FixSyntax(@"Now. ref name=""boo"">foo</ref>"), Is.EqualTo(@"Now.<ref name=""boo"">foo</ref>"), "Simple case invalid opening tag, space");

            string nochange = @"Now.<ref name=fooref>foo</ref>";
            Assert.That(Parsers.FixSyntax(nochange), Is.EqualTo(nochange), "valid, ref in ref name");
            nochange = @"Now./ref>foo</ref>";
            Assert.That(Parsers.FixSyntax(nochange), Is.EqualTo(nochange), "invalid slash");

            Assert.That(Parsers.FixSyntax(@"Now.<ref<foo</ref>"), Is.EqualTo(@"Now.<ref>foo</ref>"), "Simple case <ref<");
            Assert.That(Parsers.FixSyntax(@"Now.<ref>{{foo}}/ref>"), Is.EqualTo(@"Now.<ref>{{foo}}</ref>"), "Simple case missing final <");
        }
 
        [Test]
        public void TestFixSyntaxExternalLinkSpacing()
        {
            Assert.That(Parsers.FixSyntax(@"their new[http://www.site.com site]"), Is.EqualTo(@"their new [http://www.site.com site]"));
            Assert.That(Parsers.FixSyntax(@"their new [http://www.site.com site]was"), Is.EqualTo(@"their new [http://www.site.com site] was"));
            Assert.That(Parsers.FixSyntax(@"their new[http://www.site.com site]was"), Is.EqualTo(@"their new [http://www.site.com site] was"));

            Assert.That(Parsers.FixSyntax(@"their new [http://www.site.com site]"), Is.EqualTo(@"their new [http://www.site.com site]"));
            Assert.That(Parsers.FixSyntax(@"their new [http://www.site.com site] was"), Is.EqualTo(@"their new [http://www.site.com site] was"));

            Assert.That(Parsers.FixSyntax(@"their new [http://www.site.com site] was [[blog]]ger then"), Is.EqualTo(@"their new [http://www.site.com site] was [[blog]]ger then"));

            const string nochange1 = @"their [[play]]ers", nochange2 = @"ts borders.<ref>[http://cyber.law.harvard.edu/filtering/a/ Saudi Arabia]</ref> A Saudi";
            Assert.That(Parsers.FixSyntax(nochange1), Is.EqualTo(nochange1));
            Assert.That(Parsers.FixSyntax(nochange2), Is.EqualTo(nochange2));

            // https
            Assert.That(Parsers.FixSyntax(@"their new[https://www.site.com site]"), Is.EqualTo(@"their new [https://www.site.com site]"));
            Assert.That(Parsers.FixSyntax(@"their new [https://www.site.com site]was"), Is.EqualTo(@"their new [https://www.site.com site] was"));
            Assert.That(Parsers.FixSyntax(@"their new[https://www.site.com site]was"), Is.EqualTo(@"their new [https://www.site.com site] was"));

            Assert.That(Parsers.FixSyntax(@"their new [https://www.site.com site]"), Is.EqualTo(@"their new [https://www.site.com site]"));
            Assert.That(Parsers.FixSyntax(@"their new [https://www.site.com site] was"), Is.EqualTo(@"their new [https://www.site.com site] was"));

            Assert.That(Parsers.FixSyntax(@"their new [https://www.site.com site] was [[blog]]ger then"), Is.EqualTo(@"their new [https://www.site.com site] was [[blog]]ger then"));

            const string nochange3 = @"ts borders.<ref>[https://cyber.law.harvard.edu/filtering/a/ Saudi Arabia]</ref> A Saudi";
            Assert.That(Parsers.FixSyntax(nochange3), Is.EqualTo(nochange3));

#if DEBUG
            // In Chinese Wikipedia  the text inside and outside of the link should be directly connected
            Variables.SetProjectLangCode("zh");
            Assert.That(Parsers.FixSyntax(@"their new[http://www.site.com site]"), Is.EqualTo(@"their new[http://www.site.com site]"));
            Assert.That(Parsers.FixSyntax(@"their new [http://www.site.com site]was"), Is.EqualTo(@"their new [http://www.site.com site]was"));
            Assert.That(Parsers.FixSyntax(@"their new[http://www.site.com site]was"), Is.EqualTo(@"their new[http://www.site.com site]was"));
            
            Variables.SetProjectLangCode("en");
            Assert.That(Parsers.FixSyntax(@"their new[http://www.site.com site]"), Is.EqualTo(@"their new [http://www.site.com site]"));
#endif
        }

        [Test]
        public void TestFixSyntaxReferencesWithNoHttp()
        {
            Assert.That(Parsers.FixSyntax(@"<ref>www.foo.com</ref>"), Is.EqualTo(@"<ref>http://www.foo.com</ref>"), "missing http");
            Assert.That(Parsers.FixSyntax(@"<ref>[www.foo.com bar]</ref>"), Is.EqualTo(@"<ref>[http://www.foo.com bar]</ref>"), "missing http inside brackets");
            Assert.That(Parsers.FixSyntax(@"<ref name=test>www.foo.com</ref>"), Is.EqualTo(@"<ref name=test>http://www.foo.com</ref>"), "missing http inside named ref");
            Assert.That(Parsers.FixSyntax(@"<ref>       www.foo.com</ref>"), Is.EqualTo(@"<ref>http://www.foo.com</ref>"));
            Assert.That(Parsers.FixSyntax(@"Visit www.foo.com"), Is.EqualTo(@"Visit www.foo.com"), "no changes outside references");
            Assert.That(Parsers.FixSyntax(@"<ref>[www-foo.a.com bar]</ref>"), Is.EqualTo(@"<ref>[www-foo.a.com bar]</ref>"), "No change for www-");
        }

        [Test]
        public void TestFixIncorrectBr()
        {
            Assert.That(Parsers.FixSyntax(@"<br.>"), Is.EqualTo("<br />"));
            Assert.That(Parsers.FixSyntax(@"<BR.>"), Is.EqualTo("<br />"));
            Assert.That(Parsers.FixSyntax(@"<br\>"), Is.EqualTo("<br />"));
            Assert.That(Parsers.FixSyntax(@"<BR\>"), Is.EqualTo("<br />"));
            Assert.That(Parsers.FixSyntax(@"<\br>"), Is.EqualTo("<br />"));
            Assert.That(Parsers.FixSyntax(@"<br./>"), Is.EqualTo("<br />"));
            Assert.That(Parsers.FixSyntax(@"<br /a>"), Is.EqualTo("<br />"));
            Assert.That(Parsers.FixSyntax(@"<br /v>"), Is.EqualTo("<br />"));
            Assert.That(Parsers.FixSyntax(@"<br /r>"), Is.EqualTo("<br />"));
            Assert.That(Parsers.FixSyntax(@"<br /s>"), Is.EqualTo("<br />"));
            Assert.That(Parsers.FixSyntax(@"<br /t>"), Is.EqualTo("<br />"));
            Assert.That(Parsers.FixSyntax(@"<br /z>"), Is.EqualTo("<br />"));
            Assert.That(Parsers.FixSyntax(@"<br /0>"), Is.EqualTo("<br />"));
            Assert.That(Parsers.FixSyntax(@"<br /1>"), Is.EqualTo("<br />"));
            Assert.That(Parsers.FixSyntax(@"<br /2>"), Is.EqualTo("<br />"));
            Assert.That(Parsers.FixSyntax(@"<br /9>"), Is.EqualTo("<br />"));
            Assert.That(Parsers.FixSyntax(@"<br /•>"), Is.EqualTo("<br />"));
            Assert.That(Parsers.FixSyntax(@"<br/•>"), Is.EqualTo("<br />"));
            Assert.That(Parsers.FixSyntax(@"<br /br>"), Is.EqualTo("<br />"));
            Assert.That(Parsers.FixSyntax(@"<br ?>"), Is.EqualTo("<br />"));
            Assert.That(Parsers.FixSyntax(@"<br?>"), Is.EqualTo("<br />"));
            Assert.That(Parsers.FixSyntax(@"<br//>"), Is.EqualTo("<br />"));
            Assert.That(Parsers.FixSyntax(@"</br>"), Is.EqualTo("<br />"));
            Assert.That(Parsers.FixSyntax(@"</br >"), Is.EqualTo("<br />"));
            Assert.That(Parsers.FixSyntax(@"</br />"), Is.EqualTo("<br />"));
            Assert.That(Parsers.FixSyntax(@"</br/>"), Is.EqualTo("<br />"));

            // these are already correct
            Assert.That(Parsers.FixSyntax(@"<br/>"), Is.EqualTo("<br/>"));
            Assert.That(Parsers.FixSyntax(@"<br />"), Is.EqualTo("<br />"));
        }

    }
}