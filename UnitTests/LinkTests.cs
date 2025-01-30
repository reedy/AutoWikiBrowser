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

using System.Collections.Generic;
using NUnit.Framework;
using WikiFunctions;
using WikiFunctions.Parse;

namespace UnitTests
{
    [TestFixture]
    public class LinkTests : RequiresParser
    {
        public GenfixesTestsBase genFixes  = new GenfixesTestsBase();

        [Test]
        public void TestStickyLinks()
        {
            Assert.That(Parsers.StickyLinks("[[Russian literature|Russian]] literature"), Is.EqualTo("[[Russian literature]]"));
            Assert.That(Parsers.StickyLinks("[[Russian literature|Russian]]  literature"), Is.EqualTo("[[Russian literature]]"));
            Assert.That(Parsers.StickyLinks("[[russian literature|Russian]] literature"), Is.EqualTo("[[Russian literature]]"));
            Assert.That(Parsers.StickyLinks("[[Russian literature|russian]] literature"), Is.EqualTo("[[russian literature]]"));
            Assert.That(Parsers.StickyLinks("[[Russian literature|Russian]]\nliterature"), Is.EqualTo("[[Russian literature|Russian]]\nliterature"));
            Assert.That(Parsers.StickyLinks("   [[Russian literature|Russian]] literature  "), Is.EqualTo("   [[Russian literature]]  "));

            Assert.That(Parsers.StickyLinks("[[Russian literature|Russian]] Literature"), Is.EqualTo("[[Russian literature|Russian]] Literature"));

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_1#Link_de-piping_false_positive
            Assert.That(Parsers.StickyLinks("[[Sacramento, California|Sacramento]], California's [[capital city]]"),
                            Is.EqualTo("[[Sacramento, California|Sacramento]], California's [[capital city]]"));

            Assert.That(Parsers.StickyLinks("[[Russian literature|Russian literature]] was"), Is.EqualTo("[[Russian literature|Russian literature]] was"), "bugfix – no exception when pipe same length as target");
        }

        [Test]
        public void TestSimplifyLinks()
        {
            Assert.That(Parsers.SimplifyLinks("[[dog|dogs]]"), Is.EqualTo("[[dog]]s"));

            // case insensitivity of the first char
            Assert.That(Parsers.SimplifyLinks("[[Dog|dogs]]"), Is.EqualTo("[[dog]]s"));
            Assert.That(Parsers.SimplifyLinks("[[dog|Dogs]]"), Is.EqualTo("[[Dog]]s"));

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_8#Wrong_link_simplification_capitalisation
            Assert.That(Parsers.SimplifyLinks("[[Dog|dog]]"), Is.EqualTo("[[dog]]"));
            Assert.That(Parsers.SimplifyLinks("[[dog|Dog]]"), Is.EqualTo("[[Dog]]"));
            Assert.That(Parsers.SimplifyLinks("[[Dog|Dog]]"), Is.EqualTo("[[Dog]]"));

            Assert.That(Parsers.SimplifyLinks("[[Dog|dogs]]"), Is.EqualTo("[[dog]]s"));
            Assert.That(Parsers.SimplifyLinks("[[dog|Dogs]]"), Is.EqualTo("[[Dog]]s"));
            Assert.That(Parsers.SimplifyLinks("[[Dog|Dogs]]"), Is.EqualTo("[[Dog]]s"));
            Assert.That(Parsers.SimplifyLinks("#REDIRECT[[dog|Dog]]"), Is.EqualTo("#REDIRECT[[Dog]]"));
            Assert.That(Parsers.SimplifyLinks("[[funcy dog|funcy dogs]]"), Is.EqualTo("[[funcy dog]]s"));

            Assert.That(Parsers.SimplifyLinks("[[dog|dog.]]"), Is.EqualTo("[[dog]]."), "point inside wikilink");
            Assert.That(Parsers.SimplifyLinks("[[Dog|dog.]]"), Is.EqualTo("[[dog]]."), "point inside wikilink");
            Assert.That(Parsers.SimplifyLinks("[[Dog|dog,]]"), Is.EqualTo("[[dog]],"), "comma inside wikilink");
            Assert.That(Parsers.SimplifyLinks("[[dog|dog,]]"), Is.EqualTo("[[dog]],"), "comma inside wikilink");

            Assert.That(Parsers.SimplifyLinks("[[dog|(dog)]]"), Is.EqualTo("([[dog]])"), "brackets inside wikilink");

            Assert.That(Parsers.SimplifyLinks("[[funcy dog|funcy_dog]]"), Is.EqualTo("[[funcy dog]]"), "handles underscore in text: text");
            Assert.That(Parsers.SimplifyLinks("[[funcy_dog|funcy dog]]"), Is.EqualTo("[[funcy dog]]"), "handles underscore in text: target");
            Assert.That(Parsers.SimplifyLinks("[[funcy_dog|Funcy dog]]"), Is.EqualTo("[[Funcy dog]]"), "handles underscore in text: target");

            Variables.UnderscoredTitles.Add("Funcy dog");
            Assert.That(Parsers.SimplifyLinks("[[funcy dog|funcy_dog]]"), Is.EqualTo("[[funcy dog|funcy_dog]]"), "handles underscore in text where article with underscore in title");
            Variables.UnderscoredTitles.Remove("Funcy dog");

            // ...and sensitivity of others
            Assert.That(Parsers.SimplifyLinks("[[dog|dOgs]]"), Is.EqualTo("[[dog|dOgs]]"));

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_2#Inappropriate_link_compression
            Assert.That(Parsers.SimplifyLinks("[[foo|foo3]]"), Is.EqualTo("[[foo|foo3]]"));

            // don't touch suffixes with caps to avoid funky results like
            // https://en.wikipedia.org/w/index.php?diff=195760456
            Assert.That(Parsers.SimplifyLinks("[[FOO|FOOBAR]]"), Is.EqualTo("[[FOO|FOOBAR]]"));
            Assert.That(Parsers.SimplifyLinks("[[foo|fooBAR]]"), Is.EqualTo("[[foo|fooBAR]]"));

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_8#Only_one_spurious_space_removed_from_link
            Assert.That(Parsers.SimplifyLinks("[[Elizabeth Gunn | Elizabeth Gunn]]"), Is.EqualTo("[[Elizabeth Gunn]]"));
            Assert.That(Parsers.SimplifyLinks("[[Big Bend, Texas | Big Bend]]"), Is.EqualTo("[[Big Bend, Texas|Big Bend]]"));

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_8#SVN:_general_fixes_removes_whitespace_around_pipes_within_citation_templates
            Assert.That(Parsers.SimplifyLinks("{{foo|[[bar]] | boz}}]]"), Is.EqualTo("{{foo|[[bar]] | boz}}]]"));
            Assert.That(Parsers.SimplifyLinks("{{foo|[[bar]]\r\n| boz}}]]"), Is.EqualTo("{{foo|[[bar]]\r\n| boz}}]]"));

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_9#General_fixes_remove_spaces_from_category_sortkeys
            Assert.That(Parsers.SimplifyLinks("[[foo| bar]]"), Is.EqualTo("[[foo|bar]]"));
            Assert.That(Parsers.SimplifyLinks("[[foo| bar ]]"), Is.EqualTo("[[foo|bar]]"));
            Assert.That(Parsers.SimplifyLinks("[[Category:foo| bar]]"), Is.EqualTo("[[Category:foo| bar]]"));
            Assert.That(Parsers.SimplifyLinks("[[Category:foo| bar ]]"), Is.EqualTo("[[Category:foo| bar]]"));

            // nothing to do here
            Assert.That(Parsers.SimplifyLinks("[[dog|]]"), Is.EqualTo("[[dog|]]"));

            const string nochange = @"[[File:T and E.jpg|right|thumbnail|With [[EW|E]] in 2004]]";
            Assert.That(Parsers.SimplifyLinks(nochange), Is.EqualTo(nochange));
            Assert.That(Parsers.SimplifyLinks(@"[[File:T and E.jpg|right|thumbnail|With [[dog|dogs]] in 2004]]"), Is.EqualTo(@"[[File:T and E.jpg|right|thumbnail|With [[dog]]s in 2004]]"), "nested link handling");

            Assert.That(Parsers.SimplifyLinks(@"[[File:Lego cathedral.JPG | right | thumb | Lego cathedral]]"), Is.EqualTo(@"[[File:Lego cathedral.JPG|right|thumb|Lego cathedral]]"), "Multiple whitespace cleanup around pipes");

            Assert.That(Parsers.SimplifyLinks("[[İstanbul Cup|Istanbul Cup]]"), Is.EqualTo("[[İstanbul Cup|Istanbul Cup]]"), "No change when diacritics on first letter of page mean lowercase happens to match");

            // https://phabricator.wikimedia.org/T317207
            // Has opening ( in link display text, but no closing )
            Assert.That(Parsers.SimplifyLinks("[[Нью-Мексико|(Нью-Мексико]]"), Is.EqualTo("([[Нью-Мексико]])"));

            // Trailing ) is outside the link, so the link regex doesn't pick it up; it's now handled separately
            Assert.That(Parsers.SimplifyLinks("[[Ермітаж|(Ермітаж]])"), Is.EqualTo("([[Ермітаж]])"));
        }

