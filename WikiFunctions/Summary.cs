using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using WikiFunctions.Parse;

namespace WikiFunctions
{
    public static class Summary
    {
        public const int MaxLength = 255;

        public static string SectionEditSummary(string originalArticleTextLocal, string articleTextLocal)
        {
            // TODO: could add recursion to look for edits to only a level 3 section within a level 2 etc.

            // if edit only affects one level 2 heading, add /* heading  title */ to make a section edit
            if (!WikiRegexes.HeadingLevelTwo.IsMatch(originalArticleTextLocal))
                return ("");

            string[] levelTwoHeadingsBefore = { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
            string[] levelTwoHeadingsAfter = { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };

            int before = 0, after = 0;

            string zerothSectionBefore = WikiRegexes.ArticleToFirstLevelTwoHeading.Match(originalArticleTextLocal).Value;
            if (!string.IsNullOrEmpty(zerothSectionBefore))
                originalArticleTextLocal = originalArticleTextLocal.Replace(zerothSectionBefore, "");

            string zerothSectionAfter = WikiRegexes.ArticleToFirstLevelTwoHeading.Match(articleTextLocal).Value;
            if (!string.IsNullOrEmpty(zerothSectionAfter))
                articleTextLocal = articleTextLocal.Replace(zerothSectionAfter, "");

            // can't provide a section edit summary if there are changes in text before first level 2 heading
            if (!string.IsNullOrEmpty(zerothSectionBefore) && zerothSectionBefore != zerothSectionAfter)
                return "";

            // get sections for article text before any AWB changes
            foreach (Match m in WikiRegexes.SectionLevelTwo.Matches(originalArticleTextLocal))
            {
                levelTwoHeadingsBefore[before] = null;
                levelTwoHeadingsBefore[before] = m.Value;
                originalArticleTextLocal = originalArticleTextLocal.Replace(m.Value, "");
                before++;

                if (before == 20)
                    return "";
            }
            // add the last section to the array
            levelTwoHeadingsBefore[before] = originalArticleTextLocal;

            // get sections for article text after AWB changes
            foreach (Match m in WikiRegexes.SectionLevelTwo.Matches(articleTextLocal))
            {
                levelTwoHeadingsAfter[after] = m.Value;
                articleTextLocal = articleTextLocal.Replace(m.Value, "");
                after++;

                if (after == 20)
                    return "";
            }

            // handle the array not being big enough
            if (levelTwoHeadingsAfter.Length < after)
                return "";

            // add the last section to the array
            levelTwoHeadingsAfter[after] = articleTextLocal;

            // if number of sections has changed, can't provide section edit summary
            if (after != before)
                return "";

            int sectionsChanged = 0, sectionChangeNumber = 0;

            for (int i = 0; i <= after; i++)
            {
                if (levelTwoHeadingsBefore[i] != levelTwoHeadingsAfter[i])
                {
                    sectionsChanged++;
                    sectionChangeNumber = i;
                }

                // if multiple level 2 sections changed, can't provide section edit summary
                if (sectionsChanged == 2)
                    return "";
            }

            // so SectionsChanged == 1, get heading name from LevelTwoHeadingsBefore
            return WikiRegexes.HeadingLevelTwo.Match(levelTwoHeadingsBefore[sectionChangeNumber]).Groups[1].Value.Trim();
        }


        // Covered by ToolsTests.TrimEditSummary()
        /// <summary>
        /// Truncates an edit summary that's over the maximum supported length
        /// </summary>
        /// <param name="summary">input long edit summary</param>
        /// <returns>shortened edit summary</returns>
        public static string Trim(string summary)
        {
            int maxAvailableSummaryLength = ((MaxLength - 5) - (Variables.SummaryTag.Length + 1));
            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_10#Edit_summary_issue
            // replace last wikilink with dots as an attempt to prevent broken wikilinks in edit summary
            if (Encoding.UTF8.GetByteCount(summary) >= maxAvailableSummaryLength && summary.EndsWith(@"]]"))
                summary = Regex.Replace(summary, @"\s*\[\[[^\[\]\r\n]+?\]\]$", "...");

            return (Encoding.UTF8.GetByteCount(summary) > maxAvailableSummaryLength)
                       ? LimitByteLength(summary, maxAvailableSummaryLength)
                       : summary;
        }

        /// <summary>
        /// returns true if given string has matching double square brackets and is within the maximum permitted length
        /// </summary>
        public static bool IsCorrect(string s)
        {
            if (Encoding.UTF8.GetByteCount(s) > MaxLength)
                return false;

            bool res = true;

            int pos = s.IndexOf("[[");
            while (pos >= 0)
            {
                s = s.Remove(0, pos);

                pos = res ? s.IndexOf("]]") : s.IndexOf("[[");

                res = !res;
            }
            return res;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        /// <remarks>
        /// http://stackoverflow.com/questions/1225052/best-way-to-shorten-utf8-string-based-on-byte-length
        /// </remarks>
        private static string LimitByteLength(string input, int maxLength)
        {
            for (int i = input.Length - 1; i >= 0; i--)
            {
                if (Encoding.UTF8.GetByteCount(input.Substring(0, i + 1)) <= maxLength)
                {
                    return input.Substring(0, i + 1);
                }
            }

            return string.Empty;
        }
    }
}
