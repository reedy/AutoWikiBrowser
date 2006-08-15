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

            hashLinks.Clear();
            ArticleText = RemoveLinks(ArticleText);
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

            ArticleText = AddLinks(ArticleText);

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

        Hashtable hashLinks = new Hashtable();
        readonly Regex NoLinksRegex = new Regex("<nowiki>.*?</nowiki>|<math>.*?</math>|<!--.*?-->|[Hh]ttp://[^\\ ]*|\\[[Hh]ttp:.*?\\]|\\[\\[[Ii]mage:.*?\\]\\]|\\[\\[([a-z]{2,3}|simple|fiu-vro|minnan|roa-rup|tokipona|zh-min-nan):.*\\]\\]", RegexOptions.Singleline | RegexOptions.Compiled);
        private string RemoveLinks(string articleText)
        {
            hashLinks.Clear();
            string s = "";

            int i = 0;
            foreach (Match m in NoLinksRegex.Matches(articleText))
            {
                s = "⌊⌊⌊⌊" + i.ToString() + "⌋⌋⌋⌋";

                articleText = articleText.Replace(m.Value, s);
                hashLinks.Add(s, m.Value);
                i++;
            }

            return articleText;
        }

        private string AddLinks(string articleText)
        {
            foreach (DictionaryEntry D in hashLinks)
                articleText = articleText.Replace(D.Key.ToString(), D.Value.ToString());

            hashLinks.Clear();
            return articleText;
        }       

        private Dictionary<string, string> LoadTypos()
        {
            Dictionary<string, string> TypoStrings = new Dictionary<string, string>();

            //string file = System.Reflection.Assembly.GetExecutingAssembly().Location;
            //file = file.Remove(file.LastIndexOf("\\")) + "\\Typos.xml";
            
            //if (!File.Exists(file))
            //{
            //    MessageBox.Show("Typo xml file does not exist in current directory", "File does not exist", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return TypoStrings;
            //}

            //string find = "";
            //string replace = "";

            //try
            //{                
            //    Stream stream = new FileStream(file, FileMode.Open);                

            //    using (XmlTextReader reader = new XmlTextReader(stream))
            //    {
            //        reader.WhitespaceHandling = WhitespaceHandling.None;
            //        while (reader.Read())
            //        {
            //            if (reader.Name == "Typo" && reader.HasAttributes)
            //            {
            //                reader.MoveToAttribute("find");
            //                find = reader.Value;
            //                reader.MoveToAttribute("replace");
            //                replace = reader.Value;
            //                //try
            //                //{
            //                    TypoStrings.Add(find, replace);
            //                    continue;
            //                //}
            //                //catch (Exception ex)
            //                //{
            //                //    MessageBox.Show("Error on " + find + "\r\n\r\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //                //}
            //            }
            //            if (reader.Name == "Typos" && reader.HasAttributes)
            //            {
            //                reader.MoveToAttribute("version");
            //                Version = decimal.Parse(reader.Value);
            //                continue;                           
            //            }
            //        }
            //        stream.Close();
            //    }
            //}
            //catch (IOException ex)
            //{
            //    MessageBox.Show(ex.Message, "File error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Error on " + find + "\r\n\r\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}

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
