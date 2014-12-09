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
            Assert.AreEqual(Found, Parsers.BadCiteParameters(@"now {{cite web|url=bar|date=2009|contribution=Smith}} was"));
            Assert.AreEqual(Found, Parsers.BadCiteParameters(@"{{cite web | periodical=, |journal=, |newspaper=, |magazine= |work= |website= |encyclopedia= |encyclopaedia= |dictionary=
|contribution=, |entry=, |article=, |section=
|publicationplace=, |publication-place=
|date=, |year=, |publicationdate=, |publication-date=
|series= |volume= |issue=, |number= |page=, |pages=, |at=
|edition= |publisher=, |institution=
|chapterurl=, |chapter-url=, |contributionurl=, |contribution-url=, |sectionurl= }}"));

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
        }

        [Test]
        public void AddMissingReflistEnOnly()
        {
#if DEBUG
            const string SingleRef = @"now <ref>foo</ref>", Cat = @"
[[Category:Here]]";
            Assert.AreEqual(SingleRef + "\r\n\r\n" + @"==References==
{{Reflist}}" + Cat, Parsers.AddMissingReflist(SingleRef + Cat));

            Variables.SetProjectLangCode("fr");
            Assert.AreEqual(SingleRef + Cat, Parsers.AddMissingReflist(SingleRef + Cat));

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
    public class LinkTests : RequiresParser
    {
        public GenfixesTestsBase genFixes  = new GenfixesTestsBase();

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

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_1#Link_de-piping_false_positive
            Assert.AreEqual("[[Sacramento, California|Sacramento]], California's [[capital city]]",
                            Parsers.StickyLinks("[[Sacramento, California|Sacramento]], California's [[capital city]]"));

            Assert.AreEqual("[[Russian literature|Russian literature]] was", Parsers.StickyLinks("[[Russian literature|Russian literature]] was"), "bugfix – no exception when pipe same length as target");
        }

        [Test]
        public void TestSimplifyLinks()
        {
            Assert.AreEqual("[[dog]]s", Parsers.SimplifyLinks("[[dog|dogs]]"));

            // case insensitivity of the first char
            Assert.AreEqual("[[dog]]s", Parsers.SimplifyLinks("[[Dog|dogs]]"));
            Assert.AreEqual("[[Dog]]s", Parsers.SimplifyLinks("[[dog|Dogs]]"));

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_8#Wrong_link_simplification_capitalisation
            Assert.AreEqual("[[dog]]", Parsers.SimplifyLinks("[[Dog|dog]]"));
            Assert.AreEqual("[[Dog]]", Parsers.SimplifyLinks("[[dog|Dog]]"));
            Assert.AreEqual("[[Dog]]", Parsers.SimplifyLinks("[[Dog|Dog]]"));

            Assert.AreEqual("[[dog]]s", Parsers.SimplifyLinks("[[Dog|dogs]]"));
            Assert.AreEqual("[[Dog]]s", Parsers.SimplifyLinks("[[dog|Dogs]]"));
            Assert.AreEqual("[[Dog]]s", Parsers.SimplifyLinks("[[Dog|Dogs]]"));
            Assert.AreEqual("#REDIRECT[[Dog]]", Parsers.SimplifyLinks("#REDIRECT[[dog|Dog]]"));
            Assert.AreEqual("[[funcy dog]]s", Parsers.SimplifyLinks("[[funcy dog|funcy dogs]]"));

            Assert.AreEqual("[[funcy dog]]", Parsers.SimplifyLinks("[[funcy dog|funcy_dog]]"), "handles underscore in text: text");
            Assert.AreEqual("[[funcy dog]]", Parsers.SimplifyLinks("[[funcy_dog|funcy dog]]"), "handles underscore in text: target");
            Assert.AreEqual("[[Funcy dog]]", Parsers.SimplifyLinks("[[funcy_dog|Funcy dog]]"), "handles underscore in text: target");

            Variables.UnderscoredTitles.Add("Funcy dog");
            Assert.AreEqual("[[funcy dog|funcy_dog]]", Parsers.SimplifyLinks("[[funcy dog|funcy_dog]]"), "handles underscore in text where article with underscore in title");
            Variables.UnderscoredTitles.Remove("Funcy dog");

            // ...and sensitivity of others
            Assert.AreEqual("[[dog|dOgs]]", Parsers.SimplifyLinks("[[dog|dOgs]]"));

            //https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_2#Inappropriate_link_compression
            Assert.AreEqual("[[foo|foo3]]", Parsers.SimplifyLinks("[[foo|foo3]]"));

            // don't touch suffixes with caps to avoid funky results like
            // https://en.wikipedia.org/w/index.php?diff=195760456
            Assert.AreEqual("[[FOO|FOOBAR]]", Parsers.SimplifyLinks("[[FOO|FOOBAR]]"));
            Assert.AreEqual("[[foo|fooBAR]]", Parsers.SimplifyLinks("[[foo|fooBAR]]"));

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_8#Only_one_spurious_space_removed_from_link
            Assert.AreEqual("[[Elizabeth Gunn]]", Parsers.SimplifyLinks("[[Elizabeth Gunn | Elizabeth Gunn]]"));
            Assert.AreEqual("[[Big Bend, Texas|Big Bend]]", Parsers.SimplifyLinks("[[Big Bend, Texas | Big Bend]]"));

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_8#SVN:_general_fixes_removes_whitespace_around_pipes_within_citation_templates
            Assert.AreEqual("{{foo|[[bar]] | boz}}]]", Parsers.SimplifyLinks("{{foo|[[bar]] | boz}}]]"));
            Assert.AreEqual("{{foo|[[bar]]\r\n| boz}}]]", Parsers.SimplifyLinks("{{foo|[[bar]]\r\n| boz}}]]"));

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_9#General_fixes_remove_spaces_from_category_sortkeys
            Assert.AreEqual("[[foo|bar]]", Parsers.SimplifyLinks("[[foo| bar]]"));
            Assert.AreEqual("[[foo|bar]]", Parsers.SimplifyLinks("[[foo| bar ]]"));
            Assert.AreEqual("[[Category:foo| bar]]", Parsers.SimplifyLinks("[[Category:foo| bar]]"));
            Assert.AreEqual("[[Category:foo| bar]]", Parsers.SimplifyLinks("[[Category:foo| bar ]]"));

            // nothing to do here
            Assert.AreEqual("[[dog|]]", Parsers.SimplifyLinks("[[dog|]]"));
        }

        [Test]
        public void FixDatesHTML()
        {
            // replace <br> and <p> HTML tags tests
            Assert.AreEqual("\r\n\r\nsome text", parser.FixBrParagraphs("<p>some text"));
            Assert.AreEqual("\r\n\r\n[[some text|bar]]", parser.FixBrParagraphs("<p>[[some text|bar]]"));
            Assert.AreEqual("\r\n\r\nsome text\r\n\r\n", parser.FixBrParagraphs("<p>some text</p>"));
            Assert.AreEqual("\r\n\r\nsome text", parser.FixBrParagraphs("<p> some text"));
            Assert.AreEqual("\r\n\r\nsome text", parser.FixBrParagraphs("<br><br>some text"));
            Assert.AreEqual("some text\r\n\r\n", parser.FixBrParagraphs("some text<p>"));
            Assert.AreEqual("some text\r\n\r\n", parser.FixBrParagraphs("some text<br><br>"));
            Assert.AreEqual("some text\r\n\r\nword", parser.FixBrParagraphs("some text<br><br>word"));

            // don't match when in table or blockquote
            Assert.AreEqual("|<p>some text", parser.FixBrParagraphs("|<p>some text"));
            Assert.AreEqual("|<br><br>some text", parser.FixBrParagraphs("|<br><br>some text"));
            Assert.AreEqual("!<p>some text", parser.FixBrParagraphs("!<p>some text"));
            Assert.AreEqual("!<br><br>some text", parser.FixBrParagraphs("!<br><br>some text"));

            genFixes.AssertNotChanged(@"<blockquote><p>some text</blockquote>");
            genFixes.AssertNotChanged("<blockquote>|<br><br>some text</blockquote>");

            Assert.AreEqual(@"* {{Polish2|Krzepice (województwo dolnośląskie)|[[24 November]] [[2007]]}}

", parser.FixBrParagraphs(@"* {{Polish2|Krzepice (województwo dolnośląskie)|[[24 November]] [[2007]]}}<br><br>  "));

            const string nochange = @"{{cite web | title = Hello April 14 2009 there | date=2011-11-13 }}";

            Assert.AreEqual(nochange, parser.FixBrParagraphs(nochange));
        }

        [Test]
        public void FixDatesCommaErrors()
        {
            const string correct1 = @"Retrieved on April 14, 2009 was";
            Assert.AreEqual(correct1, parser.FixDatesA(@"Retrieved on April, 14, 2009 was"));
            Assert.AreEqual(correct1, parser.FixDatesA(@"Retrieved on April , 14 , 2009 was"));
            Assert.AreEqual(correct1, parser.FixDatesA(@"Retrieved on April , 14 ,2009 was"));

            Assert.AreEqual(correct1, parser.FixDatesA(@"Retrieved on April 14,2009 was"));
            Assert.AreEqual(correct1, parser.FixDatesA(@"Retrieved on April 14 ,2009 was"));
            Assert.AreEqual(correct1, parser.FixDatesA(@"Retrieved on April 14 2009 was"));
            Assert.AreEqual(correct1, parser.FixDatesA(@"Retrieved on April14,2009 was"));
            Assert.AreEqual(correct1, parser.FixDatesA(@"Retrieved on April14, 2009 was"));
            Assert.AreEqual(correct1, parser.FixDatesA(@"Retrieved on April14 2009 was"));
            Assert.AreEqual(correct1, parser.FixDatesA(correct1));

            // don't change image names
            string image1 = @"now foo [[Image:Foo July 24 2009.png]] was";
            Assert.AreEqual(image1, parser.FixDatesA(image1));
            string image2 = @"now foo [[File:Foo July 24 2009.png]] was";
            Assert.AreEqual(image2, parser.FixDatesA(image2));

            const string correct2 = @"Retrieved on 14 April 2009 was";
            Assert.AreEqual(correct2, parser.FixDatesA(@"Retrieved on 14 April, 2009 was"));
            Assert.AreEqual(correct2, parser.FixDatesA(@"Retrieved on 14 April , 2009 was"));
            Assert.AreEqual(correct2, parser.FixDatesA(@"Retrieved on 14 April,  2009 was"));

            const string nochange1 = @"On 14 April, 2590 people", nochange2 = @"Retrieved on April 142009 was";
            Assert.AreEqual(nochange1, parser.FixDatesA(nochange1));
            Assert.AreEqual(nochange2, parser.FixDatesA(nochange2));

            Assert.AreEqual(@"#####
'''A''' (1 December 1920 &ndash; 28 May 2013)", parser.FixDatesA(@"#####
'''A''' (1 December 1920 &ndash; 28 May, 2013)"));
        }
        
        [Test]
        public void FixDatesEnOnly()
        {
            #if DEBUG
            Variables.SetProjectLangCode("fr");
            Assert.AreEqual(@"Retrieved on April, 14, 2009 was", parser.FixDatesA(@"Retrieved on April, 14, 2009 was"));
            Assert.AreEqual(@"from (1900-1933) there", parser.FixDatesB(@"from (1900-1933) there", false, false));

            Variables.SetProjectLangCode("en");
            const string correct1 = @"Retrieved on April 14, 2009 was";
            Assert.AreEqual(correct1, parser.FixDatesA(@"Retrieved on April, 14, 2009 was"));
            const string correct = @"from (1900–1933) there";
            Assert.AreEqual(correct, parser.FixDatesB(@"from (1900-1933) there", false, false));
            #endif
        }

        [Test]
        public void FixDatesRanges()
        {
            const string correct1 = @"On 3–17 May 2009 a dog";
            Assert.AreEqual(correct1, parser.FixDatesA(@"On 3-17 May 2009 a dog"));
            Assert.AreEqual(correct1, parser.FixDatesA(@"On 3 - 17 May 2009 a dog"));
            Assert.AreEqual(correct1, parser.FixDatesA(@"On 3 -17 May 2009 a dog"));
            Assert.AreEqual(correct1, parser.FixDatesA(@"On 3 May-17 May 2009 a dog"));
            Assert.AreEqual(correct1, parser.FixDatesA(@"On 3 May - 17 May 2009 a dog"));
            Assert.AreEqual(correct1, parser.FixDatesA(@"On 3 May – 17 May 2009 a dog"));
            Assert.AreEqual(correct1, parser.FixDatesA(@"On 3 May – 17 May, 2009 a dog"));

            // American format
            const string correct2 = @"On May 3–17, 2009 a dog";
            Assert.AreEqual(correct2, parser.FixDatesA(@"On May 3-17, 2009 a dog"));
            Assert.AreEqual(correct2, parser.FixDatesA(@"On May 3-17 2009 a dog"));
            Assert.AreEqual(correct2, parser.FixDatesA(@"On May 3 - 17 2009 a dog"));
            Assert.AreEqual(correct2, parser.FixDatesA(@"On May 3   -17 2009 a dog"));
            Assert.AreEqual(correct2, parser.FixDatesA(@"On May 3 - May 17 2009 a dog"));
            Assert.AreEqual(correct2, parser.FixDatesA(@"On May 3 - May 17, 2009 a dog"));
            Assert.AreEqual(correct2, parser.FixDatesA(@"On May 3 – May 17 2009 a dog"));
            Assert.AreEqual(correct2, parser.FixDatesA(@"On May 3 – May 17, 2009 a dog"));

            // no change
            const string nochange1 = @"May 17 - 13,009 dogs";
            Assert.AreEqual(nochange1, parser.FixDatesA(nochange1));

            const string nochange2 = @"May 2-4-0";
            Assert.AreEqual(nochange2, parser.FixDatesA(nochange2));

            // month ranges
            const string correct3 = @"May–June 2010";
            Assert.AreEqual(correct3, parser.FixDatesA(@"May-June 2010"), "endash set for month range");
            Assert.AreEqual(correct3, parser.FixDatesA(correct3));

            Assert.AreEqual("from 1904 – 11 May 1956 there", parser.FixDatesA("from 1904 – 11 May 1956 there"));

            // DateRangeToYear: result is spaced endash
            const string DateToYear = @"'''Nowell''' (May 16, 1872 - 1940), was";
            genFixes.AssertChange(DateToYear, DateToYear.Replace("-", "–"));
            genFixes.AssertChange(DateToYear.Replace(" - ", "-"), DateToYear.Replace("-", "–"));
            const string DateToYear2 = @"'''Nowell''' (16 May 1872–1940), was";
            genFixes.AssertChange(DateToYear2, DateToYear2.Replace("–", " – "));
            genFixes.AssertChange(DateToYear2.Replace("–", "-"), DateToYear2.Replace("–", " – "));

            genFixes.AssertNotChanged(@"Volume 1, 2001–2004 was");
        }

        [Test]
        public void TestFullYearRanges()
        {
            const string correct = @"from (1900–1933) there";
            Assert.AreEqual(correct, parser.FixDatesB(@"from (1900-1933) there", false, false));
            Assert.AreEqual(correct, parser.FixDatesB(@"from (1900  –  1933) there", false, false));
            Assert.AreEqual(correct, parser.FixDatesB(@"from (1900 -1933) there", false, false));
            Assert.AreEqual(correct, parser.FixDatesB(@"from (1900 - 1933) there", false, false));
            Assert.AreEqual(correct, parser.FixDatesB(@"from (1900 -  1933) there", false, false));
            Assert.AreEqual(@"from (1900–1901) there", parser.FixDatesB(@"from (1900 - 1901) there", false, false));
            Assert.AreEqual(@"from (1900–1933, 2000) there", parser.FixDatesB(@"from (1900-1933, 2000) there", false, false));
            Assert.AreEqual(@"from (1900–1933, 2000–2002) there", parser.FixDatesB(@"from (1900-1933, 2000-2002) there", false, false));
            Assert.AreEqual(@"from ( 1900–1933) there", parser.FixDatesB(@"from ( 1900-1933) there", false, false));

            Assert.AreEqual(@"from 1950–1960,", parser.FixDatesB(@"from 1950-1960,", false, false));
            Assert.AreEqual(@"|1950–1960|", parser.FixDatesB(@"|1950-1960|", false, false));
            Assert.AreEqual(@"(1950–1960 and 1963–1968)", parser.FixDatesB(@"(1950-1960 and 1963-1968)", false, false));
            Assert.AreEqual(@"or 1900–1901,", parser.FixDatesB(@"or 1900 - 1901,", false, false));
            Assert.AreEqual(@"for 1900–1901,", parser.FixDatesB(@"for 1900 - 1901,", false, false));

            // no change – not valid date range
            genFixes.AssertNotChanged(@"from (1900–1870) there");

            // already okay
            genFixes.AssertNotChanged(@"from (1900&ndash;1933) there");
            genFixes.AssertNotChanged(@"from (1900–1933) there");

            Assert.AreEqual(@"now between 1900–1920
was", parser.FixDatesB(@"now between 1900-1920
was", false, false));

            string BadRange = @"from 1950-1920,";
            Assert.AreEqual(BadRange, parser.FixDatesB(BadRange, false, false));
            BadRange = @"from 1950 – 1920,";
            Assert.AreEqual(BadRange, parser.FixDatesB(BadRange, false, false));
            BadRange = @"from 1980-70,";
            Assert.AreEqual(BadRange, parser.FixDatesB(BadRange, false, false));

            // full date ranges
            Assert.AreEqual(@"over 1 April 2004 – 5 July 2009.", parser.FixDatesB(@"over 1 April 2004-5 July 2009.", false, false));
            Assert.AreEqual(@"over April 1, 2004 – July 5, 2009.", parser.FixDatesB(@"over April 1, 2004–July 5, 2009.", false, false));
        }

        [Test]
        public void CircaYearRanges()
        {
            Assert.AreEqual(@"c. 1950 – 1960,", parser.FixDatesB(@"c. 1950 - 1960,", false, false));
            Assert.AreEqual(@"ca. 1950 – 1960,", parser.FixDatesB(@"ca. 1950 - 1960,", false, false));
            Assert.AreEqual(@"circa 1950 – 1960,", parser.FixDatesB(@"circa 1950 - 1960,", false, false));

            // no changes because can't use hidemore and detect the links
            genFixes.AssertNotChanged(@"{{Circa}} 1950 – 1960,");
            genFixes.AssertNotChanged(@"[[c.]] 1950 - 1960,");
            genFixes.AssertNotChanged(@"[[c]]. 1950 - 1960,");
            genFixes.AssertNotChanged(@"[[c]] 1950 - 1960,");
            genFixes.AssertNotChanged(@"[[circa]] 1950 - 1960,");
            genFixes.AssertNotChanged(@"[[circa|c.]] 1950 - 1960,");
            genFixes.AssertNotChanged(@"[[circa|c]]. 1950 - 1960,");
            genFixes.AssertNotChanged(@"{{circa}} 1950 - 1960,");
            genFixes.AssertNotChanged(@"{{Circa}} 1950 - 1960,");
            genFixes.AssertNotChanged(@"circle 1950 - 1960,");
            genFixes.AssertNotChanged(@"[[Foo (1950-1960)|Foo]]");
            genFixes.AssertNotChanged(@"[[Foo (1950 - 1960)|Foo]]");
        }

        [Test]
        public void TestYearToPresentRanges()
        {
            const string present = @"from 2002–present was";
            Assert.AreEqual(present, parser.FixDatesB(@"from 2002-present was", false, false));
            Assert.AreEqual(present, parser.FixDatesB(@"from 2002 -   present was", false, false));
            Assert.AreEqual(present, parser.FixDatesB(@"from 2002–present was", false, false));

            Assert.AreEqual(@"from 2002–Present was", parser.FixDatesB(@"from 2002-Present was", false, false));

            const string present2 = @"== Members ==
* [[Nick Hexum]] - Vocals, [[Rhythm Guitar]], Programming (1989 - present)
* [[S. A. Martinez|Doug Martinez]] - Vocals, [[Phonograph|Turntables]], DJ (1992 - present)
* [[Tim Mahoney (guitarist)|Tim Mahoney]] - [[Lead Guitar]] (1991 - present)
* [[P-Nut|Aaron Wills]] - [[Bass guitar]] (1989 - present)
* [[Chad Sexton]] - [[Drum]]s, Programming, Percussion (1989 - present)";

            Assert.AreEqual(present2.Replace(@" - p", @"–p"), parser.FixDatesB(present2, false, false));

            genFixes.AssertNotChanged(@"* 2000 - presented");
        }

        [Test]
        public void TestDateToPresentRanges()
        {
            Assert.AreEqual(@"from May 2002 – present was", parser.FixDatesB(@"from May 2002 - present was", false, false));
            Assert.AreEqual(@"from May 2002 – present was", parser.FixDatesB(@"from May 2002-present was", false, false));
            Assert.AreEqual(@"from May 11, 2002 – present was", parser.FixDatesB(@"from May 11, 2002-present was", false, false));
            Assert.AreEqual(@"from May 11, 2002 – present was", parser.FixDatesB(@"from May 11, 2002 - present was", false, false));
        }

        [Test]
        public void TestShortenedYearRanges()
        {
            const string correct = @"from (1900–33) there";
            Assert.AreEqual(correct, parser.FixDatesB(@"from (1900-33) there", false, false));
            Assert.AreEqual(correct, parser.FixDatesB(@"from (1900 -33) there", false, false));
            Assert.AreEqual(correct, parser.FixDatesB(@"from (1900 - 33) there", false, false));
            Assert.AreEqual(correct, parser.FixDatesB(@"from (1900 -  33) there", false, false));
            Assert.AreEqual(@"from (1900–1901) there", parser.FixDatesB(@"from (1900 - 1901) there", false, false));
            Assert.AreEqual(@"from (1900–33, 2000) there", parser.FixDatesB(@"from (1900-33, 2000) there", false, false));
            Assert.AreEqual(@"from (1900–33, 2000–2002) there", parser.FixDatesB(@"from (1900-33, 2000-2002) there", false, false));
            Assert.AreEqual(@"from ( 1900–33) there", parser.FixDatesB(@"from ( 1900-33) there", false, false));

            Assert.AreEqual(@"from 1950–60,", parser.FixDatesB(@"from 1950-60,", false, false));
            Assert.AreEqual(@"(1950–60 and 1963–68)", parser.FixDatesB(@"(1950-60 and 1963-68)", false, false));

            // no change – not valid date range
            genFixes.AssertNotChanged(@"from (1920–18) there");

            // already okay
            genFixes.AssertNotChanged(@"from (1900&ndash;33) there");
            genFixes.AssertNotChanged(@"from (1900–33) there");
        }

        [Test]
        public void TestYearRangesCategories()
        {
            genFixes.AssertNotChanged(@"now foo
[[Category:Abc (2004-present)]]");
            
            genFixes.AssertChange(@"now abc (2004-present) was
now foo
[[Category:Abc (2004-present)]]", @"now abc (2004–present) was
now foo
[[Category:Abc (2004-present)]]");
        }

        [Test]
        public void FixLivingThingsRelatedDates()
        {
            Assert.AreEqual("test text", Parsers.FixLivingThingsRelatedDates("test text"));
            Assert.AreEqual("'''John Doe''' (born [[21 February]] [[2008]])", Parsers.FixLivingThingsRelatedDates("'''John Doe''' (b. [[21 February]] [[2008]])"), "b. expanded");
            Assert.AreEqual(@"'''John O'Doe''' (born [[21 February]] [[2008]])", Parsers.FixLivingThingsRelatedDates(@"'''John O'Doe''' (b. [[21 February]] [[2008]])"), "b. expanded, name has apostrophe");
            Assert.AreEqual("'''John Doe''' (born 21 February 2008)", Parsers.FixLivingThingsRelatedDates("'''John Doe''' (b. 21 February 2008)"), "non-wikilinked dates supported");
            Assert.AreEqual("'''John Doe''' (born [[21 February]] [[2008]])", Parsers.FixLivingThingsRelatedDates("'''John Doe''' ([[21 February]] [[2008]]–)"), "dash for born expanded");
            Assert.AreEqual("'''John O'Doe''' (born [[21 February]] [[2008]])", Parsers.FixLivingThingsRelatedDates("'''John O'Doe''' ([[21 February]] [[2008]]–)"), "dash for born expanded, name has apostrophe");
            Assert.AreEqual("'''John Doe''' (born [[21 February]] [[2008]])", Parsers.FixLivingThingsRelatedDates("'''John Doe''' ([[21 February]] [[2008]] &ndash;)"), "dash for born expanded");
            Assert.AreEqual("'''John Doe''' (born [[21 February]] [[2008]])", Parsers.FixLivingThingsRelatedDates("'''John Doe''' (born: [[21 February]] [[2008]])"), "born: tidied");
            Assert.AreEqual("'''John Doe''' (born [[21 February]] [[2008]])", Parsers.FixLivingThingsRelatedDates("'''John Doe''' (Born: [[21 February]] [[2008]])"), "born: tidied");
            Assert.AreEqual("'''John Doe''' (born March 6, 2008)", Parsers.FixLivingThingsRelatedDates("'''John Doe''' (b. March 6, 2008)"), "b. expanded");
            Assert.AreEqual("'''John Doe''' (born 2008)", Parsers.FixLivingThingsRelatedDates("'''John Doe''' (b. 2008)"), "b. expanded");
            Assert.AreEqual("'''John Doe''' (born 2008)", Parsers.FixLivingThingsRelatedDates("'''John Doe''' (b.2008)"), "b. expanded");
            Assert.AreEqual("'''John Doe''' (born 2008)", Parsers.FixLivingThingsRelatedDates("'''John Doe''' (born on 2008)"), "born on fixed");
            Assert.AreEqual("'''John Doe''' (born March 6, 2008, London)", Parsers.FixLivingThingsRelatedDates("'''John Doe''' (b. March 6, 2008, London)"), "b. expanded");
            Assert.AreEqual("'''John Doe''' (born March 6, 2008 in London)", Parsers.FixLivingThingsRelatedDates("'''John Doe''' (b. March 6, 2008 in London)"), "b. expanded");
            Assert.AreEqual("'''John Doe''' (born March 6, 2008, London)", Parsers.FixLivingThingsRelatedDates("'''John Doe''' (born on March 6, 2008, London)"), "born on fixed");

            Assert.AreEqual("'''John Doe''' (died [[21 February]] [[2008]])", Parsers.FixLivingThingsRelatedDates("'''John Doe''' (d. [[21 February]] [[2008]])"));
            Assert.AreEqual("'''John Doe''' (died [[21 February]] [[2008]])", Parsers.FixLivingThingsRelatedDates("'''John Doe''' (d.[[21 February]] [[2008]])"));
            Assert.AreEqual("'''John Doe''' (died 2008)", Parsers.FixLivingThingsRelatedDates("'''John Doe''' (d.2008)"));
            Assert.AreEqual("'''John O'Doe''' (died [[21 February]] [[2008]])", Parsers.FixLivingThingsRelatedDates("'''John O'Doe''' (d. [[21 February]] [[2008]])"));
            Assert.AreEqual("'''Willa Klug Baum''' ([[October 4]], [[1926]] – May 18, 2006)", Parsers.FixLivingThingsRelatedDates("'''Willa Klug Baum''' (born [[October 4]], [[1926]], died May 18, 2006)"));
            Assert.AreEqual("'''Willa Klug Baum''' (1926 – May 18, 2006)", Parsers.FixLivingThingsRelatedDates("'''Willa Klug Baum''' (b.1926, died May 18, 2006)"));

            const string Nochange = @"{{Infobox baseball team|
  FormerNames= '''[[Nishi-Nippon Railroad|Nishitetsu]] Clippers''' (1950)<br>'''Nishitetsu Lions''' (1951-1972)<br>'''Taiheiyo Club Lions''' (1973–1976)<br>'''Crown Lighter Lions''' (1977–1978)<br>'''Seibu Lions''' (1979–2007)<br>'''Saitama Seibu Lions''' (2008–)|
  foo=bar }}";

            Assert.AreEqual(Nochange, Parsers.FixLivingThingsRelatedDates(Nochange));

            const string Nochange2 = @"*'''[[Luís Godinho Lopes|Luís Filipe Fernandes David Godinho Lopes]]''' (2011–)";

            Assert.AreEqual(Nochange2, Parsers.FixLivingThingsRelatedDates(Nochange2));

            const string Nochange3 = @"** '''1st''' Andy Brown '''52''' (2008-)";

            Assert.AreEqual(Nochange3, Parsers.FixLivingThingsRelatedDates(Nochange3));

            const string Nochange4 = @"# '''[[Luís Godinho Lopes|Luís Filipe Fernandes David Godinho Lopes]]''' (2011–)";

            Assert.AreEqual(Nochange4, Parsers.FixLivingThingsRelatedDates(Nochange4));
        }

        [Test]
        public void UnlinkedFloruit()
        {
            const string LinkedFloruit = @"'''Foo''' ([[floruit|fl.]] 550) was a peasant.

==Time==
Foo was happy";
            genFixes.AssertNotChanged(LinkedFloruit);

            genFixes.AssertChange(@"'''Foo''' (fl. 550) was a peasant.

==Time==
Foo was happy", LinkedFloruit); // lowercase
             genFixes.AssertChange(@"'''Foo''' (fl 550) was a peasant.

==Time==
Foo was happy", LinkedFloruit); // no dot
            genFixes.AssertChange(@"'''Foo''' ( fl. 550) was a peasant.

==Time==
Foo was happy", LinkedFloruit); // extra whitespace
            genFixes.AssertChange(@"'''Foo''' (Fl. 550) was a peasant.

==Time==
Foo was happy", LinkedFloruit); // title case

            const string Floruit550 = @"'''Foo''' ([[floruit|fl.]] 550) was a peasant.
Foo was happy
Other (fl. 1645) was also";
            
            genFixes.AssertNotChanged(Floruit550); // No change when first floruit already linked

            genFixes.AssertChange(@"'''Foo''' (fl. 550) was a peasant.
Foo was happy
Other (fl. 1645) was also", Floruit550); // only first floruit linked

            const string FloruitLaterSection = @"'''Foo''' was a peasant.

==Other==
Other (fl. 1645) was also", FloruitTwice = @"'''Foo''' (fl. 55) was a peasant, related to other (fl. 600)";

            genFixes.AssertNotChanged(FloruitLaterSection); // not linked outside zeroth section

            genFixes.AssertChange(FloruitTwice, @"'''Foo''' ([[floruit|fl.]] 55) was a peasant, related to other (fl. 600)"); // only first occurrence linked

            genFixes.AssertNotChanged(@"{{cite encyclopedia|encyclopedia=ODNB|url=http://www.oxforddnb.com/view/olddnb/21073 |title=Packe, Christopher (fl. 1796)}}"); // not linked within template text
        }

        [Test]
        public void FixPeopleCategoriesTests()
        {
            // must be about a person
            const string a0 = @"'''Fred Smith''' (born 1960) is a bloke.";
            Assert.AreEqual(a0, Parsers.FixPeopleCategories(a0, "foo"));

            const string bug1 = @"{{BLP unsourced|date=March 2010}}

'''Z''' (born 2 January{{Year needed|date=January 2010|reason=Fix this date immediately or remove it; it look}} in .";

            Assert.AreEqual(bug1, Parsers.FixPeopleCategories(bug1, "Foo"));
        }

        [Test]
        public void FixPeopleCategoriesBirth()
        {
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

            const string b12 = @"{{Infobox actor
| name            = Gianni Capaldi
| birthdate       = <!-- {{Birth date and age|YYYY|MM|DD}} --> age 34 (as of 12 January 2010)<ref>*  Matheson, Shelley.</ref>
| birthplace      = [[Motherwell]], [[Scotland]]<br>{{Citation needed|date=September 2010}}
}}";
            Assert.AreEqual(b12, Parsers.FixPeopleCategories(b12, "foo"));
        }

        [Test]
        public void FixPeopleCategoriesUncat()
        {
            const string a1 = @"'''Fred Smith''' (born 1960) is a bloke. {{Persondata}} {{Uncat|date=May 2010}}";
            const string b2 = @"[[Category:1960 births]]";
            Assert.AreEqual(a1.Replace("Uncat", "Improve categories") + "\r\n" + b2, Parsers.FixPeopleCategories(a1, "foo"), "renames uncat to cat improve when cats added");

            Assert.AreEqual(a1 + b2, Parsers.FixPeopleCategories(a1 + b2, "foo"), "no uncat renaming when cats not added");
            Assert.AreEqual(a1.Replace(@" {{Uncat|date=May 2010}}", "") + "{{Cat improve}}" + "\r\n" + b2, Parsers.FixPeopleCategories(a1.Replace(@" {{Uncat|date=May 2010}}", "") + "{{Cat improve}}", "foo"), "no Cat improve change when cat improve already there");
        }

        [Test]
        public void FixPeopleCategoriesDeath()
        {
            const string b2 = @"[[Category:1960 births]]";

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

            const string d5 = @"Some words {{death date and age|1960|01|9}}";
            Assert.AreEqual(d5 + @"[[Category:1960 deaths]]", Parsers.FixPeopleCategories(d4 + @"[[Category:Year of death missing]]", "foo"));
        }

        [Test]
        public void FixPeopleCategoriesAlreadyCategorized()
        {
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

            Assert.AreEqual(a, Parsers.FixPeopleCategories(a, "foo"));
            Assert.AreEqual(b, Parsers.FixPeopleCategories(b, "foo"));
            Assert.AreEqual(c, Parsers.FixPeopleCategories(c, "foo"));
            Assert.AreEqual(e, Parsers.FixPeopleCategories(e, "foo"));
            Assert.AreEqual(f, Parsers.FixPeopleCategories(f, "foo"));
            Assert.AreEqual(g, Parsers.FixPeopleCategories(g, "foo"));
        }

        [Test]
        public void FixPeopleCategoriesYOBUncertain()
        {
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

            const string n4a = @"'''John Hulme''' (born c.1970) is [[Category:Living people]]
[[Category:Year of birth missing (living people)]]";

            Assert.AreEqual(n4a, Parsers.FixPeopleCategories(n4a, "foo"));

            const string n4b = @"'''John Hulme''' (born C.1970) is [[Category:Living people]]
[[Category:Year of birth missing (living people)]]";

            Assert.AreEqual(n4b, Parsers.FixPeopleCategories(n4b, "foo"));
        }

        [Test]
        public void FixPeopleCategoriesLimits()
        {
            const string b2 = @"[[Category:1960 births]]";
            const string u = "\r\n" + @"[[Category:Year of birth uncertain]]";
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

            const string bd3 = @"'''Foo''' (8 May 1920 somewhere &ndash;11 June 2004) was {{persondata}}";
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

            const string bd9b = @"'''Mannalargenna''' (c 1770-1835), a [[Tasmanian Aborigine]], was the chief of the Ben Lomond tribe (Plangermaireener). {{persondata}}";

            Assert.AreEqual(bd9b + @"
[[Category:Year of birth uncertain]]
[[Category:1835 deaths]]", Parsers.FixPeopleCategories(bd9b, "foo"));

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

            const string bd14c = @"'''Asclepigenia''' ({{fl}} 430) was {{persondata}}";
            Assert.AreEqual(bd14c, Parsers.FixPeopleCategories(bd14c, "foo"));

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
        }

        [Test]
        public void YearOfBirthMissingCategoryEnOnly()
        {
#if DEBUG
            const string good = @"[[Category:Pakistani lawyers]]
[[Category:Attorneys General of Pakistan]]
[[Category:Living people]]
[[Category:1944 births]]", bad = @"[[Category:Pakistani lawyers]]
[[Category:Attorneys General of Pakistan]]
[[Category:Year of birth missing (living people)]]
[[Category:Living people]]
[[Category:1944 births]]";

            Variables.SetProjectLangCode("fr");
            Assert.AreEqual(bad, Parsers.FixPeopleCategories(bad, "foo"));

            Variables.SetProjectLangCode("en");
            Assert.AreEqual(good, Parsers.FixPeopleCategories(bad, "foo"));
#endif
        }

        [Test]
        public void FixPeopleCategoriesFromInfobox()
        {
            const string a1 = @"'''Fred Smith''' (born 1960) is a bloke. {{Persondata}}";
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

}} {{persondata}}", infob1b = @"{{Infobox Officeholder
|honorific-prefix   =
|name            = John C. Zimmerman, Sr.
|term_start       = 1895
|term_end         = 1896
|predecessor      = [[Arthur C. McCall]]
|successor        = [[Samuel C. Randall]]
|DateOfBirth     = May 12, 1835
|birth_place     = [[Free City of Frankfurt]]
|DateOfDeath= October 26, 1935
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

            Assert.AreEqual(infob1b + @"
[[Category:1835 births]]
[[Category:1935 deaths]]", Parsers.FixPeopleCategories(infob1b, "foo"));

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
        public void FixPeopleCategoriesRefs()
        {
            string refs = @"<ref>foo</ref> and <ref>foo</ref> and <ref>foo</ref> and <ref>foo</ref> and <ref>foo</ref> and <ref>foo</ref> ";

            string Over20Refs = refs + refs + refs + refs + @"'''Fred Smith''' (born 1980) is a bloke. {{Persondata}}";

            Assert.AreEqual(Over20Refs, Parsers.FixPeopleCategories(Over20Refs, "test"), "no change when over 20 refs and no exiting birth/death/living cat");

            Assert.IsTrue(Parsers.FixPeopleCategories(Over20Refs + @" [[Category:Living people]]", "test").Contains(@"[[Category:1980 births]]"), "can add cat when over 20 refs and living people cat already");
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
            List<string> Year = new List<string>(new[] { "year" });

            Assert.AreEqual(@"1990", Parsers.GetInfoBoxFieldValue(@"hello {{infobox foo
|year=1990
|other=great}} now", Year));

            Assert.AreEqual(@"1990", Parsers.GetInfoBoxFieldValue(@"hello {{infobox foo
|  year  =  1990
|other=great}} now", Year));

            Assert.AreEqual(@"1990", Parsers.GetInfoBoxFieldValue(@"hello {{infobox foo
|  Year  =  1990
|other=great}} now", Year));

            // no infobox
            Assert.AreEqual(@"", Parsers.GetInfoBoxFieldValue(@"hello now", Year));

            // field not found
            Assert.AreEqual(@"", Parsers.GetInfoBoxFieldValue(@"hello {{infobox foo
|  Year  =  1990
|other=great}} now", new List<string>(new[] { "yearly" })));

            // multiple fields on same line
            Assert.AreEqual(@"1990", Parsers.GetInfoBoxFieldValue(@"hello {{infobox foo
|  Year  =  1990  |some=where
|other=great}} now", Year));
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
            Assert.AreEqual(a + "\r\n", Parsers.FixPeopleCategories(a + "\r\n" + @"[[Category:Year of death missing]]", "test"));

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
            Assert.IsFalse(Parsers.CategoryMatch(@"foo [[Category:1990_births]]", @"1990 births"));
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
[[Category:1960 births|Smith, Fred]]", "A"));

            // no sortkey
            Assert.AreEqual(@"'''Fred Smith''' (born 1960) is a bloke.
[[Category:1960 births]][[Category:Living people]]", Parsers.LivingPeople(@"'''Fred Smith''' (born 1960) is a bloke.
[[Category:1960 births]]", "A"));
            
            // non-mainspace: add with colon
                        Assert.AreEqual(@"'''Fred Smith''' (born 1960) is a bloke.
[[Category:1960 births]][[:Category:Living people]]", Parsers.LivingPeople(@"'''Fred Smith''' (born 1960) is a bloke.
[[Category:1960 births]]", "Wikipedia:Sandbox"));

            // no matches if not identified as born
            const string b1 = @"'''Fred Smith''' is a bloke.";
            Assert.AreEqual(b1, Parsers.LivingPeople(b1, "A"));

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

            Assert.AreEqual(a, Parsers.LivingPeople(a, "A"));
            Assert.AreEqual(b, Parsers.LivingPeople(b, "A"));
            Assert.AreEqual(c, Parsers.LivingPeople(c, "A"));
            Assert.AreEqual(d, Parsers.LivingPeople(d, "A"));
            Assert.AreEqual(e, Parsers.LivingPeople(e, "A"));
            Assert.AreEqual(f, Parsers.LivingPeople(f, "A"));
            Assert.AreEqual(g, Parsers.LivingPeople(g, "A"));
            Assert.AreEqual(h, Parsers.LivingPeople(h, "A"));

            // assume dead if born earlier than 121 years ago, so no change
            const string d1 = @"'''Fred Smith''' (born 1879) is a bloke.
[[Category:1879 births|Smith, Fred]]";
            Assert.AreEqual(d1, Parsers.LivingPeople(d1, "A"));

            // check correctly handles birth category with no year to parse
            const string d2 = @"Fred [[Category:15th-century births]]";
            Assert.AreEqual(d2, Parsers.LivingPeople(d2, "A"));
        }

        [Test]
        public void FixISBNFormat()
        {
            Assert.AreEqual(@"ISBN 1245781549", Parsers.FixSyntax(@"ISBN: 1245781549"), "removes colon after ISBN");
            Assert.AreEqual(@"ISBN 1245781549", Parsers.FixSyntax(@"[[ISBN]] 1245781549"), "removes wikilink around ISBN");
            Assert.AreEqual(@"ISBN 1245781549", Parsers.FixSyntax(@"ISBN-10: 1245781549"), "removes colon after ISBN");
            Assert.AreEqual(@"ISBN 9781245781549", Parsers.FixSyntax(@"ISBN-13: 9781245781549"), "removes colon after ISBN");
            Assert.AreEqual(@"ISBN 1245781549", Parsers.FixSyntax(@"ISBN 1245781549"), "no change if already correct");
            Assert.AreEqual(@"ISBN 1245781549", Parsers.FixSyntax(@"ISBN:1245781549"), "removes colon after ISBN");

            Assert.AreEqual(@"ISBN 1245781549", Parsers.FixSyntax(@"ISBN-1245781549"), "removes minus after ISBN");
            Assert.AreEqual(@"ISBN 1045781549", Parsers.FixSyntax(@"ISBN-1045781549"), "removes minus after ISBN");
            Assert.AreEqual(@"ISBN-10 12345781549", Parsers.FixSyntax(@"ISBN-10 12345781549"), "do nothing");
            Assert.AreEqual(@"ISBN-13 12345781549", Parsers.FixSyntax(@"ISBN-13 12345781549"), "do nothing");

            //{{ISBN-10}} and {{ISBN-13}} have been deleted
            //Assert.AreEqual(@"{{ISBN-10|1245781549}}", Parsers.FixSyntax(@"{{ISBN-10|1245781549}}"), "no change if already correct – ISBN-10 template");
            //Assert.AreEqual(@"{{ISBN-13|9781245781549}}", Parsers.FixSyntax(@"{{ISBN-13|9781245781549}}"), "no change if already correct – ISBN-13 template");

            Assert.AreEqual(@"[http://www.hup.harvard.edu/catalog.php?isbn=9780674372993 example]", Parsers.FixSyntax(@"[http://www.hup.harvard.edu/catalog.php?isbn=9780674372993 example]"), "no change inside url");
            Assert.AreEqual(@"foo<ref name=""isbn0-19-517234-5"" />", Parsers.FixSyntax(@"foo<ref name=""isbn0-19-517234-5"" />"), "no change inside ref");
        }


        [Test]
        public void FixPMIDFormat()
        {
            Assert.AreEqual(@"PMID 1245781549", Parsers.FixSyntax(@"PMID: 1245781549"), "removes colon after PMID");
            Assert.AreEqual(@"PMID 1245781549", Parsers.FixSyntax(@"PMID:1245781549"), "removes colon after PMID");
            Assert.AreEqual(@"PMID 1245781549", Parsers.FixSyntax(@"PMID:    1245781549"), "removes colon after PMID");
            Assert.AreEqual(@"PMID 1245781549", Parsers.FixSyntax(@"PMID 1245781549"), "No change if alrady correct");
        }

        [Test]
        public void FixHtmlTagsSyntax()
        {
            const string corr = @"Foo<small>bar</small> was the 1<sub>st</sub> to drink H<sup>2</sup>O";
            Assert.AreEqual(corr, Parsers.FixSyntax(@"Foo<small>bar<small/> was the 1<sub>st<sub/> to drink H<sup>2<sup/>O"));
            Assert.AreEqual(corr, Parsers.FixSyntax(@"Foo<small>bar<small/> was the 1<sub>st</sub/> to drink H<sup>2</sup/>O"));
            Assert.AreEqual(corr, Parsers.FixSyntax(corr));
 
            Assert.AreEqual(@"<center>Centered text</center>", Parsers.FixSyntax(@"<center>Centered text<center/>"));
        }

        [Test]
        public void FixSyntaxHorizontalRule()
        {
            Assert.AreEqual(@"----", Parsers.FixSyntax(@"<hr>"));
            Assert.AreEqual(@"----", Parsers.FixSyntax(@"-----"));
            Assert.AreEqual(@"A
----
B", Parsers.FixSyntax(@"A
<hr>
B"));
            string Nochange = @"A<hr>";
            Assert.AreEqual(Nochange, Parsers.FixSyntax(Nochange));
            Nochange = @"A----";
            Assert.AreEqual(Nochange, Parsers.FixSyntax(Nochange));
        }

        [Test]
        public void FixSyntaxRedirects()
        {
            Assert.AreEqual(@"#REDIRECT [[Foo]]", Parsers.FixSyntaxRedirects(@"#REDIRECT
[[Foo]]"),"newline");
            Assert.AreEqual(@"#REDIRECT [[Foo]]", Parsers.FixSyntaxRedirects(@"#REDIRECT [[[Foo]]]"),"extra opening/closing bracket");
            Assert.AreEqual(@"#REDIRECT [[Foo]]", Parsers.FixSyntaxRedirects(@"#REDIRECT [[Foo]]]"),"one extra closing bracket");
            Assert.AreEqual(@"#REDIRECT [[Foo]]", Parsers.FixSyntaxRedirects(@"#REDIRECT [[[Foo]]"),"one extra opening bracket");
            Assert.AreEqual(@"#REDIRECT [[Foo]]", Parsers.FixSyntaxRedirects(@"#REDIRECT [[[[Foo]]"),"two extra opening brackets");
            Assert.AreEqual(@"#REDIRECT [[Foo]]", Parsers.FixSyntaxRedirects(@"#REDIRECT [[Foo]]]]"),"two extra closing brackets");
            Assert.AreEqual(@"#REDIRECT [[Foo]]", Parsers.FixSyntaxRedirects(@"#REDIRECT [[[[Foo]]"),"two extra opening/closing brackets");
            Assert.AreEqual(@"#REDIRECT [[Foo]]", Parsers.FixSyntaxRedirects(@"#REDIRECT
[[[Foo]]]"),"extra brackets and newline");
            Assert.AreEqual(@"#REDIRECT [[Foo]]", Parsers.FixSyntaxRedirects(@"#REDIRECT:[[Foo]]"),"double dot unspaced");
            Assert.AreEqual(@"#REDIRECT [[Foo]]", Parsers.FixSyntaxRedirects(@"#REDIRECT: [[Foo]]"),"double dot with space");
            Assert.AreEqual(@"#REDIRECT [[Foo]]", Parsers.FixSyntaxRedirects(@"#REDIRECT=[[Foo]]"),"equal sign unspaced");
            Assert.AreEqual(@"#REDIRECT [[Foo]]", Parsers.FixSyntaxRedirects(@"#REDIRECT= [[Foo]]"),"equal sihn with space");
            Assert.AreEqual(@"#REDIRECT [[Foo]]", Parsers.FixSyntaxRedirects(@"#REDIRECT=
[[[Foo]]]"),"extra brackets, equal sign and newline");
        }

        [Test]
        public void ExternalLinksNewline()
        {
            Assert.AreEqual(@"here [http://www.site.com text here]", Parsers.FixSyntax(@"here [http://www.site.com text
here]"), "newline removed");
            Assert.AreEqual(@"here [http://www.site.com text here ]", Parsers.FixSyntax(@"here [http://www.site.com text here
]"), "newline removed");
            Assert.AreEqual(@"here [http://www.site.com text here there]", Parsers.FixSyntax(@"here [http://www.site.com text
here
there]"), "multiple newlines removed");
            Assert.AreEqual(@"here [http://www.site.com text here]", Parsers.FixSyntax(@"here [http://www.site.com |text
here]"), "newline removed");

            Assert.AreEqual(@"here [http://www.site.com text here]", Parsers.FixSyntax(@"here [http://www.site.com text here]"), "no change if no new line");
        }

        [Test]
        public void UnbalancedBrackets()
        {
            int bracketLength = 0;
            Assert.AreEqual(18, Parsers.UnbalancedBrackets(@"now hello {{bye}} {{now}", out bracketLength));
            Assert.AreEqual(2, bracketLength);
            Assert.AreEqual(18, Parsers.UnbalancedBrackets(@"now hello {{bye}} {{now} abc", out bracketLength));
            Assert.AreEqual(2, bracketLength);
            Assert.AreEqual(18, Parsers.UnbalancedBrackets(@"now hello {{bye}} [[now]", out bracketLength));
            Assert.AreEqual(2, bracketLength);
            Assert.AreEqual(18, Parsers.UnbalancedBrackets(@"now hello [[bye]] {{now}", out bracketLength));
            Assert.AreEqual(2, bracketLength);
            Assert.AreEqual(18, Parsers.UnbalancedBrackets(@"now hello {{bye}} [now words [here]", out bracketLength));
            Assert.AreEqual(1, bracketLength);
            Assert.AreEqual(21, Parsers.UnbalancedBrackets(@"now hello {{bye}} now] words [here]", out bracketLength));
            Assert.AreEqual(1, bracketLength);
            Assert.AreEqual(22, Parsers.UnbalancedBrackets(@"now hello {{bye}} {now}}", out bracketLength));
            Assert.AreEqual(2, bracketLength);
            Assert.AreEqual(33, Parsers.UnbalancedBrackets(@"[http://www.site.com a link [cool]]", out bracketLength)); // FixSyntax replaces with &#93;
            Assert.AreEqual(2, bracketLength);
            Assert.AreEqual(18, Parsers.UnbalancedBrackets(@"now hello {{bye}} {now", out bracketLength));
            Assert.AreEqual(1, bracketLength);
            Assert.AreEqual(0, Parsers.UnbalancedBrackets(@"{bye", out bracketLength));
            Assert.AreEqual(1, bracketLength);
            Assert.AreEqual(0, Parsers.UnbalancedBrackets(@"<bye", out bracketLength));
            Assert.AreEqual(1, bracketLength);
            Assert.AreEqual(36, Parsers.UnbalancedBrackets(@"now hello [words [here&#93; end] now]", out bracketLength));
            Assert.AreEqual(1, bracketLength);

            // only first reported
            Assert.AreEqual(18, Parsers.UnbalancedBrackets(@"now hello {{bye}} {{now} or {{now} was", out bracketLength));
            Assert.AreEqual(18, Parsers.UnbalancedBrackets(@"now hello {{bye}} {{now} or [[now] was", out bracketLength));

            Assert.AreEqual(115, Parsers.UnbalancedBrackets(@"==External links==
*[http://www.transfermarkt.de/profil.html]&section=p&teamid=458 Profile] at Transfermarkt.de
*[http://www.vi.nl/Spelers
", out bracketLength));
            Assert.AreEqual(1, bracketLength);
            
            Assert.AreEqual(0, Parsers.UnbalancedBrackets(@"{{Infobox|foo=bar (OMG} }}", out bracketLength));

            Assert.AreEqual(4, Parsers.UnbalancedBrackets(@"now [[link],] at", out bracketLength));
            Assert.AreEqual(2, bracketLength);
        }

        [Test]
        public void UnbalancedBracketsNone()
        {
            int bracketLength = 0;
            // brackets all okay
            Assert.AreEqual(-1, Parsers.UnbalancedBrackets(@"now hello {{bye}} {{now}}", out bracketLength));
            Assert.AreEqual(-1, Parsers.UnbalancedBrackets(@"now hello [[bye]] {{now}}", out bracketLength));
            Assert.AreEqual(-1, Parsers.UnbalancedBrackets(@"now hello {{bye}} [now]", out bracketLength));
            Assert.AreEqual(-1, Parsers.UnbalancedBrackets(@"now hello", out bracketLength));
            Assert.AreEqual(-1, Parsers.UnbalancedBrackets(@"<ref>[http://www.pubmedcentral.nih.gov/articlerender.fcgi?artid=32159 Message to
complementary and alternative medicine: evidence is a better friend than power. Andrew J Vickers]</ref>", out bracketLength));
            Assert.AreEqual(-1, Parsers.UnbalancedBrackets(@"[http://www.site.com a link [cool&#93;]", out bracketLength)); // displays as valid syntax
            Assert.AreEqual(-1, Parsers.UnbalancedBrackets(@"[http://www.site.com a link &#91;cool&#93; here]", out bracketLength)); // displays as valid syntax
            Assert.AreEqual(-1, Parsers.UnbalancedBrackets(@"*[http://external.oneonta.edu/html A&#91;lbert&#93; T. 1763]", out bracketLength));

            // don't consider stuff in <math> or <pre> tags etc.
            Assert.AreEqual(-1, Parsers.UnbalancedBrackets(@"now hello {{bye}} <pre>{now}}</pre>", out bracketLength));
            Assert.AreEqual(-1, Parsers.UnbalancedBrackets(@"now hello {{bye}} <math>{a{b}}</math>", out bracketLength));
            Assert.AreEqual(-1, Parsers.UnbalancedBrackets(@"now hello {{bye}} <code>{now}}</code>", out bracketLength));
            // ignore in certain templates
            Assert.AreEqual(-1, Parsers.UnbalancedBrackets(@"now hello {{LSJ|foo(bar}}", out bracketLength));
            Assert.AreEqual(-1, Parsers.UnbalancedBrackets(@"now hello {{)!}}", out bracketLength));
            Assert.AreEqual(-1, Parsers.UnbalancedBrackets(@"now hello {{!(}}", out bracketLength));
        }

        [Test]
        public void UnbalancedTags()
        {
            int bracketLength = 0;
            // unbalanced tags
            Assert.AreEqual(15, Parsers.UnbalancedBrackets(@"now <b>hello /b>", out bracketLength));
            Assert.AreEqual(1, bracketLength);

            Assert.AreEqual(27, Parsers.UnbalancedBrackets(@"<a>asdf</a> now <b>hello /b>", out bracketLength));
            Assert.AreEqual(1, bracketLength);

            // not unbalanced
            Assert.AreEqual(-1, Parsers.UnbalancedBrackets(@"now was < 50 cm long", out bracketLength));
        }

        [Test]
        public void FixCitationTemplatesNewlineInTitle()
        {
            Assert.AreEqual(@"now {{cite web| url=a.com|title=hello world|format=PDF}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=hello
world|format=PDF}} was"), "newline converted to space");
            const string NoURL = @"now {{cite news|title=hello
world|format=PDF}} was";

            Assert.AreEqual(NoURL, Parsers.FixCitationTemplates(NoURL), "title newline not changed when no URL");
        }

        [Test]
        public void UnspacedCommaPageRange()
        {
            Assert.AreEqual(@"{{cite book|url=http://www.stie.com | pages=55, 59 }}", Parsers.FixCitationTemplates(@"{{cite book|url=http://www.stie.com | pages=55,59 }}"));
            Assert.AreEqual(@"{{cite book|url=http://www.stie.com | pages=483, 491–492 }}", Parsers.FixCitationTemplates(@"{{cite book|url=http://www.stie.com | pages=483,491–492 }}"));
            Assert.AreEqual(@"{{cite book|url=http://www.stie.com | pages=267–268, 273, 299 }}", Parsers.FixCitationTemplates(@"{{cite book|url=http://www.stie.com | pages=267–268,273,299 }}"));
            Assert.AreEqual(@"{{cite book|url=http://www.stie.com | pages=55, 59 }}", Parsers.FixCitationTemplates(@"{{cite book|url=http://www.stie.com | pages=55, 59 }}"), "no change when already correct");
            Assert.AreEqual(@"{{cite book|url=http://www.stie.com | pages=12,354–12,386 }}", Parsers.FixCitationTemplates(@"{{cite book|url=http://www.stie.com | pages=12,354–12,386 }}"), "no change when already correct");
            Assert.AreEqual(@"{{cite book|url=http://www.stie.com | pages=12,354 }}", Parsers.FixCitationTemplates(@"{{cite book|url=http://www.stie.com | pages=12,354 }}"), "no change when already correct");

            Assert.AreEqual(@"{{cite book|url=http://www.stie.com | pages=483, 491, 492 }}", Parsers.FixCitationTemplates(@"{{cite book|url=http://www.stie.com | pages=483,491,492 }}"));
        }

        [Test]
        public void FixCitationTemplatesQuotedTitle()
        {
            Assert.AreEqual(@"now {{cite web| url=a.com|title=hello|format=PDF}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=""hello""|format=PDF}} was"));
            Assert.AreEqual(@"now {{cite web| url=a.com|title=hello|format=PDF}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=""hello|format=PDF}} was"));
            Assert.AreEqual(@"now {{cite web| url=a.com|title=hello|format=PDF}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=hello""|format=PDF}} was"));
            Assert.AreEqual(@"now {{cite web| url=a.com|trans_title=hello|format=PDF}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|trans_title=""hello""|format=PDF}} was"));

            Assert.AreEqual(@"now {{cite web| url=a.com|title=""hello"" world|format=PDF}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=""hello"" world|format=PDF}} was"));

            // curly quote cleanup to straight quotes [[MOS:PUNCT]]
            Assert.AreEqual(@"now {{cite web| url=a.com|title=hello|format=PDF}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=“hello“|format=PDF}} was"));
            Assert.AreEqual(@"now {{cite web| url=a.com|title=and ""hello"" there|format=PDF}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=and “hello“ there|format=PDF}} was"));
            Assert.AreEqual(@"now {{cite web| url=a.com|title=hello|format=PDF}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=“hello""|format=PDF}} was"));
            Assert.AreEqual(@"now {{cite web| url=a.com|title=hello|format=PDF}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=«hello»|format=PDF}} was"));
            Assert.AreEqual(@"now {{cite web| url=a.com|title=now ""hello"" at|format=PDF}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=now «hello» at|format=PDF}} was"));
            Assert.AreEqual(@"now {{cite web| url=a.com|title=now ""hello"" at|format=PDF}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=now ‹hello› at|format=PDF}} was"));
            Assert.AreEqual(@"now {{cite web| url=a.com|title=now ""hello"" at ""hello2"" be|format=PDF}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=now ‹hello› at ‹hello2› be|format=PDF}} was"));
            Assert.AreEqual(@"now {{cite web| url=a.com|title=hello» second|format=PDF}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=hello» second|format=PDF}} was"), @"no change if » used as section delimeter");
            Assert.AreEqual(@"now {{cite web| url=a.com|title=hello> second|format=PDF}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=hello> second|format=PDF}} was"), @"no change if > used as section delimeter");
            Assert.AreEqual(@"now {{cite web| url=a.com|title=hello « second|format=PDF}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=hello « second|format=PDF}} was"), @"no change if « used as section delimeter");
            Assert.AreEqual(@"now {{cite web| url=a.com|title=hello› second|format=PDF}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=hello› second|format=PDF}} was"), @"no change if › used as section delimeter");
        }

        [Test]
        public void FixCitationGoogleBooks()
        {
            string correct = @"now {{cite book|title=a |url=http://books.google.com/foo | year=2009}}";
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"now {{cite web|title=a |url=http://books.google.com/foo | year=2009}}"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"now {{cite web|title=a |url=http://books.google.com/foo | year=2009}}"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"now {{cite web|title=a |url=http://books.google.com/foo | year=2009}}"));

            // whitespace not changed
            Assert.AreEqual(@"now {{ Cite book |title=a |url=http://books.google.com/foo | year=2009}}", Parsers.FixCitationTemplates(@"now {{ Cite web |title=a |url=http://books.google.com/foo | year=2009}}"));

            Assert.AreEqual(correct, Parsers.FixCitationTemplates(correct));

            string noChange = @"now {{Cite book|title=a |url=http://books.google.com/foo | year=2009 | work = some Journal}}";
            Assert.AreEqual(noChange, Parsers.FixCitationTemplates(noChange), "journal cites to Google books not changed");
        }

        [Test]
        public void FixCitationURLNoHTTP()
        {
            string correct = @"now {{cite web|title=foo | url=http://www.foo.com | date = 1 June 2010 }}";

            Assert.AreEqual(correct, Parsers.FixCitationTemplates(correct.Replace("http://", "")), "Adds http:// when URL begins www.");
            Assert.AreEqual(correct.Replace("www", "Www"), Parsers.FixCitationTemplates(correct.Replace("http://www", "Www")), "Adds http:// when URL begins www.");
            Assert.AreEqual(correct.Replace("www", "www2"), Parsers.FixCitationTemplates(correct.Replace("http://www", "www2")), "Adds http:// when URL begins www2");
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(correct), "no change if already correct URL");
            Assert.AreEqual(correct.Replace("url=", "archiveurl="), Parsers.FixCitationTemplates(correct.Replace("url=http://", "archiveurl=")), "Adds http:// when archiveurl begins www.");
            Assert.AreEqual(correct.Replace("url=", "contribution-url="), Parsers.FixCitationTemplates(correct.Replace("url=http://", "contribution-url=")), "Adds http:// when contribution-url begins www.");
            string dash = @"now {{cite web|title=foo | url=www-foo.a.com | date = 1 June 2010 }}";
            Assert.AreEqual(dash.Replace("www", "http://www"), Parsers.FixCitationTemplates(dash), "handles www-");
        }

        [Test]
        public void WorkInItalics()
        {
            string correct = @"now {{cite web| url=http://site.net/1.pdf|format=PDF|work=Foo}}";

            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"now {{cite web| url=http://site.net/1.pdf|format=PDF|work=''Foo''}}"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(correct));

            const string website = @"now {{cite web| url=http://site.net/1.pdf|format=PDF|work=''site.net''}}";
            Assert.AreEqual(website, Parsers.FixCitationTemplates(website), "italics not removed for work=website");
        }

        [Test]
        public void FixCitationYear()
        {
            string correct = @"now {{cite book|title=a |url=http://books.google.com/foo | date=2009}}";
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(correct));

            string correct2 = @"now {{cite book|title=a |url=http://books.google.com/foo | date=2009 |page=32}}";
            Assert.AreEqual(correct2, Parsers.FixCitationTemplates(correct2));

            string nochange1 = @"now {{cite book|title=a |url=http://books.google.com/foo | year=2009a}}",
            nochange2 = @"now {{cite book|title=a |url=http://books.google.com/foo | year=2009a| date = 2009-05-16}}";

            Assert.AreEqual(nochange1, Parsers.FixCitationTemplates(nochange1));
            Assert.AreEqual(nochange2, Parsers.FixCitationTemplates(nochange2));
        }

        [Test]
        public void FixCitationTemplatesDateInYear()
        {
            string correct1 = @"now {{cite book|title=a |url=http://books.google.com/foo | date=2009-10-17}}",
            correct2 = @"now {{cite book|title=a |url=http://books.google.com/foo | date=2009-10-17|last=Smith}}";

            // ISO
            Assert.AreEqual(correct1, Parsers.FixCitationTemplates(@"now {{cite book|title=a |url=http://books.google.com/foo | year=2009-10-17}}"));
            Assert.AreEqual(correct2, Parsers.FixCitationTemplates(@"now {{cite book|title=a |url=http://books.google.com/foo | year=2009-10-17|last=Smith}}"));

            // Int
            Assert.AreEqual(correct1.Replace("2009-10-17", "17 October 2009"), Parsers.FixCitationTemplates(@"now {{cite book|title=a |url=http://books.google.com/foo | year=2009-10-17}}".Replace("2009-10-17", "17 October 2009")));
            Assert.AreEqual(correct1.Replace("2009-10-17", "17 October 2009"), Parsers.FixCitationTemplates(@"now {{cite book|title=a |url=http://books.google.com/foo | year=2009-10-17}}".Replace("2009-10-17", "17 October, 2009")));
            Assert.AreEqual(correct2.Replace("2009-10-17", "17 October 2009"), Parsers.FixCitationTemplates(@"now {{cite book|title=a |url=http://books.google.com/foo | year=2009-10-17|last=Smith}}".Replace("2009-10-17", "17 October 2009")));

            // American
            Assert.AreEqual(correct1.Replace("2009-10-17", "October 17, 2009"), Parsers.FixCitationTemplates(@"now {{cite book|title=a |url=http://books.google.com/foo | year=2009-10-17}}".Replace("2009-10-17", "October 17, 2009")));
            Assert.AreEqual(correct2.Replace("2009-10-17", "October 17, 2009"), Parsers.FixCitationTemplates(@"now {{cite book|title=a |url=http://books.google.com/foo | year=2009-10-17|last=Smith}}".Replace("2009-10-17", "October 17, 2009")));
            Assert.AreEqual(correct2.Replace("2009-10-17", "October 17, 2009"), Parsers.FixCitationTemplates(@"now {{cite book|title=a |url=http://books.google.com/foo | year=2009-10-17|last=Smith}}".Replace("2009-10-17", "October, 17, 2009")));

            Assert.AreEqual(correct1, Parsers.FixCitationTemplates(correct1));

            string nochange1 = @"now {{cite book|title=a |url=http://books.google.com/foo | year=2009–2010}}";
            Assert.AreEqual(nochange1, Parsers.FixCitationTemplates(nochange1));
        }

        [Test]
        public void FixCitationTemplatesYearWithinDate()
        {
            string correct1 = @"now {{cite book|title=a |url=http://books.google.com/foo | date=2009-10-17}}",
            nochange1 = @"now {{cite book|title=a |url=http://books.google.com/foo | date=2009-10-17 | year=2009a}}",
            nochange1b = @"now {{cite book|title=a |url=http://books.google.com/foo | date=2009 | year=2009a}}",
            nochange2 = @"now {{cite book|title=a |url=http://books.google.com/foo | date=2009-10-17 | year=2004}}",
            nochange2b = @"now {{cite book|title=a |url=http://books.google.com/foo | date=17 October 2009 | year=2004}}",
            nochange2c = @"now {{cite book|title=a |url=http://books.google.com/foo | date=October 17, 2009 | year=2004}}";

            Assert.AreEqual(correct1, Parsers.FixCitationTemplates(correct1.Replace(@"}}", @"|year=2009}}")));
            Assert.AreEqual(correct1, Parsers.FixCitationTemplates(correct1.Replace(@"foo", @"foo |year=2009")));

            Assert.AreEqual(nochange1, Parsers.FixCitationTemplates(nochange1), "Harvard anchors using YYYYa are not removed");
            Assert.AreEqual(nochange1b, Parsers.FixCitationTemplates(nochange1b), "Harvard anchor using YYYYa in year and year in date: both needed so not removed");
            Assert.AreEqual(nochange2, Parsers.FixCitationTemplates(nochange2), "Year not removed if different to year in ISO date");
            Assert.AreEqual(nochange2b, Parsers.FixCitationTemplates(nochange2b), "Year not removed if different to year in International date");
            Assert.AreEqual(nochange2c, Parsers.FixCitationTemplates(nochange2c), "Year not removed if different to year in American date");
            string correct3 = @"now {{cite book|title=a |url=http://books.google.com/foo | date=17 May 2009}}";

            string nochange4 = @"{{cite book|title=a |url=http://books.google.com/foo | date=May 2009 | year=2009 }}";
            Assert.AreEqual(nochange4, Parsers.FixCitationTemplates(nochange4), "Year not removed if date is only 'Month YYYY'");

            Assert.AreEqual(correct3, Parsers.FixCitationTemplates(correct3.Replace(@"}}", @"|year=2009}}")), "year removed when within date");
            Assert.AreEqual(@"{{cite book|title=a |url=http://books.google.com/foo | year=2009 }}", Parsers.FixCitationTemplates(@"{{cite book|title=a |url=http://books.google.com/foo | date=2009 | year=2009 }}"), 
                            "Date removed if date is YYYY and year same");
        }
        
        [Test]
        public void FixCitationTemplatesMonthWithinDate()
        {
        	string correct = @"now {{cite book|title=a |url=http://books.google.com/foo | date=17 May 2009}}",
        	nochange = @"now {{cite book|title=a |url=http://books.google.com/foo | date=17 May 2009 | month=March}}";

        	Assert.AreEqual(correct, Parsers.FixCitationTemplates(correct.Replace(@"}}", @"|month=May}}")), "month removed when within date");
        	Assert.AreEqual(nochange, Parsers.FixCitationTemplates(nochange), "month not removed if different to month in date");
        	
        	Assert.AreEqual(correct, Parsers.FixCitationTemplates(correct.Replace(@"}}", @"|month=5}}")), "number month removed when within date");
        	Assert.AreEqual(correct, Parsers.FixCitationTemplates(correct.Replace(@"}}", @"|month=05}}")), "number month removed when within date");
        	
        	string correct2 = @"now {{cite book|title=a |url=http://books.google.com/foo | date=17 December 2009}}",
        	nochange2 = @"now {{cite book|title=a |url=http://books.google.com/foo | date=17 May 2009 | month=2}}";
        	Assert.AreEqual(correct2, Parsers.FixCitationTemplates(correct2.Replace(@"}}", @"|month=12}}")), "number month removed when within date");
        	Assert.AreEqual(nochange2, Parsers.FixCitationTemplates(nochange2), "nn month not removed if different to month in date");

        	Assert.AreEqual(@"{{cite journal|last=Mahoney|first=Noreen|date=2010 April 14|volume=58}}", 
        	                Parsers.FixCitationTemplates(@"{{cite journal|last=Mahoney|first=Noreen|date=2010 April 14|year=2010|month=April|volume=58}}"), "year not added when date already has it");

        	string correct3 = @"now {{cite book|title=a |url=http://books.google.com/foo | date=2009-12-17}}";
        	Assert.AreEqual(correct3, Parsers.FixCitationTemplates(correct3.Replace(@"}}", @"|month=December}}")), "number month removed when within date");
        }

        [Test]
        public void FixCitationTemplatesDayMonthInDate()
        {
            string correct1 = @"now {{cite book|title=a |url=http://books.google.com/foo | date=17 October 2009 }}";

            Assert.AreEqual(correct1, Parsers.FixCitationTemplates(@"now {{cite book|title=a |url=http://books.google.com/foo | date=17 October |year=2009}}"));
            Assert.AreEqual(correct1.Replace("17 October", "October 17,"), Parsers.FixCitationTemplates(@"now {{cite book|title=a |url=http://books.google.com/foo | date=October 17 |year=2009}}"));
            Assert.AreEqual(correct1, Parsers.FixCitationTemplates(correct1));
        }

        [Test]
        public void CitationPublisherToWork()
        {
            string correct0 = @"{{cite news|url=http://www.timesonline.co.uk/foo494390.htm | date =2008-09-07 | work=The Times }}",
            correct1 = @"{{cite web|url=http://www.timesonline.co.uk/foo494390.htm | date =2008-09-07 | work=The Times }}",
            correct2 = @"{{citeweb|url=http://www.timesonline.co.uk/foo494390.htm | date =2008-09-07 | work = The Times }}",
            correct3 = @"{{cite web|url=http://www.timesonline.co.uk/foo494390.htm | work= |date =2008-09-07 | work = The Times }}";

            Assert.AreEqual(correct0, Parsers.CitationPublisherToWork(correct0.Replace("work", "publisher")));
            Assert.AreEqual(correct1, Parsers.CitationPublisherToWork(correct1.Replace("work", "publisher")));
            Assert.AreEqual(correct2, Parsers.CitationPublisherToWork(correct2.Replace("work", "publisher")));

            // work present but null
            Assert.AreEqual(correct3, Parsers.CitationPublisherToWork(correct3.Replace("work =", "publisher =")));

            string workalready1 = @"{{cite web|url=http://www.timesonline.co.uk/foo494390.htm | publisher=Media International |date =2008-09-07 | work = The Times }}";
            // work already
            Assert.AreEqual(workalready1, Parsers.CitationPublisherToWork(workalready1));
            Assert.AreEqual(correct0, Parsers.CitationPublisherToWork(correct0));

            // no cite web/news
            const string citeJournal = @"{{cite journal|url=http://www.timesonline.co.uk/foo494390.htm | date =2008-09-07 | publisher=The Times }}";
            Assert.AreEqual(citeJournal, Parsers.CitationPublisherToWork(citeJournal));
        }

        [Test]
        public void FixCitationDupeFields()
        {
            string correct = @"{{cite web|url=a |title=b | accessdate=11 May 2008|year=2008}}";
            string correct2 = @"{{cite web|url=a |title=b | accessdate=11 May 2008|year=2008|work=here}}";
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008|year=2008}}"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=200 | accessdate=11 May 2008|year=2008}}"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year= | accessdate=11 May 2008|year=2008}}"));

            Assert.AreEqual(correct2, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008|year=2008|work=here}}"));
            Assert.AreEqual(correct2, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |  year=   2008 | accessdate=11 May 2008|year=2008|work=here}}"));
            Assert.AreEqual(correct2, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=200| accessdate=11 May 2008|year=2008|work=here}}"));
            Assert.AreEqual(correct2, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year= | accessdate=11 May 2008|year=2008|work=here}}"));

            string correct3 = @"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008|work=here there}}";
            Assert.AreEqual(correct3, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008|work=here there|work=here}}"));
            Assert.AreEqual(correct3, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008|work=here|work=here there}}"));
            Assert.AreEqual(correct3, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008|work=here there|work = here }}"));
            Assert.AreEqual(correct3, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008|work=here there|work=here th}}"));
            Assert.AreEqual(correct3, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008|work=here there|work=here there}}"));

            string nochange1 = @"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008|year=2004}}";
            Assert.AreEqual(nochange1, Parsers.FixCitationTemplates(nochange1));

            string nochange2 = @"{{cite web|url=http://www.tise.com/abc|year=2008|page.php=7 |title=b |year=2008 | accessdate=11 May 2008}}";
            Assert.AreEqual(nochange2, Parsers.FixCitationTemplates(nochange2));

            string nochange3 = @"{{cite book|title=b |year=2008 | accessdate=11 May 2008|year=2004}}";
            Assert.AreEqual(nochange3, Parsers.FixCitationTemplates(nochange3));

            // null fields
            Assert.AreEqual(@"now {{cite web| url=http://site.net |accessdate = 2008-10-08|title=hello}}", Parsers.FixCitationTemplates(@"now {{cite web| url=http://site.net |title=|accessdate = 2008-10-08|title=hello}}"));
            Assert.AreEqual(@"now {{cite web| url=http://site.net |accessdate = 2008-10-08|title=hello|work=BBC}}", Parsers.FixCitationTemplates(@"now {{cite web| url=http://site.net |title=|accessdate = 2008-10-08|title=hello|work=BBC}}"));
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
            string correct = @"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008}}";
            string correct2 = @"{{cite web|url=a |title=b |year=2008 | accessdate=May 11, 2008}}";
            string correct3 = @"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008 |work=c}}";
            string correct4 = @"{{cite book|url=a |title=b |year=2008 | accessdate=11 May 2008 |work=c}}";

            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=11 May| accessyear=2008}}"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=11 May| accessyear = 2008  }}"));
            Assert.AreEqual(correct2, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=May 11| accessyear=2008}}"));
            Assert.AreEqual(correct2, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=May 11| accessyear = 2008  }}"));
            Assert.AreEqual(correct3, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=11 May | accessyear = 2008 |work=c}}"));
            Assert.AreEqual(correct4, Parsers.FixCitationTemplates(@"{{cite book|url=a |title=b |year=2008 | accessdate=11 May | accessyear = 2008 |work=c}}"));

            // only for cite web
            string nochange2 = @"{{cite podcast|url=a |title=b |year=2008 | accessdate=11 May |accessyear=2008 |work=c}}";
            Assert.AreEqual(nochange2, Parsers.FixCitationTemplates(nochange2));
        }

        [Test]
        public void AccessDayMonthDay()
        {
            string a = @"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008}}";
            Assert.AreEqual(a, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008|accessdaymonth=   }}"));
            Assert.AreEqual(a, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008|accessmonthday  =   }}"));
            Assert.AreEqual(a, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008|accessmonth  =   }}"));
            Assert.AreEqual(a, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 |accessmonthday  = | accessdate=11 May 2008}}"));

            string notempty = @"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008|accessdaymonth=Foo   }}";
            Assert.AreEqual(notempty, Parsers.FixCitationTemplates(notempty));
        }

        [Test]
        public void FixCitationTemplatesAccessYear()
        {
            string a = @"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008}}";
            Assert.AreEqual(a, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008|accessyear=2008   }}"));
            Assert.AreEqual(a, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008| accessyear =  2008   }}"));
            Assert.AreEqual(a, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 |accessyear=2008   | accessdate=11 May 2008}}"));

            string yearDoesNotMatch = @"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008|accessyear=Winter   }}";
            Assert.AreEqual(yearDoesNotMatch, Parsers.FixCitationTemplates(yearDoesNotMatch));
        }

        [Test]
        public void FixCitationTemplatesDateLeadingZero()
        {
            string a = @"{{cite web|url=a |title=b |year=2008 | accessdate=1 May 2008}}";
            Assert.AreEqual(a, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=01 May 2008}}"));

            string a0 = @"{{cite web|url=a |title=b | accessdate=1 May 2008 | date=May 1, 2008}}";
            Assert.AreEqual(a0, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b | accessdate=01 May 2008 | date=May 01, 2008}}"));

            string a2 = @"{{cite web|url=a |title=b |year=2008 | accessdate=1 May 1998}}";
            Assert.AreEqual(a2, Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=01 May 1998}}"));
            Assert.AreEqual(a2.Replace(@"}}", @" }}"), Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=01 May 1998 }}"));

            string b = @"{{cite book|url=a |title=b | date=May 1, 2008}}";
            Assert.AreEqual(b, Parsers.FixCitationTemplates(@"{{cite book|url=a |title=b | date=May 01, 2008}}"));
            Assert.AreEqual(b, Parsers.FixCitationTemplates(@"{{cite book|url=a |title=b | date=May 01 2008}}"));

            string c = @"{{cite book|url=a |title=b | date=May 1, 2008|author=Lee}}";
            Assert.AreEqual(c, Parsers.FixCitationTemplates(@"{{cite book|url=a |title=b | date=May 01, 2008|author=Lee}}"));

            string d = @"{{cite book|url=a |title=b |year=2008 | date=May 1|author=Lee}}";
            Assert.AreEqual(d, Parsers.FixCitationTemplates(@"{{cite book|url=a |title=b |year=2008 | date=May 01|author=Lee}}"));
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

            //removal of unneccessary language field
            Assert.AreEqual(@"{{cite web|title=foo|url=http://site.net|year=2009}}", Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009|language=English}}"));
            Assert.AreEqual(@"{{cite web|title=foo|url=http://site.net|year=2009}}", Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009|language = English}}"));
            Assert.AreEqual(@"{{cite web|title=foo|url=http://site.net|year=2009}}", Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009|language=english}}"));
            Assert.AreEqual(@"{{cite web|title=foo|url=http://site.net|year=2009}}", Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009|language=en}}"));

            //fix language field
            Assert.AreEqual(@"{{cite web|title=foo|url=http://site.net|year=2009}}", Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009|language={{en icon}}}}"));
            Assert.AreEqual(@"{{cite web|title=foo|url=http://site.net|year=2009|language=sv}}", Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009|language={{sv icon}}}}"));
            Assert.AreEqual(@"{{cite web|title=foo|url=http://site.net|year=2009|language=de}}", Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009|language={{de icon}}}}"));
            Assert.AreEqual(@"{{cite web|title=foo|url=http://site.net|year=2009|language=el|publisher=Ser}}", Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009|language={{el icon}}|publisher=Ser}}"));

            // removal of null 'format=' when URL is to HTML
            Assert.AreEqual(@"{{cite web|title=foo|url=http://site.net|year=2009|format=}}", Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009|format=}}"));
            Assert.AreEqual(@"{{cite web|title=foo|url=http://site.net/a.htm|year=2009}}", Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net/a.htm|year=2009|format=}}"));
            Assert.AreEqual(@"{{cite web|title=foo|url=http://site.net/a.html|year=2009}}", Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net/a.html|year=2009|format=}}"));
            Assert.AreEqual(@"{{cite web|title=foo|url=http://site.net/a.HTML|year=2009}}", Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net/a.HTML|year=2009|format=}}"));

            // removal of null 'origdate='
            Assert.AreEqual(@"{{cite web|title=foo|url=http://site.net|year=2009}}", Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009|origdate=}}"));
            Assert.AreEqual(@"{{cite web|title=foo|url=http://site.net|year=2009|origdate=2005}}", Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009|origdate=2005}}"));

            // id=ASIN... fix
            Assert.AreEqual(@"{{cite book|title=foo|asin=123456789X|year=2009}}", Parsers.FixCitationTemplates(@"{{cite book|title=foo|id=ASIN 123456789X|year=2009}}"));
            Assert.AreEqual(@"{{cite book|title=foo|asin=123456789X |year=2009}}", Parsers.FixCitationTemplates(@"{{cite book|title=foo|id=ASIN 123456789X |year=2009}}"));
            Assert.AreEqual(@"{{cite book|title=foo|asin=123-45678-9-X|year=2009}}", Parsers.FixCitationTemplates(@"{{cite book|title=foo|id=ASIN: 123-45678-9-X|year=2009}}"));

            const string NoChangeSpacedEndashInTitle = @"{{cite web | author=IGN staff | year=2008 | title=IGN Top 100 Games 2008 – 2 Chrono Trigger | url=http://top100.ign.com/2008/ign_top_game_2.html | publisher=IGN | accessdate=March 13, 2009}}";

            Assert.AreEqual(NoChangeSpacedEndashInTitle, Parsers.FixCitationTemplates(NoChangeSpacedEndashInTitle));

            const string NoChangeFormatGivesSize = @"{{cite web|title=foo|url=http://site.net/asdfsadf.PDF|year=2009|format=150 MB}}";

            Assert.AreEqual(NoChangeFormatGivesSize, Parsers.FixCitationTemplates(NoChangeFormatGivesSize));
        }

        [Test]
        public void FixCitationTemplatesISBN()
        {
            // id=ISBN... fix
            Assert.AreEqual(@"{{cite book|title=foo|isbn=123456789X|year=2009}}", Parsers.FixCitationTemplates(@"{{cite book|title=foo|id=ISBN 123456789X|year=2009}}"));
            Assert.AreEqual(@"{{cite book|title=foo|isbn=123456789X |year=2009}}", Parsers.FixCitationTemplates(@"{{cite book|title=foo|id=ISBN 123456789X |year=2009}}"));
            Assert.AreEqual(@"{{cite book|title=foo|isbn=123-45678-9-X|year=2009}}", Parsers.FixCitationTemplates(@"{{cite book|title=foo|id=ISBN: 123-45678-9-X|year=2009}}"));

            string doubleISBN = @"{{cite book|title=foo|id=ISBN 012345678X, 978012345678X|year=2009}}";
            Assert.AreEqual(doubleISBN, Parsers.FixCitationTemplates(doubleISBN), "no changes when two isbns present");
            doubleISBN = @"{{cite book|title=foo|id=ISBN 012345678X ISBN 978012345678X|year=2009}}";
            Assert.AreEqual(doubleISBN, Parsers.FixCitationTemplates(doubleISBN), "no changes when two isbns present");

            string existingISBN = @"{{cite book|title=foo|id=ISBN 012345678X |isbn= 978012345678X|year=2009}}";
            Assert.AreEqual(existingISBN, Parsers.FixCitationTemplates(existingISBN), "no changes when isbn param already has value");

            existingISBN = @"{{cite book|title=foo|id=ISBN 012345678X |ISBN= 978012345678X|year=2009}}";
            Assert.AreEqual(existingISBN, Parsers.FixCitationTemplates(existingISBN), "no changes when isbn param already has value");

            // ISBN format fixes
            Assert.AreEqual(@"{{cite book|title=foo|isbn=123456789X|year=2009}}", Parsers.FixCitationTemplates(@"{{cite book|title=foo|isbn=ISBN 123456789X|year=2009}}"));
            Assert.AreEqual(@"{{cite book|title=foo|ISBN=123456789X|year=2009}}", Parsers.FixCitationTemplates(@"{{cite book|title=foo|ISBN=ISBN 123456789X|year=2009}}"));
            Assert.AreEqual(@"{{cite book|title=foo|isbn=123456789X|year=2009}}", Parsers.FixCitationTemplates(@"{{cite book|title=foo|isbn=ISBN  123456789X|year=2009}}"));
            Assert.AreEqual(@"{{cite book|title=foo|isbn=123456789X|year=2009}}", Parsers.FixCitationTemplates(@"{{cite book|title=foo|isbn=ISBN123456789X|year=2009}}"));
            Assert.AreEqual(@"{{cite book|title=foo|isbn=123456789X|year=2009}}", Parsers.FixCitationTemplates(@"{{cite book|title=foo|isbn=isbn123456789X|year=2009}}"));

            Assert.AreEqual(@"{{cite book|title=foo|isbn=123456789X|year=2009}}", Parsers.FixCitationTemplates(@"{{cite book|title=foo|isbn=ISBN 123456789X.|year=2009}}"));
            Assert.AreEqual(@"{{cite book|title=foo|isbn=123456789X|year=2009}}", Parsers.FixCitationTemplates(@"{{cite book|title=foo|isbn=123456789X.|year=2009}}"));
            Assert.AreEqual(@"{{cite book|title=foo|isbn=123456789X|year=2009}}", Parsers.FixCitationTemplates(@"{{cite book|title=foo|isbn=123456789X..|year=2009}}"));
            Assert.AreEqual(@"{{cite book|title=foo|isbn=123456789X|year=2009}}", Parsers.FixCitationTemplates(@"{{cite book|title=foo|isbn=123456789X,|year=2009}}"));
            Assert.AreEqual(@"{{cite book|title=foo|isbn=123456789X|year=2009}}", Parsers.FixCitationTemplates(@"{{cite book|title=foo|isbn=123456789X;|year=2009}}"));
            Assert.AreEqual(@"{{cite book|title=foo|isbn=123456789X|year=2009}}", Parsers.FixCitationTemplates(@"{{cite book|title=foo|isbn=123456789X:|year=2009}}"));

            Assert.AreEqual(@"{{cite book|title=foo|isbn=123-456-789-X|year=2009}}", Parsers.FixCitationTemplates(@"{{cite book|title=foo|isbn=123–456–789–X :|year=2009}}"));
            Assert.AreEqual(@"{{cite book|title=foo|isbn=123-4-56789-X |year=2009}}", Parsers.FixCitationTemplates(@"{{cite book|title=foo|isbn=123–4–56789–X |year=2009}}"));
            Assert.AreEqual(@"{{cite book|title=foo|isbn=123-4-56789-X |year=2009}}", Parsers.FixCitationTemplates(@"{{cite book|title=foo|isbn=123‐4‐56789‐X |year=2009}}")); // U+2010 character
            Assert.AreEqual(@"{{cite book|title=foo|isbn=123-4-56789-X |year=2009}}", Parsers.FixCitationTemplates(@"{{cite book|title=foo|isbn=123‒4‒56789‒X |year=2009}}")); // U+2012 character
        }

        [Test]
        public void FixCitationTemplatesOrigYear()
        {
            Assert.AreEqual(@"{{cite book | title=ABC | publisher=Pan  | year=1950 }}", Parsers.FixCitationTemplates(@"{{cite book | title=ABC | publisher=Pan  | origyear=1950 }}"), "origyear to year when no year/date");
            Assert.AreEqual(@"{{cite book | title=ABC | publisher=Pan  | year=1950 }}", Parsers.FixCitationTemplates(@"{{cite book | title=ABC | publisher=Pan  | origyear=1950 | year =}}"), "origyear to year when blank year");

            const string nochange1 = @"{{cite book | title=ABC | publisher=Pan | year=2004 | origyear=1950 }}", nochange2 = @"{{cite book | title=ABC | publisher=Pan | date=May 2004 | origyear=1950 }}"
                , nochange3 = @"{{cite book | title=ABC | publisher=Pan | origyear=11 May 1950 }}";

            Assert.AreEqual(nochange1, Parsers.FixCitationTemplates(nochange1), "origyear valid when year present");
            Assert.AreEqual(nochange2, Parsers.FixCitationTemplates(nochange2), "origyear valid when date present");
            Assert.AreEqual(nochange3, Parsers.FixCitationTemplates(nochange3), "origyear not renamed when more than just a year");
        }

        [Test]
        public void PersonDataAddition()
        {
            const string Fred = @"'''Fred''' (born 1960) is.
[[Category:1960 births]]", FredPD = @"'''Fred''' (born 1960) is.
[[Category:1960 births]]
{{Persondata|DATE OF BIRTH=1960 | NAME= Fred}}";

            Assert.IsTrue(Tools.NestedTemplateRegex("persondata").IsMatch(Parsers.PersonData(Fred, "Fred")), "Adds persondata for BLP when missing");
            Assert.IsFalse(Tools.NestedTemplateRegex("persondata").IsMatch(Parsers.PersonData("test", "Fred")), "PersonData not added when not BLP");

            Assert.AreEqual(FredPD, Parsers.PersonData(FredPD, "Fred"), "No change when persondata already present for BLP");

            const string BrokenPersondata = @"{{Persondata<!--Metadata: see [[Wikipedia:Persondata]].-->
|NAME=Orbeliani, Georgy Ilyich
|ALTERNATIVE NAMES=
|SHORT DESCRIPTION=
|DATE OF BIRTH=1853
|PLACE OF BIRTH=[[Tbilisi]], [[Georgia Governorate]], [[Russian Empire]]
|DATE OF DEATH=1924
|PLACE OF DEATH=[[Paris, France]]

{{DEFAULTSORT:Orbeliani, Georgy}}";
            
            Assert.AreEqual(BrokenPersondata, Parsers.PersonData(BrokenPersondata, "Test"), "no change when existing persondata template with unbalanced main brackets");
        }

        [Test]
        public void PersonDataAdditionName()
        {
            string Fred = @"'''Fred''' (born 1960) is.
[[Category:1960 births]]";
            string res = Parsers.PersonData(Fred, "Fred");
            string PD = Tools.NestedTemplateRegex("persondata").Match(res).Value;
            Assert.AreEqual(Tools.GetTemplateParameterValue(PD, "NAME"), "Fred", "Persondata NAME taken from articletitle if no DEFAULTSORT and single word title");

            Fred = @"'''Fred''' (born 1960) is.
[[Category:1960 births]] {{DEFAULTSORT:Smith, Fred}}";
            res = Parsers.PersonData(Fred, "Fred");
            PD = Tools.NestedTemplateRegex("persondata").Match(res).Value;
            Assert.AreEqual(Tools.GetTemplateParameterValue(PD, "NAME"), "Smith, Fred", "Persondata NAME taken from DEFAULTSORT when available, even when single word title");

            Fred = @"'''Fréd''' (born 1960) is.
[[Category:1960 births]] {{DEFAULTSORT:Fred}}";
            res = Parsers.PersonData(Fred, "Fréd");
            PD = Tools.NestedTemplateRegex("persondata").Match(res).Value;
            Assert.AreEqual(Tools.GetTemplateParameterValue(PD, "NAME"), "Fréd", "Persondata NAME includes diacritics");
        }

        [Test]
        public void PersonDataAdditionEnOnly()
        {
#if DEBUG
            const string Fred = @"'''Fred''' (born 1960) is.
[[Category:1960 births]]";

            Variables.SetProjectLangCode("fr");
            Assert.IsFalse(Tools.NestedTemplateRegex("persondata").IsMatch(Parsers.PersonData(Fred, "Fred")), "Adds persondata for BLP when missing");

            Variables.SetProjectLangCode("en");
            Assert.IsTrue(Tools.NestedTemplateRegex("persondata").IsMatch(Parsers.PersonData(Fred, "Fred")), "Adds persondata for BLP when missing");
#endif
        }

        [Test]
        public void PersonDataCompletionDOB()
        {
            const string a = @"{{persondata
            |NAME=
            |DATE OF BIRTH=
            |DATE OF DEATH=}}", a2 = @"{{persondata
            |NAME= Doe, John
            |DATE OF BIRTH= 27 June 1950
            |DATE OF DEATH=}}", i1 = @"{{infobox foo| dateofbirth = 27 June 1950}}", i2 = @"{{infobox foo| dateofbirth = {{birth date|1950|06|27}}}}"
                , i2b = @"{{infobox foo| dateofbirth = {{birth date|df=y|1950|06|27}}}}",
            a3 = @"{{persondata
            |NAME= Doe, John
            |DATE OF BIRTH=
            |DATE OF DEATH=}}";

            Assert.AreEqual(i1 + a2, Parsers.PersonData(i1 + a, "John Doe"));
            Assert.AreEqual(i1 + a2, Parsers.PersonData(i1 + @"{{persondata
            |NAME= Doe, John
            |date of birth=
            |date of death=}}", "John Doe"));
            Assert.AreEqual(i1.Replace("27 June", "June 27,") + a2.Replace("27 June", "June 27,"), Parsers.PersonData(i1.Replace("27 June", "June 27,") + a.Replace("27 June", "June 27,"), "John Doe"));
            Assert.AreEqual(i2 + a2.Replace("27 June 1950", "1950-06-27"), Parsers.PersonData(i2 + a, "John Doe"));
            Assert.AreEqual(i2.Replace(@"{{birth date|1950|06|27}}", @"{{birth-date|27 June 1950|27 June 1950}}") + a2,
                            Parsers.PersonData(i2.Replace(@"{{birth date|1950|06|27}}", @"{{birth-date|27 June 1950|27 June 1950}}") + a, "John Doe"));
            Assert.AreEqual(i2.Replace("27}}", "27}} in London") + a2.Replace("27 June 1950", "1950-06-27"),
                            Parsers.PersonData(i2.Replace("27}}", "27}} in London") + a, "John Doe"), "Completes persondata from {{birth date}} when extra data in infobox field");
            Assert.AreEqual(i2b.Replace("27}}", "27}} in London") + a2,
                            Parsers.PersonData(i2b.Replace("27}}", "27}} in London") + a, "John Doe"), "Completes persondata from {{birth date}} when extra data in infobox field");

            string i3 = i1.Replace("27 June 1950", @"{{dda|2005|07|20|1950|06|27|df=yes}}");
            Assert.AreEqual(i3 + a2.Replace("DEATH=", "DEATH= 20 July 2005"), Parsers.PersonData(i3 + a, "John Doe"), "takes dates from {{dda}}");

            string i4 = @"{{infobox foo| birthyear = 1950 | birthmonth=06 | birthday=27}}";
            Assert.AreEqual(i4 + a2 + @"{{use dmy dates}}", Parsers.PersonData(i4 + a + @"{{use dmy dates}}", "John Doe"), "takes dates from birthyear etc. fields");

            string i5 = @"{{infobox foo| yob = 1950 | mob=06 | dob=27}}";
            Assert.AreEqual(i5 + a2 + @"{{use dmy dates}}", Parsers.PersonData(i5 + a + @"{{use dmy dates}}", "John Doe"), "takes dates from birthyear etc. fields");

            Assert.AreEqual(i1 + a + a, Parsers.PersonData(i1 + a + a, "John Doe"), "no change when multiple personData templates");

            string i6 = i1.Replace("27 June 1950", @"{{dda|2005|07|00|1950|06|00|df=yes}}");
            Assert.AreEqual(i6 + a3, Parsers.PersonData(i6 + a3, "John Doe"), "ignores incomplete/zerod dates from {{dda}}");

            string UnformatedDOB = @"'''Fred''' (born 27 June 1950) was great [[Category:1950 births]]";
            Assert.AreEqual(UnformatedDOB + a2, Parsers.PersonData(UnformatedDOB + a, "John Doe"), "sets full birth date when matches category");

            UnformatedDOB = @"'''Fred''' (born June 27, 1950) was great [[Category:1950 births]] {{use dmy dates}}";
            Assert.AreEqual(UnformatedDOB + a2, Parsers.PersonData(UnformatedDOB + a, "John Doe"), "sets full birth date when matches category, American date");

            UnformatedDOB = UnformatedDOB.Replace(@"[[Category:1950 births]]", "");
            Assert.AreEqual(UnformatedDOB + a3, Parsers.PersonData(UnformatedDOB + a3, "John Doe"), "not set when no birth category");

            UnformatedDOB = @"'''Fred''' (born 27 June 1949) was great [[Category:1950 births]]";
            Assert.AreEqual(UnformatedDOB + a2.Replace("27 June 1950", "1950"), Parsers.PersonData(UnformatedDOB + a, "John Doe"), "not set when full birth date doesn't match category");

            UnformatedDOB = @"'''Fred''' (born circa 27 June 1950) was great [[Category:1950 births]]";
            Assert.AreEqual(UnformatedDOB + a2.Replace("27 June 1950", "1950"), Parsers.PersonData(UnformatedDOB + a, "John Doe"), "only year set when circa date");

            UnformatedDOB = @"'''Fred''' (reigned 27 June 1950 – 11 May 1990) was great";
            Assert.AreEqual(UnformatedDOB + a2.Replace(" 27 June 1950", ""), Parsers.PersonData(UnformatedDOB + a, "John Doe"), "No dates set when reigned");

            UnformatedDOB = @"'''Fred''' (baptized 27 June 1950 – 11 May 1990) was great";
            Assert.AreEqual(UnformatedDOB + a2.Replace(" 27 June 1950", ""), Parsers.PersonData(UnformatedDOB + a, "John Doe"), "No dates set when baptized");

            const string Clark = @"
{{use mdy dates}}
{{Infobox college coach
| name          = Lyal W. Clark
| birth_date   = {{birth date|mf=yes|1904|07|04}}
}}

