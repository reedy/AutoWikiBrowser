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
    /// <summary>
    /// This class must be inhertited by test fixtures that use most parts of WikiFunctions.
    /// It ensures that WikiFunctions is aware that it's being called from unit tests, and
    /// that global data is initialised.
    /// </summary>
    public class RequiresInitialization
    {
        static RequiresInitialization()
        {
            Globals.UnitTestMode = true;
            if (WikiRegexes.Category == null) WikiRegexes.MakeLangSpecificRegexes();
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
            Assert.AreEqual(@"now<ref name=""Fred"">The Honourable Fred Smith, 2002</ref>but later than<ref name=""Fred""/> was", Parsers.DuplicateNamedReferences(@"now<ref name=""Fred"">The Honourable Fred Smith, 2002</ref>but later than<ref name=""Fred""> The Honourable Fred Smith, 2002  </ref> was"));
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
            Assert.AreEqual(@"now<ref name=""Bert"">The Honourable Bert Smith, 2002</ref>but later than<ref name=""Bert""/> was", Parsers.DuplicateNamedReferences(@"now<ref name=""Bert"">The Honourable Bert Smith, 2002</ref>but later than<ref> The Honourable Bert Smith, 2002  </ref> was"));
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
            Assert.AreEqual(@"<ref name=""bookrags.com"">""bookrags.com""</ref> foo <ref name=""bookrags.com""/>" + namedref, Parsers.DuplicateUnnamedReferences(@"<ref>""bookrags.com""</ref> foo <ref> ""bookrags.com"" </ref>" + namedref));
            Assert.AreEqual(@"<ref name=""bookrags.com"">""bookrags.com""</ref> foo bar <ref name=""abcde"">abcde</ref> now <ref name=""abcde""/>now <ref name=""bookrags.com""/>" + namedref,
                            Parsers.DuplicateUnnamedReferences(@"<ref>""bookrags.com""</ref> foo bar <ref>abcde</ref> now <ref>abcde</ref>now <ref>""bookrags.com""</ref>" + namedref));

            Assert.AreEqual(@"<ref name=""ecomodder.com"">http://ecomodder.com/forum/showthread.php/obd-mpguino-gauge-2702.html</ref> foo bar <ref name=""ReferenceA"">http://ecomodder.com/wiki/index.php/MPGuino</ref> now <ref name=""ReferenceA""/>now <ref name=""ecomodder.com""/>" + namedref,
                            Parsers.DuplicateUnnamedReferences(@"<ref>http://ecomodder.com/forum/showthread.php/obd-mpguino-gauge-2702.html</ref> foo bar <ref>http://ecomodder.com/wiki/index.php/MPGuino</ref> now <ref>http://ecomodder.com/wiki/index.php/MPGuino</ref>now <ref>http://ecomodder.com/forum/showthread.php/obd-mpguino-gauge-2702.html</ref>" + namedref));

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

            // after tracklist, before cats
            const string Tracklist = @"
{{tracklist
| foo = bar
| foo2
}}";
            Assert.AreEqual(SingleRef + Tracklist + "\r\n\r\n" + @"==References==
{{Reflist}}" + Cat, Parsers.AddMissingReflist(SingleRef + Tracklist + Cat));

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
{{Reflist}}" + Cat, Parsers.AddMissingReflist(SingleRef + Cat),"add references sections above categories");

            Variables.SetProjectLangCode("fr");
            Assert.AreEqual(SingleRef + Cat, Parsers.AddMissingReflist(SingleRef + Cat),"do nothing in non-en sites");

            Variables.SetProjectLangCode("en");
            Assert.AreEqual(SingleRef + "\r\n\r\n" + @"==References==
{{Reflist}}" + Cat, Parsers.AddMissingReflist(SingleRef + Cat));
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

    [TestFixture]
    public class MOSTests : RequiresParser
    {
        public GenfixesTestsBase genFixes  = new GenfixesTestsBase();
        
        [Test]
        public void TestFixDateMonthOfYear()
        {
            // 'of' between month and year
            Assert.AreEqual(@"Now in July 2007 a new", parser.FixDateOrdinalsAndOf(@"Now in July of 2007 a new", "test"));
            Assert.AreEqual(@"Now ''Plaice'' in July 2007 a new", parser.FixDateOrdinalsAndOf(@"Now ''Plaice'' in July of 2007 a new", "test"));
            Assert.AreEqual(@"Now in January 1907 a new", parser.FixDateOrdinalsAndOf(@"Now in January of 1907 a new", "test"));
            Assert.AreEqual(@"Now in January 1807 a new", parser.FixDateOrdinalsAndOf(@"Now in January of 1807 a new", "test"));
            Assert.AreEqual(@"Now in January 1807 and May 1804 a new", parser.FixDateOrdinalsAndOf(@"Now in January of 1807 and May of 1804 a new", "test"));

            // no Matches
            genFixes.AssertNotChanged(@"Now ""in July of 2007"" a new");
            genFixes.AssertNotChanged(@"Now {{quote|in July of 2007}} a new");
            genFixes.AssertNotChanged(@"Now ""in July of 1707"" a new");
            genFixes.AssertNotChanged(@"Now a march of 2007 resulted");
            genFixes.AssertNotChanged(@"Now the June of 2007 was");
        }

        [Test]
        public void TestFixDateOrdinals()
        {
            // no ordinals on dates
            Assert.AreEqual(@"On 14 March elections were", parser.FixDateOrdinalsAndOf(@"On 14th March elections were", "test"));
            Assert.AreEqual(@"On 14 June elections were", parser.FixDateOrdinalsAndOf(@"On 14th June elections were", "test"));
            Assert.AreEqual(@"14 June elections were", parser.FixDateOrdinalsAndOf(@"14th June elections were", "test"));
            Assert.AreEqual(@"On March 14 elections were", parser.FixDateOrdinalsAndOf(@"On March 14th elections were", "test"));
            Assert.AreEqual(@"On June 21 elections were", parser.FixDateOrdinalsAndOf(@"On June 21st elections were", "test"));
            Assert.AreEqual(@"On 14 March 2008 elections were", parser.FixDateOrdinalsAndOf(@"On 14th March 2008 elections were", "test"));
            Assert.AreEqual(@"On 14 June 2008 elections were", parser.FixDateOrdinalsAndOf(@"On 14th June 2008 elections were", "test"));
            Assert.AreEqual(@"On 14 June 2008 elections were", parser.FixDateOrdinalsAndOf(@"On 14th June, 2008 elections were", "test"));
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
            genFixes.AssertNotChanged(@"On 14th march was");
            genFixes.AssertNotChanged(@"Now the 14th February was");
            genFixes.AssertNotChanged(@"Now the February 14th was");
            genFixes.AssertNotChanged(@"Observance of 5th November Act");
            genFixes.AssertNotChanged(@"'''6th October City''' is", "6th October City");
            genFixes.AssertNotChanged(@"<blockquote>On March 14th, 2008 elections were</blockquote>");
        }

        [Test]
        public void FixDateOrdinalsAndOfEnOnly()
        {
#if DEBUG
            const string c1 = @"On 14th March elections were";
            Variables.SetProjectLangCode("fr");
            Assert.AreEqual(c1, parser.FixDateOrdinalsAndOf(c1, "test"));

            Variables.SetProjectLangCode("en");
            Assert.AreEqual(@"On 14 March elections were", parser.FixDateOrdinalsAndOf(c1, "test"));
#endif
        }

        [Test]
        public void TestFixDateDayOfMonth()
        {
            Assert.AreEqual(@"On 14 February was", parser.FixDateOrdinalsAndOf(@"On 14th of February was", "test"));
            Assert.AreEqual(@"On 4 February was", parser.FixDateOrdinalsAndOf(@"On 4th of February was", "test"));
            Assert.AreEqual(@"On 14 February was", parser.FixDateOrdinalsAndOf(@"On 14th of  February was", "test"));
            Assert.AreEqual(@"14 February 2009 was", parser.FixDateOrdinalsAndOf(@"14th of February 2009 was", "test"));
            Assert.AreEqual(@"24 June 2009 was", parser.FixDateOrdinalsAndOf(@"24th of June 2009 was", "test"));

            // no change
            genFixes.AssertNotChanged(@"On the 14th of February 2009 was");
            genFixes.AssertNotChanged(@"now foo [[File:9th of June street.jpg]] was");
            genFixes.AssertNotChanged(@"now foo [[File:9th of June street , City.JPG|Caption]] was");
            genFixes.AssertNotChanged(@"now foo [[File:9th of June street , City.JPG|On [[Main Article#Overview|9th of June]] Street]] was here");
            genFixes.AssertNotChanged(@"now foo [[File:9th of June street , Bacău.JPG|[[Main Article#Overview|9th of June]] Street]] was here");
            genFixes.AssertNotChanged(@"now foo <gallery>File:9th of June street.JPG</gallery> was here");
            genFixes.AssertNotChanged(@"now foo <gallery>File:9th of June street , B.JPG</gallery> was here");
            genFixes.AssertNotChanged(@"n<gallery>
File:9th of May_street, Bacău.jpg| Street
</gallery>");
        }

        [Test]
        public void DatesLeadingZeros()
        {
            // leading zeros
            Assert.AreEqual(@"On 3 June elections were", parser.FixDateOrdinalsAndOf(@"On 03 June elections were", "test"));
            Assert.AreEqual(@"On 7 June elections were", parser.FixDateOrdinalsAndOf(@"On 07 June elections were", "test"));
            Assert.AreEqual(@"On June 7, elections were", parser.FixDateOrdinalsAndOf(@"On June 07, elections were", "test"));
            Assert.AreEqual(@"On June 7, 2008, elections were", parser.FixDateOrdinalsAndOf(@"On June 07 2008, elections were", "test"));
            Assert.AreEqual(@"On 3 June elections were", parser.FixDateOrdinalsAndOf(@"On 03rd June elections were", "test"));

            // no Matches
            Assert.AreEqual(@"On 2 June 07, elections were", parser.FixDateOrdinalsAndOf(@"On 2 June 07, elections were", "test"));
            Assert.AreEqual(@"The 007 march was", parser.FixDateOrdinalsAndOf(@"The 007 march was", "test"));
            Assert.AreEqual(@"In June '08 there was", parser.FixDateOrdinalsAndOf(@"In June '08 there was", "test"));
            Assert.AreEqual(@"In June 2008 there was", parser.FixDateOrdinalsAndOf(@"In June 2008 there was", "test"));
            Assert.AreEqual(@"On 00 June elections were", parser.FixDateOrdinalsAndOf(@"On 00 June elections were", "test"));
            Assert.AreEqual(@"The 007 May was", parser.FixDateOrdinalsAndOf(@"The 007 May was", "test"));
            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#Overzealous_de-ordinaling
            Assert.AreEqual(@"On 27 June 2nd and 3rd Panzer Groups", parser.FixDateOrdinalsAndOf(@"On 27 June 2nd and 3rd Panzer Groups", "test"));
        }

        [Test]
        public void DeterminePredominantDateLocale()
        {
            string none = @"hello", American1 = @"On July 17, 2009 a", International1 = @"on 17 July 2009 a";
            string ISO = @"now {{use ymd dates}} here", American2 = @"now {{use mdy dates}} here", International2 = @"{{use dmy dates}}"
                , ISOmajority = @"on July 11, 2004 and 2009-11-12 and 2009-11-12 a";

            Assert.AreEqual(Parsers.DateLocale.Undetermined, Parsers.DeterminePredominantDateLocale(none));
            Assert.AreEqual(Parsers.DateLocale.Undetermined, Parsers.DeterminePredominantDateLocale(none, true));
            Assert.AreEqual(Parsers.DateLocale.American, Parsers.DeterminePredominantDateLocale(American1));
            Assert.AreEqual(Parsers.DateLocale.International, Parsers.DeterminePredominantDateLocale(International1));
            Assert.AreEqual(Parsers.DateLocale.American, Parsers.DeterminePredominantDateLocale(American2));
            Assert.AreEqual(Parsers.DateLocale.International, Parsers.DeterminePredominantDateLocale(International2));
            Assert.AreEqual(Parsers.DateLocale.ISO, Parsers.DeterminePredominantDateLocale(ISO));

            Assert.AreEqual(Parsers.DateLocale.ISO, Parsers.DeterminePredominantDateLocale(ISOmajority, true));
            Assert.AreEqual(Parsers.DateLocale.American, Parsers.DeterminePredominantDateLocale(ISOmajority, false));

            Assert.AreEqual(Parsers.DateLocale.Undetermined, Parsers.DeterminePredominantDateLocale(American1 + International1 + @"and 2009-01-04 the", true), "undetermined if count of all dates equal");
            Assert.AreEqual(Parsers.DateLocale.Undetermined, Parsers.DeterminePredominantDateLocale(American1 + International1 + International1 + @"and 2009-01-04 the", true), "undetermined if count of all dates too similar");

            const string bddf = @"{{birth date|df=y|1950|4|7}}", bdnone = @"{{birth date|1950|4|7}}", bdmf = @"{{birth date|mf=y|1950|4|7}}";
            Assert.AreEqual(Parsers.DateLocale.International, Parsers.DeterminePredominantDateLocale(bddf), "uses df=y as fallback");
            Assert.AreEqual(Parsers.DateLocale.Undetermined, Parsers.DeterminePredominantDateLocale(bdnone));
            Assert.AreEqual(Parsers.DateLocale.American, Parsers.DeterminePredominantDateLocale(bdmf), "uses mf=y as fallback");
        }

        [Test]
        public void PredominantDates()
        {
            const string c1 = @"{{cite web|url=http://www.foo.com/bar | title=text | date=2010-04-11 }}", useDMY = @"{{use dmy dates}}",
            c2 = @"{{cite web|url=http://www.foo.com/bar | title=text | date=2010-04-11 | accessdate= 2010-04-11 }}";

            Assert.AreEqual(c1, Parsers.PredominantDates(c1), "no change when no use xxx template");
            Assert.AreEqual(c1.Replace("2010-04-11", "11 April 2010") + useDMY, Parsers.PredominantDates(c1 + useDMY), "converts date to predominant format");
            Assert.AreEqual(c2.Replace("2010-04-11", "11 April 2010") + useDMY, Parsers.PredominantDates(c2 + useDMY), "converts multiple dates in same cite to predominant format");

            Assert.AreEqual(c1.Replace("2010-04-11", "11 April 2010") + useDMY, Parsers.PredominantDates(c1.Replace("2010-04-11", "11 April 2010") + useDMY), "no change when already correct");
        }

        [Test]
        public void PredominantDatesEnOnly()
        {
#if DEBUG
            const string c1 = @"{{cite web|url=http://www.foo.com/bar | title=text | date=2010-04-11 }}", useDMY = @"{{use dmy dates}}";
            Variables.SetProjectLangCode("fr");
            Assert.AreEqual(c1 + useDMY, Parsers.PredominantDates(c1 + useDMY), "no change on non-en wiki");

            Variables.SetProjectLangCode("en");
            Assert.AreEqual(c1.Replace("2010-04-11", "11 April 2010") + useDMY, Parsers.PredominantDates(c1 + useDMY), "converts date to predominant format");
#endif
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

            // apostrophe handling
            Assert.AreEqual(@"[[Image:foo's.jpg|thumb|200px|Bar]]", Parsers.FixImages(@"[[Image:foo%27s.jpg|thumb|200px|Bar]]"));

            const string doubleApos = @"[[Image:foo%27%27s.jpg|thumb|200px|Bar]]";
            Assert.AreEqual(doubleApos, Parsers.FixImages(doubleApos));

            //TODO: decide if such improvements really belong here
            //Assert.AreEqual("[[Media:foo]]",
            //    Parsers.FixImages("[[ media : foo]]"));

            //Assert.AreEqual("[[:Media:foo]]",
            //    Parsers.FixImages("[[ : media : foo]]"));

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_6#URL_underscore_regression
            Assert.AreEqual("[[File:foo|thumb]] # [http://a_b c] [[link]]",
                            Parsers.FixImages("[[File:foo|thumb]] # [http://a_b c] [[link]]"));

            Assert.AreEqual("[[Image:foo|thumb]] # [http://a_b c] [[link]]",
                            Parsers.FixImages("[[Image:foo|thumb]] # [http://a_b c] [[link]]"));

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

            Assert.AreEqual("{{infobox|image=|other=bar}}",
                            Parsers.RemoveImage("foo", "{{infobox|image=foo|other=bar}}", false, "", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("", Parsers.RemoveImage("Foo, bar", "[[File:foo%2C_bar|quux [[here]] there]]", false, "", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual(@"<gallery>
Image:foo.jpg
</gallery>", Parsers.RemoveImage(@"Image:Bar.jpg", @"<gallery>
Image:foo.jpg
Image:Bar.jpg</gallery>", false, "", out noChange));

            Assert.AreEqual(@"<gallery>
Image:foo.jpg

</gallery>", Parsers.RemoveImage(@"Image:Bar.jpg", @"<gallery>
Image:foo.jpg
Image:Bar.jpg|Some text description
</gallery>", false, "", out noChange));

            Assert.AreEqual(@"<gallery>
Image:foo.jpg
<!-- x Image:Bar.jpg|Some text description -->
</gallery>", Parsers.RemoveImage(@"Image:Bar.jpg", @"<gallery>
Image:foo.jpg
Image:Bar.jpg|Some text description
</gallery>", true, "x", out noChange));

            Assert.AreEqual("[[Media:foo.jpg]]", Parsers.RemoveImage("FOO.jpg", "[[Media:foo.jpg]]", false, "", out noChange), "image name is case sensitive");
            Assert.IsTrue(noChange);

            Assert.AreEqual("[[Media:foo.jpg]]", Parsers.RemoveImage("", "[[Media:foo.jpg]]", false, "", out noChange), "no change when blank image name input");
            Assert.IsTrue(noChange);
        }

        [Test]
        public void RemovalMultipleLines()
        {
            bool noChange;

            Assert.AreEqual("", Parsers.RemoveImage("Foo, bar", @"[[File:foo%2C_bar|quux [[here]]
there]]", false, "", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual(" [[now]]", Parsers.RemoveImage("Foo, bar", @"[[File:foo%2C_bar|quux [[here]] there]] [[now]]", false, "", out noChange));
            Assert.IsFalse(noChange);
            Assert.AreEqual(" [[now]]", Parsers.RemoveImage("Foo, bar", @"[[File:foo%2C_bar|quux there]] [[now]]", false, "", out noChange));
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
        //https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_1#Title_bolding
        public void DontEmboldenImagesAndTemplates()
        {
            Assert.IsFalse(parser.BoldTitle("[[Image:Foo.jpg]]", "Foo", out noChangeBack).Contains("'''Foo'''"));
            Assert.IsFalse(parser.BoldTitle("{{Foo}}", "Foo", out noChangeBack).Contains("'''Foo'''"));
            Assert.IsFalse(parser.BoldTitle("{{template| Foo is a bar}}", "Foo", out noChangeBack).Contains("'''Foo'''"));
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

            Assert.AreEqual(@"{{Infobox martial art| website      = }}
{{Nihongo|'''Aikido'''|???|aikido}} is a. Aikido was", parser.BoldTitle(@"{{Infobox martial art| website      = }}
{{Nihongo|'''Aikido'''|???|aikido}} is a. Aikido was", "Aikido", out noChangeBack));
            Assert.IsTrue(noChangeBack);
            
            Assert.AreEqual(@"{{Infobox martial art| name='''Aikido'''| website      = }}
'''Aikido''' was", parser.BoldTitle(@"{{Infobox martial art| name='''Aikido'''| website      = }}
Aikido was", "Aikido", out noChangeBack), "Bold text in infobox ignored");
            Assert.IsFalse(noChangeBack);

            // images then bold
            const string NoChangeImages = @"[[Image:098098889899899889089980890890.png|2390823424890243 23980324 234098234980]] [[Image:098098889899899889089980890890.png|2390823424890243 23980324 234098234980]]
            [[Image:098098889899899889089980890890.png|2390823424890243 23980324 234098234980]] [[Image:098098889899899889089980890890.png|2390823424890243 23980324 234098234980]]
'''A''' was. Foo One here";
            Assert.AreEqual(NoChangeImages, parser.BoldTitle(NoChangeImages, "Foo One", out noChangeBack));
            Assert.IsTrue(noChangeBack);

            Assert.AreEqual("{{year article header}} Foo is a bar While remaining upright may be the primary goal of beginning riders",
                            parser.BoldTitle("{{year article header}} Foo is a bar While remaining upright may be the primary goal of beginning riders", "Foo", out noChangeBack));
            Assert.IsTrue(noChangeBack);

            Assert.AreEqual("{{bio}} Foo is a bar While remaining upright may be the primary goal of beginning riders",
                parser.BoldTitle("{{bio}} Foo is a bar While remaining upright may be the primary goal of beginning riders", "Foo", out noChangeBack));
            Assert.IsTrue(noChangeBack);

            Assert.AreEqual("<dfn>Foo</dfn> is this one", parser.BoldTitle("<dfn>Foo</dfn> is this one", "Foo", out noChangeBack));
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
            const string ImageBold = @"[[Image...]]
'''Something''' is a bar";
            Assert.AreEqual(ImageBold, parser.BoldTitle(ImageBold, "Foo", out noChangeBack));
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

            Assert.AreEqual(@"{{unreferenced|date=November 2010}}
'''Vodafone Qatar''' started", parser.BoldTitle(@"{{unreferenced|date=November 2010}}
Vodafone Qatar started", "Vodafone Qatar", out noChangeBack));
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
        // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_9#Bold_letters
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

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_10#Piped_self-link_delinking_bug
            Assert.AreEqual(@"The '''2009 Indian Premier League''' While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders
==sec== 2009<br>", Parsers.FixLinks(parser.BoldTitle(@"The 2009 Indian Premier League While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders
==sec== [[2009 Indian Premier League|2009]]<br>", "2009 Indian Premier League", out noChangeBack), "2009 Indian Premier League", out noChangeBack));
            
            const string OutsideZerothSection = @"Text here.
==Bio==
John Smith was great.";
            
            Assert.AreEqual(OutsideZerothSection, parser.BoldTitle(OutsideZerothSection, "John Smith", out noChangeBack));
        }

        [Test]
        // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#If_a_selflink_is_also_bolded.2C_AWB_should_just_remove_the_selflink
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

            Assert.AreEqual(@"'''foo'''", parser.BoldTitle(@"[[foo]]", @"Foo", out noChangeBack));
            Assert.AreEqual(@"'''Foo'''", parser.BoldTitle(@"'''[[foo (here)|Foo]]'''", @"foo (here)", out noChangeBack));
            string NoChange = @"{{abc|The [[foo]] was}} A.";
            Assert.AreEqual(NoChange, parser.BoldTitle(NoChange, @"Foo", out noChangeBack));

            NoChange = @"<noinclude>'''[[foo (here)|Foo]]'''</noinclude> A.";
            Assert.AreEqual(NoChange, parser.BoldTitle(NoChange, @"Foo", out noChangeBack), "no change when noinclude/includeonly");
        }
    }

    [TestFixture]
    public class FixMainArticleTests : RequiresInitialization
    {
        [Test]
        public void BasicBehaviour()
        {
            Assert.AreEqual(@"A==Sec==
{{Main|Foo}}", Parsers.FixMainArticle(@"A==Sec==
Main article: [[Foo]]"));
            Assert.AreEqual(@"A==Sec==
{{Main|Foo}}", Parsers.FixMainArticle(@"A==Sec==
Main article: [[Foo]]."));
            Assert.AreEqual(@"A==Sec==
Main article:\r\n [[Foo]]", Parsers.FixMainArticle(@"A==Sec==
Main article:\r\n [[Foo]]"));
        }

        [Test]
        // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_3#Fixing_Main_Article_to_.7B.7Bmain.7D.7D
        public void PipedLinks()
        {
            Assert.AreEqual(@"A==Sec==
{{Main|Foo|l1=Bar}}", Parsers.FixMainArticle(@"A==Sec==
Main article: [[Foo|Bar]]"));
        }

        [Test]
        public void SupportIndenting()
        {
            Assert.AreEqual(@"A==Sec==
{{Main|Foo}}", Parsers.FixMainArticle(@"A==Sec==
:Main article: [[Foo]]"));
            Assert.AreEqual(@"A==Sec==
{{Main|Foo}}", Parsers.FixMainArticle(@"A==Sec==
:Main article: [[Foo]]."));
            Assert.AreEqual(@"A==Sec==
{{Main|Foo}}", Parsers.FixMainArticle(@"A==Sec==
:''Main article: [[Foo]]''"));
            Assert.AreEqual(@"A==Sec==
'':Main article: [[Foo]]''", Parsers.FixMainArticle(@"A==Sec==
'':Main article: [[Foo]]''"));
        }

        [Test]
        public void SupportBoldAndItalic()
        {
            Assert.AreEqual(@"A==Sec==
{{Main|Foo}}", Parsers.FixMainArticle(@"A==Sec==
Main article: '[[Foo]]'"));
            Assert.AreEqual(@"A==Sec==
{{Main|Foo}}", Parsers.FixMainArticle(@"A==Sec==
Main article: ''[[Foo]]''"));
            Assert.AreEqual(@"A==Sec==
{{Main|Foo}}", Parsers.FixMainArticle(@"A==Sec==
Main article: '''[[Foo]]'''"));
            Assert.AreEqual(@"A==Sec==
{{Main|Foo}}", Parsers.FixMainArticle(@"A==Sec==
Main article: '''''[[Foo]]'''''"));

            Assert.AreEqual(@"A==Sec==
{{Main|Foo}}", Parsers.FixMainArticle(@"A==Sec==
'Main article: [[Foo]]'"));
            Assert.AreEqual(@"A==Sec==
{{Main|Foo}}", Parsers.FixMainArticle(@"A==Sec==
''Main article: [[Foo]]''"));
            Assert.AreEqual(@"A==Sec==
{{Main|Foo}}", Parsers.FixMainArticle(@"A==Sec==
'''Main article: [[Foo]]'''"));
            Assert.AreEqual(@"A==Sec==
{{Main|Foo}}", Parsers.FixMainArticle(@"A==Sec==
'''''Main article: [[Foo]]'''''"));

            Assert.AreEqual(@"A==Sec==
{{Main|Foo}}", Parsers.FixMainArticle(@"A==Sec==
''Main article: '''[[Foo]]'''''"));
            Assert.AreEqual(@"A==Sec==
{{Main|Foo}}", Parsers.FixMainArticle(@"A==Sec==
'''Main article: ''[[Foo]]'''''"));
        }

        [Test]
        public void CaseInsensitivity()
        {
            Assert.AreEqual(@"A==Sec==
{{Main|foo}}", Parsers.FixMainArticle(@"A==Sec==
main Article: [[foo]]"));
        }

        [Test]
        // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_4#Problem_with_reverse_subst_of_.7B.7Bmain.7D.7D
        public void DontEatTooMuch()
        {
            Assert.AreEqual(@"A==Sec==
Foo is a bar, see main article: [[Foo]]", Parsers.FixMainArticle(@"A==Sec==
Foo is a bar, see main article: [[Foo]]"));
            Assert.AreEqual(@"A==Sec==
Main article: [[Foo]], bar", Parsers.FixMainArticle(@"A==Sec==
Main article: [[Foo]], bar"));
        }

        [Test]
        // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_6#Main_and_See_also_templates
        public void SingleLinkOnly()
        {
            Assert.AreEqual(@"A==Sec==
:Main article: [[Foo]] and [[Bar]]", Parsers.FixMainArticle(@"A==Sec==
:Main article: [[Foo]] and [[Bar]]"));
            Assert.AreEqual(@"A==Sec==
:Main article: [[Foo|f00]] and [[Bar]]", Parsers.FixMainArticle(@"A==Sec==
:Main article: [[Foo|f00]] and [[Bar]]"));
        }

        [Test]
        // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_7#Problem_with_.22Main_article.22_fixup
        public void Newlines()
        {
            Assert.AreEqual("test\r\n{{Main|Foo}}\r\ntest", Parsers.FixMainArticle("test\r\nMain article: [[Foo]]\r\ntest"));
            Assert.AreEqual("test\r\n\r\n{{Main|Foo}}\r\n\r\ntest", Parsers.FixMainArticle("test\r\n\r\nMain article: [[Foo]]\r\n\r\ntest"));
        }

        [Test]
        // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_3#Fixing_Main_Article_to_.7B.7Bmain.7D.7D
        public void SeeAlso()
        {
            Assert.AreEqual(@"A==Sec==
{{See also|Foo|l1=Bar}}", Parsers.FixMainArticle(@"A==Sec==
See also: [[Foo|Bar]]"));
            Assert.AreEqual(@"A==Sec==
{{See also|Foo}}", Parsers.FixMainArticle(@"A==Sec==
See also: [[Foo]]"));
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
            Assert.AreEqual(@"'''Foo bar''' is great. Foo bar is cool", Parsers.FixLinks(@"'''Foo bar''' is great. [[Foo_bar]] is cool", "Foo bar", out noChangeBack));
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

            // don't apply within {{taxobox color}}
            Assert.AreEqual(@"{{taxobox color| [[foo]] }}", Parsers.FixLinks(@"{{taxobox color| [[foo]] }}", "foo", out noChangeBack));
            Assert.IsTrue(noChangeBack);

            // don't apply if has a noinclude etc.
            Assert.AreEqual(@"<noinclude> [[foo]] </noinclude>", Parsers.FixLinks(@"<noinclude> [[foo]] </noinclude>", "foo", out noChangeBack));
            Assert.IsTrue(noChangeBack);

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_13#Incorrect_delinking_of_article_title
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

              Assert.AreEqual(@"{{Infobox Single
| Name = Feels So Right
| Last single = ""[[Old Flame (song)|Old Flame]]""<br />(1981)
| This single = '''''Feels So Right''''' <br />(1981)
| Next single = ""[[Love in the First Degree (Alabama song)|Love in the First Degree]]"" <br />(1981)
}}", Parsers.FixLinks(@"{{Infobox Single
| Name = Feels So Right
| Last single = ""[[Old Flame (song)|Old Flame]]""<br />(1981)
| This single = '''''[[Feels So Right]]''''' <br />(1981)
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

        [Test]
        public void DablinksAboutAbout()
        {
            const string AboutAbout = @"{{About|about a historical district in the foo|the modern district|Nurfoo District, Republic of Bar}}";
            Assert.AreEqual(AboutAbout.Replace("about ", ""), Parsers.Dablinks(AboutAbout), "removes about...about");
            Assert.AreEqual(AboutAbout.Replace("about ", ""), Parsers.Dablinks(AboutAbout.Replace("about ", "  About ")), "removes about...about, allows extra whitespace");

            Assert.AreEqual(AboutAbout.Replace("about ", ""), Parsers.Dablinks(AboutAbout.Replace("about ", " about")), "no change if already correct");
        }

        [Test]
        public void DablinksNoZerothSection()
        {
            const string NoZerothSection1 = @"
===fpfp===
words";
            Assert.AreEqual(NoZerothSection1, Parsers.Dablinks(NoZerothSection1));

            const string NoZerothSection2 = @"===fpfp===
words";
            Assert.AreEqual(NoZerothSection2, Parsers.Dablinks(NoZerothSection2));
        }

        [Test]
        public void DablinksOtheruses4()
        {
            string OU = @"{{Otheruses4|foo|bar}}";

            Assert.AreEqual(@"{{About|foo|bar}}", Parsers.Dablinks(OU));
        }

        [Test]
        public void DablinksMergingFor()
        {
            const string AboutBefore = @"{{about|Foo|a|b}}", forBefore = @"{{for|c|d}}", aboutAfter = @"{{about|Foo|a|b|c|d}}";

            Assert.AreEqual(aboutAfter, Parsers.Dablinks(AboutBefore + forBefore), "merges for into about (3 args)");
            Assert.AreEqual(aboutAfter, Parsers.Dablinks(forBefore + AboutBefore), "merges for into about – for first");
            Assert.AreEqual(forBefore, Parsers.Dablinks(forBefore), "single for not changed");
            Assert.AreEqual(AboutBefore, Parsers.Dablinks(AboutBefore), "single about not changed");

            const string for1 = @"{{for|a|b}}", about1 = @"{{about||a|b|c|d}}";

            Assert.AreEqual(about1, Parsers.Dablinks(for1 + forBefore));


            const string singleAbout = @"{{about|foo}}";

            Assert.AreEqual(singleAbout + forBefore, Parsers.Dablinks(singleAbout + forBefore), "no merge if {{about}} has <2 arguments");

            const string About2Before = @"{{about|foo|a}}";

            Assert.AreEqual(@"{{about|foo|a||c|d}}", Parsers.Dablinks(About2Before + forBefore), "merges for into about (2 args)");

            Assert.AreEqual(@"{{about|foo|a||c|d|e|f}}", Parsers.Dablinks(About2Before + forBefore + @"{{for|e|f}}"), "two for merges");

            Assert.AreEqual(for1 + "\r\n==foo==" + forBefore, Parsers.Dablinks(for1 + "\r\n==foo==" + forBefore), "only merges dablinks in zeroth section");

            const string For3 = @"{{for|c|d|e}}";

            Assert.AreEqual(@"{{about||a|b|c|d|and|e}}", Parsers.Dablinks(for1 + For3),"merge for with 1 and 3 arguments");
            Assert.AreEqual(@"{{about||a|b|c|d|and|e}}", Parsers.Dablinks(For3 + for1),"merge for with 1 and 3 arguments");

            Assert.AreEqual(@"{{about||a|b|c|d}}{{for|e|f|g|h}}", Parsers.Dablinks(@"{{for|a|b}}{{for|c|d}}{{for|e|f|g|h}}"),"do not merge for with 4 arguments");
            Assert.AreEqual(@"{{for|a|b|c|d}}{{for|e|f|g|h}}", Parsers.Dablinks(@"{{for|a|b|c|d}}{{for|e|f|g|h}}"),"do not merge for with 4 arguments"); 
            
            const string ForTwoCats = @"{{for|the city in California|Category:Lancaster, California}}{{for|the city in Pennsylvania|Category:Lancaster, Pennsylvania}}";

            Assert.AreEqual(@"{{about||the city in California|:Category:Lancaster, California|the city in Pennsylvania|:Category:Lancaster, Pennsylvania}}", Parsers.Dablinks(ForTwoCats));
            Assert.AreEqual(@"{{about||a|b|c|d|other uses|e}}", Parsers.Dablinks(@"{{about||a|b|c|d}}{{for||e}}"),"for with first argument empty");
            Assert.AreEqual(@"{{about|foo|a|b|c|d|other uses|e}}", Parsers.Dablinks(@"{{about|foo|a|b|c|d}}{{for||e}}"),"for with first argument empty");

            Assert.AreEqual(@"{{about|1|2|3|4|5|6|7|8|9}}{{for|a|b}}", Parsers.Dablinks(@"{{about|1|2|3|4|5|6|7|8|9}}{{for|a|b}}"),"don't do anything if about has 9 arguments");
        }

        [Test]
        public void DablinksMergingAbout()
        {
            const string AboutAfter = @"{{about||a|b|c|d}}", a1 = @"{{about||a|b}}", a2 = @"{{about||c|d}}";

            Assert.AreEqual(AboutAfter, Parsers.Dablinks(a1 + a2), "merges abouts with same reason: null reason");
            Assert.AreEqual(AboutAfter, Parsers.Dablinks(AboutAfter), "no change if already correct");

            const string AboutAfterFoo = @"{{about|Foo|a|b|c|d}}", a1Foo = @"{{about|Foo|a|b}}", a2Foo = @"{{about|Foo|c|d}}";

            Assert.AreEqual(AboutAfterFoo, Parsers.Dablinks(a1Foo + a2Foo), "merges abouts with same reason: reason given");
            Assert.AreEqual(AboutAfterFoo, Parsers.Dablinks(AboutAfterFoo), "no change if already correct");

            string a2Bar = @"{{about|Bar|c|d}}";
            Assert.AreEqual(a2Bar + a2Foo, Parsers.Dablinks(a2Bar + a2Foo), "not merged when reason different");

            const string m1 = @"{{About||the film adaptation|The League of Extraordinary Gentlemen (film)}}
{{About||a list of the characters and their origins|Characters in The League of Extraordinary Gentlemen}}
{{About||the British comedy|The League of Gentlemen}}", m1a = @"{{About||the film adaptation|The League of Extraordinary Gentlemen (film)|a list of the characters and their origins|Characters in The League of Extraordinary Gentlemen|the British comedy|The League of Gentlemen}}";

            Assert.AreEqual(m1a + "\r\n\r\n", Parsers.Dablinks(m1));

            string zerosec = @"{{about|foo||a}}{{about|foo||b}}";

            Assert.AreEqual(@"{{about|foo||a|and|b}}", Parsers.Dablinks(zerosec));

            Assert.AreEqual(@"{{about|Foo|a|b|c|d}}", Parsers.Dablinks(a1Foo + a2));
        }

        [Test]
        public void DablinksMergingAboutNamespace()
        {
            const string a2Foo = @"{{about|Foo|Category:Bar|d}}";

            Assert.AreEqual(a2Foo.Replace("|C", "|:C"), Parsers.Dablinks(a2Foo), "escapes category in {{about}}");
        }

        [Test]
        public void DablinksEnOnly()
        {
#if DEBUG
            string zerosec = @"{{about|foo||a}}{{about|foo||b}}";

            Variables.SetProjectLangCode("fr");
            Assert.AreEqual(zerosec, Parsers.Dablinks(zerosec));

            Variables.SetProjectLangCode("en");
            Assert.AreEqual(@"{{about|foo||a|and|b}}", Parsers.Dablinks(zerosec));
#endif
        }

        [Test]
        public void DablinksMergingDistinguish()
        {
            const string AB = @"{{Distinguish|a|b}}";
            Assert.AreEqual(AB, Parsers.Dablinks(@"{{Distinguish|a}}{{Distinguish|b}}"), "merges when single argument");
            Assert.AreEqual(AB.Replace("}}", "|c}}"), Parsers.Dablinks(@"{{Distinguish|a|b}}{{Distinguish|c}}"), "merges multiple arguments");
            Assert.AreEqual(AB.Replace("}}", "|c}}"), Parsers.Dablinks(@"{{Distinguish|a}}{{Distinguish|b|c}}"), "merges multiple arguments");
            Assert.AreEqual(AB, Parsers.Dablinks(AB), "no change if already merged");
        }

        [Test]
        public void MergeTemplatesBySection()
        {
            string AB = @"{{See also|a|b}}";
            Assert.AreEqual(AB, Parsers.MergeTemplatesBySection(@"{{See also|a}}{{See also|b}}"), "merges when single argument");
            Assert.AreEqual(AB.Replace("}}", "|c}}"), Parsers.MergeTemplatesBySection(@"{{See also|a|b}}{{See also|c}}"), "merges multiple arguments");
            Assert.AreEqual(AB.Replace("}}", "|c}}"), Parsers.MergeTemplatesBySection(@"{{See also|a}}{{See also|b|c}}"), "merges multiple arguments");
            Assert.AreEqual(AB.Replace("}}", "|c}}"), Parsers.MergeTemplatesBySection(@"{{See also|a}}{{see also|b|c}}"), "different capitalition");
            Assert.AreEqual(AB, Parsers.MergeTemplatesBySection(AB), "no change if already merged");

            AB = @"{{See also2|a|b}}";
            Assert.AreEqual(AB, Parsers.MergeTemplatesBySection(@"{{See also2|a}}{{See also2|b}}"), "merges when single argument");
            Assert.AreEqual(AB.Replace("}}", "|c}}"), Parsers.MergeTemplatesBySection(@"{{See also2|a|b}}{{See also2|c}}"), "merges multiple arguments");
            Assert.AreEqual(AB.Replace("}}", "|c}}"), Parsers.MergeTemplatesBySection(@"{{See also2|a}}{{See also2|b|c}}"), "merges multiple arguments");
            Assert.AreEqual(AB.Replace("}}", "|[[c]]}}"), Parsers.MergeTemplatesBySection(@"{{See also2|a}}{{See also2|b|[[c]]}}"), "merges multiple arguments, one with link");
            Assert.AreEqual(AB.Replace("}}", "|c}}"), Parsers.MergeTemplatesBySection(@"{{See also2|a}}{{see also2|b|c}}"), "different capitalition");
            Assert.AreEqual(AB, Parsers.MergeTemplatesBySection(AB), "no change if already merged");

            AB = @"{{Main|a|b}}";
            Assert.AreEqual(AB, Parsers.MergeTemplatesBySection(@"{{Main|a}}{{Main|b}}"), "merges when single argument");
            Assert.AreEqual(AB.Replace("}}", "|c}}"), Parsers.MergeTemplatesBySection(@"{{Main|a|b}}{{Main|c}}"), "merges multiple arguments");
            Assert.AreEqual(AB.Replace("}}", "|c}}"), Parsers.MergeTemplatesBySection(@"{{Main|a}}{{Main|b|c}}"), "merges multiple arguments");
            Assert.AreEqual(AB.Replace("}}", "|[[c]]}}"), Parsers.MergeTemplatesBySection(@"{{Main|a}}{{Main|b|[[c]]}}"), "merges multiple arguments, one with link");
            Assert.AreEqual(AB.Replace("}}", "|c}}"), Parsers.MergeTemplatesBySection(@"{{Main|a}}{{main|b|c}}"), "different capitalition");
            Assert.AreEqual(AB, Parsers.MergeTemplatesBySection(AB), "no change if already merged");
            
            const string SeparateSections = @"==One==
{{see also|A}}
==Two==
{{see also|B}}";
            Assert.AreEqual(SeparateSections, Parsers.MergeTemplatesBySection(SeparateSections), "does not merge templates in different sections");
            
             const string NotStartOfSection = @"==One==
{{see also|A}}
Text
{{see also|B}}";
            Assert.AreEqual(NotStartOfSection, Parsers.MergeTemplatesBySection(NotStartOfSection), "does not merge templates not at top of section");
            Assert.AreEqual(@"==One==
{{see also|A|B}}", Parsers.MergeTemplatesBySection(@"==One==
{{see also|A}}
{{see also|B}}"), "does not merge templates not at top of section");
        }
        
        [Test]
        public void MergeTemplatesBySectionEnOnly()
        {
            #if DEBUG
            string AB = @"{{See also|a|b}}", ABSeparate = @"{{See also|a}}{{See also|b}}";

            Variables.SetProjectLangCode("fr");
            Assert.AreEqual(ABSeparate, Parsers.MergeTemplatesBySection(ABSeparate), "No merge when not en-wiki");

            Variables.SetProjectLangCode("en");
            Assert.AreEqual(AB, Parsers.MergeTemplatesBySection(ABSeparate), "merges when single argument");
            #endif
        }

        [Test]
        public void MergePortals()
        {
            Assert.AreEqual("", Parsers.MergePortals(""), "no change when no portals");
            Assert.AreEqual("==see also==", Parsers.MergePortals("==see also=="), "no change when no portals");

            const string singlePortal = @"Foo
==See also==
{{Portal|Bar}}";
            Assert.AreEqual(singlePortal, Parsers.MergePortals(singlePortal), "no change when single portal");

            const string PortalBox1 = @"Foo
==See also==
{{Portal|Bar|Foo2}}
";
            Assert.AreEqual(PortalBox1, Parsers.MergePortals(@"Foo
==See also==
{{Portal|Bar}}
{{Portal|Foo2 }}"), "merges multiple portals to single portal");

            const string NoSeeAlso = @"{{Portal|Bar}}
{{Portal|Foo2 }}";

            Assert.AreEqual(NoSeeAlso, Parsers.MergePortals(NoSeeAlso), "no merging if no portal and no see also");

            const string MultipleArguments = @"Foo
==See also==
{{Portal|Bar}}
{{Portal|Foo2|other=here}}";

            Assert.AreEqual(MultipleArguments, Parsers.MergePortals(MultipleArguments), "no merging of portal with multiple arguments");

            // merging in same section
            const string SameSection = @"Foo
==Section==
{{Portal|Bar}}
{{Portal|Foo}}
==Other==";
            Assert.AreEqual(@"Foo
==Section==
{{Portal|Bar|Foo}}
==Other==", Parsers.MergePortals(SameSection), "portals merged to first portal location when all in same section");

            const string differentSection = @"Foo
==Section==
{{Portal|Bar}}
==Other==
{{Portal|Foo}}";
            Assert.AreEqual(differentSection, Parsers.MergePortals(differentSection), "not merged when portals in ddifferent sections");

            const string TwoSeeAlso = @"Foo
==See also==
{{Portal|Bar}}
==See also==
{{Portal|Foo}}";
            Assert.AreEqual(TwoSeeAlso, Parsers.MergePortals(TwoSeeAlso), "not merged when multiple see also sections");
        }

        [Test]
        public void MergePortalsEnOnly()
        {
#if DEBUG
            const string PortalBox1 = @"Foo
==See also==
{{Portal|Bar|Foo2}}
", input = @"Foo
==See also==
{{Portal|Bar}}
{{Portal|Foo2 }}";

            Variables.SetProjectLangCode("fr");
            Assert.AreEqual(input, Parsers.MergePortals(input));

            Variables.SetProjectLangCode("en");
            Assert.AreEqual(PortalBox1, Parsers.MergePortals(input));
#endif
        }
    }

    [TestFixture]
    public class UnicodifyTests : RequiresParser
    {
        [Test]
        public void PreserveTM()
        {
            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_1#AWB_corrupts_the_trademark_.28TM.29_special_character_.
            Assert.AreEqual("test™", parser.Unicodify("test™"));
        }

        [Test]
        public void DontChangeCertainEntities()
        {
            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_3#.26emsp.3B
            Assert.AreEqual("&emsp;&#013;", parser.Unicodify("&emsp;&#013;"));

            Assert.AreEqual("The F&#x2011;22 plane", parser.Unicodify("The F&#x2011;22 plane"));

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_10#SmackBot:_conversion_of_HTML_char-codes_to_raw_Unicode:_issue_.26_consequent_suggestion
            Assert.AreEqual(@"the exclamation mark&#8201;! was", parser.Unicodify(@"the exclamation mark&#8201;! was"));
            Assert.AreEqual(@"the exclamation mark&#8239;! was", parser.Unicodify(@"the exclamation mark&#8239;! was"));

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#zero-width_space
            Assert.AreEqual(@" hello &#8203; bye", parser.Unicodify(@" hello &#8203; bye"));

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Greedy regex for unicode characters
            Assert.AreEqual(@" hello &#x20000; bye", parser.Unicodify(@" hello &#x20000; bye"));
            Assert.AreEqual(@" hello &#x2000f; bye", parser.Unicodify(@" hello &#x2000f; bye"));

            Assert.AreEqual("A &#x2329; B", parser.Unicodify("A &#x2329; B"));
            Assert.AreEqual("A &#x232A; B", parser.Unicodify("A &#x232A; B"));

            // Characters above hex 10000 not changed
            Assert.AreEqual("A &#x10A80; B", parser.Unicodify("A &#x10A80; B"));
            Assert.AreEqual("A &#x20A80; B", parser.Unicodify("A &#x20A80; B"));
        }

        [Test]
        public void IgnoreMath()
        {
            Assert.AreEqual("<math>&laquo;</math>", parser.Unicodify("<math>&laquo;</math>"));
        }
        
        [Test]
        public void Casing()
        {
            Assert.AreEqual("A† B‡", parser.Unicodify("A&dagger; B&Dagger;"), "supports lowercase and first-upper HTML characters");
        }
        
        [Test]
        public void Templates()
        {
            Assert.AreEqual("A† B{{template|†}} C{{template2|one=†}}", parser.Unicodify("A&dagger; B{{template|&dagger;}} C{{template2|one=&dagger;}}"), "can support characters within templates");
        }
    }

    [TestFixture]
    public class UtilityFunctionTests : RequiresParser
    {
        [SetUp]
        public void SetUp()
        {
#if DEBUG
            Variables.SetProjectLangCode("en");
#endif
        }


        [Test]
        public void ChangeToDefaultSort()
        {
            bool noChange;

            // don't change sorting for single categories
            Assert.AreEqual("[[Category:Test1|Foooo]]",
                            Parsers.ChangeToDefaultSort("[[Category:Test1|Foooo]]", "Foo", out noChange),"don't change sorting for single categories");
            Assert.IsTrue(noChange,"don't change sorting for single categories");

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

            // should also fix diacritics in existing defaultsorts and remove leading spaces
            // also support mimicking templates: template to magic word conversion, see [[Category:Pages which use a template in place of a magic word]]
            Assert.AreEqual("{{DEFAULTSORT:Test}}", Parsers.ChangeToDefaultSort("{{defaultsort| Tést}}", "Foo", out noChange));
            Assert.IsFalse(noChange);
            Assert.AreEqual("{{DEFAULTSORT:Test}}", Parsers.ChangeToDefaultSort("{{DEFAULTSORT| Tést}}", "Foo", out noChange));
            Assert.IsFalse(noChange);
            Assert.AreEqual("{{DEFAULTSORT:Test}}", Parsers.ChangeToDefaultSort("{{DEFAULTSORT:|Test}}", "Foo", out noChange));
            Assert.IsFalse(noChange);

            // shouldn't change whitespace-only sortkeys
            Assert.AreEqual("{{DEFAULTSORT: \t}}", Parsers.ChangeToDefaultSort("{{DEFAULTSORT: \t}}", "Foo", out noChange));
            Assert.IsTrue(noChange);

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_6#DEFAULTSORT_with_spaces
            // DEFAULTSORT doesn't treat leading spaces the same way as categories do
            Assert.AreEqual("[[Category:Test1| Foooo]][[Category:Test2| Foooo]]",
                            Parsers.ChangeToDefaultSort("[[Category:Test1| Foooo]][[Category:Test2| Foooo]]", "Bar",
                                                        out noChange));
            Assert.IsTrue(noChange);

            // pages with multiple sort specifiers shouldn't be changed
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
            Assert.AreEqual(@"[[Category:Bronze Wolf awardees|LainI, Juan]]",
                            Parsers.ChangeToDefaultSort(@"[[Category:Bronze Wolf awardees|Lainİ, Juan]]", "Hi", out noChange), "unusual one where lowercase of diacritic is a Latin character");
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
{{DEFAULTSORT:Special foos}}", Parsers.ChangeToDefaultSort(@"foo
[[Category:All foos]]", "Category:Special foŏs", out noChange, false));

            Assert.AreEqual(@"foo
[[Category:All foos]]
[[Category:All foos2]]
{{DEFAULTSORT:Rail in Izmir}}", Parsers.ChangeToDefaultSort(@"foo
[[Category:All foos]]
[[Category:All foos2]]", "Rail in İzmir", out noChange, false));

            // hyphen in title becomes a minus in DEFAULTSORT key
            Assert.AreEqual(@"foo
{{DEFAULTSORT:Women's Circuit (July-September)}}", Parsers.ChangeToDefaultSort(@"foo
{{DEFAULTSORT:Women's Circuit (July–September)}}", "Women's Circuit (July–September)", out noChange, false));

            // skip when nonclude on page
            const string NoInclude = @"[[Category:Test1|Foooo]][[Category:Test2|Foooo]] <noinclude>foo</noinclude>";
            Assert.AreEqual(NoInclude,
                            Parsers.ChangeToDefaultSort(NoInclude, "Bar",
                                                        out noChange));
            Assert.IsTrue(noChange);
        }

        [Test]
        public void MissingDefaultSort()
        {
            Assert.IsFalse(Parsers.MissingDefaultSort(@"A", @"A"));
            Assert.IsFalse(Parsers.MissingDefaultSort(@"A {{DEFAULTSORT:A}}", @"A {{DEFAULTSORT:A}}"));
            Assert.IsFalse(Parsers.MissingDefaultSort(@"A {{DEFAULTSORT:A}} [[category:A]]", @"A"));
            Assert.IsTrue(Parsers.MissingDefaultSort(@"A
[[Category:1910 births]]", @"John Smith"));
        }

        [Test]
        public void ChangeToDefaultsortCaseInsensitive()
        {
            bool noChange;
            const string CInsensitive = @"x [[Category:Foo]]";

            Assert.AreEqual(CInsensitive, Parsers.ChangeToDefaultSort(CInsensitive, "BAR", out noChange), "no change when defaultsort only differs to article title by case");
            Assert.IsTrue(noChange);

            Assert.AreEqual(CInsensitive, Parsers.ChangeToDefaultSort(CInsensitive, "Bar", out noChange), "no change when defaultsort only differs to article title by case");
            Assert.IsTrue(noChange);

            Assert.AreEqual(CInsensitive, Parsers.ChangeToDefaultSort(CInsensitive, "Bar foo", out noChange), "no change when defaultsort only differs to article title by case");
            Assert.IsTrue(noChange);

            Assert.AreEqual(CInsensitive, Parsers.ChangeToDefaultSort(CInsensitive, "Bar (foo)", out noChange), "no change when defaultsort only differs to article title by case");
            Assert.IsTrue(noChange);

            string CInsensitive2 = @"{{DEFAULTSORT:Bar}} [[Category:Foo]]";

            Assert.AreEqual(CInsensitive2, Parsers.ChangeToDefaultSort(CInsensitive2, "BAR", out noChange), "no change when existing defaultsort only differs to article title by case");
            Assert.IsTrue(noChange);

            CInsensitive2 = @"{{DEFAULTSORT:bar}} [[Category:Foo]]";

            Assert.AreEqual(CInsensitive2, Parsers.ChangeToDefaultSort(CInsensitive2, "BAR", out noChange), "no change when existing defaultsort only differs to article title by case");
            Assert.IsTrue(noChange);

            CInsensitive2 = @"{{DEFAULTSORT:BAR}} [[Category:Foo]]";

            Assert.AreEqual(CInsensitive2, Parsers.ChangeToDefaultSort(CInsensitive2, "BAR", out noChange), "no change when existing defaultsort only differs to article title by case");
            Assert.IsTrue(noChange);
        }

        [Test]
        public void ChangeToDefaultSortPAGENAME()
        {
            bool noChange;

            Assert.AreEqual("[[Category:Test1]][[Category:Test2]]",
                            Parsers.ChangeToDefaultSort("[[Category:Test1|{{PAGENAME}}]][[Category:Test2]]", "Foooo", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("[[Category:Test1]][[Category:Test2]]",
                            Parsers.ChangeToDefaultSort("[[Category:Test1|{{subst:PAGENAME}}]][[Category:Test2]]", "Foooo", out noChange));
            Assert.IsFalse(noChange);
        }

        [Test]
        public void ChangeToDefaultSortMultiple()
        {
            bool noChange;
            const string Multi = "[[Category:Test1|Foooo]][[Category:Test2|Foooo]]\r\n{{DEFAULTSORT:Foooo}}\r\n{{DEFAULTSORTKEY:Foo2oo}}";

            Assert.AreEqual(Multi, Parsers.ChangeToDefaultSort(Multi, "Bar", out noChange), "no change when multiple different defaultsorts");
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

            string a2 = @"Fred Smith blah {{imdb name|id=abc}} [[Category:Living people]]";

            Assert.AreEqual(a2 + b, Parsers.ChangeToDefaultSort(a2, "Fred Smith", out noChange));
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


            // category sortkeys are cleaned too
            Assert.AreEqual(@"[[Category:Parishes of the Azores]]
[[Category:São Miguel Island]]
{{DEFAULTSORT:Agua Retorta}}", Parsers.ChangeToDefaultSort(@"[[Category:Parishes of the Azores|Agua Retorta]]
[[Category:São Miguel Island]]", @"Água Retorta", out noChange));
            Assert.IsFalse(noChange);

            // use article name
            Assert.AreEqual(@"[[Category:Parishes of the Azores]]
[[Category:São Miguel Island]]
{{DEFAULTSORT:Agua Retorta}}", Parsers.ChangeToDefaultSort(@"[[Category:Parishes of the Azores]]
[[Category:São Miguel Island]]", @"Água Retorta", out noChange));
            Assert.IsFalse(noChange);
        }

        [Test]
        public void TestIsArticleAboutAPerson()
        {
            Assert.IsTrue(Parsers.IsArticleAboutAPerson(@"Foo {{persondata|name=smith}}", "foo"));
            Assert.IsTrue(Parsers.IsArticleAboutAPerson(@"Foo {{infobox person|name=smith}}", "foo"));
            Assert.IsTrue(Parsers.IsArticleAboutAPerson(@"Foo [[Category:1900 deaths]]", "foo"));
            Assert.IsTrue(Parsers.IsArticleAboutAPerson(@"Foo [[Category:1900 births]]", "foo"));
            Assert.IsTrue(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Living people]]", "foo"));
            Assert.IsTrue(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Year of birth missing (living people)]]", "foo"));
            Assert.IsTrue(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Living people|Smith]]", "foo"));

            Assert.IsTrue(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Living people]] [[Category:Living people]]", "foo"), "duplicate categories removed, so okay");

            Assert.IsTrue(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{England-bio-stub}}", "foo"));
            Assert.IsTrue(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{Switzerland-politician-stub}}", "foo"));
            Assert.IsTrue(Parsers.IsArticleAboutAPerson(@"Some words {{death date and age|1960|01|9}}", "foo"));
            Assert.IsTrue(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{RefimproveBLP}}", "foo"));
            Assert.IsTrue(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "16 and pregnant"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP unsourced section|foo=bar}}", "foo"));

            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo {{persondata|name=smith}}", "Category:foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "List of foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Lists of foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' [[Category:Missing people organizations]]", "Foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Deaths in 2004"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "First Assembly of X"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Pierre Schaeffer bibliography"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Adoption of x"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo (family)"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo (x family)"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo (x team)"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo (publisher)"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo haunting"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo martyrs"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo quartet"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo team"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo twins"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Attack on x"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Suicide of x"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Presidency of x"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Governor of x"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Mayoralty of x"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "First presidency of x"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "2004 something"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "2004–09 something"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo discography"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo children"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo murders"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo, bar and other"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo & other"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo, bar, and Other"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo One and Other People"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo groups"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "The Foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo people"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo campaign"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo rebellion"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo native"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo center"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Second Foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo x families"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Brothers Foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "X from Y"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo brothers"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo Sisters"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "X Service"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo (artists)"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo x families (bar)"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "X in Y"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Foo campaign, 2000"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Atlanta murders of 1979–1981"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Death rates in the 20th century"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Birth rates in the 20th century"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Death rates in the 1st century"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Death rates in the 2nd century"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{BLP sources|foo=bar}}", "Death rates in the 3nd century"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:1900 deaths]] and [[Category:1905 deaths]]", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo {{infobox some organization|foo=bar}} {{persondata|name=smith}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Some noble families]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Noble families]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:X teams and stables]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:German families]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:German x families]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:X diaspora in y]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Baronies x]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Groups x]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:X royal families]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:X nicknames]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:X pageants]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:X groups]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:X magazines]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:X people]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Positions x]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:X troupes]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:X y groups]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:1900 establishments in X]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:X gods]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Companies foo]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Surnames]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:X musical groups]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:X bands]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:X music groups]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:X titles]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Performing groups]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:X ethnic groups]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:X artist groups in]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Musical groups established in 2000]] {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo {{infobox television|bye=a}} {{refimproveBLP}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''Foo''' {{England-bio-stub}} {{sia}}", "Foo"));

            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo {{Infobox settlement}} {{foo-bio-stub}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo {{italic title}} {{foo-bio-stub}}", "foo"));

            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Married couples]] {{persondata|name=smith}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:2002 animal births]] {{persondata|name=smith}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Comedy duos]] {{persondata|name=smith}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Comedy trios]] {{persondata|name=smith}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Foo Comedy duos]] {{persondata|name=smith}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo {{In-universe}} {{persondata|name=smith}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo {{in-universe}} {{persondata|name=smith}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo {{Infobox political party}} {{birth date and age|1974|11|26}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Articles about multiple people]] {{persondata|name=smith}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Fictional blah]] {{persondata|name=smith}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[fictional character]] {{persondata|name=smith}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[fictional character|character]] {{persondata|name=smith}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo {{dab}} {{persondata|name=smith}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Internet memes]] {{bda|1980|11|11}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:Military animals]] {{bda|1980|11|11}}", "foo"));

            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:2002 births]] {{infobox Musical artist|Background=group_or_band}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:2002 births]] {{infobox Musical artist|background=group_or_band}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:2002 births]] {{infobox musical artist|Background=group_or_band}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:2002 births]] {{infobox Musical artist|Background=classical_ensemble}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:2002 births]] {{Infobox Chinese-language singer and actor|currentmembers=A, B}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:2002 births]] {{infobox Band|Background=group_or_band}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo [[Category:2002 births]] {{infobox Musical artist|Background=band}}", "foo"));
            Assert.IsTrue(Parsers.IsArticleAboutAPerson(@"Foo [[Category:2002 births]] {{infobox musical artist|Background=other}}", "foo"));
            Assert.IsTrue(Parsers.IsArticleAboutAPerson(@"Foo [[Category:2002 births]] {{Infobox Chinese-language singer and actor|name=A, B}}", "foo"));

            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo {{infobox person|name=smith}} Foo {{infobox person|name=smith2}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"Foo {{death date|2002}} {{death date|2005}}", "foo"));

            // multiple different birth dates means not about one person
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"{{nat fs player|no=1|pos=GK|name=[[Meg]]|age={{Birth date|1956|01|01}} ({{Age at date|1956|01|01|1995|6|5}})|caps=|club=|clubnat=}}
{{nat fs player|no=2|pos=MF|name=[[Valeria]]|age={{Birth date|1968|09|03}} ({{Age at date|1968|09|03|1995|6|5}})|caps=|club=|clubnat=}}", "foo"));
            Assert.IsTrue(Parsers.IsArticleAboutAPerson(@"{{nat fs player|no=1|pos=GK|name=[[Meg]]|age={{Birth date|1956|01|01}} }} {{Birth date|1956|01|01}} {{Persondata}}", "foo"));

            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"{{see also|Fred}} Fred Smith is great == foo == {{persondata}}", "foo"));
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"{{Main|Fred}} Fred Smith is great == foo == {{persondata}}", "foo"));

            // link in bold in zeroth section to somewhere else is no good
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"'''military career of [[Napoleon Bonaparte]]''' == foo == {{birth date|2008|11|11}}", "foo"));

            // 'characters' category means fictional person
            Assert.IsFalse(Parsers.IsArticleAboutAPerson(@"foo [[Category:227 characters]] {{persondata}}", "foo"));

            Assert.IsTrue(Parsers.IsArticleAboutAPerson(@"'''Margaret Sidney''' was the [[pseudonym]] of American author '''Harriett Mulford Stone''' (June 22, 1844–August 2, 1924).
[[Category:1844 births]]", "foo"));
            
            string AR = @"{{Infobox rugby biography
| birth_name = Opeti Fonua
| birth_date ={{birth date and age|df=yes|1986|05|26}}
| birth_place = [[Tonga]]
| height = 1.97 m (6 ft 5.5 in)
| weight = 140 kg (22 st 2 lb, 310 lb)
| ru_position = Backrow, Secondrow
| ru_amateurclubs =
| ru_clubyears = 2005-2007<br />2007-2008<br />2008-
| ru_proclubs = [[SU Agen Lot-et-Garonne|SU Agen]]<br />[[Football club Auch Gers|FC Auch]]<br />[[Sporting union Agen Lot-et-Garonne|SU Agen]]
| ru_clubcaps = 34<ref name=""itsrugby""/><br /> 12<ref name=""itsrugby"">{{Lien web |url=http://www.itsrugby.fr/joueur-3640.html |titre=Opeti Fonua |site=www.itsrugby.fr |consulté le=30</ref><br />81<ref name=""itsrugby""/>
| ru_clubpoints = 10 (2 tries)<br />0<br />135 (23 tries)
| ru_nationalyears = 2009-
| ru_nationalteam = {{ru|Tonga}}
| ru_nationalcaps = 2
| ru_nationalpoints = (0)
}}

'''Opeti Fonua''', born [[26 May]] [[1986]], is a [[Tongan]] [[rugby union]] player who plays as a no.8 for French club [[SU Agen]]. Fonua is known for his immense size and strength (Fonua stands a shade under 6 feet 6 inches, or 197 cm, and weights in at 140 kg, well over 300 lb) but also for his speed and agility, surprising attributes given his huge physical size. Big tackles and strong carries, replete with hand-offs to opponents, are his trademarks. He is also known for bouncing off and running over opponents, often leaving them on the ground on their backsides as he continues galloping forward after smashing into them.

==References==
{{Reflist}}
";
            Assert.IsTrue(Parsers.IsArticleAboutAPerson(AR, "Opeti Fonua"));
            
        }

        [Test]
        public void ExternalURLToInternalLink()
        {
            Assert.AreEqual("", Parsers.ExternalURLToInternalLink(""));

            Assert.AreEqual("https://secure.wikimedia.org/otrs/index.pl?Action=AgentTicketQueue",
                            Parsers.ExternalURLToInternalLink(
                                "https://secure.wikimedia.org/otrs/index.pl?Action=AgentTicketQueue"));

            Assert.AreEqual("[[w:ru:Foo|Foo]]",
                            Parsers.ExternalURLToInternalLink("[http://ru.wikipedia.org/wiki/Foo Foo]"));

            Assert.AreEqual("[[m:Test|Test]]",
                            Parsers.ExternalURLToInternalLink("[http://meta.wikimedia.org/wiki/Test Test]"));
            Assert.AreEqual("[[commons:Test|Test]]",
                            Parsers.ExternalURLToInternalLink("[http://commons.wikimedia.org/wiki/Test Test]"));
        }

        [Test]
        public void ExternalURLToInternalLinkEn()
        {
            Assert.AreEqual("[[wikt:Test|Test]]",
                            Parsers.ExternalURLToInternalLink("[http://en.wiktionary.org/wiki/Test Test]"));
            Assert.AreEqual("[[wikt:Test|Test]]",
                            Parsers.ExternalURLToInternalLink("[http://en.wiktionary.org/w/Test Test]"));
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
            Assert.AreEqual("[[w:Test|Test]]",
                            Parsers.ExternalURLToInternalLink("[http://en.wikipedia.org/wiki/Test Test]"));
            Assert.AreEqual("[[w:Test|Test]]",
                            Parsers.ExternalURLToInternalLink("[https://en.wikipedia.org/wiki/Test Test]"));

            Assert.AreEqual("[[wikt:fr:Test|Test]]",
                            Parsers.ExternalURLToInternalLink("[http://fr.wiktionary.org/wiki/Test Test]"));
            Assert.AreEqual("[[w:fr:Test|Test]]",
                            Parsers.ExternalURLToInternalLink("[http://fr.wikipedia.org/wiki/Test Test]"));
            Assert.AreEqual("[[w:fr:Test|Test]]",
                            Parsers.ExternalURLToInternalLink("[https://fr.wikipedia.org/wiki/Test Test]"));

#if DEBUG
            Variables.SetProjectLangCode("fr");
            Assert.AreEqual("[[w:en:Test|Test]]",
                            Parsers.ExternalURLToInternalLink("[http://en.wikipedia.org/wiki/Test Test]"));
            Assert.AreEqual("[[w:en:Test|Test]]",
                            Parsers.ExternalURLToInternalLink("[https://en.wikipedia.org/wiki/Test Test]"));
            Variables.SetProjectLangCode("en");
            Assert.AreEqual("[[w:Test|Test]]",
                            Parsers.ExternalURLToInternalLink("[http://en.wikipedia.org/wiki/Test Test]"));
#endif
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
            Assert.IsFalse(Parsers.HasRefAfterReflist(@"blah <ref>a</ref> ==references== {{reflist|refs=<ref>abc</ref>}}"));
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

            #if DEBUG
            Variables.SetProjectLangCode("fr");

            Assert.IsFalse(Parsers.HasRefAfterReflist(@"blah <ref>a</ref>
==references== {{reflist}} <ref>b</ref>"));
            Variables.SetProjectLangCode("en");
            #endif
        }

        [Test]
        public void HasInUseTagTests()
        {
            Assert.IsTrue(Parsers.IsInUse("{{inuse}} Hello world"));
            Assert.IsTrue(Parsers.IsInUse("{{in creation}} Hello world"));
            Assert.IsTrue(Parsers.IsInUse("{{increation}} Hello world"));
            Assert.IsTrue(Parsers.IsInUse("{{ inuse  }} Hello world"));
            Assert.IsTrue(Parsers.IsInUse("{{Inuse}} Hello world"));
            Assert.IsTrue(Parsers.IsInUse("Hello {{inuse}} Hello world"));
            Assert.IsTrue(Parsers.IsInUse("{{inuse|5 minutes}} Hello world"));
            Assert.IsTrue(Parsers.IsInUse("{{In use}} Hello world"));
            Assert.IsTrue(Parsers.IsInUse("{{in use|5 minutes}} Hello world"));


            // ignore commented inuse
            Assert.IsFalse(Parsers.IsInUse("<!--{{inuse}}--> Hello world"));
            Assert.IsFalse(Parsers.IsInUse("<nowiki>{{inuse}}</nowiki> Hello world"));
            Assert.IsFalse(Parsers.IsInUse("<nowiki>{{in use}}</nowiki> Hello world"));
            Assert.IsTrue(Parsers.IsInUse("<!--{{inuse}}--> {{inuse|5 minutes}} Hello world"));
            Assert.IsTrue(Parsers.IsInUse("<!--{{inuse}}--> {{in use|5 minutes}} Hello world"));

            Assert.IsFalse(Parsers.IsInUse("{{INUSE}} Hello world")); // no such template

#if DEBUG
            Variables.SetProjectLangCode("el");
            WikiRegexes.MakeLangSpecificRegexes();
            
            Assert.IsTrue(Parsers.IsInUse("{{Σε χρήση}} Hello world"),"σε χρήση");
            Assert.IsTrue(Parsers.IsInUse("{{inuse}} Hello world"),"inuse");
            Assert.IsFalse(Parsers.IsInUse("{{goceinuse}} Hello world"),"goceinuse is en-only");

#endif
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
            Assert.IsFalse(Parsers.IsMissingReferencesDisplay(@"Hello<ref>Fred</ref> {{Reflist
|refs = 
{{cite news | title = A { hello }}
}}"), "Unbalanced brackets within cite template in reflist does not affect logic");
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

            Assert.IsFalse(Parsers.IsMissingReferencesDisplay(@"Hello<ref group=X>Fred</ref>"));
        }

        [Test]
        public void IsMissingReferencesDisplayTestsEnOnly()
        {
#if DEBUG
            Variables.SetProjectLangCode("fr");

            Assert.IsFalse(Parsers.IsMissingReferencesDisplay(@"Hello<ref>Fred</ref>"));
            Assert.IsFalse(Parsers.IsMissingReferencesDisplay(@"Hello<ref name=""F"">Fred</ref>"));

            Variables.SetProjectLangCode("en");
#endif
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
        public void InfoboxTestsEnOnly()
        {
#if DEBUG
            Variables.SetProjectLangCode("fr");
            Assert.IsFalse(Parsers.HasInfobox(@"{{Infobox fish | name = Bert }} ''Bert'' is a good fish."));
            Assert.IsFalse(Parsers.HasInfobox(@"{{INFOBOX fish | name = Bert }} ''Bert'' is a good fish."));
            Variables.SetProjectLangCode("en");
            Assert.IsTrue(Parsers.HasInfobox(@"{{Infobox fish | name = Bert }} ''Bert'' is a good fish."));
#endif
        }

        [Test]
        public void HasStubTemplate()
        {
            Assert.IsTrue(Parsers.HasStubTemplate(@"foo {{foo stub}}"));
            Assert.IsTrue(Parsers.HasStubTemplate(@"foo {{foo-stub}}"));

            Assert.IsFalse(Parsers.HasStubTemplate(@"foo {{foo tubs}}"));
        }

        [Test]
        public void HasStubTemplateAr()
        {
#if DEBUG
            Variables.SetProjectLangCode("ar");
            WikiRegexes.MakeLangSpecificRegexes();
            
            Assert.IsTrue(Parsers.HasStubTemplate(@"foo {{بذرة ممثل}}"),"actor stub");
            Assert.IsTrue(Parsers.HasStubTemplate(@"foo {{بذرة ألمانيا}}"), "germany stub");
            
            Variables.SetProjectLangCode("en");
            WikiRegexes.MakeLangSpecificRegexes();
#endif
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
        public void IsStubAr()
        {
#if DEBUG
            Variables.SetProjectLangCode("ar");
            Assert.IsTrue(Parsers.IsStub(@"foo {{بذرة ممثل}}"));

            // short article
            Assert.IsTrue(Parsers.IsStub(@"foo {{foo tubs}}"));

            const string a = @"fooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo";
            const string b = a + a + a + a + a + a;
            Assert.IsFalse(Parsers.IsStub(b + b + b + b));
            Variables.SetProjectLangCode("en");
#endif
        }

        [Test]
        public void IsStubArz()
        {
#if DEBUG
            Variables.SetProjectLangCode("arz");
            Assert.IsTrue(Parsers.IsStub(@"foo {{بذرة}}"));

            // short article
            Assert.IsTrue(Parsers.IsStub(@"foo {{foo tubs}}"));

            const string a = @"fooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo";
            const string b = a + a + a + a + a + a;
            Assert.IsFalse(Parsers.IsStub(b + b + b + b));
            Variables.SetProjectLangCode("en");
#endif
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
            Assert.IsFalse(Parsers.CheckNoBots("<!--comm-->{{bots|deny=all}}", ""));
            Assert.IsFalse(Parsers.CheckNoBots(@"<!--comm
-->{{bots|deny=all}}", ""));
            Assert.IsFalse(Parsers.CheckNoBots("{{bots|deny=awb}}", ""));
            Assert.IsFalse(Parsers.CheckNoBots("{{bots|deny=awb,test}}", ""));
            Assert.IsFalse(Parsers.CheckNoBots("{{bots|deny=awb,test}}", "test"));
            Assert.IsFalse(Parsers.CheckNoBots("{{nobots|deny=awb,test}}", "test"));
            Assert.IsFalse(Parsers.CheckNoBots("{{bots|deny=test}}", "test"));
            Assert.IsFalse(Parsers.CheckNoBots("{{bots|allow=none}}", ""));
            Assert.IsFalse(Parsers.CheckNoBots(@"{{bots|deny=AWB, MenoBot, MenoBot II}}", "AWB and other bots"));
            Assert.IsTrue(Parsers.CheckNoBots("<!-- comm {{bots|deny=all}} -->", ""));

            Assert.IsFalse(Parsers.CheckNoBots(@"{{bots|allow=MiszaBot III,SineBot}}", "otherBot"));
            Assert.IsTrue(Parsers.CheckNoBots(@"{{bots|allow=MiszaBot III,SineBot}}", "SineBot"));
            Assert.IsTrue(Parsers.CheckNoBots(@"{{bots|allow=MiszaBot III,SineBot, AWB}}", "SomeotherBot, AWB last"));
            Assert.IsTrue(Parsers.CheckNoBots(@"{{bots|allow=AWB, MiszaBot III,SineBot}}", "SomeotherBot, AWB first"));

            /* prospective future changes to bots template
            Assert.IsTrue(Parsers.CheckNoBots(@"{{bots|deny=all|allow=MiszaBot III,SineBot}}", "SineBot"));
            Assert.IsTrue(Parsers.CheckNoBots(@"{{bots|deny=all|allow=MiszaBot III,SineBot}}", "MiszaBot III"));
            Assert.IsFalse(Parsers.CheckNoBots(@"{{bots|deny=all|allow=MiszaBot III,SineBot}}", "OtherBot")); */
        }

        [Test]
        public void NoIncludeIncludeOnlyProgrammingElement()
        {
            Assert.IsTrue(Parsers.NoIncludeIncludeOnlyProgrammingElement(@"<noinclude>blah</noinclude>"));
            Assert.IsTrue(Parsers.NoIncludeIncludeOnlyProgrammingElement(@"<includeonly>blah</includeonly>"));
            Assert.IsTrue(Parsers.NoIncludeIncludeOnlyProgrammingElement(@"<onlyinclude>blah</onlyinclude>"));
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
            Assert.AreEqual(@"{{foo}}", Parsers.GetTemplate(@"now {{foo}} was here {{foo|1}}", "foo"));

            Assert.AreEqual(@"", Parsers.GetTemplate(@"now {{ foo|bar asdfasdf}} was here", "foot"));
            Assert.AreEqual(@"", Parsers.GetTemplate(@"now {{ foo|bar asdfasdf}} was here", ""));
            Assert.AreEqual(@"", Parsers.GetTemplate(@"", "foo"));
            Assert.AreEqual(@"", Parsers.GetTemplate(@"now <!--{{foo}} --> was here", "foo"));
            Assert.AreEqual(@"{{foo<!--comm-->}}", Parsers.GetTemplate(@"now {{foo<!--comm-->}} was here", "foo"));

            Assert.AreEqual(@"{{foo  |a={{bar}} here}}", Parsers.GetTemplate(@"now {{foo  |a={{bar}} here}} was here", "foo"));
            Assert.AreEqual(@"{{foo|bar}}", Parsers.GetTemplate(@"now <!--{{foo|bar}}--> {{foo}} was here", "Foo"));
        }

        [Test]
        public void GetTemplatesTests()
        {
            const string foo1 = "{{foo|a}}", foo2 = "{{foo|b}}";
            string text = @"now " + foo1 + " and " + foo2;

            Regex foo = new Regex(@"{{foo.*?}}");
            List<Match> fred = new List<Match>();

            foreach (Match m in foo.Matches(text))
                fred.Add(m);

            Assert.AreEqual(fred.ToString(), Parsers.GetTemplates(text, "foo").ToString());
            Assert.AreEqual(fred.ToString(), Parsers.GetTemplates(text, "Foo").ToString());
            List<Match> templates = Parsers.GetTemplates(text, "foo");
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

            templates = Parsers.GetTemplates(@"{{test}}", "test");
            Assert.AreEqual(1, templates.Count);

            templates = Parsers.GetTemplates(@"{{test}}
", "test");
            Assert.AreEqual(1, templates.Count);
        }

        [Test]
        public void GetTemplatesEmbeddedComments()
        {
            const string foo1 = "{{foo|a}}", foo2 = "{{foo|b}}",
            foo2a = @"{{foo<!--comm-->|b}}", foo2b = @"{{foo|b<!--comm-->}}";
            string text = @"now " + foo1 + " and " + foo2;
            List<Match> templates = new List<Match>();

            // templates with embedded comments caught
            templates = Parsers.GetTemplates(text + " space " + foo2a, "foo");
            Assert.AreEqual(3, templates.Count);
            Assert.AreEqual(foo1, templates[0].Value);
            Assert.AreEqual(foo2, templates[1].Value);
            Assert.AreEqual(foo2a, templates[2].Value);

            templates = Parsers.GetTemplates(text + " space " + foo2b, "foo");
            Assert.AreEqual(3, templates.Count);
            Assert.AreEqual(foo1, templates[0].Value);
            Assert.AreEqual(foo2, templates[1].Value);
            Assert.AreEqual(foo2b, templates[2].Value);
        }

        [Test]
        public void GetTemplatesTestsAllTemplates()
        {
            const string foo1 = "{{foo|a}}", foo2 = "{{foo|b}}",
            foo2a = @"{{foo<!--comm-->|b}}", foo2b = @"{{foo|b<!--comm-->}}";
            string text = @"now " + foo1 + " and " + foo2;
            List<Match> templates = new List<Match>();

            // templates with embedded comments caught
            templates = Parsers.GetTemplates(text + " space " + foo2a);
            Assert.AreEqual(3, templates.Count);
            Assert.AreEqual(foo1, templates[0].Value);
            Assert.AreEqual(foo2, templates[1].Value);
            Assert.AreEqual(foo2a, templates[2].Value);

            templates = Parsers.GetTemplates(text + " space " + foo2b);
            Assert.AreEqual(3, templates.Count);
            Assert.AreEqual(foo1, templates[0].Value);
            Assert.AreEqual(foo2, templates[1].Value);
            Assert.AreEqual(foo2b, templates[2].Value);

            templates = Parsers.GetTemplates(@" {{one}} {{two}} {{three|a={{bcd}} |ef=gh}}");
            Assert.AreEqual(3, templates.Count);
        }

        [Test]
        public void FixUnicode()
        {
            // https://en.wikipedia.org/wiki/Wikipedia:AWB/B#Line_break_insertion

            Assert.AreEqual("foo bar", parser.FixUnicode("foo\x2028bar"));
            Assert.AreEqual(@"foo
bar", parser.FixUnicode("foo" + "\r\n" + "\x2028bar"));
            Assert.AreEqual("foo bar", parser.FixUnicode("foo\x2029bar"));
            Assert.AreEqual(@"foo
bar", parser.FixUnicode("foo" + "\r\n" + "\x200B\x200Bbar"));
        }

        [Test]
        public void SubstUserTemplates()
        {
            Regex Hello = new Regex(@"{{hello.*?}}");

            Assert.AreEqual(@"Text Expanded template test return<!-- {{hello|2010}} -->", Parsers.SubstUserTemplates(@"Text {{hello|2010}}", "test", Hello), "performs single substitution");
            Assert.AreEqual(@"Text Expanded template test return<!-- {{hello}} -->", Parsers.SubstUserTemplates(@"Text {{hello}}", "test", Hello), "performs single substitution");
            Assert.AreEqual(@"Text Expanded template test return<!-- {{hello}} -->
Expanded template test return<!-- {{hello2}} -->", Parsers.SubstUserTemplates(@"Text {{hello}}
{{hello2}}", "test", Hello), "performs multiple subsitutions");

            const string Bye = @"Text {{bye}}";
            Assert.AreEqual(Bye, Parsers.SubstUserTemplates(Bye, "test", Hello), "no changes if no matching template");

            const string Subst = @"Now {{{subst:bar}}} text";
            Assert.AreEqual(Subst, Parsers.SubstUserTemplates(Subst, "test", Hello), "doesn't change {{{subst");

            Regex None = null;
            Assert.AreEqual(Bye, Parsers.SubstUserTemplates(Bye, "test", None), "no changes when user talk page regex is null");

            const string T2 = @"Test {{{2|}}}";
            Assert.AreEqual("Test", Parsers.SubstUserTemplates(T2, "test", Hello), "cleans up the {{{2|}}} template");
        }

        [Test]
        public void FormatToBDA()
        {
            const string Correct = @"{{birth date and age|df=y|1990|05|11}}", CorrectAmerican = @"{{birth date and age|mf=y|1990|05|11}}";

            Assert.AreEqual(Correct, Parsers.FormatToBDA(@"11 May 1990 (age 21)"));
            Assert.AreEqual(Correct, Parsers.FormatToBDA(@"11 May 1990 (Age 21)"));
            Assert.AreEqual(Correct, Parsers.FormatToBDA(@"11 May 1990, (Age 21)"));
            Assert.AreEqual(Correct, Parsers.FormatToBDA(@"11 May 1990; (Age 21)"));
            Assert.AreEqual(Correct, Parsers.FormatToBDA(@"11 May 1990 (aged 21)"));
            Assert.AreEqual(Correct, Parsers.FormatToBDA(@"11 May [[1990]] (aged 21)"));
            Assert.AreEqual(Correct, Parsers.FormatToBDA(@"[[11 May]] [[1990]] (aged 21)"));
            Assert.AreEqual(Correct, Parsers.FormatToBDA(@"[[11 May]] 1990 (aged 21)"));
            Assert.AreEqual(Correct, Parsers.FormatToBDA(@"11th May 1990 (aged 21)"));
            Assert.AreEqual(Correct, Parsers.FormatToBDA(@"11 May 1990 ( aged 21 )"));
            Assert.AreEqual(Correct, Parsers.FormatToBDA(@"11 May 1990    ( aged 21 )"));
            Assert.AreEqual(Correct, Parsers.FormatToBDA(@"11 May, 1990    ( aged 21 )"));

            Assert.AreEqual(Correct, Parsers.FormatToBDA(@"1990-05-11 (age 21)"));
            Assert.AreEqual(Correct, Parsers.FormatToBDA(@"1990-5-11 (age 21)"));
            Assert.AreEqual(Correct, Parsers.FormatToBDA(@"1990-5-11 <br/>(age 21)"));
            Assert.AreEqual(Correct, Parsers.FormatToBDA(@"1990-5-11 <br>(age 21)"));
            Assert.AreEqual(Correct, Parsers.FormatToBDA(@"1990-5-11 <br>Age 21"));

            Assert.AreEqual(CorrectAmerican, Parsers.FormatToBDA(@"May 11, 1990 (age 21)"));
            Assert.AreEqual(CorrectAmerican, Parsers.FormatToBDA(@"May 11 1990 (age 21)"));
            Assert.AreEqual(CorrectAmerican, Parsers.FormatToBDA(@"May 11th 1990 (age 21)"));

            Assert.AreEqual("", Parsers.FormatToBDA(""));
            Assert.AreEqual("Test", Parsers.FormatToBDA("Test"));
            Assert.AreEqual("May 11, 1990", Parsers.FormatToBDA("May 11, 1990"));
            Assert.AreEqual("May 1990 (age 21)", Parsers.FormatToBDA("May 1990 (age 21)"));
            Assert.AreEqual("May 11, 1990 (Age 21) and some other text", Parsers.FormatToBDA("May 11, 1990 (Age 21) and some other text"));
        }

        [Test]
        public void COinS()
        {
            string data1 = @"""coins"": ""ctx_ver=Z39.88-2004&amp;rft_id=info%3Adoi%2Fhttp%3A%2F%2Fdx.doi.org%2F10.1007%2Fs11046-005-4332-4&amp;rfr_id=info%3Asid%2Fcrossref.org%3Asearch&amp;rft.atitle=Morphological+alterations+in+toxigenic+Aspergillus+parasiticus+exposed+to+neem+%28Azadirachta+indica%29+leaf+and+seed+aqueous+extracts&amp;rft.jtitle=Mycopathologia&amp;rft.date=2005&amp;rft.volume=159&amp;rft.issue=4&amp;rft.spage=565&amp;rft.epage=570&amp;rft.aufirst=Mehdi&amp;rft.aulast=Razzaghi-Abyaneh&amp;rft_val_fmt=info%3Aofi%2Ffmt%3Akev%3Amtx%3Ajournal&amp;rft.genre=article&amp;rft.au=Mehdi+Razzaghi-Abyaneh&amp;rft.au=+Abdolamir+Allameh&amp;rft.au=+Taki+Tiraihi&amp;rft.au=+Masoomeh+Shams-Ghahfarokhi&amp;rft.au=+Mehdi+Ghorbanian""";
            Dictionary<string, string> res = Parsers.ExtractCOinS(data1);
            Assert.IsTrue(res.ContainsKey("volume"));
            Assert.IsTrue(res.ContainsKey("issue"));
            Assert.IsTrue(res.ContainsKey("spage"));
            Assert.IsTrue(res.ContainsKey("aulast"));
            Assert.IsTrue(res.ContainsKey("atitle"));
            Assert.IsTrue(res.ContainsKey("date"));
            string v;
            res.TryGetValue("volume", out v);
            Assert.AreEqual(v, "159");
        }
    }

    [TestFixture]
    public class RecategorizerTests : RequiresParser
    {
        [Test]
        public void Addition()
        {
            bool noChange;

            Assert.AreEqual("\r\n\r\n[[Category:Foo]]", parser.AddCategory("Foo", "", "bar", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("bar\r\n\r\n[[Category:Foo]]", parser.AddCategory("Foo", "bar", "bar", out noChange));
            Assert.IsFalse(noChange);

            Assert.AreEqual("test\r\n\r\n[[Category:Foo|bar]]\r\n[[Category:Bar]]",
                            parser.AddCategory("Bar", "test[[Category:Foo|bar]]", "foo", out noChange));
            Assert.IsFalse(noChange);

            // shouldn't add if category already exists
            Assert.AreEqual("[[Category:Foo]]", parser.AddCategory("Foo", "[[Category:Foo]]", "bar", out noChange));
            Assert.IsTrue(noChange);
            Assert.IsTrue(Regex.IsMatch(parser.AddCategory("Foo bar", "[[Category:Foo_bar]]", "bar", out noChange), @"\[\[Category:Foo[ _]bar\]\]"));
            Assert.IsTrue(noChange);

            Assert.AreEqual("[[category : foo_bar%20|quux]]", parser.AddCategory("Foo bar", "[[category : foo_bar%20|quux]]", "bar", out noChange));
            Assert.IsTrue(noChange);

            Assert.AreEqual("test<noinclude>\r\n[[Category:Foo]]\r\n</noinclude>",
                            parser.AddCategory("Foo", "test", "Template:foo", out noChange));
            Assert.IsFalse(noChange);

            // don't change cosmetic whitespace when adding a category
            const string Newlineheading = @"==Persian==

===Pronunciation===";
            Assert.AreEqual(Newlineheading + "\r\n\r\n" + @"[[Category:Foo]]", parser.AddCategory("Foo", Newlineheading, "bar", out noChange));
        }

        [Test]
        public void Replacement()
        {
            bool noChange;

            Assert.AreEqual("[[Category:Bar]]", Parsers.ReCategoriser("Foo", "Bar", "[[Category:Foo]]", out noChange));
            Assert.IsFalse(noChange);

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_7#Replacing_Arabic_categories
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

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_7#Replacing_Arabic_categories
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
    public class ConversionTests : RequiresParser
    {
        [Test]
        public void ConversionsTestsInfoBox()
        {
            string correct = @"{{infobox foo|date=May 2010}}";

            Assert.AreEqual(correct, Parsers.Conversions(@"{{infobox_foo|date=May 2010}}"));
            Assert.AreEqual(correct, Parsers.Conversions(correct));

            correct = @"{{infobox foo
|date=May 2010}}";
            Assert.AreEqual(correct, Parsers.Conversions(correct));

            correct = @"{{infobox
|date=May 2010}}";
            Assert.AreEqual(correct, Parsers.Conversions(correct));

            correct = @"{{infobox foo|date=May 2010}}";
            Assert.AreEqual(correct, Parsers.Conversions(@"{{Template:infobox_foo|date=May 2010}}"));
        }

        [Test]
        public void ConversionsTestsUnreferenced()
        {
            string correct = @"{{Unreferenced section|date=May 2010}}";
            Assert.AreEqual(correct, Parsers.Conversions(@"{{Unreferenced|section|date=May 2010}}"));
            Assert.AreEqual(correct, Parsers.Conversions(@"{{Unreferenced|Section|date=May 2010}}"));
            Assert.AreEqual(correct, Parsers.Conversions(@"{{Unreferenced|  section |date=May 2010}}"));
            Assert.AreEqual(correct, Parsers.Conversions(correct));
            Assert.AreEqual(correct.Replace("U", "u"), Parsers.Conversions(@"{{unreferenced|section|date=May 2010}}"));
            Assert.AreEqual(@"{{Unreferenced section|date=May 2010}}", Parsers.Conversions(@"{{Unreferenced|list|date=May 2010}}"));
            Assert.AreEqual(@"{{Unreferenced section|date=May 2010}}", Parsers.Conversions(@"{{Unreferenced|type=section|date=May 2010}}"));
            Assert.AreEqual(@"{{Unreferenced section|date=May 2010}}", Parsers.Conversions(@"{{Unreferenced|type=Section|date=May 2010}}"));

            Assert.AreEqual(@"{{Unreferenced|date=May 2010}}", Parsers.Conversions(@"{{Unreferenced|date=May 2010|auto=yes}}"));
            Assert.AreEqual(@"{{Unreferenced|date=May 2010}}", Parsers.Conversions(@"{{Unreferenced|date=May 2010|auto=YES}}"));

            Assert.IsTrue(Parsers.Conversions(@"{{unreferenced|date=October 2011}}
'''Gretchen F''' known as is a Filipina model.

==Reference==
*http://www.pep.ph/p
{{DEFAULTSORT:Gretchen F}}
[[Category:1980 births]]").Contains(@"unreferenced"), "no template renaming when within multiple issues");
        }

        [Test]
        public void ConversionsTestsSectionTemplates()
        {
            string correct = @"{{Unreferenced section|date=May 2010}}";
            Assert.AreEqual(correct, Parsers.Conversions(@"{{Unreferenced|section|date=May 2010}}"));

            correct = @"{{refimprove section|date=May 2010}}";
            Assert.AreEqual(correct, Parsers.Conversions(@"{{refimprove|section|date=May 2010}}"));

            correct = @"{{BLP sources section|date=May 2010}}";
            Assert.AreEqual(correct, Parsers.Conversions(@"{{BLP sources|section|date=May 2010}}"));

            correct = @"{{expand section|date=May 2010}}";
            Assert.AreEqual(correct, Parsers.Conversions(@"{{expand|section|date=May 2010}}"));

            correct = @"{{BLP unsourced section|date=May 2010}}";
            Assert.AreEqual(correct, Parsers.Conversions(@"{{BLP unsourced|section|date=May 2010}}"));
        }

        [Test]
        public void RemoveExcessTemplatePipes()
        {
            // extra pipe
            Assert.AreEqual(@"{{Multiple issues|sections=May 2008|POV=March 2008|COI=May 2009}}", Parsers.Conversions(@"{{Multiple issues|sections=May 2008||POV=March 2008|COI=May 2009}}"));
            Assert.AreEqual(@"{{cite web | url=http://www.site.com | title=hello}}", Parsers.FixCitationTemplates(@"{{cite web | url=http://www.site.com || title=hello}}"));
            Assert.AreEqual(@"{{cite web | url=http://www.site.com | title=hello}}", Parsers.FixCitationTemplates(@"{{cite web || url=http://www.site.com || title=hello}}"));
            Assert.AreEqual(@"{{cite web | url=http://www.site.com | title=hello}}", Parsers.FixCitationTemplates(@"{{cite web | url=http://www.site.com | | title=hello}}"));
            Assert.AreEqual(@"{{cite wikisource|bar||foo}}", Parsers.FixCitationTemplates(@"{{cite wikisource|bar||foo}}"));

            Assert.AreEqual(@"{{cite uscgll|bar||foo}}", Parsers.FixCitationTemplates(@"{{cite uscgll|bar||foo}}"));
            Assert.AreEqual(@"{{cite ngall|bar||foo}}", Parsers.FixCitationTemplates(@"{{cite ngall|bar||foo}}"));
            Assert.AreEqual(@"{{Cite Legislation AU|bar||foo}}", Parsers.FixCitationTemplates(@"{{Cite Legislation AU|bar||foo}}"));

            const string UnclosedCiteInTable = @"
|
|Yes<ref>{{cite web | title=A </ref>
|
|";
            Assert.AreEqual(UnclosedCiteInTable, Parsers.FixCitationTemplates(UnclosedCiteInTable), "Does not alter table pipes following unclosed ref cite");
        }

        [Test]
        public void ConversionTestsCommonsCat()
        {
            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#.7B.7Bcommons.7CCategory:XXX.7D.7D_.3E_.7B.7Bcommonscat.7CXXX.7D.7D
            // {{commons|Category:XXX}} > {{commonscat|XXX}}
            Assert.AreEqual(@"{{Commons category|XXX}}", Parsers.Conversions(@"{{commons|Category:XXX}}"));
            Assert.AreEqual(@"{{Commons category|XXX}}", Parsers.Conversions(@"{{Commons|category:XXX}}"));
            Assert.AreEqual(@"{{Commons category|XXX}}", Parsers.Conversions(@"{{Commons| category:XXX }}"));
            Assert.AreEqual(@"{{Commons category|Backgammon}}", Parsers.Conversions(@"{{commons|Category:Backgammon|Backgammon}}"));
            Assert.AreEqual(@"{{Commons category|Backgammon}}", Parsers.Conversions(@"{{commons|Category:Backgammon | Backgammon  }}"));
            Assert.AreEqual(@"{{Commons category|Backgammon|Backgammon main}}", Parsers.Conversions(@"{{Commons|Category:Backgammon|Backgammon main}}"));
            Assert.AreEqual(@"{{commons cat|Gander International Airport}}", Parsers.Conversions(@"{{commons cat|Gander International Airport|Gander International Airport}}"));
            Assert.AreEqual(@"{{Commons cat|Gander International Airport}}", Parsers.Conversions(@"{{Commons cat|Gander International Airport|Gander International Airport}}"));
        }

        [Test]
        public void ConversionTestsMoreFootnotes()
        {
            // no footnotes --> more footnotes
            Assert.AreEqual(@"Article <ref>A</ref>
            ==References==
            {{more footnotes}}
            {{reflist}}", Parsers.Conversions(@"Article <ref>A</ref>
            ==References==
            {{no footnotes}}
            {{reflist}}"));

            Assert.AreEqual(@"Article <ref>A</ref>
            ==References==
            {{more footnotes}}
            {{reflist}}", Parsers.Conversions(@"Article <ref>A</ref>
            ==References==
            {{no footnotes}}
            {{reflist}}"));

            Assert.AreEqual(@"Article {{sfn|Smith|2004}}
            ==References==
            {{more footnotes}}
            {{reflist}}", Parsers.Conversions(@"Article {{sfn|Smith|2004}}
            ==References==
            {{no footnotes}}
            {{reflist}}"));

            Assert.AreEqual(@"Article {{efn|Converting at a rate of Kr 20 = £1}}
            ==References==
            {{more footnotes}}
            {{reflist}}", Parsers.Conversions(@"Article {{efn|Converting at a rate of Kr 20 = £1}}
            ==References==
            {{no footnotes}}
            {{reflist}}"));

            Assert.AreEqual(@"Article {{sfn|Smith|2004}}
            ==References==
            {{more footnotes|date=May 2012}}
            {{reflist}}", Parsers.Conversions(@"Article {{sfn|Smith|2004}}
            ==References==
            {{no footnotes|date=May 2012}}
            {{reflist}}"));

            Assert.AreEqual(@"Article {{sfn|Smith|2004}}
            ==References==
            {{more footnotes|BLP=yes|date=May 2012}}
            {{reflist}}", Parsers.Conversions(@"Article {{sfn|Smith|2004}}
            ==References==
            {{no footnotes|BLP=yes|date=May 2012}}
            {{reflist}}"));

            Assert.AreEqual(@"Article {{sfn|Smith|2004}}
            ==References==
            {{more footnotes|article=yes|date=May 2012}}
            {{reflist}}", Parsers.Conversions(@"Article {{sfn|Smith|2004}}
            ==References==
            {{no footnotes|article=yes|date=May 2012}}
            {{reflist}}"));

            const string NoChange = @"Article
            ==References==
            {{no footnotes}}";
            Assert.AreEqual(NoChange, Parsers.Conversions(NoChange));

            const string NoFootnotesSection = @"Article <ref>A</ref>
            ==Sec==
            {{no footnotes|section}}
            ==References==
            {{reflist}}";

            Assert.AreEqual(NoFootnotesSection, Parsers.Conversions(NoFootnotesSection));

            const string NoFootnotesReason = @"Article <ref>A</ref>
            ==Sec==
            {{no footnotes|some reason}}
            ==References==
            {{reflist}}";

            Assert.AreEqual(NoFootnotesReason, Parsers.Conversions(NoFootnotesReason));

            const string NoChange2 = @"Article<ref name=A group=B>A</ref>
            ==References==
            {{no footnotes}}";
            Assert.AreEqual(NoChange2, Parsers.Conversions(NoChange2));
        }

        [Test]
        public void ConversionTestsBLPUnsourced()
        {
            string correct = @"Foo
{{BLP unsourced}}
[[Category:Living people]]", nochange = @"Foo
{{unreferenced}}";

            Assert.AreEqual(correct, Parsers.Conversions(nochange + "\r\n" + @"[[Category:Living people]]"));

            Assert.AreEqual(correct, Parsers.Conversions(correct));

            Assert.AreEqual(nochange, Parsers.Conversions(nochange));

            nochange = @"Foo {{unreferenced|blah}}" + @"[[Category:Living people]]";
            Assert.AreEqual(nochange, Parsers.Conversions(nochange), "no change when free-format text in unreferenced first argument");

            Assert.AreEqual(@"Foo
{{BLP unsourced|date=May 2010}}
[[Category:Living people]]", Parsers.Conversions(@"Foo
{{unreferenced|date=May 2010}}
[[Category:Living people]]"));

            Assert.AreEqual(@"Foo
{{BLP unsourced|Date=May 2010}}
[[Category:Living people]]", Parsers.Conversions(@"Foo
{{unreferenced|Date=May 2010}}
[[Category:Living people]]"));

            Assert.AreEqual(@"{{Multiple issues|BLP unsourced=January 2013|essay-like =June 2008}}
Foo
[[Category:Living people]]", Parsers.Conversions(@"{{Multiple issues|unreferenced=January 2013|essay-like =June 2008}}
Foo
[[Category:Living people]]"));
        }

        [Test]
        public void ConversionTestsBLPUnsourcedSection()
        {
            string correct = @"Foo
{{BLP unsourced section}}
[[Category:Living people]]", nochange = @"Foo
{{unreferenced section}}";

            Assert.AreEqual(correct, Parsers.Conversions(nochange + "\r\n" + @"[[Category:Living people]]"));
            Assert.AreEqual(correct, Parsers.Conversions(@"Foo
{{unreferenced section}}" + "\r\n" + @"[[Category:Living people]]"));

            Assert.AreEqual(correct, Parsers.Conversions(correct));

            Assert.AreEqual(nochange, Parsers.Conversions(nochange));
        }

        [Test]
        public void ConversionTestsBLPPrimarySources()
        {
            string correct = @"Foo
{{BLP primary sources}}
[[Category:Living people]]", nochange = @"Foo
{{primary sources}}";

            Assert.AreEqual(correct, Parsers.Conversions(nochange + "\r\n" + @"[[Category:Living people]]"));
            Assert.AreEqual(correct, Parsers.Conversions(@"Foo
{{primary sources}}" + "\r\n" + @"[[Category:Living people]]"));

            Assert.AreEqual(correct, Parsers.Conversions(correct));

            Assert.AreEqual(nochange, Parsers.Conversions(nochange));
        }

        [Test]
        public void ConversionTestsBLPSources()
        {
            string correct = @"Foo
{{BLP sources}}
[[Category:Living people]]", nochange = @"Foo
{{refimprove}}";

            Assert.AreEqual(correct, Parsers.Conversions(nochange + "\r\n" + @"[[Category:Living people]]"));
            Assert.AreEqual(correct.Replace(":", " : "), Parsers.Conversions(nochange + "\r\n" + @"[[Category : Living people]]"));
            Assert.AreEqual(correct, Parsers.Conversions(@"Foo
{{refimprove}}" + "\r\n" + @"[[Category:Living people]]"));

            Assert.AreEqual(correct, Parsers.Conversions(correct));

            Assert.AreEqual(nochange, Parsers.Conversions(nochange));

            nochange = @"Foo {{refimprove|blah}}" + @"[[Category:Living people]]";
            Assert.AreEqual(nochange, Parsers.Conversions(nochange), "no change when free-format text in refimprove first argument");

            Assert.AreEqual(@"Foo
{{BLP sources|date=May 2010}}
[[Category:Living people]]", Parsers.Conversions(@"Foo
{{refimprove|date=May 2010}}
[[Category:Living people]]"),"do conversion");

            Assert.AreEqual(@"Foo
{{BLP sources|Date=May 2010}}

[[Category:Living people]]", Parsers.Conversions(@"Foo
{{BLP sources|Date=May 2010}}
{{refimprove|Date=May 2010}}
[[Category:Living people]]"),"when have existing BLP sources then remove refimprove");

        }

        [Test]
        public void ConversionTestsInterwikiMigration()
        {
            Assert.AreEqual(@"{{hello}}", Parsers.Conversions(@"{{msg:hello}}"));
            Assert.AreEqual(@"[[zh:foo]]", Parsers.InterwikiConversions(@"[[zh-tw:foo]]"));
            Assert.AreEqual(@"[[no:foo]]", Parsers.InterwikiConversions(@"[[nb:foo]]"));
            Assert.AreEqual(@"[[da:foo]]", Parsers.InterwikiConversions(@"[[dk:foo]]"));
        }
        [Test]
        public void PageNameTests()
        {
            Assert.AreEqual(@"{{subst:PAGENAME}}", Parsers.Conversions(@"{{PAGENAME}}"));
            Assert.AreEqual(@"{{subst:PAGENAMEE}}", Parsers.Conversions(@"{{PAGENAMEE}}"));
            Assert.AreEqual(@"{{subst:PAGENAME}}", Parsers.Conversions(@"{{template:PAGENAME}}"));
            Assert.AreEqual(@"{{subst:BASEPAGENAME}}", Parsers.Conversions(@"{{BASEPAGENAME}}"));

            Assert.AreEqual(@"{{DEFAULTSORT:{{subst:PAGENAME}}}}", Parsers.Conversions(@"{{DEFAULTSORT:{{PAGENAME}}}}"));

            const string RefPAGENAME = @"<ref> Some text {{PAGENAME}} here</ref>";

            Assert.AreEqual(RefPAGENAME, Parsers.Conversions(RefPAGENAME), "No subst: within ref tags");
            Assert.AreEqual(@"<ref>a</ref> {{subst:BASEPAGENAME}}", Parsers.Conversions(@"<ref>a</ref> {{BASEPAGENAME}}"), "Subst oustide ref tags");
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
            Assert.AreEqual(@"{{Multiple issues|cleanup=May 2008|POV=March 2008}}", Parsers.Conversions(@"{{Multiple issues|cleanup=May 2008|POV=March 2008}}"));
            Assert.AreEqual(@"{{Multiple issues|sections=May 2008|POV=March 2008}}", Parsers.Conversions(@"{{Multiple issues|sections=May 2008|POV=March 2008}}"));
        }

        [Test]
        public void DuplicateTemplateFieldsTests()
        {
            Assert.AreEqual("", Parsers.Conversions(""));

            Assert.AreEqual(@"{{Multiple issues|wikify=May 2008|POV=May 2008|Expand=June 2008}}", Parsers.Conversions(@"{{Multiple issues|wikify=May 2008|POV=May 2008|Expand=June 2008|Expand=June 2008}}"));
            Assert.AreEqual(@"{{Multiple issues|wikify=May 2008|POV=May 2008|Expand=June 2008}}", Parsers.Conversions(@"{{Multiple issues|wikify=May 2008|Expand=June 2008|POV=May 2008|Expand=June 2008}}"));

            Assert.AreEqual(@"{{Multiple issues|wikify=May 2008|Expand=June 2008|POV=May 2008}}", Parsers.Conversions(@"{{Multiple issues|wikify=May 2008|Expand=June 2008|POV=May 2008|Expand=}}"));
            Assert.AreEqual(@"{{Multiple issues|wikify=May 2008|POV=May 2008|Expand=June 2008}}", Parsers.Conversions(@"{{Multiple issues|wikify=May 2008|Expand=|POV=May 2008|Expand=June 2008}}"));

            Assert.AreEqual(@"{{Multiple issues|wikify=May 2008|POV=May 2008|Expand=June 2008|Expand=June 2009}}", Parsers.Conversions(@"{{Multiple issues|wikify=May 2008|POV=May 2008|Expand=June 2008|Expand=June 2009}}"));
        }
    }

    [TestFixture]
    public class TaggerTests : RequiresParser
    {
        private bool noChange;
        private string summary;

        private const string Uncat = "{{Uncategorized|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}",
        UncatStub = "{{Uncategorized stub|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}",
        Orphan = "{{orphan|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}",
        Wikify = "{{Wikify|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}",
        Deadend = "{{Deadend|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}",
        Stub = "{{stub}}";

        [SetUp]
        public void SetUp()
        {
#if DEBUG
            Variables.SetProjectLangCode("en");
#endif
        }

        [Test]
        public void AddUncatStub()
        {
            Globals.UnitTestIntValue = 0;
            Globals.UnitTestBoolValue = true;

            string text = parser.Tagger(ShortText, "Test", false, out noChange, ref summary);
            //Stub, no existing stub tag. Needs all tags
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text), "page is orphan");
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text), "page is deadend");
            Assert.IsFalse(text.Contains("Underlinked"));
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text), "page is stub");
            Assert.IsTrue(text.Contains(UncatStub), "page is uncategorised stub");

            // uncat when not a stub
            Globals.UnitTestIntValue = 0;
            Globals.UnitTestBoolValue = true;

            text = parser.Tagger(LongText, "Test", false, out noChange, ref summary);
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text), "page is orphan");
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text));
            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text));

            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(text));
            Assert.IsFalse(text.Contains(UncatStub));

            // stub already marked uncat
            text = parser.Tagger(ShortText + @"{{uncat}}", "Test", false, out noChange, ref summary);
            //Stub, no existing stub tag. Needs all tags
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text), "uncat page and orphan");
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text),"uncat page and deadend");
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text));
            Assert.IsFalse(Tools.NestedTemplateRegex("uncat").IsMatch(text));
            Assert.IsTrue(Tools.NestedTemplateRegex("uncategorized stub").IsMatch(text));

            // stub already marked uncat but with "List of..." in pagetitle. It should not tag as stub
            text = parser.Tagger(ShortText + @"{{uncat}}", "List of Tests", false, out noChange, ref summary);
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text), "page is orphan");
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text));
            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text));
            Assert.IsTrue(Tools.NestedTemplateRegex("uncat").IsMatch(text));
            Assert.IsFalse(Tools.NestedTemplateRegex("uncategorized stub").IsMatch(text));
            
            // stub already marked uncat but with "Lists of..." in pagetitle. It should not tag as stub
            text = parser.Tagger(ShortText + @"{{uncat}}", "Lists of Tests", false, out noChange, ref summary);
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text), "page is orphan");
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text));
            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text));
            Assert.IsTrue(Tools.NestedTemplateRegex("uncat").IsMatch(text));
            Assert.IsFalse(Tools.NestedTemplateRegex("uncategorized stub").IsMatch(text));

            // Pages with Centuryinbox already have a lot of wikilinks. They should not be tagged as deadend.
            text = parser.Tagger(@"one two three four five six seven eight nine ten {{Centuryinbox
| in?=in poetry
| cpa=19
| cpb=th century
| c=20th century
| cn1=21st century
}} eleven twelve", "Test", false, out noChange, ref summary);
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text),"Tag for orphan");
            Assert.IsFalse(WikiRegexes.Wikify.IsMatch(text),"Don't tag for underlinked");
            Assert.IsFalse(WikiRegexes.DeadEnd.IsMatch(text),"Don't tag for deadend");
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text),"Tag for stub");
            Assert.IsTrue(Tools.NestedTemplateRegex("uncategorized stub").IsMatch(text),"Tag for uncat stub");

            // Pages with MinorPlanetListFooter will have wikilinks. They should not be tagged as deadend.
            text = parser.Tagger(@"A {{MinorPlanetListFooter|A}} B", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.DeadEnd.IsMatch(text),"Don't tag for deadend");
        }

        [Test]
        public void RenameUncategorised()
        {
            Globals.UnitTestIntValue = 0;

            string text = parser.Tagger(ShortText + @"{{stub}} {{Uncategorised|date=May 2010}}", "Test", false, out noChange, ref summary);
            Assert.IsTrue(Tools.NestedTemplateRegex("Uncategorized stub").IsMatch(text), "Uncategorised renamed to uncat stub");
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text));

            text = parser.Tagger(ShortText + @"{{stub}} {{Uncategorisedstub|date=May 2010}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(Tools.NestedTemplateRegex("Uncategorized stub").IsMatch(text), "uncatstub not renamed when already present");
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text));

            text = parser.Tagger(LongText + @"{{Uncategorised|date=May 2010}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(Tools.NestedTemplateRegex("Uncategorized stub").IsMatch(text), "uncategorized not renamed when not stub");
            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text));

            text = parser.Tagger(LongText + @"{{stub}} {{Uncategorised|date=May 2010}}", "Test", false, out noChange, ref summary);
            Assert.IsTrue(Tools.NestedTemplateRegex("Uncategorised").IsMatch(text), "Uncategorised not renamed when stub removed");
            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text));

            text = parser.Tagger(ShortText + @"{{stub}} {{Uncategorized stub|date=May 2010}} {{Uncategorised|date=May 2010}}", "Test", false, out noChange, ref summary);
            Assert.IsTrue(Tools.NestedTemplateRegex("Uncategorized stub").IsMatch(text), "Uncat stub retained");
            Assert.IsFalse(Tools.NestedTemplateRegex("Uncategorised").IsMatch(text), "Uncategorised removed when already uncat stub");
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text));

            text = parser.Tagger(LongText + @"{{stub}} {{Uncategorized stub|date=May 2010}} {{Uncategorised|date=May 2010}}", "Test", false, out noChange, ref summary);
            Assert.IsTrue(Tools.NestedTemplateRegex("Uncategorised").IsMatch(text), "Uncategorised not renamed when stub removed");
            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text));

            text = parser.Tagger("{{wikify}}" + Regex.Replace(LongText, @"(\w+)", "[[$1]]"), "Test", false, out noChange, ref summary);

            Assert.IsFalse(WikiRegexes.Wikify.IsMatch(text));

            text = parser.Tagger("{{wikify|reason=something}}" + Regex.Replace(LongText, @"(\w+)", "[[$1]]"), "Test", false, out noChange, ref summary);

            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(text), "wikify tag with reason NOT removed");

            Globals.UnitTestIntValue = 4;
            text = parser.Tagger("{{uncategorised}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text));

            text = parser.Tagger("{{uncategorised|date=January 2009}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text));

            text = parser.Tagger("{{uncategorised|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text));

            text = parser.Tagger("{{uncategorised|date=January 2009}} {{foo}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text), "Uncat removed even if other template present");

            Globals.UnitTestIntValue = 0;
            text = parser.Tagger(ShortText + @"{{dead end}}
{{reflist}}
{{Uncategorised|date=May 2010}}{{stub}}", "Test", false, out noChange, ref summary);
            Assert.IsTrue(text.EndsWith(@"{{reflist}}


{{Uncategorized stub|date=May 2010}}
{{stub}}"));
        }

        [Test]
        public void RenameUnreferenced()
        {
            Globals.UnitTestIntValue = 0;
            Globals.UnitTestBoolValue = false;

            string text = parser.Tagger(ShortText + @"{{unreferenced|date=May 2010}}", "Test", false, out noChange, ref summary);

            Assert.IsTrue(text.Contains("unreferenced"), "Unref when no refs");

            text = parser.Tagger(ShortText + @"{{unreferenced|date=May 2010}} <!--<ref>foo</ref>-->", "Test", false, out noChange, ref summary);
            Assert.IsTrue(text.Contains("unreferenced"), "Unref when no refs 2");

            text = parser.Tagger(ShortText + @"{{unreferenced|date=May 2010}} <ref>foo</ref>", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Unreferenced.IsMatch(text), "Unref to refimprove when no refs 3");
            Assert.IsTrue(text.Contains("refimprove"), "Unref when no refs 4");
            Assert.AreEqual(Tools.GetTemplateParameterValue(Tools.NestedTemplateRegex("refimprove").Match(text).Value, "date"), "{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}", "Date updated on change of template name");

            text = parser.Tagger(@"{{Multiple issues|COI = February 2009|wikify = April 2009|unreferenced = April 2007}} <ref>foo</ref>", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Unreferenced.IsMatch(text), "Unref to refimprove when has refs");
            Assert.IsTrue(text.Contains("refimprove"), "Renames unreferenced to refimprove in MI parameter when existing refs");

            text = parser.Tagger(ShortText + @"{{unreferenced|date=May 2010}} <ref group=X>foo</ref>", "Test", false, out noChange, ref summary);
            Assert.IsTrue(WikiRegexes.Unreferenced.IsMatch(text), "Unref remains when only grouped footnote refs");

            text = parser.Tagger(@"{{Multiple issues|COI = February 2009|wikify = April 2009|unreferenced = April 2007}} {{sfn|Smith|2004}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Unreferenced.IsMatch(text), "Unref to refimprove when has sfn refs");
            Assert.IsTrue(WikiRegexes.MultipleIssues.Match(text).Value.Contains("refimprove"), "Renames unreferenced to refimprove in MI parameter when existing refs");

            text = parser.Tagger(@"{{Multiple issues|COI = February 2009|wikify = April 2009|unreferenced = April 2007}} {{efn|A clarification.<ref name=Smith2009/>}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Unreferenced.IsMatch(text), "Unref to refimprove when has efn refs");
            Assert.IsTrue(WikiRegexes.MultipleIssues.Match(text).Value.Contains("refimprove"), "Renames unreferenced to refimprove in MI parameter when existing refs");

            text = parser.Tagger(@"{{unreferenced|date = April 2007}} <ref>foo</ref>==sec=={{Multiple issues|section=y|COI = February 2009|POV = April 2009|refimprove = April 2009}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Unreferenced.IsMatch(text), "Unref to refimprove when has refs");
            Assert.IsTrue(text.Contains(@"{{Multiple issues|section=y|COI = February 2009|POV = April 2009|refimprove = April 2009|"), "Doesn't update date of refimprove in section MI template");

            text = parser.Tagger(ShortText + @"{{unreferenced|date=May 2010}} {{refimprove|date=May 2010}} <ref>foo</ref>", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Unreferenced.IsMatch(text), "Unref removed when refimprove");
            Assert.AreEqual(1, Tools.NestedTemplateRegex("refimprove").Matches(text).Count);

            text = parser.Tagger(ShortText + @"{{multiple issues|
{{unreferenced|date=May 2010}}
{{refimprove|date=May 2010}}
}} <ref>foo[[bar]]</ref>", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Unreferenced.IsMatch(text), "Unref removed when refimprove");
            Assert.AreEqual(1, Tools.NestedTemplateRegex("refimprove").Matches(text).Count);
            Assert.AreEqual(0, Tools.NestedTemplateRegex("multiple issues").Matches(text).Count, "Multiple issues removed if unref removed ant MI no longer needed");
        }

        [Test]
        public void AddDeadEnd()
        {
            Globals.UnitTestIntValue = 0;
            Globals.UnitTestBoolValue = true;

            string text = parser.Tagger(ShortText + "{{Underlinked}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(text.Contains("Underlinked"), "underlinked removed when dead end");
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text), "page is deadend");
            Assert.IsTrue(summary.Contains("removed underlinked"));

            text = parser.Tagger(ShortText + @"{{shipindex}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.DeadEnd.IsMatch(text), "SIA page not tagged as deadend");
        }

        [Test]
        public void AddMultipleTagsWihtespace()
        {
            Globals.UnitTestIntValue = 0;
            Globals.UnitTestBoolValue = true;

            string text = parser.Tagger(ShortText, "Test", false, out noChange, ref summary);
            Assert.IsTrue(text.Contains(@"{{Dead end|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}
{{Orphan|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"), "No excess blank line between multiple tags");
        }

        [Test]
        public void Add()
        {
            Globals.UnitTestIntValue = 0;
            Globals.UnitTestBoolValue = true;

            string text = parser.Tagger(ShortText, "Test", false, out noChange, ref summary);
            //Stub, no existing stub tag. Needs all tags
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text), "page is orphan");
            Assert.IsFalse(text.Contains("Underlinked"));

            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text), "page is deadend");
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text), "page is stub");
            Assert.IsTrue(Tools.NestedTemplateRegex("Uncategorized stub").IsMatch(text), "page is uncategorised stub");
            Assert.IsTrue(text.Contains(UncatStub), "page has already been tagged as uncategorised stub");

            text = parser.Tagger(ShortTextWithLongComment, "Test", false, out noChange, ref summary);
            //Stub, no existing stub tag. Needs all tags
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text), "page is orphan");
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text));
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text));
            Assert.IsTrue(Tools.NestedTemplateRegex("Uncategorized stub").IsMatch(text));
            Assert.IsTrue(text.Contains(UncatStub));

            text = parser.Tagger(ShortTextWithLongComment, "List of Tests", false, out noChange, ref summary);
            //Stub, no existing stub tag but with "List of..." in its title. Needs all tags but stub
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text), "page is orphan");
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text));
			Assert.IsFalse(WikiRegexes.Stub.IsMatch(text));
            Assert.IsFalse(Tools.NestedTemplateRegex("Uncategorized stub").IsMatch(text));
            Assert.IsFalse(text.Contains(UncatStub));

            text = parser.Tagger(ShortTextWithLongComment, "Meanings of minor planet names: 3001–4000", false, out noChange, ref summary);
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text), "page is orphan");
            Assert.IsFalse(WikiRegexes.DeadEnd.IsMatch(text), "page is not dead end");
			Assert.IsFalse(WikiRegexes.Stub.IsMatch(text), "page is not stub");
 
            text = parser.Tagger(ShortText + Stub + Uncat + Wikify + Orphan + Deadend, "Test", false, out noChange, ref summary);
            //Tagged article, dupe tags shouldn't be added
            Assert.AreEqual(1, Tools.RegexMatchCount(Regex.Escape(Stub), text));
            Assert.AreEqual(1, Tools.RegexMatchCount(Regex.Escape(UncatStub), text));
            Assert.AreEqual(1, Tools.RegexMatchCount(Regex.Escape(Wikify), text));
            Assert.AreEqual(1, Tools.RegexMatchCount(Regex.Escape(Orphan), text));
            Assert.AreEqual(1, Tools.RegexMatchCount(Regex.Escape(Deadend), text));

            text = parser.Tagger(ShortText + Stub, "Test", false, out noChange, ref summary);
            //Stub, existing stub tag
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text), "page is orphan");
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text));
            Assert.IsTrue(text.Contains(UncatStub));
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text));

            Assert.IsFalse(text.Contains(Uncat));

            Assert.AreEqual(1, Tools.RegexMatchCount(Regex.Escape(Stub), text));

            text = parser.Tagger(ShortText + ShortText, "Test", false, out noChange, ref summary);
            //Not a stub
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text), "page is orphan");
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(text));

            Assert.IsFalse(text.Contains(UncatStub));
            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text));

            text = parser.Tagger(ShortText, "Main Page", false, out noChange, ref summary);
            //Main Page should not be tagged
            Assert.IsTrue(ShortText == text, "nothing changed");

            // rename {{improve categories}} if uncategorized
            text = parser.Tagger(ShortText + ShortText + @"{{improve categories}}", "Test", false, out noChange, ref summary);
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(text));
            Assert.IsFalse(Tools.NestedTemplateRegex("improve categories").IsMatch(text));

            // with categories and no links, Deadend not removed
            Globals.UnitTestIntValue = 3;
            text = parser.Tagger(Deadend + ShortText, "Test", false, out noChange, ref summary);
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text), "page is orphan");
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text));
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text));

            Globals.UnitTestIntValue = 5;

            text = parser.Tagger(ShortText, "Test", false, out noChange, ref summary);
            //Categorised Stub
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text), "page is orphan");
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text));
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text));

            Assert.IsFalse(text.Contains(UncatStub));
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text));

            text = parser.Tagger(ShortText + ShortText, "Test", false, out noChange, ref summary);
            //Categorised Page
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text), "page is orphan");
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text));

            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text));
            Assert.IsFalse(text.Contains(UncatStub));
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text));

            Globals.UnitTestBoolValue = false;

            text = parser.Tagger(ShortText, "Test", false, out noChange, ref summary);
            //Non orphan categorised stub
            Assert.IsFalse(WikiRegexes.Wikify.IsMatch(text));
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text));
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text));

            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));
            Assert.IsFalse(text.Contains(UncatStub));
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text));

            text = parser.Tagger(ShortText + ShortText, "Test", false, out noChange, ref summary);
            //Non orphan categorised page
            Assert.IsFalse(WikiRegexes.Wikify.IsMatch(text));
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text));

            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text));
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));
            Assert.IsFalse(text.Contains(UncatStub));
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text));

            text = parser.Tagger(ShortText.Replace("consectetur", "[[consectetur]]"), "Test", false, out noChange, ref summary);
            //Non Deadend stub
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text));

            Assert.IsFalse(WikiRegexes.Wikify.IsMatch(text));
            Assert.IsFalse(WikiRegexes.DeadEnd.IsMatch(text));
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));
            Assert.IsFalse(text.Contains(UncatStub));
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text));

            text = parser.Tagger(Regex.Replace(ShortText, @"(\w+)", "[[$1]]"), "Test", false, out noChange, ref summary);
            //very wikified stub
            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text));
            Assert.IsFalse(WikiRegexes.Wikify.IsMatch(text));
            Assert.IsFalse(WikiRegexes.DeadEnd.IsMatch(text));
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));
            Assert.IsFalse(text.Contains(UncatStub));
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text));

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_10#Autotagging_and_Articleissues
            // do not add orphan where article already has orphan tag within {{Multiple issues}}
            Globals.UnitTestBoolValue = true;
            text = parser.Tagger(@"{{Multiple issues|orphan=May 2008|cleanup=May 2008|story=May 2008}}\r\n" + ShortText, "Test", false, out noChange, ref summary);
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text));

            text = parser.Tagger(@"{{Article issues|orphan={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|deadend={{subst:CURRENTMONTH}} {{subst:CURRENTYEAR}}|wikify=May 2008}}\r\n" + ShortText, "Test", false, out noChange, ref summary);
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text), "page is orphan");

            Globals.UnitTestBoolValue = false;

            // disambigs are not stubs
            text = parser.Tagger(ShortText + @" {{hndis|Bar, Foo}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text));

            // SIAs are not stubs
            text = parser.Tagger(ShortText + @" {{surname|Smith, John}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text));

            // Pages with Events by year for decade are not stubs
            text = parser.Tagger(ShortText + @" {{Events by year for decade|79}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text));
            text = parser.Tagger(ShortText + @" {{Events by year for decade BC|79}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text));

            // {{improve categories}} --> {{uncat}} if no cats
            Globals.UnitTestIntValue = 0;
            text = parser.Tagger(@"{{improve categories}}\r\n" + ShortText, "Test", false, out noChange, ref summary);
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(text));

            // {{improve categories}} --> {{uncat stub}} if no cats and stub
            Globals.UnitTestIntValue = 0;
            text = parser.Tagger(@"{{improve categories}}  {{foo-stub}}\r\n" + ShortText, "Test", false, out noChange, ref summary);
            Assert.IsTrue(text.Contains(@"{{uncategorized stub"));
            Assert.IsFalse(text.Contains(@"{{improve categories"));

            // Do not add underlinked if page is small with a single wikilink
            Globals.UnitTestIntValue = 0;
            text = parser.Tagger(ShortTextWithSingleLink, "Test", false, out noChange, ref summary);
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(text),"uncat");
            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text),"Stub");
            Assert.IsFalse(WikiRegexes.Wikify.IsMatch(text),"underlinked");
            Assert.IsFalse(WikiRegexes.DeadEnd.IsMatch(text),"deadend");
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text),"orphan");
            Assert.IsFalse(text.Contains(UncatStub));

            // when no cats from existing page by API call but genfixes adds people categories, don't tag uncat
            Globals.UnitTestIntValue = 1;
            text = parser.Tagger(ShortText + ShortText + @"[[Category:Living people]]", "Test", false, out noChange, ref summary);
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text));
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text));
        }

        [Test]
        public void AddWikify()
        {
            Globals.UnitTestIntValue = 0;
            Globals.UnitTestBoolValue = true;
            string t1 = @"'''Ae0bgz2CNta0Qib4dK3''' VcnyafUE0bqIUdr5e 9zggyDHmIye [[PoPUJrqLG 3a8vnqpgy]].<ref>EdOkQE5gA 7u9P9ZZtd dFw0g9Fsf 99924876231</ref>

==References==
{{Reflist}}

{{Persondata <!-- Metadata: see [[Wikipedia:Persondata]]. -->
| NAME              = 0mqgyd9wZ ow3yEcrk6
| ALTERNATIVE NAMES =
| SHORT DESCRIPTION =
| DATE OF BIRTH     =
| PLACE OF BIRTH    =
| DATE OF DEATH     =
| PLACE OF DEATH    =
}}
{{DEFAULTSORT:bJqnzFm7e, opFhLKq7z}}
[[Category:Albanian people]]
[[Category:Albanian Declaration of Independence]]


{{Albania-bio-stub}}";

            string text = parser.Tagger(t1, "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Wikify.IsMatch(text));
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text));
            
            t1 = @"{{infobox something
