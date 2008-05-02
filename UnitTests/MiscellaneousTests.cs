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
            WikiRegexes.MakeLangSpecificRegexes();
        }

        string hidden = @"⌊⌊⌊⌊M?\d+⌋⌋⌋⌋";
        string allHidden = @"^(⌊⌊⌊⌊M?\d+⌋⌋⌋⌋)*$";
        HideText hider;

        private string Hide(string text)
        {
            hider = new HideText();
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
            AssertAllHidden("[[Image:foo|A [[bar]].]]");
            AssertAllHidden("[[Image:foo|A [[bar]] [[test]].]]");
            AssertAllHidden("[[Image:foo|A [[bar]]]]");
            AssertAllHidden("[[Image:foo|A [[bar|quux]].]]");
            AssertAllHidden("[[Image:foo|A [[bar]][http://fubar].]]");
            AssertAllHidden("[[Image:foo|A [[bar]][http://fubar].{{quux}}]]");
        }
    }
}
