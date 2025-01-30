using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using WikiFunctions;
using WikiFunctions.Parse;

namespace UnitTests
{
    [TestFixture]
    public class ToolsTests : RequiresInitialization
    {
        [Test]
        public void IsValidTitle()
        {
            ClassicAssert.IsTrue(Tools.IsValidTitle("test"));
            ClassicAssert.IsTrue(Tools.IsValidTitle("This is a_test"));
            ClassicAssert.IsTrue(Tools.IsValidTitle("123"));
            ClassicAssert.IsTrue(Tools.IsValidTitle("А & Б сидели на трубе! ة日?"));

            ClassicAssert.IsFalse(Tools.IsValidTitle(""), "Empty strings are not supposed to be valid titles");
            ClassicAssert.IsFalse(Tools.IsValidTitle(" "));
            ClassicAssert.IsFalse(Tools.IsValidTitle("%20"));
            ClassicAssert.IsFalse(Tools.IsValidTitle("_"));

            ClassicAssert.IsFalse(Tools.IsValidTitle("[xxx"));
            ClassicAssert.IsFalse(Tools.IsValidTitle("]abc"));
            ClassicAssert.IsFalse(Tools.IsValidTitle("{duh!"));
            ClassicAssert.IsFalse(Tools.IsValidTitle("}yoyo"));
            ClassicAssert.IsFalse(Tools.IsValidTitle("|pwn3d"));
            ClassicAssert.IsFalse(Tools.IsValidTitle("<1337"));
            ClassicAssert.IsFalse(Tools.IsValidTitle(">nooooo"));
            ClassicAssert.IsFalse(Tools.IsValidTitle("#yeee-hooo"));

            // Complex titles
            ClassicAssert.IsFalse(Tools.IsValidTitle("[test]#1"));
            ClassicAssert.IsFalse(Tools.IsValidTitle("_ _"), "Titles should be normalised before checking");
            ClassicAssert.IsTrue(Tools.IsValidTitle("http://www.wikipedia.org")); // unfortunately
            ClassicAssert.IsTrue(Tools.IsValidTitle("index.php/Viagra")); // even more unfortunately
            ClassicAssert.IsTrue(Tools.IsValidTitle("index.php?title=foobar"));

            ClassicAssert.IsFalse(Tools.IsValidTitle("::Foo"));
            ClassicAssert.IsFalse(Tools.IsValidTitle("User:"));
            ClassicAssert.IsFalse(Tools.IsValidTitle("User::"));
            ClassicAssert.IsFalse(Tools.IsValidTitle("User::Foo"));
            ClassicAssert.IsTrue(Tools.IsValidTitle(":Foo"));
            ClassicAssert.IsTrue(Tools.IsValidTitle(":User:Foo"));
        }

        [Test]
        public void RemoveInvalidChars()
        {
            Assert.That(Tools.RemoveInvalidChars("tesT 123!"), Is.EqualTo("tesT 123!"));
            Assert.That(Tools.RemoveInvalidChars("тест, ёпта"), Is.EqualTo("тест, ёпта"));
            Assert.That(Tools.RemoveInvalidChars(""), Is.Empty);
            Assert.That(Tools.RemoveInvalidChars("{<[test]>}"), Is.EqualTo("test"));
            Assert.That(Tools.RemoveInvalidChars("#|#"), Is.Empty);
            Assert.That(Tools.RemoveInvalidChars("http://www.wikipedia.org"), Is.EqualTo("http://www.wikipedia.org"));
        }

        [Test]
        public void RomanNumbers()
        {
            ClassicAssert.IsTrue(Tools.IsRomanNumber("XVII"));
            ClassicAssert.IsTrue(Tools.IsRomanNumber("I"));
            ClassicAssert.IsTrue(Tools.IsRomanNumber("LI"));
            ClassicAssert.IsTrue(Tools.IsRomanNumber("LXXXIII"));
            ClassicAssert.IsTrue(Tools.IsRomanNumber("CCLXXXIII"));

            ClassicAssert.IsFalse(Tools.IsRomanNumber("xvii"));
            ClassicAssert.IsFalse(Tools.IsRomanNumber("V II"));
            ClassicAssert.IsFalse(Tools.IsRomanNumber("AAA"));
            ClassicAssert.IsFalse(Tools.IsRomanNumber("123"));
            ClassicAssert.IsFalse(Tools.IsRomanNumber(" "));
            ClassicAssert.IsFalse(Tools.IsRomanNumber(""));
        }

        [Test]
        public void FirstLetterCaseInsensitive()
        {
            // standard cases
            Assert.That(Tools.FirstLetterCaseInsensitive("Abc"), Is.EqualTo(@"[Aa]bc"));
            Assert.That(Tools.FirstLetterCaseInsensitive("abc"), Is.EqualTo(@"[Aa]bc"));
            Assert.That(Tools.FirstLetterCaseInsensitive("aBC"), Is.EqualTo(@"[Aa]BC"));
            Assert.That(Tools.FirstLetterCaseInsensitive("abc[de]"), Is.EqualTo(@"[Aa]bc[de]"));
            Assert.That(Tools.FirstLetterCaseInsensitive("Σbc"), Is.EqualTo(@"[Σσ]bc"));

            // trimming
            Assert.That(Tools.FirstLetterCaseInsensitive("abc "), Is.EqualTo(@"[Aa]bc"));

            // no changes
            Assert.That(Tools.FirstLetterCaseInsensitive(" abc"), Is.EqualTo(@" abc"));
            Assert.That(Tools.FirstLetterCaseInsensitive(""), Is.Empty);
            Assert.That(Tools.FirstLetterCaseInsensitive("123"), Is.EqualTo("123"));
            Assert.That(Tools.FirstLetterCaseInsensitive("-"), Is.EqualTo("-"));
            Assert.That(Tools.FirstLetterCaseInsensitive(@"[Aa]bc"), Is.EqualTo(@"[Aa]bc"));

            Regex r = new Regex(Tools.FirstLetterCaseInsensitive("test"));
            ClassicAssert.IsTrue(r.IsMatch("test 123"));
            Assert.That(r.Match("Test").Value, Is.EqualTo("Test"));
            ClassicAssert.IsFalse(r.IsMatch("tEst"));

            r = new Regex(Tools.FirstLetterCaseInsensitive("Test"));
            ClassicAssert.IsTrue(r.IsMatch("test 123"));
            Assert.That(r.Match("Test").Value, Is.EqualTo("Test"));
            ClassicAssert.IsFalse(r.IsMatch("TEst"));

            r = new Regex(Tools.FirstLetterCaseInsensitive("#test#"));
            ClassicAssert.IsTrue(r.IsMatch("#test#"));
            ClassicAssert.IsFalse(r.IsMatch("#Test#"));
            ClassicAssert.IsFalse(r.IsMatch("test"));
        }

        [Test]
        public void AllCaseInsensitive()
        {
            Assert.That(Tools.AllCaseInsensitive(""), Is.Empty);
            Assert.That(Tools.AllCaseInsensitive("123"), Is.EqualTo("123"));
            Assert.That(Tools.AllCaseInsensitive("-"), Is.EqualTo("-"));

            Regex r = new Regex(Tools.AllCaseInsensitive("tEsT"));
            ClassicAssert.IsTrue(r.IsMatch("Test 123"));
            Assert.That(r.Match("Test").Value, Is.EqualTo("Test"));
            ClassicAssert.IsFalse(r.IsMatch("teZt"));

            r = new Regex(Tools.AllCaseInsensitive("[test}"));
            ClassicAssert.IsTrue(r.IsMatch("[test}"));
            ClassicAssert.IsTrue(r.IsMatch("[tEsT}"));
            ClassicAssert.IsFalse(r.IsMatch("test"));
        }

        [Test]
        public void CaseInsensitiveStringCompare()
        {
            ClassicAssert.IsTrue(Tools.CaseInsensitiveStringCompare("test", "test"));
            ClassicAssert.IsTrue(Tools.CaseInsensitiveStringCompare("test", "TEST"));
            ClassicAssert.IsTrue(Tools.CaseInsensitiveStringCompare("TEST", "TEST"));
            ClassicAssert.IsTrue(Tools.CaseInsensitiveStringCompare("testDING", "TESTding"));
            ClassicAssert.IsTrue(Tools.CaseInsensitiveStringCompare("sCr1pTkIdDy", "sCr1pTkIdDy"));

            ClassicAssert.IsFalse(Tools.CaseInsensitiveStringCompare("test", "not a test"));
            ClassicAssert.IsFalse(Tools.CaseInsensitiveStringCompare("test ", " test"));
        }

        [Test]
        public void TurnFirstToUpper()
        {
            Assert.That(Tools.TurnFirstToUpper(""), Is.Empty);
            Assert.That(Tools.TurnFirstToUpper("ASDA"), Is.EqualTo("ASDA"));
            Assert.That(Tools.TurnFirstToUpper("aSDA"), Is.EqualTo("ASDA"));
            Assert.That(Tools.TurnFirstToUpper("test"), Is.EqualTo("Test"));
            Assert.That(Tools.TurnFirstToUpper("%test"), Is.EqualTo("%test"));
            Assert.That(Tools.TurnFirstToUpper("ыыыы"), Is.EqualTo("Ыыыы"));
        }

        [Test]
        public void TurnFirstToUpperCapitalizeFirstLetter()
        {
            Variables.CapitalizeFirstLetter = false;
            Assert.That(Tools.TurnFirstToUpper("test"), Is.EqualTo("test"));
            Assert.That(Tools.TurnFirstToUpper("Test"), Is.EqualTo("Test"));

            Variables.CapitalizeFirstLetter = true;
            Assert.That(Tools.TurnFirstToUpper("test"), Is.EqualTo("Test"));
        }

        [Test]
        public void FirstToUpperAndRemoveHashOnArray()
        {
            string[] a = new string[2];
            a[0] = "Abc#f";
            a[1] = "efh ";

            string[] b = new string[2];
            b[0] = "Abc";
            b[1] = "Efh";

            Assert.That(Tools.FirstToUpperAndRemoveHashOnArray(a), Is.EqualTo(b));
            a[1] = "[[Efh]]";
            Assert.That(Tools.FirstToUpperAndRemoveHashOnArray(a), Is.EqualTo(b));
            Assert.That(Tools.FirstToUpperAndRemoveHashOnArray(null), Is.EqualTo(null));
        }

        [Test]
        public void TurnFirstToLower()
        {
            Assert.That(Tools.TurnFirstToLower(""), Is.Empty);
            Assert.That(Tools.TurnFirstToLower("test"), Is.EqualTo("test"));
            Assert.That(Tools.TurnFirstToLower("%test"), Is.EqualTo("%test"));
            Assert.That(Tools.TurnFirstToLower("Ыыыы"), Is.EqualTo("ыыыы"));

            Assert.That(Tools.TurnFirstToLower("TEST"), Is.EqualTo("tEST"));
            Assert.That(Tools.TurnFirstToLower("Test"), Is.EqualTo("test"));
        }

        [Test]
        public void TitleCaseEN()
        {
            Assert.That(Tools.TitleCaseEN("FOO"), Is.EqualTo(@"Foo"));
            Assert.That(Tools.TitleCaseEN("mastermind's interrogation"), Is.EqualTo(@"Mastermind's Interrogation"));

            Assert.That(Tools.TitleCaseEN(" FOO "), Is.EqualTo(@"Foo"));
            Assert.That(Tools.TitleCaseEN("FOO BAR"), Is.EqualTo(@"Foo Bar"));
            Assert.That(Tools.TitleCaseEN("FOO-BAR"), Is.EqualTo(@"Foo-Bar"));
            Assert.That(Tools.TitleCaseEN("FOO~BAR"), Is.EqualTo(@"Foo~Bar"));
            Assert.That(Tools.TitleCaseEN("foo bar"), Is.EqualTo(@"Foo Bar"));
            Assert.That(Tools.TitleCaseEN("foo Bar"), Is.EqualTo(@"Foo Bar"));
            Assert.That(Tools.TitleCaseEN("FOO BAR a"), Is.EqualTo(@"FOO BAR A"), "Not all text uppercase");

            Assert.That(Tools.TitleCaseEN("JANE E"), Is.EqualTo(@"Jane E"), "doesn't reformat initials");
            Assert.That(Tools.TitleCaseEN("JANE E."), Is.EqualTo(@"Jane E."), "doesn't reformat initials");
        }

