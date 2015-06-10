using WikiFunctions;
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
		[Test]
		public void RemoveStubs()
		{
			// shouldn't break anything
			string s = "==foo==\r\nbar<ref name=\"123\"/>{{boz}}";
			Assert.AreEqual("", MetaDataSorter.RemoveStubs(ref s));
			Assert.AreEqual("==foo==\r\nbar<ref name=\"123\"/>{{boz}}", s);

			// should remove stubs, but not section stubs
			s = "{{foo}}{{stub}}{{foo-stub}}bar{{sect-stub}}{{not a|stub}}";
			Assert.AreEqual("{{stub}}\r\n{{foo-stub}}\r\n", MetaDataSorter.RemoveStubs(ref s));
			Assert.AreEqual("{{foo}}bar{{sect-stub}}{{not a|stub}}", s);
			
			// handle stubs with extra parameters e.g. date=
			s = "{{foo}}{{stub}}{{foo-stub|date=May 2012}}bar{{sect-stub}}{{not a|stub}}";
			Assert.AreEqual("{{stub}}\r\n{{foo-stub|date=May 2012}}\r\n", MetaDataSorter.RemoveStubs(ref s));

			//shouldn't fail
			s = "";
			Assert.AreEqual("", MetaDataSorter.RemoveStubs(ref s));
			Assert.AreEqual("", s);
			s = "{{stub}}";
			Assert.AreEqual("{{stub}}\r\n", MetaDataSorter.RemoveStubs(ref s));
			Assert.AreEqual("", s);

			// remove duplicate stubs
			s = "{{foo}}{{stub}}{{stub}}";
			Assert.AreEqual("{{stub}}\r\n", MetaDataSorter.RemoveStubs(ref s));
			s = "{{foo}}{{Stub}}{{stub}}";
			Assert.AreEqual("{{Stub}}\r\n", MetaDataSorter.RemoveStubs(ref s));
		}
		
		[Test]
		public void RemoveDupelicateStubs()
		{
			string stub = @"{{foo stub}}", articleTextBack = "";
			string articletext = stub + " " + stub;
			articleTextBack = parser2.SortMetaData(articletext, "test");
			
			Assert.IsTrue(WikiFunctions.WikiRegexes.Stub.Matches(articleTextBack).Count == 1);
			
			// don't remove if different capitalisation
			articletext = stub + " " + @"{{fOO stub}}";
			articleTextBack = parser2.SortMetaData(articletext, "test");
			
			Assert.IsTrue(WikiFunctions.WikiRegexes.Stub.Matches(articleTextBack).Count == 2);
			
			// ignore stubs in comments
			articletext = stub + " " + @"<!--{{foo stub}}-->";
			articleTextBack = parser2.SortMetaData(articletext, "test");
			
			Assert.IsTrue(WikiFunctions.WikiRegexes.Stub.Matches(articleTextBack).Count == 2);
		}
		
		[Test]
		public void InterwikiSpacing()
		{
			parser2.SortInterwikis = false;
			parser2.Sorter.PossibleInterwikis = new System.Collections.Generic.List<string> { "de", "es", "fr", "it", "sv", "ar", "bs", "br" };
			
			const string correct = @"

[[Category:Pub chains in the United Kingdom]]
[[Category:Companies established in 1997]]

[[es:Punch Taverns]]
[[fr:Punch Taverns]]";
			
			Assert.AreEqual(correct, parser2.SortMetaData(correct, "no change"));
			Assert.AreEqual(correct, parser2.SortMetaData(@"

[[Category:Pub chains in the United Kingdom]]
[[Category:Companies established in 1997]]



[[es:Punch Taverns]]
[[fr:Punch Taverns]]", "two newlines before interwikis"));
			
			Assert.AreEqual(correct, parser2.SortMetaData(@"

[[Category:Pub chains in the United Kingdom]]
[[Category:Companies established in 1997]]
[[es:Punch Taverns]]

[[fr:Punch Taverns]]
", "an interwiki immediatelly after categories"));
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
			
			e = d2 + d1 + d1.Replace("[[December 2]]", "") + d3;

			f = MetaDataSorter.RemovePersonData(ref e);

			Assert.AreEqual(d1 + "\r\n" + d1.Replace("[[December 2]]", ""), f, "duplicate persondata: order not changed");
		}

		[Test]
		public void MoveDablinksTests()
		{
			const string d = @"Fred is a doctor.
Fred has a dog.
[[Category:Dog owners]]
{{some template}}
", e0 = @"{{redirect|foo|bar}}";

			string e = @"{{otherpeople1|Fred the dancer|Fred Smith (dancer)}}";
			Assert.AreEqual(e + "\r\n" + d, MetaDataSorter.MoveDablinks(d + e));

			e = @"{{For|Fred the dancer|Fred Smith (dancer)}}";
			Assert.AreEqual(e + "\r\n" + d, MetaDataSorter.MoveDablinks(d + e));
			
			Assert.AreEqual(e + "\r\n" + d, MetaDataSorter.MoveDablinks(e + " " + d));

			e = @"{{redirect2|Fred the dancer|Fred Smith (dancer)}}";
			Assert.AreEqual(e + "\r\n" + d, MetaDataSorter.MoveDablinks(d + e));

			e = @"{{redirect2|Fred the {{dancer}}|Fred Smith (dancer)}}";
			Assert.AreEqual(e + "\r\n" + d, MetaDataSorter.MoveDablinks(d + e));
			
			e = @"{{redirect2|Fred the {{dancer}}|Fred Smith (dancer)}}
";
			Assert.AreEqual(e + d, MetaDataSorter.MoveDablinks(d + ":" + e), "colons before dablinks removed");
			
			// don't move dablinks in a section
			const string f = @"Article words
== heading ==
{{redirect2|Fred the dancer|Fred Smith (dancer)}}
words";
			Assert.AreEqual(f, MetaDataSorter.MoveDablinks(f));

			// https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#String_cannot_be_of_zero_length._Parameter_name:_oldValue_.2F_ArgumentException_in_MetaDataSorter.MoveDablinks
			Assert.AreEqual(@"[[Category:Confederate Navy officers|Captains]]", MetaDataSorter.MoveDablinks(@"[[Category:Confederate Navy officers|Captains]]"));

			const string g = @"Some words";
			const string h = @"==heading==
more words
[[Category:Foo]]";
			e = @"{{redirect2|Fred the {{dancer}}|Fred Smith (dancer)}}";

			Assert.AreEqual(e + "\r\n" + g + "\r\n" + h, MetaDataSorter.MoveDablinks(g + "\r\n" + e + "\r\n" + h));
			
			Assert.AreEqual(e + "\r\n" + e0 + "\r\n" + d, MetaDataSorter.MoveDablinks(d + e + e0));
			Assert.AreEqual(e + "\r\n" + e0 + "\r\n\r\n" + d, MetaDataSorter.MoveDablinks(e + e0 + "\r\n\r\n" + d));
			
			// check spacing
			Assert.AreEqual(e + "\r\n" + d, MetaDataSorter.MoveDablinks(e + "\r\n" + d));
			Assert.AreEqual(e + "\r\n" + e0 + "\r\n" + d, MetaDataSorter.MoveDablinks(e + "\r\n" + e0 + d));
			Assert.AreEqual(e + "\r\n" + e0 + "\r\n" + d, MetaDataSorter.MoveDablinks(e + "\r\n" + e0 + "\r\n" + d));
			Assert.AreEqual(e + "\r\n" + e0 + "\r\n\r\n" + d, MetaDataSorter.MoveDablinks(e + "\r\n" + e0 + "\r\n\r\n" + d));
			Assert.AreEqual(e + "\r\n" + e0 + "\r\n" + e0 + "\r\n\r\n" + d, MetaDataSorter.MoveDablinks(e + "\r\n" + e0 + e0 + "\r\n\r\n" + d));
		}

        [Test]
        public void MoveDeletionProtection()
        {
            string deletiontag = @"{{Prod blp}}", dablink = @"{{otherpeople1|Fred the dancer|Fred Smith (dancer)}}", 
            maintenancetemp = @"{{cleanup}}", foo = @"The rest of the article", infobox = @"{{infobox hello}}",
            mi = @"{{multiple issues|
{{cleanup}}
{{orphan}}            
}}";

            Assert.AreEqual(dablink + "\r\n" + deletiontag + "\r\n" + maintenancetemp + "\r\n" + foo, 
                parser2.SortMetaData(dablink + "\r\n" + deletiontag + "\r\n" + maintenancetemp + "\r\n" + foo, "Foo"), "no change if already OK");
            Assert.AreEqual(dablink + "\r\n" + deletiontag + "\r\n" + maintenancetemp + "\r\n" + foo, 
                parser2.SortMetaData(deletiontag + "\r\n" + dablink + "\r\n" + maintenancetemp + "\r\n" + foo, "Foo"), "dablink above deletion");
            Assert.AreEqual(dablink + "\r\n" + deletiontag + "\r\n" + maintenancetemp + "\r\n" + foo, 
                parser2.SortMetaData(dablink + "\r\n" + maintenancetemp + "\r\n" + deletiontag + "\r\n" + foo, "Foo"), "deletion above maintenance");

            Assert.AreEqual(dablink + "\r\n" + deletiontag + "\r\n" + mi + "\r\n" + infobox + "\r\n" + foo, 
                parser2.SortMetaData(dablink + "\r\n" + mi + "\r\n" + deletiontag + "\r\n" + infobox + "\r\n" + foo, "Foo"), "deletion above multipleissues and mi above infobox");
        }
		
		[Test]
		public void MoveDablinksCommentsTests()
		{
			const string d = @"Fred is a doctor.
Fred has a dog.
[[Category:Dog owners]]
{{some template}}
", e = @"<!-- {{otheruses}} this allows users with [[WP:POPUPS|popups]] to disambiguate links.-->";
			
			// don't pull dabs out of comments
			Assert.AreEqual(d + e, MetaDataSorter.MoveDablinks(d + e));
			
			const string f = @"{{for|the book by René Descartes|The World (Descartes)}}
<!--
Might eventually be relevant on a disambig page?  But these are a redirect and a not direct reference, so they're pretty irrelevant for now.
{{For|the song by the [[Thievery Corporation]]|Le Monde (song)}}
{{For|the drum & bass dj [[Lemon D]]| Lemon D}}
-->";
			
			Assert.AreEqual(f, MetaDataSorter.MoveDablinks(f));
		}
		
		[Test]
		public void RemoveDisambig()
		{
			const string dab = @"{{dab}}", foo = @"foo";
			
			string a = dab + "\r\n" + foo;
			
			Assert.AreEqual(dab, MetaDataSorter.RemoveDisambig(ref a));
			Assert.AreEqual(a, "\r\n" + foo);
			
			// no dabs to take out – no change
			a = foo;
			Assert.AreEqual("", MetaDataSorter.RemoveDisambig(ref a));
			Assert.AreEqual(a, foo);
			
			// don't pull out of comments
			a = @"<!--" + dab + @"-->" + "\r\n" + foo;
			Assert.AreEqual("", MetaDataSorter.RemoveDisambig(ref a));
			Assert.AreEqual(a,  @"<!--" + dab + @"-->" + "\r\n" + foo);

			a = dab + "<!--comm-->\r\n" + foo;
			Assert.AreEqual(dab + "<!--comm-->", MetaDataSorter.RemoveDisambig(ref a), "Handles disambig template with comment after");
			Assert.AreEqual(a, "\r\n" + foo);
		}

		[Test]
		public void MoveMaintenanceTagsTests()
		{
			const string d = @"Fred is a doctor.
Fred has a dog.
[[Category:Dog owners]]
{{some template}}
";

			string e = @"{{Underlinked|date=May 2008}}";
			Assert.AreEqual(e + "\r\n" + d, MetaDataSorter.MoveMaintenanceTags(d + e));

			e = @"{{underlinked|date=May 2008}}";
			Assert.AreEqual(e + "\r\n" + d, MetaDataSorter.MoveMaintenanceTags(d + e));
			
			e = @"{{cleanup|date=May 2008}}";
			Assert.AreEqual(e + "\r\n" + d, MetaDataSorter.MoveMaintenanceTags(d + e), "template moved when not a section one");
			
			e = @"{{cleanup|section|date=May 2008}}";
			Assert.AreEqual(d + e, MetaDataSorter.MoveMaintenanceTags(d + e), "section templates not moved");
			
			// don't move above other maintenance templates
			string f = @"{{cleanup|date=June 2009}}
" + e + d;
			Assert.AreEqual(f, MetaDataSorter.MoveMaintenanceTags(f));
			
			string g = @"{{BLP unsourced|date=August 2009|bot=yes}}
{{Underlinked|date=February 2008}}
'''Charles M. McKim'''";
			
			Assert.AreEqual(g, MetaDataSorter.MoveMaintenanceTags(g));
			
			// do move above infoboxes
			string h1 = @"{{Infobox foo| sdajklfsdjk | dDJfsdjkl }}", h2 = @"{{Underlinked|date=February 2008}}", h3 = @"'''Charles M. McKim'''";
			
			Assert.AreEqual(h2 + "\r\n" + h1 + "\r\n" + h3, MetaDataSorter.MoveMaintenanceTags(h2 + "\r\n" + h1 + "\r\n" + h3));
			Assert.AreEqual(h2 + "\r\n" + h1 + "\r\n\r\n" + h3, MetaDataSorter.MoveMaintenanceTags(h1 + "\r\n" + h2 + "\r\n" + h3));
			
			// do move above infoboxes
			h2 = @"{{Notability|Web|date=February 2008}}";
			
			Assert.AreEqual(h2 + "\r\n" + h1 + "\r\n" + h3, MetaDataSorter.MoveMaintenanceTags(h2 + "\r\n" + h1 + "\r\n" + h3));
			Assert.AreEqual(h2 + "\r\n" + h1 + "\r\n\r\n" + h3, MetaDataSorter.MoveMaintenanceTags(h1 + "\r\n" + h2 + "\r\n" + h3));

			string i1 = @"{{cleanup|date=June 2009}}";
			e = @"{{underlinked|date=May 2008}}";
			// move when tags not all at top
			Assert.AreEqual(e + "\r\n" + i1 + "\r\nfoo\r\n", MetaDataSorter.MoveMaintenanceTags(e + "\r\nfoo\r\n" + i1));
			
			const string CommentedOut = @"<!---sufficient-?---{{Cleanup|date=January 2010}}--{{Wikify|date=January 2010}}---?--->";
			Assert.AreEqual(CommentedOut, MetaDataSorter.MoveMaintenanceTags(CommentedOut), "no change to commented out tags");
			
			const string NewMultipleIssues = @"{{multiple issues|{{Cleanup|date=January 2010}}}}";
			Assert.AreEqual(NewMultipleIssues, MetaDataSorter.MoveMaintenanceTags(NewMultipleIssues), "don't pull tags from new-style {{multiple issues}}");
			
			// zeroth section only
			const string LaterSection = @"hello
==Sec==
{{cleanup|date=May 2012}}
End";
			Assert.AreEqual(LaterSection, parser2.SortMetaData(LaterSection, "A"), "maintenance tags outside of zeroth section not moved");
		}

[Test]
        public void MoveMultipleIssues()
        {
            const string correct = @"{{multiple issues|
{{orphan}}
{{cleanup}}
}}
{{infobox foo}}

Fred has a dog.
[[Category:Dog owners]]";

            Assert.AreEqual(correct, MetaDataSorter.MoveMultipleIssues(@"{{infobox foo}}
{{multiple issues|
{{orphan}}
{{cleanup}}
}}
Fred has a dog.
[[Category:Dog owners]]"), "mi moved above infobox");

            Assert.AreEqual(correct, MetaDataSorter.MoveMultipleIssues(correct), "no change when already correct");

            const string commented = @"{{infobox foo}}
<!--{{multiple issues|
{{orphan}}
{{cleanup}}
}}-->
Fred has a dog.
[[Category:Dog owners]]";
            Assert.AreEqual(commented, MetaDataSorter.MoveMultipleIssues(commented), "commented out infobox so nothing to do");

            const string noinfobox = @"{{foo}}
{{multiple issues|
{{orphan}}
{{cleanup}}
}}
Fred has a dog.
[[Category:Dog owners]]";
            Assert.AreEqual(noinfobox, MetaDataSorter.MoveMultipleIssues(noinfobox), "nothing to move if no infobox");

            const string noMI = @"{{infobox foo}}
{{something|
{{one}}
{{two}}
}}
Fred has a dog.
[[Category:Dog owners]]";
            Assert.AreEqual(noMI, MetaDataSorter.MoveMultipleIssues(noMI), "nothing to move if no MultipleIssues");
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
{{Portal|Football}}
some words", MetaDataSorter.MovePortalTemplates(@"text here {{Portal|Football}}
text here2
== see also ==
some words"));
			
			Assert.AreEqual(@"text here
text here2
== see also ==
{{Portal|Football}}
some words", MetaDataSorter.MovePortalTemplates(@"text here
{{Portal|Football}}
text here2
== see also ==
some words"), "whitespace at end of line after portal template");

			Assert.AreEqual(@"text here
text here2
== see also ==
{{Portal|Football}}
{{Portal|Sport}}
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
		public void MoveTemplateToSeeAlsoSection()
		{
		    const string Commented = @"A
<!-- {{ext}} -->
== See also==
* [[Other]]";
		        Assert.AreEqual(Commented, MetaDataSorter.MoveTemplateToSeeAlsoSection(Commented, Tools.NestedTemplateRegex("ext")));
		}
		[Test]
		public void MovePortalTemplatesTestsDuplicates()
		{
			
			// remove duplicate portals
			Assert.AreEqual(@"text here
text here2
== see also ==
{{Portal|Football}}
some words", MetaDataSorter.MovePortalTemplates(@"text here
{{Portal|Football}}
{{Portal|Football}}
text here2
== see also ==
some words"));
			
			Assert.AreEqual(@"text here
text here2
== see also ==
{{Portal|Football}}
some words
==other==
", MetaDataSorter.MovePortalTemplates(@"text here
{{Portal|Football}}
text here2
== see also ==
some words
==other==
{{Portal|Football}}"));
			
			Assert.AreEqual(@"text here
text here2
== see also ==
{{Portal|Football}}
some words
==other==
", MetaDataSorter.MovePortalTemplates(@"text here
text here2
== see also ==
{{Portal|Football}}
some words
==other==
{{Portal|Football}}"));
		}
		
		[Test]
		public void MoveSisterLinksTest()
		{
			const string WiktInExtLinks = @"text here
text here2
== External links ==
{{wiktionary}}
* Fred";
			
			Assert.AreEqual(@"text here
text here2
== External links ==
{{wiktionary}}
* Fred
== other ==
x", MetaDataSorter.MoveSisterlinks(@"text here
{{wiktionary}}
text here2
== External links ==
* Fred
== other ==
x"), "another section at end");
			
			Assert.AreEqual(@"text here
text here2
== External links ==
{{Sister project links|Programming languages|wikt=programming language|b=Computer Programming|s=How to Think Like a Computer Scientist|commons=Category:Programming languages}}
* Fred
== other ==
x", MetaDataSorter.MoveSisterlinks(@"text here
{{Sister project links|Programming languages|wikt=programming language|b=Computer Programming|s=How to Think Like a Computer Scientist|commons=Category:Programming languages}}
text here2
== External links ==
* Fred
== other ==
x"), "sister project links may contain many parameters");

			Assert.AreEqual(WiktInExtLinks, MetaDataSorter.MoveSisterlinks(@"text here
{{wiktionary}}
text here2
== External links ==
* Fred"), "ext links is last section");
			
			Assert.AreEqual(WiktInExtLinks, MetaDataSorter.MoveSisterlinks(WiktInExtLinks), "wikt already in ext links");
			const string CommentedOut = @"text here
<!--{{wiktionary}}-->
text here2
== External links ==
* Fred
== other ==
x";
			Assert.AreEqual(CommentedOut, MetaDataSorter.MoveSisterlinks(CommentedOut), "no change if commented out");

			const string NoExtLinks = @"text here
{{wiktionary}}
text here2
== other ==
x";
			Assert.AreEqual(NoExtLinks, MetaDataSorter.MoveSisterlinks(NoExtLinks), "no ext links sections");
		}

		[Test]
		public void MoveMoreNoFootnotesTests()
		{
			const string a = @"{{nofootnotes}}", a2 = @"{{Morefootnotes}}", b = @"
'''Article''' words.
== section ==
words.
";
			const string c = @"== References ==
", d = @"== Notes ==
", e = @"== Footnotes ==
", f = @"'''Article''' words.
== section ==
{{nofootnotes}}
words.
";
			const string g = @"words";

			Assert.AreEqual(b + c + a + "\r\n" + g, MetaDataSorter.MoveTemplateToReferencesSection(a + b + c + g, WikiFunctions.WikiRegexes.MoreNoFootnotes, true));
			Assert.AreEqual(b + d + a + "\r\n" + g, MetaDataSorter.MoveTemplateToReferencesSection(a + b + d + g, WikiFunctions.WikiRegexes.MoreNoFootnotes, true));
			Assert.AreEqual(b + e + a + "\r\n" + g, MetaDataSorter.MoveTemplateToReferencesSection(a + b + e + g, WikiFunctions.WikiRegexes.MoreNoFootnotes, true));
			Assert.AreEqual(b + e + a + "\r\n", MetaDataSorter.MoveTemplateToReferencesSection(a + b + e, WikiFunctions.WikiRegexes.MoreNoFootnotes, true));
			Assert.AreEqual(b + e + a2 + "\r\n", MetaDataSorter.MoveTemplateToReferencesSection(a2 + b + e, WikiFunctions.WikiRegexes.MoreNoFootnotes, true));
			Assert.AreEqual(b + e + a2 + "\r\n", MetaDataSorter.MoveTemplateToReferencesSection(a2 + b + e, WikiFunctions.WikiRegexes.MoreNoFootnotes, false));
			
			Assert.AreEqual(b + d.Replace("Notes", "Notes and references") + a + "\r\n" + g, MetaDataSorter.MoveTemplateToReferencesSection(a + b + d.Replace("Notes", "Notes and references") + g, WikiFunctions.WikiRegexes.MoreNoFootnotes, true));

			// not moved if outside zeroth section
			Assert.AreEqual(f + c + g, MetaDataSorter.MoveTemplateToReferencesSection(f + c + g, WikiFunctions.WikiRegexes.MoreNoFootnotes, true));
			Assert.AreEqual(f + d + g, MetaDataSorter.MoveTemplateToReferencesSection(f + d + g, WikiFunctions.WikiRegexes.MoreNoFootnotes, true));
			Assert.AreEqual(f + e + g, MetaDataSorter.MoveTemplateToReferencesSection(f + e + g, WikiFunctions.WikiRegexes.MoreNoFootnotes, true));
			
			// if duplicate sections, don't duplicate template
			Assert.AreEqual(b + c + a + "\r\n" + c  + g, MetaDataSorter.MoveTemplateToReferencesSection(a + b + c +c + g, WikiFunctions.WikiRegexes.MoreNoFootnotes, true));
		}
		
		[Test]
		public void MoveIbidTests()
		{
			string a = @"{{Ibid|date=May 2009}}", b = @"
'''Article''' words<ref>ibid</ref>.
== section ==
words.
", c = @"== References ==
", d = @"== Notes ==
", e = @"== Footnotes ==
", f = @"'''Article''' words.
== section ==
{{Ibid|date=May 2009}}
words.
";
			const string g = @"more words";

			Assert.AreEqual(b + c + a + "\r\n" + g, MetaDataSorter.MoveTemplateToReferencesSection(a + b + c + g, WikiFunctions.WikiRegexes.Ibid, true));
			Assert.AreEqual(b + d + a + "\r\n" + g, MetaDataSorter.MoveTemplateToReferencesSection(a + b + d + g, WikiFunctions.WikiRegexes.Ibid, true));
			Assert.AreEqual(b + e + a + "\r\n" + g, MetaDataSorter.MoveTemplateToReferencesSection(a + b + e + g, WikiFunctions.WikiRegexes.Ibid, true));
			Assert.AreEqual(b + e + a + "\r\n", MetaDataSorter.MoveTemplateToReferencesSection(a + b + e, WikiFunctions.WikiRegexes.Ibid, true));
			
			Assert.AreEqual(b + c + a + "\r\n" + g, MetaDataSorter.MoveTemplateToReferencesSection(a + b + c + g, WikiFunctions.WikiRegexes.Ibid, false));
			Assert.AreEqual(b + d + a + "\r\n" + g, MetaDataSorter.MoveTemplateToReferencesSection(a + b + d + g, WikiFunctions.WikiRegexes.Ibid, false));
			Assert.AreEqual(b + e + a + "\r\n" + g, MetaDataSorter.MoveTemplateToReferencesSection(a + b + e + g, WikiFunctions.WikiRegexes.Ibid));
			Assert.AreEqual(b + e + a + "\r\n", MetaDataSorter.MoveTemplateToReferencesSection(a + b + e, WikiFunctions.WikiRegexes.Ibid));
			
			Assert.AreEqual(b + c + d + a + "\r\n" + g, MetaDataSorter.MoveTemplateToReferencesSection(a + b + c + d + g, WikiFunctions.WikiRegexes.Ibid, false), "move to Notes ahead of References");
			Assert.AreEqual(b + d + a + "\r\n" + c + g, MetaDataSorter.MoveTemplateToReferencesSection(b + d + a + "\r\n" + c + g, WikiFunctions.WikiRegexes.Ibid), "No change: ibid already in notes section");
			
			// outside zeroth section – okay
			Assert.AreEqual(@"'''Article''' words.
== section ==

words.
" + c + a + "\r\n" + g, MetaDataSorter.MoveTemplateToReferencesSection(f + c + g, WikiFunctions.WikiRegexes.Ibid, false));

			e = @"== Sources ==
";
			Assert.AreEqual(b + e + a + "\r\n", MetaDataSorter.MoveTemplateToReferencesSection(a + b + e, WikiFunctions.WikiRegexes.Ibid));
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
			
			// don't' move external links if would create ref after reflist
			string ExtLinkRef = a + "\r\n" + b + "<ref>foo</ref>\r\n" + c + "\r\n" + e;
			Assert.AreEqual(ExtLinkRef, MetaDataSorter.MoveExternalLinks(ExtLinkRef));
		}

		[Test]
		public void MoveSeeAlso()
		{
			const string a = @"'''article'''
== blah ==
words<ref>abc</ref>";

			const string b = @"== See also ==
* [http://www.site.com a site]";

			const string c = @"== References ==
{{reflist}}";
			const string d = @"== another section ==
blah";

			Assert.AreEqual(a + "\r\n" + b + "\r\n\r\n" + c + "\r\n" + d, MetaDataSorter.MoveSeeAlso(a + "\r\n" + c + "\r\n" + b + "\r\n" + d),"reorder");

			// no change when already correct
			Assert.AreEqual(a + "\r\n" + b + "\r\n" + c + "\r\n" + d, MetaDataSorter.MoveSeeAlso(a + "\r\n" + b + "\r\n" + c + "\r\n" + d),"no change when already correct");
			
			const string TwoReferencesSection = a + "\r\n" + c + "\r\n" + b + "\r\n" + c + "\r\n" + d;
			Assert.AreEqual(TwoReferencesSection, MetaDataSorter.MoveSeeAlso(TwoReferencesSection), "no change when two references sections");
		}
		
		[Test]
		public void CategoriesForDeletion()
		{
			const string a = @"[[Category:Railway companies established in 1851]]";
			const string b1 = @"[[Category:Pages for deletion]]", b2 = @"[[Category:Categories for deletion]]", b3 = @"[[Category:Articles for deletion]]";

			string k = a + "\r\n" + b1;
			Assert.AreEqual(a + "\r\n", parser2.Sorter.RemoveCats(ref k, "test"), "Pages for deletion cat not kept");
			
			k = a + "\r\n" + b2;
			Assert.AreEqual(a + "\r\n", parser2.Sorter.RemoveCats(ref k, "test"), "Categories for deletion cat not kept");
			
			k = a + "\r\n" + b3;
			Assert.AreEqual(a + "\r\n", parser2.Sorter.RemoveCats(ref k, "test"), "Articles for deletion cat not kept");
		}

		[Test]
		public void CategoryAndCommentTests()
		{
			// https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#Comments_get_removed_from_between_categories
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

			Assert.AreEqual(j + "\r\n", parser2.Sorter.RemoveCats(ref k, "test"), "comments between cats");

			// but don't grab any comment just after the last category
			string l = i + j + @"
<!--foo-->";
			Assert.AreEqual(j + "\r\n", parser2.Sorter.RemoveCats(ref l, "test"), "comment after cat");
			Assert.IsTrue(l.Contains("\r\n" + @"<!--foo-->"), "comment after cat 2");

			const string m = @"[[Category:American women writers]]
[[Category:Autism activists]]
<!--LBGT categories are not to be used on bios of living people unless they self-identify with the label and it is relevant to their public lives[[Category:Bisexual actors]]
[[Category:LGBT models]]
[[Category:LGBT people from the United States]]
[[Category:LGBT television personalities]]-->
[[Category:Parents of people on the autistic spectrum]]";

			string n = m;

			Assert.AreEqual(m + "\r\n", parser2.Sorter.RemoveCats(ref n, "test"), "cats not pulled out of comments");

			// comments on same line of category
			const string o = @"[[Category:Canadian Aviation Hall of Fame inductees]]
[[Category:Canadian World War I pilots]] <!-- If he was a Flying Ace, use the Canadian subcategory -->
[[Category:Canadian World War II pilots]] <!-- If he was a Flying Ace, use the subcategory -->
[[Category:Foo]]";

			string p = o;

			Assert.AreEqual(o + "\r\n", parser2.Sorter.RemoveCats(ref p, "test"), "comments on line");

			// comments beside a category are not moved
			string q = @"#REDIRECT [[Alton Railroad]]

";
			string r = @"{{DEFAULTSORT:Joliet Chicago  Railroad}}
[[Category:Predecessors of the Alton Railroad]]
[[Category:Railway companies established in 1855]]
[[Category:Railway companies disestablished in 1950]]<!--http://bulk.resource.org/courts.gov/c/F2/264/264.F2d.445.12430_1.html-->
[[Category:Defunct Illinois railroads]]";
			string s = q + r;
			Assert.AreEqual(r + "\r\n", parser2.Sorter.RemoveCats(ref s, "test"), "comments on line end");
			
			string r2 = @"{{DEFAULTSORT:Joliet Chicago  Railroad}}
[[Category:Predecessors of the Alton Railroad]]
[[Category:Railway companies established in 1855]]
[[Category:Railway companies disestablished in 1950]]
[[Category:Defunct Illinois railroads]]  <!--comm-->";
			s = q + r2;
			Assert.AreEqual(r2 + "\r\n", parser2.Sorter.RemoveCats(ref s, "test"), "comment after last cat on same line not moved");

			string bug1a = @"[[Category:Emory University]]
[[Category:Law schools in Georgia (U.S. state)]]
<!-- A work in progress to incorporate into nav
[[Category:Southern Law Schools]]
[[Category:South Atlantic Law Schools]]
 -->", bug1b = @"

{{lawschool-stub}}";

			string t = bug1a + bug1b;

			Assert.AreEqual(bug1a + "\r\n", parser2.Sorter.RemoveCats(ref t, "test"), "commented out cats");

			string bug1c = @"<!-- foo bar-->
{{info}}
text
";
			t = bug1c + bug1a + bug1b;

			Assert.AreEqual(bug1a + "\r\n", parser2.Sorter.RemoveCats(ref t, "test"), "commented out plus comments");
			
			string bug2 = @"{{The Surreal Life}}
<!--The 1951 birth date has been upheld in court, please do not add this category.[[Category:1941 births]]-->

[[Category:Living People]]
foo";
			
			Assert.IsFalse(parser2.Sorter.RemoveCats(ref bug2, "test").Contains(@"[[Category:Living People]]"), "commented out and not");
			
			string nw = @"[[Category:American women writers]]
[[Category:Autism activists]]
<nowiki>
[[Category:LGBT people from the United States]]
[[Category:LGBT television personalities]]</nowiki>
[[Category:Parents of people on the autistic spectrum]]";

			Assert.AreEqual("", parser2.Sorter.RemoveCats(ref nw, "test"), "nowiki cats");
			Assert.IsFalse(parser2.Sorter.RemoveCats(ref nw, "test").Contains(@"[[Category:LGBT people from the United States]]"), "cat after nowiki");
			
			string iw1 = @"[[Category:Hampshire|  ]]
[[Category:Articles including recorded pronunciations (UK English)]]
[[Category:Non-metropolitan counties]]", iw2 = @"

<!--interwiki-->
[[af:Hampshire]]";
			string iwall = iw1 + iw2;
			
			Assert.AreEqual(iw1 + "\r\n", parser2.Sorter.RemoveCats(ref iwall, "test"), "don't pull the interwiki comment");
			Assert.IsTrue(iwall.Contains(@"<!--interwiki-->"), "don't pull the interwiki comment");

			string writers = @"[[Category:Writers from Alabama]]
[[Category:Writers from North Carolina]]
[[Category:Writers from North Carolina]]

<!-- novelists tree -->
[[Category:20th-century American novelists]]";
			Assert.AreEqual(@"[[Category:Writers from Alabama]]
[[Category:Writers from North Carolina]]

<!-- novelists tree -->
[[Category:20th-century American novelists]]" + "\r\n",
			                parser2.Sorter.RemoveCats(ref writers, "test"), "Dupe cat removed if second has comment");
		}

		[Test]
		public void CategoryDupeRemoval()
		{
		    string cats = @"[[Category:One]]
[[Category:Two]]
[[Category:One]]";
			
			Assert.AreEqual( @"[[Category:One]]
[[Category:Two]]
", parser2.Sorter.RemoveCats(ref cats, "test"), "Duplicate category removed, no sortkey");
		    
		    cats = @"[[Category:One]]
[[Category:Two]]
[[Category:One]]
[[Category:One]]";
			
			Assert.AreEqual( @"[[Category:One]]
[[Category:Two]]
", parser2.Sorter.RemoveCats(ref cats, "test"), "Duplicate categories removed, no sortkey");
		    
		    cats = @"[[Category:One|A]]
[[Category:Two]]
[[Category:One|A]]";
			
			Assert.AreEqual( @"[[Category:One|A]]
[[Category:Two]]
", parser2.Sorter.RemoveCats(ref cats, "test"), "Duplicate category removed, same sortkey");
		    
		    cats = @"[[Category:One]] <!--comm-->
[[Category:Two]]
[[Category:One]]";
			
			Assert.AreEqual( @"[[Category:One]] <!--comm-->
[[Category:Two]]
", parser2.Sorter.RemoveCats(ref cats, "test"), "Duplicate category removed, first with comment");
		    
		    cats = @"[[Category:One|A]]
[[Category:Two]]
[[Category:One|B]]";
			
			Assert.AreEqual( @"[[Category:One|A]]
[[Category:Two]]
[[Category:One|B]]
", parser2.Sorter.RemoveCats(ref cats, "test"), "Duplicate category NOT removed, conflicting sortkey");
		    
		    cats = @"[[Category:One]] <!--comm-->
[[Category:Two]]
[[Category:One]] <!--comm2-->";
			
			Assert.AreEqual( @"[[Category:One]] <!--comm-->
[[Category:Two]]
[[Category:One]] <!--comm2-->
", parser2.Sorter.RemoveCats(ref cats, "test"), "Duplicate category NOT removed, conflicting comments");
		    cats = @"[[Category:One|A]]
[[Category:Two]]
[[Category:One]]";
		    Assert.AreEqual( @"[[Category:One|A]]
[[Category:Two]]
", parser2.Sorter.RemoveCats(ref cats, "test"), "Duplicate category removed, one without sortkey");

		    cats = @"[[Category:One|A]]
[[Category:Two]]
[[Category:One]]";
		    
		    Assert.AreEqual( @"[[Category:One|A]]
[[Category:Two]]
", parser2.Sorter.RemoveCats(ref cats, "A"), "Duplicate category removed, one without sortkey");

		    cats = @"[[Category:One|A]]
[[Category:Two]]
[[Category:One|a]]";
			
		    Assert.AreEqual( @"[[Category:One|A]]
[[Category:Two]]
", parser2.Sorter.RemoveCats(ref cats, "test"), "Duplicate category removed, same sortkey ignoring first letter case");		    

		    cats = @"[[Category:One|foo]]
[[Category:Two]]
[[Category:One|foo]]";

		    Assert.AreEqual( @"[[Category:One|foo]]
[[Category:Two]]
", parser2.Sorter.RemoveCats(ref cats, "test"), "Duplicate category removed, lowercase sortkey");

		    cats = @"[[Category:One|Foo]]
[[Category:Two]]
[[Category:One|Foo]]";

		    Assert.AreEqual( @"[[Category:One|Foo]]
[[Category:Two]]
", parser2.Sorter.RemoveCats(ref cats, "test"), "Duplicate category removed, uppercase sortkey");

            // dupe uncat tags
            cats = @"[[Category:One]]
{{uncat|date=August 2014}}
{{uncat|date=August 2014}}
";

            Assert.AreEqual(@"{{uncat|date=August 2014}}
[[Category:One]]
", parser2.Sorter.RemoveCats(ref cats, "test"), "Duplicate uncat tags removed");
		}

		[Test]
		public void CategoryCommentTests()
		{
			const string cats = @"[[Category:Hampshire|  ]]
[[Category:Articles including recorded pronunciations (UK English)]]
[[Category:Non-metropolitan counties]]", comm = @"<!-- cat -->";
			
			string a = cats + "\r\n" + comm;
			
			Assert.AreEqual(comm + "\r\n" + cats + "\r\n", parser2.Sorter.RemoveCats(ref a, "test"));

			string b = @"Text
[[Category:One]]

==References==
{{reflist}}
[[Category:Two]]";
			Assert.AreEqual("[[Category:One]]\r\n[[Category:Two]]\r\n", parser2.Sorter.RemoveCats(ref b, "test"));
			Assert.AreEqual("Text\r\n\r\n==References==\r\n{{reflist}}\r\n", b, "Blank newline before heading retained");

b = @"A
==Text==
{{foo}}[[Category:One]]
{| class=table |}

==References==
{{reflist}}
[[Category:Two]]";
			Assert.AreEqual("[[Category:One]]\r\n[[Category:Two]]\r\n", parser2.Sorter.RemoveCats(ref b, "test"));
			Assert.AreEqual("A\r\n==Text==\r\n{{foo}}\r\n{| class=table |}\r\n\r\n==References==\r\n{{reflist}}\r\n", b, "Newline retained when category not on own line");
		}
		
		[Test]
		public void CategoryRedirects()
		{
			const string redirect=@"#REDIRECT [[Category:Foo]]";
			string a = redirect;
			
			Assert.AreEqual("", parser2.Sorter.RemoveCats(ref a, "test"));
			Assert.AreEqual(a, redirect, "cats not pulled from redirect target");
		}
		
		[Test]
		public void RemoveCatsKey()
		{
		    string cats = @"[[Category:1980 births]]
[[Category:Local people]]", cats2 = @"[[Category:1980 births|Jones, Andrew]]
[[Category:Local people|Jones, Andrew]]";
			
		    parser2.Sorter.AddCatKey = true;
			Assert.AreEqual(cats2 + "\r\n", parser2.Sorter.RemoveCats(ref cats, "Andrew Jones"));
			parser2.Sorter.AddCatKey = false;

			// Extract {{Uncategorized}}
			string at = @"Text.

{{Uncategorized}}
== References ==
{{reflist}}";
			Assert.AreEqual(@"{{Uncategorized}}" + "\r\n", parser2.Sorter.RemoveCats(ref at, "Andrew Jones"), "uncat moved");
			Assert.IsFalse(WikiRegexes.Uncat.IsMatch(at));
			at = @"Text.

<!--{{Uncategorized}}-->
== References ==
{{reflist}}";
			Assert.AreEqual("", parser2.Sorter.RemoveCats(ref at, "Andrew Jones"), "commented out uncat not moved");
			at = @"Text.

{{Uncategorized stub}}
== References ==
{{reflist}}";
			Assert.AreEqual("", parser2.Sorter.RemoveCats(ref at, "Andrew Jones"), "uncat stub not moved");
			at = @"Text.

{{Uncategorized stub|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}
== References ==
{{reflist}}";
			Assert.AreEqual("", parser2.Sorter.RemoveCats(ref at, "Andrew Jones"), "uncat stub not moved");
			at = @"{{multiple issues|
{{a}}
{{Uncategorized}}
{{b}}
}}
== References ==
{{reflist}}";
			Assert.IsTrue(parser2.SortMetaData(at, "Andrew Jones").Contains(@"{{multiple issues|
{{a}}
{{b}}
}}"), "no blank line left in MI");
		}
		
		[Test]
		public void RemoveCatsSlWikiLifetime()
		{
			#if DEBUG
			Variables.SetProjectLangCode("sl");
			
			string c = @"[[Category:Hampshire]]", l = @"{{Lifetime|1899|LIVING|Surname, Name}}", waffle = @"waffle here
";
			string articleText = waffle + c + "\r\n" + l;
			
			// http://sl.wikipedia.org/wiki/Predloga:Lifetime
			Assert.AreEqual(l + "\r\n" + c + "\r\n", parser2.Sorter.RemoveCats(ref articleText, "test"), "lifetime treated like DEFAULTSORT on sl-wiki");
			
			Variables.SetProjectLangCode("en");
			articleText = waffle + c + "\r\n" + l;
			Assert.AreEqual(c + "\r\n", parser2.Sorter.RemoveCats(ref articleText, "test"), "lifetime is just another template on en-wiki");
			#endif
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
			
			string g = c + "\r\n" + c;
			Assert.AreEqual("", parser2.Sorter.RemoveCats(ref g, "test"), "does not modify page with multiple defaultsorts");
		}

		[Test]
		public void InterWikiTests()
		{
			parser2.SortInterwikis = false;

			parser2.Sorter.PossibleInterwikis = new System.Collections.Generic.List<string> { "de", "es", "fr", "it", "sv", "ar", "bs", "br", "en", "jbo" };

			string a = @"[[de:Canadian National Railway]]
[[es:Canadian National]]
[[fr:Canadien National]]";
			string b = a;

			Assert.AreEqual(b + "\r\n", parser2.Sorter.Interwikis(ref a),"no changes");
			
			// comment handling
			string comm = @"<!-- other languages -->";
			a = @"[[de:Canadian National Railway]]
[[es:Canadian National]]
[[fr:Canadien National]]" + comm;
			Assert.AreEqual(comm + "\r\n" + b + "\r\n", parser2.Sorter.Interwikis(ref a));
			
			comm = @"<!--Interwikis-->";
			a = @"[[de:Canadian National Railway]]
[[es:Canadian National]]
[[fr:Canadien National]]" + comm;
			Assert.AreEqual(comm + "\r\n" + b + "\r\n", parser2.Sorter.Interwikis(ref a));
			
			comm = @"<!--Other wikis-->";
			a = @"[[de:Canadian National Railway]]
[[es:Canadian National]]
[[fr:Canadien National]]" + comm;
			Assert.AreEqual(comm + "\r\n" + b + "\r\n", parser2.Sorter.Interwikis(ref a));
			
			comm = @"<!-- interwiki links to this article in other languages, below -->";
			a = @"[[de:Canadian National Railway]]
[[es:Canadian National]]
[[fr:Canadien National]]" + comm;
			Assert.AreEqual(comm + "\r\n" + b + "\r\n", parser2.Sorter.Interwikis(ref a));

			// https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_12#Interwiki_links_moved_out_of_comment
			string c = @"{{Canadianmetros}}

<!--
The following links are here to prevent the interwiki bot from adding them to the list above.  The links below point to disambiguation pages and not to a translated article about Canadian National Railway.
[[it:CN]]
[[sv:CN]]
-->";

			Assert.AreEqual("", parser2.Sorter.Interwikis(ref c), "interwikis not taken out of wiki comments");
			
			string c2 = c + @"[[sv:CN]]";
			Assert.AreEqual(@"[[sv:CN]]" + "\r\n", parser2.Sorter.Interwikis(ref c2), "interwiki returned even if the same one exists commented out elsewhere");

			c = @"{{Canadianmetros}}

<!--
The following links are here to prevent the interwiki bot from adding them to the list above.  The links below point to disambiguation pages and not to a translated article about Canadian National Railway.
[[it:CN]]
[[sv:CN]]
-->
second comment <!-- [[it:CN]] -->";
			Assert.AreEqual("", parser2.Sorter.Interwikis(ref c), "interwikis not taken out of wiki comments");
			
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
			
			string h = @"[[de:Canadian National Railway]]
[[en:Canadian National]]
[[fr:Canadien National]]";

			Assert.AreEqual(@"[[de:Canadian National Railway]]
[[fr:Canadien National]]" + "\r\n", parser2.Sorter.Interwikis(ref h), "interwikis to own wiki are removed");

			a = @"[[de:Canadian National Railway]]
[[es::Canadian National]]
[[fr:Canadien National]]";
			b = a.Replace("::", ":");

			Assert.AreEqual(b + "\r\n", parser2.Sorter.Interwikis(ref a), "double colon in interwiki removed");
			
			a = @"[[de:Canadian National Railway]]
[[es:Canadian National]]
[[es:canadian National]]
[[fr:Canadien National]]";
			b = @"[[de:Canadian National Railway]]
[[es:Canadian National]]
[[fr:Canadien National]]";

			Assert.AreEqual(b + "\r\n", parser2.Sorter.Interwikis(ref a), "duplicate interwiki removed, variation in first letter casing only");

			a = @"[[de:Canadian National Railway]]
[[es:Canadian National]]
[[es:Canadian National]]<!-- comm-->
[[fr:Canadien National]]";
			b = @"[[de:Canadian National Railway]]
[[es:Canadian National]]
[[fr:Canadien National]]";

			Assert.AreEqual(b + "\r\n", parser2.Sorter.Interwikis(ref a), "duplicate interwiki removed, inline comment second");

			a = @"[[de:Canadian National Railway]]
[[es:Canadian National]]<!-- comm-->
[[es:Canadian National]]
[[fr:Canadien National]]";
			b = @"[[de:Canadian National Railway]]
[[es:Canadian National]]<!-- comm-->
[[fr:Canadien National]]";

			Assert.AreEqual(b + "\r\n", parser2.Sorter.Interwikis(ref a), "duplicate interwiki removed, inline comment first");
			
			a = @"[[de:Canadian National Railway]]
[[jbo:canadian National]]
[[fr:Canadien National]]";

			Assert.AreEqual(a + "\r\n", parser2.Sorter.Interwikis(ref a), "first letter casing retained for jbo-wiki links");
		}
		
		[Test]
		public void InterWikiTestsMultiple()
		{
			parser2.SortInterwikis = false;
			parser2.Sorter.PossibleInterwikis = new System.Collections.Generic.List<string> { "de", "es", "fr", "it", "sv", "ar", "bs", "br", "en" };

			string a = @"[[de:Canadian National Railway]]
[[fr:Canadian National (foo)]]
[[fr:Canadien National]]";
			string b = a;

			Assert.AreEqual(b + "\r\n", parser2.Sorter.Interwikis(ref a));
			
			a = @"[[fr:Sénatus-consultes sous Napoléon III]]
[[fr:Sénatus-consulte]]";
			b = a;

			Assert.AreEqual(b + "\r\n", parser2.Sorter.Interwikis(ref a));
		}
		
		[Test]
		public void InterwikiSorting()
		{
		    parser2.SortInterwikis = true;
		    parser2.Sorter.PossibleInterwikis = new System.Collections.Generic.List<string> { "ar", "de", "en", "ru", "sq" };
		    
		    string a = @"[[sq:Foo]]
[[ru:Bar]]";
		    string b = @"[[ru:Bar]]
[[sq:Foo]]";
		    Assert.AreEqual(b + "\r\n", parser2.Sorter.Interwikis(ref a),"reorder if neccessary");
		    
		    a = @"[[ru:Bar]]
[[sq:Foo]]";
		    Assert.AreEqual(b + "\r\n", parser2.Sorter.Interwikis(ref a),"no change Diff everything is sorted");		    
		    
		    parser2.InterWikiOrder = InterWikiOrderEnum.Alphabetical;
		    a = @"[[ru:Bar]]
[[sq:Foo]]";
		    Assert.AreEqual(b + "\r\n", parser2.Sorter.Interwikis(ref a),"alphabetical order");
		    
		    parser2.InterWikiOrder = InterWikiOrderEnum.LocalLanguageFirstWord;
		    a = @"[[ru:Bar]]
[[sq:Foo]]";
		    Assert.AreEqual(b + "\r\n", parser2.Sorter.Interwikis(ref a),"local language");
		    
		    parser2.InterWikiOrder = InterWikiOrderEnum.AlphabeticalEnFirst;
		    a = @"[[ru:Bar]]
[[sq:Foo]]";
		    Assert.AreEqual(b + "\r\n", parser2.Sorter.Interwikis(ref a),"alphabetical order with en firsth");
		    
		    #if DEBUG
		    Variables.SetProjectLangCode("de");
		    parser2.InterWikiOrder = InterWikiOrderEnum.AlphabeticalEnFirst;
		    a = @"[[ar:Bar]]
[[en:Hello]]
[[sq:Foo]]";
		    b = @"[[en:Hello]]
[[ar:Bar]]
[[sq:Foo]]";
		    Assert.AreEqual(b + "\r\n", parser2.Sorter.Interwikis(ref a));
		    
		    parser2.InterWikiOrder = InterWikiOrderEnum.Alphabetical;
		    a = @"[[ar:Bar]]
[[en:Hello]]
[[sq:Foo]]";
		    b = @"[[ar:Bar]]
[[en:Hello]]
[[sq:Foo]]";
		    Assert.AreEqual(b + "\r\n", parser2.Sorter.Interwikis(ref a));
		    
		    Variables.SetProjectLangCode("en");
		    #endif
		    
		    parser2.SortInterwikis = false;
		}
		
		[Test]
		public void InterWikiTestsInlineComments()
		{
			parser2.SortInterwikis = false;

			parser2.Sorter.PossibleInterwikis = new System.Collections.Generic.List<string> { "de", "es", "fr", "it", "sv", "ar", "bs", "br", "en" };

			string a = @"[[de:Canadian National Railway]]
[[es:Canadian National]] <!--comm-->
[[fr:Canadien National]] <!--comm2 -->";
			string b = a;

			Assert.AreEqual(b + "\r\n", parser2.Sorter.Interwikis(ref a), "inline comment kept with interwiki");
		}
		
		[Test]
		public void SelfInterwikisEn()
		{
			Assert.AreEqual("", parser2.SortMetaData(@"<!-- [[en:Foo]]-->", "Test"), "Commented out en interwikis removed");
			
			#if DEBUG
			Variables.SetProjectSimple("en", ProjectEnum.commons);
			const string EnInterwiki = @"[[en:Foo]]";
			Assert.AreEqual(EnInterwiki, parser2.SortMetaData(EnInterwiki, "Test"), "en interwiki not removed on commons");
			
			Variables.SetProjectSimple("en", ProjectEnum.wikipedia);
			Assert.AreEqual("", parser2.SortMetaData(EnInterwiki, "Test"), "en interwiki removed on en-wiki");
			#endif
		}
		
		[Test]
		public void SorterNamespaceTests()
		{
			const string cat = @"[[Category:Foo]]";
			
			Assert.AreEqual(cat, parser2.SortMetaData(cat, "Category:Bar"), "Full trim on category namespace");
			Assert.AreEqual(cat, parser2.SortMetaData(cat, "Template:Bar"), "no sorting on template namespace");
			Assert.AreEqual("\r\n\r\n" + cat, parser2.SortMetaData(cat, "Bar"), "sorting applied on mainspace");
			
			const string CatPopStub = @"Text
{{popstub}}
Text";
			Assert.AreEqual(CatPopStub, parser2.SortMetaData(CatPopStub, "Category:Foo"), "no stub sorting on Category namespace");
		}

		[Test]
		// https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Substituted_templates
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
				
		[Test]
		public void SortOtherLanguages()
		{
		    #if DEBUG
		    Variables.SetProjectLangCode("ru");
		    WikiRegexes.MakeLangSpecificRegexes();
		    
		    parser2.Sorter.PossibleInterwikis = new System.Collections.Generic.List<string> { "en", "zh" };
		    parser2.Sorter.SortInterwikis = false;
		    
		    const string before = @"== Литература ==
* Taylor, Peter. (1989).
{{ref-en}}

[[Category:Флора Африки]]
[[Category:Хищные растения]]
[[Category:Эндемики Африки]]
[[Category:Пузырчатковые]]
[[Category:Таксоны растений, названные в честь людей]]

{{botanics-stub}}

[[en:Utricularia pobeguinii]]
[[zh:波贝甘狸藻]]", after = @"== Литература ==
* Taylor, Peter. (1989).
{{ref-en}}

{{botanics-stub}}

[[Category:Флора Африки]]
[[Category:Хищные растения]]
[[Category:Эндемики Африки]]
[[Category:Пузырчатковые]]
[[Category:Таксоны растений, названные в честь людей]]

[[en:Utricularia pobeguinii]]
[[zh:波贝甘狸藻]]";
		    
		    Assert.AreEqual(after, parser2.SortMetaData(before, "a"), "Even if RemoveStubs not run on ru-wiki, no excess whitespace left from meta data sorting");
		    
		    Variables.SetProjectLangCode("sl");
		    WikiRegexes.MakeLangSpecificRegexes();
		    
		    string beforeSl = @"Andy
{{botanist-stub}}
{{Persondata}}
[[Category:ABC]]", afterSl = @"Andy

{{botanist-stub}}

[[Category:ABC]]

{{Persondata}}";
		    Assert.AreEqual(afterSl, parser2.SortMetaData(beforeSl, "a"), "Sl sort order");
		    
		    Variables.SetProjectLangCode("de");
		    WikiRegexes.MakeLangSpecificRegexes();
		    
		    string beforeDe = @"Andy
{{botanist-stub}}
{{Personendaten}}
[[Category:ABC]]
[[en:Test]]", afterDe = @"Andy


{{botanist-stub}}

[[Category:ABC]]

{{Personendaten}}

[[en:Test]]";
		    Assert.AreEqual(afterDe, parser2.SortMetaData(beforeDe, "a"), "De sort order");
		    
		    Variables.SetProjectLangCode("it");
		    WikiRegexes.MakeLangSpecificRegexes();
		    
		    string beforeIt = @"Andy
{{botanist-stub}}
{{Persondata}}
[[Category:ABC]]
[[en:Test]]", afterIt = @"Andy

{{Persondata}}
[[Category:ABC]]


{{botanist-stub}}

[[en:Test]]";
		    Assert.AreEqual(afterIt, parser2.SortMetaData(beforeIt, "a"), "It sort order");
		    
		    Variables.SetProjectSimple("it", ProjectEnum.wikiquote);
		    string afterIt2 = @"Andy

{{Persondata}}

{{botanist-stub}}

[[Category:ABC]]

[[en:Test]]";
		    Assert.AreEqual(afterIt2, parser2.SortMetaData(beforeIt, "a"), "It wikiquote sort order");
		    
		    Variables.SetProjectSimple("en", ProjectEnum.wikipedia);
		    WikiRegexes.MakeLangSpecificRegexes();
		    #endif
		}
		
		[Test]
		public void CommentedReferences()
		{
		    const string CommentedRef = @"'''George May''' (3 May 1876 - 1955) was.

<!--
==References==
<references />
-->

==External links==
* {{CanParlbio|ID=6}}

{{Persondata <!-- Metadata: see [[Wikipedia:Persondata]]. -->
| NAME              =May, George
| ALTERNATIVE NAMES =
| SHORT DESCRIPTION = politician
| DATE OF BIRTH     =3 May 1876
| PLACE OF BIRTH    =Ontario
| DATE OF DEATH     = 1955
| PLACE OF DEATH    =
}}
{{DEFAULTSORT:Macleod, George}}
[[Category:1876 births]]
[[Category:1955 deaths]]";
		    
		    Assert.AreEqual(CommentedRef, parser2.SortMetaData(CommentedRef, "George May"));
		}
		
		[Test]
		public void ShortPagesMonitor()
		{
		    const string A = @"'''Seyyed Ahmadi''' ({{lang-fa|سيداحمدي‎}}) may refer to:
* [[Seyyed Ahmadi, Fars]]
* [[Seyyed Ahmadi, Hormozgan]]

{{geodis}}

{{Short pages monitor}}<!-- This long comment was added to the page to prevent it from being listed on Special:Shortpages. It and the accompanying monitoring template were generated via Template:Long comment. Please do not remove the monitor template without removing the comment as well.-->";
		    
		    Assert.AreEqual(A, parser2.SortMetaData(A, "Test"), "{{Short pages monitor}} kept at end of article text: template before");
		    
		    string B = @"'''Seyyed Ahmadi''' ({{lang-fa|سيداحمدي‎}}) may refer to:
* [[Seyyed Ahmadi, Fars]]
* [[Seyyed Ahmadi, Hormozgan]]

[[de:Foo]]

{{Short pages monitor}}<!-- This long comment -->";
		    Assert.AreEqual(B, parser2.SortMetaData(B, "Test"), "{{Short pages monitor}} kept at end of article text: interwiki before");
		    B = B.Replace("\r\n{{Short page", "{{Short page");
		    Assert.AreEqual(B, parser2.SortMetaData(B, "Test"), "Number of newlines before spm preserved");
		    Assert.AreEqual(B, parser2.SortMetaData(B.Replace("-->", "-->\r\n"), "Test"), "Page trim still done when spm present");		    
		}
	}
}