{{Persondata <!-- Metadata: see [[Wikipedia:Persondata]]. -->
| NAME              = Clark, Lyal W.
| ALTERNATIVE NAMES =
| SHORT DESCRIPTION = American college football coach
| DATE OF BIRTH     =
| PLACE OF BIRTH    =
| DATE OF DEATH     =
| PLACE OF DEATH    =
}}
[[Category:1904 births]]";
            Assert.AreEqual(Clark.Replace(@"| DATE OF BIRTH     =", @"| DATE OF BIRTH     = July 4, 1904"), Parsers.PersonData(Clark, "A"));

            string small = i1.Replace("27 June 1950", @"<small>27 June 1950</small>");
            Assert.AreEqual(small + a2, Parsers.PersonData(small + a, "John Doe"), "small tags removed");
        }

        [Test]
        public void PersonDataCompletionDOBFromCategory()
        {
            string Text = Parsers.PersonData(@"Foo [[Category:Living people]] [[Category:1980 births]]", "test");

            Assert.IsTrue(Tools.GetTemplateParameterValue(WikiRegexes.Persondata.Match(Text).Value, "DATE OF BIRTH").Equals("1980"));

            Text = Parsers.PersonData(@"Foo [[Category:Living people]] [[Category:1980 births|Foo]]", "test");
            Assert.IsTrue(Tools.GetTemplateParameterValue(WikiRegexes.Persondata.Match(Text).Value, "DATE OF BIRTH").Equals("1980"));

            Text = Parsers.PersonData(@"Foo [[Category:Living people]] [[Category:980 BC births|Foo]]", "test");
            Assert.IsTrue(Tools.GetTemplateParameterValue(WikiRegexes.Persondata.Match(Text).Value, "DATE OF BIRTH").Equals("980 BC"));
        }

        [Test]
        public void PersonDataCompletionDOD()
        {
            const string a = @"{{persondata
            |NAME=
            |DATE OF BIRTH=
            |DATE OF DEATH=}}", a2 = @"{{persondata
            |NAME= Doe, John
            |DATE OF BIRTH=
            |DATE OF DEATH= 27 June 1950}}", i1 = @"{{infobox foo| dateofdeath = 27 June 1950}}", i2 = @"{{infobox foo| dateofdeath = {{death date|1950|06|27}}}}";

            Assert.AreEqual(i1 + a2, Parsers.PersonData(i1 + a, "John Doe"));
            Assert.AreEqual(i1.Replace("27 June", "June 27,") + a2.Replace("27 June", "June 27,"), Parsers.PersonData(i1.Replace("27 June", "June 27,") + a.Replace("27 June", "June 27,"), "John Doe"));
            Assert.AreEqual(i2 + a2.Replace("27 June 1950", "1950-06-27"), Parsers.PersonData(i2 + a, "John Doe"));
            Assert.AreEqual(i2.Replace("27}}", "27}} in London") + a2.Replace("27 June 1950", "1950-06-27"),
                            Parsers.PersonData(i2.Replace("27}}", "27}} in London") + a, "John Doe"), "Completes persondata from {{death date}} when extra data in infobox field");

            string i4d = @"{{infobox foo| deathyear = 1950 | deathmonth=06 | deathday=27}}";
            Assert.AreEqual(i4d + a2.Replace(@"DATE OF BIRTH=27 June 1950
            |DATE OF DEATH= ", @"DATE OF BIRTH=
            |DATE OF DEATH= 27 June 1950") + @"{{use dmy dates}}", Parsers.PersonData(i4d + a + @"{{use dmy dates}}", "John Doe"), "takes dates from deathyear etc. fields");

            string i5 = @"{{infobox foo| yod = 1950 | mod=06 | dod=27}}";
            Assert.AreEqual(i5 + a2.Replace(@"DATE OF BIRTH=27 June 1950
            |DATE OF DEATH= ", @"DATE OF BIRTH=
            |DATE OF DEATH= 27 June 1950") + @"{{use dmy dates}}", Parsers.PersonData(i5 + a + @"{{use dmy dates}}", "John Doe"), "takes dates from deathyear etc. fields");

            string u1 = @"Fred (11 May 1920 – 4 June 2004) was great. [[Category:1920 births]] [[Category:2004 deaths]]";
            Assert.AreEqual(@"{{Persondata
|NAME= Doe, John
|DATE OF BIRTH= 11 May 1920
|DATE OF DEATH= 4 June 2004
}}" + u1, Parsers.PersonData(@"{{Persondata
|NAME= Doe, John
|DATE OF BIRTH=
|DATE OF DEATH=
}}" + u1, "John Doe"), "birth and death added from unformatted values");

            u1 = @"Fred (11 May 1920 – 4 June 2004) was great. [[Category:1920 births]] [[Category:2004 deaths]]";
            Assert.AreEqual(@"{{Persondata
|NAME= Doe, John
|DATE OF BIRTH= 11 May 1920
|DATE OF DEATH= 4 June 2004
}}" + u1, Parsers.PersonData(@"{{Persondata
|NAME= Doe, John
|DATE OF BIRTH=
|DATE OF DEATH=
}}" + u1, "John Doe"), "birth and death added from unformatted values");

            u1 = @"Fred (born 11 May 1920; died 4 June 2004) was great. [[Category:1920 births]] [[Category:2004 deaths]]";
            Assert.AreEqual(@"{{Persondata
|NAME= Doe, John
|DATE OF BIRTH= 11 May 1920
|DATE OF DEATH= 4 June 2004
}}" + u1, Parsers.PersonData(@"{{Persondata
|NAME= Doe, John
|DATE OF BIRTH=
|DATE OF DEATH=
}}" + u1, "John Doe"), "birth and death added from unformatted values");
            u1 = @"Fred (born 11 May 1920 {{ndash}} 4 June 2004) was great. [[Category:1920 births]] [[Category:2004 deaths]]";
            Assert.AreEqual(@"{{Persondata
|NAME= Doe, John
|DATE OF BIRTH= 11 May 1920
|DATE OF DEATH= 4 June 2004
}}" + u1, Parsers.PersonData(@"{{Persondata
|NAME= Doe, John
|DATE OF BIRTH=
|DATE OF DEATH=
}}" + u1, "John Doe"), "birth and death added from unformatted values");
            u1 = @"Fred (born 11 May 1920{{ndash}} 4 June 2004) was great. [[Category:1920 births]] [[Category:2004 deaths]]";
            Assert.AreEqual(@"{{Persondata
|NAME= Doe, John
|DATE OF BIRTH= 11 May 1920
|DATE OF DEATH= 4 June 2004
}}" + u1, Parsers.PersonData(@"{{Persondata
|NAME= Doe, John
|DATE OF BIRTH=
|DATE OF DEATH=
}}" + u1, "John Doe"), "birth and death added from unformatted values");
            u1 = @"Fred (born 11 May 1920{{ndash}} June 4, 2004) was great. [[Category:1920 births]] [[Category:2004 deaths]] {{use dmy dates}}";
            Assert.AreEqual(@"{{Persondata
|NAME= Doe, John
|DATE OF BIRTH= 11 May 1920
|DATE OF DEATH= 4 June 2004
}}" + u1, Parsers.PersonData(@"{{Persondata
|NAME= Doe, John
|DATE OF BIRTH=
|DATE OF DEATH=
}}" + u1, "John Doe"), "birth and death added from unformatted values, American format dates");

            u1 = @"Fred (11 May 1920 – 4 June 1000) was great. [[Category:1920 births]] [[Category:2004 deaths]]";
            Assert.AreEqual(@"{{Persondata
|NAME= Doe, John
|DATE OF BIRTH= 11 May 1920
|DATE OF DEATH= 2004
}}" + u1, Parsers.PersonData(@"{{Persondata
|NAME= Doe, John
|DATE OF BIRTH=
|DATE OF DEATH=
}}" + u1, "John Doe"), "unformatted death value not added if doesn't match category");

            u1 = @"Fred (11 May 1920 – {{circa}} 4 June 2004) was great. [[Category:1920 births]] [[Category:2004 deaths]]";
            Assert.AreEqual(@"{{Persondata
|NAME= Doe, John
|DATE OF BIRTH= 1920
|DATE OF DEATH= 2004
}}" + u1, Parsers.PersonData(@"{{Persondata
|NAME= Doe, John
|DATE OF BIRTH=
|DATE OF DEATH=
}}" + u1, "John Doe"), "unformatted death value not added if doesn't match category");

            const string Clark = @"
{{use mdy dates}}
{{Infobox college coach
| name          = Lyal W. Clark
| death_date   = {{death date and age|mf=yes|1904|07|04|1850|11|11}}
}}

