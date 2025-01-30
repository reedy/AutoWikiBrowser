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
    public class CategoryTests : RequiresParser
    {
        public GenfixesTestsBase genFixes  = new GenfixesTestsBase();

        [Test]
        public void TestYearRangesCategories()
        {
            genFixes.AssertNotChanged(@"now foo
[[Category:Abc (2004-present)]]");
            
            genFixes.AssertChange(@"now abc (2004-present) was
now foo
[[Category:Abc (2004-present)]]", @"now abc (2004–present) was
now foo
[[Category:Abc (2004-present)]]");
        }

        [Test]
        public void FixPeopleCategoriesBirth()
        {
            // birth
            const string a1 = @"'''Fred Smith''' (born 1960) is a bloke. {{Infobox actor}}";
            const string b2 = @"[[Category:1960 births]]";
            Assert.That(Parsers.FixPeopleCategories(a1, "foo"), Is.EqualTo(a1 + "\r\n" + b2));

            const string a2 = a1 + "\r\n" + @"[[Category:1990 deaths]]";
            Assert.That(Parsers.FixPeopleCategories(a2, "foo"), Is.EqualTo(a2 + "\r\n" + b2));

            const string b5 = @"Some words {{birth date and age|1960|01|9}}";
            Assert.That(Parsers.FixPeopleCategories(b5, "foo"), Is.EqualTo(b5 + "\r\n" + b2));

            const string b6 = @"'''Fred Lerdahl''' (born [[March 10]] [[1960]]) [[Category:Living people]]";
            Assert.That(Parsers.FixPeopleCategories(b6, "foo"), Is.EqualTo(b6 + "\r\n" + b2));

            // catch living person and very old birth date
            const string b6a = @"'''Fred Lerdahl''' (born [[March 10]] [[1860]]) [[Category:Living people]]";
            Assert.That(Parsers.FixPeopleCategories(b6a, "foo"), Is.EqualTo(b6a));

            const string b7 = @"'''William Arthur O'Donnell''' (born May 4, 1960 in [[Springhill, Nova Scotia]], Canada) is [[Category:Living people]]";
            Assert.That(Parsers.FixPeopleCategories(b7, "foo"), Is.EqualTo(b7 + "\r\n" + b2));

            const string b8 = @"'''Burrell Carver Smith''' (born [[December 16]], [[1960]] in upstate New York) [[Category:Living people]]";
            Assert.That(Parsers.FixPeopleCategories(b8, "foo"), Is.EqualTo(b8 + "\r\n" + b2));

            const string b9 = @"'''Fredro Starr''' (born '''Fredro Scruggs''' on [[April 18]] [[1960]] in [[Jamaica, Queens]]) is an [[Category:Living people]]";
            Assert.That(Parsers.FixPeopleCategories(b9, "foo"), Is.EqualTo(b9 + "\r\n" + b2));

            const string b10 = @"'''Phillip Rhodes''' (born [[May 26]], [[1960]]) was a [[Category:Living people]]";
            Assert.That(Parsers.FixPeopleCategories(b10, "foo"), Is.EqualTo(b10 + "\r\n" + b2));

            // no matches when two birth dates in article
            const string b11 = @"'''Kid 'n Play''' was a [[hip-hop music|hip-hop]] and [[comedy]] duo from [[New York City]] that was popular in the late 1980s and early 1990s. The duo comprised '''[[Christopher Kid Reid]]''' (born [[April 5]] [[1964]] in [[The Bronx|The Bronx, New York City]])
and '''[[Christopher Martin (entertainer)|Christopher Play Martin]]''' (born [[July 10]] [[1962]] in [[Queens, New York City]]). Besides their successful musical careers, Kid 'n Play are also notable for branching out into acting. [[Category:Living people]]";
            Assert.That(Parsers.FixPeopleCategories(b11, "foo"), Is.EqualTo(b11));

            const string b12 = @"{{Infobox actor
| name            = Gianni Capaldi
| birthdate       = <!-- {{Birth date and age|YYYY|MM|DD}} --> age 34 (as of 12 January 2010)<ref>*  Matheson, Shelley.</ref>
| birthplace      = [[Motherwell]], [[Scotland]]<br>{{Citation needed|date=September 2010}}
}}";
            Assert.That(Parsers.FixPeopleCategories(b12, "foo"), Is.EqualTo(b12));

            // Date of birth missing
            string m1 = @"'''Fred Smith''' (born 1960) is a bloke. {{Infobox actor}}
[[Category:Date of birth missing]]";
            Assert.That(Parsers.FixPeopleCategories(m1, "foo"), Is.EqualTo(m1 + "\r\n" + b2), "Date of birth missing cat retained when only yob");

            m1 = @"'''Fred Smith''' {{birth date and age|1960||}} is a bloke. {{Infobox actor}}
[[Category:Date of birth missing]]";
            Assert.That(Parsers.FixPeopleCategories(m1, "foo"), Is.EqualTo(m1 + "\r\n" + b2), "Date of birth missing cat retained when only yob in bda");

            const string DobMCat = @"[[Category:Date of birth missing]]";
            const string m2 = @"'''Fred Smith''' {{birth date and age|1960|01|9}} is a bloke. {{Infobox actor}}";
            Assert.That(Parsers.FixPeopleCategories(m2 + "\r\n" + DobMCat, "foo"), Is.EqualTo(m2 + "\r\n" + b2), "Date of birth missing cat removed when full date");

            const string DobMCatL = @"[[Category:Date of birth missing (living people)|Foo]]";
            Assert.That(Parsers.FixPeopleCategories(m2 + "\r\n" + DobMCatL, "foo"), Is.EqualTo(m2 + "\r\n" + b2), "Date of birth missing (living people) cat removed when full date");
        }

        [Test]
        public void FixPeopleCategoriesUncat()
        {
            const string a1 = @"'''Fred Smith''' (born 1960) is a bloke. {{Infobox actor}} {{Uncat|date=May 2010}}";
            const string b2 = @"[[Category:1960 births]]";
            Assert.That(Parsers.FixPeopleCategories(a1, "foo"), Is.EqualTo(a1.Replace("Uncat", "Improve categories") + "\r\n" + b2), "renames uncat to cat improve when cats added");

            Assert.That(Parsers.FixPeopleCategories(a1 + b2, "foo"), Is.EqualTo(a1 + b2), "no uncat renaming when cats not added");
            Assert.That(Parsers.FixPeopleCategories(a1.Replace(@" {{Uncat|date=May 2010}}", "") + "{{Cat improve}}", "foo"), Is.EqualTo(a1.Replace(@" {{Uncat|date=May 2010}}", "") + "{{Cat improve}}" + "\r\n" + b2), "no Cat improve change when cat improve already there");
        }

        [Test]
        public void FixPeopleCategoriesDeath()
        {
            const string b2 = @"[[Category:1960 births]]";

            // death
            const string a3 = @"'''Fred Smith''' (died 1960) is a bloke. {{Infobox actor}}";
            const string b3 = @"[[Category:1960 deaths]]";
            Assert.That(Parsers.FixPeopleCategories(a3, "foo"), Is.EqualTo(a3 + "\r\n" + b3));

            const string a4 = a3 + "\r\n" + @"[[Category:1990 births]]";
            Assert.That(Parsers.FixPeopleCategories(a4, "foo"), Is.EqualTo(a4 + "\r\n" + b3));

            const string d = @"'''Fred Smith''' (born 11 May 1950 - died 17 August 1990) is a bloke.
[[Category:1960 births|Smith, Fred]]";
            Assert.That(Parsers.FixPeopleCategories(d, "foo"), Is.EqualTo(d + "\r\n" + @"[[Category:1990 deaths|Smith, Fred]]"));

            const string d2 = @"'''Johnny Sandon''' (originally named '''Billy Beck''') (born in 1960, in Lıverpool, Lancashire died 23 December 1990) was {{Infobox actor}}";
            Assert.That(Parsers.FixPeopleCategories(d2, "foo"), Is.EqualTo(d2 + "\r\n" + b2 + "\r\n" + @"[[Category:1990 deaths]]"));

            // BC death
            const string d3 = @"'''Aeacides''' ({{lang-el|Aἰακίδης}}; died 313 BC), king {{Infobox actor}}";
            Assert.That(Parsers.FixPeopleCategories(d3, "foo"), Is.EqualTo(d3 + "\r\n" + @"[[Category:313 BC deaths]]"));

            const string d4 = @"Some words {{death date and age|1960|01|9}}";
            Assert.That(Parsers.FixPeopleCategories(d4, "foo"), Is.EqualTo(d4 + "\r\n" + @"[[Category:1960 deaths]]"));

            // no matches if not identified as born
            const string b1 = @"'''Fred Smith''' is a bloke.";
            Assert.That(Parsers.FixPeopleCategories(b1, "foo"), Is.EqualTo(b1));

            const string d5 = @"Some words {{death date and age|1960|01|9}}";
            Assert.That(Parsers.FixPeopleCategories(d4 + @"[[Category:Year of death missing]]", "foo"), Is.EqualTo(d5 + @"[[Category:1960 deaths]]"));

            const string d6 = @"'''Foo''' (died 960) was {{Infobox actor}}";
            Assert.That(Parsers.FixPeopleCategories(d6 + @"[[Category:Year of death missing]]", "Foo"), Is.EqualTo(d6 + @"[[Category:960 deaths]]"));
        }

        [Test]
        public void FixPeopleCategoriesAlreadyCategorized()
        {
            // no matches if already categorised
            const string a = @"'''Fred Smith''' (born 1960) is a bloke.
[[Category:1960 births]]
[[Category:1990 deaths]]";
            const string b = @"'''Fred Smith''' is a bloke.
[[Category:1960 births]]
[[Category:1990 deaths]]";
            const string c = @"'''Fred Smith''' is a bloke.
[[Category:1960 births|Smith, Fred]]
[[Category:1990 deaths|Smith, Fred]]";
            const string e = @"'''Fred Smith''' (born [[1950]]) is a bloke.
[[Category:1950 births|Smith, Fred]]
{{Recentlydeceased}}";
            const string f = @"'''Fred Smith''' (born 1950) is a bloke.
[[Category:1950 births|Smith, Fred]]
{{recentlydeceased}}";
            const string g = @"'''Fred Smith''' (born 1950) is a bloke.
[[Category:1950 births|Smith, Fred]]
[[Category:Year of death missing]]";

            Assert.That(Parsers.FixPeopleCategories(a, "foo"), Is.EqualTo(a));
            Assert.That(Parsers.FixPeopleCategories(b, "foo"), Is.EqualTo(b));
            Assert.That(Parsers.FixPeopleCategories(c, "foo"), Is.EqualTo(c));
            Assert.That(Parsers.FixPeopleCategories(e, "foo"), Is.EqualTo(e));
            Assert.That(Parsers.FixPeopleCategories(f, "foo"), Is.EqualTo(f));
            Assert.That(Parsers.FixPeopleCategories(g, "foo"), Is.EqualTo(g));

            const string h = @"'''Fred Smith''' (born 1950) is a bloke.
{{}}
[[:Category:1950 births|Smith, Fred]]";
            Assert.That(Parsers.FixPeopleCategories(h, "User:Foo"), Is.EqualTo(h));
        }

        [Test]
        public void FixPeopleCategoriesYOBUncertain()
        {
            // year of birth uncertain
            const string u = "\r\n" + @"[[Category:Year of birth uncertain]]";

            const string u2 = @"'''Charles Meik''' (born around 1330 in [[Ghent]] - [[22 July]] [[1387]]) {{Infobox actor}}";
            Assert.That(Parsers.FixPeopleCategories(u2, "foo"), Is.EqualTo(u2 + u + @"
[[Category:1387 deaths]]"));

            const string u2a = @"'''Charles Meik''' (born either 1330 or 1331 in [[Ghent]] - [[22 July]] [[1387]]) {{Infobox actor}}";
            Assert.That(Parsers.FixPeopleCategories(u2a, "foo"), Is.EqualTo(u2a + u + @"
[[Category:1387 deaths]]"));

            const string u2b = @"'''Charles Meik''' (born ~1330 in [[Ghent]] - [[22 July]] [[1387]]) {{Infobox actor}}";
            Assert.That(Parsers.FixPeopleCategories(u2b, "foo"), Is.EqualTo(u2b + u + @"
[[Category:1387 deaths]]"));

            const string u3 = @"'''Yusuf Ibn Muhammad Ibn Yusuf al-Fasi''' ([[1530]]/[[1531|31]]{{Fact|date=February 2007}} in [[Ksar-el-Kebir]], [[Morocco]] – 14 August [[1604]] in [[Fes, Morocco]]) {{Infobox actor}}";
            Assert.That(Parsers.FixPeopleCategories(u3, "foo"), Is.EqualTo(u3 + u + @"
[[Category:1604 deaths]]"));

            // no matches when approximate year of birth
            const string b12 = @"'''Judith Victor Grabiner''' (born about 1938) is {{Infobox actor}}";
            Assert.That(Parsers.FixPeopleCategories(b12, "foo"), Is.EqualTo(b12 + u));

            const string b13 = @"'''Judith Victor Grabiner''' (born circa 1938) is {{Infobox actor}}";
            Assert.That(Parsers.FixPeopleCategories(b13, "foo"), Is.EqualTo(b13 + u));

            const string b14 = @"'''Judith Victor Grabiner''' (born before 1938) is {{Infobox actor}}";
            Assert.That(Parsers.FixPeopleCategories(b14, "foo"), Is.EqualTo(b14 + u));

            const string b15 = @"'''Judith Victor Grabiner''' (born 1938 or 1939) is {{Infobox actor}}";
            Assert.That(Parsers.FixPeopleCategories(b15, "foo"), Is.EqualTo(b15 + u));

            // born BC
            const string b16 = @"'''Phillipus Rhodicus''' (born 220 BC) was a {{Infobox actor}}";
            Assert.That(Parsers.FixPeopleCategories(b16, "foo"), Is.EqualTo(b16 + "\r\n" + @"[[Category:220 BC births]]"));

            // no change: birth date not present so not 'uncertain'
            const string n1 = @"'''Thomas F. Goreau''' (born in [[Germany]], died 1970 in [[Jamaica]]) was [[Category:1970 deaths]]";
            Assert.That(Parsers.FixPeopleCategories(n1, "foo"), Is.EqualTo(n1));

            const string n2 = @"'''Charles Meik''' (born? - 1923) was an {{Infobox actor}}";
            Assert.That(Parsers.FixPeopleCategories(n2, "foo"), Is.EqualTo(n2));

            const string n2a = @"'''Anatoly Rasskazov''' (born 1960(?)) was {{Infobox actor}}";
            Assert.That(Parsers.FixPeopleCategories(n2a, "foo"), Is.EqualTo(n2a));

            // no changes
            const string n3 = @"'''Johannes Widmann''' (born c. 1460 in [[Cheb|Eger]]; died after 1498 in [[Leipzig]]) [[Category:1460s births]]";
            Assert.That(Parsers.FixPeopleCategories(n3, "foo"), Is.EqualTo(n3));

            const string n4 = @"'''John Hulme''' (born circa 1970) is [[Category:Living people]]
[[Category:Year of birth missing (living people)]]";

            Assert.That(Parsers.FixPeopleCategories(n4, "foo"), Is.EqualTo(n4));

            const string n4a = @"'''John Hulme''' (born c.1970) is [[Category:Living people]]
[[Category:Year of birth missing (living people)]]";

            Assert.That(Parsers.FixPeopleCategories(n4a, "foo"), Is.EqualTo(n4a));

            const string n4b = @"'''John Hulme''' (born C.1970) is [[Category:Living people]]
[[Category:Year of birth missing (living people)]]";

            Assert.That(Parsers.FixPeopleCategories(n4b, "foo"), Is.EqualTo(n4b));
        }

        [Test]
        public void FixPeopleCategoriesLimits()
        {
            const string b2 = @"[[Category:1960 births]]";
            const string u = "\r\n" + @"[[Category:Year of birth uncertain]]";
            // don't use born info if after died info in text
            const string n5 = @"'''Alexander II''' ({{lang-ka|ალექსანდრე II, '''''Aleksandre II'''''}}) (died [[April 1]], [[1510]]) was a.
* Prince David (born 1505)
[[Category:1510 deaths]]";
            Assert.That(Parsers.FixPeopleCategories(@"'''Alexander II''' ({{lang-ka|ალექსანდრე II, '''''Aleksandre II'''''}}) (died [[April 1]], [[1510]]) was a.
* Prince David (born 1505)
[[Category:1510 deaths]]", "foo"), Is.EqualTo(n5));

            // don't pick up outside of zeroth section
            const string n6 = @"foo {{Infobox actor}}
== hello ==
{{birth date and age|1960|01|9}}";
            Assert.That(Parsers.FixPeopleCategories(n6, "foo"), Is.EqualTo(n6));

            // don't grab a number out of a reference
            const string n7 = @"'''Buck Peterson''' (born in [[Minnesota, United States]]<ref name=extreme_1970/>) {{Infobox actor}}";

            Assert.That(Parsers.FixPeopleCategories(n7, "foo"), Is.EqualTo(n7));

            // don't accept if dash before year: could be death date
            const string n8 = @"'''Wilhem Heinrich Kramer''' (born [[Dresden]] – 1765) {{Infobox actor}}";

            Assert.That(Parsers.FixPeopleCategories(n8, "foo"), Is.EqualTo(n8));

            // date of death too far from bold name to be correct
            const string n9 = @"'''Æthelstan''' was king of [[East Anglia]] in the 9th century.

As with the other kings of East Anglia, there is very little textual information available. He did, however, leave an extensive coinage of both portrait and non-portrait type (for example, Coins of England and the United Kingdom, Spink and Son, London, 2005 and the Fitzwilliam Museum database of early medieval coins. [http://www.fitzmuseum.cam.ac.uk/coins/emc]

It is suggested that Æthelstan was probably the king who defeated and killed the Mercian kings [[Beornwulf of Mercia|Beornwulf]] (killed 826) and [[Ludeca of Mercia|Ludeca]] (killed 827). He may have attempted to seize power in [[East Anglia]] on the death of [[Coenwulf of Mercia]] (died 821). If this";

            // date of death over newline – too far away to be correct
            Assert.That(Parsers.FixPeopleCategories(n9, "foo"), Is.EqualTo(n9));

            const string n10 = @"'''Fred''' blah
died 2002
{{Infobox actor}}";

            Assert.That(Parsers.FixPeopleCategories(n10, "foo"), Is.EqualTo(n10));

            // don't grab numbers out of long wikilinks
            const string n11 = @"'''Marcus Caecilius Metellus''' was a son of [[Lucius Caecilius Metellus (died 221 BC)|Lucius Caecilius Metellus]]. Deported {{Infobox actor}}";

            Assert.That(Parsers.FixPeopleCategories(n11, "foo"), Is.EqualTo(n11));

            // birth and death
            const string bd1 = @"''Foo''' (8 May 1920 - 11 June 2004) was {{Infobox actor}}";

            const string bd1a = @"
[[Category:1920 births]]", bd1b = @"
[[Category:2004 deaths]]";

            Assert.That(Parsers.FixPeopleCategories(bd1, "foo"), Is.EqualTo(bd1 + bd1a + bd1b));

            Assert.That(Parsers.FixPeopleCategories(bd1 + bd1a, "foo"), Is.EqualTo(bd1 + bd1a + bd1b));

            Assert.That(Parsers.FixPeopleCategories(bd1 + bd1b, "foo"), Is.EqualTo(bd1 + bd1b + bd1a));

            const string image = @"[[File:long text here.jpg|thumb|250px|Long text here as well to make sure it is over 100 in total length [[this is a long wilikink]]]]
";
            Assert.That(Parsers.FixPeopleCategories(image + bd1, "foo"), Is.EqualTo(image + bd1 + bd1a + bd1b), "Ignore File link at start of page");

            const string bd2 = @"''Foo''' (8 May 1920 – 11 June 2004) was {{Infobox actor}}";
            Assert.That(Parsers.FixPeopleCategories(bd2, "foo"), Is.EqualTo(bd2 + bd1a + bd1b));

            const string bd3 = @"'''Foo''' (8 May 1920 somewhere &ndash;11 June 2004) was {{Infobox actor}}";
            Assert.That(Parsers.FixPeopleCategories(bd3, "foo"), Is.EqualTo(bd3 + bd1a + bd1b));

            // approximate date check still applied
            string bd4 = @"''Foo''' (Circa 8 May 1920 – 11 June 2004) was {{Infobox actor}}";
            Assert.That(Parsers.FixPeopleCategories(bd4, "foo"), Is.EqualTo(bd4 + u + @"
[[Category:2004 deaths]]"));

            const string bd5 = @"'''Autpert Ambrose (Ambroise)''' (ca. 730 – 784) {{Infobox actor}}
[[Category:778 deaths]]";

            Assert.That(Parsers.FixPeopleCategories(bd5, "foo"), Is.EqualTo(bd5 + u));

            const string bd6 = @"'''Saint Bruno of Querfurt''' (c. 970 – February 14 1009) (also known as ''Brun'' and ''Boniface''  {{Infobox actor}}
[[Category:1009 deaths]]";

            Assert.That(Parsers.FixPeopleCategories(bd6, "foo"), Is.EqualTo(bd6 + u));

            // all correct already
            const string bd7 = @"'''Agapetus II''' (born in [[Rome]]; died October, 955) was {{Infobox actor}}
[[Category:955 deaths]]";

            Assert.That(Parsers.FixPeopleCategories(bd7, "foo"), Is.EqualTo(bd7));

            // no change
            const string bd8 = @"'''Husain''' (d. April or May 1382) was a [[Jalayirids|Jalayirid]] ruler (1374-1382). He was the son of [[Shaikh Uvais]]. {{Infobox actor}}
[[Category:1382 deaths]]";

            // add death and uncertain birth
            Assert.That(Parsers.FixPeopleCategories(bd8, "foo"), Is.EqualTo(bd8));

            const string bd9 = @"'''Mannalargenna''' (ca. 1770-1835), a [[Tasmanian Aborigine]], was the chief of the Ben Lomond tribe (Plangermaireener). {{Infobox actor}}";

            Assert.That(Parsers.FixPeopleCategories(bd9, "foo"), Is.EqualTo(bd9 + @"
[[Category:Year of birth uncertain]]
[[Category:1835 deaths]]"));

            const string bd9b = @"'''Mannalargenna''' (c 1770-1835), a [[Tasmanian Aborigine]], was the chief of the Ben Lomond tribe (Plangermaireener). {{Infobox actor}}";

            Assert.That(Parsers.FixPeopleCategories(bd9b, "foo"), Is.EqualTo(bd9b + @"
[[Category:Year of birth uncertain]]
[[Category:1835 deaths]]"));

            const string bd10 = @"'''King Godfred''' (ruled 804 - 810) {{Infobox actor}}";
            Assert.That(Parsers.FixPeopleCategories(bd10, "foo"), Is.EqualTo(bd10));

            const string bd10b = @"'''King Godfred''' (born Abe Smith, transitioned 2005) {{Infobox actor}}";
            Assert.That(Parsers.FixPeopleCategories(bd10b, "foo"), Is.EqualTo(bd10b));

            const string bd11 = @"'''Rabat I''' (1616/7 - 1644/5) {{Infobox actor}}";

            Assert.That(Parsers.FixPeopleCategories(bd11, "foo"), Is.EqualTo(bd11 + u));

            const string bd11b = @"'''Rabat I''' (born 1969-1970) {{Infobox actor}}
[[Category:Year of birth uncertain]]";

            Assert.That(Parsers.FixPeopleCategories(bd11b, "foo"), Is.EqualTo(bd11b));

            const string bd12 = @"'''Lorenzo Monaco''' (born  '''Piero di Giovanni''' [[Circa|c.]]1370-1425) {{Infobox actor}}
[[Category:1425 deaths]]";

            Assert.That(Parsers.FixPeopleCategories(bd12, "foo"), Is.EqualTo(bd12 + u));

            const string bd13 = @"'''Mocius''' ('''Mucius''', died 288-295), also kno
[[Category:3rd-century deaths]]";

            Assert.That(Parsers.FixPeopleCategories(bd13, "foo"), Is.EqualTo(bd13));

            // with flourit both dates are uncertain
            const string bd14 = @"'''Asclepigenia''' (fl. 430  – 485 AD) was {{Infobox actor}}";
            Assert.That(Parsers.FixPeopleCategories(bd14, "foo"), Is.EqualTo(bd14));

            const string bd14a = @"'''Asclepigenia''' ([[floruit|fl]]. 430  – 485 AD) was {{Infobox actor}}";
            Assert.That(Parsers.FixPeopleCategories(bd14a, "foo"), Is.EqualTo(bd14a));

            const string bd14b = @"'''Asclepigenia''' (flourished 430  – 485 AD) was {{Infobox actor}}";
            Assert.That(Parsers.FixPeopleCategories(bd14b, "foo"), Is.EqualTo(bd14b));

            const string bd14c = @"'''Asclepigenia''' ({{fl}} 430) was {{Infobox actor}}";
            Assert.That(Parsers.FixPeopleCategories(bd14c, "foo"), Is.EqualTo(bd14c));

            const string bd14d = @"'''Asclepigenia''' ({{floruit}} 430) was {{Infobox actor}}";
            Assert.That(Parsers.FixPeopleCategories(bd14d, "foo"), Is.EqualTo(bd14d));

            // no data to use here
            const string no1 = @"'''Bahram I''' (also spelled ''Varahran'' or ''Vahram'', ''r.'' 273&ndash;276) {{Infobox actor}}";
            Assert.That(Parsers.FixPeopleCategories(no1, "foo"), Is.EqualTo(no1));

            const string no2 = @"'''Trebellianus''' (d. 260-268), also {{Infobox actor}}";
            Assert.That(Parsers.FixPeopleCategories(no2, "foo"), Is.EqualTo(no2));

            const string no3 = @"'''[[Grand Ayatollah]] {{unicode|Muhammad Sādiq as-Sadr}}''' ([[Arabic]] محمّد صادق الصدر ) is an [[Iraq]]i [[Twelver]] [[Shi'a]] cleric of high rank. He is the father of [[Muqtada al-Sadr]] (born 1973). Sometimes the son is called by his father's name. He is the cousin of Grand Ayatollah [[Muhammad Baqir al-Sadr]] (died 1980). The al-Sadr family are considered [[Sayyid]], which is used among the Shia to denote persons descending directly from [[Muhammad]]. The family's lineage is traced through Imam [[Jafar al-Sadiq]] and his son Imam [[Musa al-Kazim]] the sixth and seventh Shia Imams respectively.";
            Assert.That(Parsers.FixPeopleCategories(no3, "foo"), Is.EqualTo(no3));

            const string no4 = @"'''Bahram I''' (also spelled ''Varahran'' or ''Vahram'', ''r.'' 273&ndash;276) {{Infobox actor}}";
            Assert.That(Parsers.FixPeopleCategories(no4, "foo"), Is.EqualTo(no4));

            const string no5 = @"'''John Coggeshall''' (chr. December 9, 1601) Charles
[[Category:1835 deaths]]";
            Assert.That(Parsers.FixPeopleCategories(no5, "foo"), Is.EqualTo(no5));

            const string ISO1 = @"'''Ben Moon''' (born [[1960-06-13]]) is a {{Infobox actor}}";
            Assert.That(Parsers.FixPeopleCategories(ISO1, "foo"), Is.EqualTo(ISO1 + "\r\n" + b2));

            const string bd15 = @"'''Kristina of Norway''' (born in [[1234]] in [[Bergen]] &ndash; circa [[1262]]), sometimes {{Infobox actor}}";
            Assert.That(Parsers.FixPeopleCategories(bd15, "foo"), Is.EqualTo(bd15 + @"
[[Category:1234 births]]"));

            // sortkey usage
            const string s1 = @"'''Claire Hazel Weekes''' (1903&mdash;1990) was {{Infobox actor}}
[[Category:1990 deaths|Weekes, Claire]]";

            Assert.That(Parsers.FixPeopleCategories(s1, "foo"), Is.EqualTo(s1 + @"
[[Category:1903 births|Weekes, Claire]]"));

            const string m1 = @"'''Hans G Helms''' (born [[8 June]] [[1960]] in [[Teterow]]; full name: ''Hans Günter Helms''; the bearer of the name does not use a full stop after the initial for his middle name) is a [[Germany|German]] experimental writer, composer, and social and economic analyst and critic. {{Infobox actor}}";

            Assert.That(Parsers.FixPeopleCategories(m1, "foo"), Is.EqualTo(m1 + "\r\n" + b2));

            // uncertain year of death
            const string m2 = @"'''Arthur Paunzen''' (born [[4 February]] [[1890]], died ?[[9 August]] [[1940]])
[[Category:1890 births]]";

            Assert.That(Parsers.FixPeopleCategories(m2, "foo"), Is.EqualTo(m2));

            const string m3 = @"Foo {{death date and age|2008|8|21|1942|7|13|mf=y}} {{Infobox actor}}";
            const string m3a = @"
[[Category:1942 births]]
[[Category:2008 deaths]]";

            Assert.That(Parsers.FixPeopleCategories(m3, "foo"), Is.EqualTo(m3 + m3a));

            const string bug1 = @"'''Jane Borghesi''' (born 17 June{{Fact|date=January 2009}}, [[Melbourne]]) {{Infobox actor}}";
            Assert.That(Parsers.FixPeopleCategories(bug1, "foo"), Is.EqualTo(bug1), "Don't take year from date param of template e.g. maintenance tag");

            const string miss1 = @"'''Alonza J. White''' (ca 1836 &ndash; [[August 29]] [[1912]]) {{Infobox actor}}
{{DEFAULTSORT:White, Alonza J}}
[[Category:1912 deaths]]
[[Category:People from St. John's, Newfoundland and Labrador]]", miss2 = @"[[Category:Year of birth missing]]";

            Assert.That(Parsers.FixPeopleCategories(miss1 + "\r\n" + miss2, "foo"), Is.EqualTo(miss1 + u));

            const string both1 = @"'''Mary Ellen Wilson''' (1864–1956)<ref name=""amhum"">{{foo}}</ref> {{Infobox actor}}", both2 = @"[[Category:1864 births]]
[[Category:1956 deaths]]";

            Assert.That(Parsers.FixPeopleCategories(both1, "foo"), Is.EqualTo(both1 + "\r\n" + both2));

            const string bug2 = @"'''Foo''' (born {{circa}} 1925) was {{Infobox actor}}";
            Assert.That(Parsers.FixPeopleCategories(bug2, "foo"), Is.EqualTo(bug2 + u));

            const string bug3 = @"'''Joshua William Allen, 6th Viscount Allen''' [[Master of Arts (Oxbridge)|MA]] ({{circa}} [[1782]]–[[21 September]] [[1845]]) was an [[Irish peerage|Irish peer]].

{{DEFAULTSORT:Allen, Joshua Allen, 6th Viscount}}
[[Category:1845 deaths]]
[[Category:Viscounts in the Peerage of Ireland]]
[[Category:Year of birth uncertain]]";

            Assert.That(Parsers.FixPeopleCategories(bug3, "foo"), Is.EqualTo(bug3));

            const string bug4 = @"
{{infobox person | birth_date = {{Birth date based on age at death |49 |1994|02|25}}
}}
{{DEFAULTSORT:Goldberg, Robert P.}}
[[Category:American computer businesspeople]]
[[Category:American computer scientists]]
[[Category:Harvard University faculty]]
[[Category:1994 deaths]]
[[Category:Harvard University alumni]]
[[Category:Year of birth missing]]";

            Assert.That(Parsers.FixPeopleCategories(bug4, "foo"), Is.EqualTo(bug4), "No birth year available in {{Birth date based on age at death}}");

            const string bug5 = @"
{{infobox person | birth_date = {{Birth based on age at death|49 |1994|02|25}}
}}
{{DEFAULTSORT:Goldberg, Robert P.}}
[[Category:American computer businesspeople]]
[[Category:American computer scientists]]
[[Category:Harvard University faculty]]
[[Category:1994 deaths]]
[[Category:Harvard University alumni]]
[[Category:Year of birth missing]]";

            Assert.That(Parsers.FixPeopleCategories(bug5, "foo"), Is.EqualTo(bug5), "No birth year available in {{Birth based on age at death}}");
        }

        [Test]
        public void YearOfBirthMissingCategoryEnOnly()
        {
#if DEBUG
            const string good = @"[[Category:Pakistani lawyers]]
[[Category:Attorneys General of Pakistan]]
[[Category:Living people]]
[[Category:1944 births]]", bad = @"[[Category:Pakistani lawyers]]
[[Category:Attorneys General of Pakistan]]
[[Category:Year of birth missing (living people)]]
[[Category:Living people]]
[[Category:1944 births]]";

            Variables.SetProjectLangCode("fr");
            Assert.That(Parsers.FixPeopleCategories(bad, "foo"), Is.EqualTo(bad));

            Variables.SetProjectLangCode("en");
            Assert.That(Parsers.FixPeopleCategories(bad, "foo"), Is.EqualTo(good));
#endif
        }

        [Test]
        public void FixPeopleCategoriesFromInfobox()
        {
            const string a1 = @"'''Fred Smith''' (born 1960) is a bloke. {{Infobox actor}}";
            // infobox scraping
            const string infob1 = @"{{Infobox Officeholder
|honorific-prefix   =
|name            = John C. Zimmerman, Sr.
|term_start       = 1895
|term_end         = 1896
|predecessor      = [[Arthur C. McCall]]
|successor        = [[Samuel C. Randall]]
|birth_date      = May 12, 1835
|birth_place     = [[Free City of Frankfurt]]
|death_date= October 26, 1935
|death_place=
|restingplace = Glenwood Cemetery, Flint
|restingplacecoordinates =
|alma_mater      =
|occupation      =brickmason, mercha

}}", infob2 = @"{{Infobox Officeholder
|honorific-prefix   =
|name            = John Footus
|term_start       = 144 BCE
|term_end         = 127 BCE
|predecessor      = [[Arthur C. McCall]]
|successor        = [[Samuel C. Randall]]
|birth_date      = 193 BC
|birth_place     = [[Free City of Frankfurt]]
|death_date= 127 BCE
|death_place=
|alma_mater      =
|occupation      =brickmason, mercha

}}", infob1a = @"{{Infobox Officeholder
|honorific-prefix   =
|name            = John C. Zimmerman, Sr.
|term_start       = 1895
|term_end         = 1896
|predecessor      = [[Arthur C. McCall]]
|successor        = [[Samuel C. Randall]]
|dateofbirth     = May 12, 1835
|birth_place     = [[Free City of Frankfurt]]
|dateofdeath= October 26, 1935
|death_place=
|restingplace = Glenwood Cemetery, Flint
|restingplacecoordinates =
|alma_mater      =
|occupation      =brickmason, mercha

}}", infob1b = @"{{Infobox Officeholder
|honorific-prefix   =
|name            = John C. Zimmerman, Sr.
|term_start       = 1895
|term_end         = 1896
|predecessor      = [[Arthur C. McCall]]
|successor        = [[Samuel C. Randall]]
|DateOfBirth     = May 12, 1835
|birth_place     = [[Free City of Frankfurt]]
|DateOfDeath= October 26, 1935
|death_place=
|restingplace = Glenwood Cemetery, Flint
|restingplacecoordinates =
|alma_mater      =
|occupation      =brickmason, mercha

}}";

            // scraped from infobox
            Assert.That(Parsers.FixPeopleCategories(infob1, "foo"), Is.EqualTo(infob1 + @"
[[Category:1835 births]]
[[Category:1935 deaths]]"));

            Assert.That(Parsers.FixPeopleCategories(infob1a, "foo"), Is.EqualTo(infob1a + @"
[[Category:1835 births]]
[[Category:1935 deaths]]"));

            Assert.That(Parsers.FixPeopleCategories(infob1b, "foo"), Is.EqualTo(infob1b + @"
[[Category:1835 births]]
[[Category:1935 deaths]]"));

            Assert.That(Parsers.FixPeopleCategories(infob2, "foo"), Is.EqualTo(infob2 + @"
[[Category:193 BC births]]
[[Category:127 BC deaths]]"));

            // doesn't add twice
            Assert.That(Parsers.FixPeopleCategories(infob1 + @"
[[Category:1835 births]]
[[Category:1935 deaths]]", "foo"), Is.EqualTo(infob1 + @"
[[Category:1835 births]]
[[Category:1935 deaths]]"));

            // just decade no good
            const string infob3 = @"{{Infobox Officeholder
|honorific-prefix   =
|name            = John Foo
|predecessor      = [[Arthur C. McCall]]
|successor        = [[Samuel C. Randall]]
|birth_date      = 1970s
|birth_place     = [[Free City of Frankfurt]]
|death_date=
|death_place=
|alma_mater      =
|occupation      =brickmason, mercha

}}";
            Assert.That(Parsers.FixPeopleCategories(infob3, "foo"), Is.EqualTo(infob3), "no change if only decade of birth");

            const string infob4 = @"{{Infobox Officeholder
|honorific-prefix   =
|name            = John Foo
|predecessor      = [[Arthur C. McCall]]
|successor        = [[Samuel C. Randall]]
|birth_date      = {{floruit|1444}}
|birth_place     = [[Free City of Frankfurt]]
|death_date=
|death_place=
|alma_mater      =
|occupation      =brickmason, mercha

}}";
            Assert.That(Parsers.FixPeopleCategories(infob4, "foo"), Is.EqualTo(infob4), "no change if only floruit");

            string unc1 = @"'''Aaron Walden''' (born at [[Warsaw]] about 1835, died 1912) was a Polish Jewish [[Talmudist]], editor, and author.
{{DEFAULTSORT:Walden, Aaron}}
[[Category:Polish Jews]]
[[Category:1912 deaths]]
[[Category:Year of birth uncertain]]";

            Assert.That(Parsers.FixPeopleCategories(unc1, "Aaron Walden"), Is.EqualTo(unc1));

            // too many refs for it to be plausible that the cats are missing
            const string Refs = @"<ref>a</ref> <ref>a</ref> <ref>a</ref> <ref>a</ref> <ref>a</ref> <ref>a</ref> <ref>a</ref>";
            Assert.That(Parsers.FixPeopleCategories(a1 + Refs + Refs + Refs, "foo"), Is.EqualTo(a1 + Refs + Refs + Refs));

            string infob1Date = infob1.Replace(@"alma_mater      =", @"date=1 March");
            Assert.That(Parsers.FixPeopleCategories(infob1Date, "foo"), Is.EqualTo(infob1Date + @"
[[Category:1835 births]]
[[Category:1935 deaths]]"), "Can use template with date param when date doesn't contain year");
        
            // don't take death year out of ref
            const string infob5 = @"{{Infobox Officeholder
|honorific-prefix   =
|name            = John Foo
|birth_date      = 
|death_date= XXX{{sfn|Smith|2005}}
|death_place=
}}";
            Assert.That(Parsers.FixPeopleCategories(infob5, "foo"), Is.EqualTo(infob5), "ignore ref for death year");

            // don't take death year out of {{birth date}}
            const string infob6 = @"{{Infobox Officeholder
|honorific-prefix   =
|name            = John Foo
|birth_date      = 
|death_date= {{birth date|2005|1|1}}
|death_place=
}}
[[Category:2005 births]]";
            Assert.That(Parsers.FixPeopleCategories(infob6, "foo"), Is.EqualTo(infob6), "ignore ref for death year");
        
        }

        [Test]
        public void FixPeopleCategoriesRefs()
        {
            string refs = @"<ref>foo</ref> and <ref>foo</ref> and <ref>foo</ref> and <ref>foo</ref> and <ref>foo</ref> and <ref>foo</ref> ";

            string Over20Refs = refs + refs + refs + refs + @"'''Fred Smith''' (born 1980) is a bloke. {{Infobox actor}}";

            Assert.That(Parsers.FixPeopleCategories(Over20Refs, "test"), Is.EqualTo(Over20Refs), "no change when over 20 refs and no exiting birth/death/living cat");

            Assert.IsTrue(Parsers.FixPeopleCategories(Over20Refs + @" [[Category:Living people]]", "test").Contains(@"[[Category:1980 births]]"), "can add cat when over 20 refs and living people cat already");
        }

        [Test]
        public void FixPeopleCategoriesFutureTest()
        {
            // birth
            const string a1 = @"'''Fred Smith''' (born 2060) is a bloke. {{Infobox actor}}";
            Assert.That(Parsers.FixPeopleCategories(a1, "foo"), Is.EqualTo(a1));
        }

        [Test]
        public void YearOfBirthMissingCategory()
        {
            Assert.That(Parsers.FixPeopleCategories(@"[[Category:Pakistani lawyers]]
[[Category:Attorneys General of Pakistan]]
[[Category:Year of birth missing (living people)]]
[[Category:Living people]]
[[Category:1944 births]]", "foo"), Is.EqualTo(@"[[Category:Pakistani lawyers]]
[[Category:Attorneys General of Pakistan]]
[[Category:Living people]]
[[Category:1944 births]]"));

            Assert.That(Parsers.FixPeopleCategories(@"[[Category:Pakistani lawyers]]
[[Category:Attorneys General of Pakistan]]
[[Category:Year of birth missing (living people)]]
[[Category:Living people]]
[[Category : 1944 births]]", "foo"), Is.EqualTo(@"[[Category:Pakistani lawyers]]
[[Category:Attorneys General of Pakistan]]
[[Category:Living people]]
[[Category : 1944 births]]"), "category excess whitespace");

            Assert.That(Parsers.FixPeopleCategories(@"[[Category:Pakistani lawyers]]
[[Category:Attorneys General of Pakistan]]
[[Category:Year of birth missing]]
[[Category:Living people]]
[[Category:1944 births]]", "foo"), Is.EqualTo(@"[[Category:Pakistani lawyers]]
[[Category:Attorneys General of Pakistan]]
[[Category:Living people]]
[[Category:1944 births]]"));

            // no change when already correct
            Assert.That(Parsers.FixPeopleCategories(@"[[Category:Pakistani lawyers]]
[[Category:Attorneys General of Pakistan]]
[[Category:Living people]]
[[Category:1944 births]]", "foo"), Is.EqualTo(@"[[Category:Pakistani lawyers]]
[[Category:Attorneys General of Pakistan]]
[[Category:Living people]]
[[Category:1944 births]]"));

            const string a = @"Mr foo {{Infobox actor}} was great
[[Category:1960 deaths]]";
            Assert.That(Parsers.FixPeopleCategories(a + "\r\n" + @"[[Category:Year of death missing]]", "test"), Is.EqualTo(a + "\r\n"));
        }

        [Test]
        public void GetCategorySortTests()
        {
            Assert.That(Parsers.GetCategorySort(@"'''Jonathan Sothcott''' (born [[26 April]] [[1980]]) {{Infobox actor}}
{{DEFAULTSORT:Sothcott, Jonathan}}
[[Category:British television producers|Sothcott, Jonathon]]"), Is.Empty);

            Assert.That(Parsers.GetCategorySort(@"'''Jonathan Sothcott''' (born [[26 April]] [[1980]]) {{Infobox actor}}
[[Category:British television producers|Sothcott, Jonathan]]
[[Category:Living people|Sothcott, Jonathan]]"), Is.EqualTo(@"Sothcott, Jonathan"));

            Assert.That(Parsers.GetCategorySort(@"'''Jonathan Sothcott''' (born [[26 April]] [[1980]]) {{Infobox actor}}
[[Category:Living people|Sothcott, Jonathan]]"), Is.EqualTo(@"Sothcott, Jonathan"));

            // not all same – null return
            Assert.That(Parsers.GetCategorySort(@"'''Jonathan Sothcott''' (born [[26 April]] [[1980]]) {{Infobox actor}}
[[Category:British television producers|Sothcott, Jonathan]]
[[Category:Living people|Sothcott, Jonathan]]
[[Category:1944 births]]"), Is.Empty);
        }

        [Test]
        public void CategoryMatch()
        {
            Assert.IsTrue(Parsers.CategoryMatch(@"foo [[Category:1990 births]]", @"1990 births"));
            Assert.IsTrue(Parsers.CategoryMatch(@"foo [[Category:1990 births ]]", @"1990 births"));
            Assert.IsTrue(Parsers.CategoryMatch(@"foo [[Category: 1990 births  ]]", @"1990 births"));
            Assert.IsTrue(Parsers.CategoryMatch(@"foo [[Category:1990 Births]]", @"1990 births"));
            Assert.IsTrue(Parsers.CategoryMatch(@"foo [[Category:1990 births]]", @"1990 Births"));
            Assert.IsTrue(Parsers.CategoryMatch(@"foo [[Category:1990 births|foo]]", @"1990 births"));
            Assert.IsTrue(Parsers.CategoryMatch(@"foo [[Category:1990 births | foo]]", @"1990 births"));
            Assert.IsTrue(Parsers.CategoryMatch(@"foo [[Category:Year of birth missing|Foo, bar]]", "Year of birth missing"));

            Assert.IsFalse(Parsers.CategoryMatch(@"foo [[Category:1990 births]]", @"1990"));
            Assert.IsFalse(Parsers.CategoryMatch(@"foo [[Category:1990_births]]", @"1990 births"));
        }

        [Test]
        public void LivingPeopleTests()
        {
            // with sortkey
            Assert.That(Parsers.LivingPeople(@"'''Fred Smith''' (born 1960) is a bloke.
[[Category:1960 births|Smith, Fred]]", "A"), Is.EqualTo(@"'''Fred Smith''' (born 1960) is a bloke.
[[Category:1960 births|Smith, Fred]][[Category:Living people|Smith, Fred]]"));

            // no sortkey
            Assert.That(Parsers.LivingPeople(@"'''Fred Smith''' (born 1960) is a bloke.
[[Category:1960 births]]", "A"), Is.EqualTo(@"'''Fred Smith''' (born 1960) is a bloke.
[[Category:1960 births]][[Category:Living people]]"));

            // non-mainspace: add with colon
            Assert.That(Parsers.LivingPeople(@"'''Fred Smith''' (born 1960) is a bloke.
[[Category:1960 births]]", "Wikipedia:Sandbox"), Is.EqualTo(@"'''Fred Smith''' (born 1960) is a bloke.
[[Category:1960 births]][[:Category:Living people]]"));

            // no matches if not identified as born
            const string b1 = @"'''Fred Smith''' is a bloke.";
            Assert.That(Parsers.LivingPeople(b1, "A"), Is.EqualTo(b1));

            // no matches if identified as dead
            const string a = @"'''Fred Smith''' (born 1960) is a bloke.
[[Category:1960 births]]
[[Category:1990 deaths]]";
            const string b = @"'''Fred Smith''' is a bloke.
[[Category:1960 births]]
[[Category:1990 deaths]]";
            const string b2 = @"'''Fred Smith''' is a bloke.
[[Category:1960 births]]
[[Category:1990 suicides]]";
            const string c = @"'''Fred Smith''' is a bloke.
[[Category:1960 births|Smith, Fred]]
[[Category:1990 deaths|Smith, Fred]]";
            const string d = @"'''Fred Smith''' (born 11 May 1950 - died 17 August 1990) is a bloke.
[[Category:1960 births|Smith, Fred]]";
            const string e = @"'''Fred Smith''' (born 1950) is a bloke.
[[Category:1950 births|Smith, Fred]]
{{Recentlydeceased}}";
            const string f = @"'''Fred Smith''' (born 1950) is a bloke.
[[Category:1950 births|Smith, Fred]]
{{recentlydeceased}}";
            const string g = @"'''Fred Smith''' (born 1950) is a bloke.
[[Category:1950 births|Smith, Fred]]
[[Category:Year of death missing]]";
            const string h = @"'''Fred Smith''' (d. 1950) is a bloke.";

            Assert.That(Parsers.LivingPeople(a, "A"), Is.EqualTo(a));
            Assert.That(Parsers.LivingPeople(b, "A"), Is.EqualTo(b));
            Assert.That(Parsers.LivingPeople(b2, "A"), Is.EqualTo(b2));
            Assert.That(Parsers.LivingPeople(c, "A"), Is.EqualTo(c));
            Assert.That(Parsers.LivingPeople(d, "A"), Is.EqualTo(d));
            Assert.That(Parsers.LivingPeople(e, "A"), Is.EqualTo(e));
            Assert.That(Parsers.LivingPeople(f, "A"), Is.EqualTo(f));
            Assert.That(Parsers.LivingPeople(g, "A"), Is.EqualTo(g));
            Assert.That(Parsers.LivingPeople(h, "A"), Is.EqualTo(h));

            // assume dead if born earlier than 121 years ago, so no change
            const string d1 = @"'''Fred Smith''' (born 1879) is a bloke.
[[Category:1879 births|Smith, Fred]]";
            Assert.That(Parsers.LivingPeople(d1, "A"), Is.EqualTo(d1));

            // check correctly handles birth category with no year to parse
            const string d2 = @"Fred [[Category:15th-century births]]";
            Assert.That(Parsers.LivingPeople(d2, "A"), Is.EqualTo(d2));
        }

        [Test]
        public void LivingPeopleTestsEnOnly()
        {
#if DEBUG
            const string Before = @"'''Fred Smith''' (born 1960) is a bloke.
[[Category:1960 births|Smith, Fred]]";

            Variables.SetProjectLangCode("sco");
            Assert.That(Parsers.LivingPeople(Before, "A"), Is.EqualTo(Before), "no changes in Scottish wikipedia");

            Variables.SetProjectSimple("en", ProjectEnum.commons);
            Assert.That(Parsers.LivingPeople(Before, "A"), Is.EqualTo(Before), "no changes in Commons");
            
            Variables.SetProjectSimple("en", ProjectEnum.wikipedia);
            Assert.That(Parsers.LivingPeople(Before, "A"), Is.EqualTo(Before + @"[[Category:Living people|Smith, Fred]]"));
#endif
        }

        [Test]
        public void FixSyntaxCategory()
        {
            const string correct = @"Now [[Category:2005 albums]] there";

            Assert.That(Parsers.FixSyntax(@"Now {{Category:2005 albums]] there"), Is.EqualTo(correct));
            Assert.That(Parsers.FixSyntax(@"Now {{Category:2005 albums}} there"), Is.EqualTo(correct));
            Assert.That(Parsers.FixSyntax(@"Now {{ Category:2005 albums]] there"), Is.EqualTo(correct));
            Assert.That(Parsers.FixSyntax(@"Now [[Category:2005 albums}} there"), Is.EqualTo(correct));
            Assert.That(Parsers.FixSyntax(@"Now [[  Category:2005 albums}} there"), Is.EqualTo(correct));

            Assert.That(Parsers.FixSyntax(correct), Is.EqualTo(correct));
        }

        [Test]
        public void TestFixCategories()
        {
            Assert.That(Parsers.FixCategories("[[ categOry : Foo_bar]]"), Is.EqualTo("[[Category:Foo bar]]"));
            Assert.That(Parsers.FixCategories("[[ categOry : Foo_bar|boz]]"), Is.EqualTo("[[Category:Foo bar|boz]]"));
            Assert.That(Parsers.FixCategories("[[category : foo_bar%20|quux]]"), Is.EqualTo("[[Category:Foo bar|quux]]"));
            Assert.That(Parsers.FixCategories(@"[[Category:Foo_bar]]"), Is.EqualTo(@"[[Category:Foo bar]]"));
            Assert.That(Parsers.FixCategories(@"[[category:Foo bar]]"), Is.EqualTo(@"[[Category:Foo bar]]"));
            Assert.That(Parsers.FixCategories(@"[[Category::Foo bar]]"), Is.EqualTo(@"[[Category:Foo bar]]"));
            Assert.That(Parsers.FixCategories(@"[[Category  :  Foo_bar  ]]"), Is.EqualTo(@"[[Category:Foo bar]]"));
            Assert.That(Parsers.FixCategories("[[CATEGORY: Foo_bar]]"), Is.EqualTo("[[Category:Foo bar]]"));

            // https://en.wikipedia.org/w/index.php?title=Wikipedia_talk:AutoWikiBrowser/Bugs&oldid=262844859#General_fixes_remove_spaces_from_category_sortkeys
            Assert.That(Parsers.FixCategories(@"[[Category:Public transport in Auckland| Public transport in Auckland]]"), Is.EqualTo(@"[[Category:Public transport in Auckland| Public transport in Auckland]]"));
            Assert.That(Parsers.FixCategories(@"[[Category:Actors|Fred Astaire ]]"), Is.EqualTo(@"[[Category:Actors|Fred Astaire]]"), "trailing space IS removed");
            Assert.That(Parsers.FixCategories(@"[[Category:Actors|Fred Astaire    ]]"), Is.EqualTo(@"[[Category:Actors|Fred Astaire]]"), "trailing space IS removed");
            Assert.That(Parsers.FixCategories(@"[[Category:Actors| Fred Astaire ]]"), Is.EqualTo(@"[[Category:Actors| Fred Astaire]]"), "trailing space IS removed");
            Assert.That(Parsers.FixCategories(@"[[Category:London| ]]"), Is.EqualTo(@"[[Category:London| ]]"), "leading space NOT removed");
            Assert.That(Parsers.FixCategories(@"[[Category:Slam poetry| ]] "), Is.EqualTo(@"[[Category:Slam poetry| ]] "), "leading space NOT removed");

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Archive_18#.2Fdoc_pages_and_includeonly_sections
            Assert.That(Parsers.FixCategories("[[Category: foo_bar |boz_quux]]"), Is.EqualTo("[[Category:Foo bar|boz_quux]]"));
            Assert.That(Parsers.FixCategories("[[Category: foo_bar|{{boz_quux}}]]"), Is.EqualTo("[[Category:Foo bar|{{boz_quux}}]]"));
            Assert.That(Parsers.FixCategories("[[CategorY : foo_bar{{{boz_quux}}}]]"), Does.Contain("{{{boz_quux}}}"));
            Assert.That(Parsers.FixCategories("[[CategorY : foo_bar|{{{boz_quux}}}]]"), Is.EqualTo("[[Category:Foo bar|{{{boz_quux}}}]]"));
            Assert.That(Parsers.FixCategories("[[Category:Date computing template|{{<noinclude>BASE</noinclude>PAGENAME}}]]"), Is.EqualTo("[[Category:Date computing template|{{<noinclude>BASE</noinclude>PAGENAME}}]]"));

            // diacritics removed from sortkeys
            Assert.That(Parsers.FixCategories(@"[[Category:World Scout Committee members|Lainé, Juan]]"), Is.EqualTo(@"[[Category:World Scout Committee members|Laine, Juan]]"));

            Variables.Namespaces.Remove(Namespace.Category);
            Assert.That(Parsers.FixCategories(""), Is.Empty, "Fallback to English category namespace name");
            Variables.Namespaces.Add(Namespace.Category, "Category:");
        }

        [Test]
        public void TestFixCategoriesRu()
        {
#if DEBUG
            Variables.SetProjectLangCode("ru");
            Assert.That(Parsers.FixCategories(@"[[Category:World Scout Committee members|Lainé, Juan]]"), Is.EqualTo(@"[[Category:World Scout Committee members|Lainé, Juan]]"), "no diacritic removal for sort key on ru-wiki");
            
            Variables.SetProjectLangCode("en");
            Assert.That(Parsers.FixCategories(@"[[Category:World Scout Committee members|Lainé, Juan]]"), Is.EqualTo(@"[[Category:World Scout Committee members|Laine, Juan]]"));
#endif
        }

        [Test]
        public void TestFixCategoriesBrackets()
        {
            // brackets fixed
            Assert.That(Parsers.FixCategories(@"[[Category:London]]]"), Is.EqualTo(@"[[Category:London]]"));
            Assert.That(Parsers.FixCategories(@"[[Category:London]]]]"), Is.EqualTo(@"[[Category:London]]"));
            Assert.That(Parsers.FixCategories(@"[[[[Category:London]]]"), Is.EqualTo(@"[[Category:London]]"));
            Assert.That(Parsers.FixCategories(@"[[[[Category:London]]]]"), Is.EqualTo(@"[[Category:London]]"));
            Assert.That(Parsers.FixCategories(@"[[Category:London]]"), Is.EqualTo(@"[[Category:London]]"));
        }
    }
}