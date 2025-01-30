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
using NUnit.Framework.Legacy;
using WikiFunctions;
using WikiFunctions.Parse;

namespace UnitTests
{
    [TestFixture]
    public class FormattingTests : RequiresParser
    {
        public GenfixesTestsBase genFixes  = new GenfixesTestsBase();

        [Test]
        public void TestBrConverter()
        {
            Assert.That(Parsers.FixSyntax("*a<br>\r\nb"), Is.EqualTo("*a\r\nb"));
            Assert.That(Parsers.FixSyntax("*a<br>"), Is.EqualTo("*a"), "Br removed at end of list when list is end of text");
            Assert.That(Parsers.FixSyntax("*a<br><br>\r\nb"), Is.EqualTo("*a\r\nb"));
            Assert.That(Parsers.FixSyntax("\r\n*a<br>\r\nb"), Is.EqualTo("*a\r\nb"));
            Assert.That(Parsers.FixSyntax("foo\r\n*a<br>\r\nb"), Is.EqualTo("foo\r\n*a\r\nb"));
            const string correct1 = "foo\r\n*a<br>b";
            Assert.That(Parsers.FixSyntax(correct1), Is.EqualTo(correct1), "No change to br tag in middle of list line");

            Assert.That(Parsers.FixSyntax("*a<br>\r\n"), Is.EqualTo("*a")); // \r\n\ trimmed

            Assert.That(Parsers.FixSyntax("*a<br\\>\r\n"), Is.EqualTo("*a"));
            Assert.That(Parsers.FixSyntax("*a<br/>\r\n"), Is.EqualTo("*a"));
            Assert.That(Parsers.FixSyntax("*a <br\\> \r\n"), Is.EqualTo("*a"));

            // leading (back)slash is hack for incorrectly formatted breaks per
            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_7#br_tags_are_not_always_removed
            // remove <br> from lists (end of list line)
            Assert.That(Parsers.FixSyntax("*a </br/> \r\n"), Is.EqualTo("*a"));
            Assert.That(Parsers.FixSyntax("*a<br\\> \r\n"), Is.EqualTo("*a"));
            Assert.That(Parsers.FixSyntax("*a <\\br\\>\r\n"), Is.EqualTo("*a"));
            Assert.That(Parsers.FixSyntax("*a     <br\\>    \r\n"), Is.EqualTo("*a"));
            Assert.That(Parsers.FixSyntax("*a <br/>\r\n"), Is.EqualTo("*a"));
            Assert.That(Parsers.FixSyntax("*a <br/ >\r\n"), Is.EqualTo("*a"));
            Assert.That(Parsers.FixSyntax("*a <br / >\r\n"), Is.EqualTo("*a"));
            Assert.That(Parsers.FixSyntax("*a <br / >\t\r\n"), Is.EqualTo("*a"), "Contains trailing tabs");
            Assert.That(Parsers.FixSyntax("*a <br / >\t\t\r\n"), Is.EqualTo("*a"), "Contains trailing tabs");

            Assert.That(Parsers.FixSyntax("*:#;a<br>\r\n*b"), Is.EqualTo("*:#;a\r\n*b"));
            Assert.That(Parsers.FixSyntax("###;;;:::***a<br />\r\nb"), Is.EqualTo("###;;;:::***a\r\nb"));
            Assert.That(Parsers.FixSyntax("*&a<br/>\r\nb"), Is.EqualTo("*&a\r\nb"));
            Assert.That(Parsers.FixSyntax("*&a<br/>\t\r\nb"), Is.EqualTo("*&a\r\nb"));

            Assert.That(Parsers.FixSyntax("&*a<br>\r\nb"), Is.EqualTo("&*a<br>\r\nb"));
            Assert.That(Parsers.FixSyntax("*a\r\n<br>\r\nb"), Is.EqualTo("*a\r\n<br>\r\nb"));
        }

        [Test]
        public void BrAfterMaintanceTemplate()
        {
            Assert.That(Parsers.FixSyntax("{{Orphan|date=September 2015}}<br>"), Is.EqualTo("{{Orphan|date=September 2015}}"), "br after a maintance template");
            Assert.That(Parsers.FixSyntax("{{Orphan|date=September 2015}}<br/>"), Is.EqualTo("{{Orphan|date=September 2015}}"), "br after a maintance template");
            Assert.That(Parsers.FixSyntax("{{Orphan|date=September 2015}}<br />"), Is.EqualTo("{{Orphan|date=September 2015}}"), "br after a maintance template");
            Assert.That(Parsers.FixSyntax("{{Orphan|date=September 2015}}{{Infobox person|name=John<br>Smith}}"), Is.EqualTo("{{Orphan|date=September 2015}}{{Infobox person|name=John<br>Smith}}"), "do not affect any other br tags");
            Assert.That(Parsers.FixSyntax("{{long ton|37|10}}<br>"), Is.EqualTo("{{long ton|37|10}}<br>"), "avoid replacements in after inline templates");
        }

        [Test]
        public void SyntaxRegexListRowBrTagStart()
        {
            Assert.That(parser.FixBrParagraphs("x<br>\r\n*abc"), Is.EqualTo("x\r\n*abc"));
            Assert.That(parser.FixBrParagraphs("x<br> \r\n*abc"), Is.EqualTo("x\r\n*abc"));
            Assert.That(parser.FixBrParagraphs("x<br/> \r\n*abc"), Is.EqualTo("x\r\n*abc"));
            Assert.That(parser.FixBrParagraphs("x<br /> \r\n*abc"), Is.EqualTo("x\r\n*abc"));
            Assert.That(parser.FixBrParagraphs("x<br / > \r\n*abc"), Is.EqualTo("x\r\n*abc"));

            genFixes.AssertNotChanged(@"{{{Foo|
param=<br>
**text
**text1 }}");