|param1=text long text
|param2=text long text
|param3=text long text
|param4=text long text
|param5=text long text
}}
'''Ae0bgz2CNta0Qib4dK3''' VcnyafUE0bqIUdr5e 9zggyDHmIye [[PoPUJrqLG 3a8vnqpgy]].<ref>EdOkQE5gA 7u9P9ZZtd dFw0g9Fsf 99924876231</ref>

==References==
{{Reflist}}

{{DEFAULTSORT:bJqnzFm7e, opFhLKq7z}}
[[Category:Albanian people]]
[[Category:Albanian Declaration of Independence]]


{{Albania-bio-stub}}";

            text = parser.Tagger(t1, "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Wikify.IsMatch(text));
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text));
        }

        [Test]
        public void AddUnderlinked()
        {
            Globals.UnitTestIntValue = 1;
            Globals.UnitTestBoolValue = false;
            string t1 = @"{{temp}}'''Ae0bgz2CNta0Qib4dK3''' VcnyafUE0bqIUdr5e 9zggyDHmIye [[PoPUJrqLG 3a8vnqpgy]].<ref>EdOkQE5gA 7u9P9ZZtd dFw0g9Fsf 99924876231</ref>

==References==
{{Reflist}}

{{Persondata
| NAME              = 0mqgyd9wZ ow3yEcrk6
| ALTERNATIVE NAMES =
| SHORT DESCRIPTION =
| DATE OF BIRTH     =
| PLACE OF BIRTH    =
| DATE OF DEATH     =
| PLACE OF DEATH    =
}}
{{DEFAULTSORT:bJqnzFm7e, opFhLKq7z}}";

            string text = parser.Tagger(t1 + LongText, "Test", false, out noChange, ref summary);
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(text));
            Assert.IsTrue(text.StartsWith(@"{{Underlinked|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}
{{temp}}"), "no excess newlines on tag addition");

            t1 = @"{{dead end}}{{temp}}'''Ae0bgz2CNta0Qib4dK3''' VcnyafUE0bqIUdr5e 9zggyDHmIye [[PoPUJrqLG 3a8vnqpgy]].<ref>EdOkQE5gA 7u9P9ZZtd dFw0g9Fsf 99924876231</ref>

==References==
{{Reflist}}

{{Persondata
| NAME              = 0mqgyd9wZ ow3yEcrk6
| ALTERNATIVE NAMES =
| SHORT DESCRIPTION =
| DATE OF BIRTH     =
| PLACE OF BIRTH    =
| DATE OF DEATH     =
| PLACE OF DEATH    =
}}
{{DEFAULTSORT:bJqnzFm7e, opFhLKq7z}}";

            text = parser.Tagger(t1 + LongText, "Test", false, out noChange, ref summary);
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(text));
            Assert.IsTrue(text.StartsWith(@"{{Underlinked|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}
{{temp}}"), "no excess newlines on tag change");

            text = parser.Tagger(@"{{shipindex}}" + LongText, "Test", false, out noChange, ref summary);
            Assert.IsFalse(text.Contains(@"{{Underlinked|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"), "SIA pages not tagged as underlinked");       
        }

        [Test]
        public void AddSv()
        {
            Globals.UnitTestIntValue = 0;
            Globals.UnitTestBoolValue = true;

#if DEBUG
            Variables.SetProjectLangCode("sv");
            WikiRegexes.MakeLangSpecificRegexes();
            string text = parser.Tagger(ShortText, "Test", false, out noChange, ref summary);
            //Stub, no existing stub tag. Sv wiki doens't have dead end nor orphan
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text), "orphan");
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text), "stub");
            Assert.IsFalse(WikiRegexes.DeadEnd.IsMatch(text), "dead end");
            Assert.IsFalse(WikiRegexes.Wikify.IsMatch(text), "wikify");
            Assert.IsFalse(text.Contains("{{Wikify|" + WikiRegexes.DateYearMonthParameter + @"}}

"));
            Assert.IsTrue(Tools.NestedTemplateRegex("Okategoriserad").IsMatch(text));
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text));
            Variables.SetProjectLangCode("en");
            WikiRegexes.MakeLangSpecificRegexes();
