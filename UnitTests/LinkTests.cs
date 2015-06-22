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
    public class LinkTests : RequiresParser
    {
        public GenfixesTestsBase genFixes  = new GenfixesTestsBase();

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

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_1#Link_de-piping_false_positive
            Assert.AreEqual("[[Sacramento, California|Sacramento]], California's [[capital city]]",
                            Parsers.StickyLinks("[[Sacramento, California|Sacramento]], California's [[capital city]]"));

            Assert.AreEqual("[[Russian literature|Russian literature]] was", Parsers.StickyLinks("[[Russian literature|Russian literature]] was"), "bugfix – no exception when pipe same length as target");
        }

        [Test]
        public void TestSimplifyLinks()
        {
            Assert.AreEqual("[[dog]]s", Parsers.SimplifyLinks("[[dog|dogs]]"));

            // case insensitivity of the first char
            Assert.AreEqual("[[dog]]s", Parsers.SimplifyLinks("[[Dog|dogs]]"));
            Assert.AreEqual("[[Dog]]s", Parsers.SimplifyLinks("[[dog|Dogs]]"));

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_8#Wrong_link_simplification_capitalisation
            Assert.AreEqual("[[dog]]", Parsers.SimplifyLinks("[[Dog|dog]]"));
            Assert.AreEqual("[[Dog]]", Parsers.SimplifyLinks("[[dog|Dog]]"));
            Assert.AreEqual("[[Dog]]", Parsers.SimplifyLinks("[[Dog|Dog]]"));

            Assert.AreEqual("[[dog]]s", Parsers.SimplifyLinks("[[Dog|dogs]]"));
            Assert.AreEqual("[[Dog]]s", Parsers.SimplifyLinks("[[dog|Dogs]]"));
            Assert.AreEqual("[[Dog]]s", Parsers.SimplifyLinks("[[Dog|Dogs]]"));
            Assert.AreEqual("#REDIRECT[[Dog]]", Parsers.SimplifyLinks("#REDIRECT[[dog|Dog]]"));
            Assert.AreEqual("[[funcy dog]]s", Parsers.SimplifyLinks("[[funcy dog|funcy dogs]]"));

            Assert.AreEqual("[[funcy dog]]", Parsers.SimplifyLinks("[[funcy dog|funcy_dog]]"), "handles underscore in text: text");
            Assert.AreEqual("[[funcy dog]]", Parsers.SimplifyLinks("[[funcy_dog|funcy dog]]"), "handles underscore in text: target");
            Assert.AreEqual("[[Funcy dog]]", Parsers.SimplifyLinks("[[funcy_dog|Funcy dog]]"), "handles underscore in text: target");

            Variables.UnderscoredTitles.Add("Funcy dog");
            Assert.AreEqual("[[funcy dog|funcy_dog]]", Parsers.SimplifyLinks("[[funcy dog|funcy_dog]]"), "handles underscore in text where article with underscore in title");
            Variables.UnderscoredTitles.Remove("Funcy dog");

            // ...and sensitivity of others
            Assert.AreEqual("[[dog|dOgs]]", Parsers.SimplifyLinks("[[dog|dOgs]]"));

            //https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_2#Inappropriate_link_compression
            Assert.AreEqual("[[foo|foo3]]", Parsers.SimplifyLinks("[[foo|foo3]]"));

            // don't touch suffixes with caps to avoid funky results like
            // https://en.wikipedia.org/w/index.php?diff=195760456
            Assert.AreEqual("[[FOO|FOOBAR]]", Parsers.SimplifyLinks("[[FOO|FOOBAR]]"));
            Assert.AreEqual("[[foo|fooBAR]]", Parsers.SimplifyLinks("[[foo|fooBAR]]"));

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_8#Only_one_spurious_space_removed_from_link
            Assert.AreEqual("[[Elizabeth Gunn]]", Parsers.SimplifyLinks("[[Elizabeth Gunn | Elizabeth Gunn]]"));
            Assert.AreEqual("[[Big Bend, Texas|Big Bend]]", Parsers.SimplifyLinks("[[Big Bend, Texas | Big Bend]]"));

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_8#SVN:_general_fixes_removes_whitespace_around_pipes_within_citation_templates
            Assert.AreEqual("{{foo|[[bar]] | boz}}]]", Parsers.SimplifyLinks("{{foo|[[bar]] | boz}}]]"));
            Assert.AreEqual("{{foo|[[bar]]\r\n| boz}}]]", Parsers.SimplifyLinks("{{foo|[[bar]]\r\n| boz}}]]"));

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_9#General_fixes_remove_spaces_from_category_sortkeys
            Assert.AreEqual("[[foo|bar]]", Parsers.SimplifyLinks("[[foo| bar]]"));
            Assert.AreEqual("[[foo|bar]]", Parsers.SimplifyLinks("[[foo| bar ]]"));
            Assert.AreEqual("[[Category:foo| bar]]", Parsers.SimplifyLinks("[[Category:foo| bar]]"));
            Assert.AreEqual("[[Category:foo| bar]]", Parsers.SimplifyLinks("[[Category:foo| bar ]]"));

            // nothing to do here
            Assert.AreEqual("[[dog|]]", Parsers.SimplifyLinks("[[dog|]]"));

            const string nochange = @"[[File:T and E.jpg|right|thumbnail|With [[EW|E]] in 2004]]";
            Assert.AreEqual(nochange, Parsers.SimplifyLinks(nochange));
            Assert.AreEqual(@"[[File:T and E.jpg|right|thumbnail|With [[dog]]s in 2004]]", Parsers.SimplifyLinks(@"[[File:T and E.jpg|right|thumbnail|With [[dog|dogs]] in 2004]]"), "nested link handling");
        }

        [Test]
        public void FixDatesHTML()
        {
            // replace <br> and <p> HTML tags tests
            Assert.AreEqual("\r\n\r\nsome text", parser.FixBrParagraphs("<p>some text"));
            Assert.AreEqual("\r\n\r\n[[some text|bar]]", parser.FixBrParagraphs("<p>[[some text|bar]]"));
            Assert.AreEqual("\r\n\r\nsome text\r\n\r\n", parser.FixBrParagraphs("<p>some text</p>"));
            Assert.AreEqual("\r\n\r\nsome text", parser.FixBrParagraphs("<p> some text"));
            Assert.AreEqual("\r\n\r\nsome text", parser.FixBrParagraphs("<br><br>some text"));
            Assert.AreEqual("some text\r\n\r\n", parser.FixBrParagraphs("some text<p>"));
            Assert.AreEqual("some text\r\n\r\n", parser.FixBrParagraphs("some text<br><br>"));
            Assert.AreEqual("some text\r\n\r\nword", parser.FixBrParagraphs("some text<br><br>word"));

            // don't match when in table or blockquote
            Assert.AreEqual("|<p>some text", parser.FixBrParagraphs("|<p>some text"));
            Assert.AreEqual("|<br><br>some text", parser.FixBrParagraphs("|<br><br>some text"));
            Assert.AreEqual("!<p>some text", parser.FixBrParagraphs("!<p>some text"));
            Assert.AreEqual("!<br><br>some text", parser.FixBrParagraphs("!<br><br>some text"));

            genFixes.AssertNotChanged(@"<blockquote><p>some text</blockquote>");
            genFixes.AssertNotChanged("<blockquote>|<br><br>some text</blockquote>");

            Assert.AreEqual(@"* {{Polish2|Krzepice (województwo dolnośląskie)|[[24 November]] [[2007]]}}

", parser.FixBrParagraphs(@"* {{Polish2|Krzepice (województwo dolnośląskie)|[[24 November]] [[2007]]}}<br><br>  "));

            const string nochange = @"{{cite web | title = Hello April 14 2009 there | date=2011-11-13 }}";

            Assert.AreEqual(nochange, parser.FixBrParagraphs(nochange));
        }

        [Test]
        public void FixDatesCommaErrors()
        {
            const string correct1 = @"Retrieved on April 14, 2009 was";
            Assert.AreEqual(correct1, parser.FixDatesA(@"Retrieved on April, 14, 2009 was"));
            Assert.AreEqual(correct1, parser.FixDatesA(@"Retrieved on April , 14 , 2009 was"));
            Assert.AreEqual(correct1, parser.FixDatesA(@"Retrieved on April , 14 ,2009 was"));

            Assert.AreEqual(correct1, parser.FixDatesA(@"Retrieved on April 14,2009 was"));
            Assert.AreEqual(correct1, parser.FixDatesA(@"Retrieved on April 14 ,2009 was"));
            Assert.AreEqual(correct1, parser.FixDatesA(@"Retrieved on April 14 2009 was"));
            Assert.AreEqual(correct1, parser.FixDatesA(@"Retrieved on April14,2009 was"));
            Assert.AreEqual(correct1, parser.FixDatesA(@"Retrieved on April14, 2009 was"));
            Assert.AreEqual(correct1, parser.FixDatesA(@"Retrieved on April14 2009 was"));
            Assert.AreEqual(correct1, parser.FixDatesA(correct1));

            // don't change image names
            string image1 = @"now foo [[Image:Foo July 24 2009.png]] was";
            Assert.AreEqual(image1, parser.FixDatesA(image1));
            string image2 = @"now foo [[File:Foo July 24 2009.png]] was";
            Assert.AreEqual(image2, parser.FixDatesA(image2));

            const string correct2 = @"Retrieved on 14 April 2009 was";
            Assert.AreEqual(correct2, parser.FixDatesA(@"Retrieved on 14 April, 2009 was"));
            Assert.AreEqual(correct2, parser.FixDatesA(@"Retrieved on 14 April , 2009 was"));
            Assert.AreEqual(correct2, parser.FixDatesA(@"Retrieved on 14 April,  2009 was"));

            const string nochange1 = @"On 14 April, 2590 people", nochange2 = @"Retrieved on April 142009 was";
            Assert.AreEqual(nochange1, parser.FixDatesA(nochange1));
            Assert.AreEqual(nochange2, parser.FixDatesA(nochange2));

            Assert.AreEqual(@"#####
'''A''' (1 December 1920 &ndash; 28 May 2013)", parser.FixDatesA(@"#####
'''A''' (1 December 1920 &ndash; 28 May, 2013)"));
        }
        

        [Test]
        public void FixDatesEnOnly()
        {
            #if DEBUG
            Variables.SetProjectLangCode("fr");
            Assert.AreEqual(@"Retrieved on April, 14, 2009 was", parser.FixDatesA(@"Retrieved on April, 14, 2009 was"));
            Assert.AreEqual(@"from (1900-1933) there", parser.FixDatesB(@"from (1900-1933) there", false, false));

            Variables.SetProjectLangCode("en");
            const string correct1 = @"Retrieved on April 14, 2009 was";
            Assert.AreEqual(correct1, parser.FixDatesA(@"Retrieved on April, 14, 2009 was"));
            const string correct = @"from (1900–1933) there";
            Assert.AreEqual(correct, parser.FixDatesB(@"from (1900-1933) there", false, false));
            #endif
        }

        [Test]
        public void FixDatesRanges()
        {
            const string correct1 = @"On 3–17 May 2009 a dog";
            Assert.AreEqual(correct1, parser.FixDatesA(@"On 3-17 May 2009 a dog"));
            Assert.AreEqual(correct1, parser.FixDatesA(@"On 3 - 17 May 2009 a dog"));
            Assert.AreEqual(correct1, parser.FixDatesA(@"On 3 -17 May 2009 a dog"));
            Assert.AreEqual(correct1, parser.FixDatesA(@"On 3 May-17 May 2009 a dog"));
            Assert.AreEqual(correct1, parser.FixDatesA(@"On 3 May - 17 May 2009 a dog"));
            Assert.AreEqual(correct1, parser.FixDatesA(@"On 3 May – 17 May 2009 a dog"));
            Assert.AreEqual(correct1, parser.FixDatesA(@"On 3 May – 17 May, 2009 a dog"));

            // American format
            const string correct2 = @"On May 3–17, 2009 a dog";
            Assert.AreEqual(correct2, parser.FixDatesA(@"On May 3-17, 2009 a dog"));
            Assert.AreEqual(correct2, parser.FixDatesA(@"On May 3-17 2009 a dog"));
            Assert.AreEqual(correct2, parser.FixDatesA(@"On May 3 - 17 2009 a dog"));
            Assert.AreEqual(correct2, parser.FixDatesA(@"On May 3   -17 2009 a dog"));
            Assert.AreEqual(correct2, parser.FixDatesA(@"On May 3 - May 17 2009 a dog"));
            Assert.AreEqual(correct2, parser.FixDatesA(@"On May 3 - May 17, 2009 a dog"));
            Assert.AreEqual(correct2, parser.FixDatesA(@"On May 3 – May 17 2009 a dog"));
            Assert.AreEqual(correct2, parser.FixDatesA(@"On May 3 – May 17, 2009 a dog"));

            // no change
            const string nochange1 = @"May 17 - 13,009 dogs";
            Assert.AreEqual(nochange1, parser.FixDatesA(nochange1));

            const string nochange2 = @"May 2-4-0";
            Assert.AreEqual(nochange2, parser.FixDatesA(nochange2));

            // month ranges
            const string correct3 = @"May–June 2010";
            Assert.AreEqual(correct3, parser.FixDatesA(@"May-June 2010"), "endash set for month range");
            Assert.AreEqual(correct3, parser.FixDatesA(correct3));

            Assert.AreEqual("from 1904 – 11 May 1956 there", parser.FixDatesA("from 1904 – 11 May 1956 there"));

            // DateRangeToYear: result is spaced endash
            const string DateToYear = @"'''Nowell''' (May 16, 1872 - 1940), was";
            genFixes.AssertChange(DateToYear, DateToYear.Replace("-", "–"));
            genFixes.AssertChange(DateToYear.Replace(" - ", "-"), DateToYear.Replace("-", "–"));
            const string DateToYear2 = @"'''Nowell''' (16 May 1872–1940), was";
            genFixes.AssertChange(DateToYear2, DateToYear2.Replace("–", " – "));
            genFixes.AssertChange(DateToYear2.Replace("–", "-"), DateToYear2.Replace("–", " – "));

            genFixes.AssertNotChanged(@"Volume 1, 2001–2004 was");
        }

        [Test]
        public void TestFullYearRanges()
        {
            const string correct = @"from (1900–1933) there";
            Assert.AreEqual(correct, parser.FixDatesB(@"from (1900-1933) there", false, false));
            Assert.AreEqual(correct, parser.FixDatesB(@"from (1900  –  1933) there", false, false));
            Assert.AreEqual(correct, parser.FixDatesB(@"from (1900 -1933) there", false, false));
            Assert.AreEqual(correct, parser.FixDatesB(@"from (1900 - 1933) there", false, false));
            Assert.AreEqual(correct, parser.FixDatesB(@"from (1900 -  1933) there", false, false));
            Assert.AreEqual(@"from (1900–1901) there", parser.FixDatesB(@"from (1900 - 1901) there", false, false));
            Assert.AreEqual(@"from (1900–1933, 2000) there", parser.FixDatesB(@"from (1900-1933, 2000) there", false, false));
            Assert.AreEqual(@"from (1900–1933, 2000–2002) there", parser.FixDatesB(@"from (1900-1933, 2000-2002) there", false, false));
            Assert.AreEqual(@"from ( 1900–1933) there", parser.FixDatesB(@"from ( 1900-1933) there", false, false));

            Assert.AreEqual(@"from 1950–1960,", parser.FixDatesB(@"from 1950-1960,", false, false));
            Assert.AreEqual(@"|1950–1960|", parser.FixDatesB(@"|1950-1960|", false, false));
            Assert.AreEqual(@"(1950–1960 and 1963–1968)", parser.FixDatesB(@"(1950-1960 and 1963-1968)", false, false));
            Assert.AreEqual(@"or 1900–1901,", parser.FixDatesB(@"or 1900 - 1901,", false, false));
            Assert.AreEqual(@"for 1900–1901,", parser.FixDatesB(@"for 1900 - 1901,", false, false));

            // no change – not valid date range
            genFixes.AssertNotChanged(@"from (1900–1870) there");

            // already okay
            genFixes.AssertNotChanged(@"from (1900&ndash;1933) there");
            genFixes.AssertNotChanged(@"from (1900–1933) there");

            Assert.AreEqual(@"now between 1900–1920
was", parser.FixDatesB(@"now between 1900-1920
was", false, false));

            string BadRange = @"from 1950-1920,";
            Assert.AreEqual(BadRange, parser.FixDatesB(BadRange, false, false));
            BadRange = @"from 1950 – 1920,";
            Assert.AreEqual(BadRange, parser.FixDatesB(BadRange, false, false));
            BadRange = @"from 1980-70,";
            Assert.AreEqual(BadRange, parser.FixDatesB(BadRange, false, false));

            // full date ranges
            Assert.AreEqual(@"over 1 April 2004 – 5 July 2009.", parser.FixDatesB(@"over 1 April 2004-5 July 2009.", false, false));
            Assert.AreEqual(@"over April 1, 2004 – July 5, 2009.", parser.FixDatesB(@"over April 1, 2004–July 5, 2009.", false, false));
        }

        [Test]
        public void CircaYearRanges()
        {
            Assert.AreEqual(@"c. 1950 – 1960,", parser.FixDatesB(@"c. 1950 - 1960,", false, false));
            Assert.AreEqual(@"ca. 1950 – 1960,", parser.FixDatesB(@"ca. 1950 - 1960,", false, false));
            Assert.AreEqual(@"circa 1950 – 1960,", parser.FixDatesB(@"circa 1950 - 1960,", false, false));

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
            Assert.AreEqual(present, parser.FixDatesB(@"from 2002-present was", false, false));
            Assert.AreEqual(present, parser.FixDatesB(@"from 2002 -   present was", false, false));
            Assert.AreEqual(present, parser.FixDatesB(@"from 2002–present was", false, false));

            Assert.AreEqual(@"from 2002–Present was", parser.FixDatesB(@"from 2002-Present was", false, false));

            const string present2 = @"== Members ==
* [[Nick Hexum]] - Vocals, [[Rhythm Guitar]], Programming (1989 - present)
* [[S. A. Martinez|Doug Martinez]] - Vocals, [[Phonograph|Turntables]], DJ (1992 - present)
* [[Tim Mahoney (guitarist)|Tim Mahoney]] - [[Lead Guitar]] (1991 - present)
* [[P-Nut|Aaron Wills]] - [[Bass guitar]] (1989 - present)
* [[Chad Sexton]] - [[Drum]]s, Programming, Percussion (1989 - present)";

            Assert.AreEqual(present2.Replace(@" - p", @"–p"), parser.FixDatesB(present2, false, false));

            genFixes.AssertNotChanged(@"* 2000 - presented");
        }

        [Test]
        public void TestDateToPresentRanges()
        {
            Assert.AreEqual(@"from May 2002 – present was", parser.FixDatesB(@"from May 2002 - present was", false, false));
            Assert.AreEqual(@"from May 2002 – present was", parser.FixDatesB(@"from May 2002-present was", false, false));
            Assert.AreEqual(@"from May 11, 2002 – present was", parser.FixDatesB(@"from May 11, 2002-present was", false, false));
            Assert.AreEqual(@"from May 11, 2002 – present was", parser.FixDatesB(@"from May 11, 2002 - present was", false, false));
        }

        [Test]
        public void TestShortenedYearRanges()
        {
            const string correct = @"from (1900–33) there";
            Assert.AreEqual(correct, parser.FixDatesB(@"from (1900-33) there", false, false));
            Assert.AreEqual(correct, parser.FixDatesB(@"from (1900 -33) there", false, false));
            Assert.AreEqual(correct, parser.FixDatesB(@"from (1900 - 33) there", false, false));
            Assert.AreEqual(correct, parser.FixDatesB(@"from (1900 -  33) there", false, false));
            Assert.AreEqual(@"from (1900–1901) there", parser.FixDatesB(@"from (1900 - 1901) there", false, false));
            Assert.AreEqual(@"from (1900–33, 2000) there", parser.FixDatesB(@"from (1900-33, 2000) there", false, false));
            Assert.AreEqual(@"from (1900–33, 2000–2002) there", parser.FixDatesB(@"from (1900-33, 2000-2002) there", false, false));
            Assert.AreEqual(@"from ( 1900–33) there", parser.FixDatesB(@"from ( 1900-33) there", false, false));

            Assert.AreEqual(@"from 1950–60,", parser.FixDatesB(@"from 1950-60,", false, false));
            Assert.AreEqual(@"(1950–60 and 1963–68)", parser.FixDatesB(@"(1950-60 and 1963-68)", false, false));

            // no change – not valid date range
            genFixes.AssertNotChanged(@"from (1920–18) there");

            // already okay
            genFixes.AssertNotChanged(@"from (1900&ndash;33) there");
            genFixes.AssertNotChanged(@"from (1900–33) there");
        }


        [Test]
        public void FixLivingThingsRelatedDates()
        {
            Assert.AreEqual("test text", Parsers.FixLivingThingsRelatedDates("test text"));
            Assert.AreEqual("'''John Doe''' (born [[21 February]] [[2008]])", Parsers.FixLivingThingsRelatedDates("'''John Doe''' (b. [[21 February]] [[2008]])"), "b. expanded");
            Assert.AreEqual(@"'''John O'Doe''' (born [[21 February]] [[2008]])", Parsers.FixLivingThingsRelatedDates(@"'''John O'Doe''' (b. [[21 February]] [[2008]])"), "b. expanded, name has apostrophe");
            Assert.AreEqual("'''John Doe''' (born 21 February 2008)", Parsers.FixLivingThingsRelatedDates("'''John Doe''' (b. 21 February 2008)"), "non-wikilinked dates supported");
            Assert.AreEqual("'''John Doe''' (born [[21 February]] [[2008]])", Parsers.FixLivingThingsRelatedDates("'''John Doe''' ([[21 February]] [[2008]]–)"), "dash for born expanded");
            Assert.AreEqual("'''John O'Doe''' (born [[21 February]] [[2008]])", Parsers.FixLivingThingsRelatedDates("'''John O'Doe''' ([[21 February]] [[2008]]–)"), "dash for born expanded, name has apostrophe");
            Assert.AreEqual("'''John Doe''' (born [[21 February]] [[2008]])", Parsers.FixLivingThingsRelatedDates("'''John Doe''' ([[21 February]] [[2008]] &ndash;)"), "dash for born expanded");
            Assert.AreEqual("'''John Doe''' (born [[21 February]] [[2008]])", Parsers.FixLivingThingsRelatedDates("'''John Doe''' (born: [[21 February]] [[2008]])"), "born: tidied");
            Assert.AreEqual("'''John Doe''' (born [[21 February]] [[2008]])", Parsers.FixLivingThingsRelatedDates("'''John Doe''' (Born: [[21 February]] [[2008]])"), "born: tidied");
            Assert.AreEqual("'''John Doe''' (born March 6, 2008)", Parsers.FixLivingThingsRelatedDates("'''John Doe''' (b. March 6, 2008)"), "b. expanded");
            Assert.AreEqual("'''John Doe''' (born 2008)", Parsers.FixLivingThingsRelatedDates("'''John Doe''' (b. 2008)"), "b. expanded");
            Assert.AreEqual("'''John Doe''' (born 2008)", Parsers.FixLivingThingsRelatedDates("'''John Doe''' (b.2008)"), "b. expanded");
            Assert.AreEqual("'''John Doe''' (born 2008)", Parsers.FixLivingThingsRelatedDates("'''John Doe''' (born on 2008)"), "born on fixed");
            Assert.AreEqual("'''John Doe''' (born March 6, 2008, London)", Parsers.FixLivingThingsRelatedDates("'''John Doe''' (b. March 6, 2008, London)"), "b. expanded");
            Assert.AreEqual("'''John Doe''' (born March 6, 2008 in London)", Parsers.FixLivingThingsRelatedDates("'''John Doe''' (b. March 6, 2008 in London)"), "b. expanded");
            Assert.AreEqual("'''John Doe''' (born March 6, 2008, London)", Parsers.FixLivingThingsRelatedDates("'''John Doe''' (born on March 6, 2008, London)"), "born on fixed");

            Assert.AreEqual("'''John Doe''' (died [[21 February]] [[2008]])", Parsers.FixLivingThingsRelatedDates("'''John Doe''' (d. [[21 February]] [[2008]])"));
            Assert.AreEqual("'''John Doe''' (died [[21 February]] [[2008]])", Parsers.FixLivingThingsRelatedDates("'''John Doe''' (d.[[21 February]] [[2008]])"));
            Assert.AreEqual("'''John Doe''' (died 2008)", Parsers.FixLivingThingsRelatedDates("'''John Doe''' (d.2008)"));
            Assert.AreEqual("'''John O'Doe''' (died [[21 February]] [[2008]])", Parsers.FixLivingThingsRelatedDates("'''John O'Doe''' (d. [[21 February]] [[2008]])"));
            Assert.AreEqual("'''Willa Klug Baum''' ([[October 4]], [[1926]] – May 18, 2006)", Parsers.FixLivingThingsRelatedDates("'''Willa Klug Baum''' (born [[October 4]], [[1926]], died May 18, 2006)"));
            Assert.AreEqual("'''Willa Klug Baum''' (1926 – May 18, 2006)", Parsers.FixLivingThingsRelatedDates("'''Willa Klug Baum''' (b.1926, died May 18, 2006)"));

            const string Nochange = @"{{Infobox baseball team|
  FormerNames= '''[[Nishi-Nippon Railroad|Nishitetsu]] Clippers''' (1950)<br>'''Nishitetsu Lions''' (1951-1972)<br>'''Taiheiyo Club Lions''' (1973–1976)<br>'''Crown Lighter Lions''' (1977–1978)<br>'''Seibu Lions''' (1979–2007)<br>'''Saitama Seibu Lions''' (2008–)|
  foo=bar }}";

            Assert.AreEqual(Nochange, Parsers.FixLivingThingsRelatedDates(Nochange));

            const string Nochange2 = @"*'''[[Luís Godinho Lopes|Luís Filipe Fernandes David Godinho Lopes]]''' (2011–)";

            Assert.AreEqual(Nochange2, Parsers.FixLivingThingsRelatedDates(Nochange2));

            const string Nochange3 = @"** '''1st''' Andy Brown '''52''' (2008-)";

            Assert.AreEqual(Nochange3, Parsers.FixLivingThingsRelatedDates(Nochange3));

            const string Nochange4 = @"# '''[[Luís Godinho Lopes|Luís Filipe Fernandes David Godinho Lopes]]''' (2011–)";

            Assert.AreEqual(Nochange4, Parsers.FixLivingThingsRelatedDates(Nochange4));
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
            Assert.AreEqual(a0, Parsers.FixPeopleCategories(a0, "foo"));

            const string bug1 = @"{{BLP unsourced|date=March 2010}}

'''Z''' (born 2 January{{Year needed|date=January 2010|reason=Fix this date immediately or remove it; it look}} in .";

            Assert.AreEqual(bug1, Parsers.FixPeopleCategories(bug1, "Foo"));
        }

        [Test]
        public void GetInfoBoxFieldValue()
        {
            List<string> Year = new List<string>(new[] { "year" });

            Assert.AreEqual(@"1990", Parsers.GetInfoBoxFieldValue(@"hello {{infobox foo
|year=1990
|other=great}} now", Year));

            Assert.AreEqual(@"1990", Parsers.GetInfoBoxFieldValue(@"hello {{infobox foo
|  year  =  1990
|other=great}} now", Year));

            Assert.AreEqual(@"1990", Parsers.GetInfoBoxFieldValue(@"hello {{infobox foo
|  Year  =  1990
|other=great}} now", Year));

            // no infobox
            Assert.AreEqual(@"", Parsers.GetInfoBoxFieldValue(@"hello now", Year));

            // field not found
            Assert.AreEqual(@"", Parsers.GetInfoBoxFieldValue(@"hello {{infobox foo
|  Year  =  1990
|other=great}} now", new List<string>(new[] { "yearly" })));

            // multiple fields on same line
            Assert.AreEqual(@"1990", Parsers.GetInfoBoxFieldValue(@"hello {{infobox foo
|  Year  =  1990  |some=where
|other=great}} now", Year));
        }

        [Test]
        public void RemoveDuplicateWikiLinks()
        {
            // removes duplicate piped wikilinks on same line
            Assert.AreEqual(@"now [[foo|bar]] was bar too", Parsers.RemoveDuplicateWikiLinks(@"now [[foo|bar]] was [[foo|bar]] too"));
            Assert.AreEqual(@"now [[foo|bar]] was bars too", Parsers.RemoveDuplicateWikiLinks(@"now [[foo|bar]] was [[foo|bar]]s too"));

            // multiline – no change
            Assert.AreEqual(@"now [[foo|bar]]
was [[foo|bar]] too", Parsers.RemoveDuplicateWikiLinks(@"now [[foo|bar]]
was [[foo|bar]] too"));

            // case sensitive
            Assert.AreEqual(@"now [[foo|bar]] was [[Foo|bar]] too", Parsers.RemoveDuplicateWikiLinks(@"now [[foo|bar]] was [[Foo|bar]] too"));
            Assert.AreEqual(@"now [[foo bar]] was [[Foo bar]] too", Parsers.RemoveDuplicateWikiLinks(@"now [[foo bar]] was [[Foo bar]] too"));

            // removes duplicate unpiped wikilinks
            Assert.AreEqual(@"now [[foo bar]] was foo bar too", Parsers.RemoveDuplicateWikiLinks(@"now [[foo bar]] was [[foo bar]] too"));

            // no changes
            Assert.AreEqual(@"now [[foo|bar]] was [[foo]] too", Parsers.RemoveDuplicateWikiLinks(@"now [[foo|bar]] was [[foo]] too"));
        }

        [Test]
        public void CanonicalizeTitleRawTests()
        {
            Assert.AreEqual(@"foo bar", Parsers.CanonicalizeTitleRaw(@"foo_bar"));
            Assert.AreEqual(@"foo+bar", Parsers.CanonicalizeTitleRaw(@"foo+bar"));
            Assert.AreEqual(@"foo bar", Parsers.CanonicalizeTitleRaw(@"foo_bar", false));
            Assert.AreEqual(@"foo bar ", Parsers.CanonicalizeTitleRaw(@"foo_bar ", false));
            Assert.AreEqual(@"foo bar", Parsers.CanonicalizeTitleRaw(@"foo_bar", true));
            Assert.AreEqual(@"foo bar", Parsers.CanonicalizeTitleRaw(@"foo_bar ", true));
            Assert.AreEqual(@"foo bar", Parsers.CanonicalizeTitleRaw(@" foo_bar", true));

            Assert.AreEqual(@"Bugs#If a selflink is also bolded, AWB should", Parsers.CanonicalizeTitleRaw(@"Bugs#If_a_selflink_is_also_bolded%2C_AWB_should"));
        }

        [Test]
        public void RemoveTemplateNamespace()
        {
            Assert.AreEqual(@"{{foo}}", Parsers.RemoveTemplateNamespace(@"{{Template:foo}}"));
            Assert.AreEqual(@"{{Foo}}", Parsers.RemoveTemplateNamespace(@"{{Template:Foo}}"));
            Assert.AreEqual(@"{{foo bar}}", Parsers.RemoveTemplateNamespace(@"{{Template:foo_bar}}"));
            Assert.AreEqual(@"Template:Foo", Parsers.RemoveTemplateNamespace(@"Template:Foo"), "no change if it is not a real template");
            Assert.AreEqual(@"[[Template:Foo]]", Parsers.RemoveTemplateNamespace(@"[[Template:Foo]]"), "no change if it is not a real template");
            Assert.AreEqual(@"{{Clarify|date=May 2014|reason=Use Template:Cite web or similar}}", Parsers.RemoveTemplateNamespace(@"{{Clarify|date=May 2014|reason=Use Template:Cite web or similar}}"), "no change if it is part of a comment");
            Assert.AreEqual(@"{{Foo|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}", Parsers.RemoveTemplateNamespace(@"{{Template:Foo|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"));
        }

        [Test]
        public void FixLinkWhitespaceSpacing()
        {
            Assert.AreEqual("[[Foo]]", Parsers.FixLinkWhitespace("[[Foo]]", "Test"));
            Assert.AreEqual("[[Foo Bar]]", Parsers.FixLinkWhitespace("[[Foo Bar]]", "Test"));
            Assert.AreEqual("[[Foo Bar]]", Parsers.FixLinkWhitespace("[[Foo  Bar]]", "Test"));
            Assert.AreEqual("[[Foo Bar was]]", Parsers.FixLinkWhitespace("[[Foo  Bar  was]]", "Test"), "fixes multiple double spaces in single link");
            Assert.AreEqual("[[Foo Bar|Bar]]", Parsers.FixLinkWhitespace("[[Foo  Bar|Bar]]", "Test"));
        }

        [Test]
        public void FixLink()
        {
            bool nochange;
            Assert.AreEqual(@"[[Foo bar]]", Parsers.FixLinks(@"[[Foo_bar]]", "a", out nochange));
            Assert.IsFalse(nochange);

            const string doubleApos = @"[[Image:foo%27%27s.jpg|thumb|200px|Bar]]";
            Assert.AreEqual(doubleApos, Parsers.FixLinks(doubleApos, "a", out nochange));

            Variables.AddUnderscoredTitles(new List<string>(new [] {"Size t", "Mod perl", "Mod mono" } ));

            Assert.AreEqual(@"[[size_t]]", Parsers.FixLinks(@"[[size_t]]", "a", out nochange));
            Assert.IsTrue(nochange);
            Assert.AreEqual(@"[[mod_perl]]", Parsers.FixLinks(@"[[mod_perl]]", "a", out nochange));
            Assert.IsTrue(nochange);
            Assert.AreEqual(@"[[mod_mono]]", Parsers.FixLinks(@"[[mod_mono]]", "a", out nochange));
            Assert.IsTrue(nochange);
            Assert.AreEqual(@"[[Mod_mono]]", Parsers.FixLinks(@"[[Mod_mono]]", "a", out nochange));
            Assert.IsTrue(nochange);
            Assert.AreEqual(@"[[E|Mod_mono]]", Parsers.FixLinks(@"[[E|Mod_mono]]", "a", out nochange));
            Assert.IsTrue(nochange);
            Assert.AreEqual(@"[[Mod_mono#link]]", Parsers.FixLinks(@"[[Mod_mono#link]]", "a", out nochange));
            Assert.IsTrue(nochange);

            Assert.AreEqual(@"[[List of The Amazing Spider-Man issues#The Amazing Spider-Man #648–current x|List of issues]]", Parsers.FixLinks(@"[[List of The Amazing Spider-Man issues#The Amazing Spider-Man #648–current x|List of issues]]", "a", out nochange), "Does not break section links with hash and space");
            Assert.AreEqual(@"[[Foo#nice example|F]]", Parsers.FixLinks(@"[[Foo#nice%20example|F]]", "a", out nochange), "%20 replaced in target");
            Assert.AreEqual(@"[[Foo|bar]]", Parsers.FixLinks(@"[[Foo|bar|]]", "a", out nochange), "Fixes excess trailing pipe");
            Assert.AreEqual(@"[[Foo|bar]]", Parsers.FixLinks(@"[[Foo|bar | ]]", "a", out nochange), "Fixes excess trailing pipe");

            Assert.AreEqual(@"[[Foo bar]]", Parsers.FixLinks(@"[[Foo_bar]]", "a", out nochange), "Fixes underscore");
            Assert.AreEqual(@"[[Foo bar#ab c]]", Parsers.FixLinks(@"[[Foo_bar#ab_c]]", "a", out nochange), "Fixes underscore");

            Assert.AreEqual(@"[[foo|bar]]", Parsers.FixLinks(@"[[foo{{!}}bar]]", "a", out nochange),"Fixes pipe");
            Assert.AreEqual(@"[[foo|]]", Parsers.FixLinks(@"[[foo{{!}}]]", "a", out nochange),"Fixes pipe");
            Assert.AreEqual("[[Repeat sign|{{!}}: ]]", Parsers.FixLinks(@"[[Repeat sign|{{!}}: ]]", "a", out nochange),"Do nothing in the only exception");
        }

        [Test]
        public void FixLinkBoldItalic()
        {
            bool nochange;
            Assert.AreEqual(@"''[[foo|foo]]''", Parsers.FixLinks(@"[[foo|''foo'']]", "a", out nochange));
            Assert.IsFalse(nochange);
            Assert.AreEqual(@"'''[[foo|Foo]]'''", Parsers.FixLinks(@"[[foo|'''Foo''']]", "a", out nochange));
            Assert.IsFalse(nochange);

            // No change to single apostrophes
            Assert.AreEqual(@"[[foo|'bar']]", Parsers.FixLinks(@"[[foo|'bar']]", "a", out nochange));
            Assert.IsTrue(nochange);

            // No change if (dodgy) apostrope just after link
            Assert.AreEqual(@"[[foo|''bar'']]'", Parsers.FixLinks(@"[[foo|''bar'']]'", "a", out nochange));
            Assert.IsTrue(nochange);

            Assert.AreEqual(@"[[foo|]]", Parsers.FixLinks(@"[[foo|]]", "a", out nochange));
            Assert.IsTrue(nochange);
            
             Assert.AreEqual(@"[[foo|''b'']]", Parsers.FixLinks(@"[[foo|''b'']]", "a", out nochange));
            Assert.IsTrue(nochange);

            // No change to part of link text in bold/italics
            Assert.AreEqual(@"[[foo|A ''bar'']]", Parsers.FixLinks(@"[[foo|A ''bar'']]", "a", out nochange));
            Assert.IsTrue(nochange);
            Assert.AreEqual(@"[[foo|A '''bar''']]", Parsers.FixLinks(@"[[foo|A '''bar''']]", "a", out nochange));
            Assert.IsTrue(nochange);
        }

        [Test]
        public void FixSelfInterwikis()
        {
            bool nochange;

            Assert.AreEqual(@"[[Foo]]", Parsers.FixLinks(@"[[en:Foo]]", "Bar", out nochange));
            Assert.AreEqual(@"[[Foo|Bar]]", Parsers.FixLinks(@"[[en:Foo|Bar]]", "T1", out nochange));

            const string FrIW = @"Now [[fr:Here]]";
            Assert.AreEqual(FrIW, Parsers.FixLinks(FrIW, "Bar", out nochange));
            Assert.IsTrue(parser.SortInterwikis);
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
";
            Assert.IsFalse(Parsers.HasSeeAlsoAfterNotesReferencesOrExternalLinks(""));
            Assert.IsFalse(Parsers.HasSeeAlsoAfterNotesReferencesOrExternalLinks(start + seeAlso));
            Assert.IsFalse(Parsers.HasSeeAlsoAfterNotesReferencesOrExternalLinks(start + seeAlso + extlinks));

            Assert.IsTrue(Parsers.HasSeeAlsoAfterNotesReferencesOrExternalLinks(start + extlinks + seeAlso));
            Assert.IsTrue(Parsers.HasSeeAlsoAfterNotesReferencesOrExternalLinks(start + notes + seeAlso));
            Assert.IsTrue(Parsers.HasSeeAlsoAfterNotesReferencesOrExternalLinks(start + references + seeAlso));
            Assert.IsTrue(Parsers.HasSeeAlsoAfterNotesReferencesOrExternalLinks(start + references + seeAlso + notes));
        }

        [Test]
        public void UnclosedTags()
        {
            Dictionary<int, int> uct = new Dictionary<int, int>();

            uct = Parsers.UnclosedTags(@"<pre>bar</pre>");
            Assert.AreEqual(uct.Count, 0);

            uct = Parsers.UnclosedTags(@"<gallery>File:bar</gallery>");
            Assert.AreEqual(uct.Count, 0);

            uct = Parsers.UnclosedTags(@"<math>bar</math>");
            Assert.AreEqual(uct.Count, 0);

            uct = Parsers.UnclosedTags(@"<source>bar</source> <ref name=Foo>boo</ref>");
            Assert.AreEqual(uct.Count, 0);

            uct = Parsers.UnclosedTags(@"<pre>bar</pre> <ref name=Foo>boo</ref>");
            Assert.AreEqual(uct.Count, 0);

            uct = Parsers.UnclosedTags(@"<pre>bar</pre> <math> not ended");
            Assert.AreEqual(uct.Count, 1, "math");
            Assert.IsTrue(uct.ContainsKey(15));
            Assert.IsTrue(uct.ContainsValue(6));

            uct = Parsers.UnclosedTags(@"<pre>bar</pre> <center> not ended");
            Assert.AreEqual(uct.Count, 1, "center");
            Assert.IsTrue(uct.ContainsKey(15));
            Assert.IsTrue(uct.ContainsValue(8));

            uct = Parsers.UnclosedTags(@"<pre>bar</pre> <center> <center>a</center> not ended");
            Assert.AreEqual(uct.Count, 1, "center double");
            Assert.IsTrue(uct.ContainsKey(15),"center key");
            Assert.IsTrue(uct.ContainsValue(8),"center value");

            uct = Parsers.UnclosedTags(@"<pre>bar</pre> <sup> <sup>a</sup> not ended");
            Assert.AreEqual(uct.Count, 1);
            Assert.IsTrue(uct.ContainsKey(15),"sup key");
            Assert.IsTrue(uct.ContainsValue(5),"sup value");

            uct = Parsers.UnclosedTags(@"<pre>bar</pre> <sub> <sub>a</sub> not ended");
            Assert.AreEqual(uct.Count, 1, "sub");
            Assert.IsTrue(uct.ContainsKey(15),"sub key");
            Assert.IsTrue(uct.ContainsValue(5),"sub value");

            uct = Parsers.UnclosedTags(@"<pre>bar</pre> <ref name=<ref name=Foo/> not ended");
            Assert.AreEqual(uct.Count, 1, "ref");
            Assert.IsTrue(uct.ContainsKey(15));
            Assert.IsTrue(uct.ContainsValue(35));

            uct = Parsers.UnclosedTags(@"<pre>bar</pre> <ref> not ended");
            Assert.AreEqual(uct.Count, 1, "ref 2");
            Assert.IsTrue(uct.ContainsKey(15));
            Assert.IsTrue(uct.ContainsValue(5));

            uct = Parsers.UnclosedTags(@"<pre>bar</pre> <ref name='foo'> not ended");
            Assert.AreEqual(uct.Count, 1, "ref name");
            Assert.IsTrue(uct.ContainsKey(15));
            Assert.IsTrue(uct.ContainsValue(16));

            uct = Parsers.UnclosedTags(@"<pre>bar</pre> <source lang=""bar""> not ended");
            Assert.AreEqual(uct.Count, 1, "source");
            Assert.IsTrue(uct.ContainsKey(15));

            uct = Parsers.UnclosedTags(@"<pre>bar</pre> <small> not ended");
            Assert.AreEqual(uct.Count, 1, "small");
            Assert.IsTrue(uct.ContainsKey(15));

            uct = Parsers.UnclosedTags(@"<pre>bar</pre> < code> not ended");
            Assert.AreEqual(uct.Count, 1, "code");
            Assert.IsTrue(uct.ContainsKey(15));

            uct = Parsers.UnclosedTags(@"<pre>bar</pre> < nowiki > not ended");
            Assert.AreEqual(uct.Count, 1, "nowiki");
            Assert.IsTrue(uct.ContainsKey(15));

            uct = Parsers.UnclosedTags(@"<pre>bar</pre> <pre> not ended");
            Assert.AreEqual(uct.Count, 1, "pre");
            Assert.IsTrue(uct.ContainsKey(15));

            uct = Parsers.UnclosedTags(@"<pre>bar</pre> </pre> not opened");
            Assert.AreEqual(uct.Count, 1, "/pre");
            Assert.IsTrue(uct.ContainsKey(15));

            uct = Parsers.UnclosedTags(@"<pre>bar</pre> <gallery> not ended");
            Assert.AreEqual(uct.Count, 1, "gallery");
            Assert.IsTrue(uct.ContainsKey(15));

            uct = Parsers.UnclosedTags(@"<pre>bar</pre> </gallery> not opened");
            Assert.AreEqual(uct.Count, 1, "/gallery");
            Assert.IsTrue(uct.ContainsKey(15));

            uct = Parsers.UnclosedTags(@"<pre>bar</pre> <!-- not ended");
            Assert.AreEqual(uct.Count, 1, "<!--");
            Assert.IsTrue(uct.ContainsKey(15));

            uct = Parsers.UnclosedTags(@"<!--bar--> <!-- not ended");
            Assert.AreEqual(uct.Count, 1, "<!-- 2");
            Assert.IsTrue(uct.ContainsKey(11));

            uct = Parsers.UnclosedTags(@"<gallery> not ended <gallery>bar</gallery>");
            Assert.AreEqual(uct.Count, 1, "gallery 2");
            Assert.IsTrue(uct.ContainsKey(20));

            uct = Parsers.UnclosedTags(@"<gallery other='a'> not ended <gallery other='a'>bar</gallery>");
            Assert.AreEqual(uct.Count, 1, "gallery param");
            Assert.IsTrue(uct.ContainsKey(30));

            uct = Parsers.UnclosedTags(@"<gallery>A|<div><small>(1717)</small><br/><small><small>Munich</small></div></gallery>");
            Assert.AreEqual(uct.Count, 1, "small div");
            Assert.IsTrue(uct.ContainsKey(42));

            uct = Parsers.UnclosedTags(@"<small><small><small><small><small><small><small><small><small><small><small><small>");
            Assert.AreEqual(uct.Count, 12, "multiple unclosed small");
            uct = Parsers.UnclosedTags(@"<small><small><small><small><small><small><small><small><small><small><small><small> <!-- <math> -->");
            Assert.AreEqual(uct.Count, 12, "multiple unclosed small, ignore comments");
            uct = Parsers.UnclosedTags(@"<small><small><small><small><small><small><small><small><small><small><small><small> <small>></small>");
            Assert.AreEqual(uct.Count, 14, "multiple unclosed small, but counts tag containing >");
            uct = Parsers.UnclosedTags(@"<small><small><small><small><small><small><small><small><small><small><small><small> <ref name=fo>a</ref> <gallery param=a>b</gallery>");
            Assert.AreEqual(uct.Count, 12, "multiple unclosed small, don't count ref/gallery with params");
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

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_2#Incorrect_bulleting
            StringAssert.Contains("\r\nhttp://example.com }}", s);
            
            string NoChange = @"==External links==
* http://example.com/foo";
            Assert.AreEqual(NoChange, Parsers.BulletExternalLinks(NoChange));
        }

        [Test]
        public void TestFixLinkWhitespace()
        {
            Assert.AreEqual("b [[a]] c", Parsers.FixLinkWhitespace("b[[ a ]]c", "foo")); // regexes 1 & 2
            Assert.AreEqual("b   [[a]]  c", Parsers.FixLinkWhitespace("b   [[ a ]]  c", "foo")); // 4 & 5

            Assert.AreEqual("[[a]] b", Parsers.FixLinkWhitespace("[[a ]]b", "foo"));
            Assert.AreEqual(@"Now
[[a]] b", Parsers.FixLinkWhitespace(@"Now
[[a ]]b", "foo"), "wikilink on start of line");
            Assert.AreEqual("[[a]]" + "\r\n", Parsers.FixLinkWhitespace("[[a ]]" + "\r\n", "foo"));

            Assert.AreEqual("[[foo bar]]", Parsers.FixLinkWhitespace("[[foo  bar]]", "foot"));
            Assert.AreEqual("[[foo bar]]", Parsers.FixLinkWhitespace("[[foo  bar]]", ""));
            Assert.AreEqual("[[foo bar]]", Parsers.FixLinkWhitespace("[[foo     bar]]", "foot"));
            Assert.AreEqual("dat is [[foo bar]] show!", Parsers.FixLinkWhitespace("dat is [[ foo   bar ]] show!", "foot"));
            Assert.AreEqual("dat is [[foo bar]] show!", Parsers.FixLinkWhitespace("dat is[[ foo   bar ]]show!", "foot"));

            Assert.AreEqual(@"His [[Tiger Woods#Career]] was", Parsers.FixLinkWhitespace(@"His [[Tiger Woods# Career]] was", "Tiger Woods"));

            // don't fix when bit before # is not article name
            Assert.AreNotSame(@"Fred's [[Smith#Career]] was", Parsers.FixLinkWhitespace(@"Fred's [[Smith# Career]] was", "Fred"));

            Assert.AreEqual(@"[[Category:London| ]]", Parsers.FixLinkWhitespace(@"[[Category:London| ]]", "foo")); // leading space NOT removed from cat sortkey
            Assert.AreEqual(@"[[Category:Slam poetry| ]] ", Parsers.FixLinkWhitespace(@"[[Category:Slam poetry| ]] ", "foo")); // leading space NOT removed from cat sortkey

            // shouldn't fix - not enough information
            //Assert.AreEqual("[[ a ]]", Parsers.FixLinkWhitespace("[[ a ]]", "foo"));
            //disabled for the time being to avoid unnecesary clutter

            Assert.AreEqual("[[foo#bar]]", Parsers.FixLinkWhitespace("[[foo #bar]]", "foot"));
            Assert.AreEqual("[[foo#bar]]", Parsers.FixLinkWhitespace("[[foo # bar]]", "foot"));
            Assert.AreEqual("[[foo#bar]]", Parsers.FixLinkWhitespace("[[foo# bar]]", "foot"));
            Assert.AreEqual("[[foo#bar]]te", Parsers.FixLinkWhitespace("[[foo #bar]]te", "foot"));
            Assert.AreEqual("[[foo#bar|te]]", Parsers.FixLinkWhitespace("[[foo  #bar|te]]", "foot"));

            Assert.AreEqual(@"[[List of The Amazing Spider-Man issues#The Amazing Spider-Man #648–current x|List of issues]]", Parsers.FixLinkWhitespace(@"[[List of The Amazing Spider-Man issues#The Amazing Spider-Man #648–current x|List of issues]]", "x"), "Does not break section links with hash and space");

            Assert.AreEqual("[[A# code]]", Parsers.FixLinkWhitespace("[[A# code]]", "Test"));
            Assert.AreEqual("[[C# code]]", Parsers.FixLinkWhitespace("[[C# code]]", "Test"));
            Assert.AreEqual("[[F# code]]", Parsers.FixLinkWhitespace("[[F# code]]", "Test"));
            Assert.AreEqual("[[J# code]]", Parsers.FixLinkWhitespace("[[J# code]]", "Test"));

            Assert.AreEqual("<ref>[[abcdef-abcdef-abcdef-abcdef-abcdef-abcdef-abcdef]]</ref>", Parsers.FixLinkWhitespace("<ref>[[ abcdef-abcdef-abcdef-abcdef-abcdef-abcdef-abcdef]]</ref>", "foo"));
        }

        [Test]
        public void TestCanonicalizeTitle()
        {
            Assert.AreEqual("foo bar", Parsers.CanonicalizeTitle("foo_bar"));
            Assert.AreEqual("foo bar", Parsers.CanonicalizeTitle("foo bar"));
            Assert.AreEqual("foo (bar)", Parsers.CanonicalizeTitle("foo_%28bar%29"));
            Assert.AreEqual(@"foo+bar", Parsers.CanonicalizeTitle(@"foo%2Bbar"));
            Assert.AreEqual("foo (bar)", Parsers.CanonicalizeTitle("foo_(bar)"));
            Assert.AreEqual("Template:foo bar", Parsers.CanonicalizeTitle("Template:foo_bar"));

            Assert.AreEqual("foo_bar}}", Parsers.CanonicalizeTitle("foo_bar}}"), "no change to invalid title");

            // it may or may not fix it, but shouldn't break anything
            StringAssert.Contains("{{bar_boz}}", Parsers.CanonicalizeTitle("foo_bar{{bar_boz}}"));

            Variables.UnderscoredTitles.Add(@"Foo bar");
            Assert.AreEqual("foo_bar", Parsers.CanonicalizeTitle("foo_bar"));
            Assert.AreEqual("Foo_bar", Parsers.CanonicalizeTitle("Foo_bar"));
        }

        [Test]
        public void CanonicalizeTitleAggressively()
        {
            Assert.AreEqual("Foo", Parsers.CanonicalizeTitleAggressively("Foo"));

            Assert.AreEqual("Foo (bar)", Parsers.CanonicalizeTitleAggressively("foo_%28bar%29#anchor"));
            Assert.AreEqual("Wikipedia:Foo", Parsers.CanonicalizeTitleAggressively("project : foo"));
            Assert.AreEqual("File:Foo.jpg", Parsers.CanonicalizeTitleAggressively("Image: foo.jpg "));

            // a bit of ambiguousness here, but
            // https://en.wikipedia.org/wiki/Wikipedia:AWB/B#Problem_.28on_runecape_wikia.29_with_articles_with_.2B_in_the_name.
            Assert.AreEqual("Romeo+Juliet", Parsers.CanonicalizeTitleAggressively("Romeo+Juliet"));

            Assert.AreEqual("Foo", Parsers.CanonicalizeTitleAggressively(":Foo"));
            Assert.AreEqual("Foo", Parsers.CanonicalizeTitleAggressively(": Foo"));
            Assert.AreEqual(":Foo", Parsers.CanonicalizeTitleAggressively("::Foo"));
            Assert.AreEqual("User:Foo", Parsers.CanonicalizeTitleAggressively(":user:Foo"));

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_14#List_of_.22User_talk:.22_pages_change_to_list_of_.22Talk:.22_pages_when_started
            Assert.AreEqual("User talk:Foo", Parsers.CanonicalizeTitleAggressively("User talk:Foo"));
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
            Assert.AreEqual("", Parsers.FixEmptyLinksAndTemplates("[[[[  ]]]]"));
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
    }
}