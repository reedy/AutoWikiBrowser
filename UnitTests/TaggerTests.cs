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
using System.Text.RegularExpressions;
using NUnit.Framework;
using WikiFunctions;
using WikiFunctions.Parse;

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
            // Stub, no existing stub tag. Needs all tags
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
            // Stub, no existing stub tag. Needs all tags
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text), "uncat page and orphan");
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text), "uncat page and deadend");
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
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text), "Tag for orphan");
            Assert.IsFalse(WikiRegexes.Wikify.IsMatch(text), "Don't tag for underlinked");
            Assert.IsFalse(WikiRegexes.DeadEnd.IsMatch(text), "Don't tag for deadend");
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text), "Tag for stub");
            Assert.IsTrue(Tools.NestedTemplateRegex("uncategorized stub").IsMatch(text), "Tag for uncat stub");

            // Pages with MinorPlanetListFooter will have wikilinks. They should not be tagged as deadend.
            text = parser.Tagger(@"A {{MinorPlanetListFooter|A}} B", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.DeadEnd.IsMatch(text), "Don't tag for deadend");
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
            Assert.IsTrue(text.Contains(@"{{Uncategorized stub|date=May 2010}}"));
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
            Assert.IsFalse(WikiRegexes.Unreferenced.IsMatch(text), "Unref to more citations needed when no refs 3");
            Assert.IsTrue(text.Contains("more citations needed"), "Unref when no refs 4");
            Assert.That(Tools.GetTemplateParameterValue(Tools.NestedTemplateRegex("more citations needed").Match(text).Value, "date"), Is.EqualTo("{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}"), "Date updated on change of template name");

            text = parser.Tagger(@"{{Multiple issues|{{COI|date= February 2009}}{{unreferenced|date= April 2007}}}} <ref>foo</ref>", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Unreferenced.IsMatch(text), "Unref to refimprove when has refs");
            Assert.IsTrue(text.Contains("more citations needed"), "Renames unreferenced to more citations needed in MI when existing refs");

            text = parser.Tagger(ShortText + @"{{unreferenced|date=May 2010}} <ref group=X>foo</ref>", "Test", false, out noChange, ref summary);
            Assert.IsTrue(WikiRegexes.Unreferenced.IsMatch(text), "Unref remains when only grouped footnote refs");

            text = parser.Tagger(@"{{Multiple issues|{{COI|date= February 2009}}{{unreferenced|date= April 2007}}}} {{sfn|Smith|2004}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Unreferenced.IsMatch(text), "Unref to refimprove when has sfn refs");
            Assert.IsTrue(WikiRegexes.MultipleIssues.Match(text).Value.Contains("more citations needed"), "Renames unreferenced to more citations needed in MIwhen existing refs");

            text = parser.Tagger(@"{{Multiple issues|{{COI|date= February 2009}}{{unreferenced|date= April 2007}}}} {{efn|A clarification.<ref name=Smith2009/>}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Unreferenced.IsMatch(text), "Unref to more citations needed when has efn refs");
            Assert.IsTrue(WikiRegexes.MultipleIssues.Match(text).Value.Contains("more citations needed"), "Renames unreferenced to more citations needed in MI when existing refs");

            text = parser.Tagger(ShortText + @"{{unreferenced|date=May 2010}} {{more citations needed|date=May 2010}} <ref>foo</ref>", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Unreferenced.IsMatch(text), "Unref removed when more citations needed");
            Assert.That(Tools.NestedTemplateRegex("more citations needed").Matches(text).Count, Is.EqualTo(1));

            text = parser.Tagger(ShortText + @"==Sec==
{{unreferenced|section=yes|date=May 2010}} {{more citations needed|date=May 2010}} <ref>foo</ref>", "Test", false, out noChange, ref summary);
            Assert.IsTrue(WikiRegexes.Unreferenced.IsMatch(text), "Unref retained when section template");

            text = parser.Tagger(ShortText + @"{{BLP unsourced|date=May 2010}} {{BLP sources|date=May 2010}} <ref>foo</ref>", "Test", false, out noChange, ref summary);
            Assert.That(Tools.NestedTemplateRegex("BLP unsourced").Matches(text).Count, Is.EqualTo(0));
            Assert.That(Tools.NestedTemplateRegex("BLP sources").Matches(text).Count, Is.EqualTo(1));

            text = parser.Tagger(ShortText + @"{{BLP unsourced|date=May 2010}} <ref>foo</ref>", "Test", false, out noChange, ref summary);
            Assert.That(Tools.NestedTemplateRegex("BLP unsourced").Matches(text).Count, Is.EqualTo(0));
            Assert.That(Tools.NestedTemplateRegex("BLP sources").Matches(text).Count, Is.EqualTo(1));

            text = parser.Tagger(ShortText + @"{{multiple issues|
{{unreferenced|date=May 2010}}
{{more citations needed|date=May 2010}}
}} <ref>foo[[bar]]</ref>", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Unreferenced.IsMatch(text), "Unref removed when refimprove");
            Assert.That(Tools.NestedTemplateRegex("more citations needed").Matches(text).Count, Is.EqualTo(1));
            Assert.That(Tools.NestedTemplateRegex("multiple issues").Matches(text).Count, Is.EqualTo(0), "Multiple issues removed if unref removed ant MI no longer needed");
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
            // Stub, no existing stub tag. Needs all tags
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text), "page is orphan");
            Assert.IsFalse(text.Contains("Underlinked"));

            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text), "page is deadend");
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text), "page is stub");
            Assert.IsTrue(Tools.NestedTemplateRegex("Uncategorized stub").IsMatch(text), "page is uncategorised stub");
            Assert.IsTrue(text.Contains(UncatStub), "page has already been tagged as uncategorised stub");

            text = parser.Tagger(ShortTextWithLongComment, "Test", false, out noChange, ref summary);
            // Stub, no existing stub tag. Needs all tags
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text), "page is orphan");
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text));
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text));
            Assert.IsTrue(Tools.NestedTemplateRegex("Uncategorized stub").IsMatch(text));
            Assert.IsTrue(text.Contains(UncatStub));

            text = parser.Tagger(ShortTextWithLongComment, "List of Tests", false, out noChange, ref summary);
            // Stub, no existing stub tag but with "List of..." in its title. Needs all tags but stub
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
            // Tagged article, dupe tags shouldn't be added
            Assert.That(Tools.RegexMatchCount(Regex.Escape(Stub), text), Is.EqualTo(1));
            Assert.That(Tools.RegexMatchCount(Regex.Escape(UncatStub), text), Is.EqualTo(1));
            Assert.That(Tools.RegexMatchCount(Regex.Escape(Wikify), text), Is.EqualTo(1));
            Assert.That(Tools.RegexMatchCount(Regex.Escape(Orphan), text), Is.EqualTo(1));
            Assert.That(Tools.RegexMatchCount(Regex.Escape(Deadend), text), Is.EqualTo(1));

            text = parser.Tagger(ShortText + Stub, "Test", false, out noChange, ref summary);
            // Stub, existing stub tag
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text), "page is orphan");
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text));
            Assert.IsTrue(text.Contains(UncatStub));
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text));

            Assert.IsFalse(text.Contains(Uncat));

            Assert.That(Tools.RegexMatchCount(Regex.Escape(Stub), text), Is.EqualTo(1));

            text = parser.Tagger(ShortText + ShortText, "Test", false, out noChange, ref summary);
            // Not a stub
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text), "page is orphan");
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text));
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(text));

            Assert.IsFalse(text.Contains(UncatStub));
            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text));

            text = parser.Tagger(ShortText, "Main Page", false, out noChange, ref summary);
            // Main Page should not be tagged
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
            // Categorised Stub
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text), "page is orphan");
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text));
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text));

            Assert.IsFalse(text.Contains(UncatStub));
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text));

            text = parser.Tagger(ShortText + ShortText, "Test", false, out noChange, ref summary);
            // Categorised Page
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text), "page is orphan");
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text));

            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text));
            Assert.IsFalse(text.Contains(UncatStub));
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text));

            Globals.UnitTestBoolValue = false;

            text = parser.Tagger(ShortText, "Test", false, out noChange, ref summary);
            // Non orphan categorised stub
            Assert.IsFalse(WikiRegexes.Wikify.IsMatch(text));
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text));
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text));

            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));
            Assert.IsFalse(text.Contains(UncatStub));
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text));

            text = parser.Tagger(ShortText + ShortText, "Test", false, out noChange, ref summary);
            // Non orphan categorised page
            Assert.IsFalse(WikiRegexes.Wikify.IsMatch(text));
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text));

            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text));
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));
            Assert.IsFalse(text.Contains(UncatStub));
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text));

            text = parser.Tagger(ShortText.Replace("consectetur", "[[consectetur]]"), "Test", false, out noChange, ref summary);
            // Non Deadend stub
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text));

            Assert.IsFalse(WikiRegexes.Wikify.IsMatch(text));
            Assert.IsFalse(WikiRegexes.DeadEnd.IsMatch(text));
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));
            Assert.IsFalse(text.Contains(UncatStub));
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text));

            text = parser.Tagger(Regex.Replace(ShortText, @"(\w+)", "[[$1]]"), "Test", false, out noChange, ref summary);
            // very wikified stub
            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text));
            Assert.IsFalse(WikiRegexes.Wikify.IsMatch(text));
            Assert.IsFalse(WikiRegexes.DeadEnd.IsMatch(text));
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));
            Assert.IsFalse(text.Contains(UncatStub));
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text));

            Globals.UnitTestBoolValue = false;

            // disambigs are not stubs
            text = parser.Tagger(ShortText + @" {{hndis|Bar, Foo}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text));

            // SIAs are not stubs
            text = parser.Tagger(ShortText + @" {{surname|Smith, John}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text));

            // soft redirects are not stubs
            text = parser.Tagger(ShortText + @" {{wikispecies redirect}}", "Test", false, out noChange, ref summary);
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
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(text), "uncat");
            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text), "Stub");
            Assert.IsFalse(WikiRegexes.Wikify.IsMatch(text), "underlinked");
            Assert.IsFalse(WikiRegexes.DeadEnd.IsMatch(text), "deadend");
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text), "orphan");
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

