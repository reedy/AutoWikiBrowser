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
using System.Collections.Generic;

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
            Assert.That(Parsers.FixFootnotes("a=<ref>b</ref>"), Is.Not.StringContaining("\n"));
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
            Assert.AreEqual("<ref name= \"foo\" />", Parsers.SimplifyReferenceTags("<ref name= \"foo\"></ref>"));
            Assert.AreEqual("<ref name=\"foo\" />", Parsers.SimplifyReferenceTags("<ref name=\"foo\" >< / ref >"));
            Assert.AreEqual("<ref name=\"foo\" />", Parsers.SimplifyReferenceTags("<ref name=\"foo\" ></ref>"));

            Assert.AreEqual(@"<ref name=""foo bar"" />", Parsers.SimplifyReferenceTags(@"<ref name=""foo bar"" ></ref>"));
            Assert.AreEqual(@"<ref name='foo bar' />", Parsers.SimplifyReferenceTags(@"<ref name='foo bar' >    </ref>"));

            Assert.AreEqual("<refname=\"foo\"></ref>", Parsers.SimplifyReferenceTags("<refname=\"foo\"></ref>"));

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#Reference_bugs
            Assert.AreEqual(@"<ref name='aftermath' />", Parsers.SimplifyReferenceTags(@"<ref name='aftermath'> </ref>"));
        }

        [Test]
        // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_7#References-2column_not_replaced_with_2_argument_to_reflist
        public void TestFixReferenceListTags()
        {
            Assert.AreEqual("<references/>", Parsers.FixReferenceListTags("<references/>"));
            Assert.AreEqual("<div><references/></div>", Parsers.FixReferenceListTags("<div><references/></div>"));

            Assert.AreEqual("<references>foo</references>", Parsers.FixReferenceListTags("<references>foo</references>"));

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
            Assert.AreEqual(@"now <ref name=""foo bar"">and", Parsers.FixReferenceTags(@"now <ref name=foo bar>and"));
            Assert.AreEqual(@"now <ref name=""foo bar"" /> and", Parsers.FixReferenceTags(@"now <ref name=foo bar /> and"));
            Assert.AreEqual(@"now <ref name = ""foo bar"" >and", Parsers.FixReferenceTags(@"now <ref name = foo bar >and"));

            // <ref name=foo bar"> --> <ref name="foo bar">
            Assert.AreEqual(@"now <ref name=""foo bar"">and", Parsers.FixReferenceTags(@"now <ref name=foo bar"">and"));
            Assert.AreEqual(@"now <ref name=""foo bar"" /> and", Parsers.FixReferenceTags(@"now <ref name=foo bar"" /> and"));
            Assert.AreEqual(@"now <ref name = ""foo bar"" >and", Parsers.FixReferenceTags(@"now <ref name = foo bar"" >and"));

            // <ref name="foo bar> --> <ref name="foo bar">
            Assert.AreEqual(@"now <ref name=""foo bar"">and", Parsers.FixReferenceTags(@"now <ref name=""foo bar>and"));
            Assert.AreEqual(@"now <ref name=""foo bar"" /> and", Parsers.FixReferenceTags(@"now <ref name=""foo bar /> and"));
            Assert.AreEqual(@"now <ref name = ""foo bar"" >and", Parsers.FixReferenceTags(@"now <ref name = ""foo bar >and"));

            // <ref name = ''Fred'> --> <ref name="Fred"> (two apostrophes)
            Assert.AreEqual(@"now <ref name=""foo bar"">and", Parsers.FixReferenceTags(@"now <ref name=''foo bar'>and"));
            Assert.AreEqual(@"now <ref name=""foo bar"" /> and", Parsers.FixReferenceTags(@"now <ref name='foo bar'' /> and"));
            Assert.AreEqual(@"now <ref name = ""foo bar"" >and", Parsers.FixReferenceTags(@"now <ref name = ''foo bar'' >and"));

            // <ref name "foo bar"> --> <ref name="foo bar">
            Assert.AreEqual(@"now <ref name =""foo bar"">and", Parsers.FixReferenceTags(@"now <ref name ""foo bar"">and"));
            Assert.AreEqual(@"now <ref name =""foo bar"" /> and", Parsers.FixReferenceTags(@"now <ref name ""foo bar"" /> and"));

            Assert.AreEqual(@"now <ref name =""foo bar"">and", Parsers.FixReferenceTags(@"now <ref name -""foo bar"">and"));
            Assert.AreEqual(@"now <ref name =""foo bar"" /> and", Parsers.FixReferenceTags(@"now <ref name -""foo bar"" /> and"));
            Assert.AreEqual(@"now <ref name =  ""foo bar"" >and", Parsers.FixReferenceTags(@"now <ref name -  ""foo bar"" >and"));
            Assert.AreEqual(@"now <ref name =""foo bar"">and", Parsers.FixReferenceTags(@"now <ref name +""foo bar"">and"));
            Assert.AreEqual(@"now <ref name =""foo bar"" /> and", Parsers.FixReferenceTags(@"now <ref name +""foo bar"" /> and"));
            Assert.AreEqual(@"now <ref name =  ""foo bar"" >and", Parsers.FixReferenceTags(@"now <ref name +  ""foo bar"" >and"));

            // <ref "foo bar"> --> <ref name="foo bar">
            Assert.AreEqual(@"now <ref name=""foo bar"">and", Parsers.FixReferenceTags(@"now <ref ""foo bar"">and"));
            Assert.AreEqual(@"now <ref name=""foo bar"" /> and", Parsers.FixReferenceTags(@"now <ref ""foo bar"" /> and"));

            // <ref name="Fred" /ref> --> <ref name="Fred"/>
            Assert.AreEqual(@"now <ref name=""Fred""/>was", Parsers.FixReferenceTags(@"now <ref name=""Fred"" /ref>was"));
            Assert.AreEqual(@"now <ref name=""Fred A""/>was", Parsers.FixReferenceTags(@"now <ref name=""Fred A"" /ref>was"));
            Assert.AreEqual(@"now <ref name=""Fred A""/> was", Parsers.FixReferenceTags(@"now <ref name=""Fred A"" / /> was"));

            // <ref name="Fred".> --> <ref name="Fred"/>
            Assert.AreEqual(@"now <ref name=""Fred"".>was", Parsers.FixReferenceTags(@"now <ref name=""Fred"".>was"));
            Assert.AreEqual(@"now <ref name=""Fred""?>was", Parsers.FixReferenceTags(@"now <ref name=""Fred""?>was"));

            // <ref>...<ref/> --> <ref>...</ref>
            Assert.AreEqual(@"now <ref>[http://www.site.com a site]</ref> was", Parsers.FixReferenceTags(@"now <ref>[http://www.site.com a site]<ref/> was"));
            Assert.AreEqual(@"now <ref>[http://www.site.com a site]</ref> was", Parsers.FixReferenceTags(@"now <ref>[http://www.site.com a site]<ref/> was"));

            // <ref>...</red> --> <ref>...</ref>
            Assert.AreEqual(@"now <ref>[http://www.site.com a site]</ref>was", Parsers.FixReferenceTags(@"now <ref>[http://www.site.com a site]</red>was"));
            Assert.AreEqual(@"now <ref>[http://www.site.com a site]</ref>was", Parsers.FixReferenceTags(@"now <ref>[http://www.site.com a site]</red>was"));

            // no Matches
            Assert.AreEqual(@"now <ref name=""foo bar"">and", Parsers.FixReferenceTags(@"now <ref name=""foo bar"">and"));
            Assert.AreEqual(@"now <ref name=""foo bar"" /> and", Parsers.FixReferenceTags(@"now <ref name=""foo bar"" /> and"));
            Assert.AreEqual(@"now <ref name = ""foo bar"" >and", Parsers.FixReferenceTags(@"now <ref name = ""foo bar"" >and"));
            Assert.AreEqual(@"now <ref name = 'foo bar' >and", Parsers.FixReferenceTags(@"now <ref name = 'foo bar' >and"));

            // <REF>and <Ref> to <ref>
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
            Assert.AreEqual(@"<ref name=""Tim""><ref>foo2</ref>", Parsers.FixReferenceTags(@"<ref name=""Tim""><ref>foo2</ref>"));

            // ensure a space between a reference and text (reference within a paragrah)
            Assert.AreEqual(@"Now clearly,<ref>Smith 2004</ref> he was", Parsers.FixReferenceTags(@"Now clearly,<ref>Smith 2004</ref>he was"));
            Assert.AreEqual(@"Now clearly,<ref>Smith 2004</ref> 2 were", Parsers.FixReferenceTags(@"Now clearly,<ref>Smith 2004</ref>2 were"));
            Assert.AreEqual(@"Now clearly,<ref name=Smith/> 2 were", Parsers.FixReferenceTags(@"Now clearly,<ref name=Smith/>2 were"));

            // clean space between punctuation and reference
            Assert.AreEqual(@"Now clearly,<ref>Smith 2004</ref> he was", Parsers.FixReferenceTags(@"Now clearly, <ref>Smith 2004</ref> he was"));
            Assert.AreEqual(@"Now clearly:<ref>Smith 2004</ref> he was", Parsers.FixReferenceTags(@"Now clearly: <ref>Smith 2004</ref> he was"));
            Assert.AreEqual(@"Now clearly;<ref>Smith 2004</ref> he was", Parsers.FixReferenceTags(@"Now clearly; <ref>Smith 2004</ref> he was"));
            Assert.AreEqual(@"Now clearly.<ref name=Smith/> 2 were", Parsers.FixReferenceTags(@"Now clearly.   <ref name=Smith/> 2 were"));

            // trailing spaces in reference
            Assert.AreEqual(@"<ref>foo</ref>", Parsers.FixReferenceTags(@"<ref>foo </ref>"));
            Assert.AreEqual(@"<ref>foo</ref>", Parsers.FixReferenceTags(@"<ref>foo    </ref>"));
            Assert.AreEqual(@"<ref>foo
</ref>", Parsers.FixReferenceTags(@"<ref>foo
</ref>"));
            Assert.AreEqual(@"<ref>foo</ref>", Parsers.FixReferenceTags(@"<ref>  foo </ref>"));

            // empty tags
            Assert.AreEqual(@"", Parsers.FixReferenceTags(@"<ref> </ref>"));
            
            // <ref name="Fred" Smith> --> <ref name="Fred Smith">
            Assert.AreEqual(@"<ref name=""Fred Smith"" />", Parsers.FixReferenceTags(@"<ref name=""Fred"" Smith />"));
            Assert.AreEqual(@"<ref name = ""Fred Smith"" />", Parsers.FixReferenceTags(@"<ref name = ""Fred"" Smith />"));
            Assert.AreEqual(@"<ref name=""Fred-Smith"" />", Parsers.FixReferenceTags(@"<ref name=""Fred""-Smith />"));
            Assert.AreEqual(@"<ref name=""Fred-Smith""/>", Parsers.FixReferenceTags(@"<ref name=""Fred""-Smith/>"));
            Assert.AreEqual(@"<ref name=""Fred-Smith"" >text</ref>", Parsers.FixReferenceTags(@"<ref name=""Fred""-Smith >text</ref>"));
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

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#Re-ordering_references_can_leave_page_number_templates_behind.
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
            const string a = @"'''Article''' is great.<ref name = ""Fred3"">So says Fred</ref>
Article started off pretty good, <ref name = ""Fred3"" /> <ref name = ""John2"" >So says John</ref> and finished well.
End of.";
            const string b = @"'''Article''' is great.<ref name = ""Fred4"">So says Fred</ref>
Article started off pretty good, <ref>So says Tim</ref> <ref>So says John</ref> and finished well.
End of.";
            const string c = @"'''Article''' is great.<ref name = ""Fred5"">So says Fred</ref>
Article started off pretty good,<ref name = ""Fred5"" /> <ref>So says John</ref> and finished well.
End of.";
            const string d = @"'''Article''' is great.<ref name = ""Fred6"">So says Fred</ref>
Article started off pretty good, <ref>So says John</ref> <ref name = ""A"">So says A</ref> and finished well.
End of.";
            const string e = @"'''Article''' is great.<ref name = ""John2"" />
Article is new.<ref name = ""Fred7"">So says Fred</ref>
Article started off pretty good, <ref name = ""John2"" >So says John</ref> <ref name = ""Fred7"" /> and finished well.
End of.";
            Assert.AreEqual(a, Parsers.ReorderReferences(a));
            Assert.AreEqual(b, Parsers.ReorderReferences(b));
            Assert.AreEqual(c, Parsers.ReorderReferences(c));
            Assert.AreEqual(d, Parsers.ReorderReferences(d));
            Assert.AreEqual(e, Parsers.ReorderReferences(e));
        }
        
        [Test]
        public void ReorderReferencesNotWithinReflist()
        {
            // not reordered in <references>...</references> etc.

            string a = @"
<references>
<ref>So says John</ref>
<ref name = ""Fred1"" />
</references>",  b = @"{{reflist|3|refs=
<ref>So says John</ref>
<ref name = ""Fred1"" />
}}";
            string correctpart = @"'''Article''' is great.<ref name = ""Fred1"">So says Fred</ref>
Article started off pretty good, <ref name = ""Fred1"" /> <ref>So says John</ref> and finished well.
End of.";

            Assert.AreEqual(correctpart + a, Parsers.ReorderReferences(correctpart + a));
            Assert.AreEqual(correctpart + b, Parsers.ReorderReferences(correctpart + b));
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
            const string a = @"now<ref name=""Fred"">The Honourable Fred Smith, 2002</ref>but later than<ref name=""Fred"">The Honourable Fred Smith, 2005</ref> was"; // ref text different year
            const string b = @"now<ref name=""Fred"">The Honourable Fred Smith, 2004</ref>but later than<ref name=""Fred"">The Honourable FRED SMITH, 2004</ref> was"; // ref text has different case
            const string c = @"now<ref name=""Fred"">The Honourable Fred Smith, 2003</ref>but later than<ref name=""Fred2"">The Honourable Fred Smith, 2003</ref> was"; // ref name different
            const string c2 = @"now<ref name=""Fred"">Fred Smith, 2003</ref>but later than<ref name=""Fred"">Fred Smith, 2003</ref> was"; // too short

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
            const string d = @"now<ref name=""Bert"">The Honourable Bert Smith, 2002</ref>but later than<ref>The Honourable Bert Smith, 2005</ref> was";
            const string e = @"now<ref name=""Bert"">The Honourable Bert Smith, 2004</ref>but later than<ref>The Honourable BERT SMITH, 2004</ref> was";

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
            const string f = @"now<ref>The Honourable John Smith, 2002</ref>but later than<ref name=""John"">The Honourable John Smith, 2005</ref> was";
            const string g = @"now<ref>The Honourable John Smith, 2004</ref>but later than<ref name=""John"">The HONOURABLE JOHN SMITH, 2004</ref> was";

            Assert.AreEqual(f, Parsers.DuplicateNamedReferences(f));
            Assert.AreEqual(g, Parsers.DuplicateNamedReferences(g)); // reference text casing
        }

        [Test]
        public void DuplicateUnnamedReferences()
        {
            string namedref = @"now <ref name=foo>foo</ref>";
            Assert.AreEqual("", Parsers.DuplicateUnnamedReferences(""));
            
            // no existing named ref
            string nonamedref = @"<ref>""bookrags.com""</ref> foo <ref> ""bookrags.com"" </ref>";
            Assert.AreEqual(nonamedref, Parsers.DuplicateUnnamedReferences(nonamedref));
            
            // existing named ref – good for edit
            Assert.AreEqual(@"<ref name=""bookrags.com"">""bookrags.com""</ref> foo <ref name=""bookrags.com""/>" + namedref, Parsers.DuplicateUnnamedReferences(@"<ref>""bookrags.com""</ref> foo <ref> ""bookrags.com"" </ref>" + namedref));
            Assert.AreEqual(@"<ref name=""bookrags.com"">""bookrags.com""</ref> foo bar <ref name=""abcde"">abcde</ref> now <ref name=""abcde""/>now <ref name=""bookrags.com""/>" + namedref,
                            Parsers.DuplicateUnnamedReferences(@"<ref>""bookrags.com""</ref> foo bar <ref>abcde</ref> now <ref>abcde</ref>now <ref>""bookrags.com""</ref>" + namedref));

            Assert.AreEqual(@"<ref name=""ecomodder.com"">http://ecomodder.com/forum/showthread.php/obd-mpguino-gauge-2702.html</ref> foo bar <ref>http://ecomodder.com/wiki/index.php/MPGuino</ref> now <ref>http://ecomodder.com/wiki/index.php/MPGuino</ref>now <ref name=""ecomodder.com""/>" + namedref,
                            Parsers.DuplicateUnnamedReferences(@"<ref>http://ecomodder.com/forum/showthread.php/obd-mpguino-gauge-2702.html</ref> foo bar <ref>http://ecomodder.com/wiki/index.php/MPGuino</ref> now <ref>http://ecomodder.com/wiki/index.php/MPGuino</ref>now <ref>http://ecomodder.com/forum/showthread.php/obd-mpguino-gauge-2702.html</ref>" + namedref));
            
            const string Ibid = @"now <ref>ibid</ref> was<ref>ibid</ref> there";
            Assert.AreEqual(Ibid, Parsers.DuplicateUnnamedReferences(Ibid));
            
            // nothing to do here
            const string SingleRef = @"now <ref>first</ref> was";
            Assert.AreEqual(SingleRef, Parsers.DuplicateUnnamedReferences(SingleRef));
        }
        
        [Test]
        public void HasNamedReferences()
        {
            Assert.IsTrue(Parsers.HasNamedReferences(@"now <ref name=foo>foo</ref>"));
            Assert.IsTrue(Parsers.HasNamedReferences(@"now <ref name=""foo"">bar</ref>"));
            Assert.IsFalse(Parsers.HasNamedReferences(@"now <ref>foo</ref>"));
            Assert.IsFalse(Parsers.HasNamedReferences(@"now <!--<ref name = foo>foo</ref>-->"));
        }

        [Test]
        public void DeriveReferenceNameTests()
        {
            Assert.AreEqual("Smith", Parsers.DeriveReferenceName("a", "Smith"));
            Assert.AreEqual(@"Smith 2008", Parsers.DeriveReferenceName("a", @"{{cite book|last=Smith|title=hello|year=2008|foo=bar}}"));
            Assert.AreEqual(@"Smith 2008", Parsers.DeriveReferenceName("a", @"{{cite book|last=Smith|title=hello|year=2008}}"));
            Assert.AreEqual(@"Smith 2008 17", Parsers.DeriveReferenceName("a", @"{{cite book|year=2008|last=Smith|title=hello||foo=bar|pages=17}}"));
            Assert.AreEqual(@"Smith 2008 17", Parsers.DeriveReferenceName("a", @"{{citation|pages=17|year=2008|last=Smith|title=hello||foo=bar}}"));
            Assert.AreEqual(@"Smither Bee 2008", Parsers.DeriveReferenceName("a", @"{{Cite book|title=hello|year=2008|last=Smither Bee|foo=bar}}"));
            Assert.AreEqual("Oldani 1982: p.8", Parsers.DeriveReferenceName("a", @"Oldani (1982: p.8)"));
            Assert.AreEqual("elections.sos.state.tx.us", Parsers.DeriveReferenceName("a", @"http://elections.sos.state.tx.us/elchist.exe"));
            Assert.AreEqual("imdb.com", Parsers.DeriveReferenceName("a", @"{{cite web | url=http://www.imdb.com/title/tt0120992/crazycredits | title=Hypnotist The Incredible BORIS is Boris Cherniak | accessdate=2007-10-15 }}"));
            Assert.AreEqual("hello.imdb.com", Parsers.DeriveReferenceName("a", @"{{cite web | url=hello.imdb.com/tt0120992/ | title=Hypnotist The Incredible BORIS is Boris Cherniak | accessdate=2007-10-15 }}"));
            Assert.AreEqual("Caroline Humphrey p.27", Parsers.DeriveReferenceName("a", @"Caroline Humphrey, David Sneath-The end of Nomadism?, p.27"));
            Assert.AreEqual("Dennis, Peter 1995 Page 440", Parsers.DeriveReferenceName("a", @"Dennis, Peter (et al.) (1995) The Oxford Companion to Australian Military History, Melbourne: Oxford University Press, Page 440."));
            Assert.AreEqual("Sepkoski 2002 p.560", Parsers.DeriveReferenceName("a", @"{{cite journal
  | last = Sepkoski| first = Jack| authorlink =| coauthors =| title =  A compendium of fossil marine animal genera (Trilobita entry)| journal = Bulletins of American Paleontology| volume = 364| issue =| pages = p.560| publisher =| location =| date = 2002| url = http://strata.ummp.lsa.umich.edu/jack/showgenera.php?taxon=307&rank=class| doi =| id =| accessdate = 2008-01-12 }}"));
            Assert.AreEqual("Sepkoski 2002 p.560", Parsers.DeriveReferenceName("a", @"{{Cite journal
  | last = Sepkoski| first = Jack| authorlink =| coauthors =| title =  A compendium of fossil marine animal genera (Trilobita entry)| journal = Bulletins of American Paleontology| volume = 364| issue =| pages = p.560| publisher =| location =| year = 2002| url = http://strata.ummp.lsa.umich.edu/jack/showgenera.php?taxon=307&rank=class| doi =| id =| accessdate = 2008-01-12 }}"));
            Assert.AreEqual("gillan.com", Parsers.DeriveReferenceName("a", "[http://www.gillan.com/anecdotage-12.html Ian Gillan's official website: Caramba]"));
            Assert.AreEqual("christianitytoday.com", Parsers.DeriveReferenceName("a", @"{{cite web | title= Founder of China's Born Again Movement Set Free| work= Christianity Today | url= http://www.christianitytoday.com/ct/2000/augustweb-only/22.0.html| accessdate=2008-02-19}}"));
            Assert.AreEqual("Moscow Bound 1993", Parsers.DeriveReferenceName("a", @"Moscow Bound: Policy, Politics, and the POW/MIA Dilemma, John M. G. Brown, Veteren Press, Eureka Springs, California, 1993, Chapt. 14"));
            Assert.AreEqual("Boris Nikolayevich Kampov 2007", Parsers.DeriveReferenceName("a", @"""Boris Nikolayevich Kampov,"" ''Contemporary Authors Online'', Thomson Gale, 2007."));

            Assert.AreEqual("Farlow 1999 8", Parsers.DeriveReferenceName("a", @"{{cite book| last = Farlow| first = James O.| coauthors = M. K. Brett-Surmann| title = The Complete Dinosaur| publisher = Indiana University Press| date = 1999| location = Bloomington, Indiana| pages = 8| isbn = 0-253-21313-4}}"));
            Assert.AreEqual("Globe-trotting with the Grim Reaper", Parsers.DeriveReferenceName("a", @"JAY STRAFFORD, ""[http://www.timesdispatch.com/rtd/entertainment/books_literature/article/BMYS25_20090121-192837/184966/ Globe-trotting with the Grim Reaper],"" ''Richmond-Times Dispatch'' (January 25, 2009)."));


            Assert.AreEqual("ReferenceA", Parsers.DeriveReferenceName("a", @""));

            Assert.AreEqual("ReferenceA", Parsers.DeriveReferenceName("a", @"* Cf. Ezriel Carlebach entry in the Hebrew Wikipedia"));
            
            Assert.AreEqual("Bray, Warwick 1968 93-96", Parsers.DeriveReferenceName("a", @"{{cite book |author=Bray, Warwick |year=1968 |chapter=Everyday Life of The Aztecs |pages=93-96}}"));
            Assert.AreEqual("Olson 2000 84", Parsers.DeriveReferenceName("a", @"{{harv|Olson|2000|p=84}}"));
            Assert.AreEqual("Olson 2000 84", Parsers.DeriveReferenceName("a", @"{{harv|Olson|2000|pp =84}}"));
            Assert.AreEqual("Olson 2000", Parsers.DeriveReferenceName("a", @"{{harvnb|Olson|2000 }}"));
            Assert.AreEqual("Olson 2000 84", Parsers.DeriveReferenceName("a", @"{{harvcolnb|Olson|2000|p=84}}"));
            Assert.AreEqual("Olson 2000 84", Parsers.DeriveReferenceName("a", @"{{Harvcolnb|Olson|2000|p=84}}"));
            Assert.AreEqual(@"Caggiano 1983", Parsers.DeriveReferenceName("a", @"{{Harvnb|Caggiano|Duncan|1983}}"));
            Assert.AreEqual(@"Simpson 1986", Parsers.DeriveReferenceName("a", @"{{Harvnb|Simpson|others|1986}}"));

            Assert.AreEqual("reloadbench.com", Parsers.DeriveReferenceName("a", @"Reload Bench [http://reloadbench.com/cartridges/w17bee.html]"));

            Assert.AreEqual("The Oxford English Dictionary 1989", Parsers.DeriveReferenceName("a", @"""-logy."" ''The Oxford English Dictionary'', Second Edition. Oxford University Press, 1989. retrieved 20 Aug 2008."));

            Assert.AreEqual("firstaif", Parsers.DeriveReferenceName("a", @"{{cite web|title=firstaif|url=http://www.firstaif.info/pages/nz_mounted.htm}}</ref><ref>{{cite web|title=diggerhistory|url=http://www.diggerhistory.info/pages-nz/nzef.htm}}"));


            Assert.AreEqual("Bloom Cigar Company", Parsers.DeriveReferenceName("a", @"{{cite web
  | last =
  | first =
  | title = Welcome to the on-line catalog of Bloom Cigar Company in Pittsburgh, home of Cigar CampTM
  | work =
  | publisher = Bloom Cigar Company
  | date = 2003
  | url = http://www.bloomcigar.com/catalog/
  | accessdate = 2008-11-01
  | archiveurl = http://www.webcitation.org/5c1PcWcc8
  | archivedate = 2008-11-01}}"));

            Assert.AreEqual("Kuykendall, R.S. 1967 p. 628", Parsers.DeriveReferenceName("a", @"Kuykendall, R.S. (1967). The Hawaiian Kingdom, Vol. III, 1874-1893: The Kalakaua Dynasty. Honolulu: University of Hawaii Press. p. 628."));

            Assert.AreEqual(@"Bajanov 2003: 2-3", Parsers.DeriveReferenceName("a", @"[[#Banj1930|Bajanov 2003]]: 2-3"));

            // doesn't provide what's already in use
            Assert.AreEqual("ReferenceB", Parsers.DeriveReferenceName("a<ref name=ReferenceA>foo</ref> now", @"* Cf. Ezriel Carlebach entry in the Hebrew Wikipedia"));
            Assert.AreEqual("ReferenceC", Parsers.DeriveReferenceName("a<ref name=ReferenceA>foo</ref> now <ref name=ReferenceB>foo</ref>", @"* Cf. Ezriel Carlebach entry in the Hebrew Wikipedia"));
            
            Assert.AreEqual(@"books.Google.com", Parsers.DeriveReferenceName(@"a <ref name=""Smither Bee 2008"">a</ref> was", @"{{Cite book|title=hello|year=2008|last=Smither Bee|foo=bar|url = http://books.Google.com/special}}"));
            Assert.AreEqual(@"books.Google.com", Parsers.DeriveReferenceName(@"a <ref name='Smither Bee 2008'>a</ref> was", @"{{Cite book|title=hello|year=2008|last=Smither Bee|foo=bar|url = http://books.Google.com/special}}"));
        }

        [Test]
        public void SameRefDifferentName()
        {
            Assert.AreEqual(@"Foo<ref name=Jones>Jones 2005</ref> and bar<ref name=""Jones"">Jones 2005</ref>",
                            Parsers.SameRefDifferentName(@"Foo<ref name=Jones>Jones 2005</ref> and bar<ref name=J5>Jones 2005</ref>"));

            Assert.AreEqual(@"Foo<ref name=Jones>Jones 2005</ref> and bar<ref name=""Jones"">Jones 2005</ref> and more<ref name=""Jones""/>",
                            Parsers.SameRefDifferentName(@"Foo<ref name=Jones>Jones 2005</ref> and bar<ref name=J5>Jones 2005</ref> and more<ref name=J5/>"));

            Assert.AreEqual(@"Foo<ref name=Jones>Jones 2005</ref> and bar<ref name=""Jones"">
Jones 2005</ref>",
                            Parsers.SameRefDifferentName(@"Foo<ref name=Jones>Jones 2005</ref> and bar<ref name=J5>
Jones 2005</ref>"));

            // leading/trailing whitespace in ref doesn't matter
            Assert.AreEqual(@"Foo<ref name=Jones>Jones 2005</ref> and bar<ref name=""Jones"">Jones 2005   </ref>",
                            Parsers.SameRefDifferentName(@"Foo<ref name=Jones>Jones 2005</ref> and bar<ref name=J5>Jones 2005   </ref>"));
        }

        [Test]
        public void SameNamedRefShortText()
        {

            Assert.AreEqual(@"Foo<ref name=Jones>Jones 2005 extra words of interest</ref> and bar<ref name=""Jones""/>",
                            Parsers.SameRefDifferentName(@"Foo<ref name=Jones>Jones 2005 extra words of interest</ref> and bar<ref name=Jones>2</ref>"));

            Assert.AreEqual(@"Foo<ref name=""Jones""/> and bar<ref name=Jones>Jones 2005 extra words of interest</ref>",
                            Parsers.SameRefDifferentName(@"Foo<ref name=Jones>2</ref> and bar<ref name=Jones>Jones 2005 extra words of interest</ref>"));

            Assert.AreEqual(@"Foo<ref name=Jones>Jones 2005 extra words of interest</ref> and bar<ref name=""Jones""/> and bar2<ref name=""Jones""/>",
                            Parsers.SameRefDifferentName(@"Foo<ref name=Jones>Jones 2005 extra words of interest</ref> and bar<ref name=Jones>2</ref> and bar2<ref name=Jones>3</ref>"));

            Assert.AreEqual(@"Foo<ref name=Jones>Jones 2005 extra words of interest</ref> and bar2<ref name=""Jones""/>",
                            Parsers.SameRefDifferentName(@"Foo<ref name=Jones>Jones 2005 extra words of interest</ref> and bar2<ref name=Jones>[see above]</ref>"));

        }
        
        [Test]
        public void BadCiteParameters()
        {
            Dictionary<int, int> Found = new Dictionary<int, int>();

            // standard cases
            Found.Add(15, 3);
            Assert.AreEqual(Found, Parsers.BadCiteParameters(@"now {{cite web|foo=bar|date=2009}} was"));
            
            Found.Remove(15);
            Found.Add(15, 6);
            Assert.AreEqual(Found, Parsers.BadCiteParameters(@"now {{cite web|foodoo=bar|date=2009}} was"));
            
            Found.Remove(15);
            
            // no errors here
            Assert.AreEqual(Found, Parsers.BadCiteParameters(@"now {{cite web|url=bar|date=2009}} was"));
            Assert.AreEqual(Found, Parsers.BadCiteParameters(@"now {{cite web|url=bar|date=2009|first1=Foo|last1=Great |first2=Bar|title=here}} was"));
            
            // multiple errors
            Found.Add(15, 6);
            Found.Add(36, 4);
            Assert.AreEqual(Found, Parsers.BadCiteParameters(@"now {{cite web|foodoo=bar|date=2009|bert= false}} was"));
        }
        
        [Test]
        public void AddMissingReflist()
        {
            // above cats
            const string SingleRef = @"now <ref>foo</ref>", Cat = @"
[[Category:Here]]", ExtLinks = @"==External links==
*[http://www.site.com hello]";
            Assert.AreEqual(SingleRef + "\r\n\r\n" + @"==References==
{{Reflist}}" + Cat, Parsers.AddMissingReflist(SingleRef + Cat));
            
            // above references section
            const string References = @"==References==
{{portal|foo}}";
            Assert.AreEqual(SingleRef + "\r\n" + @"==References==
{{Reflist}}
{{portal|foo}}" + Cat, Parsers.AddMissingReflist(SingleRef + "\r\n" + References + Cat));
            
            // above external links
            Assert.AreEqual(SingleRef + "\r\n\r\n" + @"==References==
{{Reflist}}" + "\r\n" + ExtLinks, Parsers.AddMissingReflist(SingleRef + "\r\n" + ExtLinks));
            
            // reflist already present
            Assert.AreEqual(SingleRef + "\r\n\r\n" + @"==References==
{{Reflist}}" + Cat, Parsers.AddMissingReflist(SingleRef + "\r\n\r\n" + @"==References==
{{Reflist}}" + Cat));
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
            
            // nothing to do here
            Assert.AreEqual("[[dog|]]", Parsers.SimplifyLinks("[[dog|]]"));
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

            Assert.AreEqual(@"* {{Polish2|Krzepice (województwo dolnośląskie)|[[24 November]] [[2007]]}}
", parser.FixDates(@"* {{Polish2|Krzepice (województwo dolnośląskie)|[[24 November]] [[2007]]}}<br><br>  "));
        }

        [Test]
        public void FixDatesCommaErrors()
        {
            const string correct1 = @"Retrieved on April 14, 2009 was";
            Assert.AreEqual(correct1, parser.FixDates(@"Retrieved on April, 14, 2009 was"));
            Assert.AreEqual(correct1, parser.FixDates(@"Retrieved on April , 14 , 2009 was"));
            Assert.AreEqual(correct1, parser.FixDates(@"Retrieved on April , 14 ,2009 was"));

            Assert.AreEqual(correct1, parser.FixDates(@"Retrieved on April 14,2009 was"));
            Assert.AreEqual(correct1, parser.FixDates(@"Retrieved on April 14 ,2009 was"));
            Assert.AreEqual(correct1, parser.FixDates(@"Retrieved on April 14 2009 was"));
            
            // don't change image names
            string image1 = @"now foo [[Image:Foo July 24 2009.png]] was";
            Assert.AreEqual(image1, parser.FixDates(image1));
            
            const string correct2 = @"Retrieved on 14 April 2009 was";
            Assert.AreEqual(correct2, parser.FixDates(@"Retrieved on 14 April, 2009 was"));
            Assert.AreEqual(correct2, parser.FixDates(@"Retrieved on 14 April , 2009 was"));
            Assert.AreEqual(correct2, parser.FixDates(@"Retrieved on 14 April,  2009 was"));
            
            const string nochange1 = @"On 14 April, 2590 people";
            Assert.AreEqual(nochange1, parser.FixDates(nochange1));
        }
        
        [Test]
        public void FixDatesRanges()
        {
            const string correct1 = @"On 3–17 May 2009 a dog";
            Assert.AreEqual(correct1, parser.FixDates(@"On 3-17 May 2009 a dog"));
            Assert.AreEqual(correct1, parser.FixDates(@"On 3 - 17 May 2009 a dog"));
            Assert.AreEqual(correct1, parser.FixDates(@"On 3 -17 May 2009 a dog"));
            Assert.AreEqual(correct1, parser.FixDates(@"On 3 May-17 May 2009 a dog"));
            Assert.AreEqual(correct1, parser.FixDates(@"On 3 May - 17 May 2009 a dog"));
            Assert.AreEqual(correct1, parser.FixDates(@"On 3 May – 17 May 2009 a dog"));
            Assert.AreEqual(correct1, parser.FixDates(@"On 3 May – 17 May, 2009 a dog"));
            
            // American format
            const string correct2 = @"On May 3–17, 2009 a dog";
            Assert.AreEqual(correct2, parser.FixDates(@"On May 3-17, 2009 a dog"));
            Assert.AreEqual(correct2, parser.FixDates(@"On May 3-17 2009 a dog"));
            Assert.AreEqual(correct2, parser.FixDates(@"On May 3 - 17 2009 a dog"));
            Assert.AreEqual(correct2, parser.FixDates(@"On May 3   -17 2009 a dog"));
            Assert.AreEqual(correct2, parser.FixDates(@"On May 3 - May 17 2009 a dog"));
            Assert.AreEqual(correct2, parser.FixDates(@"On May 3 - May 17, 2009 a dog"));
            Assert.AreEqual(correct2, parser.FixDates(@"On May 3 – May 17 2009 a dog"));
            Assert.AreEqual(correct2, parser.FixDates(@"On May 3 – May 17, 2009 a dog"));
        }

        [Test]
        public void FixDatesRaw()
        {
            Assert.AreEqual("the later 1990s", Parsers.FixDatesRaw("the later 1990's"));
            Assert.AreEqual("the 1980s and 1990s were", Parsers.FixDatesRaw("the 1980's and 1990's were"));
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
        public void FixPeopleCategoriesTests()
        {
            // must be about a person
            const string a0 = @"'''Fred Smith''' (born 1960) is a bloke.";
            Assert.AreEqual(a0, Parsers.FixPeopleCategories(a0, "foo"));

            // birth
            const string a1 = @"'''Fred Smith''' (born 1960) is a bloke. {{Persondata}}";
            const string b2 = @"[[Category:1960 births]]";
            Assert.AreEqual(a1 + "\r\n" + b2, Parsers.FixPeopleCategories(a1, "foo"));

            const string a2 = a1 + "\r\n" + @"[[Category:1990 deaths]]";
            Assert.AreEqual(a2 + "\r\n" + b2, Parsers.FixPeopleCategories(a2, "foo"));

            const string b5 = @"Some words {{birth date and age|1960|01|9}}";
            Assert.AreEqual(b5 + "\r\n" + b2, Parsers.FixPeopleCategories(b5, "foo"));

            const string b6 = @"'''Fred Lerdahl''' (born [[March 10]] [[1960]]) [[Category:Living people]]";
            Assert.AreEqual(b6 + "\r\n" + b2, Parsers.FixPeopleCategories(b6, "foo"));

            // catch living person and very old birth date
            const string b6a = @"'''Fred Lerdahl''' (born [[March 10]] [[1860]]) [[Category:Living people]]";
            Assert.AreEqual(b6a, Parsers.FixPeopleCategories(b6a, "foo"));

            const string b7 = @"'''William Arthur O'Donnell''' (born May 4, 1960 in [[Springhill, Nova Scotia]], Canada) is [[Category:Living people]]";
            Assert.AreEqual(b7 + "\r\n" + b2, Parsers.FixPeopleCategories(b7, "foo"));

            const string b8 = @"'''Burrell Carver Smith''' (born [[December 16]], [[1960]] in upstate New York) [[Category:Living people]]";
            Assert.AreEqual(b8 + "\r\n" + b2, Parsers.FixPeopleCategories(b8, "foo"));

            const string b9 = @"'''Fredro Starr''' (born '''Fredro Scruggs''' on [[April 18]] [[1960]] in [[Jamaica, Queens]]) is an [[Category:Living people]]";
            Assert.AreEqual(b9 + "\r\n" + b2, Parsers.FixPeopleCategories(b9, "foo"));

            const string b10 = @"'''Phillip Rhodes''' (born [[May 26]], [[1960]]) was a [[Category:Living people]]";
            Assert.AreEqual(b10 + "\r\n" + b2, Parsers.FixPeopleCategories(b10, "foo"));

            // no matches when two birth dates in article
            const string b11 = @"'''Kid 'n Play''' was a [[hip-hop music|hip-hop]] and [[comedy]] duo from [[New York City]] that was popular in the late 1980s and early 1990s. The duo comprised '''[[Christopher Kid Reid]]''' (born [[April 5]] [[1964]] in [[The Bronx|The Bronx, New York City]])
and '''[[Christopher Martin (entertainer)|Christopher Play Martin]]''' (born [[July 10]] [[1962]] in [[Queens, New York City]]). Besides their successful musical careers, Kid 'n Play are also notable for branching out into acting. [[Category:Living people]]";
            Assert.AreEqual(b11, Parsers.FixPeopleCategories(b11, "foo"));

            // death
            const string a3 = @"'''Fred Smith''' (died 1960) is a bloke. {{Persondata}}";
            const string b3 = @"[[Category:1960 deaths]]";
            Assert.AreEqual(a3 + "\r\n" + b3, Parsers.FixPeopleCategories(a3, "foo"));

            const string a4 = a3 + "\r\n" + @"[[Category:1990 births]]";
            Assert.AreEqual(a4 + "\r\n" + b3, Parsers.FixPeopleCategories(a4, "foo"));

            const string d = @"'''Fred Smith''' (born 11 May 1950 - died 17 August 1990) is a bloke.
[[Category:1960 births|Smith, Fred]]";
            Assert.AreEqual(d + "\r\n" + @"[[Category:1990 deaths|Smith, Fred]]", Parsers.FixPeopleCategories(d, "foo"));

            const string d2 = @"'''Johnny Sandon''' (originally named '''Billy Beck''') (born in 1960, in Lıverpool, Lancashire died 23 December 1990) was {{Persondata}}";
            Assert.AreEqual(d2 + "\r\n" + b2 + "\r\n" + @"[[Category:1990 deaths]]", Parsers.FixPeopleCategories(d2, "foo"));

            // BC death
            const string d3 = @"'''Aeacides''' ({{lang-el|Aἰακίδης}}; died 313 BC), king {{persondata}}";
            Assert.AreEqual(d3 + "\r\n" + @"[[Category:313 BC deaths]]", Parsers.FixPeopleCategories(d3, "foo"));

            const string d4 = @"Some words {{death date and age|1960|01|9}}";
            Assert.AreEqual(d4 + "\r\n" + @"[[Category:1960 deaths]]", Parsers.FixPeopleCategories(d4, "foo"));

            // no matches if not identified as born
            const string b1 = @"'''Fred Smith''' is a bloke.";
            Assert.AreEqual(b1, Parsers.FixPeopleCategories(b1, "foo"));

            // no matches if already categorised
            const string a = @"'''Fred Smith''' (born 1960) is a bloke.
[[Category:1960 births]]
[[Category:1990 deaths]]";
            const string b = @"'''Fred Smith''' is a bloke.
[[Category:1960 births]]
[[Category:1990 deaths]]";
            const string c = @"'''Fred Smith''' is a bloke.
[[Category:1960 births|Smith, Fred]]
[[Category:1990 deaths|Smith, Fred]]";
            const string e = @"'''Fred Smith''' (born [[1950]]) is a bloke.
[[Category:1950 births|Smith, Fred]]
{{Recentlydeceased}}";
            const string f = @"'''Fred Smith''' (born 1950) is a bloke.
[[Category:1950 births|Smith, Fred]]
{{recentlydeceased}}";
            const string g = @"'''Fred Smith''' (born 1950) is a bloke.
[[Category:1950 births|Smith, Fred]]
[[Category:Year of death missing]]";
            const string h = @"'''Fred Smith''' (born 1950) is a bloke. {{lifetime|||Smith}}";

            Assert.AreEqual(a, Parsers.FixPeopleCategories(a, "foo"));
            Assert.AreEqual(b, Parsers.FixPeopleCategories(b, "foo"));
            Assert.AreEqual(c, Parsers.FixPeopleCategories(c, "foo"));
            Assert.AreEqual(e, Parsers.FixPeopleCategories(e, "foo"));
            Assert.AreEqual(f, Parsers.FixPeopleCategories(f, "foo"));
            Assert.AreEqual(g, Parsers.FixPeopleCategories(g, "foo"));
            Assert.AreEqual(h, Parsers.FixPeopleCategories(h, "foo"));

            // year of birth uncertain
            const string u = "\r\n" + @"[[Category:Year of birth uncertain]]";

            const string u2 = @"'''Charles Meik''' (born around 1330 in [[Ghent]] - [[22 July]] [[1387]]) {{Persondata}}";
            Assert.AreEqual(u2 + u + @"
[[Category:1387 deaths]]", Parsers.FixPeopleCategories(u2, "foo"));

            const string u2a = @"'''Charles Meik''' (born either 1330 or 1331 in [[Ghent]] - [[22 July]] [[1387]]) {{Persondata}}";
            Assert.AreEqual(u2a + u + @"
[[Category:1387 deaths]]", Parsers.FixPeopleCategories(u2a, "foo"));

            const string u2b = @"'''Charles Meik''' (born ~1330 in [[Ghent]] - [[22 July]] [[1387]]) {{Persondata}}";
            Assert.AreEqual(u2b + u + @"
[[Category:1387 deaths]]", Parsers.FixPeopleCategories(u2b, "foo"));

            const string u3 = @"'''Yusuf Ibn Muhammad Ibn Yusuf al-Fasi''' ([[1530]]/[[1531|31]]{{Fact|date=February 2007}} in [[Ksar-el-Kebir]], [[Morocco]] – 14 August [[1604]] in [[Fes, Morocco]]) {{persondata}}";
            Assert.AreEqual(u3 + u + @"
[[Category:1604 deaths]]", Parsers.FixPeopleCategories(u3, "foo"));

            // no matches when approximate year of birth
            const string b12 = @"'''Judith Victor Grabiner''' (born about 1938) is {{Persondata}}";
            Assert.AreEqual(b12 + u, Parsers.FixPeopleCategories(b12, "foo"));

            const string b13 = @"'''Judith Victor Grabiner''' (born circa 1938) is {{Persondata}}";
            Assert.AreEqual(b13 + u, Parsers.FixPeopleCategories(b13, "foo"));

            const string b14 = @"'''Judith Victor Grabiner''' (born before 1938) is {{Persondata}}";
            Assert.AreEqual(b14 + u, Parsers.FixPeopleCategories(b14, "foo"));

            const string b15 = @"'''Judith Victor Grabiner''' (born 1938 or 1939) is {{Persondata}}";
            Assert.AreEqual(b15 + u, Parsers.FixPeopleCategories(b15, "foo"));

            // born BC
            const string b16 = @"'''Phillipus Rhodicus''' (born 220 BC) was a {{Persondata}}";
            Assert.AreEqual(b16 + "\r\n" + @"[[Category:220 BC births]]", Parsers.FixPeopleCategories(b16, "foo"));

            // no change: birth date not present so not 'uncertain'
            const string n1 = @"'''Thomas F. Goreau''' (born in [[Germany]], died 1970 in [[Jamaica]]) was [[Category:1970 deaths]]";
            Assert.AreEqual(n1, Parsers.FixPeopleCategories(n1, "foo"));

            const string n2 = @"'''Charles Meik''' (born? - 1923) was an {{Persondata}}";
            Assert.AreEqual(n2, Parsers.FixPeopleCategories(n2, "foo"));

            const string n2a = @"'''Anatoly Rasskazov''' (born 1960(?)) was {{persondata}}";
            Assert.AreEqual(n2a, Parsers.FixPeopleCategories(n2a, "foo"));

            // no changes
            const string n3 = @"'''Johannes Widmann''' (born c. 1460 in [[Cheb|Eger]]; died after 1498 in [[Leipzig]]) [[Category:1460s births]]";
            Assert.AreEqual(n3, Parsers.FixPeopleCategories(n3, "foo"));

            const string n4 = @"'''John Hulme''' (born circa 1970) is [[Category:Living people]]
[[Category:Year of birth missing (living people)]]";

            Assert.AreEqual(n4, Parsers.FixPeopleCategories(n4, "foo"));

            // don't use born info if after died info in text
            const string n5 = @"'''Alexander II''' ({{lang-ka|ალექსანდრე II, '''''Aleksandre II'''''}}) (died [[April 1]], [[1510]]) was a.
* Prince David (born 1505)
[[Category:1510 deaths]]";
            Assert.AreEqual(n5, Parsers.FixPeopleCategories(@"'''Alexander II''' ({{lang-ka|ალექსანდრე II, '''''Aleksandre II'''''}}) (died [[April 1]], [[1510]]) was a.
* Prince David (born 1505)
[[Category:1510 deaths]]", "foo"));

            // don't pick up outside of zeroth section
            const string n6 = @"foo {{persondata}}
== hello ==
{{birth date and age|1960|01|9}}";
            Assert.AreEqual(n6, Parsers.FixPeopleCategories(n6, "foo"));

            // don't grab a number out of a reference
            const string n7 = @"'''Buck Peterson''' (born in [[Minnesota, United States]]<ref name=extreme_1970/>) {{persondata}}";

            Assert.AreEqual(n7, Parsers.FixPeopleCategories(n7, "foo"));

            // don't accept if dash before year: could be death date
            const string n8 = @"'''Wilhem Heinrich Kramer''' (born [[Dresden]] – 1765) {{persondata}}";

            Assert.AreEqual(n8, Parsers.FixPeopleCategories(n8, "foo"));

            // date of death too far from bold name to be correct
            const string n9 = @"'''Æthelstan''' was king of [[East Anglia]] in the 9th century.

As with the other kings of East Anglia, there is very little textual information available. He did, however, leave an extensive coinage of both portrait and non-portrait type (for example, Coins of England and the United Kingdom, Spink and Son, London, 2005 and the Fitzwilliam Museum database of early medieval coins. [http://www.fitzmuseum.cam.ac.uk/coins/emc]

It is suggested that Æthelstan was probably the king who defeated and killed the Mercian kings [[Beornwulf of Mercia|Beornwulf]] (killed 826) and [[Ludeca of Mercia|Ludeca]] (killed 827). He may have attempted to seize power in [[East Anglia]] on the death of [[Coenwulf of Mercia]] (died 821). If this";

            // date of death over newline – too far away to be correct
            Assert.AreEqual(n9, Parsers.FixPeopleCategories(n9, "foo"));

            const string n10 = @"'''Fred''' blah
died 2002
{{persondata}}";

            Assert.AreEqual(n10, Parsers.FixPeopleCategories(n10, "foo"));

            // don't grab numbers out of long wikilinks
            const string n11 = @"'''Marcus Caecilius Metellus''' was a son of [[Lucius Caecilius Metellus (died 221 BC)|Lucius Caecilius Metellus]]. Deported {{persondata}}";

            Assert.AreEqual(n11, Parsers.FixPeopleCategories(n11, "foo"));

            // birth and death
            const string bd1 = @"''Foo''' (8 May 1920 - 11 June 2004) was {{persondata}}";

            const string bd1a = @"
[[Category:1920 births]]", bd1b = @"
[[Category:2004 deaths]]";

            Assert.AreEqual(bd1 + bd1a + bd1b, Parsers.FixPeopleCategories(bd1, "foo"));

            Assert.AreEqual(bd1 + bd1a + bd1b, Parsers.FixPeopleCategories(bd1 + bd1a, "foo"));

            Assert.AreEqual(bd1 + bd1b + bd1a, Parsers.FixPeopleCategories(bd1 + bd1b, "foo"));

            const string bd2 = @"''Foo''' (8 May 1920 – 11 June 2004) was {{persondata}}";
            Assert.AreEqual(bd2 + bd1a + bd1b, Parsers.FixPeopleCategories(bd2, "foo"));

            const string bd3 = @"'''Foo''' (8 May 1920 somewhere or other&ndash;11 June 2004) was {{persondata}}";
            Assert.AreEqual(bd3 + bd1a + bd1b, Parsers.FixPeopleCategories(bd3, "foo"));

            // approximate date check still applied
            string bd4 = @"''Foo''' (Circa 8 May 1920 – 11 June 2004) was {{persondata}}";
            Assert.AreEqual(bd4 + u + @"
[[Category:2004 deaths]]", Parsers.FixPeopleCategories(bd4, "foo"));

            const string bd5 = @"'''Autpert Ambrose (Ambroise)''' (ca. 730 – 784) {{persondata}}
[[Category:778 deaths]]";

            Assert.AreEqual(bd5 + u, Parsers.FixPeopleCategories(bd5, "foo"));

            const string bd6 = @"'''Saint Bruno of Querfurt''' (c. 970 – February 14 1009) (also known as ''Brun'' and ''Boniface''  {{persondata}}
[[Category:1009 deaths]]";

            Assert.AreEqual(bd6 + u, Parsers.FixPeopleCategories(bd6, "foo"));

            // all correct already
            const string bd7 = @"'''Agapetus II''' (born in [[Rome]]; died October, 955) was {{persondata}}
[[Category:955 deaths]]";

            Assert.AreEqual(bd7, Parsers.FixPeopleCategories(bd7, "foo"));

            // no change
            const string bd8 = @"'''Husain''' (d. April or May 1382) was a [[Jalayirids|Jalayirid]] ruler (1374-1382). He was the son of [[Shaikh Uvais]]. {{persondata}}
[[Category:1382 deaths]]";

            // add death and uncertain birth
            Assert.AreEqual(bd8, Parsers.FixPeopleCategories(bd8, "foo"));

            const string bd9 = @"'''Mannalargenna''' (ca. 1770-1835), a [[Tasmanian Aborigine]], was the chief of the Ben Lomond tribe (Plangermaireener). {{persondata}}";

            Assert.AreEqual(bd9 + @"
[[Category:Year of birth uncertain]]
[[Category:1835 deaths]]", Parsers.FixPeopleCategories(bd9, "foo"));

            const string bd10 = @"'''King Godfred''' (ruled 804 - 810) {{persondata}}";
            Assert.AreEqual(bd10, Parsers.FixPeopleCategories(bd10, "foo"));

            const string bd11 = @"'''Rabat I''' (1616/7 - 1644/5) {{persondata}}";

            Assert.AreEqual(bd11 + u, Parsers.FixPeopleCategories(bd11, "foo"));

            const string bd12 = @"'''Lorenzo Monaco''' (born  '''Piero di Giovanni''' [[Circa|c.]]1370-1425) {{persondata}}
[[Category:1425 deaths]]";

            Assert.AreEqual(bd12 + u, Parsers.FixPeopleCategories(bd12, "foo"));

            const string bd13 = @"'''Mocius''' ('''Mucius''', died 288-295), also kno
[[Category:3rd-century deaths]]";

            Assert.AreEqual(bd13, Parsers.FixPeopleCategories(bd13, "foo"));

            // with flourit both dates are uncertain
            const string bd14 = @"'''Asclepigenia''' (fl. 430  – 485 AD) was {{persondata}}";
            Assert.AreEqual(bd14, Parsers.FixPeopleCategories(bd14, "foo"));

            const string bd14a = @"'''Asclepigenia''' ([[floruit|fl]]. 430  – 485 AD) was {{persondata}}";
            Assert.AreEqual(bd14a, Parsers.FixPeopleCategories(bd14a, "foo"));

            const string bd14b = @"'''Asclepigenia''' (flourished 430  – 485 AD) was {{persondata}}";
            Assert.AreEqual(bd14b, Parsers.FixPeopleCategories(bd14b, "foo"));

            // no data to use here
            const string no1 = @"'''Bahram I''' (also spelled ''Varahran'' or ''Vahram'', ''r.'' 273&ndash;276) {{persondata}}";
            Assert.AreEqual(no1, Parsers.FixPeopleCategories(no1, "foo"));

            const string no2 = @"'''Trebellianus''' (d. 260-268), also {{persondata}}";
            Assert.AreEqual(no2, Parsers.FixPeopleCategories(no2, "foo"));

            const string no3 = @"'''[[Grand Ayatollah]] {{unicode|Muhammad Sādiq as-Sadr}}''' ([[Arabic]] محمّد صادق الصدر ) is an [[Iraq]]i [[Twelver]] [[Shi'a]] cleric of high rank. He is the father of [[Muqtada al-Sadr]] (born 1973). Sometimes the son is called by his father's name. He is the cousin of Grand Ayatollah [[Muhammad Baqir al-Sadr]] (died 1980). The al-Sadr family are considered [[Sayyid]], which is used among the Shia to denote persons descending directly from [[Muhammad]]. The family's lineage is traced through Imam [[Jafar al-Sadiq]] and his son Imam [[Musa al-Kazim]] the sixth and seventh Shia Imams respectively.";
            Assert.AreEqual(no3, Parsers.FixPeopleCategories(no3, "foo"));

            const string no4 = @"'''Bahram I''' (also spelled ''Varahran'' or ''Vahram'', ''r.'' 273&ndash;276) {{persondata}}";
            Assert.AreEqual(no4, Parsers.FixPeopleCategories(no4, "foo"));

            const string no5 = @"'''John Coggeshall''' (chr. December 9, 1601) Charles
[[Category:1835 deaths]]";
            Assert.AreEqual(no5, Parsers.FixPeopleCategories(no5, "foo"));

            const string ISO1 = @"'''Ben Moon''' (born [[1960-06-13]]) is a {{persondata}}";
            Assert.AreEqual(ISO1 + "\r\n" + b2, Parsers.FixPeopleCategories(ISO1, "foo"));

            const string bd15 = @"'''Kristina of Norway''' (born in [[1234]] in [[Bergen]] &ndash; circa [[1262]]), sometimes {{persondata}}";
            Assert.AreEqual(bd15 + @"
[[Category:1234 births]]", Parsers.FixPeopleCategories(bd15, "foo"));

            // sortkey usage
            const string s1 = @"'''Claire Hazel Weekes''' (1903&mdash;1990) was {{persondata}}
[[Category:1990 deaths|Weekes, Claire]]";

            Assert.AreEqual(s1 + @"
[[Category:1903 births|Weekes, Claire]]", Parsers.FixPeopleCategories(s1, "foo"));

            const string m1 = @"'''Hans G Helms''' (born [[8 June]] [[1960]] in [[Teterow]]; full name: ''Hans Günter Helms''; the bearer of the name does not use a full stop after the initial for his middle name) is a [[Germany|German]] experimental writer, composer, and social and economic analyst and critic. {{persondata}}";

            Assert.AreEqual(m1 + "\r\n" + b2, Parsers.FixPeopleCategories(m1, "foo"));

            // uncertain year of death
            const string m2 = @"'''Arthur Paunzen''' (born [[4 February]] [[1890]], died ?[[9 August]] [[1940]])
[[Category:1890 births]]";

            Assert.AreEqual(m2, Parsers.FixPeopleCategories(m2, "foo"));

            const string m3 = @"Foo {{death date and age|2008|8|21|1942|7|13|mf=y}} {{persondata}}";
            const string m3a = @"
[[Category:1942 births]]
[[Category:2008 deaths]]";

            Assert.AreEqual(m3 + m3a, Parsers.FixPeopleCategories(m3, "foo"));

            const string bug1 = @"'''Jane Borghesi''' (born 17 June{{Fact|date=January 2009}}, [[Melbourne]]) {{persondata}}";
            Assert.AreEqual(bug1, Parsers.FixPeopleCategories(bug1, "foo"));

            const string miss1 = @"'''Alonza J. White''' (ca 1836 &ndash; [[August 29]] [[1912]]) {{persondata}}
{{DEFAULTSORT:White, Alonza J}}
[[Category:1912 deaths]]
[[Category:People from St. John's, Newfoundland and Labrador]]", miss2 = @"[[Category:Year of birth missing]]";

            Assert.AreEqual(miss1 + u, Parsers.FixPeopleCategories(miss1 + "\r\n" + miss2, "foo"));
            
            const string both1 = @"'''Mary Ellen Wilson''' (1864–1956)<ref name=""amhum"">{{foo}}</ref> {{persondata}}", both2 = @"[[Category:1864 births]]
[[Category:1956 deaths]]";

            Assert.AreEqual(both1 + "\r\n" + both2, Parsers.FixPeopleCategories(both1, "foo"));

            const string bug2 = @"'''Foo''' (born {{circa}} 1925) was {{persondata}}";
            Assert.AreEqual(bug2 + u, Parsers.FixPeopleCategories(bug2, "foo"));

            const string bug3 = @"'''Joshua William Allen, 6th Viscount Allen''' [[Master of Arts (Oxbridge)|MA]] ({{circa}} [[1782]]–[[21 September]] [[1845]]) was an [[Irish peerage|Irish peer]].

{{DEFAULTSORT:Allen, Joshua Allen, 6th Viscount}}
[[Category:1845 deaths]]
[[Category:Viscounts in the Peerage of Ireland]]
[[Category:Year of birth uncertain]]";

            Assert.AreEqual(bug3, Parsers.FixPeopleCategories(bug3, "foo"));

            // infobox scraping
            const string infob1 = @"{{Infobox Officeholder
|honorific-prefix   =
|name            = John C. Zimmerman, Sr.
|term_start       = 1895
|term_end         = 1896
|predecessor      = [[Arthur C. McCall]]
|successor        = [[Samuel C. Randall]]
|birth_date      = May 12, 1835
|birth_place     = [[Free City of Frankfurt]]
|death_date= October 26, 1935
|death_place=
|restingplace = Glenwood Cemetery, Flint
|restingplacecoordinates =
|alma_mater      =
|occupation      =brickmason, mercha

}} {{persondata}}", infob2 = @"{{Infobox Officeholder
|honorific-prefix   =
|name            = John Footus
|term_start       = 144 BCE
|term_end         = 127 BCE
|predecessor      = [[Arthur C. McCall]]
|successor        = [[Samuel C. Randall]]
|birth_date      = 193 BC
|birth_place     = [[Free City of Frankfurt]]
|death_date= 127 BCE
|death_place=
|alma_mater      =
|occupation      =brickmason, mercha

}} {{persondata}}", infob1a = @"{{Infobox Officeholder
|honorific-prefix   =
|name            = John C. Zimmerman, Sr.
|term_start       = 1895
|term_end         = 1896
|predecessor      = [[Arthur C. McCall]]
|successor        = [[Samuel C. Randall]]
|dateofbirth     = May 12, 1835
|birth_place     = [[Free City of Frankfurt]]
|dateofdeath= October 26, 1935
|death_place=
|restingplace = Glenwood Cemetery, Flint
|restingplacecoordinates =
|alma_mater      =
|occupation      =brickmason, mercha

}} {{persondata}}";

            // scraped from infobox
            Assert.AreEqual(infob1 + @"
[[Category:1835 births]]
[[Category:1935 deaths]]", Parsers.FixPeopleCategories(infob1, "foo"));

            Assert.AreEqual(infob1a + @"
[[Category:1835 births]]
[[Category:1935 deaths]]", Parsers.FixPeopleCategories(infob1a, "foo"));

            Assert.AreEqual(infob2 + @"
[[Category:193 BC births]]
[[Category:127 BC deaths]]", Parsers.FixPeopleCategories(infob2, "foo"));

            // doesn't add twice
            Assert.AreEqual(infob1 + @"
[[Category:1835 births]]
[[Category:1935 deaths]]", Parsers.FixPeopleCategories(infob1 + @"
[[Category:1835 births]]
[[Category:1935 deaths]]", "foo"));

            // just decade no good
            const string infob3 = @"{{Infobox Officeholder
|honorific-prefix   =
|name            = John Foo
|predecessor      = [[Arthur C. McCall]]
|successor        = [[Samuel C. Randall]]
|birth_date      = 1970s
|birth_place     = [[Free City of Frankfurt]]
|death_date=
|death_place=
|alma_mater      =
|occupation      =brickmason, mercha

}} {{persondata}}";
            Assert.AreEqual(infob3, Parsers.FixPeopleCategories(infob3, "foo"));

            string unc1 = @"'''Aaron Walden''' (born at [[Warsaw]] about 1835, died 1912) was a Polish Jewish [[Talmudist]], editor, and author.
{{DEFAULTSORT:Walden, Aaron}}
[[Category:Polish Jews]]
[[Category:1912 deaths]]
[[Category:Year of birth uncertain]]";

            Assert.AreEqual(unc1, Parsers.FixPeopleCategories(unc1, "Aaron Walden"));
            
            // too many refs for it to be plausible that the cats are missing
            const string Refs = @"<ref>a</ref> <ref>a</ref> <ref>a</ref> <ref>a</ref> <ref>a</ref> <ref>a</ref> <ref>a</ref>";
            Assert.AreEqual(a1 + Refs + Refs + Refs, Parsers.FixPeopleCategories(a1 + Refs + Refs + Refs, "foo"));
        }
        
        [Test]
        public void FixPeopleCategoriesFutureTest()
        {
            // birth
            const string a1 = @"'''Fred Smith''' (born 2060) is a bloke. {{Persondata}}";
            Assert.AreEqual(a1, Parsers.FixPeopleCategories(a1, "foo"));
        }

        [Test]
        public void GetInfoBoxFieldValue()
        {
            Assert.AreEqual(@"1990", Parsers.GetInfoBoxFieldValue(@"hello {{infobox foo
|year=1990
|other=great}} now", @"[Yy]ear"));

            Assert.AreEqual(@"1990", Parsers.GetInfoBoxFieldValue(@"hello {{infobox foo
|  year  =  1990
|other=great}} now", @"[Yy]ear"));

            Assert.AreEqual(@"1990", Parsers.GetInfoBoxFieldValue(@"hello {{infobox foo
|  Year  =  1990
|other=great}} now", @"[Yy]ear"));

            // no infobox
            Assert.AreEqual(@"", Parsers.GetInfoBoxFieldValue(@"hello now", @"[Yy]ear"));
            
            // field not found
            Assert.AreEqual(@"", Parsers.GetInfoBoxFieldValue(@"hello {{infobox foo
|  Year  =  1990
|other=great}} now", @"[Yy]early"));

            // multiple fields on same line
            Assert.AreEqual(@"", Parsers.GetInfoBoxFieldValue(@"hello {{infobox foo
|  Year  =  1990  |some=where
|other=great}} now", @"[Yy]ear"));
            
            // invalid regex caught
            Assert.AreEqual(@"", Parsers.GetInfoBoxFieldValue(@"hello {{infobox foo
|  Year  =  1990  |some=where
|other=great}} now", @"(Yy]ear"));
        }

        [Test]
        public void YearOfBirthMissingCategory()
        {
            Assert.AreEqual(@"[[Category:Pakistani lawyers]]
[[Category:Attorneys General of Pakistan]]
[[Category:Living people]]
[[Category:1944 births]]", Parsers.FixPeopleCategories(@"[[Category:Pakistani lawyers]]
[[Category:Attorneys General of Pakistan]]
[[Category:Year of birth missing (living people)]]
[[Category:Living people]]
[[Category:1944 births]]", "foo"));

            Assert.AreEqual(@"[[Category:Pakistani lawyers]]
[[Category:Attorneys General of Pakistan]]
[[Category:Living people]]
[[Category:1944 births]]", Parsers.FixPeopleCategories(@"[[Category:Pakistani lawyers]]
[[Category:Attorneys General of Pakistan]]
[[Category:Year of birth missing]]
[[Category:Living people]]
[[Category:1944 births]]", "foo"));

            // no change when already correct
            Assert.AreEqual(@"[[Category:Pakistani lawyers]]
[[Category:Attorneys General of Pakistan]]
[[Category:Living people]]
[[Category:1944 births]]", Parsers.FixPeopleCategories(@"[[Category:Pakistani lawyers]]
[[Category:Attorneys General of Pakistan]]
[[Category:Living people]]
[[Category:1944 births]]", "foo"));
            
            const string a = @"Mr foo {{persondata}} was great
[[Category:1960 deaths]]";
            Assert.AreEqual(a+ "\r\n", Parsers.FixPeopleCategories(a + "\r\n" + @"[[Category:Year of death missing]]", "test"));

        }

        [Test]
        public void GetCategorySortTests()
        {
            Assert.AreEqual(@"", Parsers.GetCategorySort(@"'''Jonathan Sothcott''' (born [[26 April]] [[1980]]) {{persondata}}
{{DEFAULTSORT:Sothcott, Jonathan}}
[[Category:British television producers|Sothcott, Jonathon]]"));

            Assert.AreEqual(@"Sothcott, Jonathan", Parsers.GetCategorySort(@"'''Jonathan Sothcott''' (born [[26 April]] [[1980]]) {{persondata}}
[[Category:British television producers|Sothcott, Jonathan]]
[[Category:Living people|Sothcott, Jonathan]]"));

            Assert.AreEqual(@"Sothcott, Jonathan", Parsers.GetCategorySort(@"'''Jonathan Sothcott''' (born [[26 April]] [[1980]]) {{persondata}}
[[Category:Living people|Sothcott, Jonathan]]"));

            // not all same – null return
            Assert.AreEqual(@"", Parsers.GetCategorySort(@"'''Jonathan Sothcott''' (born [[26 April]] [[1980]]) {{persondata}}
[[Category:British television producers|Sothcott, Jonathan]]
[[Category:Living people|Sothcott, Jonathan]]
[[Category:1944 births]]"));
        }

        [Test]
        public void CategoryMatch()
        {
            Assert.IsTrue(Parsers.CategoryMatch(@"foo [[Category:1990 births]]", @"1990 births"));
            Assert.IsTrue(Parsers.CategoryMatch(@"foo [[Category:1990 births ]]", @"1990 births"));
            Assert.IsTrue(Parsers.CategoryMatch(@"foo [[Category: 1990 births  ]]", @"1990 births"));
            Assert.IsTrue(Parsers.CategoryMatch(@"foo [[Category:1990 Births]]", @"1990 births"));
            Assert.IsTrue(Parsers.CategoryMatch(@"foo [[Category:1990 births]]", @"1990 Births"));
            Assert.IsTrue(Parsers.CategoryMatch(@"foo [[Category:1990 births|foo]]", @"1990 births"));
            Assert.IsTrue(Parsers.CategoryMatch(@"foo [[Category:1990 births | foo]]", @"1990 births"));
            Assert.IsTrue(Parsers.CategoryMatch(@"foo [[Category:Year of birth missing|Foo, bar]]", "Year of birth missing"));

            Assert.IsFalse(Parsers.CategoryMatch(@"foo [[Category:1990 births]]", @"1990"));
        }

        [Test]
        public void GetTemplateNameTests()
        {
            WikiRegexes.TemplateCall = new Regex(@"{{Template:\s*([^\]\|]*)\s*(.*)}}", RegexOptions.Singleline);

            Assert.AreEqual(@"foo", Parsers.GetTemplateName(@"{{Template:foo|bar}}"));
            Assert.AreEqual(@"Foo", Parsers.GetTemplateName(@"{{Template:Foo|bar}}"));
            Assert.AreEqual(@"foo", Parsers.GetTemplateName(@"{{Template:    foo|bar}}"));
            Assert.AreEqual(@"foo here", Parsers.GetTemplateName(@"{{Template:    foo here|bar}}"));
            Assert.AreEqual(@"foo-bar", Parsers.GetTemplateName(@"{{Template:    foo-bar}}"));
        }

        [Test]
        public void RemoveDuplicateWikiLinks()
        {
            // removes duplicate piped wikilinks on same line
            Assert.AreEqual(@"now [[foo|bar]] was bar too", Parsers.RemoveDuplicateWikiLinks(@"now [[foo|bar]] was [[foo|bar]] too"));
            Assert.AreEqual(@"now [[foo|bar]] was bars too", Parsers.RemoveDuplicateWikiLinks(@"now [[foo|bar]] was [[foo|bar]]s too"));

            // multiline – no change
            Assert.AreEqual(@"now [[foo|bar]]
was [[foo|bar]] too", Parsers.RemoveDuplicateWikiLinks(@"now [[foo|bar]]
was [[foo|bar]] too"));

            // case sensitive
            Assert.AreEqual(@"now [[foo|bar]] was [[Foo|bar]] too", Parsers.RemoveDuplicateWikiLinks(@"now [[foo|bar]] was [[Foo|bar]] too"));
            Assert.AreEqual(@"now [[foo bar]] was [[Foo bar]] too", Parsers.RemoveDuplicateWikiLinks(@"now [[foo bar]] was [[Foo bar]] too"));

            // removes duplicate unpiped wikilinks
            Assert.AreEqual(@"now [[foo bar]] was foo bar too", Parsers.RemoveDuplicateWikiLinks(@"now [[foo bar]] was [[foo bar]] too"));

            // no changes
            Assert.AreEqual(@"now [[foo|bar]] was [[foo]] too", Parsers.RemoveDuplicateWikiLinks(@"now [[foo|bar]] was [[foo]] too"));
        }

        [Test]
        public void CanonicalizeTitleRawTests()
        {
            Assert.AreEqual(@"foo bar", Parsers.CanonicalizeTitleRaw(@"foo_bar"));
            Assert.AreEqual(@"foo+bar", Parsers.CanonicalizeTitleRaw(@"foo+bar"));
            Assert.AreEqual(@"foo bar", Parsers.CanonicalizeTitleRaw(@"foo_bar", false));
            Assert.AreEqual(@"foo bar ", Parsers.CanonicalizeTitleRaw(@"foo_bar ", false));
            Assert.AreEqual(@"foo bar", Parsers.CanonicalizeTitleRaw(@"foo_bar", true));
            Assert.AreEqual(@"foo bar", Parsers.CanonicalizeTitleRaw(@"foo_bar ", true));
            Assert.AreEqual(@"foo bar", Parsers.CanonicalizeTitleRaw(@" foo_bar", true));

            Assert.AreEqual(@"Bugs#If a selflink is also bolded, AWB should", Parsers.CanonicalizeTitleRaw(@"Bugs#If_a_selflink_is_also_bolded%2C_AWB_should"));
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
            const string b1 = @"'''Fred Smith''' is a bloke.";
            Assert.AreEqual(b1, Parsers.LivingPeople(b1));

            // no matches if identified as dead
            const string a = @"'''Fred Smith''' (born 1960) is a bloke.
[[Category:1960 births]]
[[Category:1990 deaths]]";
            const string b = @"'''Fred Smith''' is a bloke.
[[Category:1960 births]]
[[Category:1990 deaths]]";
            const string c = @"'''Fred Smith''' is a bloke.
[[Category:1960 births|Smith, Fred]]
[[Category:1990 deaths|Smith, Fred]]";
            const string d = @"'''Fred Smith''' (born 11 May 1950 - died 17 August 1990) is a bloke.
[[Category:1960 births|Smith, Fred]]";
            const string e = @"'''Fred Smith''' (born 1950) is a bloke.
[[Category:1950 births|Smith, Fred]]
{{Recentlydeceased}}";
            const string f = @"'''Fred Smith''' (born 1950) is a bloke.
[[Category:1950 births|Smith, Fred]]
{{recentlydeceased}}";
            const string g = @"'''Fred Smith''' (born 1950) is a bloke.
[[Category:1950 births|Smith, Fred]]
[[Category:Year of death missing]]";
            const string h = @"'''Fred Smith''' (d. 1950) is a bloke.";

            Assert.AreEqual(a, Parsers.LivingPeople(a));
            Assert.AreEqual(b, Parsers.LivingPeople(b));
            Assert.AreEqual(c, Parsers.LivingPeople(c));
            Assert.AreEqual(d, Parsers.LivingPeople(d));
            Assert.AreEqual(e, Parsers.LivingPeople(e));
            Assert.AreEqual(f, Parsers.LivingPeople(f));
            Assert.AreEqual(g, Parsers.LivingPeople(g));
            Assert.AreEqual(h, Parsers.LivingPeople(h));

            // assume dead if born earlier than 121 years ago, so no change
            const string d1 = @"'''Fred Smith''' (born 1879) is a bloke.
[[Category:1879 births|Smith, Fred]]";
            Assert.AreEqual(d1, Parsers.LivingPeople(d1));

            // check correctly handles birth category with no year to parse
            const string d2 = @"Fred [[Category:15th-century births]]";
            Assert.AreEqual(d2, Parsers.LivingPeople(d2));

            // ignores if {{lifetime}} present

            const string d3 = @"Fred [[Category:1961 births]]
{{Lifetime|1961||Clopatofsky Ghisays, Jairo}}";
            Assert.AreEqual(d3, Parsers.LivingPeople(d3));
        }

        [Test]
        public void UnbalancedBracketsTests()
        {
            int bracketLength = 0;
            Assert.AreEqual(18, Parsers.UnbalancedBrackets(@"now hello {{bye}} {{now}", ref bracketLength));
            Assert.AreEqual(2, bracketLength);
            Assert.AreEqual(18, Parsers.UnbalancedBrackets(@"now hello {{bye}} {{now} abc", ref bracketLength));
            Assert.AreEqual(2, bracketLength);
            Assert.AreEqual(18, Parsers.UnbalancedBrackets(@"now hello {{bye}} [[now]", ref bracketLength));
            Assert.AreEqual(2, bracketLength);
            Assert.AreEqual(18, Parsers.UnbalancedBrackets(@"now hello [[bye]] {{now}", ref bracketLength));
            Assert.AreEqual(2, bracketLength);
            Assert.AreEqual(18, Parsers.UnbalancedBrackets(@"now hello {{bye}} [now words [here]", ref bracketLength));
            Assert.AreEqual(1, bracketLength);
            Assert.AreEqual(21, Parsers.UnbalancedBrackets(@"now hello {{bye}} now] words [here]", ref bracketLength));
            Assert.AreEqual(1, bracketLength);
            Assert.AreEqual(22, Parsers.UnbalancedBrackets(@"now hello {{bye}} {now}}", ref bracketLength));
            Assert.AreEqual(2, bracketLength);
            Assert.AreEqual(33, Parsers.UnbalancedBrackets(@"[http://www.site.com a link [cool]]", ref bracketLength)); // FixSyntax replaces with &#93;
            Assert.AreEqual(2, bracketLength);
            Assert.AreEqual(18, Parsers.UnbalancedBrackets(@"now hello {{bye}} {now", ref bracketLength));
            Assert.AreEqual(1, bracketLength);

            // only first reported
            Assert.AreEqual(18, Parsers.UnbalancedBrackets(@"now hello {{bye}} {{now} or {{now} was", ref bracketLength));
            Assert.AreEqual(18, Parsers.UnbalancedBrackets(@"now hello {{bye}} {{now} or [[now] was", ref bracketLength));

            // brackets all okay
            Assert.AreEqual(-1, Parsers.UnbalancedBrackets(@"now hello {{bye}} {{now}}", ref bracketLength));
            Assert.AreEqual(-1, Parsers.UnbalancedBrackets(@"now hello [[bye]] {{now}}", ref bracketLength));
            Assert.AreEqual(-1, Parsers.UnbalancedBrackets(@"now hello {{bye}} [now]", ref bracketLength));
            Assert.AreEqual(-1, Parsers.UnbalancedBrackets(@"now hello", ref bracketLength));
            Assert.AreEqual(-1, Parsers.UnbalancedBrackets(@"<ref>[http://www.pubmedcentral.nih.gov/articlerender.fcgi?artid=32159 Message to
complementary and alternative medicine: evidence is a better friend than power. Andrew J Vickers]</ref>", ref bracketLength));
            Assert.AreEqual(-1, Parsers.UnbalancedBrackets(@"[http://www.site.com a link [cool&#93;]", ref bracketLength)); // displays as valid syntax

            // don't consider stuff in <math> or <pre> tags etc.
            Assert.AreEqual(-1, Parsers.UnbalancedBrackets(@"now hello {{bye}} <pre>{now}}</pre>", ref bracketLength));
            Assert.AreEqual(-1, Parsers.UnbalancedBrackets(@"now hello {{bye}} <math>{a{b}}</math>", ref bracketLength));
            Assert.AreEqual(-1, Parsers.UnbalancedBrackets(@"now hello {{bye}} <code>{now}}</code>", ref bracketLength));

            // unbalanced tags
            Assert.AreEqual(15, Parsers.UnbalancedBrackets(@"now <b>hello /b>", ref bracketLength));
            Assert.AreEqual(1, bracketLength);

            Assert.AreEqual(27, Parsers.UnbalancedBrackets(@"<a>asdf</a> now <b>hello /b>", ref bracketLength));
            Assert.AreEqual(1, bracketLength);
        }

        [Test]
        public void TestCiteFormatFieldTypo()
        {
            Assert.AreEqual(@"now {{cite web| url=a.com|title=hello|format=PDF}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=hello|fprmat=PDF}} was"));
            Assert.AreEqual(@"now {{cite web| url=a.com|title=hello| format=PDF}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=hello| fprmat=PDF}} was"));
            Assert.AreEqual(@"now {{cite web| url=a.com|title=hello|format = PDF}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=hello|Fprmat = PDF}} was"));
            Assert.AreEqual(@"now {{Cite web| url=a.com|title=hello|format=DOC}} was", Parsers.FixCitationTemplates(@"now {{Cite web| url=a.com|title=hello|fprmat=DOC}} was"));
            Assert.AreEqual(@"now {{Cite web| url=a.com|title=hello|
          format=DOC|publisher=BBC}} was", Parsers.FixCitationTemplates(@"now {{Cite web| url=a.com|title=hello|
          fprmat=DOC|publisher=BBC}} was"));

            // no Matches
            Assert.AreEqual(@"now {{cite web| url=http://site.net|title=okay}} fprmat of PDF", Parsers.FixCitationTemplates(@"now {{cite web| url=http://site.net|title=okay}} fprmat of PDF"));
            Assert.AreEqual(@"now {{cite web| url=http://site.net/1.pdf|format=PDF}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=http://site.net/1.pdf|format=PDF}} was"));
            Assert.AreEqual(@"now {{cite web|\r\nurl=http://site.net/1.pdf|format=PDF|title=okay}} was", Parsers.FixCitationTemplates(@"now {{cite web|\r\nurl=http://site.net/1.pdf|format=PDF|title=okay}} was"));
        }

        [Test]
        public void FixCitationGoogleBooks()
        {
            string correct = @"now {{cite book|title=a |url=http://books.google.com/foo | year=2009}}";
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"now {{cite web|title=a |url=http://books.google.com/foo | year=2009}}"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"now {{ cite web|title=a |url=http://books.google.com/foo | year=2009}}"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"now {{citeweb|title=a |url=http://books.google.com/foo | year=2009}}"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"now {{Cite web|title=a |url=http://books.google.com/foo | year=2009}}"));
        }
        
        [Test]
        public void FixCitationDupeFields()
        {
            string correct = @"{{cite web|url=a |title=b |date=2008 | accessdate=11 May 2008}}";
            string correct2 = @"{{cite web|url=a |title=b |date=2008 | accessdate=11 May 2008|work=here}}";
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |date=2008 | accessdate=11 May 2008|date=2008}}"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |date=2008 | accessdate=11 May 2008|  date = 2008  }}"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |date=2008 | accessdate=11 May 2008|date=200}}"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |date=2008 | accessdate=11 May 2008|date= }}"));
            
            Assert.AreEqual(correct2, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |date=2008 | accessdate=11 May 2008|date=2008|work=here}}"));
            Assert.AreEqual(correct2, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |date=2008 | accessdate=11 May 2008|  date = 2008  |work=here}}"));
            Assert.AreEqual(correct2, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |date=2008 | accessdate=11 May 2008|date=200|work=here}}"));
            Assert.AreEqual(correct2, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |date=2008 | accessdate=11 May 2008|date= |work=here}}"));
            
            string correct3 = @"{{cite web|url=a |title=b |date=2008 | accessdate=11 May 2008|work=here there}}";
            Assert.AreEqual(correct3, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |date=2008 | accessdate=11 May 2008|work=here there|work=here}}"));
            Assert.AreEqual(correct3, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |date=2008 | accessdate=11 May 2008|work=here|work=here there}}"));
            Assert.AreEqual(correct3, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |date=2008 | accessdate=11 May 2008|work=here there|work = here }}"));
            Assert.AreEqual(correct3, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |date=2008 | accessdate=11 May 2008|work=here there|work=here th}}"));
            Assert.AreEqual(correct3, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |date=2008 | accessdate=11 May 2008|work=here there|work=here there}}"));
            
            string nochange1 = @"{{cite web|url=a |title=b |date=2008 | accessdate=11 May 2008|date=2004}}";
            Assert.AreEqual(nochange1, Parsers.FixCitationTemplates(nochange1));
            
            string nochange2 = @"{{cite web|url=http://www.tise.com/abc|date=2008|page.php=7 |title=b |date=2008 | accessdate=11 May 2008}}";
            Assert.AreEqual(nochange2, Parsers.FixCitationTemplates(nochange2));
            
            string nochange3 = @"{{cite book|title=b |date=2008 | accessdate=11 May 2008|date=2004}}";
            Assert.AreEqual(nochange3, Parsers.FixCitationTemplates(nochange3));
            
            // null fields
            Assert.AreEqual(@"now {{cite web| url=http://site.net |accessdate = 2008-10-08|title=hello}}", Parsers.FixCitationTemplates(@"now {{cite web| url=http://site.net |title=|accessdate = 2008-10-08|title=hello}}"));
            Assert.AreEqual(@"now {{cite web| url=http://site.net |accessdate = 2008-10-08|title=hello|publisher=BBC}}", Parsers.FixCitationTemplates(@"now {{cite web| url=http://site.net |title=|accessdate = 2008-10-08|title=hello|publisher=BBC}}"));
            Assert.AreEqual(@"now {{Cite web| title = hello | url=http://site.net |accessdate = 2008-10-08}}", Parsers.FixCitationTemplates(@"now {{Cite web| title = hello | url=http://site.net |accessdate = 2008-10-08|title=}}"));
            Assert.AreEqual(@"now {{cite web| url=http://site.net |accessdate = 2008-10-08| title = hello }}", Parsers.FixCitationTemplates(@"now {{cite web| url=http://site.net |title=|accessdate = 2008-10-08| title = hello }}"));
            Assert.AreEqual(@"now {{cite web| title=hello|url=http://site.net |date = 2008-10-08}}", Parsers.FixCitationTemplates(@"now {{cite web| title=hello|title=|url=http://site.net |date = 2008-10-08}}"));
            Assert.AreEqual(@"now {{cite web| url=http://site.net |title=hello|first=|accessdate = 2008-10-08}}", Parsers.FixCitationTemplates(@"now {{cite web| url=http://site.net |title=hello|first=|accessdate = 2008-10-08|first=}}"));

            //no Matches
            Assert.AreEqual(@"now {{cite web| title=hello|url=http://site.net |date = 2008-10-08|title=HELLO}}", Parsers.FixCitationTemplates(@"now {{cite web| title=hello|url=http://site.net |date = 2008-10-08|title=HELLO}}")); // case of field value different
            Assert.AreEqual(@"now {{cite web| url=http://site.net |title=hello|accessdate = 2008-10-08|name=hello}}", Parsers.FixCitationTemplates(@"now {{cite web| url=http://site.net |title=hello|accessdate = 2008-10-08|name=hello}}"));
        }
        
        [Test]
        public void MergeCiteWebAccessDateYear()
        {
            string correct = @"{{cite web|url=a |title=b |date=2008 | accessdate=11 May 2008}}";
            string correct2 = @"{{cite web|url=a |title=b |date=2008 | accessdate=May 11, 2008}}";
            string correct3 = @"{{cite web|url=a |title=b |date=2008 | accessdate=11 May 2008 |work=c}}";
            string correct4 = @"{{cite book|url=a |title=b |date=2008 | accessdate=11 May 2008 |work=c}}";
            
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |date=2008 | accessdate=11 May| accessyear=2008}}"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |date=2008 | accessdate=11 May| accessyear = 2008  }}"));
            Assert.AreEqual(correct2, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |date=2008 | accessdate=May 11| accessyear=2008}}"));
            Assert.AreEqual(correct2, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |date=2008 | accessdate=May 11| accessyear = 2008  }}"));
            Assert.AreEqual(correct3, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |date=2008 | accessdate=11 May | accessyear = 2008 |work=c}}"));
            Assert.AreEqual(correct4, Parsers.FixCitationTemplates(@"{{cite book|url=a |title=b |date=2008 | accessdate=11 May | accessyear = 2008 |work=c}}"));
            
            // only for cite web
            string nochange2 = @"{{cite news|url=a |title=b |date=2008 | accessdate=11 May |accessyear=2008 |work=c}}";
            Assert.AreEqual(nochange2, Parsers.FixCitationTemplates(nochange2));
        }
        
        [Test]
        public void AccessDayMonthDay()
        {
            string a = @"{{cite web|url=a |title=b |date=2008 | accessdate=11 May 2008}}";
            Assert.AreEqual(a, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |date=2008 | accessdate=11 May 2008|accessdaymonth=   }}"));
            Assert.AreEqual(a, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |date=2008 | accessdate=11 May 2008|accessmonthday  =   }}"));
            Assert.AreEqual(a, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |date=2008 |accessmonthday  = | accessdate=11 May 2008}}"));
        }
        
        [Test]
        public void AccessYear()
        {
            string a = @"{{cite web|url=a |title=b |date=2008 | accessdate=11 May 2008}}";
            Assert.AreEqual(a, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |date=2008 | accessdate=11 May 2008|accessyear=2008   }}"));
            Assert.AreEqual(a, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |date=2008 | accessdate=11 May 2008| accessyear =  2008   }}"));
            Assert.AreEqual(a, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |date=2008 |accessyear=2008   | accessdate=11 May 2008}}"));
        }
        
        [Test]
        public void DateLeadingZero()
        {
            string a = @"{{cite web|url=a |title=b |date=2008 | accessdate=1 May 2008}}";
            Assert.AreEqual(a, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |date=2008 | accessdate=01 May 2008}}"));
            
            string b = @"{{cite book|url=a |title=b | date=May 1, 2008}}";
            Assert.AreEqual(b, Parsers.FixCitationTemplates(@"{{cite book|url=a |title=b | date=May 01, 2008}}"));
            Assert.AreEqual(b, Parsers.FixCitationTemplates(@"{{cite book|url=a |title=b | date=May 01 2008}}"));
            
            string c = @"{{cite book|url=a |title=b | date=May 1, 2008|author=Lee}}";
            Assert.AreEqual(c, Parsers.FixCitationTemplates(@"{{cite book|url=a |title=b | date=May 01, 2008|author=Lee}}"));
            
            string d = @"{{cite book|url=a |title=b |date=2008 | date=May 1|author=Lee}}";
            Assert.AreEqual(d, Parsers.FixCitationTemplates(@"{{cite book|url=a |title=b |date=2008 | date=May 01|author=Lee}}"));
        }

        [Test]
        public void FixCitationTemplates()
        {
            Assert.AreEqual(@"{{cite web|title=foo|url=http://site.net|year=2009}}", Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009|format=HTML}}"));
            Assert.AreEqual(@"{{cite web|title=foo|url=http://site.net|year=2009}}", Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009|format=HTM}}"));
            Assert.AreEqual(@"{{cite web|title=foo|url=http://site.net|year=2009}}", Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009|format = HTML}}"));
            Assert.AreEqual(@"{{cite web|title=foo|url=http://site.net|year=2009}}", Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009| format  =HTML  }}"));
            Assert.AreEqual(@"{{cite web|title=foo|url=http://site.net|year=2009}}", Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009|     format=HTM}}"));
            Assert.AreEqual(@"{{Citation|title=foo|url=http://site.net|year=2009}}", Parsers.FixCitationTemplates(@"{{Citation|title=foo|url=http://site.net|format=HTML|year=2009}}"));
            Assert.AreEqual(@"{{cite web|title=foo|url=http://site.net|year=2009}}", Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009|format=[[HTML]]}}"));

            Assert.AreEqual(@"{{cite web|title=foo|url=http://site.net|year=2009}}", Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009|language=English}}"));
            Assert.AreEqual(@"{{cite web|title=foo|url=http://site.net|year=2009}}", Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009|language = English}}"));
            Assert.AreEqual(@"{{cite web|title=foo|url=http://site.net|year=2009}}", Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009|language=english}}"));

            // removal of null 'format=' when URL is to HTML
            Assert.AreEqual(@"{{cite web|title=foo|url=http://site.net|year=2009|format=}}", Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009|format=}}"));
            Assert.AreEqual(@"{{cite web|title=foo|url=http://site.net/a.htm|year=2009}}", Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net/a.htm|year=2009|format=}}"));
            Assert.AreEqual(@"{{cite web|title=foo|url=http://site.net/a.html|year=2009}}", Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net/a.html|year=2009|format=}}"));
            Assert.AreEqual(@"{{cite web|title=foo|url=http://site.net/a.HTML|year=2009}}", Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net/a.HTML|year=2009|format=}}"));
        }
        
        [Test]
        public void FixCitationTemplatesPagesPP()
        {
            // removal of pp. from 'pages' field
            Assert.AreEqual(@"{{cite book|author=Smith|title=Great|pages=57–59}}", Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|pages=pp. 57–59}}"));
            Assert.AreEqual(@"{{cite book|author=Smith|title=Great|pages=57–59}}", Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|pages=pp.57–59}}"));
            Assert.AreEqual(@"{{cite book|author=Smith|title=Great|pages=57–59}}", Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|pages=pp.&nbsp;57–59}}"));
            Assert.AreEqual(@"{{cite book|author=Smith|title=Great|pages= 57–59}}", Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|pages= pp. 57–59}}"));
            Assert.AreEqual(@"{{cite book|author=Smith|title=Great|pages=57–59|year=2007}}", Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|pages=pp. 57-59|year=2007}}"));
            Assert.AreEqual(@"{{cite book|author=Smith|title=Great|page=57}}", Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|page=p. 57}}"));
            Assert.AreEqual(@"{{cite book|author=Smith|title=Great|page=57}}", Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|page=p 57}}"));
            Assert.AreEqual(@"{{cite book|author=Smith|title=Great|page=57}}", Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|page=pp 57}}"));

            const string nochange0 = @"{{cite book|author=Smith|title=Great|page= 57}}", nochange1 = @"{{cite book|author=Smith|title=Great|page= para 57}}";
            Assert.AreEqual(nochange0, Parsers.FixCitationTemplates(nochange0));
            Assert.AreEqual(nochange1, Parsers.FixCitationTemplates(nochange1));

            // not when nopp
            Assert.AreEqual(@"{{cite book|author=Smith|title=Great|pages=pp. 57–59|nopp=yes}}", Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|pages=pp. 57-59|nopp=yes}}"));
            
            // not for cite journal
            const string journal = @"{{cite journal|author=Smith|title=Great|page=p.57}}";
            Assert.AreEqual(journal, Parsers.FixCitationTemplates(journal));
        }

        [Test]
        public void FixCitationTemplatesVolume()
        {
            const string correct = @"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume= 3|issue= 3|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ";
            Assert.AreEqual(correct,
                            Parsers.FixCitationTemplates(@"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume=Vol. 3|issue=No. 3|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ"));
            Assert.AreEqual(correct,
                            Parsers.FixCitationTemplates(@"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume=Volumes 3|issue=No. 3|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ"));

            Assert.AreEqual(correct,
                            Parsers.FixCitationTemplates(@"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume=vol. 3|issue=No. 3|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ"));

            Assert.AreEqual(correct,
                            Parsers.FixCitationTemplates(@"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume=volume 3|issue=Issue 3|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ"));
            
            Assert.AreEqual(correct,
                            Parsers.FixCitationTemplates(@"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume=volume 3|issue=Issues 3|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ"));

            Assert.AreEqual(correct,
                            Parsers.FixCitationTemplates(@"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume=volume 3|issue=Issue 3|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ"));

            Assert.AreEqual(@"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume= 3| issue = 3|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ",
                            Parsers.FixCitationTemplates(@"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume=volume 3 Issue 3|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ"));
            
            Assert.AreEqual(@"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume= 3| issue = 3–4|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ",
                            Parsers.FixCitationTemplates(@"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume=volume 3 numbers 3–4|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ"));

            string nochange1 = @"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume=special numbers 3–4|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}}";

            Assert.AreEqual(nochange1, Parsers.FixCitationTemplates(nochange1));
        }

        [Test]
        public void CiteTemplatesPageRange()
        {
            const string correct = @"{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu |volume= 3|issue= 3|pages = 140–148}}";

            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu |volume= 3|issue= 3|pages = 140-148}}")); // hyphen
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu |volume= 3|issue= 3|pages = 140   - 148}}")); // hyphen

            Assert.AreEqual(@"{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu |volume= 3|issue= 3| pages = 140–148}}", Parsers.FixCitationTemplates(@"{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu |volume= 3|issue= 3| pages = 140   - 148}}")); // hyphen
            
            // do not change page sections etc.
            const string nochange1 = @"{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu |volume= 3|issue= 3|pages = 140–7-2}}",
            // non-breaking hyphens to represent page sections rather than ranges
            nochange2 = @"{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu |volume= 3|issue= 3|pages = 140‑7}}",
            nochange3 = @"{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu |volume= 3|issue= 3|pages = 140&#8209;7}}";
            
            Assert.AreEqual(nochange1, Parsers.FixCitationTemplates(nochange1));
            Assert.AreEqual(nochange2, Parsers.FixCitationTemplates(nochange2));
            Assert.AreEqual(nochange3, Parsers.FixCitationTemplates(nochange3));
        }

        [Test]
        public void TestWordingIntoBareExternalLinks()
        {
            Assert.AreEqual(@"<ref>[http://www.nps.gov/history/nr/travel/cumberland/ber.htm B'er Chayim Temple, National Park Service]</ref>", Parsers.FixSyntax(@"<ref>B'er Chayim Temple, National Park Service, [ http://www.nps.gov/history/nr/travel/cumberland/ber.htm]</ref>"));

            // don't catch two bare links
            Assert.AreEqual(@"<ref>[http://www.nps.gov/history] [http://www.nps.gov/history/nr/travel/cumberland/ber.htm]</ref>", Parsers.FixSyntax(@"<ref>[http://www.nps.gov/history] [http://www.nps.gov/history/nr/travel/cumberland/ber.htm]</ref>"));
        }
        
        [Test]
        public void FixSyntaxExternalLinkBrackets()
        {
            Assert.AreEqual("[http://example.com] site", Parsers.FixSyntax("[http://example.com] site"));
            Assert.AreEqual("[http://example.com] site", Parsers.FixSyntax("[[http://example.com] site"));
            Assert.AreEqual("[http://example.com] site", Parsers.FixSyntax("[http://example.com]] site"));
            Assert.AreEqual("[http://example.com] site", Parsers.FixSyntax("[[http://example.com]] site"));

            Assert.AreEqual("[http://test.com]", Parsers.FixSyntax("[http://test.com]"));
            Assert.AreEqual("[http://test.com]", Parsers.FixSyntax("[http://http://test.com]"));
            Assert.AreEqual("[http://test.com]", Parsers.FixSyntax("[http://http://http://test.com]"));
            
            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_10#second_pair_of_brackets_added_to_https_links
            Assert.AreEqual("[https://example.com] site", Parsers.FixSyntax("[https://example.com]] site"));
            Assert.AreEqual("[https://example.com] site", Parsers.FixSyntax("[[https://example.com] site"));
        }
        
        [Test]
        public void FixSyntaxHTMLTags()
        {
            Assert.AreEqual("'''foo''' bar", Parsers.FixSyntax("<b>foo</b> bar"));
            Assert.AreEqual("'''foo''' bar", Parsers.FixSyntax("< b >foo</b> bar"));
            Assert.AreEqual("'''foo''' bar", Parsers.FixSyntax("<b>foo< /b > bar"));
            Assert.AreEqual("<b>foo<b> bar", Parsers.FixSyntax("<b>foo<b> bar"));

            Assert.AreEqual("''foo'' bar", Parsers.FixSyntax("<i>foo</i> bar"));
            Assert.AreEqual("''foo'' bar", Parsers.FixSyntax("< i >foo</i> bar"));
            Assert.AreEqual("''foo'' bar", Parsers.FixSyntax("< i >foo< / i   > bar"));
            Assert.AreEqual("''foo'' bar", Parsers.FixSyntax("<i>foo< /i > bar"));
            Assert.AreEqual("<i>foo<i> bar", Parsers.FixSyntax("<i>foo<i> bar"));
            
        }
        
        [Test]
        public void FixSyntaxSpacing()
        {
            Assert.AreEqual("[[Foo]]", Parsers.FixSyntax("[[Foo]]"));
            Assert.AreEqual("[[Foo Bar]]", Parsers.FixSyntax("[[Foo Bar]]"));
            Assert.AreEqual("[[Foo Bar]]", Parsers.FixSyntax("[[Foo  Bar]]"));
            Assert.AreEqual("[[Foo Bar|Bar]]", Parsers.FixSyntax("[[Foo  Bar|Bar]]"));
        }
        
        [Test]
        public void FixSyntaxPipesInExternalLinks()
        {
            Assert.AreEqual("[http://www.site.com ''my cool site'']", Parsers.FixSyntax("[http://www.site.com|''my cool site'']"));
            Assert.AreEqual("[http://www.site.com/here/there.html ''my cool site'']", Parsers.FixSyntax("[http://www.site.com/here/there.html|''my cool site'']"));

            Assert.AreEqual(@"port [http://www.atoc.org/general/ConnectingCommunitiesReport_S10.pdf ""Connecting Communities - Expanding Access to the Rail Network""] consid",
                            Parsers.FixSyntax(@"port [[http://www.atoc.org/general/ConnectingCommunitiesReport_S10.pdf |""Connecting Communities - Expanding Access to the Rail Network""]] consid"));
            
            const string nochange1 = @"[http://www.site.com|''my cool site''", nochange2 = @"{{Infobox Singapore School
| name = Yuan Ching Secondary School
| established = 1978
| city/town = [[Jurong]]
| enrolment = over 1,300
| homepage = [http://schools.moe.edu.sg/ycss/
| border_color = #330066
| uniform_color = #66CCFF
}}";
            Assert.AreEqual(nochange1, Parsers.FixSyntax(nochange1));
            Assert.AreEqual(nochange2, Parsers.FixSyntax(nochange2));
        }
        
        [Test]
        public void FixSyntaxHTTPFormat()
        {
            Assert.AreEqual("<ref>http://www.site.com</ref>", Parsers.FixSyntax(@"<ref>http//www.site.com</ref>"));
            Assert.AreEqual("at http://www.site.com", Parsers.FixSyntax(@"at http//www.site.com"));
            Assert.AreEqual("<ref>[http://www.site.com a website]</ref>",
                            Parsers.FixSyntax(@"<ref>[http:/www.site.com a website]</ref>"));
            Assert.AreEqual("*[http://www.site.com a website]", Parsers.FixSyntax(@"*[http//www.site.com a website]"));
            Assert.AreEqual("|url=http://www.site.com", Parsers.FixSyntax(@"|url=http//www.site.com"));
            Assert.AreEqual("|url = http://www.site.com", Parsers.FixSyntax(@"|url = http:/www.site.com"));
            Assert.AreEqual("[http://www.site.com]", Parsers.FixSyntax(@"[http/www.site.com]"));

            // these strings should not change
            string bug1 = @"now http://members.bib-arch.org/nph-proxy.pl/000000A/http/www.basarchive.org/bswbSearch was";
            Assert.AreEqual(bug1, Parsers.FixSyntax(bug1));

            string bug2 = @"now http://sunsite.utk.edu/math_archives/.http/contests/ was";
            Assert.AreEqual(bug2, Parsers.FixSyntax(bug2));

            Assert.AreEqual("the HTTP/0.9 was", Parsers.FixSyntax("the HTTP/0.9 was"));
            Assert.AreEqual("the HTTP/1.0 was", Parsers.FixSyntax("the HTTP/1.0 was"));
            Assert.AreEqual("the HTTP/1.1 was", Parsers.FixSyntax("the HTTP/1.1 was"));
            Assert.AreEqual("the HTTP/1.2 was", Parsers.FixSyntax("the HTTP/1.2 was"));

            string a = @"the HTTP/FTP was";
            Assert.AreEqual(a, Parsers.FixSyntax(a));

            Assert.AreEqual("the HTTP/1.2 protocol", Parsers.FixSyntax("the HTTP/1.2 protocol"));
            Assert.AreEqual(@"<ref>[http://cdiac.esd.ornl.gov/ftp/cdiac74/a.pdf chapter 5]</ref>",
                            Parsers.FixSyntax(@"<ref>[http://cdiac.esd.ornl.gov/ftp/cdiac74/a.pdf chapter 5]</ref>"));
        }

        [Test, Category("Incomplete")]
        public void TestFixSyntax()
        {
            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_3#NEsted_square_brackets_again.
            Assert.AreEqual("[[Image:foo.jpg|Some [http://some_crap.com]]]",
                            Parsers.FixSyntax("[[Image:foo.jpg|Some [http://some_crap.com]]]"));

            Assert.AreEqual("Image:foo.jpg|{{{some_crap}}}]]", Parsers.FixSyntax("Image:foo.jpg|{{{some_crap}}}]]"));

            Assert.AreEqual("[[somelink]]", Parsers.FixSyntax("[somelink]]"));
            Assert.AreEqual("[[somelink]]", Parsers.FixSyntax("[[somelink]"));
            Assert.AreNotEqual("[[somelink]]", Parsers.FixSyntax("[somelink]"));
            
            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_7#Erroneously_removing_pipe
            Assert.AreEqual("[[|foo]]", Parsers.FixSyntax("[[|foo]]"));

            bool noChange;
            //TODO: move it to parts testing specific functions, when they're covered
            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_4#Bug_encountered_when_perusing_Sonorous_Susurrus
            Parsers.CanonicalizeTitle("[[|foo]]"); // shouldn't throw exceptions
            Assert.AreEqual("[[|foo]]", Parsers.FixLinks("[[|foo]]", "bar", out noChange));

            Assert.AreEqual(@"[[foo|bar]]", Parsers.FixSyntax(@"[[foo||bar]]"));
        }
        
        [Test]
        public void FixLink()
        {
            bool nochange;
            Assert.AreEqual(@"[[Foo bar]]", Parsers.FixLinks(@"[[Foo_bar]]", "a", out nochange));
            Assert.IsFalse(nochange);
        }
        
        [Test]
        public void FixSyntaxFontTags()
        {
            Assert.AreEqual("hello", Parsers.FixSyntax(@"<font>hello</font>"));
            Assert.AreEqual("hello", Parsers.FixSyntax(@"<font>hello</FONT>"));
            Assert.AreEqual(@"hello
world", Parsers.FixSyntax(@"<font>hello
world</font>"));

            // only changing font tags without properties
            Assert.AreEqual(@"<font name=ab>hello</font>", Parsers.FixSyntax(@"<font name=ab>hello</font>"));
        }

        [Test]
        public void TestFixIncorrectBr()
        {
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"<br.>"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"<BR.>"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"<br\>"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"<BR\>"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"<\br>"));

            // these are already correct
            Assert.AreEqual("<br/>", Parsers.FixSyntax(@"<br/>"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"<br />"));
        }
        
        [Test]
        public void FixUnbalancedBracketsCiteTemplates()
        {
            Assert.AreEqual(@"<ref>{{cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref>{{cite web|url=a|title=b}</ref>"));
            Assert.AreEqual(@"<ref>{{cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref>{{cite web|url=a|title=b}]</ref>"));
            Assert.AreEqual(@"<ref>{{cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref>{{cite web|url=a|title=b))</ref>"));
            Assert.AreEqual(@"<ref> {{cite web|url=a|title=b}} </ref>", Parsers.FixSyntax(@"<ref> {{cite web|url=a|title=b} </ref>"));
            Assert.AreEqual(@"<ref name=Fred>{{cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref name=Fred>{{cite web|url=a|title=b}</ref>"));
            Assert.AreEqual(@"<ref name=""Fred"">{{cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref name=""Fred"">{{cite web|url=a|title=b}</ref>"));
            
            Assert.AreEqual(@"<ref>{{cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref>{cite web|url=a|title=b}}</ref>"));
            Assert.AreEqual(@"<ref> {{cite web|url=a|title=b}} </ref>", Parsers.FixSyntax(@"<ref> {cite web|url=a|title=b}} </ref>"));
            Assert.AreEqual(@"<ref name=Fred>{{Cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref name=Fred>{Cite web|url=a|title=b}}</ref>"));
            Assert.AreEqual(@"<ref name=""Fred"">{{cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref name=""Fred"">{cite web|url=a|title=b}}</ref>"));
            Assert.AreEqual(@"<ref>{{citation|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref>{citation|url=a|title=b}}</ref>"));
            
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
            
            Assert.AreEqual(@"<ref>{{cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref>cite web|url=a|title=b}}</ref>"));
            Assert.AreEqual(@"<ref name=""Smith"">{{cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref name=""Smith"">cite web|url=a|title=b}}</ref>"));
            Assert.AreEqual(@"<ref name=""Smith"">{{cite web|
url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref name=""Smith"">cite web|
url=a|title=b}}</ref>"));
            Assert.AreEqual(@"<ref>{{cite book|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref>cite book|url=a|title=b}}</ref>"));
            Assert.AreEqual(@"<ref>{{Citation|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref>Citation|url=a|title=b}}</ref>"));
            Assert.AreEqual(@"<ref>{{cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref>((cite web|url=a|title=b}}</ref>"));
        }

        [Test]
        public void TestFixSyntaxUnbalancedBrackets()
        {
            Assert.AreEqual(@"<ref>[http://www.site.com]</ref>", Parsers.FixSyntax(@"<ref>{{http://www.site.com}}</ref>"));
            Assert.AreEqual(@"<ref>[http://www.site.com cool site]</ref>", Parsers.FixSyntax(@"<ref>{{http://www.site.com cool site}}</ref>"));
            Assert.AreEqual(@"<ref name=""Fred"">[http://www.site.com]</ref>", Parsers.FixSyntax(@"<ref name=""Fred"">{{http://www.site.com}}</ref>"));
            Assert.AreEqual(@"<ref> [http://www.site.com] </ref>", Parsers.FixSyntax(@"<ref> {{http://www.site.com}} </ref>"));

            Assert.AreEqual(@"{{hello}}", Parsers.FixSyntax(@"{[hello}}"));
            Assert.AreEqual(@"{{hello}}", Parsers.FixSyntax(@"[{hello}}"));

            Assert.AreEqual(@"<ref>[http://site.com]</ref>", Parsers.FixSyntax(@"<ref>http://site.com]</ref>"));
            Assert.AreEqual(@"<ref>[http://site.com Smith, 2004]</ref>", Parsers.FixSyntax(@"<ref>Smith, 2004 http://site.com]</ref>"));
            Assert.AreEqual(@"<ref> [http://site.com] </ref>", Parsers.FixSyntax(@"<ref> http://site.com] </ref>"));
            Assert.AreEqual(@"<ref name=Fred>[http://site.com cool]</ref>", Parsers.FixSyntax(@"<ref name=Fred>http://site.com cool]</ref>"));
            Assert.AreEqual(@"<ref name=""Fred"">[http://site.com]</ref>", Parsers.FixSyntax(@"<ref name=""Fred"">http://site.com]</ref>"));
            Assert.AreEqual(@"<ref>[http://site.com great site-here]</ref>", Parsers.FixSyntax(@"<ref>http://site.com great site-here]</ref>"));

            Assert.AreEqual(@"<ref>Antara News [http://www.antara.co.id/arc/2009/1/9/kri-lebanon/ KRI Lebanon]</ref>", Parsers.FixSyntax(@"<ref>Antara News [http://www.antara.co.id/arc/2009/1/9/kri-lebanon/ KRI Lebanon</ref>"));
            Assert.AreEqual(@"<ref name=Smith>Antara News [http://www.antara.co.id/arc/2009/1/9/kri-lebanon/ KRI Lebanon]</ref>", Parsers.FixSyntax(@"<ref name=Smith>Antara News [http://www.antara.co.id/arc/2009/1/9/kri-lebanon/ KRI Lebanon</ref>"));
            Assert.AreEqual(@"<ref name=Smith>Antara News [http://www.antara.co.id/arc/2009/1/9/kri-lebanon/ KRI Lebanon]</ref>", Parsers.FixSyntax(@"<ref name=Smith>Antara News [http://www.antara.co.id/arc/2009/1/9/kri-lebanon/ KRI Lebanon]</ref>"));

            // no completion of template braces on non-template ref
            const string a = @"<ref>Smith and Jones, 2005, p46}}</ref>";
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
            const string cite1 = @"Great.<ref>{{cite web | url=http://www.site.com | title=abc } year=2009}}</ref>";
            Assert.AreEqual(cite1, Parsers.FixSyntax(cite1));

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

            Assert.AreEqual(@"now ({{lang-el|foo}}) was", Parsers.FixSyntax(@"now ({lang-el|foo}}) was"));

            //  IndexOutOfRangeException bug
            Assert.AreEqual(@"] now", Parsers.FixSyntax(@"] now"));

            Assert.AreEqual(@"{{DEFAULTSORT:hello}}
now", Parsers.FixSyntax(@"{{DEFAULTSORT:hello
now"));
            
            Assert.AreEqual(@"|[[Belmont (Durham) railway station|Belmont]] ([[Durham]])
|[[North Eastern Railway (UK)|NER]]
|1857", Parsers.FixSyntax(@"|[[Belmont (Durham) railway station|Belmont]] {[[Durham]])
|[[North Eastern Railway (UK)|NER]]
|1857"));
        }
        
        [Test]
        public void FixUnbalancedBracketsRef()
        {
            Assert.AreEqual(@"now <ref>foo</ref>", Parsers.FixSyntax(@"now <ref>>foo</ref>"));
        }
        
        [Test]
        public void FixUnbalancedBracketsStrangeBrackets()
        {
            Assert.AreEqual(@"now (there) was", Parsers.FixSyntax(@"now （there) was"));
        }

        [Test]
        public void UppercaseCiteFields()
        {
            // single uppercase field
            Assert.AreEqual(@"{{cite web|url=http://members.bib-arch.org|title=hello}}", Parsers.FixCitationTemplates(@"{{cite web|URL=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{cite web|url=http://members.bib-arch.org|title=hello}}", Parsers.FixCitationTemplates(@"{{cite web|uRL=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{citeweb|url=http://members.bib-arch.org|title=hello}}", Parsers.FixCitationTemplates(@"{{citeweb|URL=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{cite web|url=http://members.bib-arch.org|title=hello}}", Parsers.FixCitationTemplates(@"{{cite web|UrL=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{cite web|url=http://members.bib-arch.org|title=hello}}", Parsers.FixCitationTemplates(@"{{cite web|Url=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{Cite web|url=http://members.bib-arch.org|title=hello}}", Parsers.FixCitationTemplates(@"{{Cite web|URL=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{cite web | url=http://members.bib-arch.org|title=hello}}", Parsers.FixCitationTemplates(@"{{cite web | URL=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{cite web| url = http://members.bib-arch.org|title=hello}}", Parsers.FixCitationTemplates(@"{{cite web| URL = http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{cite web|url =http://members.bib-arch.org|title=HELLO}}", Parsers.FixCitationTemplates(@"{{cite web|URL =http://members.bib-arch.org|title=HELLO}}"));

            // multiple uppercase fields
            Assert.AreEqual(@"{{cite web|url=http://members.bib-arch.org|title=hello}}", Parsers.FixCitationTemplates(@"{{cite web|URL=http://members.bib-arch.org|Title=hello}}"));
            Assert.AreEqual(@"{{cite web|url=http://members.bib-arch.org|title=hello}}", Parsers.FixCitationTemplates(@"{{cite web|URL=http://members.bib-arch.org|TITLE=hello}}"));
            Assert.AreEqual(@"{{cite web|url=http://members.bib-arch.org|title=hello | publisher=BBC}}", Parsers.FixCitationTemplates(@"{{cite web|URL=http://members.bib-arch.org|TITLE=hello | Publisher=BBC}}"));

            //other templates
            Assert.AreEqual(@"{{cite web|url=http://members.bib-arch.org|title=hello}}", Parsers.FixCitationTemplates(@"{{cite web|URL=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{cite book|url=http://members.bib-arch.org|title=hello}}", Parsers.FixCitationTemplates(@"{{cite book|URL=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{cite news|url=http://members.bib-arch.org|title=hello}}", Parsers.FixCitationTemplates(@"{{cite news|URL=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{cite journal|url=http://members.bib-arch.org|title=hello}}", Parsers.FixCitationTemplates(@"{{cite journal|URL=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{cite paper|url=http://members.bib-arch.org|title=hello}}", Parsers.FixCitationTemplates(@"{{cite paper|URL=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{cite press release|url=http://members.bib-arch.org|title=hello}}", Parsers.FixCitationTemplates(@"{{cite press release|URL=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{cite hansard|url=http://members.bib-arch.org|title=hello}}", Parsers.FixCitationTemplates(@"{{cite hansard|URL=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{cite encyclopedia|url=http://members.bib-arch.org|title=hello}}", Parsers.FixCitationTemplates(@"{{cite encyclopedia|URL=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{citation|url=http://members.bib-arch.org|title=hello}}", Parsers.FixCitationTemplates(@"{{citation|URL=http://members.bib-arch.org|title=hello}}"));

            Assert.AreEqual(@"{{Cite web|url=http://members.bib-arch.org|title=hello}}", Parsers.FixCitationTemplates(@"{{Cite web|URL=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{Cite book|url=http://members.bib-arch.org|title=hello}}", Parsers.FixCitationTemplates(@"{{Cite book|URL=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{Cite news|url=http://members.bib-arch.org|title=hello}}", Parsers.FixCitationTemplates(@"{{Cite news|URL=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{Cite journal|url=http://members.bib-arch.org|title=hello}}", Parsers.FixCitationTemplates(@"{{Cite journal|URL=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{Cite paper|url=http://members.bib-arch.org|title=hello}}", Parsers.FixCitationTemplates(@"{{Cite paper|URL=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{Cite press release|url=http://members.bib-arch.org|title=hello}}", Parsers.FixCitationTemplates(@"{{Cite press release|URL=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{Cite hansard|url=http://members.bib-arch.org|title=hello}}", Parsers.FixCitationTemplates(@"{{Cite hansard|URL=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{Cite encyclopedia|url=http://members.bib-arch.org|title=hello}}", Parsers.FixCitationTemplates(@"{{Cite encyclopedia|URL=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{Citation|url=http://members.bib-arch.org|title=hello}}", Parsers.FixCitationTemplates(@"{{Citation|URL=http://members.bib-arch.org|title=hello}}"));

            Assert.AreEqual(@"{{cite book | author=Smith | title=Great Book | ISBN=15478454 | date=17 May 2004 }}", Parsers.FixCitationTemplates(@"{{cite book | author=Smith | Title=Great Book | ISBN=15478454 | date=17 May 2004 }}"));

            // no match if value of field uppercase
            Assert.AreEqual(@"{{cite web|url=http://members.bib-arch.org|title=hello | publisher=BBC}}", Parsers.FixCitationTemplates(@"{{cite web|url=http://members.bib-arch.org|title=hello | publisher=BBC}}"));

            // ISBN, DOI, PMID is allowed to be uppercase
            string ISBN = @"{{cite book | author=Smith | title=Great Book | ISBN=15478454 | date=17 May 2004 }}";
            Assert.AreEqual(ISBN, Parsers.FixCitationTemplates(ISBN));
            string DOI = @"{{cite journal| author=Smith | title=Great Book | DOI=15478454 | date=17 May 2004 }}";
            Assert.AreEqual(DOI, Parsers.FixCitationTemplates(DOI));

            string PMID = @"{{cite journal| author=Smith | title=Great Book | PMID=15478454 | date=17 May 2004 }}";
            Assert.AreEqual(PMID, Parsers.FixCitationTemplates(PMID));
            
            // don't match on part of URL
            string URL = @"{{cite news|url=http://www.expressbuzz.com/edition/story.aspx?Title=Catching++them+young&artid=rPwTAv2l2BY=&SectionID=fxm0uEWnVpc=&MainSectionID=ngGbWGz5Z14=&SectionName=RtFD/|pZbbWSsbI0jf3F5Q==&SEO=|title=Catching them young|date=August 7, 2009|publisher=[[The Indian Express]]|accessdate=2009-08-07}}";
            Assert.AreEqual(URL, Parsers.FixCitationTemplates(URL));
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
            const string correct = @"now {{cite web | url=http://site.it | title=hello|accessdate = 2008-04-23 }} was";
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"now {{cite web | url=http://site.it | title=hello|acessdate = 2008-04-23 }} was"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"now {{cite web | url=http://site.it | title=hello|accessedate = 2008-04-23 }} was"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"now {{cite web | url=http://site.it | title=hello|date accessed = 2008-04-23 }} was"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"now {{cite web | url=http://site.it | title=hello|retrieved = 2008-04-23 }} was"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"now {{cite web | url=http://site.it | title=hello|retrieved on = 2008-04-23 }} was"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"now {{cite web | url=http://site.it | title=hello|accesdate = 2008-04-23 }} was"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"now {{cite web | url=http://site.it | title=hello|Acessdate = 2008-04-23 }} was"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"now {{cite web | url=http://site.it | title=hello|Accesdate = 2008-04-23 }} was"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"now {{cite web | url=http://site.it | title=hello|Acesdate = 2008-04-23 }} was"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"now {{cite web | url=http://site.it | title=hello|acesdate = 2008-04-23 }} was"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"now {{cite web | url=http://site.it | title=hello|acccesdate = 2008-04-23 }} was"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"now {{cite web | url=http://site.it | title=hello|access date = 2008-04-23 }} was"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"now {{cite web | url=http://site.it | title=hello|acccessdate = 2008-04-23 }} was"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"now {{cite web | url=http://site.it | title=hello|accessdat = 2008-04-23 }} was"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"now {{cite web | url=http://site.it | title=hello|accessdare = 2008-04-23 }} was"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"now {{cite web | url=http://site.it | title=hello|accessdaye = 2008-04-23 }} was"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"now {{cite web | url=http://site.it | title=hello|accessdaste = 2008-04-23 }} was"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"now {{cite web | url=http://site.it | title=hello|accessate = 2008-04-23 }} was"));

            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|accessdate = 2008-04-23|publisher=BBC }} was", Parsers.FixCitationTemplates(@"now {{cite web | url=http://site.it | title=hello|acccessdate = 2008-04-23|publisher=BBC }} was"));
            Assert.AreEqual(@"now {{Cite web | url=http://site.it | title=hello|accessdate = 2008-04-23  |publisher=BBC }} was", Parsers.FixCitationTemplates(@"now {{Cite web | url=http://site.it | title=hello|acccessdate = 2008-04-23  |publisher=BBC }} was"));

            Assert.AreEqual(@"now {{cite web |accessdate = 2008-04-23| url=http://site.it | title=hello }} was", Parsers.FixCitationTemplates(@"now {{cite web |accessdaye = 2008-04-23| url=http://site.it | title=hello }} was"));

            // no match
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|acccessdated = 2008-04-23 }} was", Parsers.FixCitationTemplates(@"now {{cite web | url=http://site.it | title=hello|acccessdated = 2008-04-23 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|accessed = 2008-04-23 }} was", Parsers.FixCitationTemplates(@"now {{cite web | url=http://site.it | title=hello|accessed = 2008-04-23 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|accessdate = 2008-04-23 }} was", Parsers.FixCitationTemplates(@"now {{cite web | url=http://site.it | title=hello|accessdate = 2008-04-23 }} was"));
        }

        [Test]
        public void PublisherTypo()
        {
            const string correct = @"now {{cite web | url=http://site.it |publisher=hello|accessdate = 2008-04-23 }} was";
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"now {{cite web | url=http://site.it |ublisher=hello|acessdate = 2008-04-23 }} was"));

            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"now {{cite web | url=http://site.it |pubslisher=hello|acessdate = 2008-04-23 }} was"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"now {{cite web | url=http://site.it |puablisher=hello|acessdate = 2008-04-23 }} was"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"now {{cite web | url=http://site.it |publsiher=hello|acessdate = 2008-04-23 }} was"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"now {{cite web | url=http://site.it |pblisher=hello|acessdate = 2008-04-23 }} was"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"now {{cite web | url=http://site.it |publishet=hello|acessdate = 2008-04-23 }} was"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"now {{cite web | url=http://site.it |puiblisher=hello|acessdate = 2008-04-23 }} was"));
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

            Assert.AreEqual(@"now <ref>{{cite news|url=http://www.a1gp.com/News/NewsArticle.aspx?newsId=42370|title=It's all about the bumps|accessdate=2008-11-16|date=2008-11-07|publisher=a1gp.com}}</ref> ", Parsers.CiteTemplateDates(@"now <ref>{{cite news|url=http://www.a1gp.com/News/NewsArticle.aspx?newsId=42370|title=It's all about the bumps|accessdate=2008-Nov-16|date=2008-Nov-07|publisher=a1gp.com}}</ref> "));

            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-01-07 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 2008 Jan. 07 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-01-07 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 2008 Jan 07 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 1998-01-07 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 1998 January 07 }} was"));

            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|archivedate = 2008-12-07 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|archivedate = 2008-Dec.-07 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|airdate = 2008-12-07 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|airdate = 2008-Dec.-07 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date2 = 2008-12-07 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date2 = 2008-Dec.-07 }} was"));

            // no change – ambiguous between Am and Int date format
            Assert.AreEqual(@"<ref>{{cite web|url=http://www.nuclearweaponarchive.org/Russia/TsarBomba.html|title=The Tsar Bomba (King of Bombs)|accessdate=11-05-2006}}</ref>", Parsers.CiteTemplateDates(@"<ref>{{cite web|url=http://www.nuclearweaponarchive.org/Russia/TsarBomba.html|title=The Tsar Bomba (King of Bombs)|accessdate=11-05-2006}}</ref>"));

            // cite podcast is non-compliant to citation core standards
            const string CitePodcast = @"{{cite podcast
| url =http://www.heretv.com/APodcastDetailPage.php?id=24
| title =The Ben and Dave Show
| host =Harvey, Ben and Rubin, Dave
| accessdate =11-08
| accessyear =2007
}}";
            Assert.AreEqual(CitePodcast, Parsers.CiteTemplateDates(CitePodcast));

            // more than one date in a citation
            Assert.AreEqual("{{cite foo|date=2008-12-11|accessdate=2008-08-07}}", Parsers.CiteTemplateDates("{{cite foo|date=2008-December-11|accessdate=2008-Aug.-07}}"));
            Assert.AreEqual("{{cite foo|date=2008-12-11|accessdate=2008-08-07}}", Parsers.CiteTemplateDates("{{cite foo|date=2008-Dec.-11|accessdate=2008-Aug.-07}}"));

            // don't apply fixes when ambiguous dates present
            string ambig = @"now {{cite web | url=http://site.it | title=hello|date = 5-4-1998}} was
now {{cite web | url=http://site.it | title=hello|date = 5-5-1998}} was";

            Assert.AreEqual(ambig, Parsers.CiteTemplateDates(ambig));
        }

        [Test]
        public void AmbiguousCiteTemplateDates()
        {
            Assert.IsTrue(Parsers.AmbiguousCiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 5-4-1998}} was"));
            Assert.IsTrue(Parsers.AmbiguousCiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|accessdate = 5-4-1998}} was"));
            Assert.IsTrue(Parsers.AmbiguousCiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 5/4/1998}} was"));
            Assert.IsTrue(Parsers.AmbiguousCiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 05-04-1998}} was"));
            Assert.IsTrue(Parsers.AmbiguousCiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 11-4-1998}} was"));
            Assert.IsTrue(Parsers.AmbiguousCiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|archivedate = 11-4-2008}} was"));
            Assert.IsTrue(Parsers.AmbiguousCiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 5-11-09}} was"));

            Assert.IsFalse(Parsers.AmbiguousCiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 5-5-1998}} was"));
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
            const string correct = @"now {{cite web|url=http://site.net|title=hello}} was";

            Assert.AreEqual(correct, Parsers.FixSyntax(@"now {{cite web|url=[http://site.net]|title=hello}} was"));
            Assert.AreEqual(correct, Parsers.FixSyntax(@"now {{cite web|url=http://site.net]|title=hello}} was"));
            Assert.AreEqual(correct, Parsers.FixSyntax(@"now {{cite web|url=[http://site.net|title=hello}} was"));
            Assert.AreEqual(correct, Parsers.FixSyntax(@"now {{cite web|url=[[http://site.net]|title=hello}} was"));
            Assert.AreEqual(correct, Parsers.FixSyntax(@"now {{cite web|url=[http://site.net]]|title=hello}} was"));
            Assert.AreEqual(correct, Parsers.FixSyntax(@"now {{cite web|url=[[http://site.net]]|title=hello}} was"));
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
            Assert.AreEqual("b [[a]] c", Parsers.FixLinkWhitespace("b[[ a ]]c", "foo")); // regexes 1 & 2
            Assert.AreEqual("b   [[a]]  c", Parsers.FixLinkWhitespace("b   [[ a ]]  c", "foo")); // 4 & 5

            Assert.AreEqual("[[a]] b", Parsers.FixLinkWhitespace("[[a ]]b", "foo"));

            Assert.AreEqual("[[foo bar]]", Parsers.FixLinkWhitespace("[[foo  bar]]", "foot"));
            Assert.AreEqual("[[foo bar]]", Parsers.FixLinkWhitespace("[[foo  bar]]", ""));
            Assert.AreEqual("[[foo bar]]", Parsers.FixLinkWhitespace("[[foo     bar]]", "foot"));
            Assert.AreEqual("dat is [[foo bar]] show!", Parsers.FixLinkWhitespace("dat is [[ foo   bar ]] show!", "foot"));
            Assert.AreEqual("dat is [[foo bar]] show!", Parsers.FixLinkWhitespace("dat is[[ foo   bar ]]show!", "foot"));

            Assert.AreEqual(@"His [[Tiger Woods#Career]] was", Parsers.FixLinkWhitespace(@"His [[Tiger Woods# Career]] was", "Tiger Woods"));

            // don't fix when bit before # is not article name
            Assert.AreNotSame(@"Fred's [[Smith#Career]] was", Parsers.FixLinkWhitespace(@"Fred's [[Smith# Career]] was", "Fred"));

            Assert.AreEqual(@"[[Category:London| ]]", Parsers.FixLinkWhitespace(@"[[Category:London| ]]", "foo")); // leading space NOT removed from cat sortkey
            Assert.AreEqual(@"[[Category:Slam poetry| ]] ", Parsers.FixLinkWhitespace(@"[[Category:Slam poetry| ]] ", "foo")); // leading space NOT removed from cat sortkey

            // shouldn't fix - not enough information
            //Assert.AreEqual("[[ a ]]", Parsers.FixLinkWhitespace("[[ a ]]", "foo"));
            //disabled for the time being to avoid unnecesary clutter
        }

        public void TestCanonicalizeTitle()
        {
            Assert.AreEqual("foo bar", Parsers.CanonicalizeTitle("foo_bar"));
            Assert.AreEqual("foo bar", Parsers.CanonicalizeTitle("foo bar"));
            Assert.AreEqual("foo (bar)", Parsers.CanonicalizeTitle("foo_%28bar%29"));
            Assert.AreEqual(@"foo+bar", Parsers.CanonicalizeTitle(@"foo%2Bbar"));
            Assert.AreEqual("foo (bar)", Parsers.CanonicalizeTitle("foo_(bar)"));

            // it may or may not fix it, but shouldn't break anything
            StringAssert.Contains("{{bar_boz}}", Parsers.CanonicalizeTitle("foo_bar{{bar_boz}}"));
        }

        [Test]
        public void CanonicalizeTitleAggressively()
        {
            Assert.AreEqual("Foo", Parsers.CanonicalizeTitleAggressively("Foo"));

            Assert.AreEqual("Foo (bar)", Parsers.CanonicalizeTitleAggressively("foo_%28bar%29#anchor"));
            Assert.AreEqual("Wikipedia:Foo", Parsers.CanonicalizeTitleAggressively("project : foo"));
            Assert.AreEqual("File:Foo.jpg", Parsers.CanonicalizeTitleAggressively("Image: foo.jpg "));

            // a bit of ambiguousness here, but
            // http://en.wikipedia.org/wiki/Wikipedia:AWB/B#Problem_.28on_runecape_wikia.29_with_articles_with_.2B_in_the_name.
            Assert.AreEqual("Romeo+Juliet", Parsers.CanonicalizeTitleAggressively("Romeo+Juliet"));
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

        [Test, Ignore]
        public void SubstitutedTemplatesCategory()
        {
            Assert.AreEqual(@"<includeonly>foo</includeonly> foo
[[Category:Substituted templates]]", Parsers.FixCategories(@"<includeonly>foo</includeonly> foo"));
            Assert.AreEqual(@"<noinclude>foo</noinclude> foo
[[Category:Substituted templates]]", Parsers.FixCategories(@"<noinclude>foo</noinclude> foo"));
            Assert.AreEqual(@"{{{1}}} foo
[[Category:Substituted templates]]", Parsers.FixCategories(@"{{{1}}} foo"));

            Assert.AreEqual(@"{{{1}}} foo
[[Category:Substituted templates]]", Parsers.FixCategories(@"{{{1}}} foo
[[Category:Substituted templates]]"));

            Assert.AreEqual(@"{{{1}}} foo", Parsers.FixCategories(@"{{{1}}} foo"));
        }

        [Test]
        public void FixCategories()
        {
            Assert.AreEqual(@"[[Category:Foo bar]]", Parsers.FixCategories(@"[[Category:Foo_bar]]"));
            Assert.AreEqual(@"[[Category:Foo bar]]", Parsers.FixCategories(@"[[category:Foo bar]]"));
            Assert.AreEqual(@"[[Category:Foo bar]]", Parsers.FixCategories(@"[[Category  :  Foo_bar  ]]"));
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
            Assert.AreEqual(@"a 50&nbsp;m (170&nbsp;ft) road", parser.FixNonBreakingSpaces(@"a 50 m (170 ft) road"));
            Assert.AreEqual(@"a 50.2&nbsp;m (170&nbsp;ft) road", parser.FixNonBreakingSpaces(@"a 50.2 m (170 ft) road"));
            Assert.AreEqual(@"a long (50&nbsp;km) road", parser.FixNonBreakingSpaces(@"a long (50 km) road"));
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
            Assert.AreEqual(@"a 50.247&nbsp;kW laser", parser.FixNonBreakingSpaces(@"a 50.247 kW laser"));
            Assert.AreEqual(@"a 50.247&nbsp;mW laser", parser.FixNonBreakingSpaces(@"a 50.247 mW laser"));

            // no changes for these
            Assert.AreEqual(@"nearly 5m people", parser.FixNonBreakingSpaces(@"nearly 5m people"));
            Assert.AreEqual(@"nearly 5 in 10 people", parser.FixNonBreakingSpaces(@"nearly 5 in 10 people"));
            Assert.AreEqual(@"a 3CD set", parser.FixNonBreakingSpaces(@"a 3CD set"));
            Assert.AreEqual(@"its 3 feet are", parser.FixNonBreakingSpaces(@"its 3 feet are"));
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

            // Imperial units
            Assert.AreEqual(@"a long (50&nbsp;in) toad", parser.FixNonBreakingSpaces(@"a long (50 in) toad"));
            Assert.AreEqual(@"a long (50&nbsp;ft) toad", parser.FixNonBreakingSpaces(@"a long (50 ft) toad"));
            Assert.AreEqual(@"a long (50&nbsp;feet) toad", parser.FixNonBreakingSpaces(@"a long (50 feet) toad"));
            Assert.AreEqual(@"a long (50&nbsp;foot) toad", parser.FixNonBreakingSpaces(@"a long (50 foot) toad"));
            Assert.AreEqual(@"a long (50&nbsp;in) toad", parser.FixNonBreakingSpaces(@"a long (50in) toad"));
            Assert.AreEqual(@"a long (50-52&nbsp;in) toad", parser.FixNonBreakingSpaces(@"a long (50-52in) toad"));
            Assert.AreEqual(@"a long (50–52&nbsp;in) toad", parser.FixNonBreakingSpaces(@"a long (50–52in) toad"));
            Assert.AreEqual(@"a long (50&nbsp;inches) toad", parser.FixNonBreakingSpaces(@"a long (50 inches) toad"));
            Assert.AreEqual(@"a long 50&nbsp;inches toad", parser.FixNonBreakingSpaces(@"a long 50 inches toad"));
            Assert.AreEqual(@"a long 50&nbsp;inch toad", parser.FixNonBreakingSpaces(@"a long 50 inch toad"));
            Assert.AreEqual(@"a long (50.5&nbsp;in) toad", parser.FixNonBreakingSpaces(@"a long (50.5 in) toad"));
        }

        [Test]
        public void FixNonBreakingSpacesPagination()
        {
            Assert.AreEqual(@"Smith 2004, p.&nbsp;40", parser.FixNonBreakingSpaces(@"Smith 2004, p. 40"));
            Assert.AreEqual(@"Smith 2004, p.&nbsp;40", parser.FixNonBreakingSpaces(@"Smith 2004, p.  40"));
            Assert.AreEqual(@"Smith 2004, p.&nbsp;40", parser.FixNonBreakingSpaces(@"Smith 2004, p.40"));
            Assert.AreEqual(@"Smith 2004, p.&nbsp;XI", parser.FixNonBreakingSpaces(@"Smith 2004, p. XI"));
            Assert.AreEqual(@"Smith 2004, pp.&nbsp;40-44", parser.FixNonBreakingSpaces(@"Smith 2004, pp. 40-44"));
            Assert.AreEqual(@"Smith 2004, Pp.&nbsp;40-44", parser.FixNonBreakingSpaces(@"Smith 2004, Pp. 40-44"));
            Assert.AreEqual(@"Smith 2004, p.&nbsp;40", parser.FixNonBreakingSpaces(@"Smith 2004, p.&nbsp;40"));
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

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#ReferenceS
            Assert.AreEqual(@"==References==", Parsers.FixHeadings(@"==REFERENCES==", "a"));
            Assert.AreEqual(@"==Reference==", Parsers.FixHeadings(@"==REFERENCE:==", "a"));
            Assert.AreEqual(@"==References==", Parsers.FixHeadings(@"==REFERENSES==", "a"));
            Assert.AreEqual(@"==Reference==", Parsers.FixHeadings(@"==REFERENCE==", "a"));
            Assert.AreEqual(@"==Reference==", Parsers.FixHeadings(@"==REFERENCE:==", "a"));
            Assert.AreEqual(@"== References ==", Parsers.FixHeadings(@"== REFERENCES ==", "a"));
            Assert.AreEqual(@"==Sources==", Parsers.FixHeadings(@"==SOURCES==", "a"));
            Assert.AreEqual(@"==Sources==", Parsers.FixHeadings(@"==sources==", "a"));
            Assert.AreEqual(@"==Source==", Parsers.FixHeadings(@"==source==", "a"));
            Assert.AreEqual(@"==Source==", Parsers.FixHeadings(@"==source:==", "a"));
            Assert.AreEqual(@"== Sources ==", Parsers.FixHeadings(@"== SOURCES ==", "a"));
        }

        [Test]
        public void TestFixHeadingsDelinking()
        {
            // tests for regexRemoveHeadingsInLinks
            Assert.AreEqual("==foo==", Parsers.FixHeadings("==[[foo]]==", "test"));
            Assert.AreEqual("== foo ==", Parsers.FixHeadings("== [[foo]] ==", "test"));
            Assert.AreEqual("==bar==", Parsers.FixHeadings("==[[foo|bar]]==", "test"));
            Assert.AreEqual(@"==hello world ==
", Parsers.FixHeadings(@"==hello [[world]] ==
", "a"));
            Assert.AreEqual("==Hello world==", Parsers.FixHeadings("==[[Hello world]]==", "a"));
            Assert.AreEqual("==world==", Parsers.FixHeadings("==[[hello|world]]==", "a"));
            Assert.AreEqual("== world ==", Parsers.FixHeadings("== [[hello|world]] ==", "a"));
            Assert.AreEqual("==world==", Parsers.FixHeadings("==[[hello now|world]]==", "a"));
            Assert.AreEqual("==world now==", Parsers.FixHeadings("==[[hello|world now]]==", "a"));
            Assert.AreEqual(@"
==now world==
", Parsers.FixHeadings(@"
==now [[hello|world]]==
", "a"));
            Assert.AreEqual("==now world here==", Parsers.FixHeadings("==now [[hello|world]] here==", "a"));
            Assert.AreEqual("==now world def here==", Parsers.FixHeadings("==now [[hello abc|world def]] here==", "a"));
            Assert.AreEqual("==now world here==", Parsers.FixHeadings("==now [[ hello |world]] here==", "a"));
            Assert.AreEqual("==now world==", Parsers.FixHeadings("==now [[hello#world|world]]==", "a"));
            Assert.AreEqual("==now world and moon==", Parsers.FixHeadings("==now [[hello|world]] and [[bye|moon]]==", "a"));
            Assert.AreEqual("===hello world ===", Parsers.FixHeadings("===hello [[world]] ===", "a"));
            Assert.AreEqual("====hello world ====", Parsers.FixHeadings("====hello [[world]] ====", "a"));
            Assert.AreEqual("====hello world ====", Parsers.FixHeadings("====hello [[world]] ====", "7899"));
            Assert.AreEqual("====United States Marine Corps====", Parsers.FixHeadings("====[[United States Marine Corps]]====", "a"));
            Assert.AreEqual(@"
==foo==
====United States Marine Corps====
*[[V-22 Osprey]] - tilt-rotor, [[VTOL]] tactical transport", Parsers.FixHeadings(@"
==foo==
====[[United States Marine Corps]]====
*[[V-22 Osprey]] - tilt-rotor, [[VTOL]] tactical transport", "a"));

            // no match
            Assert.AreEqual("===hello [[world]] ==", Parsers.FixHeadings("===hello [[world]] ==", "a"));
            Assert.AreEqual("== hello world ==", Parsers.FixHeadings("== hello world ==", "a"));
            Assert.AreEqual("==hello==", Parsers.FixHeadings("==hello==", "a"));
            Assert.AreEqual("==now [[hello|world] ] here==", Parsers.FixHeadings("==now [[hello|world] ] here==", "a"));
            Assert.AreEqual("==hello[http://example.net]==", Parsers.FixHeadings("==hello[http://example.net]==", "a"));
            Assert.AreEqual("hello [[world]]", Parsers.FixHeadings("hello [[world]]", "a"));
            Assert.AreEqual("now == hello [[world]] == here", Parsers.FixHeadings("now == hello [[world]] == here", "a"));

            // some articles excepted
            const string linked = "==now [[hello|world]] here==";
            Assert.AreEqual(linked, Parsers.FixHeadings(linked, "List of stuff"));
            Assert.AreEqual(linked, Parsers.FixHeadings(linked, "1980 in poetry"));
            Assert.AreEqual(linked, Parsers.FixHeadings(linked, "1980"));
            Assert.AreEqual(linked, Parsers.FixHeadings(linked, "January 11"));
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

            // don't consider the "references", "see also", or "external links" level 2 headings when counting level two headings
            // single heading
            Assert.AreEqual(@"===hello===
text
==References==
foo", Parsers.FixHeadings(@"====hello====
text
==References==
foo", "a"));

            Assert.AreEqual(@"===hello===
text
==External links==
foo", Parsers.FixHeadings(@"====hello====
text
==External links==
foo", "a"));

            Assert.AreEqual(@"===hello===
text
==See also==
==External links==
foo", Parsers.FixHeadings(@"====hello====
text
==See also==
==External links==
foo", "a"));

            // no change
            Assert.AreEqual(@"==hello==
text
==References==
foo", Parsers.FixHeadings(@"==hello==
text
==References==
foo", "a"));

            // don't apply where level 3 headings after references/external links/see also
            const string a = @"====hello====
text
==External links==
foo
===bar===
foo2";
            Assert.AreEqual(a, Parsers.FixHeadings(a, "a"));

            const string a2 = @"text
==External links==
foo
===bar===
foo2";
            Assert.AreEqual(a2, Parsers.FixHeadings(a2, "a"));

            // only apply on mainspace
            Assert.AreEqual(@"====hello====
text", Parsers.FixHeadings(@"====hello====
text", "Talk:foo"));
        }

        [Test]
        public void TestFixHeadingsBoldRemoval()
        {
            Assert.AreEqual(@"=== Caernarvon 1536-1832 ===", Parsers.FixHeadings(@"=== '''Caernarvon''' 1536-1832 ===", "a"));
            Assert.AreEqual(@"===Caernarvon 1536-1832===", Parsers.FixHeadings(@"==='''Caernarvon''' 1536-1832===", "a"));
            Assert.AreEqual(@"=== Caernarvon 1536-1832 ===", Parsers.FixHeadings(@"=== Caernarvon '''1536-1832''' ===", "a"));
            Assert.AreEqual(@"=== Caernarvon 1536-1832 ===", Parsers.FixHeadings(@"=== '''Caernarvon 1536-1832''' ===", "a"));
            Assert.AreEqual(@"==== Caernarvon 1536-1832 ====", Parsers.FixHeadings(@"==== '''Caernarvon 1536-1832''' ====", "a"));

            // not at level 1 or 2
            Assert.AreEqual(@"== '''Caernarvon''' 1536-1832 ==", Parsers.FixHeadings(@"== '''Caernarvon''' 1536-1832 ==", "a"));
            Assert.AreEqual(@"= '''Caernarvon''' 1536-1832 =", Parsers.FixHeadings(@"= '''Caernarvon''' 1536-1832 =", "a"));
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
        public void TestMdashesRanges()
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
            Assert.AreEqual("55 – 57 m long", parser.Mdashes("55 - 57 m long", "test", 0));
            Assert.AreEqual("55 – 57 feet", parser.Mdashes("55 - 57 feet", "test", 0));
            Assert.AreEqual("55 – 57 foot", parser.Mdashes("55 - 57 foot", "test", 0));
            Assert.AreEqual("long (55 – 57 in) now", parser.Mdashes("long (55 - 57 in) now", "test", 0));
            
            // dimensions, not range
            const string dimensions = @"around 34-24-34 in (86-60-86&nbsp;cm) and";
            Assert.AreEqual(dimensions, parser.Mdashes(dimensions, "test", 0));

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
        }

        [Test]
        public void TestMdashes()
        {
            // double dash to emdash
            Assert.AreEqual(@"Djali Zwan made their live debut as a quartet—Corgan, Sweeney, Pajo and Chamberlin—at the end of 2001", parser.Mdashes(@"Djali Zwan made their live debut as a quartet -- Corgan, Sweeney, Pajo and Chamberlin -- at the end of 2001", "test", 0));
            Assert.AreEqual(@"Djali Zwan made their live debut as a quartet—Corgan, Sweeney, Pajo and Chamberlin—at the end of 2001", parser.Mdashes(@"Djali Zwan made their live debut as a quartet --  Corgan, Sweeney, Pajo and Chamberlin --at the end of 2001", "test", 0));
            Assert.AreEqual(@"Djali Zwan made their live debut as a quartet—Corgan, Sweeney, Pajo and Chamberlin—at the end of 2001", parser.Mdashes(@"Djali Zwan made their live debut as a quartet -- Corgan, Sweeney, Pajo and Chamberlin--at the end of 2001", "test", 0));

            // only applied on article namespace
            Assert.AreEqual(@"Djali Zwan made their live debut as a quartet -- Corgan, Sweeney, Pajo and Chamberlin -- at the end of 2001", parser.Mdashes(@"Djali Zwan made their live debut as a quartet -- Corgan, Sweeney, Pajo and Chamberlin -- at the end of 2001", "test", 1));

            // precisely two dashes only
            Assert.AreEqual(@"Djali Zwan made their live debut as a quartet --- Corgan, Sweeney, Pajo and Chamberlin - at the end of 2001", parser.Mdashes(@"Djali Zwan made their live debut as a quartet --- Corgan, Sweeney, Pajo and Chamberlin - at the end of 2001", "test", 0));

            Assert.AreEqual("m<sup>−33</sup>", parser.Mdashes("m<sup>-33</sup>", "test", 0)); // hyphen
            Assert.AreEqual("m<sup>−3324</sup>", parser.Mdashes("m<sup>-3324</sup>", "test", 0)); // hyphen
            Assert.AreEqual("m<sup>−2</sup>", parser.Mdashes("m<sup>-2</sup>", "test", 0)); // hyphen
            Assert.AreEqual("m<sup>−2</sup>", parser.Mdashes("m<sup>–2</sup>", "test", 0)); // en-dash
            Assert.AreEqual("m<sup>−2</sup>", parser.Mdashes("m<sup>—2</sup>", "test", 0)); // em-dash

            // already correct
            Assert.AreEqual("m<sup>−2</sup>", parser.Mdashes("m<sup>−2</sup>", "test", 0));
            Assert.AreEqual("m<sup>2</sup>", parser.Mdashes("m<sup>2</sup>", "test", 0));

            // false positive
            Assert.AreEqual("beaten 55 - 57 in 2004", parser.Mdashes("beaten 55 - 57 in 2004", "test", 0));
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

        [Test]
        public void RemoveAllWhiteSpaceTests()
        {
            Assert.AreEqual("now a", Parsers.RemoveAllWhiteSpace(@"now a"));
            Assert.AreEqual(@"now
* foo", Parsers.RemoveAllWhiteSpace(@"now

* foo"));

            Assert.AreEqual("now was", Parsers.RemoveAllWhiteSpace(@"now   was"));

            Assert.AreEqual(@"now
was", Parsers.RemoveAllWhiteSpace(@"now
was"));

            Assert.AreEqual(@"==hi==
was", Parsers.RemoveAllWhiteSpace(@"==hi==

was"));
            Assert.AreEqual(@"==hi==", Parsers.RemoveAllWhiteSpace(@"== hi =="));
            Assert.AreEqual(@"==hi==", Parsers.RemoveAllWhiteSpace(@"== hi=="));

            Assert.AreEqual("now–was", Parsers.RemoveAllWhiteSpace(@"now – was"));
        }

        [Test]
        public void FixTemperaturesTests()
        {
            Assert.AreEqual("5°C today", Parsers.FixTemperatures(@"5ºC today"));
            Assert.AreEqual("5°C today", Parsers.FixTemperatures(@"5ºc today"));
            Assert.AreEqual("5°C today", Parsers.FixTemperatures(@"5°C today"));
            Assert.AreEqual("5°C today", Parsers.FixTemperatures(@"5°    C today"));
            Assert.AreEqual("5°C today", Parsers.FixTemperatures(@"5°C today"));
            Assert.AreEqual("5°F today", Parsers.FixTemperatures(@"5°F today"));
            Assert.AreEqual("75°F today", Parsers.FixTemperatures(@"75°f today"));

            Assert.AreEqual("5ºCC", Parsers.FixTemperatures(@"5ºCC"));
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
            Assert.AreEqual(@"On June 21, 2008 elections were", parser.FixDateOrdinalsAndOf(@"On June   21st 2008 elections were", "test"));

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
            Assert.AreEqual(@"On June 7, 2008, elections were", parser.FixDateOrdinalsAndOf(@"On June 07 2008, elections were", "test"));
            Assert.AreEqual(@"On 3 June elections were", parser.FixDateOrdinalsAndOf(@"On 03rd June elections were", "test"));

            // no Matches
            Assert.AreEqual(@"The 007 march was", parser.FixDateOrdinalsAndOf(@"The 007 march was", "test"));
            Assert.AreEqual(@"In June '08 there was", parser.FixDateOrdinalsAndOf(@"In June '08 there was", "test"));
            Assert.AreEqual(@"In June 2008 there was", parser.FixDateOrdinalsAndOf(@"In June 2008 there was", "test"));
            Assert.AreEqual(@"On 00 June elections were", parser.FixDateOrdinalsAndOf(@"On 00 June elections were", "test"));
            Assert.AreEqual(@"The 007 May was", parser.FixDateOrdinalsAndOf(@"The 007 May was", "test"));
            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#Overzealous_de-ordinaling
            Assert.AreEqual(@"On 27 June 2nd and 3rd Panzer Groups", parser.FixDateOrdinalsAndOf(@"On 27 June 2nd and 3rd Panzer Groups", "test"));
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
            const string Diamminesilver = @"[[Image:Diamminesilver(I)-3D-balls.png|thumb|right|200px|Ball-and-stick model of the diamminesilver(I) cation, [Ag(NH<sub>3</sub>)<sub>2</sub>]<sup>+</sup>]]";
            Assert.AreEqual(Diamminesilver, Parsers.FixImages(Diamminesilver));

            const string a = @"[[Image:End CEST Transparent.png|thumb|left|120px|
alt=Diagram of a clock showing a transition from 3:00 to 2:00.|
When DST ends in central Europe, clocks retreat from 03:00 CEST to 02:00 CET. Other regions switch at different times.]]";

            Assert.AreEqual(a, Parsers.FixSyntax(a));
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
            
            Assert.AreEqual("'''Now''' Foo is a bar While remaining upright may be the primary goal of beginning riders",
                            parser.BoldTitle("'''Now''' Foo is a bar While remaining upright may be the primary goal of beginning riders", "Foo", out noChangeBack));
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
            
            Assert.AreEqual("'''Foo''' (here) is a bar While remaining upright may be the primary goal of beginning riders",
                            parser.BoldTitle("Foo (here) is a bar While remaining upright may be the primary goal of beginning riders", "Foo (here)", out noChangeBack));
            Assert.IsFalse(noChangeBack);
            
            Assert.AreEqual("'''Foo.(here)''' is a bar While remaining upright may be the primary goal of beginning riders",
                            parser.BoldTitle("Foo.(here) is a bar While remaining upright may be the primary goal of beginning riders", "Foo.(here)", out noChangeBack));
            Assert.IsFalse(noChangeBack);

            // only first instance bolded
            Assert.AreEqual("'''Foo''' is a bar While remaining upright may be the primary goal of beginning riders Foo",
                            parser.BoldTitle("Foo is a bar While remaining upright may be the primary goal of beginning riders Foo", "Foo", out noChangeBack));
            Assert.IsFalse(noChangeBack);

            Assert.AreEqual(@"The '''Foo''' is a bar While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders",
                            parser.BoldTitle(@"The Foo is a bar While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders", "Foo", out noChangeBack));
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
            
            // immediately after infobox – 5% rule does not apply
            Assert.AreEqual(@"{{Infobox abc| name = Foo | age=11}}
'''Foo''' is a bar", parser.BoldTitle(@"{{Infobox abc| name = Foo | age=11}}
Foo is a bar", "Foo", out noChangeBack));
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

            // no delinking when existing in bold, even if in template
            Assert.AreEqual(@"{{unicode|'''Ł̣'''}} ([[Lower case|minuscule]]: {{unicode|'''ł̣'''}}) is a letter of the [[Latin alphabet]], derived from [[Ł̣]] with a diacritical",
                            parser.BoldTitle(@"{{unicode|'''Ł̣'''}} ([[Lower case|minuscule]]: {{unicode|'''ł̣'''}}) is a letter of the [[Latin alphabet]], derived from [[Ł̣]] with a diacritical", @"Ł̣", out noChangeBack));
            Assert.IsTrue(noChangeBack);

            // image descriptions NOT bolded:
            Assert.AreEqual(@"[[Image:1.JPEG|Now Smith here]] Now '''Smith''' here While remaining upright may be the primary goal of beginning riders While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders", parser.BoldTitle(@"[[Image:1.JPEG|Now Smith here]] Now Smith here While remaining upright may be the primary goal of beginning riders While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders", "Smith", out noChangeBack));
            Assert.IsFalse(noChangeBack);
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

            Assert.AreEqual("Foo is a bar, now '''Foo''' here While remaining upright may be the primary goal of beginning riders",
                            parser.BoldTitle("Foo is a bar, now [[Foo]] here While remaining upright may be the primary goal of beginning riders", "Foo", out noChangeBack));
            Assert.IsFalse(noChangeBack);

            Assert.AreEqual("Foo is a bar, now '''foo''' here While remaining upright may be the primary goal of beginning riders, foo here",
                            parser.BoldTitle("Foo is a bar, now [[foo]] here While remaining upright may be the primary goal of beginning riders, foo here", "Foo", out noChangeBack));
            Assert.IsFalse(noChangeBack);
            Assert.AreEqual("'''Foo''' is a [[bar]] While remaining upright may be the primary goal of beginning riders",
                            parser.BoldTitle("[[Foo]] is a [[bar]] While remaining upright may be the primary goal of beginning riders", "Foo", out noChangeBack));
            Assert.IsFalse(noChangeBack);

            // no change
            Assert.AreEqual("'''Foo''' is a bar, now [[Foo]] here While remaining upright may be the primary goal of beginning riders",
                            parser.BoldTitle("'''Foo''' is a bar, now [[Foo]] here While remaining upright may be the primary goal of beginning riders", "Foo", out noChangeBack));
            Assert.IsTrue(noChangeBack);

            // limitation: can't hide image descriptions without hiding the self links too:
            Assert.AreEqual(@"[[Image:Head of serpent column.jpg|250px|thumb|'''Kukulkan''' at the base of the west face of the northern stairway of [[El Castillo, Chichen Itza]]]]

[[Image:ChichenItzaEquinox.jpg|250px|thumb|Kukulkan at Chichen Itza during the [[Equinox]]. The famous decent of the snake March 2009]]

'''Kukulkan''' (''Plumed Serpent'', ''Feathered Serpent'') is a god in the pantheon of [[Maya mythology]]. In [[Yucatec]] the name is spelt '''K'uk'ulkan''' and in [[Tzotzil language|Tzotzil]] it is '''K'uk'ul-chon'''.<ref>Freidel et al 1993, p.289.</ref> The depiction of the [[Feathered Serpent (deity)|feathered serpent deity]] is present in other cultures of [[Mesoamerica]] and Kukulkan is closely related to ''[[Gukumatz]]'' of the [[K'iche']] Maya tradition and ''[[Quetzalcoatl]]'' of [[Aztec mythology]].<ref>Read & Gonzalez 2000, pp.180-2.</ref> Little is known of the mythology of the [[pre-Columbian]] deity.<ref>Read & Gonzalez 2000, p.201.</ref>

Although heavily Mexicanised, Kukulkan has his origins among the Maya of the [[Mesoamerican chronology|Classic Period]], when he was known as ''Waxaklahun Ubah Kan'', the War Serpent, and he has been identified as the Postclassic version of the [[Vision Serpent]] of Classic Maya art.<ref>Freidel et al 1993, pp.289, 325, 441n26.</ref>

The cult of Kukulkan/Quetzalcoatl was the first Mesoamerican religion to transcend the old Classic Period linguistic and ethnic divisions.<ref>Sharer & Traxler 2006, pp582-3.</ref> This cult facilitated communication and peaceful trade among peoples of many different social and ethnic backgrounds.<ref>Sharer & Traxler 2006, pp582-3.</ref> Although the cult was originally centred on the ancient city of [[Chichen Itza|Chichén Itzá]] in the modern [[Mexico|Mexican]] state of [[Yucatán]], it spread as far as the [[Guatemala]]n highlands.<ref>Sharer & Traxler 2006, p.619.</ref>

In Yucatán, references to the deity Kukulkan are confused by references to a named individual who bore the name of the god. Because of this, the distinction between the two has become blurred.<ref>Miller & Taube 1993, p.142.</ref> This individual appears to have been a ruler or priest at Chichen Itza, who first appeared around the 10th century.<ref>Read & González 2000, p.201.</ref>
Although Kukulkan was mentioned as a historical person by Maya writers of the 16th century, the earlier 9th century texts at Chichen Itza never identified him as human and artistic representations depicted him as a Vision Serpent entwined around the figures of nobles.<ref>Freidel et al 1993, p.325.</ref> At Chichen Itza, Kukulkan is also depicted presiding over sacrifice scenes.<ref>Freidel et al 1993, p.478n60.</ref>

Sizeable temples to Kukulkan are found at archaeological sites throughout the north of the [[Yucatán Peninsula]], such as Chichen Itza, [[Uxmal]] and [[Mayapan]].<ref>Read & González 2000, p.201.</ref>

==Etymology==
The Yucatec form of the name is formed from the word", parser.BoldTitle(@"[[Image:Head of serpent column.jpg|250px|thumb|[[Kukulkan]] at the base of the west face of the northern stairway of [[El Castillo, Chichen Itza]]]]

[[Image:ChichenItzaEquinox.jpg|250px|thumb|Kukulkan at Chichen Itza during the [[Equinox]]. The famous decent of the snake March 2009]]

[[Kukulkan]] (''Plumed Serpent'', ''Feathered Serpent'') is a god in the pantheon of [[Maya mythology]]. In [[Yucatec]] the name is spelt '''K'uk'ulkan''' and in [[Tzotzil language|Tzotzil]] it is '''K'uk'ul-chon'''.<ref>Freidel et al 1993, p.289.</ref> The depiction of the [[Feathered Serpent (deity)|feathered serpent deity]] is present in other cultures of [[Mesoamerica]] and Kukulkan is closely related to ''[[Gukumatz]]'' of the [[K'iche']] Maya tradition and ''[[Quetzalcoatl]]'' of [[Aztec mythology]].<ref>Read & Gonzalez 2000, pp.180-2.</ref> Little is known of the mythology of the [[pre-Columbian]] deity.<ref>Read & Gonzalez 2000, p.201.</ref>

Although heavily Mexicanised, Kukulkan has his origins among the Maya of the [[Mesoamerican chronology|Classic Period]], when he was known as ''Waxaklahun Ubah Kan'', the War Serpent, and he has been identified as the Postclassic version of the [[Vision Serpent]] of Classic Maya art.<ref>Freidel et al 1993, pp.289, 325, 441n26.</ref>

The cult of Kukulkan/Quetzalcoatl was the first Mesoamerican religion to transcend the old Classic Period linguistic and ethnic divisions.<ref>Sharer & Traxler 2006, pp582-3.</ref> This cult facilitated communication and peaceful trade among peoples of many different social and ethnic backgrounds.<ref>Sharer & Traxler 2006, pp582-3.</ref> Although the cult was originally centred on the ancient city of [[Chichen Itza|Chichén Itzá]] in the modern [[Mexico|Mexican]] state of [[Yucatán]], it spread as far as the [[Guatemala]]n highlands.<ref>Sharer & Traxler 2006, p.619.</ref>

In Yucatán, references to the deity Kukulkan are confused by references to a named individual who bore the name of the god. Because of this, the distinction between the two has become blurred.<ref>Miller & Taube 1993, p.142.</ref> This individual appears to have been a ruler or priest at Chichen Itza, who first appeared around the 10th century.<ref>Read & González 2000, p.201.</ref>
Although Kukulkan was mentioned as a historical person by Maya writers of the 16th century, the earlier 9th century texts at Chichen Itza never identified him as human and artistic representations depicted him as a Vision Serpent entwined around the figures of nobles.<ref>Freidel et al 1993, p.325.</ref> At Chichen Itza, Kukulkan is also depicted presiding over sacrifice scenes.<ref>Freidel et al 1993, p.478n60.</ref>

Sizeable temples to Kukulkan are found at archaeological sites throughout the north of the [[Yucatán Peninsula]], such as Chichen Itza, [[Uxmal]] and [[Mayapan]].<ref>Read & González 2000, p.201.</ref>

==Etymology==
The Yucatec form of the name is formed from the word", "Kukulkan", out noChangeBack));
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

            // boldtitle delinks all self links in lead section
            Assert.AreEqual(@"{{Unreferenced|date=October 2007}}
'''Steve Cook''' is a songwriter for Sovereign Grace. While remaining upright may be the primary goal of beginning riders. While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders While remaining upright may be the primary goal of beginning riders '''Steve Cook'''
While remaining upright may be the primary goal of beginning riders While remaining upright may be the primary goal of beginning riders",
                            parser.BoldTitle(@"{{Unreferenced|date=October 2007}}
[[Steve Cook]] is a songwriter for Sovereign Grace. While remaining upright may be the primary goal of beginning riders. While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders While remaining upright may be the primary goal of beginning riders [[Steve Cook]]
While remaining upright may be the primary goal of beginning riders While remaining upright may be the primary goal of beginning riders", "Steve Cook", out noChangeBack));
            Assert.IsFalse(noChangeBack);

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_10#Piped_self-link_delinking_bug
            Assert.AreEqual(@"The '''2009 Indian Premier League''' While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders| 2009<br>", Parsers.FixLinks(parser.BoldTitle(@"The 2009 Indian Premier League While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders| [[2009 Indian Premier League|2009]]<br>", "2009 Indian Premier League", out noChangeBack), "2009 Indian Premier League", out noChangeBack));
        }

        [Test]
        // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#If_a_selflink_is_also_bolded.2C_AWB_should_just_remove_the_selflink
        public void SelfLinksWithBold()
        {
            Assert.AreEqual(@"'''Marie-Madeleine-Marguerite d'Aubray, Marquise de Brinvilliers'''", parser.BoldTitle(@"'''[[Marie-Madeleine-Marguerite d'Aubray, Marquise de Brinvilliers]]'''", @"Marie-Madeleine-Marguerite d'Aubray, Marquise de Brinvilliers", out noChangeBack));

            Assert.AreEqual(@"'''foo'''", parser.BoldTitle(@"'''[[foo]]'''", @"Foo", out noChangeBack));

            // don't remove italics
            Assert.AreEqual(@"'''''Marie-Madeleine-Marguerite d'Aubray, Marquise de Brinvilliers'''''", parser.BoldTitle(@"''[[Marie-Madeleine-Marguerite d'Aubray, Marquise de Brinvilliers]]''", @"Marie-Madeleine-Marguerite d'Aubray, Marquise de Brinvilliers", out noChangeBack));
        }

        [Test]
        public void BoldedSelfLinks()
        {
            Assert.AreEqual(@"{{Infobox Album <!-- See Wikipedia:WikiProject_Albums -->
| Name        = ''It Crawled into My Hand, Honest''
| Type        = studio
| Artist      = [[The Fugs]]
| Cover       =
| Released    = 1968
| Recorded    = 1968
| Genre       = [[Rock and roll|Rock]], [[protopunk]], [[Psychedelic music|psychedelic]] [[Folk-rock]]
| Length      =
| Label       = [[Folkways Records|Folkways]]<br />[[ESP-Disk]]
| Producer    = [[Ed Sanders]], [[Harry Everett Smith|Harry Smith]]
| Reviews     =
| Last album  = ''[[Tenderness Junction]]''<br />(1968)
| This album  = '''''It Crawled into My Hand, Honest'''''<br />(1968)
| Next album  = ''[[Belle of Avenue A]]''<br /> (1969)
}}", parser.BoldTitle(@"{{Infobox Album <!-- See Wikipedia:WikiProject_Albums -->
| Name        = ''It Crawled into My Hand, Honest''
| Type        = studio
| Artist      = [[The Fugs]]
| Cover       =
| Released    = 1968
| Recorded    = 1968
| Genre       = [[Rock and roll|Rock]], [[protopunk]], [[Psychedelic music|psychedelic]] [[Folk-rock]]
| Length      =
| Label       = [[Folkways Records|Folkways]]<br />[[ESP-Disk]]
| Producer    = [[Ed Sanders]], [[Harry Everett Smith|Harry Smith]]
| Reviews     =
| Last album  = ''[[Tenderness Junction]]''<br />(1968)
| This album  = '''''[[It Crawled into My Hand, Honest]]'''''<br />(1968)
| Next album  = ''[[Belle of Avenue A]]''<br /> (1969)
}}", "It Crawled into My Hand, Honest", out noChangeBack));

            Assert.AreEqual(@"<noinclude> = '''''[[It Crawled into My Hand, Honest]]'''''<br /> </noinlcude>", parser.BoldTitle(@"<noinclude> = '''''[[It Crawled into My Hand, Honest]]'''''<br /> </noinlcude>", "[[It Crawled into My Hand, Honest]]", out noChangeBack));
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
            bool noChangeBack;
            Assert.AreEqual(@"'''Foo''' is great. Foo is cool", Parsers.FixLinks(@"'''Foo''' is great. [[Foo]] is cool", "Foo", out noChangeBack));
            Assert.IsFalse(noChangeBack);
            Assert.AreEqual(@"'''Foo''' is great. Foo is cool and foo now", Parsers.FixLinks(@"'''Foo''' is great. [[Foo]] is cool and [[foo]] now", "Foo", out noChangeBack));
            Assert.IsFalse(noChangeBack);
            Assert.AreEqual(@"'''Foo''' is great. Foo is cool and bar now", Parsers.FixLinks(@"'''Foo''' is great. [[Foo]] is cool and [[foo|bar]] now", "Foo", out noChangeBack));
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

            // don't apply within imagemaps
            Assert.AreEqual(@"<imagemap> [[foo]] </imagemap>", Parsers.FixLinks(@"<imagemap> [[foo]] </imagemap>", "foo", out noChangeBack));
            Assert.IsTrue(noChangeBack);

            // don't apply if has a noinclude etc.
            Assert.AreEqual(@"<noinclude> [[foo]] </noinclude>", Parsers.FixLinks(@"<noinclude> [[foo]] </noinclude>", "foo", out noChangeBack));
            Assert.IsTrue(noChangeBack);

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs#Incorrect_delinking_of_article_title
            Assert.AreEqual("foo bar", Parsers.FixLinks("[[foo bar]]", "Foo bar", out noChangeBack));
            Assert.AreEqual("Foo bar", Parsers.FixLinks("[[Foo bar]]", "foo bar", out noChangeBack));
        }

        [Test]
        public void MoreSelfLinkRemoval()
        {
            bool noChangeBack;

            Assert.AreEqual("Thorsten", Parsers.FixLinks("[[Þorsteins saga Víkingssonar| Thorsten]]", "Þorsteins saga Víkingssonar", out noChangeBack));

            // convert to bold in infobox album/single
            Assert.AreEqual(@"{{Infobox Single
| Name = Feels So Right
| Cover =
| Artist =  [[Alabama (band)|Alabama]]
| from Album = [[Feels So Right (album)|Feels So Right]]
| A-side         = ""Feels So Right""
| B-side         = ""See the Embers, Feel the Flame""
| Released = May 1, 1981 <small>([[United States|U.S.]])</small>
| Format         = [[7-inch single|7""]]
| Recorded       = August 26, 1980
| Genre = [[Country music|Country]]
| Length         = 3:36
| Label          = [[RCA Records]] <small>12236</small>
| Writer = [[Randy Owen]]
| Producer       = [[Harold Shedd]] and [[Alabama (band)|Alabama]]
| Last single = ""[[Old Flame (song)|Old Flame]]""<br />(1981)
| This single = ""'''Feels So Right'''"" <br />(1981)
| Next single = ""[[Love in the First Degree (Alabama song)|Love in the First Degree]]"" <br />(1981)
}}", Parsers.FixLinks(@"{{Infobox Single
| Name = Feels So Right
| Cover =
| Artist =  [[Alabama (band)|Alabama]]
| from Album = [[Feels So Right (album)|Feels So Right]]
| A-side         = ""Feels So Right""
| B-side         = ""See the Embers, Feel the Flame""
| Released = May 1, 1981 <small>([[United States|U.S.]])</small>
| Format         = [[7-inch single|7""]]
| Recorded       = August 26, 1980
| Genre = [[Country music|Country]]
| Length         = 3:36
| Label          = [[RCA Records]] <small>12236</small>
| Writer = [[Randy Owen]]
| Producer       = [[Harold Shedd]] and [[Alabama (band)|Alabama]]
| Last single = ""[[Old Flame (song)|Old Flame]]""<br />(1981)
| This single = ""[[Feels So Right (song)|Feels So Right]]"" <br />(1981)
| Next single = ""[[Love in the First Degree (Alabama song)|Love in the First Degree]]"" <br />(1981)
}}", @"Feels So Right (song)", out noChangeBack));

            Assert.AreEqual(@"{{Infobox Single
| Name = Feels So Right
| Cover =
| Artist =  [[Alabama (band)|Alabama]]
| from Album = [[Feels So Right (album)|Feels So Right]]
| A-side         = ""Feels So Right""
| B-side         = ""See the Embers, Feel the Flame""
| Released = May 1, 1981 <small>([[United States|U.S.]])</small>
| Format         = [[7-inch single|7""]]
| Recorded       = August 26, 1980
| Genre = [[Country music|Country]]
| Length         = 3:36
| Label          = [[RCA Records]] <small>12236</small>
| Writer = [[Randy Owen]]
| Producer       = [[Harold Shedd]] and [[Alabama (band)|Alabama]]
| Last single = ""[[Old Flame (song)|Old Flame]]""<br />(1981)
| This single = ""'''Feels So Right'''"" <br />(1981)
| Next single = ""[[Love in the First Degree (Alabama song)|Love in the First Degree]]"" <br />(1981)
}}", Parsers.FixLinks(@"{{Infobox Single
| Name = Feels So Right
| Cover =
| Artist =  [[Alabama (band)|Alabama]]
| from Album = [[Feels So Right (album)|Feels So Right]]
| A-side         = ""Feels So Right""
| B-side         = ""See the Embers, Feel the Flame""
| Released = May 1, 1981 <small>([[United States|U.S.]])</small>
| Format         = [[7-inch single|7""]]
| Recorded       = August 26, 1980
| Genre = [[Country music|Country]]
| Length         = 3:36
| Label          = [[RCA Records]] <small>12236</small>
| Writer = [[Randy Owen]]
| Producer       = [[Harold Shedd]] and [[Alabama (band)|Alabama]]
| Last single = ""[[Old Flame (song)|Old Flame]]""<br />(1981)
| This single = ""[[Feels So Right]]"" <br />(1981)
| Next single = ""[[Love in the First Degree (Alabama song)|Love in the First Degree]]"" <br />(1981)
}}", @"Feels So Right", out noChangeBack));

            Assert.AreEqual(@"{{Infobox Album  <!-- See Wikipedia:WikiProject_Albums -->
|Name        = Feels So Right
|Type        = [[Album]]
|Artist      = [[Alabama (band)|Alabama]]
|Cover       = Alabama - Feels So Right.jpg
|Released    = [[1981 in country music|1981]]</br>[[July 7]], [[1987]] (re-release)
|Recorded    = | [[1980 in country music|1980]]
|Genre       = [[Country music|Country]]
|Length      = 34:59
|Label       = [[RCA Records]]
|Producer    = Larry McBride, [[Harold Shedd]], Alabama
|Reviews     = *[[Allmusic]] {{Rating|4|5}} [http://allmusic.com/cg/amg.dll?p=amg&token=&sql=10:kjfixql5ldae]
|Chronology = [[Alabama (band)|Alabama]]
|Last album  = ''[[My Home's in Alabama (album)|My Home's In Alabama]]''<br />(1980)
|This album  = '''''Feels So Right'''''<br />(1981)
|Next album  = ''[[Mountain Music (album)|Mountain Music]]''<br />(1982)
}}", Parsers.FixLinks(@"{{Infobox Album  <!-- See Wikipedia:WikiProject_Albums -->
|Name        = Feels So Right
|Type        = [[Album]]
|Artist      = [[Alabama (band)|Alabama]]
|Cover       = Alabama - Feels So Right.jpg
|Released    = [[1981 in country music|1981]]</br>[[July 7]], [[1987]] (re-release)
|Recorded    = | [[1980 in country music|1980]]
|Genre       = [[Country music|Country]]
|Length      = 34:59
|Label       = [[RCA Records]]
|Producer    = Larry McBride, [[Harold Shedd]], Alabama
|Reviews     = *[[Allmusic]] {{Rating|4|5}} [http://allmusic.com/cg/amg.dll?p=amg&token=&sql=10:kjfixql5ldae]
|Chronology = [[Alabama (band)|Alabama]]
|Last album  = ''[[My Home's in Alabama (album)|My Home's In Alabama]]''<br />(1980)
|This album  = ''[[Feels So Right (album)|Feels So Right]]''<br />(1981)
|Next album  = ''[[Mountain Music (album)|Mountain Music]]''<br />(1982)
}}", @"Feels So Right (album)", out noChangeBack));
        }

        [Test]
        public void BareReferencesTests()
        {
            Assert.IsTrue(Parsers.HasBareReferences(@"
foo
==References==
* http://www.site.com
"));

            Assert.IsTrue(Parsers.HasBareReferences(@"
foo
==References==
* http://www.site.com
==External links==
* http://www.site2.com
"));

            Assert.IsTrue(Parsers.HasBareReferences(@"
foo
==References==
* [http://www.site.com/site a site]
* http://www.site.com
==External links==
* http://www.site2.com
"));

            Assert.IsTrue(Parsers.HasBareReferences(@"
foo
==References==
* http://www.site.com
* [http://www.site.com/site a site]
==External links==
* http://www.site2.com
"));

            Assert.IsTrue(Parsers.HasBareReferences(@"
foo
==External links==
* http://www.site2.com
==References==
* http://www.site.com
* [http://www.site.com/site a site]
"));

            Assert.IsFalse(Parsers.HasBareReferences(@"
foo
==References==
* [http://www.site.com/site a site]
==External links==
* http://www.site2.com
"));

            Assert.IsFalse(Parsers.HasBareReferences(@"
foo
* [http://www.site.com/site a site]
==External links==
* http://www.site2.com
"));

            Assert.IsFalse(Parsers.HasBareReferences(@""));
            Assert.IsFalse(Parsers.HasBareReferences(@"foo"));
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

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#zero-width_space
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
        [Test, Ignore("Known Failing")]
        public void IsCorrectEditSummary()
        {
            // too long
            StringBuilder sb = new StringBuilder(300);
            for (int i = 0; i < 256; i++) sb.Append('x');
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

            Assert.IsFalse(Parsers.IsCorrectEditSummary("[[123456789 123456789 123456789 1[[WP:AWB]]"));
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

            // remove duplicate defaultsorts
            Assert.AreEqual(@"foo

[[Category:Businesspeople]]

{{DEFAULTSORT:Phillips, James M.}}
foo", Parsers.ChangeToDefaultSort(@"foo
{{DEFAULTSORT:Phillips, James M.}}
[[Category:Businesspeople]]

{{DEFAULTSORT:Phillips, James M.}}
foo", "Hi", out noChange));

            Assert.AreEqual(@"foo

[[Category:Businesspeople]]


{{DEFAULTSORT:Phillips, James M.}}
foo", Parsers.ChangeToDefaultSort(@"foo
{{DEFAULTSORT:Phillips, James M.}}
[[Category:Businesspeople]]

{{DEFAULTSORT:Phillips, James M.}}
{{DEFAULTSORT:Phillips, James M.}}
foo", "Hi", out noChange));

            // don't remove duplicate different defaultsorts
            Assert.AreEqual(@"foo
{{DEFAULTSORT:Phillips, James M.}}
[[Category:Businesspeople]]

{{DEFAULTSORT:Fred}}
foo", Parsers.ChangeToDefaultSort(@"foo
{{DEFAULTSORT:Phillips, James M.}}
[[Category:Businesspeople]]

{{DEFAULTSORT:Fred}}
foo", "Hi", out noChange));

            Assert.AreEqual(@"
foo {{persondata}}
[[Category:1910 births]]
[[Category:Australian players of Australian rules football]]
[[Category:Essendon Football Club players]]

{{DEFAULTSORT:Lahiff, Tommy}}", Parsers.ChangeToDefaultSort(@"
foo {{persondata}}
[[Category:1910 births|Lahiff, Tommy]]
[[Category:Australian players of Australian rules football|Lahiff, Tommy]]
[[Category:Essendon Football Club players|Lahiff, Tommy]]
", "foo", out noChange, false));

            // can't add a DEFAULTSORT using existing cat sortkeys even if restricted, as sortkey case may be changed
            const string a = @"
foo {{persondata}}
[[Category:1910 births|Lahiff, Tommy]]
[[Category:Australian players of Australian rules football|Lahiff, Tommy]]
[[Category:Essendon Football Club players|Lahiff, Tommy]]
";
            Assert.AreEqual(a, Parsers.ChangeToDefaultSort(a, "foo", out noChange, true));

            Assert.AreEqual(@"
foo {{persondata}}
[[Category:1910 births]]
[[Category:Australian players of Australian rules football]]
[[Category:Essendon Football Club players]]

{{DEFAULTSORT:Lahiff, Tommy}}", Parsers.ChangeToDefaultSort(a, "foo", out noChange, false));

            // can't add a DEFAULTSORT using existing cat sortkeys if they're different
            Assert.AreEqual(@"
foo {{persondata}}
[[Category:1910 births|Lahiff, Tommy]]
[[Category:Australian players of Australian rules football|Lahiff, Tommy]]
[[Category:Essendon Football Club players|TOmmy]]
", Parsers.ChangeToDefaultSort(@"
foo {{persondata}}
[[Category:1910 births|Lahiff, Tommy]]
[[Category:Australian players of Australian rules football|Lahiff, Tommy]]
[[Category:Essendon Football Club players|TOmmy]]
", "foo", out noChange, true));

            // restricted
            const string r1 = @"[[Category:Franks]]
[[Category:Carolingian dynasty]]
[[Category:Frankish people]]
[[Category:811 deaths]]
[[Category:9th-century rulers]]";
            Assert.AreEqual(r1, Parsers.ChangeToDefaultSort(r1, "foo", out noChange, true));

            // namespace not used in DEFAULTSORT key
            Assert.AreEqual(@"foo
[[Category:All foos]]
{{DEFAULTSORT:Special Foos}}", Parsers.ChangeToDefaultSort(@"foo
[[Category:All foos]]", "Category:Special foos", out noChange, false));
        }

        [Test]
        public void LifetimeDiacritics()
        {
            bool noChange;

            Assert.AreEqual(@"{{Lifetime|1900|2000|Smithe, Fred}}",
                            Parsers.ChangeToDefaultSort(@"{{Lifetime|1900|2000|Smithé, Fred}}", "Hi", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual(@"{{Lifetime|1900|2000|Smithe, Fred}}",
                            Parsers.ChangeToDefaultSort(@"{{Lifetime|1900|2000|Smithe, Fred}}", "Hi", out noChange));
            Assert.IsTrue(noChange);
        }

        [Test]
        public void CategorySortKeyPartialCleaning()
        {
            // to test that if a cat's sortkey is the start of the defaultsort key, it's removed too
            bool noChange;
            Assert.AreEqual(@"{{DEFAULTSORT:Willis, Bobby}}
[[Category:1999 deaths]]
[[Category:1942 births]]
[[Category:Cancer deaths in the United Kingdom]]", Parsers.ChangeToDefaultSort(@"{{DEFAULTSORT:Willis, Bobby}}
[[Category:1999 deaths|Willis]]
[[Category:1942 births]]
[[Category:Cancer deaths in the United Kingdom]]", "Bobby Willis", out noChange));

            Assert.IsFalse(noChange);

            Assert.AreEqual(@"{{DEFAULTSORT:Willis, Bobby}}
[[Category:1999 deaths]]
[[Category:1942 births|Foo]]
[[Category:Cancer deaths in the United Kingdom]]", Parsers.ChangeToDefaultSort(@"{{DEFAULTSORT:Willis, Bobby}}
[[Category:1999 deaths|Willis]]
[[Category:1942 births|Foo]]
[[Category:Cancer deaths in the United Kingdom]]", "Bobby Willis", out noChange));

            Assert.IsFalse(noChange);
        }

        [Test]
        public void ChangeToDefaultSortHuman()
        {
            bool noChange;

            const string a = @"Fred Smith blah [[Category:Living people]]";
            const string b = "\r\n" + @"{{DEFAULTSORT:Smith, Fred}}";

            Assert.AreEqual(a + b, Parsers.ChangeToDefaultSort(a, "Fred Smith", out noChange));
            Assert.IsFalse(noChange);

            // no defaultsort added if restricted defaultsort addition on
            Assert.AreEqual(a, Parsers.ChangeToDefaultSort(a, "Fred Smith", out noChange, true));
            Assert.IsTrue(noChange);

            const string c = @"Stéphanie Mahieu blah [[Category:Living people]]";
            const string d = "\r\n" + @"{{DEFAULTSORT:Mahieu, Stephanie}}";

            Assert.AreEqual(c + d, Parsers.ChangeToDefaultSort(c, "Stéphanie Mahieu", out noChange));
            Assert.IsFalse(noChange);
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

        [Test]
        public void TestIsArticleAboutAPerson()
        {
            Assert.IsTrue(Parsers.IsArticleAboutAPerson(@"Foo {{Lifetime|||smith}}", "foo"));
            Assert.IsTrue(Parsers.IsArticleAboutAPerson(@"Foo {{persondata|name=smith}}", "foo"));
            Assert.IsTrue(Parsers.IsArticleAboutAPerson(@"Foo [[Category:1900 deaths]]", "foo"));
            Assert.IsTrue(Parsers.IsArticleAboutAPerson(@"Foo [[Category:1900 births]]", "foo"));
            Assert.IsTrue(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Living people]]", "foo"));
            Assert.IsTrue(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Living people|Smith]]", "foo"));
            Assert.IsTrue(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{England-bio-stub}}", "foo"));
            Assert.IsTrue(Parsers.IsArticleAboutAPerson(@"Some words {{death date and age|1960|01|9}}", "foo"));
            Assert.IsTrue(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{RefimproveBLP}}", "foo"));
            Assert.IsTrue(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "foo"));

            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Married couples]] {{persondata|name=smith}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:2002 animal births]] {{persondata|name=smith}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo {{In-universe}} {{persondata|name=smith}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo {{in-universe}} {{persondata|name=smith}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Multiple people]] {{persondata|name=smith}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Fictional blah]] {{persondata|name=smith}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[fictional character]] {{persondata|name=smith}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[fictional character|character]] {{persondata|name=smith}}", "foo"));
            
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo {{Infobox fraternity|f=a}} {{Lifetime|||smith}}", "foo"));

            // multiple birth dates means not about one person
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"{{nat fs player|no=1|pos=GK|name=[[Meg]]|age={{Birth date|1956|01|01}} ({{Age at date|1956|01|01|1995|6|5}})|caps=|club=|clubnat=}}
{{nat fs player|no=2|pos=MF|name=[[Valeria]]|age={{Birth date|1968|09|03}} ({{Age at date|1968|09|03|1995|6|5}})|caps=|club=|clubnat=}}", "foo"));

            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"{{see also|Fred}} Fred Smith is great == foo == {{persondata}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"{{Main|Fred}} Fred Smith is great == foo == {{persondata}}", "foo"));

            // link in bold in zeroth section to somewhere else is no good
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''military career of [[Napoleon Bonaparte]]''' == foo == {{birth date|2008|11|11}}", "foo"));

            // 'characters' category means fictional person
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"foo [[Category:227 characters]] {{persondata}}", "foo"));
        }

        [Test, Ignore("Unused"), Category("Incomplete")]
        public void ExternalURLToInternalLink()
        {
            //TODO:MOAR
            Assert.AreEqual("", Parsers.ExternalURLToInternalLink(""));
            //Assert.AreEqual(" ", Parsers.ExternalURLToInternalLink("%20"));

            Assert.AreEqual("https://secure.wikimedia.org/otrs/index.pl?Action=AgentTicketQueue",
                            Parsers.ExternalURLToInternalLink(
                                "https://secure.wikimedia.org/otrs/index.pl?Action=AgentTicketQueue"));

            Assert.AreEqual("[[ru:Foo|Foo]]",
                            Parsers.ExternalURLToInternalLink("[http://ru.wikipedia.org/wiki/Foo Foo]"));

            Assert.AreEqual("[[m:Test|Test]]",
                            Parsers.ExternalURLToInternalLink("[http://meta.wikimedia.org/wiki/Test Test]"));
            Assert.AreEqual("[[commons:Test|Test]]",
                            Parsers.ExternalURLToInternalLink("[http://commons.wikimedia.org/wiki/Test Test]"));
            Assert.AreEqual("[[w:Test|Test]]",
                            Parsers.ExternalURLToInternalLink("[http://en.wikipedia.org/wiki/Test Test]"));
            Assert.AreEqual("[[wikt:Test|Test]]",
                            Parsers.ExternalURLToInternalLink("[http://en.wiktionary.org/wiki/Test Test]"));
            Assert.AreEqual("[[n:Test|Test]]",
                            Parsers.ExternalURLToInternalLink("[http://en.wikinews.org/wiki/Test Test]"));
            Assert.AreEqual("[[b:Test|Test]]",
                            Parsers.ExternalURLToInternalLink("[http://en.wikibooks.org/wiki/Test Test]"));
            Assert.AreEqual("[[q:Test|Test]]",
                            Parsers.ExternalURLToInternalLink("[http://en.wikiquote.org/wiki/Test Test]"));
            Assert.AreEqual("[[s:Test|Test]]",
                            Parsers.ExternalURLToInternalLink("[http://en.wikisource.org/wiki/Test Test]"));
            Assert.AreEqual("[[v:Test|Test]]",
                            Parsers.ExternalURLToInternalLink("[http://en.wikiversity.org/wiki/Test Test]"));
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
            
            string bug1 = @"
==References==
<references />

{{Northampton County, Pennsylvania}}

[[Category:Boroughs in Pennsylvania]]
[[Category:Northampton County, Pennsylvania]]
[[Category:Settlements established in 1790]]

[[ht:Tatamy, Pennsilvani]]
[[nl:Tatamy]]
[[pt:Tatamy]]
[[vo:Tatamy]]";
            Assert.IsFalse(Parsers.HasRefAfterReflist(bug1));
            
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

            Assert.IsFalse(Parsers.IsMissingReferencesDisplay(@"{{Reflist|refs=
<ref name=modern>{{cite news |first=William }}
        }}"));
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
        public void HasStubTemplate()
        {
            Assert.IsTrue(Parsers.HasStubTemplate(@"foo {{foo stub}}"));
            Assert.IsTrue(Parsers.HasStubTemplate(@"foo {{foo-stub}}"));

            Assert.IsFalse(Parsers.HasStubTemplate(@"foo {{foo tubs}}"));
        }

        [Test]
        public void IsStub()
        {
            Assert.IsTrue(Parsers.IsStub(@"foo {{foo stub}}"));
            Assert.IsTrue(Parsers.IsStub(@"foo {{foo-stub}}"));

            // short article
            Assert.IsTrue(Parsers.IsStub(@"foo {{foo tubs}}"));

            const string a = @"fooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo";
            const string b = a + a + a + a + a + a;
            Assert.IsFalse(Parsers.IsStub(b + b + b + b));
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

            Assert.IsTrue(Parsers.CheckNoBots("{{bots|deny=Xenobot Mk V}}", "Xenobot"));
            Assert.IsTrue(Parsers.CheckNoBots("{{bots|deny=Xenobot}}", "Xenobot Mk V"));

            Assert.IsFalse(Parsers.CheckNoBots("{{nobots}}", ""));
            Assert.IsFalse(Parsers.CheckNoBots("{{bots|deny=all}}", ""));
            Assert.IsFalse(Parsers.CheckNoBots("{{bots|deny=awb}}", ""));
            Assert.IsFalse(Parsers.CheckNoBots("{{bots|deny=awb,test}}", ""));
            Assert.IsFalse(Parsers.CheckNoBots("{{bots|deny=awb,test}}", "test"));
            Assert.IsFalse(Parsers.CheckNoBots("{{bots|deny=test}}", "test"));
            Assert.IsFalse(Parsers.CheckNoBots("{{bots|allow=none}}", ""));
        }

        [Test]
        public void NoIncludeIncludeOnlyProgrammingElement()
        {
            Assert.IsTrue(Parsers.NoIncludeIncludeOnlyProgrammingElement(@"<noinclude>blah</noinclude>"));
            Assert.IsTrue(Parsers.NoIncludeIncludeOnlyProgrammingElement(@"<includeonly>blah</includeonly>"));
            Assert.IsTrue(Parsers.NoIncludeIncludeOnlyProgrammingElement(@"{{{1}}}"));
            Assert.IsTrue(Parsers.NoIncludeIncludeOnlyProgrammingElement(@"{{{3}}}"));

            Assert.IsFalse(Parsers.NoIncludeIncludeOnlyProgrammingElement(@"hello"));
            Assert.IsFalse(Parsers.NoIncludeIncludeOnlyProgrammingElement(@""));
        }

        [Test]
        public void GetTemplateTests()
        {
            Assert.AreEqual(@"{{foo}}", Parsers.GetTemplate(@"now {{foo}} was here", "foo"));
            Assert.AreEqual(@"{{Foo}}", Parsers.GetTemplate(@"now {{Foo}} was here", "foo"));
            Assert.AreEqual(@"{{foo}}", Parsers.GetTemplate(@"now {{foo}} was here", "Foo"));
            Assert.AreEqual(@"{{foo}}", Parsers.GetTemplate(@"now {{foo}} was here", "[Ff]oo"));
            Assert.AreEqual(@"{{ foo|bar asdfasdf}}", Parsers.GetTemplate(@"now {{ foo|bar asdfasdf}} was here", "foo"));
            Assert.AreEqual(@"{{ foo |bar asdfasdf}}", Parsers.GetTemplate(@"now {{ foo |bar asdfasdf}} was here", "foo"));
            Assert.AreEqual(@"{{ foo|bar
asdfasdf}}", Parsers.GetTemplate(@"now {{ foo|bar
asdfasdf}} was here", "foo"));

            Assert.AreEqual(@"", Parsers.GetTemplate(@"now {{ foo|bar asdfasdf}} was here", "foot"));
            Assert.AreEqual(@"", Parsers.GetTemplate(@"now {{ foo|bar asdfasdf}} was here", ""));
            Assert.AreEqual(@"", Parsers.GetTemplate(@"", "foo"));

            Assert.AreEqual(@"{{foo  |a={{bar}} here}}", Parsers.GetTemplate(@"now {{foo  |a={{bar}} here}} was here", "foo"));
        }

        [Test]
        public void GetTemplatesTests()
        {
            const string foo1 = "{{foo|a}}";
            const string foo2 = "{{foo|b}}";
            string text = @"now " + foo1 + " and " + foo2;

            Regex foo = new Regex(@"{{foo.*?}}");
            MatchCollection fred = foo.Matches(text);

            Assert.AreEqual(fred.ToString(), Parsers.GetTemplates(text, "foo").ToString());
            Assert.AreEqual(fred.ToString(), Parsers.GetTemplates(text, "Foo").ToString());
            MatchCollection templates = Parsers.GetTemplates(text, "foo");
            Assert.AreEqual(foo1, templates[0].Value);
            Assert.AreEqual(foo2, templates[1].Value);
            Assert.AreEqual(2, templates.Count);

            // ignores commeted out templates
            templates = Parsers.GetTemplates(text + @" <!-- {{foo|c}} -->", "foo");
            Assert.AreEqual(foo1, templates[0].Value);
            Assert.AreEqual(foo2, templates[1].Value);
            Assert.AreEqual(2, templates.Count);

            // ignores nowiki templates
            templates = Parsers.GetTemplates(text + @" <nowiki> {{foo|c}} </nowiki>", "foo");
            Assert.AreEqual(foo1, templates[0].Value);
            Assert.AreEqual(foo2, templates[1].Value);
            Assert.AreEqual(2, templates.Count);

            // nested templates caught
            const string foo3 = @"{{ Foo|bar={{abc}}|beer=y}}";
            templates = Parsers.GetTemplates(@"now " + foo3 + @" there", "foo");
            Assert.AreEqual(foo3, templates[0].Value);

            // whitespace ignored
            const string foo4 = @"{{ Foo }}";
            templates = Parsers.GetTemplates(@"now " + foo4 + @" there", "foo");
            Assert.AreEqual(foo4, templates[0].Value);

            // no matches here
            templates = Parsers.GetTemplates(@"now " + foo3 + @" there", "fo");
            Assert.AreEqual(0, templates.Count);
        }

        [Test]
        public void FixUnicode()
        {
            // http://en.wikipedia.org/wiki/Wikipedia:AWB/B#Line_break_insertion
            Assert.AreEqual("foo bar", parser.FixUnicode("foo\x2028bar"));
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
                        Is.StringMatching(@"\[\[Category:Foo[ _]bar\]\]"));
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
        public void ReplacementSortkeys()
        {
            bool noChange;

            Assert.AreEqual("[[Category:Bar|key]]", Parsers.ReCategoriser("Foo", "Bar", "[[Category:Foo|key]]", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("[[Category:Bar|key here]]", Parsers.ReCategoriser("Foo", "Bar", "[[Category:Foo|key here]]", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("[[Category:Bar|key]]", Parsers.ReCategoriser("Foo", "Bar", "[[Category:Foo|key]]", out noChange, false));
            Assert.IsFalse(noChange);

            Assert.AreEqual("[[Category:Bar]]", Parsers.ReCategoriser("Foo", "Bar", "[[Category:Foo|key]]", out noChange, true));
            Assert.IsFalse(noChange);

            Assert.AreEqual("[[Category:Bar]]", Parsers.ReCategoriser("Foo", "Bar", "[[Category:Foo|key here]]", out noChange, true));
            Assert.IsFalse(noChange);
            
            //
            Assert.AreEqual(" [[Category:Bar|key]]", Parsers.ReCategoriser("Foo", "Bar", "[[Category:Foo|key]] [[Category:Bar|key]]", out noChange));
            Assert.IsFalse(noChange);
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

        private const string Uncat = "{{Uncategorized|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}",
        UncatStub = "{{Uncategorizedstub|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}",
        Orphan = "{{orphan|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}",
        Wikify = "{{Wikify|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}",
        Deadend = "{{Deadend|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}",
        Stub = "{{stub}}";

        [Test]
        public void Add()
        {
            Globals.UnitTestIntValue = 0;
            Globals.UnitTestBoolValue = true;

            string text = parser.Tagger(ShortText, "Test", out noChange, ref summary);
            //Stub, no existing stub tag. Needs all tags
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text));
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(text));
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(text));
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text));

            Assert.IsFalse(text.Contains(UncatStub));

            text = parser.Tagger(ShortText + Stub + Uncat + Wikify + Orphan + Deadend, "Test", out noChange, ref summary);
            //Tagged article, dupe tags shouldn't be added
            Assert.AreEqual(1, Tools.RegexMatchCount(Regex.Escape(Stub), text));
            Assert.AreEqual(1, Tools.RegexMatchCount(Regex.Escape(Uncat), text));
            Assert.AreEqual(1, Tools.RegexMatchCount(Regex.Escape(Wikify), text));
            Assert.AreEqual(1, Tools.RegexMatchCount(Regex.Escape(Orphan), text));
            Assert.AreEqual(1, Tools.RegexMatchCount(Regex.Escape(Deadend), text));

            text = parser.Tagger(ShortText + Stub, "Test", out noChange, ref summary);
            //Stub, existing stub tag
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text));
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(text));
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text));
            Assert.IsTrue(text.Contains(UncatStub));
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text));

            Assert.IsFalse(text.Contains(Uncat));

            Assert.AreEqual(1, Tools.RegexMatchCount(Regex.Escape(Stub), text));

            text = parser.Tagger(ShortText + ShortText, "Test", out noChange, ref summary);
            //Not a stub
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text));
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(text));
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(text));

            Assert.IsFalse(text.Contains(UncatStub));
            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text));

            // with categories and no links, Deadend not removed
            Globals.UnitTestIntValue = 3;
            text = parser.Tagger(Deadend + ShortText, "Test", out noChange, ref summary);
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text));
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(text));
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text));
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text));

            Globals.UnitTestIntValue = 5;

            text = parser.Tagger(ShortText, "Test", out noChange, ref summary);
            //Categorised Stub
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text));
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(text));
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text));
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text));

            Assert.IsFalse(text.Contains(UncatStub));
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text));

            text = parser.Tagger(ShortText + ShortText, "Test", out noChange, ref summary);
            //Categorised Page
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text));
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(text));
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text));

            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text));
            Assert.IsFalse(text.Contains(UncatStub));
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text));

            Globals.UnitTestBoolValue = false;

            text = parser.Tagger(ShortText, "Test", out noChange, ref summary);
            //Non orphan categorised stub
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(text));
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text));
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text));

            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));
            Assert.IsFalse(text.Contains(UncatStub));
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text));

            text = parser.Tagger(ShortText + ShortText, "Test", out noChange, ref summary);
            //Non orphan categorised page
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(text));
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text));

            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text));
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));
            Assert.IsFalse(text.Contains(UncatStub));
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text));

            text = parser.Tagger(ShortText.Replace("consectetur", "[[consectetur]]"), "Test", out noChange, ref summary);
            //Non Deadend stub
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text));

            Assert.IsFalse(WikiRegexes.Wikify.IsMatch(text));
            Assert.IsFalse(WikiRegexes.DeadEnd.IsMatch(text));
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));
            Assert.IsFalse(text.Contains(UncatStub));
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text));

            text = parser.Tagger(Regex.Replace(ShortText, @"(\w+)", "[[$1]]"), "Test", out noChange, ref summary);
            //very wikified stub
            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text));
            Assert.IsFalse(WikiRegexes.Wikify.IsMatch(text));
            Assert.IsFalse(WikiRegexes.DeadEnd.IsMatch(text));
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));
            Assert.IsFalse(text.Contains(UncatStub));
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text));

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_10#Autotagging_and_Articleissues
            // do not add orphan where article already has orphan tag within {{Article issues}}

            text = parser.Tagger(@"{{Article issues|orphan=May 2008|cleanup=May 2008|story=May 2008}}\r\n" + ShortText, "Test", out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));
            Assert.IsTrue(WikiRegexes.OrphanArticleIssues.IsMatch(text));
        }

        [Test]
        public void AddOrphan()
        {
            // orphan not added if disambig found – skips since a template present
            string text = parser.Tagger(ShortText + @"{{for|foo|bar}}", "Test", out noChange, ref summary);
            //Stub, no existing stub tag. Needs all tags
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));
        }

        private const string ShortText =
            @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Curabitur sit amet tortor nec neque faucibus pharetra. Fusce lorem arcu, tempus et, imperdiet a, commodo a, pede. Nulla sit amet turpis gravida elit dictum cursus. Praesent tincidunt velit eu urna.";

        private const string LongText =
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
            string text = parser.Tagger(ShortText + Stub, "Test", out noChange, ref summary);
            //Stub, tag not removed
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text));

            text = parser.Tagger(LongText + Stub, "Test", out noChange, ref summary);
            //stub tag removed
            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text));

            text = parser.Tagger("{{wikify}}" + Regex.Replace(LongText, @"(\w+)", "[[$1]]"), "Test", out noChange, ref summary);
            //wikify tag removed
            Assert.IsFalse(WikiRegexes.Wikify.IsMatch(text));

            Globals.UnitTestIntValue = 4;
            text = parser.Tagger("{{uncategorised}}", "Test", out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text));

            text = parser.Tagger("{{uncategorised|date=January 2009}}", "Test", out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text));

            text = parser.Tagger("{{uncategorised|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}", "Test", out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text));

            Globals.UnitTestBoolValue = false;

            text = parser.Tagger("{{orphan}}", "Test", out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));

            Globals.UnitTestBoolValue = true;

            text = parser.Tagger("{{orphan}}", "Test", out noChange, ref summary);
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text));
        }
        
        [Test]
        public void RemoveExpand()
        {
            Assert.IsFalse(WikiRegexes.Expand.IsMatch(parser.Tagger(@"foo {{expand}} {{bio-stub}}", "Test", out noChange, ref summary)));
        }
        
        [Test]
        public void RemoveDeadEnd()
        {
            Globals.UnitTestIntValue = 0;
            
            Assert.IsFalse(WikiRegexes.DeadEnd.IsMatch(parser.Tagger(@"foo {{deadend}} [[a]] and [[b]] and [[b]]", "Test", out noChange, ref summary)));
            Assert.IsTrue(summary.Contains("removed deadend tag"));
        }

        [Test]
        public void UpdateFactTag()
        {
            //Test of updating some of the non dated tags
            string text = parser.Tagger("{{fact}}", "Test", out noChange, ref summary);

            Assert.IsTrue(text.Contains("{{fact|date={{subst:CURRENTMONTHNAME}}"));
            Assert.IsFalse(text.Contains("{{fact}}"));

            text = parser.Tagger("{{template:fact}}", "Test", out noChange, ref summary);

            Assert.IsTrue(text.Contains("{{fact|date={{subst:CURRENTMONTHNAME}}"));
            Assert.IsFalse(text.Contains("{{fact}}"));
        }
        
        [Test]
        public void UpdateCitationNeededTag()
        {
            string text = parser.Tagger("{{citation needed}}", "Test", out noChange, ref summary);
            Assert.AreEqual(text, @"{{citation needed|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");

            text = parser.Tagger("{{template:citation needed  }}", "Test", out noChange, ref summary);
            Assert.AreEqual(text, @"{{citation needed|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");
        }
        
        [Test]
        public void UpdateWikifyTag()
        {
            string correct =  @"{{Wikify|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}";
            string text = parser.Tagger("{{wikify}}", "Test", out noChange, ref summary);
            Assert.IsTrue(text.Contains(correct));
            Assert.IsFalse(text.Contains("{{wikify}}"));

            text = parser.Tagger("{{template:wikify  }}", "Test", out noChange, ref summary);
            Assert.IsTrue(text.Contains(correct));
            
            text = parser.Tagger("{{wikify|section}}", "Test", out noChange, ref summary);
            Assert.IsTrue(text.Contains(@"{{Wikify|section|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"));
        }

        [Test]
        public void General()
        {
            Assert.AreEqual("#REDIRECT [[Test]]", parser.Tagger("#REDIRECT [[Test]]", "Test", out noChange, ref summary));
            Assert.IsTrue(noChange);

            Assert.AreEqual(ShortText, parser.Tagger(ShortText, "Talk:Test", out noChange, ref summary));
            Assert.IsTrue(noChange);

            //No change as no add/remove
            Assert.AreEqual("{{Test Template}}", parser.Tagger("{{Test Template}}", "Test", out noChange, ref summary));
            Assert.IsTrue(noChange);

            // {{Pt}} is now {{Pt icon}}
            Assert.AreEqual("hello {{pt}} hello", parser.Tagger("hello {{pt}} hello", "Test", out noChange, ref summary));
            Assert.IsTrue(noChange);

            Assert.AreEqual("hello {{Pt}} hello", parser.Tagger("hello {{Pt}} hello", "Test", out noChange, ref summary));
            Assert.IsTrue(noChange);
        }

        [Test]
        public void ConversionsTests()
        {
            Assert.AreEqual(@"{{cleanup|date=January 2008}} Article text here", Parsers.Conversions(@"{{articleissues|cleanup=January 2008}} Article text here"));
            Assert.AreEqual(@"{{cleanup|date=January 2008}} Article text here", Parsers.Conversions(@"{{article issues|cleanup=January 2008}} Article text here"));
            Assert.AreEqual(@"{{cleanup|date=January 2008}} Article text here", Parsers.Conversions(@"{{article issues|cleanup=January 2008
}} Article text here"));
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
            Assert.AreEqual(@"{{Commons category|XXX}}", Parsers.Conversions(@"{{commons|Category:XXX}}"));
            Assert.AreEqual(@"{{Commons category|XXX}}", Parsers.Conversions(@"{{Commons|category:XXX}}"));
            Assert.AreEqual(@"{{Commons category|XXX}}", Parsers.Conversions(@"{{Commons| category:XXX }}"));
            Assert.AreEqual(@"{{Commons category|Backgammon}}", Parsers.Conversions(@"{{commons|Category:Backgammon|Backgammon}}"));
            Assert.AreEqual(@"{{Commons category|Backgammon}}", Parsers.Conversions(@"{{commons|Category:Backgammon | Backgammon  }}"));
            Assert.AreEqual(@"{{Commons category|Backgammon|Backgammon main}}", Parsers.Conversions(@"{{Commons|Category:Backgammon|Backgammon main}}"));
            Assert.AreEqual(@"{{commons cat|Gander International Airport}}", Parsers.Conversions(@"{{commons cat|Gander International Airport|Gander International Airport}}"));
            Assert.AreEqual(@"{{Commons cat|Gander International Airport}}", Parsers.Conversions(@"{{Commons cat|Gander International Airport|Gander International Airport}}"));

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
        public void ConversionTestsInterwikiMigration()
        {
            Assert.AreEqual(@"{{hello}}", Parsers.Conversions(@"{{msg:hello}}"));
            Assert.AreEqual(@"[[zh:foo]]", Parsers.Conversions(@"[[zh-tw:foo]]"));
            Assert.AreEqual(@"[[no:foo]]", Parsers.Conversions(@"[[nb:foo]]"));
            Assert.AreEqual(@"[[da:foo]]", Parsers.Conversions(@"[[dk:foo]]"));
        }
        [Test]
        public void PageNameTests()
        {
            Assert.AreEqual(@"{{subst:PAGENAME}}", Parsers.Conversions(@"{{PAGENAME}}"));
            Assert.AreEqual(@"{{subst:PAGENAMEE}}", Parsers.Conversions(@"{{PAGENAMEE}}"));
            Assert.AreEqual(@"{{subst:PAGENAME}}", Parsers.Conversions(@"{{template:PAGENAME}}"));
            Assert.AreEqual(@"{{subst:BASEPAGENAME}}", Parsers.Conversions(@"{{BASEPAGENAME}}"));
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

            Assert.AreEqual(@"{{Article issues|wikify=May 2008|POV=May 2008|Expand=June 2008}}", Parsers.Conversions(@"{{Article issues|wikify=May 2008|POV=May 2008|Expand=June 2008|Expand=June 2008}}"));
            Assert.AreEqual(@"{{Article issues|wikify=May 2008|POV=May 2008|Expand=June 2008}}", Parsers.Conversions(@"{{Articleissues|wikify=May 2008|Expand=June 2008|POV=May 2008|Expand=June 2008}}"));

            Assert.AreEqual(@"{{Article issues|wikify=May 2008|Expand=June 2008|POV=May 2008}}", Parsers.Conversions(@"{{Article issues|wikify=May 2008|Expand=June 2008|POV=May 2008|Expand=}}"));
            Assert.AreEqual(@"{{Article issues|wikify=May 2008|POV=May 2008|Expand=June 2008}}", Parsers.Conversions(@"{{Article issues|wikify=May 2008|Expand=|POV=May 2008|Expand=June 2008}}"));
            
            Assert.AreEqual(@"{{Article issues|wikify=May 2008|POV=May 2008|Expand=June 2008|Expand=June 2009}}", Parsers.Conversions(@"{{Article issues|wikify=May 2008|POV=May 2008|Expand=June 2008|Expand=June 2009}}"));
        }



        [Test]
        public void CitationNeededRedirectTests()
        {
            Assert.AreEqual(@"{{citation needed}}", Parsers.Conversions(@"{{citation needed}}"));
            Assert.AreEqual(@"{{Citation needed}}", Parsers.Conversions(@"{{fact}}"));
            Assert.AreEqual(@"{{Citation needed}}", Parsers.Conversions(@"{{ Fact}}"));
            Assert.AreEqual(@"{{Citation needed}}", Parsers.Conversions(@"{{Cn}}"));
            Assert.AreEqual(@"{{Citation needed}}", Parsers.Conversions(@"{{proveit}}"));
            Assert.AreEqual(@"{{Citation needed}}", Parsers.Conversions(@"{{citeneeded}}"));
            Assert.AreEqual(@"{{Citation needed|date=May 2009}}", Parsers.Conversions(@"{{fact|date=May 2009}}"));
            Assert.AreEqual(@"{{Citation needed
|date=May 2009}}", Parsers.Conversions(@"{{fact
|date=May 2009}}"));
        }

        [Test]
        public void ArticleIssues()
        {
            const string a1 = @"{{Wikify}} {{expand}}", a2 = @" {{COI}}", a3 = @" the article";
            const string a4 = @" {{COI|date=May 2008}}", a5 = @"{{Article issues|POV|prose|spam}} ";
            const string a4A = @" {{COI|Date=May 2008}}";

            // adding new {{article issues}}
            Assert.IsTrue(parser.ArticleIssues(a1 + a2 + a3).Contains(@"{{Article issues|wikify|expand|COI}}"));
            Assert.IsTrue(parser.ArticleIssues(a1 + a4 + a3).Contains(@"{{Article issues|wikify|expand|COI date=May 2008}}"));

            // amend existing {{article issues}}
            Assert.IsTrue(parser.ArticleIssues(a5 + a1 + a2 + a3).Contains(@"{{Article issues|POV|prose|spam|wikify|expand|COI"));
            Assert.IsTrue(parser.ArticleIssues(a5 + a1 + a4 + a3).Contains(@"{{Article issues|POV|prose|spam|wikify|expand|COI date=May 2008}}"));
            Assert.IsTrue(parser.ArticleIssues(a5 + a1 + a4A + a3).Contains(@"{{Article issues|POV|prose|spam|wikify|expand|COI Date=May 2008}}"));

            // insufficient tags
            Assert.IsFalse(Parsers.Conversions(a1 + a3).Contains(@"{{Article issues"));

            // before first heading tag can be used
            const string a7 = @"{{trivia}} ==heading==";
            Assert.IsTrue(parser.ArticleIssues(a5 + a3 + a7).Contains(@"{{Article issues|POV|prose|spam|trivia}}"));

            // don't grab tags in later sections of article
            const string a6 = @"==head== {{essay}}";
            Assert.AreEqual(a5 + a3 + a6, parser.ArticleIssues(a5 + a3 + a6));

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

            // tags with a parameter value that's not a date are not supported
            Assert.AreEqual(@"{{Article issues|wikfy=May 2008|copyedit=April 2009|COI=May 2008}} {{update|some date reason}}", parser.ArticleIssues(@"{{Article issues|wikfy=May 2008|copyedit=April 2009|COI=May 2008}} {{update|some date reason}}"));

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#ArgumentException_in_Parsers.ArticleIssues
            Assert.AreEqual("", parser.ArticleIssues(""));

            const string bug1 = @"{{article issues|disputed=June 2009|primarysources=June 2009}}
{{Expert}}";
            Assert.AreEqual(bug1, parser.ArticleIssues(bug1));

            const string bug2 = @"{{article issues|article=y
|update=November 2008
|out of date=July 2009
|cleanup=July 2009
|wikify=July 2009}}";

            Assert.AreEqual(bug2, Parsers.Conversions(bug2));
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
            const string a1 = @"{{Article issues|wikfy=May 2008|COI=May 2008|expert}}";
            Assert.AreEqual(a1, Parsers.Conversions(a1));

            const string a2 = @"{{Article issues|wikfy=May 2008|COI=May 2008|expert=Fred}}";
            Assert.AreEqual(a2, Parsers.Conversions(a2));

            // don't remove 'update'field
            const string a3 = @"{{Article issues|wikfy=May 2008|COI=May 2008|update=May 2008}}";
            Assert.AreEqual(a3, Parsers.Conversions(a3));
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
