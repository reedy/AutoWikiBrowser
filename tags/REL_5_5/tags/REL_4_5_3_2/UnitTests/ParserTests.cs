/*
AWB unit tests
Copyright (C) 2008 Max Semenik

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

Portions Copyright © 2002-2007 Charlie Poole or 
Copyright © 2002-2004 James W. Newkirk, Michael C. Two, Alexei A. Vorontsov or 
Copyright © 2000-2002 Philip A. Craig

*/

using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using WikiFunctions;
using WikiFunctions.Parse;

namespace UnitTests
{
    /// <summary>
    /// This class must be inhertited by test fixtures that use most parts of WikiFunctions.
    /// It ensures that WikiFunctions is aware that it's being called from unit tests, and
    /// that global data is initialised.
    /// </summary>
    public class RequiresInitialization
    {
        int __dummy = Init();

        static int Init()
        {
            Globals.UnitTestMode = true;
            if (WikiRegexes.Category == null) WikiRegexes.MakeLangSpecificRegexes();

            return 0;
        }
    }

    public class RequiresParser : RequiresInitialization
    {
        protected Parsers parser;

        public RequiresParser()
        {
            parser = new Parsers();
        }
    }

    [TestFixture]
    public class FootnotesTests : RequiresInitialization
    {

        [Test]
        public void PrecededByEqualSign()
        {
            Assert.That(Parsers.FixFootnotes("a=<ref>b</ref>"), Text.DoesNotContain("\n"));
        }

        [Test]
        // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_6#Unexpected_modification
        public void TestTagBoundaries()
        {
            Assert.AreEqual("<ref name=\"foo\"><br></ref>", Parsers.SimplifyReferenceTags("<ref name=\"foo\"><br></ref>"));
        }

        [Test]
        public void TestSimplifyReferenceTags()
        {
            Assert.AreEqual("<ref name=\"foo\" />", Parsers.SimplifyReferenceTags("<ref name=\"foo\"></ref>"));
            Assert.AreEqual("<ref name=\"foo\" />", Parsers.SimplifyReferenceTags("<ref name=\"foo\" >< / ref >"));
            Assert.AreEqual("<ref name=\"foo\" />", Parsers.SimplifyReferenceTags("<ref name=\"foo\" ></ref>"));

            // don't use * quantifier for \s
            Assert.AreEqual("<refname=\"foo\"></ref>", Parsers.SimplifyReferenceTags("<refname=\"foo\"></ref>"));
        }

