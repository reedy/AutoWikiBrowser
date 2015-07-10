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
            
            Assert.AreEqual(@"==External links==", Parsers.FixHeadings(@"==External Links==", "a"));
            Assert.AreEqual(@"==External links==", Parsers.FixHeadings(@"==external links==", "a"));

            Assert.AreEqual(@"==Further reading==
            bar bar
            
==External links==
*http://foo.com", Parsers.FixHeadings(@"==Further reading==
            bar bar
            
==External Links==
*http://foo.com", "a"));


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
'''Foo''' great.", "Foo").Contains(@"==Introduction=="), "Excess heading at start");
            Assert.IsFalse(Parsers.FixHeadings(@"==Introduction:==
'''Foo''' great.", "Foo").Contains(@"==Introduction:=="), "Excess heading at start, with colon");
            Assert.IsFalse(Parsers.FixHeadings(@"=='''Introduction'''==
'''Foo''' great.", "Foo").Contains(@"Introduction"), "Excess heading at start, with bold");
            Assert.IsFalse(Parsers.FixHeadings(@"=='''Introduction:'''==
'''Foo''' great.", "Foo").Contains(@"=='''Introduction:'''=="), "Excess heading at start, with bold and colon");
            Assert.IsFalse(Parsers.FixHeadings(@"===Introduction===
'''Foo''' great.", "Foo").Contains(@"===Introduction==="), "Excess heading at start, level 3");

            Assert.IsFalse(Parsers.FixHeadings(@"==About==
'''Foo''' great.", "Foo").Contains(@"==About=="), "Excess heading at start About");
            Assert.IsFalse(Parsers.FixHeadings(@"==Description==
'''Foo''' great.", "Foo").Contains(@"==Description=="), "Excess heading at start Description");
            Assert.IsFalse(Parsers.FixHeadings(@"==Overview==
'''Foo''' great.", "Foo").Contains(@"==Overview=="), "Excess heading at start Overview");
            Assert.IsFalse(Parsers.FixHeadings(@"==Definition==
'''Foo''' great.", "Foo").Contains(@"==Definition=="), "Excess heading at start Definition");
            Assert.IsFalse(Parsers.FixHeadings(@"==Profile==
'''Foo''' great.", "Foo").Contains(@"==Profile=="), "Excess heading at start Profile");
            Assert.IsFalse(Parsers.FixHeadings(@"==General information==
'''Foo''' great.", "Foo").Contains(@"==General information=="), "Excess heading at start General information");

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
            Assert.AreEqual("55–57 kg", parser.Mdashes(@"55&#8212;57 kg", "test"));
            Assert.AreEqual("55–57 kg", parser.Mdashes(@"55&mdash;57 kg", "test"));
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
            Assert.AreEqual("$55–57", parser.Mdashes("$55&#8212;57", "test"));
            Assert.AreEqual("$55–57", parser.Mdashes("$55—57", "test"));

            Assert.AreEqual("5:17 AM – 5:19 AM", parser.Mdashes("5:17 AM - 5:19 AM", "test"));
            Assert.AreEqual("5:17 am – 5:19 am", parser.Mdashes("5:17 am - 5:19 am", "test"));
            Assert.AreEqual("05:17 AM – 05:19 AM", parser.Mdashes("05:17 AM - 05:19 AM", "test"));
            Assert.AreEqual("11:17 PM – 11:19 PM", parser.Mdashes("11:17 PM - 11:19 PM", "test"));
            Assert.AreEqual("11:17 pm – 11:19 pm", parser.Mdashes("11:17 pm - 11:19 pm", "test"));
            Assert.AreEqual("11:17 pm – 11:19 pm", parser.Mdashes("11:17 pm &mdash; 11:19 pm", "test"));
            Assert.AreEqual("11:17 pm – 11:19 pm", parser.Mdashes("11:17 pm &#8212; 11:19 pm", "test"));
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
        public void FixOrdinalTests()
        {
            Assert.AreEqual("1st day at school", Parsers.FixSyntax(@"1<sup>st</sup> day at school"),"1st");
            Assert.AreEqual("2nd day at school", Parsers.FixSyntax(@"2<sup>nd</sup> day at school"),"2nd");
            Assert.AreEqual("3rd day at school", Parsers.FixSyntax(@"3<sup>rd</sup> day at school"),"3rd");
            Assert.AreEqual("4th day at school", Parsers.FixSyntax(@"4<sup>th</sup> day at school"),"4th");
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

            Assert.AreEqual(@"now {{citation needed|
{{citation needed}} 
}}", Parsers.TemplateRedirects(@"now {{fact|
{{cn}} 
}}", TemplateRedirects), "renames nested templates, both levels");

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

            const string correct2a = @"<ref>{{cite web | url=http://www.site.com | title = Testing |accessdate=20 May 2009 }}</ref><ref>{{cite web | url=http://www.site2.com | title = Testing2 |accessdate=2 May 2009 }}</ref>";
            Assert.AreEqual(correct2a, Parsers.RenameTemplateParameters(correct2a.Replace("accessdate", "acccessdate"), RenamedTemplateParameters), "renames parameter in simple template call, unspaced");

            const string correct2b = @"<ref>{{cite web | url=http://www.site.com | title = Testing
|accessdate=20 May 2009 }}</ref><ref>{{cite web | url=http://www.site2.com | title = Testing2 |accessdate=2 May 2009 }}</ref>";
            Assert.AreEqual(correct2b, Parsers.RenameTemplateParameters(correct2b.Replace("accessdate", "acccessdate"), RenamedTemplateParameters), "renames parameter in simple template call, newline");

            const string correct3 = @"{{Cite web | url=http://www.site.com | title = Testing | accessdate = 20 May 2009 }}";
            Assert.AreEqual(correct3, Parsers.RenameTemplateParameters(correct3.Replace("accessdate", "acccessdate"), RenamedTemplateParameters), "renames parameter in simple template call, handles first letter casing of template name");

            const string correct4 = @"{{ cite_web | url=http://www.site.com | title = Testing | accessdate = 20 May 2009 }}";
            Assert.AreEqual(correct4, Parsers.RenameTemplateParameters(correct4.Replace("accessdate", "acccessdate"), RenamedTemplateParameters), "renames parameter in simple template call, handles underscore in template name");

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
}
