using System.Text.RegularExpressions;
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
            CatTests(WikiRegexes.Category);
        }

        [Test]
        public void CatRegexTests()
        {
            CatTests(WikiRegexes.CatRegex);
        }

        public void CatTests(Regex reg)
        {
            RegexAssert.IsMatch(reg, "[[Category:Test]]");
        }
    }
}