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
        /// <summary>
        /// Delinks all bolded self links in the article
        /// </summary>
        /// <param name="articleTitle">Title of the article</param>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns></returns>
        private static string BoldedSelfLinks(string articleTitle, string articleText)
        {
            string escTitle = Regex.Escape(articleTitle);

            Regex plainLink = new Regex(@"'''\[\[\s*(" + escTitle + @"|" + Tools.TurnFirstToLower(escTitle) + @")\s*\]\]'''");
            Regex pipedLink = new Regex(@"'''\[\[\s*(?:" + escTitle + @"|" + Tools.TurnFirstToLower(escTitle) + @")\s*\|\s*([^\[\]]+?)\s*\]\]'''");

            articleText = plainLink.Replace(articleText, @"'''$1'''");
            articleText = pipedLink.Replace(articleText, @"'''$1'''");

            return articleText;
        }

        private static readonly Regex BracketedAtEndOfLine = new Regex(@" \(.*?\)$", RegexOptions.Compiled);
        private static readonly Regex BoldTitleAlready3 = new Regex(@"^\s*({{[^\{\}]+}}\s*)*'''('')?\s*\w", RegexOptions.Compiled);
        private static readonly Regex BoldTitleAlready4 = new Regex(@"^\s*'''", RegexOptions.Multiline);
        private static readonly Regex DfnTag = new Regex(@"<\s*dfn\s*>", RegexOptions.IgnoreCase);
        private static readonly Regex NihongoTitle = Tools.NestedTemplateRegex("nihongo title");
        // bio used on it-wiki, automatically bolds link
        private static readonly Regex NoBoldTitle = Tools.NestedTemplateRegex(new[] { "year article header", "bio" });

        // Covered by: BoldTitleTests
        /// <summary>
        /// '''Emboldens''' the first occurrence of the article title, if not already bold
        /// 1) Cleans up bolded self wikilinks
        /// 2) Cleans up self wikilinks
        /// 3) '''Emboldens''' the first occurrence of the article title
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">The title of the article.</param>
        /// <param name="noChange">Value that indicated whether no change was made.</param>
        /// <returns>The modified article text.</returns>
        public string BoldTitle(string articleText, string articleTitle, out bool noChange)
        {
            noChange = true;
            List<string> alltemplates = GetAllTemplates(articleText);

            if(TemplateExists(alltemplates, NoBoldTitle))
                return articleText;

            HideText Hider2 = new HideText(), Hider3 = new HideText(true, true, true);

            // 1) clean up bolded self links first, provided no noinclude use in article
            string afterSelfLinks = BoldedSelfLinks(articleTitle, articleText);
            
            if(!afterSelfLinks.Equals(articleText) && !WikiRegexes.IncludeonlyNoinclude.IsMatch(articleText))
                articleText = afterSelfLinks;

            // 2) Clean up self wikilinks
            string articleTextAtStart = articleText, zerothSection = Tools.GetZerothSection(articleText);
            string restOfArticle = articleText.Substring(zerothSection.Length);
            string zerothSectionHidden, zerothSectionHiddenOriginal;

            // first check for any self links and no bold title, if found just convert first link to bold and return
            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#Includes_and_selflinks
            // don't apply if bold in lead section already or some noinclude transclusion business
            if(!SelfLinks(zerothSection, articleTitle).Equals(zerothSection) && !WikiRegexes.IncludeonlyNoinclude.IsMatch(articleText))
            {
                // There's a limitation here in that we can't hide image descriptions that may be above lead sentence without hiding the self links we are looking to correct
                zerothSectionHidden = Hider2.HideMore(zerothSection, false, false, false);
                zerothSectionHiddenOriginal = zerothSectionHidden;
                zerothSectionHidden = SelfLinks(zerothSectionHidden, articleTitle);
                zerothSection = Hider2.AddBackMore(zerothSectionHidden);

                if (!zerothSectionHiddenOriginal.Equals(zerothSectionHidden))
                {
                    noChange = false;
                    return (zerothSection + restOfArticle);
                }
            }

            // Performance check: if article title not in zeroth section have nothing further to do
            if(zerothSection.IndexOf(BracketedAtEndOfLine.Replace(articleTitle, ""), StringComparison.OrdinalIgnoreCase) < 0)
                return articleTextAtStart;

            // 3) '''Emboldens''' the first occurrence of the article title

            // ignore date articles (date in American or international format), nihongo title
            if (WikiRegexes.Dates2.IsMatch(articleTitle) || WikiRegexes.Dates.IsMatch(articleTitle)
                || TemplateExists(alltemplates, NihongoTitle))
                return articleTextAtStart;

            string escTitle = Regex.Escape(articleTitle), escTitleNoBrackets = Regex.Escape(BracketedAtEndOfLine.Replace(articleTitle, ""));
            Regex boldTitleAlready1 = new Regex(@"'''\s*(" + escTitle + "|" + Tools.TurnFirstToLower(escTitle) + @")\s*'''");
            Regex boldTitleAlready2 = new Regex(@"'''\s*(" + escTitleNoBrackets + "|" + Tools.TurnFirstToLower(escTitleNoBrackets) + @")\s*'''");

            // if title in bold already exists in article, or paragraph starts with something in bold, don't change anything
            // ignore any bold in infoboxes
            if(BoldTitleAlready4.IsMatch(Tools.ReplaceWithSpaces(zerothSection, WikiRegexes.InfoBox.Matches(zerothSection))) || DfnTag.IsMatch(zerothSection))
                return articleTextAtStart;
            
            string articleTextNoInfobox = Tools.ReplaceWithSpaces(articleText, WikiRegexes.InfoBox.Matches(articleText));
            if (boldTitleAlready1.IsMatch(articleTextNoInfobox) || boldTitleAlready2.IsMatch(articleTextNoInfobox)
                || BoldTitleAlready3.IsMatch(articleTextNoInfobox))
                return articleTextAtStart;

            // so no self links to remove, check for the need to add bold
            string articleTextNoTemplates = WikiRegexes.NestedTemplates.Replace(articleText, "");

            // first quick check: ignore articles with some bold in first 5% of article, ignoring infoboxes, dablinks etc.
            int fivepc = articleTextNoTemplates.Length / 20;

            if (articleTextNoTemplates.Substring(0, fivepc).Contains("'''"))
                return articleTextAtStart;

            Regex regexBoldNoBrackets = new Regex(@"([^\[]|^)(" + escTitleNoBrackets + "|" + Tools.TurnFirstToLower(escTitleNoBrackets) + ")([ ,.:;])");

            zerothSectionHidden = Hider3.HideMore(zerothSection);
            zerothSectionHiddenOriginal = zerothSectionHidden;

            // first try title with brackets removed
            zerothSectionHidden = regexBoldNoBrackets.Replace(zerothSectionHidden, "$1'''$2'''$3", 1);

            zerothSection = Hider3.AddBackMore(zerothSectionHidden);
            
            articleText = zerothSection + restOfArticle;

            // check that the bold added is the first bit in bold in the main body of the article
            if (!zerothSectionHiddenOriginal.Equals(zerothSectionHidden) && AddedBoldIsValid(articleText, escTitleNoBrackets))
            {
                noChange = false;
                return articleText;
            }

            return articleTextAtStart;
        }
        
        private static string SelfLinks(string zerothSection, string articleTitle)
        {
            string zerothSectionOriginal = zerothSection, escTitle = Regex.Escape(articleTitle);
            
            if (!zerothSection.Contains("'''" + articleTitle + "'''"))
            {
                Regex r1 = new Regex(@"\[\[\s*" + escTitle + @"\s*\]\]");
                Regex r3 = new Regex(@"\[\[\s*" + escTitle + @"\s*\|\s*([^\[\]]+?)\s*\]\]");
                
                zerothSection = r1.Replace(zerothSection, "'''" + articleTitle + @"'''");
                zerothSection = r3.Replace(zerothSection, "'''$1'''");
            }

            if (zerothSectionOriginal.Equals(zerothSection) && !zerothSection.Contains("'''" + Tools.TurnFirstToLower(articleTitle) + "'''"))
            {
                Regex r2 = new Regex(@"\[\[\s*" + Tools.TurnFirstToLower(escTitle) + @"\s*\]\]");
                Regex r4 = new Regex(@"\[\[\s*" + Tools.TurnFirstToLower(escTitle) + @"\s*\|\s*([^\[\]]+?)\s*\]\]");
                
                zerothSection = r2.Replace(zerothSection, "'''" + Tools.TurnFirstToLower(articleTitle) + @"'''");
                zerothSection = r4.Replace(zerothSection, "'''$1'''");
            }
            
            return zerothSection;
        }

        private static readonly Regex RegexFirstBold = new Regex(@"^(.*?)'''", RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Checks that the bold just added to the article is the first bold in the article, and that it's within the first 5% of the HideMore article OR immediately after the infobox
        /// </summary>
        private bool AddedBoldIsValid(string articleText, string escapedTitle)
        {
            HideText Hider2 = new HideText(true, true, true);
            Regex RegexBoldAdded = new Regex(@"^(.*?)'''(" + escapedTitle + @")", RegexOptions.Singleline | RegexOptions.IgnoreCase);

            int boldAddedPos = RegexBoldAdded.Match(articleText).Groups[2].Index;

            int firstBoldPos = RegexFirstBold.Match(articleText).Length;

            articleText = WikiRegexes.NestedTemplates.Replace(articleText, "");

            articleText = Hider2.HideMore(articleText);

            // was bold added in first 5% of article?
            bool inFirst5Percent = false;

            int articlelength = articleText.Length;

            if (articlelength > 5)
                inFirst5Percent = articleText.Trim().Substring(0, Math.Max(articlelength / 20, 5)).Contains("'''");

            articleText = Hider2.AddBackMore(articleText);
            // check that the bold added is the first bit in bold in the main body of the article, and in first 5% of HideMore article
            return inFirst5Percent && boldAddedPos <= firstBoldPos;
        }
    }
}
