using NUnit.Framework;
using WikiFunctions;

namespace UnitTests
{
    [TestFixture]
    public class TemplateTests
    {
        public TemplateTests()
        {
            Globals.UnitTestMode = true;
            if (WikiRegexes.Category == null) WikiRegexes.MakeLangSpecificRegexes();
        }

        [Test]
        public void SimpleExtraction()
        {
        }
    }
}
