using WikiFunctions.Parse;
using NUnit.Framework;

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
        public void MovePersonDataTests()
        {
            const string a = @"<!-- Metadata: see [[Wikipedia:Persondata]] -->
";
            string b1 = @"{{Persondata
|NAME= Hodgson, Jane Elizabeth
|ALTERNATIVE NAMES=
|SHORT DESCRIPTION= [[Physician]], [[obstetrician]], [[gynecologist]]
|DATE OF BIRTH= 1915-1-23
|PLACE OF BIRTH= [[Crookston, Minnesota]]
|DATE OF DEATH= 2006-10-23
|PLACE OF DEATH= [[Rochester, Minnesota]]
}}";

            const string b2 = @"{{Persondata
|NAME= Hodgson, Jane Elizabeth
|ALTERNATIVE NAMES=
|SHORT DESCRIPTION= [[Physician]], [[obstetrician]], [[gynecologist]]
|DATE OF BIRTH= {{birth date|1915|1|23}}
|PLACE OF BIRTH= [[Crookston, Minnesota]]
|DATE OF DEATH= 2006-10-23
|PLACE OF DEATH= [[Rochester, Minnesota]]
}}";

            string c1 = a + b1;
            string c2 = a + b2;

            MetaDataSorter.RemovePersonData(ref c1);
            Assert.AreEqual(c1, "");
            MetaDataSorter.RemovePersonData(ref c2);
            Assert.AreEqual(c2, "");
            MetaDataSorter.RemovePersonData(ref b1);
            Assert.AreEqual(b1, "");

            const string d1 = @"{{Persondata<!-- Metadata: see [[Wikipedia:Persondata]] -->
|NAME= Becker, Gary
|ALTERNATIVE NAMES=
|SHORT DESCRIPTION= [[Economics|Economist]]
|DATE OF BIRTH= [[December 2]], [[1930]]
|PLACE OF BIRTH= [[Pottsville, Pennsylvania]]
|DATE OF DEATH=
|PLACE OF DEATH=
}}";
            const string d2 = @"{{Winners of the National Medal of Science|behav-social}}
";
            const string d3 = @"
[[Category:Members of the National Academy of Sciences]]";

            string e = d2 + d1 + d3;

            string f = MetaDataSorter.RemovePersonData(ref e);

            Assert.AreEqual(d1, f);
            Assert.AreEqual(e, d2 + d3);
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
            const string f = @"Article words
== heading ==
{{redirect2|Fred the dancer|Fred Smith (dancer)}}
words";
            Assert.AreEqual(f, MetaDataSorter.MoveDablinks(f));

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#String_cannot_be_of_zero_length._Parameter_name:_oldValue_.2F_ArgumentException_in_MetaDataSorter.MoveDablinks
            Assert.AreEqual(@"[[Category:Confederate Navy officers|Captains]]", MetaDataSorter.MoveDablinks(@"[[Category:Confederate Navy officers|Captains]]"));

            const string g = @"Some words";
            const string h = @"==heading==
