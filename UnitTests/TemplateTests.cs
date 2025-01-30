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

            Assert.That(Parsers.GetAllTemplates(""), Is.EqualTo(t));
            t.Add("Foo");
            Assert.That(Parsers.GetAllTemplates("{{foo}}"), Is.EqualTo(t), "case converted to first letter upper");
            Assert.That(Parsers.GetAllTemplates("{{Foo}}"), Is.EqualTo(t), "uppercase template first letter handled");
            Assert.That(Parsers.GetAllTemplates("{{Foo}} {{foo}}"), Is.EqualTo(t), "Templates list deduplicated");
            Assert.That(Parsers.GetAllTemplates("{{Foo|bar=1}}"), Is.EqualTo(t), "Support templates with arguments");
            t.Add("Other");
            Assert.That(Parsers.GetAllTemplates("{{Foo|bar=1 {{Other}} }}"), Is.EqualTo(t), "Support nested templates");
            t.Add("Other2");
            Assert.That(Parsers.GetAllTemplates("{{Foo|bar=1 {{Other| a= {{Other2|}} }} }}"), Is.EqualTo(t), "Support 2x nested templates");
            Assert.That(Parsers.GetAllTemplates(@"{{_Foo  |bar=1 {{ Other
| a= {{ Other2_|}} }} }}"), Is.EqualTo(t), "Ignore whitespace");
            Assert.That(Parsers.GetAllTemplates(@"{{Foo|bar=1 {{Other
| a= {{Template:Other2_|}} }} }}"), Is.EqualTo(t), "Ignore Template: prefix");

            t.Clear();
            t.Add("DISPLAYTITLE");
            Assert.That(Parsers.GetAllTemplates("{{DISPLAYTITLE:foo}}"), Is.EqualTo(t), "Supports DISPLAYTITLE template");

            t.Clear();
            t.Add("Template");
            Assert.That(Parsers.GetAllTemplates("{{Template:}}"), Is.EqualTo(t), "Supports empty template");
        }

        [Test]
        public void GetAllTemplateDetail()
        {
            List<string> t = new List<string>();

            Assert.That(Parsers.GetAllTemplateDetail(""), Is.EqualTo(t));
            t.Add("{{foo}}");
            Assert.That(Parsers.GetAllTemplateDetail("{{foo}}"), Is.EqualTo(t), "Extracts simple template call");
            Assert.That(Parsers.GetAllTemplateDetail("{{foo}} {{foo}}"), Is.EqualTo(t), "Deduplicates");
            t.Add("{{foo2}}");
            Assert.That(Parsers.GetAllTemplateDetail("{{foo}} {{foo2}}"), Is.EqualTo(t), "Extracts each distinct template call");
            t.Clear();
            t.Add("{{foo|param={{foo2}} {{foo3=|bar3={{foo4}}}}}}");
            t.Add("{{foo2}}");
            t.Add("{{foo3=|bar3={{foo4}}}}");
            t.Add("{{foo4}}");
            Assert.That(Parsers.GetAllTemplateDetail("{{foo|param={{foo2}} {{foo3=|bar3={{foo4}}}}}}"), Is.EqualTo(t), "Extracts nested template calls");

            t.Clear();
            t.Add("{{DISPLAYTITLE:foo}}");
            Assert.That(Parsers.GetAllTemplateDetail("{{DISPLAYTITLE:foo}}"), Is.EqualTo(t), "Supports DISPLAYTITLE template call");
        }

        [Test]
        public void TemplateExistsTests()
        {
            Assert.IsTrue(Parsers.TemplateExists(Parsers.GetAllTemplates(@"{{foo}}"), WikiFunctions.Tools.NestedTemplateRegex("Foo")));
            Assert.IsTrue(Parsers.TemplateExists(Parsers.GetAllTemplates(@"{{Template:foo}}"), WikiFunctions.Tools.NestedTemplateRegex("Foo")));
            Assert.IsTrue(Parsers.TemplateExists(Parsers.GetAllTemplates(@"{{ foo }}"), WikiFunctions.Tools.NestedTemplateRegex("Foo")));
            Assert.IsTrue(Parsers.TemplateExists(Parsers.GetAllTemplates(@"{{foo|p=1}}"), WikiFunctions.Tools.NestedTemplateRegex("Foo")));
            Assert.IsFalse(Parsers.TemplateExists(Parsers.GetAllTemplates(@"{{foo}}"), WikiFunctions.Tools.NestedTemplateRegex("Foo2")));
        }
    }
}
