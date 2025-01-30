using System.Text.RegularExpressions;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using WikiFunctions;

namespace UnitTests
{
    /// <summary>
    /// Unit tests for dynamic wikiRegexes – those regexes that depend on language-specific variables retrieved from the wiki
    /// </summary>
    [TestFixture]
    public class DynamicRegexTests : RequiresInitialization
    {
        [Test]
        public void CategoryTests()
        {
            RegexAssert.IsMatch(WikiRegexes.Category, "[[Category:Test]]", "Simple case");
            RegexAssert.IsMatch(WikiRegexes.CategoryQuick, "[[Category:Test]]");
            RegexAssert.IsMatch(WikiRegexes.Category, "[[Category:Test|{{PAGENAME}}]]", " use of {{PAGENAME}}");
            RegexAssert.IsMatch(WikiRegexes.Category, "[[Category:Test|{{subst:PAGENAME}}]]", " use of {{subst:PAGENAME}}");
            RegexAssert.IsMatch(WikiRegexes.Category, "[[Category:Test now]]", "Space in name");
            RegexAssert.IsMatch(WikiRegexes.Category, "[[Category:Test|Key]]", "Simple sortkey");
            RegexAssert.IsMatch(WikiRegexes.Category, "[[ Category : Test now| Key]]", "Spaced sortkey");
            RegexAssert.IsMatch(WikiRegexes.Category, "[[CATEGORY :Test]]", "Uppercase");
            RegexAssert.IsMatch(WikiRegexes.Category, "[[Category:_Test]]", "Leading underscore in name");
            RegexAssert.IsMatch(WikiRegexes.Category, "[[_Category:Test]]", "leading underscore");
            RegexAssert.IsMatch(WikiRegexes.Category, "[[_Category:Test_]]", "Trailing underscores");
            RegexAssert.IsMatch(WikiRegexes.CategoryQuick, "[[_Category:Test_]]");

            RegexAssert.NoMatch(WikiRegexes.Category, "[[Test]]", "Wikilink");
            RegexAssert.NoMatch(WikiRegexes.Category, "[[Image:Test.jpg]]", "Image link");
            RegexAssert.NoMatch(WikiRegexes.Category, @"[[Category:
1910 births]]", "Newline in category name");
            RegexAssert.NoMatch(WikiRegexes.Category, @"[[Category:1910 births
]]", "newline in category name 2");
            Assert.That(WikiRegexes.Category.Match("[[Category:Test now]]").Groups[1].Value, Is.EqualTo("Test now"), "Group 1 is category name");
            Assert.That(WikiRegexes.Category.Match("[[Category: Test now ]]").Groups[1].Value, Is.EqualTo("Test now"), "Group 1 is category name, trimmed");
            Assert.That(WikiRegexes.Category.Match("[[Category: Test now ]]").Groups[2].Value, Is.Empty, "Group 2 is optional sort key");
            Assert.That(WikiRegexes.Category.Match("[[Category: Test now|Bar]]").Groups[2].Value, Is.EqualTo("Bar"), "Group 2 is sort key");

            #if DEBUG
            Variables.SetProjectLangCode("sv");
            Variables.NamespacesCaseInsensitive.Remove(Namespace.Category);
            Variables.NamespacesCaseInsensitive.Add(Namespace.Category, "[Kk]ategori:");
            WikiRegexes.MakeLangSpecificRegexes();
            RegexAssert.IsMatch(WikiRegexes.Category, "[[Kategori:Test]]");
            RegexAssert.IsMatch(WikiRegexes.CategoryQuick, "[[Kategori:Test]]");

            Variables.SetProjectLangCode("el");
            Variables.NamespacesCaseInsensitive.Remove(Namespace.Category);
            Variables.NamespacesCaseInsensitive.Add(Namespace.Category, "[Κκ]ατηγορία:");
            WikiRegexes.MakeLangSpecificRegexes();
            RegexAssert.IsMatch(WikiRegexes.Category, "[[Κατηγορία:Δοκιμή]]");
            RegexAssert.IsMatch(WikiRegexes.CategoryQuick, "[[Κατηγορία:Δοκιμή]]");