#endif
        }

        [Test]
        public void AddDeadEndAr()
        {
            Globals.UnitTestIntValue = 0;
            Globals.UnitTestBoolValue = true;

            #if DEBUG
            Variables.SetProjectLangCode("ar");
            WikiRegexes.MakeLangSpecificRegexes();

            string text = parser.Tagger(ShortText, "Test", false, out noChange, ref summary);
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text), "page is deadend");
            Assert.IsTrue(text.StartsWith("{{نهاية مسدودة|تاريخ={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}\r\n{{يتيمة|تاريخ={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}\r\n\r\nLorem ipsum"), 
                          "no blank line between dead end and orphan");

            // ويكي = wikify
            text = parser.Tagger(ShortText + "{{ويكي}}", "Test", false, out noChange, ref summary);
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text), "page is deadend");
            Assert.IsFalse(WikiRegexes.Wikify.IsMatch(text), "wikify should not be present which dead end added");
            Assert.IsFalse(text.Contains("ويكي"), "wikify removed when dead end");
            Assert.IsTrue(summary.Contains("ويكي"));

            Variables.SetProjectLangCode("en");
            WikiRegexes.MakeLangSpecificRegexes();
            #endif
        }

        [Test]
        public void AddAr()
        {
            Globals.UnitTestIntValue = 0;
            Globals.UnitTestBoolValue = true;

            #if DEBUG
            Variables.SetProjectLangCode("ar");
            WikiRegexes.MakeLangSpecificRegexes();

            string text = parser.Tagger(ShortText, "Test", false, out noChange, ref summary);
            //Stub, no existing stub tag. Needs all tags
            //FIXME: In fact the first group of tests should be false for wikify and true for deadend since the text
            // has no wikilinks
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text),"orphan");
            Assert.IsTrue(text.Contains("{{يتيمة|" + WikiRegexes.DateYearMonthParameter + @"}}"),"orphan");
            //Assert.IsFalse(WikiRegexes.Wikify.IsMatch(text),"wikify");
            //Assert.IsFalse(text.Contains("{{ويكي|" + WikiRegexes.DateYearMonthParameter + @"}}"),"wikify");
            //Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text));
            Assert.IsTrue(Tools.NestedTemplateRegex("بذرة غير مصنفة").IsMatch(text),"Uncategorized stub");
            Assert.IsFalse(text.Contains("Uncategorized"), "no en-wiki uncat tags");
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text),"stub");

            text = parser.Tagger(ShortText+ @"{{disambig}}", "Test", false, out noChange, ref summary);
            //Don't add stub/orphan to disambig pages. Still add wikify
            Assert.IsFalse(text.Contains("{{يتيمة|" + WikiRegexes.DateYearMonthParameter + @"}}"),"orphan");
            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text),"stub");
            //Assert.IsTrue(text.Contains("{{ويكي|" + WikiRegexes.DateYearMonthParameter + @"}}"),"wikify");

            text = parser.Tagger(ShortText+ @"{{توضيح}}", "Test", false, out noChange, ref summary);
            //Don't add stub/orphan to disambig pages. Still add wikify
            Assert.IsFalse(text.Contains("{{يتيمة|" + WikiRegexes.DateYearMonthParameter + @"}}"),"orphan");
            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text),"stub");
            Assert.IsFalse(WikiRegexes.Wikify.IsMatch(text),"wikify");
            Assert.IsFalse(text.Contains("{{ويكي|" + WikiRegexes.DateYearMonthParameter + @"}}"),"wikify");
            //Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text),"deadend");
            
            text = parser.Tagger(ShortText.Replace("consectetur", "[[consectetur]]"), "Test", false, out noChange, ref summary);
            //Non Deadend stub
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text));

            Assert.IsFalse(WikiRegexes.Wikify.IsMatch(text),"wikify");
            Assert.IsFalse(WikiRegexes.DeadEnd.IsMatch(text),"deadend");
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text),"orphan");
            Assert.IsFalse(text.Contains(UncatStub),"english uncatstub");
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(text),"uncat");

            text = parser.Tagger(Regex.Replace(ShortText, @"(\w+)", "[[$1]]"), "Test", false, out noChange, ref summary);
            //very wikified stub
            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text),"stub");
            Assert.IsFalse(WikiRegexes.Wikify.IsMatch(text),"wikify");
            Assert.IsFalse(WikiRegexes.DeadEnd.IsMatch(text),"deadend");
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text),"orphan");
            Assert.IsFalse(text.Contains(UncatStub),"english uncatstub");
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(text),"uncat");

            // dead end plus orphan but one wikilink: dead end --> orphan
            text = parser.Tagger(@"{{نهاية مسدودة|تاريخ=نوفمبر 2013}}
{{يتيمة|تاريخ=نوفمبر 2013}}

[[link]]" + LongText,"Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.DeadEnd.IsMatch(text),"deadend");
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text),"orphan");
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(text),"wikify");
            Assert.IsFalse(text.Contains("}}" + "\r\n\r\n" + "{{")); // no blank line between wikify & orphan

            Globals.UnitTestBoolValue = true;
            text = parser.Tagger(ShortText+ShortText+ShortText+ShortText, "Test", false, out noChange, ref summary);
            Assert.IsFalse(text.Contains("Uncategorized"), "no en-wiki uncat tags");

            Variables.SetProjectLangCode("en");
            WikiRegexes.MakeLangSpecificRegexes();
            #endif
        }

        [Test]
        public void AddArz()
        {
            Globals.UnitTestIntValue = 0;
            Globals.UnitTestBoolValue = true;

            #if DEBUG
            Variables.SetProjectLangCode("arz");
            WikiRegexes.MakeLangSpecificRegexes();

            string text = parser.Tagger(ShortText, "Test", false, out noChange, ref summary);
            //Stub, no existing stub tag. Needs all tags
            Assert.IsTrue(text.Contains("{{يتيمه|" + WikiRegexes.DateYearMonthParameter + @"}}"),"orphan");
            //Assert.IsFalse(text.Contains("{{ويكى|" + WikiRegexes.DateYearMonthParameter + @"}}"),"wikify");
            //Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text));
            Assert.IsTrue(Tools.NestedTemplateRegex("تقاوى مش متصنفه").IsMatch(text),"Uncategorized stub");
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text),"stub tag added");

            text = parser.Tagger(ShortText+ @"{{disambig}}", "Test", false, out noChange, ref summary);
            //Don't add stub/orphan to disambig pages. Still add wikify
            Assert.IsFalse(text.Contains("{{يتيمه|" + WikiRegexes.DateYearMonthParameter + @"}}"),"orphan not added");
            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text),"stub tag not added");
            Assert.IsFalse(text.Contains("{{ويكى|" + WikiRegexes.DateYearMonthParameter + @"}}"),"wikify not added");

            text = parser.Tagger(ShortText+ @"{{توضيح}}", "Test", false, out noChange, ref summary);
            //Don't add stub/orphan to disambig pages. Still add wikify
            Assert.IsFalse(text.Contains("{{يتيمه|" + WikiRegexes.DateYearMonthParameter + @"}}"),"orphan not added");
            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text),"stub not added");
            Assert.IsFalse(text.Contains("{{ويكى|" + WikiRegexes.DateYearMonthParameter + @"}}"),"wikify not added");

            Variables.SetProjectLangCode("en");
            WikiRegexes.MakeLangSpecificRegexes();
            #endif
        }

        [Test]
        public void AddOrphan()
        {
            Globals.UnitTestBoolValue = true;
            // orphan not added if disambig found – skips since a template present
            string text = parser.Tagger(ShortText + @"{{disambig}}", "Test", false, out noChange, ref summary);
            //Stub, no existing stub tag. Needs all tags
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));

            text = parser.Tagger(ShortText + @"{{Widirect}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text), "pages using {{wi}} not tagged as orphan");

            text = parser.Tagger(ShortText + @"{{Wi}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text), "pages using {{wi}} not tagged as orphan");

            text = parser.Tagger(ShortText + @"{{soft redirect}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text), "soft redirect not tagged as orphan");

            text = parser.Tagger(ShortText, "Test", false, out noChange, ref summary);
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text), "page is orphan");

            text = parser.Tagger(@"{{Infobox foo bar|great=yes}}" + ShortText, "Test", false, out noChange, ref summary);
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text), "page is orphan");

            text = parser.Tagger(@"{{multiple issues|foo={{subst:CURRENTMONTH}} |orphan={{subst:FOOBAR}} }}" + ShortText, "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text),"tags with subst");

            Globals.UnitTestBoolValue = false;

            text = parser.Tagger(ShortText, "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));
        }

        [Test]
        public void AddLinklessRu()
        {
            Globals.UnitTestIntValue = 0;
            Globals.UnitTestBoolValue = true;

            #if DEBUG
            Variables.SetProjectLangCode("ru");

            const string textIn = @"Foo bar {{rq|wikify|style}} here", textIn2 = @"Foo bar";

            // standard case -- add pararmeter
            string text = parser.Tagger(textIn, "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));
            Assert.IsTrue(text.Contains(@"{{rq|wikify|style|linkless}}"));

            // parameter already there
            text = parser.Tagger(textIn.Replace(@"}}", "|linkless}}"), "Test", false, out noChange, ref summary);
            Assert.IsTrue(text.Contains(@"{{rq|wikify|style|linkless}}"));

            // no {{Rq}} to update
            text = parser.Tagger(textIn2, "Test", false, out noChange, ref summary);
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text));
            Assert.IsFalse(text.Contains(@"|linkless}}"));

            // multiple {{Rq}} -- don't change
            text = parser.Tagger(textIn + textIn, "Test", false, out noChange, ref summary);
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text));
            Assert.IsFalse(text.Contains(@"|linkless}}"));

            Variables.SetProjectLangCode("en");

            // non-ru operation
            text = parser.Tagger(textIn, "Test", false, out noChange, ref summary);
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text), "page is orphan");
            Assert.IsFalse(text.Contains(@"{{rq|wikify|style|linkless}}"));
