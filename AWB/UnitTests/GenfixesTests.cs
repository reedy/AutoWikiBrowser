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
            A.PerformGeneralFixes(parser, H, S, replaceReferenceTags, true, false);
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
        }

        [Test]
        public void DateRange()
        {
            AssertChange(@"over July 09-11, 2009", @"over July 9–11, 2009");
        }

        [Test]
        // this transformation is currently at Parsers.FixDates()
        public void DoubleBr()
        {
            AssertChange("a<br><br>b", "a\r\nb");
            AssertChange("a<br /><bR>b", "a\r\nb");
            AssertChange("a<BR> <Br/>b", "a\r\nb");
            AssertChange("<br><br>", ""); // \r\n removed as extra whitespace

            AssertNotChanged("a<br/br>b");
            AssertNotChanged("a<br/>\r\n<br>b");

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_6#General_fixes_problem:_br_tags_inside_templates
            AssertChange("{{foo|bar=a<br><br>b}}<br><br>quux", "{{foo|bar=a<br><br>b}}\r\nquux");

            AssertNotChanged("<blockquote>\r\n<br><br></blockquote>");

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_7#AWB_ruins_formatting_of_table_when_applying_general_clean_up_fixes
            AssertNotChanged("|a<br><br>b");
            AssertNotChanged("!a<br><br>b");
            AssertNotChanged("| foo || a<br><br>b");
            AssertNotChanged("! foo !! a<br><br>b");

            AssertChange("[[foo|bar]] a<br><br>b", "[[foo|bar]] a\r\nb");
            AssertChange("foo! a<br><br>b", "foo! a\r\nb");

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
{{Orphan|date=November 2006}}
{{Notability|1=Music|date=September 2010}}
{{Advert|date=December 2007}}
'''Band''' is.

[[Category:Blues rock groups]]


{{Norway-band-stub}}", after = @"{{Multiple issues|unreferenced = December 2009|orphan = November 2006|advert = December 2007}}


{{Notability|1=Music|date=September 2010}}

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
        }
        
        [Test]
        public void EmboldenBorn()
        {
        	ArticleText = @"John Smith (1985-) was great.";
        	
        	GenFixes("John Smith");
        	
        	Assert.AreEqual(@"'''John Smith''' (born 1985) was great.", ArticleText);
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

        }
    }
}
