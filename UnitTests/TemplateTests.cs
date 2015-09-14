using WikiFunctions;
using WikiFunctions.Parse;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace UnitTests
{
    [TestFixture, Category("Incomplete")]
    public class TemplateTests : RequiresInitialization
    {
        [Test]
        public void GetAllTemplates()
        {
            List<string> t = new List<string>();

            Assert.AreEqual(t, Parsers.GetAllTemplates(""));
            t.Add("Foo");
            Assert.AreEqual(t, Parsers.GetAllTemplates("{{foo}}"), "case converted to first letter upper");
            Assert.AreEqual(t, Parsers.GetAllTemplates("{{Foo}}"), "uppercase template first letter handled");
            Assert.AreEqual(t, Parsers.GetAllTemplates("{{Foo}} {{foo}}"), "Templates list deduplicated");
            Assert.AreEqual(t, Parsers.GetAllTemplates("{{Foo|bar=1}}"), "Support templates with arguments");
            t.Add("Other");
            Assert.AreEqual(t, Parsers.GetAllTemplates("{{Foo|bar=1 {{Other}} }}"), "Support nested templates");
            t.Add("Other2");
            Assert.AreEqual(t, Parsers.GetAllTemplates("{{Foo|bar=1 {{Other| a= {{Other2|}} }} }}"), "Support 2x nested templates");
            Assert.AreEqual(t, Parsers.GetAllTemplates(@"{{_Foo  |bar=1 {{ Other
| a= {{ Other2_|}} }} }}"), "Ignore whitespace");
            Assert.AreEqual(t, Parsers.GetAllTemplates(@"{{Foo|bar=1 {{Other
| a= {{Template:Other2_|}} }} }}"), "Ignore Template: prefix");
        }
    }
}