        [Test]
        // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_7#References-2column_not_replaced_with_2_argument_to_reflist
        public void TestFixReferenceListTags()
        {
            Assert.AreEqual("<references/>", Parsers.FixReferenceListTags("<references/>"));
            Assert.AreEqual("<div><references/></div>", Parsers.FixReferenceListTags("<div><references/></div>"));

            Assert.AreEqual("<references/>", Parsers.FixReferenceListTags("<references>"));
            Assert.AreEqual("<references/>", Parsers.FixReferenceListTags("<references></references>"));

            Assert.AreEqual("{{reflist}}", Parsers.FixReferenceListTags("<div class=\"references-small\"><references/>\r\n</div>"));
            Assert.AreEqual("{{reflist|2}}", Parsers.FixReferenceListTags("<div class=\"references-2column\"><references/></div>"));
            Assert.AreEqual("{{reflist|2}}",
                Parsers.FixReferenceListTags(@"<div class=""references-2column""><div class=""references-small"">
<references/></div></div>"));
            Assert.AreEqual("{{reflist|2}}",
                Parsers.FixReferenceListTags(@"<div class=""references-small""><div class=""references-2column""> <references/>
</div></div>"));

            // evil don't do's
            Assert.That(Parsers.FixReferenceListTags(@"<div class=""references-small""><div class=""references-2column"">
<references/></div>* some other ref</div>"), Is.Not.Contains("{{reflist"));
            Assert.That(Parsers.FixReferenceListTags(@"<div class=""references-small""><div class=""references-2column"">
<references/></div>"), Is.Not.Contains("{{reflist"));
        }

        [Test]
        public void TestFixReferenceTags()
        {
            // whitespace cleaning
            Assert.AreEqual(@"now <ref>[http://www.site.com a site]</ref> was", Parsers.FixReferenceTags(@"now < ref>[http://www.site.com a site]</ref> was"));
            Assert.AreEqual(@"now <ref>[http://www.site.com a site]</ref> was", Parsers.FixReferenceTags(@"now < ref   >[http://www.site.com a site]</ref> was"));
            Assert.AreEqual(@"now <ref>[http://www.site.com a site]</ref> was", Parsers.FixReferenceTags(@"now <ref >[http://www.site.com a site]</ref> was"));
            Assert.AreEqual(@"now <ref>[http://www.site.com a site]</ref> was", Parsers.FixReferenceTags(@"now < ref>[http://www.site.com a site]< /ref> was"));
            Assert.AreEqual(@"now <ref>[http://www.site.com a site]</ref> was", Parsers.FixReferenceTags(@"now <ref>[http://www.site.com a site]</ ref> was"));
            Assert.AreEqual(@"now <ref>[http://www.site.com a site]</ref> was", Parsers.FixReferenceTags(@"now <ref>[http://www.site.com a site]</ref > was"));

            // <ref name=foo bar> --> <ref name="foo bar">
            Assert.AreEqual(@"now <ref name=""foo bar""> and", Parsers.FixReferenceTags(@"now <ref name=foo bar> and"));
            Assert.AreEqual(@"now <ref name=""foo bar"" /> and", Parsers.FixReferenceTags(@"now <ref name=foo bar /> and"));
            Assert.AreEqual(@"now <ref name = ""foo bar"" > and", Parsers.FixReferenceTags(@"now <ref name = foo bar > and"));

            // <ref name=foo bar"> --> <ref name="foo bar">
            Assert.AreEqual(@"now <ref name=""foo bar""> and", Parsers.FixReferenceTags(@"now <ref name=foo bar""> and"));
            Assert.AreEqual(@"now <ref name=""foo bar"" /> and", Parsers.FixReferenceTags(@"now <ref name=foo bar"" /> and"));
            Assert.AreEqual(@"now <ref name = ""foo bar"" > and", Parsers.FixReferenceTags(@"now <ref name = foo bar"" > and"));

            // <ref name="foo bar> --> <ref name="foo bar">
            Assert.AreEqual(@"now <ref name=""foo bar""> and", Parsers.FixReferenceTags(@"now <ref name=""foo bar> and"));
            Assert.AreEqual(@"now <ref name=""foo bar"" /> and", Parsers.FixReferenceTags(@"now <ref name=""foo bar /> and"));
            Assert.AreEqual(@"now <ref name = ""foo bar"" > and", Parsers.FixReferenceTags(@"now <ref name = ""foo bar > and"));

            // <ref name = ''Fred'> --> <ref name="Fred"> (two apostrophes)
            Assert.AreEqual(@"now <ref name=""foo bar""> and", Parsers.FixReferenceTags(@"now <ref name=''foo bar'> and"));
            Assert.AreEqual(@"now <ref name=""foo bar"" /> and", Parsers.FixReferenceTags(@"now <ref name='foo bar'' /> and"));
            Assert.AreEqual(@"now <ref name = ""foo bar"" > and", Parsers.FixReferenceTags(@"now <ref name = ''foo bar'' > and"));

            // <ref name "foo bar"> --> <ref name="foo bar">
            Assert.AreEqual(@"now <ref name =""foo bar""> and", Parsers.FixReferenceTags(@"now <ref name ""foo bar""> and"));
            Assert.AreEqual(@"now <ref name =""foo bar"" /> and", Parsers.FixReferenceTags(@"now <ref name ""foo bar"" /> and"));

            Assert.AreEqual(@"now <ref name =""foo bar""> and", Parsers.FixReferenceTags(@"now <ref name -""foo bar""> and"));
            Assert.AreEqual(@"now <ref name =""foo bar"" /> and", Parsers.FixReferenceTags(@"now <ref name -""foo bar"" /> and"));
            Assert.AreEqual(@"now <ref name =  ""foo bar"" > and", Parsers.FixReferenceTags(@"now <ref name -  ""foo bar"" > and"));
            Assert.AreEqual(@"now <ref name =""foo bar""> and", Parsers.FixReferenceTags(@"now <ref name +""foo bar""> and"));
            Assert.AreEqual(@"now <ref name =""foo bar"" /> and", Parsers.FixReferenceTags(@"now <ref name +""foo bar"" /> and"));
            Assert.AreEqual(@"now <ref name =  ""foo bar"" > and", Parsers.FixReferenceTags(@"now <ref name +  ""foo bar"" > and"));

            // <ref "foo bar"> --> <ref name="foo bar">
            Assert.AreEqual(@"now <ref name=""foo bar""> and", Parsers.FixReferenceTags(@"now <ref ""foo bar""> and"));
            Assert.AreEqual(@"now <ref name=""foo bar"" /> and", Parsers.FixReferenceTags(@"now <ref ""foo bar"" /> and"));

            // <ref name="Fred" /ref> --> <ref name="Fred"/>
            Assert.AreEqual(@"now <ref name=""Fred""/> was", Parsers.FixReferenceTags(@"now <ref name=""Fred"" /ref> was"));
            Assert.AreEqual(@"now <ref name=""Fred A""/> was", Parsers.FixReferenceTags(@"now <ref name=""Fred A"" /ref> was"));

            // <ref name="Fred".> --> <ref name="Fred"/>
            Assert.AreEqual(@"now <ref name=""Fred"".> was", Parsers.FixReferenceTags(@"now <ref name=""Fred"".> was"));
            Assert.AreEqual(@"now <ref name=""Fred""?> was", Parsers.FixReferenceTags(@"now <ref name=""Fred""?> was"));

            // <ref>...<ref/> --> <ref>...</ref>
            Assert.AreEqual(@"now <ref>[http://www.site.com a site]</ref> was", Parsers.FixReferenceTags(@"now <ref>[http://www.site.com a site]<ref/> was"));
            Assert.AreEqual(@"now <ref> [http://www.site.com a site]</ref> was", Parsers.FixReferenceTags(@"now <ref> [http://www.site.com a site]<ref/> was"));

            // <ref>...</red> --> <ref>...</ref>
            Assert.AreEqual(@"now <ref>[http://www.site.com a site]</ref> was", Parsers.FixReferenceTags(@"now <ref>[http://www.site.com a site]</red> was"));
            Assert.AreEqual(@"now <ref> [http://www.site.com a site]</ref> was", Parsers.FixReferenceTags(@"now <ref> [http://www.site.com a site]</red> was"));

            // no Matches
            Assert.AreEqual(@"now <ref name=""foo bar""> and", Parsers.FixReferenceTags(@"now <ref name=""foo bar""> and"));
            Assert.AreEqual(@"now <ref name=""foo bar"" /> and", Parsers.FixReferenceTags(@"now <ref name=""foo bar"" /> and"));
            Assert.AreEqual(@"now <ref name = ""foo bar"" > and", Parsers.FixReferenceTags(@"now <ref name = ""foo bar"" > and"));
            Assert.AreEqual(@"now <ref name = 'foo bar' > and", Parsers.FixReferenceTags(@"now <ref name = 'foo bar' > and"));

            // <REF> and <Ref> to <ref>
            Assert.AreEqual(@"now <ref>Smith 2004</ref>", Parsers.FixReferenceTags(@"now <ref>Smith 2004</ref>"));
            Assert.AreEqual(@"now <ref>Smith 2004</ref>", Parsers.FixReferenceTags(@"now <Ref>Smith 2004</ref>"));
            Assert.AreEqual(@"now <ref>Smith 2004</ref>", Parsers.FixReferenceTags(@"now <REF>Smith 2004</REF>"));
            Assert.AreEqual(@"now <ref>Smith 2004</ref>", Parsers.FixReferenceTags(@"now <reF>Smith 2004</ref>"));
            Assert.AreEqual(@"now <ref name=S1>Smith 2004</ref>", Parsers.FixReferenceTags(@"now <reF name=S1>Smith 2004</ref>"));
            
            // removal of spaces between consecutive references
            Assert.AreEqual(@"<ref>foo</ref><ref>foo2</ref>", Parsers.FixReferenceTags(@"<ref>foo</ref> <ref>foo2</ref>"));
            Assert.AreEqual(@"<ref>foo</ref><ref>foo2</ref>", Parsers.FixReferenceTags(@"<ref>foo</ref>     <ref>foo2</ref>"));
            Assert.AreEqual(@"<ref>foo</ref><ref name=Bert />", Parsers.FixReferenceTags(@"<ref>foo</ref> <ref name=Bert />"));
            Assert.AreEqual(@"<ref>foo</ref><ref name=Bert>Bert2</ref>", Parsers.FixReferenceTags(@"<ref>foo</ref> <ref name=Bert>Bert2</ref>"));
            
            Assert.AreEqual(@"<ref name=""Tim""/><ref>foo2</ref>", Parsers.FixReferenceTags(@"<ref name=""Tim""/> <ref>foo2</ref>"));
            Assert.AreEqual(@"<ref name=Tim/><ref>foo2</ref>", Parsers.FixReferenceTags(@"<ref name=Tim/>     <ref>foo2</ref>"));
            Assert.AreEqual(@"<ref name=Tim/><ref name=Bert />", Parsers.FixReferenceTags(@"<ref name=Tim/> <ref name=Bert />"));
            Assert.AreEqual(@"<ref name=Tim/><ref name=Bert>Bert2</ref>", Parsers.FixReferenceTags(@"<ref name=Tim/> <ref name=Bert>Bert2</ref>"));
            
            // no matches on inalid ref format
            Assert.AreEqual(@"<ref name=""Tim""> <ref>foo2</ref>", Parsers.FixReferenceTags(@"<ref name=""Tim""> <ref>foo2</ref>"));
            
            // ensure a space between a reference and text (reference within a paragrah)
            Assert.AreEqual(@"Now clearly,<ref>Smith 2004</ref> he was", Parsers.FixReferenceTags(@"Now clearly,<ref>Smith 2004</ref>he was"));
            Assert.AreEqual(@"Now clearly,<ref>Smith 2004</ref> 2 were", Parsers.FixReferenceTags(@"Now clearly,<ref>Smith 2004</ref>2 were"));
            Assert.AreEqual(@"Now clearly,<ref name=Smith/> 2 were", Parsers.FixReferenceTags(@"Now clearly,<ref name=Smith/>2 were"));
        }

        [Test]
        public void ReorderReferencesTests()
        {
            // [1]...[2][1]
            Assert.AreEqual(@"'''Article''' is great.<ref name = ""Fred1"">So says Fred</ref>
Article started off pretty good, <ref name = ""Fred1"" /> <ref>So says John</ref> and finished well.
End of.", Parsers.ReorderReferences(@"'''Article''' is great.<ref name = ""Fred1"">So says Fred</ref>
Article started off pretty good, <ref>So says John</ref> <ref name = ""Fred1"" /> and finished well.
End of."));

            // [1]...[2][1]
            Assert.AreEqual(@"'''Article''' is great.<ref name = ""Fred2"">So says Fred</ref>
Article started off pretty good, <ref name = ""Fred2"" /> <ref name = ""John1"" >So says John</ref> and finished well.
End of.", Parsers.ReorderReferences(@"'''Article''' is great.<ref name = ""Fred2"">So says Fred</ref>
Article started off pretty good, <ref name = ""John1"" >So says John</ref> <ref name = ""Fred2"" /> and finished well.
End of."));

            // [1][2]...[3][2][1]
            Assert.AreEqual(@"'''Article''' is great.<ref name = ""Fred8"">So says Fred</ref><ref name = ""John3"" />
Article started off pretty good, <ref name = ""Fred8"" /><ref name = ""John3"" >So says John</ref> <ref>Third</ref> and finished well.
End of.", Parsers.ReorderReferences(@"'''Article''' is great.<ref name = ""Fred8"">So says Fred</ref><ref name = ""John3"" />
Article started off pretty good, <ref>Third</ref><ref name = ""John3"" >So says John</ref> <ref name = ""Fred8"" /> and finished well.
End of."));

            // [1][2][3]...[3][2][1]
            Assert.AreEqual(@"'''Article''' is great.<ref name = ""Fred9"">So says Fred</ref><ref name = ""John3"" /><ref name = ""Tim1"">ABC</ref>
Article started off pretty good, <ref name = ""Fred9"" /><ref name = ""John3"" >So says John</ref> <ref name = ""Tim1""/> and finished well.
End of.", Parsers.ReorderReferences(@"'''Article''' is great.<ref name = ""Fred9"">So says Fred</ref><ref name = ""John3"" /><ref name = ""Tim1"">ABC</ref>
Article started off pretty good, <ref name = ""Tim1""/><ref name = ""John3"" >So says John</ref> <ref name = ""Fred9"" /> and finished well.
End of."));

            Assert.AreEqual(@"'''Article''' is great.<ref name = 'Fred9'>So says Fred</ref><ref name = 'John3' /><ref name = 'Tim1'>ABC</ref>
Article started off pretty good, <ref name = 'Fred9' /><ref name = 'John3' >So says John</ref> <ref name = 'Tim1'/> and finished well.
End of.", Parsers.ReorderReferences(@"'''Article''' is great.<ref name = 'Fred9'>So says Fred</ref><ref name = 'John3' /><ref name = 'Tim1'>ABC</ref>
Article started off pretty good, <ref name = 'Tim1'/><ref name = 'John3' >So says John</ref> <ref name = 'Fred9' /> and finished well.
End of."));

            Assert.AreEqual(@"'''Article''' is great.<ref name = 'Fred9'>So says Fred</ref><ref name = 'John3' /><ref name = 'Tim1'>ABC</ref>
Article started off pretty good, <ref name = Fred9 /><ref name = 'John3' >So says John</ref> <ref name = 'Tim1'/> and finished well.
End of.", Parsers.ReorderReferences(@"'''Article''' is great.<ref name = 'Fred9'>So says Fred</ref><ref name = 'John3' /><ref name = 'Tim1'>ABC</ref>
Article started off pretty good, <ref name = 'Tim1'/><ref name = 'John3' >So says John</ref> <ref name = Fred9 /> and finished well.
End of."));

            Assert.AreEqual(@"'''Article''' is great.<ref name = 'Fred9'>So says Fred</ref><ref name = 'John3' /><ref name = 'Tim1'>ABC</ref>
Article started off pretty good, <ref name = ""Fred9"" /><ref name = 'John3' >So says John</ref> <ref name = 'Tim1'/> and finished well.
End of.", Parsers.ReorderReferences(@"'''Article''' is great.<ref name = 'Fred9'>So says Fred</ref><ref name = 'John3' /><ref name = 'Tim1'>ABC</ref>
Article started off pretty good, <ref name = 'Tim1'/><ref name = 'John3' >So says John</ref> <ref name = ""Fred9"" /> and finished well.
End of."));

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs#Re-ordering_references_can_leave_page_number_templates_behind.
            // have to allow {{rp}} template after a reference
            Assert.AreEqual(@"'''Article''' is great.<ref name = 'Fred9'>So says Fred</ref><ref name = 'John3' /><ref name = 'Tim1'>ABC</ref>
Article started off pretty good, <ref name = ""Fred9"" /><ref name = 'John3' >So says John</ref> <ref name = 'Tim1'/>{{rp|11}} and finished well.
End of.", Parsers.ReorderReferences(@"'''Article''' is great.<ref name = 'Fred9'>So says Fred</ref><ref name = 'John3' /><ref name = 'Tim1'>ABC</ref>
Article started off pretty good, <ref name = 'Tim1'/>{{rp|11}}<ref name = 'John3' >So says John</ref> <ref name = ""Fred9"" /> and finished well.
End of."));

            Assert.AreEqual(@"'''Article''' is great.<ref name = 'Fred9'>So says Fred</ref><ref name = 'John3' /><ref name = 'Tim1'>ABC</ref>
Article started off pretty good, <ref name = ""Fred9"" /><ref name = 'John3' >So says John</ref> <ref name = 'Tim1'/>{{rp|needed=y|May 2008}} and finished well.
End of.", Parsers.ReorderReferences(@"'''Article''' is great.<ref name = 'Fred9'>So says Fred</ref><ref name = 'John3' /><ref name = 'Tim1'>ABC</ref>
Article started off pretty good, <ref name = 'Tim1'/>{{rp|needed=y|May 2008}}<ref name = 'John3' >So says John</ref> <ref name = ""Fred9"" /> and finished well.
End of."));

            Assert.AreEqual(@"'''Article''' is great.<ref name = 'Fred9'>So says Fred</ref><ref name = 'John3' /><ref name = 'Tim1'>ABC</ref>
Article started off pretty good, <ref name = ""Fred9"" />{{Rp|12}}<ref name = 'John3' >So says John</ref> <ref name = 'Tim1'/>{{rp|11}} and finished well.
End of.", Parsers.ReorderReferences(@"'''Article''' is great.<ref name = 'Fred9'>So says Fred</ref><ref name = 'John3' /><ref name = 'Tim1'>ABC</ref>
Article started off pretty good, <ref name = 'Tim1'/>{{rp|11}}<ref name = 'John3' >So says John</ref> <ref name = ""Fred9"" />{{Rp|12}} and finished well.
End of."));

            Assert.AreEqual(@"'''Article''' is great.<ref name = 'Fred9'>So says Fred</ref><ref name = 'John3' /><ref name = 'Tim1'>ABC</ref>
Article started off pretty good, <ref name = ""Fred9"" /><ref name = 'John3' >So says John</ref>{{rp|11}} <ref name = 'Tim1'/> and finished well.
End of.", Parsers.ReorderReferences(@"'''Article''' is great.<ref name = 'Fred9'>So says Fred</ref><ref name = 'John3' /><ref name = 'Tim1'>ABC</ref>
Article started off pretty good, <ref name = 'Tim1'/><ref name = 'John3' >So says John</ref>{{rp|11}} <ref name = ""Fred9"" /> and finished well.
End of."));

            Assert.AreEqual(@"'''Article''' is great.<ref name = 'Fred9'>So says Fred</ref><ref name = 'John3' /><ref name = 'Tim1'>ABC</ref>
Article started off pretty good, <ref name = ""Fred9"" /><ref name = 'John3' >So says John</ref>{{rp|needed=y|May 2008}} <ref name = 'Tim1'/> and finished well.
End of.", Parsers.ReorderReferences(@"'''Article''' is great.<ref name = 'Fred9'>So says Fred</ref><ref name = 'John3' /><ref name = 'Tim1'>ABC</ref>
Article started off pretty good, <ref name = 'Tim1'/><ref name = 'John3' >So says John</ref>{{rp|needed=y|May 2008}} <ref name = ""Fred9"" /> and finished well.
End of."));

            // no changes
            const string A = @"'''Article''' is great.<ref name = ""Fred3"">So says Fred</ref>
Article started off pretty good, <ref name = ""Fred3"" /> <ref name = ""John2"" >So says John</ref> and finished well.
End of.";
            const string B = @"'''Article''' is great.<ref name = ""Fred4"">So says Fred</ref>
Article started off pretty good, <ref>So says Tim</ref> <ref>So says John</ref> and finished well.
End of.";
            const string C = @"'''Article''' is great.<ref name = ""Fred5"">So says Fred</ref>
Article started off pretty good,<ref name = ""Fred5"" /> <ref>So says John</ref> and finished well.
End of.";
            const string D = @"'''Article''' is great.<ref name = ""Fred6"">So says Fred</ref>
Article started off pretty good, <ref>So says John</ref> <ref name = ""A"">So says A</ref> and finished well.
End of.";
            const string E = @"'''Article''' is great.<ref name = ""John2"" />
Article is new.<ref name = ""Fred7"">So says Fred</ref>
Article started off pretty good, <ref name = ""John2"" >So says John</ref> <ref name = ""Fred7"" /> and finished well.
End of.";
            Assert.AreEqual(A, Parsers.ReorderReferences(A));
            Assert.AreEqual(B, Parsers.ReorderReferences(B));
            Assert.AreEqual(C, Parsers.ReorderReferences(C));
            Assert.AreEqual(D, Parsers.ReorderReferences(D));
            Assert.AreEqual(E, Parsers.ReorderReferences(E));
        }

        [Test]
        public void DuplicateNamedReferencesTests()
        {
            // duplicate references fix (both named)
            // Matches
            Assert.AreEqual(@"now<ref name=""Fred"">The Honourable Fred Smith, 2002</ref>but later than<ref name=""Fred""/> was", Parsers.DuplicateNamedReferences(@"now<ref name=""Fred"">The Honourable Fred Smith, 2002</ref>but later than<ref name=""Fred"">The Honourable Fred Smith, 2002</ref> was"));
            Assert.AreEqual(@"now<ref name=""Fred"">The Honourable Fred Smith, 2002</ref>but later than<ref name=""Fred""/> was", Parsers.DuplicateNamedReferences(@"now<ref name=""Fred"">The Honourable Fred Smith, 2002</ref>but later than<ref name=""Fred""> The Honourable Fred Smith, 2002  </ref> was"));
            Assert.AreEqual(@"now<ref name=""Fred"" >The Honourable Fred Smith, 2002</ref>but later than<ref name=""Fred""/> was", Parsers.DuplicateNamedReferences(@"now<ref name=""Fred"" >The Honourable Fred Smith, 2002</ref>but later than<ref name = ""Fred"">The Honourable Fred Smith, 2002</ref> was"));
            Assert.AreEqual(@"now<ref name = ""Fred"">The Honourable Fred Smith, 2002 </ref>but later than<ref name=""Fred""/> was", Parsers.DuplicateNamedReferences(@"now<ref name = ""Fred"">The Honourable Fred Smith, 2002 </ref>but later than<ref name=""Fred"">The Honourable Fred Smith, 2002</ref> was"));
            Assert.AreEqual(@"now<ref name=""Fred"">The Honourable Fred Smith, 2002</ref>but later than<ref name=""Fred""/> was", Parsers.DuplicateNamedReferences(@"now<ref name=""Fred"">The Honourable Fred Smith, 2002</ref>but later than<ref name=""Fred"" >The Honourable Fred Smith, 2002< /ref> was"));
            Assert.AreEqual(@"now<ref name='Fred'>The Honourable Fred Smith, 2002</ref>but later than<ref name=""Fred""/> was", Parsers.DuplicateNamedReferences(@"now<ref name='Fred'>The Honourable Fred Smith, 2002</ref>but later than<ref name=""Fred"" >The Honourable Fred Smith, 2002< /ref> was"));

            // no Matches
            string a = @"now<ref name=""Fred"">The Honourable Fred Smith, 2002</ref>but later than<ref name=""Fred"">The Honourable Fred Smith, 2005</ref> was"; // ref text different year
            string b = @"now<ref name=""Fred"">The Honourable Fred Smith, 2004</ref>but later than<ref name=""Fred"">The Honourable FRED SMITH, 2004</ref> was"; // ref text has different case
            string c = @"now<ref name=""Fred"">The Honourable Fred Smith, 2003</ref>but later than<ref name=""Fred2"">The Honourable Fred Smith, 2003</ref> was"; // ref name different
            string c2 = @"now<ref name=""Fred"">Fred Smith, 2003</ref>but later than<ref name=""Fred"">Fred Smith, 2003</ref> was"; // too short

            Assert.AreEqual(a, Parsers.DuplicateNamedReferences(a));
            Assert.AreEqual(b, Parsers.DuplicateNamedReferences(b));
            Assert.AreEqual(c, Parsers.DuplicateNamedReferences(c));
            Assert.AreEqual(c2, Parsers.DuplicateNamedReferences(c2));

            // duplicate references fix (first named)
            // Matches
            Assert.AreEqual(@"now<ref name=""Bert"">The Honourable Bert Smith, 2002</ref>but later than<ref name=""Bert""/> was", Parsers.DuplicateNamedReferences(@"now<ref name=""Bert"">The Honourable Bert Smith, 2002</ref>but later than<ref> The Honourable Bert Smith, 2002  </ref> was"));
            Assert.AreEqual(@"now<ref name=""Bert"" >The Honourable Bert Smith, 2002</ref>but later than<ref name=""Bert""/> was", Parsers.DuplicateNamedReferences(@"now<ref name=""Bert"" >The Honourable Bert Smith, 2002</ref>but later than<ref>The Honourable Bert Smith, 2002</ref> was"));
            Assert.AreEqual(@"now<ref name = ""Bert"">The Honourable Bert Smith, 2002 </ref>but later than<ref name=""Bert""/> was", Parsers.DuplicateNamedReferences(@"now<ref name = ""Bert"">The Honourable Bert Smith, 2002 </ref>but later than<ref>The Honourable Bert Smith, 2002</ref> was"));
            Assert.AreEqual(@"now<ref name=""Bert"">The Honourable Bert Smith, 2002</ref>but later than<ref name=""Bert""/> was", Parsers.DuplicateNamedReferences(@"now<ref name=""Bert"">The Honourable Bert Smith, 2002</ref>but later than<ref>The Honourable Bert Smith, 2002< /ref> was"));
            Assert.AreEqual(@"now<ref name='Bert'>The Honourable Bert Smith, 2002</ref>but later than<ref name=""Bert""/> was", Parsers.DuplicateNamedReferences(@"now<ref name='Bert'>The Honourable Bert Smith, 2002</ref>but later than<ref>The Honourable Bert Smith, 2002< /ref> was"));

            // no Matches
            string d = @"now<ref name=""Bert"">The Honourable Bert Smith, 2002</ref>but later than<ref>The Honourable Bert Smith, 2005</ref> was";
            string e = @"now<ref name=""Bert"">The Honourable Bert Smith, 2004</ref>but later than<ref>The Honourable BERT SMITH, 2004</ref> was";

            Assert.AreEqual(d, Parsers.DuplicateNamedReferences(d));
            Assert.AreEqual(e, Parsers.DuplicateNamedReferences(e)); // reference text casing

            // duplicate references fix (second named)
            Assert.AreEqual(@"now<ref name=""John""/>but later than<ref name=""John"" > The Honourable John Smith, 2002  </ref> was", Parsers.DuplicateNamedReferences(@"now<ref>The Honourable John Smith, 2002</ref>but later than<ref name=""John"" > The Honourable John Smith, 2002  </ref> was"));
            Assert.AreEqual(@"now<ref name=""John""/>but later than<ref name=""John"" >The Honourable John Smith, 2002</ref> was", Parsers.DuplicateNamedReferences(@"now<ref>The Honourable John Smith, 2002</ref>but later than<ref name=""John"" >The Honourable John Smith, 2002</ref> was"));
            Assert.AreEqual(@"now<ref name=""John""/>but later than<ref name = ""John"">The Honourable John Smith, 2002</ref> was", Parsers.DuplicateNamedReferences(@"now<ref>The Honourable John Smith, 2002 </ref>but later than<ref name = ""John"">The Honourable John Smith, 2002</ref> was"));
            Assert.AreEqual(@"now<ref name=""John""/>but later than<ref name=""John"">The Honourable John Smith, 2002< /ref> was", Parsers.DuplicateNamedReferences(@"now<ref>The Honourable John Smith, 2002</ref>but later than<ref name=""John"">The Honourable John Smith, 2002< /ref> was"));
            Assert.AreEqual(@"now<ref name=""John""/>but later than<ref name='John'>The Honourable John Smith, 2002< /ref> was", Parsers.DuplicateNamedReferences(@"now<ref>The Honourable John Smith, 2002</ref>but later than<ref name='John'>The Honourable John Smith, 2002< /ref> was"));
            Assert.AreEqual(@"now<ref name=""John 2""/>but later than<ref name=""John 2"" > The Honourable John Smith, 2002  </ref> was", Parsers.DuplicateNamedReferences(@"now<ref>The Honourable John Smith, 2002</ref>but later than<ref name=""John 2"" > The Honourable John Smith, 2002  </ref> was"));

            // no Matches
            string f = @"now<ref>The Honourable John Smith, 2002</ref>but later than<ref name=""John"">The Honourable John Smith, 2005</ref> was";
            string g = @"now<ref>The Honourable John Smith, 2004</ref>but later than<ref name=""John"">The HONOURABLE JOHN SMITH, 2004</ref> was";

            Assert.AreEqual(f, Parsers.DuplicateNamedReferences(f));
            Assert.AreEqual(g, Parsers.DuplicateNamedReferences(g)); // reference text casing
        }
    }

    [TestFixture]
    public class LinkTests : RequiresParser
    {
        [Test]
        public void TestStickyLinks()
        {
            Assert.AreEqual("[[Russian literature]]", Parsers.StickyLinks("[[Russian literature|Russian]] literature"));
            Assert.AreEqual("[[Russian literature]]", Parsers.StickyLinks("[[Russian literature|Russian]]  literature"));
            Assert.AreEqual("[[Russian literature]]", Parsers.StickyLinks("[[russian literature|Russian]] literature"));
            Assert.AreEqual("[[russian literature]]", Parsers.StickyLinks("[[Russian literature|russian]] literature"));
            Assert.AreEqual("[[Russian literature|Russian]]\nliterature", Parsers.StickyLinks("[[Russian literature|Russian]]\nliterature"));
            Assert.AreEqual("   [[Russian literature]]  ", Parsers.StickyLinks("   [[Russian literature|Russian]] literature  "));

            Assert.AreEqual("[[Russian literature|Russian]] Literature", Parsers.StickyLinks("[[Russian literature|Russian]] Literature"));

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_1#Link_de-piping_false_positive
            Assert.AreEqual("[[Sacramento, California|Sacramento]], California's [[capital city]]",
                Parsers.StickyLinks("[[Sacramento, California|Sacramento]], California's [[capital city]]"));
        }

        [Test]
        public void TestSimplifyLinks()
        {
            Assert.AreEqual("[[dog]]s", Parsers.SimplifyLinks("[[dog|dogs]]"));

            // case insensitivity of the first char
            Assert.AreEqual("[[dog]]s", Parsers.SimplifyLinks("[[Dog|dogs]]"));
            Assert.AreEqual("[[Dog]]s", Parsers.SimplifyLinks("[[dog|Dogs]]"));

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_8#Wrong_link_simplification_capitalisation
            Assert.AreEqual("[[dog]]", Parsers.SimplifyLinks("[[Dog|dog]]"));
            Assert.AreEqual("[[Dog]]", Parsers.SimplifyLinks("[[dog|Dog]]"));
            Assert.AreEqual("[[Dog]]", Parsers.SimplifyLinks("[[Dog|Dog]]"));

            Assert.AreEqual("[[dog]]s", Parsers.SimplifyLinks("[[Dog|dogs]]"));
            Assert.AreEqual("[[Dog]]s", Parsers.SimplifyLinks("[[dog|Dogs]]"));
            Assert.AreEqual("[[Dog]]s", Parsers.SimplifyLinks("[[Dog|Dogs]]"));

            // ...and sensitivity of others
            Assert.AreEqual("[[dog|dOgs]]", Parsers.SimplifyLinks("[[dog|dOgs]]"));

            //http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_2#Inappropriate_link_compression
            Assert.AreEqual("[[foo|foo3]]", Parsers.SimplifyLinks("[[foo|foo3]]"));

            // don't touch suffixes with caps to avoid funky results like
            // http://en.wikipedia.org/w/index.php?diff=195760456
            Assert.AreEqual("[[FOO|FOOBAR]]", Parsers.SimplifyLinks("[[FOO|FOOBAR]]"));
            Assert.AreEqual("[[foo|fooBAR]]", Parsers.SimplifyLinks("[[foo|fooBAR]]"));

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_8#Only_one_spurious_space_removed_from_link
            Assert.AreEqual("[[Elizabeth Gunn]]", Parsers.SimplifyLinks("[[Elizabeth Gunn | Elizabeth Gunn]]"));
            Assert.AreEqual("[[Big Bend, Texas|Big Bend]]", Parsers.SimplifyLinks("[[Big Bend, Texas | Big Bend]]"));

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_8#SVN:_general_fixes_removes_whitespace_around_pipes_within_citation_templates
            Assert.AreEqual("{{foo|[[bar]] | boz}}]]", Parsers.SimplifyLinks("{{foo|[[bar]] | boz}}]]"));
            Assert.AreEqual("{{foo|[[bar]]\r\n| boz}}]]", Parsers.SimplifyLinks("{{foo|[[bar]]\r\n| boz}}]]"));

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_9#General_fixes_remove_spaces_from_category_sortkeys
            Assert.AreEqual("[[foo|bar]]", Parsers.SimplifyLinks("[[foo| bar]]"));
            Assert.AreEqual("[[foo|bar]]", Parsers.SimplifyLinks("[[foo| bar ]]"));
            Assert.AreEqual("[[Category:foo| bar]]", Parsers.SimplifyLinks("[[Category:foo| bar]]"));
            Assert.AreEqual("[[Category:foo| bar]]", Parsers.SimplifyLinks("[[Category:foo| bar ]]"));
        }

        [Test]
        public void FixDates()
        {
            Assert.AreEqual("the later 1990s", parser.FixDates("the later 1990's"));

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_1#Title_bolding
            Assert.AreEqual("the later A1990's", parser.FixDates("the later A1990's"));

            //http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_1#Breaking_a_template
            Assert.AreEqual("{{the later 1990's}}", parser.FixDates("{{the later 1990's}}"));

            // replace <br> and <p> HTML tags tests
            Assert.AreEqual("\r\n\r\nsome text", parser.FixDates("<p>some text"));
            Assert.AreEqual("\r\nsome text", parser.FixDates("<br><br>some text"));
            Assert.AreEqual("some text\r\n\r\n", parser.FixDates("some text<p>"));
            Assert.AreEqual("some text\r\n", parser.FixDates("some text<br><br>"));

            // don't match when in table or blockquote
            Assert.AreEqual("|<p>some text", parser.FixDates("|<p>some text"));
            Assert.AreEqual("|<br><br>some text", parser.FixDates("|<br><br>some text"));
            Assert.AreEqual("!<p>some text", parser.FixDates("!<p>some text"));
            Assert.AreEqual("!<br><br>some text", parser.FixDates("!<br><br>some text"));

            Assert.AreEqual("<blockquote><p>some text</blockquote>", parser.FixDates("<blockquote><p>some text</blockquote>"));
            Assert.AreEqual("<blockquote>|<br><br>some text</blockquote>", parser.FixDates("<blockquote>|<br><br>some text</blockquote>"));
        }

        [Test]
        public void FixLivingThingsRelatedDates()
        {
            Assert.AreEqual("test text", Parsers.FixLivingThingsRelatedDates("test text"));
            Assert.AreEqual("'''John Doe''' (born [[21 February]] [[2008]])", Parsers.FixLivingThingsRelatedDates("'''John Doe''' (b. [[21 February]] [[2008]])"));
            Assert.AreEqual("'''John Doe''' (died [[21 February]] [[2008]])", Parsers.FixLivingThingsRelatedDates("'''John Doe''' (d. [[21 February]] [[2008]])"));
            Assert.AreEqual("'''Willa Klug Baum''' ([[October 4]], [[1926]] – May 18, 2006)", Parsers.FixLivingThingsRelatedDates("'''Willa Klug Baum''' (born [[October 4]], [[1926]], died May 18, 2006)"));
        }
        
        [Test]
        public void LivingPeopleTests()
        {
            // with sortkey
            Assert.AreEqual(@"'''Fred Smith''' (born 1960) is a bloke.
[[Category:1960 births|Smith, Fred]][[Category:Living people|Smith, Fred]]", Parsers.LivingPeople(@"'''Fred Smith''' (born 1960) is a bloke.
[[Category:1960 births|Smith, Fred]]"));

            // no sortkey
            Assert.AreEqual(@"'''Fred Smith''' (born 1960) is a bloke.
[[Category:1960 births]][[Category:Living people]]", Parsers.LivingPeople(@"'''Fred Smith''' (born 1960) is a bloke.
[[Category:1960 births]]"));

            // no matches if not identified as born
            string b1 = @"'''Fred Smith''' is a bloke.";
            Assert.AreEqual(b1, Parsers.LivingPeople(b1));

            // no matches if identified as dead
            string a = @"'''Fred Smith''' (born 1960) is a bloke.
[[Category:1960 births]]
[[Category:1990 deaths]]";
            string b = @"'''Fred Smith''' is a bloke.
[[Category:1960 births]]
[[Category:1990 deaths]]";
            string c = @"'''Fred Smith''' is a bloke.
[[Category:1960 births|Smith, Fred]]
[[Category:1990 deaths|Smith, Fred]]";
            string d = @"'''Fred Smith''' (born 11 May 1950 - died 17 August 1990) is a bloke.
[[Category:1960 births|Smith, Fred]]";
            string e = @"'''Fred Smith''' (born 1950) is a bloke.
[[Category:1950 births|Smith, Fred]]
{{Recentlydeceased}}";
            string f = @"'''Fred Smith''' (born 1950) is a bloke.
[[Category:1950 births|Smith, Fred]]
{{recentlydeceased}}";
            string g = @"'''Fred Smith''' (born 1950) is a bloke.
[[Category:1950 births|Smith, Fred]]
[[Category:Year of death missing]]";
string h = @"'''Fred Smith''' (d. 1950) is a bloke.";

            Assert.AreEqual(a, Parsers.LivingPeople(a));
            Assert.AreEqual(b, Parsers.LivingPeople(b));
            Assert.AreEqual(c, Parsers.LivingPeople(c));
            Assert.AreEqual(d, Parsers.LivingPeople(d));
            Assert.AreEqual(e, Parsers.LivingPeople(e));
            Assert.AreEqual(f, Parsers.LivingPeople(f));
            Assert.AreEqual(g, Parsers.LivingPeople(g));
            Assert.AreEqual(h, Parsers.LivingPeople(h));
            
            // assume dead if born earlier than 1910, so no change
            string d1 = @"'''Fred Smith''' (born 1960) is a bloke.
[[Category:1909 births|Smith, Fred]]";
            Assert.AreEqual(d1, Parsers.LivingPeople(d1));
        }

        [Test]
        public void UnbalancedBracketsTests()
        {
            int BracketLength = 0;
            Assert.AreEqual(18, Parsers.UnbalancedBrackets(@"now hello {{bye}} {{now}", ref BracketLength));
            Assert.AreEqual(2, BracketLength);
            Assert.AreEqual(18, Parsers.UnbalancedBrackets(@"now hello {{bye}} {{now} abc", ref BracketLength));
            Assert.AreEqual(2, BracketLength);
            Assert.AreEqual(18, Parsers.UnbalancedBrackets(@"now hello {{bye}} [[now]", ref BracketLength));
            Assert.AreEqual(2, BracketLength);
            Assert.AreEqual(18, Parsers.UnbalancedBrackets(@"now hello [[bye]] {{now}", ref BracketLength));
            Assert.AreEqual(2, BracketLength);
            Assert.AreEqual(18, Parsers.UnbalancedBrackets(@"now hello {{bye}} [now words [here]", ref BracketLength));
            Assert.AreEqual(1, BracketLength);
            Assert.AreEqual(21, Parsers.UnbalancedBrackets(@"now hello {{bye}} now] words [here]", ref BracketLength));
            Assert.AreEqual(1, BracketLength);
            Assert.AreEqual(22, Parsers.UnbalancedBrackets(@"now hello {{bye}} {now}}", ref BracketLength));
            Assert.AreEqual(2, BracketLength);
            Assert.AreEqual(33, Parsers.UnbalancedBrackets(@"[http://www.site.com a link [cool]]", ref BracketLength)); // FixSyntax replaces with &#93;
            Assert.AreEqual(2, BracketLength);
            Assert.AreEqual(18, Parsers.UnbalancedBrackets(@"now hello {{bye}} {now", ref BracketLength));
            Assert.AreEqual(1, BracketLength);

            // only first reported
            Assert.AreEqual(18, Parsers.UnbalancedBrackets(@"now hello {{bye}} {{now} or {{now} was", ref BracketLength));
            Assert.AreEqual(18, Parsers.UnbalancedBrackets(@"now hello {{bye}} {{now} or [[now] was", ref BracketLength));

            // brackets all okay
            Assert.AreEqual(-1, Parsers.UnbalancedBrackets(@"now hello {{bye}} {{now}}", ref BracketLength));
            Assert.AreEqual(-1, Parsers.UnbalancedBrackets(@"now hello [[bye]] {{now}}", ref BracketLength));
            Assert.AreEqual(-1, Parsers.UnbalancedBrackets(@"now hello {{bye}} [now]", ref BracketLength));
            Assert.AreEqual(-1, Parsers.UnbalancedBrackets(@"now hello", ref BracketLength));
            Assert.AreEqual(-1, Parsers.UnbalancedBrackets(@"<ref>[http://www.pubmedcentral.nih.gov/articlerender.fcgi?artid=32159 Message to 
complementary and alternative medicine: evidence is a better friend than power. Andrew J Vickers]</ref>", ref BracketLength));
            Assert.AreEqual(-1, Parsers.UnbalancedBrackets(@"[http://www.site.com a link [cool&#93;]", ref BracketLength)); // displays as valid syntax

            // don't consider stuff in <math> or <pre> tags etc.
            Assert.AreEqual(-1, Parsers.UnbalancedBrackets(@"now hello {{bye}} <pre>{now}}</pre>", ref BracketLength));
            Assert.AreEqual(-1, Parsers.UnbalancedBrackets(@"now hello {{bye}} <math>{a{b}}</math>", ref BracketLength));
            Assert.AreEqual(-1, Parsers.UnbalancedBrackets(@"now hello {{bye}} <code>{now}}</code>", ref BracketLength));
        }

        [Test]
        public void TestCiteFormatFieldTypo()
        {
            Assert.AreEqual(@"now {{cite web| url=a.com|title=hello|format=PDF}} was", Parsers.FixSyntax(@"now {{cite web| url=a.com|title=hello|fprmat=PDF}} was"));
            Assert.AreEqual(@"now {{cite web| url=a.com|title=hello| format=PDF}} was", Parsers.FixSyntax(@"now {{cite web| url=a.com|title=hello| fprmat=PDF}} was"));
            Assert.AreEqual(@"now {{cite web| url=a.com|title=hello|format = PDF}} was", Parsers.FixSyntax(@"now {{cite web| url=a.com|title=hello|Fprmat = PDF}} was"));
            Assert.AreEqual(@"now {{Cite web| url=a.com|title=hello|format=DOC}} was", Parsers.FixSyntax(@"now {{Cite web| url=a.com|title=hello|fprmat=DOC}} was"));
            Assert.AreEqual(@"now {{Cite web| url=a.com|title=hello|
          format=DOC|publisher=BBC}} was", Parsers.FixSyntax(@"now {{Cite web| url=a.com|title=hello|
          fprmat=DOC|publisher=BBC}} was"));

            // no Matches
            Assert.AreEqual(@"now {{cite web| url=http://site.net|title=okay}} fprmat of PDF", Parsers.FixSyntax(@"now {{cite web| url=http://site.net|title=okay}} fprmat of PDF"));
            Assert.AreEqual(@"now {{cite web| url=http://site.net/1.pdf|format=PDF}} was", Parsers.FixSyntax(@"now {{cite web| url=http://site.net/1.pdf|format=PDF}} was"));
            Assert.AreEqual(@"now {{cite web|\r\nurl=http://site.net/1.pdf|format=PDF|title=okay}} was", Parsers.FixSyntax(@"now {{cite web|\r\nurl=http://site.net/1.pdf|format=PDF|title=okay}} was"));
        }

        [Test, Category("Incomplete")]
        public void TestFixSyntax()
        {
            bool noChange;

            Assert.AreEqual("[http://example.com] site", Parsers.FixSyntax("[http://example.com] site"));
            Assert.AreEqual("[http://example.com] site", Parsers.FixSyntax("[[http://example.com] site"));
            Assert.AreEqual("[http://example.com] site", Parsers.FixSyntax("[http://example.com]] site"));
            Assert.AreEqual("[http://example.com] site", Parsers.FixSyntax("[[http://example.com]] site"));

            Assert.AreEqual("[http://test.com]", Parsers.FixSyntax("[http://test.com]"));
            Assert.AreEqual("[http://test.com]", Parsers.FixSyntax("[http://http://test.com]"));
            Assert.AreEqual("[http://test.com]", Parsers.FixSyntax("[http://http://http://test.com]"));

            Assert.AreEqual("[http://www.site.com ''my cool site'']", Parsers.FixSyntax("[http://www.site.com|''my cool site'']"));
            Assert.AreEqual("[http://www.site.com/here/there.html ''my cool site'']", Parsers.FixSyntax("[http://www.site.com/here/there.html|''my cool site'']"));

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_3#NEsted_square_brackets_again.
            Assert.AreEqual("[[Image:foo.jpg|Some [http://some_crap.com]]]",
                            Parsers.FixSyntax("[[Image:foo.jpg|Some [http://some_crap.com]]]"));

            Assert.AreEqual("Image:foo.jpg|{{{some_crap}}}]]", Parsers.FixSyntax("Image:foo.jpg|{{{some_crap}}}]]"));

            Assert.AreEqual("[[somelink]]", Parsers.FixSyntax("[somelink]]"));
            Assert.AreEqual("[[somelink]]", Parsers.FixSyntax("[[somelink]"));
            Assert.AreNotEqual("[[somelink]]", Parsers.FixSyntax("[somelink]"));

            Assert.AreEqual("'''foo''' bar", Parsers.FixSyntax("<b>foo</b> bar"));
            Assert.AreEqual("'''foo''' bar", Parsers.FixSyntax("< b >foo</b> bar"));
            Assert.AreEqual("'''foo''' bar", Parsers.FixSyntax("<b>foo< /b > bar"));
            Assert.AreEqual("<b>foo<b> bar", Parsers.FixSyntax("<b>foo<b> bar"));

            Assert.AreEqual("''foo'' bar", Parsers.FixSyntax("<i>foo</i> bar"));
            Assert.AreEqual("''foo'' bar", Parsers.FixSyntax("< i >foo</i> bar"));
            Assert.AreEqual("''foo'' bar", Parsers.FixSyntax("<i>foo< /i > bar"));
            Assert.AreEqual("<i>foo<i> bar", Parsers.FixSyntax("<i>foo<i> bar"));

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_7#Erroneously_removing_pipe
            Assert.AreEqual("[[|foo]]", Parsers.FixSyntax("[[|foo]]"));

            //Double Spaces
            Assert.AreEqual("[[Foo]]", Parsers.FixSyntax("[[Foo]]"));
            Assert.AreEqual("[[Foo Bar]]", Parsers.FixSyntax("[[Foo Bar]]"));
            Assert.AreEqual("[[Foo Bar]]", Parsers.FixSyntax("[[Foo  Bar]]"));
            Assert.AreEqual("[[Foo Bar|Bar]]", Parsers.FixSyntax("[[Foo  Bar|Bar]]"));

            //TODO: move it to parts testing specific functions, when they're covered
            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_4#Bug_encountered_when_perusing_Sonorous_Susurrus
            Parsers.CanonicalizeTitle("[[|foo]]"); // shouldn't throw exceptions
            Assert.AreEqual("[[|foo]]", Parsers.FixLinks("[[|foo]]", "bar", out noChange));

            // string before and after
            Assert.AreEqual("<ref>http://www.site.com</ref>", Parsers.FixSyntax(@"<ref>http//www.site.com</ref>"));
            Assert.AreEqual("at http://www.site.com", Parsers.FixSyntax(@"at http//www.site.com"));
            Assert.AreEqual("<ref>[http://www.site.com a website]</ref>",
                            Parsers.FixSyntax(@"<ref>[http:/www.site.com a website]</ref>"));
            Assert.AreEqual("*[http://www.site.com a website]", Parsers.FixSyntax(@"*[http//www.site.com a website]"));
            Assert.AreEqual("|url=http://www.site.com", Parsers.FixSyntax(@"|url=http//www.site.com"));
            Assert.AreEqual("|url = http://www.site.com", Parsers.FixSyntax(@"|url = http:/www.site.com"));
            Assert.AreEqual("[http://www.site.com]", Parsers.FixSyntax(@"[http/www.site.com]"));

            // these strings should not change
            Assert.AreEqual("http://members.bib-arch.org/nph-proxy.pl/000000A/http/www.basarchive.org/bswbSearch",
                            Parsers.FixSyntax(
                                "http://members.bib-arch.org/nph-proxy.pl/000000A/http/www.basarchive.org/bswbSearch"));
            Assert.AreEqual("http://sunsite.utk.edu/math_archives/.http/contests/",
                            Parsers.FixSyntax("http://sunsite.utk.edu/math_archives/.http/contests/"));
            Assert.AreEqual("HTTP/0.9", Parsers.FixSyntax("HTTP/0.9"));
            Assert.AreEqual("HTTP/1.0", Parsers.FixSyntax("HTTP/1.0"));
            Assert.AreEqual("HTTP/1.1", Parsers.FixSyntax("HTTP/1.1"));
            Assert.AreEqual("HTTP/1.2", Parsers.FixSyntax("HTTP/1.2"));
            Assert.AreEqual("the HTTP/1.2 protocol", Parsers.FixSyntax("the HTTP/1.2 protocol"));
            Assert.AreEqual(@"<ref>[http://cdiac.esd.ornl.gov/ftp/cdiac74/a.pdf chapter 5]</ref>", Parsers.FixSyntax(@"<ref>[http://cdiac.esd.ornl.gov/ftp/cdiac74/a.pdf chapter 5]</ref>"));

            // <font> tag removal
            Assert.AreEqual("hello", Parsers.FixSyntax(@"<font>hello</font>"));
            Assert.AreEqual("hello", Parsers.FixSyntax(@"<font>hello</FONT>"));
            Assert.AreEqual(@"hello
world", Parsers.FixSyntax(@"<font>hello
world</font>"));

            // only changing font tags without properties
            Assert.AreEqual(@"<font name=ab>hello</font>", Parsers.FixSyntax(@"<font name=ab>hello</font>"));

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs#second_pair_of_brackets_added_to_https_links
            Assert.AreEqual("[https://example.com] site", Parsers.FixSyntax("[https://example.com]] site"));
            Assert.AreEqual("[https://example.com] site", Parsers.FixSyntax("[[https://example.com] site"));
        }

        [Test]
        public void TestFixSyntaxUnbalancedBrackets()
        {
            Assert.AreEqual(@"<ref>{{cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref>{{cite web|url=a|title=b}</ref>"));
            Assert.AreEqual(@"<ref>{{cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref>{{cite web|url=a|title=b}]</ref>"));
            Assert.AreEqual(@"<ref>{{cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref>{{cite web|url=a|title=b))</ref>"));
            Assert.AreEqual(@"<ref> {{cite web|url=a|title=b}} </ref>", Parsers.FixSyntax(@"<ref> {{cite web|url=a|title=b} </ref>"));
            Assert.AreEqual(@"<ref name=Fred>{{cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref name=Fred>{{cite web|url=a|title=b}</ref>"));
            Assert.AreEqual(@"<ref name=""Fred"">{{cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref name=""Fred"">{{cite web|url=a|title=b}</ref>"));

            Assert.AreEqual(@"<ref>[http://www.site.com]</ref>", Parsers.FixSyntax(@"<ref>{{http://www.site.com}}</ref>"));
            Assert.AreEqual(@"<ref>[http://www.site.com cool site]</ref>", Parsers.FixSyntax(@"<ref>{{http://www.site.com cool site}}</ref>"));
            Assert.AreEqual(@"<ref name=""Fred"">[http://www.site.com]</ref>", Parsers.FixSyntax(@"<ref name=""Fred"">{{http://www.site.com}}</ref>"));
            Assert.AreEqual(@"<ref> [http://www.site.com] </ref>", Parsers.FixSyntax(@"<ref> {{http://www.site.com}} </ref>"));

            Assert.AreEqual(@"<ref>{{cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref>{cite web|url=a|title=b}}</ref>"));
            Assert.AreEqual(@"<ref> {{cite web|url=a|title=b}} </ref>", Parsers.FixSyntax(@"<ref> {cite web|url=a|title=b}} </ref>"));
            Assert.AreEqual(@"<ref name=Fred>{{Cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref name=Fred>{Cite web|url=a|title=b}}</ref>"));
            Assert.AreEqual(@"<ref name=""Fred"">{{cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref name=""Fred"">{cite web|url=a|title=b}}</ref>"));
            Assert.AreEqual(@"<ref>{{citation|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref>{citation|url=a|title=b}}</ref>"));

            Assert.AreEqual(@"{{hello}}", Parsers.FixSyntax(@"{[hello}}"));
            Assert.AreEqual(@"{{hello}}", Parsers.FixSyntax(@"[{hello}}"));

            Assert.AreEqual(@"<ref> {{Citation|", Parsers.FixSyntax(@"<ref> {Citation|"));
            Assert.AreEqual(@"<ref> {{cite web|", Parsers.FixSyntax(@"<ref> {cite web|"));
            Assert.AreEqual(@"* {{Citation|", Parsers.FixSyntax(@"* {Citation|"));

            Assert.AreEqual(@"<ref>{{cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref>{{cite web|url=a|title=b}}}}</ref>"));
            Assert.AreEqual(@"<ref> {{cite web|url=a|title=b}} </ref>", Parsers.FixSyntax(@"<ref> {{cite web|url=a|title=b}}}} </ref>"));
            Assert.AreEqual(@"<ref name=Fred>{{Cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref name=Fred>{{Cite web|url=a|title=b}}}}</ref>"));
            Assert.AreEqual(@"<ref name=""Fred"">{{cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref name=""Fred"">{{cite web|url=a|title=b}}}}</ref>"));
            Assert.AreEqual(@"<ref>{{citation|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref>{{citation|url=a|title=b}}}}</ref>"));

            Assert.AreEqual(@"<ref>{{cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref>{[cite web|url=a|title=b}}</ref>"));
            Assert.AreEqual(@"<ref> {{cite web|url=a|title=b}} </ref>", Parsers.FixSyntax(@"<ref> {[cite web|url=a|title=b}} </ref>"));
            Assert.AreEqual(@"<ref name=Fred>{{Cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref name=Fred>{[Cite web|url=a|title=b}}</ref>"));
            Assert.AreEqual(@"<ref name=""Fred"">{{cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref name=""Fred"">{[cite web|url=a|title=b}}</ref>"));
            Assert.AreEqual(@"<ref>{{citation|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref>{[citation|url=a|title=b}}</ref>"));

            Assert.AreEqual(@"<ref>{{cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref>{{cite web|url=a|title=b{}</ref>"));
            Assert.AreEqual(@"<ref> {{cite web|url=a|title=b}} </ref>", Parsers.FixSyntax(@"<ref> {{cite web|url=a|title=b}] </ref>"));
            Assert.AreEqual(@"<ref name=Fred>{{Cite web|url=a|title=[[b]]}}</ref>", Parsers.FixSyntax(@"<ref name=Fred>{{Cite web|url=a|title=[[b]]}</ref>"));
            Assert.AreEqual(@"<ref name=""Fred"">{{cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref name=""Fred"">{{cite web|url=a|title=b{}</ref>"));
            Assert.AreEqual(@"<ref>{{citation|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref>{{citation|url=a|title=b}]</ref>"));

            Assert.AreEqual(@"<ref>[http://site.com]</ref>", Parsers.FixSyntax(@"<ref>http://site.com]</ref>"));
            Assert.AreEqual(@"<ref>Smith, 2004 [http://site.com]</ref>", Parsers.FixSyntax(@"<ref>Smith, 2004 http://site.com]</ref>"));
            Assert.AreEqual(@"<ref> [http://site.com] </ref>", Parsers.FixSyntax(@"<ref> http://site.com] </ref>"));
            Assert.AreEqual(@"<ref name=Fred>[http://site.com cool]</ref>", Parsers.FixSyntax(@"<ref name=Fred>http://site.com cool]</ref>"));
            Assert.AreEqual(@"<ref name=""Fred"">[http://site.com]</ref>", Parsers.FixSyntax(@"<ref name=""Fred"">http://site.com]</ref>"));
            Assert.AreEqual(@"<ref>[http://site.com great site-here]</ref>", Parsers.FixSyntax(@"<ref>http://site.com great site-here]</ref>"));

            Assert.AreEqual(@"<ref>{{cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref>cite web|url=a|title=b}}</ref>"));
            Assert.AreEqual(@"<ref name=""Smith"">{{cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref name=""Smith"">cite web|url=a|title=b}}</ref>"));
            Assert.AreEqual(@"<ref name=""Smith"">{{cite web|
url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref name=""Smith"">cite web|
url=a|title=b}}</ref>"));
            Assert.AreEqual(@"<ref>{{cite book|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref>cite book|url=a|title=b}}</ref>"));
            Assert.AreEqual(@"<ref>{{Citation|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref>Citation|url=a|title=b}}</ref>"));
            Assert.AreEqual(@"<ref>{{cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref>((cite web|url=a|title=b}}</ref>"));

            Assert.AreEqual(@"<ref>Antara News [http://www.antara.co.id/arc/2009/1/9/kri-lebanon/ KRI Lebanon]</ref>", Parsers.FixSyntax(@"<ref>Antara News [http://www.antara.co.id/arc/2009/1/9/kri-lebanon/ KRI Lebanon</ref>"));
            Assert.AreEqual(@"<ref name=Smith>Antara News [http://www.antara.co.id/arc/2009/1/9/kri-lebanon/ KRI Lebanon]</ref>", Parsers.FixSyntax(@"<ref name=Smith>Antara News [http://www.antara.co.id/arc/2009/1/9/kri-lebanon/ KRI Lebanon</ref>"));
            Assert.AreEqual(@"<ref name=Smith>Antara News [http://www.antara.co.id/arc/2009/1/9/kri-lebanon/ KRI Lebanon]</ref>", Parsers.FixSyntax(@"<ref name=Smith>Antara News [http://www.antara.co.id/arc/2009/1/9/kri-lebanon/ KRI Lebanon]</ref>"));

            // no completion of template braces on non-template ref
            string a = @"<ref>Smith and Jones, 2005, p46}}</ref>";
            Assert.AreEqual(a, Parsers.FixSyntax(a));

            Assert.AreEqual(@"{{DEFAULTSORT:Astaire, Fred}}", Parsers.FixSyntax(@"{{DEFAULTSORT:Astaire, Fred]}}"));
            Assert.AreEqual(@"{{DEFAULTSORT:Astaire, Fred}}", Parsers.FixSyntax(@"{{DEFAULTSORT:Astaire, Fred]]}}"));
            Assert.AreEqual(@"{{DEFAULTSORT:Astaire, Fred}}", Parsers.FixSyntax(@"{{DEFAULTSORT:Astaire, Fred]]]}}"));
            Assert.AreEqual(@"{{DEFAULTSORT:Astaire, Fred}}", Parsers.FixSyntax(@"{{DEFAULTSORT:Astaire, Fred[]}}"));

            // completes curly brackets where it would make all brackets balance
            Assert.AreEqual(@"Great.{{fact|date=May 2008}} Now", Parsers.FixSyntax(@"Great.{{fact|date=May 2008} Now"));
            Assert.AreEqual(@"Great.{{fact|date=May 2008}} Now", Parsers.FixSyntax(@"Great.{{fact|date=May 2008]} Now"));

            // don't change what could be wikitable
            Assert.AreEqual(@"Great.{{foo|} Now", Parsers.FixSyntax(@"Great.{{foo|} Now"));

            // set single curly bracket to normal bracket if that makes all brackets balance
            Assert.AreEqual(@"Great (not really) now", Parsers.FixSyntax(@"Great (not really} now"));
            Assert.AreEqual(@"# [[Herbert H. H. Fox]] ([[1934 - 1939]])<br>", Parsers.FixSyntax(@"# [[Herbert H. H. Fox]] ([[1934 - 1939]]}<br>"));
            // can't fix these
            Assert.AreEqual(@"Great { but (not really} now", Parsers.FixSyntax(@"Great { but (not really} now"));
            Assert.AreEqual(@"Great (not really)} now", Parsers.FixSyntax(@"Great (not really)} now"));
            // don't touch when it could be a table
            Assert.AreEqual(@"great (in 2001 | blah |} now", Parsers.FixSyntax(@"great (in 2001 | blah |} now"));

            // doesn't complete curly brackets where they don't balance after
            string Cite1 = @"Great.<ref>{{cite web | url=http://www.site.com | title=abc } year=2009}}</ref>";
            Assert.AreEqual(Cite1, Parsers.FixSyntax(Cite1));

            // set double round bracket to single
            Assert.AreEqual(@"then (but often) for", Parsers.FixSyntax(@"then ((but often) for"));

            // only applies changes if brackets then balance out
            Assert.AreEqual(@"then ((but often)) for", Parsers.FixSyntax(@"then ((but often)) for"));
            Assert.AreEqual(@"then ((but often)} for", Parsers.FixSyntax(@"then ((but often)} for"));
            Assert.AreEqual(@"then ((but often) for(", Parsers.FixSyntax(@"then ((but often) for("));

            // an unbalanced bracket is fixed if there are enough characters until the next one to be confident that the next one is indeed
            // a separate incident
            Assert.AreEqual(@"# [[Daniel Sylvester Tuttle|Daniel S. Tuttle]], (1867 - 1887)
# Ethelbert Talbot, (1887 - 1898), 
# James B. Funsten, (1899 - 1918)
# Herman Page, (1919 - 1919)
# Frank H. Touret, (1919 - 1924)
# Herbert H. H. Fox, (1925 - 1926)
# [[Middleton S. Barnwell]], (1926 - 1935)
# Frederick B. Bartlett, (1935 - 1941)
# Frak A. Rhea, (1942 - 1968)
# Norman L. Foote, (1957 - 1972)
# Hanford L. King, Jr., (1972 - 1981)
# David B. Birney, IV, (1982 - 1989)
# John S. Thornton. (1990 - 1998
# Harry B. Bainbridge, III, (1998 - 2008)
# Brian J. Thom  (2008 - Present)", Parsers.FixSyntax(@"# [[Daniel Sylvester Tuttle|Daniel S. Tuttle]], (1867 - 1887)
# Ethelbert Talbot, (1887 - 1898), 
# James B. Funsten, (1899 - 1918)
# Herman Page, (1919 - 1919}
# Frank H. Touret, (1919 - 1924)
# Herbert H. H. Fox, (1925 - 1926)
# [[Middleton S. Barnwell]], (1926 - 1935)
# Frederick B. Bartlett, (1935 - 1941)
# Frak A. Rhea, (1942 - 1968)
# Norman L. Foote, (1957 - 1972)
# Hanford L. King, Jr., (1972 - 1981)
# David B. Birney, IV, (1982 - 1989)
# John S. Thornton. (1990 - 1998
# Harry B. Bainbridge, III, (1998 - 2008)
# Brian J. Thom  (2008 - Present)"));

            Assert.AreEqual(@"The '''Zach Feuer Gallery''' is a [[contemporary art]] gallery located in [[Chelsea, Manhattan|Chelsea]], [[New York City|New York]].

==History==

The Zach Feuer Gallery was founded in 2000 as the LFL Gallery, by Nick Lawrence, Russell LaMontagne and Zach Feuer.  It was originally located on a fourth floor space on 26th Street.    In 2002 the gallery moved to a first floor space on 24th Street, briefly sharing space with an art book gallery owned by one of the partners.  In 2004  Zach Feuer purchased the gallery from his partners and changed the gallery name to Zach Feuer Gallery.

Some artists represented by Zach Feuer Gallery are [[Phoebe Washburn]], [[Jules de Balincourt]], [[Nathalie Djurberg]], [[Justin Lieberman]], [[Stuart Hawkins]],  [[Johannes Vanderbeek]], [[Sister Corita Kent]], [[Tamy Ben Tor]], [[Anton Henning]], [[Dana Schutz]], and [[Mark Flood]].

==External links==
* [http://www.zachfeuer.com Zach Feuer Gallery] website

{{coord missing|New York}}

[[Category:Contemporary art galleries]]
[[Category:2000 establishments]]
[[Category:Art galleries in Manhattan]]", Parsers.FixSyntax(@"The '''Zach Feuer Gallery''' is a [[contemporary art]] gallery located in [[Chelsea, Manhattan|Chelsea]], [[New York City|New York]].

==History==

The Zach Feuer Gallery was founded in 2000 as the LFL Gallery, by Nick Lawrence, Russell LaMontagne and Zach Feuer.  It was originally located on a fourth floor space on 26th Street.    In 2002 the gallery moved to a first floor space on 24th Street, briefly sharing space with an art book gallery owned by one of the partners.  In 2004  Zach Feuer purchased the gallery from his partners and changed the gallery name to Zach Feuer Gallery.

Some artists represented by Zach Feuer Gallery are [[Phoebe Washburn]], [[Jules de Balincourt]], [[Nathalie Djurberg]]], [[Justin Lieberman]], [[Stuart Hawkins]],  [[Johannes Vanderbeek]], [[Sister Corita Kent]], [[Tamy Ben Tor]], [[Anton Henning]], [[Dana Schutz]], and [[Mark Flood]].

==External links==
* [http://www.zachfeuer.com Zach Feuer Gallery] website

{{coord missing|New York}}

[[Category:Contemporary art galleries]]
[[Category:2000 establishments]]
[[Category:Art galleries in Manhattan]]"));

            Assert.AreEqual(@"<ref>[http://www.findagrave.com/cgi-bin/fg.cgi?page=gr&GRid=5194 Find A Grave]</ref>", Parsers.FixSyntax(@"<ref>{http://www.findagrave.com/cgi-bin/fg.cgi?page=gr&GRid=5194 Find A Grave]</ref>"));

            // convert [[[[link]] to [[link]] if that balances it all out
            Assert.AreEqual(@"hello [[link]] there", Parsers.FixSyntax(@"hello [[[[link]] there"));
            Assert.AreEqual(@"hello [[[[link]] there]]", Parsers.FixSyntax(@"hello [[[[link]] there]]"));

            // convert {blah) to (blah) if that balances it all out, not wikitables, templates
            Assert.AreEqual(@"hello (link) there", Parsers.FixSyntax(@"hello {link) there"));
            Assert.AreEqual(@"hello {|table|blah) there", Parsers.FixSyntax(@"hello {|table|blah) there"));
            Assert.AreEqual(@"{{cite web|title=a|url=http://www.site.com|publisher=ABC)|year=2006}", Parsers.FixSyntax(@"{{cite web|title=a|url=http://www.site.com|publisher=ABC)|year=2006}"));

            // convert [blah} to [blah] if that balances it all out
            Assert.AreEqual(@"*[http://aeiou.visao.pt/Pages/Lusa.aspx?News=200808068620453 Obituary notice] here", Parsers.FixSyntax(@"*[http://aeiou.visao.pt/Pages/Lusa.aspx?News=200808068620453 Obituary notice} here"));

            // don't touch template ends
            Assert.AreEqual(@"[http://aeiou.visao.pt/Pages/Lusa.aspx?News=200808068620453 Obituary notice}}", Parsers.FixSyntax(@"[http://aeiou.visao.pt/Pages/Lusa.aspx?News=200808068620453 Obituary notice}}"));

            // correct [[blah blah}word]] to [[blah blah|word]]
            Assert.AreEqual(@"[[blah blah|word]]", Parsers.FixSyntax(@"[[blah blah}word]]"));
            Assert.AreEqual(@"[[blah|word]]", Parsers.FixSyntax(@"[[blah}word]]"));

            // not if unbalanced brackets remain
            Assert.AreEqual(@"{ here [[blah blah}word]]", Parsers.FixSyntax(@"{ here [[blah blah}word]]"));

            // correct {[link]] or {[[link]] or [[[link]] or [[{link]]
            Assert.AreEqual(@"now [[link]] was", Parsers.FixSyntax(@"now {[link]] was"));
            Assert.AreEqual(@"now [[link]] was", Parsers.FixSyntax(@"now {[[link]] was"));
            Assert.AreEqual(@"now [[link]] was", Parsers.FixSyntax(@"now [[[link]] was"));
            Assert.AreEqual(@"now [[link]] was", Parsers.FixSyntax(@"now [[{link]] was"));

            // not if unbalanced brackets remain nearby
            Assert.AreEqual(@"now {[[link]]} was", Parsers.FixSyntax(@"now {[link]]} was"));
            Assert.AreEqual(@"now [[[link]] was]", Parsers.FixSyntax(@"now [[[link]] was]"));

            // convert [[link]]]] to [[link]] IFF that balances it all out
            Assert.AreEqual(@"hello [[link]] there", Parsers.FixSyntax(@"hello [[link]]]] there"));
            Assert.AreEqual(@"[[hello [[link]]]] there", Parsers.FixSyntax(@"[[hello [[link]]]] there"));

            // external links missing brackets
            Assert.AreEqual(@"blah
* [http://www.site.com a site]
* [http://www.site2.com another]", Parsers.FixSyntax(@"blah
* [http://www.site.com a site
* [http://www.site2.com another]"));
            Assert.AreEqual(@"blah
* [http://www.site.com a site]
* [http://www.site2.com another]", Parsers.FixSyntax(@"blah
* http://www.site.com a site]
* [http://www.site2.com another]"));
        }

        [Test]
        public void UppercaseCiteFields()
        {
            // single uppercase field
            Assert.AreEqual(@"{{cite web|url=http://members.bib-arch.org|title=hello}}", Parsers.FixSyntax(@"{{cite web|URL=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{cite web|url=http://members.bib-arch.org|title=hello}}", Parsers.FixSyntax(@"{{cite web|uRL=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{citeweb|url=http://members.bib-arch.org|title=hello}}", Parsers.FixSyntax(@"{{citeweb|URL=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{cite web|url=http://members.bib-arch.org|title=hello}}", Parsers.FixSyntax(@"{{cite web|UrL=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{cite web|url=http://members.bib-arch.org|title=hello}}", Parsers.FixSyntax(@"{{cite web|Url=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{Cite web|url=http://members.bib-arch.org|title=hello}}", Parsers.FixSyntax(@"{{Cite web|URL=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{cite web | url=http://members.bib-arch.org|title=hello}}", Parsers.FixSyntax(@"{{cite web | URL=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{cite web| url = http://members.bib-arch.org|title=hello}}", Parsers.FixSyntax(@"{{cite web| URL = http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{cite web|url =http://members.bib-arch.org|title=HELLO}}", Parsers.FixSyntax(@"{{cite web|URL =http://members.bib-arch.org|title=HELLO}}"));

            // multiple uppercase fields
            Assert.AreEqual(@"{{cite web|url=http://members.bib-arch.org|title=hello}}", Parsers.FixSyntax(@"{{cite web|URL=http://members.bib-arch.org|Title=hello}}"));
            Assert.AreEqual(@"{{cite web|url=http://members.bib-arch.org|title=hello}}", Parsers.FixSyntax(@"{{cite web|URL=http://members.bib-arch.org|TITLE=hello}}"));
            Assert.AreEqual(@"{{cite web|url=http://members.bib-arch.org|title=hello | publisher=BBC}}", Parsers.FixSyntax(@"{{cite web|URL=http://members.bib-arch.org|TITLE=hello | Publisher=BBC}}"));

            //other templates
            Assert.AreEqual(@"{{cite web|url=http://members.bib-arch.org|title=hello}}", Parsers.FixSyntax(@"{{cite web|URL=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{cite book|url=http://members.bib-arch.org|title=hello}}", Parsers.FixSyntax(@"{{cite book|URL=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{cite news|url=http://members.bib-arch.org|title=hello}}", Parsers.FixSyntax(@"{{cite news|URL=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{cite journal|url=http://members.bib-arch.org|title=hello}}", Parsers.FixSyntax(@"{{cite journal|URL=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{cite paper|url=http://members.bib-arch.org|title=hello}}", Parsers.FixSyntax(@"{{cite paper|URL=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{cite press release|url=http://members.bib-arch.org|title=hello}}", Parsers.FixSyntax(@"{{cite press release|URL=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{cite hansard|url=http://members.bib-arch.org|title=hello}}", Parsers.FixSyntax(@"{{cite hansard|URL=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{cite encyclopedia|url=http://members.bib-arch.org|title=hello}}", Parsers.FixSyntax(@"{{cite encyclopedia|URL=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{citation|url=http://members.bib-arch.org|title=hello}}", Parsers.FixSyntax(@"{{citation|URL=http://members.bib-arch.org|title=hello}}"));

            Assert.AreEqual(@"{{Cite web|url=http://members.bib-arch.org|title=hello}}", Parsers.FixSyntax(@"{{Cite web|URL=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{Cite book|url=http://members.bib-arch.org|title=hello}}", Parsers.FixSyntax(@"{{Cite book|URL=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{Cite news|url=http://members.bib-arch.org|title=hello}}", Parsers.FixSyntax(@"{{Cite news|URL=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{Cite journal|url=http://members.bib-arch.org|title=hello}}", Parsers.FixSyntax(@"{{Cite journal|URL=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{Cite paper|url=http://members.bib-arch.org|title=hello}}", Parsers.FixSyntax(@"{{Cite paper|URL=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{Cite press release|url=http://members.bib-arch.org|title=hello}}", Parsers.FixSyntax(@"{{Cite press release|URL=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{Cite hansard|url=http://members.bib-arch.org|title=hello}}", Parsers.FixSyntax(@"{{Cite hansard|URL=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{Cite encyclopedia|url=http://members.bib-arch.org|title=hello}}", Parsers.FixSyntax(@"{{Cite encyclopedia|URL=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{Citation|url=http://members.bib-arch.org|title=hello}}", Parsers.FixSyntax(@"{{Citation|URL=http://members.bib-arch.org|title=hello}}"));

            Assert.AreEqual(@"{{cite book | author=Smith | title=Great Book | ISBN=15478454 | date=17 May 2004 }}", Parsers.FixSyntax(@"{{cite book | author=Smith | Title=Great Book | ISBN=15478454 | date=17 May 2004 }}"));

            // no match if value of field uppercase
            Assert.AreEqual(@"{{cite web|url=http://members.bib-arch.org|title=hello | publisher=BBC}}", Parsers.FixSyntax(@"{{cite web|url=http://members.bib-arch.org|title=hello | publisher=BBC}}"));

            // ISBN is allowed to be uppercase
            Assert.AreEqual(@"{{cite book | author=Smith | title=Great Book | ISBN=15478454 | date=17 May 2004 }}", Parsers.FixSyntax(@"{{cite book | author=Smith | title=Great Book | ISBN=15478454 | date=17 May 2004 }}"));
        }

        [Test]
        public void TestCellpaddingTypo()
        {
            Assert.AreEqual(@"now {| class=""wikitable"" cellpadding=2", Parsers.FixSyntax(@"now {| class=""wikitable"" celpadding=2"));
            Assert.AreEqual(@"now {| class=""wikitable"" cellpadding=2", Parsers.FixSyntax(@"now {| class=""wikitable"" cellpading=2"));
            Assert.AreEqual(@"now {| class=""wikitable"" cellpadding=2", Parsers.FixSyntax(@"now {| class=""wikitable"" Celpading=2"));

            // no Matches
            Assert.AreEqual("now the cellpading of the", Parsers.FixSyntax("now the cellpading of the"));
            Assert.AreEqual(@"now {| class=""wikitable"" cellpadding=2", Parsers.FixSyntax(@"now {| class=""wikitable"" cellpadding=2"));
        }

        [Test]
        public void TestAccessdateTypo()
        {
            string Correct = @"now {{cite web | url=http://site.it | title=hello|accessdate = 2008-04-23 }} was";
            Assert.AreEqual(Correct, Parsers.FixSyntax(@"now {{cite web | url=http://site.it | title=hello|acessdate = 2008-04-23 }} was"));
            Assert.AreEqual(Correct, Parsers.FixSyntax(@"now {{cite web | url=http://site.it | title=hello|accesdate = 2008-04-23 }} was"));
            Assert.AreEqual(Correct, Parsers.FixSyntax(@"now {{cite web | url=http://site.it | title=hello|Acessdate = 2008-04-23 }} was"));
            Assert.AreEqual(Correct, Parsers.FixSyntax(@"now {{cite web | url=http://site.it | title=hello|Accesdate = 2008-04-23 }} was"));
            Assert.AreEqual(Correct, Parsers.FixSyntax(@"now {{cite web | url=http://site.it | title=hello|Acesdate = 2008-04-23 }} was"));
            Assert.AreEqual(Correct, Parsers.FixSyntax(@"now {{cite web | url=http://site.it | title=hello|acesdate = 2008-04-23 }} was"));
            Assert.AreEqual(Correct, Parsers.FixSyntax(@"now {{cite web | url=http://site.it | title=hello|acccesdate = 2008-04-23 }} was"));
            Assert.AreEqual(Correct, Parsers.FixSyntax(@"now {{cite web | url=http://site.it | title=hello|acccessdate = 2008-04-23 }} was"));

            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|accessdate = 2008-04-23|publisher=BBC }} was", Parsers.FixSyntax(@"now {{cite web | url=http://site.it | title=hello|acccessdate = 2008-04-23|publisher=BBC }} was"));
            Assert.AreEqual(@"now {{Cite web | url=http://site.it | title=hello|accessdate = 2008-04-23  |publisher=BBC }} was", Parsers.FixSyntax(@"now {{Cite web | url=http://site.it | title=hello|acccessdate = 2008-04-23  |publisher=BBC }} was"));

            // no match
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|acccessdated = 2008-04-23 }} was", Parsers.FixSyntax(@"now {{cite web | url=http://site.it | title=hello|acccessdated = 2008-04-23 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|accessdate = 2008-04-23 }} was", Parsers.FixSyntax(@"now {{cite web | url=http://site.it | title=hello|accessdate = 2008-04-23 }} was"));
        }

        [Test]
        public void TestCiteTemplateDates()
        {
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|accessdate = 2008-03-25 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|accessdate = 25/03/2008 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|accessdate = 2008-11-25 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|accessdate = 25/11/2008 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|accessdate = 2008-03-25 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|accessdate = 25/3/2008 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|accessdate = 2008-04-21 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|accessdate = 4/21/2008 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|accessdate = 2008-04-21 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|accessdate = 04/21/2008 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|accessdate = 2008-04-04 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|accessdate = 04/04/2008 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|accessdate = 2008-04-04 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|accessdate = 4/4/2008 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|accessdate = 2008-03-25 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|accessdate = 25/03/08 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|accessdate = 2008-03-30 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|accessdate = 30/03/08 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|accessdate = 2008-03-25 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|accessdate = 25/3/08 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|accessdate = 2008-04-21 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|accessdate = 4/21/08 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|accessdate = 2008-04-21 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|accessdate = 04/21/08 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|accessdate = 2008-04-04 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|accessdate = 04/04/08 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|accessdate = 2008-04-04 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|accessdate = 4/4/08 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|accessdate = 2008-04-21 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|accessdate = 04.21.08 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|accessdate = 2008-04-21 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|accessdate = 04.21.2008 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|accessdate = 2008-05-21 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|accessdate = 21.05.2008 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|accessdate = 2008-05-21 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|accessdate = 21.5.2008 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|accessdate = 2008-05-13 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|accessdate = 13.5.2008 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|accessdate = 2008-04-21 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|accessdate = 04-21-08 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|accessdate = 2008-04-21 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|accessdate = 04-21-2008 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|accessdate = 2008-04-21 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|accessdate = 21-04-2008 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|accessdate = 2008-05-21 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|accessdate = 21-5-2008 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|accessdate = 2011-05-21 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|accessdate = 21-5-2011 }} was"));
            Assert.AreEqual(@"now {{Cite web | url=http://site.it | title=hello|accessdate = 2008-05-21 }} was", Parsers.CiteTemplateDates(@"now {{Cite web | url=http://site.it | title=hello|accessdate = 21-5-08 }} was"));

            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|archivedate = 2008-03-25 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|archivedate = 25/03/2008 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|archivedate = 2008-11-25 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|archivedate = 25/11/2008 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|archivedate = 2008-03-25 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|archivedate = 25/3/2008 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|archivedate = 2008-04-21 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|archivedate = 4/21/2008 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|archivedate = 2008-04-21 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|archivedate = 04/21/2008 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|archivedate = 2008-04-04 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|archivedate = 04/04/2008 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|archivedate = 2008-04-04 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|archivedate = 4/4/2008 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|archivedate = 2008-03-25 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|archivedate = 25/03/08 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|archivedate = 2008-03-30 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|archivedate = 30/03/08 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|archivedate = 2008-03-25 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|archivedate = 25/3/08 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|archivedate = 2008-04-21 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|archivedate = 4/21/08 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|archivedate = 2008-04-21 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|archivedate = 04/21/08 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|archivedate = 2008-04-04 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|archivedate = 04/04/08 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|archivedate = 2008-04-04 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|archivedate = 4/4/08 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|archivedate = 2008-04-21 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|archivedate = 04.21.08 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|archivedate = 2008-04-21 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|archivedate = 04.21.2008 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|archivedate = 2008-05-21 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|archivedate = 21.05.2008 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|archivedate = 2008-05-21 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|archivedate = 21.5.2008 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|archivedate = 2008-05-13 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|archivedate = 13.5.2008 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|archivedate = 2008-04-21 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|archivedate = 04-21-08 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|archivedate = 2008-04-21 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|archivedate = 04-21-2008 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|archivedate = 2008-04-21 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|archivedate = 21-04-2008 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|archivedate = 2008-05-21 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|archivedate = 21-5-2008 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|archivedate = 2011-05-21 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|archivedate = 21-5-2011 }} was"));
            Assert.AreEqual(@"now {{Cite web | url=http://site.it | title=hello|archivedate = 2008-05-21 }} was", Parsers.CiteTemplateDates(@"now {{Cite web | url=http://site.it | title=hello|archivedate = 21-5-08 }} was"));

            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-03-25 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 25/03/2008 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-11-25 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 25/11/2008 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-03-25 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 25/3/2008 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-04-21 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 4/21/2008 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-04-21 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 04/21/2008 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 1980-04-21 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 04/21/1980 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-04-04 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 04/04/2008 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-04-04 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 4/4/2008 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-03-25 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 25/03/08 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-03-30 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 30/03/08 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-03-25 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 25/3/08 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-04-21 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 4/21/08 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-04-21 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 04/21/08 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-04-04 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 04/04/08 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-04-04 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 4/4/08 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-04-21 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 04.21.08 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-04-21 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 04.21.2008 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-05-21 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 21.05.2008 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-05-21 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 21.5.2008 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-05-13 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 13.5.2008 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-04-21 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 04-21-08 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-04-21 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 04-21-2008 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-04-21 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 21-04-2008 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-05-21 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 21-5-2008 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2011-05-21 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 21-5-2011 }} was"));
            Assert.AreEqual(@"now {{Cite web | url=http://site.it | title=hello|date = 2008-05-21 |publisher=BBC}} was", Parsers.CiteTemplateDates(@"now {{Cite web | url=http://site.it | title=hello|date = 21-5-08 |publisher=BBC}} was"));
            Assert.AreEqual(@"now {{Cite web | url=http://site.it | title=hello|date = 2008-05-21|publisher=BBC}} was", Parsers.CiteTemplateDates(@"now {{Cite web | url=http://site.it | title=hello|date = 21-5-08|publisher=BBC}} was"));

            // date = YYYY-Month-DD fix
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-01-11 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 2008-Jan-11 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-02-11 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 2008-Feb-11 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-03-11 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 2008-Mar-11 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-04-11 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 2008-Apr-11 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-05-11 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 2008-May-11 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-06-11 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 2008-Jun-11 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-07-11 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 2008-Jul-11 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-08-11 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 2008-Aug-11 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-09-11 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 2008-Sep-11 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-10-11 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 2008-Oct-11 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-11-11 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 2008-Nov-11 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-12-11 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 2008-Dec-11 }} was"));

            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-01-11 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 2008-January-11 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-02-11 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 2008-February-11 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-03-11 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 2008-March-11 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-04-11 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 2008-April-11 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-06-11 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 2008-June-11 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-07-11 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 2008-July-11 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-08-11 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 2008-August-11 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-09-11 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 2008-September-11 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-10-11 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 2008-October-11 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-11-11 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 2008-November-11 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-12-11 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 2008-December-11 }} was"));

            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-01-07 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 2008-Jan.-07 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-02-07 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 2008-Feb.-07 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-03-07 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 2008-Mar.-07 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-04-07 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 2008-Apr.-07 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-06-07 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 2008-Jun.-07 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-07-07 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 2008-Jul.-07 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-08-07 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 2008-Aug.-07 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-09-07 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 2008-Sep.-07 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-10-07 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 2008-Oct.-07 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-11-07 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 2008-Nov.-07 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-12-07 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 2008-Dec.-07 }} was"));

            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-01-07 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 2008 Jan. 07 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-01-07 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 2008 Jan 07 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 1998-01-07 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 1998 January 07 }} was"));

            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|archivedate = 2008-12-07 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|archivedate = 2008-Dec.-07 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|airdate = 2008-12-07 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|airdate = 2008-Dec.-07 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date2 = 2008-12-07 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date2 = 2008-Dec.-07 }} was"));

            // no change – ambiguous bbetween Am and Int date format
            Assert.AreEqual(@"<ref>{{cite web|url=http://www.nuclearweaponarchive.org/Russia/TsarBomba.html|title=The Tsar Bomba (King of Bombs)|accessdate=11-05-2006}}</ref>", Parsers.CiteTemplateDates(@"<ref>{{cite web|url=http://www.nuclearweaponarchive.org/Russia/TsarBomba.html|title=The Tsar Bomba (King of Bombs)|accessdate=11-05-2006}}</ref>"));
        }

        [Test]
        public void ExtraBracketInExternalLink()
        {
            //http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_9#Bug_in_regex_to_correct_double_bracketed_external_links
            Assert.AreEqual("now [http://www.site.com a [[a]] site] was", Parsers.FixSyntax("now [http://www.site.com a [[a]] site] was"));  // valid syntax
            Assert.AreEqual("now [http://www.site.com a b site] was", Parsers.FixSyntax("now [http://www.site.com a b site]] was"));
            Assert.AreEqual("now [http://www.site.com a c site] was", Parsers.FixSyntax("now [[http://www.site.com a c site] was"));
            Assert.AreEqual("now [http://www.site.com a d3 site] was", Parsers.FixSyntax("now [[http://www.site.com a d3 site]] was"));
            Assert.AreEqual("now [[Image:Fred1211212.JPG| here [http://www.site.com a [[e]] site]]] was", Parsers.FixSyntax("now [[Image:Fred1211212.JPG| here [http://www.site.com a [[e]] site]]] was"));   // valid wiki syntax
            Assert.AreEqual("now [[Image:Fred12.JPG| here [http://www.site.com a g site]]] was", Parsers.FixSyntax("now [[Image:Fred12.JPG| here [[http://www.site.com a g site]]] was"));
            Assert.AreEqual("now [http://www.site.com a [[b]] site] was", Parsers.FixSyntax("now [http://www.site.com a [[b]] site]] was"));
            Assert.AreEqual("now [http://www.site.com a [[c]] site] was", Parsers.FixSyntax("now [[http://www.site.com a [[c]] site] was"));
            Assert.AreEqual("now [http://www.site.com a [[d]] or [[d2]] site] was", Parsers.FixSyntax("now [[http://www.site.com a [[d]] or [[d2]] site]] was"));
            Assert.AreEqual("now [[Image:Fred12.JPG| here [http://www.site.com a [[f]] site]]] was", Parsers.FixSyntax("now [[Image:Fred12.JPG| here [http://www.site.com a [[f]] site]]]] was"));
            Assert.AreEqual("now [[Image:Fred12.JPG| here [http://www.site.com a g site]]] was", Parsers.FixSyntax("now [[Image:Fred12.JPG| here [http://www.site.com a g site]]]] was"));
            Assert.AreEqual("now [[Image:Fred12.JPG| here [http://www.site.com a g site]]] was", Parsers.FixSyntax("now [[Image:Fred12.JPG| here [[http://www.site.com a g site]]]] was"));
            Assert.AreEqual("[[Image:foo.jpg|Some [http://some_crap.com]]]", Parsers.FixSyntax("[[Image:foo.jpg|Some [[http://some_crap.com]]]]"));
        }

        [Test]
        public void SquareBracketsInExternalLinksTests()
        {
            Assert.AreEqual(@"[http://www.site.com some stuff [great&#93;]", Parsers.FixSyntax(@"[http://www.site.com some stuff [great]]"));
            Assert.AreEqual(@"[http://www.site.com some stuff [great&#93; free]", Parsers.FixSyntax(@"[http://www.site.com some stuff [great] free]"));
            Assert.AreEqual(@"[http://www.site.com some stuff [great&#93; free [here&#93;]", Parsers.FixSyntax(@"[http://www.site.com some stuff [great] free [here]]"));
            Assert.AreEqual(@"[http://www.site.com some stuff [great&#93; free [[now]]]", Parsers.FixSyntax(@"[http://www.site.com some stuff [great] free [[now]]]"));

            // no delinking needed
            Assert.AreEqual(@"[http://www.site.com some great stuff]", Parsers.FixSyntax(@"[http://www.site.com some great stuff]"));
            Assert.AreEqual(@"now [http://www.site.com some great stuff] here", Parsers.FixSyntax(@"now [http://www.site.com some great stuff] here"));
            Assert.AreEqual(@"<ref>[http://www.pubmedcentral.nih.gov/articlerender.fcgi?artid=32159 Message to 
complementary and alternative medicine: evidence is a better friend than power. Andrew J Vickers]</ref>", Parsers.FixSyntax(@"<ref>[http://www.pubmedcentral.nih.gov/articlerender.fcgi?artid=32159 Message to 
complementary and alternative medicine: evidence is a better friend than power. Andrew J Vickers]</ref>"));
            Assert.AreEqual(@"[http://www.site.com some [[great]] stuff]", Parsers.FixSyntax(@"[http://www.site.com some [[great]] stuff]"));
            Assert.AreEqual(@"[http://www.site.com some great [[stuff]]]", Parsers.FixSyntax(@"[http://www.site.com some great [[stuff]]]"));
            Assert.AreEqual(@"[http://www.site.com [[some great stuff|loads of stuff]]]", Parsers.FixSyntax(@"[http://www.site.com [[some great stuff|loads of stuff]]]"));
            Assert.AreEqual(@"[http://www.site.com [[some great stuff|loads of stuff]] here]", Parsers.FixSyntax(@"[http://www.site.com [[some great stuff|loads of stuff]] here]"));

            // don't match beyond </ref> tags
            Assert.AreEqual(@"[http://www.site.com some </ref> [stuff] here]", Parsers.FixSyntax(@"[http://www.site.com some </ref> [stuff] here]"));
        }

        [Test]
        public void TestBracketsAtCiteTemplateURL()
        {
            string Correct = @"now {{cite web|url=http://site.net|title=hello}} was";

            Assert.AreEqual(Correct, Parsers.FixSyntax(@"now {{cite web|url=[http://site.net]|title=hello}} was"));
            Assert.AreEqual(Correct, Parsers.FixSyntax(@"now {{cite web|url=http://site.net]|title=hello}} was"));
            Assert.AreEqual(Correct, Parsers.FixSyntax(@"now {{cite web|url=[http://site.net|title=hello}} was"));
            Assert.AreEqual(Correct, Parsers.FixSyntax(@"now {{cite web|url=[[http://site.net]|title=hello}} was"));
            Assert.AreEqual(Correct, Parsers.FixSyntax(@"now {{cite web|url=[http://site.net]]|title=hello}} was"));
            Assert.AreEqual(Correct, Parsers.FixSyntax(@"now {{cite web|url=[[http://site.net]]|title=hello}} was"));
            Assert.AreEqual(@"now {{cite web|url = http://site.net  |title=hello}} was", Parsers.FixSyntax(@"now {{cite web|url = [http://site.net]  |title=hello}} was"));
            Assert.AreEqual(@"now {{cite web|title=hello |url=www.site.net}} was", Parsers.FixSyntax(@"now {{cite web|title=hello |url=[www.site.net]}} was"));
            Assert.AreEqual(@"now {{cite journal|title=hello | url=site.net }} was", Parsers.FixSyntax(@"now {{cite journal|title=hello | url=[site.net]] }} was"));

            // no match
            Assert.AreEqual(@"now {{cite web| url=http://site.net|title=hello}} was", Parsers.FixSyntax(@"now {{cite web| url=http://site.net|title=hello}} was"));
            Assert.AreEqual(@"now {{cite web| url=[http://site.net cool site]|title=hello}} was", Parsers.FixSyntax(@"now {{cite web| url=[http://site.net cool site]|title=hello}} was"));
            Assert.AreEqual(@"now {{cite web| url=http://site.net cool site]|title=hello}} was", Parsers.FixSyntax(@"now {{cite web| url=http://site.net cool site]|title=hello}} was"));
            Assert.AreEqual(@"now {{cite web| url=[http://site.net cool site|title=hello}} was", Parsers.FixSyntax(@"now {{cite web| url=[http://site.net cool site|title=hello}} was"));
        }

        [Test]
        public void TestBulletExternalLinks()
        {
            string s = Parsers.BulletExternalLinks(@"==External links==
http://example.com/foo
[http://example.com foo]
{{aTemplate|url=
http://example.com }}");

            StringAssert.Contains("* http://example.com/foo", s);
            StringAssert.Contains("* [http://example.com foo]", s);

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_2#Incorrect_bulleting
            StringAssert.Contains("\r\nhttp://example.com }}", s);
        }

        [Test]
        public void TestFixLinkWhitespace()
        {
            Assert.AreEqual("b [[a]] c", Parsers.FixLinkWhitespace("b[[ a ]]c")); // regexes 1 & 2
            Assert.AreEqual("b   [[a]]  c", Parsers.FixLinkWhitespace("b   [[ a ]]  c")); // 4 & 5

            Assert.AreEqual("[[a]] b", Parsers.FixLinkWhitespace("[[a ]]b"));

            Assert.AreEqual("[[foo bar]]", Parsers.FixLinkWhitespace("[[foo  bar]]"));
            Assert.AreEqual("[[foo bar]]", Parsers.FixLinkWhitespace("[[foo     bar]]"));
            Assert.AreEqual("dat is [[foo bar]] show!", Parsers.FixLinkWhitespace("dat is [[ foo   bar ]] show!"));
            Assert.AreEqual("dat is [[foo bar]] show!", Parsers.FixLinkWhitespace("dat is[[ foo   bar ]]show!"));

            Assert.AreEqual(@"His [[Tiger Woods#Career]] was", Parsers.FixLinkWhitespace(@"His [[Tiger Woods# Career]] was", "Tiger Woods"));

            // don't fix when bit before # is not article name
            Assert.AreNotSame(@"Fred's [[Smith#Career]] was", Parsers.FixLinkWhitespace(@"Fred's [[Smith# Career]] was", "Fred"));

            Assert.AreEqual(@"[[Category:London| ]]", Parsers.FixLinkWhitespace(@"[[Category:London| ]]")); // leading space NOT removed from cat sortkey
            Assert.AreEqual(@"[[Category:Slam poetry| ]] ", Parsers.FixLinkWhitespace(@"[[Category:Slam poetry| ]] ")); // leading space NOT removed from cat sortkey

            // shouldn't fix - not enough information
            //Assert.AreEqual("[[ a ]]", Parsers.FixLinkWhitespace("[[ a ]]"));
            //disabled for the time being to avoid unnecesary clutter
        }

        [Test, Category("Incomplete")]
        public void TestCanonicalizeTitle()
        {
            Assert.AreEqual("foo (bar)", Parsers.CanonicalizeTitle("foo_%28bar%29"));

            // it may or may not fix it, but shouldn't break anything
            StringAssert.Contains("{{bar_boz}}", Parsers.CanonicalizeTitle("foo_bar{{bar_boz}}"));
        }
        [Test]
        public void TestFixCategories()
        {
            Assert.AreEqual("[[Category:Foo bar]]", Parsers.FixCategories("[[ categOry : Foo_bar]]"));
            Assert.AreEqual("[[Category:Foo bar|boz]]", Parsers.FixCategories("[[ categOry : Foo_bar|boz]]"));
            Assert.AreEqual("[[Category:Foo bar|quux]]", Parsers.FixCategories("[[category : foo_bar%20|quux]]"));

            // http://en.wikipedia.org/w/index.php?title=Wikipedia_talk:AutoWikiBrowser/Bugs&oldid=262844859#General_fixes_remove_spaces_from_category_sortkeys
            Assert.AreEqual(@"[[Category:Public transport in Auckland| Public transport in Auckland]]", Parsers.FixCategories(@"[[Category:Public transport in Auckland| Public transport in Auckland]]"));
            Assert.AreEqual(@"[[Category:Actors|Fred Astaire]]", Parsers.FixCategories(@"[[Category:Actors|Fred Astaire ]]")); // trailing space IS removed
            Assert.AreEqual(@"[[Category:Actors| Fred Astaire]]", Parsers.FixCategories(@"[[Category:Actors| Fred Astaire ]]")); // trailing space IS removed
            Assert.AreEqual(@"[[Category:London| ]]", Parsers.FixCategories(@"[[Category:London| ]]")); // leading space NOT removed
            Assert.AreEqual(@"[[Category:Slam poetry| ]] ", Parsers.FixCategories(@"[[Category:Slam poetry| ]] ")); // leading space NOT removed

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Archive_18#.2Fdoc_pages_and_includeonly_sections
            Assert.AreEqual("[[Category:Foo bar|boz_quux]]", Parsers.FixCategories("[[Category: foo_bar |boz_quux]]"));
            Assert.AreEqual("[[Category:Foo bar|{{boz_quux}}]]", Parsers.FixCategories("[[Category: foo_bar|{{boz_quux}}]]"));
            StringAssert.Contains("{{{boz_quux}}}", Parsers.FixCategories("[[CategorY : foo_bar{{{boz_quux}}}]]"));
            Assert.AreEqual("[[Category:Foo bar|{{{boz_quux}}}]]", Parsers.FixCategories("[[CategorY : foo_bar|{{{boz_quux}}}]]"));

            // diacritics removed from sortkeys
            Assert.AreEqual(@"[[Category:World Scout Committee members|Laine, Juan]]", Parsers.FixCategories(@"[[Category:World Scout Committee members|Lainé, Juan]]"));

            // brackets fixed
            Assert.AreEqual(@"[[Category:London]]", Parsers.FixCategories(@"[[Category:London]]]"));
            Assert.AreEqual(@"[[Category:London]]", Parsers.FixCategories(@"[[Category:London]]]]"));
            Assert.AreEqual(@"[[Category:London]]", Parsers.FixCategories(@"[[[[Category:London]]]"));
            Assert.AreEqual(@"[[Category:London]]", Parsers.FixCategories(@"[[[[Category:London]]]]"));
            Assert.AreEqual(@"[[Category:London]]", Parsers.FixCategories(@"[[Category:London]]"));
        }

        [Test]
        public void TestFixEmptyLinksAndTemplates()
        {
            Assert.AreEqual("", Parsers.FixEmptyLinksAndTemplates(""));

            Assert.AreEqual("Test", Parsers.FixEmptyLinksAndTemplates("Test"));

            Assert.AreEqual("{{Test}}", Parsers.FixEmptyLinksAndTemplates("{{Test}}"));

            Assert.AreEqual("Test\r\n{{Test}}", Parsers.FixEmptyLinksAndTemplates("Test\r\n{{Test}}"));

            Assert.AreEqual("", Parsers.FixEmptyLinksAndTemplates("{{}}"));
            Assert.AreEqual("", Parsers.FixEmptyLinksAndTemplates("{{ }}"));
            Assert.AreEqual("", Parsers.FixEmptyLinksAndTemplates("{{|}}"));
            Assert.AreEqual("", Parsers.FixEmptyLinksAndTemplates("{{||||||||||||||||}}"));
            Assert.AreEqual("", Parsers.FixEmptyLinksAndTemplates("{{|||| |||||||                      ||  |||}}"));

            Assert.AreEqual("", Parsers.FixEmptyLinksAndTemplates("{{Template:}}"));
            Assert.AreEqual("", Parsers.FixEmptyLinksAndTemplates("{{Template:     |||}}"));

            Assert.AreEqual("{{Test}}", Parsers.FixEmptyLinksAndTemplates("{{  }}{{Test}}{{Template: ||}}"));

            Assert.AreEqual("[[Test]]", Parsers.FixEmptyLinksAndTemplates("[[Test]]"));
            Assert.AreEqual("[[Test|Bar]]", Parsers.FixEmptyLinksAndTemplates("[[Test|Bar]]"));
            Assert.AreEqual("[[|Bar]]", Parsers.FixEmptyLinksAndTemplates("[[|Bar]]"));

            Assert.AreEqual("", Parsers.FixEmptyLinksAndTemplates("[[]]"));
            Assert.AreEqual("", Parsers.FixEmptyLinksAndTemplates("[[  ]]"));

            Assert.AreEqual("[[Category:Test]]", Parsers.FixEmptyLinksAndTemplates("[[Category:Test]]"));
            Assert.AreEqual("Text [[Category:Test]]", Parsers.FixEmptyLinksAndTemplates("Text [[Category:Test]]"));

            Assert.AreEqual("", Parsers.FixEmptyLinksAndTemplates("[[Category:]]"));
            Assert.AreEqual("Text ", Parsers.FixEmptyLinksAndTemplates("Text [[Category:]]"));
            Assert.AreEqual("", Parsers.FixEmptyLinksAndTemplates("[[Category:  ]]"));

            Assert.AreEqual("[[Image:Test]]", Parsers.FixEmptyLinksAndTemplates("[[Image:Test]]"));
            Assert.AreEqual("Text [[Image:Test]]", Parsers.FixEmptyLinksAndTemplates("Text [[Image:Test]]"));
            Assert.AreEqual("[[File:Test]]", Parsers.FixEmptyLinksAndTemplates("[[File:Test]]"));
            Assert.AreEqual("Text [[File:Test]]", Parsers.FixEmptyLinksAndTemplates("Text [[File:Test]]"));

            Assert.AreEqual("", Parsers.FixEmptyLinksAndTemplates("[[Image:]]"));
            Assert.AreEqual("Text ", Parsers.FixEmptyLinksAndTemplates("Text [[Image:]]"));
            Assert.AreEqual("", Parsers.FixEmptyLinksAndTemplates("[[Image:  ]]"));

            Assert.AreEqual("", Parsers.FixEmptyLinksAndTemplates("[[File:]]"));
            Assert.AreEqual("Text ", Parsers.FixEmptyLinksAndTemplates("Text [[File:]]"));
            Assert.AreEqual("", Parsers.FixEmptyLinksAndTemplates("[[File:  ]]"));

            Assert.AreEqual("", Parsers.FixEmptyLinksAndTemplates("[[File:]][[Image:]]"));
            Assert.AreEqual("[[File:Test]]", Parsers.FixEmptyLinksAndTemplates("[[File:Test]][[Image:]]"));
        }

        [Test]
        public void TestFixNonBreakingSpaces()
        {
            Assert.AreEqual(@"a 50&nbsp;km road", parser.FixNonBreakingSpaces(@"a 50 km road"));
            Assert.AreEqual(@"a 50&nbsp;km road", parser.FixNonBreakingSpaces(@"a 50km road"));
            Assert.AreEqual(@"a 50&nbsp;kg dog", parser.FixNonBreakingSpaces(@"a 50 kg dog"));
            Assert.AreEqual(@"a 50&nbsp;kg dog", parser.FixNonBreakingSpaces(@"a 50kg dog"));
            Assert.AreEqual(@"a 50&nbsp;cm road", parser.FixNonBreakingSpaces(@"a 50 cm road"));
            Assert.AreEqual(@"a 50&nbsp;cm road", parser.FixNonBreakingSpaces(@"a 50cm road"));
            Assert.AreEqual(@"a 50.247&nbsp;cm road", parser.FixNonBreakingSpaces(@"a 50.247cm road"));
            Assert.AreEqual(@"a 50.247&nbsp;nm laser", parser.FixNonBreakingSpaces(@"a 50.247nm laser"));
            Assert.AreEqual(@"a 50.247&nbsp;mm pen", parser.FixNonBreakingSpaces(@"a 50.247 mm pen"));
            Assert.AreEqual(@"a 50.247&nbsp;nm laser", parser.FixNonBreakingSpaces(@"a 50.247  nm laser"));
            Assert.AreEqual(@"a 50.247&nbsp;µm laser", parser.FixNonBreakingSpaces(@"a 50.247µm laser"));
            Assert.AreEqual(@"a 50.247&nbsp;cd light", parser.FixNonBreakingSpaces(@"a 50.247 cd light"));
            Assert.AreEqual(@"a 50.247&nbsp;cd light", parser.FixNonBreakingSpaces(@"a 50.247cd light"));
            Assert.AreEqual(@"a 50.247&nbsp;mmol solution", parser.FixNonBreakingSpaces(@"a 50.247mmol solution"));
            Assert.AreEqual(@"a 0.3&nbsp;mol solution", parser.FixNonBreakingSpaces(@"a 0.3mol solution"));

            // no changes for these
            Assert.AreEqual(@"nearly 5m people", parser.FixNonBreakingSpaces(@"nearly 5m people"));
            Assert.AreEqual(@"a 3CD set", parser.FixNonBreakingSpaces(@"a 3CD set"));
            Assert.AreEqual(@"http://site.com/View/3356 A show", parser.FixNonBreakingSpaces(@"http://site.com/View/3356 A show"));
            Assert.AreEqual(@"a 50&nbsp;km road", parser.FixNonBreakingSpaces(@"a 50&nbsp;km road"));
            Assert.AreEqual(@"over $200K in cash", parser.FixNonBreakingSpaces(@"over $200K in cash"));
            Assert.AreEqual(@"now {{a 50kg dog}} was", parser.FixNonBreakingSpaces(@"now {{a 50kg dog}} was"));
            Assert.AreEqual(@"now a [[50kg dog]] was", parser.FixNonBreakingSpaces(@"now a [[50kg dog]] was"));
            Assert.AreEqual(@"now “a 50kg dog” was", parser.FixNonBreakingSpaces(@"now “a 50kg dog” was"));
            Assert.AreEqual(@"now <!--a 50kg dog--> was", parser.FixNonBreakingSpaces(@"now <!--a 50kg dog--> was"));
            Assert.AreEqual(@"now <nowiki>a 50kg dog</nowiki> was", parser.FixNonBreakingSpaces(@"now <nowiki>a 50kg dog</nowiki> was"));
            Assert.AreEqual(@"*[http://site.com/blah_20cm_long Site here]", parser.FixNonBreakingSpaces("*[http://site.com/blah_20cm_long Site here]"));
            
            // firearms articles don't use spaces for ammo sizes
            Assert.AreEqual(@"the 50mm gun", parser.FixNonBreakingSpaces(@"the 50mm gun"));
        }
    }

    [TestFixture]
    public class FormattingTests : RequiresParser
    {
        [Test]
        public void TestBrConverter()
        {
            Assert.AreEqual("*a\r\nb", Parsers.FixSyntax("*a<br>\r\nb"));
            Assert.AreEqual("*a\r\nb", Parsers.FixSyntax("\r\n*a<br>\r\nb"));
            Assert.AreEqual("foo\r\n*a\r\nb", Parsers.FixSyntax("foo\r\n*a<br>\r\nb"));

            Assert.AreEqual("*a", Parsers.FixSyntax("*a<br>\r\n")); // \r\n\ trimmed

            Assert.AreEqual("*a", Parsers.FixSyntax("*a<br\\>\r\n"));
            Assert.AreEqual("*a", Parsers.FixSyntax("*a<br/>\r\n"));
            Assert.AreEqual("*a", Parsers.FixSyntax("*a <br\\> \r\n"));

            // leading (back)slash is hack for incorrectly formatted breaks per
            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_7#br_tags_are_not_always_removed
            Assert.AreEqual("*a", Parsers.FixSyntax("*a </br/> \r\n"));
            Assert.AreEqual("*a", Parsers.FixSyntax("*a<br\\> \r\n"));
            Assert.AreEqual("*a", Parsers.FixSyntax("*a <\\br\\>\r\n"));
            Assert.AreEqual("*a", Parsers.FixSyntax("*a     <br\\>    \r\n"));

            Assert.AreEqual("*:#;a\r\n*b", Parsers.FixSyntax("*:#;a<br>\r\n*b"));
            Assert.AreEqual("###;;;:::***a\r\nb", Parsers.FixSyntax("###;;;:::***a<br />\r\nb"));
            Assert.AreEqual("*&a\r\nb", Parsers.FixSyntax("*&a<br/>\r\nb"));

            Assert.AreEqual("&*a<br>\r\nb", Parsers.FixSyntax("&*a<br>\r\nb"));
            Assert.AreEqual("*a\r\n<br>\r\nb", Parsers.FixSyntax("*a\r\n<br>\r\nb"));
        }

        [Test]
        public void TestFixHeadings()
        {
            // breaks if article title is empty
            Assert.AreEqual("==foo==", Parsers.FixHeadings("=='''foo'''==", "test"));
            Assert.AreEqual("== foo ==", Parsers.FixHeadings("== '''foo''' ==", "test"));
            StringAssert.StartsWith("==foo==", Parsers.FixHeadings("=='''foo'''==\r\n", "test"));
            Assert.AreEqual("quux\r\n==foo==\r\nbar", Parsers.FixHeadings("quux\r\n=='''foo'''==\r\nbar", "test"));
            Assert.AreEqual("quux\r\n==foo==\r\n\r\nbar", Parsers.FixHeadings("quux\r\n=='''foo'''==\r\n\r\nbar", "test"));

            Assert.AreEqual("==foo==", Parsers.FixHeadings("==foo==", "test"));

            // following unit tests appear messy due to need to include whitespace and newlines
            Assert.AreEqual(@"hi.
==News==
Some news here.", Parsers.FixHeadings(@"hi.
 ==News==
Some news here.", "test"));
            Assert.AreEqual(@"hi.
==News place==
Some news here.", Parsers.FixHeadings(@"hi.
 ==News place==
Some news here.", "test"));
            Assert.AreEqual(@"hi.
==News place==
Some news here.", Parsers.FixHeadings(@"hi.
    ==News place==  
Some news here.", "test"));
            Assert.AreEqual(@"hi.
==News place==
Some news here.", Parsers.FixHeadings(@"hi.
==News place==  
Some news here.", "test"));

            // tests for regexRemoveHeadingsInLinks
            Assert.AreEqual("==foo==", Parsers.FixHeadings("==[[foo]]==", "test"));
            Assert.AreEqual("== foo ==", Parsers.FixHeadings("== [[foo]] ==", "test"));
            Assert.AreEqual("==bar==", Parsers.FixHeadings("==[[foo|bar]]==", "test"));

            // match
            Assert.AreEqual("==hello world ==", Parsers.FixHeadings("==hello [[world]] ==", "a"));
            Assert.AreEqual("==Hello world==", Parsers.FixHeadings("==[[Hello world]]==", "a"));
            Assert.AreEqual("==world==", Parsers.FixHeadings("==[[hello|world]]==", "a"));
            Assert.AreEqual("== world ==", Parsers.FixHeadings("== [[hello|world]] ==", "a"));
            Assert.AreEqual("==world==", Parsers.FixHeadings("==[[hello now|world]]==", "a"));
            Assert.AreEqual("==world now==", Parsers.FixHeadings("==[[hello|world now]]==", "a"));
            Assert.AreEqual("==now world==", Parsers.FixHeadings("==now [[hello|world]]==", "a"));
            Assert.AreEqual("==now world here==", Parsers.FixHeadings("==now [[hello|world]] here==", "a"));
            Assert.AreEqual("==now world def here==", Parsers.FixHeadings("==now [[hello abc|world def]] here==", "a"));
            Assert.AreEqual("==now world here==", Parsers.FixHeadings("==now [[ hello |world]] here==", "a"));
            Assert.AreEqual("==now world==", Parsers.FixHeadings("==now [[hello#world|world]]==", "a"));
            Assert.AreEqual("==now world and moon==", Parsers.FixHeadings("==now [[hello|world]] and [[bye|moon]]==", "a"));
            // no match
            Assert.AreEqual("===hello [[world]] ==", Parsers.FixHeadings("===hello [[world]] ==", "a"));
            Assert.AreEqual("==hello [[world]] ===", Parsers.FixHeadings("==hello [[world]] ===", "a"));
            Assert.AreEqual("== hello world ==", Parsers.FixHeadings("== hello world ==", "a"));
            Assert.AreEqual("==hello==", Parsers.FixHeadings("==hello==", "a"));
            Assert.AreEqual("==now [[hello|world] ] here==", Parsers.FixHeadings("==now [[hello|world] ] here==", "a"));
            Assert.AreEqual("==hello[http://example.net]==", Parsers.FixHeadings("==hello[http://example.net]==", "a"));
            Assert.AreEqual("hello [[world]]", Parsers.FixHeadings("hello [[world]]", "a"));
            Assert.AreEqual("now == hello [[world]] == here", Parsers.FixHeadings("now == hello [[world]] == here", "a"));

            // remove colon from end of heading text
            Assert.AreEqual(@"== hello world ==
", Parsers.FixHeadings(@"== hello world: ==
", "a"));
            Assert.AreEqual(@"== hello world  ==
", Parsers.FixHeadings(@"== hello world : ==
", "a"));
            Assert.AreEqual(@"== hello world ==
", Parsers.FixHeadings(@"=== hello world: ===
", "a"));
            Assert.AreEqual(@"== hello world ==
== hello world2 ==
", Parsers.FixHeadings(@"== hello world: ==
== hello world2: ==
", "a"));

            // no change if colon within heading text
            Assert.AreEqual(@"== hello:world ==
", Parsers.FixHeadings(@"== hello:world ==
", "a"));
        }

        [Test]
        public void TestFixHeadingsRemoveTwoLevels()
        {
            // single heading
            Assert.AreEqual(@"===hello===
text", Parsers.FixHeadings(@"====hello====
text", "a"));

            // multiple
            Assert.AreEqual(@"===hello===
text
=== hello2 ===
texty
==== hello3 ====
", Parsers.FixHeadings(@"====hello====
text
==== hello2 ====
texty
===== hello3 =====
", "a"));

            // level 1 not altered
            Assert.AreEqual(@"=level1=
text
===hello===
", Parsers.FixHeadings(@"=level1=
text
====hello====
", "a"));

            // no changes if already a level two
            Assert.AreEqual(@"==hi==
====hello====
", Parsers.FixHeadings(@"==hi==
====hello====
", "a"));

            // no changes on level 1 only
            Assert.AreEqual(@"=hello=
text", Parsers.FixHeadings(@"=hello=
text", "a"));
        }

        [Test, Category("Incomplete")]
        //TODO: cover everything
        public void TestFixWhitespace()
        {
            Assert.AreEqual("", Parsers.RemoveWhiteSpace("     "));
            Assert.AreEqual("a\r\n\r\n b", Parsers.RemoveWhiteSpace("a\r\n\r\n\r\n b"));
            //Assert.AreEqual(" a", Parsers.RemoveWhiteSpace(" a")); // fails, but it doesn't seem harmful, at least for
            // WMF projects with their design guidelines
            //Assert.AreEqual(" a", Parsers.RemoveWhiteSpace("\r\n a \r\n")); // same as above
            Assert.AreEqual("a", Parsers.RemoveWhiteSpace("\r\na \r\n")); // the above errors have effect only on the first line
            Assert.AreEqual("", Parsers.RemoveWhiteSpace("\r\n"));
            Assert.AreEqual("", Parsers.RemoveWhiteSpace("\r\n\r\n"));
            Assert.AreEqual("a\r\nb", Parsers.RemoveWhiteSpace("a\r\nb"));
            Assert.AreEqual("a\r\n\r\nb", Parsers.RemoveWhiteSpace("a\r\n\r\nb"));
            Assert.AreEqual("a\r\n\r\nb", Parsers.RemoveWhiteSpace("a\r\n\r\n\r\nb"));

            Assert.AreEqual("== foo ==\r\n==bar", Parsers.RemoveWhiteSpace("== foo ==\r\n==bar"));
            Assert.AreEqual("== foo ==\r\n==bar", Parsers.RemoveWhiteSpace("== foo ==\r\n\r\n==bar"));
            Assert.AreEqual("== foo ==\r\n==bar", Parsers.RemoveWhiteSpace("== foo ==\r\n\r\n\r\n==bar"));

            Assert.AreEqual("{|\r\n| foo\r\n|\r\nbar\r\n|}", Parsers.RemoveWhiteSpace("{|\r\n| foo\r\n\r\n|\r\n\r\nbar\r\n|}"));

            // eh? should we fix such tables too?
            //Assert.AreEqual("{|\r\n! foo\r\n!\r\nbar\r\n|}", Parsers.RemoveWhiteSpace("{|\r\n! foo\r\n\r\n!\r\n\r\nbar\r\n|}"));
        }

        [Test]
        public void TestMdashes()
        {
            Assert.AreEqual("pp. 55–57", parser.Mdashes("pp. 55-57", "test", 0));
            Assert.AreEqual("pp. 55 – 57", parser.Mdashes("pp. 55 - 57", "test", 0));
            Assert.AreEqual("pp 55–57", parser.Mdashes("pp 55-57", "test", 0));
            Assert.AreEqual("pp 1155–1157", parser.Mdashes("pp 1155-1157", "test", 0));
            Assert.AreEqual("pages= 55–57", parser.Mdashes("pages= 55-57", "test", 0));
            Assert.AreEqual("pages = 55–57", parser.Mdashes("pages = 55-57", "test", 0));
            Assert.AreEqual("pages=55–57", parser.Mdashes("pages=55-57", "test", 0));
            Assert.AreEqual("pages=55–57", parser.Mdashes("pages=55—57", "test", 0));
            Assert.AreEqual("pages=55–57", parser.Mdashes("pages=55&#8212;57", "test", 0));
            Assert.AreEqual("pages=55–57", parser.Mdashes("pages=55&mdash;57", "test", 0));

            Assert.AreEqual("55–57 miles", parser.Mdashes("55-57 miles", "test", 0));
            Assert.AreEqual("55–57 kg", parser.Mdashes("55-57 kg", "test", 0));
            Assert.AreEqual("55 – 57 kg", parser.Mdashes("55 - 57 kg", "test", 0));
            Assert.AreEqual("55–57&nbsp;kg", parser.Mdashes("55-57&nbsp;kg", "test", 0));
            Assert.AreEqual("55–57 Hz", parser.Mdashes("55-57 Hz", "test", 0));
            Assert.AreEqual("55–57 GHz", parser.Mdashes("55-57 GHz", "test", 0));

            Assert.AreEqual("$55–57", parser.Mdashes("$55-57", "test", 0));
            Assert.AreEqual("$55 – 57", parser.Mdashes("$55 - 57", "test", 0));
            Assert.AreEqual("$55–57", parser.Mdashes("$55-57", "test", 0));
            Assert.AreEqual("$1155–1157", parser.Mdashes("$1155-1157", "test", 0));
            Assert.AreEqual("$55–57", parser.Mdashes("$55&mdash;57", "test", 0));
            Assert.AreEqual("$55–57", parser.Mdashes("$55—57", "test", 0));

            Assert.AreEqual("5:17 AM – 5:19 AM", parser.Mdashes("5:17 AM - 5:19 AM", "test", 0));
            Assert.AreEqual("05:17 AM – 05:19 AM", parser.Mdashes("05:17 AM - 05:19 AM", "test", 0));
            Assert.AreEqual("11:17 PM – 11:19 PM", parser.Mdashes("11:17 PM - 11:19 PM", "test", 0));
            Assert.AreEqual("11:17 pm – 11:19 pm", parser.Mdashes("11:17 pm - 11:19 pm", "test", 0));
            Assert.AreEqual("11:17 pm – 11:19 pm", parser.Mdashes("11:17 pm &mdash; 11:19 pm", "test", 0));
            Assert.AreEqual("11:17 pm – 11:19 pm", parser.Mdashes("11:17 pm — 11:19 pm", "test", 0));

            Assert.AreEqual("Aged 5–9", parser.Mdashes("Aged 5–9", "test", 0));
            Assert.AreEqual("Aged 5–11", parser.Mdashes("Aged 5–11", "test", 0));
            Assert.AreEqual("Aged 5 – 9", parser.Mdashes("Aged 5 – 9", "test", 0));
            Assert.AreEqual("Aged 15–19", parser.Mdashes("Aged 15–19", "test", 0));
            Assert.AreEqual("Ages 15–19", parser.Mdashes("Ages 15–19", "test", 0));
            Assert.AreEqual("Aged 15–19", parser.Mdashes("Aged 15–19", "test", 0));

            Assert.AreEqual("(ages 15–18)", parser.Mdashes("(ages 15-18)", "test", 0));

            // double dash to emdash
            Assert.AreEqual(@"Djali Zwan made their live debut as a quartet—Corgan, Sweeney, Pajo and Chamberlin—at the end of 2001", parser.Mdashes(@"Djali Zwan made their live debut as a quartet -- Corgan, Sweeney, Pajo and Chamberlin -- at the end of 2001", "test", 0));
            Assert.AreEqual(@"Djali Zwan made their live debut as a quartet—Corgan, Sweeney, Pajo and Chamberlin—at the end of 2001", parser.Mdashes(@"Djali Zwan made their live debut as a quartet --  Corgan, Sweeney, Pajo and Chamberlin --at the end of 2001", "test", 0));
            Assert.AreEqual(@"Djali Zwan made their live debut as a quartet—Corgan, Sweeney, Pajo and Chamberlin—at the end of 2001", parser.Mdashes(@"Djali Zwan made their live debut as a quartet -- Corgan, Sweeney, Pajo and Chamberlin--at the end of 2001", "test", 0));

            // only applied on article namespace
            Assert.AreEqual(@"Djali Zwan made their live debut as a quartet -- Corgan, Sweeney, Pajo and Chamberlin -- at the end of 2001", parser.Mdashes(@"Djali Zwan made their live debut as a quartet -- Corgan, Sweeney, Pajo and Chamberlin -- at the end of 2001", "test", 1));

            // precisely two dashes only
            Assert.AreEqual(@"Djali Zwan made their live debut as a quartet --- Corgan, Sweeney, Pajo and Chamberlin - at the end of 2001", parser.Mdashes(@"Djali Zwan made their live debut as a quartet --- Corgan, Sweeney, Pajo and Chamberlin - at the end of 2001", "test", 0));
        }

        [Test]
        public void TestTitleDashes()
        {
            // en-dash
            Assert.AreEqual(@"The Alpher–Bethe–Gamow paper is great.", parser.Mdashes(@"The Alpher-Bethe-Gamow paper is great.", @"Alpher–Bethe–Gamow paper", 0));
            // em-dash
            Assert.AreEqual(@"The Alpher—Bethe—Gamow paper is great. The Alpher—Bethe—Gamow paper is old.", parser.Mdashes(@"The Alpher-Bethe-Gamow paper is great. The Alpher-Bethe-Gamow paper is old.", @"Alpher—Bethe—Gamow paper", 0));

            // all hyphens, no change
            Assert.AreEqual(@"The Alpher-Bethe-Gamow paper is great.", parser.Mdashes(@"The Alpher-Bethe-Gamow paper is great.", "Alpher-Bethe-Gamow paper", 0));
        }

        [Test]
        public void TestBulletListWhitespace()
        {
            Assert.AreEqual(@"* item
* item2
* item3", Parsers.RemoveAllWhiteSpace(@"*    item
*            item2
*item3"));

            Assert.AreEqual(@"# item
# item2
# item3", Parsers.RemoveAllWhiteSpace(@"#    item
#            item2
#item3"));

        }
    }

    [TestFixture]
    public class MOSTests : RequiresParser
    {
        [Test]
        public void TestFixDateOrdinalsAndOf()
        {
            // 'of' between month and year
            Assert.AreEqual(@"Now in July 2007 a new", parser.FixDateOrdinalsAndOf(@"Now in July of 2007 a new", "test"));
            Assert.AreEqual(@"Now ''Plaice'' in July 2007 a new", parser.FixDateOrdinalsAndOf(@"Now ''Plaice'' in July of 2007 a new", "test"));
            Assert.AreEqual(@"Now in January 1907 a new", parser.FixDateOrdinalsAndOf(@"Now in January of 1907 a new", "test"));
            Assert.AreEqual(@"Now in January 1807 a new", parser.FixDateOrdinalsAndOf(@"Now in January of 1807 a new", "test"));
            Assert.AreEqual(@"Now in January 1807 and May 1804 a new", parser.FixDateOrdinalsAndOf(@"Now in January of 1807 and May of 1804 a new", "test"));

            // no Matches
            Assert.AreEqual(@"Now ""in July of 2007"" a new", parser.FixDateOrdinalsAndOf(@"Now ""in July of 2007"" a new", "test"));
            Assert.AreEqual(@"Now {{quote|in July of 2007}} a new", parser.FixDateOrdinalsAndOf(@"Now {{quote|in July of 2007}} a new", "test"));
            Assert.AreEqual(@"Now ""in July of 1707"" a new", parser.FixDateOrdinalsAndOf(@"Now ""in July of 1707"" a new", "test"));
            Assert.AreEqual(@"Now a march of 2007 resulted", parser.FixDateOrdinalsAndOf(@"Now a march of 2007 resulted", "test"));
            Assert.AreEqual(@"Now the June of 2007 was", parser.FixDateOrdinalsAndOf(@"Now the June of 2007 was", "test"));

            // no ordinals on dates
            Assert.AreEqual(@"On 14 March elections were", parser.FixDateOrdinalsAndOf(@"On 14th March elections were", "test"));
            Assert.AreEqual(@"On 14 June elections were", parser.FixDateOrdinalsAndOf(@"On 14th June elections were", "test"));
            Assert.AreEqual(@"On March 14 elections were", parser.FixDateOrdinalsAndOf(@"On March 14th elections were", "test"));
            Assert.AreEqual(@"On June 21 elections were", parser.FixDateOrdinalsAndOf(@"On June 21st elections were", "test"));
            Assert.AreEqual(@"On 14 March 2008 elections were", parser.FixDateOrdinalsAndOf(@"On 14th March 2008 elections were", "test"));
            Assert.AreEqual(@"On 14 June 2008 elections were", parser.FixDateOrdinalsAndOf(@"On 14th June 2008 elections were", "test"));
            Assert.AreEqual(@"On March 14, 2008 elections were", parser.FixDateOrdinalsAndOf(@"On March 14th, 2008 elections were", "test"));
            Assert.AreEqual(@"On June 21, 2008 elections were", parser.FixDateOrdinalsAndOf(@"On June   21st, 2008 elections were", "test"));

            // date ranges
            Assert.AreEqual(@"On 14-15 June elections were", parser.FixDateOrdinalsAndOf(@"On 14th-15th June elections were", "test"));
            Assert.AreEqual(@"On 14 - 15 June elections were", parser.FixDateOrdinalsAndOf(@"On 14th - 15th June elections were", "test"));
            Assert.AreEqual(@"On 14 to 15 June elections were", parser.FixDateOrdinalsAndOf(@"On 14th to 15th June elections were", "test"));
            Assert.AreEqual(@"On June 14 to 15 elections were", parser.FixDateOrdinalsAndOf(@"On June 14th to 15th elections were", "test"));
            Assert.AreEqual(@"On 14,15 June elections were", parser.FixDateOrdinalsAndOf(@"On 14th,15th June elections were", "test"));
            Assert.AreEqual(@"On 3 and 15 June elections were", parser.FixDateOrdinalsAndOf(@"On 3rd and 15th June elections were", "test"));

            // no Matches, particularly dates with 'the' before where fixing the ordinal may leave 'on the 11 May' which wouldn't read well
            Assert.AreEqual(@"On 14th march was", parser.FixDateOrdinalsAndOf(@"On 14th march was", "test"));
            Assert.AreEqual(@"On 14th of February was", parser.FixDateOrdinalsAndOf(@"On 14th of February was", "test"));
            Assert.AreEqual(@"Now the 14th February was", parser.FixDateOrdinalsAndOf(@"Now the 14th February was", "test"));
            Assert.AreEqual(@"Now the February 14th was", parser.FixDateOrdinalsAndOf(@"Now the February 14th was", "test"));
            Assert.AreEqual(@"'''6th October City''' is", parser.FixDateOrdinalsAndOf(@"'''6th October City''' is", "6th October City"));
            Assert.AreEqual(@"<poem>On March 14th, 2008 elections were</poem>", parser.FixDateOrdinalsAndOf(@"<poem>On March 14th, 2008 elections were</poem>", "test"));

            // leading zeros
            Assert.AreEqual(@"On 3 June elections were", parser.FixDateOrdinalsAndOf(@"On 03 June elections were", "test"));
            Assert.AreEqual(@"On 7 June elections were", parser.FixDateOrdinalsAndOf(@"On 07 June elections were", "test"));
            Assert.AreEqual(@"On June 7, elections were", parser.FixDateOrdinalsAndOf(@"On June 07, elections were", "test"));
            Assert.AreEqual(@"On June 7 2008, elections were", parser.FixDateOrdinalsAndOf(@"On June 07 2008, elections were", "test"));
            Assert.AreEqual(@"On 3 June elections were", parser.FixDateOrdinalsAndOf(@"On 03rd June elections were", "test"));

            // no Matches
            Assert.AreEqual(@"The 007 march was", parser.FixDateOrdinalsAndOf(@"The 007 march was", "test"));
            Assert.AreEqual(@"In June '08 there was", parser.FixDateOrdinalsAndOf(@"In June '08 there was", "test"));
            Assert.AreEqual(@"In June 2008 there was", parser.FixDateOrdinalsAndOf(@"In June 2008 there was", "test"));
            Assert.AreEqual(@"On 00 June elections were", parser.FixDateOrdinalsAndOf(@"On 00 June elections were", "test"));
            Assert.AreEqual(@"The 007 May was", parser.FixDateOrdinalsAndOf(@"The 007 May was", "test"));
        }
    }

    [TestFixture]
    public class ImageTests : RequiresInitialization
    {
        [Test, Category("Incomplete")]
        public void BasicImprovements()
        {
            Assert.AreEqual("[[File:foo.jpg|thumb|200px|Bar]]",
                Parsers.FixImages("[[ file : foo.jpg|thumb|200px|Bar]]"));

            Assert.AreEqual("[[Image:foo.jpg|thumb|200px|Bar]]",
                Parsers.FixImages("[[ image : foo.jpg|thumb|200px|Bar]]"));

            //TODO: decide if such improvements really belong here
            //Assert.AreEqual("[[Media:foo]]",
            //    Parsers.FixImages("[[ media : foo]]"));

            //Assert.AreEqual("[[:Media:foo]]",
            //    Parsers.FixImages("[[ : media : foo]]"));

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_6#URL_underscore_regression
            Assert.AreEqual("[[File:foo|thumb]] # [http://a_b c] [[link]]",
                Parsers.FixImages("[[File:foo|thumb]] # [http://a_b c] [[link]]"));

            Assert.AreEqual("[[Image:foo|thumb]] # [http://a_b c] [[link]]",
                Parsers.FixImages("[[Image:foo|thumb]] # [http://a_b c] [[link]]"));

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_2#Removing_underscore_in_URL_in_Ref_in_Description_in_Image....
            //Assert.AreEqual("[[Image:foo_bar|[http://some_link]]]",
            //    Parsers.FixImages("[[image:foo_bar|http://some_link]]"));

            // no changes should be made to this one
            Assert.AreEqual(@"[[Image:Diamminesilver(I)-3D-balls.png|thumb|right|200px|Ball-and-stick model of the diamminesilver(I) cation, [Ag(NH<sub>3</sub>)<sub>2</sub>]<sup>+</sup>]]",
                Parsers.FixImages(@"[[Image:Diamminesilver(I)-3D-balls.png|thumb|right|200px|Ball-and-stick model of the diamminesilver(I) cation, [Ag(NH<sub>3</sub>)<sub>2</sub>]<sup>+</sup>]]"));
        }

        [Test]
        public void Removal()
        {
            bool noChange;

            Assert.AreEqual("", Parsers.RemoveImage("Foo.jpg", "[[Image:Foo.jpg]]", false, "", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("", Parsers.RemoveImage("Foo.jpg", "[[:Image:Foo.jpg]]", false, "", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("", Parsers.RemoveImage("foo.jpg", "[[Image: foo.jpg]]", false, "", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("", Parsers.RemoveImage("Foo, bar", "[[File:foo%2C_bar|quux]]", false, "", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("", Parsers.RemoveImage("Foo%2C_bar", "[[File:foo, bar|quux]]", false, "", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("", Parsers.RemoveImage("foo.jpg", "[[Media:foo.jpg]]", false, "", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("", Parsers.RemoveImage("foo.jpg", "[[:media : foo.jpg]]", false, "", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("{{infobox|image=}}",
                Parsers.RemoveImage("foo", "{{infobox|image=foo}}", false, "", out noChange));
            Assert.IsFalse(noChange);
        }

        [Test]
        public void Replacement()
        {
            bool noChange;

            // just in case...
            Assert.AreEqual("", Parsers.ReplaceImage("", "", "", out noChange));
            Assert.IsTrue(noChange);

            Assert.AreEqual("[[File:bar]]", Parsers.ReplaceImage("foo", "bar", "[[File:Foo]]", out noChange));
            Assert.IsFalse(noChange);

            // preserve namespace
            Assert.AreEqual("[[Image:bar]]", Parsers.ReplaceImage("foo", "bar", "[[image:Foo]]", out noChange));
            Assert.IsFalse(noChange);

            // pipes, non-canonical NS casing
            Assert.AreEqual("[[File:bar|boz!|666px]]",
                Parsers.ReplaceImage("Foo%2C_bar", "bar", "[[FIle:foo, bar|boz!|666px]]", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("[[Media:bar]]", Parsers.ReplaceImage("foo", "bar", "[[Media:foo]]", out noChange));
            Assert.IsFalse(noChange);

            // normalising Media: is not yet supported, see TODO in BasicImprovements()
            Assert.AreEqual("[[:media:bar]]", Parsers.ReplaceImage("foo", "bar", "[[:media : foo]]", out noChange));
            Assert.IsFalse(noChange);
        }
    }

    [TestFixture]
    // tests have to have long strings due to logic in BoldTitle looking at bolding in first 5% of article only
    public class BoldTitleTests : RequiresParser
    {
        bool noChangeBack;

        [Test]
        //http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_1#Title_bolding
        public void DontEmboldenImagesAndTemplates()
        {
            Assert.That(parser.BoldTitle("[[Image:Foo.jpg]]", "Foo", out noChangeBack), Is.Not.Contains("'''Foo'''"));
            Assert.That(parser.BoldTitle("{{Foo}}", "Foo", out noChangeBack), Is.Not.Contains("'''Foo'''"));
            Assert.That(parser.BoldTitle("{{template| Foo is a bar}}", "Foo", out noChangeBack), Is.Not.Contains("'''Foo'''"));
        }

        [Test]
        public void DatesNotChanged()
        {
            Assert.AreEqual(@"May 31 is a great day", parser.BoldTitle(@"May 31 is a great day", "May 31", out noChangeBack));
            Assert.IsTrue(noChangeBack);

            Assert.AreEqual(@"March 1 is a great day", parser.BoldTitle(@"March 1 is a great day", "March 1", out noChangeBack));
            Assert.IsTrue(noChangeBack);

            Assert.AreEqual(@"31 May is a great day", parser.BoldTitle(@"31 May is a great day", "31 May", out noChangeBack));
            Assert.IsTrue(noChangeBack);
        }

        [Test]
        public void SimilarLinksWithDifferentCaseNotChanged()
        {
            Assert.AreEqual("'''Foo''' is this one, now [[FOO]] is another While remaining upright may be the primary goal of beginning riders",
                parser.BoldTitle("Foo is this one, now [[FOO]] is another While remaining upright may be the primary goal of beginning riders", "Foo", out noChangeBack));
            Assert.IsFalse(noChangeBack);

            Assert.AreEqual("'''Foo''' is this one, now [[FOO]] is another While remaining upright may be the primary goal of beginning riders",
                parser.BoldTitle("'''Foo''' is this one, now [[FOO]] is another While remaining upright may be the primary goal of beginning riders", "Foo", out noChangeBack));
            Assert.IsTrue(noChangeBack);
        }

        [Test]
        public void DontChangeIfAlreadyBold()
        {
            Assert.AreEqual("'''Foo''' is this one", parser.BoldTitle("'''Foo''' is this one", "Foo", out noChangeBack));
            Assert.IsTrue(noChangeBack);
            Assert.AreEqual("Foo is a bar, '''Foo''' moar", parser.BoldTitle("Foo is a bar, '''Foo''' moar", "Foo", out noChangeBack));
            Assert.IsTrue(noChangeBack);
            Assert.AreEqual(@"{{Infobox | name = Foo | age=11}} '''Foo''' is a bar", parser.BoldTitle(@"{{Infobox | name = Foo | age=11}} '''Foo''' is a bar", "Foo", out noChangeBack));
            Assert.IsTrue(noChangeBack);
            Assert.AreEqual(@"{{Infobox
| age=11}} '''John David Smith''' is a bar", parser.BoldTitle(@"{{Infobox
| age=11}} '''John David Smith''' is a bar", "John Smith", out noChangeBack));
            Assert.IsTrue(noChangeBack);

            // bold earlier in body of article
            Assert.AreEqual(@"{{Infobox| age=11}} '''John David Smith''' is a bar, John Smith", parser.BoldTitle(@"{{Infobox| age=11}} '''John David Smith''' is a bar, John Smith", "John Smith", out noChangeBack));
            Assert.IsTrue(noChangeBack);
            Assert.AreEqual(@"{{Infobox
| age=11}}{{box2}}} '''John David Smith''' is a bar", parser.BoldTitle(@"{{Infobox
| age=11}}{{box2}}} '''John David Smith''' is a bar", "John Smith", out noChangeBack));
            Assert.IsTrue(noChangeBack);

            // won't change if italics either
            Assert.AreEqual("''Foo'' is this one", parser.BoldTitle("''Foo'' is this one", "Foo", out noChangeBack));
            Assert.IsTrue(noChangeBack);

            Assert.AreEqual(@"{{Infobox_martial_art| website      = }}
{{Nihongo|'''Aikido'''|???|aikido}} is a. Aikido was", parser.BoldTitle(@"{{Infobox_martial_art| website      = }}
{{Nihongo|'''Aikido'''|???|aikido}} is a. Aikido was", "Aikido", out noChangeBack));
            Assert.IsTrue(noChangeBack);
        }

        [Test]
        public void StandardCases()
        {
            Assert.AreEqual("'''Foo''' is a bar While remaining upright may be the primary goal of beginning riders",
                parser.BoldTitle("Foo is a bar While remaining upright may be the primary goal of beginning riders", "Foo", out noChangeBack));
            Assert.IsFalse(noChangeBack);

            Assert.AreEqual("'''Foo in the wild''' is a bar While remaining upright may be the primary goal of beginning riders While remaining upright may be the primary goal of beginning riders",
                parser.BoldTitle("Foo in the wild is a bar While remaining upright may be the primary goal of beginning riders While remaining upright may be the primary goal of beginning riders", "Foo in the wild", out noChangeBack));
            Assert.IsFalse(noChangeBack);

            Assert.AreEqual("'''Foo''' is a bar, Foo moar While remaining upright may be the primary goal of beginning riders",
                parser.BoldTitle("Foo is a bar, Foo moar While remaining upright may be the primary goal of beginning riders", "Foo", out noChangeBack));
            Assert.IsFalse(noChangeBack);

            Assert.AreEqual("'''F^o^o''' is a bar While remaining upright may be the primary goal of beginning riders",
                parser.BoldTitle("F^o^o is a bar While remaining upright may be the primary goal of beginning riders", "F^o^o", out noChangeBack));
            Assert.IsFalse(noChangeBack);

            Assert.AreEqual(@"{{Infobox | name = Foo | age=11}}
'''Foo''' is a bar While remaining upright may be the primary goal of beginning riders While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders",
                parser.BoldTitle(@"{{Infobox | name = Foo | age=11}}
Foo is a bar While remaining upright may be the primary goal of beginning riders While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders", "Foo", out noChangeBack));
            Assert.IsFalse(noChangeBack);

            // brackets excluded from bolding
            Assert.AreEqual("'''Foo''' (Band album) is a CD While remaining upright may be the primary goal of beginning riders",
                parser.BoldTitle("Foo (Band album) is a CD While remaining upright may be the primary goal of beginning riders", "Foo (Band album)", out noChangeBack));
            Assert.IsFalse(noChangeBack);

            // non-changes
            Assert.AreEqual("Fooo is a bar", parser.BoldTitle("Fooo is a bar", "Foo", out noChangeBack));
            Assert.IsTrue(noChangeBack);

            Assert.AreEqual(@"Foo is a '''bar''' While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders",
                parser.BoldTitle(@"Foo is a '''bar''' While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders", "Foo", out noChangeBack)); // bold within first 5% of article
            Assert.IsTrue(noChangeBack);
        }

        [Test]
        public void WithDelinking()
        {
            Assert.AreEqual("'''Foo''' is a bar While remaining upright may be the primary goal of beginning riders",
                parser.BoldTitle("[[Foo]] is a bar While remaining upright may be the primary goal of beginning riders", "Foo", out noChangeBack));
            Assert.IsFalse(noChangeBack);

            Assert.AreEqual("'''Foo''' is a bar, Foo moar While remaining upright may be the primary goal of beginning riders",
                parser.BoldTitle("[[Foo]] is a bar, Foo moar While remaining upright may be the primary goal of beginning riders", "Foo", out noChangeBack));
            Assert.IsFalse(noChangeBack);

            Assert.AreEqual("'''Foo''' is a bar, now Foo here While remaining upright may be the primary goal of beginning riders",
                parser.BoldTitle("Foo is a bar, now [[Foo]] here While remaining upright may be the primary goal of beginning riders", "Foo", out noChangeBack));
            Assert.IsFalse(noChangeBack);

            Assert.AreEqual("'''Foo''' is a bar, now foo here While remaining upright may be the primary goal of beginning riders",
                parser.BoldTitle("Foo is a bar, now [[foo]] here While remaining upright may be the primary goal of beginning riders", "Foo", out noChangeBack));
            Assert.IsFalse(noChangeBack);
            Assert.AreEqual("'''Foo''' is a [[bar]] While remaining upright may be the primary goal of beginning riders",
                parser.BoldTitle("[[Foo]] is a [[bar]] While remaining upright may be the primary goal of beginning riders", "Foo", out noChangeBack));
            Assert.IsFalse(noChangeBack);

            // removal of self links in iteslf are not a 'change'
            Assert.AreEqual("'''Foo''' is a bar, now Foo here While remaining upright may be the primary goal of beginning riders",
                parser.BoldTitle("'''Foo''' is a bar, now [[Foo]] here While remaining upright may be the primary goal of beginning riders", "Foo", out noChangeBack));
            Assert.IsTrue(noChangeBack);
        }

        [Test]
        // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_9#Bold_letters
        public void ExamplesFromBugReport()
        {
            Assert.AreEqual(@"'''Michael Bavaro''' is a [[filmmaker]] based in [[Manhattan]]. While remaining upright may be the primary goal of beginning riders, a bike must lean in order to maintain balance",
                parser.BoldTitle(@"[[Michael Bavaro]] is a [[filmmaker]] based in [[Manhattan]]. While remaining upright may be the primary goal of beginning riders, a bike must lean in order to maintain balance", "Michael Bavaro", out noChangeBack));
            Assert.IsFalse(noChangeBack);

            Assert.AreEqual(@"{{Unreferenced|date=October 2007}}
'''Steve Cook''' is a songwriter for Sovereign Grace. While remaining upright may be the primary goal of beginning riders. While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders While remaining upright may be the primary goal of beginning riders",
                    parser.BoldTitle(@"{{Unreferenced|date=October 2007}}
Steve Cook is a songwriter for Sovereign Grace. While remaining upright may be the primary goal of beginning riders. While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders While remaining upright may be the primary goal of beginning riders", "Steve Cook", out noChangeBack));
            Assert.IsFalse(noChangeBack);

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs#Piped_self-link_delinking__bug
            Assert.AreEqual(@"The '''2009 Indian Premier League''' While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders| 2009<br>", parser.BoldTitle(@"The 2009 Indian Premier League While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders| [[2009 Indian Premier League|2009]]<br>", "2009 Indian Premier League", out noChangeBack));
        }
    }

    [TestFixture]
    public class FixMainArticleTests : RequiresInitialization
    {
        [Test]
        public void BasicBehaviour()
        {
            Assert.AreEqual("{{main|Foo}}", Parsers.FixMainArticle("Main article: [[Foo]]"));
            Assert.AreEqual("{{main|Foo}}", Parsers.FixMainArticle("Main article: [[Foo]]."));
            Assert.AreEqual("Main article:\r\n [[Foo]]", Parsers.FixMainArticle("Main article:\r\n [[Foo]]"));
        }

        [Test]
        // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_3#Fixing_Main_Article_to_.7B.7Bmain.7D.7D
        public void PipedLinks()
        {
            Assert.AreEqual("{{main|Foo|l1=Bar}}", Parsers.FixMainArticle("Main article: [[Foo|Bar]]"));
        }

        [Test]
        public void SupportIndenting()
        {
            Assert.AreEqual("{{main|Foo}}", Parsers.FixMainArticle(":Main article: [[Foo]]"));
            Assert.AreEqual("{{main|Foo}}", Parsers.FixMainArticle(":Main article: [[Foo]]."));
            Assert.AreEqual("{{main|Foo}}", Parsers.FixMainArticle(":''Main article: [[Foo]]''"));
            Assert.AreEqual("'':Main article: [[Foo]]''", Parsers.FixMainArticle("'':Main article: [[Foo]]''"));
        }

        [Test]
        public void SupportBoldAndItalic()
        {
            Assert.AreEqual("{{main|Foo}}", Parsers.FixMainArticle("Main article: '[[Foo]]'"));
            Assert.AreEqual("{{main|Foo}}", Parsers.FixMainArticle("Main article: ''[[Foo]]''"));
            Assert.AreEqual("{{main|Foo}}", Parsers.FixMainArticle("Main article: '''[[Foo]]'''"));
            Assert.AreEqual("{{main|Foo}}", Parsers.FixMainArticle("Main article: '''''[[Foo]]'''''"));

            Assert.AreEqual("{{main|Foo}}", Parsers.FixMainArticle("'Main article: [[Foo]]'"));
            Assert.AreEqual("{{main|Foo}}", Parsers.FixMainArticle("''Main article: [[Foo]]''"));
            Assert.AreEqual("{{main|Foo}}", Parsers.FixMainArticle("'''Main article: [[Foo]]'''"));
            Assert.AreEqual("{{main|Foo}}", Parsers.FixMainArticle("'''''Main article: [[Foo]]'''''"));

            Assert.AreEqual("{{main|Foo}}", Parsers.FixMainArticle("''Main article: '''[[Foo]]'''''"));
            Assert.AreEqual("{{main|Foo}}", Parsers.FixMainArticle("'''Main article: ''[[Foo]]'''''"));
        }

        [Test]
        public void CaseInsensitivity()
        {
            Assert.AreEqual("{{main|foo}}", Parsers.FixMainArticle("main Article: [[foo]]"));
        }

        [Test]
        // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_4#Problem_with_reverse_subst_of_.7B.7Bmain.7D.7D
        public void DontEatTooMuch()
        {
            Assert.AreEqual("Foo is a bar, see main article: [[Foo]]",
                Parsers.FixMainArticle("Foo is a bar, see main article: [[Foo]]"));
            Assert.AreEqual("Main article: [[Foo]], bar", Parsers.FixMainArticle("Main article: [[Foo]], bar"));
        }

        [Test]
        // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_6#Main_and_See_also_templates
        public void SingleLinkOnly()
        {
            Assert.AreEqual(":Main article: [[Foo]] and [[Bar]]", Parsers.FixMainArticle(":Main article: [[Foo]] and [[Bar]]"));
            Assert.AreEqual(":Main article: [[Foo|f00]] and [[Bar]]", Parsers.FixMainArticle(":Main article: [[Foo|f00]] and [[Bar]]"));
        }

        [Test]
        // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_7#Problem_with_.22Main_article.22_fixup
        public void Newlines()
        {
            Assert.AreEqual("test\r\n{{main|Foo}}\r\ntest", Parsers.FixMainArticle("test\r\nMain article: [[Foo]]\r\ntest"));
            Assert.AreEqual("test\r\n\r\n{{main|Foo}}\r\n\r\ntest", Parsers.FixMainArticle("test\r\n\r\nMain article: [[Foo]]\r\n\r\ntest"));
        }

        [Test]
        public void SelfLinkRemoval()
        {
            bool noChangeBack = false;
            Assert.AreEqual(@"'''Foo''' is great. Foo is cool", Parsers.FixLinks(@"'''Foo''' is great. [[Foo]] is cool", "Foo", out noChangeBack));
            Assert.IsFalse(noChangeBack);
            Assert.AreEqual(@"'''Foo''' is great. Bar is cool", Parsers.FixLinks(@"'''Foo''' is great. [[Foo|Bar]] is cool", "Foo", out noChangeBack));
            Assert.IsFalse(noChangeBack);
            Assert.AreEqual(@"'''Foo bar''' is great. Foo bar is cool", Parsers.FixLinks(@"'''Foo bar''' is great. [[Foo bar]] is cool", "Foo bar", out noChangeBack));
            Assert.IsFalse(noChangeBack);
            Assert.AreEqual(@"'''Foo''' is great. Foo is cool.{{cite web|url=a|title=b|publisher=Foo}}", Parsers.FixLinks(@"'''Foo''' is great. Foo is cool.{{cite web|url=a|title=b|publisher=[[Foo]]}}", "Foo", out noChangeBack));
            Assert.IsFalse(noChangeBack);

            // no support for delinking self section links
            Assert.AreEqual(@"'''Foo''' is great. [[Foo#Bar|Bar]] is cool", Parsers.FixLinks(@"'''Foo''' is great. [[Foo#Bar|Bar]] is cool", "Foo", out noChangeBack));
            Assert.IsTrue(noChangeBack);

            Assert.AreEqual(@"'''the''' extreme anti-cult activists resort", Parsers.FixLinks(@"'''the''' extreme [[anti-cult movement|anti-cult activist]]s resort", "Anti-cult movement", out noChangeBack));
            Assert.IsFalse(noChangeBack);
            Assert.AreEqual(@"'''the''' extreme anti-cult activists resort", Parsers.FixLinks(@"'''the''' extreme [[Anti-cult movement|anti-cult activist]]s resort", "Anti-cult movement", out noChangeBack));
            Assert.IsFalse(noChangeBack);
        }
    }

    [TestFixture]
    public class UnicodifyTests : RequiresParser
    {
        [Test]
        public void PreserveTM()
        {
            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_1#AWB_corrupts_the_trademark_.28TM.29_special_character_.
            Assert.AreEqual("test™", parser.Unicodify("test™"));
        }

        [Test]
        public void DontChangeCertainEntities()
        {
            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_3#.26emsp.3B
            Assert.AreEqual("&emsp;&#013;", parser.Unicodify("&emsp;&#013;"));

            Assert.AreEqual("The F&#x2011;22 plane", parser.Unicodify("The F&#x2011;22 plane"));

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_10#SmackBot:_conversion_of_HTML_char-codes_to_raw_Unicode:_issue_.26_consequent_suggestion
            Assert.AreEqual(@"the exclamation mark&#8201;! was", parser.Unicodify(@"the exclamation mark&#8201;! was"));
            Assert.AreEqual(@"the exclamation mark&#8239;! was", parser.Unicodify(@"the exclamation mark&#8239;! was"));

            //http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs#zero-width_space
            Assert.AreEqual(@" hello &#8203; bye", parser.Unicodify(@" hello &#8203; bye"));
        }

        [Test]
        public void IgnoreMath()
        {
            Assert.AreEqual("<math>&laquo;</math>", parser.Unicodify("<math>&laquo;</math>"));
        }
    }

    [TestFixture]
    public class UtilityFunctionTests : RequiresParser
    {

        [Test]
        public void IsCorrectEditSummary()
        {
            // too long
            StringBuilder sb = new StringBuilder(300);
            for (int i = 0; i < 300; i++) sb.Append('x');
            Assert.IsFalse(Parsers.IsCorrectEditSummary(sb.ToString()));

            // no wikilinks
            Assert.IsTrue(Parsers.IsCorrectEditSummary(""));
            Assert.IsTrue(Parsers.IsCorrectEditSummary("test"));
            Assert.IsTrue(Parsers.IsCorrectEditSummary("["));
            Assert.IsTrue(Parsers.IsCorrectEditSummary("]"));
            Assert.IsTrue(Parsers.IsCorrectEditSummary("[test]"));
            Assert.IsTrue(Parsers.IsCorrectEditSummary("[test]]"));
            Assert.IsTrue(Parsers.IsCorrectEditSummary("[[]]"));

            // correctly (sort of..) terminated wikilinks
            Assert.IsTrue(Parsers.IsCorrectEditSummary("[[test]]"));
            Assert.IsTrue(Parsers.IsCorrectEditSummary("[[test]] [[foo]]"));
            Assert.IsTrue(Parsers.IsCorrectEditSummary("[[foo[[]]]"));

            //broken wikilinks, should be found to be invalid
            Assert.IsFalse(Parsers.IsCorrectEditSummary("[["));
            Assert.IsFalse(Parsers.IsCorrectEditSummary("[[["));
            Assert.IsFalse(Parsers.IsCorrectEditSummary("[[test]"));
            Assert.IsFalse(Parsers.IsCorrectEditSummary("[[test]] [["));
        }

        [Test]
        public void ChangeToDefaultSort()
        {
            bool noChange;

            // don't change sorting for single categories
            Assert.AreEqual("[[Category:Test1|Foooo]]",
                            Parsers.ChangeToDefaultSort("[[Category:Test1|Foooo]]", "Foo", out noChange));
            Assert.IsTrue(noChange);

            // should work
            Assert.AreEqual("[[Category:Test1]][[Category:Test2]]\r\n{{DEFAULTSORT:Foooo}}",
                            Parsers.ChangeToDefaultSort("[[Category:Test1|Foooo]][[Category:Test2|Foooo]]", "Bar",
                                                  out noChange));
            Assert.IsFalse(noChange);

            // ...but don't add DEFAULTSORT if the key equals page title
            Assert.AreEqual("[[Category:Test1]][[Category:Test2]]",
                            Parsers.ChangeToDefaultSort("[[Category:Test1|Foooo]][[Category:Test2|Foooo]]", "Foooo",
                                                  out noChange));
            Assert.IsFalse(noChange, "Should detect a change even if it hasn't added a DEFAULTSORT");

            // don't change if key is 3 chars or less
            Assert.AreEqual("[[Category:Test1|Foo]][[Category:Test2|Foo]]",
                            Parsers.ChangeToDefaultSort("[[Category:Test1|Foo]][[Category:Test2|Foo]]", "Bar", out noChange));
            Assert.IsTrue(noChange);

            // Remove explicit keys equal to page title
            Assert.AreEqual("[[Category:Test1]][[Category:Test2]]",
                            Parsers.ChangeToDefaultSort("[[Category:Test1|Foooo]][[Category:Test2]]", "Foooo", out noChange));
            Assert.IsFalse(noChange);
            // swap
            Assert.AreEqual("[[Category:Test1]][[Category:Test2]]",
                            Parsers.ChangeToDefaultSort("[[Category:Test1]][[Category:Test2|Foooo]]", "Foooo", out noChange));
            Assert.IsFalse(noChange);

            // Borderline condition
            Assert.AreEqual("[[Category:Test1|Fooooo]][[Category:Test2]]",
                            Parsers.ChangeToDefaultSort("[[Category:Test1|Fooooo]][[Category:Test2]]", "Foooo", out noChange));
            Assert.IsTrue(noChange);

            // Don't change anything if there's ambiguity
            Assert.AreEqual("[[Category:Test1|Foooo]][[Category:Test2|Baaar]]",
                            Parsers.ChangeToDefaultSort("[[Category:Test1|Foooo]][[Category:Test2|Baaar]]", "Teeest",
                                                  out noChange));
            Assert.IsTrue(noChange);
            // same thing
            Assert.AreEqual("[[Category:Test1|Foooo]][[Category:Test2|Baaar]]",
                            Parsers.ChangeToDefaultSort("[[Category:Test1|Foooo]][[Category:Test2|Baaar]]", "Foooo",
                                                  out noChange));
            Assert.IsTrue(noChange);

            // remove diacritics when generating a key
            Assert.AreEqual("[[Category:Test1]][[Category:Test2]]\r\n{{DEFAULTSORT:Foooo}}",
                            Parsers.ChangeToDefaultSort("[[Category:Test1|Foooô]][[Category:Test2|Foooô]]", "Bar",
                                                  out noChange));
            Assert.IsFalse(noChange);

            // should also fix diacritics in existing defaultsort's and remove leading spaces
            // also support mimicking templates
            Assert.AreEqual("{{DEFAULTSORT:Test}}",
                            Parsers.ChangeToDefaultSort("{{defaultsort| Tést}}", "Foo", out noChange));
            Assert.IsFalse(noChange);

            // shouldn't change whitespace-only sortkeys
            Assert.AreEqual("{{DEFAULTSORT: \t}}", Parsers.ChangeToDefaultSort("{{DEFAULTSORT: \t}}", "Foo", out noChange));
            Assert.IsTrue(noChange);

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_6#DEFAULTSORT_with_spaces
            // DEFAULTSORT doesn't treat leading spaces the same way as categories do
            Assert.AreEqual("[[Category:Test1| Foooo]][[Category:Test2| Foooo]]",
                            Parsers.ChangeToDefaultSort("[[Category:Test1| Foooo]][[Category:Test2| Foooo]]", "Bar",
                                                  out noChange));
            Assert.IsTrue(noChange);

            // {{Lifetime}} is not fully supported
            Parsers.ChangeToDefaultSort("{{lifetime|blah}}[[Category:Test1|Foooo]][[Category:Test2|Foooo]]",
                                  "Bar", out noChange);
            Assert.IsTrue(noChange);

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_7#AWB_needs_to_handle_lifetime_template_correctly
            // pages with multiple sort specifiers shouldn't be changed
            Parsers.ChangeToDefaultSort("{{DEFAULTSORT:Foo}}{{lifetime|Bar}}",
                                  "Foo", out noChange);
            Assert.IsTrue(noChange);
            // continued...
            Parsers.ChangeToDefaultSort("{{defaultsort| Tést}}{{DEFAULTSORT: Tést}}", "Foo", out noChange);
            Assert.IsTrue(noChange);

            //Remove explicitally defined sort keys from categories when the page has defaultsort
            Assert.AreEqual("{{DEFAULTSORT:Test}}[[Category:Test]]",
                Parsers.ChangeToDefaultSort("{{DEFAULTSORT:Test}}[[Category:Test|Test]]", "Foo", out noChange));
            Assert.IsFalse(noChange);

            //Case difference of above
            Assert.AreEqual("{{DEFAULTSORT:Test}}[[Category:Test]]",
                Parsers.ChangeToDefaultSort("{{DEFAULTSORT:Test}}[[Category:Test|TEST]]", "Foo", out noChange));
            Assert.IsFalse(noChange);

            //No change due to different key
            Assert.AreEqual("{{DEFAULTSORT:Test}}[[Category:Test|Not a Test]]",
                Parsers.ChangeToDefaultSort("{{DEFAULTSORT:Test}}[[Category:Test|Not a Test]]", "Foo", out noChange));
            Assert.IsTrue(noChange);

            //Multiple to be removed
            Assert.AreEqual("{{DEFAULTSORT:Test}}[[Category:Test]][[Category:Foo]][[Category:Bar]]",
                Parsers.ChangeToDefaultSort("{{DEFAULTSORT:Test}}[[Category:Test|TEST]][[Category:Foo|Test]][[Category:Bar|test]]", "Foo", out noChange));
            Assert.IsFalse(noChange);

            //Multiple with 1 no key
            Assert.AreEqual("{{DEFAULTSORT:Test}}[[Category:Test]][[Category:Foo]][[Category:Bar]]",
                Parsers.ChangeToDefaultSort("{{DEFAULTSORT:Test}}[[Category:Test|TEST]][[Category:Foo]][[Category:Bar|test]]", "Foo", out noChange));
            Assert.IsFalse(noChange);

            //Multiple with 1 different key
            Assert.AreEqual("{{DEFAULTSORT:Test}}[[Category:Test]][[Category:Foo|Bar]][[Category:Bar]]",
                Parsers.ChangeToDefaultSort("{{DEFAULTSORT:Test}}[[Category:Test|TEST]][[Category:Foo|Bar]][[Category:Bar|test]]", "Foo", out noChange));
            Assert.IsFalse(noChange);

            // just removing diacritics in categories is useful
            Assert.AreEqual(@"[[Category:Bronze Wolf awardees|Laine, Juan]]",
                Parsers.ChangeToDefaultSort(@"[[Category:Bronze Wolf awardees|Lainé, Juan]]", "Hi", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual(@"[[Category:Bronze Wolf awardees|Laine, Juan]]",
                Parsers.ChangeToDefaultSort(@"[[Category:Bronze Wolf awardees|Laine, Juan]]", "Hi", out noChange));
            Assert.IsTrue(noChange);
        }

        [Test]
        public void TestDefaultsortTitlesWithDiacritics()
        {
            bool noChange;

            Assert.AreEqual(@"[[Category:Parishes in Asturias]]
{{DEFAULTSORT:Abandames}}",
                Parsers.ChangeToDefaultSort(@"[[Category:Parishes in Asturias]]", "Abándames", out noChange));
            Assert.IsFalse(noChange);

            // no change if a defaultsort already there
            Assert.AreEqual(@"[[Category:Parishes in Asturias]]
{{DEFAULTSORT:Bert}}",
    Parsers.ChangeToDefaultSort(@"[[Category:Parishes in Asturias]]
{{DEFAULTSORT:Bert}}", "Abándames", out noChange));
            Assert.IsTrue(noChange);


            // lifetime provides a defaultsort, no change
            Assert.AreEqual(@"[[Category:Parishes in Asturias]]
{{Lifetime|1833|1907|Bisson, Elie-Hercule}}",
Parsers.ChangeToDefaultSort(@"[[Category:Parishes in Asturias]]
{{Lifetime|1833|1907|Bisson, Elie-Hercule}}", "Abándames", out noChange));
            Assert.IsTrue(noChange);

            // category sortkeys are cleaned too
            Assert.AreEqual(@"[[Category:Parishes of the Azores]]
[[Category:São Miguel Island]]
{{DEFAULTSORT:Agua Retorta}}", Parsers.ChangeToDefaultSort(@"[[Category:Parishes of the Azores|Agua Retorta]]
[[Category:São Miguel Island]]", @"Água Retorta", out noChange));
            Assert.IsFalse(noChange);

            // lifetime provides a defaultsort, no change
            Assert.AreEqual(@"[[Category:Parishes of the Azores]]
[[Category:São Miguel Island]]
{{Lifetime|1833|1907|Bisson, Elie-Hercule}}", Parsers.ChangeToDefaultSort(@"[[Category:Parishes of the Azores]]
[[Category:São Miguel Island]]
{{Lifetime|1833|1907|Bisson, Elie-Hercule}}", @"Água Retorta", out noChange));
            Assert.IsTrue(noChange);

            // else if no lifetime, use article name
            Assert.AreEqual(@"[[Category:Parishes of the Azores]]
[[Category:São Miguel Island]]
{{DEFAULTSORT:Agua Retorta}}", Parsers.ChangeToDefaultSort(@"[[Category:Parishes of the Azores]]
[[Category:São Miguel Island]]", @"Água Retorta", out noChange));
            Assert.IsFalse(noChange);
        }

        [Test, Ignore("Unused"), Category("Incomplete")]
        public void ExternalURLToInternalLink()
        {
            //TODO:MOAR
            Assert.AreEqual("", Parsers.ExternalURLToInternalLink(""));
            Assert.AreEqual("%20", Parsers.ExternalURLToInternalLink("%20"));

            // ExtToInt1
            Assert.AreEqual("https://secure.wikimedia.org/otrs/index.pl?Action=AgentTicketQueue",
                Parsers.ExternalURLToInternalLink("https://secure.wikimedia.org/otrs/index.pl?Action=AgentTicketQueue"));

            // ExtToInt2
            Assert.AreEqual("[[:ru:????|Foo]]", Parsers.ExternalURLToInternalLink("[http://ru.wikipedia.org/wiki/???? Foo]"));
        }

        [Test]
        public void RemoveEmptyComments()
        {
            Assert.AreEqual("", Parsers.RemoveEmptyComments("<!---->"));
            Assert.AreEqual("", Parsers.RemoveEmptyComments("<!-- -->"));

            // newline comments are used to split wikitext to lines w/o breaking formatting,
            // they should not be removed
            Assert.AreEqual("<!--\r\n\r\n-->", Parsers.RemoveEmptyComments("<!--\r\n\r\n-->"));

            Assert.AreEqual("", Parsers.RemoveEmptyComments("<!----><!---->"));
            Assert.AreEqual("<!--\r\n\r\n-->", Parsers.RemoveEmptyComments("<!--\r\n\r\n--><!---->"));
            Assert.AreEqual("<!--Test-->", Parsers.RemoveEmptyComments("<!----><!--Test-->"));
            Assert.AreEqual(" <!--Test-->", Parsers.RemoveEmptyComments("<!----> <!--Test-->"));
            Assert.AreEqual("<!--Test\r\nfoo--> <!--Test-->", Parsers.RemoveEmptyComments("<!--Test\r\nfoo--> <!--Test-->"));

            Assert.AreEqual("<!--Test-->", Parsers.RemoveEmptyComments("<!--Test-->"));

            Assert.AreEqual("", Parsers.RemoveEmptyComments(""));
            Assert.AreEqual("test", Parsers.RemoveEmptyComments("test"));
        }

        [Test]
        public void HasSicTagTests()
        {
            Assert.IsTrue(Parsers.HasSicTag("now helo [sic] there"));
            Assert.IsTrue(Parsers.HasSicTag("now helo [sic!] there"));
            Assert.IsTrue(Parsers.HasSicTag("now helo[sic] there"));
            Assert.IsTrue(Parsers.HasSicTag("now helo (sic) there"));
            Assert.IsTrue(Parsers.HasSicTag("now helo (sic!) there"));
            Assert.IsTrue(Parsers.HasSicTag("now helo {sic} there"));
            Assert.IsTrue(Parsers.HasSicTag("now helo [Sic] there"));
            Assert.IsTrue(Parsers.HasSicTag("now helo [ Sic ] there"));
            Assert.IsTrue(Parsers.HasSicTag("now {{sic|helo}} there"));
            Assert.IsTrue(Parsers.HasSicTag("now {{sic|hel|o}} there"));
            Assert.IsTrue(Parsers.HasSicTag("now {{typo|helo}} there"));
            Assert.IsTrue(Parsers.HasSicTag("now helo <!--[sic]-->there"));

            Assert.IsFalse(Parsers.HasSicTag("now sickened by"));
            Assert.IsFalse(Parsers.HasSicTag("sic transit gloria mundi"));
            Assert.IsFalse(Parsers.HasSicTag("The Sound Information Company (SIC) is"));
        }

        [Test]
        public void HasMorefootnotesAndManyReferencesTests()
        {
            Assert.IsTrue(Parsers.HasMorefootnotesAndManyReferences(@"Article<ref>A</ref> <ref>B</ref> <ref>C</ref> <ref>B</ref> <ref>E</ref> 
==References== 
{{reflist}} 
{{nofootnotes}}"));

            Assert.IsTrue(Parsers.HasMorefootnotesAndManyReferences(@"Article<ref>A</ref> <ref>B</ref> <ref>C</ref> <ref>B</ref> <ref>E</ref> 
==References== 
{{reflist}} 
{{morefootnotes}}"));

            Assert.IsTrue(Parsers.HasMorefootnotesAndManyReferences(@"Article<ref name=A>A</ref> <ref name=B>B</ref> <ref>C</ref> <ref>B</ref> <ref>E</ref> 
==References== 
{{reflist}} 
{{Morefootnotes}}"));

            // not enough references
            Assert.IsFalse(Parsers.HasMorefootnotesAndManyReferences(@"Article<ref>A</ref> <ref>B</ref> <ref>C</ref> <ref>B</ref> 
==References== 
{{reflist}} 
{{nofootnotes}}"));

            // no {{nofootnotes}}
            Assert.IsFalse(Parsers.HasMorefootnotesAndManyReferences(@"Article<ref name=A>A</ref> <ref name=B>B</ref> <ref>C</ref> <ref>B</ref> <ref>E</ref> 
==References== 
{{reflist}}"));
        }

        [Test]
        public void HasRefAfterReflistTest()
        {
            Assert.IsTrue(Parsers.HasRefAfterReflist(@"blah <ref>a</ref> ==references== {{reflist}} <ref>b</ref>"));
            Assert.IsTrue(Parsers.HasRefAfterReflist(@"blah <ref>a</ref> ==references== {{reflist}} {{GR|4}}"));
            Assert.IsTrue(Parsers.HasRefAfterReflist(@"blah <ref>a</ref> 
==references== {{reflist}} <ref>b</ref>"));
            Assert.IsTrue(Parsers.HasRefAfterReflist(@"blah <ref>a</ref> ==references== {{reflist}} <ref name=""b"">b</ref>"));

            // this is correct syntax
            Assert.IsFalse(Parsers.HasRefAfterReflist(@"blah <ref>a</ref> ==references== {{reflist}}"));
            // ignores commented out refs
            Assert.IsFalse(Parsers.HasRefAfterReflist(@"blah <ref>a</ref> ==references== {{reflist}} <!--<ref>b</ref>-->"));

            // the second template means this is okay too
            Assert.IsFalse(Parsers.HasRefAfterReflist(@"blah <ref>a</ref> ==references== {{reflist}} <ref name=""b"">b</ref> {{reflist}}"));

            // 'r' in argument means no embedded <ref></ref>
            Assert.IsFalse(Parsers.HasRefAfterReflist(@"blah <ref>a</ref> ==references== {{reflist}} {{GR|r4}}"));
            Assert.IsFalse(Parsers.HasRefAfterReflist(@"blah <ref>a</ref> ==references== {{reflist}} {{GR|India}}"));
        }

        [Test]
        public void HasInUseTagTests()
        {
            Assert.IsTrue(Parsers.IsInUse("{{inuse}} Hello world"));
            Assert.IsTrue(Parsers.IsInUse("{{ inuse  }} Hello world"));
            Assert.IsTrue(Parsers.IsInUse("{{Inuse}} Hello world"));
            Assert.IsTrue(Parsers.IsInUse("Hello {{inuse}} Hello world"));
            Assert.IsTrue(Parsers.IsInUse("{{inuse|5 minutes}} Hello world"));

            // ignore commented inuse
            Assert.IsFalse(Parsers.IsInUse("<!--{{inuse}}--> Hello world"));
            Assert.IsTrue(Parsers.IsInUse("<!--{{inuse}}--> {{inuse|5 minutes}} Hello world"));

            Assert.IsFalse(Parsers.IsInUse("{{INUSE}} Hello world")); // no such template
        }

        [Test]
        public void IsMissingReferencesDisplayTests()
        {
            Assert.IsTrue(Parsers.IsMissingReferencesDisplay(@"Hello<ref>Fred</ref>"));
            Assert.IsTrue(Parsers.IsMissingReferencesDisplay(@"Hello<ref name=""F"">Fred</ref>"));

            // {{GR}} provides an embedded <ref></ref> if its argument is a decimal
            Assert.IsTrue(Parsers.IsMissingReferencesDisplay(@"Hello{{GR|4}}"));

            Assert.IsFalse(Parsers.IsMissingReferencesDisplay(@"Hello"));
            Assert.IsFalse(Parsers.IsMissingReferencesDisplay(@"Hello<ref>Fred</ref> {{reflist}}"));
            Assert.IsFalse(Parsers.IsMissingReferencesDisplay(@"Hello<ref>Fred</ref> {{Reflist}}"));
            Assert.IsFalse(Parsers.IsMissingReferencesDisplay(@"Hello<ref>Fred</ref> {{ref-list}}"));
            Assert.IsFalse(Parsers.IsMissingReferencesDisplay(@"Hello<ref>Fred</ref> {{reflink}}"));
            Assert.IsFalse(Parsers.IsMissingReferencesDisplay(@"Hello<ref>Fred</ref> {{references}}"));
            Assert.IsFalse(Parsers.IsMissingReferencesDisplay(@"Hello<ref>Fred</ref> <references/>"));
            Assert.IsFalse(Parsers.IsMissingReferencesDisplay(@"Hello{{GR|4}} <references/>"));

            // this specifies to {{GR}} not to embed <ref></ref>
            Assert.IsFalse(Parsers.IsMissingReferencesDisplay(@"Hello{{GR|r4}}"));
            Assert.IsFalse(Parsers.IsMissingReferencesDisplay(@"Hello{{GR|India}}"));
        }

        [Test]
        public void InfoboxTests()
        {
            Assert.IsTrue(Parsers.HasInfobox(@"{{Infobox fish | name = Bert }} ''Bert'' is a good fish."));
            Assert.IsTrue(Parsers.HasInfobox(@"{{Infobox 
fish | name = Bert }} ''Bert'' is a good fish."));
            Assert.IsTrue(Parsers.HasInfobox(@"{{infobox fish | name = Bert }} ''Bert'' is a good fish."));
            Assert.IsTrue(Parsers.HasInfobox(@"{{ infobox fish | name = Bert }} ''Bert'' is a good fish."));
            Assert.IsTrue(Parsers.HasInfobox(@"{{ infobox fish | name = Bert }} ''Bert'' is a good <!--fish-->."));

            Assert.IsFalse(Parsers.HasInfobox(@"{{INFOBOX fish | name = Bert }} ''Bert'' is a good fish."));
            Assert.IsFalse(Parsers.HasInfobox(@"{{infoboxfish | name = Bert }} ''Bert'' is a good fish."));
            Assert.IsFalse(Parsers.HasInfobox(@"<!--{{infobox fish | name = Bert }}--> ''Bert'' is a good fish."));
            Assert.IsFalse(Parsers.HasInfobox(@"<nowiki>{{infobox fish | name = Bert }}</nowiki> ''Bert'' is a good fish."));
        }

        [Test]
        public void NoBotsTests()
        {
            Assert.IsTrue(Parsers.CheckNoBots("", ""));
            Assert.IsTrue(Parsers.CheckNoBots("{{test}}", ""));
            Assert.IsTrue(Parsers.CheckNoBots("lol, test", ""));

            Assert.IsTrue(Parsers.CheckNoBots("{{bots}}", ""));
            Assert.IsTrue(Parsers.CheckNoBots("{{bots|allow=awb}}", ""));
            Assert.IsTrue(Parsers.CheckNoBots("{{bots|allow=test}}", "test"));
            Assert.IsTrue(Parsers.CheckNoBots("{{bots|allow=user,test}}", "test"));
            Assert.IsTrue(Parsers.CheckNoBots("{{bots|deny=none}}", ""));

            Assert.IsFalse(Parsers.CheckNoBots("{{nobots}}", ""));
            Assert.IsFalse(Parsers.CheckNoBots("{{bots|deny=all}}", ""));
            Assert.IsFalse(Parsers.CheckNoBots("{{bots|deny=awb}}", ""));
            Assert.IsFalse(Parsers.CheckNoBots("{{bots|deny=awb,test}}", ""));
            Assert.IsFalse(Parsers.CheckNoBots("{{bots|deny=awb,test}}", "test"));
            Assert.IsFalse(Parsers.CheckNoBots("{{bots|deny=test}}", "test"));
            Assert.IsFalse(Parsers.CheckNoBots("{{bots|allow=none}}", ""));
        }
    }

    [TestFixture]
    public class RecategorizerTests : RequiresParser
    {
        [Test]
        public void Addition()
        {
            bool noChange;

            Assert.AreEqual("\r\n\r\n[[Category:Foo]]\r\n", parser.AddCategory("Foo", "", "bar", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("bar\r\n\r\n[[Category:Foo]]\r\n", parser.AddCategory("Foo", "bar", "bar", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("test\r\n\r\n[[Category:Foo|bar]]\r\n[[Category:Bar]]\r\n",
                parser.AddCategory("Bar", "test[[Category:Foo|bar]]", "foo", out noChange));
            Assert.IsFalse(noChange);

            // shouldn't add if category already exists
            Assert.AreEqual("[[Category:Foo]]", parser.AddCategory("Foo", "[[Category:Foo]]", "bar", out noChange));
            Assert.IsTrue(noChange);
            Assert.That(parser.AddCategory("Foo bar", "[[Category:Foo_bar]]", "bar", out noChange),
                Text.Matches(@"\[\[Category:Foo[ _]bar\]\]"));
            Assert.IsTrue(noChange);

            Assert.AreEqual("[[category : foo_bar%20|quux]]", parser.AddCategory("Foo bar", "[[category : foo_bar%20|quux]]", "bar", out noChange));
            Assert.IsTrue(noChange);

            Assert.AreEqual("test<noinclude>\r\n[[Category:Foo]]\r\n</noinclude>",
                parser.AddCategory("Foo", "test", "Template:foo", out noChange));
            Assert.IsFalse(noChange);
        }

        [Test]
        public void Replacement()
        {
            bool noChange;

            Assert.AreEqual("[[Category:Bar]]", Parsers.ReCategoriser("Foo", "Bar", "[[Category:Foo]]", out noChange));
            Assert.IsFalse(noChange);

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_7#Replacing_Arabic_categories
            // addresses special case in Tools.CaseInsensitive
            Assert.AreEqual("[[Category:Bar]]", Parsers.ReCategoriser("-Foo bar-", "Bar", "[[Category:-Foo bar-]]", out noChange));
            Assert.IsFalse(noChange);
            Assert.AreEqual("[[Category:-Bar II-]]", Parsers.ReCategoriser("Foo", "-Bar II-", "[[Category:Foo]]", out noChange));
            Assert.IsFalse(noChange);


            Assert.AreEqual("[[Category:Bar]]", Parsers.ReCategoriser("Foo", "Bar", "[[ catEgory: Foo]]", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("[[Category:Bar]]", Parsers.ReCategoriser("Foo", "Bar", "[[Category:foo]]", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("[[Category:Bar|boz]]", Parsers.ReCategoriser("Foo", "Bar", "[[Category:Foo|boz]]", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("[[Category:Bar| boz]]", Parsers.ReCategoriser("foo? Bar!", "Bar", "[[ category:Foo?_Bar! | boz]]", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual(@"[[Category:Boz]]
[[Category:Bar]]
[[Category:Quux]]", Parsers.ReCategoriser("Foo", "Bar", @"[[Category:Boz]]
[[Category:foo]]
[[Category:Quux]]", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("test[[Category:Bar]]test", Parsers.ReCategoriser("Foo", "Bar", "test[[Category:Foo]]test", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("[[Category:Fooo]]", Parsers.ReCategoriser("Foo", "Bar", "[[Category:Fooo]]", out noChange));
            Assert.IsTrue(noChange);
        }

        [Test]
        public void Removal()
        {
            bool noChange;

            Assert.AreEqual("", Parsers.RemoveCategory("Foo", "[[Category:Foo]]", out noChange));
            Assert.IsFalse(noChange);

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_7#Replacing_Arabic_categories
            // addresses special case in Tools.CaseInsensitive
            Assert.AreEqual("", Parsers.RemoveCategory("-Foo bar-", "[[Category:-Foo bar-]]", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("", Parsers.RemoveCategory("Foo", "[[ category: foo | bar]]", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("", Parsers.RemoveCategory("Foo", "[[Category:Foo|]]", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("  ", Parsers.RemoveCategory("Foo", " [[Category:Foo]] ", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("", Parsers.RemoveCategory("Foo", "[[Category:Foo]]\r\n", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("\r\n", Parsers.RemoveCategory("Foo", "[[Category:Foo]]\r\n\r\n", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("", Parsers.RemoveCategory("Foo? Bar!", "[[Category:Foo?_Bar!|boz]]", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("[[Category:Fooo]]", Parsers.RemoveCategory("Foo", "[[Category:Fooo]]", out noChange));
            Assert.IsTrue(noChange);
        }
    }

    [TestFixture]
    public class TaggerTests : RequiresParser
    {
        private bool noChange;
        private string summary;

        private const string uncat = "{{Uncategorized|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}",
                       uncatstub = "{{Uncategorizedstub|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}",
                       orphan = "{{orphan|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}",
                       wikify = "{{Wikify|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}",
                       deadend = "{{deadend|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}",
                       stub = "{{stub}}";

        [Test]
        public void Add()
        {
            Globals.UnitTestIntValue = 0;
            Globals.UnitTestBoolValue = true;

            string text = parser.Tagger(shortText, "Test", out noChange, ref summary, true, false);
            //Stub, no existing stub tag. Needs all tags
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text));
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(text));
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(text));
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text));

            Assert.IsFalse(text.Contains(uncatstub));

            text = parser.Tagger(shortText + stub + uncat + wikify + orphan + deadend, "Test", out noChange, ref summary, true, false);
            //Tagged article, dupe tags shouldn't be added
            Assert.AreEqual(1, Tools.RegexMatchCount(Regex.Escape(stub), text));
            Assert.AreEqual(1, Tools.RegexMatchCount(Regex.Escape(uncat), text));
            Assert.AreEqual(1, Tools.RegexMatchCount(Regex.Escape(wikify), text));
            Assert.AreEqual(1, Tools.RegexMatchCount(Regex.Escape(orphan), text));
            Assert.AreEqual(1, Tools.RegexMatchCount(Regex.Escape(deadend), text));

            text = parser.Tagger(shortText + stub, "Test", out noChange, ref summary, true, false);
            //Stub, existing stub tag
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text));
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(text));
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text));
            Assert.IsTrue(text.Contains(uncatstub));
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text));

            Assert.IsFalse(text.Contains(uncat));

            Assert.AreEqual(1, Tools.RegexMatchCount(Regex.Escape(stub), text));

            text = parser.Tagger(shortText + shortText, "Test", out noChange, ref summary, true, false);
            //Not a stub
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text));
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(text));
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(text));

            Assert.IsFalse(text.Contains(uncatstub));
            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text));

            // with categories and no links, deadend not removed
            Globals.UnitTestIntValue = 3;
            text = parser.Tagger(deadend + shortText, "Test", out noChange, ref summary, true, false);
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text));
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(text));
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text));
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text));

            Globals.UnitTestIntValue = 5;

            text = parser.Tagger(shortText, "Test", out noChange, ref summary, true, false);
            //Categorised Stub
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text));
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(text));
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text));
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text));

            Assert.IsFalse(text.Contains(uncatstub));
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text));

            text = parser.Tagger(shortText + shortText, "Test", out noChange, ref summary, true, false);
            //Categorised Page
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text));
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(text));
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text));

            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text));
            Assert.IsFalse(text.Contains(uncatstub));
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text));

            Globals.UnitTestBoolValue = false;

            text = parser.Tagger(shortText, "Test", out noChange, ref summary, true, false);
            //Non orphan categorised stub
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(text));
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text));
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text));

            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));
            Assert.IsFalse(text.Contains(uncatstub));
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text));

            text = parser.Tagger(shortText + shortText, "Test", out noChange, ref summary, true, false);
            //Non orphan categorised page
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(text));
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text));

            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text));
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));
            Assert.IsFalse(text.Contains(uncatstub));
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text));

            text = parser.Tagger(shortText.Replace("consectetur", "[[consectetur]]"), "Test", out noChange, ref summary, true, false);
            //Non deadend stub
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text));

            Assert.IsFalse(WikiRegexes.Wikify.IsMatch(text));
            Assert.IsFalse(WikiRegexes.DeadEnd.IsMatch(text));
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));
            Assert.IsFalse(text.Contains(uncatstub));
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text));

            text = parser.Tagger(Regex.Replace(shortText, @"(\w+)", "[[$1]]"), "Test", out noChange, ref summary, true, false);
            //very wikified stub
            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text));
            Assert.IsFalse(WikiRegexes.Wikify.IsMatch(text));
            Assert.IsFalse(WikiRegexes.DeadEnd.IsMatch(text));
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));
            Assert.IsFalse(text.Contains(uncatstub));
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text));

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_10#Autotagging_and_Articleissues
            // do not add orphan where article already has orphan tag within {{Article issues}}

            text = parser.Tagger(@"{{Article issues|orphan=May 2008|cleanup=May 2008|story=May 2008}}\r\n" + shortText, "Test", out noChange, ref summary, true, false);
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));
            Assert.IsTrue(WikiRegexes.OrphanArticleIssues.IsMatch(text));
        }

        private const string shortText =
            @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Curabitur sit amet tortor nec neque faucibus pharetra. Fusce lorem arcu, tempus et, imperdiet a, commodo a, pede. Nulla sit amet turpis gravida elit dictum cursus. Praesent tincidunt velit eu urna.";

        private const string longText =
            @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse dictum ultrices augue. Fusce sem diam, vestibulum sit amet, vehicula id, congue a, nisl. Phasellus pulvinar posuere purus. Donec elementum justo mattis nulla. Sed a purus dictum lacus pharetra adipiscing. Nam non dui non ante viverra iaculis. Fusce euismod lacus id nulla vulputate gravida. Suspendisse lectus pede, tempus sed, tristique id, pharetra eget, urna. Integer mattis libero vel quam accumsan suscipit. Vivamus molestie dapibus est. Quisque quis metus eget nisl accumsan aliquet. Donec tempus pellentesque tellus. Aliquam lacinia gravida justo. Aliquam erat volutpat. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Mauris ultricies suscipit urna. Ut mollis tempor leo. Pellentesque fringilla mattis enim. Proin sapien enim, congue non, aliquet et, sollicitudin nec, mauris. Sed porta. 

Curabitur luctus mollis massa. Nullam consectetur mollis lacus. Suspendisse turpis. Fusce velit. Morbi egestas dui. Donec commodo ornare lorem. Vestibulum sodales. Curabitur egestas libero ut metus. Sed eget orci a ligula consectetur vestibulum. Cras sapien. 

Sed libero. Ut volutpat massa. Donec nulla pede, porttitor eu, sodales et, consectetur nec, quam. Pellentesque vestibulum hendrerit est. Nulla facilisi. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Duis et nibh eu lacus iaculis pretium. Fusce sed turpis. In cursus. Etiam interdum augue. Morbi commodo auctor ligula. In imperdiet, neque nec hendrerit consequat, lacus purus tristique turpis, eu hendrerit ipsum ligula at libero. Duis varius nunc vel tortor. Praesent tempor. Nunc non pede at velit congue feugiat. Curabitur gravida, nisl quis mattis porttitor, purus nulla viverra dui, non suscipit augue nunc ac libero. Donec lacinia est non augue. 

Nulla quam dui, tristique id, condimentum sed, sodales in, ante. Vestibulum vitae diam. Integer placerat ante non orci. Nulla gravida. Integer magna enim, iaculis ut, ornare dignissim, ultrices a, urna. Donec urna. Fusce fringilla, pede vitae pulvinar ullamcorper, est nisi eleifend ipsum, ac adipiscing odio massa vehicula neque. Sed blandit est. Morbi faucibus, nisl vel commodo vulputate, mi ipsum tincidunt sem, id ornare orci orci et velit. Morbi commodo sollicitudin ligula. Pellentesque vitae urna. Duis massa arcu, accumsan id, euismod eu, tincidunt et, odio. Phasellus purus leo, rhoncus sed, condimentum nec, vestibulum vel, lacus. In egestas, lectus vitae lacinia tristique, elit magna consequat risus, id sodales metus nulla ac pede. Suspendisse potenti. 

Fusce massa. Nullam lacinia purus nec ipsum. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Suspendisse potenti. Proin augue. Donec mi magna, interdum a, elementum quis, bibendum sit amet, felis. Donec vel libero eget magna hendrerit ultrices. Suspendisse potenti. Sed scelerisque lacinia nisi. Quisque elementum, nunc nec luctus iaculis, ante quam aliquet orci, et ullamcorper dui ipsum at mi. Vestibulum a dolor id tortor posuere elementum. Sed mauris nisl, ultrices a, malesuada non, convallis ac, velit. Sed aliquam elit id metus. Donec malesuada, lorem ut pharetra auctor, mi risus viverra enim, vitae pulvinar urna metus at lorem. Vivamus id lorem. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Nulla facilisi. Ut vel odio. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Pellentesque lobortis sem. 

Proin in odio. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Vivamus bibendum arcu nec risus. Nulla iaculis ligula in purus. Etiam vulputate nibh sit amet lectus. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Suspendisse potenti. Suspendisse eleifend. Donec blandit nibh hendrerit turpis. Integer accumsan posuere odio. Ut commodo augue malesuada risus. Curabitur augue. Praesent volutpat nunc a diam. Nulla lobortis interdum dolor. Nunc imperdiet, ipsum ac tempor iaculis, nunc. 
";

        [Test]
        public void Remove()
        {
            string text = parser.Tagger(shortText + stub, "Test", out noChange, ref summary, false, true);
            //Stub, tag not removed
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text));

            text = parser.Tagger(longText + stub, "Test", out noChange, ref summary, false, true);
            //stub tag removed
            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text));

            text = parser.Tagger("{{wikify}}" + Regex.Replace(longText, @"(\w+)", "[[$1]]"), "Test", out noChange, ref summary, false, true);
            //wikify tag removed
            Assert.IsFalse(WikiRegexes.Wikify.IsMatch(text));


            Globals.UnitTestIntValue = 4;
            text = parser.Tagger("{{uncategorised}}", "Test", out noChange, ref summary, false, true);
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text));

            text = parser.Tagger("{{uncategorised|date=January 2009}}", "Test", out noChange, ref summary, false, true);
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text));

            text = parser.Tagger("{{uncategorised|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}", "Test", out noChange, ref summary, false, true);
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text));
            Assert.IsTrue(string.IsNullOrEmpty(text));

            Globals.UnitTestBoolValue = false;

            text = parser.Tagger("{{orphan}}", "Test", out noChange, ref summary, false, true);
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));

            Globals.UnitTestBoolValue = true;

            text = parser.Tagger("{{orphan}}", "Test", out noChange, ref summary, false, true);
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text));
        }

        [Test]
        public void Update()
        {
            //Test of updating some of the non dated tags
            string text = parser.Tagger("{{fact}}", "Test", out noChange, ref summary, false, true);

            Assert.IsTrue(text.Contains("{{Fact|date={{subst:CURRENTMONTHNAME}}"));
            Assert.IsFalse(text.Contains("{{fact}}"));

            text = parser.Tagger("{{template:fact}}", "Test", out noChange, ref summary, false, true);

            Assert.IsTrue(text.Contains("{{Fact|date={{subst:CURRENTMONTHNAME}}"));
            Assert.IsFalse(text.Contains("{{fact}}"));
        }

        [Test]
        public void General()
        {
            Assert.AreEqual("#REDIRECT [[Test]]", parser.Tagger("#REDIRECT [[Test]]", "Test", out noChange, ref summary, true, true));
            Assert.IsTrue(noChange);

            Assert.AreEqual(shortText, parser.Tagger(shortText, "Talk:Test", out noChange, ref summary, true, true));
            Assert.IsTrue(noChange);

            //No change as no add/remove
            Assert.AreEqual(shortText, parser.Tagger(shortText, "Test", out noChange, ref summary, false, false));
            Assert.IsTrue(noChange);

            Assert.AreEqual("{{Test Template}}", parser.Tagger("{{Test Template}}", "Test", out noChange, ref summary, true, true));
            Assert.IsTrue(noChange);

            // {{Pt}} is now {{Pt icon}}
            Assert.AreEqual("hello {{pt}} hello", parser.Tagger("hello {{pt}} hello", "Test", out noChange, ref summary, true, true));
            Assert.IsTrue(noChange);

            Assert.AreEqual("hello {{Pt}} hello", parser.Tagger("hello {{Pt}} hello", "Test", out noChange, ref summary, true, true));
            Assert.IsTrue(noChange);
        }

        [Test]
        public void ConversionsTests()
        {
            Assert.AreEqual(@"{{cleanup|date=January 2008}} Article text here", Parsers.Conversions(@"{{articleissues|cleanup=January 2008}} Article text here"));
            Assert.AreEqual(@"{{cleanup|date=January 2008}} Article text here", Parsers.Conversions(@"{{article issues|cleanup=January 2008}} Article text here"));
            Assert.AreEqual(@"{{cleanup|date=January 2008}} Article text here", Parsers.Conversions(@"{{articleissues|
            cleanup=January 2008}} Article text here"));
            Assert.AreEqual(@"{{cleanup|date=January 2009}} Article text here", Parsers.Conversions(@"{{Articleissues|cleanup=January 2009}} Article text here"));
            Assert.AreEqual(@"{{trivia|date= January 2008}} Article text here", Parsers.Conversions(@"{{articleissues|trivia = January 2008}} Article text here"));
            Assert.AreEqual(@"{{trivia|date= May 2010}} Article text here", Parsers.Conversions(@"{{articleissues|trivia = May 2010}} Article text here"));

            // no changes
            string a = @"{{article issues|trivia=January 2008|cleanup=January 2008}} Article text here";
            Assert.AreEqual(a, Parsers.Conversions(a));

            a = @"{{ARTICLEISSUES|cleanup=January 2008}} Article text here";
            Assert.AreEqual(a, Parsers.Conversions(a));

            a = @"{{Article issues|cleanup=May 2007|trivia=January 2008}} Article text here";
            Assert.AreEqual(a, Parsers.Conversions(a));

            Assert.AreEqual(@"{{Article issues|cleanup=March 2008|expert=Anime and manga|refimprove=May 2008|date=February 2009}}", Parsers.Conversions(@"{{Article issues|cleanup=March 2008|expert=Anime and manga|refimprove=May 2008|date=February 2009}}"));

            // nofootnotes --> morefootnotes
            Assert.AreEqual(@"Article <ref>A</ref> 
            ==References==
            {{morefootnotes}}
            {{reflist}}", Parsers.Conversions(@"Article <ref>A</ref> 
            ==References==
            {{nofootnotes}}
            {{reflist}}"));

            Assert.AreEqual(@"Article <ref>A</ref> 
            ==References==
            {{morefootnotes}}
            {{reflist}}", Parsers.Conversions(@"Article <ref>A</ref> 
            ==References==
            {{Nofootnotes}}
            {{reflist}}"));

            // no change
            Assert.AreEqual(@"Article 
            ==References==
            {{nofootnotes}}", Parsers.Conversions(@"Article 
            ==References==
            {{nofootnotes}}"));

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#.7B.7Bcommons.7CCategory:XXX.7D.7D_.3E_.7B.7Bcommonscat.7CXXX.7D.7D
            // {{commons|Category:XXX}} > {{commonscat|XXX}}
            Assert.AreEqual(@"{{commons cat|XXX}}", Parsers.Conversions(@"{{commons|Category:XXX}}"));
            Assert.AreEqual(@"{{commons cat|XXX}}", Parsers.Conversions(@"{{Commons|category:XXX}}"));
            Assert.AreEqual(@"{{commons cat|XXX}}", Parsers.Conversions(@"{{Commons| category:XXX }}"));
            Assert.AreEqual(@"{{commons cat|Backgammon|Backgammon}}", Parsers.Conversions(@"{{commons|Category:Backgammon|Backgammon}}"));

            // {{2otheriuses}} --> {{Two other uses}}
            Assert.AreEqual(@"{{Two other uses}}", Parsers.Conversions(@"{{2otheruses}}"));
            Assert.AreEqual(@"{{Two other uses|a|b}}", Parsers.Conversions(@"{{2otheruses|a|b}}"));
            Assert.AreEqual(@"{{Two other uses
|a|b}}", Parsers.Conversions(@"{{2otheruses
|a|b}}"));
            Assert.AreEqual(@"{{Two other uses|aasd}}", Parsers.Conversions(@"{{2otheruses|aasd}}"));

            // {{articleissues}} with {{article issues}}
            Assert.AreEqual(@"{{Article issues|sections=May 2008|POV=March 2008|COI=May 2009}}", Parsers.Conversions(@"{{Articleissues|sections=May 2008|POV=March 2008|COI=May 2009}}"));
            Assert.AreEqual(@"{{article issues|sections=May 2008|POV=March 2008|COI=May 2009}}", Parsers.Conversions(@"{{articleissues|sections=May 2008|POV=March 2008|COI=May 2009}}"));
        }

        [Test]
        public void TestRemoveEmptyArticleIssues()
        {
            Assert.AreEqual(@"", Parsers.Conversions(@"{{Article issues}}"));
            Assert.AreEqual(@"", Parsers.Conversions(@"{{Articleissues}}"));
            Assert.AreEqual(@"", Parsers.Conversions(@"{{article issues}}"));
            Assert.AreEqual(@"", Parsers.Conversions(@"{{articleissues}}"));
            Assert.AreEqual(@"", Parsers.Conversions(@"{{articleissues }}"));
            Assert.AreEqual(@"", Parsers.Conversions(@"{{Article issues|article=y}}"));
            Assert.AreEqual(@"", Parsers.Conversions(@"{{Article issues|article = y}}"));
            Assert.AreEqual(@"", Parsers.Conversions(@"{{Article issues|section = y}}"));
            Assert.AreEqual(@"", Parsers.Conversions(@"{{Article issues|section= y}}"));
            Assert.AreEqual(@"", Parsers.Conversions(@"{{Article issues | section= y}}"));

            // no match, 'section' and 'sections' are different parameters for the template
            Assert.AreEqual(@"{{Article issues|cleanup=May 2008|POV=March 2008}}", Parsers.Conversions(@"{{Article issues|cleanup=May 2008|POV=March 2008}}"));
            Assert.AreEqual(@"{{Article issues|sections=May 2008|POV=March 2008}}", Parsers.Conversions(@"{{Article issues|sections=May 2008|POV=March 2008}}"));
        }

        [Test]
        public void DuplicateTemplateFieldsTests()
        {
            Assert.AreEqual("", Parsers.Conversions(""));

            // duplciate fields
            Assert.AreEqual(@"now {{cite web| url=http://site.net |accessdate = 2008-10-08|title=hello}}", Parsers.Conversions(@"now {{cite web| url=http://site.net |title=hello|accessdate = 2008-10-08|title=hello}}"));
            Assert.AreEqual(@"now {{cite web| url=http://site.net |accessdate = 2008-10-08|title=hello|publisher=BBC}}", Parsers.Conversions(@"now {{cite web| url=http://site.net |title=hello|accessdate = 2008-10-08|title=hello|publisher=BBC}}"));
            Assert.AreEqual(@"now {{Cite web|  url=http://site.net |accessdate = 2008-10-08|title=hello}}", Parsers.Conversions(@"now {{Cite web| title = hello | url=http://site.net |accessdate = 2008-10-08|title=hello}}"));
            Assert.AreEqual(@"now {{cite web| url=http://site.net |accessdate = 2008-10-08| title = hello }}", Parsers.Conversions(@"now {{cite web| url=http://site.net |title=hello|accessdate = 2008-10-08| title = hello }}"));
            Assert.AreEqual(@"now {{cite web| url=http://site.net |date = 2008-10-08|title=hello}}", Parsers.Conversions(@"now {{cite web| url=http://site.net |date = 2008-10-08|date = 2008-10-08|title=hello}}"));
            Assert.AreEqual(@"now {{cite web| url=http://site.net |date = 2008-10-08|title=hello}}", Parsers.Conversions(@"now {{cite web| title=hello|url=http://site.net |date = 2008-10-08|title=hello}}"));
            Assert.AreEqual(@"now {{cite web| title=hello|url=http://site.net |date = 2008-10-08}}", Parsers.Conversions(@"now {{cite web| title=hello|title=hello|url=http://site.net |date = 2008-10-08}}"));

            Assert.AreEqual(@"{{Article issues|wikify=May 2008|POV=May 2008|Expand=June 2008}}", Parsers.Conversions(@"{{Article issues|wikify=May 2008|POV=May 2008|Expand=June 2008|Expand=June 2008}}"));
            Assert.AreEqual(@"{{Article issues|wikify=May 2008|POV=May 2008|Expand=June 2008}}", Parsers.Conversions(@"{{Articleissues|wikify=May 2008|Expand=June 2008|POV=May 2008|Expand=June 2008}}"));

            // null fields
            Assert.AreEqual(@"now {{cite web| url=http://site.net |accessdate = 2008-10-08|title=hello}}", Parsers.Conversions(@"now {{cite web| url=http://site.net |title=|accessdate = 2008-10-08|title=hello}}"));
            Assert.AreEqual(@"now {{cite web| url=http://site.net |accessdate = 2008-10-08|title=hello|publisher=BBC}}", Parsers.Conversions(@"now {{cite web| url=http://site.net |title=|accessdate = 2008-10-08|title=hello|publisher=BBC}}"));
            Assert.AreEqual(@"now {{Cite web| title = hello | url=http://site.net |accessdate = 2008-10-08}}", Parsers.Conversions(@"now {{Cite web| title = hello | url=http://site.net |accessdate = 2008-10-08|title=}}"));
            Assert.AreEqual(@"now {{cite web| url=http://site.net |accessdate = 2008-10-08| title = hello }}", Parsers.Conversions(@"now {{cite web| url=http://site.net |title=|accessdate = 2008-10-08| title = hello }}"));
            Assert.AreEqual(@"now {{cite web| title=hello|url=http://site.net |date = 2008-10-08}}", Parsers.Conversions(@"now {{cite web| title=hello|title=|url=http://site.net |date = 2008-10-08}}"));

            Assert.AreEqual(@"{{Article issues|wikify=May 2008|Expand=June 2008|POV=May 2008}}", Parsers.Conversions(@"{{Article issues|wikify=May 2008|Expand=June 2008|POV=May 2008|Expand=}}"));
            Assert.AreEqual(@"{{Article issues|wikify=May 2008|POV=May 2008|Expand=June 2008}}", Parsers.Conversions(@"{{Article issues|wikify=May 2008|Expand=|POV=May 2008|Expand=June 2008}}"));

            //no Matches
            Assert.AreEqual(@"now {{cite web| title=hello|url=http://site.net |date = 2008-10-08|title=HELLO}}", Parsers.Conversions(@"now {{cite web| title=hello|url=http://site.net |date = 2008-10-08|title=HELLO}}")); // case of field value different
            Assert.AreEqual(@"now {{cite web| url=http://site.net |title=hello|accessdate = 2008-10-08|title=hello t}}", Parsers.Conversions(@"now {{cite web| url=http://site.net |title=hello|accessdate = 2008-10-08|title=hello t}}"));
            Assert.AreEqual(@"now {{cite web| url=http://site.net |title=hello|accessdate = 2008-10-08|name=hello}}", Parsers.Conversions(@"now {{cite web| url=http://site.net |title=hello|accessdate = 2008-10-08|name=hello}}"));
            Assert.AreEqual(@"now {{cite web| url=http://site.net |title=hello|first=|accessdate = 2008-10-08|first=}}", Parsers.Conversions(@"now {{cite web| url=http://site.net |title=hello|first=|accessdate = 2008-10-08|first=}}")); //dupe fields have no value

            Assert.AreEqual(@"{{Article issues|wikify=May 2008|POV=May 2008|Expand=June 2008|Expand=June 2009}}", Parsers.Conversions(@"{{Article issues|wikify=May 2008|POV=May 2008|Expand=June 2008|Expand=June 2009}}"));

        }

        [Test]
        public void ArticleIssues()
        {
            string A1 = @"{{Wikify}} {{expand}}", A2 = @" {{COI}}", A3 = @" the article";
            string A4 = @" {{COI|date=May 2008}}", A5 = @"{{Article issues|POV|prose|spam}} ";

            // adding new {{article issues}}
            Assert.IsTrue(parser.ArticleIssues(A1 + A2 + A3).Contains(@"{{Article issues|wikify|expand|COI}}"));
            Assert.IsTrue(parser.ArticleIssues(A1 + A4 + A3).Contains(@"{{Article issues|wikify|expand|COI date=May 2008}}"));

            // amend existing {{article issues}}
            Assert.IsTrue(parser.ArticleIssues(A5 + A1 + A2 + A3).Contains(@"{{Article issues|POV|prose|spam|wikify|expand|COI"));
            Assert.IsTrue(parser.ArticleIssues(A5 + A1 + A4 + A3).Contains(@"{{Article issues|POV|prose|spam|wikify|expand|COI date=May 2008}}"));

            // insufficient tags
            Assert.IsFalse(Parsers.Conversions(A1 + A3).Contains(@"{{Article issues"));

            // before first heading tag can be used
            string A7 = @"{{trivia}} ==heading==";
            Assert.IsTrue(parser.ArticleIssues(A5 + A3 + A7).Contains(@"{{Article issues|POV|prose|spam|trivia}}"));

            // don't grab tags in later sections of article
            string A6 = @"==head== {{essay}}";
            Assert.AreEqual(A5 + A3 + A6, parser.ArticleIssues(A5 + A3 + A6));

            // title case parameters converted to lowercase
            Assert.AreEqual(@"{{article issues|POV=May 2008|cleanup=May 2008|expand=June 2007}}", parser.ArticleIssues(@"{{article issues|POV=May 2008|cleanup=May 2008|Expand=June 2007}}"));
            Assert.AreEqual(@"{{article issues|POV=May 2008|cleanup=May 2008|expand=June 2007}}", parser.ArticleIssues(@"{{article issues|POV=May 2008|cleanup=May 2008|Expand=June 2007}}"));
            Assert.AreEqual(@"{{article issues|POV=May 2008|cleanup=May 2008| expand = June 2007}}", parser.ArticleIssues(@"{{article issues|POV=May 2008|cleanup=May 2008| Expand = June 2007}}"));
            Assert.AreEqual(@"{{Articleissues|BLPunsourced=May 2008|cleanup=May 2008|expand=June 2007}}", parser.ArticleIssues(@"{{Articleissues|BLPunsourced=May 2008|Cleanup=May 2008|expand=June 2007}}"));

            // parsers function doesn't add tags if total tags would be less than 3
            Assert.AreEqual(@"{{article issues|POV=May 2008}} {{wikify|date=May 2007}}", parser.ArticleIssues(@"{{article issues|POV=May 2008}} {{wikify|date=May 2007}}"));
            Assert.AreEqual(@"{{article issues|POV=May 2008|article=y}} {{wikify|date=May 2007}}", parser.ArticleIssues(@"{{article issues|POV=May 2008|article=y}} {{wikify|date=May 2007}}"));

            // add tags if total would reach 3
            Assert.AreEqual(@"{{article issues|POV=May 2008|wikify date=May 2007|cleanup date=June 2008}}  ", parser.ArticleIssues(@"{{article issues|POV=May 2008}} {{wikify|date=May 2007}} {{cleanup|date=June 2008}}"));

            // don't remove date field where expert field is using it
            Assert.AreEqual(@"{{Article issues|cleanup=March 2008|expert=Anime and manga|refimprove=May 2008|date=February 2009}}", parser.ArticleIssues(@"{{Article issues|Cleanup=March 2008|expert=Anime and manga|refimprove=May 2008|date=February 2009}}"));

            // date field removed where no expert field to use it
            Assert.AreEqual(@"{{Article issues|cleanup=March 2008|COI=March 2008|refimprove=May 2008}}", parser.ArticleIssues(@"{{Article issues|Cleanup=March 2008|COI=March 2008|refimprove=May 2008|date=February 2009}}"));
            // removal of non-existent date field
            Assert.AreEqual(@"{{Article issues|wikfy=May 2008|COI=May 2008|cleanup=May 2008}}", parser.ArticleIssues(@"{{Article issues|wikfy=May 2008|COI=May 2008|cleanup=May 2008|date = March 2007}}"));
            Assert.AreEqual(@"{{Article issues|wikfy=May 2008|COI=May 2008|cleanup=May 2008}}", parser.ArticleIssues(@"{{Article issues|wikfy=May 2008|COI=May 2008|cleanup=May 2008| date=March 2007}}"));
            Assert.AreEqual(@"{{Article issues|wikfy=May 2008|COI=May 2008|cleanup=May 2008}}", parser.ArticleIssues(@"{{Article issues|wikfy=May 2008|COI=May 2008|date = March 2007|cleanup=May 2008}}"));
        }

        [Test]
        public void ArticleIssuesDates()
        {
            // addition of date
            Assert.AreEqual(@"{{Article issues|wikfy=May 2008|COI=May 2008|expand={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}", Parsers.Conversions(@"{{Article issues|wikfy=May 2008|COI=May 2008|expand}}"));
            Assert.AreEqual(@"{{Article issues|wikfy=May 2008|expand={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|COI=May 2008}}", Parsers.Conversions(@"{{Article issues|wikfy=May 2008|expand|COI=May 2008}}"));

            // multiple dates
            Assert.AreEqual(@"{{Article issues|wikfy=May 2008|COI=May 2008|expand={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|external links={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}", Parsers.Conversions(@"{{Article issues|wikfy=May 2008|COI=May 2008|expand|external links}}"));

            // removal of date word
            Assert.AreEqual(@"{{Article issues|wikfy=May 2008|COI=May 2008|expand =March 2009|POV=May 2008}}", Parsers.Conversions(@"{{Article issues|wikfy=May 2008|COI=May 2008|expand date=March 2009|POV=May 2008}}"));
            Assert.AreEqual(@"{{Article issues|wikfy =May 2008|COI =May 2008|expand =March 2009|POV =May 2008}}", Parsers.Conversions(@"{{Article issues|wikfy date=May 2008|COI date=May 2008|expand date=March 2009|POV date=May 2008}}"));
            Assert.AreEqual(@"{{Article issues|wikfy=May 2008|COI=May 2008|expand =March 2009}}", Parsers.Conversions(@"{{Article issues|wikfy=May 2008|COI=May 2008|expand date=March 2009}}"));
            Assert.AreEqual(@"{{article issues|unreferenced=March 2009|wikify=March 2009|cleanup=March 2009|autobiography={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|COI = April 2009}}", Parsers.Conversions(@"{{article issues|unreferenced=March 2009|wikify=March 2009|cleanup=March 2009|autobiography={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|COI =date April 2009}}"));
            Assert.AreEqual(@"{{Article issues|wikfy ={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|COI ={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|expand ={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|POV ={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}", Parsers.Conversions(@"{{Article issues|wikfy date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|COI date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|expand date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|POV date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"));

            // clean of 'do-attempt =July 2006|att=April 2008'
            Assert.AreEqual(@"{{Article issues|wikfy=May 2008|COI=May 2008|do-attempt =April 2008}}", Parsers.Conversions(@"{{Article issues|wikfy=May 2008|COI=May 2008|do-attempt =July 2006|att=April 2008}}"));
            Assert.AreEqual(@"{{Article issues|wikfy=May 2008|do-attempt =April 2008|COI=May 2008}}", Parsers.Conversions(@"{{Article issues|wikfy=May 2008|do-attempt =July 2006|att=April 2008|COI=May 2008}}"));

            // clean of "Copyedit|for=grammar|date=April 2009"to "Copyedit=April 2009"
            Assert.AreEqual(@"{{Article issues|wikfy=May 2008|COI=May 2008|Copyedit =April 2009}}", Parsers.Conversions(@"{{Article issues|wikfy=May 2008|COI=May 2008|Copyedit for=grammar|date=April 2009}}"));
            Assert.AreEqual(@"{{Article issues|wikfy=May 2008|copyedit=April 2009|COI=May 2008}}", Parsers.Conversions(@"{{Article issues|wikfy=May 2008|copyeditfor=grammar|date=April 2009|COI=May 2008}}"));

            // don't add date for expert field
            string A1 = @"{{Article issues|wikfy=May 2008|COI=May 2008|expert}}";
            Assert.AreEqual(A1, Parsers.Conversions(A1));

            string A2 = @"{{Article issues|wikfy=May 2008|COI=May 2008|expert=Fred}}";
            Assert.AreEqual(A2, Parsers.Conversions(A2));

            // don't remove 'update'field
            string A3 = @"{{Article issues|wikfy=May 2008|COI=May 2008|update=May 2008}}";
            Assert.AreEqual(A3, Parsers.Conversions(A3));
        }

        [Test]
        public void NullTemplateParameterRemoval()
        {
            Assert.AreEqual(@"{{Article issues}}", Parsers.Conversions(@"{{Article issues|}}"));
            Assert.AreEqual(@"{{Article issues|POV=May 2008}}", Parsers.Conversions(@"{{Article issues||POV=May 2008}}"));
            Assert.AreEqual(@"{{Article issues|POV=May 2008}}", Parsers.Conversions(@"{{Article issues|  |POV=May 2008}}"));


            Assert.AreEqual(@"{{cite web|url=http://www.site.com|title=hello}}", Parsers.Conversions(@"{{cite web||url=http://www.site.com|title=hello}}"));
            Assert.AreEqual(@"{{cite web|url=http://www.site.com|title=hello}}", Parsers.Conversions(@"{{cite web|url=http://www.site.com|title=hello|}}"));
            Assert.AreEqual(@"{{cite web|url=http://www.site.com|title=hello}}", Parsers.Conversions(@"{{cite web|url=http://www.site.com|  |title=hello}}"));


        }
    }
}
