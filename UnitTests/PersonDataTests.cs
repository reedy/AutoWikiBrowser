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
    public class PersonDataTests : RequiresParser
    {
        public GenfixesTestsBase genFixes  = new GenfixesTestsBase();

        [Test]
        public void PersonDataAddition()
        {
            const string Fred = @"'''Fred''' (born 1960) is.
[[Category:1960 births]]", FredPD = @"'''Fred''' (born 1960) is.
[[Category:1960 births]]
{{Persondata|DATE OF BIRTH=1960 | NAME= Fred}}";

            Assert.IsTrue(Tools.NestedTemplateRegex("persondata").IsMatch(Parsers.PersonData(Fred, "Fred")), "Adds persondata for BLP when missing");
            Assert.IsFalse(Tools.NestedTemplateRegex("persondata").IsMatch(Parsers.PersonData("test", "Fred")), "PersonData not added when not BLP");

            Assert.AreEqual(FredPD, Parsers.PersonData(FredPD, "Fred"), "No change when persondata already present for BLP");

            const string BrokenPersondata = @"{{Persondata<!--Metadata: see [[Wikipedia:Persondata]].-->
|NAME=Orbeliani, Georgy Ilyich
|ALTERNATIVE NAMES=
|SHORT DESCRIPTION=
|DATE OF BIRTH=1853
|PLACE OF BIRTH=[[Tbilisi]], [[Georgia Governorate]], [[Russian Empire]]
|DATE OF DEATH=1924
|PLACE OF DEATH=[[Paris, France]]

{{DEFAULTSORT:Orbeliani, Georgy}}";
            
            Assert.AreEqual(BrokenPersondata, Parsers.PersonData(BrokenPersondata, "Test"), "no change when existing persondata template with unbalanced main brackets");
        }

        [Test]
        public void PersonDataAdditionName()
        {
            string Fred = @"'''Fred''' (born 1960) is.
[[Category:1960 births]]";
            string res = Parsers.PersonData(Fred, "Fred");
            string PD = Tools.NestedTemplateRegex("persondata").Match(res).Value;
            Assert.AreEqual(Tools.GetTemplateParameterValue(PD, "NAME"), "Fred", "Persondata NAME taken from articletitle if no DEFAULTSORT and single word title");

            Fred = @"'''Fred''' (born 1960) is.
[[Category:1960 births]] {{DEFAULTSORT:Smith, Fred}}";
            res = Parsers.PersonData(Fred, "Fred");
            PD = Tools.NestedTemplateRegex("persondata").Match(res).Value;
            Assert.AreEqual(Tools.GetTemplateParameterValue(PD, "NAME"), "Smith, Fred", "Persondata NAME taken from DEFAULTSORT when available, even when single word title");

            Fred = @"'''Fréd''' (born 1960) is.
[[Category:1960 births]] {{DEFAULTSORT:Fred}}";
            res = Parsers.PersonData(Fred, "Fréd");
            PD = Tools.NestedTemplateRegex("persondata").Match(res).Value;
            Assert.AreEqual(Tools.GetTemplateParameterValue(PD, "NAME"), "Fréd", "Persondata NAME includes diacritics");
        }

        [Test]
        public void PersonDataAdditionEnOnly()
        {
#if DEBUG
            const string Fred = @"'''Fred''' (born 1960) is.
[[Category:1960 births]]";

            Variables.SetProjectLangCode("fr");
            Assert.IsFalse(Tools.NestedTemplateRegex("persondata").IsMatch(Parsers.PersonData(Fred, "Fred")), "Adds persondata for BLP when missing");

            Variables.SetProjectLangCode("en");
            Assert.IsTrue(Tools.NestedTemplateRegex("persondata").IsMatch(Parsers.PersonData(Fred, "Fred")), "Adds persondata for BLP when missing");
#endif
        }

        [Test]
        public void PersonDataCompletionDOB()
        {
            const string a = @"{{persondata
            |NAME=
            |DATE OF BIRTH=
            |DATE OF DEATH=}}", a2 = @"{{persondata
            |NAME= Doe, John
            |DATE OF BIRTH= 27 June 1950
            |DATE OF DEATH=}}", i1 = @"{{infobox foo| dateofbirth = 27 June 1950}}", i2 = @"{{infobox foo| dateofbirth = {{birth date|1950|06|27}}}}"
                , i2b = @"{{infobox foo| dateofbirth = {{birth date|df=y|1950|06|27}}}}",
            a3 = @"{{persondata
            |NAME= Doe, John
            |DATE OF BIRTH=
            |DATE OF DEATH=}}";

            Assert.AreEqual(i1 + a2, Parsers.PersonData(i1 + a, "John Doe"));
            Assert.AreEqual(i1 + a2, Parsers.PersonData(i1 + @"{{persondata
            |NAME= Doe, John
            |date of birth=
            |date of death=}}", "John Doe"));
            Assert.AreEqual(i1.Replace("27 June", "June 27,") + a2.Replace("27 June", "June 27,"), Parsers.PersonData(i1.Replace("27 June", "June 27,") + a.Replace("27 June", "June 27,"), "John Doe"));
            Assert.AreEqual(i2 + a2.Replace("27 June 1950", "1950-06-27"), Parsers.PersonData(i2 + a, "John Doe"));
            Assert.AreEqual(i2.Replace(@"{{birth date|1950|06|27}}", @"{{birth-date|27 June 1950|27 June 1950}}") + a2,
                            Parsers.PersonData(i2.Replace(@"{{birth date|1950|06|27}}", @"{{birth-date|27 June 1950|27 June 1950}}") + a, "John Doe"));
            Assert.AreEqual(i2.Replace("27}}", "27}} in London") + a2.Replace("27 June 1950", "1950-06-27"),
                            Parsers.PersonData(i2.Replace("27}}", "27}} in London") + a, "John Doe"), "Completes persondata from {{birth date}} when extra data in infobox field");
            Assert.AreEqual(i2b.Replace("27}}", "27}} in London") + a2,
                            Parsers.PersonData(i2b.Replace("27}}", "27}} in London") + a, "John Doe"), "Completes persondata from {{birth date}} when extra data in infobox field");

            string i3 = i1.Replace("27 June 1950", @"{{dda|2005|07|20|1950|06|27|df=yes}}");
            Assert.AreEqual(i3 + a2.Replace("DEATH=", "DEATH= 20 July 2005"), Parsers.PersonData(i3 + a, "John Doe"), "takes dates from {{dda}}");

            string i4 = @"{{infobox foo| birthyear = 1950 | birthmonth=06 | birthday=27}}";
            Assert.AreEqual(i4 + a2 + @"{{use dmy dates}}", Parsers.PersonData(i4 + a + @"{{use dmy dates}}", "John Doe"), "takes dates from birthyear etc. fields");

            string i5 = @"{{infobox foo| yob = 1950 | mob=06 | dob=27}}";
            Assert.AreEqual(i5 + a2 + @"{{use dmy dates}}", Parsers.PersonData(i5 + a + @"{{use dmy dates}}", "John Doe"), "takes dates from birthyear etc. fields");

            Assert.AreEqual(i1 + a + a, Parsers.PersonData(i1 + a + a, "John Doe"), "no change when multiple personData templates");

            string i6 = i1.Replace("27 June 1950", @"{{dda|2005|07|00|1950|06|00|df=yes}}");
            Assert.AreEqual(i6 + a3, Parsers.PersonData(i6 + a3, "John Doe"), "ignores incomplete/zerod dates from {{dda}}");

            string UnformatedDOB = @"'''Fred''' (born 27 June 1950) was great [[Category:1950 births]]";
            Assert.AreEqual(UnformatedDOB + a2, Parsers.PersonData(UnformatedDOB + a, "John Doe"), "sets full birth date when matches category");

            UnformatedDOB = @"'''Fred''' (born June 27, 1950) was great [[Category:1950 births]] {{use dmy dates}}";
            Assert.AreEqual(UnformatedDOB + a2, Parsers.PersonData(UnformatedDOB + a, "John Doe"), "sets full birth date when matches category, American date");

            UnformatedDOB = UnformatedDOB.Replace(@"[[Category:1950 births]]", "");
            Assert.AreEqual(UnformatedDOB + a3, Parsers.PersonData(UnformatedDOB + a3, "John Doe"), "not set when no birth category");

            UnformatedDOB = @"'''Fred''' (born 27 June 1949) was great [[Category:1950 births]]";
            Assert.AreEqual(UnformatedDOB + a2.Replace("27 June 1950", "1950"), Parsers.PersonData(UnformatedDOB + a, "John Doe"), "not set when full birth date doesn't match category");

            UnformatedDOB = @"'''Fred''' (born circa 27 June 1950) was great [[Category:1950 births]]";
            Assert.AreEqual(UnformatedDOB + a2.Replace("27 June 1950", "1950"), Parsers.PersonData(UnformatedDOB + a, "John Doe"), "only year set when circa date");

            UnformatedDOB = @"'''Fred''' (reigned 27 June 1950 – 11 May 1990) was great";
            Assert.AreEqual(UnformatedDOB + a2.Replace(" 27 June 1950", ""), Parsers.PersonData(UnformatedDOB + a, "John Doe"), "No dates set when reigned");

            UnformatedDOB = @"'''Fred''' (baptized 27 June 1950 – 11 May 1990) was great";
            Assert.AreEqual(UnformatedDOB + a2.Replace(" 27 June 1950", ""), Parsers.PersonData(UnformatedDOB + a, "John Doe"), "No dates set when baptized");

            const string Clark = @"
{{use mdy dates}}
{{Infobox college coach
| name          = Lyal W. Clark
| birth_date   = {{birth date|mf=yes|1904|07|04}}
}}

