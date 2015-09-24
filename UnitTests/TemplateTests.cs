using System.Collections.Generic;
using NUnit.Framework;
using WikiFunctions.Parse;

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

        [Test]
        public void GetAllTemplateDetail()
        {
            List<string> t = new List<string>();

            Assert.AreEqual(t, Parsers.GetAllTemplateDetail(""));
            t.Add("{{foo}}");
            Assert.AreEqual(t, Parsers.GetAllTemplateDetail("{{foo}}"), "Extracts simple template call");
            Assert.AreEqual(t, Parsers.GetAllTemplateDetail("{{foo}} {{foo}}"), "Deduplicates");
            t.Add("{{foo2}}");
            Assert.AreEqual(t, Parsers.GetAllTemplateDetail("{{foo}} {{foo2}}"), "Extracts each distinct template call");
            t.Clear();
            t.Add("{{foo|param={{foo2}} {{foo3=|bar3={{foo4}}}}}}");
            t.Add("{{foo2}}");
            t.Add("{{foo3=|bar3={{foo4}}}}");
            t.Add("{{foo4}}");
            Assert.AreEqual(t, Parsers.GetAllTemplateDetail("{{foo|param={{foo2}} {{foo3=|bar3={{foo4}}}}}}"), "Extracts nested template calls");
        }
    }
}
