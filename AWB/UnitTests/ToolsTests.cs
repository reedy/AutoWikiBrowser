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

        [Test, Category("Incomplete")] //TODO: address the need of escaped string as argument
        public void CaseInsensitive()
        {
            Assert.AreEqual("", Tools.CaseInsensitive(""));
            Assert.AreEqual("123", Tools.CaseInsensitive("123"));
            Assert.AreEqual("-", Tools.CaseInsensitive("-"));

            Regex r = new Regex(Tools.CaseInsensitive("test"));
            Assert.IsTrue(r.IsMatch("test 123"));
            Assert.AreEqual("Test", r.Match("Test").Value);
            Assert.IsFalse(r.IsMatch("tEst"));

            r = new Regex(Tools.CaseInsensitive("Test"));
            Assert.IsTrue(r.IsMatch("test 123"));
            Assert.AreEqual("Test", r.Match("Test").Value);
            Assert.IsFalse(r.IsMatch("TEst"));

            //behavior changed
            //r = new Regex(Tools.CaseInsensitive("[test}"));
            //Assert.IsTrue(r.IsMatch("[test}"));
            //Assert.IsFalse(r.IsMatch("[Test}"));
            //Assert.IsFalse(r.IsMatch("test"));
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

        [Test, Ignore("Too slow")]
        public void TurnFirstToUpper()
        {
            Assert.AreEqual("", Tools.TurnFirstToUpper(""));
            Assert.AreEqual("ASDA", Tools.TurnFirstToUpper("ASDA"));
            Assert.AreEqual("ASDA", Tools.TurnFirstToUpper("aSDA"));
            Assert.AreEqual("Test", Tools.TurnFirstToUpper("test"));
            Assert.AreEqual("%test", Tools.TurnFirstToUpper("%test"));
            Assert.AreEqual("Ыыыы", Tools.TurnFirstToUpper("ыыыы"));

            Variables.SetProject("en", ProjectEnum.wiktionary);
            Assert.AreEqual("test", Tools.TurnFirstToUpper("test"));
            Assert.AreEqual("Test", Tools.TurnFirstToUpper("Test"));
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
        public void WordCount()
        {
            Assert.AreEqual(0, Tools.WordCount(""));
            Assert.AreEqual(0, Tools.WordCount("    "));
            Assert.AreEqual(0, Tools.WordCount("."));

            Assert.AreEqual(1, Tools.WordCount("1"));
            Assert.AreEqual(1, Tools.WordCount("  1 "));
            Assert.AreEqual(1, Tools.WordCount("foo"));
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
            var foo = new Regex("foo");
            Assert.AreEqual("", Tools.ReplaceWithSpaces("", foo));
            Assert.AreEqual("   ", Tools.ReplaceWithSpaces("foo", foo));
            Assert.AreEqual("   bar   ", Tools.ReplaceWithSpaces("foobarfoo", foo));
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
        public void ISOToDate()
        {
            string iso = @"2009-06-11", iso2 = @"1890-07-04";
            Assert.AreEqual(@"11 June 2009", Tools.ISOToENDate(iso, Parsers.DateLocale.International));
            Assert.AreEqual(@"June 11, 2009", Tools.ISOToENDate(iso, Parsers.DateLocale.American));
            Assert.AreEqual(iso, Tools.ISOToENDate(iso, Parsers.DateLocale.ISO));
            Assert.AreEqual(iso, Tools.ISOToENDate(iso, Parsers.DateLocale.Undetermined));
            Assert.AreEqual(@"4 July 1890", Tools.ISOToENDate(iso2, Parsers.DateLocale.International));

            // handles incorect format
            string wrong = @"foo";
            Assert.AreEqual(wrong, Tools.ISOToENDate(wrong, Parsers.DateLocale.International));
        }

        [Test, SetCulture("ru-RU")]
        public void ISOToDateOtherCulture()
        {
            // if user's computer has non-en culture, ISOToENDate should still work
            string iso = @"2009-06-11";
            Assert.AreEqual(@"11 June 2009", Tools.ISOToENDate(iso, Parsers.DateLocale.International));
        }
        
        
        [Test]
        public void AppendParameterToTemplate()
        {
            Assert.AreEqual(@"{{cite|title=abc | location=London}}", Tools.AppendParameterToTemplate(@"{{cite|title=abc}}", "location", "London"));
            Assert.AreEqual(@"{{cite|title=abc | location=London}}", Tools.AppendParameterToTemplate(@"{{cite|title=abc }}", "location", "London"));
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
        	
        	Assert.AreEqual(@"here [[piped|link]]", Tools.GetTemplateParameterValue(@"{{cite|param1 = here [[piped|link]]}}", "param1"));
        	Assert.AreEqual(@"[[piped|link]], [[another|piped link]] here", Tools.GetTemplateParameterValue(@"{{cite|param1 = [[piped|link]], [[another|piped link]] here}}", "param1"));
        	
        	// not found
        	Assert.AreEqual("", Tools.GetTemplateParameterValue(@"{{cite|param2=here}}", "param1"));
        	
        	// null value
        	Assert.AreEqual("", Tools.GetTemplateParameterValue(@"{{cite|param1= }}", "param1"));
        	
        	// returns first value
        	Assert.AreEqual("here", Tools.GetTemplateParameterValue(@"{{cite|param1=here|foo=bar|param1=there}}", "param1"));
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
        }
        
        [Test]
        public void RenameTemplate()
        {
            string correct = @"Now {{bar}} was {{bar|here}} there", correct2 = @"Now {{bar}} was {{bar
|here
|other}} there";
            Assert.AreEqual(correct, Tools.RenameTemplate(@"Now {{foo}} was {{foo|here}} there", "foo", "bar"));
            Assert.AreEqual(correct, Tools.RenameTemplate(@"Now {{Foo}} was {{foo|here}} there", "Foo", "bar"));
            
             Assert.AreEqual(correct2, Tools.RenameTemplate(@"Now {{foo}} was {{foo
|here
|other}} there", "Foo", "bar"));
            
            Assert.AreEqual(correct, Tools.RenameTemplate(correct, "bar2", "foo"));
        }
        
        [Test]
        public void RemoveTemplateParameter()
        {
            string correct = @"{{cite web|url=http://www.site.com |title=here |year=2008 }}";
            
            Assert.AreEqual(correct, Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat=mdy}}", "cite web", "dateformat"));
            Assert.AreEqual(correct, Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat=mdy }}", "cite web", "dateformat"));
            Assert.AreEqual(correct, Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat=}}", "cite web", "dateformat"));
            Assert.AreEqual(correct, Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |year=2008 | dateformat =   mdy}}", "cite web", "dateformat"));
            Assert.AreEqual(correct, Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here | dateformat=mdy|year=2008 }}", "cite web", "dateformat"));
            
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
    }

    [TestFixture]
    public class HumanCatKeyTests : RequiresInitialization
    {
        [Test]
        public void OneWordNames()
        {
            Assert.AreEqual("Onewordname", Tools.MakeHumanCatKey("OneWordName"));
            Assert.AreEqual("Onewordname", Tools.MakeHumanCatKey("ONEWORDNAME"));
            Assert.AreEqual("Onewordname", Tools.MakeHumanCatKey("Onewordname"));
            Assert.AreEqual("Onewordname", Tools.MakeHumanCatKey("onewordname"));
        }

        [Test]
        public void WithRomanNumbers()
        {
            Assert.AreEqual("Doe, John, Iii", Tools.MakeHumanCatKey("John Doe III"));
            Assert.AreEqual("Xvii", Tools.MakeHumanCatKey("XVII"));
        }

        [Test]
        public void WithJrSr()
        {
            Assert.AreEqual("Doe, John, Jr.", Tools.MakeHumanCatKey("John Doe, Jr."));
            Assert.AreEqual("Doe, John, Sr.", Tools.MakeHumanCatKey("John Doe, Sr."));
        }

        [Test]
        public void WithApostrophes()
        {
            Assert.AreEqual("Ddoe, John", Tools.MakeHumanCatKey("J'ohn D'Doe"));
            Assert.AreEqual("Test", Tools.MakeHumanCatKey("'Test"));
        }

        [Test]
        public void WithPrefixes()
        {
            Assert.AreEqual("Doe, John De", Tools.MakeHumanCatKey("John de Doe"));
        }

        [Test]
        public void MakeHumanCatKey()
        {
            Assert.AreEqual("Doe", Tools.MakeHumanCatKey("Ďöê"));
            Assert.AreEqual("Doe, John", Tools.MakeHumanCatKey("Ĵǒħń Ďöê"));

            Assert.AreEqual(@"Gu, Prince Imperial Hoeun", Tools.MakeHumanCatKey("Gu, Prince Imperial Hoeun"));

            // Ё should be changed, but not Й
            Assert.AreEqual("Епрстий", Tools.MakeHumanCatKey("Ёпрстий"));
        }

        [Test]
        public void ArabicNames()
        {
            Assert.AreEqual(@"Ahmed Mohammed Mukit", Tools.MakeHumanCatKey(@"Ahmed Mohammed Mukit"));
            Assert.AreEqual(@"Ahmed Mohammed Mukit", Tools.MakeHumanCatKey(@"AHMED Mohammed MUKIT"));
            Assert.AreEqual(@"Ahmed Mohammed Mukit", Tools.MakeHumanCatKey(@"ahmed Mohammed mukit"));
            
            Assert.AreEqual(@"Smith, John", Tools.MakeHumanCatKey(@"John Smith"));
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