#endif
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

        private const string ShortTextWithLongComment =
            @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Curabitur sit amet tortor nec neque faucibus pharetra. Fusce lorem arcu, tempus et, imperdiet a, commodo a, pede. Nulla sit amet turpis gravida elit dictum cursus. Praesent tincidunt velit eu urna.

<!--Curabitur luctus mollis massa. Nullam consectetur mollis lacus. Suspendisse turpis. Fusce velit. Morbi egestas dui. Donec commodo ornare lorem. Vestibulum sodales. Curabitur egestas libero ut metus. Sed eget orci a ligula consectetur vestibulum. Cras sapien.

Sed libero. Ut volutpat massa. Donec nulla pede, porttitor eu, sodales et, consectetur nec, quam. Pellentesque vestibulum hendrerit est. Nulla facilisi. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Duis et nibh eu lacus iaculis pretium. Fusce sed turpis. In cursus. Etiam interdum augue. Morbi commodo auctor ligula. In imperdiet, neque nec hendrerit consequat, lacus purus tristique turpis, eu hendrerit ipsum ligula at libero. Duis varius nunc vel tortor. Praesent tempor. Nunc non pede at velit congue feugiat. Curabitur gravida, nisl quis mattis porttitor, purus nulla viverra dui, non suscipit augue nunc ac libero. Donec lacinia est non augue.

