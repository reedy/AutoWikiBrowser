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

using System.Text.RegularExpressions;
using NUnit.Framework;
using WikiFunctions;
using WikiFunctions.Parse;
using System.Collections.Generic;

namespace UnitTests
{
    [TestFixture]
    public class FootnotesTests : RequiresInitialization
    {
        [Test]
        // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_6#Unexpected_modification
        public void TestTagBoundaries()
        {
            Assert.AreEqual("<ref name=\"foo\"><br></ref>", Parsers.SimplifyReferenceTags("<ref name=\"foo\"><br></ref>"));
        }

        [Test]
        public void TestSimplifyReferenceTags()
        {
            Assert.AreEqual("<ref name= \"foo\" />", Parsers.SimplifyReferenceTags("<ref name= \"foo\"></ref>"));
            Assert.AreEqual("<ref name = \"foo\" />", Parsers.SimplifyReferenceTags("<ref name = \"foo\"></ref>"));
            Assert.AreEqual("<ref name=\"foo\" />", Parsers.SimplifyReferenceTags("<ref name=\"foo\" >< / ref >"));
            Assert.AreEqual("<ref name=\"foo\" />", Parsers.SimplifyReferenceTags("<ref name=\"foo\" ></ref>"));

            Assert.AreEqual(@"<ref name=""foo bar"" />", Parsers.SimplifyReferenceTags(@"<ref name=""foo bar"" ></ref>"));
            Assert.AreEqual(@"<ref name='foo bar' />", Parsers.SimplifyReferenceTags(@"<ref name='foo bar' >    </ref>"));

            Assert.AreEqual("<refname=\"foo\"></ref>", Parsers.SimplifyReferenceTags("<refname=\"foo\"></ref>"));

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#Reference_bugs
            Assert.AreEqual(@"<ref name='aftermath' />", Parsers.SimplifyReferenceTags(@"<ref name='aftermath'> </ref>"));
            Assert.AreEqual(@"<ref name='aftermath' />", Parsers.SimplifyReferenceTags(@"<ref name='aftermath' /> </ref>"));
        }

