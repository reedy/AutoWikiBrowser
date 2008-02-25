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

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using WikiFunctions;
using WikiFunctions.Parse;
using NUnit.Framework.SyntaxHelpers;

namespace UnitTests
{
    [TestFixture]
    public class FootnotesTests
    {
        Parsers parser = new Parsers();

        [Test]
        public void PrecededByEqualSign()
        {
            Assert.That(parser.FixFootnotes("a=<ref>b</ref>"), Text.DoesNotContain("\n"));
        }
    }

    [TestFixture]
    public class LinkTests
    {
        [SetUp]
        public void SetUp()
        {
            Globals.UnitTestMode = true;
        }

        Parsers parser = new Parsers();

        [Test]
        public void DontFixTrailedLinks()
        {
            bool dummy;

            Assert.AreEqual(parser.FixLinks("[[a ]]b", out dummy), "[[a ]]b");
        }
    }

    [TestFixture]
    public class ImageTests
    {
        [SetUp]
        public void SetUp()
        {
            Globals.UnitTestMode = true;
        }

        [Test]
        public void BasicImprovements()
        {
            Parsers p = new Parsers();

            Assert.AreEqual("[[Image:foo.jpg|thumb|200px|Bar]]",
                p.FixImages("[[ image : foo.jpg|thumb|200px|Bar]]"));

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_6#URL_underscore_regression
            Assert.AreEqual("[[Image:Apple I.jpg|thumb]] # [http://www.atariarchives.org/deli/homebrew_and_how_the_apple.php Apple Came] [[link]]",
                p.FixImages("[[Image:Apple I.jpg|thumb]] # [http://www.atariarchives.org/deli/homebrew_and_how_the_apple.php Apple Came] [[link]]"));
        }
    }
}