#if DEBUG
            Variables.SetProjectLangCode("simple");
            WikiRegexes.MakeLangSpecificRegexes();

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

            text = parser.Tagger(@"{{disambiguation}} [[a]]" + LongText, "Test", false, out noChange, ref summary);
            Assert.IsFalse(text.Contains(@"{{Underlinked|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"), "disambig pages with 1 link not tagged as underlinked");

            Variables.SetProjectLangCode("en");
            WikiRegexes.MakeLangSpecificRegexes();
            text = parser.Tagger(t1 + LongText, "Test", false, out noChange, ref summary);
            Assert.IsFalse(text.StartsWith(@"{{Underlinked|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}
{{temp}}"), "Don't add underlinked for en-wiki");
#endif
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
            // Stub, no existing stub tag. Sv wiki doens't have dead end nor orphan
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

            // وصلات قليلة = wikify
            text = parser.Tagger(ShortText + "{{وصلات قليلة}}", "Test", false, out noChange, ref summary);
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text), "page is deadend");
            Assert.IsFalse(WikiRegexes.Wikify.IsMatch(text), "wikify should not be present which dead end added");
            Assert.IsFalse(text.Contains("وصلات قليلة"), "wikify removed when dead end");
            Assert.IsTrue(summary.Contains("وصلات قليلة"));

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
            // Stub, no existing stub tag. Needs all tags
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text), "orphan");
            Assert.IsFalse(text.Contains("{{وصلات قليلة|" +WikiRegexes.DateYearMonthParameter + @"}}"), "wikify in Arabic");
            Assert.IsFalse(WikiRegexes.Wikify.IsMatch(text), "wikify");
            Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text), "dead end");
            Assert.IsTrue(Tools.NestedTemplateRegex("بذرة غير مصنفة").IsMatch(text), "Uncategorized stub");
            Assert.IsFalse(text.Contains("Uncategorized"), "no en-wiki uncat tags");
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text), " stub");

            text = parser.Tagger(ShortText + @"{{disambig}}", "Test", false, out noChange, ref summary);
            // Don't add stub/orphan to disambig pages. Still add wikify
            Assert.IsFalse(text.Contains("{{يتيمة|" + WikiRegexes.DateYearMonthParameter + @"}}"), "orphan");
            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text), "stub");
            // Assert.IsTrue(text.Contains("{{وصلات قليلة|" + WikiRegexes.DateYearMonthParameter + @"}}"),"wikify");

            text = parser.Tagger(ShortText + @"{{توضيح}}", "Test", false, out noChange, ref summary);
            // Don't add stub/orphan to disambig pages. Still add wikify
            Assert.IsFalse(text.Contains("{{يتيمة|" + WikiRegexes.DateYearMonthParameter + @"}}"), "orphan");
            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text), "stub");
            Assert.IsFalse(WikiRegexes.Wikify.IsMatch(text), "wikify");
            Assert.IsFalse(text.Contains("{{وصلات قليلة|" + WikiRegexes.DateYearMonthParameter + @"}}"), "wikify");
            // Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text),"deadend");
            
            text = parser.Tagger(ShortText.Replace("consectetur", "[[consectetur]]"), "Test", false, out noChange, ref summary);
            // Non Deadend stub
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text));

            Assert.IsFalse(WikiRegexes.Wikify.IsMatch(text), "wikify");
            Assert.IsFalse(WikiRegexes.DeadEnd.IsMatch(text), "deadend");
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text), "orphan");
            Assert.IsFalse(text.Contains(UncatStub), "english uncatstub");
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(text), "uncat");

            text = parser.Tagger(Regex.Replace(ShortText, @"(\w+)", "[[$1]]"), "Test", false, out noChange, ref summary);
            // very wikified stub
            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text), "stub");
            Assert.IsFalse(WikiRegexes.Wikify.IsMatch(text), "wikify");
            Assert.IsFalse(WikiRegexes.DeadEnd.IsMatch(text), "deadend");
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text), "orphan");
            Assert.IsFalse(text.Contains(UncatStub), "english uncatstub");
            Assert.IsTrue(WikiRegexes.Uncat.IsMatch(text), "uncat");

            // dead end plus orphan but one wikilink: dead end --> orphan
            text = parser.Tagger(@"{{نهاية مسدودة|تاريخ=نوفمبر 2013}}
{{يتيمة|تاريخ=نوفمبر 2013}}

[[link]]" + LongText, "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.DeadEnd.IsMatch(text), "deadend");
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text), "orphan");
            Assert.IsTrue(WikiRegexes.Wikify.IsMatch(text), "wikify");
            Assert.IsFalse(text.Contains("}}" + "\r\n\r\n" + "{{")); // no blank line between wikify & orphan

            Globals.UnitTestBoolValue = true;
            text = parser.Tagger(ShortText + ShortText + ShortText + ShortText, "Test", false, out noChange, ref summary);
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
            // Stub, no existing stub tag. Needs all tags
            Assert.IsTrue(text.Contains("{{يتيمه|" + WikiRegexes.DateYearMonthParameter + @"}}"), "orphan");
            // Assert.IsFalse(text.Contains("{{ويكى|" + WikiRegexes.DateYearMonthParameter + @"}}"),"wikify");
            // Assert.IsTrue(WikiRegexes.DeadEnd.IsMatch(text));
            Assert.IsTrue(Tools.NestedTemplateRegex("تقاوى مش متصنفه").IsMatch(text), "Uncategorized stub");
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text), "stub tag added");

            text = parser.Tagger(ShortText + @"{{disambig}}", "Test", false, out noChange, ref summary);
            // Don't add stub/orphan to disambig pages. Still add wikify
            Assert.IsFalse(text.Contains("{{يتيمه|" + WikiRegexes.DateYearMonthParameter + @"}}"), "orphan not added");
            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text), "stub tag not added");
            Assert.IsFalse(text.Contains("{{ويكى|" + WikiRegexes.DateYearMonthParameter + @"}}"), "wikify not added");

            text = parser.Tagger(ShortText + @"{{توضيح}}", "Test", false, out noChange, ref summary);
            // Don't add stub/orphan to disambig pages. Still add wikify
            Assert.IsFalse(text.Contains("{{يتيمه|" + WikiRegexes.DateYearMonthParameter + @"}}"), "orphan not added");
            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text), "stub not added");
            Assert.IsFalse(text.Contains("{{ويكى|" + WikiRegexes.DateYearMonthParameter + @"}}"), "wikify not added");

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
            // Stub, no existing stub tag. Needs all tags
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));

            text = parser.Tagger(ShortText + @"{{Widirect}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text), "pages using {{wi}} not tagged as orphan");

            text = parser.Tagger(ShortText + @"{{List of lists}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text), "pages using {{List of lists}} not tagged as orphan");

            text = parser.Tagger(ShortText + @"{{Wi}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text), "pages using {{wi}} not tagged as orphan");

            text = parser.Tagger(ShortText + @"{{soft redirect}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text), "soft redirect not tagged as orphan");

            text = parser.Tagger(ShortText, "Test", false, out noChange, ref summary);
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text), "page is orphan");

            text = parser.Tagger(@"{{Infobox foo bar|great=yes}}" + ShortText, "Test", false, out noChange, ref summary);
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text), "page is orphan");

            text = parser.Tagger(@"{{multiple issues|foo={{subst:CURRENTMONTH}} |orphan={{subst:FOOBAR}} }}" + ShortText, "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text), "tags with subst");

            Globals.UnitTestBoolValue = false;

            text = parser.Tagger(ShortText, "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));
        }

        [Test]
        public void AddTagsWikia()
        {
            #if DEBUG
            Variables.SetProjectSimple("en", ProjectEnum.wikia);
            Globals.UnitTestBoolValue = true;
            string text = parser.Tagger(@"{{Infobox foo bar|great=yes}}" + ShortText, "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text), "No orphan tag added on Wikia");

            text = parser.Tagger(ShortText + "{{Underlinked}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.DeadEnd.IsMatch(text), "page is deadend");
            Assert.IsFalse(WikiRegexes.Uncat.IsMatch(text), "uncat");
            Globals.UnitTestBoolValue = false;
            Variables.SetProjectSimple("en", ProjectEnum.wikipedia);
            #endif
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

            // Stub, tag not removed
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text));

            text = parser.Tagger(LongText + Stub, "Test", false, out noChange, ref summary);

            // stub tag removed
            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text));

            text = parser.Tagger("{{wikify}}" + Regex.Replace(LongText, @"(\w+)", "[[$1]]"), "Test", false, out noChange, ref summary);
            string text1 = parser.Tagger(Regex.Replace(LongText, @"(\w+)", "[[$1]]"), "Test", false, out noChange, ref summary);
            string text2 = parser.Tagger(Regex.Replace(LongText, @"(\w+)", "[[$1]]" + Stub), "Test", false, out noChange, ref summary);

            // wikify tag removed
            Assert.IsFalse(WikiRegexes.Wikify.IsMatch(text));
            Assert.That(text1, Is.EqualTo(text), "check whether wikify tag is removed properly");
            Assert.That(text2, Is.EqualTo(text1), "check whether stub tag is removed properly");

            text = parser.Tagger("{{wikify|reason=something}}" + Regex.Replace(LongText, @"(\w+)", "[[$1]]"), "Test", false, out noChange, ref summary);

            // wikify tag with reason NOT removed
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
            
            // Test if orphan tag is removed properly. Use wikilink and List of to prevent tagging for wikify, deadend and stub
            text = parser.Tagger(@"{{orphan}}

[[foo]]", "List of Tests", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));
            Assert.That(text, Is.EqualTo("[[foo]]"));

            // Don't remove when few parameter set
            text = parser.Tagger("{{orphan|few=a}}", "Test", false, out noChange, ref summary);
            Assert.IsTrue(WikiRegexes.Orphan.IsMatch(text));

            // multiple issues removed if tagger removes only tag in it
            text = parser.Tagger(@"{{multiple issues|
{{orphan}}
}}[[foo]]", "List of Tests", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));
            Assert.IsFalse(WikiRegexes.MultipleIssues.IsMatch(text));
            Assert.That(text, Is.EqualTo("[[foo]]"));

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

            // Test if orphan tag is removed properly. Use wikilink and disambig to prevent tagging for wikify, deadend and stub
            text = parser.Tagger("{{orphan}}[[foo]]{{disambig}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));
            Assert.That(text, Is.EqualTo("{{orphan}}[[foo]]{{disambig}}"));
            
            text = parser.Tagger("{{يتيمة}}[[foo]]{{disambig}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));
            Assert.That(text, Is.EqualTo("[[foo]]{{disambig}}"));


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

            // Test if orphan tag is removed properly. Use wikilink and disambig to prevent tagging for wikify, deadend and stub
            text = parser.Tagger("{{orphan}}[[foo]]{{disambig}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));
            Assert.That(text, Is.EqualTo("{{orphan}}[[foo]]{{disambig}}"));
            
            text = parser.Tagger("{{يتيمه}}[[foo]]{{disambig}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));
            Assert.That(text, Is.EqualTo("[[foo]]{{disambig}}"));

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

            // Test if orphan tag is removed properly. Use wikilink and disambig to prevent tagging for wikify, deadend and stub
            text = parser.Tagger("{{orphan}}[[foo]]{{disambig}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));
            Assert.That(text, Is.EqualTo("{{orphan}}[[foo]]{{disambig}}"));
            
            text = parser.Tagger("{{Föräldralös}}[[foo]]{{disambig}}", "Test", false, out noChange, ref summary);
            Assert.IsFalse(WikiRegexes.Orphan.IsMatch(text));
            Assert.That(text, Is.EqualTo("[[foo]]{{disambig}}"));

            Globals.UnitTestBoolValue = true;
            Variables.SetProjectLangCode("en");
            WikiRegexes.MakeLangSpecificRegexes();
#endif
        }

        [Test]
        public void RemoveAr()
        {
#if DEBUG
            string text = "";
            Variables.SetProjectLangCode("ar");
            WikiRegexes.MakeLangSpecificRegexes();

            Globals.UnitTestBoolValue = true;

            text = parser.Tagger(ShortText + @"{{بذرة}}", "Test", false, out noChange, ref summary);
            // Stub, tag not removed
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text));

            text = parser.Tagger(LongText + @"{{بذرة}}", "Test", false, out noChange, ref summary);
            // stub tag removed
            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text));
            
            text = parser.Tagger("{{وصلات قليلة}}" + Regex.Replace(LongText, @"(\w+)", "[[$1]]"), "Test", false, out noChange, ref summary);
            string text1 = parser.Tagger(Regex.Replace(LongText, @"(\w+)", "[[$1]]"), "Test", false, out noChange, ref summary);
            string text2 = parser.Tagger(Regex.Replace(LongText, @"(\w+)", "[[$1]]") + @"{{بذرة}}", "Test", false, out noChange, ref summary);
            // wikify tag removed
            Assert.IsFalse(WikiRegexes.Wikify.IsMatch(text));
            // Assert.AreEqual(text,text1,"check whether wikify tag is removed properly");
            Assert.That(text2, Is.EqualTo(text1), "check whether stub tag is removed properly");

            Variables.SetProjectLangCode("en");
            WikiRegexes.MakeLangSpecificRegexes();
#endif
        }
        
        [Test]
        public void RemoveArz()
        {
#if DEBUG
            string text = "";
            Variables.SetProjectLangCode("arz");
            WikiRegexes.MakeLangSpecificRegexes();

            Globals.UnitTestBoolValue = true;

            text = parser.Tagger(ShortText + @"{{بذرة}}", "Test", false, out noChange, ref summary);
            // Stub, tag not removed
            Assert.IsTrue(WikiRegexes.Stub.IsMatch(text));

            text = parser.Tagger(LongText + @"{{بذرة}}", "Test", false, out noChange, ref summary);
            // stub tag removed
            Assert.IsFalse(WikiRegexes.Stub.IsMatch(text));

            text = parser.Tagger("{{ويكى}}" + Regex.Replace(LongText, @"(\w+)", "[[$1]]"), "Test", false, out noChange, ref summary);
            string text1 = parser.Tagger(Regex.Replace(LongText, @"(\w+)", "[[$1]]"), "Test", false, out noChange, ref summary);
            string text2 = parser.Tagger(Regex.Replace(LongText, @"(\w+)", "[[$1]]") + @"{{بذرة}}", "Test", false, out noChange, ref summary);
            // wikify tag removed
            Assert.IsFalse(WikiRegexes.Wikify.IsMatch(text));
            // Assert.AreEqual(text,text1,"check whether wikify tag is removed properly");
            Assert.That(text2, Is.EqualTo(text1), "check whether stub tag is removed properly");

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
        public void RemoveDeadEnd()
        {
            Assert.IsFalse(WikiRegexes.DeadEnd.IsMatch(parser.Tagger(@"foo {{deadend|date=May 2010}} [[a]] and [[b]] and [[b]]", "Test", false, out noChange, ref summary)));
            Assert.IsTrue(summary.Contains("removed deadend tag"));

            Globals.UnitTestBoolValue = false;
            Assert.That(parser.Tagger(@"{{multiple issues|
{{dead end|date=September 2012}}
{{expert-subject|1=History|date=September 2012}}
{{Unreferenced|date=December 2006}}
}}

''Now'' [[a]] and [[b]] and [[b]]", "Test", false, out noChange, ref summary), Is.EqualTo(@"{{multiple issues|
{{expert-subject|1=History|date=September 2012}}
{{Unreferenced|date=December 2006}}
}}

''Now'' [[a]] and [[b]] and [[b]]


{{stub}}"));

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

            // Test of updating some of the non dated tags
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

            Assert.That(Parsers.TagUpdater(@"{{wikify}}"), Is.EqualTo(correct), "tags undated tag");
            Assert.That(Parsers.TagUpdater(@"{{wikify|date=}}"), Is.EqualTo(correct), "tags undated tag");
            Assert.That(Parsers.TagUpdater(@"{{wikify|dates=}}"), Is.EqualTo(correct), "tags undated tag");
            Assert.That(Parsers.TagUpdater(@"{{wikify|date}}"), Is.EqualTo(correct), "tags undated tag");
            Assert.That(Parsers.TagUpdater(@"{{wikify|Date=}}"), Is.EqualTo(correct), "tags undated tag");
            Assert.That(Parsers.TagUpdater(@"{{Template:wikify}}"), Is.EqualTo(correct), "tags undated tag, removes template namespace");
            Assert.That(Parsers.TagUpdater(@"{{template:wikify}}"), Is.EqualTo(correct), "tags undated tag, removes template namespace");
            Assert.That(Parsers.TagUpdater(@"{{Wikify}}"), Is.EqualTo(correct.Replace("wik", "Wik")), "tags undated tag, keeping existing template case");

            Assert.That(Parsers.TagUpdater(@"{{wikify|Date=May 2010}}"), Is.EqualTo(@"{{wikify|date=May 2010}}"), "corrects Date --> date");
            Assert.That(Parsers.TagUpdater(@"{{wikify|Date-May 2010}}"), Is.EqualTo(@"{{wikify|date=May 2010}}"), "corrects date- --> date=");
            Assert.That(Parsers.TagUpdater(@"{{wikify|date-=May 2010}}"), Is.EqualTo(@"{{wikify|date=May 2010}}"), "corrects date-= --> date=");
            Assert.That(Parsers.TagUpdater(@"{{wikify|Date-=May 2010}}"), Is.EqualTo(@"{{wikify|date=May 2010}}"), "corrects date-= --> date=");
            Assert.That(Parsers.TagUpdater(@"{{wikify|date-May 2010}}"), Is.EqualTo(@"{{wikify|date=May 2010}}"), "corrects date- --> date=");
            Assert.That(Parsers.TagUpdater(@"{{wikify|May 2010}}"), Is.EqualTo(@"{{wikify|date=May 2010}}"), "corrects unnamed date parameter");
            Assert.That(Parsers.TagUpdater(@"{{wikify|May 2010 }}"), Is.EqualTo(@"{{wikify|date=May 2010 }}"), "corrects unnamed date parameter");
            Assert.That(Parsers.TagUpdater(@"{{multiple issues {{wikify|May 2010 }} }}"), Is.EqualTo(@"{{multiple issues {{wikify|date=May 2010 }} }}"), "corrects unnamed date parameter, nested template");

            Assert.That(Parsers.TagUpdater(@"{{wikify|section}}"), Is.EqualTo(@"{{wikify|section|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"), "supports templates with additional arguments");
            Assert.That(Parsers.TagUpdater(@"{{wikify|section|other={{foo}} bar}}"), Is.EqualTo(@"{{wikify|section|other={{foo}} bar|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"), "supports templates with additional arguments");
            Assert.That(Parsers.TagUpdater(@"{{wikify|section|other={{foo}}}}"), Is.EqualTo(@"{{wikify|section|other={{foo}}|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}"), "supports templates with additional arguments");

            Assert.That(Parsers.TagUpdater(@"{{wikify|date=May, 2010}}"), Is.EqualTo(@"{{wikify|date=May 2010}}"), "removes excess comma");
            Assert.That(Parsers.TagUpdater(@"{{wikify|date=May 2, 2010}}"), Is.EqualTo(@"{{wikify|date=May 2, 2010}}"), "American date not altered");

            WikiRegexes.DatedTemplates.Clear();
            WikiRegexes.DatedTemplates.Add("Wikify");
            Assert.That(Parsers.TagUpdater(@"{{wikify}}"), Is.EqualTo(correct), "first letter casing of template rule does not matter");

            const string commentedOut = @"<!-- {{wikify}} -->";
            Assert.That(Parsers.TagUpdater(commentedOut), Is.EqualTo(commentedOut), "ignores commented out tags");

            WikiRegexes.DatedTemplates.Add("Clarify");
            string nochange = @"{{Clarify|date=May 2014|reason=Use Template:Cite web or similar}}";
            Assert.That(Parsers.TagUpdater(nochange), Is.EqualTo(nochange));
        }

        [Test]
        public void TagUpdaterFormatDate()
        {
            WikiRegexes.DatedTemplates.Add("Dead link");
            WikiRegexes.DatedTemplates.Add("Wikify");
            Assert.That(Parsers.TagUpdater(@"<ref>{{cite web | title=foo| url=http://www.site.com }} {{dead link}}</ref>"), Is.EqualTo(@"<ref>{{cite web | title=foo| url=http://www.site.com }} {{dead link|date=" + System.DateTime.UtcNow.ToString("MMMM yyyy", BritishEnglish) + @"}}</ref>"));

            Assert.That(Parsers.TagUpdater(@"{{wikify|date=may 2010}}"), Is.EqualTo(@"{{wikify|date=May 2010}}"), "corrects lower case month name");
            Assert.That(Parsers.TagUpdater(@"{{wikify|date=May, 2010}}"), Is.EqualTo(@"{{wikify|date=May 2010}}"), "removes comma between month and year");
            Assert.That(Parsers.TagUpdater(@"{{wikify|date=11 May 2010}}"), Is.EqualTo(@"{{wikify|date=May 2010}}"), "removes day in International date");
            Assert.That(Parsers.TagUpdater(@"{{wikify|date=07 May 2010}}"), Is.EqualTo(@"{{wikify|date=May 2010}}"), "removes day in International date");
            Assert.That(Parsers.TagUpdater(@"{{wikify|date=11 may 2010}}"), Is.EqualTo(@"{{wikify|date=May 2010}}"), "corrects lower case month name, removes day in International date");
            Assert.That(Parsers.TagUpdater(@"{{wikify|date=2010-05-13}}"), Is.EqualTo(@"{{wikify|date=May 2010}}"), "corrects lower case month name");
            Assert.That(Parsers.TagUpdater(@"{{wikify|date=MAY 2010}}"), Is.EqualTo(@"{{wikify|date=May 2010}}"), "corrects upper case month name");
            Assert.That(Parsers.TagUpdater(@"{{wikify|date=MAy 2010}}"), Is.EqualTo(@"{{wikify|date=May 2010}}"), "corrects mixed case month name");

            const string subst = @"{{wikify|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}";
            Assert.That(Parsers.TagUpdater(subst), Is.EqualTo(subst), "no change when value is subst month/year");

            const string notSubst = @"{{wikify|date={{CURRENTMONTHNAME}} {{CURRENTYEAR}}}}";
            Assert.That(Parsers.TagUpdater(notSubst), Is.EqualTo(notSubst), "no change when value is non-subst month/year keywords");

            Assert.That(Parsers.TagUpdater(@"{{wikify|date=December 2024<!-- primarily from edits on 2024-03-21 by User: -->}}"), Is.EqualTo(@"{{wikify|date=December 2024<!-- primarily from edits on 2024-03-21 by User: -->}}"), "Handles comment");
        }

        [Test]
        public void TagUpdaterLocalizeDate()
        {
            #if DEBUG
            WikiRegexes.DatedTemplates = Parsers.LoadDatedTemplates(@"{{tl|wikify}}");

            Variables.SetProjectLangCode("ar");
            WikiRegexes.MakeLangSpecificRegexes();

            Assert.That(Parsers.TagUpdater(@"{{wikify|date=May 2010}}"), Is.EqualTo(@"{{wikify|تاريخ=May 2010}}"), "Renames date= when localized");

            Variables.SetProjectLangCode("en");
            WikiRegexes.MakeLangSpecificRegexes();

            Assert.That(Parsers.TagUpdater(@"{{wikify|date=May 2010}}"), Is.EqualTo(@"{{wikify|date=May 2010}}"), "");
            #endif
        }

        [Test]
        public void General()
        {
            Globals.UnitTestBoolValue = false;
            Assert.That(parser.Tagger("#REDIRECT [[Test]]", "Test", false, out noChange, ref summary), Is.EqualTo("#REDIRECT [[Test]]"));
            Assert.IsTrue(noChange);

            Assert.That(parser.Tagger(ShortText, "Talk:Test", false, out noChange, ref summary), Is.EqualTo(ShortText));
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

            Assert.That(Parsers.Conversions(@"{{citation needed|May 2010|date=May 2010}}"), Is.EqualTo(correct));
            Assert.That(Parsers.Conversions(@"{{citation needed|may 2010|date=May 2010}}"), Is.EqualTo(correct));
            Assert.That(Parsers.Conversions(@"{{citation needed|  May 2010  |date=May 2010}}"), Is.EqualTo(correct));
            Assert.That(Parsers.Conversions(@"{{Citation needed|May 2010|date=May 2010}}"), Is.EqualTo(correct.Replace("ci", "Ci")));
        }

        [Test]
        public void MultipleIssuesNoTags()
        {
            Assert.That(parser.MultipleIssues("{{multiple issues}}"), Is.Empty);
            Assert.That(parser.MultipleIssues(@"{{multiple issues}}{{Advert|date=May 2012}}"), Is.EqualTo("{{Advert|date=May 2012}}"));
        }

        [Test]
        public void MultipleIssuesLimits()
        {
            const string bug2 = @"{{multiple issues|article=y
|update=November 2008
|out of date=July 2009
|cleanup=July 2009
|wikify=July 2009}}";

            Assert.That(Parsers.Conversions(bug2), Is.EqualTo(bug2));

            const string ManyInSection  = @"==Section==
{{multiple issues |
{{orphan}}
}}
{{dead end}}
{{multiple issues |
{{refimprove}}
}}";
            Assert.That(parser.MultipleIssues(ManyInSection), Is.EqualTo(ManyInSection));

            const string dupe  = @"{{unreferenced}}
{{unreferenced}}";
            Assert.That(parser.MultipleIssues(dupe), Is.EqualTo(dupe));

            const string LaterInSection = @"Bats.

