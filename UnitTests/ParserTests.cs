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