        [Test]
        public void TestFixReferenceListTags()
        {
            Assert.AreEqual("<references/>", Parsers.FixReferenceListTags("<references/>"));
            Assert.AreEqual("<div><references/></div>", Parsers.FixReferenceListTags("<div><references/></div>"));

            Assert.AreEqual("<references>foo</references>", Parsers.FixReferenceListTags("<references>foo</references>"));

            Assert.AreEqual("{{Reflist}}", Parsers.FixReferenceListTags("<div class=\"references-small\"><references/>\r\n</div>"));
            Assert.AreEqual("{{Reflist|2}}", Parsers.FixReferenceListTags("<div class=\"references-2column\"><references/></div>"));
            Assert.AreEqual("{{Reflist|2}}",
                            Parsers.FixReferenceListTags(@"<div class=""references-2column""><div class=""references-small"">
<references/></div></div>"));
            Assert.AreEqual("{{Reflist|2}}",
                            Parsers.FixReferenceListTags(@"<div class=""references-small""><div class=""references-2column""> <references/>
</div></div>"));

            // evil don't do's
            Assert.IsFalse(Parsers.FixReferenceListTags(@"<div class=""references-small""><div class=""references-2column"">
<references/></div>* some other ref</div>").Contains("{{Reflist"));
            Assert.IsFalse(Parsers.FixReferenceListTags(@"<div class=""references-small""><div class=""references-2column"">
<references/></div>").Contains("{{Reflist"));

            Assert.AreEqual("{{Reflist|2}}",
                            Parsers.FixReferenceListTags(@"<div class=""references-small"" style=""-moz-column-count:2; column-count:2;"">
<references/>
</div>"), @"Converts to reflist|2 when column-count:2");

       Assert.AreEqual("{{Reflist}}", Parsers.FixReferenceListTags(@"<small><references/></small>"));
       Assert.AreEqual("{{Reflist}}", Parsers.FixReferenceListTags(@"<small><references /></small>"));

#if DEBUG
            Variables.SetProjectLangCode("sv");
            Assert.AreEqual("<references/>", Parsers.FixReferenceListTags("<div class=\"references-small\"><references/>\r\n</div>"));

            Variables.SetProjectLangCode("en");
            Assert.AreEqual("{{Reflist}}", Parsers.FixReferenceListTags("<div class=\"references-small\"><references/>\r\n</div>"));
#endif
        }
        

        [Test]
        public void TestFixReferenceTags()
        {
            // whitespace cleaning
            Assert.AreEqual(@"now <ref>[http://www.site.com a site]</ref> was", Parsers.FixReferenceTags(@"now < ref>[http://www.site.com a site]</ref> was"),"leading");
            Assert.AreEqual(@"now <ref>[http://www.site.com a site]</ref> was", Parsers.FixReferenceTags(@"now < ref   >[http://www.site.com a site]</ref> was"),"both sides");
            Assert.AreEqual(@"now <ref>[http://www.site.com a site]</ref> was", Parsers.FixReferenceTags(@"now <ref >[http://www.site.com a site]</ref> was"), "trailing");
            Assert.AreEqual(@"now <ref>[http://www.site.com a site]</ref> was", Parsers.FixReferenceTags(@"now < ref>[http://www.site.com a site]< /ref> was"),"leading in borth opening and closing tag");
            Assert.AreEqual(@"now <ref>[http://www.site.com a site]</ref> was", Parsers.FixReferenceTags(@"now <ref>[http://www.site.com a site]</ ref> was"),"in closing tag");
            Assert.AreEqual(@"now <ref>[http://www.site.com a site]</ref> was", Parsers.FixReferenceTags(@"now <ref>[http://www.site.com a site]</ref > was"),"in closing tag");
            Assert.AreEqual(@"now <ref>[http://www.site.com a site]</ref> was", Parsers.FixReferenceTags(@"now <    ref  >[http://www.site.com a site]< / ref > was"),"everywhere");

            // <ref name=foo bar> --> <ref name="foo bar">
            Assert.AreEqual(@"now <ref name=""foo bar"">and", Parsers.FixReferenceTags(@"now <ref name=foo bar>and"));
            Assert.AreEqual(@"now <ref name=""foo bar"" /> and", Parsers.FixReferenceTags(@"now <ref name=foo bar /> and"));
            Assert.AreEqual(@"now <ref name = ""foo bar"" >and", Parsers.FixReferenceTags(@"now <ref name = foo bar >and"));

            const string nochange = @"<ref name=VulgarisAerae1
/>";
            Assert.AreEqual(nochange, Parsers.FixReferenceTags(nochange));

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
            Assert.AreEqual(@"now <ref name=""foo bar"">and", Parsers.FixReferenceTags(@"now <ref =""foo bar"">and"));

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

            // <REF> and <Ref> to <ref>
            Assert.AreEqual(@"now <ref>Smith 2004</ref>", Parsers.FixReferenceTags(@"now <ref>Smith 2004</ref>"));
            Assert.AreEqual(@"now <ref>Smith 2004</ref>", Parsers.FixReferenceTags(@"now <Ref>Smith 2004</ref>"));
            Assert.AreEqual(@"now <ref>Smith 2004</ref>", Parsers.FixReferenceTags(@"now <REF>Smith 2004</REF>"));
            Assert.AreEqual(@"now <ref>Smith 2004</ref>", Parsers.FixReferenceTags(@"now <reF>Smith 2004</ref>"));
            Assert.AreEqual(@"now <ref name=S1>Smith 2004</ref>", Parsers.FixReferenceTags(@"now <reF name=S1>Smith 2004</ref>"));
            Assert.AreEqual(@"now <ref>Smith 2004</ref>", Parsers.FixReferenceTags(@"now <REF> Smith 2004 </REF>"));

            // removal of spaces between consecutive references
            Assert.AreEqual(@"<ref>foo</ref><ref>foo2</ref>", Parsers.FixReferenceTags(@"<ref>foo</ref> <ref>foo2</ref>"));
            Assert.AreEqual(@"<ref>foo</ref><ref>foo2</ref>", Parsers.FixReferenceTags(@"<ref>foo</ref>     <ref>foo2</ref>"));
            Assert.AreEqual(@"<ref>foo</ref><ref name=Bert />", Parsers.FixReferenceTags(@"<ref>foo</ref> <ref name=Bert />"));
            Assert.AreEqual(@"<ref>foo</ref><ref name=Bert>Bert2</ref>", Parsers.FixReferenceTags(@"<ref>foo</ref> <ref name=Bert>Bert2</ref>"));
            Assert.AreEqual(@"<ref>foo</ref><ref>foo2</ref>", Parsers.FixReferenceTags(@"<REF>foo</REF> <REF>foo2</REF>"));

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
            Assert.AreEqual(@"Now clearly.<ref name=Smith/> 2 were", Parsers.FixReferenceTags(@"Now clearly. <ref name=Smith/> 2 were"));
            Assert.AreEqual(@"Now clearly.<ref name=Smith/> 2 were", Parsers.FixReferenceTags(@"Now clearly. <Ref name=Smith/> 2 were"));

            // trailing spaces in reference
            Assert.AreEqual(@"<ref>foo</ref>", Parsers.FixReferenceTags(@"<ref>foo </ref>"));
            Assert.AreEqual(@"<ref>foo</ref>", Parsers.FixReferenceTags(@"<ref>foo    </ref>"));
            Assert.AreEqual(@"<ref>foo
</ref>", Parsers.FixReferenceTags(@"<ref>foo
</ref>"));
            Assert.AreEqual(@"<ref>foo</ref>", Parsers.FixReferenceTags(@"<ref>  foo </ref>"));
            Assert.AreEqual(@"<ref>foo</ref>", Parsers.FixReferenceTags(@"<REF>  foo </REF>"));

            // empty tags
            Assert.AreEqual(@"", Parsers.FixReferenceTags(@"<ref> </ref>"));
            Assert.AreEqual(@"", Parsers.FixReferenceTags(@"<ref><ref> </ref></ref>"));

            // <ref name="Fred" Smith> --> <ref name="Fred Smith">
            Assert.AreEqual(@"<ref name=""Fred Smith"" />", Parsers.FixReferenceTags(@"<ref name=""Fred"" Smith />"));
            Assert.AreEqual(@"<ref name = ""Fred Smith"" />", Parsers.FixReferenceTags(@"<ref name = ""Fred"" Smith />"));
            Assert.AreEqual(@"<ref name=""Fred-Smith"" />", Parsers.FixReferenceTags(@"<ref name=""Fred""-Smith />"));
            Assert.AreEqual(@"<ref name=""Fred-Smith""/>", Parsers.FixReferenceTags(@"<ref name=""Fred""-Smith/>"));
            Assert.AreEqual(@"<ref name=""Fred-Smith"" >text</ref>", Parsers.FixReferenceTags(@"<ref name=""Fred""-Smith >text</ref>"));

            // <ref name-"Fred"> --> <ref name="Fred">
            Assert.AreEqual(@"<ref name=""Fred"" />", Parsers.FixReferenceTags(@"<ref name-""Fred"" />"));
            Assert.AreEqual(@"<ref name=""Fred"">", Parsers.FixReferenceTags(@"<ref name-""Fred"">"));
            Assert.AreEqual(@"<ref name=Fred />", Parsers.FixReferenceTags(@"<ref name-Fred />"));
            Assert.AreEqual(@"< ref name=""Fred"" />", Parsers.FixReferenceTags(@"< ref name-""Fred"" />"));

            // <ref NAME= --> <ref name=
            Assert.AreEqual(@"<ref name=""Fred"" />", Parsers.FixReferenceTags(@"< ref NAME=""Fred"" />"),"ref NAME");
            Assert.AreEqual(@"<ref name =""Fred"" />", Parsers.FixReferenceTags(@"<ref NAME =""Fred"" />"),"ref NAME");

            // <refname= --> <ref name=
            Assert.AreEqual(@"<ref name=""Fred"" />", Parsers.FixReferenceTags(@"<refname=""Fred"" />"),"refname");

            // <ref name=> --> <ref>
            Assert.AreEqual(@"<ref>", Parsers.FixReferenceTags(@"<ref name=>"));
            Assert.AreEqual(@"<ref>", Parsers.FixReferenceTags(@"<ref name =  >"));

#if DEBUG
            Variables.SetProjectLangCode("zh");
            Assert.AreEqual(@"</ref>some text", Parsers.FixReferenceTags(@"</ref> some text"), "Chinese put no space before/after the reference tags");
            Assert.AreEqual(@"<ref name=""foo"" />some text", Parsers.FixReferenceTags(@"<ref name=""foo"" />      some text"), "Chinese put no space before/after the reference tags");

            Variables.SetProjectLangCode("en");
            Assert.AreEqual(@"</ref> some text", Parsers.FixReferenceTags(@"</ref> some text"), "English put a space after the reference tags");
#endif
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

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#Re-ordering_references_can_leave_page_number_templates_behind.
            // have to allow {{rp}} template after a reference
            Assert.AreEqual(@"'''Article''' is great.<ref name = 'Fred9'>So says Fred</ref><ref name = 'John3' /><ref name = 'Tim1'>ABC</ref>
Article started off pretty good, <ref name = ""Fred9"" /><ref name = 'John3' >So says John</ref> <ref name = 'Tim1'/>{{rp|11}} and finished well.
End of.", Parsers.ReorderReferences(@"'''Article''' is great.<ref name = 'Fred9'>So says Fred</ref><ref name = 'John3' /><ref name = 'Tim1'>ABC</ref>
Article started off pretty good, <ref name = 'Tim1'/>{{rp|11}}<ref name = 'John3' >So says John</ref> <ref name = ""Fred9"" /> and finished well.
End of."));

            Assert.AreEqual(@"'''Article''' is great.<ref name = 'Fred9'>So says Fred</ref><ref name = 'John3' /><ref name = 'Tim1'>ABC</ref>
Article started off pretty good, <ref name = ""Fred9"" /><ref name = 'John3' >So says John</ref> <ref name = 'Tim1'/>{{rp |11}} and finished well.
End of.", Parsers.ReorderReferences(@"'''Article''' is great.<ref name = 'Fred9'>So says Fred</ref><ref name = 'John3' /><ref name = 'Tim1'>ABC</ref>
Article started off pretty good, <ref name = 'Tim1'/>{{rp |11}}<ref name = 'John3' >So says John</ref> <ref name = ""Fred9"" /> and finished well.
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
            
            Assert.AreEqual(@"'''Article''' is great.<ref name = 'Fred9'>So says Fred</ref><ref name = 'John3' /><ref name = 'Tim1'>ABC</ref>
Article started off pretty good, <ref name = ""Fred9"" /><ref name = 'John3' >So says John</ref>{{Page needed|needed=y|May 2008}} <ref name = 'Tim1'/> and finished well.
End of.", Parsers.ReorderReferences(@"'''Article''' is great.<ref name = 'Fred9'>So says Fred</ref><ref name = 'John3' /><ref name = 'Tim1'>ABC</ref>
Article started off pretty good, <ref name = 'Tim1'/><ref name = 'John3' >So says John</ref>{{Page needed|needed=y|May 2008}} <ref name = ""Fred9"" /> and finished well.
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

            // bugfix: <br> in ref
            string nochange = @"* [[Algeria]]<ref name=""UNESCO""/><ref name=""oic"">[http://www.sesrtcic.org/members/default.shtml OIC members and Palestine] ''The Statistical, Economic and Social Research and Training Centre for Islamic Countries''<br> [https://english.people.com.cn/200604/14/eng20060414_258351.html OIC members urge recognition of Hamas] ''People's Daily''</ref><ref name=""MEDEA""/>
* [[Angola]]<ref name=""UNESCO"">{{cite web|url=http://unesdoc.unesco.org/images/0008/000827/082711eo.pdf|title=Request for the admission of the State of Palestine to Unesco as a Member State|date=12 May 1989|publisher=[[UNESCO]]|accessdate=2009-08-22}}</ref><ref name=""MEDEA""/>
* [[Benin]]<ref name=""UNESCO""/><ref name=""oic""/><ref name=""MEDEA""/>";

            Assert.AreEqual(nochange, Parsers.ReorderReferences(nochange));
        }

        [Test]
        public void ReorderReferencesNotWithinReflist()
        {
            // not reordered in <references>...</references> etc.

            string a = @"
<references>
<ref>So says John</ref>
<ref name = ""Fred1"" />
</references>", b = @"{{reflist|3|refs=
<ref>So says John</ref>
<ref name = ""Fred1"" />
}}";
            string correctpart = @"'''Article''' is great.<ref name = ""Fred1"">So says Fred</ref>
Article started off pretty good, <ref name = ""Fred1"" /> <ref>So says John</ref> and finished well.
End of.";

            Assert.AreEqual(correctpart + a, Parsers.ReorderReferences(correctpart + a));
            Assert.AreEqual(correctpart + b, Parsers.ReorderReferences(correctpart + b));

#if DEBUG
            Variables.SetProjectLangCode("fr");
            WikiRegexes.MakeLangSpecificRegexes();
            Assert.AreEqual(correctpart + b.Replace("reflist", "références"), Parsers.ReorderReferences(correctpart + b.Replace("reflist", "références")));

            Variables.SetProjectLangCode("en");
            WikiRegexes.MakeLangSpecificRegexes();
            Assert.AreEqual(correctpart + b, Parsers.ReorderReferences(correctpart + b));
#endif
        }

        [Test]
        public void ReorderReferencesRefGroups()
        {
            string refgroup = @"'''Article''' is great.<ref name = ""Fred8"" group=a>So says Fred</ref><ref name = ""John3"" />
Article started off pretty good, <ref>Third</ref><ref name = ""Fred8"" group=a/> and finished well.
End of.";
            Assert.AreEqual(refgroup, Parsers.ReorderReferences(refgroup));
        }

        [Test]
        public void DuplicateNamedReferencesTests()
        {
            // duplicate references fix (both named)
            // Matches
            Assert.AreEqual(@"now<ref name=""Fred"">The Honourable Fred Smith, 2002</ref>but later than<ref name=""Fred""/> was", Parsers.DuplicateNamedReferences(@"now<ref name=""Fred"">The Honourable Fred Smith, 2002</ref>but later than<ref name=""Fred"">The Honourable Fred Smith, 2002</ref> was"));
            Assert.AreEqual(@"now<ref name=""Fred"">The Honourable Fred Smith, 2002</ref>but later than<ref name=""Fred""/> was", Parsers.DuplicateNamedReferences(@"now<ref name=""Fred"">The Honourable Fred Smith, 2002</ref>but later than<ref name=""Fred""> The Honourable Fred Smith, 2002  </ref> was"), "excess whitespace");
            Assert.AreEqual(@"now<ref name=""Fred"" >The Honourable Fred Smith, 2002</ref>but later than<ref name=""Fred""/> was", Parsers.DuplicateNamedReferences(@"now<ref name=""Fred"" >The Honourable Fred Smith, 2002</ref>but later than<ref name = ""Fred"">The Honourable Fred Smith, 2002</ref> was"));
            Assert.AreEqual(@"now<ref name = ""Fred"">The Honourable Fred Smith, 2002 </ref>but later than<ref name=""Fred""/> was", Parsers.DuplicateNamedReferences(@"now<ref name = ""Fred"">The Honourable Fred Smith, 2002 </ref>but later than<ref name=""Fred"">The Honourable Fred Smith, 2002</ref> was"));
            Assert.AreEqual(@"now<ref name=""Fred"">The Honourable Fred Smith, 2002</ref>but later than<ref name=""Fred""/> was", Parsers.DuplicateNamedReferences(@"now<ref name=""Fred"">The Honourable Fred Smith, 2002</ref>but later than<ref name=""Fred"" >The Honourable Fred Smith, 2002< /ref> was"));
            Assert.AreEqual(@"now<ref name='Fred'>The Honourable Fred Smith, 2002</ref>but later than<ref name=""Fred""/> was", Parsers.DuplicateNamedReferences(@"now<ref name='Fred'>The Honourable Fred Smith, 2002</ref>but later than<ref name=""Fred"" >The Honourable Fred Smith, 2002< /ref> was"));
            Assert.AreEqual(@"now<ref name='Fred'>The Honourable Fred Smith, <br>2002</ref>but later than<ref name=""Fred""/> was", Parsers.DuplicateNamedReferences(@"now<ref name='Fred'>The Honourable Fred Smith, <br>2002</ref>but later than<ref name=""Fred"" >The Honourable Fred Smith, <br>2002< /ref> was"));

            Assert.AreEqual(@"now<ref name=""Fred"">The Honourable Fred Smith, 2002</ref> and here<ref name=""Fred""/> but later than<ref name=""Fred""/> was",
                            Parsers.DuplicateNamedReferences(@"now<ref name=""Fred"">The Honourable Fred Smith, 2002</ref> and here<ref name=""Fred"">The Honourable Fred Smith, 2002</ref> but later than<ref name=""Fred"">The Honourable Fred Smith, 2002</ref> was"));

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
            Assert.AreEqual(@"now<ref name=""Bert"">The Honourable Bert Smith, 2002</ref>but later than<ref name=""Bert""/> was", Parsers.DuplicateNamedReferences(@"now<ref name=""Bert"">The Honourable Bert Smith, 2002</ref>but later than<ref> The Honourable Bert Smith, 2002  </ref> was"), "first named excess whitespace");
            Assert.AreEqual(@"now<ref name=""Bert"" >The Honourable Bert Smith, 2002</ref>but later than<ref name=""Bert""/> was", Parsers.DuplicateNamedReferences(@"now<ref name=""Bert"" >The Honourable Bert Smith, 2002</ref>but later than<ref>The Honourable Bert Smith, 2002</ref> was"));
            Assert.AreEqual(@"now<ref name = ""Bert"">The Honourable Bert Smith, 2002 </ref>but later than<ref name=""Bert""/> was", Parsers.DuplicateNamedReferences(@"now<ref name = ""Bert"">The Honourable Bert Smith, 2002 </ref>but later than<ref>The Honourable Bert Smith, 2002</ref> was"));
            Assert.AreEqual(@"now<ref name=""Bert"">The Honourable Bert Smith, 2002</ref>but later than<ref name=""Bert""/> was", Parsers.DuplicateNamedReferences(@"now<ref name=""Bert"">The Honourable Bert Smith, 2002</ref>but later than<ref>The Honourable Bert Smith, 2002< /ref> was"));
            Assert.AreEqual(@"now<ref name='Bert'>The Honourable Bert Smith, 2002</ref>but later than<ref name=""Bert""/> was", Parsers.DuplicateNamedReferences(@"now<ref name='Bert'>The Honourable Bert Smith, 2002</ref>but later than<ref>The Honourable Bert Smith, 2002< /ref> was"));
            Assert.AreEqual(@"now<ref name=""Bert"">The Honourable Bert <small>Smith</small>, 2002</ref>but later than<ref name=""Bert""/> was", Parsers.DuplicateNamedReferences(@"now<ref name=""Bert"">The Honourable Bert <small>Smith</small>, 2002</ref>but later than<ref> The Honourable Bert <small>Smith</small>, 2002  </ref> was"), "nested tag support");

            // no Matches
            const string d = @"now<ref name=""Bert"">The Honourable Bert Smith, 2002</ref>but later than<ref>The Honourable Bert Smith, 2005</ref> was";
            const string e = @"now<ref name=""Bert"">The Honourable Bert Smith, 2004</ref>but later than<ref>The Honourable BERT SMITH, 2004</ref> was";

            Assert.AreEqual(d, Parsers.DuplicateNamedReferences(d));
            Assert.AreEqual(e, Parsers.DuplicateNamedReferences(e)); // reference text casing

            // duplicate references fix (second named)
            Assert.AreEqual(@"now<ref name=""John""/>but later than<ref name=""John"" > The Honourable John Smith, 2002  </ref> was", Parsers.DuplicateNamedReferences(@"now<ref>The Honourable John Smith, 2002</ref>but later than<ref name=""John"" > The Honourable John Smith, 2002  </ref> was"), "second named excess whitespace");
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

            // don't condense a ref in {{reflist}}
            const string RefInReflist = @"Foo<ref name='first'>690nBTivR9o2mJ6vMYccwmjl5TO9BxvhF9deev2VSi17H</ref>here.
            
            ==Notes==
            {{reflist|2|refs=
            
            <ref name='first'>690nBTivR9o2mJ6vMYccwmjl5TO9BxvhF9deev2VSi17H</ref>
            
            }}";

            Assert.AreEqual(RefInReflist, Parsers.DuplicateNamedReferences(RefInReflist));

            // don't condense where same named ref has different values, multiple instances
            const string h = @"now<ref name=""Fred"">The Honourable Fred Smith, 2002</ref>but later than first<ref name=""Fred"">The Honourable Fred Smith, 2002</ref> and second<ref name=""Fred"">The Honourable Fred Smith, 2005</ref> was"; // ref text different year
            Assert.AreEqual(h, Parsers.DuplicateNamedReferences(h));
        }

        [Test]
        public void DuplicateNamedReferencesReparse()
        {
            Assert.AreEqual(@"now<ref name=""Fred"">The Honourable Fred Smith, 2002 wcK63lPneu8pqiK6bxxYMgRSL7ySo4XaOIl4Kr24XB8Hd</ref>but later<ref name=Fred2>text words random 3b3HmI0viQTbmiLIDiGnjTAIX8mpebT520I1 words</ref> than<ref name=""Fred""/> was<ref name=""Fred2""/>",
                            Parsers.DuplicateNamedReferences(@"now<ref name=""Fred"">The Honourable Fred Smith, 2002 wcK63lPneu8pqiK6bxxYMgRSL7ySo4XaOIl4Kr24XB8Hd</ref>but later<ref name=Fred2>text words random 3b3HmI0viQTbmiLIDiGnjTAIX8mpebT520I1 words</ref> than<ref name=""Fred"">The Honourable Fred Smith, 2002 wcK63lPneu8pqiK6bxxYMgRSL7ySo4XaOIl4Kr24XB8Hd</ref> was<ref name=Fred2>text words random 3b3HmI0viQTbmiLIDiGnjTAIX8mpebT520I1 words</ref>"), "reparse used to fix all duplicate refs");
        }

        [Test]
        public void DuplicateUnnamedReferences()
        {
            string namedref = @"now <ref name=foo>foo</ref>";
            Assert.AreEqual("", Parsers.DuplicateUnnamedReferences(""));
            Assert.AreEqual(namedref, Parsers.DuplicateUnnamedReferences(namedref));

            // no existing named ref
            string nonamedref = @"<ref>""bookrags.com""</ref> foo <ref> ""bookrags.com"" </ref>";
            Assert.AreEqual(nonamedref, Parsers.DuplicateUnnamedReferences(nonamedref));

            // existing named ref – good for edit
            Assert.AreEqual(@"<ref name=""bookrags.com"">""bookrags.com""</ref> foo <ref name=""bookrags.com""/>" + namedref, Parsers.DuplicateUnnamedReferences(@"<ref>""bookrags.com""</ref> foo <ref> ""bookrags.com"" </ref>" + namedref), "named ref 1");
            Assert.AreEqual(@"<ref name=""bookrags.com"">""bookrags.com""</ref> foo bar <ref name=""abcde"">abcde</ref> now <ref name=""abcde""/>now <ref name=""bookrags.com""/>" + namedref,
                            Parsers.DuplicateUnnamedReferences(@"<ref>""bookrags.com""</ref> foo bar <ref>abcde</ref> now <ref>abcde</ref>now <ref>""bookrags.com""</ref>" + namedref), "named ref 2");

            Assert.AreEqual(@"<ref name=""ecomodder.com"">http://ecomodder.com/forum/showthread.php/obd-mpguino-gauge-2702.html</ref> foo bar <ref name=""ReferenceA"">http://ecomodder.com/wiki/index.php/MPGuino</ref> now <ref name=""ReferenceA""/>now <ref name=""ecomodder.com""/>" + namedref,
                            Parsers.DuplicateUnnamedReferences(@"<ref>http://ecomodder.com/forum/showthread.php/obd-mpguino-gauge-2702.html</ref> foo bar <ref>http://ecomodder.com/wiki/index.php/MPGuino</ref> now <ref>http://ecomodder.com/wiki/index.php/MPGuino</ref>now <ref>http://ecomodder.com/forum/showthread.php/obd-mpguino-gauge-2702.html</ref>" + namedref), "named ref 3");

            const string Ibid = @"now <ref>ibid</ref> was<ref>ibid</ref> there";
            Assert.AreEqual(Ibid + namedref, Parsers.DuplicateUnnamedReferences(Ibid + namedref));

			const string Pageneeded =@"now <ref>Class 50s in Operation. D Clough {{page needed|date=September 2014}}</ref>  was <ref>Class 50s in Operation. D Clough {{page needed|date=September 2014}}</ref> there";
			Assert.AreEqual(Pageneeded + namedref, Parsers.DuplicateUnnamedReferences(Pageneeded + namedref));

			// nothing to do here
            const string SingleRef = @"now <ref>first</ref> was";
            Assert.AreEqual(SingleRef + namedref, Parsers.DuplicateUnnamedReferences(SingleRef + namedref));

            const string E = @"Foo<ref name=a>a</ref> bar<ref name=a/> was<ref>b</ref> and 1<ref>c</ref> or 2<ref>c</ref>",
            E2 = @"Foo<ref name=a>a</ref> bar<ref name=a/> was<ref>b</ref> and 1<ref name=""ReferenceA"">c</ref> or 2<ref name=""ReferenceA""/>";

            Assert.AreEqual(E2, Parsers.DuplicateUnnamedReferences(E));

            const string F = @"<ref>""book""</ref> foo <ref> ""book"" </ref> <ref name=book>A</ref><ref name=""ReferenceA"">""bookA""</ref><ref name=""ReferenceB"">""bookB""</ref><ref name=""ReferenceC"">""bookC""</ref>";
            Assert.AreEqual(F, Parsers.DuplicateUnnamedReferences(F));
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
            Assert.AreEqual("Christianity Today", Parsers.DeriveReferenceName("a", @"{{cite web | title= Founder of China's Born Again Movement Set Free| work= Christianity Today | url= http://www.christianitytoday.com/ct/2000/augustweb-only/22.0.html| accessdate=2008-02-19}}"));
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
            Assert.AreEqual("Olson 2000", Parsers.DeriveReferenceName("a", @"{{Sfn|Olson|2000 }}"));
            Assert.AreEqual("Olson 2000", Parsers.DeriveReferenceName("a", @"{{Harvard citation no brackets|Olson|2000 }}"));
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

            Assert.AreEqual(@"Baeza, March 2, 1476. p.396", Parsers.DeriveReferenceName("a", @"<ref>The city of Baeza, March 2, 1476. ''Colección de documentos España'', t. XI, p.396</ref>"), "correct date format used");

            Assert.AreEqual(@"pmid123456", Parsers.DeriveReferenceName("a", @"{{cite pmid|123456}}"));
            Assert.AreEqual(@"doi10.1112/123456", Parsers.DeriveReferenceName("a", @"{{cite doi|10.1112/123456}}"));

            Assert.AreEqual("ReferenceA", Parsers.DeriveReferenceName("abc <ref name=dtic.mil>a</ref>", "http://www.dtic.mil/dtic/tr/fulltext/u2/a168577.pdf |ANALYSIS OF M16A2 RIFLE CHARACTERISTICS AND RECOMMENDED IMPROVEMENTS. Arthur D. Osborne. Mellonics Systems Development Division. Litton Systems, Inc. WD and Seward Smith ARI Field Unit at Fort Benning, Georgia. TRAINING RESEARCH LABORATORY. U. S. Army Research Institute for the Behavioral and Social Sciences. February 1986"));
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

            // order reversed
            Assert.AreEqual(@"Foo<ref name=""Jones"">Jones 2005</ref> and bar<ref name=Jones>Jones 2005</ref>",
                            Parsers.SameRefDifferentName(@"Foo<ref name=J5>Jones 2005</ref> and bar<ref name=Jones>Jones 2005</ref>"));

            // leading/trailing whitespace in ref doesn't matter
            Assert.AreEqual(@"Foo<ref name=Jones>Jones 2005</ref> and bar<ref name=""Jones"">Jones 2005   </ref>",
                            Parsers.SameRefDifferentName(@"Foo<ref name=Jones>Jones 2005</ref> and bar<ref name=J5>Jones 2005   </ref>"));

            // don't merge ibid refs
            string nochange = @"Foo<ref name=Jones>ibid</ref> and bar<ref name =A5>ibid</ref>";
            Assert.AreEqual(nochange, Parsers.SameRefDifferentName(nochange));

            // don't rename ref is used in a group ref
            nochange = @"Foo<ref name=Jones group=A/> and bar<ref name =Jones>Text</ref> and <ref name =JonesY>Text</ref>";
            Assert.AreEqual(nochange, Parsers.SameRefDifferentName(nochange));
            nochange = @"Foo<ref name=""Jones"" group=A/> and bar<ref name =""Jones"">Text2</ref> and <ref name =JonesY>Text2</ref>";
            Assert.AreEqual(nochange, Parsers.SameRefDifferentName(nochange));
            nochange = @"Foo<ref name='Jones' group=A/> and bar<ref name =Jones>Text3</ref> and <ref name =JonesY>Text3</ref>";
            Assert.AreEqual(nochange, Parsers.SameRefDifferentName(nochange));
            nochange = @"Foo<ref group=A name=Jones /> and bar<ref name =Jones>Text4</ref> and <ref name =JonesY>Text4</ref>";
            Assert.AreEqual(nochange, Parsers.SameRefDifferentName(nochange));
        }

        [Test]
        public void SameRefDifferentNameNameDerivation()
        {
            string correct = @"Foo<ref name=Jones>Jones 2005</ref> and bar<ref name=""Jones"">Jones 2005</ref>";
            Assert.AreEqual(correct, Parsers.SameRefDifferentName(@"Foo<ref name=Jones>Jones 2005</ref> and bar<ref name=J5>Jones 2005</ref>"));
            Assert.AreEqual(correct, Parsers.SameRefDifferentName(@"Foo<ref name=Jones>Jones 2005</ref> and bar<ref name=""Jo"">Jones 2005</ref>"));
            Assert.AreEqual(correct, Parsers.SameRefDifferentName(@"Foo<ref name=Jones>Jones 2005</ref> and bar<ref name=autogenerated1>Jones 2005</ref>"));
            Assert.AreEqual(correct, Parsers.SameRefDifferentName(@"Foo<ref name=Jones>Jones 2005</ref> and bar<ref name= ReferenceA>Jones 2005</ref>"));
        }

        [Test]
        public void SameNamedRefShortText()
        {

            Assert.AreEqual(@"Foo<ref name=Jones>Jones 2005 extra words of interest</ref> and bar<ref name=""Jones""/>",
                            Parsers.SameRefDifferentName(@"Foo<ref name=Jones>Jones 2005 extra words of interest</ref> and bar<ref name=Jones>a</ref>"));

            Assert.AreEqual(@"Foo<ref name=""Jones""/> and bar<ref name=Jones>Jones 2005 extra words of interest</ref>",
                            Parsers.SameRefDifferentName(@"Foo<ref name=Jones>x</ref> and bar<ref name=Jones>Jones 2005 extra words of interest</ref>"));

            Assert.AreEqual(@"Foo<ref name=Jones>Jones 2005 extra words of interest</ref> and bar<ref name=""Jones""/> and bar2<ref name=""Jones""/>",
                            Parsers.SameRefDifferentName(@"Foo<ref name=Jones>Jones 2005 extra words of interest</ref> and bar<ref name=Jones>a</ref> and bar2<ref name=Jones>x</ref>"));

            Assert.AreEqual(@"Foo<ref name=Jones>Jones 2005 extra words of interest</ref> and bar2<ref name=""Jones""/>",
                            Parsers.SameRefDifferentName(@"Foo<ref name=Jones>Jones 2005 extra words of interest</ref> and bar2<ref name=Jones>[see above]</ref>"));
            Assert.AreEqual(@"Foo<ref name=Jones>Jones 2005 extra words of interest</ref> and bar2<ref name=""Jones""/>",
                            Parsers.SameRefDifferentName(@"Foo<ref name=Jones>Jones 2005 extra words of interest</ref> and bar2<ref name=Jones>{{cite web}}</ref>"));

            string pageref1 = @"Foo<ref name=Jones>Jones 2005 extra words of interest</ref> and bar<ref name=Jones>2</ref>";
            string pageref2 = @"Foo<ref name=Jones>Jones 2005 extra words of interest</ref> and bar<ref name=Jones> page 2</ref>";
            string pageref3 = @"Foo<ref name=Jones>Jones 2005 extra words of interest</ref> and bar<ref name=Jones>pp. 2</ref>";
            string pageref4 = @"Foo<ref name=Jones>Jones 2005 extra words of interest</ref> and bar<ref name=Jones>P 2</ref>";
            string pageref5 = @"Foo<ref name=Jones>Jones 2005 extra words of interest</ref> and bar<ref name=Jones>Jones P 2</ref>";

            Assert.AreEqual(pageref1, Parsers.SameRefDifferentName(pageref1));
            Assert.AreEqual(pageref2, Parsers.SameRefDifferentName(pageref2));
            Assert.AreEqual(pageref3, Parsers.SameRefDifferentName(pageref3));
            Assert.AreEqual(pageref4, Parsers.SameRefDifferentName(pageref4));
            Assert.AreEqual(pageref5, Parsers.SameRefDifferentName(pageref5));
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
            Found.Add(15, 6);
            Assert.AreEqual(Found, Parsers.BadCiteParameters(@"now {{cite web|fooéoo=bar|date=2009}} was"));

            Found.Remove(15);
            Found.Add(15, 3);
            Assert.AreEqual(Found, Parsers.BadCiteParameters(@"now {{cite web|day=11|date=2009}} was"));

            Found.Remove(15);
            Found.Add(15, 3);
            Assert.AreEqual(Found, Parsers.BadCiteParameters(@"now {{cite web|day=1|date=2009}} was"));

            Found.Remove(15);
            Found.Add(15, 5);
            Assert.AreEqual(Found, Parsers.BadCiteParameters(@"now {{cite web|da te=1|date=2009}} was"));

            Found.Remove(15);
            Found.Add(15, 9);
            Assert.AreEqual(Found, Parsers.BadCiteParameters(@"now {{cite web|'language=Thai|date=2009}} was"));

            Found.Remove(15);

            // no errors here
            Assert.AreEqual(Found, Parsers.BadCiteParameters(@"now {{cite web|url=bar|date=2009}} was"));
            Assert.AreEqual(Found, Parsers.BadCiteParameters(@"now {{cite web|url=bar|date=2009|title=A|trans-title=B}} was"));
            Assert.AreEqual(Found, Parsers.BadCiteParameters(@"now {{cite web|url=bar|date=2009|editor=a}} was"));
            Assert.AreEqual(Found, Parsers.BadCiteParameters(@"now {{cite web|url=bar|date=2009|id=3838}} was"));
            Assert.AreEqual(Found, Parsers.BadCiteParameters(@"now {{cite web|url=bar|date=2009|issn=3838-1542}} was"));
            Assert.AreEqual(Found, Parsers.BadCiteParameters(@"now {{cite web|url=bar|date=2009|isbn=1234567890}} was"));
            Assert.AreEqual(Found, Parsers.BadCiteParameters(@"now {{cite web|url=bar|date=2009|author1=Y|author2=X}} was"));
            Assert.AreEqual(Found, Parsers.BadCiteParameters(@"now {{cite web|url=bar|date=2009|deadurl=no}} was"));
            Assert.AreEqual(Found, Parsers.BadCiteParameters(@"now {{cite web|url={{Allmusic|class = foo}}|date=2009}} was"));
            Assert.AreEqual(Found, Parsers.BadCiteParameters(@"now {{cite web|url=bar|date=2009|at=here}} was"));
            Assert.AreEqual(Found, Parsers.BadCiteParameters(@"now {{cite web|url=bar|date=2009|first1=Foo|last1=Great |first2=Bar|title=here}} was"));
            Assert.AreEqual(Found, Parsers.BadCiteParameters(@"now {{cite web|url=bar|date=2009|authorlink1=Smith}} was"));
            Assert.AreEqual(Found, Parsers.BadCiteParameters(@"now {{cite web|url=bar|date=2009|authorlink20=Bill}} was"));
            Assert.AreEqual(Found, Parsers.BadCiteParameters(@"now {{cite web|url=bar|date=2009|author=Smith}} was"));
            Assert.AreEqual(Found, Parsers.BadCiteParameters(@"{{cite web | periodical=, |journal=, |newspaper=, |magazine= |work= |website= |encyclopedia= |encyclopaedia= |dictionary=
|arxiv=, |ARXIV=, |ASIN=, |ASIN-TLD=
|publicationplace=, |publication-place=
|date=, |year=, |publicationdate=, |publication-date=
|series= |volume= |issue=, |number= |page=, |pages=, |at=
|edition= |publisher=, |institution=
|journal=, |jstor=, |agency=, |archive-date=, |others= }}"));

            // multiple errors
            Found.Add(15, 6);
            Found.Add(36, 4);
            Assert.AreEqual(Found, Parsers.BadCiteParameters(@"now {{cite web|foodoo=bar|date=2009|bert= false}} was"));

            // no = between two bars
            Found.Clear();
            Found.Add(14, 11);
            Assert.AreEqual(Found, Parsers.BadCiteParameters(@"now {{cite web|title-bar|url=foo}}"));

            Found.Clear();
            Found.Add(24, 6);
            Assert.AreEqual(Found, Parsers.BadCiteParameters(@"now {{cite web|title=bar|bee |url=foo}}"));

            Found.Clear();
            Found.Add(29, 6);
            Assert.AreEqual(Found, Parsers.BadCiteParameters(@"now {{cite web|title=bar{{a}}|bee |url=foo}}"));

            Found.Clear();
            Found.Add(30, 28);
            Assert.AreEqual(Found, Parsers.BadCiteParameters(@"now {{cite web|title=bar |url=http://www.foo.com/bar text/}}"), "Reports URLs with spaces");

            Found.Clear();
            Assert.AreEqual(Found, Parsers.BadCiteParameters(@"now {{cite web|title=bar |url=http://www.foo.com/bar <!--text comm-->}}"), "Ignores comments with spaces in URLs");

            // URL with space
            Found.Clear();
            Found.Add(34, 3);
            Assert.AreEqual(Found, Parsers.BadCiteParameters(@"now {{cite web|title=bar bee |url=f a}}"));
            Found.Clear();
            Found.Add(30, 3);
            Assert.AreEqual(Found, Parsers.BadCiteParameters(@"now {{cite web|title=f a |url=f a}}"), "reports position of url not title if have same value");
        }

        [Test]
        public void AddMissingReflist()
        {
            // above cats
            const string SingleRef = @"now <ref>foo</ref>", Cat = @"
[[Category:Here]]", ExtLinks = @"==External links==
*[http://www.site.com hello]";
            Assert.AreEqual(SingleRef + "\r\n\r\n" + @"==References==
{{Reflist}}" + "\r\n" + Cat, Parsers.AddMissingReflist(SingleRef + Cat));

            // article has category out of place
            Assert.AreEqual(SingleRef + "\r\n" + @"==References==
{{Reflist}}" + "\r\n" + Cat, Parsers.AddMissingReflist(Cat + SingleRef));

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
            Assert.AreEqual(SingleRef + "\r\n" + @"==References==
{{Reflist}}" + Cat, Parsers.AddMissingReflist(SingleRef + "\r\n" + @"==References==
{{Reflist}}" + Cat));

            // after tracklist, before cats
            const string Tracklist = @"
{{tracklist
| foo = bar
| foo2
}}";
            Assert.AreEqual(SingleRef + Tracklist + "\r\n\r\n" + @"==References==
{{Reflist}}" + "\r\n" + Cat, Parsers.AddMissingReflist(SingleRef + Tracklist + Cat));

            // after s-end, before cats
            const string SEnd = @"
{{s-end}}";
            Assert.AreEqual(SingleRef + SEnd + "\r\n\r\n" + @"==References==
{{Reflist}}" + "\r\n" + Cat, Parsers.AddMissingReflist(SingleRef + SEnd + Cat));

            // missing slash in <references>
            const string missingSlash = @"Foo <ref>a</ref>
==References==
<references>";
            Assert.AreEqual(missingSlash.Replace(@"s>", @"s/>"), Parsers.AddMissingReflist(missingSlash));
            Assert.AreEqual(missingSlash.Replace(@"s>", @"s/>"), Parsers.AddMissingReflist(missingSlash.Replace("s>", "s  >")), "allows whitespace around <references>");

            // missing slash in <References>
            const string missingSlash2 = @"Foo <ref>a</ref>
==References==
<References>";
            Assert.AreEqual(missingSlash.Replace(@"s>", @"s/>"), Parsers.AddMissingReflist(missingSlash2));

            // list of references already present
            const string LDR = @"Foo <ref name='ab'/>
==References==
<references>
<ref name='ab'>abc</ref>
</references>";

            Assert.AreEqual(LDR, Parsers.AddMissingReflist(LDR));

            // Nothing to indicate end of article: goes at end
            Assert.AreEqual(SingleRef + "\r\n\r\n" + @"==References==
{{Reflist}}", Parsers.AddMissingReflist(SingleRef));
        }

        [Test]
        public void AddMissingReflistEnOnly()
        {
#if DEBUG
            const string SingleRef = @"now <ref>foo</ref>", Cat = @"
[[Category:Here]]";
            Assert.AreEqual(SingleRef + "\r\n\r\n" + @"==References==
{{Reflist}}" + "\r\n" + Cat, Parsers.AddMissingReflist(SingleRef + Cat),"add references sections above categories");

            Variables.SetProjectLangCode("fr");
            Assert.AreEqual(SingleRef + Cat, Parsers.AddMissingReflist(SingleRef + Cat),"do nothing in non-en sites");

            Variables.SetProjectLangCode("en");
            Assert.AreEqual(SingleRef + "\r\n\r\n" + @"==References==
{{Reflist}}" + "\r\n" + Cat, Parsers.AddMissingReflist(SingleRef + Cat));
#endif
        }

        [Test]
        public void RefsAfterPunctuation()
        {
            string AllAfter = @"Foo.<ref>bar</ref> The next Foo.<ref>bar</ref> The next Foo.<ref>bar</ref> The next Foo.<ref>bar</ref> The next";
            string R1 = @"Foo<ref>bar</ref>. The next";
            string ellipsis = @"Foo now<ref>abc</ref> ... was this.";

            Assert.AreEqual(AllAfter, Parsers.RefsAfterPunctuation(AllAfter.Replace(".<", "..<")), "duplicate punctuation removed");

            Assert.AreEqual(AllAfter.Replace(".<", "...<"), Parsers.RefsAfterPunctuation(AllAfter.Replace(".<", "...<")), "ellipsis punctuation NOT changed");
            Assert.AreEqual(ellipsis + AllAfter, Parsers.RefsAfterPunctuation(ellipsis + AllAfter));

            Assert.AreEqual(AllAfter + @"Foo.<ref>bar</ref> The next", Parsers.RefsAfterPunctuation(AllAfter + R1), "ref moved after punctuation when majority are after");

            Assert.AreEqual(AllAfter + @"Foo.<ref>ba
r</ref> The next", Parsers.RefsAfterPunctuation(AllAfter + @"Foo<ref>ba
r</ref>. The next"), "ref moved after punctuation when majority are after");

            R1 = R1.Replace("Foo", "Foo ");
            Assert.AreEqual(AllAfter + @"Foo.<ref>bar</ref> The next", Parsers.RefsAfterPunctuation(AllAfter + R1), "Whitespace before ref cleaned when punctuation moved");

            R1 = R1.Replace(".", ",");
            Assert.AreEqual(AllAfter + @"Foo,<ref>bar</ref> The next", Parsers.RefsAfterPunctuation(AllAfter + R1), "handles commas too");
            Assert.AreEqual(AllAfter + AllAfter + @"Foo,<ref>bar</ref> The next" + @"Foo,<ref>bar</ref> The next", Parsers.RefsAfterPunctuation(AllAfter + AllAfter + R1 + R1), "multiple conversions");

            R1 = R1.Replace(",", ":");
            Assert.AreEqual(AllAfter + @"Foo:<ref>bar</ref> The next", Parsers.RefsAfterPunctuation(AllAfter + R1), "handles colons too");

            R1 = R1.Replace(":", "?");
            Assert.AreEqual(AllAfter + @"Foo?<ref>bar</ref> The next", Parsers.RefsAfterPunctuation(AllAfter + R1), "handles question mark");

            R1 = R1.Replace("?", "!");
            Assert.AreEqual(AllAfter + @"Foo!<ref>bar</ref> The next", Parsers.RefsAfterPunctuation(AllAfter + R1), "handles exclamation mark");

            R1 = R1.Replace("!", "–");
            Assert.AreEqual(AllAfter + R1, Parsers.RefsAfterPunctuation(AllAfter + R1), "ref not moved when before dash");

            string TwoRefs = @"Now<ref name=a>bar</ref><ref name=b>bar</ref>. Then";

            Assert.AreEqual(AllAfter + @"Now.<ref name=a>bar</ref><ref name=b>bar</ref> Then", Parsers.RefsAfterPunctuation(AllAfter + TwoRefs), "punctuation moved through multiple refs");

            R1 = @"Foo<ref>bar</ref>.
The next";
            Assert.AreEqual(AllAfter + @"Foo.<ref>bar</ref>
The next", Parsers.RefsAfterPunctuation(AllAfter + R1), "doesn't eat newlines after ref punctuation");
            

			string RandomTable =@"{|
!title
!<ref>bar</ref>
|}";
			Assert.AreEqual(RandomTable, Parsers.RefsAfterPunctuation(RandomTable),"does not affect wikitables");

			string Multiple = @"works<ref>{{cite book |last=McDonald }}</ref>,<ref>{{cite book |last=Gingrich }}</ref>,<ref>{{cite book |location=Norwalk, CT }}</ref>,<ref name=HerdFly/>";
            
            Assert.AreEqual(@"works,<ref>{{cite book |last=McDonald }}</ref><ref>{{cite book |last=Gingrich }}</ref><ref>{{cite book |location=Norwalk, CT }}</ref><ref name=HerdFly/>", Parsers.RefsAfterPunctuation(Multiple));

            Assert.AreEqual(@"thing.{{sfn|Jones|2000}}<ref>foo</ref>", Parsers.RefsAfterPunctuation(@"thing.{{sfn|Jones|2000}}.<ref>foo</ref>"), "Handles {{sfn}}");
            Assert.AreEqual(@"thing.{{sfn|Jones|2000}}<ref>foo</ref>", Parsers.RefsAfterPunctuation(@"thing.{{sfn|Jones|2000}}<ref>foo</ref>."), "Handles {{sfn}}");
            Assert.AreEqual(@"thing.{{sfn|Jones|2000}}<ref>foo</ref>", Parsers.RefsAfterPunctuation(@"thing{{sfn|Jones|2000}}<ref>foo</ref>."), "Handles {{sfn}}");
            Assert.AreEqual(@"thing.<ref>foo</ref>{{sfn|Jones|2000}}", Parsers.RefsAfterPunctuation(@"thing<ref>foo</ref>{{sfn|Jones|2000}}."), "Handles {{sfn}}");

            Assert.AreEqual(@"thing.{{sfnp|Jones|2000}}<ref>foo</ref>", Parsers.RefsAfterPunctuation(@"thing.{{sfnp|Jones|2000}}.<ref>foo</ref>"), "Handles {{sfnp}}");
            Assert.AreEqual(@"thing.{{sfnp|Jones|2000}}<ref>foo</ref>", Parsers.RefsAfterPunctuation(@"thing.{{sfnp|Jones|2000}}<ref>foo</ref>."), "Handles {{sfnp}}");
            Assert.AreEqual(@"thing.{{sfnp|Jones|2000}}<ref>foo</ref>", Parsers.RefsAfterPunctuation(@"thing{{sfnp|Jones|2000}}<ref>foo</ref>."), "Handles {{sfnp}}");
            Assert.AreEqual(@"thing.<ref>foo</ref>{{sfnp|Jones|2000}}", Parsers.RefsAfterPunctuation(@"thing<ref>foo</ref>{{sfnp|Jones|2000}}."), "Handles {{sfnp}}");

            Assert.AreEqual(@"thing.{{efn|Jones|2000}}<ref>foo</ref>", Parsers.RefsAfterPunctuation(@"thing.{{efn|Jones|2000}}.<ref>foo</ref>"), "Handles {{efn}}");
            Assert.AreEqual(@"thing.{{efn|Jones|2000}}<ref>foo</ref>", Parsers.RefsAfterPunctuation(@"thing.{{efn|Jones|2000}}<ref>foo</ref>."), "Handles {{efn}}");
            Assert.AreEqual(@"thing.{{efn|Jones|2000}}<ref>foo</ref>", Parsers.RefsAfterPunctuation(@"thing{{efn|Jones|2000}}<ref>foo</ref>."), "Handles {{efn}}");

            Assert.AreEqual(@"Start,{{sfn|S|1983|p=4}}{{sfn|H|1984|p=5}}{{sfn|M|1984|p=9}} though.", Parsers.RefsAfterPunctuation(@"Start,{{sfn|S|1983|p=4}}{{sfn|H|1984|p=5}}{{sfn|M|1984|p=9}}, though."));

            Assert.AreEqual(@"thing.{{sfn|Jones|2000}}<ref>foo</ref>{{rp|18}}", Parsers.RefsAfterPunctuation(@"thing{{sfn|Jones|2000}}<ref>foo</ref>{{rp|18}}."), "Handles {{sfn}} and {{rp}}");
            Assert.AreEqual(@"thing.<ref>foo</ref>{{rp|18}}{{sfn|Jones|2000}}", Parsers.RefsAfterPunctuation(@"thing<ref>foo</ref>{{rp|18}}{{sfn|Jones|2000}}."), "Handles {{sfn}} and {{rp}}");

            //Assert.AreEqual(@"thing.{{by whom|date=February 2014}}<ref>foo</ref>", Parsers.RefsAfterPunctuation(@"thing.{{by whom|date=February 2014}}.<ref>foo</ref>"), "Handles inline templates");
            //Assert.AreEqual(@"thing.{{by whom|date=February 2014}}<ref>foo</ref>", Parsers.RefsAfterPunctuation(@"thing.{{by whom|date=February 2014}}<ref>foo</ref>."), "Handles inline templates");
            //Assert.AreEqual(@"thing.{{by whom|date=February 2014}}<ref>foo</ref>", Parsers.RefsAfterPunctuation(@"thing{{by whom|date=February 2014}}<ref>foo</ref>."), "Handles inline templates");
            //Assert.AreEqual(@"thing.{{by whom|date=February 2014}}{{whom|date=February 2014}}", Parsers.RefsAfterPunctuation(@"thing{{by whom|date=February 2014}}{{whom|date=February 2014}}."), "Handles inline templates");
            const string Excl = @"Foo!!<ref>bar</ref>";
            Assert.AreEqual(Excl, Parsers.RefsAfterPunctuation(Excl), "No change to !! before ref, can be within table");
        }

        [Test]
        public void RefsAfterPunctuationEnOnly()
        {
#if DEBUG
            string AllAfter = @"Foo.<ref>bar</ref> The next Foo.<ref>bar</ref> The next Foo.<ref>bar</ref> The next Foo.<ref>bar</ref> The next";
            string R1 = @"Foo<ref>bar</ref>. The next";

            Variables.SetProjectLangCode("fr");
            Assert.AreEqual(AllAfter + R1, Parsers.RefsAfterPunctuation(AllAfter + R1), "ref moved after reflist when majority are after");

            Variables.SetProjectLangCode("en");
            Assert.AreEqual(AllAfter + @"Foo.<ref>bar</ref> The next", Parsers.RefsAfterPunctuation(AllAfter + R1), "ref moved after reflist when majority are after");
#endif
        }
    }
}
