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
using NUnit.Framework.Legacy;

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
            ClassicAssert.IsTrue(noChange, "don't change sorting for single categories");

            // should work
            Assert.That(Parsers.ChangeToDefaultSort("[[Category:Test1|Foooo]][[Category:Test2|Foooo]]", "Bar",
                                                        out noChange),
                            Is.EqualTo("[[Category:Test1]][[Category:Test2]]\r\n{{DEFAULTSORT:Foooo}}"));
            ClassicAssert.IsFalse(noChange);

            // ...but don't add DEFAULTSORT if the key equals page title
            Assert.That(Parsers.ChangeToDefaultSort("[[Category:Test1|Foooo]][[Category:Test2|Foooo]]", "Foooo",
                                                        out noChange),
                            Is.EqualTo("[[Category:Test1]][[Category:Test2]]"));
            ClassicAssert.IsFalse(noChange, "Should detect a change even if it hasn't added a DEFAULTSORT");

            // don't change if key is 3 chars or less
            Assert.That(Parsers.ChangeToDefaultSort("[[Category:Test1|Foo]][[Category:Test2|Foo]]", "Bar", out noChange),
                            Is.EqualTo("[[Category:Test1|Foo]][[Category:Test2|Foo]]"));
            ClassicAssert.IsTrue(noChange);

            // Remove explicit keys equal to page title
            Assert.That(Parsers.ChangeToDefaultSort("[[Category:Test1|Foooo]][[Category:Test2]]", "Foooo", out noChange),
                            Is.EqualTo("[[Category:Test1]][[Category:Test2]]"));
            ClassicAssert.IsFalse(noChange);

            // swap
            Assert.That(Parsers.ChangeToDefaultSort("[[Category:Test1]][[Category:Test2|Foooo]]", "Foooo", out noChange),
                            Is.EqualTo("[[Category:Test1]][[Category:Test2]]"));
            ClassicAssert.IsFalse(noChange);

            // Borderline condition
            Assert.That(Parsers.ChangeToDefaultSort("[[Category:Test1|Fooooo]][[Category:Test2]]", "Foooo", out noChange),
                            Is.EqualTo("[[Category:Test1|Fooooo]][[Category:Test2]]"));
            ClassicAssert.IsTrue(noChange);

            // Don't change anything if there's ambiguity
            Assert.That(Parsers.ChangeToDefaultSort("[[Category:Test1|Foooo]][[Category:Test2|Baaar]]", "Teeest",
                                                        out noChange),
                            Is.EqualTo("[[Category:Test1|Foooo]][[Category:Test2|Baaar]]"));
            ClassicAssert.IsTrue(noChange);
            // same thing
            Assert.That(Parsers.ChangeToDefaultSort("[[Category:Test1|Foooo]][[Category:Test2|Baaar]]", "Foooo",
                                                        out noChange),
                            Is.EqualTo("[[Category:Test1|Foooo]][[Category:Test2|Baaar]]"));
            ClassicAssert.IsTrue(noChange);

            // remove diacritics when generating a key
            Assert.That(Parsers.ChangeToDefaultSort("[[Category:Test1|Foooô]][[Category:Test2|Foooô]]", "Bar",
                                                        out noChange),
                            Is.EqualTo("[[Category:Test1]][[Category:Test2]]\r\n{{DEFAULTSORT:Foooo}}"));
            ClassicAssert.IsFalse(noChange);

            // should also fix diacritics in existing defaultsorts and remove leading spaces
            // also support mimicking templates: template to magic word conversion, see [[Category:Pages which use a template in place of a magic word]]
            Assert.That(Parsers.ChangeToDefaultSort("{{defaultsort| Tést}}", "Foo", out noChange), Is.EqualTo("{{DEFAULTSORT:Test}}"));
            ClassicAssert.IsFalse(noChange);
            Assert.That(Parsers.ChangeToDefaultSort("{{DEFAULTSORT| Tést}}", "Foo", out noChange), Is.EqualTo("{{DEFAULTSORT:Test}}"));
            ClassicAssert.IsFalse(noChange);
            Assert.That(Parsers.ChangeToDefaultSort("{{DEFAULTSORT:|Test}}", "Foo", out noChange), Is.EqualTo("{{DEFAULTSORT:Test}}"));
            ClassicAssert.IsFalse(noChange);

            // shouldn't change whitespace-only sortkeys
            Assert.That(Parsers.ChangeToDefaultSort("{{DEFAULTSORT: \t}}", "Foo", out noChange), Is.EqualTo("{{DEFAULTSORT: \t}}"));
            ClassicAssert.IsTrue(noChange);

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_6#DEFAULTSORT_with_spaces
            // DEFAULTSORT doesn't treat leading spaces the same way as categories do
            Assert.That(Parsers.ChangeToDefaultSort("[[Category:Test1| Foooo]][[Category:Test2| Foooo]]", "Bar",
                                                        out noChange),
                            Is.EqualTo("[[Category:Test1| Foooo]][[Category:Test2| Foooo]]"));
            ClassicAssert.IsTrue(noChange);

            // pages with multiple sort specifiers shouldn't be changed
            Parsers.ChangeToDefaultSort("{{defaultsort| Tést}}{{DEFAULTSORT: Tést}}", "Foo", out noChange);
            ClassicAssert.IsTrue(noChange);

            // Remove explicitally defined sort keys from categories when the page has defaultsort
            Assert.That(Parsers.ChangeToDefaultSort("{{DEFAULTSORT:Test}}[[Category:Test|Test]]", "Foo", out noChange),
                            Is.EqualTo("{{DEFAULTSORT:Test}}[[Category:Test]]"));
            ClassicAssert.IsFalse(noChange);

            // Case difference of above
            Assert.That(Parsers.ChangeToDefaultSort("{{DEFAULTSORT:Test}}[[Category:Test|TEST]]", "Foo", out noChange),
                            Is.EqualTo("{{DEFAULTSORT:Test}}[[Category:Test]]"));
            ClassicAssert.IsFalse(noChange);

            // No change due to different key
            Assert.That(Parsers.ChangeToDefaultSort("{{DEFAULTSORT:Test}}[[Category:Test|Not a Test]]", "Foo", out noChange),
                            Is.EqualTo("{{DEFAULTSORT:Test}}[[Category:Test|Not a Test]]"));
            ClassicAssert.IsTrue(noChange);

            // Multiple to be removed
            Assert.That(Parsers.ChangeToDefaultSort("{{DEFAULTSORT:Test}}[[Category:Test|TEST]][[Category:Foo|Test]][[Category:Bar|test]]", "Foo", out noChange),
                            Is.EqualTo("{{DEFAULTSORT:Test}}[[Category:Test]][[Category:Foo]][[Category:Bar]]"));
            ClassicAssert.IsFalse(noChange);

            // Multiple with 1 no key
            Assert.That(Parsers.ChangeToDefaultSort("{{DEFAULTSORT:Test}}[[Category:Test|TEST]][[Category:Foo]][[Category:Bar|test]]", "Foo", out noChange),
                            Is.EqualTo("{{DEFAULTSORT:Test}}[[Category:Test]][[Category:Foo]][[Category:Bar]]"));
            ClassicAssert.IsFalse(noChange);

            // Multiple with 1 different key
            Assert.That(Parsers.ChangeToDefaultSort("{{DEFAULTSORT:Test}}[[Category:Test|TEST]][[Category:Foo|Bar]][[Category:Bar|test]]", "Foo", out noChange),
                            Is.EqualTo("{{DEFAULTSORT:Test}}[[Category:Test]][[Category:Foo|Bar]][[Category:Bar]]"));
            ClassicAssert.IsFalse(noChange);

            // just removing diacritics in categories is useful
            Assert.That(Parsers.ChangeToDefaultSort(@"[[Category:Bronze Wolf awardees|Lainé, Juan]]", "Hi", out noChange),
                            Is.EqualTo(@"[[Category:Bronze Wolf awardees|Laine, Juan]]"));
            ClassicAssert.IsFalse(noChange);
            Assert.That(Parsers.ChangeToDefaultSort(@"[[Category:Bronze Wolf awardees|Lainİ, Juan]]", "Hi", out noChange),
                            Is.EqualTo(@"[[Category:Bronze Wolf awardees|LainI, Juan]]"), "unusual one where lowercase of diacritic is a Latin character");
            ClassicAssert.IsFalse(noChange);

            Assert.That(Parsers.ChangeToDefaultSort(@"[[Category:Bronze Wolf awardees|Laine, Juan]]", "Hi", out noChange),
                            Is.EqualTo(@"[[Category:Bronze Wolf awardees|Laine, Juan]]"));
            ClassicAssert.IsTrue(noChange);

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
            ClassicAssert.IsTrue(noChange);

            Variables.UnicodeCategoryCollation = true;
            Assert.That(Parsers.ChangeToDefaultSort("[[Category:Test1|Foooô]][[Category:Test2|Foooô]]", "Bar",
                                                        out noChange),
                            Is.EqualTo("[[Category:Test1]][[Category:Test2]]\r\n{{DEFAULTSORT:Foooô}}"));
            ClassicAssert.IsFalse(noChange, "retain diacritics when generating a key if uca collation is on");

            Assert.That(Parsers.ChangeToDefaultSort("[[Category:Test1]][[Category:Test2]]", "Foooô",
                                                        out noChange),
                            Is.EqualTo(@"[[Category:Test1]][[Category:Test2]]"));
            ClassicAssert.IsTrue(noChange, "No change when defaultsort not needed when uca collation on");
            Variables.UnicodeCategoryCollation = false;
        }

        [Test]
        public void MissingDefaultSort()
        {
            ClassicAssert.IsFalse(Parsers.MissingDefaultSort(@"A", @"A"));
            ClassicAssert.IsFalse(Parsers.MissingDefaultSort(@"A {{DEFAULTSORT:A}}", @"A {{DEFAULTSORT:A}}"));
            ClassicAssert.IsFalse(Parsers.MissingDefaultSort(@"A {{DEFAULTSORT:A}} [[category:A]]", @"A"));
            ClassicAssert.IsTrue(Parsers.MissingDefaultSort(@"A
[[Category:1910 births]]", @"John Smith"));
        }

        [Test]
        public void ChangeToDefaultsortCaseInsensitive()
        {
            bool noChange;
            const string CInsensitive = @"x [[Category:Foo]]";

            Assert.That(Parsers.ChangeToDefaultSort(CInsensitive, "BAR", out noChange), Is.EqualTo(CInsensitive), "no change when defaultsort only differs to article title by case");
            ClassicAssert.IsTrue(noChange);

            Assert.That(Parsers.ChangeToDefaultSort(CInsensitive, "Bar", out noChange), Is.EqualTo(CInsensitive), "no change when defaultsort only differs to article title by case");
            ClassicAssert.IsTrue(noChange);

            Assert.That(Parsers.ChangeToDefaultSort(CInsensitive, "Bar foo", out noChange), Is.EqualTo(CInsensitive), "no change when defaultsort only differs to article title by case");
            ClassicAssert.IsTrue(noChange);

            Assert.That(Parsers.ChangeToDefaultSort(CInsensitive, "Bar (foo)", out noChange), Is.EqualTo(CInsensitive), "no change when defaultsort only differs to article title by case");
            ClassicAssert.IsTrue(noChange);

            string CInsensitive2 = @"{{DEFAULTSORT:Bar}} [[Category:Foo]]";

            Assert.That(Parsers.ChangeToDefaultSort(CInsensitive2, "BAR", out noChange), Is.EqualTo(CInsensitive2), "no change when existing defaultsort only differs to article title by case");
            ClassicAssert.IsTrue(noChange);

            CInsensitive2 = @"{{DEFAULTSORT:bar}} [[Category:Foo]]";

            Assert.That(Parsers.ChangeToDefaultSort(CInsensitive2, "BAR", out noChange), Is.EqualTo(CInsensitive2), "no change when existing defaultsort only differs to article title by case");
            ClassicAssert.IsTrue(noChange);

            CInsensitive2 = @"{{DEFAULTSORT:BAR}} [[Category:Foo]]";

            Assert.That(Parsers.ChangeToDefaultSort(CInsensitive2, "BAR", out noChange), Is.EqualTo(CInsensitive2), "no change when existing defaultsort only differs to article title by case");
            ClassicAssert.IsTrue(noChange);
        }

        [Test]
        public void ChangeToDefaultSortPAGENAME()
        {
            bool noChange;

            Assert.That(Parsers.ChangeToDefaultSort("[[Category:Test1|{{PAGENAME}}]][[Category:Test2]]", "Foooo", out noChange),
                            Is.EqualTo("[[Category:Test1]][[Category:Test2]]"));
            ClassicAssert.IsFalse(noChange);

            Assert.That(Parsers.ChangeToDefaultSort("[[Category:Test1|{{subst:PAGENAME}}]][[Category:Test2]]", "Foooo", out noChange),
                            Is.EqualTo("[[Category:Test1]][[Category:Test2]]"));
            ClassicAssert.IsFalse(noChange);
        }

        [Test]
        public void ChangeToDefaultSortMultiple()
        {
            bool noChange;
            const string Multi = "[[Category:Test1|Foooo]][[Category:Test2|Foooo]]\r\n{{DEFAULTSORT:Foooo}}\r\n{{DEFAULTSORTKEY:Foo2oo}}";

            Assert.That(Parsers.ChangeToDefaultSort(Multi, "Bar", out noChange), Is.EqualTo(Multi), "no change when multiple different defaultsorts");
            ClassicAssert.IsTrue(noChange);
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

            ClassicAssert.IsFalse(noChange);

            Assert.That(Parsers.ChangeToDefaultSort(@"{{DEFAULTSORT:Willis, Bobby}}
[[Category:1999 deaths|Willis]]
[[Category:1942 births|Foo]]
[[Category:Cancer deaths in the United Kingdom]]", "Bobby Willis", out noChange), Is.EqualTo(@"{{DEFAULTSORT:Willis, Bobby}}
[[Category:1999 deaths]]
[[Category:1942 births|Foo]]
[[Category:Cancer deaths in the United Kingdom]]"));

            ClassicAssert.IsFalse(noChange);
        }

        [Test]
        public void ChangeToDefaultSortHuman()
        {
            bool noChange;

            const string a = @"Fred Smith blah [[Category:Living people]]";
            const string b = "\r\n" + @"{{DEFAULTSORT:Smith, Fred}}";

            Assert.That(Parsers.ChangeToDefaultSort(a, "Fred Smith", out noChange), Is.EqualTo(a + b));
            ClassicAssert.IsFalse(noChange);

            string a2 = @"Fred Smith blah {{imdb name|id=abc}} [[Category:Living people]]";

            Assert.That(Parsers.ChangeToDefaultSort(a2, "Fred Smith", out noChange), Is.EqualTo(a2 + b));
            ClassicAssert.IsFalse(noChange);

            // no defaultsort added if restricted defaultsort addition on
            Assert.That(Parsers.ChangeToDefaultSort(a, "Fred Smith", out noChange, true), Is.EqualTo(a));
            ClassicAssert.IsTrue(noChange);

            const string c = @"Stéphanie Mahieu blah [[Category:Living people]]";
            const string d = "\r\n" + @"{{DEFAULTSORT:Mahieu, Stephanie}}";

            Assert.That(Parsers.ChangeToDefaultSort(c, "Stéphanie Mahieu", out noChange), Is.EqualTo(c + d));
            ClassicAssert.IsFalse(noChange);
        }

        [Test]
        public void TestDefaultsortTitlesWithDiacritics()
        {
            bool noChange;

            Assert.That(Parsers.ChangeToDefaultSort(@"[[Category:Parishes in Asturias]]", "Abándames", out noChange),
                            Is.EqualTo(@"[[Category:Parishes in Asturias]]
{{DEFAULTSORT:Abandames}}"));
            ClassicAssert.IsFalse(noChange);

            // no change if a defaultsort already there
            Assert.That(Parsers.ChangeToDefaultSort(@"[[Category:Parishes in Asturias]]
{{DEFAULTSORT:Bert}}", "Abándames", out noChange),
                            Is.EqualTo(@"[[Category:Parishes in Asturias]]
{{DEFAULTSORT:Bert}}"));
            ClassicAssert.IsTrue(noChange);


            // category sortkeys are cleaned too
            Assert.That(Parsers.ChangeToDefaultSort(@"[[Category:Parishes of the Azores|Agua Retorta]]
[[Category:São Miguel Island]]", @"Água Retorta", out noChange), Is.EqualTo(@"[[Category:Parishes of the Azores]]
[[Category:São Miguel Island]]
{{DEFAULTSORT:Agua Retorta}}"));
            ClassicAssert.IsFalse(noChange);

            // use article name
            Assert.That(Parsers.ChangeToDefaultSort(@"[[Category:Parishes of the Azores]]
[[Category:São Miguel Island]]", @"Água Retorta", out noChange), Is.EqualTo(@"[[Category:Parishes of the Azores]]
[[Category:São Miguel Island]]
{{DEFAULTSORT:Agua Retorta}}"));
            ClassicAssert.IsFalse(noChange);
        }

        [Test]
        public void TestIsArticleAboutAPerson()
        {
            ClassicAssert.IsTrue(Parsers.IsArticleAboutAPerson(@"Foo {{infobox person|name=smith}}", "foo"));
            ClassicAssert.IsTrue(Parsers.IsArticleAboutAPerson(@"Foo [[Category:1900 deaths]]", "foo"));
            ClassicAssert.IsTrue(Parsers.IsArticleAboutAPerson(@"Foo [[Category:1900 births]]", "foo"));
            ClassicAssert.IsTrue(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Living people]]", "foo"));
            ClassicAssert.IsTrue(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Year of birth missing (living people)]]", "foo"));
            ClassicAssert.IsTrue(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Living people|Smith]]", "foo"));

            ClassicAssert.IsTrue(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Living people]] [[Category:Living people]]", "foo"), "duplicate categories removed, so okay");

            ClassicAssert.IsTrue(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{England-bio-stub}}", "foo"));
            ClassicAssert.IsTrue(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{Switzerland-politician-stub}}", "foo"));
            ClassicAssert.IsTrue(Parsers.IsArticleAboutAPerson(@"Some words {{death date and age|1960|01|9}}", "foo"));
            ClassicAssert.IsTrue(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{RefimproveBLP}}", "foo"),"BLP template");
            ClassicAssert.IsTrue(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "foo"),"BLP sources template");
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "16 and pregnant"),"BLP sources template");
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP unsourced section|foo=bar}}", "foo"),"BLP unsourced template");

            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo {{Infobox actor|name=smith}}", "Category:foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "List of foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Lists of foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' [[Category:Missing people organizations]]", "Foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Deaths in 2004"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "First Assembly of X"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Pierre Schaeffer bibliography"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Adoption of x"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo (family)"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo (x family)"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo (x team)"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo (publisher)"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo haunting"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo martyrs"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo quartet"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo team"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo twins"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Attack on x"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Suicide of x"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Presidency of x"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Governor of x"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Mayoralty of x"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "First presidency of x"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "2004 something"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "2004–09 something"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo discography"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo children"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo murders"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo, bar and other"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo & other"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo, bar, and Other"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo One and Other People"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo groups"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "The Foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo people"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo campaign"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo rebellion"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo native"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo center"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Second Foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo x families"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Brothers Foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "X from Y"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo brothers"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo Sisters"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "X Service"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo (artists)"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo x families (bar)"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "X in Y"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo campaign, 2000"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Atlanta murders of 1979–1981"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Death rates in the 20th century"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Birth rates in the 20th century"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Death rates in the 1st century"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Death rates in the 2nd century"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Death rates in the 3nd century"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:1900 deaths]] and [[Category:1905 deaths]]", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo {{infobox some organization|foo=bar}} {{foo-bio-stub}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Some noble families]] {{foo-bio-stub}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Noble families]] {{foo-bio-stub}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:X teams and stables]] {{foo-bio-stub}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:German families]] {{foo-bio-stub}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:German x families]] {{foo-bio-stub}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:X diaspora in y]] {{foo-bio-stub}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Baronies x]] {{foo-bio-stub}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Groups x]] {{foo-bio-stub}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:X royal families]] {{foo-bio-stub}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:X nicknames]] {{foo-bio-stub}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:X pageants]] {{foo-bio-stub}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:X groups]] {{foo-bio-stub}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:X magazines]] {{foo-bio-stub}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Positions x]] {{foo-bio-stub}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:X troupes]] {{foo-bio-stub}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:X y groups]] {{foo-bio-stub}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:1900 establishments in X]] {{foo-bio-stub}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:X gods]] {{foo-bio-stub}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Companies foo]] {{foo-bio-stub}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Surnames]] {{foo-bio-stub}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:X musical groups]] {{foo-bio-stub}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:X bands]] {{foo-bio-stub}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:X music groups]] {{foo-bio-stub}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:X titles]] {{foo-bio-stub}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Performing groups]] {{foo-bio-stub}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:X ethnic groups]] {{foo-bio-stub}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:X artist groups in]] {{foo-bio-stub}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Musical groups established in 2000]] {{foo-bio-stub}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo {{infobox television|bye=a}} {{refimproveBLP}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{England-bio-stub}} {{sia}}", "Foo"));

            ClassicAssert.IsTrue(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Afrikaner people]] {{foo-bio-stub}}", "foo"),"category about people");

            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo {{Infobox settlement}} {{foo-bio-stub}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo {{italic title}} {{foo-bio-stub}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo {{Infobox racehorse}} {{foo-bio-stub}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo {{Infobox named horse}} {{foo-bio-stub}}", "foo"));

            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Married couples]] {{infoxbox actor|name=smith}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:2002 animal births]] {{infoxbox actor|name=smith}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Comedy duos]] {{infoxbox actor|name=smith}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Comedy trios]] {{infoxbox actor|name=smith}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Foo Comedy duos]] {{infoxbox actor|name=smith}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo {{In-universe}} {{infoxbox actor|name=smith}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo {{in-universe}} {{infoxbox actor|name=smith}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo {{Infobox political party}} {{birth date and age|1974|11|26}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Articles about multiple people]] {{infoxbox actor|name=smith}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Fictional blah]] {{infoxbox actor|name=smith}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[fictional character]] {{infoxbox actor|name=smith}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[fictional character|character]] {{infoxbox actor|name=smith}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo {{dab}} {{infoxbox actor|name=smith}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Internet memes]] {{bda|1980|11|11}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Military animals]] {{bda|1980|11|11}}", "foo"));

            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:2002 births]] {{infobox Musical artist|Background=group_or_band}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:2002 births]] {{infobox Musical artist|background=group_or_band}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:2002 births]] {{infobox musical artist|Background=group_or_band}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:2002 births]] {{infobox Musical artist|Background=classical_ensemble}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:2002 births]] {{Infobox Chinese-language singer and actor|currentmembers=A, B}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:2002 births]] {{infobox Band|Background=group_or_band}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:2002 births]] {{infobox Musical artist|Background=band}}", "foo"));
            ClassicAssert.IsTrue(Parsers.IsArticleAboutAPerson(@"Foo [[Category:2002 births]] {{infobox musical artist|Background=other}}", "foo"));

            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo {{infobox person|name=smith}} Foo {{infobox person|name=smith2}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo {{death date|2002}} {{death date|2005}}", "foo"));

            // multiple different birth dates means not about one person
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"{{nat fs player|no=1|pos=GK|name=[[Meg]]|age={{Birth date|1956|01|01}} ({{Age at date|1956|01|01|1995|6|5}})|caps=|club=|clubnat=}}
{{nat fs player|no=2|pos=MF|name=[[Valeria]]|age={{Birth date|1968|09|03}} ({{Age at date|1968|09|03|1995|6|5}})|caps=|club=|clubnat=}}", "foo"));
            ClassicAssert.IsTrue(Parsers.IsArticleAboutAPerson(@"{{infobox actor|no=1|pos=GK|name=[[Meg]]|age={{Birth date|1956|01|01}} }} {{Birth date|1956|01|01}}", "foo"));

            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"{{see also|Fred}} Fred Smith is great == foo == {{infoxbox actor}}", "foo"));
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"{{Main|Fred}} Fred Smith is great == foo == {{infoxbox actor}}", "foo"));

            // link in bold in zeroth section to somewhere else is no good
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''military career of [[Napoleon Bonaparte]]''' == foo == {{birth date|2008|11|11}}", "foo"));

            // 'characters' category means fictional person
            ClassicAssert.IsFalse(Parsers.IsArticleAboutAPerson(@"foo [[Category:227 characters]] {{infoxbox actor}}", "foo"));

            ClassicAssert.IsTrue(Parsers.IsArticleAboutAPerson(@"'''Margaret Sidney''' was the [[pseudonym]] of American author '''Harriett Mulford Stone''' (June 22, 1844–August 2, 1924).
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
            ClassicAssert.IsTrue(Parsers.IsArticleAboutAPerson(AR, "Opeti Fonua"),"Infobox about a person");
            
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
            ClassicAssert.IsTrue(Parsers.HasSicTag("now helo [sic] there"));
            ClassicAssert.IsTrue(Parsers.HasSicTag("now helo [sic!] there"));
            ClassicAssert.IsTrue(Parsers.HasSicTag("now helo[sic] there"));
            ClassicAssert.IsTrue(Parsers.HasSicTag("now helo (sic) there"));
            ClassicAssert.IsTrue(Parsers.HasSicTag("now helo (sic!) there"));
            ClassicAssert.IsTrue(Parsers.HasSicTag("now helo {sic} there"));
            ClassicAssert.IsTrue(Parsers.HasSicTag("now helo [Sic] there"));
            ClassicAssert.IsTrue(Parsers.HasSicTag("now helo [ Sic ] there"));
            ClassicAssert.IsTrue(Parsers.HasSicTag("now {{sic|helo}} there"));
            ClassicAssert.IsTrue(Parsers.HasSicTag("now {{sic|hel|o}} there"));
            ClassicAssert.IsTrue(Parsers.HasSicTag("now {{typo|helo}} there"));
            ClassicAssert.IsTrue(Parsers.HasSicTag("now helo <!--[sic]-->there"));

            ClassicAssert.IsFalse(Parsers.HasSicTag("now sickened by"));
            ClassicAssert.IsFalse(Parsers.HasSicTag("sic transit gloria mundi"));
            ClassicAssert.IsFalse(Parsers.HasSicTag("The Sound Information Company (SIC) is"));
        }

        [Test]
        public void HasMorefootnotesAndManyReferencesTests()
        {
            ClassicAssert.IsTrue(Parsers.HasMorefootnotesAndManyReferences(@"Article<ref>A</ref> <ref>B</ref> <ref>C</ref> <ref>B</ref> <ref>E</ref>
==References==
{{reflist}}
{{nofootnotes}}"));

            ClassicAssert.IsTrue(Parsers.HasMorefootnotesAndManyReferences(@"Article<ref>A</ref> <ref>B</ref> <ref>C</ref> <ref>B</ref> <ref>E</ref>
==References==
{{reflist}}
{{morefootnotes}}"));

            ClassicAssert.IsTrue(Parsers.HasMorefootnotesAndManyReferences(@"Article<ref name=A>A</ref> <ref name=B>B</ref> <ref>C</ref> <ref>B</ref> <ref>E</ref>
==References==
{{reflist}}
{{Morefootnotes}}"));

            // not enough references
            ClassicAssert.IsFalse(Parsers.HasMorefootnotesAndManyReferences(@"Article<ref>A</ref> <ref>B</ref> <ref>C</ref> <ref>B</ref>
==References==
{{reflist}}
{{nofootnotes}}"));

            // no {{nofootnotes}}
            ClassicAssert.IsFalse(Parsers.HasMorefootnotesAndManyReferences(@"Article<ref name=A>A</ref> <ref name=B>B</ref> <ref>C</ref> <ref>B</ref> <ref>E</ref>
==References==
{{reflist}}"));
        }

        [Test]
        public void HasRefAfterReflistTest()
        {
            ClassicAssert.IsTrue(Parsers.HasRefAfterReflist(@"blah <ref>a</ref> ==references== {{reflist}} <ref>b</ref>"));
            ClassicAssert.IsTrue(Parsers.HasRefAfterReflist(@"blah <ref>a</ref>
==references== {{reflist}} <ref>b</ref>"));
            ClassicAssert.IsTrue(Parsers.HasRefAfterReflist(@"blah <ref>a</ref> ==references== {{reflist}} <ref name=""b"">b</ref>"));

            // this is correct syntax
            ClassicAssert.IsFalse(Parsers.HasRefAfterReflist(@"blah <ref>a</ref> ==references== {{reflist}}"));
            ClassicAssert.IsFalse(Parsers.HasRefAfterReflist(@"blah.(Jones 2000)"));
            // ignores commented out refs
            ClassicAssert.IsFalse(Parsers.HasRefAfterReflist(@"blah <ref>a</ref> ==references== {{reflist}} <!--<ref>b</ref>-->"));

            // the second template means this is okay too
            ClassicAssert.IsFalse(Parsers.HasRefAfterReflist(@"blah <ref>a</ref> ==references== {{reflist}} <ref name=""b"">b</ref> {{reflist}}"));

            // 'r' in argument means no embedded <ref></ref>
            ClassicAssert.IsFalse(Parsers.HasRefAfterReflist(@"blah <ref>a</ref> ==references== {{reflist|refs=<ref>abc</ref>}}"));
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
            ClassicAssert.IsFalse(Parsers.HasRefAfterReflist(bug1));

            #if DEBUG
            Variables.SetProjectLangCode("fr");

            ClassicAssert.IsFalse(Parsers.HasRefAfterReflist(@"blah <ref>a</ref>
==references== {{reflist}} <ref>b</ref>"));
            Variables.SetProjectLangCode("en");
            #endif

            ClassicAssert.IsFalse(Parsers.HasRefAfterReflist(@""));
        }

        [Test]
        public void HasInUseTagTests()
        {
            ClassicAssert.IsTrue(Parsers.IsInUse("{{inuse}} Hello world"));
            ClassicAssert.IsTrue(Parsers.IsInUse("{{in creation}} Hello world"));
            ClassicAssert.IsTrue(Parsers.IsInUse("{{increation}} Hello world"));
            ClassicAssert.IsTrue(Parsers.IsInUse("{{ inuse  }} Hello world"));
            ClassicAssert.IsTrue(Parsers.IsInUse("{{Inuse}} Hello world"));
            ClassicAssert.IsTrue(Parsers.IsInUse("Hello {{inuse}} Hello world"));
            ClassicAssert.IsTrue(Parsers.IsInUse("{{inuse|5 minutes}} Hello world"));
            ClassicAssert.IsTrue(Parsers.IsInUse("{{In use}} Hello world"));
            ClassicAssert.IsTrue(Parsers.IsInUse("{{in use|5 minutes}} Hello world"));


            // ignore commented inuse
            ClassicAssert.IsFalse(Parsers.IsInUse("<!--{{inuse}}--> Hello world"));
            ClassicAssert.IsFalse(Parsers.IsInUse("<nowiki>{{inuse}}</nowiki> Hello world"));
            ClassicAssert.IsFalse(Parsers.IsInUse("<nowiki>{{in use}}</nowiki> Hello world"));
            ClassicAssert.IsTrue(Parsers.IsInUse("<!--{{inuse}}--> {{inuse|5 minutes}} Hello world"));
            ClassicAssert.IsTrue(Parsers.IsInUse("<!--{{inuse}}--> {{in use|5 minutes}} Hello world"));

            ClassicAssert.IsFalse(Parsers.IsInUse("{{INUSE}} Hello world")); // no such template

#if DEBUG
            Variables.SetProjectLangCode("el");
            WikiRegexes.MakeLangSpecificRegexes();
            
            ClassicAssert.IsTrue(Parsers.IsInUse("{{Σε χρήση}} Hello world"), "σε χρήση");
            ClassicAssert.IsTrue(Parsers.IsInUse("{{inuse}} Hello world"), "inuse");
            ClassicAssert.IsFalse(Parsers.IsInUse("{{goceinuse}} Hello world"), "goceinuse is en-only");

#endif
        }

        [Test]
        public void IsMissingReferencesDisplayTests()
        {
            ClassicAssert.IsTrue(Parsers.IsMissingReferencesDisplay(@"Hello<ref>Fred</ref>"));
            ClassicAssert.IsTrue(Parsers.IsMissingReferencesDisplay(@"Hello<ref name=""F"">Fred</ref>"));

            // {{GR}} provides an embedded <ref></ref> if its argument is a decimal
            ClassicAssert.IsTrue(Parsers.IsMissingReferencesDisplay(@"Hello{{GR|4}}"));

            ClassicAssert.IsFalse(Parsers.IsMissingReferencesDisplay(@"Hello"));
            ClassicAssert.IsFalse(Parsers.IsMissingReferencesDisplay(@"Hello<ref>Fred</ref> {{reflist}}"));
            ClassicAssert.IsFalse(Parsers.IsMissingReferencesDisplay(@"Hello<ref>Fred</ref> {{Reflist}}"));
            ClassicAssert.IsFalse(Parsers.IsMissingReferencesDisplay(@"Hello<ref>Fred</ref> {{Reflist
|refs = 
{{cite news | title = A { hello }}
}}"), "Unbalanced brackets within cite template in reflist does not affect logic");
            ClassicAssert.IsFalse(Parsers.IsMissingReferencesDisplay(@"Hello<ref>Fred</ref> {{ref-list}}"));
            ClassicAssert.IsFalse(Parsers.IsMissingReferencesDisplay(@"Hello<ref>Fred</ref> {{reflink}}"));
            ClassicAssert.IsFalse(Parsers.IsMissingReferencesDisplay(@"Hello<ref>Fred</ref> {{references}}"));
            ClassicAssert.IsFalse(Parsers.IsMissingReferencesDisplay(@"Hello<ref>Fred</ref> <references/>"));
            ClassicAssert.IsFalse(Parsers.IsMissingReferencesDisplay(@"Hello{{GR|4}} <references/>"));

            // this specifies to {{GR}} not to embed <ref></ref>
            ClassicAssert.IsFalse(Parsers.IsMissingReferencesDisplay(@"Hello{{GR|r4}}"));
            ClassicAssert.IsFalse(Parsers.IsMissingReferencesDisplay(@"Hello{{GR|India}}"));

            ClassicAssert.IsFalse(Parsers.IsMissingReferencesDisplay(@"{{Reflist|refs=
<ref name=modern>{{cite news |first=William }}
        }}"));

            ClassicAssert.IsFalse(Parsers.IsMissingReferencesDisplay(@"Hello<ref group=X>Fred</ref>"));
        }

        [Test]
        public void IsMissingReferencesDisplayTestsEnOnly()
        {
#if DEBUG
            Variables.SetProjectLangCode("fr");

            ClassicAssert.IsFalse(Parsers.IsMissingReferencesDisplay(@"Hello<ref>Fred</ref>"));
            ClassicAssert.IsFalse(Parsers.IsMissingReferencesDisplay(@"Hello<ref name=""F"">Fred</ref>"));

            Variables.SetProjectLangCode("en");
#endif
        }

        [Test]
        public void InfoboxTests()
        {
            ClassicAssert.IsTrue(Parsers.HasInfobox(@"{{Infobox fish | name = Bert }} ''Bert'' is a good fish."));
            ClassicAssert.IsTrue(Parsers.HasInfobox(@"{{Infobox
fish | name = Bert }} ''Bert'' is a good fish."));
            ClassicAssert.IsTrue(Parsers.HasInfobox(@"{{infobox fish | name = Bert }} ''Bert'' is a good fish."));
            ClassicAssert.IsTrue(Parsers.HasInfobox(@"{{ infobox fish | name = Bert }} ''Bert'' is a good fish."));
            ClassicAssert.IsTrue(Parsers.HasInfobox(@"{{ infobox fish | name = Bert }} ''Bert'' is a good <!--fish-->."));

            ClassicAssert.IsFalse(Parsers.HasInfobox(@"{{INFOBOX fish | name = Bert }} ''Bert'' is a good fish."));
            ClassicAssert.IsFalse(Parsers.HasInfobox(@"{{infoboxfish | name = Bert }} ''Bert'' is a good fish."));
            ClassicAssert.IsFalse(Parsers.HasInfobox(@"<!--{{infobox fish | name = Bert }}--> ''Bert'' is a good fish."));
            ClassicAssert.IsFalse(Parsers.HasInfobox(@"<nowiki>{{infobox fish | name = Bert }}</nowiki> ''Bert'' is a good fish."));
        }

        [Test]
        public void InfoboxTestsEnOnly()
        {
#if DEBUG
            Variables.SetProjectLangCode("fr");
            ClassicAssert.IsFalse(Parsers.HasInfobox(@"{{Infobox fish | name = Bert }} ''Bert'' is a good fish."));
            ClassicAssert.IsFalse(Parsers.HasInfobox(@"{{INFOBOX fish | name = Bert }} ''Bert'' is a good fish."));
            Variables.SetProjectLangCode("en");
            ClassicAssert.IsTrue(Parsers.HasInfobox(@"{{Infobox fish | name = Bert }} ''Bert'' is a good fish."));
#endif
        }

        [Test]
        public void HasStubTemplate()
        {
            ClassicAssert.IsTrue(Parsers.HasStubTemplate(@"foo {{foo stub}}"));
            ClassicAssert.IsTrue(Parsers.HasStubTemplate(@"foo {{foo-stub}}"));

            ClassicAssert.IsFalse(Parsers.HasStubTemplate(@"foo {{foo tubs}}"));
        }

        [Test]
        public void HasStubTemplateAr()
        {
#if DEBUG
            Variables.SetProjectLangCode("ar");
            WikiRegexes.MakeLangSpecificRegexes();
            
            ClassicAssert.IsTrue(Parsers.HasStubTemplate(@"foo {{بذرة ممثل}}"), "actor stub");
            ClassicAssert.IsTrue(Parsers.HasStubTemplate(@"foo {{بذرة ألمانيا}}"), "germany stub");
            
            Variables.SetProjectLangCode("en");
            WikiRegexes.MakeLangSpecificRegexes();
#endif
        }

        [Test]
        public void IsStub()
        {
            ClassicAssert.IsTrue(Parsers.IsStub(@"foo {{foo stub}}"));
            ClassicAssert.IsTrue(Parsers.IsStub(@"foo {{foo-stub}}"));

            // short article
            ClassicAssert.IsTrue(Parsers.IsStub(@"foo {{foo tubs}}"));

            const string a = @"fooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo";
            const string b = a + a + a + a + a + a;
            ClassicAssert.IsFalse(Parsers.IsStub(b + b + b + b));
        }

        [Test]
        public void IsStubAr()
        {
#if DEBUG
            Variables.SetProjectLangCode("ar");
            ClassicAssert.IsTrue(Parsers.IsStub(@"foo {{بذرة ممثل}}"));

            // short article
            ClassicAssert.IsTrue(Parsers.IsStub(@"foo {{foo tubs}}"));

            const string a = @"fooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo";
            const string b = a + a + a + a + a + a;
            ClassicAssert.IsFalse(Parsers.IsStub(b + b + b + b));
            Variables.SetProjectLangCode("en");
#endif
        }

        [Test]
        public void IsStubArz()
        {
#if DEBUG
            Variables.SetProjectLangCode("arz");
            ClassicAssert.IsTrue(Parsers.IsStub(@"foo {{بذرة}}"));

            // short article
            ClassicAssert.IsTrue(Parsers.IsStub(@"foo {{foo tubs}}"));

            const string a = @"fooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo";
            const string b = a + a + a + a + a + a;
            ClassicAssert.IsFalse(Parsers.IsStub(b + b + b + b));
            Variables.SetProjectLangCode("en");
#endif
        }

        [Test]
        public void NoBotsTests()
        {
            ClassicAssert.IsTrue(Parsers.CheckNoBots("", ""));
            ClassicAssert.IsTrue(Parsers.CheckNoBots("{{test}}", ""));
            ClassicAssert.IsTrue(Parsers.CheckNoBots("lol, test", ""));

            ClassicAssert.IsTrue(Parsers.CheckNoBots("{{bots}}", ""));
            ClassicAssert.IsTrue(Parsers.CheckNoBots("{{bots|allow=awb}}", ""));
            ClassicAssert.IsTrue(Parsers.CheckNoBots("{{bots|allow=test}}", "test"));
            ClassicAssert.IsTrue(Parsers.CheckNoBots("{{bots|allow=user,test}}", "test"));
            ClassicAssert.IsTrue(Parsers.CheckNoBots("{{bots|deny=none}}", ""));

            ClassicAssert.IsTrue(Parsers.CheckNoBots("{{bots|deny=Xenobot Mk V}}", "Xenobot"));
            ClassicAssert.IsTrue(Parsers.CheckNoBots("{{bots|deny=Xenobot}}", "Xenobot Mk V"));

            ClassicAssert.IsFalse(Parsers.CheckNoBots("{{nobots}}", ""));
            ClassicAssert.IsFalse(Parsers.CheckNoBots("{{bots|deny=all}}", ""));
            ClassicAssert.IsFalse(Parsers.CheckNoBots("<!--comm-->{{bots|deny=all}}", ""));
            ClassicAssert.IsFalse(Parsers.CheckNoBots(@"<!--comm
-->{{bots|deny=all}}", ""));
            ClassicAssert.IsFalse(Parsers.CheckNoBots("{{bots|deny=awb}}", ""));
            ClassicAssert.IsFalse(Parsers.CheckNoBots("{{bots|deny=awb,test}}", ""));
            ClassicAssert.IsFalse(Parsers.CheckNoBots("{{bots|deny=awb,test}}", "test"));
            ClassicAssert.IsFalse(Parsers.CheckNoBots("{{nobots|deny=awb,test}}", "test"));
            ClassicAssert.IsFalse(Parsers.CheckNoBots("{{bots|deny=test}}", "test"));
            ClassicAssert.IsFalse(Parsers.CheckNoBots("{{bots|allow=none}}", ""));
            ClassicAssert.IsFalse(Parsers.CheckNoBots(@"{{bots|deny=AWB, MenoBot, MenoBot II}}", "AWB and other bots"));
            ClassicAssert.IsTrue(Parsers.CheckNoBots("<!-- comm {{bots|deny=all}} -->", ""));

            ClassicAssert.IsFalse(Parsers.CheckNoBots(@"{{bots|allow=MiszaBot III,SineBot}}", "otherBot"));
            ClassicAssert.IsTrue(Parsers.CheckNoBots(@"{{bots|allow=MiszaBot III,SineBot}}", "SineBot"));
            ClassicAssert.IsTrue(Parsers.CheckNoBots(@"{{bots|allow=MiszaBot III,SineBot, AWB}}", "SomeotherBot, AWB last"));
            ClassicAssert.IsTrue(Parsers.CheckNoBots(@"{{bots|allow=AWB, MiszaBot III,SineBot}}", "SomeotherBot, AWB first"));

            /* prospective future changes to bots template
            ClassicAssert.IsTrue(Parsers.CheckNoBots(@"{{bots|deny=all|allow=MiszaBot III,SineBot}}", "SineBot"));
            ClassicAssert.IsTrue(Parsers.CheckNoBots(@"{{bots|deny=all|allow=MiszaBot III,SineBot}}", "MiszaBot III"));
            ClassicAssert.IsFalse(Parsers.CheckNoBots(@"{{bots|deny=all|allow=MiszaBot III,SineBot}}", "OtherBot")); */
        }

        [Test]
        public void NoIncludeIncludeOnlyProgrammingElement()
        {
            ClassicAssert.IsTrue(Parsers.NoIncludeIncludeOnlyProgrammingElement(@"<noinclude>blah</noinclude>"));
            ClassicAssert.IsTrue(Parsers.NoIncludeIncludeOnlyProgrammingElement(@"<includeonly>blah</includeonly>"));
            ClassicAssert.IsTrue(Parsers.NoIncludeIncludeOnlyProgrammingElement(@"<onlyinclude>blah</onlyinclude>"));
            ClassicAssert.IsTrue(Parsers.NoIncludeIncludeOnlyProgrammingElement(@"{{{1}}}"));
            ClassicAssert.IsTrue(Parsers.NoIncludeIncludeOnlyProgrammingElement(@"{{{3}}}"));

            ClassicAssert.IsFalse(Parsers.NoIncludeIncludeOnlyProgrammingElement(@"hello"));
            ClassicAssert.IsFalse(Parsers.NoIncludeIncludeOnlyProgrammingElement(@""));
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
            ClassicAssert.IsTrue(res.ContainsKey("volume"));
            ClassicAssert.IsTrue(res.ContainsKey("issue"));
            ClassicAssert.IsTrue(res.ContainsKey("spage"));
            ClassicAssert.IsTrue(res.ContainsKey("aulast"));
            ClassicAssert.IsTrue(res.ContainsKey("atitle"));
            ClassicAssert.IsTrue(res.ContainsKey("date"));
            ClassicAssert.IsTrue(res.ContainsKey("jtitle"));
            string v;
            res.TryGetValue("volume", out v);
            Assert.That(v, Is.EqualTo("159"));
            res.TryGetValue("jtitle", out v);
            Assert.That(v, Is.EqualTo("Nature Structural &#38; Molecular Biology"));
        }
    }
}