{{Persondata <!-- Metadata: see [[Wikipedia:Persondata]]. -->
| NAME              = Clark, Lyal W.
| ALTERNATIVE NAMES =
| SHORT DESCRIPTION = American college football coach
| DATE OF BIRTH     =
| PLACE OF BIRTH    =
| DATE OF DEATH     =
| PLACE OF DEATH    =
}}
[[Category:1904 births]]";
            Assert.AreEqual(Clark.Replace(@"| DATE OF BIRTH     =", @"| DATE OF BIRTH     = July 4, 1904"), Parsers.PersonData(Clark, "A"));

            string small = i1.Replace("27 June 1950", @"<small>27 June 1950</small>");
            Assert.AreEqual(small + a2, Parsers.PersonData(small + a, "John Doe"), "small tags removed");

            string twoDDA = i1.Replace("27 June 1950", @"{{dda|2005|07|20|1950|06|27|df=yes}} {{dda|2009|07|20|1950|06|27|df=yes}}");
            Assert.AreEqual(twoDDA + a2, Parsers.PersonData(twoDDA + a, "John Doe"), "Ignores conflicting {{dda}}");
        }

        [Test]
        public void PersonDataCompletionDOBFromCategory()
        {
            string Text = Parsers.PersonData(@"Foo [[Category:Living people]] [[Category:1980 births]]", "test");

            Assert.IsTrue(Tools.GetTemplateParameterValue(WikiRegexes.Persondata.Match(Text).Value, "DATE OF BIRTH").Equals("1980"));

            Text = Parsers.PersonData(@"Foo [[Category:Living people]] [[Category:1980 births|Foo]]", "test");
            Assert.IsTrue(Tools.GetTemplateParameterValue(WikiRegexes.Persondata.Match(Text).Value, "DATE OF BIRTH").Equals("1980"));

            Text = Parsers.PersonData(@"Foo [[Category:Living people]] [[Category:980 BC births|Foo]]", "test");
            Assert.IsTrue(Tools.GetTemplateParameterValue(WikiRegexes.Persondata.Match(Text).Value, "DATE OF BIRTH").Equals("980 BC"));
        }

        [Test]
        public void PersonDataCompletionDOD()
        {
            const string a = @"{{persondata
            |NAME=
            |DATE OF BIRTH=
            |DATE OF DEATH=}}", a2 = @"{{persondata
            |NAME= Doe, John
            |DATE OF BIRTH=
            |DATE OF DEATH= 27 June 1950}}", i1 = @"{{infobox foo| dateofdeath = 27 June 1950}}", i2 = @"{{infobox foo| dateofdeath = {{death date|1950|06|27}}}}";

            Assert.AreEqual(i1 + a2, Parsers.PersonData(i1 + a, "John Doe"));
            Assert.AreEqual(i1.Replace("27 June", "June 27,") + a2.Replace("27 June", "June 27,"), Parsers.PersonData(i1.Replace("27 June", "June 27,") + a.Replace("27 June", "June 27,"), "John Doe"));
            Assert.AreEqual(i2 + a2.Replace("27 June 1950", "1950-06-27"), Parsers.PersonData(i2 + a, "John Doe"));
            Assert.AreEqual(i2.Replace("27}}", "27}} in London") + a2.Replace("27 June 1950", "1950-06-27"),
                            Parsers.PersonData(i2.Replace("27}}", "27}} in London") + a, "John Doe"), "Completes persondata from {{death date}} when extra data in infobox field");

            string i4d = @"{{infobox foo| deathyear = 1950 | deathmonth=06 | deathday=27}}";
            Assert.AreEqual(i4d + a2.Replace(@"DATE OF BIRTH=27 June 1950
            |DATE OF DEATH= ", @"DATE OF BIRTH=
            |DATE OF DEATH= 27 June 1950") + @"{{use dmy dates}}", Parsers.PersonData(i4d + a + @"{{use dmy dates}}", "John Doe"), "takes dates from deathyear etc. fields");

            string i5 = @"{{infobox foo| yod = 1950 | mod=06 | dod=27}}";
            Assert.AreEqual(i5 + a2.Replace(@"DATE OF BIRTH=27 June 1950
            |DATE OF DEATH= ", @"DATE OF BIRTH=
            |DATE OF DEATH= 27 June 1950") + @"{{use dmy dates}}", Parsers.PersonData(i5 + a + @"{{use dmy dates}}", "John Doe"), "takes dates from deathyear etc. fields");

            string u1 = @"Fred (11 May 1920 – 4 June 2004) was great. [[Category:1920 births]] [[Category:2004 deaths]]";
            Assert.AreEqual(@"{{Persondata
|NAME= Doe, John
|DATE OF BIRTH= 11 May 1920
|DATE OF DEATH= 4 June 2004
}}" + u1, Parsers.PersonData(@"{{Persondata
|NAME= Doe, John
|DATE OF BIRTH=
|DATE OF DEATH=
}}" + u1, "John Doe"), "birth and death added from unformatted values");

            u1 = @"Fred (11 May 1920 – 4 June 2004) was great. [[Category:1920 births]] [[Category:2004 deaths]]";
            Assert.AreEqual(@"{{Persondata
|NAME= Doe, John
|DATE OF BIRTH= 11 May 1920
|DATE OF DEATH= 4 June 2004
}}" + u1, Parsers.PersonData(@"{{Persondata
|NAME= Doe, John
|DATE OF BIRTH=
|DATE OF DEATH=
}}" + u1, "John Doe"), "birth and death added from unformatted values");

            u1 = @"Fred (born 11 May 1920; died 4 June 2004) was great. [[Category:1920 births]] [[Category:2004 deaths]]";
            Assert.AreEqual(@"{{Persondata
|NAME= Doe, John
|DATE OF BIRTH= 11 May 1920
|DATE OF DEATH= 4 June 2004
}}" + u1, Parsers.PersonData(@"{{Persondata
|NAME= Doe, John
|DATE OF BIRTH=
|DATE OF DEATH=
}}" + u1, "John Doe"), "birth and death added from unformatted values");
            u1 = @"Fred (born 11 May 1920 {{ndash}} 4 June 2004) was great. [[Category:1920 births]] [[Category:2004 deaths]]";
            Assert.AreEqual(@"{{Persondata
|NAME= Doe, John
|DATE OF BIRTH= 11 May 1920
|DATE OF DEATH= 4 June 2004
}}" + u1, Parsers.PersonData(@"{{Persondata
|NAME= Doe, John
|DATE OF BIRTH=
|DATE OF DEATH=
}}" + u1, "John Doe"), "birth and death added from unformatted values");
            u1 = @"Fred (born 11 May 1920{{ndash}} 4 June 2004) was great. [[Category:1920 births]] [[Category:2004 deaths]]";
            Assert.AreEqual(@"{{Persondata
|NAME= Doe, John
|DATE OF BIRTH= 11 May 1920
|DATE OF DEATH= 4 June 2004
}}" + u1, Parsers.PersonData(@"{{Persondata
|NAME= Doe, John
|DATE OF BIRTH=
|DATE OF DEATH=
}}" + u1, "John Doe"), "birth and death added from unformatted values");
            u1 = @"Fred (born 11 May 1920{{ndash}} June 4, 2004) was great. [[Category:1920 births]] [[Category:2004 deaths]] {{use dmy dates}}";
            Assert.AreEqual(@"{{Persondata
|NAME= Doe, John
|DATE OF BIRTH= 11 May 1920
|DATE OF DEATH= 4 June 2004
}}" + u1, Parsers.PersonData(@"{{Persondata
|NAME= Doe, John
|DATE OF BIRTH=
|DATE OF DEATH=
}}" + u1, "John Doe"), "birth and death added from unformatted values, American format dates");

            u1 = @"Fred (11 May 1920 – 4 June 1000) was great. [[Category:1920 births]] [[Category:2004 deaths]]";
            Assert.AreEqual(@"{{Persondata
|NAME= Doe, John
|DATE OF BIRTH= 11 May 1920
|DATE OF DEATH= 2004
}}" + u1, Parsers.PersonData(@"{{Persondata
|NAME= Doe, John
|DATE OF BIRTH=
|DATE OF DEATH=
}}" + u1, "John Doe"), "unformatted death value not added if doesn't match category");

            u1 = @"Fred (11 May 1920 – {{circa}} 4 June 2004) was great. [[Category:1920 births]] [[Category:2004 deaths]]";
            Assert.AreEqual(@"{{Persondata
|NAME= Doe, John
|DATE OF BIRTH= 1920
|DATE OF DEATH= 2004
}}" + u1, Parsers.PersonData(@"{{Persondata
|NAME= Doe, John
|DATE OF BIRTH=
|DATE OF DEATH=
}}" + u1, "John Doe"), "unformatted death value not added if doesn't match category");

            const string Clark = @"
{{use mdy dates}}
{{Infobox college coach
| name          = Lyal W. Clark
| death_date   = {{death date and age|mf=yes|1904|07|04|1850|11|11}}
}}

