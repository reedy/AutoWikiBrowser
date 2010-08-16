using System;
using System.Collections.Generic;
using NUnit.Framework;
using WikiFunctions;
using System.Text.RegularExpressions;
using WikiFunctions.Parse;
using System.Text;

namespace UnitTests
{
    [TestFixture]
    public class ToolsTests : RequiresInitialization
    {
        [Test]
        public void IsValidTitle()
        {
            Assert.IsTrue(Tools.IsValidTitle("test"));
            Assert.IsTrue(Tools.IsValidTitle("This is a_test"));
            Assert.IsTrue(Tools.IsValidTitle("123"));
            Assert.IsTrue(Tools.IsValidTitle("А & Б сидели на трубе! ة日?"));

            Assert.IsFalse(Tools.IsValidTitle(""), "Empty strings are not supposed to be valid titles");
            Assert.IsFalse(Tools.IsValidTitle(" "));
            Assert.IsFalse(Tools.IsValidTitle("%20"));
            Assert.IsFalse(Tools.IsValidTitle("_"));

            Assert.IsFalse(Tools.IsValidTitle("[xxx"));
            Assert.IsFalse(Tools.IsValidTitle("]abc"));
            Assert.IsFalse(Tools.IsValidTitle("{duh!"));
            Assert.IsFalse(Tools.IsValidTitle("}yoyo"));
            Assert.IsFalse(Tools.IsValidTitle("|pwn3d"));
            Assert.IsFalse(Tools.IsValidTitle("<1337"));
            Assert.IsFalse(Tools.IsValidTitle(">nooooo"));
            Assert.IsFalse(Tools.IsValidTitle("#yeee-hooo"));

            //Complex titles
            Assert.IsFalse(Tools.IsValidTitle("[test]#1"));
            Assert.IsFalse(Tools.IsValidTitle("_ _"), "Titles should be normalised before checking");
            Assert.IsTrue(Tools.IsValidTitle("http://www.wikipedia.org")); //unfortunately
            Assert.IsTrue(Tools.IsValidTitle("index.php/Viagra")); //even more unfortunately
            Assert.IsTrue(Tools.IsValidTitle("index.php?title=foobar"));

            Assert.IsFalse(Tools.IsValidTitle("::Foo"));
            Assert.IsFalse(Tools.IsValidTitle("User:"));
            Assert.IsFalse(Tools.IsValidTitle("User::"));
            Assert.IsFalse(Tools.IsValidTitle("User::Foo"));
            Assert.IsTrue(Tools.IsValidTitle(":Foo"));
            Assert.IsTrue(Tools.IsValidTitle(":User:Foo"));
        }

        [Test]
        public void RemoveInvalidChars()
        {
            Assert.AreEqual("tesT 123!", Tools.RemoveInvalidChars("tesT 123!"));
            Assert.AreEqual("тест, ёпта", Tools.RemoveInvalidChars("тест, ёпта"));
            Assert.AreEqual("", Tools.RemoveInvalidChars(""));
            Assert.AreEqual("test", Tools.RemoveInvalidChars("{<[test]>}"));
            Assert.AreEqual("", Tools.RemoveInvalidChars("#|#"));
            Assert.AreEqual("http://www.wikipedia.org", Tools.RemoveInvalidChars("http://www.wikipedia.org"));
        }

        [Test]
        public void RomanNumbers()
        {
            Assert.IsTrue(Tools.IsRomanNumber("XVII"));
            Assert.IsTrue(Tools.IsRomanNumber("I"));

            Assert.IsFalse(Tools.IsRomanNumber("xvii"));
            Assert.IsFalse(Tools.IsRomanNumber("XXXXXX"));
            Assert.IsFalse(Tools.IsRomanNumber("V II"));
            Assert.IsFalse(Tools.IsRomanNumber("AAA"));
            Assert.IsFalse(Tools.IsRomanNumber("123"));
            Assert.IsFalse(Tools.IsRomanNumber(" "));
            Assert.IsFalse(Tools.IsRomanNumber(""));
        }

        [Test]
        public void CaseInsensitive()
        {
            // standard cases
            Assert.AreEqual(@"[Aa]bc", Tools.CaseInsensitive("Abc"));
            Assert.AreEqual(@"[Aa]bc", Tools.CaseInsensitive("abc"));
            Assert.AreEqual(@"[Aa]BC", Tools.CaseInsensitive("aBC"));
            Assert.AreEqual(@"[Aa]bc[de]", Tools.CaseInsensitive("abc[de]"));
            
            // trimming
            Assert.AreEqual(@"[Aa]bc", Tools.CaseInsensitive("abc "));
            
            // no changes
            Assert.AreEqual(@" abc", Tools.CaseInsensitive(" abc"));
            Assert.AreEqual("", Tools.CaseInsensitive(""));
            Assert.AreEqual("123", Tools.CaseInsensitive("123"));
            Assert.AreEqual("-", Tools.CaseInsensitive("-"));
            Assert.AreEqual(@"[Aa]bc", Tools.CaseInsensitive(@"[Aa]bc"));

            Regex r = new Regex(Tools.CaseInsensitive("test"));
            Assert.IsTrue(r.IsMatch("test 123"));
            Assert.AreEqual("Test", r.Match("Test").Value);
            Assert.IsFalse(r.IsMatch("tEst"));

            r = new Regex(Tools.CaseInsensitive("Test"));
            Assert.IsTrue(r.IsMatch("test 123"));
            Assert.AreEqual("Test", r.Match("Test").Value);
            Assert.IsFalse(r.IsMatch("TEst"));

            r = new Regex(Tools.CaseInsensitive("#test#"));
            Assert.IsTrue(r.IsMatch("#test#"));
            Assert.IsFalse(r.IsMatch("#Test#"));
            Assert.IsFalse(r.IsMatch("test"));
        }

        [Test]
        public void AllCaseInsensitive()
        {
            Assert.AreEqual("", Tools.AllCaseInsensitive(""));
            Assert.AreEqual("123", Tools.AllCaseInsensitive("123"));
            Assert.AreEqual("-", Tools.AllCaseInsensitive("-"));

            Regex r = new Regex(Tools.AllCaseInsensitive("tEsT"));
            Assert.IsTrue(r.IsMatch("Test 123"));
            Assert.AreEqual("Test", r.Match("Test").Value);
            Assert.IsFalse(r.IsMatch("teZt"));

            r = new Regex(Tools.AllCaseInsensitive("[test}"));
            Assert.IsTrue(r.IsMatch("[test}"));
            Assert.IsTrue(r.IsMatch("[tEsT}"));
            Assert.IsFalse(r.IsMatch("test"));
        }

        [Test]
        public void CaseInsensitiveStringCompare()
        {
            Assert.IsTrue(Tools.CaseInsensitiveStringCompare("test", "test"));
            Assert.IsTrue(Tools.CaseInsensitiveStringCompare("test", "TEST"));
            Assert.IsTrue(Tools.CaseInsensitiveStringCompare("TEST", "TEST"));
            Assert.IsTrue(Tools.CaseInsensitiveStringCompare("testDING", "TESTding"));
            Assert.IsTrue(Tools.CaseInsensitiveStringCompare("sCr1pTkIdDy", "sCr1pTkIdDy"));

            Assert.IsFalse(Tools.CaseInsensitiveStringCompare("test", "not a test"));
            Assert.IsFalse(Tools.CaseInsensitiveStringCompare("test ", " test"));
        }

        [Test]
        public void TurnFirstToUpper()
        {
            Assert.AreEqual("", Tools.TurnFirstToUpper(""));
            Assert.AreEqual("ASDA", Tools.TurnFirstToUpper("ASDA"));
            Assert.AreEqual("ASDA", Tools.TurnFirstToUpper("aSDA"));
            Assert.AreEqual("Test", Tools.TurnFirstToUpper("test"));
            Assert.AreEqual("%test", Tools.TurnFirstToUpper("%test"));
            Assert.AreEqual("Ыыыы", Tools.TurnFirstToUpper("ыыыы"));
        }
        
        [Test]
        public void TurnFirstToUpperCapitalizeFirstLetter()
        {
            Variables.CapitalizeFirstLetter = false;
            Assert.AreEqual("test", Tools.TurnFirstToUpper("test"));
            Assert.AreEqual("Test", Tools.TurnFirstToUpper("Test"));

            Variables.CapitalizeFirstLetter = true;
            Assert.AreEqual("Test", Tools.TurnFirstToUpper("test"));
        }

        [Test]
        public void TurnFirstToLower()
        {
            Assert.AreEqual("", Tools.TurnFirstToLower(""));
            Assert.AreEqual("test", Tools.TurnFirstToLower("test"));
            Assert.AreEqual("%test", Tools.TurnFirstToLower("%test"));
            Assert.AreEqual("ыыыы", Tools.TurnFirstToLower("Ыыыы"));

            Assert.AreEqual("tEST", Tools.TurnFirstToLower("TEST"));
            Assert.AreEqual("test", Tools.TurnFirstToLower("Test"));
        }
        
        [Test]
        public void TitleCaseEN()
        {
            Assert.AreEqual(@"Foo", Tools.TitleCaseEN("FOO"));
            Assert.AreEqual(@"Mastermind's Interrogation", Tools.TitleCaseEN("mastermind's interrogation"));
            
            Assert.AreEqual(@"Foo", Tools.TitleCaseEN(" FOO "));
            Assert.AreEqual(@"Foo Bar", Tools.TitleCaseEN("FOO BAR"));
            Assert.AreEqual(@"Foo-Bar", Tools.TitleCaseEN("FOO-BAR"));
            Assert.AreEqual(@"Foo~Bar", Tools.TitleCaseEN("FOO~BAR"));
            Assert.AreEqual(@"Foo Bar", Tools.TitleCaseEN("foo bar"));
            Assert.AreEqual(@"Foo Bar", Tools.TitleCaseEN("foo Bar"));
            Assert.AreEqual(@"FOO BAR A", Tools.TitleCaseEN("FOO BAR a"), "Not all text uppercase");
            
            Assert.AreEqual(@"Jane E", Tools.TitleCaseEN("JANE E"), "doesn't reformat initials");
            Assert.AreEqual(@"Jane E.", Tools.TitleCaseEN("JANE E."), "doesn't reformat initials");
        }

