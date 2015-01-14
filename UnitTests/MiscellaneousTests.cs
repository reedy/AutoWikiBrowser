using WikiFunctions;
using WikiFunctions.TalkPages;
using WikiFunctions.Parse;
using WikiFunctions.ReplaceSpecial;
using NUnit.Framework;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using WikiFunctions.Controls.Lists;

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

        #endregion

        [Test]
        public void HideMore()
        {
            Hider = new HideText(true, false, true);

            string text = Hider.HideMore("[[foo]]", false, true);
            RegexAssert.IsMatch(AllHidden, text);
            text = Hider.AddBackMore(text);
            Assert.AreEqual("[[foo]]", text);
        }

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
        public void HideItalics()
        {
            StringAssert.DoesNotContain("text", HideMore("Now ''text'' was"));
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
            
            AssertAllHidden(@"<imagemap>
File:Blogs001.jpeg|Description
File:Blogs002.jpeg|Description
</imagemap>");

            AssertBothHidden(@"[[File:foo.jpg]]");
            AssertBothHidden(@"[[Image:foo with space and 0004.png|");
            AssertBothHidden(@"[[Image:foo_here.png|");

            Assert.IsFalse(HideMore(@"[[Category:Foo|abc]]", false).Contains("abc"), "Category sort key always hidden if hiding wikilinks and not leaving target");
            Assert.IsFalse(HideMore(@"[[Category:Foo|abc]]", true).Contains("abc"), "Category sort key hidden even if keeping targets");

            HideText h = new HideText(true, false, false);
            Assert.IsTrue(h.HideMore(@"[[Category:Foo|abc]]", false, false).Contains("abc"), "Category sort key kept if keeping wikilinks");
        }
        
        [Test]
        public void HideImagesLimits()
        {
            // in these ones all but the last | is hidden
            Assert.IsTrue(Regex.IsMatch(Hide(@"|image_skyline=442px_-_London_Lead_Image.jpg|"), Hidden + @"\|"));
            Assert.IsTrue(Regex.IsMatch(Hide(@"|image_skyline=442px_-_London_Lead_Image.jpg <!--comm-->|"), Hidden + " " + Hidden + @"\|"));
            Assert.IsTrue(Regex.IsMatch(Hide(@"|image_map=London (European Parliament constituency).svg   |"), Hidden + @"   \|"));
            Assert.IsTrue(Regex.IsMatch(Hide(@"|image_map=westminster.tube.station.jubilee.arp.jpg|"), Hidden + @"\|"));
            Assert.IsTrue(Regex.IsMatch(Hide(@"|Cover  = AmorMexicanaThalia.jpg |"), Hidden + @" \|"));
            Assert.IsTrue(Regex.IsMatch(Hide(@"|image = AmorMexicanaThalia.jpg |"), Hidden + @" \|"));
            Assert.IsTrue(Regex.IsMatch(Hide(@"|
image = AmorMexicanaThalia.jpg |"), Hidden + @" \|"));
            Assert.IsTrue(Regex.IsMatch(Hide(@"|Img = BBC_logo1.jpg
|"), Hidden + @"
\|"));
            Assert.IsTrue(Regex.IsMatch(Hide(@"| image name = Fred Astaire.jpg |"), Hidden + @" \|"));
            Assert.IsTrue(Regex.IsMatch(Hide(@"|image2 = AmorMexicanaThalia.jpg |"), Hidden + @" \|"));
            Assert.IsTrue(Regex.IsMatch(Hide(@"|map = AmorMexicanaThalia.jpg |"), Hidden + @" \|"));
            Assert.IsTrue(Regex.IsMatch(Hide(@"|AmorMexicanaThalia.jpg |"), Hidden + @" \|"));
        }
        
        [Test]
        public void HideCiteTitles()
        {
            Assert.IsFalse(Hide(@"{{cite web| title = foo | date = May 2011").Contains(@"foo"));
            Assert.IsTrue(Tools.GetTemplateParameterValue(Hide(@"{{cite web| title = foo | date = May 2011"), "title").Length > 0);
            
            Assert.IsFalse(Hide(@"{{cite web| trans_title = foo | date = May 2011").Contains(@"foo"));
        }
        
        [Test]
        public void HideNotATypo()
        {
            AssertAllHidden(@"{{not a typo|foo}}");
            AssertAllHidden(@"{{Not a typo|The pEneLOpe[s]}}");
            AssertAllHidden(@"{{typo|foo}}");
            AssertAllHidden(@"{{proper name|foo}}");
            AssertAllHidden(@"{{as written|foo}}");
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
   | CAS_number=58-94-6}}").Contains("=Chlorothiazide"));
            
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

            // in tests below no text is hidden

            Assert.AreEqual(Caption1, HideMore(Caption1));
            Assert.AreEqual(Caption2, HideMore(Caption2));
            Assert.AreEqual(Caption3, HideMore(Caption3));
            Assert.AreEqual(Caption4, HideMore(Caption4));
            Assert.AreEqual(Caption5, HideMore(Caption5));

            // in tests below part of string is hidden
            Assert.IsTrue(HideMore(@"[[Image:some_image_name.png]] Normal words in text").EndsWith(@" Normal words in text"));
            Assert.IsTrue(HideMore(Caption4 + Field1).EndsWith(Field1));

            Assert.IsFalse(HideMore(@"{{Drugbox|
   |IUPAC_name = 6-chloro-1,1-dioxo-2''H''-1,2,4-benzothiadiazine-7-sulfonamide
   | image=Chlorothiazide.svg
   | image2=Chlorothiazide-from-xtal-3D-balls.png
   | CAS_number=58-94-6}}").Contains("=Chlorothiazide"));
            
            AssertAllHiddenMore(@"<imagemap>
File:Blogs001.jpeg|Description
File:Blogs002.jpeg|Description
</imagemap>");
        }

        [Test]
        public void HideGalleries()
        {
            Assert.IsTrue(Regex.IsMatch(HideMore(@"<gallery>
Image:quux.JPEG|text
</gallery>"), @"<gallery>
" + Hidden + @"|text
</gallery>"), "Simple case: one image link with namespace hidden, description retained");
            Assert.IsTrue(Regex.IsMatch(HideMore(@"<gallery>
quux.JPEG|text
</gallery>"), @"<gallery>
" + Hidden + @"|text
</gallery>"), "One image link without namespace hidden, description retained");
            Assert.IsTrue(Regex.IsMatch(HideMore(@"<gallery>
quux.JPEG|
</gallery>"), @"<gallery>
" + Hidden + @"|
</gallery>"), "One image link without namespace hidden, blank description");
            Assert.IsTrue(Regex.IsMatch(HideMore(@"<gallery>
quux.JPEG
</gallery>"), @"<gallery>
" + Hidden + @"
</gallery>"), "One image link without namespace hidden, no description ");
            Assert.IsTrue(Regex.IsMatch(HideMore(@"<gallery>
Image:foo.png|a bar
Image:quux.JPEG|text
</gallery>"), @"<gallery>
" + Hidden + @"|a bar
" + Hidden + @"|text
</gallery>"), "Multiple image links hidden");

            Assert.IsTrue(Regex.IsMatch(HideMore(@"<gallery param=""a"">
Image:quux.svg|text
</gallery>"), @"<gallery param=""a"">
" + Hidden + @"|text
</gallery>"), "Images hidden within gallery tags with parameters");
            Assert.IsTrue(Regex.IsMatch(HideMore(@"<Gallery>
Image:foo.png|a bar
Image:quux.JPEG|text
</gallery>"), @"<Gallery>
" + Hidden + @"|a bar
" + Hidden + @"|text
</gallery>"), "gallery tag casing");

            Assert.IsTrue(Regex.IsMatch(HideMore(@"<gallery>
Image:foo
Image:quux.JPEG|text
</gallery>"), @"<gallery>
Image:foo
" + Hidden + @"|text
</gallery>"), "Image link without file extension not hidden");
            Assert.IsTrue(Regex.IsMatch(HideMore(@"<gallery>
File:foo.png
File:9th of quux.JPEG|text
</gallery>"), @"<gallery>
" + Hidden + @"
" + Hidden + @"|text
</gallery>"), "image without description hidden OK");
            Assert.IsTrue(Regex.IsMatch(HideMore(@"<gallery>
File:bar ă foo.jpg|text1
File:9th of May, ățuux.JPEG|text2
</gallery>"), @"<gallery>
" + Hidden + @"|text1
" + Hidden + @"|text2
</gallery>"),"special characters in image name handled OK");
            Assert.IsTrue(Regex.IsMatch(HideMore(@"<gallery>
File:foo1.jpg|[[foobar]] barbar
File:foo2.jpg|Winter Festival
Image:Foo3.jpg|Bar, detail
File:9th of June street , Bacău.JPG|[[Romanian War of Independence#Overview|9th of May]] Street
</gallery>"), @"<gallery>
" + Hidden + "|" + Hidden + @" barbar
" + Hidden + @"|Winter Festival
" + Hidden + @"|Bar, detail
" + Hidden + "|" + Hidden + @" Street
</gallery>"), "Wiki formatting within image description handled OK");
        }

        [Test]
        public void HideExternalLinks()
        {
            AssertAllHiddenMore("[http://foo]", true);
            AssertAllHiddenMore("[http://foo bar]", true);
            AssertAllHiddenMore("[http://foo [bar]", true);
            AssertAllHiddenMore("[[ru:Link]]", true); // possible interwiki
            Assert.IsFalse(Hide(@"[[ru:Link]]", true, true, true).Contains("Link")); // possible interwiki
            
            Assert.IsTrue(Hide(@"date=April 2010|url=http://w/010111a.html}}", true, true, true).Contains(@"date=April 2010|url="));
            Assert.IsTrue(Hide(@"date=April 2010|url=http://w/010111a.html}}", true, true, true).Contains(@"}}"));
        }
        
        [Test]
        public void HidePlainExternalLinks()
        {
            AssertAllHiddenMore("http://foo.com/asdfasdf/asdf.htm", true);
            AssertAllHiddenMore("https://www.foo.com/asdfasdf/asdf.htm", true);
            AssertAllHiddenMore("ftp://www.foo.com/asdfasdf/asdf.htm", true);
            AssertAllHiddenMore("mailto://www.foo.com/asdfasdf/asdf.htm", true);
            AssertAllHiddenMore("irc://www.foo.com/asdfasdf/asdf.htm", true);
            AssertAllHiddenMore("gopher://www.foo.com/asdfasdf/asdf.htm", true);
            AssertAllHiddenMore("telnet://www.foo.com/asdfasdf/asdf.htm", true);
            AssertAllHiddenMore("nntp://www.foo.com/asdfasdf/asdf.htm", true);
            AssertAllHiddenMore("worldwind://www.foo.com/asdfasdf/asdf.htm", true);
            AssertAllHiddenMore("news://www.foo.com/asdfasdf/asdf.htm", true);
            AssertAllHiddenMore("svn://www.foo.com/asdfasdf/asdf.htm", true);
            
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
            AssertHidden(@"<syntaxhighlight  lang=""foo_bar"">\nfoo\n</syntaxhighlight>");
            AssertHidden(@"<syntaxhighlight  lang=""javascript"">\nfoo\n</syntaxhighlight>");
            AssertAllHidden("<syntaxhighlight>\r\nfoo\r\n</source><source foo>bar</syntaxhighlight>");
            AssertHidden("<tt>foo</tt>");

            AssertHidden("<pre>foo\r\nbar</pre>");
            AssertHidden("<pre style=quux>foo\r\nbar</pre>");
            AssertHidden("<math>foo\r\nbar</math>");
            AssertHidden("{{math|foo\r\nbar}}");
            AssertHidden("{{code|foo\r\nbar}}");
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
            AssertHidden("[[File:foo.JPG]]");
            Assert.AreEqual("[[File:foo.jpg]]", Hide("[[File:foo.jpg]]", true, false, false));

            // <hiero> tags
            AssertAllHiddenMore(@"<hiero>foo</hiero>");
        }
    }

    [TestFixture]
    public class ArticleTests : RequiresInitialization
    {
        [Test]
        public void General()
        {
            Article a = new Article("A");
            Article b = new Article("A");
            b=null;
            Assert.IsFalse(a.Equals(null));
            b = new Article("A");
            Assert.AreEqual("A", a.ToString());
            Assert.AreEqual("A", a.Name);
            Assert.IsFalse(a != b);
            
            Assert.AreEqual("A", a.URLEncodedName);
            Assert.AreEqual(null, a.DisplayTitle);
        }
        
        [Test]
        public void AutoTag()
        {
			#if DEBUG
            const string LongText =
            @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse dictum ultrices augue. Fusce sem diam, vestibulum sit amet, vehicula id, congue a, nisl. Phasellus pulvinar posuere purus. Donec elementum justo mattis nulla. Sed a purus dictum lacus pharetra adipiscing. Nam non dui non ante viverra iaculis. Fusce euismod lacus id nulla vulputate gravida. Suspendisse lectus pede, tempus sed, tristique id, pharetra eget, urna. Integer mattis libero vel quam accumsan suscipit. Vivamus molestie dapibus est. Quisque quis metus eget nisl accumsan aliquet. Donec tempus pellentesque tellus. Aliquam lacinia gravida justo. Aliquam erat volutpat. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Mauris ultricies suscipit urna. Ut mollis tempor leo. Pellentesque fringilla mattis enim. Proin sapien enim, congue non, aliquet et, sollicitudin nec, mauris. Sed porta.

Curabitur luctus mollis massa. Nullam consectetur mollis lacus. Suspendisse turpis. Fusce velit. Morbi egestas dui. Donec commodo ornare lorem. Vestibulum sodales. Curabitur egestas libero ut metus. Sed eget orci a ligula consectetur vestibulum. Cras sapien.

Sed libero. Ut volutpat massa. Donec nulla pede, porttitor eu, sodales et, consectetur nec, quam. Pellentesque vestibulum hendrerit est. Nulla facilisi. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Duis et nibh eu lacus iaculis pretium. Fusce sed turpis. In cursus. Etiam interdum augue. Morbi commodo auctor ligula. In imperdiet, neque nec hendrerit consequat, lacus purus tristique turpis, eu hendrerit ipsum ligula at libero. Duis varius nunc vel tortor. Praesent tempor. Nunc non pede at velit congue feugiat. Curabitur gravida, nisl quis mattis porttitor, purus nulla viverra dui, non suscipit augue nunc ac libero. Donec lacinia est non augue.

Nulla quam dui, tristique id, condimentum sed, sodales in, ante. Vestibulum vitae diam. Integer placerat ante non orci. Nulla gravida. Integer magna enim, iaculis ut, ornare dignissim, ultrices a, urna. Donec urna. Fusce fringilla, pede vitae pulvinar ullamcorper, est nisi eleifend ipsum, ac adipiscing odio massa vehicula neque. Sed blandit est. Morbi faucibus, nisl vel commodo vulputate, mi ipsum tincidunt sem, id ornare orci orci et velit. Morbi commodo sollicitudin ligula. Pellentesque vitae urna. Duis massa arcu, accumsan id, euismod eu, tincidunt et, odio. Phasellus purus leo, rhoncus sed, condimentum nec, vestibulum vel, lacus. In egestas, lectus vitae lacinia tristique, elit magna consequat risus, id sodales metus nulla ac pede. Suspendisse potenti.

Fusce massa. Nullam lacinia purus nec ipsum. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Suspendisse potenti. Proin augue. Donec mi magna, interdum a, elementum quis, bibendum sit amet, felis. Donec vel libero eget magna hendrerit ultrices. Suspendisse potenti. Sed scelerisque lacinia nisi. Quisque elementum, nunc nec luctus iaculis, ante quam aliquet orci, et ullamcorper dui ipsum at mi. Vestibulum a dolor id tortor posuere elementum. Sed mauris nisl, ultrices a, malesuada non, convallis ac, velit. Sed aliquam elit id metus. Donec malesuada, lorem ut pharetra auctor, mi risus viverra enim, vitae pulvinar urna metus at lorem. Vivamus id lorem. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Nulla facilisi. Ut vel odio. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Pellentesque lobortis sem.

Proin in odio. Pellentesque [[habitant]] [[morbi]] [[tristique]] senectus et netus et malesuada fames ac turpis egestas. Vivamus bibendum arcu nec risus. Nulla iaculis ligula in purus. Etiam vulputate nibh sit amet lectus. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Suspendisse potenti. Suspendisse eleifend. Donec blandit nibh hendrerit turpis. Integer accumsan posuere odio. Ut commodo augue malesuada risus. Curabitur augue. Praesent volutpat nunc a diam. Nulla lobortis interdum dolor. Nunc imperdiet, ipsum ac tempor iaculis, nunc.
";
            Article a = new Article("A", "ABC");
            Parsers p = new Parsers(500, true);
            a.AutoTag(p, true, true);
            Assert.IsFalse(a.NoArticleTextChanged);

            a = new Article("A", LongText);
            Globals.UnitTestIntValue = 1;
            Globals.UnitTestBoolValue = false;
            a.AutoTag(p, true, true);
            Assert.IsTrue(a.NoArticleTextChanged);
            #endif
        }

        [Test]
        public void AlertProperties()
        {
            Article a = new Article("A", "{{dead link}}");
            Assert.AreEqual(1, a.DeadLinks().Count);
            Assert.IsTrue(a.HasDeadLinks);
            
            a = new Article("A", "<small>");
            Assert.AreEqual(1, a.UnclosedTags().Count);
            
            a = new Article("A", "==[[A]]==");
            Assert.AreEqual(1, a.WikiLinkedHeaders().Count);
            
            a = new Article("A", "{{multiple issues|foo=bar}}");
            Assert.AreEqual(1, a.UnknownMultipleIssuesParameters().Count);
            
            a = new Article("Talk:A", "{{WikiProjectBannerShell|foo=bar}}");
            Assert.AreEqual(1, a.UnknownWikiProjectBannerShellParameters().Count);
            
            a = new Article("Talk:A", "{{WikiProjectBannerShell|foo=bar|foo=bar}}");
            Assert.AreEqual(1, a.DuplicateWikiProjectBannerShellParameters().Count);
            
            a = new Article("A", "[[A|B|C]]");
            Assert.AreEqual(1, a.DoublepipeLinks().Count);

            a = new Article("A", "[[A||BC]]");
            Assert.AreEqual(1, a.DoublepipeLinks().Count);
            
            a = new Article("A", "[[|A]]");
            Assert.AreEqual(1, a.TargetlessLinks().Count);
            
            a = new Article("A", "{{cite web|sajksdfa=a}}");
            Assert.AreEqual(1, a.BadCiteParameters().Count);
            
            a = new Article("A", "{{cite web|date=5-4-10}}");
            Assert.AreEqual(1, a.AmbiguousCiteTemplateDates().Count);
            
            a = new Article("A", "[[User talk:Noobie]]");
            Assert.AreEqual(1, a.UserSignature().Count);

            a.ResetEditSummary();
            Assert.AreEqual("", a.EditSummary);
        }

        [Test]
        public void Properties()
        {
            Article a = new Article("A", "ABC");
            Assert.IsFalse(a.ArticleIsAboutAPerson);
            
            a = new Article("A");
            Assert.IsFalse(a.HasDiacriticsInTitle);
            
            a = new Article("Aé");
            Assert.IsTrue(a.HasDiacriticsInTitle);
            
            Assert.IsTrue(a.IsStub);
            
            a = new Article("A", "ABC");
            Assert.IsFalse(a.HasStubTemplate);
            Assert.IsFalse(a.HasInfoBox);
            Assert.IsFalse(a.HasSicTag);
            Assert.IsFalse(a.IsInUse);
            Assert.IsFalse(a.HasTargetLessLinks);
            Assert.IsFalse(a.HasDoublePipeLinks);
            Assert.IsFalse(a.HasMorefootnotesAndManyReferences);
            Assert.IsFalse(a.IsDisambiguationPage);
            Assert.IsFalse(a.IsDisambiguationPageWithRefs);
            Assert.IsFalse(a.IsSIAPage);
            Assert.IsFalse(a.HasRefAfterReflist);
            Assert.IsFalse(a.HasNamedReferences);
            Assert.IsFalse(a.HasBareReferences);
            Assert.IsFalse(a.HasAmbiguousCiteTemplateDates);
            Assert.IsFalse(a.HasSeeAlsoAfterNotesReferencesOrExternalLinks);
            Assert.IsFalse(a.IsMissingReferencesDisplay);
            Assert.IsFalse(a.IsRedirect);
        }
        
        [Test]
        public void CanDoGeneralFixes()
        {
            Article a = new Article("A", "ABC");
            Assert.IsTrue(a.CanDoGeneralFixes);
            
            Assert.IsFalse(a.CanDoTalkGeneralFixes);
            a = new Article("Talk:A", "ABC");
            Assert.IsTrue(a.CanDoTalkGeneralFixes);
        }

        [Test]
        public void FixPeopleCategories()
        {
            Parsers p = new Parsers();
            Article a = new Article("A", "ABC");
            a.FixPeopleCategories(p, true);
            Assert.AreEqual("ABC", a.ArticleText);
        }

        [Test]
        public void SetDefaultSort()
        {
            Article a = new Article("A", "ABC");
            a.SetDefaultSort("en", true);
            Assert.AreEqual("ABC", a.ArticleText);
        }

        [Test]
        public void Changes()
        {
            Article a = new Article("A", "ABC");
            a.AWBChangeArticleText("test", "ABC D", false);
            Assert.IsFalse(a.OnlyWhiteSpaceChanged);
            Assert.IsFalse(a.OnlyCasingChanged);

            a = new Article("A", "ABC");
            a.AWBChangeArticleText("test", "AB C", false);
            Assert.IsTrue(a.OnlyWhiteSpaceChanged);
            Assert.IsTrue(a.OnlyWhiteSpaceAndCasingChanged);
            Assert.IsFalse(a.OnlyCasingChanged);
            Assert.IsFalse(a.NoArticleTextChanged);

            a = new Article("A", "ABC");
            a.AWBChangeArticleText("test", "ABc", false);
            Assert.IsTrue(a.OnlyCasingChanged);

            a = new Article("A", "ABC");
            a.AWBChangeArticleText("test", "ABC", false);
            Assert.IsTrue(a.NoArticleTextChanged);
            Assert.IsFalse(a.OnlyMinorGeneralFixesChanged);
        }

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
        
        [Test]
        public void UnbalancedBrackets()
        {
            Dictionary<int, int> UnB = new Dictionary<int, int>();
            
            Article a = new Article("TestArticle", "This is the text (here.");
            UnB = a.UnbalancedBrackets();
            Assert.AreEqual(1, UnB.Count, "One unbalanced bracket in mainspace article");
            
            //   UnB.Clear();
            a = new Article("TestArticle", @"This is the text here.
== Section ==
There [was.");
            UnB = a.UnbalancedBrackets();
            Assert.AreEqual(1, UnB.Count, "Unbalanced bracket in mainspace article later sections found");
            
            // UnB.Clear();
            a = new Article("Talk:TestArticle", @"This is the text (here.
== Section ==
There [was.");
            UnB = a.UnbalancedBrackets();
            Assert.AreEqual(1, UnB.Count, "One unbalanced bracket in zeroth section of talk article");
            
            // UnB.Clear();
            a = new Article("Talk:TestArticle", @"This is the text here.
== Section ==
There [was.");
            UnB = a.UnbalancedBrackets();
            Assert.AreEqual(0, UnB.Count, "No unbalanced bracket in zeroth section of talk article");
        }

        [Test]
        public void ContainsComparers()
        {
            IArticleComparer containsComparer = ArticleComparerFactory.Create(@"foo\nbar",
                                                                              false,
                                                                              false,
                                                                              false, // singleline
                                                                              false); // multiline

            IArticleComparer notContainsComparer = ArticleComparerFactory.Create(@"foo\nbar",
                                                                                 false,
                                                                                 false,
                                                                                 false, // singleline
                                                                                 false); // multiline
            
            Article a = new Article("A", @"Now foo
bar");
            Assert.IsTrue(notContainsComparer.Matches(a),"does not contain comparer");
            Assert.IsTrue(containsComparer.Matches(a),"it contains comparer");
            
            a = new Article("A", @"Now foo-bar");
            Assert.IsFalse(notContainsComparer.Matches(a),"not contains is false");
            Assert.IsFalse(containsComparer.Matches(a),"contains is false");
        }

        [Test]
        public void Comparers()
        {
            Article a = new Article("A"), b = new Article("B"), z = new Article("Z"), one = new Article("1"), diacritic = new Article("Ș"),
                dollar = new Article("$");

            Assert.AreEqual(-1, a.CompareTo(b), "A just before B");
            Assert.AreEqual(0, a.CompareTo(a), "equal");
            Assert.AreEqual(1, b.CompareTo(a), "B just after A");
            Assert.AreEqual(-16, one.CompareTo(a), "1 before A");
            Assert.AreEqual(-446, z.CompareTo(diacritic), "Diacritics later than Latin");
            Assert.AreEqual(-487, one.CompareTo(diacritic), "Diacritics later than numbers");
            Assert.AreEqual(29, a.CompareTo(dollar), "Keyboard characters before Latin");
        }

        [Test]
        public void Unicodify()
        {
            Parsers Parser = new Parsers();
            HideText RemoveText = new HideText(false, true, false);
            Article a = new Article("a", @"'''test'''. z &amp; a&Dagger; &dagger;.

{{DEFAULTSORT:Hello test}}
[[Category:Test pages]]
");

            a.Unicodify(true, Parser, RemoveText);

            Assert.AreEqual(@"'''test'''. z & a‡ †.

{{DEFAULTSORT:Hello test}}
[[Category:Test pages]]
", a.ArticleText, "Text unicodified");

            a = new Article("a", @"'''test'''. z &amp; {{t|a&Dagger; &dagger;}}.

{{DEFAULTSORT:Hello test}}
[[Category:Test pages]]
");

            a.Unicodify(true, Parser, RemoveText);

            Assert.AreEqual(@"'''test'''. z & {{t|a&Dagger; &dagger;}}.

{{DEFAULTSORT:Hello test}}
[[Category:Test pages]]
", a.ArticleText, "Text unicodified, hidemore used");
            
            a = new Article("a", @"ABC");
            a.Unicodify(true, Parser, RemoveText);
            Assert.AreEqual(@"ABC", a.ArticleText, "No change");
        }
        
        [Test]
        public void BulletExternalLinks()
        {
            Article a = new Article("a", @"A
==External links==
http://www.site.com

[[Category:A]]");
            a.BulletExternalLinks(false);
            Assert.IsTrue(a.ArticleText.Contains("* http://www.site.com"));
            
             a = new Article("a", @"A
==External links==
http://www.site.com

[[Category:A]]");
            a.BulletExternalLinks(true);
            Assert.IsTrue(a.ArticleText.Contains("* http://www.site.com"));
            
            a = new Article("a", @"A
==External links==
* http://www.site.com

[[Category:A]]");
            a.BulletExternalLinks(true);
            Assert.IsTrue(a.ArticleText.Contains("* http://www.site.com"));
        }
        
        [Test]
        public void FixLinks()
        {
            Article a = new Article("a", @"A [[_B ]]");
            a.FixLinks(false);
            Assert.IsTrue(a.ArticleText.Contains("[[B]]"));
            
            a = new Article("a", @"A [[ B_]]");
            a.FixLinks(true);
            Assert.IsTrue(a.ArticleText.Contains("[[B]]"));
            
            a = new Article("a", @"A [[B]]");
            a.FixLinks(true);
            Assert.IsTrue(a.ArticleText.Contains("[[B]]"));
            
            a = new Article("a", @"A [[B]]");
            a.FixLinks(false);
            Assert.IsTrue(a.ArticleText.Contains("[[B]]"));
        }
        
        [Test]
        public void CiteTemplateDates()
        {
            Parsers Parser = new Parsers();
            Article a = new Article("a", @"A [[B]]");
            a.CiteTemplateDates(Parser, true);
            Assert.IsTrue(a.ArticleText.Contains("[[B]]"));
        }
        
        [Test]
        public void EmboldenTitles()
        {
            Parsers Parser = new Parsers();
            Article a = new Article("a", @"A [[B]]");
            a.EmboldenTitles(Parser, true);
            Assert.IsTrue(a.ArticleText.Contains("[[B]]"));
        }
    }

    [TestFixture]
    public class NamespaceTests : RequiresInitialization
    {
        [Test]
        public void Determine()
        {
            Assert.AreEqual(0, Namespace.Determine("test"));
            Assert.AreEqual(0, Namespace.Determine(" test "));
            Assert.AreEqual(0, Namespace.Determine(":test"));
            Assert.AreEqual(0, Namespace.Determine("test:test"));
            Assert.AreEqual(0, Namespace.Determine("My Project:Foo"));
            Assert.AreEqual(0, Namespace.Determine("Magic: The Gathering"));

            Assert.AreEqual(Namespace.User, Namespace.Determine("User:"));

            Assert.AreEqual(Namespace.Talk, Namespace.Determine("Talk:foo"));
            Assert.AreEqual(Namespace.Talk, Namespace.Determine("Talk%3Afoo"), "Handles URL encoded colon");
            Assert.AreEqual(Namespace.UserTalk, Namespace.Determine("User talk:bar"));

            Assert.AreEqual(Namespace.File, Namespace.Determine("File:foo"));
            Assert.AreEqual(Namespace.File, Namespace.Determine("Image:foo"));

            Assert.AreEqual(Namespace.Project, Namespace.Determine("Wikipedia:Foo"));
            Assert.AreEqual(Namespace.Project, Namespace.Determine("Project:Foo"));
            
            Assert.AreEqual(Namespace.Talk, Namespace.Determine(@"Talk:ʿAyn"), "handles pages with spacing modifier Unicode characters at start of name");
        }

        [Test]
        public void DetermineDeviations()
        {
            Assert.AreEqual(Namespace.File, Namespace.Determine("File : foo"));
            Assert.AreEqual(Namespace.File, Namespace.Determine("File :foo"));
            Assert.AreEqual(Namespace.File, Namespace.Determine("File: foo"));
            Assert.AreEqual(Namespace.File, Namespace.Determine("File :  foo"));
            Assert.AreEqual(Namespace.User, Namespace.Determine("user:foo"));
            Assert.AreEqual(Namespace.UserTalk, Namespace.Determine("user_talk:foo"));
            Assert.AreEqual(Namespace.UserTalk, Namespace.Determine("user%20talk:foo"));
        }
        
        [Test]
        public void OtherLanguages()
        {
            #if DEBUG
            Variables.SetProjectLangCode("fr");
            Variables.Namespaces[1] = "Discussion:";
            Assert.AreEqual(Namespace.Talk, Namespace.Determine(@"Discussion:foo"));
            Variables.SetProjectLangCode("en");
            Variables.Namespaces[1] = "Talk:";
            #endif
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
            
            Assert.AreEqual("user:", Namespace.Normalize("user :", 7), "only changes colon for incorrect namespace number");
            Assert.AreEqual("user:", Namespace.Normalize("user:", 7), "only changes colon for incorrect namespace number");
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

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_10#NullReferenceException_in_HideText.AddBackMore
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

        [Test]
        public void MoveBanners()
        {
            string a = @"{{Skip to talk}}", b = @"{{Talk header}}", c = @"{{GA nominee}}", d = @"{{Controversial}}", e = @"{{Not a forum}}", f = @"{{British English}}", g = @"{{FailedGA}}";
            string correct = a + "\r\n" + b + "\r\n" + c + "\r\n" + d + "\r\n" + e + "\r\n" + f + "\r\n" + g + "\r\n";
            string articleText = b + "\r\n"+ a + "\r\n"+ c + "\r\n"+ d + "\r\n"+ e + "\r\n" + f + g;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(correct, articleText,"bacdefg with newlines");

            articleText = a + b + c + d + e + f + g;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(correct, articleText,"abcdefg");

            articleText = b + a + c + d + e + f + g;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(correct, articleText,"bacdefg without newlines");

            articleText = a + c + b + d + e + f + g;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(correct, articleText,"acbdefg");

            articleText = a + b + c + e + d + f + g;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(correct, articleText,"abcedfg");

            articleText = a + e + c + b + d + f + g;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(correct, articleText,"aecbdfg");

            articleText = a + e + c + b + d + g + f;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(correct, articleText, "aecbdgf");

            articleText = a + c + d + e + f + b + g;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(correct, articleText,"acdefbg");

            articleText = f + a + c + b + d + e + g;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(correct, articleText,"facbdeg");

            articleText = f + e + d + c + b + a + g;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(correct, articleText,"fedcbag");

            string h=@"{{WikiProjectBannerShell|1={{WikiProject Greece|class=}}}}", i=@"{{Image requested}}", j=@"{{Connected contributor|John Doe}}";

			correct = correct + h + "\r\n";
			articleText = correct;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(correct, articleText,"abcdefgh");

			correct = correct + i + "\r\n";

			articleText = correct;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(correct, articleText,"abcdefghi");

            articleText = b + "\r\n" + a + "\r\n" + c + "\r\n" + d + "\r\n" + e + "\r\n" + f + "\r\n" + h + "\r\n" + g + "\r\n" + i;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(correct, articleText,"bacdefhgi");

			correct = correct + j + "\r\n";

			articleText = correct;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(correct, articleText,"abcdefghij");

            articleText = b + "\r\n" + a + "\r\n" + c + "\r\n" + d + "\r\n" + e + "\r\n" + f + "\r\n" + h + "\r\n" + g + "\r\n" + j + "\r\n" + i;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(correct, articleText,"bacdefhgji");

            articleText = b + "\r\n" + a + "\r\n" + c + "\r\n" + d + "\r\n" + e + "\r\n" + f + "\r\n" + h + "\r\n" + g + "\r\n" + j + "\r\n" + i + "\r\n" + "\r\n" + "\r\n";
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(correct, articleText,"bacdefhgji with newlines at the end");

            articleText = b + "\r\n" + "\r\n" + "\r\n" + a + "\r\n" + c + "\r\n" + d + "\r\n" + e + "\r\n" + f + "\r\n" + h + "\r\n" + g + "\r\n" + j + "\r\n" + "\r\n" + "\r\n" + i + "\r\n" + "\r\n" + "\r\n";

            string k=@"{{To do}}", l=@"{{Maintained|[[User:Foo|bar]]}}", m=@"{{Find sources notice}}", n=@"{{Split from|foo}}";

			correct = correct + k + "\r\n";
			articleText = correct;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(correct, articleText,"abcdefghijk");

            articleText = b + "\r\n" + a + "\r\n" + c + "\r\n" + d + "\r\n" + e + "\r\n" + f + "\r\n" + k + "\r\n" + g + "\r\n" + j + "\r\n" + i + "\r\n" + h;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(correct, articleText,"bacdefkgjih");

            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(correct, articleText,"bacdefhgji with newlines at the end and in the middle");

            correct = correct + l + "\r\n";
            articleText = correct;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(correct, articleText, "abcdefghijkl");

            articleText = b + "\r\n" + a + "\r\n" + c + "\r\n" + d + "\r\n" + e + "\r\n" + f + "\r\n" + k + "\r\n" + g + "\r\n" + j + "\r\n" + l + "\r\n" + i + "\r\n" + h;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(correct, articleText, "bacdefkgjlih");

            correct = correct + m + "\r\n" + n + "\r\n";
            articleText = correct;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(correct, articleText, "abcdefghijklmn");

            articleText = b + "\r\n" + a + "\r\n" + c + "\r\n" + d + "\r\n" + e + "\r\n" + f + "\r\n" + k + "\r\n" + n + "\r\n" + g + "\r\n" + j + "\r\n" + m + "\r\n" + l + "\r\n" + i + "\r\n" + h;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(correct, articleText, "bacdefkngjmlih");

            articleText = @"{{some other template}}" + "\r\n" + @"==Untitled==" + "\r\n" + @"some text";
            string articleText2 = articleText;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(articleText2, articleText,"no changes if nothing is detected; starts with template");

            articleText = @"{{GA|21:12, 11 May 2013 (UTC)|topic=Geography|page=1|oldid=554646767}}
{{DYK talk|17 April|2013|entry=... that Canada's '''[[Glacier National Park (Canada)|Glacier National Park]]''' ''(pictured)'' contains [[moonmilk]]?}}
{{WikiProjectBannerShell|1=
{{WikiProject Canada|geography=yes|class=Start|importance=Mid|bc=yes}}
{{WikiProject Protected areas|class=Start|importance=Mid}}
{{WikiProject Greece|class=Start}}
}}
{{some random template}}";
            articleText2 = articleText;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(articleText2, articleText,"no changes if everything is in place; contains WPBS");

            articleText = @"{{GA|21:12, 11 May 2013 (UTC)|topic=Geography|page=1|oldid=554646767}}
{{DYK talk|17 April|2013|entry=... that Canada's '''[[Glacier National Park (Canada)|Glacier National Park]]''' ''(pictured)'' contains [[moonmilk]]?}}
{{WikiProject Canada|geography=yes|class=Start|importance=Mid|bc=yes}}
{{WikiProject Protected areas|class=Start|importance=Mid}}
{{some random template}}";
            articleText2 = articleText;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(articleText2, articleText,"no changes if everything is in place; contains 2 Wikiprojects");

        }
         [Test]
        public void MoveBannersAndWikiProjects()
        {
            string a = @"{{Skip to talk}}", b = @"{{Talk header}}", c = @"{{GA nominee}}", d = @"{{Controversial}}", e = @"{{Not a forum}}", f = @"{{British English}}", g = @"{{FailedGA}}", h=@"{{WikiProject Greece|class=}}", i=@"{{Image requested}}", j=@"{{Connected contributor|John Doe}}";
            string correct = a + "\r\n" + b + "\r\n" + c + "\r\n" + d + "\r\n" + e + "\r\n" + f + "\r\n" + g + "\r\n"+ h + "\r\n" + i + "\r\n";
            string articleText = correct;

            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(correct, articleText,"abcdefghi");

            articleText = b + "\r\n" + a + "\r\n" + c + "\r\n" + d + "\r\n" + e + "\r\n" + f + "\r\n" + h + "\r\n" + g + "\r\n" + i;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(correct, articleText,"bacdefhgi");

            articleText = h + "\r\n" + c + "\r\n" + a + "\r\n" + d + "\r\n" + e + "\r\n" + f + "\r\n" + b + "\r\n" + g + "\r\n" + i;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(correct, articleText,"hcadefbgi");

            articleText = d + "\r\n" + c + "\r\n" + a + "\r\n" + h + "\r\n" + f + "\r\n" + e + "\r\n" + b + "\r\n" + i + "\r\n" + g;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(correct, articleText,"dcahfebig");

            correct = correct + j + "\r\n";

			articleText = correct;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(correct, articleText,"abcdefghij");

            articleText = b + "\r\n" + a + "\r\n" + c + "\r\n" + d + "\r\n" + e + "\r\n" + f + "\r\n" + h + "\r\n" + g + "\r\n" + j + "\r\n" + i;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(correct, articleText,"bacdefhgji");

            articleText = b + "\r\n" + a + "\r\n" + c + "\r\n" + d + "\r\n" + e + "\r\n" + f + "\r\n" + h + "\r\n" + g + "\r\n" + j + "\r\n" + i + "\r\n" + "\r\n" + "\r\n";
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(correct, articleText,"bacdefhgji with newlines at the end");

            articleText = b + "\r\n" + "\r\n" + "\r\n" + a + "\r\n" + c + "\r\n" + d + "\r\n" + e + "\r\n" + f + "\r\n" + h + "\r\n" + g + "\r\n" + j + "\r\n" + "\r\n" + "\r\n" + i + "\r\n" + "\r\n" + "\r\n";

            string k=@"{{To do}}", l=@"{{Maintained|[[User:Foo|bar]]}}";

			correct = correct + k + "\r\n";
			articleText = correct;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(correct, articleText,"abcdefghijk");

            articleText = b + "\r\n" + a + "\r\n" + c + "\r\n" + d + "\r\n" + e + "\r\n" + f + "\r\n" + k + "\r\n" + g + "\r\n" + j + "\r\n" + i + "\r\n" + h;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(correct, articleText,"bacdefkgjih");

            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(correct, articleText,"bacdefhgji with newlines at the end and in the middle");

            correct = correct + l + "\r\n";
            articleText = correct;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(correct, articleText, "abcdefghijkl");

            articleText = b + "\r\n" + a + "\r\n" + c + "\r\n" + d + "\r\n" + e + "\r\n" + f + "\r\n" + k + "\r\n" + g + "\r\n" + j + "\r\n" + l + "\r\n" + i + "\r\n" + h;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(correct, articleText, "bacdefkgjlih");

        }

        [Test]
        public void MoveBannersAndSpacing()
        {
            string articleText = @"==Untitled==" + "\r\n" + @"some text";
            string articleText2 = articleText;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(articleText2, articleText,"no changes if nothing is detected; starts with header");
            
            articleText = @"{{afd-merged-from|George Piggins Medal|George Piggins Medal|10 June 2012}}

{{User:WildBot/m04|sect={{User:WildBot/m03|1|History of rugby league#Professional rugby begins in Australia|schism}}|m04}}
{{ArticleHistory
|action1=FAC

|currentstatus=GA
|topic=Everydaylife}}
{{WikiProjectBannerShell|1=
{{WikiProject Rugby league|class=GA|importance=High}}
}}

== older entries ==
The";

            articleText2 = @"{{ArticleHistory
|action1=FAC

|currentstatus=GA
|topic=Everydaylife}}
{{afd-merged-from|George Piggins Medal|George Piggins Medal|10 June 2012}}
{{WikiProjectBannerShell|1=
{{WikiProject Rugby league|class=GA|importance=High}}
}}

{{User:WildBot/m04|sect={{User:WildBot/m03|1|History of rugby league#Professional rugby begins in Australia|schism}}|m04}}

== older entries ==
The";
          	TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(articleText2, articleText,"unknown templates get below known ones");

			articleText = @"{{Talk header}}
{{WikiProjectBannerShell|1=
{{WikiProject Architecture|class=Start |importance=Low }}
{{WikiProject Virginia|class=Start |importance=Low }}
{{WikiProject National Register of Historic Places|class=start|importance=low}}
{{Image requested|in=Virginia}}
}}";
			articleText2 = @"{{Talk header}}
{{WikiProjectBannerShell|1=
{{WikiProject Architecture|class=Start |importance=Low }}
{{WikiProject Virginia|class=Start |importance=Low }}
{{WikiProject National Register of Historic Places|class=start|importance=low}}
}}
{{Image requested|in=Virginia}}
";
          	TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(articleText2, articleText,"image requested is moved away from WPBS; at some point we could fix the whitespace inside WPBS");
			
            articleText = @"{{Talk header|search=yes}}

==Random header==
Some text";
            articleText2 = articleText;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(articleText2, articleText,"no changes if nothing is detected; starts with talk header");

            articleText = @"{{Not a forum}}

==Random header==
Some text";
            articleText2 = articleText;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(articleText2, articleText,"no changes if nothing is detected; starts with not a forum");

            articleText = @"{{Random template}}

==Random header==
Some text";
            articleText2 = articleText;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(articleText2, articleText,"no changes if nothing is detected; starts with a random template");

            articleText = @"{{Random template}}
{{Yet another random template}}

==Random header==
Some text";
            articleText2 = articleText;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(articleText2, articleText,"no changes if nothing is detected; starts with two random templates");

            articleText = @"{{Skip to talk}}
{{Talk header}}
{{Random template}}

==Random header==
Some text";
            articleText2 = articleText;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(articleText2, articleText,"no changes if nothing is detected; starts with two skt, th and a random template");

            articleText = @"{{Skip to talk}}
{{Not a forum}}
{{Random template}}

==Random header==
Some text";
            articleText2 = articleText;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(articleText2, articleText,"no changes if nothing is detected; starts with skt, naf and a random template");

            articleText = @"{{Not a forum}}
{{Skip to talk}}
{{Random template}}

==Random header==
Some text";
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(articleText2, articleText,"moves skip to talk on the top 1");


            articleText = @"{{Talk header}}
{{Skip to talk}}
{{Random template}}

==Random header==
Some text";
            articleText2 = @"{{Skip to talk}}
{{Talk header}}
{{Random template}}

==Random header==
Some text";
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(articleText2, articleText,"moves skip to talk on the top 2");

            articleText = @"{{Talk header}}
{{WikiProjectBannerShell|1=
{{WikiProject Iraq|class=C|importance=mid}}
{{WikiProject Crime|class=C|importance=low}}
{{WikiProject Terrorism|class=C|importance=low}}
}}
{{image requested}}

== Reliable sources ==";
            articleText2 = articleText;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(articleText2, articleText, "No change, heading whitespace not affected");
        }

        [Test]
        public void MoveTalkHeader()
        {
            string talkheader = @"{{talk header|noarchive=no|search=no|arpol=no|wp=no|disclaimer=no|shortcut1|shortcut2}}", talkrest = @"==hello==
hello talk";
            string articleText = talkrest + "\r\n" + talkheader;
            
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            
            Assert.AreEqual(talkheader.Replace("{{talk", @"{{Talk") + "\r\n" + talkrest + "\r\n", articleText,"move talk header");
            
            // handles {{talk header}} on same line as other template
            string WPBS = @"{{WikiProjectBannerShell|blp=yes|1=
{{OH-Project|class=B|importance=Low|nested=yes}}
{{WPBiography|living=yes|class=B|priority=Low|filmbio-work-group=yes|nested=yes|listas=Parker, Sarah Jessica}}
{{WikiProject Cincinnati|class=B|importance=mid|nested=yes}}
}}", rest = "\r\n\r\n" +  @"==Song Jessie by Joshua Kadison==
In the article it says that above mentioned";
            articleText = WPBS + @"{{Talkheader}}" + rest;
            
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            
            Assert.AreEqual(@"{{Talk header}}" + "\r\n" + WPBS + rest, articleText,"talk header on same line as other template");
            
            // no change if already at top
            articleText = talkheader + "\r\n" + talkrest;
            
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(talkheader + "\r\n" + talkrest, articleText,"talk header already on top");
            
            // no change if no talk header
            articleText = talkrest;
            
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(talkrest, articleText,"no talk header");

            talkheader = @"{{Talk header}}";
            talkrest = @"==hello==
hello talk";
            articleText = talkrest + "\r\n" + talkheader + "\r\n{{GA nominee}}";

            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);

            Assert.AreEqual(talkheader + "\r\n" + "{{GA nominee}}\r\n" + talkrest + "\r\n", articleText,"GA nominee involved");
        }
        
        [Test]
        public void RenameTalkHeader()
        {
            string talkheader = @"{{talkheader|noarchive=no}}", talkrest = @"==hello==
hello talk";
            string articleText = talkrest + "\r\n" + talkheader;
            
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            
            Assert.AreEqual(@"{{Talk header|noarchive=no}}" + "\r\n" + talkrest+ "\r\n", articleText, "renamed to upper case with space");
        }
        
        [Test]
        public void TalkHeaderDefaultsortChange()
        {
            string start = @"
==Foo==
bar", df = @"{{DEFAULTSORT:Bert}}";
            
            string articleText = start + "\r\n" + df;
            
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.MoveToBottom);
            Assert.AreEqual(start + "\r\n"+ "\r\n" + df, articleText, "removes second newline");
            
            articleText = start + df;
            
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.MoveToTop);
            Assert.AreEqual(df + "\r\n" + start, articleText, "moves df after text");
            
            articleText = start + df;
            
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(start + df, articleText, "no changes");
            
            string df2 = @"{{DEFAULTSORT:}}";
            
            articleText = start + df2;
            
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.MoveToBottom);
            Assert.AreEqual(start, articleText, "defaultsort with no key removed");
            
            articleText = start + df2;
            
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.MoveToTop);
            Assert.AreEqual(start, articleText, "defaultsort with no key removed");
        }
        
        [Test]
        public void SkipToTalk()
        {
            string articleText = @"{{Skiptotoc}}", STT = @"{{Skip to talk}}";
            
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(STT + "\r\n", articleText);
            
            articleText = @"{{skiptotoctalk}}";
            
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.AreEqual(STT + "\r\n", articleText);
        }
        
        [Test]
        public void AddMissingFirstCommentHeader()
        {
            const string comment = @"
Hello world comment.";
            string articleTextIn = articleTextHeader + comment;
            
            // plain comment
            TalkPageFixes.ProcessTalkPage(ref articleTextIn, DEFAULTSORT.NoChange);
            
            Assert.AreEqual(articleTextIn, articleTextHeader + "\r\n" + @"
==Untitled==
Hello world comment.");
            
            // idented comment2
            articleTextIn = articleTextHeader + @"
*Hello world comment2.";
            
            TalkPageFixes.ProcessTalkPage(ref articleTextIn, DEFAULTSORT.NoChange);
            
            Assert.AreEqual(articleTextIn, articleTextHeader +"\r\n" + @"
==Untitled==
*Hello world comment2.");
            
            // idented comment3
            articleTextIn = articleTextHeader + @"
:Hello world comment3.";
            
            TalkPageFixes.ProcessTalkPage(ref articleTextIn, DEFAULTSORT.NoChange);
            
            Assert.AreEqual(articleTextIn, articleTextHeader + "\r\n" + @"
==Untitled==
:Hello world comment3.");
            
            // quoted comment
            articleTextIn = articleTextHeader + @"
""Hello world comment4"".";
            
            TalkPageFixes.ProcessTalkPage(ref articleTextIn, DEFAULTSORT.NoChange);
            
            Assert.AreEqual(articleTextIn, articleTextHeader + "\r\n" + @"
==Untitled==
""Hello world comment4"".");
            
            // heading level 3 changed to level 2
            articleTextIn = articleTextHeader + "\r\n" + @"===Foo bar===
*Hello world comment2.";
            
            TalkPageFixes.ProcessTalkPage(ref articleTextIn, DEFAULTSORT.NoChange);
            
            Assert.IsTrue(articleTextIn.Contains(@"==Foo bar=="));
            Assert.IsFalse(articleTextIn.Contains(@"==Untitled=="));
            
            articleTextIn = comment;

            TalkPageFixes.ProcessTalkPage(ref articleTextIn, DEFAULTSORT.NoChange);
            
            Assert.AreEqual(articleTextIn, @"==Untitled==
Hello world comment.","don't add blank line when header is on the top");

            articleTextIn = @"{{Football}}" +"\r\n" + comment;

            TalkPageFixes.ProcessTalkPage(ref articleTextIn, DEFAULTSORT.NoChange);
            
            Assert.AreEqual(articleTextIn, @"{{Football}}

==Untitled==
Hello world comment.","add header between template and text");

        }
        
        [Test]
        public void AddMissingFirstCommentHeaderNoChanges()
        {
            // no change – header already
            string articleTextIn = articleTextHeader + @"
==Question==
:Hello world comment3.";
            
            
            TalkPageFixes.ProcessTalkPage(ref articleTextIn, DEFAULTSORT.NoChange);
            
            Assert.AreEqual(articleTextIn, articleTextHeader + @"
==Question==
:Hello world comment3.");
            
            // no change – already header at top
            articleTextIn = @"
{{Some template}}
==Question==
:Hello world comment3.";
            
            
            TalkPageFixes.ProcessTalkPage(ref articleTextIn, DEFAULTSORT.NoChange);
            
            Assert.AreEqual(@"
{{Some template}}
==Question==
:Hello world comment3.", articleTextIn);
            
            // no change – already header at top 2
            articleTextIn = @"
==Question==
{{Some template}}
:Hello world comment3.";
            
            TalkPageFixes.ProcessTalkPage(ref articleTextIn, DEFAULTSORT.NoChange);
            
            Assert.AreEqual(@"
==Question==
{{Some template}}
:Hello world comment3.", articleTextIn);
            
            // no change – no comments
            articleTextIn = @"
==Question==
{{Some template}}";
            
            TalkPageFixes.ProcessTalkPage(ref articleTextIn, DEFAULTSORT.NoChange);
            
            Assert.AreEqual(@"
==Question==
{{Some template}}", articleTextIn);
            
            // no change – only text in template
            articleTextIn = @"
{{foo|
bar|
end}}";
            
            TalkPageFixes.ProcessTalkPage(ref articleTextIn, DEFAULTSORT.NoChange);
            
            Assert.AreEqual(@"
{{foo|
bar|
end}}", articleTextIn);
            
            // no change – only comments
            articleTextIn = @"
<!--
foo
-->";
            
            TalkPageFixes.ProcessTalkPage(ref articleTextIn, DEFAULTSORT.NoChange);
            
            Assert.AreEqual(@"
<!--
foo
-->", articleTextIn);
            
            // no change – only TOC
            articleTextIn = @"
__TOC__";
            
            TalkPageFixes.ProcessTalkPage(ref articleTextIn, DEFAULTSORT.NoChange);
            
            Assert.AreEqual(@"
__TOC__", articleTextIn);
            
            // no change -- only in template
            const string allInTemplate = @"{{archive box|
*[[/Archive: GA review|Good Article review]]}}

== Explanation of Wright's work in ''Certaine Errors'' ==";
            
            articleTextIn = allInTemplate;
            
            TalkPageFixes.ProcessTalkPage(ref articleTextIn, DEFAULTSORT.NoChange);
            
            Assert.AreEqual(allInTemplate, articleTextIn);
            
            // no change -- only after template on same line
            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs#Section_header_added_in_wrong_position
            const string allAfterTemplate = @"{{archive box|words}} extra foo

{{another one}}";
            
            articleTextIn = allAfterTemplate;
            
            TalkPageFixes.ProcessTalkPage(ref articleTextIn, DEFAULTSORT.NoChange);
            
            Assert.AreEqual(allAfterTemplate, articleTextIn);

            // no change – only text in gallery tags
            articleTextIn = @"
<gallery>
File:Example.jpg|Caption1
File:Example.jpg|Caption2
</gallery>";

            TalkPageFixes.ProcessTalkPage(ref articleTextIn, DEFAULTSORT.NoChange);

            Assert.AreEqual(@"
<gallery>
File:Example.jpg|Caption1
File:Example.jpg|Caption2
</gallery>", articleTextIn);
        }
        
        [Test]
        public void WikiProjectBannerShellRedirects()
        {
            string red1 = @"{{WPBS}}", red2=@"{{Wikiprojectbannershell}}", red3=@"{{wpb}}", red4 = @"{{WPB}}",

            WikiProjectBannerShell = @"{{WikiProjectBannerShell}}",
            WikiProjectBanners = @"{{WikiProjectBanners}}";
            
            Assert.AreEqual(WikiProjectBannerShell, TalkPageFixes.WikiProjectBannerShell(red1));
            Assert.AreEqual(WikiProjectBannerShell, TalkPageFixes.WikiProjectBannerShell(red2));
            Assert.AreEqual(WikiProjectBannerShell, TalkPageFixes.WikiProjectBannerShell(WikiProjectBannerShell));
            Assert.AreNotEqual(WikiProjectBannerShell, TalkPageFixes.WikiProjectBannerShell(red3));
            Assert.AreEqual(WikiProjectBanners, TalkPageFixes.WikiProjectBannerShell(red3));
            Assert.AreEqual(WikiProjectBanners, TalkPageFixes.WikiProjectBannerShell(red4));
            Assert.AreEqual(WikiProjectBanners, TalkPageFixes.WikiProjectBannerShell(WikiProjectBanners));
            Assert.AreNotEqual(WikiProjectBanners, TalkPageFixes.WikiProjectBannerShell(WikiProjectBannerShell));
        }
        
        [Test]
        public void WikiProjectBannerShellEnOnly()
        {
            #if DEBUG
            Variables.SetProjectLangCode("fr");
            Assert.AreEqual(@"{{WikiProjectBannerShell|blp=yes|blp=yes}}", TalkPageFixes.WikiProjectBannerShell(@"{{WikiProjectBannerShell|blp=yes|blp=yes}}"));
            Variables.SetProjectLangCode("en");
            Assert.AreEqual(@"{{WikiProjectBannerShell|blp=yes}}", TalkPageFixes.WikiProjectBannerShell(@"{{WikiProjectBannerShell|blp=yes|blp=yes}}"));
            #endif
        }
        
        [Test]
        public void WikiProjectBannerShellDupeParameters()
        {
            Assert.AreEqual(@"{{WikiProjectBannerShell|blp=yes}}", TalkPageFixes.WikiProjectBannerShell(@"{{WikiProjectBannerShell|blp=yes|blp=yes}}"));
            Assert.AreEqual(@"{{WikiProjectBannerShell|blpo=yes|blp=yes}}", TalkPageFixes.WikiProjectBannerShell(@"{{WikiProjectBannerShell|blpo=yes|blpo=yes|blp=yes|blp=yes}}"));
            Assert.AreEqual(@"{{WikiProjectBannerShell|collapsed=yes}}", TalkPageFixes.WikiProjectBannerShell(@"{{WikiProjectBannerShell|collapsed=yes|collapsed=yes|collapsed=yes}}"));
            Assert.AreEqual(@"{{WikiProjectBannerShell|collapsed=yes}}", TalkPageFixes.WikiProjectBannerShell(@"{{WikiProjectBannerShell||collapsed=yes}}"), "excess pipes cleaned");
        }
        
        [Test]
        public void WikiProjectBannerShellUnneededParams()
        {
            Assert.AreEqual(@"{{WikiProjectBannerShell}}", TalkPageFixes.WikiProjectBannerShell(@"{{WikiProjectBannerShell|blp=no|activepol=no|collapsed=no|blpo=no}}"));
            Assert.AreEqual(@"{{WikiProjectBannerShell|collapsed=yes}}", TalkPageFixes.WikiProjectBannerShell(@"{{WikiProjectBannerShell|blp=no|activepol=no|collapsed=yes|blpo=no}}"));
            Assert.AreEqual(@"{{WikiProjectBanners|collapsed=no}}", TalkPageFixes.WikiProjectBannerShell(@"{{WikiProjectBanners|blp=no|activepol=no|collapsed=no|blpo=no}}"));
            Assert.AreEqual(@"{{WikiProjectBanners}}", TalkPageFixes.WikiProjectBannerShell(@"{{WikiProjectBanners|blp=no|activepol=no|collapsed=yes|blpo=no}}"));
        }
        
        [Test]
        public void WikiProjectBannerShellWPBiography()
        {
            Assert.AreEqual(@"{{WikiProjectBannerShell|1={{WPBiography|foo=bar}}}}", TalkPageFixes.WikiProjectBannerShell(@"{{WikiProjectBannerShell|1={{WPBiography|foo=bar}}}}"));
            
            Assert.AreEqual(@"{{WikiProjectBannerShell|blp=yes|1={{WPBiography|foo=bar|living=yes}}}}", TalkPageFixes.WikiProjectBannerShell(@"{{WikiProjectBannerShell|blp=|1={{WPBiography|foo=bar|living=yes}}}}"),"appends blp=yes to WPBS");
            Assert.AreEqual(@"{{WikiProjectBannerShell|blp=yes|1={{WikiProject Biography|foo=bar|living=yes}}}}", TalkPageFixes.WikiProjectBannerShell(@"{{WikiProjectBannerShell|blp=|1={{WikiProject Biography|foo=bar|living=yes}}}}"),"appends blp=yes to WPBS");
            Assert.AreEqual(@"{{WikiProjectBannerShell|activepol=yes|1={{WPBiography|foo=bar|activepol=yes}}}}", TalkPageFixes.WikiProjectBannerShell(@"{{WikiProjectBannerShell|activepol=abc|1={{WPBiography|foo=bar|activepol=yes}}}}"),"ignores invalid values");
            Assert.AreEqual(@"{{WikiProjectBannerShell|blpo=yes|1={{WPBiography|foo=bar|blpo=yes}}}}", TalkPageFixes.WikiProjectBannerShell(@"{{WikiProjectBannerShell|blpo=|1={{WPBiography|foo=bar|blpo=yes}}}}"),"appends blpo=yes to WPBS");
            Assert.AreEqual(@"{{WikiProjectBannerShell|blpo=|1={{WPBiography|foo=bar|blpo=no}}}}", TalkPageFixes.WikiProjectBannerShell(@"{{WikiProjectBannerShell|blpo=|1={{WPBiography|foo=bar|blpo=no}}}}"));
            
            Assert.AreEqual(@"{{WikiProjectBannerShell|1={{WPBiography|foo=bar|living=no}}}}", TalkPageFixes.WikiProjectBannerShell(@"{{WikiProjectBannerShell|blp=yes|1={{WPBiography|foo=bar|living=no}}}}"));
        }
        
        [Test]
        public void WikiProjectBannerShellAddingWikiProjects()
        {
            Assert.AreEqual(@"{{WikiProjectBannerShell|1={{WPBiography|foo=bar}}
{{WikiProject foo}}}}
", TalkPageFixes.WikiProjectBannerShell(@"{{WikiProjectBannerShell|1={{WPBiography|foo=bar}}}}
{{WikiProject foo}}"), "WikiProjects pulled into WPBS");

            Assert.AreEqual(@"{{WikiProjectBannerShell|1={{WPBiography|foo=bar}}
{{WikiProject foo}}
{{WikiProject bar}}}}

", TalkPageFixes.WikiProjectBannerShell(@"{{WikiProjectBannerShell|1={{WPBiography|foo=bar}}}}
{{WikiProject foo}}
{{WikiProject bar}}"), "WikiProjects pulled into WPBS");

            Assert.AreEqual(@"{{WikiProjectBannerShell|1={{WikiProject foo}}
{{WikiProject bar}}
{{WikiProject Biography|living=yes|foo=bar}} | blp=yes}}", TalkPageFixes.WikiProjectBannerShell(@"{{WikiProject Biography|living=yes|foo=bar}}{{WikiProjectBannerShell|1={{WikiProject foo}}
{{WikiProject bar}}}}"), "WikiProjects pulled into WPBS, WPBIO contains living=yes");

            Assert.AreEqual(@"{{WikiProjectBannerShell|1={{WikiProject foo}}
{{WikiProject bar}}
{{WikiProject Biography|activepol=yes|foo=bar}} | activepol=yes}}", TalkPageFixes.WikiProjectBannerShell(@"{{WikiProject Biography|activepol=yes|foo=bar}}{{WikiProjectBannerShell|1={{WikiProject foo}}
{{WikiProject bar}}}}"), "WikiProjects pulled into WPBS, WPBIO contains activepol=yes");

            Assert.AreEqual(@"{{WikiProjectBannerShell|1={{WikiProject foo}}
{{WikiProject bar}}
{{WikiProject Biography|living=yes|activepol=yes|foo=bar}} | blp=yes | activepol=yes}}", TalkPageFixes.WikiProjectBannerShell(@"{{WikiProject Biography|living=yes|activepol=yes|foo=bar}}{{WikiProjectBannerShell|1={{WikiProject foo}}
{{WikiProject bar}}}}"), "WikiProjects pulled into WPBS, WPBIO contains living, activepol=yes");

            Assert.AreEqual(@"{{WikiProjectBannerShell|1={{WPBiography|foo=bar}}
{{WikiProject bar}}
{{WikiProject foo}}}}
", TalkPageFixes.WikiProjectBannerShell(@"{{WikiProject bar}}

{{WikiProjectBannerShell|1={{WPBiography|foo=bar}}}}
{{WikiProject foo}}"), "WikiProjects pulled into WPBS, no excess whitespace left");
        }
        
        [Test]
        public void WikiProjectBannerShellUnnamedParam()
        {
            Assert.AreEqual(@"{{WikiProjectBannerShell|1={{WPBiography|foo=bar}}}}", TalkPageFixes.WikiProjectBannerShell(@"{{WikiProjectBannerShell|{{WPBiography|foo=bar}}}}"), "1= added when missing");
            Assert.AreEqual(@"{{WikiProjectBannerShell|1=
{{WPBiography|foo=bar}}}}", TalkPageFixes.WikiProjectBannerShell(@"{{WikiProjectBannerShell|
{{WPBiography|foo=bar}}}}"));
            
            const string otherUnnamed = @"{{WikiProjectBannerShell|random}}";
            Assert.AreEqual(otherUnnamed, TalkPageFixes.WikiProjectBannerShell(otherUnnamed), "other unknown parameter not named 1=");
        }
        
        [Test]
        public void WikiProjectBannerShellBLP()
        {
            const string a = @"{{WikiProjectBannerShell|blp=yes|1={{WPBiography|foo=bar|living=yes}}}}";
            
            Assert.AreEqual(a, TalkPageFixes.WikiProjectBannerShell(a + "{{Blp}}"),"removes redundant banner");
            Assert.AreEqual(a, TalkPageFixes.WikiProjectBannerShell(a.Replace("blp=yes", "blp=") + "{{Blp}}"),"empty parameter in WPBS");
            Assert.AreEqual("{{Blp}}", TalkPageFixes.WikiProjectBannerShell("{{Blp}}"));
        }
        
        [Test]
        public void WikiProjectBannerShellBLPO()
        {
            const string a = @"{{WikiProjectBannerShell|blpo=yes|1={{WPBiography|foo=bar}}}}";
            
            Assert.AreEqual(a, TalkPageFixes.WikiProjectBannerShell(a + "{{Blpo}}"),"removes redundant banner");
            Assert.AreEqual(a, TalkPageFixes.WikiProjectBannerShell(a + "{{BLPO}}"),"removes redundant banner");
            Assert.AreEqual(a, TalkPageFixes.WikiProjectBannerShell(a + "{{BLP others}}"),"removes redundant banner");
            Assert.AreEqual(a, TalkPageFixes.WikiProjectBannerShell(a.Replace("blpo=yes", "blpo=") + "{{Blpo}}"),"empty parameter in WPBS");
            Assert.AreEqual("{{Blpo}}", TalkPageFixes.WikiProjectBannerShell("{{Blpo}}"));
        }
        
        [Test]
        public void WikiProjectBannerShellActivepol()
        {
            const string a = @"{{WikiProjectBannerShell|activepol=yes|1={{WPBiography|foo=bar|activepol=yes}}}}";
            
            Assert.AreEqual(a, TalkPageFixes.WikiProjectBannerShell(a + "{{Activepol}}"),"removes redundant banner");
            Assert.AreEqual(a, TalkPageFixes.WikiProjectBannerShell(a.Replace("activepol=yes|", "activepol=|") + "{{activepol}}"),"empty parameter in WPBS");
            Assert.AreEqual("{{activepol}}", TalkPageFixes.WikiProjectBannerShell("{{activepol}}"));
        }
        
        [Test]
        public void WikiProjectBannerShellMisc()
        {
            const string a = @"{{wpbs|1=|banner collapsed=no|
{{WPBiography|living=yes|class=Start|priority=|listas=Hill, A}}
{{WikiProject Gender Studies}}
{{WikiProject Oklahoma}}
}}", b = @"{{WikiProjectBannerShell|banner collapsed=no|1=
{{WPBiography|living=yes|class=Start|priority=|listas=Hill, A}}
{{WikiProject Gender Studies}}
{{WikiProject Oklahoma}}
| blp=yes
}}";
            Assert.AreEqual(b, TalkPageFixes.WikiProjectBannerShell(a));
        }
        
        [Test]
        public void AddWikiProjectBannerShell()
        {
            const string a = @"{{WikiProject a|text}}", b = @"{{WikiProject b|text}}", c = @"{{WikiProject c|text}}", d = @"{{WikiProject d|text}}";
            Assert.AreEqual(a, TalkPageFixes.WikiProjectBannerShell(a));
            Assert.AreEqual(a + b, TalkPageFixes.WikiProjectBannerShell(a + b));
            Assert.AreEqual(@"{{WikiProjectBannerShell|1=" + "\r\n" + a + "\r\n" + b + "\r\n" + c + "\r\n" + @"}}", TalkPageFixes.WikiProjectBannerShell(a + b + c), "banner shell added for 3 or more wikiproject links");
            Assert.AreEqual(@"{{WikiProjectBannerShell|1=" + "\r\n" + a + "\r\n" + b + "\r\n" + c + "\r\n" + d + "\r\n" + @"}}", TalkPageFixes.WikiProjectBannerShell(a + b + c + d));
            
            const string e = @"{{talk header}}
{{WikiProject Biography|listas=Bar, F|living=yes|class=Start|priority=mid|sports-work-group=yes}}
{{WikiProject F|class=Start|importance=mid|italy=y}}
{{WikiProject X|class=Start|importance=Mid}}
{{WikiProject D|class=Start|importance=Low}}", f = @"{{WikiProjectBannerShell|1=
{{WikiProject Biography|listas=Bar, F|living=yes|class=Start|priority=mid|sports-work-group=yes}}
{{WikiProject F|class=Start|importance=mid|italy=y}}
{{WikiProject X|class=Start|importance=Mid}}
{{WikiProject D|class=Start|importance=Low}}
| blp=yes
}}{{talk header}}

";
            
            Assert.AreEqual(f, TalkPageFixes.WikiProjectBannerShell(e), "adds WPBS, ignores non-wikiproject templates");
        }
        
        [Test]
        public void RemoveTemplateNamespace()
        {
            string T1 = @"{{Template:Foo}}", T2 = @"{{Template:Foo}}
==Section==
{{Template:Bar}}";
            
            Assert.IsFalse(TalkPageFixes.ProcessTalkPage(ref T1, DEFAULTSORT.NoChange));
            Assert.AreEqual("{{Foo}}", T1, "template namespace removed");
            
            Assert.IsFalse(TalkPageFixes.ProcessTalkPage(ref T2, DEFAULTSORT.NoChange));
            Assert.AreEqual(@"{{Foo}}
==Section==
{{Template:Bar}}", T2, "changes only made in zeroth section");
        }
        
        [Test]
        public void WPBiographyTopBilling()
        {
            string a = @"{{WPBiography|foo=yes|living=yes}}
{{WikiProject London}}
",  b = @"{{WikiProjectBannerShell|banner collapsed=no|1=
{{WPBiography|living=yes|class=Start|priority=|listas=Hill, A}}
{{WikiProject Gender Studies}}
{{WikiProject Oklahoma}}
| blp=yes
}}", c = @"{{WPBiography|foo=yes|living=no}}
{{WikiProject London}}";
            Assert.AreEqual(a, TalkPageFixes.WPBiography(@"{{WikiProject London}}
{{WPBiography|foo=yes|living=yes}}"), "WPBiography moved above WikiProjects");
            Assert.AreEqual(a, TalkPageFixes.WPBiography(a), "no change when WPBiography ahead of WikiProjects");
            Assert.AreEqual(b, TalkPageFixes.WPBiography(b), "no change when WPBS present");
            Assert.AreEqual(c, TalkPageFixes.WPBiography(c), "no change when not living");
            Assert.AreEqual(c + @"{{blp}}", TalkPageFixes.WPBiography(c + @"{{blp}}"), "no change when not living");
            
            Assert.AreEqual(a, TalkPageFixes.WPBiography(a + @"{{blp}}"), "blp template removed when living=y");
        }
        
        [Test]
        public void WPBiographyListasDiacritics()
        {
            string a = @"{{WPBiography|foo=yes|living=yes|listas=Foé}}";
            
            Assert.AreEqual(a.Replace(@"é","e"), TalkPageFixes.WPBiography(a), "diacritics removed from WPBiography listas");
            Assert.AreEqual(a.Replace(@"é","e"), TalkPageFixes.WPBiography(a.Replace(@"é","e")), "no change when no diacritics in WPBiography listas");
        }
        
        [Test]
        public void WPBiographyBLPActivepol()
        {
            string a = @"{{WPBiography}}";
            
            Assert.AreEqual(a.Replace(@"}}"," | living=yes}}"), TalkPageFixes.WPBiography(a + @"{{blp}}"), "Add blp to WPBiography");
            Assert.AreEqual(a.Replace(@"}}"," |living=yes}}"), TalkPageFixes.WPBiography(a.Replace(@"}}"," |living=}}") + @"{{blp}}"), "Add value to empty parameter");
            Assert.AreEqual(a.Replace(@"}}","|living=no}}" + @"{{blp}}"), TalkPageFixes.WPBiography(a.Replace(@"}}","|living=no}}") + @"{{blp}}"), "No change if blp=yes and living=no");
            Assert.AreEqual(a.Replace(@"}}"," | living=yes | activepol=yes | politician-work-group=yes}}"), TalkPageFixes.WPBiography(a + @"{{activepol}}"), "Add activepol to WPBiography");
            Assert.AreEqual(a.Replace(@"}}"," | living=yes | activepol=yes | politician-work-group=yes}}"), TalkPageFixes.WPBiography(a + @"{{active politician}}"), "Add activepol via redirect to WPBiography");
            Assert.AreEqual(a.Replace(@"}}"," |activepol= yes| living=yes | politician-work-group=yes}}"), TalkPageFixes.WPBiography(a.Replace(@"}}"," |activepol=}}") + @"{{activepol}}"), "Add value to empty parameter");
            Assert.AreEqual(a.Replace(@"}}"," |activepol=yes | living=yes | politician-work-group=yes}}"), TalkPageFixes.WPBiography(a.Replace(@"}}"," |activepol=no}}") + @"{{activepol}}"), "Change value to activepol no nonsense parameter");
            Assert.AreEqual(a.Replace(@"}}"," | living=yes | activepol=yes | politician-work-group=yes}}"), TalkPageFixes.WPBiography(a + @"{{blp}}{{activepol}}"), "Add activepol and blp to WPBiography");
        }

        [Test]
        public void WPSongs()
        {
            string a = @"{{WPSongs}}";

            Assert.AreEqual(a.Replace(@"}}"," | needs-infobox=yes}}"), TalkPageFixes.WPSongs(a + @"{{sir}}"), "Add sir to WPSongs");
            Assert.AreEqual(a.Replace(@"}}"," | needs-infobox=yes}}"), TalkPageFixes.WPSongs(a.Replace(@"}}"," | needs-infobox=}}" + @"{{sir}}")), "Add sir to WPSongs with empty parameter");
            Assert.AreEqual(a.Replace(@"}}"," | needs-infobox=yes}}"), TalkPageFixes.WPSongs(a.Replace(@"}}"," | needs-infobox=}}" + @"{{Single infobox request}}")), "Add Single infobox request to WPSongs with empty parameter");
            Assert.AreEqual(a.Replace(@"}}"," | needs-infobox=yes}}"), TalkPageFixes.WPSongs(a.Replace(@"}}"," | needs-infobox=yes}}") + @"{{sir}}"), "Remove sir when WPSongs with needs-infobox=yes exists");
            Assert.AreEqual(a, TalkPageFixes.WPSongs(a), "Do nothing");
            Assert.AreEqual(a, TalkPageFixes.WPSongs(a.Replace(@"}}","|importance=yes}}")), "Remove importance");
            Assert.AreEqual(a, TalkPageFixes.WPSongs(a.Replace(@"}}","|needs-infobox=no}}")), "Remove needs-infobox=no");
            Assert.AreEqual(a, TalkPageFixes.WPSongs(a.Replace(@"}}","|importance=yes|needs-infobox=no}}")), "Remove importance and needs-infobox=no");

            string b = @"{{WikiProject Songs}}";

            Assert.AreEqual(b.Replace(@"}}"," | needs-infobox=yes}}"), TalkPageFixes.WPSongs(b + @"{{sir}}"), "Add sir to WPSongs");
            Assert.AreEqual(b.Replace(@"}}"," | needs-infobox=yes}}"), TalkPageFixes.WPSongs(b.Replace(@"}}"," | needs-infobox=}}" + @"{{sir}}")), "Add sir to WPSongs with empty parameter");
            Assert.AreEqual(b.Replace(@"}}"," | needs-infobox=yes}}"), TalkPageFixes.WPSongs(b.Replace(@"}}"," | needs-infobox=}}" + @"{{Single infobox request}}")), "Add Single infobox request to WPSongs with empty parameter");
            Assert.AreEqual(b.Replace(@"}}"," | needs-infobox=yes}}"), TalkPageFixes.WPSongs(b.Replace(@"}}"," | needs-infobox=yes}}") + @"{{sir}}"), "Remove sir when WPSongs with needs-infobox=yes exists");
            Assert.AreEqual(b, TalkPageFixes.WPSongs(b), "Do nothing");
        }

        [Test]
        public void WPJazz()
        {
            string a = @"{{WPJazz}}";

            Assert.AreEqual(a.Replace(@"}}", " | song=yes}}{{WPSongs}}"), TalkPageFixes.WPJazz(a + @"{{WPSongs}}"), "Add song to WPJazz");
            Assert.AreEqual(a.Replace(@"}}", " | album=yes}}{{WPAlbums}}"), TalkPageFixes.WPJazz(a + @"{{WPAlbums}}"), "Add album to WPJazz");
            Assert.AreEqual(a.Replace(@"}}", " | song=yes}}{{WikiProject Songs}}"), TalkPageFixes.WPJazz(a.Replace(@"}}", " | song=}}" + @"{{WikiProject Songs}}")), "Add song to WPJazz with empty parameter");
            Assert.AreEqual(a.Replace(@"}}", " | album=yes}}{{WikiProject Albums}}"), TalkPageFixes.WPJazz(a.Replace(@"}}", " | album=}}" + @"{{WikiProject Albums}}")), "Add album to WPJazz with empty parameter");
            Assert.AreEqual(a, TalkPageFixes.WPJazz(a), "Do nothing");
            Assert.AreEqual(a, TalkPageFixes.WPJazz(a.Replace(@"}}", "|needs-infobox=no}}")), "Remove needs-infobox=no");

            string b = @"{{WikiProject Jazz}}";

            Assert.AreEqual(b.Replace(@"}}", " | song=yes}}{{WPSongs}}"), TalkPageFixes.WPJazz(b + @"{{WPSongs}}"), "Add song to WikiProject Jazz");
            Assert.AreEqual(b.Replace(@"}}", " | album=yes}}{{WPAlbums}}"), TalkPageFixes.WPJazz(b + @"{{WPAlbums}}"), "Add album to WikiProject Jazz");
            Assert.AreEqual(b.Replace(@"}}", " | song=yes}}{{WikiProject Songs}}"), TalkPageFixes.WPJazz(b.Replace(@"}}", " | song=}}" + @"{{WikiProject Songs}}")), "Add song to WPJAzz with empty parameter");
            Assert.AreEqual(b, TalkPageFixes.WPJazz(b), "Do nothing");
            Assert.AreEqual(b, TalkPageFixes.WPJazz(b.Replace(@"}}", "|needs-infobox=no}}")), "Remove needs-infobox=no");
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
    
    [TestFixture]
    public class ListMakerTests : RequiresInitialization
    {
        private readonly ListMaker LMaker = new ListMaker();
        
        [Test]
        public void NormalizeTitle()
        {
            Assert.AreEqual("Foo", LMaker.NormalizeTitle(@"https://en.wikipedia.org/w/index.php?title=Foo&diff=3&oldid=4"));
            Assert.AreEqual("Health effects of chocolate", LMaker.NormalizeTitle(@"https://en.wikipedia.org/w/index.php?title=Health_effects_of_chocolate&diff=4018&oldid=40182"));
            Assert.AreEqual("Foo", LMaker.NormalizeTitle(@"https://en.wikipedia.org/w/index.php?title=Foo&action=history"));
            Assert.AreEqual("Foo", LMaker.NormalizeTitle(@"https://en.wikipedia.org/w/index.php?title=Foo&action=edit"));
            Assert.AreEqual("Foo", LMaker.NormalizeTitle(@"https://en.wikipedia.org/w/index.php?title=Foo&oldid=5"));
            Assert.AreEqual("Science (journal)", LMaker.NormalizeTitle(@"https://en.wikipedia.org/w/index.php?title=Science%20%28journal%29&action=history"));
            Assert.AreEqual(@"Wikipedia:AutoWikiBrowser/Sandbox", LMaker.NormalizeTitle(@"https://en.wikipedia.org/w/index.php?title=Wikipedia:AutoWikiBrowser/Sandbox&action=edit"));
            Assert.AreEqual(@"Wikipedia:AutoWikiBrowser/Sandbox", LMaker.NormalizeTitle(@"en.wikipedia.org/w/index.php?title=Wikipedia:AutoWikiBrowser/Sandbox&action=edit"));
            Assert.AreEqual("Foo", LMaker.NormalizeTitle(@"https://en.wikipedia.org/w/index.php?title=Foo#References"));

            Assert.AreEqual("Foo", LMaker.NormalizeTitle(@"   Foo"), "cleans spacing from Firefox Category paste");

            Assert.AreEqual(@"Foo", LMaker.NormalizeTitle(@"https://en.wikipedia.org/w/index.php?title=Foo&pe=1&#Date_on"));
            Assert.AreEqual(@"Foo", LMaker.NormalizeTitle(@"Foo‎"),"title has left-to-right mark at the end");

            Assert.AreEqual("Foo", LMaker.NormalizeTitle(@"https://en.wikipedia.org/w/index.php?title=Foo&offset=20130214190000&action=history"), "cleans spacing from Firefox Category paste");
            Assert.AreEqual("Foo", LMaker.NormalizeTitle(@"https://en.wikipedia.org/w/?title=Foo"));
            Assert.AreEqual("Foo", LMaker.NormalizeTitle(@"https://en.wikipedia.org/wiki/Foo"));
            Assert.AreEqual("Foo", LMaker.NormalizeTitle(@"http://en.wikipedia.org/wiki/Foo"), "HTTP not HTTPS support");
            Assert.AreEqual("Foo", LMaker.NormalizeTitle(@"//en.wikipedia.org/w/index.php?title=Foo&action=history"), "Protocol-relative support");
        }

        [Test]
        public void NormalizeTitleSecure()
        {
            Assert.AreEqual("Foo", LMaker.NormalizeTitle(@"https://en.wikipedia.org/w/index.php?title=Foo&diff=3&oldid=4"));
        }
        
        [Test]
        public void RemoveListDuplicatesSimple()
        {
            ListMaker LMakerRLD = new ListMaker();
            LMakerRLD.Add(new Article("A"));
            LMakerRLD.Add(new Article("B"));
            LMakerRLD.Add(new Article("C"));
            LMakerRLD.Add(new Article("A"));
            
            LMakerRLD.RemoveListDuplicates();
            
            Assert.AreEqual(LMakerRLD.Count, 3, "Duplicate removed");
            foreach(Article a in LMakerRLD)
            {
                Assert.AreEqual(a, "A", "Duplicates removed from end of list");
                break;
            }
        }
        
        
        [Test]
        public void RemoveListDuplicates10K()
        {
            const int big = 10000;
            ListMaker LMakerLarge = new ListMaker();
            LMakerLarge.Clear();
            for(int i=1; i<big; i++)
                LMakerLarge.Add(new Article(i.ToString()));
            
            LMakerLarge.Add(new Article("1"));
            
            Assert.AreEqual(LMakerLarge.Count, big);
            
            LMakerLarge.RemoveListDuplicates();
            
            Assert.AreEqual(LMakerLarge.Count, big-1, "Duplicate removed");
            Assert.IsTrue(LMakerLarge.Contains(new Article("1")), "First instance of article retained");
        }

        [Test]
        public void FilterNonMainArticlesVolume()
        {
            const int big = 500;
            ListMaker LMakerLarge = new ListMaker();
            LMakerLarge.Clear();
            for(int i=1; i<big; i++)
                LMakerLarge.Add(i.ToString());

            LMakerLarge.Add("Talk:Me");

            LMakerLarge.FilterNonMainArticles();
            Assert.AreEqual(LMakerLarge.Count, big-1, "Non-mainspace article removed");
        }

        [Test]
        public void FilterNonMainArticles()
        {
            ListMaker LMaker = new ListMaker();
            LMaker.Add("One");
            LMaker.Add("Two");
            LMaker.Add("Talk:Three");
            LMaker.Add("Four");
            LMaker.Add("Talk:Five");
            LMaker.Add("Talk:Five2");
            LMaker.Add("Six");

            LMaker.FilterNonMainArticles();
            Assert.AreEqual(4, LMaker.NumberOfArticles);
        }
    }
    
    [TestFixture]
    public class ListComparerTests : RequiresInitialization
    {
        [Test]
        public void ListComparerSimple()
        {
            ListMaker LMaker = new ListMaker();
            LMaker.Add(new Article("A"));
            LMaker.Add(new Article("B"));
            LMaker.Add(new Article("C"));
            LMaker.Add(new Article("C")); // duplicate, removed during compare
            System.Windows.Forms.ListBox lb1 = new System.Windows.Forms.ListBox();
            System.Windows.Forms.ListBox lb2 = new System.Windows.Forms.ListBox();
            System.Windows.Forms.ListBox lb3 = new System.Windows.Forms.ListBox();
            
            List<Article> articles = new List<Article>();
            articles.Add(new Article("A"));
            articles.Add(new Article("D"));
            articles.Add(new Article("E"));
            
            ListComparer.CompareLists(LMaker, articles, lb1, lb2, lb3);
            
            // unique in 1
            Assert.IsTrue(lb1.Items.Contains("B"));
            Assert.IsTrue(lb1.Items.Contains("C"));
            Assert.IsFalse(lb1.Items.Contains("A"));
            Assert.AreEqual(lb1.Items.Count, 2);

            // unique in 2
            Assert.IsFalse(lb2.Items.Contains("A"));
            Assert.IsTrue(lb2.Items.Contains("D"));
            Assert.IsTrue(lb2.Items.Contains("E"));
            Assert.AreEqual(lb2.Items.Count, 2);

            // common to both
            Assert.IsTrue(lb3.Items.Contains("A"));
            Assert.AreEqual(lb3.Items.Count, 1);
        }
        
        [Test]
        public void ListComparer10K()
        {
            const int big = 10000;
            ListMaker LMakerC10K = new ListMaker();

            for(int i=0; i<big; i++)
                LMakerC10K.Add(new Article(i.ToString()));

            LMakerC10K.Add(new Article("A"));
            LMakerC10K.Add(new Article("B"));

            System.Windows.Forms.ListBox lb1 = new System.Windows.Forms.ListBox();
            System.Windows.Forms.ListBox lb2 = new System.Windows.Forms.ListBox();
            System.Windows.Forms.ListBox lb3 = new System.Windows.Forms.ListBox();
            
            List<Article> articlesC = new List<Article>();
            for(int i=0; i<big; i++)
                articlesC.Add(new Article(i.ToString()));
            
            articlesC.Add(new Article("C"));
            articlesC.Add(new Article("D"));
            
            ListComparer.CompareLists(LMakerC10K, articlesC, lb1, lb2, lb3);
            
            // unique in 1
            Assert.IsTrue(lb1.Items.Contains("A"));
            Assert.IsTrue(lb1.Items.Contains("B"));
            
            // unique in 2
            Assert.IsTrue(lb2.Items.Contains("C"));
            Assert.IsTrue(lb2.Items.Contains("D"));
            
            // common to both
            Assert.IsTrue(lb3.Items.Contains("1"));
            Assert.AreEqual(lb3.Items.Count, big);
        }
    }
    
    [TestFixture]
    public class ListBoxArticleTests : RequiresInitialization
    {
        [Test]
        public void RemoveSelected()
        {
            WikiFunctions.Controls.Lists.ListBoxArticle lbArticles = new WikiFunctions.Controls.Lists.ListBoxArticle();
            
            const int big = 70000, sel=5000;

            // sequential block deletion performance
            for(int i=0; i<big; i++)
                lbArticles.Items.Add(new Article(i.ToString()));
            
            for(int j=sel; j>0; j--)
                lbArticles.SetSelected(big-j, true);
            
            lbArticles.RemoveSelected(true);

            if(Globals.UsingMono) // Mono implementation of SetSelected does not seem to honour second input to multi-select
                Assert.AreEqual(lbArticles.Items.Count, big-1);
            else
                Assert.AreEqual(lbArticles.Items.Count, big-sel);

            // single selected item deletion performance
            lbArticles.Items.Clear();
            for(int i=0; i<big*3; i++)
                lbArticles.Items.Add(new Article(i.ToString()));
            lbArticles.SetSelected((int)big/2, true);
            lbArticles.RemoveSelected(true);
            Assert.AreEqual(lbArticles.Items.Count, big*3-1);

            // non-sequential deletion
            lbArticles.Items.Clear();
            for(int i=0; i<10; i++)
                lbArticles.Items.Add(new Article(i.ToString()));
            lbArticles.SetSelected(1, true);
            lbArticles.SetSelected(3, true);
            lbArticles.RemoveSelected(true);
            if(Globals.UsingMono)
            {
                lbArticles.SetSelected(1, true);
                lbArticles.RemoveSelected(true);
            }
            Assert.IsFalse(lbArticles.Items.Contains(new Article("1")));
            Assert.IsFalse(lbArticles.Items.Contains(new Article("3")));
            Assert.IsTrue(lbArticles.Items.Contains(new Article("2")));

            lbArticles.Items.Clear();
            for(int i=0; i<10; i++)
                lbArticles.Items.Add(new Article(i.ToString()));
            lbArticles.SetSelected(1, true);
            lbArticles.SetSelected(5, true);
            lbArticles.RemoveSelected(true);
            if(Globals.UsingMono)
            {
                lbArticles.SetSelected(1, true);
                lbArticles.RemoveSelected(true);
            }
            Assert.IsFalse(lbArticles.Items.Contains(new Article("1")));
            Assert.IsFalse(lbArticles.Items.Contains(new Article("5")));
            Assert.IsTrue(lbArticles.Items.Contains(new Article("2")));

            lbArticles.Items.Clear();
            lbArticles.Items.Add(new Article("A"));
            lbArticles.Items.Add(new Article("A"));
            lbArticles.Items.Add(new Article("B"));
            lbArticles.SetSelected(2, true);
            lbArticles.RemoveSelected(false);
            Assert.AreEqual(lbArticles.Items.Count, 2, "Duplicates not removed if not in duplicate mode");
        }
    }
    
    [TestFixture]
    public class FindAndReplaceTests
    {
        [Test]
        public void FindAndReplace()
        {
            WikiFunctions.Parse.Replacement r = new WikiFunctions.Parse.Replacement("foo", "bar", true, true, true, true, RegexOptions.None, "");
            
            WikiFunctions.Parse.FindandReplace fr = new FindandReplace();
            
            bool changemade = false;
            
            Assert.AreEqual(fr.PerformFindAndReplace(r, "the was", "Test", out changemade), "the was");
            Assert.IsFalse(changemade);
            Assert.AreEqual(null, fr.ReplacedSummary, "No match: no edit summary");
            
            Assert.AreEqual(fr.PerformFindAndReplace(r, "the foo was", "Test", out changemade), "the bar was");
            Assert.IsTrue(changemade);
            Assert.AreEqual("foo" + WikiFunctions.Parse.FindandReplace.Arrow + "bar", fr.ReplacedSummary, "One match: a to b");
            
            fr = new FindandReplace();
            changemade = false;
            
            Assert.AreEqual(fr.PerformFindAndReplace(r, "the foo was or foo was", "Test", out changemade), "the bar was or bar was");
            Assert.AreEqual("foo" + WikiFunctions.Parse.FindandReplace.Arrow + "bar (2)", fr.ReplacedSummary, "Match count shown");
            
            fr = new FindandReplace();
            changemade = false;
            
            Assert.AreEqual(fr.PerformFindAndReplace(r, "the foo was or foo was foo", "Test", out changemade), "the bar was or bar was bar");
            Assert.AreEqual("foo" + WikiFunctions.Parse.FindandReplace.Arrow + "bar (3)", fr.ReplacedSummary, "Match count shown, 3");
            
            r = new WikiFunctions.Parse.Replacement("foot?", "bar", true, true, true, true, RegexOptions.None, "");
            fr = new FindandReplace();
            changemade = false;
            
            Assert.AreEqual(fr.PerformFindAndReplace(r, "the foot was or foo was", "Test", out changemade), "the bar was or bar was");
            Assert.IsTrue(changemade);
            Assert.AreEqual("foot" + WikiFunctions.Parse.FindandReplace.Arrow + "bar (2)", fr.ReplacedSummary, "Different matches, match text of first used");
            
            r = new WikiFunctions.Parse.Replacement("fooo?", "foo", true, true, true, true, RegexOptions.None, "");
            fr = new FindandReplace();
            changemade = false;
            Assert.AreEqual(fr.PerformFindAndReplace(r, "the foo was a fooo it", "Test", out changemade), "the foo was a foo it");
            Assert.IsTrue(changemade);
            Assert.AreEqual("fooo" + WikiFunctions.Parse.FindandReplace.Arrow + "foo", fr.ReplacedSummary, "No-change match ignored");
            
            fr = new FindandReplace();
            changemade = false;
            Assert.AreEqual(fr.PerformFindAndReplace(r, "the foo was", "Test", out changemade), "the foo was");
            Assert.IsFalse(changemade, "only match is no-change on replace, so no change made");
            Assert.AreEqual(null, fr.ReplacedSummary, "Only match is No-change match, no edit summary");
        }

        [Test]
        public void FindAndReplaceNewLines()
        {
            // regex
            WikiFunctions.Parse.Replacement r = new WikiFunctions.Parse.Replacement("foo\n", "bar ", true, true, true, true, RegexOptions.None, "");
            WikiFunctions.Parse.FindandReplace fr = new FindandReplace();
            bool changemade = false;

            Assert.AreEqual("the bar was", fr.PerformFindAndReplace(r, "the foo\nwas", "Test", out changemade));
            Assert.IsTrue(changemade);
            
            // not regex
            r = new WikiFunctions.Parse.Replacement("foo\n", "bar ", false, true, true, true, RegexOptions.None, "");
            Assert.AreEqual("the bar was", fr.PerformFindAndReplace(r, "the foo\nwas", "Test", out changemade));
            Assert.IsTrue(changemade);
        }
        
        [Test]
        public void FindAndReplaceRemove()
        {
            WikiFunctions.Parse.Replacement r = new WikiFunctions.Parse.Replacement("foo", "", true, true, true, true, RegexOptions.None, "");
            
            WikiFunctions.Parse.FindandReplace fr = new FindandReplace();
            
            bool changemade = false;
            
            Assert.AreEqual(fr.PerformFindAndReplace(r, "the was", "Test", out changemade), "the was");
            Assert.IsFalse(changemade);
            Assert.AreEqual(null, fr.ReplacedSummary, "No match: no edit summary");
            Assert.AreEqual(null, fr.RemovedSummary, "No match: no edit summary");
            
            Assert.AreEqual(fr.PerformFindAndReplace(r, "the foo was", "Test", out changemade), "the  was");
            Assert.IsTrue(changemade);
            Assert.AreEqual("foo", fr.RemovedSummary, "One match: removed a");
            
            fr = new FindandReplace();
            changemade = false;
            
            Assert.AreEqual(fr.PerformFindAndReplace(r, "the foo was or foo was", "Test", out changemade), "the  was or  was");
            Assert.AreEqual("foo (2)", fr.RemovedSummary, "Match count shown");

            r = new WikiFunctions.Parse.Replacement("foot?", "", true, true, true, true, RegexOptions.None, "");
            fr = new FindandReplace();
            changemade = false;
            
            Assert.AreEqual(fr.PerformFindAndReplace(r, "the foot was or foo was", "Test", out changemade), "the  was or  was");
            Assert.IsTrue(changemade);
            Assert.AreEqual("foot (2)", fr.RemovedSummary, "Different matches, match text of first used");
        }
        
        [Test]
        public void FindAndReplaceProperties()
        {
            WikiFunctions.Parse.Replacement r = new WikiFunctions.Parse.Replacement("foo", "bar", true, true, true, true, RegexOptions.None, "");
            WikiFunctions.Parse.Replacement r2 = new WikiFunctions.Parse.Replacement("foo2", "bar2", true, true, true, false, RegexOptions.None, "");
            WikiFunctions.Parse.Replacement r2Disabled = new WikiFunctions.Parse.Replacement("foo2", "bar2", true, false, true, false, RegexOptions.None, "");
            WikiFunctions.Parse.FindandReplace fr = new FindandReplace();

            fr.AddNew(r);
            Assert.IsTrue(fr.HasAfterProcessingReplacements);
            Assert.IsTrue(fr.HasProcessingReplacements(true));
            Assert.IsFalse(fr.HasProcessingReplacements(false));

            fr.AddNew(r2);
            Assert.IsTrue(fr.HasAfterProcessingReplacements);
            Assert.IsTrue(fr.HasProcessingReplacements(true));
            Assert.IsTrue(fr.HasProcessingReplacements(false));

            fr.Clear();
            fr.AddNew(r2);
            Assert.IsFalse(fr.HasAfterProcessingReplacements);
            Assert.IsFalse(fr.HasProcessingReplacements(true));
            Assert.IsTrue(fr.HasProcessingReplacements(false));

            // all false when no enabled rules
            fr.Clear();
            fr.AddNew(r2Disabled);
            Assert.IsFalse(fr.HasAfterProcessingReplacements);
            Assert.IsFalse(fr.HasProcessingReplacements(true));
            Assert.IsFalse(fr.HasProcessingReplacements(false));

            fr.Clear();
            List<WikiFunctions.Parse.Replacement> l = new List<Replacement>();
            l.Add(r);
            l.Add(r2);
            fr.AddNew(l);
            Assert.IsTrue(fr.HasReplacements);
            Assert.AreEqual(fr.GetList(), l);
        }
    }
    
    [TestFixture]
    public class SubstTemplates
    {
        [Test]
        public void SubstTemplatesNoExpand()
        {
            WikiFunctions.SubstTemplates st = new WikiFunctions.SubstTemplates();
            st.ExpandRecursively =false;
            
            Assert.AreEqual("Now {{foo}}", st.SubstituteTemplates("Now {{foo}}", "test"));

            st.TemplateList = new [] {"foo", "bar"};
            
            Assert.AreEqual(2, st.NoOfRegexes);
            
            Assert.AreEqual("Now {{subst:foo}}", st.SubstituteTemplates("Now {{foo}}", "test"));
            Assert.AreEqual("Now {{subst:foo}}", st.SubstituteTemplates("Now {{ foo}}", "test"));
            Assert.AreEqual("Now {{subst:foo}}", st.SubstituteTemplates("Now {{ foo }}", "test"));
            Assert.AreEqual("Now {{subst:foo|first=y}}", st.SubstituteTemplates("Now {{foo|first=y}}", "test"));
            Assert.AreEqual("Now {{subst:foo|first}}", st.SubstituteTemplates("Now {{foo|first}}", "test"));
            Assert.AreEqual("Now {{subst:foo}} {{subst:bar}}", st.SubstituteTemplates("Now {{foo}} {{bar}}", "test"));
            
            Assert.AreEqual("Now {{subst:foo}}", st.SubstituteTemplates("Now {{Template:foo}}", "test"));
            Assert.AreEqual("Now {{subst:foo}}", st.SubstituteTemplates("Now {{template:foo}}", "test"));
            Assert.AreEqual("Now {{subst:foo}}", st.SubstituteTemplates("Now {{template :foo}}", "test"));
            Assert.AreEqual("Now {{subst:foo}}", st.SubstituteTemplates("Now {{ template :foo}}", "test"));
            Assert.AreEqual("Now {{subst:foo}}", st.SubstituteTemplates("Now {{ template : foo}}", "test"));
            Assert.AreEqual("Now {{subst:foo}}", st.SubstituteTemplates("Now {{Msg:foo}}", "test"));
            Assert.AreEqual("Now {{subst:foo}}", st.SubstituteTemplates("Now {{msg :foo}}", "test"));
        }
    }

    [TestFixture]
    public class SessionTests
    {
        [Test]
        public void SessionUser()
        {
            Assert.IsTrue(WikiFunctions.Session.UserNameInText("ABC", @"* ABC "));
            Assert.IsTrue(WikiFunctions.Session.UserNameInText("abc", @"* abc "));
            Assert.IsTrue(WikiFunctions.Session.UserNameInText("Abc", @"* abc "));
            Assert.IsTrue(WikiFunctions.Session.UserNameInText("abc", @"* Abc "));
            Assert.IsTrue(WikiFunctions.Session.UserNameInText("ABC", @"*ABC"));
            Assert.IsTrue(WikiFunctions.Session.UserNameInText("ABC", @"*          ABC"));

            Assert.IsTrue(WikiFunctions.Session.UserNameInText("ABC", @"
* HELLO
* ABC
* DEF"));

            Assert.IsTrue(WikiFunctions.Session.UserNameInText("aBC", @"* ABC "));
            Assert.IsTrue(WikiFunctions.Session.UserNameInText("ABC", @"* aBC "));

            Assert.IsTrue(WikiFunctions.Session.UserNameInText("ABC*", @"* ABC* "));
            Assert.IsTrue(WikiFunctions.Session.UserNameInText("ABC[D]", @"* ABC[D]"));

            Assert.IsTrue(WikiFunctions.Session.UserNameInText("ABC D", @"* ABC D"));
            Assert.IsTrue(WikiFunctions.Session.UserNameInText("ABC D", @"* ABC_D"));
            Assert.IsTrue(WikiFunctions.Session.UserNameInText("ABC_D", @"* ABC D"));

            Assert.IsFalse(WikiFunctions.Session.UserNameInText("ABC", @"* ABCD "));
            Assert.IsFalse(WikiFunctions.Session.UserNameInText("Abc", @"* ABC "));
            Assert.IsFalse(WikiFunctions.Session.UserNameInText("ABC", @"* X"));
        }
    }
}
