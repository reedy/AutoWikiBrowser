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
    public class FixCitationTemplatesTests : RequiresParser
    {
        public GenfixesTestsBase genFixes  = new GenfixesTestsBase();

        [Test]
        public void FixCitationTemplatesNewlineInParamValue()
        {
            Assert.That(Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=hello
world|format=PDF}} was"), Is.EqualTo(@"now {{cite web| url=a.com|title=hello world|format=PDF}} was"), "newline converted to space");
            Assert.That(Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=hello world|publisher=Foo
Bar}} was"), Is.EqualTo(@"now {{cite web| url=a.com|title=hello world|publisher=Foo Bar}} was"), "newline converted to space, any parameter");
        }

        [Test]
        public void UnspacedCommaPageRange()
        {
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|url=http://www.stie.com | pages=55,59 }}"), Is.EqualTo(@"{{cite book|url=http://www.stie.com | pages=55, 59 }}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|url=http://www.stie.com | pages=483,491–492 }}"), Is.EqualTo(@"{{cite book|url=http://www.stie.com | pages=483, 491–492 }}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|url=http://www.stie.com | pages=267–268,273,299 }}"), Is.EqualTo(@"{{cite book|url=http://www.stie.com | pages=267–268, 273, 299 }}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|url=http://www.stie.com | pages=55, 59 }}"), Is.EqualTo(@"{{cite book|url=http://www.stie.com | pages=55, 59 }}"), "no change when already correct");
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|url=http://www.stie.com | pages=12,354–12,386 }}"), Is.EqualTo(@"{{cite book|url=http://www.stie.com | pages=12,354–12,386 }}"), "no change when already correct");
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|url=http://www.stie.com | pages=12,354 }}"), Is.EqualTo(@"{{cite book|url=http://www.stie.com | pages=12,354 }}"), "no change when already correct");

            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|url=http://www.stie.com | pages=483,491,492 }}"), Is.EqualTo(@"{{cite book|url=http://www.stie.com | pages=483, 491, 492 }}"));
        }

        [Test]
        public void FixCitationTemplatesQuotedTitle()
        {
            Assert.That(Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=""hello""|format=PDF}} was"), Is.EqualTo(@"now {{cite web| url=a.com|title=""hello""|format=PDF}} was"), "no change: title is quote");
            Assert.That(Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=""hello|format=PDF}} was"), Is.EqualTo(@"now {{cite web| url=a.com|title=hello|format=PDF}} was"), "stray starting quote");
            Assert.That(Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=hello""|format=PDF}} was"), Is.EqualTo(@"now {{cite web| url=a.com|title=hello|format=PDF}} was"), "stray end quote");
            Assert.That(Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|trans_title=""hello|format=PDF}} was"), Is.EqualTo(@"now {{cite web| url=a.com|trans_title=hello|format=PDF}} was"), "trans title stray starting quote");

            Assert.That(Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=""hello"" world|format=PDF}} was"), Is.EqualTo(@"now {{cite web| url=a.com|title=""hello"" world|format=PDF}} was"), "no change: partial quote");

            // curly quote cleanup to straight quotes [[MOS:PUNCT]]
            Assert.That(Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=“hello“|format=PDF}} was"), Is.EqualTo(@"now {{cite web| url=a.com|title=""hello""|format=PDF}} was"), "curly quotes");
            Assert.That(Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=and “hello“ there|format=PDF}} was"), Is.EqualTo(@"now {{cite web| url=a.com|title=and ""hello"" there|format=PDF}} was"), "curly quotes partial quote");
            Assert.That(Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=“hello""|format=PDF}} was"), Is.EqualTo(@"now {{cite web| url=a.com|title=""hello""|format=PDF}} was"), "mixed curly and straight");
            Assert.That(Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=«hello»|format=PDF}} was"), Is.EqualTo(@"now {{cite web| url=a.com|title=""hello""|format=PDF}} was"), "balanced arrows");
            Assert.That(Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=“hello″|format=PDF}} was"), Is.EqualTo(@"now {{cite web| url=a.com|title=""hello""|format=PDF}} was"), "mixed curly");
            Assert.That(Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=now «hello» at|format=PDF}} was"), Is.EqualTo(@"now {{cite web| url=a.com|title=now ""hello"" at|format=PDF}} was"), "partial quote arrows");
            Assert.That(Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=now ‹hello› at|format=PDF}} was"), Is.EqualTo(@"now {{cite web| url=a.com|title=now ""hello"" at|format=PDF}} was"), "partial quote short arrows");
            Assert.That(Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=now ‹hello› at ‹hello2› be|format=PDF}} was"), Is.EqualTo(@"now {{cite web| url=a.com|title=now ""hello"" at ""hello2"" be|format=PDF}} was"), "partial quote short arrows");
            Assert.That(Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=hello» second|format=PDF}} was"), Is.EqualTo(@"now {{cite web| url=a.com|title=hello» second|format=PDF}} was"), @"no change if » used as section delimeter");
            Assert.That(Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=hello> second|format=PDF}} was"), Is.EqualTo(@"now {{cite web| url=a.com|title=hello> second|format=PDF}} was"), @"no change if > used as section delimeter");
            Assert.That(Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=hello « second|format=PDF}} was"), Is.EqualTo(@"now {{cite web| url=a.com|title=hello « second|format=PDF}} was"), @"no change if « used as section delimeter");
            Assert.That(Parsers.FixCitationTemplates(@"now {{cite web| url=a.com|title=hello› second|format=PDF}} was"), Is.EqualTo(@"now {{cite web| url=a.com|title=hello› second|format=PDF}} was"), @"no change if › used as section delimeter");
        }

        [Test]
        public void FixCitationGoogleBooks()
        {
            string correct = @"now {{cite book|title=a |url=http://books.google.com/foo | year=2009}}";
            Assert.That(Parsers.FixCitationTemplates(@"now {{cite web|title=a |url=http://books.google.com/foo | year=2009}}"), Is.EqualTo(correct));
            Assert.That(Parsers.FixCitationTemplates(@"now {{cite web|title=a |url=http://books.google.com/foo | year=2009}}"), Is.EqualTo(correct));
            Assert.That(Parsers.FixCitationTemplates(@"now {{cite web|title=a |url=http://books.google.com/foo | year=2009}}"), Is.EqualTo(correct));

            // whitespace not changed
            Assert.That(Parsers.FixCitationTemplates(@"now {{ Cite web |title=a |url=http://books.google.com/foo | year=2009}}"), Is.EqualTo(@"now {{ Cite book |title=a |url=http://books.google.com/foo | year=2009}}"));

            Assert.That(Parsers.FixCitationTemplates(correct), Is.EqualTo(correct));

            string noChange = @"now {{Cite book|title=a |url=http://books.google.com/foo | year=2009 | work = some Journal}}";
            Assert.That(Parsers.FixCitationTemplates(noChange), Is.EqualTo(noChange), "journal cites to Google books not changed");
        }

        [Test]
        public void FixCitationURLNoHTTP()
        {
            string correct = @"now {{cite web|title=foo | url=http://www.foo.com | date = 1 June 2010 }}";

            Assert.That(Parsers.FixCitationTemplates(correct.Replace("http://", "")), Is.EqualTo(correct), "Adds http:// when URL begins www.");
            Assert.That(Parsers.FixCitationTemplates(correct.Replace("http://www", "Www")), Is.EqualTo(correct.Replace("www", "Www")), "Adds http:// when URL begins www.");
            Assert.That(Parsers.FixCitationTemplates(correct.Replace("http://www", "www2")), Is.EqualTo(correct.Replace("www", "www2")), "Adds http:// when URL begins www2");
            Assert.That(Parsers.FixCitationTemplates(correct), Is.EqualTo(correct), "no change if already correct URL");
            Assert.That(Parsers.FixCitationTemplates(correct.Replace("url=http://", "archiveurl=")), Is.EqualTo(correct.Replace("url=", "archiveurl=")), "Adds http:// when archiveurl begins www.");
            Assert.That(Parsers.FixCitationTemplates(correct.Replace("url=http://", "archive-url=")), Is.EqualTo(correct.Replace("url=", "archive-url=")), "Adds http:// when archive-url begins www.");
            Assert.That(Parsers.FixCitationTemplates(correct.Replace("url=http://", "contribution-url=")), Is.EqualTo(correct.Replace("url=", "contribution-url=")), "Adds http:// when contribution-url begins www.");
            string dash = @"now {{cite web|title=foo | url=www-foo.a.com | date = 1 June 2010 }}";
            Assert.That(Parsers.FixCitationTemplates(dash), Is.EqualTo(dash.Replace("www", "http://www")), "handles www-");

            Assert.That(Parsers.FixCitationTemplates(correct.Replace("http://www", "http:www")), Is.EqualTo(correct), "Adds missing http slashes");
        }

        [Test]
        public void WorkInItalics()
        {
            string correct = @"now {{cite web| url=http://site.net/1.pdf|format=PDF|work=Foo}}";

            Assert.That(Parsers.FixCitationTemplates(@"now {{cite web| url=http://site.net/1.pdf|format=PDF|work=''Foo''}}"), Is.EqualTo(correct));
            Assert.That(Parsers.FixCitationTemplates(correct), Is.EqualTo(correct));

            const string website = @"now {{cite web| url=http://site.net/1.pdf|format=PDF|work=''site.net''}}";
            Assert.That(Parsers.FixCitationTemplates(website), Is.EqualTo(website), "italics not removed for work=website");
        }

        [Test]
        public void FixCitationYear()
        {
            string correct = @"now {{cite book|title=a |url=http://books.google.com/foo | date=2009}}";
            Assert.That(Parsers.FixCitationTemplates(correct), Is.EqualTo(correct));

            string correct2 = @"now {{cite book|title=a |url=http://books.google.com/foo | date=2009 |page=32}}";
            Assert.That(Parsers.FixCitationTemplates(correct2), Is.EqualTo(correct2));

            string nochange1 = @"now {{cite book|title=a |url=http://books.google.com/foo | year=2009a}}",
            nochange2 = @"now {{cite book|title=a |url=http://books.google.com/foo | year=2009a| date = 2009-05-16}}";

            Assert.That(Parsers.FixCitationTemplates(nochange1), Is.EqualTo(nochange1));
            Assert.That(Parsers.FixCitationTemplates(nochange2), Is.EqualTo(nochange2));
        }

        [Test]
        public void FixCitationTemplatesDateInYear()
        {
            string correct1 = @"now {{cite book|title=a |url=http://books.google.com/foo | date=2009-10-17}}",
            correct2 = @"now {{cite book|title=a |url=http://books.google.com/foo | date=2009-10-17|last=Smith}}";

            // ISO
            Assert.That(Parsers.FixCitationTemplates(@"now {{cite book|title=a |url=http://books.google.com/foo | year=2009-10-17}}"), Is.EqualTo(correct1));
            Assert.That(Parsers.FixCitationTemplates(@"now {{cite book|title=a |url=http://books.google.com/foo | year=2009-10-17|last=Smith}}"), Is.EqualTo(correct2));

            // Int
            Assert.That(Parsers.FixCitationTemplates(@"now {{cite book|title=a |url=http://books.google.com/foo | year=2009-10-17}}".Replace("2009-10-17", "17 October 2009")), Is.EqualTo(correct1.Replace("2009-10-17", "17 October 2009")));
            Assert.That(Parsers.FixCitationTemplates(@"now {{cite book|title=a |url=http://books.google.com/foo | year=2009-10-17}}".Replace("2009-10-17", "17 October, 2009")), Is.EqualTo(correct1.Replace("2009-10-17", "17 October 2009")));
            Assert.That(Parsers.FixCitationTemplates(@"now {{cite book|title=a |url=http://books.google.com/foo | year=2009-10-17|last=Smith}}".Replace("2009-10-17", "17 October 2009")), Is.EqualTo(correct2.Replace("2009-10-17", "17 October 2009")));

            // American
            Assert.That(Parsers.FixCitationTemplates(@"now {{cite book|title=a |url=http://books.google.com/foo | year=2009-10-17}}".Replace("2009-10-17", "October 17, 2009")), Is.EqualTo(correct1.Replace("2009-10-17", "October 17, 2009")));
            Assert.That(Parsers.FixCitationTemplates(@"now {{cite book|title=a |url=http://books.google.com/foo | year=2009-10-17|last=Smith}}".Replace("2009-10-17", "October 17, 2009")), Is.EqualTo(correct2.Replace("2009-10-17", "October 17, 2009")));
            Assert.That(Parsers.FixCitationTemplates(@"now {{cite book|title=a |url=http://books.google.com/foo | year=2009-10-17|last=Smith}}".Replace("2009-10-17", "October, 17, 2009")), Is.EqualTo(correct2.Replace("2009-10-17", "October 17, 2009")));

            Assert.That(Parsers.FixCitationTemplates(correct1), Is.EqualTo(correct1));

            string nochange1 = @"now {{cite book|title=a |url=http://books.google.com/foo | year=2009–2010}}";
            Assert.That(Parsers.FixCitationTemplates(nochange1), Is.EqualTo(nochange1));
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

            Assert.That(Parsers.FixCitationTemplates(correct1.Replace(@"}}", @"|year=2009}}")), Is.EqualTo(correct1));
            Assert.That(Parsers.FixCitationTemplates(correct1.Replace(@"foo", @"foo |year=2009")), Is.EqualTo(correct1));

            Assert.That(Parsers.FixCitationTemplates(nochange1), Is.EqualTo(nochange1), "Harvard anchors using YYYYa are not removed");
            Assert.That(Parsers.FixCitationTemplates(nochange1b), Is.EqualTo(nochange1b), "Harvard anchor using YYYYa in year and year in date: both needed so not removed");
            Assert.That(Parsers.FixCitationTemplates(nochange2), Is.EqualTo(nochange2), "Year not removed if different to year in ISO date");
            Assert.That(Parsers.FixCitationTemplates(nochange2b), Is.EqualTo(nochange2b), "Year not removed if different to year in International date");
            Assert.That(Parsers.FixCitationTemplates(nochange2c), Is.EqualTo(nochange2c), "Year not removed if different to year in American date");
            string correct3 = @"now {{cite book|title=a |url=http://books.google.com/foo | date=17 May 2009}}";

            string nochange4 = @"{{cite book|title=a |url=http://books.google.com/foo | date=May 2009 | year=2009 }}";
            Assert.That(Parsers.FixCitationTemplates(nochange4), Is.EqualTo(nochange4), "Year not removed if date is only 'Month YYYY'");

            Assert.That(Parsers.FixCitationTemplates(correct3.Replace(@"}}", @"|year=2009}}")), Is.EqualTo(correct3), "year removed when within date");
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|title=a |url=http://books.google.com/foo | date=2009 | year=2009 }}"), Is.EqualTo(@"{{cite book|title=a |url=http://books.google.com/foo | year=2009 }}"),
                            "Date removed if date is YYYY and year same");
        }
        

        [Test]
        public void FixCitationTemplatesMonthWithinDate()
        {
            string correct = @"now {{cite book|title=a |url=http://books.google.com/foo | date=17 May 2009}}",
            nochange = @"now {{cite book|title=a |url=http://books.google.com/foo | date=17 May 2009 | month=March}}";

            Assert.That(Parsers.FixCitationTemplates(correct.Replace(@"}}", @"|month=May}}")), Is.EqualTo(correct), "month removed when within date");
            Assert.That(Parsers.FixCitationTemplates(nochange), Is.EqualTo(nochange), "month not removed if different to month in date");

            Assert.That(Parsers.FixCitationTemplates(correct.Replace(@"}}", @"|month=5}}")), Is.EqualTo(correct), "number month removed when within date");
            Assert.That(Parsers.FixCitationTemplates(correct.Replace(@"}}", @"|month=05}}")), Is.EqualTo(correct), "number month removed when within date");

            string correct2 = @"now {{cite book|title=a |url=http://books.google.com/foo | date=17 December 2009}}",
            nochange2 = @"now {{cite book|title=a |url=http://books.google.com/foo | date=17 May 2009 | month=2}}";
            Assert.That(Parsers.FixCitationTemplates(correct2.Replace(@"}}", @"|month=12}}")), Is.EqualTo(correct2), "number month removed when within date");
            Assert.That(Parsers.FixCitationTemplates(nochange2), Is.EqualTo(nochange2), "nn month not removed if different to month in date");

            Assert.That(Parsers.FixCitationTemplates(@"{{cite journal|last=Mahoney|first=Noreen|date=2010 April 14|year=2010|month=April|volume=58}}"),
                            Is.EqualTo(@"{{cite journal|last=Mahoney|first=Noreen|date=2010 April 14|volume=58}}"), "year not added when date already has it");

            string correct3 = @"now {{cite book|title=a |url=http://books.google.com/foo | date=2009-12-17}}";
            Assert.That(Parsers.FixCitationTemplates(correct3.Replace(@"}}", @"|month=December}}")), Is.EqualTo(correct3), "number month removed when within date");
        }

        [Test]
        public void FixCitationTemplatesDayMonthInDate()
        {
            string correct1 = @"now {{cite book|title=a |url=http://books.google.com/foo | date=17 October 2009 }}";

            Assert.That(Parsers.FixCitationTemplates(@"now {{cite book|title=a |url=http://books.google.com/foo | date=17 October |year=2009}}"), Is.EqualTo(correct1));
            Assert.That(Parsers.FixCitationTemplates(@"now {{cite book|title=a |url=http://books.google.com/foo | date=October 17 |year=2009}}"), Is.EqualTo(correct1.Replace("17 October", "October 17,")));
            Assert.That(Parsers.FixCitationTemplates(correct1), Is.EqualTo(correct1));
        }

        [Test]
        public void CitationPublisherToWork()
        {
            string correct0 = @"{{cite news|url=http://www.timesonline.co.uk/foo494390.htm | date =2008-09-07 | work=The Times }}",
            correct1 = @"{{cite web|url=http://www.timesonline.co.uk/foo494390.htm | date =2008-09-07 | work=The Times }}",
            correct2 = @"{{citeweb|url=http://www.timesonline.co.uk/foo494390.htm | date =2008-09-07 | work = The Times }}",
            correct3 = @"{{cite web|url=http://www.timesonline.co.uk/foo494390.htm | work= |date =2008-09-07 | work = The Times }}";

            Assert.That(Parsers.CitationPublisherToWork(correct0.Replace("work", "publisher")), Is.EqualTo(correct0));
            Assert.That(Parsers.CitationPublisherToWork(correct1.Replace("work", "publisher")), Is.EqualTo(correct1));
            Assert.That(Parsers.CitationPublisherToWork(correct2.Replace("work", "publisher")), Is.EqualTo(correct2));

            // work present but null
            Assert.That(Parsers.CitationPublisherToWork(correct3.Replace("work =", "publisher =")), Is.EqualTo(correct3));

            string workalready1 = @"{{cite web|url=http://www.timesonline.co.uk/foo494390.htm | publisher=Media International |date =2008-09-07 | work = The Times }}";
            // work already
            Assert.That(Parsers.CitationPublisherToWork(workalready1), Is.EqualTo(workalready1));
            Assert.That(Parsers.CitationPublisherToWork(correct0), Is.EqualTo(correct0));

            // no cite web/news
            const string citeJournal = @"{{cite journal|url=http://www.timesonline.co.uk/foo494390.htm | date =2008-09-07 | publisher=The Times }}";
            Assert.That(Parsers.CitationPublisherToWork(citeJournal), Is.EqualTo(citeJournal));
        }

        [Test]
        public void FixCitationDupeFields()
        {
            string correct = @"{{cite web|url=a |title=b | accessdate=11 May 2008|year=2008}}";
            string correct2 = @"{{cite web|url=a |title=b | accessdate=11 May 2008|year=2008|work=here}}";
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008|year=2008}}"), Is.EqualTo(correct));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year= | accessdate=11 May 2008|year=2008}}"), Is.EqualTo(correct));

            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008|year=2008|work=here}}"), Is.EqualTo(correct2));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |  year=   2008 | accessdate=11 May 2008|year=2008|work=here}}"), Is.EqualTo(correct2));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year= | accessdate=11 May 2008|year=2008|work=here}}"), Is.EqualTo(correct2));

            string correct3 = @"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008|work=here there}}";
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008|work=here there|work=here there}}"), Is.EqualTo(correct3));

            string nochange1 = @"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008|year=2004}}";
            Assert.That(Parsers.FixCitationTemplates(nochange1), Is.EqualTo(nochange1));

            string nochange2 = @"{{cite web|url=http://www.tise.com/abc|year=2008|page.php=7 |title=b |year=2008 | accessdate=11 May 2008}}";
            Assert.That(Parsers.FixCitationTemplates(nochange2), Is.EqualTo(nochange2));

            string nochange3 = @"{{cite book|title=b |year=2008 | accessdate=11 May 2008|year=2004}}";
            Assert.That(Parsers.FixCitationTemplates(nochange3), Is.EqualTo(nochange3));

            // null fields
            Assert.That(Parsers.FixCitationTemplates(@"now {{cite web| url=http://site.net |title=|accessdate = 2008-10-08|title=hello}}"), Is.EqualTo(@"now {{cite web| url=http://site.net |accessdate = 2008-10-08|title=hello}}"));
            Assert.That(Parsers.FixCitationTemplates(@"now {{cite web| url=http://site.net |title=|accessdate = 2008-10-08|title=hello|work=BBC}}"), Is.EqualTo(@"now {{cite web| url=http://site.net |accessdate = 2008-10-08|title=hello|work=BBC}}"));
            Assert.That(Parsers.FixCitationTemplates(@"now {{Cite web| title = hello | url=http://site.net |accessdate = 2008-10-08|title=}}"), Is.EqualTo(@"now {{Cite web| title = hello | url=http://site.net |accessdate = 2008-10-08}}"));
            Assert.That(Parsers.FixCitationTemplates(@"now {{cite web| url=http://site.net |title=|accessdate = 2008-10-08| title = hello }}"), Is.EqualTo(@"now {{cite web| url=http://site.net |accessdate = 2008-10-08| title = hello }}"));
            Assert.That(Parsers.FixCitationTemplates(@"now {{cite web| title=hello|title=|url=http://site.net |date = 2008-10-08}}"), Is.EqualTo(@"now {{cite web| title=hello|url=http://site.net |date = 2008-10-08}}"));
            Assert.That(Parsers.FixCitationTemplates(@"now {{cite web| url=http://site.net |title=hello|first=|accessdate = 2008-10-08|first=}}"), Is.EqualTo(@"now {{cite web| url=http://site.net |title=hello|first=|accessdate = 2008-10-08}}"));

            //no Matches
            Assert.That(Parsers.FixCitationTemplates(@"now {{cite web| title=hello|url=http://site.net |date = 2008-10-08|title=HELLO}}"), Is.EqualTo(@"now {{cite web| title=hello|url=http://site.net |date = 2008-10-08|title=HELLO}}")); // case of field value different
            Assert.That(Parsers.FixCitationTemplates(@"now {{cite web| url=http://site.net |title=hello|accessdate = 2008-10-08|name=hello}}"), Is.EqualTo(@"now {{cite web| url=http://site.net |title=hello|accessdate = 2008-10-08|name=hello}}"));

            // duplicate parameter with number at end (last1 etc.) not deduped
            Assert.That(Parsers.FixCitationTemplates(@"now {{cite web| url=http://site.net |title=hello|accessdate = 2008-10-08|last1=hello|last1=hello}}"), Is.EqualTo(@"now {{cite web| url=http://site.net |title=hello|accessdate = 2008-10-08|last1=hello|last1=hello}}"));
        }

        [Test]
        public void MergeCiteWebAccessDateYear()
        {
            string correct = @"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008}}";
            string correct2 = @"{{cite web|url=a |title=b |year=2008 | accessdate=May 11, 2008}}";
            string correct3 = @"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008 |work=c}}";
            string correct4 = @"{{cite book|url=a |title=b |year=2008 | accessdate=11 May 2008 |work=c}}";

            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=11 May| accessyear=2008}}"), Is.EqualTo(correct));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=11 May| accessyear = 2008  }}"), Is.EqualTo(correct));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=May 11| accessyear=2008}}"), Is.EqualTo(correct2));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=May 11| accessyear = 2008  }}"), Is.EqualTo(correct2));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=11 May | accessyear = 2008 |work=c}}"), Is.EqualTo(correct3));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|url=a |title=b |year=2008 | accessdate=11 May | accessyear = 2008 |work=c}}"), Is.EqualTo(correct4));

            // only for cite web
            string nochange2 = @"{{cite podcast|url=a |title=b |year=2008 | accessdate=11 May |accessyear=2008 |work=c}}";
            Assert.That(Parsers.FixCitationTemplates(nochange2), Is.EqualTo(nochange2));
        }

        [Test]
        public void AccessDayMonthDay()
        {
            string a = @"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008}}";
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008|accessdaymonth=   }}"), Is.EqualTo(a));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008|accessmonthday  =   }}"), Is.EqualTo(a));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008|accessmonth  =   }}"), Is.EqualTo(a));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 |accessmonthday  = | accessdate=11 May 2008}}"), Is.EqualTo(a));

            string notempty = @"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008|accessdaymonth=Foo   }}";
            Assert.That(Parsers.FixCitationTemplates(notempty), Is.EqualTo(notempty));
        }

        [Test]
        public void FixCitationTemplatesAccessYear()
        {
            string a = @"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008}}";
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008|accessyear=2008   }}"), Is.EqualTo(a));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008| accessyear =  2008   }}"), Is.EqualTo(a));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 |accessyear=2008   | accessdate=11 May 2008}}"), Is.EqualTo(a));

            string yearDoesNotMatch = @"{{cite web|url=a |title=b |year=2008 | accessdate=11 May 2008|accessyear=Winter   }}";
            Assert.That(Parsers.FixCitationTemplates(yearDoesNotMatch), Is.EqualTo(yearDoesNotMatch));
        }

        [Test]
        public void FixCitationTemplatesDateLeadingZero()
        {
            string a = @"{{cite web|url=a |title=b |year=2008 | accessdate=1 May 2008}}";
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=01 May 2008}}"), Is.EqualTo(a));

            string a0 = @"{{cite web|url=a |title=b | accessdate=1 May 2008 | date=May 1, 2008}}";
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b | accessdate=01 May 2008 | date=May 01, 2008}}"), Is.EqualTo(a0));

            string a2 = @"{{cite web|url=a |title=b |year=2008 | accessdate=1 May 1998}}";
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=01 May 1998}}"), Is.EqualTo(a2));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|url=a |title=b |year=2008 | accessdate=01 May 1998 }}"), Is.EqualTo(a2.Replace(@"}}", @" }}")));

            string b = @"{{cite book|url=a |title=b | date=May 1, 2008}}";
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|url=a |title=b | date=May 01, 2008}}"), Is.EqualTo(b));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|url=a |title=b | date=May 01 2008}}"), Is.EqualTo(b));

            b = @"{{cite book|url=a |title=b | date=May 1, 2020}}";
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|url=a |title=b | date=May 01, 2020}}"), Is.EqualTo(b));

            string c = @"{{cite book|url=a |title=b | date=May 1, 2008|author=Lee}}";
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|url=a |title=b | date=May 01, 2008|author=Lee}}"), Is.EqualTo(c));

            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|url=a |title=b |year=2008 | date=May 01|author=Lee}}"), Is.EqualTo(c), "handles field merge and leading zero in date");

            const string URL = @"{{cite web|url=http://www.broadwaytovegas.com/October31,1999.html |title=Out of Chapter 11 bankruptcy |publisher=Broadwaytovegas.com |accessdate=2011-02-12}}";

            Assert.That(Parsers.FixCitationTemplates(URL), Is.EqualTo(URL), "No change to date spacing in URL");
        }

        [Test]
        public void FixCitationTemplates()
        {
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009|format=HTML}}"), Is.EqualTo(@"{{cite web|title=foo|url=http://site.net|year=2009}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009|format=HTM}}"), Is.EqualTo(@"{{cite web|title=foo|url=http://site.net|year=2009}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009|format = HTML}}"), Is.EqualTo(@"{{cite web|title=foo|url=http://site.net|year=2009}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009| format  =HTML  }}"), Is.EqualTo(@"{{cite web|title=foo|url=http://site.net|year=2009}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009|     format=HTM}}"), Is.EqualTo(@"{{cite web|title=foo|url=http://site.net|year=2009}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{Citation|title=foo|url=http://site.net|format=HTML|year=2009}}"), Is.EqualTo(@"{{Citation|title=foo|url=http://site.net|year=2009}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009|format=[[HTML]]}}"), Is.EqualTo(@"{{cite web|title=foo|url=http://site.net|year=2009}}"));

            //fix language field
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009|language={{en icon}}}}"), Is.EqualTo(@"{{cite web|title=foo|url=http://site.net|year=2009|language=en}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009|language={{sv icon}}}}"), Is.EqualTo(@"{{cite web|title=foo|url=http://site.net|year=2009|language=sv}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009|language={{de icon}}}}"), Is.EqualTo(@"{{cite web|title=foo|url=http://site.net|year=2009|language=de}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009|language={{el icon}}|publisher=Ser}}"), Is.EqualTo(@"{{cite web|title=foo|url=http://site.net|year=2009|language=el|publisher=Ser}}"));

            // removal of null 'format=' when URL is to HTML
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009|format=}}"), Is.EqualTo(@"{{cite web|title=foo|url=http://site.net|year=2009|format=}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net/a.htm|year=2009|format=}}"), Is.EqualTo(@"{{cite web|title=foo|url=http://site.net/a.htm|year=2009}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net/a.html|year=2009|format=}}"), Is.EqualTo(@"{{cite web|title=foo|url=http://site.net/a.html|year=2009}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net/a.HTML|year=2009|format=}}"), Is.EqualTo(@"{{cite web|title=foo|url=http://site.net/a.HTML|year=2009}}"));

            // removal of null 'origdate='
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009|origdate=}}"), Is.EqualTo(@"{{cite web|title=foo|url=http://site.net|year=2009}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|title=foo|url=http://site.net|year=2009|origdate=2005}}"), Is.EqualTo(@"{{cite web|title=foo|url=http://site.net|year=2009|origdate=2005}}"));

            const string NoChangeSpacedEndashInTitle = @"{{cite web | author=IGN staff | year=2008 | title=IGN Top 100 Games 2008 – 2 Chrono Trigger | url=http://top100.ign.com/2008/ign_top_game_2.html | publisher=IGN | accessdate=March 13, 2009}}";

            Assert.That(Parsers.FixCitationTemplates(NoChangeSpacedEndashInTitle), Is.EqualTo(NoChangeSpacedEndashInTitle));

            const string NoChangeFormatGivesSize = @"{{cite web|title=foo|url=http://site.net/asdfsadf.PDF|year=2009|format=150 MB}}";

            Assert.That(Parsers.FixCitationTemplates(NoChangeFormatGivesSize), Is.EqualTo(NoChangeFormatGivesSize));
        }

        [Test]
        public void FixCitationTemplatesId()
        {
            // id=ASIN... fix
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|title=foo|id=ASIN 123456789X|year=2009}}"), Is.EqualTo(@"{{cite book|title=foo|asin=123456789X|year=2009}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|title=foo|ID=ASIN 123456789X|year=2009}}"), Is.EqualTo(@"{{cite book|title=foo|asin=123456789X|year=2009}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|title=foo|id=ASIN 123456789X |year=2009}}"), Is.EqualTo(@"{{cite book|title=foo|asin=123456789X |year=2009}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|title=foo|id=ASIN: 123-45678-9-X|year=2009}}"), Is.EqualTo(@"{{cite book|title=foo|asin=123-45678-9-X|year=2009}}"));

            //id=ISSN fix
            const string correct = @"{{cite journal|title=foo|journal=foo|issn=1234-5678|year=2009}}";
            Assert.That(Parsers.FixCitationTemplates(@"{{cite journal|title=foo|journal=foo|id=ISSN 12345678|year=2009}}"), Is.EqualTo(correct), "id ISSN conversion, unspaced");
            Assert.That(Parsers.FixCitationTemplates(@"{{cite journal|title=foo|journal=foo|id=ISSN: 12345678|year=2009}}"), Is.EqualTo(correct), "id ISSN conversion, colon");
            Assert.That(Parsers.FixCitationTemplates(@"{{cite journal|title=foo|journal=foo|id=ISSN : 12345678|year=2009}}"), Is.EqualTo(correct), "id ISSN conversion, colon with space");
            Assert.That(Parsers.FixCitationTemplates(@"{{cite journal|title=foo|journal=foo|id=ISSN= 12345678|year=2009}}"), Is.EqualTo(correct), "id ISSN conversion, equal sign");
            Assert.That(Parsers.FixCitationTemplates(@"{{cite journal|title=foo|journal=foo|id=ISSN = 12345678|year=2009}}"), Is.EqualTo(correct), "id ISSN conversion, equal sign with space");
            Assert.That(Parsers.FixCitationTemplates(@"{{cite journal|title=foo|journal=foo|id=ISSN 1234 5678|year=2009}}"), Is.EqualTo(correct), "id ISSN conversion, spaced");
            Assert.That(Parsers.FixCitationTemplates(@"{{cite journal|title=foo|journal=foo|id=ISSN 1234  5678|year=2009}}"), Is.EqualTo(correct), "id ISSN conversion, spaced");
            Assert.That(Parsers.FixCitationTemplates(@"{{cite journal|title=foo|journal=foo|id=ISSN 1234-5678|year=2009}}"), Is.EqualTo(correct), "id ISSN conversion, dashed");
            Assert.That(Parsers.FixCitationTemplates(@"{{cite journal|title=foo|journal=foo|id=ISSN 1234 - 5678|year=2009}}"), Is.EqualTo(correct), "id ISSN conversion, dashed");
            Assert.That(Parsers.FixCitationTemplates(@"{{cite journal|title=foo|journal=foo|id=issn 1234-5678|year=2009}}"), Is.EqualTo(correct), "id ISSN conversion, lowercase");
            Assert.That(Parsers.FixCitationTemplates(@"{{cite journal|title=foo|journal=foo|ID=issn 1234-5678|year=2009}}"), Is.EqualTo(correct), "ID ISSN conversion, uppercase ID");

            Assert.That(Parsers.FixCitationTemplates(@"{{cite journal|title=foo|journal=foo|id=ISSN 1234567X|year=2009}}"), Is.EqualTo(@"{{cite journal|title=foo|journal=foo|issn=1234-567X|year=2009}}"), "id ISSN conversion, unspaced");

            const string BadISSN = @"{{cite journal|title=foo|journal=foo|id=issn 1234-56789X|year=2009}}";
            Assert.That(Parsers.FixCitationTemplates(BadISSN), Is.EqualTo(BadISSN), "No change, invalid ISSN");
            string ExistingISSN = @"{{cite journal|title=foo|journal=foo|id=issn 1234-5678|year=2009|issn= 1234-5678}}";
            Assert.That(Parsers.FixCitationTemplates(ExistingISSN), Is.EqualTo(ExistingISSN), "No change, existing issn");
            ExistingISSN = @"{{cite journal|title=foo|journal=foo|id=issn 1234-5678|year=2009|ISSN= 1234-5678}}";
            Assert.That(Parsers.FixCitationTemplates(ExistingISSN), Is.EqualTo(ExistingISSN), "No change, existing ISSN");
        }

        [Test]
        public void FixCitationTemplatesISSNFormat()
        {
            const string correct = @"{{cite journal|title=foo|journal=foo|ISSN=1234-5678|year=2009}}";
            Assert.That(Parsers.FixCitationTemplates(@"{{cite journal|title=foo|journal=foo|ISSN=12345678|year=2009}}"), Is.EqualTo(correct), "format ISSN, unspaced");
            Assert.That(Parsers.FixCitationTemplates(@"{{cite journal|title=foo|journal=foo|ISSN=1234–5678|year=2009}}"), Is.EqualTo(correct), "format ISSN, dash");
            Assert.That(Parsers.FixCitationTemplates(@"{{cite journal|title=foo|journal=foo|ISSN=1234 5678|year=2009}}"), Is.EqualTo(correct), "format ISSN, spaced");
            Assert.That(Parsers.FixCitationTemplates(@"{{cite journal|title=foo|journal=foo|ISSN=1234  5678|year=2009}}"), Is.EqualTo(correct), "format ISSN, doublespaced");
            Assert.That(Parsers.FixCitationTemplates(@"{{cite journal|title=foo|journal=foo|ISSN=1234 - 5678|year=2009}}"), Is.EqualTo(correct), "format ISSN, dashed");
            Assert.That(Parsers.FixCitationTemplates(@"{{cite journal|title=foo|journal=foo|ISSN=1234567X|year=2009}}"), Is.EqualTo(@"{{cite journal|title=foo|journal=foo|ISSN=1234-567X|year=2009}}"), "format ISSN, dashed");

            Assert.That(Parsers.FixCitationTemplates(correct), Is.EqualTo(correct), "No change, already correct");

            Assert.That(Parsers.FixCitationTemplates(@"{{cite journal|title=foo|journal=foo|issn=12345678|year=2009}}"), Is.EqualTo(@"{{cite journal|title=foo|journal=foo|issn=1234-5678|year=2009}}"), "format lowercase issn");
        }

        [Test]
        public void FixCitationTemplatesISBN()
        {
            // id=ISBN... fix
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|title=foo|id=ISBN 123456789X|year=2009}}"), Is.EqualTo(@"{{cite book|title=foo|isbn=123456789X|year=2009}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|title=foo|id=ISBN 123456789X |year=2009}}"), Is.EqualTo(@"{{cite book|title=foo|isbn=123456789X |year=2009}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|title=foo|ID=ISBN 123456789X |year=2009}}"), Is.EqualTo(@"{{cite book|title=foo|isbn=123456789X |year=2009}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|title=foo|id=ISBN: 123-45678-9-X|year=2009}}"), Is.EqualTo(@"{{cite book|title=foo|isbn=123-45678-9-X|year=2009}}"));

            string doubleISBN = @"{{cite book|title=foo|id=ISBN 012345678X, 978012345678X|year=2009}}";
            Assert.That(Parsers.FixCitationTemplates(doubleISBN), Is.EqualTo(doubleISBN), "no changes when two isbns present");
            doubleISBN = @"{{cite book|title=foo|id=ISBN 012345678X ISBN 978012345678X|year=2009}}";
            Assert.That(Parsers.FixCitationTemplates(doubleISBN), Is.EqualTo(doubleISBN), "no changes when two isbns present");

            string existingISBN = @"{{cite book|title=foo|id=ISBN 012345678X |isbn= 978012345678X|year=2009}}";
            Assert.That(Parsers.FixCitationTemplates(existingISBN), Is.EqualTo(existingISBN), "no changes when isbn param already has value");

            existingISBN = @"{{cite book|title=foo|id=ISBN 012345678X |ISBN= 978012345678X|year=2009}}";
            Assert.That(Parsers.FixCitationTemplates(existingISBN), Is.EqualTo(existingISBN), "no changes when isbn param already has value");

            // ISBN format fixes
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|title=foo|isbn=ISBN 123456789X|year=2009}}"), Is.EqualTo(@"{{cite book|title=foo|isbn=123456789X|year=2009}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|title=foo|ISBN=ISBN 123456789X|year=2009}}"), Is.EqualTo(@"{{cite book|title=foo|ISBN=123456789X|year=2009}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|title=foo|isbn=ISBN  123456789X|year=2009}}"), Is.EqualTo(@"{{cite book|title=foo|isbn=123456789X|year=2009}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|title=foo|isbn=ISBN123456789X|year=2009}}"), Is.EqualTo(@"{{cite book|title=foo|isbn=123456789X|year=2009}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|title=foo|isbn=isbn123456789X|year=2009}}"), Is.EqualTo(@"{{cite book|title=foo|isbn=123456789X|year=2009}}"));

            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|title=foo|isbn=ISBN 123456789X.|year=2009}}"), Is.EqualTo(@"{{cite book|title=foo|isbn=123456789X|year=2009}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|title=foo|isbn=123456789X.|year=2009}}"), Is.EqualTo(@"{{cite book|title=foo|isbn=123456789X|year=2009}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|title=foo|isbn=123456789X..|year=2009}}"), Is.EqualTo(@"{{cite book|title=foo|isbn=123456789X|year=2009}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|title=foo|isbn=123456789X,|year=2009}}"), Is.EqualTo(@"{{cite book|title=foo|isbn=123456789X|year=2009}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|title=foo|isbn=123456789X;|year=2009}}"), Is.EqualTo(@"{{cite book|title=foo|isbn=123456789X|year=2009}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|title=foo|isbn=123456789X:|year=2009}}"), Is.EqualTo(@"{{cite book|title=foo|isbn=123456789X|year=2009}}"));

            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|title=foo|isbn=123–456–789–X :|year=2009}}"), Is.EqualTo(@"{{cite book|title=foo|isbn=123-456-789-X|year=2009}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|title=foo|isbn=123–4–56789–X |year=2009}}"), Is.EqualTo(@"{{cite book|title=foo|isbn=123-4-56789-X |year=2009}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|title=foo|isbn=123‐4‐56789‐X |year=2009}}"), Is.EqualTo(@"{{cite book|title=foo|isbn=123-4-56789-X |year=2009}}")); // U+2010 character
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|title=foo|isbn=123‒4‒56789‒X |year=2009}}"), Is.EqualTo(@"{{cite book|title=foo|isbn=123-4-56789-X |year=2009}}")); // U+2012 character
        }

        [Test]
        public void FixCitationTemplatesOrigYear()
        {
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book | title=ABC | publisher=Pan  | origyear=1950 }}"), Is.EqualTo(@"{{cite book | title=ABC | publisher=Pan  | year=1950 }}"), "origyear to year when no year/date");
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book | title=ABC | publisher=Pan  | origyear=1950 | year =}}"), Is.EqualTo(@"{{cite book | title=ABC | publisher=Pan  | year=1950 }}"), "origyear to year when blank year");

            const string nochange1 = @"{{cite book | title=ABC | publisher=Pan | year=2004 | origyear=1950 }}", nochange2 = @"{{cite book | title=ABC | publisher=Pan | date=May 2004 | origyear=1950 }}"
                , nochange3 = @"{{cite book | title=ABC | publisher=Pan | origyear=11 May 1950 }}";

            Assert.That(Parsers.FixCitationTemplates(nochange1), Is.EqualTo(nochange1), "origyear valid when year present");
            Assert.That(Parsers.FixCitationTemplates(nochange2), Is.EqualTo(nochange2), "origyear valid when date present");
            Assert.That(Parsers.FixCitationTemplates(nochange3), Is.EqualTo(nochange3), "origyear not renamed when more than just a year");
        }

        [Test]
        public void FixCitationTemplatesEnOnly()
        {
#if DEBUG
            const string bad = @"{{cite web|title=foo|url=http://site.net|year=2009|format=HTML}}";

            Variables.SetProjectLangCode("fr");
            Assert.That(Parsers.FixCitationTemplates(bad), Is.EqualTo(bad));

            Variables.SetProjectLangCode("en");
            Assert.That(Parsers.FixCitationTemplates(bad), Is.EqualTo(@"{{cite web|title=foo|url=http://site.net|year=2009}}"));
#endif
        }

        [Test]
        public void FixCitationTemplatesPagesPP()
        {
            // removal of pp. from 'pages' field
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|pages=pp. 57–59}}"), Is.EqualTo(@"{{cite book|author=Smith|title=Great|pages=57–59}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|pages=pgs. 57–59}}"), Is.EqualTo(@"{{cite book|author=Smith|title=Great|pages=57–59}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|pages=pg. 57–59}}"), Is.EqualTo(@"{{cite book|author=Smith|title=Great|pages=57–59}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|pages=pp.57–59}}"), Is.EqualTo(@"{{cite book|author=Smith|title=Great|pages=57–59}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|pages=pp.&nbsp;57–59}}"), Is.EqualTo(@"{{cite book|author=Smith|title=Great|pages=57–59}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|pages= pp. 57–59}}"), Is.EqualTo(@"{{cite book|author=Smith|title=Great|pages= 57–59}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|pages=pp. 57-59|year=2007}}"), Is.EqualTo(@"{{cite book|author=Smith|title=Great|pages=57–59|year=2007}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|page=p. 57}}"), Is.EqualTo(@"{{cite book|author=Smith|title=Great|page=57}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|page=p 57}}"), Is.EqualTo(@"{{cite book|author=Smith|title=Great|page=57}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|page=pp 57}}"), Is.EqualTo(@"{{cite book|author=Smith|title=Great|page=57}}"));

            const string nochange0 = @"{{cite book|author=Smith|title=Great|page= 57}}", nochange1 = @"{{cite book|author=Smith|title=Great|page= para 57}}",
            nochange2 = @"{{cite book|author=Smith|title=Great|page= P57}}";
            Assert.That(Parsers.FixCitationTemplates(nochange0), Is.EqualTo(nochange0));
            Assert.That(Parsers.FixCitationTemplates(nochange1), Is.EqualTo(nochange1));
            Assert.That(Parsers.FixCitationTemplates(nochange2), Is.EqualTo(nochange2));

            // not when nopp
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|pages=pp. 57-59|nopp=yes}}"), Is.EqualTo(@"{{cite book|author=Smith|title=Great|pages=pp. 57–59|nopp=yes}}"));

            // not for cite journal
            const string journal = @"{{cite journal|author=Smith|title=Great|page=p.57}}";
            Assert.That(Parsers.FixCitationTemplates(journal), Is.EqualTo(journal));
        }

        [Test]
        public void FixCitationTemplatesPageRangeName()
        {
            string correct = @"{{cite book|author=Smith|title=Great|pages=57–59}}";
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|pages=pp. 57–59}}"), Is.EqualTo(correct));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|page=pp. 57–59}}"), Is.EqualTo(correct));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|pages=57–59}}"), Is.EqualTo(correct));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|pages=57 – 59}}"), Is.EqualTo(correct));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|pages=57 – 59}}"), Is.EqualTo(correct));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|pages=57--59}}"), Is.EqualTo(correct));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|page=57–59}}"), Is.EqualTo(correct));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|page=57—59}}"), Is.EqualTo(correct));

            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|page=57, 59}}"), Is.EqualTo(correct.Replace("–", ", ")), "page -> pages for comma list of page numbers");

            correct = @"{{cite book|author=Smith|title=Great|pages=57&ndash;59}}";
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|pages=57 &ndash; 59}}"), Is.EqualTo(correct));

            const string nochange = @"{{cite book|author=Smith|title=Great|pages=12,255}}";
            Assert.That(Parsers.FixCitationTemplates(nochange), Is.EqualTo(nochange));

            const string correct2 = @"{{cite book |title=Evaluation of x |last=Office |year=2001 |pages=1–2 |accessdate=39 June 2011 }}";

            Assert.That(Parsers.FixCitationTemplates(@"{{cite book |title=Evaluation of x |last=Office |year=2001 |page=1-2 |pages= |accessdate=39 June 2011 }}"), Is.EqualTo(correct2));

            const string correct3 = @"{{cite book | page = ੯-੨ }}";
            Assert.That(Parsers.FixCitationTemplates(correct3), Is.EqualTo(correct3), "No change to non-standard numbers");
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|author=Smith|title=Great|page=59999990—59999999}}"), Is.EqualTo(@"{{cite book|author=Smith|title=Great|pages=59999990–59999999}}"));
            const string tooLong = @"{{cite book|author=Smith|title=Great|page=57999999000—59999999000}}";
            Assert.That(Parsers.FixCitationTemplates(tooLong), Is.EqualTo(tooLong));
            const string correct4 = @"{{cite book | at = 5-7 }}";
            Assert.That(Parsers.FixCitationTemplates(correct4), Is.EqualTo(correct4), "No change within at parameter");
            const string correct5 = @"{{cite book | page = [https://site.com/a-1990-1994 123] }}";
            Assert.That(Parsers.FixCitationTemplates(correct5), Is.EqualTo(correct5), "No change to text within link in pages param");
        }

        [Test]
        public void FixCitationTemplatesVolume()
        {
            const string correct = @"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume= 3|issue= 3|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ";
            Assert.That(Parsers.FixCitationTemplates(@"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume=Vol. 3|issue=No. 3|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ"),
                            Is.EqualTo(correct));
            Assert.That(Parsers.FixCitationTemplates(@"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume=Volumes 3|issue=No. 3|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ"),
                            Is.EqualTo(correct));

            Assert.That(Parsers.FixCitationTemplates(@"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume=vol. 3|issue=No. 3|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ"),
                            Is.EqualTo(correct));

            Assert.That(Parsers.FixCitationTemplates(@"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume=volume 3|issue=Issue 3|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ"),
                            Is.EqualTo(correct));

            Assert.That(Parsers.FixCitationTemplates(@"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume=volume 3|issue=Issues 3|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ"),
                            Is.EqualTo(correct));

            Assert.That(Parsers.FixCitationTemplates(@"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume=volume 3|issue=Issue 3|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ"),
                            Is.EqualTo(correct));
            Assert.That(Parsers.FixCitationTemplates(@"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume=Vol. 3|issue=Nos. 3|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ"),
                            Is.EqualTo(correct));

            Assert.That(Parsers.FixCitationTemplates(@"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume=volume 3 Issue 3|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ"),
                            Is.EqualTo(@"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume= 3| issue = 3|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ"));

            Assert.That(Parsers.FixCitationTemplates(@"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume=volume 3 Issues 3–4|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ"),
                            Is.EqualTo(@"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume= 3| issue = 3–4|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ"));
            Assert.That(Parsers.FixCitationTemplates(@"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume=3, Nos. 3–4|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ"),
                            Is.EqualTo(@"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume=3| issue =  3–4|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ"));


            Assert.That(Parsers.FixCitationTemplates(@"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume=volume 3 numbers 3–4|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ"),
                            Is.EqualTo(@"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume= 3| issue = 3–4|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}} Robert M. Price (ed.), Bloomfield, NJ"));

            string nochange1 = @"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: A Pulp Thriller and Theological Journal |volume=special numbers 3–4|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm}}";

            Assert.That(Parsers.FixCitationTemplates(nochange1), Is.EqualTo(nochange1));

            string nochange2 = @"*{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu #19: |volume=3 Issue 3|url=http://www.clare.ltd.new.net/cryptofcthulhu/blreanimator.htm|issue=December}} Robert M. Price (ed.), Bloomfield, NJ";

            Assert.That(Parsers.FixCitationTemplates(nochange2), Is.EqualTo(nochange2));

            nochange2 = @"*{{cite journal |title=Lia |journal=Control |volume=I Nov 1962 II Dec 1962 |year=1962 }}";

            Assert.That(Parsers.FixCitationTemplates(nochange2), Is.EqualTo(nochange2), "No change to No as part of word in volume param");
        }

        [Test]
        public void CiteTemplatesPageRange()
        {
            const string correct = @"{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu |volume= 3|issue= 3|pages = 140–148}}";

            Assert.That(Parsers.FixCitationTemplates(@"{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu |volume= 3|issue= 3|pages = 140-148}}"), Is.EqualTo(correct)); // hyphen
            Assert.That(Parsers.FixCitationTemplates(@"{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu |volume= 3|issue= 3|pages = 140   - 148}}"), Is.EqualTo(correct)); // hyphen

            const string journalstart = @"{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu |volume= 3|issue= 3| ";

            Assert.That(Parsers.FixCitationTemplates(journalstart + @"pages = 140   - 148}}"), Is.EqualTo(journalstart + @"pages = 140–148}}")); // hyphen

            Assert.That(Parsers.FixCitationTemplates(journalstart + @"pages = 140-8}}"), Is.EqualTo(journalstart + @"pages = 140–8}}")); // hyphen

            Assert.That(Parsers.FixCitationTemplates(journalstart + @"pages = 140 -48}}"), Is.EqualTo(journalstart + @"pages = 140–48}}")); // hyphen

            Assert.That(Parsers.FixCitationTemplates(journalstart + @"pages = 140   - 148}}"), Is.EqualTo(journalstart + @"pages = 140–148}}")); // hyphen
            Assert.That(Parsers.FixCitationTemplates(journalstart + @"pages = 940   - 1083}}"), Is.EqualTo(journalstart + @"pages = 940–1083}}")); // hyphen

            // multiple ranges
            Assert.That(Parsers.FixCitationTemplates(journalstart + @"pages = 140-148, 150, 152-157}}"), Is.EqualTo(journalstart + @"pages = 140–148, 150, 152–157}}")); // hyphen

            const string nochange1 = @"{{cite conference
  | first = Owen
  | title = System Lifecycle Cost
  | booktitle = AIAA Space 2007
  | pages = Paper No. AIAA-2007–6023
  | year = 2007
  }}";

            Assert.That(Parsers.FixCitationTemplates(nochange1), Is.EqualTo(nochange1)); // range over 999 pages
        }

        [Test]
        public void HarvTemplatesPageRange()
        {
            Assert.That(Parsers.FixCitationTemplates(@"{{harv|Smith|2005|pp=55–59}}"), Is.EqualTo(@"{{harv|Smith|2005|pp=55–59}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{harvnb|Smith|2005|pp=55–59}}"), Is.EqualTo(@"{{harvnb|Smith|2005|pp=55–59}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{harv|Smith|2005|pp=55 – 59}}"), Is.EqualTo(@"{{harv|Smith|2005|pp=55–59}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{harv|Smith|2005|pp=55–59, 77-81}}"), Is.EqualTo(@"{{harv|Smith|2005|pp=55–59, 77–81}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{harv|Smith|at=7-8}}"), Is.EqualTo(@"{{harv|Smith|at=7-8}}"), "no change within at parameter");


            Assert.That(Parsers.FixCitationTemplates(@"{{rp|55–59, 77-81}}"), Is.EqualTo(@"{{rp|55–59, 77–81}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{rp|77-81}}"), Is.EqualTo(@"{{rp|77–81}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{rp|77}}"), Is.EqualTo(@"{{rp|77}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{rp|77, 80}}"), Is.EqualTo(@"{{rp|77, 80}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{rp|at=7-8}}"), Is.EqualTo(@"{{rp|at=7-8}}"), "rp: no change within at parameter");
        }

        [Test]
        public void HarvTemplatesPP()
        {
            Assert.That(Parsers.FixCitationTemplates(@"{{harv|Smith|2005|p=55–59}}"), Is.EqualTo(@"{{harv|Smith|2005|pp=55–59}}"), "renames p to pp for page range");
            Assert.That(Parsers.FixCitationTemplates(@"{{harvnb|Smith|2005|p=55–59}}"), Is.EqualTo(@"{{harvnb|Smith|2005|pp=55–59}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{harvnb|Smith|2005|p=55&ndash;59}}"), Is.EqualTo(@"{{harvnb|Smith|2005|pp=55&ndash;59}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{harv|Smith|2005|p=55 – 59}}"), Is.EqualTo(@"{{harv|Smith|2005|pp=55–59}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{harv|Smith|2005|p=55–59, 77-81}}"), Is.EqualTo(@"{{harv|Smith|2005|pp=55–59, 77–81}}"));

            const string nochange = @"{{Harvnb|Shapiro|2010|p=271 (238–9)}}";
            Assert.That(Parsers.FixCitationTemplates(nochange), Is.EqualTo(nochange));
        }

        [Test]
        public void CiteTemplatesPageSections()
        {
            const string journalstart = @"{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu |volume= 3|issue= 3| ";

            // do not change page sections etc.
            const string nochange1 = @"{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu |volume= 3|issue= 3|pages = 140–7-2}}";

            Assert.That(Parsers.FixCitationTemplates(nochange1), Is.EqualTo(nochange1));

            Assert.That(Parsers.FixCitationTemplates(journalstart + @"pages = 8-4}}"), Is.EqualTo(journalstart + @"pages = 8-4}}")); // hyphen
            Assert.That(Parsers.FixCitationTemplates(journalstart + @"pages = 8-8}}"), Is.EqualTo(journalstart + @"pages = 8-8}}")); // hyphen
            Assert.That(Parsers.FixCitationTemplates(journalstart + @"pages = 8-4, 17-34}}"), Is.EqualTo(journalstart + @"pages = 8-4, 17-34}}")); // hyphen
            Assert.That(Parsers.FixCitationTemplates(journalstart + @"pages = 17-34, 8-4}}"), Is.EqualTo(journalstart + @"pages = 17-34, 8-4}}")); // hyphen

            // non-breaking hyphens to represent page sections rather than ranges
            const string nochange2 = @"{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu |volume= 3|issue= 3|pages = 140‑7}}", nochange3 = @"{{cite journal|first=Robert M.|last=Price|year=Candlemas 1984|title=Brian Lumley&mdash;Reanimator|
journal=Crypt of Cthulhu |volume= 3|issue= 3|pages = 140&#8209;7}}";

            Assert.That(Parsers.FixCitationTemplates(nochange2), Is.EqualTo(nochange2));
            Assert.That(Parsers.FixCitationTemplates(nochange3), Is.EqualTo(nochange3));

            const string nochange4 = @"*{{cite book
 | author = United States Navy
 | pages = 4-13 to 4-17
 | chapter = 4
}}";

            Assert.That(Parsers.FixCitationTemplates(nochange4), Is.EqualTo(nochange4), "two section links with word to");

            const string nochange5a = @"{{cite book | isbn = 084
 | pages = 3-262, 8-106, 15-20
 | url =
}}", nochange5b = @"{{cite book | isbn = 084
 | pages = 3-262, 3-106, 15-20
 | url =
}}";
            Assert.That(Parsers.FixCitationTemplates(nochange5a), Is.EqualTo(nochange5a), "overlapping ranges");
            Assert.That(Parsers.FixCitationTemplates(nochange5b), Is.EqualTo(nochange5b), "overlapping ranges, same start");
        }

        [Test]
        public void FixCitationTemplatesOrdinalDates()
        {
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|url=http://news.bbc.co.uk/1/hi/uk_politics/8080777.stm|title=Brown pressure as Blears quits|
publisher=The BBC|date=June 3rd, 2009|accessdate=January 15th, 2010}}"), Is.EqualTo(@"{{cite web|url=http://news.bbc.co.uk/1/hi/uk_politics/8080777.stm|title=Brown pressure as Blears quits|
publisher=The BBC|date=June 3, 2009|accessdate=January 15, 2010}}"));

            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|url=http://news.bbc.co.uk/1/hi/uk_politics/8080777.stm|title=Brown pressure as Blears quits|