{{Persondata <!-- Metadata: see [[Wikipedia:Persondata]]. -->
| NAME              = Clark, Lyal W.
| ALTERNATIVE NAMES =
| SHORT DESCRIPTION = American college football coach
| DATE OF BIRTH     =
| PLACE OF BIRTH    =
| DATE OF DEATH     =
| PLACE OF DEATH    =
}}
[[Category:1904 deaths]]";
            Assert.IsTrue(Parsers.PersonData(Clark, "A").Contains(@"| DATE OF DEATH     = July 4, 1904"));

			const string Question = @"
{{use mdy dates}}
{{Infobox college coach
| name          = Lyal W. Clark
| death_date   = ???
}}

{{Persondata <!-- Metadata: see [[Wikipedia:Persondata]]. -->
| NAME              = Clark, Lyal W.
| ALTERNATIVE NAMES =
| SHORT DESCRIPTION = American college football coach
| DATE OF BIRTH     =
| PLACE OF BIRTH    =
| DATE OF DEATH     =
| PLACE OF DEATH    =
}}
[[Category:1904 deaths]]";
			Assert.IsFalse(Tools.NestedTemplateRegex("persondata").Match(Parsers.PersonData(Question, "A")).Value.Contains(@"???"));
        }

        [Test]
        public void PersonDataParameterCasing()
        {
            const string LowerCase = @"Foo (born 1980) {{Persondata
|name=X, Foo
|date of birth=1980
}}
[[Cateogory:1980 births]]";

            Assert.AreEqual(LowerCase, Parsers.PersonData(LowerCase, "foo x"), "no update when lowercase parameter holds the data already");

            const string LowerCase2 = @"{{Persondata
