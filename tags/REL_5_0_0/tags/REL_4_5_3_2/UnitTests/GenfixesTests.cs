// This file is only for tests that require more than one transformation functions at the same time, so
//don't add tests for separate functions here

using WikiFunctions;
using NUnit.Framework;
using WikiFunctions.Parse;

namespace UnitTests
{
    public class GenfixesTestsBase : RequiresParser
    {
        private readonly Article A = new Article("Test");
        private readonly HideText H = new HideText();
        private readonly MockSkipOptions S = new MockSkipOptions();

        public void GenFixes(bool replaceReferenceTags)
        {
            A.PerformGeneralFixes(parser, H, S, replaceReferenceTags);
        }

        public void GenFixes()
        {
            GenFixes(true);
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
                A.AWBChangeArticleText("unit testing", value, true);
                A.OriginalArticleText = value;
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
        // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_2#Incorrect_Underscore_removal_in_URL.27s_in_wikilinks
        public void UndersoreRemovalInExternalLink()
        {
            // just in case...
            AssertNotChanged("testing http://some_link testing");

            AssertNotChanged("[http://some_link]");

            AssertChange("[[http://some_link]] testing", "[http://some_link] testing");
        }

        [Test]
        // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_2#Incorrect_Underscore_removal_in_URL.27s_in_wikilinks
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
        // this transformation is currently at Parsers.FixDates()
        public void DoubleBr()
        {
            AssertChange("a<br><br>b", "a\r\nb");
            AssertChange("a<br /><bR>b", "a\r\nb");
            AssertChange("a<BR> <Br/>b", "a\r\nb");
            AssertChange("<br><br>", ""); // \r\n removed as extra whitespace

            AssertNotChanged("a<br/br>b");
            AssertNotChanged("a<br/>\r\n<br>b");

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_6#General_fixes_problem:_br_tags_inside_templates
            AssertChange("{{foo|bar=a<br><br>b}}<br><br>quux", "{{foo|bar=a<br><br>b}}\r\nquux");

            AssertNotChanged("<blockquote>\r\n<br><br></blockquote>");

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_7#AWB_ruins_formatting_of_table_when_applying_general_clean_up_fixes
            AssertNotChanged("|a<br><br>b");
            AssertNotChanged("!a<br><br>b");
            AssertNotChanged("| foo || a<br><br>b");
            AssertNotChanged("! foo !! a<br><br>b");

            AssertChange("[[foo|bar]] a<br><br>b", "[[foo|bar]] a\r\nb");
            AssertChange("foo! a<br><br>b", "foo! a\r\nb");
        }

        [Test]
        public void TestParagraphFormatter()
        {
            AssertChange("<p>", ""); // trimmed by whitespace optimiser
            AssertChange("a</p>b", "a\r\n\r\nb");
            AssertChange("<p>a</p>b", "a\r\n\r\nb");
            AssertChange("a\r\n<p>b", "a\r\n\r\n\r\nb");

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_6#Clean_up_deformats_tables_removing_.3C_p_.3E_tags
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
    }
}