        [Test]
        public void WordCount()
        {
            Assert.AreEqual(0, Tools.WordCount(""));
            Assert.AreEqual(0, Tools.WordCount("    "));
            Assert.AreEqual(0, Tools.WordCount("."));

            Assert.AreEqual(1, Tools.WordCount("1"));
            Assert.AreEqual(1, Tools.WordCount("  1 "));
            Assert.AreEqual(1, Tools.WordCount("foo"));
            Assert.AreEqual(2, Tools.WordCount("foo-bar"));
            Assert.AreEqual(2, Tools.WordCount("Превед медвед"));
            Assert.AreEqual(1, Tools.WordCount("123"));
            Assert.AreEqual(3, Tools.WordCount("foo\nbar\r\nboz"));
            Assert.AreEqual(2, Tools.WordCount("foo.bar"));

            Assert.AreEqual(1, Tools.WordCount("foo<!-- bar boz -->"));
            Assert.AreEqual(1, Tools.WordCount("foo<!--bar-->quux"));
            Assert.AreEqual(2, Tools.WordCount("foo <!--\r\nbar--> quux"));

            Assert.AreEqual(2, Tools.WordCount("foo {{template| bar boz box}} quux"));
            Assert.AreEqual(2, Tools.WordCount("foo{{template\r\n|boz = quux}}bar"));

            Assert.AreEqual(2, Tools.WordCount(@"foo
{|
! test !! test
|-
| test
| test || test
|-
|}bar"));

            Assert.AreEqual(2, Tools.WordCount(@"foo
{|
! test !! test
|-
| test
| test || test
|-
|}
bar"));

            Assert.AreEqual(1, Tools.WordCount(@"foo
{| class=""wikitable""
! test !! test
|- style=""color:red""
| test
| test || test
|-
|}"));
        }

        [Test]
        public void LineEndings()
        {
            Assert.AreEqual("", Tools.ConvertToLocalLineEndings(""));
            Assert.AreEqual("foo bar", Tools.ConvertToLocalLineEndings("foo bar"));
            Assert.AreEqual("\r\nfoo\r\nbar\r\n", Tools.ConvertToLocalLineEndings("\nfoo\nbar\n"));

            Assert.AreEqual("", Tools.ConvertFromLocalLineEndings(""));
            Assert.AreEqual("foo bar", Tools.ConvertFromLocalLineEndings("foo bar"));
            Assert.AreEqual("\nfoo\nbar\n", Tools.ConvertFromLocalLineEndings("\r\nfoo\r\nbar\r\n"));
        }

        [Test]
        public void ReplacePartOfString()
        {
            Assert.AreEqual("abc123ef", Tools.ReplacePartOfString("abcdef", 3, 1, "123"));
            Assert.AreEqual("123abc", Tools.ReplacePartOfString("abc", 0, 0, "123"));
            Assert.AreEqual("abc123", Tools.ReplacePartOfString("abc", 3, 0, "123"));
            Assert.AreEqual("123", Tools.ReplacePartOfString("", 0, 0, "123"));
            Assert.AreEqual("abc", Tools.ReplacePartOfString("abc", 1, 0, ""));
            Assert.AreEqual("123", Tools.ReplacePartOfString("abc", 0, 3, "123"));
            Assert.AreEqual("1bc", Tools.ReplacePartOfString("abc", 0, 1, "1"));
            Assert.AreEqual("ab3", Tools.ReplacePartOfString("abc", 2, 1, "3"));
        }

        // Helper function
        private string ReplaceOnceStringBuilder(string input, string oldValue, string newValue)
        {
            StringBuilder sb = new StringBuilder(input);
            Tools.ReplaceOnce(sb, oldValue, newValue);
            return sb.ToString();
        }

        // Helper function
        private string ReplaceOnceString(string input, string oldValue, string newValue)
        {
            Tools.ReplaceOnce(ref input, oldValue, newValue);
            return input;
        }

        [Test]
        public void ReplaceOnce()
        {
            Assert.AreEqual("", ReplaceOnceStringBuilder("", "foo", "bar"));
            Assert.AreEqual("test bar!", ReplaceOnceStringBuilder("test foo!", "foo", "bar"));
            Assert.AreEqual("foobar", ReplaceOnceStringBuilder("barbar", "bar", "foo"));

            Assert.AreEqual("", ReplaceOnceString("", "foo", "bar"));
            Assert.AreEqual("test bar!", ReplaceOnceString("test foo!", "foo", "bar"));
            Assert.AreEqual("foobar", ReplaceOnceString("barbar", "bar", "foo"));
        }

        [Test]
        public void BasePageName()
        {
            Assert.AreEqual("", Tools.BasePageName(""));
            Assert.AreEqual("Foo", Tools.BasePageName("Foo"));
            Assert.AreEqual("Foo", Tools.BasePageName("Project:Foo"));
            Assert.AreEqual("Foo", Tools.BasePageName("Foo/Bar"));
            Assert.AreEqual("Foo", Tools.BasePageName("Foo/Bar/Boz"));
            Assert.AreEqual("Foo", Tools.BasePageName("Project:Foo/Bar/Boz"));
        }

        [Test]
        public void SubPageName()
        {
            Assert.AreEqual("", Tools.SubPageName(""));
            Assert.AreEqual("Foo", Tools.SubPageName("Foo"));
            Assert.AreEqual("Bar", Tools.SubPageName("Foo/Bar"));
            Assert.AreEqual("Boz", Tools.SubPageName("Foo/Bar/Boz"));
            Assert.AreEqual("Foo", Tools.SubPageName("Project:Foo"));
            Assert.AreEqual("Bar", Tools.SubPageName("Image:Foo/Bar"));
        }

        [Test]
        public void ServerName()
        {
            Assert.AreEqual("foo", Tools.ServerName("http://foo"));
            Assert.AreEqual("foo", Tools.ServerName("http://foo/"));
            Assert.AreEqual("foo.bar.com", Tools.ServerName("http://foo.bar.com/path/script?a=foo/bar"));
        }

        [Test]
        public void WikiEncode()
        {
            Assert.AreEqual("foo", Tools.WikiEncode("foo"));
            Assert.AreEqual("Foo", Tools.WikiEncode("Foo"));
            Assert.AreEqual("foo_bar", Tools.WikiEncode("foo bar"));
            Assert.AreEqual("foo_bar", Tools.WikiEncode("foo_bar"));
            Assert.AreEqual("foo/bar", Tools.WikiEncode("foo/bar"));
            Assert.AreEqual("foo:bar", Tools.WikiEncode("foo:bar"));
            StringAssert.AreEqualIgnoringCase("Caf%C3%A9", Tools.WikiEncode("Café"));
            StringAssert.AreEqualIgnoringCase("%D1%82%D0%B5%D1%81%D1%82:%D1%82%D0%B5%D1%81%D1%82", Tools.WikiEncode("тест:тест"));

            Assert.AreEqual("foo%2bbar", Tools.WikiEncode("foo+bar"));
        }

        [Test]
        public void WikiDecode()
        {
            Assert.AreEqual("foo", Tools.WikiDecode("foo"));
            Assert.AreEqual("Foo", Tools.WikiDecode("Foo"));
            Assert.AreEqual("foo bar", Tools.WikiDecode("foo_bar"));
            Assert.AreEqual("foo bar", Tools.WikiDecode("foo bar"));
            Assert.AreEqual("foo/bar", Tools.WikiDecode("foo/bar"));
            Assert.AreEqual("foo:bar", Tools.WikiDecode("foo:bar"));

            Assert.AreEqual("foo+bar", Tools.WikiDecode("foo+bar"));
            Assert.AreEqual("foo+bar", Tools.WikiDecode("foo%2bbar"));
        }

        [Test]
        public void SplitToSections()
        {
            string[] sections = Tools.SplitToSections("foo\r\n==bar=\r\nboo\r\n\r\n= boz =\r\n==quux==");
            CollectionAssert.AreEqual(new[]
                                      {
                                          "foo\r\n",
                                          "==bar=\r\nboo\r\n\r\n",
                                          "= boz =\r\n",
                                          "==quux==\r\n"
                                      }, sections);

            sections = Tools.SplitToSections("==bar=\r\nboo\r\n\r\n= boz =\r\n==quux==");
            CollectionAssert.AreEqual(new[]
                                      {
                                          "==bar=\r\nboo\r\n\r\n",
                                          "= boz =\r\n",
                                          "==quux==\r\n"
                                      }, sections);

            sections = Tools.SplitToSections("\r\n==bar=\r\nboo\r\n\r\n= boz =\r\n==quux==");
            CollectionAssert.AreEqual(new[]
                                      {
                                          "\r\n",
                                          "==bar=\r\nboo\r\n\r\n",
                                          "= boz =\r\n",
                                          "==quux==\r\n"
                                      }, sections);

            sections = Tools.SplitToSections("");
            CollectionAssert.AreEqual(new[] { "\r\n" }, sections);

            sections = Tools.SplitToSections("==foo==");
            CollectionAssert.AreEqual(new[] { "==foo==\r\n" }, sections);
        }

        [Test]
        public void RemoveMatches()
        {
            MatchCollection matches = Regex.Matches("abc bce cde def", "[ce]");
            Assert.AreEqual("ab b d df", Tools.RemoveMatches("abc bce cde def", matches));

            matches = Regex.Matches("", "test");
            Assert.AreEqual("test", Tools.RemoveMatches("test", matches));
            Assert.AreEqual("abc", Tools.RemoveMatches("abc", matches));
            Assert.AreEqual("", Tools.RemoveMatches("", matches));

            matches = Regex.Matches("abc123", "(123|abc)");
            Assert.AreEqual("", Tools.RemoveMatches("abc123", matches));

            matches = Regex.Matches("test", "[Tt]est");
            Assert.AreEqual("", Tools.RemoveMatches("test", matches));
            
            List<Match> MatchesList = new List<Match>();
            
            Assert.AreEqual("test", Tools.RemoveMatches("test", MatchesList));
        }

        [Test]
        public void RemoveHashFromPageTitle()
        {
            Assert.AreEqual("ab c", Tools.RemoveHashFromPageTitle("ab c"));
            Assert.AreEqual("foo", Tools.RemoveHashFromPageTitle("foo#bar"));
            Assert.AreEqual("foo", Tools.RemoveHashFromPageTitle("foo##bar#"));
            Assert.AreEqual("foo", Tools.RemoveHashFromPageTitle("foo#"));
            Assert.AreEqual("", Tools.RemoveHashFromPageTitle("#"));
            Assert.AreEqual("", Tools.RemoveHashFromPageTitle(""));
        }

        [Test]
        public void SplitLines()
        {
            CollectionAssert.IsEmpty(Tools.SplitLines(""));

            string[] test = new[] { "foo" };
            CollectionAssert.AreEqual(test, Tools.SplitLines("foo"));
            CollectionAssert.AreEqual(test, Tools.SplitLines("foo\r"));
            CollectionAssert.AreEqual(test, Tools.SplitLines("foo\n"));
            CollectionAssert.AreEqual(test, Tools.SplitLines("foo\r\n"));

            test = new[] { "foo", "bar" };
            CollectionAssert.AreEqual(test, Tools.SplitLines("foo\r\nbar"));
            CollectionAssert.AreEqual(test, Tools.SplitLines("foo\rbar"));
            CollectionAssert.AreEqual(test, Tools.SplitLines("foo\rbar"));

            test = new[] { "" };
            CollectionAssert.AreEqual(test, Tools.SplitLines("\n"));
            CollectionAssert.AreEqual(test, Tools.SplitLines("\r\n"));
            CollectionAssert.AreEqual(test, Tools.SplitLines("\r"));

            test = new[] { "", "" };
            CollectionAssert.AreEqual(test, Tools.SplitLines("\n\n"));
            CollectionAssert.AreEqual(test, Tools.SplitLines("\r\n\r\n"));
            CollectionAssert.AreEqual(test, Tools.SplitLines("\r\r"));

            test = new[] { "", "foo", "", "bar" };
            CollectionAssert.AreEqual(test, Tools.SplitLines("\r\nfoo\r\n\r\nbar"));
            CollectionAssert.AreEqual(test, Tools.SplitLines("\rfoo\r\rbar"));
            CollectionAssert.AreEqual(test, Tools.SplitLines("\nfoo\n\nbar"));
        }

        [Test]
        public void FirstChars()
        {
            Assert.AreEqual("", Tools.FirstChars("", 0));
            Assert.AreEqual("", Tools.FirstChars("", 3));
            Assert.AreEqual("", Tools.FirstChars("123", 0));
            Assert.AreEqual("12", Tools.FirstChars("12", 2));
            Assert.AreEqual("12", Tools.FirstChars("12", 3));
            Assert.AreEqual("12", Tools.FirstChars("123", 2));
        }
        
        [Test]
        public void Newline()
        {
            Assert.AreEqual("\r\n" + "foo", Tools.Newline("foo"));
            Assert.AreEqual("\r\n" + "foo", Tools.Newline("foo", 1));
            Assert.AreEqual("\r\n\r\n" + "foo", Tools.Newline("foo", 2));
            
            Assert.AreEqual("", Tools.Newline(""));
            Assert.AreEqual("", Tools.Newline("", 2));
        }

        [Test]
        public void IsRedirect()
        {
            Assert.IsTrue(Tools.IsRedirect("#REDIRECT  [[Foo]]"));
            Assert.IsTrue(Tools.IsRedirect("#redirecT[[:Foo]]"));
            Assert.IsTrue(Tools.IsRedirect("should work!\r\n#REDIRECT [[Foo]]"));

            Assert.IsFalse(Tools.IsRedirect("#REDIRECT you to [[Hell]]"));
            Assert.IsFalse(Tools.IsRedirect("REDIRECT [[Foo]]"));

            // http://en.wikipedia.org/w/index.php?title=Middleton_Lake&diff=246079011&oldid=240299146
            Assert.IsTrue(Tools.IsRedirect("#REDIRECT:[[Foo]]"));
            Assert.IsTrue(Tools.IsRedirect("#REDIRECT : [[Foo]]"));

            Assert.IsFalse(Tools.IsRedirect("<nowiki>#REDIRECT  [[Foo]]</nowiki>"));
        }

        [Test]
        public void RedirectTarget()
        {
            Assert.AreEqual("Foo", Tools.RedirectTarget("#redirect [[Foo]]"));
            Assert.AreEqual("Foo", Tools.RedirectTarget("#REDIRECT[[Foo]]"));
            Assert.AreEqual("Foo bar", Tools.RedirectTarget("#redirect[[:Foo bar ]]"));
            Assert.AreEqual("Foo bar", Tools.RedirectTarget("#redirect[[ :  Foo bar ]]"));
            Assert.AreEqual("Foo", Tools.RedirectTarget("{{delete}}\r\n#redirect [[Foo]]"));

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_6#AWB_follows_piped_redirects_to_an_invalid_page_title
            Assert.AreEqual("Foo", Tools.RedirectTarget("#REDIRECT [[Foo|bar]]"));

            // URL-decode targets
            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_7#Problem_with_redirects
            Assert.AreEqual("Foo, bar", Tools.RedirectTarget("#REDIRECT[[Foo%2C_bar]]"));
            Assert.AreEqual("Хуй", Tools.RedirectTarget("#REDIRECT[[%D0%A5%D1%83%D0%B9]]"));

            // http://en.wikipedia.org/w/index.php?title=Middleton_Lake&diff=246079011&oldid=240299146
            Assert.AreEqual("Foo", Tools.RedirectTarget("#REDIRECT:[[Foo]]"));
            Assert.AreEqual("Foo", Tools.RedirectTarget("#REDIRECT : [[Foo]]"));

            Assert.AreEqual("Foo#bar", Tools.RedirectTarget("#REDIRECT [[Foo#bar]]"));

            Assert.AreEqual("", Tools.RedirectTarget("<nowiki>#REDIRECT  [[Foo]]</nowiki>"));
        }

        [Test]
        public void GetTitleFromURL()
        {
            Assert.AreEqual("foo bar", Tools.GetTitleFromURL("http://en.wikipedia.org/wiki/foo_bar"));
            Assert.AreEqual("Хуй", Tools.GetTitleFromURL("http://en.wikipedia.org/wiki/%D0%A5%D1%83%D0%B9"));
            Assert.AreEqual("foo", Tools.GetTitleFromURL("http://en.wikipedia.org/w/index.php?title=foo"));
            Assert.AreEqual("foo", Tools.GetTitleFromURL("http://en.wikipedia.org/w/index.php/foo"));

            // return null if there is something wrong
            Assert.IsNull(Tools.GetTitleFromURL(""));
            Assert.IsNull(Tools.GetTitleFromURL("foo"));
            Assert.IsNull(Tools.GetTitleFromURL("http://en.wikipedia.org"));
            Assert.IsNull(Tools.GetTitleFromURL("http://en.wikipedia.org/wiki/"));
            Assert.IsNull(Tools.GetTitleFromURL("http://en.wikipedia.org/w/index.php?title=foo&action=delete"));

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_7#list_entries_like:_Index.html.3Fcurid.3D16235168
            Assert.IsNull(Tools.GetTitleFromURL("http://en.wikipedia.org/wiki/index.html?curid=666"));
            Assert.IsNull(Tools.GetTitleFromURL("http://en.wikipedia.org/wiki/foo?action=delete"));
            Assert.IsNull(Tools.GetTitleFromURL("http://en.wikipedia.org/w/index.php?title=foo&action=delete"));
            Assert.IsNull(Tools.GetTitleFromURL("http://en.wikipedia.org/w/index.php/foo?action=bar"));
        }

        [Test]
        public void FirstDifference()
        {
            Assert.AreEqual(0, Tools.FirstDifference("a", "b"));
            Assert.AreEqual(0, Tools.FirstDifference("", "a"));
            Assert.AreEqual(0, Tools.FirstDifference("a", ""));

            Assert.AreEqual(1, Tools.FirstDifference("aa", "ab"));
            Assert.AreEqual(1, Tools.FirstDifference("ab", "aa"));
            Assert.AreEqual(3, Tools.FirstDifference("foo", "foobar"));
            Assert.AreEqual(3, Tools.FirstDifference("football", "foobar"));

            // beyond the end
            Assert.AreEqual(3, Tools.FirstDifference("foo", "foo"));
        }

        [Test]
        public void ApplyKeyWords()
        {
            //Test majority of Key Words except %%key%%
            Assert.AreEqual(@"AutoWikiBrowser/Sandbox
AutoWikiBrowser/Sandbox

AutoWikiBrowser
AutoWikiBrowser

Wikipedia talk
Wikipedia_talk

Sandbox
Sandbox

Wikipedia talk:AutoWikiBrowser/Sandbox
Wikipedia_talk:AutoWikiBrowser/Sandbox

Wikipedia talk:AutoWikiBrowser/Sandbox
Wikipedia talk",

                            Tools.ApplyKeyWords("Wikipedia talk:AutoWikiBrowser/Sandbox", @"%%pagename%%
%%pagenamee%%

%%basepagename%%
%%basepagenamee%%

%%namespace%%
%%namespacee%%

%%subpagename%%
%%subpagenamee%%

%%fullpagename%%
%%fullpagenamee%%

%%title%%
%%namespace%%"));

            //Date Stuff - disabled for now
            //            Assert.AreEqual(DateTime.Now.Day.ToString() + "\r\n" +DateTime.Now.ToString("MMM") + "\r\n" +DateTime.Now.Year.ToString(), Tools.ApplyKeyWords("n/a", @"{{CURRENTDAY}}
            //{{CURRENTMONTHNAME}}
            //{{CURRENTYEAR}}"));

            //Server Stuff
            Assert.AreEqual(@"http://en.wikipedia.org
/w
en.wikipedia.org", Tools.ApplyKeyWords("n/a", @"%%server%%
%%scriptpath%%
%%servername%%"));

            //%%key%%, Tools.MakeHumanCatKey() - Covered by HumanCatKeyTests

            Assert.AreEqual("", Tools.ApplyKeyWords("", ""));
            Assert.AreEqual("", Tools.ApplyKeyWords(@"%%foo%%", ""));
            Assert.AreEqual(@"%%foo%%", Tools.ApplyKeyWords("", @"%%foo%%"));
        }

        [Test]
        public void IsWikimediaProject()
        {
            Assert.IsTrue(Tools.IsWikimediaProject(ProjectEnum.wikipedia));
            Assert.IsTrue(Tools.IsWikimediaProject(ProjectEnum.commons));
            Assert.IsTrue(Tools.IsWikimediaProject(ProjectEnum.meta));
            Assert.IsTrue(Tools.IsWikimediaProject(ProjectEnum.species));
            Assert.IsTrue(Tools.IsWikimediaProject(ProjectEnum.wikibooks));
            Assert.IsTrue(Tools.IsWikimediaProject(ProjectEnum.wikinews));
            Assert.IsTrue(Tools.IsWikimediaProject(ProjectEnum.wikiquote));
            Assert.IsTrue(Tools.IsWikimediaProject(ProjectEnum.wikisource));
            Assert.IsTrue(Tools.IsWikimediaProject(ProjectEnum.wikiversity));
            Assert.IsTrue(Tools.IsWikimediaProject(ProjectEnum.wiktionary));
            Assert.IsTrue(Tools.IsWikimediaProject(ProjectEnum.mediawiki));

            Assert.IsFalse(Tools.IsWikimediaProject(ProjectEnum.custom));
            Assert.IsFalse(Tools.IsWikimediaProject(ProjectEnum.wikia));
        }

        [Test]
        public void StripNamespaceColon()
        {
            string s = Tools.StripNamespaceColon("User:");
            Assert.IsFalse(s.Contains(":"));

            s = Tools.StripNamespaceColon("Project:");
            Assert.IsFalse(s.Contains(":"));
        }

        [Test]
        public void RemoveNamespaceString()
        {
            Assert.AreEqual("Test", Tools.RemoveNamespaceString("Test"));
            Assert.AreEqual("Test", Tools.RemoveNamespaceString("User:Test"));
            Assert.AreEqual("Test", Tools.RemoveNamespaceString("User talk:Test"));
            Assert.AreEqual("Test", Tools.RemoveNamespaceString("Category:Test"));
        }

        [Test]
        public void GetNamespaceString()
        {
            Assert.AreEqual("", Tools.GetNamespaceString("Test"));
            Assert.AreEqual("User", Tools.GetNamespaceString("User:Test"));
            Assert.AreEqual("User talk", Tools.GetNamespaceString("User talk:Test"));
            Assert.AreEqual("Category", Tools.GetNamespaceString("Category:Test"));
            Assert.AreEqual("Help", Tools.GetNamespaceString("Help:Test"));
        }

        [Test]
        public void FilterSomeArticles()
        {
            List<Article> articles = new List<Article>(new[]
                                                       {
                                                           new Article("Test"), new Article("Commons:Test"),
                                                           new Article("MediaWiki:Test"),
                                                           new Article("MediaWiki talk: test"),
                                                           new Article("User talk:Test")
                                                       });

            List<Article> res = Tools.FilterSomeArticles(articles);

            Assert.AreEqual(2, res.Count);

            foreach (Article a in res)
            {
                Assert.IsFalse(a.Name.StartsWith("Commons:"));
                Assert.AreNotEqual(Namespace.MediaWiki, a.NameSpaceKey);
                Assert.AreNotEqual(Namespace.MediaWikiTalk, a.NameSpaceKey);
                Assert.IsTrue(a.NameSpaceKey >= Namespace.Article);
            }
        }

        [Test]
        public void ReplaceWithSpacesTests()
        {
            Regex foo = new Regex("foo");
            Assert.AreEqual("", Tools.ReplaceWithSpaces("", foo));
            Assert.AreEqual("   ", Tools.ReplaceWithSpaces("foo", foo));
            Assert.AreEqual("   bar   ", Tools.ReplaceWithSpaces("foobarfoo", foo));
        }
        
        [Test]
        public void ReplaceWithTests()
        {
            Regex foo = new Regex("foo");
            Assert.AreEqual("", Tools.ReplaceWith("", foo, '#'));
            Assert.AreEqual("###", Tools.ReplaceWith("foo", foo, '#'));
            Assert.AreEqual("###bar###", Tools.ReplaceWith("foobarfoo", foo, '#'));
        }

        [Test]
        public void HTMLListToWiki()
        {
            Assert.AreEqual(@"*Fred", Tools.HTMLListToWiki("Fred", "*"));
            Assert.AreEqual(@"*Fred
*Jones", Tools.HTMLListToWiki(@"Fred
Jones", "*"));
            Assert.AreEqual(@"*Fred", Tools.HTMLListToWiki("Fred<BR>", "*"));
            Assert.AreEqual(@"*Fred", Tools.HTMLListToWiki("Fred<br/>", "*"));
            Assert.AreEqual(@"#Fred", Tools.HTMLListToWiki("Fred", "#"));
            Assert.AreEqual(@"#Fred", Tools.HTMLListToWiki("<OL>Fred</OL>", "#"));
            Assert.AreEqual(@"#Fred", Tools.HTMLListToWiki("<li>Fred</li>", "#"));
            Assert.AreEqual(@"*Fred", Tools.HTMLListToWiki(":Fred", "*"));
            Assert.AreEqual(@"*Fred", Tools.HTMLListToWiki("*Fred", "*"));
            Assert.AreEqual(@"*Fred Smith [[Foo#bar]]", Tools.HTMLListToWiki("#Fred Smith [[Foo#bar]]", "*"));

            Assert.AreEqual(@"*Fred Smith:here", Tools.HTMLListToWiki("Fred Smith:here", "*"));

            Assert.AreEqual(@"* Fred
* Jones", Tools.HTMLListToWiki(@"1 Fred
2 Jones", "*"));

            Assert.AreEqual(@"* Fred
* Jones", Tools.HTMLListToWiki(@"11. Fred
12. Jones", "*"));

            Assert.AreEqual(@"* Fred
* Jones", Tools.HTMLListToWiki(@"(1) Fred
(2) Jones", "*"));

            Assert.AreEqual(@"* Fred
* Jones", Tools.HTMLListToWiki(@"1) Fred
2) Jones", "*"));

            Assert.AreEqual(@"* Fred
* Jones", Tools.HTMLListToWiki(@"(11) Fred
(12) Jones", "*"));

            Assert.AreEqual(@"* Fred
* Jones", Tools.HTMLListToWiki(@"(998) Fred
(999) Jones", "*"));

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#Text_deleted_by_.22convert_list_to.22_.22.2A_list.22
            Assert.AreEqual(@"*1980 Fred
*2004 Jones", Tools.HTMLListToWiki(@"1980 Fred
2004 Jones", "*"));
            // do not add list to blank lines/lines with just whitespace
            Assert.AreEqual(@"*Fred
*Tim
*John", Tools.HTMLListToWiki(@"Fred
Tim
John", "*"));

            Assert.AreEqual(@"*Fred
 
*Tim
 
*John", Tools.HTMLListToWiki(@"Fred
 
Tim
 
John", "*"));
        }

        [Test]
        public void RemoveSyntax()
        {
            Assert.AreEqual(@"", Tools.RemoveSyntax(@""));
            Assert.AreEqual(@"foo", Tools.RemoveSyntax(@"* foo"));
            Assert.AreEqual(@"foo", Tools.RemoveSyntax(@"# foo"));
            Assert.AreEqual(@"foo", Tools.RemoveSyntax(@":foo"));
            Assert.AreEqual(@"foo bar", Tools.RemoveSyntax(@"foo_bar"));
            Assert.AreEqual(@"foo bar:the great", Tools.RemoveSyntax(@"foo_bar:the great"));
            Assert.AreEqual(@"foo", Tools.RemoveSyntax(@"[foo]"));
            Assert.AreEqual(@"foo&bar", Tools.RemoveSyntax(@"foo&amp;bar"));
            Assert.AreEqual(@"http://site.com words", Tools.RemoveSyntax(@"* [http://site.com words]"));
            Assert.AreEqual(@"foo ""bar"" here", Tools.RemoveSyntax(@"foo &quot;bar&quot; here"));
            Assert.AreEqual(@"foobar", Tools.RemoveSyntax(@"foo�bar"));
            
            Assert.AreEqual(@"foo", Tools.RemoveSyntax(@"  * foo"));
        }

        [Test]
        public void StringBetween()
        {
            Assert.AreEqual("foo", Tools.StringBetween("now foo here", "now ", " here"));
            Assert.AreEqual(" foo", Tools.StringBetween("now foo here foo", "now", " here"));
            
            Assert.AreEqual("", Tools.StringBetween("now foo av", "now ", " here"));

            // returns shortest matching string
            Assert.AreEqual("foo", Tools.StringBetween("now foo here blah here", "now ", " here"));
            Assert.AreEqual("foo", Tools.StringBetween("now foo here now foo2 here", "now ", " here"));
        }

        [Test]
        public void RegexMatchCount()
        {
            Assert.AreEqual(1, Tools.RegexMatchCount("a", "a"));
            Assert.AreEqual(5, Tools.RegexMatchCount("\\w", "abcde"));
            Assert.AreEqual(4, Tools.RegexMatchCount("a", "aAAa", RegexOptions.IgnoreCase));
            Assert.AreEqual(2, Tools.RegexMatchCount("\\w+", "test case"));
        }

        [Test]
        public void InterwikiCount()
        {
            SiteMatrix.Languages = new List<string> { "de", "es", "fr", "it", "sv" };

            Assert.AreEqual(0, Tools.InterwikiCount(@"now [[foo]] was great"));
            Assert.AreEqual(0, Tools.LinkCount(@"now [[lol:foo]] was great"));

            Assert.AreEqual(1, Tools.InterwikiCount(@"now [[de:foo]] was great"));
            Assert.AreEqual(1, Tools.InterwikiCount(@"now [[de:foo]] was great [[aa:now]] here"));

            Assert.AreEqual(2, Tools.InterwikiCount(@"now [[de:foo]] was great [[aa:now]] here [[fr:bye]]"));
        }

        [Test]
        public void LinkCountTests()
        {
            Assert.AreEqual(0, Tools.LinkCount(@"foo"));
            Assert.AreEqual(0, Tools.LinkCount(@"[foo]"));
            Assert.AreEqual(1, Tools.LinkCount(@"[[foo]]"));
            Assert.AreEqual(2, Tools.LinkCount(@"[[foo]]s and [[barbie|bar]]"));
        }

        [Test]
        public void ConvertDate()
        {
            string iso = @"2009-06-11", iso2 = @"1890-07-04";
            Assert.AreEqual(@"11 June 2009", Tools.ConvertDate(iso, Parsers.DateLocale.International));
            Assert.AreEqual(@"June 11, 2009", Tools.ConvertDate(iso, Parsers.DateLocale.American));
            Assert.AreEqual(iso, Tools.ConvertDate(iso, Parsers.DateLocale.ISO));
            Assert.AreEqual(iso, Tools.ConvertDate(iso, Parsers.DateLocale.Undetermined));
            Assert.AreEqual(@"4 July 1890", Tools.ConvertDate(iso2, Parsers.DateLocale.International));

            // handles incorect format
            string wrong = @"foo";
            Assert.AreEqual(wrong, Tools.ConvertDate(wrong, Parsers.DateLocale.International));
            
            // supports other valid date formats
            Assert.AreEqual(@"11 June 2009", Tools.ConvertDate("11 June 2009", Parsers.DateLocale.International));
            Assert.AreEqual(@"11 June 2009", Tools.ConvertDate("June 11, 2009", Parsers.DateLocale.International));
        }
        
        [Test]
        public void ConvertDateEnOnly()
        {
            #if DEBUG         
            string iso2 = @"1890-07-04";
            
            Variables.SetProjectLangCode("fr");
            Assert.AreEqual(iso2, Tools.ConvertDate(iso2, Parsers.DateLocale.International));
            
            Variables.SetProjectLangCode("en");
            Assert.AreEqual(@"4 July 1890", Tools.ConvertDate(iso2, Parsers.DateLocale.International));
            #endif
        }

        [Test, SetCulture("ru-RU")]
        public void ConvertDateOtherCulture()
        {
            // if user's computer has non-en culture, ISOToENDate should still work
            string iso = @"2009-06-11";
            Assert.AreEqual(@"11 June 2009", Tools.ConvertDate(iso, Parsers.DateLocale.International));
        }        
        
        [Test]
        public void AppendParameterToTemplate()
        {
            Assert.AreEqual(@"", Tools.AppendParameterToTemplate("", "location", "London"));
            
            Assert.AreEqual(@"{{cite|title=abc | location=London}}", Tools.AppendParameterToTemplate(@"{{cite|title=abc}}", "location", "London"));
            Assert.AreEqual(@"{{cite|title=abc | location=}}", Tools.AppendParameterToTemplate(@"{{cite|title=abc}}", "location", ""));
            Assert.AreEqual(@"{{cite|title=abc | location=London}}", Tools.AppendParameterToTemplate(@"{{cite|title=abc }}", "location", "London"));
            Assert.AreEqual(@"{{cite|title=abc|last=a|first=b|date=2009-12-12 | location=London}}", Tools.AppendParameterToTemplate(@"{{cite|title=abc|last=a|first=b|date=2009-12-12 }}", "location", "London"), "no newlines in template");
            
            Assert.AreEqual(@"{{cite
|title=abc | location=London}}", Tools.AppendParameterToTemplate(@"{{cite
|title=abc }}", "location", "London"), "insufficient newlines – space used");
            
            Assert.AreEqual(@"{{cite
|title=abc
|date=2009-12-12 | location=London}}", Tools.AppendParameterToTemplate(@"{{cite
|title=abc
|date=2009-12-12 }}", "location", "London"), "insufficient newlines – space used");
            
            Assert.AreEqual(@"{{cite
|title=abc
|last=a
|first=b
|date=2009-12-12
| location=London}}", Tools.AppendParameterToTemplate(@"{{cite
|title=abc
|last=a
|first=b
|date=2009-12-12 }}", "location", "London"), "newline set when > 2 existing");
            
            Assert.AreEqual(@"{{cite
|title=abc
|last=a
|first=b
|date=2009-12-12
| location=London}}", Tools.AppendParameterToTemplate(@"{{cite
|title=abc
|last=a
|first=b
|date=2009-12-12        }}", "location", "London"), "existing template end spaces removed");
            
            Assert.AreEqual(@"{{cite
|title=abc
|last=a
|first=b
|date=2009-12-12
| location=London
}}", Tools.AppendParameterToTemplate(@"{{cite
|title=abc
|last=a
|first=b
|date=2009-12-12
}}", "location", "London"), "template end on blank line");
        }
        
        [Test]
        public void GetTemplateParameterValue()
        {
            Assert.AreEqual("here", Tools.GetTemplateParameterValue(@"{{cite|param1=here}}", "param1"));
            Assert.AreEqual("here", Tools.GetTemplateParameterValue(@"{{cite|param1 = here }}", "param1"));
            Assert.AreEqual("here", Tools.GetTemplateParameterValue(@"{{cite
| param1=here
|param=there}}", "param1"));
            Assert.AreEqual("here", Tools.GetTemplateParameterValue(@"{{cite|param1=here|foo=bar}}", "param1"));
            Assert.AreEqual("[[1892]]-[[1893]]", Tools.GetTemplateParameterValue(@"{{cite|param1 = [[1892]]-[[1893]]}}", "param1"));
            
            Assert.AreEqual("here {{t1|foo|bar}} there", Tools.GetTemplateParameterValue(@"{{cite|param1=here {{t1|foo|bar}} there|foo=bar}}", "param1"));
            Assert.AreEqual("here <!--comm|bar--> there", Tools.GetTemplateParameterValue(@"{{cite|param1=here <!--comm|bar--> there|foo=bar}}", "param1"));
            
            Assert.AreEqual(@"here [[piped|link]]", Tools.GetTemplateParameterValue(@"{{cite|param1 = here [[piped|link]]}}", "param1"));
            Assert.AreEqual(@"[[piped|link]], [[another|piped link]] here", Tools.GetTemplateParameterValue(@"{{cite|param1 = [[piped|link]], [[another|piped link]] here}}", "param1"));
            
            // not found
            Assert.AreEqual("", Tools.GetTemplateParameterValue(@"{{cite|param2=here}}", "param1"));
            
            Assert.AreEqual("", Tools.GetTemplateParameterValue(@"{{cite|param1 {{test}} =here}}", "param1"));
            Assert.AreEqual("", Tools.GetTemplateParameterValue(@"{{cite|param1 [[test|2]] =here}}", "param1"));
            Assert.AreEqual("here", Tools.GetTemplateParameterValue(@"{{cite|param1 <!--comm--> =here}}", "param1"));
            
            // null value
            Assert.AreEqual("", Tools.GetTemplateParameterValue(@"{{cite|param1= }}", "param1"));
            
            // comments handled
            Assert.AreEqual("", Tools.GetTemplateParameterValue(@"{{cite|<!--param1=foo-->other=yes }}", "param1"));
            Assert.AreEqual("yes", Tools.GetTemplateParameterValue(@"{{cite|<!--param1=foo-->other=yes }}", "other"));
            Assert.AreEqual("yes", Tools.GetTemplateParameterValue(@"{{cite|other<!--param1=foo-->=yes }}", "other"));
            
            Assert.AreEqual("[[1892]]-[[1893]]", Tools.GetTemplateParameterValue(@"{{cite|param1<!--comm--> = [[1892]]-[[1893]]}}", "param1"));
            Assert.AreEqual("[[1892]]-[[1893]]", Tools.GetTemplateParameterValue(@"{{cite|<!--comm-->param1 = [[1892]]-[[1893]]}}", "param1"));
            
            // returns first value
            Assert.AreEqual("here", Tools.GetTemplateParameterValue(@"{{cite|param1=here|foo=bar|param1=there}}", "param1"));
            
            // case insensitive option
            Assert.AreEqual("here", Tools.GetTemplateParameterValue(@"{{cite|PARAM1=here}}", "param1", true));
        }
        
        [Test]
        public void GetTemplateParameterValueAdvancedCases()
        {
            Assert.AreEqual(@"here {{foo}}", Tools.GetTemplateParameterValue(@"{{cite|param1 = here {{foo}}}}", "param1"));
            Assert.AreEqual(@"here {{foo|bar}}", Tools.GetTemplateParameterValue(@"{{cite|param1 = here {{foo|bar}}}}", "param1"));
            Assert.AreEqual(@"here <!--foo|bar-->", Tools.GetTemplateParameterValue(@"{{cite|param1 = here <!--foo|bar-->}}", "param1"));
            Assert.AreEqual(@"[[File:abc.png|asdf|dsfjk|a]]", Tools.GetTemplateParameterValue(@"{{cite|param1 = [[File:abc.png|asdf|dsfjk|a]] }}", "param1"));
            Assert.AreEqual(@"[[File:abc.png|asdf|dsfjk|a]]", Tools.GetTemplateParameterValue(@"{{cite|param1 = [[File:abc.png|asdf|dsfjk|a]]
}}", "param1"));
            Assert.AreEqual(@"[[File:abc.png|asdf|dsfjk|a]]", Tools.GetTemplateParameterValue(@"{{cite|param1 = [[File:abc.png|asdf|dsfjk|a]]
|param2=other}}", "param1"));
            Assert.AreEqual(@"here <nowiki>|</nowiki> there", Tools.GetTemplateParameterValue(@"{{cite|param1 = here <nowiki>|</nowiki> there}}", "param1"));
            Assert.AreEqual(@"here <nowiki>|</nowiki> there", Tools.GetTemplateParameterValue(@"{{cite|param1 = here <nowiki>|</nowiki> there|parae=aaa}}", "param1"));
            
            string ItFilm = @"{{Film
|titoloitaliano= Matrix
|immagine= [[image:Matrix.png|200px|Il codice di Matrix]]
|didascalia= Il codice di Matrix
|titolooriginale= The Matrix
|nomepaese= [[Stati Uniti d'America|USA]]
|annoproduzione= [[1999]]
|durata= 136 min
|tipocolore= colore
|tipoaudio= sonoro}}";
            
            Assert.AreEqual(@"[[1999]]", Tools.GetTemplateParameterValue(ItFilm, "annoproduzione"));
            Assert.AreEqual(@"[[1999]", Tools.GetTemplateParameterValue(ItFilm.Replace(@"[[1999]]", @"[[1999]"), "annoproduzione"));
            Assert.AreEqual(@"[[1999", Tools.GetTemplateParameterValue(ItFilm.Replace(@"[[1999]]", @"[[1999"), "annoproduzione"));
            Assert.AreEqual(@"1999]]", Tools.GetTemplateParameterValue(ItFilm.Replace(@"[[1999]]", @"1999]]"), "annoproduzione"));
        }
        
        [Test]
        public void GetTemplateParametersValues()
        {
            List<string> returned = new List<string>();
            
            List<string> parameters = new List<string>();
            
            string template = @"{{cite web| title=abc |date=1 May 2009 | author=Smith, Ed | work=Times | location=Here }}";
            
            Assert.AreEqual(returned, Tools.GetTemplateParametersValues(template, parameters));
            
            Assert.IsTrue(returned.Count.Equals(0));
            
            parameters.Add("title");
            returned.Add("abc");
            
            Assert.AreEqual(returned, Tools.GetTemplateParametersValues(template, parameters));
            
            Assert.IsTrue(returned.Count.Equals(1));
            
            parameters.Add("date");
            returned.Add("1 May 2009");
            
            Assert.AreEqual(returned, Tools.GetTemplateParametersValues(template, parameters));
            
            Assert.IsTrue(returned.Count.Equals(2));
            
            parameters.Add("page");
            returned.Add("");
            
            Assert.AreEqual(returned, Tools.GetTemplateParametersValues(template, parameters));
            
            Assert.IsTrue(returned.Count.Equals(3), "zero length string added to return list if parameter not found");
            
            Assert.AreEqual(string.Join("", returned.ToArray()), "abc1 May 2009");
        }
        
        [Test]
        public void GetTemplateArgument()
        {
            Assert.AreEqual("foo", Tools.GetTemplateArgument(@"{{abc|foo}}", 1));
            Assert.AreEqual("foo", Tools.GetTemplateArgument(@"{{abc| foo}}", 1));
            Assert.AreEqual("foo", Tools.GetTemplateArgument(@"{{abc |  foo  }}", 1));            
            Assert.AreEqual("foo", Tools.GetTemplateArgument(@"{{abc |  foo  |bar}}", 1));
            
            Assert.AreEqual("bar", Tools.GetTemplateArgument(@"{{abc |  foo  |bar }}", 2));
            Assert.AreEqual("", Tools.GetTemplateArgument(@"{{abc ||  foo  |bar }}", 1));
            Assert.AreEqual("bar [[piped|link]]", Tools.GetTemplateArgument(@"{{abc |  foo  |bar [[piped|link]] }}", 2));
        }
        
        [Test]
        public void GetTemplateArgumentIndex()
        {
            Assert.AreEqual(6, Tools.GetTemplateArgumentIndex("{{abc|foo=yes}}", 1));
            Assert.AreEqual(6, Tools.GetTemplateArgumentIndex("{{abc|" +
                                                              "foo=yes}}", 1));
            Assert.AreEqual(6, Tools.GetTemplateArgumentIndex("{{abc|  foo=yes|bar=no}}", 1));
            Assert.AreEqual(-1, Tools.GetTemplateArgumentIndex("{{abc|foo=yes}}", 2));
        }
        
        [Test]
        public void GetTemplateArgumentCount()
        {
            Assert.AreEqual(0, Tools.GetTemplateArgumentCount(@"{{foo}}"));
            Assert.AreEqual(1, Tools.GetTemplateArgumentCount(@"{{foo|bar}}"), "counts unnamed parameters");
            Assert.AreEqual(1, Tools.GetTemplateArgumentCount(@"{{foo|bar=yes}}"), "counts named parameters");
            
            Assert.AreEqual(3, Tools.GetTemplateArgumentCount(@"{{foo|bar|here|yte}}"), "counts multiple parameters");
            
            Assert.AreEqual(1, Tools.GetTemplateArgumentCount(@"{{foo|bar={{yes|foo}}}}"), "doesn't count nested template calls");
        }
        
        [Test]
        public void RenameTemplateParameter()
        {
            Assert.AreEqual(@"{{cite |param1=bar|param3=great}}", Tools.RenameTemplateParameter(@"{{cite |param1=bar|param2=great}}", "param2", "param3"));
            Assert.AreEqual(@"{{cite |param1=bar| param3 = great}}", Tools.RenameTemplateParameter(@"{{cite |param1=bar| param2 = great}}", "param2", "param3"));
            Assert.AreEqual(@"{{cite
|param1=bar
|param3=great}}", Tools.RenameTemplateParameter(@"{{cite
|param1=bar
|param2=great}}", "param2", "param3"));
            Assert.AreEqual(@"{{cite |param1=bar|param3=great|param=here}}", Tools.RenameTemplateParameter(@"{{cite |param1=bar|param2=great|param=here}}", "param2", "param3"));
            
            // comment handling
            Assert.AreEqual(@"{{cite |param1=bar|<!--comm-->param3=great}}", Tools.RenameTemplateParameter(@"{{cite |param1=bar|<!--comm-->param2=great}}", "param2", "param3"));
            Assert.AreEqual(@"{{cite |param1=bar|param3<!--comm-->=great}}", Tools.RenameTemplateParameter(@"{{cite |param1=bar|param2<!--comm-->=great}}", "param2", "param3"));
        }
        
        [Test]
        public void RenameTemplateParameterList()
        {
            List<string> Params = new List<string>(new [] { "param1" } );
            
            Assert.AreEqual(@"{{cite |paramx=bar|param2=great}}", Tools.RenameTemplateParameter(@"{{cite |param1=bar|param2=great}}", Params, "paramx"));
            
            Params.Add("param2");
            
            Assert.AreEqual(@"{{cite |paramx=bar|paramx=great}}", Tools.RenameTemplateParameter(@"{{cite |param1=bar|param2=great}}", Params, "paramx"));
            
            Params.Add("param3");
            Assert.AreEqual(@"{{cite |paramx=bar|paramx=great}}", Tools.RenameTemplateParameter(@"{{cite |param1=bar|param2=great}}", Params, "paramx"));
            Assert.AreEqual(@"{{cite |paramx=bar|param4=great}}", Tools.RenameTemplateParameter(@"{{cite |param1=bar|param4=great}}", Params, "paramx"));
        }
        
        [Test]
        public void RenameTemplateArticleText()
        {
            string correct = @"Now {{bar}} was {{bar|here}} there", correct2 = @"Now {{bar}} was {{bar
|here
|other}} there", correct3= @"Now {{bar man}} was {{bar man|here}} there",
            correct4= @"Now {{bar man<!--comm-->}} was {{bar man<!--comm-->|here}} there";
            Assert.AreEqual(correct, Tools.RenameTemplate(@"Now {{foo}} was {{foo|here}} there", "foo", "bar"));
            Assert.AreEqual(correct, Tools.RenameTemplate(@"Now {{Foo}} was {{foo|here}} there", "Foo", "bar"));
            
            Assert.AreEqual(@"Now {{bar}} was {{ bar |here}} there", Tools.RenameTemplate(@"Now {{foo}} was {{ foo |here}} there", "foo", "bar"));
            
            Assert.AreEqual(correct2, Tools.RenameTemplate(@"Now {{foo}} was {{foo
|here
|other}} there", "Foo", "bar"));
            
            Assert.AreEqual(correct, Tools.RenameTemplate(correct, "bar2", "foo"));
            
            Assert.AreEqual(correct3, Tools.RenameTemplate(@"Now {{foo man}} was {{foo man|here}} there", "foo man", "bar man"));
            Assert.AreEqual(correct3, Tools.RenameTemplate(@"Now {{foo man}} was {{foo man|here}} there", "Foo man", "bar man"));
            Assert.AreEqual(correct3, Tools.RenameTemplate(@"Now {{foo_man}} was {{foo man|here}} there", "Foo man", "bar man"));
            
            // comment handling
            Assert.AreEqual(correct4, Tools.RenameTemplate(@"Now {{foo_man<!--comm-->}} was {{foo man<!--comm-->|here}} there", "Foo man", "bar man"));
            
            // handles invalid template names gracefully
            Assert.AreEqual(correct, Tools.RenameTemplate(correct, @"foo(", "bar"));
            
            Assert.AreEqual(@"{{bar}} {{foo}}", Tools.RenameTemplate(@"{{foo}} {{foo}}", "foo", "bar", 1), "count applied correctly");
            
            Assert.AreEqual(@"{{bar||here1}} {{foo|here2}}", Tools.RenameTemplate(@"{{foo|here1}} {{foo|here2}}", "foo", "bar|", 1), "rename to add pipe");
        }
        
        [Test]
        public void RenameTemplate()
        {
            Assert.AreEqual(@"{{bar}}", Tools.RenameTemplate(@"{{foo}}", "bar"));
            
            // space kept
            Assert.AreEqual(@"{{ bar }}", Tools.RenameTemplate(@"{{ foo }}", "bar"));
            
            // casing
            Assert.AreEqual(@"{{bar}}", Tools.RenameTemplate(@"{{Foo}}", "bar"));
            Assert.AreEqual(@"{{Bar}}", Tools.RenameTemplate(@"{{Foo}}", "Bar"));
            
            // with params
            Assert.AreEqual(@"{{bar|parameters=adsfjk}}", Tools.RenameTemplate(@"{{Foo|parameters=adsfjk}}", "bar"));
            Assert.AreEqual(@"{{bar |parameters=adsfjk}}", Tools.RenameTemplate(@"{{Foo |parameters=adsfjk}}", "bar"));
            
            // comment handling
            Assert.AreEqual(@"{{bar man<!--comm-->|here}}", Tools.RenameTemplate(@"{{foo man<!--comm-->|here}}", "bar man"));
        }
        
        [Test]
        public void RemoveTemplateParameterTemplateName()
        {
            string correct = @"{{cite web|url=http://www.site.com |title=here |year=2008 }}";
            
            Assert.AreEqual(correct, Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat=mdy}}", "cite web", "dateformat"));
            Assert.AreEqual(correct, Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat=mdy }}", "cite web", "dateformat"));
            Assert.AreEqual(correct, Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat=}}", "cite web", "dateformat"));
            Assert.AreEqual(correct, Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |year=2008 | dateformat =   mdy}}", "cite web", "dateformat"));
            Assert.AreEqual(correct, Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here | dateformat=mdy|year=2008 }}", "cite web", "dateformat"));
            
            Assert.AreEqual(correct, Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat=<!--comm-->mdy}}", "cite web", "dateformat"));
            Assert.AreEqual(correct, Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat=mdy<!--comm-->}}", "cite web", "dateformat"));
            
            Assert.AreEqual(correct, Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat=[[200]]-[[288]]}}", "cite web", "dateformat"));
            Assert.AreEqual(correct, Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat = [[200]]-[[288]]}}", "cite web", "dateformat"));
            
            Assert.AreEqual(@"{{cite web<!--foo-->|url=http://www.site.com |title=here |year=2008 }}",
                            Tools.RemoveTemplateParameter(@"{{cite web<!--foo-->|url=http://www.site.com |title=here |year=2008 |dateformat=mdy}}", "cite web", "dateformat"));
            
            // processes multiple templates
            string citeweb = @"{{cite web|url=http://www.site.com |title=here | dateformat=mdy|year=2008 }}";
            Assert.AreEqual(correct + correct, Tools.RemoveTemplateParameter(citeweb + citeweb, "cite web", "dateformat"));
            
            // first letter case insensitive
            Assert.AreEqual(correct, Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat=mdy}}", "Cite web", "dateformat"));
            
