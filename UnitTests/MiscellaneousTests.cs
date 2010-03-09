using WikiFunctions;
using WikiFunctions.TalkPages;
using WikiFunctions.Parse;
using WikiFunctions.ReplaceSpecial;
using NUnit.Framework;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace UnitTests
{
    [TestFixture]
    public class HideTextTests : RequiresInitialization
    {
        #region Helpers
        const string Hidden = @"⌊⌊⌊⌊M?\d+⌋⌋⌋⌋",
            AllHidden = @"^(⌊⌊⌊⌊M?\d+⌋⌋⌋⌋)*$";
        HideText Hider;

        private string HideMore(string text, bool hideExternalLinks, bool leaveMetaHeadings, bool hideImages)
        {
            Hider = new HideText(hideExternalLinks, leaveMetaHeadings, hideImages);
            string s = Hider.HideMore(text);
            Assert.AreEqual(text, Hider.AddBackMore(s));
            return s;
        }

        private string HideMore(string text)
        {
            return HideMore(text, false);
        }

        private string HideMore(string text, bool hideOnlyTargetOfWikilink)
        {
            Hider = new HideText();
            string s = Hider.HideMore(text, hideOnlyTargetOfWikilink);
            Assert.AreEqual(text, Hider.AddBackMore(s));
            return s;
        }

        private void AssertHiddenMore(string text)
        {
            RegexAssert.IsMatch(Hidden, HideMore(text));
        }

        private void AssertAllHiddenMore(string text)
        {
            RegexAssert.IsMatch(AllHidden, HideMore(text));
        }

        private void AssertAllHiddenMore(string text, bool hideExternalLinks)
        {
            RegexAssert.IsMatch(AllHidden, HideMore(text, hideExternalLinks, false, true));
        }

        private string Hide(string text)
        {
            return Hide(text, true, false, true);
        }

        private string Hide(string text, bool hideExternalLinks, bool leaveMetaHeadings, bool hideImages)
        {
            Hider = new HideText(hideExternalLinks, leaveMetaHeadings, hideImages);
            string s = Hider.Hide(text);
            Assert.AreEqual(text, Hider.AddBack(s));
            return s;
        }

        private void AssertHidden(string text, bool hideExternalLinks, bool leaveMetaHeadings, bool hideImages)
        {
            RegexAssert.IsMatch(Hidden, Hide(text, hideExternalLinks, leaveMetaHeadings, hideImages));
        }

        private void AssertHidden(string text)
        {
            AssertHidden(text, true, false, true);
        }

        private void AssertAllHidden(string text)
        {
            RegexAssert.IsMatch(AllHidden, Hide(text));
        }
        private void AssertBothHidden(string text)
        {
            AssertBothHidden(text, true, false, true);
        }

        private void AssertBothHidden(string text, bool hideExternalLinks, bool leaveMetaHeadings, bool hideImages)
        {
            Hider = new HideText(hideExternalLinks, leaveMetaHeadings, hideImages);
            AssertAllHidden(text);
            AssertAllHiddenMore(text);
        }

        private void AssertPartiallyHidden(string expected, string text)
        {
            Assert.AreEqual(expected, Hide(text));
        }

        private void AssertPartiallyHiddenMore(string expected, string text, bool hideOnlyTargetOfWikilink)
        {
            Assert.AreEqual(expected, HideMore(text, hideOnlyTargetOfWikilink));
        }

        private void AssertPartiallyHiddenMore(string expected, string text)
        {
            AssertPartiallyHiddenMore(expected, text, false);
        }

        #endregion

        [Test]
        public void AcceptEmptyStrings()
        {
            Assert.AreEqual("", Hide(""));
            Assert.AreEqual("", HideMore(""));
        }

        [Test]
        public void HideLinks()
        {
            AssertHiddenMore("[[foo]]");
            AssertHiddenMore("[[foo|bar]]");
        }

        [Test]
        public void LeaveLinkFace()
        {
            StringAssert.Contains("bar", HideMore("[[foo|bar]]", true));
        }

        [Test]
        public void HideTemplates()
        {
            AssertAllHiddenMore("{{foo}}");
            AssertAllHiddenMore("{{{foo}}}");
            AssertAllHiddenMore("{{foo|}}");
            AssertAllHiddenMore("{{foo|bar}}");
            RegexAssert.IsMatch("123" + Hidden + "123", HideMore("123{{foo}}123"));
            AssertAllHiddenMore("{{foo|{{bar}}}}");
            AssertAllHiddenMore(@"{{foo|
abc={{bar}}
|def=hello}}");
            AssertAllHiddenMore("{{foo|{{bar|{{{1|}}}}}}}");
            AssertAllHiddenMore("{{foo|\r\nbar= {blah} blah}}");
            AssertAllHiddenMore("{{foo|\r\nbar= {blah} {{{1|{{blah}}}}}}}");
        }

        [Test]
        public void HidePstyles()
        {
            AssertAllHiddenMore(@"<p style=""margin:0px;font-size:100%""><span style=""color:#00ff00"">▪</span> <small>Francophone minorities</small></p>");
            AssertAllHiddenMore(@"<p style=""font-family:monospace; line-height:130%"">hello</p>");
            AssertAllHiddenMore(@"<p style=""font-family:monospace; line-height:130%"">hello</P>");
            AssertAllHiddenMore(@"<p style=""font-family:monospace; line-height:130%"">hello
</P>");
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
            AssertAllHidden(@"[[Image:westminster.tube.station.jubilee.arp.jpg|");
        }
        
        [Test]
        public void HideImagesLimits()
        {
            // in these ones all but the last | is hidden
            Assert.IsTrue(Regex.IsMatch(Hide(@"|image_skyline=442px_-_London_Lead_Image.jpg|"), Hidden + @"\|"));
            Assert.IsTrue(Regex.IsMatch(Hide(@"|image_map=London (European Parliament constituency).svg   |"), Hidden + @"\|"));
            Assert.IsTrue(Regex.IsMatch(Hide(@"|image_map=westminster.tube.station.jubilee.arp.jpg|"), Hidden + @"\|"));
            Assert.IsTrue(Regex.IsMatch(Hide(@"|Cover  = AmorMexicanaThalia.jpg |"), Hidden + @"\|"));
            Assert.IsTrue(Regex.IsMatch(Hide(@"|image = AmorMexicanaThalia.jpg |"), Hidden + @"\|"));
            Assert.IsTrue(Regex.IsMatch(Hide(@"|
image = AmorMexicanaThalia.jpg |"), Hidden + @"\|"));
            Assert.IsTrue(Regex.IsMatch(Hide(@"|Img = BBC_logo1.jpg 
|"), Hidden + @"\|"));
            Assert.IsTrue(Regex.IsMatch(Hide(@"| image name = Fred Astaire.jpg |"), Hidden + @"\|"));
            Assert.IsTrue(Regex.IsMatch(Hide(@"|image2 = AmorMexicanaThalia.jpg |"), Hidden + @"\|"));
            Assert.IsTrue(Regex.IsMatch(Hide(@"|map = AmorMexicanaThalia.jpg |"), Hidden + @"\|"));
        }
        
        [Test]
        public void HideImagesPartial()
        {
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
        }
        
        [Test]
        public void HideImagesMisc()
        {
            Assert.IsFalse(HideMore(@"{{Drugbox|
   |IUPAC_name = 6-chloro-1,1-dioxo-2''H''-1,2,4-benzothiadiazine-7-sulfonamide
   | image=Chlorothiazide.svg
   | image2=Chlorothiazide-from-xtal-3D-balls.png
   | CAS_number=58-94-6").Contains("=Chlorothiazide"));
            
            AssertAllHidden(@"{{ gallery
|title=Lamu images
|width=150
|lines=2
|Image:LamuFort.jpg|Lamu Fort
|Image:LAMU Riyadha Mosque.jpg|Riyadha Mosque
|Image:04 Donkey Hospital (June 30 2001).jpg|Donkey Sanctuary
}}");
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
            AssertAllHiddenMore(@"[[Image:westminster.tube.station.jubilee.arp.jpg|");

            // in these ones all but the last | is hidden
            Assert.IsTrue(Regex.IsMatch(HideMore(@"|image_skyline=442px_-_London_Lead_Image.jpg|"), Hidden + @"\|"));
            Assert.IsTrue(Regex.IsMatch(HideMore(@"|image_map=London (European Parliament constituency).svg   |"), Hidden + @"\|"));
            Assert.IsTrue(Regex.IsMatch(HideMore(@"|image_map=westminster.tube.station.jubilee.arp.jpg|"), Hidden + @"\|"));
            Assert.IsTrue(Regex.IsMatch(HideMore(@"|Cover  = AmorMexicanaThalia.jpg |"), Hidden + @"\|"));
            Assert.IsTrue(Regex.IsMatch(HideMore(@"|image = AmorMexicanaThalia.jpg |"), Hidden + @"\|"));
            Assert.IsTrue(Regex.IsMatch(HideMore(@"|
image = AmorMexicanaThalia.jpg |"), Hidden + @"\|"));
            Assert.IsTrue(Regex.IsMatch(HideMore(@"|Img = BBC_logo1.jpg 
|"), Hidden + @"\|"));
            Assert.IsTrue(Regex.IsMatch(HideMore(@"| image name = Fred Astaire.jpg |"), Hidden + @"\|"));
            Assert.IsTrue(Regex.IsMatch(HideMore(@"|image2 = AmorMexicanaThalia.jpg |"), Hidden + @"\|"));

            // in tests below no text is hidden

            Assert.AreEqual(Caption1, HideMore(Caption1));
            Assert.AreEqual(Caption2, HideMore(Caption2));
            Assert.AreEqual(Caption3, HideMore(Caption3));
            Assert.AreEqual(Caption4, HideMore(Caption4));
            Assert.AreEqual(Caption5, HideMore(Caption5));

            // in tests below part of string is hidden
            Assert.IsTrue(HideMore(@"[[Image:some_image_name.png]] Normal words in text").EndsWith(@" Normal words in text"));
            Assert.IsTrue(HideMore(Caption4 + Field1).EndsWith(Field1));

            //following changes to not mask image descriptions, the following old tests are now invalid
            /*  AssertAllHiddenMore("[[File:foo]]");
                AssertAllHiddenMore("[[Image:foo|100px|bar]]");
                AssertAllHiddenMore("[[Image:foo|A [[bar]] [http://boz.com gee].]]");
                AssertAllHiddenMore("[[Image:foo|A [[bar]] [[test]].]]");
                AssertAllHiddenMore("[[Image:foo|A [[bar]]]]");
                AssertAllHiddenMore("[[FILE:foo|A [[bar|quux]].]]");
                AssertAllHiddenMore("[[Image:foo|A [[bar]][http://fubar].]]");
                AssertAllHiddenMore("[[Image:foo|A [[bar]][http://fubar].{{quux}}]]"); */

            // in these ones all but the last | or }} is hidden
            Assert.IsTrue(Regex.IsMatch(HideMore(@"| Photo =Arlberg passstrasse.jpg |"), Hidden + @"\|"));
            Assert.IsTrue(Regex.IsMatch(HideMore(@"| Photo =Arlberg passstrasse.jpg}}"), Hidden + @"}}"));
            Assert.IsTrue(Regex.IsMatch(HideMore(@"|photo=Arlberg passstrasse.jpg|"), Hidden + @"\|"));
            Assert.IsTrue(Regex.IsMatch(HideMore(@"| Photo =Arlberg passstrasse.jpg
|"), Hidden + @"\|"));
            Assert.IsTrue(Regex.IsMatch(HideMore(@"| Image =Arlberg passstrasse.jpg |"), Hidden + @"\|"));
            Assert.IsTrue(Regex.IsMatch(HideMore(@"| image =Arlberg passstrasse.jpg |"), Hidden + @"\|"));
            Assert.IsTrue(Regex.IsMatch(HideMore(@"| Img =Arlberg passstrasse.jpg |"), Hidden + @"\|"));
            Assert.IsTrue(Regex.IsMatch(HideMore(@"| img =Arlberg passstrasse.jpg }}"), Hidden + @"}}"));
            // AssertAllHiddenMore("[[Image:foo|test [[File:bar|thumb|[[boz]]]]]]");

            Assert.IsFalse(HideMore(@"{{Drugbox|
   |IUPAC_name = 6-chloro-1,1-dioxo-2''H''-1,2,4-benzothiadiazine-7-sulfonamide
   | image=Chlorothiazide.svg
   | image2=Chlorothiazide-from-xtal-3D-balls.png
   | CAS_number=58-94-6").Contains("=Chlorothiazide"));
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
            AssertAllHiddenMore("[http://foo]", true);
            AssertAllHiddenMore("[http://foo bar]", true);
            AssertAllHiddenMore("[http://foo [bar]", true);
        }
        
        [Test]
        public void HidePlainExternalLinks()
        {
            AssertAllHiddenMore("http://foo.com/asdfasdf/asdf.htm", true);
            AssertAllHiddenMore("https://www.foo.com/asdfasdf/asdf.htm", true);
            Assert.IsFalse(HideMore(@"http://foo.com/asdfasdf/asdf.htm", true, false, false).Contains("asdf"));
            Assert.IsTrue(HideMore(@"http://foo.com/asdfasdf/asdf.htm", false, false, false).Contains("asdf"));
        }        

        [Test]
        public void HideWikiLinksOnlyPlusWord()
        {
            AssertAllHiddenMore("[[Foo]]bar");
            AssertAllHiddenMore("[[Foo|test]]bar");
        }

        [Test]
        public void SimpleHide()
        {
            AssertHidden("<source>foo</source>");
            AssertHidden(@"<source  lang=""foo_bar"">\nfoo\n</source>");
            AssertAllHidden("<source>\r\nfoo\r\n</source><source foo>bar</source>");

            AssertHidden("<pre>foo\r\nbar</pre>");
            AssertHidden("<pre style=quux>foo\r\nbar</pre>");
            AssertHidden("<math>foo\r\nbar</math>");
            AssertHidden("<timeline>foo\r\nbar</timeline>");
            AssertHidden("<timeline foo=bar>foo\r\nbar</timeline>");
            AssertHidden("<nowiki>foo\r\nbar</nowiki>");

            AssertHidden("<!--foo-->");
            AssertHidden("<!--foo\r\nbar-->");

            // hideExternalLinks
            AssertHidden("[http://foo]");
            AssertHidden("[http://foo bar]");
            AssertHidden("https://bar");
            Assert.AreEqual("http://foo", Hide("http://foo", false, false, true));

            // leaveMetaHeadings
            AssertHidden("<!--Categories -->");
            AssertHidden("<!--foo-->", true, true, true);
            Assert.AreEqual("<!-- categories-->", Hide("<!-- categories-->", true, true, true));

            // hideImages
            AssertHidden("[[Image:foo.JPG]]");
            Assert.AreEqual("[[Image:foo.jpg]]", Hide("[[Image:foo.jpg]]", true, false, false));
        }
    }

    [TestFixture]
    public class ArticleTests : RequiresInitialization
    {
        [Test]
        public void NamespacelessName()
        {
            Assert.AreEqual("Foo", new Article("Foo").NamespacelessName);
            Assert.AreEqual("Foo", new Article("Category:Foo").NamespacelessName);
            Assert.AreEqual("Category:Foo", new Article("Category:Category:Foo").NamespacelessName);

            // TODO: uncomment when Namespace.Determine() will support non-normalised names
            //Assert.AreEqual("Foo", new Article("Category : Foo").NamespacelessName);

            // this behaviour has changed in recent MW versions
            //Assert.AreEqual("", new Article("Category:").NamespacelessName);
            //Assert.AreEqual("", new Article("Category: ").NamespacelessName);
        }
    }

    [TestFixture]
    public class NamespaceTests : RequiresInitialization
    {
        [Test]
        public void Determine()
        {
            Assert.AreEqual(0, Namespace.Determine("test"));
            Assert.AreEqual(0, Namespace.Determine(":test"));
            Assert.AreEqual(0, Namespace.Determine("test:test"));
            Assert.AreEqual(0, Namespace.Determine("My Project:Foo"));

            Assert.AreEqual(Namespace.User, Namespace.Determine("User:"));

            Assert.AreEqual(Namespace.Talk, Namespace.Determine("Talk:foo"));
            Assert.AreEqual(Namespace.UserTalk, Namespace.Determine("User talk:bar"));

            Assert.AreEqual(Namespace.File, Namespace.Determine("File:foo"));
            Assert.AreEqual(Namespace.File, Namespace.Determine("Image:foo"));

            Assert.AreEqual(Namespace.Project, Namespace.Determine("Wikipedia:Foo"));
            Assert.AreEqual(Namespace.Project, Namespace.Determine("Project:Foo"));
        }

        [Test]
        public void DetermineDeviations()
        {
            Assert.AreEqual(Namespace.File, Namespace.Determine("File : foo"));
            Assert.AreEqual(Namespace.User, Namespace.Determine("user:foo"));
            Assert.AreEqual(Namespace.UserTalk, Namespace.Determine("user_talk:foo"));
            Assert.AreEqual(Namespace.UserTalk, Namespace.Determine("user%20talk:foo"));
        }

        [Test]
        public void IsTalk()
        {
            Assert.IsTrue(Namespace.IsTalk(Namespace.Talk));
            Assert.IsTrue(Namespace.IsTalk(Namespace.UserTalk));

            Assert.IsFalse(Namespace.IsTalk(Namespace.User));
            Assert.IsFalse(Namespace.IsTalk(Namespace.Project));

            Assert.IsFalse(Namespace.IsTalk(Namespace.Special));
            Assert.IsFalse(Namespace.IsTalk(Namespace.Media));

            Assert.IsTrue(Namespace.IsTalk("Talk:Test"));
            Assert.IsTrue(Namespace.IsTalk("User talk:Test"));

            Assert.IsFalse(Namespace.IsTalk("Test"));
            Assert.IsFalse(Namespace.IsTalk("User:Test"));
        }

        [Test]
        public void IsMainSpace()
        {
            Assert.IsTrue(Namespace.IsMainSpace(Namespace.Article));
            Assert.IsFalse(Namespace.IsMainSpace(Namespace.Talk));

            Assert.IsTrue(Namespace.IsMainSpace("Test"));

            Assert.IsFalse(Namespace.IsMainSpace("User:"));
            Assert.IsFalse(Namespace.IsMainSpace("Talk:Test"));
            Assert.IsFalse(Namespace.IsMainSpace("User:Test"));
            Assert.IsFalse(Namespace.IsMainSpace("User talk:Test"));

            Assert.IsFalse(Namespace.IsMainSpace("File:Test"));
            Assert.IsFalse(Namespace.IsMainSpace("Image:Test"));
        }

        [Test]
        public void IsImportant()
        {
            Assert.IsTrue(Namespace.IsImportant("Test"));
            Assert.IsTrue(Namespace.IsImportant("File:Test.jpg"));
            Assert.IsTrue(Namespace.IsImportant("Template:Test"));
            Assert.IsTrue(Namespace.IsImportant("Category:Test"));

            Assert.IsFalse(Namespace.IsImportant("Talk:Test"));
            Assert.IsFalse(Namespace.IsImportant("File talk:Test"));
            Assert.IsFalse(Namespace.IsImportant("Template talk:Test"));
            Assert.IsFalse(Namespace.IsImportant("Category talk:Test"));
            Assert.IsFalse(Namespace.IsImportant("User:Test"));
        }

        [Test]
        public void IsUserSpace()
        {
            Assert.IsTrue(Namespace.IsUserSpace("User:Test"));
            Assert.IsTrue(Namespace.IsUserSpace("User talk:Test"));

            Assert.IsFalse(Namespace.IsUserSpace("User"));
            Assert.IsFalse(Namespace.IsUserSpace("Test"));
            Assert.IsFalse(Namespace.IsUserSpace("Project:User"));
        }

        [Test]
        public void IsUserTalk()
        {
            Assert.IsTrue(Namespace.IsUserTalk("User talk:Test"));

            Assert.IsFalse(Namespace.IsUserTalk("User:Test"));
            Assert.IsFalse(Namespace.IsUserTalk("Test"));
            Assert.IsFalse(Namespace.IsUserTalk("Project:User"));
        }

        [Test]
        public void IsUserPage()
        {
            Assert.IsTrue(Namespace.IsUserPage("User:Test"));

            Assert.IsFalse(Namespace.IsUserPage("User talk:Test"));
            Assert.IsFalse(Namespace.IsUserPage("Test"));
            Assert.IsFalse(Namespace.IsUserPage("Project:User"));
        }

        [Test]
        public void IsSpecial()
        {
            Assert.IsTrue(Namespace.IsSpecial(Namespace.Media));
            Assert.IsTrue(Namespace.IsSpecial(Namespace.Special));

            Assert.IsFalse(Namespace.IsSpecial(Namespace.Article));
            Assert.IsFalse(Namespace.IsSpecial(Namespace.TemplateTalk));
        }

        [Test]
        public void NormalizeNamespace()
        {
            Assert.AreEqual("User:", Namespace.Normalize("User:", 2));
            Assert.AreEqual("User:", Namespace.Normalize("user :", 2));
            Assert.AreEqual("User talk:", Namespace.Normalize("User_talk:", 3));

            Assert.AreEqual("Image:", Namespace.Normalize("image:", 6));
            Assert.AreEqual("File:", Namespace.Normalize("file:", 6));
            Assert.AreEqual("Image talk:", Namespace.Normalize("image talk:", 7));
        }

        [Test]
        public void Verify()
        {
            Assert.IsTrue(Namespace.VerifyNamespaces(Variables.CanonicalNamespaces));

            var ns = new Dictionary<int, string>(Variables.CanonicalNamespaces);
            
            ns[0] = "";
            Assert.IsFalse(Namespace.VerifyNamespaces(ns));

            ns.Remove(0);
            ns[1] = "Talk";
            Assert.IsFalse(Namespace.VerifyNamespaces(ns));
            
            ns.Remove(1);
            Assert.IsFalse(Namespace.VerifyNamespaces(ns));
        }
    }

    [TestFixture]
    public class ConcurrencyTests : RequiresInitialization
    {
        [Test]
        public void Parser()
        {
            Parsers p1 = new Parsers();
            Parsers p2 = new Parsers();

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_10#NullReferenceException_in_HideText.AddBackMore
            string s1 = p1.HideText("<pre>foo bar</pre>");
            string s2 = p2.HideText("<source>quux</source>");
            Assert.AreEqual("<pre>foo bar</pre>", p1.AddBackText(s1));
            Assert.AreEqual("<source>quux</source>", p2.AddBackText(s2));

            // in the future, we may use parser objects for processing several wikis at once
            //Assert.AreNotEqual(p1.StubMaxWordCount, p2.StubMaxWordCount);

        }
    }
    
    [TestFixture]
    public class TalkHeaderTests : RequiresInitialization
    {
        const string articleTextHeader = @"{{talkheader|noarchive=no}}
[[Category:Foo bar]]";
        string newSummary = "";
        
        [Test]
        public void MoveTalkHeader()
        {
            string talkheader = @"{{talk header|noarchive=no|search=no|arpol=no|wp=no|disclaimer=no|shortcut1|shortcut2}}", talkrest = @"==hello==
hello talk";
            string articleText = talkrest + "\r\n" + talkheader;
            
            TalkPageHeaders.ProcessTalkPage(ref articleText, ref newSummary, DEFAULTSORT.NoChange);
            
            Assert.AreEqual(talkheader + "\r\n" + talkrest + "\r\n", articleText);
            Assert.IsTrue(newSummary.Contains("{{tl|Talk header}} given top billing"));
            
            // handles {{talk header}} on same line as other template
            string WPBS = @"{{WikiProjectBannerShell|blp=yes|1=
{{OH-Project|class=B|importance=Low|nested=yes}}
{{WPBiography|living=yes|class=B|priority=Low|filmbio-work-group=yes|nested=yes|listas=Parker, Sarah Jessica}}
{{WikiProject Cincinnati|class=B|importance=mid|nested=yes}}
}}", rest = "\r\n" +  @"==Song Jessie by Joshua Kadison==
In the article it says that above mentioned";
            articleText = WPBS + @"{{Talkheader}}" + rest;
            
            TalkPageHeaders.ProcessTalkPage(ref articleText, ref newSummary, DEFAULTSORT.NoChange);
            
            Assert.AreEqual(@"{{talk header}}" + "\r\n" + WPBS + rest, articleText);
            Assert.IsTrue(newSummary.Contains("{{tl|Talk header}} given top billing"));
            
            // no change if already at top
            articleText = talkheader + "\r\n" + talkrest;
            newSummary = "";
            TalkPageHeaders.ProcessTalkPage(ref articleText, ref newSummary, DEFAULTSORT.NoChange);
            Assert.AreEqual(talkheader + "\r\n" + talkrest, articleText);
            Assert.IsFalse(newSummary.Contains("{{tl|Talk header}} given top billing"));
            
            // no change if no talk header
            articleText = talkrest;
            newSummary = "";
            TalkPageHeaders.ProcessTalkPage(ref articleText, ref newSummary, DEFAULTSORT.NoChange);
            Assert.AreEqual(talkrest, articleText);
            Assert.IsFalse(newSummary.Contains("{{tl|Talk header}} given top billing"));
        }
        
        [Test]
        public void RenameTalkHeader()
        {
        	 string talkheader = @"{{talkheader|noarchive=no}}", talkrest = @"==hello==
hello talk";
            string articleText = talkrest + "\r\n" + talkheader, newSummary = "";
            
            TalkPageHeaders.ProcessTalkPage(ref articleText, ref newSummary, DEFAULTSORT.NoChange);
            
            Assert.AreEqual(@"{{talk header|noarchive=no}}" + "\r\n" + talkrest+ "\r\n", articleText);
            Assert.IsTrue(newSummary.Contains("{{tl|Talk header}} given top billing"));
        }
        
        [Test]
        public void AddMissingFirstCommentHeader()
        {
            const string comment = @"
Hello world comment.";
            string newSummary = "";
            string articleTextIn = articleTextHeader + comment;
            
            // plain comment
            TalkPageHeaders.ProcessTalkPage(ref articleTextIn, ref newSummary, DEFAULTSORT.NoChange);
            
            Assert.AreEqual(articleTextIn, articleTextHeader + "\r\n" + @"
==Untitled==
Hello world comment.");
            Assert.IsTrue(newSummary.Contains("Added missing comments section header"));
            
            // idented comment2
            articleTextIn = articleTextHeader + @"
*Hello world comment2.";
            newSummary = "";
            TalkPageHeaders.ProcessTalkPage(ref articleTextIn, ref newSummary, DEFAULTSORT.NoChange);
            
            Assert.AreEqual(articleTextIn, articleTextHeader +"\r\n" + @"
==Untitled==
*Hello world comment2.");
            Assert.IsTrue(newSummary.Contains("Added missing comments section header"));
            
            // idented comment3
            articleTextIn = articleTextHeader + @"
:Hello world comment3.";
            newSummary = "";
            TalkPageHeaders.ProcessTalkPage(ref articleTextIn, ref newSummary, DEFAULTSORT.NoChange);
            
            Assert.AreEqual(articleTextIn, articleTextHeader + "\r\n" + @"
==Untitled==
:Hello world comment3.");
            Assert.IsTrue(newSummary.Contains("Added missing comments section header"));
            
            // quoted comment
            articleTextIn = articleTextHeader + @"
""Hello world comment4"".";
            newSummary = "";
            TalkPageHeaders.ProcessTalkPage(ref articleTextIn, ref newSummary, DEFAULTSORT.NoChange);
            
            Assert.AreEqual(articleTextIn, articleTextHeader + "\r\n" + @"
==Untitled==
""Hello world comment4"".");
            Assert.IsTrue(newSummary.Contains("Added missing comments section header"));
            
            // heading level 3 changed to level 2
            articleTextIn = articleTextHeader + "\r\n" + @"===Foo bar===
*Hello world comment2.";
            newSummary = "";
            TalkPageHeaders.ProcessTalkPage(ref articleTextIn, ref newSummary, DEFAULTSORT.NoChange);
            
            Assert.IsTrue(articleTextIn.Contains(@"==Foo bar=="));
            Assert.IsFalse(articleTextIn.Contains(@"==Untitled=="));
            Assert.IsTrue(newSummary.Contains("Corrected comments section header"));
        }
        
         [Test]
        public void AddMissingFirstCommentHeaderNoChanges()
        {
            // no change – header already
            string articleTextIn = articleTextHeader + @"
==Question==
:Hello world comment3.";
            
            newSummary = "";
            TalkPageHeaders.ProcessTalkPage(ref articleTextIn, ref newSummary, DEFAULTSORT.NoChange);
            
            Assert.AreEqual(articleTextIn, articleTextHeader + @"
==Question==
:Hello world comment3.");
            Assert.IsFalse(newSummary.Contains("Added missing comments section header"));
            
            // no change – already header at top
            articleTextIn = @"
{{Some template}}
==Question==
:Hello world comment3.";
            
            newSummary = "";
            TalkPageHeaders.ProcessTalkPage(ref articleTextIn, ref newSummary, DEFAULTSORT.NoChange);
            
            Assert.AreEqual(@"
{{Some template}}
==Question==
:Hello world comment3.", articleTextIn);
            Assert.IsFalse(newSummary.Contains("Added missing comments section header"));
            
            // no change – already header at top 2
            articleTextIn = @"
==Question==
{{Some template}}
:Hello world comment3.";
            
            newSummary = "";
            TalkPageHeaders.ProcessTalkPage(ref articleTextIn, ref newSummary, DEFAULTSORT.NoChange);
            
            Assert.AreEqual(@"
==Question==
{{Some template}}
:Hello world comment3.", articleTextIn);
            Assert.IsFalse(newSummary.Contains("Added missing comments section header"));
            
            // no change – no comments
                  articleTextIn = @"
==Question==
{{Some template}}";
            
            newSummary = "";
            TalkPageHeaders.ProcessTalkPage(ref articleTextIn, ref newSummary, DEFAULTSORT.NoChange);
            
            Assert.AreEqual(@"
==Question==
{{Some template}}", articleTextIn);
            Assert.IsFalse(newSummary.Contains("Added missing comments section header"));
            
            // no change – only text in template
                  articleTextIn = @"
{{foo|
bar|
end}}";
            
            newSummary = "";
            TalkPageHeaders.ProcessTalkPage(ref articleTextIn, ref newSummary, DEFAULTSORT.NoChange);
            
            Assert.AreEqual(@"
{{foo|
bar|
end}}", articleTextIn);
            Assert.IsFalse(newSummary.Contains("Added missing comments section header"));
            
            // no change – only TOC
            articleTextIn = @"
__TOC__";
            
            newSummary = "";
            TalkPageHeaders.ProcessTalkPage(ref articleTextIn, ref newSummary, DEFAULTSORT.NoChange);
            
            Assert.AreEqual(@"
__TOC__", articleTextIn);
            Assert.IsFalse(newSummary.Contains("Added missing comments section header"));
            
            // no change -- only in template
            const string allInTemplate = @"{{archive box|
*[[/Archive: GA review|Good Article review]]}}

== Explanation of Wright's work in ''Certaine Errors'' ==";
            
            articleTextIn = allInTemplate;
             newSummary = "";
            TalkPageHeaders.ProcessTalkPage(ref articleTextIn, ref newSummary, DEFAULTSORT.NoChange);
            
            Assert.AreEqual(allInTemplate, articleTextIn);
            Assert.IsFalse(newSummary.Contains("Added missing comments section header"));
        }
    }
    
    [TestFixture]
    public class InTemplateRuleTests
    {
        [Test]
        public void TemplateUsedInText()
        {
            // casing
            Assert.IsTrue(InTemplateRule.TemplateUsedInText("Bert", @"Bert}} was great"));
            Assert.IsTrue(InTemplateRule.TemplateUsedInText("Bert", @"bert}} was great"));
            Assert.IsTrue(InTemplateRule.TemplateUsedInText("bert", @"Bert}} was great"));
            Assert.IsTrue(InTemplateRule.TemplateUsedInText("bert", @"bert}} was great"));
            
            //spacing
            Assert.IsTrue(InTemplateRule.TemplateUsedInText("Bert", @"Bert }} was great"));
            Assert.IsTrue(InTemplateRule.TemplateUsedInText("Bert", @" Bert}} was great"));
            Assert.IsTrue(InTemplateRule.TemplateUsedInText("Bert", @"Bert|fo=yes}} was great"));
            Assert.IsTrue(InTemplateRule.TemplateUsedInText("Bert", @"Bert
|fo=yes}} was great"));
            Assert.IsTrue(InTemplateRule.TemplateUsedInText("Bert", @"Bert |fo=yes}} was great"));
            Assert.IsTrue(InTemplateRule.TemplateUsedInText("Bert", @" Bert|fo=yes}} was great"));
            Assert.IsTrue(InTemplateRule.TemplateUsedInText("Bert", @"
    Bert|fo=yes}} was great"));
            
            //comments
            Assert.IsTrue(InTemplateRule.TemplateUsedInText("Bert", @"Bert|fo=yes}} was <!--great-->"));
            Assert.IsTrue(InTemplateRule.TemplateUsedInText("Bert", @"<!--thing--> Bert }} was great"));
            Assert.IsTrue(InTemplateRule.TemplateUsedInText("Bert", @"Bert<!--thing-->}} was great"));
            Assert.IsTrue(InTemplateRule.TemplateUsedInText("Bert", @"Bert<!--thing-->|foo=bar}} was great"));
            
            Assert.IsTrue(InTemplateRule.TemplateUsedInText("", @"Bert}} was great"));
            
            // underscores
            Assert.IsTrue(InTemplateRule.TemplateUsedInText("Bert Li", @"Bert Li}} was great"));
            Assert.IsTrue(InTemplateRule.TemplateUsedInText("Bert Li", @"Bert   Li}} was great"));
            Assert.IsTrue(InTemplateRule.TemplateUsedInText("Bert Li", @"Bert_Li}} was great"));
        }
        
        [Test]
        public void TemplateUsedInTextNoMatches()
        {
            Assert.IsFalse(InTemplateRule.TemplateUsedInText("Bert", @"{{Bert}} was great"));
            Assert.IsFalse(InTemplateRule.TemplateUsedInText("Bert", @"Bert was great"));
            Assert.IsFalse(InTemplateRule.TemplateUsedInText("Bert", @"[[Bert]] was great"));
            Assert.IsFalse(InTemplateRule.TemplateUsedInText("Bert", @"Tim}} was great"));
            Assert.IsFalse(InTemplateRule.TemplateUsedInText("Bert", @"BERT}} was great"));
            Assert.IsFalse(InTemplateRule.TemplateUsedInText("Bert Li", @"BertLi}} was great"));
            
            Assert.IsFalse(InTemplateRule.TemplateUsedInText("Bert", @"<!--Bert}}--> was great"));
        }
    }
}
