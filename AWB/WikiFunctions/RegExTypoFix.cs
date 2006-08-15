/*
WikiFunctions
Copyright (C) 2006

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
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;
using System.Windows.Forms;

namespace WikiFunctions
{
    public class RegExTypoFix
    {
        public RegExTypoFix()
        {
            MakeRegexes();
        }

        Dictionary<Regex, string> TypoRegexes = new Dictionary<Regex, string>();
        HideText RemoveText = new HideText(true, false);

        private void MakeRegexes()
        {
            try
            {
                TypoRegexes.Clear();
                Dictionary<string, string> TypoStrings = LoadTypos();

                Regex r;
                RegexOptions roptions = RegexOptions.Compiled;
                foreach (KeyValuePair<string, string> k in TypoStrings)
                {
                    r = new Regex(k.Key, roptions);
                    TypoRegexes.Add(r, k.Value);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        MatchCollection Matches;
        string summary = "";

        public string PerformTypoFixes(string ArticleText, ref bool NoChange)
        {
            if (Regex.IsMatch(ArticleText, "133t|-ology|\\(sic\\)|\\[sic\\]|\\[''sic''\\]|\\{\\{sic\\}\\}|spellfixno"))
                return ArticleText;

            ArticleText = RemoveText.Hide(ArticleText);
            string OriginalText = ArticleText;
            string Replace = "";

            foreach (KeyValuePair<Regex, string> k in TypoRegexes)
            {
                Matches = k.Key.Matches(ArticleText);

                if (Matches.Count > 0)
                {
                    Replace = k.Value;
                    ArticleText = k.Key.Replace(ArticleText, Replace);

                    if (Matches[0].Value != Matches[0].Result(Replace))
                    {
                        summary = Matches[0].Value + " → " + Matches[0].Result(Replace);

                        if (Matches.Count > 1)
                            summary += " (" + Matches.Count.ToString() + ")";

                        EditSummary += summary + ", ";
                    }
                }
            }

            if (OriginalText == ArticleText)
                NoChange = true;
            else
                NoChange = false;

            ArticleText = RemoveText.AddBack(ArticleText);

            if (EditSummary != "")
                EditSummary = " Typos: " + EditSummary.Trim();

            return ArticleText;
        }

        string strSummary = "";
        public string EditSummary
        {
            get { return strSummary; }
            set { strSummary = value; }
        }

        private Dictionary<string, string> LoadTypos()
        {
            Dictionary<string, string> TypoStrings = new Dictionary<string, string>();

            Regex TypoRegex = new Regex("<(?:Typo )?word=\".*?\"[ \\t]find=\"(.*?)\"[ \\t]replace=\"(.*?)\" ?/?>");
            try
            {
                string text = Tools.GetArticleText("Wikipedia:AutoWikiBrowser/Typos");
                foreach (Match m in TypoRegex.Matches(text))
                {
                    TypoStrings.Add(m.Groups[1].Value, m.Groups[2].Value);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return TypoStrings;
        }

        public Dictionary<Regex, string> Typos
        {
            get { return TypoRegexes; }
        }
    }
}
