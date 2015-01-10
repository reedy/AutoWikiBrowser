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
    /// <summary>
    /// This class must be inhertited by test fixtures that use most parts of WikiFunctions.
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
            Assert.AreEqual("[[Category:Test1|Foooo]]",
                            Parsers.ChangeToDefaultSort("[[Category:Test1|Foooo]]", "Foo", out noChange),"don't change sorting for single categories");
            Assert.IsTrue(noChange,"don't change sorting for single categories");

            // should work
            Assert.AreEqual("[[Category:Test1]][[Category:Test2]]\r\n{{DEFAULTSORT:Foooo}}",
                            Parsers.ChangeToDefaultSort("[[Category:Test1|Foooo]][[Category:Test2|Foooo]]", "Bar",
                                                        out noChange));
            Assert.IsFalse(noChange);

            // ...but don't add DEFAULTSORT if the key equals page title
            Assert.AreEqual("[[Category:Test1]][[Category:Test2]]",
                            Parsers.ChangeToDefaultSort("[[Category:Test1|Foooo]][[Category:Test2|Foooo]]", "Foooo",
                                                        out noChange));
            Assert.IsFalse(noChange, "Should detect a change even if it hasn't added a DEFAULTSORT");

            // don't change if key is 3 chars or less
            Assert.AreEqual("[[Category:Test1|Foo]][[Category:Test2|Foo]]",
                            Parsers.ChangeToDefaultSort("[[Category:Test1|Foo]][[Category:Test2|Foo]]", "Bar", out noChange));
            Assert.IsTrue(noChange);

            // Remove explicit keys equal to page title
            Assert.AreEqual("[[Category:Test1]][[Category:Test2]]",
                            Parsers.ChangeToDefaultSort("[[Category:Test1|Foooo]][[Category:Test2]]", "Foooo", out noChange));
            Assert.IsFalse(noChange);

            // swap
            Assert.AreEqual("[[Category:Test1]][[Category:Test2]]",
                            Parsers.ChangeToDefaultSort("[[Category:Test1]][[Category:Test2|Foooo]]", "Foooo", out noChange));
            Assert.IsFalse(noChange);

            // Borderline condition
            Assert.AreEqual("[[Category:Test1|Fooooo]][[Category:Test2]]",
                            Parsers.ChangeToDefaultSort("[[Category:Test1|Fooooo]][[Category:Test2]]", "Foooo", out noChange));
            Assert.IsTrue(noChange);

            // Don't change anything if there's ambiguity
            Assert.AreEqual("[[Category:Test1|Foooo]][[Category:Test2|Baaar]]",
                            Parsers.ChangeToDefaultSort("[[Category:Test1|Foooo]][[Category:Test2|Baaar]]", "Teeest",
                                                        out noChange));
            Assert.IsTrue(noChange);
            // same thing
            Assert.AreEqual("[[Category:Test1|Foooo]][[Category:Test2|Baaar]]",
                            Parsers.ChangeToDefaultSort("[[Category:Test1|Foooo]][[Category:Test2|Baaar]]", "Foooo",
                                                        out noChange));
            Assert.IsTrue(noChange);

            // remove diacritics when generating a key
            Assert.AreEqual("[[Category:Test1]][[Category:Test2]]\r\n{{DEFAULTSORT:Foooo}}",
                            Parsers.ChangeToDefaultSort("[[Category:Test1|Foooô]][[Category:Test2|Foooô]]", "Bar",
                                                        out noChange));
            Assert.IsFalse(noChange);

            // should also fix diacritics in existing defaultsorts and remove leading spaces
            // also support mimicking templates: template to magic word conversion, see [[Category:Pages which use a template in place of a magic word]]
            Assert.AreEqual("{{DEFAULTSORT:Test}}", Parsers.ChangeToDefaultSort("{{defaultsort| Tést}}", "Foo", out noChange));
            Assert.IsFalse(noChange);
            Assert.AreEqual("{{DEFAULTSORT:Test}}", Parsers.ChangeToDefaultSort("{{DEFAULTSORT| Tést}}", "Foo", out noChange));
            Assert.IsFalse(noChange);
            Assert.AreEqual("{{DEFAULTSORT:Test}}", Parsers.ChangeToDefaultSort("{{DEFAULTSORT:|Test}}", "Foo", out noChange));
            Assert.IsFalse(noChange);

            // shouldn't change whitespace-only sortkeys
            Assert.AreEqual("{{DEFAULTSORT: \t}}", Parsers.ChangeToDefaultSort("{{DEFAULTSORT: \t}}", "Foo", out noChange));
            Assert.IsTrue(noChange);

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_6#DEFAULTSORT_with_spaces
            // DEFAULTSORT doesn't treat leading spaces the same way as categories do
            Assert.AreEqual("[[Category:Test1| Foooo]][[Category:Test2| Foooo]]",
                            Parsers.ChangeToDefaultSort("[[Category:Test1| Foooo]][[Category:Test2| Foooo]]", "Bar",
                                                        out noChange));
            Assert.IsTrue(noChange);

            // pages with multiple sort specifiers shouldn't be changed
            Parsers.ChangeToDefaultSort("{{defaultsort| Tést}}{{DEFAULTSORT: Tést}}", "Foo", out noChange);
            Assert.IsTrue(noChange);

            //Remove explicitally defined sort keys from categories when the page has defaultsort
            Assert.AreEqual("{{DEFAULTSORT:Test}}[[Category:Test]]",
                            Parsers.ChangeToDefaultSort("{{DEFAULTSORT:Test}}[[Category:Test|Test]]", "Foo", out noChange));
            Assert.IsFalse(noChange);

            //Case difference of above
            Assert.AreEqual("{{DEFAULTSORT:Test}}[[Category:Test]]",
                            Parsers.ChangeToDefaultSort("{{DEFAULTSORT:Test}}[[Category:Test|TEST]]", "Foo", out noChange));
            Assert.IsFalse(noChange);

            //No change due to different key
            Assert.AreEqual("{{DEFAULTSORT:Test}}[[Category:Test|Not a Test]]",
                            Parsers.ChangeToDefaultSort("{{DEFAULTSORT:Test}}[[Category:Test|Not a Test]]", "Foo", out noChange));
            Assert.IsTrue(noChange);

            //Multiple to be removed
            Assert.AreEqual("{{DEFAULTSORT:Test}}[[Category:Test]][[Category:Foo]][[Category:Bar]]",
                            Parsers.ChangeToDefaultSort("{{DEFAULTSORT:Test}}[[Category:Test|TEST]][[Category:Foo|Test]][[Category:Bar|test]]", "Foo", out noChange));
            Assert.IsFalse(noChange);

            //Multiple with 1 no key
            Assert.AreEqual("{{DEFAULTSORT:Test}}[[Category:Test]][[Category:Foo]][[Category:Bar]]",
                            Parsers.ChangeToDefaultSort("{{DEFAULTSORT:Test}}[[Category:Test|TEST]][[Category:Foo]][[Category:Bar|test]]", "Foo", out noChange));
            Assert.IsFalse(noChange);

            //Multiple with 1 different key
            Assert.AreEqual("{{DEFAULTSORT:Test}}[[Category:Test]][[Category:Foo|Bar]][[Category:Bar]]",
                            Parsers.ChangeToDefaultSort("{{DEFAULTSORT:Test}}[[Category:Test|TEST]][[Category:Foo|Bar]][[Category:Bar|test]]", "Foo", out noChange));
            Assert.IsFalse(noChange);

            // just removing diacritics in categories is useful
            Assert.AreEqual(@"[[Category:Bronze Wolf awardees|Laine, Juan]]",
                            Parsers.ChangeToDefaultSort(@"[[Category:Bronze Wolf awardees|Lainé, Juan]]", "Hi", out noChange));
            Assert.IsFalse(noChange);
            Assert.AreEqual(@"[[Category:Bronze Wolf awardees|LainI, Juan]]",
                            Parsers.ChangeToDefaultSort(@"[[Category:Bronze Wolf awardees|Lainİ, Juan]]", "Hi", out noChange), "unusual one where lowercase of diacritic is a Latin character");
            Assert.IsFalse(noChange);

            Assert.AreEqual(@"[[Category:Bronze Wolf awardees|Laine, Juan]]",
                            Parsers.ChangeToDefaultSort(@"[[Category:Bronze Wolf awardees|Laine, Juan]]", "Hi", out noChange));
            Assert.IsTrue(noChange);

            // remove duplicate defaultsorts
            Assert.AreEqual(@"foo

[[Category:Businesspeople]]

{{DEFAULTSORT:Phillips, James M.}}
foo", Parsers.ChangeToDefaultSort(@"foo
{{DEFAULTSORT:Phillips, James M.}}
[[Category:Businesspeople]]

{{DEFAULTSORT:Phillips, James M.}}
foo", "Hi", out noChange));

            Assert.AreEqual(@"foo

[[Category:Businesspeople]]


{{DEFAULTSORT:Phillips, James M.}}
foo", Parsers.ChangeToDefaultSort(@"foo
{{DEFAULTSORT:Phillips, James M.}}
[[Category:Businesspeople]]

{{DEFAULTSORT:Phillips, James M.}}
{{DEFAULTSORT:Phillips, James M.}}
foo", "Hi", out noChange));

            // don't remove duplicate different defaultsorts
            Assert.AreEqual(@"foo
{{DEFAULTSORT:Phillips, James M.}}
[[Category:Businesspeople]]

{{DEFAULTSORT:Fred}}
foo", Parsers.ChangeToDefaultSort(@"foo
{{DEFAULTSORT:Phillips, James M.}}
[[Category:Businesspeople]]

{{DEFAULTSORT:Fred}}
foo", "Hi", out noChange));

            Assert.AreEqual(@"
foo {{persondata}}
[[Category:1910 births]]
[[Category:Australian players of Australian rules football]]
[[Category:Essendon Football Club players]]

{{DEFAULTSORT:Lahiff, Tommy}}", Parsers.ChangeToDefaultSort(@"
foo {{persondata}}
[[Category:1910 births|Lahiff, Tommy]]
[[Category:Australian players of Australian rules football|Lahiff, Tommy]]
[[Category:Essendon Football Club players|Lahiff, Tommy]]
", "foo", out noChange, false));

            // can't add a DEFAULTSORT using existing cat sortkeys even if restricted, as sortkey case may be changed
            const string a = @"
foo {{persondata}}
[[Category:1910 births|Lahiff, Tommy]]
[[Category:Australian players of Australian rules football|Lahiff, Tommy]]
[[Category:Essendon Football Club players|Lahiff, Tommy]]
";
            Assert.AreEqual(a, Parsers.ChangeToDefaultSort(a, "foo", out noChange, true));

            Assert.AreEqual(@"
foo {{persondata}}
[[Category:1910 births]]
[[Category:Australian players of Australian rules football]]
[[Category:Essendon Football Club players]]

{{DEFAULTSORT:Lahiff, Tommy}}", Parsers.ChangeToDefaultSort(a, "foo", out noChange, false));

            // can't add a DEFAULTSORT using existing cat sortkeys if they're different
            Assert.AreEqual(@"
foo {{persondata}}
[[Category:1910 births|Lahiff, Tommy]]
[[Category:Australian players of Australian rules football|Lahiff, Tommy]]
[[Category:Essendon Football Club players|TOmmy]]
", Parsers.ChangeToDefaultSort(@"
foo {{persondata}}
[[Category:1910 births|Lahiff, Tommy]]
[[Category:Australian players of Australian rules football|Lahiff, Tommy]]
[[Category:Essendon Football Club players|TOmmy]]
", "foo", out noChange, true));

            // restricted
            const string r1 = @"[[Category:Franks]]
[[Category:Carolingian dynasty]]
[[Category:Frankish people]]
[[Category:811 deaths]]
[[Category:9th-century rulers]]";
            Assert.AreEqual(r1, Parsers.ChangeToDefaultSort(r1, "foo", out noChange, true));

            // namespace not used in DEFAULTSORT key
            Assert.AreEqual(@"foo
[[Category:All foos]]
{{DEFAULTSORT:Special foos}}", Parsers.ChangeToDefaultSort(@"foo
[[Category:All foos]]", "Category:Special foŏs", out noChange, false));

            Assert.AreEqual(@"foo
[[Category:All foos]]
[[Category:All foos2]]
{{DEFAULTSORT:Rail in Izmir}}", Parsers.ChangeToDefaultSort(@"foo
[[Category:All foos]]
[[Category:All foos2]]", "Rail in İzmir", out noChange, false));

            // hyphen in title becomes a minus in DEFAULTSORT key
            Assert.AreEqual(@"foo
{{DEFAULTSORT:Women's Circuit (July-September)}}", Parsers.ChangeToDefaultSort(@"foo
{{DEFAULTSORT:Women's Circuit (July–September)}}", "Women's Circuit (July–September)", out noChange, false));

            // skip when nonclude on page
            const string NoInclude = @"[[Category:Test1|Foooo]][[Category:Test2|Foooo]] <noinclude>foo</noinclude>";
            Assert.AreEqual(NoInclude,
                            Parsers.ChangeToDefaultSort(NoInclude, "Bar",
                                                        out noChange));
            Assert.IsTrue(noChange);
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

            Assert.AreEqual(CInsensitive, Parsers.ChangeToDefaultSort(CInsensitive, "BAR", out noChange), "no change when defaultsort only differs to article title by case");
            Assert.IsTrue(noChange);

            Assert.AreEqual(CInsensitive, Parsers.ChangeToDefaultSort(CInsensitive, "Bar", out noChange), "no change when defaultsort only differs to article title by case");
            Assert.IsTrue(noChange);

            Assert.AreEqual(CInsensitive, Parsers.ChangeToDefaultSort(CInsensitive, "Bar foo", out noChange), "no change when defaultsort only differs to article title by case");
            Assert.IsTrue(noChange);

            Assert.AreEqual(CInsensitive, Parsers.ChangeToDefaultSort(CInsensitive, "Bar (foo)", out noChange), "no change when defaultsort only differs to article title by case");
            Assert.IsTrue(noChange);

            string CInsensitive2 = @"{{DEFAULTSORT:Bar}} [[Category:Foo]]";

            Assert.AreEqual(CInsensitive2, Parsers.ChangeToDefaultSort(CInsensitive2, "BAR", out noChange), "no change when existing defaultsort only differs to article title by case");
            Assert.IsTrue(noChange);

            CInsensitive2 = @"{{DEFAULTSORT:bar}} [[Category:Foo]]";

            Assert.AreEqual(CInsensitive2, Parsers.ChangeToDefaultSort(CInsensitive2, "BAR", out noChange), "no change when existing defaultsort only differs to article title by case");
            Assert.IsTrue(noChange);

            CInsensitive2 = @"{{DEFAULTSORT:BAR}} [[Category:Foo]]";

            Assert.AreEqual(CInsensitive2, Parsers.ChangeToDefaultSort(CInsensitive2, "BAR", out noChange), "no change when existing defaultsort only differs to article title by case");
            Assert.IsTrue(noChange);
        }

        [Test]
        public void ChangeToDefaultSortPAGENAME()
        {
            bool noChange;

            Assert.AreEqual("[[Category:Test1]][[Category:Test2]]",
                            Parsers.ChangeToDefaultSort("[[Category:Test1|{{PAGENAME}}]][[Category:Test2]]", "Foooo", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("[[Category:Test1]][[Category:Test2]]",
                            Parsers.ChangeToDefaultSort("[[Category:Test1|{{subst:PAGENAME}}]][[Category:Test2]]", "Foooo", out noChange));
            Assert.IsFalse(noChange);
        }

        [Test]
        public void ChangeToDefaultSortMultiple()
        {
            bool noChange;
            const string Multi = "[[Category:Test1|Foooo]][[Category:Test2|Foooo]]\r\n{{DEFAULTSORT:Foooo}}\r\n{{DEFAULTSORTKEY:Foo2oo}}";

            Assert.AreEqual(Multi, Parsers.ChangeToDefaultSort(Multi, "Bar", out noChange), "no change when multiple different defaultsorts");
            Assert.IsTrue(noChange);
        }

        [Test]
        public void CategorySortKeyPartialCleaning()
        {
            // to test that if a cat's sortkey is the start of the defaultsort key, it's removed too
            bool noChange;
            Assert.AreEqual(@"{{DEFAULTSORT:Willis, Bobby}}
[[Category:1999 deaths]]
[[Category:1942 births]]
[[Category:Cancer deaths in the United Kingdom]]", Parsers.ChangeToDefaultSort(@"{{DEFAULTSORT:Willis, Bobby}}
[[Category:1999 deaths|Willis]]
[[Category:1942 births]]
[[Category:Cancer deaths in the United Kingdom]]", "Bobby Willis", out noChange));

            Assert.IsFalse(noChange);

            Assert.AreEqual(@"{{DEFAULTSORT:Willis, Bobby}}
[[Category:1999 deaths]]
[[Category:1942 births|Foo]]
[[Category:Cancer deaths in the United Kingdom]]", Parsers.ChangeToDefaultSort(@"{{DEFAULTSORT:Willis, Bobby}}
[[Category:1999 deaths|Willis]]
[[Category:1942 births|Foo]]
[[Category:Cancer deaths in the United Kingdom]]", "Bobby Willis", out noChange));

            Assert.IsFalse(noChange);
        }

        [Test]
        public void ChangeToDefaultSortHuman()
        {
            bool noChange;

            const string a = @"Fred Smith blah [[Category:Living people]]";
            const string b = "\r\n" + @"{{DEFAULTSORT:Smith, Fred}}";

            Assert.AreEqual(a + b, Parsers.ChangeToDefaultSort(a, "Fred Smith", out noChange));
            Assert.IsFalse(noChange);

            string a2 = @"Fred Smith blah {{imdb name|id=abc}} [[Category:Living people]]";

            Assert.AreEqual(a2 + b, Parsers.ChangeToDefaultSort(a2, "Fred Smith", out noChange));
            Assert.IsFalse(noChange);

            // no defaultsort added if restricted defaultsort addition on
            Assert.AreEqual(a, Parsers.ChangeToDefaultSort(a, "Fred Smith", out noChange, true));
            Assert.IsTrue(noChange);

            const string c = @"Stéphanie Mahieu blah [[Category:Living people]]";
            const string d = "\r\n" + @"{{DEFAULTSORT:Mahieu, Stephanie}}";

            Assert.AreEqual(c + d, Parsers.ChangeToDefaultSort(c, "Stéphanie Mahieu", out noChange));
            Assert.IsFalse(noChange);
        }

        [Test]
        public void TestDefaultsortTitlesWithDiacritics()
        {
            bool noChange;

            Assert.AreEqual(@"[[Category:Parishes in Asturias]]
{{DEFAULTSORT:Abandames}}",
                            Parsers.ChangeToDefaultSort(@"[[Category:Parishes in Asturias]]", "Abándames", out noChange));
            Assert.IsFalse(noChange);

            // no change if a defaultsort already there
            Assert.AreEqual(@"[[Category:Parishes in Asturias]]
{{DEFAULTSORT:Bert}}",
                            Parsers.ChangeToDefaultSort(@"[[Category:Parishes in Asturias]]
{{DEFAULTSORT:Bert}}", "Abándames", out noChange));
            Assert.IsTrue(noChange);


            // category sortkeys are cleaned too
            Assert.AreEqual(@"[[Category:Parishes of the Azores]]
[[Category:São Miguel Island]]
{{DEFAULTSORT:Agua Retorta}}", Parsers.ChangeToDefaultSort(@"[[Category:Parishes of the Azores|Agua Retorta]]
[[Category:São Miguel Island]]", @"Água Retorta", out noChange));
            Assert.IsFalse(noChange);

            // use article name
            Assert.AreEqual(@"[[Category:Parishes of the Azores]]
[[Category:São Miguel Island]]
{{DEFAULTSORT:Agua Retorta}}", Parsers.ChangeToDefaultSort(@"[[Category:Parishes of the Azores]]
[[Category:São Miguel Island]]", @"Água Retorta", out noChange));
            Assert.IsFalse(noChange);
        }

        [Test]
        public void TestIsArticleAboutAPerson()
        {
            Assert.IsTrue(Parsers.IsArticleAboutAPerson(@"Foo {{persondata|name=smith}}", "foo"));
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
            Assert.IsTrue(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{RefimproveBLP}}", "foo"));
            Assert.IsTrue(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "16 and pregnant"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP unsourced section|foo=bar}}", "foo"));

            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo {{persondata|name=smith}}", "Category:foo"));
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
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo {{infobox some organization|foo=bar}} {{persondata|name=smith}}", "foo"));
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
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:X people]] {{foo-bio-stub}}", "foo"));
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

            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo {{Infobox settlement}} {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo {{italic title}} {{foo-bio-stub}}", "foo"));

            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Married couples]] {{persondata|name=smith}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:2002 animal births]] {{persondata|name=smith}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Comedy duos]] {{persondata|name=smith}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Comedy trios]] {{persondata|name=smith}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Foo Comedy duos]] {{persondata|name=smith}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo {{In-universe}} {{persondata|name=smith}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo {{in-universe}} {{persondata|name=smith}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo {{Infobox political party}} {{birth date and age|1974|11|26}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Articles about multiple people]] {{persondata|name=smith}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Fictional blah]] {{persondata|name=smith}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[fictional character]] {{persondata|name=smith}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[fictional character|character]] {{persondata|name=smith}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo {{dab}} {{persondata|name=smith}}", "foo"));
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
            Assert.IsTrue(Parsers.IsArticleAboutAPerson(@"Foo [[Category:2002 births]] {{Infobox Chinese-language singer and actor|name=A, B}}", "foo"));

            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo {{infobox person|name=smith}} Foo {{infobox person|name=smith2}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo {{death date|2002}} {{death date|2005}}", "foo"));

            // multiple different birth dates means not about one person
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"{{nat fs player|no=1|pos=GK|name=[[Meg]]|age={{Birth date|1956|01|01}} ({{Age at date|1956|01|01|1995|6|5}})|caps=|club=|clubnat=}}
{{nat fs player|no=2|pos=MF|name=[[Valeria]]|age={{Birth date|1968|09|03}} ({{Age at date|1968|09|03|1995|6|5}})|caps=|club=|clubnat=}}", "foo"));
            Assert.IsTrue(Parsers.IsArticleAboutAPerson(@"{{nat fs player|no=1|pos=GK|name=[[Meg]]|age={{Birth date|1956|01|01}} }} {{Birth date|1956|01|01}} {{Persondata}}", "foo"));

            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"{{see also|Fred}} Fred Smith is great == foo == {{persondata}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"{{Main|Fred}} Fred Smith is great == foo == {{persondata}}", "foo"));

            // link in bold in zeroth section to somewhere else is no good
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''military career of [[Napoleon Bonaparte]]''' == foo == {{birth date|2008|11|11}}", "foo"));

            // 'characters' category means fictional person
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"foo [[Category:227 characters]] {{persondata}}", "foo"));

            Assert.IsTrue(Parsers.IsArticleAboutAPerson(@"'''Margaret Sidney''' was the [[pseudonym]] of American author '''Harriett Mulford Stone''' (June 22, 1844–August 2, 1924).
[[Category:1844 births]]", "foo"));
            
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
            Assert.IsTrue(Parsers.IsArticleAboutAPerson(AR, "Opeti Fonua"));
            
        }

        [Test]
        public void ExternalURLToInternalLink()
        {
            Assert.AreEqual("", Parsers.ExternalURLToInternalLink(""));

            Assert.AreEqual("https://secure.wikimedia.org/otrs/index.pl?Action=AgentTicketQueue",
                            Parsers.ExternalURLToInternalLink(
                                "https://secure.wikimedia.org/otrs/index.pl?Action=AgentTicketQueue"));

            Assert.AreEqual("[[w:ru:Foo|Foo]]",
                            Parsers.ExternalURLToInternalLink("[http://ru.wikipedia.org/wiki/Foo Foo]"));

            Assert.AreEqual("[[m:Test|Test]]",
                            Parsers.ExternalURLToInternalLink("[http://meta.wikimedia.org/wiki/Test Test]"));
            Assert.AreEqual("[[commons:Test|Test]]",
                            Parsers.ExternalURLToInternalLink("[http://commons.wikimedia.org/wiki/Test Test]"));
        }

        [Test]
        public void ExternalURLToInternalLinkEn()
        {
            Assert.AreEqual("[[wikt:Test|Test]]",
                            Parsers.ExternalURLToInternalLink("[http://en.wiktionary.org/wiki/Test Test]"));
            Assert.AreEqual("[[wikt:Test|Test]]",
                            Parsers.ExternalURLToInternalLink("[http://en.wiktionary.org/w/Test Test]"));
            Assert.AreEqual("[[n:Test|Test]]",
                            Parsers.ExternalURLToInternalLink("[http://en.wikinews.org/wiki/Test Test]"));
            Assert.AreEqual("[[b:Test|Test]]",
                            Parsers.ExternalURLToInternalLink("[http://en.wikibooks.org/wiki/Test Test]"));
            Assert.AreEqual("[[q:Test|Test]]",
                            Parsers.ExternalURLToInternalLink("[http://en.wikiquote.org/wiki/Test Test]"));
            Assert.AreEqual("[[s:Test|Test]]",
                            Parsers.ExternalURLToInternalLink("[http://en.wikisource.org/wiki/Test Test]"));
            Assert.AreEqual("[[v:Test|Test]]",
                            Parsers.ExternalURLToInternalLink("[http://en.wikiversity.org/wiki/Test Test]"));
            Assert.AreEqual("[[w:Test|Test]]",
                            Parsers.ExternalURLToInternalLink("[http://en.wikipedia.org/wiki/Test Test]"));
            Assert.AreEqual("[[w:Test|Test]]",
                            Parsers.ExternalURLToInternalLink("[https://en.wikipedia.org/wiki/Test Test]"));

            Assert.AreEqual("[[wikt:fr:Test|Test]]",
                            Parsers.ExternalURLToInternalLink("[http://fr.wiktionary.org/wiki/Test Test]"));
            Assert.AreEqual("[[w:fr:Test|Test]]",
                            Parsers.ExternalURLToInternalLink("[http://fr.wikipedia.org/wiki/Test Test]"));
            Assert.AreEqual("[[w:fr:Test|Test]]",
                            Parsers.ExternalURLToInternalLink("[https://fr.wikipedia.org/wiki/Test Test]"));

#if DEBUG
            Variables.SetProjectLangCode("fr");
            Assert.AreEqual("[[w:en:Test|Test]]",
                            Parsers.ExternalURLToInternalLink("[http://en.wikipedia.org/wiki/Test Test]"));
            Assert.AreEqual("[[w:en:Test|Test]]",
                            Parsers.ExternalURLToInternalLink("[https://en.wikipedia.org/wiki/Test Test]"));
            Variables.SetProjectLangCode("en");
            Assert.AreEqual("[[w:Test|Test]]",
                            Parsers.ExternalURLToInternalLink("[http://en.wikipedia.org/wiki/Test Test]"));
#endif
        }

        [Test]
        public void RemoveEmptyComments()
        {
            Assert.AreEqual("", Parsers.RemoveEmptyComments("<!---->"));
            Assert.AreEqual("", Parsers.RemoveEmptyComments("<!-- -->"));

            // newline comments are used to split wikitext to lines w/o breaking formatting,
            // they should not be removed
            Assert.AreEqual("<!--\r\n\r\n-->", Parsers.RemoveEmptyComments("<!--\r\n\r\n-->"));

            Assert.AreEqual("", Parsers.RemoveEmptyComments("<!----><!---->"));
            Assert.AreEqual("<!--\r\n\r\n-->", Parsers.RemoveEmptyComments("<!--\r\n\r\n--><!---->"));
            Assert.AreEqual("<!--Test-->", Parsers.RemoveEmptyComments("<!----><!--Test-->"));
            Assert.AreEqual(" <!--Test-->", Parsers.RemoveEmptyComments("<!----> <!--Test-->"));
            Assert.AreEqual("<!--Test\r\nfoo--> <!--Test-->", Parsers.RemoveEmptyComments("<!--Test\r\nfoo--> <!--Test-->"));

            Assert.AreEqual("<!--Test-->", Parsers.RemoveEmptyComments("<!--Test-->"));

            Assert.AreEqual("", Parsers.RemoveEmptyComments(""));
            Assert.AreEqual("test", Parsers.RemoveEmptyComments("test"));
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
            Assert.IsTrue(Parsers.HasRefAfterReflist(@"blah <ref>a</ref> ==references== {{reflist}} {{GR|4}}"));
            Assert.IsTrue(Parsers.HasRefAfterReflist(@"blah <ref>a</ref>
==references== {{reflist}} <ref>b</ref>"));
            Assert.IsTrue(Parsers.HasRefAfterReflist(@"blah <ref>a</ref> ==references== {{reflist}} <ref name=""b"">b</ref>"));

            // this is correct syntax
            Assert.IsFalse(Parsers.HasRefAfterReflist(@"blah <ref>a</ref> ==references== {{reflist}}"));
            // ignores commented out refs
            Assert.IsFalse(Parsers.HasRefAfterReflist(@"blah <ref>a</ref> ==references== {{reflist}} <!--<ref>b</ref>-->"));

            // the second template means this is okay too
            Assert.IsFalse(Parsers.HasRefAfterReflist(@"blah <ref>a</ref> ==references== {{reflist}} <ref name=""b"">b</ref> {{reflist}}"));

            // 'r' in argument means no embedded <ref></ref>
            Assert.IsFalse(Parsers.HasRefAfterReflist(@"blah <ref>a</ref> ==references== {{reflist}} {{GR|r4}}"));
            Assert.IsFalse(Parsers.HasRefAfterReflist(@"blah <ref>a</ref> ==references== {{reflist}} {{GR|India}}"));
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
            
            Assert.IsTrue(Parsers.IsInUse("{{Σε χρήση}} Hello world"),"σε χρήση");
            Assert.IsTrue(Parsers.IsInUse("{{inuse}} Hello world"),"inuse");
            Assert.IsFalse(Parsers.IsInUse("{{goceinuse}} Hello world"),"goceinuse is en-only");

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
            
            Assert.IsTrue(Parsers.HasStubTemplate(@"foo {{بذرة ممثل}}"),"actor stub");
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
            Assert.AreEqual(@"{{foo}}", Parsers.GetTemplate(@"now {{foo}} was here", "foo"));
            Assert.AreEqual(@"{{Foo}}", Parsers.GetTemplate(@"now {{Foo}} was here", "foo"));
            Assert.AreEqual(@"{{foo}}", Parsers.GetTemplate(@"now {{foo}} was here", "Foo"));
            Assert.AreEqual(@"{{foo}}", Parsers.GetTemplate(@"now {{foo}} was here", "[Ff]oo"));
            Assert.AreEqual(@"{{ foo|bar asdfasdf}}", Parsers.GetTemplate(@"now {{ foo|bar asdfasdf}} was here", "foo"));
            Assert.AreEqual(@"{{ foo |bar asdfasdf}}", Parsers.GetTemplate(@"now {{ foo |bar asdfasdf}} was here", "foo"));
            Assert.AreEqual(@"{{ foo|bar
asdfasdf}}", Parsers.GetTemplate(@"now {{ foo|bar
asdfasdf}} was here", "foo"));
            Assert.AreEqual(@"{{foo}}", Parsers.GetTemplate(@"now {{foo}} was here {{foo|1}}", "foo"));

            Assert.AreEqual(@"", Parsers.GetTemplate(@"now {{ foo|bar asdfasdf}} was here", "foot"));
            Assert.AreEqual(@"", Parsers.GetTemplate(@"now {{ foo|bar asdfasdf}} was here", ""));
            Assert.AreEqual(@"", Parsers.GetTemplate(@"", "foo"));
            Assert.AreEqual(@"", Parsers.GetTemplate(@"now <!--{{foo}} --> was here", "foo"));
            Assert.AreEqual(@"{{foo<!--comm-->}}", Parsers.GetTemplate(@"now {{foo<!--comm-->}} was here", "foo"));

            Assert.AreEqual(@"{{foo  |a={{bar}} here}}", Parsers.GetTemplate(@"now {{foo  |a={{bar}} here}} was here", "foo"));
            Assert.AreEqual(@"{{foo|bar}}", Parsers.GetTemplate(@"now <!--{{foo|bar}}--> {{foo}} was here", "Foo"));
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

            Assert.AreEqual(fred.ToString(), Parsers.GetTemplates(text, "foo").ToString());
            Assert.AreEqual(fred.ToString(), Parsers.GetTemplates(text, "Foo").ToString());
            List<Match> templates = Parsers.GetTemplates(text, "foo");
            Assert.AreEqual(foo1, templates[0].Value);
            Assert.AreEqual(foo2, templates[1].Value);
            Assert.AreEqual(2, templates.Count);

            // ignores commeted out templates
            templates = Parsers.GetTemplates(text + @" <!-- {{foo|c}} -->", "foo");
            Assert.AreEqual(foo1, templates[0].Value);
            Assert.AreEqual(foo2, templates[1].Value);
            Assert.AreEqual(2, templates.Count);

            // ignores nowiki templates
            templates = Parsers.GetTemplates(text + @" <nowiki> {{foo|c}} </nowiki>", "foo");
            Assert.AreEqual(foo1, templates[0].Value);
            Assert.AreEqual(foo2, templates[1].Value);
            Assert.AreEqual(2, templates.Count);

            // nested templates caught
            const string foo3 = @"{{ Foo|bar={{abc}}|beer=y}}";
            templates = Parsers.GetTemplates(@"now " + foo3 + @" there", "foo");
            Assert.AreEqual(foo3, templates[0].Value);

            // whitespace ignored
            const string foo4 = @"{{ Foo }}";
            templates = Parsers.GetTemplates(@"now " + foo4 + @" there", "foo");
            Assert.AreEqual(foo4, templates[0].Value);

            // no matches here
            templates = Parsers.GetTemplates(@"now " + foo3 + @" there", "fo");
            Assert.AreEqual(0, templates.Count);

            templates = Parsers.GetTemplates(@"{{test}}", "test");
            Assert.AreEqual(1, templates.Count);

            templates = Parsers.GetTemplates(@"{{test}}
", "test");
            Assert.AreEqual(1, templates.Count);
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
            Assert.AreEqual(3, templates.Count);
            Assert.AreEqual(foo1, templates[0].Value);
            Assert.AreEqual(foo2, templates[1].Value);
            Assert.AreEqual(foo2a, templates[2].Value);

            templates = Parsers.GetTemplates(text + " space " + foo2b, "foo");
            Assert.AreEqual(3, templates.Count);
            Assert.AreEqual(foo1, templates[0].Value);
            Assert.AreEqual(foo2, templates[1].Value);
            Assert.AreEqual(foo2b, templates[2].Value);
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
            Assert.AreEqual(3, templates.Count);
            Assert.AreEqual(foo1, templates[0].Value);
            Assert.AreEqual(foo2, templates[1].Value);
            Assert.AreEqual(foo2a, templates[2].Value);

            templates = Parsers.GetTemplates(text + " space " + foo2b);
            Assert.AreEqual(3, templates.Count);
            Assert.AreEqual(foo1, templates[0].Value);
            Assert.AreEqual(foo2, templates[1].Value);
            Assert.AreEqual(foo2b, templates[2].Value);

            templates = Parsers.GetTemplates(@" {{one}} {{two}} {{three|a={{bcd}} |ef=gh}}");
            Assert.AreEqual(3, templates.Count);
        }

        [Test]
        public void FixUnicode()
        {
            // https://en.wikipedia.org/wiki/Wikipedia:AWB/B#Line_break_insertion

            Assert.AreEqual("foo bar", parser.FixUnicode("foo\x2028bar"));
            Assert.AreEqual(@"foo
bar", parser.FixUnicode("foo" + "\r\n" + "\x2028bar"));
            Assert.AreEqual("foo bar", parser.FixUnicode("foo\x2029bar"));
            Assert.AreEqual(@"foo
bar", parser.FixUnicode("foo" + "\r\n" + "\x200B\x200Bbar"));
        }

        [Test]
        public void SubstUserTemplates()
        {
            Regex Hello = new Regex(@"{{hello.*?}}");

            Assert.AreEqual(@"Text Expanded template test return<!-- {{hello|2010}} -->", Parsers.SubstUserTemplates(@"Text {{hello|2010}}", "test", Hello), "performs single substitution");
            Assert.AreEqual(@"Text Expanded template test return<!-- {{hello}} -->", Parsers.SubstUserTemplates(@"Text {{hello}}", "test", Hello), "performs single substitution");
            Assert.AreEqual(@"Text Expanded template test return<!-- {{hello}} -->
Expanded template test return<!-- {{hello2}} -->", Parsers.SubstUserTemplates(@"Text {{hello}}
{{hello2}}", "test", Hello), "performs multiple subsitutions");

            const string Bye = @"Text {{bye}}";
            Assert.AreEqual(Bye, Parsers.SubstUserTemplates(Bye, "test", Hello), "no changes if no matching template");

            const string Subst = @"Now {{{subst:bar}}} text";
            Assert.AreEqual(Subst, Parsers.SubstUserTemplates(Subst, "test", Hello), "doesn't change {{{subst");

            Regex None = null;
            Assert.AreEqual(Bye, Parsers.SubstUserTemplates(Bye, "test", None), "no changes when user talk page regex is null");

            const string T2 = @"Test {{{2|}}}";
            Assert.AreEqual("Test", Parsers.SubstUserTemplates(T2, "test", Hello), "cleans up the {{{2|}}} template");
        }

        [Test]
        public void FormatToBDA()
        {
            const string Correct = @"{{birth date and age|df=y|1990|05|11}}", CorrectAmerican = @"{{birth date and age|mf=y|1990|05|11}}";

            Assert.AreEqual(Correct, Parsers.FormatToBDA(@"11 May 1990 (age 21)"));
            Assert.AreEqual(Correct, Parsers.FormatToBDA(@"11 May 1990 (Age 21)"));
            Assert.AreEqual(Correct, Parsers.FormatToBDA(@"11 May 1990, (Age 21)"));
            Assert.AreEqual(Correct, Parsers.FormatToBDA(@"11 May 1990; (Age 21)"));
            Assert.AreEqual(Correct, Parsers.FormatToBDA(@"11 May 1990 (aged 21)"));
            Assert.AreEqual(Correct, Parsers.FormatToBDA(@"11 May [[1990]] (aged 21)"));
            Assert.AreEqual(Correct, Parsers.FormatToBDA(@"[[11 May]] [[1990]] (aged 21)"));
            Assert.AreEqual(Correct, Parsers.FormatToBDA(@"[[11 May]] 1990 (aged 21)"));
            Assert.AreEqual(Correct, Parsers.FormatToBDA(@"11th May 1990 (aged 21)"));
            Assert.AreEqual(Correct, Parsers.FormatToBDA(@"11 May 1990 ( aged 21 )"));
            Assert.AreEqual(Correct, Parsers.FormatToBDA(@"11 May 1990    ( aged 21 )"));
            Assert.AreEqual(Correct, Parsers.FormatToBDA(@"11 May, 1990    ( aged 21 )"));

            Assert.AreEqual(Correct, Parsers.FormatToBDA(@"1990-05-11 (age 21)"));
            Assert.AreEqual(Correct, Parsers.FormatToBDA(@"1990-5-11 (age 21)"));
            Assert.AreEqual(Correct, Parsers.FormatToBDA(@"1990-5-11 <br/>(age 21)"));
            Assert.AreEqual(Correct, Parsers.FormatToBDA(@"1990-5-11 <br>(age 21)"));
            Assert.AreEqual(Correct, Parsers.FormatToBDA(@"1990-5-11 <br>Age 21"));

            Assert.AreEqual(CorrectAmerican, Parsers.FormatToBDA(@"May 11, 1990 (age 21)"));
            Assert.AreEqual(CorrectAmerican, Parsers.FormatToBDA(@"May 11 1990 (age 21)"));
            Assert.AreEqual(CorrectAmerican, Parsers.FormatToBDA(@"May 11th 1990 (age 21)"));

            Assert.AreEqual("", Parsers.FormatToBDA(""));
            Assert.AreEqual("Test", Parsers.FormatToBDA("Test"));
            Assert.AreEqual("May 11, 1990", Parsers.FormatToBDA("May 11, 1990"));
            Assert.AreEqual("May 1990 (age 21)", Parsers.FormatToBDA("May 1990 (age 21)"));
            Assert.AreEqual("May 11, 1990 (Age 21) and some other text", Parsers.FormatToBDA("May 11, 1990 (Age 21) and some other text"));
        }

        [Test]
        public void COinS()
        {
            string data1 = @"""coins"": ""ctx_ver=Z39.88-2004&amp;rft_id=info%3Adoi%2Fhttp%3A%2F%2Fdx.doi.org%2F10.1007%2Fs11046-005-4332-4&amp;rfr_id=info%3Asid%2Fcrossref.org%3Asearch&amp;rft.atitle=Morphological+alterations+in+toxigenic+Aspergillus+parasiticus+exposed+to+neem+%28Azadirachta+indica%29+leaf+and+seed+aqueous+extracts&amp;rft.jtitle=Mycopathologia&amp;rft.date=2005&amp;rft.volume=159&amp;rft.issue=4&amp;rft.spage=565&amp;rft.epage=570&amp;rft.aufirst=Mehdi&amp;rft.aulast=Razzaghi-Abyaneh&amp;rft_val_fmt=info%3Aofi%2Ffmt%3Akev%3Amtx%3Ajournal&amp;rft.genre=article&amp;rft.au=Mehdi+Razzaghi-Abyaneh&amp;rft.au=+Abdolamir+Allameh&amp;rft.au=+Taki+Tiraihi&amp;rft.au=+Masoomeh+Shams-Ghahfarokhi&amp;rft.au=+Mehdi+Ghorbanian""";
            Dictionary<string, string> res = Parsers.ExtractCOinS(data1);
            Assert.IsTrue(res.ContainsKey("volume"));
            Assert.IsTrue(res.ContainsKey("issue"));
            Assert.IsTrue(res.ContainsKey("spage"));
            Assert.IsTrue(res.ContainsKey("aulast"));
            Assert.IsTrue(res.ContainsKey("atitle"));
            Assert.IsTrue(res.ContainsKey("date"));
            string v;
            res.TryGetValue("volume", out v);
            Assert.AreEqual(v, "159");
        }
    }
}
