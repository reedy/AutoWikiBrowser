using WikiFunctions;
using NUnit.Framework;

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
            RegexAssert.IsMatch(WikiRegexes.Category, "[[Category:Test|Key]]");

            RegexAssert.NoMatch(WikiRegexes.Category, "[[Test]]");
            RegexAssert.NoMatch(WikiRegexes.Category, "[[Image:Test.jpg]]");
        }

        [Test, Ignore("Incomplete")]
        public void ImageTests()
        {
        }

        [Test, Ignore("Incomplete")]
        public void StubTests()
        {
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

        [Test, Ignore("Incomplete")]
        public void RedirectTests()
        {
        }

        [Test, Ignore("Incomplete")]
        public void DisambigsTests()
        {
        }

        [Test, Ignore("Incomplete")]
        public void DefaultsortTests()
        {
        }

        [Test, Ignore("Incomplete")]
        public void ExtractTitleTests()
        {
        }

        [Test, Ignore("Incomplete")]
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
    }
}