publisher=The BBC|date=June 3rd, 2009|accessdate=January 15th 2010}}"), Is.EqualTo(@"{{cite web|url=http://news.bbc.co.uk/1/hi/uk_politics/8080777.stm|title=Brown pressure as Blears quits|
publisher=The BBC|date=June 3, 2009|accessdate=January 15, 2010}}"));

            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|url=http://news.bbc.co.uk/1/hi/uk_politics/8080777.stm|title=Brown pressure as Blears quits|
publisher=The BBC|date=June 3rd, 2009|accessdate=January 15, 2010}}"), Is.EqualTo(@"{{cite web|url=http://news.bbc.co.uk/1/hi/uk_politics/8080777.stm|title=Brown pressure as Blears quits|
publisher=The BBC|date=June 3, 2009|accessdate=January 15, 2010}}"));

            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|url=http://news.bbc.co.uk/1/hi/uk_politics/8080777.stm|title=Brown pressure as Blears quits|
publisher=The BBC|date=3rd June 2009|accessdate=15th January 2010}}"), Is.EqualTo(@"{{cite web|url=http://news.bbc.co.uk/1/hi/uk_politics/8080777.stm|title=Brown pressure as Blears quits|
publisher=The BBC|date=3 June 2009|accessdate=15 January 2010}}"));

            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|url=http://news.bbc.co.uk/1/hi/uk_politics/8080777.stm|title=Brown pressure as Blears quits|