|name=X, Foo
|date of birth=1980
|DATE OF BIRTH=1980
}}
[[Category:2005 deaths]]";

            Assert.AreEqual(@"{{Persondata
|NAME=X, Foo
|DATE OF BIRTH=1980
| DATE OF DEATH= 2005
}}
[[Category:2005 deaths]]", Parsers.PersonData(LowerCase2, "foo x"), "duplicate fields removed");
        }

        [Test]
        public void PersonDataCompletionDODFromCategory()
        {
            string Text = Parsers.PersonData(@"Foo [[Category:2005 deaths]] [[Category:1930 births]]", "test");

            Assert.IsTrue(Tools.GetTemplateParameterValue(WikiRegexes.Persondata.Match(Text).Value, "DATE OF BIRTH").Equals("1930"));
            Assert.IsTrue(Tools.GetTemplateParameterValue(WikiRegexes.Persondata.Match(Text).Value, "DATE OF DEATH").Equals("2005"));
        }

        [Test]
        public void PersonDataAll()
        {
            const string IB = @"{{Infobox person
|name=James Jerome Hill
|birth_date={{birthdate|1838|9|16}}
|birth_place=[[Guelph/Eramosa, Ontario|Eramosa Township]],<br>[[Ontario]], [[Canada]]
|death_date = {{dda|1916|5|29|1838|9|16}}
|death_place=[[St. Paul, Minnesota|Saint Paul]], [[Minnesota]]
|occupation=Railroad tycoon
|children= 10
}}
{{DEFAULTSORT:Hill, James J.}}", PD = @"{{Persondata
| NAME              = Hill, James J.
| ALTERNATIVE NAMES =
| SHORT DESCRIPTION =
| DATE OF BIRTH     = September 16, 1838
| PLACE OF BIRTH    = [[Guelph/Eramosa, Ontario|Eramosa Township]], [[Ontario]], [[Canada]]
| DATE OF DEATH     = May 29, 1916
| PLACE OF DEATH    = [[St. Paul, Minnesota|Saint Paul]], [[Minnesota]]
}}", AT = @"{{birthdate|1838|9|16}}
{{dda|1916|5|29|1838|9|16}}
{{Infobox foo
|birth_place=[[Guelph/Eramosa, Ontario|Eramosa Township]], [[Ontario]], [[Canada]]
|death_place=[[St. Paul, Minnesota|Saint Paul]], [[Minnesota]]
|children= 10}}
{{DEFAULTSORT:Hill, James J.}}";

            Assert.AreEqual(PD, WikiRegexes.Persondata.Match(Parsers.PersonData(IB + "May 2, 2010 and May 2, 2010", "test")).Value);
            Assert.AreEqual(PD, WikiRegexes.Persondata.Match(Parsers.PersonData(IB.Replace(@"[[Canada]]", @"[[Canada]], {{CAN}}") + "May 2, 2010 and May 2, 2010", "test")).Value, "trims trailing comma template combo");
            Assert.AreEqual(PD, WikiRegexes.Persondata.Match(Parsers.PersonData(AT + "May 2, 2010 and May 2, 2010", "test")).Value, "Values taken from birth/death templates even if outside infobox");

            const string IB2 = @"{{Infobox person
