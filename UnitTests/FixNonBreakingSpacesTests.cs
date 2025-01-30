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

using NUnit.Framework;
using WikiFunctions;
using WikiFunctions.Parse;

namespace UnitTests
{
    [TestFixture]
    public class FixNonBreakingSpacesTests : RequiresParser
    {
        public GenfixesTestsBase genFixes  = new GenfixesTestsBase();

        [Test]
        public void TestFixPercent()
        {
            Assert.That(parser.FixNonBreakingSpaces(@"a 15 % "), Is.EqualTo(@"a 15% "), "remove space");
            Assert.That(parser.FixNonBreakingSpaces(@"a -15 % "), Is.EqualTo(@"a -15% "), "remove space (minus sign)");
            Assert.That(parser.FixNonBreakingSpaces(@"a +15 % "), Is.EqualTo(@"a +15% "), "remove space (plus sign)");
            Assert.That(parser.FixNonBreakingSpaces(@"a ±15 % "), Is.EqualTo(@"a ±15% "), "remove space (plus-minus sign)");
            Assert.That(parser.FixNonBreakingSpaces(@"a 15&nbsp;% "), Is.EqualTo(@"a 15% "), "remove non breaking space");
            Assert.That(parser.FixNonBreakingSpaces(@"a 15 &nbsp;%."), Is.EqualTo(@"a 15%."), "remove space and nbsp and maintain point");
            Assert.That(parser.FixNonBreakingSpaces(@"a 15 % "), Is.EqualTo(@"a 15% "), "remove space");
            Assert.That(parser.FixNonBreakingSpaces(@"a 15&nbsp;%."), Is.EqualTo(@"a 15%."), "remove non breaking space and maintain point");
            Assert.That(parser.FixNonBreakingSpaces(@"a 15 %,"), Is.EqualTo(@"a 15%,"), "remove space and maintain comma");
            Assert.That(parser.FixNonBreakingSpaces(@"a 15&nbsp;%,"), Is.EqualTo(@"a 15%,"), "remove non breaking space  and maintain comma");
            Assert.That(parser.FixNonBreakingSpaces(@"a 15 %!"), Is.EqualTo(@"a 15%!"), "remove space and maintain mark");
            Assert.That(parser.FixNonBreakingSpaces(@"a 15&nbsp;%!"), Is.EqualTo(@"a 15%!"), "remove non breaking space  and maintain mark");
            Assert.That(parser.FixNonBreakingSpaces(@"a 15  %"), Is.EqualTo(@"a 15  %"), "no changes");
            Assert.That(parser.FixNonBreakingSpaces(@"5a21 %"), Is.EqualTo(@"5a21 %"), "no changes");
            Assert.That(parser.FixNonBreakingSpaces(@"a15 %"), Is.EqualTo(@"a15 %"), "no changes");
            Assert.That(parser.FixNonBreakingSpaces(@"a 15 %a"), Is.EqualTo(@"a 15 %a"), "no changes (character)");
            Assert.That(parser.FixNonBreakingSpaces(@"a 15 %2"), Is.EqualTo(@"a 15 %2"), "no changes (character)");
            Assert.That(parser.FixNonBreakingSpaces(@"a 15.2 % "), Is.EqualTo(@"a 15.2% "), "catch decimal numbers");
            Assert.That(parser.FixNonBreakingSpaces(@"acid (15.2 %) "), Is.EqualTo(@"acid (15.2%) "), "decimal numbers in parenthenses");
            Assert.That(parser.FixNonBreakingSpaces(@"a 15.a2 % "), Is.EqualTo(@"a 15.a2 % "), "avoid weird things");

#if DEBUG
            Variables.SetProjectLangCode("sv");
            Assert.That(parser.FixNonBreakingSpaces(@"a 15 % "), Is.EqualTo(@"a 15 % "), "Don't remove space in svwiki per sv:Procent");

            Variables.SetProjectLangCode("simple");
            Assert.That(parser.FixNonBreakingSpaces(@"a 15 % "), Is.EqualTo(@"a 15% "), "Remove space in simple");

            Variables.SetProjectLangCode("en");
            Assert.That(parser.FixNonBreakingSpaces(@"a 15 % "), Is.EqualTo(@"a 15% "), "remove space");
#endif
        }

