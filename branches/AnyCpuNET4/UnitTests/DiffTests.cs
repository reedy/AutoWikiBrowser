using System.Collections.Generic;
using WikiFunctions;
using NUnit.Framework;

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
        }
    }
}
