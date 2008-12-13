using WikiFunctions;
using WikiFunctions.Parse;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class HideTextTests
    {
        public HideTextTests()
        {
            Globals.UnitTestMode = true;
            if (WikiRegexes.Category == null) WikiRegexes.MakeLangSpecificRegexes();
        }

        #region Helpers
        string hidden = @"⌊⌊⌊⌊M?\d+⌋⌋⌋⌋";
        string allHidden = @"^(⌊⌊⌊⌊M?\d+⌋⌋⌋⌋)*$";
        HideText hider;

        private string HideMore(string text)
        {
            return HideMore(text, true, false, true);
        }

        private string HideMore(string text, bool HideExternalLinks, bool LeaveMetaHeadings, bool HideImages)
        {
            hider = new HideText(HideExternalLinks, LeaveMetaHeadings, HideImages);
            return hider.HideMore(text);
        }

        private void AssertHiddenMore(string text)
        {
            RegexAssert.IsMatch(hidden, HideMore(text));
        }

        private void AssertAllHiddenMore(string text)
        {
            string s = HideMore(text);
            RegexAssert.IsMatch(allHidden, s);
            s = hider.AddBackMore(s);
            Assert.AreEqual(text, s);
        }

        private string Hide(string text)
        {
            return Hide(text, true, false, true);
        }

        private string Hide(string text, bool HideExternalLinks, bool LeaveMetaHeadings, bool HideImages)
        {
            hider = new HideText(HideExternalLinks, LeaveMetaHeadings, HideImages);
            return hider.Hide(text);
        }

        private void AssertHidden(string text)
        {
            RegexAssert.IsMatch(hidden, Hide(text));
        }

        private void AssertAllHidden(string text)
        {
            string s = Hide(text);
            RegexAssert.IsMatch(allHidden, s);
            s = hider.AddBack(s);
            Assert.AreEqual(text, s);
        }
        private void AssertBothHidden(string text)
        {
            AssertBothHidden(text, true, false, true);
        }

        private void AssertBothHidden(string text, bool HideExternalLinks, bool LeaveMetaHeadings, bool HideImages)
        {
            AssertAllHidden(text);
            AssertAllHiddenMore(text);
        }

        #endregion

        [Test]
        public void AcceptEmptyStrings()
        {
            Assert.AreEqual("", Hide(""));
            Assert.AreEqual("", HideMore(""));
        }

        [Test]
        public void HideTemplates()
        {
            AssertAllHiddenMore("{{foo}}");
            AssertAllHiddenMore("{{foo|}}");
            AssertAllHiddenMore("{{foo|bar}}");
            RegexAssert.IsMatch("123" + hidden + "123", HideMore("123{{foo}}123"));
            AssertAllHiddenMore("{{foo|{{bar}}}}");
            AssertAllHiddenMore("{{foo|{{bar|{{{1|}}}}}}}");
            AssertAllHiddenMore("{{foo|\r\nbar= {blah} blah}}");
            AssertAllHiddenMore("{{foo|\r\nbar= {blah} {{{1|{{blah}}}}}}}");

            RegexAssert.IsMatch(@"\{"+hidden+@"\}", HideMore("{{{foo}}}"));
        }

        [Test]
        public void HideImages()
        {
            AssertAllHidden("[[Image:foo]]");
            AssertAllHidden("[[Image:foo|100px|bar]]");
            AssertAllHidden("[[Image:foo|A [[bar]] [http://boz.com gee].]]");
            AssertAllHidden("[[Image:foo|A [[bar]] [[test]].]]");
            AssertAllHidden("[[File:foo|A [[bar]]]]");
            AssertAllHidden("[[Image:foo|A [[bar|quux]].]]");
            AssertAllHidden("[[Image:foo|A [[bar]][http://fubar].]]");
            AssertAllHidden("[[FILE:foo|A [[bar]][http://fubar].{{quux}}]]");
            AssertAllHidden("[[Image:foo|test [[Image:bar|thumb|[[boz]]]]]]");
        }

        [Test]
        public void HideImagesMore()
        {
            AssertAllHiddenMore("[[File:foo]]");
            AssertAllHiddenMore("[[Image:foo|100px|bar]]");
            AssertAllHiddenMore("[[Image:foo|A [[bar]] [http://boz.com gee].]]");
            AssertAllHiddenMore("[[Image:foo|A [[bar]] [[test]].]]");
            AssertAllHiddenMore("[[Image:foo|A [[bar]]]]");
            AssertAllHiddenMore("[[FILE:foo|A [[bar|quux]].]]");
            AssertAllHiddenMore("[[Image:foo|A [[bar]][http://fubar].]]");
            AssertAllHiddenMore("[[Image:foo|A [[bar]][http://fubar].{{quux}}]]");
            AssertAllHiddenMore("[[Image:foo|test [[File:bar|thumb|[[boz]]]]]]");
        }

        [Test]
        public void HideGalleries()
        {
            AssertAllHiddenMore(@"<gallery>
Image:foo|a [[bar]]
Image:quux[http://example.com]
</gallery>");
            AssertAllHiddenMore(@"<gallery name=""test"">
Image:foo|a [[bar]]
Image:quux[http://example.com]
</gallery>");
        }

        [Test]
        public void HideExternalLinks()
        {
            AssertAllHiddenMore("[http://foo]");
            AssertAllHiddenMore("[http://foo bar]");
            AssertAllHiddenMore("[http://foo [bar]");
        }
    }

    [TestFixture]
    public class ArticleTests
    {
        public ArticleTests()
        {
            Globals.UnitTestMode = true;
            if (WikiRegexes.Category == null) WikiRegexes.MakeLangSpecificRegexes();
        }

        [Test]
        public void NamespacelessName()
        {
            Assert.AreEqual("Foo", new Article("Foo").NamespacelessName);
            Assert.AreEqual("Foo", new Article("Category:Foo").NamespacelessName);
            Assert.AreEqual("Category:Foo", new Article("Category:Category:Foo").NamespacelessName);

            // uncomment when Tools.CalculateNS() will support non-normalised names
            //Assert.AreEqual("Foo", new Article("Category : Foo").NamespacelessName);

            Assert.AreEqual("", new Article("Category:").NamespacelessName);
            Assert.AreEqual("", new Article("Category: ").NamespacelessName);
        }
    }
}
