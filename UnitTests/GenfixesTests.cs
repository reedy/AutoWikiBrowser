// This file is only for tests that require more than one transformation functions at the same time, so
//don't add tests for separate functions here

using WikiFunctions;
using NUnit.Framework;
using WikiFunctions.Parse;

namespace UnitTests
{
    public class GenfixesTestsBase : RequiresParser
    {
        private Article A = new Article("Test");
        private readonly HideText H = new HideText();
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
            Assert.AreEqual(text, ArticleText);
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
            AssertNotChanged("[[Image:foo.jpg|Some http://some_crap.com]]");

            AssertNotChanged("[[Image:foo.jpg|Some [http://some_crap.com]]]");

            AssertNotChanged("[[File:foo.jpg|Some [http://some_crap.com]]]");

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
        }

        [Test]
        public void BulletExternalLink()
        {
            AssertChange(@"==External link==
[http://www.site.com Foo]", @"==External links==
* [http://www.site.com Foo]");
        }

        [Test]
        public void DateRange()
        {
            AssertChange(@"over July 09-11, 2009", @"over July 9–11, 2009");
            AssertChange(@"{{Citation | title = Admiralty notice | date = March 1st – March 5th, 1747 | page = 1}}", @"{{Citation | title = Admiralty notice | date = March 1–5, 1747 | page = 1}}");
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
[[Category:1987 births]]","no persondata added");

			            AssertNotChanged(@"{{Unreferenced|date=December 2009}}
{{Dead end|date=November 2006}}
{{Notability|1=Music|date=September 2010}}
{{Advert|date=December 2007}}
'''Band''' is.

[[Category:Blues rock groups]]


{{Norway-band-stub}}","no multiple issues added");
			
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
        public void MultipleIssuesSectionAutoTemplates()
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

            Assert.AreEqual(correct, ArticleText);
            
            ArticleText = @"==External links==
[http://foo.com]
<br>
[http://foo2.com]";
            correct = @"==External links==
* [http://foo.com]
* [http://foo2.com]";
            
            GenFixes("Test");
            
            Assert.AreEqual(correct, ArticleText);
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
            
            Assert.IsTrue(ArticleText.Contains(@"[[Category:1990 births]]"),"birth category");
            Assert.IsTrue(ArticleText.Contains(@"[[Category:Living people]]"),"living people");
            Assert.IsTrue(ArticleText.Contains(@"{{DEFAULTSORT:Smith, John}}"),"human name defaultsort");
        }

        [Test]
        public void CatsAndDefaultSort2()
        {
            ArticleText = @"{{infobox person}}
'''Cecilia Uddén''' (born 11 April 1990) is great.";
            
            GenFixes("Cecilia Uddén");
            
            Assert.IsTrue(ArticleText.Contains(@"[[Category:1990 births]]"),"birth category");
            Assert.IsTrue(ArticleText.Contains(@"[[Category:Living people]]"),"living people");
            Assert.IsTrue(ArticleText.Contains(@"{{DEFAULTSORT:Udden, Cecilia}}"),"human name defaultsort without special characters");

            ArticleText = @"{{infobox person}}
'''İbrahim Smith''' (born 12 April 1991) is great.";

            GenFixes("İbrahim Smith");

            Assert.IsTrue(ArticleText.Contains(@"{{DEFAULTSORT:Ibrahim Smith}}"),"human name defaultsort (diacritics removed)");
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

             AssertChange(@"A<ref>{{cite book|last=Warnock|first=C |title=The Affair|year=1980|publisher=Pro |language=English}}</ref> which stood for son of the
company's founder<ref name=""Warnock 1980"">{{cite book|last=Warnock|first=C |title=The Affair|year=1980|publisher=Pro }}</ref>

==References==
{{reflist}}", @"A<ref name=""Warnock 1980""/> which stood for son of the
company's founder<ref name=""Warnock 1980"">{{cite book|last=Warnock|first=C |title=The Affair|year=1980|publisher=Pro }}</ref>

==References==
{{reflist}}");
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
            Assert.AreEqual(AllCommented, ArticleText, "no WikiProjectBannerShell addition when templates all commented out");

            string a = @"{{Talk header}}
{{WikiProjectBannerShell|1=
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
            Assert.AreEqual(a, ArticleText, "Adds WikiProjectBannerShell below talk header");

            a = @"{{Talk header}}
{{WikiProjectBannerShell|1=
{{WikiProject a |text}}
{{WikiProject b|text}}
{{WikiProject c|text}}
}}
";

            ArticleText = @"{{Talk header}}
{{WikiProject a |text}}
{{WikiProject b|text}}
{{WikiProject c|text}}";

            TalkGenFixes();
            Assert.AreEqual(a, ArticleText, "Adds WikiProjectBannerShell when 3 wikiproject links");

            ArticleText = @"{{Talk header}}
{{wikiProject a |text}}
{{wikiProject b|text}}
{{wiki project c|text}}";

            TalkGenFixes();
            Assert.AreEqual(a.Replace("WikiProject c", "wiki project c").Replace("WikiProject ", "wikiProject "), ArticleText, "Adds WikiProjectBannerShell when 3 wikiProject links, wikiproject name variatsions");
        }

        [Test]
        public void AddWikiProjectBannerShellWhitespace()
        {
            ArticleText = @"{{WikiProject Biography|living=yes}}
{{WikiProject a |text}}
{{WikiProject b|text}}
{{WikiProject c|text}}";

            string a = @"{{WikiProjectBannerShell|1=
{{WikiProject Biography|living=yes}}
{{WikiProject a |text}}
{{WikiProject b|text}}
{{WikiProject c|text}}
| blp=yes
}}
";

            TalkGenFixes();
            Assert.AreEqual(a, ArticleText, "Adds WikiProjectBannerShell when 3 wikiproject links, cleans whitespace");

             ArticleText = @"{{WikiProject Biography|living=yes}}
{{WikiProject a |text}}
{{WikiProject b|text}}
{{WikiProject c|text}}

==First==
Word.




Second.";

            a = @"{{WikiProjectBannerShell|1=
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
            Assert.AreEqual(a, ArticleText, "Adds WikiProjectBannerShell when 3 wikiproject links, cleans whitespace");
        }
    }
}
