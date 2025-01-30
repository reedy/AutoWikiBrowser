﻿using System.Text;
using NUnit.Framework;
using WikiFunctions;

namespace UnitTests
{
    [TestFixture]
    public class SummaryTests : RequiresInitialization
    {
        [Test]
        public void IsCorrect()
        {
            // too long
            StringBuilder sb = new StringBuilder(300);
            for (int i = 0; i < 256; i++) sb.Append('x');
            Assert.IsFalse(Summary.IsCorrect(sb.ToString()));

            // no wikilinks
            Assert.IsTrue(Summary.IsCorrect(""));
            Assert.IsTrue(Summary.IsCorrect("test"));
            Assert.IsTrue(Summary.IsCorrect("["));
            Assert.IsTrue(Summary.IsCorrect("]"));
            Assert.IsTrue(Summary.IsCorrect("[test]"));
            Assert.IsTrue(Summary.IsCorrect("[test]]"));
            Assert.IsTrue(Summary.IsCorrect("[[]]"));

            // correctly (sort of..) terminated wikilinks
            Assert.IsTrue(Summary.IsCorrect("[[test]]"));
            Assert.IsTrue(Summary.IsCorrect("[[test]] [[foo]]"));

            // broken wikilinks, should be found to be invalid
            Assert.IsFalse(Summary.IsCorrect("[["));
            Assert.IsFalse(Summary.IsCorrect("[[["));
            Assert.IsFalse(Summary.IsCorrect("[[test]"));
            Assert.IsFalse(Summary.IsCorrect("[[test]] [["));
            Assert.IsFalse(Summary.IsCorrect("[[123456789 123456789 123456789 1[[WP:AWB]]"));
            Assert.IsFalse(Summary.IsCorrect("[[foo[[]]]"));
        }

        [Test]
        public void Trim()
        {
            Assert.That(Summary.Trim("test"), Is.EqualTo("test"));

            const string bug1 =
                @"replaced category 'Actual event ballads' → 'Songs based on actual events' per [[Wikipedia:Categories for discussion/Log/2009 November 6|CfD 2009 Nov 6]]";
            const string waffle = @"some waffle here to make the edit summary too long";

            Assert.That(Summary.Trim(bug1), Is.EqualTo(bug1));
            Assert.That(Summary.Trim(waffle + bug1), Is.EqualTo(waffle + bug1));
            Assert.That(
                Summary.Trim(waffle + waffle + bug1),
                Is.EqualTo(waffle + waffle + @"replaced category 'Actual event ballads' → 'Songs based on actual events' per..."));
        }

        [Test]
        public void ModifiedSection()
        {
            Assert.That(Summary.ModifiedSection("", ""), Is.Empty, "No change to text");
            Assert.That(Summary.ModifiedSection("foo", "foo"), Is.Empty, "No change to text 2");
            Assert.That(Summary.ModifiedSection("foo", "bar"), Is.EqualTo("top"), "Zeroth section change, no other sections");
            Assert.That(Summary.ModifiedSection("foo\r\n==sec==\r\na", "bar\r\n==sec==\r\na"), Is.EqualTo("top"), "Zeroth section change");

            Assert.That(Summary.ModifiedSection("==foo==\r\n123", "==foo==\r\ntest"), Is.EqualTo("foo"), "Section 1 change, no zeroth section");
            Assert.That(Summary.ModifiedSection("rawr\r\n==foo==\r\n123", "rawr\r\n==foo==\r\ntest"), Is.EqualTo("foo"));
            Assert.That(Summary.ModifiedSection("==foo==\r\n123", "== foo ==\r\ntest"), Is.EqualTo("foo"));
            Assert.That(Summary.ModifiedSection("==foo==\r\n123", "==bar==\r\ntest"), Is.EqualTo("bar"));

            // https://en.wikipedia.org/w/index.php?diff=prev&oldid=338360216
            Assert.That(Summary.ModifiedSection("==foo==\r\nbar", "test\r\n==foo==\r\nbar"), Is.Empty);
        }
    }
}
