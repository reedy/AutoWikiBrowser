﻿/*
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
        public void FixCitationTemplatesNewlineInTitle()
        {
            Assert.AreEqual(@"now {{cite web| url=a.com|title=hello world|format=PDF}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=hello
world|format=PDF}} was"), "newline converted to space");
            const string NoURL = @"now {{cite news|title=hello
world|format=PDF}} was";

            Assert.AreEqual(NoURL, Parsers.FixCitationTemplates(NoURL), "title newline not changed when no URL");
        }

        [Test]
        public void UnspacedCommaPageRange()
        {
            Assert.AreEqual(@"{{cite book|url=http://www.stie.com | pages=55, 59 }}", Parsers.FixCitationTemplates(@"{{cite book|url=http://www.stie.com | pages=55,59 }}"));
            Assert.AreEqual(@"{{cite book|url=http://www.stie.com | pages=483, 491–492 }}", Parsers.FixCitationTemplates(@"{{cite book|url=http://www.stie.com | pages=483,491–492 }}"));
            Assert.AreEqual(@"{{cite book|url=http://www.stie.com | pages=267–268, 273, 299 }}", Parsers.FixCitationTemplates(@"{{cite book|url=http://www.stie.com | pages=267–268,273,299 }}"));
            Assert.AreEqual(@"{{cite book|url=http://www.stie.com | pages=55, 59 }}", Parsers.FixCitationTemplates(@"{{cite book|url=http://www.stie.com | pages=55, 59 }}"), "no change when already correct");
            Assert.AreEqual(@"{{cite book|url=http://www.stie.com | pages=12,354–12,386 }}", Parsers.FixCitationTemplates(@"{{cite book|url=http://www.stie.com | pages=12,354–12,386 }}"), "no change when already correct");
            Assert.AreEqual(@"{{cite book|url=http://www.stie.com | pages=12,354 }}", Parsers.FixCitationTemplates(@"{{cite book|url=http://www.stie.com | pages=12,354 }}"), "no change when already correct");

            Assert.AreEqual(@"{{cite book|url=http://www.stie.com | pages=483, 491, 492 }}", Parsers.FixCitationTemplates(@"{{cite book|url=http://www.stie.com | pages=483,491,492 }}"));
        }

        [Test]
        public void FixCitationTemplatesQuotedTitle()
        {
            Assert.AreEqual(@"now {{cite web| url=a.com|title=hello|format=PDF}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=""hello""|format=PDF}} was"));
            Assert.AreEqual(@"now {{cite web| url=a.com|title=hello|format=PDF}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=""hello|format=PDF}} was"));
            Assert.AreEqual(@"now {{cite web| url=a.com|title=hello|format=PDF}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=hello""|format=PDF}} was"));
            Assert.AreEqual(@"now {{cite web| url=a.com|trans_title=hello|format=PDF}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|trans_title=""hello""|format=PDF}} was"));

            Assert.AreEqual(@"now {{cite web| url=a.com|title=""hello"" world|format=PDF}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=""hello"" world|format=PDF}} was"));

            // curly quote cleanup to straight quotes [[MOS:PUNCT]]
            Assert.AreEqual(@"now {{cite web| url=a.com|title=hello|format=PDF}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=“hello“|format=PDF}} was"));
            Assert.AreEqual(@"now {{cite web| url=a.com|title=and ""hello"" there|format=PDF}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=and “hello“ there|format=PDF}} was"));
            Assert.AreEqual(@"now {{cite web| url=a.com|title=hello|format=PDF}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=“hello""|format=PDF}} was"));
            Assert.AreEqual(@"now {{cite web| url=a.com|title=hello|format=PDF}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=«hello»|format=PDF}} was"));
            Assert.AreEqual(@"now {{cite web| url=a.com|title=hello|format=PDF}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=“hello″|format=PDF}} was"));
            Assert.AreEqual(@"now {{cite web| url=a.com|title=now ""hello"" at|format=PDF}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=now «hello» at|format=PDF}} was"));
            Assert.AreEqual(@"now {{cite web| url=a.com|title=now ""hello"" at|format=PDF}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=now ‹hello› at|format=PDF}} was"));
            Assert.AreEqual(@"now {{cite web| url=a.com|title=now ""hello"" at ""hello2"" be|format=PDF}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=now ‹hello› at ‹hello2› be|format=PDF}} was"));
            Assert.AreEqual(@"now {{cite web| url=a.com|title=hello» second|format=PDF}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=hello» second|format=PDF}} was"), @"no change if » used as section delimeter");
            Assert.AreEqual(@"now {{cite web| url=a.com|title=hello> second|format=PDF}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=hello> second|format=PDF}} was"), @"no change if > used as section delimeter");
            Assert.AreEqual(@"now {{cite web| url=a.com|title=hello « second|format=PDF}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=hello « second|format=PDF}} was"), @"no change if « used as section delimeter");
            Assert.AreEqual(@"now {{cite web| url=a.com|title=hello› second|format=PDF}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=hello› second|format=PDF}} was"), @"no change if › used as section delimeter");
        }

        [Test]
        public void FixCitationGoogleBooks()
        {
            string correct = @"now {{cite book|title=a |url=http://books.google.com/foo | year=2009}}";
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"now {{cite web|title=a |url=http://books.google.com/foo | year=2009}}"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"now {{cite web|title=a |url=http://books.google.com/foo | year=2009}}"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"now {{cite web|title=a |url=http://books.google.com/foo | year=2009}}"));

            // whitespace not changed
            Assert.AreEqual(@"now {{ Cite book |title=a |url=http://books.google.com/foo | year=2009}}", Parsers.FixCitationTemplates(@"now {{ Cite web |title=a |url=http://books.google.com/foo | year=2009}}"));

            Assert.AreEqual(correct, Parsers.FixCitationTemplates(correct));

            string noChange = @"now {{Cite book|title=a |url=http://books.google.com/foo | year=2009 | work = some Journal}}";
            Assert.AreEqual(noChange, Parsers.FixCitationTemplates(noChange), "journal cites to Google books not changed");
        }

        [Test]
        public void FixCitationURLNoHTTP()
        {
            string correct = @"now {{cite web|title=foo | url=http://www.foo.com | date = 1 June 2010 }}";

            Assert.AreEqual(correct, Parsers.FixCitationTemplates(correct.Replace("http://", "")), "Adds http:// when URL begins www.");
            Assert.AreEqual(correct.Replace("www", "Www"), Parsers.FixCitationTemplates(correct.Replace("http://www", "Www")), "Adds http:// when URL begins www.");
            Assert.AreEqual(correct.Replace("www", "www2"), Parsers.FixCitationTemplates(correct.Replace("http://www", "www2")), "Adds http:// when URL begins www2");
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(correct), "no change if already correct URL");
            Assert.AreEqual(correct.Replace("url=", "archiveurl="), Parsers.FixCitationTemplates(correct.Replace("url=http://", "archiveurl=")), "Adds http:// when archiveurl begins www.");
            Assert.AreEqual(correct.Replace("url=", "contribution-url="), Parsers.FixCitationTemplates(correct.Replace("url=http://", "contribution-url=")), "Adds http:// when contribution-url begins www.");
            string dash = @"now {{cite web|title=foo | url=www-foo.a.com | date = 1 June 2010 }}";
            Assert.AreEqual(dash.Replace("www", "http://www"), Parsers.FixCitationTemplates(dash), "handles www-");
        }

        [Test]
        public void WorkInItalics()
        {
            string correct = @"now {{cite web| url=http://site.net/1.pdf|format=PDF|work=Foo}}";

            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"now {{cite web| url=http://site.net/1.pdf|format=PDF|work=''Foo''}}"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(correct));

            const string website = @"now {{cite web| url=http://site.net/1.pdf|format=PDF|work=''site.net''}}";
            Assert.AreEqual(website, Parsers.FixCitationTemplates(website), "italics not removed for work=website");
        }

        [Test]
        public void FixCitationYear()
        {
            string correct = @"now {{cite book|title=a |url=http://books.google.com/foo | date=2009}}";
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(correct));

            string correct2 = @"now {{cite book|title=a |url=http://books.google.com/foo | date=2009 |page=32}}";
            Assert.AreEqual(correct2, Parsers.FixCitationTemplates(correct2));

            string nochange1 = @"now {{cite book|title=a |url=http://books.google.com/foo | year=2009a}}",
            nochange2 = @"now {{cite book|title=a |url=http://books.google.com/foo | year=2009a| date = 2009-05-16}}";

            Assert.AreEqual(nochange1, Parsers.FixCitationTemplates(nochange1));
            Assert.AreEqual(nochange2, Parsers.FixCitationTemplates(nochange2));
        }

        [Test]
        public void FixCitationTemplatesDateInYear()
        {
            string correct1 = @"now {{cite book|title=a |url=http://books.google.com/foo | date=2009-10-17}}",
            correct2 = @"now {{cite book|title=a |url=http://books.google.com/foo | date=2009-10-17|last=Smith}}";

            // ISO
            Assert.AreEqual(correct1, Parsers.FixCitationTemplates(@"now {{cite book|title=a |url=http://books.google.com/foo | year=2009-10-17}}"));
            Assert.AreEqual(correct2, Parsers.FixCitationTemplates(@"now {{cite book|title=a |url=http://books.google.com/foo | year=2009-10-17|last=Smith}}"));

            // Int
            Assert.AreEqual(correct1.Replace("2009-10-17", "17 October 2009"), Parsers.FixCitationTemplates(@"now {{cite book|title=a |url=http://books.google.com/foo | year=2009-10-17}}".Replace("2009-10-17", "17 October 2009")));
            Assert.AreEqual(correct1.Replace("2009-10-17", "17 October 2009"), Parsers.FixCitationTemplates(@"now {{cite book|title=a |url=http://books.google.com/foo | year=2009-10-17}}".Replace("2009-10-17", "17 October, 2009")));
            Assert.AreEqual(correct2.Replace("2009-10-17", "17 October 2009"), Parsers.FixCitationTemplates(@"now {{cite book|title=a |url=http://books.google.com/foo | year=2009-10-17|last=Smith}}".Replace("2009-10-17", "17 October 2009")));

            // American
            Assert.AreEqual(correct1.Replace("2009-10-17", "October 17, 2009"), Parsers.FixCitationTemplates(@"now {{cite book|title=a |url=http://books.google.com/foo | year=2009-10-17}}".Replace("2009-10-17", "October 17, 2009")));
            Assert.AreEqual(correct2.Replace("2009-10-17", "October 17, 2009"), Parsers.FixCitationTemplates(@"now {{cite book|title=a |url=http://books.google.com/foo | year=2009-10-17|last=Smith}}".Replace("2009-10-17", "October 17, 2009")));
            Assert.AreEqual(correct2.Replace("2009-10-17", "October 17, 2009"), Parsers.FixCitationTemplates(@"now {{cite book|title=a |url=http://books.google.com/foo | year=2009-10-17|last=Smith}}".Replace("2009-10-17", "October, 17, 2009")));

            Assert.AreEqual(correct1, Parsers.FixCitationTemplates(correct1));

            string nochange1 = @"now {{cite book|title=a |url=http://books.google.com/foo | year=2009–2010}}";
            Assert.AreEqual(nochange1, Parsers.FixCitationTemplates(nochange1));
        }

        [Test]
        public void FixCitationTemplatesYearWithinDate()
        {
            string correct1 = @"now {{cite book|title=a |url=http://books.google.com/foo | date=2009-10-17}}",
            nochange1 = @"now {{cite book|title=a |url=http://books.google.com/foo | date=2009-10-17 | year=2009a}}",
            nochange1b = @"now {{cite book|title=a |url=http://books.google.com/foo | date=2009 | year=2009a}}",
            nochange2 = @"now {{cite book|title=a |url=http://books.google.com/foo | date=2009-10-17 | year=2004}}",
            nochange2b = @"now {{cite book|title=a |url=http://books.google.com/foo | date=17 October 2009 | year=2004}}",
            nochange2c = @"now {{cite book|title=a |url=http://books.google.com/foo | date=October 17, 2009 | year=2004}}";

            Assert.AreEqual(correct1, Parsers.FixCitationTemplates(correct1.Replace(@"}}", @"|year=2009}}")));
            Assert.AreEqual(correct1, Parsers.FixCitationTemplates(correct1.Replace(@"foo", @"foo |year=2009")));

            Assert.AreEqual(nochange1, Parsers.FixCitationTemplates(nochange1), "Harvard anchors using YYYYa are not removed");
            Assert.AreEqual(nochange1b, Parsers.FixCitationTemplates(nochange1b), "Harvard anchor using YYYYa in year and year in date: both needed so not removed");
            Assert.AreEqual(nochange2, Parsers.FixCitationTemplates(nochange2), "Year not removed if different to year in ISO date");
            Assert.AreEqual(nochange2b, Parsers.FixCitationTemplates(nochange2b), "Year not removed if different to year in International date");
            Assert.AreEqual(nochange2c, Parsers.FixCitationTemplates(nochange2c), "Year not removed if different to year in American date");
            string correct3 = @"now {{cite book|title=a |url=http://books.google.com/foo | date=17 May 2009}}";

            string nochange4 = @"{{cite book|title=a |url=http://books.google.com/foo | date=May 2009 | year=2009 }}";
            Assert.AreEqual(nochange4, Parsers.FixCitationTemplates(nochange4), "Year not removed if date is only 'Month YYYY'");

            Assert.AreEqual(correct3, Parsers.FixCitationTemplates(correct3.Replace(@"}}", @"|year=2009}}")), "year removed when within date");
            Assert.AreEqual(@"{{cite book|title=a |url=http://books.google.com/foo | year=2009 }}", Parsers.FixCitationTemplates(@"{{cite book|title=a |url=http://books.google.com/foo | date=2009 | year=2009 }}"), 
                            "Date removed if date is YYYY and year same");
        }
        

        [Test]
        public void FixCitationTemplatesMonthWithinDate()
        {
        	string correct = @"now {{cite book|title=a |url=http://books.google.com/foo | date=17 May 2009}}",
        	nochange = @"now {{cite book|title=a |url=http://books.google.com/foo | date=17 May 2009 | month=March}}";

        	Assert.AreEqual(correct, Parsers.FixCitationTemplates(correct.Replace(@"}}", @"|month=May}}")), "month removed when within date");
        	Assert.AreEqual(nochange, Parsers.FixCitationTemplates(nochange), "month not removed if different to month in date");
        	
        	Assert.AreEqual(correct, Parsers.FixCitationTemplates(correct.Replace(@"}}", @"|month=5}}")), "number month removed when within date");
        	Assert.AreEqual(correct, Parsers.FixCitationTemplates(correct.Replace(@"}}", @"|month=05}}")), "number month removed when within date");
        	
        	string correct2 = @"now {{cite book|title=a |url=http://books.google.com/foo | date=17 December 2009}}",
        	nochange2 = @"now {{cite book|title=a |url=http://books.google.com/foo | date=17 May 2009 | month=2}}";
        	Assert.AreEqual(correct2, Parsers.FixCitationTemplates(correct2.Replace(@"}}", @"|month=12}}")), "number month removed when within date");
        	Assert.AreEqual(nochange2, Parsers.FixCitationTemplates(nochange2), "nn month not removed if different to month in date");

        	Assert.AreEqual(@"{{cite journal|last=Mahoney|first=Noreen|date=2010 April 14|volume=58}}", 
        	                Parsers.FixCitationTemplates(@"{{cite journal|last=Mahoney|first=Noreen|date=2010 April 14|year=2010|month=April|volume=58}}"), "year not added when date already has it");

        	string correct3 = @"now {{cite book|title=a |url=http://books.google.com/foo | date=2009-12-17}}";
        	Assert.AreEqual(correct3, Parsers.FixCitationTemplates(correct3.Replace(@"}}", @"|month=December}}")), "number month removed when within date");
        }

        [Test]
        public void FixCitationTemplatesDayMonthInDate()
        {
            string correct1 = @"now {{cite book|title=a |url=http://books.google.com/foo | date=17 October 2009 }}";

            Assert.AreEqual(correct1, Parsers.FixCitationTemplates(@"now {{cite book|title=a |url=http://books.google.com/foo | date=17 October |year=2009}}"));
            Assert.AreEqual(correct1.Replace("17 October", "October 17,"), Parsers.FixCitationTemplates(@"now {{cite book|title=a |url=http://books.google.com/foo | date=October 17 |year=2009}}"));
            Assert.AreEqual(correct1, Parsers.FixCitationTemplates(correct1));
        }

        [Test]
        public void CitationPublisherToWork()
        {
            string correct0 = @"{{cite news|url=http://www.timesonline.co.uk/foo494390.htm | date =2008-09-07 | work=The Times }}",
            correct1 = @"{{cite web|url=http://www.timesonline.co.uk/foo494390.htm | date =2008-09-07 | work=The Times }}",
            correct2 = @"{{citeweb|url=http://www.timesonline.co.uk/foo494390.htm | date =2008-09-07 | work = The Times }}",
            correct3 = @"{{cite web|url=http://www.timesonline.co.uk/foo494390.htm | work= |date =2008-09-07 | work = The Times }}";

            Assert.AreEqual(correct0, Parsers.CitationPublisherToWork(correct0.Replace("work", "publisher")));
            Assert.AreEqual(correct1, Parsers.CitationPublisherToWork(correct1.Replace("work", "publisher")));
            Assert.AreEqual(correct2, Parsers.CitationPublisherToWork(correct2.Replace("work", "publisher")));

            // work present but null
            Assert.AreEqual(correct3, Parsers.CitationPublisherToWork(correct3.Replace("work =", "publisher =")));

            string workalready1 = @"{{cite web|url=http://www.timesonline.co.uk/foo494390.htm | publisher=Media International |date =2008-09-07 | work = The Times }}";
            // work already
            Assert.AreEqual(workalready1, Parsers.CitationPublisherToWork(workalready1));
            Assert.AreEqual(correct0, Parsers.CitationPublisherToWork(correct0));

            // no cite web/news
            const string citeJournal = @"{{cite journal|url=http://www.timesonline.co.uk/foo494390.htm | date =2008-09-07 | publisher=The Times }}";
            Assert.AreEqual(citeJournal, Parsers.CitationPublisherToWork(citeJournal));
        }

        [Test]
        public void FixCitationDupeFields()
        {
            string correct = @"{{cite web|url=a |title=b | accessdate=11 May 2008|year=2008}}";
            string correct2 = @"{{cite web|url=a |title=b | accessdate=11 May 2008|year=2008|work=here}}";
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008|year=2008}}"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=200 | accessdate=11 May 2008|year=2008}}"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year= | accessdate=11 May 2008|year=2008}}"));

            Assert.AreEqual(correct2, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008|year=2008|work=here}}"));
            Assert.AreEqual(correct2, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |  year=   2008 | accessdate=11 May 2008|year=2008|work=here}}"));
            Assert.AreEqual(correct2, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=200| accessdate=11 May 2008|year=2008|work=here}}"));
            Assert.AreEqual(correct2, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year= | accessdate=11 May 2008|year=2008|work=here}}"));

            string correct3 = @"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008|work=here there}}";
            Assert.AreEqual(correct3, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008|work=here there|work=here}}"));
            Assert.AreEqual(correct3, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008|work=here|work=here there}}"));
            Assert.AreEqual(correct3, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008|work=here there|work = here }}"));
            Assert.AreEqual(correct3, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008|work=here there|work=here th}}"));
            Assert.AreEqual(correct3, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008|work=here there|work=here there}}"));

            string nochange1 = @"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008|year=2004}}";
            Assert.AreEqual(nochange1, Parsers.FixCitationTemplates(nochange1));

            string nochange2 = @"{{cite web|url=http://www.tise.com/abc|year=2008|page.php=7 |title=b |year=2008 | accessdate=11 May 2008}}";
            Assert.AreEqual(nochange2, Parsers.FixCitationTemplates(nochange2));

            string nochange3 = @"{{cite book|title=b |year=2008 | accessdate=11 May 2008|year=2004}}";
            Assert.AreEqual(nochange3, Parsers.FixCitationTemplates(nochange3));

            // null fields
            Assert.AreEqual(@"now {{cite web| url=http://site.net |accessdate = 2008-10-08|title=hello}}", Parsers.FixCitationTemplates(@"now {{cite web| url=http://site.net |title=|accessdate = 2008-10-08|title=hello}}"));
            Assert.AreEqual(@"now {{cite web| url=http://site.net |accessdate = 2008-10-08|title=hello|work=BBC}}", Parsers.FixCitationTemplates(@"now {{cite web| url=http://site.net |title=|accessdate = 2008-10-08|title=hello|work=BBC}}"));
            Assert.AreEqual(@"now {{Cite web| title = hello | url=http://site.net |accessdate = 2008-10-08}}", Parsers.FixCitationTemplates(@"now {{Cite web| title = hello | url=http://site.net |accessdate = 2008-10-08|title=}}"));
            Assert.AreEqual(@"now {{cite web| url=http://site.net |accessdate = 2008-10-08| title = hello }}", Parsers.FixCitationTemplates(@"now {{cite web| url=http://site.net |title=|accessdate = 2008-10-08| title = hello }}"));
            Assert.AreEqual(@"now {{cite web| title=hello|url=http://site.net |date = 2008-10-08}}", Parsers.FixCitationTemplates(@"now {{cite web| title=hello|title=|url=http://site.net |date = 2008-10-08}}"));
            Assert.AreEqual(@"now {{cite web| url=http://site.net |title=hello|first=|accessdate = 2008-10-08}}", Parsers.FixCitationTemplates(@"now {{cite web| url=http://site.net |title=hello|first=|accessdate = 2008-10-08|first=}}"));

            //no Matches
            Assert.AreEqual(@"now {{cite web| title=hello|url=http://site.net |date = 2008-10-08|title=HELLO}}", Parsers.FixCitationTemplates(@"now {{cite web| title=hello|url=http://site.net |date = 2008-10-08|title=HELLO}}")); // case of field value different
            Assert.AreEqual(@"now {{cite web| url=http://site.net |title=hello|accessdate = 2008-10-08|name=hello}}", Parsers.FixCitationTemplates(@"now {{cite web| url=http://site.net |title=hello|accessdate = 2008-10-08|name=hello}}"));

            // duplicate parameter with number at end (last1 etc.) not deduped
            Assert.AreEqual(@"now {{cite web| url=http://site.net |title=hello|accessdate = 2008-10-08|last1=hello|last1=hello}}", Parsers.FixCitationTemplates(@"now {{cite web| url=http://site.net |title=hello|accessdate = 2008-10-08|last1=hello|last1=hello}}"));
        }

        [Test]
        public void MergeCiteWebAccessDateYear()
        {
            string correct = @"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008}}";
            string correct2 = @"{{cite web|url=a |title=b |year=2008 | accessdate=May 11, 2008}}";
            string correct3 = @"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008 |work=c}}";
            string correct4 = @"{{cite book|url=a |title=b |year=2008 | accessdate=11 May 2008 |work=c}}";

            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=11 May| accessyear=2008}}"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=11 May| accessyear = 2008  }}"));
            Assert.AreEqual(correct2, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=May 11| accessyear=2008}}"));
            Assert.AreEqual(correct2, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=May 11| accessyear = 2008  }}"));
            Assert.AreEqual(correct3, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=11 May | accessyear = 2008 |work=c}}"));
            Assert.AreEqual(correct4, Parsers.FixCitationTemplates(@"{{cite book|url=a |title=b |year=2008 | accessdate=11 May | accessyear = 2008 |work=c}}"));

            // only for cite web
            string nochange2 = @"{{cite podcast|url=a |title=b |year=2008 | accessdate=11 May |accessyear=2008 |work=c}}";
            Assert.AreEqual(nochange2, Parsers.FixCitationTemplates(nochange2));
        }

        [Test]
        public void AccessDayMonthDay()
        {
            string a = @"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008}}";
            Assert.AreEqual(a, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008|accessdaymonth=   }}"));
            Assert.AreEqual(a, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008|accessmonthday  =   }}"));
            Assert.AreEqual(a, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008|accessmonth  =   }}"));
            Assert.AreEqual(a, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 |accessmonthday  = | accessdate=11 May 2008}}"));

            string notempty = @"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008|accessdaymonth=Foo   }}";
            Assert.AreEqual(notempty, Parsers.FixCitationTemplates(notempty));
        }

        [Test]
        public void FixCitationTemplatesAccessYear()
        {
            string a = @"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008}}";
            Assert.AreEqual(a, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008|accessyear=2008   }}"));
            Assert.AreEqual(a, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008| accessyear =  2008   }}"));
            Assert.AreEqual(a, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 |accessyear=2008   | accessdate=11 May 2008}}"));

            string yearDoesNotMatch = @"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008|accessyear=Winter   }}";
            Assert.AreEqual(yearDoesNotMatch, Parsers.FixCitationTemplates(yearDoesNotMatch));
        }

        [Test]
        public void FixCitationTemplatesDateLeadingZero()
        {
            string a = @"{{cite web|url=a |title=b |year=2008 | accessdate=1 May 2008}}";
            Assert.AreEqual(a, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=01 May 2008}}"));

            string a0 = @"{{cite web|url=a |title=b | accessdate=1 May 2008 | date=May 1, 2008}}";
            Assert.AreEqual(a0, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b | accessdate=01 May 2008 | date=May 01, 2008}}"));

            string a2 = @"{{cite web|url=a |title=b |year=2008 | accessdate=1 May 1998}}";
            Assert.AreEqual(a2, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=01 May 1998}}"));
            Assert.AreEqual(a2.Replace(@"}}", @" }}"), Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=01 May 1998 }}"));

            string b = @"{{cite book|url=a |title=b | date=May 1, 2008}}";
            Assert.AreEqual(b, Parsers.FixCitationTemplates(@"{{cite book|url=a |title=b | date=May 01, 2008}}"));
            Assert.AreEqual(b, Parsers.FixCitationTemplates(@"{{cite book|url=a |title=b | date=May 01 2008}}"));

            string c = @"{{cite book|url=a |title=b | date=May 1, 2008|author=Lee}}";
            Assert.AreEqual(c, Parsers.FixCitationTemplates(@"{{cite book|url=a |title=b | date=May 01, 2008|author=Lee}}"));

            Assert.AreEqual(c, Parsers.FixCitationTemplates(@"{{cite book|url=a |title=b |year=2008 | date=May 01|author=Lee}}"), "handles field merge and leading zero in date");
        }

        [Test]
        public void FixCitationTemplates()
        {
            Assert.AreEqual(@"{{cite web|title=foo|url=http://site.net|year=2009}}", Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009|format=HTML}}"));
            Assert.AreEqual(@"{{cite web|title=foo|url=http://site.net|year=2009}}", Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009|format=HTM}}"));
            Assert.AreEqual(@"{{cite web|title=foo|url=http://site.net|year=2009}}", Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009|format = HTML}}"));
            Assert.AreEqual(@"{{cite web|title=foo|url=http://site.net|year=2009}}", Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009| format  =HTML  }}"));
            Assert.AreEqual(@"{{cite web|title=foo|url=http://site.net|year=2009}}", Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009|     format=HTM}}"));
            Assert.AreEqual(@"{{Citation|title=foo|url=http://site.net|year=2009}}", Parsers.FixCitationTemplates(@"{{Citation|title=foo|url=http://site.net|format=HTML|year=2009}}"));
            Assert.AreEqual(@"{{cite web|title=foo|url=http://site.net|year=2009}}", Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009|format=[[HTML]]}}"));

            //removal of unneccessary language field
            Assert.AreEqual(@"{{cite web|title=foo|url=http://site.net|year=2009}}", Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009|language=English}}"));
            Assert.AreEqual(@"{{cite web|title=foo|url=http://site.net|year=2009}}", Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009|language = English}}"));
            Assert.AreEqual(@"{{cite web|title=foo|url=http://site.net|year=2009}}", Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009|language=english}}"));
            Assert.AreEqual(@"{{cite web|title=foo|url=http://site.net|year=2009}}", Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009|language=en}}"));

            //fix language field
            Assert.AreEqual(@"{{cite web|title=foo|url=http://site.net|year=2009}}", Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009|language={{en icon}}}}"));
            Assert.AreEqual(@"{{cite web|title=foo|url=http://site.net|year=2009|language=sv}}", Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009|language={{sv icon}}}}"));
            Assert.AreEqual(@"{{cite web|title=foo|url=http://site.net|year=2009|language=de}}", Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009|language={{de icon}}}}"));
            Assert.AreEqual(@"{{cite web|title=foo|url=http://site.net|year=2009|language=el|publisher=Ser}}", Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009|language={{el icon}}|publisher=Ser}}"));

            // removal of null 'format=' when URL is to HTML
            Assert.AreEqual(@"{{cite web|title=foo|url=http://site.net|year=2009|format=}}", Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009|format=}}"));
            Assert.AreEqual(@"{{cite web|title=foo|url=http://site.net/a.htm|year=2009}}", Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net/a.htm|year=2009|format=}}"));
            Assert.AreEqual(@"{{cite web|title=foo|url=http://site.net/a.html|year=2009}}", Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net/a.html|year=2009|format=}}"));
            Assert.AreEqual(@"{{cite web|title=foo|url=http://site.net/a.HTML|year=2009}}", Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net/a.HTML|year=2009|format=}}"));

            // removal of null 'origdate='
            Assert.AreEqual(@"{{cite web|title=foo|url=http://site.net|year=2009}}", Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009|origdate=}}"));
            Assert.AreEqual(@"{{cite web|title=foo|url=http://site.net|year=2009|origdate=2005}}", Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009|origdate=2005}}"));

            // id=ASIN... fix
            Assert.AreEqual(@"{{cite book|title=foo|asin=123456789X|year=2009}}", Parsers.FixCitationTemplates(@"{{cite book|title=foo|id=ASIN 123456789X|year=2009}}"));
            Assert.AreEqual(@"{{cite book|title=foo|asin=123456789X |year=2009}}", Parsers.FixCitationTemplates(@"{{cite book|title=foo|id=ASIN 123456789X |year=2009}}"));
            Assert.AreEqual(@"{{cite book|title=foo|asin=123-45678-9-X|year=2009}}", Parsers.FixCitationTemplates(@"{{cite book|title=foo|id=ASIN: 123-45678-9-X|year=2009}}"));

            const string NoChangeSpacedEndashInTitle = @"{{cite web | author=IGN staff | year=2008 | title=IGN Top 100 Games 2008 – 2 Chrono Trigger | url=http://top100.ign.com/2008/ign_top_game_2.html | publisher=IGN | accessdate=March 13, 2009}}";

            Assert.AreEqual(NoChangeSpacedEndashInTitle, Parsers.FixCitationTemplates(NoChangeSpacedEndashInTitle));

            const string NoChangeFormatGivesSize = @"{{cite web|title=foo|url=http://site.net/asdfsadf.PDF|year=2009|format=150 MB}}";

            Assert.AreEqual(NoChangeFormatGivesSize, Parsers.FixCitationTemplates(NoChangeFormatGivesSize));
        }

        [Test]
        public void FixCitationTemplatesISBN()
        {
            // id=ISBN... fix
            Assert.AreEqual(@"{{cite book|title=foo|isbn=123456789X|year=2009}}", Parsers.FixCitationTemplates(@"{{cite book|title=foo|id=ISBN 123456789X|year=2009}}"));
            Assert.AreEqual(@"{{cite book|title=foo|isbn=123456789X |year=2009}}", Parsers.FixCitationTemplates(@"{{cite book|title=foo|id=ISBN 123456789X |year=2009}}"));
            Assert.AreEqual(@"{{cite book|title=foo|isbn=123-45678-9-X|year=2009}}", Parsers.FixCitationTemplates(@"{{cite book|title=foo|id=ISBN: 123-45678-9-X|year=2009}}"));

            string doubleISBN = @"{{cite book|title=foo|id=ISBN 012345678X, 978012345678X|year=2009}}";
            Assert.AreEqual(doubleISBN, Parsers.FixCitationTemplates(doubleISBN), "no changes when two isbns present");
            doubleISBN = @"{{cite book|title=foo|id=ISBN 012345678X ISBN 978012345678X|year=2009}}";
            Assert.AreEqual(doubleISBN, Parsers.FixCitationTemplates(doubleISBN), "no changes when two isbns present");

            string existingISBN = @"{{cite book|title=foo|id=ISBN 012345678X |isbn= 978012345678X|year=2009}}";
            Assert.AreEqual(existingISBN, Parsers.FixCitationTemplates(existingISBN), "no changes when isbn param already has value");

            existingISBN = @"{{cite book|title=foo|id=ISBN 012345678X |ISBN= 978012345678X|year=2009}}";
            Assert.AreEqual(existingISBN, Parsers.FixCitationTemplates(existingISBN), "no changes when isbn param already has value");

            // ISBN format fixes
            Assert.AreEqual(@"{{cite book|title=foo|isbn=123456789X|year=2009}}", Parsers.FixCitationTemplates(@"{{cite book|title=foo|isbn=ISBN 123456789X|year=2009}}"));
            Assert.AreEqual(@"{{cite book|title=foo|ISBN=123456789X|year=2009}}", Parsers.FixCitationTemplates(@"{{cite book|title=foo|ISBN=ISBN 123456789X|year=2009}}"));
            Assert.AreEqual(@"{{cite book|title=foo|isbn=123456789X|year=2009}}", Parsers.FixCitationTemplates(@"{{cite book|title=foo|isbn=ISBN  123456789X|year=2009}}"));
            Assert.AreEqual(@"{{cite book|title=foo|isbn=123456789X|year=2009}}", Parsers.FixCitationTemplates(@"{{cite book|title=foo|isbn=ISBN123456789X|year=2009}}"));
            Assert.AreEqual(@"{{cite book|title=foo|isbn=123456789X|year=2009}}", Parsers.FixCitationTemplates(@"{{cite book|title=foo|isbn=isbn123456789X|year=2009}}"));

            Assert.AreEqual(@"{{cite book|title=foo|isbn=123456789X|year=2009}}", Parsers.FixCitationTemplates(@"{{cite book|title=foo|isbn=ISBN 123456789X.|year=2009}}"));
            Assert.AreEqual(@"{{cite book|title=foo|isbn=123456789X|year=2009}}", Parsers.FixCitationTemplates(@"{{cite book|title=foo|isbn=123456789X.|year=2009}}"));
            Assert.AreEqual(@"{{cite book|title=foo|isbn=123456789X|year=2009}}", Parsers.FixCitationTemplates(@"{{cite book|title=foo|isbn=123456789X..|year=2009}}"));
            Assert.AreEqual(@"{{cite book|title=foo|isbn=123456789X|year=2009}}", Parsers.FixCitationTemplates(@"{{cite book|title=foo|isbn=123456789X,|year=2009}}"));
            Assert.AreEqual(@"{{cite book|title=foo|isbn=123456789X|year=2009}}", Parsers.FixCitationTemplates(@"{{cite book|title=foo|isbn=123456789X;|year=2009}}"));
            Assert.AreEqual(@"{{cite book|title=foo|isbn=123456789X|year=2009}}", Parsers.FixCitationTemplates(@"{{cite book|title=foo|isbn=123456789X:|year=2009}}"));

            Assert.AreEqual(@"{{cite book|title=foo|isbn=123-456-789-X|year=2009}}", Parsers.FixCitationTemplates(@"{{cite book|title=foo|isbn=123–456–789–X :|year=2009}}"));
            Assert.AreEqual(@"{{cite book|title=foo|isbn=123-4-56789-X |year=2009}}", Parsers.FixCitationTemplates(@"{{cite book|title=foo|isbn=123–4–56789–X |year=2009}}"));
            Assert.AreEqual(@"{{cite book|title=foo|isbn=123-4-56789-X |year=2009}}", Parsers.FixCitationTemplates(@"{{cite book|title=foo|isbn=123‐4‐56789‐X |year=2009}}")); // U+2010 character
            Assert.AreEqual(@"{{cite book|title=foo|isbn=123-4-56789-X |year=2009}}", Parsers.FixCitationTemplates(@"{{cite book|title=foo|isbn=123‒4‒56789‒X |year=2009}}")); // U+2012 character
        }

        [Test]
        public void FixCitationTemplatesOrigYear()
        {
            Assert.AreEqual(@"{{cite book | title=ABC | publisher=Pan  | year=1950 }}", Parsers.FixCitationTemplates(@"{{cite book | title=ABC | publisher=Pan  | origyear=1950 }}"), "origyear to year when no year/date");
            Assert.AreEqual(@"{{cite book | title=ABC | publisher=Pan  | year=1950 }}", Parsers.FixCitationTemplates(@"{{cite book | title=ABC | publisher=Pan  | origyear=1950 | year =}}"), "origyear to year when blank year");

            const string nochange1 = @"{{cite book | title=ABC | publisher=Pan | year=2004 | origyear=1950 }}", nochange2 = @"{{cite book | title=ABC | publisher=Pan | date=May 2004 | origyear=1950 }}"
                , nochange3 = @"{{cite book | title=ABC | publisher=Pan | origyear=11 May 1950 }}";

            Assert.AreEqual(nochange1, Parsers.FixCitationTemplates(nochange1), "origyear valid when year present");
            Assert.AreEqual(nochange2, Parsers.FixCitationTemplates(nochange2), "origyear valid when date present");
            Assert.AreEqual(nochange3, Parsers.FixCitationTemplates(nochange3), "origyear not renamed when more than just a year");
        }

        [Test]
        public void FixCitationTemplatesEnOnly()
        {
#if DEBUG
            const string bad = @"{{cite web|title=foo|url=http://site.net|year=2009|format=HTML}}";

            Variables.SetProjectLangCode("fr");
            Assert.AreEqual(bad, Parsers.FixCitationTemplates(bad));

            Variables.SetProjectLangCode("en");
            Assert.AreEqual(@"{{cite web|title=foo|url=http://site.net|year=2009}}", Parsers.FixCitationTemplates(bad));
#endif
        }

        [Test]
        public void FixCitationTemplatesPagesPP()
        {
            // removal of pp. from 'pages' field
            Assert.AreEqual(@"{{cite book|author=Smith|title=Great|pages=57–59}}", Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|pages=pp. 57–59}}"));
            Assert.AreEqual(@"{{cite book|author=Smith|title=Great|pages=57–59}}", Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|pages=pgs. 57–59}}"));
            Assert.AreEqual(@"{{cite book|author=Smith|title=Great|pages=57–59}}", Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|pages=pg. 57–59}}"));
            Assert.AreEqual(@"{{cite book|author=Smith|title=Great|pages=57–59}}", Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|pages=pp.57–59}}"));
            Assert.AreEqual(@"{{cite book|author=Smith|title=Great|pages=57–59}}", Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|pages=pp.&nbsp;57–59}}"));
            Assert.AreEqual(@"{{cite book|author=Smith|title=Great|pages= 57–59}}", Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|pages= pp. 57–59}}"));
            Assert.AreEqual(@"{{cite book|author=Smith|title=Great|pages=57–59|year=2007}}", Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|pages=pp. 57-59|year=2007}}"));
            Assert.AreEqual(@"{{cite book|author=Smith|title=Great|page=57}}", Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|page=p. 57}}"));
            Assert.AreEqual(@"{{cite book|author=Smith|title=Great|page=57}}", Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|page=p 57}}"));
            Assert.AreEqual(@"{{cite book|author=Smith|title=Great|page=57}}", Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|page=pp 57}}"));

            const string nochange0 = @"{{cite book|author=Smith|title=Great|page= 57}}", nochange1 = @"{{cite book|author=Smith|title=Great|page= para 57}}",
            nochange2 = @"{{cite book|author=Smith|title=Great|page= P57}}";
            Assert.AreEqual(nochange0, Parsers.FixCitationTemplates(nochange0));
            Assert.AreEqual(nochange1, Parsers.FixCitationTemplates(nochange1));
            Assert.AreEqual(nochange2, Parsers.FixCitationTemplates(nochange2));

            // not when nopp
            Assert.AreEqual(@"{{cite book|author=Smith|title=Great|pages=pp. 57–59|nopp=yes}}", Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|pages=pp. 57-59|nopp=yes}}"));

            // not for cite journal
            const string journal = @"{{cite journal|author=Smith|title=Great|page=p.57}}";
            Assert.AreEqual(journal, Parsers.FixCitationTemplates(journal));
        }

        [Test]
        public void FixCitationTemplatesPageRangeName()
        {
            string correct = @"{{cite book|author=Smith|title=Great|pages=57–59}}";
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|pages=pp. 57–59}}"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|page=pp. 57–59}}"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|pages=57–59}}"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|pages=57 – 59}}"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|pages=57 – 59}}"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|pages=57--59}}"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|page=57–59}}"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|page=57—59}}"));

            Assert.AreEqual(correct.Replace("–", ", "), Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|page=57, 59}}"), "page -> pages for comma list of page numbers");

            correct = @"{{cite book|author=Smith|title=Great|pages=57&ndash;59}}";
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|pages=57 &ndash; 59}}"));

            const string nochange = @"{{cite book|author=Smith|title=Great|pages=12,255}}";
            Assert.AreEqual(nochange, Parsers.FixCitationTemplates(nochange));

            const string correct2 = @"{{cite book |title=Evaluation of x |last=Office |year=2001 |pages=1–2 |accessdate=39 June 2011 }}";

            Assert.AreEqual(correct2, Parsers.FixCitationTemplates(@"{{cite book |title=Evaluation of x |last=Office |year=2001 |page=1-2 |pages= |accessdate=39 June 2011 }}"));
        }

        [Test]
        public void FixCitationTemplatesVolume()
        {
            const string correct = @"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume= 3|issue= 3|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ";
            Assert.AreEqual(correct,
                            Parsers.FixCitationTemplates(@"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume=Vol. 3|issue=No. 3|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ"));
            Assert.AreEqual(correct,
                            Parsers.FixCitationTemplates(@"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume=Volumes 3|issue=No. 3|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ"));

            Assert.AreEqual(correct,
                            Parsers.FixCitationTemplates(@"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume=vol. 3|issue=No. 3|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ"));

            Assert.AreEqual(correct,
                            Parsers.FixCitationTemplates(@"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume=volume 3|issue=Issue 3|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ"));

            Assert.AreEqual(correct,
                            Parsers.FixCitationTemplates(@"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume=volume 3|issue=Issues 3|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ"));

            Assert.AreEqual(correct,
                            Parsers.FixCitationTemplates(@"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume=volume 3|issue=Issue 3|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ"));
            Assert.AreEqual(correct,
                            Parsers.FixCitationTemplates(@"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume=Vol. 3|issue=Nos. 3|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ"));

            Assert.AreEqual(@"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume= 3| issue = 3|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ",
                            Parsers.FixCitationTemplates(@"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume=volume 3 Issue 3|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ"));

            Assert.AreEqual(@"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume= 3| issue = 3–4|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ",
                            Parsers.FixCitationTemplates(@"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume=volume 3 Issues 3–4|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ"));
            Assert.AreEqual(@"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume=3| issue =  3–4|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ",
                            Parsers.FixCitationTemplates(@"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume=3, Nos. 3–4|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ"));


            Assert.AreEqual(@"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume= 3| issue = 3–4|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ",
                            Parsers.FixCitationTemplates(@"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume=volume 3 numbers 3–4|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ"));

            string nochange1 = @"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume=special numbers 3–4|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}}";

            Assert.AreEqual(nochange1, Parsers.FixCitationTemplates(nochange1));

            string nochange2 = @"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: |volume=3 Issue 3|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm|issue=December}} Robert M. Price (ed.), Bloomfield, NJ";

            Assert.AreEqual(nochange2, Parsers.FixCitationTemplates(nochange2));
        }

        [Test]
        public void CiteTemplatesPageRange()
        {
            const string correct = @"{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu |volume= 3|issue= 3|pages = 140–148}}";

            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu |volume= 3|issue= 3|pages = 140-148}}")); // hyphen
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu |volume= 3|issue= 3|pages = 140   - 148}}")); // hyphen

            const string journalstart = @"{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu |volume= 3|issue= 3| ";

            Assert.AreEqual(journalstart + @"pages = 140–148}}", Parsers.FixCitationTemplates(journalstart + @"pages = 140   - 148}}")); // hyphen

            Assert.AreEqual(journalstart + @"pages = 140–8}}", Parsers.FixCitationTemplates(journalstart + @"pages = 140-8}}")); // hyphen

            Assert.AreEqual(journalstart + @"pages = 140–48}}", Parsers.FixCitationTemplates(journalstart + @"pages = 140 -48}}")); // hyphen

            Assert.AreEqual(journalstart + @"pages = 140–148}}", Parsers.FixCitationTemplates(journalstart + @"pages = 140   - 148}}")); // hyphen
            Assert.AreEqual(journalstart + @"pages = 940–1083}}", Parsers.FixCitationTemplates(journalstart + @"pages = 940   - 1083}}")); // hyphen

            // multiple ranges
            Assert.AreEqual(journalstart + @"pages = 140–148, 150, 152–157}}", Parsers.FixCitationTemplates(journalstart + @"pages = 140-148, 150, 152-157}}")); // hyphen

            const string nochange1 = @"{{cite conference
  | first = Owen
  | title = System Lifecycle Cost
  | booktitle = AIAA Space 2007
  | pages = Paper No. AIAA-2007–6023
  | year = 2007
  }}";

            Assert.AreEqual(nochange1, Parsers.FixCitationTemplates(nochange1)); // range over 999 pages
        }

        [Test]
        public void HarvTemplatesPageRange()
        {
            Assert.AreEqual(@"{{harv|Smith|2005|pp=55–59}}", Parsers.FixCitationTemplates(@"{{harv|Smith|2005|pp=55–59}}"));
            Assert.AreEqual(@"{{harvnb|Smith|2005|pp=55–59}}", Parsers.FixCitationTemplates(@"{{harvnb|Smith|2005|pp=55–59}}"));
            Assert.AreEqual(@"{{harv|Smith|2005|pp=55–59}}", Parsers.FixCitationTemplates(@"{{harv|Smith|2005|pp=55 – 59}}"));
            Assert.AreEqual(@"{{harv|Smith|2005|pp=55–59, 77–81}}", Parsers.FixCitationTemplates(@"{{harv|Smith|2005|pp=55–59, 77-81}}"));

            Assert.AreEqual(@"{{rp|55–59, 77–81}}", Parsers.FixCitationTemplates(@"{{rp|55–59, 77-81}}"));
            Assert.AreEqual(@"{{rp|77–81}}", Parsers.FixCitationTemplates(@"{{rp|77-81}}"));
            Assert.AreEqual(@"{{rp|77}}", Parsers.FixCitationTemplates(@"{{rp|77}}"));
            Assert.AreEqual(@"{{rp|77, 80}}", Parsers.FixCitationTemplates(@"{{rp|77, 80}}"));
        }

        [Test]
        public void HarvTemplatesPP()
        {
            Assert.AreEqual(@"{{harv|Smith|2005|pp=55–59}}", Parsers.FixCitationTemplates(@"{{harv|Smith|2005|p=55–59}}"), "renames p to pp for page range");
            Assert.AreEqual(@"{{harvnb|Smith|2005|pp=55–59}}", Parsers.FixCitationTemplates(@"{{harvnb|Smith|2005|p=55–59}}"));
            Assert.AreEqual(@"{{harvnb|Smith|2005|pp=55&ndash;59}}", Parsers.FixCitationTemplates(@"{{harvnb|Smith|2005|p=55&ndash;59}}"));
            Assert.AreEqual(@"{{harv|Smith|2005|pp=55–59}}", Parsers.FixCitationTemplates(@"{{harv|Smith|2005|p=55 – 59}}"));
            Assert.AreEqual(@"{{harv|Smith|2005|pp=55–59, 77–81}}", Parsers.FixCitationTemplates(@"{{harv|Smith|2005|p=55–59, 77-81}}"));

            const string nochange = @"{{Harvnb|Shapiro|2010|p=271 (238–9)}}";
            Assert.AreEqual(nochange, Parsers.FixCitationTemplates(nochange));
        }

        [Test]
        public void CiteTemplatesPageSections()
        {
            const string journalstart = @"{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu |volume= 3|issue= 3| ";

            // do not change page sections etc.
            const string nochange1 = @"{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu |volume= 3|issue= 3|pages = 140–7-2}}";

            Assert.AreEqual(nochange1, Parsers.FixCitationTemplates(nochange1));

            Assert.AreEqual(journalstart + @"pages = 8-4}}", Parsers.FixCitationTemplates(journalstart + @"pages = 8-4}}")); // hyphen
            Assert.AreEqual(journalstart + @"pages = 8-8}}", Parsers.FixCitationTemplates(journalstart + @"pages = 8-8}}")); // hyphen
            Assert.AreEqual(journalstart + @"pages = 8-4, 17-34}}", Parsers.FixCitationTemplates(journalstart + @"pages = 8-4, 17-34}}")); // hyphen
            Assert.AreEqual(journalstart + @"pages = 17-34, 8-4}}", Parsers.FixCitationTemplates(journalstart + @"pages = 17-34, 8-4}}")); // hyphen

            // non-breaking hyphens to represent page sections rather than ranges
            const string nochange2 = @"{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu |volume= 3|issue= 3|pages = 140‑7}}", nochange3 = @"{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu |volume= 3|issue= 3|pages = 140&#8209;7}}";

            Assert.AreEqual(nochange2, Parsers.FixCitationTemplates(nochange2));
            Assert.AreEqual(nochange3, Parsers.FixCitationTemplates(nochange3));

            const string nochange4 = @"*{{cite book
 | author = United States Navy
 | pages = 4-13 to 4-17
 | chapter = 4
}}";

            Assert.AreEqual(nochange4, Parsers.FixCitationTemplates(nochange4), "two section links with word to");

            const string nochange5a = @"{{cite book | isbn = 084
 | pages = 3-262, 8-106, 15-20
 | url =
}}", nochange5b = @"{{cite book | isbn = 084
 | pages = 3-262, 3-106, 15-20
 | url =
}}";
            Assert.AreEqual(nochange5a, Parsers.FixCitationTemplates(nochange5a), "overlapping ranges");
            Assert.AreEqual(nochange5b, Parsers.FixCitationTemplates(nochange5b), "overlapping ranges, same start");
        }

        [Test]
        public void FixCitationTemplatesOrdinalDates()
        {
            Assert.AreEqual(@"{{cite web|url=http://news.bbc.co.uk/1/hi/uk_politics/8080777.stm|title=Brown pressure as Blears quits|
publisher=The BBC|date=June 3, 2009|accessdate=January 15, 2010}}", Parsers.FixCitationTemplates(@"{{cite web|url=http://news.bbc.co.uk/1/hi/uk_politics/8080777.stm|title=Brown pressure as Blears quits|
publisher=The BBC|date=June 3rd, 2009|accessdate=January 15th, 2010}}"));

            Assert.AreEqual(@"{{cite web|url=http://news.bbc.co.uk/1/hi/uk_politics/8080777.stm|title=Brown pressure as Blears quits|
publisher=The BBC|date=June 3, 2009|accessdate=January 15, 2010}}", Parsers.FixCitationTemplates(@"{{cite web|url=http://news.bbc.co.uk/1/hi/uk_politics/8080777.stm|title=Brown pressure as Blears quits|
publisher=The BBC|date=June 3rd, 2009|accessdate=January 15th 2010}}"));

            Assert.AreEqual(@"{{cite web|url=http://news.bbc.co.uk/1/hi/uk_politics/8080777.stm|title=Brown pressure as Blears quits|
publisher=The BBC|date=June 3, 2009|accessdate=January 15, 2010}}", Parsers.FixCitationTemplates(@"{{cite web|url=http://news.bbc.co.uk/1/hi/uk_politics/8080777.stm|title=Brown pressure as Blears quits|
publisher=The BBC|date=June 3rd, 2009|accessdate=January 15, 2010}}"));

            Assert.AreEqual(@"{{cite web|url=http://news.bbc.co.uk/1/hi/uk_politics/8080777.stm|title=Brown pressure as Blears quits|
publisher=The BBC|date=3 June 2009|accessdate=15 January 2010}}", Parsers.FixCitationTemplates(@"{{cite web|url=http://news.bbc.co.uk/1/hi/uk_politics/8080777.stm|title=Brown pressure as Blears quits|
publisher=The BBC|date=3rd June 2009|accessdate=15th January 2010}}"));

            // no change - only in title
            string nochange = @"{{cite web|url=http://news.bbc.co.uk/1/hi/uk_politics/8080777.stm|title=Brown pressure at January 15th, 2010}}";

            Assert.AreEqual(nochange, Parsers.FixCitationTemplates(nochange));

            Assert.AreEqual(@"{{cite web | url=http://www.foo.com| title=Bar | date=January 28, 2013 | accessdate=March 7, 2013 | other=}}", 
                            Parsers.FixCitationTemplates(@"{{cite web | url=http://www.foo.com| title=Bar | date=January 28th, 2013 | accessdate=March 07, 2013 | other=}}"));
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
        public void UppercaseCiteFields()
        {
            // single uppercase field
            Assert.AreEqual(@"{{cite web|foo=hello|title=hello}}", Parsers.FixCitationTemplates(@"{{cite web|FOO=hello|title=hello}}"));
            Assert.AreEqual(@"{{cite web|url=http://members.bib-arch.org|title=hello}}", Parsers.FixCitationTemplates(@"{{cite web|uRL=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{cite web|url=http://members.bib-arch.org|title=hello}}", Parsers.FixCitationTemplates(@"{{cite web|UrL=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{cite web|url=http://members.bib-arch.org|title=hello}}", Parsers.FixCitationTemplates(@"{{cite web|Url=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{Cite web|foo=hello|title=hello}}", Parsers.FixCitationTemplates(@"{{Cite web|FOO=hello|title=hello}}"));
            Assert.AreEqual(@"{{cite web | foo=hello|title=hello}}", Parsers.FixCitationTemplates(@"{{cite web | FOO=hello|title=hello}}"));
            Assert.AreEqual(@"{{cite web | foo = hello|title=hello}}", Parsers.FixCitationTemplates(@"{{cite web | FOO = hello|title=hello}}"));

            // multiple uppercase fields
            Assert.AreEqual(@"{{cite web|foo=hello|title=hello}}", Parsers.FixCitationTemplates(@"{{cite web|FOO=hello|Title=hello}}"));
            Assert.AreEqual(@"{{cite web|foo=hello|title=hello}}", Parsers.FixCitationTemplates(@"{{cite web|FOO=hello|TITLE=hello}}"));
            Assert.AreEqual(@"{{cite web|foo=hello|title=hello | work=BBC}}", Parsers.FixCitationTemplates(@"{{cite web|FOO=hello|TITLE=hello | Work=BBC}}"));

            //other templates
            Assert.AreEqual(@"{{cite web|foo=hello|title=hello}}", Parsers.FixCitationTemplates(@"{{cite web|FOO=hello|title=hello}}"));
            Assert.AreEqual(@"{{cite book|foo=hello|title=hello}}", Parsers.FixCitationTemplates(@"{{cite book|FOO=hello|title=hello}}"));
            Assert.AreEqual(@"{{cite news|foo=hello|title=hello}}", Parsers.FixCitationTemplates(@"{{cite news|FOO=hello|title=hello}}"));
            Assert.AreEqual(@"{{cite journal|foo=hello|title=hello}}", Parsers.FixCitationTemplates(@"{{cite journal|FOO=hello|title=hello}}"));
            Assert.AreEqual(@"{{cite paper|foo=hello|title=hello}}", Parsers.FixCitationTemplates(@"{{cite paper|FOO=hello|title=hello}}"));
            Assert.AreEqual(@"{{cite press release|foo=hello|title=hello}}", Parsers.FixCitationTemplates(@"{{cite press release|FOO=hello|title=hello}}"));
            Assert.AreEqual(@"{{cite hansard|foo=hello|title=hello}}", Parsers.FixCitationTemplates(@"{{cite hansard|FOO=hello|title=hello}}"));
            Assert.AreEqual(@"{{cite encyclopedia|foo=hello|title=hello}}", Parsers.FixCitationTemplates(@"{{cite encyclopedia|FOO=hello|title=hello}}"));
            Assert.AreEqual(@"{{citation|foo=hello|title=hello}}", Parsers.FixCitationTemplates(@"{{citation|FOO=hello|title=hello}}"));

            Assert.AreEqual(@"{{Cite web|foo=hello|title=hello}}", Parsers.FixCitationTemplates(@"{{Cite web|FOO=hello|title=hello}}"));
            Assert.AreEqual(@"{{Cite book|foo=hello|title=hello}}", Parsers.FixCitationTemplates(@"{{Cite book|FOO=hello|title=hello}}"));
            Assert.AreEqual(@"{{Cite news|foo=hello|title=hello}}", Parsers.FixCitationTemplates(@"{{Cite news|FOO=hello|title=hello}}"));
            Assert.AreEqual(@"{{Cite journal|foo=hello|title=hello}}", Parsers.FixCitationTemplates(@"{{Cite journal|FOO=hello|title=hello}}"));
            Assert.AreEqual(@"{{Cite paper|foo=hello|title=hello}}", Parsers.FixCitationTemplates(@"{{Cite paper|FOO=hello|title=hello}}"));
            Assert.AreEqual(@"{{Cite press release|foo=hello|title=hello}}", Parsers.FixCitationTemplates(@"{{Cite press release|FOO=hello|title=hello}}"));
            Assert.AreEqual(@"{{Cite hansard|foo=hello|title=hello}}", Parsers.FixCitationTemplates(@"{{Cite hansard|FOO=hello|title=hello}}"));
            Assert.AreEqual(@"{{Cite encyclopedia|foo=hello|title=hello}}", Parsers.FixCitationTemplates(@"{{Cite encyclopedia|FOO=hello|title=hello}}"));
            Assert.AreEqual(@"{{Citation|foo=hello|title=hello}}", Parsers.FixCitationTemplates(@"{{Citation|FOO=hello|title=hello}}"));

            Assert.AreEqual(@"{{cite book | author=Smith | title=Great Book | ISBN=15478454 | date=17 May 2004 }}", Parsers.FixCitationTemplates(@"{{cite book | author=Smith | Title=Great Book | ISBN=15478454 | date=17 May 2004 }}"));

            // ISBN, DOI, PMID, PMC, LCCN, ASIN is allowed to be uppercase
            string ISBN = @"{{cite book | author=Smith | title=Great Book | ISBN=15478454 | date=17 May 2004 }}";
            Assert.AreEqual(ISBN, Parsers.FixCitationTemplates(ISBN));
            string ISSN = @"{{cite book | author=Smith | title=Great Book | ISSN=15478454 | date=17 May 2004 }}";
            Assert.AreEqual(ISSN, Parsers.FixCitationTemplates(ISSN));
            string OCLC = @"{{cite book | author=Smith | title=Great Book | OCLC=15478454 | date=17 May 2004 }}";
            Assert.AreEqual(OCLC, Parsers.FixCitationTemplates(OCLC));
            string DOI = @"{{cite journal| author=Smith | title=Great Book | DOI=15478454 | date=17 May 2004 }}";
            Assert.AreEqual(DOI, Parsers.FixCitationTemplates(DOI));
            string PMID = @"{{cite journal| author=Smith | title=Great Book | PMID=15478454 | date=17 May 2004 }}";
            Assert.AreEqual(PMID, Parsers.FixCitationTemplates(PMID));
            string PMC = @"{{cite journal| author=Smith | title=Great Book | PMC=15478454 | date=17 May 2004 }}";
            Assert.AreEqual(PMC, Parsers.FixCitationTemplates(PMC));
            string LCCN = @"{{cite journal| author=Smith | title=Great Book | PMC=15478454 | date=17 May 2004 }}";
            Assert.AreEqual(LCCN, Parsers.FixCitationTemplates(LCCN));
            string ASIN = @"{{cite journal| author=Smith | title=Great Book | ASIN=15478454 | date=17 May 2004 }}";
            Assert.AreEqual(ASIN, Parsers.FixCitationTemplates(ASIN));

            // don't match on part of URL
            string URL = @"{{cite news|url=http://www.expressbuzz.com/edition/story.aspx?Title=Catching++them+young&artid=rPwTAv2l2BY=&SectionID=fxm0uEWnVpc=&MainSectionID=ngGbWGz5Z14=&SectionName=RtFD/|pZbbWSsbI0jf3F5Q==&SEO=|title=Catching them young|date=August 7, 2009|work=[[The Indian Express]]|accessdate=2009-08-07}}";
            Assert.AreEqual(URL, Parsers.FixCitationTemplates(URL));
        }
        

        [Test]
        public void FixCitationTemplatesDeadLinkInFormat()
        {
            const string correct = @"{{cite web | url=http://www.site.com/article100.html | title=Foo }} {{dead link|date=May 2010}}";
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"{{cite web | url=http://www.site.com/article100.html | title=Foo | format= {{dead link|date=May 2010}}}}"), "{{dead link}} taken out of format field");

            Assert.AreEqual(correct.Replace("Foo", "Foo | format= PDF"), Parsers.FixCitationTemplates(@"{{cite web | url=http://www.site.com/article100.html | title=Foo | format= PDF {{dead link|date=May 2010}}}}"), "Only {{dead link}} taken out of format field");

            Assert.AreEqual(correct, Parsers.FixCitationTemplates(correct), "no change when already correct");

            const string NodDead = @"{{cite web | url=http://www.site.com/article100.html | title=Foo | format= PDF}}";
            Assert.AreEqual(NodDead, Parsers.FixCitationTemplates(NodDead), "no change when no dead link in format field");
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
        public void TestBracketsAtCiteTemplateURL()
        {
            const string correct = @"now {{cite web|url=http://site.net|title=hello}} was";

            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"now {{cite web|url=[http://site.net]|title=hello}} was"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"now {{cite web|url=http://site.net]|title=hello}} was"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"now {{cite web|url=[http://site.net|title=hello}} was"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"now {{cite web|url=[[http://site.net]|title=hello}} was"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"now {{cite web|url=[http://site.net]]|title=hello}} was"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"now {{cite web|url=[[http://site.net]]|title=hello}} was"));
            Assert.AreEqual(@"now {{cite web|url = http://site.net  |title=hello}} was", Parsers.FixCitationTemplates(@"now {{cite web|url = [http://site.net]  |title=hello}} was"));
            Assert.AreEqual(@"now {{cite web|title=hello |url=http://www.site.net}} was", Parsers.FixCitationTemplates(@"now {{cite web|title=hello |url=[www.site.net]}} was"), "bracket and protocol fix combined");
            Assert.AreEqual(@"now {{cite journal|title=hello | url=http://site.net }} was", Parsers.FixCitationTemplates(@"now {{cite journal|title=hello | url=[http://site.net]] }} was"));

            // no match
            Assert.AreEqual(@"now {{cite web| url=http://site.net|title=hello}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=http://site.net|title=hello}} was"));
            Assert.AreEqual(@"now {{cite web| url=[http://site.net cool site]|title=hello}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=[http://site.net cool site]|title=hello}} was"));
            Assert.AreEqual(@"now {{cite web| url=http://site.net cool site]|title=hello}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=http://site.net cool site]|title=hello}} was"));
            Assert.AreEqual(@"now {{cite web| url=[http://site.net cool site|title=hello}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=[http://site.net cool site|title=hello}} was"));
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