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
            const string a1 = @"'''Fred Smith''' (born 1960) is a bloke. {{Persondata}}";
            const string b2 = @"[[Category:1960 births]]";
            Assert.AreEqual(a1 + "\r\n" + b2, Parsers.FixPeopleCategories(a1, "foo"));

            const string a2 = a1 + "\r\n" + @"[[Category:1990 deaths]]";
            Assert.AreEqual(a2 + "\r\n" + b2, Parsers.FixPeopleCategories(a2, "foo"));

            const string b5 = @"Some words {{birth date and age|1960|01|9}}";
            Assert.AreEqual(b5 + "\r\n" + b2, Parsers.FixPeopleCategories(b5, "foo"));

            const string b6 = @"'''Fred Lerdahl''' (born [[March 10]] [[1960]]) [[Category:Living people]]";
            Assert.AreEqual(b6 + "\r\n" + b2, Parsers.FixPeopleCategories(b6, "foo"));

            // catch living person and very old birth date
            const string b6a = @"'''Fred Lerdahl''' (born [[March 10]] [[1860]]) [[Category:Living people]]";
            Assert.AreEqual(b6a, Parsers.FixPeopleCategories(b6a, "foo"));

            const string b7 = @"'''William Arthur O'Donnell''' (born May 4, 1960 in [[Springhill, Nova Scotia]], Canada) is [[Category:Living people]]";
            Assert.AreEqual(b7 + "\r\n" + b2, Parsers.FixPeopleCategories(b7, "foo"));

            const string b8 = @"'''Burrell Carver Smith''' (born [[December 16]], [[1960]] in upstate New York) [[Category:Living people]]";
            Assert.AreEqual(b8 + "\r\n" + b2, Parsers.FixPeopleCategories(b8, "foo"));

            const string b9 = @"'''Fredro Starr''' (born '''Fredro Scruggs''' on [[April 18]] [[1960]] in [[Jamaica, Queens]]) is an [[Category:Living people]]";
            Assert.AreEqual(b9 + "\r\n" + b2, Parsers.FixPeopleCategories(b9, "foo"));

            const string b10 = @"'''Phillip Rhodes''' (born [[May 26]], [[1960]]) was a [[Category:Living people]]";
            Assert.AreEqual(b10 + "\r\n" + b2, Parsers.FixPeopleCategories(b10, "foo"));

            // no matches when two birth dates in article
            const string b11 = @"'''Kid 'n Play''' was a [[hip-hop music|hip-hop]] and [[comedy]] duo from [[New York City]] that was popular in the late 1980s and early 1990s. The duo comprised '''[[Christopher Kid Reid]]''' (born [[April 5]] [[1964]] in [[The Bronx|The Bronx, New York City]])
and '''[[Christopher Martin (entertainer)|Christopher Play Martin]]''' (born [[July 10]] [[1962]] in [[Queens, New York City]]). Besides their successful musical careers, Kid 'n Play are also notable for branching out into acting. [[Category:Living people]]";
            Assert.AreEqual(b11, Parsers.FixPeopleCategories(b11, "foo"));

            const string b12 = @"{{Infobox actor
