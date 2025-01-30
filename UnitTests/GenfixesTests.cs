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
using NUnit.Framework.Legacy;
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
            Assert.That(ArticleText, Is.EqualTo(expected));
        }

        public void AssertNotChanged(string text)
        {
            ArticleText = text;
            GenFixes();
            Assert.That(ArticleText, Is.EqualTo(text));
        }

        public void AssertNotChanged(string text, string articleTitle)
        {
            ArticleText = text;
            GenFixes(articleTitle);
            Assert.That(ArticleText, Is.EqualTo(text), "unit test");
        }

        public void AssertNotChanged(string text, string articleTitle, string message)
        {
            ArticleText = text;
            GenFixes(articleTitle);
            Assert.That(ArticleText, Is.EqualTo(text), message);
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
            Assert.That(ArticleText, Is.EqualTo(@"#REDIRECT [[Action of 12-17 January 1640]] {{R from modification}}"));
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
            Assert.That(ArticleText, Does.StartWith("[[Image:foo.jpg|Some [http://some_crap.com]]]"));
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

            ClassicAssert.IsTrue(ArticleText.Contains(@"{{Multiple issues|
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

            Assert.That(ArticleText, Is.EqualTo(@"'''John Smith''' (born 1985) was great."));
        }

        [Test]
        public void FixCitationTemplates()
        {
            AssertNotChanged(@"* {{cite book |editor1-first=T |editor1-last=C |title=A |edition=E |publisher=M |page=7-124<!--HYPHEN PAGE--> }}", "Text", "No change to hyphen in page when comment");
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
            Assert.That(ArticleText, Is.EqualTo(correct));

            // RefsAfterPunctuation
            ArticleText = @"A<ref>ABC</REF>.

==References==
{{reflist}}";
            GenFixes("Test");
            Assert.That(ArticleText, Is.EqualTo(@"A.<ref>ABC</ref>

==References==
{{reflist}}"));

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
            ClassicAssert.IsTrue(ArticleText.Contains(@"<ref name=""XXL Mag"">") && ArticleText.Contains(@"<ref name=""XXL Mag""/>"), "Fix the ref quote errors, then merge them");
        }
        
        [Test]
        public void RefPunctuationSpacing()
        {
            ArticleText = @"Targ<ref>[http://www.site.com] Lubawskiego</ref>.It adopted {{reflist}}";
            
            GenFixes("Test");
            
            string correct = @"Targ.<ref>[http://www.site.com] Lubawskiego</ref> It adopted {{reflist}}";

            Assert.That(ArticleText, Is.EqualTo(correct));
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

            Assert.That(ArticleText, Is.EqualTo(correct), "References section exists");

            ArticleText = @"FOOBAR decreases<ref name=""G"">{{cite journal | author = M| title = R by p53: s}}</ref><ref name=""Bensaad""/>. It catalyses the removal of a phosphate group from fructose (F-2,6-BP)<ref name=""B""/><ref name=""G""/>:";

            GenFixes("Test");

            Assert.That(ArticleText, Is.EqualTo(correct), "References section does not exist");

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
            Assert.That(ar1.ArticleText, Is.EqualTo("'''Hello''' world text"));
        }

        [Test]
        public void OnlyGeneralFixesChanged()
        {
            HideText H = new HideText();
            MockSkipOptions S = new MockSkipOptions();
            Article ar1 = new Article("Hello", " '''Hello''' world text");
            ar1.PerformUniversalGeneralFixes();
            ar1.PerformGeneralFixes(parser, H, S, false, false, false);
            ClassicAssert.IsTrue(ar1.OnlyGeneralFixesChanged, "Universal genfixes made change");

            // Categorization and then universal genfix
            Article ar2 = new Article("Category:Hello", " Text [[Category:Foo]]");
            ar2.Categorisation((WikiFunctions.Options.CategorisationOptions)
                1, parser, false,
                "Foo",
                "Foo2", false);
            Assert.That(ar2.ArticleText, Is.EqualTo(" Text [[Category:Foo2]]"), "Category rename operation");
            ar2.PerformUniversalGeneralFixes();
            Assert.That(ar2.ArticleText, Is.EqualTo("Text [[Category:Foo2]]"), "Universal genfix trim");
            ar2.PerformGeneralFixes(parser, H, S, false, false, false);
            ClassicAssert.IsFalse(ar2.OnlyGeneralFixesChanged, "Categorisation did cause change");
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

            Assert.That(ArticleText, Is.EqualTo(correct), "Clean br at end of lists");

            ArticleText = @"==External links==
[http://foo.com]
<br>
[http://foo2.com]";
            correct = @"==External links==
* [http://foo.com]
* [http://foo2.com]";

            GenFixes("Test");

            Assert.That(ArticleText, Is.EqualTo(correct), "Clean br between list items");
        }

        [Test]
        public void BoldTitle()
        {
            ArticleText = @"
==Foo.(here)==
Foo.(here) is a bar While remaining upright may be the primary goal of beginning riders";
            GenFixes("Foo.(here)");
            Assert.That(ArticleText, Is.EqualTo("'''Foo.(here)''' is a bar While remaining upright may be the primary goal of beginning riders"));
        }

        [Test]
        public void RedirectsSimplifyLinks()
        {
            ArticleText = @"#REDIRECT[[foo|foo]]";
            GenFixes("test");

            Assert.That(ArticleText, Is.EqualTo(@"#REDIRECT[[foo]]"));
        }

        [Test]
        public void RedirectsDefaultsort()
        {
            ArticleText = @"#REDIRECT[[Foo]]
[[Category:One]]";
            GenFixes("Foé");

            Assert.That(ArticleText, Is.EqualTo(@"#REDIRECT[[Foo]]
[[Category:One]]
{{DEFAULTSORT:Foe}}"));
        }

        [Test]
        public void CatsAndDefaultSort()
        {
            ArticleText = @"{{infobox person}}
'''John Smith''' (born 11 April 1990) is great.";
            
            GenFixes("John Smith");
            
            ClassicAssert.IsTrue(ArticleText.Contains(@"[[Category:1990 births]]"), "birth category");
            ClassicAssert.IsTrue(ArticleText.Contains(@"[[Category:Living people]]"), "living people");
            ClassicAssert.IsTrue(ArticleText.Contains(@"{{DEFAULTSORT:Smith, John}}"), "human name defaultsort");
        }

        [Test]
        public void CatsAndDefaultSort2()
        {
            ArticleText = @"{{infobox person}}
'''Cecilia Uddén''' (born 11 April 1990) is great.";
            
            GenFixes("Cecilia Uddén");
            
            ClassicAssert.IsTrue(ArticleText.Contains(@"[[Category:1990 births]]"), "birth category");
            ClassicAssert.IsTrue(ArticleText.Contains(@"[[Category:Living people]]"), "living people");
            ClassicAssert.IsTrue(ArticleText.Contains(@"{{DEFAULTSORT:Udden, Cecilia}}"), "human name defaultsort without special characters");

            ArticleText = @"{{infobox person}}
'''İbrahim Smith''' (born 12 April 1991) is great.";

            GenFixes("İbrahim Smith");

            ClassicAssert.IsTrue(ArticleText.Contains(@"{{DEFAULTSORT:Ibrahim Smith}}"), "human name defaultsort (diacritics removed)");
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

            Assert.That(ArticleText, Is.EqualTo(@"Z<ref name = ""Smith63"">Smith (2004), p.&nbsp;63</ref> in all probability.<ref name=""Smith63""/> For.<ref name=""Smith63""/>

A.<ref name=""Smith63"">Smith (2004), p.&nbsp;63</ref>

God.<ref name=""Smith63""/>

==References==
{{reflist|2}}"));
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
            ArticleText = @"{{Multiple issues|{{More citations needed|date=July 2015}}{{BLP sources|date=July 2015}}}}

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

            ClassicAssert.IsFalse(ArticleText.Contains(@"{Multiple issues"));
            ClassicAssert.IsTrue(ArticleText.Contains(@"Category:Living people"));
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

            Assert.That(t, Is.EqualTo(ArticleText), "No change to unbalanced brackets in images, Chinese brackets");

            t = @"Foo

Other [[Image:A.png|Now [[foo]]]]
Bar";
            ArticleText = t;
            GenFixes();

            Assert.That(t, Is.EqualTo(ArticleText), "No change to unbalanced brackets in images, simple");

            t = @"Foo

Other [[Image:A.png|foo [[bar]] there]]
Bar";
            ArticleText = t;
            GenFixes();

            Assert.That(t, Is.EqualTo(ArticleText), "No change to unbalanced brackets in images, simple 2");

            t = @"[[File:Miguel.jpg|x120px|thumb|[[Miguel Enríquez (privateer)|Miguel Enríquez]]]][[File:Demetrio O'Daly.jpg|x120px|thumb|[[Demetrio O'Daly]]]] [[File:Antonio Valero Bernabe.gif|x120px|thumb|[[Antonio Valero de Bernabé]]]] [[File:Manuel Rojas drawing.jpg|x120px|thumb|[[Manuel Rojas (independence leader)|Manuel Rojas]]]] [[File:CIVILWAR,PRSOLDIERsmall2.jpg|x120px|thumb|[[Augusto Rodríguez (soldier)|Augusto Rodríguez]]]] [[File:General Juan Luis Rivera.jpg|x120px|thumb|[[Juan Ríus Rivera]]]] [[File:GenSemidei.jpg|x120px|thumb|[[José Semidei Rodríguez]]]]";

            ArticleText = t;
            GenFixes();

            Assert.That(t, Is.EqualTo(ArticleText), "No change to unbalanced brackets in images, 3");

            t = @"[[File:Mat.jpg|thumb|Same|link=https://en.wikipedia.org/wiki/File:Mat.jpg]]";

            ArticleText = t;
            GenFixes();

            Assert.That(t, Is.EqualTo(ArticleText), "No change to brackets/images, 4");
        }

        [Test]
        public void FixSyntaxBrackets()
        {
            string t = @"*[https://www.site.com [XXX<nowiki>]</nowiki> XXX]

{{s | title=[[Party (1931)|XXX]] | before=}}";
            ArticleText = t;
            GenFixes();

            Assert.That(t, Is.EqualTo(ArticleText), "No change to unbalanced brackets from nowiki");

            t = @"A <nowiki>[</nowiki>[[sub-creation]]] story.";

            ArticleText = t;
            GenFixes();

            Assert.That(t, Is.EqualTo(ArticleText), "No change to unbalanced brackets from nowiki, 2");
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

            Assert.That(ArticleText, Is.EqualTo(t), "No change: ReorderReferences not applied within en-wp genfixes");
        }

        [Test]
        public void FixUnbalancedBrackets()
        {
            const string unfixable = @"Symbolist style: [[he abandoned realism and steered his work towards a more mythical and aestheticizing, almost evasionist tone, as denoted in his decorative plafonds for the [[Cau Ferrat]] of [[Sitges]] in 1896 (''La Pintura'', ''La Poesía'', ''La Música'').<ref name="":19"">{{harvtxt|Hofstätter|1981|p=257}}</ref> With the beginning of the 20th century he moved more towards landscape painting, still with a certain symbolist stamp but with a greater tendency towards realism.<ref>{{harvtxt|Socías|1987|p=45}}</ref>
[[File:Alexandre_de_Riquer_-_Composition_with_winged_nymph_at_sunrise_-_Google_Art_Project.jpg|left|thumb|''Composition with winged nymph at sunrise'' (1887), by [[Alexandre de Riquer]], [[National Art Museum of Catalonia]], [[Barcelona]]]]
[[Alexandre de Riquer]] was a painter, engraver, decorator, illustrator and poster artist, as well as a poet and art theorist. He lived for a time in London, where he was influenced by preraphaelitism and the [[Arts & Crafts]] movement. He excelled especially in book illustration (''Crisantemes'', 1899; ''Anyoranses'', 1902) and in the design of [[Ex Libris (bookplate)|ex-libris]], a genre he raised to heights of great quality.<ref>{{harvtxt|Hofstätter|1981|pp=256–257}}</ref>
[[File:Somni_Joan_Brull.jpg|thumb|''Ensueño'' (1897) by [[Joan Brull]], National Museum of Art of Catalonia, Barcelona]]
[[Joan Llimona]], founder of the [[Cercle Artístic de Sant Lluc]], leaned towards a mysticism of strong religiosity, as denoted in his paintings for the dome of the camarín of the church of the [[Monastery of Montserrat]] (1898) or the murals of the dining room of the Recolons house in Barcelona (1905).<ref name="":19"" /> Trained at the [[Escola de la Llotja]], he furthered his studies in Italy for four years. His first works were of genre costumbrista, but by 1890 his painting focused on religion, with compositions that combine formal realism with the idealism of the subjects, with a style sometimes compared to [[Jean-François Millet|Millet]] and Puvis de Chavannes.<ref>{{harvtxt|Socías|1987|pp=39–41}}</ref>

==References==
{{reflist}}";
            ArticleText = unfixable;

            GenFixes();

            Assert.That(ArticleText, Is.EqualTo(unfixable), "no change to unfixable square brackets");
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
            Assert.That(ArticleText, Is.EqualTo(AllCommented), "no WikiProject banner shell addition when templates all commented out");

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
            Assert.That(ArticleText, Is.EqualTo(a), "Adds WikiProject banner shell below talk header");

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
            Assert.That(ArticleText, Is.EqualTo(a), "Adds WikiProject banner shell when 3 wikiproject links");

            ArticleText = @"{{Talk header}}
{{wikiProject a |text}}
{{wikiProject d|text}}
{{wiki project c|text}}";

            TalkGenFixes();
            Assert.That(ArticleText, Is.EqualTo(a.Replace("WikiProject c", "wiki project c").Replace("WikiProject a", "wikiProject a").Replace("WikiProject d", "wikiProject d")), "Adds WikiProject banner shell when 3 wikiProject links, wikiproject name variations");
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
            Assert.That(ArticleText, Is.EqualTo(a), "Adds WikiProject banner shell when 3 wikiproject links, cleans whitespace");

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
            Assert.That(ArticleText, Is.EqualTo(a), "Adds WikiProject banner shell when 3 wikiproject links, cleans whitespace");
        }
    }
}