|name=James Jerome Hill
|yearofbirth=1838
|monthofbirth=9
|dayofbirth=16
|birth_place=<small>[[Guelph/Eramosa, Ontario|Eramosa Township]], [[Ontario]], [[Canada]]</small>
|yearofdeath=1916
|monthofdeath=5
|dayofdeath=29
|death_place=[[St. Paul, Minnesota|Saint Paul]], [[Minnesota]]
|occupation=Railroad tycoon
|children= 10
}}
{{DEFAULTSORT:Hill, James J.}}";

            Assert.AreEqual(PD, WikiRegexes.Persondata.Match(Parsers.PersonData(IB2 + "May 2, 2010 and May 2, 2010", "test")).Value, "pulls dates from year/month/day infobox fields");

            Assert.AreEqual(PD, WikiRegexes.Persondata.Match(Parsers.PersonData(IB.Replace(@":Hill, James J.}}", @":Hill, James J. (writer)}}") + "May 2, 2010 and May 2, 2010", "test")).Value, "Occupation cleaned from defaultsort");

            Assert.AreEqual(PD.Replace(@"[[Guelph/Eramosa, Ontario|Eramosa Township]],<br>[[Ontario]], [[Canada]]", "Ontario, Canada"),
                            WikiRegexes.Persondata.Match(Parsers.PersonData(IB.Replace(@"[[Guelph/Eramosa, Ontario|Eramosa Township]], [[Ontario]], [[Canada]]", "{{city-state|Ontario|Canada}}") + "May 2, 2010 and May 2, 2010", "test")).Value, "city state template converted");
            Assert.AreEqual(PD.Replace(@"[[St. Paul, Minnesota|Saint Paul]], [[Minnesota]]", "Saint Paul, Minnesota"),
                            WikiRegexes.Persondata.Match(Parsers.PersonData(IB.Replace(@"[[St. Paul, Minnesota|Saint Paul]], [[Minnesota]]", "Saint Paul, Minnesota}}") + "May 2, 2010 and May 2, 2010", "test")).Value, "city state template converted (death place)");

            Assert.AreEqual(PD, WikiRegexes.Persondata.Match(Parsers.PersonData(IB.Replace(@"[[Canada]]", @"[[Canada]] [[File:Foo.svg|country flag]]") + "May 2, 2010 and May 2, 2010", "test")).Value, "removes country flag from place name");
            Assert.AreEqual(PD, WikiRegexes.Persondata.Match(Parsers.PersonData(IB.Replace(@"[[Canada]]", @"[[Canada]] ???") + "May 2, 2010 and May 2, 2010", "test")).Value, "removes ??? from place name");
            Assert.AreEqual(PD, WikiRegexes.Persondata.Match(Parsers.PersonData(IB.Replace(@"[[Canada]]", @"[[Canada]] <ref>abc</ref>") + "May 2, 2010 and May 2, 2010", "test")).Value, "removes ref from place name");

            Assert.AreEqual(PD, WikiRegexes.Persondata.Match(Parsers.PersonData(IB.Replace(@"{{birthdate|1838|9|16}}", @"1838-09-16") + @"{{use mdy dates}}", "test")).Value, "ISO dates supported");

            Assert.AreEqual(PD.Replace(@"[[Guelph/Eramosa, Ontario|Eramosa Township]],<br>[[Ontario]], [[Canada]]", "[[Guelph/Eramosa, Ontario|Eramosa Township]]"),
                            WikiRegexes.Persondata.Match(Parsers.PersonData(IB.Replace(@"[[Guelph/Eramosa, Ontario|Eramosa Township]], [[Ontario]], [[Canada]]", @"[[Guelph/Eramosa, Ontario|Eramosa Township]],<br>") + "May 2, 2010 and May 2, 2010", "test")).Value, "city state template converted");


            Assert.AreEqual(PD.Replace("PLACE OF BIRTH", "place of birth"), WikiRegexes.Persondata.Match(Parsers.PersonData(IB + PD.Replace("PLACE OF BIRTH", "place of birth") + "May 2, 2010 and May 2, 2010", "test")).Value);
        }

        [Test]
        public void PersondataCleanup()
        {
          const string PD = @"{{Persondata
| NAME              = Hill, James J.
| ALTERNATIVE NAMES =
| SHORT DESCRIPTION = Politician
| DATE OF BIRTH     = September 16, 1838
| PLACE OF BIRTH    = Calgary, [[Canada]]
| DATE OF DEATH     = May 29, 1916
| PLACE OF DEATH    = [[Minnesota]]
}}";
          Assert.AreEqual(PD, Parsers.PersonData(PD.Replace("1838", "<small>1838</small>"), "Test"), "Small tag removal");

          string PD1 = @"{{Persondata