{{Persondata <!-- Metadata: see [[Wikipedia:Persondata]]. -->
| NAME              = Clark, Lyal W.
| ALTERNATIVE NAMES =
| SHORT DESCRIPTION = American college football coach
| DATE OF BIRTH     =
| PLACE OF BIRTH    =
| DATE OF DEATH     =
| PLACE OF DEATH    =
}}
[[Category:1904 deaths]]";
            Assert.IsTrue(Parsers.PersonData(Clark, "A").Contains(@"| DATE OF DEATH     = July 4, 1904"));

			const string Question = @"
{{use mdy dates}}
{{Infobox college coach
| name          = Lyal W. Clark
| death_date   = ???
}}

{{Persondata <!-- Metadata: see [[Wikipedia:Persondata]]. -->
| NAME              = Clark, Lyal W.
| ALTERNATIVE NAMES =
| SHORT DESCRIPTION = American college football coach
| DATE OF BIRTH     =
| PLACE OF BIRTH    =
| DATE OF DEATH     =
| PLACE OF DEATH    =
}}
[[Category:1904 deaths]]";
			Assert.IsFalse(Tools.NestedTemplateRegex("persondata").Match(Parsers.PersonData(Question, "A")).Value.Contains(@"???"));
        }

        [Test]
        public void PersonDataParameterCasing()
        {
            const string LowerCase = @"Foo (born 1980) {{Persondata
|name=X, Foo
|date of birth=1980
}}
[[Cateogory:1980 births]]";

            Assert.AreEqual(LowerCase, Parsers.PersonData(LowerCase, "foo x"), "no update when lowercase parameter holds the data already");

            const string LowerCase2 = @"{{Persondata