        [Test]
        public void WordCount()
        {
            Assert.That(Tools.WordCount(""), Is.EqualTo(0));
            Assert.That(Tools.WordCount("    "), Is.EqualTo(0));
            Assert.That(Tools.WordCount("."), Is.EqualTo(0));

            Assert.That(Tools.WordCount("1"), Is.EqualTo(1));
            Assert.That(Tools.WordCount("  1 "), Is.EqualTo(1));
            Assert.That(Tools.WordCount("foo"), Is.EqualTo(1));
            Assert.That(Tools.WordCount("foo-bar"), Is.EqualTo(2));
            Assert.That(Tools.WordCount("Превед медвед"), Is.EqualTo(2));
            Assert.That(Tools.WordCount("123"), Is.EqualTo(1));
            Assert.That(Tools.WordCount("foo\nbar\r\nboz"), Is.EqualTo(3));
            Assert.That(Tools.WordCount("foo.bar"), Is.EqualTo(2));
            Assert.That(Tools.WordCount("foo.bar", 10), Is.EqualTo(2));
            Assert.That(Tools.WordCount("foo.bar", 1), Is.EqualTo(1));

            Assert.That(Tools.WordCount("foo<!-- bar boz -->"), Is.EqualTo(1));
            Assert.That(Tools.WordCount("foo<!--bar-->quux"), Is.EqualTo(1));
            Assert.That(Tools.WordCount("foo <!--\r\nbar--> quux"), Is.EqualTo(2));

            Assert.That(Tools.WordCount("foo {{template| bar boz box}} quux"), Is.EqualTo(2));
            Assert.That(Tools.WordCount("foo{{template\r\n|boz = quux}}bar"), Is.EqualTo(2));

            Assert.That(Tools.WordCount(@"foo
{|
! test !! test
|-
| test
| test || test
|-
|}bar"), Is.EqualTo(2));

            Assert.That(Tools.WordCount(@"foo
{|
! test !! test
|-
| test
| test || test
|-
|}
bar"), Is.EqualTo(2));

            Assert.That(Tools.WordCount(@"foo
{|
! test !! {{test}}
|-
| test
| test || test
|-
|}
bar"), Is.EqualTo(2));

            Assert.That(Tools.WordCount(@"foo
{| class=""wikitable""
! test !! test
|- style=""color:red""
| test
| test || test
|-
|}"), Is.EqualTo(1));
        }

        [Test]
        public void LineEndings()
        {
            Assert.That(Tools.ConvertToLocalLineEndings(""), Is.Empty);
            Assert.That(Tools.ConvertToLocalLineEndings("foo bar"), Is.EqualTo("foo bar"));
            if (!Globals.UsingMono)
                Assert.That(Tools.ConvertToLocalLineEndings("\nfoo\nbar\n"), Is.EqualTo("\r\nfoo\r\nbar\r\n"));

            Assert.That(Tools.ConvertFromLocalLineEndings(""), Is.Empty);
            Assert.That(Tools.ConvertFromLocalLineEndings("foo bar"), Is.EqualTo("foo bar"));
            if (!Globals.UsingMono)
                Assert.That(Tools.ConvertFromLocalLineEndings("\r\nfoo\r\nbar\r\n"), Is.EqualTo("\nfoo\nbar\n"));
        }

        [Test]
        public void ReplacePartOfString()
        {
            Assert.That(Tools.ReplacePartOfString("abcdef", 3, 1, "123"), Is.EqualTo("abc123ef"));
            Assert.That(Tools.ReplacePartOfString("abc", 0, 0, "123"), Is.EqualTo("123abc"));
            Assert.That(Tools.ReplacePartOfString("abc", 3, 0, "123"), Is.EqualTo("abc123"));
            Assert.That(Tools.ReplacePartOfString("", 0, 0, "123"), Is.EqualTo("123"));
            Assert.That(Tools.ReplacePartOfString("abc", 1, 0, ""), Is.EqualTo("abc"));
            Assert.That(Tools.ReplacePartOfString("abc", 0, 3, "123"), Is.EqualTo("123"));
            Assert.That(Tools.ReplacePartOfString("abc", 0, 1, "1"), Is.EqualTo("1bc"));
            Assert.That(Tools.ReplacePartOfString("abc", 2, 1, "3"), Is.EqualTo("ab3"));
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
            Assert.That(ReplaceOnceStringBuilder("", "foo", "bar"), Is.Empty);
            Assert.That(ReplaceOnceStringBuilder("test foo!", "foo", "bar"), Is.EqualTo("test bar!"));
            Assert.That(ReplaceOnceStringBuilder("barbar", "bar", "foo"), Is.EqualTo("foobar"));

            Assert.That(ReplaceOnceString("", "foo", "bar"), Is.Empty);
            Assert.That(ReplaceOnceString("test foo!", "foo", "bar"), Is.EqualTo("test bar!"));
            Assert.That(ReplaceOnceString("barbar", "bar", "foo"), Is.EqualTo("foobar"));

            string y = @"Șen";
            Assert.That(ReplaceOnceString(y, y, "z"), Is.EqualTo("z"), "Handle combining diacritic letter");
        }

        [Test]
        public void BasePageName()
        {
            Assert.That(Tools.BasePageName(""), Is.Empty);
            Assert.That(Tools.BasePageName("Foo"), Is.EqualTo("Foo"));
            Assert.That(Tools.BasePageName("Project:Foo"), Is.EqualTo("Foo"));
            Assert.That(Tools.BasePageName("Foo/Bar"), Is.EqualTo("Foo"));
            Assert.That(Tools.BasePageName("Foo/Bar/Boz"), Is.EqualTo("Foo"));
            Assert.That(Tools.BasePageName("Project:Foo/Bar/Boz"), Is.EqualTo("Foo"));
        }

        [Test]
        public void SubPageName()
        {
            Assert.That(Tools.SubPageName(""), Is.Empty);
            Assert.That(Tools.SubPageName("Foo"), Is.EqualTo("Foo"));
            Assert.That(Tools.SubPageName("Foo/Bar"), Is.EqualTo("Bar"));
            Assert.That(Tools.SubPageName("Foo/Bar/Boz"), Is.EqualTo("Boz"));
            Assert.That(Tools.SubPageName("Project:Foo"), Is.EqualTo("Foo"));
            Assert.That(Tools.SubPageName("Image:Foo/Bar"), Is.EqualTo("Bar"));
        }

        [Test]
        public void ServerName()
        {
            Assert.That(Tools.ServerName("http://foo"), Is.EqualTo("foo"));
            Assert.That(Tools.ServerName("http://foo/"), Is.EqualTo("foo"));
            Assert.That(Tools.ServerName("http://foo.bar.com/path/script?a=foo/bar"), Is.EqualTo("foo.bar.com"));
        }

        [Test]
        public void WikiEncode()
        {
            Assert.That(Tools.WikiEncode("foo"), Is.EqualTo("foo"));
            Assert.That(Tools.WikiEncode("Foo"), Is.EqualTo("Foo"));
            Assert.That(Tools.WikiEncode("foo bar"), Is.EqualTo("foo_bar"));
            Assert.That(Tools.WikiEncode("foo_bar"), Is.EqualTo("foo_bar"));
            Assert.That(Tools.WikiEncode("foo/bar"), Is.EqualTo("foo/bar"));
            Assert.That(Tools.WikiEncode("foo:bar"), Is.EqualTo("foo:bar"));
            Assert.That(Tools.WikiEncode("Café"), Is.EqualTo("Caf%C3%A9").IgnoreCase);
            Assert.That(Tools.WikiEncode("тест:тест"), Is.EqualTo("%D1%82%D0%B5%D1%81%D1%82:%D1%82%D0%B5%D1%81%D1%82").IgnoreCase);

            Assert.That(Tools.WikiEncode("foo+bar"), Is.EqualTo("foo%2bbar"));
        }

        [Test]
        public void WikiDecode()
        {
            Assert.That(Tools.WikiDecode("foo"), Is.EqualTo("foo"));
            Assert.That(Tools.WikiDecode("Foo"), Is.EqualTo("Foo"));
            Assert.That(Tools.WikiDecode("foo_bar"), Is.EqualTo("foo bar"));
            Assert.That(Tools.WikiDecode("foo bar"), Is.EqualTo("foo bar"));
            Assert.That(Tools.WikiDecode("foo/bar"), Is.EqualTo("foo/bar"));
            Assert.That(Tools.WikiDecode("foo:bar"), Is.EqualTo("foo:bar"));

            Assert.That(Tools.WikiDecode("foo+bar"), Is.EqualTo("foo+bar"));
            Assert.That(Tools.WikiDecode("foo%2bbar"), Is.EqualTo("foo+bar"));
        }

        [Test]
        public void SplitToSections()
        {
            string[] sections = Tools.SplitToSections("foo\r\n==bar=\r\nboo1\r\n\r\n= boz =\r\n==quux==");
            Assert.That(sections, Is.EqualTo(new[]
                                      {
                                          "foo\r\n",
                                          "==bar=\r\nboo1\r\n\r\n",
                                          "= boz =\r\n",
                                          "==quux==\r\n"
                                      }).AsCollection);

            sections = Tools.SplitToSections("==bar=\r\nboo2\r\n\r\n= boz =\r\n==quux==");
            Assert.That(sections, Is.EqualTo(new[]
                                      {
                                          "==bar=\r\nboo2\r\n\r\n",
                                          "= boz =\r\n",
                                          "==quux==\r\n"
                                      }).AsCollection);

            sections = Tools.SplitToSections("\r\n==bar=\r\nboo3\r\n\r\n= boz =\r\n==quux==");
            Assert.That(sections, Is.EqualTo(new[]
                                      {
                                          "\r\n",
                                          "==bar=\r\nboo3\r\n\r\n",
                                          "= boz =\r\n",
                                          "==quux==\r\n"
                                      }).AsCollection);

            sections = Tools.SplitToSections("\r\n==bar=\r\nboo4\r\n\r\n=== boz ===\r\n==quux==");
            Assert.That(sections, Is.EqualTo(new[]
                                      {
                                          "\r\n",
                                          "==bar=\r\nboo4\r\n\r\n",
                                          "=== boz ===\r\n",
                                          "==quux==\r\n"
                                      }).AsCollection);

            sections = Tools.SplitToSections("");
            Assert.That(sections, Is.EqualTo(new[] { "\r\n" }).AsCollection);

            sections = Tools.SplitToSections("==foo==");
            Assert.That(sections, Is.EqualTo(new[] { "==foo==\r\n" }).AsCollection);
        }

        [Test]
        public void GetZerothSection()
        {
            Assert.That(Tools.GetZerothSection("Hello" + "\r\n" + "==Heading=="), Is.EqualTo("Hello" + "\r\n"));
            Assert.That(Tools.GetZerothSection("Hello" + "\r\n" + "===Heading==="), Is.EqualTo("Hello" + "\r\n"));
            Assert.That(Tools.GetZerothSection("Hello" + "\r\n"), Is.EqualTo("Hello" + "\r\n"));
        }

        [Test]
        public void RemoveMatches()
        {
            MatchCollection matches = Regex.Matches("abc bce cde def", "[ce]");
            Assert.That(Tools.RemoveMatches("abc bce cde def", matches), Is.EqualTo("ab b d df"));

            matches = Regex.Matches("", "test");
            Assert.That(Tools.RemoveMatches("test", matches), Is.EqualTo("test"));
            Assert.That(Tools.RemoveMatches("abc", matches), Is.EqualTo("abc"));
            Assert.That(Tools.RemoveMatches("", matches), Is.Empty);

            matches = Regex.Matches("abc123", "(123|abc)");
            Assert.That(Tools.RemoveMatches("abc123", matches), Is.Empty);

            matches = Regex.Matches("test", "[Tt]est");
            Assert.That(Tools.RemoveMatches("test", matches), Is.Empty);

            List<Match> MatchesList = new List<Match>();

            Assert.That(Tools.RemoveMatches("test", MatchesList), Is.EqualTo("test"));

            matches = Regex.Matches("world abc then abc after that then", "abc");
            Assert.That(Tools.RemoveMatches("world abc then abc after that then", matches), Is.EqualTo("world  then  after that then"));
        }

        [Test]
        public void RemoveHashFromPageTitle()
        {
            Assert.That(Tools.RemoveHashFromPageTitle("ab c"), Is.EqualTo("ab c"));
            Assert.That(Tools.RemoveHashFromPageTitle("foo#bar"), Is.EqualTo("foo"));
            Assert.That(Tools.RemoveHashFromPageTitle("foo##bar#"), Is.EqualTo("foo"));
            Assert.That(Tools.RemoveHashFromPageTitle("foo#"), Is.EqualTo("foo"));
            Assert.That(Tools.RemoveHashFromPageTitle("#"), Is.Empty);
            Assert.That(Tools.RemoveHashFromPageTitle(""), Is.Empty);
        }

        [Test]
        public void DeduplicateList()
        {
            List<string> A = new List<string>();
            Assert.That(Tools.DeduplicateList(A), Is.EqualTo(A));
            A.Add("hello");
            Assert.That(Tools.DeduplicateList(A), Is.EqualTo(A));
            A.Add("hello2");
            Assert.That(Tools.DeduplicateList(A), Is.EqualTo(A));
            A.Add("hello");
            Assert.That(Tools.DeduplicateList(A).Count, Is.EqualTo(2));
        }

        [Test]
        public void SplitLines()
        {
            Assert.That(Tools.SplitLines(""), Is.Empty);

            string[] test = new[] { "foo" };
            Assert.That(Tools.SplitLines("foo"), Is.EqualTo(test).AsCollection);
            Assert.That(Tools.SplitLines("foo\r"), Is.EqualTo(test).AsCollection);
            Assert.That(Tools.SplitLines("foo\n"), Is.EqualTo(test).AsCollection);
            Assert.That(Tools.SplitLines("foo\r\n"), Is.EqualTo(test).AsCollection);

            test = new[] { "foo", "bar" };
            Assert.That(Tools.SplitLines("foo\r\nbar"), Is.EqualTo(test).AsCollection);
            Assert.That(Tools.SplitLines("foo\rbar"), Is.EqualTo(test).AsCollection);
            Assert.That(Tools.SplitLines("foo\rbar"), Is.EqualTo(test).AsCollection);

            test = new[] { "" };
            Assert.That(Tools.SplitLines("\n"), Is.EqualTo(test).AsCollection);
            Assert.That(Tools.SplitLines("\r\n"), Is.EqualTo(test).AsCollection);
            Assert.That(Tools.SplitLines("\r"), Is.EqualTo(test).AsCollection);

            test = new[] { "", "" };
            Assert.That(Tools.SplitLines("\n\n"), Is.EqualTo(test).AsCollection);
            Assert.That(Tools.SplitLines("\r\n\r\n"), Is.EqualTo(test).AsCollection);
            Assert.That(Tools.SplitLines("\r\r"), Is.EqualTo(test).AsCollection);

            test = new[] { "", "foo", "", "bar" };
            Assert.That(Tools.SplitLines("\r\nfoo\r\n\r\nbar"), Is.EqualTo(test).AsCollection);
            Assert.That(Tools.SplitLines("\rfoo\r\rbar"), Is.EqualTo(test).AsCollection);
            Assert.That(Tools.SplitLines("\nfoo\n\nbar"), Is.EqualTo(test).AsCollection);
        }

        [Test]
        public void FirstChars()
        {
            Assert.That(Tools.FirstChars("", 0), Is.Empty);
            Assert.That(Tools.FirstChars("", 3), Is.Empty);
            Assert.That(Tools.FirstChars("123", 0), Is.Empty);
            Assert.That(Tools.FirstChars("12", 2), Is.EqualTo("12"));
            Assert.That(Tools.FirstChars("12", 3), Is.EqualTo("12"));
            Assert.That(Tools.FirstChars("123", 2), Is.EqualTo("12"));
        }

        [Test]
        public void Newline()
        {
            Assert.That(Tools.Newline("foo"), Is.EqualTo("\r\n" + "foo"));
            Assert.That(Tools.Newline("foo", 1), Is.EqualTo("\r\n" + "foo"));
            Assert.That(Tools.Newline("foo", 2), Is.EqualTo("\r\n\r\n" + "foo"));

            Assert.That(Tools.Newline(""), Is.Empty);
            Assert.That(Tools.Newline("", 2), Is.Empty);
        }

        [Test]
        public void IsRedirect()
        {
            ClassicAssert.IsTrue(Tools.IsRedirect("#REDIRECT  [[Foo]]"));
            ClassicAssert.IsTrue(Tools.IsRedirect("#REDIRECT  [[Foo|bar]]"));
            ClassicAssert.IsTrue(Tools.IsRedirect("#redirecT[[:Foo]]"));
            ClassicAssert.IsTrue(Tools.IsRedirect("should work!\r\n#REDIRECT [[Foo]]"));

            ClassicAssert.IsFalse(Tools.IsRedirect("#REDIRECT you to [[Hell]]"));
            ClassicAssert.IsFalse(Tools.IsRedirect("REDIRECT [[Foo]]"));

            // https://en.wikipedia.org/w/index.php?title=Middleton_Lake&diff=246079011&oldid=240299146
            ClassicAssert.IsTrue(Tools.IsRedirect("#REDIRECT:[[Foo]]"));
            ClassicAssert.IsTrue(Tools.IsRedirect("#REDIRECT : [[Foo]]"));

            ClassicAssert.IsFalse(Tools.IsRedirect("<nowiki>#REDIRECT  [[Foo]]</nowiki>"));
        }

        [Test]
        public void IsRedirectOrSoftRedirect()
        {
            ClassicAssert.IsTrue(Tools.IsRedirectOrSoftRedirect("#REDIRECT  [[Foo]]"));
            ClassicAssert.IsTrue(Tools.IsRedirectOrSoftRedirect("#redirecT[[:Foo]]"));
            ClassicAssert.IsTrue(Tools.IsRedirectOrSoftRedirect("should work!\r\n#REDIRECT [[Foo]]"));
            ClassicAssert.IsTrue(Tools.IsRedirectOrSoftRedirect("{{soft redirect|Foo}}"));
            ClassicAssert.IsTrue(Tools.IsRedirectOrSoftRedirect("{{soft|Foo}}"));
            ClassicAssert.IsTrue(Tools.IsRedirectOrSoftRedirect("{{category redirect|Foo}}"));
            ClassicAssert.IsTrue(Tools.IsRedirectOrSoftRedirect("{{Interwiki redirect|Foo}}"));
            ClassicAssert.IsTrue(Tools.IsRedirectOrSoftRedirect("{{Userrename|Foo}}"));
            ClassicAssert.IsTrue(Tools.IsRedirectOrSoftRedirect("{{Commons category redirect|Foo}}"));
            ClassicAssert.IsTrue(Tools.IsRedirectOrSoftRedirect("{{Deprecated shortcut|Foo}}"));
            ClassicAssert.IsTrue(Tools.IsRedirectOrSoftRedirect("{{Wikisource redirect|Foo}}"));
            ClassicAssert.IsTrue(Tools.IsRedirectOrSoftRedirect("{{Double soft redirect|Foo}}"));

            ClassicAssert.IsFalse(Tools.IsRedirectOrSoftRedirect("{{software|Foo}}"));
        }

        [Test]
        public void RedirectTarget()
        {
            Assert.That(Tools.RedirectTarget("#redirect [[Foo]]"), Is.EqualTo("Foo"));
            Assert.That(Tools.RedirectTarget("#redirect [[Foo]] {{R from something}}"), Is.EqualTo("Foo"));
            Assert.That(Tools.RedirectTarget("#REDIRECT[[Foo]]"), Is.EqualTo("Foo"));
            Assert.That(Tools.RedirectTarget("#redirect[[:Foo bar ]]"), Is.EqualTo("Foo bar"));
            Assert.That(Tools.RedirectTarget("#redirect[[ :  Foo bar ]]"), Is.EqualTo("Foo bar"));
            Assert.That(Tools.RedirectTarget("{{delete}}\r\n#redirect [[Foo]]"), Is.EqualTo("Foo"));

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_6#AWB_follows_piped_redirects_to_an_invalid_page_title
            Assert.That(Tools.RedirectTarget("#REDIRECT [[Foo|bar]]"), Is.EqualTo("Foo"));

            // URL-decode targets
            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_7#Problem_with_redirects
            Assert.That(Tools.RedirectTarget("#REDIRECT[[Foo%2C_bar]]"), Is.EqualTo("Foo, bar"));
            Assert.That(Tools.RedirectTarget("#REDIRECT[[%D0%A5%D1%83%D0%B9]]"), Is.EqualTo("Хуй"));

            // https://en.wikipedia.org/w/index.php?title=Middleton_Lake&diff=246079011&oldid=240299146
            Assert.That(Tools.RedirectTarget("#REDIRECT:[[Foo]]"), Is.EqualTo("Foo"));
            Assert.That(Tools.RedirectTarget("#REDIRECT : [[Foo]]"), Is.EqualTo("Foo"));

            Assert.That(Tools.RedirectTarget("#REDIRECT [[Foo#bar]]"), Is.EqualTo("Foo#bar"), "redirect to section");
            Assert.That(Tools.RedirectTarget("#REDIRECT [[Foo#bar|other]]"), Is.EqualTo("Foo#bar"), "redirect to section, piped link");

            Assert.That(Tools.RedirectTarget("<nowiki>#REDIRECT  [[Foo]]</nowiki>"), Is.Empty);

            Assert.That(Tools.RedirectTarget(@"{{Refimprove|date=October 2008}}
{{Infobox Military Conflict
|conflict=1383–1385 Crisis
|image=
|caption=
|partof=
|date=April 1383 - October 1385
|place=[[Portugal]] and [[Crown of Castile|Castille]]
|result=Victory for John I of Portugal
|combatant1=[[File:PortugueseFlag1385.svg|22px]] [[Kingdom of Portugal]]&lt;br&gt;''Supported by:''&lt;br&gt;[[Image:England Arms 1405.svg|22px]] [[Kingdom of England]]
|combatant2=[[Image:Escudo Corona de Castilla.png|22px]] [[Crown of Castile]]&lt;br&gt;''Supported by:''&lt;br&gt;[[Image:France Ancient.svg|22px]] [[Kingdom of France]]
|commander1=[[John I of Portugal]]&lt;br&gt;[[Nuno Álvares Pereira]]
|commander2=[[John I of Castile]]&lt;br&gt;[[Fernando Sanchez de Tovar]]&lt;br&gt;[[Pedro Álvares Pereira]]
|strength1=
|strength2=
|casualties1=
|casualties2=
|
}}
{{Campaignbox Portuguese Crisis 1383}}


{{History of Portugal|
image=[[Image:AljubarrotaBattle.jpg|250px]]|
caption=Battle of Aljubarrota}}
The '''1383–1385 Crisis''' was a period of [[civil war]] in [[History of Portugal|Portuguese history]] that began with the [[death]] of [[King of Portugal|King]] [[Ferdinand I of Portugal]], who left no male [[heir]]s, and ended with the accession to the [[throne]] of King [[John I of Portugal|John I]] in 1385, in the wake of the [[Battle of Aljubarrota]].

In Portugal, this period is also known as the &quot;Portuguese [[Interregnum]]&quot;, since it is a period when no crowned [[monarch|king]] reigned. The period is interpreted in Portuguese popular history as a Portuguese national &quot;resistance movement&quot; countering Castilian intervention, as the &quot;great revealer of national consciousness&quot;, as Robert Durand expressed it.&lt;ref&gt;Robert Durand, in ''Encyclopedia of the Middle Ages'' (Routledge, 2000), ''s.v.'' &quot;Portugal&quot;, p 1173; see also Armíndo de Sousa, &quot;Portugal&quot; in ''The New Cambridge Medieval History'' 2004, vol. II p. 629.&lt;/ref&gt; The role of the burgesses and nobles that established the Aviz dynasty securely on an independent throne can be contrasted with the centrifugal pull of aristocratic factions against a centralised monarchy in the English [[War of the Roses]] and with national and political aspects of the [[Hundred Years' War]] being waged in France.

==Prelude=="), Is.Empty);
        }

        [Test]
        public void GetTitleFromURL()
        {
            Assert.That(Tools.GetTitleFromURL("https://en.wikipedia.org/wiki/foo_bar"), Is.EqualTo("foo bar"));
            Assert.That(Tools.GetTitleFromURL("https://en.wikipedia.org/wiki/%D0%A5%D1%83%D0%B9"), Is.EqualTo("Хуй"));
            Assert.That(Tools.GetTitleFromURL("https://en.wikipedia.org/w/index.php?title=foo"), Is.EqualTo("foo"));
            Assert.That(Tools.GetTitleFromURL("https://en.wikipedia.org/w/index.php/foo"), Is.EqualTo("foo"));
            Assert.That(Tools.GetTitleFromURL("http://en.wikipedia.org/w/index.php/foo"), Is.EqualTo("foo"));

            // return null if there is something wrong
            ClassicAssert.IsNull(Tools.GetTitleFromURL(""));
            ClassicAssert.IsNull(Tools.GetTitleFromURL("foo"));
            ClassicAssert.IsNull(Tools.GetTitleFromURL("https://en.wikipedia.org"));
            ClassicAssert.IsNull(Tools.GetTitleFromURL("https://en.wikipedia.org/wiki/"));
            ClassicAssert.IsNull(Tools.GetTitleFromURL("https://en.wikipedia.org/w/index.php?title=foo&action=delete"));

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_7#list_entries_like:_Index.html.3Fcurid.3D16235168
            ClassicAssert.IsNull(Tools.GetTitleFromURL("https://en.wikipedia.org/wiki/index.html?curid=666"));
            ClassicAssert.IsNull(Tools.GetTitleFromURL("https://en.wikipedia.org/wiki/foo?action=delete"));
            ClassicAssert.IsNull(Tools.GetTitleFromURL("https://en.wikipedia.org/w/index.php?title=foo&action=delete"));
            ClassicAssert.IsNull(Tools.GetTitleFromURL("https://en.wikipedia.org/w/index.php/foo?action=bar"));
        }

        [Test]
        public void FirstDifference()
        {
            Assert.That(Tools.FirstDifference("a", "b"), Is.EqualTo(0));
            Assert.That(Tools.FirstDifference("", "a"), Is.EqualTo(0));
            Assert.That(Tools.FirstDifference("a", ""), Is.EqualTo(0));

            Assert.That(Tools.FirstDifference("aa", "ab"), Is.EqualTo(1));
            Assert.That(Tools.FirstDifference("ab", "aa"), Is.EqualTo(1));
            Assert.That(Tools.FirstDifference("foo", "foobar"), Is.EqualTo(3));
            Assert.That(Tools.FirstDifference("football", "foobar"), Is.EqualTo(3));

            // beyond the end
            Assert.That(Tools.FirstDifference("foo", "foo"), Is.EqualTo(3));
        }

        [Test]
        public void ApplyKeyWords()
        {
            // Test majority of Key Words except %%key%%
            Assert.That(Tools.ApplyKeyWords("Wikipedia talk:AutoWikiBrowser/Sandbox", @"%%pagename%%
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
%%namespace%%"),

                            Is.EqualTo(@"AutoWikiBrowser/Sandbox
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
Wikipedia talk"));

            // Date Stuff - disabled for now
            // Assert.AreEqual(DateTime.Now.Day.ToString() + "\r\n" +DateTime.Now.ToString("MMM") + "\r\n" +DateTime.Now.Year.ToString(), Tools.ApplyKeyWords("n/a", @"{{CURRENTDAY}}
            // {{CURRENTMONTHNAME}}
            // {{CURRENTYEAR}}"));

            // Server Stuff
            Assert.That(Tools.ApplyKeyWords("n/a", @"%%server%%
%%scriptpath%%
%%servername%%"), Is.EqualTo(@"https://en.wikipedia.org
/w
en.wikipedia.org"));

            // %%key%%, Tools.MakeHumanCatKey() - Covered by HumanCatKeyTests

            Assert.That(Tools.ApplyKeyWords("", ""), Is.Empty);
            Assert.That(Tools.ApplyKeyWords(@"%%foo%%", ""), Is.Empty);
            Assert.That(Tools.ApplyKeyWords("", @"%%foo%%"), Is.EqualTo(@"%%foo%%"));

            Assert.That(Tools.ApplyKeyWords(@"foo(bar)", "%%title%% was", true), Is.EqualTo(@"foo\(bar\) was"), "%%title%% escaped if requested");
            Assert.That(Tools.ApplyKeyWords(@"foo(bar)", "%%pagename%% was", true), Is.EqualTo(@"foo\(bar\) was"), "%%pagename%% escaped if requested");
            Assert.That(Tools.ApplyKeyWords(@"foo(bar)", "%%basepagename%% was", true), Is.EqualTo(@"foo\(bar\) was"), "%%basepagename%% escaped if requested");
            Assert.That(Tools.ApplyKeyWords(@"foo(bar)", "%%title%% was", false), Is.EqualTo(@"foo(bar) was"), "%%title%% not escaped if not requested");
        }

        [Test]
        public void IsWikimediaProject()
        {
            ClassicAssert.IsTrue(Tools.IsWikimediaProject(ProjectEnum.wikipedia));
            ClassicAssert.IsTrue(Tools.IsWikimediaProject(ProjectEnum.commons));
            ClassicAssert.IsTrue(Tools.IsWikimediaProject(ProjectEnum.meta));
            ClassicAssert.IsTrue(Tools.IsWikimediaProject(ProjectEnum.species));
            ClassicAssert.IsTrue(Tools.IsWikimediaProject(ProjectEnum.wikibooks));
            ClassicAssert.IsTrue(Tools.IsWikimediaProject(ProjectEnum.wikinews));
            ClassicAssert.IsTrue(Tools.IsWikimediaProject(ProjectEnum.wikiquote));
            ClassicAssert.IsTrue(Tools.IsWikimediaProject(ProjectEnum.wikisource));
            ClassicAssert.IsTrue(Tools.IsWikimediaProject(ProjectEnum.wikiversity));
            ClassicAssert.IsTrue(Tools.IsWikimediaProject(ProjectEnum.wikivoyage));
            ClassicAssert.IsTrue(Tools.IsWikimediaProject(ProjectEnum.wiktionary));
            ClassicAssert.IsTrue(Tools.IsWikimediaProject(ProjectEnum.mediawiki));

            ClassicAssert.IsFalse(Tools.IsWikimediaProject(ProjectEnum.custom));
            ClassicAssert.IsFalse(Tools.IsWikimediaProject(ProjectEnum.wikia));
            
        }

        [Test]
        public void OrdeOfWikimediaProjects()
        {
            // Be very causious if changing order or Wikimedia Projects
            ClassicAssert.IsTrue(ProjectEnum.commons > ProjectEnum.species);
            ClassicAssert.IsTrue(ProjectEnum.meta > ProjectEnum.commons);
            ClassicAssert.IsTrue(ProjectEnum.mediawiki > ProjectEnum.meta);
            ClassicAssert.IsTrue(ProjectEnum.incubator > ProjectEnum.mediawiki);
            ClassicAssert.IsTrue(ProjectEnum.wikia > ProjectEnum.incubator);
            ClassicAssert.IsTrue(ProjectEnum.custom > ProjectEnum.wikia);
        }

        [Test]
        public void StripNamespaceColon()
        {
            string s = Tools.StripNamespaceColon("User:");
            ClassicAssert.IsFalse(s.Contains(":"));

            s = Tools.StripNamespaceColon("Project:");
            ClassicAssert.IsFalse(s.Contains(":"));
        }

        [Test]
        public void RemoveNamespaceString()
        {
            Assert.That(Tools.RemoveNamespaceString("Test"), Is.EqualTo("Test"));
            Assert.That(Tools.RemoveNamespaceString("User:Test"), Is.EqualTo("Test"));
            Assert.That(Tools.RemoveNamespaceString("User talk:Test"), Is.EqualTo("Test"));
            Assert.That(Tools.RemoveNamespaceString("Category:Test"), Is.EqualTo("Test"));
        }

        [Test]
        public void GetNamespaceString()
        {
            Assert.That(Tools.GetNamespaceString("Test"), Is.Empty);
            Assert.That(Tools.GetNamespaceString("User:Test"), Is.EqualTo("User"));
            Assert.That(Tools.GetNamespaceString("User talk:Test"), Is.EqualTo("User talk"));
            Assert.That(Tools.GetNamespaceString("Category:Test"), Is.EqualTo("Category"));
            Assert.That(Tools.GetNamespaceString("Help:Test"), Is.EqualTo("Help"));
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

            Assert.That(res.Count, Is.EqualTo(2));

            foreach (Article a in res)
            {
                ClassicAssert.IsFalse(a.Name.StartsWith("Commons:"));
                Assert.That(a.NameSpaceKey, Is.Not.EqualTo(Namespace.MediaWiki));
                Assert.That(a.NameSpaceKey, Is.Not.EqualTo(Namespace.MediaWikiTalk));
                ClassicAssert.IsTrue(a.NameSpaceKey >= Namespace.Article);
            }
        }

        [Test]
        public void ReplaceWithSpacesTests()
        {
            Regex foo = new Regex("foo");
            Assert.That(Tools.ReplaceWithSpaces("", foo), Is.Empty);
            Assert.That(Tools.ReplaceWithSpaces("foo", foo), Is.EqualTo("   "));
            Assert.That(Tools.ReplaceWithSpaces("foobarfoo", foo), Is.EqualTo("   bar   "));
            Assert.That(Tools.ReplaceWithSpaces("foobarfoo", foo, 1), Is.EqualTo("   bar   "));

            foo = new Regex("f(o)o");
            Assert.That(Tools.ReplaceWithSpaces("foo", foo, 1), Is.EqualTo(" o "));
            Assert.That(Tools.ReplaceWithSpaces("foobarfoo", foo, 1), Is.EqualTo(" o bar o "));
        }

        [Test]
        public void ReplaceWithTests()
        {
            Regex foo = new Regex("foo");
            Assert.That(Tools.ReplaceWith("", foo, '#'), Is.Empty);
            Assert.That(Tools.ReplaceWith("foo", foo, '#'), Is.EqualTo("###"));
            Assert.That(Tools.ReplaceWith("foobarfoo", foo, '#'), Is.EqualTo("###bar###"));

            foo = new Regex("f(o)o");
            Assert.That(Tools.ReplaceWith("", foo, '#'), Is.Empty);
            Assert.That(Tools.ReplaceWith("foo", foo, '#'), Is.EqualTo("###"));
            Assert.That(Tools.ReplaceWith("foobarfoo", foo, '#'), Is.EqualTo("###bar###"));
            Assert.That(Tools.ReplaceWith("", foo, '#', 1), Is.Empty);
            Assert.That(Tools.ReplaceWith("foo", foo, '#', 1), Is.EqualTo("#o#"));
            Assert.That(Tools.ReplaceWith("foobarfoo", foo, '#', 1), Is.EqualTo("#o#bar#o#"));
        }

        [Test]
        public void HTMLListToWiki()
        {
            Assert.That(Tools.HTMLListToWiki("Fred", "*"), Is.EqualTo(@"* Fred"), "simple case");
            Assert.That(Tools.HTMLListToWiki("Fred" + "\r\n", "*"), Is.EqualTo(@"* Fred" + "\r\n"), "simple case, trailing newline");
            Assert.That(Tools.HTMLListToWiki(@"Fred
Jones", "*"), Is.EqualTo(@"* Fred
* Jones"), "Two entries");
            Assert.That(Tools.HTMLListToWiki("Fred<BR>", "*"), Is.EqualTo(@"* Fred"), "br handling");
            Assert.That(Tools.HTMLListToWiki("Fred<br/>", "*"), Is.EqualTo(@"* Fred"), "br handling");
            Assert.That(Tools.HTMLListToWiki("Fred", "#"), Is.EqualTo(@"# Fred"), "simple case #");
            Assert.That(Tools.HTMLListToWiki("<OL>Fred</OL>", "#"), Is.EqualTo(@"# Fred"), "ol handling");
            Assert.That(Tools.HTMLListToWiki("<li>Fred</li>", "#"), Is.EqualTo(@"# Fred"), "li handling");
            Assert.That(Tools.HTMLListToWiki(":Fred", "*"), Is.EqualTo(@"* Fred"), "trim colon");
            Assert.That(Tools.HTMLListToWiki("*Fred", "*"), Is.EqualTo(@"* Fred"), "already a list");
            Assert.That(Tools.HTMLListToWiki("#Fred Smith [[Foo#bar]]", "*"), Is.EqualTo(@"* Fred Smith [[Foo#bar]]"), "number to bullet conversion");

            Assert.That(Tools.HTMLListToWiki("Fred Smith:here", "*"), Is.EqualTo(@"* Fred Smith:here"), "normal colon retained");

            Assert.That(Tools.HTMLListToWiki(@"1 Fred
2 Jones", "*"), Is.EqualTo(@"* Fred
* Jones"), "number to bullet conversion 2 entries");

            Assert.That(Tools.HTMLListToWiki(@"11. Fred
12. Jones", "*"), Is.EqualTo(@"* Fred
* Jones"), "dot number conversion");

            Assert.That(Tools.HTMLListToWiki(@"(1) Fred
(2) Jones", "*"), Is.EqualTo(@"* Fred
* Jones"), "full bracket number conversion 1 digit");

            Assert.That(Tools.HTMLListToWiki(@"1) Fred
2) Jones", "*"), Is.EqualTo(@"* Fred
* Jones"), "bracket number conversion 1 digit");

            Assert.That(Tools.HTMLListToWiki(@"(11) Fred
(12) Jones", "*"), Is.EqualTo(@"* Fred
* Jones"), "bracket number conversion 2 digits");

            Assert.That(Tools.HTMLListToWiki(@"(998) Fred
(999) Jones", "*"), Is.EqualTo(@"* Fred
* Jones"), "bracket number conversion 3 digits");

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#Text_deleted_by_.22convert_list_to.22_.22.2A_list.22
            Assert.That(Tools.HTMLListToWiki(@"1980 Fred
2004 Jones", "*"), Is.EqualTo(@"* 1980 Fred
* 2004 Jones"));

            Assert.That(Tools.HTMLListToWiki(@"Fred
Tim
John", "*"), Is.EqualTo(@"* Fred
* Tim
* John"), "do not add list to blank lines/lines with just whitespace");

            Assert.That(Tools.HTMLListToWiki(@"Fred
 
Tim
 
John", "*"), Is.EqualTo(@"* Fred
 
* Tim
 
* John"), "do not add list to blank lines/lines with just whitespace 2");
        }

        [Test]
        public void RemoveSyntax()
        {
            Assert.That(Tools.RemoveSyntax(@""), Is.Empty);
            Assert.That(Tools.RemoveSyntax(@" "), Is.Empty);
            Assert.That(Tools.RemoveSyntax(@"* foo"), Is.EqualTo(@"foo"));
            Assert.That(Tools.RemoveSyntax(@"# foo"), Is.EqualTo(@"foo"));
            Assert.That(Tools.RemoveSyntax(@":foo"), Is.EqualTo(@"foo"));
            Assert.That(Tools.RemoveSyntax(@"foo_bar"), Is.EqualTo(@"foo bar"));
            Assert.That(Tools.RemoveSyntax(@"foo_bar:the great"), Is.EqualTo(@"foo bar:the great"));
            Assert.That(Tools.RemoveSyntax(@"[foo]"), Is.EqualTo(@"foo"));
            Assert.That(Tools.RemoveSyntax(@"[[foo]]"), Is.EqualTo(@"foo"));
            Assert.That(Tools.RemoveSyntax(@"#[[foo]]"), Is.EqualTo(@"foo"));
            Assert.That(Tools.RemoveSyntax(@"#[[ foo ]]"), Is.EqualTo(@"foo"));
            Assert.That(Tools.RemoveSyntax(@"foo&amp;bar"), Is.EqualTo(@"foo&bar"));
            Assert.That(Tools.RemoveSyntax(@"* [http://site.com words]"), Is.EqualTo(@"http://site.com words"));
            Assert.That(Tools.RemoveSyntax(@"* [https://site.com words]"), Is.EqualTo(@"https://site.com words"));
            Assert.That(Tools.RemoveSyntax(@"foo &quot;bar&quot; here"), Is.EqualTo(@"foo ""bar"" here"));
            Assert.That(Tools.RemoveSyntax(@"foo�bar"), Is.EqualTo(@"foobar"));

            Assert.That(Tools.RemoveSyntax(@"  * foo"), Is.EqualTo(@"foo"));
        }

        [Test]
        public void StringBetween()
        {
            Assert.That(Tools.StringBetween("now foo here", "now ", " here"), Is.EqualTo("foo"));
            Assert.That(Tools.StringBetween("now foo here foo", "now", " here"), Is.EqualTo(" foo"));

            Assert.That(Tools.StringBetween("now foo av", "now ", " here"), Is.Empty);

            // returns shortest matching string
            Assert.That(Tools.StringBetween("now foo here blah here", "now ", " here"), Is.EqualTo("foo"));
            Assert.That(Tools.StringBetween("now foo here now foo2 here", "now ", " here"), Is.EqualTo("foo"));
        }

        [Test]
        public void RegexMatchCount()
        {
            Assert.That(Tools.RegexMatchCount("a", "a"), Is.EqualTo(1));
            Assert.That(Tools.RegexMatchCount("\\w", "abcde"), Is.EqualTo(5));
            Assert.That(Tools.RegexMatchCount("a", "aAAa", RegexOptions.IgnoreCase), Is.EqualTo(4));
            Assert.That(Tools.RegexMatchCount("\\w+", "test case"), Is.EqualTo(2));
        }

        [Test]
        public void InterwikiCount()
        {
            SiteMatrix.Languages = new List<string> { "de", "es", "fr", "it", "sv" };

            Assert.That(Tools.InterwikiCount(@"now [[foo]] was great"), Is.EqualTo(0));
            Assert.That(Tools.LinkCount(@"now [[Template:foo]] was great"), Is.EqualTo(0));
            Assert.That(Tools.LinkCount(@"now [[Category:foo]] was great"), Is.EqualTo(0));

            Assert.That(Tools.InterwikiCount(@"now [[de:foo]] was great"), Is.EqualTo(1));
            Assert.That(Tools.InterwikiCount(@"now [[de:foo]] was great [[aa:now]] here"), Is.EqualTo(1));

            Assert.That(Tools.InterwikiCount(@"now [[de:foo]] was great [[aa:now]] here [[fr:bye]]"), Is.EqualTo(2));
        }

        [Test]
        public void LinkCountTests()
        {
            Assert.That(Tools.LinkCount(@"foo"), Is.EqualTo(0), "No links");
            Assert.That(Tools.LinkCount(@"[foo]"), Is.EqualTo(0), "broken links not counted");
            Assert.That(Tools.LinkCount(@"[[Image:foo.png]]"), Is.EqualTo(0), "Image links not counted");
            Assert.That(Tools.LinkCount(@"[[File:foo.png]]"), Is.EqualTo(0), "File links not counted");
            Assert.That(Tools.LinkCount(@"[[Template:Dn]]"), Is.EqualTo(0), "Templates not counted");
            Assert.That(Tools.LinkCount(@"now [[en:foo]] was great"), Is.EqualTo(1), "Interwikis are counted");
            Assert.That(Tools.LinkCount(@"[[foo]]"), Is.EqualTo(1));
            Assert.That(Tools.LinkCount(@"[[foo]] and [[foo]]"), Is.EqualTo(2), "counts repeated links");
            Assert.That(Tools.LinkCount(@"[[Image:foo.png]] and [[foo]]"), Is.EqualTo(1));
            Assert.That(Tools.LinkCount(@"[[foo]]s and [[barbie|bar]]"), Is.EqualTo(2), "counts piped links");
            Assert.That(Tools.LinkCount(@"{{flagIOC}}"), Is.EqualTo(1));
            Assert.That(Tools.LinkCount(@"{{speciesbox}}"), Is.EqualTo(1));
            Assert.That(Tools.LinkCount(@"{{automatic taxobox}}"), Is.EqualTo(1));
            Assert.That(Tools.LinkCount(@"now [[Magic: the gathering]] was great"), Is.EqualTo(1), "handles mainspace wikilink with colon");
            Assert.That(Tools.LinkCount(@"[[foo]]s and [[barbie|bar]] {{flagIOC}}"), Is.EqualTo(3), "counts flagIOC template as a link");

            Assert.That(Tools.LinkCount(@"[[foo]]s and [[barbie|bar]] and [[foo2]]", 1), Is.EqualTo(1), "count capped at limit");
            Assert.That(Tools.LinkCount(@"[[foo]]s and [[barbie|bar]] and [[foo2]]", 2), Is.EqualTo(2), "count capped at limit");
            Assert.That(Tools.LinkCount(@"{{flagIOC}} {{flagIOC}} {{flagIOC}}", 2), Is.EqualTo(2), "count capped at limit, flagIOC");

            Assert.That(Tools.LinkCount(@"[[Foo|ʿUrwa]]"), Is.EqualTo(1), "Handling of Unicode modifier letters");
        }

        [Test]
        public void DatesCount()
        {
            Dictionary<Parsers.DateLocale, int> results = new Dictionary<Parsers.DateLocale, int>();

            results.Add(Parsers.DateLocale.ISO, 1);
            results.Add(Parsers.DateLocale.International, 1);
            results.Add(Parsers.DateLocale.American, 0);

            Assert.That(Tools.DatesCount("2015-01-01 and 11 January 2010"), Is.EqualTo(results), "zero dates of a type reported");

            results.Clear();
            results.Add(Parsers.DateLocale.ISO, 1);
            results.Add(Parsers.DateLocale.International, 1);
            results.Add(Parsers.DateLocale.American, 1);

            Assert.That(Tools.DatesCount("2015-01-01 and 11 January 2010 and March 5, 2011"), Is.EqualTo(results), "each date type reported");
            Assert.That(Tools.DatesCount("2015-01-01 and| 11 January 2010 and| March 5, 2011"), Is.EqualTo(results), "each date type reported when split");
        
            results.Clear();
            results.Add(Parsers.DateLocale.ISO, 2);
            results.Add(Parsers.DateLocale.International, 1);
            results.Add(Parsers.DateLocale.American, 1);

            Assert.That(Tools.DatesCount("2015-01-01 and 2015-01-01 and 11 January and March 5"), Is.EqualTo(results), "Duplicate dates counted, short format matched");
        }

        [Test]
        public void DuplicateWikiLinks()
        {
            List<string> dupeWikiLinks = new List<string>();

            dupeWikiLinks.Add("Foo (2)");

            Assert.That(Tools.DuplicateWikiLinks(@"[[Foo]] [[Foo]]"), Is.EqualTo(dupeWikiLinks), "Simple case two plain links same case");
            Assert.That(Tools.DuplicateWikiLinks(@"[[foo]] [[foo]]"), Is.EqualTo(dupeWikiLinks), "Simple case two plain links same lower case");
            Assert.That(Tools.DuplicateWikiLinks(@"[[Foo]] [[Foo]] [[Bar]]"), Is.EqualTo(dupeWikiLinks), "Don't count single link");
            Assert.That(Tools.DuplicateWikiLinks(@"[[Foo|bar2]] [[Foo|bar]]"), Is.EqualTo(dupeWikiLinks), "Simple case two piped links same case");
            Assert.That(Tools.DuplicateWikiLinks(@"[[Foo]] [[foo]]"), Is.EqualTo(dupeWikiLinks), "convert first letter case for compare");
            Assert.That(Tools.DuplicateWikiLinks(@"[[ Foo |bar]] [[ Foo ]]"), Is.EqualTo(dupeWikiLinks), "Ignore excess whitespace");
            Assert.That(Tools.DuplicateWikiLinks(@"[[Foo]] [[Foo]] [[Bar]] [[Bar2]]"), Is.EqualTo(dupeWikiLinks), "Match on whole link name");
            Assert.That(Tools.DuplicateWikiLinks(@"[[Foo]] [[Foo]] [[|x]] [[|x]]"), Is.EqualTo(dupeWikiLinks), "Ignore targetless links");
            Assert.That(Tools.DuplicateWikiLinks(@"[[Foo]] [[Foo]] [[1 May]] [[1 May]]"), Is.EqualTo(dupeWikiLinks), "Ignore dates (int format)");
            Assert.That(Tools.DuplicateWikiLinks(@"[[Foo]] [[Foo]] [[May 1]] [[May 1]]"), Is.EqualTo(dupeWikiLinks), "Ignore dates (Am format)");

            dupeWikiLinks.Clear();
            dupeWikiLinks.Add("Foo (3)");

            Assert.That(Tools.DuplicateWikiLinks(@"[[Foo]] [[Foo]] [[Foo|bar]]"), Is.EqualTo(dupeWikiLinks), "Includes count after link name");
            Assert.That(Tools.DuplicateWikiLinks(@"[[Foo]] [[Foo]] [[Foo|bar]] <!-- [[Foo]] -->"), Is.EqualTo(dupeWikiLinks), "Ingore commented out link");

            dupeWikiLinks.Add("Get (3)");

            Assert.That(Tools.DuplicateWikiLinks(@"[[Foo]] [[Get]] [[Foo]] [[Get]] [[Foo|bar]] [[Get]]"), Is.EqualTo(dupeWikiLinks), "List returned is sorted");

            dupeWikiLinks.Clear();
            dupeWikiLinks.Add("Foo (2)");
            Assert.That(Tools.DuplicateWikiLinks(@"[[Foo|ʿUrwa]] [[Foo|ʿUrwa]]"), Is.EqualTo(dupeWikiLinks), "Handling of Unicode modifier letters");
        }

        [Test]
        public void ConvertDate()
        {
            string iso = @"2009-06-11", iso2 = @"1890-07-04";
            Assert.That(Tools.ConvertDate(iso, Parsers.DateLocale.International), Is.EqualTo(@"11 June 2009"));
            Assert.That(Tools.ConvertDate(iso, Parsers.DateLocale.International, false), Is.EqualTo(@"11 June 2009"));
            Assert.That(Tools.ConvertDate(iso, Parsers.DateLocale.International, true), Is.EqualTo(@"11 June 2009"));

            Assert.That(Tools.ConvertDate(iso, Parsers.DateLocale.American), Is.EqualTo(@"June 11, 2009"));
            Assert.That(Tools.ConvertDate(iso, Parsers.DateLocale.ISO), Is.EqualTo(iso));
            Assert.That(Tools.ConvertDate(iso, Parsers.DateLocale.Undetermined), Is.EqualTo(iso));
            Assert.That(Tools.ConvertDate(iso2, Parsers.DateLocale.International), Is.EqualTo(@"4 July 1890"));

            // handles incorect format
            string wrong = @"foo";
            Assert.That(Tools.ConvertDate(wrong, Parsers.DateLocale.International), Is.EqualTo(wrong));
            Assert.That(Tools.ConvertDate(@"2009-10", Parsers.DateLocale.International), Is.EqualTo(@"2009-10"), "day not added to year month combo");

            // supports other valid date formats
            Assert.That(Tools.ConvertDate("11 June 2009", Parsers.DateLocale.International), Is.EqualTo(@"11 June 2009"));
            Assert.That(Tools.ConvertDate("June 11, 2009", Parsers.DateLocale.International), Is.EqualTo(@"11 June 2009"));
            Assert.That(Tools.ConvertDate("June 11, 2009", Parsers.DateLocale.International, true), Is.EqualTo(@"11 June 2009"));

            Assert.That(Tools.ConvertDate("Jan 15 2008", Parsers.DateLocale.International), Is.EqualTo(@"15 January 2008"));
            Assert.That(Tools.ConvertDate("Jan 15 2008", Parsers.DateLocale.International, true), Is.EqualTo(@"15 January 2008"));
            Assert.That(Tools.ConvertDate("Jan 15 2008", Parsers.DateLocale.International, false), Is.EqualTo(@"15 January 2008"));
            Assert.That(Tools.ConvertDate("Jan 15 998", Parsers.DateLocale.International), Is.EqualTo(@"15 January 998"));

            const string AmericanDate = @"06/11/2009", UKDate = @"11/06/2009";

            Assert.That(Tools.ConvertDate(AmericanDate, Parsers.DateLocale.ISO, true), Is.EqualTo(iso), "Converts MM/DD/YYYY format dates when flagged");
            Assert.That(Tools.ConvertDate(UKDate, Parsers.DateLocale.ISO, false), Is.EqualTo(iso), "Converts DD/MM/YYYY format dates when flagged");
            Assert.That(Tools.ConvertDate(UKDate, Parsers.DateLocale.ISO), Is.EqualTo(iso), "Assumes DD/MM/YYYY format dates when NOT flagged");

            Assert.That(Tools.ConvertDate(@"May 2009", Parsers.DateLocale.International), Is.EqualTo(@"May 2009"), "day not added to month year combo");
            Assert.That(Tools.ConvertDate(@" May 2009", Parsers.DateLocale.International), Is.EqualTo(@"May 2009"), "day not added to month year combo");

            Assert.That(Tools.ConvertDate("2008-Jan-15", Parsers.DateLocale.International), Is.EqualTo(@"15 January 2008"));
            Assert.That(Tools.ConvertDate("2008 Jan 15", Parsers.DateLocale.International), Is.EqualTo(@"15 January 2008"));
            Assert.That(Tools.ConvertDate("2008 Jan. 15", Parsers.DateLocale.International), Is.EqualTo(@"15 January 2008"));
            Assert.That(Tools.ConvertDate("2008 January 15", Parsers.DateLocale.International), Is.EqualTo(@"15 January 2008"));
        }

        [Test]
        public void DateBeforeToday()
        {
            ClassicAssert.IsTrue(Tools.DateBeforeToday("11 May 2009"));
            ClassicAssert.IsTrue(Tools.DateBeforeToday("May 11, 2009"));
            ClassicAssert.IsTrue(Tools.DateBeforeToday("2013-12-31"));
            ClassicAssert.IsTrue(Tools.DateBeforeToday(DateTime.Now.AddDays(-1).ToString(CultureInfo.CurrentCulture)));

            ClassicAssert.IsFalse(Tools.DateBeforeToday(DateTime.Now.AddMonths(1).ToString(CultureInfo.CurrentCulture)));
            ClassicAssert.IsFalse(Tools.DateBeforeToday("foo"));
        }

        [Test]
        public void ConvertDateEnOnly()
        {
#if DEBUG
            string iso2 = @"1890-07-04";

            Variables.SetProjectLangCode("fr");
            Assert.That(Tools.ConvertDate(iso2, Parsers.DateLocale.International), Is.EqualTo(iso2));

            Variables.SetProjectLangCode("en");
            Assert.That(Tools.ConvertDate(iso2, Parsers.DateLocale.International), Is.EqualTo(@"4 July 1890"));
#endif
        }

        [Test, SetCulture("ru-RU")]
        public void ConvertDateOtherCulture()
        {
            // if user's computer has non-en culture, ISOToENDate should still work
            string iso = @"2009-06-11";
            Assert.That(Tools.ConvertDate(iso, Parsers.DateLocale.International), Is.EqualTo(@"11 June 2009"));
        }

        [Test]
        public void AppendParameterToTemplate()
        {
            Assert.That(Tools.AppendParameterToTemplate("", "location", "London"), Is.Empty);

            Assert.That(Tools.AppendParameterToTemplate(@"{{cite|title=abc}}", "location", "London"), Is.EqualTo(@"{{cite|title=abc | location=London}}"));
            Assert.That(Tools.AppendParameterToTemplate(@"{{cite|title=abc}}", "location", ""), Is.EqualTo(@"{{cite|title=abc | location=}}"));
            Assert.That(Tools.AppendParameterToTemplate(@"{{cite|title=abc }}", "location", "London"), Is.EqualTo(@"{{cite|title=abc | location=London }}"));
            Assert.That(Tools.AppendParameterToTemplate(@"{{cite|title=abc|last=a|first=b|date=2009-12-12}}", "location", "London"), Is.EqualTo(@"{{cite|title=abc|last=a|first=b|date=2009-12-12|location=London}}"), "no newlines/excess spaces in template");
            Assert.That(Tools.AppendParameterToTemplate(@"{{cite | title=abc | last=a | first=b | date=2009-12-12 }}", "location", "London"), Is.EqualTo(@"{{cite | title=abc | last=a | first=b | date=2009-12-12 | location=London }}"), "spaced parameters in template");
            Assert.That(Tools.AppendParameterToTemplate(@"{{cite | title = abc | last = a | first = b | date = 2009-11-11 }}", "location", "London"), Is.EqualTo(@"{{cite | title = abc | last = a | first = b | date = 2009-11-11 | location = London }}"), "=spaced parameters in template");
            Assert.That(Tools.AppendParameterToTemplate(@"{{cite | title= abc | last= a | first= b | date= 2009-11-11 }}", "location", "London"), Is.EqualTo(@"{{cite | title= abc | last= a | first= b | date= 2009-11-11 | location= London }}"), "= half spaced parameters in template");

            Assert.That(Tools.AppendParameterToTemplate(@"{{cite|title=abc}}", "location", "London", false), Is.EqualTo(@"{{cite|title=abc | location=London}}"));
            Assert.That(Tools.AppendParameterToTemplate(@"{{cite|title=abc}}", "location", "London", true), Is.EqualTo(@"{{cite|title=abc|location=London}}"));

            Assert.That(Tools.AppendParameterToTemplate(@"{{cite
|title=abc }}", "location", "London"), Is.EqualTo(@"{{cite
|title=abc | location=London }}"), "insufficient newlines – space used");

            Assert.That(Tools.AppendParameterToTemplate(@"{{cite
|title=abc
|date=2009-12-12 }}", "location", "London"), Is.EqualTo(@"{{cite
|title=abc
|date=2009-12-12 | location=London }}"), "insufficient newlines – space used");

            Assert.That(Tools.AppendParameterToTemplate(@"{{cite
|title=abc
|last=a
|first=b
|date=2009-12-12}}", "location", "London"), Is.EqualTo(@"{{cite
|title=abc
|last=a
|first=b
|date=2009-12-12
|location=London}}"), "newline set when > 2 existing");

            Assert.That(Tools.AppendParameterToTemplate(@"{{cite
|title=abc
|last=a
|first=b
|date=2009-12-12        }}", "location", "London"), Is.EqualTo(@"{{cite
|title=abc
|last=a
|first=b
|date=2009-12-12
|location=London }}"), "existing template end spaces cleaned");

            Assert.That(Tools.AppendParameterToTemplate(@"{{cite
|title=abc
|last=a
|first=b
|date=2009-12-12
}}", "location", "London"), Is.EqualTo(@"{{cite
|title=abc
|last=a
|first=b
|date=2009-12-12
|location=London
}}"), "template end on blank line");

            Assert.That(Tools.AppendParameterToTemplate(@"{{cite
  |title=abc
  |last=a
  |first=b
  |date=2009-12-12
}}", "location", "London"), Is.EqualTo(@"{{cite
  |title=abc
  |last=a
  |first=b
  |date=2009-12-12
  |location=London
}}"), "template with multiple spaces prior to bar, all params on newline");

            Assert.That(Tools.AppendParameterToTemplate(@"{{cite
 |title=abc
 |last=a
 |first=b
 |date=2009-12-12
}}", "location", "London"), Is.EqualTo(@"{{cite
 |title=abc
 |last=a
 |first=b
 |date=2009-12-12
 |location=London
}}"), "template with single prior to bar, all params on newline");

            Assert.That(Tools.AppendParameterToTemplate(@"{{cite journal
 | quotes =
 | doi = 10.2307/904183
 | id =
 | url = http://links.jstor.org/sici?sici=0027-4666(19060101)47%3A755%3C27%3AMSIC(J%3E2.0.CO%3B2-N
 | publisher = The Musical Times, Vol. 47, No. 755
 }}", "jstor", "904183"), Is.EqualTo(@"{{cite journal
 | quotes =
 | doi = 10.2307/904183
 | id =
 | url = http://links.jstor.org/sici?sici=0027-4666(19060101)47%3A755%3C27%3AMSIC(J%3E2.0.CO%3B2-N
 | publisher = The Musical Times, Vol. 47, No. 755
 | jstor = 904183
 }}"));
        }

        [Test]
        public void GetTemplateParameterValue()
        {
            Assert.That(Tools.GetTemplateParameterValue(@"{{cite|param1=here}}", "param1"), Is.EqualTo("here"));
            Assert.That(Tools.GetTemplateParameterValue(@"{{cite|param1 = here }}", "param1"), Is.EqualTo("here"));
            Assert.That(Tools.GetTemplateParameterValue(@"{{cite
| param1=here
|param=there}}", "param1"), Is.EqualTo("here"));
            Assert.That(Tools.GetTemplateParameterValue(@"{{cite|param1=here|foo=bar}}", "param1"), Is.EqualTo("here"));
            Assert.That(Tools.GetTemplateParameterValue(@"{{cite|param1 = [[1892]]-[[1893]]}}", "param1"), Is.EqualTo("[[1892]]-[[1893]]"));

            Assert.That(Tools.GetTemplateParameterValue(@"{{cite|param1=here {{t1|foo|bar}} there|foo=bar}}", "param1"), Is.EqualTo("here {{t1|foo|bar}} there"));
            Assert.That(Tools.GetTemplateParameterValue(@"{{cite|param1=here <!--comm|bar--> there|foo=bar}}", "param1"), Is.EqualTo("here <!--comm|bar--> there"));

            Assert.That(Tools.GetTemplateParameterValue(@"{{cite|param1 = here [[piped|link]]}}", "param1"), Is.EqualTo(@"here [[piped|link]]"));
            Assert.That(Tools.GetTemplateParameterValue(@"{{cite|param1 = [[piped|link]], [[another|piped link]] here}}", "param1"), Is.EqualTo(@"[[piped|link]], [[another|piped link]] here"));

            // not found
            Assert.That(Tools.GetTemplateParameterValue(@"{{cite|param2=here}}", "param1"), Is.Empty);

            Assert.That(Tools.GetTemplateParameterValue(@"{{cite|param1 {{test}} =here}}", "param1"), Is.Empty);
            Assert.That(Tools.GetTemplateParameterValue(@"{{cite|param1 [[test|2]] =here}}", "param1"), Is.Empty);
            Assert.That(Tools.GetTemplateParameterValue(@"{{cite|param1 <!--comm--> =here}}", "param1"), Is.EqualTo("here"));

            // null value
            Assert.That(Tools.GetTemplateParameterValue(@"{{cite|param1= }}", "param1"), Is.Empty);

            // comments handled
            Assert.That(Tools.GetTemplateParameterValue(@"{{cite|<!--param1=foo-->other=yes }}", "param1"), Is.Empty);
            Assert.That(Tools.GetTemplateParameterValue(@"{{cite|<!--param1=foo-->other=yes }}", "other"), Is.EqualTo("yes"));
            Assert.That(Tools.GetTemplateParameterValue(@"{{cite|other<!--param1=foo-->=yes }}", "other"), Is.EqualTo("yes"));

            Assert.That(Tools.GetTemplateParameterValue(@"{{cite|param1<!--comm--> = [[1892]]-[[1893]]}}", "param1"), Is.EqualTo("[[1892]]-[[1893]]"));
            Assert.That(Tools.GetTemplateParameterValue(@"{{cite|<!--comm-->param1 = [[1892]]-[[1893]]}}", "param1"), Is.EqualTo("[[1892]]-[[1893]]"));

            // returns first value
            Assert.That(Tools.GetTemplateParameterValue(@"{{cite|param1=here|foo=bar|param1=there}}", "param1"), Is.EqualTo("here"));

            // case insensitive option
            Assert.That(Tools.GetTemplateParameterValue(@"{{cite|PARAM1=here}}", "param1", true), Is.EqualTo("here"));

            // parameters with space in name
            Assert.That(Tools.GetTemplateParameterValue(@"{{cite|param 1=here}}", "param 1"), Is.EqualTo("here"));
            Assert.That(Tools.GetTemplateParameterValue(@"{{cite|param other=here}}", "param other"), Is.EqualTo("here"));
        }
        
        
        [Test]
        public void GetTemplateParameterValues()
        {
            Dictionary<string, string> Back = new Dictionary<string, string>();
            Back.Add("accessdate", "2012-05-15");
            Back.Add("title", "Hello");
            Back.Add("url", "http://www.site.com/abc");
            Back.Add("author", "");

            Assert.That(Tools.GetTemplateParameterValues(@"{{cite web|url=http://www.site.com/abc|title=Hello | author = | accessdate = 2012-05-15 }}"), Is.EqualTo(Back));
            Assert.That(Tools.GetTemplateParameterValues(@"{{cite web|url=http://www.site.com/abc|title=Hello |<!-- comm--> author = | accessdate = 2012-05-15 }}"), Is.EqualTo(Back), "ignores comments between parameters");
            Assert.That(Tools.GetTemplateParameterValues(@"{{cite web|url=http://www.site.com/abc|title=Hello | author <!-- comm--> = | accessdate = 2012-05-15 }}"), Is.EqualTo(Back), "ignores comments between parameters 2");
            Assert.That(Tools.GetTemplateParameterValues(@"{{cite web|url=http://www.site.com/abc|title=Hello |author =|accessdate = 2012-05-15 }}"), Is.EqualTo(Back));
            Assert.That(Tools.GetTemplateParameterValues(@"{{cite web|url=http://www.site.com/abc|title=Hello | author = | accessdate = 2012-05-15 | url=}}"), Is.EqualTo(Back), "ignores second parameter call (no value)");
            Assert.That(Tools.GetTemplateParameterValues(@"{{cite web|url=http://www.site.com/abc|title=Hello | author = | accessdate = 2012-05-15 | url=http://site2.com}}"), Is.EqualTo(Back), "ignores second parameter call (with value)");

            Back.Remove("author");
            Back.Add("author", "<!-- comm-->");
            Assert.That(Tools.GetTemplateParameterValues(@"{{cite web|url=http://www.site.com/abc|title=Hello | author = <!-- comm--> | accessdate = 2012-05-15 }}"), Is.EqualTo(Back), "Reports parameter value of comment");
            Assert.That(Tools.GetTemplateParameterValues(@"{{cite web|url=http://www.site.com/abc|title=Hello | accessdate = 2012-05-15 | author = <!-- comm--> }}"), Is.EqualTo(Back), "Reports parameter value of comment: last param");

            Back.Remove("author");
            Back.Add("author", "");
            Back.Add("format", "{{PDF|test}}");
            Assert.That(Tools.GetTemplateParameterValues(@"{{cite web|url=http://www.site.com/abc|title=Hello | author = | accessdate = 2012-05-15 | format={{PDF|test}} }}"), Is.EqualTo(Back), "handles nested templates");
            Back.Add("last1", "Jones");
            Assert.That(Tools.GetTemplateParameterValues(@"{{cite web|url=http://www.site.com/abc|title=Hello | author = | accessdate = 2012-05-15 | format={{PDF|test}} |last1=Jones}}"), Is.EqualTo(Back), "handles parameters with numbers");
            Back.Add("trans_title", @"Here 
There");
            Assert.That(Tools.GetTemplateParameterValues(@"{{cite web|url=http://www.site.com/abc|title=Hello | author = | accessdate = 2012-05-15 | format={{PDF|test}} |last1=Jones|trans_title=Here 
There}}"), Is.EqualTo(Back), "handles parameters with newlines");
            
            Back.Clear();
            Back.Add("ONE", "somevalue");
            Assert.That(Tools.GetTemplateParameterValues(@"{{test|ONE=somevalue}}"), Is.EqualTo(Back), "handles uppercase parameters");
            Back.Add("V_TWO", "some_value");
            Assert.That(Tools.GetTemplateParameterValues(@"{{test|ONE=somevalue|V_TWO=some_value}}"), Is.EqualTo(Back), "handles uppercase parameters");

            // parameters with space in name
            Back.Clear();
            Back.Add("ONE", "somevalue");
            Assert.That(Tools.GetTemplateParameterValues(@"{{test|ONE=somevalue}}"), Is.EqualTo(Back), "handles uppercase parameters");
            Back.Add("V TWO", "some_value");
            Assert.That(Tools.GetTemplateParameterValues(@"{{test|ONE=somevalue|V TWO=some_value}}"), Is.EqualTo(Back), "handles uppercase parameters");
            Assert.That(Tools.GetTemplateParameterValues(@"{{test|ONE=somevalue|V TWO  =  some_value}}"), Is.EqualTo(Back), "handles uppercase parameters");

            Back.Clear();
            Back.Add("name", "<timeline>abc</timeline>X");
            Assert.That(Tools.GetTemplateParameterValues(@"{{test|name = <timeline>abc</timeline>X}}"), Is.EqualTo(Back), "handles parameters including unformatted text");

            Back.Clear();
            Back.Add("name", "X [http://site.com A | B]");
            Back.Add("other", "Y");
            Assert.That(Tools.GetTemplateParameterValues(@"{{test|name = X [http://site.com A | B]|other=Y}}"), Is.EqualTo(Back), "handles parameters including external link with pipe");
        }

        [Test]
        public void GetTemplateParameterValueAdvancedCases()
        {
            Assert.That(Tools.GetTemplateParameterValue(@"{{cite|param1 = here {{foo}}}}", "param1"), Is.EqualTo(@"here {{foo}}"));
            Assert.That(Tools.GetTemplateParameterValue(@"{{cite|param1 = here {{foo|bar}}}}", "param1"), Is.EqualTo(@"here {{foo|bar}}"));
            Assert.That(Tools.GetTemplateParameterValue(@"{{cite|param1 = here <!--foo|bar-->}}", "param1"), Is.EqualTo(@"here <!--foo|bar-->"));
            Assert.That(Tools.GetTemplateParameterValue(@"{{cite|param1 = <!--foo-->}}", "param1"), Is.EqualTo(@"<!--foo-->"), "Reports comment when param value just comment");
            Assert.That(Tools.GetTemplateParameterValue(@"{{cite|param1 = <!--foo--> | param2 = bar }}", "param1"), Is.EqualTo(@"<!--foo-->"), "Reports comment when param value just comment");
            Assert.That(Tools.GetTemplateParameterValue(@"{{cite|param1 = [[File:abc.png|asdf|dsfjk|a]] }}", "param1"), Is.EqualTo(@"[[File:abc.png|asdf|dsfjk|a]]"));
            Assert.That(Tools.GetTemplateParameterValue(@"{{cite|param1 = [[File:abc.png|asdf|dsfjk|a]]
}}", "param1"), Is.EqualTo(@"[[File:abc.png|asdf|dsfjk|a]]"));
            Assert.That(Tools.GetTemplateParameterValue(@"{{cite|param1 = [[File:abc.png|asdf|dsfjk|a]]
|param2=other}}", "param1"), Is.EqualTo(@"[[File:abc.png|asdf|dsfjk|a]]"));
            Assert.That(Tools.GetTemplateParameterValue(@"{{cite|param1 = here <nowiki>|</nowiki> there}}", "param1"), Is.EqualTo(@"here <nowiki>|</nowiki> there"));
            Assert.That(Tools.GetTemplateParameterValue(@"{{cite|param1 = here <nowiki>|</nowiki> there|parae=aaa}}", "param1"), Is.EqualTo(@"here <nowiki>|</nowiki> there"));
            Assert.That(Tools.GetTemplateParameterValue(@"{{cite|param1 = here <pre>|</pre> there|parae=aaa}}", "param1"), Is.EqualTo(@"here <pre>|</pre> there"));
            Assert.That(Tools.GetTemplateParameterValue(@"{{cite|param1 = here <math>foo</math> there|parae=aaa}}", "param1"), Is.EqualTo(@"here <math>foo</math> there"));
            Assert.That(Tools.GetTemplateParameterValue(@"{{cite|param1 = here <timeline>foo</timeline> there|parae=aaa}}", "param1"), Is.EqualTo(@"here <timeline>foo</timeline> there"));

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

            Assert.That(Tools.GetTemplateParameterValue(ItFilm, "annoproduzione"), Is.EqualTo(@"[[1999]]"));
            Assert.That(Tools.GetTemplateParameterValue(ItFilm.Replace(@"[[1999]]", @"[[1999]"), "annoproduzione"), Is.EqualTo(@"[[1999]"));
            Assert.That(Tools.GetTemplateParameterValue(ItFilm.Replace(@"[[1999]]", @"[[1999"), "annoproduzione"), Is.EqualTo(@"[[1999"));
            Assert.That(Tools.GetTemplateParameterValue(ItFilm.Replace(@"[[1999]]", @"1999]]"), "annoproduzione"), Is.EqualTo(@"1999]]"));

            // value with braces in
            Assert.That(Tools.GetTemplateParameterValue(@"{{cite|param1= here {and} was }}", "param1"), Is.EqualTo("here {and} was"));
        }

        [Test]
        public void GetTemplateParametersValues()
        {
            List<string> returned = new List<string>();

            List<string> parameters = new List<string>();

            string template = @"{{cite web| title=abc |date=1 May 2009 | author=Smith, Ed | work=Times | location=Here }}";

            Assert.That(Tools.GetTemplateParametersValues(template, parameters), Is.EqualTo(returned));

            ClassicAssert.IsTrue(returned.Count.Equals(0));

            parameters.Add("title");
            returned.Add("abc");

            Assert.That(Tools.GetTemplateParametersValues(template, parameters), Is.EqualTo(returned));

            ClassicAssert.IsTrue(returned.Count.Equals(1));

            parameters.Add("date");
            returned.Add("1 May 2009");

            Assert.That(Tools.GetTemplateParametersValues(template, parameters), Is.EqualTo(returned));

            ClassicAssert.IsTrue(returned.Count.Equals(2));

            Assert.That(Tools.GetTemplateParametersValues(template, parameters)[0], Is.EqualTo("abc"));
            Assert.That(Tools.GetTemplateParametersValues(template, parameters)[1], Is.EqualTo("1 May 2009"));

            parameters.Add("page");
            returned.Add("");

            Assert.That(Tools.GetTemplateParametersValues(template, parameters), Is.EqualTo(returned));

            ClassicAssert.IsTrue(returned.Count.Equals(3), "zero length string added to return list if parameter not found");

            Assert.That(string.Join("", returned.ToArray()), Is.EqualTo("abc1 May 2009"));
        }

        [Test]
        public void GetTemplateArgument()
        {
            Assert.That(Tools.GetTemplateArgument(@"{{abc|foo}}", 1), Is.EqualTo("foo"));
            Assert.That(Tools.GetTemplateArgument(@"{{abc| foo}}", 1), Is.EqualTo("foo"));
            Assert.That(Tools.GetTemplateArgument(@"{{abc |  foo  }}", 1), Is.EqualTo("foo"));
            Assert.That(Tools.GetTemplateArgument(@"{{abc |  foo  |bar}}", 1), Is.EqualTo("foo"));

            Assert.That(Tools.GetTemplateArgument(@"{{abc |  foo  |bar }}", 2), Is.EqualTo("bar"));
            Assert.That(Tools.GetTemplateArgument(@"{{abc ||  foo  |bar }}", 1), Is.Empty);
            Assert.That(Tools.GetTemplateArgument(@"{{abc |  foo  |bar [[piped|link]] }}", 2), Is.EqualTo("bar [[piped|link]]"));

            Assert.That(Tools.GetTemplateArgument(@"{{abc|foo=def}}", 1), Is.EqualTo("foo=def"));
            Assert.That(Tools.GetTemplateArgument(@"{{Cleanup|section|date=May 2024}}", 1), Is.EqualTo("section"));
        }

        [Test]
        public void GetTemplateArgumentIndex()
        {
            Assert.That(Tools.GetTemplateArgumentIndex("{{abc|foo=yes}}", 1), Is.EqualTo(6));
            Assert.That(Tools.GetTemplateArgumentIndex("{{abc|" +
                                                              "foo=yes}}", 1), Is.EqualTo(6));
            Assert.That(Tools.GetTemplateArgumentIndex("{{abc|  foo=yes|bar=no}}", 1), Is.EqualTo(6));
            Assert.That(Tools.GetTemplateArgumentIndex("{{abc|foo=yes}}", 2), Is.EqualTo(-1));
        }

        [Test]
        public void GetTemplateArgumentCount()
        {
            Assert.That(Tools.GetTemplateArgumentCount(@"{{foo}}"), Is.EqualTo(0));
            Assert.That(Tools.GetTemplateArgumentCount(@"{{foo}}", true), Is.EqualTo(0));
            Assert.That(Tools.GetTemplateArgumentCount(@"{{foo|}}"), Is.EqualTo(1));
            Assert.That(Tools.GetTemplateArgumentCount(@"{{foo|bar}}"), Is.EqualTo(1), "counts unnamed parameters");
            Assert.That(Tools.GetTemplateArgumentCount(@"{{foo|bar}}", true), Is.EqualTo(0), "counts populated named parameters only when requested");
            Assert.That(Tools.GetTemplateArgumentCount(@"{{foo|bar=}}", true), Is.EqualTo(0), "counts populated named parameters only when requested");
            Assert.That(Tools.GetTemplateArgumentCount(@"{{foo|bar=yes}}"), Is.EqualTo(1), "counts named parameters");
            Assert.That(Tools.GetTemplateArgumentCount(@"{{foo|bar=yes}}", true), Is.EqualTo(1), "counts named parameters");
            Assert.That(Tools.GetTemplateArgumentCount(@"{{foo|bar=yes|asdf=iiuu|asdfsadf=|eaa=ef}}", true), Is.EqualTo(3), "counts named parameters");

            Assert.That(Tools.GetTemplateArgumentCount(@"{{foo|bar|here|yte}}"), Is.EqualTo(3), "counts multiple parameters");

            Assert.That(Tools.GetTemplateArgumentCount(@"{{foo|bar={{yes|foo}}}}"), Is.EqualTo(1), "doesn't count nested template calls");
        }

        [Test]
        public void RenameTemplateParameter()
        {
            Assert.That(Tools.RenameTemplateParameter(@"{{cite |param1=bar|param2=great}}", "param2", "param3"), Is.EqualTo(@"{{cite |param1=bar|param3=great}}"));
            Assert.That(Tools.RenameTemplateParameter(@"{{cite |param1=bar| param2 = great}}", "param2", "param3"), Is.EqualTo(@"{{cite |param1=bar| param3 = great}}"));
            Assert.That(Tools.RenameTemplateParameter(@"{{cite
|param1=bar
|param2=great}}", "param2", "param3"), Is.EqualTo(@"{{cite
|param1=bar
|param3=great}}"));
            Assert.That(Tools.RenameTemplateParameter(@"{{cite |param1=bar|param2=great|param=here}}", "param2", "param3"), Is.EqualTo(@"{{cite |param1=bar|param3=great|param=here}}"));

            // comment handling
            Assert.That(Tools.RenameTemplateParameter(@"{{cite |param1=bar|<!--comm-->param2=great}}", "param2", "param3"), Is.EqualTo(@"{{cite |param1=bar|<!--comm-->param3=great}}"));
            Assert.That(Tools.RenameTemplateParameter(@"{{cite |param1=bar|param2<!--comm-->=great}}", "param2", "param3"), Is.EqualTo(@"{{cite |param1=bar|param3<!--comm-->=great}}"));
        }

        [Test]
        public void RenameTemplateParameterList()
        {
            List<string> Params = new List<string>(new[] { "param1" });

            Assert.That(Tools.RenameTemplateParameter(@"{{cite |param1=bar|param2=great}}", Params, "paramx"), Is.EqualTo(@"{{cite |paramx=bar|param2=great}}"));

            Params.Add("param2");

            Assert.That(Tools.RenameTemplateParameter(@"{{cite |param1=bar|param2=great}}", Params, "paramx"), Is.EqualTo(@"{{cite |paramx=bar|paramx=great}}"));

            Params.Add("param3");
            Assert.That(Tools.RenameTemplateParameter(@"{{cite |param1=bar|param2=great}}", Params, "paramx"), Is.EqualTo(@"{{cite |paramx=bar|paramx=great}}"));
            Assert.That(Tools.RenameTemplateParameter(@"{{cite |param1=bar|param4=great}}", Params, "paramx"), Is.EqualTo(@"{{cite |paramx=bar|param4=great}}"));
        }

        [Test]
        public void RenameTemplateParameterDictionary()
        {
            Dictionary<string, string> Params = new Dictionary<string, string>();

            Params.Add("accesdate", "accessdate");
            Params.Add("acessdate", "accessdate");

            Assert.That(Tools.RenameTemplateParameter(@"{{cite | accesdate=2011-01-24 | title=yes }}", Params), Is.EqualTo(@"{{cite | accessdate=2011-01-24 | title=yes }}"));
            Assert.That(Tools.RenameTemplateParameter(@"{{cite | acessdate=2011-01-24 | title=yes }}", Params), Is.EqualTo(@"{{cite | accessdate=2011-01-24 | title=yes }}"));
        }

        [Test]
        public void RenameTemplateArticleText()
        {
            string correct = @"Now {{bar}} was {{bar|here}} there", correct2 = @"Now {{bar}} was {{bar
|here
|other}} there", correct3 = @"Now {{bar man}} was {{bar man|here}} there",
            correct4 = @"Now {{bar man<!--comm-->}} was {{bar man<!--comm-->|here}} there";
            Assert.That(Tools.RenameTemplate(@"Now {{foo}} was {{foo|here}} there", "foo", "bar"), Is.EqualTo(correct));
            Assert.That(Tools.RenameTemplate(@"Now {{foo}} was {{foo|here}} there", "Foo", "bar"), Is.EqualTo(correct));

            Assert.That(Tools.RenameTemplate(@"Now {{foo}} was {{ foo |here}} there", "foo", "bar"), Is.EqualTo(@"Now {{bar}} was {{ bar |here}} there"));

            Assert.That(Tools.RenameTemplate(@"Now {{foo}} was {{foo
|here
|other}} there", "Foo", "bar"), Is.EqualTo(correct2));

            Assert.That(Tools.RenameTemplate(correct, "bar2", "foo"), Is.EqualTo(correct));

            Assert.That(Tools.RenameTemplate(@"Now {{foo man}} was {{foo man|here}} there", "foo man", "bar man"), Is.EqualTo(correct3));
            Assert.That(Tools.RenameTemplate(@"Now {{foo man}} was {{foo man|here}} there", "Foo man", "bar man"), Is.EqualTo(correct3));
            Assert.That(Tools.RenameTemplate(@"Now {{foo_man}} was {{foo man|here}} there", "Foo man", "bar man"), Is.EqualTo(correct3));

            // comment handling
            Assert.That(Tools.RenameTemplate(@"Now {{foo_man<!--comm-->}} was {{foo man<!--comm-->|here}} there", "Foo man", "bar man"), Is.EqualTo(correct4));

            // handles invalid template names gracefully
            Assert.That(Tools.RenameTemplate(correct, @"foo(", "bar"), Is.EqualTo(correct));

            Assert.That(Tools.RenameTemplate(@"{{foo}} {{foo}}", "foo", "bar", 1), Is.EqualTo(@"{{bar}} {{foo}}"), "count applied correctly");

            Assert.That(Tools.RenameTemplate(@"{{foo|here1}} {{foo|here2}}", "foo", "bar|", 1), Is.EqualTo(@"{{bar||here1}} {{foo|here2}}"), "rename to add pipe");
        }

        private readonly HideText Hider = new HideText();

        [Test]
        public void RenameTemplate()
        {
            Assert.That(Tools.RenameTemplate(@"{{foo}}", "bar"), Is.EqualTo(@"{{bar}}"));

            // space kept
            Assert.That(Tools.RenameTemplate(@"{{ foo }}", "bar"), Is.EqualTo(@"{{ bar }}"));

            // casing
            Assert.That(Tools.RenameTemplate(@"{{Foo}}", "bar", false), Is.EqualTo(@"{{bar}}"));
            Assert.That(Tools.RenameTemplate(@"{{Foo}}", "bar", true), Is.EqualTo(@"{{Bar}}"));
            Assert.That(Tools.RenameTemplate(@"{{Foo}}", "Bar"), Is.EqualTo(@"{{Bar}}"));
            Assert.That(Tools.RenameTemplate(@"{{Foo}}", "Bar", true), Is.EqualTo(@"{{Bar}}"));

            // with params
            Assert.That(Tools.RenameTemplate(@"{{Foo|parameters=adsfjk}}", "bar", false), Is.EqualTo(@"{{bar|parameters=adsfjk}}"));
            Assert.That(Tools.RenameTemplate(@"{{Foo |parameters=adsfjk}}", "bar", false), Is.EqualTo(@"{{bar |parameters=adsfjk}}"));
            Assert.That(Tools.RenameTemplate(@"{{foo |parameters=adsfjk}}", "Bar", true), Is.EqualTo(@"{{bar |parameters=adsfjk}}"));

            // comment handling
            Assert.That(Tools.RenameTemplate(@"{{foo man<!--comm-->|here}}", "bar man"), Is.EqualTo(@"{{bar man<!--comm-->|here}}"));
            Assert.That(Tools.RenameTemplate(@"{{foo man <!--comm-->|here}}", "bar man"), Is.EqualTo(@"{{bar man <!--comm-->|here}}"));
            Assert.That(Tools.RenameTemplate(@"{{foo man <!--comm-->
|here}}", "bar man"), Is.EqualTo(@"{{bar man <!--comm-->
|here}}"));

            Assert.That(Hider.AddBack(Tools.RenameTemplate(Hider.Hide(@"{{foo man <!--comm-->|here}}"), "bar man")), Is.EqualTo(@"{{bar man <!--comm-->|here}}"));

            // special case of subst:, first letter case rule does not apply
            Assert.That(Tools.RenameTemplate(@"{{PAGENAME}}", "PAGENAME", "subst:PAGENAME"), Is.EqualTo(@"{{subst:PAGENAME}}"));
            Assert.That(Tools.RenameTemplate(@"{{PAGENAME}}", "PAGENAME", "subst:PAGENAME", false), Is.EqualTo(@"{{subst:PAGENAME}}"));
            Assert.That(Tools.RenameTemplate(@"{{PAGENAME}}", "PAGENAME", "subst:PAGENAME", true), Is.EqualTo(@"{{subst:PAGENAME}}"));

        }

        [Test]
        public void RemoveTemplateParameterTemplateName()
        {
            string correct = @"{{cite web|url=http://www.site.com |title=here |year=2008 }}";

            Assert.That(Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat=mdy}}", "cite web", "dateformat"), Is.EqualTo(correct));
            Assert.That(Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat=mdy }}", "cite web", "dateformat"), Is.EqualTo(correct));
            Assert.That(Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat=}}", "cite web", "dateformat"), Is.EqualTo(correct));
            Assert.That(Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |year=2008 | dateformat =   mdy}}", "cite web", "dateformat"), Is.EqualTo(correct));
            Assert.That(Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here | dateformat=mdy|year=2008 }}", "cite web", "dateformat"), Is.EqualTo(correct));

            Assert.That(Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat=<!--comm-->mdy}}", "cite web", "dateformat"), Is.EqualTo(correct));
            Assert.That(Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat=mdy<!--comm-->}}", "cite web", "dateformat"), Is.EqualTo(correct));

            Assert.That(Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat=[[200]]-[[288]]}}", "cite web", "dateformat"), Is.EqualTo(correct));
            Assert.That(Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat = [[200]]-[[288]]}}", "cite web", "dateformat"), Is.EqualTo(correct));

            Assert.That(Tools.RemoveTemplateParameter(@"{{cite web<!--foo-->|url=http://www.site.com |title=here |year=2008 |dateformat=mdy}}", "cite web", "dateformat"),
                            Is.EqualTo(@"{{cite web<!--foo-->|url=http://www.site.com |title=here |year=2008 }}"));

            // processes multiple templates
            string citeweb = @"{{cite web|url=http://www.site.com |title=here | dateformat=mdy|year=2008 }}";
            Assert.That(Tools.RemoveTemplateParameter(citeweb + citeweb, "cite web", "dateformat"), Is.EqualTo(correct + correct));

            // first letter case insensitive
            Assert.That(Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat=mdy}}", "Cite web", "dateformat"), Is.EqualTo(correct));

            // no change when parameter doesn't exist
            Assert.That(Tools.RemoveTemplateParameter(correct, "cite web", "dateformat"), Is.EqualTo(correct));

            // no partial match on paramter name
            Assert.That(Tools.RemoveTemplateParameter(correct, "cite web", "yea"), Is.EqualTo(correct));

            // no partial match on template name
            Assert.That(Tools.RemoveTemplateParameter(correct, "cite webs", "year"), Is.EqualTo(correct));

            // parameter name case sensitive
            string nochange = @"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat=mdy}}";
            Assert.That(Tools.RemoveTemplateParameter(nochange, "cite web", "Dateformat"), Is.EqualTo(nochange));
        }

        [Test]
        public void RemoveTemplateParameterSingleTemplate()
        {
            string correct = @"{{cite web|url=http://www.site.com |title=here |year=2008 }}";

            Assert.That(Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat=mdy}}", "dateformat"), Is.EqualTo(correct));
            Assert.That(Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat=mdy }}", "dateformat"), Is.EqualTo(correct));
            Assert.That(Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat=}}", "dateformat"), Is.EqualTo(correct));
            Assert.That(Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |year=2008 | dateformat =   mdy}}", "dateformat"), Is.EqualTo(correct));
            Assert.That(Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here | dateformat=mdy|year=2008 }}", "dateformat"), Is.EqualTo(correct));

            Assert.That(Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here | dateformat=mdy|year=2008 }}", "dateformat", false), Is.EqualTo(correct));
            Assert.That(Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here | dateformat=mdy|year=2008 }}", "dateformat", true), Is.EqualTo(correct));

            // first letter case insensitive
            Assert.That(Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat=mdy}}", "dateformat"), Is.EqualTo(correct));

            // no change when parameter doesn't exist
            Assert.That(Tools.RemoveTemplateParameter(correct, "dateformat"), Is.EqualTo(correct));

            // no partial match on paramter name
            Assert.That(Tools.RemoveTemplateParameter(correct, "yea"), Is.EqualTo(correct));

            // parameter name case sensitive
            string nochange = @"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat=mdy}}";
            Assert.That(Tools.RemoveTemplateParameter(nochange, "Dateformat"), Is.EqualTo(nochange));

            // duplicate parameters
            Assert.That(Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here | dateformat=mdy|year=2008 |dateformat=foo}}", "dateformat", true),
                            Is.EqualTo(@"{{cite web|url=http://www.site.com |title=here | dateformat=mdy|year=2008 }}"));

            Assert.That(Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here | dateformat=mdy|year=2008 |dateformat=foo}}", "dateformat", false),
                            Is.EqualTo(@"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat=foo}}"));

            Assert.That(Tools.RemoveTemplateParameter(@"{{foo|first=abc|second=def|second=def}}", "second", false),
                            Is.EqualTo(@"{{foo|first=abc|second=def}}"));
        }

        [Test]
        public void RemoveTemplateParametersSingleTemplate()
        {
            List<string> Params = new List<string>();
            Params.Add("dateformat");

            string correct = @"{{cite web|url=http://www.site.com |title=here |year=2008 }}";

            Assert.That(Tools.RemoveTemplateParameters(@"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat=mdy}}", Params), Is.EqualTo(correct));
            Assert.That(Tools.RemoveTemplateParameters(@"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat=mdy }}", Params), Is.EqualTo(correct));
            Assert.That(Tools.RemoveTemplateParameters(@"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat=}}", Params), Is.EqualTo(correct));
            Assert.That(Tools.RemoveTemplateParameters(@"{{cite web|url=http://www.site.com |title=here |year=2008 | dateformat =   mdy}}", Params), Is.EqualTo(correct));
            Assert.That(Tools.RemoveTemplateParameters(@"{{cite web|url=http://www.site.com |title=here | dateformat=mdy|year=2008 }}", Params), Is.EqualTo(correct));
            Params.Add("format");

            Assert.That(Tools.RemoveTemplateParameters(@"{{cite web|url=http://www.site.com |title=here | dateformat=mdy|year=2008 |format=DOC}}", Params), Is.EqualTo(correct));
        }

        [Test]
        public void RemoveTemplateParameterAdvancedCases()
        {
            string correct = @"{{cite web|url=http://www.site.com |title=here |year=2008 }}";

            Assert.That(Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |year=2008 |
dateformat=mdy}}", "cite web", "dateformat"), Is.EqualTo(correct));
            Assert.That(Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat
=mdy}}", "cite web", "dateformat"), Is.EqualTo(correct));
            Assert.That(Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat=[[Foo|bar]]}}", "cite web", "dateformat"), Is.EqualTo(correct));
            Assert.That(Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat= now [[Foo|bar]] and [[Foo|bar]] again}}", "cite web", "dateformat"), Is.EqualTo(correct));
            Assert.That(Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat= {{foo|mdy}} bar}}", "cite web", "dateformat"), Is.EqualTo(correct));
            Assert.That(Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat= {{foo|mdy}} bar {{here}}}}", "cite web", "dateformat"), Is.EqualTo(correct));
            Assert.That(Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat= <!--now|--> bar}}", "cite web", "dateformat"), Is.EqualTo(correct));
            Assert.That(Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |year=2008 |date_format= A<nowiki>|</nowiki>B bar}}", "cite web", "date_format"), Is.EqualTo(correct));
            Assert.That(Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat= [[File:foo.JPG|bar|here]] bar}}", "cite web", "dateformat"), Is.EqualTo(correct));

            Assert.That(Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |title=here |dateformat= [[File:foo.JPG|bar|here]] bar|year=2008 }}", "cite web", "dateformat"), Is.EqualTo(correct));
            Assert.That(Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |dateformat= <!--now|--> bar|title=here |year=2008 }}", "cite web", "dateformat"), Is.EqualTo(correct));
            Assert.That(Tools.RemoveTemplateParameter(@"{{cite web|url=http://www.site.com |dateformat= {{some template|foo={{a}}|bar=b}} bar|title=here |year=2008 }}", "cite web", "dateformat"), Is.EqualTo(correct));

            correct = @"{{Fred | first=Bar | upper={{Bert|lower=yes}} }}";

            Assert.That(Tools.RenameTemplateParameter(@"{{Fred | first=Bar | lower={{Bert|lower=yes}} }}", "lower", "upper"), Is.EqualTo(correct), @"Parameter within nested template not renamed");
            Assert.That(Tools.RenameTemplateParameter(@"{{Fred | last=Bar | upper={{Bert|lower=yes}} }}", "last", "first"), Is.EqualTo(correct), @"Template parameters can be rename when nested templates present");

            correct = @"{{Fred | first=Bar | upper={{Bert|lower=yes|2={{great}} }} }}";

            Assert.That(Tools.RenameTemplateParameter(@"{{Fred | first=Bar | lower={{Bert|lower=yes|2={{great}} }} }}", "lower", "upper"), Is.EqualTo(correct), @"Parameter within nested nested template not renamed");
        }

        [Test]
        public void RemoveDuplicateTemplateParameters()
        {
            Assert.That(Tools.RemoveDuplicateTemplateParameters(@"{{foo|first=abc|second=def|second=def}}"), Is.EqualTo(@"{{foo|first=abc|second=def}}"));
            Assert.That(Tools.RemoveDuplicateTemplateParameters(@"{{foo|first=abc|second=def|second = def}}"), Is.EqualTo(@"{{foo|first=abc|second = def}}"), "first removed if both the same");
            Assert.That(Tools.RemoveDuplicateTemplateParameters(@"{{foo|first=abc|second=def|second=}}"), Is.EqualTo(@"{{foo|first=abc|second=def}}"), "null dupe param removed");
            Assert.That(Tools.RemoveDuplicateTemplateParameters(@"{{foo|first=abc|second=def|second=
def
}}"), Is.EqualTo(@"{{foo|first=abc|second=
def
}}"), "dupe param removed ignoring leading/trailing whitespace");
            Assert.That(Tools.RemoveDuplicateTemplateParameters(@"{{foo|first=abc|second=|second=def}}"), Is.EqualTo(@"{{foo|first=abc|second=def}}"), "null dupe param removed");

            Assert.That(Tools.RemoveDuplicateTemplateParameters(@"{{foo|first=abc|second=def|second=def|second=def}}"), Is.EqualTo(@"{{foo|first=abc|second=def}}"), "multiple duplicates removed");
            Assert.That(Tools.RemoveDuplicateTemplateParameters(@"{{foo|first=abc|first=abc|second=def|second=def|second=def}}"), Is.EqualTo(@"{{foo|first=abc|second=def}}"), "multiple duplicates removed");

            const string noDupe2 = @"{{foo|first=abc|second=def|Second=def}}";

            Assert.That(Tools.RemoveDuplicateTemplateParameters(noDupe2), Is.EqualTo(noDupe2), "case sensitive parameter name matching");

            Assert.That(Tools.RemoveDuplicateTemplateParameters(@"{{foo|first=abc|second=dex|second=dex|second=defg}}"), Is.EqualTo(@"{{foo|first=abc|second=dex|second=defg}}"), "non-duplicates not removed");

            Assert.That(Tools.RemoveDuplicateTemplateParameters(@"{{foo|first=abc|second={{def|bar}}|second={{def|bar}}}}"), Is.EqualTo(@"{{foo|first=abc|second={{def|bar}}}}"));
        
            Dictionary<string, string> Params = new Dictionary<string, string>();
            Tools.RemoveDuplicateTemplateParameters(@"{{foo|first=abc|second=def|second=def}}", Params);
            Assert.That(Params.Count, Is.EqualTo(2));
            Params.Clear();
            Tools.RemoveDuplicateTemplateParameters(noDupe2, Params);
            Assert.That(Params.Count, Is.EqualTo(3));
            Params.Clear();
            Tools.RemoveDuplicateTemplateParameters(@"{{foo|first=abc||second=  def <!--com-->  }}", Params);
            string d = "";
            ClassicAssert.IsTrue(Params.TryGetValue("second", out d));
            Assert.That(d, Is.EqualTo("def <!--com-->"), "parameter with space and comment retrieved correctly");

            Assert.That(Tools.RemoveDuplicateTemplateParameters(@"{{foo|first=abc|second2=def|second2=def}}"), Is.EqualTo(@"{{foo|first=abc|second2=def|second2=def}}"));
        }

        [Test]
        public void RemoveDuplicateTemplateParametersURLs()
        {
            const string withUnescapedPipes = @"{{cite web|foo=bar|url=http://site.com/news/foo|bar=yes|bar=yes|other.stm | date=2010}}";

            Assert.That(Tools.RemoveDuplicateTemplateParameters(withUnescapedPipes), Is.EqualTo(withUnescapedPipes), "no change when URL could be borken");

            Assert.That(Tools.RemoveDuplicateTemplateParameters(@"{{foo|first=abc|url= | url=http://site.com/news }}"), Is.EqualTo(@"{{foo|first=abc| url=http://site.com/news }}"));

            Assert.That(Tools.RemoveDuplicateTemplateParameters(@"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008|year=2008}}"), Is.EqualTo(@"{{cite web|url=a |title=b | accessdate=11 May 2008|year=2008}}"));
        }

        [Test]
        public void DuplicateTemplateParameters()
        {
            Dictionary<int, int> Dupes = new Dictionary<int, int>();

            Assert.That(Tools.DuplicateTemplateParameters(@"{{cite web|url=here |title=there}}"), Is.EqualTo(Dupes));

            Dupes.Add(32, 9);
            Assert.That(Tools.DuplicateTemplateParameters(@"{{cite web|url=here |title=there|url=here}}"), Is.EqualTo(Dupes), "dupes if the same");
            Assert.That(Tools.DuplicateTemplateParameters(@"{{cite web|url=here |title=there|url=her2}}"), Is.EqualTo(Dupes), "dupes if not the same");

            Dupes.Clear();
            Dupes.Add(36, 5);
            Assert.That(Tools.DuplicateTemplateParameters(@"{{cite web|url=here |title=therehere|url=}}"), Is.EqualTo(Dupes), "dupes if the same");

            Dupes.Clear();
            Dupes.Add(32, 9);
            Dupes.Add(41, 12);
            Assert.That(Tools.DuplicateTemplateParameters(@"{{cite web|url=here |title=there|url=here|title=there}}"), Is.EqualTo(Dupes), "multiple dupes reported");
        }

        [Test]
        public void UnknownTemplateParameters()
        {
            List<string> Unknowns = new List<string>();

            List<string> Knowns = new List<string>(new[] { "title", "date", "url" });

            Assert.That(Tools.UnknownTemplateParameters(@"{{cite web|title=a|date=2010}}", Knowns), Is.EqualTo(Unknowns));
            // Confirm other casing work too
            Assert.That(Tools.UnknownTemplateParameters(@"{{cite web|TITLE=a|DATE=2010}}", Knowns), Is.EqualTo(Unknowns));

            Unknowns.Add("foo");
            Assert.That(Tools.UnknownTemplateParameters(@"{{cite web|title=a|date=2010|foo=}}", Knowns), Is.EqualTo(Unknowns), "reported even if blank");
            Assert.That(Tools.UnknownTemplateParameters(@"{{cite web|title=a|date=2010|foo=b}}", Knowns), Is.EqualTo(Unknowns), "unknown parameter reported");

            Unknowns.Add("bar");
            Assert.That(Tools.UnknownTemplateParameters(@"{{cite web|title=a|date=2010|foo=b|bar=}}", Knowns), Is.EqualTo(Unknowns), "multiple unknowns reported");
        }

        [Test]
        public void RemoveExcessTemplatePipes()
        {
            Assert.That(Tools.RemoveExcessTemplatePipes(@"{{foo||param1=before}}"), Is.EqualTo(@"{{foo|param1=before}}"), "excess pipe removed");
            Assert.That(Tools.RemoveExcessTemplatePipes(@"{{foo||param1=before|}}"), Is.EqualTo(@"{{foo|param1=before}}"), "excess pipes removed");
            Assert.That(Tools.RemoveExcessTemplatePipes(@"{{foo| |param1=before}}"), Is.EqualTo(@"{{foo|param1=before}}"), "excess spaced pipe removed");
            Assert.That(Tools.RemoveExcessTemplatePipes(@"{{foo|
		|param1=before}}"), Is.EqualTo(@"{{foo|param1=before}}"), "space is newline");
            Assert.That(Tools.RemoveExcessTemplatePipes(@"{{foo|     |param1=before}}"), Is.EqualTo(@"{{foo|param1=before}}"));
            Assert.That(Tools.RemoveExcessTemplatePipes(@"{{foo||param1=before||param2=b}}"), Is.EqualTo(@"{{foo|param1=before|param2=b}}"), "mulitple excess pipes");
            Assert.That(Tools.RemoveExcessTemplatePipes(@"{{foo||param1=before|}}"), Is.EqualTo(@"{{foo|param1=before}}"), "excess pipe at end");
            const string nested = @"{{foo| one={{bar|a||c}}|two=x}}";
            Assert.That(Tools.RemoveExcessTemplatePipes(nested), Is.EqualTo(nested), "pipes within nested templates not changed");
        }

        [Test]
        public void UpdateTemplateParameterValue()
        {
            Assert.That(Tools.UpdateTemplateParameterValue(@"{{foo|param1=before}}", "param1", "valueafter"), Is.EqualTo(@"{{foo|param1=valueafter}}"));
            Assert.That(Tools.UpdateTemplateParameterValue(@"{{foo|param1= before }}", "param1", "valueafter"), Is.EqualTo(@"{{foo|param1= valueafter }}"), "whitespace kept");
            Assert.That(Tools.UpdateTemplateParameterValue(@"{{foo|param1=
before}}", "param1", "valueafter"), Is.EqualTo(@"{{foo|param1=
valueafter}}"), "newline before populated parameter kept");

            Assert.That(Tools.UpdateTemplateParameterValue(@"{{foo
|param1=before
|param2=okay}}", "param1", "valueafter"), Is.EqualTo(@"{{foo
|param1=valueafter
|param2=okay}}"), "existing newline kept");

            Assert.That(Tools.UpdateTemplateParameterValue(@"{{foo
|param1= before
|param2=okay}}", "param1", "valueafter"), Is.EqualTo(@"{{foo
|param1= valueafter
|param2=okay}}"), "existing newline & whitespace kept");

            Assert.That(Tools.UpdateTemplateParameterValue(@"{{foo|param1<!--comm-->= before }}", "param1", "valueafter"), Is.EqualTo(@"{{foo|param1<!--comm-->= valueafter }}"));
            Assert.That(Tools.UpdateTemplateParameterValue(@"{{foo|<!--comm-->param1= before }}", "param1", "valueafter"), Is.EqualTo(@"{{foo|<!--comm-->param1= valueafter }}"));

            Assert.That(Tools.UpdateTemplateParameterValue(@"{{foo|param1=[[1891]]-[[1892]]}}", "param1", "[[1891]]–[[1892]]"), Is.EqualTo(@"{{foo|param1=[[1891]]–[[1892]]}}"));
            Assert.That(Tools.UpdateTemplateParameterValue(@"{{foo|param1= [[1891]]-[[1892]] }}", "param1", "[[1891]]–[[1892]]"), Is.EqualTo(@"{{foo|param1= [[1891]]–[[1892]] }}"));

            Assert.That(Tools.UpdateTemplateParameterValue(@"{{foo|param1=before|param2=before}}", "param1", "valueafter"), Is.EqualTo(@"{{foo|param1=valueafter|param2=before}}"));

            Assert.That(Tools.UpdateTemplateParameterValue(@"{{foo|param1=before<!--comm-->|param2=before}}", "param1", "valueafter<!--comm-->"), Is.EqualTo(@"{{foo|param1=valueafter<!--comm-->|param2=before}}"));
            Assert.That(Tools.UpdateTemplateParameterValue(@"{{foo|param1=<!--comm-->before|param2=before}}", "param1", "<!--comm-->valueafter"), Is.EqualTo(@"{{foo|param1=<!--comm-->valueafter|param2=before}}"));

            // parameter not used – no change
            Assert.That(Tools.UpdateTemplateParameterValue(@"{{foo|param1=before}}", "param2", "valueafter"), Is.EqualTo(@"{{foo|param1=before}}"));

            // old value null – updated correctly
            Assert.That(Tools.UpdateTemplateParameterValue(@"{{foo|param1=}}", "param1", "valueafter"), Is.EqualTo(@"{{foo|param1=valueafter}}"));

            string input = @"{{cite news
|author=
|title=Obituary 1 -- No Title
|date=
|work=[[New York Times]]
|url=http://query.nytimes.com/gst/abstra
|accessdate=2008-08-08
}}";
            Assert.That(Tools.UpdateTemplateParameterValue(input, "date", "April 4, 1922"), Is.EqualTo(input.Replace("|date=", "|date=April 4, 1922")));

            input = @"{{cite news
|author=
|title=Obituary 1 -- No Title
|date=


|work=[[New York Times]]
|url=http://query.nytimes.com/gst/abstra }}";

            Assert.That(Tools.UpdateTemplateParameterValue(input, "date", "April 4, 1922"), Is.EqualTo(input.Replace("|date=", "|date=April 4, 1922")));
        }

