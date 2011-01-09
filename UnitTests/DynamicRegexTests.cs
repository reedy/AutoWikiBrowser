using WikiFunctions;
using NUnit.Framework;
using System.Text.RegularExpressions;

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
            RegexAssert.IsMatch(WikiRegexes.Category, "[[Category:Test]]");
            RegexAssert.IsMatch(WikiRegexes.Category, "[[Category:Test|{{PAGENAME}}]]");
            RegexAssert.IsMatch(WikiRegexes.Category, "[[Category:Test|{{subst:PAGENAME}}]]");
            RegexAssert.IsMatch(WikiRegexes.Category, "[[Category:Test now]]");
            RegexAssert.IsMatch(WikiRegexes.Category, "[[Category:Test|Key]]");
            RegexAssert.IsMatch(WikiRegexes.Category, "[[ Category : Test now| Key]]");
            RegexAssert.IsMatch(WikiRegexes.Category, "[[CATEGORY :Test]]");
            RegexAssert.IsMatch(WikiRegexes.Category, "[[Category:_Test]]");
            RegexAssert.IsMatch(WikiRegexes.Category, "[[_Category:Test]]");
            RegexAssert.IsMatch(WikiRegexes.Category, "[[_Category:Test_]]");

            RegexAssert.NoMatch(WikiRegexes.Category, "[[Test]]");
            RegexAssert.NoMatch(WikiRegexes.Category, "[[Image:Test.jpg]]");
            RegexAssert.NoMatch(WikiRegexes.Category, @"[[Category:
1910 births]]");
            RegexAssert.NoMatch(WikiRegexes.Category, @"[[Category:1910 births
]]");
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
            RegexAssert.IsMatch(WikiRegexes.Images, "[[File:Test_of_the.ogg]]");
            RegexAssert.IsMatch(WikiRegexes.Images, "[[Image:Test.JPG]]");
            RegexAssert.IsMatch(WikiRegexes.Images, "[[Image:Test here.png|right|200px|Some description [[here]] or there]]");
            RegexAssert.IsMatch(WikiRegexes.Images, @"[[Image:Test here.png|right|200px|Some description [[here]] or there
 over lines]]");
            RegexAssert.IsMatch(WikiRegexes.Images, "[[File:Test.JPG");
            
            RegexAssert.NoMatch(WikiRegexes.Images, "[[File Test.JPG]]");
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
            
            RegexAssert.NoMatch(WikiRegexes.Images, @"{{Infobox foo|
  bar = a|
  map = Image without exension
  | there=here}}");
            
            RegexAssert.Matches(WikiRegexes.Images, @"{{Infobox foo|
  bar = a|
  picture = Test.JPG
  | there=here}}", @" Test.JPG");
        }
        
        [Test]
        public void ImageTestsGallery()
        {
            RegexAssert.IsMatch(WikiRegexes.Images, @"<gallery>
Test.JPG
Foo.JPEG
</gallery>");
            RegexAssert.IsMatch(WikiRegexes.Images, @"<Gallery>
Test.JPG
Foo.JPEG
</Gallery>");
            RegexAssert.IsMatch(WikiRegexes.Images, @"<Gallery foo=bar>
Test.JPG
Foo.JPEG
</Gallery>");
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
            
            Assert.AreEqual(WikiRegexes.Images.Match(@"<gallery>
Test.JPG
</gallery>").Value, @"<gallery>
Test.JPG
</gallery>");
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
        }

        public void StubTests()
        {
            RegexAssert.IsMatch(WikiRegexes.Stub, @"{{footballer-bio-stub}}");
            RegexAssert.IsMatch(WikiRegexes.Stub, @"{{ footballer-bio-stub}}");
            RegexAssert.IsMatch(WikiRegexes.Stub, @"{{ Footballer-Bio-Stub}}");
            RegexAssert.IsMatch(WikiRegexes.Stub, @"{{bio-stub}}");
            RegexAssert.IsMatch(WikiRegexes.Stub, @"{{stub}}");
            RegexAssert.IsMatch(WikiRegexes.Stub, @"{{ stub}}");
            RegexAssert.IsMatch(WikiRegexes.Stub, @"{{Stub}}");
            RegexAssert.IsMatch(WikiRegexes.Stub, @"{{-stub}}");
            
            RegexAssert.NoMatch(WikiRegexes.Stub, @"{{now stubborn}}");
            RegexAssert.NoMatch(WikiRegexes.Stub, @"{{stubby}}");
        }

        [Test]
        public void PossiblyCommentedStubTests()
        {
            RegexAssert.IsMatch(WikiRegexes.PossiblyCommentedStub, @"{{footballer-bio-stub}}");
            RegexAssert.IsMatch(WikiRegexes.PossiblyCommentedStub, @"{{ footballer-bio-stub}}");
            RegexAssert.IsMatch(WikiRegexes.PossiblyCommentedStub, @"{{ Footballer-Bio-Stub}}");
            RegexAssert.IsMatch(WikiRegexes.PossiblyCommentedStub, @"{{bio-stub}}");
            RegexAssert.IsMatch(WikiRegexes.PossiblyCommentedStub, @"{{stub}}");
            RegexAssert.IsMatch(WikiRegexes.PossiblyCommentedStub, @"{{ stub}}");
            RegexAssert.IsMatch(WikiRegexes.PossiblyCommentedStub, @"{{Stub}}");
            RegexAssert.IsMatch(WikiRegexes.PossiblyCommentedStub, @"<!--{{Stub}}-->");
            
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
            
            RegexAssert.NoMatch(WikiRegexes.Redirect, @"#  REDIRECT:[[Foo]]");
        }
        
        [Test]
        public void OrphanTests()
        {
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(@"{{orphan}}"));
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(@"{{Orphan}}"));
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(@"{{orphan|date=May 2008}}"));
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(@"{{Orphan| date = May 2008}}"));
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(@"{{orphan|date=subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"));
            
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(@"{{orphanblahblah}}"));
            
            #if DEBUG
            Variables.SetProjectLangCode("sv");
            WikiRegexes.MakeLangSpecificRegexes();
            
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(@"{{orphan}}"));
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(@"{{föräldralös}}"));
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(@"{{Föräldralös}}"));
            
            Variables.SetProjectLangCode("ru");
            WikiRegexes.MakeLangSpecificRegexes();
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(@"{{orphan}}"));
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(@"{{изолированная статья}}"));
            
            Variables.SetProjectLangCode("en");
            WikiRegexes.MakeLangSpecificRegexes();
            #endif
        }
        
            [Test]
        public void WikifyTests()
        {
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(@"{{wikify}}"));
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(@"{{wikify|date=March 2009}}"));
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(@"{{Wikify}}"));
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(@"{{Wikify|date=March 2009}}"));
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(@"{{wikify|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"));
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(@"{{Article issues|wikify=May 2008|a=b|c=d}}"));
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(@"{{Articleissues|wikify=May 2008|a=b|c=d}}"));
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(@"{{articleissues|wikify=May 2008|a=b|c=d}}"));
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(@"{{article issues|wikify=May 2008|a=b|c=d}}"));
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(@"{{multiple issues|wikify=May 2008|a=b|c=d}}"));
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(@"{{multiple issues|wikify={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|a=b|c=d}}"));
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(@"{{multiple issues|a=b|c=d| wikify={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|a=b|c=d}}"));
            Assert.AreEqual(WikiRegexes.Wikify.Replace(@"{{multiple issues|a=b|c=d|wikify={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|a=b|c=d}}", "$1"), @"{{multiple issues|a=b|c=d|a=b|c=d}}");
            Assert.AreEqual(WikiRegexes.Wikify.Replace(@"{{multiple issues|a=b|c=d|wikify={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}", "$1"), @"{{multiple issues|a=b|c=d}}");
            Assert.AreEqual(WikiRegexes.Wikify.Replace(@"{{multiple issues|a=b|c=d|wikify = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}", "$1"), @"{{multiple issues|a=b|c=d}}");
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(@"{{Article issues|a=b|c=d| wikify = May 2008|a=b|c=d}}"));

            // don't remove the whole of an {{article issues}} template if removing wikify tag
            Assert.IsTrue(WikiRegexes.Wikify.Replace(@"{{Article issues|a=b|c=d| wikify = May 2008|a=b|c=d}}", "$1").Contains(@"{{Article issues|a=b|c=d|"));

            Assert.IsFalse(WikiRegexes.Wikify.IsMatch(@"{{wikifyworldblah}}"));            
            
            #if DEBUG
            Variables.SetProjectLangCode("sv");
            WikiRegexes.MakeLangSpecificRegexes();
            
            Assert.IsFalse(WikiRegexes.Wikify.IsMatch(@"{{wikify}}"));
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(@"{{ickewiki}}"));
            
            Variables.SetProjectLangCode("en");
            WikiRegexes.MakeLangSpecificRegexes();
            #endif
        }
        
        [Test]
        public void DateYearMonthParameterTests()
        {
            #if DEBUG
            Variables.SetProjectLangCode("sv");
            WikiRegexes.MakeLangSpecificRegexes();
            
            Assert.AreEqual(WikiRegexes.DateYearMonthParameter, @"datum={{subst:CURRENTYEAR}}-{{subst:CURRENTMONTH}}");
            
            Variables.SetProjectLangCode("fr");
            WikiRegexes.MakeLangSpecificRegexes();
            
            Assert.AreEqual(WikiRegexes.DateYearMonthParameter, @"date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}");
            
            Variables.SetProjectLangCode("en");
            WikiRegexes.MakeLangSpecificRegexes();
            #endif
            Assert.AreEqual(WikiRegexes.DateYearMonthParameter, @"date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}");
        }
        
        [Test]
        public void UncatTests()
        {
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Uncategorized}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Uncategorised}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{uncategorized}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{uncategorised}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{uncat}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Uncat}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Uncat|date=January 2009}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{uncategorised|date=May 2008}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Uncategorised|date=May 2008}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Uncategorisedstub|date=May 2008}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Uncategorisedstub|date = May 2008}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{uncategorised|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"));
            
            Assert.IsTrue(WikiRegexes.Uncat.Match(@"{{Uncat}}").Groups[1].Value.Equals("Uncat"));

            // all the other redirects
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Classify}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{CatNeeded}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Catneeded}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Uncategorised}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Uncat}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Categorize}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Categories needed}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Categoryneeded}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Category needed}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Category requested}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Categories requested}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Nocats}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Categorise}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Nocat}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Uncat-date}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Uncategorized-date}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Needs cat}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Needs cats}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Cats needed}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Cat needed}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{classify}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{catneeded}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{catneeded}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{uncategorised}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{uncat}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{categorize}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{categories needed}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{categoryneeded}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{category needed}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{category requested}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{categories requested}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{nocats}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{categorise}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{nocat}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{uncat-date}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{uncategorized-date}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{needs cat}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{needs cats}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{cats needed}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{cat needed}}"));
            
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{uncategorizedstub}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{uncategorized stub}}"));

            // no match
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(@"{{Uncategorized other template}}"));
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(@"{{Uncategorized other template|foo=bar}}"));
            
            // language variation
            #if DEBUG
            Variables.SetProjectLangCode("sv");
            WikiRegexes.MakeLangSpecificRegexes();
            
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{okategoriserad}}"));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(@"{{Okategoriserad}}"));
            
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
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Numberdis}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{numberdis}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Letter-NumberCombdisambig}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Letter-NumberCombDisambig}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{hndis}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{  disambig}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Template:disambig}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{template:disambig}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{LatinNameDisambig}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{SpeciesLatinNameDisambig}}");
            RegexAssert.IsMatch(WikiRegexes.Disambigs, @"{{Hurricane season disambiguation}}");
            
            RegexAssert.NoMatch(WikiRegexes.Disambigs, @"{{now disambig}}");
            RegexAssert.NoMatch(WikiRegexes.Disambigs, @"{{dablink|foo}}");
        }

        [Test]
        public void SIAsTests()
        {
            RegexAssert.IsMatch(WikiRegexes.SIAs, @"{{surname}}");
               RegexAssert.IsMatch(WikiRegexes.SIAs, @"{{given name}}");
            RegexAssert.IsMatch(WikiRegexes.SIAs, @"{{Shipindex}}");
            RegexAssert.IsMatch(WikiRegexes.SIAs, @"{{mountainindex}}");
            RegexAssert.IsMatch(WikiRegexes.SIAs, @"{{Roadindex}}");
            RegexAssert.IsMatch(WikiRegexes.SIAs, @"{{SIA}}");
            RegexAssert.IsMatch(WikiRegexes.SIAs, @"{{sportindex}}");
            RegexAssert.NoMatch(WikiRegexes.SIAs, @"{{surname-stub}}");
        }

            [Test]
        public void ExtractTitleTests()
        {
            RegexAssert.IsMatch(WikiRegexes.ExtractTitle, @"http://en.wikipedia.org/wiki/Foo");
            RegexAssert.IsMatch(WikiRegexes.ExtractTitle, @"http://en.wikipedia.org/wiki/Foo_bar");
            
            Assert.AreEqual(WikiRegexes.ExtractTitle.Match(@"http://en.wikipedia.org/wiki/Foo").Groups[1].Value, "Foo");
            Assert.AreEqual(WikiRegexes.ExtractTitle.Match(@"http://en.wikipedia.org/w/index.php?title=Foo").Groups[1].Value, "Foo");
            Assert.AreEqual(WikiRegexes.ExtractTitle.Match(@"http://en.wikipedia.org/w/index.php/Foo").Groups[1].Value, "Foo");
            Assert.AreEqual(WikiRegexes.ExtractTitle.Match(@"http://en.wikipedia.org/w/index.php/Foo bar here").Groups[1].Value, "Foo bar here");
            
            RegexAssert.NoMatch(WikiRegexes.ExtractTitle, @"http://random.org/wiki/Foo");
            RegexAssert.NoMatch(WikiRegexes.ExtractTitle, @"http://en.wikipedia.org/wikirandom/Foo");
        }

        [Test]
        public void EmptyLinkTests()
        {
            RegexAssert.IsMatch(WikiRegexes.EmptyLink, "[[]]");
            RegexAssert.IsMatch(WikiRegexes.EmptyLink, "[[   ]]");
            RegexAssert.IsMatch(WikiRegexes.EmptyLink, "[[|]]");
            RegexAssert.IsMatch(WikiRegexes.EmptyLink, "[[       |    ]]");
            RegexAssert.IsMatch(WikiRegexes.EmptyLink, "[[Category:]]");
            RegexAssert.IsMatch(WikiRegexes.EmptyLink, "[[Image:]]");
            RegexAssert.IsMatch(WikiRegexes.EmptyLink, "[[File:]]");
        }

        [Test]
        public void EmptyTemplateTests()
        {
            RegexAssert.NoMatch(WikiRegexes.EmptyTemplate, "{{TemplateTest}}");
            RegexAssert.NoMatch(WikiRegexes.EmptyTemplate, "{{Test}}");
            RegexAssert.NoMatch(WikiRegexes.EmptyTemplate, "{{Test|Parameter}}");
            RegexAssert.NoMatch(WikiRegexes.EmptyTemplate, "{{Test|}}");

            RegexAssert.IsMatch(WikiRegexes.EmptyTemplate, "{{Template:}}");
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
            Assert.AreEqual("January", mo.Match(@"in January there").Groups[1].Value);

            Regex mong = new Regex(WikiRegexes.MonthsNoGroup);
            Assert.AreEqual("", mong.Match(@"in January there").Groups[1].Value);
        }

        [Test]
        public void DefaultsortTests()
        {
            RegexAssert.IsMatch(WikiRegexes.Defaultsort, "{{DEFAULTSORT:foo}}");
            RegexAssert.IsMatch(WikiRegexes.Defaultsort, "{{DEFAULTSORT:foo bar}}");

            RegexAssert.IsMatch(WikiRegexes.Defaultsort, @"{{DEFAULTSORT:foo
");
            RegexAssert.NoMatch(WikiRegexes.Defaultsort, @"{{DEFAULTSORT:foo");
            
            Assert.AreEqual("foo", WikiRegexes.Defaultsort.Match(@"{{DEFAULTSORT:foo}}").Groups["key"].Value);
            Assert.AreEqual(@"{{PAGENAME}}", WikiRegexes.Defaultsort.Match(@"{{DEFAULTSORT:{{PAGENAME}}}}").Groups["key"].Value);

            Assert.AreEqual("{{DEFAULTSORT:foo}}", WikiRegexes.Defaultsort.Match(@"{{DEFAULTSORT:foo}}").Value);
            Assert.AreEqual("{{DEFAULTSORT:foo\r", WikiRegexes.Defaultsort.Match(@"{{DEFAULTSORT:foo
now").Value);
            
            Assert.AreEqual(@"{{DEFAULTSORT:{{PAGENAME}}}}", WikiRegexes.Defaultsort.Match(@"{{DEFAULTSORT:{{PAGENAME}}}}").Value);
            
            Assert.AreEqual(@"{{DEFAULTSORT:foo]]" + "\r", WikiRegexes.Defaultsort.Match(@"{{DEFAULTSORT:foo]]
pp").Value);
             Assert.AreEqual(@"{{DEFAULTSORT:foo]]" + "\r", WikiRegexes.Defaultsort.Match(@"{{DEFAULTSORT:foo]]
pp
{{x}}
").Value);
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
            Assert.IsTrue(WikiRegexes.LinkFGAs.IsMatch(@"foo {{Link FA|ar}}"));
            Assert.IsTrue(WikiRegexes.LinkFGAs.IsMatch(@"foo {{Link GA|ar}}"));
            Assert.IsTrue(WikiRegexes.LinkFGAs.IsMatch(@"foo {{link FA|ar}}"));
            Assert.IsTrue(WikiRegexes.LinkFGAs.IsMatch(@"foo {{Link FL|ar}}"));
        }

        [Test]
        public void LinkFGAsArabic()
        {
            #if DEBUG
            Variables.SetProjectLangCode("ar");
            WikiRegexes.MakeLangSpecificRegexes();
            
            Assert.IsTrue(WikiRegexes.LinkFGAs.IsMatch(@"foo {{وصلة مقالة مختارة|he}}"));
            Assert.IsTrue(WikiRegexes.LinkFGAs.IsMatch(@"foo {{وصلة مقالة مختارة|
he}}"));
            Assert.IsTrue(WikiRegexes.LinkFGAs.IsMatch(@"foo {{وصلة مقالة مختارة
|he}}"));
            
            Variables.SetProjectLangCode("en");
            WikiRegexes.MakeLangSpecificRegexes();
            #endif
        }

        [Test]
        public void LinkFGAsCatalan()
        {
            #if DEBUG
            Variables.SetProjectLangCode("ca");
            WikiRegexes.MakeLangSpecificRegexes();

            Assert.IsTrue(WikiRegexes.LinkFGAs.IsMatch(@"foo {{link FA|ar}}"));
            Assert.IsTrue(WikiRegexes.LinkFGAs.IsMatch(@"foo {{Enllaç AD|ar}}"));
            
            Variables.SetProjectLangCode("en");
            WikiRegexes.MakeLangSpecificRegexes();
            #endif
        }

        [Test]
        public void LinkFGAsFrench()
        {
            #if DEBUG
            Variables.SetProjectLangCode("fr");
            WikiRegexes.MakeLangSpecificRegexes();

            Assert.IsTrue(WikiRegexes.LinkFGAs.IsMatch(@"foo {{lien BA|ar}}"));
            Assert.IsTrue(WikiRegexes.LinkFGAs.IsMatch(@"foo {{Lien AdQ|ar}}"));
            Assert.IsTrue(WikiRegexes.LinkFGAs.IsMatch(@"foo {{lien PdQ|ar}}"));
            
            Variables.SetProjectLangCode("en");
            WikiRegexes.MakeLangSpecificRegexes();
            #endif
        }

        [Test]
        public void LinkFGAsItalian()
        {
            #if DEBUG
            Variables.SetProjectLangCode("it");
            WikiRegexes.MakeLangSpecificRegexes();

            Assert.IsTrue(WikiRegexes.LinkFGAs.IsMatch(@"foo {{link FA|ar}}"));
            Assert.IsTrue(WikiRegexes.LinkFGAs.IsMatch(@"foo {{Link AdQ|ar}}"));
            
            Variables.SetProjectLangCode("en");
            WikiRegexes.MakeLangSpecificRegexes();
            #endif
        }

        [Test]
        public void LinkFGAsPortuguese()
        {
            #if DEBUG
            Variables.SetProjectLangCode("pt");
            WikiRegexes.MakeLangSpecificRegexes();

            Assert.IsTrue(WikiRegexes.LinkFGAs.IsMatch(@"foo {{link GA|ar}}"));
            Assert.IsTrue(WikiRegexes.LinkFGAs.IsMatch(@"foo {{Bom interwiki|ar}}"));
            Assert.IsTrue(WikiRegexes.LinkFGAs.IsMatch(@"foo {{Link FA|ar}}"));
            Assert.IsTrue(WikiRegexes.LinkFGAs.IsMatch(@"foo {{FA|ar}}"));
            Assert.IsTrue(WikiRegexes.LinkFGAs.IsMatch(@"foo {{Interwiki destacado|ar}}"));
            
            Assert.IsFalse(WikiRegexes.LinkFGAs.IsMatch(@"foo {{foo |pub=FA|text=a}}"));
            
            Variables.SetProjectLangCode("en");
            WikiRegexes.MakeLangSpecificRegexes();
            #endif
        }

        [Test]
        public void LinkFGAsSpanish()
        {
            #if DEBUG
            Variables.SetProjectLangCode("es");
            WikiRegexes.MakeLangSpecificRegexes();

            Assert.IsTrue(WikiRegexes.LinkFGAs.IsMatch(@"foo {{link FA|ar}}"));
            Assert.IsTrue(WikiRegexes.LinkFGAs.IsMatch(@"foo {{Destacado|ar}}"));
            Assert.IsTrue(WikiRegexes.LinkFGAs.IsMatch(@"foo {{Bueno|el}}"));
            Assert.IsTrue(WikiRegexes.LinkFGAs.IsMatch(@"foo {{bueno|el}}"));
            
            Variables.SetProjectLangCode("en");
            WikiRegexes.MakeLangSpecificRegexes();
            #endif
        }
		
		[Test]
		public void LinkFGAsSwedish()
        {
            #if DEBUG
            Variables.SetProjectLangCode("sv");
            WikiRegexes.MakeLangSpecificRegexes();

            Assert.IsTrue(WikiRegexes.LinkFGAs.IsMatch(@"foo {{link GA|ar}}"));
            Assert.IsTrue(WikiRegexes.LinkFGAs.IsMatch(@"foo {{GA|ar}}"));
            Assert.IsTrue(WikiRegexes.LinkFGAs.IsMatch(@"foo {{Link FA|ar}}"));
            Assert.IsTrue(WikiRegexes.LinkFGAs.IsMatch(@"foo {{Länk UA|ar}}"));
            Assert.IsTrue(WikiRegexes.LinkFGAs.IsMatch(@"foo {{lank UA|ar}}"));
            Assert.IsTrue(WikiRegexes.LinkFGAs.IsMatch(@"foo {{UA|ar}}"));

            Variables.SetProjectLangCode("en");
            WikiRegexes.MakeLangSpecificRegexes();
            #endif
        }

    }
}