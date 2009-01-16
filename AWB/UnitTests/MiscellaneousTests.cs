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
        const string hidden = @"⌊⌊⌊⌊M?\d+⌋⌋⌋⌋",
            allHidden = @"^(⌊⌊⌊⌊M?\d+⌋⌋⌋⌋)*$";
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
            hider = new HideText(HideExternalLinks, LeaveMetaHeadings, HideImages);
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

            RegexAssert.IsMatch(@"\{" + hidden + @"\}", HideMore("{{{foo}}}"));
        }

        const string Caption1 = @"|image_caption=London is a European Parliament constituency. It has water. |",
            Caption2 = @"|image_caption= some load of text here. Some more there.}}",
            Caption3 = @"|image_caption=some load of text here. Some more there.   }}",
            Caption4 = @"|image_caption= some load of text here. Some more there.|",
            Caption5 = @"|image_caption
            = some load of text here. Some more there.
            |",
              Field1 = @"field = value |";

        [Test]
        public void HideImages()
        {
            AssertAllHidden(@"[[File:foo.jpg]]");
            AssertAllHidden(@"[[File:foo with space and 0004.jpg]]");
            AssertAllHidden(@"[[File:foo.jpeg]]");
            AssertAllHidden(@"[[File:foo.JPEG]]");
            AssertAllHidden(@"[[Image:foo with space and 0004.jpeg]]");
            AssertAllHidden(@"[[Image:foo.jpeg]]");
            AssertAllHidden(@"[[Image:foo with space and 0004.jpg]]");
            AssertAllHidden(@"[[File:foo.jpg|");
            AssertAllHidden(@"[[File:foo with space and 0004.jpg|");
            AssertAllHidden(@"[[File:foo.jpeg|");
            AssertAllHidden(@"[[Image:foo with space and 0004.jpeg|");
            AssertAllHidden(@"[[Image:foo.jpeg|");
            AssertAllHidden(@"[[Image:foo with SPACE() and 0004.jpg|");
            AssertAllHidden(@"[[File:foo.gif|");
            AssertAllHidden(@"[[Image:foo with space and 0004.gif|");
            AssertAllHidden(@"[[Image:foo.gif|");
            AssertAllHidden(@"[[Image:foo with SPACE() and 0004.gif|");
            AssertAllHidden(@"[[File:foo.png|");
            AssertAllHidden(@"[[Image:foo with space and 0004.png|");
            AssertAllHidden(@"[[Image:foo_here.png|");
            AssertAllHidden(@"[[Image:foo with SPACE() and 0004.png|");
            AssertAllHidden(@"|image_skyline=442px_-_London_Lead_Image.jpg|");
            AssertAllHidden(@"|image_map=London (European Parliament constituency).svg   |");
            AssertAllHidden(@"[[Image:westminster.tube.station.jubilee.arp.jpg|");
            AssertAllHidden(@"|image_map=westminster.tube.station.jubilee.arp.jpg|");
            AssertAllHidden(@"|Cover  = AmorMexicanaThalia.jpg |");
            AssertAllHidden(@"|image = AmorMexicanaThalia.jpg |");
            AssertAllHidden(@"|
image = AmorMexicanaThalia.jpg |");
            AssertAllHidden(@"|Img = BBC_logo1.jpg 
|");
            AssertAllHidden(@"| image name = Fred Astaire.jpg |");
            AssertAllHidden(@"|image2 = AmorMexicanaThalia.jpg |");
            // in tests below no text is hidden

            Assert.AreEqual(Caption1, Hide(Caption1));
            Assert.AreEqual(Caption2, Hide(Caption2));
            Assert.AreEqual(Caption3, Hide(Caption3));
            Assert.AreEqual(Caption4, Hide(Caption4));
            Assert.AreEqual(Caption5, Hide(Caption5));

            // in tests below part of string is hidden
            Assert.IsTrue(Hide(@"[[Image:some image name.JPG|thumb|words with typos]]").EndsWith(@"thumb|words with typos]]"));
            Assert.IsTrue(Hide(@"[[Image:some image name.JPEG|words with typos]]").EndsWith(@"words with typos]]"));
            Assert.IsTrue(Hide(@"[[Image:some image name.jpg|thumb|words with typos ]]").EndsWith(@"thumb|words with typos ]]"));
            Assert.IsTrue(Hide(@"[[Image:some image name.png|20px|words with typos and [[links]] here]]").EndsWith(@"20px|words with typos and [[links]] here]]"));
            Assert.IsTrue(Hide(@"[[Image:some image name.svg|thumb|words with typos ]]").EndsWith(@"thumb|words with typos ]]"));
            Assert.IsTrue(Hide(@"[[Image:some_image_name.further words.tiff|thumb|words with typos<ref name=a/>]]").EndsWith(@"thumb|words with typos<ref name=a/>]]"));
            Assert.IsTrue(Hide(@"[[Image:some_image_name.for a word.png|thumb|words with typo]]").EndsWith(@"thumb|words with typo]]"));
            Assert.IsTrue(Hide(@"[[Image:some_image_name.png|thumb|words with typo]] Normal words in text").EndsWith(@"thumb|words with typo]] Normal words in text"));
            Assert.IsTrue(Hide(@"[[Image:some_image_name.png]] Normal words in text").EndsWith(@" Normal words in text"));
            Assert.IsTrue(Hide(Caption4 + Field1).EndsWith(Field1));

            // TODO, resolve issue below
            // this test fails at the moment due to two image files in an infobox/template in consecutive fields
            Assert.IsFalse(Hide(@"{{Drugbox|
|IUPAC_name = 6-chloro-1,1-dioxo-2''H''-1,2,4-benzothiadiazine-7-sulfonamide
| image=Chlorothiazide.svg
| image2=Chlorothiazide-from-xtal-3D-balls.png
| CAS_number=58-94-6").Contains("=Chlorothiazide"));


            //following changes to not mask image descriptions, the following old tests are now invalid
            /*
            AssertAllHidden("[[Image:foo]]");
            AssertAllHidden("[[Image:foo|100px|bar]]");
            AssertAllHidden("[[Image:foo|A [[bar]] [http://boz.com gee].]]");
            AssertAllHidden("[[Image:foo|A [[bar]] [[test]].]]");
            AssertAllHidden("[[File:foo|A [[bar]]]]");
            AssertAllHidden("[[Image:foo|A [[bar|quux]].]]");
            AssertAllHidden("[[Image:foo|A [[bar]][http://fubar].]]");
            AssertAllHidden("[[FILE:foo|A [[bar]][http://fubar].{{quux}}]]");
            AssertAllHidden("[[Image:foo|test [[Image:bar|thumb|[[boz]]]]]]"); */
        }

        [Test]
        public void HideImagesMore()
        {
            AssertAllHiddenMore(@"[[File:foo.jpg]]");
            AssertAllHiddenMore(@"[[File:foo with space and 0004.jpg]]");
            AssertAllHiddenMore(@"[[File:foo.jpeg]]");
            AssertAllHiddenMore(@"[[File:foo.JPEG]]");
            AssertAllHiddenMore(@"[[Image:foo with space and 0004.jpeg]]");
            AssertAllHiddenMore(@"[[Image:foo.jpeg]]");
            AssertAllHiddenMore(@"[[Image:foo with space and 0004.jpg]]");
            AssertAllHiddenMore(@"[[File:foo.jpg|");
            AssertAllHiddenMore(@"[[File:foo with space and 0004.jpg|");
            AssertAllHiddenMore(@"[[File:foo.jpeg|");
            AssertAllHiddenMore(@"[[Image:foo with space and 0004.jpeg|");
            AssertAllHiddenMore(@"[[Image:foo.jpeg|");
            AssertAllHiddenMore(@"[[Image:foo with SPACE() and 0004.jpg|");
            AssertAllHiddenMore(@"[[File:foo.gif|");
            AssertAllHiddenMore(@"[[Image:foo with space and 0004.gif|");
            AssertAllHiddenMore(@"[[Image:foo.gif|");
            AssertAllHiddenMore(@"[[Image:foo with SPACE() and 0004.gif|");
            AssertAllHiddenMore(@"[[File:foo.png|");
            AssertAllHiddenMore(@"[[Image:foo with space and 0004.png|");
            AssertAllHiddenMore(@"[[Image:foo_here.png|");
            AssertAllHiddenMore(@"[[Image:foo with SPACE() and 0004.png|");
            AssertAllHiddenMore(@"|image_skyline=442px_-_London_Lead_Image.jpg|");
            AssertAllHiddenMore(@"|image_map=London (European Parliament constituency).svg   |");
            AssertAllHiddenMore(@"[[Image:westminster.tube.station.jubilee.arp.jpg|");
            AssertAllHiddenMore(@"|image_map=westminster.tube.station.jubilee.arp.jpg|");
            AssertAllHiddenMore(@"|Cover  = AmorMexicanaThalia.jpg |");
            AssertAllHiddenMore(@"|image = AmorMexicanaThalia.jpg |");
            AssertAllHiddenMore(@"|
image = AmorMexicanaThalia.jpg |");
            AssertAllHiddenMore(@"|Img = BBC_logo1.jpg 
|");
            AssertAllHiddenMore(@"| image name = Fred Astaire.jpg |");
            AssertAllHiddenMore(@"|image2 = AmorMexicanaThalia.jpg |");

            // in tests below no text is hidden

            Assert.AreEqual(Caption1, HideMore(Caption1));
            Assert.AreEqual(Caption2, HideMore(Caption2));
            Assert.AreEqual(Caption3, HideMore(Caption3));
            Assert.AreEqual(Caption4, HideMore(Caption4));
            Assert.AreEqual(Caption5, HideMore(Caption5));

            // in tests below part of string is hidden
            Assert.IsTrue(HideMore(@"[[Image:some_image_name.png]] Normal words in text").EndsWith(@" Normal words in text"));
            Assert.IsTrue(HideMore(Caption4 + Field1).EndsWith(Field1));

            // this test fails at the moment due to two image files in an infobox/template in consecutive fields
            Assert.IsFalse(HideMore(@"{{Drugbox|
|IUPAC_name = 6-chloro-1,1-dioxo-2''H''-1,2,4-benzothiadiazine-7-sulfonamide
| image=Chlorothiazide.svg
| image2=Chlorothiazide-from-xtal-3D-balls.png
| CAS_number=58-94-6").Contains("=Chlorothiazide"));

            //following changes to not mask image descriptions, the following old tests are now invalid
            /*  AssertAllHiddenMore("[[File:foo]]");
                AssertAllHiddenMore("[[Image:foo|100px|bar]]");
                AssertAllHiddenMore("[[Image:foo|A [[bar]] [http://boz.com gee].]]");
                AssertAllHiddenMore("[[Image:foo|A [[bar]] [[test]].]]");
                AssertAllHiddenMore("[[Image:foo|A [[bar]]]]");
                AssertAllHiddenMore("[[FILE:foo|A [[bar|quux]].]]");
                AssertAllHiddenMore("[[Image:foo|A [[bar]][http://fubar].]]");
                AssertAllHiddenMore("[[Image:foo|A [[bar]][http://fubar].{{quux}}]]"); */
            AssertAllHiddenMore(@"| Photo =Arlberg passstrasse.jpg |");
            AssertAllHiddenMore(@"| Photo =Arlberg passstrasse.jpg}}");
            AssertAllHiddenMore(@"|photo=Arlberg passstrasse.jpg|");
            AssertAllHiddenMore(@"| Photo =Arlberg passstrasse.jpg
|");
            AssertAllHiddenMore(@"| Image =Arlberg passstrasse.jpg |");
            AssertAllHiddenMore(@"| image =Arlberg passstrasse.jpg |");
            AssertAllHiddenMore(@"| Img =Arlberg passstrasse.jpg |");
            AssertAllHiddenMore(@"| img =Arlberg passstrasse.jpg }}");
            // AssertAllHiddenMore("[[Image:foo|test [[File:bar|thumb|[[boz]]]]]]");
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

        [Test]
        public void HideWikiLinksOnlyPlusWord()
        {
            AssertAllHiddenMore("[[Foo]]bar");
            AssertAllHiddenMore("[[Foo|test]]bar");
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