        [Test]
        public void AddTemplateParameterValue()
        {
            Assert.That(Tools.AddTemplateParameterValue(@"{{foo|param1=oldvalue}}", "param1", "newvalue", false),
                Is.EqualTo(@"{{foo|param1=oldvalue}}"));

            Assert.That(Tools.AddTemplateParameterValue(@"{{foo}}", "param1", "newvalue", false),
                Is.EqualTo(@"{{foo | param1=newvalue}}"));

            Assert.That(Tools.AddTemplateParameterValue(@"{{foo}}", "param1", "newvalue", true),
                Is.EqualTo(@"{{foo | param1= newvalue}}"));
        }

        [Test]
        public void SetTemplateParameterValue()
        {
            Assert.That(Tools.SetTemplateParameterValue(@"{{foo|param1=before}}", "param1", "valueafter"), Is.EqualTo(@"{{foo|param1=valueafter}}"));
            Assert.That(Tools.SetTemplateParameterValue(@"{{foo|param1= before }}", "param1", "valueafter"), Is.EqualTo(@"{{foo|param1= valueafter }}"));

            Assert.That(Tools.SetTemplateParameterValue(@"{{foo|param1=before}}", "param1", "valueafter", true), Is.EqualTo(@"{{foo|param1= valueafter}}"));
            Assert.That(Tools.SetTemplateParameterValue(@"{{foo|param1=before}}", "param1", "valueafter", false), Is.EqualTo(@"{{foo|param1=valueafter}}"));

            Assert.That(Tools.SetTemplateParameterValue(@"{{foo|param1=before|param2=before}}", "param1", "valueafter"), Is.EqualTo(@"{{foo|param1=valueafter|param2=before}}"));

            // parameter not used – set
            Assert.That(Tools.SetTemplateParameterValue(@"{{foo|param1=before}}", "param2", "valueafter"), Is.EqualTo(@"{{foo|param1=before | param2=valueafter}}"));
            Assert.That(Tools.SetTemplateParameterValue(@"{{foo|param1=before|param3=a}}", "param2", "valueafter"), Is.EqualTo(@"{{foo|param1=before|param3=a | param2=valueafter}}"));
            Assert.That(Tools.SetTemplateParameterValue(@"{{foo|param1=w|param2=x|param3=y|param4=z}}", "param5", "a"), Is.EqualTo(@"{{foo|param1=w|param2=x|param3=y|param4=z|param5=a}}"));
            Assert.That(Tools.SetTemplateParameterValue(@"{{foo| param1=w | param2=x | param3=y | param4=z }}", "param5", "a"), Is.EqualTo(@"{{foo| param1=w | param2=x | param3=y | param4=z | param5=a }}"), "retain template call end space");

            // old value null – updated correctly
            Assert.That(Tools.SetTemplateParameterValue(@"{{foo|param1=}}", "param1", "valueafter"), Is.EqualTo(@"{{foo|param1=valueafter}}"));

            string bug1 = @"{{Infobox college coach|
| Name          = Bill
| CurrentRecord = 202–43 ({{Winning percentage|202|43}})
| OverallRecord = 409-148 ({{Winning percentage|409|148}})
| Player        = *
| CollegeHOFID  =
| BBallHOF      =
}}}";
            Assert.That(Tools.SetTemplateParameterValue(bug1, "OverallRecord", @"409–148 ({{Winning percentage|409|148}})"), Is.EqualTo(bug1.Replace(@"409-148 ({{Winning percentage", @"409–148 ({{Winning percentage")));

            string input = @"{{cite news
