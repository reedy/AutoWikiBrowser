using WikiFunctions;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace UnitTests
{
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class DynamicRegexTests : RequiresInitialization
    {
        [Test]
        public void CategoryTests()
        {
            RegexAssert.IsMatch(WikiRegexes.Category, "[[Category:Test]]");
            RegexAssert.IsMatch(WikiRegexes.Category, "[[Category:Test now]]");
            RegexAssert.IsMatch(WikiRegexes.Category, "[[Category:Test|Key]]");
            RegexAssert.IsMatch(WikiRegexes.Category, "[[ Category : Test now| Key]]");

            RegexAssert.NoMatch(WikiRegexes.Category, "[[Test]]");
            RegexAssert.NoMatch(WikiRegexes.Category, "[[Image:Test.jpg]]");
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
  cover = [[File:Test.JPG]]
  | there=here}}");
            RegexAssert.IsMatch(WikiRegexes.Images, @"{{Infobox foo|
  bar = a|
  cover art = [[File:Test.JPG]]
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
            
            RegexAssert.NoMatch(WikiRegexes.Stub, @"{{now stubborn}}");
            RegexAssert.NoMatch(WikiRegexes.Stub, @"{{stubby}}");
        }

        [Test, Ignore("Incomplete")]
        public void PossiblyCommentedStubTests()
        {
        }

        [Test, Ignore("Incomplete")]
        public void TemplateCallTests()
        {
        }

        [Test, Ignore("Incomplete")]
        public void LooseCategoryTests()
        {
        }

        [Test, Ignore("Incomplete")]
        public void LooseImageTests()
        {
        }

        [Test, Ignore("Incomplete")]
        public void DatesTests()
        {
        }

        [Test, Ignore("Incomplete")]
        public void Dates2Tests()
        {
        }

        [Test]
        public void RedirectTests()
        {
            RegexAssert.IsMatch(WikiRegexes.Redirect, @"#REDIRECT:[[Foo]]");
            RegexAssert.IsMatch(WikiRegexes.Redirect, @"#redirect:[[Foo]]");
            RegexAssert.IsMatch(WikiRegexes.Redirect, @"#REDIRECT [[Foo]]");
            RegexAssert.IsMatch(WikiRegexes.Redirect, @"#REDIRECT   :   [[Foo]]");
            RegexAssert.IsMatch(WikiRegexes.Redirect, @"#REDIRECT:[[Foo bar]]");
            RegexAssert.IsMatch(WikiRegexes.Redirect, @"#REDIRECT:[[ Foo bar]]");
            RegexAssert.IsMatch(WikiRegexes.Redirect, @"#REDIRECT:[[ :Foo bar]]");
            RegexAssert.IsMatch(WikiRegexes.Redirect, @"#REDIRECT:[[Foo bar|the best]]");
            RegexAssert.IsMatch(WikiRegexes.Redirect, @"#REDIRECT:[[Foo bar#best]]");
            
            RegexAssert.NoMatch(WikiRegexes.Redirect, @"#  REDIRECT:[[Foo]]");
        }

        [Test, Ignore("Incomplete")]
        public void DisambigsTests()
        {
        }

        [Test, Ignore("Incomplete")]
        public void ExtractTitleTests()
        {
        }

        [Test, Ignore("Incomplete")]
        // add tests for empty category and Image
        public void EmptyLinkTests()
        {
            RegexAssert.IsMatch(WikiRegexes.EmptyLink, "[[]]");
            RegexAssert.IsMatch(WikiRegexes.EmptyLink, "[[   ]]");
            RegexAssert.IsMatch(WikiRegexes.EmptyLink, "[[|]]");
            RegexAssert.IsMatch(WikiRegexes.EmptyLink, "[[       |    ]]");
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


            Assert.AreEqual("{{DEFAULTSORT:foo}}", WikiRegexes.Defaultsort.Match(@"{{DEFAULTSORT:foo}}").Value);
            Assert.AreEqual("{{DEFAULTSORT:foo\r", WikiRegexes.Defaultsort.Match(@"{{DEFAULTSORT:foo
now").Value);

        }
    }
}