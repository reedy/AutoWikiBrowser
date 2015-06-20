/*

Copyright (C) 2007 Martin Richards

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
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using WikiFunctions.Lists.Providers;

namespace WikiFunctions.Parse
{

    /// <summary>
    /// Provides functions for editing wiki text, such as formatting and re-categorisation.
    /// </summary>
    public partial class Parsers
    {
        #region FixCitationTemplates
        private static readonly Regex CiteUrl = new Regex(@"\|\s*url\s*=\s*([^\[\]<>""\s]+)");

        private static readonly Regex WorkInItalics = new Regex(@"(\|\s*work\s*=\s*)''([^'{}\|]+)''(?=\s*(?:\||}}))");

        private static readonly Regex CiteTemplatePagesPP = new Regex(@"(?<=\|\s*pages?\s*=\s*)p(?:p|gs?)?(?:\.|\b)(?:&nbsp;|\s*)(?=[^{}\|]+(?:\||}}))");
        private static readonly Regex CiteTemplatesJournalVolume = new Regex(@"(?<=\|\s*volume\s*=\s*)vol(?:umes?|\.)?(?:&nbsp;|:)?", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex CiteTemplatesJournalVolumeAndIssue = new Regex(@"(?<=\|\s*volume\s*=\s*[0-9VXMILC]+?)(?:[;,]?\s*(?:nos?[\.:;]?|(?:numbers?|issues?|iss)\s*[:;]?))", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex CiteTemplatesJournalIssue = new Regex(@"(?<=\|\s*issue\s*=\s*)(?:issues?|(?:nos?|iss)(?:[\.,;:]|\b)|numbers?[\.,;:]?)(?:&nbsp;)?", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex CiteTemplatesPageRangeName = new Regex(@"(\|\s*)page(\s*=\s*\d+\s*(?:–|, )\s*\d)");

        private static readonly Regex AccessDateYear = new Regex(@"(?<=\|\s*access\-?date\s*=\s*(?:[1-3]?\d\s+" + WikiRegexes.MonthsNoGroup + @"|\s*" + WikiRegexes.MonthsNoGroup + @"\s+[1-3]?\d))(\s*)\|\s*accessyear\s*=\s*(20[01]\d)\s*(\||}})");
        private static readonly Regex AccessDayMonthDay = new Regex(@"\|\s*access(?:daymonth|month(?:day)?|year)\s*=\s*(?=\||}})");
        private static readonly Regex DateLeadingZero = new Regex(@"(?<=\|\s*(?:access|archive)?\-?date\s*=\s*)(?:0([1-9]\s+" + WikiRegexes.MonthsNoGroup + @")|(\s*" + WikiRegexes.MonthsNoGroup + @"\s)+0([1-9],?))(\s+(?:20[01]|1[89]\d)\d)?(\s*(?:\||}}))");

        private static readonly Regex LangTemplate = new Regex(@"(\|\s*language\s*=\s*)({{(\w{2}) icon}}\s*)(?=\||}})");

        private static readonly Regex UnspacedCommaPageRange = new Regex(@"((?:[ ,–]|^)\d+),(\d+(?:[ ,–]|$))");

        private static readonly List<string> ParametersToDequote = new List<string>(new[] { "title", "trans_title" });
        private static readonly Regex rpTemplate = Tools.NestedTemplateRegex("rp");

        /// <summary>
        /// Applies various formatting fixes to citation templates
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The updated wiki text</returns>
        public static string FixCitationTemplates(string articleText)
        {
            if (!Variables.LangCode.Equals("en"))
                return articleText;

            if(TemplateExists(GetAllTemplates(articleText), WikiRegexes.CiteTemplate))
            {
                foreach (string s in GetAllTemplateDetail(articleText))
                {
                    string res = s, original = "";
                    while(!res.Equals(original))
                    {
                        original = res;
                        res = WikiRegexes.CiteTemplate.Replace(res, FixCitationTemplatesME);
                    }

                    if(!res.Equals(s))
                        articleText = articleText.Replace(s, res);
                }
            }

            if(TemplateExists(GetAllTemplates(articleText), WikiRegexes.HarvTemplate))
                articleText = WikiRegexes.HarvTemplate.Replace(articleText, m =>
                                                           {
                                                               string newValue = FixPageRanges(m.Value, Tools.GetTemplateParameterValues(m.Value));
                                                               string page = Tools.GetTemplateParameterValue(newValue, "p");

                                                               // ignore brackets
                                                               if (page.Contains(@"("))
                                                                   page = page.Substring(0, page.IndexOf(@"("));

                                                               if (Regex.IsMatch(page, @"\d+\s*(?:–|&ndash;|, )\s*\d") && Tools.GetTemplateParameterValue(newValue, "pp").Length == 0)
                                                                   newValue = Tools.RenameTemplateParameter(newValue, "p", "pp");

                                                               return newValue;
                                                           });
            
            if(TemplateExists(GetAllTemplates(articleText), rpTemplate))
                articleText = rpTemplate.Replace(articleText, m =>
                                                                  {
                                                                      string pagerange = Tools.GetTemplateArgument(m.Value, 1);
                                                                      if(pagerange.Length > 0)
                                                                          return m.Value.Replace(pagerange, FixPageRangesValue(pagerange));

                                                                      return m.Value;
                                                                  });

            return articleText;
        }

        private static readonly Regex IdISBN = new Regex(@"^ISBN:?\s*([\d \-]+X?)$", RegexOptions.Compiled);
        private static readonly Regex IdASIN = new Regex(@"^ASIN:?\s*([\d \-]+X?)$", RegexOptions.Compiled);
        private static readonly Regex YearOnly = new Regex(@"^[12]\d{3}$", RegexOptions.Compiled);
        private static readonly Regex ISBNDash = new Regex(@"(\d)[–](\d|X$)");
        private static readonly Regex BalancedArrows = new Regex(@"(?:«([^»]+)»|‹([^›]+)›)");

        /// <summary>
        /// Performs fixes to a given citation template call
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private static string FixCitationTemplatesME(Match m)
        {
            string newValue = Tools.RemoveExcessTemplatePipes(m.Value);
            string templatename = m.Groups[2].Value;
            
            Dictionary<string, string> paramsFound = new Dictionary<string, string>();
            // remove duplicated fields, ensure the URL is not touched (may have pipes in)
            newValue = Tools.RemoveDuplicateTemplateParameters(newValue, paramsFound);

            // fix cite params not in lower case, allowing for ISBN, DOI identifiers being uppercase, avoiding changing text within malformatted URL
            foreach(string notlowercaseCiteParam in paramsFound.Keys.ToList().Where(p => (p.ToLower() != p) && !Regex.IsMatch(p, @"(?:IS[BS]N|DOI|PMID|OCLC|PMC|LCCN|ASIN|ARXIV|ASIN\-TLD|BIBCODE|ID|ISBN13|JFM|JSTOR|MR|OL|OSTI|RFC|SSRN|URL|ZBL)")
                && !CiteUrl.Match(newValue).Value.Contains(p)).ToList())
                newValue = Tools.RenameTemplateParameter(newValue, notlowercaseCiteParam, notlowercaseCiteParam.ToLower());

            string theURL,
                id,
                format,
                theTitle,
                TheYear,
                lang,
                TheDate,
                TheMonth,
                TheWork,
                nopp,
                TheIssue,
                accessyear,
                accessdate,
                pages,
                page,
                ISBN,
                origyear,
                origdate,
                archiveurl,
                contributionurl;
            if(!paramsFound.TryGetValue("url", out theURL))
                theURL = "";
            if(!paramsFound.TryGetValue("id", out id))
                id = "";
            if(!paramsFound.TryGetValue("format", out format))
                format = "";
            if(!paramsFound.TryGetValue("title", out theTitle))
                theTitle = "";
            if(!paramsFound.TryGetValue("year", out TheYear))
                TheYear = "";
            if(!paramsFound.TryGetValue("date", out TheDate))
                TheDate = "";
            if(!paramsFound.TryGetValue("language", out lang))
                lang = "";
            if(!paramsFound.TryGetValue("month", out TheMonth))
                TheMonth = "";
            if(!paramsFound.TryGetValue("work", out TheWork))
                TheWork = "";
            if(!paramsFound.TryGetValue("nopp", out nopp))
                nopp = "";
            if(!paramsFound.TryGetValue("issue", out TheIssue))
                TheIssue = "";
            if(!paramsFound.TryGetValue("accessyear", out accessyear))
                accessyear = "";
            if(!paramsFound.TryGetValue("accessdate", out accessdate) && !paramsFound.TryGetValue("access-date", out accessdate))
                accessdate = "";
            if(!paramsFound.TryGetValue("pages", out pages))
                pages = "";
            if(!paramsFound.TryGetValue("page", out page))
                page = "";
            if(!paramsFound.TryGetValue("origyear", out origyear))
                origyear = "";
            if(!paramsFound.TryGetValue("origdate", out origdate))
                origdate = "";
            if(!paramsFound.TryGetValue("archiveurl", out archiveurl))
                archiveurl = "";
            if(!paramsFound.TryGetValue("contribution-url", out contributionurl))
                contributionurl = "";
            if(!paramsFound.TryGetValue("isbn", out ISBN) && !paramsFound.TryGetValue("ISBN", out ISBN))
                ISBN = "";

            string theURLoriginal = theURL;

            // remove the unneeded 'format=HTML' field
            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Remove_.22format.3DHTML.22_in_citation_templates
            // remove format= field with null value when URL is HTML page
            if(paramsFound.ContainsKey("format"))
            {
                if (format.TrimStart("[]".ToCharArray()).ToUpper().StartsWith("HTM")
                    ||
                    (format.Length == 0 &&
                     theURL.ToUpper().TrimEnd('L').EndsWith("HTM")))
                    newValue = Tools.RemoveTemplateParameter(newValue, "format");
            }

            if(paramsFound.ContainsKey("origdate") && origdate.Length == 0)
            {
                newValue = Tools.RemoveTemplateParameter(newValue, "origdate");
            }

            // newlines to spaces in title field if URL used, otherwise display broken
            if (theTitle.Contains("\r\n") && theURL.Length > 0)
            {
                theTitle = theTitle.Replace("\r\n", " ");
                paramsFound.Remove("title");
                paramsFound.Add("title", theTitle);
                newValue = Tools.UpdateTemplateParameterValue(newValue, "title", theTitle);
            }

            // {{sv icon}} -> sv in language=
            if(lang.Contains("{{"))
            {
                newValue = LangTemplate.Replace(newValue, "$1$3");
                lang = Tools.GetTemplateParameterValue(newValue, "language");
            }

            // remove language=English on en-wiki
            if (lang.Equals("english", StringComparison.OrdinalIgnoreCase) || lang.Equals("en", StringComparison.OrdinalIgnoreCase))
                newValue = Tools.RemoveTemplateParameter(newValue, "language");
            
            // remove italics for work field for book/periodical, but not website -- auto italicised by template
            if (TheWork.Contains("''") && !TheWork.Contains("."))
                newValue = WorkInItalics.Replace(newValue, "$1$2");

            // remove quotes around title field: are automatically added by template markup
            foreach (string dequoteParam in ParametersToDequote)
            {
                string quotetitle;
                if(paramsFound.TryGetValue(dequoteParam, out quotetitle))
                {
                    string before = quotetitle;
                    // convert curly quotes to straight quotes per [[MOS:PUNCT]], but » or › may be section delimeter
                    // so only change those when balanced. Note regular <> characters are not changed.
                    quotetitle = WikiRegexes.CurlyDoubleQuotes.Replace(quotetitle, @"""");
                    quotetitle = BalancedArrows.Replace(quotetitle, @"""$1$2""");

                    if (quotetitle.Contains(@"""") && !quotetitle.Trim('"').Contains(@""""))
                        quotetitle = quotetitle.Trim('"');

                    if(!before.Equals(quotetitle))
                        newValue = Tools.SetTemplateParameterValue(newValue, dequoteParam, quotetitle);
                }
            }

            // page= and pages= fields don't need p. or pp. in them when nopp not set
            if ((pages.Contains("p") || page.Contains("p")) && !templatename.Equals("cite journal", StringComparison.OrdinalIgnoreCase) && nopp.Length == 0)
            {
                newValue = CiteTemplatePagesPP.Replace(newValue, "");
                pages = Tools.GetTemplateParameterValue(newValue, "pages");
                paramsFound.Remove("pages");
                paramsFound.Add("pages", pages);
            }

            // with Lua no need to rename date to year when date = YYYY, just remove year and date duplicating each other
            if (TheDate.Length == 4 && TheYear.Equals(TheDate))
                newValue = Tools.RemoveTemplateParameter(newValue, "date");

            // year = full date --> date = full date
            if (TheYear.Length > 5)
            {
                string TheYearCorected = IncorrectCommaInternationalDates.Replace(TheYear, @"$1 $2");
                TheYearCorected = IncorrectCommaAmericanDates.Replace(TheYearCorected, @"$1 $2, $3");
                
                if(!TheYearCorected.Equals(TheYear))
                {
                    newValue = Tools.UpdateTemplateParameterValue(newValue, "year", TheYearCorected);
                    TheYear = TheYearCorected;
                }
            }
            
            if (TheYear.Length > 5 && (WikiRegexes.ISODates.IsMatch(TheYear) || WikiRegexes.InternationalDates.IsMatch(TheYear)
                                       || WikiRegexes.AmericanDates.IsMatch(TheYear)))
            {
                TheDate = TheYear;
                TheYear = "";
                newValue = Tools.RenameTemplateParameter(newValue, "year", "date");
            }

            // year=YYYY and date=...YYYY -> remove year; not for year=YYYYa
            if (TheYear.Length == 4 && TheDate.Contains(TheYear) && YearOnly.IsMatch(TheYear))
            {
                Parsers p = new Parsers();
                TheDate = p.FixDatesAInternal(TheDate);

                if(WikiRegexes.InternationalDates.IsMatch(TheDate) || WikiRegexes.AmericanDates.IsMatch(TheDate)
                   || WikiRegexes.ISODates.IsMatch(TheDate))
                {
                    TheYear = "";
                    newValue = Tools.RemoveTemplateParameter(newValue, "year");
                }
            }

            // month=Month and date=...Month... OR month=Month and date=same month (by conversion from ISO format)Ors month=nn and date=same month (by conversion to ISO format)
            int num;
            if ((TheMonth.Length > 2 && TheDate.Contains(TheMonth)) // named month within date
                || (TheMonth.Length > 2 && Tools.ConvertDate(TheDate, DateLocale.International).Contains(TheMonth))
                || (int.TryParse(TheMonth, out num) && Regex.IsMatch(Tools.ConvertDate(TheDate, DateLocale.ISO), @"\-0?" + TheMonth + @"\-")))
                newValue = Tools.RemoveTemplateParameter(newValue, "month");

            // date = Month DD and year = YYYY --> date = Month DD, YYYY
            if (!YearOnly.IsMatch(TheDate) && YearOnly.IsMatch(TheYear))
            {
                if (!WikiRegexes.AmericanDates.IsMatch(TheDate) && WikiRegexes.AmericanDates.IsMatch(TheDate + ", " + TheYear))
                {
                    if(!TheDate.Contains(TheYear))
                        newValue = Tools.SetTemplateParameterValue(newValue, "date", TheDate + ", " + TheYear);
                    newValue = Tools.RemoveTemplateParameter(newValue, "year");
                }
                else if (!WikiRegexes.InternationalDates.IsMatch(TheDate) && WikiRegexes.InternationalDates.IsMatch(TheDate + " " + TheYear))
                {
                    if(!TheDate.Contains(TheYear))
                        newValue = Tools.SetTemplateParameterValue(newValue, "date", TheDate + " " + TheYear);
                    newValue = Tools.RemoveTemplateParameter(newValue, "year");
                }
            }

            // correct volume=vol 7... and issue=no. 8 for {{cite journal}} only
            if (templatename.Equals("cite journal", StringComparison.OrdinalIgnoreCase))
            {
                newValue = CiteTemplatesJournalVolume.Replace(newValue, "");
                newValue = CiteTemplatesJournalIssue.Replace(newValue, "");

                if (TheIssue.Length == 0)
                    newValue = CiteTemplatesJournalVolumeAndIssue.Replace(newValue, @"| issue = ");
            }

            // {{cite web}} for Google books -> {{Cite book}}
            if (templatename.Contains("web") && newValue.Contains("http://books.google.") && TheWork.Length == 0)
                newValue = Tools.RenameTemplate(newValue, templatename, "Cite book");

            // remove leading zero in day of month
            if(Regex.IsMatch(newValue, @"\b0[1-9]") && DateLeadingZero.IsMatch(newValue))
            {
                newValue = DateLeadingZero.Replace(newValue, @"$1$2$3$4$5");
                newValue = DateLeadingZero.Replace(newValue, @"$1$2$3$4$5");
                TheDate = Tools.GetTemplateParameterValue(newValue, "date");
                accessdate = Tools.GetTemplateParameterValue(newValue, "accessdate");
            }

            if(paramsFound.Where(s => s.Key.Contains("access") && !s.Key.Contains("date")).Count() > 0)
            {
                if(Regex.IsMatch(templatename, @"[Cc]ite(?: ?web| book| news)"))
                {
                    // remove any empty accessdaymonth, accessmonthday, accessmonth and accessyear
                    newValue = AccessDayMonthDay.Replace(newValue, "");

                    // merge accessdate of 'D Month' or 'Month D' and accessyear of 'YYYY' in cite web
                    if(accessyear.Length == 4)
                        newValue = AccessDateYear.Replace(newValue, @" $2$1$3");
                }

                // remove accessyear where accessdate is present and contains said year
                if (accessyear.Length > 0 && accessdate.Contains(accessyear))
                    newValue = Tools.RemoveTemplateParameter(newValue, "accessyear");
            }

            // fix unspaced comma ranges, avoid pages=12,345 as could be valid page number
            if (pages.Contains(",") && Regex.Matches(pages, @"\b\d{1,2},\d{3}\b").Count == 0)
            {
                while (UnspacedCommaPageRange.IsMatch(pages))
                {
                    pages = UnspacedCommaPageRange.Replace(pages, "$1, $2");
                }
                newValue = Tools.UpdateTemplateParameterValue(newValue, "pages", pages);
                paramsFound.Remove("pages");
                paramsFound.Add("pages", pages);
            }

            // page range should have unspaced en-dash; validate that page is range not section link
            newValue = FixPageRanges(newValue, paramsFound);

            // page range or list should use 'pages' parameter not 'page'
            if (CiteTemplatesPageRangeName.IsMatch(newValue))
            {
                newValue = CiteTemplatesPageRangeName.Replace(newValue, @"$1pages$2");
                newValue = Tools.RemoveDuplicateTemplateParameters(newValue);
            }

            // remove ordinals from dates
            if(Ordinal.IsMatch(TheDate) || Ordinal.IsMatch(accessdate))
            {
                if (OrdinalsInDatesInt.IsMatch(TheDate))
                    newValue = Tools.UpdateTemplateParameterValue(newValue, "date", OrdinalsInDatesInt.Replace(TheDate, "$1$2$3 $4"));
                else if (OrdinalsInDatesAm.IsMatch(TheDate))
                    newValue = Tools.UpdateTemplateParameterValue(newValue, "date", OrdinalsInDatesAm.Replace(TheDate, "$1 $2$3"));

                if (OrdinalsInDatesInt.IsMatch(accessdate))
                    newValue = Tools.UpdateTemplateParameterValue(newValue, "accessdate", OrdinalsInDatesInt.Replace(accessdate, "$1$2$3 $4"));
                else if(OrdinalsInDatesAm.IsMatch(accessdate))
                    newValue = Tools.UpdateTemplateParameterValue(newValue, "accessdate", OrdinalsInDatesAm.Replace(accessdate, "$1 $2$3"));
            }
            // catch after any other fixes
            newValue = IncorrectCommaAmericanDates.Replace(newValue, @"$1 $2, $3");

            // URL starting www needs http://
            if (theURL.StartsWith("www", StringComparison.OrdinalIgnoreCase))
                theURL = "http://" + theURL;

            if(archiveurl.StartsWith("www", StringComparison.OrdinalIgnoreCase))
                newValue = Tools.UpdateTemplateParameterValue(newValue, "archiveurl", "http://" + archiveurl);
            if(contributionurl.StartsWith("www", StringComparison.OrdinalIgnoreCase))
                newValue = Tools.UpdateTemplateParameterValue(newValue, "contribution-url", "http://" + contributionurl);

            // (part) wikilinked/external linked URL in cite template, don't change when named external link format
            if(!theURL.Contains(" "))
                theURL = theURL.Trim('[').Trim(']');

            if(!theURLoriginal.Equals(theURL))
                newValue = Tools.UpdateTemplateParameterValue(newValue, "url", theURL);

            // {{dead link}} should be placed outside citation, not in format field per [[Template:Dead link]]
            if (WikiRegexes.DeadLink.IsMatch(format))
            {
                string deadLink = WikiRegexes.DeadLink.Match(format).Value;

                if (theURL.ToUpper().TrimEnd('L').EndsWith("HTM") && format.Equals(deadLink))
                    newValue = Tools.RemoveTemplateParameter(newValue, "format");
                else
                    newValue = Tools.UpdateTemplateParameterValue(newValue, "format", format.Replace(deadLink, ""));

                newValue += (" " + deadLink);
            }

            //id=ISBN fix
            if (IdISBN.IsMatch(id) && ISBN.Length == 0)
            {
                newValue = Tools.RenameTemplateParameter(newValue, "id", "isbn");
                newValue = Tools.SetTemplateParameterValue(newValue, "isbn", IdISBN.Match(id).Groups[1].Value.Trim());
            }

            //id=ASIN fix
            if (IdASIN.IsMatch(id) && Tools.GetTemplateParameterValue(newValue, "asin").Length == 0 && Tools.GetTemplateParameterValue(newValue, "ASIN").Length == 0)
            {
                newValue = Tools.RenameTemplateParameter(newValue, "id", "asin");
                newValue = Tools.SetTemplateParameterValue(newValue, "asin", IdASIN.Match(id).Groups[1].Value.Trim());
            }

            if(ISBN.Length > 0)
            {
                string ISBNbefore = ISBN;
                // remove ISBN at start, but not if multiple ISBN
                if(ISBN.IndexOf("isbn", StringComparison.OrdinalIgnoreCase) > -1
                   && ISBN.Substring(4).IndexOf("isbn", StringComparison.OrdinalIgnoreCase) == -1)
                    ISBN = Regex.Replace(ISBN, @"^(?i)ISBN\s*", "");

                // trim unneeded characters
                ISBN = ISBN.Trim(".;,:".ToCharArray()).Trim();

                // fix dashes: only hyphens allowed
                while(ISBNDash.IsMatch(ISBN))
                    ISBN = ISBNDash.Replace(ISBN, @"$1-$2");
                ISBN = ISBN.Replace('\x2010', '-');
                ISBN = ISBN.Replace('\x2012', '-');

                if(!ISBN.Equals(ISBNbefore))
                {
                    if(paramsFound.ContainsKey("ISBN"))
                        newValue = Tools.UpdateTemplateParameterValue(newValue, "ISBN", ISBN);
                    else
                        newValue = Tools.UpdateTemplateParameterValue(newValue, "isbn", ISBN);
                }
            }

            // origyear --> year when no year/date
            if (TheYear.Length == 0 && TheDate.Length == 0 && origyear.Length == 4)
            {
                newValue = Tools.RenameTemplateParameter(newValue, "origyear", "year");
                newValue = Tools.RemoveDuplicateTemplateParameters(newValue);
            }

            return newValue;
        }

        #endregion

        #region PageRanges
        private static readonly List<string> PageFields = new List<string>(new[] { "page", "pages", "p", "pp" });
        private static readonly Regex PageRange = new Regex(@"\b(\d+)\s*[-—]+\s*(\d+)", RegexOptions.Compiled);
        private static readonly Regex SpacedPageRange = new Regex(@"(\d+) +(–|&ndash;) +(\d)", RegexOptions.Compiled);

        /// <summary>
        /// Converts hyphens in page ranges in citation template fields to endashes
        /// </summary>
        /// <param name="templateCall">The template call</param>
        /// <param name="Params">Dictionary of template parameters in template call</param>
        /// <returns>The updated template call</returns>
        private static string FixPageRanges(string templateCall, Dictionary<string, string> Params)
        {
            foreach (string pageField in PageFields)
            {
                string pageRange;
                if(Params.TryGetValue(pageField, out pageRange) && pageRange.Length > 0)
                {
                    string res = FixPageRangesValue(pageRange);
                    if(!res.Equals(pageRange))
                        templateCall = Tools.UpdateTemplateParameterValue(templateCall, pageField, res);
                }
            }

            return templateCall;
        }

        private static string FixPageRangesValue(string pageRange)
        {
            string original = pageRange;
            // fix spaced page ranges e.g. 15 – 20 --> 15–20
            if (SpacedPageRange.IsMatch(pageRange))
                return SpacedPageRange.Replace(pageRange, "$1$2$3");

            if (pageRange.Length > 2 && !pageRange.Contains(" to "))
            {
                bool pagerangesokay = true;
                Dictionary<int, int> PageRanges = new Dictionary<int, int>();

                foreach (Match pagerange in PageRange.Matches(pageRange))
                {
                    string page1 = pagerange.Groups[1].Value;
                    string page2 = pagerange.Groups[2].Value;

                    // convert 350-2 into 350-352 etc.
                    if (page1.Length > page2.Length)
                        page2 = page1.Substring(0, page1.Length - page2.Length) + page2;

                    // check a valid range with difference < 999
                    pagerangesokay = (Convert.ToInt32(page1) < Convert.ToInt32(page2) &&
                                      Convert.ToInt32(page2) - Convert.ToInt32(page1) < 999);

                    // check range doesn't overlap with another range found
                    foreach (KeyValuePair<int, int> kvp in PageRanges)
                    {
                        // check if page 1 or page 2 within existing range
                        if ((Convert.ToInt32(page1) >= kvp.Key && Convert.ToInt32(page1) <= kvp.Value) || (Convert.ToInt32(page2) >= kvp.Key && Convert.ToInt32(page2) <= kvp.Value))
                        {
                            pagerangesokay = false;
                            break;
                        }
                    }

                    if (!pagerangesokay)
                        break;

                    // add to dictionary of ranges found
                    PageRanges.Add(Convert.ToInt32(page1), Convert.ToInt32(page2));
                }

                if (pagerangesokay)
                    return PageRange.Replace(pageRange, @"$1–$2");
            }
            
            return original;
        }
        #endregion

        #region CitationPublisherToWork
        private static readonly Regex CiteWebOrNews = Tools.NestedTemplateRegex(new[] { "cite web", "citeweb", "cite news", "citenews" });
        private static readonly Regex PressPublishers = new Regex(@"(Associated Press|United Press International)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly List<string> WorkParameterAndAliases = new List<string>(new[] { "work", "newspaper", "journal", "periodical", "magazine" });

        /// <summary>
        /// Where the publisher field is used incorrectly instead of the work field in a {{cite web}} or {{cite news}} citation
        /// convert the parameter to be 'work'
        /// Scenarios covered:
        /// * publisher == URL domain, no work= used
        /// </summary>
        /// <param name="citation">the citation</param>
        /// <returns>the updated citation</returns>
        public static string CitationPublisherToWork(string citation)
        {
            // only for {{cite web}} or {{cite news}}
            if (!CiteWebOrNews.IsMatch(citation))
                return citation;

            string publisher = Tools.GetTemplateParameterValue(citation, "publisher");

            // nothing to do if no publisher, or publisher is a press publisher
            if (publisher.Length == 0 | PressPublishers.IsMatch(publisher))
                return citation;

            List<string> workandaliases = Tools.GetTemplateParametersValues(citation, WorkParameterAndAliases);

            if (string.Join("", workandaliases.ToArray()).Length == 0)
            {
                citation = Tools.RenameTemplateParameter(citation, "publisher", "work");
                citation = WorkInItalics.Replace(citation, "$1$2");
            }

            return citation;
        }
        #endregion

        #region CiteTemplateDates
        /// <summary>
        /// Corrects common formatting errors in dates in external reference citation templates (doesn't link/delink dates)
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="noChange"></param>
        /// <returns>The modified article text.</returns>
        public string CiteTemplateDates(string articleText, out bool noChange)
        {
            string newText = CiteTemplateDates(articleText);

            noChange = newText.Equals(articleText);

            return newText;
        }

        private const string SiCitStart = @"(?si)(\|\s*";
        private const string CitAccessdate = SiCitStart + @"(?:access|archive)\-?date\s*=\s*";
        private const string CitDate = SiCitStart + @"(?:archive|air)?date2?\s*=\s*";

        private static readonly RegexReplacement[] CiteTemplateIncorrectISOAccessdates = {
            new RegexReplacement(CitAccessdate + @")(1[0-2])[/_\-\.]?(1[3-9])[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-$2-$3"),
            new RegexReplacement(CitAccessdate + @")(1[0-2])[/_\-\.]?([23]\d)[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-$2-$3"),
            new RegexReplacement(CitAccessdate + @")(1[0-2])[/_\-\.]?\2[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$3-$2-$2"), // nn-nn-2004 and nn-nn-04 to ISO format (both nn the same)
            new RegexReplacement(CitAccessdate + @")(1[3-9])[/_\-\.]?(1[0-2])[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-$3-$2"),
            new RegexReplacement(CitAccessdate + @")(1[3-9])[/_\-\.]?0?([1-9])[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-0$3-$2"),
            new RegexReplacement(CitAccessdate + @")(20[01]\d)0?([01]\d)[/_\-\.]([0-3]\d\s*(?:\||}}))", "$1$2-$3-$4"),
            new RegexReplacement(CitAccessdate + @")(20[01]\d)[/_\-\.]([01]\d)0?([0-3]\d\s*(?:\||}}))", "$1$2-$3-$4"),
            new RegexReplacement(CitAccessdate + @")(20[01]\d)[/_\-\.]?([01]\d)[/_\-\.]?([1-9]\s*(?:\||}}))", "$1$2-$3-0$4"),
            new RegexReplacement(CitAccessdate + @")(20[01]\d)[/_\-\.]?([1-9])[/_\-\.]?([0-3]\d\s*(?:\||}}))", "$1$2-0$3-$4"),
            new RegexReplacement(CitAccessdate + @")(20[01]\d)[/_\-\.]?([1-9])[/_\-\.]0?([1-9]\s*(?:\||}}))", "$1$2-0$3-0$4"),
            new RegexReplacement(CitAccessdate + @")(20[01]\d)[/_\-\.]0?([1-9])[/_\-\.]([1-9]\s*(?:\||}}))", "$1$2-0$3-0$4"),
            new RegexReplacement(CitAccessdate + @")(20[01]\d)[/_\.]?([01]\d)[/_\.]?([0-3]\d\s*(?:\||}}))", "$1$2-$3-$4"),

            new RegexReplacement(CitAccessdate + @")([23]\d)[/_\-\.](1[0-2])[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-$3-$2"),
            new RegexReplacement(CitAccessdate + @")([23]\d)[/_\-\.]0?([1-9])[/_\-\.](?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-0$3-$2"),
            new RegexReplacement(CitAccessdate + @")0?([1-9])[/_\-\.]?(1[3-9]|[23]\d)[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-0$2-$3"),
            new RegexReplacement(CitAccessdate + @")0?([1-9])[/_\-\.]?0?\2[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$3-0$2-0$2") // n-n-2004 and n-n-04 to ISO format (both n the same)
        };

        private static readonly RegexReplacement[] CiteTemplateIncorrectISODates = {
            new RegexReplacement(CitDate + @"\[?\[?)(20\d\d|19[7-9]\d)[/_]?([01]\d)[/_]?([0-3]\d\s*(?:\||}}))", "$1$2-$3-$4"),
            new RegexReplacement(CitDate + @"\[?\[?)(1[0-2])[/_\-\.]?([23]\d)[/_\-\.]?(19[7-9]\d)(?=\s*(?:\||}}))", "$1$4-$2-$3"),
            new RegexReplacement(CitDate + @"\[?\[?)0?([1-9])[/_\-\.]?([23]\d)[/_\-\.]?(19[7-9]\d)(?=\s*(?:\||}}))", "$1$4-0$2-$3"),
            new RegexReplacement(CitDate + @"\[?\[?)([23]\d)[/_\-\.]?0?([1-9])[/_\-\.]?(19[7-9]\d)(?=\s*(?:\||}}))", "$1$4-0$3-$2"),
            new RegexReplacement(CitDate + @"\[?\[?)([23]\d)[/_\-\.]?(1[0-2])[/_\-\.]?(19[7-9]\d)(?=\s*(?:\||}}))", "$1$4-$3-$2"),
            new RegexReplacement(CitDate + @"\[?\[?)(1[0-2])[/_\-\.]([23]\d)[/_\-\.](?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-$2-$3"),
            new RegexReplacement(CitDate + @"\[?\[?)0?([1-9])[/_\-\.]([23]\d)[/_\-\.](?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-0$2-$3"),
            new RegexReplacement(CitDate + @"\[?\[?)([23]\d)[/_\-\.]0?([1-9])[/_\-\.](?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-0$3-$2"),
            new RegexReplacement(CitDate + @"\[?\[?)([23]\d)[/_\-\.](1[0-2])[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-$3-$2"),
            new RegexReplacement(CitDate + @"\[?\[?)(1[0-2])[/_\-\.]?(1[3-9])[/_\-\.]?(19[7-9]\d)(?=\s*(?:\||}}))", "$1$4-$2-$3"),
            new RegexReplacement(CitDate + @"\[?\[?)0?([1-9])[/_\-\.](1[3-9])[/_\-\.](19[7-9]\d|20\d\d)(?=\s*(?:\||}}))", "$1$4-0$2-$3"),
            new RegexReplacement(CitDate + @"\[?\[?)(1[3-9])[/_\-\.]?0?([1-9])[/_\-\.]?(19[7-9]\d)(?=\s*(?:\||}}))", "$1$4-0$3-$2"),
            new RegexReplacement(CitDate + @"\[?\[?)(1[3-9])[/_\-\.]?(1[0-2])[/_\-\.]?(19[7-9]\d)(?=\s*(?:\||}}))", "$1$4-$3-$2"),
            new RegexReplacement(CitDate + @"\[?\[?)(1[0-2])[/_\-\.](1[3-9])[/_\-\.](?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-$2-$3"),
            new RegexReplacement(CitDate + @"\[?\[?)([1-9])[/_\-\.](1[3-9])[/_\-\.](?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-0$2-$3"),
            new RegexReplacement(CitDate + @"\[?\[?)(1[3-9])[/_\-\.]([1-9])[/_\-\.](?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-0$3-$2"),
            new RegexReplacement(CitDate + @"\[?\[?)(1[3-9])[/_\-\.](1[0-2])[/_\-\.](?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-$3-$2"),
            new RegexReplacement(CitDate + @")0?([1-9])[/_\-\.]0?\2[/_\-\.](20\d\d|19[7-9]\d)(?=\s*(?:\||}}))", "$1$3-0$2-0$2"), // n-n-2004 and n-n-1980 to ISO format (both n the same)
            new RegexReplacement(CitDate + @")0?([1-9])[/_\-\.]0?\2[/_\-\.]([01]\d)(?=\s*(?:\||}}))", "${1}20$3-0$2-0$2"), // n-n-04 to ISO format (both n the same)
            new RegexReplacement(CitDate + @")(1[0-2])[/_\-\.]\2[/_\-\.]?(20\d\d|19[7-9]\d)(?=\s*(?:\||}}))", "$1$3-$2-$2"), // nn-nn-2004 and nn-nn-1980 to ISO format (both nn the same)
            new RegexReplacement(CitDate + @")(1[0-2])[/_\-\.]\2[/_\-\.]([01]\d)(?=\s*(?:\||}}))", "${1}20$3-$2-$2"), // nn-nn-04 to ISO format (both nn the same)
            new RegexReplacement(CitDate + @")((?:\[\[)?20\d\d|1[5-9]\d{2})[/_\-\.]([1-9])[/_\-\.]0?([1-9](?:\]\])?\s*(?:\||}}))", "$1$2-0$3-0$4"),
            new RegexReplacement(CitDate + @")((?:\[\[)?20\d\d|1[5-9]\d{2})[/_\-\.]0?([1-9])[/_\-\.]([1-9](?:\]\])?\s*(?:\||}}))", "$1$2-0$3-0$4"),
            new RegexReplacement(CitDate + @")((?:\[\[)?20\d\d|1[5-9]\d{2})[/_\-\.]?([01]\d)[/_\-\.]?([1-9](?:\]\])?\s*(?:\||}}))", "$1$2-$3-0$4"),
            new RegexReplacement(CitDate + @")((?:\[\[)?20\d\d|1[5-9]\d{2})[/_\-\.]?([1-9])[/_\-\.]?([0-3]\d(?:\]\])?\s*(?:\||}}))", "$1$2-0$3-$4"),
            new RegexReplacement(CitDate + @")((?:\[\[)?20\d\d|1[5-9]\d{2})([01]\d)[/_\-\.]([0-3]\d(?:\]\])?\s*(?:\||}}))", "$1$2-$3-$4"),
            new RegexReplacement(CitDate + @")((?:\[\[)?20\d\d|1[5-9]\d{2})[/_\-\.](0[1-9]|1[0-2])0?([0-3]\d(?:\]\])?\s*(?:\||}}))", "$1$2-$3-$4")
        };

        private static readonly Regex CiteTemplateAbbreviatedMonthISO = new Regex(@"(?si)(\|\s*(?:archive|air|access)?date2?\s*=\s*)(\d{4}[-/\s][A-Z][a-z]+\.?[-/\s][0-3]?\d)(\s*(?:\||}}))", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex CiteTemplateDateYYYYDDMMFormat = new Regex(SiCitStart + @"(?:archive|air|access)?date2?\s*=\s*(?:\[\[)?20\d\d)-([23]\d|1[3-9])-(0[1-9]|1[0-2])(\]\])?");
        private static readonly Regex CiteTemplateTimeInDateParameter = new Regex(@"(\|\s*(?:archive|air|access)?date2?\s*=\s*(?:(?:20\d\d|19[7-9]\d)-[01]?\d-[0-3]?\d|[0-3]?\d[a-z]{0,2}\s*\w+,?\s*(?:20\d\d|19[7-9]\d)|\w+\s*[0-3]?\d[a-z]{0,2},?\s*(?:20\d\d|19[7-9]\d)))(\s*[,-:]?\s+[0-2]?\d[:\.]?[0-5]\d(?:\:?[0-5]\d)?\s*(?:[^\|\}]*\[\[[^[\]\n]+(?<!\[\[[A-Z]?[a-z-]{2,}:[^[\]\n]+)\]\][^\|\}]*|[^\|\}]*)?)(?<!.*(?:20|1[7-9])\d+\s*)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private static readonly Regex WhitespaceEnd = new Regex(@"(\s+)$");
        private static readonly Regex CitePodcast = Tools.NestedTemplateRegex("cite podcast");

        /// <summary>
        /// Corrects common formatting errors in dates in external reference citation templates (doesn't link/delink dates)
        /// note some incorrect date formats such as 3-2-2009 are ambiguous as could be 3-FEB-2009 or MAR-2-2009, these fixes don't address such errors
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string CiteTemplateDates(string articleText)
        {
            // cite podcast is non-compliant to citation core standards
            if (!Variables.IsWikipediaEN || TemplateExists(GetAllTemplates(articleText), CitePodcast))
                return articleText;

            string originalArticleText = articleText;

            // loop in case a single citation has multiple dates to be fixed
            foreach (string s in GetAllTemplateDetail(articleText))
            {
                string res = s, original = "";
                while(!res.Equals(original))
                {
                    original = res;
                    res = WikiRegexes.CiteTemplate.Replace(res, CiteTemplateME);
                }

                if(!res.Equals(s))
                    articleText = articleText.Replace(s, res);
            }

            // don't apply fixes when ambiguous dates present, for performance only appply this check if changes made
            if(!originalArticleText.Equals(articleText) && AmbiguousCiteTemplateDates(originalArticleText))
                return originalArticleText;

            return articleText;
        }

        /// <summary>
        /// convert invalid date formats like DD-MM-YYYY, MM-DD-YYYY, YYYY-D-M, YYYY-DD-MM, YYYY_MM_DD etc. to iso format of YYYY-MM-DD
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private static string CiteTemplateME(Match m)
        {
            string newValue = m.Value;

            Dictionary<string, string> paramsFound = Tools.GetTemplateParameterValues(newValue);            

            string accessdate, date, date2, archivedate, airdate, journal;
            if(!paramsFound.TryGetValue("accessdate", out accessdate) && !paramsFound.TryGetValue("access-date", out accessdate))
                accessdate = "";
            if(!paramsFound.TryGetValue("date", out date))
                date = "";
            if(!paramsFound.TryGetValue("date2", out date2))
                date2 = "";
            if(!paramsFound.TryGetValue("archivedate", out archivedate))
                archivedate = "";
            if(!paramsFound.TryGetValue("airdate", out airdate))
                airdate = "";
            if(!paramsFound.TryGetValue("journal", out journal))
                journal = "";

            List<string> dates = new List<string> {accessdate, archivedate, date, date2, airdate};

            if(CiteTemplateMEParameterToProcess(dates))
            {
                // accessdate=, archivedate=
                newValue = CiteTemplateIncorrectISOAccessdates.Aggregate(newValue, (current, rr) => rr.Regex.Replace(current, rr.Replacement));

                // date=, archivedate=, airdate=, date2=
                newValue = CiteTemplateIncorrectISODates.Aggregate(newValue, (current, rr) => rr.Regex.Replace(current, rr.Replacement));

                newValue = CiteTemplateDateYYYYDDMMFormat.Replace(newValue, "$1-$3-$2$4"); // YYYY-DD-MM to YYYY-MM-DD

                // date = YYYY-Month-DD fix, not for cite journal PubMed date format
                if(journal.Length == 0)
                    newValue = CiteTemplateAbbreviatedMonthISO.Replace(newValue, m2 => m2.Groups[1].Value + Tools.ConvertDate(m2.Groups[2].Value.Replace(".", ""), DateLocale.ISO) + m2.Groups[3].Value);
            }
            // all citation dates: Remove time from date fields
            newValue = CiteTemplateTimeInDateParameter.Replace(newValue, m3 => {
                                                                   // keep end whitespace outside comment
                                                                   string comm = m3.Groups[2].Value, whitespace = "";

                                                                   Match whm = WhitespaceEnd.Match(comm);

                                                                   if(whm.Success)
                                                                   {
                                                                       comm = comm.TrimEnd();
                                                                       whitespace = whm.Groups[1].Value;
                                                                   }

                                                                   return m3.Groups[1].Value + "<!--" + comm + @"-->" + whitespace;
                                                               });

            return newValue;
        }

        private static bool CiteTemplateMEParameterToProcess(List<string> parameters)
        {
            foreach(string s in parameters)
            {
                if(s.Length > 4 && !WikiRegexes.ISODates.IsMatch(s)
                   && !Regex.IsMatch(s, @"^(\d{1,2} *)?" + WikiRegexes.MonthsNoGroup))
                    return true;
            }
            return false;
        }

        private static readonly Regex PossibleAmbiguousCiteDate = new Regex(@"(?<=\|\s*(?:access|archive|air)?date2?\s*=\s*)(0?[1-9]|1[0-2])[/_\-\.](0?[1-9]|1[0-2])[/_\-\.](20\d\d|19[7-9]\d|[01]\d)\b");
        private static readonly Regex PossibleAmbiguousCiteDateQuick = new Regex(@"(\|\s*(?:access|archive|air)?date2?\s*=\s*)(0?[1-9]|1[0-2])[/_\-\.](0?[1-9]|1[0-2])[/_\-\.](20\d\d|19[7-9]\d|[01]\d)\b");

        /// <summary>
        /// Returns whether the input article text contains ambiguous cite template dates in XX-XX-YYYY or XX-XX-YY format
        /// </summary>
        /// <param name="articleText">the article text to search</param>
        /// <returns>If any matches were found</returns>
        public static bool AmbiguousCiteTemplateDates(string articleText)
        {
            return AmbigCiteTemplateDates(articleText).Count > 0;
        }

        /// <summary>
        /// Returns whether the input article text contains ambiguous cite template dates in XX-XX-YYYY or XX-XX-YY format
        /// </summary>
        /// <param name="articleText">The article text to search</param>
        /// <returns>A dictionary of matches (index and length)</returns>
        public static Dictionary<int, int> AmbigCiteTemplateDates(string articleText)
        {
            Dictionary<int, int> ambigDates = new Dictionary<int, int>();
            if(PossibleAmbiguousCiteDateQuick.IsMatch(articleText)) // check for performance
            {
                foreach(Match m in WikiRegexes.CiteTemplate.Matches(articleText))
                {
                    foreach (Match m2 in PossibleAmbiguousCiteDate.Matches(m.Value))
                    {
                        // for YYYY-AA-BB date, ambiguous if AA and BB not the same
                        if (!m2.Groups[1].Value.Equals(m2.Groups[2].Value))
                            ambigDates.Add(m.Index+m2.Index, m2.Length);
                    }
                }
            }

            return ambigDates;
        }
        #endregion

        private static readonly Regex CiteArXiv = Tools.NestedTemplateRegex(new[] { "cite arxiv", "cite arXiv" });
        private static readonly Regex CitationPopulatedParameter = new Regex(@"\|\s*([\w_\d- ']+)\s*=\s*([^\|}]+)");
        private static readonly Regex citeWebParameters = new Regex(@"^(access-?date|agency|archive-?date|archive-?url|arxiv|ARXIV|asin|ASIN|asin-tld|ASIN-TLD|at|[Aa]uthor\d*|author\d*-first|author-?format|author\d*-last|author-?link\d*|author\d*-?link|authors|author-mask|author-name-separator|author-separator|bibcode|BIBCODE|date|dead-?url|dictionary|display-?authors|display-?editors|doi|DOI|DoiBroken|doi-broken|doi-broken-date|doi_brokendate|doi-inactive-date|doi_inactivedate|edition|[Ee]ditor|editor\d*|editor\d*-first|editor-?format|EditorGiven\d*|editor\d*-given|editor\d*-last|editor\d*-?link|editor-?mask|editor-name-separator|EditorSurname\d*|editor\d*-surname|editor-first\d*|editor-given\d*|editor-last\d*|editor-surname\d*|editorlink\d*|editors|[Ee]mbargo|encyclopa?edia|first\d*|format|given\d*|id|ID|ignoreisbnerror|ignore-isbn-error|institution|isbn|ISBN|isbn13|ISBN13|issn|ISSN|issue|jfm|JFM|journal|jstor|JSTOR|language|last\d*|lastauthoramp|last-author-amp|lay-?date|lay-?source|lay-?summary|lay-?url|lccn|LCCN|location|magazine|mode|mr|MR|newspaper|no-?pp|number|oclc|OCLC|ol|OL|orig-?year|others|osti|pp?|pages?|people|periodical|place|pmc|PMC|pmid|PMID|postscript|publication-?(?:place|date)|publisher|quotation|quote|[Rr]ef|registration|rfc|RFC|script\-title|separator|series|series-?link|ssrn|SSRN|subscription|surname\d*|title|trans[_-]title|type|url|URL|version|via|volume|website|work|year|zbl|ZBL)\b", RegexOptions.Compiled);
        private static readonly Regex citeArXivParameters = new Regex(@"\b(arxiv|asin|ASIN|author\d*|authorlink\d*|author\d*-link|bibcode|class|coauthors?|date|day|doi|DOI|doi brokendate|doi inactivedate|eprint|first\d*|format|given\d*|id|in|isbn|ISBN|issn|ISSN|jfm|JFM|jstor|JSTOR|language|last\d*|laydate|laysource|laysummary|lccn|LCCN|month|mr|MR|oclc|OCLC|ol|OL|osti|OSTI|pmc|PMC|pmid|PMID|postscript|publication-date|quote|ref|rfc|RFC|separator|seperator|ssrn|SSRN|surname\d*|title|version|year|zbl)\b", RegexOptions.Compiled);
        private static readonly Regex NoEqualsTwoBars = new Regex(@"\|[^=\|]+\|");

        /// <summary>
        /// Searches for unknown/invalid parameters within citation templates
        /// </summary>
        /// <param name="articleText">the wiki text to search</param>
        /// <returns>Dictionary of parameter index in wiki text, and parameter length</returns>
        public static Dictionary<int, int> BadCiteParameters(string articleText)
        {
            Dictionary<int, int> found = new Dictionary<int, int>();

            // unknown parameters in cite arXiv
            foreach (Match m in CiteArXiv.Matches(articleText))
            {
                // ignore parameters in templates within cite
                string cite = @"{{" + Tools.ReplaceWithSpaces(m.Value.Substring(2), WikiRegexes.NestedTemplates.Matches(m.Value.Substring(2)));

                foreach (Match m2 in CitationPopulatedParameter.Matches(cite))
                {
                    if (!citeArXivParameters.IsMatch(m2.Groups[1].Value) && Tools.GetTemplateParameterValue(cite, m2.Groups[1].Value).Length > 0)
                        found.Add(m.Index + m2.Groups[1].Index, m2.Groups[1].Length);
                }
            }

            foreach (Match m in WikiRegexes.CiteTemplate.Matches(articleText))
            {
                // unknown parameters in cite web
                if(m.Groups[2].Value.EndsWith("web"))
                {
                    // ignore parameters in templates within cite
                    string cite = @"{{" + Tools.ReplaceWithSpaces(m.Value.Substring(2), WikiRegexes.NestedTemplates.Matches(m.Value.Substring(2)));

                    foreach (Match m2 in CitationPopulatedParameter.Matches(cite))
                    {
                        if (!citeWebParameters.IsMatch(m2.Groups[1].Value) && Tools.GetTemplateParameterValue(cite, m2.Groups[1].Value).Length > 0)
                            found.Add(m.Index + m2.Groups[1].Index, m2.Groups[1].Length);
                    }
                }

                string pipecleaned = Tools.PipeCleanedTemplate(m.Value, false);

                // no equals between two separator pipes
                if (pipecleaned.Contains("="))
                {
                    Match m2 = NoEqualsTwoBars.Match(pipecleaned);

                    if (m2.Success)
                        found.Add(m.Index + m2.Index, m2.Length);
                }

                // URL has space in it
                int urlpos = m.Value.IndexOf("url");
                if(urlpos > 0)
                {
                    string URL = Tools.GetTemplateParameterValue(m.Value, "url");
                    if (URL.Contains(" ") && WikiRegexes.UnformattedText.Replace(WikiRegexes.NestedTemplates.Replace(URL, ""), "").Trim().Contains(" "))
                    {
                        string fromURL = m.Value.Substring(urlpos); // value of url may be in another earlier parameter, report correct position
                        found.Add(m.Index + urlpos + fromURL.IndexOf(URL), URL.Length);
                    }
                }
            }

            return found;
        }
    }
}
