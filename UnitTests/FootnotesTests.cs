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

using System.Collections.Generic;
using NUnit.Framework;
using WikiFunctions;
using WikiFunctions.Parse;

namespace UnitTests
{
    [TestFixture]
    public class FootnotesTests : RequiresInitialization
    {
        [Test]
        // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_6#Unexpected_modification
        public void TestTagBoundaries()
        {
            Assert.That(Parsers.SimplifyReferenceTags("<ref name=\"foo\"><br></ref>"), Is.EqualTo("<ref name=\"foo\"><br></ref>"));
        }

        [Test]
        public void TestSimplifyReferenceTags()
        {
            Assert.That(Parsers.SimplifyReferenceTags("<ref name= \"foo\"></ref>"), Is.EqualTo("<ref name= \"foo\" />"));
            Assert.That(Parsers.SimplifyReferenceTags("<ref name = \"foo\"></ref>"), Is.EqualTo("<ref name = \"foo\" />"));
            Assert.That(Parsers.SimplifyReferenceTags("<ref name=\"foo\" >< / ref >"), Is.EqualTo("<ref name=\"foo\" />"));
            Assert.That(Parsers.SimplifyReferenceTags("<ref name=\"foo\" ></ref>"), Is.EqualTo("<ref name=\"foo\" />"));

            Assert.That(Parsers.SimplifyReferenceTags(@"<ref name=""foo bar"" ></ref>"), Is.EqualTo(@"<ref name=""foo bar"" />"));
            Assert.That(Parsers.SimplifyReferenceTags(@"<ref name='foo bar' >    </ref>"), Is.EqualTo(@"<ref name='foo bar' />"));

            Assert.That(Parsers.SimplifyReferenceTags("<refname=\"foo\"></ref>"), Is.EqualTo("<refname=\"foo\"></ref>"));

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#Reference_bugs
            Assert.That(Parsers.SimplifyReferenceTags(@"<ref name='aftermath'> </ref>"), Is.EqualTo(@"<ref name='aftermath' />"));
            Assert.That(Parsers.SimplifyReferenceTags(@"<ref name='aftermath' /> </ref>"), Is.EqualTo(@"<ref name='aftermath' />"));
        }