        [Test]
        public void TestFixCurrency()
        {
            Assert.That(parser.FixNonBreakingSpaces(@"£ 123 "), Is.EqualTo(@"£123 "), "remove space (Pound sign)");
            Assert.That(parser.FixNonBreakingSpaces(@"€ 123 "), Is.EqualTo(@"€123 "), "remove space (Euro sign)");
            Assert.That(parser.FixNonBreakingSpaces(@"$ 123 "), Is.EqualTo(@"$123 "), "remove space (Dollar sign)");
            Assert.That(parser.FixNonBreakingSpaces(@"£&nbsp;123 "), Is.EqualTo(@"£123 "), "remove non breaking space");
            Assert.That(parser.FixNonBreakingSpaces(@"£ &nbsp;123."), Is.EqualTo(@"£123."), "remove space and nbsp and maintain point");
            Assert.That(parser.FixNonBreakingSpaces(@"£ 12.3,"), Is.EqualTo(@"£12.3,"), "remove space and maintain comma");

#if DEBUG
            Variables.SetProjectLangCode("sv");
            Assert.That(parser.FixNonBreakingSpaces(@"£ 123 "), Is.EqualTo(@"£ 123 "), "Don't remove space in svwiki");

            Variables.SetProjectLangCode("simple");
            Assert.That(parser.FixNonBreakingSpaces(@"£ 123 "), Is.EqualTo(@"£123 "), "Remove space in simple");

            Variables.SetProjectLangCode("en");
            Assert.That(parser.FixNonBreakingSpaces(@"£ 123 "), Is.EqualTo(@"£123 "), "remove space");
#endif
        }

        [Test]
        public void TestFixClockTime()
        {
            Assert.That(parser.FixNonBreakingSpaces(@"2:35a.m."), Is.EqualTo(@"2:35&nbsp;a.m."));
            Assert.That(parser.FixNonBreakingSpaces(@"9:59a.m."), Is.EqualTo(@"9:59&nbsp;a.m."));
            Assert.That(parser.FixNonBreakingSpaces(@"12:35a.m."), Is.EqualTo(@"12:35&nbsp;a.m."));
            Assert.That(parser.FixNonBreakingSpaces(@"12:35 a.m."), Is.EqualTo(@"12:35&nbsp;a.m."));
            Assert.That(parser.FixNonBreakingSpaces(@"12:35p.m."), Is.EqualTo(@"12:35&nbsp;p.m."));
            Assert.That(parser.FixNonBreakingSpaces(@"12:35 p.m."), Is.EqualTo(@"12:35&nbsp;p.m."));
            Assert.That(parser.FixNonBreakingSpaces(@"02:35a.m."), Is.EqualTo(@"2:35&nbsp;a.m."), "starts with zero");
            Assert.That(parser.FixNonBreakingSpaces(@"09:59a.m."), Is.EqualTo(@"9:59&nbsp;a.m."));
            Assert.That(parser.FixNonBreakingSpaces(@"2:35p.m."), Is.EqualTo(@"2:35&nbsp;p.m."));
            Assert.That(parser.FixNonBreakingSpaces(@"12:35p.m."), Is.EqualTo(@"12:35&nbsp;p.m."));
            Assert.That(parser.FixNonBreakingSpaces(@"02:35p.m."), Is.EqualTo(@"2:35&nbsp;p.m."), "starts with zero");
            Assert.That(parser.FixNonBreakingSpaces(@"2:75a.m."), Is.EqualTo(@"2:75a.m."), "invalid minutes number");
            Assert.That(parser.FixNonBreakingSpaces(@"36:15a.m."), Is.EqualTo(@"36:15a.m."), "invalid hours number");
            Assert.That(parser.FixNonBreakingSpaces(@"16:15c.m."), Is.EqualTo(@"16:15c.m."), "invalid suffix");

            Assert.That(parser.FixNonBreakingSpaces(@"(8:00:57 p.m. EST)"), Is.EqualTo(@"(8:00:57&nbsp;p.m. EST)"));
        }

