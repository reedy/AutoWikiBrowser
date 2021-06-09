/*
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
using System.Text;
using System.Text.RegularExpressions;

namespace WikiFunctions
{
    /// <summary>
    /// 
    /// </summary>
    public static class Summary
    {
        public const int MaxLength = 255;

        /// <summary>
        /// Returns the name of modified section, top for zeroth section, or empty string if more than one section has changed or no changes
        /// </summary>
        /// <param name="originalText"></param>
        /// <param name="articleText"></param>
        /// <returns></returns>
        public static string ModifiedSection(string originalText, string articleText)
        {
            string[] sectionsBefore = Tools.SplitToSections(originalText), 
            sectionsAfter = Tools.SplitToSections(articleText);

            // if number of sections has changed, can't provide section edit summary
            if (sectionsAfter.Length != sectionsBefore.Length)
                return "";

            int sectionsChanged = 0, sectionChangeNumber = 0;

            for (int i = 0; i < sectionsAfter.Length; i++)
            {
                if (!sectionsBefore[i].Equals(sectionsAfter[i]))
                {
                    sectionsChanged++;
                    sectionChangeNumber = i;
                }

                // if multiple sections changed, can't provide section edit summary
                if (sectionsChanged > 1)
                    return "";
            }

            if(sectionsChanged == 0)
                return "";

            // so SectionsChanged == 1, get heading name from regex, or return "top" if zeroth section
            string heading = WikiRegexes.Headings.Match(sectionsAfter[sectionChangeNumber]).Groups[1].Value.Trim();
            return (heading.Length == 0 ? "top" : heading);
        }

        private static readonly Regex SummaryTrim = new Regex(@"\s*\[\[[^\[\]\r\n]+?\]\]$", RegexOptions.Compiled);

        // Covered by ToolsTests.TrimEditSummary()
        /// <summary>
        /// Truncates an edit summary that's over the maximum supported length
        /// </summary>
        /// <param name="summary">Edit summary</param>
        /// <returns>Shortened edit summary if the input summary was too long</returns>
        public static string Trim(string summary)
        {
            int maxAvailableSummaryLength = MaxLength - 5 - (Variables.SummaryTag.Length + 1);
            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_10#Edit_summary_issue
            // replace last wikilink with dots as an attempt to prevent broken wikilinks in edit summary
            if (Encoding.UTF8.GetByteCount(summary) >= maxAvailableSummaryLength && summary.EndsWith(@"]]"))
            {
                summary = SummaryTrim.Replace(summary, "...");
            }

            return Encoding.UTF8.GetByteCount(summary) > maxAvailableSummaryLength
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

            // check for unbalanced double brackets
            int pos = s.IndexOf("[[", StringComparison.Ordinal);
            while (pos >= 0)
            {
                s = s.Remove(0, pos);

                if (res)
                {
                    // if more double brackets opened before current one closed, summary is invalid
                    if (s.Substring(2,
                            s.IndexOf("]]", StringComparison.Ordinal) > 0
                                ? s.IndexOf("]]", StringComparison.Ordinal)
                                : 0)
                        .Contains("[["))
                        return false;
                    pos = s.IndexOf("]]", StringComparison.Ordinal);
                }
                else
                {
                    pos = s.IndexOf("[[", StringComparison.Ordinal);
                }

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