        [Test]
        public void TestFixReferenceListTags()
        {
            Assert.That(Parsers.FixReferenceListTags("<references/>"), Is.EqualTo("<references/>"));
            Assert.That(Parsers.FixReferenceListTags("<div><references/></div>"), Is.EqualTo("<div><references/></div>"));

            Assert.That(Parsers.FixReferenceListTags("<references>foo</references>"), Is.EqualTo("<references>foo</references>"));

            Assert.That(Parsers.FixReferenceListTags("<div class=\"references-small\"><references/>\r\n</div>"), Is.EqualTo("{{Reflist}}"));
            Assert.That(Parsers.FixReferenceListTags("<div class=\"references-2column\"><references/></div>"), Is.EqualTo("{{Reflist|2}}"));
            Assert.That(Parsers.FixReferenceListTags(@"<div class=""references-2column""><div class=""references-small"">
<references/></div></div>"),
                            Is.EqualTo("{{Reflist|2}}"));
            Assert.That(Parsers.FixReferenceListTags(@"<div class=""references-small""><div class=""references-2column""> <references/>
</div></div>"),
                            Is.EqualTo("{{Reflist|2}}"));

            // evil don't do's
            Assert.IsFalse(Parsers.FixReferenceListTags(@"<div class=""references-small""><div class=""references-2column"">
<references/></div>* some other ref</div>").Contains("{{Reflist"));
            Assert.IsFalse(Parsers.FixReferenceListTags(@"<div class=""references-small""><div class=""references-2column"">
<references/></div>").Contains("{{Reflist"));

            Assert.That(Parsers.FixReferenceListTags(@"<div class=""references-small"" style=""-moz-column-count:2; column-count:2;"">
<references/>
</div>"),
                            Is.EqualTo("{{Reflist|2}}"), @"Converts to reflist|2 when column-count:2");

            Assert.That(Parsers.FixReferenceListTags(@"<small><references/></small>"), Is.EqualTo("{{Reflist}}"));
            Assert.That(Parsers.FixReferenceListTags(@"<small><references /></small>"), Is.EqualTo("{{Reflist}}"));

#if DEBUG
            Variables.SetProjectLangCode("sv");
            Assert.That(Parsers.FixReferenceListTags("<div class=\"references-small\"><references/>\r\n</div>"), Is.EqualTo("<references/>"));

            Variables.SetProjectLangCode("en");
            Assert.That(Parsers.FixReferenceListTags("<div class=\"references-small\"><references/>\r\n</div>"), Is.EqualTo("{{Reflist}}"));
#endif
        }
        
        [Test]
        public void TestFixReferenceTags()
        {
            // whitespace cleaning
            Assert.That(Parsers.FixReferenceTags(@"now < ref>[http://www.site.com a site]</ref> was"), Is.EqualTo(@"now <ref>[http://www.site.com a site]</ref> was"), "leading");
            Assert.That(Parsers.FixReferenceTags(@"now < ref   >[http://www.site.com a site]</ref> was"), Is.EqualTo(@"now <ref>[http://www.site.com a site]</ref> was"), "both sides");
            Assert.That(Parsers.FixReferenceTags(@"now <ref >[http://www.site.com a site]</ref> was"), Is.EqualTo(@"now <ref>[http://www.site.com a site]</ref> was"), "trailing");
            Assert.That(Parsers.FixReferenceTags(@"now < ref>[http://www.site.com a site]< /ref> was"), Is.EqualTo(@"now <ref>[http://www.site.com a site]</ref> was"), "leading in borth opening and closing tag");
            Assert.That(Parsers.FixReferenceTags(@"now <ref>[http://www.site.com a site]</ ref> was"), Is.EqualTo(@"now <ref>[http://www.site.com a site]</ref> was"), "in closing tag");
            Assert.That(Parsers.FixReferenceTags(@"now <ref>[http://www.site.com a site]</ref > was"), Is.EqualTo(@"now <ref>[http://www.site.com a site]</ref> was"), "in closing tag");
            Assert.That(Parsers.FixReferenceTags(@"now <    ref  >[http://www.site.com a site]< / ref > was"), Is.EqualTo(@"now <ref>[http://www.site.com a site]</ref> was"), "everywhere");

            // <ref name=foo bar> --> <ref name="foo bar">
            Assert.That(Parsers.FixReferenceTags(@"now <ref name=foo bar>and"), Is.EqualTo(@"now <ref name=""foo bar"">and"));
            Assert.That(Parsers.FixReferenceTags(@"now <ref name=foo bar /> and"), Is.EqualTo(@"now <ref name=""foo bar"" /> and"));
            Assert.That(Parsers.FixReferenceTags(@"now <ref name = foo bar >and"), Is.EqualTo(@"now <ref name = ""foo bar"" >and"));
            Assert.That(Parsers.FixReferenceTags(@"now <ref name=foo/bar>and"), Is.EqualTo(@"now <ref name=""foo/bar"">and"), "Use of slash needs quotes");
            Assert.That(Parsers.FixReferenceTags(@"now <ref name=foo/bar />"), Is.EqualTo(@"now <ref name=""foo/bar"" />"), "Use of slash needs quotes, short ref");

            Assert.That(Parsers.FixReferenceTags(@"now<ref name=a.com/2014/11/1744>"), Is.EqualTo(@"now<ref name=""a.com/2014/11/1744"">"), "Use of multiple slashes needs quotes");
            Assert.That(Parsers.FixReferenceTags(@"now<ref name=a.com/2014/11/1744 />"), Is.EqualTo(@"now<ref name=""a.com/2014/11/1744"" />"), "Use of multiple slashes needs quotes");

            string nochange = @"<ref name=VulgarisAerae1
/>";
            Assert.That(Parsers.FixReferenceTags(nochange), Is.EqualTo(nochange));

            // <ref name="Fred""> --> <ref name="Fred">
            Assert.That(Parsers.FixReferenceTags(@"now <ref name=""foo bar"""">and"), Is.EqualTo(@"now <ref name=""foo bar"">and"));
            Assert.That(Parsers.FixReferenceTags(@"now <ref name=""foo bar"""" >and"), Is.EqualTo(@"now <ref name=""foo bar"" >and"));
            Assert.That(Parsers.FixReferenceTags(@"now <ref name=foo bar"""">and"), Is.EqualTo(@"now <ref name=""foo bar"">and"));
            Assert.That(Parsers.FixReferenceTags(@"now <ref name=""""foo bar"""">and"), Is.EqualTo(@"now <ref name=""foo bar"">and"));
            Assert.That(Parsers.FixReferenceTags(@"now <ref name""""foo bar"">and"), Is.EqualTo(@"now <ref name=""foo bar"">and"));
            Assert.That(Parsers.FixReferenceTags(@"now <ref name=""foo bar"".>and"), Is.EqualTo(@"now <ref name=""foo bar"">and"), "Excess . after ref name");
            Assert.That(Parsers.FixReferenceTags(@"now <ref name=""foo bar""'>and"), Is.EqualTo(@"now <ref name=""foo bar"">and"), "Excess ' after ref name");
            Assert.That(Parsers.FixReferenceTags(@"now <ref name=""foo bar""=>and"), Is.EqualTo(@"now <ref name=""foo bar"">and"), "Excess = after ref name");
            Assert.That(Parsers.FixReferenceTags(@"now <ref name""foo bar""=>and"), Is.EqualTo(@"now <ref name=""foo bar"">and"), "= after ref name not before");

            Assert.That(Parsers.FixReferenceTags(@"now <ref name=foo=>and"), Is.EqualTo(@"now <ref name=foo>and"), "Excess = after ref name, no quotes");

            Assert.That(Parsers.FixReferenceTags(@"now <ref name=""""foo ""bar"" x"">and"), Is.EqualTo(@"now <ref name=""foo bar x"">and"), "Excess internal quotes removed");
            Assert.That(Parsers.FixReferenceTags(@"now <ref name=""""foo ""bar"" x"" /> and"), Is.EqualTo(@"now <ref name=""foo bar x"" /> and"), "Excess internal quotes removed, short ref");
            const string grouped = @"<ref name=""Foo"" group=""A"" />";
            Assert.That(Parsers.FixReferenceTags(grouped), Is.EqualTo(grouped), "No change to valid group ref");

            const string grouped2 = @"<ref name=""Foo"" group=A>test</ref>";
            Assert.That(Parsers.FixReferenceTags(grouped2), Is.EqualTo(grouped2), "No change to valid group ref");

            const string grouped3 = @"<ref name=""Foo"" group=""A"">test</ref>";
            Assert.That(Parsers.FixReferenceTags(grouped3), Is.EqualTo(grouped3), "No change to valid group ref, quotes");

            const string grouped4 = @"<ref name=""Foo/Bar"" group=""A"">test</ref>";
            Assert.That(Parsers.FixReferenceTags(grouped4), Is.EqualTo(grouped4), "No change to valid group ref, slash");

            nochange = @"now <ref name=""foo bar"">and";
            Assert.That(Parsers.FixReferenceTags(nochange), Is.EqualTo(nochange));

            nochange = @"now <ref name=foo/   > and";
            Assert.That(Parsers.FixReferenceTags(nochange), Is.EqualTo(nochange));
            nochange = @"now <ref name=foo / > and";
            Assert.That(Parsers.FixReferenceTags(nochange), Is.EqualTo(nochange));

            Assert.That(Parsers.FixReferenceTags(@"now<ref name=WWOS/ref>Foo</ref>"), Is.EqualTo(@"now<ref name=""WWOS/ref"">Foo</ref>"));
            Assert.That(Parsers.FixReferenceTags(@"now<ref name=WWOS/ref />"), Is.EqualTo(@"now<ref name=""WWOS/ref"" />"));

            // <ref name=""Fred"> --> <ref name="Fred">
            Assert.That(Parsers.FixReferenceTags(@"now <ref name=""""foo bar"">and"), Is.EqualTo(@"now <ref name=""foo bar"">and"));
            Assert.That(Parsers.FixReferenceTags(@"now <ref name=""""foo bar>and"), Is.EqualTo(@"now <ref name=""foo bar"">and"));
            Assert.That(Parsers.FixReferenceTags(@"now <ref name=""""foo bar"" >and"), Is.EqualTo(@"now <ref name=""foo bar"" >and"));
            Assert.That(Parsers.FixReferenceTags(@"now <ref name=“""foo bar"" >and"), Is.EqualTo(@"now <ref name=""foo bar"" >and"));

            // <ref name=foo bar"> --> <ref name="foo bar">
            Assert.That(Parsers.FixReferenceTags(@"now <ref name=foo bar"">and"), Is.EqualTo(@"now <ref name=""foo bar"">and"));
            Assert.That(Parsers.FixReferenceTags(@"now <ref name foo bar"">and"), Is.EqualTo(@"now <ref name =""foo bar"">and"), "missing = and first quotes");
            Assert.That(Parsers.FixReferenceTags(@"now <ref name=foo bar"" /> and"), Is.EqualTo(@"now <ref name=""foo bar"" /> and"));
            Assert.That(Parsers.FixReferenceTags(@"now <ref name = foo bar"" >and"), Is.EqualTo(@"now <ref name = ""foo bar"" >and"));

            // <ref name="foo bar> --> <ref name="foo bar">
            Assert.That(Parsers.FixReferenceTags(@"now <ref name=""foo bar>and"), Is.EqualTo(@"now <ref name=""foo bar"">and"));
            Assert.That(Parsers.FixReferenceTags(@"now <ref name=""foo bar /> and"), Is.EqualTo(@"now <ref name=""foo bar"" /> and"));
            Assert.That(Parsers.FixReferenceTags(@"now <ref name = ""foo bar >and"), Is.EqualTo(@"now <ref name = ""foo bar"" >and"));

            // <ref name = ''Fred'> --> <ref name="Fred"> (two apostrophes)
            Assert.That(Parsers.FixReferenceTags(@"now <ref name=''foo bar'>and"), Is.EqualTo(@"now <ref name=""foo bar"">and"));
            Assert.That(Parsers.FixReferenceTags(@"now <ref name='foo bar'' /> and"), Is.EqualTo(@"now <ref name=""foo bar"" /> and"));
            Assert.That(Parsers.FixReferenceTags(@"now <ref name = ''foo bar'' >and"), Is.EqualTo(@"now <ref name = ""foo bar"" >and"));

            // <ref name "foo bar"> --> <ref name="foo bar">
            Assert.That(Parsers.FixReferenceTags(@"now <ref name ""foo bar"">and"), Is.EqualTo(@"now <ref name =""foo bar"">and"));
            Assert.That(Parsers.FixReferenceTags(@"now <ref name ""foo bar"" /> and"), Is.EqualTo(@"now <ref name =""foo bar"" /> and"));

            Assert.That(Parsers.FixReferenceTags(@"now <ref name -""foo bar"">and"), Is.EqualTo(@"now <ref name =""foo bar"">and"));
            Assert.That(Parsers.FixReferenceTags(@"now <ref name -""foo bar"" /> and"), Is.EqualTo(@"now <ref name =""foo bar"" /> and"));
            Assert.That(Parsers.FixReferenceTags(@"now <ref name -  ""foo bar"" >and"), Is.EqualTo(@"now <ref name =  ""foo bar"" >and"));
            Assert.That(Parsers.FixReferenceTags(@"now <ref name +""foo bar"">and"), Is.EqualTo(@"now <ref name =""foo bar"">and"));
            Assert.That(Parsers.FixReferenceTags(@"now <ref name +""foo bar"" /> and"), Is.EqualTo(@"now <ref name =""foo bar"" /> and"));
            Assert.That(Parsers.FixReferenceTags(@"now <ref name +  ""foo bar"" >and"), Is.EqualTo(@"now <ref name =  ""foo bar"" >and"));

            Assert.That(Parsers.FixReferenceTags(@"now <ref name +""foo bar>and"), Is.EqualTo(@"now <ref name =""foo bar"">and"), "Plus and missing ending quote");
            Assert.That(Parsers.FixReferenceTags(@"now <ref name +foo bar"">and"), Is.EqualTo(@"now <ref name =""foo bar"">and"), "Plus and missing opening quote");

            // <ref "foo bar"> --> <ref name="foo bar">
            Assert.That(Parsers.FixReferenceTags(@"now <ref ""foo bar"">and"), Is.EqualTo(@"now <ref name=""foo bar"">and"));
            Assert.That(Parsers.FixReferenceTags(@"now <ref ""foo bar"" /> and"), Is.EqualTo(@"now <ref name=""foo bar"" /> and"));
            Assert.That(Parsers.FixReferenceTags(@"now <ref =""foo bar"">and"), Is.EqualTo(@"now <ref name=""foo bar"">and"));

            // <ref name="Fred" /ref> --> <ref name="Fred"/>
            Assert.That(Parsers.FixReferenceTags(@"now <ref name=""Fred"" /ref>was"), Is.EqualTo(@"now <ref name=""Fred""/>was"));
            Assert.That(Parsers.FixReferenceTags(@"now <ref name=""Fred A"" /ref>was"), Is.EqualTo(@"now <ref name=""Fred A""/>was"));
            Assert.That(Parsers.FixReferenceTags(@"now <ref name=""Fred A"" / /> was"), Is.EqualTo(@"now <ref name=""Fred A""/> was"));
            Assert.That(Parsers.FixReferenceTags(@"now <ref name=""Fred A"" //> was"), Is.EqualTo(@"now <ref name=""Fred A""/> was"));
            Assert.That(Parsers.FixReferenceTags(@"now <ref name=Fred //> was"), Is.EqualTo(@"now <ref name=Fred/> was"));

            // don't change
            Assert.That(Parsers.FixReferenceTags(@"now <ref name=""Fred""?>was"), Is.EqualTo(@"now <ref name=""Fred""?>was"));

            // <ref>...<ref/> --> <ref>...</ref>
            Assert.That(Parsers.FixReferenceTags(@"now <ref>[http://www.site.com a site]<ref/> was"), Is.EqualTo(@"now <ref>[http://www.site.com a site]</ref> was"));
            Assert.That(Parsers.FixReferenceTags(@"now <ref>[http://www.site.com a site]<ref/> was"), Is.EqualTo(@"now <ref>[http://www.site.com a site]</ref> was"));

            // <ref>...</red> --> <ref>...</ref>
            Assert.That(Parsers.FixReferenceTags(@"now <ref>[http://www.site.com a site]</red>was"), Is.EqualTo(@"now <ref>[http://www.site.com a site]</ref>was"));
            Assert.That(Parsers.FixReferenceTags(@"now <ref>[http://www.site.com a site]</red>was"), Is.EqualTo(@"now <ref>[http://www.site.com a site]</ref>was"));

            // no Matches
            Assert.That(Parsers.FixReferenceTags(@"now <ref name=""foo bar"">and"), Is.EqualTo(@"now <ref name=""foo bar"">and"));
            Assert.That(Parsers.FixReferenceTags(@"now <ref name=""foo bar"" /> and"), Is.EqualTo(@"now <ref name=""foo bar"" /> and"));
            Assert.That(Parsers.FixReferenceTags(@"now <ref name = ""foo bar"" >and"), Is.EqualTo(@"now <ref name = ""foo bar"" >and"));
            Assert.That(Parsers.FixReferenceTags(@"now <ref name = 'foo bar' >and"), Is.EqualTo(@"now <ref name = 'foo bar' >and"));

            // <REF> and <Ref> to <ref>
            Assert.That(Parsers.FixReferenceTags(@"now <ref>Smith 2004</ref>"), Is.EqualTo(@"now <ref>Smith 2004</ref>"));
            Assert.That(Parsers.FixReferenceTags(@"now <Ref>Smith 2004</ref>"), Is.EqualTo(@"now <ref>Smith 2004</ref>"));
            Assert.That(Parsers.FixReferenceTags(@"now <REF>Smith 2004</REF>"), Is.EqualTo(@"now <ref>Smith 2004</ref>"));
            Assert.That(Parsers.FixReferenceTags(@"now <reF>Smith 2004</ref>"), Is.EqualTo(@"now <ref>Smith 2004</ref>"));
            Assert.That(Parsers.FixReferenceTags(@"now <reF name=S1>Smith 2004</ref>"), Is.EqualTo(@"now <ref name=S1>Smith 2004</ref>"));
            Assert.That(Parsers.FixReferenceTags(@"now <REF> Smith 2004 </REF>"), Is.EqualTo(@"now <ref>Smith 2004</ref>"));

            // removal of spaces between consecutive references
            Assert.That(Parsers.FixReferenceTags(@"<ref>foo</ref> <ref>foo2</ref>"), Is.EqualTo(@"<ref>foo</ref><ref>foo2</ref>"));
            Assert.That(Parsers.FixReferenceTags(@"<ref>foo</ref>     <ref>foo2</ref>"), Is.EqualTo(@"<ref>foo</ref><ref>foo2</ref>"));
            Assert.That(Parsers.FixReferenceTags(@"<ref>foo</ref> <ref name=Bert />"), Is.EqualTo(@"<ref>foo</ref><ref name=Bert />"));
            Assert.That(Parsers.FixReferenceTags(@"<ref>foo</ref> <ref name=Bert>Bert2</ref>"), Is.EqualTo(@"<ref>foo</ref><ref name=Bert>Bert2</ref>"));
            Assert.That(Parsers.FixReferenceTags(@"<REF>foo</REF> <REF>foo2</REF>"), Is.EqualTo(@"<ref>foo</ref><ref>foo2</ref>"));

            Assert.That(Parsers.FixReferenceTags(@"<ref name=""Tim""/> <ref>foo2</ref>"), Is.EqualTo(@"<ref name=""Tim""/><ref>foo2</ref>"));
            Assert.That(Parsers.FixReferenceTags(@"<ref name=Tim/>     <ref>foo2</ref>"), Is.EqualTo(@"<ref name=Tim/><ref>foo2</ref>"));
            Assert.That(Parsers.FixReferenceTags(@"<ref name=Tim/> <ref name=Bert />"), Is.EqualTo(@"<ref name=Tim/><ref name=Bert />"));
            Assert.That(Parsers.FixReferenceTags(@"<ref name=Tim/> <ref name=Bert>Bert2</ref>"), Is.EqualTo(@"<ref name=Tim/><ref name=Bert>Bert2</ref>"));

            // no matches on inalid ref format
            Assert.That(Parsers.FixReferenceTags(@"<ref name=""Tim""><ref>foo2</ref>"), Is.EqualTo(@"<ref name=""Tim""><ref>foo2</ref>"));

            // ensure a space between a reference and text (reference within a paragrah)
            Assert.That(Parsers.FixReferenceTags(@"Now clearly,<ref>Smith 2004</ref>he was"), Is.EqualTo(@"Now clearly,<ref>Smith 2004</ref> he was"));
            Assert.That(Parsers.FixReferenceTags(@"Now clearly,<ref>Smith 2004</ref>2 were"), Is.EqualTo(@"Now clearly,<ref>Smith 2004</ref> 2 were"));
            Assert.That(Parsers.FixReferenceTags(@"Now clearly,<ref name=Smith/>2 were"), Is.EqualTo(@"Now clearly,<ref name=Smith/> 2 were"));

            // clean space between punctuation and reference
            Assert.That(Parsers.FixReferenceTags(@"Now clearly, <ref>Smith 2004</ref> he was"), Is.EqualTo(@"Now clearly,<ref>Smith 2004</ref> he was"));
            Assert.That(Parsers.FixReferenceTags(@"Now clearly: <ref>Smith 2004</ref> he was"), Is.EqualTo(@"Now clearly:<ref>Smith 2004</ref> he was"));
            Assert.That(Parsers.FixReferenceTags(@"Now clearly; <ref>Smith 2004</ref> he was"), Is.EqualTo(@"Now clearly;<ref>Smith 2004</ref> he was"));
            Assert.That(Parsers.FixReferenceTags(@"Now clearly.   <ref name=Smith/> 2 were"), Is.EqualTo(@"Now clearly.<ref name=Smith/> 2 were"));
            Assert.That(Parsers.FixReferenceTags(@"Now clearly. <ref name=Smith/> 2 were"), Is.EqualTo(@"Now clearly.<ref name=Smith/> 2 were"));
            Assert.That(Parsers.FixReferenceTags(@"Now clearly. <Ref name=Smith/> 2 were"), Is.EqualTo(@"Now clearly.<ref name=Smith/> 2 were"));

            // trailing spaces in reference
            Assert.That(Parsers.FixReferenceTags(@"<ref>foo </ref>"), Is.EqualTo(@"<ref>foo</ref>"));
            Assert.That(Parsers.FixReferenceTags(@"<ref>foo    </ref>"), Is.EqualTo(@"<ref>foo</ref>"));
            Assert.That(Parsers.FixReferenceTags(@"<ref>foo
</ref>"), Is.EqualTo(@"<ref>foo
</ref>"));
            Assert.That(Parsers.FixReferenceTags(@"<ref>  foo </ref>"), Is.EqualTo(@"<ref>foo</ref>"));
            Assert.That(Parsers.FixReferenceTags(@"<REF>  foo </REF>"), Is.EqualTo(@"<ref>foo</ref>"));

            // empty tags
            Assert.That(Parsers.FixReferenceTags(@"<ref> </ref>"), Is.Empty);
            Assert.That(Parsers.FixReferenceTags(@"<ref><ref> </ref></ref>"), Is.Empty);

            // <ref name="Fred" Smith> --> <ref name="Fred Smith">
            Assert.That(Parsers.FixReferenceTags(@"<ref name=""Fred"" Smith />"), Is.EqualTo(@"<ref name=""Fred Smith"" />"));
            Assert.That(Parsers.FixReferenceTags(@"<ref name = ""Fred"" Smith />"), Is.EqualTo(@"<ref name = ""Fred Smith"" />"));
            Assert.That(Parsers.FixReferenceTags(@"<ref name=""Fred""-Smith />"), Is.EqualTo(@"<ref name=""Fred-Smith"" />"));
            Assert.That(Parsers.FixReferenceTags(@"<ref name=""Fred""-Smith/>"), Is.EqualTo(@"<ref name=""Fred-Smith""/>"));
            Assert.That(Parsers.FixReferenceTags(@"<ref name=""Fred""-Smith >text</ref>"), Is.EqualTo(@"<ref name=""Fred-Smith"" >text</ref>"));

            // <ref name-"Fred"> --> <ref name="Fred">
            Assert.That(Parsers.FixReferenceTags(@"<ref name-""Fred"" />"), Is.EqualTo(@"<ref name=""Fred"" />"));
            Assert.That(Parsers.FixReferenceTags(@"<ref name+""Fred"" />"), Is.EqualTo(@"<ref name=""Fred"" />"));
            Assert.That(Parsers.FixReferenceTags(@"<ref name:""Fred"" />"), Is.EqualTo(@"<ref name=""Fred"" />"));
            Assert.That(Parsers.FixReferenceTags(@"<ref name==""Fred"" />"), Is.EqualTo(@"<ref name=""Fred"" />"));
            Assert.That(Parsers.FixReferenceTags(@"<ref name= =""Fred"" />"), Is.EqualTo(@"<ref name=""Fred"" />"));
            Assert.That(Parsers.FixReferenceTags(@"<ref name-""Fred"">"), Is.EqualTo(@"<ref name=""Fred"">"));
            Assert.That(Parsers.FixReferenceTags(@"<ref name-Fred />"), Is.EqualTo(@"<ref name=Fred />"));
            Assert.That(Parsers.FixReferenceTags(@"< ref name-""Fred"" />"), Is.EqualTo(@"< ref name=""Fred"" />"));
            Assert.That(Parsers.FixReferenceTags(@"< ref name+""Fred"" />"), Is.EqualTo(@"< ref name=""Fred"" />"));

            // <ref NAME= --> <ref name=
            Assert.That(Parsers.FixReferenceTags(@"< ref NAME=""Fred"" />"), Is.EqualTo(@"<ref name=""Fred"" />"), "ref NAME");
            Assert.That(Parsers.FixReferenceTags(@"<ref NAME =""Fred"" />"), Is.EqualTo(@"<ref name =""Fred"" />"), "ref NAME");
            Assert.That(Parsers.FixReferenceTags(@"<ref name = name =""Fred"" />"), Is.EqualTo(@"<ref name =""Fred"" />"), "ref name=name=");
            Assert.That(Parsers.FixReferenceTags(@"<ref name=name =""Fred"" />"), Is.EqualTo(@"<ref name =""Fred"" />"), "ref name=name=");
            Assert.That(Parsers.FixReferenceTags(@"<ref name=""ref name=""Fred""/>"), Is.EqualTo(@"<ref name=""Fred""/>"), @"ref name=""ref name=""");
            Assert.That(Parsers.FixReferenceTags(@"<ref name = "" ref name = ""Fred""/>"), Is.EqualTo(@"<ref name = ""Fred""/>"), @"ref name=""ref name=""");

            // <refname= --> <ref name=
            Assert.That(Parsers.FixReferenceTags(@"<refname=""Fred"" />"), Is.EqualTo(@"<ref name=""Fred"" />"), "refname");

            // <ref name"= --> <ref name="
            Assert.That(Parsers.FixReferenceTags(@"<ref name""=Fred"" />"), Is.EqualTo(@"<ref name=""Fred"" />"), @"ref name""= 1");
            Assert.That(Parsers.FixReferenceTags(@"<ref name""=Fred"">"), Is.EqualTo(@"<ref name=""Fred"">"), @"ref name""= 2");
            Assert.That(Parsers.FixReferenceTags(@"<ref name""=Fred>"), Is.EqualTo(@"<ref name=""Fred"">"), @"ref name""= 3");

            // <ref name=> --> <ref>
            Assert.That(Parsers.FixReferenceTags(@"<ref name=>"), Is.EqualTo(@"<ref>"));
            Assert.That(Parsers.FixReferenceTags(@"<ref name =  >"), Is.EqualTo(@"<ref>"));
            Assert.That(Parsers.FixReferenceTags(@"<ref name = """" >"), Is.EqualTo(@"<ref>"));
            Assert.That(Parsers.FixReferenceTags(@"<ref name """"=>"), Is.EqualTo(@"<ref>"));
            Assert.That(Parsers.FixReferenceTags(@"<ref name = group = >"), Is.EqualTo(@"<ref>"));
            Assert.That(Parsers.FixReferenceTags(@"<ref name"">"), Is.EqualTo(@"<ref>"));
            Assert.That(Parsers.FixReferenceTags(@"<ref name"""">"), Is.EqualTo(@"<ref>"));

            Assert.That(Parsers.FixReferenceTags(@"A<ref name="""">Hello</ref>, B<ref name=""""/>"), Is.EqualTo(@"A<ref name="""">Hello</ref>, B<ref name=""""/>"));

            Assert.That(Parsers.FixReferenceTags(@"A.<ref>[http://www.site.com a site]</ref
Then"), Is.EqualTo(@"A.<ref>[http://www.site.com a site]</ref>
Then"), "incorrect closing </ref>");

#if DEBUG
            Variables.SetProjectLangCode("zh");
            Assert.That(Parsers.FixReferenceTags(@"</ref> some text"), Is.EqualTo(@"</ref>some text"), "Chinese put no space before/after the reference tags");
            Assert.That(Parsers.FixReferenceTags(@"<ref name=""foo"" />      some text"), Is.EqualTo(@"<ref name=""foo"" />some text"), "Chinese put no space before/after the reference tags");

            Variables.SetProjectLangCode("en");
            Assert.That(Parsers.FixReferenceTags(@"</ref> some text"), Is.EqualTo(@"</ref> some text"), "English put a space after the reference tags");
#endif
        }

        [Test]
        public void ReorderReferencesTests()
        {
            // [1]...[2][1]
            Assert.That(Parsers.ReorderReferences(@"'''Article''' is great.<ref name = ""Fred1"">So says Fred</ref>
Article started off pretty good, <ref>So says John</ref> <ref name = ""Fred1"" /> and finished well.
End of."), Is.EqualTo(@"'''Article''' is great.<ref name = ""Fred1"">So says Fred</ref>
Article started off pretty good, <ref name = ""Fred1"" /> <ref>So says John</ref> and finished well.
End of."));

            // [1]...[2][1]
            Assert.That(Parsers.ReorderReferences(@"'''Article''' is great.<ref name = ""Fred2"">So says Fred</ref>
Article started off pretty good, <ref name = ""John1"" >So says John</ref> <ref name = ""Fred2"" /> and finished well.
End of."), Is.EqualTo(@"'''Article''' is great.<ref name = ""Fred2"">So says Fred</ref>
Article started off pretty good, <ref name = ""Fred2"" /> <ref name = ""John1"" >So says John</ref> and finished well.
End of."));

            // [1][2]...[3][2][1]
            Assert.That(Parsers.ReorderReferences(@"'''Article''' is great.<ref name = ""Fred8"">So says Fred</ref><ref name = ""John3"" />
Article started off pretty good, <ref>Third</ref><ref name = ""John3"" >So says John</ref> <ref name = ""Fred8"" /> and finished well.
End of."), Is.EqualTo(@"'''Article''' is great.<ref name = ""Fred8"">So says Fred</ref><ref name = ""John3"" />
Article started off pretty good, <ref name = ""Fred8"" /><ref name = ""John3"" >So says John</ref> <ref>Third</ref> and finished well.
End of."));

            // [1][2][3]...[3][2][1]
            Assert.That(Parsers.ReorderReferences(@"'''Article''' is great.<ref name = ""Fred9"">So says Fred</ref><ref name = ""John3"" /><ref name = ""Tim1"">ABC</ref>
Article started off pretty good, <ref name = ""Tim1""/><ref name = ""John3"" >So says John</ref> <ref name = ""Fred9"" /> and finished well.
End of."), Is.EqualTo(@"'''Article''' is great.<ref name = ""Fred9"">So says Fred</ref><ref name = ""John3"" /><ref name = ""Tim1"">ABC</ref>
Article started off pretty good, <ref name = ""Fred9"" /><ref name = ""John3"" >So says John</ref> <ref name = ""Tim1""/> and finished well.
End of."));

            Assert.That(Parsers.ReorderReferences(@"'''Article''' is great.<ref name = 'Fred9'>So says Fred</ref><ref name = 'John3' /><ref name = 'Tim1'>ABC</ref>
Article started off pretty good, <ref name = 'Tim1'/><ref name = 'John3' >So says John</ref> <ref name = 'Fred9' /> and finished well.
End of."), Is.EqualTo(@"'''Article''' is great.<ref name = 'Fred9'>So says Fred</ref><ref name = 'John3' /><ref name = 'Tim1'>ABC</ref>
Article started off pretty good, <ref name = 'Fred9' /><ref name = 'John3' >So says John</ref> <ref name = 'Tim1'/> and finished well.
End of."));

            Assert.That(Parsers.ReorderReferences(@"'''Article''' is great.<ref name = 'Fred9'>So says Fred</ref><ref name = 'John3' /><ref name = 'Tim1'>ABC</ref>
Article started off pretty good, <ref name = 'Tim1'/><ref name = 'John3' >So says John</ref> <ref name = Fred9 /> and finished well.
End of."), Is.EqualTo(@"'''Article''' is great.<ref name = 'Fred9'>So says Fred</ref><ref name = 'John3' /><ref name = 'Tim1'>ABC</ref>
Article started off pretty good, <ref name = Fred9 /><ref name = 'John3' >So says John</ref> <ref name = 'Tim1'/> and finished well.
End of."));

            Assert.That(Parsers.ReorderReferences(@"'''Article''' is great.<ref name = 'Fred9'>So says Fred</ref><ref name = 'John3' /><ref name = 'Tim1'>ABC</ref>
Article started off pretty good, <ref name = 'Tim1'/><ref name = 'John3' >So says John</ref> <ref name = ""Fred9"" /> and finished well.
End of."), Is.EqualTo(@"'''Article''' is great.<ref name = 'Fred9'>So says Fred</ref><ref name = 'John3' /><ref name = 'Tim1'>ABC</ref>
Article started off pretty good, <ref name = ""Fred9"" /><ref name = 'John3' >So says John</ref> <ref name = 'Tim1'/> and finished well.
End of."));

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#Re-ordering_references_can_leave_page_number_templates_behind.
            // have to allow {{rp}} template after a reference
            Assert.That(Parsers.ReorderReferences(@"'''Article''' is great.<ref name = 'Fred9'>So says Fred</ref><ref name = 'John3' /><ref name = 'Tim1'>ABC</ref>
Article started off pretty good, <ref name = 'Tim1'/>{{rp|11}}<ref name = 'John3' >So says John</ref> <ref name = ""Fred9"" /> and finished well.
End of."), Is.EqualTo(@"'''Article''' is great.<ref name = 'Fred9'>So says Fred</ref><ref name = 'John3' /><ref name = 'Tim1'>ABC</ref>
Article started off pretty good, <ref name = ""Fred9"" /><ref name = 'John3' >So says John</ref> <ref name = 'Tim1'/>{{rp|11}} and finished well.
End of."));

            Assert.That(Parsers.ReorderReferences(@"'''Article''' is great.<ref name = 'Fred9'>So says Fred</ref><ref name = 'John3' /><ref name = 'Tim1'>ABC</ref>
Article started off pretty good, <ref name = 'Tim1'/>{{rp |11}}<ref name = 'John3' >So says John</ref> <ref name = ""Fred9"" /> and finished well.
End of."), Is.EqualTo(@"'''Article''' is great.<ref name = 'Fred9'>So says Fred</ref><ref name = 'John3' /><ref name = 'Tim1'>ABC</ref>
Article started off pretty good, <ref name = ""Fred9"" /><ref name = 'John3' >So says John</ref> <ref name = 'Tim1'/>{{rp |11}} and finished well.
End of."));

            Assert.That(Parsers.ReorderReferences(@"'''Article''' is great.<ref name = 'Fred9'>So says Fred</ref><ref name = 'John3' /><ref name = 'Tim1'>ABC</ref>
Article started off pretty good, <ref name = 'Tim1'/>{{rp|needed=y|May 2008}}<ref name = 'John3' >So says John</ref> <ref name = ""Fred9"" /> and finished well.
End of."), Is.EqualTo(@"'''Article''' is great.<ref name = 'Fred9'>So says Fred</ref><ref name = 'John3' /><ref name = 'Tim1'>ABC</ref>
Article started off pretty good, <ref name = ""Fred9"" /><ref name = 'John3' >So says John</ref> <ref name = 'Tim1'/>{{rp|needed=y|May 2008}} and finished well.
End of."));

            Assert.That(Parsers.ReorderReferences(@"'''Article''' is great.<ref name = 'Fred9'>So says Fred</ref><ref name = 'John3' /><ref name = 'Tim1'>ABC</ref>
Article started off pretty good, <ref name = 'Tim1'/>{{rp|11}}<ref name = 'John3' >So says John</ref> <ref name = ""Fred9"" />{{Rp|12}} and finished well.
End of."), Is.EqualTo(@"'''Article''' is great.<ref name = 'Fred9'>So says Fred</ref><ref name = 'John3' /><ref name = 'Tim1'>ABC</ref>
Article started off pretty good, <ref name = ""Fred9"" />{{Rp|12}}<ref name = 'John3' >So says John</ref> <ref name = 'Tim1'/>{{rp|11}} and finished well.
End of."));

            Assert.That(Parsers.ReorderReferences(@"'''Article''' is great.<ref name = 'Fred9'>So says Fred</ref><ref name = 'John3' /><ref name = 'Tim1'>ABC</ref>
Article started off pretty good, <ref name = 'Tim1'/><ref name = 'John3' >So says John</ref>{{rp|11}} <ref name = ""Fred9"" /> and finished well.
End of."), Is.EqualTo(@"'''Article''' is great.<ref name = 'Fred9'>So says Fred</ref><ref name = 'John3' /><ref name = 'Tim1'>ABC</ref>
Article started off pretty good, <ref name = ""Fred9"" /><ref name = 'John3' >So says John</ref>{{rp|11}} <ref name = 'Tim1'/> and finished well.
End of."));

            Assert.That(Parsers.ReorderReferences(@"'''Article''' is great.<ref name = 'Fred9'>So says Fred</ref><ref name = 'John3' /><ref name = 'Tim1'>ABC</ref>
Article started off pretty good, <ref name = 'Tim1'/><ref name = 'John3' >So says John</ref>{{rp|needed=y|May 2008}} <ref name = ""Fred9"" /> and finished well.
End of."), Is.EqualTo(@"'''Article''' is great.<ref name = 'Fred9'>So says Fred</ref><ref name = 'John3' /><ref name = 'Tim1'>ABC</ref>
Article started off pretty good, <ref name = ""Fred9"" /><ref name = 'John3' >So says John</ref>{{rp|needed=y|May 2008}} <ref name = 'Tim1'/> and finished well.
End of."));

            Assert.That(Parsers.ReorderReferences(@"'''Article''' is great.<ref name = 'Fred9'>So says Fred</ref><ref name = 'John3' /><ref name = 'Tim1'>ABC</ref>
Article started off pretty good, <ref name = 'Tim1'/><ref name = 'John3' >So says John</ref>{{Page needed|needed=y|May 2008}} <ref name = ""Fred9"" /> and finished well.
End of."), Is.EqualTo(@"'''Article''' is great.<ref name = 'Fred9'>So says Fred</ref><ref name = 'John3' /><ref name = 'Tim1'>ABC</ref>
Article started off pretty good, <ref name = ""Fred9"" /><ref name = 'John3' >So says John</ref>{{Page needed|needed=y|May 2008}} <ref name = 'Tim1'/> and finished well.
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
            Assert.That(Parsers.ReorderReferences(a), Is.EqualTo(a));
            Assert.That(Parsers.ReorderReferences(b), Is.EqualTo(b));
            Assert.That(Parsers.ReorderReferences(c), Is.EqualTo(c));
            Assert.That(Parsers.ReorderReferences(d), Is.EqualTo(d));
            Assert.That(Parsers.ReorderReferences(e), Is.EqualTo(e));

            // bugfix: <br> in ref
            string nochange = @"* [[Algeria]]<ref name=""UNESCO""/><ref name=""oic"">[http://www.sesrtcic.org/members/default.shtml OIC members and Palestine] ''The Statistical, Economic and Social Research and Training Centre for Islamic Countries''<br> [https://english.people.com.cn/200604/14/eng20060414_258351.html OIC members urge recognition of Hamas] ''People's Daily''</ref><ref name=""MEDEA""/>
* [[Angola]]<ref name=""UNESCO"">{{cite web|url=http://unesdoc.unesco.org/images/0008/000827/082711eo.pdf|title=Request for the admission of the State of Palestine to Unesco as a Member State|date=12 May 1989|publisher=[[UNESCO]]|accessdate=2009-08-22}}</ref><ref name=""MEDEA""/>
* [[Benin]]<ref name=""UNESCO""/><ref name=""oic""/><ref name=""MEDEA""/>";

            Assert.That(Parsers.ReorderReferences(nochange), Is.EqualTo(nochange));
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

            Assert.That(Parsers.ReorderReferences(correctpart + a), Is.EqualTo(correctpart + a));
            Assert.That(Parsers.ReorderReferences(correctpart + b), Is.EqualTo(correctpart + b));

#if DEBUG
            Variables.SetProjectLangCode("fr");
            WikiRegexes.MakeLangSpecificRegexes();
            Assert.That(Parsers.ReorderReferences(correctpart + b.Replace("reflist", "références")), Is.EqualTo(correctpart + b.Replace("reflist", "références")));

            Variables.SetProjectLangCode("en");
            WikiRegexes.MakeLangSpecificRegexes();
            Assert.That(Parsers.ReorderReferences(correctpart + b), Is.EqualTo(correctpart + b));
#endif
        }

        [Test]
        public void ReorderReferencesRefGroups()
        {
            string refgroup = @"'''Article''' is great.<ref name = ""Fred8"" group=a>So says Fred</ref><ref name = ""John3"" />
Article started off pretty good, <ref>Third</ref><ref name = ""Fred8"" group=a/> and finished well.
End of.";
            Assert.That(Parsers.ReorderReferences(refgroup), Is.EqualTo(refgroup));
        }

        [Test]
        public void DuplicateNamedReferencesCondense()
        {
            // duplicate references condense (both named)
            // Matches
            Assert.That(Parsers.DuplicateNamedReferences(@"now<ref name=""Fred"">The Honourable Fred Smith, 2002</ref>but later than<ref name=""Fred"">The Honourable Fred Smith, 2002</ref> was"), Is.EqualTo(@"now<ref name=""Fred"">The Honourable Fred Smith, 2002</ref>but later than<ref name=""Fred""/> was"));
            Assert.That(Parsers.DuplicateNamedReferences(@"now<ref name=""Fred"">The Honourable Fred Smith, 2002</ref>but later than<ref name=""Fred""> The Honourable Fred Smith, 2002  </ref> was"), Is.EqualTo(@"now<ref name=""Fred"">The Honourable Fred Smith, 2002</ref>but later than<ref name=""Fred""/> was"), "excess whitespace");
            Assert.That(Parsers.DuplicateNamedReferences(@"now<ref name=""Fred"" >The Honourable Fred Smith, 2002</ref>but later than<ref name = ""Fred"">The Honourable Fred Smith, 2002</ref> was"), Is.EqualTo(@"now<ref name=""Fred"" >The Honourable Fred Smith, 2002</ref>but later than<ref name=""Fred""/> was"));
            Assert.That(Parsers.DuplicateNamedReferences(@"now<ref name = ""Fred"">The Honourable Fred Smith, 2002 </ref>but later than<ref name=""Fred"">The Honourable Fred Smith, 2002</ref> was"), Is.EqualTo(@"now<ref name = ""Fred"">The Honourable Fred Smith, 2002 </ref>but later than<ref name=""Fred""/> was"));
            Assert.That(Parsers.DuplicateNamedReferences(@"now<ref name=""Fred"">The Honourable Fred Smith, 2002</ref>but later than<ref name=""Fred"" >The Honourable Fred Smith, 2002< /ref> was"), Is.EqualTo(@"now<ref name=""Fred"">The Honourable Fred Smith, 2002</ref>but later than<ref name=""Fred""/> was"));
            Assert.That(Parsers.DuplicateNamedReferences(@"now<ref name='Fred'>The Honourable Fred Smith, 2002</ref>but later than<ref name=""Fred"" >The Honourable Fred Smith, 2002< /ref> was"), Is.EqualTo(@"now<ref name='Fred'>The Honourable Fred Smith, 2002</ref>but later than<ref name=""Fred""/> was"));
            Assert.That(Parsers.DuplicateNamedReferences(@"now<ref name='Fred'>The Honourable Fred Smith, <br>2002</ref>but later than<ref name=""Fred"" >The Honourable Fred Smith, <br>2002< /ref> was"), Is.EqualTo(@"now<ref name='Fred'>The Honourable Fred Smith, <br>2002</ref>but later than<ref name=""Fred""/> was"));

            Assert.That(Parsers.DuplicateNamedReferences(@"now<ref name=""Fred"">The Honourable Fred Smith, 2002</ref> and here<ref name=""Fred"">The Honourable Fred Smith, 2002</ref> but later than<ref name=""Fred"">The Honourable Fred Smith, 2002</ref> was"),
                            Is.EqualTo(@"now<ref name=""Fred"">The Honourable Fred Smith, 2002</ref> and here<ref name=""Fred""/> but later than<ref name=""Fred""/> was"));

            // no Matches
            const string a = @"now<ref name=""Fred"">The Honourable Fred Smith, 2002</ref>but later than<ref name=""Fred"">The Honourable Fred Smith, 2005</ref> was"; // ref text different year
            const string b = @"now<ref name=""Fred"">The Honourable Fred Smith, 2004</ref>but later than<ref name=""Fred"">The Honourable FRED SMITH, 2004</ref> was"; // ref text has different case
            const string c = @"now<ref name=""Fred"">The Honourable Fred Smith, 2003</ref>but later than<ref name=""Fred2"">The Honourable Fred Smith, 2003</ref> was"; // ref name different
            const string c2 = @"now<ref name=""Fred"">Fred Smith, 2003</ref>but later than<ref name=""Fred"">Fred Smith, 2003</ref> was"; // too short

            Assert.That(Parsers.DuplicateNamedReferences(a), Is.EqualTo(a));
            Assert.That(Parsers.DuplicateNamedReferences(b), Is.EqualTo(b));
            Assert.That(Parsers.DuplicateNamedReferences(c), Is.EqualTo(c));
            Assert.That(Parsers.DuplicateNamedReferences(c2), Is.EqualTo(c2));

            // Don't condense ref that was declared in template, template may not use the parameter (hence ref would be unused)
            const string infobox = @"{{infobox foo | badparam=A<ref name=Fred>The Honourable Fred Smith, 2002</ref> | otherparam = yes}}", rest = @"Now.<ref name=Fred>The Honourable Fred Smith, 2002</ref>
==References==
{{reflist}}";
            Assert.IsTrue(Parsers.DuplicateNamedReferences(infobox + "\r\n" + rest).Contains(rest), "Ref that was declared in template not condensed");

            Assert.That(Parsers.DuplicateNamedReferences(@"{{infobox foo | badparam=A<ref name=Fred /> | otherparam = yes}}
Now.<ref name=Fred>The Honourable Fred Smith, 2002</ref> And.<ref name=Fred>The Honourable Fred Smith, 2002</ref>
==References==
{{reflist}}"), Is.EqualTo(@"{{infobox foo | badparam=A<ref name=Fred /> | otherparam = yes}}
Now.<ref name=Fred>The Honourable Fred Smith, 2002</ref> And.<ref name=Fred/>
==References==
{{reflist}}"), "Ref that was used in short form in template can be condensed elsewhere");

            const string Lewis = @"Hello<ref name=""Lewis"">{{cite journal |authors=Lewis JT |title=Low-grade cases |journal=Am J Surg Pathol |pmid=22301502}}</ref> recommended.

A<ref name=""Wang"">{{cite journal |authors=Wang X |title=Recurrent |journal=Nat Genet |pmid=24859338}}</ref> only. Surgery.<ref name=""Lewis"">{{cite journal |authors=Lewis JT |title=Low-grade cases |journal=Am J Surg Pathol |pmid=22301502}}</ref>

== References ==
{{Reflist}}

<ref name=""Wang"">{{cite journal |authors=Wang X |title=Recurrent |journal=Nat Genet |pmid=24859338}}</ref>
<ref name=""Lewis"">{{cite journal |authors=Lewis JT |title=Low-grade cases |journal=Am J Surg Pathol |pmid=22301502}}</ref>";

            Assert.That(Parsers.DuplicateNamedReferences(Lewis), Is.EqualTo(@"Hello<ref name=""Lewis"">{{cite journal |authors=Lewis JT |title=Low-grade cases |journal=Am J Surg Pathol |pmid=22301502}}</ref> recommended.

A<ref name=""Wang"">{{cite journal |authors=Wang X |title=Recurrent |journal=Nat Genet |pmid=24859338}}</ref> only. Surgery.<ref name=""Lewis""/>

== References ==
{{Reflist}}

<ref name=""Wang"">{{cite journal |authors=Wang X |title=Recurrent |journal=Nat Genet |pmid=24859338}}</ref>
<ref name=""Lewis"">{{cite journal |authors=Lewis JT |title=Low-grade cases |journal=Am J Surg Pathol |pmid=22301502}}</ref>"), "When dupe cite to condense, and another one after reflist, don't condense both");
        }

        [Test]
        public void DuplicateNamedReferencesCondenseQuotes()
        {
            // duplicate references condense (both named)
            Assert.That(Parsers.DuplicateNamedReferences(@"now<ref name=Fred>The Honourable Fred Smith, 2002</ref>but later than<ref name=Fred>The Honourable Fred Smith, 2002</ref> was"), Is.EqualTo(@"now<ref name=Fred>The Honourable Fred Smith, 2002</ref>but later than<ref name=Fred/> was"), "Don't add quotes around ref name if none there currently");
            Assert.That(Parsers.DuplicateNamedReferences(@"now<ref name=Fred>The Honourable Fred Smith, 2002</ref>but later than<ref name='Fred'>The Honourable Fred Smith, 2002</ref> was"), Is.EqualTo(@"now<ref name=Fred>The Honourable Fred Smith, 2002</ref>but later than<ref name='Fred'/> was"), "Retain single quotes around ref name");
            Assert.That(Parsers.DuplicateNamedReferences(@"now<ref name=Fred>The Honourable Fred Smith, 2002</ref>but later than<ref name=""Fred"">The Honourable Fred Smith, 2002</ref> was"), Is.EqualTo(@"now<ref name=Fred>The Honourable Fred Smith, 2002</ref>but later than<ref name=""Fred""/> was"), "Retain double quotes around ref name");
        }

        [Test]
        public void DuplicateNamedReferencesTests()
        {
            // duplicate references fix (first named)
            // Matches
            Assert.That(Parsers.DuplicateNamedReferences(@"now<ref name=""Bert"">The Honourable Bert Smith, 2002</ref>but later than<ref> The Honourable Bert Smith, 2002  </ref> was"), Is.EqualTo(@"now<ref name=""Bert"">The Honourable Bert Smith, 2002</ref>but later than<ref name=""Bert""/> was"), "first named excess whitespace");
            Assert.That(Parsers.DuplicateNamedReferences(@"now<ref name=""Bert"" >The Honourable Bert Smith, 2002</ref>but later than<ref>The Honourable Bert Smith, 2002</ref> was"), Is.EqualTo(@"now<ref name=""Bert"" >The Honourable Bert Smith, 2002</ref>but later than<ref name=""Bert""/> was"));
            Assert.That(Parsers.DuplicateNamedReferences(@"now<ref name = ""Bert"">The Honourable Bert Smith, 2002 </ref>but later than<ref>The Honourable Bert Smith, 2002</ref> was"), Is.EqualTo(@"now<ref name = ""Bert"">The Honourable Bert Smith, 2002 </ref>but later than<ref name=""Bert""/> was"));
            Assert.That(Parsers.DuplicateNamedReferences(@"now<ref name=""Bert"">The Honourable Bert Smith, 2002</ref>but later than<ref>The Honourable Bert Smith, 2002< /ref> was"), Is.EqualTo(@"now<ref name=""Bert"">The Honourable Bert Smith, 2002</ref>but later than<ref name=""Bert""/> was"));
            Assert.That(Parsers.DuplicateNamedReferences(@"now<ref name='Bert'>The Honourable Bert Smith, 2002</ref>but later than<ref>The Honourable Bert Smith, 2002< /ref> was"), Is.EqualTo(@"now<ref name='Bert'>The Honourable Bert Smith, 2002</ref>but later than<ref name=""Bert""/> was"));
            Assert.That(Parsers.DuplicateNamedReferences(@"now<ref name=""Bert"">The Honourable Bert <small>Smith</small>, 2002</ref>but later than<ref> The Honourable Bert <small>Smith</small>, 2002  </ref> was"), Is.EqualTo(@"now<ref name=""Bert"">The Honourable Bert <small>Smith</small>, 2002</ref>but later than<ref name=""Bert""/> was"), "nested tag support");

            // no Matches
            const string d = @"now<ref name=""Bert"">The Honourable Bert Smith, 2002</ref>but later than<ref>The Honourable Bert Smith, 2005</ref> was";
            const string e = @"now<ref name=""Bert"">The Honourable Bert Smith, 2004</ref>but later than<ref>The Honourable BERT SMITH, 2004</ref> was";

            Assert.That(Parsers.DuplicateNamedReferences(d), Is.EqualTo(d));
            Assert.That(Parsers.DuplicateNamedReferences(e), Is.EqualTo(e)); // reference text casing

            // duplicate references fix (second named)
            Assert.That(Parsers.DuplicateNamedReferences(@"now<ref>The Honourable John Smith, 2002</ref>but later than<ref name=""John"" > The Honourable John Smith, 2002  </ref> was"), Is.EqualTo(@"now<ref name=""John""/>but later than<ref name=""John"" > The Honourable John Smith, 2002  </ref> was"), "second named excess whitespace");
            Assert.That(Parsers.DuplicateNamedReferences(@"now<ref>The Honourable John Smith, 2002</ref>but later than<ref name=""John"" >The Honourable John Smith, 2002</ref> was"), Is.EqualTo(@"now<ref name=""John""/>but later than<ref name=""John"" >The Honourable John Smith, 2002</ref> was"));
            Assert.That(Parsers.DuplicateNamedReferences(@"now<ref>The Honourable John Smith, 2002 </ref>but later than<ref name = ""John"">The Honourable John Smith, 2002</ref> was"), Is.EqualTo(@"now<ref name=""John""/>but later than<ref name = ""John"">The Honourable John Smith, 2002</ref> was"));
            Assert.That(Parsers.DuplicateNamedReferences(@"now<ref>The Honourable John Smith, 2002</ref>but later than<ref name=""John"">The Honourable John Smith, 2002< /ref> was"), Is.EqualTo(@"now<ref name=""John""/>but later than<ref name=""John"">The Honourable John Smith, 2002< /ref> was"));
            Assert.That(Parsers.DuplicateNamedReferences(@"now<ref>The Honourable John Smith, 2002</ref>but later than<ref name='John'>The Honourable John Smith, 2002< /ref> was"), Is.EqualTo(@"now<ref name=""John""/>but later than<ref name='John'>The Honourable John Smith, 2002< /ref> was"));
            Assert.That(Parsers.DuplicateNamedReferences(@"now<ref>The Honourable John Smith, 2002</ref>but later than<ref name=""John 2"" > The Honourable John Smith, 2002  </ref> was"), Is.EqualTo(@"now<ref name=""John 2""/>but later than<ref name=""John 2"" > The Honourable John Smith, 2002  </ref> was"));

            // no Matches
            const string f = @"now<ref>The Honourable John Smith, 2002</ref>but later than<ref name=""John"">The Honourable John Smith, 2005</ref> was";
            const string g = @"now<ref>The Honourable John Smith, 2004</ref>but later than<ref name=""John"">The HONOURABLE JOHN SMITH, 2004</ref> was";

            Assert.That(Parsers.DuplicateNamedReferences(f), Is.EqualTo(f));
            Assert.That(Parsers.DuplicateNamedReferences(g), Is.EqualTo(g)); // reference text casing

            // don't condense a ref in {{reflist}}
            const string RefInReflist = @"Foo<ref name='first'>690nBTivR9o2mJ6vMYccwmjl5TO9BxvhF9deev2VSi17H</ref>here.
            
            ==Notes==
            {{reflist|2|refs=
            
            <ref name='first'>690nBTivR9o2mJ6vMYccwmjl5TO9BxvhF9deev2VSi17H</ref>
            
            }}";

            Assert.That(Parsers.DuplicateNamedReferences(RefInReflist), Is.EqualTo(RefInReflist));

            // don't condense where same named ref has different values, multiple instances
            const string h = @"now<ref name=""Fred"">The Honourable Fred Smith, 2002</ref>but later than first<ref name=""Fred"">The Honourable Fred Smith, 2002</ref> and second<ref name=""Fred"">The Honourable Fred Smith, 2005</ref> was"; // ref text different year
            Assert.That(Parsers.DuplicateNamedReferences(h), Is.EqualTo(h));
        }

        [Test]
        public void DuplicateNamedReferencesReparse()
        {
            Assert.That(Parsers.DuplicateNamedReferences(@"now<ref name=""Fred"">The Honourable Fred Smith, 2002 wcK63lPneu8pqiK6bxxYMgRSL7ySo4XaOIl4Kr24XB8Hd</ref>but later<ref name=Fred2>text words random 3b3HmI0viQTbmiLIDiGnjTAIX8mpebT520I1 words</ref> than<ref name=""Fred"">The Honourable Fred Smith, 2002 wcK63lPneu8pqiK6bxxYMgRSL7ySo4XaOIl4Kr24XB8Hd</ref> was<ref name=""Fred2"">text words random 3b3HmI0viQTbmiLIDiGnjTAIX8mpebT520I1 words</ref>"),
                Is.EqualTo(@"now<ref name=""Fred"">The Honourable Fred Smith, 2002 wcK63lPneu8pqiK6bxxYMgRSL7ySo4XaOIl4Kr24XB8Hd</ref>but later<ref name=Fred2>text words random 3b3HmI0viQTbmiLIDiGnjTAIX8mpebT520I1 words</ref> than<ref name=""Fred""/> was<ref name=""Fred2""/>"), "reparse used to fix all duplicate refs");
        }

        [Test]
        public void DuplicateUnnamedReferences()
        {
            string namedref = @"now <ref name=foo>foo</ref> and <ref name=foo/>";
            Assert.That(Parsers.DuplicateUnnamedReferences(""), Is.Empty);
            Assert.That(Parsers.DuplicateUnnamedReferences(namedref), Is.EqualTo(namedref));

            // no existing named ref
            string nonamedref = @"<ref>""bookrags.com""</ref> foo <ref> ""bookrags.com"" </ref>";
            Assert.That(Parsers.DuplicateUnnamedReferences(nonamedref), Is.EqualTo(nonamedref));

            // existing named ref – good for edit
            Assert.That(Parsers.DuplicateUnnamedReferences(@"<ref>""bookrags.com""</ref> foo <ref> ""bookrags.com"" </ref>" + namedref), Is.EqualTo(@"<ref name=""bookrags.com"">""bookrags.com""</ref> foo <ref name=""bookrags.com""/>" + namedref), "named ref 1");
            Assert.That(Parsers.DuplicateUnnamedReferences(@"<ref>""bookrags.com""</ref> foo bar <ref>abcde</ref> now <ref>abcde</ref>now <ref>""bookrags.com""</ref>" + namedref),
                            Is.EqualTo(@"<ref name=""bookrags.com"">""bookrags.com""</ref> foo bar <ref name=""abcde"">abcde</ref> now <ref name=""abcde""/>now <ref name=""bookrags.com""/>" + namedref), "named ref 2");

            Assert.That(Parsers.DuplicateUnnamedReferences(@"<ref>http://ecomodder.com/forum/showthread.php/obd-mpguino-gauge-2702.html</ref> foo bar <ref>http://ecomodder.com/wiki/index.php/MPGuino</ref> now <ref>http://ecomodder.com/wiki/index.php/MPGuino</ref>now <ref>http://ecomodder.com/forum/showthread.php/obd-mpguino-gauge-2702.html</ref>" + namedref),
                            Is.EqualTo(@"<ref name=""ecomodder.com"">http://ecomodder.com/forum/showthread.php/obd-mpguino-gauge-2702.html</ref> foo bar <ref name=""ReferenceA"">http://ecomodder.com/wiki/index.php/MPGuino</ref> now <ref name=""ReferenceA""/>now <ref name=""ecomodder.com""/>" + namedref), "named ref 3");

            const string Ibid = @"now <ref>ibid</ref> was<ref>ibid</ref> there";
            Assert.That(Parsers.DuplicateUnnamedReferences(Ibid + namedref), Is.EqualTo(Ibid + namedref));

            const string Pageneeded = @"now <ref>Class 50s in Operation. D Clough {{page needed|date=September 2014}}</ref>  was <ref>Class 50s in Operation. D Clough {{page needed|date=September 2014}}</ref> there";
            Assert.That(Parsers.DuplicateUnnamedReferences(Pageneeded + namedref), Is.EqualTo(Pageneeded + namedref));

            // nothing to do here
            const string SingleRef = @"now <ref>first</ref> was";
            Assert.That(Parsers.DuplicateUnnamedReferences(SingleRef + namedref), Is.EqualTo(SingleRef + namedref));

            const string E = @"Foo<ref name=a>a</ref> bar<ref name=a/> was<ref>b</ref> and 1<ref>c</ref> or 2<ref>c</ref>",
            E2 = @"Foo<ref name=a>a</ref> bar<ref name=a/> was<ref>b</ref> and 1<ref name=""ReferenceA"">c</ref> or 2<ref name=""ReferenceA""/>";

            Assert.That(Parsers.DuplicateUnnamedReferences(E), Is.EqualTo(E2));

            const string F = @"<ref>""book""</ref> foo <ref> ""book"" </ref> <ref name=book>A</ref><ref name=""ReferenceA"">""bookA""</ref><ref name=""ReferenceB"">""bookB""</ref><ref name=""ReferenceC"">""bookC""</ref>";
            Assert.That(Parsers.DuplicateUnnamedReferences(F), Is.EqualTo(F));

            Assert.That(Parsers.DuplicateUnnamedReferences(@"{{infobox foo|bar=1<ref>""bookrags.com""</ref>}} foo <ref> ""bookrags.com"" </ref>" + namedref), Is.EqualTo(@"{{infobox foo|bar=1<ref name=""bookrags.com"">""bookrags.com""</ref>}} foo <ref name=""bookrags.com"">""bookrags.com""</ref>" + namedref), "Ref named, but not condensed if declared in template");
        }

        [Test]
        public void HasNamedReferences()
        {
            Assert.IsTrue(Parsers.HasNamedReferences(@"now <ref name=foo />"));
            Assert.IsTrue(Parsers.HasNamedReferences(@"now <ref name=""foo"" />"));
            Assert.IsFalse(Parsers.HasNamedReferences(@"now <ref name=foo>foo</ref>"));
            Assert.IsFalse(Parsers.HasNamedReferences(@"now <ref name=""foo"">bar</ref>"));
            Assert.IsFalse(Parsers.HasNamedReferences(@"now <ref>foo</ref>"));
            Assert.IsFalse(Parsers.HasNamedReferences(@"now <!--<ref name = foo>foo</ref>-->"));
        }

        [Test]
        public void DeriveReferenceNameTests()
        {
            Assert.That(Parsers.DeriveReferenceName("a", "Smith"), Is.EqualTo("Smith"));
            Assert.That(Parsers.DeriveReferenceName("a", @"{{cite book|last=Smith|title=hello|year=2008|foo=bar}}"), Is.EqualTo(@"Smith 2008"));
            Assert.That(Parsers.DeriveReferenceName("a", @"{{cite book|last=Smith|title=hello|year=2008}}"), Is.EqualTo(@"Smith 2008"));
            Assert.That(Parsers.DeriveReferenceName("a", @"{{cite book|year=2008|last=Smith|title=hello||foo=bar|pages=17}}"), Is.EqualTo(@"Smith 2008 17"));
            Assert.That(Parsers.DeriveReferenceName("a", @"{{citation|pages=17|year=2008|last=Smith|title=hello||foo=bar}}"), Is.EqualTo(@"Smith 2008 17"));
            Assert.That(Parsers.DeriveReferenceName("a", @"{{Cite book|title=hello|year=2008|last=Smither Bee|foo=bar}}"), Is.EqualTo(@"Smither Bee 2008"));
            Assert.That(Parsers.DeriveReferenceName("a", @"Oldani (1982: p.8)"), Is.EqualTo("Oldani 1982: p.8"));
            Assert.That(Parsers.DeriveReferenceName("a", @"http://elections.sos.state.tx.us/elchist.exe"), Is.EqualTo("elections.sos.state.tx.us"));
            Assert.That(Parsers.DeriveReferenceName("a", @"{{cite web | url=http://www.imdb.com/title/tt0120992/crazycredits | title=Hypnotist The Incredible BORIS is Boris Cherniak | accessdate=2007-10-15 }}"), Is.EqualTo("imdb.com"));
            Assert.That(Parsers.DeriveReferenceName("a", @"{{cite web | url=hello.imdb.com/tt0120992/ | title=Hypnotist The Incredible BORIS is Boris Cherniak | accessdate=2007-10-15 }}"), Is.EqualTo("Hypnotist The Incredible BORIS is B"));
            Assert.That(Parsers.DeriveReferenceName("a", @"{{cite web | url=hello.imdb.com/tt0120992/ | title=Hypnotist{{!}} The Incredible | accessdate=2007-10-15 }}"), Is.EqualTo("Hypnotist The Incredible"));
            Assert.That(Parsers.DeriveReferenceName("a", @"{{cite web | url=hello.imdb.com/tt0120992/ | trans-title= |title=Hypnotist The Incredible BORIS | accessdate=2007-10-15 }}"), Is.EqualTo("Hypnotist The Incredible BORIS"), "title use");
            Assert.That(Parsers.DeriveReferenceName("a", @"{{cite web | url=hello.imdb.com/tt0120992/ | title=Foreign language|trans-title=Hypnotist The Incredible BORIS | accessdate=2007-10-15 }}"), Is.EqualTo("Hypnotist The Incredible BORIS"), "Trans-title use");
            Assert.That(Parsers.DeriveReferenceName(@"A<ref name=""hello.imdb.com"">x</ref>", @"{{cite web | url=https://hello.imdb.com/tt0120992/ | title=Hypnotist The Incredible BORIS is Boris Cherniak | accessdate=2007-10-15 }}"), Is.EqualTo("Hypnotist The Incredible BORIS is B"));
            Assert.That(Parsers.DeriveReferenceName("a", @"Caroline Humphrey, David Sneath-The end of Nomadism?, p.27"), Is.EqualTo("Caroline Humphrey p.27"));
            Assert.That(Parsers.DeriveReferenceName("a", @"Dennis, Peter (et al.) (1995) The Oxford Companion to Australian Military History, Melbourne: Oxford University Press, Page 440."), Is.EqualTo("Dennis, Peter 1995 Page 440"));
            Assert.That(Parsers.DeriveReferenceName("a", @"{{cite journal
  | last = Sepkoski| first = Jack| authorlink =| coauthors =| title =  A compendium of fossil marine animal genera (Trilobita entry)| journal = Bulletins of American Paleontology| volume = 364| issue =| pages = p.560| publisher =| location =| date = 2002| url = http://strata.ummp.lsa.umich.edu/jack/showgenera.php?taxon=307&rank=class| doi =| id =| accessdate = 2008-01-12 }}"), Is.EqualTo("Sepkoski 2002 p.560"));
            Assert.That(Parsers.DeriveReferenceName("a", @"{{Cite journal
  | last = Sepkoski| first = Jack| authorlink =| coauthors =| title =  A compendium of fossil marine animal genera (Trilobita entry)| journal = Bulletins of American Paleontology| volume = 364| issue =| pages = p.560| publisher =| location =| year = 2002| url = http://strata.ummp.lsa.umich.edu/jack/showgenera.php?taxon=307&rank=class| doi =| id =| accessdate = 2008-01-12 }}"), Is.EqualTo("Sepkoski 2002 p.560"));
            Assert.That(Parsers.DeriveReferenceName("a", "[http://www.gillan.com/anecdotage-12.html Ian Gillan's official website: Caramba]"), Is.EqualTo("gillan.com"));
            Assert.That(Parsers.DeriveReferenceName("a", @"{{cite web | title= Founder of China's Born Again Movement Set Free| work= Christianity Today | url= http://www.christianitytoday.com/ct/2000/augustweb-only/22.0.html| accessdate=2008-02-19}}"), Is.EqualTo("Christianity Today"));
            Assert.That(Parsers.DeriveReferenceName("a", @"Moscow Bound: Policy, Politics, and the POW/MIA Dilemma, John M. G. Brown, Veteren Press, Eureka Springs, California, 1993, Chapt. 14"), Is.EqualTo("Moscow Bound 1993"));
            Assert.That(Parsers.DeriveReferenceName("a", @"""Boris Nikolayevich Kampov,"" ''Contemporary Authors Online'', Thomson Gale, 2007."), Is.EqualTo("Boris Nikolayevich Kampov 2007"));

            Assert.That(Parsers.DeriveReferenceName("a", @"{{cite book| last = Farlow| first = James O.| coauthors = M. K. Brett-Surmann| title = The Complete Dinosaur| publisher = Indiana University Press| date = 1999| location = Bloomington, Indiana| pages = 8| isbn = 0-253-21313-4}}"), Is.EqualTo("Farlow 1999 8"));
            Assert.That(Parsers.DeriveReferenceName("a", @"JAY STRAFFORD, ""[http://www.timesdispatch.com/rtd/entertainment/books_literature/article/BMYS25_20090121-192837/184966/ Globe-trotting with the Grim Reaper],"" ''Richmond-Times Dispatch'' (January 25, 2009)."), Is.EqualTo("Globe-trotting with the Grim Reaper"));


            Assert.That(Parsers.DeriveReferenceName("a", @""), Is.EqualTo("ReferenceA"));

            Assert.That(Parsers.DeriveReferenceName("a", @"* Cf. Ezriel Carlebach entry in the Hebrew Wikipedia"), Is.EqualTo("ReferenceA"));

            Assert.That(Parsers.DeriveReferenceName("a", @"{{cite book |author=Bray, Warwick |year=1968 |chapter=Everyday Life of The Aztecs |pages=93-96}}"), Is.EqualTo("Bray, Warwick 1968 93-96"));
            Assert.That(Parsers.DeriveReferenceName("a", @"{{harv|Olson|2000|p=84}}"), Is.EqualTo("Olson 2000 84"));
            Assert.That(Parsers.DeriveReferenceName("a", @"{{harv|Olson|2000|pp =84}}"), Is.EqualTo("Olson 2000 84"));
            Assert.That(Parsers.DeriveReferenceName("a", @"{{harvnb|Olson|2000 }}"), Is.EqualTo("Olson 2000"));
            Assert.That(Parsers.DeriveReferenceName("a", @"{{Sfn|Olson|2000 }}"), Is.EqualTo("Olson 2000"));
            Assert.That(Parsers.DeriveReferenceName("a", @"{{Harvard citation no brackets|Olson|2000 }}"), Is.EqualTo("Olson 2000"));
            Assert.That(Parsers.DeriveReferenceName("a", @"{{harvcolnb|Olson|2000|p=84}}"), Is.EqualTo("Olson 2000 84"));
            Assert.That(Parsers.DeriveReferenceName("a", @"{{Harvcolnb|Olson|2000|p=84}}"), Is.EqualTo("Olson 2000 84"));
            Assert.That(Parsers.DeriveReferenceName("a", @"{{Harvnb|Caggiano|Duncan|1983}}"), Is.EqualTo(@"Caggiano 1983"));
            Assert.That(Parsers.DeriveReferenceName("a", @"{{Harvnb|Simpson|others|1986}}"), Is.EqualTo(@"Simpson 1986"));

            Assert.That(Parsers.DeriveReferenceName("a", @"Reload Bench [http://reloadbench.com/cartridges/w17bee.html]"), Is.EqualTo("reloadbench.com"));

            Assert.That(Parsers.DeriveReferenceName("a", @"""-logy."" ''The Oxford English Dictionary'', Second Edition. Oxford University Press, 1989. retrieved 20 Aug 2008."), Is.EqualTo("The Oxford English Dictionary 1989"));

            Assert.That(Parsers.DeriveReferenceName("a", @"{{cite web|title=firstaif|url=http://www.firstaif.info/pages/nz_mounted.htm}}</ref><ref>{{cite web|title=diggerhistory|url=http://www.diggerhistory.info/pages-nz/nzef.htm}}"), Is.EqualTo("firstaif"));


            Assert.That(Parsers.DeriveReferenceName("a", @"{{cite web
  | last =
  | first =
  | title = Welcome to the on-line catalog of Bloom Cigar Company in Pittsburgh, home of Cigar CampTM
  | work =
  | publisher = Bloom Cigar Company
  | date = 2003
  | url = http://www.bloomcigar.com/catalog/
  | accessdate = 2008-11-01
  | archiveurl = http://www.webcitation.org/5c1PcWcc8
  | archivedate = 2008-11-01}}"), Is.EqualTo("Bloom Cigar Company"));

            Assert.That(Parsers.DeriveReferenceName("a", @"Kuykendall, R.S. (1967). The Hawaiian Kingdom, Vol. III, 1874-1893: The Kalakaua Dynasty. Honolulu: University of Hawaii Press. p. 628."), Is.EqualTo("Kuykendall, R.S. 1967 p. 628"));

            Assert.That(Parsers.DeriveReferenceName("a", @"[[#Banj1930|Bajanov 2003]]: 2-3"), Is.EqualTo(@"Bajanov 2003: 2-3"));

            // doesn't provide what's already in use
            Assert.That(Parsers.DeriveReferenceName("a<ref name=ReferenceA>foo</ref> now", @"* Cf. Ezriel Carlebach entry in the Hebrew Wikipedia"), Is.EqualTo("ReferenceB"));
            Assert.That(Parsers.DeriveReferenceName("a<ref name=ReferenceA>foo</ref> now <ref name=ReferenceB>foo</ref>", @"* Cf. Ezriel Carlebach entry in the Hebrew Wikipedia"), Is.EqualTo("ReferenceC"));
            Assert.That(Parsers.DeriveReferenceName("a<ref name=ReferenceA>foo</ref> now <ref name=ReferenceB>foo</ref> <ref name=ReferenceC>foo</ref>", @"* Cf. Ezriel Carlebach entry in the Hebrew Wikipedia"), Is.EqualTo("ReferenceD"));

            Assert.That(Parsers.DeriveReferenceName(@"a <ref name=""Smither Bee 2008"">a</ref> was", @"{{Cite book|title=hello|year=2008|last=Smither Bee|foo=bar|url = http://books.Google.com/special}}"), Is.EqualTo(@"books.Google.com"));
            Assert.That(Parsers.DeriveReferenceName(@"a <ref name='Smither Bee 2008'>a</ref> was", @"{{Cite book|title=hello|year=2008|last=Smither Bee|foo=bar|url = http://books.Google.com/special}}"), Is.EqualTo(@"books.Google.com"));

            Assert.That(Parsers.DeriveReferenceName("a", @"<ref>The city of Baeza, March 2, 1476. ''Colección de documentos España'', t. XI, p.396</ref>"), Is.EqualTo(@"Baeza, March 2, 1476. p.396"), "correct date format used");

            Assert.That(Parsers.DeriveReferenceName("a", @"{{cite pmid|123456}}"), Is.EqualTo(@"pmid123456"));
            Assert.That(Parsers.DeriveReferenceName("a", @"{{cite doi|10.1112/123456}}"), Is.EqualTo(@"doi10.1112/123456"));

            Assert.That(Parsers.DeriveReferenceName("abc <ref name=dtic.mil>a</ref>", "http://www.dtic.mil/dtic/tr/fulltext/u2/a168577.pdf |ANALYSIS OF M16A2 RIFLE CHARACTERISTICS AND RECOMMENDED IMPROVEMENTS. Arthur D. Osborne. Mellonics Systems Development Division. Litton Systems, Inc. WD and Seward Smith ARI Field Unit at Fort Benning, Georgia. TRAINING RESEARCH LABORATORY. U. S. Army Research Institute for the Behavioral and Social Sciences. February 1986"), Is.EqualTo("ReferenceA"));
        }

        [Test]
        public void SameRefDifferentName()
        {
            Assert.That(Parsers.SameRefDifferentName(@"Foo<ref name=Jones>Jones 2005</ref> and bar<ref name=J5>Jones 2005</ref>"),
                            Is.EqualTo(@"Foo<ref name=Jones>Jones 2005</ref> and bar<ref name=""Jones"">Jones 2005</ref>"));

            Assert.That(Parsers.SameRefDifferentName(@"Foo<ref name=Jones>Jones 2005</ref> and bar<ref name=J5>Jones 2005</ref> and more<ref name=J5/>"),
                            Is.EqualTo(@"Foo<ref name=Jones>Jones 2005</ref> and bar<ref name=""Jones"">Jones 2005</ref> and more<ref name=""Jones""/>"));

            Assert.That(Parsers.SameRefDifferentName(@"Foo<ref name=Jones>Jones 2005</ref> and bar<ref name=J5>
Jones 2005</ref>"),
                            Is.EqualTo(@"Foo<ref name=Jones>Jones 2005</ref> and bar<ref name=""Jones"">
Jones 2005</ref>"));

            // order reversed
            Assert.That(Parsers.SameRefDifferentName(@"Foo<ref name=J5>Jones 2005</ref> and bar<ref name=Jones>Jones 2005</ref>"),
                            Is.EqualTo(@"Foo<ref name=""Jones"">Jones 2005</ref> and bar<ref name=Jones>Jones 2005</ref>"));

            // leading/trailing whitespace in ref doesn't matter
            Assert.That(Parsers.SameRefDifferentName(@"Foo<ref name=Jones>Jones 2005</ref> and bar<ref name=J5>Jones 2005   </ref>"),
                            Is.EqualTo(@"Foo<ref name=Jones>Jones 2005</ref> and bar<ref name=""Jones"">Jones 2005   </ref>"));

            // don't merge ibid refs
            string nochange = @"Foo<ref name=Jones>ibid</ref> and bar<ref name =A5>ibid</ref>";
            Assert.That(Parsers.SameRefDifferentName(nochange), Is.EqualTo(nochange));

            // don't rename ref is used in a group ref
            nochange = @"Foo<ref name=Jones group=A/> and bar<ref name =Jones>Text</ref> and <ref name =JonesY>Text</ref>";
            Assert.That(Parsers.SameRefDifferentName(nochange), Is.EqualTo(nochange));
            nochange = @"Foo<ref name=""Jones"" group=A/> and bar<ref name =""Jones"">Text2</ref> and <ref name =JonesY>Text2</ref>";
            Assert.That(Parsers.SameRefDifferentName(nochange), Is.EqualTo(nochange));
            nochange = @"Foo<ref name='Jones' group=A/> and bar<ref name =Jones>Text3</ref> and <ref name =JonesY>Text3</ref>";
            Assert.That(Parsers.SameRefDifferentName(nochange), Is.EqualTo(nochange));
            nochange = @"Foo<ref group=A name=Jones /> and bar<ref name =Jones>Text4</ref> and <ref name =JonesY>Text4</ref>";
            Assert.That(Parsers.SameRefDifferentName(nochange), Is.EqualTo(nochange));
        }

        [Test]
        public void SameRefDifferentNameMulti()
        {
            string before = @"Foo.<ref>{{cite web | url = http://site.com | title = This is a normal title }}</ref>
Foo2.<ref name=A>{{cite web | url = http://site.com | title = This is a normal title }}</ref>
Bar3.<ref name=ABCDEF>{{cite web | url = http://site.com | title = This is a normal title }}</ref>
Bar4.<ref name=""ABCDEFGHI"">{{cite web | url = http://site.com | title = This is a normal title }}</ref>

==References==
{{reflist}}";

            string after = @"Foo.<ref name=""ABCDEFGHI""/>
Foo2.<ref name=""ABCDEFGHI"">{{cite web | url = http://site.com | title = This is a normal title }}</ref>
Bar3.<ref name=""ABCDEFGHI""/>
Bar4.<ref name=""ABCDEFGHI""/>

==References==
{{reflist}}";

            Assert.That(Parsers.SameRefDifferentName(before), Is.EqualTo(after), "reparse used to rename multiple refs with same name");
        }

        [Test]
        public void SameRefDifferentNameNameDerivation()
        {
            string correct = @"Foo<ref name=Jones>Jones 2005</ref> and bar<ref name=""Jones"">Jones 2005</ref>";
            Assert.That(Parsers.SameRefDifferentName(@"Foo<ref name=Jones>Jones 2005</ref> and bar<ref name=J5>Jones 2005</ref>"), Is.EqualTo(correct));
            Assert.That(Parsers.SameRefDifferentName(@"Foo<ref name=Jones>Jones 2005</ref> and bar<ref name=""Jo"">Jones 2005</ref>"), Is.EqualTo(correct));
            Assert.That(Parsers.SameRefDifferentName(@"Foo<ref name=Jones>Jones 2005</ref> and bar<ref name=autogenerated1>Jones 2005</ref>"), Is.EqualTo(correct));
            Assert.That(Parsers.SameRefDifferentName(@"Foo<ref name=Jones>Jones 2005</ref> and bar<ref name= ReferenceA>Jones 2005</ref>"), Is.EqualTo(correct));
        }

        [Test]
        public void SameNamedRefShortText()
        {

            Assert.That(Parsers.SameRefDifferentName(@"Foo<ref name=Jones>Jones 2005 extra words of interest</ref> and bar<ref name=Jones>a</ref>"),
                            Is.EqualTo(@"Foo<ref name=Jones>Jones 2005 extra words of interest</ref> and bar<ref name=""Jones""/>"));

            Assert.That(Parsers.SameRefDifferentName(@"Foo<ref name=Jones>x</ref> and bar<ref name=Jones>Jones 2005 extra words of interest</ref>"),
                            Is.EqualTo(@"Foo<ref name=""Jones""/> and bar<ref name=Jones>Jones 2005 extra words of interest</ref>"));

            Assert.That(Parsers.SameRefDifferentName(@"Foo<ref name=Jones>Jones 2005 extra words of interest</ref> and bar<ref name=Jones>a</ref> and bar2<ref name=Jones>x</ref>"),
                            Is.EqualTo(@"Foo<ref name=Jones>Jones 2005 extra words of interest</ref> and bar<ref name=""Jones""/> and bar2<ref name=""Jones""/>"));

            Assert.That(Parsers.SameRefDifferentName(@"Foo<ref name=Jones>Jones 2005 extra words of interest</ref> and bar2<ref name=Jones>[see above]</ref>"),
                            Is.EqualTo(@"Foo<ref name=Jones>Jones 2005 extra words of interest</ref> and bar2<ref name=""Jones""/>"));
            Assert.That(Parsers.SameRefDifferentName(@"Foo<ref name=Jones>Jones 2005 extra words of interest</ref> and bar2<ref name=Jones>{{cite web}}</ref>"),
                            Is.EqualTo(@"Foo<ref name=Jones>Jones 2005 extra words of interest</ref> and bar2<ref name=""Jones""/>"));

            string pageref1 = @"Foo<ref name=Jones>Jones 2005 extra words of interest</ref> and bar<ref name=Jones>2</ref>";
            string pageref2 = @"Foo<ref name=Jones>Jones 2005 extra words of interest</ref> and bar<ref name=Jones> page 2</ref>";
            string pageref3 = @"Foo<ref name=Jones>Jones 2005 extra words of interest</ref> and bar<ref name=Jones>pp. 2</ref>";
            string pageref4 = @"Foo<ref name=Jones>Jones 2005 extra words of interest</ref> and bar<ref name=Jones>P 2</ref>";
            string pageref5 = @"Foo<ref name=Jones>Jones 2005 extra words of interest</ref> and bar<ref name=Jones>Jones P 2</ref>";

            Assert.That(Parsers.SameRefDifferentName(pageref1), Is.EqualTo(pageref1));
            Assert.That(Parsers.SameRefDifferentName(pageref2), Is.EqualTo(pageref2));
            Assert.That(Parsers.SameRefDifferentName(pageref3), Is.EqualTo(pageref3));
            Assert.That(Parsers.SameRefDifferentName(pageref4), Is.EqualTo(pageref4));
            Assert.That(Parsers.SameRefDifferentName(pageref5), Is.EqualTo(pageref5));
        }

        [Test]
        public void BadCiteParameters()
        {
            Dictionary<int, int> Found = new Dictionary<int, int>();

            // standard cases
            Found.Add(15, 3);
            Assert.That(Parsers.BadCiteParameters(@"now {{cite web|foo=bar|date=2009}} was"), Is.EqualTo(Found));

            Found.Remove(15);
            Found.Add(15, 6);
            Assert.That(Parsers.BadCiteParameters(@"now {{cite web|foodoo=bar|date=2009}} was"), Is.EqualTo(Found));

            Found.Remove(15);
            Found.Add(15, 6);
            Assert.That(Parsers.BadCiteParameters(@"now {{cite web|fooéoo=bar|date=2009}} was"), Is.EqualTo(Found));

            Found.Remove(15);
            Found.Add(15, 3);
            Assert.That(Parsers.BadCiteParameters(@"now {{cite web|day=11|date=2009}} was"), Is.EqualTo(Found));

            Found.Remove(15);
            Found.Add(15, 3);
            Assert.That(Parsers.BadCiteParameters(@"now {{cite web|day=1|date=2009}} was"), Is.EqualTo(Found));

            Found.Remove(15);
            Found.Add(15, 5);
            Assert.That(Parsers.BadCiteParameters(@"now {{cite web|da te=1|date=2009}} was"), Is.EqualTo(Found));

            Found.Remove(15);
            Found.Add(15, 9);
            Assert.That(Parsers.BadCiteParameters(@"now {{cite web|'language=Thai|date=2009}} was"), Is.EqualTo(Found));

            Found.Remove(15);

            // no errors here
            Assert.That(Parsers.BadCiteParameters(@"now {{cite web|url=bar|date=2009}} was"), Is.EqualTo(Found));
            Assert.That(Parsers.BadCiteParameters(@"now {{cite web|url=bar|date=2009|title=A|trans-title=B}} was"), Is.EqualTo(Found));
            Assert.That(Parsers.BadCiteParameters(@"now {{cite web|url=bar|date=2009|editor=a}} was"), Is.EqualTo(Found));
            Assert.That(Parsers.BadCiteParameters(@"now {{cite web|url=bar|date=2009|id=3838}} was"), Is.EqualTo(Found));
            Assert.That(Parsers.BadCiteParameters(@"now {{cite web|url=bar|date=2009|issn=3838-1542}} was"), Is.EqualTo(Found));
            Assert.That(Parsers.BadCiteParameters(@"now {{cite web|url=bar|date=2009|isbn=1234567890}} was"), Is.EqualTo(Found));
            Assert.That(Parsers.BadCiteParameters(@"now {{cite web|url=bar|date=2009|author1=Y|author2=X}} was"), Is.EqualTo(Found));
            Assert.That(Parsers.BadCiteParameters(@"now {{cite web|url=bar|date=2009|deadurl=no}} was"), Is.EqualTo(Found));
            Assert.That(Parsers.BadCiteParameters(@"now {{cite web|url={{Allmusic|class = foo}}|date=2009}} was"), Is.EqualTo(Found));
            Assert.That(Parsers.BadCiteParameters(@"now {{cite web|url=bar|date=2009|at=here}} was"), Is.EqualTo(Found));
            Assert.That(Parsers.BadCiteParameters(@"now {{cite web|url=bar|date=2009|first1=Foo|last1=Great |first2=Bar|title=here}} was"), Is.EqualTo(Found));
            Assert.That(Parsers.BadCiteParameters(@"now {{cite web|url=bar|date=2009|authorlink1=Smith}} was"), Is.EqualTo(Found));
            Assert.That(Parsers.BadCiteParameters(@"now {{cite web|url=bar|date=2009|authorlink20=Bill}} was"), Is.EqualTo(Found));
            Assert.That(Parsers.BadCiteParameters(@"now {{cite web|url=bar|date=2009|author=Smith}} was"), Is.EqualTo(Found));
            Assert.That(Parsers.BadCiteParameters(@"now {{cite web|url=bar|date=2009|df=mdy|collaboration=Foo|eissn=1234-5678|hdl=123.456}} was"), Is.EqualTo(Found));
            Assert.That(Parsers.BadCiteParameters(@"{{cite web | periodical=, |journal=, |newspaper=, |magazine=, |work=, |website=, |encyclopedia=, |encyclopaedia=, |dictionary=,
|arxiv=, |ARXIV=, |ASIN=, |ASIN-TLD=,
|publicationplace=, |publication-place=,
|date=, |year=, |publicationdate=, |publication-date=,
|series=, |volume=, |issue=, |number=, |page=, |pages=, |at=,
|edition=, |publisher=, |institution=,|interviewer=,|script-website=,
|journal=, |jstor=, |agency=, |archive-date=, |others=, | vauthors =, | veditors =, |translator =, |translator-last=, |translator-first=, |translator-link=, |time=,|citeseerx=,
|name-list-style=a |script-work=, |trans-quote=, |archive-format=, |lay-url=
|author-given=, |author-surname=, |display-subjects=, |interviewer-given=, |interviewer-surname=, |orig-date=, |quote-page=, |quote-pages=, |sbn=, |script-quote=, |subject-mask=, |s2cid=, |s2cid-access=, |title-link=, |trans-quote= , |translator1=, |trans-work= ,
|trans-website=,}}"), Is.EqualTo(Found));

            Assert.That(Parsers.BadCiteParameters(@"now {{cite arxiv|display-authors=0}} was"), Is.EqualTo(Found));
            Assert.That(Parsers.BadCiteParameters(@"now {{cite arxiv|vauthors=0|mode=cs2|page=0}} was"), Is.EqualTo(Found));

            // multiple errors
            Found.Add(15, 6);
            Found.Add(36, 4);
            Assert.That(Parsers.BadCiteParameters(@"now {{cite web|foodoo=bar|date=2009|bert= false}} was"), Is.EqualTo(Found));

            // no = between two bars
            Found.Clear();
            Found.Add(14, 11);
            Assert.That(Parsers.BadCiteParameters(@"now {{cite web|title-bar|url=foo}}"), Is.EqualTo(Found));

            Found.Clear();
            Found.Add(24, 6);
            Assert.That(Parsers.BadCiteParameters(@"now {{cite web|title=bar|bee |url=foo}}"), Is.EqualTo(Found));

            Found.Clear();
            Found.Add(29, 6);
            Assert.That(Parsers.BadCiteParameters(@"now {{cite web|title=bar{{a}}|bee |url=foo}}"), Is.EqualTo(Found));

            Found.Clear();
            Found.Add(30, 28);
            Assert.That(Parsers.BadCiteParameters(@"now {{cite web|title=bar |url=http://www.foo.com/bar text/}}"), Is.EqualTo(Found), "Reports URLs with spaces");

            Found.Clear();
            Assert.That(Parsers.BadCiteParameters(@"now {{cite web|title=bar |url=http://www.foo.com/bar <!--text comm-->}}"), Is.EqualTo(Found), "Ignores comments with spaces in URLs");

            // URL with space
            Found.Clear();
            Found.Add(34, 3);
            Assert.That(Parsers.BadCiteParameters(@"now {{cite web|title=bar bee |url=f a}}"), Is.EqualTo(Found));
            Found.Clear();
            Found.Add(30, 3);
            Assert.That(Parsers.BadCiteParameters(@"now {{cite web|title=f a |url=f a}}"), Is.EqualTo(Found), "reports position of url not title if have same value");
        }

        [Test]
        public void AddMissingReflist()
        {
            // above cats
            const string SingleRef = @"now <ref>foo</ref>", Cat = @"
[[Category:Here]]", ExtLinks = @"==External links==
*[http://www.site.com hello]";
            Assert.That(Parsers.AddMissingReflist(SingleRef + Cat), Is.EqualTo(SingleRef + "\r\n\r\n" + @"==References==
{{Reflist}}" + "\r\n" + Cat));

            // article has category out of place
            Assert.That(Parsers.AddMissingReflist(Cat + SingleRef), Is.EqualTo(SingleRef + "\r\n" + @"==References==
{{Reflist}}" + "\r\n" + Cat));

            // above references section
            const string References = @"==References==
{{portal|foo}}";
            Assert.That(Parsers.AddMissingReflist(SingleRef + "\r\n" + References + Cat), Is.EqualTo(SingleRef + "\r\n" + @"==References==
{{Reflist}}
{{portal|foo}}" + Cat));

            // above external links
            Assert.That(Parsers.AddMissingReflist(SingleRef + "\r\n" + ExtLinks), Is.EqualTo(SingleRef + "\r\n\r\n" + @"==References==
{{Reflist}}" + "\r\n" + ExtLinks));

            // reflist already present
            Assert.That(Parsers.AddMissingReflist(SingleRef + "\r\n" + @"==References==
{{Reflist}}" + Cat), Is.EqualTo(SingleRef + "\r\n" + @"==References==
{{Reflist}}" + Cat));

            // after tracklist, before cats
            const string Tracklist = @"
{{tracklist
| foo = bar
| foo2
}}";
            Assert.That(Parsers.AddMissingReflist(SingleRef + Tracklist + Cat), Is.EqualTo(SingleRef + Tracklist + "\r\n\r\n" + @"==References==
{{Reflist}}" + "\r\n" + Cat));

            // after s-end, before cats
            const string SEnd = @"
{{s-end}}";
            Assert.That(Parsers.AddMissingReflist(SingleRef + SEnd + Cat), Is.EqualTo(SingleRef + SEnd + "\r\n\r\n" + @"==References==
{{Reflist}}" + "\r\n" + Cat));

            // missing slash in <references>
            const string missingSlash = @"Foo <ref>a</ref>
==References==
<references>";
            Assert.That(Parsers.AddMissingReflist(missingSlash), Is.EqualTo(missingSlash.Replace(@"s>", @"s/>")));
            Assert.That(Parsers.AddMissingReflist(missingSlash.Replace("s>", "s  >")), Is.EqualTo(missingSlash.Replace(@"s>", @"s/>")), "allows whitespace around <references>");

            // missing slash in <References>
            const string missingSlash2 = @"Foo <ref>a</ref>
==References==
<References>";
            Assert.That(Parsers.AddMissingReflist(missingSlash2), Is.EqualTo(missingSlash.Replace(@"s>", @"s/>")));

            // list of references already present
            const string LDR = @"Foo <ref name='ab'/>
==References==
<references>
<ref name='ab'>abc</ref>
</references>";

            Assert.That(Parsers.AddMissingReflist(LDR), Is.EqualTo(LDR));

            // Nothing to indicate end of article: goes at end
            Assert.That(Parsers.AddMissingReflist(SingleRef), Is.EqualTo(SingleRef + "\r\n\r\n" + @"==References==
{{Reflist}}"));
        }

        [Test]
        public void AddMissingReflistEnOnly()
        {
#if DEBUG
            const string SingleRef = @"now <ref>foo</ref>", Cat = @"
[[Category:Here]]";
            Assert.That(Parsers.AddMissingReflist(SingleRef + Cat), Is.EqualTo(SingleRef + "\r\n\r\n" + @"==References==
{{Reflist}}" + "\r\n" + Cat), "add references sections above categories");

            Variables.SetProjectLangCode("fr");
            Assert.That(Parsers.AddMissingReflist(SingleRef + Cat), Is.EqualTo(SingleRef + Cat), "do nothing in non-en sites");

            Variables.SetProjectLangCode("en");
            Assert.That(Parsers.AddMissingReflist(SingleRef + Cat), Is.EqualTo(SingleRef + "\r\n\r\n" + @"==References==
{{Reflist}}" + "\r\n" + Cat));
#endif
        }

        [Test]
        public void RefsAfterPunctuation()
        {
            string AllAfter = @"Foo.<ref>bar</ref> The next Foo.<ref>bar</ref> The next Foo.<ref>bar</ref> The next Foo.<ref>bar</ref> The next";
            string R1 = @"Foo<ref>bar</ref>. The next";
            string ellipsis = @"Foo now<ref>abc</ref> ... was this.";

            Assert.That(Parsers.RefsAfterPunctuation(AllAfter.Replace(".<", "..<")), Is.EqualTo(AllAfter), "duplicate punctuation removed");

            Assert.That(Parsers.RefsAfterPunctuation(AllAfter.Replace(".<", "...<")), Is.EqualTo(AllAfter.Replace(".<", "...<")), "ellipsis punctuation NOT changed");
            Assert.That(Parsers.RefsAfterPunctuation(ellipsis + AllAfter), Is.EqualTo(ellipsis + AllAfter));

            Assert.That(Parsers.RefsAfterPunctuation(AllAfter + R1), Is.EqualTo(AllAfter + @"Foo.<ref>bar</ref> The next"), "ref moved after punctuation when majority are after");

            Assert.That(Parsers.RefsAfterPunctuation(AllAfter + @"Foo<ref>ba
r</ref>. The next"), Is.EqualTo(AllAfter + @"Foo.<ref>ba
r</ref> The next"), "ref moved after punctuation when majority are after");

            R1 = R1.Replace("Foo", "Foo ");
            Assert.That(Parsers.RefsAfterPunctuation(AllAfter + R1), Is.EqualTo(AllAfter + @"Foo.<ref>bar</ref> The next"), "Whitespace before ref cleaned when punctuation moved");

            R1 = R1.Replace(".", ",");
            Assert.That(Parsers.RefsAfterPunctuation(AllAfter + R1), Is.EqualTo(AllAfter + @"Foo,<ref>bar</ref> The next"), "handles commas too");
            Assert.That(Parsers.RefsAfterPunctuation(AllAfter + AllAfter + R1 + R1), Is.EqualTo(AllAfter + AllAfter + @"Foo,<ref>bar</ref> The next" + @"Foo,<ref>bar</ref> The next"), "multiple conversions");

            R1 = R1.Replace(",", ":");
            Assert.That(Parsers.RefsAfterPunctuation(AllAfter + R1), Is.EqualTo(AllAfter + @"Foo:<ref>bar</ref> The next"), "handles colons too");

            R1 = R1.Replace(":", "?");
            Assert.That(Parsers.RefsAfterPunctuation(AllAfter + R1), Is.EqualTo(AllAfter + @"Foo?<ref>bar</ref> The next"), "handles question mark");

            R1 = R1.Replace("?", "!");
            Assert.That(Parsers.RefsAfterPunctuation(AllAfter + R1), Is.EqualTo(AllAfter + @"Foo!<ref>bar</ref> The next"), "handles exclamation mark");

            R1 = R1.Replace("!", "–");
            Assert.That(Parsers.RefsAfterPunctuation(AllAfter + R1), Is.EqualTo(AllAfter + R1), "ref not moved when before dash");

            string TwoRefs = @"Now<ref name=a>bar</ref><ref name=b>bar</ref>. Then";

            Assert.That(Parsers.RefsAfterPunctuation(AllAfter + TwoRefs), Is.EqualTo(AllAfter + @"Now.<ref name=a>bar</ref><ref name=b>bar</ref> Then"), "punctuation moved through multiple refs");

            R1 = @"Foo<ref>bar</ref>.
The next";
            Assert.That(Parsers.RefsAfterPunctuation(AllAfter + R1), Is.EqualTo(AllAfter + @"Foo.<ref>bar</ref>
The next"), "doesn't eat newlines after ref punctuation");
            

            string RandomTable = @"{|
!title
!<ref>bar</ref>
|}";
            Assert.That(Parsers.RefsAfterPunctuation(RandomTable), Is.EqualTo(RandomTable), "does not affect wikitables");

            string Multiple = @"works<ref>{{cite book |last=McDonald }}</ref>,<ref>{{cite book |last=Gingrich }}</ref>,<ref>{{cite book |location=Norwalk, CT }}</ref>,<ref name=HerdFly/>";

            Assert.That(Parsers.RefsAfterPunctuation(Multiple), Is.EqualTo(@"works,<ref>{{cite book |last=McDonald }}</ref><ref>{{cite book |last=Gingrich }}</ref><ref>{{cite book |location=Norwalk, CT }}</ref><ref name=HerdFly/>"));

            Assert.That(Parsers.RefsAfterPunctuation(@"thing.{{sfn|Jones|2000}}.<ref>foo</ref>"), Is.EqualTo(@"thing.{{sfn|Jones|2000}}<ref>foo</ref>"), "Handles {{sfn}}");
            Assert.That(Parsers.RefsAfterPunctuation(@"thing.{{sfn|Jones|2000}}<ref>foo</ref>."), Is.EqualTo(@"thing.{{sfn|Jones|2000}}<ref>foo</ref>"), "Handles {{sfn}}");
            Assert.That(Parsers.RefsAfterPunctuation(@"thing{{sfn|Jones|2000}}<ref>foo</ref>."), Is.EqualTo(@"thing.{{sfn|Jones|2000}}<ref>foo</ref>"), "Handles {{sfn}}");
            Assert.That(Parsers.RefsAfterPunctuation(@"thing<ref>foo</ref>{{sfn|Jones|2000}}."), Is.EqualTo(@"thing.<ref>foo</ref>{{sfn|Jones|2000}}"), "Handles {{sfn}}");

            Assert.That(Parsers.RefsAfterPunctuation(@"thing.{{sfnp|Jones|2000}}.<ref>foo</ref>"), Is.EqualTo(@"thing.{{sfnp|Jones|2000}}<ref>foo</ref>"), "Handles {{sfnp}}");
            Assert.That(Parsers.RefsAfterPunctuation(@"thing.{{sfnp|Jones|2000}}<ref>foo</ref>."), Is.EqualTo(@"thing.{{sfnp|Jones|2000}}<ref>foo</ref>"), "Handles {{sfnp}}");
            Assert.That(Parsers.RefsAfterPunctuation(@"thing{{sfnp|Jones|2000}}<ref>foo</ref>."), Is.EqualTo(@"thing.{{sfnp|Jones|2000}}<ref>foo</ref>"), "Handles {{sfnp}}");
            Assert.That(Parsers.RefsAfterPunctuation(@"thing<ref>foo</ref>{{sfnp|Jones|2000}}."), Is.EqualTo(@"thing.<ref>foo</ref>{{sfnp|Jones|2000}}"), "Handles {{sfnp}}");

            Assert.That(Parsers.RefsAfterPunctuation(@"thing.{{efn|Jones|2000}}.<ref>foo</ref>"), Is.EqualTo(@"thing.{{efn|Jones|2000}}<ref>foo</ref>"), "Handles {{efn}}");
            Assert.That(Parsers.RefsAfterPunctuation(@"thing.{{efn|Jones|2000}}<ref>foo</ref>."), Is.EqualTo(@"thing.{{efn|Jones|2000}}<ref>foo</ref>"), "Handles {{efn}}");
            Assert.That(Parsers.RefsAfterPunctuation(@"thing{{efn|Jones|2000}}<ref>foo</ref>."), Is.EqualTo(@"thing.{{efn|Jones|2000}}<ref>foo</ref>"), "Handles {{efn}}");

            Assert.That(Parsers.RefsAfterPunctuation(@"thing.{{better source|Jones|2000}}.<ref>foo</ref>"), Is.EqualTo(@"thing.{{better source|Jones|2000}}<ref>foo</ref>"), "Handles {{better source}}");
            Assert.That(Parsers.RefsAfterPunctuation(@"thing.{{better source|Jones|2000}}<ref>foo</ref>."), Is.EqualTo(@"thing.{{better source|Jones|2000}}<ref>foo</ref>"), "Handles {{better source}}");
            Assert.That(Parsers.RefsAfterPunctuation(@"thing{{better source|Jones|2000}}<ref>foo</ref>."), Is.EqualTo(@"thing.{{better source|Jones|2000}}<ref>foo</ref>"), "Handles {{better source}}");
            Assert.That(Parsers.RefsAfterPunctuation(@"thing<ref>foo</ref>{{better source|Jones|2000}}."), Is.EqualTo(@"thing.<ref>foo</ref>{{better source|Jones|2000}}"), "Handles {{better source}}");

            Assert.That(Parsers.RefsAfterPunctuation(@"Start,{{sfn|S|1983|p=4}}{{sfn|H|1984|p=5}}{{sfn|M|1984|p=9}}, though."), Is.EqualTo(@"Start,{{sfn|S|1983|p=4}}{{sfn|H|1984|p=5}}{{sfn|M|1984|p=9}} though."));

            Assert.That(Parsers.RefsAfterPunctuation(@"thing{{sfn|Jones|2000}}<ref>foo</ref>{{rp|18}}."), Is.EqualTo(@"thing.{{sfn|Jones|2000}}<ref>foo</ref>{{rp|18}}"), "Handles {{sfn}} and {{rp}}");
            Assert.That(Parsers.RefsAfterPunctuation(@"thing<ref>foo</ref>{{rp|18}}{{sfn|Jones|2000}}."), Is.EqualTo(@"thing.<ref>foo</ref>{{rp|18}}{{sfn|Jones|2000}}"), "Handles {{sfn}} and {{rp}}");

            // Assert.AreEqual(@"thing.{{by whom|date=February 2014}}<ref>foo</ref>", Parsers.RefsAfterPunctuation(@"thing.{{by whom|date=February 2014}}.<ref>foo</ref>"), "Handles inline templates");
            // Assert.AreEqual(@"thing.{{by whom|date=February 2014}}<ref>foo</ref>", Parsers.RefsAfterPunctuation(@"thing.{{by whom|date=February 2014}}<ref>foo</ref>."), "Handles inline templates");
            // Assert.AreEqual(@"thing.{{by whom|date=February 2014}}<ref>foo</ref>", Parsers.RefsAfterPunctuation(@"thing{{by whom|date=February 2014}}<ref>foo</ref>."), "Handles inline templates");
            // Assert.AreEqual(@"thing.{{by whom|date=February 2014}}{{whom|date=February 2014}}", Parsers.RefsAfterPunctuation(@"thing{{by whom|date=February 2014}}{{whom|date=February 2014}}."), "Handles inline templates");
            string Excl = @"Foo!!<ref>bar</ref>";
            Assert.That(Parsers.RefsAfterPunctuation(Excl), Is.EqualTo(Excl), "No change to !! before ref, can be within table");
            Excl = @"Foo{{efn|Jones}}!!";
            Assert.That(Parsers.RefsAfterPunctuation(Excl), Is.EqualTo(Excl), "No change to !! after {{efn}}, can be within table");

            // colon may serve as wiki markup or as punctuation. We should not affect it in the first case
            Assert.That(Parsers.RefsAfterPunctuation(@"Foo<ref>something</ref>: Bar"), Is.EqualTo(@"Foo:<ref>something</ref> Bar"), "colon as punctuation");
            Assert.That(Parsers.RefsAfterPunctuation(@";Foo<ref>something</ref>: Bar"), Is.EqualTo(@";Foo<ref>something</ref>: Bar"), "no changes, colon is not a punctuation");
            Assert.That(Parsers.RefsAfterPunctuation(@"Sometimes; Foo<ref>something</ref>: Bar"), Is.EqualTo(@"Sometimes; Foo:<ref>something</ref> Bar"), "colon as punctuation");
        }

        [Test]
        public void RefsAfterPunctuationSfnInRef()
        {
            const string Correct = @"Now.<ref>{{sfn|Wilcox|2017}}; {{harvnb|Anon.|2007a|p=378}}</ref>";

            Assert.That(Parsers.RefsAfterPunctuation(Correct), Is.EqualTo(Correct), "No chnage to punct after {{sfn}} etc. when within ref");

            string x = @" thing.{{sfn|Jones|2000}}.<ref>foo</ref>";

            Assert.That(Parsers.RefsAfterPunctuation(Correct + x), Is.EqualTo(Correct + x), "No change to punctuation around {{sfn}} if also punctuation after {{sfn}} in ref tags");
        }

        [Test]
        public void RefsAfterPunctuationSpecificLanguages()
        {
#if DEBUG
            string AllAfter = @"Foo.<ref>bar</ref> The next Foo.<ref>bar</ref> The next Foo.<ref>bar</ref> The next Foo.<ref>bar</ref> The next";
            string R1 = @"Foo<ref>bar</ref>. The next";

            Variables.SetProjectLangCode("fr");
            Assert.That(Parsers.RefsAfterPunctuation(AllAfter + R1), Is.EqualTo(AllAfter + R1), "No change: disallowed language");

            Variables.SetProjectLangCode("ar");
            Assert.That(Parsers.RefsAfterPunctuation(AllAfter + R1), Is.EqualTo(AllAfter + @"Foo.<ref>bar</ref> The next"), "ref moved after reflist when majority are after");

            Variables.SetProjectLangCode("en");
            Assert.That(Parsers.RefsAfterPunctuation(AllAfter + R1), Is.EqualTo(AllAfter + @"Foo.<ref>bar</ref> The next"), "ref moved after reflist when majority are after");
#endif
        }
    }
}