        [Test]
        public void FixDatesHTML()
        {
            // replace <br> and <p> HTML tags tests
            Assert.That(parser.FixBrParagraphs("<p>some text"), Is.EqualTo("\r\n\r\nsome text"));
            Assert.That(parser.FixBrParagraphs("<p>[[some text|bar]]"), Is.EqualTo("\r\n\r\n[[some text|bar]]"));
            Assert.That(parser.FixBrParagraphs("<p>some text</p>"), Is.EqualTo("\r\n\r\nsome text\r\n\r\n"));
            Assert.That(parser.FixBrParagraphs("<p> some text"), Is.EqualTo("\r\n\r\nsome text"));
            Assert.That(parser.FixBrParagraphs("<br><br>some text"), Is.EqualTo("\r\n\r\nsome text"));
            Assert.That(parser.FixBrParagraphs("some text<p>"), Is.EqualTo("some text\r\n\r\n"));
            Assert.That(parser.FixBrParagraphs("some text<br><br>"), Is.EqualTo("some text\r\n\r\n"));
            Assert.That(parser.FixBrParagraphs("some text<br><br>word"), Is.EqualTo("some text\r\n\r\nword"));

            // don't match when in table or blockquote
            Assert.That(parser.FixBrParagraphs("|<p>some text"), Is.EqualTo("|<p>some text"));
            Assert.That(parser.FixBrParagraphs("|<br><br>some text"), Is.EqualTo("|<br><br>some text"));
            Assert.That(parser.FixBrParagraphs("!<p>some text"), Is.EqualTo("!<p>some text"));
            Assert.That(parser.FixBrParagraphs("!<br><br>some text"), Is.EqualTo("!<br><br>some text"));

            genFixes.AssertNotChanged(@"<blockquote><p>some text</blockquote>");
            genFixes.AssertNotChanged("<blockquote>|<br><br>some text</blockquote>");

            Assert.That(parser.FixBrParagraphs(@"* {{Polish2|Krzepice (województwo dolnośląskie)|[[24 November]] [[2007]]}}<br><br>  "), Is.EqualTo(@"* {{Polish2|Krzepice (województwo dolnośląskie)|[[24 November]] [[2007]]}}

"));

            const string nochange = @"{{cite web | title = Hello April 14 2009 there | date=2011-11-13 }}";

            Assert.That(parser.FixBrParagraphs(nochange), Is.EqualTo(nochange));
        }

        [Test]
        public void FixDatesCommaErrors()
        {
            const string correct1 = @"Retrieved on April 14, 2009 was";
            Assert.That(parser.FixDatesA(@"Retrieved on April, 14, 2009 was"), Is.EqualTo(correct1));
            Assert.That(parser.FixDatesA(@"Retrieved on April , 14 , 2009 was"), Is.EqualTo(correct1));
            Assert.That(parser.FixDatesA(@"Retrieved on April , 14 ,2009 was"), Is.EqualTo(correct1));

            Assert.That(parser.FixDatesA(@"Retrieved on April 14,2009 was"), Is.EqualTo(correct1));
            Assert.That(parser.FixDatesA(@"Retrieved on April 14 ,2009 was"), Is.EqualTo(correct1));
            Assert.That(parser.FixDatesA(@"Retrieved on April 14 2009 was"), Is.EqualTo(correct1));
            Assert.That(parser.FixDatesA(@"Retrieved on April14,2009 was"), Is.EqualTo(correct1));
            Assert.That(parser.FixDatesA(@"Retrieved on April14, 2009 was"), Is.EqualTo(correct1));
            Assert.That(parser.FixDatesA(@"Retrieved on April14 2009 was"), Is.EqualTo(correct1));
            Assert.That(parser.FixDatesA(correct1), Is.EqualTo(correct1));

            // don't change image/link target names
            string image1 = @"now foo [[Image:Foo July 24 2009.png]] was";
            Assert.That(parser.FixDatesA(image1), Is.EqualTo(image1));
            string image2 = @"now foo [[File:Foo July 24 2009.png]] was";
            Assert.That(parser.FixDatesA(image2), Is.EqualTo(image2));
            string link1 = @"now [[July 29 1966, P.N.E.]] was";
            Assert.That(parser.FixDatesA(link1), Is.EqualTo(link1));

            const string correct2 = @"Retrieved on 14 April 2009 was";
            Assert.That(parser.FixDatesA(@"Retrieved on 14 April, 2009 was"), Is.EqualTo(correct2));
            Assert.That(parser.FixDatesA(@"Retrieved on 14 April , 2009 was"), Is.EqualTo(correct2));
            Assert.That(parser.FixDatesA(@"Retrieved on 14 April,  2009 was"), Is.EqualTo(correct2));

            const string correct3 = @"[[Now]] on 14 April 2009 was";
            Assert.That(parser.FixDatesA(@"[[Now]] on 14 April, 2009 was"), Is.EqualTo(correct3));

            const string nochange1 = @"On 14 April, 2590 people", nochange2 = @"Retrieved on April 142009 was";
            Assert.That(parser.FixDatesA(nochange1), Is.EqualTo(nochange1));
            Assert.That(parser.FixDatesA(nochange2), Is.EqualTo(nochange2));

            Assert.That(parser.FixDatesA(@"#####
'''A''' (1 December 1920 &ndash; 28 May, 2013)"), Is.EqualTo(@"#####
'''A''' (1 December 1920 &ndash; 28 May 2013)"));
        }
        

        [Test]
        public void FixDatesEnOnly()
        {
            #if DEBUG
            Variables.SetProjectLangCode("fr");
            Assert.That(parser.FixDatesA(@"Retrieved on April, 14, 2009 was"), Is.EqualTo(@"Retrieved on April, 14, 2009 was"));
            Assert.That(parser.FixDatesB(@"from (1900-1933) there", false, false), Is.EqualTo(@"from (1900-1933) there"));

            Variables.SetProjectLangCode("en");
            const string correct1 = @"Retrieved on April 14, 2009 was";
            Assert.That(parser.FixDatesA(@"Retrieved on April, 14, 2009 was"), Is.EqualTo(correct1));
            const string correct = @"from (1900–1933) there";
            Assert.That(parser.FixDatesB(@"from (1900-1933) there", false, false), Is.EqualTo(correct));
            #endif
        }

        [Test]
        public void FixDatesRanges()
        {
            const string correct1 = @"On 3–17 May 2009 a dog";
            Assert.That(parser.FixDatesA(@"On 3-17 May 2009 a dog"), Is.EqualTo(correct1));
            Assert.That(parser.FixDatesA(@"On 3 - 17 May 2009 a dog"), Is.EqualTo(correct1));
            Assert.That(parser.FixDatesA(@"On 3 -17 May 2009 a dog"), Is.EqualTo(correct1));
            Assert.That(parser.FixDatesA(@"On 3 May-17 May 2009 a dog"), Is.EqualTo(correct1));
            Assert.That(parser.FixDatesA(@"On 3 May - 17 May 2009 a dog"), Is.EqualTo(correct1));
            Assert.That(parser.FixDatesA(@"On 3 May – 17 May 2009 a dog"), Is.EqualTo(correct1));
            Assert.That(parser.FixDatesA(@"On 3 May – 17 May, 2009 a dog"), Is.EqualTo(correct1));

            // American format
            const string correct2 = @"On May 3–17, 2009 a dog";
            Assert.That(parser.FixDatesA(@"On May 3-17, 2009 a dog"), Is.EqualTo(correct2));
            Assert.That(parser.FixDatesA(@"On May 3-17 2009 a dog"), Is.EqualTo(correct2));
            Assert.That(parser.FixDatesA(@"On May 3 - 17 2009 a dog"), Is.EqualTo(correct2));
            Assert.That(parser.FixDatesA(@"On May 3   -17 2009 a dog"), Is.EqualTo(correct2));
            Assert.That(parser.FixDatesA(@"On May 3 - May 17 2009 a dog"), Is.EqualTo(correct2));
            Assert.That(parser.FixDatesA(@"On May 3 - May 17, 2009 a dog"), Is.EqualTo(correct2));
            Assert.That(parser.FixDatesA(@"On May 3 – May 17 2009 a dog"), Is.EqualTo(correct2));
            Assert.That(parser.FixDatesA(@"On May 3 – May 17, 2009 a dog"), Is.EqualTo(correct2));

            // no change
            const string nochange1 = @"May 17 - 13,009 dogs";
            Assert.That(parser.FixDatesA(nochange1), Is.EqualTo(nochange1));

            const string nochange2 = @"May 2-4-0";
            Assert.That(parser.FixDatesA(nochange2), Is.EqualTo(nochange2));

            // month ranges
            const string correct3 = @"May–June 2010";
            Assert.That(parser.FixDatesA(@"May-June 2010"), Is.EqualTo(correct3), "endash set for month range");
            Assert.That(parser.FixDatesA(correct3), Is.EqualTo(correct3));

            Assert.That(parser.FixDatesA("from 1904 – 11 May 1956 there"), Is.EqualTo("from 1904 – 11 May 1956 there"));

            // DateRangeToYear: result is spaced endash
            const string DateToYear = @"'''Nowell''' (May 16, 1872 - 1940), was";
            genFixes.AssertChange(DateToYear, DateToYear.Replace("-", "–"));
            genFixes.AssertChange(DateToYear.Replace(" - ", "-"), DateToYear.Replace("-", "–"));
            const string DateToYear2 = @"'''Nowell''' (16 May 1872–1940), was";
            genFixes.AssertChange(DateToYear2, DateToYear2.Replace("–", " – "));
            genFixes.AssertChange(DateToYear2.Replace("–", "-"), DateToYear2.Replace("–", " – "));

            genFixes.AssertNotChanged(@"Volume 1, 2001–2004 was");
            genFixes.AssertNotChanged(@"ISBN 1-883402-17-4 February 1998, hardcover");
        }

        [Test]
        public void TestFullYearRanges()
        {
            const string correct = @"from (1900–1933) there";
            Assert.That(parser.FixDatesB(@"from (1900-1933) there", false, false), Is.EqualTo(correct));
            Assert.That(parser.FixDatesB(@"from (1900  –  1933) there", false, false), Is.EqualTo(correct));
            Assert.That(parser.FixDatesB(@"from (1900 -1933) there", false, false), Is.EqualTo(correct));
            Assert.That(parser.FixDatesB(@"from (1900 - 1933) there", false, false), Is.EqualTo(correct));
            Assert.That(parser.FixDatesB(@"from (1900 -  1933) there", false, false), Is.EqualTo(correct));
            Assert.That(parser.FixDatesB(@"from (1900 - 1901) there", false, false), Is.EqualTo(@"from (1900–1901) there"));
            Assert.That(parser.FixDatesB(@"from (1900-1933, 2000) there", false, false), Is.EqualTo(@"from (1900–1933, 2000) there"));
            Assert.That(parser.FixDatesB(@"from (1900-1933, 2000-2002) there", false, false), Is.EqualTo(@"from (1900–1933, 2000–2002) there"));
            Assert.That(parser.FixDatesB(@"from ( 1900-1933) there", false, false), Is.EqualTo(@"from ( 1900–1933) there"));

            Assert.That(parser.FixDatesB(@"from 1950-1960,", false, false), Is.EqualTo(@"from 1950–1960,"));
            Assert.That(parser.FixDatesB(@"|1950-1960|", false, false), Is.EqualTo(@"|1950–1960|"));
            Assert.That(parser.FixDatesB(@"(1950-1960 and 1963-1968)", false, false), Is.EqualTo(@"(1950–1960 and 1963–1968)"));
            Assert.That(parser.FixDatesB(@"or 1900 - 1901,", false, false), Is.EqualTo(@"or 1900–1901,"));
            Assert.That(parser.FixDatesB(@"for 1900 - 1901,", false, false), Is.EqualTo(@"for 1900–1901,"));

            // no change – not valid date range
            genFixes.AssertNotChanged(@"from (1900–1870) there");

            // already okay
            genFixes.AssertNotChanged(@"from (1900&ndash;1933) there");
            genFixes.AssertNotChanged(@"from (1900–1933) there");

            Assert.That(parser.FixDatesB(@"now between 1900-1920
was", false, false), Is.EqualTo(@"now between 1900–1920
was"));

            string BadRange = @"from 1950-1920,";
            Assert.That(parser.FixDatesB(BadRange, false, false), Is.EqualTo(BadRange));
            BadRange = @"from 1950 – 1920,";
            Assert.That(parser.FixDatesB(BadRange, false, false), Is.EqualTo(BadRange));
            BadRange = @"from 1980-70,";
            Assert.That(parser.FixDatesB(BadRange, false, false), Is.EqualTo(BadRange));

            // full date ranges
            Assert.That(parser.FixDatesB(@"over 1 April 2004-5 July 2009.", false, false), Is.EqualTo(@"over 1 April 2004 – 5 July 2009."));
            Assert.That(parser.FixDatesB(@"over April 1, 2004–July 5, 2009.", false, false), Is.EqualTo(@"over April 1, 2004 – July 5, 2009."));
        }

        [Test]
        public void CircaYearRanges()
        {
            Assert.That(parser.FixDatesB(@"c. 1950 - 1960,", false, false), Is.EqualTo(@"c. 1950 – 1960,"));
            Assert.That(parser.FixDatesB(@"ca. 1950 - 1960,", false, false), Is.EqualTo(@"ca. 1950 – 1960,"));
            Assert.That(parser.FixDatesB(@"circa 1950 - 1960,", false, false), Is.EqualTo(@"circa 1950 – 1960,"));

            // no changes because can't use hidemore and detect the links
            genFixes.AssertNotChanged(@"{{Circa}} 1950 – 1960,");
            genFixes.AssertNotChanged(@"[[c.]] 1950 - 1960,");
            genFixes.AssertNotChanged(@"[[c]]. 1950 - 1960,");
            genFixes.AssertNotChanged(@"[[c]] 1950 - 1960,");
            genFixes.AssertNotChanged(@"[[circa]] 1950 - 1960,");
            genFixes.AssertNotChanged(@"[[circa|c.]] 1950 - 1960,");
            genFixes.AssertNotChanged(@"[[circa|c]]. 1950 - 1960,");
            genFixes.AssertNotChanged(@"{{circa}} 1950 - 1960,");
            genFixes.AssertNotChanged(@"{{Circa}} 1950 - 1960,");
            genFixes.AssertNotChanged(@"circle 1950 - 1960,");
            genFixes.AssertNotChanged(@"[[Foo (1950-1960)|Foo]]");
            genFixes.AssertNotChanged(@"[[Foo (1950 - 1960)|Foo]]");
        }

        [Test]
        public void TestYearToPresentRanges()
        {
            const string present = @"from 2002–present was";
            Assert.That(parser.FixDatesB(@"from 2002-present was", false, false), Is.EqualTo(present));
            Assert.That(parser.FixDatesB(@"from 2002 -   present was", false, false), Is.EqualTo(present));
            Assert.That(parser.FixDatesB(@"from 2002–present was", false, false), Is.EqualTo(present));

            Assert.That(parser.FixDatesB(@"from 2002-Present was", false, false), Is.EqualTo(@"from 2002–Present was"));

            const string present2 = @"== Members ==
* [[Nick Hexum]] - Vocals, [[Rhythm Guitar]], Programming (1989 - present)
* [[S. A. Martinez|Doug Martinez]] - Vocals, [[Phonograph|Turntables]], DJ (1992 - present)
* [[Tim Mahoney (guitarist)|Tim Mahoney]] - [[Lead Guitar]] (1991 - present)
* [[P-Nut|Aaron Wills]] - [[Bass guitar]] (1989 - present)
* [[Chad Sexton]] - [[Drum]]s, Programming, Percussion (1989 - present)";

            Assert.That(parser.FixDatesB(present2, false, false), Is.EqualTo(present2.Replace(@" - p", @"–p")));

            genFixes.AssertNotChanged(@"* 2000 - presented");
        }

        [Test]
        public void TestDateToPresentRanges()
        {
            Assert.That(parser.FixDatesB(@"from May 2002 - present was", false, false), Is.EqualTo(@"from May 2002 – present was"));
            Assert.That(parser.FixDatesB(@"from May 2002-present was", false, false), Is.EqualTo(@"from May 2002 – present was"));
            Assert.That(parser.FixDatesB(@"from May 11, 2002-present was", false, false), Is.EqualTo(@"from May 11, 2002 – present was"));
            Assert.That(parser.FixDatesB(@"from May 11, 2002 - present was", false, false), Is.EqualTo(@"from May 11, 2002 – present was"));
            Assert.That(parser.FixDatesB(@"from 2002 - present was", false, false), Is.EqualTo(@"from 2002–present was"), "year range, becomes unspaced");
            Assert.That(parser.FixDatesB(@"from 2002-present was", false, false), Is.EqualTo(@"from 2002–present was"), "year range, unspaced");

            Assert.That(parser.FixDatesB(@"Deputy-leader 2008-11, 2014-present", false, false), Is.EqualTo(@"Deputy-leader 2008-11, 2014–present"), "Endash unspaced when year to present, ignore previous year range");
        }

        [Test]
        public void TestShortenedYearRanges()
        {
            const string correct = @"from (1900–33) there";
            Assert.That(parser.FixDatesB(@"from (1900-33) there", false, false), Is.EqualTo(correct));
            Assert.That(parser.FixDatesB(@"from (1900 -33) there", false, false), Is.EqualTo(correct));
            Assert.That(parser.FixDatesB(@"from (1900 - 33) there", false, false), Is.EqualTo(correct));
            Assert.That(parser.FixDatesB(@"from (1900 -  33) there", false, false), Is.EqualTo(correct));
            Assert.That(parser.FixDatesB(@"from (1900 - 1901) there", false, false), Is.EqualTo(@"from (1900–1901) there"));
            Assert.That(parser.FixDatesB(@"from (1900-33, 2000) there", false, false), Is.EqualTo(@"from (1900–33, 2000) there"));
            Assert.That(parser.FixDatesB(@"from (1900-33, 2000-2002) there", false, false), Is.EqualTo(@"from (1900–33, 2000–2002) there"));
            Assert.That(parser.FixDatesB(@"from ( 1900-33) there", false, false), Is.EqualTo(@"from ( 1900–33) there"));

            Assert.That(parser.FixDatesB(@"from 1950-60,", false, false), Is.EqualTo(@"from 1950–60,"));
            Assert.That(parser.FixDatesB(@"x the 2010-11, 2011-12 and 2013-14 x", false, false), Is.EqualTo(@"x the 2010–11, 2011–12 and 2013–14 x"));
            Assert.That(parser.FixDatesB(@"(1950-60 and 1963-68)", false, false), Is.EqualTo(@"(1950–60 and 1963–68)"));

            // no change – not valid date range
            genFixes.AssertNotChanged(@"from (1920–18) there");

            // already okay
            genFixes.AssertNotChanged(@"from (1900&ndash;33) there");
            genFixes.AssertNotChanged(@"from (1900–33) there");
        }


        [Test]
        public void FixLivingThingsRelatedDates()
        {
            Assert.That(Parsers.FixLivingThingsRelatedDates("test text"), Is.EqualTo("test text"));
            Assert.That(Parsers.FixLivingThingsRelatedDates("'''John Doe''' (b. [[21 February]] [[2008]])"), Is.EqualTo("'''John Doe''' (born [[21 February]] [[2008]])"), "b. expanded");
            Assert.That(Parsers.FixLivingThingsRelatedDates(@"'''John O'Doe''' (b. [[21 February]] [[2008]])"), Is.EqualTo(@"'''John O'Doe''' (born [[21 February]] [[2008]])"), "b. expanded, name has apostrophe");
            Assert.That(Parsers.FixLivingThingsRelatedDates("'''John Doe''' (b. 21 February 2008)"), Is.EqualTo("'''John Doe''' (born 21 February 2008)"), "non-wikilinked dates supported");
            Assert.That(Parsers.FixLivingThingsRelatedDates("'''John Doe''' ([[21 February]] [[2008]]–)"), Is.EqualTo("'''John Doe''' (born [[21 February]] [[2008]])"), "dash for born expanded");
            Assert.That(Parsers.FixLivingThingsRelatedDates("'''John O'Doe''' ([[21 February]] [[2008]]–)"), Is.EqualTo("'''John O'Doe''' (born [[21 February]] [[2008]])"), "dash for born expanded, name has apostrophe");
            Assert.That(Parsers.FixLivingThingsRelatedDates("'''John Doe''' ([[21 February]] [[2008]] &ndash;)"), Is.EqualTo("'''John Doe''' (born [[21 February]] [[2008]])"), "dash for born expanded");
            Assert.That(Parsers.FixLivingThingsRelatedDates("'''John Doe''' (born: [[21 February]] [[2008]])"), Is.EqualTo("'''John Doe''' (born [[21 February]] [[2008]])"), "born: tidied");
            Assert.That(Parsers.FixLivingThingsRelatedDates("'''John Doe''' (Born: [[21 February]] [[2008]])"), Is.EqualTo("'''John Doe''' (born [[21 February]] [[2008]])"), "born: tidied");
            Assert.That(Parsers.FixLivingThingsRelatedDates("'''John Doe''' (b. March 6, 2008)"), Is.EqualTo("'''John Doe''' (born March 6, 2008)"), "b. expanded");
            Assert.That(Parsers.FixLivingThingsRelatedDates("'''John Doe''' (b. 2008)"), Is.EqualTo("'''John Doe''' (born 2008)"), "b. expanded");
            Assert.That(Parsers.FixLivingThingsRelatedDates("'''John Doe''' (b.2008)"), Is.EqualTo("'''John Doe''' (born 2008)"), "b. expanded");
            Assert.That(Parsers.FixLivingThingsRelatedDates("'''John Doe''' (born on 2008)"), Is.EqualTo("'''John Doe''' (born 2008)"), "born on fixed");
            Assert.That(Parsers.FixLivingThingsRelatedDates("'''John Doe''' (Born 2008)"), Is.EqualTo("'''John Doe''' (born 2008)"), "Born fixed");
            Assert.That(Parsers.FixLivingThingsRelatedDates("'''John Doe''' (b. March 6, 2008, London)"), Is.EqualTo("'''John Doe''' (born March 6, 2008, London)"), "b. expanded");
            Assert.That(Parsers.FixLivingThingsRelatedDates("'''John Doe''' (b. March 6, 2008 in London)"), Is.EqualTo("'''John Doe''' (born March 6, 2008 in London)"), "b. expanded");
            Assert.That(Parsers.FixLivingThingsRelatedDates("'''John Doe''' (born on March 6, 2008, London)"), Is.EqualTo("'''John Doe''' (born March 6, 2008, London)"), "born on fixed");

            Assert.That(Parsers.FixLivingThingsRelatedDates("'''John Doe''' (d. [[21 February]] [[2008]])"), Is.EqualTo("'''John Doe''' (died [[21 February]] [[2008]])"));
            Assert.That(Parsers.FixLivingThingsRelatedDates("'''John Doe''' (d.[[21 February]] [[2008]])"), Is.EqualTo("'''John Doe''' (died [[21 February]] [[2008]])"));
            Assert.That(Parsers.FixLivingThingsRelatedDates("'''John Doe''' (d.2008)"), Is.EqualTo("'''John Doe''' (died 2008)"));
            Assert.That(Parsers.FixLivingThingsRelatedDates("'''John Doe''' (d. February 21, 2008)"), Is.EqualTo("'''John Doe''' (died February 21, 2008)"));
            Assert.That(Parsers.FixLivingThingsRelatedDates("'''John O'Doe''' (d. [[21 February]] [[2008]])"), Is.EqualTo("'''John O'Doe''' (died [[21 February]] [[2008]])"));
            Assert.That(Parsers.FixLivingThingsRelatedDates("'''Willa Klug Baum''' (born [[October 4]], [[1926]], died May 18, 2006)"), Is.EqualTo("'''Willa Klug Baum''' ([[October 4]], [[1926]] – May 18, 2006)"));
            Assert.That(Parsers.FixLivingThingsRelatedDates("'''Willa Klug Baum''' (b.1926, died May 18, 2006)"), Is.EqualTo("'''Willa Klug Baum''' (1926 – May 18, 2006)"));

            const string Nochange = @"{{Infobox baseball team|
  FormerNames= '''[[Nishi-Nippon Railroad|Nishitetsu]] Clippers''' (1950)<br>'''Nishitetsu Lions''' (1951-1972)<br>'''Taiheiyo Club Lions''' (1973–1976)<br>'''Crown Lighter Lions''' (1977–1978)<br>'''Seibu Lions''' (1979–2007)<br>'''Saitama Seibu Lions''' (2008–)|
  foo=bar }}";

            Assert.That(Parsers.FixLivingThingsRelatedDates(Nochange), Is.EqualTo(Nochange));

            const string Nochange2 = @"*'''[[Luís Godinho Lopes|Luís Filipe Fernandes David Godinho Lopes]]''' (2011–)";

            Assert.That(Parsers.FixLivingThingsRelatedDates(Nochange2), Is.EqualTo(Nochange2));

            const string Nochange3 = @"** '''1st''' Andy Brown '''52''' (2008-)";

            Assert.That(Parsers.FixLivingThingsRelatedDates(Nochange3), Is.EqualTo(Nochange3));

            const string Nochange4 = @"# '''[[Luís Godinho Lopes|Luís Filipe Fernandes David Godinho Lopes]]''' (2011–)";

            Assert.That(Parsers.FixLivingThingsRelatedDates(Nochange4), Is.EqualTo(Nochange4));
        }

        [Test]
        public void UnlinkedFloruit()
        {
            const string LinkedFloruit = @"'''Foo''' ([[floruit|fl.]] 550) was a peasant.

==Time==
Foo was happy";
            genFixes.AssertNotChanged(LinkedFloruit);

            genFixes.AssertChange(@"'''Foo''' (fl. 550) was a peasant.

==Time==
Foo was happy", LinkedFloruit); // lowercase
             genFixes.AssertChange(@"'''Foo''' (fl 550) was a peasant.

==Time==
Foo was happy", LinkedFloruit); // no dot
            genFixes.AssertChange(@"'''Foo''' ( fl. 550) was a peasant.

==Time==
Foo was happy", LinkedFloruit); // extra whitespace
            genFixes.AssertChange(@"'''Foo''' (Fl. 550) was a peasant.

==Time==
Foo was happy", LinkedFloruit); // title case

            const string Floruit550 = @"'''Foo''' ([[floruit|fl.]] 550) was a peasant.
Foo was happy
Other (fl. 1645) was also";
            
            genFixes.AssertNotChanged(Floruit550); // No change when first floruit already linked

            genFixes.AssertChange(@"'''Foo''' (fl. 550) was a peasant.
Foo was happy
Other (fl. 1645) was also", Floruit550); // only first floruit linked

            const string FloruitLaterSection = @"'''Foo''' was a peasant.

==Other==
Other (fl. 1645) was also", FloruitTwice = @"'''Foo''' (fl. 55) was a peasant, related to other (fl. 600)";

            genFixes.AssertNotChanged(FloruitLaterSection); // not linked outside zeroth section

            genFixes.AssertChange(FloruitTwice, @"'''Foo''' ([[floruit|fl.]] 55) was a peasant, related to other (fl. 600)"); // only first occurrence linked

            genFixes.AssertNotChanged(@"{{cite encyclopedia|encyclopedia=ODNB|url=http://www.oxforddnb.com/view/olddnb/21073 |title=Packe, Christopher (fl. 1796)}}"); // not linked within template text
        }

        [Test]
        public void FixPeopleCategoriesTests()
        {
            // must be about a person
            const string a0 = @"'''Fred Smith''' (born 1960) is a bloke.";
            Assert.That(Parsers.FixPeopleCategories(a0, "foo"), Is.EqualTo(a0));

            const string bug1 = @"{{BLP unsourced|date=March 2010}}

'''Z''' (born 2 January{{Year needed|date=January 2010|reason=Fix this date immediately or remove it; it look}} in .";

            Assert.That(Parsers.FixPeopleCategories(bug1, "Foo"), Is.EqualTo(bug1));
        }

        [Test]
        public void GetInfoBoxFieldValue()
        {
            List<string> Year = new List<string>(new[] { "year" });

            Assert.That(Parsers.GetInfoBoxFieldValue(@"hello {{infobox foo
|year=1990
|other=great}} now", Year), Is.EqualTo(@"1990"));

            Assert.That(Parsers.GetInfoBoxFieldValue(@"hello {{infobox foo
|  year  =  1990
|other=great}} now", Year), Is.EqualTo(@"1990"));

            Assert.That(Parsers.GetInfoBoxFieldValue(@"hello {{infobox foo
|  Year  =  1990
|other=great}} now", Year), Is.EqualTo(@"1990"));

            // no infobox
            Assert.That(Parsers.GetInfoBoxFieldValue(@"hello now", Year), Is.Empty);

            // field not found
            Assert.That(Parsers.GetInfoBoxFieldValue(@"hello {{infobox foo
|  Year  =  1990
|other=great}} now", new List<string>(new[] { "yearly" })), Is.Empty);

            // multiple fields on same line
            Assert.That(Parsers.GetInfoBoxFieldValue(@"hello {{infobox foo
|  Year  =  1990  |some=where
|other=great}} now", Year), Is.EqualTo(@"1990"));

            // comments/refs
            Assert.That(Parsers.GetInfoBoxFieldValue(@"hello {{infobox foo
|  Year  =  <!--1990-->
|other=great}} now", Year), Is.Empty);

            Assert.That(Parsers.GetInfoBoxFieldValue(@"hello {{infobox foo
|  Year  =  <ref>1990</ref>
|other=great}} now", Year), Is.Empty);
            Assert.That(Parsers.GetInfoBoxFieldValue(@"hello {{infobox foo
|  Year  =  {{efn|1990}}
|other=great}} now", Year), Is.Empty);

        }

        [Test]
        public void RemoveDuplicateWikiLinks()
        {
            // removes duplicate piped wikilinks on same line
            Assert.That(Parsers.RemoveDuplicateWikiLinks(@"now [[foo|bar]] was [[foo|bar]] too"), Is.EqualTo(@"now [[foo|bar]] was bar too"));
            Assert.That(Parsers.RemoveDuplicateWikiLinks(@"now [[foo|bar]] was [[foo|bar]]s too"), Is.EqualTo(@"now [[foo|bar]] was bars too"));

            // multiline – no change
            Assert.That(Parsers.RemoveDuplicateWikiLinks(@"now [[foo|bar]]
was [[foo|bar]] too"), Is.EqualTo(@"now [[foo|bar]]
was [[foo|bar]] too"));

            // case sensitive
            Assert.That(Parsers.RemoveDuplicateWikiLinks(@"now [[foo|bar]] was [[Foo|bar]] too"), Is.EqualTo(@"now [[foo|bar]] was [[Foo|bar]] too"));
            Assert.That(Parsers.RemoveDuplicateWikiLinks(@"now [[foo bar]] was [[Foo bar]] too"), Is.EqualTo(@"now [[foo bar]] was [[Foo bar]] too"));

            // removes duplicate unpiped wikilinks
            Assert.That(Parsers.RemoveDuplicateWikiLinks(@"now [[foo bar]] was [[foo bar]] too"), Is.EqualTo(@"now [[foo bar]] was foo bar too"));

            // no changes
            Assert.That(Parsers.RemoveDuplicateWikiLinks(@"now [[foo|bar]] was [[foo]] too"), Is.EqualTo(@"now [[foo|bar]] was [[foo]] too"));
        }

        [Test]
        public void CanonicalizeTitleRawTests()
        {
            Assert.That(Parsers.CanonicalizeTitleRaw(@"foo_bar"), Is.EqualTo(@"foo bar"));
            Assert.That(Parsers.CanonicalizeTitleRaw(@"foo%22bar%22"), Is.EqualTo(@"foo""bar"""));
            Assert.That(Parsers.CanonicalizeTitleRaw(@"foo+bar"), Is.EqualTo(@"foo+bar"));
            Assert.That(Parsers.CanonicalizeTitleRaw(@"foo_bar", false), Is.EqualTo(@"foo bar"));
            Assert.That(Parsers.CanonicalizeTitleRaw(@"foo_bar ", false), Is.EqualTo(@"foo bar "));
            Assert.That(Parsers.CanonicalizeTitleRaw(@"foo_bar", true), Is.EqualTo(@"foo bar"));
            Assert.That(Parsers.CanonicalizeTitleRaw(@"foo_bar ", true), Is.EqualTo(@"foo bar"));
            Assert.That(Parsers.CanonicalizeTitleRaw(@" foo_bar", true), Is.EqualTo(@"foo bar"));

            Assert.That(Parsers.CanonicalizeTitleRaw(@"Bugs#If_a_selflink_is_also_bolded%2C_AWB_should"), Is.EqualTo(@"Bugs#If a selflink is also bolded, AWB should"));
        }

        [Test]
        public void RemoveTemplateNamespace()
        {
            Assert.That(Parsers.RemoveTemplateNamespace(@"{{Template:foo}}"), Is.EqualTo(@"{{foo}}"));
            Assert.That(Parsers.RemoveTemplateNamespace(@"{{Template:Foo}}"), Is.EqualTo(@"{{Foo}}"));
            Assert.That(Parsers.RemoveTemplateNamespace(@"{{Template:foo_bar}}"), Is.EqualTo(@"{{foo bar}}"));
            Assert.That(Parsers.RemoveTemplateNamespace(@"Template:Foo"), Is.EqualTo(@"Template:Foo"), "no change if it is not a real template");
            Assert.That(Parsers.RemoveTemplateNamespace(@"[[Template:Foo]]"), Is.EqualTo(@"[[Template:Foo]]"), "no change if it is not a real template");
            Assert.That(Parsers.RemoveTemplateNamespace(@"{{Clarify|date=May 2014|reason=Use Template:Cite web or similar}}"), Is.EqualTo(@"{{Clarify|date=May 2014|reason=Use Template:Cite web or similar}}"), "no change if it is part of a comment");
            Assert.That(Parsers.RemoveTemplateNamespace(@"{{Template:Foo|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"), Is.EqualTo(@"{{Foo|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"));
        }

        [Test]
        public void FixLinkWhitespaceSpacing()
        {
            Assert.That(Parsers.FixLinkWhitespace("[[Foo]]", "Test"), Is.EqualTo("[[Foo]]"));
            Assert.That(Parsers.FixLinkWhitespace("[[Foo Bar]]", "Test"), Is.EqualTo("[[Foo Bar]]"));
            Assert.That(Parsers.FixLinkWhitespace("[[Foo  Bar]]", "Test"), Is.EqualTo("[[Foo Bar]]"));
            Assert.That(Parsers.FixLinkWhitespace("[[Foo  Bar  was]]", "Test"), Is.EqualTo("[[Foo Bar was]]"), "fixes multiple double spaces in single link");
            Assert.That(Parsers.FixLinkWhitespace("[[Foo  Bar|Bar]]", "Test"), Is.EqualTo("[[Foo Bar|Bar]]"));
            Assert.That(Parsers.FixLinkWhitespace("[[64th Fighter Aviation Corps|64th  Fighter Aviation Corps]]", "Test"), Is.EqualTo("[[64th Fighter Aviation Corps|64th Fighter Aviation Corps]]"), "long wikilink double space");

            Assert.That(Parsers.FixLinkWhitespace(@"[[      Foo    ]] was", "Test"), Is.EqualTo(@"[[Foo]] was"), "multiple spaces, space after");
            Assert.That(Parsers.FixLinkWhitespace(@"[[      Foo2    ]]A", "Test"), Is.EqualTo(@"[[Foo2]] A"), "multiple spaces, word after");
            Assert.That(Parsers.FixLinkWhitespace(@"[[      Foo3]]", "Test"), Is.EqualTo(@"[[Foo3]]"), "multiple spaces only at start");
        }

        [Test]
        public void FixLink()
        {
            bool nochange;
            Assert.That(Parsers.FixLinks(@"[[Foo_bar]]", "a", out nochange), Is.EqualTo(@"[[Foo bar]]"));
            Assert.IsFalse(nochange);

            const string doubleApos = @"[[Image:foo%27%27s.jpg|thumb|200px|Bar]]";
            Assert.That(Parsers.FixLinks(doubleApos, "a", out nochange), Is.EqualTo(doubleApos));

            Variables.AddUnderscoredTitles(new List<string>(new[] {"Size t", "Mod perl", "Mod mono" }));

            Assert.That(Parsers.FixLinks(@"[[size_t]]", "a", out nochange), Is.EqualTo(@"[[size_t]]"));
            Assert.IsTrue(nochange);
            Assert.That(Parsers.FixLinks(@"[[mod_perl]]", "a", out nochange), Is.EqualTo(@"[[mod_perl]]"));
            Assert.IsTrue(nochange);
            Assert.That(Parsers.FixLinks(@"[[mod_mono]]", "a", out nochange), Is.EqualTo(@"[[mod_mono]]"));
            Assert.IsTrue(nochange);
            Assert.That(Parsers.FixLinks(@"[[Mod_mono]]", "a", out nochange), Is.EqualTo(@"[[Mod_mono]]"));
            Assert.IsTrue(nochange);
            Assert.That(Parsers.FixLinks(@"[[E|Mod_mono]]", "a", out nochange), Is.EqualTo(@"[[E|Mod_mono]]"));
            Assert.IsTrue(nochange);
            Assert.That(Parsers.FixLinks(@"[[Mod_mono#link]]", "a", out nochange), Is.EqualTo(@"[[Mod_mono#link]]"));
            Assert.IsTrue(nochange);

            Assert.That(Parsers.FixLinks(@"[[List of The Amazing Spider-Man issues#The Amazing Spider-Man #648–current x|List of issues]]", "a", out nochange), Is.EqualTo(@"[[List of The Amazing Spider-Man issues#The Amazing Spider-Man #648–current x|List of issues]]"), "Does not break section links with hash and space");
            Assert.That(Parsers.FixLinks(@"[[Foo#nice%20example|F]]", "a", out nochange), Is.EqualTo(@"[[Foo#nice example|F]]"), "%20 replaced in target");
            Assert.That(Parsers.FixLinks(@"[[Foo|bar|]]", "a", out nochange), Is.EqualTo(@"[[Foo|bar]]"), "Fixes excess trailing pipe");
            Assert.That(Parsers.FixLinks(@"[[Foo|bar | ]]", "a", out nochange), Is.EqualTo(@"[[Foo|bar]]"), "Fixes excess trailing pipe");

            Assert.That(Parsers.FixLinks(@"[[Foo_bar]]", "a", out nochange), Is.EqualTo(@"[[Foo bar]]"), "Fixes underscore");
            Assert.That(Parsers.FixLinks(@"[[Foo_bar#ab_c]]", "a", out nochange), Is.EqualTo(@"[[Foo bar#ab c]]"), "Fixes underscore");
            Assert.That(Parsers.FixLinks(@"[[Mercedes-Benz C-Class#W202 %281993%E2%80%932000%29|Mercedes-Benz C-Class]]", "a", out nochange), Is.EqualTo(@"[[Mercedes-Benz C-Class#W202 (1993–2000)|Mercedes-Benz C-Class]]"), "Fixes percent encoding");

            Assert.That(Parsers.FixLinks(@"[[foo{{!}}bar]]", "a", out nochange), Is.EqualTo(@"[[foo|bar]]"), "Fixes pipe");
            Assert.That(Parsers.FixLinks(@"[[foo{{!}}]]", "a", out nochange), Is.EqualTo(@"[[foo|]]"), "Fixes pipe");
            Assert.That(Parsers.FixLinks(@"[[Repeat sign|{{!}}: ]]", "a", out nochange), Is.EqualTo("[[Repeat sign|{{!}}: ]]"), "Do nothing in the only exception");

            Assert.That(Parsers.FixLinks(@"[[|foo]]", "a", out nochange), Is.EqualTo(@"[[|foo]]"), "No change if single leading pipe");
            Assert.That(Parsers.FixLinks(@"[[|foo|bar]]", "a", out nochange), Is.EqualTo(@"[[foo|bar]]"), "Fixes excess leading pipe");
            Assert.That(Parsers.FixLinks(@"[[|  foo|bar]]", "a", out nochange), Is.EqualTo(@"[[foo|bar]]"), "Fixes excess leading pipe & whitespace");
            Assert.That(Parsers.FixLinks(@"[[|thumb|300px]]", "a", out nochange), Is.EqualTo(@"[[|thumb|300px]]"), "Nochange: thumb");
            Assert.That(Parsers.FixLinks(@"[[|thumbnail|300px]]", "a", out nochange), Is.EqualTo(@"[[|thumbnail|300px]]"), "Nochange: thumbnail");

            Assert.That(Parsers.FixLinks(@"A
[[Category:Soldiers|XXX]]", "Category:Soldiers", out nochange), Is.EqualTo(@"A" + "\r\n"), "Category self link");

            const string iarchive = @"[[iarchive:a0000camp_y0p5/page/82/mode/2up|Foo]]";
            Assert.That(Parsers.FixLinks(iarchive, "A", out nochange), Is.EqualTo(iarchive), "iarchive");
        }

        [Test]
        public void FixLinkBoldItalic()
        {
            bool nochange;
            Assert.That(Parsers.FixLinks(@"[[foo|''foo'']]", "a", out nochange), Is.EqualTo(@"''[[foo|foo]]''"));
            Assert.IsFalse(nochange);
            Assert.That(Parsers.FixLinks(@"[[foo|'''Foo''']]", "a", out nochange), Is.EqualTo(@"'''[[foo|Foo]]'''"));
            Assert.IsFalse(nochange);
            Assert.That(Parsers.FixLinks(@"[[foo|'''''Foo''''']]", "a", out nochange), Is.EqualTo(@"'''''[[foo|Foo]]'''''"));
            Assert.IsFalse(nochange);

            Assert.That(Parsers.FixLinks(@"[[foo|'''Foo''']] [[bar|''bar'']]", "a", out nochange), Is.EqualTo(@"'''[[foo|Foo]]''' ''[[bar|bar]]''"));
            Assert.That(Parsers.FixLinks(@"''[[foo|'''foo''']]''", "a", out nochange), Is.EqualTo(@"''[[foo|'''foo''']]''"));
            Assert.IsTrue(nochange);

            Assert.That(Parsers.FixLinks(@"[[foo|'''Foo''']] ''[[foor|bar]]''", "a", out nochange), Is.EqualTo(@"'''[[foo|Foo]]''' ''[[foor|bar]]''"));

            // No change to single apostrophes
            Assert.That(Parsers.FixLinks(@"[[foo|'bar']]", "a", out nochange), Is.EqualTo(@"[[foo|'bar']]"));
            Assert.IsTrue(nochange);

            // No change if (dodgy) apostrope just after link
            Assert.That(Parsers.FixLinks(@"[[foo|''bar'']]'", "a", out nochange), Is.EqualTo(@"[[foo|''bar'']]'"));
            Assert.IsTrue(nochange);

            Assert.That(Parsers.FixLinks(@"[[foo|]]", "a", out nochange), Is.EqualTo(@"[[foo|]]"));
            Assert.IsTrue(nochange);

            Assert.That(Parsers.FixLinks(@"[[foo|''b'']]", "a", out nochange), Is.EqualTo(@"[[foo|''b'']]"));
            Assert.IsTrue(nochange);

            // No change to part of link text in bold/italics
            Assert.That(Parsers.FixLinks(@"[[foo|A ''bar'']]", "a", out nochange), Is.EqualTo(@"[[foo|A ''bar'']]"));
            Assert.IsTrue(nochange);
            Assert.That(Parsers.FixLinks(@"[[foo|A '''bar''']]", "a", out nochange), Is.EqualTo(@"[[foo|A '''bar''']]"));
            Assert.IsTrue(nochange);
        }

        [Test]
        public void FixSelfInterwikis()
        {
            bool nochange;

            Assert.That(Parsers.FixLinks(@"[[en:Foo]]", "Bar", out nochange), Is.EqualTo(@"[[Foo]]"));
            Assert.That(Parsers.FixLinks(@"[[en:Foo|Bar]]", "T1", out nochange), Is.EqualTo(@"[[Foo|Bar]]"));

            const string FrIW = @"Now [[fr:Here]]";
            Assert.That(Parsers.FixLinks(FrIW, "Bar", out nochange), Is.EqualTo(FrIW));
            Assert.IsTrue(parser.SortInterwikis);

            #if DEBUG
            Variables.SetProjectSimple("en", ProjectEnum.commons);
            const string EnInterwiki = @"[[en:Foo]]";
            Assert.That(Parsers.FixLinks(EnInterwiki, "Test", out nochange), Is.EqualTo(EnInterwiki), "en interwiki not changed on commons");
            
            Variables.SetProjectSimple("en", ProjectEnum.wikipedia);
            Assert.That(Parsers.FixLinks(EnInterwiki, "Test", out nochange), Is.EqualTo("[[Foo]]"), "self-interwiki converted on en-wiki");
            #endif
        }

        [Test]
        public void HasSeeAlsoAfterNotesReferencesOrExternalLinks()
        {
            const string start = @"start
", seeAlso = @"Foo
==See also==
x
", extlinks = @"==External links==
x
", notes = @"==Notes==
x
", references = @"== References==
x
", notes4 = @"==== Notes ====
x
";
            Assert.IsFalse(Parsers.HasSeeAlsoAfterNotesReferencesOrExternalLinks(""), "Empty string check");
            Assert.IsFalse(Parsers.HasSeeAlsoAfterNotesReferencesOrExternalLinks(start + seeAlso), "Only see also");
            Assert.IsFalse(Parsers.HasSeeAlsoAfterNotesReferencesOrExternalLinks(start + seeAlso + extlinks), "see also external links");

            Assert.IsTrue(Parsers.HasSeeAlsoAfterNotesReferencesOrExternalLinks(start + extlinks + seeAlso), "external links then see also");
            Assert.IsTrue(Parsers.HasSeeAlsoAfterNotesReferencesOrExternalLinks(start + notes + seeAlso), "notes then see also");
            Assert.IsTrue(Parsers.HasSeeAlsoAfterNotesReferencesOrExternalLinks(start + references + seeAlso), "refs then see also");
            Assert.IsTrue(Parsers.HasSeeAlsoAfterNotesReferencesOrExternalLinks(start + references + seeAlso + notes), "refs then see also then notes");
            Assert.IsFalse(Parsers.HasSeeAlsoAfterNotesReferencesOrExternalLinks(start + notes4 + seeAlso), "L4 notes ignored");
        }

        [Test]
        public void UnclosedTags()
        {
            Dictionary<int, int> uct = new Dictionary<int, int>();

            uct = Parsers.UnclosedTags(@"<pre>bar</pre>");
            Assert.That(uct.Count, Is.EqualTo(0));

            uct = Parsers.UnclosedTags(@"<gallery>File:bar</gallery>");
            Assert.That(uct.Count, Is.EqualTo(0));

            uct = Parsers.UnclosedTags(@"<math>bar</math>");
            Assert.That(uct.Count, Is.EqualTo(0));

            uct = Parsers.UnclosedTags(@"<math chem>bar</math>");
            Assert.That(uct.Count, Is.EqualTo(0));

            uct = Parsers.UnclosedTags(@"<source>bar</source> <ref name=Foo>boo</ref>");
            Assert.That(uct.Count, Is.EqualTo(0));

            uct = Parsers.UnclosedTags(@"<pre>bar</pre> <ref name=Foo>boo</ref>");
            Assert.That(uct.Count, Is.EqualTo(0));

            uct = Parsers.UnclosedTags(@"<source lang=""text"">
<typename T>
<std::string, T>
<typename T>
<std::string, T>
<std::string, T> 
<std::string, int>
<int>
<typename T>
<std::string, T>
<std::string, int>
<int>
</source>");
            Assert.That(uct.Count, Is.EqualTo(0), "Ignore other tags");

            uct = Parsers.UnclosedTags(@"<pre>bar</pre> <math> not ended");
            Assert.That(uct.Count, Is.EqualTo(1), "math");
            Assert.IsTrue(uct.ContainsKey(15));
            Assert.IsTrue(uct.ContainsValue(6));

            uct = Parsers.UnclosedTags(@"<pre>bar</pre> <center> not ended");
            Assert.That(uct.Count, Is.EqualTo(1), "center");
            Assert.IsTrue(uct.ContainsKey(15));
            Assert.IsTrue(uct.ContainsValue(8));

            uct = Parsers.UnclosedTags(@"<pre>bar</pre> <center> <center>a</center> not ended");
            Assert.That(uct.Count, Is.EqualTo(1), "center double");
            Assert.IsTrue(uct.ContainsKey(15), "center key");
            Assert.IsTrue(uct.ContainsValue(8), "center value");

            uct = Parsers.UnclosedTags(@"<pre>bar</pre> <sup> <sup>a</sup> not ended");
            Assert.That(uct.Count, Is.EqualTo(1));
            Assert.IsTrue(uct.ContainsKey(15), "sup key");
            Assert.IsTrue(uct.ContainsValue(5), "sup value");

            uct = Parsers.UnclosedTags(@"<pre>bar</pre> <sub> <sub>a</sub> not ended");
            Assert.That(uct.Count, Is.EqualTo(1), "sub");
            Assert.IsTrue(uct.ContainsKey(15), "sub key");
            Assert.IsTrue(uct.ContainsValue(5), "sub value");

            uct = Parsers.UnclosedTags(@"<pre>bar</pre> <ref name=<ref name=Foo/> not ended");
            Assert.That(uct.Count, Is.EqualTo(1), "ref");
            Assert.IsTrue(uct.ContainsKey(15));
            Assert.IsTrue(uct.ContainsValue(35));

            uct = Parsers.UnclosedTags(@"<pre>bar</pre> <ref> not ended");
            Assert.That(uct.Count, Is.EqualTo(1), "ref 2");
            Assert.IsTrue(uct.ContainsKey(15));
            Assert.IsTrue(uct.ContainsValue(5));

            uct = Parsers.UnclosedTags(@"<pre>bar</pre> <ref name='foo'> not ended");
            Assert.That(uct.Count, Is.EqualTo(1), "ref name");
            Assert.IsTrue(uct.ContainsKey(15));
            Assert.IsTrue(uct.ContainsValue(16));

            uct = Parsers.UnclosedTags(@"<pre>bar</pre> <source lang=""bar""> not ended");
            Assert.That(uct.Count, Is.EqualTo(1), "source");
            Assert.IsTrue(uct.ContainsKey(15));

            uct = Parsers.UnclosedTags(@"<pre>bar</pre> <small> not ended");
            Assert.That(uct.Count, Is.EqualTo(1), "small");
            Assert.IsTrue(uct.ContainsKey(15));

            uct = Parsers.UnclosedTags(@"<pre>bar</pre> < code> not ended");
            Assert.That(uct.Count, Is.EqualTo(1), "code");
            Assert.IsTrue(uct.ContainsKey(15));

            uct = Parsers.UnclosedTags(@"<pre>bar</pre> < nowiki > not ended");
            Assert.That(uct.Count, Is.EqualTo(1), "nowiki");
            Assert.IsTrue(uct.ContainsKey(15));

            uct = Parsers.UnclosedTags(@"<pre>bar</pre> <pre> not ended");
            Assert.That(uct.Count, Is.EqualTo(1), "pre");
            Assert.IsTrue(uct.ContainsKey(15));

            uct = Parsers.UnclosedTags(@"<pre>bar</pre> </pre> not opened");
            Assert.That(uct.Count, Is.EqualTo(1), "/pre");
            Assert.IsTrue(uct.ContainsKey(15));

            uct = Parsers.UnclosedTags(@"<pre>bar</pre> <gallery> not ended");
            Assert.That(uct.Count, Is.EqualTo(1), "gallery");
            Assert.IsTrue(uct.ContainsKey(15));

            uct = Parsers.UnclosedTags(@"<pre>bar</pre> </gallery> not opened");
            Assert.That(uct.Count, Is.EqualTo(1), "/gallery");
            Assert.IsTrue(uct.ContainsKey(15));

            uct = Parsers.UnclosedTags(@"<pre>bar</pre> <!-- not ended");
            Assert.That(uct.Count, Is.EqualTo(1), "<!--");
            Assert.IsTrue(uct.ContainsKey(15));

            uct = Parsers.UnclosedTags(@"<!--bar--> <!-- not ended");
            Assert.That(uct.Count, Is.EqualTo(1), "<!-- 2");
            Assert.IsTrue(uct.ContainsKey(11));

            uct = Parsers.UnclosedTags(@"<!--not ended
<!--  ended -->");
            Assert.That(uct.Count, Is.EqualTo(1), "Comment within unclosed comment");

            uct = Parsers.UnclosedTags(@"<gallery> not ended <gallery>bar</gallery>");
            Assert.That(uct.Count, Is.EqualTo(1), "gallery 2");
            Assert.IsTrue(uct.ContainsKey(20));

            uct = Parsers.UnclosedTags(@"<gallery other='a'> not ended <gallery other='a'>bar</gallery>");
            Assert.That(uct.Count, Is.EqualTo(1), "gallery param");
            Assert.IsTrue(uct.ContainsKey(30));

            uct = Parsers.UnclosedTags(@"<gallery>A|<div><small>(1717)</small><br/><small><small>Munich</small></div></gallery>");
            Assert.That(uct.Count, Is.EqualTo(1), "small div");
            Assert.IsTrue(uct.ContainsKey(42));

            uct = Parsers.UnclosedTags(@"<small><small><small><small><small><small><small><small><small><small><small><small>");
            Assert.That(uct.Count, Is.EqualTo(12), "multiple unclosed small");
            uct = Parsers.UnclosedTags(@"<small><small><small><small><small><small><small><small><small><small><small><small> <!-- <math> -->");
            Assert.That(uct.Count, Is.EqualTo(12), "multiple unclosed small, ignore comments");
            uct = Parsers.UnclosedTags(@"<small><small><small><small><small><small><small><small><small><small><small><small> <small>></small>");
            Assert.That(uct.Count, Is.EqualTo(14), "multiple unclosed small, but counts tag containing >");
            uct = Parsers.UnclosedTags(@"<small><small><small><small><small><small><small><small><small><small><small><small> <ref name=fo>a</ref> <gallery param=a>b</gallery>");
            Assert.That(uct.Count, Is.EqualTo(12), "multiple unclosed small, don't count ref/gallery with params");

            uct = Parsers.UnclosedTags(@"<code>a</code><code>a</code><code>a</code><code>a</code><code>a</code><code>a</code>
<code>a</code><code>a</code><code>a</code><code>a</code><code>a</code><code>a</code>
<code> a <some-tag> </code>");
            Assert.That(uct.Count, Is.EqualTo(0), "No unclosed code tags here, don't report code tags just because of another tag");

            uct = Parsers.UnclosedTags(@"<code> a <'foo'> </code>
<code> a <'foo'> </code>
<code> a <'foo'> </code>
<code> a <'foo'> </code>
<code> a <'foo'> </code>
<code> a <'foo'> </code>
<code> a <'foo'> </code>
<code> a <'foo'> </code>
<code> a <'foo'> </code>
<code> a <'foo'> </code>
<code> a <'foo'> </code>
<code> a <'foo'> </code>");
            Assert.That(uct.Count, Is.EqualTo(0), "No unclosed code tags here, ignore what aren't genuine tags");
        }

        [Test]
        public void TestBulletExternalLinks()
        {
            string s = Parsers.BulletExternalLinks(@"==External links==
http://example.com/foo
[http://example.com foo]
{{aTemplate|url=
http://example.com }}");

            Assert.That(s, Does.Contain("* http://example.com/foo"));
            Assert.That(s, Does.Contain("* [http://example.com foo]"));

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_2#Incorrect_bulleting
            Assert.That(s, Does.Contain("\r\nhttp://example.com }}"));
            
            string NoChange = @"==External links==
* http://example.com/foo";
            Assert.That(Parsers.BulletExternalLinks(NoChange), Is.EqualTo(NoChange));
        }

        [Test]
        public void TestFixLinkWhitespace()
        {
            Assert.That(Parsers.FixLinkWhitespace("b[[ a ]]c", "foo"), Is.EqualTo("b [[a]] c")); // regexes 1 & 2
            Assert.That(Parsers.FixLinkWhitespace("b   [[ a ]]  c", "foo"), Is.EqualTo("b   [[a]]  c")); // 4 & 5

            Assert.That(Parsers.FixLinkWhitespace("[[a ]]b", "foo"), Is.EqualTo("[[a]] b"));
            Assert.That(Parsers.FixLinkWhitespace(@"Now
[[a ]]b", "foo"), Is.EqualTo(@"Now
[[a]] b"), "wikilink on start of line");
            Assert.That(Parsers.FixLinkWhitespace("[[a ]]" + "\r\n", "foo"), Is.EqualTo("[[a]]" + "\r\n"));

            Assert.That(Parsers.FixLinkWhitespace("[[foo  bar]]", "foot"), Is.EqualTo("[[foo bar]]"));
            Assert.That(Parsers.FixLinkWhitespace("[[foo  bar]]", ""), Is.EqualTo("[[foo bar]]"));
            Assert.That(Parsers.FixLinkWhitespace("[[foo     bar]]", "foot"), Is.EqualTo("[[foo bar]]"));
            Assert.That(Parsers.FixLinkWhitespace("dat is [[ foo   bar ]] show!", "foot"), Is.EqualTo("dat is [[foo bar]] show!"));
            Assert.That(Parsers.FixLinkWhitespace("dat is[[ foo   bar ]]show!", "foot"), Is.EqualTo("dat is [[foo bar]] show!"));

            Assert.That(Parsers.FixLinkWhitespace(@"His [[Tiger Woods# Career]] was", "Tiger Woods"), Is.EqualTo(@"His [[Tiger Woods#Career]] was"));

            // don't fix when bit before # is not article name
            Assert.That(Parsers.FixLinkWhitespace(@"Fred's [[Smith# Career]] was", "Fred"), Is.Not.SameAs(@"Fred's [[Smith#Career]] was"));

            Assert.That(Parsers.FixLinkWhitespace(@"[[Category:London| ]]", "foo"), Is.EqualTo(@"[[Category:London| ]]")); // leading space NOT removed from cat sortkey
            Assert.That(Parsers.FixLinkWhitespace(@"[[Category:Slam poetry| ]] ", "foo"), Is.EqualTo(@"[[Category:Slam poetry| ]] ")); // leading space NOT removed from cat sortkey

            // shouldn't fix - not enough information
            // Assert.AreEqual("[[ a ]]", Parsers.FixLinkWhitespace("[[ a ]]", "foo"));
            // disabled for the time being to avoid unnecesary clutter

            Assert.That(Parsers.FixLinkWhitespace("[[foo #bar]]", "foot"), Is.EqualTo("[[foo#bar]]"));
            Assert.That(Parsers.FixLinkWhitespace("[[foo # bar]]", "foot"), Is.EqualTo("[[foo#bar]]"));
            Assert.That(Parsers.FixLinkWhitespace("[[foo# bar]]", "foot"), Is.EqualTo("[[foo#bar]]"));
            Assert.That(Parsers.FixLinkWhitespace("[[foo #bar]]te", "foot"), Is.EqualTo("[[foo#bar]]te"));
            Assert.That(Parsers.FixLinkWhitespace("[[foo  #bar|te]]", "foot"), Is.EqualTo("[[foo#bar|te]]"));

            Assert.That(Parsers.FixLinkWhitespace(@"[[List of The Amazing Spider-Man issues#The Amazing Spider-Man #648–current x|List of issues]]", "x"), Is.EqualTo(@"[[List of The Amazing Spider-Man issues#The Amazing Spider-Man #648–current x|List of issues]]"), "Does not break section links with hash and space");

            Assert.That(Parsers.FixLinkWhitespace("[[A# code]]", "Test"), Is.EqualTo("[[A# code]]"));
            Assert.That(Parsers.FixLinkWhitespace("[[C# code]]", "Test"), Is.EqualTo("[[C# code]]"));
            Assert.That(Parsers.FixLinkWhitespace("[[F# code]]", "Test"), Is.EqualTo("[[F# code]]"));
            Assert.That(Parsers.FixLinkWhitespace("[[J# code]]", "Test"), Is.EqualTo("[[J# code]]"));

            Assert.That(Parsers.FixLinkWhitespace("<ref>[[ abcdef-abcdef-abcdef-abcdef-abcdef-abcdef-abcdef]]</ref>", "foo"), Is.EqualTo("<ref>[[abcdef-abcdef-abcdef-abcdef-abcdef-abcdef-abcdef]]</ref>"));
        }

        [Test]
        public void TestCanonicalizeTitle()
        {
            Assert.That(Parsers.CanonicalizeTitle("foo_bar"), Is.EqualTo("foo bar"));
            Assert.That(Parsers.CanonicalizeTitle("foo bar"), Is.EqualTo("foo bar"));
            Assert.That(Parsers.CanonicalizeTitle("foo_%28bar%29"), Is.EqualTo("foo (bar)"));
            Assert.That(Parsers.CanonicalizeTitle(@"foo%2Bbar"), Is.EqualTo(@"foo+bar"));
            Assert.That(Parsers.CanonicalizeTitle("foo_(bar)"), Is.EqualTo("foo (bar)"));
            Assert.That(Parsers.CanonicalizeTitle("Template:foo_bar"), Is.EqualTo("Template:foo bar"));
            Assert.That(Parsers.CanonicalizeTitle("W202 %281993%E2%80%932000%29"), Is.EqualTo("W202 (1993–2000)"));

            Assert.That(Parsers.CanonicalizeTitle("foo_bar}}"), Is.EqualTo("foo_bar}}"), "no change to invalid title");

            // it may or may not fix it, but shouldn't break anything
            Assert.That(Parsers.CanonicalizeTitle("foo_bar{{bar_boz}}"), Does.Contain("{{bar_boz}}"));

            Variables.UnderscoredTitles.Add(@"Foo bar");
            Assert.That(Parsers.CanonicalizeTitle("foo_bar"), Is.EqualTo("foo_bar"));
            Assert.That(Parsers.CanonicalizeTitle("Foo_bar"), Is.EqualTo("Foo_bar"));
        }

        [Test]
        public void CanonicalizeTitleAggressively()
        {
            Assert.That(Parsers.CanonicalizeTitleAggressively("Foo"), Is.EqualTo("Foo"));

            Assert.That(Parsers.CanonicalizeTitleAggressively("foo_%28bar%29#anchor"), Is.EqualTo("Foo (bar)"));
            Assert.That(Parsers.CanonicalizeTitleAggressively("project : foo"), Is.EqualTo("Wikipedia:Foo"));
            Assert.That(Parsers.CanonicalizeTitleAggressively("Image: foo.jpg "), Is.EqualTo("File:Foo.jpg"));

            // a bit of ambiguousness here, but
            // https://en.wikipedia.org/wiki/Wikipedia:AWB/B#Problem_.28on_runecape_wikia.29_with_articles_with_.2B_in_the_name.
            Assert.That(Parsers.CanonicalizeTitleAggressively("Romeo+Juliet"), Is.EqualTo("Romeo+Juliet"));

            Assert.That(Parsers.CanonicalizeTitleAggressively(":Foo"), Is.EqualTo("Foo"));
            Assert.That(Parsers.CanonicalizeTitleAggressively(": Foo"), Is.EqualTo("Foo"));
            Assert.That(Parsers.CanonicalizeTitleAggressively("::Foo"), Is.EqualTo(":Foo"));
            Assert.That(Parsers.CanonicalizeTitleAggressively(":user:Foo"), Is.EqualTo("User:Foo"));

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_14#List_of_.22User_talk:.22_pages_change_to_list_of_.22Talk:.22_pages_when_started
            Assert.That(Parsers.CanonicalizeTitleAggressively("User talk:Foo"), Is.EqualTo("User talk:Foo"));
        }

        [Test]
        public void TestFixEmptyLinksAndTemplates()
        {
            Assert.That(Parsers.FixEmptyLinksAndTemplates(""), Is.Empty);

            Assert.That(Parsers.FixEmptyLinksAndTemplates("Test"), Is.EqualTo("Test"));

            Assert.That(Parsers.FixEmptyLinksAndTemplates("{{Test}}"), Is.EqualTo("{{Test}}"));

            Assert.That(Parsers.FixEmptyLinksAndTemplates("Test\r\n{{Test}}"), Is.EqualTo("Test\r\n{{Test}}"));

            Assert.That(Parsers.FixEmptyLinksAndTemplates("{{}}"), Is.Empty);
            Assert.That(Parsers.FixEmptyLinksAndTemplates("{{ }}"), Is.Empty);
            Assert.That(Parsers.FixEmptyLinksAndTemplates("{{|}}"), Is.Empty);
            Assert.That(Parsers.FixEmptyLinksAndTemplates("{{||||||||||||||||}}"), Is.Empty);
            Assert.That(Parsers.FixEmptyLinksAndTemplates("{{|||| |||||||                      ||  |||}}"), Is.Empty);

            Assert.That(Parsers.FixEmptyLinksAndTemplates("{{Template:}}"), Is.Empty, "Template:");
            Assert.That(Parsers.FixEmptyLinksAndTemplates("{{Template: }}"), Is.Empty, "Template: with space");
            Assert.That(Parsers.FixEmptyLinksAndTemplates("{{Template:     |||}}"), Is.Empty);

            Assert.That(Parsers.FixEmptyLinksAndTemplates("{{  }}{{Test}}{{Template: ||}}"), Is.EqualTo("{{Test}}"));

            Assert.That(Parsers.FixEmptyLinksAndTemplates("[[Test]]"), Is.EqualTo("[[Test]]"));
            Assert.That(Parsers.FixEmptyLinksAndTemplates("[[Test|Bar]]"), Is.EqualTo("[[Test|Bar]]"));
            Assert.That(Parsers.FixEmptyLinksAndTemplates("[[|Bar]]"), Is.EqualTo("[[|Bar]]"));

            Assert.That(Parsers.FixEmptyLinksAndTemplates("[[]]"), Is.Empty);
            Assert.That(Parsers.FixEmptyLinksAndTemplates("[[[[  ]]]]"), Is.Empty);
            Assert.That(Parsers.FixEmptyLinksAndTemplates("[[  ]]"), Is.Empty);

            Assert.That(Parsers.FixEmptyLinksAndTemplates("[[Category:Test]]"), Is.EqualTo("[[Category:Test]]"));
            Assert.That(Parsers.FixEmptyLinksAndTemplates("Text [[Category:Test]]"), Is.EqualTo("Text [[Category:Test]]"));

            Assert.That(Parsers.FixEmptyLinksAndTemplates("[[Category:]]"), Is.Empty);
            Assert.That(Parsers.FixEmptyLinksAndTemplates("Text [[Category:]]"), Is.EqualTo("Text "));
            Assert.That(Parsers.FixEmptyLinksAndTemplates("[[Category:  ]]"), Is.Empty);

            Assert.That(Parsers.FixEmptyLinksAndTemplates("[[Image:Test]]"), Is.EqualTo("[[Image:Test]]"));
            Assert.That(Parsers.FixEmptyLinksAndTemplates("Text [[Image:Test]]"), Is.EqualTo("Text [[Image:Test]]"));
            Assert.That(Parsers.FixEmptyLinksAndTemplates("[[File:Test]]"), Is.EqualTo("[[File:Test]]"));
            Assert.That(Parsers.FixEmptyLinksAndTemplates("Text [[File:Test]]"), Is.EqualTo("Text [[File:Test]]"));

            Assert.That(Parsers.FixEmptyLinksAndTemplates("[[Image:]]"), Is.Empty);
            Assert.That(Parsers.FixEmptyLinksAndTemplates("Text [[Image:]]"), Is.EqualTo("Text "));
            Assert.That(Parsers.FixEmptyLinksAndTemplates("[[Image:  ]]"), Is.Empty);

            Assert.That(Parsers.FixEmptyLinksAndTemplates("[[File:]]"), Is.Empty);
            Assert.That(Parsers.FixEmptyLinksAndTemplates("Text [[File:]]"), Is.EqualTo("Text "));
            Assert.That(Parsers.FixEmptyLinksAndTemplates("[[File:  ]]"), Is.Empty);

            Assert.That(Parsers.FixEmptyLinksAndTemplates("[[File:]][[Image:]]"), Is.Empty);
            Assert.That(Parsers.FixEmptyLinksAndTemplates("[[File:Test]][[Image:]]"), Is.EqualTo("[[File:Test]]"));
        }
    }
}