Nulla quam dui, tristique id, condimentum sed, sodales in, ante. Vestibulum vitae diam. Integer placerat ante non orci. Nulla gravida. Integer magna enim, iaculis ut, ornare dignissim, ultrices a, urna. Donec urna. Fusce fringilla, pede vitae pulvinar ullamcorper, est nisi eleifend ipsum, ac adipiscing odio massa vehicula neque. Sed blandit est. Morbi faucibus, nisl vel commodo vulputate, mi ipsum tincidunt sem, id ornare orci orci et velit. Morbi commodo sollicitudin ligula. Pellentesque vitae urna. Duis massa arcu, accumsan id, euismod eu, tincidunt et, odio. Phasellus purus leo, rhoncus sed, condimentum nec, vestibulum vel, lacus. In egestas, lectus vitae lacinia tristique, elit magna consequat risus, id sodales metus nulla ac pede. Suspendisse potenti.

Fusce massa. Nullam lacinia purus nec ipsum. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Suspendisse potenti. Proin augue. Donec mi magna, interdum a, elementum quis, bibendum sit amet, felis. Donec vel libero eget magna hendrerit ultrices. Suspendisse potenti. Sed scelerisque lacinia nisi. Quisque elementum, nunc nec luctus iaculis, ante quam aliquet orci, et ullamcorper dui ipsum at mi. Vestibulum a dolor id tortor posuere elementum. Sed mauris nisl, ultrices a, malesuada non, convallis ac, velit. Sed aliquam elit id metus. Donec malesuada, lorem ut pharetra auctor, mi risus viverra enim, vitae pulvinar urna metus at lorem. Vivamus id lorem. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Nulla facilisi. Ut vel odio. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Pellentesque lobortis sem.

Proin in odio. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Vivamus bibendum arcu nec risus. Nulla iaculis ligula in purus. Etiam vulputate nibh sit amet lectus. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Suspendisse potenti. Suspendisse eleifend. Donec blandit nibh hendrerit turpis. Integer accumsan posuere odio. Ut commodo augue malesuada risus. Curabitur augue. Praesent volutpat nunc a diam. Nulla lobortis interdum dolor. Nunc imperdiet, ipsum ac tempor iaculis, nunc.-->
";

        private const string ShortTextWithSingleLink =
            @"'''KH Fakhruddin''' is regarded as a [[National Hero of Indonesia]].<ref>{{Cite book|last=Mirnawati|title=Kumpulan Pahlawan Indonesia Terlengkap|trans_title=Most Complete Collection of Indonesian Heroes|language=Indonesian|date=2012|location=Jakarta|publisher=CIF|isbn=978-979-788-343-0}}</ref>

==References==
{{Reflist}}

