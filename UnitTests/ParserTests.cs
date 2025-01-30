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
using System.Text.RegularExpressions;
using NUnit.Framework;
using WikiFunctions;
using WikiFunctions.Parse;

namespace UnitTests
{
    /// <summary>
    /// This class must be inherited by test fixtures that use most parts of WikiFunctions.
    /// It ensures that WikiFunctions is aware that it's being called from unit tests, and
    /// that global data is initialised.
    /// </summary>
    public class RequiresInitialization
    {
        static RequiresInitialization()
        {
            Globals.UnitTestMode = true;
            if (WikiRegexes.Category == null) WikiRegexes.MakeLangSpecificRegexes();
        }
    }

    public class RequiresParser : RequiresInitialization
    {
        protected Parsers parser;

        public RequiresParser()
        {
            parser = new Parsers();
        }
    }

    [TestFixture]
    public class MOSTests : RequiresParser
    {
        public GenfixesTestsBase genFixes  = new GenfixesTestsBase();
        
        [Test]
        public void TestFixDateMonthOfYear()
        {
            // 'of' between month and year
            Assert.That(parser.FixDateOrdinalsAndOf(@"Now in July of 2007 a new", "test"), Is.EqualTo(@"Now in July 2007 a new"));
            Assert.That(parser.FixDateOrdinalsAndOf(@"Now ''Plaice'' in July of 2007 a new", "test"), Is.EqualTo(@"Now ''Plaice'' in July 2007 a new"));
            Assert.That(parser.FixDateOrdinalsAndOf(@"Now in January of 1907 a new", "test"), Is.EqualTo(@"Now in January 1907 a new"));
            Assert.That(parser.FixDateOrdinalsAndOf(@"Now in January of 1807 a new", "test"), Is.EqualTo(@"Now in January 1807 a new"));
            Assert.That(parser.FixDateOrdinalsAndOf(@"Now in January of 1807 and May of 1804 a new", "test"), Is.EqualTo(@"Now in January 1807 and May 1804 a new"));

            Assert.That(parser.FixDateOrdinalsAndOf(@"On 28 July of 2006 – 14 October of 2008 was", "test"), Is.EqualTo(@"On 28 July 2006 – 14 October 2008 was"));

            // no Matches
            genFixes.AssertNotChanged(@"Now ""in July of 2007"" a new");
            genFixes.AssertNotChanged(@"Now {{quote|in July of 2007}} a new");
            genFixes.AssertNotChanged(@"Now ""in July of 1707"" a new");
            genFixes.AssertNotChanged(@"Now a march of 2007 resulted");
            genFixes.AssertNotChanged(@"Now the June of 2007 was");
        }

        [Test]
        public void TestFixDateOrdinals()
        {
            // no ordinals on dates
            Assert.That(parser.FixDateOrdinalsAndOf(@"On 14th March elections were", "test"), Is.EqualTo(@"On 14 March elections were"));
            Assert.That(parser.FixDateOrdinalsAndOf(@"On 14th June elections were", "test"), Is.EqualTo(@"On 14 June elections were"));
            Assert.That(parser.FixDateOrdinalsAndOf(@"14th June elections were", "test"), Is.EqualTo(@"14 June elections were"));
            Assert.That(parser.FixDateOrdinalsAndOf(@"On March 14th elections were", "test"), Is.EqualTo(@"On March 14 elections were"));
            Assert.That(parser.FixDateOrdinalsAndOf(@"On June 21st elections were", "test"), Is.EqualTo(@"On June 21 elections were"));
            Assert.That(parser.FixDateOrdinalsAndOf(@"On June 21<sup>st</sup> elections were", "test"), Is.EqualTo(@"On June 21 elections were"));
            Assert.That(parser.FixDateOrdinalsAndOf(@"On June 21<sup> st </sup> elections were", "test"), Is.EqualTo(@"On June 21 elections were"));
            Assert.That(parser.FixDateOrdinalsAndOf(@"On June 21<SUP>st</SUP> elections were", "test"), Is.EqualTo(@"On June 21 elections were"));
            Assert.That(parser.FixDateOrdinalsAndOf(@"On 14th March 2008 elections were", "test"), Is.EqualTo(@"On 14 March 2008 elections were"));
            Assert.That(parser.FixDateOrdinalsAndOf(@"On 14th June 2008 elections were", "test"), Is.EqualTo(@"On 14 June 2008 elections were"));
            Assert.That(parser.FixDateOrdinalsAndOf(@"On 14th June, 2008 elections were", "test"), Is.EqualTo(@"On 14 June 2008 elections were"));
            Assert.That(parser.FixDateOrdinalsAndOf(@"On 14<sup>th</sup> June, 2008 elections were", "test"), Is.EqualTo(@"On 14 June 2008 elections were"));
            Assert.That(parser.FixDateOrdinalsAndOf(@"On March 14th, 2008 elections were", "test"), Is.EqualTo(@"On March 14, 2008 elections were"));
            Assert.That(parser.FixDateOrdinalsAndOf(@"On June   21st, 2008 elections were", "test"), Is.EqualTo(@"On June 21, 2008 elections were"));
            Assert.That(parser.FixDateOrdinalsAndOf(@"On June   21st 2008 elections were", "test"), Is.EqualTo(@"On June 21, 2008 elections were"));

            // date ranges
            Assert.That(parser.FixDateOrdinalsAndOf(@"On 14th-15th June elections were", "test"), Is.EqualTo(@"On 14-15 June elections were"));
            Assert.That(parser.FixDateOrdinalsAndOf(@"On 14th - 15th June elections were", "test"), Is.EqualTo(@"On 14 - 15 June elections were"));
            Assert.That(parser.FixDateOrdinalsAndOf(@"On 14th to 15th June elections were", "test"), Is.EqualTo(@"On 14 to 15 June elections were"));
            Assert.That(parser.FixDateOrdinalsAndOf(@"On June 14th to 15th elections were", "test"), Is.EqualTo(@"On June 14 to 15 elections were"));
            Assert.That(parser.FixDateOrdinalsAndOf(@"On 14th,15th June elections were", "test"), Is.EqualTo(@"On 14,15 June elections were"));
            Assert.That(parser.FixDateOrdinalsAndOf(@"On 3rd and 15th June elections were", "test"), Is.EqualTo(@"On 3 and 15 June elections were"));

            Assert.That(parser.FixDateOrdinalsAndOf(@"On March 12th, 13th, 14th, 2008 elections were", "test"), Is.EqualTo(@"On March 12, 13, 14, 2008 elections were"));

            // no Matches, particularly dates with 'the' before where fixing the ordinal may leave 'on the 11 May' which wouldn't read well
            genFixes.AssertNotChanged(@"On 14th march was");
            genFixes.AssertNotChanged(@"Now the 14th February was");
            genFixes.AssertNotChanged(@"Now the February 14th was");
            genFixes.AssertNotChanged(@"Observance of 5th November Act");
            genFixes.AssertNotChanged(@"'''6th October City''' is", "6th October City");
            genFixes.AssertNotChanged(@"<blockquote>On March 14th, 2008 elections were</blockquote>");
        }

        [Test]
        public void FixDateOrdinalsAndOfEnOnly()
        {
#if DEBUG
            const string c1 = @"On 14th March elections were";
            Variables.SetProjectLangCode("fr");
            Assert.That(parser.FixDateOrdinalsAndOf(c1, "test"), Is.EqualTo(c1));

            Variables.SetProjectLangCode("en");
            Assert.That(parser.FixDateOrdinalsAndOf(c1, "test"), Is.EqualTo(@"On 14 March elections were"));
#endif
        }

        [Test]
        public void TestFixDateDayOfMonth()
        {
            Assert.That(parser.FixDateOrdinalsAndOf(@"On 14th of February was", "test"), Is.EqualTo(@"On 14 February was"));
            Assert.That(parser.FixDateOrdinalsAndOf(@"On 4th of February was", "test"), Is.EqualTo(@"On 4 February was"));
            Assert.That(parser.FixDateOrdinalsAndOf(@"On 14th of  February was", "test"), Is.EqualTo(@"On 14 February was"));
            Assert.That(parser.FixDateOrdinalsAndOf(@"14th of February 2009 was", "test"), Is.EqualTo(@"14 February 2009 was"));
            Assert.That(parser.FixDateOrdinalsAndOf(@"24th of June 2009 was", "test"), Is.EqualTo(@"24 June 2009 was"));

            // no change
            genFixes.AssertNotChanged(@"On the 14th of February 2009 was");
            genFixes.AssertNotChanged(@"now foo [[File:9th of June street.jpg]] was");
            genFixes.AssertNotChanged(@"now foo [[File:9th of June street , City.JPG|Caption]] was");
            genFixes.AssertNotChanged(@"now foo [[File:9th of June street , City.JPG|On [[Main Article#Overview|9th of June]] Street]] was here");
            genFixes.AssertNotChanged(@"now foo [[File:9th of June street , Bacău.JPG|[[Main Article#Overview|9th of June]] Street]] was here");
            genFixes.AssertNotChanged(@"now foo <gallery>File:9th of June street.JPG</gallery> was here");
            genFixes.AssertNotChanged(@"now foo <gallery>File:9th of June street , B.JPG</gallery> was here");
            genFixes.AssertNotChanged(@"n<gallery>
File:9th of May_street, Bacău.jpg| Street
</gallery>");
        }

        [Test]
        public void DatesLeadingZeros()
        {
            // leading zeros
            Assert.That(parser.FixDateOrdinalsAndOf(@"On 03 June elections were", "test"), Is.EqualTo(@"On 3 June elections were"));
            Assert.That(parser.FixDateOrdinalsAndOf(@"On 07 June elections were", "test"), Is.EqualTo(@"On 7 June elections were"));
            Assert.That(parser.FixDateOrdinalsAndOf(@"On June 07, elections were", "test"), Is.EqualTo(@"On June 7, elections were"));
            Assert.That(parser.FixDateOrdinalsAndOf(@"On June 07 2008, elections were", "test"), Is.EqualTo(@"On June 7, 2008, elections were"));
            Assert.That(parser.FixDateOrdinalsAndOf(@"On 03rd June elections were", "test"), Is.EqualTo(@"On 3 June elections were"));

            // no Matches
            Assert.That(parser.FixDateOrdinalsAndOf(@"On 2 June 07, elections were", "test"), Is.EqualTo(@"On 2 June 07, elections were"));
            Assert.That(parser.FixDateOrdinalsAndOf(@"The 007 march was", "test"), Is.EqualTo(@"The 007 march was"));
            Assert.That(parser.FixDateOrdinalsAndOf(@"In June '08 there was", "test"), Is.EqualTo(@"In June '08 there was"));
            Assert.That(parser.FixDateOrdinalsAndOf(@"In June 2008 there was", "test"), Is.EqualTo(@"In June 2008 there was"));
            Assert.That(parser.FixDateOrdinalsAndOf(@"On 00 June elections were", "test"), Is.EqualTo(@"On 00 June elections were"));
            Assert.That(parser.FixDateOrdinalsAndOf(@"The 007 May was", "test"), Is.EqualTo(@"The 007 May was"));
            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#Overzealous_de-ordinaling
            Assert.That(parser.FixDateOrdinalsAndOf(@"On 27 June 2nd and 3rd Panzer Groups", "test"), Is.EqualTo(@"On 27 June 2nd and 3rd Panzer Groups"));
        }

        [Test]
        public void DeterminePredominantDateLocale()
        {
            string none = @"hello", American1 = @"On July 17, 2009 a", International1 = @"on 17 July 2009 a";
            string ISO = @"now {{use ymd dates}} here", American2 = @"now {{use mdy dates}} here", International2 = @"{{use dmy dates}}",
                ISOmajority = @"on July 11, 2004 and 2009-11-12 and 2009-11-12 a";

            Assert.That(Parsers.DeterminePredominantDateLocale(none), Is.EqualTo(Parsers.DateLocale.Undetermined));
            Assert.That(Parsers.DeterminePredominantDateLocale(none, true), Is.EqualTo(Parsers.DateLocale.Undetermined));
            Assert.That(Parsers.DeterminePredominantDateLocale(American1), Is.EqualTo(Parsers.DateLocale.American));
            Assert.That(Parsers.DeterminePredominantDateLocale(International1), Is.EqualTo(Parsers.DateLocale.International));
            Assert.That(Parsers.DeterminePredominantDateLocale(American2), Is.EqualTo(Parsers.DateLocale.American));
            Assert.That(Parsers.DeterminePredominantDateLocale(International2), Is.EqualTo(Parsers.DateLocale.International));
            Assert.That(Parsers.DeterminePredominantDateLocale(ISO), Is.EqualTo(Parsers.DateLocale.ISO));

            Assert.That(Parsers.DeterminePredominantDateLocale(ISOmajority, true), Is.EqualTo(Parsers.DateLocale.ISO));
            Assert.That(Parsers.DeterminePredominantDateLocale(ISOmajority, false), Is.EqualTo(Parsers.DateLocale.American));

            Assert.That(Parsers.DeterminePredominantDateLocale(American1 + International1 + @"and 2009-01-04 the", true), Is.EqualTo(Parsers.DateLocale.Undetermined), "undetermined if count of all dates equal");
            Assert.That(Parsers.DeterminePredominantDateLocale(American1 + International1 + International1 + @"and 2009-01-04 the", true), Is.EqualTo(Parsers.DateLocale.Undetermined), "undetermined if count of all dates too similar");

            const string bddf = @"{{birth date|df=y|1950|4|7}}", bdnone = @"{{birth date|1950|4|7}}", bdmf = @"{{birth date|mf=y|1950|4|7}}";
            Assert.That(Parsers.DeterminePredominantDateLocale(bddf), Is.EqualTo(Parsers.DateLocale.International), "uses df=y as fallback");
            Assert.That(Parsers.DeterminePredominantDateLocale(bdnone), Is.EqualTo(Parsers.DateLocale.Undetermined));
            Assert.That(Parsers.DeterminePredominantDateLocale(bdmf), Is.EqualTo(Parsers.DateLocale.American), "uses mf=y as fallback");
        }

        [Test]
        public void PredominantDates()
        {
            const string c1 = @"{{cite web|url=http://www.foo.com/bar | title=text | date=2010-04-11 }}", useDMY = @"{{use dmy dates}}",
            c2 = @"{{cite web|url=http://www.foo.com/bar | title=text | date=2010-04-11 | accessdate= 2010-04-11 }}";

            Assert.That(Parsers.PredominantDates(c1), Is.EqualTo(c1), "no change when no use xxx template");
            Assert.That(Parsers.PredominantDates(c1 + useDMY), Is.EqualTo(c1.Replace("2010-04-11", "11 April 2010") + useDMY), "converts date to predominant format");
            Assert.That(Parsers.PredominantDates(c2 + useDMY), Is.EqualTo(c2.Replace("2010-04-11", "11 April 2010") + useDMY), "converts multiple dates in same cite to predominant format");

            Assert.That(Parsers.PredominantDates(c1.Replace("2010-04-11", "11 April 2010") + useDMY), Is.EqualTo(c1.Replace("2010-04-11", "11 April 2010") + useDMY), "no change when already correct");
        }

        [Test]
        public void PredominantDatesEnOnly()
        {
#if DEBUG
            const string c1 = @"{{cite web|url=http://www.foo.com/bar | title=text | date=2010-04-11 }}", useDMY = @"{{use dmy dates}}";
            Variables.SetProjectLangCode("fr");
            Assert.That(Parsers.PredominantDates(c1 + useDMY), Is.EqualTo(c1 + useDMY), "no change on non-en wiki");

            Variables.SetProjectLangCode("en");
            Assert.That(Parsers.PredominantDates(c1 + useDMY), Is.EqualTo(c1.Replace("2010-04-11", "11 April 2010") + useDMY), "converts date to predominant format");
#endif
        }
    }

    [TestFixture]
    public class ImageTests : RequiresInitialization
    {
        [Test, Category("Incomplete")]
        public void BasicImprovements()
        {
            Assert.That(Parsers.FixImages("[[ file : foo.jpg|thumb|200px|Bar]]"),
                            Is.EqualTo("[[File:foo.jpg|thumb|200px|Bar]]"));

            Assert.That(Parsers.FixImages("[[ image : foo.jpg|thumb|200px|Bar]]"),
                            Is.EqualTo("[[Image:foo.jpg|thumb|200px|Bar]]"));

            // apostrophe handling
            Assert.That(Parsers.FixImages(@"[[Image:foo%27s.jpg|thumb|200px|Bar]]"), Is.EqualTo(@"[[Image:foo's.jpg|thumb|200px|Bar]]"));

            const string doubleApos = @"[[Image:foo%27%27s.jpg|thumb|200px|Bar]]";
            Assert.That(Parsers.FixImages(doubleApos), Is.EqualTo(doubleApos));

            // TODO: decide if such improvements really belong here
            // Assert.AreEqual("[[Media:foo]]",
            // Parsers.FixImages("[[ media : foo]]"));

            // Assert.AreEqual("[[:Media:foo]]",
            // Parsers.FixImages("[[ : media : foo]]"));

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_6#URL_underscore_regression
            Assert.That(Parsers.FixImages("[[File:foo|thumb]] # [http://a_b c] [[link]]"),
                            Is.EqualTo("[[File:foo|thumb]] # [http://a_b c] [[link]]"));

            Assert.That(Parsers.FixImages("[[Image:foo|thumb]] # [http://a_b c] [[link]]"),
                            Is.EqualTo("[[Image:foo|thumb]] # [http://a_b c] [[link]]"));

            // no changes should be made to this one
            const string Diamminesilver = @"[[Image:Diamminesilver(I)-3D-balls.png|thumb|right|200px|Ball-and-stick model of the diamminesilver(I) cation, [Ag(NH<sub>3</sub>)<sub>2</sub>]<sup>+</sup>]]";
            Assert.That(Parsers.FixImages(Diamminesilver), Is.EqualTo(Diamminesilver));

            const string a = @"[[Image:End CEST Transparent.png|thumb|left|120px|
alt=Diagram of a clock showing a transition from 3:00 to 2:00.|
When DST ends in central Europe, clocks retreat from 03:00 CEST to 02:00 CET. Other regions switch at different times.]]";

            Assert.That(Parsers.FixSyntax(a), Is.EqualTo(a));
        }