| name            = Gianni Capaldi
| birthdate       = <!-- {{Birth date and age|YYYY|MM|DD}} --> age 34 (as of 12 January 2010)<ref>*  Matheson, Shelley.</ref>
| birthplace      = [[Motherwell]], [[Scotland]]<br>{{Citation needed|date=September 2010}}
}}";
            Assert.AreEqual(b12, Parsers.FixPeopleCategories(b12, "foo"));
        }

        [Test]
        public void FixPeopleCategoriesUncat()
        {
            const string a1 = @"'''Fred Smith''' (born 1960) is a bloke. {{Persondata}} {{Uncat|date=May 2010}}";
            const string b2 = @"[[Category:1960 births]]";
            Assert.AreEqual(a1.Replace("Uncat", "Improve categories") + "\r\n" + b2, Parsers.FixPeopleCategories(a1, "foo"), "renames uncat to cat improve when cats added");

            Assert.AreEqual(a1 + b2, Parsers.FixPeopleCategories(a1 + b2, "foo"), "no uncat renaming when cats not added");
            Assert.AreEqual(a1.Replace(@" {{Uncat|date=May 2010}}", "") + "{{Cat improve}}" + "\r\n" + b2, Parsers.FixPeopleCategories(a1.Replace(@" {{Uncat|date=May 2010}}", "") + "{{Cat improve}}", "foo"), "no Cat improve change when cat improve already there");
        }

        [Test]
        public void FixPeopleCategoriesDeath()
        {
            const string b2 = @"[[Category:1960 births]]";

            // death
            const string a3 = @"'''Fred Smith''' (died 1960) is a bloke. {{Persondata}}";
            const string b3 = @"[[Category:1960 deaths]]";
            Assert.AreEqual(a3 + "\r\n" + b3, Parsers.FixPeopleCategories(a3, "foo"));

            const string a4 = a3 + "\r\n" + @"[[Category:1990 births]]";
            Assert.AreEqual(a4 + "\r\n" + b3, Parsers.FixPeopleCategories(a4, "foo"));

            const string d = @"'''Fred Smith''' (born 11 May 1950 - died 17 August 1990) is a bloke.
[[Category:1960 births|Smith, Fred]]";
            Assert.AreEqual(d + "\r\n" + @"[[Category:1990 deaths|Smith, Fred]]", Parsers.FixPeopleCategories(d, "foo"));

            const string d2 = @"'''Johnny Sandon''' (originally named '''Billy Beck''') (born in 1960, in Lıverpool, Lancashire died 23 December 1990) was {{Persondata}}";
            Assert.AreEqual(d2 + "\r\n" + b2 + "\r\n" + @"[[Category:1990 deaths]]", Parsers.FixPeopleCategories(d2, "foo"));

            // BC death
            const string d3 = @"'''Aeacides''' ({{lang-el|Aἰακίδης}}; died 313 BC), king {{persondata}}";
            Assert.AreEqual(d3 + "\r\n" + @"[[Category:313 BC deaths]]", Parsers.FixPeopleCategories(d3, "foo"));

            const string d4 = @"Some words {{death date and age|1960|01|9}}";
            Assert.AreEqual(d4 + "\r\n" + @"[[Category:1960 deaths]]", Parsers.FixPeopleCategories(d4, "foo"));

            // no matches if not identified as born
            const string b1 = @"'''Fred Smith''' is a bloke.";
            Assert.AreEqual(b1, Parsers.FixPeopleCategories(b1, "foo"));

            const string d5 = @"Some words {{death date and age|1960|01|9}}";
            Assert.AreEqual(d5 + @"[[Category:1960 deaths]]", Parsers.FixPeopleCategories(d4 + @"[[Category:Year of death missing]]", "foo"));
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

            Assert.AreEqual(a, Parsers.FixPeopleCategories(a, "foo"));
            Assert.AreEqual(b, Parsers.FixPeopleCategories(b, "foo"));
            Assert.AreEqual(c, Parsers.FixPeopleCategories(c, "foo"));
            Assert.AreEqual(e, Parsers.FixPeopleCategories(e, "foo"));
            Assert.AreEqual(f, Parsers.FixPeopleCategories(f, "foo"));
            Assert.AreEqual(g, Parsers.FixPeopleCategories(g, "foo"));
        }

        [Test]
        public void FixPeopleCategoriesYOBUncertain()
        {
            // year of birth uncertain
            const string u = "\r\n" + @"[[Category:Year of birth uncertain]]";

            const string u2 = @"'''Charles Meik''' (born around 1330 in [[Ghent]] - [[22 July]] [[1387]]) {{Persondata}}";
            Assert.AreEqual(u2 + u + @"
[[Category:1387 deaths]]", Parsers.FixPeopleCategories(u2, "foo"));

            const string u2a = @"'''Charles Meik''' (born either 1330 or 1331 in [[Ghent]] - [[22 July]] [[1387]]) {{Persondata}}";
            Assert.AreEqual(u2a + u + @"
[[Category:1387 deaths]]", Parsers.FixPeopleCategories(u2a, "foo"));

            const string u2b = @"'''Charles Meik''' (born ~1330 in [[Ghent]] - [[22 July]] [[1387]]) {{Persondata}}";
            Assert.AreEqual(u2b + u + @"
[[Category:1387 deaths]]", Parsers.FixPeopleCategories(u2b, "foo"));

            const string u3 = @"'''Yusuf Ibn Muhammad Ibn Yusuf al-Fasi''' ([[1530]]/[[1531|31]]{{Fact|date=February 2007}} in [[Ksar-el-Kebir]], [[Morocco]] – 14 August [[1604]] in [[Fes, Morocco]]) {{persondata}}";
            Assert.AreEqual(u3 + u + @"
[[Category:1604 deaths]]", Parsers.FixPeopleCategories(u3, "foo"));

            // no matches when approximate year of birth
            const string b12 = @"'''Judith Victor Grabiner''' (born about 1938) is {{Persondata}}";
            Assert.AreEqual(b12 + u, Parsers.FixPeopleCategories(b12, "foo"));

            const string b13 = @"'''Judith Victor Grabiner''' (born circa 1938) is {{Persondata}}";
            Assert.AreEqual(b13 + u, Parsers.FixPeopleCategories(b13, "foo"));

            const string b14 = @"'''Judith Victor Grabiner''' (born before 1938) is {{Persondata}}";
            Assert.AreEqual(b14 + u, Parsers.FixPeopleCategories(b14, "foo"));

            const string b15 = @"'''Judith Victor Grabiner''' (born 1938 or 1939) is {{Persondata}}";
            Assert.AreEqual(b15 + u, Parsers.FixPeopleCategories(b15, "foo"));

            // born BC
            const string b16 = @"'''Phillipus Rhodicus''' (born 220 BC) was a {{Persondata}}";
            Assert.AreEqual(b16 + "\r\n" + @"[[Category:220 BC births]]", Parsers.FixPeopleCategories(b16, "foo"));

            // no change: birth date not present so not 'uncertain'
            const string n1 = @"'''Thomas F. Goreau''' (born in [[Germany]], died 1970 in [[Jamaica]]) was [[Category:1970 deaths]]";
            Assert.AreEqual(n1, Parsers.FixPeopleCategories(n1, "foo"));

            const string n2 = @"'''Charles Meik''' (born? - 1923) was an {{Persondata}}";
            Assert.AreEqual(n2, Parsers.FixPeopleCategories(n2, "foo"));

            const string n2a = @"'''Anatoly Rasskazov''' (born 1960(?)) was {{persondata}}";
            Assert.AreEqual(n2a, Parsers.FixPeopleCategories(n2a, "foo"));

            // no changes
            const string n3 = @"'''Johannes Widmann''' (born c. 1460 in [[Cheb|Eger]]; died after 1498 in [[Leipzig]]) [[Category:1460s births]]";
            Assert.AreEqual(n3, Parsers.FixPeopleCategories(n3, "foo"));

            const string n4 = @"'''John Hulme''' (born circa 1970) is [[Category:Living people]]
[[Category:Year of birth missing (living people)]]";

            Assert.AreEqual(n4, Parsers.FixPeopleCategories(n4, "foo"));

            const string n4a = @"'''John Hulme''' (born c.1970) is [[Category:Living people]]
[[Category:Year of birth missing (living people)]]";

            Assert.AreEqual(n4a, Parsers.FixPeopleCategories(n4a, "foo"));

            const string n4b = @"'''John Hulme''' (born C.1970) is [[Category:Living people]]
[[Category:Year of birth missing (living people)]]";

            Assert.AreEqual(n4b, Parsers.FixPeopleCategories(n4b, "foo"));
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
            Assert.AreEqual(n5, Parsers.FixPeopleCategories(@"'''Alexander II''' ({{lang-ka|ალექსანდრე II, '''''Aleksandre II'''''}}) (died [[April 1]], [[1510]]) was a.
* Prince David (born 1505)
[[Category:1510 deaths]]", "foo"));

            // don't pick up outside of zeroth section
            const string n6 = @"foo {{persondata}}
== hello ==
{{birth date and age|1960|01|9}}";
            Assert.AreEqual(n6, Parsers.FixPeopleCategories(n6, "foo"));

            // don't grab a number out of a reference
            const string n7 = @"'''Buck Peterson''' (born in [[Minnesota, United States]]<ref name=extreme_1970/>) {{persondata}}";

            Assert.AreEqual(n7, Parsers.FixPeopleCategories(n7, "foo"));

            // don't accept if dash before year: could be death date
            const string n8 = @"'''Wilhem Heinrich Kramer''' (born [[Dresden]] – 1765) {{persondata}}";

            Assert.AreEqual(n8, Parsers.FixPeopleCategories(n8, "foo"));

            // date of death too far from bold name to be correct
            const string n9 = @"'''Æthelstan''' was king of [[East Anglia]] in the 9th century.

As with the other kings of East Anglia, there is very little textual information available. He did, however, leave an extensive coinage of both portrait and non-portrait type (for example, Coins of England and the United Kingdom, Spink and Son, London, 2005 and the Fitzwilliam Museum database of early medieval coins. [http://www.fitzmuseum.cam.ac.uk/coins/emc]

It is suggested that Æthelstan was probably the king who defeated and killed the Mercian kings [[Beornwulf of Mercia|Beornwulf]] (killed 826) and [[Ludeca of Mercia|Ludeca]] (killed 827). He may have attempted to seize power in [[East Anglia]] on the death of [[Coenwulf of Mercia]] (died 821). If this";

            // date of death over newline – too far away to be correct
            Assert.AreEqual(n9, Parsers.FixPeopleCategories(n9, "foo"));

            const string n10 = @"'''Fred''' blah
died 2002
{{persondata}}";

            Assert.AreEqual(n10, Parsers.FixPeopleCategories(n10, "foo"));

            // don't grab numbers out of long wikilinks
            const string n11 = @"'''Marcus Caecilius Metellus''' was a son of [[Lucius Caecilius Metellus (died 221 BC)|Lucius Caecilius Metellus]]. Deported {{persondata}}";

            Assert.AreEqual(n11, Parsers.FixPeopleCategories(n11, "foo"));

            // birth and death
            const string bd1 = @"''Foo''' (8 May 1920 - 11 June 2004) was {{persondata}}";

            const string bd1a = @"
[[Category:1920 births]]", bd1b = @"
[[Category:2004 deaths]]";

            Assert.AreEqual(bd1 + bd1a + bd1b, Parsers.FixPeopleCategories(bd1, "foo"));

            Assert.AreEqual(bd1 + bd1a + bd1b, Parsers.FixPeopleCategories(bd1 + bd1a, "foo"));

            Assert.AreEqual(bd1 + bd1b + bd1a, Parsers.FixPeopleCategories(bd1 + bd1b, "foo"));

            const string bd2 = @"''Foo''' (8 May 1920 – 11 June 2004) was {{persondata}}";
            Assert.AreEqual(bd2 + bd1a + bd1b, Parsers.FixPeopleCategories(bd2, "foo"));

            const string bd3 = @"'''Foo''' (8 May 1920 somewhere &ndash;11 June 2004) was {{persondata}}";
            Assert.AreEqual(bd3 + bd1a + bd1b, Parsers.FixPeopleCategories(bd3, "foo"));

            // approximate date check still applied
            string bd4 = @"''Foo''' (Circa 8 May 1920 – 11 June 2004) was {{persondata}}";
            Assert.AreEqual(bd4 + u + @"
[[Category:2004 deaths]]", Parsers.FixPeopleCategories(bd4, "foo"));

            const string bd5 = @"'''Autpert Ambrose (Ambroise)''' (ca. 730 – 784) {{persondata}}
[[Category:778 deaths]]";

            Assert.AreEqual(bd5 + u, Parsers.FixPeopleCategories(bd5, "foo"));

            const string bd6 = @"'''Saint Bruno of Querfurt''' (c. 970 – February 14 1009) (also known as ''Brun'' and ''Boniface''  {{persondata}}
[[Category:1009 deaths]]";

            Assert.AreEqual(bd6 + u, Parsers.FixPeopleCategories(bd6, "foo"));

            // all correct already
            const string bd7 = @"'''Agapetus II''' (born in [[Rome]]; died October, 955) was {{persondata}}
[[Category:955 deaths]]";

            Assert.AreEqual(bd7, Parsers.FixPeopleCategories(bd7, "foo"));

            // no change
            const string bd8 = @"'''Husain''' (d. April or May 1382) was a [[Jalayirids|Jalayirid]] ruler (1374-1382). He was the son of [[Shaikh Uvais]]. {{persondata}}
[[Category:1382 deaths]]";

            // add death and uncertain birth
            Assert.AreEqual(bd8, Parsers.FixPeopleCategories(bd8, "foo"));

            const string bd9 = @"'''Mannalargenna''' (ca. 1770-1835), a [[Tasmanian Aborigine]], was the chief of the Ben Lomond tribe (Plangermaireener). {{persondata}}";

            Assert.AreEqual(bd9 + @"
[[Category:Year of birth uncertain]]
[[Category:1835 deaths]]", Parsers.FixPeopleCategories(bd9, "foo"));

            const string bd9b = @"'''Mannalargenna''' (c 1770-1835), a [[Tasmanian Aborigine]], was the chief of the Ben Lomond tribe (Plangermaireener). {{persondata}}";

            Assert.AreEqual(bd9b + @"
[[Category:Year of birth uncertain]]
[[Category:1835 deaths]]", Parsers.FixPeopleCategories(bd9b, "foo"));

            const string bd10 = @"'''King Godfred''' (ruled 804 - 810) {{persondata}}";
            Assert.AreEqual(bd10, Parsers.FixPeopleCategories(bd10, "foo"));

            const string bd11 = @"'''Rabat I''' (1616/7 - 1644/5) {{persondata}}";

            Assert.AreEqual(bd11 + u, Parsers.FixPeopleCategories(bd11, "foo"));

            const string bd12 = @"'''Lorenzo Monaco''' (born  '''Piero di Giovanni''' [[Circa|c.]]1370-1425) {{persondata}}
[[Category:1425 deaths]]";

            Assert.AreEqual(bd12 + u, Parsers.FixPeopleCategories(bd12, "foo"));

            const string bd13 = @"'''Mocius''' ('''Mucius''', died 288-295), also kno
[[Category:3rd-century deaths]]";

            Assert.AreEqual(bd13, Parsers.FixPeopleCategories(bd13, "foo"));

            // with flourit both dates are uncertain
            const string bd14 = @"'''Asclepigenia''' (fl. 430  – 485 AD) was {{persondata}}";
            Assert.AreEqual(bd14, Parsers.FixPeopleCategories(bd14, "foo"));

            const string bd14a = @"'''Asclepigenia''' ([[floruit|fl]]. 430  – 485 AD) was {{persondata}}";
            Assert.AreEqual(bd14a, Parsers.FixPeopleCategories(bd14a, "foo"));

            const string bd14b = @"'''Asclepigenia''' (flourished 430  – 485 AD) was {{persondata}}";
            Assert.AreEqual(bd14b, Parsers.FixPeopleCategories(bd14b, "foo"));

            const string bd14c = @"'''Asclepigenia''' ({{fl}} 430) was {{persondata}}";
            Assert.AreEqual(bd14c, Parsers.FixPeopleCategories(bd14c, "foo"));

            // no data to use here
            const string no1 = @"'''Bahram I''' (also spelled ''Varahran'' or ''Vahram'', ''r.'' 273&ndash;276) {{persondata}}";
            Assert.AreEqual(no1, Parsers.FixPeopleCategories(no1, "foo"));

            const string no2 = @"'''Trebellianus''' (d. 260-268), also {{persondata}}";
            Assert.AreEqual(no2, Parsers.FixPeopleCategories(no2, "foo"));

            const string no3 = @"'''[[Grand Ayatollah]] {{unicode|Muhammad Sādiq as-Sadr}}''' ([[Arabic]] محمّد صادق الصدر ) is an [[Iraq]]i [[Twelver]] [[Shi'a]] cleric of high rank. He is the father of [[Muqtada al-Sadr]] (born 1973). Sometimes the son is called by his father's name. He is the cousin of Grand Ayatollah [[Muhammad Baqir al-Sadr]] (died 1980). The al-Sadr family are considered [[Sayyid]], which is used among the Shia to denote persons descending directly from [[Muhammad]]. The family's lineage is traced through Imam [[Jafar al-Sadiq]] and his son Imam [[Musa al-Kazim]] the sixth and seventh Shia Imams respectively.";
            Assert.AreEqual(no3, Parsers.FixPeopleCategories(no3, "foo"));

            const string no4 = @"'''Bahram I''' (also spelled ''Varahran'' or ''Vahram'', ''r.'' 273&ndash;276) {{persondata}}";
            Assert.AreEqual(no4, Parsers.FixPeopleCategories(no4, "foo"));

            const string no5 = @"'''John Coggeshall''' (chr. December 9, 1601) Charles
[[Category:1835 deaths]]";
            Assert.AreEqual(no5, Parsers.FixPeopleCategories(no5, "foo"));

            const string ISO1 = @"'''Ben Moon''' (born [[1960-06-13]]) is a {{persondata}}";
            Assert.AreEqual(ISO1 + "\r\n" + b2, Parsers.FixPeopleCategories(ISO1, "foo"));

            const string bd15 = @"'''Kristina of Norway''' (born in [[1234]] in [[Bergen]] &ndash; circa [[1262]]), sometimes {{persondata}}";
            Assert.AreEqual(bd15 + @"
[[Category:1234 births]]", Parsers.FixPeopleCategories(bd15, "foo"));

            // sortkey usage
            const string s1 = @"'''Claire Hazel Weekes''' (1903&mdash;1990) was {{persondata}}
[[Category:1990 deaths|Weekes, Claire]]";

            Assert.AreEqual(s1 + @"
[[Category:1903 births|Weekes, Claire]]", Parsers.FixPeopleCategories(s1, "foo"));

            const string m1 = @"'''Hans G Helms''' (born [[8 June]] [[1960]] in [[Teterow]]; full name: ''Hans Günter Helms''; the bearer of the name does not use a full stop after the initial for his middle name) is a [[Germany|German]] experimental writer, composer, and social and economic analyst and critic. {{persondata}}";

            Assert.AreEqual(m1 + "\r\n" + b2, Parsers.FixPeopleCategories(m1, "foo"));

            // uncertain year of death
            const string m2 = @"'''Arthur Paunzen''' (born [[4 February]] [[1890]], died ?[[9 August]] [[1940]])
[[Category:1890 births]]";

            Assert.AreEqual(m2, Parsers.FixPeopleCategories(m2, "foo"));

            const string m3 = @"Foo {{death date and age|2008|8|21|1942|7|13|mf=y}} {{persondata}}";
            const string m3a = @"
[[Category:1942 births]]
[[Category:2008 deaths]]";

            Assert.AreEqual(m3 + m3a, Parsers.FixPeopleCategories(m3, "foo"));

            const string bug1 = @"'''Jane Borghesi''' (born 17 June{{Fact|date=January 2009}}, [[Melbourne]]) {{persondata}}";
            Assert.AreEqual(bug1, Parsers.FixPeopleCategories(bug1, "foo"));

            const string miss1 = @"'''Alonza J. White''' (ca 1836 &ndash; [[August 29]] [[1912]]) {{persondata}}
{{DEFAULTSORT:White, Alonza J}}
[[Category:1912 deaths]]
[[Category:People from St. John's, Newfoundland and Labrador]]", miss2 = @"[[Category:Year of birth missing]]";

            Assert.AreEqual(miss1 + u, Parsers.FixPeopleCategories(miss1 + "\r\n" + miss2, "foo"));

            const string both1 = @"'''Mary Ellen Wilson''' (1864–1956)<ref name=""amhum"">{{foo}}</ref> {{persondata}}", both2 = @"[[Category:1864 births]]
[[Category:1956 deaths]]";

            Assert.AreEqual(both1 + "\r\n" + both2, Parsers.FixPeopleCategories(both1, "foo"));

            const string bug2 = @"'''Foo''' (born {{circa}} 1925) was {{persondata}}";
            Assert.AreEqual(bug2 + u, Parsers.FixPeopleCategories(bug2, "foo"));

            const string bug3 = @"'''Joshua William Allen, 6th Viscount Allen''' [[Master of Arts (Oxbridge)|MA]] ({{circa}} [[1782]]–[[21 September]] [[1845]]) was an [[Irish peerage|Irish peer]].

{{DEFAULTSORT:Allen, Joshua Allen, 6th Viscount}}
[[Category:1845 deaths]]
[[Category:Viscounts in the Peerage of Ireland]]
[[Category:Year of birth uncertain]]";

            Assert.AreEqual(bug3, Parsers.FixPeopleCategories(bug3, "foo"));
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
            Assert.AreEqual(bad, Parsers.FixPeopleCategories(bad, "foo"));

            Variables.SetProjectLangCode("en");
            Assert.AreEqual(good, Parsers.FixPeopleCategories(bad, "foo"));
#endif
        }

        [Test]
        public void FixPeopleCategoriesFromInfobox()
        {
            const string a1 = @"'''Fred Smith''' (born 1960) is a bloke. {{Persondata}}";
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

}} {{persondata}}", infob2 = @"{{Infobox Officeholder
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

}} {{persondata}}", infob1a = @"{{Infobox Officeholder
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

}} {{persondata}}", infob1b = @"{{Infobox Officeholder
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

}} {{persondata}}";

            // scraped from infobox
            Assert.AreEqual(infob1 + @"
[[Category:1835 births]]
[[Category:1935 deaths]]", Parsers.FixPeopleCategories(infob1, "foo"));

            Assert.AreEqual(infob1a + @"
[[Category:1835 births]]
[[Category:1935 deaths]]", Parsers.FixPeopleCategories(infob1a, "foo"));

            Assert.AreEqual(infob1b + @"
[[Category:1835 births]]
[[Category:1935 deaths]]", Parsers.FixPeopleCategories(infob1b, "foo"));

            Assert.AreEqual(infob2 + @"
[[Category:193 BC births]]
[[Category:127 BC deaths]]", Parsers.FixPeopleCategories(infob2, "foo"));

            // doesn't add twice
            Assert.AreEqual(infob1 + @"
[[Category:1835 births]]
[[Category:1935 deaths]]", Parsers.FixPeopleCategories(infob1 + @"
[[Category:1835 births]]
[[Category:1935 deaths]]", "foo"));

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

}} {{persondata}}";
            Assert.AreEqual(infob3, Parsers.FixPeopleCategories(infob3, "foo"));

            string unc1 = @"'''Aaron Walden''' (born at [[Warsaw]] about 1835, died 1912) was a Polish Jewish [[Talmudist]], editor, and author.
{{DEFAULTSORT:Walden, Aaron}}
[[Category:Polish Jews]]
[[Category:1912 deaths]]
[[Category:Year of birth uncertain]]";

            Assert.AreEqual(unc1, Parsers.FixPeopleCategories(unc1, "Aaron Walden"));

            // too many refs for it to be plausible that the cats are missing
            const string Refs = @"<ref>a</ref> <ref>a</ref> <ref>a</ref> <ref>a</ref> <ref>a</ref> <ref>a</ref> <ref>a</ref>";
            Assert.AreEqual(a1 + Refs + Refs + Refs, Parsers.FixPeopleCategories(a1 + Refs + Refs + Refs, "foo"));
        }

        [Test]
        public void FixPeopleCategoriesRefs()
        {
            string refs = @"<ref>foo</ref> and <ref>foo</ref> and <ref>foo</ref> and <ref>foo</ref> and <ref>foo</ref> and <ref>foo</ref> ";

            string Over20Refs = refs + refs + refs + refs + @"'''Fred Smith''' (born 1980) is a bloke. {{Persondata}}";

            Assert.AreEqual(Over20Refs, Parsers.FixPeopleCategories(Over20Refs, "test"), "no change when over 20 refs and no exiting birth/death/living cat");

            Assert.IsTrue(Parsers.FixPeopleCategories(Over20Refs + @" [[Category:Living people]]", "test").Contains(@"[[Category:1980 births]]"), "can add cat when over 20 refs and living people cat already");
        }

        [Test]
        public void FixPeopleCategoriesFutureTest()
        {
            // birth
            const string a1 = @"'''Fred Smith''' (born 2060) is a bloke. {{Persondata}}";
            Assert.AreEqual(a1, Parsers.FixPeopleCategories(a1, "foo"));
        }

        [Test]
        public void YearOfBirthMissingCategory()
        {
            Assert.AreEqual(@"[[Category:Pakistani lawyers]]
[[Category:Attorneys General of Pakistan]]
[[Category:Living people]]
[[Category:1944 births]]", Parsers.FixPeopleCategories(@"[[Category:Pakistani lawyers]]
[[Category:Attorneys General of Pakistan]]
[[Category:Year of birth missing (living people)]]
[[Category:Living people]]
[[Category:1944 births]]", "foo"));

            Assert.AreEqual(@"[[Category:Pakistani lawyers]]
[[Category:Attorneys General of Pakistan]]
[[Category:Living people]]
[[Category:1944 births]]", Parsers.FixPeopleCategories(@"[[Category:Pakistani lawyers]]
[[Category:Attorneys General of Pakistan]]
[[Category:Year of birth missing]]
[[Category:Living people]]
[[Category:1944 births]]", "foo"));

            // no change when already correct
            Assert.AreEqual(@"[[Category:Pakistani lawyers]]
[[Category:Attorneys General of Pakistan]]
[[Category:Living people]]
[[Category:1944 births]]", Parsers.FixPeopleCategories(@"[[Category:Pakistani lawyers]]
[[Category:Attorneys General of Pakistan]]
[[Category:Living people]]
[[Category:1944 births]]", "foo"));

            const string a = @"Mr foo {{persondata}} was great
[[Category:1960 deaths]]";
            Assert.AreEqual(a + "\r\n", Parsers.FixPeopleCategories(a + "\r\n" + @"[[Category:Year of death missing]]", "test"));

        }

        [Test]
        public void GetCategorySortTests()
        {
            Assert.AreEqual(@"", Parsers.GetCategorySort(@"'''Jonathan Sothcott''' (born [[26 April]] [[1980]]) {{persondata}}
{{DEFAULTSORT:Sothcott, Jonathan}}
[[Category:British television producers|Sothcott, Jonathon]]"));

            Assert.AreEqual(@"Sothcott, Jonathan", Parsers.GetCategorySort(@"'''Jonathan Sothcott''' (born [[26 April]] [[1980]]) {{persondata}}
[[Category:British television producers|Sothcott, Jonathan]]
[[Category:Living people|Sothcott, Jonathan]]"));

            Assert.AreEqual(@"Sothcott, Jonathan", Parsers.GetCategorySort(@"'''Jonathan Sothcott''' (born [[26 April]] [[1980]]) {{persondata}}
[[Category:Living people|Sothcott, Jonathan]]"));

            // not all same – null return
            Assert.AreEqual(@"", Parsers.GetCategorySort(@"'''Jonathan Sothcott''' (born [[26 April]] [[1980]]) {{persondata}}
[[Category:British television producers|Sothcott, Jonathan]]
[[Category:Living people|Sothcott, Jonathan]]
[[Category:1944 births]]"));
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
            Assert.AreEqual(@"'''Fred Smith''' (born 1960) is a bloke.
[[Category:1960 births|Smith, Fred]][[Category:Living people|Smith, Fred]]", Parsers.LivingPeople(@"'''Fred Smith''' (born 1960) is a bloke.
[[Category:1960 births|Smith, Fred]]", "A"));

            // no sortkey
            Assert.AreEqual(@"'''Fred Smith''' (born 1960) is a bloke.
[[Category:1960 births]][[Category:Living people]]", Parsers.LivingPeople(@"'''Fred Smith''' (born 1960) is a bloke.
[[Category:1960 births]]", "A"));
            
            // non-mainspace: add with colon
                        Assert.AreEqual(@"'''Fred Smith''' (born 1960) is a bloke.
[[Category:1960 births]][[:Category:Living people]]", Parsers.LivingPeople(@"'''Fred Smith''' (born 1960) is a bloke.
[[Category:1960 births]]", "Wikipedia:Sandbox"));

            // no matches if not identified as born
            const string b1 = @"'''Fred Smith''' is a bloke.";
            Assert.AreEqual(b1, Parsers.LivingPeople(b1, "A"));

            // no matches if identified as dead
            const string a = @"'''Fred Smith''' (born 1960) is a bloke.
[[Category:1960 births]]
[[Category:1990 deaths]]";
            const string b = @"'''Fred Smith''' is a bloke.
[[Category:1960 births]]
[[Category:1990 deaths]]";
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

            Assert.AreEqual(a, Parsers.LivingPeople(a, "A"));
            Assert.AreEqual(b, Parsers.LivingPeople(b, "A"));
            Assert.AreEqual(c, Parsers.LivingPeople(c, "A"));
            Assert.AreEqual(d, Parsers.LivingPeople(d, "A"));
            Assert.AreEqual(e, Parsers.LivingPeople(e, "A"));
            Assert.AreEqual(f, Parsers.LivingPeople(f, "A"));
            Assert.AreEqual(g, Parsers.LivingPeople(g, "A"));
            Assert.AreEqual(h, Parsers.LivingPeople(h, "A"));

            // assume dead if born earlier than 121 years ago, so no change
            const string d1 = @"'''Fred Smith''' (born 1879) is a bloke.
[[Category:1879 births|Smith, Fred]]";
            Assert.AreEqual(d1, Parsers.LivingPeople(d1, "A"));

            // check correctly handles birth category with no year to parse
            const string d2 = @"Fred [[Category:15th-century births]]";
            Assert.AreEqual(d2, Parsers.LivingPeople(d2, "A"));
        }

        [Test]
        public void LivingPeopleTestsEnOnly()
        {
#if DEBUG
            const string Before = @"'''Fred Smith''' (born 1960) is a bloke.
[[Category:1960 births|Smith, Fred]]";

            Variables.SetProjectLangCode("sco");
            Assert.AreEqual(Before, Parsers.LivingPeople(Before, "A"));

            Variables.SetProjectLangCode("en");
            Assert.AreEqual(Before + @"[[Category:Living people|Smith, Fred]]", Parsers.LivingPeople(Before, "A"));
#endif
        }

        [Test]
        public void FixSyntaxCategory()
        {
            const string correct = @"Now [[Category:2005 albums]] there";

            Assert.AreEqual(correct, Parsers.FixSyntax(@"Now {{Category:2005 albums]] there"));
            Assert.AreEqual(correct, Parsers.FixSyntax(@"Now {{Category:2005 albums}} there"));
            Assert.AreEqual(correct, Parsers.FixSyntax(@"Now {{ Category:2005 albums]] there"));
            Assert.AreEqual(correct, Parsers.FixSyntax(@"Now [[Category:2005 albums}} there"));
            Assert.AreEqual(correct, Parsers.FixSyntax(@"Now [[  Category:2005 albums}} there"));

            Assert.AreEqual(correct, Parsers.FixSyntax(correct));
        }


        [Test]
        public void TestFixCategories()
        {
            Assert.AreEqual("[[Category:Foo bar]]", Parsers.FixCategories("[[ categOry : Foo_bar]]"));
            Assert.AreEqual("[[Category:Foo bar|boz]]", Parsers.FixCategories("[[ categOry : Foo_bar|boz]]"));
            Assert.AreEqual("[[Category:Foo bar|quux]]", Parsers.FixCategories("[[category : foo_bar%20|quux]]"));
            Assert.AreEqual(@"[[Category:Foo bar]]", Parsers.FixCategories(@"[[Category:Foo_bar]]"));
            Assert.AreEqual(@"[[Category:Foo bar]]", Parsers.FixCategories(@"[[category:Foo bar]]"));
            Assert.AreEqual(@"[[Category:Foo bar]]", Parsers.FixCategories(@"[[Category::Foo bar]]"));
            Assert.AreEqual(@"[[Category:Foo bar]]", Parsers.FixCategories(@"[[Category  :  Foo_bar  ]]"));
            Assert.AreEqual("[[Category:Foo bar]]", Parsers.FixCategories("[[CATEGORY: Foo_bar]]"));


            // https://en.wikipedia.org/w/index.php?title=Wikipedia_talk:AutoWikiBrowser/Bugs&oldid=262844859#General_fixes_remove_spaces_from_category_sortkeys
            Assert.AreEqual(@"[[Category:Public transport in Auckland| Public transport in Auckland]]", Parsers.FixCategories(@"[[Category:Public transport in Auckland| Public transport in Auckland]]"));
            Assert.AreEqual(@"[[Category:Actors|Fred Astaire]]", Parsers.FixCategories(@"[[Category:Actors|Fred Astaire ]]"), "trailing space IS removed");
            Assert.AreEqual(@"[[Category:Actors|Fred Astaire]]", Parsers.FixCategories(@"[[Category:Actors|Fred Astaire    ]]"), "trailing space IS removed"); 
            Assert.AreEqual(@"[[Category:Actors| Fred Astaire]]", Parsers.FixCategories(@"[[Category:Actors| Fred Astaire ]]"), "trailing space IS removed");
            Assert.AreEqual(@"[[Category:London| ]]", Parsers.FixCategories(@"[[Category:London| ]]"), "leading space NOT removed");
            Assert.AreEqual(@"[[Category:Slam poetry| ]] ", Parsers.FixCategories(@"[[Category:Slam poetry| ]] "), "leading space NOT removed");

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Archive_18#.2Fdoc_pages_and_includeonly_sections
            Assert.AreEqual("[[Category:Foo bar|boz_quux]]", Parsers.FixCategories("[[Category: foo_bar |boz_quux]]"));
            Assert.AreEqual("[[Category:Foo bar|{{boz_quux}}]]", Parsers.FixCategories("[[Category: foo_bar|{{boz_quux}}]]"));
            StringAssert.Contains("{{{boz_quux}}}", Parsers.FixCategories("[[CategorY : foo_bar{{{boz_quux}}}]]"));
            Assert.AreEqual("[[Category:Foo bar|{{{boz_quux}}}]]", Parsers.FixCategories("[[CategorY : foo_bar|{{{boz_quux}}}]]"));

            // diacritics removed from sortkeys
            Assert.AreEqual(@"[[Category:World Scout Committee members|Laine, Juan]]", Parsers.FixCategories(@"[[Category:World Scout Committee members|Lainé, Juan]]"));

            Variables.Namespaces.Remove(Namespace.Category);
            Assert.AreEqual("", Parsers.FixCategories(""), "Fallback to English category namespace name");
            Variables.Namespaces.Add(Namespace.Category,"Category:");
        }

        [Test]
        public void TestFixCategoriesRu()
        {
#if debug
            Variables.SetProjectLangCode("ru");
            Assert.AreEqual(@"[[Category:World Scout Committee members|Lainé, Juan]]", Parsers.FixCategories(@"[[Category:World Scout Committee members|Lainé, Juan]]"), "no diacritic removal for sort key on ru-wiki");
            
            Variables.SetProjectLangCode("en");
            Assert.AreEqual(@"[[Category:World Scout Committee members|Laine, Juan]]", Parsers.FixCategories(@"[[Category:World Scout Committee members|Lainé, Juan]]"));
#endif
        }

        [Test]
        public void TestFixCategoriesBrackets()
        {
            // brackets fixed
            Assert.AreEqual(@"[[Category:London]]", Parsers.FixCategories(@"[[Category:London]]]"));
            Assert.AreEqual(@"[[Category:London]]", Parsers.FixCategories(@"[[Category:London]]]]"));
            Assert.AreEqual(@"[[Category:London]]", Parsers.FixCategories(@"[[[[Category:London]]]"));
            Assert.AreEqual(@"[[Category:London]]", Parsers.FixCategories(@"[[[[Category:London]]]]"));
            Assert.AreEqual(@"[[Category:London]]", Parsers.FixCategories(@"[[Category:London]]"));
        }


	}
}