|name=X, Foo
|date of birth=1980
|DATE OF BIRTH=1980
}}
[[Category:2005 deaths]]";

            Assert.AreEqual(@"{{Persondata
|NAME=X, Foo
|DATE OF BIRTH=1980
| DATE OF DEATH= 2005
}}
[[Category:2005 deaths]]", Parsers.PersonData(LowerCase2, "foo x"), "duplicate fields removed");
        }

        [Test]
        public void PersonDataCompletionDODFromCategory()
        {
            string Text = Parsers.PersonData(@"Foo [[Category:2005 deaths]] [[Category:1930 births]]", "test");

            Assert.IsTrue(Tools.GetTemplateParameterValue(WikiRegexes.Persondata.Match(Text).Value, "DATE OF BIRTH").Equals("1930"));
            Assert.IsTrue(Tools.GetTemplateParameterValue(WikiRegexes.Persondata.Match(Text).Value, "DATE OF DEATH").Equals("2005"));
        }

        [Test]
        public void PersonDataAll()
        {
            const string IB = @"{{Infobox person
|name=James Jerome Hill
|birth_date={{birthdate|1838|9|16}}
|birth_place=[[Guelph/Eramosa, Ontario|Eramosa Township]],<br>[[Ontario]], [[Canada]]
|death_date = {{dda|1916|5|29|1838|9|16}}
|death_place=[[St. Paul, Minnesota|Saint Paul]], [[Minnesota]]
|occupation=Railroad tycoon
|children= 10
}}
{{DEFAULTSORT:Hill, James J.}}", PD = @"{{Persondata
| NAME              = Hill, James J.
| ALTERNATIVE NAMES =
| SHORT DESCRIPTION =
| DATE OF BIRTH     = September 16, 1838
| PLACE OF BIRTH    = [[Guelph/Eramosa, Ontario|Eramosa Township]], [[Ontario]], [[Canada]]
| DATE OF DEATH     = May 29, 1916
| PLACE OF DEATH    = [[St. Paul, Minnesota|Saint Paul]], [[Minnesota]]
}}", AT = @"{{birthdate|1838|9|16}}
{{dda|1916|5|29|1838|9|16}}
{{Infobox foo
|birth_place=[[Guelph/Eramosa, Ontario|Eramosa Township]], [[Ontario]], [[Canada]]
|death_place=[[St. Paul, Minnesota|Saint Paul]], [[Minnesota]]
|children= 10}}
{{DEFAULTSORT:Hill, James J.}}";

            Assert.AreEqual(PD, WikiRegexes.Persondata.Match(Parsers.PersonData(IB + "May 2, 2010 and May 2, 2010", "test")).Value);
            Assert.AreEqual(PD, WikiRegexes.Persondata.Match(Parsers.PersonData(IB.Replace(@"[[Canada]]", @"[[Canada]], {{CAN}}") + "May 2, 2010 and May 2, 2010", "test")).Value, "trims trailing comma template combo");
            Assert.AreEqual(PD, WikiRegexes.Persondata.Match(Parsers.PersonData(AT + "May 2, 2010 and May 2, 2010", "test")).Value, "Values taken from birth/death templates even if outside infobox");

            const string IB2 = @"{{Infobox person
|name=James Jerome Hill
|yearofbirth=1838
|monthofbirth=9
|dayofbirth=16
|birth_place=<small>[[Guelph/Eramosa, Ontario|Eramosa Township]], [[Ontario]], [[Canada]]</small>
|yearofdeath=1916
|monthofdeath=5
|dayofdeath=29
|death_place=[[St. Paul, Minnesota|Saint Paul]], [[Minnesota]]
|occupation=Railroad tycoon
|children= 10
}}
{{DEFAULTSORT:Hill, James J.}}";

            Assert.AreEqual(PD, WikiRegexes.Persondata.Match(Parsers.PersonData(IB2 + "May 2, 2010 and May 2, 2010", "test")).Value, "pulls dates from year/month/day infobox fields");

            Assert.AreEqual(PD, WikiRegexes.Persondata.Match(Parsers.PersonData(IB.Replace(@":Hill, James J.}}", @":Hill, James J. (writer)}}") + "May 2, 2010 and May 2, 2010", "test")).Value, "Occupation cleaned from defaultsort");

            Assert.AreEqual(PD.Replace(@"[[Guelph/Eramosa, Ontario|Eramosa Township]],<br>[[Ontario]], [[Canada]]", "Ontario, Canada"),
                            WikiRegexes.Persondata.Match(Parsers.PersonData(IB.Replace(@"[[Guelph/Eramosa, Ontario|Eramosa Township]], [[Ontario]], [[Canada]]", "{{city-state|Ontario|Canada}}") + "May 2, 2010 and May 2, 2010", "test")).Value, "city state template converted");
            Assert.AreEqual(PD.Replace(@"[[St. Paul, Minnesota|Saint Paul]], [[Minnesota]]", "Saint Paul, Minnesota"),
                            WikiRegexes.Persondata.Match(Parsers.PersonData(IB.Replace(@"[[St. Paul, Minnesota|Saint Paul]], [[Minnesota]]", "Saint Paul, Minnesota}}") + "May 2, 2010 and May 2, 2010", "test")).Value, "city state template converted (death place)");

            Assert.AreEqual(PD, WikiRegexes.Persondata.Match(Parsers.PersonData(IB.Replace(@"[[Canada]]", @"[[Canada]] [[File:Foo.svg|country flag]]") + "May 2, 2010 and May 2, 2010", "test")).Value, "removes country flag from place name");
            Assert.AreEqual(PD, WikiRegexes.Persondata.Match(Parsers.PersonData(IB.Replace(@"[[Canada]]", @"[[Canada]] ???") + "May 2, 2010 and May 2, 2010", "test")).Value, "removes ??? from place name");
            Assert.AreEqual(PD, WikiRegexes.Persondata.Match(Parsers.PersonData(IB.Replace(@"[[Canada]]", @"[[Canada]] <ref>abc</ref>") + "May 2, 2010 and May 2, 2010", "test")).Value, "removes ref from place name");

            Assert.AreEqual(PD, WikiRegexes.Persondata.Match(Parsers.PersonData(IB.Replace(@"{{birthdate|1838|9|16}}", @"1838-09-16") + @"{{use mdy dates}}", "test")).Value, "ISO dates supported");

            Assert.AreEqual(PD.Replace(@"[[Guelph/Eramosa, Ontario|Eramosa Township]],<br>[[Ontario]], [[Canada]]", "[[Guelph/Eramosa, Ontario|Eramosa Township]]"),
                            WikiRegexes.Persondata.Match(Parsers.PersonData(IB.Replace(@"[[Guelph/Eramosa, Ontario|Eramosa Township]], [[Ontario]], [[Canada]]", @"[[Guelph/Eramosa, Ontario|Eramosa Township]],<br>") + "May 2, 2010 and May 2, 2010", "test")).Value, "city state template converted");


            Assert.AreEqual(PD.Replace("PLACE OF BIRTH", "place of birth"), WikiRegexes.Persondata.Match(Parsers.PersonData(IB + PD.Replace("PLACE OF BIRTH", "place of birth") + "May 2, 2010 and May 2, 2010", "test")).Value);
        }

        [Test]
        public void PersondataCleanup()
        {
          const string PD = @"{{Persondata
| NAME              = Hill, James J.
| ALTERNATIVE NAMES =
| SHORT DESCRIPTION = Politician
| DATE OF BIRTH     = September 16, 1838
| PLACE OF BIRTH    = Calgary, [[Canada]]
| DATE OF DEATH     = May 29, 1916
| PLACE OF DEATH    = [[Minnesota]]
}}";
          Assert.AreEqual(PD, Parsers.PersonData(PD.Replace("1838", "<small>1838</small>"), "Test"), "Small tag removal");

          string PD1 = @"{{Persondata
| NAME              = Hill, James J.
| ALTERNATIVE NAMES =
| SHORT DESCRIPTION = Politician
| DATE OF BIRTH     = September 16, 1838
| PLACE OF BIRTH    = Calgary, <small>[[Canada]]</small>
| DATE OF DEATH     = May 29, 1916
| PLACE OF DEATH    = [[Minnesota]]
}}";
          Assert.AreEqual(PD, Parsers.PersonData(PD1, "Test"), "Balanced small tag removal");

                    string PD2 = @"{{Persondata
| NAME              = Hill, James J.
| ALTERNATIVE NAMES =
| SHORT DESCRIPTION = Politician
| DATE OF BIRTH     = September 16, 1838
| PLACE OF BIRTH    = Calgary, [[Canada]]</small>
| DATE OF DEATH     = May 29, 1916
| PLACE OF DEATH    = [[Minnesota]]
}}";

          Assert.AreEqual(PD, Parsers.PersonData(PD2, "Test"), "Unbalanced small tag removal");

                    string PD3 = @"{{Persondata