more words
[[Category:Foo]]";

            Assert.AreEqual(e + "\r\n" + g + "\r\n\r\n" + h, MetaDataSorter.MoveDablinks(g + "\r\n" + e + "\r\n" + h));
        }

        [Test]
        public void MoveOrphanTagsTests()
        {
            const string d = @"Fred is a doctor.
Fred has a dog.
[[Category:Dog owners]]
{{some template}}
";

            string e = @"{{Orphan|date=May 2008}}";
            Assert.AreEqual(e + "\r\n" + d, MetaDataSorter.MoveOrphanTags(d + e));

            e = @"{{orphan|date=May 2008}}";
            Assert.AreEqual(e + "\r\n" + d, MetaDataSorter.MoveOrphanTags(d + e));
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
{{Portal|Sport}}
{{Portal|Football}}
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
            const string a = @"{{nofootnotes}}";
            const string a2 = @"{{Morefootnotes}}";

            const string b = @"
'''Article''' words.
== section ==
words.
";
            const string c = @"== References ==
";
            const string d = @"== Notes ==
";
            const string e = @"== Footnotes ==
";

            const string f = @"'''Article''' words.
== section ==
{{nofootnotes}}
words.
";
            const string g = @"words";

            Assert.AreEqual(b + c + a + "\r\n" + g, MetaDataSorter.MoveMoreNoFootnotes(a + b + c + g));
            Assert.AreEqual(b + d + a + "\r\n" + g, MetaDataSorter.MoveMoreNoFootnotes(a + b + d + g));
            Assert.AreEqual(b + e + a + "\r\n" + g, MetaDataSorter.MoveMoreNoFootnotes(a + b + e + g));
            Assert.AreEqual(b + e + a + "\r\n", MetaDataSorter.MoveMoreNoFootnotes(a + b + e));
            Assert.AreEqual(b + e + a2 + "\r\n", MetaDataSorter.MoveMoreNoFootnotes(a2 + b + e));

            // not moved if outside zeroth section
            Assert.AreEqual(f + c + g, MetaDataSorter.MoveMoreNoFootnotes(f + c + g));
            Assert.AreEqual(f + d + g, MetaDataSorter.MoveMoreNoFootnotes(f + d + g));
            Assert.AreEqual(f + e + g, MetaDataSorter.MoveMoreNoFootnotes(f + e + g));
        }

        [Test]
        public void MoveExternalLinksTests()
        {
            const string a = @"'''article'''
== blah ==
words<ref>abc</ref>";
            const string b = @"== external links ==
* [http://www.site.com a site]";
            const string c = @"== References ==
{{reflist}}";
            const string d = @"=== another section ===
blah";
            const string e = @"[[Category:Foos]]";

            const string f = @"{{some footer thing}}";

            const string g = @"== another section ==
blah";
            const string h = @"{{DEFAULTSORT:Foo, bar}}";

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

        [Test]
        public void MoveSeeAlso()
        {
            const string a = @"'''article'''
== blah ==
words<ref>abc</ref>";

            const string b = @"== see also ==
* [http://www.site.com a site]";

            const string c = @"== References ==
{{reflist}}";
            const string d = @"== another section ==
blah";

            Assert.AreEqual(a + "\r\n" + b + "\r\n" + c + "\r\n" + d, MetaDataSorter.MoveSeeAlso(a + "\r\n" + c + "\r\n" + b + "\r\n" + d));

            // no change when already correct
            Assert.AreEqual(a + "\r\n" + b + "\r\n" + c + "\r\n" + d, MetaDataSorter.MoveSeeAlso(a + "\r\n" + b + "\r\n" + c + "\r\n" + d));
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

            const string h = @"[[Category:American keyboardists]]
[[Category:Maroon 5]]
{{Lifetime|1979||Carmichael, Jesse}}
";

            Assert.AreEqual(h, parser2.Sorter.RemoveCats(ref g, "test"));

        }

        [Test]
        public void CategoryAndCommentTests()
        {
            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#Comments_get_removed_from_between_categories
            // allow comments on newline between categories, and keep them in the same place
            const string i = @"#REDIRECT [[Ohio and Mississippi Railway]]

";
            const string j = @"[[Category:Predecessors of the Baltimore and Ohio Railroad]]
<!--Illinois company:-->
[[Category:Railway companies established in 1851]]
[[Category:Railway companies disestablished in 1862]]
[[Category:Defunct Illinois railroads]]
<!--Indiana company:-->
[[Category:Railway companies established in 1848]]
[[Category:Railway companies disestablished in 1867]]
[[Category:Defunct Indiana railroads]]
[[Category:Defunct Ohio railroads]]";

            string k = i + j;

            Assert.AreEqual(j + "\r\n", parser2.Sorter.RemoveCats(ref k, "test"));

            // but don't grab any comment just after the last category
            string l = i + j + @"
<!--foo-->";
            Assert.AreEqual(j + "\r\n", parser2.Sorter.RemoveCats(ref l, "test"));
            Assert.IsTrue(l.Contains("\r\n" + @"<!--foo-->"));

            const string m = @"[[Category:American women writers]]
[[Category:Autism activists]]
<!--LBGT categories are not to be used on bios of living people unless they self-identify with the label and it is relevant to their public lives[[Category:Bisexual actors]]
[[Category:LGBT models]]
[[Category:LGBT people from the United States]]
[[Category:LGBT television personalities]]-->
[[Category:Parents of people on the autistic spectrum]]";

            string n = m;

            Assert.AreEqual(m + "\r\n", parser2.Sorter.RemoveCats(ref n, "test"));

            // comments on same line of category
            const string o = @"[[Category:Canadian Aviation Hall of Fame inductees]]
[[Category:Canadian World War I pilots]] <!-- If he was a Flying Ace, use the Canadian subcategory -->
[[Category:Canadian World War II pilots]] <!-- If he was a Flying Ace, use the subcategory -->
[[Category:Foo]]";

            string p = o;

            Assert.AreEqual(o + "\r\n", parser2.Sorter.RemoveCats(ref p, "test"));

            // comments beside a category are not moved
            string q = @"#REDIRECT [[Alton Railroad]]

";
            string r = @"{{DEFAULTSORT:Joliet Chicago  Railroad}}
[[Category:Predecessors of the Alton Railroad]]
[[Category:Railway companies established in 1855]]
[[Category:Railway companies disestablished in 1950]]<!--http://bulk.resource.org/courts.gov/c/F2/264/264.F2d.445.12430_1.html-->
[[Category:Defunct Illinois railroads]]";
            string s = q + r;
            Assert.AreEqual(r + "\r\n", parser2.Sorter.RemoveCats(ref s, "test"));

            string bug1a = @"[[Category:Emory University]]
[[Category:Law schools in Georgia (U.S. state)]]
<!-- A work in progress to incorporate into nav
[[Category:Southern Law Schools]]
[[Category:South Atlantic Law Schools]]
 -->", bug1b = @"

{{lawschool-stub}}";

            string t = bug1a + bug1b;

            Assert.AreEqual(bug1a + "\r\n", parser2.Sorter.RemoveCats(ref t, "test"));
        }

        [Test]
        public void DefaultSortAndCommentTests()
        {
            string a = @"Foo", b = @"[[Category:Predecessors of the Alton Railroad]]", 
                c = @"{{DEFAULTSORT:Joliet Chicago  Railroad}}", e = @"<!--{{DEFAULTSORT:Joliet Chicago  Railroad}}-->";
            string d = a + "\r\n" + b + "\r\n" + c;
            string f = a + "\r\n" + b + "\r\n" + e;

            Assert.AreEqual(c + "\r\n" + b + "\r\n", parser2.Sorter.RemoveCats(ref d, "test"));
            Assert.AreEqual(b + "\r\n", parser2.Sorter.RemoveCats(ref f, "test"));
        }

        [Test]
        public void InterWikiTests()
        {
            parser2.SortInterwikis = false;

            parser2.Sorter.PossibleInterwikis = new System.Collections.Generic.List<string> { "de", "es", "fr", "it", "sv", "ar", "bs", "br" };

            string a = @"[[de:Canadian National Railway]]
[[es:Canadian National]]
[[fr:Canadien National]]";
            string b = a;

            Assert.AreEqual(b + "\r\n", parser2.Sorter.Interwikis(ref a));

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_12#Interwiki_links_moved_out_of_comment
            string c = @"{{Canadianmetros}}

<!-- 
The following links are here to prevent the interwiki bot from adding them to the list above.  The links below point to disambiguation pages and not to a translated article about Canadian National Railway.
[[it:CN]]
[[sv:CN]]
-->";
            // no interwikis here
            Assert.AreEqual("", parser2.Sorter.Interwikis(ref c));

            // deduplication
            string d = @"[[de:Canadian National Railway]]
[[es:Canadian National]]
[[es:Canadian National]]
[[fr:Canadien National]]";

            Assert.AreEqual(b + "\r\n", parser2.Sorter.Interwikis(ref d));

            string e1 = @"{{DEFAULTSORT:Boleyn, Anne}}

", e2 = @"{{Link FA|bs}}
{{Link FA|no}}
{{Link FA|sv}}
[[ar:آن بولين]]
[[bs:Anne Boleyn]]
[[br:Anne Boleyn]]";

            string f = e2;
            string g = e1 + e2;

            Assert.AreEqual(f + "\r\n", parser2.Sorter.Interwikis(ref g));
        } 

        [Test]
        // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Substituted_templates
        public void NoIncludeIncludeOnlyTests()
        {
            const string a = @"<noinclude>", b = @"[[ar:قالب:بذرة موسيقي]]
[[bg:Шаблон:Музика-мъниче]]
[[ca:Plantilla:Esborrany de música]]
[[cs:Šablona:Hudební pahýl]]
[[cs:Šablona:Hudební pahýl]]
[[vi:Tiêu bản:Music-stub]]", c = @"</noinclude>";

            Assert.AreEqual(a + b + c, parser2.SortMetaData(a + b + c, "foo"));

            Assert.AreNotEqual(b, parser2.SortMetaData(b, "foo"));

            const string d = @"[[Category:Foo]]", e = @"[[Category:More foo]]", f = @"<includeonly>blah[[Category:Foo]]</includeonly>";

            Assert.AreEqual(a + d + c + e, parser2.SortMetaData(a + d + c + e, "foo"));

            Assert.AreNotEqual(d + "\r\n" + e, parser2.SortMetaData(d + e, "foo"));

            Assert.AreEqual(f + d, parser2.SortMetaData(f + d, "foo"));
        }
    }
}
