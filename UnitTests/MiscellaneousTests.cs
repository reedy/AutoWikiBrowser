using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using WikiFunctions;
using WikiFunctions.Controls.Lists;
using WikiFunctions.Parse;
using WikiFunctions.ReplaceSpecial;
using WikiFunctions.TalkPages;

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
            Assert.That(Hider.AddBackMore(s), Is.EqualTo(text));
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
            Assert.That(Hider.AddBackMore(s), Is.EqualTo(text));
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

        private void AssertAllHiddenMore(string text, bool hideExternalLinks, string message)
        {
            RegexAssert.IsMatch(AllHidden, HideMore(text, hideExternalLinks, false, true), message);
        }

        private string Hide(string text)
        {
            return Hide(text, true, false, true);
        }

        private string Hide(string text, bool hideExternalLinks, bool leaveMetaHeadings, bool hideImages)
        {
            Hider = new HideText(hideExternalLinks, leaveMetaHeadings, hideImages);
            string s = Hider.Hide(text);
            Assert.That(Hider.AddBack(s), Is.EqualTo(text));
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
            Assert.That(text, Is.EqualTo("[[foo]]"));
        }

        [Test]
        public void AcceptEmptyStrings()
        {
            Assert.That(Hide(""), Is.Empty);
            Assert.That(HideMore(""), Is.Empty);
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
            Assert.That(HideMore("[[foo|bar]]", true), Does.Contain("bar"));
        }
        
        [Test]
        public void HideItalics()
        {
            Assert.That(HideMore("Now ''text'' was"), Does.Not.Contain("text"));
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
            ClassicAssert.IsFalse(Hide(@"[[File:foo.jpg]]").Contains("foo"), "Standard case");
            ClassicAssert.IsFalse(Hide(@"[[File:foo with space and 0004.jpg]]").Contains("foo"), "with space");
            ClassicAssert.IsFalse(Hide(@"[[File:foo.jpeg]]").Contains("foo"), "jpeg");
            ClassicAssert.IsFalse(Hide(@"[[File:foo.JPEG]]").Contains("foo"), "JPEG");
            ClassicAssert.IsFalse(Hide(@"[[Image:foo with space and 0004.jpeg]]").Contains("foo"), "space and jpeg");
            ClassicAssert.IsFalse(Hide(@"[[Image:foo.jpeg]]").Contains("foo"), "Image jpeg");
            ClassicAssert.IsFalse(Hide(@"[[Image:foo with space and 0004.jpg]]").Contains("foo"), "image jpeg space");
            ClassicAssert.IsFalse(Hide(@"[[File:foo.jpg|").Contains("foo"), "To pipe");
            ClassicAssert.IsFalse(Hide(@"[[File:foo with space and 0004.jpg|").Contains("foo"), "Space to pipe");
            ClassicAssert.IsFalse(Hide(@"[[File:foo.jpeg|").Contains("foo"), "Standard case");
            ClassicAssert.IsFalse(Hide(@"[[Image:foo with space and 0004.jpeg|").Contains("foo"), "Standard case");
            ClassicAssert.IsFalse(Hide(@"[[Image:foo.jpeg|").Contains("foo"), "Standard case");
            ClassicAssert.IsFalse(Hide(@"[[Image:foo with SPACE() and 0004.jpg|").Contains("foo"), "Standard case");
            ClassicAssert.IsFalse(Hide(@"[[File:foo.gif|").Contains("foo"), "Standard case");
            ClassicAssert.IsFalse(Hide(@"[[Image:foo with space and 0004.gif|").Contains("foo"), "Standard case");
            ClassicAssert.IsFalse(Hide(@"[[Image:foo.gif|").Contains("foo"), "Standard case");
            ClassicAssert.IsFalse(Hide(@"[[Image:foo with SPACE() and 0004.gif|").Contains("foo"), "Standard case");
            ClassicAssert.IsFalse(Hide(@"[[File:foo.png|").Contains("foo"), "Standard case");
            ClassicAssert.IsFalse(Hide(@"[[Image:foo with space and 0004.png|").Contains("foo"), "Standard case");
            ClassicAssert.IsFalse(Hide(@"[[Image:foo_here.png|").Contains("foo"), "Standard case");
            ClassicAssert.IsFalse(Hide(@"[[Image:foo with SPACE() and 0004.png|").Contains("foo"), "Standard case");
            ClassicAssert.IsFalse(Hide(@"[[Image:westminster.tube.station.jubilee.arp.jpg|").Contains("westminster.tube.station.jubilee.arp"), "Dot name");

            ClassicAssert.IsTrue(Hide(@"[[File:foo.jpg|thumb|140px|[[Jo]] Assistant [[Ge]]]]").StartsWith("[["), "Retain starting brackets");
            ClassicAssert.IsTrue(Hide(@"[[File:foo.jpg|thumb|140px|[[Jo]] Assistant [[Ge]]]]").Contains(@"thumb|140px|[[Jo]] Assistant [[Ge]]]]"), "Retain ending brackets");


            AssertAllHidden(@"<imagemap>
File:Blogs001.jpeg|Description
File:Blogs002.jpeg|Description
</imagemap>");

            ClassicAssert.IsFalse(HideMore(@"[[Category:Foo|abc]]", false).Contains("abc"), "Category sort key always hidden if hiding wikilinks and not leaving target");
            ClassicAssert.IsFalse(HideMore(@"[[Category:Foo|abc]]", true).Contains("abc"), "Category sort key hidden even if keeping targets");

            HideText h = new HideText(true, false, false);
            ClassicAssert.IsTrue(h.HideMore(@"[[Category:Foo|abc]]", false, false).Contains("abc"), "Category sort key kept if keeping wikilinks");
        }
        
        [Test]
        public void HideImagesLimits()
        {
            // in these ones all but the last | is hidden
            ClassicAssert.IsTrue(Regex.IsMatch(Hide(@"|image_skyline=442px_-_London_Lead_Image.jpg|"), Hidden + @"\|"));
            ClassicAssert.IsTrue(Regex.IsMatch(Hide(@"|image_skyline=442px_-_London_Lead_Image.jpg <!--comm-->|"), Hidden + " " + Hidden + @"\|"));
            ClassicAssert.IsTrue(Regex.IsMatch(Hide(@"|image_map=London (European Parliament constituency).svg   |"), Hidden + @"   \|"));
            ClassicAssert.IsTrue(Regex.IsMatch(Hide(@"|image_map=westminster.tube.station.jubilee.arp.jpg|"), Hidden + @"\|"));
            ClassicAssert.IsTrue(Regex.IsMatch(Hide(@"|Cover  = AmorMexicanaThalia.jpg |"), Hidden + @" \|"));
            ClassicAssert.IsTrue(Regex.IsMatch(Hide(@"|image = AmorMexicanaThalia.jpg |"), Hidden + @" \|"));
            ClassicAssert.IsTrue(Regex.IsMatch(Hide(@"|
image = AmorMexicanaThalia.jpg |"), Hidden + @" \|"));
            ClassicAssert.IsTrue(Regex.IsMatch(Hide(@"|Img = BBC_logo1.jpg
|"), Hidden + @"
\|"));
            ClassicAssert.IsTrue(Regex.IsMatch(Hide(@"| image name = Fred Astaire.jpg |"), Hidden + @" \|"));
            ClassicAssert.IsTrue(Regex.IsMatch(Hide(@"|image2 = AmorMexicanaThalia.jpg |"), Hidden + @" \|"));
            ClassicAssert.IsTrue(Regex.IsMatch(Hide(@"|map = AmorMexicanaThalia.jpg |"), Hidden + @" \|"));
            ClassicAssert.IsTrue(Regex.IsMatch(Hide(@"|AmorMexicanaThalia.jpg |"), Hidden + @" \|"));
        }
        
        [Test]
        public void HideCiteTitles()
        {
            ClassicAssert.IsTrue(Hide(@"{{cite web| title = foo | date = May 2011").Contains(@"foo"), "Title not hidden if no year");
            ClassicAssert.IsFalse(Hide(@"{{cite web| title = Now September 20 - September 26, 2010 was | date = May 2011").Contains(@"September"), "title hidden when contains year");
            ClassicAssert.IsTrue(Tools.GetTemplateParameterValue(Hide(@"{{cite web| title = September 20 - September 26, 2010 | date = May 2011"), "title").Length > 0, "title parameter has length if hidden");

            ClassicAssert.IsTrue(Hide(@"{{cite web| title = [Now September 20 - September 26, 2010] was | date = May 2011").Contains(@"]"), "title hiding, bracket handling");
            
            ClassicAssert.IsTrue(Hide(@"{{cite web| trans_title = foo | date = May 2011").Contains(@"foo"), "trans_title not hidden if no year");
            ClassicAssert.IsFalse(Hide(@"{{cite web| trans_title = Now September 20 - September 26, 2010 was | date = May 2011").Contains(@"September"), "trans_title hidden when contains year");
            ClassicAssert.IsTrue(Tools.GetTemplateParameterValue(Hide(@"{{cite web| trans_title = September 20 - September 26, 2010 | date = May 2011"), "trans_title").Length > 0, "trans_title parameter has length if hidden");

            Assert.That(Hide(@"{{cite web | url = www.site.com/a.pdf }}"), Is.EqualTo(@"{{cite web | url = www.site.com/a.pdf }}"), "PDF www URL not hidden");

            ClassicAssert.IsFalse(Hide(@"{{cite web| trans_title = Now September 2009 - September 26, 2010 was | date = May 2011").Contains(@"2010"), "2 years");
            ClassicAssert.IsTrue(Hide(@"{{cite web| trans_title = September 26, 2010 was (in three volumes} | date = May 2011").Contains(@"(in three volumes}"), "Don't hide beyond year");
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
            Assert.That(Hide(Caption1), Is.EqualTo(Caption1));
            Assert.That(Hide(Caption2), Is.EqualTo(Caption2));
            Assert.That(Hide(Caption3), Is.EqualTo(Caption3));
            Assert.That(Hide(Caption4), Is.EqualTo(Caption4));
            Assert.That(Hide(Caption5), Is.EqualTo(Caption5));

            // in tests below part of string is hidden
            ClassicAssert.IsTrue(Hide(@"[[Image:some image name.JPG|thumb|words with typos]]").EndsWith(@"thumb|words with typos]]"));
            ClassicAssert.IsTrue(Hide(@"[[Image:some image name.JPEG|words with typos]]").EndsWith(@"words with typos]]"));
            ClassicAssert.IsTrue(Hide(@"[[Image:some image name.jpg|thumb|words with typos ]]").EndsWith(@"thumb|words with typos ]]"));
            ClassicAssert.IsTrue(Hide(@"[[Image:some image name.png|20px|words with typos and [[links]] here]]").EndsWith(@"20px|words with typos and [[links]] here]]"));
            ClassicAssert.IsTrue(Hide(@"[[Image:some image name.svg|thumb|words with typos ]]").EndsWith(@"thumb|words with typos ]]"));
            ClassicAssert.IsTrue(Hide(@"[[Image:some_image_name.further words.tiff|thumb|words with typos<ref name=a/>]]").EndsWith(@"thumb|words with typos<ref name=a/>]]"));
            ClassicAssert.IsTrue(Hide(@"[[Image:some_image_name.for a word.png|thumb|words with typo]]").EndsWith(@"thumb|words with typo]]"));
            ClassicAssert.IsTrue(Hide(@"[[Image:some_image_name.png|thumb|words with typo]] Normal words in text").EndsWith(@"thumb|words with typo]] Normal words in text"));
            ClassicAssert.IsTrue(Hide(@"[[Image:some_image_name.png]] Normal words in text").EndsWith(@" Normal words in text"));
            ClassicAssert.IsTrue(Hide(Caption4 + Field1).EndsWith(Field1));
        }
        
        [Test]
        public void HideImagesMisc()
        {
            ClassicAssert.IsFalse(HideMore(@"{{Drugbox|
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

            Assert.That(HideMore(Caption1), Is.EqualTo(Caption1));
            Assert.That(HideMore(Caption2), Is.EqualTo(Caption2));
            Assert.That(HideMore(Caption3), Is.EqualTo(Caption3));
            Assert.That(HideMore(Caption4), Is.EqualTo(Caption4));
            Assert.That(HideMore(Caption5), Is.EqualTo(Caption5));

            // in tests below part of string is hidden
            ClassicAssert.IsTrue(HideMore(@"[[Image:some_image_name.png]] Normal words in text").EndsWith(@" Normal words in text"));
            ClassicAssert.IsTrue(HideMore(Caption4 + Field1).EndsWith(Field1));

            ClassicAssert.IsFalse(HideMore(@"{{Drugbox|
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
            ClassicAssert.IsTrue(Regex.IsMatch(HideMore(@"<gallery>
Image:quux.JPEG|text
</gallery>"), @"<gallery>
" + Hidden + @"|text
</gallery>"), "Simple case: one image link with namespace hidden, description retained");
            ClassicAssert.IsTrue(Regex.IsMatch(HideMore(@"<gallery>
quux.JPEG|text
</gallery>"), @"<gallery>
" + Hidden + @"|text
</gallery>"), "One image link without namespace hidden, description retained");
            ClassicAssert.IsTrue(Regex.IsMatch(HideMore(@"<gallery>
quux.JPEG|
</gallery>"), @"<gallery>
" + Hidden + @"|
</gallery>"), "One image link without namespace hidden, blank description");
            ClassicAssert.IsTrue(Regex.IsMatch(HideMore(@"<gallery>
quux.JPEG
</gallery>"), @"<gallery>
" + Hidden + @"
</gallery>"), "One image link without namespace hidden, no description ");
            ClassicAssert.IsTrue(Regex.IsMatch(HideMore(@"<gallery>
Image:foo.png|a bar
Image:quux.JPEG|text
</gallery>"), @"<gallery>
" + Hidden + @"|a bar
" + Hidden + @"|text
</gallery>"), "Multiple image links hidden");

            ClassicAssert.IsTrue(Regex.IsMatch(HideMore(@"<gallery param=""a"">
Image:quux.svg|text
</gallery>"), @"<gallery param=""a"">
" + Hidden + @"|text
</gallery>"), "Images hidden within gallery tags with parameters");
            ClassicAssert.IsTrue(Regex.IsMatch(HideMore(@"<Gallery>
Image:foo.png|a bar
Image:quux.JPEG|text
</gallery>"), @"<Gallery>
" + Hidden + @"|a bar
" + Hidden + @"|text
</gallery>"), "gallery tag casing");

            ClassicAssert.IsTrue(Regex.IsMatch(HideMore(@"<gallery>
Image:foo
Image:quux.JPEG|text
</gallery>"), @"<gallery>
Image:foo
" + Hidden + @"|text
</gallery>"), "Image link without file extension not hidden");
            ClassicAssert.IsTrue(Regex.IsMatch(HideMore(@"<gallery>
File:foo.png
File:9th of quux.JPEG|text
</gallery>"), @"<gallery>
" + Hidden + @"
" + Hidden + @"|text
</gallery>"), "image without description hidden OK");
            ClassicAssert.IsTrue(Regex.IsMatch(HideMore(@"<gallery>
File:bar ă foo.jpg|text1
File:9th of May, ățuux.JPEG|text2
</gallery>"), @"<gallery>
" + Hidden + @"|text1
" + Hidden + @"|text2
</gallery>"), "special characters in image name handled OK");
            ClassicAssert.IsTrue(Regex.IsMatch(HideMore(@"<gallery>
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

            ClassicAssert.IsTrue(Regex.IsMatch(Hide(@"<gallery>
quux.JPEG|text
</gallery>"), @"<gallery>
" + Hidden + @"|text
</gallery>"), "One image link without namespace hidden, description retained");
        }

        [Test]
        public void HideExternalLinks()
        {
            AssertAllHiddenMore("[http://foo]", true, "External link no text");
            AssertAllHiddenMore("[http://foo bar]", true, "External link with text");
            AssertAllHiddenMore("[//foo.com bar]", true, "External link Protocol-relative URL"); 
            AssertAllHiddenMore("[http://foo [bar]", true, "External link unbalanced bracket");
            AssertAllHiddenMore("[[ru:Link]]", true, "Possible interwiki, hidemore");
            ClassicAssert.IsFalse(Hide(@"[[ru:Link]]", true, true, true).Contains("Link"), "Possible interwiki, hide");
            
            ClassicAssert.IsTrue(Hide(@"date=April 2010|url=http://w/010111a.html}}", true, true, true).Contains(@"date=April 2010|url="), "cite template URL");
            ClassicAssert.IsTrue(Hide(@"date=April 2010|url=//w/010111a.html}}", true, true, true).Contains(@"date=April 2010|url="), "Protocol-relative URL");
            ClassicAssert.IsTrue(Hide(@"date=April 2010|url=http://w/010111a.html}}", true, true, true).Contains(@"}}"), "Cite template URL, }} retention");
        }
        
        [Test]
        public void HidePlainExternalLinks()
        {
            AssertAllHiddenMore("http://foo.com/asdfasdf/asdf.htm", true);
            AssertAllHiddenMore("https://www.foo.com/asdfasdf/asdf.htm", true);
            AssertAllHiddenMore("//www.foo.com/asdfasdf/asdf.htm", true); // Protocol-relative URL
            AssertAllHiddenMore("ftp://www.foo.com/asdfasdf/asdf.htm", true);
            AssertAllHiddenMore("mailto://www.foo.com/asdfasdf/asdf.htm", true);
            AssertAllHiddenMore("irc://www.foo.com/asdfasdf/asdf.htm", true);
            AssertAllHiddenMore("gopher://www.foo.com/asdfasdf/asdf.htm", true);
            AssertAllHiddenMore("telnet://www.foo.com/asdfasdf/asdf.htm", true);
            AssertAllHiddenMore("nntp://www.foo.com/asdfasdf/asdf.htm", true);
            AssertAllHiddenMore("worldwind://www.foo.com/asdfasdf/asdf.htm", true);
            AssertAllHiddenMore("news://www.foo.com/asdfasdf/asdf.htm", true);
            AssertAllHiddenMore("svn://www.foo.com/asdfasdf/asdf.htm", true);
            
            ClassicAssert.IsFalse(HideMore(@"http://foo.com/asdfasdf/asdf.htm", true, false, false).Contains("asdf"));
            ClassicAssert.IsTrue(HideMore(@"http://foo.com/asdfasdf/asdf.htm", false, false, false).Contains("asdf"));
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
            AssertHidden("<chem>foo\r\nbar</chem>");
            AssertHidden("<math chem>foo\r\nbar</math>");
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
            Assert.That(Hide("http://foo", false, false, true), Is.EqualTo("http://foo"));

            // leaveMetaHeadings
            AssertHidden("<!--Categories -->");
            AssertHidden("<!--foo-->", true, true, true);
            Assert.That(Hide("<!-- categories-->", true, true, true), Is.EqualTo("<!-- categories-->"));

            // hideImages
            AssertHidden("[[Image:foo.JPG]]");
            Assert.That(Hide("[[Image:foo.jpg]]", true, false, false), Is.EqualTo("[[Image:foo.jpg]]"));
            AssertHidden("[[File:foo.JPG]]");
            Assert.That(Hide("[[File:foo.jpg]]", true, false, false), Is.EqualTo("[[File:foo.jpg]]"));

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
            b = null;
            ClassicAssert.IsFalse(a.Equals(null));
            b = new Article("A");
            Assert.That(a.ToString(), Is.EqualTo("A"));
            Assert.That(a.Name, Is.EqualTo("A"));
            ClassicAssert.IsFalse(a != b);

            Assert.That(a.URLEncodedName, Is.EqualTo("A"));
            Assert.That(a.DisplayTitle, Is.EqualTo(null));
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
            ClassicAssert.IsFalse(a.NoArticleTextChanged);

            a = new Article("A", LongText);
            Globals.UnitTestIntValue = 1;
            Globals.UnitTestBoolValue = false;
            a.AutoTag(p, true, true);
            ClassicAssert.IsTrue(a.NoArticleTextChanged);
            #endif
        }

        [Test]
        public void AlertProperties()
        {
            Article a = new Article("A", "{{dead link}}");
            Assert.That(a.DeadLinks().Count, Is.EqualTo(1));
            ClassicAssert.IsTrue(a.HasDeadLinks);
            
            a = new Article("A", "<small>");
            Assert.That(a.UnclosedTags().Count, Is.EqualTo(1));
            
            a = new Article("A", "==[[A]]==");
            Assert.That(a.WikiLinkedHeaders().Count, Is.EqualTo(1));
            
            a = new Article("A", "{{multiple issues|foo=bar}}");
            Assert.That(a.UnknownMultipleIssuesParameters().Count, Is.EqualTo(1));
            
            a = new Article("Talk:A", "{{WikiProject banner shell|foo=bar}}");
            Assert.That(a.UnknownWikiProjectBannerShellParameters().Count, Is.EqualTo(1));

            a = new Article("Talk:A", "{{WikiProject banner shell|collapsed=true|living=a|class=a|listas=a|demo_page=a|vital=a}}");
            Assert.That(a.UnknownWikiProjectBannerShellParameters().Count, Is.EqualTo(0));

            a = new Article("Talk:A", "{{WikiProject banner shell|foo=bar|foo=bar}}");
            Assert.That(a.DuplicateWikiProjectBannerShellParameters().Count, Is.EqualTo(1));

            a = new Article("A", "{{multiple issues|section=bar|collapsed=yes}}");
            Assert.That(a.UnknownMultipleIssuesParameters().Count, Is.EqualTo(0));
            
            a = new Article("A", "[[A|B|C]]");
            Assert.That(a.DoublepipeLinks().Count, Is.EqualTo(1));

            a = new Article("A", "[[A||BC]]");
            Assert.That(a.DoublepipeLinks().Count, Is.EqualTo(1));
            
            a = new Article("A", "[[|A]]");
            Assert.That(a.TargetlessLinks().Count, Is.EqualTo(1));
            
            a = new Article("A", "{{cite web|sajksdfa=a}}");
            Assert.That(a.BadCiteParameters().Count, Is.EqualTo(1));
            
            a = new Article("A", "{{cite web|date=5-4-10}}");
            Assert.That(a.AmbiguousCiteTemplateDates().Count, Is.EqualTo(1));
            
            a = new Article("A", "[[User talk:Noobie]]");
            Assert.That(a.UserSignature().Count, Is.EqualTo(1));

            a.ResetEditSummary();
            Assert.That(a.EditSummary, Is.Empty);
        }

        [Test]
        public void Properties()
        {
            Article a = new Article("A", "ABC");
            ClassicAssert.IsFalse(a.ArticleIsAboutAPerson);
            
            a = new Article("A");
            ClassicAssert.IsFalse(a.HasDiacriticsInTitle);
            
            a = new Article("Aé");
            ClassicAssert.IsTrue(a.HasDiacriticsInTitle);
            
            ClassicAssert.IsTrue(a.IsStub);
            
            a = new Article("A", "ABC");
            ClassicAssert.IsFalse(a.HasStubTemplate);
            ClassicAssert.IsFalse(a.HasInfoBox);
            ClassicAssert.IsFalse(a.HasSicTag);
            ClassicAssert.IsFalse(a.IsInUse);
            ClassicAssert.IsFalse(a.HasTargetLessLinks);
            ClassicAssert.IsFalse(a.HasDoublePipeLinks);
            ClassicAssert.IsFalse(a.HasMorefootnotesAndManyReferences);
            ClassicAssert.IsFalse(a.IsDisambiguationPage);
            ClassicAssert.IsFalse(a.IsDisambiguationPageWithRefs);
            ClassicAssert.IsFalse(a.IsSIAPage);
            ClassicAssert.IsFalse(a.HasRefAfterReflist);
            ClassicAssert.IsFalse(a.HasNamedReferences);
            ClassicAssert.IsFalse(a.HasBareReferences);
            ClassicAssert.IsFalse(a.HasAmbiguousCiteTemplateDates);
            ClassicAssert.IsFalse(a.HasSeeAlsoAfterNotesReferencesOrExternalLinks);
            ClassicAssert.IsFalse(a.IsMissingReferencesDisplay);
            ClassicAssert.IsFalse(a.IsRedirect);
        }
        
        [Test]
        public void CanDoGeneralFixes()
        {
            Article a = new Article("A", "ABC");
            ClassicAssert.IsTrue(a.CanDoGeneralFixes);
            
            ClassicAssert.IsFalse(a.CanDoTalkGeneralFixes);
            a = new Article("Talk:A", "ABC");
            ClassicAssert.IsTrue(a.CanDoTalkGeneralFixes);
        }

        [Test]
        public void FixPeopleCategories()
        {
            Parsers p = new Parsers();
            Article a = new Article("A", "ABC");
            a.FixPeopleCategories(p, true);
            Assert.That(a.ArticleText, Is.EqualTo("ABC"));
        }

        [Test]
        public void SetDefaultSort()
        {
            Article a = new Article("A", "ABC");
            a.SetDefaultSort("en", true);
            Assert.That(a.ArticleText, Is.EqualTo("ABC"));
        }

        [Test]
        public void Changes()
        {
            Article a = new Article("A", "ABC");
            a.AWBChangeArticleText("test", "ABC D", false);
            ClassicAssert.IsFalse(a.OnlyWhiteSpaceChanged);
            ClassicAssert.IsFalse(a.OnlyCasingChanged);

            a = new Article("A", "ABC");
            a.AWBChangeArticleText("test", "AB C", false);
            ClassicAssert.IsTrue(a.OnlyWhiteSpaceChanged);
            ClassicAssert.IsTrue(a.OnlyWhiteSpaceAndCasingChanged);
            ClassicAssert.IsFalse(a.OnlyCasingChanged);
            ClassicAssert.IsFalse(a.NoArticleTextChanged);

            a = new Article("A", "ABC");
            a.AWBChangeArticleText("test", "ABc", false);
            ClassicAssert.IsTrue(a.OnlyCasingChanged);

            a = new Article("A", "ABC");
            a.AWBChangeArticleText("test", "ABC", false);
            ClassicAssert.IsTrue(a.NoArticleTextChanged);
            ClassicAssert.IsFalse(a.OnlyMinorGeneralFixesChanged);
        }

        [Test]
        public void Categorisation()
        {
            Article theArticle = new Article("Category:Hello", "Text [[Category:Foo]]");
            Parsers p = new Parsers(500, true);

            theArticle.Categorisation((WikiFunctions.Options.CategorisationOptions)
                1, p, false,
                "Foo",
                "Foo2", false);

            Assert.That(theArticle.ArticleText, Is.EqualTo("Text [[Category:Foo2]]"), "Category rename operation");
        }

        [Test]
        public void NamespacelessName()
        {
            Assert.That(new Article("Foo").NamespacelessName, Is.EqualTo("Foo"));
            Assert.That(new Article("Category:Foo").NamespacelessName, Is.EqualTo("Foo"));
            Assert.That(new Article("Category:Category:Foo").NamespacelessName, Is.EqualTo("Category:Foo"));

            // TODO: uncomment when Namespace.Determine() will support non-normalised names
            // Assert.AreEqual("Foo", new Article("Category : Foo").NamespacelessName);

            // this behaviour has changed in recent MW versions
            // Assert.AreEqual("", new Article("Category:").NamespacelessName);
            // Assert.AreEqual("", new Article("Category: ").NamespacelessName);
        }
        
        [Test]
        public void UnbalancedBrackets()
        {
            Dictionary<int, int> UnB = new Dictionary<int, int>();
            
            Article a = new Article("TestArticle", "This is the text (here.");
            UnB = a.UnbalancedBrackets();
            Assert.That(UnB.Count, Is.EqualTo(1), "One unbalanced bracket in mainspace article");
            
            // UnB.Clear();
            a = new Article("TestArticle", @"This is the text here.
== Section ==
There [was.");
            UnB = a.UnbalancedBrackets();
            Assert.That(UnB.Count, Is.EqualTo(1), "Unbalanced bracket in mainspace article later sections found");
            
            // UnB.Clear();
            a = new Article("Talk:TestArticle", @"This is the text (here.
== Section ==
There [was.");
            UnB = a.UnbalancedBrackets();
            Assert.That(UnB.Count, Is.EqualTo(1), "One unbalanced bracket in zeroth section of talk article");
            
            // UnB.Clear();
            a = new Article("Talk:TestArticle", @"This is the text here.
== Section ==
There [was.");
            UnB = a.UnbalancedBrackets();
            Assert.That(UnB.Count, Is.EqualTo(0), "No unbalanced bracket in zeroth section of talk article");
        }

        [Test]
        public void ContainsComparers()
        {
            IArticleComparer containsComparer = ArticleComparerFactory.Create("foo\nbar",
                                                                              false,
                                                                              false,
                                                                              false, // singleline
                                                                              false); // multiline

            IArticleComparer notContainsComparer = ArticleComparerFactory.Create("foo\nbar",
                                                                                 false,
                                                                                 false,
                                                                                 false, // singleline
                                                                                 false); // multiline
            
            Article a = new Article("A", "Now foo\nbar");
            ClassicAssert.IsTrue(notContainsComparer.Matches(a), "does not contain comparer");
            ClassicAssert.IsTrue(containsComparer.Matches(a), "it contains comparer");
            
            a = new Article("A", @"Now foo-bar");
            ClassicAssert.IsFalse(notContainsComparer.Matches(a), "not contains is false");
            ClassicAssert.IsFalse(containsComparer.Matches(a), "contains is false");
        }

        [Test]
        public void Comparers()
        {
            Article a = new Article("A"), b = new Article("B"), z = new Article("Z"), one = new Article("1"), diacritic = new Article("Ș"),
                dollar = new Article("$");

            Assert.That(a.CompareTo(b), Is.EqualTo(-1), "A just before B");
            Assert.That(a.CompareTo(a), Is.EqualTo(0), "equal");
            Assert.That(b.CompareTo(a), Is.EqualTo(1), "B just after A");
            Assert.That(one.CompareTo(a), Is.EqualTo(-16), "1 before A");
            Assert.That(z.CompareTo(diacritic), Is.EqualTo(-446), "Diacritics later than Latin");
            Assert.That(one.CompareTo(diacritic), Is.EqualTo(-487), "Diacritics later than numbers");
            Assert.That(a.CompareTo(dollar), Is.EqualTo(29), "Keyboard characters before Latin");
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

            Assert.That(a.ArticleText, Is.EqualTo(@"'''test'''. z & a‡ †.

{{DEFAULTSORT:Hello test}}
[[Category:Test pages]]
"), "Text unicodified");

            a = new Article("a", @"'''test'''. z &amp; {{t|a&Dagger; &dagger;}}.

{{DEFAULTSORT:Hello test}}
[[Category:Test pages]]
");

            a.Unicodify(true, Parser, RemoveText);

            Assert.That(a.ArticleText, Is.EqualTo(@"'''test'''. z & {{t|a&Dagger; &dagger;}}.

{{DEFAULTSORT:Hello test}}
[[Category:Test pages]]
"), "Text unicodified, hidemore used");
            
            a = new Article("a", @"ABC");
            a.Unicodify(true, Parser, RemoveText);
            Assert.That(a.ArticleText, Is.EqualTo(@"ABC"), "No change");
        }
        
        [Test]
        public void BulletExternalLinks()
        {
            Article a = new Article("a", @"A
==External links==
http://www.site.com

[[Category:A]]");
            a.BulletExternalLinks(false);
            ClassicAssert.IsTrue(a.ArticleText.Contains("* http://www.site.com"));
            
             a = new Article("a", @"A
==External links==
http://www.site.com

[[Category:A]]");
            a.BulletExternalLinks(true);
            ClassicAssert.IsTrue(a.ArticleText.Contains("* http://www.site.com"));
            
            a = new Article("a", @"A
==External links==
* http://www.site.com

[[Category:A]]");
            a.BulletExternalLinks(true);
            ClassicAssert.IsTrue(a.ArticleText.Contains("* http://www.site.com"));
        }
        
        [Test]
        public void FixLinks()
        {
            Article a = new Article("a", @"A [[_B ]]");
            a.FixLinks(false);
            ClassicAssert.IsTrue(a.ArticleText.Contains("[[B]]"));
            
            a = new Article("a", @"A [[ B_]]");
            a.FixLinks(true);
            ClassicAssert.IsTrue(a.ArticleText.Contains("[[B]]"));
            
            a = new Article("a", @"A [[B]]");
            a.FixLinks(true);
            ClassicAssert.IsTrue(a.ArticleText.Contains("[[B]]"));
            
            a = new Article("a", @"A [[B]]");
            a.FixLinks(false);
            ClassicAssert.IsTrue(a.ArticleText.Contains("[[B]]"));
        }
        
        [Test]
        public void CiteTemplateDates()
        {
            Parsers Parser = new Parsers();
            Article a = new Article("a", @"A [[B]]");
            a.CiteTemplateDates(Parser, true);
            ClassicAssert.IsTrue(a.ArticleText.Contains("[[B]]"));
        }
        
        [Test]
        public void EmboldenTitles()
        {
            Parsers Parser = new Parsers();
            Article a = new Article("a", @"A [[B]]");
            a.EmboldenTitles(Parser, true);
            ClassicAssert.IsTrue(a.ArticleText.Contains("[[B]]"));
        }
    }

    [TestFixture]
    public class NamespaceTests : RequiresInitialization
    {
        [Test]
        public void Determine()
        {
            Assert.That(Namespace.Determine("test"), Is.EqualTo(0));
            Assert.That(Namespace.Determine(" test "), Is.EqualTo(0));
            Assert.That(Namespace.Determine(":test"), Is.EqualTo(0));
            Assert.That(Namespace.Determine("test:test"), Is.EqualTo(0));
            Assert.That(Namespace.Determine("My Project:Foo"), Is.EqualTo(0));
            Assert.That(Namespace.Determine("Magic: The Gathering"), Is.EqualTo(0));

            Assert.That(Namespace.Determine("User:"), Is.EqualTo(Namespace.User));

            Assert.That(Namespace.Determine("Talk:foo"), Is.EqualTo(Namespace.Talk));
            Assert.That(Namespace.Determine("Talk%3Afoo"), Is.EqualTo(Namespace.Talk), "Handles URL encoded colon");
            Assert.That(Namespace.Determine("User talk:bar"), Is.EqualTo(Namespace.UserTalk));

            Assert.That(Namespace.Determine("File:foo"), Is.EqualTo(Namespace.File));
            Assert.That(Namespace.Determine("Image:foo"), Is.EqualTo(Namespace.File));

            Assert.That(Namespace.Determine("Wikipedia:Foo"), Is.EqualTo(Namespace.Project));
            Assert.That(Namespace.Determine("Project:Foo"), Is.EqualTo(Namespace.Project));

            Assert.That(Namespace.Determine(@"Talk:ʿAyn"), Is.EqualTo(Namespace.Talk), "handles pages with spacing modifier Unicode characters at start of name");
            Assert.That(Namespace.Determine("Module:foo"), Is.EqualTo(Namespace.Module));
            Assert.That(Namespace.Determine("Module talk:foo"), Is.EqualTo(Namespace.ModuleTalk));
        }

        [Test]
        public void DetermineDeviations()
        {
            Assert.That(Namespace.Determine("File : foo"), Is.EqualTo(Namespace.File));
            Assert.That(Namespace.Determine("File :foo"), Is.EqualTo(Namespace.File));
            Assert.That(Namespace.Determine("File: foo"), Is.EqualTo(Namespace.File));
            Assert.That(Namespace.Determine("File :  foo"), Is.EqualTo(Namespace.File));
            Assert.That(Namespace.Determine("user:foo"), Is.EqualTo(Namespace.User));
            Assert.That(Namespace.Determine("user_talk:foo"), Is.EqualTo(Namespace.UserTalk));
            Assert.That(Namespace.Determine("user%20talk:foo"), Is.EqualTo(Namespace.UserTalk));
            Assert.That(Namespace.Determine("Book talk:Math"), Is.EqualTo(Namespace.BookTalk));
        }
        
        [Test]
        public void OtherLanguages()
        {
            #if DEBUG
            Variables.SetProjectLangCode("fr");
            Variables.Namespaces[1] = "Discussion:";
            Assert.That(Namespace.Determine(@"Discussion:foo"), Is.EqualTo(Namespace.Talk));
            Variables.SetProjectLangCode("en");
            Variables.Namespaces[1] = "Talk:";
            #endif
        }

        [Test]
        public void IsTalk()
        {
            ClassicAssert.IsTrue(Namespace.IsTalk(Namespace.Talk));
            ClassicAssert.IsTrue(Namespace.IsTalk(Namespace.UserTalk));

            ClassicAssert.IsFalse(Namespace.IsTalk(Namespace.User));
            ClassicAssert.IsFalse(Namespace.IsTalk(Namespace.Project));

            ClassicAssert.IsFalse(Namespace.IsTalk(Namespace.Special));
            ClassicAssert.IsFalse(Namespace.IsTalk(Namespace.Media));

            ClassicAssert.IsTrue(Namespace.IsTalk("Talk:Test"));
            ClassicAssert.IsTrue(Namespace.IsTalk("User talk:Test"));

            ClassicAssert.IsFalse(Namespace.IsTalk("Test"));
            ClassicAssert.IsFalse(Namespace.IsTalk("User:Test"));
        }

        [Test]
        public void IsMainSpace()
        {
            ClassicAssert.IsTrue(Namespace.IsMainSpace(Namespace.Article));
            ClassicAssert.IsFalse(Namespace.IsMainSpace(Namespace.Talk));

            ClassicAssert.IsTrue(Namespace.IsMainSpace("Test"));

            ClassicAssert.IsFalse(Namespace.IsMainSpace("User:"));
            ClassicAssert.IsFalse(Namespace.IsMainSpace("Talk:Test"));
            ClassicAssert.IsFalse(Namespace.IsMainSpace("User:Test"));
            ClassicAssert.IsFalse(Namespace.IsMainSpace("User talk:Test"));

            ClassicAssert.IsFalse(Namespace.IsMainSpace("File:Test"));
            ClassicAssert.IsFalse(Namespace.IsMainSpace("Image:Test"));
            ClassicAssert.IsFalse(Namespace.IsMainSpace("Book talk:Math"));
        }

        [Test]
        public void IsImportant()
        {
            ClassicAssert.IsTrue(Namespace.IsImportant("Test"));
            ClassicAssert.IsTrue(Namespace.IsImportant("File:Test.jpg"));
            ClassicAssert.IsTrue(Namespace.IsImportant("Template:Test"));
            ClassicAssert.IsTrue(Namespace.IsImportant("Category:Test"));

            ClassicAssert.IsFalse(Namespace.IsImportant("Talk:Test"));
            ClassicAssert.IsFalse(Namespace.IsImportant("File talk:Test"));
            ClassicAssert.IsFalse(Namespace.IsImportant("Template talk:Test"));
            ClassicAssert.IsFalse(Namespace.IsImportant("Category talk:Test"));
            ClassicAssert.IsFalse(Namespace.IsImportant("User:Test"));
        }

        [Test]
        public void IsUserSpace()
        {
            ClassicAssert.IsTrue(Namespace.IsUserSpace("User:Test"));
            ClassicAssert.IsTrue(Namespace.IsUserSpace("User talk:Test"));

            ClassicAssert.IsFalse(Namespace.IsUserSpace("User"));
            ClassicAssert.IsFalse(Namespace.IsUserSpace("Test"));
            ClassicAssert.IsFalse(Namespace.IsUserSpace("Project:User"));
        }

        [Test]
        public void IsUserTalk()
        {
            ClassicAssert.IsTrue(Namespace.IsUserTalk("User talk:Test"));

            ClassicAssert.IsFalse(Namespace.IsUserTalk("User:Test"));
            ClassicAssert.IsFalse(Namespace.IsUserTalk("Test"));
            ClassicAssert.IsFalse(Namespace.IsUserTalk("Project:User"));
        }

        [Test]
        public void IsUserPage()
        {
            ClassicAssert.IsTrue(Namespace.IsUserPage("User:Test"));

            ClassicAssert.IsFalse(Namespace.IsUserPage("User talk:Test"));
            ClassicAssert.IsFalse(Namespace.IsUserPage("Test"));
            ClassicAssert.IsFalse(Namespace.IsUserPage("Project:User"));
        }

        [Test]
        public void IsSpecial()
        {
            ClassicAssert.IsTrue(Namespace.IsSpecial(Namespace.Media));
            ClassicAssert.IsTrue(Namespace.IsSpecial(Namespace.Special));

            ClassicAssert.IsFalse(Namespace.IsSpecial(Namespace.Article));
            ClassicAssert.IsFalse(Namespace.IsSpecial(Namespace.TemplateTalk));
        }

        [Test]
        public void NormalizeNamespace()
        {
            Assert.That(Namespace.Normalize("User:", 2), Is.EqualTo("User:"));
            Assert.That(Namespace.Normalize("user :", 2), Is.EqualTo("User:"));
            Assert.That(Namespace.Normalize("User_talk:", 3), Is.EqualTo("User talk:"));

            Assert.That(Namespace.Normalize("image:", 6), Is.EqualTo("Image:"));
            Assert.That(Namespace.Normalize("file:", 6), Is.EqualTo("File:"));
            Assert.That(Namespace.Normalize("image talk:", 7), Is.EqualTo("Image talk:"));

            Assert.That(Namespace.Normalize("user :", 7), Is.EqualTo("user:"), "only changes colon for incorrect namespace number");
            Assert.That(Namespace.Normalize("user:", 7), Is.EqualTo("user:"), "only changes colon for incorrect namespace number");
        }

        [Test]
        public void Verify()
        {
            ClassicAssert.IsTrue(Namespace.VerifyNamespaces(Variables.CanonicalNamespaces));

            var ns = new Dictionary<int, string>(Variables.CanonicalNamespaces);
            
            ns[0] = "";
            ClassicAssert.IsFalse(Namespace.VerifyNamespaces(ns));

            ns.Remove(0);
            ns[1] = "Talk";
            ClassicAssert.IsFalse(Namespace.VerifyNamespaces(ns));
            
            ns.Remove(1);
            ClassicAssert.IsFalse(Namespace.VerifyNamespaces(ns));
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
            Assert.That(p1.AddBackText(s1), Is.EqualTo("<pre>foo bar</pre>"));
            Assert.That(p2.AddBackText(s2), Is.EqualTo("<source>quux</source>"));

            // in the future, we may use parser objects for processing several wikis at once
            // Assert.AreNotEqual(p1.StubMaxWordCount, p2.StubMaxWordCount);

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
            string correct = c + "\r\n" + a + "\r\n" + b + "\r\n" + d + "\r\n" + e + "\r\n" + f + "\r\n" + g + "\r\n";
            string articleText = b + "\r\n" + a + "\r\n" + c + "\r\n" + d + "\r\n" + e + "\r\n" + f + g;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(correct), "bacdefg with newlines");

            articleText = a + b + c + d + e + f + g;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(correct), "abcdefg");

            articleText = b + a + c + d + e + f + g;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(correct), "bacdefg without newlines");

            articleText = a + c + b + d + e + f + g;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(correct), "acbdefg");

            articleText = a + b + c + e + d + f + g;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(correct), "abcedfg");

            articleText = a + e + c + b + d + f + g;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(correct), "aecbdfg");

            articleText = a + e + c + b + d + g + f;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(correct), "aecbdgf");

            articleText = a + c + d + e + f + b + g;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(correct), "acdefbg");

            articleText = f + a + c + b + d + e + g;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(correct), "facbdeg");

            articleText = f + e + d + c + b + a + g;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(correct), "fedcbag");

            string h = @"{{WikiProject banner shell|1={{WikiProject Greece|class=}}}}", i = @"{{Image requested}}", j = @"{{Connected contributor|John Doe}}";

            correct = correct + h + "\r\n";
            articleText = correct;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(correct), "abcdefgh");

            correct = correct + i + "\r\n";

            articleText = correct;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(correct), "abcdefghi");

            articleText = b + "\r\n" + a + "\r\n" + c + "\r\n" + d + "\r\n" + e + "\r\n" + f + "\r\n" + h + "\r\n" + g + "\r\n" + i;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(correct), "bacdefhgi");

            correct = correct + j + "\r\n";

            articleText = correct;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(correct), "abcdefghij");

            articleText = b + "\r\n" + a + "\r\n" + c + "\r\n" + d + "\r\n" + e + "\r\n" + f + "\r\n" + h + "\r\n" + g + "\r\n" + j + "\r\n" + i;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(correct), "bacdefhgji");

            articleText = b + "\r\n" + a + "\r\n" + c + "\r\n" + d + "\r\n" + e + "\r\n" + f + "\r\n" + h + "\r\n" + g + "\r\n" + j + "\r\n" + i + "\r\n" + "\r\n" + "\r\n";
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(correct), "bacdefhgji with newlines at the end");

            articleText = b + "\r\n" + "\r\n" + "\r\n" + a + "\r\n" + c + "\r\n" + d + "\r\n" + e + "\r\n" + f + "\r\n" + h + "\r\n" + g + "\r\n" + j + "\r\n" + "\r\n" + "\r\n" + i + "\r\n" + "\r\n" + "\r\n";

            string k = @"{{To do}}", m = @"{{Find sources notice}}", n = @"{{Split from|foo}}";

            correct = correct + k + "\r\n";
            articleText = correct;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(correct), "abcdefghijk");

            string cop = correct + @"{{Copied}}" + "\r\n";
            articleText = cop;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(cop), "Templates including {{Copied}}");

            articleText = b + "\r\n" + a + "\r\n" + c + "\r\n" + d + "\r\n" + e + "\r\n" + f + "\r\n" + k + "\r\n" + g + "\r\n" + j + "\r\n" + i + "\r\n" + h;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(correct), "bacdefkgjih");

            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(correct), "bacdefhgji with newlines at the end and in the middle");

            articleText = b + "\r\n" + a + "\r\n" + c + "\r\n" + d + "\r\n" + e + "\r\n" + f + "\r\n" + k + "\r\n" + g + "\r\n" + j + "\r\n" + i + "\r\n" + h;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(correct), "bacdefkgjlih");

            correct = correct + m + "\r\n" + n + "\r\n";
            articleText = correct;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(correct), "abcdefghijklmn");

            articleText = b + "\r\n" + a + "\r\n" + c + "\r\n" + d + "\r\n" + e + "\r\n" + f + "\r\n" + k + "\r\n" + n + "\r\n" + g + "\r\n" + j + "\r\n" + m + "\r\n" + i + "\r\n" + h;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(correct), "bacdefkngjmlih");

            articleText = @"{{some other template}}" + "\r\n" + @"==Untitled==" + "\r\n" + @"some text";
            string articleText2 = articleText;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(articleText2), "no changes if nothing is detected; starts with template");

            articleText = @"{{GA|21:12, 11 May 2013 (UTC)|topic=Geography|page=1|oldid=554646767}}
{{WikiProject banner shell|1=
{{WikiProject Canada|geography=yes|class=Start|importance=Mid|bc=yes}}
{{WikiProject Protected areas|class=Start|importance=Mid}}
{{WikiProject Greece|class=Start}}
}}
{{DYK talk|17 April|2013|entry=... that Canada}}
{{some random template}}";
            articleText2 = @"{{GA|21:12, 11 May 2013 (UTC)|topic=Geography|page=1|oldid=554646767}}
{{DYK talk|17 April|2013|entry=... that Canada}}
{{WikiProject banner shell|1=
{{WikiProject Canada|geography=yes|class=Start|importance=Mid|bc=yes}}
{{WikiProject Protected areas|class=Start|importance=Mid}}
{{WikiProject Greece|class=Start}}
}}

{{some random template}}";
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(articleText2), "articleilestonene templates above wikiproject banners; contains WPBS");

            articleText = @"{{GA|21:12, 11 May 2013 (UTC)|topic=Geography|page=1|oldid=554646767}}
{{WikiProject Canada|geography=yes|class=Start|importance=Mid|bc=yes}}
{{WikiProject Protected areas|class=Start|importance=Mid}}
{{some random template}}";
            articleText2 = articleText;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(articleText2), "no changes if everything is in place; contains 2 Wikiprojects");
        }
        
         [Test]
        public void MoveBannersAndWikiProjects()
        {
            string a = @"{{Skip to talk}}", b = @"{{Talk header}}", c = @"{{GA nominee}}", d = @"{{Controversial}}", e = @"{{Not a forum}}", f = @"{{British English}}", g = @"{{FailedGA}}", h = @"{{WikiProject Greece|class=}}", i = @"{{Image requested}}", j = @"{{Connected contributor|John Doe}}";
            string correct = c + "\r\n" + a + "\r\n" + b + "\r\n" + d + "\r\n" + e + "\r\n" + f + "\r\n" + g + "\r\n" + h + "\r\n" + i + "\r\n";
            string articleText = correct;

            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(correct), "abcdefghi");

            articleText = b + "\r\n" + a + "\r\n" + c + "\r\n" + d + "\r\n" + e + "\r\n" + f + "\r\n" + h + "\r\n" + g + "\r\n" + i;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(correct), "bacdefhgi");

            articleText = h + "\r\n" + c + "\r\n" + a + "\r\n" + d + "\r\n" + e + "\r\n" + f + "\r\n" + b + "\r\n" + g + "\r\n" + i;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(correct), "hcadefbgi");

            articleText = d + "\r\n" + c + "\r\n" + a + "\r\n" + h + "\r\n" + f + "\r\n" + e + "\r\n" + b + "\r\n" + i + "\r\n" + g;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(correct), "dcahfebig");

            correct = correct + j + "\r\n";

            articleText = correct;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(correct), "abcdefghij");

            articleText = b + "\r\n" + a + "\r\n" + c + "\r\n" + d + "\r\n" + e + "\r\n" + f + "\r\n" + h + "\r\n" + g + "\r\n" + j + "\r\n" + i;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(correct), "bacdefhgji");

            articleText = b + "\r\n" + a + "\r\n" + c + "\r\n" + d + "\r\n" + e + "\r\n" + f + "\r\n" + h + "\r\n" + g + "\r\n" + j + "\r\n" + i + "\r\n" + "\r\n" + "\r\n";
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(correct), "bacdefhgji with newlines at the end");

            articleText = b + "\r\n" + "\r\n" + "\r\n" + a + "\r\n" + c + "\r\n" + d + "\r\n" + e + "\r\n" + f + "\r\n" + h + "\r\n" + g + "\r\n" + j + "\r\n" + "\r\n" + "\r\n" + i + "\r\n" + "\r\n" + "\r\n";

            string k = @"{{To do}}";

            correct = correct + k + "\r\n";
            articleText = correct;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(correct), "abcdefghijk");

            articleText = b + "\r\n" + a + "\r\n" + c + "\r\n" + d + "\r\n" + e + "\r\n" + f + "\r\n" + k + "\r\n" + g + "\r\n" + j + "\r\n" + i + "\r\n" + h;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(correct), "bacdefkgjih");

            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(correct), "bacdefhgji with newlines at the end and in the middle");
        }

        [Test]
        public void MoveBannersAndSpacing()
        {
            string articleText = @"==Untitled==" + "\r\n" + @"some text";
            string articleText2 = articleText;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(articleText2), "no changes if nothing is detected; starts with header");
            
            articleText = @"{{afd-merged-from|George Piggins Medal|George Piggins Medal|10 June 2012}}

{{User:WildBot/m04|sect={{User:WildBot/m03|1|History of rugby league#Professional rugby begins in Australia|schism}}|m04}}
{{ArticleHistory
|action1=FAC

|currentstatus=GA
|topic=Everydaylife}}
{{WikiProject banner shell|1=
{{WikiProject Rugby league|class=GA|importance=High}}
}}

== older entries ==
The";

            articleText2 = @"{{ArticleHistory
|action1=FAC

|currentstatus=GA
|topic=Everydaylife}}
{{afd-merged-from|George Piggins Medal|George Piggins Medal|10 June 2012}}
{{WikiProject banner shell|1=
{{WikiProject Rugby league|class=GA|importance=High}}
}}

{{User:WildBot/m04|sect={{User:WildBot/m03|1|History of rugby league#Professional rugby begins in Australia|schism}}|m04}}

== older entries ==
The";
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(articleText2), "unknown templates get below known ones");

            articleText = @"{{Talk header}}
{{WikiProject banner shell|1=
{{WikiProject Architecture|class=Start |importance=Low }}
{{WikiProject Virginia|class=Start |importance=Low }}
{{WikiProject National Register of Historic Places|class=start|importance=low}}
{{Image requested|in=Virginia}}
}}";
            articleText2 = @"{{Talk header}}
{{WikiProject banner shell|1=
{{WikiProject Architecture|class=Start |importance=Low }}
{{WikiProject Virginia|class=Start |importance=Low }}
{{WikiProject National Register of Historic Places|class=start|importance=low}}
}}
{{Image requested|in=Virginia}}
";
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(articleText2), "image requested is moved away from WPBS; at some point we could fix the whitespace inside WPBS");

            articleText = @"{{Talk header|search=yes}}

==Random header==
Some text";
            articleText2 = articleText;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(articleText2), "no changes if nothing is detected; starts with talk header");

            articleText = @"{{Not a forum}}

==Random header==
Some text";
            articleText2 = articleText;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(articleText2), "no changes if nothing is detected; starts with not a forum");

            articleText = @"{{Random template}}

==Random header==
Some text";
            articleText2 = articleText;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(articleText2), "no changes if nothing is detected; starts with a random template");

            articleText = @"{{Random template}}
{{Yet another random template}}

==Random header==
Some text";
            articleText2 = articleText;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(articleText2), "no changes if nothing is detected; starts with two random templates");

            articleText = @"{{Skip to talk}}
{{Talk header}}
{{Random template}}

==Random header==
Some text";
            articleText2 = articleText;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(articleText2), "no changes if nothing is detected; starts with two skt, th and a random template");

            articleText = @"{{Skip to talk}}
{{Not a forum}}
{{Random template}}

==Random header==
Some text";
            articleText2 = articleText;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(articleText2), "no changes if nothing is detected; starts with skt, naf and a random template");

            articleText = @"{{Not a forum}}
{{Skip to talk}}
{{Random template}}

==Random header==
Some text";
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(articleText2), "moves skip to talk on the top 1");


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
            Assert.That(articleText, Is.EqualTo(articleText2), "moves skip to talk on the top 2");

            articleText = @"{{Talk header}}
{{WikiProject banner shell|1=
{{WikiProject Iraq|class=C|importance=mid}}
{{WikiProject Crime|class=C|importance=low}}
{{WikiProject Terrorism|class=C|importance=low}}
}}
{{image requested}}

== Reliable sources ==";
            articleText2 = articleText;
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(articleText2), "No change, heading whitespace not affected");
        }

        [Test]
        public void MoveTalkHeader()
        {
            string talkheader = @"{{talk header|noarchive=no|search=no|arpol=no|wp=no|disclaimer=no|shortcut1|shortcut2}}", talkrest = @"==hello==
hello talk";
            string articleText = talkrest + "\r\n" + talkheader;
            
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);

            Assert.That(articleText, Is.EqualTo(talkheader.Replace("{{talk", @"{{Talk") + "\r\n" + talkrest + "\r\n"), "move talk header");
            
            // handles {{talk header}} on same line as other template
            string WPBS = @"{{WikiProject banner shell|blp=yes|1=
{{OH-Project|class=B|importance=Low|nested=yes}}
{{WPBiography|living=yes|class=B|priority=Low|filmbio-work-group=yes|nested=yes|listas=Parker, Sarah Jessica}}
{{WikiProject Cincinnati|class=B|importance=mid|nested=yes}}
}}", rest = "\r\n\r\n" + @"==Song Jessie by Joshua Kadison==
In the article it says that above mentioned";
            articleText = WPBS + @"{{Talkheader}}" + rest;
            
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);

            Assert.That(articleText, Is.EqualTo(@"{{Talk header}}" + "\r\n" + WPBS + rest), "talk header on same line as other template");
            
            // no change if already at top
            articleText = talkheader + "\r\n" + talkrest;
            
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(talkheader + "\r\n" + talkrest), "talk header already on top");
            
            // no change if no talk header
            articleText = talkrest;
            
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(talkrest), "no talk header");

            talkheader = @"{{Talk header}}";
            talkrest = @"==hello==
hello talk";
            articleText = talkrest + "\r\n" + talkheader + "\r\n{{GA nominee}}";

            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);

            Assert.That(articleText, Is.EqualTo("{{GA nominee}}\r\n" + talkheader + "\r\n" + talkrest + "\r\n"), "GA nominee involved");
        }
        
        [Test]
        public void RenameTalkHeader()
        {
            string talkheader = @"{{talkheader|noarchive=no}}", talkrest = @"==hello==
hello talk";
            string articleText = talkrest + "\r\n" + talkheader;
            
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);

            Assert.That(articleText, Is.EqualTo(@"{{Talk header|noarchive=no}}" + "\r\n" + talkrest + "\r\n"), "renamed to upper case with space");
        }
        
        [Test]
        public void TalkHeaderDefaultsortChange()
        {
            string start = @"
==Foo==
bar", df = @"{{DEFAULTSORT:Bert}}";
            
            string articleText = start + "\r\n" + df;
            
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.MoveToBottom);
            Assert.That(articleText, Is.EqualTo(start + "\r\n" + "\r\n" + df), "removes second newline");
            
            articleText = start + df;
            
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.MoveToTop);
            Assert.That(articleText, Is.EqualTo(df + "\r\n" + start), "moves df after text");
            
            articleText = start + df;
            
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(start + df), "no changes");
            
            string df2 = @"{{DEFAULTSORT:}}";
            
            articleText = start + df2;
            
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.MoveToBottom);
            Assert.That(articleText, Is.EqualTo(start), "defaultsort with no key removed");
            
            articleText = start + df2;
            
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.MoveToTop);
            Assert.That(articleText, Is.EqualTo(start), "defaultsort with no key removed");
        }
        
        [Test]
        public void SkipToTalk()
        {
            string articleText = @"{{Skiptotoc}}", stt = @"{{Skip to talk}}";
            
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(stt + "\r\n"));
            
            articleText = @"{{skiptotoctalk}}";
            
            TalkPageFixes.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);
            Assert.That(articleText, Is.EqualTo(stt + "\r\n"));
        }
        
        [Test]
        public void AddMissingFirstCommentHeader()
        {
            const string comment = @"
Hello world comment.";
            string articleTextIn = articleTextHeader + comment;
            
            // plain comment
            TalkPageFixes.ProcessTalkPage(ref articleTextIn, DEFAULTSORT.NoChange);

            Assert.That(articleTextIn, Is.EqualTo(articleTextHeader + "\r\n" + @"
==Untitled==
Hello world comment."));
            
            // idented comment2
            articleTextIn = articleTextHeader + @"
*Hello world comment2.";
            
            TalkPageFixes.ProcessTalkPage(ref articleTextIn, DEFAULTSORT.NoChange);

            Assert.That(articleTextIn, Is.EqualTo(articleTextHeader + "\r\n" + @"
==Untitled==
*Hello world comment2."));
            
            // idented comment3
            articleTextIn = articleTextHeader + @"
:Hello world comment3.";
            
            TalkPageFixes.ProcessTalkPage(ref articleTextIn, DEFAULTSORT.NoChange);

            Assert.That(articleTextIn, Is.EqualTo(articleTextHeader + "\r\n" + @"
==Untitled==
:Hello world comment3."));
            
            // quoted comment
            articleTextIn = articleTextHeader + @"
""Hello world comment4"".";
            
            TalkPageFixes.ProcessTalkPage(ref articleTextIn, DEFAULTSORT.NoChange);

            Assert.That(articleTextIn, Is.EqualTo(articleTextHeader + "\r\n" + @"
==Untitled==
""Hello world comment4""."));
            
            // heading level 3 changed to level 2
            articleTextIn = articleTextHeader + "\r\n" + @"===Foo bar===
*Hello world comment2.";
            
            TalkPageFixes.ProcessTalkPage(ref articleTextIn, DEFAULTSORT.NoChange);
            
            ClassicAssert.IsTrue(articleTextIn.Contains(@"==Foo bar=="));
            ClassicAssert.IsFalse(articleTextIn.Contains(@"==Untitled=="));
            
            articleTextIn = comment;

            TalkPageFixes.ProcessTalkPage(ref articleTextIn, DEFAULTSORT.NoChange);

            Assert.That(articleTextIn, Is.EqualTo(@"==Untitled==
Hello world comment."), "don't add blank line when header is on the top");

            articleTextIn = @"{{Football}}" +"\r\n" + comment;

            TalkPageFixes.ProcessTalkPage(ref articleTextIn, DEFAULTSORT.NoChange);

            Assert.That(articleTextIn, Is.EqualTo(@"{{Football}}

==Untitled==
Hello world comment."), "add header between template and text");

        }

        [Test]
        public void AddMissingFirstCommentHeaderLocalization()
        {
#if DEBUG
            Variables.SetProjectLangCode("zh");
            Assert.That(TalkPageFixes.WikiProjectBannerShell(@"{{WikiProject banner shell|blp=yes|blp=yes}}"), Is.EqualTo(@"{{WikiProject banner shell|blp=yes|blp=yes}}"));

            const string comment = @"
Hello world comment.";
            string articleTextIn = articleTextHeader + comment;

            // plain comment
            TalkPageFixes.ProcessTalkPage(ref articleTextIn, DEFAULTSORT.NoChange);

            Assert.That(articleTextIn, Is.EqualTo(articleTextHeader + "\r\n" + @"
==無標題==
Hello world comment."));
            Variables.SetProjectLangCode("en");
#endif
        }

        [Test]
        public void AddMissingFirstCommentHeaderNoChanges()
        {
            // no change – header already
            string articleTextIn = articleTextHeader + @"
==Question==
:Hello world comment3.";
            
            
            TalkPageFixes.ProcessTalkPage(ref articleTextIn, DEFAULTSORT.NoChange);

            Assert.That(articleTextIn, Is.EqualTo(articleTextHeader + @"
==Question==
:Hello world comment3."));
            
            // no change – already header at top
            articleTextIn = @"
{{Some template}}
==Question==
:Hello world comment3.";
            

            TalkPageFixes.ProcessTalkPage(ref articleTextIn, DEFAULTSORT.NoChange);

            Assert.That(articleTextIn, Is.EqualTo(@"
{{Some template}}
==Question==
:Hello world comment3."));
            
            // no change – already header at top 2
            articleTextIn = @"
==Question==
{{Some template}}
:Hello world comment3.";
            
            TalkPageFixes.ProcessTalkPage(ref articleTextIn, DEFAULTSORT.NoChange);

            Assert.That(articleTextIn, Is.EqualTo(@"
==Question==
{{Some template}}
:Hello world comment3."));
            
            // no change – no comments
            articleTextIn = @"
==Question==
{{Some template}}";
            
            TalkPageFixes.ProcessTalkPage(ref articleTextIn, DEFAULTSORT.NoChange);

            Assert.That(articleTextIn, Is.EqualTo(@"
==Question==
{{Some template}}"));
            
            // no change – only text in template
            articleTextIn = @"
{{foo|
bar|
end}}";
            
            TalkPageFixes.ProcessTalkPage(ref articleTextIn, DEFAULTSORT.NoChange);

            Assert.That(articleTextIn, Is.EqualTo(@"
{{foo|
bar|
end}}"));
            
            // no change – only comments
            articleTextIn = @"
<!--
foo
-->";
            
            TalkPageFixes.ProcessTalkPage(ref articleTextIn, DEFAULTSORT.NoChange);

            Assert.That(articleTextIn, Is.EqualTo(@"
<!--
foo
-->"));
            
            // no change – only TOC
            articleTextIn = @"
__TOC__";
            
            TalkPageFixes.ProcessTalkPage(ref articleTextIn, DEFAULTSORT.NoChange);

            Assert.That(articleTextIn, Is.EqualTo(@"
__TOC__"));
            
            // no change -- only in template
            const string allInTemplate = @"{{archive box|
*[[/Archive: GA review|Good Article review]]}}

== Explanation of Wright's work in ''Certaine Errors'' ==";
            
            articleTextIn = allInTemplate;
            
            TalkPageFixes.ProcessTalkPage(ref articleTextIn, DEFAULTSORT.NoChange);

            Assert.That(articleTextIn, Is.EqualTo(allInTemplate));
            
            // no change -- only after template on same line
            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_15#Section_header_added_in_wrong_position
            const string allAfterTemplate = @"{{archive box|words}} extra foo

{{another one}}";
            
            articleTextIn = allAfterTemplate;
            
            TalkPageFixes.ProcessTalkPage(ref articleTextIn, DEFAULTSORT.NoChange);

            Assert.That(articleTextIn, Is.EqualTo(allAfterTemplate));

            // no change – only text in gallery tags
            articleTextIn = @"
<gallery>
File:Example.jpg|Caption1
File:Example.jpg|Caption2
</gallery>";

            TalkPageFixes.ProcessTalkPage(ref articleTextIn, DEFAULTSORT.NoChange);

            Assert.That(articleTextIn, Is.EqualTo(@"
<gallery>
File:Example.jpg|Caption1
File:Example.jpg|Caption2
</gallery>"));

            // no change – don't pick up text within wiki table
            articleTextIn = @"
{|
|Foo
bar
|}";

            TalkPageFixes.ProcessTalkPage(ref articleTextIn, DEFAULTSORT.NoChange);

            Assert.That(articleTextIn, Is.EqualTo(@"
{|
|Foo
bar
|}"));
        }
        
        [Test]
        public void WikiProjectBannerShellRedirects()
        {
            string red1 = @"{{WPBS}}", red2 = @"{{Wikiprojectbannershell}}", red3 = @"{{wpb}}", red4 = @"{{WPB}}",

            WikiProjectBannerShell = @"{{WikiProject banner shell}}",
            WikiProjectBanners = @"{{WikiProjectBanners}}";

            Assert.That(TalkPageFixes.WikiProjectBannerShell(red1), Is.EqualTo(WikiProjectBannerShell));
            Assert.That(TalkPageFixes.WikiProjectBannerShell(red2), Is.EqualTo(WikiProjectBannerShell));
            Assert.That(TalkPageFixes.WikiProjectBannerShell(WikiProjectBannerShell), Is.EqualTo(WikiProjectBannerShell));
            Assert.That(TalkPageFixes.WikiProjectBannerShell(red3), Is.EqualTo(WikiProjectBannerShell));
            Assert.That(TalkPageFixes.WikiProjectBannerShell(red4), Is.EqualTo(WikiProjectBannerShell));
            Assert.That(TalkPageFixes.WikiProjectBannerShell(WikiProjectBanners), Is.EqualTo(WikiProjectBannerShell));
        }
        
        [Test]
        public void WikiProjectBannerShellEnOnly()
        {
            #if DEBUG
            Variables.SetProjectLangCode("fr");
            Assert.That(TalkPageFixes.WikiProjectBannerShell(@"{{WikiProject banner shell|blp=yes|blp=yes}}"), Is.EqualTo(@"{{WikiProject banner shell|blp=yes|blp=yes}}"));
            Variables.SetProjectLangCode("en");
            Assert.That(TalkPageFixes.WikiProjectBannerShell(@"{{WikiProject banner shell|blp=yes|blp=yes}}"), Is.EqualTo(@"{{WikiProject banner shell|blp=yes}}"));
            #endif
        }
        
        [Test]
        public void WikiProjectBannerShellDupeParameters()
        {
            Assert.That(TalkPageFixes.WikiProjectBannerShell(@"{{WikiProject banner shell|blp=yes|blp=yes}}"), Is.EqualTo(@"{{WikiProject banner shell|blp=yes}}"));
            Assert.That(TalkPageFixes.WikiProjectBannerShell(@"{{WikiProject banner shell|blpo=yes|blpo=yes|blp=yes|blp=yes}}"), Is.EqualTo(@"{{WikiProject banner shell|blpo=yes|blp=yes}}"));
            Assert.That(TalkPageFixes.WikiProjectBannerShell(@"{{WikiProject banner shell|collapsed=yes|collapsed=yes|collapsed=yes}}"), Is.EqualTo(@"{{WikiProject banner shell|collapsed=yes}}"));
            Assert.That(TalkPageFixes.WikiProjectBannerShell(@"{{WikiProject banner shell||collapsed=yes}}"), Is.EqualTo(@"{{WikiProject banner shell|collapsed=yes}}"), "excess pipes cleaned");
        }
        
        [Test]
        public void WikiProjectBannerShellUnneededParams()
        {
            Assert.That(TalkPageFixes.WikiProjectBannerShell(@"{{WikiProject banner shell|blp=no|activepol=no|collapsed=no|blpo=no}}"), Is.EqualTo(@"{{WikiProject banner shell|blp=no}}"), "Retain blp=no");
            Assert.That(TalkPageFixes.WikiProjectBannerShell(@"{{WikiProject banner shell|blp=no|activepol=no|collapsed=yes|blpo=no}}"), Is.EqualTo(@"{{WikiProject banner shell|blp=no|collapsed=yes}}"), "Retain blp=no");
            Assert.That(TalkPageFixes.WikiProjectBannerShell(@"{{WikiProjectBanners|activepol=no|collapsed=no|blpo=no}}"), Is.EqualTo(@"{{WikiProject banner shell}}"));
            Assert.That(TalkPageFixes.WikiProjectBannerShell(@"{{WikiProjectBanners|activepol=no|collapsed=yes|blpo=no}}"), Is.EqualTo(@"{{WikiProject banner shell|collapsed=yes}}"));
        }
        
        [Test]
        public void WikiProjectBannerShellWPBiography()
        {
            Assert.That(TalkPageFixes.WikiProjectBannerShell(@"{{WikiProject banner shell|1={{WPBiography|foo=bar}}}}"), Is.EqualTo(@"{{WikiProject banner shell|1={{WPBiography|foo=bar}}}}"));

            Assert.That(TalkPageFixes.WikiProjectBannerShell(@"{{WikiProject banner shell|blp=|1={{WPBiography|foo=bar|living=yes}}}}"), Is.EqualTo(@"{{WikiProject banner shell|blp=yes|1={{WPBiography|foo=bar|living=yes}}}}"), "appends blp=yes to WPBS");
            Assert.That(TalkPageFixes.WikiProjectBannerShell(@"{{WikiProject banner shell|blp=|1={{WikiProject Biography|foo=bar|living=yes}}}}"), Is.EqualTo(@"{{WikiProject banner shell|blp=yes|1={{WikiProject Biography|foo=bar|living=yes}}}}"), "appends blp=yes to WPBS");
            Assert.That(TalkPageFixes.WikiProjectBannerShell(@"{{WikiProject banner shell|activepol=abc|1={{WPBiography|foo=bar|activepol=yes}}}}"), Is.EqualTo(@"{{WikiProject banner shell|activepol=yes|1={{WPBiography|foo=bar|activepol=yes}}}}"), "ignores invalid values");
            Assert.That(TalkPageFixes.WikiProjectBannerShell(@"{{WikiProject banner shell|blpo=|1={{WPBiography|foo=bar|blpo=yes}}}}"), Is.EqualTo(@"{{WikiProject banner shell|blpo=yes|1={{WPBiography|foo=bar|blpo=yes}}}}"), "appends blpo=yes to WPBS");
            Assert.That(TalkPageFixes.WikiProjectBannerShell(@"{{WikiProject banner shell|blpo=|1={{WPBiography|foo=bar|blpo=no}}}}"), Is.EqualTo(@"{{WikiProject banner shell|blpo=|1={{WPBiography|foo=bar|blpo=no}}}}"));

            Assert.That(TalkPageFixes.WikiProjectBannerShell(@"{{WikiProject banner shell|blp=yes|1={{WPBiography|foo=bar|living=no}}}}"), Is.EqualTo(@"{{WikiProject banner shell|1={{WPBiography|foo=bar|living=no}}}}"));
        }
        
        [Test]
        public void WikiProjectBannerShellAddingWikiProjects()
        {
            Assert.That(TalkPageFixes.WikiProjectBannerShell(@"{{WikiProject banner shell|1={{WPBiography|foo=bar}}}}
{{WikiProject foo}}"), Is.EqualTo(@"{{WikiProject banner shell|1={{WPBiography|foo=bar}}
{{WikiProject foo}}}}
"), "WikiProjects pulled into WPBS");

            Assert.That(TalkPageFixes.WikiProjectBannerShell(@"{{WikiProject banner shell|1={{WPBiography|foo=bar}}}}
{{WikiProject foo}}
{{WikiProject bar}}"), Is.EqualTo(@"{{WikiProject banner shell|1={{WPBiography|foo=bar}}
{{WikiProject foo}}
{{WikiProject bar}}}}

"), "WikiProjects pulled into WPBS");

            Assert.That(TalkPageFixes.WikiProjectBannerShell(@"{{WikiProject Biography|living=yes|foo=bar}}{{WikiProject banner shell|1={{WikiProject foo}}
{{WikiProject bar}}}}"), Is.EqualTo(@"{{WikiProject banner shell|1={{WikiProject foo}}
{{WikiProject bar}}
{{WikiProject Biography|living=yes|foo=bar}} | blp=yes}}"), "WikiProjects pulled into WPBS, WPBIO contains living=yes");

            Assert.That(TalkPageFixes.WikiProjectBannerShell(@"{{WikiProject Biography|activepol=yes|foo=bar}}{{WikiProject banner shell|1={{WikiProject foo}}
{{WikiProject bar}}}}"), Is.EqualTo(@"{{WikiProject banner shell|1={{WikiProject foo}}
{{WikiProject bar}}
{{WikiProject Biography|activepol=yes|foo=bar}} | activepol=yes}}"), "WikiProjects pulled into WPBS, WPBIO contains activepol=yes");

            Assert.That(TalkPageFixes.WikiProjectBannerShell(@"{{WikiProject Biography|living=yes|activepol=yes|foo=bar}}{{WikiProject banner shell|1={{WikiProject foo}}
{{WikiProject bar}}}}"), Is.EqualTo(@"{{WikiProject banner shell|1={{WikiProject foo}}
{{WikiProject bar}}
{{WikiProject Biography|living=yes|activepol=yes|foo=bar}} | blp=yes | activepol=yes}}"), "WikiProjects pulled into WPBS, WPBIO contains living, activepol=yes");

            Assert.That(TalkPageFixes.WikiProjectBannerShell(@"{{WikiProject bar}}

{{WikiProject banner shell|1={{WPBiography|foo=bar}}}}
{{WikiProject foo}}"), Is.EqualTo(@"{{WikiProject banner shell|1={{WPBiography|foo=bar}}
{{WikiProject bar}}
{{WikiProject foo}}}}
"), "WikiProjects pulled into WPBS, no excess whitespace left");

            const string VitalArticle = @"{{WikiProject banner shell|1={{WPBiography|foo=bar}}
{{WikiProject foo}}
{{Vital article|level=4|topic=Biology|class=FA}}}}
";

            Assert.That(TalkPageFixes.WikiProjectBannerShell(VitalArticle), Is.EqualTo(VitalArticle), "Support Vital article - already inside");

            Assert.That(TalkPageFixes.WikiProjectBannerShell(@"{{Vital article|level=4|topic=Biology|class=FA}}
{{WikiProject banner shell|1={{WPBiography|foo=bar}}
{{WikiProject foo}}}}
"), Is.EqualTo(VitalArticle), "Support Vital article - put inside");

        }
        
        [Test]
        public void WikiProjectBannerShellUnnamedParam()
        {
            Assert.That(TalkPageFixes.WikiProjectBannerShell(@"{{WikiProject banner shell|{{WPBiography|foo=bar}}}}"), Is.EqualTo(@"{{WikiProject banner shell|1={{WPBiography|foo=bar}}}}"), "1= added when missing");
            Assert.That(TalkPageFixes.WikiProjectBannerShell(@"{{WikiProject banner shell|
{{WPBiography|foo=bar}}}}"), Is.EqualTo(@"{{WikiProject banner shell|1=
{{WPBiography|foo=bar}}}}"));
            
            const string otherUnnamed = @"{{WikiProject banner shell|random}}";
            Assert.That(TalkPageFixes.WikiProjectBannerShell(otherUnnamed), Is.EqualTo(otherUnnamed), "other unknown parameter not named 1=");
        }
        
        [Test]
        public void WikiProjectBannerShellBLP()
        {
            const string a = @"{{WikiProject banner shell|blp=yes|1={{WPBiography|foo=bar|living=yes}}}}";

            Assert.That(TalkPageFixes.WikiProjectBannerShell(a + "{{Blp}}"), Is.EqualTo(a), "removes redundant banner");
            Assert.That(TalkPageFixes.WikiProjectBannerShell(a.Replace("blp=yes", "blp=") + "{{Blp}}"), Is.EqualTo(a), "empty parameter in WPBS");
            Assert.That(TalkPageFixes.WikiProjectBannerShell("{{Blp}}"), Is.EqualTo("{{Blp}}"));
        }
        
        [Test]
        public void WikiProjectBannerShellBLPO()
        {
            const string a = @"{{WikiProject banner shell|blpo=yes|1={{WPBiography|foo=bar}}}}";

            Assert.That(TalkPageFixes.WikiProjectBannerShell(a + "{{Blpo}}"), Is.EqualTo(a), "removes redundant banner");
            Assert.That(TalkPageFixes.WikiProjectBannerShell(a + "{{BLPO}}"), Is.EqualTo(a), "removes redundant banner");
            Assert.That(TalkPageFixes.WikiProjectBannerShell(a + "{{BLP others}}"), Is.EqualTo(a), "removes redundant banner");
            Assert.That(TalkPageFixes.WikiProjectBannerShell(a.Replace("blpo=yes", "blpo=") + "{{Blpo}}"), Is.EqualTo(a), "empty parameter in WPBS");
            Assert.That(TalkPageFixes.WikiProjectBannerShell("{{Blpo}}"), Is.EqualTo("{{Blpo}}"));
        }
        
        [Test]
        public void WikiProjectBannerShellActivepol()
        {
            const string a = @"{{WikiProject banner shell|activepol=yes|1={{WPBiography|foo=bar|activepol=yes}}}}";

            Assert.That(TalkPageFixes.WikiProjectBannerShell(a + "{{Activepol}}"), Is.EqualTo(a), "removes redundant banner");
            Assert.That(TalkPageFixes.WikiProjectBannerShell(a.Replace("activepol=yes|", "activepol=|") + "{{activepol}}"), Is.EqualTo(a), "empty parameter in WPBS");
            Assert.That(TalkPageFixes.WikiProjectBannerShell("{{activepol}}"), Is.EqualTo("{{activepol}}"));
        }
        
        [Test]
        public void WikiProjectBannerShellMisc()
        {
            const string a = @"{{wpbs|1=|banner collapsed=no|
{{WPBiography|living=yes|class=Start|priority=|listas=Hill, A}}
{{WikiProject Gender Studies}}
{{WikiProject Oklahoma}}
}}", b = @"{{WikiProject banner shell|banner collapsed=no|1=
{{WPBiography|living=yes|class=Start|priority=|listas=Hill, A}}
{{WikiProject Gender Studies}}
{{WikiProject Oklahoma}}
| blp=yes
}}";
            Assert.That(TalkPageFixes.WikiProjectBannerShell(a), Is.EqualTo(b));
        }
        
        [Test]
        public void AddWikiProjectBannerShell()
        {
            const string a = @"{{WikiProject a|text}}", b = @"{{WikiProject b|text}}", c = @"{{WikiProject c|text}}", d = @"{{WikiProject d|text}}";
            Assert.That(TalkPageFixes.WikiProjectBannerShell(a), Is.EqualTo(a));
            Assert.That(TalkPageFixes.WikiProjectBannerShell(a + b), Is.EqualTo(a + b));
            Assert.That(TalkPageFixes.WikiProjectBannerShell(a + b + c), Is.EqualTo(@"{{WikiProject banner shell|1=" + "\r\n" + a + "\r\n" + b + "\r\n" + c + "\r\n" + @"}}"), "banner shell added for 3 or more wikiproject links");
            Assert.That(TalkPageFixes.WikiProjectBannerShell(a + b + c + d), Is.EqualTo(@"{{WikiProject banner shell|1=" + "\r\n" + a + "\r\n" + b + "\r\n" + c + "\r\n" + d + "\r\n" + @"}}"));
            
            const string e = @"{{talk header}}
{{WikiProject Biography|listas=Bar, F|living=yes|class=Start|priority=mid|sports-work-group=yes}}
{{WikiProject F|class=Start|importance=mid|italy=y}}
{{WikiProject X|class=Start|importance=Mid}}
{{WikiProject D|class=Start|importance=Low}}", f = @"{{WikiProject banner shell|1=
{{WikiProject Biography|listas=Bar, F|living=yes|class=Start|priority=mid|sports-work-group=yes}}
{{WikiProject F|class=Start|importance=mid|italy=y}}
{{WikiProject X|class=Start|importance=Mid}}
{{WikiProject D|class=Start|importance=Low}}
| blp=yes
}}{{talk header}}

";

            Assert.That(TalkPageFixes.WikiProjectBannerShell(e), Is.EqualTo(f), "adds WPBS, ignores non-wikiproject templates");
        }
        
        [Test]
        public void RemoveTemplateNamespace()
        {
            string T1 = @"{{Template:Foo}}", T2 = @"{{Template:Foo}}
==Section==
{{Template:Bar}}";
            
            ClassicAssert.IsFalse(TalkPageFixes.ProcessTalkPage(ref T1, DEFAULTSORT.NoChange));
            Assert.That(T1, Is.EqualTo("{{Foo}}"), "template namespace removed");
            
            ClassicAssert.IsFalse(TalkPageFixes.ProcessTalkPage(ref T2, DEFAULTSORT.NoChange));
            Assert.That(T2, Is.EqualTo(@"{{Foo}}
==Section==
{{Template:Bar}}"), "changes only made in zeroth section");
        }
        
        [Test]
        public void WPBiographyTopBilling()
        {
            string a = @"{{WPBiography|foo=yes|living=yes}}
{{WikiProject London}}
",  b = @"{{WikiProject banner shell|banner collapsed=no|1=
{{WPBiography|living=yes|class=Start|priority=|listas=Hill, A}}
{{WikiProject Gender Studies}}
{{WikiProject Oklahoma}}
| blp=yes
}}", c = @"{{WPBiography|foo=yes|living=no}}
{{WikiProject London}}";
            Assert.That(TalkPageFixes.WPBiography(@"{{WikiProject London}}
{{WPBiography|foo=yes|living=yes}}"), Is.EqualTo(a), "WPBiography moved above WikiProjects");
            Assert.That(TalkPageFixes.WPBiography(a), Is.EqualTo(a), "no change when WPBiography ahead of WikiProjects");
            Assert.That(TalkPageFixes.WPBiography(b), Is.EqualTo(b), "no change when WPBS present");
            Assert.That(TalkPageFixes.WPBiography(c), Is.EqualTo(c), "no change when not living");
            Assert.That(TalkPageFixes.WPBiography(c + @"{{blp}}"), Is.EqualTo(c + @"{{blp}}"), "no change when not living");

            Assert.That(TalkPageFixes.WPBiography(a + @"{{blp}}"), Is.EqualTo(a), "blp template removed when living=y");
        }
        
        [Test]
        public void WPBiographyBLPActivepol()
        {
            string a = @"{{WPBiography}}";

            Assert.That(TalkPageFixes.WPBiography(a + @"{{blp}}"), Is.EqualTo(a.Replace(@"}}", " | living=yes}}")), "Add blp to WPBiography");
            Assert.That(TalkPageFixes.WPBiography(a.Replace(@"}}", " |living=}}") + @"{{blp}}"), Is.EqualTo(a.Replace(@"}}", " |living=yes}}")), "Add value to empty parameter");
            Assert.That(TalkPageFixes.WPBiography(a.Replace(@"}}", "|living=no}}") + @"{{blp}}"), Is.EqualTo(a.Replace(@"}}", "|living=no}}" + @"{{blp}}")), "No change if blp=yes and living=no");
            Assert.That(TalkPageFixes.WPBiography(a + @"{{activepol}}"), Is.EqualTo(a.Replace(@"}}", " | living=yes | activepol=yes | politician-work-group=yes}}")), "Add activepol to WPBiography");
            Assert.That(TalkPageFixes.WPBiography(a + @"{{active politician}}"), Is.EqualTo(a.Replace(@"}}", " | living=yes | activepol=yes | politician-work-group=yes}}")), "Add activepol via redirect to WPBiography");
            Assert.That(TalkPageFixes.WPBiography(a.Replace(@"}}", " |activepol=}}") + @"{{activepol}}"), Is.EqualTo(a.Replace(@"}}", " |activepol= yes| living=yes | politician-work-group=yes}}")), "Add value to empty parameter");
            Assert.That(TalkPageFixes.WPBiography(a.Replace(@"}}", " |activepol=no}}") + @"{{activepol}}"), Is.EqualTo(a.Replace(@"}}", " |activepol=yes | living=yes | politician-work-group=yes}}")), "Change value to activepol no nonsense parameter");
            Assert.That(TalkPageFixes.WPBiography(a + @"{{blp}}{{activepol}}"), Is.EqualTo(a.Replace(@"}}", " | living=yes | activepol=yes | politician-work-group=yes}}")), "Add activepol and blp to WPBiography");
        }

        [Test]
        public void WPSongs()
        {
            string a = @"{{WPSongs}}";

            Assert.That(TalkPageFixes.WPSongs(a + @"{{sir}}"), Is.EqualTo(a.Replace(@"}}", " | needs-infobox=yes}}")), "Add sir to WPSongs");
            Assert.That(TalkPageFixes.WPSongs(a.Replace(@"}}", " | needs-infobox=}}" + @"{{sir}}")), Is.EqualTo(a.Replace(@"}}", " | needs-infobox=yes}}")), "Add sir to WPSongs with empty parameter");
            Assert.That(TalkPageFixes.WPSongs(a.Replace(@"}}", " | needs-infobox=}}" + @"{{Single infobox request}}")), Is.EqualTo(a.Replace(@"}}", " | needs-infobox=yes}}")), "Add Single infobox request to WPSongs with empty parameter");
            Assert.That(TalkPageFixes.WPSongs(a.Replace(@"}}", " | needs-infobox=yes}}") + @"{{sir}}"), Is.EqualTo(a.Replace(@"}}", " | needs-infobox=yes}}")), "Remove sir when WPSongs with needs-infobox=yes exists");
            Assert.That(TalkPageFixes.WPSongs(a), Is.EqualTo(a), "Do nothing");
            Assert.That(TalkPageFixes.WPSongs(a.Replace(@"}}", "|importance=yes}}")), Is.EqualTo(a), "Remove importance");
            Assert.That(TalkPageFixes.WPSongs(a.Replace(@"}}", "|needs-infobox=no}}")), Is.EqualTo(a), "Remove needs-infobox=no");
            Assert.That(TalkPageFixes.WPSongs(a.Replace(@"}}", "|importance=yes|needs-infobox=no}}")), Is.EqualTo(a), "Remove importance and needs-infobox=no");

            string b = @"{{WikiProject Songs}}";

            Assert.That(TalkPageFixes.WPSongs(b + @"{{sir}}"), Is.EqualTo(b.Replace(@"}}", " | needs-infobox=yes}}")), "Add sir to WPSongs");
            Assert.That(TalkPageFixes.WPSongs(b.Replace(@"}}", " | needs-infobox=}}" + @"{{sir}}")), Is.EqualTo(b.Replace(@"}}", " | needs-infobox=yes}}")), "Add sir to WPSongs with empty parameter");
            Assert.That(TalkPageFixes.WPSongs(b.Replace(@"}}", " | needs-infobox=}}" + @"{{Single infobox request}}")), Is.EqualTo(b.Replace(@"}}", " | needs-infobox=yes}}")), "Add Single infobox request to WPSongs with empty parameter");
            Assert.That(TalkPageFixes.WPSongs(b.Replace(@"}}", " | needs-infobox=yes}}") + @"{{sir}}"), Is.EqualTo(b.Replace(@"}}", " | needs-infobox=yes}}")), "Remove sir when WPSongs with needs-infobox=yes exists");
            Assert.That(TalkPageFixes.WPSongs(b), Is.EqualTo(b), "Do nothing");
        }

        [Test]
        public void WPJazz()
        {
            string a = @"{{WPJazz}}";

            Assert.That(TalkPageFixes.WPJazz(a + @"{{WPSongs}}"), Is.EqualTo(a.Replace(@"}}", " | song=yes}}{{WPSongs}}")), "Add song to WPJazz");
            Assert.That(TalkPageFixes.WPJazz(a + @"{{WPAlbums}}"), Is.EqualTo(a.Replace(@"}}", " | album=yes}}{{WPAlbums}}")), "Add album to WPJazz");
            Assert.That(TalkPageFixes.WPJazz(a.Replace(@"}}", " | song=}}" + @"{{WikiProject Songs}}")), Is.EqualTo(a.Replace(@"}}", " | song=yes}}{{WikiProject Songs}}")), "Add song to WPJazz with empty parameter");
            Assert.That(TalkPageFixes.WPJazz(a.Replace(@"}}", " | album=}}" + @"{{WikiProject Albums}}")), Is.EqualTo(a.Replace(@"}}", " | album=yes}}{{WikiProject Albums}}")), "Add album to WPJazz with empty parameter");
            Assert.That(TalkPageFixes.WPJazz(a), Is.EqualTo(a), "Do nothing");
            Assert.That(TalkPageFixes.WPJazz(a.Replace(@"}}", "|needs-infobox=no}}")), Is.EqualTo(a), "Remove needs-infobox=no");

            string b = @"{{WikiProject Jazz}}";

            Assert.That(TalkPageFixes.WPJazz(b + @"{{WPSongs}}"), Is.EqualTo(b.Replace(@"}}", " | song=yes}}{{WPSongs}}")), "Add song to WikiProject Jazz");
            Assert.That(TalkPageFixes.WPJazz(b + @"{{WPAlbums}}"), Is.EqualTo(b.Replace(@"}}", " | album=yes}}{{WPAlbums}}")), "Add album to WikiProject Jazz");
            Assert.That(TalkPageFixes.WPJazz(b.Replace(@"}}", " | song=}}" + @"{{WikiProject Songs}}")), Is.EqualTo(b.Replace(@"}}", " | song=yes}}{{WikiProject Songs}}")), "Add song to WPJAzz with empty parameter");
            Assert.That(TalkPageFixes.WPJazz(b), Is.EqualTo(b), "Do nothing");
            Assert.That(TalkPageFixes.WPJazz(b.Replace(@"}}", "|needs-infobox=no}}")), Is.EqualTo(b), "Remove needs-infobox=no");
        }
    }
    
    [TestFixture]
    public class InTemplateRuleTests
    {
        [Test]
        public void TemplateUsedInText()
        {
            // casing
            ClassicAssert.IsTrue(InTemplateRule.TemplateUsedInText("Bert", @"Bert}} was great"));
            ClassicAssert.IsTrue(InTemplateRule.TemplateUsedInText("Bert", @"bert}} was great"));
            ClassicAssert.IsTrue(InTemplateRule.TemplateUsedInText("bert", @"Bert}} was great"));
            ClassicAssert.IsTrue(InTemplateRule.TemplateUsedInText("bert", @"bert}} was great"));
            
            // spacing
            ClassicAssert.IsTrue(InTemplateRule.TemplateUsedInText("Bert", @"Bert }} was great"));
            ClassicAssert.IsTrue(InTemplateRule.TemplateUsedInText("Bert", @" Bert}} was great"));
            ClassicAssert.IsTrue(InTemplateRule.TemplateUsedInText("Bert", @"Bert|fo=yes}} was great"));
            ClassicAssert.IsTrue(InTemplateRule.TemplateUsedInText("Bert", @"Bert
|fo=yes}} was great"));
            ClassicAssert.IsTrue(InTemplateRule.TemplateUsedInText("Bert", @"Bert |fo=yes}} was great"));
            ClassicAssert.IsTrue(InTemplateRule.TemplateUsedInText("Bert", @" Bert|fo=yes}} was great"));
            ClassicAssert.IsTrue(InTemplateRule.TemplateUsedInText("Bert", @"
    Bert|fo=yes}} was great"));
            
            // comments
            ClassicAssert.IsTrue(InTemplateRule.TemplateUsedInText("Bert", @"Bert|fo=yes}} was <!--great-->"));
            ClassicAssert.IsTrue(InTemplateRule.TemplateUsedInText("Bert", @"<!--thing--> Bert }} was great"));
            ClassicAssert.IsTrue(InTemplateRule.TemplateUsedInText("Bert", @"Bert<!--thing-->}} was great"));
            ClassicAssert.IsTrue(InTemplateRule.TemplateUsedInText("Bert", @"Bert<!--thing-->|foo=bar}} was great"));
            
            ClassicAssert.IsTrue(InTemplateRule.TemplateUsedInText("", @"Bert}} was great"));
            
            // underscores
            ClassicAssert.IsTrue(InTemplateRule.TemplateUsedInText("Bert Li", @"Bert Li}} was great"));
            ClassicAssert.IsTrue(InTemplateRule.TemplateUsedInText("Bert Li", @"Bert   Li}} was great"));
            ClassicAssert.IsTrue(InTemplateRule.TemplateUsedInText("Bert Li", @"Bert_Li}} was great"));
        }
        
        [Test]
        public void TemplateUsedInTextNoMatches()
        {
            ClassicAssert.IsFalse(InTemplateRule.TemplateUsedInText("Bert", @"{{Bert}} was great"));
            ClassicAssert.IsFalse(InTemplateRule.TemplateUsedInText("Bert", @"Bert was great"));
            ClassicAssert.IsFalse(InTemplateRule.TemplateUsedInText("Bert", @"[[Bert]] was great"));
            ClassicAssert.IsFalse(InTemplateRule.TemplateUsedInText("Bert", @"Tim}} was great"));
            ClassicAssert.IsFalse(InTemplateRule.TemplateUsedInText("Bert", @"BERT}} was great"));
            ClassicAssert.IsFalse(InTemplateRule.TemplateUsedInText("Bert Li", @"BertLi}} was great"));
            
            ClassicAssert.IsFalse(InTemplateRule.TemplateUsedInText("Bert", @"<!--Bert}}--> was great"));
        }
    }
    
    [TestFixture]
    public class ListMakerTests : RequiresInitialization
    {
        [Test]
        public void NormalizeTitle()
        {
            ListMaker LMaker = new ListMaker();
            Assert.That(LMaker.NormalizeTitle(@"https://en.wikipedia.org/w/index.php?title=Foo&diff=3&oldid=4"), Is.EqualTo("Foo"));
            Assert.That(LMaker.NormalizeTitle(@"https://en.wikipedia.org/w/index.php?title=Foo&diff=3&curid=4"), Is.EqualTo("Foo"));
            Assert.That(LMaker.NormalizeTitle(@"https://en.wikipedia.org/w/index.php?title=Health_effects_of_chocolate&diff=4018&oldid=40182"), Is.EqualTo("Health effects of chocolate"));
            Assert.That(LMaker.NormalizeTitle(@"https://en.wikipedia.org/w/index.php?title=Foo&action=history"), Is.EqualTo("Foo"));
            Assert.That(LMaker.NormalizeTitle(@"https://en.wikipedia.org/w/index.php?title=Foo&action=edit"), Is.EqualTo("Foo"));
            Assert.That(LMaker.NormalizeTitle(@"https://en.wikipedia.org/w/index.php?title=Foo&oldid=5"), Is.EqualTo("Foo"));
            Assert.That(LMaker.NormalizeTitle(@"https://en.wikipedia.org/w/index.php?title=Science%20%28journal%29&action=history"), Is.EqualTo("Science (journal)"));
            Assert.That(LMaker.NormalizeTitle(@"https://en.wikipedia.org/w/index.php?title=Wikipedia:AutoWikiBrowser/Sandbox&action=edit"), Is.EqualTo(@"Wikipedia:AutoWikiBrowser/Sandbox"));
            Assert.That(LMaker.NormalizeTitle(@"en.wikipedia.org/w/index.php?title=Wikipedia:AutoWikiBrowser/Sandbox&action=edit"), Is.EqualTo(@"Wikipedia:AutoWikiBrowser/Sandbox"));
            Assert.That(LMaker.NormalizeTitle(@"https://en.wikipedia.org/w/index.php?title=Foo#References"), Is.EqualTo("Foo"));

            Assert.That(LMaker.NormalizeTitle(@"   Foo"), Is.EqualTo("Foo"), "cleans spacing from Firefox Category paste");

            Assert.That(LMaker.NormalizeTitle(@"https://en.wikipedia.org/w/index.php?title=Foo&pe=1&#Date_on"), Is.EqualTo(@"Foo"));
            Assert.That(LMaker.NormalizeTitle(@"Foo‎"), Is.EqualTo(@"Foo"), "title has left-to-right mark at the end");

            Assert.That(LMaker.NormalizeTitle(@"https://en.wikipedia.org/w/index.php?title=Foo&offset=20130214190000&action=history"), Is.EqualTo("Foo"), "cleans spacing from Firefox Category paste");
            Assert.That(LMaker.NormalizeTitle(@"https://en.wikipedia.org/w/?title=Foo"), Is.EqualTo("Foo"));
            Assert.That(LMaker.NormalizeTitle(@"https://en.wikipedia.org/wiki/Foo"), Is.EqualTo("Foo"));
            Assert.That(LMaker.NormalizeTitle(@"http://en.wikipedia.org/wiki/Foo"), Is.EqualTo("Foo"), "HTTP not HTTPS support");
            Assert.That(LMaker.NormalizeTitle(@"//en.wikipedia.org/w/index.php?title=Foo&action=history"), Is.EqualTo("Foo"), "Protocol-relative support");
            Assert.That(LMaker.NormalizeTitle(@"https://en.wikipedia.org/w/index.php?title=Foo&redirect=no"), Is.EqualTo("Foo"));

            Assert.That(LMaker.NormalizeTitle(@"https://en.wikipedia.org/w/index.php?title=Foo&type=revision&diff=75558108&oldid=65316243"), Is.EqualTo("Foo"));

            Assert.That(LMaker.NormalizeTitle(@"#[[Foo]]"), Is.EqualTo("#[[Foo]]"), "#wikilinked");
            Assert.That(LMaker.NormalizeTitle(@"Foo#bar"), Is.EqualTo("Foo"), "#wikilinked");
        }

        [Test]
        public void NormalizeTitleSecure()
        {
            ListMaker LMaker = new ListMaker();
            Assert.That(LMaker.NormalizeTitle(@"https://en.wikipedia.org/w/index.php?title=Foo&diff=3&oldid=4"), Is.EqualTo("Foo"));
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

            Assert.That(LMakerRLD.Count, Is.EqualTo(3), "Duplicate removed");
            foreach (Article a in LMakerRLD)
            {
                Assert.That(a.ToString(), Is.EqualTo("A"), "Duplicates removed from end of list");
                break;
            }
        }
        
        [Test]
        public void RemoveListDuplicates10K()
        {
            const int big = 10000;
            ListMaker LMakerLarge = new ListMaker();
            LMakerLarge.Clear();
            for (int i = 1; i < big; i++)
                LMakerLarge.Add(new Article(i.ToString()));
            
            LMakerLarge.Add(new Article("1"));

            Assert.That(LMakerLarge.Count, Is.EqualTo(big));
            
            LMakerLarge.RemoveListDuplicates();

            Assert.That(LMakerLarge.Count, Is.EqualTo(big - 1), "Duplicate removed");
            ClassicAssert.IsTrue(LMakerLarge.Contains(new Article("1")), "First instance of article retained");
        }

        [Test]
        public void FilterNonMainArticlesVolume()
        {
            const int big = 500;
            ListMaker LMakerLarge = new ListMaker();
            LMakerLarge.Clear();
            for (int i = 1; i < big; i++)
                LMakerLarge.Add(i.ToString());

            LMakerLarge.Add("Talk:Me");

            LMakerLarge.FilterNonMainArticles();
            Assert.That(LMakerLarge.Count, Is.EqualTo(big - 1), "Non-mainspace article removed");
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
            Assert.That(LMaker.NumberOfArticles, Is.EqualTo(4));
        }

        [Test]
        public void AddList()
        {
            ListMaker LMaker = new ListMaker();
            List<Article> l = new List<Article>();

            l.Add(new Article("A"));
            l.Add(new Article("B"));

            LMaker.Add("A");
            LMaker.Add("B");

            LMaker.FilterDuplicates = true;

            LMaker.Add(l);
            Assert.That(LMaker.NumberOfArticles, Is.EqualTo(2));

            l.Add(new Article("C"));
            LMaker.Add(l);
            Assert.That(LMaker.NumberOfArticles, Is.EqualTo(3));

            l.Add(new Article("C"));
            l.Add(new Article("C"));
            l.Add(new Article("C"));
            l.Add(new Article("C"));
            LMaker.Add("D");
            LMaker.Add(l);
            Assert.That(LMaker.NumberOfArticles, Is.EqualTo(4));
        }

        [Test]
        public void GetArticleList()
        {
            ListMaker LMaker = new ListMaker();
            LMaker.Add("A");
            LMaker.Add("B");

            Assert.That(LMaker.GetArticleList().Count, Is.EqualTo(2));

            LMaker.Items.SetSelected(0, true);
            LMaker.Items.SetSelected(1, true);

            Assert.That(LMaker.GetSelectedArticleList().Count, Is.EqualTo(2));

            LMaker.Items.SetSelected(1, false);

            Assert.That(LMaker.GetSelectedArticleList().Count, Is.EqualTo(1));
            Assert.That(LMaker.GetArticleList().Count, Is.EqualTo(2));
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
            ClassicAssert.IsTrue(lb1.Items.Contains("B"));
            ClassicAssert.IsTrue(lb1.Items.Contains("C"));
            ClassicAssert.IsFalse(lb1.Items.Contains("A"));
            Assert.That(lb1.Items.Count, Is.EqualTo(2));

            // unique in 2
            ClassicAssert.IsFalse(lb2.Items.Contains("A"));
            ClassicAssert.IsTrue(lb2.Items.Contains("D"));
            ClassicAssert.IsTrue(lb2.Items.Contains("E"));
            Assert.That(lb2.Items.Count, Is.EqualTo(2));

            // common to both
            ClassicAssert.IsTrue(lb3.Items.Contains("A"));
            Assert.That(lb3.Items.Count, Is.EqualTo(1));
        }
        
        [Test]
        public void ListComparer10K()
        {
            const int big = 10000;
            ListMaker LMakerC10K = new ListMaker();

            for (int i = 0; i < big; i++)
                LMakerC10K.Add(new Article(i.ToString()));

            LMakerC10K.Add(new Article("A"));
            LMakerC10K.Add(new Article("B"));

            System.Windows.Forms.ListBox lb1 = new System.Windows.Forms.ListBox();
            System.Windows.Forms.ListBox lb2 = new System.Windows.Forms.ListBox();
            System.Windows.Forms.ListBox lb3 = new System.Windows.Forms.ListBox();
            
            List<Article> articlesC = new List<Article>();
            for (int i = 0; i < big; i++)
                articlesC.Add(new Article(i.ToString()));
            
            articlesC.Add(new Article("C"));
            articlesC.Add(new Article("D"));
            
            ListComparer.CompareLists(LMakerC10K, articlesC, lb1, lb2, lb3);
            
            // unique in 1
            ClassicAssert.IsTrue(lb1.Items.Contains("A"));
            ClassicAssert.IsTrue(lb1.Items.Contains("B"));
            
            // unique in 2
            ClassicAssert.IsTrue(lb2.Items.Contains("C"));
            ClassicAssert.IsTrue(lb2.Items.Contains("D"));
            
            // common to both
            ClassicAssert.IsTrue(lb3.Items.Contains("1"));
            Assert.That(lb3.Items.Count, Is.EqualTo(big));
        }
    }
    
    [TestFixture]
    public class ListBoxArticleTests : RequiresInitialization
    {
        [Test]
        public void RemoveSelected()
        {
            ListBoxArticle lbArticles = new ListBoxArticle();

            const int big = 70000, sel = 5000;

            // sequential block deletion performance
            for (int i = 0; i < big; i++)
                lbArticles.Items.Add(new Article(i.ToString()));
            
            for (int j = sel; j > 0; j--)
                lbArticles.SetSelected(big-j, true);
            
            lbArticles.RemoveSelected(true);

            if (Globals.UsingMono) // Mono implementation of SetSelected does not seem to honour second input to multi-select
                Assert.That(lbArticles.Items.Count, Is.EqualTo(big - 1));
            else
                Assert.That(lbArticles.Items.Count, Is.EqualTo(big - sel));

            // single selected item deletion performance
            lbArticles.Items.Clear();
            for (int i = 0; i < big*3; i++)
                lbArticles.Items.Add(new Article(i.ToString()));
            lbArticles.SetSelected((int)big/2, true);
            lbArticles.RemoveSelected(true);
            Assert.That(lbArticles.Items.Count, Is.EqualTo(big * 3 - 1));

            // non-sequential deletion
            lbArticles.Items.Clear();
            for (int i = 0; i < 10; i++)
                lbArticles.Items.Add(new Article(i.ToString()));
            lbArticles.SetSelected(1, true);
            lbArticles.SetSelected(3, true);
            lbArticles.RemoveSelected(true);
            if (Globals.UsingMono)
            {
                lbArticles.SetSelected(1, true);
                lbArticles.RemoveSelected(true);
            }
            ClassicAssert.IsFalse(lbArticles.Items.Contains(new Article("1")));
            ClassicAssert.IsFalse(lbArticles.Items.Contains(new Article("3")));
            ClassicAssert.IsTrue(lbArticles.Items.Contains(new Article("2")));

            lbArticles.Items.Clear();
            for (int i = 0; i < 10; i++)
                lbArticles.Items.Add(new Article(i.ToString()));
            lbArticles.SetSelected(1, true);
            lbArticles.SetSelected(5, true);
            lbArticles.RemoveSelected(true);
            if (Globals.UsingMono)
            {
                lbArticles.SetSelected(1, true);
                lbArticles.RemoveSelected(true);
            }

            ClassicAssert.IsFalse(lbArticles.Items.Contains(new Article("1")));
            ClassicAssert.IsFalse(lbArticles.Items.Contains(new Article("5")));
            ClassicAssert.IsTrue(lbArticles.Items.Contains(new Article("2")));

            lbArticles.Items.Clear();
            lbArticles.Items.Add(new Article("A"));
            lbArticles.Items.Add(new Article("A"));
            lbArticles.Items.Add(new Article("B"));
            lbArticles.SetSelected(2, true);
            lbArticles.RemoveSelected(false);
            Assert.That(lbArticles.Items.Count, Is.EqualTo(2), "Duplicates not removed if not in duplicate mode");

            lbArticles.Items.Clear();
            lbArticles.Items.Add(new Article("A"));
            lbArticles.Items.Add(new Article("B"));
            lbArticles.Items.Add(new Article("C"));
            lbArticles.SetSelected(0, true);
            lbArticles.SetSelected(1, true);
            lbArticles.SetSelected(2, true);
            lbArticles.RemoveSelected(false);

            // under mono list count is still 2 now, appears it implements RemoveSelected as for one item only not all selected
            // so set and remove to get down to count of zero under Mono too
            if(Globals.UsingMono)
            {
                lbArticles.SetSelected(0, true);
                lbArticles.RemoveSelected(false);
                lbArticles.SetSelected(0, true);
                lbArticles.RemoveSelected(false);
            }
            Assert.That(lbArticles.Items.Count, Is.EqualTo(0), "List cleared if all items selected and removed");
        }
    }
    
    [TestFixture]
    public class FindAndReplaceTests
    {
        [Test]
        public void FindAndReplace()
        {
            Replacement r = new Replacement("foo", "bar", true, true, true, true, RegexOptions.None, "");
            
            FindandReplace fr = new FindandReplace();
            
            bool changemade = false;

            Assert.That(fr.PerformFindAndReplace(r, "the was", "Test", out changemade), Is.EqualTo("the was"));
            ClassicAssert.IsFalse(changemade);
            Assert.That(fr.ReplacedSummary, Is.EqualTo(null), "No match: no edit summary");

            Assert.That(fr.PerformFindAndReplace(r, "the foo was", "Test", out changemade), Is.EqualTo("the bar was"));
            ClassicAssert.IsTrue(changemade);
            Assert.That(fr.ReplacedSummary, Is.EqualTo("foo" + FindandReplace.Arrow + "bar"), "One match: a to b");
            
            fr = new FindandReplace();
            changemade = false;

            Assert.That(fr.PerformFindAndReplace(r, "the foo was or foo was", "Test", out changemade), Is.EqualTo("the bar was or bar was"));
            Assert.That(fr.ReplacedSummary, Is.EqualTo("foo" + FindandReplace.Arrow + "bar (2)"), "Match count shown");
            
            fr = new FindandReplace();
            changemade = false;

            Assert.That(fr.PerformFindAndReplace(r, "the foo was or foo was foo", "Test", out changemade), Is.EqualTo("the bar was or bar was bar"));
            Assert.That(fr.ReplacedSummary, Is.EqualTo("foo" + FindandReplace.Arrow + "bar (3)"), "Match count shown, 3");
            
            r = new Replacement("foot?", "bar", true, true, true, true, RegexOptions.None, "");
            fr = new FindandReplace();
            changemade = false;

            Assert.That(fr.PerformFindAndReplace(r, "the foot was or foo was", "Test", out changemade), Is.EqualTo("the bar was or bar was"));
            ClassicAssert.IsTrue(changemade);
            Assert.That(fr.ReplacedSummary, Is.EqualTo("foot" + FindandReplace.Arrow + "bar (2)"), "Different matches, match text of first used");
            
            r = new Replacement("fooo?", "foo", true, true, true, true, RegexOptions.None, "");
            fr = new FindandReplace();
            changemade = false;
            Assert.That(fr.PerformFindAndReplace(r, "the foo was a fooo it", "Test", out changemade), Is.EqualTo("the foo was a foo it"));
            ClassicAssert.IsTrue(changemade);
            Assert.That(fr.ReplacedSummary, Is.EqualTo("fooo" + FindandReplace.Arrow + "foo"), "No-change match ignored");
            
            fr = new FindandReplace();
            changemade = false;
            Assert.That(fr.PerformFindAndReplace(r, "the foo was", "Test", out changemade), Is.EqualTo("the foo was"));
            ClassicAssert.IsFalse(changemade, "only match is no-change on replace, so no change made");
            Assert.That(fr.ReplacedSummary, Is.EqualTo(null), "Only match is No-change match, no edit summary");
        }

        [Test]
        public void FindAndReplaceNewLines()
        {
            // regex
            Replacement r = new Replacement("foo\n", "bar ", true, true, true, true, RegexOptions.None, "");
            FindandReplace fr = new FindandReplace();
            bool changemade = false;

            Assert.That(fr.PerformFindAndReplace(r, "the foo\nwas", "Test", out changemade), Is.EqualTo("the bar was"));
            ClassicAssert.IsTrue(changemade);
            
            // not regex
            r = new Replacement("foo\n", "bar ", false, true, true, true, RegexOptions.None, "");
            Assert.That(fr.PerformFindAndReplace(r, "the foo\nwas", "Test", out changemade), Is.EqualTo("the bar was"));
            ClassicAssert.IsTrue(changemade);
        }
        
        [Test]
        public void FindAndReplaceRemove()
        {
            Replacement r = new Replacement("foo", "", true, true, true, true, RegexOptions.None, "");
            
            FindandReplace fr = new FindandReplace();
            
            bool changemade = false;

            Assert.That(fr.PerformFindAndReplace(r, "the was", "Test", out changemade), Is.EqualTo("the was"));
            ClassicAssert.IsFalse(changemade);
            Assert.That(fr.ReplacedSummary, Is.EqualTo(null), "No match: no edit summary");
            Assert.That(fr.RemovedSummary, Is.EqualTo(null), "No match: no edit summary");

            Assert.That(fr.PerformFindAndReplace(r, "the foo was", "Test", out changemade), Is.EqualTo("the  was"));
            ClassicAssert.IsTrue(changemade);
            Assert.That(fr.RemovedSummary, Is.EqualTo("foo"), "One match: removed a");
            
            fr = new FindandReplace();
            changemade = false;

            Assert.That(fr.PerformFindAndReplace(r, "the foo was or foo was", "Test", out changemade), Is.EqualTo("the  was or  was"));
            Assert.That(fr.RemovedSummary, Is.EqualTo("foo (2)"), "Match count shown");

            r = new Replacement("foot?", "", true, true, true, true, RegexOptions.None, "");
            fr = new FindandReplace();
            changemade = false;

            Assert.That(fr.PerformFindAndReplace(r, "the foot was or foo was", "Test", out changemade), Is.EqualTo("the  was or  was"));
            ClassicAssert.IsTrue(changemade);
            Assert.That(fr.RemovedSummary, Is.EqualTo("foot (2)"), "Different matches, match text of first used");
        }
        
        [Test]
        public void FindAndReplaceProperties()
        {
            Replacement r = new Replacement("foo", "bar", true, true, true, true, RegexOptions.None, "");
            Replacement r2 = new Replacement("foo2", "bar2", true, true, true, false, RegexOptions.None, "");
            Replacement r2Disabled = new Replacement("foo2", "bar2", true, false, true, false, RegexOptions.None, "");
            FindandReplace fr = new FindandReplace();

            fr.AddNew(r);
            ClassicAssert.IsTrue(fr.HasAfterProcessingReplacements);
            ClassicAssert.IsTrue(fr.HasProcessingReplacements(true));
            ClassicAssert.IsFalse(fr.HasProcessingReplacements(false));

            fr.AddNew(r2);
            ClassicAssert.IsTrue(fr.HasAfterProcessingReplacements);
            ClassicAssert.IsTrue(fr.HasProcessingReplacements(true));
            ClassicAssert.IsTrue(fr.HasProcessingReplacements(false));

            fr.Clear();
            fr.AddNew(r2);
            ClassicAssert.IsFalse(fr.HasAfterProcessingReplacements);
            ClassicAssert.IsFalse(fr.HasProcessingReplacements(true));
            ClassicAssert.IsTrue(fr.HasProcessingReplacements(false));

            // all false when no enabled rules
            fr.Clear();
            fr.AddNew(r2Disabled);
            ClassicAssert.IsFalse(fr.HasAfterProcessingReplacements);
            ClassicAssert.IsFalse(fr.HasProcessingReplacements(true));
            ClassicAssert.IsFalse(fr.HasProcessingReplacements(false));

            fr.Clear();
            List<Replacement> l = new List<Replacement>();
            l.Add(r);
            l.Add(r2);
            fr.AddNew(l);
            ClassicAssert.IsTrue(fr.HasReplacements);
            Assert.That(l, Is.EqualTo(fr.GetList()));
        }
    }
    
    [TestFixture]
    public class SubstTemplates
    {
        [Test]
        public void SubstTemplatesNoExpand()
        {
            WikiFunctions.SubstTemplates st = new WikiFunctions.SubstTemplates();
            st.ExpandRecursively = false;

            Assert.That(st.SubstituteTemplates("Now {{foo}}", "test"), Is.EqualTo("Now {{foo}}"), "no change when no templates in list");

            st.TemplateList = new[] {"foo", "bar"};

            Assert.That(st.NoOfRegexes, Is.EqualTo(2));

            Assert.That(st.SubstituteTemplates("Now {{foo}}", "test"), Is.EqualTo("Now {{subst:foo}}"), "simple case");
            Assert.That(st.SubstituteTemplates("Now {{Foo}}", "test"), Is.EqualTo("Now {{subst:foo}}"), "First letter casing handling");
            Assert.That(st.SubstituteTemplates("Now {{ foo}}", "test"), Is.EqualTo("Now {{subst:foo}}"), "whitespace before");
            Assert.That(st.SubstituteTemplates("Now {{ foo }}", "test"), Is.EqualTo("Now {{subst:foo}}"), "whitespace after");
            Assert.That(st.SubstituteTemplates("Now {{foo|first=y}}", "test"), Is.EqualTo("Now {{subst:foo|first=y}}"), "template with parameters");
            Assert.That(st.SubstituteTemplates("Now {{foo|first}}", "test"), Is.EqualTo("Now {{subst:foo|first}}"), "template with arguments");
            Assert.That(st.SubstituteTemplates("Now {{foo}} {{bar}}", "test"), Is.EqualTo("Now {{subst:foo}} {{subst:bar}}"), "Multiple");

            Assert.That(st.SubstituteTemplates("Now {{Template:foo}}", "test"), Is.EqualTo("Now {{subst:foo}}"), "Template prefix");
            Assert.That(st.SubstituteTemplates("Now {{template:foo}}", "test"), Is.EqualTo("Now {{subst:foo}}"), "template prefix");
            Assert.That(st.SubstituteTemplates("Now {{template :foo}}", "test"), Is.EqualTo("Now {{subst:foo}}"), "template spacing after");
            Assert.That(st.SubstituteTemplates("Now {{ template :foo}}", "test"), Is.EqualTo("Now {{subst:foo}}"), "template spacing before");
            Assert.That(st.SubstituteTemplates("Now {{ template : foo}}", "test"), Is.EqualTo("Now {{subst:foo}}"), "all whitespace");
            Assert.That(st.SubstituteTemplates("Now {{Msg:foo}}", "test"), Is.EqualTo("Now {{subst:foo}}"), "Msg prefix");
            Assert.That(st.SubstituteTemplates("Now {{msg :foo}}", "test"), Is.EqualTo("Now {{subst:foo}}"), "Msg prefix spacing");

            st.TemplateList = new[] {"foo (bar)"};
            Assert.That(st.NoOfRegexes, Is.EqualTo(1));

            Assert.That(st.SubstituteTemplates("Now {{foo (bar)}}", "test"), Is.EqualTo("Now {{subst:foo (bar)}}"), "template name is escaped");
            Assert.That(st.SubstituteTemplates("Now {{Foo (bar)}}", "test"), Is.EqualTo("Now {{subst:foo (bar)}}"), "template name is escaped, casing handling");
        }
    }
}
