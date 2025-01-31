using WikiFunctions;
using WikiFunctions.Parse;
using NUnit.Framework;
using NUnit.Framework.Legacy;

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
            Assert.That(MetaDataSorter.RemoveStubs(ref s), Is.Empty);
            Assert.That(s, Is.EqualTo("==foo==\r\nbar<ref name=\"123\"/>{{boz}}"));

            // should remove stubs, but not section stubs
            s = "{{foo}}{{some-stub}}{{foo-stub}}bar{{sect-stub}}{{not a|stub}}";
            Assert.That(MetaDataSorter.RemoveStubs(ref s), Is.EqualTo("{{some-stub}}\r\n{{foo-stub}}\r\n"));
            Assert.That(s, Is.EqualTo("{{foo}}bar{{sect-stub}}{{not a|stub}}"));

            // handle stubs with extra parameters e.g. date=
            s = "{{foo}}{{my-stub}}{{foo-stub|date=May 2012}}bar{{sect-stub}}{{not a|stub}}";
            Assert.That(MetaDataSorter.RemoveStubs(ref s), Is.EqualTo("{{my-stub}}\r\n{{foo-stub|date=May 2012}}\r\n"));

            // shouldn't fail
            s = "";
            Assert.That(MetaDataSorter.RemoveStubs(ref s), Is.Empty);
            Assert.That(s, Is.Empty);
            s = "{{stub}}";
            Assert.That(MetaDataSorter.RemoveStubs(ref s), Is.EqualTo("{{stub}}\r\n"));
            Assert.That(s, Is.Empty);

            // remove duplicate stubs
            s = "{{foo}}{{stub}}{{stub}}";
            Assert.That(MetaDataSorter.RemoveStubs(ref s), Is.EqualTo("{{stub}}\r\n"), "deduplication, same case");
            s = "{{foo}}{{Stub}}{{stub}}";
            Assert.That(MetaDataSorter.RemoveStubs(ref s), Is.EqualTo("{{Stub}}\r\n"), "deduplication, different case");

            s = @"<!-- {{stub}} -->";
            Assert.That(MetaDataSorter.RemoveStubs(ref s), Is.EqualTo(@"<!-- {{stub}} -->" + "\r\n"), "Extract commented out stubs");
            s = @"#REDIRECT [[X]]