            // no change when parameter doesn't exist
            Assert.AreEqual(correct, Tools.RemoveTemplateParameter(correct, "cite web", "dateformat"));
            
            // no partial match on paramter name
            Assert.AreEqual(correct, Tools.RemoveTemplateParameter(correct, "cite web", "yea"));
            
            // no partial match on template name
            Assert.AreEqual(correct, Tools.RemoveTemplateParameter(correct, "cite webs", "year"));
            
            // parameter name case sensitive
            string nochange = @"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat=mdy}}";
            Assert.AreEqual(nochange, Tools.RemoveTemplateParameter(nochange, "cite web", "Dateformat"));
        }
        
        [Test]
        public void RemoveTemplateParameterSingleTemplate()
        {
            string correct = @"{{cite web|url=http://www.site.com |title=here |year=2008 }}";
            
            Assert.AreEqual(correct, Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat=mdy}}", "dateformat"));
            Assert.AreEqual(correct, Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat=mdy }}", "dateformat"));
            Assert.AreEqual(correct, Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat=}}", "dateformat"));
            Assert.AreEqual(correct, Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |year=2008 | dateformat =   mdy}}", "dateformat"));
            Assert.AreEqual(correct, Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here | dateformat=mdy|year=2008 }}", "dateformat"));
            
            Assert.AreEqual(correct, Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here | dateformat=mdy|year=2008 }}", "dateformat", false));
            Assert.AreEqual(correct, Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here | dateformat=mdy|year=2008 }}", "dateformat", true));
            
            // first letter case insensitive
            Assert.AreEqual(correct, Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat=mdy}}", "dateformat"));
            
            // no change when parameter doesn't exist
            Assert.AreEqual(correct, Tools.RemoveTemplateParameter(correct, "dateformat"));
            
            // no partial match on paramter name
            Assert.AreEqual(correct, Tools.RemoveTemplateParameter(correct, "yea"));
            
            // parameter name case sensitive
            string nochange = @"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat=mdy}}";
            Assert.AreEqual(nochange, Tools.RemoveTemplateParameter(nochange, "Dateformat"));
            
            // duplicate parameters
            Assert.AreEqual(@"{{cite web|url=http://www.site.com |title=here | dateformat=mdy|year=2008 }}", 
                            Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here | dateformat=mdy|year=2008 |dateformat=foo}}", "dateformat", true));
            
            Assert.AreEqual(@"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat=foo}}",
                            Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here | dateformat=mdy|year=2008 |dateformat=foo}}", "dateformat", false));
            
            Assert.AreEqual(@"{{foo|first=abc|second=def}}",
                            Tools.RemoveTemplateParameter(@"{{foo|first=abc|second=def|second=def}}", "second", false));
        }
        
        [Test]
        public void RemoveTemplateParameterAdvancedCases()
        {
            string correct = @"{{cite web|url=http://www.site.com |title=here |year=2008 }}";
            
            Assert.AreEqual(correct, Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |year=2008 |
dateformat=mdy}}", "cite web", "dateformat"));
            Assert.AreEqual(correct, Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat
=mdy}}", "cite web", "dateformat"));
            Assert.AreEqual(correct, Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat=[[Foo|bar]]}}", "cite web", "dateformat"));
            Assert.AreEqual(correct, Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat= now [[Foo|bar]] and [[Foo|bar]] again}}", "cite web", "dateformat"));
            Assert.AreEqual(correct, Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat= {{foo|mdy}} bar}}", "cite web", "dateformat"));
            Assert.AreEqual(correct, Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat= {{foo|mdy}} bar {{here}}}}", "cite web", "dateformat"));
            Assert.AreEqual(correct, Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat= <!--now|--> bar}}", "cite web", "dateformat"));
            Assert.AreEqual(correct, Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |year=2008 |date_format= A<nowiki>|</nowiki>B bar}}", "cite web", "date_format"));
            Assert.AreEqual(correct, Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat= [[File:foo.JPG|bar|here]] bar}}", "cite web", "dateformat"));
            
            Assert.AreEqual(correct, Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |dateformat= [[File:foo.JPG|bar|here]] bar|year=2008 }}", "cite web", "dateformat"));
            Assert.AreEqual(correct, Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |dateformat= <!--now|--> bar|title=here |year=2008 }}", "cite web", "dateformat"));
            Assert.AreEqual(correct, Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |dateformat= {{some template|foo={{a}}|bar=b}} bar|title=here |year=2008 }}", "cite web", "dateformat"));
        }
        
        [Test]
        public void RemoveDuplicateTemplateParameters()
        {
            Assert.AreEqual(@"{{foo|first=abc|second=def}}", Tools.RemoveDuplicateTemplateParameters(@"{{foo|first=abc|second=def|second=def}}"));
            Assert.AreEqual(@"{{foo|first=abc|second=def}}", Tools.RemoveDuplicateTemplateParameters(@"{{foo|first=abc|second=def|second=de}}"), "removed when one contains the other");
            Assert.AreEqual(@"{{foo|first=abc|second=def|a=c}}", Tools.RemoveDuplicateTemplateParameters(@"{{foo|first=abc|second=def|a=c|second=de}}"));
            Assert.AreEqual(@"{{foo|first=abc|a=c|second=def}}", Tools.RemoveDuplicateTemplateParameters(@"{{foo|first=abc|second=de|a=c|second=def}}"));
            Assert.AreEqual(@"{{foo|first=abc|second = def}}", Tools.RemoveDuplicateTemplateParameters(@"{{foo|first=abc|second=def|second = def}}"), "first removed if both the same");
            Assert.AreEqual(@"{{foo|first=abc|second=def}}", Tools.RemoveDuplicateTemplateParameters(@"{{foo|first=abc|second=def|second=}}"), "null dupe param removed");
            Assert.AreEqual(@"{{foo|first=abc|second=
def
}}", Tools.RemoveDuplicateTemplateParameters(@"{{foo|first=abc|second=def|second=
def
}}"), "dupe param removed ignoring leading/trailing whitespace");
            Assert.AreEqual(@"{{foo|first=abc|second=def}}", Tools.RemoveDuplicateTemplateParameters(@"{{foo|first=abc|second=|second=def}}"), "null dupe param removed");
            
            Assert.AreEqual(@"{{foo|first=abc|second=def}}", Tools.RemoveDuplicateTemplateParameters(@"{{foo|first=abc|second=def|second=def|second=def}}"), "multiple duplicates removed");
            Assert.AreEqual(@"{{foo|first=abc|second=def}}", Tools.RemoveDuplicateTemplateParameters(@"{{foo|first=abc|first=abc|second=def|second=def|second=def}}"), "multiple duplicates removed");
            
            const string noDupe2 = @"{{foo|first=abc|second=def|Second=def}}";
            
            Assert.AreEqual(noDupe2, Tools.RemoveDuplicateTemplateParameters(noDupe2), "case sensitive parameter name matching");
            
            Assert.AreEqual(@"{{foo|first=abc|second=dex|second=defg}}", Tools.RemoveDuplicateTemplateParameters(@"{{foo|first=abc|second=dex|second=dex|second=defg}}"), "non-duplicates not removed");
            
            Assert.AreEqual(@"{{foo|first=abc|second={{def|bar}}}}", Tools.RemoveDuplicateTemplateParameters(@"{{foo|first=abc|second={{def|bar}}|second={{def|bar}}}}"));
        }
        
        [Test]
        public void RemoveDuplicateTemplateParametersURLs()
        {
            const string withUnescapedPipes = @"{{cite web|foo=bar|url=http://site.com/news/foo|bar=yes|bar=yes|other.stm | date=2010}}";
            
            Assert.AreEqual(withUnescapedPipes, Tools.RemoveDuplicateTemplateParameters(withUnescapedPipes), "no change when URL could be borken");
        }
        
        [Test]
        public void DuplicateTemplateParameters()
        {
            Dictionary<int,int> Dupes = new Dictionary<int, int>();
            
            Assert.AreEqual(Dupes, Tools.DuplicateTemplateParameters(@"{{cite web|url=here |title=there}}"));            
            
            Dupes.Add(32, 9);
            Assert.AreEqual(Dupes, Tools.DuplicateTemplateParameters(@"{{cite web|url=here |title=there|url=here}}"), "dupes if the same");
            Assert.AreEqual(Dupes, Tools.DuplicateTemplateParameters(@"{{cite web|url=here |title=there|url=her2}}"), "dupes if not the same");
            
            Dupes.Clear();
            Dupes.Add(36, 5);
            Assert.AreEqual(Dupes, Tools.DuplicateTemplateParameters(@"{{cite web|url=here |title=therehere|url=}}"), "dupes if the same");
            
            Dupes.Clear();
            Dupes.Add(32, 9);
            Dupes.Add(41, 12);
            Assert.AreEqual(Dupes, Tools.DuplicateTemplateParameters(@"{{cite web|url=here |title=there|url=here|title=there}}"), "multiple dupes reported");
        }
        
        [Test]
        public void UnknownTemplateParameters()
        {
            List<string> Unknowns = new List<string>();
            
            List<string> Knowns = new List<string>(new [] { "title", "date", "url" } );
            
            Assert.AreEqual(Unknowns, Tools.UnknownTemplateParameters(@"{{cite web|title=a|date=2010}}", Knowns));
            
            Unknowns.Add("foo");
            Assert.AreEqual(Unknowns, Tools.UnknownTemplateParameters(@"{{cite web|title=a|date=2010|foo=}}", Knowns), "reported even if blank");
            Assert.AreEqual(Unknowns, Tools.UnknownTemplateParameters(@"{{cite web|title=a|date=2010|foo=b}}", Knowns), "unknown parameter reported");
        
            Unknowns.Add("bar");
            Assert.AreEqual(Unknowns, Tools.UnknownTemplateParameters(@"{{cite web|title=a|date=2010|foo=b|bar=}}", Knowns), "multiple unknowns reported");
        }
        
        [Test]
        public void UpdateTemplateParameterValue()
        {
            Assert.AreEqual(@"{{foo|param1=valueafter}}", Tools.UpdateTemplateParameterValue(@"{{foo|param1=before}}", "param1", "valueafter"));
            Assert.AreEqual(@"{{foo|param1= valueafter }}", Tools.UpdateTemplateParameterValue(@"{{foo|param1= before }}", "param1", "valueafter"), "whitepsace kept");
            
            Assert.AreEqual(@"{{foo
|param1=valueafter
|param2=okay}}", Tools.UpdateTemplateParameterValue(@"{{foo
|param1=before
|param2=okay}}", "param1", "valueafter"), "existing newline kept");
            
            Assert.AreEqual(@"{{foo
|param1= valueafter 
|param2=okay}}", Tools.UpdateTemplateParameterValue(@"{{foo
|param1= before 
|param2=okay}}", "param1", "valueafter"), "existing newline & whitespace kept");
            
            Assert.AreEqual(@"{{foo|param1<!--comm-->= valueafter }}", Tools.UpdateTemplateParameterValue(@"{{foo|param1<!--comm-->= before }}", "param1", "valueafter"));
            Assert.AreEqual(@"{{foo|<!--comm-->param1= valueafter }}", Tools.UpdateTemplateParameterValue(@"{{foo|<!--comm-->param1= before }}", "param1", "valueafter"));
            
            Assert.AreEqual(@"{{foo|param1=[[1891]]–[[1892]]}}", Tools.UpdateTemplateParameterValue(@"{{foo|param1=[[1891]]-[[1892]]}}", "param1", "[[1891]]–[[1892]]"));
            Assert.AreEqual(@"{{foo|param1= [[1891]]–[[1892]] }}", Tools.UpdateTemplateParameterValue(@"{{foo|param1= [[1891]]-[[1892]] }}", "param1", "[[1891]]–[[1892]]"));
            
            Assert.AreEqual(@"{{foo|param1=valueafter|param2=before}}", Tools.UpdateTemplateParameterValue(@"{{foo|param1=before|param2=before}}", "param1", "valueafter"));
            
            Assert.AreEqual(@"{{foo|param1=valueafter<!--comm-->|param2=before}}", Tools.UpdateTemplateParameterValue(@"{{foo|param1=before<!--comm-->|param2=before}}", "param1", "valueafter<!--comm-->"));
            Assert.AreEqual(@"{{foo|param1=<!--comm-->valueafter|param2=before}}", Tools.UpdateTemplateParameterValue(@"{{foo|param1=<!--comm-->before|param2=before}}", "param1", "<!--comm-->valueafter"));
            
            // parameter not used – no change
            Assert.AreEqual(@"{{foo|param1=before}}", Tools.UpdateTemplateParameterValue(@"{{foo|param1=before}}", "param2", "valueafter"));
            
            // old value null – updated correctly
            Assert.AreEqual(@"{{foo|param1=valueafter}}", Tools.UpdateTemplateParameterValue(@"{{foo|param1=}}", "param1", "valueafter"));
            
            string input=@"{{cite news