publisher=The BBC|date=3rd June 2009|access-date=15th January 2010}}"), Is.EqualTo(@"{{cite web|url=http://news.bbc.co.uk/1/hi/uk_politics/8080777.stm|title=Brown pressure as Blears quits|
publisher=The BBC|date=3 June 2009|access-date=15 January 2010}}"));

            // no change - only in title
            string nochange = @"{{cite web|url=http://news.bbc.co.uk/1/hi/uk_politics/8080777.stm|title=Brown pressure at January 15th, 2010}}";

            Assert.That(Parsers.FixCitationTemplates(nochange), Is.EqualTo(nochange));

            Assert.That(Parsers.FixCitationTemplates(@"{{cite web | url=http://www.foo.com| title=Bar | date=January 28th, 2013 | accessdate=March 07, 2013 | other=}}"),
                            Is.EqualTo(@"{{cite web | url=http://www.foo.com| title=Bar | date=January 28, 2013 | accessdate=March 7, 2013 | other=}}"));
        }

        [Test]
        public void UppercaseCiteFields()
        {
            // single uppercase field
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|FOO=hello|title=hello}}"), Is.EqualTo(@"{{cite web|foo=hello|title=hello}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|uRL=http://members.bib-arch.org|title=hello}}"), Is.EqualTo(@"{{cite web|url=http://members.bib-arch.org|title=hello}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|UrL=http://members.bib-arch.org|title=hello}}"), Is.EqualTo(@"{{cite web|url=http://members.bib-arch.org|title=hello}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|Url=http://members.bib-arch.org|title=hello}}"), Is.EqualTo(@"{{cite web|url=http://members.bib-arch.org|title=hello}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{Cite web|FOO=hello|title=hello}}"), Is.EqualTo(@"{{Cite web|foo=hello|title=hello}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web | FOO=hello|title=hello}}"), Is.EqualTo(@"{{cite web | foo=hello|title=hello}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web | FOO = hello|title=hello}}"), Is.EqualTo(@"{{cite web | foo = hello|title=hello}}"));

            // multiple uppercase fields
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|FOO=hello|Title=hello}}"), Is.EqualTo(@"{{cite web|foo=hello|title=hello}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|FOO=hello|TITLE=hello}}"), Is.EqualTo(@"{{cite web|foo=hello|title=hello}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|FOO=hello|TITLE=hello | Work=BBC}}"), Is.EqualTo(@"{{cite web|foo=hello|title=hello | work=BBC}}"));

            //other templates
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web|FOO=hello|title=hello}}"), Is.EqualTo(@"{{cite web|foo=hello|title=hello}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite book|FOO=hello|title=hello}}"), Is.EqualTo(@"{{cite book|foo=hello|title=hello}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite news|FOO=hello|title=hello}}"), Is.EqualTo(@"{{cite news|foo=hello|title=hello}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite journal|FOO=hello|title=hello}}"), Is.EqualTo(@"{{cite journal|foo=hello|title=hello}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite paper|FOO=hello|title=hello}}"), Is.EqualTo(@"{{cite paper|foo=hello|title=hello}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite press release|FOO=hello|title=hello}}"), Is.EqualTo(@"{{cite press release|foo=hello|title=hello}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite hansard|FOO=hello|title=hello}}"), Is.EqualTo(@"{{cite hansard|foo=hello|title=hello}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{cite encyclopedia|FOO=hello|title=hello}}"), Is.EqualTo(@"{{cite encyclopedia|foo=hello|title=hello}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{citation|FOO=hello|title=hello}}"), Is.EqualTo(@"{{citation|foo=hello|title=hello}}"));

            Assert.That(Parsers.FixCitationTemplates(@"{{Cite web|FOO=hello|title=hello}}"), Is.EqualTo(@"{{Cite web|foo=hello|title=hello}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{Cite book|FOO=hello|title=hello}}"), Is.EqualTo(@"{{Cite book|foo=hello|title=hello}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{Cite news|FOO=hello|title=hello}}"), Is.EqualTo(@"{{Cite news|foo=hello|title=hello}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{Cite journal|FOO=hello|title=hello}}"), Is.EqualTo(@"{{Cite journal|foo=hello|title=hello}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{Cite paper|FOO=hello|title=hello}}"), Is.EqualTo(@"{{Cite paper|foo=hello|title=hello}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{Cite press release|FOO=hello|title=hello}}"), Is.EqualTo(@"{{Cite press release|foo=hello|title=hello}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{Cite hansard|FOO=hello|title=hello}}"), Is.EqualTo(@"{{Cite hansard|foo=hello|title=hello}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{Cite encyclopedia|FOO=hello|title=hello}}"), Is.EqualTo(@"{{Cite encyclopedia|foo=hello|title=hello}}"));
            Assert.That(Parsers.FixCitationTemplates(@"{{Citation|FOO=hello|title=hello}}"), Is.EqualTo(@"{{Citation|foo=hello|title=hello}}"));

            Assert.That(Parsers.FixCitationTemplates(@"{{cite book | author=Smith | Title=Great Book | ISBN=15478454 | date=17 May 2004 }}"), Is.EqualTo(@"{{cite book | author=Smith | title=Great Book | ISBN=15478454 | date=17 May 2004 }}"));

            // ISBN, DOI, PMID, PMC, LCCN, ASIN is allowed to be uppercase
            string ISBN = @"{{cite book | author=Smith | title=Great Book | ISBN=15478454 | date=17 May 2004 }}";
            Assert.That(Parsers.FixCitationTemplates(ISBN), Is.EqualTo(ISBN));
            string ISSN = @"{{cite book | author=Smith | title=Great Book | ISSN=1547-8454 | date=17 May 2004 }}";
            Assert.That(Parsers.FixCitationTemplates(ISSN), Is.EqualTo(ISSN));
            string OCLC = @"{{cite book | author=Smith | title=Great Book | OCLC=15478454 | date=17 May 2004 }}";
            Assert.That(Parsers.FixCitationTemplates(OCLC), Is.EqualTo(OCLC));
            string DOI = @"{{cite journal| author=Smith | title=Great Book | DOI=15478454 | date=17 May 2004 }}";
            Assert.That(Parsers.FixCitationTemplates(DOI), Is.EqualTo(DOI));
            string PMID = @"{{cite journal| author=Smith | title=Great Book | PMID=15478454 | date=17 May 2004 }}";
            Assert.That(Parsers.FixCitationTemplates(PMID), Is.EqualTo(PMID));
            string PMC = @"{{cite journal| author=Smith | title=Great Book | PMC=15478454 | date=17 May 2004 }}";
            Assert.That(Parsers.FixCitationTemplates(PMC), Is.EqualTo(PMC));
            string LCCN = @"{{cite journal| author=Smith | title=Great Book | PMC=15478454 | date=17 May 2004 }}";
            Assert.That(Parsers.FixCitationTemplates(LCCN), Is.EqualTo(LCCN));
            string ASIN = @"{{cite journal| author=Smith | title=Great Book | ASIN=15478454 | date=17 May 2004 }}";
            Assert.That(Parsers.FixCitationTemplates(ASIN), Is.EqualTo(ASIN));

            // don't match on part of URL
            string URL = @"{{cite news|url=http://www.expressbuzz.com/edition/story.aspx?Title=Catching++them+young&artid=rPwTAv2l2BY=&SectionID=fxm0uEWnVpc=&MainSectionID=ngGbWGz5Z14=&SectionName=RtFD/|pZbbWSsbI0jf3F5Q==&SEO=|title=Catching them young|date=August 7, 2009|work=[[The Indian Express]]|accessdate=2009-08-07}}";
            Assert.That(Parsers.FixCitationTemplates(URL), Is.EqualTo(URL));
        }

        [Test]
        public void FixCitationTemplatesDeadLinkInFormat()
        {
            const string correct = @"{{cite web | url=http://www.site.com/article100.html | title=Foo }} {{dead link|date=May 2010}}";
            Assert.That(Parsers.FixCitationTemplates(@"{{cite web | url=http://www.site.com/article100.html | title=Foo | format= {{dead link|date=May 2010}}}}"), Is.EqualTo(correct), "{{dead link}} taken out of format field");

            Assert.That(Parsers.FixCitationTemplates(@"{{cite web | url=http://www.site.com/article100.html | title=Foo | format= PDF {{dead link|date=May 2010}}}}"), Is.EqualTo(correct.Replace("Foo", "Foo | format= PDF")), "Only {{dead link}} taken out of format field");

            Assert.That(Parsers.FixCitationTemplates(correct), Is.EqualTo(correct), "no change when already correct");

            const string NodDead = @"{{cite web | url=http://www.site.com/article100.html | title=Foo | format= PDF}}";
            Assert.That(Parsers.FixCitationTemplates(NodDead), Is.EqualTo(NodDead), "no change when no dead link in format field");
        }

        [Test]
        public void TestBracketsAtCiteTemplateURL()
        {
            const string correct = @"now {{cite web|url=http://site.net|title=hello}} was";

            Assert.That(Parsers.FixCitationTemplates(@"now {{cite web|url=[http://site.net]|title=hello}} was"), Is.EqualTo(correct));
            Assert.That(Parsers.FixCitationTemplates(@"now {{cite web|url=http://site.net]|title=hello}} was"), Is.EqualTo(correct));
            Assert.That(Parsers.FixCitationTemplates(@"now {{cite web|url=[http://site.net|title=hello}} was"), Is.EqualTo(correct));
            Assert.That(Parsers.FixCitationTemplates(@"now {{cite web|url=[[http://site.net]|title=hello}} was"), Is.EqualTo(correct));
            Assert.That(Parsers.FixCitationTemplates(@"now {{cite web|url=[http://site.net]]|title=hello}} was"), Is.EqualTo(correct));
            Assert.That(Parsers.FixCitationTemplates(@"now {{cite web|url=[[http://site.net]]|title=hello}} was"), Is.EqualTo(correct));
            Assert.That(Parsers.FixCitationTemplates(@"now {{cite web|url = [http://site.net]  |title=hello}} was"), Is.EqualTo(@"now {{cite web|url = http://site.net  |title=hello}} was"));
            Assert.That(Parsers.FixCitationTemplates(@"now {{cite web|title=hello |url=[www.site.net]}} was"), Is.EqualTo(@"now {{cite web|title=hello |url=http://www.site.net}} was"), "bracket and protocol fix combined");
            Assert.That(Parsers.FixCitationTemplates(@"now {{cite journal|title=hello | url=[http://site.net]] }} was"), Is.EqualTo(@"now {{cite journal|title=hello | url=http://site.net }} was"));

            // no match
            Assert.That(Parsers.FixCitationTemplates(@"now {{cite web| url=http://site.net|title=hello}} was"), Is.EqualTo(@"now {{cite web| url=http://site.net|title=hello}} was"));
            Assert.That(Parsers.FixCitationTemplates(@"now {{cite web| url=[http://site.net cool site]|title=hello}} was"), Is.EqualTo(@"now {{cite web| url=[http://site.net cool site]|title=hello}} was"));
            Assert.That(Parsers.FixCitationTemplates(@"now {{cite web| url=http://site.net cool site]|title=hello}} was"), Is.EqualTo(@"now {{cite web| url=http://site.net cool site]|title=hello}} was"));
            Assert.That(Parsers.FixCitationTemplates(@"now {{cite web| url=[http://site.net cool site|title=hello}} was"), Is.EqualTo(@"now {{cite web| url=[http://site.net cool site|title=hello}} was"));
        }
    }
}