<!--
'''X''' is a x in the [[x]] of [[x]] in [[x]]. It lies to the east of [[x]].
{{stub}}
-->";
            Assert.That(MetaDataSorter.RemoveStubs(ref s), Is.Empty, "Don't pull stubs out of comments");
        }

        [Test]
        public void RemoveDupelicateStubs()
        {
            string stub = @"{{foo stub}}", articleTextBack = "", articletext = stub + " " + stub;
            articleTextBack = parser2.SortMetaData(articletext, "test");

            ClassicAssert.IsTrue(WikiRegexes.Stub.Matches(articleTextBack).Count == 1);

            // don't remove if different capitalisation
            articletext = stub + " " + @"{{fOO stub}}";
            articleTextBack = parser2.SortMetaData(articletext, "test");

            ClassicAssert.IsTrue(WikiRegexes.Stub.Matches(articleTextBack).Count == 2);

            // ignore stubs in comments
            articletext = stub + " " + @"<!--{{foo stub}}-->";
            articleTextBack = parser2.SortMetaData(articletext, "test");

            ClassicAssert.IsTrue(WikiRegexes.Stub.Matches(articleTextBack).Count == 2);

            // remove {{stub}} if more detailed stub
            string s = "{{foo}}{{stub}}{{foo-stub}}";
            Assert.That(MetaDataSorter.RemoveStubs(ref s), Is.EqualTo("{{foo-stub}}\r\n"), "removes {{stub}} if more detailed stub");
            s = "{{foo}}{{ Stub }}{{foo-stub}}";
            Assert.That(MetaDataSorter.RemoveStubs(ref s), Is.EqualTo("{{foo-stub}}\r\n"), "removes {{stub}} if more detailed stub");
            s = "{{foo}}{{-stub}}{{foo-stub}}";
            Assert.That(MetaDataSorter.RemoveStubs(ref s), Is.EqualTo("{{foo-stub}}\r\n"), "removes {{-stub}} if more detailed stub");
            s = "{{foo}}{{-stub}}";
            Assert.That(MetaDataSorter.RemoveStubs(ref s), Is.EqualTo("{{-stub}}\r\n"), "retain {{-stub}} if only stub tag");
            s = "{{foo}}{{uncategorized stub}}{{stub}}";
            Assert.That(MetaDataSorter.RemoveStubs(ref s), Is.EqualTo("{{uncategorized stub}}\r\n{{stub}}\r\n"), "retain {{stub}} if other tag is {{uncategorized stub}}");
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

            Assert.That(parser2.SortMetaData(correct, "no change"), Is.EqualTo(correct));
            Assert.That(parser2.SortMetaData(@"

[[Category:Pub chains in the United Kingdom]]
[[Category:Companies established in 1997]]



[[es:Punch Taverns]]
[[fr:Punch Taverns]]", "two newlines before interwikis"), Is.EqualTo(correct));

            Assert.That(parser2.SortMetaData(@"

[[Category:Pub chains in the United Kingdom]]
[[Category:Companies established in 1997]]
[[es:Punch Taverns]]

[[fr:Punch Taverns]]
", "an interwiki immediatelly after categories"), Is.EqualTo(correct));
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

            string c1 = a + b1,
            c2 = a + b2;

            MetaDataSorter.RemovePersonData(ref c1);
            Assert.That(c1, Is.EqualTo(""));
            MetaDataSorter.RemovePersonData(ref c2);
            Assert.That(c2, Is.EqualTo(""));
            MetaDataSorter.RemovePersonData(ref b1);
            Assert.That(b1, Is.EqualTo(""));

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

            Assert.That(f, Is.EqualTo(d1));
            Assert.That(e, Is.EqualTo(d2 + d3));

            e = d2 + d1 + d1.Replace("[[December 2]]", "") + d3;

            f = MetaDataSorter.RemovePersonData(ref e);

            Assert.That(f, Is.EqualTo(d1 + "\r\n" + d1.Replace("[[December 2]]", "")), "duplicate persondata: order not changed");

            string commented = @"<!--{{Persondata}}-->";
            Assert.That(MetaDataSorter.RemovePersonData(ref commented), Is.Empty, "commented out persondata not extracted");
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
            Assert.That(MetaDataSorter.MoveTemplate(d + e, WikiRegexes.Dablinks), Is.EqualTo(e + "\r\n" + d));

            Assert.That(MetaDataSorter.MoveTemplate(d + e + e, WikiRegexes.Dablinks), Is.EqualTo(e + "\r\n" + d), "Deduplication of identical tags");

            e = @"{{For|Fred the dancer|Fred Smith (dancer)}}";
            Assert.That(MetaDataSorter.MoveTemplate(d + e, WikiRegexes.Dablinks), Is.EqualTo(e + "\r\n" + d), "odering plus add newline");

            Assert.That(MetaDataSorter.MoveTemplate(e + " " + d, WikiRegexes.Dablinks), Is.EqualTo(e + "\r\n" + d), "whitepsace handling");

            e = @"{{redirect2|Fred the dancer|Fred Smith (dancer)}}";
            Assert.That(MetaDataSorter.MoveTemplate(d + e, WikiRegexes.Dablinks), Is.EqualTo(e + "\r\n" + d), "odering plus add newline, redirect2");

            e = @"{{redirect2|Fred the {{dancer}}|Fred Smith (dancer)}}";
            Assert.That(MetaDataSorter.MoveTemplate(d + e, WikiRegexes.Dablinks), Is.EqualTo(e + "\r\n" + d), "odering plus add newline, redirect2, nested");

            e = @"{{redirect2|Fred the {{dancer}}|Fred Smith (dancer)}}
";
            Assert.That(MetaDataSorter.MoveTemplate(d + ":" + e, WikiRegexes.Dablinks), Is.EqualTo(e + d), "colons before dablinks removed");

            // don't move dablinks in a section
            const string f = @"Article words
== heading ==
{{redirect2|Fred the dancer|Fred Smith (dancer)}}
words";
            Assert.That(MetaDataSorter.MoveTemplate(f, WikiRegexes.Dablinks), Is.EqualTo(f));

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#String_cannot_be_of_zero_length._Parameter_name:_oldValue_.2F_ArgumentException_in_MetaDataSorter.MoveDablinks
            Assert.That(MetaDataSorter.MoveTemplate(@"[[Category:Confederate Navy officers|Captains]]", WikiRegexes.Dablinks), Is.EqualTo(@"[[Category:Confederate Navy officers|Captains]]"));

            const string g = @"Some words";
            const string h = @"==heading==
more words
[[Category:Foo]]";
            e = @"{{redirect2|Fred the {{dancer}}|Fred Smith (dancer)}}";

            Assert.That(MetaDataSorter.MoveTemplate(g + "\r\n" + e + "\r\n" + h, WikiRegexes.Dablinks), Is.EqualTo(e + "\r\n" + g + "\r\n" + h));

            Assert.That(MetaDataSorter.MoveTemplate(d + e + e0, WikiRegexes.Dablinks), Is.EqualTo(e + "\r\n" + e0 + "\r\n" + d), "multi tag ordering");
            Assert.That(MetaDataSorter.MoveTemplate(e + e0 + "\r\n\r\n" + d, WikiRegexes.Dablinks), Is.EqualTo(e + "\r\n" + e0 + "\r\n\r\n" + d), "Don't affect whitespace after tags");

            // check spacing
            Assert.That(MetaDataSorter.MoveTemplate(e + "\r\n" + d, WikiRegexes.Dablinks), Is.EqualTo(e + "\r\n" + d));
            Assert.That(MetaDataSorter.MoveTemplate(e + "\r\n" + e0 + d, WikiRegexes.Dablinks), Is.EqualTo(e + "\r\n" + e0 + "\r\n" + d));
            Assert.That(MetaDataSorter.MoveTemplate(e + "\r\n" + e0 + "\r\n" + d, WikiRegexes.Dablinks), Is.EqualTo(e + "\r\n" + e0 + "\r\n" + d));
            Assert.That(MetaDataSorter.MoveTemplate(e + "\r\n" + e0 + "\r\n\r\n" + d, WikiRegexes.Dablinks), Is.EqualTo(e + "\r\n" + e0 + "\r\n\r\n" + d));
            Assert.That(MetaDataSorter.MoveTemplate(e + "\r\n" + e0 + e0 + "\r\n\r\n" + d, WikiRegexes.Dablinks), Is.EqualTo(e + "\r\n" + e0 + "\r\n\r\n" + d), "Dupe tag removal");

            // short description above dablink per MOS:ORDER
            Assert.That(MetaDataSorter.MoveTemplate(e + "\r\n" + @"{{short description|foo}}" + "\r\n" + d, WikiRegexes.ShortDescriptionTemplate), Is.EqualTo(@"{{short description|foo}}" + "\r\n" + e + "\r\n" + d));
            const string shortDescNone = @"{{Short description|none}} <!-- ""none"" is preferred when the title is sufficiently descriptive; see [[WP:SDNONE]] -->";
            Assert.That(MetaDataSorter.MoveTemplate(e + "\r\n" + shortDescNone + "\r\n" + d, WikiRegexes.ShortDescriptionTemplate), Is.EqualTo(shortDescNone + "\r\n" + e + "\r\n" + d));

            // don't pull dablinks out of {{hatnote group}}
            const string i = @"{{Short description|American character encoding standard}}
{{hatnote group|
{{other uses}}
{{Distinguish|text=MS [[Windows-1252]] or other types of [[Extended ASCII]]}}
}}
{{Use mdy dates|date=June 2013|cs1-dates=y}}
Article";

            Assert.That(parser2.SortMetaData(i, "Don't pull hatnotes out of hatnote group"), Is.EqualTo(i));
        }

        [Test]
        public void UseDatesTemplates()
        {
            const string correct = @"{{Short description|American character}}
{{other uses}}
{{featured article}}
{{Prod blp}}
{{Use mdy dates}}
{{Use American English}}
{{Infobox actor}}
Hello";

            Assert.That(parser2.SortMetaData(correct, "Title"), Is.EqualTo(correct), "Use mdy dates - already correctly ordered");

            Assert.That(parser2.SortMetaData(@"{{Use mdy dates}}
{{Short description|American character}}
{{other uses}}
{{featured article}}
{{Prod blp}}
{{Use American English}}
{{Infobox actor}}
Hello", "Title"), Is.EqualTo(correct), "Use mdy dates starting at top");

            Assert.That(parser2.SortMetaData(@"{{Use mdy dates}}
{{Use American English}}
{{Short description|American character}}
{{other uses}}
{{featured article}}
{{Prod blp}}
{{Infobox actor}}
Hello", "Title"), Is.EqualTo(correct), "Use mdy dates and American English starting at top");

            Assert.That(parser2.SortMetaData(@"{{featured article}}
{{Short description|American character}}
{{other uses}}
{{Use mdy dates}}
{{Use American English}}
{{Prod blp}}
{{Infobox actor}}
Hello", "Title"), Is.EqualTo(correct), "featured article starting at top");

            const string correct2 = @"{{Short description|American character}}
{{other uses}}
{{featured article}}
{{Prod blp}}
{{multiple issues|
{{Cleanup}}
{{Expert needed}}
}}
{{Use mdy dates}}
{{Use American English}}
{{Infobox actor}}
Hello";

            Assert.That(parser2.SortMetaData(correct2, "Title"), Is.EqualTo(correct2), "Already correctly ordered with use mdy, use American and MI");

            const string correct3 = @"{{Short description|American character with section word}}
{{other uses}}
{{featured article}}
{{Prod blp}}
{{Cleanup}}
{{Use mdy dates}}
{{Use American English}}
{{Infobox actor}}
Hello";

            Assert.That(parser2.SortMetaData(correct3, "Title"), Is.EqualTo(correct3), "Already correctly ordered with use mdy, use American and single maintenance template");

            const string correct4 = @"{{Short description|American character with section word}}
{{other uses}}
{{featured article}}
{{Prod blp}}
{{Cleanup}}
{{Use mdy dates}}
{{Use American English}}
{{Infobox actor|fiddles with articletitle}}
{{lowercase title}}
Hello";

            Assert.That(parser2.SortMetaData(correct4, "Title"), Is.EqualTo(correct4), "Already correct - {{lowercase title}} after infobox");

            const string correct5 = @"{{Short description|American character with section word}}
{{other uses}}
{{featured article}}
{{Prod blp}}
{{Cleanup}}
{{Use mdy dates}}
{{Use American English}}
{{lowercase title}}
{{Infobox actor|doesn't fiddle with articletitle}}
Hello";

            Assert.That(parser2.SortMetaData(correct5, "Title"), Is.EqualTo(correct5), "Already correct - {{lowercase title}} just before infobox");

            const string correct6 = @"{{Short description|Operating system for Apple computers}}
{{Lowercase title}}
{{Redirect2|OSX|OS X}}
{{About|macOS version 10.0 and later|Mac OS 9 and earlier}}
{{pp-semi-indef}}
{{Use mdy dates|date=August 2019}}
{{Infobox OS
| name = macOS
| support_status = Supported
}}
{{macOS sidebar}}

'''macOS''', originally.";

            Assert.That(parser2.SortMetaData(correct6, "MacOS"), Is.EqualTo(correct6), "Already correct - {{lowercase title}} after {{short description}}");

            Assert.That(parser2.SortMetaData(@"{{Short description|Operating system for Apple computers}}
{{Redirect2|OSX|OS X}}
{{Lowercase title}}
{{About|macOS version 10.0 and later|Mac OS 9 and earlier}}
{{pp-semi-indef}}
{{Use mdy dates|date=August 2019}}
{{Infobox OS
| name = macOS
| support_status = Supported
}}
{{macOS sidebar}}

'''macOS''', originally.", "MacOS"), Is.EqualTo(correct6), "Move {{lowercase title}} - incorrectly placed, not attached to infobox");

            const string correct7 = @"{{Short description|Operating system for Apple computers}}
{{Redirect2|OSX|OS X}}
{{About|macOS version 10.0 and later|Mac OS 9 and earlier}}
{{pp-semi-indef}}
{{Use mdy dates|date=August 2019}}
{{Lowercase title}}
{{Infobox OS
| name = macOS
| support_status = Supported
}}
{{macOS sidebar}}

'''macOS''', originally.";

            Assert.That(parser2.SortMetaData(correct7, "MacOS"), Is.EqualTo(correct7), "Already correct - {{lowercase title}} just before infobox");

            const string correct8 = @"{{Short description|Operating system for Apple computers}}
{{Redirect2|OSX|OS X}}
{{About|macOS version 10.0 and later|Mac OS 9 and earlier}}
{{pp-semi-indef}}
{{Use mdy dates|date=August 2019}}
{{Infobox OS
| name = macOS
| support_status = Supported
}}
{{Lowercase title}}
{{macOS sidebar}}

'''macOS''', originally.";

            Assert.That(parser2.SortMetaData(correct8, "MacOS"), Is.EqualTo(correct8), "Already correct - {{lowercase title}} after infobox");

            const string noinclude = @"
==Heading==
<noinclude>something</noinclude>
Text";
            Assert.That(parser2.SortMetaData(@"{{Use mdy dates}}
{{Use American English}}
{{Short description|American character}}
{{other uses}}
{{featured article}}
{{Prod blp}}
{{Infobox actor}}
Hello" + noinclude, "Title"), Is.EqualTo(correct + noinclude), "Zeroth section still sorted OK if later section has noinclude");

            const string zerothWithNoInclude = @"{{Use mdy dates}}
{{Use American English}}
{{Short description|American character}}
{{other uses}}
{{featured article}}
{{Prod blp}}
{{Infobox actor}}
<noinclude>something</noinclude>
Hello";
            Assert.That(parser2.SortMetaData(zerothWithNoInclude, "Title"), Is.EqualTo(zerothWithNoInclude), "Zeroth section NOT sorted if has noinclude");
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

            Assert.That(parser2.SortMetaData(dablink + "\r\n" + deletiontag + "\r\n" + maintenancetemp + "\r\n" + foo, "Foo"),
                Is.EqualTo(dablink + "\r\n" + deletiontag + "\r\n" + maintenancetemp + "\r\n" + foo), "no change if already OK");
            Assert.That(parser2.SortMetaData(deletiontag + "\r\n" + dablink + "\r\n" + maintenancetemp + "\r\n" + foo, "Foo"),
                Is.EqualTo(dablink + "\r\n" + deletiontag + "\r\n" + maintenancetemp + "\r\n" + foo), "dablink above deletion");
            Assert.That(parser2.SortMetaData(dablink + "\r\n" + maintenancetemp + "\r\n" + deletiontag + "\r\n" + foo, "Foo"),
                Is.EqualTo(dablink + "\r\n" + deletiontag + "\r\n" + maintenancetemp + "\r\n" + foo), "deletion above maintenance");
            
            deletiontag = @"{{Prod blp/dated}}";
            Assert.That(parser2.SortMetaData(dablink + "\r\n" + maintenancetemp + "\r\n" + deletiontag + "\r\n" + foo, "Foo"),
                Is.EqualTo(dablink + "\r\n" + deletiontag + "\r\n" + maintenancetemp + "\r\n" + foo), "deletion above maintenance, dated prod");

            deletiontag = @"{{Article for deletion/dated}} <!-- foo -->";
            Assert.That(parser2.SortMetaData(dablink + "\r\n" + maintenancetemp + "\r\n" + deletiontag + "\r\n" + foo, "Foo"),
                Is.EqualTo(dablink + "\r\n" + deletiontag + "\r\n" + maintenancetemp + "\r\n" + foo), "deletion above maintenance, AfD with comment");

            Assert.That(parser2.SortMetaData(dablink + "\r\n" + mi + "\r\n" + deletiontag + "\r\n" + infobox + "\r\n" + foo, "Foo"),
                Is.EqualTo(dablink + "\r\n" + deletiontag + "\r\n" + mi + "\r\n" + infobox + "\r\n" + foo), "deletion above multipleissues and mi above infobox");

            deletiontag = @"{{Prod blp/dated|concern=|month=September|day=2|year=2015|time=13:57|timestamp=2015|user=}} <!-- Do not use the ""prod blp/dated"" template directly; the above line is generated by ""subst:prod blp|reason"" -->";
            Assert.That(parser2.SortMetaData(dablink + "\r\n" + maintenancetemp + "\r\n" + deletiontag + "\r\n" + foo, "Foo"),
                Is.EqualTo(dablink + "\r\n" + deletiontag + "\r\n" + maintenancetemp + "\r\n" + foo), "deletion above maintenance, dated prod and comment");

            deletiontag = @"<!-- foo -->
{{Article for deletion/dated}} <!-- foo2 -->
<!-- 
foo3 
-->";
            Assert.That(parser2.SortMetaData(dablink + "\r\n" + maintenancetemp + "\r\n" + deletiontag + "\r\n" + foo, "Foo"),
                Is.EqualTo(dablink + "\r\n" + deletiontag + "\r\n" + maintenancetemp + "\r\n" + foo), "deletion above maintenance, dated prod with multiple comments");

            maintenancetemp = @"{{unreferenced|date=March 2024}}";
            deletiontag = @"<!-- Please do not remove or change this AfD message until the discussion has been closed. -->
<!-- The nomination page for this article already...
-->{{Article for deletion/dated|page=Foo|timestamp=20240331140235|year=2024|month=March|day=31|substed=yes}}
<!-- Once discussion is closed, please place on talk page: {{Old AfD multi|page=Foo|date=31 March 2024|result='''keep'''}} -->
<!-- End of AfD message, feel free to edit beyond this point -->";
            Assert.That(parser2.SortMetaData(dablink + "\r\n" + deletiontag + maintenancetemp + "\r\n" + foo, "Foo"),
                Is.EqualTo(dablink + "\r\n" + deletiontag + "\r\n" + maintenancetemp + "\r\n" + foo), "deletion then maintenance, dated prod with multiple comments inc newlines");
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
            Assert.That(MetaDataSorter.MoveTemplate(d + e, WikiRegexes.Dablinks), Is.EqualTo(d + e));

            const string f = @"{{for|the book by René Descartes|The World (Descartes)}}
<!--
Might eventually be relevant on a disambig page?  But these are a redirect and a not direct reference, so they're pretty irrelevant for now.
{{For|the song by the [[Thievery Corporation]]|Le Monde (song)}}
{{For|the drum & bass dj [[Lemon D]]| Lemon D}}
-->";

            Assert.That(MetaDataSorter.MoveTemplate(f, WikiRegexes.Dablinks), Is.EqualTo(f));
        }

        [Test]
        public void RemoveDisambig()
        {
            const string dab = @"{{dab}}", foo = @"foo";

            string a = dab + "\r\n" + foo;

            Assert.That(MetaDataSorter.RemoveDisambig(ref a), Is.EqualTo(dab));
            Assert.That(a, Is.EqualTo("\r\n" + foo));

            // no dabs to take out – no change
            a = foo;
            Assert.That(MetaDataSorter.RemoveDisambig(ref a), Is.Empty);
            Assert.That(a, Is.EqualTo(foo));

            // don't pull out of comments
            a = @"<!--" + dab + @"-->" + "\r\n" + foo;
            Assert.That(MetaDataSorter.RemoveDisambig(ref a), Is.Empty);
            Assert.That(a, Is.EqualTo(@"<!--" + dab + @"-->" + "\r\n" + foo));

            a = dab + "<!--comm-->\r\n" + foo;
            Assert.That(MetaDataSorter.RemoveDisambig(ref a), Is.EqualTo(dab + "<!--comm-->"), "Handles disambig template with comment after");
            Assert.That(a, Is.EqualTo("\r\n" + foo));
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
            Assert.That(MetaDataSorter.MoveTemplate(d + e, WikiRegexes.MaintenanceTemplates), Is.EqualTo(e + "\r\n" + d));

            e = @"{{underlinked|date=May 2008}}";
            Assert.That(MetaDataSorter.MoveTemplate(d + e, WikiRegexes.MaintenanceTemplates), Is.EqualTo(e + "\r\n" + d));

            e = @"{{cleanup|date=May 2008}}";
            Assert.That(MetaDataSorter.MoveTemplate(d + e, WikiRegexes.MaintenanceTemplates), Is.EqualTo(e + "\r\n" + d), "template moved when not a section one");

            e = @"{{cleanup|section|date=May 2008}}";
            Assert.That(MetaDataSorter.MoveTemplate(d + e, WikiRegexes.MaintenanceTemplates), Is.EqualTo(d + e), "section templates not moved");

            // don't move above other maintenance templates
            string f = @"{{cleanup|date=June 2009}}
" + e + d;
            Assert.That(MetaDataSorter.MoveTemplate(f, WikiRegexes.MaintenanceTemplates), Is.EqualTo(f));

            string g = @"{{BLP unsourced|date=August 2009|bot=yes}}
{{Underlinked|date=February 2008}}
'''Charles M. McKim'''";

            Assert.That(MetaDataSorter.MoveTemplate(g, WikiRegexes.MaintenanceTemplates), Is.EqualTo(g));

            // do move above infoboxes
            string h1 = @"{{Infobox foo| sdajklfsdjk | dDJfsdjkl }}", h2 = @"{{Underlinked|date=February 2008}}", h3 = @"'''Charles M. McKim'''";

            Assert.That(MetaDataSorter.MoveTemplate(h2 + "\r\n" + h1 + "\r\n" + h3, WikiRegexes.MaintenanceTemplates), Is.EqualTo(h2 + "\r\n" + h1 + "\r\n" + h3));
            Assert.That(MetaDataSorter.MoveTemplate(h1 + "\r\n" + h2 + "\r\n\r\n" + h3, WikiRegexes.MaintenanceTemplates), Is.EqualTo(h2 + "\r\n" + h1 + "\r\n\r\n" + h3));

            // do move above infoboxes
            h2 = @"{{Notability|Web|date=February 2008}}";

            Assert.That(MetaDataSorter.MoveTemplate(h2 + "\r\n" + h1 + "\r\n" + h3, WikiRegexes.MaintenanceTemplates), Is.EqualTo(h2 + "\r\n" + h1 + "\r\n" + h3));
            Assert.That(MetaDataSorter.MoveTemplate(h1 + "\r\n" + h2 + "\r\n\r\n" + h3, WikiRegexes.MaintenanceTemplates), Is.EqualTo(h2 + "\r\n" + h1 + "\r\n\r\n" + h3));

            string i1 = @"{{cleanup|date=June 2009}}";
            e = @"{{underlinked|date=May 2008}}";

            // move when tags not all at top
            Assert.That(MetaDataSorter.MoveTemplate(e + "\r\nfoo\r\n" + i1, WikiRegexes.MaintenanceTemplates), Is.EqualTo(e + "\r\n" + i1 + "\r\nfoo\r\n"));

            const string CommentedOut = @"<!---sufficient-?---{{Cleanup|date=January 2010}}--{{Wikify|date=January 2010}}---?--->";
            Assert.That(MetaDataSorter.MoveTemplate(CommentedOut, WikiRegexes.MaintenanceTemplates), Is.EqualTo(CommentedOut), "no change to commented out tags");

            const string NewMultipleIssues = @"{{multiple issues|{{Cleanup|date=January 2010}}}}";
            Assert.That(parser2.SortMetaData(NewMultipleIssues, "A"), Is.EqualTo(NewMultipleIssues), "don't pull tags from new-style {{multiple issues}}");

            // zeroth section only
            const string LaterSection = @"hello
==Sec==
{{cleanup|date=May 2012}}
End";
            Assert.That(parser2.SortMetaData(LaterSection, "A"), Is.EqualTo(LaterSection), "maintenance tags outside of zeroth section not moved");
        }

        [Test]
        public void MoveMaintenanceTagsDupes()
        {
            const string d = @"Fred is a doctor.
Fred has a dog.
[[Category:Dog owners]]
{{some template}}";
            string e = @"{{Underlinked|date=May 2008}}";
            Assert.That(MetaDataSorter.MoveTemplate(d + "\r\n" + e + "\r\n" + e, WikiRegexes.MaintenanceTemplates), Is.EqualTo(e + "\r\n" + d + "\r\n"), "move and dedupe");

            Assert.That(MetaDataSorter.MoveTemplate(e + "\r\n" + e + "\r\n" + d, WikiRegexes.MaintenanceTemplates), Is.EqualTo(e + "\r\n" + d), "dedupe when move not required");
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

            Assert.That(MetaDataSorter.MoveMultipleIssues(@"{{infobox foo}}
{{multiple issues|
{{orphan}}
{{cleanup}}
}}
Fred has a dog.
[[Category:Dog owners]]"), Is.EqualTo(correct), "mi moved above infobox");

            Assert.That(MetaDataSorter.MoveMultipleIssues(correct), Is.EqualTo(correct), "no change when already correct");

            const string commented = @"{{infobox foo}}
<!--{{multiple issues|
{{orphan}}
{{cleanup}}
}}-->
Fred has a dog.
[[Category:Dog owners]]";
            Assert.That(MetaDataSorter.MoveMultipleIssues(commented), Is.EqualTo(commented), "commented out infobox so nothing to do");

            const string noinfobox = @"{{foo}}
{{multiple issues|
{{orphan}}
{{cleanup}}
}}
Fred has a dog.
[[Category:Dog owners]]";
            Assert.That(MetaDataSorter.MoveMultipleIssues(noinfobox), Is.EqualTo(noinfobox), "nothing to move if no infobox");

            const string noMI = @"{{infobox foo}}
{{something|
{{one}}
{{two}}
}}
Fred has a dog.
[[Category:Dog owners]]";
            Assert.That(MetaDataSorter.MoveMultipleIssues(noMI), Is.EqualTo(noMI), "nothing to move if no MultipleIssues");
        }

        [Test]
        public void MovePortalTemplatesTests()
        {
            Assert.That(MetaDataSorter.MovePortalTemplates(@"text here
{{Portal|Football}}
text here2
== see also ==
some words"), Is.EqualTo(@"text here
text here2
== see also ==
{{Portal|Football}}
some words"), "example 1");

            Assert.That(MetaDataSorter.MovePortalTemplates(@"text here {{Portal|Football}}
text here2
== see also ==
some words"), Is.EqualTo(@"text here 
text here2
== see also ==
{{Portal|Football}}
some words"), "example 2");

            Assert.That(MetaDataSorter.MovePortalTemplates(@"text here
{{Portal|Football}}
text here2
== see also ==
some words"), Is.EqualTo(@"text here
text here2
== see also ==
{{Portal|Football}}
some words"), "whitespace at end of line after portal template");

            Assert.That(MetaDataSorter.MovePortalTemplates(@"text here
{{Portal|Sport}}
{{Portal|Football}}
text here2
== see also ==
some words"), Is.EqualTo(@"text here
text here2
== see also ==
{{Portal|Football}}
{{Portal|Sport}}
some words"), "example 3");

            Assert.That(MetaDataSorter.MovePortalTemplates(@"{{Portal|Football}}
text here
text here2
== see also ==
some words"), Is.EqualTo(@"text here
text here2
== see also ==
{{Portal|Football}}
some words"), "example 4");

            Assert.That(MetaDataSorter.MovePortalTemplates(@"text here
{{Portal|abc}}
text here2
== see also==
* Fred"), Is.EqualTo(@"text here
text here2
== see also==
{{Portal|abc}}
* Fred"), "example 4");

            Assert.That(MetaDataSorter.MovePortalTemplates(@"text here
{{Portal|Football}}
text here2
== see also ==
* Fred
== hello ==
some words"), Is.EqualTo(@"text here
text here2
== see also ==
{{Portal|Football}}
* Fred
== hello ==
some words"), "example 5");

            Assert.That(MetaDataSorter.MovePortalTemplates(@"text here
{{Portal|Football}}
text here2
== see also ==
* Fred
=== hello ===
some words"), Is.EqualTo(@"text here
text here2
== see also ==
{{Portal|Football}}
* Fred
=== hello ===
some words"), "example 6");

            Assert.That(MetaDataSorter.MovePortalTemplates(@"text here
text here2
== see also ==
{{Portal|Football}}
some words"), Is.EqualTo(@"text here
text here2
== see also ==
{{Portal|Football}}
some words"), "if portal is already in 'see also', don't move it");

            Assert.That(MetaDataSorter.MovePortalTemplates(@"text here
text here2
== see also ==
* Fred
{{Portal|Football}}
some words"), Is.EqualTo(@"text here
text here2
== see also ==
* Fred
{{Portal|Football}}
some words"));

            Assert.That(MetaDataSorter.MovePortalTemplates(@"text here
text here2
== see also ==
* Fred
=== portals ===
{{Portal|Football}}
some words"), Is.EqualTo(@"text here
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
            Assert.That(MetaDataSorter.MoveTemplateToSeeAlsoSection(Commented, Tools.NestedTemplateRegex("ext")), Is.EqualTo(Commented));
        }

        [Test]
        public void MovePortalTemplatesTestsDuplicates()
        {

            // remove duplicate portals
            Assert.That(MetaDataSorter.MovePortalTemplates(@"text here
{{Portal|Football}}
{{Portal|Football}}
text here2
== see also ==
some words"), Is.EqualTo(@"text here
text here2
== see also ==
{{Portal|Football}}
some words"));

            Assert.That(MetaDataSorter.MovePortalTemplates(@"text here
{{Portal|Football}}
text here2
== see also ==
some words
==other==
{{Portal|Football}}"), Is.EqualTo(@"text here
text here2
== see also ==
{{Portal|Football}}
some words
==other==
"));

            Assert.That(MetaDataSorter.MovePortalTemplates(@"text here
text here2
== see also ==
{{Portal|Football}}
some words
==other==
{{Portal|Football}}"), Is.EqualTo(@"text here
text here2
== see also ==
{{Portal|Football}}
some words
==other==
"));
        }

        [Test]
        public void MoveSisterLinksTest()
        {
            const string WiktInExtLinks = @"text here
text here2
== External links ==
{{wiktionary}}
* Fred";

            Assert.That(MetaDataSorter.MoveSisterlinks(@"text here
{{wiktionary}}
text here2
== External links ==
* Fred
== other ==
x"), Is.EqualTo(@"text here
text here2
== External links ==
{{wiktionary}}
* Fred
== other ==
x"), "another section at end");

            Assert.That(MetaDataSorter.MoveSisterlinks(@"text here
{{Sister project links|Programming languages|wikt=programming language|b=Computer Programming|s=How to Think Like a Computer Scientist|commons=Category:Programming languages}}
text here2
== External links ==
* Fred
== other ==
x"), Is.EqualTo(@"text here
text here2
== External links ==
{{Sister project links|Programming languages|wikt=programming language|b=Computer Programming|s=How to Think Like a Computer Scientist|commons=Category:Programming languages}}
* Fred
== other ==
x"), "sister project links may contain many parameters");

            Assert.That(MetaDataSorter.MoveSisterlinks(@"text here
{{wiktionary}}
text here2
== External links ==
* Fred"), Is.EqualTo(WiktInExtLinks), "ext links is last section");

            Assert.That(MetaDataSorter.MoveSisterlinks(WiktInExtLinks), Is.EqualTo(WiktInExtLinks), "wikt already in ext links");
            const string CommentedOut = @"text here
<!--{{wiktionary}}-->
text here2
== External links ==
* Fred
== other ==
x";
            Assert.That(MetaDataSorter.MoveSisterlinks(CommentedOut), Is.EqualTo(CommentedOut), "no change if commented out");

            const string NoExtLinks = @"text here
{{wiktionary}}
text here2
== other ==
x";
            Assert.That(MetaDataSorter.MoveSisterlinks(NoExtLinks), Is.EqualTo(NoExtLinks), "no ext links sections");
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

            Assert.That(MetaDataSorter.MoveTemplateToReferencesSection(a + b + c + g, WikiFunctions.WikiRegexes.MoreNoFootnotes, true), Is.EqualTo(b + c + a + "\r\n" + g));
            Assert.That(MetaDataSorter.MoveTemplateToReferencesSection(a + b + d + g, WikiFunctions.WikiRegexes.MoreNoFootnotes, true), Is.EqualTo(b + d + a + "\r\n" + g));
            Assert.That(MetaDataSorter.MoveTemplateToReferencesSection(a + b + e + g, WikiRegexes.MoreNoFootnotes, true), Is.EqualTo(b + e + a + "\r\n" + g));
            Assert.That(MetaDataSorter.MoveTemplateToReferencesSection(a + b + e, WikiRegexes.MoreNoFootnotes, true), Is.EqualTo(b + e + a + "\r\n"));
            Assert.That(MetaDataSorter.MoveTemplateToReferencesSection(a2 + b + e, WikiRegexes.MoreNoFootnotes, true), Is.EqualTo(b + e + a2 + "\r\n"));
            Assert.That(MetaDataSorter.MoveTemplateToReferencesSection(a2 + b + e, WikiRegexes.MoreNoFootnotes, false), Is.EqualTo(b + e + a2 + "\r\n"));

            Assert.That(MetaDataSorter.MoveTemplateToReferencesSection(a + b + d.Replace("Notes", "Notes and references") + g, WikiRegexes.MoreNoFootnotes, true), Is.EqualTo(b + d.Replace("Notes", "Notes and references") + a + "\r\n" + g));

            // not moved if outside zeroth section
            Assert.That(MetaDataSorter.MoveTemplateToReferencesSection(f + c + g, WikiRegexes.MoreNoFootnotes, true), Is.EqualTo(f + c + g));
            Assert.That(MetaDataSorter.MoveTemplateToReferencesSection(f + d + g, WikiRegexes.MoreNoFootnotes, true), Is.EqualTo(f + d + g));
            Assert.That(MetaDataSorter.MoveTemplateToReferencesSection(f + e + g, WikiRegexes.MoreNoFootnotes, true), Is.EqualTo(f + e + g));

            // if duplicate sections, don't duplicate template
            Assert.That(MetaDataSorter.MoveTemplateToReferencesSection(a + b + c + c + g, WikiRegexes.MoreNoFootnotes, true), Is.EqualTo(b + c + a + "\r\n" + c + g));
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

            Assert.That(MetaDataSorter.MoveTemplateToReferencesSection(a + b + c + g, WikiRegexes.Ibid, true), Is.EqualTo(b + c + a + "\r\n" + g));
            Assert.That(MetaDataSorter.MoveTemplateToReferencesSection(a + b + d + g, WikiRegexes.Ibid, true), Is.EqualTo(b + d + a + "\r\n" + g));
            Assert.That(MetaDataSorter.MoveTemplateToReferencesSection(a + b + e + g, WikiRegexes.Ibid, true), Is.EqualTo(b + e + a + "\r\n" + g));
            Assert.That(MetaDataSorter.MoveTemplateToReferencesSection(a + b + e, WikiRegexes.Ibid, true), Is.EqualTo(b + e + a + "\r\n"));

            Assert.That(MetaDataSorter.MoveTemplateToReferencesSection(a + b + c + g, WikiRegexes.Ibid, false), Is.EqualTo(b + c + a + "\r\n" + g));
            Assert.That(MetaDataSorter.MoveTemplateToReferencesSection(a + b + d + g, WikiRegexes.Ibid, false), Is.EqualTo(b + d + a + "\r\n" + g));
            Assert.That(MetaDataSorter.MoveTemplateToReferencesSection(a + b + e + g, WikiRegexes.Ibid), Is.EqualTo(b + e + a + "\r\n" + g));
            Assert.That(MetaDataSorter.MoveTemplateToReferencesSection(a + b + e, WikiRegexes.Ibid), Is.EqualTo(b + e + a + "\r\n"));

            Assert.That(MetaDataSorter.MoveTemplateToReferencesSection(a + b + c + d + g, WikiRegexes.Ibid, false), Is.EqualTo(b + c + d + a + "\r\n" + g), "move to Notes ahead of References");
            Assert.That(MetaDataSorter.MoveTemplateToReferencesSection(b + d + a + "\r\n" + c + g, WikiRegexes.Ibid), Is.EqualTo(b + d + a + "\r\n" + c + g), "No change: ibid already in notes section");

            // outside zeroth section – okay
            Assert.That(MetaDataSorter.MoveTemplateToReferencesSection(f + c + g, WikiRegexes.Ibid, false), Is.EqualTo(@"'''Article''' words.
== section ==

words.
" + c + a + "\r\n" + g));

            e = @"== Sources ==
";
            Assert.That(MetaDataSorter.MoveTemplateToReferencesSection(a + b + e, WikiRegexes.Ibid), Is.EqualTo(b + e + a + "\r\n"));
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

            Assert.That(MetaDataSorter.MoveExternalLinks(a + "\r\n" + b + "\r\n" + c + "\r\n" + e), Is.EqualTo(a + "\r\n" + c + "\r\n" + b + "\r\n" + e));
            Assert.That(MetaDataSorter.MoveExternalLinks(a + "\r\n" + b + "\r\n" + c + "\r\n" + h), Is.EqualTo(a + "\r\n" + c + "\r\n" + b + "\r\n" + h));
            Assert.That(MetaDataSorter.MoveExternalLinks(a + "\r\n" + b + "\r\n" + c + "\r\n" + g), Is.EqualTo(a + "\r\n" + c + "\r\n" + b + "\r\n" + g));

            // no change if already correct
            Assert.That(MetaDataSorter.MoveExternalLinks(a + "\r\n" + c + "\r\n" + b), Is.EqualTo(a + "\r\n" + c + "\r\n" + b));
            Assert.That(MetaDataSorter.MoveExternalLinks(a + "\r\n" + c + "\r\n" + b + "\r\n" + g), Is.EqualTo(a + "\r\n" + c + "\r\n" + b + "\r\n" + g));

            // no change if end of references section is unclear
            Assert.That(MetaDataSorter.MoveExternalLinks(a + "\r\n" + b + "\r\n" + c + "\r\n" + f), Is.EqualTo(a + "\r\n" + b + "\r\n" + c + "\r\n" + f));

            // only matching level two headings following references
            Assert.That(MetaDataSorter.MoveExternalLinks(a + "\r\n" + b + "\r\n" + c + "\r\n" + d), Is.EqualTo(a + "\r\n" + b + "\r\n" + c + "\r\n" + d));

            // don't' move external links if would create ref after reflist
            string ExtLinkRef = a + "\r\n" + b + "<ref>foo</ref>\r\n" + c + "\r\n" + e;
            Assert.That(MetaDataSorter.MoveExternalLinks(ExtLinkRef), Is.EqualTo(ExtLinkRef));

            // Notes not References
            Assert.That(MetaDataSorter.MoveExternalLinks(a + "\r\n" + b + "\r\n" + c.Replace("References", "Notes") + "\r\n" + e), Is.EqualTo(a + "\r\n" + c.Replace("References", "Notes") + "\r\n" + b + "\r\n" + e), "Notes");
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

            Assert.That(MetaDataSorter.MoveSeeAlso(a + "\r\n" + c + "\r\n" + b + "\r\n" + d), Is.EqualTo(a + "\r\n" + b + "\r\n\r\n" + c + "\r\n" + d), "reorder");

            // no change when already correct
            Assert.That(MetaDataSorter.MoveSeeAlso(a + "\r\n" + b + "\r\n" + c + "\r\n" + d), Is.EqualTo(a + "\r\n" + b + "\r\n" + c + "\r\n" + d), "no change when already correct");

            const string TwoReferencesSection = a + "\r\n" + c + "\r\n" + b + "\r\n" + c + "\r\n" + d;
            Assert.That(MetaDataSorter.MoveSeeAlso(TwoReferencesSection), Is.EqualTo(TwoReferencesSection), "no change when two references sections");

            Assert.That(MetaDataSorter.MoveSeeAlso(a + "\r\n" + c.Replace("References", "Notes") + "\r\n" + b + "\r\n" + d), Is.EqualTo(a + "\r\n" + b + "\r\n\r\n" + c.Replace("References", "Notes") + "\r\n" + d), "reorder, Notes");
        }

        [Test]
        public void CategoriesForDeletion()
        {
            const string a = @"[[Category:Railway companies established in 1851]]";
            const string b1 = @"[[Category:Pages for deletion]]", b2 = @"[[Category:Categories for deletion]]", b3 = @"[[Category:Articles for deletion]]";

            string k = a + "\r\n" + b1;
            Assert.That(parser2.Sorter.RemoveCats(ref k, "test"), Is.EqualTo(a + "\r\n"), "Pages for deletion cat not kept");

            k = a + "\r\n" + b2;
            Assert.That(parser2.Sorter.RemoveCats(ref k, "test"), Is.EqualTo(a + "\r\n"), "Categories for deletion cat not kept");

            k = a + "\r\n" + b3;
            Assert.That(parser2.Sorter.RemoveCats(ref k, "test"), Is.EqualTo(a + "\r\n"), "Articles for deletion cat not kept");
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

            Assert.That(parser2.Sorter.RemoveCats(ref k, "test"), Is.EqualTo(j + "\r\n"), "comments between cats");

            // but don't grab any comment just after the last category
            string l = i + j + @"
<!--foo-->";
            Assert.That(parser2.Sorter.RemoveCats(ref l, "test"), Is.EqualTo(j + "\r\n"), "comment after cat");
            ClassicAssert.IsTrue(l.Contains("\r\n" + @"<!--foo-->"), "comment after cat 2");

            const string m = @"[[Category:American women writers]]
[[Category:Autism activists]]
<!--LBGT categories are not to be used on bios of living people unless they self-identify with the label and it is relevant to their public lives[[Category:Bisexual actors]]
[[Category:LGBT models]]
[[Category:LGBT people from the United States]]
[[Category:LGBT television personalities]]-->
[[Category:Parents of people on the autistic spectrum]]";

            string n = m;

            Assert.That(parser2.Sorter.RemoveCats(ref n, "test"), Is.EqualTo(m + "\r\n"), "cats not pulled out of comments");

            // comments on same line of category
            const string o = @"[[Category:Canadian Aviation Hall of Fame inductees]]
[[Category:Canadian World War I pilots]] <!-- If he was a Flying Ace, use the Canadian subcategory -->
[[Category:Canadian World War II pilots]] <!-- If he was a Flying Ace, use the subcategory -->
[[Category:Foo]]";

            string p = o;

            Assert.That(parser2.Sorter.RemoveCats(ref p, "test"), Is.EqualTo(o + "\r\n"), "comments on line");

            // comments beside a category are not moved
            string q = @"#REDIRECT [[Alton Railroad]]

";
            string r = @"{{DEFAULTSORT:Joliet Chicago  Railroad}}
[[Category:Predecessors of the Alton Railroad]]
[[Category:Railway companies established in 1855]]
[[Category:Railway companies disestablished in 1950]]<!--http://bulk.resource.org/courts.gov/c/F2/264/264.F2d.445.12430_1.html-->
[[Category:Defunct Illinois railroads]]";
            string s = q + r;
            Assert.That(parser2.Sorter.RemoveCats(ref s, "test"), Is.EqualTo(r + "\r\n"), "comments on line end");

            string r2 = @"{{DEFAULTSORT:Joliet Chicago  Railroad}}
[[Category:Predecessors of the Alton Railroad]]
[[Category:Railway companies established in 1855]]
[[Category:Railway companies disestablished in 1950]]
[[Category:Defunct Illinois railroads]]  <!--comm-->";
            s = q + r2;
            Assert.That(parser2.Sorter.RemoveCats(ref s, "test"), Is.EqualTo(r2 + "\r\n"), "comment after last cat on same line not moved");

            string bug1a = @"[[Category:Emory University]]
[[Category:Law schools in Georgia (U.S. state)]]
<!-- A work in progress to incorporate into nav
[[Category:Southern Law Schools]]
[[Category:South Atlantic Law Schools]]
 -->", bug1b = @"

{{lawschool-stub}}";

            string t = bug1a + bug1b;

            Assert.That(parser2.Sorter.RemoveCats(ref t, "test"), Is.EqualTo(bug1a + "\r\n"), "commented out cats");

            string bug1c = @"<!-- foo bar-->
{{info}}
text
";
            t = bug1c + bug1a + bug1b;

            Assert.That(parser2.Sorter.RemoveCats(ref t, "test"), Is.EqualTo(bug1a + "\r\n"), "commented out plus comments");

            string bug2 = @"{{The Surreal Life}}
<!--The 1951 birth date has been upheld in court, please do not add this category.[[Category:1941 births]]-->

[[Category:Living People]]
foo";

            ClassicAssert.IsFalse(parser2.Sorter.RemoveCats(ref bug2, "test").Contains(@"[[Category:1941 births]]"), "commented out and not");

            string nw = @"Hello
<nowiki>[[Category:LGBT people from the United States]]
[[Category:LGBT television personalities]]</nowiki>

[[Category:American women writers]]
[[Category:Autism activists]]
[[Category:Parents of people on the autistic spectrum]]";

            Assert.That(parser2.Sorter.RemoveCats(ref nw, "test"), Is.EqualTo(@"[[Category:American women writers]]
[[Category:Autism activists]]
[[Category:Parents of people on the autistic spectrum]]
"), "Don't pull cats from nowiki");
            nw = @"Hello
<nowiki>[[Category:LGBT people from the United States]]
[[Category:LGBT television personalities]]</nowiki>

[[Category:American women writers]]
[[Category:Autism activists]]
[[Category:Parents of people on the autistic spectrum]]";
            ClassicAssert.IsFalse(parser2.Sorter.RemoveCats(ref nw, "test").Contains(@"[[Category:LGBT people from the United States]]"), "Don't pull cats from nowiki 2");

            string iw1 = @"[[Category:Hampshire|  ]]
[[Category:Articles including recorded pronunciations (UK English)]]
[[Category:Non-metropolitan counties]]", iw2 = @"

<!--interwiki-->
[[af:Hampshire]]";
            string iwall = iw1 + iw2;

            Assert.That(parser2.Sorter.RemoveCats(ref iwall, "test"), Is.EqualTo(iw1 + "\r\n"), "don't pull the interwiki comment");
            ClassicAssert.IsTrue(iwall.Contains(@"<!--interwiki-->"), "don't pull the interwiki comment");

            string writers = @"[[Category:Writers from Alabama]]
[[Category:Writers from North Carolina]]
[[Category:Writers from North Carolina]]

<!-- novelists tree -->
[[Category:20th-century American novelists]]";
            Assert.That(parser2.Sorter.RemoveCats(ref writers, "test"),
            Is.EqualTo(@"[[Category:Writers from Alabama]]
[[Category:Writers from North Carolina]]

<!-- novelists tree -->
[[Category:20th-century American novelists]]" + "\r\n"), "Dupe cat removed if second has comment");

            string comm = @"<!--[[Category:foo]]-->" + "\r\n";
            Assert.That(parser2.Sorter.RemoveCats(ref comm, "test"), Is.Empty, "Don't move commented out cats");

            comm = @"#REDIRECT [[X]]
<!-- XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX 
 XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX 
 XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX 
 XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX 
 XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX 
 XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX 

[[Category:Foo]]
[[Category:Foo2]]
 -->";
            Assert.That(parser2.Sorter.RemoveCats(ref comm, "test"), Is.Empty, "Don't move commented out cats, large comment");

            string longComment = @"Hello
<!--{{Navboxes
|title=Candinho managerial positions
|list1=
{{Esporte Clube São Bento managers}}
{{Clube Atlético Juventus managers}}
{{Al-Hilal FC managers}}
{{Grêmio Foot-Ball Porto Alegrense managers}}
{{Santos Futebol Clube managers}}
{{Clube de Regatas do Flamengo managers}}
{{Fluminense Football Club managers}}
{{Esporte Clube Bahia managers}}
{{Clube Atlético Bragantino managers}}
{{Guarani Futebol Clube managers}}
{{Esporte Clube Vitória managers}}
{{Sport Club Corinthians Paulista managers}}
{{Associação Portuguesa de Desportos managers}}
{{Goiás Esporte Clube managers}}
{{Al-Ittihad managers}}
{{Brazil national football team managers}}
{{Sociedade Esportiva Palmeiras managers}}
}} [[Category:Pages where template include size is exceeded]]-->

{{DEFAULTSORT:Candinho}}
[[Category:1945 births]]
[[Category:Living people]]";
            
            ClassicAssert.IsFalse(parser2.Sorter.RemoveCats(ref longComment, "test").Contains(@"[[Category:Pages where template include size is exceeded]]"), "Don't pull out of long comment section");
        }

        [Test]
        public void CategoryDupeRemoval()
        {
            string cats = @"[[Category:One]]
[[Category:Two]]
[[Category:One]]";

            Assert.That(parser2.Sorter.RemoveCats(ref cats, "test"), Is.EqualTo(@"[[Category:One]]
[[Category:Two]]
"), "Duplicate category removed, no sortkey");

            cats = @"[[Category:One]]
[[Category:Two]]
[[Category:One]]
[[Category:One]]";

            Assert.That(parser2.Sorter.RemoveCats(ref cats, "test"), Is.EqualTo(@"[[Category:One]]
[[Category:Two]]
"), "Duplicate categories removed, no sortkey");

            cats = @"[[Category:One|A]]
[[Category:Two]]
[[Category:One|A]]";

            Assert.That(parser2.Sorter.RemoveCats(ref cats, "test"), Is.EqualTo(@"[[Category:One|A]]
[[Category:Two]]
"), "Duplicate category removed, same sortkey");

            cats = @"[[Category:One]] <!--comm-->
[[Category:Two]]
[[Category:One]]";

            Assert.That(parser2.Sorter.RemoveCats(ref cats, "test"), Is.EqualTo(@"[[Category:One]] <!--comm-->
[[Category:Two]]
"), "Duplicate category removed, first with comment");

            cats = @"[[Category:One|A]]
[[Category:Two]]
[[Category:One|B]]";

            Assert.That(parser2.Sorter.RemoveCats(ref cats, "test"), Is.EqualTo(@"[[Category:One|A]]
[[Category:Two]]
[[Category:One|B]]
"), "Duplicate category NOT removed, conflicting sortkey");

            cats = @"[[Category:One]] <!--comm-->
[[Category:Two]]
[[Category:One]] <!--comm2-->";

            Assert.That(parser2.Sorter.RemoveCats(ref cats, "test"), Is.EqualTo(@"[[Category:One]] <!--comm-->
[[Category:Two]]
[[Category:One]] <!--comm2-->
"), "Duplicate category NOT removed, conflicting comments");
            cats = @"[[Category:One|A]]
[[Category:Two]]
[[Category:One]]";
            Assert.That(parser2.Sorter.RemoveCats(ref cats, "test"), Is.EqualTo(@"[[Category:One|A]]
[[Category:Two]]
"), "Duplicate category removed, one without sortkey");

            cats = @"[[Category:One|A]]
[[Category:Two]]
[[Category:One]]";

            Assert.That(parser2.Sorter.RemoveCats(ref cats, "A"), Is.EqualTo(@"[[Category:One|A]]
[[Category:Two]]
"), "Duplicate category removed, one without sortkey");

            cats = @"[[Category:One|A]]
[[Category:Two]]
[[Category:One|a]]";

            Assert.That(parser2.Sorter.RemoveCats(ref cats, "test"), Is.EqualTo(@"[[Category:One|A]]
[[Category:Two]]
"), "Duplicate category removed, same sortkey ignoring first letter case");

            cats = @"[[Category:One|foo]]
[[Category:Two]]
[[Category:One|foo]]";

            Assert.That(parser2.Sorter.RemoveCats(ref cats, "test"), Is.EqualTo(@"[[Category:One|foo]]
[[Category:Two]]
"), "Duplicate category removed, lowercase sortkey");

            cats = @"[[Category:One|Foo]]
[[Category:Two]]
[[Category:One|Foo]]";

            Assert.That(parser2.Sorter.RemoveCats(ref cats, "test"), Is.EqualTo(@"[[Category:One|Foo]]
[[Category:Two]]
"), "Duplicate category removed, uppercase sortkey");

            cats = @"[[Category:One|Foo Bar]]
[[Category:Two]]
[[Category:One|Foo]]";

            Assert.That(parser2.Sorter.RemoveCats(ref cats, "test"), Is.EqualTo(@"[[Category:One|Foo Bar]]
[[Category:Two]]
"), "Duplicate category removed, one sortkey substring of another");

            cats = @"[[Category:One|Foo]]
[[Category:Two]]
[[Category:One|Foo Bar]]";

            Assert.That(parser2.Sorter.RemoveCats(ref cats, "test"), Is.EqualTo(@"[[Category:Two]]
[[Category:One|Foo Bar]]
"), "Duplicate category removed, one sortkey substring of another, longer second");

            cats = @"[[Category:One Foo]]
[[Category:Two]]
[[Category:One]]";

            Assert.That(parser2.Sorter.RemoveCats(ref cats, "test"), Is.EqualTo(@"[[Category:One Foo]]
[[Category:Two]]
[[Category:One]]
"), "No change, cat substring rule only applies to sortkeys, longer first");

            cats = @"[[Category:One]]
[[Category:Two]]
[[Category:One Foo]]";

            Assert.That(parser2.Sorter.RemoveCats(ref cats, "test"), Is.EqualTo(@"[[Category:One]]
[[Category:Two]]
[[Category:One Foo]]
"), "No change, cat substring rule only applies to sortkeys, longer second");
        }

        [Test]
        public void CategoryDupeUncatTagRemoval()
        {
            // dupe uncat tags
            string cats = @"[[Category:One]]
{{uncat|date=August 2014}}
{{uncat|date=August 2014}}
";

            Assert.That(parser2.Sorter.RemoveCats(ref cats, "test"), Is.EqualTo(@"[[Category:One]]
{{uncat|date=August 2014}}
"), "Duplicate uncat tags removed");
        }

        [Test]
        public void CategoryDupeRemovalWikis()
        {
            #if DEBUG
            Variables.SetProjectSimple("en", ProjectEnum.species);

            string cats = @"[[Category:One]]
[[Category:Two]]
[[Category:One]]";

            Assert.That(parser2.Sorter.RemoveCats(ref cats, "test"), Is.EqualTo(@"[[Category:One]]
[[Category:Two]]
"), "Duplicate category removed, no sortkey");

            Variables.SetProjectSimple("en", ProjectEnum.commons);

            cats = @"[[Category:One]]
[[Category:Two]]
[[Category:One]]";

            Assert.That(parser2.Sorter.RemoveCats(ref cats, "test"), Is.EqualTo(@"[[Category:One]]
[[Category:Two]]
"), "Duplicate category removed, no sortkey");

            Variables.SetProjectSimple("en", ProjectEnum.wikipedia);
            #endif
        }

        [Test]
        public void CategoryCommentTests()
        {
            const string cats = @"[[Category:Hampshire|  ]]
[[Category:Articles including recorded pronunciations (UK English)]]
[[Category:Non-metropolitan counties]]", comm = @"<!-- cat -->";

            string a = cats + "\r\n" + comm;

            Assert.That(parser2.Sorter.RemoveCats(ref a, "test"), Is.EqualTo(comm + "\r\n" + cats + "\r\n"));

            string b = @"Text
[[Category:One]]

==References==
{{reflist}}
[[Category:Two]]";
            Assert.That(parser2.Sorter.RemoveCats(ref b, "test"), Is.EqualTo("[[Category:One]]\r\n[[Category:Two]]\r\n"));
            Assert.That(b, Is.EqualTo("Text\r\n\r\n==References==\r\n{{reflist}}\r\n"), "Blank newline before heading retained");

b = @"A
==Text==
{{foo}}[[Category:One]]
{| class=table |}

==References==
{{reflist}}
[[Category:Two]]";
            Assert.That(parser2.Sorter.RemoveCats(ref b, "test"), Is.EqualTo("[[Category:One]]\r\n[[Category:Two]]\r\n"));
            Assert.That(b, Is.EqualTo("A\r\n==Text==\r\n{{foo}}\r\n{| class=table |}\r\n\r\n==References==\r\n{{reflist}}\r\n"), "Newline retained when category not on own line");
        }

        [Test]
        public void CategoryRedirects()
        {
            const string redirect = @"#REDIRECT [[Category:Foo]]";
            string a = redirect;

            Assert.That(parser2.Sorter.RemoveCats(ref a, "test"), Is.Empty);
            Assert.That(a, Is.EqualTo(redirect), "cats not pulled from redirect target");
        }

        [Test]
        public void CategoryUncatImproveCat()
        {
            string a = @"[[Category:Foo]]
{{improve categories}}
";

            Assert.That(parser2.Sorter.RemoveCats(ref a, "test"), Is.EqualTo(@"[[Category:Foo]]
{{improve categories}}
"));

            a = @"{{improve categories}}
[[Category:Foo]]
";
            Assert.That(parser2.Sorter.RemoveCats(ref a, "test"), Is.EqualTo(@"[[Category:Foo]]
{{improve categories}}
"));

            a = @"{{uncategorized}}
[[Category:Foo]]
";
            Assert.That(parser2.Sorter.RemoveCats(ref a, "test"), Is.EqualTo(@"[[Category:Foo]]
{{uncategorized}}
"));

            a = @"{{uncategorized}}
== Section ==
[[Category:Foo]]
";
            Assert.That(parser2.Sorter.RemoveCats(ref a, "test"), Is.EqualTo(@"[[Category:Foo]]
"), "Don't pull uncat out of zeroth section");

            a = @"A
== Section ==
{{improve categories}}
== Section 2 ==
[[Category:Foo]]
";
            Assert.That(parser2.Sorter.RemoveCats(ref a, "test"), Is.EqualTo(@"[[Category:Foo]]
"), "Don't pull {{improve categories}} out of earlier section");

            a = @"A
== Section ==
<!--{{improve categories}}-->
[[Category:Foo]]
";
            Assert.That(parser2.Sorter.RemoveCats(ref a, "test"), Is.EqualTo(@"[[Category:Foo]]
"), "Don't pull commented out template");

            a = @"A
== Section ==
{{improve categories}}
== Section 2 ==
{{improve categories}}
[[Category:Foo]]
";
            Assert.That(parser2.Sorter.RemoveCats(ref a, "test"), Is.EqualTo(@"[[Category:Foo]]
"), "Don't pull improve categories if in earlier and last section");
        }

        [Test]
        public void RemoveCatsKey()
        {
            string cats = @"[[Category:1980 births]]
[[Category:Local people]]", cats2 = @"[[Category:1980 births|Jones, Andrew]]
[[Category:Local people|Jones, Andrew]]";

            parser2.Sorter.AddCatKey = true;
            Assert.That(parser2.Sorter.RemoveCats(ref cats, "Andrew Jones"), Is.EqualTo(cats2 + "\r\n"));
            parser2.Sorter.AddCatKey = false;

            // Extract {{Uncategorized}}
            string at = @"Text.

== References ==
{{reflist}}
{{Uncategorized}}";
            Assert.That(parser2.Sorter.RemoveCats(ref at, "Andrew Jones"), Is.EqualTo(@"{{Uncategorized}}" + "\r\n"), "uncat moved");
            ClassicAssert.IsFalse(WikiRegexes.Uncat.IsMatch(at));
            at = @"Text.

<!--{{Uncategorized}}-->
== References ==
{{reflist}}";
            Assert.That(parser2.Sorter.RemoveCats(ref at, "Andrew Jones"), Is.Empty, "commented out uncat not moved");
            at = @"Text.

== References ==
{{reflist}}
{{Uncategorized stub}}";
            Assert.That(parser2.Sorter.RemoveCats(ref at, "Andrew Jones"), Is.EqualTo(@"{{Uncategorized stub}}" + "\r\n"), "uncat stub moved - redirect to uncat");
            at = @"{{multiple issues|
{{a}}
{{Uncategorized}}
{{b}}
}}
== References ==
{{reflist}}";
            ClassicAssert.IsTrue(parser2.SortMetaData(at, "Andrew Jones").Contains(@"{{multiple issues|
{{a}}
{{Uncategorized}}
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
            Assert.That(parser2.Sorter.RemoveCats(ref articleText, "test"), Is.EqualTo(l + "\r\n" + c + "\r\n"), "lifetime treated like DEFAULTSORT on sl-wiki");

            Variables.SetProjectLangCode("es");
            l = @"{{NF|1980||Nieto, Pablo}}";
            articleText = waffle + c + "\r\n" + l;

            // https://es.wikipedia.org/wiki/Plantilla:NF#Ubicaci%C3%B3n%20en%20los%20art%C3%ADculos
            Assert.That(parser2.Sorter.RemoveCats(ref articleText, "test"), Is.EqualTo(l + "\r\n" + c + "\r\n"), "NF treated like DEFAULTSORT on es-wiki");

            Variables.SetProjectLangCode("en");
            articleText = waffle + c + "\r\n" + l;
            Assert.That(parser2.Sorter.RemoveCats(ref articleText, "test"), Is.EqualTo(c + "\r\n"), "lifetime/NF is just another template on en-wiki");
            #endif
        }

        [Test]
        public void DefaultSortAndCommentTests()
        {
            string a = @"Foo", b = @"[[Category:Predecessors of the Alton Railroad]]",
            c = @"{{DEFAULTSORT:Joliet Chicago  Railroad}}", e = @"<!--{{DEFAULTSORT:Joliet Chicago  Railroad}}-->",
            d = a + "\r\n" + b + "\r\n" + c,
            f = a + "\r\n" + b + "\r\n" + e,
            g = c + "\r\n" + c;

            Assert.That(parser2.Sorter.RemoveCats(ref d, "test"), Is.EqualTo(c + "\r\n" + b + "\r\n"), "standard case, return defaultsort then category");
            Assert.That(parser2.Sorter.RemoveCats(ref f, "test"), Is.EqualTo(b + "\r\n"), "Don't return commented out defaultsort");
            Assert.That(parser2.Sorter.RemoveCats(ref g, "test"), Is.Empty, "does not modify page with multiple defaultsorts");

            d = a + "\r\n" + b + "\r\n" + c + " <!--foo -->";
            Assert.That(parser2.Sorter.RemoveCats(ref d, "test"), Is.EqualTo(c + " <!--foo -->\r\n" + b + "\r\n"), "Retain comment on same line as defaultsort");
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

            Assert.That(parser2.Sorter.Interwikis(ref a), Is.EqualTo(b + "\r\n"), "no changes");

            // comment handling
            string comm = @"<!-- other languages -->";
            a = @"[[de:Canadian National Railway]]
[[es:Canadian National]]
[[fr:Canadien National]]" + comm;
            Assert.That(parser2.Sorter.Interwikis(ref a), Is.EqualTo(comm + "\r\n" + b + "\r\n"));

            comm = @"<!--Interwikis-->";
            a = @"[[de:Canadian National Railway]]
[[es:Canadian National]]
[[fr:Canadien National]]" + comm;
            Assert.That(parser2.Sorter.Interwikis(ref a), Is.EqualTo(comm + "\r\n" + b + "\r\n"));

            comm = @"<!--Other wikis-->";
            a = @"[[de:Canadian National Railway]]
[[es:Canadian National]]
[[fr:Canadien National]]" + comm;
            Assert.That(parser2.Sorter.Interwikis(ref a), Is.EqualTo(comm + "\r\n" + b + "\r\n"));

            comm = @"<!-- interwiki links to this article in other languages, below -->";
            a = @"[[de:Canadian National Railway]]
[[es:Canadian National]]
[[fr:Canadien National]]" + comm;
            Assert.That(parser2.Sorter.Interwikis(ref a), Is.EqualTo(comm + "\r\n" + b + "\r\n"));

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_12#Interwiki_links_moved_out_of_comment
            string c = @"{{Canadianmetros}}

<!--
The following links are here to prevent the interwiki bot from adding them to the list above.  The links below point to disambiguation pages and not to a translated article about Canadian National Railway.
[[it:CN]]
[[sv:CN]]
-->";
            Assert.That(parser2.Sorter.Interwikis(ref c), Is.Empty, "interwikis not taken out of wiki comments");

            string c2 = c + @"[[sv:CN]]";
            Assert.That(parser2.Sorter.Interwikis(ref c2), Is.EqualTo(@"[[sv:CN]]" + "\r\n"), "interwiki returned even if the same one exists commented out elsewhere");

            c = @"{{Canadianmetros}}

<!--
The following links are here to prevent the interwiki bot from adding them to the list above.  The links below point to disambiguation pages and not to a translated article about Canadian National Railway.
[[it:CN]]
[[sv:CN]]
-->
second comment <!-- [[it:CN]] -->";
            Assert.That(parser2.Sorter.Interwikis(ref c), Is.Empty, "interwikis not taken out of wiki comments");

            // deduplication
            string d = @"[[de:Canadian National Railway]]
[[es:Canadian National]]
[[es:Canadian National]]
[[fr:Canadien National]]";

            Assert.That(parser2.Sorter.Interwikis(ref d), Is.EqualTo(b + "\r\n"));

            string e1 = @"{{DEFAULTSORT:Boleyn, Anne}}

", e2 = @"{{Link FA|bs}}
{{Link FA|no}}
{{Link FA|sv}}
[[ar:آن بولين]]
[[bs:Anne Boleyn]]
[[br:Anne Boleyn]]";

            string f = e2, g = e1 + e2;

            Assert.That(parser2.Sorter.Interwikis(ref g), Is.EqualTo(f + "\r\n"));

            string h = @"[[de:Canadian National Railway]]
[[en:Canadian National]]
[[fr:Canadien National]]";

            Assert.That(parser2.Sorter.Interwikis(ref h), Is.EqualTo(@"[[de:Canadian National Railway]]
[[fr:Canadien National]]" + "\r\n"), "interwikis to own wiki are removed");

            a = @"[[de:Canadian National Railway]]
[[es::Canadian National]]
[[fr:Canadien National]]";
            b = a.Replace("::", ":");

            Assert.That(parser2.Sorter.Interwikis(ref a), Is.EqualTo(b + "\r\n"), "double colon in interwiki removed");

            a = @"[[de:Canadian National Railway]]
[[es:Canadian National]]
[[es:canadian National]]
[[fr:Canadien National]]";
            b = @"[[de:Canadian National Railway]]
[[es:Canadian National]]
[[fr:Canadien National]]";

            Assert.That(parser2.Sorter.Interwikis(ref a), Is.EqualTo(b + "\r\n"), "duplicate interwiki removed, variation in first letter casing only");

            a = @"[[de:Canadian National Railway]]
[[es:Canadian National]]
[[es:Canadian National]]<!-- comm-->
[[fr:Canadien National]]";
            b = @"[[de:Canadian National Railway]]
[[es:Canadian National]]
[[fr:Canadien National]]";

            Assert.That(parser2.Sorter.Interwikis(ref a), Is.EqualTo(b + "\r\n"), "duplicate interwiki removed, inline comment second");

            a = @"[[de:Canadian National Railway]]
[[es:Canadian National]]<!-- comm-->
[[es:Canadian National]]
[[fr:Canadien National]]";
            b = @"[[de:Canadian National Railway]]
[[es:Canadian National]]<!-- comm-->
[[fr:Canadien National]]";

            Assert.That(parser2.Sorter.Interwikis(ref a), Is.EqualTo(b + "\r\n"), "duplicate interwiki removed, inline comment first");

            a = @"[[de:Canadian National Railway]]
[[jbo:canadian National]]
[[fr:Canadien National]]";

             Assert.That(parser2.Sorter.Interwikis(ref a), Is.EqualTo(@"[[de:Canadian National Railway]]
[[jbo:canadian National]]
[[fr:Canadien National]]
"), "first letter casing retained for jbo-wiki links");

            string i = @"{{Canadianmetros|
[[it:CN]]
[[sv:CN]]
}}";
            Assert.That(parser2.Sorter.Interwikis(ref i), Is.Empty, "interwikis not taken out of templates");
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

            Assert.That(parser2.Sorter.Interwikis(ref a), Is.EqualTo(b + "\r\n"));

            a = @"[[fr:Sénatus-consultes sous Napoléon III]]
[[fr:Sénatus-consulte]]";
            b = a;

            Assert.That(parser2.Sorter.Interwikis(ref a), Is.EqualTo(b + "\r\n"));
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
            Assert.That(parser2.Sorter.Interwikis(ref a), Is.EqualTo(b + "\r\n"), "reorder if neccessary");

            a = @"[[ru:Bar]]
[[sq:Foo]]";
            Assert.That(parser2.Sorter.Interwikis(ref a), Is.EqualTo(b + "\r\n"), "no change Diff everything is sorted");

            parser2.InterWikiOrder = InterWikiOrderEnum.Alphabetical;
            a = @"[[ru:Bar]]
[[sq:Foo]]";
            Assert.That(parser2.Sorter.Interwikis(ref a), Is.EqualTo(b + "\r\n"), "alphabetical order");

            parser2.InterWikiOrder = InterWikiOrderEnum.LocalLanguageFirstWord;
            a = @"[[ru:Bar]]
[[sq:Foo]]";
            Assert.That(parser2.Sorter.Interwikis(ref a), Is.EqualTo(b + "\r\n"), "local language");

            parser2.InterWikiOrder = InterWikiOrderEnum.AlphabeticalEnFirst;
            a = @"[[ru:Bar]]
[[sq:Foo]]";
            Assert.That(parser2.Sorter.Interwikis(ref a), Is.EqualTo(b + "\r\n"), "alphabetical order with en firsth");

            #if DEBUG
            Variables.SetProjectLangCode("de");
            parser2.InterWikiOrder = InterWikiOrderEnum.AlphabeticalEnFirst;
            a = @"[[ar:Bar]]
[[en:Hello]]
[[sq:Foo]]";
            b = @"[[en:Hello]]
[[ar:Bar]]
[[sq:Foo]]";
            Assert.That(parser2.Sorter.Interwikis(ref a), Is.EqualTo(b + "\r\n"));

            parser2.InterWikiOrder = InterWikiOrderEnum.Alphabetical;
            a = @"[[ar:Bar]]
[[en:Hello]]
[[sq:Foo]]";
            b = @"[[ar:Bar]]
[[en:Hello]]
[[sq:Foo]]";
            Assert.That(parser2.Sorter.Interwikis(ref a), Is.EqualTo(b + "\r\n"));

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

            Assert.That(parser2.Sorter.Interwikis(ref a), Is.EqualTo(b + "\r\n"), "inline comment kept with interwiki");
        }

        [Test]
        public void SelfInterwikisEn()
        {
            Assert.That(parser2.SortMetaData(@"<!-- [[en:Foo]]-->", "Test"), Is.Empty, "Commented out en interwikis removed");

            #if DEBUG
            const string EnInterwiki = @"[[en:Foo]]";

            Variables.SetProjectSimple("en", ProjectEnum.commons);
            Assert.That(parser2.SortMetaData(EnInterwiki, "Test"), Is.EqualTo("\r\n\r\n" + EnInterwiki), "en interwiki not removed on commons");

            Variables.SetProjectSimple("en", ProjectEnum.species);
            Assert.That(parser2.SortMetaData(EnInterwiki, "Test"), Is.EqualTo("\r\n\r\n" + EnInterwiki), "en interwiki not removed on species");

            Variables.SetProjectSimple("en", ProjectEnum.wikipedia);
            Assert.That(parser2.SortMetaData(EnInterwiki, "Test"), Is.Empty, "en interwiki removed on en-wiki");
            #endif
        }

        [Test]
        public void SorterNamespaceTests()
        {
            const string cat = @"[[Category:Foo]]";

            Assert.That(parser2.SortMetaData(cat, "Category:Bar"), Is.EqualTo(cat), "Full trim on category namespace");
            Assert.That(parser2.SortMetaData(cat, "Template:Bar"), Is.EqualTo(cat), "no sorting on template namespace");
            Assert.That(parser2.SortMetaData(cat, "Module:Bar"), Is.EqualTo(cat), "no sorting on Module namespace");
            Assert.That(parser2.SortMetaData(cat, "Bar"), Is.EqualTo("\r\n\r\n" + cat), "sorting applied on mainspace");

            const string CatPopStub = @"Text
{{popstub}}
Text";
            Assert.That(parser2.SortMetaData(CatPopStub, "Category:Foo"), Is.EqualTo(CatPopStub), "no stub sorting on Category namespace");
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

            Assert.That(parser2.SortMetaData(a + b + c, "foo"), Is.EqualTo(a + b + c));

            Assert.That(parser2.SortMetaData(b, "foo"), Is.Not.EqualTo(b));

            const string d = @"[[Category:Foo]]", e = @"[[Category:More foo]]", f = @"<includeonly>blah[[Category:Foo]]</includeonly>";

            Assert.That(parser2.SortMetaData(a + d + c + e, "foo"), Is.EqualTo(a + d + c + e));
            Assert.That(parser2.SortMetaData(d + e, "foo"), Is.Not.EqualTo(d + "\r\n" + e));
            Assert.That(parser2.SortMetaData(f + d, "foo"), Is.EqualTo(f + d));
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

            Assert.That(parser2.SortMetaData(before, "a"), Is.EqualTo(after), "Even if RemoveStubs not run on ru-wiki, no excess whitespace left from meta data sorting");

            Variables.SetProjectLangCode("uk");
            WikiRegexes.MakeLangSpecificRegexes();

            string beforeUK = @"Andy
{{botanist-stub}}
[[Category:ABC]]", afterUK = @"Andy


{{botanist-stub}}

[[Category:ABC]]";
            Assert.That(parser2.SortMetaData(beforeUK, "a"), Is.EqualTo(afterUK), "uk sort order: stubs NOT below categories");

            Variables.SetProjectLangCode("uk");
            WikiRegexes.MakeLangSpecificRegexes();

            string beforeDK = @"Andy
{{botanist-dk-stub}}
[[Category:ABC]]", afterDK = @"Andy


{{botanist-dk-stub}}

[[Category:ABC]]";
            Assert.That(parser2.SortMetaData(beforeDK, "a"), Is.EqualTo(afterDK), "dk sort order: stubs NOT below categories");

            Variables.SetProjectLangCode("sl");
            WikiRegexes.MakeLangSpecificRegexes();

            string beforeSl = @"Andy
{{botanist-stub}}
{{Persondata}}
[[Category:ABC]]", afterSl = @"Andy

{{botanist-stub}}

[[Category:ABC]]

{{Persondata}}";
            Assert.That(parser2.SortMetaData(beforeSl, "a"), Is.EqualTo(afterSl), "Sl sort order");

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
            Assert.That(parser2.SortMetaData(beforeDe, "a"), Is.EqualTo(afterDe), "De sort order");

            Variables.SetProjectLangCode("it");
            WikiRegexes.MakeLangSpecificRegexes();

            string beforeIt = @"{{S|foo}}

Andy
{{Persondata}}
[[Category:ABC]]
[[en:Test]]", afterIt = @"{{S|foo}}

Andy

{{Persondata}}
[[Category:ABC]]

[[en:Test]]";
            Assert.That(parser2.SortMetaData(beforeIt, "a"), Is.EqualTo(afterIt), "It sort order");

            Variables.SetProjectSimple("it", ProjectEnum.wikiquote);
            string afterIt2 = @"{{S|foo}}

Andy

{{Persondata}}
[[Category:ABC]]

[[en:Test]]";
            Assert.That(parser2.SortMetaData(beforeIt, "a"), Is.EqualTo(afterIt2), "It wikiquote sort order");

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

            Assert.That(parser2.SortMetaData(CommentedRef, "George May"), Is.EqualTo(CommentedRef));
        }

        [Test]
        public void ShortPagesMonitor()
        {
            const string A = @"'''Seyyed Ahmadi''' ({{lang-fa|سيداحمدي‎}}) may refer to:
* [[Seyyed Ahmadi, Fars]]
* [[Seyyed Ahmadi, Hormozgan]]

{{geodis}}

{{Short pages monitor}}<!-- This long comment was added to the page to prevent it from being listed on Special:Shortpages. It and the accompanying monitoring template were generated via Template:Long comment. Please do not remove the monitor template without removing the comment as well.-->";

            Assert.That(parser2.SortMetaData(A, "Test"), Is.EqualTo(A), "{{Short pages monitor}} kept at end of article text: template before");

            string B = @"'''Seyyed Ahmadi''' ({{lang-fa|سيداحمدي‎}}) may refer to:
* [[Seyyed Ahmadi, Fars]]
* [[Seyyed Ahmadi, Hormozgan]]

[[de:Foo]]

{{Short pages monitor}}<!-- This long comment -->";
            Assert.That(parser2.SortMetaData(B, "Test"), Is.EqualTo(B), "{{Short pages monitor}} kept at end of article text: interwiki before");
            B = B.Replace("\r\n{{Short page", "{{Short page");
            Assert.That(parser2.SortMetaData(B, "Test"), Is.EqualTo(B), "Number of newlines before spm preserved");
            Assert.That(parser2.SortMetaData(B.Replace("-->", "-->\r\n"), "Test"), Is.EqualTo(B), "Page trim still done when spm present");
        }

        [Test]
        public void RedirectsDablinks()
        {
            string x = @"#REDIRECT [[Foo]]

{{distinguish|A}}";
            Assert.That(parser2.SortMetaData(x, "Test"), Is.EqualTo(x), "No sorting of zeroth section templates for redirects");
        }
    }
}
