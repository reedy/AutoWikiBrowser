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
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace UnitTests
{
    [TestFixture]
    public class UtilityFunctionTests : RequiresParser
    {
        [SetUp]
        public void SetUp()
        {
#if DEBUG
            Variables.SetProjectLangCode("en");
#endif
        }

        [Test]
        public void ChangeToDefaultSort()
        {
            bool noChange;

            // don't change sorting for single categories
            Assert.That(Parsers.ChangeToDefaultSort("[[Category:Test1|Foooo]]", "Foo", out noChange),
                            Is.EqualTo("[[Category:Test1|Foooo]]"), "don't change sorting for single categories");
            Assert.IsTrue(noChange, "don't change sorting for single categories");

            // should work
            Assert.That(Parsers.ChangeToDefaultSort("[[Category:Test1|Foooo]][[Category:Test2|Foooo]]", "Bar",
                                                        out noChange),
                            Is.EqualTo("[[Category:Test1]][[Category:Test2]]\r\n{{DEFAULTSORT:Foooo}}"));
            Assert.IsFalse(noChange);

            // ...but don't add DEFAULTSORT if the key equals page title
            Assert.That(Parsers.ChangeToDefaultSort("[[Category:Test1|Foooo]][[Category:Test2|Foooo]]", "Foooo",
                                                        out noChange),
                            Is.EqualTo("[[Category:Test1]][[Category:Test2]]"));
            Assert.IsFalse(noChange, "Should detect a change even if it hasn't added a DEFAULTSORT");

            // don't change if key is 3 chars or less
            Assert.That(Parsers.ChangeToDefaultSort("[[Category:Test1|Foo]][[Category:Test2|Foo]]", "Bar", out noChange),
                            Is.EqualTo("[[Category:Test1|Foo]][[Category:Test2|Foo]]"));
            Assert.IsTrue(noChange);

            // Remove explicit keys equal to page title
            Assert.That(Parsers.ChangeToDefaultSort("[[Category:Test1|Foooo]][[Category:Test2]]", "Foooo", out noChange),
                            Is.EqualTo("[[Category:Test1]][[Category:Test2]]"));
            Assert.IsFalse(noChange);

            // swap
            Assert.That(Parsers.ChangeToDefaultSort("[[Category:Test1]][[Category:Test2|Foooo]]", "Foooo", out noChange),
                            Is.EqualTo("[[Category:Test1]][[Category:Test2]]"));
            Assert.IsFalse(noChange);

            // Borderline condition
            Assert.That(Parsers.ChangeToDefaultSort("[[Category:Test1|Fooooo]][[Category:Test2]]", "Foooo", out noChange),
                            Is.EqualTo("[[Category:Test1|Fooooo]][[Category:Test2]]"));
            Assert.IsTrue(noChange);

            // Don't change anything if there's ambiguity
            Assert.That(Parsers.ChangeToDefaultSort("[[Category:Test1|Foooo]][[Category:Test2|Baaar]]", "Teeest",
                                                        out noChange),
                            Is.EqualTo("[[Category:Test1|Foooo]][[Category:Test2|Baaar]]"));
            Assert.IsTrue(noChange);
            // same thing
            Assert.That(Parsers.ChangeToDefaultSort("[[Category:Test1|Foooo]][[Category:Test2|Baaar]]", "Foooo",
                                                        out noChange),
                            Is.EqualTo("[[Category:Test1|Foooo]][[Category:Test2|Baaar]]"));
            Assert.IsTrue(noChange);

            // remove diacritics when generating a key
            Assert.That(Parsers.ChangeToDefaultSort("[[Category:Test1|Foooô]][[Category:Test2|Foooô]]", "Bar",
                                                        out noChange),
                            Is.EqualTo("[[Category:Test1]][[Category:Test2]]\r\n{{DEFAULTSORT:Foooo}}"));
            Assert.IsFalse(noChange);

            // should also fix diacritics in existing defaultsorts and remove leading spaces
            // also support mimicking templates: template to magic word conversion, see [[Category:Pages which use a template in place of a magic word]]
            Assert.That(Parsers.ChangeToDefaultSort("{{defaultsort| Tést}}", "Foo", out noChange), Is.EqualTo("{{DEFAULTSORT:Test}}"));
            Assert.IsFalse(noChange);
            Assert.That(Parsers.ChangeToDefaultSort("{{DEFAULTSORT| Tést}}", "Foo", out noChange), Is.EqualTo("{{DEFAULTSORT:Test}}"));
            Assert.IsFalse(noChange);
            Assert.That(Parsers.ChangeToDefaultSort("{{DEFAULTSORT:|Test}}", "Foo", out noChange), Is.EqualTo("{{DEFAULTSORT:Test}}"));
            Assert.IsFalse(noChange);

            // shouldn't change whitespace-only sortkeys
            Assert.That(Parsers.ChangeToDefaultSort("{{DEFAULTSORT: \t}}", "Foo", out noChange), Is.EqualTo("{{DEFAULTSORT: \t}}"));
            Assert.IsTrue(noChange);

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_6#DEFAULTSORT_with_spaces
            // DEFAULTSORT doesn't treat leading spaces the same way as categories do
            Assert.That(Parsers.ChangeToDefaultSort("[[Category:Test1| Foooo]][[Category:Test2| Foooo]]", "Bar",
                                                        out noChange),
                            Is.EqualTo("[[Category:Test1| Foooo]][[Category:Test2| Foooo]]"));
            Assert.IsTrue(noChange);

            // pages with multiple sort specifiers shouldn't be changed
            Parsers.ChangeToDefaultSort("{{defaultsort| Tést}}{{DEFAULTSORT: Tést}}", "Foo", out noChange);
            Assert.IsTrue(noChange);

            // Remove explicitally defined sort keys from categories when the page has defaultsort
            Assert.That(Parsers.ChangeToDefaultSort("{{DEFAULTSORT:Test}}[[Category:Test|Test]]", "Foo", out noChange),
                            Is.EqualTo("{{DEFAULTSORT:Test}}[[Category:Test]]"));
            Assert.IsFalse(noChange);

            // Case difference of above
            Assert.That(Parsers.ChangeToDefaultSort("{{DEFAULTSORT:Test}}[[Category:Test|TEST]]", "Foo", out noChange),
                            Is.EqualTo("{{DEFAULTSORT:Test}}[[Category:Test]]"));
            Assert.IsFalse(noChange);

            // No change due to different key
            Assert.That(Parsers.ChangeToDefaultSort("{{DEFAULTSORT:Test}}[[Category:Test|Not a Test]]", "Foo", out noChange),
                            Is.EqualTo("{{DEFAULTSORT:Test}}[[Category:Test|Not a Test]]"));
            Assert.IsTrue(noChange);

            // Multiple to be removed
            Assert.That(Parsers.ChangeToDefaultSort("{{DEFAULTSORT:Test}}[[Category:Test|TEST]][[Category:Foo|Test]][[Category:Bar|test]]", "Foo", out noChange),
                            Is.EqualTo("{{DEFAULTSORT:Test}}[[Category:Test]][[Category:Foo]][[Category:Bar]]"));
            Assert.IsFalse(noChange);

            // Multiple with 1 no key
            Assert.That(Parsers.ChangeToDefaultSort("{{DEFAULTSORT:Test}}[[Category:Test|TEST]][[Category:Foo]][[Category:Bar|test]]", "Foo", out noChange),
                            Is.EqualTo("{{DEFAULTSORT:Test}}[[Category:Test]][[Category:Foo]][[Category:Bar]]"));
            Assert.IsFalse(noChange);

            // Multiple with 1 different key
            Assert.That(Parsers.ChangeToDefaultSort("{{DEFAULTSORT:Test}}[[Category:Test|TEST]][[Category:Foo|Bar]][[Category:Bar|test]]", "Foo", out noChange),
                            Is.EqualTo("{{DEFAULTSORT:Test}}[[Category:Test]][[Category:Foo|Bar]][[Category:Bar]]"));
            Assert.IsFalse(noChange);

            // just removing diacritics in categories is useful
            Assert.That(Parsers.ChangeToDefaultSort(@"[[Category:Bronze Wolf awardees|Lainé, Juan]]", "Hi", out noChange),
                            Is.EqualTo(@"[[Category:Bronze Wolf awardees|Laine, Juan]]"));
            Assert.IsFalse(noChange);
            Assert.That(Parsers.ChangeToDefaultSort(@"[[Category:Bronze Wolf awardees|Lainİ, Juan]]", "Hi", out noChange),
                            Is.EqualTo(@"[[Category:Bronze Wolf awardees|LainI, Juan]]"), "unusual one where lowercase of diacritic is a Latin character");
            Assert.IsFalse(noChange);

            Assert.That(Parsers.ChangeToDefaultSort(@"[[Category:Bronze Wolf awardees|Laine, Juan]]", "Hi", out noChange),
                            Is.EqualTo(@"[[Category:Bronze Wolf awardees|Laine, Juan]]"));
            Assert.IsTrue(noChange);

            // remove duplicate defaultsorts
            Assert.That(Parsers.ChangeToDefaultSort(@"foo
{{DEFAULTSORT:Phillips, James M.}}
[[Category:Businesspeople]]

{{DEFAULTSORT:Phillips, James M.}}
foo", "Hi", out noChange), Is.EqualTo(@"foo

[[Category:Businesspeople]]

{{DEFAULTSORT:Phillips, James M.}}
foo"));

            Assert.That(Parsers.ChangeToDefaultSort(@"foo
{{DEFAULTSORT:Phillips, James M.}}
[[Category:Businesspeople]]

{{DEFAULTSORT:Phillips, James M.}}
{{DEFAULTSORT:Phillips, James M.}}
foo", "Hi", out noChange), Is.EqualTo(@"foo

[[Category:Businesspeople]]


{{DEFAULTSORT:Phillips, James M.}}
foo"));

            // don't remove duplicate different defaultsorts
            Assert.That(Parsers.ChangeToDefaultSort(@"foo
{{DEFAULTSORT:Phillips, James M.}}
[[Category:Businesspeople]]

{{DEFAULTSORT:Fred}}
foo", "Hi", out noChange), Is.EqualTo(@"foo
{{DEFAULTSORT:Phillips, James M.}}
[[Category:Businesspeople]]

{{DEFAULTSORT:Fred}}
foo"));

            Assert.That(Parsers.ChangeToDefaultSort(@"
foo {{persondata}}
[[Category:1910 births|Lahiff, Tommy]]
[[Category:Australian players of Australian rules football|Lahiff, Tommy]]
[[Category:Essendon Football Club players|Lahiff, Tommy]]
", "foo", out noChange, false), Is.EqualTo(@"
foo {{persondata}}
[[Category:1910 births]]
[[Category:Australian players of Australian rules football]]
[[Category:Essendon Football Club players]]

{{DEFAULTSORT:Lahiff, Tommy}}"));

            // can't add a DEFAULTSORT using existing cat sortkeys even if restricted, as sortkey case may be changed
            const string a = @"
foo {{persondata}}
[[Category:1910 births|Lahiff, Tommy]]
[[Category:Australian players of Australian rules football|Lahiff, Tommy]]
[[Category:Essendon Football Club players|Lahiff, Tommy]]
";
            Assert.That(Parsers.ChangeToDefaultSort(a, "foo", out noChange, true), Is.EqualTo(a));

            Assert.That(Parsers.ChangeToDefaultSort(a, "foo", out noChange, false), Is.EqualTo(@"
foo {{persondata}}
[[Category:1910 births]]
[[Category:Australian players of Australian rules football]]
[[Category:Essendon Football Club players]]

{{DEFAULTSORT:Lahiff, Tommy}}"));

            // can't add a DEFAULTSORT using existing cat sortkeys if they're different
            Assert.That(Parsers.ChangeToDefaultSort(@"
foo {{persondata}}
[[Category:1910 births|Lahiff, Tommy]]
[[Category:Australian players of Australian rules football|Lahiff, Tommy]]
[[Category:Essendon Football Club players|TOmmy]]
", "foo", out noChange, true), Is.EqualTo(@"
foo {{persondata}}
[[Category:1910 births|Lahiff, Tommy]]
[[Category:Australian players of Australian rules football|Lahiff, Tommy]]
[[Category:Essendon Football Club players|TOmmy]]
"));

            // restricted
            const string r1 = @"[[Category:Franks]]
[[Category:Carolingian dynasty]]
[[Category:Frankish people]]
[[Category:811 deaths]]
[[Category:9th-century rulers]]";
            Assert.That(Parsers.ChangeToDefaultSort(r1, "foo", out noChange, true), Is.EqualTo(r1));

            // namespace not used in DEFAULTSORT key
            Assert.That(Parsers.ChangeToDefaultSort(@"foo
[[Category:All foos]]", "Category:Special foŏs", out noChange, false), Is.EqualTo(@"foo
[[Category:All foos]]
{{DEFAULTSORT:Special foos}}"));

            Assert.That(Parsers.ChangeToDefaultSort(@"foo
[[Category:All foos]]
[[Category:All foos2]]", "Rail in İzmir", out noChange, false), Is.EqualTo(@"foo
[[Category:All foos]]
[[Category:All foos2]]
{{DEFAULTSORT:Rail in Izmir}}"));

            // hyphen in title becomes a minus in DEFAULTSORT key
            Assert.That(Parsers.ChangeToDefaultSort(@"foo
{{DEFAULTSORT:Women's Circuit (July–September)}}", "Women's Circuit (July–September)", out noChange, false), Is.EqualTo(@"foo
{{DEFAULTSORT:Women's Circuit (July-September)}}"));

            // skip when nonclude on page
            const string NoInclude = @"[[Category:Test1|Foooo]][[Category:Test2|Foooo]] <noinclude>foo</noinclude>";
            Assert.That(Parsers.ChangeToDefaultSort(NoInclude, "Bar",
                                                        out noChange),
                            Is.EqualTo(NoInclude));
            Assert.IsTrue(noChange);

            Variables.UnicodeCategoryCollation = true;
            Assert.That(Parsers.ChangeToDefaultSort("[[Category:Test1|Foooô]][[Category:Test2|Foooô]]", "Bar",
                                                        out noChange),
                            Is.EqualTo("[[Category:Test1]][[Category:Test2]]\r\n{{DEFAULTSORT:Foooô}}"));
            Assert.IsFalse(noChange, "retain diacritics when generating a key if uca collation is on");

            Assert.That(Parsers.ChangeToDefaultSort("[[Category:Test1]][[Category:Test2]]", "Foooô",
                                                        out noChange),
                            Is.EqualTo(@"[[Category:Test1]][[Category:Test2]]"));
            Assert.IsTrue(noChange, "No change when defaultsort not needed when uca collation on");
            Variables.UnicodeCategoryCollation = false;
        }

        [Test]
        public void MissingDefaultSort()
        {
            Assert.IsFalse(Parsers.MissingDefaultSort(@"A", @"A"));
            Assert.IsFalse(Parsers.MissingDefaultSort(@"A {{DEFAULTSORT:A}}", @"A {{DEFAULTSORT:A}}"));
            Assert.IsFalse(Parsers.MissingDefaultSort(@"A {{DEFAULTSORT:A}} [[category:A]]", @"A"));
            Assert.IsTrue(Parsers.MissingDefaultSort(@"A
[[Category:1910 births]]", @"John Smith"));
        }

        [Test]
        public void ChangeToDefaultsortCaseInsensitive()
        {
            bool noChange;
            const string CInsensitive = @"x [[Category:Foo]]";

            Assert.That(Parsers.ChangeToDefaultSort(CInsensitive, "BAR", out noChange), Is.EqualTo(CInsensitive), "no change when defaultsort only differs to article title by case");
            Assert.IsTrue(noChange);

            Assert.That(Parsers.ChangeToDefaultSort(CInsensitive, "Bar", out noChange), Is.EqualTo(CInsensitive), "no change when defaultsort only differs to article title by case");
            Assert.IsTrue(noChange);

            Assert.That(Parsers.ChangeToDefaultSort(CInsensitive, "Bar foo", out noChange), Is.EqualTo(CInsensitive), "no change when defaultsort only differs to article title by case");
            Assert.IsTrue(noChange);

            Assert.That(Parsers.ChangeToDefaultSort(CInsensitive, "Bar (foo)", out noChange), Is.EqualTo(CInsensitive), "no change when defaultsort only differs to article title by case");
            Assert.IsTrue(noChange);

            string CInsensitive2 = @"{{DEFAULTSORT:Bar}} [[Category:Foo]]";

            Assert.That(Parsers.ChangeToDefaultSort(CInsensitive2, "BAR", out noChange), Is.EqualTo(CInsensitive2), "no change when existing defaultsort only differs to article title by case");
            Assert.IsTrue(noChange);

            CInsensitive2 = @"{{DEFAULTSORT:bar}} [[Category:Foo]]";

            Assert.That(Parsers.ChangeToDefaultSort(CInsensitive2, "BAR", out noChange), Is.EqualTo(CInsensitive2), "no change when existing defaultsort only differs to article title by case");
            Assert.IsTrue(noChange);

            CInsensitive2 = @"{{DEFAULTSORT:BAR}} [[Category:Foo]]";

            Assert.That(Parsers.ChangeToDefaultSort(CInsensitive2, "BAR", out noChange), Is.EqualTo(CInsensitive2), "no change when existing defaultsort only differs to article title by case");
            Assert.IsTrue(noChange);
        }

        [Test]
        public void ChangeToDefaultSortPAGENAME()
        {
            bool noChange;

            Assert.That(Parsers.ChangeToDefaultSort("[[Category:Test1|{{PAGENAME}}]][[Category:Test2]]", "Foooo", out noChange),
                            Is.EqualTo("[[Category:Test1]][[Category:Test2]]"));
            Assert.IsFalse(noChange);

            Assert.That(Parsers.ChangeToDefaultSort("[[Category:Test1|{{subst:PAGENAME}}]][[Category:Test2]]", "Foooo", out noChange),
                            Is.EqualTo("[[Category:Test1]][[Category:Test2]]"));
            Assert.IsFalse(noChange);
        }

        [Test]
        public void ChangeToDefaultSortMultiple()
        {
            bool noChange;
            const string Multi = "[[Category:Test1|Foooo]][[Category:Test2|Foooo]]\r\n{{DEFAULTSORT:Foooo}}\r\n{{DEFAULTSORTKEY:Foo2oo}}";

            Assert.That(Parsers.ChangeToDefaultSort(Multi, "Bar", out noChange), Is.EqualTo(Multi), "no change when multiple different defaultsorts");
            Assert.IsTrue(noChange);
        }

        [Test]
        public void CategorySortKeyPartialCleaning()
        {
            // to test that if a cat's sortkey is the start of the defaultsort key, it's removed too
            bool noChange;
            Assert.That(Parsers.ChangeToDefaultSort(@"{{DEFAULTSORT:Willis, Bobby}}
[[Category:1999 deaths|Willis]]
[[Category:1942 births]]
[[Category:Cancer deaths in the United Kingdom]]", "Bobby Willis", out noChange), Is.EqualTo(@"{{DEFAULTSORT:Willis, Bobby}}
[[Category:1999 deaths]]
[[Category:1942 births]]
[[Category:Cancer deaths in the United Kingdom]]"));

            Assert.IsFalse(noChange);

            Assert.That(Parsers.ChangeToDefaultSort(@"{{DEFAULTSORT:Willis, Bobby}}
[[Category:1999 deaths|Willis]]
[[Category:1942 births|Foo]]
[[Category:Cancer deaths in the United Kingdom]]", "Bobby Willis", out noChange), Is.EqualTo(@"{{DEFAULTSORT:Willis, Bobby}}
[[Category:1999 deaths]]
[[Category:1942 births|Foo]]
[[Category:Cancer deaths in the United Kingdom]]"));

            Assert.IsFalse(noChange);
        }

        [Test]
        public void ChangeToDefaultSortHuman()
        {
            bool noChange;

            const string a = @"Fred Smith blah [[Category:Living people]]";
            const string b = "\r\n" + @"{{DEFAULTSORT:Smith, Fred}}";

            Assert.That(Parsers.ChangeToDefaultSort(a, "Fred Smith", out noChange), Is.EqualTo(a + b));
            Assert.IsFalse(noChange);

            string a2 = @"Fred Smith blah {{imdb name|id=abc}} [[Category:Living people]]";

            Assert.That(Parsers.ChangeToDefaultSort(a2, "Fred Smith", out noChange), Is.EqualTo(a2 + b));
            Assert.IsFalse(noChange);

            // no defaultsort added if restricted defaultsort addition on
            Assert.That(Parsers.ChangeToDefaultSort(a, "Fred Smith", out noChange, true), Is.EqualTo(a));
            Assert.IsTrue(noChange);

            const string c = @"Stéphanie Mahieu blah [[Category:Living people]]";
            const string d = "\r\n" + @"{{DEFAULTSORT:Mahieu, Stephanie}}";

            Assert.That(Parsers.ChangeToDefaultSort(c, "Stéphanie Mahieu", out noChange), Is.EqualTo(c + d));
            Assert.IsFalse(noChange);
        }

        [Test]
        public void TestDefaultsortTitlesWithDiacritics()
        {
            bool noChange;

            Assert.That(Parsers.ChangeToDefaultSort(@"[[Category:Parishes in Asturias]]", "Abándames", out noChange),
                            Is.EqualTo(@"[[Category:Parishes in Asturias]]
{{DEFAULTSORT:Abandames}}"));
            Assert.IsFalse(noChange);

            // no change if a defaultsort already there
            Assert.That(Parsers.ChangeToDefaultSort(@"[[Category:Parishes in Asturias]]
{{DEFAULTSORT:Bert}}", "Abándames", out noChange),
                            Is.EqualTo(@"[[Category:Parishes in Asturias]]
{{DEFAULTSORT:Bert}}"));
            Assert.IsTrue(noChange);


            // category sortkeys are cleaned too
            Assert.That(Parsers.ChangeToDefaultSort(@"[[Category:Parishes of the Azores|Agua Retorta]]
[[Category:São Miguel Island]]", @"Água Retorta", out noChange), Is.EqualTo(@"[[Category:Parishes of the Azores]]
[[Category:São Miguel Island]]
{{DEFAULTSORT:Agua Retorta}}"));
            Assert.IsFalse(noChange);

            // use article name
            Assert.That(Parsers.ChangeToDefaultSort(@"[[Category:Parishes of the Azores]]
[[Category:São Miguel Island]]", @"Água Retorta", out noChange), Is.EqualTo(@"[[Category:Parishes of the Azores]]
[[Category:São Miguel Island]]
{{DEFAULTSORT:Agua Retorta}}"));
            Assert.IsFalse(noChange);
        }

        [Test]
        public void TestIsArticleAboutAPerson()
        {
            Assert.IsTrue(Parsers.IsArticleAboutAPerson(@"Foo {{infobox person|name=smith}}", "foo"));
            Assert.IsTrue(Parsers.IsArticleAboutAPerson(@"Foo [[Category:1900 deaths]]", "foo"));
            Assert.IsTrue(Parsers.IsArticleAboutAPerson(@"Foo [[Category:1900 births]]", "foo"));
            Assert.IsTrue(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Living people]]", "foo"));
            Assert.IsTrue(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Year of birth missing (living people)]]", "foo"));
            Assert.IsTrue(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Living people|Smith]]", "foo"));

            Assert.IsTrue(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Living people]] [[Category:Living people]]", "foo"), "duplicate categories removed, so okay");

            Assert.IsTrue(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{England-bio-stub}}", "foo"));
            Assert.IsTrue(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{Switzerland-politician-stub}}", "foo"));
            Assert.IsTrue(Parsers.IsArticleAboutAPerson(@"Some words {{death date and age|1960|01|9}}", "foo"));
            Assert.IsTrue(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{RefimproveBLP}}", "foo"),"BLP template");
            Assert.IsTrue(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "foo"),"BLP sources template");
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "16 and pregnant"),"BLP sources template");
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP unsourced section|foo=bar}}", "foo"),"BLP unsourced template");

            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo {{Infobox actor|name=smith}}", "Category:foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "List of foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Lists of foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' [[Category:Missing people organizations]]", "Foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Deaths in 2004"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "First Assembly of X"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Pierre Schaeffer bibliography"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Adoption of x"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo (family)"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo (x family)"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo (x team)"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo (publisher)"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo haunting"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo martyrs"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo quartet"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo team"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo twins"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Attack on x"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Suicide of x"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Presidency of x"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Governor of x"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Mayoralty of x"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "First presidency of x"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "2004 something"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "2004–09 something"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo discography"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo children"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo murders"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo, bar and other"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo & other"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo, bar, and Other"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo One and Other People"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo groups"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "The Foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo people"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo campaign"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo rebellion"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo native"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo center"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Second Foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo x families"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Brothers Foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "X from Y"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo brothers"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo Sisters"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "X Service"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo (artists)"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo x families (bar)"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "X in Y"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo campaign, 2000"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Atlanta murders of 1979–1981"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Death rates in the 20th century"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Birth rates in the 20th century"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Death rates in the 1st century"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Death rates in the 2nd century"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Death rates in the 3nd century"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:1900 deaths]] and [[Category:1905 deaths]]", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo {{infobox some organization|foo=bar}} {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Some noble families]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Noble families]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:X teams and stables]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:German families]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:German x families]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:X diaspora in y]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Baronies x]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Groups x]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:X royal families]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:X nicknames]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:X pageants]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:X groups]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:X magazines]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Positions x]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:X troupes]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:X y groups]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:1900 establishments in X]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:X gods]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Companies foo]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Surnames]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:X musical groups]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:X bands]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:X music groups]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:X titles]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Performing groups]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:X ethnic groups]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:X artist groups in]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Musical groups established in 2000]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo {{infobox television|bye=a}} {{refimproveBLP}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{England-bio-stub}} {{sia}}", "Foo"));

            Assert.IsTrue(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Afrikaner people]] {{foo-bio-stub}}", "foo"),"category about people");

            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo {{Infobox settlement}} {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo {{italic title}} {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo {{Infobox racehorse}} {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo {{Infobox named horse}} {{foo-bio-stub}}", "foo"));

            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Married couples]] {{infoxbox actor|name=smith}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:2002 animal births]] {{infoxbox actor|name=smith}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Comedy duos]] {{infoxbox actor|name=smith}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Comedy trios]] {{infoxbox actor|name=smith}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Foo Comedy duos]] {{infoxbox actor|name=smith}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo {{In-universe}} {{infoxbox actor|name=smith}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo {{in-universe}} {{infoxbox actor|name=smith}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo {{Infobox political party}} {{birth date and age|1974|11|26}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Articles about multiple people]] {{infoxbox actor|name=smith}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Fictional blah]] {{infoxbox actor|name=smith}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[fictional character]] {{infoxbox actor|name=smith}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[fictional character|character]] {{infoxbox actor|name=smith}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo {{dab}} {{infoxbox actor|name=smith}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Internet memes]] {{bda|1980|11|11}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Military animals]] {{bda|1980|11|11}}", "foo"));

            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:2002 births]] {{infobox Musical artist|Background=group_or_band}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:2002 births]] {{infobox Musical artist|background=group_or_band}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:2002 births]] {{infobox musical artist|Background=group_or_band}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:2002 births]] {{infobox Musical artist|Background=classical_ensemble}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:2002 births]] {{Infobox Chinese-language singer and actor|currentmembers=A, B}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:2002 births]] {{infobox Band|Background=group_or_band}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:2002 births]] {{infobox Musical artist|Background=band}}", "foo"));
            Assert.IsTrue(Parsers.IsArticleAboutAPerson(@"Foo [[Category:2002 births]] {{infobox musical artist|Background=other}}", "foo"));

            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo {{infobox person|name=smith}} Foo {{infobox person|name=smith2}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo {{death date|2002}} {{death date|2005}}", "foo"));

            // multiple different birth dates means not about one person
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"{{nat fs player|no=1|pos=GK|name=[[Meg]]|age={{Birth date|1956|01|01}} ({{Age at date|1956|01|01|1995|6|5}})|caps=|club=|clubnat=}}
{{nat fs player|no=2|pos=MF|name=[[Valeria]]|age={{Birth date|1968|09|03}} ({{Age at date|1968|09|03|1995|6|5}})|caps=|club=|clubnat=}}", "foo"));
            Assert.IsTrue(Parsers.IsArticleAboutAPerson(@"{{infobox actor|no=1|pos=GK|name=[[Meg]]|age={{Birth date|1956|01|01}} }} {{Birth date|1956|01|01}}", "foo"));

            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"{{see also|Fred}} Fred Smith is great == foo == {{infoxbox actor}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"{{Main|Fred}} Fred Smith is great == foo == {{infoxbox actor}}", "foo"));

            // link in bold in zeroth section to somewhere else is no good
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''military career of [[Napoleon Bonaparte]]''' == foo == {{birth date|2008|11|11}}", "foo"));

            // 'characters' category means fictional person
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"foo [[Category:227 characters]] {{infoxbox actor}}", "foo"));

            Assert.IsTrue(Parsers.IsArticleAboutAPerson(@"'''Margaret Sidney''' was the [[pseudonym]] of American author '''Harriett Mulford Stone''' (June 22, 1844–August 2, 1924).
[[Category:1844 births]]", "foo"),"births category");
            
            string AR = @"{{Infobox rugby biography
| birth_name = Opeti Fonua
| birth_date ={{birth date and age|df=yes|1986|05|26}}
| birth_place = [[Tonga]]
| height = 1.97 m (6 ft 5.5 in)
| weight = 140 kg (22 st 2 lb, 310 lb)
| ru_position = Backrow, Secondrow
| ru_amateurclubs =
| ru_clubyears = 2005-2007<br />2007-2008<br />2008-
| ru_proclubs = [[SU Agen Lot-et-Garonne|SU Agen]]<br />[[Football club Auch Gers|FC Auch]]<br />[[Sporting union Agen Lot-et-Garonne|SU Agen]]
| ru_clubcaps = 34<ref name=""itsrugby""/><br /> 12<ref name=""itsrugby"">{{Lien web |url=http://www.itsrugby.fr/joueur-3640.html |titre=Opeti Fonua |site=www.itsrugby.fr |consulté le=30</ref><br />81<ref name=""itsrugby""/>
| ru_clubpoints = 10 (2 tries)<br />0<br />135 (23 tries)
| ru_nationalyears = 2009-
| ru_nationalteam = {{ru|Tonga}}
| ru_nationalcaps = 2
| ru_nationalpoints = (0)
}}

'''Opeti Fonua''', born [[26 May]] [[1986]], is a [[Tongan]] [[rugby union]] player who plays as a no.8 for French club [[SU Agen]]. Fonua is known for his immense size and strength (Fonua stands a shade under 6 feet 6 inches, or 197 cm, and weights in at 140 kg, well over 300 lb) but also for his speed and agility, surprising attributes given his huge physical size. Big tackles and strong carries, replete with hand-offs to opponents, are his trademarks. He is also known for bouncing off and running over opponents, often leaving them on the ground on their backsides as he continues galloping forward after smashing into them.

==References==
{{Reflist}}
";
            Assert.IsTrue(Parsers.IsArticleAboutAPerson(AR, "Opeti Fonua"),"Infobox about a person");
            
        }

        [Test]
        public void ExternalURLToInternalLink()
        {
            Assert.That(Parsers.ExternalURLToInternalLink(""), Is.Empty);

            Assert.That(Parsers.ExternalURLToInternalLink(
                                "https://secure.wikimedia.org/otrs/index.pl?Action=AgentTicketQueue"),
                            Is.EqualTo("https://secure.wikimedia.org/otrs/index.pl?Action=AgentTicketQueue"));

            Assert.That(Parsers.ExternalURLToInternalLink("[http://ru.wikipedia.org/wiki/Foo Foo]"),
                            Is.EqualTo("[[w:ru:Foo|Foo]]"));

            Assert.That(Parsers.ExternalURLToInternalLink("[http://meta.wikimedia.org/wiki/Test Test]"),
                            Is.EqualTo("[[m:Test|Test]]"));
            Assert.That(Parsers.ExternalURLToInternalLink("[http://commons.wikimedia.org/wiki/Test Test]"),
                            Is.EqualTo("[[commons:Test|Test]]"));
        }

        [Test]
        public void ExternalURLToInternalLinkEn()
        {
            Assert.That(Parsers.ExternalURLToInternalLink("[http://en.wiktionary.org/wiki/Test Test]"),
                            Is.EqualTo("[[wikt:Test|Test]]"));
            Assert.That(Parsers.ExternalURLToInternalLink("[http://en.wiktionary.org/w/Test Test]"),
                            Is.EqualTo("[[wikt:Test|Test]]"));
            Assert.That(Parsers.ExternalURLToInternalLink("[http://en.wikinews.org/wiki/Test Test]"),
                            Is.EqualTo("[[n:Test|Test]]"));
            Assert.That(Parsers.ExternalURLToInternalLink("[http://en.wikibooks.org/wiki/Test Test]"),
                            Is.EqualTo("[[b:Test|Test]]"));
            Assert.That(Parsers.ExternalURLToInternalLink("[http://en.wikiquote.org/wiki/Test Test]"),
                            Is.EqualTo("[[q:Test|Test]]"));
            Assert.That(Parsers.ExternalURLToInternalLink("[http://en.wikisource.org/wiki/Test Test]"),
                            Is.EqualTo("[[s:Test|Test]]"));
            Assert.That(Parsers.ExternalURLToInternalLink("[http://en.wikiversity.org/wiki/Test Test]"),
                            Is.EqualTo("[[v:Test|Test]]"));
            Assert.That(Parsers.ExternalURLToInternalLink("[http://en.wikipedia.org/wiki/Test Test]"),
                            Is.EqualTo("[[w:Test|Test]]"));
            Assert.That(Parsers.ExternalURLToInternalLink("[https://en.wikipedia.org/wiki/Test Test]"),
                            Is.EqualTo("[[w:Test|Test]]"));

            Assert.That(Parsers.ExternalURLToInternalLink("[http://fr.wiktionary.org/wiki/Test Test]"),
                            Is.EqualTo("[[wikt:fr:Test|Test]]"));
            Assert.That(Parsers.ExternalURLToInternalLink("[http://fr.wikipedia.org/wiki/Test Test]"),
                            Is.EqualTo("[[w:fr:Test|Test]]"));
            Assert.That(Parsers.ExternalURLToInternalLink("[https://fr.wikipedia.org/wiki/Test Test]"),
                            Is.EqualTo("[[w:fr:Test|Test]]"));

#if DEBUG
            Variables.SetProjectLangCode("fr");
            Assert.That(Parsers.ExternalURLToInternalLink("[http://en.wikipedia.org/wiki/Test Test]"),
                            Is.EqualTo("[[w:en:Test|Test]]"));
            Assert.That(Parsers.ExternalURLToInternalLink("[https://en.wikipedia.org/wiki/Test Test]"),
                            Is.EqualTo("[[w:en:Test|Test]]"));
            Variables.SetProjectLangCode("en");
            Assert.That(Parsers.ExternalURLToInternalLink("[http://en.wikipedia.org/wiki/Test Test]"),
                            Is.EqualTo("[[w:Test|Test]]"));
#endif
        }

        [Test]
        public void RemoveEmptyComments()
        {
            Assert.That(Parsers.RemoveEmptyComments("<!---->"), Is.Empty);
            Assert.That(Parsers.RemoveEmptyComments("<!-- -->"), Is.Empty);

            // newline comments are used to split wikitext to lines w/o breaking formatting,
            // they should not be removed
            Assert.That(Parsers.RemoveEmptyComments("<!--\r\n\r\n-->"), Is.EqualTo("<!--\r\n\r\n-->"));

            Assert.That(Parsers.RemoveEmptyComments("<!----><!---->"), Is.Empty);
            Assert.That(Parsers.RemoveEmptyComments("<!--\r\n\r\n--><!---->"), Is.EqualTo("<!--\r\n\r\n-->"));
            Assert.That(Parsers.RemoveEmptyComments("<!----><!--Test-->"), Is.EqualTo("<!--Test-->"));
            Assert.That(Parsers.RemoveEmptyComments("<!----> <!--Test-->"), Is.EqualTo(" <!--Test-->"));
            Assert.That(Parsers.RemoveEmptyComments("<!--Test\r\nfoo--> <!--Test-->"), Is.EqualTo("<!--Test\r\nfoo--> <!--Test-->"));

            Assert.That(Parsers.RemoveEmptyComments("<!--Test-->"), Is.EqualTo("<!--Test-->"));

            Assert.That(Parsers.RemoveEmptyComments(""), Is.Empty);
            Assert.That(Parsers.RemoveEmptyComments("test"), Is.EqualTo("test"));
        }

        [Test]
        public void HasSicTagTests()
        {
            Assert.IsTrue(Parsers.HasSicTag("now helo [sic] there"));
            Assert.IsTrue(Parsers.HasSicTag("now helo [sic!] there"));
            Assert.IsTrue(Parsers.HasSicTag("now helo[sic] there"));
            Assert.IsTrue(Parsers.HasSicTag("now helo (sic) there"));
            Assert.IsTrue(Parsers.HasSicTag("now helo (sic!) there"));
            Assert.IsTrue(Parsers.HasSicTag("now helo {sic} there"));
            Assert.IsTrue(Parsers.HasSicTag("now helo [Sic] there"));
            Assert.IsTrue(Parsers.HasSicTag("now helo [ Sic ] there"));
            Assert.IsTrue(Parsers.HasSicTag("now {{sic|helo}} there"));
            Assert.IsTrue(Parsers.HasSicTag("now {{sic|hel|o}} there"));
            Assert.IsTrue(Parsers.HasSicTag("now {{typo|helo}} there"));
            Assert.IsTrue(Parsers.HasSicTag("now helo <!--[sic]-->there"));

            Assert.IsFalse(Parsers.HasSicTag("now sickened by"));
            Assert.IsFalse(Parsers.HasSicTag("sic transit gloria mundi"));
            Assert.IsFalse(Parsers.HasSicTag("The Sound Information Company (SIC) is"));
        }

        [Test]
        public void HasMorefootnotesAndManyReferencesTests()
        {
            Assert.IsTrue(Parsers.HasMorefootnotesAndManyReferences(@"Article<ref>A</ref> <ref>B</ref> <ref>C</ref> <ref>B</ref> <ref>E</ref>
==References==
{{reflist}}
{{nofootnotes}}"));

            Assert.IsTrue(Parsers.HasMorefootnotesAndManyReferences(@"Article<ref>A</ref> <ref>B</ref> <ref>C</ref> <ref>B</ref> <ref>E</ref>
==References==
{{reflist}}
{{morefootnotes}}"));

            Assert.IsTrue(Parsers.HasMorefootnotesAndManyReferences(@"Article<ref name=A>A</ref> <ref name=B>B</ref> <ref>C</ref> <ref>B</ref> <ref>E</ref>
==References==
{{reflist}}
{{Morefootnotes}}"));

            // not enough references
            Assert.IsFalse(Parsers.HasMorefootnotesAndManyReferences(@"Article<ref>A</ref> <ref>B</ref> <ref>C</ref> <ref>B</ref>
==References==
{{reflist}}
{{nofootnotes}}"));

            // no {{nofootnotes}}
            Assert.IsFalse(Parsers.HasMorefootnotesAndManyReferences(@"Article<ref name=A>A</ref> <ref name=B>B</ref> <ref>C</ref> <ref>B</ref> <ref>E</ref>
==References==
{{reflist}}"));
        }

        [Test]
        public void HasRefAfterReflistTest()
        {
            Assert.IsTrue(Parsers.HasRefAfterReflist(@"blah <ref>a</ref> ==references== {{reflist}} <ref>b</ref>"));
            Assert.IsTrue(Parsers.HasRefAfterReflist(@"blah <ref>a</ref>
==references== {{reflist}} <ref>b</ref>"));
            Assert.IsTrue(Parsers.HasRefAfterReflist(@"blah <ref>a</ref> ==references== {{reflist}} <ref name=""b"">b</ref>"));

            // this is correct syntax
            Assert.IsFalse(Parsers.HasRefAfterReflist(@"blah <ref>a</ref> ==references== {{reflist}}"));
            Assert.IsFalse(Parsers.HasRefAfterReflist(@"blah.(Jones 2000)"));
            // ignores commented out refs
            Assert.IsFalse(Parsers.HasRefAfterReflist(@"blah <ref>a</ref> ==references== {{reflist}} <!--<ref>b</ref>-->"));

            // the second template means this is okay too
            Assert.IsFalse(Parsers.HasRefAfterReflist(@"blah <ref>a</ref> ==references== {{reflist}} <ref name=""b"">b</ref> {{reflist}}"));

            // 'r' in argument means no embedded <ref></ref>
            Assert.IsFalse(Parsers.HasRefAfterReflist(@"blah <ref>a</ref> ==references== {{reflist|refs=<ref>abc</ref>}}"));
            string bug1 = @"
==References==
<references />

{{Northampton County, Pennsylvania}}

[[Category:Boroughs in Pennsylvania]]
[[Category:Northampton County, Pennsylvania]]
[[Category:Settlements established in 1790]]

[[ht:Tatamy, Pennsilvani]]
[[nl:Tatamy]]
[[pt:Tatamy]]
[[vo:Tatamy]]";
            Assert.IsFalse(Parsers.HasRefAfterReflist(bug1));

            #if DEBUG
            Variables.SetProjectLangCode("fr");

            Assert.IsFalse(Parsers.HasRefAfterReflist(@"blah <ref>a</ref>
==references== {{reflist}} <ref>b</ref>"));
            Variables.SetProjectLangCode("en");
            #endif

            Assert.IsFalse(Parsers.HasRefAfterReflist(@""));
        }

        [Test]
        public void HasInUseTagTests()
        {
            Assert.IsTrue(Parsers.IsInUse("{{inuse}} Hello world"));
            Assert.IsTrue(Parsers.IsInUse("{{in creation}} Hello world"));
            Assert.IsTrue(Parsers.IsInUse("{{increation}} Hello world"));
            Assert.IsTrue(Parsers.IsInUse("{{ inuse  }} Hello world"));
            Assert.IsTrue(Parsers.IsInUse("{{Inuse}} Hello world"));
            Assert.IsTrue(Parsers.IsInUse("Hello {{inuse}} Hello world"));
            Assert.IsTrue(Parsers.IsInUse("{{inuse|5 minutes}} Hello world"));
            Assert.IsTrue(Parsers.IsInUse("{{In use}} Hello world"));
            Assert.IsTrue(Parsers.IsInUse("{{in use|5 minutes}} Hello world"));


            // ignore commented inuse
            Assert.IsFalse(Parsers.IsInUse("<!--{{inuse}}--> Hello world"));
            Assert.IsFalse(Parsers.IsInUse("<nowiki>{{inuse}}</nowiki> Hello world"));
            Assert.IsFalse(Parsers.IsInUse("<nowiki>{{in use}}</nowiki> Hello world"));
            Assert.IsTrue(Parsers.IsInUse("<!--{{inuse}}--> {{inuse|5 minutes}} Hello world"));
            Assert.IsTrue(Parsers.IsInUse("<!--{{inuse}}--> {{in use|5 minutes}} Hello world"));

            Assert.IsFalse(Parsers.IsInUse("{{INUSE}} Hello world")); // no such template

#if DEBUG
            Variables.SetProjectLangCode("el");
            WikiRegexes.MakeLangSpecificRegexes();
            
            Assert.IsTrue(Parsers.IsInUse("{{Σε χρήση}} Hello world"), "σε χρήση");
            Assert.IsTrue(Parsers.IsInUse("{{inuse}} Hello world"), "inuse");
            Assert.IsFalse(Parsers.IsInUse("{{goceinuse}} Hello world"), "goceinuse is en-only");

#endif
        }

        [Test]
        public void IsMissingReferencesDisplayTests()
        {
            Assert.IsTrue(Parsers.IsMissingReferencesDisplay(@"Hello<ref>Fred</ref>"));
            Assert.IsTrue(Parsers.IsMissingReferencesDisplay(@"Hello<ref name=""F"">Fred</ref>"));

            // {{GR}} provides an embedded <ref></ref> if its argument is a decimal
            Assert.IsTrue(Parsers.IsMissingReferencesDisplay(@"Hello{{GR|4}}"));

            Assert.IsFalse(Parsers.IsMissingReferencesDisplay(@"Hello"));
            Assert.IsFalse(Parsers.IsMissingReferencesDisplay(@"Hello<ref>Fred</ref> {{reflist}}"));
            Assert.IsFalse(Parsers.IsMissingReferencesDisplay(@"Hello<ref>Fred</ref> {{Reflist}}"));
            Assert.IsFalse(Parsers.IsMissingReferencesDisplay(@"Hello<ref>Fred</ref> {{Reflist
|refs = 
{{cite news | title = A { hello }}
}}"), "Unbalanced brackets within cite template in reflist does not affect logic");
            Assert.IsFalse(Parsers.IsMissingReferencesDisplay(@"Hello<ref>Fred</ref> {{ref-list}}"));
            Assert.IsFalse(Parsers.IsMissingReferencesDisplay(@"Hello<ref>Fred</ref> {{reflink}}"));
            Assert.IsFalse(Parsers.IsMissingReferencesDisplay(@"Hello<ref>Fred</ref> {{references}}"));
            Assert.IsFalse(Parsers.IsMissingReferencesDisplay(@"Hello<ref>Fred</ref> <references/>"));
            Assert.IsFalse(Parsers.IsMissingReferencesDisplay(@"Hello{{GR|4}} <references/>"));

            // this specifies to {{GR}} not to embed <ref></ref>
            Assert.IsFalse(Parsers.IsMissingReferencesDisplay(@"Hello{{GR|r4}}"));
            Assert.IsFalse(Parsers.IsMissingReferencesDisplay(@"Hello{{GR|India}}"));

            Assert.IsFalse(Parsers.IsMissingReferencesDisplay(@"{{Reflist|refs=
<ref name=modern>{{cite news |first=William }}
        }}"));

            Assert.IsFalse(Parsers.IsMissingReferencesDisplay(@"Hello<ref group=X>Fred</ref>"));
        }

        [Test]
        public void IsMissingReferencesDisplayTestsEnOnly()
        {
#if DEBUG
            Variables.SetProjectLangCode("fr");

            Assert.IsFalse(Parsers.IsMissingReferencesDisplay(@"Hello<ref>Fred</ref>"));
            Assert.IsFalse(Parsers.IsMissingReferencesDisplay(@"Hello<ref name=""F"">Fred</ref>"));

            Variables.SetProjectLangCode("en");
#endif
        }

        [Test]
        public void InfoboxTests()
        {
            Assert.IsTrue(Parsers.HasInfobox(@"{{Infobox fish | name = Bert }} ''Bert'' is a good fish."));
            Assert.IsTrue(Parsers.HasInfobox(@"{{Infobox
fish | name = Bert }} ''Bert'' is a good fish."));
            Assert.IsTrue(Parsers.HasInfobox(@"{{infobox fish | name = Bert }} ''Bert'' is a good fish."));
            Assert.IsTrue(Parsers.HasInfobox(@"{{ infobox fish | name = Bert }} ''Bert'' is a good fish."));
            Assert.IsTrue(Parsers.HasInfobox(@"{{ infobox fish | name = Bert }} ''Bert'' is a good <!--fish-->."));

            Assert.IsFalse(Parsers.HasInfobox(@"{{INFOBOX fish | name = Bert }} ''Bert'' is a good fish."));
            Assert.IsFalse(Parsers.HasInfobox(@"{{infoboxfish | name = Bert }} ''Bert'' is a good fish."));
            Assert.IsFalse(Parsers.HasInfobox(@"<!--{{infobox fish | name = Bert }}--> ''Bert'' is a good fish."));
            Assert.IsFalse(Parsers.HasInfobox(@"<nowiki>{{infobox fish | name = Bert }}</nowiki> ''Bert'' is a good fish."));
        }

        [Test]
        public void InfoboxTestsEnOnly()
        {
#if DEBUG
            Variables.SetProjectLangCode("fr");
            Assert.IsFalse(Parsers.HasInfobox(@"{{Infobox fish | name = Bert }} ''Bert'' is a good fish."));
            Assert.IsFalse(Parsers.HasInfobox(@"{{INFOBOX fish | name = Bert }} ''Bert'' is a good fish."));
            Variables.SetProjectLangCode("en");
            Assert.IsTrue(Parsers.HasInfobox(@"{{Infobox fish | name = Bert }} ''Bert'' is a good fish."));
#endif
        }

        [Test]
        public void HasStubTemplate()
        {
            Assert.IsTrue(Parsers.HasStubTemplate(@"foo {{foo stub}}"));
            Assert.IsTrue(Parsers.HasStubTemplate(@"foo {{foo-stub}}"));

            Assert.IsFalse(Parsers.HasStubTemplate(@"foo {{foo tubs}}"));
        }

        [Test]
        public void HasStubTemplateAr()
        {
#if DEBUG
            Variables.SetProjectLangCode("ar");
            WikiRegexes.MakeLangSpecificRegexes();
            
            Assert.IsTrue(Parsers.HasStubTemplate(@"foo {{بذرة ممثل}}"), "actor stub");
            Assert.IsTrue(Parsers.HasStubTemplate(@"foo {{بذرة ألمانيا}}"), "germany stub");
            
            Variables.SetProjectLangCode("en");
            WikiRegexes.MakeLangSpecificRegexes();
#endif
        }

        [Test]
        public void IsStub()
        {
            Assert.IsTrue(Parsers.IsStub(@"foo {{foo stub}}"));
            Assert.IsTrue(Parsers.IsStub(@"foo {{foo-stub}}"));

            // short article
            Assert.IsTrue(Parsers.IsStub(@"foo {{foo tubs}}"));

            const string a = @"fooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo";
            const string b = a + a + a + a + a + a;
            Assert.IsFalse(Parsers.IsStub(b + b + b + b));
        }

        [Test]
        public void IsStubAr()
        {
#if DEBUG
            Variables.SetProjectLangCode("ar");
            Assert.IsTrue(Parsers.IsStub(@"foo {{بذرة ممثل}}"));

            // short article
            Assert.IsTrue(Parsers.IsStub(@"foo {{foo tubs}}"));

            const string a = @"fooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo";
            const string b = a + a + a + a + a + a;
            Assert.IsFalse(Parsers.IsStub(b + b + b + b));
            Variables.SetProjectLangCode("en");
#endif
        }

        [Test]
        public void IsStubArz()
        {
#if DEBUG
            Variables.SetProjectLangCode("arz");
            Assert.IsTrue(Parsers.IsStub(@"foo {{بذرة}}"));

            // short article
            Assert.IsTrue(Parsers.IsStub(@"foo {{foo tubs}}"));

            const string a = @"fooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo";
            const string b = a + a + a + a + a + a;
            Assert.IsFalse(Parsers.IsStub(b + b + b + b));
            Variables.SetProjectLangCode("en");
#endif
        }

        [Test]
        public void NoBotsTests()
        {
            Assert.IsTrue(Parsers.CheckNoBots("", ""));
            Assert.IsTrue(Parsers.CheckNoBots("{{test}}", ""));
            Assert.IsTrue(Parsers.CheckNoBots("lol, test", ""));

            Assert.IsTrue(Parsers.CheckNoBots("{{bots}}", ""));
            Assert.IsTrue(Parsers.CheckNoBots("{{bots|allow=awb}}", ""));
            Assert.IsTrue(Parsers.CheckNoBots("{{bots|allow=test}}", "test"));
            Assert.IsTrue(Parsers.CheckNoBots("{{bots|allow=user,test}}", "test"));
            Assert.IsTrue(Parsers.CheckNoBots("{{bots|deny=none}}", ""));

            Assert.IsTrue(Parsers.CheckNoBots("{{bots|deny=Xenobot Mk V}}", "Xenobot"));
            Assert.IsTrue(Parsers.CheckNoBots("{{bots|deny=Xenobot}}", "Xenobot Mk V"));

            Assert.IsFalse(Parsers.CheckNoBots("{{nobots}}", ""));
            Assert.IsFalse(Parsers.CheckNoBots("{{bots|deny=all}}", ""));
            Assert.IsFalse(Parsers.CheckNoBots("<!--comm-->{{bots|deny=all}}", ""));
            Assert.IsFalse(Parsers.CheckNoBots(@"<!--comm
-->{{bots|deny=all}}", ""));
            Assert.IsFalse(Parsers.CheckNoBots("{{bots|deny=awb}}", ""));
            Assert.IsFalse(Parsers.CheckNoBots("{{bots|deny=awb,test}}", ""));
            Assert.IsFalse(Parsers.CheckNoBots("{{bots|deny=awb,test}}", "test"));
            Assert.IsFalse(Parsers.CheckNoBots("{{nobots|deny=awb,test}}", "test"));
            Assert.IsFalse(Parsers.CheckNoBots("{{bots|deny=test}}", "test"));
            Assert.IsFalse(Parsers.CheckNoBots("{{bots|allow=none}}", ""));
            Assert.IsFalse(Parsers.CheckNoBots(@"{{bots|deny=AWB, MenoBot, MenoBot II}}", "AWB and other bots"));
            Assert.IsTrue(Parsers.CheckNoBots("<!-- comm {{bots|deny=all}} -->", ""));

            Assert.IsFalse(Parsers.CheckNoBots(@"{{bots|allow=MiszaBot III,SineBot}}", "otherBot"));
            Assert.IsTrue(Parsers.CheckNoBots(@"{{bots|allow=MiszaBot III,SineBot}}", "SineBot"));
            Assert.IsTrue(Parsers.CheckNoBots(@"{{bots|allow=MiszaBot III,SineBot, AWB}}", "SomeotherBot, AWB last"));
            Assert.IsTrue(Parsers.CheckNoBots(@"{{bots|allow=AWB, MiszaBot III,SineBot}}", "SomeotherBot, AWB first"));

            /* prospective future changes to bots template
            Assert.IsTrue(Parsers.CheckNoBots(@"{{bots|deny=all|allow=MiszaBot III,SineBot}}", "SineBot"));
            Assert.IsTrue(Parsers.CheckNoBots(@"{{bots|deny=all|allow=MiszaBot III,SineBot}}", "MiszaBot III"));
            Assert.IsFalse(Parsers.CheckNoBots(@"{{bots|deny=all|allow=MiszaBot III,SineBot}}", "OtherBot")); */
        }

        [Test]
        public void NoIncludeIncludeOnlyProgrammingElement()
        {
            Assert.IsTrue(Parsers.NoIncludeIncludeOnlyProgrammingElement(@"<noinclude>blah</noinclude>"));
            Assert.IsTrue(Parsers.NoIncludeIncludeOnlyProgrammingElement(@"<includeonly>blah</includeonly>"));
            Assert.IsTrue(Parsers.NoIncludeIncludeOnlyProgrammingElement(@"<onlyinclude>blah</onlyinclude>"));
            Assert.IsTrue(Parsers.NoIncludeIncludeOnlyProgrammingElement(@"{{{1}}}"));
            Assert.IsTrue(Parsers.NoIncludeIncludeOnlyProgrammingElement(@"{{{3}}}"));

            Assert.IsFalse(Parsers.NoIncludeIncludeOnlyProgrammingElement(@"hello"));
            Assert.IsFalse(Parsers.NoIncludeIncludeOnlyProgrammingElement(@""));
        }

        [Test]
        public void GetTemplateTests()
        {
            Assert.That(Parsers.GetTemplate(@"now {{foo}} was here", "foo"), Is.EqualTo(@"{{foo}}"));
            Assert.That(Parsers.GetTemplate(@"now {{Foo}} was here", "foo"), Is.EqualTo(@"{{Foo}}"));
            Assert.That(Parsers.GetTemplate(@"now {{foo}} was here", "Foo"), Is.EqualTo(@"{{foo}}"));
            Assert.That(Parsers.GetTemplate(@"now {{foo}} was here", "[Ff]oo"), Is.EqualTo(@"{{foo}}"));
            Assert.That(Parsers.GetTemplate(@"now {{ foo|bar asdfasdf}} was here", "foo"), Is.EqualTo(@"{{ foo|bar asdfasdf}}"));
            Assert.That(Parsers.GetTemplate(@"now {{ foo |bar asdfasdf}} was here", "foo"), Is.EqualTo(@"{{ foo |bar asdfasdf}}"));
            Assert.That(Parsers.GetTemplate(@"now {{ foo|bar
asdfasdf}} was here", "foo"), Is.EqualTo(@"{{ foo|bar
asdfasdf}}"));
            Assert.That(Parsers.GetTemplate(@"now {{foo}} was here {{foo|1}}", "foo"), Is.EqualTo(@"{{foo}}"));

            Assert.That(Parsers.GetTemplate(@"now {{ foo|bar asdfasdf}} was here", "foot"), Is.Empty);
            Assert.That(Parsers.GetTemplate(@"now {{ foo|bar asdfasdf}} was here", ""), Is.Empty);
            Assert.That(Parsers.GetTemplate(@"", "foo"), Is.Empty);
            Assert.That(Parsers.GetTemplate(@"now <!--{{foo}} --> was here", "foo"), Is.Empty);
            Assert.That(Parsers.GetTemplate(@"now {{foo<!--comm-->}} was here", "foo"), Is.EqualTo(@"{{foo<!--comm-->}}"));

            Assert.That(Parsers.GetTemplate(@"now {{foo  |a={{bar}} here}} was here", "foo"), Is.EqualTo(@"{{foo  |a={{bar}} here}}"));
            Assert.That(Parsers.GetTemplate(@"now <!--{{foo|bar}}--> {{foo}} was here", "Foo"), Is.EqualTo(@"{{foo|bar}}"));
        }

        [Test]
        public void GetTemplatesTests()
        {
            const string foo1 = "{{foo|a}}", foo2 = "{{foo|b}}";
            string text = @"now " + foo1 + " and " + foo2;

            Regex foo = new Regex(@"{{foo.*?}}");
            List<Match> fred = new List<Match>();

            foreach (Match m in foo.Matches(text))
                fred.Add(m);

            Assert.That(Parsers.GetTemplates(text, "foo").ToString(), Is.EqualTo(fred.ToString()));
            Assert.That(Parsers.GetTemplates(text, "Foo").ToString(), Is.EqualTo(fred.ToString()));
            List<Match> templates = Parsers.GetTemplates(text, "foo");
            Assert.That(templates[0].Value, Is.EqualTo(foo1));
            Assert.That(templates[1].Value, Is.EqualTo(foo2));
            Assert.That(templates.Count, Is.EqualTo(2));

            // ignores commeted out templates
            templates = Parsers.GetTemplates(text + @" <!-- {{foo|c}} -->", "foo");
            Assert.That(templates[0].Value, Is.EqualTo(foo1));
            Assert.That(templates[1].Value, Is.EqualTo(foo2));
            Assert.That(templates.Count, Is.EqualTo(2));

            // ignores nowiki templates
            templates = Parsers.GetTemplates(text + @" <nowiki> {{foo|c}} </nowiki>", "foo");
            Assert.That(templates[0].Value, Is.EqualTo(foo1));
            Assert.That(templates[1].Value, Is.EqualTo(foo2));
            Assert.That(templates.Count, Is.EqualTo(2));

            // nested templates caught
            const string foo3 = @"{{ Foo|bar={{abc}}|beer=y}}";
            templates = Parsers.GetTemplates(@"now " + foo3 + @" there", "foo");
            Assert.That(templates[0].Value, Is.EqualTo(foo3));

            // whitespace ignored
            const string foo4 = @"{{ Foo }}";
            templates = Parsers.GetTemplates(@"now " + foo4 + @" there", "foo");
            Assert.That(templates[0].Value, Is.EqualTo(foo4));

            // no matches here
            templates = Parsers.GetTemplates(@"now " + foo3 + @" there", "fo");
            Assert.That(templates.Count, Is.EqualTo(0));

            templates = Parsers.GetTemplates(@"{{test}}", "test");
            Assert.That(templates.Count, Is.EqualTo(1));

            templates = Parsers.GetTemplates(@"{{test}}
", "test");
            Assert.That(templates.Count, Is.EqualTo(1));
        }

        [Test]
        public void GetTemplatesEmbeddedComments()
        {
            const string foo1 = "{{foo|a}}", foo2 = "{{foo|b}}",
            foo2a = @"{{foo<!--comm-->|b}}", foo2b = @"{{foo|b<!--comm-->}}";
            string text = @"now " + foo1 + " and " + foo2;
            List<Match> templates = new List<Match>();

            // templates with embedded comments caught
            templates = Parsers.GetTemplates(text + " space " + foo2a, "foo");
            Assert.That(templates.Count, Is.EqualTo(3));
            Assert.That(templates[0].Value, Is.EqualTo(foo1));
            Assert.That(templates[1].Value, Is.EqualTo(foo2));
            Assert.That(templates[2].Value, Is.EqualTo(foo2a));

            templates = Parsers.GetTemplates(text + " space " + foo2b, "foo");
            Assert.That(templates.Count, Is.EqualTo(3));
            Assert.That(templates[0].Value, Is.EqualTo(foo1));
            Assert.That(templates[1].Value, Is.EqualTo(foo2));
            Assert.That(templates[2].Value, Is.EqualTo(foo2b));
        }

        [Test]
        public void GetTemplatesTestsAllTemplates()
        {
            const string foo1 = "{{foo|a}}", foo2 = "{{foo|b}}",
            foo2a = @"{{foo<!--comm-->|b}}", foo2b = @"{{foo|b<!--comm-->}}";
            string text = @"now " + foo1 + " and " + foo2;
            List<Match> templates = new List<Match>();

            // templates with embedded comments caught
            templates = Parsers.GetTemplates(text + " space " + foo2a);
            Assert.That(templates.Count, Is.EqualTo(3));
            Assert.That(templates[0].Value, Is.EqualTo(foo1));
            Assert.That(templates[1].Value, Is.EqualTo(foo2));
            Assert.That(templates[2].Value, Is.EqualTo(foo2a));

            templates = Parsers.GetTemplates(text + " space " + foo2b);
            Assert.That(templates.Count, Is.EqualTo(3));
            Assert.That(templates[0].Value, Is.EqualTo(foo1));
            Assert.That(templates[1].Value, Is.EqualTo(foo2));
            Assert.That(templates[2].Value, Is.EqualTo(foo2b));

            templates = Parsers.GetTemplates(@" {{one}} {{two}} {{three|a={{bcd}} |ef=gh}}");
            Assert.That(templates.Count, Is.EqualTo(3));
        }

        [Test]
        public void FixUnicode()
        {
            // https://en.wikipedia.org/wiki/Wikipedia:AWB/B#Line_break_insertion

            Assert.That(parser.FixUnicode("foo\x2028bar"), Is.EqualTo("foo bar"));
            Assert.That(parser.FixUnicode("foo" + "\r\n" + "\x2028bar"), Is.EqualTo(@"foo
bar"));
            Assert.That(parser.FixUnicode("foo\x2029bar"), Is.EqualTo("foo bar"));
            Assert.That(parser.FixUnicode("foo" + "\r\n" + "\x200B\x200Bbar"), Is.EqualTo(@"foo
bar"));
        }

        [Test]
        public void SubstUserTemplates()
        {
            Regex Hello = new Regex(@"{{hello.*?}}");

            Assert.That(Parsers.SubstUserTemplates(@"Text {{hello|2010}}", "test", Hello), Is.EqualTo(@"Text Expanded template test return<!-- {{hello|2010}} -->"), "performs single substitution");
            Assert.That(Parsers.SubstUserTemplates(@"Text {{hello}}", "test", Hello), Is.EqualTo(@"Text Expanded template test return<!-- {{hello}} -->"), "performs single substitution");
            Assert.That(Parsers.SubstUserTemplates(@"Text {{hello}}
{{hello2}}", "test", Hello), Is.EqualTo(@"Text Expanded template test return<!-- {{hello}} -->
Expanded template test return<!-- {{hello2}} -->"), "performs multiple subsitutions");

            const string Bye = @"Text {{bye}}";
            Assert.That(Parsers.SubstUserTemplates(Bye, "test", Hello), Is.EqualTo(Bye), "no changes if no matching template");

            const string Subst = @"Now {{{subst:bar}}} text";
            Assert.That(Parsers.SubstUserTemplates(Subst, "test", Hello), Is.EqualTo(Subst), "doesn't change {{{subst");

            Regex None = null;
            Assert.That(Parsers.SubstUserTemplates(Bye, "test", None), Is.EqualTo(Bye), "no changes when user talk page regex is null");

            const string T2 = @"Test {{{2|}}}";
            Assert.That(Parsers.SubstUserTemplates(T2, "test", Hello), Is.EqualTo("Test"), "cleans up the {{{2|}}} template");
        }

        [Test]
        public void FormatToBDA()
        {
            const string Correct = @"{{birth date and age|df=y|1990|05|11}}", CorrectAmerican = @"{{birth date and age|mf=y|1990|05|11}}";

            Assert.That(Parsers.FormatToBDA(@"11 May 1990 (age 21)"), Is.EqualTo(Correct));
            Assert.That(Parsers.FormatToBDA(@"11 May 1990 (Age 21)"), Is.EqualTo(Correct));
            Assert.That(Parsers.FormatToBDA(@"11 May 1990, (Age 21)"), Is.EqualTo(Correct));
            Assert.That(Parsers.FormatToBDA(@"11 May 1990; (Age 21)"), Is.EqualTo(Correct));
            Assert.That(Parsers.FormatToBDA(@"11 May 1990 (aged 21)"), Is.EqualTo(Correct));
            Assert.That(Parsers.FormatToBDA(@"11 May [[1990]] (aged 21)"), Is.EqualTo(Correct));
            Assert.That(Parsers.FormatToBDA(@"[[11 May]] [[1990]] (aged 21)"), Is.EqualTo(Correct));
            Assert.That(Parsers.FormatToBDA(@"[[11 May]] 1990 (aged 21)"), Is.EqualTo(Correct));
            Assert.That(Parsers.FormatToBDA(@"11th May 1990 (aged 21)"), Is.EqualTo(Correct));
            Assert.That(Parsers.FormatToBDA(@"11 May 1990 ( aged 21 )"), Is.EqualTo(Correct));
            Assert.That(Parsers.FormatToBDA(@"11 May 1990    ( aged 21 )"), Is.EqualTo(Correct));
            Assert.That(Parsers.FormatToBDA(@"11 May, 1990    ( aged 21 )"), Is.EqualTo(Correct));

            Assert.That(Parsers.FormatToBDA(@"1990-05-11 (age 21)"), Is.EqualTo(Correct));
            Assert.That(Parsers.FormatToBDA(@"1990-5-11 (age 21)"), Is.EqualTo(Correct));
            Assert.That(Parsers.FormatToBDA(@"1990-5-11 <br/>(age 21)"), Is.EqualTo(Correct));
            Assert.That(Parsers.FormatToBDA(@"1990-5-11 <br>(age 21)"), Is.EqualTo(Correct));
            Assert.That(Parsers.FormatToBDA(@"1990-5-11 <br>Age 21"), Is.EqualTo(Correct));

            Assert.That(Parsers.FormatToBDA(@"May 11, 1990 (age 21)"), Is.EqualTo(CorrectAmerican));
            Assert.That(Parsers.FormatToBDA(@"May 11 1990 (age 21)"), Is.EqualTo(CorrectAmerican));
            Assert.That(Parsers.FormatToBDA(@"May 11th 1990 (age 21)"), Is.EqualTo(CorrectAmerican));

            Assert.That(Parsers.FormatToBDA(""), Is.Empty);
            Assert.That(Parsers.FormatToBDA("Test"), Is.EqualTo("Test"));
            Assert.That(Parsers.FormatToBDA("May 11, 1990"), Is.EqualTo("May 11, 1990"));
            Assert.That(Parsers.FormatToBDA("May 1990 (age 21)"), Is.EqualTo("May 1990 (age 21)"));
            Assert.That(Parsers.FormatToBDA("May 11, 1990 (Age 21) and some other text"), Is.EqualTo("May 11, 1990 (Age 21) and some other text"));
        }

        [Test]
        public void COinS()
        {
            string data1 = @"""coins"": ""ctx_ver=Z39.88-2004&amp;rft_id=info%3Adoi%2Fhttp%3A%2F%2Fdx.doi.org%2F10.1007%2Fs11046-005-4332-4&amp;rfr_id=info%3Asid%2Fcrossref.org%3Asearch&amp;rft.atitle=Morphological+alterations+in+toxigenic+Aspergillus+parasiticus+exposed+to+neem+%28Azadirachta+indica%29+leaf+and+seed+aqueous+extracts&amp;rft.jtitle=Nature+Structural+%26%2338%3B+Molecular+Biology&amp;rft.date=2005&amp;rft.volume=159&amp;rft.issue=4&amp;rft.spage=565&amp;rft.epage=570&amp;rft.aufirst=Mehdi&amp;rft.aulast=Razzaghi-Abyaneh&amp;rft_val_fmt=info%3Aofi%2Ffmt%3Akev%3Amtx%3Ajournal&amp;rft.genre=article&amp;rft.au=Mehdi+Razzaghi-Abyaneh&amp;rft.au=+Abdolamir+Allameh&amp;rft.au=+Taki+Tiraihi&amp;rft.au=+Masoomeh+Shams-Ghahfarokhi&amp;rft.au=+Mehdi+Ghorbanian""";
            Dictionary<string, string> res = Parsers.ExtractCOinS(data1);
            Assert.IsTrue(res.ContainsKey("volume"));
            Assert.IsTrue(res.ContainsKey("issue"));
            Assert.IsTrue(res.ContainsKey("spage"));
            Assert.IsTrue(res.ContainsKey("aulast"));
            Assert.IsTrue(res.ContainsKey("atitle"));
            Assert.IsTrue(res.ContainsKey("date"));
            Assert.IsTrue(res.ContainsKey("jtitle"));
            string v;
            res.TryGetValue("volume", out v);
            Assert.That(v, Is.EqualTo("159"));
            res.TryGetValue("jtitle", out v);
            Assert.That(v, Is.EqualTo("Nature Structural &#38; Molecular Biology"));
        }
    }
}