{{National Heroes of Indonesia}}

<!-- comments do not affect page length and therefore do not affect page tagging as well -->

{{Persondata
| NAME              = Fakhruddin, KH
| ALTERNATIVE NAMES =
| SHORT DESCRIPTION = National hero of Indonesia
| DATE OF BIRTH     = 1804
| PLACE OF BIRTH    = Indonesia
| DATE OF DEATH     = 1904
| PLACE OF DEATH    = Earth
}}";

        [Test]
        public void Remove()
        {
            string text = parser.Tagger(ShortText + Stub, "Test", false, out noChange, ref summary);
            //Stub, tag not removed
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text));

            text = parser.Tagger(LongText + Stub, "Test", false, out noChange, ref summary);
            //stub tag removed
            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text));

            text = parser.Tagger("{{wikify}}" + Regex.Replace(LongText, @"(\w+)", "[[$1]]"), "Test", false, out noChange, ref summary);
            string text1 = parser.Tagger(Regex.Replace(LongText, @"(\w+)", "[[$1]]"), "Test", false, out noChange, ref summary);
            string text2 = parser.Tagger(Regex.Replace(LongText, @"(\w+)", "[[$1]]" + Stub), "Test", false, out noChange, ref summary);
            //wikify tag removed
            Assert.IsFalse(WikiRegexes.Wikify.IsMatch(text));
            Assert.AreEqual(text,text1,"check whether wikify tag is removed properly");
            Assert.AreEqual(text1,text2,"check whether stub tag is removed properly");

            text = parser.Tagger("{{wikify|reason=something}}" + Regex.Replace(LongText, @"(\w+)", "[[$1]]"), "Test", false, out noChange, ref summary);
            //wikify tag with reason NOT removed
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(text));

            text = parser.Tagger("{{multiple issues|COI=May 2010 | POV = May 2010 |wikify=June 2010}}" + Regex.Replace(LongText, @"(\w+)", "[[$1]]"), "Test", false, out noChange, ref summary);
            //wikify tag removed
            Assert.IsFalse(WikiRegexes.Wikify.IsMatch(text));

            text = parser.Tagger("A ==x== {{multiple issues|COI=May 2010 | POV = May 2010 |wikify=June 2010|section=y}} ==B==" + Regex.Replace(LongText, @"(\w+)", "[[$1]]"), "Test", false, out noChange, ref summary);
            //wikify tag removed
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(text));

            Globals.UnitTestIntValue = 4;
            text = parser.Tagger("{{uncategorised}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text));

            text = parser.Tagger("{{uncategorised|date=January 2009}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text));

            text = parser.Tagger("{{uncategorised|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text));

            text = parser.Tagger("{{uncategorised|date=January 2009}} {{foo}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text), "Uncat removed even if other template present");
        }

        [Test]
        public void RemoveDisambig()
        {
            Globals.UnitTestBoolValue = false;
            string text = parser.Tagger("{{disambig|date=September 2013}}{{disambig cleanup|date=September 2013}}", "Test", false, out noChange, ref summary);
            Assert.IsTrue(WikiRegexes.DisambigsCleanup.IsMatch(text), "keeps disambig cleanup");
            Assert.IsFalse(WikiRegexes.DisambigsGeneral.IsMatch(text), "removes disambig");

            text = parser.Tagger("{{disambiguation|date=September 2013}}{{disambig cleanup|date=September 2013}}", "Test", false, out noChange, ref summary);
            Assert.IsTrue(WikiRegexes.DisambigsCleanup.IsMatch(text), "keeps disambig cleanup");
            Assert.IsFalse(WikiRegexes.DisambigsGeneral.IsMatch(text), "removes disambig");

            text = parser.Tagger("{{disambig cleanup|date=September 2013}}{{disambig|date=September 2013}}", "Test", false, out noChange, ref summary);
            Assert.IsTrue(WikiRegexes.DisambigsCleanup.IsMatch(text), "keeps disambig cleanup");
            Assert.IsFalse(WikiRegexes.DisambigsGeneral.IsMatch(text), "removes disambig");

            Globals.UnitTestBoolValue = true;
        }

        [Test]
        public void RemoveOrphan()
        {
            Globals.UnitTestBoolValue = false;

            string text = parser.Tagger("{{orphan}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));
            
            //Test if orphan tag is removed properly. Use wikilink and List of to prevent tagging for wikify, deadend and stub
            text = parser.Tagger("{{orphan}}[[foo]]", "List of Tests", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));
            Assert.AreEqual(text,"[[foo]]");

            // Don't remove when few parameter set
            text = parser.Tagger("{{orphan|few=a}}", "Test", false, out noChange, ref summary);
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text));

            // multiple issues removed if tagger removes only tag in it
            text = parser.Tagger(@"{{multiple issues|
{{orphan}}
}}[[foo]]", "List of Tests", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));
            Assert.IsFalse(WikiRegexes.MultipleIssues.IsMatch(text));
            Assert.AreEqual(text,"[[foo]]");

            Globals.UnitTestBoolValue = true;

            text = parser.Tagger("{{orphan}}", "Test", false, out noChange, ref summary);
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text), "page is orphan");
        }

        [Test]
        public void RemoveOrphanAr()
        {
#if DEBUG
            Variables.SetProjectLangCode("ar");
            WikiRegexes.MakeLangSpecificRegexes();
            Globals.UnitTestBoolValue = false;

            string text = parser.Tagger("{{orphan}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));
            
            text = parser.Tagger("{{يتيمة}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));

            //Test if orphan tag is removed properly. Use wikilink and disambig to prevent tagging for wikify, deadend and stub
            text = parser.Tagger("{{orphan}}[[foo]]{{disambig}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));
            Assert.AreEqual(text,"{{orphan}}[[foo]]{{disambig}}");
            
            text = parser.Tagger("{{يتيمة}}[[foo]]{{disambig}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));
            Assert.AreEqual(text,"[[foo]]{{disambig}}");


            Globals.UnitTestBoolValue = true;
            Variables.SetProjectLangCode("en");
            WikiRegexes.MakeLangSpecificRegexes();
#endif
        }


        [Test]
        public void RemoveOrphanArz()
        {
#if DEBUG
            Variables.SetProjectLangCode("arz");
            WikiRegexes.MakeLangSpecificRegexes();
            Globals.UnitTestBoolValue = false;

            string text = parser.Tagger("{{orphan}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));
            
            text = parser.Tagger("{{يتيمه}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));

            //Test if orphan tag is removed properly. Use wikilink and disambig to prevent tagging for wikify, deadend and stub
            text = parser.Tagger("{{orphan}}[[foo]]{{disambig}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));
            Assert.AreEqual(text,"{{orphan}}[[foo]]{{disambig}}");
            
            text = parser.Tagger("{{يتيمه}}[[foo]]{{disambig}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));
            Assert.AreEqual(text,"[[foo]]{{disambig}}");


            Globals.UnitTestBoolValue = true;
            Variables.SetProjectLangCode("en");
            WikiRegexes.MakeLangSpecificRegexes();
#endif
        }

        [Test]
        public void RemoveOrphanSv()
        {
#if DEBUG
            Variables.SetProjectLangCode("sv");
            WikiRegexes.MakeLangSpecificRegexes();
            Globals.UnitTestBoolValue = false;

            string text = parser.Tagger("{{orphan}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));
            
            text = parser.Tagger("{{Föräldralös}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));

            //Test if orphan tag is removed properly. Use wikilink and disambig to prevent tagging for wikify, deadend and stub
            text = parser.Tagger("{{orphan}}[[foo]]{{disambig}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));
            Assert.AreEqual(text,"{{orphan}}[[foo]]{{disambig}}");
            
            text = parser.Tagger("{{Föräldralös}}[[foo]]{{disambig}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));
            Assert.AreEqual(text,"[[foo]]{{disambig}}");


            Globals.UnitTestBoolValue = true;
            Variables.SetProjectLangCode("en");
            WikiRegexes.MakeLangSpecificRegexes();
#endif
        }
        
        [Test]
        public void RemoveAr()
        {
#if DEBUG
            string text ="";
            Variables.SetProjectLangCode("ar");
            WikiRegexes.MakeLangSpecificRegexes();
            
            Globals.UnitTestBoolValue = true;

            text = parser.Tagger(ShortText + @"{{بذرة}}", "Test", false, out noChange, ref summary);
            //Stub, tag not removed
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text));

            text = parser.Tagger(LongText + @"{{بذرة}}", "Test", false, out noChange, ref summary);
            //stub tag removed
            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text));
            
            text = parser.Tagger("{{ويكي}}" + Regex.Replace(LongText, @"(\w+)", "[[$1]]"), "Test", false, out noChange, ref summary);
            string text1 = parser.Tagger(Regex.Replace(LongText, @"(\w+)", "[[$1]]"), "Test", false, out noChange, ref summary);
            string text2 = parser.Tagger(Regex.Replace(LongText, @"(\w+)", "[[$1]]") + @"{{بذرة}}", "Test", false, out noChange, ref summary);
            //wikify tag removed
            Assert.IsFalse(WikiRegexes.Wikify.IsMatch(text));
            //Assert.AreEqual(text,text1,"check whether wikify tag is removed properly");
            Assert.AreEqual(text1,text2,"check whether stub tag is removed properly");

            Variables.SetProjectLangCode("en");
            WikiRegexes.MakeLangSpecificRegexes();
#endif
        }
        
        [Test]
        public void RemoveArz()
        {
#if DEBUG
            string text ="";
            Variables.SetProjectLangCode("arz");
            WikiRegexes.MakeLangSpecificRegexes();
            
            Globals.UnitTestBoolValue = true;

            text = parser.Tagger(ShortText + @"{{بذرة}}", "Test", false, out noChange, ref summary);
            //Stub, tag not removed
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text));

            text = parser.Tagger(LongText + @"{{بذرة}}", "Test", false, out noChange, ref summary);
            //stub tag removed
            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text));
            
            text = parser.Tagger("{{ويكى}}" + Regex.Replace(LongText, @"(\w+)", "[[$1]]"), "Test", false, out noChange, ref summary);
            string text1 = parser.Tagger(Regex.Replace(LongText, @"(\w+)", "[[$1]]"), "Test", false, out noChange, ref summary);
            string text2 = parser.Tagger(Regex.Replace(LongText, @"(\w+)", "[[$1]]") + @"{{بذرة}}", "Test", false, out noChange, ref summary);
            //wikify tag removed
            Assert.IsFalse(WikiRegexes.Wikify.IsMatch(text));
            //Assert.AreEqual(text,text1,"check whether wikify tag is removed properly");
            Assert.AreEqual(text1,text2,"check whether stub tag is removed properly");

            Variables.SetProjectLangCode("en");
            WikiRegexes.MakeLangSpecificRegexes();
#endif
        }

        [Test]
        public void MultipleIssuesNewTags()
        {
            string Text = LongText + @"{{Multiple issues | COI=May 2010 | POV=May 2010 }}";

            Text = parser.Tagger(Text, "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Wikify.IsMatch(Text), "added tags go in existing multipleIssues");
        }

        [Test]
        public void MultipleIssuesRemovedTags()
        {
            string Text = Regex.Replace(LongText, @"(\w+)", "[[$1]]") + @"{{Multiple issues | wikify=May 2010 | POV=May 2010 }}";

            Text = parser.Tagger(Text, "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.MultipleIssues.IsMatch(Text));
            Assert.IsFalse(WikiRegexes.Wikify.IsMatch(Text));
            Assert.IsTrue(Tools.NestedTemplateRegex("POV").IsMatch(Text));
        }

        [Test]
        public void RemoveDeadEnd()
        {
            Assert.IsFalse(WikiRegexes.DeadEnd.IsMatch(parser.Tagger(@"foo {{deadend|date=May 2010}} [[a]] and [[b]] and [[b]]", "Test", false, out noChange, ref summary)));
            Assert.IsTrue(summary.Contains("removed deadend tag"));

            Globals.UnitTestBoolValue = false;
            Assert.AreEqual(@"{{multiple issues|
{{expert-subject|1=History|date=September 2012}}
{{Unreferenced|date=December 2006}}
}}

''Now'' [[a]] and [[b]] and [[b]]


{{stub}}", parser.Tagger(@"{{multiple issues|
{{dead end|date=September 2012}}
{{expert-subject|1=History|date=September 2012}}
{{Unreferenced|date=December 2006}}
}}

''Now'' [[a]] and [[b]] and [[b]]", "Test", false, out noChange, ref summary));
            
            Globals.UnitTestBoolValue = true;

            Assert.IsTrue(summary.Contains("removed deadend tag"));

            Assert.IsFalse(WikiRegexes.DeadEnd.IsMatch(parser.Tagger(@"foo {{Multiple issues|COI = May 2010 |POV = June 2010 | dead end=May 2010}} [[a]] and [[b]] and [[b]]", "Test", false, out noChange, ref summary)));

            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(parser.Tagger(@"foo==x== {{deadend|section|date=May 2010}} [[a]] and [[b]] and [[b]]", "Test", false, out noChange, ref summary)), "does not remove section tags");
            Assert.IsFalse(summary.Contains("removed deadend tag"));

            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(parser.Tagger(@"foo==x== {{deadend|date=May 2010}} {{Proposed deletion/dated|[[a]] and [[b]] and [[b]]}}", "Test", false, out noChange, ref summary)),
                          @"does not remove {{dead end}} when links are within prod template");
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(parser.Tagger(@"foo==x== {{deadend|date=May 2010}} {{Proposed deletion endorsed|[[a]] and [[b]] and [[b]]}}", "Test", false, out noChange, ref summary)),
                          @"does not remove {{dead end}} when links are within prod template, 2");
            Assert.IsFalse(summary.Contains("removed deadend tag"));
        }

        [Test]
        public void RemoveDeadEndAr()
        {
            #if DEBUG
            Variables.SetProjectLangCode("ar");
            WikiRegexes.MakeLangSpecificRegexes();

            Assert.IsFalse(WikiRegexes.DeadEnd.IsMatch(parser.Tagger(@"foo {{نهاية مسدودة|تاريخ=ديسمبر 2012}} [[a]] and [[b]] and [[b]]", "Test", false, out noChange, ref summary)));
            Assert.IsTrue(summary.Contains("نهاية مسدودة"));

            Assert.IsFalse(WikiRegexes.DeadEnd.IsMatch(parser.Tagger(@"foo {{dead end|date=December 2012}} [[a]] and [[b]] and [[b]]", "Test", false, out noChange, ref summary)));
            Assert.IsTrue(summary.Contains("نهاية مسدودة"));

            Variables.SetProjectLangCode("en");
            WikiRegexes.MakeLangSpecificRegexes();
            #endif
        }

        [Test]
        public void UpdateFactTag()
        {
            WikiRegexes.DatedTemplates.Clear();
            WikiRegexes.DatedTemplates.Add("Fact");

            //Test of updating some of the non dated tags
            string text = parser.Tagger("{{fact}}", "Test", false, out noChange, ref summary);

            Assert.IsTrue(text.Contains("{{fact|date={{subst:CURRENTMONTHNAME}}"));
            Assert.IsFalse(text.Contains("{{fact}}"));

            text = parser.Tagger("{{template:fact}}", "Test", false, out noChange, ref summary);

            Assert.IsTrue(text.Contains("{{fact|date={{subst:CURRENTMONTHNAME}}"));
            Assert.IsFalse(text.Contains("{{fact}}"));

            text = parser.Tagger("{{fact|}}", "Test", false, out noChange, ref summary);

            Assert.IsTrue(text.Contains("{{fact|date={{subst:CURRENTMONTHNAME}}"));
            Assert.IsFalse(text.Contains("{{fact}}"));
        }

        [Test]
        public void UpdateCitationNeededTag()
        {
            Globals.UnitTestBoolValue = false;
            WikiRegexes.DatedTemplates.Clear();
            WikiRegexes.DatedTemplates.Add("Citation needed");
            string text = parser.Tagger("{{citation needed}}", "Test", false, out noChange, ref summary);
            Assert.IsTrue(text.Contains(@"{{citation needed|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"));

            text = parser.Tagger("{{template:citation needed  }}", "Test", false, out noChange, ref summary);
            Assert.IsTrue(text.Contains(@"{{citation needed  |date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"));

            text = parser.Tagger("{{ citation needed}}", "Test", false, out noChange, ref summary);
            Assert.IsTrue(text.Contains(@"{{ citation needed|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"));

            text = parser.Tagger("{{Citation needed}}", "Test", false, out noChange, ref summary);
            Assert.IsTrue(text.Contains(@"{{Citation needed|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"));

            text = parser.Tagger("{{citation needed|reason=something}}", "Test", false, out noChange, ref summary);
            Assert.IsTrue(text.Contains(@"{{citation needed|reason=something|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"));

            // no change if already dated
            const string correctcn = @"{{citation needed|reason=something|date=May 2009}}";
            text = parser.Tagger(correctcn, "Test", false, out noChange, ref summary);
            Assert.IsTrue(text.Contains(correctcn));

            const string commentText = "<!--{{citation needed}}-->";

            text = parser.Tagger(commentText, "Test", false, out noChange, ref summary);
            Assert.IsTrue(text.Contains(commentText), "tag not dated when commented out");

            text = parser.Tagger(@"{{Citation needed|Date=May 2009}}", "Test", false, out noChange, ref summary);
            Assert.IsTrue(text.Contains(@"{{Citation needed|date=May 2009}}"), "Date -> date");

            text = parser.Tagger(@"{{Citation needed|date=May 2009}}", "Test", false, out noChange, ref summary);
            Assert.IsTrue(text.Contains(@"{{Citation needed|date=May 2009}}"), "if tag already dated, no change");
        }

        [Test]
        public void UpdateWikifyTag()
        {
            WikiRegexes.DatedTemplates.Clear();
            WikiRegexes.DatedTemplates.Add("Wikify");
            WikiRegexes.DatedTemplates.Add("Wikify section");
            string correct = @"{{wikify|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}";
            string text = parser.Tagger("{{wikify}}", "Test", false, out noChange, ref summary);
            Assert.IsTrue(text.Contains(correct));
            Assert.IsFalse(text.Contains("{{wikify}}"));

            text = parser.Tagger("{{template:wikify  }}", "Test", false, out noChange, ref summary);
            Assert.IsTrue(text.Contains(@"|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"));

            text = parser.Tagger("{{wikify section}}", "Test", false, out noChange, ref summary);
            Assert.IsTrue(text.Contains(@"{{wikify section|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"));

            text = parser.Tagger("{{Wikify section}}", "Test", false, out noChange, ref summary);
            Assert.IsTrue(text.Contains(@"{{Wikify section|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"));
        }

        private static readonly System.Globalization.CultureInfo BritishEnglish = new System.Globalization.CultureInfo("en-GB");

        [Test]
        public void TagUpdaterAddDate()
        {
            WikiRegexes.DatedTemplates.Clear();
            WikiRegexes.DatedTemplates.Add("Wikify");

            string correct = @"{{wikify|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}";

            Assert.AreEqual(correct, Parsers.TagUpdater(@"{{wikify}}"), "tags undated tag");
            Assert.AreEqual(correct, Parsers.TagUpdater(@"{{wikify|date=}}"), "tags undated tag");
            Assert.AreEqual(correct, Parsers.TagUpdater(@"{{wikify|dates=}}"), "tags undated tag");
            Assert.AreEqual(correct, Parsers.TagUpdater(@"{{wikify|date}}"), "tags undated tag");
            Assert.AreEqual(correct, Parsers.TagUpdater(@"{{wikify|Date=}}"), "tags undated tag");
            Assert.AreEqual(correct, Parsers.TagUpdater(@"{{Template:wikify}}"), "tags undated tag, removes template namespace");
            Assert.AreEqual(correct, Parsers.TagUpdater(@"{{template:wikify}}"), "tags undated tag, removes template namespace");
            Assert.AreEqual(correct.Replace("wik", "Wik"), Parsers.TagUpdater(@"{{Wikify}}"), "tags undated tag, keeping existing template case");

            Assert.AreEqual(@"{{wikify|date=May 2010}}", Parsers.TagUpdater(@"{{wikify|Date=May 2010}}"), "corrects Date --> date");
            Assert.AreEqual(@"{{wikify|date=May 2010}}", Parsers.TagUpdater(@"{{wikify|Date-May 2010}}"), "corrects date- --> date=");
            Assert.AreEqual(@"{{wikify|date=May 2010}}", Parsers.TagUpdater(@"{{wikify|date-=May 2010}}"), "corrects date-= --> date=");
            Assert.AreEqual(@"{{wikify|date=May 2010}}", Parsers.TagUpdater(@"{{wikify|Date-=May 2010}}"), "corrects date-= --> date=");
            Assert.AreEqual(@"{{wikify|date=May 2010}}", Parsers.TagUpdater(@"{{wikify|date-May 2010}}"), "corrects date- --> date=");
            Assert.AreEqual(@"{{wikify|date=May 2010}}", Parsers.TagUpdater(@"{{wikify|May 2010}}"), "corrects unnamed date parameter");
            Assert.AreEqual(@"{{wikify|date=May 2010 }}", Parsers.TagUpdater(@"{{wikify|May 2010 }}"), "corrects unnamed date parameter");
            Assert.AreEqual(@"{{multiple issues {{wikify|date=May 2010 }} }}", Parsers.TagUpdater(@"{{multiple issues {{wikify|May 2010 }} }}"), "corrects unnamed date parameter, nested template");

            Assert.AreEqual(@"{{wikify|section|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}", Parsers.TagUpdater(@"{{wikify|section}}"), "supports templates with additional arguments");
            Assert.AreEqual(@"{{wikify|section|other={{foo}} bar|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}", Parsers.TagUpdater(@"{{wikify|section|other={{foo}} bar}}"), "supports templates with additional arguments");
            Assert.AreEqual(@"{{wikify|section|other={{foo}}|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}", Parsers.TagUpdater(@"{{wikify|section|other={{foo}}}}"), "supports templates with additional arguments");

            Assert.AreEqual(@"{{wikify|date=May 2010}}", Parsers.TagUpdater(@"{{wikify|date=May, 2010}}"), "removes excess comma");
            Assert.AreEqual(@"{{wikify|date=May 2, 2010}}", Parsers.TagUpdater(@"{{wikify|date=May 2, 2010}}"), "American date not altered");

            WikiRegexes.DatedTemplates.Clear();
            WikiRegexes.DatedTemplates.Add("Wikify");
            Assert.AreEqual(correct, Parsers.TagUpdater(@"{{wikify}}"), "first letter casing of template rule does not matter");

            const string commentedOut = @"<!-- {{wikify}} -->";
            Assert.AreEqual(commentedOut, Parsers.TagUpdater(commentedOut), "ignores commented out tags");

            WikiRegexes.DatedTemplates.Add("Clarify");
            string nochange=@"{{Clarify|date=May 2014|reason=Use Template:Cite web or similar}}";
            Assert.AreEqual(nochange, Parsers.TagUpdater(nochange));
        }

        [Test]
        public void TagUpdaterFormatDate()
        {
            WikiRegexes.DatedTemplates.Add("Dead link");
            WikiRegexes.DatedTemplates.Add("Wikify");
            Assert.AreEqual(@"<ref>{{cite web | title=foo| url=http://www.site.com }} {{dead link|date=" + System.DateTime.UtcNow.ToString("MMMM yyyy", BritishEnglish) + @"}}</ref>", Parsers.TagUpdater(@"<ref>{{cite web | title=foo| url=http://www.site.com }} {{dead link}}</ref>"));

            Assert.AreEqual(@"{{wikify|date=May 2010}}", Parsers.TagUpdater(@"{{wikify|date=may 2010}}"), "corrects lower case month name");
            Assert.AreEqual(@"{{wikify|date=May 2010}}", Parsers.TagUpdater(@"{{wikify|date=May, 2010}}"), "removes comma between month and year");
            Assert.AreEqual(@"{{wikify|date=May 2010}}", Parsers.TagUpdater(@"{{wikify|date=11 May 2010}}"), "removes day in International date");
            Assert.AreEqual(@"{{wikify|date=May 2010}}", Parsers.TagUpdater(@"{{wikify|date=07 May 2010}}"), "removes day in International date");
            Assert.AreEqual(@"{{wikify|date=May 2010}}", Parsers.TagUpdater(@"{{wikify|date=11 may 2010}}"), "corrects lower case month name, removes day in International date");
            Assert.AreEqual(@"{{wikify|date=May 2010}}", Parsers.TagUpdater(@"{{wikify|date=2010-05-13}}"), "corrects lower case month name");
            Assert.AreEqual(@"{{wikify|date=May 2010}}", Parsers.TagUpdater(@"{{wikify|date=MAY 2010}}"), "corrects upper case month name");
            Assert.AreEqual(@"{{wikify|date=May 2010}}", Parsers.TagUpdater(@"{{wikify|date=MAy 2010}}"), "corrects mixed case month name");

            const string subst = @"{{wikify|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}";
            Assert.AreEqual(subst, Parsers.TagUpdater(subst), "no change when value is subst month/year");

            const string notSubst = @"{{wikify|date={{CURRENTMONTHNAME}} {{CURRENTYEAR}}}}";
            Assert.AreEqual(notSubst, Parsers.TagUpdater(notSubst), "no change when value is non-subst month/year keywords");
        }

        [Test]
        public void TagUpdaterLocalizeDate()
        {
            #if DEBUG
            WikiRegexes.DatedTemplates = Parsers.LoadDatedTemplates(@"{{tl|wikify}}");

            Variables.SetProjectLangCode("ar");
            WikiRegexes.MakeLangSpecificRegexes();

            Assert.AreEqual(@"{{wikify|تاريخ=May 2010}}", Parsers.TagUpdater(@"{{wikify|date=May 2010}}"), "Renames date= when localized");

            Variables.SetProjectLangCode("en");
            WikiRegexes.MakeLangSpecificRegexes();

            Assert.AreEqual(@"{{wikify|date=May 2010}}", Parsers.TagUpdater(@"{{wikify|date=May 2010}}"), "");
            #endif
        }

        [Test]
        public void General()
        {
            Globals.UnitTestBoolValue = false;
            Assert.AreEqual("#REDIRECT [[Test]]", parser.Tagger("#REDIRECT [[Test]]", "Test", false, out noChange, ref summary));
            Assert.IsTrue(noChange);

            Assert.AreEqual(ShortText, parser.Tagger(ShortText, "Talk:Test", false, out noChange, ref summary));
            Assert.IsTrue(noChange);

            string text = parser.Tagger("{{Test Template}}", "Test", false, out noChange, ref summary);

            Assert.IsFalse(WikiRegexes.Wikify.IsMatch(text));
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text));
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text));
            Assert.IsFalse(noChange);

            // {{Pt}} is now {{Pt icon}}
            text = parser.Tagger("hello {{pt}} hello", "Test", false, out noChange, ref summary);
            Assert.IsFalse(noChange);

            text = parser.Tagger("hello {{Pt}} hello", "Test", false, out noChange, ref summary);
            Assert.IsFalse(noChange);
        }

        [Test]
        public void TaggerPermitted()
        {
            Assert.IsTrue(Parsers.TaggerPermitted("A", "Wikipedia:AutoWikiBrowser/Sandbox"));
            Assert.IsFalse(Parsers.TaggerPermitted("A", "Wikipedia:ABC"));
            Assert.IsFalse(Parsers.TaggerPermitted("A {{wi}}", "ABC"));
            Assert.IsFalse(Parsers.TaggerPermitted("A", "Main Page"));
            Assert.IsFalse(Parsers.TaggerPermitted("#REDIRECT [[A]]", "ABC"));
            Assert.IsFalse(Parsers.TaggerPermitted("{{soft redirect}}", "ABC"));

            #if DEBUG
            Variables.SetProjectLangCode("ar");
            Assert.IsFalse(Parsers.TaggerPermitted("A", "Talk:A"));

            Variables.Namespaces.Add(104, "Special104");
            Assert.IsTrue(Parsers.TaggerPermitted("A", "Special104:A"));
            Variables.Namespaces.Remove(104);
            Variables.SetProjectLangCode("en");
            #endif
        }

        [Test]
        public void ConversionsTestsCitationNeeded()
        {
            string correct = @"{{citation needed|date=May 2010}}";

            Assert.AreEqual(correct, Parsers.Conversions(@"{{citation needed|May 2010|date=May 2010}}"));
            Assert.AreEqual(correct, Parsers.Conversions(@"{{citation needed|may 2010|date=May 2010}}"));
            Assert.AreEqual(correct, Parsers.Conversions(@"{{citation needed|  May 2010  |date=May 2010}}"));
            Assert.AreEqual(correct.Replace("ci", "Ci"), Parsers.Conversions(@"{{Citation needed|May 2010|date=May 2010}}"));
        }

        [Test]
        public void MultipleIssues()
        {
            const string a1 = @"{{Wikify}} {{expand}}", a2 = @" {{COI}}", a3 = @" the article";
            const string a4 = @" {{COI|date=May 2008}}", a5 = @"{{multiple issues|POV|prose|spam}} ";
            const string a4A = @" {{COI|Date=May 2008}}", a4B = @"{{COI|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}";

            // adding new {{multiple issues}}
            Assert.IsTrue(parser.MultipleIssuesOld(a1 + a2 + a3).Contains(@"{{Multiple issues|wikify = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|expand = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|COI = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"));
            Assert.IsTrue(parser.MultipleIssuesOld(a1 + a4 + a3).Contains(@"{{Multiple issues|wikify = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|expand = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|COI = May 2008}}"));
            Assert.IsTrue(parser.MultipleIssuesOld(a1 + a4B + a3).Contains(@"{{Multiple issues|wikify = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|expand = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|COI = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"));
            Assert.IsTrue(parser.MultipleIssuesOld(a1 + @"{{cleanup-rewrite}}" + a3).Contains(@"{{Multiple issues|wikify = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|expand = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|rewrite = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"), "takes cleanup-rewrite, adds as rewrite");
            Assert.IsTrue(parser.MultipleIssuesOld(a1 + @"{{Cleanup-rewrite}}" + a3).Contains(@"{{Multiple issues|wikify = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|expand = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|rewrite = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"));
            Assert.IsTrue(parser.MultipleIssuesOld(a1 + @"{{primary sources}}" + a3).Contains(@"{{Multiple issues|wikify = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|expand = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|primarysources = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"));
            Assert.IsTrue(parser.MultipleIssuesOld(a1 + @"{{very long}}" + a3).Contains(@"{{Multiple issues|wikify = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|expand = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|verylong = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"));
            Assert.IsTrue(parser.MultipleIssuesOld(a1 + @"{{cleanup-jargon}}" + a3).Contains(@"{{Multiple issues|wikify = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|expand = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|jargon = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"), "takes cleanup-jargon, adds as jargon");
            Assert.IsTrue(parser.MultipleIssuesOld(a1 + @"{{cleanup-laundry}}" + a3).Contains(@"{{Multiple issues|wikify = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|expand = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|laundrylists = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"), "takes cleanup-laundry, adds as laundrylists");
            Assert.IsTrue(parser.MultipleIssuesOld(a1 + @"{{cleanup-reorganise}}" + a3).Contains(@"{{Multiple issues|wikify = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|expand = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|restructure = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"), "takes cleanup-reorgnise, adds as restructure");
            Assert.IsTrue(parser.MultipleIssuesOld(a1 + @"{{cleanup-reorganize}}" + a3).Contains(@"{{Multiple issues|wikify = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|expand = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|restructure = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"), "takes cleanup-reorgnize, adds as restructure");
            Assert.IsTrue(parser.MultipleIssuesOld(a1 + @"{{cleanup-spam}}" + a3).Contains(@"{{Multiple issues|wikify = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|expand = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|spam = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"), "takes cleanup-spam, adds as spam");
            Assert.IsTrue(parser.MultipleIssuesOld(a1 + @"{{criticism section}}" + a3).Contains(@"{{Multiple issues|wikify = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|expand = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|criticisms = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"), "takes criticism section, adds as criticisms");
            Assert.IsTrue(parser.MultipleIssuesOld(a1 + @"{{game guide}}" + a3).Contains(@"{{Multiple issues|wikify = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|expand = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|gameguide = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"), "takes game guide, adds as gameguide");
            Assert.IsTrue(parser.MultipleIssuesOld(a1 + @"{{travel guide}}" + a3).Contains(@"{{Multiple issues|wikify = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|expand = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|travelguide = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"), "takes travel guide, adds as travelguide");
            Assert.IsTrue(parser.MultipleIssuesOld(a1 + @"{{news release}}" + a3).Contains(@"{{Multiple issues|wikify = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|expand = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|newsrelease = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"), "takes news release, adds as newsrelease");
            Assert.IsTrue(parser.MultipleIssuesOld(a1 + @"{{POV-check}}" + a3).Contains(@"{{Multiple issues|wikify = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|expand = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|pov-check = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"), "takes POV-check, adds as pov-check");
            Assert.IsTrue(parser.MultipleIssuesOld(a1 + @"{{expert-subject|Foo}}" + a3).Contains(@"{{Multiple issues|wikify = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|expand = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|expert = Foo}}"), "takes expert-subject, adds as expert");
            Assert.IsTrue(parser.MultipleIssuesOld(a1 + @"{{expert-subject|Foo|date=May 2012}}" + a3).Contains(@"{{Multiple issues|wikify = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|expand = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|expert = Foo|date = May 2012}}"), "takes expert-subject, adds as expert");
            Assert.IsTrue(parser.MultipleIssuesOld(a1 + @"{{copy edit|for=grammar}}" + a3).Contains(@"{{Multiple issues|wikify = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|expand = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|grammar = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"), "takes copy edit|for=grammar, adds as grammar");

            // amend existing {{multiple issues}}
            Assert.IsTrue(parser.MultipleIssuesOld(a5 + a1 + a2 + a3).Contains(@"{{multiple issues|POV|prose|spam|wikify = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|expand = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|COI = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}"));
            Assert.IsTrue(parser.MultipleIssuesOld(a5 + a1 + a4 + a3).Contains(@"{{multiple issues|POV|prose|spam|wikify = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|expand = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|COI = May 2008}}"));
            Assert.IsTrue(parser.MultipleIssuesOld(a5 + a1 + a4A + a3).Contains(@"{{multiple issues|POV|prose|spam|wikify = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|expand = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|COI = May 2008}}"));

            Assert.IsTrue(parser.MultipleIssuesOld(@"{{multiple issues|OR=May 2010|COI=May 2010}} {{wikify|date=June 2010}}").Contains(@"{{multiple issues|OR=May 2010|COI=May 2010|wikify = June 2010}}"), "OR renamed for tag counting");

            // insufficient tags
            Assert.IsFalse(Parsers.Conversions(a1 + a3).Contains(@"{{multiple issues"));

            // before first heading tag can be used
            const string a7 = @"{{trivia}}
==heading==";
            Assert.IsTrue(parser.MultipleIssuesOld(a5 + a3 + a7).Contains(@"{{multiple issues|POV|prose|spam|trivia = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"));

            // don't grab tags in later sections of article
            const string a6 = @"
==head== {{essay}}";
            Assert.AreEqual(a5 + a3 + a6, parser.MultipleIssuesOld(a5 + a3 + a6));
        }

        [Test]
        public void MultipleIssuesTitleCase()
        {
            // title case parameters converted to lowercase
            Assert.AreEqual(@"{{article issues|POV=May 2008|cleanup=May 2008|expand=June 2007}}", parser.MultipleIssuesOld(@"{{article issues|POV=May 2008|cleanup=May 2008|Expand=June 2007}}"));
            Assert.AreEqual(@"{{article issues|POV=May 2008|cleanup=May 2008|expand=June 2007}}", parser.MultipleIssuesOld(@"{{article issues|POV=May 2008|cleanup=May 2008|Expand=June 2007}}"));
            Assert.AreEqual(@"{{article issues|POV=May 2008|cleanup=May 2008| expand = June 2007}}", parser.MultipleIssuesOld(@"{{article issues|POV=May 2008|cleanup=May 2008| Expand = June 2007}}"));
            Assert.AreEqual(@"{{Articleissues|BLPunsourced=May 2008|cleanup=May 2008|expand=June 2007}}", parser.MultipleIssuesOld(@"{{Articleissues|BLPunsourced=May 2008|Cleanup=May 2008|expand=June 2007}}"));
            Assert.AreEqual(@"{{article issues|POV=May 2008|cleanup=May 2008|unreferencedBLP=June 2007}}", parser.MultipleIssuesOld(@"{{article issues|POV=May 2008|cleanup=May 2008|UnreferencedBLP=June 2007}}"));
        }

        [Test]
        public void MultipleIssuesSingleTag()
        {
            Assert.AreEqual(@"{{cleanup|date=January 2008}} Article text here", parser.MultipleIssuesOld(@"{{multipleissues|cleanup=January 2008}} Article text here"));
            Assert.AreEqual(@"{{cleanup|date=January 2008}} Article text here", parser.MultipleIssuesOld(@"{{multiple issues|cleanup=January 2008}} Article text here"));
            Assert.AreEqual(@"{{cleanup|date=January 2008}} Article text here", parser.MultipleIssuesOld(@"{{multiple issues|cleanup=January 2008
}} Article text here"));
            Assert.AreEqual(@"{{cleanup|date=January 2008}} Article text here", parser.MultipleIssuesOld(@"{{multipleissues|
            cleanup=January 2008}} Article text here"));
            Assert.AreEqual(@"{{cleanup|date=January 2009}} Article text here", parser.MultipleIssuesOld(@"{{multipleissues|cleanup=January 2009}} Article text here"));
            Assert.AreEqual(@"{{trivia|date=January 2008}} Article text here", parser.MultipleIssuesOld(@"{{multipleissues|trivia = January 2008}} Article text here"));
            Assert.AreEqual(@"{{trivia|date=May 2010}} Article text here", parser.MultipleIssuesOld(@"{{multipleissues|trivia = May 2010}} Article text here"));
            Assert.AreEqual(@"{{cleanup|date=January 2008}} Article text here", parser.MultipleIssuesOld(@"{{multipleissues|cleanup=January 2008|}} Article text here"));
            Assert.AreEqual(@"{{cleanup|date=January 2008}} Article text here", parser.MultipleIssuesOld(@"{{multiple issues|cleanup=January 2008|}} Article text here"));
            Assert.AreEqual(@"{{OR|date=January 2008}} Article text here", parser.MultipleIssuesOld(@"{{multipleissues|OR=January 2008}} Article text here"));

            // no changes
            string a = @"{{multiple issues|trivia=January 2008|cleanup=January 2008}} Article text here";
            Assert.AreEqual(a, parser.MultipleIssuesOld(a));

            a = @"{{ARTICLEISSUES|cleanup=January 2008}} Article text here";
            Assert.AreEqual(a, parser.MultipleIssuesOld(a));

            a = @"{{multiple issues|cleanup=May 2007|trivia=January 2008}} Article text here";
            Assert.AreEqual(a, parser.MultipleIssuesOld(a));

            a = @"{{multiple issues|section=y|refimprove=February 2007|OR=December 2009}}  Article text here";
            Assert.AreEqual(a, parser.MultipleIssuesOld(a));

            a = @"{{multiple issues|section=yes|
{{essay|section|date = October 2011}}
{{unbalanced|section|date = October 2011}}
}}";
            Assert.AreEqual(a, parser.MultipleIssuesOld(a));
        }

        [Test]
        public void MultipleIssuesNoTags()
        {
            Assert.AreEqual("", parser.MultipleIssues("{{multiple issues}}"));
            Assert.AreEqual("{{Advert|date=May 2012}}", parser.MultipleIssues(@"{{multiple issues}}{{Advert|date=May 2012}}"));
        }

        [Test]
        public void MultipleIssuesTagCount()
        {
            // add tags if total would reach 3
            Assert.AreEqual(@"{{multiple issues|POV=May 2008|wikify = May 2007|cleanup = June 2008}}  ", parser.MultipleIssuesOld(@"{{multiple issues|POV=May 2008}} {{wikify|date=May 2007}} {{cleanup|date=June 2008}}"));
            
            Assert.AreEqual(@"{{multiple issues|POV=May 2008|wikify = May 2007}} ", parser.MultipleIssuesOld(@"{{multiple issues|POV=May 2008}} {{wikify|date=May 2007}}"));
                 
        }

        [Test]
        public void MultipleIssuesDateField()
        {
            // don't remove date field where expert field is using it
            const string ESD = @"{{Article issues|cleanup=March 2008|expert=Anime and manga|refimprove=May 2008|date=February 2009}}";
            Assert.AreEqual(ESD, parser.MultipleIssuesOld(ESD));

            // can remove date field when expert=Month YYYY
            Assert.AreEqual(@"{{multiple issues|cleanup=March 2008|expert=May 2008|refimprove=May 2008}}", parser.MultipleIssuesOld(@"{{multiple issues|Cleanup=March 2008|expert=May 2008|refimprove=May 2008|date=February 2009}}"));

            // date field removed where no expert field to use it
            Assert.AreEqual(@"{{multiple issues|cleanup=March 2008|COI=March 2008|refimprove=May 2008}}", parser.MultipleIssuesOld(@"{{multiple issues|Cleanup=March 2008|COI=March 2008|refimprove=May 2008|date=February 2009}}"));
            // removal of non-existent date field
            Assert.AreEqual(@"{{multiple issues|wikfy=May 2008|COI=May 2008|cleanup=May 2008}}", parser.MultipleIssuesOld(@"{{multiple issues|wikfy=May 2008|COI=May 2008|cleanup=May 2008|date = March 2007}}"));
            Assert.AreEqual(@"{{multiple issues|wikfy=May 2008|COI=May 2008|cleanup=May 2008}}", parser.MultipleIssuesOld(@"{{multiple issues|wikfy=May 2008|COI=May 2008|cleanup=May 2008| date=March 2007}}"));
            Assert.AreEqual(@"{{multiple issues|wikfy=May 2008|COI=May 2008|cleanup=May 2008}}", parser.MultipleIssuesOld(@"{{multiple issues|wikfy=May 2008|COI=May 2008|date = March 2007|cleanup=May 2008}}"));

            Assert.AreEqual(@"{{Multiple issues|wikfy=May 2008|COI=May 2008|cleanup=May 2008}}", parser.MultipleIssuesOld(@"{{Multiple issues|wikfy=May 2008|COI=May 2008|date = March 2007|cleanup=May 2008}}"));

            // tags with a parameter value that's not a date are not supported
            Assert.AreEqual(@"{{multiple issues|wikfy=May 2008|copyedit=April 2009|COI=May 2008}} {{update|some date reason}}", parser.MultipleIssuesOld(@"{{multiple issues|wikfy=May 2008|copyedit=April 2009|COI=May 2008}} {{update|some date reason}}"));

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#ArgumentException_in_Parsers.ArticleIssues
            Assert.AreEqual("", parser.MultipleIssuesOld(""));
        }

        [Test]
        public void MultipleIssuesLimits()
        {
            const string bug1 = @"{{article issues|disputed=June 2009|primarysources=June 2009}}
{{Expert}}";
            Assert.AreEqual(bug1, parser.MultipleIssuesOld(bug1));

            const string bug2 = @"{{article issues|article=y
|update=November 2008
|out of date=July 2009
|cleanup=July 2009
|wikify=July 2009}}";

            Assert.AreEqual(bug2, Parsers.Conversions(bug2));

            const string ManyInSection  = @"==Section==
{{multiple issues |
{{orphan}}
}}
{{dead end}}
{{multiple issues |
{{refimprove}}
}}";
            Assert.AreEqual(ManyInSection, parser.MultipleIssues(ManyInSection));

            const string Many  = @"{{multiple issues |
{{orphan}}
}}
{{dead end}}
{{multiple issues |
{{refimprove}}
}}";
            Assert.AreEqual(Many, parser.MultipleIssues(Many));

            const string dupe  = @"{{unreferenced}}
{{unreferenced}}";
            Assert.AreEqual(dupe, parser.MultipleIssues(dupe));

            const string LaterInSection = @"Bats.

