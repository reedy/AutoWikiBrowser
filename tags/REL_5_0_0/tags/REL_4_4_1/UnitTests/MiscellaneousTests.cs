using System;
using System.Collections.Generic;
using System.Text;
using WikiFunctions;
using WikiFunctions.Parse;
using NUnit.Framework;
using System.Text.RegularExpressions;

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

        string hidden = @"⌊⌊⌊⌊M?\d+⌋⌋⌋⌋";
        string allHidden = @"^(⌊⌊⌊⌊M?\d+⌋⌋⌋⌋)*$";
        HideText hider;

        private string Hide(string text)
        {
            return Hide(text, true, false, true);
        }

        private string Hide(string text, bool HideExternalLinks, bool LeaveMetaHeadings, bool HideImages)
        {
            hider = new HideText(HideExternalLinks, LeaveMetaHeadings, HideImages);
            return hider.HideMore(text);
        }

        private void AssertHidden(string text)
        {
            RegexAssert.IsMatch(hidden, Hide(text));
        }

        private void AssertAllHidden(string text)
        {
            string s = Hide(text);
            RegexAssert.IsMatch(allHidden, s);
            s = hider.AddBackMore(s);
            Assert.AreEqual(text, s);
        }

        [Test]
        public void AcceptEmptyStrings()
        {
            Assert.AreEqual("", Hide(""));
        }

        [Test]
        public void HideTemplates()
        {
            AssertAllHidden("{{foo}}");
            AssertAllHidden("{{foo|}}");
            AssertAllHidden("{{foo|bar}}");
            RegexAssert.IsMatch("123" + hidden + "123", Hide("123{{foo}}123"));
            AssertAllHidden("{{foo|{{bar}}}}");
            AssertAllHidden("{{foo|{{bar|{{{1|}}}}}}}");
            AssertAllHidden("{{foo|\r\nbar= {blah} blah}}");
            AssertAllHidden("{{foo|\r\nbar= {blah} {{{1|{{blah}}}}}}}");

            RegexAssert.IsMatch(@"\{"+hidden+@"\}", Hide("{{{foo}}}"));
        }

        [Test]
        public void HideImages()
        {
            AssertAllHidden("[[Image:foo]]");
            AssertAllHidden("[[Image:foo|100px|bar]]");
            AssertAllHidden("[[Image:foo|A [[bar]] [http://boz.com gee].]]");
            AssertAllHidden("[[Image:foo|A [[bar]] [[test]].]]");
            AssertAllHidden("[[Image:foo|A [[bar]]]]");
            AssertAllHidden("[[Image:foo|A [[bar|quux]].]]");
            AssertAllHidden("[[Image:foo|A [[bar]][http://fubar].]]");
            AssertAllHidden("[[Image:foo|A [[bar]][http://fubar].{{quux}}]]");
            AssertAllHidden("[[Image:foo|test [[Image:bar|thumb]]]]");
        }

        [Test]
        public void HideGalleries()
        {
            AssertAllHidden(@"<gallery>
Image:foo|a [[bar]]
Image:quux[http://example.com]
</gallery>");
            AssertAllHidden(@"<gallery name=""test"">
Image:foo|a [[bar]]
Image:quux[http://example.com]
</gallery>");
        }

        [Test]
        public void HideExternalLinks()
        {
            AssertAllHidden("[http://foo]");
            AssertAllHidden("[http://foo bar]");
            AssertAllHidden("[http://foo [bar]");
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