        [Test]
        public void Removal()
        {
            bool noChange;

            Assert.That(Parsers.RemoveImage("Foo.jpg", "[[Image:Foo.jpg]]", false, "", out noChange), Is.Empty);
            Assert.IsFalse(noChange);

            Assert.That(Parsers.RemoveImage("Foo.jpg", "[[:Image:Foo.jpg]]", false, "", out noChange), Is.Empty);
            Assert.IsFalse(noChange);

            Assert.That(Parsers.RemoveImage("foo.jpg", "[[Image: foo.jpg]]", false, "", out noChange), Is.Empty);
            Assert.IsFalse(noChange);

            Assert.That(Parsers.RemoveImage("Foo, bar", "[[File:foo%2C_bar|quux]]", false, "", out noChange), Is.Empty);
            Assert.IsFalse(noChange);

            Assert.That(Parsers.RemoveImage("Foo%2C_bar", "[[File:foo, bar|quux]]", false, "", out noChange), Is.Empty);
            Assert.IsFalse(noChange);

            Assert.That(Parsers.RemoveImage("foo.jpg", "[[Media:foo.jpg]]", false, "", out noChange), Is.Empty);
            Assert.IsFalse(noChange);

            Assert.That(Parsers.RemoveImage("foo.jpg", "[[:media : foo.jpg]]", false, "", out noChange), Is.Empty);
            Assert.IsFalse(noChange);

            Assert.That(Parsers.RemoveImage("foo", "{{infobox|image=foo}}", false, "", out noChange),
                            Is.EqualTo("{{infobox|image=}}"));
            Assert.IsFalse(noChange);

            Assert.That(Parsers.RemoveImage("foo", "{{infobox|image=foo|other=bar}}", false, "", out noChange),
                            Is.EqualTo("{{infobox|image=|other=bar}}"));
            Assert.IsFalse(noChange);

            Assert.That(Parsers.RemoveImage("Foo, bar", "[[File:foo%2C_bar|quux [[here]] there]]", false, "", out noChange), Is.Empty);
            Assert.IsFalse(noChange);

            Assert.That(Parsers.RemoveImage(@"Image:Bar.jpg", @"<gallery>
Image:foo.jpg
Image:Bar.jpg</gallery>", false, "", out noChange), Is.EqualTo(@"<gallery>
Image:foo.jpg
</gallery>"));

            Assert.That(Parsers.RemoveImage(@"Image:Bar.jpg", @"<gallery>
Image:foo.jpg
Image:Bar.jpg|Some text description
</gallery>", false, "", out noChange), Is.EqualTo(@"<gallery>
Image:foo.jpg

</gallery>"));

            Assert.That(Parsers.RemoveImage(@"Image:Bar.jpg", @"<gallery>
Image:foo.jpg
Image:Bar.jpg|Some text description
</gallery>", true, "x", out noChange), Is.EqualTo(@"<gallery>
Image:foo.jpg
<!-- x Image:Bar.jpg|Some text description -->
</gallery>"));

            Assert.That(Parsers.RemoveImage("FOO.jpg", "[[Media:foo.jpg]]", false, "", out noChange), Is.EqualTo("[[Media:foo.jpg]]"), "image name is case sensitive");
            Assert.IsTrue(noChange);

            Assert.That(Parsers.RemoveImage("", "[[Media:foo.jpg]]", false, "", out noChange), Is.EqualTo("[[Media:foo.jpg]]"), "no change when blank image name input");
            Assert.IsTrue(noChange);
        }

        [Test]
        public void RemovalMultipleLines()
        {
            bool noChange;

            Assert.That(Parsers.RemoveImage("Foo, bar", @"[[File:foo%2C_bar|quux [[here]]
there]]", false, "", out noChange), Is.Empty);
            Assert.IsFalse(noChange);

            Assert.That(Parsers.RemoveImage("Foo, bar", @"[[File:foo%2C_bar|quux [[here]] there]] [[now]]", false, "", out noChange), Is.EqualTo(" [[now]]"));
            Assert.IsFalse(noChange);
            Assert.That(Parsers.RemoveImage("Foo, bar", @"[[File:foo%2C_bar|quux there]] [[now]]", false, "", out noChange), Is.EqualTo(" [[now]]"));
            Assert.IsFalse(noChange);
        }

        [Test]
        public void Replacement()
        {
            bool noChange;

            // just in case...
            Assert.That(Parsers.ReplaceImage("", "", "", out noChange), Is.Empty);
            Assert.IsTrue(noChange);

            Assert.That(Parsers.ReplaceImage("foo", "bar", "[[File:Foo]]", out noChange), Is.EqualTo("[[File:bar]]"));
            Assert.IsFalse(noChange);

            // preserve namespace
            Assert.That(Parsers.ReplaceImage("foo", "bar", "[[image:Foo]]", out noChange), Is.EqualTo("[[Image:bar]]"));
            Assert.IsFalse(noChange);

            // pipes, non-canonical NS casing
            Assert.That(Parsers.ReplaceImage("Foo%2C_bar", "bar", "[[FIle:foo, bar|boz!|666px]]", out noChange),
                            Is.EqualTo("[[File:bar|boz!|666px]]"));
            Assert.IsFalse(noChange);

            Assert.That(Parsers.ReplaceImage("foo", "bar", "[[Media:foo]]", out noChange), Is.EqualTo("[[Media:bar]]"));
            Assert.IsFalse(noChange);

            // normalising Media: is not yet supported, see TODO in BasicImprovements()
            Assert.That(Parsers.ReplaceImage("foo", "bar", "[[:media : foo]]", out noChange), Is.EqualTo("[[:media:bar]]"));
            Assert.IsFalse(noChange);
        }
    }

    [TestFixture]
    // tests have to have long strings due to logic in BoldTitle looking at bolding in first 5% of article only
    public class BoldTitleTests : RequiresParser
    {
        bool noChangeBack;

        [Test]
        // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_1#Title_bolding
        public void DontEmboldenImagesAndTemplates()
        {
            Assert.IsFalse(parser.BoldTitle("[[Image:Foo.jpg]]", "Foo", out noChangeBack).Contains("'''Foo'''"));
            Assert.IsFalse(parser.BoldTitle("{{Foo}}", "Foo", out noChangeBack).Contains("'''Foo'''"));
            Assert.IsFalse(parser.BoldTitle("{{template| Foo is a bar}}", "Foo", out noChangeBack).Contains("'''Foo'''"));
        }

        [Test]
        public void DatesNotChanged()
        {
            Assert.That(parser.BoldTitle(@"May 31 is a great day", "May 31", out noChangeBack), Is.EqualTo(@"May 31 is a great day"));
            Assert.IsTrue(noChangeBack);

            Assert.That(parser.BoldTitle(@"March 1 is a great day", "March 1", out noChangeBack), Is.EqualTo(@"March 1 is a great day"));
            Assert.IsTrue(noChangeBack);

            Assert.That(parser.BoldTitle(@"31 May is a great day", "31 May", out noChangeBack), Is.EqualTo(@"31 May is a great day"));
            Assert.IsTrue(noChangeBack);
        }

        [Test]
        public void SimilarLinksWithDifferentCaseNotChanged()
        {
            Assert.That(parser.BoldTitle("Foo is this one, now [[FOO]] is another While remaining upright may be the primary goal of beginning riders", "Foo", out noChangeBack),
                            Is.EqualTo("'''Foo''' is this one, now [[FOO]] is another While remaining upright may be the primary goal of beginning riders"));
            Assert.IsFalse(noChangeBack);

            Assert.That(parser.BoldTitle("'''Foo''' is this one, now [[FOO]] is another While remaining upright may be the primary goal of beginning riders", "Foo", out noChangeBack),
                            Is.EqualTo("'''Foo''' is this one, now [[FOO]] is another While remaining upright may be the primary goal of beginning riders"));
            Assert.IsTrue(noChangeBack);
        }

        [Test]
        public void DontChangeIfAlreadyBold()
        {
            Assert.That(parser.BoldTitle("'''Foo''' is this one", "Foo", out noChangeBack), Is.EqualTo("'''Foo''' is this one"));
            Assert.IsTrue(noChangeBack);
            Assert.That(parser.BoldTitle("Foo is a bar, '''Foo''' moar", "Foo", out noChangeBack), Is.EqualTo("Foo is a bar, '''Foo''' moar"));
            Assert.IsTrue(noChangeBack);
            Assert.That(parser.BoldTitle(@"{{Infobox | name = Foo | age=11}} '''Foo''' is a bar", "Foo", out noChangeBack), Is.EqualTo(@"{{Infobox | name = Foo | age=11}} '''Foo''' is a bar"));
            Assert.IsTrue(noChangeBack);
            Assert.That(parser.BoldTitle(@"{{Infobox
| age=11}} '''John David Smith''' is a bar", "John Smith", out noChangeBack), Is.EqualTo(@"{{Infobox
| age=11}} '''John David Smith''' is a bar"));
            Assert.IsTrue(noChangeBack);

            // bold earlier in body of article
            Assert.That(parser.BoldTitle(@"{{Infobox| age=11}} '''John David Smith''' is a bar, John Smith", "John Smith", out noChangeBack), Is.EqualTo(@"{{Infobox| age=11}} '''John David Smith''' is a bar, John Smith"));
            Assert.IsTrue(noChangeBack);
            Assert.That(parser.BoldTitle(@"{{Infobox
| age=11}}{{box2}}} '''John David Smith''' is a bar", "John Smith", out noChangeBack), Is.EqualTo(@"{{Infobox
| age=11}}{{box2}}} '''John David Smith''' is a bar"));
            Assert.IsTrue(noChangeBack);

            Assert.That(parser.BoldTitle("'''Now''' Foo is a bar While remaining upright may be the primary goal of beginning riders", "Foo", out noChangeBack),
                            Is.EqualTo("'''Now''' Foo is a bar While remaining upright may be the primary goal of beginning riders"));
            Assert.IsTrue(noChangeBack);

            // won't change if italics either
            Assert.That(parser.BoldTitle("''Foo'' is this one", "Foo", out noChangeBack), Is.EqualTo("''Foo'' is this one"));
            Assert.IsTrue(noChangeBack);

            Assert.That(parser.BoldTitle(@"{{Infobox martial art| website      = }}
{{Nihongo|'''Aikido'''|???|aikido}} is a. Aikido was", "Aikido", out noChangeBack), Is.EqualTo(@"{{Infobox martial art| website      = }}
{{Nihongo|'''Aikido'''|???|aikido}} is a. Aikido was"));
            Assert.IsTrue(noChangeBack);

            Assert.That(parser.BoldTitle(@"{{Infobox martial art| name='''Aikido'''| website      = }}
Aikido was", "Aikido", out noChangeBack), Is.EqualTo(@"{{Infobox martial art| name='''Aikido'''| website      = }}
'''Aikido''' was"), "Bold text in infobox ignored");
            Assert.IsFalse(noChangeBack);

            // images then bold
            const string NoChangeImages = @"[[Image:098098889899899889089980890890.png|2390823424890243 23980324 234098234980]] [[Image:098098889899899889089980890890.png|2390823424890243 23980324 234098234980]]
            [[Image:098098889899899889089980890890.png|2390823424890243 23980324 234098234980]] [[Image:098098889899899889089980890890.png|2390823424890243 23980324 234098234980]]
'''A''' was. Foo One here";
            Assert.That(parser.BoldTitle(NoChangeImages, "Foo One", out noChangeBack), Is.EqualTo(NoChangeImages));
            Assert.IsTrue(noChangeBack);

            Assert.That(parser.BoldTitle("{{year article header}} Foo is a bar While remaining upright may be the primary goal of beginning riders", "Foo", out noChangeBack),
                            Is.EqualTo("{{year article header}} Foo is a bar While remaining upright may be the primary goal of beginning riders"));
            Assert.IsTrue(noChangeBack);

            Assert.That(parser.BoldTitle("{{bio}} Foo is a bar While remaining upright may be the primary goal of beginning riders", "Foo", out noChangeBack),
                Is.EqualTo("{{bio}} Foo is a bar While remaining upright may be the primary goal of beginning riders"));
            Assert.IsTrue(noChangeBack);

            Assert.That(parser.BoldTitle("<dfn>Foo</dfn> is this one", "Foo", out noChangeBack), Is.EqualTo("<dfn>Foo</dfn> is this one"));
            Assert.IsTrue(noChangeBack);
        }

        [Test]
        public void Exceptions()
        {
            bool noChange;
            string Section = @"<section>'''[[Foo]]''' bar</section>";
            Assert.That(parser.BoldTitle(Section, "Foo", out noChange), Is.EqualTo(Section), "No change when article text contains <section> tags");

            Section = @"{{year article header}} Foo bar.";
            Assert.That(parser.BoldTitle(Section, "Foo", out noChange), Is.EqualTo(Section), "No change when article text contains {{year article header}}");

            Section = @"{{bio}} Foo bar.";
            Assert.That(parser.BoldTitle(Section, "Foo", out noChange), Is.EqualTo(Section), "No change when article text contains {{bio}}");

            string x = @"While remaining upright may be the primary goal of beginning riders";
            Section = @"{{Terme défini|Foo}} is a bar. Foo while remaining upright may be the primary goal of beginning riders." + x + x + x + x + x + x;
            Assert.That(parser.BoldTitle(Section, "Foo", out noChange), Is.EqualTo(Section), "No change when article text contains {{Terme défini}}");
        }

        [Test]
        public void StandardCases()
        {
            Assert.That(parser.BoldTitle("Foo is a bar While remaining upright may be the primary goal of beginning riders", "Foo", out noChangeBack),
                            Is.EqualTo("'''Foo''' is a bar While remaining upright may be the primary goal of beginning riders"));
            Assert.IsFalse(noChangeBack);

            Assert.That(parser.BoldTitle("Foo (here) is a bar While remaining upright may be the primary goal of beginning riders", "Foo (here)", out noChangeBack),
                            Is.EqualTo("'''Foo''' (here) is a bar While remaining upright may be the primary goal of beginning riders"));
            Assert.IsFalse(noChangeBack);

            Assert.That(parser.BoldTitle("Foo.(here) is a bar While remaining upright may be the primary goal of beginning riders", "Foo.(here)", out noChangeBack),
                            Is.EqualTo("'''Foo.(here)''' is a bar While remaining upright may be the primary goal of beginning riders"));
            Assert.IsFalse(noChangeBack);

            // only first instance bolded
            Assert.That(parser.BoldTitle("Foo is a bar While remaining upright may be the primary goal of beginning riders Foo", "Foo", out noChangeBack),
                            Is.EqualTo("'''Foo''' is a bar While remaining upright may be the primary goal of beginning riders Foo"));
            Assert.IsFalse(noChangeBack);

            Assert.That(parser.BoldTitle(@"The Foo is a bar While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders", "Foo", out noChangeBack),
                            Is.EqualTo(@"The '''Foo''' is a bar While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders"));
            Assert.IsFalse(noChangeBack);

            Assert.That(parser.BoldTitle("Foo in the wild is a bar While remaining upright may be the primary goal of beginning riders While remaining upright may be the primary goal of beginning riders", "Foo in the wild", out noChangeBack),
                            Is.EqualTo("'''Foo in the wild''' is a bar While remaining upright may be the primary goal of beginning riders While remaining upright may be the primary goal of beginning riders"));
            Assert.IsFalse(noChangeBack);

            Assert.That(parser.BoldTitle("Foo is a bar, Foo moar While remaining upright may be the primary goal of beginning riders", "Foo", out noChangeBack),
                            Is.EqualTo("'''Foo''' is a bar, Foo moar While remaining upright may be the primary goal of beginning riders"));
            Assert.IsFalse(noChangeBack);

            Assert.That(parser.BoldTitle("F^o^o is a bar While remaining upright may be the primary goal of beginning riders", "F^o^o", out noChangeBack),
                            Is.EqualTo("'''F^o^o''' is a bar While remaining upright may be the primary goal of beginning riders"));
            Assert.IsFalse(noChangeBack);

            Assert.That(parser.BoldTitle(@"{{Infobox | name = Foo | age=11}}
Foo is a bar While remaining upright may be the primary goal of beginning riders While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders", "Foo", out noChangeBack),
                            Is.EqualTo(@"{{Infobox | name = Foo | age=11}}
'''Foo''' is a bar While remaining upright may be the primary goal of beginning riders While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders"));
            Assert.IsFalse(noChangeBack);

            // immediately after infobox – 5% rule does not apply
            Assert.That(parser.BoldTitle(@"{{Infobox abc| name = Foo | age=11}}
Foo is a bar", "Foo", out noChangeBack), Is.EqualTo(@"{{Infobox abc| name = Foo | age=11}}
'''Foo''' is a bar"));
            Assert.IsFalse(noChangeBack);

            // brackets excluded from bolding
            Assert.That(parser.BoldTitle("Foo (Band album) is a CD While remaining upright may be the primary goal of beginning riders", "Foo (Band album)", out noChangeBack),
                            Is.EqualTo("'''Foo''' (Band album) is a CD While remaining upright may be the primary goal of beginning riders"));
            Assert.IsFalse(noChangeBack);

            // non-changes
            Assert.That(parser.BoldTitle("Fooo is a bar", "Foo", out noChangeBack), Is.EqualTo("Fooo is a bar"));
            Assert.IsTrue(noChangeBack);
            const string ImageBold = @"[[Image...]]
'''Something''' is a bar";
            Assert.That(parser.BoldTitle(ImageBold, "Foo", out noChangeBack), Is.EqualTo(ImageBold));
            Assert.IsTrue(noChangeBack);

            Assert.That(parser.BoldTitle(@"Foo is a '''bar''' While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders", "Foo", out noChangeBack),
                            Is.EqualTo(@"Foo is a '''bar''' While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders")); // bold within first 5% of article
            Assert.IsTrue(noChangeBack);

            // no delinking when existing in bold, even if in template
            Assert.That(parser.BoldTitle(@"{{unicode|'''Ł̣'''}} ([[Lower case|minuscule]]: {{unicode|'''ł̣'''}}) is a letter of the [[Latin alphabet]], derived from [[Ł̣]] with a diacritical", @"Ł̣", out noChangeBack),
                            Is.EqualTo(@"{{unicode|'''Ł̣'''}} ([[Lower case|minuscule]]: {{unicode|'''ł̣'''}}) is a letter of the [[Latin alphabet]], derived from [[Ł̣]] with a diacritical"));
            Assert.IsTrue(noChangeBack);

            // image descriptions NOT bolded:
            Assert.That(parser.BoldTitle(@"[[Image:1.JPEG|Now Smith here]] Now Smith here While remaining upright may be the primary goal of beginning riders While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders", "Smith", out noChangeBack), Is.EqualTo(@"[[Image:1.JPEG|Now Smith here]] Now '''Smith''' here While remaining upright may be the primary goal of beginning riders While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders"));
            Assert.IsFalse(noChangeBack);

            Assert.That(parser.BoldTitle(@"{{unreferenced|date=November 2010}}
Vodafone Qatar started", "Vodafone Qatar", out noChangeBack), Is.EqualTo(@"{{unreferenced|date=November 2010}}
'''Vodafone Qatar''' started"));
        }

        [Test]
        public void WithDelinking()
        {
            Assert.That(parser.BoldTitle("[[Foo]] is a bar While remaining upright may be the primary goal of beginning riders", "Foo", out noChangeBack),
                            Is.EqualTo("'''Foo''' is a bar While remaining upright may be the primary goal of beginning riders"));
            Assert.IsFalse(noChangeBack);

            Assert.That(parser.BoldTitle("[[Foo]] is a bar, Foo moar While remaining upright may be the primary goal of beginning riders", "Foo", out noChangeBack),
                            Is.EqualTo("'''Foo''' is a bar, Foo moar While remaining upright may be the primary goal of beginning riders"));
            Assert.IsFalse(noChangeBack);

            Assert.That(parser.BoldTitle("Foo is a bar, now [[Foo]] here While remaining upright may be the primary goal of beginning riders", "Foo", out noChangeBack),
                            Is.EqualTo("Foo is a bar, now '''Foo''' here While remaining upright may be the primary goal of beginning riders"));
            Assert.IsFalse(noChangeBack);

            Assert.That(parser.BoldTitle("Foo is a bar, now [[foo]] here While remaining upright may be the primary goal of beginning riders, foo here", "Foo", out noChangeBack),
                            Is.EqualTo("Foo is a bar, now '''foo''' here While remaining upright may be the primary goal of beginning riders, foo here"));
            Assert.IsFalse(noChangeBack);
            Assert.That(parser.BoldTitle("[[Foo]] is a [[bar]] While remaining upright may be the primary goal of beginning riders", "Foo", out noChangeBack),
                            Is.EqualTo("'''Foo''' is a [[bar]] While remaining upright may be the primary goal of beginning riders"));
            Assert.IsFalse(noChangeBack);

            // no change
            Assert.That(parser.BoldTitle("'''Foo''' is a bar, now [[Foo]] here While remaining upright may be the primary goal of beginning riders", "Foo", out noChangeBack),
                            Is.EqualTo("'''Foo''' is a bar, now [[Foo]] here While remaining upright may be the primary goal of beginning riders"));
            Assert.IsTrue(noChangeBack);

            // limitation: can't hide image descriptions without hiding the self links too:
            Assert.That(parser.BoldTitle(@"[[Image:Head of serpent column.jpg|250px|thumb|[[Kukulkan]] at the base of the west face of the northern stairway of [[El Castillo, Chichen Itza]]]]

[[Image:ChichenItzaEquinox.jpg|250px|thumb|Kukulkan at Chichen Itza during the [[Equinox]]. The famous decent of the snake March 2009]]

[[Kukulkan]] (''Plumed Serpent'', ''Feathered Serpent'') is a god in the pantheon of [[Maya mythology]]. In [[Yucatec]] the name is spelt '''K'uk'ulkan''' and in [[Tzotzil language|Tzotzil]] it is '''K'uk'ul-chon'''.<ref>Freidel et al 1993, p.289.</ref> The depiction of the [[Feathered Serpent (deity)|feathered serpent deity]] is present in other cultures of [[Mesoamerica]] and Kukulkan is closely related to ''[[Gukumatz]]'' of the [[K'iche']] Maya tradition and ''[[Quetzalcoatl]]'' of [[Aztec mythology]].<ref>Read & Gonzalez 2000, pp.180-2.</ref> Little is known of the mythology of the [[pre-Columbian]] deity.<ref>Read & Gonzalez 2000, p.201.</ref>

Although heavily Mexicanised, Kukulkan has his origins among the Maya of the [[Mesoamerican chronology|Classic Period]], when he was known as ''Waxaklahun Ubah Kan'', the War Serpent, and he has been identified as the Postclassic version of the [[Vision Serpent]] of Classic Maya art.<ref>Freidel et al 1993, pp.289, 325, 441n26.</ref>

The cult of Kukulkan/Quetzalcoatl was the first Mesoamerican religion to transcend the old Classic Period linguistic and ethnic divisions.<ref>Sharer & Traxler 2006, pp582-3.</ref> This cult facilitated communication and peaceful trade among peoples of many different social and ethnic backgrounds.<ref>Sharer & Traxler 2006, pp582-3.</ref> Although the cult was originally centred on the ancient city of [[Chichen Itza|Chichén Itzá]] in the modern [[Mexico|Mexican]] state of [[Yucatán]], it spread as far as the [[Guatemala]]n highlands.<ref>Sharer & Traxler 2006, p.619.</ref>

In Yucatán, references to the deity Kukulkan are confused by references to a named individual who bore the name of the god. Because of this, the distinction between the two has become blurred.<ref>Miller & Taube 1993, p.142.</ref> This individual appears to have been a ruler or priest at Chichen Itza, who first appeared around the 10th century.<ref>Read & González 2000, p.201.</ref>
Although Kukulkan was mentioned as a historical person by Maya writers of the 16th century, the earlier 9th century texts at Chichen Itza never identified him as human and artistic representations depicted him as a Vision Serpent entwined around the figures of nobles.<ref>Freidel et al 1993, p.325.</ref> At Chichen Itza, Kukulkan is also depicted presiding over sacrifice scenes.<ref>Freidel et al 1993, p.478n60.</ref>

Sizeable temples to Kukulkan are found at archaeological sites throughout the north of the [[Yucatán Peninsula]], such as Chichen Itza, [[Uxmal]] and [[Mayapan]].<ref>Read & González 2000, p.201.</ref>

==Etymology==
The Yucatec form of the name is formed from the word", "Kukulkan", out noChangeBack), Is.EqualTo(@"[[Image:Head of serpent column.jpg|250px|thumb|'''Kukulkan''' at the base of the west face of the northern stairway of [[El Castillo, Chichen Itza]]]]

[[Image:ChichenItzaEquinox.jpg|250px|thumb|Kukulkan at Chichen Itza during the [[Equinox]]. The famous decent of the snake March 2009]]

'''Kukulkan''' (''Plumed Serpent'', ''Feathered Serpent'') is a god in the pantheon of [[Maya mythology]]. In [[Yucatec]] the name is spelt '''K'uk'ulkan''' and in [[Tzotzil language|Tzotzil]] it is '''K'uk'ul-chon'''.<ref>Freidel et al 1993, p.289.</ref> The depiction of the [[Feathered Serpent (deity)|feathered serpent deity]] is present in other cultures of [[Mesoamerica]] and Kukulkan is closely related to ''[[Gukumatz]]'' of the [[K'iche']] Maya tradition and ''[[Quetzalcoatl]]'' of [[Aztec mythology]].<ref>Read & Gonzalez 2000, pp.180-2.</ref> Little is known of the mythology of the [[pre-Columbian]] deity.<ref>Read & Gonzalez 2000, p.201.</ref>

Although heavily Mexicanised, Kukulkan has his origins among the Maya of the [[Mesoamerican chronology|Classic Period]], when he was known as ''Waxaklahun Ubah Kan'', the War Serpent, and he has been identified as the Postclassic version of the [[Vision Serpent]] of Classic Maya art.<ref>Freidel et al 1993, pp.289, 325, 441n26.</ref>

The cult of Kukulkan/Quetzalcoatl was the first Mesoamerican religion to transcend the old Classic Period linguistic and ethnic divisions.<ref>Sharer & Traxler 2006, pp582-3.</ref> This cult facilitated communication and peaceful trade among peoples of many different social and ethnic backgrounds.<ref>Sharer & Traxler 2006, pp582-3.</ref> Although the cult was originally centred on the ancient city of [[Chichen Itza|Chichén Itzá]] in the modern [[Mexico|Mexican]] state of [[Yucatán]], it spread as far as the [[Guatemala]]n highlands.<ref>Sharer & Traxler 2006, p.619.</ref>

In Yucatán, references to the deity Kukulkan are confused by references to a named individual who bore the name of the god. Because of this, the distinction between the two has become blurred.<ref>Miller & Taube 1993, p.142.</ref> This individual appears to have been a ruler or priest at Chichen Itza, who first appeared around the 10th century.<ref>Read & González 2000, p.201.</ref>
Although Kukulkan was mentioned as a historical person by Maya writers of the 16th century, the earlier 9th century texts at Chichen Itza never identified him as human and artistic representations depicted him as a Vision Serpent entwined around the figures of nobles.<ref>Freidel et al 1993, p.325.</ref> At Chichen Itza, Kukulkan is also depicted presiding over sacrifice scenes.<ref>Freidel et al 1993, p.478n60.</ref>

Sizeable temples to Kukulkan are found at archaeological sites throughout the north of the [[Yucatán Peninsula]], such as Chichen Itza, [[Uxmal]] and [[Mayapan]].<ref>Read & González 2000, p.201.</ref>

==Etymology==
The Yucatec form of the name is formed from the word"));
        }

        [Test]
        // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_9#Bold_letters
        public void ExamplesFromBugReport()
        {
            Assert.That(parser.BoldTitle(@"[[Michael Bavaro]] is a [[filmmaker]] based in [[Manhattan]]. While remaining upright may be the primary goal of beginning riders, a bike must lean in order to maintain balance", "Michael Bavaro", out noChangeBack),
                            Is.EqualTo(@"'''Michael Bavaro''' is a [[filmmaker]] based in [[Manhattan]]. While remaining upright may be the primary goal of beginning riders, a bike must lean in order to maintain balance"));
            Assert.IsFalse(noChangeBack);

            Assert.That(parser.BoldTitle(@"{{Unreferenced|date=October 2007}}
Steve Cook is a songwriter for Sovereign Grace. While remaining upright may be the primary goal of beginning riders. While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders While remaining upright may be the primary goal of beginning riders", "Steve Cook", out noChangeBack),
                            Is.EqualTo(@"{{Unreferenced|date=October 2007}}
'''Steve Cook''' is a songwriter for Sovereign Grace. While remaining upright may be the primary goal of beginning riders. While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders While remaining upright may be the primary goal of beginning riders"));
            Assert.IsFalse(noChangeBack);

            // boldtitle delinks all self links in lead section
            Assert.That(parser.BoldTitle(@"{{Unreferenced|date=October 2007}}
[[Steve Cook]] is a songwriter for Sovereign Grace. While remaining upright may be the primary goal of beginning riders. While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders While remaining upright may be the primary goal of beginning riders [[Steve Cook]]
While remaining upright may be the primary goal of beginning riders While remaining upright may be the primary goal of beginning riders", "Steve Cook", out noChangeBack),
                            Is.EqualTo(@"{{Unreferenced|date=October 2007}}
'''Steve Cook''' is a songwriter for Sovereign Grace. While remaining upright may be the primary goal of beginning riders. While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders While remaining upright may be the primary goal of beginning riders '''Steve Cook'''
While remaining upright may be the primary goal of beginning riders While remaining upright may be the primary goal of beginning riders"));
            Assert.IsFalse(noChangeBack);

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_10#Piped_self-link_delinking_bug
            Assert.That(Parsers.FixLinks(parser.BoldTitle(@"The 2009 Indian Premier League While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders
==sec== [[2009 Indian Premier League|2009]]<br>", "2009 Indian Premier League", out noChangeBack), "2009 Indian Premier League", out noChangeBack), Is.EqualTo(@"The '''2009 Indian Premier League''' While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders
==sec== 2009<br>"));
            
            const string OutsideZerothSection = @"Text here.
==Bio==
John Smith was great.";

            Assert.That(parser.BoldTitle(OutsideZerothSection, "John Smith", out noChangeBack), Is.EqualTo(OutsideZerothSection));
        }

        [Test]
        // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#If_a_selflink_is_also_bolded.2C_AWB_should_just_remove_the_selflink
        public void SelfLinksWithBold()
        {
            Assert.That(parser.BoldTitle(@"'''[[Marie-Madeleine-Marguerite d'Aubray, Marquise de Brinvilliers]]'''", @"Marie-Madeleine-Marguerite d'Aubray, Marquise de Brinvilliers", out noChangeBack), Is.EqualTo(@"'''Marie-Madeleine-Marguerite d'Aubray, Marquise de Brinvilliers'''"));

            Assert.That(parser.BoldTitle(@"'''[[foo]]'''", @"Foo", out noChangeBack), Is.EqualTo(@"'''foo'''"));

            // don't remove italics
            Assert.That(parser.BoldTitle(@"''[[Marie-Madeleine-Marguerite d'Aubray, Marquise de Brinvilliers]]''", @"Marie-Madeleine-Marguerite d'Aubray, Marquise de Brinvilliers", out noChangeBack), Is.EqualTo(@"'''''Marie-Madeleine-Marguerite d'Aubray, Marquise de Brinvilliers'''''"));
        }

        [Test]
        public void BoldedSelfLinks()
        {
            Assert.That(parser.BoldTitle(@"{{Infobox Album <!-- See Wikipedia:WikiProject_Albums -->
| Name        = ''It Crawled into My Hand, Honest''
| Type        = studio
| Artist      = [[The Fugs]]
| Cover       =
| Released    = 1968
| Recorded    = 1968
| Genre       = [[Rock and roll|Rock]], [[protopunk]], [[Psychedelic music|psychedelic]] [[Folk-rock]]
| Length      =
| Label       = [[Folkways Records|Folkways]]<br />[[ESP-Disk]]
| Producer    = [[Ed Sanders]], [[Harry Everett Smith|Harry Smith]]
| Reviews     =
| Last album  = ''[[Tenderness Junction]]''<br />(1968)
| This album  = '''''[[It Crawled into My Hand, Honest]]'''''<br />(1968)
| Next album  = ''[[Belle of Avenue A]]''<br /> (1969)
}}", "It Crawled into My Hand, Honest", out noChangeBack), Is.EqualTo(@"{{Infobox Album <!-- See Wikipedia:WikiProject_Albums -->
| Name        = ''It Crawled into My Hand, Honest''
| Type        = studio
| Artist      = [[The Fugs]]
| Cover       =
| Released    = 1968
| Recorded    = 1968
| Genre       = [[Rock and roll|Rock]], [[protopunk]], [[Psychedelic music|psychedelic]] [[Folk-rock]]
| Length      =
| Label       = [[Folkways Records|Folkways]]<br />[[ESP-Disk]]
| Producer    = [[Ed Sanders]], [[Harry Everett Smith|Harry Smith]]
| Reviews     =
| Last album  = ''[[Tenderness Junction]]''<br />(1968)
| This album  = '''''It Crawled into My Hand, Honest'''''<br />(1968)
| Next album  = ''[[Belle of Avenue A]]''<br /> (1969)
}}"));

            Assert.That(parser.BoldTitle(@"<noinclude> = '''''[[It Crawled into My Hand, Honest]]'''''<br /> </noinlcude>", "[[It Crawled into My Hand, Honest]]", out noChangeBack), Is.EqualTo(@"<noinclude> = '''''[[It Crawled into My Hand, Honest]]'''''<br /> </noinlcude>"));

            Assert.That(parser.BoldTitle(@"[[foo]]", @"Foo", out noChangeBack), Is.EqualTo(@"'''foo'''"));
            Assert.That(parser.BoldTitle(@"'''[[foo (here)|Foo]]'''", @"foo (here)", out noChangeBack), Is.EqualTo(@"'''Foo'''"));
            string NoChange = @"{{abc|The [[foo]] was}} A.";
            Assert.That(parser.BoldTitle(NoChange, @"Foo", out noChangeBack), Is.EqualTo(NoChange));

            NoChange = @"<noinclude>'''[[foo (here)|Foo]]'''</noinclude> A.";
            Assert.That(parser.BoldTitle(NoChange, @"Foo", out noChangeBack), Is.EqualTo(NoChange), "no change when noinclude/includeonly");
        }
    }

    [TestFixture]
    public class FixMainArticleTests : RequiresInitialization
    {
        [Test]
        public void BasicBehaviour()
        {
            Assert.That(Parsers.FixMainArticle(@"A==Sec==
Main article: [[Foo]]"), Is.EqualTo(@"A==Sec==
{{Main|Foo}}"));
            Assert.That(Parsers.FixMainArticle(@"A==Sec==
Main article: [[Foo]]."), Is.EqualTo(@"A==Sec==
{{Main|Foo}}"));
            Assert.That(Parsers.FixMainArticle(@"A==Sec==
Main article:\r\n [[Foo]]"), Is.EqualTo(@"A==Sec==
Main article:\r\n [[Foo]]"));
        }

        [Test]
        // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_3#Fixing_Main_Article_to_.7B.7Bmain.7D.7D
        public void PipedLinks()
        {
            Assert.That(Parsers.FixMainArticle(@"A==Sec==
Main article: [[Foo|Bar]]"), Is.EqualTo(@"A==Sec==
{{Main|Foo|l1=Bar}}"));
        }

        [Test]
        public void SupportIndenting()
        {
            Assert.That(Parsers.FixMainArticle(@"A==Sec==
:Main article: [[Foo]]"), Is.EqualTo(@"A==Sec==
{{Main|Foo}}"));
            Assert.That(Parsers.FixMainArticle(@"A==Sec==
:Main article: [[Foo]]."), Is.EqualTo(@"A==Sec==
{{Main|Foo}}"));
            Assert.That(Parsers.FixMainArticle(@"A==Sec==
:''Main article: [[Foo]]''"), Is.EqualTo(@"A==Sec==
{{Main|Foo}}"));
            Assert.That(Parsers.FixMainArticle(@"A==Sec==
'':Main article: [[Foo]]''"), Is.EqualTo(@"A==Sec==
'':Main article: [[Foo]]''"));
        }

        [Test]
        public void SupportBoldAndItalic()
        {
            Assert.That(Parsers.FixMainArticle(@"A==Sec==
Main article: '[[Foo]]'"), Is.EqualTo(@"A==Sec==
{{Main|Foo}}"));
            Assert.That(Parsers.FixMainArticle(@"A==Sec==
Main article: ''[[Foo]]''"), Is.EqualTo(@"A==Sec==
{{Main|Foo}}"));
            Assert.That(Parsers.FixMainArticle(@"A==Sec==
Main article: '''[[Foo]]'''"), Is.EqualTo(@"A==Sec==
{{Main|Foo}}"));
            Assert.That(Parsers.FixMainArticle(@"A==Sec==
Main article: '''''[[Foo]]'''''"), Is.EqualTo(@"A==Sec==
{{Main|Foo}}"));

            Assert.That(Parsers.FixMainArticle(@"A==Sec==
'Main article: [[Foo]]'"), Is.EqualTo(@"A==Sec==
{{Main|Foo}}"));
            Assert.That(Parsers.FixMainArticle(@"A==Sec==
''Main article: [[Foo]]''"), Is.EqualTo(@"A==Sec==
{{Main|Foo}}"));
            Assert.That(Parsers.FixMainArticle(@"A==Sec==
'''Main article: [[Foo]]'''"), Is.EqualTo(@"A==Sec==
{{Main|Foo}}"));
            Assert.That(Parsers.FixMainArticle(@"A==Sec==
'''''Main article: [[Foo]]'''''"), Is.EqualTo(@"A==Sec==
{{Main|Foo}}"));

            Assert.That(Parsers.FixMainArticle(@"A==Sec==
''Main article: '''[[Foo]]'''''"), Is.EqualTo(@"A==Sec==
{{Main|Foo}}"));
            Assert.That(Parsers.FixMainArticle(@"A==Sec==
'''Main article: ''[[Foo]]'''''"), Is.EqualTo(@"A==Sec==
{{Main|Foo}}"));
        }

        [Test]
        public void CaseInsensitivity()
        {
            Assert.That(Parsers.FixMainArticle(@"A==Sec==
main Article: [[foo]]"), Is.EqualTo(@"A==Sec==
{{Main|foo}}"));
        }

        [Test]
        // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_4#Problem_with_reverse_subst_of_.7B.7Bmain.7D.7D
        public void DontEatTooMuch()
        {
            Assert.That(Parsers.FixMainArticle(@"A==Sec==
Foo is a bar, see main article: [[Foo]]"), Is.EqualTo(@"A==Sec==
Foo is a bar, see main article: [[Foo]]"));
            Assert.That(Parsers.FixMainArticle(@"A==Sec==
Main article: [[Foo]], bar"), Is.EqualTo(@"A==Sec==
Main article: [[Foo]], bar"));
        }

        [Test]
        // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_6#Main_and_See_also_templates
        public void SingleLinkOnly()
        {
            Assert.That(Parsers.FixMainArticle(@"A==Sec==
:Main article: [[Foo]] and [[Bar]]"), Is.EqualTo(@"A==Sec==
:Main article: [[Foo]] and [[Bar]]"));
            Assert.That(Parsers.FixMainArticle(@"A==Sec==
:Main article: [[Foo|f00]] and [[Bar]]"), Is.EqualTo(@"A==Sec==
:Main article: [[Foo|f00]] and [[Bar]]"));
        }

        [Test]
        // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_7#Problem_with_.22Main_article.22_fixup
        public void Newlines()
        {
            Assert.That(Parsers.FixMainArticle("test\r\nMain article: [[Foo]]\r\ntest"), Is.EqualTo("test\r\n{{Main|Foo}}\r\ntest"));
            Assert.That(Parsers.FixMainArticle("test\r\n\r\nMain article: [[Foo]]\r\n\r\ntest"), Is.EqualTo("test\r\n\r\n{{Main|Foo}}\r\n\r\ntest"));
        }

        [Test]
        // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_3#Fixing_Main_Article_to_.7B.7Bmain.7D.7D
        public void SeeAlso()
        {
            Assert.That(Parsers.FixMainArticle(@"A==Sec==
See also: [[Foo|Bar]]"), Is.EqualTo(@"A==Sec==
{{See also|Foo|l1=Bar}}"));
            Assert.That(Parsers.FixMainArticle(@"A==Sec==
See also: [[Foo]]"), Is.EqualTo(@"A==Sec==
{{See also|Foo}}"));
        }

        [Test]
        public void SelfLinkRemoval()
        {
            bool noChangeBack;
            Assert.That(Parsers.FixLinks(@"'''Foo''' is great. [[Foo]] is cool", "Foo", out noChangeBack), Is.EqualTo(@"'''Foo''' is great. Foo is cool"));
            Assert.IsFalse(noChangeBack);
            Assert.That(Parsers.FixLinks(@"'''Foo''' is great. [[Foo]] is cool and [[foo]] now", "Foo", out noChangeBack), Is.EqualTo(@"'''Foo''' is great. Foo is cool and foo now"));
            Assert.IsFalse(noChangeBack);
            Assert.That(Parsers.FixLinks(@"'''Foo''' is great. [[Foo]] is cool and [[foo|bar]] now", "Foo", out noChangeBack), Is.EqualTo(@"'''Foo''' is great. Foo is cool and bar now"));
            Assert.IsFalse(noChangeBack);
            Assert.That(Parsers.FixLinks(@"'''Foo''' is great. [[Foo|Bar]] is cool", "Foo", out noChangeBack), Is.EqualTo(@"'''Foo''' is great. Bar is cool"));
            Assert.IsFalse(noChangeBack);
            Assert.That(Parsers.FixLinks(@"'''Foo bar''' is great. [[Foo bar]] is cool", "Foo bar", out noChangeBack), Is.EqualTo(@"'''Foo bar''' is great. Foo bar is cool"));
            Assert.IsFalse(noChangeBack);
            Assert.That(Parsers.FixLinks(@"'''Foo bar''' is great. [[Foo_bar]] is cool", "Foo bar", out noChangeBack), Is.EqualTo(@"'''Foo bar''' is great. Foo bar is cool"));
            Assert.IsFalse(noChangeBack);
            Assert.That(Parsers.FixLinks(@"'''Foo''' is great. Foo is cool.{{cite web|url=a|title=b|publisher=[[Foo]]}}", "Foo", out noChangeBack), Is.EqualTo(@"'''Foo''' is great. Foo is cool.{{cite web|url=a|title=b|publisher=Foo}}"));
            Assert.IsFalse(noChangeBack);

            // no support for delinking self section links
            Assert.That(Parsers.FixLinks(@"'''Foo''' is great. [[Foo#Bar|Bar]] is cool", "Foo", out noChangeBack), Is.EqualTo(@"'''Foo''' is great. [[Foo#Bar|Bar]] is cool"));
            Assert.IsTrue(noChangeBack);

            Assert.That(Parsers.FixLinks(@"'''the''' extreme [[anti-cult movement|anti-cult activist]]s resort", "Anti-cult movement", out noChangeBack), Is.EqualTo(@"'''the''' extreme anti-cult activists resort"));
            Assert.IsFalse(noChangeBack);
            Assert.That(Parsers.FixLinks(@"'''the''' extreme [[Anti-cult movement|anti-cult activist]]s resort", "Anti-cult movement", out noChangeBack), Is.EqualTo(@"'''the''' extreme anti-cult activists resort"));
            Assert.IsFalse(noChangeBack);

            // don't apply within imagemaps
            Assert.That(Parsers.FixLinks(@"<imagemap> [[foo]] </imagemap>", "foo", out noChangeBack), Is.EqualTo(@"<imagemap> [[foo]] </imagemap>"));
            Assert.IsTrue(noChangeBack);

            // don't apply within {{taxobox color}}
            Assert.That(Parsers.FixLinks(@"{{taxobox color| [[foo]] }}", "foo", out noChangeBack), Is.EqualTo(@"{{taxobox color| [[foo]] }}"));
            Assert.IsTrue(noChangeBack);

            // don't apply if has a noinclude etc.
            Assert.That(Parsers.FixLinks(@"<noinclude> [[foo]] </noinclude>", "foo", out noChangeBack), Is.EqualTo(@"<noinclude> [[foo]] </noinclude>"));
            Assert.IsTrue(noChangeBack);

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_13#Incorrect_delinking_of_article_title
            Assert.That(Parsers.FixLinks("[[foo bar]]", "Foo bar", out noChangeBack), Is.EqualTo("foo bar"));
            Assert.That(Parsers.FixLinks("[[Foo bar]]", "foo bar", out noChangeBack), Is.EqualTo("Foo bar"));
        }

        [Test]
        public void MoreSelfLinkRemoval()
        {
            bool noChangeBack;

            Assert.That(Parsers.FixLinks("[[Þorsteins saga Víkingssonar| Thorsten]]", "Þorsteins saga Víkingssonar", out noChangeBack), Is.EqualTo("Thorsten"));

            // convert to bold in infobox album/single
            Assert.That(Parsers.FixLinks(@"{{Infobox Single
| Name = Feels So Right
| Cover =
| Artist =  [[Alabama (band)|Alabama]]
| from Album = [[Feels So Right (album)|Feels So Right]]
| A-side         = ""Feels So Right""
| B-side         = ""See the Embers, Feel the Flame""
| Released = May 1, 1981 <small>([[United States|U.S.]])</small>
| Format         = [[7-inch single|7""]]
| Recorded       = August 26, 1980
| Genre = [[Country music|Country]]
| Length         = 3:36
| Label          = [[RCA Records]] <small>12236</small>
| Writer = [[Randy Owen]]
| Producer       = [[Harold Shedd]] and [[Alabama (band)|Alabama]]
| Last single = ""[[Old Flame (song)|Old Flame]]""<br />(1981)
| This single = ""[[Feels So Right (song)|Feels So Right]]"" <br />(1981)
| Next single = ""[[Love in the First Degree (Alabama song)|Love in the First Degree]]"" <br />(1981)
}}", @"Feels So Right (song)", out noChangeBack), Is.EqualTo(@"{{Infobox Single
| Name = Feels So Right
| Cover =
| Artist =  [[Alabama (band)|Alabama]]
| from Album = [[Feels So Right (album)|Feels So Right]]
| A-side         = ""Feels So Right""
| B-side         = ""See the Embers, Feel the Flame""
| Released = May 1, 1981 <small>([[United States|U.S.]])</small>
| Format         = [[7-inch single|7""]]
| Recorded       = August 26, 1980
| Genre = [[Country music|Country]]
| Length         = 3:36
| Label          = [[RCA Records]] <small>12236</small>
| Writer = [[Randy Owen]]
| Producer       = [[Harold Shedd]] and [[Alabama (band)|Alabama]]
| Last single = ""[[Old Flame (song)|Old Flame]]""<br />(1981)
| This single = ""'''Feels So Right'''"" <br />(1981)
| Next single = ""[[Love in the First Degree (Alabama song)|Love in the First Degree]]"" <br />(1981)
}}"));

            Assert.That(Parsers.FixLinks(@"{{Infobox Single
| Name = Feels So Right
| Cover =
| Artist =  [[Alabama (band)|Alabama]]
| from Album = [[Feels So Right (album)|Feels So Right]]
| A-side         = ""Feels So Right""
| B-side         = ""See the Embers, Feel the Flame""
| Released = May 1, 1981 <small>([[United States|U.S.]])</small>
| Format         = [[7-inch single|7""]]
| Recorded       = August 26, 1980
| Genre = [[Country music|Country]]
| Length         = 3:36
| Label          = [[RCA Records]] <small>12236</small>
| Writer = [[Randy Owen]]
| Producer       = [[Harold Shedd]] and [[Alabama (band)|Alabama]]
| Last single = ""[[Old Flame (song)|Old Flame]]""<br />(1981)
| This single = ""[[Feels So Right]]"" <br />(1981)
| Next single = ""[[Love in the First Degree (Alabama song)|Love in the First Degree]]"" <br />(1981)
}}", @"Feels So Right", out noChangeBack), Is.EqualTo(@"{{Infobox Single
| Name = Feels So Right
| Cover =
| Artist =  [[Alabama (band)|Alabama]]
| from Album = [[Feels So Right (album)|Feels So Right]]
| A-side         = ""Feels So Right""
| B-side         = ""See the Embers, Feel the Flame""
| Released = May 1, 1981 <small>([[United States|U.S.]])</small>
| Format         = [[7-inch single|7""]]
| Recorded       = August 26, 1980
| Genre = [[Country music|Country]]
| Length         = 3:36
| Label          = [[RCA Records]] <small>12236</small>
| Writer = [[Randy Owen]]
| Producer       = [[Harold Shedd]] and [[Alabama (band)|Alabama]]
| Last single = ""[[Old Flame (song)|Old Flame]]""<br />(1981)
| This single = ""'''Feels So Right'''"" <br />(1981)
| Next single = ""[[Love in the First Degree (Alabama song)|Love in the First Degree]]"" <br />(1981)
}}"));

            Assert.That(Parsers.FixLinks(@"{{Infobox Single
| Name = Feels So Right
| Last single = ""[[Old Flame (song)|Old Flame]]""<br />(1981)
| This single = '''''[[Feels So Right]]''''' <br />(1981)
| Next single = ""[[Love in the First Degree (Alabama song)|Love in the First Degree]]"" <br />(1981)
}}", @"Feels So Right", out noChangeBack), Is.EqualTo(@"{{Infobox Single
| Name = Feels So Right
| Last single = ""[[Old Flame (song)|Old Flame]]""<br />(1981)
| This single = '''''Feels So Right''''' <br />(1981)
| Next single = ""[[Love in the First Degree (Alabama song)|Love in the First Degree]]"" <br />(1981)
}}"));

            Assert.That(Parsers.FixLinks(@"{{Infobox Album  <!-- See Wikipedia:WikiProject_Albums -->
|Name        = Feels So Right
|Type        = [[Album]]
|Artist      = [[Alabama (band)|Alabama]]
|Cover       = Alabama - Feels So Right.jpg
|Released    = [[1981 in country music|1981]]</br>[[July 7]], [[1987]] (re-release)
|Recorded    = | [[1980 in country music|1980]]
|Genre       = [[Country music|Country]]
|Length      = 34:59
|Label       = [[RCA Records]]
|Producer    = Larry McBride, [[Harold Shedd]], Alabama
|Reviews     = *[[Allmusic]] {{Rating|4|5}} [http://allmusic.com/cg/amg.dll?p=amg&token=&sql=10:kjfixql5ldae]
|Chronology = [[Alabama (band)|Alabama]]
|Last album  = ''[[My Home's in Alabama (album)|My Home's In Alabama]]''<br />(1980)
|This album  = ''[[Feels So Right (album)|Feels So Right]]''<br />(1981)
|Next album  = ''[[Mountain Music (album)|Mountain Music]]''<br />(1982)
}}", @"Feels So Right (album)", out noChangeBack), Is.EqualTo(@"{{Infobox Album  <!-- See Wikipedia:WikiProject_Albums -->
|Name        = Feels So Right
|Type        = [[Album]]
|Artist      = [[Alabama (band)|Alabama]]
|Cover       = Alabama - Feels So Right.jpg
|Released    = [[1981 in country music|1981]]</br>[[July 7]], [[1987]] (re-release)
|Recorded    = | [[1980 in country music|1980]]
|Genre       = [[Country music|Country]]
|Length      = 34:59
|Label       = [[RCA Records]]
|Producer    = Larry McBride, [[Harold Shedd]], Alabama
|Reviews     = *[[Allmusic]] {{Rating|4|5}} [http://allmusic.com/cg/amg.dll?p=amg&token=&sql=10:kjfixql5ldae]
|Chronology = [[Alabama (band)|Alabama]]
|Last album  = ''[[My Home's in Alabama (album)|My Home's In Alabama]]''<br />(1980)
|This album  = '''''Feels So Right'''''<br />(1981)
|Next album  = ''[[Mountain Music (album)|Mountain Music]]''<br />(1982)
}}"));
        }

        [Test]
        public void BareReferencesTests()
        {
            Assert.IsTrue(Parsers.HasBareReferences(@"
foo
==References==
* http://www.site.com
"));

            Assert.IsTrue(Parsers.HasBareReferences(@"
foo
==References==
* http://www.site.com
==External links==
* http://www.site2.com
"));

            Assert.IsTrue(Parsers.HasBareReferences(@"
foo
==References==
* [http://www.site.com/site a site]
* http://www.site.com
==External links==
* http://www.site2.com
"));

            Assert.IsTrue(Parsers.HasBareReferences(@"
foo
==References==
* http://www.site.com
* [http://www.site.com/site a site]
==External links==
* http://www.site2.com
"));

            Assert.IsTrue(Parsers.HasBareReferences(@"
foo
==External links==
* http://www.site2.com
==References==
* http://www.site.com
* [http://www.site.com/site a site]
"));

            Assert.IsFalse(Parsers.HasBareReferences(@"
foo
==References==
* [http://www.site.com/site a site]
==External links==
* http://www.site2.com
"));

            Assert.IsFalse(Parsers.HasBareReferences(@"
foo
* [http://www.site.com/site a site]
==External links==
* http://www.site2.com
"));

            Assert.IsFalse(Parsers.HasBareReferences(@""));
            Assert.IsFalse(Parsers.HasBareReferences(@"foo"));
        }

        [Test]
        public void DablinksAboutAbout()
        {
            const string AboutAbout = @"{{About|about a historical district in the foo|the modern district|Nurfoo District, Republic of Bar}}";
            Assert.That(Parsers.Dablinks(AboutAbout), Is.EqualTo(AboutAbout.Replace("about ", "")), "removes about...about");
            Assert.That(Parsers.Dablinks(AboutAbout.Replace("about ", "  About ")), Is.EqualTo(AboutAbout.Replace("about ", "")), "removes about...about, allows extra whitespace");

            Assert.That(Parsers.Dablinks(AboutAbout.Replace("about ", " about")), Is.EqualTo(AboutAbout.Replace("about ", "")), "no change if already correct");
        }

        [Test]
        public void DablinksNoZerothSection()
        {
            const string NoZerothSection1 = @"
===fpfp===
words";
            Assert.That(Parsers.Dablinks(NoZerothSection1), Is.EqualTo(NoZerothSection1));

            const string NoZerothSection2 = @"===fpfp===
words";
            Assert.That(Parsers.Dablinks(NoZerothSection2), Is.EqualTo(NoZerothSection2));
        }

        [Test]
        public void DablinksOtheruses4()
        {
            string OU = @"{{Otheruses4|foo|bar}}";

            Assert.That(Parsers.Dablinks(OU), Is.EqualTo(@"{{About|foo|bar}}"));
        }

        [Test]
        public void DablinksMergingFor()
        {
            const string AboutBefore = @"{{about|Foo|a|b}}", forBefore = @"{{for|c|d}}", aboutAfter = @"{{about|Foo|a|b|c|d}}";

            Assert.That(Parsers.Dablinks(AboutBefore + forBefore), Is.EqualTo(aboutAfter), "merges for into about (3 args)");
            Assert.That(Parsers.Dablinks(forBefore + AboutBefore), Is.EqualTo(aboutAfter), "merges for into about – for first");
            Assert.That(Parsers.Dablinks(forBefore), Is.EqualTo(forBefore), "single for not changed");
            Assert.That(Parsers.Dablinks(AboutBefore), Is.EqualTo(AboutBefore), "single about not changed");

            const string for1 = @"{{for|a|b}}", about1 = @"{{about||a|b|c|d}}";

            Assert.That(Parsers.Dablinks(for1 + forBefore), Is.EqualTo(about1));

           const string singleAbout = @"{{about|foo}}";

            Assert.That(Parsers.Dablinks(singleAbout + forBefore), Is.EqualTo(singleAbout + forBefore), "no merge if {{about}} has <2 arguments");

            const string About2Before = @"{{about|foo|a}}";

            Assert.That(Parsers.Dablinks(About2Before + forBefore), Is.EqualTo(@"{{about|foo|a||c|d}}"), "merges for into about (2 args)");

            Assert.That(Parsers.Dablinks(About2Before + forBefore + @"{{for|e|f}}"), Is.EqualTo(@"{{about|foo|a||c|d|e|f}}"), "two for merges");

            Assert.That(Parsers.Dablinks(for1 + "\r\n==foo==" + forBefore), Is.EqualTo(for1 + "\r\n==foo==" + forBefore), "only merges dablinks in zeroth section");

            const string For3 = @"{{for|c|d|e}}";

            Assert.That(Parsers.Dablinks(for1 + For3), Is.EqualTo(@"{{about||a|b|c|d|and|e}}"), "merge for with 1 and 3 arguments");
            Assert.That(Parsers.Dablinks(For3 + for1), Is.EqualTo(@"{{about||a|b|c|d|and|e}}"), "merge for with 1 and 3 arguments");

            Assert.That(Parsers.Dablinks(@"{{for|a|b}}{{for|c|d}}{{for|e|f|g|h}}"), Is.EqualTo(@"{{about||a|b|c|d}}{{for|e|f|g|h}}"), "do not merge for with 4 arguments");
            Assert.That(Parsers.Dablinks(@"{{for|a|b|c|d}}{{for|e|f|g|h}}"), Is.EqualTo(@"{{for|a|b|c|d}}{{for|e|f|g|h}}"), "do not merge for with 4 arguments"); 
            
            const string ForTwoCats = @"{{for|the city in California|Category:Lancaster, California}}{{for|the city in Pennsylvania|Category:Lancaster, Pennsylvania}}";

            Assert.That(Parsers.Dablinks(ForTwoCats), Is.EqualTo(@"{{about||the city in California|:Category:Lancaster, California|the city in Pennsylvania|:Category:Lancaster, Pennsylvania}}"));
            Assert.That(Parsers.Dablinks(@"{{about||a|b|c|d}}{{for||e}}"), Is.EqualTo(@"{{about||a|b|c|d|other uses|e}}"), "for with first argument empty");
            Assert.That(Parsers.Dablinks(@"{{about|foo|a|b|c|d}}{{for||e}}"), Is.EqualTo(@"{{about|foo|a|b|c|d|other uses|e}}"), "for with first argument empty");

            Assert.That(Parsers.Dablinks(@"{{about|1|2|3|4|5|6|7|8|9}}{{for|a|b}}"), Is.EqualTo(@"{{about|1|2|3|4|5|6|7|8|9}}{{for|a|b}}"), "don't do anything if about has 9 arguments");

            Assert.That(Parsers.Dablinks(@"{{about||ended|List of ended}}{{about||original|Lists of original|specials|List of specials}}{{about||exclusive|List of exclusive}}"), Is.EqualTo(@"{{about||ended|List of ended|original|Lists of original|specials|List of specials|exclusive|List of exclusive}}"), "Combine multiple in || mode");
        }

        [Test]
        public void DablinksMergingAbout()
        {
            const string AboutAfter = @"{{about||a|b|c|d}}", a1 = @"{{about||a|b}}", a2 = @"{{about||c|d}}";

            Assert.That(Parsers.Dablinks(a1 + a2), Is.EqualTo(AboutAfter), "merges abouts with same reason: null reason");
            Assert.That(Parsers.Dablinks(AboutAfter), Is.EqualTo(AboutAfter), "no change if already correct");

            const string AboutAfterFoo = @"{{about|Foo|a|b|c|d}}", a1Foo = @"{{about|Foo|a|b}}", a2Foo = @"{{about|Foo|c|d}}";

            Assert.That(Parsers.Dablinks(a1Foo + a2Foo), Is.EqualTo(AboutAfterFoo), "merges abouts with same reason: reason given");
            Assert.That(Parsers.Dablinks(AboutAfterFoo), Is.EqualTo(AboutAfterFoo), "no change if already correct");

            string a2Bar = @"{{about|Bar|c|d}}";
            Assert.That(Parsers.Dablinks(a2Bar + a2Foo), Is.EqualTo(a2Bar + a2Foo), "not merged when reason different");

            const string m1 = @"{{About||the film adaptation|The League of Extraordinary Gentlemen (film)}}
{{About||a list of the characters and their origins|Characters in The League of Extraordinary Gentlemen}}
{{About||the British comedy|The League of Gentlemen}}", m1a = @"{{About||the film adaptation|The League of Extraordinary Gentlemen (film)|a list of the characters and their origins|Characters in The League of Extraordinary Gentlemen|the British comedy|The League of Gentlemen}}";

            Assert.That(Parsers.Dablinks(m1), Is.EqualTo(m1a + "\r\n\r\n"));

            string zerosec = @"{{about|foo||a}}{{about|foo||b}}";

            Assert.That(Parsers.Dablinks(zerosec), Is.EqualTo(@"{{about|foo||a|and|b}}"));

            Assert.That(Parsers.Dablinks(a1Foo + a2), Is.EqualTo(@"{{about|Foo|a|b|c|d}}"));
        }

        [Test]
        public void DablinksMergingAboutNamespace()
        {
            const string a2Foo = @"{{about|Foo|Category:Bar|d}}";

            Assert.That(Parsers.Dablinks(a2Foo), Is.EqualTo(a2Foo.Replace("|C", "|:C")), "escapes category in {{about}}");
        }

        [Test]
        public void DablinksEnOnly()
        {
#if DEBUG
            string zerosec = @"{{about|foo||a}}{{about|foo||b}}";

            Variables.SetProjectLangCode("fr");
            Assert.That(Parsers.Dablinks(zerosec), Is.EqualTo(zerosec));

            Variables.SetProjectLangCode("en");
            Assert.That(Parsers.Dablinks(zerosec), Is.EqualTo(@"{{about|foo||a|and|b}}"));
#endif
        }

        [Test]
        public void DablinksMergingDistinguish()
        {
            string AB = @"{{Distinguish|a|b}}";
            Assert.That(Parsers.Dablinks(@"{{Distinguish|a}}{{Distinguish|b}}"), Is.EqualTo(AB), "merges when single argument");
            Assert.That(Parsers.Dablinks(@"{{Distinguish|a|b}}{{Distinguish|c}}"), Is.EqualTo(AB.Replace("}}", "|c}}")), "merges multiple arguments");
            Assert.That(Parsers.Dablinks(@"{{Distinguish|a}}{{Distinguish|b|c}}"), Is.EqualTo(AB.Replace("}}", "|c}}")), "merges multiple arguments");
            Assert.That(Parsers.Dablinks(AB), Is.EqualTo(AB), "no change if already merged");
            AB = @"{{Distinguish|text=a}}{{Distinguish|text=b}}";
            Assert.That(Parsers.Dablinks(AB), Is.EqualTo(AB), "no change if using text= parameter");
            AB = @"{{Distinguish|A}}{{Distinguish|text=b}}";
            Assert.That(Parsers.Dablinks(AB), Is.EqualTo(AB), "no change if using text= parameter, second only");
        }

        [Test]
        public void MergeTemplatesBySection()
        {
            string AB = @"{{See also|a|b}}";
            Assert.That(Parsers.MergeTemplatesBySection(@"{{See also|a}}{{See also|b}}"), Is.EqualTo(AB), "merges when single argument");
            Assert.That(Parsers.MergeTemplatesBySection(@"{{See also|a|b}}{{See also|c}}"), Is.EqualTo(AB.Replace("}}", "|c}}")), "merges multiple arguments");
            Assert.That(Parsers.MergeTemplatesBySection(@"{{See also|a}}{{See also|b|c}}"), Is.EqualTo(AB.Replace("}}", "|c}}")), "merges multiple arguments");
            Assert.That(Parsers.MergeTemplatesBySection(@"{{See also|a}}{{see also|b|c}}"), Is.EqualTo(AB.Replace("}}", "|c}}")), "different capitalition");
            AB = @"{{See also|a{{!}}b}}";
            Assert.That(Parsers.MergeTemplatesBySection(@"{{See also|a{{!}}b}}{{see also|c|d}}"), Is.EqualTo(AB.Replace("b}}", "b|c|d}}")), "With {{!}}");
            Assert.That(Parsers.MergeTemplatesBySection(AB), Is.EqualTo(AB), "no change if already merged");

            AB = @"{{See also2|a|b}}";
            Assert.That(Parsers.MergeTemplatesBySection(@"{{See also2|a}}{{See also2|b}}"), Is.EqualTo(AB), "merges when single argument");
            Assert.That(Parsers.MergeTemplatesBySection(@"{{See also2|a|b}}{{See also2|c}}"), Is.EqualTo(AB.Replace("}}", "|c}}")), "merges multiple arguments");
            Assert.That(Parsers.MergeTemplatesBySection(@"{{See also2|a}}{{See also2|b|c}}"), Is.EqualTo(AB.Replace("}}", "|c}}")), "merges multiple arguments");
            Assert.That(Parsers.MergeTemplatesBySection(@"{{See also2|a}}{{See also2|b|[[c]]}}"), Is.EqualTo(AB.Replace("}}", "|[[c]]}}")), "merges multiple arguments, one with link");
            Assert.That(Parsers.MergeTemplatesBySection(@"{{See also2|a}}{{see also2|b|c}}"), Is.EqualTo(AB.Replace("}}", "|c}}")), "different capitalition");
            Assert.That(Parsers.MergeTemplatesBySection(AB), Is.EqualTo(AB), "no change if already merged");

            AB = @"{{Main|a|b}}";
            Assert.That(Parsers.MergeTemplatesBySection(@"{{Main|a}}{{Main|b}}"), Is.EqualTo(AB), "merges when single argument");
            Assert.That(Parsers.MergeTemplatesBySection(@"{{Main|a|b}}{{Main|c}}"), Is.EqualTo(AB.Replace("}}", "|c}}")), "merges multiple arguments");
            Assert.That(Parsers.MergeTemplatesBySection(@"{{Main|a}}{{Main|b|c}}"), Is.EqualTo(AB.Replace("}}", "|c}}")), "merges multiple arguments");
            Assert.That(Parsers.MergeTemplatesBySection(@"{{Main|a}}{{Main|b|[[c]]}}"), Is.EqualTo(AB.Replace("}}", "|[[c]]}}")), "merges multiple arguments, one with link");
            Assert.That(Parsers.MergeTemplatesBySection(@"{{Main|a}}{{main|b|c}}"), Is.EqualTo(AB.Replace("}}", "|c}}")), "different capitalition");
            Assert.That(Parsers.MergeTemplatesBySection(AB), Is.EqualTo(AB), "no change if already merged");

            AB = @"{{Main|a}}{{Main|b|l1=d}}";
            Assert.That(Parsers.MergeTemplatesBySection(AB), Is.EqualTo(AB), "no change when link params used");

            const string SeparateSections = @"==One==
{{see also|A}}
==Two==
{{see also|B}}";
            Assert.That(Parsers.MergeTemplatesBySection(SeparateSections), Is.EqualTo(SeparateSections), "does not merge templates in different sections");
            
             const string NotStartOfSection = @"==One==
{{see also|A}}
Text
{{see also|B}}";
            Assert.That(Parsers.MergeTemplatesBySection(NotStartOfSection), Is.EqualTo(NotStartOfSection), "does not merge templates not at top of section");
            Assert.That(Parsers.MergeTemplatesBySection(@"==One==
{{see also|A}}
{{see also|B}}"), Is.EqualTo(@"==One==
{{see also|A|B}}"), "does not merge templates not at top of section");
        }
        
        [Test]
        public void MergeTemplatesBySectionEnOnly()
        {
            #if DEBUG
            string AB = @"{{See also|a|b}}", ABSeparate = @"{{See also|a}}{{See also|b}}";

            Variables.SetProjectLangCode("fr");
            Assert.That(Parsers.MergeTemplatesBySection(ABSeparate), Is.EqualTo(ABSeparate), "No merge when not en-wiki");

            Variables.SetProjectLangCode("en");
            Assert.That(Parsers.MergeTemplatesBySection(ABSeparate), Is.EqualTo(AB), "merges when single argument");
            #endif
        }

        [Test]
        public void MergePortals()
        {
            Assert.That(Parsers.MergePortals(""), Is.Empty, "no change when no portals");
            Assert.That(Parsers.MergePortals("{{portal|Foo|Bar}}"), Is.EqualTo("{{portal|Foo|Bar}}"), "no change when already OK");
            Assert.That(Parsers.MergePortals("==see also=="), Is.EqualTo("==see also=="), "no change when no portals");

            const string singlePortal = @"Foo
==See also==
{{Portal|Bar}}";
            Assert.That(Parsers.MergePortals(singlePortal), Is.EqualTo(singlePortal), "no change when single portal");

            const string PortalBox1 = @"Foo
==See also==
{{Portal|Bar|Foo2}}
";
            Assert.That(Parsers.MergePortals(@"Foo
==See also==
{{Portal|Bar}}
{{Portal|Foo2 }}"), Is.EqualTo(PortalBox1), "merges multiple portals to single portal");

            const string NoSeeAlso = @"{{Portal|Bar}}
===One===
{{Portal|Foo2 }}";

            Assert.That(Parsers.MergePortals(NoSeeAlso), Is.EqualTo(NoSeeAlso), "no merging if no portal and no see also, separate sections");

            string MultipleArguments = @"Foo
==See also==
{{Portal|Bar}}
{{Portal|Foo2|other=here}}";

            Assert.That(Parsers.MergePortals(MultipleArguments), Is.EqualTo(MultipleArguments), "no merging of portal with named arguments");

            MultipleArguments = @"Foo
==See also==
{{Portal|Foo2|other=here}}
{{Portal|Bar}}
";
            Assert.That(Parsers.MergePortals(MultipleArguments), Is.EqualTo(MultipleArguments), "no merging of portal with named arguments 2");

            // merging in same section
            const string SameSection = @"Foo
==Section==
{{Portal|Bar}}
{{Portal|Foo}}
==Other==";
            Assert.That(Parsers.MergePortals(SameSection), Is.EqualTo(@"Foo
==Section==
{{Portal|Bar|Foo}}
==Other=="), "portals merged to first portal location when all in same section");

            const string differentSection = @"Foo
==Section==
{{Portal|Bar}}
==Other==
{{Portal|Foo}}";
            Assert.That(Parsers.MergePortals(differentSection), Is.EqualTo(differentSection), "not merged when portals in different sections");

            const string TwoSeeAlso = @"Foo
==See also==
{{Portal|Bar}}
==See also==
{{Portal|Foo}}";
            Assert.That(Parsers.MergePortals(TwoSeeAlso), Is.EqualTo(TwoSeeAlso), "not merged when multiple see also sections");

            const string SingleSection = @"Foo
{{Portal|Bar}}
{{Portal|Foo}}
";
            Assert.That(Parsers.MergePortals(SingleSection), Is.EqualTo(@"Foo
{{Portal|Bar|Foo}}
"), "portals merged to first portal location when article has no sections");

            const string Dupes = @"Foo
{{Portal|Bar}}
{{Portal|Bar}}
";
            Assert.That(Parsers.MergePortals(Dupes), Is.EqualTo(@"Foo
{{Portal|Bar}}
"), "portals deduplicated");

            Assert.That(Parsers.MergePortals(@"Foo
{{Portal|Bar|Bar2}}
{{Portal|Foo|Foo2}}
"), Is.EqualTo(@"Foo
{{Portal|Bar|Bar2|Foo|Foo2}}
"), "portals with multiple arguments combined");

            Assert.That(Parsers.MergePortals(@"Foo
{{Portal|Bar|Bar2|Foo}}
{{Portal|Foo2}}
"), Is.EqualTo(@"Foo
{{Portal|Bar|Bar2|Foo|Foo2}}
"), "portals with multiple arguments combined 2");

            Assert.That(Parsers.MergePortals(@"Foo
{{Portal|Bar|Bar2|Foo1}}
{{Portal|Foo2}}
"), Is.EqualTo(@"Foo
{{Portal|Bar|Bar2|Foo1|Foo2}}
"), "portals with multiple arguments combined and deduplicated");

            Assert.That(Parsers.MergePortals(@"Foo
{{Portal|Bar|Bar2|Foo1|Abc}}
{{Portal|Foo2|Abc}}
"), Is.EqualTo(@"Foo
{{Portal|Bar|Bar2|Foo1|Abc|Foo2}}
"), "portals with multiple arguments combined and deduplicated");

            Assert.That(Parsers.MergePortals(@"Foo
{{Portal|Bar|Foo1}}
{{Portal|Abc}}
==Head==
"), Is.EqualTo(@"Foo
{{Portal|Bar|Foo1|Abc}}
==Head==
"), "portals with multiple arguments combined, zeroth section");

            const string stack = @"Foo
==See also==
{{stack|{{portal|Foo|bar}}}}
";
            Assert.That(Parsers.MergePortals(stack), Is.EqualTo(stack), "No change when portal in stack template");
        }

        [Test]
        public void MergePortalsEnOnly()
        {
#if DEBUG
            const string PortalBox1 = @"Foo
==See also==
{{Portal|Bar|Foo2}}
", input = @"Foo
==See also==
{{Portal|Bar}}
{{Portal|Foo2 }}";

            Variables.SetProjectLangCode("fr");
            Assert.That(Parsers.MergePortals(input), Is.EqualTo(input));

            Variables.SetProjectLangCode("en");
            Assert.That(Parsers.MergePortals(input), Is.EqualTo(PortalBox1));
#endif
        }
    }

    [TestFixture]
    public class UnicodifyTests : RequiresParser
    {
        [Test]
        public void PreserveTM()
        {
            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_1#AWB_corrupts_the_trademark_.28TM.29_special_character_.
            Assert.That(parser.Unicodify("test™"), Is.EqualTo("test™"));
        }

        [Test]
        public void DontChangeCertainEntities()
        {
            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_3#.26emsp.3B
            Assert.That(parser.Unicodify("&emsp;&#013;"), Is.EqualTo("&emsp;&#013;"));

            Assert.That(parser.Unicodify("The F&#x2011;22 plane"), Is.EqualTo("The F&#x2011;22 plane"));

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_10#SmackBot:_conversion_of_HTML_char-codes_to_raw_Unicode:_issue_.26_consequent_suggestion
            Assert.That(parser.Unicodify(@"the exclamation mark&#8201;! was"), Is.EqualTo(@"the exclamation mark&#8201;! was"));
            Assert.That(parser.Unicodify(@"the exclamation mark&#8239;! was"), Is.EqualTo(@"the exclamation mark&#8239;! was"));

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#zero-width_space
            Assert.That(parser.Unicodify(@" hello &#8203; bye"), Is.EqualTo(@" hello &#8203; bye"));

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Greedy regex for unicode characters
            Assert.That(parser.Unicodify(@" hello &#x20000; bye"), Is.EqualTo(@" hello &#x20000; bye"));
            Assert.That(parser.Unicodify(@" hello &#x2000f; bye"), Is.EqualTo(@" hello &#x2000f; bye"));

            Assert.That(parser.Unicodify("A &#x2329; B"), Is.EqualTo("A &#x2329; B"));
            Assert.That(parser.Unicodify("A &#x232A; B"), Is.EqualTo("A &#x232A; B"));

            // Characters above hex 10000 not changed
            Assert.That(parser.Unicodify("A &#x10A80; B"), Is.EqualTo("A &#x10A80; B"));
            Assert.That(parser.Unicodify("A &#x20A80; B"), Is.EqualTo("A &#x20A80; B"));

            // &apos can be necessary to avoid affecting italics/bold
            Assert.That(parser.Unicodify(@"''Foo''&apos;s work was the best since ''bar''&apos;s"), Is.EqualTo(@"''Foo''&apos;s work was the best since ''bar''&apos;s"));

            const string ColonLine = @"; Heading&#58;
Text";
            Assert.That(parser.Unicodify(ColonLine), Is.EqualTo(ColonLine));
        }

        [Test]
        public void IgnoreMath()
        {
            Assert.That(parser.Unicodify("<math>&laquo;</math>"), Is.EqualTo("<math>&laquo;</math>"));
            Assert.That(parser.Unicodify("<math chem>&laquo;</math>"), Is.EqualTo("<math chem>&laquo;</math>"));
            Assert.That(parser.Unicodify("<chem>&laquo;</chem>"), Is.EqualTo("<chem>&laquo;</chem>"));
        }
        
        [Test]
        public void Casing()
        {
            Assert.That(parser.Unicodify("A&dagger; B&Dagger;"), Is.EqualTo("A† B‡"), "supports lowercase and first-upper HTML characters");
        }
        
        [Test]
        public void Templates()
        {
            Assert.That(parser.Unicodify("A&dagger; B{{template|&dagger;}} C{{template2|one=&dagger;}}"), Is.EqualTo("A† B{{template|†}} C{{template2|one=†}}"), "can support characters within templates");
        }
    }

    [TestFixture]
    public class RecategorizerTests : RequiresParser
    {
        [Test]
        public void Addition()
        {
            bool noChange;

            Assert.That(parser.AddCategory("Foo", "", "bar", out noChange), Is.EqualTo("\r\n\r\n[[Category:Foo]]"));
            Assert.IsFalse(noChange);

            Assert.That(parser.AddCategory("Foo", "bar", "bar", out noChange), Is.EqualTo("bar\r\n\r\n[[Category:Foo]]"));
            Assert.IsFalse(noChange);

            Assert.That(parser.AddCategory("Bar", "test[[Category:Foo|bar]]", "foo", out noChange),
                            Is.EqualTo("test\r\n\r\n[[Category:Foo|bar]]\r\n[[Category:Bar]]"));
            Assert.IsFalse(noChange);

            // shouldn't add if category already exists
            Assert.That(parser.AddCategory("Foo", "[[Category:Foo]]", "bar", out noChange), Is.EqualTo("[[Category:Foo]]"));
            Assert.IsTrue(noChange);
            Assert.IsTrue(Regex.IsMatch(parser.AddCategory("Foo bar", "[[Category:Foo_bar]]", "bar", out noChange), @"\[\[Category:Foo[ _]bar\]\]"));
            Assert.IsTrue(noChange);

            Assert.That(parser.AddCategory("Foo bar", "[[category : foo_bar%20|quux]]", "bar", out noChange), Is.EqualTo("[[category : foo_bar%20|quux]]"));
            Assert.IsTrue(noChange);

            Assert.That(parser.AddCategory("Foo", "test", "Template:foo", out noChange),
                            Is.EqualTo("test<noinclude>\r\n[[Category:Foo]]\r\n</noinclude>"));
            Assert.IsFalse(noChange);

            // don't change cosmetic whitespace when adding a category
            const string Newlineheading = @"==Persian==

===Pronunciation===";
            Assert.That(parser.AddCategory("Foo", Newlineheading, "bar", out noChange), Is.EqualTo(Newlineheading + "\r\n\r\n" + @"[[Category:Foo]]"));
        }

        [Test]
        public void Replacement()
        {
            bool noChange;

            Assert.That(Parsers.ReCategoriser("Foo", "Bar", "[[Category:Foo]]", out noChange), Is.EqualTo("[[Category:Bar]]"));
            Assert.IsFalse(noChange);

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_7#Replacing_Arabic_categories
            // addresses special case in Tools.FirstLetterCaseInsensitive
            Assert.That(Parsers.ReCategoriser("-Foo bar-", "Bar", "[[Category:-Foo bar-]]", out noChange), Is.EqualTo("[[Category:Bar]]"));
            Assert.IsFalse(noChange);
            Assert.That(Parsers.ReCategoriser("Foo", "-Bar II-", "[[Category:Foo]]", out noChange), Is.EqualTo("[[Category:-Bar II-]]"));
            Assert.IsFalse(noChange);


            Assert.That(Parsers.ReCategoriser("Foo", "Bar", "[[ catEgory: Foo]]", out noChange), Is.EqualTo("[[Category:Bar]]"));
            Assert.IsFalse(noChange);

            Assert.That(Parsers.ReCategoriser("Foo", "Bar", "[[Category:foo]]", out noChange), Is.EqualTo("[[Category:Bar]]"));
            Assert.IsFalse(noChange);

            Assert.That(Parsers.ReCategoriser("Foo", "Bar", "[[Category:Foo|boz]]", out noChange), Is.EqualTo("[[Category:Bar|boz]]"));
            Assert.IsFalse(noChange);

            Assert.That(Parsers.ReCategoriser("foo? Bar!", "Bar", "[[ category:Foo?_Bar! | boz]]", out noChange), Is.EqualTo("[[Category:Bar| boz]]"));
            Assert.IsFalse(noChange);

            Assert.That(Parsers.ReCategoriser("Foo", "Bar", @"[[Category:Boz]]
[[Category:foo]]
[[Category:Quux]]", out noChange), Is.EqualTo(@"[[Category:Boz]]
[[Category:Bar]]
[[Category:Quux]]"));
            Assert.IsFalse(noChange);

            Assert.That(Parsers.ReCategoriser("Foo", "Bar", "test[[Category:Foo]]test", out noChange), Is.EqualTo("test[[Category:Bar]]test"));
            Assert.IsFalse(noChange);

            Assert.That(Parsers.ReCategoriser("Foo", "Bar", "[[Category:Fooo]]", out noChange), Is.EqualTo("[[Category:Fooo]]"));
            Assert.IsTrue(noChange);
        }

        [Test]
        public void ReplacementSortkeys()
        {
            bool noChange;

            Assert.That(Parsers.ReCategoriser("Foo", "Bar", "[[Category:Foo|key]]", out noChange), Is.EqualTo("[[Category:Bar|key]]"));
            Assert.IsFalse(noChange);

            Assert.That(Parsers.ReCategoriser("Foo", "Bar", "[[Category:Foo|key here]]", out noChange), Is.EqualTo("[[Category:Bar|key here]]"));
            Assert.IsFalse(noChange);

            Assert.That(Parsers.ReCategoriser("Foo", "Bar", "[[Category:Foo|key]]", out noChange, false), Is.EqualTo("[[Category:Bar|key]]"));
            Assert.IsFalse(noChange);

            Assert.That(Parsers.ReCategoriser("Foo", "Bar", "[[Category:Foo|key]]", out noChange, true), Is.EqualTo("[[Category:Bar]]"));
            Assert.IsFalse(noChange);

            Assert.That(Parsers.ReCategoriser("Foo", "Bar", "[[Category:Foo|key here]]", out noChange, true), Is.EqualTo("[[Category:Bar]]"));
            Assert.IsFalse(noChange);

            //
            Assert.That(Parsers.ReCategoriser("Foo", "Bar", "[[Category:Foo|key]] [[Category:Bar|key]]", out noChange), Is.EqualTo(" [[Category:Bar|key]]"));
            Assert.IsFalse(noChange);
        }

        [Test]
        public void Removal()
        {
            bool noChange;

            Assert.That(Parsers.RemoveCategory("Foo", "[[Category:Foo]]", out noChange), Is.Empty);
            Assert.IsFalse(noChange);

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_7#Replacing_Arabic_categories
            // addresses special case in Tools.FirstLetterCaseInsensitive
            Assert.That(Parsers.RemoveCategory("-Foo bar-", "[[Category:-Foo bar-]]", out noChange), Is.Empty);
            Assert.IsFalse(noChange);

            Assert.That(Parsers.RemoveCategory("Foo", "[[ category: foo | bar]]", out noChange), Is.Empty);
            Assert.IsFalse(noChange);

            Assert.That(Parsers.RemoveCategory("Foo", "[[Category:Foo|]]", out noChange), Is.Empty);
            Assert.IsFalse(noChange);

            Assert.That(Parsers.RemoveCategory("Foo", " [[Category:Foo]] ", out noChange), Is.EqualTo("  "));
            Assert.IsFalse(noChange);

            Assert.That(Parsers.RemoveCategory("Foo", "[[Category:Foo]]\r\n", out noChange), Is.Empty);
            Assert.IsFalse(noChange);

            Assert.That(Parsers.RemoveCategory("Foo", "[[Category:Foo]]\r\n\r\n", out noChange), Is.EqualTo("\r\n"));
            Assert.IsFalse(noChange);

            Assert.That(Parsers.RemoveCategory("Foo? Bar!", "[[Category:Foo?_Bar!|boz]]", out noChange), Is.Empty);
            Assert.IsFalse(noChange);

            Assert.That(Parsers.RemoveCategory("Foo", "[[Category:Fooo]]", out noChange), Is.EqualTo("[[Category:Fooo]]"));
            Assert.IsTrue(noChange);
        }
    }

    [TestFixture]
    public class ConversionTests : RequiresParser
    {
        [Test]
        public void ConversionsTestsInfoBox()
        {
            string correct = @"{{infobox foo|date=May 2010}}";

            Assert.That(Parsers.Conversions(@"{{infobox_foo|date=May 2010}}"), Is.EqualTo(correct));
            Assert.That(Parsers.Conversions(correct), Is.EqualTo(correct));

            correct = @"{{infobox foo
|date=May 2010}}";
            Assert.That(Parsers.Conversions(correct), Is.EqualTo(correct));

            correct = @"{{infobox
|date=May 2010}}";
            Assert.That(Parsers.Conversions(correct), Is.EqualTo(correct));

            correct = @"{{infobox foo|date=May 2010}}";
            Assert.That(Parsers.Conversions(@"{{Template:infobox_foo|date=May 2010}}"), Is.EqualTo(correct));
        }

        [Test]
        public void ConversionsTestsUnreferenced()
        {
            string correct = @"{{Unreferenced section|date=May 2010}}";
            Assert.That(Parsers.Conversions(@"{{Unreferenced|section|date=May 2010}}"), Is.EqualTo(correct));
            Assert.That(Parsers.Conversions(@"{{Unreferenced|Section|date=May 2010}}"), Is.EqualTo(correct));
            Assert.That(Parsers.Conversions(@"{{Unreferenced|  section |date=May 2010}}"), Is.EqualTo(correct));
            Assert.That(Parsers.Conversions(correct), Is.EqualTo(correct));
            Assert.That(Parsers.Conversions(@"{{unreferenced|section|date=May 2010}}"), Is.EqualTo(correct.Replace("U", "u")));
            Assert.That(Parsers.Conversions(@"{{Unreferenced|list|date=May 2010}}"), Is.EqualTo(@"{{Unreferenced section|date=May 2010}}"));
            Assert.That(Parsers.Conversions(@"{{Unreferenced|type=section|date=May 2010}}"), Is.EqualTo(@"{{Unreferenced section|date=May 2010}}"));
            Assert.That(Parsers.Conversions(@"{{Unreferenced|type=Section|date=May 2010}}"), Is.EqualTo(@"{{Unreferenced section|date=May 2010}}"));

            Assert.That(Parsers.Conversions(@"{{Unreferenced|date=May 2010|auto=yes}}"), Is.EqualTo(@"{{Unreferenced|date=May 2010}}"));
            Assert.That(Parsers.Conversions(@"{{Unreferenced|date=May 2010|auto=YES}}"), Is.EqualTo(@"{{Unreferenced|date=May 2010}}"));

            Assert.IsTrue(Parsers.Conversions(@"{{unreferenced|date=October 2011}}
'''Gretchen F''' known as is a Filipina model.

==Reference==
*http://www.pep.ph/p
{{DEFAULTSORT:Gretchen F}}
[[Category:1980 births]]").Contains(@"unreferenced"), "no template renaming when within multiple issues");
        }

        [Test]
        public void ConversionsTestsSectionTemplates()
        {
            string correct = @"{{Unreferenced section|date=May 2010}}";
            Assert.That(Parsers.Conversions(@"{{Unreferenced|section|date=May 2010}}"), Is.EqualTo(correct));

            correct = @"{{refimprove section|date=May 2010}}";
            Assert.That(Parsers.Conversions(@"{{refimprove|section|date=May 2010}}"), Is.EqualTo(correct));

            correct = @"{{More citations needed section|date=May 2010}}";
            Assert.That(Parsers.Conversions(@"{{More citations needed|section|date=May 2010}}"), Is.EqualTo(correct));

            correct = @"{{BLP sources section|date=May 2010}}";
            Assert.That(Parsers.Conversions(@"{{BLP sources|section|date=May 2010}}"), Is.EqualTo(correct));

            correct = @"{{expand section|date=May 2010}}";
            Assert.That(Parsers.Conversions(@"{{expand|section|date=May 2010}}"), Is.EqualTo(correct));

            correct = @"{{BLP unsourced section|date=May 2010}}";
            Assert.That(Parsers.Conversions(@"{{BLP unsourced|section|date=May 2010}}"), Is.EqualTo(correct));
        }

        [Test]
        public void RemoveExcessTemplatePipes()
        {
            // extra pipe
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web | url=http://www.site.com || title=hello}}"), Is.EqualTo(@"{{cite web | url=http://www.site.com | title=hello}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web || url=http://www.site.com || title=hello}}"), Is.EqualTo(@"{{cite web | url=http://www.site.com | title=hello}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web | url=http://www.site.com | | title=hello}}"), Is.EqualTo(@"{{cite web | url=http://www.site.com | title=hello}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite wikisource|bar||foo}}"), Is.EqualTo(@"{{cite wikisource|bar||foo}}"));

            Assert.That(Parsers.FixCitationTemplates(@"{{cite uscgll|bar||foo}}"), Is.EqualTo(@"{{cite uscgll|bar||foo}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite ngall|bar||foo}}"), Is.EqualTo(@"{{cite ngall|bar||foo}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{Cite Legislation AU|bar||foo}}"), Is.EqualTo(@"{{Cite Legislation AU|bar||foo}}"));

            const string UnclosedCiteInTable = @"
|
|Yes<ref>{{cite web | title=A </ref>
|
|";
            Assert.That(Parsers.FixCitationTemplates(UnclosedCiteInTable), Is.EqualTo(UnclosedCiteInTable), "Does not alter table pipes following unclosed ref cite");
        }

        [Test]
        public void ConversionTestsCommonsCat()
        {
            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#.7B.7Bcommons.7CCategory:XXX.7D.7D_.3E_.7B.7Bcommonscat.7CXXX.7D.7D
            // {{commons|Category:XXX}} > {{commons category|XXX}}
            Assert.That(Parsers.Conversions(@"{{commons|Category:XXX}}"), Is.EqualTo(@"{{Commons category|XXX}}"));
            Assert.That(Parsers.Conversions(@"{{commons |Category:XXX}}"), Is.EqualTo(@"{{Commons category|XXX}}"));
            Assert.That(Parsers.Conversions(@"{{Commons|category:XXX}}"), Is.EqualTo(@"{{Commons category|XXX}}"));
            Assert.That(Parsers.Conversions(@"{{Commons| category:XXX }}"), Is.EqualTo(@"{{Commons category|XXX}}"));
            Assert.That(Parsers.Conversions(@"{{commons|Category:Backgammon|Backgammon}}"), Is.EqualTo(@"{{Commons category|Backgammon}}"));
            Assert.That(Parsers.Conversions(@"{{commons|Category:Backgammon | Backgammon  }}"), Is.EqualTo(@"{{Commons category|Backgammon}}"));
            Assert.That(Parsers.Conversions(@"{{Commons|Category:Backgammon|Backgammon main}}"), Is.EqualTo(@"{{Commons category|Backgammon|Backgammon main}}"));
            Assert.That(Parsers.Conversions(@"{{commons cat|Gander International Airport|Gander International Airport}}"), Is.EqualTo(@"{{commons cat|Gander International Airport}}"));
            Assert.That(Parsers.Conversions(@"{{Commons cat|Gander International Airport|Gander International Airport}}"), Is.EqualTo(@"{{Commons cat|Gander International Airport}}"));

            #if DEBUG
            Variables.SetProjectSimple("en", ProjectEnum.wikia);
            Assert.That(Parsers.Conversions(@"{{commons|Category:Backgammon|Backgammon}}"), Is.EqualTo(@"{{commons|Category:Backgammon|Backgammon}}"), "{{Commons category}} not used on Wikia");
            Variables.SetProjectSimple("en", ProjectEnum.wikipedia);
            #endif
        }

        [Test]
        public void ConversionTestsMoreFootnotes()
        {
            // no footnotes --> more footnotes
            Assert.That(Parsers.Conversions(@"Article <ref>A</ref>
            ==References==
            {{no footnotes}}
            {{reflist}}"), Is.EqualTo(@"Article <ref>A</ref>
            ==References==
            {{more footnotes needed}}
            {{reflist}}"));

            Assert.That(Parsers.Conversions(@"Article <ref>A</ref>
            ==References==
            {{no footnotes}}
            {{reflist}}"), Is.EqualTo(@"Article <ref>A</ref>
            ==References==
            {{more footnotes needed}}
            {{reflist}}"));

            Assert.That(Parsers.Conversions(@"Article {{sfn|Smith|2004}}
            ==References==
            {{no footnotes}}
            {{reflist}}"), Is.EqualTo(@"Article {{sfn|Smith|2004}}
            ==References==
            {{more footnotes needed}}
            {{reflist}}"));

            Assert.That(Parsers.Conversions(@"Article {{efn|Converting at a rate of Kr 20 = £1}}
            ==References==
            {{no footnotes}}
            {{reflist}}"), Is.EqualTo(@"Article {{efn|Converting at a rate of Kr 20 = £1}}
            ==References==
            {{more footnotes needed}}
            {{reflist}}"));

            Assert.That(Parsers.Conversions(@"Article {{sfn|Smith|2004}}
            ==References==
            {{no footnotes|date=May 2012}}
            {{reflist}}"), Is.EqualTo(@"Article {{sfn|Smith|2004}}
            ==References==
            {{more footnotes needed|date=May 2012}}
            {{reflist}}"));

            Assert.That(Parsers.Conversions(@"Article {{sfn|Smith|2004}}
            ==References==
            {{no footnotes|BLP=yes|date=May 2012}}
            {{reflist}}"), Is.EqualTo(@"Article {{sfn|Smith|2004}}
            ==References==
            {{more footnotes needed|BLP=yes|date=May 2012}}
            {{reflist}}"));

            Assert.That(Parsers.Conversions(@"Article {{sfn|Smith|2004}}
            ==References==
            {{no footnotes|article=yes|date=May 2012}}
            {{reflist}}"), Is.EqualTo(@"Article {{sfn|Smith|2004}}
            ==References==
            {{more footnotes needed|article=yes|date=May 2012}}
            {{reflist}}"));

            const string NoChange = @"Article
            ==References==
            {{no footnotes}}";
            Assert.That(Parsers.Conversions(NoChange), Is.EqualTo(NoChange));

            const string NoFootnotesSection = @"Article <ref>A</ref>
            ==Sec==
            {{no footnotes|section}}
            ==References==
            {{reflist}}";

            Assert.That(Parsers.Conversions(NoFootnotesSection), Is.EqualTo(NoFootnotesSection));

            const string NoFootnotesReason = @"Article <ref>A</ref>
            ==Sec==
            {{no footnotes|some reason}}
            ==References==
            {{reflist}}";

            Assert.That(Parsers.Conversions(NoFootnotesReason), Is.EqualTo(NoFootnotesReason));

            const string NoChange2 = @"Article<ref name=A group=B>A</ref>
            ==References==
            {{no footnotes}}";
            Assert.That(Parsers.Conversions(NoChange2), Is.EqualTo(NoChange2));
        }

        [Test]
        public void ConversionTestsBLPUnsourced()
        {
            string correct = @"Foo
{{BLP unreferenced}}
[[Category:Living people]]", nochange = @"Foo
{{unreferenced}}";

            Assert.That(Parsers.Conversions(nochange + "\r\n" + @"[[Category:Living people]]"), Is.EqualTo(correct));

            Assert.That(Parsers.Conversions(correct), Is.EqualTo(correct));

            Assert.That(Parsers.Conversions(nochange), Is.EqualTo(nochange));

            nochange = @"Foo {{unreferenced|blah}}" + @"[[Category:Living people]]";
            Assert.That(Parsers.Conversions(nochange), Is.EqualTo(nochange), "no change when free-format text in unreferenced first argument");

            Assert.That(Parsers.Conversions(@"Foo
{{unreferenced|date=May 2010}}
[[Category:Living people]]"), Is.EqualTo(@"Foo
{{BLP unreferenced|date=May 2010}}
[[Category:Living people]]"));

            Assert.That(Parsers.Conversions(@"Foo
{{unreferenced|Date=May 2010}}
[[Category:Living people]]"), Is.EqualTo(@"Foo
{{BLP unreferenced|Date=May 2010}}
[[Category:Living people]]"));

            Assert.That(Parsers.Conversions(@"Foo
{{BLP unsourced|date=May 2010}}
{{unreferenced|date=May 2010}}
[[Category:Living people]]"), Is.EqualTo(@"Foo
{{BLP unsourced|date=May 2010}}

[[Category:Living people]]"));
            Assert.That(Parsers.Conversions(@"Foo
{{BLP unreferenced|date=May 2010}}
{{unreferenced|date=May 2010}}
[[Category:Living people]]"), Is.EqualTo(@"Foo
{{BLP unreferenced|date=May 2010}}

[[Category:Living people]]"));
        }

        [Test]
        public void ConversionTestsBLPUnsourcedSection()
        {
            string correct = @"Foo
{{BLP unreferenced section}}
[[Category:Living people]]", nochange = @"Foo
{{unreferenced section}}";

            Assert.That(Parsers.Conversions(nochange + "\r\n" + @"[[Category:Living people]]"), Is.EqualTo(correct));
            Assert.That(Parsers.Conversions(@"Foo
{{unreferenced section}}" + "\r\n" + @"[[Category:Living people]]"), Is.EqualTo(correct));

            Assert.That(Parsers.Conversions(correct), Is.EqualTo(correct));

            Assert.That(Parsers.Conversions(nochange), Is.EqualTo(nochange));
        }

        [Test]
        public void ConversionTestsBLPPrimarySources()
        {
            string correct = @"Foo
{{BLP primary sources}}
[[Category:Living people]]", nochange = @"Foo
{{primary sources}}";

            Assert.That(Parsers.Conversions(nochange + "\r\n" + @"[[Category:Living people]]"), Is.EqualTo(correct));
            Assert.That(Parsers.Conversions(@"Foo
{{primary sources}}" + "\r\n" + @"[[Category:Living people]]"), Is.EqualTo(correct));

            Assert.That(Parsers.Conversions(correct), Is.EqualTo(correct));

            Assert.That(Parsers.Conversions(nochange), Is.EqualTo(nochange));
        }

        [Test]
        public void ConversionTestsBLPOneSource()
        {
            string correct = @"Foo
{{BLP one source}}
[[Category:Living people]]", nochange = @"Foo
{{One source}}";

            Assert.That(Parsers.Conversions(nochange + "\r\n" + @"[[Category:Living people]]"), Is.EqualTo(correct));
            Assert.That(Parsers.Conversions(@"Foo
{{One source}}" + "\r\n" + @"[[Category:Living people]]"), Is.EqualTo(correct));

            Assert.That(Parsers.Conversions(correct), Is.EqualTo(correct));

            Assert.That(Parsers.Conversions(nochange), Is.EqualTo(nochange));
        }

        [Test]
        public void ConversionTestsSelfPublished()
        {
            string correct = @"Foo
{{BLP self-published}}
[[Category:Living people]]", nochange = @"Foo
{{self-published}}";

            Assert.That(Parsers.Conversions(nochange + "\r\n" + @"[[Category:Living people]]"), Is.EqualTo(correct), "BLP conv 1");
            Assert.That(Parsers.Conversions(@"Foo
{{self-published}}" + "\r\n" + @"[[Category:Living people]]"), Is.EqualTo(correct), "BLP conv 2");

            Assert.That(Parsers.Conversions(correct), Is.EqualTo(correct), "No change already correct");

            Assert.That(Parsers.Conversions(nochange), Is.EqualTo(nochange), "no change not BLP");
        }

        [Test]
        public void ConversionTestsNoFootnotes()
        {
            string correct = @"Foo
{{BLP no footnotes}}
[[Category:Living people]]", nochange = @"Foo
{{No footnotes}}";

            Assert.That(Parsers.Conversions(nochange + "\r\n" + @"[[Category:Living people]]"), Is.EqualTo(correct), "BLP conv 1");
            Assert.That(Parsers.Conversions(@"Foo
{{No footnotes}}" + "\r\n" + @"[[Category:Living people]]"), Is.EqualTo(correct), "BLP conv 2");

            Assert.That(Parsers.Conversions(correct), Is.EqualTo(correct), "No change already correct");

            Assert.That(Parsers.Conversions(nochange), Is.EqualTo(nochange), "no change not BLP");
        }

        [Test]
        public void ConversionTestsBLPSources()
        {
            string correct = @"Foo
{{BLP sources}}
[[Category:Living people]]", nochange = @"Foo
{{More citations needed}}";

            Assert.That(Parsers.Conversions(nochange + "\r\n" + @"[[Category:Living people]]"), Is.EqualTo(correct));
            Assert.That(Parsers.Conversions(nochange + "\r\n" + @"[[Category : Living people]]"), Is.EqualTo(correct.Replace(":", " : ")));
            Assert.That(Parsers.Conversions(@"Foo
{{More citations needed}}" + "\r\n" + @"[[Category:Living people]]"), Is.EqualTo(correct));

            Assert.That(Parsers.Conversions(correct), Is.EqualTo(correct));

            Assert.That(Parsers.Conversions(nochange), Is.EqualTo(nochange));

            nochange = @"Foo {{More citations needed|blah}}" + @"[[Category:Living people]]";
            Assert.That(Parsers.Conversions(nochange), Is.EqualTo(nochange), "no change when free-format text in More citations needed first argument");

            Assert.That(Parsers.Conversions(@"Foo
{{More citations needed|date=May 2010}}
[[Category:Living people]]"), Is.EqualTo(@"Foo
{{BLP sources|date=May 2010}}
[[Category:Living people]]"), "do conversion");

            Assert.That(Parsers.Conversions(@"Foo
{{BLP sources|Date=May 2010}}
{{More citations needed|Date=May 2010}}
[[Category:Living people]]"), Is.EqualTo(@"Foo
{{BLP sources|Date=May 2010}}

[[Category:Living people]]"), "when have existing BLP sources then remove More citations needed");

            Assert.That(Parsers.Conversions(@"Foo
{{multiple issues|
{{BLP sources|Date=May 2010}}
{{More citations needed|Date=May 2010}}
}}
[[Category:Living people]]"), Is.EqualTo(@"Foo
{{BLP sources|Date=May 2010}}
[[Category:Living people]]"), "when have existing BLP sources then remove More citations needed");

            Assert.That(Parsers.Conversions(@"Foo
{{More citations needed|section|date=May 2010}}
[[Category:Living people]]"), Is.EqualTo(@"Foo
{{BLP sources section|date=May 2010}}
[[Category:Living people]]"));
        }

        [Test]
        public void ConversionTestsInterwikiMigration()
        {
            #if DEBUG
            Variables.SetProjectSimple("en", ProjectEnum.wikia);
            Assert.That(Parsers.InterwikiConversions(@"[[zh-tw:foo]]"), Is.EqualTo(@"[[zh-tw:foo]]"));

            Variables.SetProjectSimple("en", ProjectEnum.wikipedia);
            Assert.That(Parsers.Conversions(@"{{msg:hello}}"), Is.EqualTo(@"{{hello}}"));
            Assert.That(Parsers.InterwikiConversions(@"[[zh-tw:foo]]"), Is.EqualTo(@"[[zh:foo]]"));
            Assert.That(Parsers.InterwikiConversions(@"[[nb:foo]]"), Is.EqualTo(@"[[no:foo]]"));
            Assert.That(Parsers.InterwikiConversions(@"[[dk:foo]]"), Is.EqualTo(@"[[da:foo]]"));
            #endif
        }
        [Test]
        public void PageNameTests()
        {
            Assert.That(Parsers.Conversions(@"{{PAGENAME}}"), Is.EqualTo(@"{{subst:PAGENAME}}"));
            Assert.That(Parsers.Conversions(@"{{PAGENAMEE}}"), Is.EqualTo(@"{{subst:PAGENAMEE}}"));
            Assert.That(Parsers.Conversions(@"{{template:PAGENAME}}"), Is.EqualTo(@"{{subst:PAGENAME}}"));
            Assert.That(Parsers.Conversions(@"{{BASEPAGENAME}}"), Is.EqualTo(@"{{subst:BASEPAGENAME}}"));

            Assert.That(Parsers.Conversions(@"{{DEFAULTSORT:{{PAGENAME}}}}"), Is.EqualTo(@"{{DEFAULTSORT:{{subst:PAGENAME}}}}"));

            const string RefPAGENAME = @"<ref> Some text {{PAGENAME}} here</ref>";

            Assert.That(Parsers.Conversions(RefPAGENAME), Is.EqualTo(RefPAGENAME), "No subst: within ref tags");
            Assert.That(Parsers.Conversions(@"<ref>a</ref> {{BASEPAGENAME}}"), Is.EqualTo(@"<ref>a</ref> {{subst:BASEPAGENAME}}"), "Subst oustide ref tags");

            const string IfEqPageName = @"{{#ifeq:{{PAGENAME}}|Thing 1|Thing 2}}";
            Assert.That(Parsers.Conversions(IfEqPageName), Is.EqualTo(IfEqPageName), "No subst: #ifeq template");
        }

        [Test]
        public void TestRemoveEmptyMultipleIssues()
        {
            Assert.That(parser.MultipleIssues(@"{{Multiple issues}}"), Is.Empty);
            Assert.That(parser.MultipleIssues(@"{{Multiple issues|section = y}}"), Is.Empty);
            Assert.That(parser.MultipleIssues(@"{{Multiple issues|section= y}}"), Is.Empty);
            Assert.That(parser.MultipleIssues(@"{{Multiple issues | section= y}}"), Is.Empty);
        }

        [Test]
        public void DeduplicateMaintenanceTags()
        {
            List<string> tags = new List<string>();
            tags.Add("{{orphan}}");
            tags.Add("{{orphan}}");

            List<string> tags2 = new List<string>();
            tags2.Add("{{orphan}}");

            Assert.That(Parsers.DeduplicateMaintenanceTags(tags), Is.EqualTo(tags2), "exact dupes");

            tags.Add("{{Orphan}}");
            Assert.That(Parsers.DeduplicateMaintenanceTags(tags), Is.EqualTo(tags2), "casing of template name");

            tags.Clear();
            tags.Add("{{orphan|date=May 2012}}");
            tags.Add("{{orphan|date=May 2012}}");
            tags2.Clear();
            tags2.Add("{{orphan|date=May 2012}}");
            Assert.That(Parsers.DeduplicateMaintenanceTags(tags), Is.EqualTo(tags2), "exact dupes with param");

            tags.Clear();
            tags.Add("{{orphan}}");
            tags.Add("{{orphan|date=May 2012}}");
            tags2.Clear();
            tags2.Add("{{orphan | date=May 2012}}");
            Assert.That(Parsers.DeduplicateMaintenanceTags(tags), Is.EqualTo(tags2), "only one with param");

            tags.Clear();
            tags.Add("{{orphan|date=May 2012}}");
            tags.Add("{{orphan|date=May 2012}}");
            tags.Add("{{cleanup|date=May 2012}}");
            tags2.Clear();
            tags2.Add("{{orphan|date=May 2012}}");
            tags2.Add("{{cleanup|date=May 2012}}");
            Assert.That(Parsers.DeduplicateMaintenanceTags(tags), Is.EqualTo(tags2), "Non-dupe tag retained");

            tags.Clear();
            tags.Add("{{orphan|date=May 2012}}");
            tags.Add("{{Orphan|date=May 2015}}");
            tags2.Clear();
            tags2.Add("{{orphan|date=May 2012}}");
            Assert.That(Parsers.DeduplicateMaintenanceTags(tags), Is.EqualTo(tags2), "use earlier date param");

            tags.Clear();
            tags.Add("{{orphan|date=May 2012}}");
            tags.Add("{{Orphan|date=May 2015|other=foo}}");
            tags2.Clear();
            tags2.Add("{{orphan|date=May 2012 | other=foo}}");
            Assert.That(Parsers.DeduplicateMaintenanceTags(tags), Is.EqualTo(tags2), "Take other params");

            tags.Clear();
            tags.Add("{{orphan|date=May 2012|other=bar}}");
            tags.Add("{{Orphan|date=May 2015|other=foo}}");
            tags2.Clear();
            tags2.Add("{{orphan|date=May 2012|other=bar}}");
            tags2.Add("{{Orphan|date=May 2015|other=foo}}");
            Assert.That(Parsers.DeduplicateMaintenanceTags(tags), Is.EqualTo(tags2), "cannot dedupe if conflicting parameters");

            tags.Clear();
            tags.Add("{{orphan|date=foo}}");
            tags.Add("{{Orphan|date=foo}}");
            tags2.Clear();
            tags2.Add("{{orphan|date=foo}}");
            Assert.That(Parsers.DeduplicateMaintenanceTags(tags), Is.EqualTo(tags2), "Take non-date date param if identical");

            tags.Clear();
            tags.Add("{{orphan|date=foo}}");
            tags.Add("{{Orphan|date=foo2}}");
            tags2.Clear();
            tags2.Add("{{orphan|date=foo}}");
            tags2.Add("{{Orphan|date=foo2}}");
            Assert.That(Parsers.DeduplicateMaintenanceTags(tags), Is.EqualTo(tags2), "Cannot process conflicting non-date date params");

            tags.Clear();
            tags.Add("{{notability|date=May 2012}}");
            tags.Add("{{notability|Biography|date=May 2015}}");
            tags2.Clear();
            tags2.Add("{{notability|Biography|date=May 2012}}");
            Assert.That(Parsers.DeduplicateMaintenanceTags(tags), Is.EqualTo(tags2), "Retain template argument, in second tag");

            tags.Clear();
            tags.Add("{{notability|Biography|date=May 2012}}");
            tags.Add("{{notability|date=May 2015}}");
            tags2.Clear();
            tags2.Add("{{notability|Biography|date=May 2012}}");
            Assert.That(Parsers.DeduplicateMaintenanceTags(tags), Is.EqualTo(tags2), "Retain template argument, in first tag");

            tags.Clear();
            tags.Add("{{Expert-subject|Science fiction|date=February 2009}}");
            tags.Add("{{Expert-subject|Military history/Weaponry task force|date=February 2009}}");
            Assert.That(Parsers.DeduplicateMaintenanceTags(tags), Is.EqualTo(tags), "Retain template argument in both");
        }
    }

    [TestFixture]
    public class MultipleIssuesNewTests : RequiresParser
    {
        [Test]
        public void MultipleIssuesNewZerothTagLines()
        {
            Assert.That(parser.MultipleIssues(@""), Is.Empty);

            Assert.That(parser.MultipleIssues(@"{{wikify}} {{unreferenced}} {{POV}}"), Is.EqualTo(@"{{Multiple issues|
{{wikify}}
{{unreferenced}}
{{POV}}
}}"), "preserves tag order adding new tags");

            Assert.That(parser.MultipleIssues(@"{{wikify}}{{unreferenced}}{{POV}}
==hello=="), Is.EqualTo(@"{{Multiple issues|
{{wikify}}
{{unreferenced}}
{{POV}}
}}

==hello=="), "takes tags from same line");

            Assert.That(parser.MultipleIssues(@"{{wikify}}
{{unreferenced}}
{{POV}}
==hello=="), Is.EqualTo(@"{{Multiple issues|
{{wikify}}
{{unreferenced}}
{{POV}}
}}



==hello=="), "takes tags from separate lines, takes tags without dates");

            Assert.That(parser.MultipleIssues(@"{{wikify}}
{{unreferenced}}
{{POV}} Article starts.

==hello=="), Is.EqualTo(@"{{Multiple issues|
{{wikify}}
{{unreferenced}}
{{POV}}
}}


Article starts.

==hello=="), "Tag trailing whitespace handling");
        }
        
        [Test]
        public void MultipleIssuesNewZerothTagParameters()
        {
            Assert.That(parser.MultipleIssues(@"{{underlinked|date=May 2012}}{{unreferenced}}{{POV}}
==hello=="), Is.EqualTo(@"{{Multiple issues|
{{underlinked|date=May 2012}}
{{unreferenced}}
{{POV}}
}}

==hello=="), "takes tags with dates");

            Assert.That(parser.MultipleIssues(@"{{underlinked|date=May 2012|reason=x}}{{unreferenced}}{{POV}}
==hello=="), Is.EqualTo(@"{{Multiple issues|
{{underlinked|date=May 2012|reason=x}}
{{unreferenced}}
{{POV}}
}}

==hello=="), "takes tags with extra parameters");
        }
        
        [Test]
        public void MultipleIssuesNewTagZeroth()
        {
            Assert.That(parser.MultipleIssues(@"{{underlinked|date=May 2012}}{{unreferenced}}
Text
{{POV}}
==hello=="), Is.EqualTo(@"{{Multiple issues|
{{underlinked|date=May 2012}}
{{unreferenced}}
{{POV}}
}}

Text

==hello=="), "takes tags from anywhere in zeroth section");

            Assert.That(parser.MultipleIssues(@"Text
{{underlinked|date=May 2012}}{{unreferenced}}{{POV}}
==hello=="), Is.EqualTo(@"{{Multiple issues|
{{underlinked|date=May 2012}}
{{unreferenced}}
{{POV}}
}}
Text

==hello=="), "takes tags from anywhere in zeroth section: all after");
            
        }

        [Test]
        public void MergeMultipleMI()
        {
            Assert.That(parser.MultipleIssues(@"{{Multiple issues|
{{unreferenced}}
{{POV}}
}}

{{Multiple issues|
{{cleanup}}
{{wikify}}
}}

Foo"), Is.EqualTo(@"{{Multiple issues|
{{unreferenced}}
{{POV}}
{{cleanup}}
{{wikify}}
}}



Foo"), "merge multiple MI");

            Assert.That(parser.MultipleIssues(@"{{Multiple issues|
{{unreferenced}}
{{POV}}
}}

{{Multiple issues|
}}

Foo"), Is.EqualTo(@"{{Multiple issues|
{{unreferenced}}
{{POV}}
}}



Foo"), "merge multiple MI, second empty");

            Assert.That(parser.MultipleIssues(@"{{Multiple issues|
{{unreferenced}}
}}

{{Multiple issues|
{{POV}}
}}

Foo"), Is.EqualTo(@"{{Multiple issues|
{{unreferenced}}
{{POV}}
}}



Foo"), "merge multiple MI, one tag");

            Assert.That(parser.MultipleIssues(@"{{Multiple issues|
{{unreferenced}}
}}

{{Multiple issues|
{{unreferenced}}
{{POV}}
}}

Foo"), Is.EqualTo(@"{{Multiple issues|
{{unreferenced}}
{{POV}}
}}



Foo"), "merge multiple MI, dedupe tag");

            Assert.That(parser.MultipleIssues(@"{{Multiple issues|
{{unreferenced|date=June 2012}}
}}

{{Multiple issues|
{{unreferenced|date=May 2015}}
{{POV}}
}}

Foo"), Is.EqualTo(@"{{Multiple issues|
{{unreferenced|date=June 2012}}
{{POV}}
}}



Foo"), "merge multiple MI, dedupe tag, using earlier date");

            Assert.That(parser.MultipleIssues(@"{{Multiple issues|
{{unreferenced}}
}}

{{Multiple issues|
{{unreferenced}}
}}

Foo"), Is.EqualTo(@"{{unreferenced}}



Foo"), "merge multiple MI, only dupe tags, convert to standalone tag");

            string NoChange = @"{{multiple issues|
{{unreferenced}}
{{POV}}
}}

{{Multiple issues|section|
{{cleanup}}
{{wikify}}
}}

Foo";
            Assert.That(parser.MultipleIssues(NoChange), Is.EqualTo(NoChange), "no change when one MI is a section one");
        }
        
        [Test]
        public void MultipleIssuesNewZerothExistingMINotChanged()
        {
            const string ThreeTagNew = @"{{multiple issues|
{{underlinked|date=May 2012}}
{{unreferenced}}
{{POV}}
}}
Text

==hello==", TwoTagNew = @"{{multiple issues|
{{unreferenced}}
{{POV}}
}}
Text

==hello==";
            Assert.That(parser.MultipleIssues(ThreeTagNew), Is.EqualTo(ThreeTagNew), "no change to existing 3-tag MI new style");
            Assert.That(parser.MultipleIssues(TwoTagNew), Is.EqualTo(TwoTagNew), "no change to existing 2-tag MI new style");            
        }
        
        [Test]
        public void MultipleIssuesNewZerothExistingMIMoreTags()
        {
            Assert.That(parser.MultipleIssues(@"{{multiple issues|
{{wikify|date=May 2012}}
{{peacock}}
{{POV}}
}}
{{unreferenced}}

==hello=="), Is.EqualTo(@"{{multiple issues|
{{wikify|date=May 2012}}
{{peacock}}
{{POV}}
{{unreferenced}}
}}


==hello=="), "adds tags to existing MI, MI new style");

            Assert.That(parser.MultipleIssues(@"{{multiple issues|
{{wikify|date=May 2012}}
{{peacock}}
{{advert}}
}}
{{POV}}
{{unreferenced}}

==hello=="), Is.EqualTo(@"{{multiple issues|
{{wikify|date=May 2012}}
{{peacock}}
{{advert}}
{{POV}}
{{unreferenced}}
}}



==hello=="), "adds tags to existing MI, MI new style");

            Assert.That(parser.MultipleIssues(@"{{unreferenced}}{{multiple issues|
{{wikify|date=May 2012}}
{{peacock}}
{{POV}}
}}

==hello=="), Is.EqualTo(@"{{multiple issues|
{{wikify|date=May 2012}}
{{peacock}}
{{POV}}
{{unreferenced}}
}}

==hello=="), "adds tags to existing MI, MI new style, tags before MI");

            Assert.That(parser.MultipleIssues(@"{{multiple issues|
{{wikify|date=May 2012}}
}}
{{unreferenced}}

==hello=="), Is.EqualTo(@"{{multiple issues|
{{wikify|date=May 2012}}
{{unreferenced}}
}}


==hello=="), "adds 1 tag to existing MI with 1 tag, MI new style");

            Assert.That(parser.MultipleIssues(@"{{multiple issues|
{{wikify|date=May 2012}}
{{peacock}}
{{POV}}
{{unreferenced}}
}}
{{unreferenced}}

==hello=="), Is.EqualTo(@"{{multiple issues|
{{wikify|date=May 2012}}
{{peacock}}
{{POV}}
{{unreferenced}}
}}


==hello=="), "duplicate tags not added");
            
        }
        
        [Test]
        public void MultipleIssuesNewZerothSingleTag()
        {
            Assert.That(parser.MultipleIssues(@"{{multiple issues|
{{wikify}}
}}"), Is.EqualTo(@"{{wikify}}"), "converts new style 1-tag MI to single template");
            Assert.That(parser.MultipleIssues(@"{{multiple issues|
{{wikify|date=May 2012}}
}}"), Is.EqualTo(@"{{wikify|date=May 2012}}"), "converts new style 1-tag MI to single template, tag with date");
            Assert.That(parser.MultipleIssues(@"{{multiple issues|
{{wikify|reason=links}}
}}"), Is.EqualTo(@"{{wikify|reason=links}}"), "converts new style 1-tag MI to single template, tag with extra parameter");
        }

        [Test]
        public void MultipleIssuesNewSectionTagLines()
        {
            Assert.That(parser.MultipleIssues(@""), Is.Empty);

            Assert.That(parser.MultipleIssues(@"==sec==
{{wikify section}} {{expand section}} {{POV-section}}"), Is.EqualTo(@"==sec==
{{Multiple issues|section=yes|
{{wikify section}}
{{expand section}}
{{POV-section}}
}}"), "preserves tag order adding new tags");

            Assert.That(parser.MultipleIssues(@"==sec==
{{wikify section}}{{expand section}}{{POV-section}}
A
==hello=="), Is.EqualTo(@"==sec==
{{Multiple issues|section=yes|
{{wikify section}}
{{expand section}}
{{POV-section}}
}}
A
==hello=="), "takes tags from same line");

            Assert.That(parser.MultipleIssues(@"==sec==
{{wikify section}}
{{expand section}}
{{POV-section}}
A
==hello=="), Is.EqualTo(@"==sec==
{{Multiple issues|section=yes|
{{wikify section}}
{{expand section}}
{{POV-section}}
}}
A
==hello=="), "takes tags from separate lines, takes tags without dates");
        }
        
        [Test]
        public void MultipleIssuesNewSectionTagParameters()
        {
            Assert.That(parser.MultipleIssues(@"==sec==
{{wikify section|date=May 2012}}{{expand section}}{{POV-section}}
A
==hello=="), Is.EqualTo(@"==sec==
{{Multiple issues|section=yes|
{{wikify section|date=May 2012}}
{{expand section}}
{{POV-section}}
}}
A
==hello=="), "takes tags with dates");

            Assert.That(parser.MultipleIssues(@"==sec==
{{wikify section|date=May 2012|reason=x}}{{expand section}}{{POV-section}}
A
==hello=="), Is.EqualTo(@"==sec==
{{Multiple issues|section=yes|
{{wikify section|date=May 2012|reason=x}}
{{expand section}}
{{POV-section}}
}}
A
==hello=="), "takes tags with extra parameters");
        }
        
        [Test]
        public void MultipleIssuesNewTagSection()
        {
            const string After = @"==sec==
{{wikify section|date=May 2012}}
Text
{{POV-section}}
==hello==";
            Assert.That(parser.MultipleIssues(After), Is.EqualTo(After), "Does not take tags from after text in section");

            Assert.That(parser.MultipleIssues(@"==sec==
{{wikify section|date=May 2012}}{{expand section}}{{POV-section}}
Text
{{unreferenced section}}
==hello=="), Is.EqualTo(@"==sec==
{{Multiple issues|section=yes|
{{wikify section|date=May 2012}}
{{expand section}}
{{POV-section}}
}}
Text
{{unreferenced section}}
==hello=="), "takes tags before tag in section");
        }
        
        [Test]
        public void MultipleIssuesNewSectionExistingMINotChanged()
        {
            const string ThreeTagNew = @"==sec==
{{multiple issues|section=yes|
{{wikify|date=May 2012}}
{{expand section}}
{{POV-section}}
}}
Text

==hello==", TwoTagNew = @"==sec==
{{multiple issues|section=yes|
{{expand section}}
{{POV-section}}
}}
Text

==hello==";
            Assert.That(parser.MultipleIssues(ThreeTagNew), Is.EqualTo(ThreeTagNew), "no change to existing 3-tag MI new style");
            Assert.That(parser.MultipleIssues(TwoTagNew), Is.EqualTo(TwoTagNew), "no change to existing 2-tag MI new style");            
        }
        
        [Test]
        public void MultipleIssuesNewSectionExistingMIMoreTags()
        {
            Assert.That(parser.MultipleIssues(@"==sec==
{{multiple issues|
{{wikify section|date=May 2012}}
{{expand section}}
{{POV-section}}
}}
{{unreferenced section}}

==hello=="), Is.EqualTo(@"==sec==
{{multiple issues|
{{wikify section|date=May 2012}}
{{expand section}}
{{POV-section}}
{{unreferenced section}}
}}
==hello=="), "adds tags to existing MI, MI new style");

            Assert.That(parser.MultipleIssues(@"==sec==
{{multiple issues|section=yes|
{{wikify section|date=May 2012}}
{{expand section}}
{{POV-section}}
}}
{{unreferenced section}}

==hello=="), Is.EqualTo(@"==sec==
{{multiple issues|section=yes|
{{wikify section|date=May 2012}}
{{expand section}}
{{POV-section}}
{{unreferenced section}}
}}
==hello=="), "adds tags to existing MI, MI new style");

            Assert.That(parser.MultipleIssues(@"==sec==
{{unreferenced section}}{{multiple issues|
{{wikify section|date=May 2012}}
{{expand section}}
{{POV-section}}
}}

==hello=="), Is.EqualTo(@"==sec==
{{multiple issues|
{{wikify section|date=May 2012}}
{{expand section}}
{{POV-section}}
{{unreferenced section}}
}}
==hello=="), "adds tags to existing MI, MI new style, tags before MI");

            Assert.That(parser.MultipleIssues(@"==sec==
{{multiple issues|
{{wikify section|date=May 2012}}
}}
{{unreferenced section}}

==hello=="), Is.EqualTo(@"==sec==
{{multiple issues|
{{wikify section|date=May 2012}}
{{unreferenced section}}
}}
==hello=="), "adds 1 tag to existing MI with 1 tag, MI new style");

            Assert.That(parser.MultipleIssues(@"==sec==
{{multiple issues|
{{wikify section|date=May 2012}}
{{expand section}}
{{POV-section}}
{{unreferenced section}}
}}
{{unreferenced section}}

==hello=="), Is.EqualTo(@"==sec==
{{multiple issues|
{{wikify section|date=May 2012}}
{{expand section}}
{{POV-section}}
{{unreferenced section}}
}}
==hello=="), "duplicate tags not added");
        }
        
        [Test]
        public void MultipleIssuesNewSectionSingleTag()
        {
            Assert.That(parser.MultipleIssues(@"==sec==
{{multiple issues|
{{wikify section}}
}}"), Is.EqualTo(@"==sec==
{{wikify section}}"), "converts new style 1-tag MI to single template");
            Assert.That(parser.MultipleIssues(@"==sec==
{{multiple issues|1=
{{wikify section}}
}}"), Is.EqualTo(@"==sec==
{{wikify section}}"), "converts new style 1-tag MI to single template, removing 1=");
            Assert.That(parser.MultipleIssues(@"==sec==
{{multiple issues|section=yes|
{{wikify section}}
}}"), Is.EqualTo(@"==sec==
{{wikify section}}"), "converts new style 1-tag MI to single template");
            Assert.That(parser.MultipleIssues(@"==sec==
{{multiple issues|
{{wikify section|date=May 2012}}
}}"), Is.EqualTo(@"==sec==
{{wikify section|date=May 2012}}"), "converts new style 1-tag MI to single template, tag with date");
            Assert.That(parser.MultipleIssues(@"==sec==
{{multiple issues|
{{wikify section|reason=links}}
}}"), Is.EqualTo(@"==sec==
{{wikify section|reason=links}}"), "converts new style 1-tag MI to single template, tag with extra parameter");
        }

        [Test]
        public void MultipleIssuesNewCleanup()
        {
            Assert.That(parser.MultipleIssues(@"{{multiple issues|
{{wikify}}

{{underlinked}}
{{POV}}
}}"), Is.EqualTo(@"{{multiple issues|
{{wikify}}
{{underlinked}}
{{POV}}
}}"), "Cleans up excess newlines");

            Assert.That(parser.MultipleIssues(@"{{multiple issues|
{{POV}}
{{wikify}}
}}
{{wikify}}"), Is.EqualTo(@"{{multiple issues|
{{POV}}
{{wikify}}
}}"), "De-duplicates tags");

            Assert.That(parser.MultipleIssues(@"{{multiple issues|
{{POV}}
{{wikify|date=May 2012}}
}}
{{wikify|date=May 2015}}"), Is.EqualTo(@"{{multiple issues|
{{POV}}
{{wikify|date=May 2012}}
}}"), "De-duplicates tags, takes earlier date");

            Assert.That(parser.MultipleIssues(@"{{multiple issues|
{{POV}}
{{wikify|date=May 2012}}
{{wikify|date=May 2015}}
}}"), Is.EqualTo(@"{{multiple issues|
{{POV}}
{{wikify|date=May 2012}}
}}"), "De-duplicates tags within MI, takes earlier date");

            Assert.That(parser.MultipleIssues(@"{{multiple issues|
{{wikify}}
}}
{{wikify}}"), Is.EqualTo(@"{{wikify}}"), "De-duplicates tags, removes MI if only one tag remains");
        }
    }

    [TestFixture]
    public class OtherParserTests
    {
        [Test]
        public void HasSicTag()
        {
            Assert.IsTrue(Parsers.HasSicTag("now helo [sic] there"));
            Assert.IsTrue(Parsers.HasSicTag("now helo[sic] there"));
            Assert.IsTrue(Parsers.HasSicTag("now helo (sic) there"));
            Assert.IsTrue(Parsers.HasSicTag("now helo {sic} there"));
            Assert.IsTrue(Parsers.HasSicTag("now helo [Sic] there"));
            Assert.IsTrue(Parsers.HasSicTag("now {{sic|helo}} there"));
            Assert.IsTrue(Parsers.HasSicTag("now {{sic|hel|o}} there"));
            Assert.IsTrue(Parsers.HasSicTag("now {{typo|helo}} there"));

            Assert.IsFalse(Parsers.HasSicTag("now sickened by"));
            Assert.IsFalse(Parsers.HasSicTag("now helo sic there"));
        }
    }
}