| NAME              = Hill, James J.
| ALTERNATIVE NAMES =
| SHORT DESCRIPTION = Politician
| DATE OF BIRTH     = September 16, 1838
| PLACE OF BIRTH    = Calgary, <small>[[Canada]]</small>
| DATE OF DEATH     = May 29, 1916
| PLACE OF DEATH    = [[Minnesota]]
}}";
          Assert.AreEqual(PD, Parsers.PersonData(PD1, "Test"), "Balanced small tag removal");

                    string PD2 = @"{{Persondata
| NAME              = Hill, James J.
| ALTERNATIVE NAMES =
| SHORT DESCRIPTION = Politician
| DATE OF BIRTH     = September 16, 1838
| PLACE OF BIRTH    = Calgary, [[Canada]]</small>
| DATE OF DEATH     = May 29, 1916
| PLACE OF DEATH    = [[Minnesota]]
}}";

          Assert.AreEqual(PD, Parsers.PersonData(PD2, "Test"), "Unbalanced small tag removal");

                    string PD3 = @"{{Persondata
| NAME              = Hill, James J.
| ALTERNATIVE NAMES =
| SHORT DESCRIPTION = <small>Politician</small>
| DATE OF BIRTH     = September 16, 1838
| PLACE OF BIRTH    = Calgary, [[Canada]]</small>
| DATE OF DEATH     = May 29, 1916
| PLACE OF DEATH    = [[Minnesota]]
}}";

          Assert.AreEqual(PD, Parsers.PersonData(PD3, "Test"), "Small tag removal");
        }
	}
}