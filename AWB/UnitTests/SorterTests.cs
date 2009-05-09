using WikiFunctions.Parse;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace UnitTests
{
    public class RequiresParser2 : RequiresInitialization
    {
        protected Parsers parser2;

        public RequiresParser2()
        {
            parser2 = new Parsers();
        }
    }

    [TestFixture]
    public class SorterTests : RequiresParser2
    {
        //public SorterTests()
        //{
        //Variables.SetToEnglish();
        //}

        [Test]
        public void RemoveStubs()
        {
            // shouldn't break anything
            string s = "==foo==\r\nbar<ref name=\"123\"/>{{boz}}";
            Assert.AreEqual("", MetaDataSorter.RemoveStubs(ref s));
            Assert.AreEqual("==foo==\r\nbar<ref name=\"123\"/>{{boz}}", s);

            // should remove stubs, but not section stubs
            s = "{{foo}}{{stub}}{{foo-stub}}bar{{sect-stub}}{{not-a-stub|123}}{{not a|stub}}";
            Assert.AreEqual("{{stub}}\r\n{{foo-stub}}\r\n", MetaDataSorter.RemoveStubs(ref s));
            Assert.AreEqual("{{foo}}bar{{sect-stub}}{{not-a-stub|123}}{{not a|stub}}", s);

            //shouldn't fail
            s = "";
            Assert.AreEqual("", MetaDataSorter.RemoveStubs(ref s));
            Assert.AreEqual("", s);
            s = "{{stub}}";
            Assert.AreEqual("{{stub}}\r\n", MetaDataSorter.RemoveStubs(ref s));
            Assert.AreEqual("", s);
        }

        [Test]
        public void MoveDablinksTests()
        {
            const string d = @"Fred is a doctor.
Fred has a dog.
[[Category:Dog owners]]
{{some template}}
";

            string e = @"{{otherpeople1|Fred the dancer|Fred Smith (dancer)}}";
            Assert.AreEqual(e + "\r\n" + d, MetaDataSorter.MoveDablinks(d + e));

            e = @"{{For|Fred the dancer|Fred Smith (dancer)}}";
            Assert.AreEqual(e + "\r\n" + d, MetaDataSorter.MoveDablinks(d + e));

            e = @"{{redirect2|Fred the dancer|Fred Smith (dancer)}}";
            Assert.AreEqual(e + "\r\n" + d, MetaDataSorter.MoveDablinks(d + e));

            e = @"{{redirect2|Fred the {{dancer}}|Fred Smith (dancer)}}";
            Assert.AreEqual(e + "\r\n" + d, MetaDataSorter.MoveDablinks(d + e));

            // check no change when already in correct position
            Assert.AreEqual(e + "\r\n" + d, MetaDataSorter.MoveDablinks(e + "\r\n" + d));

            // don't move dablinks in a section
            string f = @"Article words
== heading ==
{{redirect2|Fred the dancer|Fred Smith (dancer)}}
words";
            Assert.AreEqual(f, MetaDataSorter.MoveDablinks(f));

        }

        [Test]
        public void MovePortalTemplatesTests()
        {
         Assert.AreEqual(@"text here
text here2
== see also ==
{{Portal|Football}}
some words", MetaDataSorter.MovePortalTemplates(@"text here
{{Portal|Football}}
text here2
== see also ==
some words"));

         Assert.AreEqual(@"text here
text here2
== see also ==
{{Portal|Sport}}
{{Portal|Football}}
some words", MetaDataSorter.MovePortalTemplates(@"text here
{{Portal|Football}}
{{Portal|Sport}}
text here2
== see also ==
some words"));

          Assert.AreEqual(@"text here
text here2
== see also ==
{{Portal|Football}}
some words", MetaDataSorter.MovePortalTemplates(@"{{Portal|Football}}
text here
text here2
== see also ==
some words"));

            Assert.AreEqual(@"text here
text here2
== see also==
{{Portal|abc}}
* Fred", MetaDataSorter.MovePortalTemplates(@"text here
{{Portal|abc}}
text here2
== see also==
* Fred"));

            Assert.AreEqual(@"text here
text here2
== see also ==
{{Portal|Football}}
* Fred
== hello ==
some words", MetaDataSorter.MovePortalTemplates(@"text here
{{Portal|Football}}
text here2
== see also ==
* Fred
== hello ==
some words"));

            Assert.AreEqual(@"text here
text here2
== see also ==
{{Portal|Football}}
* Fred
=== hello ===
some words", MetaDataSorter.MovePortalTemplates(@"text here
{{Portal|Football}}
text here2
== see also ==
* Fred
=== hello ===
some words"));

            // if portal is already in 'see also', don't move it
            Assert.AreEqual(@"text here
text here2
== see also ==
{{Portal|Football}}
some words", MetaDataSorter.MovePortalTemplates(@"text here
text here2
== see also ==
{{Portal|Football}}
some words"));

            Assert.AreEqual(@"text here
text here2
== see also ==
* Fred
{{Portal|Football}}
some words", MetaDataSorter.MovePortalTemplates(@"text here
text here2
== see also ==
* Fred
{{Portal|Football}}
some words"));

            Assert.AreEqual(@"text here
text here2
== see also ==
* Fred
=== portals ===
{{Portal|Football}}
some words", MetaDataSorter.MovePortalTemplates(@"text here
text here2
== see also ==
* Fred
=== portals ===
{{Portal|Football}}
some words"));
        }
        
        [Test]
        public void MoveMoreNoFootnotesTests()
        {
          string A = @"{{nofootnotes}}";
          string A2 = @"{{Morefootnotes}}";
          
          string B = @"
'''Article''' words.
== section ==
words.
";
          string C = @"== References ==
";
          string D = @"== Notes ==
";
          string E = @"== Footnotes ==
";

          string F = @"'''Article''' words.
== section ==
{{nofootnotes}}
words.
";  
          string G = @"words";
        
          Assert.AreEqual(B + C + A + "\r\n" + G, MetaDataSorter.MoveMoreNoFootnotes(A + B + C + G));
          Assert.AreEqual(B + D + A + "\r\n" + G, MetaDataSorter.MoveMoreNoFootnotes(A + B + D + G));
          Assert.AreEqual(B + E + A + "\r\n" + G, MetaDataSorter.MoveMoreNoFootnotes(A + B + E + G));
          Assert.AreEqual(B + E + A + "\r\n", MetaDataSorter.MoveMoreNoFootnotes(A + B + E));
          Assert.AreEqual(B + E + A2 + "\r\n", MetaDataSorter.MoveMoreNoFootnotes(A2 + B + E));
        
          // not moved if outside zeroth section
          Assert.AreEqual(F + C + G, MetaDataSorter.MoveMoreNoFootnotes(F + C + G));
          Assert.AreEqual(F + D + G, MetaDataSorter.MoveMoreNoFootnotes(F + D + G));
          Assert.AreEqual(F + E + G, MetaDataSorter.MoveMoreNoFootnotes(F + E + G));
        }

        [Test]
        public void MoveExternalLinksTests()
        {
            string a = @"'''article'''
== blah ==
words<ref>abc</ref>";
            string b = @"== external links ==
* [http://www.site.com a site]";
            string c = @"== References ==
{{reflist}}";
            string d = @"=== another section ===
blah";
            string e = @"[[Category:Foos]]";

            string f = @"{{some footer thing}}";

            string g = @"== another section ==
blah";
            string h = @"{{DEFAULTSORT:Foo, bar}}";

            Assert.AreEqual(a + "\r\n" + c + "\r\n" + b + "\r\n" + e, MetaDataSorter.MoveExternalLinks(a + "\r\n" + b + "\r\n" + c + "\r\n" + e));
            Assert.AreEqual(a + "\r\n" + c + "\r\n" + b + "\r\n" + h, MetaDataSorter.MoveExternalLinks(a + "\r\n" + b + "\r\n" + c + "\r\n" + h));
            Assert.AreEqual(a + "\r\n" + c + "\r\n" + b + "\r\n" + g, MetaDataSorter.MoveExternalLinks(a + "\r\n" + b + "\r\n" + c + "\r\n" + g));

            // no change if already correct
            Assert.AreEqual(a + "\r\n" + c + "\r\n" + b, MetaDataSorter.MoveExternalLinks(a + "\r\n" + c + "\r\n" + b));
            Assert.AreEqual(a + "\r\n" + c + "\r\n" + b + "\r\n" + g, MetaDataSorter.MoveExternalLinks(a + "\r\n" + c + "\r\n" + b + "\r\n" + g));

            // no change if end of references section is unclear
            Assert.AreEqual(a + "\r\n" + b + "\r\n" + c + "\r\n" + f, MetaDataSorter.MoveExternalLinks(a + "\r\n" + b + "\r\n" + c + "\r\n" + f));

            // only matching level two headings following references
            Assert.AreEqual(a + "\r\n" + b + "\r\n" + c + "\r\n" + d, MetaDataSorter.MoveExternalLinks(a + "\r\n" + b + "\r\n" + c + "\r\n" + d));

        }

        // {{Lifetime}} template lives after categories on en-wiki
        [Test]
        public void LifetimeTests()
        {
            string a = @"Fred is a doctor. Fred has a dog.
{{Lifetime|1922|1987|Smith, Fred}}
[[Category:Dog owners]]";
            const string b = @"[[Category:Dog owners]]
{{Lifetime|1922|1987|Smith, Fred}}
";

            Assert.AreEqual(b, parser2.Sorter.RemoveCats(ref a, "test"));

            string c = @"Fred is a doctor. Fred has a dog.
{{lifetime|1922|1987|Smith, Fred}}
[[Category:Dog owners]]
[[Category:Foo]]
[[Category:Bar]]";
            const string d = @"[[Category:Dog owners]]
[[Category:Foo]]
[[Category:Bar]]
{{lifetime|1922|1987|Smith, Fred}}
";

            Assert.AreEqual(d, parser2.Sorter.RemoveCats(ref c, "test"));

            string e = @"Fred is a doctor. Fred has a dog.
{{BIRTH-DEATH-SORT|1922|1987|Smith, Fred}}
[[Category:Dog owners]]
[[Category:Foo]]
[[Category:Bar]]";
            const string f = @"[[Category:Dog owners]]
[[Category:Foo]]
[[Category:Bar]]
{{BIRTH-DEATH-SORT|1922|1987|Smith, Fred}}
";

            Assert.AreEqual(f, parser2.Sorter.RemoveCats(ref e, "test"));

            // normal spacing rules apply for {{lifetime}} 1 for interwikis, two for stubs
            string g = @"{{Maroon 5}}

{{Lifetime|1979||Carmichael, Jesse}}
[[Category:American keyboardists]]
[[Category:Maroon 5]]

";

            string h = @"[[Category:American keyboardists]]
[[Category:Maroon 5]]
{{Lifetime|1979||Carmichael, Jesse}}
";

            Assert.AreEqual(h, parser2.Sorter.RemoveCats(ref g, "test"));
        }
    }
}
