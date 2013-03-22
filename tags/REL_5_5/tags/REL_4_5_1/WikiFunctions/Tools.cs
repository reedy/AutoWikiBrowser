/*

(C) 2007 Martin Richards
(C) 2007 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/

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
using System.Text;
using System.Net;
using System.Web;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WikiFunctions.Parse;

namespace WikiFunctions
{
    /// <summary>
    /// Contains constants for canonical namespace numbers
    /// </summary>
    public static class Namespace
    {
        public static readonly int Media = -2;
        public static readonly int Special = -1;
        public static readonly int Article = 0;
        public static readonly int Talk = 1;
        public static readonly int User = 2;
        public static readonly int UserTalk = 3;
        public static readonly int Project = 4;
        public static readonly int ProjectTalk = 5;
        public static readonly int File = 6;
        public static readonly int FileTalk = 7;
        public static readonly int MediaWiki = 8;
        public static readonly int MediaWikiTalk = 9;
        public static readonly int Template = 10;
        public static readonly int TemplateTalk = 11;
        public static readonly int Help = 12;
        public static readonly int HelpTalk = 13;
        public static readonly int Category = 14;
        public static readonly int CategoryTalk = 15;

        public static readonly int FirstCustom = 100;

        // Aliases

        public static readonly int Mainspace = Article;
        public static readonly int Image = File;
        public static readonly int ImageTalk = FileTalk;
    };


    /// <summary>
    /// Provides various tools as static methods, such as getting the html of a page
    /// </summary>
    public static class Tools
    {
        public delegate void SetProgress(int percent);

        // Not Covered
        /// <summary>
        /// Calculates the namespace index.
        /// TODO: doesn't recognise acceptable deviations such as "template:foo" or "Category : bar"
        /// </summary>
        public static int CalculateNS(string ArticleTitle)
        {
            if (!ArticleTitle.Contains(":"))
                return 0;

            foreach (KeyValuePair<int, string> k in Variables.Namespaces)
            {
                if (ArticleTitle.StartsWith(k.Value))
                    return k.Key;
            }

            foreach (KeyValuePair<int, List<string>> k in Variables.NamespaceAliases)
            {
                foreach (string s in k.Value)
                {
                    if (ArticleTitle.StartsWith(s))
                        return k.Key;
                }
            }

            //foreach(KeyValuePair<int, string> k in Variables.NamespacesCaseInsensitive)
            //{
            //    if (Regex.Match(ArticleTitle, k.Value).Success)
            //        return k.Key;
            //}

            foreach (KeyValuePair<int, string> k in Variables.CanonicalNamespaces)
            {
                if (ArticleTitle.StartsWith(k.Value))
                    return k.Key;
            }

            return 0;
        }

        /// <summary>
        /// Returns the version of WikiFunctions
        /// </summary>
        public static Version Version
        { get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version; } }

        /// <summary>
        /// Returns a String including the version of WikiFunctions
        /// </summary>
        public static string VersionString
        { get { return Version.ToString(); } }

        /// <summary>
        /// 
        /// </summary>
        public static string DefaultUserAgentString
        { get { return "WikiFunctions/" + VersionString; } }

        /// <summary>
        /// Displays the WikiFunctions About box
        /// </summary>
        public static void About()
        {
            new Controls.AboutBox().Show();
        }

        // Covered by ToolsTests.IsMainSpace()
        /// <summary>
        /// Tests title to make sure it is main space
        /// </summary>
        /// <param name="ArticleTitle">The title.</param>
        public static bool IsMainSpace(string ArticleTitle)
        {
            return (CalculateNS(ArticleTitle) == 0);
        }

        // Covered by ToolsTests.IsRedirect()
        /// <summary>
        /// Tests article to see if it is a redirect
        /// </summary>
        /// <param name="Text">The title.</param>
        public static bool IsRedirect(string Text)
        {
            return WikiRegexes.Redirect.IsMatch(FirstChars(Text, 512));
        }

        // Covered by ToolsTests.RedirectTarget()
        /// <summary>
        /// Gets the target of the redirect
        /// </summary>
        /// <param name="Text">Title of redirect target</param>
        public static string RedirectTarget(string Text)
        {
            Match m = WikiRegexes.Redirect.Match(FirstChars(Text, 512));
            return WikiDecode(m.Groups[1].Value).Trim();
        }

        // Covered by ToolsTests.IsWikimediaProject()
        /// <summary>
        /// Returns true if given project belongs to Wikimedia
        /// </summary>
        public static bool IsWikimediaProject(ProjectEnum p)
        {
            return (p != ProjectEnum.custom && p != ProjectEnum.wikia);
        }

        private static char[] InvalidChars = new[] {'[', ']', '{', '}', '|', '<', '>', '#'};

        // Covered by ToolsTests.InvalidChars()
        /// <summary>
        /// Tests article title to see if it is valid
        /// </summary>
        /// <param name="ArticleTitle">The title.</param>
        public static bool IsValidTitle(string ArticleTitle)
        {
            ArticleTitle = WikiDecode(ArticleTitle).Trim();
            if (ArticleTitle.Length == 0) return false;

            return (ArticleTitle.IndexOfAny(InvalidChars) < 0);
        }

        // Covered by ToolsTests.RemoveInvalidChars()
        /// <summary>
        /// Removes Invalid Characters from an Article ArticleTitle
        /// </summary>
        /// <param name="ArticleTitle">Article Title</param>
        /// <returns>Article ArticleTitle with no invalid characters</returns>
        public static string RemoveInvalidChars(string ArticleTitle)
        {
            int pos;
            while ((pos = ArticleTitle.IndexOfAny(InvalidChars)) >= 0)
                ArticleTitle = ArticleTitle.Remove(pos, 1);

            return ArticleTitle;
        }

        // Covered by ToolsTests.IsImportantNamespace()
        /// <summary>
        /// Tests title to make sure it is either main, image, category or template namespace.
        /// </summary>
        /// <param name="ArticleTitle">The title.</param>
        public static bool IsImportantNamespace(string ArticleTitle)
        {
            int i = CalculateNS(ArticleTitle);
            return (i == Namespace.Article || i == Namespace.File
                || i == Namespace.Template || i == Namespace.Category);
        }

        // Covered by ToolsTest.IsTalkPage()
        /// <summary>
        /// Tests title to make sure it is a talk page.
        /// </summary>
        /// <param name="ArticleTitle">The title.</param>
        public static bool IsTalkPage(string ArticleTitle)
        {
            return IsTalkPage(CalculateNS(ArticleTitle));
        }

        // Covered by ToolsTests.IsTalkPage
        /// <summary>
        /// Tests title to make sure it is a talk page.
        /// </summary>
        /// <param name="Key">The namespace key</param>
        public static bool IsTalkPage(int Key)
        {
            return (Key % 2 == 1);
        }

        /// <summary>
        /// Strips trailing colon from a namespace name, e.g. "User:" -> "User"
        /// </summary>
        /// <param name="ns">Namespace string to process</param>
        public static string StripNamespaceColon(string ns)
        {
            return ns.TrimEnd(':');
        }

        private static readonly Regex NormalizeColon = new Regex(@"\s*:$", RegexOptions.Compiled);

        // Covered by NamespaceFunctions.NormalizeNamespace()
        /// <summary>
        /// Normalizes a namespace string, but does not changes it to default namespace name
        /// </summary>
        public static string NormalizeNamespace(string ns, int nsId)
        {
            ns = WikiDecode(NormalizeColon.Replace(ns, ":"));
            if (Variables.Namespaces[nsId].Equals(ns, StringComparison.InvariantCultureIgnoreCase))
                return Variables.Namespaces[nsId];

            foreach (string s in Variables.NamespaceAliases[nsId])
            {
                if (s.Equals(ns, StringComparison.InvariantCultureIgnoreCase))
                    return s;
            }

            // fail
            return ns;
        }

        // Covered by ToolsTests.RegexMatchCount()
        /// <summary>
        /// Get the number of times the regex matches the input string
        /// </summary>
        /// <param name="regex">String to become a regex</param>
        /// <param name="input">Input string to search for matches</param>
        /// <returns>No. of Matches</returns>
        public static int RegexMatchCount(string regex, string input)
        {
            return RegexMatchCount(new Regex(regex), input);
        }

        // Covered by ToolsTests.RegexMatchCount()
        /// <summary>
        /// Get the number of times the regex matches the input string
        /// </summary>
        /// <param name="regex">Regex to try and match input against</param>
        /// <param name="input">Input string to search for matches</param>
        /// <param name="opts">Regex Options</param>
        /// <returns>No. of Matches</returns>
        public static int RegexMatchCount(string regex, string input, RegexOptions opts)
        {
            return RegexMatchCount(new Regex(regex, opts), input);
        }

        // Covered by ToolsTests.RegexMatchCount()
        /// <summary>
        /// Get the number of times the regex matches the input string
        /// </summary>
        /// <param name="regex">Regex to try and match input against</param>
        /// <param name="input">Input string to search for matches</param>
        /// <returns>No. of Matches</returns>
        public static int RegexMatchCount(Regex regex, string input)
        {
            return regex.Matches(input).Count;
        }

        // Covered by HumanCatKeyTests
        /// <summary>
        /// Returns Category key from article name e.g. "David Smith" returns "Smith, David".
        /// special case: "John Doe, Jr." turns into "Doe, Jonn Jr."
        /// http://en.wikipedia.org/wiki/Wikipedia:Categorization_of_people
        /// </summary>
        public static string MakeHumanCatKey(string Name)
        {
            Name = RemoveNamespaceString(Regex.Replace(RemoveDiacritics(Name), @"\(.*?\)$", "").Replace("'", "").Trim()).Trim();
            string origName = Name;
            if (!Name.Contains(" ") || Variables.LangCode == LangCodeEnum.uk) return origName;
            // ukwiki uses "Lastname Firstname Patronymic" convention, nothing more is needed

            string suffix = "";
            int pos = Name.IndexOf(',');

            // ruwiki has "Lastname, Firstname Patronymic" convention
            if (pos >= 0 && Variables.LangCode != LangCodeEnum.ru)
            {
                suffix = Name.Substring(pos + 1).Trim();
                Name = Name.Substring(0, pos).Trim();
            }

            int intLast = Name.LastIndexOf(" ") + 1;
            string lastName = Name.Substring(intLast).Trim();
            Name = Name.Remove(intLast).Trim();
            if (IsRomanNumber(lastName))
            {
                if (Name.Contains(" "))
                {
                    suffix += lastName;
                    intLast = Name.LastIndexOf(" ") + 1;
                    lastName = Name.Substring(intLast);
                    Name = Name.Remove(intLast).Trim();
                }
                else
                { //if (Suffix != "") {
                    // We have something like "Peter" "II" "King of Spain" (first/last/suffix), so return what we started with
                    // OR We have "Fred" "II", we don't want to return "II, Fred" so we must return "Fred II"
                    return origName;
                }
            }
            lastName = TurnFirstToUpper(lastName.ToLower());

            Name = (lastName + ", " + Name + ", " + suffix).Trim(" ,".ToCharArray());

            return Name;
        }

        // Covered by ToolsTests.RemoveNamespaceString()
        /// <summary>
        /// Returns a string with the namespace removed
        /// </summary>
        /// <returns></returns>
        public static string RemoveNamespaceString(string title)
        {
            return RemoveNamespaceString(new Article(title));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static string RemoveNamespaceString(Article a)
        {
            return a.NamespacelessName;
        }

        // Covered by ToolsTests.GetNamespaceString()
        /// <summary>
        /// Returns a string just including the namespace of the article
        /// </summary>
        /// <returns></returns>
        public static string GetNamespaceString(string title)
        {
            return GetNamespaceString(new Article(title));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static string GetNamespaceString(Article a)
        {
            int ns = a.NameSpaceKey;
            if (ns == 0)
                return "";

            return Variables.Namespaces[ns].Replace(":", "");
        }

        // Covered by ToolsTests.BasePageName()
        /// <summary>
        /// Works like MediaWiki's {{BASEPAGENAME}} by retrieving page's parent name
        /// </summary>
        /// <param name="title">Title to process</param>
        /// <returns>For input like "Namespace:Foo/Bar/Boz", returns "Foo"</returns>
        public static string BasePageName(string title)
        {
            title = RemoveNamespaceString(title);
            int i = title.IndexOf('/');
            if (i < 0)
                return title;
            
            return title.Substring(0, i);
        }

        // Covered by ToolsTests.SubPageName()
        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public static string SubPageName(string title)
        {
            title = RemoveNamespaceString(title);

            int i = title.LastIndexOf('/');
            if (i < 0)
                return title;
            
            return title.Substring(i + 1);
        }

        // Covered by ToolsTests.RomanNumbers()
        /// <summary>
        /// checks if given string represents a small Roman number
        /// </summary>
        public static bool IsRomanNumber(string s)
        {
            if (string.IsNullOrEmpty(s) || s.Length > 5) return false;
            foreach (char c in s)
            {
                if (c != 'I' && c != 'V' && c != 'X') return false;
            }
            return true;
        }

        const int MaxEditSummaryLength = 250; // in bytes

        // Covered by ToolsTests.TrimEditSummary()
        /// <summary>
        /// UNFINISHED
        /// </summary>
        /// <param name="summary"></param>
        /// <param name="awbAd"></param>
        /// <returns></returns>
        public static string TrimEditSummary(string summary, string awbAd)
        {
            byte[] buf = Encoding.UTF8.GetBytes(summary);
            byte[] adBuf = Encoding.UTF8.GetBytes(awbAd);

            if (buf.Length + adBuf.Length <= MaxEditSummaryLength)
                return summary + awbAd;



            return summary + awbAd;
        }

        /// <summary>
        /// Gets the HTML from the given web address.
        /// </summary>
        /// <param name="URL">The URL of the webpage.</param>
        /// <returns>The HTML.</returns>
        public static string GetHTML(string URL)
        {
            return GetHTML(URL, Encoding.UTF8);
        }

        /// <summary>
        /// Gets the HTML from the given web address.
        /// </summary>
        /// <param name="URL">The URL of the webpage.</param>
        /// <param name="Enc">The encoding to use.</param>
        /// <returns>The HTML.</returns>
        public static string GetHTML(string URL, Encoding Enc)
        {
            if (Globals.UnitTestMode) throw new Exception("You shouldn't access Wikipedia from unit tests");

            HttpWebRequest rq = Variables.PrepareWebRequest(URL); // Uses WikiFunctions' default UserAgent string

            HttpWebResponse response = (HttpWebResponse) rq.GetResponse();

            Stream stream = response.GetResponseStream();
            StreamReader sr = new StreamReader(stream, Enc);

            string text = sr.ReadToEnd();

            sr.Close();
            stream.Close();
            response.Close();

            return text;
        }

        /// <summary>
        /// Gets the wiki text of the given article.
        /// </summary>
        /// <param name="ArticleTitle">The name of the article.</param>
        /// <returns>The wiki text of the article.</returns>
        public static string GetArticleText(string ArticleTitle)
        {
            if (!IsValidTitle(ArticleTitle))
                return "";

            string url = Variables.GetPlainTextURL(ArticleTitle);
            try
            {
                return GetHTML(url, Encoding.UTF8);
            }
            catch
            {
                throw new Exception("There was a problem loading " + url + ", please make sure the page exists");
            }
        }

        [DllImport("user32.dll")]
        private static extern bool FlashWindow(IntPtr hwnd, bool bInvert);

        /// <summary>
        /// Flashes the given form in the taskbar
        /// </summary>
        public static void FlashWindow(Control window)
        {
            try
            {
                FlashWindow(window.Handle, true);
            }
            catch { }
        }

        // Not Covered
        /// <summary>
        /// Case-Insensitive String Comparison
        /// </summary>
        /// <param name="one">First String</param>
        /// <param name="two">Second String</param>
        /// <returns>If the strings are equal</returns>
        public static bool CaseInsensitiveStringCompare(string one, string two)
        {
            return (string.Compare(one, two, true) == 0);
        }

        // Partially Covered by ToolsTests.CaseInsensitive()
        /// <summary>
        /// Returns a regex case insensitive version of a string for the first letter only e.g. "Category" returns "[Cc]ategory"
        /// </summary>
        public static string CaseInsensitive(string input)
        {
            if (!string.IsNullOrEmpty(input) && char.IsLetter(input[0]) && (char.ToUpper(input[0]) != char.ToLower(input[0])))
            {
                input = input.Trim();
                // escaping breaks many places that alredy escape their data
                return "[" + char.ToUpper(input[0]) + char.ToLower(input[0]) + "]" + input.Remove(0, 1);
            }
            
            return input;
        }

        // Covered by ToolsTests.AllCaseInsensitive()
        /// <summary>
        /// Returns a regex case insensitive version of an entire string e.g. "Category" returns "[Cc][Aa][Tt][Ee][Gg][Oo][Rr][Yy]"
        /// </summary>
        public static string AllCaseInsensitive(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                input = input.Trim();
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i <= input.Length - 1; i++)
                {
                    if (char.IsLetter(input[i]))
                        builder.Append("[" + char.ToUpper(input[i]) + char.ToLower(input[i]) + "]");
                    else builder.Append(input[i]);
                }
                return builder.ToString();
            }
            
            return input;
        }

        // Covered by ToolsTests.ApplyKeyWords()
        /// <summary>
        /// Applies the key words "%%title%%" etc.
        /// </summary>
        /// http://meta.wikimedia.org/wiki/Help:Magic_words
        public static string ApplyKeyWords(string Title, string Text)
        {
            if (Text.Contains("%%"))
            {
                string titleEncoded = WikiEncode(Title);
                Text = Text.Replace("%%title%%", Title);
                Text = Text.Replace("%%titlee%%", titleEncoded);
                Text = Text.Replace("%%fullpagename%%", Title);
                Text = Text.Replace("%%fullpagenamee%%", titleEncoded);
                Text = Text.Replace("%%key%%", MakeHumanCatKey(Title));

                string titleNoNamespace = RemoveNamespaceString(Title);
                string basePageName = BasePageName(Title);
                string subPageName = SubPageName(Title);
                string Namespace = GetNamespaceString(Title);

                Text = Text.Replace("%%pagename%%", titleNoNamespace);
                Text = Text.Replace("%%pagenamee%%", WikiEncode(titleNoNamespace));

                Text = Text.Replace("%%basepagename%%", basePageName);
                Text = Text.Replace("%%basepagenamee%%", WikiEncode(basePageName));

                Text = Text.Replace("%%namespace%%", Namespace);
                Text = Text.Replace("%%namespacee%%", WikiEncode(Namespace));

                Text = Text.Replace("%%subpagename%%", subPageName);
                Text = Text.Replace("%%subpagenamee%%", WikiEncode(subPageName));

                // we need to use project's names, not user's
                //Text = Text.Replace("{{CURRENTDAY}}", DateTime.Now.Day.ToString());
                //Text = Text.Replace("{{CURRENTMONTHNAME}}", DateTime.Now.ToString("MMM"));
                //Text = Text.Replace("{{CURRENTYEAR}}", DateTime.Now.Year.ToString());

                Text = Text.Replace("%%server%%", Variables.URL);
                Text = Text.Replace("%%scriptpath%%", Variables.ScriptPath);
                Text = Text.Replace("%%servername%%", ServerName(Variables.URL));
            }

            return Text;
        }

        // Covered by ToolsTests.TurnFirstToUpper()
        /// <summary>
        /// Returns uppercase version of the string
        /// </summary>
        public static string TurnFirstToUpper(string input)
        {   //other projects don't always start with capital
            if (Variables.Project == ProjectEnum.wiktionary || string.IsNullOrEmpty(input))
                return input;

            return (char.ToUpper(input[0]) + input.Remove(0, 1));
        }

        // Covered by ToolsTests.TurnFirstToLower()
        /// <summary>
        /// Returns lowercase version of the string
        /// </summary>
        public static string TurnFirstToLower(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            return (char.ToLower(input[0]) + input.Remove(0, 1));
        }

        private static readonly Regex RegexWordCountTable = new Regex("\\{\\|.*?\\|\\}", RegexOptions.Compiled | RegexOptions.Singleline);

        // Covered by ToolsTests.WordCount()
        /// <summary>
        /// Returns word count of the string
        /// </summary>
        public static int WordCount(string Text)
        {
            Text = RegexWordCountTable.Replace(Text, "");
            Text = WikiRegexes.TemplateMultiLine.Replace(Text, " ");
            Text = WikiRegexes.Comments.Replace(Text, "");

            return WikiRegexes.RegexWordCount.Matches(Text).Count;
        }

        // Not Covered
        /// <summary>
        /// Returns number of interwiki links in the text
        /// </summary>
        public static int InterwikiCount(string text)
        {
            int count = 0;
            foreach (Match m in WikiRegexes.PossibleInterwikis.Matches(text))
            {
                if (SiteMatrix.Languages.Contains(m.Groups[1].Value.ToLower())) count++;
            }
            return count;
        }

        // Not Covered
        /// <summary>
        /// Returns the number of [[links]] in the string
        /// </summary>
        public static int LinkCount(string Text)
        { return WikiRegexes.WikiLinksOnly.Matches(Text).Count; }

        // Not Covered
        /// <summary>
        /// Removes underscores and wiki syntax from links
        /// </summary>
        public static string RemoveSyntax(string Text)
        {
            if (Text[0] == '#' || Text[0] == '*')
                Text = Text.Substring(1);

            Text = Text.Replace("_", " ").Trim();
            Text = Text.Trim('[', ']');

            return Text;
        }

        // Covered by ToolsTests.SplitToSections()
        /// <summary>
        /// Splits wikitext to sections
        /// </summary>
        /// <param name="ArticleText">Page text</param>
        /// <returns>Array of strings, each represents a section with its heading (if any)</returns>
        public static string[] SplitToSections(string ArticleText)
        {
            string[] lines = ArticleText.Split(new [] { "\r\n" }, StringSplitOptions.None);

            List<string> sections = new List<string>();
            StringBuilder section = new StringBuilder();

            foreach (string s in lines)
            {
                if (WikiRegexes.Heading.IsMatch(s))
                {
                    if (section.Length > 0)
                    {
                        sections.Add(section.ToString());
                        section.Length = 0;
                    }
                }
                section.Append(s);
                section.Append("\r\n");
            }
            if (section.Length > 0) sections.Add(section.ToString());

            return sections.ToArray();
        }

        // Covered by ToolsTests.RemoveMatches()
        /// <summary>
        /// Removes every matched pattern. To be used only if MatchCollection is needed for something else,
        /// otherwise Regex.Replace(foo, "") will be faster
        /// </summary>
        /// <param name="str">String to process</param>
        /// <param name="matches">Matches of a regex on this string</param>
        public static string RemoveMatches(string str, MatchCollection matches)
        {
            if (matches.Count == 0) return str;

            StringBuilder sb = new StringBuilder(str);

            for (int i = matches.Count - 1; i >= 0; i--)
            {
                sb.Remove(matches[i].Index, matches[i].Value.Length);
            }

            return sb.ToString();
        }

        // Not Covered
        /// <summary>
        /// Removes every matched pattern. To be used only if MatchCollection is needed for something else,
        /// otherwise Regex.Replace(foo, "") will be faster
        /// </summary>
        /// <param name="str">String to process</param>
        /// <param name="matches">List of matches of a regex on this string</param>
        public static string RemoveMatches(string str, IList<Match> matches)
        {
            if (matches.Count == 0) return str;

            StringBuilder sb = new StringBuilder(str);

            for (int i = matches.Count - 1; i >= 0; i--)
            {
                sb.Remove(matches[i].Index, matches[i].Value.Length);
            }

            return sb.ToString();
        }

        #region boring chars
        static readonly KeyValuePair<string, string>[] Diacritics = 
        {
                //Latin
                new KeyValuePair<string, string>("Á", "A"),
                new KeyValuePair<string, string>("á", "a"),
                new KeyValuePair<string, string>("Ć", "C"),
                new KeyValuePair<string, string>("ć", "c"),
                new KeyValuePair<string, string>("É", "E"),
                new KeyValuePair<string, string>("é", "e"),
                new KeyValuePair<string, string>("Í", "I"),
                new KeyValuePair<string, string>("í", "i"),
                new KeyValuePair<string, string>("Ĺ", "L"),
                new KeyValuePair<string, string>("ĺ", "l"),
                new KeyValuePair<string, string>("Ń", "N"),
                new KeyValuePair<string, string>("ń", "n"),
                new KeyValuePair<string, string>("Ó", "O"),
                new KeyValuePair<string, string>("ó", "o"),
                new KeyValuePair<string, string>("Ŕ", "R"),
                new KeyValuePair<string, string>("ŕ", "r"),
                new KeyValuePair<string, string>("Ś", "S"),
                new KeyValuePair<string, string>("ś", "s"),
                new KeyValuePair<string, string>("Ú", "U"),
                new KeyValuePair<string, string>("ú", "u"),
                new KeyValuePair<string, string>("Ý", "Y"),
                new KeyValuePair<string, string>("ý", "y"),
                new KeyValuePair<string, string>("Ź", "Z"),
                new KeyValuePair<string, string>("ź", "z"),
                new KeyValuePair<string, string>("À", "A"),
                new KeyValuePair<string, string>("à", "a"),
                new KeyValuePair<string, string>("È", "E"),
                new KeyValuePair<string, string>("è", "e"),
                new KeyValuePair<string, string>("Ì", "I"),
                new KeyValuePair<string, string>("ì", "i"),
                new KeyValuePair<string, string>("Ò", "O"),
                new KeyValuePair<string, string>("ò", "o"),
                new KeyValuePair<string, string>("Ù", "U"),
                new KeyValuePair<string, string>("ù", "u"),
                new KeyValuePair<string, string>("Â", "A"),
                new KeyValuePair<string, string>("â", "a"),
                new KeyValuePair<string, string>("Ĉ", "C"),
                new KeyValuePair<string, string>("ĉ", "c"),
                new KeyValuePair<string, string>("Ê", "E"),
                new KeyValuePair<string, string>("ê", "e"),
                new KeyValuePair<string, string>("Ĝ", "G"),
                new KeyValuePair<string, string>("ĝ", "g"),
                new KeyValuePair<string, string>("Ĥ", "H"),
                new KeyValuePair<string, string>("ĥ", "h"),
                new KeyValuePair<string, string>("Î", "I"),
                new KeyValuePair<string, string>("î", "i"),
                new KeyValuePair<string, string>("Ĵ", "J"),
                new KeyValuePair<string, string>("ĵ", "j"),
                new KeyValuePair<string, string>("Ô", "O"),
                new KeyValuePair<string, string>("ô", "o"),
                new KeyValuePair<string, string>("Ŝ", "S"),
                new KeyValuePair<string, string>("ŝ", "s"),
                new KeyValuePair<string, string>("Û", "U"),
                new KeyValuePair<string, string>("û", "u"),
                new KeyValuePair<string, string>("Ŵ", "W"),
                new KeyValuePair<string, string>("ŵ", "w"),
                new KeyValuePair<string, string>("Ŷ", "Y"),
                new KeyValuePair<string, string>("ŷ", "y"),
                new KeyValuePair<string, string>("Ä", "A"),
                new KeyValuePair<string, string>("ä", "a"),
                new KeyValuePair<string, string>("Ë", "E"),
                new KeyValuePair<string, string>("ë", "e"),
                new KeyValuePair<string, string>("Ï", "I"),
                new KeyValuePair<string, string>("ï", "i"),
                new KeyValuePair<string, string>("Ö", "O"),
                new KeyValuePair<string, string>("ö", "o"),
                new KeyValuePair<string, string>("Ü", "U"),
                new KeyValuePair<string, string>("ü", "u"),
                new KeyValuePair<string, string>("Ÿ", "Y"),
                new KeyValuePair<string, string>("ÿ", "y"),
                new KeyValuePair<string, string>("ß", "ss"),
                new KeyValuePair<string, string>("Ã", "A"),
                new KeyValuePair<string, string>("ã", "a"),
                new KeyValuePair<string, string>("Ẽ", "E"),
                new KeyValuePair<string, string>("ẽ", "e"),
                new KeyValuePair<string, string>("Ĩ", "I"),
                new KeyValuePair<string, string>("ĩ", "i"),
                new KeyValuePair<string, string>("Ñ", "N"),
                new KeyValuePair<string, string>("ñ", "n"),
                new KeyValuePair<string, string>("Õ", "O"),
                new KeyValuePair<string, string>("õ", "o"),
                new KeyValuePair<string, string>("Ũ", "U"),
                new KeyValuePair<string, string>("ũ", "u"),
                new KeyValuePair<string, string>("Ỹ", "Y"),
                new KeyValuePair<string, string>("ỹ", "y"),
                new KeyValuePair<string, string>("Ç", "C"),
                new KeyValuePair<string, string>("ç", "c"),
                new KeyValuePair<string, string>("Ģ", "G"),
                new KeyValuePair<string, string>("ģ", "g"),
                new KeyValuePair<string, string>("Ķ", "K"),
                new KeyValuePair<string, string>("ķ", "k"),
                new KeyValuePair<string, string>("Ļ", "L"),
                new KeyValuePair<string, string>("ļ", "l"),
                new KeyValuePair<string, string>("Ņ", "N"),
                new KeyValuePair<string, string>("ņ", "n"),
                new KeyValuePair<string, string>("Ŗ", "R"),
                new KeyValuePair<string, string>("ŗ", "r"),
                new KeyValuePair<string, string>("Ş", "S"),
                new KeyValuePair<string, string>("ş", "s"),
                new KeyValuePair<string, string>("Ţ", "T"),
                new KeyValuePair<string, string>("ţ", "t"),
                new KeyValuePair<string, string>("Đ", "D"),
                new KeyValuePair<string, string>("đ", "d"),
                new KeyValuePair<string, string>("Ů", "U"),
                new KeyValuePair<string, string>("ů", "u"),
                new KeyValuePair<string, string>("Ǎ", "A"),
                new KeyValuePair<string, string>("ǎ", "a"),
                new KeyValuePair<string, string>("Č", "C"),
                new KeyValuePair<string, string>("č", "c"),
                new KeyValuePair<string, string>("Ď", "D"),
                new KeyValuePair<string, string>("ď", "d"),
                new KeyValuePair<string, string>("Ě", "E"),
                new KeyValuePair<string, string>("ě", "e"),
                new KeyValuePair<string, string>("Ǐ", "I"),
                new KeyValuePair<string, string>("ǐ", "I"),
                new KeyValuePair<string, string>("Ľ", "L"),
                new KeyValuePair<string, string>("ľ", "l"),
                new KeyValuePair<string, string>("Ň", "N"),
                new KeyValuePair<string, string>("ň", "N"),
                new KeyValuePair<string, string>("Ǒ", "O"),
                new KeyValuePair<string, string>("ǒ", "o"),
                new KeyValuePair<string, string>("Ř", "R"),
                new KeyValuePair<string, string>("ř", "r"),
                new KeyValuePair<string, string>("Š", "S"),
                new KeyValuePair<string, string>("š", "s"),
                new KeyValuePair<string, string>("Ť", "T"),
                new KeyValuePair<string, string>("ť", "t"),
                new KeyValuePair<string, string>("Ǔ", "U"),
                new KeyValuePair<string, string>("ǔ", "u"),
                new KeyValuePair<string, string>("Ž", "Z"),
                new KeyValuePair<string, string>("ž", "z"),
                new KeyValuePair<string, string>("Ā", "A"),
                new KeyValuePair<string, string>("ā", "a"),
                new KeyValuePair<string, string>("Ē", "E"),
                new KeyValuePair<string, string>("ē", "e"),
                new KeyValuePair<string, string>("Ī", "I"),
                new KeyValuePair<string, string>("ī", "i"),
                new KeyValuePair<string, string>("Ō", "O"),
                new KeyValuePair<string, string>("ō", "o"),
                new KeyValuePair<string, string>("Ū", "U"),
                new KeyValuePair<string, string>("ū", "u"),
                new KeyValuePair<string, string>("Ȳ", "O"),
                new KeyValuePair<string, string>("ȳ", "o"),
                new KeyValuePair<string, string>("Ǣ", "E"),
                new KeyValuePair<string, string>("ǣ", "e"),
                new KeyValuePair<string, string>("ǖ", "u"),
                new KeyValuePair<string, string>("ǘ", "u"),
                new KeyValuePair<string, string>("ǚ", "u"),
                new KeyValuePair<string, string>("ǜ", "u"),
                new KeyValuePair<string, string>("Ă", "A"),
                new KeyValuePair<string, string>("ă", "a"),
                new KeyValuePair<string, string>("Ĕ", "E"),
                new KeyValuePair<string, string>("ĕ", "e"),
                new KeyValuePair<string, string>("Ğ", "G"),
                new KeyValuePair<string, string>("ğ", "g"),
                new KeyValuePair<string, string>("Ĭ", "I"),
                new KeyValuePair<string, string>("ĭ", "i"),
                new KeyValuePair<string, string>("Ŏ", "O"),
                new KeyValuePair<string, string>("ŏ", "o"),
                new KeyValuePair<string, string>("Ŭ", "U"),
                new KeyValuePair<string, string>("ŭ", "u"),
                new KeyValuePair<string, string>("Ċ", "C"),
                new KeyValuePair<string, string>("ċ", "c"),
                new KeyValuePair<string, string>("Ė", "E"),
                new KeyValuePair<string, string>("ė", "e"),
                new KeyValuePair<string, string>("Ġ", "G"),
                new KeyValuePair<string, string>("ġ", "g"),
                new KeyValuePair<string, string>("İ", "I"),
                new KeyValuePair<string, string>("ı", "i"),
                new KeyValuePair<string, string>("Ż", "Z"),
                new KeyValuePair<string, string>("ż", "z"),
                new KeyValuePair<string, string>("Ą", "A"),
                new KeyValuePair<string, string>("ą", "a"),
                new KeyValuePair<string, string>("Ę", "E"),
                new KeyValuePair<string, string>("ę", "e"),
                new KeyValuePair<string, string>("Į", "I"),
                new KeyValuePair<string, string>("į", "i"),
                new KeyValuePair<string, string>("Ǫ", "O"),
                new KeyValuePair<string, string>("ǫ", "o"),
                new KeyValuePair<string, string>("Ų", "U"),
                new KeyValuePair<string, string>("ų", "u"),
                new KeyValuePair<string, string>("Ḍ", "D"),
                new KeyValuePair<string, string>("ḍ", "d"),
                new KeyValuePair<string, string>("Ḥ", "H"),
                new KeyValuePair<string, string>("ḥ", "h"),
                new KeyValuePair<string, string>("Ḷ", "L"),
                new KeyValuePair<string, string>("ḷ", "l"),
                new KeyValuePair<string, string>("Ḹ", "L"),
                new KeyValuePair<string, string>("ḹ", "l"),
                new KeyValuePair<string, string>("Ṃ", "M"),
                new KeyValuePair<string, string>("ṃ", "m"),
                new KeyValuePair<string, string>("Ṇ", "N"),
                new KeyValuePair<string, string>("ṇ", "n"),
                new KeyValuePair<string, string>("Ṛ", "R"),
                new KeyValuePair<string, string>("ṛ", "r"),
                new KeyValuePair<string, string>("Ṝ", "R"),
                new KeyValuePair<string, string>("ṝ", "r"),
                new KeyValuePair<string, string>("Ṣ", "S"),
                new KeyValuePair<string, string>("ṣ", "s"),
                new KeyValuePair<string, string>("Ṭ", "T"),
                new KeyValuePair<string, string>("ṭ", "t"),
                new KeyValuePair<string, string>("Ł", "L"),
                new KeyValuePair<string, string>("ł", "l"),
                new KeyValuePair<string, string>("Ő", "O"),
                new KeyValuePair<string, string>("ő", "o"),
                new KeyValuePair<string, string>("Ű", "U"),
                new KeyValuePair<string, string>("ű", "u"),
                new KeyValuePair<string, string>("Ŀ", "L"),
                new KeyValuePair<string, string>("ŀ", "l"),
                new KeyValuePair<string, string>("Ħ", "H"),
                new KeyValuePair<string, string>("ħ", "h"),
                new KeyValuePair<string, string>("Ð", "D"),
                new KeyValuePair<string, string>("ð", "d"),
                new KeyValuePair<string, string>("Þ", "TH"),
                new KeyValuePair<string, string>("þ", "th"),
                new KeyValuePair<string, string>("Œ", "O"),
                new KeyValuePair<string, string>("œ", "o"),
                new KeyValuePair<string, string>("Æ", "E"),
                new KeyValuePair<string, string>("æ", "e"),
                new KeyValuePair<string, string>("Ø", "O"),
                new KeyValuePair<string, string>("ø", "o"),
                new KeyValuePair<string, string>("Å", "A"),
                new KeyValuePair<string, string>("å", "a"),
                new KeyValuePair<string, string>("Ə", "E"),
                new KeyValuePair<string, string>("ə", "e"),

                //Russian
                new KeyValuePair<string, string>("Ё", "Е"),
                new KeyValuePair<string, string>("ё", "е"),

            };
        #endregion

        // Not Covered
        /// <summary>
        /// substitutes characters with diacritics with their simple equivalents
        /// </summary>
        public static string RemoveDiacritics(string s)
        {
            foreach (KeyValuePair<string, string> p in Diacritics)
            {
                s = s.Replace(p.Key, p.Value);
            }
            return s;
        }

        /// <summary>
        /// Writes a message to the given file in the specified location.
        /// </summary>
        /// <param name="Message">The message to write.</param>
        /// <param name="File">The name of the file, e.g. "Log.txt".</param>
        /// <param name="append"></param>
        public static void WriteTextFileAbsolutePath(string Message, string File, bool append)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(File, append, Encoding.UTF8))
                {
                    writer.Write(Message);
                    writer.Close();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }

        /// <summary>
        /// Writes a message to the given file in the specified location.
        /// </summary>
        /// <param name="Message">The message to write.</param>
        /// <param name="File">The name of the file, e.g. "Log.txt".</param>
        /// <param name="append"></param>
        public static void WriteTextFileAbsolutePath(StringBuilder Message, string File, bool append)
        {
            WriteTextFileAbsolutePath(Message.ToString(), File, append);
        }

        /// <summary>
        /// Writes a message to the given file in the directory of the application.
        /// </summary>
        /// <param name="Message">The message to write.</param>
        /// <param name="File">The name of the file, e.g. "Log.txt".</param>
        /// <param name="append"></param>
        public static void WriteTextFile(string Message, string File, bool append)
        {
            WriteTextFileAbsolutePath(Message, Application.StartupPath + "\\" + File, append);
        }

        /// <summary>
        /// Writes a message to the given file in the directory of the application.
        /// </summary>
        /// <param name="Message">The message to write.</param>
        /// <param name="File">The name of the file, e.g. "Log.txt".</param>
        /// <param name="append"></param>
        public static void WriteTextFile(StringBuilder Message, string File, bool append)
        {
            WriteTextFileAbsolutePath(Message.ToString(), Application.StartupPath + "\\" + File, append);
        }

        // Not Covered
        /// <summary>
        /// Turns an HTML list into a wiki style list
        /// </summary>
        public static string HTMLListToWiki(string text, string bullet)
        {//converts wiki/html/plain text list to wiki formatted list, bullet should be * or #
            text = text.Replace("\r\n\r\n", "\r\n");
            text = Regex.Replace(text, "<br ?/?>", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "</?(ol|ul|li)>", "", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "^</?(ol|ul|li)>\r\n", "", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "^(<li>|\\:|\\*|#|\\(? ?[0-9]{1,3} ?\\)|[0-9]{1,3}\\.?)", "", RegexOptions.Multiline);
            return Regex.Replace(text, "^", bullet, RegexOptions.Multiline);
        }

        private static readonly System.Media.SoundPlayer sound = new System.Media.SoundPlayer();

        /// <summary>
        /// Beeps
        /// </summary>
        public static void Beep()
        {//public domain sounds from http://www.partnersinrhyme.com/soundfx/PDsoundfx/beep.shtml
            sound.Stream = Properties.Resources.beep1;
            sound.Play();
        }

        static bool bWriteDebug;
        /// <summary>
        /// Gets or sets value whether debug is enabled
        /// </summary>
        public static bool WriteDebugEnabled
        {
            get { return bWriteDebug; }
            set { bWriteDebug = value; }
        }

        /// <summary>
        /// Writes debug log message
        /// </summary>
        public static void WriteDebug(string Object, string Text)
        {
            if (!bWriteDebug)
                return;

            try
            {
                WriteTextFile(string.Format(
@"Object: {0}
Time: {1}
Message: {2}

", Object, DateTime.Now.ToLongTimeString(), Text), "Log.txt", true);
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }

        /// <summary>
        /// Checks whether given string is a valid IP address
        /// </summary>
        public static bool IsIP(string s)
        {
            IPAddress dummy;
            return IPAddress.TryParse(s, out dummy);
        }

        // Not Covered
        /// <summary>
        /// returns content of a given string that lies between two other strings
        /// </summary>
        public static string StringBetween(string source, string start, string end)
        {
            int startPos = source.IndexOf(start) + start.Length;
            int endPos = source.IndexOf(end);

            if (startPos >= 0 && endPos >= 0 && startPos <= endPos)
                return source.Substring(startPos, endPos - startPos);

            return "";
        }

        // Covered by ToolsTests.ReplacePartOfString()
        /// <summary>
        /// For disambiguation - replaces part of a string with another string
        /// </summary>
        /// <param name="source">String</param>
        /// <param name="position"></param>
        /// <param name="length"></param>
        /// <param name="replace"></param>
        /// <returns></returns>
        public static string ReplacePartOfString(string source, int position, int length, string replace)
        {
            return source.Substring(0, position) + replace + source.Substring(position + length);
        }

        // Covered by ToolsTests.FirstChars()
        /// <summary>
        /// Returns substring at the start of a given string
        /// </summary>
        /// <param name="str">String to process</param>
        /// <param name="count">Number of chars at the beginning of str to process</param>
        /// <returns>String of maximum count chars from the beginning of str</returns>
        public static string FirstChars(string str, int count)
        {
            return str.Length <= count ? str : str.Substring(0, count);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void OpenArticleInBrowser(string title)
        {
            OpenURLInBrowser(Variables.NonPrettifiedURL(title));
        }

        /// <summary>
        /// Error supressed URL opener in default browser
        /// </summary>
        public static void OpenURLInBrowser(string url)
        {
            try
            {
                System.Diagnostics.Process.Start(url);
            }
            catch { }
        }

        /// <summary>
        /// Forces the loading of a page in En.Wiki
        /// Used for 'static' links to the english wikipedia
        /// </summary>
        public static void OpenENArticleInBrowser(string title, bool userspace)
        {
            if (userspace)
                OpenURLInBrowser("http://en.wikipedia.org/wiki/User:" + WikiEncode(title));
            else
                OpenURLInBrowser("http://en.wikipedia.org/wiki/" + WikiEncode(title));
        }

        public static string GetENLinkWithSimpleSkinAndLocalLanguage(string Article)
        {
            return "http://en.wikipedia.org/w/index.php?title=" + WikiEncode(Article) + "&useskin=simple&uselang=" +
              Variables.LangCodeEnumString();
        }

        /// <summary>
        /// Opens the specified articles history in the browser
        /// </summary>
        public static void OpenArticleHistoryInBrowser(string title)
        {
            OpenURLInBrowser(Variables.GetArticleHistoryURL(title));
        }

        /// <summary>
        /// Opens the specified users talk page in the browser
        /// </summary>
        public static void OpenUserTalkInBrowser(string username)
        {
            OpenURLInBrowser(Variables.GetUserTalkURL(username));
        }

        /// <summary>
        /// Opens the current users talk page in the browser
        /// </summary>
        public static void OpenUserTalkInBrowser()
        {
            OpenURLInBrowser(Variables.GetUserTalkURL());
        }

        /// <summary>
        /// Opens the specified articles edit page
        /// </summary>
        public static void EditArticleInBrowser(string title)
        {
            OpenURLInBrowser(Variables.GetEditURL(title));
        }

        /// <summary>
        /// Add the specified article to the current users watchlist
        /// </summary>
        public static void WatchArticleInBrowser(string title)
        {
            OpenURLInBrowser(Variables.GetAddToWatchlistURL(title));
        }

        /// <summary>
        /// Removes the specified article from the current users watchlist
        /// </summary>
        public static void UnwatchArticleInBrowser(string title)
        {
            OpenURLInBrowser(Variables.GetRemoveFromWatchlistURL(title));
        }

        // Covered by ToolsTests.WikiEncode()
        /// <summary>
        /// Replaces spaces with underscores for article title names
        /// </summary>
        /// <param name="title"></param>
        public static string WikiEncode(string title)
        {
            return HttpUtility.UrlEncode(title.Replace(' ', '_')).Replace("%2f", "/").Replace("%3a", ":");
        }

        // Not Covered
        /// <summary>
        /// Decodes URL-encoded page titles into a normal string
        /// </summary>
        /// <param name="title">Page title to decode</param>
        public static string WikiDecode(string title)
        {
            return HttpUtility.UrlDecode(title).Replace('_', ' ');
        }

        // Covered by ToolsTests.RemoveHashFromPageTitle()
        /// <summary>
        /// Removes the # and text after from a page title. Some redirects redirect to sections, the API doesnt like this
        /// </summary>
        /// <param name="title">Page Title</param>
        /// <returns>Title without # and proceeding, if appropriate</returns>
        public static string RemoveHashFromPageTitle(string title)
        {
            return !title.Contains("#") ? title : (title.Substring(0, title.IndexOf('#')));
        }

        // Covered by ToolsTests.ServerName()
        /// <summary>
        /// Returns URL stripped of protocol and subdirectories, e.g. http://en.wikipedia.org/wiki/ --> en.wikipedia.org
        /// </summary>
        /// <param name="url">URL to process</param>
        public static string ServerName(string url)
        {
            int pos = url.IndexOf("://");
            if (pos >= 0)
                url = url.Substring(pos + 3);

            pos = url.IndexOf('/');
            if (pos >= 0)
                url = url.Substring(0, pos);

            return url;
        }

        static readonly Regex ExpandTemplatesRegex = new Regex("<expandtemplates>(.*?)</expandtemplates>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ArticleText">The text of the article</param>
        /// <param name="ArticleTitle">The title of the artlce</param>
        /// <param name="Regexes"></param>
        /// <param name="includeComment"></param>
        /// <returns></returns>
        public static string ExpandTemplate(string ArticleText, string ArticleTitle, Dictionary<Regex, string> Regexes, bool includeComment)
        {
            foreach (KeyValuePair<Regex, string> p in Regexes)
            {
                MatchCollection uses = p.Key.Matches(ArticleText);
                foreach (Match m in uses)
                {
                    string call = m.Value;

                    string expandUri = Variables.URLApi + "?action=expandtemplates&format=xml&title=" + WikiEncode(ArticleTitle) + "&text=" + HttpUtility.UrlEncode(call);
                    string result;

                    try
                    {
                        string respStr = GetHTML(expandUri);
                        Match m1 = ExpandTemplatesRegex.Match(respStr);
                        if (!m.Success) continue;
                        result = HttpUtility.HtmlDecode(m1.Groups[1].Value);
                    }
                    catch
                    {
                        continue;
                    }

                    bool skipArticle;
                    result = Parsers.Unicodify(result, out skipArticle);
                    if (includeComment)
                        result = result + "<!-- " + call + " -->";

                    ArticleText = ArticleText.Replace(call, result);
                }
            }

            return ArticleText;
        }

        // Covered by ToolsTests.GetTitleFromURL()
        /// <summary>
        /// Extracts page title from URL
        /// </summary>
        /// <param name="link">Link to process</param>
        /// <returns>Page title or null if failed</returns>
        public static string GetTitleFromURL(string link)
        {
            link = WikiRegexes.ExtractTitle.Match(link).Groups[1].Value;

            if (string.IsNullOrEmpty(link))
                return null;

            return WikiDecode(link);
        }

        // Not Covered
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string[] FirstToUpperAndRemoveHashOnArray(string[] input)
        {
            if (input == null)
                return null;

            for (int i = 0; i < input.Length; i++)
            {
                input[i] = TurnFirstToUpper(RemoveHashFromPageTitle(input[i].Trim('[', ']', ' ', '\t')));
            }
            return input;
        }

        #region Copy
        /// <summary>
        /// Copy selected items from a listbox
        /// </summary>
        /// <param name="box">The list box to copy from</param>
        public static void Copy(ListBox box)
        {
            try
            {
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < box.SelectedItems.Count; i++)
                {
                    builder.AppendLine(box.SelectedItems[i].ToString());
                }
                Clipboard.SetDataObject(builder.ToString().Trim(), true);
            }
            catch { }
        }

        /// <summary>
        /// Copy selected items from a listview
        /// </summary>
        /// <param name="view">The list view to copy from</param>
        public static void Copy(ListView view)
        {
            try
            {
                StringBuilder builder = new StringBuilder();
                foreach (ListViewItem a in view.SelectedItems)
                {
                    builder.AppendLine(a.Text);
                }
                Clipboard.SetDataObject(builder.ToString().Trim(), true);
            }
            catch { }
        }
        #endregion

        // Covered by ToolsTests.SplitLines()
        /// <summary>
        /// Splits a string of text to separate lines. Supports every line ending possible - CRLF, CR, LF
        /// </summary>
        /// <param name="source">String to split</param>
        /// <returns>Array of lines</returns>
        public static string[] SplitLines(string source)
        {
            char[] separators = new [] { '\r', '\n' };
            List<string> res = new List<string>();
            
            int pos = 0;
            int sourceLength = source.Length;

            while (pos < sourceLength)
            {
                int eol = source.IndexOfAny(separators, pos);
                string s;
                if (eol < 0)
                {
                    s = source.Substring(pos);
                    pos = sourceLength;
                }
                else
                {
                    s = source.Substring(pos, eol - pos);
                    char ch = source[eol];
                    eol++;
                    if (ch == '\r' && eol < sourceLength)
                    {
                        if (source[eol] == '\n') eol++;
                    }
                    pos = eol;
                }
                res.Add(s);
            }

            return res.ToArray();
        }

        // Covered by ToolsTests.FindDifference()
        /// <summary>
        /// Returns index of first character different between strings
        /// </summary>
        /// <param name="a">First string</param>
        /// <param name="b">Second string</param>
        public static int FirstDifference(string a, string b)
        {
            for (int i = 0; i < Math.Min(a.Length, b.Length); i++)
            {
                if (a[i] != b[i]) return i;
            }

            return Math.Min(a.Length, b.Length);
        }

        // TODO: remove in 4.5 or 5.0, whichever will be released next
        /// <summary>
        /// 
        /// </summary>
        public static void RegistryMigration()
        {
            try
            {
                const string regKey = "Software\\Wikipedia";

                Microsoft.Win32.RegistryKey wpReg = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(regKey);

                if (wpReg == null)
                    return;

                //RecentSettings
                Microsoft.Win32.RegistryKey reg = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(regKey + "\\AutoWikiBrowser");

                if (reg == null)
                    return;

                Profiles.AWBProfiles.MigrateProfiles();

                if (reg.ValueCount > 0)
                {
                    string s = reg.GetValue("RecentList", "").ToString();

                    string pluginLocation = "";
                    try
                    {
                        pluginLocation = reg.GetValue("RecentPluginLoadedLocation").ToString();
                    }
                    catch { }

                    reg = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\\AutoWikiBrowser");
                    reg.SetValue("RecentList", s);

                    if (!string.IsNullOrEmpty(pluginLocation))
                        reg.SetValue("RecentPluginLoadedLocation", pluginLocation);
                }

                //Delete old Registry Stuff
                if (wpReg.SubKeyCount == 1)
                    new Microsoft.VisualBasic.Devices.Computer().Registry.CurrentUser.DeleteSubKeyTree(regKey);
                else
                    new Microsoft.VisualBasic.Devices.Computer().Registry.CurrentUser.DeleteSubKeyTree(regKey + "\\AutoWikiBrowser");

            }
            catch
            { }
        }

        // Covered by NamespaceFunctions.ConvertToTalk()
        /// <summary>
        /// Turns an article into its associated talk page
        /// </summary>
        /// <param name="a">The Article</param>
        /// <returns>Article Title</returns>
        public static string ConvertToTalk(Article a)
        {
            if (IsTalkPage(a.NameSpaceKey))
                return a.Name;

            if (a.NameSpaceKey == Namespace.Article)
                return (Variables.Namespaces[Namespace.Talk] + a.Name);

            return Variables.Namespaces[a.NameSpaceKey + 1] + a.NamespacelessName;
        }

        // Covered by NamespaceFunctions.ToTalkOnList()
        /// <summary>
        /// Turns a list of articles into an list of the associated talk pages.
        /// </summary>
        /// <param name="list">The list of articles.</param>
        /// <returns>The list of the talk pages.</returns>
        public static List<Article> ConvertToTalk(List<Article> list)
        {
            List<Article> newList = new List<Article>(list.Count);

            foreach (Article a in list)
            {
                string s = ConvertToTalk(a);
                if (a.Equals(s))
                    newList.Add(a);
                else
                    newList.Add(new Article(s));
            }
            return newList;
        }

        // Covered by NamespaceFunctions.ConvertFromTalk()
        /// <summary>
        /// Turns a talk page into its associated article
        /// </summary>
        /// <param name="a">The Article</param>
        /// <returns>Article Title</returns>
        public static string ConvertFromTalk(Article a)
        {
            if (IsTalkPage(a.NameSpaceKey))
            {
                if (a.NameSpaceKey == 1)
                    return a.NamespacelessName;

                return Variables.Namespaces[a.NameSpaceKey - 1] + a.NamespacelessName;
            }
            return a.Name;
        }

        // Covered by NamespaceFunctions.FromTalkOnList
        /// <summary>
        /// Turns a list of talk pages into a list of the associated articles.
        /// </summary>
        /// <param name="list">The list of talk pages.</param>
        /// <returns>The list of articles.</returns>
        public static List<Article> ConvertFromTalk(List<Article> list)
        {
            List<Article> newList = new List<Article>(list.Count);

            foreach (Article a in list)
            {
                string s = ConvertFromTalk(a);
                if (a.Equals(s))
                    newList.Add(a);
                else
                    newList.Add(new Article(s));
            }
            return newList;
        }

        // Not Covered
        /// <summary>
        /// 
        /// </summary>
        /// <param name="UnfilteredArticles"></param>
        /// <returns></returns>
        public static List<Article> FilterSomeArticles(List<Article> UnfilteredArticles)
        {
            //Filter out articles which we definately do not want to edit and remove duplicates.
            List<Article> items = new List<Article>();

            foreach (Article a in UnfilteredArticles)
            {
                if (a.NameSpaceKey >= 0 && a.NameSpaceKey != 9 && a.NameSpaceKey != 8 && !a.Name.StartsWith("Commons:"))
                {
                    if (!items.Contains(a))
                        items.Add(a);
                }
            }
            return items;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="postvars"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string PostData(NameValueCollection postvars, string url)
        {
            //echo scripts which just print out the POST vars, handy for early stages of testing:
            //const string url = "http://www.cs.tut.fi/cgi-bin/run/~jkorpela/echo.cgi";
            //const string url = "http://www.tipjar.com/cgi-bin/test";

            HttpWebRequest rq = Variables.PrepareWebRequest(url);
            rq.Method = "POST";
            rq.ContentType = "application/x-www-form-urlencoded";

            Stream RequestStream = rq.GetRequestStream();
            byte[] data = Encoding.UTF8.GetBytes(BuildPostDataString(postvars));
            RequestStream.Write(data, 0, data.Length);
            RequestStream.Close();

            HttpWebResponse rs = (HttpWebResponse)rq.GetResponse();
            if (rs.StatusCode == HttpStatusCode.OK)
                return new StreamReader(rs.GetResponseStream()).ReadToEnd();
            
            throw new WebException(rs.StatusDescription, WebExceptionStatus.UnknownError);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="postvars"></param>
        /// <returns></returns>
        public static string BuildPostDataString(NameValueCollection postvars)
        {
            StringBuilder ret = new StringBuilder();
            for (int i = 0; i < postvars.Keys.Count; i++)
            {
                if (i > 0)
                    ret.Append("&");

                ret.Append(postvars.Keys[i] + "=" + HttpUtility.UrlEncode(postvars[postvars.Keys[i]]));
            }

            return ret.ToString();
        }

        /// <summary>
        /// Wrapper function for setting text to clipboard. Clears clipboard and waits before continuing
        /// </summary>
        /// <param name="text">Text to copy to clipboard</param>
        public static void CopyToClipboard(string text)
        {
            try
            {
                Clipboard.Clear();
                System.Threading.Thread.Sleep(50); // give it some time to clear
                Clipboard.SetText(text);
            }
            catch { }
        }

        /// <summary>
        /// Wrapper function for setting text to clipboard. Clears clipboard and waits before continuing
        /// </summary>
        /// <param name="data"></param>
        /// <param name="copy"></param>
        public static void CopyToClipboard(object data, bool copy)
        {
            try
            {
                Clipboard.Clear();
                System.Threading.Thread.Sleep(50); // give it some time to clear
                Clipboard.SetDataObject(data, copy);
            }
            catch { }
        }

        /// <summary>
        /// Wrapper for VisualBasic's Inputbox, so other projects don't have to reference Microsoft.VisualBasic
        /// </summary>
        /// <param name="Prompt">String displayed as the message</param>
        /// <param name="Title">Title of Input Box</param>
        /// <param name="DefaultResponse">Response if no other input provided</param>
        /// <param name="XPos">X Position of input box</param>
        /// <param name="YPos">Y Position of input box</param>
        /// <returns>A string with the contents of the input box</returns>
        public static string VBInputBox(string Prompt, string Title, string DefaultResponse, int XPos, int YPos)
        {
            return Microsoft.VisualBasic.Interaction.InputBox(Prompt, Title, DefaultResponse, XPos, YPos);
        }

        /// <summary>
        /// Wrapper for System.Windows.Forms.MessageBox.Show() - So things dont have to reference the Forms library
        /// </summary>
        /// <param name="message"></param>
        public static void MessageBox(string message)
        {
            System.Windows.Forms.MessageBox.Show(message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static int GetNumberFromUser(bool edits)
        {
            using (Controls.LevelNumber num = new Controls.LevelNumber(edits))
            {
                if (num.ShowDialog() != DialogResult.OK) return -1;
                return num.Levels;
            }
        }
    }
}