==Con==
{{Refimprove section|date=January 2009}}
The.

{{multiple issues|section = yes|
{{unreferenced section|date = November 2014}}
{{confusing section |date = November 2014}}
}}

Chris.";
            Assert.AreEqual(LaterInSection, parser.MultipleIssues(LaterInSection));
        }

        [Test]
        public void MultipleIssuesDupeParameters()
        {
            Assert.AreEqual(@"{{Multiple issues|wikify=May 2008|COI=May 2008|expand =March 2009|POV=May 2008}}", parser.MultipleIssuesOld(@"{{Multiple issues|wikify=May 2008|COI=May 2008|expand =March 2009|POV=May 2008}}{{POV}}"), "duplicate undated tag removed");
            Assert.AreEqual(@"{{Multiple issues|wikify=May 2008|COI=May 2008|expand =March 2009|POV=May 2008}}", parser.MultipleIssuesOld(@"{{Multiple issues|wikify=May 2008|COI=May 2008|expand =March 2009|POV=May 2008}}{{POV|date=June 2010}}"), "duplicate dated tag removed");
            Assert.AreEqual(@"{{Multiple issues|wikify=May 2008|COI=May 2008|expand =March 2009|POV=May 2008}}", parser.MultipleIssuesOld(@"{{Multiple issues|wikify=May 2008|COI=May 2008|expand =March 2009|POV=May 2008}}{{Expand}}{{POV}}"), "duplicate undated tags removed");
        }

        [Test]
        public void ArticleIssuesDates()
        {
            // addition of date
            Assert.AreEqual(@"{{Multiple issues|wikify=May 2008|COI=May 2008|expand={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}", Parsers.Conversions(@"{{Multiple issues|wikify=May 2008|COI=May 2008|expand}}"));
            Assert.AreEqual(@"{{Multiple issues|wikify=May 2008|expand={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|COI=May 2008}}", Parsers.Conversions(@"{{Multiple issues|wikify=May 2008|expand|COI=May 2008}}"));

            Assert.AreEqual(@"{{Multiple issues|wikify=May 2008|notability={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|COI=May 2008}}", Parsers.Conversions(@"{{Multiple issues|wikify=May 2008|notability|COI=May 2008}}"));

            // multiple dates
            Assert.AreEqual(@"{{Multiple issues|wikify=May 2008|COI=May 2008|expand={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|external links={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}", Parsers.Conversions(@"{{Multiple issues|wikify=May 2008|COI=May 2008|expand|external links}}"));
            Assert.AreEqual(@"{{Multiple issues|wikify={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|expand={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|external links=June 2009}}", Parsers.Conversions(@"{{Multiple issues|wikify|expand|external links=June 2009}}"));

            // removal of date word
            Assert.AreEqual(@"{{Multiple issues|wikify=May 2008|COI=May 2008|expand =March 2009|POV=May 2008}}", Parsers.Conversions(@"{{Multiple issues|wikify=May 2008|COI=May 2008|expand date=March 2009|POV=May 2008}}"));
            Assert.AreEqual(@"{{Multiple issues|wikify =May 2008|COI =May 2008|expand =March 2009|POV =May 2008}}", Parsers.Conversions(@"{{Multiple issues|wikify date=May 2008|COI date=May 2008|expand date=March 2009|POV date=May 2008}}"));
            Assert.AreEqual(@"{{Multiple issues|wikify=May 2008|COI=May 2008|expand =March 2009}}", Parsers.Conversions(@"{{Multiple issues|wikify=May 2008|COI=May 2008|expand date=March 2009}}"));
            Assert.AreEqual(@"{{Multiple issues|unreferenced=March 2009|wikify=March 2009|cleanup=March 2009|autobiography={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|COI = April 2009}}", Parsers.Conversions(@"{{Multiple issues|unreferenced=March 2009|wikify=March 2009|cleanup=March 2009|autobiography={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|COI =date April 2009}}"));
            Assert.AreEqual(@"{{Multiple issues|wikify ={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|COI ={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|expand ={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|POV ={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}", Parsers.Conversions(@"{{Multiple issues|wikify date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|COI date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|expand date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|POV date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"));
            Assert.AreEqual(@"{{Multiple issues|underlinked =February 2009|out of date =October 2009|context =October 2009}}", Parsers.Conversions(@"{{Multiple issues|underlinked =February 2009|out of date date=October 2009|context date=October 2009}}"));

            // clean of 'do-attempt =July 2006|att=April 2008'
            Assert.AreEqual(@"{{Multiple issues|wikify=May 2008|COI=May 2008|do-attempt =April 2008}}", Parsers.Conversions(@"{{Multiple issues|wikify=May 2008|COI=May 2008|do-attempt =July 2006|att=April 2008}}"));
            Assert.AreEqual(@"{{Multiple issues|wikify=May 2008|do-attempt =April 2008|COI=May 2008}}", Parsers.Conversions(@"{{Multiple issues|wikify=May 2008|do-attempt =July 2006|att=April 2008|COI=May 2008}}"));

            // clean of "Copyedit|for=grammar|date=April 2009"to "Copyedit=April 2009"
            Assert.AreEqual(@"{{Multiple issues|wikify=May 2008|COI=May 2008|Copyedit =April 2009}}", Parsers.Conversions(@"{{Multiple issues|wikify=May 2008|COI=May 2008|Copyedit for=grammar|date=April 2009}}"));
            Assert.AreEqual(@"{{Multiple issues|wikify=May 2008|copyedit=April 2009|COI=May 2008}}", Parsers.Conversions(@"{{Multiple issues|wikify=May 2008|copyeditfor=grammar|date=April 2009|COI=May 2008}}"));

            // don't add date for expert field
            const string a1 = @"{{Multiple issues|wikify=May 2008|COI=May 2008|expert}}";
            Assert.AreEqual(a1, Parsers.Conversions(a1));

            const string a2 = @"{{Multiple issues|wikify=May 2008|COI=May 2008|expert=Fred}}";
            Assert.AreEqual(a2, Parsers.Conversions(a2));
            
            // expert must have parameter
            Assert.AreEqual(@"{{Multiple issues|wikify=May 2008|COI=May 2008|do-attempt =July 2006}} {{expert}}", 
                            parser.MultipleIssuesOld(@"{{Multiple issues|wikify=May 2008|COI=May 2008|do-attempt =July 2006}} {{expert}}"));

            // don't remove 'update'field
            const string a3 = @"{{Multiple issues|wikify=May 2008|COI=May 2008|update=May 2008}}";
            Assert.AreEqual(a3, Parsers.Conversions(a3));
        }

        [Test]
        public void MultipleIssuesEnOnly()
        {
#if DEBUG
            Variables.SetProjectLangCode("fr");

            const string a1 = @"{{Wikify}} {{expand}}", a2 = @" {{COI}}", a3 = @" the article";

            // adding new {{article issues}}
            Assert.IsFalse(parser.MultipleIssuesOld(a1 + a2 + a3).Contains(@"{{Multiple issues|wikify = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|expand = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|COI}}"));

            Variables.SetProjectLangCode("en");

            Assert.IsTrue(parser.MultipleIssuesOld(a1 + a2 + a3).Contains(@"{{Multiple issues|wikify = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|expand = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|COI = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"));
#endif
        }

        [Test]
        public void MultipleIssuesUnref()
        {
            string at = @"Foo
[[Category:Living people]]", ai = @"{{Multiple issues|wikify=May 2008 | expand=June 2007 | COI=March 2010  | unref=June 2009}}";

            Assert.AreEqual(ai.Replace("unref", "BLP unsourced") + at, parser.MultipleIssuesOld(ai + at), "unref changed if article about a person");
            Assert.AreEqual(ai.Replace("unref", "BLP unsourced") + at, parser.MultipleIssuesOld(ai.Replace("unref", "unreferenced") + at), "unreferenced changed if article about a person");

            Assert.AreEqual(ai + "foo", parser.MultipleIssuesOld(ai + "foo"), "unref not changed if article not about a person");
            Assert.AreEqual(ai + "foo {{persondata|here=there}}", parser.MultipleIssuesOld(ai + "foo {{persondata|here=there}}"), "unref not changed if article not about a living person");

            Assert.IsTrue(parser.MultipleIssuesOld(@"{{wikify|date=May 2008}} {{expand|date=June 2007}} {{COI|date=March 2008}} {{unref|date=June 2009}} " + at).Contains("BLP unsourced"), "unref changed if article about a person when adding {{Multiple issues}}");
        }

        [Test]
        public void RedirectTaggerModDashes()
        {
            const string correct = @"#REDIRECT:[[Foo–bar]] {{R from modification}}", redirectendash = @"#REDIRECT:[[Foo–bar]]";
            Assert.AreEqual(correct, Parsers.RedirectTagger(redirectendash, "Foo-bar"));

            // already tagged
            Assert.AreEqual(correct, Parsers.RedirectTagger(correct, "Foo-bar"));

            // different change
            Assert.AreEqual(redirectendash, Parsers.RedirectTagger(redirectendash, "Foo barism"));
        }

        [Test]
        public void RedirectTaggerModPunct()
        {
            const string correct = @"#REDIRECT:[[Foo .bar]] {{R from modification}}", redirectpunct = @"#REDIRECT:[[Foo .bar]]";

            // removed punct
            Assert.AreEqual(correct, Parsers.RedirectTagger(redirectpunct, "Foo bar"));

            // different punct
            Assert.AreEqual(correct, Parsers.RedirectTagger(redirectpunct, "Foo /bar"));

            // extra punct
            Assert.AreEqual(correct, Parsers.RedirectTagger(redirectpunct, "Foo ..bar"));

            // other changes in addition to punctuation
            Assert.AreEqual(redirectpunct, Parsers.RedirectTagger(redirectpunct, "Foo ..bar (long)"));
        }

        [Test]
        public void RedirectTaggerDiacr()
        {
            const string correct = @"#REDIRECT:[[Fiancée]] {{R from title without diacritics}}", redirectaccent = @"#REDIRECT:[[Fiancée]]";
            Assert.AreEqual(correct, Parsers.RedirectTagger(redirectaccent, "Fiancee"));

            // already tagged
            Assert.AreEqual(correct, Parsers.RedirectTagger(correct, "Fiancee"));

            // different change
            Assert.AreEqual(redirectaccent, Parsers.RedirectTagger(redirectaccent, "Fiancee-bar"));
        }

        [Test]
        public void RedirectTagger()
        {
            const string redirecttext = @"#REDIRECT:[[Foo bar]]";
            const string redirecttext2 = @"#REDIRECT:[[foo bar]]";

            // skips recursive redirects
            Assert.AreEqual(redirecttext, Parsers.RedirectTagger(redirecttext, "Foo bar"));
            Assert.AreEqual(redirecttext2, Parsers.RedirectTagger(redirecttext2, "Foo bar"));
            Assert.AreEqual(redirecttext, Parsers.RedirectTagger(redirecttext, "foo bar"));

            // skips if not a redirect
            string notredirect = @"Now foo bar";
            Assert.AreEqual(notredirect, Parsers.RedirectTagger(notredirect, "Foo bar"));

            // don't tag if already tagged
            string alreadytagged = @"#REDIRECT [[Bethune's Gully]] {{R from alternative spelling}}";

            Assert.AreEqual(alreadytagged, Parsers.RedirectTagger(alreadytagged, @"""Bethune's Gully"""));
        }

        [Test]
        public void RedirectTaggerCapitalisation()
        {
            const string correct = @"#REDIRECT:[[FooBar]] {{R from other capitalisation}}", redirectCap = @"#REDIRECT:[[FooBar]]";
            Assert.AreEqual(correct, Parsers.RedirectTagger(redirectCap, "Foobar"));
            Assert.AreEqual(correct, Parsers.RedirectTagger(redirectCap, "foobar"));
            Assert.AreEqual(correct, Parsers.RedirectTagger(redirectCap, "FOObar"));
        }

        [Test]
        public void RedirectTaggerOtherNamespace()
        {
            Assert.AreEqual(@"#REDIRECT:[[Project:FooBar]] {{R to project namespace}}", Parsers.RedirectTagger(@"#REDIRECT:[[Project:FooBar]]", "FooBar"));
            Assert.AreEqual(@"#REDIRECT:[[Help:FooBar]] {{R to help namespace}}", Parsers.RedirectTagger(@"#REDIRECT:[[Help:FooBar]]", "FooBar"));
            Assert.AreEqual(@"#REDIRECT:[[Portal:FooBar]] {{R to portal namespace}}", Parsers.RedirectTagger(@"#REDIRECT:[[Portal:FooBar]]", "FooBar"));
            Assert.AreEqual(@"#REDIRECT:[[Template:FooBar]] {{R to template namespace}}", Parsers.RedirectTagger(@"#REDIRECT:[[Template:FooBar]]", "FooBar"));
            Assert.AreEqual(@"#REDIRECT:[[Category:FooBar]] {{R to category namespace}}", Parsers.RedirectTagger(@"#REDIRECT:[[Category:FooBar]]", "FooBar"));
            Assert.AreEqual(@"#REDIRECT:[[User:FooBar]] {{R to user namespace}}", Parsers.RedirectTagger(@"#REDIRECT:[[User:FooBar]]", "FooBar"));
            Assert.AreEqual(@"#REDIRECT:[[Talk:FooBar]] {{R to talk namespace}}", Parsers.RedirectTagger(@"#REDIRECT:[[Talk:FooBar]]", "FooBar"));
            Assert.AreEqual(@"#REDIRECT:[[Template talk:FooBar]] {{R to other namespace}}", Parsers.RedirectTagger(@"#REDIRECT:[[Template talk:FooBar]]", "FooBar"));

            const string correct = @"#REDIRECT:[[Category:FooBar]] {{R to other namespace}}", redirectNam = @"#REDIRECT:[[Category:FooBar]]";

            Assert.AreEqual(correct, Parsers.RedirectTagger(correct, "FooBar"), "No change when already tagged");
            Assert.AreEqual(redirectNam, Parsers.RedirectTagger(redirectNam, "Template:FooBar"), "Not tagged when redirect not in mainspace");
        }

        [Test]
        public void TagRefsIbid()
        {
            string summary = "";
            string returned = parser.Tagger(@"now<ref>ibid</ref> was", "test", false, ref summary);

            Assert.IsTrue(returned.Contains(@"{{Ibid|date="));
            Assert.IsTrue(summary.Contains("Ibid"));

            returned = parser.Tagger(@"now<ref name=""again"">ibid</ref> was", "test", false, ref summary);

            Assert.IsTrue(returned.Contains(@"{{Ibid|date="));
            Assert.IsTrue(summary.Contains("Ibid"));

            returned = parser.Tagger(@"now<ref>foo</ref> was", "test", false, ref summary);
            Assert.IsFalse(returned.Contains(@"{{Ibid|date="));
            Assert.IsFalse(summary.Contains("Ibid"));

            returned = parser.Tagger(@"now<Ref name=""again"">IBID</ref> was", "test", false, ref summary);
            Assert.IsTrue(returned.Contains(@"{{Ibid|date="), "Case-insensitive check for ibid");
        }

        [Test]
        public void TagEmptySection()
        {
            string summary = "";
            string twoTwos = @"==Foo1==
==Foo2==
", commentedOutTwoTwos = @"
<!--
text
==Foo1==
==Foo2==
text -->
";
            string returned = parser.Tagger(twoTwos, "test", false, ref summary);
            Assert.IsTrue(returned.Contains(@"==Foo1==
{{Empty section|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}

==Foo2==
"));
            Assert.IsTrue(summary.Contains("Empty section (1)"));

            twoTwos = @"==Foo1==

==Foo2==
";
            returned = parser.Tagger(twoTwos, "test", false, ref summary);
            Assert.IsTrue(returned.Contains(@"{{Empty section|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}

==Foo2==
"));

            // tagging multiple sections
            summary = "";
            returned = parser.Tagger(twoTwos + "\r\n" + twoTwos, "test", false, ref summary);
            Assert.IsTrue(returned.Contains(@"{{Empty section|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}

==Foo2==
"));
            Assert.IsTrue(summary.Contains("Empty section (3)"));

            // not empty
            twoTwos = @"==Foo1==
x
==Foo2==
";
            returned = parser.Tagger(twoTwos, "test", false, ref summary);
            Assert.IsFalse(returned.Contains(@"{{Empty section|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"));

            // level 3
            twoTwos = @"===Foo1===

===Foo2===
";
            returned = parser.Tagger(twoTwos, "test", false, ref summary);
            Assert.IsFalse(returned.Contains(@"{{Empty section|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"));

            // commented out sections - no change
            returned = parser.Tagger(commentedOutTwoTwos, "test", false, ref summary);
            Assert.IsFalse(returned.Contains(@"{{Empty section|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"));

            // single letter heading – alpha list – empty section allowed – no change
            returned = parser.Tagger(@"==F==
==G==", "test", false, ref summary);
            Assert.IsFalse(returned.Contains(@"{{Empty section|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"));

            // section with pre is not empty
            returned = parser.Tagger(twoTwos.Replace(@"==Foo1==", @"==Foo1==
<pre>
foo
</pre>"), "test", false, ref summary);
            Assert.IsFalse(returned.Contains(@"{{Empty section|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"));
        }
        
        [Test]
        public void RegularCategories()
        {
            List<Article> Cats = new  List<Article>();
            Assert.AreEqual(0, Parsers.RegularCategories(Cats).Count);
            
            Cats.Add(new Article("Category:Foo"));
            Assert.AreEqual(1, Parsers.RegularCategories(Cats).Count);
            
            Cats.Add(new Article("Category:Bar"));
            Cats.Add(new Article("Category:Some stubs"));
            Cats.Add(new Article("Category:A :Stubs"));
            Cats.Add(new Article("Category:Stubs"));
            Assert.AreEqual(2, Parsers.RegularCategories(Cats).Count);
            
            Cats.Add(new Article("Category:Proposed deletion"));
            Cats.Add(new Article("Category:Foo proposed deletions"));
            Cats.Add(new Article("Category:Foo proposed for deletion"));
            Cats.Add(new Article("Category:Articles created via the Article Wizard"));
            Assert.AreEqual(2, Parsers.RegularCategories(Cats).Count);
            
            Cats.Clear();
            Assert.AreEqual(0, Parsers.RegularCategories("").Count);
            Assert.AreEqual(1, Parsers.RegularCategories("[[Category:Foo]]").Count);
            Assert.AreEqual(1, Parsers.RegularCategories("[[Category:Foo]] [[Category:Some stubs]]").Count);
            Assert.AreEqual(1, Parsers.RegularCategories("[[Category:Foo]] <!--[[Category:Bar]]-->").Count);
        }
        
    }

    [TestFixture]
    public class MultipleIssuesNewTests : RequiresParser
    {
        [Test]
        public void MultipleIssuesNewZerothTagLines()
        {
            Assert.AreEqual(@"", parser.MultipleIssues(@""));
            
            Assert.AreEqual(@"{{Multiple issues|
{{wikify}}
{{unreferenced}}
{{POV}}
}}", parser.MultipleIssues(@"{{wikify}} {{unreferenced}} {{POV}}"), "preserves tag order adding new tags");
            
            Assert.AreEqual(@"{{Multiple issues|
{{wikify}}
{{unreferenced}}
{{POV}}
}}

==hello==", parser.MultipleIssues(@"{{wikify}}{{unreferenced}}{{POV}}
==hello=="), "takes tags from same line");
            
            Assert.AreEqual(@"{{Multiple issues|
{{wikify}}
{{unreferenced}}
{{POV}}
}}



==hello==", parser.MultipleIssues(@"{{wikify}}
{{unreferenced}}
{{POV}}
==hello=="), "takes tags from separate lines, takes tags without dates");
        }
        
        [Test]
        public void MultipleIssuesNewZerothTagParameters()
        {
            Assert.AreEqual(@"{{Multiple issues|
{{underlinked|date=May 2012}}
{{unreferenced}}
{{POV}}
}}

==hello==", parser.MultipleIssues(@"{{underlinked|date=May 2012}}{{unreferenced}}{{POV}}
==hello=="), "takes tags with dates");
            
            Assert.AreEqual(@"{{Multiple issues|
{{underlinked|date=May 2012|reason=x}}
{{unreferenced}}
{{POV}}
}}

==hello==", parser.MultipleIssues(@"{{underlinked|date=May 2012|reason=x}}{{unreferenced}}{{POV}}
==hello=="), "takes tags with extra parameters");
        }
        
        [Test]
        public void MultipleIssuesNewTagZeroth()
        {
            Assert.AreEqual(@"{{Multiple issues|
{{underlinked|date=May 2012}}
{{unreferenced}}
{{POV}}
}}

Text

==hello==", parser.MultipleIssues(@"{{underlinked|date=May 2012}}{{unreferenced}}
Text
{{POV}}
==hello=="), "takes tags from anywhere in zeroth section");
            
            Assert.AreEqual(@"{{Multiple issues|
{{underlinked|date=May 2012}}
{{unreferenced}}
{{POV}}
}}
Text

==hello==", parser.MultipleIssues(@"Text
{{underlinked|date=May 2012}}{{unreferenced}}{{POV}}
==hello=="), "takes tags from anywhere in zeroth section: all after");
            
        }
        
        [Test]
        public void MultipleIssuesNewZerothExistingMINotChanged()
        {
            const string ThreeTagNew = @"{{multiple issues|
{{underlinked|date=May 2012}}
{{unreferenced}}
{{POV}}
}}
Text

==hello==", TwoTagNew = @"{{multiple issues|
{{unreferenced}}
{{POV}}
}}
Text

==hello==", ThreeTag = @"{{multiple issues|underlinked=May 2012|unreferenced=May 2012|POV=May 2012}}
Text

==hello==", TwoTag = @"{{multiple issues|underlinked=May 2012|unreferenced=May 2012}}
Text

==hello==";
            Assert.AreEqual(ThreeTagNew, parser.MultipleIssues(ThreeTagNew), "no change to existing 3-tag MI new style");
            Assert.AreEqual(TwoTagNew, parser.MultipleIssues(TwoTagNew), "no change to existing 2-tag MI new style");
            
            Assert.AreEqual(ThreeTag, parser.MultipleIssues(ThreeTag), "no change to existing 3-tag MI old style");
            Assert.AreEqual(TwoTag, parser.MultipleIssues(TwoTag), "no change to existing 2-tag MI old style");
        }
        
        [Test]
        public void MultipleIssuesNewZerothExistingMIMoreTags()
        {
            Assert.AreEqual(@"{{multiple issues|
{{wikify|date=May 2012}}
{{peacock}}
{{POV}}
{{unreferenced}}
}}


==hello==", parser.MultipleIssues(@"{{multiple issues|
{{wikify|date=May 2012}}
{{peacock}}
{{POV}}
}}
{{unreferenced}}

==hello=="), "adds tags to existing MI, MI new style");
            
            Assert.AreEqual(@"{{multiple issues|
{{wikify|date=May 2012}}
{{peacock}}
{{advert}}
{{POV}}
{{unreferenced}}
}}