|author=
|title=Obituary 1 -- No Title
|date=
|work=[[New York Times]]
|url=http://query.nytimes.com/gst/abstra
|accessdate=2008-08-08
}}";
            Assert.AreEqual(input.Replace("|date=", "|date=April 4, 1922"), Tools.UpdateTemplateParameterValue(input, "date", "April 4, 1922"));
            
            input=@"{{cite news
|author=
|title=Obituary 1 -- No Title
|date=


|work=[[New York Times]]
|url=http://query.nytimes.com/gst/abstra }}";
            
            Assert.AreEqual(input.Replace("|date=", "|date=April 4, 1922"), Tools.UpdateTemplateParameterValue(input, "date", "April 4, 1922"));
        }

        [Test]
        public void SetTemplateParameterValue()
        {
            Assert.AreEqual(@"{{foo|param1=valueafter}}", Tools.SetTemplateParameterValue(@"{{foo|param1=before}}", "param1", "valueafter"));
            Assert.AreEqual(@"{{foo|param1= valueafter }}", Tools.SetTemplateParameterValue(@"{{foo|param1= before }}", "param1", "valueafter"));
            
            Assert.AreEqual(@"{{foo|param1=valueafter|param2=before}}", Tools.SetTemplateParameterValue(@"{{foo|param1=before|param2=before}}", "param1", "valueafter"));
            
            // parameter not used – set
            Assert.AreEqual(@"{{foo|param1=before | param2=valueafter}}", Tools.SetTemplateParameterValue(@"{{foo|param1=before}}", "param2", "valueafter"));
            Assert.AreEqual(@"{{foo|param1=before|param3=a | param2=valueafter}}", Tools.SetTemplateParameterValue(@"{{foo|param1=before|param3=a}}", "param2", "valueafter"));
            
            // old value null – updated correctly
            Assert.AreEqual(@"{{foo|param1=valueafter}}", Tools.SetTemplateParameterValue(@"{{foo|param1=}}", "param1", "valueafter"));
            
            string bug1 = @"{{Infobox college coach|
| Name          = Bill
| CurrentRecord = 202–43 ({{Winning percentage|202|43}})
| OverallRecord = 409-148 ({{Winning percentage|409|148}})
| Player        = *
| CollegeHOFID  =
| BBallHOF      =
}}}";
            Assert.AreEqual(bug1.Replace(@"409-148 ({{Winning percentage", @"409–148 ({{Winning percentage"), Tools.SetTemplateParameterValue(bug1, "OverallRecord", @"409–148 ({{Winning percentage|409|148}})"));
            
            string input=@"{{cite news
