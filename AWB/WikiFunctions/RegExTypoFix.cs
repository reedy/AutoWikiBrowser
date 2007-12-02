/*

Copyright (C) 2007 Martin Richards & Max Semenik

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

namespace WikiFunctions.Parse
{
    /// <summary>
    /// Represents a group of similar typo regexes
    /// </summary>
    class TypoGroup
    {
        public TypoGroup(int groupSize, string prefix, string postfix)
        {
            GroupSize = groupSize;
            Prefix = prefix;
            Postfix = postfix;
        }

        int GroupSize;
        string Prefix;
        string Postfix;

        public List<KeyValuePair<Regex, string>> Typos = new List<KeyValuePair<Regex, string>>(20);
        List<Regex> Groups = new List<Regex>(5);

        public bool SuitableTypo(string typo)
        {
            return typo.StartsWith(Prefix) && typo.EndsWith(Postfix);
        }

        /// <summary>
        /// Adds one typo regex to the list
        /// </summary>
        public void Add(string typo, string replacement)
        {
            //*
            if (!SuitableTypo(typo))
            {
                throw new ArgumentException("Typo \"" + typo + "\" does not start with \"" + Prefix +
                    "\" or end with \"" + Postfix + "\"");
            }
            //*/
            Typos.Add(new KeyValuePair<Regex, string>(new Regex(typo, RegexOptions.Compiled), replacement));

        }

        /// <summary>
        /// Makes grouped regexes
        /// </summary>
        public void MakeGroups()
        {
            for (int n = 0; n < (Typos.Count - 1) / GroupSize + 1; n++)
            {
                string s = "";
                for (int i = 0; i < Math.Min(GroupSize, Typos.Count - n * GroupSize); i++)
                {
                    string typo = Typos[n * GroupSize + i].Key.ToString();
                    if (Prefix.Length > 0) typo = typo.Remove(0, Prefix.Length);
                    if (Postfix.Length > 0) typo = typo.Remove(typo.Length - Postfix.Length);
                    s += (s.Length == 0 ? "" : "|") + typo;
                }
                if (s.Length > 0) Groups.Add(new Regex(Prefix + "(" + s + ")" + Postfix, RegexOptions.Compiled));
            }
        }

        /// <summary>
        /// Fixes typos
        /// </summary>
        public void FixTypos(ref string ArticleText, ref string summary)
        {
            for (int i = 0; i < Groups.Count; i++)
            {
                if (Groups[i].IsMatch(ArticleText))
                {
                    for (int j = 0; j < Math.Min(GroupSize, Typos.Count - i * GroupSize); j++)
                    {
                        KeyValuePair<Regex, string> typo = Typos[i * GroupSize + j];
                        MatchCollection matches = typo.Key.Matches(ArticleText);
                        if (matches.Count > 0)
                        {
                            ArticleText = typo.Key.Replace(ArticleText, typo.Value);
                            int count = 0;
                            foreach (Match m in matches)
                            {
                                string res = typo.Key.Replace(m.Value, typo.Value);
                                if (res != m.Value)
                                {
                                    count++;
                                    if (1 == count) summary += (summary.Length > 0 ? ", " : "") + m.Value + " → " + res;
                                }
                            }
                            if (count > 1) summary += " (" + count.ToString() + ")";
                        }
                    }
                }
            }
        }
    }


    public class RegExTypoFix
    {
        public RegExTypoFix()
        {
            MakeRegexes();
        }

        public int TyposCount;

        Regex IgnoreRegex = new Regex("133t|-ology|\\(sic\\)|\\[sic\\]|\\[''sic''\\]|\\{\\{sic\\}\\}|spellfixno", RegexOptions.Compiled);
        HideText RemoveText = new HideText(true, false, true);

        List<TypoGroup> Typos = new List<TypoGroup>();

        static readonly Regex RemoveTail = new Regex(@"(\s|\n|\r|\*|#|:|⌊⌊⌊⌊M?\d*⌋⌋⌋⌋)*$", RegexOptions.Compiled);

        private void MakeRegexes()
        {
            try
            {
                Typos.Clear();
                TyposCount = 0;
                TypoGroup bounded = new TypoGroup(20, "\\b", "\\b");
                TypoGroup other = new TypoGroup(5, "", "");

                Typos.Add(bounded);
                Typos.Add(other);

                Dictionary<string, string> typoStrings = LoadTypos();

                foreach (KeyValuePair<string, string> k in typoStrings)
                {
                    if (bounded.SuitableTypo(k.Key)) bounded.Add(k.Key, k.Value);
                    else other.Add(k.Key, k.Value);
                    TyposCount++;
                }

                foreach (TypoGroup grp in Typos) grp.MakeGroups();
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }

        public string PerformTypoFixes(string ArticleText, out bool NoChange, out string Summary)
        {
            Summary = "";
            if (IgnoreRegex.IsMatch(ArticleText))
            {
                NoChange = true;
                return ArticleText;
            }

            ArticleText = RemoveText.HideMore(ArticleText);

            //remove newlines, whitespace and hide tokens from bottom
            //to avoid running 2K regexps on them
            Match m = RemoveTail.Match(ArticleText);
            string tail = m.Value;
            if (tail != "") ArticleText = ArticleText.Remove(m.Index);

            string originalText = ArticleText;
            string strSummary = "";

            foreach (TypoGroup grp in Typos)
            {
                grp.FixTypos(ref ArticleText, ref strSummary);
            }

            NoChange = (originalText == ArticleText);

            ArticleText = RemoveText.AddBackMore(ArticleText + tail);

            if (strSummary != "")
            {
                strSummary = ", typos fixed: " + strSummary.Trim();
                Summary = strSummary;
            }

            return ArticleText;
        }

        public bool DetectTypo(string ArticleText)
        {
            bool noChange;
            string summary = "";

            PerformTypoFixes(ArticleText, out noChange, out summary);

            return !noChange;
        }

        private Dictionary<string, string> LoadTypos()
        {
            Dictionary<string, string> typoStrings = new Dictionary<string, string>();

            Regex typoRegex = new Regex("<(?:Typo )?word=\"(.*?)\"[ \\t]find=\"(.*?)\"[ \\t]replace=\"(.*?)\" ?/?>", RegexOptions.Compiled);
            try
            {
                string text = "";
                try
                {
                    string typolistURL = Variables.RETFPath;

                    if (!typolistURL.StartsWith("http:"))
                        typolistURL = Variables.GetPlainTextURL(typolistURL);

                    text = Tools.GetHTML(typolistURL, Encoding.UTF8);
                }
                catch
                {
                    if (text == "")
                    {
                        if (MessageBox.Show("No list of typos was found.  Would you like to use the list of typos from the English Wikipedia?\r\nOnly choose OK if this is an English wiki.", "Load from English Wikipedia?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            try
                            {
                                text = Tools.GetHTML("http://en.wikipedia.org/w/index.php?title=Wikipedia:AutoWikiBrowser/Typos&action=raw&ctype=text/plain&dontcountme=s", Encoding.UTF8);
                            }
                            catch
                            {
                                MessageBox.Show("There was a problem loading the list of typos.");
                            }
                        }
                    }
                }
                foreach (Match m in typoRegex.Matches(text))
                {
                    try
                    {
                        typoStrings.Add(m.Groups[2].Value, m.Groups[3].Value);
                    }
                    catch (Exception ex)
                    {
                        ErrorHandler.Handle(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }

            return typoStrings;
        }

        public List<KeyValuePair<Regex, string>> GetTypos()
        {
            List<KeyValuePair<Regex, string>> lst = new List<KeyValuePair<Regex, string>>();

            foreach (TypoGroup grp in Typos)
            {
                lst.AddRange(grp.Typos);
            }

            return lst;
        }
    }
}
