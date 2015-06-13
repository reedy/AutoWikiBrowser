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
    public class CiteTemplateDatesTests : RequiresParser
    {
        public GenfixesTestsBase genFixes  = new GenfixesTestsBase();

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
	}
}