        [Test]
        public void TestFixNonBreakingSpaces()
        {
            Assert.That(parser.FixNonBreakingSpaces(@"a 50 km road"), Is.EqualTo(@"a 50&nbsp;km road"));
            Assert.That(parser.FixNonBreakingSpaces(@"a 50 m (170 ft) road"), Is.EqualTo(@"a 50&nbsp;m (170&nbsp;ft) road"));
            Assert.That(parser.FixNonBreakingSpaces(@"a 50.2 m (170 ft) road"), Is.EqualTo(@"a 50.2&nbsp;m (170&nbsp;ft) road"));
            Assert.That(parser.FixNonBreakingSpaces(@"a 5002 m (17,000 ft) road"), Is.EqualTo(@"a 5002&nbsp;m (17,000&nbsp;ft) road"));
            Assert.That(parser.FixNonBreakingSpaces(@"a long (50 km) road"), Is.EqualTo(@"a long (50&nbsp;km) road"));
            Assert.That(parser.FixNonBreakingSpaces(@"a 50km road"), Is.EqualTo(@"a 50&nbsp;km road"));
            Assert.That(parser.FixNonBreakingSpaces(@"a 50 kg dog"), Is.EqualTo(@"a 50&nbsp;kg dog"));
            Assert.That(parser.FixNonBreakingSpaces(@"a 50kg dog"), Is.EqualTo(@"a 50&nbsp;kg dog"));

            Assert.That(parser.FixNonBreakingSpaces(@"a 50Hz rod"), Is.EqualTo(@"a 50&nbsp;Hz rod"));
            Assert.That(parser.FixNonBreakingSpaces(@"a 50kHz rod"), Is.EqualTo(@"a 50&nbsp;kHz rod"));

            Assert.That(parser.FixNonBreakingSpaces(@"a 50 cm road"), Is.EqualTo(@"a 50&nbsp;cm road"));
            Assert.That(parser.FixNonBreakingSpaces(@"a 50cm road"), Is.EqualTo(@"a 50&nbsp;cm road"));
            Assert.That(parser.FixNonBreakingSpaces(@"a 50.247cm road"), Is.EqualTo(@"a 50.247&nbsp;cm road"));
            Assert.That(parser.FixNonBreakingSpaces(@"a 50.247nm laser"), Is.EqualTo(@"a 50.247&nbsp;nm laser"));
            Assert.That(parser.FixNonBreakingSpaces(@"a 50.247 mm pen"), Is.EqualTo(@"a 50.247&nbsp;mm pen"));
            Assert.That(parser.FixNonBreakingSpaces(@"a 50.247  nm laser"), Is.EqualTo(@"a 50.247&nbsp;nm laser"));
            Assert.That(parser.FixNonBreakingSpaces(@"a 50.247µm laser"), Is.EqualTo(@"a 50.247&nbsp;µm laser"));
            Assert.That(parser.FixNonBreakingSpaces(@"a 50.247 cd light"), Is.EqualTo(@"a 50.247&nbsp;cd light"));
            Assert.That(parser.FixNonBreakingSpaces(@"a 50.247cd light"), Is.EqualTo(@"a 50.247&nbsp;cd light"));
            Assert.That(parser.FixNonBreakingSpaces(@"a 50.247mmol solution"), Is.EqualTo(@"a 50.247&nbsp;mmol solution"));
            Assert.That(parser.FixNonBreakingSpaces(@"a 0.3mol solution"), Is.EqualTo(@"a 0.3&nbsp;mol solution"));
            Assert.That(parser.FixNonBreakingSpaces(@"a 50.247 kW laser"), Is.EqualTo(@"a 50.247&nbsp;kW laser"));
            Assert.That(parser.FixNonBreakingSpaces(@"a 50.247 mW laser"), Is.EqualTo(@"a 50.247&nbsp;mW laser"));
            Assert.That(parser.FixNonBreakingSpaces(@"a 50m/s car"), Is.EqualTo(@"a 50&nbsp;m/s car"));
            Assert.That(parser.FixNonBreakingSpaces(@"at 5°C today"), Is.EqualTo(@"at 5&nbsp;°C today"));
            Assert.That(parser.FixNonBreakingSpaces(@"at 5 °C today"), Is.EqualTo(@"at 5&nbsp;°C today"));
            Assert.That(parser.FixNonBreakingSpaces(@"at 55°F today"), Is.EqualTo(@"at 55&nbsp;°F today"));
            Assert.That(parser.FixNonBreakingSpaces(@"at 55  °F today"), Is.EqualTo(@"at 55&nbsp;°F today"));
            Assert.That(parser.FixNonBreakingSpaces(@"at 55 °F today"), Is.EqualTo(@"at 55&nbsp;°F today"), "invisible nbsp (Unicode U+00A0) before unit");
            Assert.That(parser.FixNonBreakingSpaces(@"a 50.2 m (170 ft) road"), Is.EqualTo(@"a 50.2&nbsp;m (170&nbsp;ft) road"), "invisible nbsp (Unicode U+00A0) before m and ft");

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
            Assert.That(parser.FixNonBreakingSpaces(@"the 50mm gun"), Is.EqualTo(@"the 50mm gun"));

            // Imperial units
            Assert.That(parser.FixNonBreakingSpaces(@"a long (50 in) toad"), Is.EqualTo(@"a long (50&nbsp;in) toad"));
            Assert.That(parser.FixNonBreakingSpaces(@"a long (50 ft) toad"), Is.EqualTo(@"a long (50&nbsp;ft) toad"));

            Assert.That(parser.FixNonBreakingSpaces(@"a long (50in) toad"), Is.EqualTo(@"a long (50&nbsp;in) toad"));
            Assert.That(parser.FixNonBreakingSpaces(@"a long (50-52in) toad"), Is.EqualTo(@"a long (50-52&nbsp;in) toad"));
            Assert.That(parser.FixNonBreakingSpaces(@"a long (50–52in) toad"), Is.EqualTo(@"a long (50–52&nbsp;in) toad"));
            Assert.That(parser.FixNonBreakingSpaces(@"a long (50.5 in) toad"), Is.EqualTo(@"a long (50.5&nbsp;in) toad"));
            Assert.That(parser.FixNonBreakingSpaces(@"a big (50.5 oz) toad"), Is.EqualTo(@"a big (50.5&nbsp;oz) toad"));
        }

