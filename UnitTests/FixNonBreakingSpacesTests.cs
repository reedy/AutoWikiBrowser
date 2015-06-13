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
    public class FixNonBreakingSpacesTests : RequiresParser
    {
        public GenfixesTestsBase genFixes  = new GenfixesTestsBase();

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
        	Assert.AreEqual(@"9:59&nbsp;a.m.", parser.FixNonBreakingSpaces(@"9:59a.m."));
            Assert.AreEqual(@"12:35&nbsp;a.m.", parser.FixNonBreakingSpaces(@"12:35a.m."));
            Assert.AreEqual(@"12:35&nbsp;a.m.", parser.FixNonBreakingSpaces(@"12:35 a.m."));
            Assert.AreEqual(@"12:35&nbsp;p.m.", parser.FixNonBreakingSpaces(@"12:35p.m."));
            Assert.AreEqual(@"12:35&nbsp;p.m.", parser.FixNonBreakingSpaces(@"12:35 p.m."));
            Assert.AreEqual(@"2:35&nbsp;a.m.", parser.FixNonBreakingSpaces(@"02:35a.m."), "starts with zero");
        	Assert.AreEqual(@"9:59&nbsp;a.m.", parser.FixNonBreakingSpaces(@"09:59a.m."));
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
            Assert.AreEqual(@"at 55&nbsp;°F today", parser.FixNonBreakingSpaces(@"at 55 °F today"), "invisible nbsp (Unicode U+00A0) before unit");

			Assert.AreEqual(@"a 50.2&nbsp;m (170&nbsp;ft) road", parser.FixNonBreakingSpaces(@"a 50.2 m (170 ft) road"), "invisible nbsp (Unicode U+00A0) before m and ft");

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
}