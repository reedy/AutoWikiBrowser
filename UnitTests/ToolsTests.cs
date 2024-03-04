﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
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

            // Complex titles
            Assert.IsFalse(Tools.IsValidTitle("[test]#1"));
            Assert.IsFalse(Tools.IsValidTitle("_ _"), "Titles should be normalised before checking");
            Assert.IsTrue(Tools.IsValidTitle("http://www.wikipedia.org")); // unfortunately
            Assert.IsTrue(Tools.IsValidTitle("index.php/Viagra")); // even more unfortunately
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
            Assert.IsTrue(Tools.IsRomanNumber("LI"));
            Assert.IsTrue(Tools.IsRomanNumber("LXXXIII"));
            Assert.IsTrue(Tools.IsRomanNumber("CCLXXXIII"));

            Assert.IsFalse(Tools.IsRomanNumber("xvii"));
            Assert.IsFalse(Tools.IsRomanNumber("V II"));
            Assert.IsFalse(Tools.IsRomanNumber("AAA"));
            Assert.IsFalse(Tools.IsRomanNumber("123"));
            Assert.IsFalse(Tools.IsRomanNumber(" "));
            Assert.IsFalse(Tools.IsRomanNumber(""));
        }

        [Test]
        public void FirstLetterCaseInsensitive()
        {
            // standard cases
            Assert.AreEqual(@"[Aa]bc", Tools.FirstLetterCaseInsensitive("Abc"));
            Assert.AreEqual(@"[Aa]bc", Tools.FirstLetterCaseInsensitive("abc"));
            Assert.AreEqual(@"[Aa]BC", Tools.FirstLetterCaseInsensitive("aBC"));
            Assert.AreEqual(@"[Aa]bc[de]", Tools.FirstLetterCaseInsensitive("abc[de]"));
            Assert.AreEqual(@"[Σσ]bc", Tools.FirstLetterCaseInsensitive("Σbc"));

            // trimming
            Assert.AreEqual(@"[Aa]bc", Tools.FirstLetterCaseInsensitive("abc "));

            // no changes
            Assert.AreEqual(@" abc", Tools.FirstLetterCaseInsensitive(" abc"));
            Assert.AreEqual("", Tools.FirstLetterCaseInsensitive(""));
            Assert.AreEqual("123", Tools.FirstLetterCaseInsensitive("123"));
            Assert.AreEqual("-", Tools.FirstLetterCaseInsensitive("-"));
            Assert.AreEqual(@"[Aa]bc", Tools.FirstLetterCaseInsensitive(@"[Aa]bc"));

            Regex r = new Regex(Tools.FirstLetterCaseInsensitive("test"));
            Assert.IsTrue(r.IsMatch("test 123"));
            Assert.AreEqual("Test", r.Match("Test").Value);
            Assert.IsFalse(r.IsMatch("tEst"));

            r = new Regex(Tools.FirstLetterCaseInsensitive("Test"));
            Assert.IsTrue(r.IsMatch("test 123"));
            Assert.AreEqual("Test", r.Match("Test").Value);
            Assert.IsFalse(r.IsMatch("TEst"));

            r = new Regex(Tools.FirstLetterCaseInsensitive("#test#"));
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
        public void FirstToUpperAndRemoveHashOnArray()
        {
            string[] a = new string[2];
            a[0] = "Abc#f";
            a[1] = "efh ";

            string[] b = new string[2];
            b[0] = "Abc";
            b[1] = "Efh";

            Assert.AreEqual(b, Tools.FirstToUpperAndRemoveHashOnArray(a));
            a[1] = "[[Efh]]";
            Assert.AreEqual(b, Tools.FirstToUpperAndRemoveHashOnArray(a));
            Assert.AreEqual(null, Tools.FirstToUpperAndRemoveHashOnArray(null));
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
            Assert.AreEqual(2, Tools.WordCount("foo.bar", 10));
            Assert.AreEqual(1, Tools.WordCount("foo.bar", 1));

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

            Assert.AreEqual(2, Tools.WordCount(@"foo
{|
! test !! {{test}}
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
            if (!Globals.UsingMono)
                Assert.AreEqual("\r\nfoo\r\nbar\r\n", Tools.ConvertToLocalLineEndings("\nfoo\nbar\n"));

            Assert.AreEqual("", Tools.ConvertFromLocalLineEndings(""));
            Assert.AreEqual("foo bar", Tools.ConvertFromLocalLineEndings("foo bar"));
            if (!Globals.UsingMono)
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

            string y = @"Șen";
            Assert.AreEqual("z", ReplaceOnceString(y, y, "z"), "Handle combining diacritic letter");
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
            string[] sections = Tools.SplitToSections("foo\r\n==bar=\r\nboo1\r\n\r\n= boz =\r\n==quux==");
            CollectionAssert.AreEqual(new[]
                                      {
                                          "foo\r\n",
                                          "==bar=\r\nboo1\r\n\r\n",
                                          "= boz =\r\n",
                                          "==quux==\r\n"
                                      }, sections);

            sections = Tools.SplitToSections("==bar=\r\nboo2\r\n\r\n= boz =\r\n==quux==");
            CollectionAssert.AreEqual(new[]
                                      {
                                          "==bar=\r\nboo2\r\n\r\n",
                                          "= boz =\r\n",
                                          "==quux==\r\n"
                                      }, sections);

            sections = Tools.SplitToSections("\r\n==bar=\r\nboo3\r\n\r\n= boz =\r\n==quux==");
            CollectionAssert.AreEqual(new[]
                                      {
                                          "\r\n",
                                          "==bar=\r\nboo3\r\n\r\n",
                                          "= boz =\r\n",
                                          "==quux==\r\n"
                                      }, sections);

            sections = Tools.SplitToSections("\r\n==bar=\r\nboo4\r\n\r\n=== boz ===\r\n==quux==");
            CollectionAssert.AreEqual(new[]
                                      {
                                          "\r\n",
                                          "==bar=\r\nboo4\r\n\r\n",
                                          "=== boz ===\r\n",
                                          "==quux==\r\n"
                                      }, sections);

            sections = Tools.SplitToSections("");
            CollectionAssert.AreEqual(new[] { "\r\n" }, sections);

            sections = Tools.SplitToSections("==foo==");
            CollectionAssert.AreEqual(new[] { "==foo==\r\n" }, sections);
        }

        [Test]
        public void GetZerothSection()
        {
            Assert.AreEqual("Hello" + "\r\n", Tools.GetZerothSection("Hello" + "\r\n" + "==Heading=="));
            Assert.AreEqual("Hello" + "\r\n", Tools.GetZerothSection("Hello" + "\r\n" + "===Heading==="));
            Assert.AreEqual("Hello" + "\r\n", Tools.GetZerothSection("Hello" + "\r\n"));
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

            matches = Regex.Matches("world abc then abc after that then", "abc");
            Assert.AreEqual("world  then  after that then", Tools.RemoveMatches("world abc then abc after that then", matches));
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
        public void DeduplicateList()
        {
            List<string> A = new List<string>();
            Assert.AreEqual(A, Tools.DeduplicateList(A));
            A.Add("hello");
            Assert.AreEqual(A, Tools.DeduplicateList(A));
            A.Add("hello2");
            Assert.AreEqual(A, Tools.DeduplicateList(A));
            A.Add("hello");
            Assert.AreEqual(2, Tools.DeduplicateList(A).Count);
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
            Assert.IsTrue(Tools.IsRedirect("#REDIRECT  [[Foo|bar]]"));
            Assert.IsTrue(Tools.IsRedirect("#redirecT[[:Foo]]"));
            Assert.IsTrue(Tools.IsRedirect("should work!\r\n#REDIRECT [[Foo]]"));

            Assert.IsFalse(Tools.IsRedirect("#REDIRECT you to [[Hell]]"));
            Assert.IsFalse(Tools.IsRedirect("REDIRECT [[Foo]]"));

            // https://en.wikipedia.org/w/index.php?title=Middleton_Lake&diff=246079011&oldid=240299146
            Assert.IsTrue(Tools.IsRedirect("#REDIRECT:[[Foo]]"));
            Assert.IsTrue(Tools.IsRedirect("#REDIRECT : [[Foo]]"));

            Assert.IsFalse(Tools.IsRedirect("<nowiki>#REDIRECT  [[Foo]]</nowiki>"));
        }

        [Test]
        public void IsRedirectOrSoftRedirect()
        {
            Assert.IsTrue(Tools.IsRedirectOrSoftRedirect("#REDIRECT  [[Foo]]"));
            Assert.IsTrue(Tools.IsRedirectOrSoftRedirect("#redirecT[[:Foo]]"));
            Assert.IsTrue(Tools.IsRedirectOrSoftRedirect("should work!\r\n#REDIRECT [[Foo]]"));
            Assert.IsTrue(Tools.IsRedirectOrSoftRedirect("{{soft redirect|Foo}}"));
            Assert.IsTrue(Tools.IsRedirectOrSoftRedirect("{{soft|Foo}}"));
            Assert.IsTrue(Tools.IsRedirectOrSoftRedirect("{{category redirect|Foo}}"));
            Assert.IsTrue(Tools.IsRedirectOrSoftRedirect("{{Interwiki redirect|Foo}}"));
            Assert.IsTrue(Tools.IsRedirectOrSoftRedirect("{{Userrename|Foo}}"));
            Assert.IsTrue(Tools.IsRedirectOrSoftRedirect("{{Commons category redirect|Foo}}"));
            Assert.IsTrue(Tools.IsRedirectOrSoftRedirect("{{Deprecated shortcut|Foo}}"));
            Assert.IsTrue(Tools.IsRedirectOrSoftRedirect("{{Wikisource redirect|Foo}}"));
            Assert.IsTrue(Tools.IsRedirectOrSoftRedirect("{{Double soft redirect|Foo}}"));

            Assert.IsFalse(Tools.IsRedirectOrSoftRedirect("{{software|Foo}}"));
        }

        [Test]
        public void RedirectTarget()
        {
            Assert.AreEqual("Foo", Tools.RedirectTarget("#redirect [[Foo]]"));
            Assert.AreEqual("Foo", Tools.RedirectTarget("#redirect [[Foo]] {{R from something}}"));
            Assert.AreEqual("Foo", Tools.RedirectTarget("#REDIRECT[[Foo]]"));
            Assert.AreEqual("Foo bar", Tools.RedirectTarget("#redirect[[:Foo bar ]]"));
            Assert.AreEqual("Foo bar", Tools.RedirectTarget("#redirect[[ :  Foo bar ]]"));
            Assert.AreEqual("Foo", Tools.RedirectTarget("{{delete}}\r\n#redirect [[Foo]]"));

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_6#AWB_follows_piped_redirects_to_an_invalid_page_title
            Assert.AreEqual("Foo", Tools.RedirectTarget("#REDIRECT [[Foo|bar]]"));

            // URL-decode targets
            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_7#Problem_with_redirects
            Assert.AreEqual("Foo, bar", Tools.RedirectTarget("#REDIRECT[[Foo%2C_bar]]"));
            Assert.AreEqual("Хуй", Tools.RedirectTarget("#REDIRECT[[%D0%A5%D1%83%D0%B9]]"));

            // https://en.wikipedia.org/w/index.php?title=Middleton_Lake&diff=246079011&oldid=240299146
            Assert.AreEqual("Foo", Tools.RedirectTarget("#REDIRECT:[[Foo]]"));
            Assert.AreEqual("Foo", Tools.RedirectTarget("#REDIRECT : [[Foo]]"));

            Assert.AreEqual("Foo#bar", Tools.RedirectTarget("#REDIRECT [[Foo#bar]]"), "redirect to section");
            Assert.AreEqual("Foo#bar", Tools.RedirectTarget("#REDIRECT [[Foo#bar|other]]"), "redirect to section, piped link");

            Assert.AreEqual("", Tools.RedirectTarget("<nowiki>#REDIRECT  [[Foo]]</nowiki>"));

            Assert.AreEqual("", Tools.RedirectTarget(@"{{Refimprove|date=October 2008}}
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

==Prelude=="));
        }

        [Test]
        public void GetTitleFromURL()
        {
            Assert.AreEqual("foo bar", Tools.GetTitleFromURL("https://en.wikipedia.org/wiki/foo_bar"));
            Assert.AreEqual("Хуй", Tools.GetTitleFromURL("https://en.wikipedia.org/wiki/%D0%A5%D1%83%D0%B9"));
            Assert.AreEqual("foo", Tools.GetTitleFromURL("https://en.wikipedia.org/w/index.php?title=foo"));
            Assert.AreEqual("foo", Tools.GetTitleFromURL("https://en.wikipedia.org/w/index.php/foo"));
            Assert.AreEqual("foo", Tools.GetTitleFromURL("http://en.wikipedia.org/w/index.php/foo"));

            // return null if there is something wrong
            Assert.IsNull(Tools.GetTitleFromURL(""));
            Assert.IsNull(Tools.GetTitleFromURL("foo"));
            Assert.IsNull(Tools.GetTitleFromURL("https://en.wikipedia.org"));
            Assert.IsNull(Tools.GetTitleFromURL("https://en.wikipedia.org/wiki/"));
            Assert.IsNull(Tools.GetTitleFromURL("https://en.wikipedia.org/w/index.php?title=foo&action=delete"));

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_7#list_entries_like:_Index.html.3Fcurid.3D16235168
            Assert.IsNull(Tools.GetTitleFromURL("https://en.wikipedia.org/wiki/index.html?curid=666"));
            Assert.IsNull(Tools.GetTitleFromURL("https://en.wikipedia.org/wiki/foo?action=delete"));
            Assert.IsNull(Tools.GetTitleFromURL("https://en.wikipedia.org/w/index.php?title=foo&action=delete"));
            Assert.IsNull(Tools.GetTitleFromURL("https://en.wikipedia.org/w/index.php/foo?action=bar"));
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
            // Test majority of Key Words except %%key%%
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

            // Date Stuff - disabled for now
            // Assert.AreEqual(DateTime.Now.Day.ToString() + "\r\n" +DateTime.Now.ToString("MMM") + "\r\n" +DateTime.Now.Year.ToString(), Tools.ApplyKeyWords("n/a", @"{{CURRENTDAY}}
            // {{CURRENTMONTHNAME}}
            // {{CURRENTYEAR}}"));

            // Server Stuff
            Assert.AreEqual(@"https://en.wikipedia.org
/w
en.wikipedia.org", Tools.ApplyKeyWords("n/a", @"%%server%%
%%scriptpath%%
%%servername%%"));

            // %%key%%, Tools.MakeHumanCatKey() - Covered by HumanCatKeyTests

            Assert.AreEqual("", Tools.ApplyKeyWords("", ""));
            Assert.AreEqual("", Tools.ApplyKeyWords(@"%%foo%%", ""));
            Assert.AreEqual(@"%%foo%%", Tools.ApplyKeyWords("", @"%%foo%%"));

            Assert.AreEqual(@"foo\(bar\) was", Tools.ApplyKeyWords(@"foo(bar)", "%%title%% was", true), "%%title%% escaped if requested");
            Assert.AreEqual(@"foo\(bar\) was", Tools.ApplyKeyWords(@"foo(bar)", "%%pagename%% was", true), "%%pagename%% escaped if requested");
            Assert.AreEqual(@"foo\(bar\) was", Tools.ApplyKeyWords(@"foo(bar)", "%%basepagename%% was", true), "%%basepagename%% escaped if requested");
            Assert.AreEqual(@"foo(bar) was", Tools.ApplyKeyWords(@"foo(bar)", "%%title%% was", false), "%%title%% not escaped if not requested");
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
            Assert.IsTrue(Tools.IsWikimediaProject(ProjectEnum.wikivoyage));
            Assert.IsTrue(Tools.IsWikimediaProject(ProjectEnum.wiktionary));
            Assert.IsTrue(Tools.IsWikimediaProject(ProjectEnum.mediawiki));

            Assert.IsFalse(Tools.IsWikimediaProject(ProjectEnum.custom));
            Assert.IsFalse(Tools.IsWikimediaProject(ProjectEnum.wikia));
            
        }

        [Test]
        public void OrdeOfWikimediaProjects()
        {
            // Be very causious if changing order or Wikimedia Projects
            Assert.IsTrue(ProjectEnum.commons > ProjectEnum.species);
            Assert.IsTrue(ProjectEnum.meta > ProjectEnum.commons);
            Assert.IsTrue(ProjectEnum.mediawiki > ProjectEnum.meta);
            Assert.IsTrue(ProjectEnum.incubator > ProjectEnum.mediawiki);
            Assert.IsTrue(ProjectEnum.wikia > ProjectEnum.incubator);
            Assert.IsTrue(ProjectEnum.custom > ProjectEnum.wikia);
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
            Assert.AreEqual("   bar   ", Tools.ReplaceWithSpaces("foobarfoo", foo, 1));

            foo = new Regex("f(o)o");
            Assert.AreEqual(" o ", Tools.ReplaceWithSpaces("foo", foo, 1));
            Assert.AreEqual(" o bar o ", Tools.ReplaceWithSpaces("foobarfoo", foo, 1));
        }

        [Test]
        public void ReplaceWithTests()
        {
            Regex foo = new Regex("foo");
            Assert.AreEqual("", Tools.ReplaceWith("", foo, '#'));
            Assert.AreEqual("###", Tools.ReplaceWith("foo", foo, '#'));
            Assert.AreEqual("###bar###", Tools.ReplaceWith("foobarfoo", foo, '#'));

            foo = new Regex("f(o)o");
            Assert.AreEqual("", Tools.ReplaceWith("", foo, '#'));
            Assert.AreEqual("###", Tools.ReplaceWith("foo", foo, '#'));
            Assert.AreEqual("###bar###", Tools.ReplaceWith("foobarfoo", foo, '#'));
            Assert.AreEqual("", Tools.ReplaceWith("", foo, '#', 1));
            Assert.AreEqual("#o#", Tools.ReplaceWith("foo", foo, '#', 1));
            Assert.AreEqual("#o#bar#o#", Tools.ReplaceWith("foobarfoo", foo, '#', 1));
        }

        [Test]
        public void HTMLListToWiki()
        {
            Assert.AreEqual(@"* Fred", Tools.HTMLListToWiki("Fred", "*"), "simple case");
            Assert.AreEqual(@"* Fred" + "\r\n", Tools.HTMLListToWiki("Fred" + "\r\n", "*"), "simple case, trailing newline");
            Assert.AreEqual(@"* Fred
* Jones", Tools.HTMLListToWiki(@"Fred
Jones", "*"), "Two entries");
            Assert.AreEqual(@"* Fred", Tools.HTMLListToWiki("Fred<BR>", "*"), "br handling");
            Assert.AreEqual(@"* Fred", Tools.HTMLListToWiki("Fred<br/>", "*"), "br handling");
            Assert.AreEqual(@"# Fred", Tools.HTMLListToWiki("Fred", "#"), "simple case #");
            Assert.AreEqual(@"# Fred", Tools.HTMLListToWiki("<OL>Fred</OL>", "#"), "ol handling");
            Assert.AreEqual(@"# Fred", Tools.HTMLListToWiki("<li>Fred</li>", "#"), "li handling");
            Assert.AreEqual(@"* Fred", Tools.HTMLListToWiki(":Fred", "*"), "trim colon");
            Assert.AreEqual(@"* Fred", Tools.HTMLListToWiki("*Fred", "*"), "already a list");
            Assert.AreEqual(@"* Fred Smith [[Foo#bar]]", Tools.HTMLListToWiki("#Fred Smith [[Foo#bar]]", "*"), "number to bullet conversion");

            Assert.AreEqual(@"* Fred Smith:here", Tools.HTMLListToWiki("Fred Smith:here", "*"), "normal colon retained");

            Assert.AreEqual(@"* Fred
* Jones", Tools.HTMLListToWiki(@"1 Fred
2 Jones", "*"), "number to bullet conversion 2 entries");

            Assert.AreEqual(@"* Fred
* Jones", Tools.HTMLListToWiki(@"11. Fred
12. Jones", "*"), "dot number conversion");

            Assert.AreEqual(@"* Fred
* Jones", Tools.HTMLListToWiki(@"(1) Fred
(2) Jones", "*"), "full bracket number conversion 1 digit");

            Assert.AreEqual(@"* Fred
* Jones", Tools.HTMLListToWiki(@"1) Fred
2) Jones", "*"), "bracket number conversion 1 digit");

            Assert.AreEqual(@"* Fred
* Jones", Tools.HTMLListToWiki(@"(11) Fred
(12) Jones", "*"), "bracket number conversion 2 digits");

            Assert.AreEqual(@"* Fred
* Jones", Tools.HTMLListToWiki(@"(998) Fred
(999) Jones", "*"), "bracket number conversion 3 digits");

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#Text_deleted_by_.22convert_list_to.22_.22.2A_list.22
            Assert.AreEqual(@"* 1980 Fred
* 2004 Jones", Tools.HTMLListToWiki(@"1980 Fred
2004 Jones", "*"));

            Assert.AreEqual(@"* Fred
* Tim
* John", Tools.HTMLListToWiki(@"Fred
Tim
John", "*"), "do not add list to blank lines/lines with just whitespace");

            Assert.AreEqual(@"* Fred
 
* Tim
 
* John", Tools.HTMLListToWiki(@"Fred
 
Tim
 
John", "*"), "do not add list to blank lines/lines with just whitespace 2");
        }

        [Test]
        public void RemoveSyntax()
        {
            Assert.AreEqual(@"", Tools.RemoveSyntax(@""));
            Assert.AreEqual(@"", Tools.RemoveSyntax(@" "));
            Assert.AreEqual(@"foo", Tools.RemoveSyntax(@"* foo"));
            Assert.AreEqual(@"foo", Tools.RemoveSyntax(@"# foo"));
            Assert.AreEqual(@"foo", Tools.RemoveSyntax(@":foo"));
            Assert.AreEqual(@"foo bar", Tools.RemoveSyntax(@"foo_bar"));
            Assert.AreEqual(@"foo bar:the great", Tools.RemoveSyntax(@"foo_bar:the great"));
            Assert.AreEqual(@"foo", Tools.RemoveSyntax(@"[foo]"));
            Assert.AreEqual(@"foo", Tools.RemoveSyntax(@"[[foo]]"));
            Assert.AreEqual(@"foo", Tools.RemoveSyntax(@"#[[foo]]"));
            Assert.AreEqual(@"foo", Tools.RemoveSyntax(@"#[[ foo ]]"));
            Assert.AreEqual(@"foo&bar", Tools.RemoveSyntax(@"foo&amp;bar"));
            Assert.AreEqual(@"http://site.com words", Tools.RemoveSyntax(@"* [http://site.com words]"));
            Assert.AreEqual(@"https://site.com words", Tools.RemoveSyntax(@"* [https://site.com words]"));
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
            Assert.AreEqual(0, Tools.LinkCount(@"now [[Template:foo]] was great"));
            Assert.AreEqual(0, Tools.LinkCount(@"now [[Category:foo]] was great"));

            Assert.AreEqual(1, Tools.InterwikiCount(@"now [[de:foo]] was great"));
            Assert.AreEqual(1, Tools.InterwikiCount(@"now [[de:foo]] was great [[aa:now]] here"));

            Assert.AreEqual(2, Tools.InterwikiCount(@"now [[de:foo]] was great [[aa:now]] here [[fr:bye]]"));
        }

        [Test]
        public void LinkCountTests()
        {
            Assert.AreEqual(0, Tools.LinkCount(@"foo"), "No links");
            Assert.AreEqual(0, Tools.LinkCount(@"[foo]"), "broken links not counted");
            Assert.AreEqual(0, Tools.LinkCount(@"[[Image:foo.png]]"), "Image links not counted");
            Assert.AreEqual(0, Tools.LinkCount(@"[[File:foo.png]]"), "File links not counted");
            Assert.AreEqual(0, Tools.LinkCount(@"[[Template:Dn]]"), "Templates not counted");
            Assert.AreEqual(1, Tools.LinkCount(@"now [[en:foo]] was great"), "Interwikis are counted");
            Assert.AreEqual(1, Tools.LinkCount(@"[[foo]]"));
            Assert.AreEqual(2, Tools.LinkCount(@"[[foo]] and [[foo]]"), "counts repeated links");
            Assert.AreEqual(1, Tools.LinkCount(@"[[Image:foo.png]] and [[foo]]"));
            Assert.AreEqual(2, Tools.LinkCount(@"[[foo]]s and [[barbie|bar]]"), "counts piped links");
            Assert.AreEqual(1, Tools.LinkCount(@"{{flagIOC}}"));
            Assert.AreEqual(1, Tools.LinkCount(@"{{speciesbox}}"));
            Assert.AreEqual(1, Tools.LinkCount(@"{{automatic taxobox}}"));
            Assert.AreEqual(1, Tools.LinkCount(@"now [[Magic: the gathering]] was great"), "handles mainspace wikilink with colon");
            Assert.AreEqual(3, Tools.LinkCount(@"[[foo]]s and [[barbie|bar]] {{flagIOC}}"), "counts flagIOC template as a link");
            
            Assert.AreEqual(1, Tools.LinkCount(@"[[foo]]s and [[barbie|bar]] and [[foo2]]", 1), "count capped at limit");
            Assert.AreEqual(2, Tools.LinkCount(@"[[foo]]s and [[barbie|bar]] and [[foo2]]", 2), "count capped at limit");
            Assert.AreEqual(2, Tools.LinkCount(@"{{flagIOC}} {{flagIOC}} {{flagIOC}}", 2), "count capped at limit, flagIOC");

            Assert.AreEqual(1, Tools.LinkCount(@"[[Foo|ʿUrwa]]"), "Handling of Unicode modifier letters");
        }

        [Test]
        public void DatesCount()
        {
            Dictionary<Parsers.DateLocale, int> results = new Dictionary<Parsers.DateLocale, int>();

            results.Add(Parsers.DateLocale.ISO, 1);
            results.Add(Parsers.DateLocale.International, 1);
            results.Add(Parsers.DateLocale.American, 0);

            Assert.AreEqual(results, Tools.DatesCount("2015-01-01 and 11 January 2010"), "zero dates of a type reported");

            results.Clear();
            results.Add(Parsers.DateLocale.ISO, 1);
            results.Add(Parsers.DateLocale.International, 1);
            results.Add(Parsers.DateLocale.American, 1);

            Assert.AreEqual(results, Tools.DatesCount("2015-01-01 and 11 January 2010 and March 5, 2011"), "each date type reported");
            Assert.AreEqual(results, Tools.DatesCount("2015-01-01 and| 11 January 2010 and| March 5, 2011"), "each date type reported when split");
        
            results.Clear();
            results.Add(Parsers.DateLocale.ISO, 2);
            results.Add(Parsers.DateLocale.International, 1);
            results.Add(Parsers.DateLocale.American, 1);

            Assert.AreEqual(results, Tools.DatesCount("2015-01-01 and 2015-01-01 and 11 January and March 5"), "Duplicate dates counted, short format matched");
        }

        [Test]
        public void DuplicateWikiLinks()
        {
            List<string> dupeWikiLinks = new List<string>();

            dupeWikiLinks.Add("Foo (2)");

            Assert.AreEqual(dupeWikiLinks, Tools.DuplicateWikiLinks(@"[[Foo]] [[Foo]]"), "Simple case two plain links same case");
            Assert.AreEqual(dupeWikiLinks, Tools.DuplicateWikiLinks(@"[[foo]] [[foo]]"), "Simple case two plain links same lower case");
            Assert.AreEqual(dupeWikiLinks, Tools.DuplicateWikiLinks(@"[[Foo]] [[Foo]] [[Bar]]"), "Don't count single link");
            Assert.AreEqual(dupeWikiLinks, Tools.DuplicateWikiLinks(@"[[Foo|bar2]] [[Foo|bar]]"), "Simple case two piped links same case");
            Assert.AreEqual(dupeWikiLinks, Tools.DuplicateWikiLinks(@"[[Foo]] [[foo]]"), "convert first letter case for compare");
            Assert.AreEqual(dupeWikiLinks, Tools.DuplicateWikiLinks(@"[[ Foo |bar]] [[ Foo ]]"), "Ignore excess whitespace");
            Assert.AreEqual(dupeWikiLinks, Tools.DuplicateWikiLinks(@"[[Foo]] [[Foo]] [[Bar]] [[Bar2]]"), "Match on whole link name");
            Assert.AreEqual(dupeWikiLinks, Tools.DuplicateWikiLinks(@"[[Foo]] [[Foo]] [[|x]] [[|x]]"), "Ignore targetless links");
            Assert.AreEqual(dupeWikiLinks, Tools.DuplicateWikiLinks(@"[[Foo]] [[Foo]] [[1 May]] [[1 May]]"), "Ignore dates (int format)");
            Assert.AreEqual(dupeWikiLinks, Tools.DuplicateWikiLinks(@"[[Foo]] [[Foo]] [[May 1]] [[May 1]]"), "Ignore dates (Am format)");

            dupeWikiLinks.Clear();
            dupeWikiLinks.Add("Foo (3)");

            Assert.AreEqual(dupeWikiLinks, Tools.DuplicateWikiLinks(@"[[Foo]] [[Foo]] [[Foo|bar]]"), "Includes count after link name");
            Assert.AreEqual(dupeWikiLinks, Tools.DuplicateWikiLinks(@"[[Foo]] [[Foo]] [[Foo|bar]] <!-- [[Foo]] -->"), "Ingore commented out link");

            dupeWikiLinks.Add("Get (3)");

            Assert.AreEqual(dupeWikiLinks, Tools.DuplicateWikiLinks(@"[[Foo]] [[Get]] [[Foo]] [[Get]] [[Foo|bar]] [[Get]]"), "List returned is sorted");

            dupeWikiLinks.Clear();
            dupeWikiLinks.Add("Foo (2)");
            Assert.AreEqual(dupeWikiLinks, Tools.DuplicateWikiLinks(@"[[Foo|ʿUrwa]] [[Foo|ʿUrwa]]"), "Handling of Unicode modifier letters");
        }

        [Test]
        public void ConvertDate()
        {
            string iso = @"2009-06-11", iso2 = @"1890-07-04";
            Assert.AreEqual(@"11 June 2009", Tools.ConvertDate(iso, Parsers.DateLocale.International));
            Assert.AreEqual(@"11 June 2009", Tools.ConvertDate(iso, Parsers.DateLocale.International, false));
            Assert.AreEqual(@"11 June 2009", Tools.ConvertDate(iso, Parsers.DateLocale.International, true));

            Assert.AreEqual(@"June 11, 2009", Tools.ConvertDate(iso, Parsers.DateLocale.American));
            Assert.AreEqual(iso, Tools.ConvertDate(iso, Parsers.DateLocale.ISO));
            Assert.AreEqual(iso, Tools.ConvertDate(iso, Parsers.DateLocale.Undetermined));
            Assert.AreEqual(@"4 July 1890", Tools.ConvertDate(iso2, Parsers.DateLocale.International));

            // handles incorect format
            string wrong = @"foo";
            Assert.AreEqual(wrong, Tools.ConvertDate(wrong, Parsers.DateLocale.International));
            Assert.AreEqual(@"2009-10", Tools.ConvertDate(@"2009-10", Parsers.DateLocale.International), "day not added to year month combo");

            // supports other valid date formats
            Assert.AreEqual(@"11 June 2009", Tools.ConvertDate("11 June 2009", Parsers.DateLocale.International));
            Assert.AreEqual(@"11 June 2009", Tools.ConvertDate("June 11, 2009", Parsers.DateLocale.International));
            Assert.AreEqual(@"11 June 2009", Tools.ConvertDate("June 11, 2009", Parsers.DateLocale.International, true));

            Assert.AreEqual(@"15 January 2008", Tools.ConvertDate("Jan 15 2008", Parsers.DateLocale.International));
            Assert.AreEqual(@"15 January 2008", Tools.ConvertDate("Jan 15 2008", Parsers.DateLocale.International, true));
            Assert.AreEqual(@"15 January 2008", Tools.ConvertDate("Jan 15 2008", Parsers.DateLocale.International, false));
            Assert.AreEqual(@"15 January 998", Tools.ConvertDate("Jan 15 998", Parsers.DateLocale.International));

            const string AmericanDate = @"06/11/2009", UKDate = @"11/06/2009";

            Assert.AreEqual(iso, Tools.ConvertDate(AmericanDate, Parsers.DateLocale.ISO, true), "Converts MM/DD/YYYY format dates when flagged");
            Assert.AreEqual(iso, Tools.ConvertDate(UKDate, Parsers.DateLocale.ISO, false), "Converts DD/MM/YYYY format dates when flagged");
            Assert.AreEqual(iso, Tools.ConvertDate(UKDate, Parsers.DateLocale.ISO), "Assumes DD/MM/YYYY format dates when NOT flagged");

            Assert.AreEqual(@"May 2009", Tools.ConvertDate(@"May 2009", Parsers.DateLocale.International), "day not added to month year combo");
            Assert.AreEqual(@"May 2009", Tools.ConvertDate(@" May 2009", Parsers.DateLocale.International), "day not added to month year combo");

            Assert.AreEqual(@"15 January 2008", Tools.ConvertDate("2008-Jan-15", Parsers.DateLocale.International));
            Assert.AreEqual(@"15 January 2008", Tools.ConvertDate("2008 Jan 15", Parsers.DateLocale.International));
            Assert.AreEqual(@"15 January 2008", Tools.ConvertDate("2008 Jan. 15", Parsers.DateLocale.International));
            Assert.AreEqual(@"15 January 2008", Tools.ConvertDate("2008 January 15", Parsers.DateLocale.International));
        }

        [Test]
        public void DateBeforeToday()
        {
            Assert.IsTrue(Tools.DateBeforeToday("11 May 2009"));
            Assert.IsTrue(Tools.DateBeforeToday("May 11, 2009"));
            Assert.IsTrue(Tools.DateBeforeToday("2013-12-31"));
            Assert.IsTrue(Tools.DateBeforeToday(DateTime.Now.AddDays(-1).ToString(CultureInfo.CurrentCulture)));

            Assert.IsFalse(Tools.DateBeforeToday(DateTime.Now.AddMonths(1).ToString(CultureInfo.CurrentCulture)));
            Assert.IsFalse(Tools.DateBeforeToday("foo"));
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
            Assert.AreEqual(@"{{cite|title=abc | location=London }}", Tools.AppendParameterToTemplate(@"{{cite|title=abc }}", "location", "London"));
            Assert.AreEqual(@"{{cite|title=abc|last=a|first=b|date=2009-12-12|location=London}}", Tools.AppendParameterToTemplate(@"{{cite|title=abc|last=a|first=b|date=2009-12-12}}", "location", "London"), "no newlines/excess spaces in template");
            Assert.AreEqual(@"{{cite | title=abc | last=a | first=b | date=2009-12-12 | location=London }}", Tools.AppendParameterToTemplate(@"{{cite | title=abc | last=a | first=b | date=2009-12-12 }}", "location", "London"), "spaced parameters in template");
            Assert.AreEqual(@"{{cite | title = abc | last = a | first = b | date = 2009-11-11 | location = London }}", Tools.AppendParameterToTemplate(@"{{cite | title = abc | last = a | first = b | date = 2009-11-11 }}", "location", "London"), "=spaced parameters in template");
            Assert.AreEqual(@"{{cite | title= abc | last= a | first= b | date= 2009-11-11 | location= London }}", Tools.AppendParameterToTemplate(@"{{cite | title= abc | last= a | first= b | date= 2009-11-11 }}", "location", "London"), "= half spaced parameters in template");

            Assert.AreEqual(@"{{cite|title=abc | location=London}}", Tools.AppendParameterToTemplate(@"{{cite|title=abc}}", "location", "London", false));
            Assert.AreEqual(@"{{cite|title=abc|location=London}}", Tools.AppendParameterToTemplate(@"{{cite|title=abc}}", "location", "London", true));

            Assert.AreEqual(@"{{cite
|title=abc | location=London }}", Tools.AppendParameterToTemplate(@"{{cite
|title=abc }}", "location", "London"), "insufficient newlines – space used");

            Assert.AreEqual(@"{{cite
|title=abc
|date=2009-12-12 | location=London }}", Tools.AppendParameterToTemplate(@"{{cite
|title=abc
|date=2009-12-12 }}", "location", "London"), "insufficient newlines – space used");

            Assert.AreEqual(@"{{cite
|title=abc
|last=a
|first=b
|date=2009-12-12
|location=London}}", Tools.AppendParameterToTemplate(@"{{cite
|title=abc
|last=a
|first=b
|date=2009-12-12}}", "location", "London"), "newline set when > 2 existing");

            Assert.AreEqual(@"{{cite
|title=abc
|last=a
|first=b
|date=2009-12-12
|location=London }}", Tools.AppendParameterToTemplate(@"{{cite
|title=abc
|last=a
|first=b
|date=2009-12-12        }}", "location", "London"), "existing template end spaces cleaned");

            Assert.AreEqual(@"{{cite
|title=abc
|last=a
|first=b
|date=2009-12-12
|location=London
}}", Tools.AppendParameterToTemplate(@"{{cite
|title=abc
|last=a
|first=b
|date=2009-12-12
}}", "location", "London"), "template end on blank line");

            Assert.AreEqual(@"{{cite
  |title=abc
  |last=a
  |first=b
  |date=2009-12-12
  |location=London
}}", Tools.AppendParameterToTemplate(@"{{cite
  |title=abc
  |last=a
  |first=b
  |date=2009-12-12
}}", "location", "London"), "template with multiple spaces prior to bar, all params on newline");

            Assert.AreEqual(@"{{cite
 |title=abc
 |last=a
 |first=b
 |date=2009-12-12
 |location=London
}}", Tools.AppendParameterToTemplate(@"{{cite
 |title=abc
 |last=a
 |first=b
 |date=2009-12-12
}}", "location", "London"), "template with single prior to bar, all params on newline");

            Assert.AreEqual(@"{{cite journal
 | quotes =
 | doi = 10.2307/904183
 | id =
 | url = http://links.jstor.org/sici?sici=0027-4666(19060101)47%3A755%3C27%3AMSIC(J%3E2.0.CO%3B2-N
 | publisher = The Musical Times, Vol. 47, No. 755
 | jstor = 904183
 }}", Tools.AppendParameterToTemplate(@"{{cite journal
 | quotes =
 | doi = 10.2307/904183
 | id =
 | url = http://links.jstor.org/sici?sici=0027-4666(19060101)47%3A755%3C27%3AMSIC(J%3E2.0.CO%3B2-N
 | publisher = The Musical Times, Vol. 47, No. 755
 }}", "jstor", "904183"));
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
            
            // parameters with space in name
            Assert.AreEqual("here", Tools.GetTemplateParameterValue(@"{{cite|param 1=here}}", "param 1"));
            Assert.AreEqual("here", Tools.GetTemplateParameterValue(@"{{cite|param other=here}}", "param other"));
        }
        
        
        [Test]
        public void GetTemplateParameterValues()
        {
            Dictionary<string, string> Back = new Dictionary<string, string>();
            Back.Add("accessdate", "2012-05-15");
            Back.Add("title", "Hello");
            Back.Add("url", "http://www.site.com/abc");
            Back.Add("author", "");
            
            Assert.AreEqual(Back, Tools.GetTemplateParameterValues(@"{{cite web|url=http://www.site.com/abc|title=Hello | author = | accessdate = 2012-05-15 }}"));
            Assert.AreEqual(Back, Tools.GetTemplateParameterValues(@"{{cite web|url=http://www.site.com/abc|title=Hello |<!-- comm--> author = | accessdate = 2012-05-15 }}"), "ignores comments between parameters");
            Assert.AreEqual(Back, Tools.GetTemplateParameterValues(@"{{cite web|url=http://www.site.com/abc|title=Hello | author <!-- comm--> = | accessdate = 2012-05-15 }}"), "ignores comments between parameters 2");
            Assert.AreEqual(Back, Tools.GetTemplateParameterValues(@"{{cite web|url=http://www.site.com/abc|title=Hello |author =|accessdate = 2012-05-15 }}"));
            Assert.AreEqual(Back, Tools.GetTemplateParameterValues(@"{{cite web|url=http://www.site.com/abc|title=Hello | author = | accessdate = 2012-05-15 | url=}}"), "ignores second parameter call (no value)");
            Assert.AreEqual(Back, Tools.GetTemplateParameterValues(@"{{cite web|url=http://www.site.com/abc|title=Hello | author = | accessdate = 2012-05-15 | url=http://site2.com}}"), "ignores second parameter call (with value)");

            Back.Remove("author");
            Back.Add("author", "<!-- comm-->");
            Assert.AreEqual(Back, Tools.GetTemplateParameterValues(@"{{cite web|url=http://www.site.com/abc|title=Hello | author = <!-- comm--> | accessdate = 2012-05-15 }}"), "Reports parameter value of comment");
            Assert.AreEqual(Back, Tools.GetTemplateParameterValues(@"{{cite web|url=http://www.site.com/abc|title=Hello | accessdate = 2012-05-15 | author = <!-- comm--> }}"), "Reports parameter value of comment: last param");

            Back.Remove("author");
            Back.Add("author", "");
            Back.Add("format", "{{PDF|test}}");
            Assert.AreEqual(Back, Tools.GetTemplateParameterValues(@"{{cite web|url=http://www.site.com/abc|title=Hello | author = | accessdate = 2012-05-15 | format={{PDF|test}} }}"), "handles nested templates");
            Back.Add("last1", "Jones");
            Assert.AreEqual(Back, Tools.GetTemplateParameterValues(@"{{cite web|url=http://www.site.com/abc|title=Hello | author = | accessdate = 2012-05-15 | format={{PDF|test}} |last1=Jones}}"), "handles parameters with numbers");
            Back.Add("trans_title", @"Here 
There");
            Assert.AreEqual(Back, Tools.GetTemplateParameterValues(@"{{cite web|url=http://www.site.com/abc|title=Hello | author = | accessdate = 2012-05-15 | format={{PDF|test}} |last1=Jones|trans_title=Here 
There}}"), "handles parameters with newlines");
            
            Back.Clear();
            Back.Add("ONE", "somevalue");
            Assert.AreEqual(Back, Tools.GetTemplateParameterValues(@"{{test|ONE=somevalue}}"), "handles uppercase parameters");
            Back.Add("V_TWO", "some_value");
            Assert.AreEqual(Back, Tools.GetTemplateParameterValues(@"{{test|ONE=somevalue|V_TWO=some_value}}"), "handles uppercase parameters");

            // parameters with space in name
            Back.Clear();
            Back.Add("ONE", "somevalue");
            Assert.AreEqual(Back, Tools.GetTemplateParameterValues(@"{{test|ONE=somevalue}}"), "handles uppercase parameters");
            Back.Add("V TWO", "some_value");
            Assert.AreEqual(Back, Tools.GetTemplateParameterValues(@"{{test|ONE=somevalue|V TWO=some_value}}"), "handles uppercase parameters");
            Assert.AreEqual(Back, Tools.GetTemplateParameterValues(@"{{test|ONE=somevalue|V TWO  =  some_value}}"), "handles uppercase parameters");

            Back.Clear();
            Back.Add("name", "<timeline>abc</timeline>X");
            Assert.AreEqual(Back, Tools.GetTemplateParameterValues(@"{{test|name = <timeline>abc</timeline>X}}"), "handles parameters including unformatted text");

            Back.Clear();
            Back.Add("name", "X [http://site.com A | B]");
            Back.Add("other", "Y");
            Assert.AreEqual(Back, Tools.GetTemplateParameterValues(@"{{test|name = X [http://site.com A | B]|other=Y}}"), "handles parameters including external link with pipe");
        }

        [Test]
        public void GetTemplateParameterValueAdvancedCases()
        {
            Assert.AreEqual(@"here {{foo}}", Tools.GetTemplateParameterValue(@"{{cite|param1 = here {{foo}}}}", "param1"));
            Assert.AreEqual(@"here {{foo|bar}}", Tools.GetTemplateParameterValue(@"{{cite|param1 = here {{foo|bar}}}}", "param1"));
            Assert.AreEqual(@"here <!--foo|bar-->", Tools.GetTemplateParameterValue(@"{{cite|param1 = here <!--foo|bar-->}}", "param1"));
            Assert.AreEqual(@"<!--foo-->", Tools.GetTemplateParameterValue(@"{{cite|param1 = <!--foo-->}}", "param1"), "Reports comment when param value just comment");
            Assert.AreEqual(@"<!--foo-->", Tools.GetTemplateParameterValue(@"{{cite|param1 = <!--foo--> | param2 = bar }}", "param1"), "Reports comment when param value just comment");
            Assert.AreEqual(@"[[File:abc.png|asdf|dsfjk|a]]", Tools.GetTemplateParameterValue(@"{{cite|param1 = [[File:abc.png|asdf|dsfjk|a]] }}", "param1"));
            Assert.AreEqual(@"[[File:abc.png|asdf|dsfjk|a]]", Tools.GetTemplateParameterValue(@"{{cite|param1 = [[File:abc.png|asdf|dsfjk|a]]
}}", "param1"));
            Assert.AreEqual(@"[[File:abc.png|asdf|dsfjk|a]]", Tools.GetTemplateParameterValue(@"{{cite|param1 = [[File:abc.png|asdf|dsfjk|a]]
|param2=other}}", "param1"));
            Assert.AreEqual(@"here <nowiki>|</nowiki> there", Tools.GetTemplateParameterValue(@"{{cite|param1 = here <nowiki>|</nowiki> there}}", "param1"));
            Assert.AreEqual(@"here <nowiki>|</nowiki> there", Tools.GetTemplateParameterValue(@"{{cite|param1 = here <nowiki>|</nowiki> there|parae=aaa}}", "param1"));
            Assert.AreEqual(@"here <pre>|</pre> there", Tools.GetTemplateParameterValue(@"{{cite|param1 = here <pre>|</pre> there|parae=aaa}}", "param1"));
            Assert.AreEqual(@"here <math>foo</math> there", Tools.GetTemplateParameterValue(@"{{cite|param1 = here <math>foo</math> there|parae=aaa}}", "param1"));
            Assert.AreEqual(@"here <timeline>foo</timeline> there", Tools.GetTemplateParameterValue(@"{{cite|param1 = here <timeline>foo</timeline> there|parae=aaa}}", "param1"));

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

            // value with braces in
            Assert.AreEqual("here {and} was", Tools.GetTemplateParameterValue(@"{{cite|param1= here {and} was }}", "param1"));
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
            
            Assert.AreEqual(Tools.GetTemplateParametersValues(template, parameters)[0], "abc");
            Assert.AreEqual(Tools.GetTemplateParametersValues(template, parameters)[1], "1 May 2009");

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

            Assert.AreEqual("foo=def", Tools.GetTemplateArgument(@"{{abc|foo=def}}", 1));
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
            Assert.AreEqual(0, Tools.GetTemplateArgumentCount(@"{{foo}}", true));
            Assert.AreEqual(1, Tools.GetTemplateArgumentCount(@"{{foo|}}"));
            Assert.AreEqual(1, Tools.GetTemplateArgumentCount(@"{{foo|bar}}"), "counts unnamed parameters");
            Assert.AreEqual(0, Tools.GetTemplateArgumentCount(@"{{foo|bar}}", true), "counts populated named parameters only when requested");
            Assert.AreEqual(0, Tools.GetTemplateArgumentCount(@"{{foo|bar=}}", true), "counts populated named parameters only when requested");
            Assert.AreEqual(1, Tools.GetTemplateArgumentCount(@"{{foo|bar=yes}}"), "counts named parameters");
            Assert.AreEqual(1, Tools.GetTemplateArgumentCount(@"{{foo|bar=yes}}", true), "counts named parameters");
            Assert.AreEqual(3, Tools.GetTemplateArgumentCount(@"{{foo|bar=yes|asdf=iiuu|asdfsadf=|eaa=ef}}", true), "counts named parameters");

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
            List<string> Params = new List<string>(new[] { "param1" });

            Assert.AreEqual(@"{{cite |paramx=bar|param2=great}}", Tools.RenameTemplateParameter(@"{{cite |param1=bar|param2=great}}", Params, "paramx"));

            Params.Add("param2");

            Assert.AreEqual(@"{{cite |paramx=bar|paramx=great}}", Tools.RenameTemplateParameter(@"{{cite |param1=bar|param2=great}}", Params, "paramx"));

            Params.Add("param3");
            Assert.AreEqual(@"{{cite |paramx=bar|paramx=great}}", Tools.RenameTemplateParameter(@"{{cite |param1=bar|param2=great}}", Params, "paramx"));
            Assert.AreEqual(@"{{cite |paramx=bar|param4=great}}", Tools.RenameTemplateParameter(@"{{cite |param1=bar|param4=great}}", Params, "paramx"));
        }

        [Test]
        public void RenameTemplateParameterDictionary()
        {
            Dictionary<string, string> Params = new Dictionary<string, string>();

            Params.Add("accesdate", "accessdate");
            Params.Add("acessdate", "accessdate");

            Assert.AreEqual(@"{{cite | accessdate=2011-01-24 | title=yes }}", Tools.RenameTemplateParameter(@"{{cite | accesdate=2011-01-24 | title=yes }}", Params));
            Assert.AreEqual(@"{{cite | accessdate=2011-01-24 | title=yes }}", Tools.RenameTemplateParameter(@"{{cite | acessdate=2011-01-24 | title=yes }}", Params));
        }

        [Test]
        public void RenameTemplateArticleText()
        {
            string correct = @"Now {{bar}} was {{bar|here}} there", correct2 = @"Now {{bar}} was {{bar
|here
|other}} there", correct3 = @"Now {{bar man}} was {{bar man|here}} there",
            correct4 = @"Now {{bar man<!--comm-->}} was {{bar man<!--comm-->|here}} there";
            Assert.AreEqual(correct, Tools.RenameTemplate(@"Now {{foo}} was {{foo|here}} there", "foo", "bar"));
            Assert.AreEqual(correct, Tools.RenameTemplate(@"Now {{foo}} was {{foo|here}} there", "Foo", "bar"));

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

        private readonly HideText Hider = new HideText();

        [Test]
        public void RenameTemplate()
        {
            Assert.AreEqual(@"{{bar}}", Tools.RenameTemplate(@"{{foo}}", "bar"));

            // space kept
            Assert.AreEqual(@"{{ bar }}", Tools.RenameTemplate(@"{{ foo }}", "bar"));

            // casing
            Assert.AreEqual(@"{{bar}}", Tools.RenameTemplate(@"{{Foo}}", "bar", false));
            Assert.AreEqual(@"{{Bar}}", Tools.RenameTemplate(@"{{Foo}}", "bar", true));
            Assert.AreEqual(@"{{Bar}}", Tools.RenameTemplate(@"{{Foo}}", "Bar"));
            Assert.AreEqual(@"{{Bar}}", Tools.RenameTemplate(@"{{Foo}}", "Bar", true));

            // with params
            Assert.AreEqual(@"{{bar|parameters=adsfjk}}", Tools.RenameTemplate(@"{{Foo|parameters=adsfjk}}", "bar", false));
            Assert.AreEqual(@"{{bar |parameters=adsfjk}}", Tools.RenameTemplate(@"{{Foo |parameters=adsfjk}}", "bar", false));
            Assert.AreEqual(@"{{bar |parameters=adsfjk}}", Tools.RenameTemplate(@"{{foo |parameters=adsfjk}}", "Bar", true));

            // comment handling
            Assert.AreEqual(@"{{bar man<!--comm-->|here}}", Tools.RenameTemplate(@"{{foo man<!--comm-->|here}}", "bar man"));
            Assert.AreEqual(@"{{bar man <!--comm-->|here}}", Tools.RenameTemplate(@"{{foo man <!--comm-->|here}}", "bar man"));
            Assert.AreEqual(@"{{bar man <!--comm-->
|here}}", Tools.RenameTemplate(@"{{foo man <!--comm-->
|here}}", "bar man"));

            Assert.AreEqual(@"{{bar man <!--comm-->|here}}", Hider.AddBack(Tools.RenameTemplate(Hider.Hide(@"{{foo man <!--comm-->|here}}"), "bar man")));

            // special case of subst:, first letter case rule does not apply
            Assert.AreEqual(@"{{subst:PAGENAME}}", Tools.RenameTemplate(@"{{PAGENAME}}", "PAGENAME", "subst:PAGENAME"));
            Assert.AreEqual(@"{{subst:PAGENAME}}", Tools.RenameTemplate(@"{{PAGENAME}}", "PAGENAME", "subst:PAGENAME", false));
            Assert.AreEqual(@"{{subst:PAGENAME}}", Tools.RenameTemplate(@"{{PAGENAME}}", "PAGENAME", "subst:PAGENAME", true));

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
        public void RemoveTemplateParametersSingleTemplate()
        {
            List<string> Params = new List<string>();
            Params.Add("dateformat");

            string correct = @"{{cite web|url=http://www.site.com |title=here |year=2008 }}";

            Assert.AreEqual(correct, Tools.RemoveTemplateParameters(@"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat=mdy}}", Params));
            Assert.AreEqual(correct, Tools.RemoveTemplateParameters(@"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat=mdy }}", Params));
            Assert.AreEqual(correct, Tools.RemoveTemplateParameters(@"{{cite web|url=http://www.site.com |title=here |year=2008 |dateformat=}}", Params));
            Assert.AreEqual(correct, Tools.RemoveTemplateParameters(@"{{cite web|url=http://www.site.com |title=here |year=2008 | dateformat =   mdy}}", Params));
            Assert.AreEqual(correct, Tools.RemoveTemplateParameters(@"{{cite web|url=http://www.site.com |title=here | dateformat=mdy|year=2008 }}", Params));
            Params.Add("format");

            Assert.AreEqual(correct, Tools.RemoveTemplateParameters(@"{{cite web|url=http://www.site.com |title=here | dateformat=mdy|year=2008 |format=DOC}}", Params));
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

            correct = @"{{Fred | first=Bar | upper={{Bert|lower=yes}} }}";

            Assert.AreEqual(correct, Tools.RenameTemplateParameter(@"{{Fred | first=Bar | lower={{Bert|lower=yes}} }}", "lower", "upper"), @"Parameter within nested template not renamed");
            Assert.AreEqual(correct, Tools.RenameTemplateParameter(@"{{Fred | last=Bar | upper={{Bert|lower=yes}} }}", "last", "first"), @"Template parameters can be rename when nested templates present");

            correct = @"{{Fred | first=Bar | upper={{Bert|lower=yes|2={{great}} }} }}";

            Assert.AreEqual(correct, Tools.RenameTemplateParameter(@"{{Fred | first=Bar | lower={{Bert|lower=yes|2={{great}} }} }}", "lower", "upper"), @"Parameter within nested nested template not renamed");
        }

        [Test]
        public void RemoveDuplicateTemplateParameters()
        {
            Assert.AreEqual(@"{{foo|first=abc|second=def}}", Tools.RemoveDuplicateTemplateParameters(@"{{foo|first=abc|second=def|second=def}}"));
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
        
            Dictionary<string, string> Params = new Dictionary<string, string>();
            Tools.RemoveDuplicateTemplateParameters(@"{{foo|first=abc|second=def|second=def}}", Params);
            Assert.AreEqual(Params.Count, 2);
            Params.Clear();
            Tools.RemoveDuplicateTemplateParameters(noDupe2, Params);
            Assert.AreEqual(Params.Count, 3);
            Params.Clear();
            Tools.RemoveDuplicateTemplateParameters(@"{{foo|first=abc||second=  def <!--com-->  }}", Params);
            string d = "";
            Assert.IsTrue(Params.TryGetValue("second", out d));
            Assert.AreEqual("def <!--com-->", d, "parameter with space and comment retrieved correctly");

            Assert.AreEqual(@"{{foo|first=abc|second2=def|second2=def}}", Tools.RemoveDuplicateTemplateParameters(@"{{foo|first=abc|second2=def|second2=def}}"));
        }

        [Test]
        public void RemoveDuplicateTemplateParametersURLs()
        {
            const string withUnescapedPipes = @"{{cite web|foo=bar|url=http://site.com/news/foo|bar=yes|bar=yes|other.stm | date=2010}}";

            Assert.AreEqual(withUnescapedPipes, Tools.RemoveDuplicateTemplateParameters(withUnescapedPipes), "no change when URL could be borken");

            Assert.AreEqual(@"{{foo|first=abc| url=http://site.com/news }}", Tools.RemoveDuplicateTemplateParameters(@"{{foo|first=abc|url= | url=http://site.com/news }}"));

            Assert.AreEqual(@"{{cite web|url=a |title=b | accessdate=11 May 2008|year=2008}}", Tools.RemoveDuplicateTemplateParameters(@"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008|year=2008}}"));
        }

        [Test]
        public void DuplicateTemplateParameters()
        {
            Dictionary<int, int> Dupes = new Dictionary<int, int>();

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

            List<string> Knowns = new List<string>(new[] { "title", "date", "url" });

            Assert.AreEqual(Unknowns, Tools.UnknownTemplateParameters(@"{{cite web|title=a|date=2010}}", Knowns));
            // Confirm other casing work too
            Assert.AreEqual(Unknowns, Tools.UnknownTemplateParameters(@"{{cite web|TITLE=a|DATE=2010}}", Knowns));

            Unknowns.Add("foo");
            Assert.AreEqual(Unknowns, Tools.UnknownTemplateParameters(@"{{cite web|title=a|date=2010|foo=}}", Knowns), "reported even if blank");
            Assert.AreEqual(Unknowns, Tools.UnknownTemplateParameters(@"{{cite web|title=a|date=2010|foo=b}}", Knowns), "unknown parameter reported");

            Unknowns.Add("bar");
            Assert.AreEqual(Unknowns, Tools.UnknownTemplateParameters(@"{{cite web|title=a|date=2010|foo=b|bar=}}", Knowns), "multiple unknowns reported");
        }

        [Test]
        public void RemoveExcessTemplatePipes()
        {
            Assert.AreEqual(@"{{foo|param1=before}}", Tools.RemoveExcessTemplatePipes(@"{{foo||param1=before}}"), "excess pipe removed");
            Assert.AreEqual(@"{{foo|param1=before}}", Tools.RemoveExcessTemplatePipes(@"{{foo||param1=before|}}"), "excess pipes removed");
            Assert.AreEqual(@"{{foo|param1=before}}", Tools.RemoveExcessTemplatePipes(@"{{foo| |param1=before}}"), "excess spaced pipe removed");
            Assert.AreEqual(@"{{foo|param1=before}}", Tools.RemoveExcessTemplatePipes(@"{{foo|
		|param1=before}}"), "space is newline");
            Assert.AreEqual(@"{{foo|param1=before}}", Tools.RemoveExcessTemplatePipes(@"{{foo|     |param1=before}}"));
            Assert.AreEqual(@"{{foo|param1=before|param2=b}}", Tools.RemoveExcessTemplatePipes(@"{{foo||param1=before||param2=b}}"), "mulitple excess pipes");
            Assert.AreEqual(@"{{foo|param1=before}}", Tools.RemoveExcessTemplatePipes(@"{{foo||param1=before|}}"), "excess pipe at end");
            const string nested = @"{{foo| one={{bar|a||c}}|two=x}}";
            Assert.AreEqual(nested, Tools.RemoveExcessTemplatePipes(nested), "pipes within nested templates not changed");
        }

        [Test]
        public void UpdateTemplateParameterValue()
        {
            Assert.AreEqual(@"{{foo|param1=valueafter}}", Tools.UpdateTemplateParameterValue(@"{{foo|param1=before}}", "param1", "valueafter"));
            Assert.AreEqual(@"{{foo|param1= valueafter }}", Tools.UpdateTemplateParameterValue(@"{{foo|param1= before }}", "param1", "valueafter"), "whitespace kept");
            Assert.AreEqual(@"{{foo|param1=
valueafter}}", Tools.UpdateTemplateParameterValue(@"{{foo|param1=
before}}", "param1", "valueafter"), "newline before populated parameter kept");

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

            string input = @"{{cite news
|author=
|title=Obituary 1 -- No Title
|date=
|work=[[New York Times]]
|url=http://query.nytimes.com/gst/abstra
|accessdate=2008-08-08
}}";
            Assert.AreEqual(input.Replace("|date=", "|date=April 4, 1922"), Tools.UpdateTemplateParameterValue(input, "date", "April 4, 1922"));

            input = @"{{cite news
|author=
|title=Obituary 1 -- No Title
|date=


|work=[[New York Times]]
|url=http://query.nytimes.com/gst/abstra }}";

            Assert.AreEqual(input.Replace("|date=", "|date=April 4, 1922"), Tools.UpdateTemplateParameterValue(input, "date", "April 4, 1922"));
        }

        [Test]
        public void AddTemplateParameterValue()
        {
            Assert.AreEqual(@"{{foo|param1=oldvalue}}",
                Tools.AddTemplateParameterValue(@"{{foo|param1=oldvalue}}", "param1", "newvalue", false));

            Assert.AreEqual(@"{{foo | param1=newvalue}}",
                Tools.AddTemplateParameterValue(@"{{foo}}", "param1", "newvalue", false));

            Assert.AreEqual(@"{{foo | param1= newvalue}}",
                Tools.AddTemplateParameterValue(@"{{foo}}", "param1", "newvalue", true));
        }

        [Test]
        public void SetTemplateParameterValue()
        {
            Assert.AreEqual(@"{{foo|param1=valueafter}}", Tools.SetTemplateParameterValue(@"{{foo|param1=before}}", "param1", "valueafter"));
            Assert.AreEqual(@"{{foo|param1= valueafter }}", Tools.SetTemplateParameterValue(@"{{foo|param1= before }}", "param1", "valueafter"));

            Assert.AreEqual(@"{{foo|param1= valueafter}}", Tools.SetTemplateParameterValue(@"{{foo|param1=before}}", "param1", "valueafter", true));
            Assert.AreEqual(@"{{foo|param1=valueafter}}", Tools.SetTemplateParameterValue(@"{{foo|param1=before}}", "param1", "valueafter", false));

            Assert.AreEqual(@"{{foo|param1=valueafter|param2=before}}", Tools.SetTemplateParameterValue(@"{{foo|param1=before|param2=before}}", "param1", "valueafter"));

            // parameter not used – set
            Assert.AreEqual(@"{{foo|param1=before | param2=valueafter}}", Tools.SetTemplateParameterValue(@"{{foo|param1=before}}", "param2", "valueafter"));
            Assert.AreEqual(@"{{foo|param1=before|param3=a | param2=valueafter}}", Tools.SetTemplateParameterValue(@"{{foo|param1=before|param3=a}}", "param2", "valueafter"));
            Assert.AreEqual(@"{{foo|param1=w|param2=x|param3=y|param4=z|param5=a}}", Tools.SetTemplateParameterValue(@"{{foo|param1=w|param2=x|param3=y|param4=z}}", "param5", "a"));
            Assert.AreEqual(@"{{foo| param1=w | param2=x | param3=y | param4=z | param5=a }}", Tools.SetTemplateParameterValue(@"{{foo| param1=w | param2=x | param3=y | param4=z }}", "param5", "a"), "retain template call end space");

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

            string input = @"{{cite news
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
        public void NestedTemplateRegexSingleWord()
        {
            Regex FooTemplate = Tools.NestedTemplateRegex("foo");

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

            FooTemplate = Tools.NestedTemplateRegex("Foo");
            Assert.IsTrue(FooTemplate.IsMatch(@"{{foo}}"));
            Assert.IsTrue(FooTemplate.IsMatch(@"{{Foo}}"));
            Assert.IsTrue(FooTemplate.IsMatch(@"{{ foo}}"));
            Assert.IsTrue(FooTemplate.IsMatch(@"{{Template:foo}}"));
            Assert.IsTrue(FooTemplate.IsMatch(@"{{:Template:foo}}"));
            Assert.IsTrue(FooTemplate.IsMatch(@"{{:Msg:foo}}"));
            Assert.IsTrue(FooTemplate.IsMatch(@"{{Msg:foo}}"));
            Assert.IsTrue(FooTemplate.IsMatch(@"{{_:_Template_:_foo_}}"));

            Assert.IsFalse(FooTemplate.IsMatch(@"{{Template foo}}"));

            FooTemplate = Tools.NestedTemplateRegex("foo", true);
            Assert.AreEqual(FooTemplate.Match(@"{{  foo}}").Groups[1].Value, @"{{  ");

            FooTemplate = Tools.NestedTemplateRegex("", true);
            Assert.AreEqual(null, FooTemplate);
            
            Variables.NamespacesCaseInsensitive.Remove(Namespace.Template);
            FooTemplate = Tools.NestedTemplateRegex("Foo", true);
            Assert.IsTrue(FooTemplate.IsMatch(@"{{Template:foo}}"));
            Variables.NamespacesCaseInsensitive.Add(Namespace.Template, "[Tt]emplate:");
        }

        [Test]
        public void NestedTemplateRegexRTL()
        {
            Regex ArTemplate = Tools.NestedTemplateRegex(@"وصلات قليلة");
            Assert.IsTrue(ArTemplate.IsMatch(@"{{وصلات قليلة|تاريخ=ديسمبر 2012}}"));
            Assert.AreEqual("", ArTemplate.Replace(@"{{وصلات قليلة|تاريخ=ديسمبر 2012}}", ""));
            Assert.AreEqual("", ArTemplate.Replace(@"{{وصلات قليلة|تاريخ=يناير_2009}}", ""));
        }

        [Test]
        public void NestedTemplateRegexTwoWords()
        {
            Regex FooTemplate2 = Tools.NestedTemplateRegex("foo bar");

            Assert.IsTrue(FooTemplate2.IsMatch(@"{{foo bar}}"));
            Assert.IsTrue(FooTemplate2.IsMatch(@"{{foo_bar}}"));
            Assert.IsTrue(FooTemplate2.IsMatch(@"{{foo_____bar}}"));
            Assert.IsTrue(FooTemplate2.IsMatch(@"{{Foo bar}}"));
            Assert.IsTrue(FooTemplate2.IsMatch(@"{{Foo      bar}}"));

            Assert.IsFalse(FooTemplate2.IsMatch(@"{{foo}}"));
            Assert.IsFalse(FooTemplate2.IsMatch(@"{{foo b_ar}}"));
            Assert.IsFalse(FooTemplate2.IsMatch(@"{{foo
bar|text}}"));
            Assert.IsFalse(Tools.NestedTemplateRegex("birth date").IsMatch(@"{{birth-date|May 11, 1980}}"));
        }

        [Test]
        public void NestedTemplateRegexLimits()
        {
            Regex FooTemplate2 = Tools.NestedTemplateRegex("");

            Assert.IsNull(FooTemplate2);
        }

        [Test]
        public void NestedTemplateRegexListSingle()
        {
            List<string> ListOfTemplates = new List<string>();

            Regex MultipleTemplatesN = Tools.NestedTemplateRegex(ListOfTemplates);
            Assert.IsNull(MultipleTemplatesN, "null return if zero-entry list input");

            ListOfTemplates.Add(@"Foo");

            Regex multipleTemplates = Tools.NestedTemplateRegex(ListOfTemplates);

            Assert.AreEqual(multipleTemplates.Match(@"{{foo}}").Groups[2].Value, @"foo", "Group 2 is template name");
            Assert.AreEqual(multipleTemplates.Match(@"{{ Foo}}").Groups[2].Value, @"Foo", "Group 2 is template name");
            Assert.AreEqual(multipleTemplates.Match(@"{{ Foo|bar}}").Groups[3].Value, @"|bar}}", "Group 3 is template from bar to end");
            Assert.AreEqual(multipleTemplates.Match(@"{{ Foo|bar=one|he=there}}").Groups[3].Value, @"|bar=one|he=there}}", "Group 3 is template from bar to end");
            Assert.AreEqual(multipleTemplates.Match(@"{{ Foo|bar={{one}}|he=there}}").Groups[3].Value, @"|bar={{one}}|he=there}}", "Group 3 is template from bar to end");
            Assert.AreEqual(multipleTemplates.Match(@"{{ Foo|bar={{one}}|he=ther {{e}}}}").Groups[3].Value, @"|bar={{one}}|he=ther {{e}}}}", "Group 3 is template from bar to end");

            Assert.AreEqual(multipleTemplates.Match(@"{{ Foo|bar={{one}}|he=there}}").Groups[4].Value, @"|bar={{one}}|he=there", "Group 4 is template from bar to end excluding end }}");
            Assert.AreEqual(multipleTemplates.Match(@"{{ Foo|bar={{one}}|he=ther {{e}}}}").Groups[4].Value, @"|bar={{one}}|he=ther {{e}}", "Group 4 is template from bar to end excluding end }}");

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

            ListOfTemplates.Clear();
            ListOfTemplates.Add(@"Foo ");
            multipleTemplates = Tools.NestedTemplateRegex(ListOfTemplates);
            Assert.AreEqual(multipleTemplates.Match(@"{{foo}}").Groups[2].Value, @"foo", "matches correctly from input template name with trailing whitespace");
        }

        [Test]
        public void NestedTemplateRegexListMultiple()
        {
            List<string> listOfTemplates = new List<string>(new[] { "Foo", "bar" });

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
        public void MergeTemplateParametersTemplateName()
        {
            string correct = @"{{Foo | e=b d }}";
            List<string> ToMerge = new List<string>(new[] { "a", "c" });

            Assert.AreEqual(correct, Tools.MergeTemplateParametersValues(@"{{Foo|a= b|c= d}}", ToMerge, "e", true), "single spaces");
            Assert.AreEqual(correct, Tools.MergeTemplateParametersValues(@"{{Foo|a=b|c=d}}", ToMerge, "e", true), "no spaces");
            Assert.AreEqual(correct, Tools.MergeTemplateParametersValues(@"{{Foo|a=   b|c=d}}", ToMerge, "e", true), "big spaces");
        }

        [Test]
        public void GetMetaContentValue()
        {
            Assert.AreEqual(@"2009-03-02", Tools.GetMetaContentValue(@"<meta name=""PubDate""  content=""2009-03-02"">", "PubDate"));
            Assert.AreEqual(@"2009-03-02", Tools.GetMetaContentValue(@"<meta name='PubDate'  content='2009-03-02'>", "PubDate"));
            Assert.AreEqual(@"2009-03-02", Tools.GetMetaContentValue(@"< META NAME = ""PubDate""  content = ""2009-03-02""  />", "PubDate"));
            Assert.AreEqual(@"2009-03-02", Tools.GetMetaContentValue(@"< meta itemprop = ""PubDate""  content = ""2009-03-02""  />", "PubDate"));
            Assert.AreEqual(@"2009-03-02", Tools.GetMetaContentValue(@"<meta name  =""PubDate"" CONTENT="" 2009-03-02 "">", "PUBDATE"));
            Assert.AreEqual(@"2009-03-02", Tools.GetMetaContentValue(@"<meta name  =""PubDate"" scheme=""URI"" CONTENT="" 2009-03-02 "">", "PUBDATE"));
            Assert.AreEqual(@"2009-03-02", Tools.GetMetaContentValue(@"<meta property  =""PubDate"" scheme=""URI"" CONTENT="" 2009-03-02 "">", "PUBDATE"));
            Assert.AreEqual(@"Football: Ken P is headed to X", Tools.GetMetaContentValue(@"<meta property=""og:title"" content=""Football: Ken P is headed to X"" data-meta-updatable/>", "og:title"));

            Assert.AreEqual(@"", Tools.GetMetaContentValue(@"<meta name  =""PubDate"" CONTENT="" 2009-03-02 "">", "PUBDATEXX"));
            Assert.AreEqual(@"", Tools.GetMetaContentValue(@"<meta name  =""PubDateX"" CONTENT="" 2009-03-02 "">", "PUBDATE"));

            Assert.AreEqual(@"", Tools.GetMetaContentValue(@"<meta name  =""PubDateX"" CONTENT="" 2009-03-02 "">", ""));
            Assert.AreEqual(@"", Tools.GetMetaContentValue("", "PUBDATE"));

            Assert.AreEqual(@"10.1111/j.1096-0031.2009.00267.x", Tools.GetMetaContentValue(@"<meta xmlns=""http://www.w3.org/1999/xhtml"" name=""citation_doi"" content=""10.1111/j.1096-0031.2009.00267.x"" />", "citation_doi"));
            Assert.AreEqual(@"10.1093/nar/27.19.3821", Tools.GetMetaContentValue(@"<meta content=""10.1093/nar/27.19.3821"" name=""DC.Identifier"" />", "DC.Identifier"));
            Assert.AreEqual(@"10.1101/gr.7.4.359", Tools.GetMetaContentValue(@"<meta content=""10.1101/gr.7.4.359"" name=""DC.Identifier"" />", "DC.Identifier"));

            Assert.AreEqual(@"Air wasn't x", Tools.GetMetaContentValue(@"<meta id=""og_title"" property=""og:title"" content=""Air wasn't x""/>", "og:title"));
            Assert.AreEqual(@"Air wasn't x", Tools.GetMetaContentValue(@"<meta property=""og:title"" content=""Air wasn't x"">", "og:title"));
            Assert.AreEqual(@"Air wasn't x", Tools.GetMetaContentValue(@"<meta data-ephemeral=""true"" property=""og:title"" content=""Air wasn't x""/>", "og:title"));
            Assert.AreEqual(@"Air wasn't x", Tools.GetMetaContentValue(@"<meta data-react-helmet=""true"" property=""og:title"" content=""Air wasn't x""/>", "og:title"));

            // <meta data-react-helmet="true" name="citation_doi" content="10.1016/0022-0000(78)90043-0"/
            Assert.AreEqual(@"Air wasn't x", Tools.GetMetaContentValue(@"<meta ng-attr-content=""{{meta.title}}"" property=""og:title"" content=""Air wasn't x""/>", "og:title"));
        }
        
        [Test]
        public void UnescapeXML()
        {
            Assert.AreEqual(@"<tag>value</tag>", Tools.UnescapeXML(@"<tag>value</tag>"));
            Assert.AreEqual("", Tools.UnescapeXML(""));
            Assert.AreEqual(@"<tag>A&B</tag>", Tools.UnescapeXML(@"<tag>A&amp;B</tag>"));
        }

        [Test]
        public void GetTemplateName()
        {
            Assert.AreEqual(Tools.GetTemplateName(@"{{Start date and age|1833|7|11}}"), "Start date and age");

            // whitespace handling
            Assert.AreEqual(Tools.GetTemplateName(@"{{ Start date and age |1833|7|11}}"), "Start date and age");
            Assert.AreEqual(Tools.GetTemplateName(@"{{
Start date and age
|1833|7|11}}"), "Start date and age");

            Assert.AreEqual(Tools.GetTemplateName(@"{{start date and age <!--comm--> |1833|7|11}}"), "start date and age", "handles embedded comments");
            Assert.AreEqual(Tools.GetTemplateName(@"{{start date and age <!--comm-->}}"), "start date and age", "handles embedded comments");
            Assert.AreEqual(Tools.GetTemplateName(@"{{start date and age ⌊⌊⌊⌊1⌋⌋⌋⌋}}"), "start date and age", "handles embedded comments (hidetext)");

            Assert.AreEqual(Tools.GetTemplateName(@"{{Start date and age|1833|7|"), "Start date and age", "works on part templates");

            Assert.AreEqual(Tools.GetTemplateName(@"{{Template:Foo|1=yes}}"), "Foo", "Template namespace removed");
            Assert.AreEqual(Tools.GetTemplateName(@"{{ Template : Foo |1=yes}}"), "Foo", "Template namespace removed");
            Assert.AreEqual(Tools.GetTemplateName(@"{{template:Foo|1=yes}}"), "Foo", "Template namespace removed");
            Assert.AreEqual(Tools.GetTemplateName(@"{{Foo_one|1=yes}}"), "Foo one", "underscore cleaned");
            Assert.AreEqual(Tools.GetTemplateName(@"{{_Foo_one|1=yes}}"), "Foo one", "Leading underscore cleaned");
            Assert.AreEqual(Tools.GetTemplateName(@"{{Foo___one|1=yes}}"), "Foo one", "underscores cleaned");
            Assert.AreEqual(Tools.GetTemplateName(@"{{Foo   one|1=yes}}"), "Foo one", "underscores cleaned");

            Assert.AreEqual(Tools.GetTemplateName(@""), "");

            Variables.NamespacesCaseInsensitive.Remove(Namespace.Template);
            Assert.AreEqual(Tools.GetTemplateName(@""), "");
            Variables.NamespacesCaseInsensitive.Add(Namespace.Template, "[Tt]emplate:");
        }

        [Test]
        public void ReAddDiacritics()
        {
            Assert.AreEqual(@"Floué, John", Tools.ReAddDiacritics(@"Floué, John", @"Floue, John"), "diacritics reapplied");
            Assert.AreEqual(@"Floué", Tools.ReAddDiacritics(@"Floué", @"Floue"), "diacritics reapplied");
            Assert.AreEqual(@"Floué, John", Tools.ReAddDiacritics(@" Floué, John ", @"Floue, John"), "diacritics reapplied");
            Assert.AreEqual(@" Floué, John ", Tools.ReAddDiacritics(@" Floué, John ", @" Floue, John "), "diacritics reapplied");

            Assert.AreEqual(@"Floué, John", Tools.ReAddDiacritics(@"John Floué", @"Floue, John"), "diacritics reapplied, word order irrelevant");
            Assert.AreEqual(@"Floué, John", Tools.ReAddDiacritics(@"John von Floué", @"Floue, John"), "diacritics reapplied, word order irrelevant");

            Assert.AreEqual(@"Greatère, Véry", Tools.ReAddDiacritics(@"Greatère, Véry", @"Greatere, Very"), "multiple words changed");
            Assert.AreEqual(@"Greatère, Véry", Tools.ReAddDiacritics(@"Véry Greatère", @"Greatere, Very"), "multiple words changed");
            Assert.AreEqual(@"Greatère, Very der Very", Tools.ReAddDiacritics(@"Véry Greatère", @"Greatere, Very der Very"), "when multiple matches for same word without, that word not changed");
            Assert.AreEqual(@"Greatère, Very der Very", Tools.ReAddDiacritics(@"Véry de Vèry Greatère", @"Greatere, Very der Very"), "when multiple matches for same word without, that word not changed");
        }
        
        [Test]
        public void IsIP()
        {
            Assert.IsTrue(Tools.IsIP("192.168.0.1"));
            Assert.IsTrue(Tools.IsIP("8.8.8.8"));
            Assert.IsFalse(Tools.IsIP("www.google.com"));
        }

        [Test]
        public void BuildPostDataString()
        {
            System.Collections.Specialized.NameValueCollection nvc = new System.Collections.Specialized.NameValueCollection();

            nvc.Add("param1", "value1");
            Assert.AreEqual("param1=value1", Tools.BuildPostDataString(nvc));
            nvc.Add("param2", "value2");
            Assert.AreEqual("param1=value1&param2=value2", Tools.BuildPostDataString(nvc));
            nvc.Add("param3", "A B C");
            Assert.AreEqual("param1=value1&param2=value2&param3=A+B+C", Tools.BuildPostDataString(nvc));
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
            Assert.AreEqual(@"{{DEFAULTSORT:Foo}}", Tools.TemplateToMagicWord(@"{{DEFAULTSORT|Foo}}"));
            Assert.AreEqual(@"{{DISPLAYTITLE:Foo}}", Tools.TemplateToMagicWord(@"{{DISPLAYTITLE|Foo}}"));
            Assert.AreEqual(@"{{DISPLAYTITLE:Foo}}", Tools.TemplateToMagicWord(@"{{Displaytitle|Foo}}"));
            Assert.AreEqual(@"{{FULLPAGENAME:Foo}}", Tools.TemplateToMagicWord(@"{{FULLPAGENAME|Foo}}"));
            Assert.AreEqual(@"{{FULLPAGENAME:Foo}}", Tools.TemplateToMagicWord(@"{{Fullpagename|Foo}}"));
            Assert.AreEqual(@"{{namespace:Foo}}", Tools.TemplateToMagicWord(@"{{Namespace|Foo}}"));
            Assert.AreEqual(@"{{numberofarticles:Foo}}", Tools.TemplateToMagicWord(@"{{Numberofarticles|Foo}}"));
            Assert.AreEqual(@"{{PAGENAME:Foo}}", Tools.TemplateToMagicWord(@"{{PAGENAME|Foo}}"));
            Assert.AreEqual(@"{{PAGESIZE:Foo}}", Tools.TemplateToMagicWord(@"{{PAGESIZE|Foo}}"));
            Assert.AreEqual(@"{{PROTECTIONLEVEL:Foo}}", Tools.TemplateToMagicWord(@"{{PROTECTIONLEVEL|Foo}}"));
            Assert.AreEqual(@"{{PAGENAME:Foo}}", Tools.TemplateToMagicWord(@"{{Pagename|Foo}}"));
            Assert.AreEqual(@"{{PAGENAME:Foo}}", Tools.TemplateToMagicWord(@"{{pagename|Foo}}"));
            Assert.AreEqual(@"{{SUBPAGENAME:Foo}}", Tools.TemplateToMagicWord(@"{{SUBPAGENAME|Foo}}"));
            Assert.AreEqual(@"{{SUBPAGENAME:Foo}}", Tools.TemplateToMagicWord(@"{{Subpagename|Foo}}"));
            Assert.AreEqual(@"{{padleft:Foo}}", Tools.TemplateToMagicWord(@"{{padleft|Foo}}"));

            Assert.AreEqual(@"{{DEFAULTSORT:Foo}}", Tools.TemplateToMagicWord(@"{{ DEFAULTSORT |Foo}}"));
            Assert.AreEqual(@"{{DISPLAYTITLE:''Foo''}}", Tools.TemplateToMagicWord(@"{{DISPLAYTITLE|''Foo''}}"));
            Assert.AreEqual(@"{{DISPLAYTITLE:''Foo''}} {{DEFAULTSORT:Foo}}", Tools.TemplateToMagicWord(@"{{DISPLAYTITLE|''Foo''}} {{DEFAULTSORT|Foo}}"));
            Assert.AreEqual(@"{{BASEPAGENAME:Foo}}", Tools.TemplateToMagicWord(@"{{BASEPAGENAME|Foo}}"));
            Assert.AreEqual(@"{{FULLPAGENAME}}", Tools.TemplateToMagicWord(@"{{FULLPAGENAME}}"));
        }
        
        [Test]
        public void IsSectionOrReasonTemplate()
        {
            Assert.IsTrue(Tools.IsSectionOrReasonTemplate(@"{{wikify|reason=foo}}"));
            Assert.IsTrue(Tools.IsSectionOrReasonTemplate(@"{{wikify|date=May 2012|reason=foo}}"));
            Assert.IsTrue(Tools.IsSectionOrReasonTemplate(@"{{wikify|section|date=May 2012}}"));
            Assert.IsTrue(Tools.IsSectionOrReasonTemplate(@"{{wikify|section|date=May 2012}}", ""));
            Assert.IsTrue(Tools.IsSectionOrReasonTemplate(@"{{foo}}", @"{{multiple issues|section=y|POV=May 2012}}"));
            
            Assert.IsFalse(Tools.IsSectionOrReasonTemplate(@"{{abc|param1=foo}}"));
            Assert.IsFalse(Tools.IsSectionOrReasonTemplate(@"{{abc|section=foo}}"));
            Assert.IsFalse(Tools.IsSectionOrReasonTemplate(@"{{abc}}"));
        }
        
        [Test]
        public void HowMuchStartsWithTests()
        {
            Assert.AreEqual(9, Tools.HowMuchStartsWith(@"{{foo}}
hello", Tools.NestedTemplateRegex("foo"), false));
            
            Assert.AreEqual(9, Tools.HowMuchStartsWith(@"{{foo}}
hello", Tools.NestedTemplateRegex("foo"), true));
            
            Assert.AreEqual(9, Tools.HowMuchStartsWith(@"{{foo}}
hello {{foo}}", Tools.NestedTemplateRegex("foo"), false));
            
            Assert.AreEqual(10, Tools.HowMuchStartsWith(@" {{foo}}
hello", Tools.NestedTemplateRegex("foo"), false));
            
            Assert.AreEqual(0, Tools.HowMuchStartsWith(@"hello{{foo}}
hello", Tools.NestedTemplateRegex("foo"), false));
            
            Assert.AreEqual(0, Tools.HowMuchStartsWith(@"==hello==
{{foo}}
hello", Tools.NestedTemplateRegex("foo"), false));
            
            Assert.AreEqual(0, Tools.HowMuchStartsWith(@"hello{{foo}}
hello", Tools.NestedTemplateRegex("foo"), true));
            
            Assert.AreEqual(18, Tools.HowMuchStartsWith(@"{{foo}}
{{foo}}
hello", Tools.NestedTemplateRegex("foo"), false));
            
            Assert.AreEqual(22, Tools.HowMuchStartsWith(@"{{foo}}
{{foo|bar}}
hello", Tools.NestedTemplateRegex("foo"), false));
            
            Assert.AreEqual(20, Tools.HowMuchStartsWith(@"==hello==
{{foo}}
hello", Tools.NestedTemplateRegex("foo"), true));
            
            Assert.AreEqual(22, Tools.HowMuchStartsWith(@"===hello===
{{foo}}
hello", Tools.NestedTemplateRegex("foo"), true));
            
            Assert.AreEqual(9, Tools.HowMuchStartsWith(@"{{foo}}
hello {{foo}}
==hi==
text", Tools.NestedTemplateRegex("foo"), false));
            
            Assert.AreEqual(9, Tools.HowMuchStartsWith(@"{{foo}}
hello {{foo}}
==hi==
text", Tools.NestedTemplateRegex("foo"), true));
            Assert.AreEqual(0, Tools.HowMuchStartsWith(@"===hello===
hello", Tools.NestedTemplateRegex("foo"), true));
        }

        [Test]
        public void PipeCleanedTemplate()
        {
            Assert.AreEqual(@"{{cite journal|title=A}}", Tools.PipeCleanedTemplate(@"{{cite journal|title=A}}", true));
            Assert.AreEqual(@"{{cite journal|title=A ~~~~~~~~~~ }}", Tools.PipeCleanedTemplate(@"{{cite journal|title=A <!-- a --> }}", true));
            Assert.AreEqual(@"{{cite journal|title=A ############## }}", Tools.PipeCleanedTemplate(@"{{cite journal|title=A [[here|there]] }}", true));
            Assert.AreEqual(@"{{cite journal|title=A ######## }}", Tools.PipeCleanedTemplate(@"{{cite journal|title=A [[here]] }}", true));
            Assert.AreEqual(@"{{cite journal|title=A ######## }}", Tools.PipeCleanedTemplate(@"{{cite journal|title=A {{here}} }}", true));
            Assert.AreEqual(@"{{cite journal|title=A ############ }}", Tools.PipeCleanedTemplate(@"{{cite journal|title=A <pre>a</pre> }}", true));
            Assert.AreEqual(@"{{cite journal|title=A ############## }}", Tools.PipeCleanedTemplate(@"{{cite journal|title=A <code>a</code> }}", true));
        }

        [Test]
        public void UnformattedTextNotChanged()
        {
            Assert.IsTrue(Tools.UnformattedTextNotChanged("", ""));
            Assert.IsTrue(Tools.UnformattedTextNotChanged("A", "A"));
            Assert.IsTrue(Tools.UnformattedTextNotChanged("<nowiki>A</nowiki>", "<nowiki>A</nowiki>"));
            Assert.IsTrue(Tools.UnformattedTextNotChanged("<nowiki>A</nowiki> <nowiki>B</nowiki>", "<nowiki>A</nowiki> <nowiki>B</nowiki>"));
            Assert.IsTrue(Tools.UnformattedTextNotChanged("<nowiki>A</nowiki> <nowiki>B</nowiki>", "<nowiki>A</nowiki>"), "Unformatted text entirely removed, true");
            Assert.IsTrue(Tools.UnformattedTextNotChanged("<nowiki>A</nowiki>", ""), "Unformatted text entirely removed, true");

            Assert.IsFalse(Tools.UnformattedTextNotChanged("<nowiki>A</nowiki> <nowiki>B</nowiki>", "<nowiki>C</nowiki>"));
            Assert.IsFalse(Tools.UnformattedTextNotChanged("<nowiki>A</nowiki>", "<nowiki></nowiki>"));
            Assert.IsFalse(Tools.UnformattedTextNotChanged("<nowiki>A</nowiki>", "<nowiki>B</nowiki>"), "Unformatted text changed removed, false");
            Assert.IsFalse(Tools.UnformattedTextNotChanged("<nowiki>A</nowiki>", "<nowiki></nowiki>B"), "Unformatted text changed (no content) removed, false");
        }

        [Test]
        public void GetMd5Sum()
        {
            Assert.AreEqual("5d41402abc4b2a76b9719d911017c592", Tools.GetMd5Sum("hello"));
        }

        [Test]
        public void SortDictionaryPairs()
        {
            CollectionAssert.AreEqual(
                new Dictionary<string, string>
                {
                    ["InfoC"] = "Baz",
                    ["InfoA"] = "foo",
                    ["InfoB"] = "Bar"
                },
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
                )
            );

            CollectionAssert.AreEqual(
                new Dictionary<string, string>
                {
                    ["InfoC"] = "Baz",
                    ["InfoA"] = "foo",
                    ["InfoB"] = "Bar",
                    ["InfoZ"] = "Zed",
                    ["InfoY"] = "Why"
                },
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
                )
            );
        }

        [Test]
        public void SortTemplateCallParameters()
        {
            Assert.AreEqual(
                @"{{MyTemplate | InfoC=Baz | InfoA=foo | InfoB=Bar
}}",
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
                )
            );
        }
    }

    [TestFixture]
    public class HumanCatKeyTests : RequiresInitialization
    {
        [Test]
        public void MakeHumanCatKeyOneWordNames()
        {
            Assert.AreEqual("OneWordName", Tools.MakeHumanCatKey("OneWordName", ""));
            Assert.AreEqual("ONEWORDNAME", Tools.MakeHumanCatKey("ONEWORDNAME", ""));
            Assert.AreEqual("Onewordname", Tools.MakeHumanCatKey("Onewordname", ""));
            Assert.AreEqual("onewordname", Tools.MakeHumanCatKey("onewordname", ""));

            Assert.IsTrue(Tools.MakeHumanCatKey(@"Friends of the Mission Clinic of Our Lady of Guadalupe, Inc.", "Test").Length > 0);
        }

        [Test]
        public void MakeHumanCatKeyPersonOfPlace()
        {
            Assert.AreEqual("Foo of London", Tools.MakeHumanCatKey("Foo of London", ""));
            Assert.AreEqual("Foe of London", Tools.MakeHumanCatKey("Foé of London", ""));
            Assert.AreEqual("Foo 02 of London", Tools.MakeHumanCatKey("Foo II of London", ""));
            Assert.AreEqual("Foo 11 of London", Tools.MakeHumanCatKey("Foo XI of London", ""));

            Assert.AreEqual("Clinoch of Alt Clut", Tools.MakeHumanCatKey("Clinoch of Alt Clut", ""), "Person of place with two-word place name");
            Assert.AreEqual("Byzantine Master of the Crucifix of Pisa", Tools.MakeHumanCatKey("Byzantine Master of the Crucifix of Pisa", ""));
        }

        [Test]
        public void MakeHumanCatKeyWithRomanNumbers()
        {
            Assert.AreEqual("Doe, John III", Tools.MakeHumanCatKey("John Doe III", ""));
            Assert.AreEqual("John III", Tools.MakeHumanCatKey("John III", ""));
            Assert.AreEqual("XVII", Tools.MakeHumanCatKey("XVII", ""));
            Assert.AreEqual("Spain, John Doe King of III", Tools.MakeHumanCatKey("John Doe King of Spain III", ""));
        }

        [Test]
        public void MakeHumanCatKeyWithJrSr()
        {
            Assert.AreEqual("Doe, John Jr.", Tools.MakeHumanCatKey("John Doe Jr.", ""));
            Assert.AreEqual("Doe, John Sr.", Tools.MakeHumanCatKey("John Doe Sr.", ""));
            Assert.AreEqual("Doe, John Jnr.", Tools.MakeHumanCatKey("John Doe Jnr.", ""));
            Assert.AreEqual("Doe, John Snr.", Tools.MakeHumanCatKey("John Doe Snr.", ""));
            Assert.AreEqual("Morgan, Carey Elmore Jr.", Tools.MakeHumanCatKey(@"Carey Elmore Morgan Jr.", ""));

            Assert.AreEqual("Doe, John Snr.", Tools.MakeHumanCatKey("John Doe Snr.", ""));
            Assert.AreEqual("Hickham, Steven A. Jr.", Tools.MakeHumanCatKey("Steven A. Hickham Jr.", ""));
            Assert.AreEqual("Hickham, Steven A. Jnr.", Tools.MakeHumanCatKey("Steven A. Hickham Jnr.", ""));
            Assert.AreEqual("Hickham, Steven Jr.", Tools.MakeHumanCatKey("Steven Hickham Jr.", ""));
        }

        [Test]
        public void MakeHumanCatKeyWithApostrophes()
        {
            Assert.AreEqual("DDoe, John", Tools.MakeHumanCatKey("J'ohn D'Doe", ""));
            Assert.AreEqual("Test", Tools.MakeHumanCatKey("'Test", ""));
            Assert.AreEqual("ODonnell, Lillian", Tools.MakeHumanCatKey("Lillian O’Donnell", ""));
            Assert.AreEqual("Word", Tools.MakeHumanCatKey(", Word", ""));
        }

        [Test]
        public void MakeHumanCatKeyWithPrefixes()
        {
            Assert.AreEqual("Doe, John de", Tools.MakeHumanCatKey("John de Doe", ""));
        }

        [Test]
        public void MakeHumanCatKeyDiacritics()
        {
            Assert.AreEqual("Doe", Tools.MakeHumanCatKey("Ďöê", ""));
            Assert.AreEqual("Doe, John", Tools.MakeHumanCatKey("Ĵǒħń Ďöê", ""));

            Assert.AreEqual(@"Gu, Prince Imperial Hoeun", Tools.MakeHumanCatKey("Gu, Prince Imperial Hoeun", ""));

            Assert.AreEqual("Eпрcтии", Tools.MakeHumanCatKey("Ёпрстий", ""));
        }

        [Test]
        public void MakeHumanCatKeyArabicNames()
        {
            Assert.AreEqual(@"Ahmed Mohammed Mukit", Tools.MakeHumanCatKey(@"Ahmed Mohammed Mukit", ""), "no change");
            Assert.AreEqual(@"AHMED Mohammed MUKIT", Tools.MakeHumanCatKey(@"AHMED Mohammed MUKIT", ""), "no change");
            Assert.AreEqual(@"ahmed Mohammed mukit", Tools.MakeHumanCatKey(@"ahmed Mohammed mukit", ""), "no change");

            Assert.AreEqual(@"Smith, John", Tools.MakeHumanCatKey(@"John Smith", ""));
        }

        [Test]
        public void MakeHumanCatKeyMcName()
        {
            Assert.AreEqual(@"McSmith, John", Tools.MakeHumanCatKey(@"John McSmith", ""));
            Assert.AreEqual(@"MacSmith, John", Tools.MakeHumanCatKey(@"John MacSmith", ""));

            Assert.AreEqual(@"Mcsmith, John", Tools.MakeHumanCatKey(@"John Mcsmith", ""));

            Assert.AreEqual(@"Smith, John", Tools.MakeHumanCatKey(@"John Smith", ""));
            Assert.AreEqual(@"Macintosh, John", Tools.MakeHumanCatKey(@"John Macintosh", ""));
        }

        [Test]
        public void MakeHumanCatKeyFamilyName()
        {
            Assert.AreEqual(@"Kong Qingdong", Tools.MakeHumanCatKey(@"Kong Qingdong", "{{Chinese name}}"));
            Assert.AreEqual(@"Qingdong, Kong", Tools.MakeHumanCatKey(@"Kong Qingdong", "{{foo}"));
        }

        [Test]
        public void RemoveDiacritics()
        {
            foreach (var p in Tools.Diacritics)
            {
                Assert.AreEqual(p[1], Tools.RemoveDiacritics(p[0]));
            }

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#Leaving_foreign_characters_in_DEFAULTSORT
            Assert.AreEqual(@"aaaaaa", Tools.RemoveDiacritics(@"ầắạảằẩ"), "a");
            Assert.AreEqual(@"ccccc", Tools.RemoveDiacritics(@"ḉćĉçċ"), "c");
            Assert.AreEqual(@"eeee", Tools.RemoveDiacritics(@"ḕềẹĕ"), "e");
            Assert.AreEqual(@"hh", Tools.RemoveDiacritics(@"ĥḫ"), "h");
            Assert.AreEqual(@"iiiii", Tools.RemoveDiacritics(@"ịỉíįí"), "i");
            Assert.AreEqual(@"I", Tools.RemoveDiacritics(@"İ"), "I");
            Assert.AreEqual(@"oooooooooooo", Tools.RemoveDiacritics(@"òỏøờồȱȯȭȫoỗơ"), "o");
            Assert.AreEqual(@"O", Tools.RemoveDiacritics(@"Ø"), "null");
            Assert.AreEqual(@"s", Tools.RemoveDiacritics(@"š"), "s");
            Assert.AreEqual(@"uuuu", Tools.RemoveDiacritics(@"ụủữự"), "u");
            Assert.AreEqual(@"x", Tools.RemoveDiacritics(@"x̌"), "x");
            Assert.AreEqual(@"yy", Tools.RemoveDiacritics(@"ỳỵ"), "y");
            Assert.AreEqual(@"d p S", Tools.RemoveDiacritics(@"ḏ p̄ Ś̄"), "Random");
            Assert.AreEqual(@"2", Tools.RemoveDiacritics(@"²"), "2");
            Assert.AreEqual(@"Ae", Tools.RemoveDiacritics(@"Ǣ"));
            Assert.AreEqual(@"ae", Tools.RemoveDiacritics(@"ǣ"));
            Assert.AreEqual(@"AaBbCcDdEeFfGgHhIiMmNnOoPpRrSsTtWwXxYyZz", Tools.RemoveDiacritics(@"ȦȧḂḃĊċḊḋĖėḞḟĠġḢḣİıṀṁṄṅȮȯṖṗṘṙṠṡṪṫẆẇẊẋẎẏŻż"), "letters with dot above sign");
            Assert.AreEqual(@"AaAaAaCcCcCcCcDdDdEeEeEeEeEeGgGgGgGgHhHhIiIiIiIiIiJjKkkLlLlLlLlLlNnNnNnnNnOoOoOoRrRrRr", Tools.RemoveDiacritics(@"ĀāĂăĄąĆćĈĉĊċČčĎďĐđĒēĔĕĖėĘęĚěĜĝĞğĠġĢģĤĥĦħĨĩĪīĬĭĮįİıĴĵĶķĸĹĺĻļĽľĿŀŁłŃńŅņŇňŉŊŋŌōŎŏŐőŔŕŖŗŘř"), "extended Latin-A part 1");
            Assert.AreEqual(@"SsSsSsSsTtTtTtUuUuUuUuUuUuWwYyYZzZzZzs", Tools.RemoveDiacritics(@"ŚśŜŝŞşŠšŢţŤťŦŧŨũŪūŬŭŮůŰűŲųŴŵŶŷŸŹźŻżŽžſ"), "extended Latin-A part 2");
            Assert.AreEqual(@"AaIiOoUuUuUuUuUueAaAaGgGgKkOoOoNnAaOoAaAaEeEeIiIiOoOoRrRrUuUuSsTt", Tools.RemoveDiacritics(@"ǍǎǏǐǑǒǓǔǕǖǗǘǙǚǛǜǝǞǟǠǡǤǥǦǧǨǩǪǫǬǭǸǹǺǻǾǿȀȁȂȃȄȅȆȇȈȉȊȋȌȍȎȏȐȑȒȓȔȕȖȗȘșȚț"), "extended Latin-B");
            Assert.AreEqual(@"AaBbBbBbCcDdDdDdDdDdEeEeEeEeEeFfGgHhHhHhHhHhIiIiKkKkKkLlLlLlLlMmMmMmNnNnNnNn", Tools.RemoveDiacritics(@"ḀḁḂḃḄḅḆḇḈḉḊḋḌḍḎḏḐḑḒḓḔḕḖḗḘḙḚḛḜḝḞḟḠḡḢḣḤḥḦḧḨḩḪḫḬḭḮḯḰḱḲḳḴḵḶḷḸḹḺḻḼḽḾḿṀṁṂṃṄṅṆṇṈṉṊṋ"), "Latin Extended Additional A-N");
            }

        [Test]
        public void CleanSortKey()
        {
            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_20#Not_replacing_.26_with_.22and.22_in_sort_values
            Assert.AreEqual(@"and", Tools.CleanSortKey(@"&"), "per SORTKEY");
            Assert.AreEqual(@"A and B", Tools.CleanSortKey(@"A & B"), "&");
            Assert.AreEqual(@"Ai-Ais", Tools.CleanSortKey(@"ǀAi-ǀAis"), "removes weird character");
            Assert.AreEqual(@"Ai-Ais Richtersveld Transfrontier Park", Tools.CleanSortKey(@"ǀAi-ǀAis/Richtersveld Transfrontier Park"), "removes weird character");
            Assert.AreEqual(@"Der Nachtkurier meldet...", Tools.CleanSortKey(@"Der Nachtkurier meldet…"), "replaces …");
            Assert.AreEqual(@"A L", Tools.CleanSortKey(@"A·L"), "replaces · with space");
            Assert.AreEqual(@"Bb, Dd, h, Kk, Ll, Nn, Rr, Tt", Tools.CleanSortKey(@"Ḇḇ, Ḏḏ, ẖ, Ḵḵ, Ḻḻ, Ṉṉ, Ṟṟ, Ṯṯ"), "letters with macron below");
            Assert.AreEqual(@"b, c, d, f, g, h, k, n, p, q, t, v, w, y", Tools.CleanSortKey(@"ɓ, ƈ, ɗ, ƒ, ɠ, ɦ, ƙ, ɲ, ƥ, ʠ, ƭ, ʋ, ⱳ, ƴ"), "letters with hook");
            Assert.AreEqual(@"b, d, f, g, k, l, m, n, p, r, s, t, y, x, z", Tools.CleanSortKey(@"ᶀ, ᶁ, ᶂ, ᶃ, ᶄ, ᶅ, ᶆ, ᶇ, ᶈ, ᶉ, ᶊ, ƫ, ᶌ, ᶍ, ᶎ"), "letters with palatal hook");
            Assert.AreEqual(@"Aa, Ee, Ii, Oo, Uu", Tools.CleanSortKey(@"Ąą, Ęę, Įį, Ǫǫ, Ųų"), "letters using ogonek sign");
            Assert.AreEqual(@"Aa, Ee, Ii, Oo, Uu, Rr", Tools.CleanSortKey(@"Ȃȃ, Ȇȇ, Ȋȋ, Ȏȏ, Ȗȗ, Ȓȓ"), "letters using inverted breve");
            Assert.AreEqual(@"Dd, Ss, Tt", Tools.CleanSortKey(@"D̦d̦, Șș, Țț"), "letters using comma sign");
            Assert.AreEqual(@"'''''''", Tools.CleanSortKey(@"’‘ʻ`´“”"), "quotes");
            Assert.AreEqual(@"1-2-3", Tools.CleanSortKey(@"1–2–3"), "endash");
            Assert.AreEqual(@"1-2-3", Tools.CleanSortKey(@"1–2&ndash;3"), "&ndash;");
            Assert.AreEqual(@"A ", Tools.CleanSortKey(@"A       "), "Excess whitespace");
            Assert.AreEqual(@"A B", Tools.CleanSortKey(@"A/B"), "Forward slash");
            Assert.AreEqual("{{<noinclude>BASE</noinclude>PAGENAME}}", Tools.CleanSortKey("{{<noinclude>BASE</noinclude>PAGENAME}}"), "noinclude tags, no change");
        }

        [Test]
        public void CleanSortKeyLang()
        {
#if DEBUG
            Variables.UnicodeCategoryCollation = true;
            Variables.SetProjectLangCode("ru");
            Assert.AreEqual("Hellõ", Tools.CleanSortKey("Hellõ"), "no diacritic removal for defaultsort key on ru-wiki");
            Variables.SetProjectLangCode("fr");
            Assert.AreEqual("Hellõ", Tools.CleanSortKey("Hellõ"), "no diacritic removal for defaultsort key on fr-wiki");
            Variables.SetProjectLangCode("pl");
            Assert.AreEqual("Hellõ", Tools.CleanSortKey("Hellõ"), "no diacritic removal for defaultsort key on pl-wiki");
            Variables.UnicodeCategoryCollation = false;
            Variables.SetProjectLangCode("en");
            Assert.AreEqual("Hello", Tools.FixupDefaultSort("Hellõ"), "do remove diacritics on en-wiki");
#endif
        }

        [Test]
        public void HasDiacritics()
        {
            Assert.IsTrue(Tools.HasDiacritics("hellõ"));
            Assert.IsTrue(Tools.HasDiacritics("hellõ there"));
            Assert.IsTrue(Tools.HasDiacritics("hẽllõ there"));
            Assert.IsTrue(Tools.HasDiacritics("hẽllo there"));
            Assert.IsTrue(Tools.HasDiacritics("İzmir"));

            Assert.IsFalse(Tools.HasDiacritics("hello"));
            Assert.IsFalse(Tools.HasDiacritics("abcdefghijklmnopqrstuvwxyz"), "standard Latin alphabet");
            Assert.IsFalse(Tools.HasDiacritics("0123456789"), "digits");
            Assert.IsFalse(Tools.HasDiacritics(""), "empty string");
            Assert.IsFalse(Tools.HasDiacritics("   "), "whitespace");
        }

        [Test]
        public void FixUpDefaultSortTests()
        {
            Assert.AreEqual("hello", Tools.FixupDefaultSort("hellõ"));
            Assert.AreEqual("hello", Tools.FixupDefaultSort("hellõ   "));
            Assert.AreEqual(@"fred smithson", Tools.FixupDefaultSort(@"fred smithson"));
            Assert.AreEqual(@"De Merino, Fernando Arturo", Tools.FixupDefaultSort(@"De Meriño, Fernando Arturo"));
            Assert.AreEqual(@"OneWordItem", Tools.FixupDefaultSort(@"OneWordItem"));
            Assert.AreEqual(@"Foo (bar)", Tools.FixupDefaultSort(@"Foo (bar)"));

            Assert.AreEqual("Kwakwaka'wakw mythology", Tools.FixupDefaultSort("Kwakwaka'wakw mythology"));
            Assert.AreEqual(@"Peewee's Playhouse", Tools.FixupDefaultSort(@"Peewee's Playhouse"));
            Assert.AreEqual(@"Peewee's Playhouse", Tools.FixupDefaultSort(@"Peewee’s Playhouse"));
            Assert.AreEqual(@"2010 ITF Women's Circuit (July-September)", Tools.FixupDefaultSort(@"2010 ITF Women's Circuit (July–September)"));
        }

        [Test]
        public void FixUpDefaultSortHumanName()
        {
            Assert.AreEqual(@"Oconnor, Fred", Tools.FixupDefaultSort(@"O'connor, Fred", true), "apostrophe removed for bio sortkey per [[WP:MCSTJR]]");
            Assert.AreEqual(@"O'connor Trading", Tools.FixupDefaultSort(@"O'connor Trading", false), "apostrophes not removed on non-bio sortkey");
            Assert.AreEqual(@"Jones, Fred", Tools.FixupDefaultSort(@"Jones,Fred", true), "comma spacing");
            Assert.AreEqual(@"Jones, Fred", Tools.FixupDefaultSort(@"Jones ,Fred", true), "comma spacing");
            Assert.AreEqual(@"Jones, Fred", Tools.FixupDefaultSort(@"Jones, Fred", true), "comma spacing: no change if already correct");
        }

        [Test]
        public void FixUpDefaultSortBadChars()
        {
            Assert.AreEqual(@"fred smitH", Tools.FixupDefaultSort(@"fred smitHº"));
            Assert.AreEqual(@"fred smitH", Tools.FixupDefaultSort(@"fred ""smitH"""));
            Assert.AreEqual(@"Fred Smith", Tools.FixupDefaultSort(@"Fred ""Smith"""));
        }

        [Test]
        public void FixUpDefaultSortTestsRu()
        {
#if DEBUG
            Variables.SetProjectLangCode("ru");
            Variables.UnicodeCategoryCollation = true;
            Assert.AreEqual("Hellõ", Tools.FixupDefaultSort("Hellõ"), "no diacritic removal for defaultsort key on ru-wiki");
            
            Variables.SetProjectLangCode("en");
            Variables.UnicodeCategoryCollation = false;
            Assert.AreEqual("Hello", Tools.FixupDefaultSort("Hellõ"));
#endif
        }

        [Test]
        public void RemoveNamespace()
        {
            Assert.AreEqual("Doe, John", Tools.MakeHumanCatKey("Wikipedia:John Doe", ""));
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

                Assert.IsFalse(name.Contains("  "), "Sorting key shouldn't contain consecutive spaces - it breaks the sorting ({0})", name);
                Assert.IsFalse(name.StartsWith(" "), "Sorting key shouldn't start with spaces");
                Assert.IsFalse(name.EndsWith(" "), "Sorting key shouldn't end with spaces");
            }
        }

        [Test]
        public void RomanToInt()
        {
            Assert.AreEqual("01", Tools.RomanToInt("I"));
            Assert.AreEqual("02", Tools.RomanToInt("II"));
            Assert.AreEqual("03", Tools.RomanToInt("III"));
            Assert.AreEqual("04", Tools.RomanToInt("IV"));
            Assert.AreEqual("05", Tools.RomanToInt("V"));
            Assert.AreEqual("06", Tools.RomanToInt("VI"));
            Assert.AreEqual("07", Tools.RomanToInt("VII"));
            Assert.AreEqual("08", Tools.RomanToInt("VIII"));
            Assert.AreEqual("09", Tools.RomanToInt("IX"));
            Assert.AreEqual("10", Tools.RomanToInt("X"));
            Assert.AreEqual("11", Tools.RomanToInt("XI"));
            Assert.AreEqual("14", Tools.RomanToInt("XIV"));
            Assert.AreEqual("16", Tools.RomanToInt("XVI"));
            Assert.AreEqual("26", Tools.RomanToInt("XXVI"));
            Assert.AreEqual("76", Tools.RomanToInt("LXXVI"));
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
            Assert.AreEqual("Talk:Foo", Tools.ConvertToTalk("Foo"));
            Assert.AreEqual("Talk:Foo bar", ToTalk("Foo bar"));
            Assert.AreEqual("Talk:Foo:Bar", ToTalk("Foo:Bar"));
            Assert.AreEqual("Wikipedia talk:Foo", ToTalk("Wikipedia:Foo"));
            Assert.AreEqual("File talk:Foo bar", ToTalk("File:Foo bar"));
            Assert.AreEqual("File talk:Foo bar", ToTalk("File talk:Foo bar"));

            // Don't choke on special namespaces
            Assert.AreEqual("Special:Foo", ToTalk("Special:Foo"));
            Assert.AreEqual("Media:Bar", ToTalk("Media:Bar"));

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
            CollectionAssert.AreEquivalent(Tools.ConvertToTalk(l),
                                           new[] { "Talk:Foo", "Talk:Foo bar", "File talk:Foo", "Special:Foo" });
        }

        [Test]
        public void ConvertFromTalk()
        {
            Assert.AreEqual("Foo", FromTalk("Talk:Foo"));
            Assert.AreEqual("Foo", Tools.ConvertFromTalk("Talk:Foo"));
            Assert.AreEqual("Foo", FromTalk("Foo"));
            Assert.AreEqual("Foo:Bar", FromTalk("Foo:Bar"));
            Assert.AreEqual("Foo:Bar", FromTalk("Talk:Foo:Bar"));
            Assert.AreEqual("User:Foo bar", FromTalk("User:Foo bar"));
            Assert.AreEqual("File:Bar", FromTalk("File talk:Bar"));
            // Assert.AreEqual("File:Bar", FromTalk("Image talk:Bar"),"it bypasses redirects to file namespace");
            Assert.AreEqual("Template:Bar", FromTalk("Template talk:Bar"));
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