==Con==
{{More citations needed section|date=January 2009}}
The.

{{multiple issues|section = yes|
{{unreferenced section|date = November 2014}}
{{confusing section |date = November 2014}}
}}

Chris.";
            Assert.That(parser.MultipleIssues(LaterInSection), Is.EqualTo(LaterInSection));
        }

        [Test]
        public void RedirectTaggerModDashes()
        {
            const string correct = @"#REDIRECT:[[Foo–bar]] {{R from modification}}", redirectendash = @"#REDIRECT:[[Foo–bar]]";
            Assert.That(Parsers.RedirectTagger(redirectendash, "Foo-bar"), Is.EqualTo(correct));

            // already tagged
            Assert.That(Parsers.RedirectTagger(correct, "Foo-bar"), Is.EqualTo(correct));

            // different change
            Assert.That(Parsers.RedirectTagger(redirectendash, "Foo barism"), Is.EqualTo(redirectendash));
        }

        [Test]
        public void RedirectTaggerModPunct()
        {
            const string correct = @"#REDIRECT:[[Foo .bar]] {{R from modification}}", redirectpunct = @"#REDIRECT:[[Foo .bar]]";

            // removed punct
            Assert.That(Parsers.RedirectTagger(redirectpunct, "Foo bar"), Is.EqualTo(correct));

            // different punct
            Assert.That(Parsers.RedirectTagger(redirectpunct, "Foo /bar"), Is.EqualTo(correct));

            // extra punct
            Assert.That(Parsers.RedirectTagger(redirectpunct, "Foo ..bar"), Is.EqualTo(correct));

            // other changes in addition to punctuation
            Assert.That(Parsers.RedirectTagger(redirectpunct, "Foo ..bar (long)"), Is.EqualTo(redirectpunct));
        }

        [Test]
        public void RedirectTaggerDiacr()
        {
            const string correct = @"#REDIRECT:[[Fiancée]] {{R to diacritic}}", redirectaccent = @"#REDIRECT:[[Fiancée]]";
            Assert.That(Parsers.RedirectTagger(redirectaccent, "Fiancee"), Is.EqualTo(correct));

            // already tagged
            Assert.That(Parsers.RedirectTagger(correct, "Fiancee"), Is.EqualTo(correct));

            // different change
            Assert.That(Parsers.RedirectTagger(redirectaccent, "Fiancee-bar"), Is.EqualTo(redirectaccent));
        }

        [Test]
        public void RedirectTagger()
        {
            const string redirecttext = @"#REDIRECT:[[Foo bar]]";
            const string redirecttext2 = @"#REDIRECT:[[foo bar]]";

            // skips recursive redirects
            Assert.That(Parsers.RedirectTagger(redirecttext, "Foo bar"), Is.EqualTo(redirecttext));
            Assert.That(Parsers.RedirectTagger(redirecttext2, "Foo bar"), Is.EqualTo(redirecttext2));
            Assert.That(Parsers.RedirectTagger(redirecttext, "foo bar"), Is.EqualTo(redirecttext));

            // skips if not a redirect
            string notredirect = @"Now foo bar";
            Assert.That(Parsers.RedirectTagger(notredirect, "Foo bar"), Is.EqualTo(notredirect));

            // don't tag if already tagged
            string alreadytagged = @"#REDIRECT [[Bethune's Gully]] {{R from alternative spelling}}";

            Assert.That(Parsers.RedirectTagger(alreadytagged, @"""Bethune's Gully"""), Is.EqualTo(alreadytagged));
        }

        [Test]
        public void RedirectTaggerCapitalisation()
        {
            const string correct = @"#REDIRECT:[[FooBar]] {{R from other capitalisation}}", redirectCap = @"#REDIRECT:[[FooBar]]";
            Assert.That(Parsers.RedirectTagger(redirectCap, "Foobar"), Is.EqualTo(correct));
            Assert.That(Parsers.RedirectTagger(redirectCap, "foobar"), Is.EqualTo(correct));
            Assert.That(Parsers.RedirectTagger(redirectCap, "FOObar"), Is.EqualTo(correct));
        }

        [Test]
        public void RedirectTaggerOtherNamespace()
        {
            Assert.That(Parsers.RedirectTagger(@"#REDIRECT:[[Project:FooBar]]", "FooBar"), Is.EqualTo(@"#REDIRECT:[[Project:FooBar]] {{R to project namespace}}"));
            Assert.That(Parsers.RedirectTagger(@"#REDIRECT:[[Help:FooBar]]", "FooBar"), Is.EqualTo(@"#REDIRECT:[[Help:FooBar]] {{R to help namespace}}"));
            Assert.That(Parsers.RedirectTagger(@"#REDIRECT:[[Portal:FooBar]]", "FooBar"), Is.EqualTo(@"#REDIRECT:[[Portal:FooBar]] {{R to portal namespace}}"));
            Assert.That(Parsers.RedirectTagger(@"#REDIRECT:[[Template:FooBar]]", "FooBar"), Is.EqualTo(@"#REDIRECT:[[Template:FooBar]] {{R to template namespace}}"));
            Assert.That(Parsers.RedirectTagger(@"#REDIRECT:[[Category:FooBar]]", "FooBar"), Is.EqualTo(@"#REDIRECT:[[Category:FooBar]] {{R to category namespace}}"));
            Assert.That(Parsers.RedirectTagger(@"#REDIRECT:[[User:FooBar]]", "FooBar"), Is.EqualTo(@"#REDIRECT:[[User:FooBar]] {{R to user namespace}}"));
            Assert.That(Parsers.RedirectTagger(@"#REDIRECT:[[Talk:FooBar]]", "FooBar"), Is.EqualTo(@"#REDIRECT:[[Talk:FooBar]] {{R to talk namespace}}"));
            Assert.That(Parsers.RedirectTagger(@"#REDIRECT:[[Template talk:FooBar]]", "FooBar"), Is.EqualTo(@"#REDIRECT:[[Template talk:FooBar]]"), "No change for unsupported namespace");

            const string correct = @"#REDIRECT:[[Category:FooBar]] {{R to category namespace}}", redirectNam = @"#REDIRECT:[[Category:FooBar]]";

            Assert.That(Parsers.RedirectTagger(correct, "FooBar"), Is.EqualTo(correct), "No change when already tagged");
            Assert.That(Parsers.RedirectTagger(redirectNam, "Template:FooBar"), Is.EqualTo(redirectNam), "Not tagged when redirect not in mainspace");
        }

        [Test]
        public void RedirectTaggerSection()
        {
            const string correct = @"#REDIRECT:[[Foo#Bar]] {{R to section}}";

            Assert.That(Parsers.RedirectTagger(@"#REDIRECT:[[Foo#Bar]]", "Foo"), Is.EqualTo(correct), "Tag r to section");
            Assert.That(Parsers.RedirectTagger(correct, "Foo"), Is.EqualTo(correct), "No change when already tagged");
        }

        [Test]
        public void TagRefsIbid()
        {
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
            List<Article> Cats = new List<Article>();
            Assert.That(Parsers.RegularCategories(Cats).Count, Is.EqualTo(0));

            Cats.Add(new Article("Category:Foo"));
            Assert.That(Parsers.RegularCategories(Cats).Count, Is.EqualTo(1));

            Cats.Add(new Article("Category:Bar"));
            Cats.Add(new Article("Category:Some stubs"));
            Cats.Add(new Article("Category:A :Stubs"));
            Cats.Add(new Article("Category:Stubs"));
            Assert.That(Parsers.RegularCategories(Cats).Count, Is.EqualTo(2));

            Cats.Add(new Article("Category:Proposed deletion"));
            Cats.Add(new Article("Category:Foo proposed deletions"));
            Cats.Add(new Article("Category:Foo proposed for deletion"));
            Cats.Add(new Article("Category:Articles created via the Article Wizard"));
            Assert.That(Parsers.RegularCategories(Cats).Count, Is.EqualTo(2));

            Cats.Clear();
            Assert.That(Parsers.RegularCategories("").Count, Is.EqualTo(0));
            Assert.That(Parsers.RegularCategories("[[Category:Foo]]").Count, Is.EqualTo(1));
            Assert.That(Parsers.RegularCategories("[[Category:Foo]] [[Category:Some stubs]]").Count, Is.EqualTo(1));
            Assert.That(Parsers.RegularCategories("[[Category:Foo]] <!--[[Category:Bar]]-->").Count, Is.EqualTo(1));
        }
    }
}