|author=
|title=Obituary 1 -- No Title
|date=
|work=[[New York Times]]
|url=http://query.nytimes.com/gst/abstra
|accessdate=2008-08-08
}}";
            Assert.AreEqual(input.Replace("|date=", "|date=April 4, 1922"), Tools.SetTemplateParameterValue(input, "date", "April 4, 1922"));
        
        // existing value = new one, no change
        Assert.AreEqual(@"{{foo|param1=valueafter}}", Tools.SetTemplateParameterValue(@"{{foo|param1=valueafter}}", "param1", "valueafter"));
        }
        
        [Test]
        public void  NestedTemplateRegexSingleWord()
        {
            Regex FooTemplate =  Tools.NestedTemplateRegex("foo");
            
            Assert.AreEqual(FooTemplate.Match(@"{{  foo}}").Groups[1].Value, @"{{  ");
            Assert.AreEqual(FooTemplate.Match(@"{{foo}}").Groups[2].Value, @"foo");
            Assert.AreEqual(FooTemplate.Match(@"{{ Foo}}").Groups[2].Value, @"Foo");
            Assert.AreEqual(FooTemplate.Match(@"{{ Foo|title=abc}}").Groups[3].Value, @"|title=abc}}");
            
            Assert.IsTrue(FooTemplate.IsMatch(@"{{foo}}"));
            Assert.IsTrue(FooTemplate.IsMatch(@"{{___foo___}}"));
            Assert.IsTrue(FooTemplate.IsMatch(@"{{Foo}}"));
            Assert.IsTrue(FooTemplate.IsMatch(@"{{ foo}}"));
            Assert.IsTrue(FooTemplate.IsMatch(@"{{ foo|}}"));
            Assert.IsTrue(FooTemplate.IsMatch(@"{{foo|title=abc}}"));
            Assert.IsTrue(FooTemplate.IsMatch(@"{{foo|
title=abc
|other=yes}}"));
            Assert.IsTrue(FooTemplate.IsMatch(@"{{foo
|title=abc
|other=yes}}"));
            Assert.IsTrue(FooTemplate.IsMatch(@"{{foo<!--comm-->|title=abc}}"));
            Assert.IsTrue(FooTemplate.IsMatch(@"{{foo <!--comm--> |title=abc}}"));
            Assert.IsTrue(FooTemplate.IsMatch(@"{{foo ⌊⌊⌊⌊0⌋⌋⌋⌋ |title=abc}}"));
            Assert.IsTrue(FooTemplate.IsMatch(@"{{
foo<!--comm-->|title=abc
}}"));
            Assert.IsTrue(FooTemplate.IsMatch(@"{{foo|title={{abc}}}}"));
            Assert.IsTrue(FooTemplate.IsMatch(@"{{foo|title={{abc}}|last=Fred}}"), "matches nested templates");
            Assert.IsTrue(FooTemplate.IsMatch(@"{{foo|
title={{abc|fdkjdsfjk=fdaskjlfds
|fdof=affdsa}}
|last=Fred}}"), "matches nested parameterised templates");
            
            Assert.IsTrue(FooTemplate.IsMatch(@"{{foo|title={abc} def|last=Fred}}"), "matches balanced single curly braces");
            
            Assert.IsFalse(FooTemplate.IsMatch(@"{{foo|title={abc def|last=Fred}}"), "doesn't match unbalanced single curly braces");
            
            Assert.IsFalse(FooTemplate.IsMatch(@"{{foobar}}"));
            Assert.IsFalse(FooTemplate.IsMatch(@"{{foo"));
            Assert.IsFalse(FooTemplate.IsMatch(@"{{foo}"));
            Assert.IsFalse(FooTemplate.IsMatch(@"{foo}"));
            
            FooTemplate =  Tools.NestedTemplateRegex("Foo");
            Assert.IsTrue(FooTemplate.IsMatch(@"{{foo}}"));
            Assert.IsTrue(FooTemplate.IsMatch(@"{{Foo}}"));
            Assert.IsTrue(FooTemplate.IsMatch(@"{{ foo}}"));
            Assert.IsTrue(FooTemplate.IsMatch(@"{{Template:foo}}"));
            Assert.IsTrue(FooTemplate.IsMatch(@"{{:Template:foo}}"));
            Assert.IsTrue(FooTemplate.IsMatch(@"{{_:_Template_:_foo_}}"));
            
            Assert.IsFalse(FooTemplate.IsMatch(@"{{Template foo}}"));
        }
        
        [Test]
        public void  NestedTemplateRegexTwoWords()
        {
            Regex FooTemplate2 =  Tools.NestedTemplateRegex("foo bar");
            
            Assert.IsTrue(FooTemplate2.IsMatch(@"{{foo bar}}"));
            Assert.IsTrue(FooTemplate2.IsMatch(@"{{foo_bar}}"));
            Assert.IsTrue(FooTemplate2.IsMatch(@"{{Foo bar}}"));
            
            Assert.IsFalse(FooTemplate2.IsMatch(@"{{foo}}"));
        }
        
        [Test]
        public void  NestedTemplateRegexLimits()
        {
            Regex FooTemplate2 =  Tools.NestedTemplateRegex("");
            
            Assert.IsNull(FooTemplate2);
        }
        
        [Test]
        public void  NestedTemplateRegexListSingle()
        {
            List<string> ListOfTemplates = new List<string>();
            
            Regex MultipleTemplatesN = Tools.NestedTemplateRegex(ListOfTemplates);
            Assert.IsNull(MultipleTemplatesN, "null return if zero-entry list input");
            
            ListOfTemplates.Add(@"Foo");
            
            Regex multipleTemplates = Tools.NestedTemplateRegex(ListOfTemplates);
            
            Assert.AreEqual(multipleTemplates.Match(@"{{foo}}").Groups[2].Value, @"foo");
            Assert.AreEqual(multipleTemplates.Match(@"{{ Foo}}").Groups[2].Value, @"Foo");
            Assert.AreEqual(multipleTemplates.Match(@"{{ Foo|bar}}").Groups[3].Value, @"|bar}}");
            
            Assert.IsTrue(multipleTemplates.IsMatch(@"{{foo}}"));
            Assert.IsTrue(multipleTemplates.IsMatch(@"{{Foo}}"));
            Assert.IsTrue(multipleTemplates.IsMatch(@"{{ foo}}"));
            Assert.IsTrue(multipleTemplates.IsMatch(@"{{ foo|}}"));
            Assert.IsTrue(multipleTemplates.IsMatch(@"{{foo|title=abc}}"));
            Assert.IsTrue(multipleTemplates.IsMatch(@"{{foo|
title=abc
|other=yes}}"));
            Assert.IsTrue(multipleTemplates.IsMatch(@"{{foo
|title=abc
|other=yes}}"));
            Assert.IsTrue(multipleTemplates.IsMatch(@"{{foo<!--comm-->|title=abc}}"));
            Assert.IsTrue(multipleTemplates.IsMatch(@"{{foo <!--comm--> |title=abc}}"));
            Assert.IsTrue(multipleTemplates.IsMatch(@"{{
foo<!--comm-->|title=abc
}}"));
            Assert.IsTrue(multipleTemplates.IsMatch(@"{{foo|title={{abc}}}}"));
            Assert.IsTrue(multipleTemplates.IsMatch(@"{{foo|title={{abc}}|last=Fred}}"));
            
            Assert.IsFalse(multipleTemplates.IsMatch(@"{{a}}"));
            Assert.IsFalse(multipleTemplates.IsMatch(@""));
        }
        
        [Test]
        public void  NestedTemplateRegexListMultiple()
        {
            List<string> listOfTemplates = new List<string>(new [] { "Foo", "bar" } );
            
            Regex multipleTemplates = Tools.NestedTemplateRegex(listOfTemplates);
            
            Assert.AreEqual(multipleTemplates.Match(@"{{foo}}").Groups[2].Value, @"foo");
            Assert.AreEqual(multipleTemplates.Match(@"{{Template:foo}}").Groups[2].Value, @"foo");
            Assert.AreEqual(multipleTemplates.Match(@"{{ template :foo}}").Groups[2].Value, @"foo");
            Assert.AreEqual(multipleTemplates.Match(@"{{ Foo}}").Groups[2].Value, @"Foo");
            Assert.AreEqual(multipleTemplates.Match(@"{{Bar |akjldasf=a}}").Groups[2].Value, @"Bar");
            Assert.AreEqual(multipleTemplates.Match(@"{{bar
}}").Groups[2].Value, @"bar");
            Assert.AreEqual(multipleTemplates.Match(@"{{ foo}}").Groups[1].Value, @"{{ ");
            Assert.AreEqual(multipleTemplates.Match(@"{{ foo |akjldasf=a}}").Groups[3].Value, @" |akjldasf=a}}");
            
            Assert.IsTrue(multipleTemplates.IsMatch(@"{{foo}}"));
            Assert.IsTrue(multipleTemplates.IsMatch(@"{{Foo}}"));
            Assert.IsTrue(multipleTemplates.IsMatch(@"{{ foo}}"));
            Assert.IsTrue(multipleTemplates.IsMatch(@"{{ foo|}}"));
            Assert.IsTrue(multipleTemplates.IsMatch(@"{{foo|title=abc}}"));
            Assert.IsTrue(multipleTemplates.IsMatch(@"{{foo|
title=abc
|other=yes}}"));
            Assert.IsTrue(multipleTemplates.IsMatch(@"{{foo
|title=abc
|other=yes}}"));
            Assert.IsTrue(multipleTemplates.IsMatch(@"{{foo<!--comm-->|title=abc}}"));
            Assert.IsTrue(multipleTemplates.IsMatch(@"{{foo <!--comm--> |title=abc}}"));
            Assert.IsTrue(multipleTemplates.IsMatch(@"{{
foo<!--comm-->|title=abc
}}"));
            Assert.IsTrue(multipleTemplates.IsMatch(@"{{foo|title={{abc}}}}"));
            Assert.IsTrue(multipleTemplates.IsMatch(@"{{foo|title={{abc}}|last=Fred}}"));
            
            Assert.IsTrue(multipleTemplates.IsMatch(@"{{bar}}"));
            Assert.IsTrue(multipleTemplates.IsMatch(@"{{ bar}}"));
            Assert.IsTrue(multipleTemplates.IsMatch(@"{{Bar}}"));
            
            Assert.IsFalse(multipleTemplates.IsMatch(@"{{a}}"));
            Assert.IsFalse(multipleTemplates.IsMatch(@""));
        }
        
        [Test]
        public void GetMetaContentValue()
        {
            Assert.AreEqual(@"2009-03-02", Tools.GetMetaContentValue(@"<meta name=""PubDate""  content=""2009-03-02"">", "PubDate"));
            Assert.AreEqual(@"2009-03-02", Tools.GetMetaContentValue(@"< META NAME = ""PubDate""  content = ""2009-03-02""  />", "PubDate"));
            Assert.AreEqual(@"2009-03-02", Tools.GetMetaContentValue(@"<meta name  =""PubDate"" CONTENT="" 2009-03-02 "">", "PUBDATE"));
            
            Assert.AreEqual(@"", Tools.GetMetaContentValue(@"<meta name  =""PubDate"" CONTENT="" 2009-03-02 "">", "PUBDATEXX"));
            Assert.AreEqual(@"", Tools.GetMetaContentValue(@"<meta name  =""PubDateX"" CONTENT="" 2009-03-02 "">", "PUBDATE"));
            
            Assert.AreEqual(@"", Tools.GetMetaContentValue(@"<meta name  =""PubDateX"" CONTENT="" 2009-03-02 "">", ""));
            Assert.AreEqual(@"", Tools.GetMetaContentValue("", "PUBDATE"));
        }
        
        [Test]
        public void GetTemplateName()
        {
            Assert.AreEqual(Tools.GetTemplateName(@"{{Start date and age|1833|7|11}}"),"Start date and age");
            
            // whitespace handling
            Assert.AreEqual(Tools.GetTemplateName(@"{{ Start date and age |1833|7|11}}"),"Start date and age");
            Assert.AreEqual(Tools.GetTemplateName(@"{{
Start date and age
|1833|7|11}}"),"Start date and age");
            
            Assert.AreEqual(Tools.GetTemplateName(@"{{start date and age <!--comm--> |1833|7|11}}"),"start date and age", "handles embedded comments");
            Assert.AreEqual(Tools.GetTemplateName(@"{{start date and age <!--comm-->}}"),"start date and age", "handles embedded comments");
            
            Assert.AreEqual(Tools.GetTemplateName(@"{{Start date and age|1833|7|"),"Start date and age", "works on part templates");
            
            Assert.AreEqual(Tools.GetTemplateName(@"{{Template:Foo|1=yes}}"), "Foo", "Template namespace removed");
            Assert.AreEqual(Tools.GetTemplateName(@"{{ Template : Foo |1=yes}}"), "Foo", "Template namespace removed");
            Assert.AreEqual(Tools.GetTemplateName(@"{{template:Foo|1=yes}}"), "Foo", "Template namespace removed");
            
            Assert.AreEqual(Tools.GetTemplateName(@""), "");
        }
    }

    [TestFixture]
    public class HumanCatKeyTests : RequiresInitialization
    {
        [Test]
        public void MakeHumanCatKeyOneWordNames()
        {
            Assert.AreEqual("Onewordname", Tools.MakeHumanCatKey("OneWordName"));
            Assert.AreEqual("Onewordname", Tools.MakeHumanCatKey("ONEWORDNAME"));
            Assert.AreEqual("Onewordname", Tools.MakeHumanCatKey("Onewordname"));
            Assert.AreEqual("Onewordname", Tools.MakeHumanCatKey("onewordname"));
        }

        [Test]
        public void MakeHumanCatKeyWithRomanNumbers()
        {
            Assert.AreEqual("Doe, John, Iii", Tools.MakeHumanCatKey("John Doe III"));
            Assert.AreEqual("John Iii", Tools.MakeHumanCatKey("John III"));
            Assert.AreEqual("Xvii", Tools.MakeHumanCatKey("XVII"));
            Assert.AreEqual("Spain, John Doe King Of, Iii", Tools.MakeHumanCatKey("John Doe King of Spain III"));
        }

        [Test]
        public void MakeHumanCatKeyWithJrSr()
        {
            Assert.AreEqual("Doe, John, Jr.", Tools.MakeHumanCatKey("John Doe, Jr."));
            Assert.AreEqual("Doe, John, Sr.", Tools.MakeHumanCatKey("John Doe, Sr."));
            Assert.AreEqual("Doe, John, Jnr.", Tools.MakeHumanCatKey("John Doe, Jnr."));
            Assert.AreEqual("Doe, John, Snr.", Tools.MakeHumanCatKey("John Doe, Snr."));
            
            Assert.AreEqual("Doe, John, Snr.", Tools.MakeHumanCatKey("John Doe Snr."));
            Assert.AreEqual("Hickham, Steven A., Jr.", Tools.MakeHumanCatKey("Steven A. Hickham Jr."));
            Assert.AreEqual("Hickham, Steven A., Jnr.", Tools.MakeHumanCatKey("Steven A. Hickham Jnr."));
            Assert.AreEqual("Hickham, Steven, Jr.", Tools.MakeHumanCatKey("Steven Hickham Jr."));
        }

        [Test]
        public void MakeHumanCatKeyWithApostrophes()
        {
            Assert.AreEqual("Ddoe, John", Tools.MakeHumanCatKey("J'ohn D'Doe"));
            Assert.AreEqual("Test", Tools.MakeHumanCatKey("'Test"));
        }

        [Test]
        public void MakeHumanCatKeyWithPrefixes()
        {
            Assert.AreEqual("Doe, John De", Tools.MakeHumanCatKey("John de Doe"));
        }

        [Test]
        public void MakeHumanCatKeyDiacritics()
        {
            Assert.AreEqual("Doe", Tools.MakeHumanCatKey("Ďöê"));
            Assert.AreEqual("Doe, John", Tools.MakeHumanCatKey("Ĵǒħń Ďöê"));

            Assert.AreEqual(@"Gu, Prince Imperial Hoeun", Tools.MakeHumanCatKey("Gu, Prince Imperial Hoeun"));

            // Ё should be changed, but not Й
            Assert.AreEqual("Епрстий", Tools.MakeHumanCatKey("Ёпрстий"));
        }

        [Test]
        public void MakeHumanCatKeyArabicNames()
        {
            Assert.AreEqual(@"Ahmed Mohammed Mukit", Tools.MakeHumanCatKey(@"Ahmed Mohammed Mukit"));
            Assert.AreEqual(@"Ahmed Mohammed Mukit", Tools.MakeHumanCatKey(@"AHMED Mohammed MUKIT"));
            Assert.AreEqual(@"Ahmed Mohammed Mukit", Tools.MakeHumanCatKey(@"ahmed Mohammed mukit"));
            
            Assert.AreEqual(@"Smith, John", Tools.MakeHumanCatKey(@"John Smith"));
        }
        
        [Test]
        public void MakeHumanCatKeyMcName()
        {
            Assert.AreEqual(@"Macsmith, John", Tools.MakeHumanCatKey(@"John McSmith"));
            Assert.AreEqual(@"Macsmith, John", Tools.MakeHumanCatKey(@"John MacSmith"));
            
            Assert.AreEqual(@"Mcsmith, John", Tools.MakeHumanCatKey(@"John Mcsmith"));
            
            Assert.AreEqual(@"Smith, John", Tools.MakeHumanCatKey(@"John Smith"));
            Assert.AreEqual(@"Macintosh, John", Tools.MakeHumanCatKey(@"John Macintosh"));
        }

        [Test]
        public void RemoveDiacritics()
        {
            foreach (KeyValuePair<string, string> kvp in Tools.Diacritics)
            {
                Assert.AreEqual(kvp.Value, Tools.RemoveDiacritics(kvp.Key));
            }

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#Leaving_foreign_characters_in_DEFAULTSORT
            Assert.AreEqual(@"aaaaa eee ii oooo uuu y", Tools.RemoveDiacritics(@"ắạảằẩ ếễệ ịỉ ỏøờồ ụủữ ỳ"));
        }
        
        [Test]
        public void HasDiacritics()
        {
            Assert.IsTrue(Tools.HasDiacritics("hellõ"));
            Assert.IsTrue(Tools.HasDiacritics("hellõ there"));
            Assert.IsTrue(Tools.HasDiacritics("hẽllõ there"));
            Assert.IsTrue(Tools.HasDiacritics("hẽllo there"));
            
            Assert.IsFalse(Tools.HasDiacritics("hello"));
            Assert.IsFalse(Tools.HasDiacritics(""));
        }

        [Test]
        public void FixUpDefaultSortTests()
        {
            Assert.AreEqual("Hello", Tools.FixupDefaultSort("hellõ"));
            Assert.AreEqual("Hello", Tools.FixupDefaultSort("hellõ   "));
            Assert.AreEqual(@"Fred Smith", Tools.FixupDefaultSort(@"FRED SMITH"));
            Assert.AreEqual(@"Fred Smith", Tools.FixupDefaultSort(@"fred smith"));
            Assert.AreEqual(@"Fred Smithson", Tools.FixupDefaultSort(@"fred smithson"));
            Assert.AreEqual(@"Fred Smith", Tools.FixupDefaultSort(@"fred smitH"));
            Assert.AreEqual(@"Fred Smith", Tools.FixupDefaultSort(@"Fred Smith"));
            Assert.AreEqual(@"Smith, Fred", Tools.FixupDefaultSort(@"Smith, FRed"));
            Assert.AreEqual(@"Oneworditem", Tools.FixupDefaultSort(@"OneWordItem"));
            Assert.AreEqual(@"2007 Fifa Women World Cup Squads", Tools.FixupDefaultSort(@"2007 Fifa women world cup squads"));

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#DEFAULTSORT_capitalization_after_apostrophes
            Assert.AreEqual("Kwakwaka'wakw Mythology", Tools.FixupDefaultSort("Kwakwaka'wakw mythology"));
            Assert.AreEqual(@"Peewee's Playhouse", Tools.FixupDefaultSort(@"Peewee's Playhouse"));
        }

        [Test]
        public void RemoveNamespace()
        {
            Assert.AreEqual("Doe, John", Tools.MakeHumanCatKey("Wikipedia:John Doe"));
        }

        [Test]
        public void FuzzTest()
        {
            const string allowedChars = "abcdefghijklmnopqrstuvwxyzйцукенфывапролдж              ,,,,,";

            Random rnd = new Random();

            for (int i = 0; i < 10000; i++)
            {
                string name = "";

                for (int j = 0; j < rnd.Next(45); j++) name += allowedChars[rnd.Next(allowedChars.Length)];
                name = Regex.Replace(name, @"\s{2,}", " ").Trim(new[] { ' ', ',' });

                name = Tools.MakeHumanCatKey(name);

                Assert.IsFalse(name.Contains("  "), "Sorting key shouldn't contain consecutive spaces - it breaks the sorting ({0})", name);
                Assert.IsFalse(name.StartsWith(" "), "Sorting key shouldn't start with spaces");
                Assert.IsFalse(name.EndsWith(" "), "Sorting key shouldn't end with spaces");
            }
        }
    }

    [TestFixture]
    public class NamespaceFunctions : RequiresInitialization
    {
        #region Helpers

        static string ToTalk(string title)
        {
            return Tools.ConvertToTalk(new Article(title));
        }

        static string FromTalk(string title)
        {
            return Tools.ConvertFromTalk(new Article(title));
        }
        #endregion

        #region NS switching
        [Test]
        public void ConvertToTalk()
        {
            Assert.AreEqual("Talk:Foo", ToTalk("Foo"));
            Assert.AreEqual("Talk:Foo bar", ToTalk("Foo bar"));
            Assert.AreEqual("Talk:Foo:Bar", ToTalk("Foo:Bar"));
            Assert.AreEqual("Wikipedia talk:Foo", ToTalk("Wikipedia:Foo"));
            Assert.AreEqual("File talk:Foo bar", ToTalk("File:Foo bar"));
            Assert.AreEqual("File talk:Foo bar", ToTalk("File talk:Foo bar"));

            // Don't choke on special namespaces
            Assert.AreEqual("Special:Foo", ToTalk("Special:Foo"));
            Assert.AreEqual("Media:Bar", ToTalk("Media:Bar"));

            // current namespace detection sucks, must be tested elsewhere
            //Assert.AreEqual("Wikipedia talk:Foo", ToTalk("Project:Foo"));
            //Assert.AreEqual("Image talk:Foo bar", ToTalk("Image:Foo bar"));
            //Assert.AreEqual("Image talk:Foo bar", ToTalk("Image talk:Foo bar"));
        }

        [Test]
        public void ToTalkOnList()
        {
            List<Article> l = new List<Article>
            {
                new Article("Foo"),
                new Article("Talk:Foo bar"),
                new Article("File:Foo"),
                new Article("Special:Foo")
            };
            CollectionAssert.AreEquivalent(Tools.ConvertToTalk(l),
                                           new[] { "Talk:Foo", "Talk:Foo bar", "File talk:Foo", "Special:Foo" });
        }

        [Test]
        public void ConvertFromTalk()
        {
            Assert.AreEqual("Foo", FromTalk("Talk:Foo"));
            Assert.AreEqual("Foo", FromTalk("Foo"));
            Assert.AreEqual("Foo:Bar", FromTalk("Foo:Bar"));
            Assert.AreEqual("Foo:Bar", FromTalk("Talk:Foo:Bar"));
            Assert.AreEqual("User:Foo bar", FromTalk("User:Foo bar"));
            Assert.AreEqual("Category:Foo bar", FromTalk("Category talk:Foo bar"));

            // Don't choke on special namespaces
            Assert.AreEqual("Special:Foo", FromTalk("Special:Foo"));
            Assert.AreEqual("Media:Bar", FromTalk("Media:Bar"));
        }

        [Test]
        public void FromTalkOnList()
        {
            List<Article> l = new List<Article>
            {
                new Article("Foo"),
                new Article("Talk:Foo bar"),
                new Article("User talk:Foo"),
                new Article("Special:Foo")
            };
            CollectionAssert.AreEquivalent(Tools.ConvertFromTalk(l),
                                           new[] { "Foo", "Foo bar", "User:Foo", "Special:Foo" });
        }
        #endregion
    }
}