            Assert.That(parser.FixBrParagraphs(@"** Blog x
<br>
<br>
'''No"), Is.EqualTo(@"** Blog x

'''No"));
        }

        [Test]
        public void TestFixHeadings()
        {
            // breaks if article title is empty
            Assert.That(Parsers.FixHeadings("=='''foo'''==", "Heading with bold"), Is.EqualTo("==foo=="));
            Assert.That(Parsers.FixHeadings("== '''foo''' ==", "Heading with bold and space"), Is.EqualTo("== foo =="));
            Assert.That(Parsers.FixHeadings("==  '''foo'''  ==", "Heading with bold and more than one space"), Is.EqualTo("==  foo  =="));
            Assert.That(Parsers.FixHeadings("=='''foo'''==\r\n", "test"), Does.StartWith("==foo=="));
            Assert.That(Parsers.FixHeadings("quux\r\n=='''foo'''==\r\nbar", "test"), Is.EqualTo("quux\r\n\r\n==foo==\r\nbar"));
            Assert.That(Parsers.FixHeadings("quux\r\n=='''foo'''==\r\n\r\nbar", "test"), Is.EqualTo("quux\r\n\r\n==foo==\r\n\r\nbar"));

            Assert.That(Parsers.FixHeadings("==foo==", "No change"), Is.EqualTo("==foo=="));
            Assert.That(Parsers.FixHeadings("== foo ==", "No change"), Is.EqualTo("== foo =="));

            Assert.That(Parsers.FixHeadings(@"hi.
 ==News==
Some news here.", "test"), Is.EqualTo(@"hi.

==News==
Some news here."));
            Assert.That(Parsers.FixHeadings(@"hi.
 ==News place==
Some news here.", "test"), Is.EqualTo(@"hi.

==News place==
Some news here."));
            Assert.That(Parsers.FixHeadings(@"hi.
    ==News place==
Some news here.", "test"), Is.EqualTo(@"hi.

==News place==
Some news here."));
            Assert.That(Parsers.FixHeadings(@"hi.
==News place==
Some news here.", "test"), Is.EqualTo(@"hi.

==News place==
Some news here."));
            Assert.That(Parsers.FixHeadings(@"hi.

==News place==
Some news here.", "test"), Is.EqualTo(@"hi.

==News place==
Some news here."), "space trimmed from end of paragraph");

            Assert.That(Parsers.FixHeadings(@"hi.
<br>
==News place==
Some news here.", "test"), Is.EqualTo(@"hi.

==News place==
Some news here."), "space trimmed from end of paragraph when br replaces newline");

            Assert.That(Parsers.FixHeadings(@"hi.
<br>
<BR>
<BR>
==News place==
Some news here.", "test"), Is.EqualTo(@"hi.

==News place==
Some news here."), "space trimmed from end of paragraph when br replaces newline");
        }

        [Test]
        public void TestFixHeadingsRenaming()
        {
            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#ReferenceS
            Assert.That(Parsers.FixHeadings(@"==REFERENCES==", "a"), Is.EqualTo(@"==References=="));
            Assert.That(Parsers.FixHeadings(@"==REFERENCE:==", "a"), Is.EqualTo(@"==References=="));
            Assert.That(Parsers.FixHeadings(@"==REFERENSES==", "a"), Is.EqualTo(@"==References=="));
            Assert.That(Parsers.FixHeadings(@"==REFERENCE==", "a"), Is.EqualTo(@"==References=="));
            Assert.That(Parsers.FixHeadings(@"==REFERENCE:==", "a"), Is.EqualTo(@"==References=="));
            Assert.That(Parsers.FixHeadings(@"== REFERENCES ==", "a"), Is.EqualTo(@"== References =="));
            Assert.That(Parsers.FixHeadings(@"==Reference==", "a"), Is.EqualTo(@"==References=="));
            Assert.That(Parsers.FixHeadings(@"==Refferences==", "a"), Is.EqualTo(@"==References=="));
            Assert.That(Parsers.FixHeadings(@"==Referrences==", "a"), Is.EqualTo(@"==References=="));
            Assert.That(Parsers.FixHeadings(@"==Refferrences==", "a"), Is.EqualTo(@"==References=="));
            Assert.That(Parsers.FixHeadings(@"==Refrences==", "a"), Is.EqualTo(@"==References=="));
            Assert.That(Parsers.FixHeadings(@"==Reffrences==", "a"), Is.EqualTo(@"==References=="));
            Assert.That(Parsers.FixHeadings(@"==Refrence==", "a"), Is.EqualTo(@"==References=="));
            Assert.That(Parsers.FixHeadings(@"==SOURCES==", "a"), Is.EqualTo(@"==Sources=="));
            Assert.That(Parsers.FixHeadings(@"==sources==", "a"), Is.EqualTo(@"==Sources=="));
            Assert.That(Parsers.FixHeadings(@"==source==", "a"), Is.EqualTo(@"==Sources=="));
            Assert.That(Parsers.FixHeadings(@"==source:==", "a"), Is.EqualTo(@"==Sources=="));
            Assert.That(Parsers.FixHeadings(@"== SOURCES ==", "a"), Is.EqualTo(@"== Sources =="));
            Assert.That(Parsers.FixHeadings(@"==  SOURCES  ==", "a"), Is.EqualTo(@"==  Sources  =="));

            Assert.That(Parsers.FixHeadings(@"==Further reading==
            bar bar
            
==External Links==
*http://foo.com", "a"), Is.EqualTo(@"==Further reading==
            bar bar
            
==External links==
*http://foo.com"));

            Assert.That(Parsers.FixHeadings(@"some text

==Also see==", "test"), Is.EqualTo(@"some text

==See also=="), "rename also see to see also section");

            Assert.That(Parsers.FixHeadings(@"==Also see==", "test"), Is.EqualTo(@"==See also=="), "rename also see to see also section");
            Assert.That(Parsers.FixHeadings(@"==Internal links==", "test"), Is.EqualTo(@"==See also=="), "rename to see also section");
            Assert.That(Parsers.FixHeadings(@"==Related articles==", "test"), Is.EqualTo(@"==See also=="), "rename to see also section");
            Assert.That(Parsers.FixHeadings(@"Foo

==See Also==
Bar", "test"), Is.EqualTo(@"Foo

==See also==
Bar"), "See Also capitalization");
            Assert.That(Parsers.FixHeadings(@"===Related articles===", "test"), Is.EqualTo(@"===Related articles==="), "do nothing if level 3");
            Assert.That(Parsers.FixHeadings(@"some text

===Related articles===", "test"), Is.EqualTo(@"some text

===Related articles==="), "do nothing if level 3");

            Assert.That(Parsers.FixHeadings(@"Foo

==External Links==
Bar", "test"), Is.EqualTo(@"Foo

==External links==
Bar"), "External Links capitalization");
        }

        [Test]
        public void TestFixHeadingsHeadingEqualTitle()
        {
            string HeadingEqualTitle = Parsers.FixHeadings(@"
==Foo==
B", "Foo");
            ClassicAssert.IsFalse(HeadingEqualTitle.Contains("Foo"), "Heading same as title");

            HeadingEqualTitle = Parsers.FixHeadings(@"Words here
==Section==
Words there.
==Foo==
B", "Foo");
            ClassicAssert.IsTrue(HeadingEqualTitle.Contains("Foo"), "Heading same as title in later section retained");

            Assert.That(Parsers.FixHeadings(@"'''The'''.

== History ==	 
 		 
Five", "a"), Is.EqualTo(@"'''The'''.

== History ==

Five"));
        }

        [Test]
        public void TestFixHeadingsColon()
        {
            // remove colon from end of heading text
            Assert.That(Parsers.FixHeadings(@"== hello world: ==
", "a"), Is.EqualTo(@"== hello world ==
"), "removes colon");
            Assert.That(Parsers.FixHeadings(@"== hello world : ==
", "a"), Is.EqualTo(@"== hello world  ==
"), "removes colon with space");
            Assert.That(Parsers.FixHeadings(@"=== hello world: ===
", "a"), Is.EqualTo(@"== hello world ==
"), "removes colon - header level 3");
            Assert.That(Parsers.FixHeadings(@"== hello world: ==
== hello world2: ==
", "a"), Is.EqualTo(@"== hello world ==

== hello world2 ==
"), "removes colons from multiple places");

            Assert.That(Parsers.FixHeadings(@"== hello world: == 
== hello world2: ==
", "a"), Is.EqualTo(@"== hello world ==

== hello world2 ==
"), "fixes colon in heading when excess whitespace after heading end");

            // no change if colon within heading text
            Assert.That(Parsers.FixHeadings(@"== hello:world ==
", "a"), Is.EqualTo(@"== hello:world ==
"));

            Assert.That(Parsers.FixHeadings(@"== : ==
", "a"), Is.EqualTo(@"== : ==
"));
            Assert.That(Parsers.FixHeadings(@"A
 == hello world: ==
", "a"), Is.EqualTo(@"A

== hello world ==
"));
        }

        [Test]
        public void TestFixHeadingsBrRemoval()
        {
            Assert.That(Parsers.FixHeadings(@"==Foo<br>==", "a"), Is.EqualTo(@"==Foo=="));
            Assert.That(Parsers.FixHeadings(@"==Foo<br />==", "a"), Is.EqualTo(@"==Foo=="));
            Assert.That(Parsers.FixHeadings(@"==Foo< BR >==", "a"), Is.EqualTo(@"==Foo=="));
        }

        [Test]
        public void TestFixHeadingsBigRemoval()
        {
            Assert.That(Parsers.FixHeadings(@"==<big>Foo</big>==", "a"), Is.EqualTo(@"==Foo=="));
            Assert.That(Parsers.FixHeadings(@"==<Big>Foo</big>==", "a"), Is.EqualTo(@"==Foo=="));
        }

        [Test]
        public void TestFixHeadingsRemoveTwoLevels()
        {
            // single heading
            Assert.That(Parsers.FixHeadings(@"====hello====
text", "a"), Is.EqualTo(@"==hello==
text"));

            // multiple
            Assert.That(Parsers.FixHeadings(@"====hello====
text
==== hello2 ====
texty
===== hello3 =====
", "a"), Is.EqualTo(@"==hello==
text

== hello2 ==
texty

=== hello3 ===
"));

            // level 1 not altered
            Assert.That(Parsers.FixHeadings(@"=level1=
text
====hello====
", "a"), Is.EqualTo(@"=level1=
text

==hello==
"));

            // no changes if already a level two
            Assert.That(Parsers.FixHeadings(@"==hi==

====hello====
", "a"), Is.EqualTo(@"==hi==

====hello====
"));

            // no changes on level 1 only
            Assert.That(Parsers.FixHeadings(@"=hello=
text", "a"), Is.EqualTo(@"=hello=
text"));

            // don't consider the "references", "see also", or "external links" level 2 headings when counting level two headings
            // single heading
            Assert.That(Parsers.FixHeadings(@"====hello====
text
==References==
foo", "a"), Is.EqualTo(@"==hello==
text

==References==
foo"));

            Assert.That(Parsers.FixHeadings(@"====hello====
text
==External links==
foo", "a"), Is.EqualTo(@"==hello==
text

==External links==
foo"));

            ClassicAssert.IsTrue(Parsers.FixHeadings(@"====hello====
text

==See also==

==External links==
foo", "a").StartsWith(@"==hello==
text

==See also=="));

            // no change
            Assert.That(Parsers.FixHeadings(@"==hello==
text

==References==
foo", "a"), Is.EqualTo(@"==hello==
text

==References==
foo"));

            // don't apply where level 3 headings after references/external links/see also
            const string a = @"====hello====
text

==External links==
foo

===bar===
foo2";
            Assert.That(Parsers.FixHeadings(a, "a"), Is.EqualTo(a));

            const string a2 = @"text

==External links==
foo

===bar===
foo2";
            Assert.That(Parsers.FixHeadings(a2, "a"), Is.EqualTo(a2));

            // only apply on mainspace
            Assert.That(Parsers.FixHeadings(@"====hello====
text", "Talk:foo"), Is.EqualTo(@"====hello====
text"));
        }

        [Test]
        public void TestFixHeadingsBoldRemoval()
        {
            Assert.That(Parsers.FixHeadings(@"=== '''Caernarvon''' 1536-1832 ===", "a"), Is.EqualTo(@"=== Caernarvon 1536-1832 ==="));
            Assert.That(Parsers.FixHeadings(@"=== <b>Caernarvon</b> 1536-1832 ===", "a"), Is.EqualTo(@"=== Caernarvon 1536-1832 ==="));
            Assert.That(Parsers.FixHeadings(@"=== <B>Caernarvon</B> 1536-1832 ===", "a"), Is.EqualTo(@"=== Caernarvon 1536-1832 ==="));
            Assert.That(Parsers.FixHeadings(@"==='''Caernarvon''' 1536-1832===", "a"), Is.EqualTo(@"===Caernarvon 1536-1832==="));
            Assert.That(Parsers.FixHeadings(@"=== Caernarvon '''1536-1832''' ===", "a"), Is.EqualTo(@"=== Caernarvon 1536-1832 ==="));
            Assert.That(Parsers.FixHeadings(@"=== '''Caernarvon 1536-1832''' ===", "a"), Is.EqualTo(@"=== Caernarvon 1536-1832 ==="));
            Assert.That(Parsers.FixHeadings(@"==== '''Caernarvon 1536-1832''' ====", "a"), Is.EqualTo(@"==== Caernarvon 1536-1832 ===="));

            // not at level 1 or 2
            Assert.That(Parsers.FixHeadings(@"== '''Caernarvon''' 1536-1832 ==", "a"), Is.EqualTo(@"== '''Caernarvon''' 1536-1832 =="));
            Assert.That(Parsers.FixHeadings(@"= '''Caernarvon''' 1536-1832 =", "a"), Is.EqualTo(@"= '''Caernarvon''' 1536-1832 ="));

            Assert.That(Parsers.FixHeadings("=='''See Also'''==", "test"), Is.EqualTo("==See also=="), "remove bold and fix casing at once");

            Assert.That(Parsers.FixHeadings(@"=='''Header with bold'''==<br/>", "test"), Is.EqualTo(@"==Header with bold=="));
        }

        [Test]
        public void TestFixHeadingsEmpytyBoldRemoval()
        {
            Assert.That(Parsers.FixHeadings(@"==''''''Foo==", "test"), Is.EqualTo(@"==Foo=="), "empty tag in the beginning");
            Assert.That(Parsers.FixHeadings(@"==Foo''''''==", "test"), Is.EqualTo(@"==Foo=="), "empty tag at the end");
            Assert.That(Parsers.FixHeadings(@"==Foo''' ''' bar==", "test"), Is.EqualTo(@"==Foo bar=="), "empty tag in the middle");
            Assert.That(Parsers.FixHeadings(@"== Foo''' ''' bar ==", "test"), Is.EqualTo(@"== Foo bar =="), "empty tag in the middle and spaces around");
            Assert.That(Parsers.FixHeadings(@"==Foo'''   ''' bar==", "test"), Is.EqualTo(@"==Foo bar=="), "more spaces");
            Assert.That(Parsers.FixHeadings(@"== '''Foo''' ''' bar''' ==", "test"), Is.EqualTo(@"== Foo bar =="));
        }

        [Test]
        public void TestFixHeadingsBlankLineBefore()
        {
            const string correct = @"Foo

==1920s==
Bar";
            Assert.That(Parsers.FixHeadings(@"Foo

==1920s==
Bar", "Test"), Is.EqualTo(correct), "no change when already one blank line");
            Assert.That(Parsers.FixHeadings(@"Foo


==1920s==
Bar", "Test"), Is.EqualTo(correct), "fixes excess blank lines");
            Assert.That(Parsers.FixHeadings(@"Foo
==1920s==
Bar", "Test"), Is.EqualTo(correct), "inserts blank line if one missing");

            Assert.That(Parsers.FixHeadings(@"====4====
==2==
text", "Test"), Is.EqualTo(@"====4====

==2==
text"), "fixes excess blank lines, sub-heading then heading");

            Assert.That(Parsers.FixHeadings(@"==2==
====4====
text", "Test"), Is.EqualTo(@"==2==
====4====
text"), "Don't add blank line between heading and sub-heading");

            Assert.That(Parsers.FixHeadings(@"x

====Major championships====
==Wins==
x", "test"), Is.EqualTo(@"x

====Major championships====

==Wins==
x"));

            Assert.That(Parsers.FixHeadings(@"x

==Major championships==
====Wins====
x", "test"), Is.EqualTo(@"x

==Major championships==
====Wins====
x"));

            Assert.That(Parsers.FixHeadings(@"x
==Major championships==
====Wins====
x", "test"), Is.EqualTo(@"x

==Major championships==
====Wins====
x"));

            Assert.That(Parsers.FixHeadings(@"==Foo 1==
x
===Major championships===
====Wins====
x", "test"), Is.EqualTo(@"==Foo 1==
x

===Major championships===
====Wins====
x"));

            Assert.That(Parsers.FixHeadings(@"== Events ==
x
=== By place ===
==== Roman Empire ====
x", "test"), Is.EqualTo(@"== Events ==
x

=== By place ===
==== Roman Empire ====
x"));

            Assert.That(Parsers.FixHeadings(@"x
==Major championships==
====Wins====	
x", "test"), Is.EqualTo(@"x

==Major championships==
====Wins====
x"), "Excess tab whitespace in second header handled");

            const string indented = @"A
 == some code here === and here ==
 Done";
            Assert.That(Parsers.FixHeadings(indented, "Test"), Is.EqualTo(indented), "No change to indented text with other == in");

            string consecutiveSubHeadings = @"==2==
===3===
====4====
text";

            Assert.That(Parsers.FixHeadings(consecutiveSubHeadings, "Test"), Is.EqualTo(consecutiveSubHeadings), "Don't add blank line between heading and sub-heading, consecutive");

            consecutiveSubHeadings = @"==2==
===3===
====4====
=====5=====
text";

            Assert.That(Parsers.FixHeadings(consecutiveSubHeadings, "Test"), Is.EqualTo(consecutiveSubHeadings), "Don't add blank line between heading and sub-heading, consecutive x2");
        }

        [Test]
        public void TestFixHeadingsAnchor()
        {
            string correct = @"Foo
{{Anchor|Nineteentwenties}}
==1920s==
Bar";
            Assert.That(Parsers.FixHeadings(correct, "Test"), Is.EqualTo(correct), "no change to anchor just before L2 heading");

            correct = @"Foo

==Some heading==
Some text.
{{Anchors|Nineteentwenties}}
===1920s===
Bar";
            Assert.That(Parsers.FixHeadings(correct, "Test"), Is.EqualTo(correct), "no change to anchor just before L3 heading");

            correct = @"Foo

==Some heading==
Some text.
{{Anchors|Nineteentwenties}}
===1920s===

Bar";
            Assert.That(Parsers.FixHeadings(correct, "Test"), Is.EqualTo(correct), "no change to anchor just before L3 heading - newline after");
        }

        [Test]
        public void TestFixHeadingsBlankLineBeforeEnOnly()
        {
#if DEBUG
            const string correct = @"Foo

==1920s==
Bar";

            Variables.SetProjectLangCode("fr");
            Assert.That(Parsers.FixHeadings(@"Foo
==1920s==
Bar", "Test"), Is.EqualTo(@"Foo
==1920s==
Bar"), "No change – not en wiki");

            Variables.SetProjectLangCode("en");
            Assert.That(Parsers.FixHeadings(@"Foo
==1920s==
Bar", "Test"), Is.EqualTo(correct), "inserts blank line if one missing");
#endif
        }

        [Test]
        public void TestFixHeadingsBadHeaders()
        {
            ClassicAssert.IsFalse(Parsers.FixHeadings(@"==Introduction==
'''Foo''' great.", "Foo").Contains(@"==Introduction=="), "Excess heading at start");
            ClassicAssert.IsFalse(Parsers.FixHeadings(@"==Introduction:==
'''Foo''' great.", "Foo").Contains(@"==Introduction:=="), "Excess heading at start, with colon");
            ClassicAssert.IsFalse(Parsers.FixHeadings(@"=='''Introduction'''==
'''Foo''' great.", "Foo").Contains(@"Introduction"), "Excess heading at start, with bold");
            ClassicAssert.IsFalse(Parsers.FixHeadings(@"=='''Introduction:'''==
'''Foo''' great.", "Foo").Contains(@"=='''Introduction:'''=="), "Excess heading at start, with bold and colon");
            ClassicAssert.IsFalse(Parsers.FixHeadings(@"===Introduction===
'''Foo''' great.", "Foo").Contains(@"===Introduction==="), "Excess heading at start, level 3");

            ClassicAssert.IsFalse(Parsers.FixHeadings(@"==About==
'''Foo''' great.", "Foo").Contains(@"==About=="), "Excess heading at start About");
            ClassicAssert.IsFalse(Parsers.FixHeadings(@"==Description==
'''Foo''' great.", "Foo").Contains(@"==Description=="), "Excess heading at start Description");
            ClassicAssert.IsFalse(Parsers.FixHeadings(@"==Overview==
'''Foo''' great.", "Foo").Contains(@"==Overview=="), "Excess heading at start Overview");
            ClassicAssert.IsFalse(Parsers.FixHeadings(@"==Definition==
'''Foo''' great.", "Foo").Contains(@"==Definition=="), "Excess heading at start Definition");
            ClassicAssert.IsFalse(Parsers.FixHeadings(@"==Profile==
'''Foo''' great.", "Foo").Contains(@"==Profile=="), "Excess heading at start Profile");
            ClassicAssert.IsFalse(Parsers.FixHeadings(@"==General information==
'''Foo''' great.", "Foo").Contains(@"==General information=="), "Excess heading at start General information");

            Assert.That(Parsers.FixHeadings(@"==Introduction==
'''Foo''' great.", "Foo"), Is.EqualTo(@"
'''Foo''' great."), "Removes unnecessary general headers from start of article");

            Assert.That(Parsers.FixHeadings(@"==Foo==
Really great", "Foo"), Is.EqualTo(@"Really great"), "Removes heading if it matches pagetitle");

            const string L3 = @"Great article

==A==

===Foo===
Really great";
            Assert.That(Parsers.FixHeadings(L3, "Foo"), Is.EqualTo(L3), "Does not remove level 3 heading that matches pagetitle");

            const string HeadingNotAtStart = @"Foo is great.

==Overview==
Here there";
            Assert.That(Parsers.FixHeadings(HeadingNotAtStart, "Foo"), Is.EqualTo(HeadingNotAtStart), "Heading not removed when not at start of article");

            Assert.That(Parsers.FixHeadings(@"==Weblinks==
* Foo", "foo"), Is.EqualTo(@"==External links==
* Foo"));
            Assert.That(Parsers.FixHeadings(@"==Outside links==
* Foo", "foo"), Is.EqualTo(@"==External links==
* Foo"));
            Assert.That(Parsers.FixHeadings(@"==external Links==
* Foo", "foo"), Is.EqualTo(@"==External links==
* Foo"));
        }

        [Test]
        public void UnbalancedHeadings()
        {
            ClassicAssert.IsTrue(Parsers.FixHeadings(@"==External links=
*Foo", "Bar").Contains(@"==External links=="));
            ClassicAssert.IsTrue(Parsers.FixHeadings(@"==References=
{{Reflist}}", "Bar").Contains(@"==References=="));
            ClassicAssert.IsTrue(Parsers.FixHeadings(@"==See also=
*Foo1
*Foo2", "Bar").Contains(@"==See also=="));
        }


        [Test, Category("Incomplete")]
        // TODO: cover everything
        public void TestFixWhitespace()
        {
            Assert.That(Parsers.RemoveWhiteSpace("     "), Is.Empty);
            Assert.That(Parsers.RemoveWhiteSpace("a\r\n\r\n\r\n b"), Is.EqualTo("a\r\n\r\n b"));
            // Assert.AreEqual(" a", Parsers.RemoveWhiteSpace(" a")); // fails, but it doesn't seem harmful, at least for
            // WMF projects with their design guidelines
            // Assert.AreEqual(" a", Parsers.RemoveWhiteSpace("\r\n a \r\n")); // same as above
            Assert.That(Parsers.RemoveWhiteSpace("\r\na \r\n"), Is.EqualTo("a")); // the above errors have effect only on the first line
            Assert.That(Parsers.RemoveWhiteSpace("\r\n"), Is.Empty);
            Assert.That(Parsers.RemoveWhiteSpace("\r\n\r\n"), Is.Empty);
            Assert.That(Parsers.RemoveWhiteSpace("a\r\nb"), Is.EqualTo("a\r\nb"));
            Assert.That(Parsers.RemoveWhiteSpace("a\r\n\r\nb"), Is.EqualTo("a\r\n\r\nb"));
            Assert.That(Parsers.RemoveWhiteSpace("a\r\n\r\n\r\nb"), Is.EqualTo("a\r\n\r\nb"));
            Assert.That(Parsers.RemoveWhiteSpace("a\r\n\r\n\r\n\r\nb"), Is.EqualTo("a\r\n\r\nb"));
            Assert.That(Parsers.RemoveWhiteSpace("a \r\nb"), Is.EqualTo("a \r\nb"), "keep space at end of line before single line break, e.g. within infobox parameters");
            Assert.That(Parsers.RemoveWhiteSpace("a \r\n\r\nb"), Is.EqualTo("a\r\n\r\nb"));
            Assert.That(Parsers.RemoveWhiteSpace("a\u00a0\r\n\r\nb"), Is.EqualTo("a\r\n\r\nb"), "Unicode U+00A0 NO-BREAK SPACE");
            Assert.That(Parsers.RemoveWhiteSpace("a\r\n\r\n\r\n{{foo stub}}"), Is.EqualTo("a\r\n\r\n\r\n{{foo stub}}"), "two newlines before stub are kept");

            Assert.That(Parsers.RemoveWhiteSpace("== foo ==\r\n==bar"), Is.EqualTo("== foo ==\r\n==bar"));
            Assert.That(Parsers.RemoveWhiteSpace("== foo ==\r\n\r\n==bar"), Is.EqualTo("== foo ==\r\n\r\n==bar"));
            Assert.That(Parsers.RemoveWhiteSpace("== foo ==\r\n\r\n\r\n==bar"), Is.EqualTo("== foo ==\r\n\r\n==bar"));

            Assert.That(Parsers.RemoveWhiteSpace("a<br/>\r\n\r\nx"), Is.EqualTo("a\r\n\r\nx"));
            Assert.That(Parsers.RemoveWhiteSpace("a<br/> \r\n\r\nx"), Is.EqualTo("a\r\n\r\nx"));
            Assert.That(Parsers.RemoveWhiteSpace("a<br/><br/>\r\n\r\nx"), Is.EqualTo("a\r\n\r\nx"));
            Assert.That(Parsers.RemoveWhiteSpace("a\r\n\r\n<br/>\r\n\r\nx"), Is.EqualTo("a\r\n\r\nx"));
            Assert.That(Parsers.RemoveWhiteSpace("a\r\n\r\n<br/><br/>\r\n\r\nx"), Is.EqualTo("a\r\n\r\nx"));
            Assert.That(Parsers.RemoveWhiteSpace("a\r\n\r\n<br/><br/>\r\n\r\nx"), Is.EqualTo("a\r\n\r\nx"));

            Assert.That(Parsers.RemoveWhiteSpace(@"f.
<br>
<br>

*Rookie"), Is.EqualTo(@"f.

*Rookie"));

            // eh? should we fix such tables too?
            // Assert.AreEqual("{|\r\n! foo\r\n!\r\nbar\r\n|}", Parsers.RemoveWhiteSpace("{|\r\n! foo\r\n\r\n!\r\n\r\nbar\r\n|}"));

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

            Assert.That(Parsers.RemoveWhiteSpace(TrackList), Is.EqualTo(TrackList));
            
            const string BlockQuote = @"<blockquote>
Friendship betrayal<br />
They require <br />

Why miserable patient<br />
</blockquote>";

            Assert.That(Parsers.RemoveWhiteSpace(BlockQuote), Is.EqualTo(BlockQuote), "no changes to <br> tags within blockquotes");

             const string Poem = @"<poem>
Friendship betrayal
They require


Why miserable patient
</poem>";

            Assert.That(Parsers.RemoveWhiteSpace(Poem), Is.EqualTo(Poem), "no changes to newlines within poem");
            Assert.That(Parsers.RemoveWhiteSpace("a\r\n\r\n\r\nb<poem>A</poem"), Is.EqualTo("a\r\n\r\nb<poem>A</poem"), "Changes to newlines OK when poem tags don't contain excess newlines");

            Assert.That(Parsers.RemoveWhiteSpace(@"</sup>
 

Bring"), Is.EqualTo(@"</sup>

Bring"));
        }

        [Test]
        public void NewlinesinLists()
        {
            Assert.That(Parsers.RemoveWhiteSpace(@"The following items:
* ab

* ac"), Is.EqualTo(@"The following items:
* ab
* ac"));

            Assert.That(Parsers.RemoveWhiteSpace(@"The following items:
* ab
     
* ac"), Is.EqualTo(@"The following items:
* ab
* ac"), "Newlines with spaces cleaned");

            Assert.That(Parsers.RemoveWhiteSpace(@"The following items:

* ab

* ac

* ad"), Is.EqualTo(@"The following items:

* ab
* ac
* ad"));

            string TwoNewLines = @"The following items:

* ab
* ac


* ba
* bb";

            Assert.That(Parsers.RemoveWhiteSpace(TwoNewLines), Is.EqualTo(TwoNewLines), "No change to two or more newlines between list items, could be two separate lists");

            TwoNewLines = @"The following items:

* ab
* ac


* [http://www.foo.com ba]
* bb";

            Assert.That(Parsers.RemoveWhiteSpace(TwoNewLines), Is.EqualTo(TwoNewLines), "No change to two or more newlines between list items, could be two separate lists");
        }

        [Test]
        public void TestMdashesPageRanges()
        {
            Assert.That(parser.Mdashes("pp. 55-57", "test"), Is.EqualTo("pp. 55–57"));
            Assert.That(parser.Mdashes("pp. 55--57", "test"), Is.EqualTo("pp. 55–57"));
            Assert.That(parser.Mdashes("pp.&nbsp;55-57", "test"), Is.EqualTo("pp.&nbsp;55–57"));
            Assert.That(parser.Mdashes("pp. 55 - 57", "test"), Is.EqualTo("pp. 55 – 57"));
            Assert.That(parser.Mdashes("Pp. 55 - 57", "test"), Is.EqualTo("pp. 55 – 57"));
            Assert.That(parser.Mdashes("pp 55-57", "test"), Is.EqualTo("pp 55–57"));
            Assert.That(parser.Mdashes("pp 1155-1157", "test"), Is.EqualTo("pp 1155–1157"));
            Assert.That(parser.Mdashes("pages= 55-57", "test"), Is.EqualTo("pages= 55–57"));
            Assert.That(parser.Mdashes("pages = 55-57", "test"), Is.EqualTo("pages = 55–57"));
            Assert.That(parser.Mdashes("pages=55-57", "test"), Is.EqualTo("pages=55–57"));
            Assert.That(parser.Mdashes("pages=55—57", "test"), Is.EqualTo("pages=55–57"));
            Assert.That(parser.Mdashes("pages=55&#8212;57", "test"), Is.EqualTo("pages=55–57"));
            Assert.That(parser.Mdashes("pages=55&mdash;57", "test"), Is.EqualTo("pages=55–57"));

            // no change if already correct
            Assert.That(parser.Mdashes("pages=55–57", "test"), Is.EqualTo("pages=55–57"));
        }

        [Test]
        public void TestMdashesOtherRanges()
        {
            Assert.That(parser.Mdashes("55-57 miles", "test"), Is.EqualTo("55–57 miles"));
            Assert.That(parser.Mdashes("55-57 kg", "test"), Is.EqualTo("55–57 kg"));
            Assert.That(parser.Mdashes("55 - 57 kg", "test"), Is.EqualTo("55 – 57 kg"));
            Assert.That(parser.Mdashes(@"55&#8212;57 kg", "test"), Is.EqualTo("55–57 kg"));
            Assert.That(parser.Mdashes(@"55&mdash;57 kg", "test"), Is.EqualTo("55–57 kg"));
            Assert.That(parser.Mdashes("55-57&nbsp;kg", "test"), Is.EqualTo("55–57&nbsp;kg"));
            Assert.That(parser.Mdashes("55-57 Hz", "test"), Is.EqualTo("55–57 Hz"));
            Assert.That(parser.Mdashes("55-57 GHz", "test"), Is.EqualTo("55–57 GHz"));
            Assert.That(parser.Mdashes("55 - 57 m long", "test"), Is.EqualTo("55 – 57 m long"));
            Assert.That(parser.Mdashes("55 - 57 feet", "test"), Is.EqualTo("55 – 57 feet"));
            Assert.That(parser.Mdashes("55 - 57 foot", "test"), Is.EqualTo("55 – 57 foot"));
            Assert.That(parser.Mdashes("long (55 - 57 in) now", "test"), Is.EqualTo("long (55 – 57 in) now"));

            Assert.That(parser.Mdashes("55-57 meters", "test"), Is.EqualTo("55–57 meters"));
            Assert.That(parser.Mdashes("55-57 metres", "test"), Is.EqualTo("55–57 metres"));

            // dimensions, not range
            const string dimensions = @"around 34-24-34 in (86-60-86&nbsp;cm) and";
            Assert.That(parser.Mdashes(dimensions, "test"), Is.EqualTo(dimensions));

            Assert.That(parser.Mdashes("$55-57", "test"), Is.EqualTo("$55–57"));
            Assert.That(parser.Mdashes("$55 - 57", "test"), Is.EqualTo("$55 – 57"));
            Assert.That(parser.Mdashes("$55-57", "test"), Is.EqualTo("$55–57"));
            Assert.That(parser.Mdashes("$1155-1157", "test"), Is.EqualTo("$1155–1157"));
            Assert.That(parser.Mdashes("$55&mdash;57", "test"), Is.EqualTo("$55–57"));
            Assert.That(parser.Mdashes("$55&#8212;57", "test"), Is.EqualTo("$55–57"));
            Assert.That(parser.Mdashes("$55—57", "test"), Is.EqualTo("$55–57"));

            Assert.That(parser.Mdashes("5:17 AM - 5:19 AM", "test"), Is.EqualTo("5:17 AM – 5:19 AM"));
            Assert.That(parser.Mdashes("5:17 am - 5:19 am", "test"), Is.EqualTo("5:17 am – 5:19 am"));
            Assert.That(parser.Mdashes("05:17 AM - 05:19 AM", "test"), Is.EqualTo("05:17 AM – 05:19 AM"));
            Assert.That(parser.Mdashes("11:17 PM - 11:19 PM", "test"), Is.EqualTo("11:17 PM – 11:19 PM"));
            Assert.That(parser.Mdashes("11:17 pm - 11:19 pm", "test"), Is.EqualTo("11:17 pm – 11:19 pm"));
            Assert.That(parser.Mdashes("11:17 pm &mdash; 11:19 pm", "test"), Is.EqualTo("11:17 pm – 11:19 pm"));
            Assert.That(parser.Mdashes("11:17 pm &#8212; 11:19 pm", "test"), Is.EqualTo("11:17 pm – 11:19 pm"));
            Assert.That(parser.Mdashes("11:17 pm — 11:19 pm", "test"), Is.EqualTo("11:17 pm – 11:19 pm"));

            Assert.That(parser.Mdashes("Aged 5–9", "test"), Is.EqualTo("Aged 5–9"));
            Assert.That(parser.Mdashes("Aged 5–11", "test"), Is.EqualTo("Aged 5–11"));
            Assert.That(parser.Mdashes("Aged 5 – 9", "test"), Is.EqualTo("Aged 5 – 9"));
            Assert.That(parser.Mdashes("Aged 15–19", "test"), Is.EqualTo("Aged 15–19"));
            Assert.That(parser.Mdashes("Ages 15–19", "test"), Is.EqualTo("Ages 15–19"));
            Assert.That(parser.Mdashes("Aged 15–19", "test"), Is.EqualTo("Aged 15–19"));

            Assert.That(parser.Mdashes("(ages 15-18)", "test"), Is.EqualTo("(ages 15–18)"));
        }

        [Test]
        public void TestMdashes()
        {
            // double dash to emdash
            Assert.That(parser.Mdashes(@"Djali Zwan made their live debut as a quartet -- Corgan, Sweeney, Pajo and Chamberlin -- at the end of 2001", "test"), Is.EqualTo(@"Djali Zwan made their live debut as a quartet—Corgan, Sweeney, Pajo and Chamberlin—at the end of 2001"));
            Assert.That(parser.Mdashes(@"Djali Zwan made their live debut as a quartet --  Corgan, Sweeney, Pajo and Chamberlin --at the end of 2001", "test"), Is.EqualTo(@"Djali Zwan made their live debut as a quartet—Corgan, Sweeney, Pajo and Chamberlin—at the end of 2001"));
            Assert.That(parser.Mdashes(@"Djali Zwan made their live debut as a quartet -- Corgan, Sweeney, Pajo and Chamberlin--at the end of 2001", "test"), Is.EqualTo(@"Djali Zwan made their live debut as a quartet—Corgan, Sweeney, Pajo and Chamberlin—at the end of 2001"));
            Assert.That(parser.Mdashes("Eugene Ormandy--who later", "test"), Is.EqualTo("Eugene Ormandy—who later"));
            genFixes.AssertChange("[[Eugene Ormandy]]--who later", "[[Eugene Ormandy]]—who later");

            genFixes.AssertNotChanged(@"now the domain xn-- was");

            // only applied on article namespace
            genFixes.AssertNotChanged(@"Djali Zwan made their live debut as a quartet -- Corgan, Sweeney, Pajo and Chamberlin -- at the end of 2001", "Template:test");

            // precisely two dashes only
            Assert.That(parser.Mdashes(@"Djali Zwan made their live debut as a quartet --- Corgan, Sweeney, Pajo and Chamberlin - at the end of 2001", "test"), Is.EqualTo(@"Djali Zwan made their live debut as a quartet --- Corgan, Sweeney, Pajo and Chamberlin - at the end of 2001"));

            Assert.That(parser.Mdashes("m<sup>-33</sup>", "test"), Is.EqualTo("m<sup>−33</sup>")); // hyphen
            Assert.That(parser.Mdashes("m<sup>-3324</sup>", "test"), Is.EqualTo("m<sup>−3324</sup>")); // hyphen
            Assert.That(parser.Mdashes("m<sup>-2</sup>", "test"), Is.EqualTo("m<sup>−2</sup>")); // hyphen
            Assert.That(parser.Mdashes("m<sup>–2</sup>", "test"), Is.EqualTo("m<sup>−2</sup>")); // en-dash
            Assert.That(parser.Mdashes("m<sup>—2</sup>", "test"), Is.EqualTo("m<sup>−2</sup>")); // em-dash

            // already correct
            Assert.That(parser.Mdashes("m<sup>−2</sup>", "test"), Is.EqualTo("m<sup>−2</sup>"));
            Assert.That(parser.Mdashes("m<sup>2</sup>", "test"), Is.EqualTo("m<sup>2</sup>"));

            // false positive
            Assert.That(parser.Mdashes("beaten 55 - 57 in 2004", "test"), Is.EqualTo("beaten 55 - 57 in 2004"));
            Assert.That(parser.Mdashes("from 1904 – 11 May 1956 there", "test"), Is.EqualTo("from 1904 – 11 May 1956 there"));

            // two dashes to endash for numeric ranges
            Assert.That(parser.Mdashes("55--77", "test"), Is.EqualTo("55–77"));
        }

        [Test]
        public void TestTitleDashes()
        {
            // en-dash
            Assert.That(parser.Mdashes(@"The Alpher-Bethe-Gamow paper is great.", @"Alpher–Bethe–Gamow paper"), Is.EqualTo(@"The Alpher–Bethe–Gamow paper is great."));
            // em-dash
            Assert.That(parser.Mdashes(@"The Alpher-Bethe-Gamow paper is great. The Alpher-Bethe-Gamow paper is old.", @"Alpher—Bethe—Gamow paper"), Is.EqualTo(@"The Alpher—Bethe—Gamow paper is great. The Alpher—Bethe—Gamow paper is old."));

            // all hyphens, no change
            Assert.That(parser.Mdashes(@"The Alpher-Bethe-Gamow paper is great.", "Alpher-Bethe-Gamow paper"), Is.EqualTo(@"The Alpher-Bethe-Gamow paper is great."));
        }

        [Test]
        public void TestBulletListWhitespace()
        {
            Assert.That(Parsers.RemoveAllWhiteSpace(@"*    item
*            item2
* item3"), Is.EqualTo(@"* item
* item2
* item3"));

            Assert.That(Parsers.RemoveAllWhiteSpace(@"#    item
#            item2
# item3"), Is.EqualTo(@"# item
# item2
# item3"));

            Assert.That(Parsers.RemoveAllWhiteSpace(@"#    item
#            item2
#item3"), Is.EqualTo(@"# item
# item2
#item3"));
        }

        [Test]
        public void RemoveAllWhiteSpaceTests()
        {
            Assert.That(Parsers.RemoveAllWhiteSpace(@"now a"), Is.EqualTo("now a"));
            Assert.That(Parsers.RemoveAllWhiteSpace(@"now

* foo"), Is.EqualTo(@"now
* foo"));

            Assert.That(Parsers.RemoveAllWhiteSpace(@"now   was"), Is.EqualTo("now was"));

            Assert.That(Parsers.RemoveAllWhiteSpace(@"now
was"), Is.EqualTo(@"now
was"));

            Assert.That(Parsers.RemoveAllWhiteSpace(@"==hi==

was"), Is.EqualTo(@"==hi==
was"));
            Assert.That(Parsers.RemoveAllWhiteSpace(@"18 March 1980 – 12 June 1993"), Is.EqualTo(@"18 March 1980 – 12 June 1993"), "endash spacing not changed, may be correct");
            Assert.That(Parsers.RemoveAllWhiteSpace(@"in 1980 &mdash; very"), Is.EqualTo(@"in 1980&mdash;very"), "mdash spacing cleaned");
        }

        [Test]
        public void FixTemperaturesTests()
        {
            Assert.That(Parsers.FixTemperatures(@"5ºC today"), Is.EqualTo("5°C today"));
            Assert.That(Parsers.FixTemperatures(@"5ºc today"), Is.EqualTo("5°C today"));
            Assert.That(Parsers.FixTemperatures(@"5º C today"), Is.EqualTo("5°C today"));
            Assert.That(Parsers.FixTemperatures(@"5°C today"), Is.EqualTo("5°C today"));
            Assert.That(Parsers.FixTemperatures(@"5&nbsp;°C today"), Is.EqualTo("5&nbsp;°C today"));
            Assert.That(Parsers.FixTemperatures(@"5&nbsp;ºC today"), Is.EqualTo("5&nbsp;°C today"));
            Assert.That(Parsers.FixTemperatures(@"5°    C today"), Is.EqualTo("5°C today"));
            Assert.That(Parsers.FixTemperatures(@"5°C today"), Is.EqualTo("5°C today"));
            Assert.That(Parsers.FixTemperatures(@"5°F today"), Is.EqualTo("5°F today"));
            Assert.That(Parsers.FixTemperatures(@"75°f today"), Is.EqualTo("75°F today"));

            Assert.That(Parsers.FixTemperatures(@"5ºCC"), Is.EqualTo("5ºCC"));
        }

        [Test]
        public void FixOrdinalTests()
        {
            Assert.That(Parsers.FixSyntax(@"1<sup>st</sup> day at school"), Is.EqualTo("1st day at school"), "1st");
            Assert.That(Parsers.FixSyntax(@"2<sup>nd</sup> day at school"), Is.EqualTo("2nd day at school"), "2nd");
            Assert.That(Parsers.FixSyntax(@"3<sup>rd</sup> day at school"), Is.EqualTo("3rd day at school"), "3rd");
            Assert.That(Parsers.FixSyntax(@"4<sup>th</sup> day at school"), Is.EqualTo("4th day at school"), "4th");

            Assert.That(Parsers.FixSyntax(@"1<sup>st </sup> day at school"), Is.EqualTo("1st day at school"), "1st with whitespace");
            Assert.That(Parsers.FixSyntax(@"1<sup> st </sup> day at school"), Is.EqualTo("1st day at school"), "1st with whitespace");        }


        [Test]
        public void FixSmallTags()
        {
            const string s1 = @"<small>foo</small>";
            Assert.That(Parsers.FixSyntax(s1), Is.EqualTo(s1));

            Assert.That(Parsers.FixSyntax(@"<ref><small>foo</small></ref>"), Is.EqualTo(@"<ref>foo</ref>"), "removes small from ref tags");
            Assert.That(Parsers.FixSyntax(@"<REF><small>foo</small></REF>"), Is.EqualTo(@"<REF>foo</REF>"), "removes small from ref tags");
            Assert.That(Parsers.FixSyntax(@"<sup><small>foo</small></sup>"), Is.EqualTo(@"<sup>foo</sup>"), "removes small from sup tags");
            Assert.That(Parsers.FixSyntax(@"<sub><small>foo</small></sub>"), Is.EqualTo(@"<sub>foo</sub>"), "removes small from sub tags");
            Assert.That(Parsers.FixSyntax(@"<small>a <small>foo</small> b</small>"), Is.EqualTo(@"<small>a foo b</small>"), "removes nested small from small tags");

            Assert.That(Parsers.FixSyntax(@"<small><ref>foo</ref></small>"), Is.EqualTo(@"<ref>foo</ref>"), "removes small around ref tags");
            Assert.That(Parsers.FixSyntax(@"<small><REF>foo</REF></small>"), Is.EqualTo(@"<REF>foo</REF>"), "removes small around ref tags");
            Assert.That(Parsers.FixSyntax(@"<small><sup>foo</sup></small>"), Is.EqualTo(@"<sup>foo</sup>"), "removes small around sup tags");
            Assert.That(Parsers.FixSyntax(@"<small><sub>foo</sub></small>"), Is.EqualTo(@"<sub>foo</sub>"), "removes small around sub tags");

            const string unclosedTag = @"<ref><small>foo</small></ref> now <small>";
            Assert.That(Parsers.FixSyntax(unclosedTag), Is.EqualTo(unclosedTag));
            
             const string unclosedTag2 = @"<small>
{|
...<small><ref>foo</ref></small>...
|}</small>";
            Assert.That(Parsers.FixSyntax(unclosedTag2), Is.EqualTo(unclosedTag2), "No change to small tags across table end, implies multiple incorrect small tags");

            const string NoSmall = @"<ref>foo</ref> <small>A</small>";
            Assert.That(Parsers.FixSyntax(NoSmall), Is.EqualTo(NoSmall), "No change when no small that should be removed");
        }

        [Test]
        public void FixSmallTagsImageDescriptions()
        {
            Assert.That(Parsers.FixSyntax(@"[[File:foo.jpg|<small>bar</small>]]"), Is.EqualTo(@"[[File:foo.jpg|bar]]"), "removes small from image descriptions");
            Assert.That(Parsers.FixSyntax(@"[[File:foo.jpg|thumb|<small>bar</small>]]"), Is.EqualTo(@"[[File:foo.jpg|thumb|bar]]"), "removes small from image descriptions when other parameters too");

            const string FileCaptionLegend = @"[[File:foo.jpg|This text is a caption
{{legend|foo|<small>a</small>}}
{{legend|foo2|<small>b</small>}}
]]";
            Assert.That(Parsers.FixSyntax(FileCaptionLegend), Is.EqualTo(FileCaptionLegend), "small tag not removed from legend template");
        }

        [Test]
        public void LoadTemplateRedirects()
        {
            Dictionary<Regex, string> TemplateRedirects = new Dictionary<Regex, string>();

            Assert.That(Parsers.LoadTemplateRedirects(""), Is.EqualTo(TemplateRedirects), "returns empty dictionary when no rules present");

            TemplateRedirects.Add(Tools.NestedTemplateRegex("Cn"), "Citation needed");

            Assert.That(Parsers.LoadTemplateRedirects("{{tl|Cn}} → {{tl|Citation needed}}").Values, Is.EqualTo(TemplateRedirects.Values), "loads single redirect rules");
            Assert.That(Parsers.LoadTemplateRedirects("{{tl|Cn}} → '''{{tl|Citation needed}}'''").Values, Is.EqualTo(TemplateRedirects.Values), "loads single redirect rules");
            ClassicAssert.IsTrue(WikiRegexes.AllTemplateRedirects.IsMatch("{{cn}}"));

            TemplateRedirects.Clear();
            TemplateRedirects.Add(Tools.NestedTemplateRegex(new[] { "Cn", "fact" }), "Citation needed");

            Assert.That(Parsers.LoadTemplateRedirects("{{tl|Cn}}, {{tl|fact}} → {{tl|Citation needed}}").Values, Is.EqualTo(TemplateRedirects.Values), "loads multiple redirect rules");
            ClassicAssert.IsTrue(WikiRegexes.AllTemplateRedirects.IsMatch("{{cn}}"));
            ClassicAssert.IsTrue(WikiRegexes.AllTemplateRedirects.IsMatch("{{fact}}"));
        }

        [Test]
        public void LoadDatedTemplates()
        {
            List<string> DatedTemplates = new List<string>();

            Assert.That(Parsers.LoadDatedTemplates(""), Is.EqualTo(DatedTemplates), "returns empty list when no rules present");

            Assert.That(Parsers.LoadDatedTemplates("<!--{{tl|wikif-->"), Is.EqualTo(DatedTemplates), "ignores commented out rules");

            DatedTemplates.Add("Wikify");
            Assert.That(Parsers.LoadDatedTemplates(@"{{tl|wikify}}").ToString(), Is.EqualTo(DatedTemplates.ToString()), "loads single rule");

            DatedTemplates.Add("Citation needed");
            Assert.That(Parsers.LoadDatedTemplates(@"{{tl|wikify}}
{{tl|citation needed}}").ToString(), Is.EqualTo(DatedTemplates.ToString()), "loads multiple rules");
        }

        [Test]
        public void TemplateRedirects()
        {
            Dictionary<Regex, string> TemplateRedirs = new Dictionary<Regex, string>();
            WikiRegexes.AllTemplateRedirects = null;
            Assert.That(Parsers.TemplateRedirects("now {{Cn}} was", TemplateRedirs), Is.EqualTo("now {{Cn}} was"), "no change when redirects dictionary not built");
            TemplateRedirs = Parsers.LoadTemplateRedirects("{{tl|Cn}}, {{tl|fact}} → {{tl|Citation needed}}");

            Assert.That(Parsers.TemplateRedirects("now was", TemplateRedirs), Is.EqualTo("now was"), "no change when no templates in text");
            Assert.That(Parsers.TemplateRedirects("now {{Cn}} was", TemplateRedirs), Is.EqualTo("now {{Citation needed}} was"));
            Assert.That(Parsers.TemplateRedirects("now {{cn}} was", TemplateRedirs), Is.EqualTo("now {{citation needed}} was"));
            Assert.That(Parsers.TemplateRedirects("now {{cn}} was{{fact}} or", TemplateRedirs), Is.EqualTo("now {{citation needed}} was{{citation needed}} or"), "renames multiple different redirects");
            Assert.That(Parsers.TemplateRedirects("now {{cn}} was{{cn}} or", TemplateRedirs), Is.EqualTo("now {{citation needed}} was{{citation needed}} or"), "renames multiple redirects");

            Assert.That(Parsers.TemplateRedirects(@"now {{one|
{{cn}} 
}}", TemplateRedirs), Is.EqualTo(@"now {{one|
{{citation needed}} 
}}"), "renames when template nested");

            Assert.That(Parsers.TemplateRedirects(@"now {{fact|
{{cn}} 
}}", TemplateRedirs), Is.EqualTo(@"now {{citation needed|
{{citation needed}} 
}}"), "renames nested templates, both levels");

            Assert.That(Parsers.TemplateRedirects(@"now {{one|
{{two|
{{cn}} 
}}}}", TemplateRedirs), Is.EqualTo(@"now {{one|
{{two|
{{citation needed}} 
}}}}"), "renames when template double nested");

            TemplateRedirs = Parsers.LoadTemplateRedirects("{{tl|Cn}}, {{tl|fact}} → {{tl|citation needed}}");
            Assert.That(Parsers.TemplateRedirects("now {{cn}} was", TemplateRedirs), Is.EqualTo("now {{citation needed}} was"), "follows case of new template name");
            Assert.That(Parsers.TemplateRedirects("now {{Cn}} was", TemplateRedirs), Is.EqualTo("now {{Citation needed}} was"), "follows case of new template name");
            
            TemplateRedirs = Parsers.LoadTemplateRedirects(@"{{tl|Cn}}, {{tl|fact}} → {{tl|citation needed}}
{{tl|foo}} → {{tl|bar}}");
            Assert.That(Parsers.TemplateRedirects("now {{cn}} was", TemplateRedirs), Is.EqualTo("now {{citation needed}} was"), "follows case of new template name");

            TemplateRedirs = Parsers.LoadTemplateRedirects(@"{{tl|Articleissues}} → {{tl|multiple issues}}
{{tl|Rewrite}} → {{tl|cleanup-rewrite}}
{{tl|Cn}}, {{tl|fact}} → {{tl|citation needed}}");

            Assert.That(Parsers.TemplateRedirects(@"now {{Articleissues|
{{Refimprove|date=July 2012}}
{{Citation style|date=July 2012}}
{{Rewrite|date=July 2012}}
}} was", TemplateRedirs), Is.EqualTo(@"now {{Multiple issues|
{{Refimprove|date=July 2012}}
{{Citation style|date=July 2012}}
{{Cleanup-rewrite|date=July 2012}}
}} was"), "nested template example");

            const string A = @"{{Infobox musical artist
| name       = A
| birth_date  = {{Birth date and age|1989|10|17}}
| genre      = [[Compas music|Konpa]]{{Cn|date=January 2013}}
| label      = Recording
| website        = {{URL|a.com}}
}}";

            Assert.That(Parsers.TemplateRedirects(A, TemplateRedirs), Is.EqualTo(A.Replace(@"{{Cn", @"{{Citation needed")));
            
            string B = @"{{ONE | {{TWO | {{THREE | {{Cn}} }} }} }}";

            Assert.That(Parsers.TemplateRedirects(B, TemplateRedirs), Is.EqualTo(B.Replace(@"{{Cn", @"{{Citation needed")));

            TemplateRedirs = Parsers.LoadTemplateRedirects("{{tl|Infobox Play}} → {{tl|Infobox play}}");
            Assert.That(Parsers.TemplateRedirects("{{Infobox_Play}}", TemplateRedirs), Is.EqualTo("{{Infobox play}}"), "");

            // when magic word
            TemplateRedirs = Parsers.LoadTemplateRedirects("{{tl|Display title}}, {{tl|Displaytitle}} → {{tl|DISPLAYTITLE}}");

            Assert.That(Parsers.TemplateRedirects("now {{display title|Foo}} was", TemplateRedirs), Is.EqualTo("now {{DISPLAYTITLE:Foo}} was"), "Magic word template redirected and formatted");
            Assert.That(Parsers.TemplateRedirects("now {{displaytitle|Foo}} was", TemplateRedirs), Is.EqualTo("now {{DISPLAYTITLE:Foo}} was"), "Magic word template redirected and formatted");
            Assert.That(Parsers.TemplateRedirects("now {{Displaytitle|Foo}} was", TemplateRedirs), Is.EqualTo("now {{DISPLAYTITLE:Foo}} was"), "Magic word template redirected and formatted");
        }

        [Test]
        public void TemplateRedirectsAcronyms()
        {
            Dictionary<Regex, string> theTemplateRedirects = Parsers.LoadTemplateRedirects("{{tl|fb}}, {{tl|foob}} → {{tl|FOO bar}}");

            // for acronym templates enforce first letter uppercase
            Assert.That(Parsers.TemplateRedirects("now {{fb}} was", theTemplateRedirects), Is.EqualTo("now {{FOO bar}} was"));
            Assert.That(Parsers.TemplateRedirects("now {{Fb}} was", theTemplateRedirects), Is.EqualTo("now {{FOO bar}} was"));
            Assert.That(Parsers.TemplateRedirects("now {{foob}} was", theTemplateRedirects), Is.EqualTo("now {{FOO bar}} was"));
            Assert.That(Parsers.TemplateRedirects("now {{Foob}} was", theTemplateRedirects), Is.EqualTo("now {{FOO bar}} was"));

            theTemplateRedirects = Parsers.LoadTemplateRedirects("{{tl|fb}}, {{tl|foob}} → {{tl|fOO bar}}");
            Assert.That(Parsers.TemplateRedirects("now {{fb}} was", theTemplateRedirects), Is.EqualTo("now {{fOO bar}} was"), "first letter case respected for non-acronym template");
            Assert.That(Parsers.TemplateRedirects("now {{Fb}} was", theTemplateRedirects), Is.EqualTo("now {{FOO bar}} was"));
        }

        [Test]
        public void RenameTemplateParameters()
        {
            List<WikiRegexes.TemplateParameters> RenamedTemplateParameters = Parsers.LoadRenamedTemplateParameters(@"{{AWB rename template parameter|cite web|acccessdate|accessdate}}");

            const string correct = @"{{cite web | url=http://www.site.com | title = Testing | accessdate = 20 May 2009 }}";
            Assert.That(Parsers.RenameTemplateParameters(correct.Replace("accessdate", "acccessdate"), RenamedTemplateParameters), Is.EqualTo(correct), "renames parameter in simple template call");

            const string correct2 = @"<ref>{{cite web | url=http://www.site.com | title = Testing | accessdate = 20 May 2009 }}</ref><ref>{{cite web | url=http://www.site2.com | title = Testing2 | accessdate = 2 May 2009 }}</ref>";
            Assert.That(Parsers.RenameTemplateParameters(correct2.Replace("accessdate", "acccessdate"), RenamedTemplateParameters), Is.EqualTo(correct2), "renames parameter in simple template call");

            const string correct2a = @"<ref>{{cite web | url=http://www.site.com | title = Testing |accessdate=20 May 2009 }}</ref><ref>{{cite web | url=http://www.site2.com | title = Testing2 |accessdate=2 May 2009 }}</ref>";
            Assert.That(Parsers.RenameTemplateParameters(correct2a.Replace("accessdate", "acccessdate"), RenamedTemplateParameters), Is.EqualTo(correct2a), "renames parameter in simple template call, unspaced");

            const string correct2b = @"<ref>{{cite web | url=http://www.site.com | title = Testing
|accessdate=20 May 2009 }}</ref><ref>{{cite web | url=http://www.site2.com | title = Testing2 |accessdate=2 May 2009 }}</ref>";
            Assert.That(Parsers.RenameTemplateParameters(correct2b.Replace("accessdate", "acccessdate"), RenamedTemplateParameters), Is.EqualTo(correct2b), "renames parameter in simple template call, newline");

            const string correct3 = @"{{Cite web | url=http://www.site.com | title = Testing | accessdate = 20 May 2009 }}";
            Assert.That(Parsers.RenameTemplateParameters(correct3.Replace("accessdate", "acccessdate"), RenamedTemplateParameters), Is.EqualTo(correct3), "renames parameter in simple template call, handles first letter casing of template name");

            const string correct4 = @"{{ cite_web | url=http://www.site.com | title = Testing | accessdate = 20 May 2009 }}";
            Assert.That(Parsers.RenameTemplateParameters(correct4.Replace("accessdate", "acccessdate"), RenamedTemplateParameters), Is.EqualTo(correct4), "renames parameter in simple template call, handles underscore in template name");

            string Dupe = @"{{cite web | url=http://www.site.com | title = Testing | accessdate = 20 May 2012 | acccessdate = 11 June 2012 }}";
            Assert.That(Parsers.RenameTemplateParameters(Dupe, RenamedTemplateParameters), Is.EqualTo(Dupe), "no change when target parameter already has a value");

            string Dupe2 = @"{{cite web | url=http://www.site.com | title = Testing | accessdate = | acccessdate = 11 June 2012 }}";
            Assert.That(Parsers.RenameTemplateParameters(Dupe2, RenamedTemplateParameters), Is.EqualTo(@"{{cite web | url=http://www.site.com | title = Testing | accessdate = | accessdate = 11 June 2012 }}"), "changed when target parameter present without value");

            const string nested = @"{{reflist|refs={{cite web | url=http://www.site.com | title = Testing | accessdate = 20 May 2009 }}}}";
            Assert.That(Parsers.RenameTemplateParameters(nested.Replace("accessdate", "acccessdate"), RenamedTemplateParameters), Is.EqualTo(nested), "renames parameter in nested template call");

            const string nested2 = @"{{reflist|refs={{cite web | url=http://www.site.com | title = Testing | accessdate = 20 May 2009 }}
{{cite web | url=http://www.site2.com | title = Testing2 | accessdate = 20 June 2009 }}}}";
            Assert.That(Parsers.RenameTemplateParameters(nested2.Replace("accessdate", "acccessdate"), RenamedTemplateParameters), Is.EqualTo(nested2), "renames parameter in nested and unnested template calls");

            string nomatch = @"{{cite web | url=http://www.site.com | title = Testing | accessdate = 20 May 2009 }}";
            Assert.That(Parsers.RenameTemplateParameters(nomatch, RenamedTemplateParameters), Is.EqualTo(nomatch), "No change when no matched parameters");

            nomatch = @"{{cite web | url=http://www.site.com | title = Testing | accessdate = 20 May 2009 }} {{cn}}";
            Assert.That(Parsers.RenameTemplateParameters(nomatch, RenamedTemplateParameters), Is.EqualTo(nomatch), "No change when no matched parameters, other template");

            nomatch = @"{{cite news | url=http://www.site.com | title = Testing | acccessdate = 20 May 2009 }}";
            Assert.That(Parsers.RenameTemplateParameters(nomatch, RenamedTemplateParameters), Is.EqualTo(nomatch), "No change when no matched templates");
        }

        [Test]
        public void LoadRenamedTemplateParameters()
        {
            List<WikiRegexes.TemplateParameters> RenamedTemplateParameters = Parsers.LoadRenamedTemplateParameters(@"{{AWB rename template parameter|cite web|acccessdate|accessdate}}");

            Assert.That(RenamedTemplateParameters.Count, Is.EqualTo(1));

            foreach (WikiRegexes.TemplateParameters TP in RenamedTemplateParameters)
            {
                Assert.That(TP.TemplateName, Is.EqualTo("cite web"));
                Assert.That(TP.OldParameter, Is.EqualTo("acccessdate"));
                Assert.That(TP.NewParameter, Is.EqualTo("accessdate"));
            }

            RenamedTemplateParameters = Parsers.LoadRenamedTemplateParameters(@"
{{AWB rename template parameter|cite web|acccessdate|accessdate}}
{{AWB rename template parameter|cite web|acessdate|accessdate}}");

            Assert.That(RenamedTemplateParameters.Count, Is.EqualTo(2));
        }
    }
}
