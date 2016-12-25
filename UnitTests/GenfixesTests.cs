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

// This file is only for tests that require more than one transformation functions at the same time, so
// Don't add tests for separate functions here

using NUnit.Framework;
using WikiFunctions;
using WikiFunctions.Parse;

namespace UnitTests
{
    public class GenfixesTestsBase : RequiresParser
    {
        private Article A = new Article("Test");
        private readonly HideText H = new HideText(false, true, true);
        private readonly MockSkipOptions S = new MockSkipOptions();

        public void GenFixes(bool replaceReferenceTags)
        {
            A.PerformGeneralFixes(parser, H, S, replaceReferenceTags, false, false);
        }

        public void GenFixes()
        {
            GenFixes(true);
        }

        public void GenFixes(string articleTitle)
        {
            A = new Article(articleTitle, ArticleText);
            GenFixes(true);
        }

        public void TalkGenFixes()
        {
            A.PerformTalkGeneralFixes(H);
        }

        public GenfixesTestsBase()
        {
            A.InitialiseLogListener();
            #if DEBUG
            Variables.SetProjectSimple("en", ProjectEnum.wikipedia);
            #endif
        }

        public string ArticleText
        {
            get { return A.ArticleText; }
            set
            {
                A = new Article(A.Name, value);
                A.InitialiseLogListener();
            }
        }

        public void AssertChange(string text, string expected)
        {
            ArticleText = text;
            GenFixes();
            Assert.AreEqual(expected, ArticleText);
        }

        public void AssertNotChanged(string text)
        {
            ArticleText = text;
            GenFixes();
            Assert.AreEqual(text, ArticleText);
        }

        public void AssertNotChanged(string text, string articleTitle)
        {
            ArticleText = text;
            GenFixes(articleTitle);
            Assert.AreEqual(text, ArticleText, "unit test");
        }

        public void AssertNotChanged(string text, string articleTitle, string message)
        {
            ArticleText = text;
            GenFixes(articleTitle);
            Assert.AreEqual(text, ArticleText, message);
        }
    }

    [TestFixture]
    public class GenfixesTests : GenfixesTestsBase
    {
        [Test]
        // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_2#Incorrect_Underscore_removal_in_URL.27s_in_wikilinks
        public void UndersoreRemovalInExternalLink()
        {
            // just in case...
            AssertNotChanged("testing http://some_link testing");

            AssertNotChanged("[http://some_link]");

            AssertChange("[[http://some_link]] testing", "[http://some_link] testing");
        }

        [Test]
        public void DablinksHTMLComments()
        {
            const string a1 = @"{{about||a|b}}", a2 = @"{{about||c|d}}";
            AssertNotChanged(@"<!--" + a1 + a2 + @"-->"); // no change to commented out tags
        }

        [Test]
        public void OnlyRedirectTaggerOnRedirects()
        {
            ArticleText = @"#REDIRECT [[Action of 12-17 January 1640]]";
            GenFixes("Action of 12–17 January 1640");
            Assert.AreEqual(ArticleText, @"#REDIRECT [[Action of 12-17 January 1640]] {{R from modification}}");
        }

        [Test]
        // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_2#Incorrect_Underscore_removal_in_URL.27s_in_wikilinks
        public void ExternalLinksInImageCaptions()
        {
            AssertNotChanged("[[Image:foo.jpg|Some http://some_crap.com]]", "Test", "Unlinked external link in image description");

            AssertNotChanged("[[Image:foo.jpg|Some [http://some_crap.com]]]", "Test", "Linked external link at end of image description");

            AssertNotChanged("[[File:foo.jpg|Some [http://some_crap.com]]]", "Test", "Linked external link at end of image description 2");

            ArticleText = "[[Image:foo.jpg|Some [[http://some_crap.com]]]]";
            GenFixes();
            // not performing a full comparison due to a bug that should be tested elsewhere
            StringAssert.StartsWith("[[Image:foo.jpg|Some [http://some_crap.com]]]", ArticleText);
        }