        [Test]
        public void TestFixNonBreakingSpacesDE()
        {
#if DEBUG
            Variables.SetProjectLangCode("fr");
            parser = new Parsers();
            Assert.That(parser.FixNonBreakingSpaces(@"a 50.247µm laser"), Is.EqualTo(@"a 50.247&nbsp;µm laser"));
            Assert.That(parser.FixNonBreakingSpaces(@"a 50.247um laser"), Is.EqualTo(@"a 50.247um laser"));

            Variables.SetProjectLangCode("en");
            parser = new Parsers();
            Assert.That(parser.FixNonBreakingSpaces(@"a 50.247um laser"), Is.EqualTo(@"a 50.247&nbsp;um laser"));
#endif
        }

        [Test]
        public void FixNonBreakingSpacesPagination()
        {
            Assert.That(parser.FixNonBreakingSpaces(@"Smith 2004, p. 40"), Is.EqualTo(@"Smith 2004, p.&nbsp;40"));
            Assert.That(parser.FixNonBreakingSpaces(@"Smith 2004, p.  40"), Is.EqualTo(@"Smith 2004, p.&nbsp;40"));
            Assert.That(parser.FixNonBreakingSpaces(@"Smith 2004, p.40"), Is.EqualTo(@"Smith 2004, p.&nbsp;40"));
            Assert.That(parser.FixNonBreakingSpaces(@"Smith 2004, p. XI"), Is.EqualTo(@"Smith 2004, p.&nbsp;XI"));
            Assert.That(parser.FixNonBreakingSpaces(@"Smith 2004, pp. 40-44"), Is.EqualTo(@"Smith 2004, pp.&nbsp;40-44"));
            Assert.That(parser.FixNonBreakingSpaces(@"Smith 2004, Pp. 40-44"), Is.EqualTo(@"Smith 2004, Pp.&nbsp;40-44"));
            Assert.That(parser.FixNonBreakingSpaces(@"Smith 2004, p.&nbsp;40"), Is.EqualTo(@"Smith 2004, p.&nbsp;40"));

            Assert.That(parser.FixNonBreakingSpaces(@"Smith 200 pp. ISBN 12345678X"), Is.EqualTo(@"Smith 200 pp. ISBN 12345678X"), "No change for number of pages in book");
        }
    }
}