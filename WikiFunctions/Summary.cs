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

        /// <summary>
        /// Returns the name of modified section or empty string if more than one section has changed
        /// </summary>
        /// <param name="originalText"></param>
        /// <param name="articleText"></param>
        /// <returns></returns>
        public static string ModifiedSection(string originalText, string articleText)
        {
            var sectionsBefore = Tools.SplitToSections(originalText);
            if (sectionsBefore.Length == 0)
                return "";

            var sectionsAfter = Tools.SplitToSections(articleText);

            // if number of sections has changed, can't provide section edit summary
            if (sectionsAfter.Length != sectionsBefore.Length)
                return "";

            int sectionsChanged = 0, sectionChangeNumber = 0;

            for (int i = 0; i < sectionsAfter.Length; i++)
            {
                if (sectionsBefore[i] != sectionsAfter[i])
                {
                    sectionsChanged++;
                    sectionChangeNumber = i;
                }

                // if multiple sections changed, can't provide section edit summary
                if (sectionsChanged > 1)
                    return "";
            }

            // so SectionsChanged == 1, get heading name from regex
            return WikiRegexes.Headings.Match(sectionsAfter[sectionChangeNumber]).Groups[1].Value.Trim();
        }

        private static readonly Regex SummaryTrim = new Regex(@"\s*\[\[[^\[\]\r\n]+?\]\]$", RegexOptions.Compiled);

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
                summary = SummaryTrim.Replace(summary, "...");

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
