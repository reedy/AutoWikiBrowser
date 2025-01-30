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
            Assert.That(words.Count, Is.EqualTo(expected.Length));
            for (int i = 0; i < expected.Length; i++)
                Assert.That(words[i].ToString(), Is.EqualTo(expected[i]));
        }

        [Test]
        public void WordTests()
        {
            Word w1 = new Word("", "");
            Word w2 = new Word("", " ");

            Assert.That(w1.TheWord, Is.Empty);
            Assert.That(w1.Whitespace, Is.Empty);
            Assert.That(w1.ToString(), Is.Empty);
            Assert.That(w2.TheWord, Is.Empty);
            Assert.That(w2.Whitespace, Is.EqualTo(" "));
            Assert.That(w2.ToString(), Is.EqualTo(" "));
            Assert.That(w2, Is.EqualTo(w1));
            Assert.That(w2.GetHashCode(), Is.EqualTo(w1.GetHashCode()));

            w1 = new Word("foo", "  ");
            w2 = new Word("foo", "");
            Assert.That(w1.TheWord, Is.EqualTo("foo"));
            Assert.That(w1.ToString(), Is.EqualTo("foo  "));
            Assert.That(w2.ToString(), Is.EqualTo("foo"));
            Assert.That(w2, Is.EqualTo(w1));
            Assert.That(w2.GetHashCode(), Is.EqualTo(w1.GetHashCode()));

            w2 = new Word("Foo", "");
            Assert.That(w1, Is.Not.EqualTo(w2));
            Assert.That(w1.GetHashCode(), Is.Not.EqualTo(w2.GetHashCode()));
        }

        [Test]
        public void SplitString()
        {
            Assert.That(Word.SplitString(""), Is.Empty);

            AssertWords(Word.SplitString(" "), " ");
            AssertWords(Word.SplitString("   "), "   ");

            List<Word> lst = Word.SplitString("foo");
            Assert.That(lst.Count, Is.EqualTo(1));
            Assert.That(lst, Is.EqualTo(Word.SplitString("foo ")).AsCollection);

            lst = Word.SplitString("foo bar");
            AssertWords(lst, "foo ", "bar");
            Assert.That(lst, Is.EqualTo(Word.SplitString("foo    bar ")).AsCollection);
            Assert.That(lst, Is.Not.EqualTo(Word.SplitString(" foo bar")).AsCollection);

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

            Assert.That(d.UndoDeletion(0, 0), Is.EqualTo(@"
A1
A2"), "Undo of delete first blank line");

d = new WikiDiff();

            d.GetDiff(@"A1
A2
", @"A1
A2", 2);

            Assert.That(d.UndoDeletion(2, 2), Is.EqualTo(@"A1
A2
"), "Undo of delete last blank line");
        }


        [Test]
        public void DiffHTML()
        {
            WikiDiff d = new WikiDiff();

            string diffResult = d.GetDiff(@"A1
A2", @"A1
A2", 2);

            Assert.That(diffResult, Is.Empty, "No HTML when no change");

            diffResult = d.GetDiff(@"A1
A2", @"A1X
A2", 2).Replace(System.Environment.NewLine, " ");

            diffResult = Regex.Replace(diffResult, @"\s+", " ");

            Assert.That(diffResult, Is.EqualTo(@"<tr onclick='window.external.GoTo(0)'> <td colspan='2' class='diff-lineno'>Line 1:</td> <td colspan='2' class='diff-lineno'>Line 1:</td> </tr><tr onclick='window.external.GoTo(0)' ondblclick='window.external.UndoChange(0,0)'> <td>-</td> <td class='diff-deletedline'><span class='diffchange'>A1</span> </td> <td>+</td> <td class='diff-addedline'><span class='diffchange'>A1X</span> </td> </tr><tr onclick='window.external.GoTo(1);'> <td class='diff-marker'> </td> <td class='diff-context'>A2</td> <td class='diff-marker'> </td> <td class='diff-context'>A2</td> </tr>"), "Standard case: first line changed");

            diffResult = d.GetDiff(@"A1
A2", @"A1X
A2X", 2).Replace(System.Environment.NewLine, " ");

            diffResult = Regex.Replace(diffResult, @"\s+", " ");

            Assert.That(diffResult, Is.EqualTo(@"<tr onclick='window.external.GoTo(0)'> <td colspan='2' class='diff-lineno'>Line 1:</td> <td colspan='2' class='diff-lineno'>Line 1:</td> </tr><tr onclick='window.external.GoTo(0)' ondblclick='window.external.UndoChange(0,0)'> <td>-</td> <td class='diff-deletedline'><span class='diffchange'>A1</span> </td> <td>+</td> <td class='diff-addedline'><span class='diffchange'>A1X</span> </td> </tr><tr onclick='window.external.GoTo(1)' ondblclick='window.external.UndoChange(1,1)'> <td>-</td> <td class='diff-deletedline'><span class='diffchange'>A2</span> </td> <td>+</td> <td class='diff-addedline'><span class='diffchange'>A2X</span> </td> </tr>"), "Standard case: two lines changed");

            diffResult = d.GetDiff(@"A1
A2", @"A1", 2).Replace(System.Environment.NewLine, " ");

            diffResult = Regex.Replace(diffResult, @"\s+", " ");

            Assert.That(diffResult, Is.EqualTo(@"<tr onclick='window.external.GoTo(0);'> <td class='diff-marker'> </td> <td class='diff-context'>A1</td> <td class='diff-marker'> </td> <td class='diff-context'>A1</td> </tr><tr> <td>-</td> <td class='diff-deletedline' onclick='window.external.GoTo(1)' ondblclick='window.external.UndoDeletion(1, 1)'>A2 </td> <td> </td> <td> </td> </tr>"), "Standard case: second line deleted");
        }
    }
}
