﻿/*
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
    public class FixCitationTemplatesTests : RequiresParser
    {
        public GenfixesTestsBase genFixes  = new GenfixesTestsBase();

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
            Assert.AreEqual(@"now {{cite web| url=a.com|title=hello|format=PDF}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=“hello″|format=PDF}} was"));
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

            // duplicate parameter with number at end (last1 etc.) not deduped
            Assert.AreEqual(@"now {{cite web| url=http://site.net |title=hello|accessdate = 2008-10-08|last1=hello|last1=hello}}", Parsers.FixCitationTemplates(@"now {{cite web| url=http://site.net |title=hello|accessdate = 2008-10-08|last1=hello|last1=hello}}"));
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

            Assert.AreEqual(c, Parsers.FixCitationTemplates(@"{{cite book|url=a |title=b |year=2008 | date=May 01|author=Lee}}"), "handles field merge and leading zero in date");
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
  }}";

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
}}", nochange5b = @"{{cite book | isbn = 084
 | pages = 3-262, 3-106, 15-20
 | url =
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
        public void UppercaseCiteFields()
        {
            // single uppercase field
            Assert.AreEqual(@"{{cite web|foo=hello|title=hello}}", Parsers.FixCitationTemplates(@"{{cite web|FOO=hello|title=hello}}"));
            Assert.AreEqual(@"{{cite web|url=http://members.bib-arch.org|title=hello}}", Parsers.FixCitationTemplates(@"{{cite web|uRL=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{cite web|url=http://members.bib-arch.org|title=hello}}", Parsers.FixCitationTemplates(@"{{cite web|UrL=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{cite web|url=http://members.bib-arch.org|title=hello}}", Parsers.FixCitationTemplates(@"{{cite web|Url=http://members.bib-arch.org|title=hello}}"));
            Assert.AreEqual(@"{{Cite web|foo=hello|title=hello}}", Parsers.FixCitationTemplates(@"{{Cite web|FOO=hello|title=hello}}"));
            Assert.AreEqual(@"{{cite web | foo=hello|title=hello}}", Parsers.FixCitationTemplates(@"{{cite web | FOO=hello|title=hello}}"));
            Assert.AreEqual(@"{{cite web | foo = hello|title=hello}}", Parsers.FixCitationTemplates(@"{{cite web | FOO = hello|title=hello}}"));

            // multiple uppercase fields
            Assert.AreEqual(@"{{cite web|foo=hello|title=hello}}", Parsers.FixCitationTemplates(@"{{cite web|FOO=hello|Title=hello}}"));
            Assert.AreEqual(@"{{cite web|foo=hello|title=hello}}", Parsers.FixCitationTemplates(@"{{cite web|FOO=hello|TITLE=hello}}"));
            Assert.AreEqual(@"{{cite web|foo=hello|title=hello | work=BBC}}", Parsers.FixCitationTemplates(@"{{cite web|FOO=hello|TITLE=hello | Work=BBC}}"));

            //other templates
            Assert.AreEqual(@"{{cite web|foo=hello|title=hello}}", Parsers.FixCitationTemplates(@"{{cite web|FOO=hello|title=hello}}"));
            Assert.AreEqual(@"{{cite book|foo=hello|title=hello}}", Parsers.FixCitationTemplates(@"{{cite book|FOO=hello|title=hello}}"));
            Assert.AreEqual(@"{{cite news|foo=hello|title=hello}}", Parsers.FixCitationTemplates(@"{{cite news|FOO=hello|title=hello}}"));
            Assert.AreEqual(@"{{cite journal|foo=hello|title=hello}}", Parsers.FixCitationTemplates(@"{{cite journal|FOO=hello|title=hello}}"));
            Assert.AreEqual(@"{{cite paper|foo=hello|title=hello}}", Parsers.FixCitationTemplates(@"{{cite paper|FOO=hello|title=hello}}"));
            Assert.AreEqual(@"{{cite press release|foo=hello|title=hello}}", Parsers.FixCitationTemplates(@"{{cite press release|FOO=hello|title=hello}}"));
            Assert.AreEqual(@"{{cite hansard|foo=hello|title=hello}}", Parsers.FixCitationTemplates(@"{{cite hansard|FOO=hello|title=hello}}"));
            Assert.AreEqual(@"{{cite encyclopedia|foo=hello|title=hello}}", Parsers.FixCitationTemplates(@"{{cite encyclopedia|FOO=hello|title=hello}}"));
            Assert.AreEqual(@"{{citation|foo=hello|title=hello}}", Parsers.FixCitationTemplates(@"{{citation|FOO=hello|title=hello}}"));

            Assert.AreEqual(@"{{Cite web|foo=hello|title=hello}}", Parsers.FixCitationTemplates(@"{{Cite web|FOO=hello|title=hello}}"));
            Assert.AreEqual(@"{{Cite book|foo=hello|title=hello}}", Parsers.FixCitationTemplates(@"{{Cite book|FOO=hello|title=hello}}"));
            Assert.AreEqual(@"{{Cite news|foo=hello|title=hello}}", Parsers.FixCitationTemplates(@"{{Cite news|FOO=hello|title=hello}}"));
            Assert.AreEqual(@"{{Cite journal|foo=hello|title=hello}}", Parsers.FixCitationTemplates(@"{{Cite journal|FOO=hello|title=hello}}"));
            Assert.AreEqual(@"{{Cite paper|foo=hello|title=hello}}", Parsers.FixCitationTemplates(@"{{Cite paper|FOO=hello|title=hello}}"));
            Assert.AreEqual(@"{{Cite press release|foo=hello|title=hello}}", Parsers.FixCitationTemplates(@"{{Cite press release|FOO=hello|title=hello}}"));
            Assert.AreEqual(@"{{Cite hansard|foo=hello|title=hello}}", Parsers.FixCitationTemplates(@"{{Cite hansard|FOO=hello|title=hello}}"));
            Assert.AreEqual(@"{{Cite encyclopedia|foo=hello|title=hello}}", Parsers.FixCitationTemplates(@"{{Cite encyclopedia|FOO=hello|title=hello}}"));
            Assert.AreEqual(@"{{Citation|foo=hello|title=hello}}", Parsers.FixCitationTemplates(@"{{Citation|FOO=hello|title=hello}}"));

            Assert.AreEqual(@"{{cite book | author=Smith | title=Great Book | ISBN=15478454 | date=17 May 2004 }}", Parsers.FixCitationTemplates(@"{{cite book | author=Smith | Title=Great Book | ISBN=15478454 | date=17 May 2004 }}"));

            // ISBN, DOI, PMID, PMC, LCCN, ASIN is allowed to be uppercase
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
            string ASIN = @"{{cite journal| author=Smith | title=Great Book | ASIN=15478454 | date=17 May 2004 }}";
            Assert.AreEqual(ASIN, Parsers.FixCitationTemplates(ASIN));

            // don't match on part of URL
            string URL = @"{{cite news|url=http://www.expressbuzz.com/edition/story.aspx?Title=Catching++them+young&artid=rPwTAv2l2BY=&SectionID=fxm0uEWnVpc=&MainSectionID=ngGbWGz5Z14=&SectionName=RtFD/|pZbbWSsbI0jf3F5Q==&SEO=|title=Catching them young|date=August 7, 2009|work=[[The Indian Express]]|accessdate=2009-08-07}}";
            Assert.AreEqual(URL, Parsers.FixCitationTemplates(URL));
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
            Assert.AreEqual(@"now {{cite web|title=hello |url=http://www.site.net}} was", Parsers.FixCitationTemplates(@"now {{cite web|title=hello |url=[www.site.net]}} was"), "bracket and protocol fix combined");
            Assert.AreEqual(@"now {{cite journal|title=hello | url=http://site.net }} was", Parsers.FixCitationTemplates(@"now {{cite journal|title=hello | url=[http://site.net]] }} was"));

            // no match
            Assert.AreEqual(@"now {{cite web| url=http://site.net|title=hello}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=http://site.net|title=hello}} was"));
            Assert.AreEqual(@"now {{cite web| url=[http://site.net cool site]|title=hello}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=[http://site.net cool site]|title=hello}} was"));
            Assert.AreEqual(@"now {{cite web| url=http://site.net cool site]|title=hello}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=http://site.net cool site]|title=hello}} was"));
            Assert.AreEqual(@"now {{cite web| url=[http://site.net cool site|title=hello}} was", Parsers.FixCitationTemplates(@"now {{cite web| url=[http://site.net cool site|title=hello}} was"));
        }
	}
}