            Variables.SetProjectLangCode("en");
            Variables.NamespacesCaseInsensitive.Remove(Namespace.Category);
            Variables.NamespacesCaseInsensitive.Add(Namespace.Category, @"(?i:Category)\s*:");
            WikiRegexes.MakeLangSpecificRegexes();
            RegexAssert.IsMatch(WikiRegexes.Category, "[[Category:Test]]");
            RegexAssert.IsMatch(WikiRegexes.CategoryQuick, "[[Category:Test]]");
            #endif
        }
        
        [Test]
        public void LooseCategoryTests()
        {
            RegexAssert.IsMatch(WikiRegexes.LooseCategory, "[[Category :Test]]");
            RegexAssert.IsMatch(WikiRegexes.LooseCategory, "[[ category :Test]]");
            RegexAssert.IsMatch(WikiRegexes.LooseCategory, "[[CATEGORY :Test]]");
            RegexAssert.IsMatch(WikiRegexes.LooseCategory, "[[ Category: Test]]");
            RegexAssert.IsMatch(WikiRegexes.LooseCategory, "[[_Category: Test]]");
            RegexAssert.IsMatch(WikiRegexes.LooseCategory, "[[ Category :Test|here]]");
            RegexAssert.IsMatch(WikiRegexes.LooseCategory, @"[[Category:
1910 births]]");
            RegexAssert.IsMatch(WikiRegexes.LooseCategory, "[[Category::Test]]");

            RegexAssert.NoMatch(WikiRegexes.LooseCategory, "[[Test]]");
            RegexAssert.NoMatch(WikiRegexes.LooseCategory, "[[Category");
            RegexAssert.NoMatch(WikiRegexes.LooseCategory, "[[Image:Test.jpg]]");
        }

        [Test]
        public void ImageTestsStandard()
        {
            RegexAssert.IsMatch(WikiRegexes.Images, "[[File:Test.JPG]]");
            RegexAssert.IsMatch(WikiRegexes.Images, "[[File:Test.jpg]]");
            RegexAssert.IsMatch(WikiRegexes.Images, "[[File:Test of the.ogg]]");
            RegexAssert.Matches(WikiRegexes.Images, "[[File:Test of the.ogg]]", "[[File:Test of the.ogg]]");
            RegexAssert.IsMatch(WikiRegexes.Images, "[[File:Test_of_the.ogg]]");
            RegexAssert.IsMatch(WikiRegexes.Images, "[[Image:Test.JPG]]");
            RegexAssert.IsMatch(WikiRegexes.Images, "[[File:test.jpg|thumbnail|Some description [[here]], 1987]]");
            RegexAssert.IsMatch(WikiRegexes.Images, "[[Image:Test here.png|right|200px|Some description [[here]] or there]]");
            RegexAssert.IsMatch(WikiRegexes.Images, @"[[Image:Test here.png|right|200px|Some description [[here]] or there
 over lines]]");
            RegexAssert.IsMatch(WikiRegexes.Images, "[[File:Test.JPG");
            
            RegexAssert.NoMatch(WikiRegexes.Images, "[[File Test.JPG]]");

            string bef = @"[[File:Miguel.jpg|thumb|[[Miguel (privateer)|Miguel]]]] [[File:Demetrio.jpg|thumb|[[Demetrio]]]]";

            Assert.That(WikiRegexes.Images.Replace(bef, "_"), Is.EqualTo(@"_thumb|[[Miguel (privateer)|Miguel]]]] _thumb|[[Demetrio]]]]"), "Image on same line handling");
        }
        
        [Test]
        public void ImageTestsInfoboxes()
        {
            RegexAssert.IsMatch(WikiRegexes.Images, @"{{Infobox foo|
bar = a|
image = [[File:Test.JPG]]
| there=here}}");
            RegexAssert.IsMatch(WikiRegexes.Images, @"{{Infobox foo|
  bar = a|
  image2 = [[File:Test.JPG]]
  | there=here}}");
            RegexAssert.IsMatch(WikiRegexes.Images, @"{{Infobox foo|
  bar = a|
  img = [[File:Test.JPG]]
  | there=here}}");
            RegexAssert.IsMatch(WikiRegexes.Images, @"{{Infobox foo|
  bar = a|
  img = Test.JPEG
  | there=here}}");
            RegexAssert.IsMatch(WikiRegexes.Images, @"{{Infobox foo|
  bar = a|
  cover = [[Image:Test.JPG]]
  | there=here}}");
            RegexAssert.IsMatch(WikiRegexes.Images, @"{{Infobox foo|
  bar = a|
  cover art = Test2.png
  | there=here}}");
            RegexAssert.IsMatch(WikiRegexes.Images, @"{{Infobox foo|
  bar = a|
  1 = Test2.png
  | there=here}}", "1-character parameter name");
            RegexAssert.IsMatch(WikiRegexes.Images, @"{{Infobox foo|
  bar = a|
  cover_ar = Test.JPG
  | there=here}}");
            RegexAssert.IsMatch(WikiRegexes.Images, @"{{Infobox foo|
  bar = a|map=[[File:Test.JPG]]
  | there=here}}");
            RegexAssert.IsMatch(WikiRegexes.Images, @"{{Infobox foo|
  bar = a| map = [[File:Test.JPG]]
  | there=here}}");
            
            RegexAssert.IsMatch(WikiRegexes.Images, @"{{Infobox foo|
  bar = a|
  strangename = Test.JPEG
  | there=here}}");
            
            RegexAssert.IsMatch(WikiRegexes.Images, @"{{Infobox foo|
bar = a|
picture = Test.JPG
| there=here}}");
            
            RegexAssert.Matches(WikiRegexes.Images, @"{{Infobox foo|
  bar = a|
  picture = Test.JPG
  | there=here}}", @" Test.JPG");

            RegexAssert.NoMatch(WikiRegexes.Images, @"{{Infobox foo|
  bar = a|
  url = http://apageof.com/a.html
  | there=here}}");

            RegexAssert.IsMatch(WikiRegexes.Images, @"{{double image
|right|
Bob D.jpg|120|Florence.jpg|120|Mmm}}");
        }
        
        [Test]
        public void ImageTestsGalleryTag()
        {
            RegexAssert.Matches(WikiRegexes.Images, @"<gallery>
File:Foo.png
</gallery>", "File:Foo.png");
            RegexAssert.Matches(WikiRegexes.Images, @"<gallery>
File:Foo.png|description
</gallery>", "File:Foo.png|");
            RegexAssert.Matches(WikiRegexes.Images, @"<gallery>
Image:Foo.png|description
</gallery>", "Image:Foo.png|");
            RegexAssert.Matches(WikiRegexes.Images, @"<gallery>
Image : Foo.png |description
</gallery>", "Image : Foo.png |");
            RegexAssert.Matches(WikiRegexes.Images, @"<GALLERY>
Image:Foo.png|description
</GALLERY>", "Image:Foo.png|");
            RegexAssert.Matches(WikiRegexes.Images, @"<gallery name=bar style=silver>
File:Foo.png|description<br>text
</gallery>", "File:Foo.png|");
            RegexAssert.Matches(WikiRegexes.Images, @"< gallery >
File:Foo.png
< /gallery >", "File:Foo.png");
             RegexAssert.Matches(WikiRegexes.Images, @"< gallery >
File:9th of May_street, Bacău.jpg
< /gallery >", "File:9th of May_street, Bacău.jpg");
            
            MatchCollection mc = WikiRegexes.Images.Matches(@"<gallery>
Image:Foo.png|description
Image:Foo2.png|description2
</gallery>");

            Assert.That(mc[0].Value, Is.EqualTo("Image:Foo.png|"));
            Assert.That(mc[1].Value, Is.EqualTo("Image:Foo2.png|"));
            
            RegexAssert.IsMatch(WikiRegexes.Images, @"File:Foo.png|description");
            RegexAssert.NoMatch(WikiRegexes.Images, @"<gallery>

</gallery>
Image here");
        }
        
        [Test]
        public void ImageTestsGalleryTemplate()
        {
            RegexAssert.IsMatch(WikiRegexes.Images, @"{{Gallery
|title=Lamu images
|width=150
|lines=2
|Image:LamuFort.jpg|Lamu Fort
|Image:LAMU Riyadha Mosque.jpg|Riyadha Mosque
|Image:04 Donkey Hospital (June 30 2001).jpg|Donkey Sanctuary
}}");
            RegexAssert.IsMatch(WikiRegexes.Images, @"{{ gallery
|title=Lamu images
|width=150
|lines=2
|Image:LamuFort.jpg|Lamu Fort
|Image:LAMU Riyadha Mosque.jpg|Riyadha Mosque
|Image:04 Donkey Hospital (June 30 2001).jpg|Donkey Sanctuary
}}");
        }
        
        [Test]
        public void ImagesCountOnlyTestsStandard()
        {
            RegexAssert.IsMatch(WikiRegexes.ImagesCountOnly, "[[File:Test.JPG]]");
            RegexAssert.IsMatch(WikiRegexes.ImagesCountOnly, "[[File:Test.jpg]]");
            RegexAssert.IsMatch(WikiRegexes.ImagesCountOnly, "[[File:Test of the.ogg]]");
            RegexAssert.Matches(WikiRegexes.ImagesCountOnly, "[[File:Test of the.ogg]]", "[[File:Test of the.ogg]]");
            RegexAssert.IsMatch(WikiRegexes.ImagesCountOnly, "[[File:Test_of_the.ogg]]");
            RegexAssert.IsMatch(WikiRegexes.ImagesCountOnly, "[[Image:Test.JPG]]");
            RegexAssert.IsMatch(WikiRegexes.ImagesCountOnly, "[[Image:Test here.png|right|200px|Some description [[here]] or there]]");
            RegexAssert.IsMatch(WikiRegexes.ImagesCountOnly, @"[[Image:Test here.png|right|200px|Some description [[here]] or there
 over lines]]");
            RegexAssert.IsMatch(WikiRegexes.ImagesCountOnly, "[[File:Test.JPG");
            
            RegexAssert.NoMatch(WikiRegexes.ImagesCountOnly, "[[File Test.JPG]]");
        }
        
        [Test]
        public void ImagesCountOnlyTestsInfoboxes()
        {
            RegexAssert.IsMatch(WikiRegexes.ImagesCountOnly, @"{{Infobox foo|
bar = a|
image = [[File:Test.JPG]]
| there=here}}");
            RegexAssert.IsMatch(WikiRegexes.ImagesCountOnly, @"{{Infobox foo|
  bar = a|
  image2 = [[File:Test.JPG]]
  | there=here}}");
            RegexAssert.IsMatch(WikiRegexes.ImagesCountOnly, @"{{Infobox foo|
  bar = a|
  img = [[File:Test.JPG]]
  | there=here}}");
            RegexAssert.IsMatch(WikiRegexes.ImagesCountOnly, @"{{Infobox foo|
  bar = a|
  img = Test.JPEG
  | there=here}}");
            RegexAssert.IsMatch(WikiRegexes.ImagesCountOnly, @"{{Infobox foo|
  bar = a|
  cover = [[Image:Test.JPG]]
  | there=here}}");
            RegexAssert.IsMatch(WikiRegexes.ImagesCountOnly, @"{{Infobox foo|
  bar = a|
  cover art = Test2.png
  | there=here}}");
            RegexAssert.IsMatch(WikiRegexes.ImagesCountOnly, @"{{Infobox foo|
  bar = a|
  cover_ar = Test.JPG
  | there=here}}");
            RegexAssert.IsMatch(WikiRegexes.ImagesCountOnly, @"{{Infobox foo|
  bar = a|map=[[File:Test.JPG]]
  | there=here}}");
            RegexAssert.IsMatch(WikiRegexes.ImagesCountOnly, @"{{Infobox foo|
  bar = a| map = [[File:Test.JPG]]
  | there=here}}");
            
            RegexAssert.IsMatch(WikiRegexes.ImagesCountOnly, @"{{Infobox foo|
  bar = a|
  strangename = Test.JPEG
  | there=here}}");
            
            RegexAssert.IsMatch(WikiRegexes.ImagesCountOnly, @"{{Infobox foo|
bar = a|
picture = Test.JPG
| there=here}}");
            
            // here's the difference between Images and ImagesCountOnly
            RegexAssert.Matches(WikiRegexes.ImagesCountOnly, @"{{Infobox foo|
  bar = a|
  picture = Test.JPG
  | there=here}}", @"|
  picture = Test.JPG");

            RegexAssert.NoMatch(WikiRegexes.ImagesCountOnly, @"{{Infobox foo|
  bar = a|
  url = http://apageof.com/a.html
  | there=here}}");
        }
        
        [Test]
        public void ImagesCountOnlyTestsGalleryTag()
        {
            RegexAssert.Matches(WikiRegexes.ImagesCountOnly, @"<gallery>
File:Foo.png
</gallery>", "File:Foo.png");
            RegexAssert.Matches(WikiRegexes.ImagesCountOnly, @"<gallery>
File:Foo.png|description
</gallery>", "File:Foo.png|");
            RegexAssert.Matches(WikiRegexes.ImagesCountOnly, @"<gallery>
Image:Foo.png|description
</gallery>", "Image:Foo.png|");
            RegexAssert.Matches(WikiRegexes.ImagesCountOnly, @"<gallery>
Image : Foo.png |description
</gallery>", "Image : Foo.png |");
            RegexAssert.Matches(WikiRegexes.ImagesCountOnly, @"<GALLERY>
Image:Foo.png|description
</GALLERY>", "Image:Foo.png|");
            RegexAssert.Matches(WikiRegexes.ImagesCountOnly, @"<gallery name=bar style=silver>
File:Foo.png|description<br>text
</gallery>", "File:Foo.png|");
            RegexAssert.Matches(WikiRegexes.ImagesCountOnly, @"< gallery >
File:Foo.png
< /gallery >", "File:Foo.png");
             RegexAssert.Matches(WikiRegexes.ImagesCountOnly, @"< gallery >
File:9th of May_street, Bacău.jpg
< /gallery >", "File:9th of May_street, Bacău.jpg");
            
            MatchCollection mc = WikiRegexes.ImagesCountOnly.Matches(@"<gallery>
Image:Foo.png|description
Image:Foo2.png|description2
</gallery>");

            Assert.That(mc[0].Value, Is.EqualTo("Image:Foo.png|"));
            Assert.That(mc[1].Value, Is.EqualTo("Image:Foo2.png|"));
            
            RegexAssert.IsMatch(WikiRegexes.ImagesCountOnly, @"File:Foo.png|description");
            RegexAssert.NoMatch(WikiRegexes.ImagesCountOnly, @"<gallery>

</gallery>
Image here");
        }
        
        [Test]
        public void ImagesCountOnlyTestsGalleryTemplate()
        {
            RegexAssert.IsMatch(WikiRegexes.ImagesCountOnly, @"{{Gallery
|title=Lamu images
|width=150
|lines=2
|Image:LamuFort.jpg|Lamu Fort
|Image:LAMU Riyadha Mosque.jpg|Riyadha Mosque
|Image:04 Donkey Hospital (June 30 2001).jpg|Donkey Sanctuary
}}");
            RegexAssert.IsMatch(WikiRegexes.ImagesCountOnly, @"{{ gallery
|title=Lamu images
|width=150
|lines=2
|Image:LamuFort.jpg|Lamu Fort
|Image:LAMU Riyadha Mosque.jpg|Riyadha Mosque
|Image:04 Donkey Hospital (June 30 2001).jpg|Donkey Sanctuary
}}");
        }
        
        [Test]
        public void ImagesNotTemplatesTestsStandard()
        {
            RegexAssert.IsMatch(WikiRegexes.ImagesNotTemplates, "[[File:Test.JPG]]");
            RegexAssert.IsMatch(WikiRegexes.ImagesNotTemplates, "[[File:Test.jpg]]");
            RegexAssert.IsMatch(WikiRegexes.ImagesNotTemplates, "[[File:Test of the.ogg]]");
            RegexAssert.Matches(WikiRegexes.ImagesNotTemplates, "[[File:Test of the.ogg]]", "[[File:Test of the.ogg]]");
            RegexAssert.IsMatch(WikiRegexes.ImagesNotTemplates, "[[File:Test_of_the.ogg]]");
            RegexAssert.IsMatch(WikiRegexes.ImagesNotTemplates, "[[Image:Test.JPG]]");
            RegexAssert.IsMatch(WikiRegexes.ImagesNotTemplates, "[[Image:Test here.png|right|200px|Some description [[here]] or there]]");
            RegexAssert.IsMatch(WikiRegexes.ImagesNotTemplates, @"[[Image:Test here.png|right|200px|Some description [[here]] or there
 over lines]]");
            RegexAssert.IsMatch(WikiRegexes.ImagesNotTemplates, "[[File:Test.JPG");
            
            RegexAssert.NoMatch(WikiRegexes.ImagesNotTemplates, "[[File Test.JPG]]");
            RegexAssert.Matches(WikiRegexes.ImagesNotTemplates, "[[Image:Shane Bernagh’s Chair. 28th may 2007 -2.jpg]]", "[[Image:Shane Bernagh’s Chair. 28th may 2007 -2.jpg]]");
        }
        
        [Test]
        public void ImagesNotTemplatesTestsInfoboxes()
        {
            // Matches file namespace links
            RegexAssert.IsMatch(WikiRegexes.ImagesNotTemplates, @"{{Infobox foo|
bar = a|
image = [[File:Test.JPG]]
| there=here}}");
            RegexAssert.IsMatch(WikiRegexes.ImagesNotTemplates, @"{{Infobox foo|
  bar = a|
  image2 = [[File:Test.JPG]]
  | there=here}}");
            RegexAssert.IsMatch(WikiRegexes.ImagesNotTemplates, @"{{Infobox foo|
  bar = a|
  img = [[File:Test.JPG]]
  | there=here}}");
            RegexAssert.IsMatch(WikiRegexes.ImagesNotTemplates, @"{{Infobox foo|
  bar = a|
  img = File:Test.JPG
  | there=here}}");
            RegexAssert.IsMatch(WikiRegexes.ImagesNotTemplates, @"{{Infobox foo|
  bar = a|
  cover = [[Image:Test.JPG]]
  | there=here}}");
            RegexAssert.IsMatch(WikiRegexes.ImagesNotTemplates, @"{{Infobox foo|
  bar = a|map=[[File:Test.JPG]]
  | there=here}}");
            RegexAssert.IsMatch(WikiRegexes.ImagesNotTemplates, @"{{Infobox foo|
  bar = a| map = [[File:Test.JPG]]
  | there=here}}");
            
            // no match on parameter links without File:/Image: namespace
            RegexAssert.NoMatch(WikiRegexes.ImagesNotTemplates, @"{{Infobox foo|
  bar = a|
  img = Test.JPEG
  | there=here}}");

            RegexAssert.NoMatch(WikiRegexes.ImagesNotTemplates, @"{{Infobox foo|
  bar = a|
  cover art = Test2.png
  | there=here}}");
            RegexAssert.NoMatch(WikiRegexes.ImagesNotTemplates, @"{{Infobox foo|
  bar = a|
  cover_ar = Test.JPG
  | there=here}}");

            RegexAssert.NoMatch(WikiRegexes.ImagesNotTemplates, @"{{Infobox foo|
  bar = a|
  strangename = Test.JPEG
  | there=here}}");
            
            RegexAssert.NoMatch(WikiRegexes.ImagesNotTemplates, @"{{Infobox foo|
bar = a|
picture = Test.JPG
| there=here}}");
            RegexAssert.NoMatch(WikiRegexes.ImagesNotTemplates, @"{{Infobox foo|
  bar = a|
  picture = Test.JPG
  | there=here}}", @"|
  picture = Test.JPG");
        }
        
        [Test]
        public void ImagesNotTemplatesTestsGalleryTag()
        {
            RegexAssert.Matches(WikiRegexes.ImagesNotTemplates, @"<gallery>
File:Foo.png
</gallery>", "File:Foo.png");
            RegexAssert.Matches(WikiRegexes.ImagesNotTemplates, @"<gallery>
File:Foo.png|description
</gallery>", "File:Foo.png|");
            RegexAssert.Matches(WikiRegexes.ImagesNotTemplates, @"<gallery>
Image:Foo.png|description
</gallery>", "Image:Foo.png|");
            RegexAssert.Matches(WikiRegexes.ImagesNotTemplates, @"<gallery>
Image : Foo.png |description
</gallery>", "Image : Foo.png |");
            RegexAssert.Matches(WikiRegexes.ImagesNotTemplates, @"<GALLERY>
Image:Foo.png|description
</GALLERY>", "Image:Foo.png|");
            RegexAssert.Matches(WikiRegexes.ImagesNotTemplates, @"<gallery name=bar style=silver>
File:Foo.png|description<br>text
</gallery>", "File:Foo.png|");
            RegexAssert.Matches(WikiRegexes.ImagesNotTemplates, @"< gallery >
File:Foo.png
< /gallery >", "File:Foo.png");
             RegexAssert.Matches(WikiRegexes.ImagesNotTemplates, @"< gallery >
File:9th of May_street, Bacău.jpg
< /gallery >", "File:9th of May_street, Bacău.jpg");
            
            MatchCollection mc = WikiRegexes.ImagesNotTemplates.Matches(@"<gallery>
Image:Foo.png|description
Image:Foo2.png|description2
</gallery>");

            Assert.That(mc[0].Value, Is.EqualTo("Image:Foo.png|"));
            Assert.That(mc[1].Value, Is.EqualTo("Image:Foo2.png|"));
            
            RegexAssert.IsMatch(WikiRegexes.ImagesNotTemplates, @"File:Foo.png|description");
            RegexAssert.NoMatch(WikiRegexes.ImagesNotTemplates, @"<gallery>

</gallery>
Image here");
        }
        
        [Test]
        public void ImagesNotTemplatesTestsGalleryTemplate()
        {
            RegexAssert.IsMatch(WikiRegexes.ImagesNotTemplates, @"{{Gallery
|title=Lamu images
|width=150
|lines=2
|Image:LamuFort.jpg|Lamu Fort
|Image:LAMU Riyadha Mosque.jpg|Riyadha Mosque
|Image:04 Donkey Hospital (June 30 2001).jpg|Donkey Sanctuary
}}");
            RegexAssert.IsMatch(WikiRegexes.ImagesNotTemplates, @"{{ gallery
|title=Lamu images
|width=150
|lines=2
|Image:LamuFort.jpg|Lamu Fort
|Image:LAMU Riyadha Mosque.jpg|Riyadha Mosque
|Image:04 Donkey Hospital (June 30 2001).jpg|Donkey Sanctuary
}}");
        }
        
        [Test]
        public void LooseImageTests()
        {
            RegexAssert.IsMatch(WikiRegexes.LooseImage, "[[File:Test.JPG]]");
            RegexAssert.IsMatch(WikiRegexes.LooseImage, "[[ File:Test.JPG]]");
            RegexAssert.IsMatch(WikiRegexes.LooseImage, "[[ File :Test.JPG]]");
            RegexAssert.IsMatch(WikiRegexes.LooseImage, "[[File:Test.jpg]]");
            RegexAssert.IsMatch(WikiRegexes.LooseImage, "[[File:Test of the.ogg]]");
            RegexAssert.IsMatch(WikiRegexes.LooseImage, "[[File:Test_of_the.ogg]]");
            RegexAssert.IsMatch(WikiRegexes.LooseImage, "[[Image:Test.JPG]]");
            RegexAssert.IsMatch(WikiRegexes.LooseImage, "[[  Image: Test.JPG]]");
            RegexAssert.IsMatch(WikiRegexes.LooseImage, "[[Image:Test here.png|right|200px|Some description [[here]] or there]]");
            RegexAssert.IsMatch(WikiRegexes.LooseImage, @"[[Image:Test here.png|right|200px|Some description [[here]] or there
 over lines]]");
            RegexAssert.NoMatch(WikiRegexes.LooseImage, "[[File:Test.JPG");
        }
        
        [Test]
        public void FileNamespaceLinkTests()
        {
            RegexAssert.IsMatch(WikiRegexes.FileNamespaceLink, "[[File:Test.JPG]]");
            RegexAssert.IsMatch(WikiRegexes.FileNamespaceLink, "[[ File:Test.JPG]]");
            RegexAssert.IsMatch(WikiRegexes.FileNamespaceLink, "[[ File :Test.JPG]]");
            RegexAssert.IsMatch(WikiRegexes.FileNamespaceLink, "[[File:Test.jpg]]");
            RegexAssert.IsMatch(WikiRegexes.FileNamespaceLink, "[[File:Test of the.ogg]]");
            RegexAssert.IsMatch(WikiRegexes.FileNamespaceLink, "[[File:Test_of_the.ogg]]");
            RegexAssert.IsMatch(WikiRegexes.FileNamespaceLink, "[[Image:Test.JPG]]");
            RegexAssert.IsMatch(WikiRegexes.FileNamespaceLink, "[[Image:Test here.png|right|200px|Some description [[here]] or there]]");
            RegexAssert.IsMatch(WikiRegexes.FileNamespaceLink, @"[[Image:Test here.png|right|200px|Some description [[here]] or there
 over lines]]");
            
            RegexAssert.NoMatch(WikiRegexes.FileNamespaceLink, "[[File:Test.JPG");
            RegexAssert.NoMatch(WikiRegexes.FileNamespaceLink, "[[File Test.JPG]]");

            Assert.That(WikiRegexes.FileNamespaceLink.Match(@"[[ File :Test.JPG]]").Groups[1].Value, Is.EqualTo("Test.JPG"));
        }

        [Test]
        public void UserSignature()
        {
            RegexAssert.IsMatch(WikiRegexes.UserSignature, "[[User:");
            RegexAssert.IsMatch(WikiRegexes.UserSignature, "[[User talk:");
            RegexAssert.IsMatch(WikiRegexes.UserSignature, "[[User talk :");
            RegexAssert.IsMatch(WikiRegexes.UserSignature, "[[User_talk:");
        }


        [Test]
        public void StubTests()
        {
            RegexAssert.IsMatch(WikiRegexes.Stub, @"{{footballer-bio-stub}}");
            RegexAssert.IsMatch(WikiRegexes.Stub, @"{{ footballer-bio-stub}}");
            RegexAssert.IsMatch(WikiRegexes.Stub, @"{{ Footballer-Bio-Stub}}");
            RegexAssert.IsMatch(WikiRegexes.Stub, @"{{bio-stub}}");
            RegexAssert.IsMatch(WikiRegexes.Stub, @"{{ bio-stub }}");
            RegexAssert.IsMatch(WikiRegexes.Stub, @"{{stub}}");
            RegexAssert.IsMatch(WikiRegexes.Stub, @"{{ stub}}");
            RegexAssert.IsMatch(WikiRegexes.Stub, @"{{Stub}}");
            RegexAssert.IsMatch(WikiRegexes.Stub, @"{{-stub}}");
            RegexAssert.IsMatch(WikiRegexes.Stub, @"{{Japan-spacecraft-stub|nocat=yes}}");
            RegexAssert.IsMatch(WikiRegexes.Stub, @"{{Japan-spacecraft-stub|date=August 2012}}");
            RegexAssert.IsMatch(WikiRegexes.Stub, @"{{Uncategorized stub}}");
            RegexAssert.IsMatch(WikiRegexes.Stub, @"{{Uncategorized stub|date=March 2013}}");

            RegexAssert.NoMatch(WikiRegexes.Stub, @"{{Uncategorized stub|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}", "stub does not match templates with nesting");

            RegexAssert.NoMatch(WikiRegexes.Stub, @"{{now stubborn}}");
            RegexAssert.NoMatch(WikiRegexes.Stub, @"{{stubby}}");
            RegexAssert.NoMatch(WikiRegexes.Stub, @"{{foo|stub}}");
            RegexAssert.NoMatch(WikiRegexes.Stub, @":{{main|stub (electronics)#Short circuited stub|l1=stub}}");

            #if DEBUG
            Variables.SetProjectLangCode("ar");
            WikiRegexes.MakeLangSpecificRegexes();
            
            RegexAssert.IsMatch(WikiRegexes.Stub, @"{{بذرة}}");
            RegexAssert.IsMatch(WikiRegexes.Stub, @"{{stub}}");
            RegexAssert.IsMatch(WikiRegexes.Stub, @"{{قالب:بذرة أعلام البرتغال}}"); // Portugal-bio-stub
            RegexAssert.IsMatch(WikiRegexes.Stub, @"{{قالب:بذرة كرة سلة}}"); // basketball-stub
            RegexAssert.IsMatch(WikiRegexes.Stub, @"{{بذرة غير مصنفة}}"); // uncategorised stub

            Variables.SetProjectLangCode("arz");
            WikiRegexes.MakeLangSpecificRegexes();
            
            RegexAssert.IsMatch(WikiRegexes.Stub, @"{{بذرة}}");
            RegexAssert.IsMatch(WikiRegexes.Stub, @"{{تقاوى}}");
            RegexAssert.IsMatch(WikiRegexes.Stub, @"{{stub}}");
            RegexAssert.IsMatch(WikiRegexes.Stub, @"{{قالب:تقاوى تونس}}"); // Tunisia-stub
            RegexAssert.IsMatch(WikiRegexes.Stub, @"{{تقاوى مش متصنفه}}"); // uncategorised stub

            Variables.SetProjectLangCode("sv");
            WikiRegexes.MakeLangSpecificRegexes();

            RegexAssert.IsMatch(WikiRegexes.Stub, @"{{1920-talsstub}}");
            RegexAssert.IsMatch(WikiRegexes.Stub, @"{{1910-talsstub-USA}}");
            RegexAssert.IsMatch(WikiRegexes.Stub, @"{{stub}}");
            RegexAssert.IsMatch(WikiRegexes.Stub, @"{{Robotskapad stub om hjuldjur}}");
            RegexAssert.IsMatch(WikiRegexes.Stub, @"{{Robotskapad djurstub}}");

            RegexAssert.NoMatch(WikiRegexes.Stub, @"{{uncategorized stub}}");
            RegexAssert.NoMatch(WikiRegexes.Stub, @"{{wordwithstubin}}");
            RegexAssert.NoMatch(WikiRegexes.Stub, @"{{substub}}");
            RegexAssert.NoMatch(WikiRegexes.Stub, @"{{stubbmall}}");
            RegexAssert.NoMatch(WikiRegexes.Stub, @"{{ Stubbmall }}");
            RegexAssert.NoMatch(WikiRegexes.Stub, @"{{Stubavsnitt}}");

            Variables.SetProjectLangCode("zh");
            WikiRegexes.MakeLangSpecificRegexes();
            RegexAssert.IsMatch(WikiRegexes.Stub, @"{{保加利亞地理小作品}}");
            RegexAssert.IsMatch(WikiRegexes.Stub, @"{{Germany-geo-stub}}");

            Variables.SetProjectLangCode("sco");
            WikiRegexes.MakeLangSpecificRegexes();
            RegexAssert.IsMatch(WikiRegexes.Stub, @"{{Stub/foo}}");
            RegexAssert.IsMatch(WikiRegexes.Stub, @"{{Stub/foo bar}}");
            RegexAssert.IsMatch(WikiRegexes.Stub, @"{{foo-stub}}");

            Variables.SetProject("en", ProjectEnum.wikipedia);
            WikiRegexes.MakeLangSpecificRegexes();
            #endif
        }

        [Test]
        public void PossiblyCommentedStubTests()
        {
            RegexAssert.IsMatch(WikiRegexes.PossiblyCommentedStub, @"{{footballer-bio-stub}}");
            RegexAssert.IsMatch(WikiRegexes.PossiblyCommentedStub, @"{{ footballer-bio-stub}}");
            RegexAssert.IsMatch(WikiRegexes.PossiblyCommentedStub, @"{{ Footballer-Bio-Stub}}");
            RegexAssert.IsMatch(WikiRegexes.PossiblyCommentedStub, @"{{ Footballer-Bio-Stub|date=May 2012}}");
            RegexAssert.IsMatch(WikiRegexes.PossiblyCommentedStub, @"{{bio-stub}}");
            RegexAssert.IsMatch(WikiRegexes.PossiblyCommentedStub, @"{{ bio-stub }}");
            RegexAssert.IsMatch(WikiRegexes.PossiblyCommentedStub, @"{{stub}}");
            RegexAssert.IsMatch(WikiRegexes.PossiblyCommentedStub, @"{{ stub}}");
            RegexAssert.IsMatch(WikiRegexes.PossiblyCommentedStub, @"{{Stub}}");
            RegexAssert.IsMatch(WikiRegexes.PossiblyCommentedStub, @"<!--{{Stub}}-->");
            RegexAssert.IsMatch(WikiRegexes.PossiblyCommentedStub, @"{{Uncategorized stub|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");

            RegexAssert.NoMatch(WikiRegexes.PossiblyCommentedStub, @"{{now stubborn}}");
            RegexAssert.NoMatch(WikiRegexes.PossiblyCommentedStub, @"{{stubby}}");
        }

        [Test]
        public void TemplateCallTests()
        {
            RegexAssert.IsMatch(WikiRegexes.TemplateCall, @"{{now stubborn}}");
            RegexAssert.IsMatch(WikiRegexes.TemplateCall, @"{{ now stubborn}}");
            RegexAssert.IsMatch(WikiRegexes.TemplateCall, @"{{
now stubborn}}");
            RegexAssert.IsMatch(WikiRegexes.TemplateCall, @"{{now stubborn|abc|derf=gh|ijk}}");
            RegexAssert.IsMatch(WikiRegexes.TemplateCall, @"{{Template:now stubborn}}");
            RegexAssert.IsMatch(WikiRegexes.TemplateCall, @"{{template:now stubborn}}");
            
            RegexAssert.NoMatch(WikiRegexes.TemplateCall, "[[Test]]");
            RegexAssert.NoMatch(WikiRegexes.TemplateCall, "Test");
            RegexAssert.NoMatch(WikiRegexes.TemplateCall, "[[Image:Test.jpg]]");
            RegexAssert.IsMatch(WikiRegexes.TemplateCall, @"{{cite book|url=http://www.site.com
|pages = 50{{ndash}}70
|year=2009");
        }

        [Test]
        public void DatesTests()
        {
            RegexAssert.IsMatch(WikiRegexes.Dates, @"1 May");
            RegexAssert.IsMatch(WikiRegexes.Dates, @"1 June");
            RegexAssert.IsMatch(WikiRegexes.Dates, @"01 May");
            RegexAssert.IsMatch(WikiRegexes.Dates, @"11 May");
            RegexAssert.IsMatch(WikiRegexes.Dates, @"31 May");
            
            RegexAssert.NoMatch(WikiRegexes.Dates, @"33 May");
            RegexAssert.NoMatch(WikiRegexes.Dates, @"May 13");
            RegexAssert.NoMatch(WikiRegexes.Dates, @"3 Maybe");
            RegexAssert.NoMatch(WikiRegexes.Dates, @"3 may");
        }

        [Test]
        public void Dates2Tests()
        {
            RegexAssert.IsMatch(WikiRegexes.Dates2, @"May 1");
            RegexAssert.IsMatch(WikiRegexes.Dates2, @"June 1");
            RegexAssert.IsMatch(WikiRegexes.Dates2, @"May 01");
            RegexAssert.IsMatch(WikiRegexes.Dates2, @"May 11");
            RegexAssert.IsMatch(WikiRegexes.Dates2, @"May 31");
            
            RegexAssert.NoMatch(WikiRegexes.Dates2, @"May 33");
            RegexAssert.NoMatch(WikiRegexes.Dates2, @"13 May");
            RegexAssert.NoMatch(WikiRegexes.Dates2, @"MigMay 3");
            RegexAssert.NoMatch(WikiRegexes.Dates2, @"may 3");
        }

        [Test]
        public void RedirectTests()
        {
            RegexAssert.IsMatch(WikiRegexes.Redirect, @"#REDIRECT:[[Foo]]");
            RegexAssert.IsMatch(WikiRegexes.Redirect, @"#redirect:[[Foo]]");
            RegexAssert.IsMatch(WikiRegexes.Redirect, @"#redirect [[Foo]]");
            RegexAssert.IsMatch(WikiRegexes.Redirect, @"#REDIRECT [[Foo]]");
            RegexAssert.IsMatch(WikiRegexes.Redirect, @"#REDIRECT   :   [[Foo]]");
            RegexAssert.IsMatch(WikiRegexes.Redirect, @"#REDIRECT:[[Foo bar]]");
            RegexAssert.IsMatch(WikiRegexes.Redirect, @"#REDIRECT:[[ Foo bar]]");
            RegexAssert.IsMatch(WikiRegexes.Redirect, @"#REDIRECT:[[ :Foo bar]]");
            RegexAssert.IsMatch(WikiRegexes.Redirect, @"#REDIRECT:[[Foo bar|the best]]");
            RegexAssert.IsMatch(WikiRegexes.Redirect, @"#REDIRECT:[[Foo bar#best]]");

            RegexAssert.IsMatch(WikiRegexes.Redirect, @"#REDIRECT=[[Foo]]");
            RegexAssert.IsMatch(WikiRegexes.Redirect, @"#REDIRECT = [[Foo bar#best]]");

            RegexAssert.IsMatch(WikiRegexes.Redirect, @"#REDIRECT[[Foo]] {{R from move}}");
            
            RegexAssert.NoMatch(WikiRegexes.Redirect, @"#  REDIRECT:[[Foo]]");
        }
        
        [Test]
        public void OrphanTests()
        {
            ClassicAssert.IsTrue(WikiRegexes.Orphan.IsMatch(@"{{orphan}}"));
            ClassicAssert.IsTrue(WikiRegexes.Orphan.IsMatch(@"{{Orphan}}"));
            ClassicAssert.IsTrue(WikiRegexes.Orphan.IsMatch(@"{{orphan|date=May 2008}}"));
            ClassicAssert.IsTrue(WikiRegexes.Orphan.IsMatch(@"{{Orphan| date = May 2008}}"));
            ClassicAssert.IsTrue(WikiRegexes.Orphan.IsMatch(@"{{Orphan | date = May 2008}}"));
            ClassicAssert.IsTrue(WikiRegexes.Orphan.IsMatch(@"{{orphan|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"));
            
            ClassicAssert.IsFalse(WikiRegexes.Orphan.IsMatch(@"{{orphanblahblah}}"));
            
            #if DEBUG
            Variables.SetProjectLangCode("ar");
            WikiRegexes.MakeLangSpecificRegexes();
            ClassicAssert.IsFalse(WikiRegexes.Orphan.IsMatch(@"{{orphan}}"));
            ClassicAssert.IsTrue(WikiRegexes.Orphan.IsMatch(@"{{يتيمة}}"));
            
            Variables.SetProjectLangCode("arz");
            WikiRegexes.MakeLangSpecificRegexes();
            ClassicAssert.IsFalse(WikiRegexes.Orphan.IsMatch(@"{{orphan}}"));
            ClassicAssert.IsTrue(WikiRegexes.Orphan.IsMatch(@"{{يتيمه}}"));

            Variables.SetProjectLangCode("hy");
            WikiRegexes.MakeLangSpecificRegexes();
            ClassicAssert.IsFalse(WikiRegexes.Orphan.IsMatch(@"{{orphan}}"));
            ClassicAssert.IsTrue(WikiRegexes.Orphan.IsMatch(@"{{Որբ}}"));

            Variables.SetProjectLangCode("ru");
            WikiRegexes.MakeLangSpecificRegexes();
            ClassicAssert.IsTrue(WikiRegexes.Orphan.IsMatch(@"{{orphan|date=февраль 2016}}"));
            ClassicAssert.IsTrue(WikiRegexes.Orphan.IsMatch(@"{{изолированная статья}}"));
            ClassicAssert.IsTrue(WikiRegexes.Orphan.IsMatch(@"{{Сирота}}"));

            Variables.SetProjectLangCode("sv");
            WikiRegexes.MakeLangSpecificRegexes();
            ClassicAssert.IsFalse(WikiRegexes.Orphan.IsMatch(@"{{orphan}}"));
            ClassicAssert.IsTrue(WikiRegexes.Orphan.IsMatch(@"{{föräldralös}}"));
            ClassicAssert.IsTrue(WikiRegexes.Orphan.IsMatch(@"{{Föräldralös}}"));
            
            Variables.SetProjectLangCode("zh");
            WikiRegexes.MakeLangSpecificRegexes();
            ClassicAssert.IsTrue(WikiRegexes.Orphan.IsMatch(@"{{orphan}}"));
            ClassicAssert.IsTrue(WikiRegexes.Orphan.IsMatch(@"{{orphan|time=2014-03-07}}"));

            Variables.SetProjectLangCode("en");
            WikiRegexes.MakeLangSpecificRegexes();
            #endif
        }
        
        [Test]
        public void WikifyTests()
        {
            ClassicAssert.IsTrue(WikiRegexes.Wikify.IsMatch(@"{{wikify}}"));
            ClassicAssert.IsTrue(WikiRegexes.Wikify.IsMatch(@"{{underlinked}}"));
            ClassicAssert.IsTrue(WikiRegexes.Wikify.IsMatch(@"{{wikify|date=March 2009}}"));
            ClassicAssert.IsTrue(WikiRegexes.Wikify.IsMatch(@"{{underlinked|date=March 2009}}"));
            ClassicAssert.IsTrue(WikiRegexes.Wikify.IsMatch(@"{{Wikify}}"));
            ClassicAssert.IsTrue(WikiRegexes.Wikify.IsMatch(@"{{Wikify|date=March 2009}}"));
            ClassicAssert.IsTrue(WikiRegexes.Wikify.IsMatch(@"{{wikify|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"));

            ClassicAssert.IsFalse(WikiRegexes.Wikify.IsMatch(@"{{wikifyworldblah}}"));
            
            #if DEBUG
            Variables.SetProjectLangCode("sv");
            WikiRegexes.MakeLangSpecificRegexes();
            
            ClassicAssert.IsFalse(WikiRegexes.Wikify.IsMatch(@"{{wikify}}"));
            ClassicAssert.IsTrue(WikiRegexes.Wikify.IsMatch(@"{{ickewiki}}"));
            
            Variables.SetProjectLangCode("ar");
            WikiRegexes.MakeLangSpecificRegexes();

            ClassicAssert.IsFalse(WikiRegexes.Wikify.IsMatch(@"{{wikify}}"));
            ClassicAssert.IsTrue(WikiRegexes.Wikify.IsMatch(@"{{وصلات قليلة}}"));

            Variables.SetProjectLangCode("arz");
            WikiRegexes.MakeLangSpecificRegexes();

            ClassicAssert.IsFalse(WikiRegexes.Wikify.IsMatch(@"{{wikify}}"));
            ClassicAssert.IsTrue(WikiRegexes.Wikify.IsMatch(@"{{ويكى}}"));

            Variables.SetProjectLangCode("hy");
            WikiRegexes.MakeLangSpecificRegexes();
            
            ClassicAssert.IsFalse(WikiRegexes.Wikify.IsMatch(@"{{wikify}}"));
            ClassicAssert.IsTrue(WikiRegexes.Wikify.IsMatch(@"{{Վիքիֆիկացում}}"));

            Variables.SetProjectLangCode("ru");
            WikiRegexes.MakeLangSpecificRegexes();
            
            ClassicAssert.IsTrue(WikiRegexes.Wikify.IsMatch(@"{{wikify}}"));
            ClassicAssert.IsTrue(WikiRegexes.Wikify.IsMatch(@"{{underlinked}}"));

            Variables.SetProjectLangCode("en");
            WikiRegexes.MakeLangSpecificRegexes();
            #endif
        }
        
        [Test]
        public void DeadEndTests()
        {
            ClassicAssert.IsTrue(WikiRegexes.DeadEnd.IsMatch(@"{{Deadend}}"));
            ClassicAssert.IsTrue(WikiRegexes.DeadEnd.IsMatch(@"{{deadend}}"));
            ClassicAssert.IsTrue(WikiRegexes.DeadEnd.IsMatch(@"{{dead end}}"));
            ClassicAssert.IsTrue(WikiRegexes.DeadEnd.IsMatch(@"{{deadend|date=May 2008}}"));
            ClassicAssert.IsTrue(WikiRegexes.DeadEnd.IsMatch(@"{{Dead end}}"));
            ClassicAssert.IsTrue(WikiRegexes.DeadEnd.IsMatch(@"{{internal links}}"));
            ClassicAssert.IsTrue(WikiRegexes.DeadEnd.IsMatch(@"{{internallinks}}"));
            ClassicAssert.IsTrue(WikiRegexes.DeadEnd.IsMatch(@"{{Internal links}}"));
            ClassicAssert.IsTrue(WikiRegexes.DeadEnd.IsMatch(@"{{Internal links|date=May 2008}}"));
            ClassicAssert.IsTrue(WikiRegexes.DeadEnd.IsMatch(@"{{nuevointernallinks}}"));
            ClassicAssert.IsTrue(WikiRegexes.DeadEnd.IsMatch(@"{{Nuevointernallinks}}"));
            ClassicAssert.IsTrue(WikiRegexes.DeadEnd.IsMatch(@"{{dep}}"));
            ClassicAssert.IsTrue(WikiRegexes.DeadEnd.IsMatch(@"{{dep|date=May 2008|Foobar}}"));

            ClassicAssert.IsTrue(WikiRegexes.DeadEnd.IsMatch(@"{{Dead end|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"));

            #if DEBUG
            Variables.SetProjectLangCode("ar");
            WikiRegexes.MakeLangSpecificRegexes();
            
            ClassicAssert.IsTrue(WikiRegexes.DeadEnd.IsMatch(@"{{dead end}}"), "ar dead end");
            ClassicAssert.IsTrue(WikiRegexes.DeadEnd.IsMatch(@"{{internal links}}"), "ar dead end");
            ClassicAssert.IsTrue(WikiRegexes.DeadEnd.IsMatch(@"{{نهاية مسدودة}}"), "ar dead end");
            
            Variables.SetProjectLangCode("arz");
            WikiRegexes.MakeLangSpecificRegexes();
            
            ClassicAssert.IsTrue(WikiRegexes.DeadEnd.IsMatch(@"{{dead end}}"), "arz dead end");
            ClassicAssert.IsFalse(WikiRegexes.DeadEnd.IsMatch(@"{{internal links}}"), "arz dead end");
            ClassicAssert.IsTrue(WikiRegexes.DeadEnd.IsMatch(@"{{نهايه مسدوده}}"), "arz dead end");

            Variables.SetProjectLangCode("hy");
            WikiRegexes.MakeLangSpecificRegexes();
            
            ClassicAssert.IsTrue(WikiRegexes.DeadEnd.IsMatch(@"{{dead end}}"), "hy dead end");
            ClassicAssert.IsFalse(WikiRegexes.DeadEnd.IsMatch(@"{{internal links}}"), "hy dead end");
            ClassicAssert.IsTrue(WikiRegexes.DeadEnd.IsMatch(@"{{Փակ}}"), "hy dead end");

            Variables.SetProjectLangCode("ru");
            WikiRegexes.MakeLangSpecificRegexes();

            ClassicAssert.IsTrue(WikiRegexes.DeadEnd.IsMatch(@"{{dead end}}"), "ru dead end");
            ClassicAssert.IsFalse(WikiRegexes.DeadEnd.IsMatch(@"{{internal links}}"), "ru dead end");
            ClassicAssert.IsTrue(WikiRegexes.DeadEnd.IsMatch(@"{{Tупиковая статья}}"), "ru dead end");

            Variables.SetProjectLangCode("en");
            WikiRegexes.MakeLangSpecificRegexes();
            #endif
        }

        [Test]
        public void InUseTests()
        {
            ClassicAssert.IsTrue(WikiRegexes.InUse.IsMatch(@"{{in use}}"));
            ClassicAssert.IsTrue(WikiRegexes.InUse.IsMatch(@"{{inuse}}"));
            ClassicAssert.IsTrue(WikiRegexes.InUse.IsMatch(@"{{in creation}}"));
            ClassicAssert.IsTrue(WikiRegexes.InUse.IsMatch(@"{{GOCEinuse}}"));
            ClassicAssert.IsTrue(WikiRegexes.InUse.IsMatch(@"{{GOCE in use}}"));

#if DEBUG
            Variables.SetProjectLangCode("el");
            WikiRegexes.MakeLangSpecificRegexes();
            
            ClassicAssert.IsTrue(WikiRegexes.InUse.IsMatch(@"{{σε χρήση}}"), "σε χρήση");
            ClassicAssert.IsTrue(WikiRegexes.InUse.IsMatch(@"{{inuse}}"));
            ClassicAssert.IsFalse(WikiRegexes.InUse.IsMatch(@"{{goceinuse}}"), "goceinuse is en-only");            
            Variables.SetProjectLangCode("en");
            WikiRegexes.MakeLangSpecificRegexes();
            #endif
        }

        [Test]
        public void DateYearMonthParameterTests()
        {
            #if DEBUG
            Variables.SetProjectLangCode("fr");
            WikiRegexes.MakeLangSpecificRegexes();

            Assert.That(WikiRegexes.DateYearMonthParameter, Is.EqualTo(@"date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}"));
            
            Variables.SetProjectLangCode("sv");
            WikiRegexes.MakeLangSpecificRegexes();

            Assert.That(WikiRegexes.DateYearMonthParameter, Is.EqualTo(@"datum={{subst:CURRENTYEAR}}-{{subst:CURRENTMONTH}}"));
            
            Variables.SetProjectLangCode("zh");
            WikiRegexes.MakeLangSpecificRegexes();

            Assert.That(WikiRegexes.DateYearMonthParameter, Is.EqualTo(@"time={{subst:#time:c}}"));

            Variables.SetProjectLangCode("en");
            WikiRegexes.MakeLangSpecificRegexes();

            Assert.That(WikiRegexes.DateYearMonthParameter, Is.EqualTo(@"date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}"));
            #endif
        }
        
        [Test]
        public void UncatTests()
        {
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Uncategorized}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Uncategorised}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{uncategorized}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{uncategorised}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{uncat}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Uncat}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Uncat|date=January 2009}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{uncategorised|date=May 2008}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Uncategorised|date=May 2008}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Uncategorisedstub|date=May 2008}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Uncategorisedstub|date = May 2008}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{uncategorised|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"));
            
            ClassicAssert.IsTrue(WikiRegexes.Uncat.Match(@"{{Uncat}}").Groups[1].Value.Equals("Uncat"));

            // all the other redirects
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Classify}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{CatNeeded}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Catneeded}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Uncategorised}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Uncat}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Categorize}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Categories needed}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Categoryneeded}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Category needed}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Category requested}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Categories requested}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Nocats}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Categorise}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Nocat}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Uncat-date}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Uncategorized-date}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Needs cat}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Needs cats}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Cats needed}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Cat needed}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{classify}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{catneeded}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{catneeded}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{uncategorised}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{uncat}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{categorize}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{categories needed}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{categoryneeded}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{category needed}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{category requested}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{categories requested}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{nocats}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{categorise}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{nocat}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{uncat-date}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{uncategorized-date}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{needs cat}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{needs cats}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{cats needed}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{cat needed}}"));
            
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{uncategorizedstub}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{uncategorized stub}}"));

            // no match
            ClassicAssert.IsFalse(WikiRegexes.Uncat.IsMatch(@"{{Uncategorized other template}}"));
            ClassicAssert.IsFalse(WikiRegexes.Uncat.IsMatch(@"{{Uncategorized other template|foo=bar}}"));

            // language variation
            #if DEBUG
            Variables.SetProjectLangCode("sv");
            WikiRegexes.MakeLangSpecificRegexes();

            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{okategoriserad}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{okategoriserad|datum=May-2013}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{okategoriserad|datum={{subst:CURRENTYEAR}}-{{subst:CURRENTMONTH}}}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Okategoriserad}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{uncategorizedstub}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{uncategorized stub}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Uncategorised}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Uncategorized}}"));
            Assert.That(WikiRegexes.Uncat.Match(@"{{okategoriserad}}").Groups[1].Value, Is.EqualTo("okategoriserad"));

            Variables.SetProjectLangCode("ar");
            WikiRegexes.MakeLangSpecificRegexes();

            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{غير مصنف}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{غير مصنفة}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{بذرة غير مصنفة}}"));

            Variables.SetProjectLangCode("arz");
            WikiRegexes.MakeLangSpecificRegexes();

            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{مش متصنفه}}"));
            ClassicAssert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{تقاوى مش متصنفه}}"));

            Variables.SetProjectLangCode("en");
            WikiRegexes.MakeLangSpecificRegexes();
            #endif
        }

        [Test]
        public void DisambigsTests()
        {
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{disambig}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{disambig|surname}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{
disambig|surname
}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{disambig|page=Foo|page2=Bar}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{disamb}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{disamb  }}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{disamb|}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{disamb|foo}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{dab}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Disambig}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{disambig-cleanup}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Disambiguation cleanup}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Chinese title disambiguation}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{chinese title disambiguation}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{chinese title disambig}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Genus disambiguation}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{genus disambig}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{hndis}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{human name disambiguation}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Hndis-cleanup}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Mathdab}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Mathematical disambiguation}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Numberdis}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Number disambiguation}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{numberdis}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Letter-NumberCombdisambig}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Letter-NumberCombDisambig}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Letter-Number Combination Disambiguation}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Letter–number combination disambiguation}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{  disambig}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Template:disambig}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{template:disambig}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{LatinNameDisambig}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Schooldis}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{School disambiguation}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{SpeciesLatinNameDisambig}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Species Latin name disambiguation}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Species Latin name abbreviation disambiguation}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Hurricane season disambiguation}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{airport disambiguation}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{airport disambig}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Call sign disambiguation}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Callsigndis}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Mil-unit-dis}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Road disambiguation}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{place name disambiguation}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{station disambiguation}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{geodis}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{geo-dis}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{dis}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{WoO number disambiguation}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Disambiguation with potential}}");

            RegexAssert.NoMatch(WikiRegexes.Disambigs, @"{{now disambig}}");
            RegexAssert.NoMatch(WikiRegexes.Disambigs, @"{{dablink|foo}}");
            RegexAssert.Matches(@"{{dab}} <!--comm-->", WikiRegexes.Disambigs, @"{{dab}} <!--comm-->");
            RegexAssert.Matches(@"{{dab}}<!--comm-->", WikiRegexes.Disambigs, @"{{dab}}<!--comm-->
ABC");

            RegexAssert.IsMatch(WikiRegexes.DisambigsGeneral, @"{{Disamb}}");
            RegexAssert.IsMatch(WikiRegexes.DisambigsGeneral, @"{{Disambig}}");
            RegexAssert.IsMatch(WikiRegexes.DisambigsGeneral, @"{{Disambiguation}}");
            RegexAssert.IsMatch(WikiRegexes.DisambigsGeneral, @"{{Dab}}");

            RegexAssert.IsMatch(WikiRegexes.DisambigsCleanup, @"{{Disambig-cleanup}}");
            RegexAssert.IsMatch(WikiRegexes.DisambigsCleanup, @"{{Disambig cleanup}}");
            RegexAssert.IsMatch(WikiRegexes.DisambigsCleanup, @"{{Disambiguation cleanup}}");
            RegexAssert.IsMatch(WikiRegexes.DisambigsCleanup, @"{{Disambiguation cleanup}} <!-- page needs cleanup-->");

            // language variation
            #if DEBUG
            Variables.SetProjectLangCode("sv");
            WikiRegexes.MakeLangSpecificRegexes();

            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Förgrening}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Betydelselista}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Dab}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Disambig}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{disambiguation}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Flertydig}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Förgreningssida}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{gaffel}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{gren}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Grensida}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Ortnamn}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Trebokstavsförgrening}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{4LA}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Namnförgrening}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Hndis}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Namngrensida}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Efternamn}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Förnamn}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Robotskapad förgrening}}");

            Variables.SetProjectLangCode("el");
            WikiRegexes.MakeLangSpecificRegexes();
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{disambig}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Αποσαφήνιση}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{αποσαφήνιση}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Αποσαφ}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{αποσαφ}}");

            Variables.SetProjectLangCode("pl");
            WikiRegexes.MakeLangSpecificRegexes();
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{disambig}}");

            Variables.SetProjectLangCode("ru");
            WikiRegexes.MakeLangSpecificRegexes();

            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{АТДы}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Военные части}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Воинские формирования}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Воинские части}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Горы}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{ЖДС}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Ждс}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Многозначность}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{НПы}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Неоднозначность}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Неоднозначность2}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Нпы}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Одноименные фильмы}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Одноимённые НП}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Одноимённые воинские части}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Одноимённые горные объекты}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Одноимённые горы}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Одноимённые железнодорожные станции}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Одноимённые координаты}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Одноимённые корабли}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Одноимённые монастыри}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Одноимённые муниципальные образования}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Одноимённые муниципальные образования}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Одноимённые населённые пункты}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Одноимённые объекты АТД}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Одноимённые озёра}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Одноимённые острова}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Одноимённые памятники}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Одноимённые площади}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Одноимённые реки}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Одноимённые станции}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Одноимённые станции метро}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Одноимённые улицы}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Одноимённые фильмы}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Одноимённые храмы}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Однофамильцы-тёзки}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Озёра}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Острова}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Реки}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Список однофамильцев}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Список однофамильцев-тёзок}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Список полных тёзок}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Список тёзок}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Список тёзок-однофамильцев}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Станции}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Тёзки-однофамильцы}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Churchdis}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Coorddis}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Disambig}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Disambiguation}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Metrodis}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Militarydis}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Mondis}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Monumdis}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Mountaindis}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Moviedis}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Placedis}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Riverdis}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Roaddis}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Shipdis}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Stationdis}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Surname}}");

            Variables.SetProjectLangCode("en");
            WikiRegexes.MakeLangSpecificRegexes();
            #endif
        }

        [Test]
        public void SIAsTests()
        {
            RegexAssert.IsMatch(WikiRegexes.SIAs, @"{{Chemistry index}}");
            RegexAssert.IsMatch(WikiRegexes.SIAs, @"{{Enzyme index}}");
            RegexAssert.IsMatch(WikiRegexes.SIAs, @"{{Lake index}}");
            RegexAssert.IsMatch(WikiRegexes.SIAs, @"{{Media set index}}");
            RegexAssert.IsMatch(WikiRegexes.SIAs, @"{{Molecular formula index}}");
            RegexAssert.IsMatch(WikiRegexes.SIAs, @"{{MolFormIndex}}");
            RegexAssert.IsMatch(WikiRegexes.SIAs, @"{{mountain index}}");
            RegexAssert.IsMatch(WikiRegexes.SIAs, @"{{mountainindex}}");
            RegexAssert.IsMatch(WikiRegexes.SIAs, @"{{Roadindex}}");
            RegexAssert.IsMatch(WikiRegexes.SIAs, @"{{Road index}}");
            RegexAssert.IsMatch(WikiRegexes.SIAs, @"{{Shipindex}}");
            RegexAssert.IsMatch(WikiRegexes.SIAs, @"{{sportindex}}");
            RegexAssert.IsMatch(WikiRegexes.SIAs, @"{{sport index}}");
            RegexAssert.IsMatch(WikiRegexes.SIAs, @"{{storm index}}");
            RegexAssert.IsMatch(WikiRegexes.SIAs, @"{{Painting index}}");
            RegexAssert.IsMatch(WikiRegexes.SIAs, @"{{Locomotive index}}");
            RegexAssert.IsMatch(WikiRegexes.SIAs, @"{{Animal common name}}");
            RegexAssert.IsMatch(WikiRegexes.SIAs, @"{{Fungus common name}}");
            RegexAssert.IsMatch(WikiRegexes.SIAs, @"{{Plant common name}}");
            
            RegexAssert.IsMatch(WikiRegexes.SIAs, @"{{surname}}");
            RegexAssert.IsMatch(WikiRegexes.SIAs, @"{{surnames}}");
            RegexAssert.IsMatch(WikiRegexes.SIAs, @"{{Nickname}}");
            RegexAssert.IsMatch(WikiRegexes.SIAs, @"{{given name}}");
            RegexAssert.IsMatch(WikiRegexes.SIAs, @"{{SIA}}");
            RegexAssert.IsMatch(WikiRegexes.SIAs, @"{{sia}}");
            RegexAssert.IsMatch(WikiRegexes.SIAs, @"{{set index}}");
            RegexAssert.IsMatch(WikiRegexes.SIAs, @"{{set index article}}");
            RegexAssert.NoMatch(WikiRegexes.SIAs, @"{{surname-stub}}");

            RegexAssert.IsMatch(WikiRegexes.SIAs, @"{{set index article|param}}");

            #if DEBUG
            Variables.SetProjectLangCode("ar");
            WikiRegexes.MakeLangSpecificRegexes();
            RegexAssert.IsMatch(WikiRegexes.SIAs, @"{{الاسم الشائع للحيوان}}");

            Variables.SetProjectLangCode("en");
            WikiRegexes.MakeLangSpecificRegexes();
            #endif
        }

        [Test]
        public void WiTests()
        {
            RegexAssert.IsMatch(WikiRegexes.Wi, @"{{Wiktionary redirect}}");
            RegexAssert.IsMatch(WikiRegexes.Wi, @"{{wiktionary redirect}}");
            RegexAssert.IsMatch(WikiRegexes.Wi, @"{{wi}}");
            RegexAssert.IsMatch(WikiRegexes.Wi, @"{{Wi}}");
            RegexAssert.IsMatch(WikiRegexes.Wi, @"{{Moved to Wiktionary}}");
            RegexAssert.IsMatch(WikiRegexes.Wi, @"{{RedirecttoWiktionary}}");
            RegexAssert.IsMatch(WikiRegexes.Wi, @"{{Seewiktionary}}");
            
            RegexAssert.IsMatch(WikiRegexes.Wi, @"{{Wikiquote redirect}}");
            RegexAssert.IsMatch(WikiRegexes.Wi, @"{{Wikivoyage redirect}}");
        }

        [Test]
        public void ExtractTitleTests()
        {
            RegexAssert.IsMatch(WikiRegexes.ExtractTitle, @"https://en.wikipedia.org/wiki/Foo");
            RegexAssert.IsMatch(WikiRegexes.ExtractTitle, @"https://en.wikipedia.org/wiki/Foo_bar");

            Assert.That(WikiRegexes.ExtractTitle.Match(@"https://en.wikipedia.org/wiki/Foo").Groups[1].Value, Is.EqualTo("Foo"));
            Assert.That(WikiRegexes.ExtractTitle.Match(@"https://en.wikipedia.org/w/index.php?title=Foo").Groups[1].Value, Is.EqualTo("Foo"));
            Assert.That(WikiRegexes.ExtractTitle.Match(@"https://en.wikipedia.org/w/index.php/Foo").Groups[1].Value, Is.EqualTo("Foo"));
            Assert.That(WikiRegexes.ExtractTitle.Match(@"https://en.wikipedia.org/w/index.php/Foo bar here").Groups[1].Value, Is.EqualTo("Foo bar here"));
            
            RegexAssert.NoMatch(WikiRegexes.ExtractTitle, @"https://random.org/wiki/Foo");
            RegexAssert.NoMatch(WikiRegexes.ExtractTitle, @"https://en.wikipedia.org/wikirandom/Foo");
        }

        [Test]
        public void EmptyLinkTests()
        {
            RegexAssert.IsMatch(WikiRegexes.EmptyLink, "[[]]");
            RegexAssert.IsMatch(WikiRegexes.EmptyLink, "[[   ]]");
            RegexAssert.IsMatch(WikiRegexes.EmptyLink, "[[|]]");
            RegexAssert.IsMatch(WikiRegexes.EmptyLink, "[[       |    ]]");
            RegexAssert.IsMatch(WikiRegexes.EmptyLink, "[[Category:]]");
            RegexAssert.IsMatch(WikiRegexes.EmptyLink, "[[ Category:]]");
            RegexAssert.IsMatch(WikiRegexes.EmptyLink, "[[Category:|]]");
            RegexAssert.IsMatch(WikiRegexes.EmptyLink, "[[Category:|A]]");
            RegexAssert.IsMatch(WikiRegexes.EmptyLink, "[[Image:]]");
            RegexAssert.IsMatch(WikiRegexes.EmptyLink, "[[File:]]");
            RegexAssert.IsMatch(WikiRegexes.EmptyLink, "[[File:|]]");
            RegexAssert.IsMatch(WikiRegexes.EmptyLink, "[[category:]]");
            RegexAssert.IsMatch(WikiRegexes.EmptyLink, "[[IMAGE:]]");
            RegexAssert.IsMatch(WikiRegexes.EmptyLink, "[[file:]]");
        }

        [Test]
        public void EmptyTemplateTests()
        {
            RegexAssert.NoMatch(WikiRegexes.EmptyTemplate, "{{TemplateTest}}");
            RegexAssert.NoMatch(WikiRegexes.EmptyTemplate, "{{Test}}");
            RegexAssert.NoMatch(WikiRegexes.EmptyTemplate, "{{Test|Parameter}}");
            RegexAssert.NoMatch(WikiRegexes.EmptyTemplate, "{{Test|}}");

            RegexAssert.IsMatch(WikiRegexes.EmptyTemplate, "{{Template:}}");
            RegexAssert.IsMatch(WikiRegexes.EmptyTemplate, "{{Template: }}");
            RegexAssert.IsMatch(WikiRegexes.EmptyTemplate, "{{template:}}");
            RegexAssert.IsMatch(WikiRegexes.EmptyTemplate, "{{}}");
            RegexAssert.IsMatch(WikiRegexes.EmptyTemplate, "{{|}}");
            RegexAssert.IsMatch(WikiRegexes.EmptyTemplate, "{{          }}");
            RegexAssert.IsMatch(WikiRegexes.EmptyTemplate, "{{|||||}}");
            RegexAssert.IsMatch(WikiRegexes.EmptyTemplate, "{{       || }}");
        }

        [Test]
        public void MonthsTests()
        {
            Regex mo = new Regex(WikiRegexes.Months);
            Assert.That(mo.Match(@"in January there").Groups[1].Value, Is.EqualTo("January"));

            Regex mong = new Regex(WikiRegexes.MonthsNoGroup);
            Assert.That(mong.Match(@"in January there").Groups[1].Value, Is.Empty);
        }

        [Test]
        public void DefaultsortTests()
        {
            RegexAssert.IsMatch(WikiRegexes.Defaultsort, "{{DEFAULTSORT:foo}}");
            RegexAssert.IsMatch(WikiRegexes.Defaultsort, "{{DEFAULTSORT|foo}}");
            RegexAssert.IsMatch(WikiRegexes.Defaultsort, "{{DEFAULTSORT:foo bar}}");

            RegexAssert.IsMatch(WikiRegexes.Defaultsort, @"{{DEFAULTSORT:foo
");
            RegexAssert.NoMatch(WikiRegexes.Defaultsort, @"{{DEFAULTSORT:foo");
            RegexAssert.IsMatch(WikiRegexes.Defaultsort, @"{{DEFAULTSORT:}}");
            Assert.That(WikiRegexes.Defaultsort.Match(@"{{DEFAULTSORT:}}").Groups["key"].Value, Is.Empty);
            
            RegexAssert.IsMatch(WikiRegexes.Defaultsort, @"{{DEFAULTSORT:
Wangchuck, Tshering Pem}}");
            
            RegexAssert.IsMatch(WikiRegexes.Defaultsort, @"{{DEFAULTSORT:
Wangchuck, Tshering Pem
}}");

            Assert.That(WikiRegexes.Defaultsort.Match(@"{{DEFAULTSORT:foo}}").Groups["key"].Value, Is.EqualTo("foo"));

            Assert.That(WikiRegexes.Defaultsort.Match(@"{{DEFAULTSORT:
foo}}").Groups["key"].Value, Is.EqualTo("foo"));

            Assert.That(WikiRegexes.Defaultsort.Match(@"{{DEFAULTSORT:{{PAGENAME}}}}").Groups["key"].Value, Is.EqualTo(@"{{PAGENAME}}"));

            Assert.That(WikiRegexes.Defaultsort.Match(@"{{DEFAULTSORT:foo}}").Value, Is.EqualTo("{{DEFAULTSORT:foo}}"));
            Assert.That(WikiRegexes.Defaultsort.Match(@"{{DEFAULTSORT:foo
now").Value, Is.EqualTo("{{DEFAULTSORT:foo\r"));

            Assert.That(WikiRegexes.Defaultsort.Match(@"{{DEFAULTSORT:{{PAGENAME}}}}").Value, Is.EqualTo(@"{{DEFAULTSORT:{{PAGENAME}}}}"));

            Assert.That(WikiRegexes.Defaultsort.Match(@"{{DEFAULTSORT:foo]]
pp").Value, Is.EqualTo(@"{{DEFAULTSORT:foo]]" + "\r"));
            Assert.That(WikiRegexes.Defaultsort.Match(@"{{DEFAULTSORT:foo]]
pp
{{x}}
").Value, Is.EqualTo(@"{{DEFAULTSORT:foo]]" + "\r"));
            Assert.That(WikiRegexes.Defaultsort.Match(@"{{DEFAULTSORT:foo
}}").Value, Is.EqualTo(@"{{DEFAULTSORT:foo
}}"));

            #if DEBUG
            Variables.SetProjectLangCode("en");
            System.Collections.Generic.List<string> ds = new System.Collections.Generic.List<string>(new[] { "DEFAULTSORT", "dsort" });
            Variables.MagicWords.Add("defaultsort", ds);
            WikiRegexes.MakeLangSpecificRegexes();
            RegexAssert.IsMatch(WikiRegexes.Defaultsort, "{{DEFAULTSORT:foo}}");
            RegexAssert.IsMatch(WikiRegexes.Defaultsort, "{{dsort:foo}}");
            Variables.MagicWords.Remove("defaultsort");
            Assert.That(WikiRegexes.Defaultsort.Match(@"{{DEFAULTSORT:foo}}<!--comm-->").Value, Is.EqualTo("{{DEFAULTSORT:foo}}"), "comment after DEFAULTSORT not extracted");

            Variables.SetProjectLangCode("sv");
            WikiRegexes.MakeLangSpecificRegexes();
            Assert.That(WikiRegexes.Defaultsort.Match(@"{{DEFAULTSORT:foo}}").Value, Is.EqualTo("{{DEFAULTSORT:foo}}"));
            Assert.That(WikiRegexes.Defaultsort.Match(@"{{DEFAULTSORT:foo}} <!--comm-->").Value, Is.EqualTo("{{DEFAULTSORT:foo}} <!--comm-->"), "comment after DEFAULTSORT allowed for sv-wiki");
            Assert.That(WikiRegexes.Defaultsort.Match(@"{{DEFAULTSORT:foo}}<!--comm-->").Value, Is.EqualTo("{{DEFAULTSORT:foo}}<!--comm-->"), "comment after DEFAULTSORT allowed for sv-wiki");

            Variables.SetProjectLangCode("en");
            WikiRegexes.MakeLangSpecificRegexes();
            #endif
        }
        
        [Test]
        public void InternationalDates()
        {
            RegexAssert.IsMatch(WikiRegexes.InternationalDates, @"On 11 July 2009");
            RegexAssert.IsMatch(WikiRegexes.InternationalDates, @"On 11 July  2009");
            RegexAssert.IsMatch(WikiRegexes.InternationalDates, @"On 1 July 1809");
            RegexAssert.IsMatch(WikiRegexes.InternationalDates, @"On 11&nbsp;July 2009");
            
            RegexAssert.NoMatch(WikiRegexes.InternationalDates, @"On 11 July 44");
            RegexAssert.NoMatch(WikiRegexes.InternationalDates, @"On July 11, 2009 a");
        }
        
        [Test]
        public void AmericanDates()
        {
            RegexAssert.IsMatch(WikiRegexes.AmericanDates, @"On July 11, 2009 a");
            RegexAssert.IsMatch(WikiRegexes.AmericanDates, @"On July 11 2009 a");
            RegexAssert.IsMatch(WikiRegexes.AmericanDates, @"On July 11,  1809 a");
            RegexAssert.IsMatch(WikiRegexes.AmericanDates, @"On July&nbsp;11, 2009 a");
            
            RegexAssert.NoMatch(WikiRegexes.AmericanDates, @"On July 11, 29 a");
            RegexAssert.NoMatch(WikiRegexes.AmericanDates, @"On 11 July 2009");
        }
        
        [Test]
        public void MonthDay()
        {
            RegexAssert.IsMatch(WikiRegexes.MonthDay, @"On July 11, 2009 a");
            RegexAssert.IsMatch(WikiRegexes.MonthDay, @"On July 11 a");
            RegexAssert.IsMatch(WikiRegexes.MonthDay, @"On July&nbsp;11 a");
            RegexAssert.IsMatch(WikiRegexes.MonthDay, @"On July 11–12 a");
            RegexAssert.IsMatch(WikiRegexes.MonthDay, @"On July 11,  1809 a");
            
            RegexAssert.NoMatch(WikiRegexes.MonthDay, @"In July 07, a");
            RegexAssert.NoMatch(WikiRegexes.MonthDay, @"In July 2007, a");
        }
        
        [Test]
        public void DayMonth()
        {
            RegexAssert.IsMatch(WikiRegexes.DayMonth, @"On 11 July, 2009 a");
            RegexAssert.IsMatch(WikiRegexes.DayMonth, @"On 11 July 2009 a");
            RegexAssert.IsMatch(WikiRegexes.DayMonth, @"On 11 July a");
            RegexAssert.IsMatch(WikiRegexes.DayMonth, @"On 11&nbsp;July a");
            RegexAssert.IsMatch(WikiRegexes.DayMonth, @"On 11–12 July a");
            RegexAssert.IsMatch(WikiRegexes.DayMonth, @"On 11 July,  1809 a");
            
            RegexAssert.NoMatch(WikiRegexes.DayMonth, @"In 07 July, a");
            RegexAssert.NoMatch(WikiRegexes.DayMonth, @"In July 27, a");
        }
        
        [Test]
        public void MonthDayRangeSpan()
        {
            RegexAssert.IsMatch(WikiRegexes.MonthDayRangeSpan, @"On July 11–12 a");
            RegexAssert.IsMatch(WikiRegexes.MonthDayRangeSpan, @"On July 11–12, 2004 a");
            RegexAssert.IsMatch(WikiRegexes.MonthDayRangeSpan, @"On July 11–12 2004 a");
            RegexAssert.IsMatch(WikiRegexes.MonthDayRangeSpan, @"On July 11&ndash;12 a");
            RegexAssert.IsMatch(WikiRegexes.MonthDayRangeSpan, @"On July 11{{ndash}}12 a");
            RegexAssert.IsMatch(WikiRegexes.MonthDayRangeSpan, @"On July 11/12 a");
            
            RegexAssert.NoMatch(WikiRegexes.MonthDayRangeSpan, @"On July 11-12 a");
            RegexAssert.NoMatch(WikiRegexes.MonthDayRangeSpan, @"On July 1112 a");
            RegexAssert.NoMatch(WikiRegexes.MonthDayRangeSpan, @"On July 11 12 a");
            RegexAssert.NoMatch(WikiRegexes.MonthDayRangeSpan, @"On 11–12 July a");
        }
        
        [Test]
        public void DayMonthRangeSpan()
        {
            RegexAssert.IsMatch(WikiRegexes.DayMonthRangeSpan, @"On 11–12 July a");
            RegexAssert.IsMatch(WikiRegexes.DayMonthRangeSpan, @"On 11–12 July, 2004 a");
            RegexAssert.IsMatch(WikiRegexes.DayMonthRangeSpan, @"On 11&ndash;12 July a");
            RegexAssert.IsMatch(WikiRegexes.DayMonthRangeSpan, @"On 11{{ndash}}12 July a");
            RegexAssert.IsMatch(WikiRegexes.DayMonthRangeSpan, @"On 11/12 July a");
            
            RegexAssert.NoMatch(WikiRegexes.DayMonthRangeSpan, @"On 11-12 July a");
            RegexAssert.NoMatch(WikiRegexes.DayMonthRangeSpan, @"On 1112 July a");
            RegexAssert.NoMatch(WikiRegexes.DayMonthRangeSpan, @"On  11 12 July a");
            RegexAssert.NoMatch(WikiRegexes.DayMonthRangeSpan, @"On July 11–12 a");
        }
        
        [Test]
        public void LinkFGAs()
        {
            ClassicAssert.IsTrue(WikiRegexes.LinkFGAs.IsMatch(@"foo {{Link FA|ar}}"));
            ClassicAssert.IsTrue(WikiRegexes.LinkFGAs.IsMatch(@"foo {{Link GA|ar}}"));
            ClassicAssert.IsTrue(WikiRegexes.LinkFGAs.IsMatch(@"foo {{link FA|ar}}"));
        }
    }
}