==hello==", parser.MultipleIssues(@"{{multiple issues|
{{wikify|date=May 2012}}
{{peacock}}
{{advert}}
}}
{{POV}}
{{unreferenced}}

==hello=="), "adds tags to existing MI, MI new style");
            
             Assert.AreEqual(@"{{multiple issues|
{{wikify|date=May 2012}}
{{peacock}}
{{POV}}
{{unreferenced}}
}}

==hello==", parser.MultipleIssues(@"{{unreferenced}}{{multiple issues|
{{wikify|date=May 2012}}
{{peacock}}
{{POV}}
}}

==hello=="), "adds tags to existing MI, MI new style, tags before MI");
            
            Assert.AreEqual(@"{{multiple issues|wikify=May 2012|peacock=May 2012|POV=May 2012|
{{unreferenced}}
}}


==hello==", parser.MultipleIssues(@"{{multiple issues|wikify=May 2012|peacock=May 2012|POV=May 2012}}
{{unreferenced}}

==hello=="), "adds tags to existing MI, MI old style");
            
            Assert.AreEqual(@"{{multiple issues|wikify=May 2012|peacock=May 2012|POV=May 2012|
{{unreferenced}}
{{POV}}
}}



==hello==", parser.MultipleIssues(@"{{multiple issues|wikify=May 2012|peacock=May 2012|POV=May 2012}}
{{unreferenced}}
{{POV}}

==hello=="), "adds tags to existing MI, MI old style");
            
            Assert.AreEqual(@"{{multiple issues|
{{wikify|date=May 2012}}
{{unreferenced}}
}}


==hello==", parser.MultipleIssues(@"{{multiple issues|
{{wikify|date=May 2012}}
}}
{{unreferenced}}

==hello=="), "adds 1 tag to existing MI with 1 tag, MI new style");
            
            Assert.AreEqual(@"{{multiple issues|wikify=May 2012|
{{unreferenced}}
}}


==hello==", parser.MultipleIssues(@"{{multiple issues|wikify=May 2012}}
{{unreferenced}}

==hello=="), "adds 1 tag to existing MI with 1 tag, MI old style");
            
            Assert.AreEqual(@"{{multiple issues|
{{wikify|date=May 2012}}
{{peacock}}
{{POV}}
{{unreferenced}}
}}


==hello==", parser.MultipleIssues(@"{{multiple issues|
{{wikify|date=May 2012}}
{{peacock}}
{{POV}}
{{unreferenced}}
}}
{{unreferenced}}

==hello=="), "duplicate tags not added");
            
        }
        
        [Test]
        public void MultipleIssuesNewZerothSingleTag()
        {
            Assert.AreEqual(@"{{wikify|date=May 2012}}", parser.MultipleIssues(@"{{multiple issues|wikify=May 2012}}"), "converts old style 1-tag MI to single template");
            Assert.AreEqual(@"{{wikify}}", parser.MultipleIssues(@"{{multiple issues|
{{wikify}}
}}"), "converts new style 1-tag MI to single template");
            Assert.AreEqual(@"{{wikify|date=May 2012}}", parser.MultipleIssues(@"{{multiple issues|
{{wikify|date=May 2012}}
}}"), "converts new style 1-tag MI to single template, tag with date");
            Assert.AreEqual(@"{{wikify|reason=links}}", parser.MultipleIssues(@"{{multiple issues|
{{wikify|reason=links}}
}}"), "converts new style 1-tag MI to single template, tag with extra parameter");
        }
        
        [Test]
        public void MultipleIssuesOldZerothSingleTag()
        {
            Assert.AreEqual(@"{{cleanup|date=January 2008}} Article text here", parser.MultipleIssues(@"{{multipleissues|cleanup=January 2008}} Article text here"));
            Assert.AreEqual(@"{{cleanup|date=January 2008}} Article text here", parser.MultipleIssues(@"{{multiple issues|cleanup=January 2008}} Article text here"));
            Assert.AreEqual(@"{{cleanup|date=January 2008}} Article text here", parser.MultipleIssues(@"{{multiple issues|cleanup=January 2008
}} Article text here"));
            Assert.AreEqual(@"{{cleanup|date=January 2008}} Article text here", parser.MultipleIssues(@"{{multipleissues|
            cleanup=January 2008}} Article text here"));
            Assert.AreEqual(@"{{cleanup|date=January 2009}} Article text here", parser.MultipleIssues(@"{{multipleissues|cleanup=January 2009}} Article text here"));
            Assert.AreEqual(@"{{trivia|date=January 2008}} Article text here", parser.MultipleIssues(@"{{multipleissues|trivia = January 2008}} Article text here"));
            Assert.AreEqual(@"{{trivia|date=May 2010}} Article text here", parser.MultipleIssues(@"{{multipleissues|trivia = May 2010}} Article text here"));
            Assert.AreEqual(@"{{cleanup|date=January 2008}} Article text here", parser.MultipleIssues(@"{{multipleissues|cleanup=January 2008|}} Article text here"));
            Assert.AreEqual(@"{{cleanup|date=January 2008}} Article text here", parser.MultipleIssues(@"{{multiple issues|cleanup=January 2008|}} Article text here"));
            Assert.AreEqual(@"{{OR|date=January 2008}} Article text here", parser.MultipleIssues(@"{{multipleissues|OR=January 2008}} Article text here"));

            // no changes
            string a = @"{{multiple issues|trivia=January 2008|cleanup=January 2008}} Article text here";
            Assert.AreEqual(a, parser.MultipleIssues(a));

            a = @"{{ARTICLEISSUES|cleanup=January 2008}} Article text here";
            Assert.AreEqual(a, parser.MultipleIssues(a));

            a = @"{{multiple issues|cleanup=May 2007|trivia=January 2008}} Article text here";
            Assert.AreEqual(a, parser.MultipleIssues(a));

            a = @"{{multiple issues|section=y|refimprove=February 2007|OR=December 2009}}  Article text here";
            Assert.AreEqual(a, parser.MultipleIssues(a));

            a = @"{{multiple issues|section=yes|
{{essay|section|date = October 2011}}
{{unbalanced|section|date = October 2011}}
}}";
            Assert.AreEqual(a, parser.MultipleIssues(a));
        }
        
          [Test]
        public void MultipleIssuesNewSectionTagLines()
        {
            Assert.AreEqual(@"", parser.MultipleIssues(@""));
            
            Assert.AreEqual(@"==sec==
{{Multiple issues|section=yes|
{{wikify section}}
{{expand section}}
{{POV-section}}
}}", parser.MultipleIssues(@"==sec==
{{wikify section}} {{expand section}} {{POV-section}}"), "preserves tag order adding new tags");
            
            Assert.AreEqual(@"==sec==
{{Multiple issues|section=yes|
{{wikify section}}
{{expand section}}
{{POV-section}}
}}
A
==hello==", parser.MultipleIssues(@"==sec==
{{wikify section}}{{expand section}}{{POV-section}}
A
==hello=="), "takes tags from same line");
            
            Assert.AreEqual(@"==sec==
{{Multiple issues|section=yes|
{{wikify section}}
{{expand section}}
{{POV-section}}
}}
A
==hello==", parser.MultipleIssues(@"==sec==
{{wikify section}}
{{expand section}}
{{POV-section}}
A
==hello=="), "takes tags from separate lines, takes tags without dates");
        }
        
        [Test]
        public void MultipleIssuesNewSectionTagParameters()
        {
            Assert.AreEqual(@"==sec==
{{Multiple issues|section=yes|
{{wikify section|date=May 2012}}
{{expand section}}
{{POV-section}}
}}
A
==hello==", parser.MultipleIssues(@"==sec==
{{wikify section|date=May 2012}}{{expand section}}{{POV-section}}
A
==hello=="), "takes tags with dates");
            
            Assert.AreEqual(@"==sec==
{{Multiple issues|section=yes|
{{wikify section|date=May 2012|reason=x}}
{{expand section}}
{{POV-section}}
}}
A
==hello==", parser.MultipleIssues(@"==sec==
{{wikify section|date=May 2012|reason=x}}{{expand section}}{{POV-section}}
A
==hello=="), "takes tags with extra parameters");
        }
        
        [Test]
        public void MultipleIssuesNewTagSection()
        {
            const string After = @"==sec==
{{wikify section|date=May 2012}}
Text
{{POV-section}}
==hello==";
            Assert.AreEqual(After, parser.MultipleIssues(After), "Does not take tags from after text in section");

            Assert.AreEqual(@"==sec==
{{Multiple issues|section=yes|
{{wikify section|date=May 2012}}
{{expand section}}
{{POV-section}}
}}
Text
{{unreferenced section}}
==hello==", parser.MultipleIssues(@"==sec==
{{wikify section|date=May 2012}}{{expand section}}{{POV-section}}
Text
{{unreferenced section}}
==hello=="), "takes tags before tag in section");
        }
        
        [Test]
        public void MultipleIssuesNewSectionExistingMINotChanged()
        {
            const string ThreeTagNew = @"==sec==
{{multiple issues|section=yes|
{{wikify|date=May 2012}}
{{expand section}}
{{POV-section}}
}}
Text

==hello==", TwoTagNew = @"==sec==
{{multiple issues|section=yes|
{{expand section}}
{{POV-section}}
}}
Text

==hello==", ThreeTag = @"==sec==
{{multiple issues|section=yes|wikify=May 2012|underlinked=May 2012|POV=May 2012}}
Text

==hello==", TwoTag = @"==sec==
{{multiple issues|section=yes|wikify=May 2012|underlinked=May 2012}}
Text

==hello==";
            Assert.AreEqual(ThreeTagNew, parser.MultipleIssues(ThreeTagNew), "no change to existing 3-tag MI new style");
            Assert.AreEqual(TwoTagNew, parser.MultipleIssues(TwoTagNew), "no change to existing 2-tag MI new style");
            
            Assert.AreEqual(ThreeTag, parser.MultipleIssues(ThreeTag), "no change to existing 3-tag MI old style");
            Assert.AreEqual(TwoTag, parser.MultipleIssues(TwoTag), "no change to existing 2-tag MI old style");
        }
        
        [Test]
        public void MultipleIssuesNewSectionExistingMIMoreTags()
        {
            Assert.AreEqual(@"==sec==
{{multiple issues|
{{wikify section|date=May 2012}}
{{expand section}}
{{POV-section}}
{{unreferenced section}}
}}
==hello==", parser.MultipleIssues(@"==sec==
{{multiple issues|
{{wikify section|date=May 2012}}
{{expand section}}
{{POV-section}}
}}
{{unreferenced section}}

==hello=="), "adds tags to existing MI, MI new style");
            
            Assert.AreEqual(@"==sec==
{{multiple issues|section=yes|
{{wikify section|date=May 2012}}
{{expand section}}
{{POV-section}}
{{unreferenced section}}
}}
==hello==", parser.MultipleIssues(@"==sec==
{{multiple issues|section=yes|
{{wikify section|date=May 2012}}
{{expand section}}
{{POV-section}}
}}
{{unreferenced section}}

==hello=="), "adds tags to existing MI, MI new style");
            
            Assert.AreEqual(@"==sec==
{{multiple issues|
{{wikify section|date=May 2012}}
{{expand section}}
{{POV-section}}
{{unreferenced section}}
}}
==hello==", parser.MultipleIssues(@"==sec==
{{unreferenced section}}{{multiple issues|
{{wikify section|date=May 2012}}
{{expand section}}
{{POV-section}}
}}

==hello=="), "adds tags to existing MI, MI new style, tags before MI");
            
            Assert.AreEqual(@"==sec==
{{multiple issues|wikify=May 2012|underlinked=May 2012|POV=May 2012|
{{unreferenced section}}
}}
==hello==", parser.MultipleIssues(@"==sec==
{{multiple issues|wikify=May 2012|underlinked=May 2012|POV=May 2012}}
{{unreferenced section}}

==hello=="), "adds tags to existing MI, MI old style");
            
            Assert.AreEqual(@"==sec==
{{multiple issues|
{{wikify section|date=May 2012}}
{{unreferenced section}}
}}
==hello==", parser.MultipleIssues(@"==sec==
{{multiple issues|
{{wikify section|date=May 2012}}
}}
{{unreferenced section}}

==hello=="), "adds 1 tag to existing MI with 1 tag, MI new style");
            
            Assert.AreEqual(@"==sec==
{{multiple issues|wikify section=May 2012|
{{unreferenced section}}
}}
==hello==", parser.MultipleIssues(@"==sec==
{{multiple issues|wikify section=May 2012}}
{{unreferenced section}}

==hello=="), "adds 1 tag to existing MI with 1 tag, MI old style");
            
            Assert.AreEqual(@"==sec==
{{multiple issues|
{{wikify section|date=May 2012}}
{{expand section}}
{{POV-section}}
{{unreferenced section}}
}}
==hello==", parser.MultipleIssues(@"==sec==
{{multiple issues|
{{wikify section|date=May 2012}}
{{expand section}}
{{POV-section}}
{{unreferenced section}}
}}
{{unreferenced section}}

==hello=="), "duplicate tags not added");
        }
        
        [Test]
        public void MultipleIssuesNewSectionSingleTag()
        {          
            Assert.AreEqual(@"==sec==
{{wikify section}}", parser.MultipleIssues(@"==sec==
{{multiple issues|
{{wikify section}}
}}"), "converts new style 1-tag MI to single template");
            Assert.AreEqual(@"==sec==
{{wikify section}}", parser.MultipleIssues(@"==sec==
{{multiple issues|section=yes|
{{wikify section}}
}}"), "converts new style 1-tag MI to single template");
            Assert.AreEqual(@"==sec==
{{wikify section|date=May 2012}}", parser.MultipleIssues(@"==sec==
{{multiple issues|
{{wikify section|date=May 2012}}
}}"), "converts new style 1-tag MI to single template, tag with date");
            Assert.AreEqual(@"==sec==
{{wikify section|reason=links}}", parser.MultipleIssues(@"==sec==
{{multiple issues|
{{wikify section|reason=links}}
}}"), "converts new style 1-tag MI to single template, tag with extra parameter");
            
            Assert.AreEqual(@"==sec==
{{wikify|date=May 2012}}", parser.MultipleIssues(@"==sec==
{{multiple issues|wikify=May 2012}}"), "converts old style 1-tag MI to single template");
        }

        [Test]
        public void MultipleIssuesNewCleanup()
        {
            
            Assert.AreEqual(@"{{multiple issues|
{{wikify}}
{{underlinked}}
{{POV}}
}}", parser.MultipleIssues(@"{{multiple issues|
{{wikify}}

{{underlinked}}
{{POV}}
}}"), "Cleans up excess newlines");
        }

        [Test]
        public void MultipleIssuesOldSectionSingleTag()
        {
            Assert.AreEqual(@"==sec==
{{cleanup|date=January 2008}} Article text here", parser.MultipleIssues(@"==sec==
{{multipleissues|cleanup=January 2008}} Article text here"));
            Assert.AreEqual(@"==sec==
{{cleanup|date=January 2008}} Article text here", parser.MultipleIssues(@"==sec==
{{multiple issues|cleanup=January 2008}} Article text here"));
            Assert.AreEqual(@"==sec==
{{cleanup|date=January 2008}} Article text here", parser.MultipleIssues(@"==sec==
{{multiple issues|cleanup=January 2008
}} Article text here"));
            Assert.AreEqual(@"==sec==
{{cleanup|date=January 2008}} Article text here", parser.MultipleIssues(@"==sec==
{{multipleissues|
            cleanup=January 2008}} Article text here"));
            Assert.AreEqual(@"==sec==
{{cleanup|date=January 2009}} Article text here", parser.MultipleIssues(@"==sec==
{{multipleissues|cleanup=January 2009}} Article text here"));
            Assert.AreEqual(@"==sec==
{{trivia|date=January 2008}} Article text here", parser.MultipleIssues(@"==sec==
{{multipleissues|trivia = January 2008}} Article text here"));
            Assert.AreEqual(@"==sec==
{{trivia|date=May 2010}} Article text here", parser.MultipleIssues(@"==sec==
{{multipleissues|trivia = May 2010}} Article text here"));
            Assert.AreEqual(@"==sec==
{{cleanup|date=January 2008}} Article text here", parser.MultipleIssues(@"==sec==
{{multipleissues|cleanup=January 2008|}} Article text here"));
            Assert.AreEqual(@"==sec==
{{cleanup|date=January 2008}} Article text here", parser.MultipleIssues(@"==sec==
{{multiple issues|cleanup=January 2008|}} Article text here"));
            Assert.AreEqual(@"==sec==
{{OR|date=January 2008}} Article text here", parser.MultipleIssues(@"==sec==
{{multipleissues|OR=January 2008}} Article text here"));

            // no changes
            string a = @"==sec==
{{multiple issues|trivia=January 2008|cleanup=January 2008}} Article text here";
            Assert.AreEqual(a, parser.MultipleIssues(a));

            a = @"==sec==
{{ARTICLEISSUES|cleanup=January 2008}} Article text here";
            Assert.AreEqual(a, parser.MultipleIssues(a));

            a = @"==sec==
{{multiple issues|cleanup=May 2007|trivia=January 2008}} Article text here";
            Assert.AreEqual(a, parser.MultipleIssues(a));

            a = @"==sec==
{{multiple issues|section=y|refimprove=February 2007|OR=December 2009}}  Article text here";
            Assert.AreEqual(a, parser.MultipleIssues(a));

            a = @"==sec==
{{multiple issues|section=yes|
{{essay|section|date = October 2011}}
{{unbalanced|section|date = October 2011}}
}}";
            Assert.AreEqual(a, parser.MultipleIssues(a));
        }
        
        [Test]
        public void MultipleIssuesOldTitleCase()
        {
            // title case parameters converted to lowercase
            Assert.AreEqual(@"{{article issues|POV=May 2008|cleanup=May 2008|expand=June 2007}}", parser.MultipleIssues(@"{{article issues|POV=May 2008|cleanup=May 2008|Expand=June 2007}}"));
            Assert.AreEqual(@"{{article issues|POV=May 2008|cleanup=May 2008|expand=June 2007}}", parser.MultipleIssues(@"{{article issues|POV=May 2008|cleanup=May 2008|Expand=June 2007}}"));
            Assert.AreEqual(@"{{article issues|POV=May 2008|cleanup=May 2008| expand = June 2007}}", parser.MultipleIssues(@"{{article issues|POV=May 2008|cleanup=May 2008| Expand = June 2007}}"));
            Assert.AreEqual(@"{{Articleissues|BLPunsourced=May 2008|cleanup=May 2008|expand=June 2007}}", parser.MultipleIssues(@"{{Articleissues|BLPunsourced=May 2008|Cleanup=May 2008|expand=June 2007}}"));
            Assert.AreEqual(@"{{article issues|POV=May 2008|cleanup=May 2008|unreferencedBLP=June 2007}}", parser.MultipleIssues(@"{{article issues|POV=May 2008|cleanup=May 2008|UnreferencedBLP=June 2007}}"));
        }
        
        [Test]
        public void MultipleIssuesOldDateField()
        {
            // don't remove date field where expert field is using it
            const string ESD = @"{{Article issues|cleanup=March 2008|expert=Anime and manga|refimprove=May 2008|date=February 2009}}";
            Assert.AreEqual(ESD, parser.MultipleIssues(ESD));

            // can remove date field when expert=Month YYYY
            Assert.AreEqual(@"{{multiple issues|cleanup=March 2008|expert=May 2008|refimprove=May 2008}}", parser.MultipleIssues(@"{{multiple issues|Cleanup=March 2008|expert=May 2008|refimprove=May 2008|date=February 2009}}"));

            // date field removed where no expert field to use it
            Assert.AreEqual(@"{{multiple issues|cleanup=March 2008|COI=March 2008|refimprove=May 2008}}", parser.MultipleIssues(@"{{multiple issues|Cleanup=March 2008|COI=March 2008|refimprove=May 2008|date=February 2009}}"));
            
            // removal of unneeded date field
            Assert.AreEqual(@"{{multiple issues|wikfy=May 2008|COI=May 2008|cleanup=May 2008}}", parser.MultipleIssues(@"{{multiple issues|wikfy=May 2008|COI=May 2008|cleanup=May 2008|date = March 2007}}"));
            Assert.AreEqual(@"{{multiple issues|wikfy=May 2008|COI=May 2008|cleanup=May 2008}}", parser.MultipleIssues(@"{{multiple issues|wikfy=May 2008|COI=May 2008|cleanup=May 2008| date=March 2007}}"));
            Assert.AreEqual(@"{{multiple issues|wikfy=May 2008|COI=May 2008|cleanup=May 2008}}", parser.MultipleIssues(@"{{multiple issues|wikfy=May 2008|COI=May 2008|date = March 2007|cleanup=May 2008}}"));

            Assert.AreEqual(@"{{Multiple issues|wikfy=May 2008|COI=May 2008|cleanup=May 2008}}", parser.MultipleIssues(@"{{Multiple issues|wikfy=May 2008|COI=May 2008|date = March 2007|cleanup=May 2008}}"));
        }
    }
}
