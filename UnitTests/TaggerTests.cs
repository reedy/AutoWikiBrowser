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

            text = parser.Tagger(@"{{Multiple issues|{{COI|date= February 2009}}{{unreferenced|date= April 2007}}}} <ref>foo</ref>", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Unreferenced.IsMatch(text), "Unref to refimprove when has refs");
            Assert.IsTrue(text.Contains("refimprove"), "Renames unreferenced to refimprove in MI when existing refs");

            text = parser.Tagger(ShortText + @"{{unreferenced|date=May 2010}} <ref group=X>foo</ref>", "Test", false, out noChange, ref summary);
            Assert.IsTrue(WikiRegexes.Unreferenced.IsMatch(text), "Unref remains when only grouped footnote refs");

            text = parser.Tagger(@"{{Multiple issues|{{COI|date= February 2009}}{{unreferenced|date= April 2007}}}} {{sfn|Smith|2004}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Unreferenced.IsMatch(text), "Unref to refimprove when has sfn refs");
            Assert.IsTrue(WikiRegexes.MultipleIssues.Match(text).Value.Contains("refimprove"), "Renames unreferenced to refimprove in MIwhen existing refs");

            text = parser.Tagger(@"{{Multiple issues|{{COI|date= February 2009}}{{unreferenced|date= April 2007}}}} {{efn|A clarification.<ref name=Smith2009/>}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Unreferenced.IsMatch(text), "Unref to refimprove when has efn refs");
            Assert.IsTrue(WikiRegexes.MultipleIssues.Match(text).Value.Contains("refimprove"), "Renames unreferenced to refimprove in MI when existing refs");

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

            text = parser.Tagger(@"{{Multiple issues|orphan={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|deadend={{subst:CURRENTMONTH}} {{subst:CURRENTYEAR}}|wikify=May 2008}}\r\n" + ShortText, "Test", false, out noChange, ref summary);
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
            Assert.IsTrue(text.Contains(@"{{uncategorized stub"), "improve cats to uncat stub");
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
            Assert.AreEqual(@"{{multiple issues|POV=May 2008|cleanup=May 2008|underlinked=June 2007}}", parser.MultipleIssuesOld(@"{{multiple issues|POV=May 2008|cleanup=May 2008|Underlinked=June 2007}}"));
            Assert.AreEqual(@"{{multiple issues|POV=May 2008|cleanup=May 2008| underlinked = June 2007}}", parser.MultipleIssuesOld(@"{{multiple issues|POV=May 2008|cleanup=May 2008| Underlinked = June 2007}}"));
            Assert.AreEqual(@"{{multiple issues|BLPunsourced=May 2008|cleanup=May 2008|underlinked=June 2007}}", parser.MultipleIssuesOld(@"{{multiple issues|BLPunsourced=May 2008|Cleanup=May 2008|underlinked=June 2007}}"));
            Assert.AreEqual(@"{{multiple issues|POV=May 2008|cleanup=May 2008|unreferencedBLP=June 2007}}", parser.MultipleIssuesOld(@"{{multiple issues|POV=May 2008|cleanup=May 2008|UnreferencedBLP=June 2007}}"));
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
            Assert.AreEqual(@"{{Multiple issues|wikify=May 2008|COI=May 2008|underlinked =March 2009|POV=May 2008}}", parser.MultipleIssuesOld(@"{{Multiple issues|wikify=May 2008|COI=May 2008|underlinked =March 2009|POV=May 2008}}{{POV}}"), "duplicate undated tag removed");
            Assert.AreEqual(@"{{Multiple issues|wikify=May 2008|COI=May 2008|underlinked =March 2009|POV=May 2008}}", parser.MultipleIssuesOld(@"{{Multiple issues|wikify=May 2008|COI=May 2008|underlinked =March 2009|POV=May 2008}}{{POV|date=June 2010}}"), "duplicate dated tag removed");
            Assert.AreEqual(@"{{Multiple issues|wikify=May 2008|COI=May 2008|underlinked =March 2009|POV=May 2008}}", parser.MultipleIssuesOld(@"{{Multiple issues|wikify=May 2008|COI=May 2008|underlinked =March 2009|POV=May 2008}}{{Underlinked}}{{POV}}"), "duplicate undated tags removed");
        }

        [Test]
        public void MultipleIssuesDates()
        {

            // removal of date word
            Assert.AreEqual(@"{{Multiple issues|wikify=May 2008|COI=May 2008|expand =March 2009|POV=May 2008}}", Parsers.Conversions(@"{{Multiple issues|wikify=May 2008|COI=May 2008|expand date=March 2009|POV=May 2008}}"));
            Assert.AreEqual(@"{{Multiple issues|wikify =May 2008|COI =May 2008|expand =March 2009|POV =May 2008}}", Parsers.Conversions(@"{{Multiple issues|wikify date=May 2008|COI date=May 2008|expand date=March 2009|POV date=May 2008}}"));
            Assert.AreEqual(@"{{Multiple issues|wikify=May 2008|COI=May 2008|expand =March 2009}}", Parsers.Conversions(@"{{Multiple issues|wikify=May 2008|COI=May 2008|expand date=March 2009}}"));
            Assert.AreEqual(@"{{Multiple issues|wikify ={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|COI ={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|expand ={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|POV ={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}", Parsers.Conversions(@"{{Multiple issues|wikify date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|COI date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|expand date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|POV date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"));
            Assert.AreEqual(@"{{Multiple issues|underlinked =February 2009|out of date =October 2009|context =October 2009}}", Parsers.Conversions(@"{{Multiple issues|underlinked =February 2009|out of date date=October 2009|context date=October 2009}}"));

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

            const string a1 = @"{{Wikify}} {{copyedit}}", a2 = @" {{COI}}", a3 = @" the article";

            // adding new {{multiple issues}}
            Assert.IsFalse(parser.MultipleIssuesOld(a1 + a2 + a3).Contains(@"{{Multiple issues|wikify = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|copyedit = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|COI}}"));

            Variables.SetProjectLangCode("en");

            Assert.IsTrue(parser.MultipleIssuesOld(a1 + a2 + a3).Contains(@"{{Multiple issues|wikify = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|copyedit = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|COI = {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"));
#endif
        }

        [Test]
        public void MultipleIssuesUnref()
        {
            string at = @"Foo
[[Category:Living people]]", ai = @"{{Multiple issues|wikify=May 2008 | underlinked=June 2007 | COI=March 2010  | unref=June 2009}}";

            Assert.AreEqual(ai.Replace("unref", "BLP unsourced") + at, parser.MultipleIssuesOld(ai + at), "unref changed if article about a person");
            Assert.AreEqual(ai.Replace("unref", "BLP unsourced") + at, parser.MultipleIssuesOld(ai.Replace("unref", "unreferenced") + at), "unreferenced changed if article about a person");

            Assert.AreEqual(ai + "foo", parser.MultipleIssuesOld(ai + "foo"), "unref not changed if article not about a person");
            Assert.AreEqual(ai + "foo {{persondata|here=there}}", parser.MultipleIssuesOld(ai + "foo {{persondata|here=there}}"), "unref not changed if article not about a living person");

            Assert.IsTrue(parser.MultipleIssuesOld(@"{{wikify|date=May 2008}} {{underlinked|date=June 2007}} {{COI|date=March 2008}} {{unref|date=June 2009}} " + at).Contains("BLP unsourced"), "unref changed if article about a person when adding {{Multiple issues}}");
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
"), "Commented out text not changed");
            Assert.IsTrue(summary.Contains("Empty section (1)"));

            twoTwos = @"==Foo1==

==Foo2==
";
            returned = parser.Tagger(twoTwos, "test", false, ref summary);
            Assert.IsTrue(returned.Contains(@"{{Empty section|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}

==Foo2==
"), "Standard empty section is tagged");

            // tagging multiple sections
            summary = "";
            returned = parser.Tagger(twoTwos + "\r\n" + twoTwos, "test", false, ref summary);
            Assert.IsTrue(returned.Contains(@"{{Empty section|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}

==Foo2==
"), "Extra whitespace handled");
            Assert.IsTrue(summary.Contains("Empty section (3)"));

            // not empty
            twoTwos = @"==Foo1==
x
==Foo2==
";
            returned = parser.Tagger(twoTwos, "test", false, ref summary);
            Assert.IsFalse(returned.Contains(@"{{Empty section|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"), "Section not empty");

            // level 3
            twoTwos = @"===Foo1===

===Foo2===
";
            returned = parser.Tagger(twoTwos, "test", false, ref summary);
            Assert.IsFalse(returned.Contains(@"{{Empty section|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"), "Empty level 3 sections not tagged");

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
}