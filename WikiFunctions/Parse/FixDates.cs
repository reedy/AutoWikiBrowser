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

        // don't match on 'in the June of 2007', 'on the 11th May 2008' etc. as these won't read well if changed
        private static readonly Regex OfBetweenMonthAndYear = new Regex(@"\b" + WikiRegexes.Months + @" +of +(20\d\d|1[89]\d\d)\b(?<!\b[Tt]he {1,5}\w{3,15} {1,5}of {1,5}(20\d\d|1[89]\d\d))");
        private static readonly Regex OrdinalsInDatesAm = new Regex(@"(?<!\b[1-3]\d +)\b" + WikiRegexes.Months + @" +([0-3]?\d)(?:st|nd|rd|th)\b(?<!\b[Tt]he +\w{3,10} +(?:[0-3]?\d)(?:st|nd|rd|th)\b)(?:( *(?:to|and|.|&.dash;) *[0-3]?\d)(?:st|nd|rd|th)\b)?");
        private static readonly Regex OrdinalsInDatesInt = new Regex(@"(?:\b([0-3]?\d)(?:st|nd|rd|th)( *(?:to|and|.|&.dash;) *))?\b([0-3]?\d)(?:st|nd|rd|th) +" + WikiRegexes.Months + @"\b(?<!\b[Tt]he +(?:[0-3]?\d)(?:st|nd|rd|th) +\w{3,10})");
        private static readonly Regex DateLeadingZerosAm = new Regex(@"(?<!\b[0-3]?\d *)\b" + WikiRegexes.Months + @" +0([1-9])" + @"\b");
        private static readonly Regex DateLeadingZerosInt = new Regex(@"\b" + @"0([1-9]) +" + WikiRegexes.Months + @"\b");
        private static readonly Regex MonthsRegex = new Regex(@"\b" + WikiRegexes.MonthsNoGroup + @"\b.{0,25}");
        private static readonly Regex MonthsRegexNoSecondBreak = new Regex(@"\b" + WikiRegexes.MonthsNoGroup + @".{0,30}");
        private static readonly Regex DayOfMonth = new Regex(@"(?<![Tt]he +)\b([1-9]|[12]\d|3[01])(?:st|nd|rd|th) +of +" + WikiRegexes.Months);
        private static readonly Regex Ordinal = new Regex(@"\d(?:st|nd|rd|th)");
        private static readonly Regex MonthsAct = new Regex(@"\b(?:January|February|March|April|May|June|July|August|September|October|November|December) Act\b");
        //Ordinal number found inside <sup> tags.
        private static readonly Regex SupOrdinal = new Regex(@"(\d)<sup>(st|nd|rd|th)</sup>", RegexOptions.Compiled);
        private static readonly Regex FixDateOrdinalsAndOfQuick = new Regex(@"[0-9](st|nd|rd|th)|\b0[1-9]\b| of +([0-9]|[A-Z])");

        // Covered by TestFixDateOrdinalsAndOf
        /// <summary>
        /// Removes ordinals, leading zeros from dates and 'of' between a month and a year, per [[WP:MOSDATE]]; on en wiki only
        /// </summary>
        /// <param name="articleText">The wiki text of the article</param>
        /// <param name="articleTitle">The article's title</param>
        /// <returns>The modified article text.</returns>
        public string FixDateOrdinalsAndOf(string articleText, string articleTitle)
        {
            if (!Variables.LangCode.Equals("en"))
                return articleText;
            
            bool monthsInTitle = MonthsRegex.IsMatch(articleTitle);

            for (; ; )
            {
                bool reparse = false;
                // performance: better to loop through all instances of dates and apply regexes to those than
                // to apply regexes to whole article text
                // Secondly: filter down only to those portions that could be changed
                List<Match> monthsm = (from Match m in MonthsRegex.Matches(articleText) select m).Where(m => 
                    FixDateOrdinalsAndOfQuick.IsMatch(articleText.Substring(m.Index-Math.Min(25, m.Index), Math.Min(25, m.Index)+m.Length))).ToList();

                foreach(Match m in monthsm)
                {
                    // take up to 25 characters before match, unless match within first 25 characters of article
                    string before = articleText.Substring(m.Index-Math.Min(25, m.Index), Math.Min(25, m.Index)+m.Length);

                    if(MonthsAct.IsMatch(before))
                        continue;

                    string after = FixDateOrdinalsAndOfLocal(before, monthsInTitle);
                    
                    if(!after.Equals(before))
                    {
                        reparse = true;
                        articleText = articleText.Replace(before, after);

                        // catch after other fixes
                        articleText = IncorrectCommaAmericanDates.Replace(articleText, @"$1 $2, $3");
                        articleText = IncorrectCommaInternationalDates.Replace(articleText, @"$1 $2");

                        break;
                    }
                }
                if (!reparse)
                    break;
            }
            
            return articleText;
        }
        
        private string FixDateOrdinalsAndOfLocal(string textPortion, bool monthsInTitle)
        {
            textPortion = OfBetweenMonthAndYear.Replace(textPortion, "$1 $2");

            // don't apply if article title has a month in it (e.g. [[6th of October City]])
            // ordinals check for performance
            if (!monthsInTitle && Regex.IsMatch(textPortion, @"[0-9](st|nd|rd|th)"))
            {
                textPortion = OrdinalsInDatesAm.Replace(textPortion, "$1 $2$3");
                textPortion = OrdinalsInDatesInt.Replace(textPortion, "$1$2$3 $4");
                textPortion = DayOfMonth.Replace(textPortion, "$1 $2");
            }

            textPortion = DateLeadingZerosAm.Replace(textPortion, "$1 $2");
            return DateLeadingZerosInt.Replace(textPortion, "$1 $2");
        }

        // fixes extra comma or space in American format dates
        private static readonly Regex IncorrectCommaAmericanDates = new Regex(WikiRegexes.Months + @"[ ,]*([1-3]?\d(?:–[1-3]?\d)?)[ ,]+([12]\d{3})\b");

        // fix incorrect comma between month and year in Internaltional-format dates
        private static readonly Regex IncorrectCommaInternationalDates = new Regex(@"\b((?:[1-3]?\d) +" + WikiRegexes.MonthsNoGroup + @") *, *(1\d{3}|20\d{2})\b", RegexOptions.Compiled);

        // date ranges use an en-dash per [[WP:MOSDATE]]
        private static readonly Regex SameMonthInternationalDateRange = new Regex(@"\b([1-3]?\d) *- *([1-3]?\d +" + WikiRegexes.MonthsNoGroup + @")\b", RegexOptions.Compiled);
        private static readonly Regex SameMonthAmericanDateRange = new Regex(@"(" + WikiRegexes.MonthsNoGroup + @" *)([0-3]?\d) *- *([0-3]?\d)\b(?!\-)", RegexOptions.Compiled);

        // 13 July -28 July 2009 -> 13–28 July 2009
        // July 13 - July 28 2009 -> July 13–28, 2009
        private static readonly Regex LongFormatInternationalDateRange = new Regex(@"\b([1-3]?\d) +" + WikiRegexes.Months + @" *(?:-|–|&nbsp;) *([1-3]?\d) +\2,? *([12]\d{3})\b", RegexOptions.Compiled);
        private static readonly Regex LongFormatAmericanDateRange = new Regex(WikiRegexes.Months + @" +([1-3]?\d) +" + @" *(?:-|–|&nbsp;) *\1 +([1-3]?\d) *,? *([12]\d{3})\b", RegexOptions.Compiled);
        private static readonly Regex EnMonthRange = new Regex(@"\b" + WikiRegexes.Months + @"-" + WikiRegexes.Months + @"\b", RegexOptions.Compiled);

        private static readonly Regex FullYearRange = new Regex(@"((?:[\(,=;\|]|\b(?:from|between|and|reigned|f?or|ca?\.?\]*|circa)) *)([12]\d{3}) *- *([12]\d{3})(?= *(?:\)|[,;\|]|and\b|\s*$))", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex SpacedFullYearRange = new Regex(@"(?<!\b(?:ca?\.?\]*|circa) *)([12]\d{3})(?: +– *| *– +)([12]\d{3})", RegexOptions.Compiled);
        private static readonly Regex YearRangeShortenedCentury = new Regex(@"((?:[\(,=;]|\b(?:from|between|and|reigned)) *)([12]\d{3}) *- *(\d{2})(?= *(?:\)|[,;]|and\b|\s*$))", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex DateRangeToPresent = new Regex(@"\b(" + WikiRegexes.MonthsNoGroup + @"|[0-3]?\d,?) +" + @"([12]\d{3}) *- *([Pp]resent\b)", RegexOptions.Compiled);
        // matches 11 May 2010–2012 (unspaced endash in month–year range, should be spaced)
        private static readonly Regex DateRangeToYear = new Regex(@"\b(" + WikiRegexes.MonthsNoGroup + @"|\b" + WikiRegexes.MonthsNoGroup + @"(?:&nbsp;|\s+)[0-3]?\d,?) +" + @"([12]\d{3})[-–]([12]\d{3})\b", RegexOptions.Compiled);
        private static readonly Regex YearRangeToPresent = new Regex(@"\b([12]\d{3}) *- *([Pp]resent\b)", RegexOptions.Compiled);

        private static readonly Regex YearDash = new Regex(@"[12]\d{3}[–-]");
        private static readonly Regex InternationalDateFullUnspacedRange = new Regex(WikiRegexes.InternationalDates + @"[–-]" + WikiRegexes.InternationalDates);
        private static readonly Regex AmericanDateFullUnspacedRange = new Regex(WikiRegexes.AmericanDates + @"[–-]" + WikiRegexes.AmericanDates);

        /// <summary>
        /// Fix date and decade formatting errors: commas in American/international dates, full date ranges, month ranges
        /// </summary>
        /// <param name="articleText"></param>
        /// <returns></returns>
        public string FixDatesA(string articleText)
        {
            if (!Variables.LangCode.Equals("en"))
                return articleText;

            /* performance check: on most articles no date changes, on long articles HideMore is slow, so if no changes to raw text
             * don't need to perform actual check on HideMore text, and this is faster overall
             * Secondly: faster to apply regexes to each date found than to apply regexes to whole article text
             */
            bool changes = false;
            foreach(Match m in MonthsRegexNoSecondBreak.Matches(articleText))
            {
                // take up to 25 characters before match, unless match within first 25 characters of article
                string before = articleText.Substring(m.Index-Math.Min(25, m.Index), Math.Min(25, m.Index)+m.Length);

                string after = FixDatesAInternal(before);

                if(!after.Equals(before))
                {
                    changes = true;
                    break;
                }
            }

            if(!changes)
                return articleText;

            articleText = HideTextImages(articleText);

            articleText = FixDatesAInternal(articleText);

            return AddBackTextImages(articleText);
        }

        private string FixDatesAInternal(string textPortion)
        {
            bool hasDash = (textPortion.Contains("-") || textPortion.Contains("–")), hasComma = textPortion.Contains(",");
            
            if(hasComma)
                textPortion = IncorrectCommaInternationalDates.Replace(textPortion, @"$1 $2");

            if(hasDash)
            {
                textPortion = SameMonthInternationalDateRange.Replace(textPortion, @"$1–$2");

                textPortion = SameMonthAmericanDateRange.Replace(textPortion, SameMonthAmericanDateRangeME);

                textPortion = LongFormatInternationalDateRange.Replace(textPortion, @"$1–$3 $2 $4");
                textPortion = LongFormatAmericanDateRange.Replace(textPortion, @"$1 $2–$3, $4");
            }

            // run this after the date range fixes
            textPortion = IncorrectCommaAmericanDates.Replace(textPortion, @"$1 $2, $3");

            // month range
            if(hasDash)
                textPortion = EnMonthRange.Replace(textPortion, @"$1–$2");
        
            return textPortion;
        }
        
        private static readonly Regex YearRange = new Regex(@"\b[12][0-9]{3}.{0,25}");

        /// <summary>
        /// Fix date and decade formatting errors: date/year ranges to present, full year ranges, performs floruit term wikilinking
        /// </summary>
        /// <param name="articleText"></param>
        /// <param name="CircaLink"></param>
        /// <param name="Floruit"></param>
        /// <returns></returns>
        public string FixDatesB(string articleText, bool CircaLink, bool Floruit)
        {
            if (!Variables.LangCode.Equals("en"))
                return articleText;

            for (; ; )
            {
                /* performance check: faster to apply regexes to each year/date found
                 * than to apply regexes to whole article text
                 */
                bool reparse = false;
                foreach(Match m in YearRange.Matches(articleText))
                {
                    // take up to 25 characters before match, unless match within first 25 characters of article
                    string before = articleText.Substring(m.Index-Math.Min(25, m.Index), Math.Min(25, m.Index)+m.Length);
                    
                    string after = FixDatesBInternal(before, CircaLink);

                    if(!after.Equals(before))
                    {
                        reparse = true;
                        articleText = articleText.Replace(before, after);
                        break;
                    }
                }

                if (!reparse)
                    break;
            }

            // replace first occurrence of unlinked floruit with linked version, zeroth section only
            if(Floruit)
                articleText = WikiRegexes.UnlinkedFloruit.Replace(articleText, @"([[floruit|fl.]] $1", 1);

            return articleText;
        }

        private string FixDatesBInternal(string textPortion, bool CircaLink)
        {
            textPortion = DateRangeToPresent.Replace(textPortion, @"$1 $2 – $3");
            textPortion = YearRangeToPresent.Replace(textPortion, @"$1–$2");

            // 1965–1968 fixes: only appy year range fix if two years are in order
            if (!CircaLink)
            {
                textPortion = FullYearRange.Replace(textPortion, FullYearRangeME);
                textPortion = SpacedFullYearRange.Replace(textPortion, SpacedFullYearRangeME);
            }

            // 1965–68 fixes
            textPortion = YearRangeShortenedCentury.Replace(textPortion, YearRangeShortenedCenturyME);

            // endash spacing: date–year --> date – year
            textPortion = DateRangeToYear.Replace(textPortion, @"$1 $2 – $3");

            // full date range spacing: date–date --> date – date
            if(YearDash.IsMatch(textPortion))
            {
                textPortion = InternationalDateFullUnspacedRange.Replace(textPortion, m => m.Value.Replace("-", "–").Replace("–", " – "));
                textPortion = AmericanDateFullUnspacedRange.Replace(textPortion, m => m.Value.Replace("-", "–").Replace("–", " – "));
            }

            return textPortion;
        }


        private static string FullYearRangeME(Match m)
        {
            int year1 = Convert.ToInt32(m.Groups[2].Value), year2 = Convert.ToInt32(m.Groups[3].Value);

            if (year2 > year1 && year2 - year1 <= 300)
                return m.Groups[1].Value + m.Groups[2].Value + (m.Groups[1].Value.ToLower().Contains("c") ? @" – " : @"–") + m.Groups[3].Value;

            return m.Value;
        }

        private static string SpacedFullYearRangeME(Match m)
        {
            int year1 = Convert.ToInt32(m.Groups[1].Value), year2 = Convert.ToInt32(m.Groups[2].Value);

            if (year2 > year1 && year2 - year1 <= 300)
                return m.Groups[1].Value + @"–" + m.Groups[2].Value;

            return m.Value;
        }

        private static string YearRangeShortenedCenturyME(Match m)
        {
            int year1 = Convert.ToInt32(m.Groups[2].Value); // 1965
            int year2 = Convert.ToInt32(m.Groups[2].Value.Substring(0, 2) + m.Groups[3].Value); // 68 -> 19 || 68 -> 1968

            if (year2 > year1 && year2 - year1 <= 99)
                return m.Groups[1].Value + m.Groups[2].Value + @"–" + m.Groups[3].Value;

            return m.Value;
        }

        private static string SameMonthAmericanDateRangeME(Match m)
        {
            int day1 = Convert.ToInt32(m.Groups[2].Value), day2 = Convert.ToInt32(m.Groups[3].Value);

            if (day2 > day1)
                return Regex.Replace(m.Value, @" *- *", @"–");

            return m.Value;
        }
    }
}