|author=
|title=Obituary 1 -- No Title
|date=
|work=[[New York Times]]
|url=http://query.nytimes.com/gst/abstra
|accessdate=2008-08-08
}}";
            Assert.That(Tools.SetTemplateParameterValue(input, "date", "April 4, 1922"), Is.EqualTo(input.Replace("|date=", "|date=April 4, 1922")));

            // existing value = new one, no change
            Assert.That(Tools.SetTemplateParameterValue(@"{{foo|param1=valueafter}}", "param1", "valueafter"), Is.EqualTo(@"{{foo|param1=valueafter}}"));
        }

        [Test]
        public void NestedTemplateRegexSingleWord()
        {
            Regex FooTemplate = Tools.NestedTemplateRegex("foo");

            Assert.That(FooTemplate.Match(@"{{  foo}}").Groups[1].Value, Is.EqualTo(@"{{  "));
            Assert.That(FooTemplate.Match(@"{{foo}}").Groups[2].Value, Is.EqualTo(@"foo"));
            Assert.That(FooTemplate.Match(@"{{ Foo}}").Groups[2].Value, Is.EqualTo(@"Foo"));
            Assert.That(FooTemplate.Match(@"{{ Foo|title=abc}}").Groups[3].Value, Is.EqualTo(@"|title=abc}}"));

            ClassicAssert.IsTrue(FooTemplate.IsMatch(@"{{foo}}"));
            ClassicAssert.IsTrue(FooTemplate.IsMatch(@"{{___foo___}}"));
            ClassicAssert.IsTrue(FooTemplate.IsMatch(@"{{Foo}}"));
            ClassicAssert.IsTrue(FooTemplate.IsMatch(@"{{ foo}}"));
            ClassicAssert.IsTrue(FooTemplate.IsMatch(@"{{ foo|}}"));
            ClassicAssert.IsTrue(FooTemplate.IsMatch(@"{{foo|title=abc}}"));
            ClassicAssert.IsTrue(FooTemplate.IsMatch(@"{{foo|
title=abc
|other=yes}}"));
            ClassicAssert.IsTrue(FooTemplate.IsMatch(@"{{foo
|title=abc
|other=yes}}"));
            ClassicAssert.IsTrue(FooTemplate.IsMatch(@"{{foo<!--comm-->|title=abc}}"));
            ClassicAssert.IsTrue(FooTemplate.IsMatch(@"{{foo <!--comm--> |title=abc}}"));
            ClassicAssert.IsTrue(FooTemplate.IsMatch(@"{{foo ⌊⌊⌊⌊0⌋⌋⌋⌋ |title=abc}}"));
            ClassicAssert.IsTrue(FooTemplate.IsMatch(@"{{
foo<!--comm-->|title=abc
}}"));
            ClassicAssert.IsTrue(FooTemplate.IsMatch(@"{{foo|title={{abc}}}}"));
            ClassicAssert.IsTrue(FooTemplate.IsMatch(@"{{foo|title={{abc}}|last=Fred}}"), "matches nested templates");
            ClassicAssert.IsTrue(FooTemplate.IsMatch(@"{{foo|
title={{abc|fdkjdsfjk=fdaskjlfds
|fdof=affdsa}}
|last=Fred}}"), "matches nested parameterised templates");

            ClassicAssert.IsTrue(FooTemplate.IsMatch(@"{{foo|title={abc} def|last=Fred}}"), "matches balanced single curly braces");

            ClassicAssert.IsFalse(FooTemplate.IsMatch(@"{{foo|title={abc def|last=Fred}}"), "doesn't match unbalanced single curly braces");

            ClassicAssert.IsFalse(FooTemplate.IsMatch(@"{{foobar}}"));
            ClassicAssert.IsFalse(FooTemplate.IsMatch(@"{{foo"));
            ClassicAssert.IsFalse(FooTemplate.IsMatch(@"{{foo}"));
            ClassicAssert.IsFalse(FooTemplate.IsMatch(@"{foo}"));

            FooTemplate = Tools.NestedTemplateRegex("Foo");
            ClassicAssert.IsTrue(FooTemplate.IsMatch(@"{{foo}}"));
            ClassicAssert.IsTrue(FooTemplate.IsMatch(@"{{Foo}}"));
            ClassicAssert.IsTrue(FooTemplate.IsMatch(@"{{ foo}}"));
            ClassicAssert.IsTrue(FooTemplate.IsMatch(@"{{Template:foo}}"));
            ClassicAssert.IsTrue(FooTemplate.IsMatch(@"{{:Template:foo}}"));
            ClassicAssert.IsTrue(FooTemplate.IsMatch(@"{{:Msg:foo}}"));
            ClassicAssert.IsTrue(FooTemplate.IsMatch(@"{{Msg:foo}}"));
            ClassicAssert.IsTrue(FooTemplate.IsMatch(@"{{_:_Template_:_foo_}}"));

            ClassicAssert.IsFalse(FooTemplate.IsMatch(@"{{Template foo}}"));

            FooTemplate = Tools.NestedTemplateRegex("foo", true);
            Assert.That(FooTemplate.Match(@"{{  foo}}").Groups[1].Value, Is.EqualTo(@"{{  "));

            FooTemplate = Tools.NestedTemplateRegex("", true);
            Assert.That(FooTemplate, Is.EqualTo(null));
            
            Variables.NamespacesCaseInsensitive.Remove(Namespace.Template);
            FooTemplate = Tools.NestedTemplateRegex("Foo", true);
            ClassicAssert.IsTrue(FooTemplate.IsMatch(@"{{Template:foo}}"));
            Variables.NamespacesCaseInsensitive.Add(Namespace.Template, "[Tt]emplate:");
        }

        [Test]
        public void NestedTemplateRegexRTL()
        {
            Regex ArTemplate = Tools.NestedTemplateRegex(@"وصلات قليلة");
            ClassicAssert.IsTrue(ArTemplate.IsMatch(@"{{وصلات قليلة|تاريخ=ديسمبر 2012}}"));
            Assert.That(ArTemplate.Replace(@"{{وصلات قليلة|تاريخ=ديسمبر 2012}}", ""), Is.Empty);
            Assert.That(ArTemplate.Replace(@"{{وصلات قليلة|تاريخ=يناير_2009}}", ""), Is.Empty);
        }

        [Test]
        public void NestedTemplateRegexTwoWords()
        {
            Regex FooTemplate2 = Tools.NestedTemplateRegex("foo bar");

            ClassicAssert.IsTrue(FooTemplate2.IsMatch(@"{{foo bar}}"));
            ClassicAssert.IsTrue(FooTemplate2.IsMatch(@"{{foo_bar}}"));
            ClassicAssert.IsTrue(FooTemplate2.IsMatch(@"{{foo_____bar}}"));
            ClassicAssert.IsTrue(FooTemplate2.IsMatch(@"{{Foo bar}}"));
            ClassicAssert.IsTrue(FooTemplate2.IsMatch(@"{{Foo      bar}}"));

            ClassicAssert.IsFalse(FooTemplate2.IsMatch(@"{{foo}}"));
            ClassicAssert.IsFalse(FooTemplate2.IsMatch(@"{{foo b_ar}}"));
            ClassicAssert.IsFalse(FooTemplate2.IsMatch(@"{{foo
bar|text}}"));
            ClassicAssert.IsFalse(Tools.NestedTemplateRegex("birth date").IsMatch(@"{{birth-date|May 11, 1980}}"));
        }

        [Test]
        public void NestedTemplateRegexLimits()
        {
            Regex FooTemplate2 = Tools.NestedTemplateRegex("");

            ClassicAssert.IsNull(FooTemplate2);
        }

        [Test]
        public void NestedTemplateRegexListSingle()
        {
            List<string> ListOfTemplates = new List<string>();

            Regex MultipleTemplatesN = Tools.NestedTemplateRegex(ListOfTemplates);
            ClassicAssert.IsNull(MultipleTemplatesN, "null return if zero-entry list input");

            ListOfTemplates.Add(@"Foo");

            Regex multipleTemplates = Tools.NestedTemplateRegex(ListOfTemplates);

            Assert.That(multipleTemplates.Match(@"{{foo}}").Groups[2].Value, Is.EqualTo(@"foo"), "Group 2 is template name");
            Assert.That(multipleTemplates.Match(@"{{ Foo}}").Groups[2].Value, Is.EqualTo(@"Foo"), "Group 2 is template name");
            Assert.That(multipleTemplates.Match(@"{{ Foo|bar}}").Groups[3].Value, Is.EqualTo(@"|bar}}"), "Group 3 is template from bar to end");
            Assert.That(multipleTemplates.Match(@"{{ Foo|bar=one|he=there}}").Groups[3].Value, Is.EqualTo(@"|bar=one|he=there}}"), "Group 3 is template from bar to end");
            Assert.That(multipleTemplates.Match(@"{{ Foo|bar={{one}}|he=there}}").Groups[3].Value, Is.EqualTo(@"|bar={{one}}|he=there}}"), "Group 3 is template from bar to end");
            Assert.That(multipleTemplates.Match(@"{{ Foo|bar={{one}}|he=ther {{e}}}}").Groups[3].Value, Is.EqualTo(@"|bar={{one}}|he=ther {{e}}}}"), "Group 3 is template from bar to end");

            Assert.That(multipleTemplates.Match(@"{{ Foo|bar={{one}}|he=there}}").Groups[4].Value, Is.EqualTo(@"|bar={{one}}|he=there"), "Group 4 is template from bar to end excluding end }}");
            Assert.That(multipleTemplates.Match(@"{{ Foo|bar={{one}}|he=ther {{e}}}}").Groups[4].Value, Is.EqualTo(@"|bar={{one}}|he=ther {{e}}"), "Group 4 is template from bar to end excluding end }}");

            ClassicAssert.IsTrue(multipleTemplates.IsMatch(@"{{foo}}"));
            ClassicAssert.IsTrue(multipleTemplates.IsMatch(@"{{Foo}}"));
            ClassicAssert.IsTrue(multipleTemplates.IsMatch(@"{{ foo}}"));
            ClassicAssert.IsTrue(multipleTemplates.IsMatch(@"{{ foo|}}"));
            ClassicAssert.IsTrue(multipleTemplates.IsMatch(@"{{foo|title=abc}}"));
            ClassicAssert.IsTrue(multipleTemplates.IsMatch(@"{{foo|
title=abc
|other=yes}}"));
            ClassicAssert.IsTrue(multipleTemplates.IsMatch(@"{{foo
|title=abc
|other=yes}}"));
            ClassicAssert.IsTrue(multipleTemplates.IsMatch(@"{{foo<!--comm-->|title=abc}}"));
            ClassicAssert.IsTrue(multipleTemplates.IsMatch(@"{{foo <!--comm--> |title=abc}}"));
            ClassicAssert.IsTrue(multipleTemplates.IsMatch(@"{{
foo<!--comm-->|title=abc
}}"));
            ClassicAssert.IsTrue(multipleTemplates.IsMatch(@"{{foo|title={{abc}}}}"));
            ClassicAssert.IsTrue(multipleTemplates.IsMatch(@"{{foo|title={{abc}}|last=Fred}}"));

            ClassicAssert.IsFalse(multipleTemplates.IsMatch(@"{{a}}"));
            ClassicAssert.IsFalse(multipleTemplates.IsMatch(@""));

            ListOfTemplates.Clear();
            ListOfTemplates.Add(@"Foo ");
            multipleTemplates = Tools.NestedTemplateRegex(ListOfTemplates);
            Assert.That(multipleTemplates.Match(@"{{foo}}").Groups[2].Value, Is.EqualTo(@"foo"), "matches correctly from input template name with trailing whitespace");
        }

        [Test]
        public void NestedTemplateRegexListMultiple()
        {
            List<string> listOfTemplates = new List<string>(new[] { "Foo", "bar" });

            Regex multipleTemplates = Tools.NestedTemplateRegex(listOfTemplates);

            Assert.That(multipleTemplates.Match(@"{{foo}}").Groups[2].Value, Is.EqualTo(@"foo"));
            Assert.That(multipleTemplates.Match(@"{{Template:foo}}").Groups[2].Value, Is.EqualTo(@"foo"));
            Assert.That(multipleTemplates.Match(@"{{ template :foo}}").Groups[2].Value, Is.EqualTo(@"foo"));
            Assert.That(multipleTemplates.Match(@"{{ Foo}}").Groups[2].Value, Is.EqualTo(@"Foo"));
            Assert.That(multipleTemplates.Match(@"{{Bar |akjldasf=a}}").Groups[2].Value, Is.EqualTo(@"Bar"));
            Assert.That(multipleTemplates.Match(@"{{bar
}}").Groups[2].Value, Is.EqualTo(@"bar"));
            Assert.That(multipleTemplates.Match(@"{{ foo}}").Groups[1].Value, Is.EqualTo(@"{{ "));
            Assert.That(multipleTemplates.Match(@"{{ foo |akjldasf=a}}").Groups[3].Value, Is.EqualTo(@" |akjldasf=a}}"));

            ClassicAssert.IsTrue(multipleTemplates.IsMatch(@"{{foo}}"));
            ClassicAssert.IsTrue(multipleTemplates.IsMatch(@"{{Foo}}"));
            ClassicAssert.IsTrue(multipleTemplates.IsMatch(@"{{ foo}}"));
            ClassicAssert.IsTrue(multipleTemplates.IsMatch(@"{{ foo|}}"));
            ClassicAssert.IsTrue(multipleTemplates.IsMatch(@"{{foo|title=abc}}"));
            ClassicAssert.IsTrue(multipleTemplates.IsMatch(@"{{foo|
title=abc
|other=yes}}"));
            ClassicAssert.IsTrue(multipleTemplates.IsMatch(@"{{foo
|title=abc
|other=yes}}"));
            ClassicAssert.IsTrue(multipleTemplates.IsMatch(@"{{foo<!--comm-->|title=abc}}"));
            ClassicAssert.IsTrue(multipleTemplates.IsMatch(@"{{foo <!--comm--> |title=abc}}"));
            ClassicAssert.IsTrue(multipleTemplates.IsMatch(@"{{
foo<!--comm-->|title=abc
}}"));
            ClassicAssert.IsTrue(multipleTemplates.IsMatch(@"{{foo|title={{abc}}}}"));
            ClassicAssert.IsTrue(multipleTemplates.IsMatch(@"{{foo|title={{abc}}|last=Fred}}"));

            ClassicAssert.IsTrue(multipleTemplates.IsMatch(@"{{bar}}"));
            ClassicAssert.IsTrue(multipleTemplates.IsMatch(@"{{ bar}}"));
            ClassicAssert.IsTrue(multipleTemplates.IsMatch(@"{{Bar}}"));

            ClassicAssert.IsFalse(multipleTemplates.IsMatch(@"{{a}}"));
            ClassicAssert.IsFalse(multipleTemplates.IsMatch(@""));
        }

        [Test]
        public void MergeTemplateParametersTemplateName()
        {
            string correct = @"{{Foo | e=b d }}";
            List<string> ToMerge = new List<string>(new[] { "a", "c" });

            Assert.That(Tools.MergeTemplateParametersValues(@"{{Foo|a= b|c= d}}", ToMerge, "e", true), Is.EqualTo(correct), "single spaces");
            Assert.That(Tools.MergeTemplateParametersValues(@"{{Foo|a=b|c=d}}", ToMerge, "e", true), Is.EqualTo(correct), "no spaces");
            Assert.That(Tools.MergeTemplateParametersValues(@"{{Foo|a=   b|c=d}}", ToMerge, "e", true), Is.EqualTo(correct), "big spaces");
        }

        [Test]
        public void GetMetaContentValue()
        {
            Assert.That(Tools.GetMetaContentValue(@"<meta name=""PubDate""  content=""2009-03-02"">", "PubDate"), Is.EqualTo(@"2009-03-02"));
            Assert.That(Tools.GetMetaContentValue(@"<meta name='PubDate'  content='2009-03-02'>", "PubDate"), Is.EqualTo(@"2009-03-02"));
            Assert.That(Tools.GetMetaContentValue(@"< META NAME = ""PubDate""  content = ""2009-03-02""  />", "PubDate"), Is.EqualTo(@"2009-03-02"));
            Assert.That(Tools.GetMetaContentValue(@"< meta itemprop = ""PubDate""  content = ""2009-03-02""  />", "PubDate"), Is.EqualTo(@"2009-03-02"));
            Assert.That(Tools.GetMetaContentValue(@"<meta name  =""PubDate"" CONTENT="" 2009-03-02 "">", "PUBDATE"), Is.EqualTo(@"2009-03-02"));
            Assert.That(Tools.GetMetaContentValue(@"<meta name  =""PubDate"" scheme=""URI"" CONTENT="" 2009-03-02 "">", "PUBDATE"), Is.EqualTo(@"2009-03-02"));
            Assert.That(Tools.GetMetaContentValue(@"<meta property  =""PubDate"" scheme=""URI"" CONTENT="" 2009-03-02 "">", "PUBDATE"), Is.EqualTo(@"2009-03-02"));
            Assert.That(Tools.GetMetaContentValue(@"<meta property=""og:title"" content=""Football: Ken P is headed to X"" data-meta-updatable/>", "og:title"), Is.EqualTo(@"Football: Ken P is headed to X"));

            Assert.That(Tools.GetMetaContentValue(@"<meta name  =""PubDate"" CONTENT="" 2009-03-02 "">", "PUBDATEXX"), Is.Empty);
            Assert.That(Tools.GetMetaContentValue(@"<meta name  =""PubDateX"" CONTENT="" 2009-03-02 "">", "PUBDATE"), Is.Empty);

            Assert.That(Tools.GetMetaContentValue(@"<meta name  =""PubDateX"" CONTENT="" 2009-03-02 "">", ""), Is.Empty);
            Assert.That(Tools.GetMetaContentValue("", "PUBDATE"), Is.Empty);

            Assert.That(Tools.GetMetaContentValue(@"<meta xmlns=""http://www.w3.org/1999/xhtml"" name=""citation_doi"" content=""10.1111/j.1096-0031.2009.00267.x"" />", "citation_doi"), Is.EqualTo(@"10.1111/j.1096-0031.2009.00267.x"));
            Assert.That(Tools.GetMetaContentValue(@"<meta content=""10.1093/nar/27.19.3821"" name=""DC.Identifier"" />", "DC.Identifier"), Is.EqualTo(@"10.1093/nar/27.19.3821"));
            Assert.That(Tools.GetMetaContentValue(@"<meta content=""10.1101/gr.7.4.359"" name=""DC.Identifier"" />", "DC.Identifier"), Is.EqualTo(@"10.1101/gr.7.4.359"));

            Assert.That(Tools.GetMetaContentValue(@"<meta id=""og_title"" property=""og:title"" content=""Air wasn't x""/>", "og:title"), Is.EqualTo(@"Air wasn't x"));
            Assert.That(Tools.GetMetaContentValue(@"<meta property=""og:title"" content=""Air wasn't x"">", "og:title"), Is.EqualTo(@"Air wasn't x"));
            Assert.That(Tools.GetMetaContentValue(@"<meta data-ephemeral=""true"" property=""og:title"" content=""Air wasn't x""/>", "og:title"), Is.EqualTo(@"Air wasn't x"));
            Assert.That(Tools.GetMetaContentValue(@"<meta data-react-helmet=""true"" property=""og:title"" content=""Air wasn't x""/>", "og:title"), Is.EqualTo(@"Air wasn't x"));

            // <meta data-react-helmet="true" name="citation_doi" content="10.1016/0022-0000(78)90043-0"/
            Assert.That(Tools.GetMetaContentValue(@"<meta ng-attr-content=""{{meta.title}}"" property=""og:title"" content=""Air wasn't x""/>", "og:title"), Is.EqualTo(@"Air wasn't x"));
        }
        
        [Test]
        public void UnescapeXML()
        {
            Assert.That(Tools.UnescapeXML(@"<tag>value</tag>"), Is.EqualTo(@"<tag>value</tag>"));
            Assert.That(Tools.UnescapeXML(""), Is.Empty);
            Assert.That(Tools.UnescapeXML(@"<tag>A&amp;B</tag>"), Is.EqualTo(@"<tag>A&B</tag>"));
        }

        [Test]
        public void GetTemplateName()
        {
            Assert.That(Tools.GetTemplateName(@"{{Start date and age|1833|7|11}}"), Is.EqualTo("Start date and age"));

            // whitespace handling
            Assert.That(Tools.GetTemplateName(@"{{ Start date and age |1833|7|11}}"), Is.EqualTo("Start date and age"));
            Assert.That(Tools.GetTemplateName(@"{{
Start date and age
|1833|7|11}}"), Is.EqualTo("Start date and age"));

            Assert.That(Tools.GetTemplateName(@"{{start date and age <!--comm--> |1833|7|11}}"), Is.EqualTo("start date and age"), "handles embedded comments");
            Assert.That(Tools.GetTemplateName(@"{{start date and age <!--comm-->}}"), Is.EqualTo("start date and age"), "handles embedded comments");
            Assert.That(Tools.GetTemplateName(@"{{start date and age ⌊⌊⌊⌊1⌋⌋⌋⌋}}"), Is.EqualTo("start date and age"), "handles embedded comments (hidetext)");

            Assert.That(Tools.GetTemplateName(@"{{Start date and age|1833|7|"), Is.EqualTo("Start date and age"), "works on part templates");

            Assert.That(Tools.GetTemplateName(@"{{Template:Foo|1=yes}}"), Is.EqualTo("Foo"), "Template namespace removed");
            Assert.That(Tools.GetTemplateName(@"{{ Template : Foo |1=yes}}"), Is.EqualTo("Foo"), "Template namespace removed");
            Assert.That(Tools.GetTemplateName(@"{{template:Foo|1=yes}}"), Is.EqualTo("Foo"), "Template namespace removed");
            Assert.That(Tools.GetTemplateName(@"{{Foo_one|1=yes}}"), Is.EqualTo("Foo one"), "underscore cleaned");
            Assert.That(Tools.GetTemplateName(@"{{_Foo_one|1=yes}}"), Is.EqualTo("Foo one"), "Leading underscore cleaned");
            Assert.That(Tools.GetTemplateName(@"{{Foo___one|1=yes}}"), Is.EqualTo("Foo one"), "underscores cleaned");
            Assert.That(Tools.GetTemplateName(@"{{Foo   one|1=yes}}"), Is.EqualTo("Foo one"), "underscores cleaned");

            Assert.That(Tools.GetTemplateName(@"{{DISPLAYTITLE:11}}"), Is.EqualTo("DISPLAYTITLE"));
            Assert.That(Tools.GetTemplateName(@"{{DISPLAYTITLE:}}"), Is.EqualTo("DISPLAYTITLE"));

            Assert.That(Tools.GetTemplateName(@""), Is.EqualTo(""));

            Variables.NamespacesCaseInsensitive.Remove(Namespace.Template);
            Assert.That(Tools.GetTemplateName(@""), Is.EqualTo(""));
            Variables.NamespacesCaseInsensitive.Add(Namespace.Template, "[Tt]emplate:");
        }

        [Test]
        public void ReAddDiacritics()
        {
            Assert.That(Tools.ReAddDiacritics(@"Floué, John", @"Floue, John"), Is.EqualTo(@"Floué, John"), "diacritics reapplied");
            Assert.That(Tools.ReAddDiacritics(@"Floué", @"Floue"), Is.EqualTo(@"Floué"), "diacritics reapplied");
            Assert.That(Tools.ReAddDiacritics(@" Floué, John ", @"Floue, John"), Is.EqualTo(@"Floué, John"), "diacritics reapplied");
            Assert.That(Tools.ReAddDiacritics(@" Floué, John ", @" Floue, John "), Is.EqualTo(@" Floué, John "), "diacritics reapplied");

            Assert.That(Tools.ReAddDiacritics(@"John Floué", @"Floue, John"), Is.EqualTo(@"Floué, John"), "diacritics reapplied, word order irrelevant");
            Assert.That(Tools.ReAddDiacritics(@"John von Floué", @"Floue, John"), Is.EqualTo(@"Floué, John"), "diacritics reapplied, word order irrelevant");

            Assert.That(Tools.ReAddDiacritics(@"Greatère, Véry", @"Greatere, Very"), Is.EqualTo(@"Greatère, Véry"), "multiple words changed");
            Assert.That(Tools.ReAddDiacritics(@"Véry Greatère", @"Greatere, Very"), Is.EqualTo(@"Greatère, Véry"), "multiple words changed");
            Assert.That(Tools.ReAddDiacritics(@"Véry Greatère", @"Greatere, Very der Very"), Is.EqualTo(@"Greatère, Very der Very"), "when multiple matches for same word without, that word not changed");
            Assert.That(Tools.ReAddDiacritics(@"Véry de Vèry Greatère", @"Greatere, Very der Very"), Is.EqualTo(@"Greatère, Very der Very"), "when multiple matches for same word without, that word not changed");
        }
        
        [Test]
        public void IsIP()
        {
            ClassicAssert.IsTrue(Tools.IsIP("192.168.0.1"));
            ClassicAssert.IsTrue(Tools.IsIP("8.8.8.8"));
            ClassicAssert.IsFalse(Tools.IsIP("www.google.com"));
        }

        [Test]
        public void BuildPostDataString()
        {
            System.Collections.Specialized.NameValueCollection nvc = new System.Collections.Specialized.NameValueCollection();

            nvc.Add("param1", "value1");
            Assert.That(Tools.BuildPostDataString(nvc), Is.EqualTo("param1=value1"));
            nvc.Add("param2", "value2");
            Assert.That(Tools.BuildPostDataString(nvc), Is.EqualTo("param1=value1&param2=value2"));
            nvc.Add("param3", "A B C");
            Assert.That(Tools.BuildPostDataString(nvc), Is.EqualTo("param1=value1&param2=value2&param3=A+B+C"));
        }

        [Test]
        public void OpenInBRowser()
        {
            Tools.OpenURLInBrowser("https://en.wikipedia.org");
            Tools.OpenArticleInBrowser("A");
            Tools.OpenArticleHistoryInBrowser("A");
            Tools.OpenENArticleInBrowser("A", true);
            Tools.OpenENArticleInBrowser("A", false);
            Tools.OpenUserTalkInBrowser("A");
            Tools.OpenArticleLogInBrowser("A");
            Tools.EditArticleInBrowser("A");
        }

        [Test]
        public void TemplateToMagicWord()
        {
            Assert.That(Tools.TemplateToMagicWord(@"{{DEFAULTSORT|Foo}}"), Is.EqualTo(@"{{DEFAULTSORT:Foo}}"));
            Assert.That(Tools.TemplateToMagicWord(@"{{DISPLAYTITLE|Foo}}"), Is.EqualTo(@"{{DISPLAYTITLE:Foo}}"));
            Assert.That(Tools.TemplateToMagicWord(@"{{Displaytitle|Foo}}"), Is.EqualTo(@"{{DISPLAYTITLE:Foo}}"));
            Assert.That(Tools.TemplateToMagicWord(@"{{FULLPAGENAME|Foo}}"), Is.EqualTo(@"{{FULLPAGENAME:Foo}}"));
            Assert.That(Tools.TemplateToMagicWord(@"{{Fullpagename|Foo}}"), Is.EqualTo(@"{{FULLPAGENAME:Foo}}"));
            Assert.That(Tools.TemplateToMagicWord(@"{{Namespace|Foo}}"), Is.EqualTo(@"{{namespace:Foo}}"));
            Assert.That(Tools.TemplateToMagicWord(@"{{Numberofarticles|Foo}}"), Is.EqualTo(@"{{numberofarticles:Foo}}"));
            Assert.That(Tools.TemplateToMagicWord(@"{{PAGENAME|Foo}}"), Is.EqualTo(@"{{PAGENAME:Foo}}"));
            Assert.That(Tools.TemplateToMagicWord(@"{{PAGESIZE|Foo}}"), Is.EqualTo(@"{{PAGESIZE:Foo}}"));
            Assert.That(Tools.TemplateToMagicWord(@"{{PROTECTIONLEVEL|Foo}}"), Is.EqualTo(@"{{PROTECTIONLEVEL:Foo}}"));
            Assert.That(Tools.TemplateToMagicWord(@"{{Pagename|Foo}}"), Is.EqualTo(@"{{PAGENAME:Foo}}"));
            Assert.That(Tools.TemplateToMagicWord(@"{{pagename|Foo}}"), Is.EqualTo(@"{{PAGENAME:Foo}}"));
            Assert.That(Tools.TemplateToMagicWord(@"{{SUBPAGENAME|Foo}}"), Is.EqualTo(@"{{SUBPAGENAME:Foo}}"));
            Assert.That(Tools.TemplateToMagicWord(@"{{Subpagename|Foo}}"), Is.EqualTo(@"{{SUBPAGENAME:Foo}}"));
            Assert.That(Tools.TemplateToMagicWord(@"{{padleft|Foo}}"), Is.EqualTo(@"{{padleft:Foo}}"));

            Assert.That(Tools.TemplateToMagicWord(@"{{ DEFAULTSORT |Foo}}"), Is.EqualTo(@"{{DEFAULTSORT:Foo}}"));
            Assert.That(Tools.TemplateToMagicWord(@"{{DISPLAYTITLE|''Foo''}}"), Is.EqualTo(@"{{DISPLAYTITLE:''Foo''}}"));
            Assert.That(Tools.TemplateToMagicWord(@"{{DISPLAYTITLE|''Foo''}} {{DEFAULTSORT|Foo}}"), Is.EqualTo(@"{{DISPLAYTITLE:''Foo''}} {{DEFAULTSORT:Foo}}"));
            Assert.That(Tools.TemplateToMagicWord(@"{{BASEPAGENAME|Foo}}"), Is.EqualTo(@"{{BASEPAGENAME:Foo}}"));
            Assert.That(Tools.TemplateToMagicWord(@"{{FULLPAGENAME}}"), Is.EqualTo(@"{{FULLPAGENAME}}"));
        }
        
        [Test]
        public void IsSectionOrReasonTemplate()
        {
            ClassicAssert.IsTrue(Tools.IsSectionOrReasonTemplate(@"{{wikify|reason=foo}}"));
            ClassicAssert.IsTrue(Tools.IsSectionOrReasonTemplate(@"{{wikify|date=May 2012|reason=foo}}"));
            ClassicAssert.IsTrue(Tools.IsSectionOrReasonTemplate(@"{{wikify|section|date=May 2012}}"));
            ClassicAssert.IsTrue(Tools.IsSectionOrReasonTemplate(@"{{wikify|section|date=May 2012}}", ""));
            ClassicAssert.IsTrue(Tools.IsSectionOrReasonTemplate(@"{{foo}}", @"{{multiple issues|section=y|POV=May 2012}}"));
            
            ClassicAssert.IsFalse(Tools.IsSectionOrReasonTemplate(@"{{abc|param1=foo}}"));
            ClassicAssert.IsFalse(Tools.IsSectionOrReasonTemplate(@"{{abc|section=foo}}"));
            ClassicAssert.IsFalse(Tools.IsSectionOrReasonTemplate(@"{{abc}}"));
        }
        
        [Test]
        public void HowMuchStartsWithTests()
        {
            Assert.That(Tools.HowMuchStartsWith(@"{{foo}}
hello", Tools.NestedTemplateRegex("foo"), false), Is.EqualTo(9));

            Assert.That(Tools.HowMuchStartsWith(@"{{foo}}
hello", Tools.NestedTemplateRegex("foo"), true), Is.EqualTo(9));

            Assert.That(Tools.HowMuchStartsWith(@"{{foo}}
hello {{foo}}", Tools.NestedTemplateRegex("foo"), false), Is.EqualTo(9));

            Assert.That(Tools.HowMuchStartsWith(@" {{foo}}
hello", Tools.NestedTemplateRegex("foo"), false), Is.EqualTo(10));

            Assert.That(Tools.HowMuchStartsWith(@"hello{{foo}}
hello", Tools.NestedTemplateRegex("foo"), false), Is.EqualTo(0));

            Assert.That(Tools.HowMuchStartsWith(@"==hello==
{{foo}}
hello", Tools.NestedTemplateRegex("foo"), false), Is.EqualTo(0));

            Assert.That(Tools.HowMuchStartsWith(@"hello{{foo}}
hello", Tools.NestedTemplateRegex("foo"), true), Is.EqualTo(0));

            Assert.That(Tools.HowMuchStartsWith(@"{{foo}}
{{foo}}
hello", Tools.NestedTemplateRegex("foo"), false), Is.EqualTo(18));

            Assert.That(Tools.HowMuchStartsWith(@"{{foo}}
{{foo|bar}}
hello", Tools.NestedTemplateRegex("foo"), false), Is.EqualTo(22));

            Assert.That(Tools.HowMuchStartsWith(@"==hello==
{{foo}}
hello", Tools.NestedTemplateRegex("foo"), true), Is.EqualTo(20));

            Assert.That(Tools.HowMuchStartsWith(@"===hello===
{{foo}}
hello", Tools.NestedTemplateRegex("foo"), true), Is.EqualTo(22));

            Assert.That(Tools.HowMuchStartsWith(@"{{foo}}
hello {{foo}}
==hi==
text", Tools.NestedTemplateRegex("foo"), false), Is.EqualTo(9));

            Assert.That(Tools.HowMuchStartsWith(@"{{foo}}
hello {{foo}}
==hi==
text", Tools.NestedTemplateRegex("foo"), true), Is.EqualTo(9));
            Assert.That(Tools.HowMuchStartsWith(@"===hello===
hello", Tools.NestedTemplateRegex("foo"), true), Is.EqualTo(0));
        }

        [Test]
        public void PipeCleanedTemplate()
        {
            Assert.That(Tools.PipeCleanedTemplate(@"{{cite journal|title=A}}", true), Is.EqualTo(@"{{cite journal|title=A}}"));
            Assert.That(Tools.PipeCleanedTemplate(@"{{cite journal|title=A <!-- a --> }}", true), Is.EqualTo(@"{{cite journal|title=A ~~~~~~~~~~ }}"));
            Assert.That(Tools.PipeCleanedTemplate(@"{{cite journal|title=A [[here|there]] }}", true), Is.EqualTo(@"{{cite journal|title=A ############## }}"));
            Assert.That(Tools.PipeCleanedTemplate(@"{{cite journal|title=A [[here]] }}", true), Is.EqualTo(@"{{cite journal|title=A ######## }}"));
            Assert.That(Tools.PipeCleanedTemplate(@"{{cite journal|title=A {{here}} }}", true), Is.EqualTo(@"{{cite journal|title=A ######## }}"));
            Assert.That(Tools.PipeCleanedTemplate(@"{{cite journal|title=A <pre>a</pre> }}", true), Is.EqualTo(@"{{cite journal|title=A ############ }}"));
            Assert.That(Tools.PipeCleanedTemplate(@"{{cite journal|title=A <code>a</code> }}", true), Is.EqualTo(@"{{cite journal|title=A ############## }}"));
        }

        [Test]
        public void UnformattedTextNotChanged()
        {
            ClassicAssert.IsTrue(Tools.UnformattedTextNotChanged("", ""));
            ClassicAssert.IsTrue(Tools.UnformattedTextNotChanged("A", "A"));
            ClassicAssert.IsTrue(Tools.UnformattedTextNotChanged("<nowiki>A</nowiki>", "<nowiki>A</nowiki>"));
            ClassicAssert.IsTrue(Tools.UnformattedTextNotChanged("<nowiki>A</nowiki> <nowiki>B</nowiki>", "<nowiki>A</nowiki> <nowiki>B</nowiki>"));
            ClassicAssert.IsTrue(Tools.UnformattedTextNotChanged("<nowiki>A</nowiki> <nowiki>B</nowiki>", "<nowiki>A</nowiki>"), "Unformatted text entirely removed, true");
            ClassicAssert.IsTrue(Tools.UnformattedTextNotChanged("<nowiki>A</nowiki>", ""), "Unformatted text entirely removed, true");

            ClassicAssert.IsFalse(Tools.UnformattedTextNotChanged("<nowiki>A</nowiki> <nowiki>B</nowiki>", "<nowiki>C</nowiki>"));
            ClassicAssert.IsFalse(Tools.UnformattedTextNotChanged("<nowiki>A</nowiki>", "<nowiki></nowiki>"));
            ClassicAssert.IsFalse(Tools.UnformattedTextNotChanged("<nowiki>A</nowiki>", "<nowiki>B</nowiki>"), "Unformatted text changed removed, false");
            ClassicAssert.IsFalse(Tools.UnformattedTextNotChanged("<nowiki>A</nowiki>", "<nowiki></nowiki>B"), "Unformatted text changed (no content) removed, false");
        }

        [Test]
        public void GetMd5Sum()
        {
            Assert.That(Tools.GetMd5Sum("hello"), Is.EqualTo("5d41402abc4b2a76b9719d911017c592"));
        }

        [Test]
        public void SortDictionaryPairs()
        {
            Assert.That(
                Tools.SortDictionaryPairs(
                    new Dictionary<string, string>
                    {
                        ["InfoA"] = "foo",
                        ["InfoB"] = "Bar",
                        ["InfoC"] = "Baz"
                    },
                    new List<string>
                    {
                        "InfoC",
                        "InfoA",
                        "InfoB"
                    }
                ),
                Is.EqualTo(new Dictionary<string, string>
                {
                    ["InfoC"] = "Baz",
                    ["InfoA"] = "foo",
                    ["InfoB"] = "Bar"
                }).AsCollection
            );

            Assert.That(
                Tools.SortDictionaryPairs(
                    new Dictionary<string, string>
                    {
                        ["InfoA"] = "foo",
                        ["InfoB"] = "Bar",
                        ["InfoC"] = "Baz",
                        ["InfoZ"] = "Zed",
                        ["InfoY"] = "Why"
                    },
                    new List<string>
                    {
                        "InfoC",
                        "InfoA",
                        "InfoB"
                    }
                ),
                Is.EqualTo(new Dictionary<string, string>
                {
                    ["InfoC"] = "Baz",
                    ["InfoA"] = "foo",
                    ["InfoB"] = "Bar",
                    ["InfoZ"] = "Zed",
                    ["InfoY"] = "Why"
                }).AsCollection
            );
        }

        [Test]
        public void SortTemplateCallParameters()
        {
            Assert.That(
                Tools.SortTemplateCallParameters(
                    @"{{MyTemplate
| InfoA = foo
| InfoB = Bar
| InfoC = Baz
}}",
                    new List<string>
                    {
                        "InfoC",
                        "InfoA",
                        "InfoB"
                    }
                ),
                Is.EqualTo(@"{{MyTemplate | InfoC=Baz | InfoA=foo | InfoB=Bar
}}")
            );
        }
    }

    [TestFixture]
    public class HumanCatKeyTests : RequiresInitialization
    {
        [Test]
        public void MakeHumanCatKeyOneWordNames()
        {
            Assert.That(Tools.MakeHumanCatKey("OneWordName", ""), Is.EqualTo("OneWordName"));
            Assert.That(Tools.MakeHumanCatKey("ONEWORDNAME", ""), Is.EqualTo("ONEWORDNAME"));
            Assert.That(Tools.MakeHumanCatKey("Onewordname", ""), Is.EqualTo("Onewordname"));
            Assert.That(Tools.MakeHumanCatKey("onewordname", ""), Is.EqualTo("onewordname"));

            ClassicAssert.IsTrue(Tools.MakeHumanCatKey(@"Friends of the Mission Clinic of Our Lady of Guadalupe, Inc.", "Test").Length > 0);
        }

        [Test]
        public void MakeHumanCatKeyPersonOfPlace()
        {
            Assert.That(Tools.MakeHumanCatKey("Foo of London", ""), Is.EqualTo("Foo of London"));
            Assert.That(Tools.MakeHumanCatKey("Foé of London", ""), Is.EqualTo("Foe of London"));
            Assert.That(Tools.MakeHumanCatKey("Foo II of London", ""), Is.EqualTo("Foo 02 of London"));
            Assert.That(Tools.MakeHumanCatKey("Foo XI of London", ""), Is.EqualTo("Foo 11 of London"));

            Assert.That(Tools.MakeHumanCatKey("Clinoch of Alt Clut", ""), Is.EqualTo("Clinoch of Alt Clut"), "Person of place with two-word place name");
            Assert.That(Tools.MakeHumanCatKey("Byzantine Master of the Crucifix of Pisa", ""), Is.EqualTo("Byzantine Master of the Crucifix of Pisa"));
        }

        [Test]
        public void MakeHumanCatKeyWithRomanNumbers()
        {
            Assert.That(Tools.MakeHumanCatKey("John Doe III", ""), Is.EqualTo("Doe, John III"));
            Assert.That(Tools.MakeHumanCatKey("John III", ""), Is.EqualTo("John III"));
            Assert.That(Tools.MakeHumanCatKey("XVII", ""), Is.EqualTo("XVII"));
            Assert.That(Tools.MakeHumanCatKey("John Doe King of Spain III", ""), Is.EqualTo("Spain, John Doe King of III"));
        }

        [Test]
        public void MakeHumanCatKeyWithJrSr()
        {
            Assert.That(Tools.MakeHumanCatKey("John Doe Jr.", ""), Is.EqualTo("Doe, John Jr."));
            Assert.That(Tools.MakeHumanCatKey("John Doe Sr.", ""), Is.EqualTo("Doe, John Sr."));
            Assert.That(Tools.MakeHumanCatKey("John Doe Jnr.", ""), Is.EqualTo("Doe, John Jnr."));
            Assert.That(Tools.MakeHumanCatKey("John Doe Snr.", ""), Is.EqualTo("Doe, John Snr."));
            Assert.That(Tools.MakeHumanCatKey(@"Carey Elmore Morgan Jr.", ""), Is.EqualTo("Morgan, Carey Elmore Jr."));

            Assert.That(Tools.MakeHumanCatKey("John Doe Snr.", ""), Is.EqualTo("Doe, John Snr."));
            Assert.That(Tools.MakeHumanCatKey("Steven A. Hickham Jr.", ""), Is.EqualTo("Hickham, Steven A. Jr."));
            Assert.That(Tools.MakeHumanCatKey("Steven A. Hickham Jnr.", ""), Is.EqualTo("Hickham, Steven A. Jnr."));
            Assert.That(Tools.MakeHumanCatKey("Steven Hickham Jr.", ""), Is.EqualTo("Hickham, Steven Jr."));
        }

        [Test]
        public void MakeHumanCatKeyWithApostrophes()
        {
            Assert.That(Tools.MakeHumanCatKey("J'ohn D'Doe", ""), Is.EqualTo("DDoe, John"));
            Assert.That(Tools.MakeHumanCatKey("'Test", ""), Is.EqualTo("Test"));
            Assert.That(Tools.MakeHumanCatKey("Lillian O’Donnell", ""), Is.EqualTo("ODonnell, Lillian"));
            Assert.That(Tools.MakeHumanCatKey(", Word", ""), Is.EqualTo("Word"));
        }

        [Test]
        public void MakeHumanCatKeyWithPrefixes()
        {
            Assert.That(Tools.MakeHumanCatKey("John de Doe", ""), Is.EqualTo("Doe, John de"));
        }

        [Test]
        public void MakeHumanCatKeyDiacritics()
        {
            Assert.That(Tools.MakeHumanCatKey("Ďöê", ""), Is.EqualTo("Doe"));
            Assert.That(Tools.MakeHumanCatKey("Ĵǒħń Ďöê", ""), Is.EqualTo("Doe, John"));

            Assert.That(Tools.MakeHumanCatKey("Gu, Prince Imperial Hoeun", ""), Is.EqualTo(@"Gu, Prince Imperial Hoeun"));

            Assert.That(Tools.MakeHumanCatKey("Ёпрстий", ""), Is.EqualTo("Eпрcтии"));
        }

        [Test]
        public void MakeHumanCatKeyArabicNames()
        {
            Assert.That(Tools.MakeHumanCatKey(@"Ahmed Mohammed Mukit", ""), Is.EqualTo(@"Ahmed Mohammed Mukit"), "no change");
            Assert.That(Tools.MakeHumanCatKey(@"AHMED Mohammed MUKIT", ""), Is.EqualTo(@"AHMED Mohammed MUKIT"), "no change");
            Assert.That(Tools.MakeHumanCatKey(@"ahmed Mohammed mukit", ""), Is.EqualTo(@"ahmed Mohammed mukit"), "no change");

            Assert.That(Tools.MakeHumanCatKey(@"John Smith", ""), Is.EqualTo(@"Smith, John"));
        }

        [Test]
        public void MakeHumanCatKeyMcName()
        {
            Assert.That(Tools.MakeHumanCatKey(@"John McSmith", ""), Is.EqualTo(@"McSmith, John"));
            Assert.That(Tools.MakeHumanCatKey(@"John MacSmith", ""), Is.EqualTo(@"MacSmith, John"));

            Assert.That(Tools.MakeHumanCatKey(@"John Mcsmith", ""), Is.EqualTo(@"Mcsmith, John"));

            Assert.That(Tools.MakeHumanCatKey(@"John Smith", ""), Is.EqualTo(@"Smith, John"));
            Assert.That(Tools.MakeHumanCatKey(@"John Macintosh", ""), Is.EqualTo(@"Macintosh, John"));
        }

        [Test]
        public void MakeHumanCatKeyFamilyName()
        {
            Assert.That(Tools.MakeHumanCatKey(@"Kong Qingdong", "{{Chinese name}}"), Is.EqualTo(@"Kong Qingdong"));
            Assert.That(Tools.MakeHumanCatKey(@"Kong Qingdong", "{{foo}"), Is.EqualTo(@"Qingdong, Kong"));
        }

        [Test]
        public void RemoveDiacritics()
        {
            foreach (var p in Tools.Diacritics)
            {
                Assert.That(Tools.RemoveDiacritics(p[0]), Is.EqualTo(p[1]));
            }

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#Leaving_foreign_characters_in_DEFAULTSORT
            Assert.That(Tools.RemoveDiacritics(@"ầắạảằẩ"), Is.EqualTo(@"aaaaaa"), "a");
            Assert.That(Tools.RemoveDiacritics(@"ḉćĉçċ"), Is.EqualTo(@"ccccc"), "c");
            Assert.That(Tools.RemoveDiacritics(@"ḕềẹĕ"), Is.EqualTo(@"eeee"), "e");
            Assert.That(Tools.RemoveDiacritics(@"ĥḫ"), Is.EqualTo(@"hh"), "h");
            Assert.That(Tools.RemoveDiacritics(@"ịỉíįí"), Is.EqualTo(@"iiiii"), "i");
            Assert.That(Tools.RemoveDiacritics(@"İ"), Is.EqualTo(@"I"), "I");
            Assert.That(Tools.RemoveDiacritics(@"òỏøờồȱȯȭȫoỗơ"), Is.EqualTo(@"oooooooooooo"), "o");
            Assert.That(Tools.RemoveDiacritics(@"Ø"), Is.EqualTo(@"O"), "null");
            Assert.That(Tools.RemoveDiacritics(@"š"), Is.EqualTo(@"s"), "s");
            Assert.That(Tools.RemoveDiacritics(@"ụủữự"), Is.EqualTo(@"uuuu"), "u");
            Assert.That(Tools.RemoveDiacritics(@"x̌"), Is.EqualTo(@"x"), "x");
            Assert.That(Tools.RemoveDiacritics(@"ỳỵ"), Is.EqualTo(@"yy"), "y");
            Assert.That(Tools.RemoveDiacritics(@"ḏ p̄ Ś̄"), Is.EqualTo(@"d p S"), "Random");
            Assert.That(Tools.RemoveDiacritics(@"²"), Is.EqualTo(@"2"), "2");
            Assert.That(Tools.RemoveDiacritics(@"Ǣ"), Is.EqualTo(@"Ae"));
            Assert.That(Tools.RemoveDiacritics(@"ǣ"), Is.EqualTo(@"ae"));
            Assert.That(Tools.RemoveDiacritics(@"ȦȧḂḃĊċḊḋĖėḞḟĠġḢḣİıṀṁṄṅȮȯṖṗṘṙṠṡṪṫẆẇẊẋẎẏŻż"), Is.EqualTo(@"AaBbCcDdEeFfGgHhIiMmNnOoPpRrSsTtWwXxYyZz"), "letters with dot above sign");
            Assert.That(Tools.RemoveDiacritics(@"ĀāĂăĄąĆćĈĉĊċČčĎďĐđĒēĔĕĖėĘęĚěĜĝĞğĠġĢģĤĥĦħĨĩĪīĬĭĮįİıĴĵĶķĸĹĺĻļĽľĿŀŁłŃńŅņŇňŉŊŋŌōŎŏŐőŔŕŖŗŘř"), Is.EqualTo(@"AaAaAaCcCcCcCcDdDdEeEeEeEeEeGgGgGgGgHhHhIiIiIiIiIiJjKkkLlLlLlLlLlNnNnNnnNnOoOoOoRrRrRr"), "extended Latin-A part 1");
            Assert.That(Tools.RemoveDiacritics(@"ŚśŜŝŞşŠšŢţŤťŦŧŨũŪūŬŭŮůŰűŲųŴŵŶŷŸŹźŻżŽžſ"), Is.EqualTo(@"SsSsSsSsTtTtTtUuUuUuUuUuUuWwYyYZzZzZzs"), "extended Latin-A part 2");
            Assert.That(Tools.RemoveDiacritics(@"ǍǎǏǐǑǒǓǔǕǖǗǘǙǚǛǜǝǞǟǠǡǤǥǦǧǨǩǪǫǬǭǸǹǺǻǾǿȀȁȂȃȄȅȆȇȈȉȊȋȌȍȎȏȐȑȒȓȔȕȖȗȘșȚț"), Is.EqualTo(@"AaIiOoUuUuUuUuUueAaAaGgGgKkOoOoNnAaOoAaAaEeEeIiIiOoOoRrRrUuUuSsTt"), "extended Latin-B");
            Assert.That(Tools.RemoveDiacritics(@"ḀḁḂḃḄḅḆḇḈḉḊḋḌḍḎḏḐḑḒḓḔḕḖḗḘḙḚḛḜḝḞḟḠḡḢḣḤḥḦḧḨḩḪḫḬḭḮḯḰḱḲḳḴḵḶḷḸḹḺḻḼḽḾḿṀṁṂṃṄṅṆṇṈṉṊṋ"), Is.EqualTo(@"AaBbBbBbCcDdDdDdDdDdEeEeEeEeEeFfGgHhHhHhHhHhIiIiKkKkKkLlLlLlLlMmMmMmNnNnNnNn"), "Latin Extended Additional A-N");
            }

        [Test]
        public void CleanSortKey()
        {
            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_20#Not_replacing_.26_with_.22and.22_in_sort_values
            Assert.That(Tools.CleanSortKey(@"&"), Is.EqualTo(@"and"), "per SORTKEY");
            Assert.That(Tools.CleanSortKey(@"A & B"), Is.EqualTo(@"A and B"), "&");
            Assert.That(Tools.CleanSortKey(@"ǀAi-ǀAis"), Is.EqualTo(@"Ai-Ais"), "removes weird character");
            Assert.That(Tools.CleanSortKey(@"ǀAi-ǀAis/Richtersveld Transfrontier Park"), Is.EqualTo(@"Ai-Ais Richtersveld Transfrontier Park"), "removes weird character");
            Assert.That(Tools.CleanSortKey(@"Der Nachtkurier meldet…"), Is.EqualTo(@"Der Nachtkurier meldet..."), "replaces …");
            Assert.That(Tools.CleanSortKey(@"A·L"), Is.EqualTo(@"A L"), "replaces · with space");
            Assert.That(Tools.CleanSortKey(@"Ḇḇ, Ḏḏ, ẖ, Ḵḵ, Ḻḻ, Ṉṉ, Ṟṟ, Ṯṯ"), Is.EqualTo(@"Bb, Dd, h, Kk, Ll, Nn, Rr, Tt"), "letters with macron below");
            Assert.That(Tools.CleanSortKey(@"ɓ, ƈ, ɗ, ƒ, ɠ, ɦ, ƙ, ɲ, ƥ, ʠ, ƭ, ʋ, ⱳ, ƴ"), Is.EqualTo(@"b, c, d, f, g, h, k, n, p, q, t, v, w, y"), "letters with hook");
            Assert.That(Tools.CleanSortKey(@"ᶀ, ᶁ, ᶂ, ᶃ, ᶄ, ᶅ, ᶆ, ᶇ, ᶈ, ᶉ, ᶊ, ƫ, ᶌ, ᶍ, ᶎ"), Is.EqualTo(@"b, d, f, g, k, l, m, n, p, r, s, t, y, x, z"), "letters with palatal hook");
            Assert.That(Tools.CleanSortKey(@"Ąą, Ęę, Įį, Ǫǫ, Ųų"), Is.EqualTo(@"Aa, Ee, Ii, Oo, Uu"), "letters using ogonek sign");
            Assert.That(Tools.CleanSortKey(@"Ȃȃ, Ȇȇ, Ȋȋ, Ȏȏ, Ȗȗ, Ȓȓ"), Is.EqualTo(@"Aa, Ee, Ii, Oo, Uu, Rr"), "letters using inverted breve");
            Assert.That(Tools.CleanSortKey(@"D̦d̦, Șș, Țț"), Is.EqualTo(@"Dd, Ss, Tt"), "letters using comma sign");
            Assert.That(Tools.CleanSortKey(@"’‘ʻ`´“”"), Is.EqualTo(@"'''''''"), "quotes");
            Assert.That(Tools.CleanSortKey(@"1–2–3"), Is.EqualTo(@"1-2-3"), "endash");
            Assert.That(Tools.CleanSortKey(@"1–2&ndash;3"), Is.EqualTo(@"1-2-3"), "&ndash;");
            Assert.That(Tools.CleanSortKey(@"A       "), Is.EqualTo(@"A "), "Excess whitespace");
            Assert.That(Tools.CleanSortKey(@"A/B"), Is.EqualTo(@"A B"), "Forward slash");
            Assert.That(Tools.CleanSortKey("{{<noinclude>BASE</noinclude>PAGENAME}}"), Is.EqualTo("{{<noinclude>BASE</noinclude>PAGENAME}}"), "noinclude tags, no change");
        }

        [Test]
        public void CleanSortKeyLang()
        {
#if DEBUG
            Variables.UnicodeCategoryCollation = true;
            Variables.SetProjectLangCode("ru");
            Assert.That(Tools.CleanSortKey("Hellõ"), Is.EqualTo("Hellõ"), "no diacritic removal for defaultsort key on ru-wiki");
            Variables.SetProjectLangCode("fr");
            Assert.That(Tools.CleanSortKey("Hellõ"), Is.EqualTo("Hellõ"), "no diacritic removal for defaultsort key on fr-wiki");
            Variables.SetProjectLangCode("pl");
            Assert.That(Tools.CleanSortKey("Hellõ"), Is.EqualTo("Hellõ"), "no diacritic removal for defaultsort key on pl-wiki");
            Variables.UnicodeCategoryCollation = false;
            Variables.SetProjectLangCode("en");
            Assert.That(Tools.FixupDefaultSort("Hellõ"), Is.EqualTo("Hello"), "do remove diacritics on en-wiki");
#endif
        }

        [Test]
        public void HasDiacritics()
        {
            ClassicAssert.IsTrue(Tools.HasDiacritics("hellõ"));
            ClassicAssert.IsTrue(Tools.HasDiacritics("hellõ there"));
            ClassicAssert.IsTrue(Tools.HasDiacritics("hẽllõ there"));
            ClassicAssert.IsTrue(Tools.HasDiacritics("hẽllo there"));
            ClassicAssert.IsTrue(Tools.HasDiacritics("İzmir"));

            ClassicAssert.IsFalse(Tools.HasDiacritics("hello"));
            ClassicAssert.IsFalse(Tools.HasDiacritics("abcdefghijklmnopqrstuvwxyz"), "standard Latin alphabet");
            ClassicAssert.IsFalse(Tools.HasDiacritics("0123456789"), "digits");
            ClassicAssert.IsFalse(Tools.HasDiacritics(""), "empty string");
            ClassicAssert.IsFalse(Tools.HasDiacritics("   "), "whitespace");
        }

        [Test]
        public void FixUpDefaultSortTests()
        {
            Assert.That(Tools.FixupDefaultSort("hellõ"), Is.EqualTo("hello"));
            Assert.That(Tools.FixupDefaultSort("hellõ   "), Is.EqualTo("hello"));
            Assert.That(Tools.FixupDefaultSort(@"fred smithson"), Is.EqualTo(@"fred smithson"));
            Assert.That(Tools.FixupDefaultSort(@"De Meriño, Fernando Arturo"), Is.EqualTo(@"De Merino, Fernando Arturo"));
            Assert.That(Tools.FixupDefaultSort(@"OneWordItem"), Is.EqualTo(@"OneWordItem"));
            Assert.That(Tools.FixupDefaultSort(@"Foo (bar)"), Is.EqualTo(@"Foo (bar)"));

            Assert.That(Tools.FixupDefaultSort("Kwakwaka'wakw mythology"), Is.EqualTo("Kwakwaka'wakw mythology"));
            Assert.That(Tools.FixupDefaultSort(@"Peewee's Playhouse"), Is.EqualTo(@"Peewee's Playhouse"));
            Assert.That(Tools.FixupDefaultSort(@"Peewee’s Playhouse"), Is.EqualTo(@"Peewee's Playhouse"));
            Assert.That(Tools.FixupDefaultSort(@"2010 ITF Women's Circuit (July–September)"), Is.EqualTo(@"2010 ITF Women's Circuit (July-September)"));

            Assert.That(Tools.FixupDefaultSort(@"List of Foo"), Is.EqualTo(@"Foo"));
            Assert.That(Tools.FixupDefaultSort(@"List of foos"), Is.EqualTo(@"Foos"));
        }

        [Test]
        public void FixUpDefaultSortHumanName()
        {
            Assert.That(Tools.FixupDefaultSort(@"O'connor, Fred", true), Is.EqualTo(@"Oconnor, Fred"), "apostrophe removed for bio sortkey per [[WP:MCSTJR]]");
            Assert.That(Tools.FixupDefaultSort(@"O'connor Trading", false), Is.EqualTo(@"O'connor Trading"), "apostrophes not removed on non-bio sortkey");
            Assert.That(Tools.FixupDefaultSort(@"Jones,Fred", true), Is.EqualTo(@"Jones, Fred"), "comma spacing");
            Assert.That(Tools.FixupDefaultSort(@"Jones ,Fred", true), Is.EqualTo(@"Jones, Fred"), "comma spacing");
            Assert.That(Tools.FixupDefaultSort(@"Jones, Fred", true), Is.EqualTo(@"Jones, Fred"), "comma spacing: no change if already correct");
        }

        [Test]
        public void FixUpDefaultSortBadChars()
        {
            Assert.That(Tools.FixupDefaultSort(@"fred smitHº"), Is.EqualTo(@"fred smitH"));
            Assert.That(Tools.FixupDefaultSort(@"fred ""smitH"""), Is.EqualTo(@"fred smitH"));
            Assert.That(Tools.FixupDefaultSort(@"Fred ""Smith"""), Is.EqualTo(@"Fred Smith"));
        }

        [Test]
        public void FixUpDefaultSortTestsRu()
        {
#if DEBUG
            Variables.SetProjectLangCode("ru");
            Variables.UnicodeCategoryCollation = true;
            Assert.That(Tools.FixupDefaultSort("Hellõ"), Is.EqualTo("Hellõ"), "no diacritic removal for defaultsort key on ru-wiki");
            
            Variables.SetProjectLangCode("en");
            Variables.UnicodeCategoryCollation = false;
            Assert.That(Tools.FixupDefaultSort("Hellõ"), Is.EqualTo("Hello"));
#endif
        }

        [Test]
        public void RemoveNamespace()
        {
            Assert.That(Tools.MakeHumanCatKey("Wikipedia:John Doe", ""), Is.EqualTo("Doe, John"));
        }

        [Test]
        public void FuzzTest()
        {
            const string allowedChars = "abcdefghijklmnopqrstuvwxyzйцукенфывапролдж              ,,,,,";

            Random rnd = new Random();

            for (int i = 0; i < 10000; i++)
            {
                string name = "";

                for (int j = 0; j < rnd.Next(45); j++)
                {
                    name += allowedChars[rnd.Next(allowedChars.Length)];
                }
                name = Regex.Replace(name, @"\s{2,}", " ").Trim(new[] { ' ', ',' });

                name = Tools.MakeHumanCatKey(name, "");

                ClassicAssert.IsFalse(name.Contains("  "), "Sorting key shouldn't contain consecutive spaces - it breaks the sorting ({0})", name);
                ClassicAssert.IsFalse(name.StartsWith(" "), "Sorting key shouldn't start with spaces");
                ClassicAssert.IsFalse(name.EndsWith(" "), "Sorting key shouldn't end with spaces");
            }
        }

        [Test]
        public void RomanToInt()
        {
            Assert.That(Tools.RomanToInt("I"), Is.EqualTo("01"));
            Assert.That(Tools.RomanToInt("II"), Is.EqualTo("02"));
            Assert.That(Tools.RomanToInt("III"), Is.EqualTo("03"));
            Assert.That(Tools.RomanToInt("IV"), Is.EqualTo("04"));
            Assert.That(Tools.RomanToInt("V"), Is.EqualTo("05"));
            Assert.That(Tools.RomanToInt("VI"), Is.EqualTo("06"));
            Assert.That(Tools.RomanToInt("VII"), Is.EqualTo("07"));
            Assert.That(Tools.RomanToInt("VIII"), Is.EqualTo("08"));
            Assert.That(Tools.RomanToInt("IX"), Is.EqualTo("09"));
            Assert.That(Tools.RomanToInt("X"), Is.EqualTo("10"));
            Assert.That(Tools.RomanToInt("XI"), Is.EqualTo("11"));
            Assert.That(Tools.RomanToInt("XIV"), Is.EqualTo("14"));
            Assert.That(Tools.RomanToInt("XVI"), Is.EqualTo("16"));
            Assert.That(Tools.RomanToInt("XXVI"), Is.EqualTo("26"));
            Assert.That(Tools.RomanToInt("LXXVI"), Is.EqualTo("76"));
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
            Assert.That(ToTalk("Foo"), Is.EqualTo("Talk:Foo"));
            Assert.That(Tools.ConvertToTalk("Foo"), Is.EqualTo("Talk:Foo"));
            Assert.That(ToTalk("Foo bar"), Is.EqualTo("Talk:Foo bar"));
            Assert.That(ToTalk("Foo:Bar"), Is.EqualTo("Talk:Foo:Bar"));
            Assert.That(ToTalk("Wikipedia:Foo"), Is.EqualTo("Wikipedia talk:Foo"));
            Assert.That(ToTalk("File:Foo bar"), Is.EqualTo("File talk:Foo bar"));
            Assert.That(ToTalk("File talk:Foo bar"), Is.EqualTo("File talk:Foo bar"));

            // Don't choke on special namespaces
            Assert.That(ToTalk("Special:Foo"), Is.EqualTo("Special:Foo"));
            Assert.That(ToTalk("Media:Bar"), Is.EqualTo("Media:Bar"));

            // current namespace detection sucks, must be tested elsewhere
            // Assert.AreEqual("Wikipedia talk:Foo", ToTalk("Project:Foo"));
            // Assert.AreEqual("Image talk:Foo bar", ToTalk("Image:Foo bar"));
            // Assert.AreEqual("Image talk:Foo bar", ToTalk("Image talk:Foo bar"));
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
            Assert.That(new[] { "Talk:Foo", "Talk:Foo bar", "File talk:Foo", "Special:Foo" },
                                           Is.EquivalentTo(Tools.ConvertToTalk(l)));
        }

        [Test]
        public void ConvertFromTalk()
        {
            Assert.That(FromTalk("Talk:Foo"), Is.EqualTo("Foo"));
            Assert.That(Tools.ConvertFromTalk("Talk:Foo"), Is.EqualTo("Foo"));
            Assert.That(FromTalk("Foo"), Is.EqualTo("Foo"));
            Assert.That(FromTalk("Foo:Bar"), Is.EqualTo("Foo:Bar"));
            Assert.That(FromTalk("Talk:Foo:Bar"), Is.EqualTo("Foo:Bar"));
            Assert.That(FromTalk("User:Foo bar"), Is.EqualTo("User:Foo bar"));
            Assert.That(FromTalk("File talk:Bar"), Is.EqualTo("File:Bar"));
            // Assert.AreEqual("File:Bar", FromTalk("Image talk:Bar"),"it bypasses redirects to file namespace");
            Assert.That(FromTalk("Template talk:Bar"), Is.EqualTo("Template:Bar"));
            Assert.That(FromTalk("Category talk:Foo bar"), Is.EqualTo("Category:Foo bar"));

            // Don't choke on special namespaces
            Assert.That(FromTalk("Special:Foo"), Is.EqualTo("Special:Foo"));
            Assert.That(FromTalk("Media:Bar"), Is.EqualTo("Media:Bar"));
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
            Assert.That(new[] { "Foo", "Foo bar", "User:Foo", "Special:Foo" },
                                           Is.EquivalentTo(Tools.ConvertFromTalk(l)));
        }
        #endregion
    }
}