        [Test]
        // superset of LinkTests.TestFixLinkWhitespace() and others, tests in complex
        public void LinkWhitespace()
        {
            AssertChange("[[a ]]b", "[[a]] b");
            AssertChange("a[[ b]]", "a [[b]]");
            AssertChange("[[Foo bar|Foo_bar]]", "[[Foo bar]]");
            AssertChange(@"* [[ http://www.site.com/abcdef-abcdef-abcdef-abcdef-abcdef-abcdef-abcdef]]", @"* [http://www.site.com/abcdef-abcdef-abcdef-abcdef-abcdef-abcdef-abcdef]");
        }

        [Test]
        public void BulletExternalLink()
        {
            AssertChange(@"==External link==
[http://www.site.com Foo]", @"==External links==
* [http://www.site.com Foo]");

            AssertChange(@"==External link==
[http://www.site.com Foo]

[http://www.site2.com Foo2]", @"==External links==
* [http://www.site.com Foo]
* [http://www.site2.com Foo2]");

        }

        [Test]
        public void DateRange()
        {
            AssertChange(@"over July 09-11, 2009", @"over July 9–11, 2009");
            AssertChange(@"{{Citation | title = Admiralty notice | date = March 1st – March 5th, 1747 | page = 1}}", @"{{Citation | title = Admiralty notice | date = March 1–5, 1747 | page = 1}}");

            AssertNotChanged(@"First was won on 2007 February 7. 
In 2007 May 22 team has won gold medals.");
        }

        [Test]
        // this transformation is currently at Parsers.FixDates()
        public void DoubleBr()
        {
            AssertChange("a<br><br>b", "a\r\n\r\nb");
            AssertChange("a<br /><bR>b", "a\r\n\r\nb");
            AssertChange("a<BR> <Br/>b", "a\r\n\r\nb");
            AssertChange("<br><br>", ""); // \r\n removed as extra whitespace
            AssertChange("a<br/br>b", "a<br />b"); // incorrect single br tag

            AssertNotChanged("a<br/>\r\n<br>b");
            AssertChange("a\r\n<br/>\r\n<br>\r\nb", "a\r\n\r\nb");

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_6#General_fixes_problem:_br_tags_inside_templates
            AssertChange("{{foo|bar=a<br><br>b}}<br><br>quux", "{{foo|bar=a<br><br>b}}\r\n\r\nquux");

            AssertNotChanged("<blockquote>\r\n<br><br></blockquote>");

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_7#AWB_ruins_formatting_of_table_when_applying_general_clean_up_fixes
            AssertNotChanged("|a<br><br>b");
            AssertNotChanged("!a<br><br>b");
            AssertNotChanged("| foo || a<br><br>b");
            AssertNotChanged("! foo !! a<br><br>b");

            AssertChange("[[foo|bar]] a<br><br>b", "[[foo|bar]] a\r\n\r\nb");
            AssertChange("foo! a<br><br>b", "foo! a\r\n\r\nb");
            
            AssertChange("{{Orphan|date=September 2015}}<br>", "{{Orphan|date=September 2015}}");

            AssertChange(@"* {{Polish2|Krzepice (województwo dolnośląskie)|[[24 November]] [[2007]]}}<br><br>  a", @"* {{Polish2|Krzepice (województwo dolnośląskie)|[[24 November]] [[2007]]}}

a");
        }

        [Test]
        public void TestParagraphFormatter()
        {
            AssertChange("<p>", ""); // trimmed by whitespace optimiser
            AssertChange("a</p>b", "a\r\n\r\nb");
            AssertChange("<p>a</p>b", "a\r\n\r\nb");
            AssertChange("a\r\n<p>b", "a\r\n\r\n\r\nb");

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_6#Clean_up_deformats_tables_removing_.3C_p_.3E_tags
            AssertNotChanged("| a<p>b");
            AssertNotChanged("! a<p>b");
            AssertNotChanged("foo\r\n| a<p>b");
            AssertNotChanged("foo\r\n! a<p>b");
            AssertNotChanged("foo! a\r\n\r\nb");
            AssertNotChanged("{{foo|bar}} a\r\n\r\nb");
            AssertNotChanged("!<p>");
            AssertNotChanged("|<p>");

            AssertNotChanged("<blockquote>a<p>b</blockquote>");
            AssertNotChanged("<blockquote>\r\na<p>b\r\n</blockquote>");

            AssertNotChanged("{{cquote|a<p>b}}");
            AssertNotChanged("{{cquote|foo\r\na<p>b}}");
        }

        [Test]
        public void DontRenameImageToFile()
        {
            // Makes sure that Parsers.FixImages() is not readded to fixes unless it's fixed
            AssertNotChanged("[[Image:foo]]");
            AssertNotChanged("[[File:foo]]");
        }

        [Test]
        public void CiteTwoDateFixes()
        {
            AssertChange(@"{{cite web | url = http://www.census.gov/popest/geographic/boundary_changes/index.html | year=2010 |title = Boundary Changes | date = 2010-1-1 }}",
                         @"{{cite web | url = http://www.census.gov/popest/geographic/boundary_changes/index.html |title = Boundary Changes | date = 2010-01-01 }}");

            AssertChange(@"Over March 4th - March 10th, 2012 then.", @"Over March 4–10, 2012 then.");
            
            AssertChange(@"{{cite web| url=http://www.site.com/f | title=A | date= June 29th, 2012 08:44}}", 
                         @"{{cite web| url=http://www.site.com/f | title=A | date= June 29, 2012<!-- 08:44-->}}");
            AssertChange(@"{{cite web| url=http://www.site.com/f | title=A | date= 29 June, 2012| year=2012}}",
                         @"{{cite web| url=http://www.site.com/f | title=A | date= 29 June 2012}}");
        }

        [Test]
        public void CiteWorkLink()
        {
            AssertChange(@"{{cite news|work=''[[foo|foo]]''}}", @"{{cite news|work=[[foo]]}}");
        }
        
        [Test]
        public void UnbalancedBracketRenameTemplateParameter()
        {
            WikiRegexes.RenamedTemplateParameters.Clear();
            WikiRegexes.RenamedTemplateParameters = Parsers.LoadRenamedTemplateParameters(@"{{AWB rename template parameter|cite web|acccessdate|accessdate}}");
            
            AssertChange(@"A.<ref>{{cite web| url=http://www.site.com | title = ABC | acccessdate = 20 May 2012</ref> {{reflist}}", 
                         @"A.<ref>{{cite web| url=http://www.site.com | title = ABC | accessdate = 20 May 2012}}</ref> {{reflist}}");
        }

        [Test]
        public void UnbalancedBracketCite()
        {
            AssertChange(@"<ref>{{cite web}lastl=Levine|firstl=Syndey|title=Cannes15|url=http://blogs.i.com/s|website=I.com|accessdate=May 17, 2015}}</ref>
{{reflist}}",
                        @"<ref>{{cite web|lastl=Levine|firstl=Syndey|title=Cannes15|url=http://blogs.i.com/s|website=I.com|accessdate=May 17, 2015}}</ref>
{{reflist}}"); // fixes } that should be |

            AssertChange(@"<ref>{{cite web{lastl=Levine|firstl=Syndey|title=Cannes15|url=http://blogs.i.com/s|website=I.com|accessdate=May 17, 2015}}</ref>
{{reflist}}",
                        @"<ref>{{cite web|lastl=Levine|firstl=Syndey|title=Cannes15|url=http://blogs.i.com/s|website=I.com|accessdate=May 17, 2015}}</ref>
{{reflist}}"); // fixes { that should be |

            AssertChange(@"Foo<ref>a</ref>

==References==
((Reflist}}", @"Foo<ref>a</ref>

==References==
{{Reflist}}"); // ((template}} --> {{template}}

            AssertNotChanged(@"'''Peroni''' is<ref>{{cite web|url=http://www.assobirra.it|form.

==Nastro Azzurro==
W.<ref>[http://www.millerbrands.co.uk]. 0.</ref> T

==References==
{{reflist}}");

            AssertNotChanged(@"'''J''' [http://foo.com|title=2014] in [[W–S|A]].

{{DEFAULTSORT:P, A}}
[[Category:1960]]");
        }

        [Test]
        public void UnbalancedBracketsTemplateRedirects()
        {
            WikiRegexes.TemplateRedirects.Clear();
            WikiRegexes.TemplateRedirects = Parsers.LoadTemplateRedirects(@"{{tl|facts}} → {{tl|citation needed}}");

            AssertChange(@"Foo.{facts}}", @"Foo.{{citation needed}}");
        }


        [Test]
        public void Wikia()
        {
            #if DEBUG
            Variables.SetProjectSimple("en", ProjectEnum.wikia);

            AssertNotChanged(@"{{BLP sources|date=May 2010}}
'''Bob Jones''' (born 1987 in Smith).<ref>a</ref>

==References==
{{reflist}}

{{DEFAULTSORT:Jones, Bob}}
[[Category:Living people]]
[[Category:1987 births]]", "no persondata added");

            AssertNotChanged(@"{{Unreferenced|date=December 2009}}
{{Dead end|date=November 2006}}
{{Notability|1=Music|date=September 2010}}
{{Advert|date=December 2007}}
'''Band''' is.

[[Category:Blues rock groups]]


{{Norway-band-stub}}", "no multiple issues added");

        Variables.SetProjectSimple("en", ProjectEnum.wikipedia);
        #endif
        }

        [Test]
        public void CiteMissingClosingCurlyBracePairTable()
        {
            const string MissingClosingCurlyBracePair = @"'''Foo''' is great.

{|
||
||Foo<ref>{{cite web | title = foo2 | year=2010 | url=http://www.site.com </ref>
||
||x
||y
|}

==Refs==
{{reflist}}";

            AssertChange(MissingClosingCurlyBracePair, MissingClosingCurlyBracePair.Replace(@" </ref>", @"}}</ref>")); // mising braces fixed before cleaning double pipes within unclosed cite template
        }

        [Test]
        public void MultipleIssues()
        {
            string before = @"{{Unreferenced|auto=yes|date=December 2009}}
{{Underlinked|date=November 2006}}
{{Notability|1=Music|date=September 2010}}
{{Advert|date=December 2007}}
'''Band''' is.

[[Category:Blues rock groups]]


{{Norway-band-stub}}", after = @"{{Multiple issues|
{{Unreferenced|date=December 2009}}
{{Underlinked|date=November 2006}}
{{Notability|1=Music|date=September 2010}}
{{Advert|date=December 2007}}
}}




'''Band''' is.

[[Category:Blues rock groups]]


{{Norway-band-stub}}";
            AssertChange(before, after);


            ArticleText = @"{{POV|date=May 2016}}
{{NPOV|date=May 2016}}
{{Orphan|date=May 2016}}
{{Dead end|date=May 2016}}

Minor bug";

            WikiRegexes.TemplateRedirects.Clear();
            WikiRegexes.TemplateRedirects = Parsers.LoadTemplateRedirects(@"{{tl|NPOV}} → {{tl|POV}}");
            GenFixes("A");

            Assert.IsTrue(ArticleText.Contains(@"{{Multiple issues|
{{POV|date=May 2016}}
{{Orphan|date=May 2016}}
{{Dead end|date=May 2016}}
}}"), "No excess pipes left when MI incorporates duplicate tags");
        }

        [Test]
        public void HeadingWhitespace()
        {
            string foo = @"x

== Events ==
<onlyinclude>
=== By place ===
==== Roman Empire ====
* Emperor ", foo2 = @"x

== Events ==
<onlyinclude>

=== By place ===
==== Roman Empire ====
* Emperor";

            AssertChange(foo, foo2);
            
            const string HeadingWithOptionalSpace = @"x

== Events ==
y";
            AssertNotChanged(HeadingWithOptionalSpace);
            
            const string HeadingWithoutOptionalSpace = @"x

==Events==
y";
            AssertNotChanged(HeadingWithoutOptionalSpace);
        }
        
        [Test]
        public void EmboldenBorn()
        {
            ArticleText = @"John Smith (1985-) was great.";

            GenFixes("John Smith");

            Assert.AreEqual(@"'''John Smith''' (born 1985) was great.", ArticleText);
        }
        
        [Test]
        public void RefsPunctuationReorder()
        {
            #if DEBUG
            Variables.SetProjectSimple("en", ProjectEnum.wikisource); // as require en and don't run for en-wp
            ArticleText = @"* Andrew influences.<ref name=HerdFly>{{cite book |last=Herd |first=Andrew Dr |title=The Fly }}</ref>

* Hills  works<ref>{{cite book |last=McDonald }}</ref>,<ref>{{cite book |last=Gingrich }}</ref>,<ref>{{cite book |location=Norwalk, CT }}</ref>,<ref name=HerdFly/> {{Reflist}}";
            string correct = @"* Andrew influences.<ref name=HerdFly>{{cite book |last=Herd |first=Andrew Dr |title=The Fly }}</ref>

* Hills  works,<ref name=HerdFly/><ref>{{cite book |last=McDonald }}</ref><ref>{{cite book |last=Gingrich }}</ref><ref>{{cite book |location=Norwalk, CT }}</ref> {{Reflist}}";
            GenFixes("Test");
            Assert.AreEqual(correct, ArticleText);

            // RefsAfterPunctuation
            ArticleText = @"A<ref>ABC</REF>.

==References==
{{reflist}}";
            GenFixes("Test");
            Assert.AreEqual(@"A.<ref>ABC</ref>

==References==
{{reflist}}", ArticleText);

            Variables.SetProjectSimple("en", ProjectEnum.wikipedia);
            #endif
        }

        [Test]
        public void MergeRefsErrors()
        {
            ArticleText = @"A.<ref name=”XXL Mag”>{{cite web|url=http://www.somesite.com/online/?p=70413 |title=a}}</ref><ref name=c>x</ref>

B.<ref name=c />
C.<ref name=”XXL Mag”>{{cite web|url=http://www.somesite.com/online/?p=70413 |title=a}}</ref>

==References==
{{reflist}}";
            GenFixes("Test");
            Assert.IsTrue(ArticleText.Contains(@"<ref name=""XXL Mag"">") && ArticleText.Contains(@"<ref name=""XXL Mag""/>"), "Fix the ref quote errors, then merge them");
        }
        
        [Test]
        public void RefPunctuationSpacing()
        {
            ArticleText = @"Targ<ref>[http://www.site.com] Lubawskiego</ref>.It adopted {{reflist}}";
            
            GenFixes("Test");
            
            string correct = @"Targ.<ref>[http://www.site.com] Lubawskiego</ref> It adopted {{reflist}}";

            Assert.AreEqual(correct, ArticleText);
        }

        [Test]
        public void ReorderPunctuation()
        {
            #if DEBUG
            Variables.SetProjectSimple("en", ProjectEnum.wikisource);  // as require en and don't run for en-wp
            string correct = @"FOOBAR decreases.<ref name=""G"">{{cite journal | author = M| title = R by p53: s}}</ref><ref name=""Bensaad""/> It catalyses the removal of a phosphate group from fructose (F-2,6-BP):<ref name=""G""/><ref name=""B""/>

==References==
{{Reflist}}";

            ArticleText = @"FOOBAR decreases<ref name=""G"">{{cite journal | author = M| title = R by p53: s}}</ref><ref name=""Bensaad""/>. It catalyses the removal of a phosphate group from fructose (F-2,6-BP)<ref name=""B""/><ref name=""G""/>:

==References==
{{Reflist}}";
            
            GenFixes("Test");
            
            Assert.AreEqual(correct, ArticleText, "References section exists");

            ArticleText = @"FOOBAR decreases<ref name=""G"">{{cite journal | author = M| title = R by p53: s}}</ref><ref name=""Bensaad""/>. It catalyses the removal of a phosphate group from fructose (F-2,6-BP)<ref name=""B""/><ref name=""G""/>:";

            GenFixes("Test");
            
            Assert.AreEqual(correct, ArticleText, "References section does not exist");

            Variables.SetProjectSimple("en", ProjectEnum.wikipedia);
            #endif
        }

        [Test]
        public void PerformUniversalGeneralFixes()
        {
            HideText H = new HideText();
            MockSkipOptions S = new MockSkipOptions();
            Article ar1 = new Article("Hello", " '''Hello''' world text");
            ar1.PerformUniversalGeneralFixes();
            ar1.PerformGeneralFixes(parser, H, S, false, false, false);
            Assert.AreEqual("'''Hello''' world text", ar1.ArticleText);
        }

        [Test]
        public void ExternalLinksBr()
        {
            ArticleText = @"==External links==
[http://foo.com]</br>
[http://foo2.com]</br>
[[Category:A]]";

            GenFixes("Test");

            string correct = @"==External links==
* [http://foo.com]
* [http://foo2.com]
[[Category:A]]";

            Assert.AreEqual(correct, ArticleText, "Clean br at end of lists");

            ArticleText = @"==External links==
[http://foo.com]
<br>
[http://foo2.com]";
            correct = @"==External links==
* [http://foo.com]
* [http://foo2.com]";

            GenFixes("Test");

            Assert.AreEqual(correct, ArticleText, "Clean br between list items");
        }

        [Test]
        public void BoldTitle()
        {
            ArticleText = @"
==Foo.(here)==
Foo.(here) is a bar While remaining upright may be the primary goal of beginning riders";
            GenFixes("Foo.(here)");
            Assert.AreEqual("'''Foo.(here)''' is a bar While remaining upright may be the primary goal of beginning riders", ArticleText);
        }

        [Test]
        public void RedirectsSimplifyLinks()
        {
            ArticleText = @"#REDIRECT[[foo|foo]]";
            GenFixes("test");

            Assert.AreEqual(@"#REDIRECT[[foo]]", ArticleText);
        }

        [Test]
        public void RedirectsDefaultsort()
        {
            ArticleText = @"#REDIRECT[[Foo]]
[[Category:One]]";
            GenFixes("Foé");

            Assert.AreEqual(@"#REDIRECT[[Foo]]
[[Category:One]]
{{DEFAULTSORT:Foe}}", ArticleText);
        }

        [Test]
        public void CatsAndDefaultSort()
        {
            ArticleText = @"{{infobox person}}
'''John Smith''' (born 11 April 1990) is great.";
            
            GenFixes("John Smith");
            
            Assert.IsTrue(ArticleText.Contains(@"[[Category:1990 births]]"), "birth category");
            Assert.IsTrue(ArticleText.Contains(@"[[Category:Living people]]"), "living people");
            Assert.IsTrue(ArticleText.Contains(@"{{DEFAULTSORT:Smith, John}}"), "human name defaultsort");
        }

        [Test]
        public void CatsAndDefaultSort2()
        {
            ArticleText = @"{{infobox person}}
'''Cecilia Uddén''' (born 11 April 1990) is great.";
            
            GenFixes("Cecilia Uddén");
            
            Assert.IsTrue(ArticleText.Contains(@"[[Category:1990 births]]"), "birth category");
            Assert.IsTrue(ArticleText.Contains(@"[[Category:Living people]]"), "living people");
            Assert.IsTrue(ArticleText.Contains(@"{{DEFAULTSORT:Udden, Cecilia}}"), "human name defaultsort without special characters");

            ArticleText = @"{{infobox person}}
'''İbrahim Smith''' (born 12 April 1991) is great.";

            GenFixes("İbrahim Smith");

            Assert.IsTrue(ArticleText.Contains(@"{{DEFAULTSORT:Ibrahim Smith}}"), "human name defaultsort (diacritics removed)");
        }

        [Test]
        public void NamedAndUnnamedRefs()
        {
            ArticleText = @"Z<ref name = ""Smith63"">Smith (2004), p.&nbsp;63</ref> in all probability.<ref>Smith (2004), p.&nbsp;63</ref> For.<ref>Smith (2004), p.&nbsp;63</ref>

A.<ref name=""S63"">Smith (2004), p.&nbsp;63</ref>

God.<ref name=""S63"" />

==References==
{{reflist|2}}";

             GenFixes();

            Assert.AreEqual(@"Z<ref name = ""Smith63"">Smith (2004), p.&nbsp;63</ref> in all probability.<ref name=""Smith63""/> For.<ref name=""Smith63""/>

A.<ref name=""Smith63"">Smith (2004), p.&nbsp;63</ref>

God.<ref name=""Smith63""/>

==References==
{{reflist|2}}", ArticleText);
        }

        [Test]
        public void CiteUrl()
        {
            AssertNotChanged(@"*{{cite thesis |last=A |first=S |title=Categorizing |url=http://urn.fi/URN:ISBN:978-999-61-9999-2 |year=2010 }}");

            AssertChange(@"{{cite web | url = www.site.com/a.pdf | title = Something }}", @"{{cite web | url = http://www.site.com/a.pdf | title = Something }}");
            AssertChange(@"{{cite web | url = WWW.site.com/a.pdf | title = Something }}", @"{{cite web | url = http://WWW.site.com/a.pdf | title = Something }}");
        }

        [Test]
        public void LivingPeople()
        {
            ArticleText = @"{{Multiple issues|{{refimprove|date=July 2015}}{{BLP sources|date=July 2015}}}}

{{Infobox football biography
| name = Am
| birth_date  = 3 May 1983
}}

'''Amir'''  is.<ref>http://ca.soccerway.com/players/</ref>

==References==
<references/>

{{iran-footy-bio-stub}}

{{DEFAULTSORT:Amir}}
[[Category:A players]]";

            GenFixes();

            Assert.IsFalse(ArticleText.Contains(@"{Multiple issues"));
            Assert.IsTrue(ArticleText.Contains(@"Category:Living people"));
        }

        [Test]
        public void FixSyntaxImages()
        {
            string t = @"Foo
<Gallery>
File:T（p13 T2).jpg|
File:T（ｐ16_T3）.jpg|a
File:T（ｐ22 T5）.jpg|aa
</Gallery>

Bar";
            ArticleText = t;
            GenFixes();

            Assert.AreEqual(ArticleText, t, "No change to unbalanced brackets in images, Chinese brackets");

            t = @"Foo

Other [[Image:A.png|Now [[foo]]]]
Bar";
            ArticleText = t;
            GenFixes();

            Assert.AreEqual(ArticleText, t, "No change to unbalanced brackets in images, simple");

            t = @"Foo

Other [[Image:A.png|foo [[bar]] there]]
Bar";
            ArticleText = t;
            GenFixes();

            Assert.AreEqual(ArticleText, t, "No change to unbalanced brackets in images, simple 2");

            t = @"[[File:Miguel.jpg|x120px|thumb|[[Miguel Enríquez (privateer)|Miguel Enríquez]]]][[File:Demetrio O'Daly.jpg|x120px|thumb|[[Demetrio O'Daly]]]] [[File:Antonio Valero Bernabe.gif|x120px|thumb|[[Antonio Valero de Bernabé]]]] [[File:Manuel Rojas drawing.jpg|x120px|thumb|[[Manuel Rojas (independence leader)|Manuel Rojas]]]] [[File:CIVILWAR,PRSOLDIERsmall2.jpg|x120px|thumb|[[Augusto Rodríguez (soldier)|Augusto Rodríguez]]]] [[File:General Juan Luis Rivera.jpg|x120px|thumb|[[Juan Ríus Rivera]]]] [[File:GenSemidei.jpg|x120px|thumb|[[José Semidei Rodríguez]]]]";

            ArticleText = t;
            GenFixes();

            Assert.AreEqual(ArticleText, t, "No change to unbalanced brackets in images, 3");

            t = @"[[File:Mat.jpg|thumb|Same|link=https://en.wikipedia.org/wiki/File:Mat.jpg]]";

            ArticleText = t;
            GenFixes();

            Assert.AreEqual(ArticleText, t, "No change to brackets/images, 4");
        }

        [Test]
        public void ReorderReferencesNotEnWp()
        {
            const string t = @"'''Article''' is great.<ref name = ""Fred1"">So says Fred</ref>
Article started off pretty good,<ref>So says John</ref><ref name = ""Fred1"" /> and finished well.
End of.

==References
{{Reflist}}";
            ArticleText = t;
            GenFixes();

            Assert.AreEqual(ArticleText, t, "No change: ReorderReferences not applied within en-wp genfixes");
        }
    }

    [TestFixture]
    public class TalkGenfixesTests : GenfixesTestsBase
    {
        [Test]
        public void AddWikiProjectBannerShell()
        {
            const string AllCommented = @"<!-- {{WikiProject a|text}}
{{WikiProject b|text}}
{{WikiProject c|text}
{{WikiProject d|text}} -->";

            ArticleText = AllCommented;
            TalkGenFixes();
            Assert.AreEqual(AllCommented, ArticleText, "no WikiProject banner shell addition when templates all commented out");

            string a = @"{{Talk header}}
{{WikiProject banner shell|1=
{{WikiProject a |text}}
{{WikiProject b|text}}
{{WikiProject Biography|living=yes}}
{{WikiProject c|text}}
| blp=yes
}}
";

            ArticleText = @"{{Talk header}}
{{WikiProject a |text}}
{{WikiProject b|text}}
{{WikiProject Biography|living=yes}}
{{WikiProject c|text}}";

            TalkGenFixes();
            Assert.AreEqual(a, ArticleText, "Adds WikiProject banner shell below talk header");

            a = @"{{Talk header}}
{{WikiProject banner shell|1=
{{WikiProject a |text}}
{{WikiProject d|text}}
{{WikiProject c|text}}
}}
";

            ArticleText = @"{{Talk header}}
{{WikiProject a |text}}
{{WikiProject d|text}}
{{WikiProject c|text}}";

            TalkGenFixes();
            Assert.AreEqual(a, ArticleText, "Adds WikiProject banner shell when 3 wikiproject links");

            ArticleText = @"{{Talk header}}
{{wikiProject a |text}}
{{wikiProject d|text}}
{{wiki project c|text}}";

            TalkGenFixes();
            Assert.AreEqual(a.Replace("WikiProject c", "wiki project c").Replace("WikiProject a", "wikiProject a").Replace("WikiProject d", "wikiProject d"), ArticleText, "Adds WikiProject banner shell when 3 wikiProject links, wikiproject name variations");
        }

        [Test]
        public void AddWikiProjectBannerShellWhitespace()
        {
            ArticleText = @"{{WikiProject Biography|living=yes}}
{{WikiProject a |text}}
{{WikiProject b|text}}
{{WikiProject c|text}}";

            string a = @"{{WikiProject banner shell|1=
{{WikiProject Biography|living=yes}}
{{WikiProject a |text}}
{{WikiProject b|text}}
{{WikiProject c|text}}
| blp=yes
}}
";

            TalkGenFixes();
            Assert.AreEqual(a, ArticleText, "Adds WikiProject banner shell when 3 wikiproject links, cleans whitespace");

             ArticleText = @"{{WikiProject Biography|living=yes}}
{{WikiProject a |text}}
{{WikiProject b|text}}
{{WikiProject c|text}}

==First==
Word.




Second.";

            a = @"{{WikiProject banner shell|1=
{{WikiProject Biography|living=yes}}
{{WikiProject a |text}}
{{WikiProject b|text}}
{{WikiProject c|text}}
| blp=yes
}}

==First==
Word.




Second.";

            TalkGenFixes();
            Assert.AreEqual(a, ArticleText, "Adds WikiProject banner shell when 3 wikiproject links, cleans whitespace");
        }
    }
}
