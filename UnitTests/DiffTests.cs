using System.Collections.Generic;
using WikiFunctions;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace UnitTests
{
    [TestFixture]
    public class DiffTests
    {
        private static void AssertWords(IList<Word> words, params string[] expected)
        {
            Assert.AreEqual(expected.Length, words.Count);
            for (int i = 0; i < expected.Length; i++)
                Assert.AreEqual(expected[i], words[i].ToString());
        }

        [Test]
        public void WordTests()
        {
            Word w1 = new Word("", "");
            Word w2 = new Word("", " ");

            Assert.AreEqual("", w1.TheWord);
            Assert.AreEqual("", w1.Whitespace);
            Assert.AreEqual("", w1.ToString());
            Assert.AreEqual("", w2.TheWord);
            Assert.AreEqual(" ", w2.Whitespace);
            Assert.AreEqual(" ", w2.ToString());
            Assert.AreEqual(w1, w2);
            Assert.AreEqual(w1.GetHashCode(), w2.GetHashCode());

            w1 = new Word("foo", "  ");
            w2 = new Word("foo", "");
            Assert.AreEqual("foo", w1.TheWord);
            Assert.AreEqual("foo  ", w1.ToString());
            Assert.AreEqual("foo", w2.ToString());
            Assert.AreEqual(w1, w2);
            Assert.AreEqual(w1.GetHashCode(), w2.GetHashCode());

            w2 = new Word("Foo", "");
            Assert.AreNotEqual(w1, w2);
            Assert.AreNotEqual(w1.GetHashCode(), w2.GetHashCode());
        }

        [Test]
        public void SplitString()
        {
            CollectionAssert.IsEmpty(Word.SplitString(""));

            AssertWords(Word.SplitString(" "), " ");
            AssertWords(Word.SplitString("   "), "   ");

            List<Word> lst = Word.SplitString("foo");
            Assert.AreEqual(1, lst.Count);
            CollectionAssert.AreEqual(Word.SplitString("foo "), lst);

            lst = Word.SplitString("foo bar");
            AssertWords(lst, "foo ", "bar");
            CollectionAssert.AreEqual(Word.SplitString("foo    bar "), lst);
            CollectionAssert.AreNotEqual(Word.SplitString(" foo bar"), lst);

            lst = Word.SplitString("foo.bar");
            AssertWords(lst, "foo", ".", "bar");

            lst = Word.SplitString("foo..bar");
            AssertWords(lst, "foo", ".", ".", "bar");

            lst = Word.SplitString("{{foo}}");
            AssertWords(lst, "{", "{", "foo", "}", "}");

            lst = Word.SplitString("foo|bar");
            AssertWords(lst, "foo", "|", "bar");
            
            lst = Word.SplitString("foé bar");
            AssertWords(lst, "foé ", "bar");
            
            // Chinese – each character is defined as a word
            lst = Word.SplitString("汉语");
            AssertWords(lst, "汉", "语");
            
            // Thai – each character is defined as a word
            lst = Word.SplitString("ภา");
            AssertWords(lst, "ภ", "า");
        }

        [Test]
        public void UndoDeletion()
        {
            WikiDiff d = new WikiDiff();

            d.GetDiff(@"
A1
A2", @"A1
A2", 2);

            Assert.AreEqual(@"
A1
A2", d.UndoDeletion(0, 0), "Undo of delete first blank line");

d = new WikiDiff();

            d.GetDiff(@"A1
A2
", @"A1
A2", 2);

            Assert.AreEqual(@"A1
A2
", d.UndoDeletion(2, 2), "Undo of delete last blank line");
        }


        [Test]
        public void DiffHTML()
        {
            WikiDiff d = new WikiDiff();

            string diffResult = d.GetDiff(@"A1
A2", @"A1
A2", 2);

            Assert.AreEqual(@"", diffResult, "No HTML when no change");

            diffResult = d.GetDiff(@"A1
A2", @"A1X
A2", 2).Replace(System.Environment.NewLine, " ");

            diffResult = Regex.Replace(diffResult, @"\s+", " ");

            Assert.AreEqual(@"<tr onclick='window.external.GoTo(0)'> <td colspan='2' class='diff-lineno'>Line 1:</td> <td colspan='2' class='diff-lineno'>Line 1:</td> </tr><tr onclick='window.external.GoTo(0)' ondblclick='window.external.UndoChange(0,0)'> <td>-</td> <td class='diff-deletedline'><span class='diffchange'>A1</span> </td> <td>+</td> <td class='diff-addedline'><span class='diffchange'>A1X</span> </td> </tr><tr onclick='window.external.GoTo(1);'> <td class='diff-marker'> </td> <td class='diff-context'>A2</td> <td class='diff-marker'> </td> <td class='diff-context'>A2</td> </tr>", diffResult, "Standard case: first line changed");

            diffResult = d.GetDiff(@"A1
A2", @"A1X
A2X", 2).Replace(System.Environment.NewLine, " ");

            diffResult = Regex.Replace(diffResult, @"\s+", " ");

            Assert.AreEqual(@"<tr onclick='window.external.GoTo(0)'> <td colspan='2' class='diff-lineno'>Line 1:</td> <td colspan='2' class='diff-lineno'>Line 1:</td> </tr><tr onclick='window.external.GoTo(0)' ondblclick='window.external.UndoChange(0,0)'> <td>-</td> <td class='diff-deletedline'><span class='diffchange'>A1</span> </td> <td>+</td> <td class='diff-addedline'><span class='diffchange'>A1X</span> </td> </tr><tr onclick='window.external.GoTo(1)' ondblclick='window.external.UndoChange(1,1)'> <td>-</td> <td class='diff-deletedline'><span class='diffchange'>A2</span> </td> <td>+</td> <td class='diff-addedline'><span class='diffchange'>A2X</span> </td> </tr>", diffResult, "Standard case: two lines changed");

            diffResult = d.GetDiff(@"A1
A2", @"A1", 2).Replace(System.Environment.NewLine, " ");

            diffResult = Regex.Replace(diffResult, @"\s+", " ");

            Assert.AreEqual(@"<tr onclick='window.external.GoTo(0);'> <td class='diff-marker'> </td> <td class='diff-context'>A1</td> <td class='diff-marker'> </td> <td class='diff-context'>A1</td> </tr><tr> <td>-</td> <td class='diff-deletedline' onclick='window.external.GoTo(1)' ondblclick='window.external.UndoDeletion(1, 1)'>A2 </td> <td> </td> <td> </td> </tr>", diffResult, "Standard case: second line deleted");
        }
    }
}
