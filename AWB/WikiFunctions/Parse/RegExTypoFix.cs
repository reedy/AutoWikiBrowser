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
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WikiFunctions.Background;
using WikiFunctions.Controls;

namespace WikiFunctions.Parse
{
    /// <summary>
    /// Interface for loading typo list
    /// </summary>
    public interface ITyposProvider
    {
        /// <summary>
        /// Returns a list of RegExTypoFix rules
        /// </summary>
        /// <returns>Key is the find part, value is replace</returns>
        Dictionary<string, string> GetTypos();
    }

    /// <summary>
    /// This is a default ITyposProvider implementation that downloads typos from
    /// location specified in Variables.RetfPath
    /// </summary>
    public class TyposDownloader : ITyposProvider
    {
        private static readonly Regex TypoRegex = new Regex("<(?:Typo)?\\s+(?:word=\"(.*?)\"\\s+)?find=\"(.*?)\"\\s+replace=\"(.*?)\"\\s*/?>", RegexOptions.Compiled);

        public static string Url
        {
            get
            {
                string typolistUrl = Variables.RetfPath;

                if (!typolistUrl.StartsWith("http:"))
                    typolistUrl = Variables.GetPlainTextURL(typolistUrl);

                return typolistUrl;
            }
        }

        public Dictionary<string, string> GetTypos()
        {
            Dictionary<string, string> typoStrings = new Dictionary<string, string>();

            try
            {
                string text = "";
                try
                {
                    text = Tools.GetHTML(Url, Encoding.UTF8);
                }
                catch
                {
                    if (string.IsNullOrEmpty(text))
                    {
                        if (MessageBox.Show("No list of typos was found. Would you like to use the list of typos from the English Wikipedia?\r\nOnly choose 'Yes' if this is an English wiki.", "Load from English Wikipedia?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            try
                            {
                                text = Tools.GetHTML("http://en.wikipedia.org/w/index.php?title=Wikipedia:AutoWikiBrowser/Typos&action=raw", Encoding.UTF8);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("There was a problem loading the list of typos: " + ex.Message);
                            }
                        }
                        else
                            text = "";
                    }
                }

                if (string.IsNullOrEmpty(text))
                    return typoStrings; // Currently an empty dictionary

                foreach (Match m in TypoRegex.Matches(text))
                {
                    try
                    {
                        typoStrings.Add(m.Groups[2].Value, m.Groups[3].Value);
                    }
                    catch (ArgumentException)
                    {
                        RegExTypoFix.TypoError("Duplicate typo rule '" + m.Groups[2].Value + "' found.");
                        return new Dictionary<string, string>();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
                // refuse to accept malformed typo lists to encourage people to correct errors
                return new Dictionary<string, string>();
            }

            return typoStrings;
        }
    }

    /// <summary>
    /// Represents a group of similar typo regexes
    /// </summary>
    class TypoGroup
    {
        /// <summary>
        /// Creates a group that holds similar typos
        /// </summary>
        /// <param name="groupSize">Typos in a batch</param>
        /// <param name="match">Regex each typo should match, its first group will be used for extraction</param>
        /// <param name="dontMatch">Regex each typo shouldn't match</param>
        /// <param name="prefix"></param>
        /// <param name="postfix"></param>
        public TypoGroup(int groupSize, string match, string dontMatch, string prefix, string postfix)
        {
            GroupSize = groupSize;

            if (!string.IsNullOrEmpty(match)) Allow = new Regex(match, RegexOptions.Compiled);
            if (!string.IsNullOrEmpty(dontMatch)) Disallow = new Regex(dontMatch, RegexOptions.Compiled);

            Prefix = prefix;
            Postfix = postfix;
        }

        private readonly int GroupSize;
        private readonly Regex Allow, Disallow;
        private readonly string Prefix, Postfix;

        private List<Regex> Groups;

        public readonly List<KeyValuePair<Regex, string>> Typos = new List<KeyValuePair<Regex, string>>(20);

        public readonly List<TypoStat> Statistics = new List<TypoStat>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typo"></param>
        /// <returns></returns>
        public bool IsSuitableTypo(string typo)
        {
            if (Allow != null && !Allow.IsMatch(typo)) return false;
            if (Disallow != null && Disallow.IsMatch(typo)) return false;

            return true;
        }

        /// <summary>
        /// Adds one typo regex to the list
        /// </summary>
        public void Add(string typo, string replacement)
        {
            if (!IsSuitableTypo(typo))
            {
                throw new ArgumentException("Typo \"" + typo + "\" is not suitable for this group.");
            }
            Regex r;
            try
            {
                r = new Regex(typo, RegexOptions.Compiled);
            }
            catch (Exception ex)
            {
                RegExTypoFix.TypoError("Error in typo '" + typo + "': " + ex.Message);
                throw new TypoException();
            }

            Typos.Add(new KeyValuePair<Regex, string>(r, replacement));
        }

        /// <summary>
        /// Makes grouped regexes
        /// </summary>
        public void MakeGroups()
        {
            if (GroupSize <= 1) return;

            Groups = new List<Regex>(5);
            for (int n = 0; n < (Typos.Count - 1) / GroupSize + 1; n++)
            {
                string s = "";
                for (int i = 0; i < Math.Min(GroupSize, Typos.Count - n * GroupSize); i++)
                {
                    string typo = Typos[n * GroupSize + i].Key.ToString();
                    if (Allow != null) typo = Allow.Match(typo).Groups[1].Value;
                    s += (s.Length == 0 ? "" : "|") + typo;
                }
                if (s.Length > 0)
                {
                    Groups.Add(new Regex(Prefix + "(" + s + ")" + Postfix, RegexOptions.Compiled));
                }
            }
        }

        /// <summary>
        /// Applies a given typo fix to the article provided the typo does not also match the article title
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="summary"></param>
        /// <param name="typo"></param>
        /// <param name="articleTitle">Title of the article</param>
        private void FixTypo(ref string articleText, ref string summary, KeyValuePair<Regex, string> typo, string articleTitle)
        {
            // don't apply the typo if it matches on the Article's title
            if (typo.Key.IsMatch(articleTitle))
                return;

            MatchCollection matches = typo.Key.Matches(articleText);

            if (matches.Count > 0)
            {
                TypoStat stats = new TypoStat(typo) {Total = matches.Count};

                articleText = typo.Key.Replace(articleText, typo.Value);

                int count = 0;

                foreach (Match m in matches)
                {
                    string res = typo.Key.Replace(m.Value, typo.Value);
                    if (res != m.Value)
                    {
                        count++;
                        if (1 == count)
                            summary += (summary.Length > 0 ? ", " : "") + m.Value + FindandReplace.Arrow + res;
                    }
                }
                if (count > 1)
                    summary += " (" + count + ")";

                stats.SelfMatches = stats.Total - count;
                Statistics.Add(stats);
            }
        }

        /// <summary>
        /// Fixes typos
        /// </summary>
        /// <param name="articleText">The wiki text to update</param>
        /// <param name="articleTitle">The title of the wiki page</param>
        /// <param name="summary">The edit summary of the page changes</param>
        [Obsolete]
        public void FixTypos(ref string articleText, ref string summary, string articleTitle)
        {
            FixTypos(ref articleText, ref summary, articleTitle, articleText);
        }

        /// <summary>
        /// Fixes typos
        /// </summary>
        /// <param name="articleText">The wiki text to update</param>
        /// <param name="articleTitle">The title of the wiki page</param>
        /// <param name="originalArticleText">The wiki text of the page, without any typo fixes applied</param>
        /// <param name="summary">The edit summary of the page changes</param>
        public void FixTypos(ref string articleText, ref string summary, string articleTitle, string originalArticleText)
        {
            Statistics.Clear();
            if (Groups != null)
                for (int i = 0; i < Groups.Count; i++)
                {
                    if (Groups[i].IsMatch(articleText))
                    {
                        for (int j = 0; j < Math.Min(GroupSize, Typos.Count - i * GroupSize); j++)
                        {
                            // don't apply the typo if it matches on a link target
                            if (!Regex.IsMatch(originalArticleText, @"\[\[[^\[\]\r\n\|]*?" + Typos[i * GroupSize + j].Key + @"[^\[\]\r\n\|]*?(?:\]\]|\|)"))
                                FixTypo(ref articleText, ref summary, Typos[i * GroupSize + j], articleTitle);
                        }
                    }
                }
            else
                foreach (KeyValuePair<Regex, string> typo in Typos)
                    FixTypo(ref articleText, ref summary, typo, articleTitle);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class RegExTypoFix
    {
        private readonly BackgroundRequest TypoThread;
        public event BackgroundRequestComplete Complete;

        private readonly ITyposProvider Source;

        /// <summary>
        /// Default constructor, typos will be loaded on a new thread from the default location
        /// </summary>
        public RegExTypoFix()
            : this(true, new TyposDownloader())
        {
        }

        /// <summary>
        /// Constructs an object that will load typos from the specified source in a separate thread
        /// </summary>
        /// <param name="provider">Typos provider to use</param>
        public RegExTypoFix(ITyposProvider provider)
            : this(true, provider)
        {
        }

        /// <summary>
        /// Constructs an object that 
        /// </summary>
        /// <param name="loadThreaded"></param>
        public RegExTypoFix(bool loadThreaded)
            : this(loadThreaded, new TyposDownloader())
        {
        }

        /// <summary>
        /// Default constructor, typos being loaded on separate thread is optional
        /// </summary>
        /// <param name="loadThreaded">Whether to load typos on a new thread</param>
        /// <param name="provider">Typos provider to use</param>
        public RegExTypoFix(bool loadThreaded, ITyposProvider provider)
        {
            Source = provider;
            if (!loadThreaded)
            {
                MakeRegexes();
                return;
            }

            TypoThread = new BackgroundRequest(Complete);
            TypoThread.Execute(MakeRegexes);
        }

        /// <summary>
        /// Creates a RETF rule
        /// </summary>
        public static string CreateRule(string find, string replace, string name)
        {
            return "<Typo word=\"" + name + "\" find=\"" + find + "\" replace=\"" + replace + "\" />";
        }

        /// <summary>
        /// Creates a RETF rule
        /// </summary>
        public static string CreateRule(string find, string replace)
        {
            return CreateRule(find, replace, "<enter a name>");
        }

        /// <summary>
        /// Number of typos loaded
        /// </summary>
        public int TypoCount { get; private set; }

        /// <summary>
        /// Whether the typos have been loaded successfully
        /// </summary>
        public bool TyposLoaded { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        private static readonly Regex IgnoreRegex = new Regex("133t|-ology|\\(sic\\)|\\[sic\\]|\\[''sic''\\]|\\{\\{sic\\}\\}|spellfixno", RegexOptions.Compiled);
        static readonly Regex RemoveTail = new Regex(@"(\s|\n|\r|\*|#|:|⌊⌊⌊⌊M?\d*⌋⌋⌋⌋)*$", RegexOptions.Compiled);
        private readonly List<TypoGroup> Groups = new List<TypoGroup>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        internal static void TypoError(string error)
        {
            MessageBox.Show(error + "\r\n\r\nPlease visit the typo page at " + Variables.RetfPath +
                " and fix this error, then click 'File → Refresh status/typos' menu item to reload typos.",
                "RegexTypoFix error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// 
        /// </summary>
        private void MakeRegexes()
        {
            try
            {
                Groups.Clear();
                TypoCount = 0;

                Groups.Add(new TypoGroup(20, @"^\\b(.*)\\b$", @"[^\\]\\\d", @"\b", @"\b")); // 
                Groups.Add(new TypoGroup(5, null, @"[^\\]\\\d", "", ""));
                Groups.Add(new TypoGroup(1, null, null, "", ""));

                Dictionary<string, string> typoStrings = Source.GetTypos();

                TyposLoaded = typoStrings.Count > 0;

                if (TyposLoaded)
                {
                    foreach (KeyValuePair<string, string> rule in typoStrings)
                    {
                        foreach (TypoGroup grp in Groups)
                        {
                            if (grp.IsSuitableTypo(rule.Key))
                            {
                                grp.Add(rule.Key, rule.Value);
                                TypoCount++;
                                break;
                            }
                        }
                    }

                    foreach (TypoGroup grp in Groups)
                        grp.MakeGroups();
                }
            }
            catch (TypoException)
            {
                Groups.Clear();
                TypoCount = 0;
                TyposLoaded = false;
            }
            catch (Exception ex)
            {
                TyposLoaded = false;
                ErrorHandler.Handle(ex);
            }
            finally
            {
                if (Complete != null) Complete(TypoThread);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="noChange"></param>
        /// <param name="summary"></param>
        /// <param name="articleTitle">Title of the article</param>
        /// <returns></returns>
        public string PerformTypoFixes(string articleText, out bool noChange, out string summary, string articleTitle)
        {
            string originalArticleText = articleText;
            summary = "";
            if ((TypoCount == 0) || IgnoreRegex.IsMatch(articleText))
            {
                noChange = true;
                return articleText;
            }

            HideText removeText = new HideText(true, false, true);

            articleText = removeText.HideMore(articleText, true);

            //remove newlines, whitespace and hide tokens from bottom
            //to avoid running 2K regexps on them
            Match m = RemoveTail.Match(articleText);
            string tail = m.Value;
            if (!string.IsNullOrEmpty(tail)) articleText = articleText.Remove(m.Index);

            string originalText = articleText;
            string strSummary = "";

            foreach (TypoGroup grp in Groups)
            {
                grp.FixTypos(ref articleText, ref strSummary, articleTitle, originalArticleText);
            }

            noChange = (originalText == articleText);

            summary = Variables.TypoSummaryTag + strSummary.Trim();

            return removeText.AddBackMore(articleText + tail);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">Title of the article</param>
        /// <returns></returns>
        public bool DetectTypo(string articleText, string articleTitle)
        {
            bool noChange;
            string summary;

            PerformTypoFixes(articleText, out noChange, out summary, articleTitle);

            return !noChange;
        }

        /// <summary>
        /// Returns statistics of typos corrected in the past article
        /// </summary>
        public List<TypoStat> GetStatistics()
        {
            List<TypoStat> res = new List<TypoStat>();

            foreach (TypoGroup g in Groups)
            {
                res.AddRange(g.Statistics);
            }

            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<KeyValuePair<Regex, string>> GetTypos()
        {
            List<KeyValuePair<Regex, string>> lst = new List<KeyValuePair<Regex, string>>();

            foreach (TypoGroup grp in Groups)
            {
                lst.AddRange(grp.Typos);
            }

            return lst;
        }
    }

    /// <summary>
    /// Represents statistics for one typo rule
    /// </summary>
    public class TypoStat
    {
        public readonly string Find, Replace;
        public int Total, SelfMatches, FalsePositives;

        public TypoStatsListViewItem ListViewItem;

        public TypoStat(KeyValuePair<Regex, string> typo)
            : this(typo.Key.ToString(), typo.Value)
        { }

        public TypoStat(string find, string replace)
        {
            Find = find;
            Replace = replace;
        }

        #region Inherited from Object
        public override string ToString()
        {
            return Find + " → " + Replace;
        }

        public override int GetHashCode()
        {
            return Find.GetHashCode() + Replace.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            TypoStat item = (obj as TypoStat);

            if (item == null)
                return false;

            return ((item.Find == Find) && (item.Replace == Replace));
        }
        #endregion
    }

    [Serializable]
    public class TypoException : ApplicationException
    {
        public TypoException() { }

        public TypoException(string message)
            : base(message) { }

        public TypoException(string message, Exception inner)
            : base(message, inner) { }

        protected TypoException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