| NAME              = Hill, James J.
| ALTERNATIVE NAMES =
| SHORT DESCRIPTION = <small>Politician</small>
| DATE OF BIRTH     = September 16, 1838
| PLACE OF BIRTH    = Calgary, [[Canada]]</small>
| DATE OF DEATH     = May 29, 1916
| PLACE OF DEATH    = [[Minnesota]]
}}";

          Assert.AreEqual(PD, Parsers.PersonData(PD3, "Test"), "Small tag removal");
        }

        [Test]
        public void FixCitationTemplatesEnOnly()
        {
#if DEBUG
            const string bad = @"{{cite web|title=foo|url=http://site.net|year=2009|format=HTML}}";

            Variables.SetProjectLangCode("fr");
            Assert.AreEqual(bad, Parsers.FixCitationTemplates(bad));

            Variables.SetProjectLangCode("en");
            Assert.AreEqual(@"{{cite web|title=foo|url=http://site.net|year=2009}}", Parsers.FixCitationTemplates(bad));
#endif
        }

        [Test]
        public void FixCitationTemplatesPagesPP()
        {
            // removal of pp. from 'pages' field
            Assert.AreEqual(@"{{cite book|author=Smith|title=Great|pages=57–59}}", Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|pages=pp. 57–59}}"));
            Assert.AreEqual(@"{{cite book|author=Smith|title=Great|pages=57–59}}", Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|pages=pgs. 57–59}}"));
            Assert.AreEqual(@"{{cite book|author=Smith|title=Great|pages=57–59}}", Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|pages=pg. 57–59}}"));
            Assert.AreEqual(@"{{cite book|author=Smith|title=Great|pages=57–59}}", Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|pages=pp.57–59}}"));
            Assert.AreEqual(@"{{cite book|author=Smith|title=Great|pages=57–59}}", Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|pages=pp.&nbsp;57–59}}"));
            Assert.AreEqual(@"{{cite book|author=Smith|title=Great|pages= 57–59}}", Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|pages= pp. 57–59}}"));
            Assert.AreEqual(@"{{cite book|author=Smith|title=Great|pages=57–59|year=2007}}", Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|pages=pp. 57-59|year=2007}}"));
            Assert.AreEqual(@"{{cite book|author=Smith|title=Great|page=57}}", Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|page=p. 57}}"));
            Assert.AreEqual(@"{{cite book|author=Smith|title=Great|page=57}}", Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|page=p 57}}"));
            Assert.AreEqual(@"{{cite book|author=Smith|title=Great|page=57}}", Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|page=pp 57}}"));

            const string nochange0 = @"{{cite book|author=Smith|title=Great|page= 57}}", nochange1 = @"{{cite book|author=Smith|title=Great|page= para 57}}",
            nochange2 = @"{{cite book|author=Smith|title=Great|page= P57}}";
            Assert.AreEqual(nochange0, Parsers.FixCitationTemplates(nochange0));
            Assert.AreEqual(nochange1, Parsers.FixCitationTemplates(nochange1));
            Assert.AreEqual(nochange2, Parsers.FixCitationTemplates(nochange2));

            // not when nopp
            Assert.AreEqual(@"{{cite book|author=Smith|title=Great|pages=pp. 57–59|nopp=yes}}", Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|pages=pp. 57-59|nopp=yes}}"));

            // not for cite journal
            const string journal = @"{{cite journal|author=Smith|title=Great|page=p.57}}";
            Assert.AreEqual(journal, Parsers.FixCitationTemplates(journal));
        }

        [Test]
        public void FixCitationTemplatesPageRangeName()
        {
            string correct = @"{{cite book|author=Smith|title=Great|pages=57–59}}";
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|pages=pp. 57–59}}"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|page=pp. 57–59}}"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|pages=57–59}}"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|pages=57 – 59}}"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|pages=57 – 59}}"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|pages=57--59}}"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|page=57–59}}"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|page=57—59}}"));

            Assert.AreEqual(correct.Replace("–", ", "), Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|page=57, 59}}"), "page -> pages for comma list of page numbers");

            correct = @"{{cite book|author=Smith|title=Great|pages=57&ndash;59}}";
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|pages=57 &ndash; 59}}"));

            const string nochange = @"{{cite book|author=Smith|title=Great|pages=12,255}}";
            Assert.AreEqual(nochange, Parsers.FixCitationTemplates(nochange));

            const string correct2 = @"{{cite book |title=Evaluation of x |last=Office |year=2001 |pages=1–2 |accessdate=39 June 2011 }}";

            Assert.AreEqual(correct2, Parsers.FixCitationTemplates(@"{{cite book |title=Evaluation of x |last=Office |year=2001 |page=1-2 |pages= |accessdate=39 June 2011 }}"));
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
            Assert.AreEqual(correct,
                            Parsers.FixCitationTemplates(@"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume=Vol. 3|issue=Nos. 3|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ"));

            Assert.AreEqual(@"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume= 3| issue = 3|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ",
                            Parsers.FixCitationTemplates(@"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume=volume 3 Issue 3|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ"));

            Assert.AreEqual(@"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume= 3| issue = 3–4|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ",
                            Parsers.FixCitationTemplates(@"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume=volume 3 Issues 3–4|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ"));
            Assert.AreEqual(@"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume=3| issue =  3–4|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ",
                            Parsers.FixCitationTemplates(@"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume=3, Nos. 3–4|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ"));


            Assert.AreEqual(@"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume= 3| issue = 3–4|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ",
                            Parsers.FixCitationTemplates(@"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume=volume 3 numbers 3–4|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ"));

            string nochange1 = @"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume=special numbers 3–4|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}}";

            Assert.AreEqual(nochange1, Parsers.FixCitationTemplates(nochange1));

            string nochange2 = @"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: |volume=3 Issue 3|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm|issue=December}} Robert M. Price (ed.), Bloomfield, NJ";

            Assert.AreEqual(nochange2, Parsers.FixCitationTemplates(nochange2));
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

            const string journalstart = @"{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu |volume= 3|issue= 3| ";

            Assert.AreEqual(journalstart + @"pages = 140–148}}", Parsers.FixCitationTemplates(journalstart + @"pages = 140   - 148}}")); // hyphen

            Assert.AreEqual(journalstart + @"pages = 140–8}}", Parsers.FixCitationTemplates(journalstart + @"pages = 140-8}}")); // hyphen

            Assert.AreEqual(journalstart + @"pages = 140–48}}", Parsers.FixCitationTemplates(journalstart + @"pages = 140 -48}}")); // hyphen

            Assert.AreEqual(journalstart + @"pages = 140–148}}", Parsers.FixCitationTemplates(journalstart + @"pages = 140   - 148}}")); // hyphen
            Assert.AreEqual(journalstart + @"pages = 940–1083}}", Parsers.FixCitationTemplates(journalstart + @"pages = 940   - 1083}}")); // hyphen

            // multiple ranges
            Assert.AreEqual(journalstart + @"pages = 140–148, 150, 152–157}}", Parsers.FixCitationTemplates(journalstart + @"pages = 140-148, 150, 152-157}}")); // hyphen

            const string nochange1 = @"{{cite conference
  | first = Owen
  | title = System Lifecycle Cost
  | booktitle = AIAA Space 2007
  | pages = Paper No. AIAA-2007–6023
  | year = 2007
  | accessdate = }}";

            Assert.AreEqual(nochange1, Parsers.FixCitationTemplates(nochange1)); // range over 999 pages
        }

        [Test]
        public void HarvTemplatesPageRange()
        {
            Assert.AreEqual(@"{{harv|Smith|2005|pp=55–59}}", Parsers.FixCitationTemplates(@"{{harv|Smith|2005|pp=55–59}}"));
            Assert.AreEqual(@"{{harvnb|Smith|2005|pp=55–59}}", Parsers.FixCitationTemplates(@"{{harvnb|Smith|2005|pp=55–59}}"));
            Assert.AreEqual(@"{{harv|Smith|2005|pp=55–59}}", Parsers.FixCitationTemplates(@"{{harv|Smith|2005|pp=55 – 59}}"));
            Assert.AreEqual(@"{{harv|Smith|2005|pp=55–59, 77–81}}", Parsers.FixCitationTemplates(@"{{harv|Smith|2005|pp=55–59, 77-81}}"));

            Assert.AreEqual(@"{{rp|55–59, 77–81}}", Parsers.FixCitationTemplates(@"{{rp|55–59, 77-81}}"));
            Assert.AreEqual(@"{{rp|77–81}}", Parsers.FixCitationTemplates(@"{{rp|77-81}}"));
            Assert.AreEqual(@"{{rp|77}}", Parsers.FixCitationTemplates(@"{{rp|77}}"));
            Assert.AreEqual(@"{{rp|77, 80}}", Parsers.FixCitationTemplates(@"{{rp|77, 80}}"));
        }

        [Test]
        public void HarvTemplatesPP()
        {
            Assert.AreEqual(@"{{harv|Smith|2005|pp=55–59}}", Parsers.FixCitationTemplates(@"{{harv|Smith|2005|p=55–59}}"), "renames p to pp for page range");
            Assert.AreEqual(@"{{harvnb|Smith|2005|pp=55–59}}", Parsers.FixCitationTemplates(@"{{harvnb|Smith|2005|p=55–59}}"));
            Assert.AreEqual(@"{{harvnb|Smith|2005|pp=55&ndash;59}}", Parsers.FixCitationTemplates(@"{{harvnb|Smith|2005|p=55&ndash;59}}"));
            Assert.AreEqual(@"{{harv|Smith|2005|pp=55–59}}", Parsers.FixCitationTemplates(@"{{harv|Smith|2005|p=55 – 59}}"));
            Assert.AreEqual(@"{{harv|Smith|2005|pp=55–59, 77–81}}", Parsers.FixCitationTemplates(@"{{harv|Smith|2005|p=55–59, 77-81}}"));

            const string nochange = @"{{Harvnb|Shapiro|2010|p=271 (238–9)}}";
            Assert.AreEqual(nochange, Parsers.FixCitationTemplates(nochange));
        }

        [Test]
        public void CiteTemplatesPageSections()
        {
            const string journalstart = @"{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu |volume= 3|issue= 3| ";

            // do not change page sections etc.
            const string nochange1 = @"{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu |volume= 3|issue= 3|pages = 140–7-2}}";

            Assert.AreEqual(nochange1, Parsers.FixCitationTemplates(nochange1));

            Assert.AreEqual(journalstart + @"pages = 8-4}}", Parsers.FixCitationTemplates(journalstart + @"pages = 8-4}}")); // hyphen
            Assert.AreEqual(journalstart + @"pages = 8-8}}", Parsers.FixCitationTemplates(journalstart + @"pages = 8-8}}")); // hyphen
            Assert.AreEqual(journalstart + @"pages = 8-4, 17-34}}", Parsers.FixCitationTemplates(journalstart + @"pages = 8-4, 17-34}}")); // hyphen
            Assert.AreEqual(journalstart + @"pages = 17-34, 8-4}}", Parsers.FixCitationTemplates(journalstart + @"pages = 17-34, 8-4}}")); // hyphen

            // non-breaking hyphens to represent page sections rather than ranges
            const string nochange2 = @"{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu |volume= 3|issue= 3|pages = 140‑7}}", nochange3 = @"{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu |volume= 3|issue= 3|pages = 140&#8209;7}}";

            Assert.AreEqual(nochange2, Parsers.FixCitationTemplates(nochange2));
            Assert.AreEqual(nochange3, Parsers.FixCitationTemplates(nochange3));

            const string nochange4 = @"*{{cite book
 | author = United States Navy
 | pages = 4-13 to 4-17
 | chapter = 4
}}";

            Assert.AreEqual(nochange4, Parsers.FixCitationTemplates(nochange4), "two section links with word to");

            const string nochange5a = @"{{cite book | isbn = 084
 | pages = 3-262, 8-106, 15-20
 | url =
 | accessdate =
}}", nochange5b = @"{{cite book | isbn = 084
 | pages = 3-262, 3-106, 15-20
 | url =
 | accessdate =
}}";
            Assert.AreEqual(nochange5a, Parsers.FixCitationTemplates(nochange5a), "overlapping ranges");
            Assert.AreEqual(nochange5b, Parsers.FixCitationTemplates(nochange5b), "overlapping ranges, same start");
        }

        [Test]
        public void FixCitationTemplatesOrdinalDates()
        {
            Assert.AreEqual(@"{{cite web|url=http://news.bbc.co.uk/1/hi/uk_politics/8080777.stm|title=Brown pressure as Blears quits|
publisher=The BBC|date=June 3, 2009|accessdate=January 15, 2010}}", Parsers.FixCitationTemplates(@"{{cite web|url=http://news.bbc.co.uk/1/hi/uk_politics/8080777.stm|title=Brown pressure as Blears quits|
publisher=The BBC|date=June 3rd, 2009|accessdate=January 15th, 2010}}"));

            Assert.AreEqual(@"{{cite web|url=http://news.bbc.co.uk/1/hi/uk_politics/8080777.stm|title=Brown pressure as Blears quits|
publisher=The BBC|date=June 3, 2009|accessdate=January 15, 2010}}", Parsers.FixCitationTemplates(@"{{cite web|url=http://news.bbc.co.uk/1/hi/uk_politics/8080777.stm|title=Brown pressure as Blears quits|
publisher=The BBC|date=June 3rd, 2009|accessdate=January 15th 2010}}"));

            Assert.AreEqual(@"{{cite web|url=http://news.bbc.co.uk/1/hi/uk_politics/8080777.stm|title=Brown pressure as Blears quits|
publisher=The BBC|date=June 3, 2009|accessdate=January 15, 2010}}", Parsers.FixCitationTemplates(@"{{cite web|url=http://news.bbc.co.uk/1/hi/uk_politics/8080777.stm|title=Brown pressure as Blears quits|
publisher=The BBC|date=June 3rd, 2009|accessdate=January 15, 2010}}"));

            Assert.AreEqual(@"{{cite web|url=http://news.bbc.co.uk/1/hi/uk_politics/8080777.stm|title=Brown pressure as Blears quits|
publisher=The BBC|date=3 June 2009|accessdate=15 January 2010}}", Parsers.FixCitationTemplates(@"{{cite web|url=http://news.bbc.co.uk/1/hi/uk_politics/8080777.stm|title=Brown pressure as Blears quits|
publisher=The BBC|date=3rd June 2009|accessdate=15th January 2010}}"));

            // no change - only in title
            string nochange = @"{{cite web|url=http://news.bbc.co.uk/1/hi/uk_politics/8080777.stm|title=Brown pressure at January 15th, 2010}}";

            Assert.AreEqual(nochange, Parsers.FixCitationTemplates(nochange));

            Assert.AreEqual(@"{{cite web | url=http://www.foo.com| title=Bar | date=January 28, 2013 | accessdate=March 7, 2013 | other=}}", 
                            Parsers.FixCitationTemplates(@"{{cite web | url=http://www.foo.com| title=Bar | date=January 28th, 2013 | accessdate=March 07, 2013 | other=}}"));
        }

        [Test]
        public void TestWordingIntoBareExternalLinks()
        {
            Assert.AreEqual(@"<ref>[http://www.nps.gov/history/nr/travel/cumberland/ber.htm B'er Chayim Temple, National Park Service]</ref>", Parsers.FixSyntax(@"<ref>B'er Chayim Temple, National Park Service, [ http://www.nps.gov/history/nr/travel/cumberland/ber.htm]</ref>"));

            // don't catch two bare links
            Assert.AreEqual(@"<ref>[http://www.nps.gov/history] [http://www.nps.gov/history/nr/travel/cumberland/ber.htm]</ref>", Parsers.FixSyntax(@"<ref>[http://www.nps.gov/history] [http://www.nps.gov/history/nr/travel/cumberland/ber.htm]</ref>"));
        }

        private static readonly System.Globalization.CultureInfo BritishEnglish = new System.Globalization.CultureInfo("en-GB");

        [Test]
        public void FixSyntaxSubstRefTags()
        {
            Assert.IsFalse(Parsers.FixSyntax(@"<ref>{{cite web | title=foo| url=http://www.site.com }} {{dead link|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}</ref>").Contains(@"{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}"), "subst converted within ref tags");
            Assert.AreEqual(@"<ref>{{cite web | title=foo| url=http://www.site.com }} {{dead link|date=" + System.DateTime.UtcNow.ToString("MMMM yyyy", BritishEnglish) + @"}}</ref>", Parsers.FixSyntax(@"<ref>{{cite web | title=foo| url=http://www.site.com }} {{dead link|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}</ref>"));
            Assert.IsTrue(Parsers.FixSyntax(@"* {{cite web | title=foo| url=http://www.site.com }} {{dead link|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}").Contains(@"{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}"), "subst not converted when outside ref tags");

            Assert.AreEqual(@"<gallery>
 Foo.JPG |Foo great{{citation needed|date=" + System.DateTime.UtcNow.ToString("MMMM yyyy", BritishEnglish) + @"}}
</gallery>", Parsers.FixSyntax(@"<gallery>
 Foo.JPG |Foo great{{citation needed|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}
</gallery>"), "subst converted within gallery tags");
        }

        [Test]
        public void FixSyntaxTemplateNamespace()
        {
            Assert.AreEqual(@"{{foo}}", Parsers.FixSyntax(@"{{Template:foo}}"));
            Assert.AreEqual(@"{{ foo}}", Parsers.FixSyntax(@"{{ Template:foo}}"));
            Assert.AreEqual(@"{{ foo}}", Parsers.FixSyntax(@"{{ template :foo}}"));
            Assert.AreEqual(@"{{
foo}}", Parsers.FixSyntax(@"{{
Template:foo}}"));
            Assert.AreEqual(@"{{foo
|bar=yes}}", Parsers.FixSyntax(@"{{Template:foo
|bar=yes}}"));
        }

        [Test]
        public void RemoveTemplateNamespace()
        {
            Assert.AreEqual(@"{{foo}}", Parsers.RemoveTemplateNamespace(@"{{Template:foo}}"));
            Assert.AreEqual(@"{{Foo}}", Parsers.RemoveTemplateNamespace(@"{{Template:Foo}}"));
            Assert.AreEqual(@"{{foo bar}}", Parsers.RemoveTemplateNamespace(@"{{Template:foo_bar}}"));
            Assert.AreEqual(@"Template:Foo", Parsers.RemoveTemplateNamespace(@"Template:Foo"), "no change if it is not a real template");
            Assert.AreEqual(@"[[Template:Foo]]", Parsers.RemoveTemplateNamespace(@"[[Template:Foo]]"), "no change if it is not a real template");
            Assert.AreEqual(@"{{Clarify|date=May 2014|reason=Use Template:Cite web or similar}}", Parsers.RemoveTemplateNamespace(@"{{Clarify|date=May 2014|reason=Use Template:Cite web or similar}}"), "no change if it is part of a comment");
            Assert.AreEqual(@"{{Foo|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}", Parsers.RemoveTemplateNamespace(@"{{Template:Foo|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"));
        }

        [Test]
        public void FixSyntaxExternalLinkBrackets()
        {
            Assert.AreEqual("[http://example.com] site", Parsers.FixSyntax("[http://example.com] site"));
            Assert.AreEqual("[http://example.com] site", Parsers.FixSyntax("[[http://example.com] site"));
            Assert.AreEqual("[http://example.com] site", Parsers.FixSyntax("[[[http://example.com] site"));
            Assert.AreEqual("[http://example.com] site", Parsers.FixSyntax("[http://example.com]] site"));
            Assert.AreEqual("[http://example.com] site", Parsers.FixSyntax("[[http://example.com]] site"));
            Assert.AreEqual("[http://example.com] site", Parsers.FixSyntax("[[[http://example.com]]] site"));
            Assert.AreEqual("[http://example.com] site", Parsers.FixSyntax("[[ http://example.com]] site"));
            Assert.AreEqual(@"[http://example.com]
* List 2", Parsers.FixSyntax(@"[[http://example.com
* List 2"));

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_10#second_pair_of_brackets_added_to_https_links
            Assert.AreEqual("[https://example.com] site", Parsers.FixSyntax("[https://example.com]] site"));
            Assert.AreEqual("[https://example.com] site", Parsers.FixSyntax("[[https://example.com] site"));

            Assert.AreEqual("<ref>[http://test.com]</ref>", Parsers.FixSyntax("<ref>[http://test.com}</ref>"));
            Assert.AreEqual("<ref>[http://test.com]</ref>", Parsers.FixSyntax("<ref>[http://test.com</ref>"));

            Assert.AreEqual("<ref>[http://test.com foo bar]</ref>", Parsers.FixSyntax("<ref>[http://test.com foo" + "\r\n" +
                                                                                      @"bar</ref>"));

            Assert.AreEqual("{{cite web | url = http://test.com |title=a }}", Parsers.FixSyntax("{{cite web | url = http:http://test.com |title=a }}"));
            Assert.AreEqual(@"* [http://www.site.com text]", Parsers.FixSyntax(@"* [http://www.site.com text)"));
            Assert.AreEqual(@"* [http://www.site.com text]", Parsers.FixSyntax(@"* [http://www.site.com text) "));

            const string Correct = @"* [http://www.site.com text (here) there]";
            Assert.AreEqual(Correct, Parsers.FixSyntax(Correct));
        }

        [Test]
        public void FixSyntaxExternalLinks()
        {
            Assert.AreEqual("[http://test.com]", Parsers.FixSyntax("[http://test.com]"),"do nothing if everything is OK");
            Assert.AreEqual("[http://test.com]", Parsers.FixSyntax("[http://http://test.com]"),"double http");
            Assert.AreEqual("[http://test.com]", Parsers.FixSyntax("[http:// http://test.com]"),"double http");
            Assert.AreEqual("[http://test.com]", Parsers.FixSyntax("[http:http://test.com]"),"double http with first lacking slashes");
            Assert.AreEqual("[http://test.com]", Parsers.FixSyntax("[http://http://http://test.com]"),"more than two");
            Assert.AreEqual("[https://test.com]", Parsers.FixSyntax("[https://https://test.com]"),"double https");
            Assert.AreEqual("[ftp://test.com]", Parsers.FixSyntax("[ftp://ftp://test.com]"),"double ftp");
            Assert.AreEqual("[ftp://test.com]", Parsers.FixSyntax("[ftp://ftp://ftp://test.com]"),"triple ftp");

            Assert.AreEqual("{{url|http://test.com}}", Parsers.FixSyntax("{{url|http://http://test.com}}"),"double http inside url template");
            Assert.AreEqual("{{official website|http://test.com}}", Parsers.FixSyntax("{{official website|http://http://test.com}}"),"double http inside official website template");
            Assert.AreEqual("{{foo|http://http://test.com}}", Parsers.FixSyntax("{{foo|http://http://test.com}}"),"no change in a random template");
        }
 
        [Test]
        public void TestFixSyntaxRemoveEmptyGallery()
        {
            Assert.AreEqual("", Parsers.FixSyntax(@"<gallery></gallery>"));
            Assert.AreEqual("", Parsers.FixSyntax(@"<gallery>
</gallery>"));
            Assert.AreEqual("", Parsers.FixSyntax(@"<gallery>   </gallery>"));
            Assert.AreEqual("", Parsers.FixSyntax(@"< Gallery >   </gallery>"));

            const string Gallery = @"<gallery>Image1.jpeg</gallery>";

            Assert.AreEqual(Gallery, Parsers.FixSyntax(Gallery));
        }

        [Test]
        public void FixSyntaxHTMLTags()
        {
            Assert.AreEqual("'''foo''' bar", Parsers.FixSyntax("<b>foo</b> bar"));
            Assert.AreEqual("'''foo''' bar", Parsers.FixSyntax("<B>foo</B> bar"));
            Assert.AreEqual("'''foo''' bar", Parsers.FixSyntax("< b >foo</b> bar"));
            Assert.AreEqual("'''foo''' bar", Parsers.FixSyntax("<b>foo< /b > bar"));
            Assert.AreEqual("<b>foo<b> bar", Parsers.FixSyntax("<b>foo<b> bar"));
            Assert.AreEqual("'''foobar'''", Parsers.FixSyntax("<b>foo</b><b>bar</b>"));

            Assert.AreEqual("''foo'' bar", Parsers.FixSyntax("<i>foo</i> bar"));
            Assert.AreEqual("''foo'' bar", Parsers.FixSyntax("< i >foo</i> bar"));
            Assert.AreEqual("''foo'' bar", Parsers.FixSyntax("< i >foo< / i   > bar"));
            Assert.AreEqual("''foo'' bar", Parsers.FixSyntax("<i>foo< /i > bar"));
            Assert.AreEqual("''foo'' bar", Parsers.FixSyntax("<i>foo<i /> bar"));
            Assert.AreEqual("''foo'' bar", Parsers.FixSyntax(@"<i>foo<i\> bar"));
            Assert.AreEqual("<i>foo<i> bar", Parsers.FixSyntax("<i>foo<i> bar"));
            Assert.AreEqual("''foo'' bar", Parsers.FixSyntax("<em>foo</em> bar"));
            Assert.AreEqual("''foobar''", Parsers.FixSyntax("<i>foo</i><i>bar</i>"));
            Assert.AreEqual("''foobar''", Parsers.FixSyntax("<i>foo<i/><i>bar</i>"));
            Assert.AreEqual("'''''foo''''' bar", Parsers.FixSyntax("<b><i>foo</i></b> bar"));

            const string EmTemplate = @"{{em|foo}}";
            Assert.AreEqual(EmTemplate, Parsers.FixSyntax(EmTemplate));
        }

        [Test]
        public void FixSyntaxStrike()
        {
            Assert.AreEqual("<s>hello</s>", Parsers.FixSyntax(@"<strike>hello</strike>"));
        }

        [Test]
        public void FixSyntaxSpacing()
        {
            Assert.AreEqual("[[Foo]]", Parsers.FixSyntax("[[Foo]]"));
            Assert.AreEqual("[[Foo Bar]]", Parsers.FixSyntax("[[Foo Bar]]"));
            Assert.AreEqual("[[Foo Bar]]", Parsers.FixSyntax("[[Foo  Bar]]"));
            Assert.AreEqual("[[Foo Bar was]]", Parsers.FixSyntax("[[Foo  Bar  was]]"), "fixes multiple double spaces in single link");
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
        public void FixSyntaxCategory()
        {
            const string correct = @"Now [[Category:2005 albums]] there";

            Assert.AreEqual(correct, Parsers.FixSyntax(@"Now {{Category:2005 albums]] there"));
            Assert.AreEqual(correct, Parsers.FixSyntax(@"Now {{Category:2005 albums}} there"));
            Assert.AreEqual(correct, Parsers.FixSyntax(@"Now {{ Category:2005 albums]] there"));
            Assert.AreEqual(correct, Parsers.FixSyntax(@"Now [[Category:2005 albums}} there"));
            Assert.AreEqual(correct, Parsers.FixSyntax(@"Now [[  Category:2005 albums}} there"));

            Assert.AreEqual(correct, Parsers.FixSyntax(correct));
        }

        [Test]
        public void FixSyntaxHTTPFormat()
        {
            Assert.AreEqual("<ref>http://www.site.com</ref>", Parsers.FixSyntax(@"<ref>http//www.site.com</ref>"),"missing colon");
            Assert.AreEqual("<ref>https://www.site.com</ref>", Parsers.FixSyntax(@"<ref>https//www.site.com</ref>"),"missing colon");
            Assert.AreEqual("<ref>http://www.site.com</ref>", Parsers.FixSyntax(@"<ref>http:://www.site.com</ref>"),"double colon");
            Assert.AreEqual("<ref>http://www.site.com</ref>", Parsers.FixSyntax(@"<ref>http:www.site.com</ref>"),"missing slashes");
            Assert.AreEqual("<ref>http://www.site.com</ref>", Parsers.FixSyntax(@"<ref>http:///www.site.com</ref>"),"triple slashes");
            Assert.AreEqual("<ref>http://www.site.com</ref>", Parsers.FixSyntax(@"<ref>http:////www.site.com</ref>"),"four slashes");
            Assert.AreEqual("at http://www.site.com", Parsers.FixSyntax(@"at http//www.site.com"));
            Assert.AreEqual("<ref>[http://www.site.com a website]</ref>",
                            Parsers.FixSyntax(@"<ref>[http:/www.site.com a website]</ref>"),"missing a slash");
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

        [Test]
        public void TestFixSyntaxExternalLinkSpacing()
        {
            Assert.AreEqual(@"their new [http://www.site.com site]", Parsers.FixSyntax(@"their new[http://www.site.com site]"));
            Assert.AreEqual(@"their new [http://www.site.com site] was", Parsers.FixSyntax(@"their new [http://www.site.com site]was"));
            Assert.AreEqual(@"their new [http://www.site.com site] was", Parsers.FixSyntax(@"their new[http://www.site.com site]was"));

            Assert.AreEqual(@"their new [http://www.site.com site]", Parsers.FixSyntax(@"their new [http://www.site.com site]"));
            Assert.AreEqual(@"their new [http://www.site.com site] was", Parsers.FixSyntax(@"their new [http://www.site.com site] was"));

            Assert.AreEqual(@"their new [http://www.site.com site] was [[blog]]ger then", Parsers.FixSyntax(@"their new [http://www.site.com site] was [[blog]]ger then"));

            const string nochange1 = @"their [[play]]ers", nochange2 = @"ts borders.<ref>[http://cyber.law.harvard.edu/filtering/a/ Saudi Arabia]</ref> A Saudi";
            Assert.AreEqual(nochange1, Parsers.FixSyntax(nochange1));
            Assert.AreEqual(nochange2, Parsers.FixSyntax(nochange2));

            // https
            Assert.AreEqual(@"their new [https://www.site.com site]", Parsers.FixSyntax(@"their new[https://www.site.com site]"));
            Assert.AreEqual(@"their new [https://www.site.com site] was", Parsers.FixSyntax(@"their new [https://www.site.com site]was"));
            Assert.AreEqual(@"their new [https://www.site.com site] was", Parsers.FixSyntax(@"their new[https://www.site.com site]was"));

            Assert.AreEqual(@"their new [https://www.site.com site]", Parsers.FixSyntax(@"their new [https://www.site.com site]"));
            Assert.AreEqual(@"their new [https://www.site.com site] was", Parsers.FixSyntax(@"their new [https://www.site.com site] was"));

            Assert.AreEqual(@"their new [https://www.site.com site] was [[blog]]ger then", Parsers.FixSyntax(@"their new [https://www.site.com site] was [[blog]]ger then"));

            const string nochange3 = @"ts borders.<ref>[https://cyber.law.harvard.edu/filtering/a/ Saudi Arabia]</ref> A Saudi";
            Assert.AreEqual(nochange3, Parsers.FixSyntax(nochange3));

#if debug
			// In Chinese Wikipedia  the text inside and outside of the link should be directly connected
            Variables.SetProjectLangCode("zh);
            Assert.AreEqual(@"their new[http://www.site.com site]", Parsers.FixSyntax(@"their new[http://www.site.com site]"));
            Assert.AreEqual(@"their new [http://www.site.com site]was", Parsers.FixSyntax(@"their new [http://www.site.com site]was"));
            Assert.AreEqual(@"their new[http://www.site.com site]was", Parsers.FixSyntax(@"their new[http://www.site.com site]was"));
            
            Variables.SetProjectLangCode("en");
            Assert.AreEqual(@"their new [http://www.site.com site]", Parsers.FixSyntax(@"their new[http://www.site.com site]"));
#endif

        }

        [Test]
        public void TestFixSyntaxReferencesWithNoHttp()
        {
            Assert.AreEqual(@"<ref>http://www.foo.com</ref>", Parsers.FixSyntax(@"<ref>www.foo.com</ref>"),"missing http");
            Assert.AreEqual(@"<ref>[http://www.foo.com bar]</ref>", Parsers.FixSyntax(@"<ref>[www.foo.com bar]</ref>"),"missing http inside brackets");
            Assert.AreEqual(@"<ref name=test>http://www.foo.com</ref>", Parsers.FixSyntax(@"<ref name=test>www.foo.com</ref>"), "missing http inside named ref");
            Assert.AreEqual(@"<ref>http://www.foo.com</ref>", Parsers.FixSyntax(@"<ref>       www.foo.com</ref>"));
            Assert.AreEqual(@"Visit www.foo.com", Parsers.FixSyntax(@"Visit www.foo.com"), "no changes outside references");
            Assert.AreEqual(@"<ref>[www-foo.a.com bar]</ref>", Parsers.FixSyntax(@"<ref>[www-foo.a.com bar]</ref>"),"No change for www-");
        }

        [Test]
        public void FixImagesBr()
        {
            Assert.AreEqual(@"[[File:Foo.png|description]]", Parsers.FixSyntax(@"[[File:Foo.png|description<br>]]"));
            Assert.AreEqual(@"[[File:Foo.png|description]]", Parsers.FixSyntax(@"[[File:Foo.png|description<br >]]"));
            Assert.AreEqual(@"[[File:Foo.png|description]]", Parsers.FixSyntax(@"[[File:Foo.png|description<br/>]]"));
            Assert.AreEqual(@"[[File:Foo.png|description]]", Parsers.FixSyntax(@"[[File:Foo.png|description<BR>]]"));
            Assert.AreEqual(@"[[File:Foo.png|description]]", Parsers.FixSyntax(@"[[File:Foo.png|description<br />]]"));
            Assert.AreEqual(@"[[File:Foo.png|description]]", Parsers.FixSyntax(@"[[File:Foo.png|description<br />
]]"));

            const string nochange1 = @"[[File:Foo.png|description<br>here]]";
            Assert.AreEqual(nochange1, Parsers.FixSyntax(nochange1));
        }

        [Test, Category("Incomplete")]
        public void TestFixSyntax()
        {
            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_3#NEsted_square_brackets_again.
            Assert.AreEqual("[[Image:foo.jpg|Some [http://some_crap.com]]]",
                            Parsers.FixSyntax("[[Image:foo.jpg|Some [http://some_crap.com]]]"));

            Assert.AreEqual("[[File:foo.jpg|Some [http://some_crap.com]]]",
                            Parsers.FixSyntax("[[File:foo.jpg|Some [http://some_crap.com]]]"));

            Assert.AreEqual("Image:foo.jpg|{{{some_crap}}}]]", Parsers.FixSyntax("Image:foo.jpg|{{{some_crap}}}]]"));

            Assert.AreEqual("[[somelink]]", Parsers.FixSyntax("[somelink]]"));
            Assert.AreEqual("[[somelink]]", Parsers.FixSyntax("[[somelink]"));
            Assert.AreNotEqual("[[somelink]]", Parsers.FixSyntax("[somelink]"));

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_7#Erroneously_removing_pipe
            Assert.AreEqual("[[|foo]]", Parsers.FixSyntax("[[|foo]]"));

            bool noChange;
            //TODO: move it to parts testing specific functions, when they're covered
            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_4#Bug_encountered_when_perusing_Sonorous_Susurrus
            Parsers.CanonicalizeTitle("[[|foo]]"); // shouldn't throw exceptions
            Assert.AreEqual("[[|foo]]", Parsers.FixLinks("[[|foo]]", "bar", out noChange));

            Assert.AreEqual(@"[[foo|bar]]", Parsers.FixSyntax(@"[[foo||bar]]"));
            Assert.AreEqual("[[somelink#a]]", Parsers.FixSyntax("[[somelink_#a]]"));
        }

        [Test]
        public void FixSyntaxDEFAULTSORT()
        {
            Assert.AreEqual(@"{{DEFAULTSORT:Foo}}", Parsers.FixSyntax(@"{{DEFAULTSORT:
Foo}}"));

            Assert.AreEqual(@"{{DEFAULTSORT:Foo}}", Parsers.FixSyntax(@"{{DEFAULTSORT: Foo }}"));
            Assert.AreEqual(@"{{DEFAULTSORT:Foo}}", Parsers.FixSyntax(@"{{DEFAULTSORT:Foo
}}"));
        }

        [Test]
        public void FixSyntaxMagicWords()
        {
            Assert.AreEqual(@"{{FULLPAGENAME:Foo}}", Parsers.FixSyntax(@"{{Fullpagename|Foo}}"));
        }

        [Test]
        public void FixSyntaxMagicWordsBehaviourSwitches()
        {
            Assert.AreEqual(@"__TOC__", Parsers.FixSyntax(@"__ToC__"));
            Assert.AreEqual(@"__TOC__", Parsers.FixSyntax(@"__toC__"));
            Assert.AreEqual(@"__TOC__", Parsers.FixSyntax(@"__TOC__"));
            Assert.AreEqual(@"__NOTOC__", Parsers.FixSyntax(@"__NoToC__"));
        }

        [Test]
        public void FixLink()
        {
            bool nochange;
            Assert.AreEqual(@"[[Foo bar]]", Parsers.FixLinks(@"[[Foo_bar]]", "a", out nochange));
            Assert.IsFalse(nochange);

            const string doubleApos = @"[[Image:foo%27%27s.jpg|thumb|200px|Bar]]";
            Assert.AreEqual(doubleApos, Parsers.FixLinks(doubleApos, "a", out nochange));

            Variables.AddUnderscoredTitles(new List<string>(new [] {"Size t", "Mod perl", "Mod mono" } ));

            Assert.AreEqual(@"[[size_t]]", Parsers.FixLinks(@"[[size_t]]", "a", out nochange));
            Assert.IsTrue(nochange);
            Assert.AreEqual(@"[[mod_perl]]", Parsers.FixLinks(@"[[mod_perl]]", "a", out nochange));
            Assert.IsTrue(nochange);
            Assert.AreEqual(@"[[mod_mono]]", Parsers.FixLinks(@"[[mod_mono]]", "a", out nochange));
            Assert.IsTrue(nochange);
            Assert.AreEqual(@"[[Mod_mono]]", Parsers.FixLinks(@"[[Mod_mono]]", "a", out nochange));
            Assert.IsTrue(nochange);
            Assert.AreEqual(@"[[E|Mod_mono]]", Parsers.FixLinks(@"[[E|Mod_mono]]", "a", out nochange));
            Assert.IsTrue(nochange);
            Assert.AreEqual(@"[[Mod_mono#link]]", Parsers.FixLinks(@"[[Mod_mono#link]]", "a", out nochange));
            Assert.IsTrue(nochange);

            Assert.AreEqual(@"[[List of The Amazing Spider-Man issues#The Amazing Spider-Man #648–current x|List of issues]]", Parsers.FixLinks(@"[[List of The Amazing Spider-Man issues#The Amazing Spider-Man #648–current x|List of issues]]", "a", out nochange), "Does not break section links with hash and space");
            Assert.AreEqual(@"[[Foo#nice example|F]]", Parsers.FixLinks(@"[[Foo#nice%20example|F]]", "a", out nochange), "%20 replaced in target");
            Assert.AreEqual(@"[[Foo|bar]]", Parsers.FixLinks(@"[[Foo|bar|]]", "a", out nochange), "Fixes excess trailing pipe");
            Assert.AreEqual(@"[[Foo|bar]]", Parsers.FixLinks(@"[[Foo|bar | ]]", "a", out nochange), "Fixes excess trailing pipe");

            Assert.AreEqual(@"[[Foo bar]]", Parsers.FixLinks(@"[[Foo_bar]]", "a", out nochange), "Fixes underscore");
            Assert.AreEqual(@"[[Foo bar#ab c]]", Parsers.FixLinks(@"[[Foo_bar#ab_c]]", "a", out nochange), "Fixes underscore");
        }

        [Test]
        public void FixLinkBoldItalic()
        {
            bool nochange;
            Assert.AreEqual(@"''[[foo|foo]]''", Parsers.FixLinks(@"[[foo|''foo'']]", "a", out nochange));
            Assert.IsFalse(nochange);
            Assert.AreEqual(@"'''[[foo|Foo]]'''", Parsers.FixLinks(@"[[foo|'''Foo''']]", "a", out nochange));
            Assert.IsFalse(nochange);

            // No change to single apostrophes
            Assert.AreEqual(@"[[foo|'bar']]", Parsers.FixLinks(@"[[foo|'bar']]", "a", out nochange));
            Assert.IsTrue(nochange);

            Assert.AreEqual(@"[[foo|]]", Parsers.FixLinks(@"[[foo|]]", "a", out nochange));
            Assert.IsTrue(nochange);
            
             Assert.AreEqual(@"[[foo|''b'']]", Parsers.FixLinks(@"[[foo|''b'']]", "a", out nochange));
            Assert.IsTrue(nochange);

            // No change to part of link text in bold/italics
            Assert.AreEqual(@"[[foo|A ''bar'']]", Parsers.FixLinks(@"[[foo|A ''bar'']]", "a", out nochange));
            Assert.IsTrue(nochange);
            Assert.AreEqual(@"[[foo|A '''bar''']]", Parsers.FixLinks(@"[[foo|A '''bar''']]", "a", out nochange));
            Assert.IsTrue(nochange);
        }

        [Test]
        public void FixSelfInterwikis()
        {
            bool nochange;

            Assert.AreEqual(@"[[Foo]]", Parsers.FixLinks(@"[[en:Foo]]", "Bar", out nochange));
            Assert.AreEqual(@"[[Foo|Bar]]", Parsers.FixLinks(@"[[en:Foo|Bar]]", "T1", out nochange));

            const string FrIW = @"Now [[fr:Here]]";
            Assert.AreEqual(FrIW, Parsers.FixLinks(FrIW, "Bar", out nochange));
            Assert.IsTrue(parser.SortInterwikis);
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
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"<br./>"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"<br /a>"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"<br /v>"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"<br /r>"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"<br /s>"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"<br /t>"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"<br /z>"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"<br /0>"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"<br /1>"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"<br /2>"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"<br /9>"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"<br /•>"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"<br/•>"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"<br /br>"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"<br ?>"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"<br?>"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"<br//>"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"</br>"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"</br >"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"</br />"));
            Assert.AreEqual("<br />", Parsers.FixSyntax(@"</br/>"));

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
             Assert.AreEqual(@"<ref>{{cite web|url=a|title=b {{rp}}}}</ref>", Parsers.FixSyntax(@"<ref>{{cite web|url=a|title=b {{rp}}}}</ref>"));

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

            Assert.AreEqual(@"<ref>{{cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref>{{cite web|url=a|title=b}}}</ref>"), "fixes cite ending in three closing braces");
            Assert.AreEqual(@"<ref>{{cite web|url=a|title=b}}
			</ref>", Parsers.FixSyntax(@"<ref>{{cite web|url=a|title=b}}}
			</ref>"), "fixes cite ending in three closing braces, newline before ref end");

            Assert.AreEqual(@"<ref>{{cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref>{{{cite web|url=a|title=b}}</ref>"), "fixes cite starting in three opening braces");
            Assert.AreEqual(@"<ref name=""foo"">{{cite web|url=a|title=b}}</ref>", Parsers.FixSyntax(@"<ref name=""foo"">{{{cite web|url=a|title=b}}</ref>"), "fixes cite starting in three opening braces");

            string CiteDeadLink = @"<ref>{{cite web|url=a|title=b}} {{dead link|date=May 2011}}</ref>";
            Assert.AreEqual(CiteDeadLink, Parsers.FixSyntax(CiteDeadLink));

            CiteDeadLink = @"<ref>{{cite web|url=a|title=b {{dead link|date=May 2011}}}}</ref>";
            Assert.AreEqual(CiteDeadLink, Parsers.FixSyntax(CiteDeadLink));
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
            Assert.AreEqual(@"<ref>[http://site.com]</ref>", Parsers.FixSyntax(@"<ref>[http://site.com</ref>"));
            Assert.AreEqual(@"<REF>[http://site.com]</ref>", Parsers.FixSyntax(@"<REF>[http://site.com</ref>"));
            Assert.AreEqual(@"<ref>[http://site.com Smith, 2004]</ref>", Parsers.FixSyntax(@"<ref>Smith, 2004 http://site.com]</ref>"));
            Assert.AreEqual(@"<ref> [http://site.com] </ref>", Parsers.FixSyntax(@"<ref> http://site.com] </ref>"));
            Assert.AreEqual(@"<ref name=Fred>[http://site.com cool]</ref>", Parsers.FixSyntax(@"<ref name=Fred>http://site.com cool]</ref>"));
            Assert.AreEqual(@"<ref name=""Fred"">[http://site.com]</ref>", Parsers.FixSyntax(@"<ref name=""Fred"">http://site.com]</ref>"));
            Assert.AreEqual(@"<ref>[http://site.com great site-here]</ref>", Parsers.FixSyntax(@"<ref>http://site.com great site-here]</ref>"));

            Assert.AreEqual(@"<ref>Antara News [http://www.antara.co.id/arc/2009/1/9/kri-lebanon/ KRI Lebanon]</ref>", Parsers.FixSyntax(@"<ref>Antara News [http://www.antara.co.id/arc/2009/1/9/kri-lebanon/ KRI Lebanon</ref>"));
            Assert.AreEqual(@"<ref>Antara News [ftp://www.antara.co.id/arc/2009/1/9/kri-lebanon/ KRI Lebanon]</ref>", Parsers.FixSyntax(@"<ref>Antara News [ftp://www.antara.co.id/arc/2009/1/9/kri-lebanon/ KRI Lebanon</ref>"), "handles FTP protocol");
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

            // []foo]] --> [[foo]]
            Assert.AreEqual(@"now [[link]] was", Parsers.FixSyntax(@"now []link]] was"));
            Assert.AreEqual(@"now [[link|a]] was", Parsers.FixSyntax(@"now []link|a]] was"));

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

            //Unbalanced bracket and double pipe [[foo||bar] inside a table
            Assert.AreEqual(@"{|
			|-
			| [[foo|bar]]
			|}", Parsers.FixSyntax(@"{|
			|-
			| [[foo||bar]
			|}"));


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

            const string Choisir = @"{{Thoroughbred
| horsename = Choisir
| image =
| caption =
| sire = [[Danehill Dancer]]
| grandsire = [[Danehill (horse)|Danehill]]
| dam = [[Great Selection]]
| damsire =
| sex =
| foaled =
| country = [[Australia|Australian]]}
| colour =
| breeder =
| owner = T
| trainer = [[Paul Perry]]
}}
'''Choisir'''";

            Assert.AreEqual(Choisir, Parsers.FixSyntax(Choisir));

            const string Nochange = @"** >> {[[Sei Young Animation Co., Ltd.|Animação Retrô]]}";

            Assert.AreEqual(Nochange, Parsers.FixSyntax(Nochange));
            
            Assert.AreEqual(@"<ref>{{cite web|url=a|title=b (ABC) }}</ref>", Parsers.FixSyntax(@"<ref>{{cite web|url=a|title=b (ABC} }}</ref>"));
        }

        [Test]
        public void FixUnbalancedBracketsRef()
        {
            Assert.AreEqual(@"now <ref>foo</ref>", Parsers.FixSyntax(@"now <ref>>foo</ref>"));
            Assert.AreEqual(@"now <ref>[http://foo.com/bar/ text here]</ref>", Parsers.FixSyntax(@"now <ref>[http://foo.com/bar/ text here[</ref>"));

            Assert.AreEqual(@"<ref>[http://www.foo.com bar]</ref>", Parsers.FixSyntax(@"<ref>]http://www.foo.com bar]</ref>"));
            Assert.AreEqual(@"<ref name=A>[http://www.foo.com bar]</ref>", Parsers.FixSyntax(@"<ref name=A>]http://www.foo.com bar]</ref>"));
            Assert.AreEqual(@"<ref>[http://www.foo.com bar] this one</ref>", Parsers.FixSyntax(@"<ref>]http://www.foo.com bar] this one</ref>"));
        }

        [Test]
        public void FixUnbalancedBracketsGeneral()
        {
            const string CorrectCat = @"[[Category:Foo]]
[[Category:Foo2]]";

            Assert.AreEqual(CorrectCat, Parsers.FixSyntax(@"[[Category:Foo
[[Category:Foo2]]"), "closes unclosed cats");
            Assert.AreEqual(CorrectCat, Parsers.FixSyntax(CorrectCat));

            Assert.AreEqual(@"[[es:Foo]]
[[fr:Foo2]]", Parsers.FixSyntax(@"[[es:Foo
[[fr:Foo2]]"), "closes unclosed interwikis");

            const string CorrectFileLink = @"[[File:foo.jpeg|eflkjfdslkj]]";

            Assert.AreEqual(CorrectFileLink, Parsers.FixSyntax(@"{{File:foo.jpeg|eflkjfdslkj]]"));
            Assert.AreEqual(CorrectFileLink, Parsers.FixSyntax(CorrectFileLink));

            const string CorrectFileLink2 = @"[[Image:foo.jpeg|eflkjfdslkj]]";

            Assert.AreEqual(CorrectFileLink2, Parsers.FixSyntax(@"{{Image:foo.jpeg|eflkjfdslkj]]"));
            Assert.AreEqual(CorrectFileLink2, Parsers.FixSyntax(CorrectFileLink2));

            const string CorrectFileLink3 = @"[[file:foo.jpeg|eflkjfdslkj]]";

            Assert.AreEqual(CorrectFileLink3, Parsers.FixSyntax(@"{{file:foo.jpeg|eflkjfdslkj]]"));
            Assert.AreEqual(CorrectFileLink3, Parsers.FixSyntax(CorrectFileLink3));

            const string NoFix1 = @"==Charact==
[[Image:X.jpg|thumb
|alt=
|xx.]]

Japanese classification systemJapanese classification systemJapanese classification systemJapanese classification systemJapanese classification systemJapanese classification systemJapanese classification system

<gallery>
Image:X.JPG|Japanese classification systemJapanese classification systemJapanese classification system]]
</gallery>";

            Assert.AreEqual(NoFix1, Parsers.FixSyntax(NoFix1));
            
            Assert.AreEqual(@"[[Foo]]", Parsers.FixSyntax(@"[ [Foo]]"), "fixes link spacing");
            
            Assert.AreEqual(@"[[Foo]]", Parsers.FixSyntax(@"[[Foo]]]]"), "fixes excess link bracketss");
            
            Assert.AreEqual(@"[[Foo]],", Parsers.FixSyntax(@"[[Foo],]"), "fixes links broken by punctuation");
            Assert.AreEqual(@"[[Foo]].", Parsers.FixSyntax(@"[[Foo].]"), "fixes links broken by punctuation");
            Assert.AreEqual(@"[[Foo]]:", Parsers.FixSyntax(@"[[Foo]:]"), "fixes links broken by punctuation");
            Assert.AreEqual(@"[[Foo]]  bar", Parsers.FixSyntax(@"[[Foo] ] bar"), "fixes links broken by punctuation");
            Assert.AreEqual(@"[[Foo]]''", Parsers.FixSyntax(@"[[Foo]'']"), "fixes links broken by punctuation");

            Assert.AreEqual(@"[[panka Smith]] (Local national)", Parsers.FixSyntax(@"[panka  Smith]] (Local national)"), "bracket and whitespace fix in one");

            const string Football = @"{{Infobox football biography
| playername     = D
| image          = 
| dateofdeath    = 1940 {aged 57)<ref name=A/>
| cityofdeath    = [[S]]
| years3         = 1911–19?? }}";

            Assert.AreEqual(Football.Replace(@"{aged", @"(aged"), Parsers.FixSyntax(Football));
            Assert.AreEqual(@"{{DEFAULTSORT:Foo}}", Parsers.FixSyntax(@"{{DEFAULTSORT:Foo
"), "fixes DEFAULTSORT ending");
            
            Assert.AreEqual(@"{{foo|par=[[Bar]]|par2=Bar2}}", Parsers.FixSyntax(@"{{foo|par=[[Bar[[|par2=Bar2}}"), "reversed brackets");

            Assert.AreEqual(@"{{foo|par=[[Bar]]|par2=Bar2}}", Parsers.FixSyntax(@"{{foo|par=[{Bar]]|par2=Bar2}}"));

            const string Unfixable1 = @"Ruth singled and Meusel [[bunt (baseball)|ed him over, but Ruth split his pants sliding into second, [[announcer|Radio announcer]] [[Graham McNamee]]";
            Assert.AreEqual(Unfixable1, Parsers.FixSyntax(Unfixable1));

             Assert.AreEqual(@"{{DEFAULTSORT:Foo}}", Parsers.FixSyntax(@"{{DEFAULTSORT:Foo))"), "fixes Template )) ending");
             const string Unfixable2 = @"Ruth [[File:One.JPEG|A [http://www.site.com/a]]]
XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
Now [[Fred was";
             Assert.AreEqual(Unfixable2, Parsers.FixSyntax(Unfixable2));

             Assert.AreEqual(Unfixable2 + @"
==Heading==
Now [[A]], was.", Parsers.FixSyntax(Unfixable2 + @"
==Heading==
Now [[A],] was."));
        }

        [Test]
        public void FixUnbalancedBracketsMathSetNotation()
        {
            const string MathSet1 = @"{[0], [1], [2]}", Foo = @"Foo { ...";
            Assert.AreEqual(Foo + MathSet1, Parsers.FixSyntax(Foo + MathSet1));
        }

        [Test]
        public void FixUnbalancedBracketsChineseBrackets()
        {
        	#if DEBUG
        	const string CB = @"now （there) was";

        	Variables.SetProjectLangCode("fr");
        	Assert.AreEqual(CB, Parsers.CiteTemplateDates(CB));

        	Variables.SetProjectLangCode("en");
        	Assert.AreEqual(@"now (there) was", Parsers.FixSyntax(CB));

        	const string CB2 = @"now （there） was";
        	Assert.AreEqual(CB2, Parsers.FixSyntax(CB2), "No change when brackets are balanced");
        	#endif
        }
        
        [Test]
        public void FixUnbalancedBracketsPersondataEnd()
        {
            const string PD = @"{{Persondata<!--Metadata: see [[Wikipedia:Persondata]].-->
|NAME=Orbe, Georgy
|ALTERNATIVE NAMES=
|SHORT DESCRIPTION=
|DATE OF BIRTH=1873
|PLACE OF BIRTH=[[Tbilisi]]
|DATE OF DEATH=1944
|PLACE OF DEATH=Paris

{{DEFAULTSORT:Orbe, Georgy}}";
            
            Assert.AreEqual(PD.Replace(@"Paris", @"Paris}}"), Parsers.FixSyntax(PD));
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
            Assert.AreEqual(@"{{cite web|url=http://members.bib-arch.org|title=hello | work=BBC}}", Parsers.FixCitationTemplates(@"{{cite web|URL=http://members.bib-arch.org|TITLE=hello | Work=BBC}}"));

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

            // ISBN, DOI, PMID, PMC, LCCN is allowed to be uppercase
            string ISBN = @"{{cite book | author=Smith | title=Great Book | ISBN=15478454 | date=17 May 2004 }}";
            Assert.AreEqual(ISBN, Parsers.FixCitationTemplates(ISBN));
            string ISSN = @"{{cite book | author=Smith | title=Great Book | ISSN=15478454 | date=17 May 2004 }}";
            Assert.AreEqual(ISSN, Parsers.FixCitationTemplates(ISSN));
            string OCLC = @"{{cite book | author=Smith | title=Great Book | OCLC=15478454 | date=17 May 2004 }}";
            Assert.AreEqual(OCLC, Parsers.FixCitationTemplates(OCLC));
            string DOI = @"{{cite journal| author=Smith | title=Great Book | DOI=15478454 | date=17 May 2004 }}";
            Assert.AreEqual(DOI, Parsers.FixCitationTemplates(DOI));
            string PMID = @"{{cite journal| author=Smith | title=Great Book | PMID=15478454 | date=17 May 2004 }}";
            Assert.AreEqual(PMID, Parsers.FixCitationTemplates(PMID));
            string PMC = @"{{cite journal| author=Smith | title=Great Book | PMC=15478454 | date=17 May 2004 }}";
            Assert.AreEqual(PMC, Parsers.FixCitationTemplates(PMC));
            string LCCN = @"{{cite journal| author=Smith | title=Great Book | PMC=15478454 | date=17 May 2004 }}";
            Assert.AreEqual(LCCN, Parsers.FixCitationTemplates(LCCN));

            // don't match on part of URL
            string URL = @"{{cite news|url=http://www.expressbuzz.com/edition/story.aspx?Title=Catching++them+young&artid=rPwTAv2l2BY=&SectionID=fxm0uEWnVpc=&MainSectionID=ngGbWGz5Z14=&SectionName=RtFD/|pZbbWSsbI0jf3F5Q==&SEO=|title=Catching them young|date=August 7, 2009|work=[[The Indian Express]]|accessdate=2009-08-07}}";
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
        public void FixDeadlinkOutsideRef()
        {
        	Assert.AreEqual("<ref>foo {{dead link|date=July 2014}}</ref> boo", Parsers.FixSyntax(@"<ref>foo</ref> {{dead link|date=July 2014}} boo"), "only {{dead link}} taken inside ref");
        	Assert.AreEqual("<ref>foo {{Dead link | date=July 2014 }}</ref> boo", Parsers.FixSyntax(@"<ref>foo</ref> {{Dead link | date=July 2014 }} boo"), "only {{dead link}} taken inside ref");
        	Assert.AreEqual("<ref>foo {{Dead link|date=July 2014}}</ref> boo", Parsers.FixSyntax(@"<ref>foo</ref> {{Dead link|date=July 2014}} boo"), "only {{dead link}} taken inside ref");        	Assert.AreEqual("<ref>{{cite web | url=http://www.site.com/article100.html | title=Foo }} {{dead link|date=July 2014}}</ref>", Parsers.FixSyntax(@"<ref>{{cite web | url=http://www.site.com/article100.html | title=Foo }}</ref> {{dead link|date=July 2014}}"), "{{dead link}} taken inside ref");
        }
        
        [Test]
        public void FixCitationTemplatesDeadLinkInFormat()
        {
            const string correct = @"{{cite web | url=http://www.site.com/article100.html | title=Foo }} {{dead link|date=May 2010}}";
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"{{cite web | url=http://www.site.com/article100.html | title=Foo | format= {{dead link|date=May 2010}}}}"), "{{dead link}} taken out of format field");

            Assert.AreEqual(correct.Replace("Foo", "Foo | format= PDF"), Parsers.FixCitationTemplates(@"{{cite web | url=http://www.site.com/article100.html | title=Foo | format= PDF {{dead link|date=May 2010}}}}"), "Only {{dead link}} taken out of format field");

            Assert.AreEqual(correct, Parsers.FixCitationTemplates(correct), "no change when already correct");

            const string NodDead = @"{{cite web | url=http://www.site.com/article100.html | title=Foo | format= PDF}}";
            Assert.AreEqual(NodDead, Parsers.FixCitationTemplates(NodDead), "no change when no dead link in format field");
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

            const string PubMed = @"now {{cite journal | journal=BMJ | title=hello|date = 2008-Dec-11 }} was";

            Assert.AreEqual(PubMed, Parsers.CiteTemplateDates(PubMed), "no change to PubMed date format in scientific journal cite");

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

            Assert.AreEqual(@"<ref>{{cite web|url=http://www.nuclearweaponarchive.org/Russia/TsarBomba.html|title=The Tsar Bomba (King of Bombs)|accessdate=2010-12}}</ref>", Parsers.CiteTemplateDates(@"<ref>{{cite web|url=http://www.nuclearweaponarchive.org/Russia/TsarBomba.html|title=The Tsar Bomba (King of Bombs)|accessdate=2010-12}}</ref>"));


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
            Assert.AreEqual("{{cite web|date=2008-12-11|accessdate=2008-08-07}}", Parsers.CiteTemplateDates("{{cite web|date=2008-December-11|accessdate=2008-Aug.-07}}"));
            Assert.AreEqual("{{cite web|date=2008-12-11|accessdate=2008-08-07}}", Parsers.CiteTemplateDates("{{cite web|date=2008-Dec.-11|accessdate=2008-Aug.-07}}"));

            // don't apply fixes when ambiguous dates present
            string ambig = @"now {{cite web | url=http://site.it | title=hello|date = 5-4-1998}} was
now {{cite web | url=http://site.it | title=hello|date = 5-5-1998}} was";

            Assert.AreEqual(ambig, Parsers.CiteTemplateDates(ambig));

            // no change on YYYY-MM format
            string Y4M2 = @"now {{cite web | url=http://site.it | title=hello|date = 2010-03 }} was";
            Assert.AreEqual(Y4M2, Parsers.CiteTemplateDates(Y4M2));

            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-02-15 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 2/15/2008 }} was"));
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|date = 2008-02-15 }} was", Parsers.CiteTemplateDates(@"now {{cite web | url=http://site.it | title=hello|date = 02/15/2008 }} was"));

            // no change on year range
            string YearRange = @"{{cite web | url=http://site.it | title=hello|date = 1910-1911 }}";
            Assert.AreEqual(YearRange, Parsers.CiteTemplateDates(YearRange));
        }

        [Test]
        public void CiteTemplateDatesTimeStamp()
        {
            string correctpart = @"now {{cite web | url=http://site.it | title=hello|accessdate = 2008-03-25";

            string datestamp = @" 12:12:54 BST";
            Assert.AreEqual(correctpart + @"<!--" + datestamp + @"-->}} was", Parsers.CiteTemplateDates(correctpart + datestamp + @"}} was"));
            Assert.AreEqual(correctpart + @"<!--" + datestamp + @"-->
}} was", Parsers.CiteTemplateDates(correctpart + datestamp + @"
}} was")); // end whitespace handling

            datestamp = @" 12:30 BST";
            Assert.AreEqual(correctpart + @"<!--" + datestamp + @"-->}} was", Parsers.CiteTemplateDates(correctpart + datestamp + @"}} was"));

            datestamp = @" 12:30 CET";
            Assert.AreEqual(correctpart + @"<!--" + datestamp + @"-->}} was", Parsers.CiteTemplateDates(correctpart + datestamp + @"}} was"));

            datestamp = @" 12:30 GMT, 13:30 RST";
            Assert.AreEqual(correctpart + @"<!--" + datestamp + @"-->}} was", Parsers.CiteTemplateDates(correctpart + datestamp + @"}} was"));

            datestamp = @" 12:30 GMT, 13:30 [[RST]]";
            Assert.AreEqual(correctpart + @"<!--" + datestamp + @"-->}} was", Parsers.CiteTemplateDates(correctpart + datestamp + @"}} was"));

            datestamp = @" 12:30 GMT, 13:30 [[Foo|RST]]";
            Assert.AreEqual(correctpart + @"<!--" + datestamp + @"-->}} was", Parsers.CiteTemplateDates(correctpart + datestamp + @"}} was"));

            datestamp = @" 12.30 BST";
            Assert.AreEqual(correctpart + @"<!--" + datestamp + @"-->}} was", Parsers.CiteTemplateDates(correctpart + datestamp + @"}} was"));
            Assert.AreEqual(correctpart.Replace("2018-03-25", "25 March 2018") + @"<!--" + datestamp + @"-->}} was", Parsers.CiteTemplateDates(correctpart.Replace("2018-03-25", "25 March 2018") + datestamp + @"}} was"));
            Assert.AreEqual(correctpart.Replace("2018-03-25", "March 25, 2018") + @"<!--" + datestamp + @"-->}} was", Parsers.CiteTemplateDates(correctpart.Replace("2018-03-25", "March 25, 2018") + datestamp + @"}} was"));
            Assert.AreEqual(correctpart.Replace("2018-03-25", "March 25th, 2018") + @"<!--" + datestamp + @"-->}} was", Parsers.CiteTemplateDates(correctpart.Replace("2018-03-25", "March 25th, 2018") + datestamp + @"}} was"));

            const string YearList = @"{{cite web | url=http://www.site.com | title=a | date=2004, 2006, 2009 }}";
            Assert.AreEqual(YearList, Parsers.CiteTemplateDates(YearList));
        }

        [Test]
        public void CiteTemplateDatesEnOnly()
        {
#if DEBUG
            const string bad = @"now {{cite web | url=http://site.it | title=hello|accessdate = 25/03/2008 }} was";

            Variables.SetProjectLangCode("fr");
            Assert.AreEqual(bad, Parsers.CiteTemplateDates(bad));

            Variables.SetProjectLangCode("en");
            Assert.AreEqual(@"now {{cite web | url=http://site.it | title=hello|accessdate = 2008-03-25 }} was", Parsers.CiteTemplateDates(bad));
#endif
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
        public void AmbiguousCiteTemplateDates2()
        {
            Dictionary<int, int> ambigDates = new Dictionary<int, int>();

            ambigDates = Parsers.AmbigCiteTemplateDates(@"now {{cite web | url=http://site.it | title=a |date=7-6-2005 }}");

            Assert.AreEqual(ambigDates.Count, 1);
            Assert.IsTrue(ambigDates.ContainsKey(52));
            Assert.IsTrue(ambigDates.ContainsValue(8));
        }

        [Test]
        public void HasSeeAlsoAfterNotesReferencesOrExternalLinks()
        {
            const string start = @"start
", seeAlso = @"Foo
==See also==
x
", extlinks = @"==External links==
x
", notes = @"==Notes==
x
", references = @"== References==
x
";
            Assert.IsFalse(Parsers.HasSeeAlsoAfterNotesReferencesOrExternalLinks(""));
            Assert.IsFalse(Parsers.HasSeeAlsoAfterNotesReferencesOrExternalLinks(start + seeAlso));
            Assert.IsFalse(Parsers.HasSeeAlsoAfterNotesReferencesOrExternalLinks(start + seeAlso + extlinks));

            Assert.IsTrue(Parsers.HasSeeAlsoAfterNotesReferencesOrExternalLinks(start + extlinks + seeAlso));
            Assert.IsTrue(Parsers.HasSeeAlsoAfterNotesReferencesOrExternalLinks(start + notes + seeAlso));
            Assert.IsTrue(Parsers.HasSeeAlsoAfterNotesReferencesOrExternalLinks(start + references + seeAlso));
            Assert.IsTrue(Parsers.HasSeeAlsoAfterNotesReferencesOrExternalLinks(start + references + seeAlso + notes));
        }

        [Test]
        public void UnclosedTags()
        {
            Dictionary<int, int> uct = new Dictionary<int, int>();

            uct = Parsers.UnclosedTags(@"<pre>bar</pre>");
            Assert.AreEqual(uct.Count, 0);

            uct = Parsers.UnclosedTags(@"<gallery>File:bar</gallery>");
            Assert.AreEqual(uct.Count, 0);

            uct = Parsers.UnclosedTags(@"<math>bar</math>");
            Assert.AreEqual(uct.Count, 0);

            uct = Parsers.UnclosedTags(@"<source>bar</source> <ref name=Foo>boo</ref>");
            Assert.AreEqual(uct.Count, 0);

            uct = Parsers.UnclosedTags(@"<pre>bar</pre> <ref name=Foo>boo</ref>");
            Assert.AreEqual(uct.Count, 0);

            uct = Parsers.UnclosedTags(@"<pre>bar</pre> <math> not ended");
            Assert.AreEqual(uct.Count, 1);
            Assert.IsTrue(uct.ContainsKey(15));
            Assert.IsTrue(uct.ContainsValue(6));

            uct = Parsers.UnclosedTags(@"<pre>bar</pre> <ref name=<ref name=Foo/> not ended");
            Assert.AreEqual(uct.Count, 1);
            Assert.IsTrue(uct.ContainsKey(15));
            Assert.IsTrue(uct.ContainsValue(35));

            uct = Parsers.UnclosedTags(@"<pre>bar</pre> <ref> not ended");
            Assert.AreEqual(uct.Count, 1);
            Assert.IsTrue(uct.ContainsKey(15));
            Assert.IsTrue(uct.ContainsValue(5));

            uct = Parsers.UnclosedTags(@"<pre>bar</pre> <ref name='foo'> not ended");
            Assert.AreEqual(uct.Count, 1);
            Assert.IsTrue(uct.ContainsKey(15));
            Assert.IsTrue(uct.ContainsValue(16));

            uct = Parsers.UnclosedTags(@"<pre>bar</pre> <source lang=""bar""> not ended");
            Assert.AreEqual(uct.Count, 1);
            Assert.IsTrue(uct.ContainsKey(15));

            uct = Parsers.UnclosedTags(@"<pre>bar</pre> <small> not ended");
            Assert.AreEqual(uct.Count, 1);
            Assert.IsTrue(uct.ContainsKey(15));

            uct = Parsers.UnclosedTags(@"<pre>bar</pre> < code> not ended");
            Assert.AreEqual(uct.Count, 1);
            Assert.IsTrue(uct.ContainsKey(15));

            uct = Parsers.UnclosedTags(@"<pre>bar</pre> < nowiki > not ended");
            Assert.AreEqual(uct.Count, 1);
            Assert.IsTrue(uct.ContainsKey(15));

            uct = Parsers.UnclosedTags(@"<pre>bar</pre> <pre> not ended");
            Assert.AreEqual(uct.Count, 1);
            Assert.IsTrue(uct.ContainsKey(15));

            uct = Parsers.UnclosedTags(@"<pre>bar</pre> </pre> not opened");
            Assert.AreEqual(uct.Count, 1);
            Assert.IsTrue(uct.ContainsKey(15));

            uct = Parsers.UnclosedTags(@"<pre>bar</pre> <gallery> not ended");
            Assert.AreEqual(uct.Count, 1);
            Assert.IsTrue(uct.ContainsKey(15));

            uct = Parsers.UnclosedTags(@"<pre>bar</pre> </gallery> not opened");
            Assert.AreEqual(uct.Count, 1);
            Assert.IsTrue(uct.ContainsKey(15));

            uct = Parsers.UnclosedTags(@"<pre>bar</pre> <!-- not ended");
            Assert.AreEqual(uct.Count, 1);
            Assert.IsTrue(uct.ContainsKey(15));

            uct = Parsers.UnclosedTags(@"<!--bar--> <!-- not ended");
            Assert.AreEqual(uct.Count, 1);
            Assert.IsTrue(uct.ContainsKey(11));

            uct = Parsers.UnclosedTags(@"<gallery> not ended <gallery>bar</gallery>");
            Assert.AreEqual(uct.Count, 1);
            Assert.IsTrue(uct.ContainsKey(20));

            uct = Parsers.UnclosedTags(@"<gallery other='a'> not ended <gallery other='a'>bar</gallery>");
            Assert.AreEqual(uct.Count, 1);
            Assert.IsTrue(uct.ContainsKey(30));

            uct = Parsers.UnclosedTags(@"<gallery>A|<div><small>(1717)</small><br/><small><small>Munich</small></div></gallery>");
            Assert.AreEqual(uct.Count, 1);
            Assert.IsTrue(uct.ContainsKey(42));
        }

        [Test]
        public void ExtraBracketInExternalLink()
        {
            //https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_9#Bug_in_regex_to_correct_double_bracketed_external_links
            const string valid = "now [http://www.site.com a [[a]] site] was";
            Assert.AreEqual(valid, Parsers.FixSyntax(valid));  // valid syntax
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
            Assert.AreEqual("[[File:foo.jpg|Some [https://some_crap.com]]]", Parsers.FixSyntax("[[File:foo.jpg|Some [[https://some_crap.com]]]]"));
        }

        [Test]
        public void SquareBracketsInExternalLinksTests()
        {
            Assert.AreEqual(@"[http://www.site.com some stuff &#91;great&#93;]", Parsers.FixSyntax(@"[http://www.site.com some stuff [great]]"));
            Assert.AreEqual(@"[http://www.site.com some stuff &#91;great&#93; free]", Parsers.FixSyntax(@"[http://www.site.com some stuff [great] free]"));
            Assert.AreEqual(@"[http://www.site.com some stuff &#91;great&#93; free &#91;here&#93;]", Parsers.FixSyntax(@"[http://www.site.com some stuff [great] free [here]]"));
            Assert.AreEqual(@"[http://www.site.com some stuff &#91;great&#93; free [[now]]]", Parsers.FixSyntax(@"[http://www.site.com some stuff [great] free [[now]]]"));

            Assert.AreEqual(@"*This: [http://www.privacyinternational.org/article.shtml?cmd&#91;347&#93;=x-347-359656&als&#91;theme&#93;=AT+Law+and+Policy Terrorism Profile - Uganda] Privacy International",
                            Parsers.FixSyntax(@"*This: [http://www.privacyinternational.org/article.shtml?cmd[347]=x-347-359656&als[theme]=AT+Law+and+Policy Terrorism Profile - Uganda] Privacy International"));

            // no delinking needed
            Assert.AreEqual(@"[http://www.site.com some great stuff]", Parsers.FixSyntax(@"[http://www.site.com some great stuff]"));
            Assert.AreEqual(@"now [http://www.site.com some great stuff] here", Parsers.FixSyntax(@"now [http://www.site.com some great stuff] here"));
            Assert.AreEqual(@"[http://www.site.com some [[great]] stuff]", Parsers.FixSyntax(@"[http://www.site.com some [[great]] stuff]"));
            Assert.AreEqual(@"[http://www.site.com some great [[stuff]]]", Parsers.FixSyntax(@"[http://www.site.com some great [[stuff]]]"));
            Assert.AreEqual(@"[http://www.site.com [[some great stuff|loads of stuff]]]", Parsers.FixSyntax(@"[http://www.site.com [[some great stuff|loads of stuff]]]"));
            Assert.AreEqual(@"[http://www.site.com [[some great stuff|loads of stuff]] here]", Parsers.FixSyntax(@"[http://www.site.com [[some great stuff|loads of stuff]] here]"));

            // don't match beyond </ref> tags
            Assert.AreEqual(@"[http://www.site.com some </ref> [stuff] here]", Parsers.FixSyntax(@"[http://www.site.com some </ref> [stuff] here]"));

            // exception handling
            Assert.AreEqual(@"* [http://www.site.com some stuff
* [http://site2.com stuff 2]
Other
]", Parsers.FixSyntax(@"* [http://www.site.com some stuff
* [http://site2.com stuff 2]
Other
]"));
        }

        [Test]
        public void TestBracketsAtCiteTemplateURL()
        {
            const string correct = @"now {{cite web|url=http://site.net|title=hello}} was";

            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"now {{cite web|url=[http://site.net]|title=hello}} was"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"now {{cite web|url=http://site.net]|title=hello}} was"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"now {{cite web|url=[http://site.net|title=hello}} was"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"now {{cite web|url=[[http://site.net]|title=hello}} was"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"now {{cite web|url=[http://site.net]]|title=hello}} was"));
            Assert.AreEqual(correct, Parsers.FixCitationTemplates(@"now {{cite web|url=[[http://site.net]]|title=hello}} was"));
            Assert.AreEqual(@"now {{cite web|url = http://site.net  |title=hello}} was", Parsers.FixCitationTemplates(@"now {{cite web|url = [http://site.net]  |title=hello}} was"));
            Assert.AreEqual(@"now {{cite web|title=hello |url=www.site.net}} was", Parsers.FixCitationTemplates(@"now {{cite web|title=hello |url=[www.site.net]}} was"));
            Assert.AreEqual(@"now {{cite journal|title=hello | url=http://site.net }} was", Parsers.FixCitationTemplates(@"now {{cite journal|title=hello | url=[http://site.net]] }} was"));

            // no match
            Assert.AreEqual(@"now {{cite web| url=http://site.net|title=hello}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=http://site.net|title=hello}} was"));
            Assert.AreEqual(@"now {{cite web| url=[http://site.net cool site]|title=hello}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=[http://site.net cool site]|title=hello}} was"));
            Assert.AreEqual(@"now {{cite web| url=http://site.net cool site]|title=hello}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=http://site.net cool site]|title=hello}} was"));
            Assert.AreEqual(@"now {{cite web| url=[http://site.net cool site|title=hello}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=[http://site.net cool site|title=hello}} was"));
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

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_2#Incorrect_bulleting
            StringAssert.Contains("\r\nhttp://example.com }}", s);
            
            string NoChange = @"==External links==
* http://example.com/foo";
            Assert.AreEqual(NoChange, Parsers.BulletExternalLinks(NoChange));
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

            Assert.AreEqual("[[foo#bar]]", Parsers.FixLinkWhitespace("[[foo #bar]]", "foot"));
            Assert.AreEqual("[[foo#bar]]", Parsers.FixLinkWhitespace("[[foo # bar]]", "foot"));
            Assert.AreEqual("[[foo#bar]]", Parsers.FixLinkWhitespace("[[foo# bar]]", "foot"));
            Assert.AreEqual("[[foo#bar]]te", Parsers.FixLinkWhitespace("[[foo #bar]]te", "foot"));
            Assert.AreEqual("[[foo#bar|te]]", Parsers.FixLinkWhitespace("[[foo  #bar|te]]", "foot"));

            Assert.AreEqual(@"[[List of The Amazing Spider-Man issues#The Amazing Spider-Man #648–current x|List of issues]]", Parsers.FixLinkWhitespace(@"[[List of The Amazing Spider-Man issues#The Amazing Spider-Man #648–current x|List of issues]]", "x"), "Does not break section links with hash and space");

            Assert.AreEqual("[[A# code]]", Parsers.FixLinkWhitespace("[[A# code]]", "Test"));
            Assert.AreEqual("[[C# code]]", Parsers.FixLinkWhitespace("[[C# code]]", "Test"));
            Assert.AreEqual("[[F# code]]", Parsers.FixLinkWhitespace("[[F# code]]", "Test"));
            Assert.AreEqual("[[J# code]]", Parsers.FixLinkWhitespace("[[J# code]]", "Test"));
        }

        [Test]
        public void TestCanonicalizeTitle()
        {
            Assert.AreEqual("foo bar", Parsers.CanonicalizeTitle("foo_bar"));
            Assert.AreEqual("foo bar", Parsers.CanonicalizeTitle("foo bar"));
            Assert.AreEqual("foo (bar)", Parsers.CanonicalizeTitle("foo_%28bar%29"));
            Assert.AreEqual(@"foo+bar", Parsers.CanonicalizeTitle(@"foo%2Bbar"));
            Assert.AreEqual("foo (bar)", Parsers.CanonicalizeTitle("foo_(bar)"));
            Assert.AreEqual("Template:foo bar", Parsers.CanonicalizeTitle("Template:foo_bar"));

            Assert.AreEqual("foo_bar}}", Parsers.CanonicalizeTitle("foo_bar}}"), "no change to invalid title");

            // it may or may not fix it, but shouldn't break anything
            StringAssert.Contains("{{bar_boz}}", Parsers.CanonicalizeTitle("foo_bar{{bar_boz}}"));

            Variables.UnderscoredTitles.Add(@"Foo bar");
            Assert.AreEqual("foo_bar", Parsers.CanonicalizeTitle("foo_bar"));
            Assert.AreEqual("Foo_bar", Parsers.CanonicalizeTitle("Foo_bar"));
        }

        [Test]
        public void CanonicalizeTitleAggressively()
        {
            Assert.AreEqual("Foo", Parsers.CanonicalizeTitleAggressively("Foo"));

            Assert.AreEqual("Foo (bar)", Parsers.CanonicalizeTitleAggressively("foo_%28bar%29#anchor"));
            Assert.AreEqual("Wikipedia:Foo", Parsers.CanonicalizeTitleAggressively("project : foo"));
            Assert.AreEqual("File:Foo.jpg", Parsers.CanonicalizeTitleAggressively("Image: foo.jpg "));

            // a bit of ambiguousness here, but
            // https://en.wikipedia.org/wiki/Wikipedia:AWB/B#Problem_.28on_runecape_wikia.29_with_articles_with_.2B_in_the_name.
            Assert.AreEqual("Romeo+Juliet", Parsers.CanonicalizeTitleAggressively("Romeo+Juliet"));

            Assert.AreEqual("Foo", Parsers.CanonicalizeTitleAggressively(":Foo"));
            Assert.AreEqual("Foo", Parsers.CanonicalizeTitleAggressively(": Foo"));
            Assert.AreEqual(":Foo", Parsers.CanonicalizeTitleAggressively("::Foo"));
            Assert.AreEqual("User:Foo", Parsers.CanonicalizeTitleAggressively(":user:Foo"));

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs#List_of_.22User_talk:.22_pages_change_to_list_of_.22Talk:.22_pages_when_started
            Assert.AreEqual("User talk:Foo", Parsers.CanonicalizeTitleAggressively("User talk:Foo"));
        }

        [Test]
        public void TestFixCategories()
        {
            Assert.AreEqual("[[Category:Foo bar]]", Parsers.FixCategories("[[ categOry : Foo_bar]]"));
            Assert.AreEqual("[[Category:Foo bar|boz]]", Parsers.FixCategories("[[ categOry : Foo_bar|boz]]"));
            Assert.AreEqual("[[Category:Foo bar|quux]]", Parsers.FixCategories("[[category : foo_bar%20|quux]]"));
            Assert.AreEqual(@"[[Category:Foo bar]]", Parsers.FixCategories(@"[[Category:Foo_bar]]"));
            Assert.AreEqual(@"[[Category:Foo bar]]", Parsers.FixCategories(@"[[category:Foo bar]]"));
            Assert.AreEqual(@"[[Category:Foo bar]]", Parsers.FixCategories(@"[[Category::Foo bar]]"));
            Assert.AreEqual(@"[[Category:Foo bar]]", Parsers.FixCategories(@"[[Category  :  Foo_bar  ]]"));
            Assert.AreEqual("[[Category:Foo bar]]", Parsers.FixCategories("[[CATEGORY: Foo_bar]]"));


            // https://en.wikipedia.org/w/index.php?title=Wikipedia_talk:AutoWikiBrowser/Bugs&oldid=262844859#General_fixes_remove_spaces_from_category_sortkeys
            Assert.AreEqual(@"[[Category:Public transport in Auckland| Public transport in Auckland]]", Parsers.FixCategories(@"[[Category:Public transport in Auckland| Public transport in Auckland]]"));
            Assert.AreEqual(@"[[Category:Actors|Fred Astaire]]", Parsers.FixCategories(@"[[Category:Actors|Fred Astaire ]]"), "trailing space IS removed");
            Assert.AreEqual(@"[[Category:Actors|Fred Astaire]]", Parsers.FixCategories(@"[[Category:Actors|Fred Astaire    ]]"), "trailing space IS removed"); 
            Assert.AreEqual(@"[[Category:Actors| Fred Astaire]]", Parsers.FixCategories(@"[[Category:Actors| Fred Astaire ]]"), "trailing space IS removed");
            Assert.AreEqual(@"[[Category:London| ]]", Parsers.FixCategories(@"[[Category:London| ]]"), "leading space NOT removed");
            Assert.AreEqual(@"[[Category:Slam poetry| ]] ", Parsers.FixCategories(@"[[Category:Slam poetry| ]] "), "leading space NOT removed");

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Archive_18#.2Fdoc_pages_and_includeonly_sections
            Assert.AreEqual("[[Category:Foo bar|boz_quux]]", Parsers.FixCategories("[[Category: foo_bar |boz_quux]]"));
            Assert.AreEqual("[[Category:Foo bar|{{boz_quux}}]]", Parsers.FixCategories("[[Category: foo_bar|{{boz_quux}}]]"));
            StringAssert.Contains("{{{boz_quux}}}", Parsers.FixCategories("[[CategorY : foo_bar{{{boz_quux}}}]]"));
            Assert.AreEqual("[[Category:Foo bar|{{{boz_quux}}}]]", Parsers.FixCategories("[[CategorY : foo_bar|{{{boz_quux}}}]]"));

            // diacritics removed from sortkeys
            Assert.AreEqual(@"[[Category:World Scout Committee members|Laine, Juan]]", Parsers.FixCategories(@"[[Category:World Scout Committee members|Lainé, Juan]]"));

            Variables.Namespaces.Remove(Namespace.Category);
            Assert.AreEqual("", Parsers.FixCategories(""), "Fallback to English category namespace name");
            Variables.Namespaces.Add(Namespace.Category,"Category:");
        }

        [Test]
        public void TestFixCategoriesRu()
        {
#if debug
            Variables.SetProjectLangCode("ru");
            Assert.AreEqual(@"[[Category:World Scout Committee members|Lainé, Juan]]", Parsers.FixCategories(@"[[Category:World Scout Committee members|Lainé, Juan]]"), "no diacritic removal for sort key on ru-wiki");
            
            Variables.SetProjectLangCode("en");
            Assert.AreEqual(@"[[Category:World Scout Committee members|Laine, Juan]]", Parsers.FixCategories(@"[[Category:World Scout Committee members|Lainé, Juan]]"));
#endif
        }

        [Test]
        public void TestFixCategoriesBrackets()
        {
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
            Assert.AreEqual("", Parsers.FixEmptyLinksAndTemplates("[[[[  ]]]]"));
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
        public void TestFixPercent()
        {
            Assert.AreEqual(@"a 15% ", parser.FixNonBreakingSpaces(@"a 15 % "), "remove space");
            Assert.AreEqual(@"a -15% ", parser.FixNonBreakingSpaces(@"a -15 % "), "remove space (minus sign)");
            Assert.AreEqual(@"a +15% ", parser.FixNonBreakingSpaces(@"a +15 % "), "remove space (plus sign)");
            Assert.AreEqual(@"a ±15% ", parser.FixNonBreakingSpaces(@"a ±15 % "), "remove space (plus-minus sign)");
            Assert.AreEqual(@"a 15% ", parser.FixNonBreakingSpaces(@"a 15&nbsp;% "), "remove non breaking space");
            Assert.AreEqual(@"a 15%.", parser.FixNonBreakingSpaces(@"a 15 &nbsp;%."), "remove space and nbsp and maintain point");
            Assert.AreEqual(@"a 15% ", parser.FixNonBreakingSpaces(@"a 15 % "), "remove space");
            Assert.AreEqual(@"a 15%.", parser.FixNonBreakingSpaces(@"a 15&nbsp;%."), "remove non breaking space and maintain point");
            Assert.AreEqual(@"a 15%,", parser.FixNonBreakingSpaces(@"a 15 %,"), "remove space and maintain comma");
            Assert.AreEqual(@"a 15%,", parser.FixNonBreakingSpaces(@"a 15&nbsp;%,"), "remove non breaking space  and maintain comma");
            Assert.AreEqual(@"a 15%!", parser.FixNonBreakingSpaces(@"a 15 %!"), "remove space and maintain mark");
            Assert.AreEqual(@"a 15%!", parser.FixNonBreakingSpaces(@"a 15&nbsp;%!"), "remove non breaking space  and maintain mark");
            Assert.AreEqual(@"a 15  %", parser.FixNonBreakingSpaces(@"a 15  %"), "no changes");
            Assert.AreEqual(@"5a21 %", parser.FixNonBreakingSpaces(@"5a21 %"), "no changes");
            Assert.AreEqual(@"a15 %", parser.FixNonBreakingSpaces(@"a15 %"), "no changes");
            Assert.AreEqual(@"a 15 %a", parser.FixNonBreakingSpaces(@"a 15 %a"), "no changes (character)");
            Assert.AreEqual(@"a 15 %2", parser.FixNonBreakingSpaces(@"a 15 %2"), "no changes (character)");
            Assert.AreEqual(@"a 15.2% ", parser.FixNonBreakingSpaces(@"a 15.2 % "), "catch decimal numbers");
            Assert.AreEqual(@"acid (15.2%) ", parser.FixNonBreakingSpaces(@"acid (15.2 %) "), "decimal numbers in parenthenses");
            Assert.AreEqual(@"a 15.a2 % ", parser.FixNonBreakingSpaces(@"a 15.a2 % "), "avoid weird things");

#if DEBUG
            Variables.SetProjectLangCode("sv");
            Assert.AreEqual(@"a 15 % ", parser.FixNonBreakingSpaces(@"a 15 % "), "Don't remove space in svwiki per sv:Procent");

            Variables.SetProjectLangCode("simple");
            Assert.AreEqual(@"a 15% ", parser.FixNonBreakingSpaces(@"a 15 % "), "Remove space in simple");

            Variables.SetProjectLangCode("en");
            Assert.AreEqual(@"a 15% ", parser.FixNonBreakingSpaces(@"a 15 % "), "remove space");
#endif
        }
        
        [Test]
        public void TestFixCurrency()
        {
            Assert.AreEqual(@"£123 ", parser.FixNonBreakingSpaces(@"£ 123 "), "remove space (Pound sign)");
            Assert.AreEqual(@"€123 ", parser.FixNonBreakingSpaces(@"€ 123 "), "remove space (Euro sign)");
            Assert.AreEqual(@"$123 ", parser.FixNonBreakingSpaces(@"$ 123 "), "remove space (Dollar sign)");
            Assert.AreEqual(@"£123 ", parser.FixNonBreakingSpaces(@"£&nbsp;123 "), "remove non breaking space");
            Assert.AreEqual(@"£123.", parser.FixNonBreakingSpaces(@"£ &nbsp;123."), "remove space and nbsp and maintain point");
            Assert.AreEqual(@"£12.3,", parser.FixNonBreakingSpaces(@"£ 12.3,"), "remove space and maintain comma");

#if DEBUG
            Variables.SetProjectLangCode("sv");
            Assert.AreEqual(@"£ 123 ", parser.FixNonBreakingSpaces(@"£ 123 "), "Don't remove space in svwiki");

            Variables.SetProjectLangCode("simple");
            Assert.AreEqual(@"£123 ", parser.FixNonBreakingSpaces(@"£ 123 "), "Remove space in simple");

            Variables.SetProjectLangCode("en");
            Assert.AreEqual(@"£123 ", parser.FixNonBreakingSpaces(@"£ 123 "), "remove space");
#endif
        }

        [Test]
        public void TestFixClockTime()
        {
        	Assert.AreEqual(@"2:35&nbsp;a.m.", parser.FixNonBreakingSpaces(@"2:35a.m."));
            Assert.AreEqual(@"12:35&nbsp;a.m.", parser.FixNonBreakingSpaces(@"12:35a.m."));
            Assert.AreEqual(@"12:35&nbsp;a.m.", parser.FixNonBreakingSpaces(@"12:35 a.m."));
            Assert.AreEqual(@"12:35&nbsp;p.m.", parser.FixNonBreakingSpaces(@"12:35p.m."));
            Assert.AreEqual(@"12:35&nbsp;p.m.", parser.FixNonBreakingSpaces(@"12:35 p.m."));
            Assert.AreEqual(@"2:35&nbsp;a.m.", parser.FixNonBreakingSpaces(@"02:35a.m."), "starts with zero");
        	Assert.AreEqual(@"2:35&nbsp;p.m.", parser.FixNonBreakingSpaces(@"2:35p.m."));
            Assert.AreEqual(@"12:35&nbsp;p.m.", parser.FixNonBreakingSpaces(@"12:35p.m."));
            Assert.AreEqual(@"2:35&nbsp;p.m.", parser.FixNonBreakingSpaces(@"02:35p.m."), "starts with zero");
            Assert.AreEqual(@"2:75a.m.", parser.FixNonBreakingSpaces(@"2:75a.m."), "invalid minutes number");
            Assert.AreEqual(@"36:15a.m.", parser.FixNonBreakingSpaces(@"36:15a.m."), "invalid hours number");
            Assert.AreEqual(@"16:15c.m.", parser.FixNonBreakingSpaces(@"16:15c.m."), "invalid suffix");
	
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

            Assert.AreEqual(@"a 50&nbsp;Hz rod", parser.FixNonBreakingSpaces(@"a 50Hz rod"));
            Assert.AreEqual(@"a 50&nbsp;kHz rod", parser.FixNonBreakingSpaces(@"a 50kHz rod"));

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
            Assert.AreEqual(@"a 50&nbsp;m/s car", parser.FixNonBreakingSpaces(@"a 50m/s car"));
            Assert.AreEqual(@"at 5&nbsp;°C today", parser.FixNonBreakingSpaces(@"at 5°C today"));
            Assert.AreEqual(@"at 5&nbsp;°C today", parser.FixNonBreakingSpaces(@"at 5 °C today"));
            Assert.AreEqual(@"at 55&nbsp;°F today", parser.FixNonBreakingSpaces(@"at 55°F today"));
            Assert.AreEqual(@"at 55&nbsp;°F today", parser.FixNonBreakingSpaces(@"at 55  °F today"));

			Assert.AreEqual(@"a 50.2&nbsp;m (170&nbsp;ft) road", parser.FixNonBreakingSpaces(@"a 50.2 m (170 ft) road"),"invisible nbsp before m and ft");

			//Assert.AreEqual(@" ", @"&nbsp;");

			// no changes for these
            genFixes.AssertNotChanged(@"nearly 5m people");
			genFixes.AssertNotChanged(@"nearly 5 in 10 people");
			genFixes.AssertNotChanged(@"a 3CD set");
			genFixes.AssertNotChanged(@"its 3 feet are");
			genFixes.AssertNotChanged(@"http://site.com/View/3356 A show");
			genFixes.AssertNotChanged(@"a 50&nbsp;km road");
			genFixes.AssertNotChanged(@"over $200K in cash");
			genFixes.AssertNotChanged(@"now {{a 50kg dog}} was");
			genFixes.AssertNotChanged(@"now a [[50kg dog]] was");
			genFixes.AssertNotChanged(@"now “a 50kg dog” was");
			genFixes.AssertNotChanged(@"now <!--a 50kg dog--> was");
			genFixes.AssertNotChanged(@"now <nowiki>a 50kg dog</nowiki> was");
			genFixes.AssertNotChanged(@"*[http://site.com/blah_20cm_long Site here]");
			genFixes.AssertNotChanged(@"a 50 gram rod");
			genFixes.AssertNotChanged(@"a long (50 foot) toad");

            // firearms articles don't use spaces for ammo sizes
            Assert.AreEqual(@"the 50mm gun", parser.FixNonBreakingSpaces(@"the 50mm gun"));

            // Imperial units
            Assert.AreEqual(@"a long (50&nbsp;in) toad", parser.FixNonBreakingSpaces(@"a long (50 in) toad"));
            Assert.AreEqual(@"a long (50&nbsp;ft) toad", parser.FixNonBreakingSpaces(@"a long (50 ft) toad"));

            Assert.AreEqual(@"a long (50&nbsp;in) toad", parser.FixNonBreakingSpaces(@"a long (50in) toad"));
            Assert.AreEqual(@"a long (50-52&nbsp;in) toad", parser.FixNonBreakingSpaces(@"a long (50-52in) toad"));
            Assert.AreEqual(@"a long (50–52&nbsp;in) toad", parser.FixNonBreakingSpaces(@"a long (50–52in) toad"));
            Assert.AreEqual(@"a long (50.5&nbsp;in) toad", parser.FixNonBreakingSpaces(@"a long (50.5 in) toad"));
            Assert.AreEqual(@"a big (50.5&nbsp;oz) toad", parser.FixNonBreakingSpaces(@"a big (50.5 oz) toad"));
        }

        [Test]
        public void TestFixNonBreakingSpacesDE()
        {
#if DEBUG
            Variables.SetProjectLangCode("fr");
            parser = new Parsers();
            Assert.AreEqual(@"a 50.247&nbsp;µm laser", parser.FixNonBreakingSpaces(@"a 50.247µm laser"));
            Assert.AreEqual(@"a 50.247um laser", parser.FixNonBreakingSpaces(@"a 50.247um laser"));

            Variables.SetProjectLangCode("en");
            parser = new Parsers();
            Assert.AreEqual(@"a 50.247&nbsp;um laser", parser.FixNonBreakingSpaces(@"a 50.247um laser"));
#endif
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

            Assert.AreEqual(@"Smith 200 pp. ISBN 12345678X", parser.FixNonBreakingSpaces(@"Smith 200 pp. ISBN 12345678X"), "No change for number of pages in book");
        }
    }

    [TestFixture]
    public class FormattingTests : RequiresParser
    {
        public GenfixesTestsBase genFixes  = new GenfixesTestsBase();

        [Test]
        public void TestBrConverter()
        {
            Assert.AreEqual("*a\r\nb", Parsers.FixSyntax("*a<br>\r\nb"));
            Assert.AreEqual("*a\r\nb", Parsers.FixSyntax("*a<br><br>\r\nb"));
            Assert.AreEqual("*a\r\nb", Parsers.FixSyntax("\r\n*a<br>\r\nb"));
            Assert.AreEqual("foo\r\n*a\r\nb", Parsers.FixSyntax("foo\r\n*a<br>\r\nb"));
            const string correct1 = "foo\r\n*a<br>b";
            Assert.AreEqual(correct1, Parsers.FixSyntax(correct1), "No change to br tag in middle of list line");

            Assert.AreEqual("*a", Parsers.FixSyntax("*a<br>\r\n")); // \r\n\ trimmed

            Assert.AreEqual("*a", Parsers.FixSyntax("*a<br\\>\r\n"));
            Assert.AreEqual("*a", Parsers.FixSyntax("*a<br/>\r\n"));
            Assert.AreEqual("*a", Parsers.FixSyntax("*a <br\\> \r\n"));

            // leading (back)slash is hack for incorrectly formatted breaks per
            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_7#br_tags_are_not_always_removed
            // remove <br> from lists (end of list line)
            Assert.AreEqual("*a", Parsers.FixSyntax("*a </br/> \r\n"));
            Assert.AreEqual("*a", Parsers.FixSyntax("*a<br\\> \r\n"));
            Assert.AreEqual("*a", Parsers.FixSyntax("*a <\\br\\>\r\n"));
            Assert.AreEqual("*a", Parsers.FixSyntax("*a     <br\\>    \r\n"));
            Assert.AreEqual("*a", Parsers.FixSyntax("*a <br/>\r\n"));
            Assert.AreEqual("*a", Parsers.FixSyntax("*a <br/ >\r\n"));
            Assert.AreEqual("*a", Parsers.FixSyntax("*a <br / >\r\n"));

            Assert.AreEqual("*:#;a\r\n*b", Parsers.FixSyntax("*:#;a<br>\r\n*b"));
            Assert.AreEqual("###;;;:::***a\r\nb", Parsers.FixSyntax("###;;;:::***a<br />\r\nb"));
            Assert.AreEqual("*&a\r\nb", Parsers.FixSyntax("*&a<br/>\r\nb"));

            Assert.AreEqual("&*a<br>\r\nb", Parsers.FixSyntax("&*a<br>\r\nb"));
            Assert.AreEqual("*a\r\n<br>\r\nb", Parsers.FixSyntax("*a\r\n<br>\r\nb"));
        }

        [Test]
        public void SyntaxRegexListRowBrTagStart()
        {
            Assert.AreEqual("x\r\n*abc", parser.FixBrParagraphs("x<br>\r\n*abc"));
            Assert.AreEqual("x\r\n*abc", parser.FixBrParagraphs("x<br> \r\n*abc"));
            Assert.AreEqual("x\r\n*abc", parser.FixBrParagraphs("x<br/> \r\n*abc"));
            Assert.AreEqual("x\r\n*abc", parser.FixBrParagraphs("x<br /> \r\n*abc"));
            Assert.AreEqual("x\r\n*abc", parser.FixBrParagraphs("x<br / > \r\n*abc"));

            genFixes.AssertNotChanged(@"{{{Foo|
param=<br>
**text
**text1 }}");

            Assert.AreEqual(@"** Blog x

'''No", parser.FixBrParagraphs(@"** Blog x
<br>
<br>
'''No"));
        }

        [Test]
        public void TestFixHeadings()
        {
            // breaks if article title is empty
            Assert.AreEqual("==foo==", Parsers.FixHeadings("=='''foo'''==", "Heading with bold"));
            Assert.AreEqual("== foo ==", Parsers.FixHeadings("== '''foo''' ==", "Heading with bold and space"));
            Assert.AreEqual("==  foo  ==", Parsers.FixHeadings("==  '''foo'''  ==", "Heading with bold and more than one space"));
            StringAssert.StartsWith("==foo==", Parsers.FixHeadings("=='''foo'''==\r\n", "test"));
            Assert.AreEqual("quux\r\n\r\n==foo==\r\nbar", Parsers.FixHeadings("quux\r\n=='''foo'''==\r\nbar", "test"));
            Assert.AreEqual("quux\r\n\r\n==foo==\r\n\r\nbar", Parsers.FixHeadings("quux\r\n=='''foo'''==\r\n\r\nbar", "test"));

            Assert.AreEqual("==foo==", Parsers.FixHeadings("==foo==", "No change"));
            Assert.AreEqual("== foo ==", Parsers.FixHeadings("== foo ==", "No change"));

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
            Assert.AreEqual(@"hi.

==News place==
Some news here.", Parsers.FixHeadings(@"hi.

==News place==
Some news here.", "test"), "space trimmed from end of paragraph");

            Assert.AreEqual(@"hi.

==News place==
Some news here.", Parsers.FixHeadings(@"hi.
<br>
==News place==
Some news here.", "test"), "space trimmed from end of paragraph when br replaces newline");

            Assert.AreEqual(@"hi.

==News place==
Some news here.", Parsers.FixHeadings(@"hi.
<br>
<BR>
<BR>
==News place==
Some news here.", "test"), "space trimmed from end of paragraph when br replaces newline");

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#ReferenceS
            Assert.AreEqual(@"==References==", Parsers.FixHeadings(@"==REFERENCES==", "a"));
            Assert.AreEqual(@"==References==", Parsers.FixHeadings(@"==REFERENCE:==", "a"));
            Assert.AreEqual(@"==References==", Parsers.FixHeadings(@"==REFERENSES==", "a"));
            Assert.AreEqual(@"==References==", Parsers.FixHeadings(@"==REFERENCE==", "a"));
            Assert.AreEqual(@"==References==", Parsers.FixHeadings(@"==REFERENCE:==", "a"));
            Assert.AreEqual(@"== References ==", Parsers.FixHeadings(@"== REFERENCES ==", "a"));
            Assert.AreEqual(@"==References==", Parsers.FixHeadings(@"==Reference==", "a"));
            Assert.AreEqual(@"==References==", Parsers.FixHeadings(@"==Refferences==", "a"));
            Assert.AreEqual(@"==References==", Parsers.FixHeadings(@"==Referrences==", "a"));
            Assert.AreEqual(@"==References==", Parsers.FixHeadings(@"==Refferrences==", "a"));
            Assert.AreEqual(@"==References==", Parsers.FixHeadings(@"==Refrences==", "a"));
            Assert.AreEqual(@"==References==", Parsers.FixHeadings(@"==Reffrences==", "a"));
            Assert.AreEqual(@"==References==", Parsers.FixHeadings(@"==Refrence==", "a"));
            Assert.AreEqual(@"==Sources==", Parsers.FixHeadings(@"==SOURCES==", "a"));
            Assert.AreEqual(@"==Sources==", Parsers.FixHeadings(@"==sources==", "a"));
            Assert.AreEqual(@"==Sources==", Parsers.FixHeadings(@"==source==", "a"));
            Assert.AreEqual(@"==Sources==", Parsers.FixHeadings(@"==source:==", "a"));
            Assert.AreEqual(@"== Sources ==", Parsers.FixHeadings(@"== SOURCES ==", "a"));
            Assert.AreEqual(@"==  Sources  ==", Parsers.FixHeadings(@"==  SOURCES  ==", "a"));
            
            Assert.AreEqual(@"some text

==See also==", Parsers.FixHeadings(@"some text

==Also see==", "test"),"rename also see to see also section");

            Assert.AreEqual(@"==See also==", Parsers.FixHeadings(@"==Also see==", "test"),"rename also see to see also section");
            Assert.AreEqual(@"==See also==", Parsers.FixHeadings(@"==Internal links==", "test"),"rename to see also section");
            Assert.AreEqual(@"==See also==", Parsers.FixHeadings(@"==Related articles==", "test"),"rename to see also section");
            Assert.AreEqual(@"===Related articles===", Parsers.FixHeadings(@"===Related articles===", "test"),"do nothing if level 3");
            Assert.AreEqual(@"some text

===Related articles===", Parsers.FixHeadings(@"some text

===Related articles===", "test"),"do nothing if level 3");

            
            string HeadingEqualTitle = Parsers.FixHeadings(@"A
==Foo==
B","Foo");
            Assert.IsFalse(HeadingEqualTitle.Contains("Foo"), "Heading same as title");

            Assert.AreEqual(@"'''The'''.

== History ==

Five", Parsers.FixHeadings(@"'''The'''.

== History ==	 
 		 
Five", "a"));
        }

        [Test]
        public void TestFixHeadingsColon()
        {
            // remove colon from end of heading text
            Assert.AreEqual(@"== hello world ==
", Parsers.FixHeadings(@"== hello world: ==
", "a"),"removes colon");
            Assert.AreEqual(@"== hello world  ==
", Parsers.FixHeadings(@"== hello world : ==
", "a"),"removes colon with space");
            Assert.AreEqual(@"== hello world ==
", Parsers.FixHeadings(@"=== hello world: ===
", "a"),"removes colon - header level 3");
            Assert.AreEqual(@"== hello world ==

== hello world2 ==
", Parsers.FixHeadings(@"== hello world: ==
== hello world2: ==
", "a"),"removes colons from multiple places");

            Assert.AreEqual(@"== hello world ==

== hello world2 ==
", Parsers.FixHeadings(@"== hello world: == 
== hello world2: ==
", "a"), "fixes colon in heading when excess whitespace after heading end");

            // no change if colon within heading text
            Assert.AreEqual(@"== hello:world ==
", Parsers.FixHeadings(@"== hello:world ==
", "a"));

            Assert.AreEqual(@"== : ==
", Parsers.FixHeadings(@"== : ==
", "a"));
            Assert.AreEqual(@"A

== hello world ==
", Parsers.FixHeadings(@"A
 == hello world: ==
", "a"));
        }

        [Test]
        public void TestFixHeadingsBrRemoval()
        {
            Assert.AreEqual(@"==Foo==", Parsers.FixHeadings(@"==Foo<br>==", "a"));
            Assert.AreEqual(@"==Foo==", Parsers.FixHeadings(@"==Foo<br />==", "a"));
            Assert.AreEqual(@"==Foo==", Parsers.FixHeadings(@"==Foo< BR >==", "a"));
        }

        [Test]
        public void TestFixHeadingsBigRemoval()
        {
            Assert.AreEqual(@"==Foo==", Parsers.FixHeadings(@"==<big>Foo</big>==", "a"));
            Assert.AreEqual(@"==Foo==", Parsers.FixHeadings(@"==<Big>Foo</big>==", "a"));
        }

        [Test]
        public void TestFixHeadingsRemoveTwoLevels()
        {
            // single heading
            Assert.AreEqual(@"==hello==
text", Parsers.FixHeadings(@"====hello====
text", "a"));

            // multiple
            Assert.AreEqual(@"==hello==
text

== hello2 ==
texty

=== hello3 ===
", Parsers.FixHeadings(@"====hello====
text
==== hello2 ====
texty
===== hello3 =====
", "a"));

            // level 1 not altered
            Assert.AreEqual(@"=level1=
text

==hello==
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
            Assert.AreEqual(@"==hello==
text

==References==
foo", Parsers.FixHeadings(@"====hello====
text
==References==
foo", "a"));

            Assert.AreEqual(@"==hello==
text

==External links==
foo", Parsers.FixHeadings(@"====hello====
text
==External links==
foo", "a"));

            Assert.IsTrue(Parsers.FixHeadings(@"====hello====
text

==See also==

==External links==
foo", "a").StartsWith(@"==hello==
text

==See also=="));

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
            Assert.AreEqual(@"=== Caernarvon 1536-1832 ===", Parsers.FixHeadings(@"=== <b>Caernarvon</b> 1536-1832 ===", "a"));
            Assert.AreEqual(@"=== Caernarvon 1536-1832 ===", Parsers.FixHeadings(@"=== <B>Caernarvon</B> 1536-1832 ===", "a"));
            Assert.AreEqual(@"===Caernarvon 1536-1832===", Parsers.FixHeadings(@"==='''Caernarvon''' 1536-1832===", "a"));
            Assert.AreEqual(@"=== Caernarvon 1536-1832 ===", Parsers.FixHeadings(@"=== Caernarvon '''1536-1832''' ===", "a"));
            Assert.AreEqual(@"=== Caernarvon 1536-1832 ===", Parsers.FixHeadings(@"=== '''Caernarvon 1536-1832''' ===", "a"));
            Assert.AreEqual(@"==== Caernarvon 1536-1832 ====", Parsers.FixHeadings(@"==== '''Caernarvon 1536-1832''' ====", "a"));

            // not at level 1 or 2
            Assert.AreEqual(@"== '''Caernarvon''' 1536-1832 ==", Parsers.FixHeadings(@"== '''Caernarvon''' 1536-1832 ==", "a"));
            Assert.AreEqual(@"= '''Caernarvon''' 1536-1832 =", Parsers.FixHeadings(@"= '''Caernarvon''' 1536-1832 =", "a"));

            Assert.AreEqual("==See also==", Parsers.FixHeadings("=='''See Also'''==", "test"),"remove bold and fix casing at once");

            Assert.AreEqual(@"==Header with bold==", Parsers.FixHeadings(@"=='''Header with bold'''==<br/>", "test"));
        }

        [Test]
        public void TestFixHeadingsEmpytyBoldRemoval()
        {
        	Assert.AreEqual(@"==Foo==", Parsers.FixHeadings(@"==''''''Foo==", "test"),"empty tag in the beginning");
        	Assert.AreEqual(@"==Foo==", Parsers.FixHeadings(@"==Foo''''''==", "test"),"empty tag at the end");
        	Assert.AreEqual(@"==Foo bar==", Parsers.FixHeadings(@"==Foo''' ''' bar==", "test"),"empty tag in the middle");
        	Assert.AreEqual(@"== Foo bar ==", Parsers.FixHeadings(@"== Foo''' ''' bar ==", "test"),"empty tag in the middle and spaces around");
        	Assert.AreEqual(@"==Foo bar==", Parsers.FixHeadings(@"==Foo'''   ''' bar==", "test"), "more spaces");
        	Assert.AreEqual(@"== Foo bar ==", Parsers.FixHeadings(@"== '''Foo''' ''' bar''' ==", "test"));
        }

        [Test]
        public void TestFixHeadingsBlankLineBefore()
        {
            const string correct = @"Foo

==1920s==
Bar";
            Assert.AreEqual(correct, Parsers.FixHeadings(@"Foo

==1920s==
Bar", "Test"), "no change when already one blank line");
            Assert.AreEqual(correct, Parsers.FixHeadings(@"Foo


==1920s==
Bar", "Test"), "fixes excess blank lines");
            Assert.AreEqual(correct, Parsers.FixHeadings(@"Foo
==1920s==
Bar", "Test"), "inserts blank line if one missing");

            Assert.AreEqual(@"====4====

==2==
text", Parsers.FixHeadings(@"====4====
==2==
text", "Test"), "fixes excess blank lines");

            Assert.AreEqual(@"==2==

====4====
text", Parsers.FixHeadings(@"==2==
====4====
text", "Test"), "fixes excess blank lines");

            Assert.AreEqual(@"x

====Major championships====

==Wins==
x", Parsers.FixHeadings(@"x

====Major championships====
==Wins==
x", "test"));

            Assert.AreEqual(@"x

==Major championships==

====Wins====
x", Parsers.FixHeadings(@"x

==Major championships==
====Wins====
x", "test"));

            Assert.AreEqual(@"x

==Major championships==

====Wins====
x", Parsers.FixHeadings(@"x
==Major championships==
====Wins====
x", "test"));

            Assert.AreEqual(@"==Foo 1==
x

===Major championships===

====Wins====
x", Parsers.FixHeadings(@"==Foo 1==
x
===Major championships===
====Wins====
x", "test"));

            Assert.AreEqual(@"== Events ==
x

=== By place ===

==== Roman Empire ====
x", Parsers.FixHeadings(@"== Events ==
x
=== By place ===
==== Roman Empire ====
x", "test"));

            Assert.AreEqual(@"x

==Major championships==

====Wins====
x", Parsers.FixHeadings(@"x
==Major championships==
====Wins====	
x", "test"), "Excess tab whitespace in second header handled");

            const string indented = @"A
 == some code here === and here ==
 Done";
            Assert.AreEqual(indented, Parsers.FixHeadings(indented, "Test"), "No change to indented text with other == in");
        }

        [Test]
        public void TestFixHeadingsBlankLineBeforeEnOnly()
        {
#if DEBUG
            const string correct = @"Foo

==1920s==
Bar";

            Variables.SetProjectLangCode("fr");
            Assert.AreEqual(@"Foo
==1920s==
Bar", Parsers.FixHeadings(@"Foo
==1920s==
Bar", "Test"), "No change – not en wiki");

            Variables.SetProjectLangCode("en");
            Assert.AreEqual(correct, Parsers.FixHeadings(@"Foo
==1920s==
Bar", "Test"), "inserts blank line if one missing");
#endif
        }

        [Test]
        public void TestFixHeadingsBadHeaders()
        {
            Assert.IsFalse(Parsers.FixHeadings(@"==Introduction==
'''Foo''' great.", "Foo").Contains(@"==Introduction=="));
            Assert.IsFalse(Parsers.FixHeadings(@"==Introduction:==
'''Foo''' great.", "Foo").Contains(@"==Introduction:=="));
            Assert.IsFalse(Parsers.FixHeadings(@"==Introduction:==
'''Foo''' great.", "Foo").Contains(@"==Introduction=="));
            Assert.IsFalse(Parsers.FixHeadings(@"=='''Introduction'''==
'''Foo''' great.", "Foo").Contains(@"==Introduction=="));
            Assert.IsFalse(Parsers.FixHeadings(@"=='''Introduction'''==
'''Foo''' great.", "Foo").Contains(@"=='''Introduction'''=="));
            Assert.IsFalse(Parsers.FixHeadings(@"=='''Introduction:'''==
'''Foo''' great.", "Foo").Contains(@"=='''Introduction:'''=="));
            Assert.IsFalse(Parsers.FixHeadings(@"=='''Introduction:'''==
'''Foo''' great.", "Foo").Contains(@"=='''Introduction'''=="));
            Assert.IsFalse(Parsers.FixHeadings(@"=='''Introduction:'''==
'''Foo''' great.", "Foo").Contains(@"==Introduction:=="));
            Assert.IsFalse(Parsers.FixHeadings(@"=='''Introduction:'''==
'''Foo''' great.", "Foo").Contains(@"==Introduction=="));

            Assert.IsFalse(Parsers.FixHeadings(@"==About==
'''Foo''' great.", "Foo").Contains(@"==About=="));
            Assert.IsFalse(Parsers.FixHeadings(@"==Description==
'''Foo''' great.", "Foo").Contains(@"==Description=="));
            Assert.IsFalse(Parsers.FixHeadings(@"==Overview==
'''Foo''' great.", "Foo").Contains(@"==Overview=="));
            Assert.IsFalse(Parsers.FixHeadings(@"==Definition==
'''Foo''' great.", "Foo").Contains(@"==Definition=="));
            Assert.IsFalse(Parsers.FixHeadings(@"==Profile==
'''Foo''' great.", "Foo").Contains(@"==Profile=="));
            Assert.IsFalse(Parsers.FixHeadings(@"==General information==
'''Foo''' great.", "Foo").Contains(@"==General information=="));

            Assert.AreEqual(Parsers.FixHeadings(@"==Introduction==
'''Foo''' great.", "Foo"),@"
'''Foo''' great.","Removes unnecessary general headers from start of article");

            Assert.AreEqual(@"Great article

Really great", Parsers.FixHeadings(@"Great article

==Foo==
Really great", "Foo"),"Removes heading if it matches pagetitle");

            const string L3 = @"Great article

==A==

===Foo===
Really great";
            Assert.AreEqual(L3, Parsers.FixHeadings(L3, "Foo"),"Does not remove level 3 heading that matches pagetitle");

        	const string HeadingNotAtStart = @"Foo is great.

==Overview==
Here there";
            Assert.AreEqual(HeadingNotAtStart, Parsers.FixHeadings(HeadingNotAtStart, "Foo"), "Heading not removed when not at start of article");
        }

        [Test]
        public void UnbalancedHeadings()
        {
            Assert.IsTrue(Parsers.FixHeadings(@"==External links=
*Foo", "Bar").Contains(@"==External links=="));
            Assert.IsTrue(Parsers.FixHeadings(@"==References=
{{Reflist}}", "Bar").Contains(@"==References=="));
            Assert.IsTrue(Parsers.FixHeadings(@"==See also=
*Foo1
*Foo2", "Bar").Contains(@"==See also=="));
        		
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
            Assert.AreEqual("a\r\n\r\nb", Parsers.RemoveWhiteSpace("a\r\n\r\n\r\n\r\nb"));
            Assert.AreEqual("a \r\nb", Parsers.RemoveWhiteSpace("a \r\nb"), "keep space at end of line before single line break, e.g. within infobox parameters");
            Assert.AreEqual("a\r\n\r\nb", Parsers.RemoveWhiteSpace("a \r\n\r\nb"));
            Assert.AreEqual("a\r\n\r\n\r\n{{foo stub}}", Parsers.RemoveWhiteSpace("a\r\n\r\n\r\n{{foo stub}}"), "two newlines before stub are kept");

            Assert.AreEqual("== foo ==\r\n==bar", Parsers.RemoveWhiteSpace("== foo ==\r\n==bar"));
            Assert.AreEqual("== foo ==\r\n\r\n==bar", Parsers.RemoveWhiteSpace("== foo ==\r\n\r\n==bar"));
            Assert.AreEqual("== foo ==\r\n\r\n==bar", Parsers.RemoveWhiteSpace("== foo ==\r\n\r\n\r\n==bar"));

            Assert.AreEqual("a\r\n\r\nx", Parsers.RemoveWhiteSpace("a<br/>\r\n\r\nx"));
            Assert.AreEqual("a\r\n\r\nx", Parsers.RemoveWhiteSpace("a<br/> \r\n\r\nx"));
            Assert.AreEqual("a\r\n\r\nx", Parsers.RemoveWhiteSpace("a<br/><br/>\r\n\r\nx"));
            Assert.AreEqual("a\r\n\r\nx", Parsers.RemoveWhiteSpace("a\r\n\r\n<br/>\r\n\r\nx"));
            Assert.AreEqual("a\r\n\r\nx", Parsers.RemoveWhiteSpace("a\r\n\r\n<br/><br/>\r\n\r\nx"));
            Assert.AreEqual("a\r\n\r\nx", Parsers.RemoveWhiteSpace("a\r\n\r\n<br/><br/>\r\n\r\nx"));
            
            Assert.AreEqual(@"f.

*Rookie", Parsers.RemoveWhiteSpace(@"f.
<br>
<br>

*Rookie"));

            // eh? should we fix such tables too?
            //Assert.AreEqual("{|\r\n! foo\r\n!\r\nbar\r\n|}", Parsers.RemoveWhiteSpace("{|\r\n! foo\r\n\r\n!\r\n\r\nbar\r\n|}"));

            const string TrackList = @"{{Track listing
| collapsed       =
| headline        =
| extra_column    =
| total_length    =

| all_writing     =
| all_lyrics      =
| all_music       =

| writing_credits =
| lyrics_credits  =
| music_credits   =

| title1          =
| note1           =
| writer1         =
| lyrics1         =
| music1          =
| extra1          =
| length1         =
}}";

            Assert.AreEqual(TrackList, Parsers.RemoveWhiteSpace(TrackList));
            
            const string BlockQuote = @"<blockquote>
Friendship betrayal<br />
They require <br />

Why miserable patient<br />
</blockquote>";
            
            Assert.AreEqual(BlockQuote, Parsers.RemoveWhiteSpace(BlockQuote), "no changes to <br> tags within blockquotes");

             const string Poem = @"<poem>
Friendship betrayal
They require


Why miserable patient
</poem>";

            Assert.AreEqual(Poem, Parsers.RemoveWhiteSpace(Poem), "no changes to newlines within poem");
            Assert.AreEqual("a\r\n\r\nb<poem>A</poem", Parsers.RemoveWhiteSpace("a\r\n\r\n\r\nb<poem>A</poem"), "Changes to newlines OK when poem tags don't contain excess newlines");

            Assert.AreEqual(@"</sup>

Bring",  Parsers.RemoveWhiteSpace(@"</sup>
 

Bring"));
        }

        [Test]
        public void TestMdashesPageRanges()
        {
            Assert.AreEqual("pp. 55–57", parser.Mdashes("pp. 55-57", "test"));
            Assert.AreEqual("pp. 55–57", parser.Mdashes("pp. 55--57", "test"));
            Assert.AreEqual("pp.&nbsp;55–57", parser.Mdashes("pp.&nbsp;55-57", "test"));
            Assert.AreEqual("pp. 55 – 57", parser.Mdashes("pp. 55 - 57", "test"));
            Assert.AreEqual("pp. 55 – 57", parser.Mdashes("Pp. 55 - 57", "test"));
            Assert.AreEqual("pp 55–57", parser.Mdashes("pp 55-57", "test"));
            Assert.AreEqual("pp 1155–1157", parser.Mdashes("pp 1155-1157", "test"));
            Assert.AreEqual("pages= 55–57", parser.Mdashes("pages= 55-57", "test"));
            Assert.AreEqual("pages = 55–57", parser.Mdashes("pages = 55-57", "test"));
            Assert.AreEqual("pages=55–57", parser.Mdashes("pages=55-57", "test"));
            Assert.AreEqual("pages=55–57", parser.Mdashes("pages=55—57", "test"));
            Assert.AreEqual("pages=55–57", parser.Mdashes("pages=55&#8212;57", "test"));
            Assert.AreEqual("pages=55–57", parser.Mdashes("pages=55&mdash;57", "test"));

            // no change if already correct
            Assert.AreEqual("pages=55–57", parser.Mdashes("pages=55–57", "test"));
        }

        [Test]
        public void TestMdashesOtherRanges()
        {
            Assert.AreEqual("55–57 miles", parser.Mdashes("55-57 miles", "test"));
            Assert.AreEqual("55–57 kg", parser.Mdashes("55-57 kg", "test"));
            Assert.AreEqual("55 – 57 kg", parser.Mdashes("55 - 57 kg", "test"));
            Assert.AreEqual("55–57&nbsp;kg", parser.Mdashes("55-57&nbsp;kg", "test"));
            Assert.AreEqual("55–57 Hz", parser.Mdashes("55-57 Hz", "test"));
            Assert.AreEqual("55–57 GHz", parser.Mdashes("55-57 GHz", "test"));
            Assert.AreEqual("55 – 57 m long", parser.Mdashes("55 - 57 m long", "test"));
            Assert.AreEqual("55 – 57 feet", parser.Mdashes("55 - 57 feet", "test"));
            Assert.AreEqual("55 – 57 foot", parser.Mdashes("55 - 57 foot", "test"));
            Assert.AreEqual("long (55 – 57 in) now", parser.Mdashes("long (55 - 57 in) now", "test"));

            Assert.AreEqual("55–57 meters", parser.Mdashes("55-57 meters", "test"));
            Assert.AreEqual("55–57 metres", parser.Mdashes("55-57 metres", "test"));

            // dimensions, not range
            const string dimensions = @"around 34-24-34 in (86-60-86&nbsp;cm) and";
            Assert.AreEqual(dimensions, parser.Mdashes(dimensions, "test"));

            Assert.AreEqual("$55–57", parser.Mdashes("$55-57", "test"));
            Assert.AreEqual("$55 – 57", parser.Mdashes("$55 - 57", "test"));
            Assert.AreEqual("$55–57", parser.Mdashes("$55-57", "test"));
            Assert.AreEqual("$1155–1157", parser.Mdashes("$1155-1157", "test"));
            Assert.AreEqual("$55–57", parser.Mdashes("$55&mdash;57", "test"));
            Assert.AreEqual("$55–57", parser.Mdashes("$55—57", "test"));

            Assert.AreEqual("5:17 AM – 5:19 AM", parser.Mdashes("5:17 AM - 5:19 AM", "test"));
            Assert.AreEqual("05:17 AM – 05:19 AM", parser.Mdashes("05:17 AM - 05:19 AM", "test"));
            Assert.AreEqual("11:17 PM – 11:19 PM", parser.Mdashes("11:17 PM - 11:19 PM", "test"));
            Assert.AreEqual("11:17 pm – 11:19 pm", parser.Mdashes("11:17 pm - 11:19 pm", "test"));
            Assert.AreEqual("11:17 pm – 11:19 pm", parser.Mdashes("11:17 pm &mdash; 11:19 pm", "test"));
            Assert.AreEqual("11:17 pm – 11:19 pm", parser.Mdashes("11:17 pm — 11:19 pm", "test"));

            Assert.AreEqual("Aged 5–9", parser.Mdashes("Aged 5–9", "test"));
            Assert.AreEqual("Aged 5–11", parser.Mdashes("Aged 5–11", "test"));
            Assert.AreEqual("Aged 5 – 9", parser.Mdashes("Aged 5 – 9", "test"));
            Assert.AreEqual("Aged 15–19", parser.Mdashes("Aged 15–19", "test"));
            Assert.AreEqual("Ages 15–19", parser.Mdashes("Ages 15–19", "test"));
            Assert.AreEqual("Aged 15–19", parser.Mdashes("Aged 15–19", "test"));

            Assert.AreEqual("(ages 15–18)", parser.Mdashes("(ages 15-18)", "test"));
        }

        [Test]
        public void TestMdashes()
        {
            // double dash to emdash
            Assert.AreEqual(@"Djali Zwan made their live debut as a quartet—Corgan, Sweeney, Pajo and Chamberlin—at the end of 2001", parser.Mdashes(@"Djali Zwan made their live debut as a quartet -- Corgan, Sweeney, Pajo and Chamberlin -- at the end of 2001", "test"));
            Assert.AreEqual(@"Djali Zwan made their live debut as a quartet—Corgan, Sweeney, Pajo and Chamberlin—at the end of 2001", parser.Mdashes(@"Djali Zwan made their live debut as a quartet --  Corgan, Sweeney, Pajo and Chamberlin --at the end of 2001", "test"));
            Assert.AreEqual(@"Djali Zwan made their live debut as a quartet—Corgan, Sweeney, Pajo and Chamberlin—at the end of 2001", parser.Mdashes(@"Djali Zwan made their live debut as a quartet -- Corgan, Sweeney, Pajo and Chamberlin--at the end of 2001", "test"));
            Assert.AreEqual("Eugene Ormandy—who later", parser.Mdashes("Eugene Ormandy--who later", "test"));
            genFixes.AssertChange("[[Eugene Ormandy]]--who later", "[[Eugene Ormandy]]—who later");

            genFixes.AssertNotChanged(@"now the domain xn-- was");

            // only applied on article namespace
            genFixes.AssertNotChanged(@"Djali Zwan made their live debut as a quartet -- Corgan, Sweeney, Pajo and Chamberlin -- at the end of 2001", "Template:test");

            // precisely two dashes only
            Assert.AreEqual(@"Djali Zwan made their live debut as a quartet --- Corgan, Sweeney, Pajo and Chamberlin - at the end of 2001", parser.Mdashes(@"Djali Zwan made their live debut as a quartet --- Corgan, Sweeney, Pajo and Chamberlin - at the end of 2001", "test"));

            Assert.AreEqual("m<sup>−33</sup>", parser.Mdashes("m<sup>-33</sup>", "test")); // hyphen
            Assert.AreEqual("m<sup>−3324</sup>", parser.Mdashes("m<sup>-3324</sup>", "test")); // hyphen
            Assert.AreEqual("m<sup>−2</sup>", parser.Mdashes("m<sup>-2</sup>", "test")); // hyphen
            Assert.AreEqual("m<sup>−2</sup>", parser.Mdashes("m<sup>–2</sup>", "test")); // en-dash
            Assert.AreEqual("m<sup>−2</sup>", parser.Mdashes("m<sup>—2</sup>", "test")); // em-dash

            // already correct
            Assert.AreEqual("m<sup>−2</sup>", parser.Mdashes("m<sup>−2</sup>", "test"));
            Assert.AreEqual("m<sup>2</sup>", parser.Mdashes("m<sup>2</sup>", "test"));

            // false positive
            Assert.AreEqual("beaten 55 - 57 in 2004", parser.Mdashes("beaten 55 - 57 in 2004", "test"));
            Assert.AreEqual("from 1904 – 11 May 1956 there", parser.Mdashes("from 1904 – 11 May 1956 there", "test"));

            // two dashes to endash for numeric ranges
            Assert.AreEqual("55–77", parser.Mdashes("55--77", "test"));
        }

        [Test]
        public void TestTitleDashes()
        {
            // en-dash
            Assert.AreEqual(@"The Alpher–Bethe–Gamow paper is great.", parser.Mdashes(@"The Alpher-Bethe-Gamow paper is great.", @"Alpher–Bethe–Gamow paper"));
            // em-dash
            Assert.AreEqual(@"The Alpher—Bethe—Gamow paper is great. The Alpher—Bethe—Gamow paper is old.", parser.Mdashes(@"The Alpher-Bethe-Gamow paper is great. The Alpher-Bethe-Gamow paper is old.", @"Alpher—Bethe—Gamow paper"));

            // all hyphens, no change
            Assert.AreEqual(@"The Alpher-Bethe-Gamow paper is great.", parser.Mdashes(@"The Alpher-Bethe-Gamow paper is great.", "Alpher-Bethe-Gamow paper"));
        }

        [Test]
        public void TestBulletListWhitespace()
        {
        	Assert.AreEqual(@"* item
* item2
* item3", Parsers.RemoveAllWhiteSpace(@"*    item
*            item2
* item3"));

        	Assert.AreEqual(@"# item
# item2
# item3", Parsers.RemoveAllWhiteSpace(@"#    item
#            item2
# item3"));
        	
        	Assert.AreEqual(@"# item
# item2
#item3", Parsers.RemoveAllWhiteSpace(@"#    item
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
            Assert.AreEqual(@"18 March 1980 – 12 June 1993", Parsers.RemoveAllWhiteSpace(@"18 March 1980 – 12 June 1993"), "endash spacing not changed, may be correct");
            Assert.AreEqual(@"in 1980&mdash;very", Parsers.RemoveAllWhiteSpace(@"in 1980 &mdash; very"), "mdash spacing cleaned");
        }

        [Test]
        public void FixTemperaturesTests()
        {
            Assert.AreEqual("5°C today", Parsers.FixTemperatures(@"5ºC today"));
            Assert.AreEqual("5°C today", Parsers.FixTemperatures(@"5ºc today"));
            Assert.AreEqual("5°C today", Parsers.FixTemperatures(@"5º C today"));
            Assert.AreEqual("5°C today", Parsers.FixTemperatures(@"5°C today"));
            Assert.AreEqual("5&nbsp;°C today", Parsers.FixTemperatures(@"5&nbsp;°C today"));
            Assert.AreEqual("5&nbsp;°C today", Parsers.FixTemperatures(@"5&nbsp;ºC today"));
            Assert.AreEqual("5°C today", Parsers.FixTemperatures(@"5°    C today"));
            Assert.AreEqual("5°C today", Parsers.FixTemperatures(@"5°C today"));
            Assert.AreEqual("5°F today", Parsers.FixTemperatures(@"5°F today"));
            Assert.AreEqual("75°F today", Parsers.FixTemperatures(@"75°f today"));

            Assert.AreEqual("5ºCC", Parsers.FixTemperatures(@"5ºCC"));
        }

        [Test]
        public void FixSmallTags()
        {
            const string s1 = @"<small>foo</small>";
            Assert.AreEqual(s1, Parsers.FixSyntax(s1));

            Assert.AreEqual(@"<ref>foo</ref>", Parsers.FixSyntax(@"<ref><small>foo</small></ref>"), "removes small from ref tags");
            Assert.AreEqual(@"<REF>foo</REF>", Parsers.FixSyntax(@"<REF><small>foo</small></REF>"), "removes small from ref tags");
            Assert.AreEqual(@"<sup>foo</sup>", Parsers.FixSyntax(@"<sup><small>foo</small></sup>"), "removes small from sup tags");
            Assert.AreEqual(@"<sub>foo</sub>", Parsers.FixSyntax(@"<sub><small>foo</small></sub>"), "removes small from sub tags");
            Assert.AreEqual(@"<small>a foo b</small>", Parsers.FixSyntax(@"<small>a <small>foo</small> b</small>"), "removes nested small from small tags");

            Assert.AreEqual(@"<ref>foo</ref>", Parsers.FixSyntax(@"<small><ref>foo</ref></small>"), "removes small around ref tags");
            Assert.AreEqual(@"<REF>foo</REF>", Parsers.FixSyntax(@"<small><REF>foo</REF></small>"), "removes small around ref tags");
            Assert.AreEqual(@"<sup>foo</sup>", Parsers.FixSyntax(@"<small><sup>foo</sup></small>"), "removes small around sup tags");
            Assert.AreEqual(@"<sub>foo</sub>", Parsers.FixSyntax(@"<small><sub>foo</sub></small>"), "removes small around sub tags");

            const string unclosedTag = @"<ref><small>foo</small></ref> now <small>";
            Assert.AreEqual(unclosedTag, Parsers.FixSyntax(unclosedTag));
            
             const string unclosedTag2 = @"<small>
{|
...<small><ref>foo</ref></small>...
|}</small>";
            Assert.AreEqual(unclosedTag2, Parsers.FixSyntax(unclosedTag2), "No change to small tags across table end, implies multiple incorrect small tags");

            const string NoSmall =  @"<ref>foo</ref> <small>A</small>";
            Assert.AreEqual(NoSmall, Parsers.FixSyntax(NoSmall), "No change when no small that should be removed");
        }

        [Test]
        public void FixSmallTagsImageDescriptions()
        {
            Assert.AreEqual(@"[[File:foo.jpg|bar]]", Parsers.FixSyntax(@"[[File:foo.jpg|<small>bar</small>]]"), "removes small from image descriptions");
            Assert.AreEqual(@"[[File:foo.jpg|thumb|bar]]", Parsers.FixSyntax(@"[[File:foo.jpg|thumb|<small>bar</small>]]"), "removes small from image descriptions when other parameters too");

            const string FileCaptionLegend = @"[[File:foo.jpg|This text is a caption
{{legend|foo|<small>a</small>}}
{{legend|foo2|<small>b</small>}}
]]";
            Assert.AreEqual(FileCaptionLegend, Parsers.FixSyntax(FileCaptionLegend), "small tag not removed from legend template");
        }

        [Test]
        public void LoadTemplateRedirects()
        {
            Dictionary<Regex, string> TemplateRedirects = new Dictionary<Regex, string>();

            Assert.AreEqual(TemplateRedirects, Parsers.LoadTemplateRedirects(""), "returns empty dictionary when no rules present");

            TemplateRedirects.Add(Tools.NestedTemplateRegex("Cn"), "Citation needed");

            Assert.AreEqual(TemplateRedirects.Values, Parsers.LoadTemplateRedirects("{{tl|Cn}} → {{tl|Citation needed}}").Values, "loads single redirect rules");
            Assert.AreEqual(TemplateRedirects.Values, Parsers.LoadTemplateRedirects("{{tl|Cn}} → '''{{tl|Citation needed}}'''").Values, "loads single redirect rules");
            Assert.IsTrue(WikiRegexes.AllTemplateRedirects.IsMatch("{{cn}}"));

            TemplateRedirects.Clear();
            TemplateRedirects.Add(Tools.NestedTemplateRegex(new List<string>(new[] { "Cn", "fact" })), "Citation needed");

            Assert.AreEqual(TemplateRedirects.Values, Parsers.LoadTemplateRedirects("{{tl|Cn}}, {{tl|fact}} → {{tl|Citation needed}}").Values, "loads multiple redirect rules");
            Assert.IsTrue(WikiRegexes.AllTemplateRedirects.IsMatch("{{cn}}"));
            Assert.IsTrue(WikiRegexes.AllTemplateRedirects.IsMatch("{{fact}}"));
        }

        [Test]
        public void LoadDatedTemplates()
        {
            List<string> DatedTemplates = new List<string>();

            Assert.AreEqual(DatedTemplates, Parsers.LoadDatedTemplates(""), "returns empty list when no rules present");

            Assert.AreEqual(DatedTemplates, Parsers.LoadDatedTemplates("<!--{{tl|wikif-->"), "ignores commented out rules");

            DatedTemplates.Add("Wikify");
            Assert.AreEqual(DatedTemplates.ToString(), Parsers.LoadDatedTemplates(@"{{tl|wikify}}").ToString(), "loads single rule");

            DatedTemplates.Add("Citation needed");
            Assert.AreEqual(DatedTemplates.ToString(), Parsers.LoadDatedTemplates(@"{{tl|wikify}}
{{tl|citation needed}}").ToString(), "loads multiple rules");
        }

        [Test]
        public void TemplateRedirects()
        {
            Dictionary<Regex, string> TemplateRedirects = new Dictionary<Regex, string>();
            WikiRegexes.AllTemplateRedirects = null;
            Assert.AreEqual("now {{Cn}} was", Parsers.TemplateRedirects("now {{Cn}} was", TemplateRedirects), "no change when redirects dictionary not built");
            TemplateRedirects = Parsers.LoadTemplateRedirects("{{tl|Cn}}, {{tl|fact}} → {{tl|Citation needed}}");

            Assert.AreEqual("now was", Parsers.TemplateRedirects("now was", TemplateRedirects), "no change when no templates in text");
            Assert.AreEqual("now {{Citation needed}} was", Parsers.TemplateRedirects("now {{Cn}} was", TemplateRedirects));
            Assert.AreEqual("now {{citation needed}} was", Parsers.TemplateRedirects("now {{cn}} was", TemplateRedirects));
            Assert.AreEqual("now {{citation needed}} was{{citation needed}} or", Parsers.TemplateRedirects("now {{cn}} was{{fact}} or", TemplateRedirects), "renames multiple different redirects");
            Assert.AreEqual("now {{citation needed}} was{{citation needed}} or", Parsers.TemplateRedirects("now {{cn}} was{{cn}} or", TemplateRedirects), "renames multiple redirects");

            Assert.AreEqual(@"now {{one|
{{citation needed}} 
}}", Parsers.TemplateRedirects(@"now {{one|
{{cn}} 
}}", TemplateRedirects), "renames when template nested");

            Assert.AreEqual(@"now {{one|
{{two|
{{citation needed}} 
}}}}", Parsers.TemplateRedirects(@"now {{one|
{{two|
{{cn}} 
}}}}", TemplateRedirects), "renames when template double nested");

            TemplateRedirects = Parsers.LoadTemplateRedirects("{{tl|Cn}}, {{tl|fact}} → {{tl|citation needed}}");
            Assert.AreEqual("now {{citation needed}} was", Parsers.TemplateRedirects("now {{cn}} was", TemplateRedirects), "follows case of new template name");
            Assert.AreEqual("now {{Citation needed}} was", Parsers.TemplateRedirects("now {{Cn}} was", TemplateRedirects), "follows case of new template name");
            
            TemplateRedirects = Parsers.LoadTemplateRedirects(@"{{tl|Cn}}, {{tl|fact}} → {{tl|citation needed}}
{{tl|foo}} → {{tl|bar}}");
            Assert.AreEqual("now {{citation needed}} was", Parsers.TemplateRedirects("now {{cn}} was", TemplateRedirects), "follows case of new template name");

            TemplateRedirects = Parsers.LoadTemplateRedirects(@"{{tl|Articleissues}} → {{tl|multiple issues}}
{{tl|Rewrite}} → {{tl|cleanup-rewrite}}
{{tl|Cn}}, {{tl|fact}} → {{tl|citation needed}}");

            Assert.AreEqual(@"now {{Multiple issues|
{{Refimprove|date=July 2012}}
{{Citation style|date=July 2012}}
{{Cleanup-rewrite|date=July 2012}}
}} was", Parsers.TemplateRedirects(@"now {{Articleissues|
{{Refimprove|date=July 2012}}
{{Citation style|date=July 2012}}
{{Rewrite|date=July 2012}}
}} was", TemplateRedirects), "nested template example");

            const string A = @"{{Infobox musical artist
| name       = A
| birth_date  = {{Birth date and age|1989|10|17}}
| genre      = [[Compas music|Konpa]]{{Cn|date=January 2013}}
| label      = Recording
| website        = {{URL|a.com}}
}}";

            Assert.AreEqual(A.Replace(@"{{Cn", @"{{Citation needed"), Parsers.TemplateRedirects(A, TemplateRedirects));
            
            string B = @"{{ONE | {{TWO | {{THREE | {{Cn}} }} }} }}";
            
            Assert.AreEqual(B.Replace(@"{{Cn", @"{{Citation needed"), Parsers.TemplateRedirects(B, TemplateRedirects));

            TemplateRedirects = Parsers.LoadTemplateRedirects("{{tl|Infobox Play}} → {{tl|Infobox play}}");
            Assert.AreEqual("{{Infobox play}}", Parsers.TemplateRedirects("{{Infobox_Play}}", TemplateRedirects), "");

            // when magic word
            TemplateRedirects = Parsers.LoadTemplateRedirects("{{tl|Display title}}, {{tl|Displaytitle}} → {{tl|DISPLAYTITLE}}");

            Assert.AreEqual("now {{DISPLAYTITLE:Foo}} was", Parsers.TemplateRedirects("now {{display title|Foo}} was", TemplateRedirects), "Magic word template redirected and formatted");
            Assert.AreEqual("now {{DISPLAYTITLE:Foo}} was", Parsers.TemplateRedirects("now {{displaytitle|Foo}} was", TemplateRedirects), "Magic word template redirected and formatted");
            Assert.AreEqual("now {{DISPLAYTITLE:Foo}} was", Parsers.TemplateRedirects("now {{Displaytitle|Foo}} was", TemplateRedirects), "Magic word template redirected and formatted");
        }

        [Test]
        public void TemplateRedirectsAcronyms()
        {
            Dictionary<Regex, string> TemplateRedirects = Parsers.LoadTemplateRedirects("{{tl|fb}}, {{tl|foob}} → {{tl|FOO bar}}");

            // for acronym templates enforce first letter uppercase
            Assert.AreEqual("now {{FOO bar}} was", Parsers.TemplateRedirects("now {{fb}} was", TemplateRedirects));
            Assert.AreEqual("now {{FOO bar}} was", Parsers.TemplateRedirects("now {{Fb}} was", TemplateRedirects));
            Assert.AreEqual("now {{FOO bar}} was", Parsers.TemplateRedirects("now {{foob}} was", TemplateRedirects));
            Assert.AreEqual("now {{FOO bar}} was", Parsers.TemplateRedirects("now {{Foob}} was", TemplateRedirects));

            TemplateRedirects = Parsers.LoadTemplateRedirects("{{tl|fb}}, {{tl|foob}} → {{tl|fOO bar}}");
            Assert.AreEqual("now {{fOO bar}} was", Parsers.TemplateRedirects("now {{fb}} was", TemplateRedirects), "first letter case respected for non-acronym template");
            Assert.AreEqual("now {{FOO bar}} was", Parsers.TemplateRedirects("now {{Fb}} was", TemplateRedirects));
        }

        [Test]
        public void RenameTemplateParameters()
        {
            List<WikiRegexes.TemplateParameters> RenamedTemplateParameters = Parsers.LoadRenamedTemplateParameters(@"{{AWB rename template parameter|cite web|acccessdate|accessdate}}");

            const string correct = @"{{cite web | url=http://www.site.com | title = Testing | accessdate = 20 May 2009 }}";
            Assert.AreEqual(correct, Parsers.RenameTemplateParameters(correct.Replace("accessdate", "acccessdate"), RenamedTemplateParameters), "renames parameter in simple template call");

            const string correct2 = @"<ref>{{cite web | url=http://www.site.com | title = Testing | accessdate = 20 May 2009 }}</ref><ref>{{cite web | url=http://www.site2.com | title = Testing2 | accessdate = 2 May 2009 }}</ref>";
            Assert.AreEqual(correct2, Parsers.RenameTemplateParameters(correct2.Replace("accessdate", "acccessdate"), RenamedTemplateParameters), "renames parameter in simple template call");

            const string correct3 = @"{{Cite web | url=http://www.site.com | title = Testing | accessdate = 20 May 2009 }}";
            Assert.AreEqual(correct3, Parsers.RenameTemplateParameters(correct3.Replace("accessdate", "acccessdate"), RenamedTemplateParameters), "renames parameter in simple template call, handles first letter casing of template name");

            string Dupe = @"{{cite web | url=http://www.site.com | title = Testing | accessdate = 20 May 2012 | acccessdate = 11 June 2012 }}";
            Assert.AreEqual(Dupe, Parsers.RenameTemplateParameters(Dupe, RenamedTemplateParameters), "no change when target parameter already has a value");

            string Dupe2 = @"{{cite web | url=http://www.site.com | title = Testing | accessdate = | acccessdate = 11 June 2012 }}";
            Assert.AreEqual(@"{{cite web | url=http://www.site.com | title = Testing | accessdate = | accessdate = 11 June 2012 }}", Parsers.RenameTemplateParameters(Dupe2, RenamedTemplateParameters), "changed when target parameter present without value");
        }

        [Test]
        public void LoadRenamedTemplateParameters()
        {
            List<WikiRegexes.TemplateParameters> RenamedTemplateParameters = Parsers.LoadRenamedTemplateParameters(@"{{AWB rename template parameter|cite web|acccessdate|accessdate}}");

            Assert.AreEqual(1, RenamedTemplateParameters.Count);

            foreach (WikiRegexes.TemplateParameters TP in RenamedTemplateParameters)
            {
                Assert.AreEqual("cite web", TP.TemplateName);
                Assert.AreEqual("acccessdate", TP.OldParameter);
                Assert.AreEqual("accessdate", TP.NewParameter);
            }

            RenamedTemplateParameters = Parsers.LoadRenamedTemplateParameters(@"
{{AWB rename template parameter|cite web|acccessdate|accessdate}}
{{AWB rename template parameter|cite web|acessdate|accessdate}}");

            Assert.AreEqual(2, RenamedTemplateParameters.Count);
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
While remaining upright may be the primary goal of beginning riders==sec== 2009<br>", Parsers.FixLinks(parser.BoldTitle(@"The 2009 Indian Premier League While remaining upright may be the primary goal of beginning riders
While remaining upright may be the primary goal of beginning riders==sec== [[2009 Indian Premier League|2009]]<br>", "2009 Indian Premier League", out noChangeBack), "2009 Indian Premier League", out noChangeBack));
            
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

            Assert.AreEqual(for1 + @"==foo==" + forBefore, Parsers.Dablinks(for1 + @"==foo==" + forBefore), "only merges dablinks in zeroth section");

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
            WikiRegexes.DatedTemplates.Add("fact");

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
            WikiRegexes.DatedTemplates.Add("citation needed");
            string text = parser.Tagger("{{citation needed}}", "Test", false, out noChange, ref summary);
            Assert.IsTrue(text.Contains(@"{{citation needed|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"));

            text = parser.Tagger("{{template:citation needed  }}", "Test", false, out noChange, ref summary);
            Assert.IsTrue(text.Contains(@"{{citation needed  |date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"));

            text = parser.Tagger("{{ citation needed}}", "Test", false, out noChange, ref summary);
            Assert.IsTrue(text.Contains(@"{{ citation needed|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"));

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
            WikiRegexes.DatedTemplates.Add("wikify");
            WikiRegexes.DatedTemplates.Add("wikify section");
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
            WikiRegexes.DatedTemplates.Add("wikify");

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

            WikiRegexes.DatedTemplates.Add("clarify");
            string nochange=@"{{Clarify|date=May 2014|reason=Use Template:Cite web or similar}}";
            Assert.AreEqual(nochange, Parsers.TagUpdater(nochange));
        }

        [Test]
        public void TagUpdaterFormatDate()
        {
            WikiRegexes.DatedTemplates.Add("dead link");
            WikiRegexes.DatedTemplates.Add("wikify");
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
            const string a7 = @"{{trivia}} ==heading==";
            Assert.IsTrue(parser.MultipleIssuesOld(a5 + a3 + a7).Contains(@"{{multiple issues|POV|prose|spam|trivia = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"));

            // don't grab tags in later sections of article
            const string a6 = @"==head== {{essay}}";
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
            
            Assert.AreEqual(@"{{multiple issues|
{{wikify}}
{{unreferenced}}
{{POV}}
}}", parser.MultipleIssues(@"{{wikify}} {{unreferenced}} {{POV}}"), "preserves tag order adding new tags");
            
            Assert.AreEqual(@"{{multiple issues|
{{wikify}}
{{unreferenced}}
{{POV}}
}}

==hello==", parser.MultipleIssues(@"{{wikify}}{{unreferenced}}{{POV}}
==hello=="), "takes tags from same line");
            
            Assert.AreEqual(@"{{multiple issues|
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
            Assert.AreEqual(@"{{multiple issues|
{{underlinked|date=May 2012}}
{{unreferenced}}
{{POV}}
}}

==hello==", parser.MultipleIssues(@"{{underlinked|date=May 2012}}{{unreferenced}}{{POV}}
==hello=="), "takes tags with dates");
            
            Assert.AreEqual(@"{{multiple issues|
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
            Assert.AreEqual(@"{{multiple issues|
{{underlinked|date=May 2012}}
{{unreferenced}}
{{POV}}
}}

Text

==hello==", parser.MultipleIssues(@"{{underlinked|date=May 2012}}{{unreferenced}}
Text
{{POV}}
==hello=="), "takes tags from anywhere in zeroth section");
            
            Assert.AreEqual(@"{{multiple issues|
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
{{multiple issues|section=yes|
{{wikify section}}
{{expand section}}
{{POV-section}}
}}", parser.MultipleIssues(@"==sec==
{{wikify section}} {{expand section}} {{POV-section}}"), "preserves tag order adding new tags");
            
            Assert.AreEqual(@"==sec==
{{multiple issues|section=yes|
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
{{multiple issues|section=yes|
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
{{multiple issues|section=yes|
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
{{multiple issues|section=yes|
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
{{multiple issues|section=yes|
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
