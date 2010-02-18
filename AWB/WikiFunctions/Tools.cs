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
    /// Provides various tools as static methods, such as getting the html of a page
    /// </summary>
    public static class Tools
    {
        static Tools()
        {
            DefaultUserAgentString = string.Format("WikiFunctions/{0} ({1}; .NET CLR {2})",
                                                   VersionString,
                                                   Environment.OSVersion.VersionString,
                                                   Environment.Version);
        }

        public delegate void SetProgress(int percent);

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
        { get; private set; }

        /// <summary>
        /// Displays the WikiFunctions About box
        /// </summary>
        public static void About()
        {
            new Controls.AboutBox().Show();
        }

        // Covered by ToolsTests.IsRedirect()
        /// <summary>
        /// Tests article to see if it is a redirect
        /// </summary>
        /// <param name="text">The title.</param>
        public static bool IsRedirect(string text)
        {
            return (RedirectTarget(text).Length > 0);
        }

        // Covered by ToolsTests.RedirectTarget()
        /// <summary>
        /// Gets the target of the redirect
        /// </summary>
        /// <param name="text">Title of redirect target</param>
        public static string RedirectTarget(string text)
        {
            Match m = WikiRegexes.Redirect.Match(WikiRegexes.UnformattedText.Replace(FirstChars(text, 512), ""));
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

        private readonly static char[] InvalidChars = new[] { '[', ']', '{', '}', '|', '<', '>', '#' };

        // Covered by ToolsTests.IsValidTitle()
        /// <summary>
        /// Tests article title to see if it is valid
        /// </summary>
        /// <param name="articleTitle">The title.</param>
        public static bool IsValidTitle(string articleTitle)
        {
            articleTitle = WikiDecode(articleTitle).Trim();
            if (articleTitle.Length == 0) return false;

            if (articleTitle.IndexOfAny(InvalidChars) >= 0)
                return false;

            articleTitle = Parsers.CanonicalizeTitleAggressively(articleTitle);
            var a = new Article(articleTitle);
            var name = a.NamespacelessName;
            return name.Length > 0 && !name.StartsWith(":");
        }

        // Covered by ToolsTests.RemoveInvalidChars()
        /// <summary>
        /// Removes Invalid Characters from an Article articleTitle
        /// </summary>
        /// <param name="articleTitle">Article Title</param>
        /// <returns>Article articleTitle with no invalid characters</returns>
        public static string RemoveInvalidChars(string articleTitle)
        {
            int pos;
            while ((pos = articleTitle.IndexOfAny(InvalidChars)) >= 0)
                articleTitle = articleTitle.Remove(pos, 1);

            return articleTitle;
        }

        // Covered by ToolsTests.StripNamespaceColon()
        /// <summary>
        /// Strips trailing colon from a namespace name, e.g. "User:" -> "User"
        /// </summary>
        /// <param name="ns">Namespace string to process</param>
        public static string StripNamespaceColon(string ns)
        {
            return ns.TrimEnd(':');
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
        public static string MakeHumanCatKey(string name)
        {
            name = RemoveNamespaceString(Regex.Replace(RemoveDiacritics(name), @"\(.*?\)$", "").Replace("'", "").Trim()).Trim();
            string origName = name;
            if (!name.Contains(" ") || Variables.LangCode == "uk")
                return FixupDefaultSort(origName);
            // ukwiki uses "Lastname Firstname Patronymic" convention, nothing more is needed

            string suffix = "";
            int pos = name.IndexOf(',');

            // ruwiki has "Lastname, Firstname Patronymic" convention
            if (pos >= 0 && Variables.LangCode != "ru")
            {
                suffix = name.Substring(pos + 1).Trim();
                name = name.Substring(0, pos).Trim();
            }

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#Arabic_names
            // Arabic, Chinese names etc. don't use the "Lastname, Firstname" format, perferring "Full Name" format
            // find the most common of these names and use that format for them
            if (Regex.IsMatch(origName, @"(\b(Abd[au]ll?ah?|Ahmed|Mustaq|Merza|Kandah[a-z]*|Mohabet|Nasrat|Nazargul|Yasi[mn]|Husayn|Akram|M[ou]hamm?[ae]d\w*|Abd[eu]l|Razzaq|Adil|Anwar|Fahed|Habi[bdr]|Hafiz|Jawad|Hassan|Ibr[ao]him|Khal[ei]d|Karam|Majid|Mustafa|Rash[ie]d|Yusef|[Bb]in|Nasir|Aziz|Rahim|Kareem|Abu|Aminullah|Fahd|Fawaz|Ahmad|Rahman|Hasan|Nassar|A(?:zz|s)am|Jam[ai]l|Tariqe?|Yussef|Said|Wass?im|Wazir|Tarek|Umran|Mahmoud|Malik|Shoaib|Hizani|Abib|Raza|Salim|Iqbal|Saleh|Hajj|Brahim|Zahir|Wasm|Yo?usef|Yunis|Zakim|Shah|Yasser|Samil|Akh[dk]ar|Haji|Uthman|Khadr|Asiri|Rajab|Shakouri|Ishmurat|Anazi|Nahdi|Zaheed|Ramzi|Rasul|Muktar|Muhassen|Radhi|Rafat|Kadir|Zaman|Karim|Awal|Mahmud|Mohammon|Husein|Airat|Alawi|Ullah|Sayaf|Henali|Ismael|Salih|Mahnut|Faha|Hammad|Hozaifa|Ravil|Jehan|Abdah|Djamel|Sabir|Ruhani|Hisham|Rehman|Mesut|Mehdi|Lakhdar|Mourad|Fazal[a-z]*|Mukit|Jalil|Rustam|Jumm?a|Omar Ali)\b|(?:[bdfmtnrz]ullah|alludin|[hm]atulla|r[ao]llah|harudin|millah)\b|\b(?:Abd[aeu][lr]|Nazur| Al[- ][A-Z]| al-[A-Z]))"))
                return FixupDefaultSort(origName);

            int intLast = name.LastIndexOf(" ") + 1;
            string lastName = name.Substring(intLast).Trim();
            name = name.Remove(intLast).Trim();
            if (IsRomanNumber(lastName))
            {
                if (name.Contains(" "))
                {
                    suffix += lastName;
                    intLast = name.LastIndexOf(" ") + 1;
                    lastName = name.Substring(intLast);
                    name = name.Remove(intLast).Trim();
                }
                else
                { //if (Suffix != "") {
                    // We have something like "Peter" "II" "King of Spain" (first/last/suffix), so return what we started with
                    // OR We have "Fred" "II", we don't want to return "II, Fred" so we must return "Fred II"
                    return FixupDefaultSort(origName);
                }
            }
            lastName = TurnFirstToUpper(lastName.ToLower());

            name = (lastName + ", " + (name.Length > 0 ? name + ", " : "") + suffix).Trim(" ,".ToCharArray());

            // set correct casing
            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Correct_case_for_.25.25key.25.25

            return FixupDefaultSort(name);
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

        // Covered by ToolsTests.RemoveNamespaceString()
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

        // Covered by ToolsTests.GetNamespaceString()
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static string GetNamespaceString(Article a)
        {
            int ns = a.NameSpaceKey;
            return (ns == 0) ? "" : Variables.Namespaces[ns].Replace(":", "");
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
            return (i < 0) ? title : title.Substring(0, i);
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
            return (i < 0) ? title : title.Substring(i + 1);
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

        /// <summary>
        /// Gets the HTML from the given web address.
        /// </summary>
        /// <param name="url">The URL of the webpage.</param>
        /// <returns>The HTML.</returns>
        public static string GetHTML(string url)
        {
            return GetHTML(url, Encoding.UTF8);
        }

        /// <summary>
        /// Gets the HTML from the given web address.
        /// </summary>
        /// <param name="url">The URL of the webpage.</param>
        /// <param name="enc">The encoding to use.</param>
        /// <returns>The HTML.</returns>
        public static string GetHTML(string url, Encoding enc)
        {
            if (Globals.UnitTestMode) throw new Exception("You shouldn't access Wikipedia from unit tests");

            HttpWebRequest rq = Variables.PrepareWebRequest(url); // Uses WikiFunctions' default UserAgent string

            HttpWebResponse response = (HttpWebResponse)rq.GetResponse();

            Stream stream = response.GetResponseStream();
            StreamReader sr = new StreamReader(stream, enc);

            string text = sr.ReadToEnd();

            sr.Close();
            stream.Close();
            response.Close();

            return text;
        }

        #if !MONO
        [DllImport("user32.dll")]
        private static extern void FlashWindow(IntPtr hwnd, bool bInvert);

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
        #endif

        // Covered by ToolsTests.CaseInsensitiveStringCompare()
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
                // escaping breaks many places that already escape their data
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
            if (string.IsNullOrEmpty(input))
                return input;

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

        // Covered by ToolsTests.ApplyKeyWords()
        /// <summary>
        /// Applies the key words "%%title%%" etc.
        /// </summary>
        /// http://meta.wikimedia.org/wiki/Help:Magic_words
        public static string ApplyKeyWords(string title, string text)
        {
            if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(title) && text.Contains("%%"))
            {
                string titleEncoded = WikiEncode(title);
                text = text.Replace("%%title%%", title);
                text = text.Replace("%%titlee%%", titleEncoded);
                text = text.Replace("%%fullpagename%%", title);
                text = text.Replace("%%fullpagenamee%%", titleEncoded);
                text = text.Replace("%%key%%", MakeHumanCatKey(title));

                string titleNoNamespace = RemoveNamespaceString(title);
                string basePageName = BasePageName(title);
                string subPageName = SubPageName(title);
                string theNamespace = GetNamespaceString(title);

                text = text.Replace("%%pagename%%", titleNoNamespace);
                text = text.Replace("%%pagenamee%%", WikiEncode(titleNoNamespace));

                text = text.Replace("%%basepagename%%", basePageName);
                text = text.Replace("%%basepagenamee%%", WikiEncode(basePageName));

                text = text.Replace("%%namespace%%", theNamespace);
                text = text.Replace("%%namespacee%%", WikiEncode(theNamespace));

                text = text.Replace("%%subpagename%%", subPageName);
                text = text.Replace("%%subpagenamee%%", WikiEncode(subPageName));

                // we need to use project's names, not user's
                //text = text.Replace("{{CURRENTDAY}}", DateTime.Now.Day.ToString());
                //text = text.Replace("{{CURRENTMONTHNAME}}", DateTime.Now.ToString("MMM"));
                //text = text.Replace("{{CURRENTYEAR}}", DateTime.Now.Year.ToString());

                text = text.Replace("%%server%%", Variables.URL);
                text = text.Replace("%%scriptpath%%", Variables.ScriptPath);
                text = text.Replace("%%servername%%", ServerName(Variables.URL));
            }

            return text;
        }

        // Covered by ToolsTests.TurnFirstToUpper()
        /// <summary>
        /// Returns version of the string with first character in upper case but not on wiktionary
        /// </summary>
        public static string TurnFirstToUpper(string input)
        {   //other projects don't always start with capital
            if (Variables.Project == ProjectEnum.wiktionary || string.IsNullOrEmpty(input))
                return input;

            return TurnFirstToUpperNoProjectCheck(input);
        }

        // Covered by ToolsTests.TurnFirstToUpper()
        /// <summary>
        /// Returns version of the string with first character in upper case
        /// </summary>
        public static string TurnFirstToUpperNoProjectCheck(string input)
        {
            return (string.IsNullOrEmpty(input)) ? "" : (char.ToUpper(input[0]) + input.Remove(0, 1));
        }

        // Covered by ToolsTests.TurnFirstToLower()
        /// <summary>
        /// Returns version of the string with first character in lower case
        /// </summary>
        public static string TurnFirstToLower(string input)
        {
            return (string.IsNullOrEmpty(input)) ? "" : (char.ToLower(input[0]) + input.Remove(0, 1));
        }

        private static readonly Regex RegexWordCountTable = new Regex(@"\{\|.*?\|\}", RegexOptions.Compiled | RegexOptions.Singleline);

        // Covered by ToolsTests.WordCount()
        /// <summary>
        /// Returns word count of the string
        /// </summary>
        public static int WordCount(string text)
        {
            text = RegexWordCountTable.Replace(text, "");
            text = WikiRegexes.TemplateMultiline.Replace(text, " ");
            text = WikiRegexes.Comments.Replace(text, "");

            int words = 0;
            int i = 0;

            while (i < text.Length)
            {
                if (!char.IsLetterOrDigit(text[i]))
                {
                    do
                        i++;
                    while (i < text.Length && !char.IsLetterOrDigit(text[i]));
                }
                else
                {
                    words++;
                    do
                        i++ ;
                    while (i < text.Length && char.IsLetterOrDigit(text[i]));
                }
            }

            return words;
        }

        // Covered by ToolsTests.InterwikiCount
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

        // Covered by ToolsTests.LinkCountTests
        /// <summary>
        /// Returns the number of [[links]] in the string
        /// </summary>
        public static int LinkCount(string text)
        { return WikiRegexes.WikiLinksOnly.Matches(text).Count; }

        // Covered by ToolsTests.RemoveSyntax
        /// <summary>
        /// Removes underscores and wiki syntax from links
        /// </summary>
        public static string RemoveSyntax(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            if (text[0] == '#' || text[0] == '*')
                text = text.Substring(1);

            text = text.Replace("_", " ").Trim();
            text = text.Trim('[', ']');
            text = text.Replace(@"&amp;", @"&");
            text = text.Replace(@"&quot;", @"""");
            text = text.Replace(@"�", "");

            return text.TrimStart(':');
        }

        // Covered by ToolsTests.SplitToSections()
        /// <summary>
        /// Splits wikitext to sections
        /// </summary>
        /// <param name="articleText">Page text</param>
        /// <returns>Array of strings, each represents a section with its heading (if any)</returns>
        public static string[] SplitToSections(string articleText)
        {
            string[] lines = articleText.Split(new[] { "\r\n" }, StringSplitOptions.None);

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
        public static readonly KeyValuePair<string, string>[] Diacritics =
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
            new KeyValuePair<string, string>("ǐ", "i"),
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
            new KeyValuePair<string, string>("Ȳ", "Y"),
            new KeyValuePair<string, string>("ȳ", "y"),
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
            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#.22.C3.86.22_.E2.86.92_.22ae.22_not_.22e.22
            new KeyValuePair<string, string>("Æ", "AE"),
            new KeyValuePair<string, string>("æ", "ae"),
            new KeyValuePair<string, string>("Ø", "O"),
            new KeyValuePair<string, string>("ø", "o"),
            new KeyValuePair<string, string>("Å", "A"),
            new KeyValuePair<string, string>("å", "a"),
            new KeyValuePair<string, string>("Ə", "E"),
            new KeyValuePair<string, string>("ə", "e"),

            //Russian
            new KeyValuePair<string, string>("Ё", "Е"),
            new KeyValuePair<string, string>("ё", "е"),

            // new
            new KeyValuePair<string, string>("ộ", "o"),
            new KeyValuePair<string, string>("ầ", "a"),
            new KeyValuePair<string, string>("ơ", "o"),
            new KeyValuePair<string, string>("Ơ", "O"),
            new KeyValuePair<string, string>("ư", "u"),
            new KeyValuePair<string, string>("Ư", "U"),
            new KeyValuePair<string, string>("ề", "e"),
            new KeyValuePair<string, string>("ứ", "u"),
            new KeyValuePair<string, string>("ṅ", "n"),
            new KeyValuePair<string, string>("ẏ", "y"),
            new KeyValuePair<string, string>("ḻ", "l"),
            new KeyValuePair<string, string>("ṟ", "r"),
            new KeyValuePair<string, string>("ṉ", "n"),
            new KeyValuePair<string, string>("ƀ", "b"),
            new KeyValuePair<string, string>("ƌ", "d"),
            new KeyValuePair<string, string>("ƒ", "f"),
            new KeyValuePair<string, string>("ƙ", "k"),
            new KeyValuePair<string, string>("ƚ", "l"),
            new KeyValuePair<string, string>("ƞ", "n"),
            new KeyValuePair<string, string>("ơ", "o"),
            new KeyValuePair<string, string>("ƥ", "p"),
            new KeyValuePair<string, string>("ƫ", "t"),
            new KeyValuePair<string, string>("ƭ", "t"),
            new KeyValuePair<string, string>("ư", "u"),
            new KeyValuePair<string, string>("ƴ", "y"),
            new KeyValuePair<string, string>("ƶ", "z"),
            new KeyValuePair<string, string>("ǎ", "a"),
            new KeyValuePair<string, string>("ǒ", "o"),
            new KeyValuePair<string, string>("ǔ", "u"),
            new KeyValuePair<string, string>("ǖ", "u"),
            new KeyValuePair<string, string>("ǘ", "u"),
            new KeyValuePair<string, string>("ǚ", "u"),
            new KeyValuePair<string, string>("ǜ", "u"),
            new KeyValuePair<string, string>("ǟ", "a"),
            new KeyValuePair<string, string>("ǡ", "a"),
            new KeyValuePair<string, string>("ǥ", "g"),
            new KeyValuePair<string, string>("ǧ", "g"),
            new KeyValuePair<string, string>("ǩ", "k"),
            new KeyValuePair<string, string>("ǫ", "o"),
            new KeyValuePair<string, string>("ợ", "o"),
            new KeyValuePair<string, string>("ố", "o"),
            new KeyValuePair<string, string>("ǭ", "o"),
            new KeyValuePair<string, string>("ǰ", "j"),
            new KeyValuePair<string, string>("ǵ", "g"),
            new KeyValuePair<string, string>("ǹ", "n"),
            new KeyValuePair<string, string>("ǻ", "a"),
            new KeyValuePair<string, string>("ǿ", "o"),
            new KeyValuePair<string, string>("ȁ", "a"),
            new KeyValuePair<string, string>("ȃ", "a"),
            new KeyValuePair<string, string>("ȅ", "e"),
            new KeyValuePair<string, string>("ȇ", "e"),
            new KeyValuePair<string, string>("ȉ", "i"),
            new KeyValuePair<string, string>("ȋ", "i"),
            new KeyValuePair<string, string>("ȍ", "o"),
            new KeyValuePair<string, string>("ȏ", "o"),
            new KeyValuePair<string, string>("ȑ", "r"),
            new KeyValuePair<string, string>("ȓ", "r"),
            new KeyValuePair<string, string>("ȕ", "u"),
            new KeyValuePair<string, string>("ȗ", "u"),
            new KeyValuePair<string, string>("ș", "s"),
            new KeyValuePair<string, string>("ț", "t"),
            new KeyValuePair<string, string>("ȟ", "h"),
            new KeyValuePair<string, string>("ȡ", "d"),
            new KeyValuePair<string, string>("ȥ", "z"),
            new KeyValuePair<string, string>("ȧ", "a"),
            new KeyValuePair<string, string>("ȩ", "e"),
            new KeyValuePair<string, string>("ȫ", "o"),
            new KeyValuePair<string, string>("ȭ", "o"),
            new KeyValuePair<string, string>("ȯ", "o"),
            new KeyValuePair<string, string>("ȱ", "o"),
            new KeyValuePair<string, string>("ȴ", "l"),
            new KeyValuePair<string, string>("ȵ", "n"),
            new KeyValuePair<string, string>("ȶ", "t"),
            new KeyValuePair<string, string>("ȼ", "c"),
            new KeyValuePair<string, string>("ȿ", "s"),
            new KeyValuePair<string, string>("ɀ", "z"),
            new KeyValuePair<string, string>("Ɖ", "D"),
            new KeyValuePair<string, string>("Ɗ", "D"),
            new KeyValuePair<string, string>("Ƌ", "D"),
            new KeyValuePair<string, string>("Ǝ", "E"),
            new KeyValuePair<string, string>("Ɛ", "E"),
            new KeyValuePair<string, string>("Ƒ", "F"),
            new KeyValuePair<string, string>("Ɠ", "G"),
            new KeyValuePair<string, string>("Ɨ", "I"),
            new KeyValuePair<string, string>("Ƙ", "K"),
            new KeyValuePair<string, string>("Ɯ", "M"),
            new KeyValuePair<string, string>("Ɲ", "N"),
            new KeyValuePair<string, string>("Ɵ", "O"),
            new KeyValuePair<string, string>("Ƥ", "P"),
            new KeyValuePair<string, string>("Ƭ", "T"),
            new KeyValuePair<string, string>("Ʈ", "T"),
            new KeyValuePair<string, string>("Ʋ", "V"),
            new KeyValuePair<string, string>("Ƴ", "Y"),
            new KeyValuePair<string, string>("Ƶ", "Z"),
            new KeyValuePair<string, string>("ǅ", "D"),
            new KeyValuePair<string, string>("ǈ", "L"),
            new KeyValuePair<string, string>("ǋ", "N"),
            new KeyValuePair<string, string>("Ǖ", "U"),
            new KeyValuePair<string, string>("Ǘ", "U"),
            new KeyValuePair<string, string>("Ǚ", "U"),
            new KeyValuePair<string, string>("Ǜ", "U"),
            new KeyValuePair<string, string>("ǝ", "e"),
            new KeyValuePair<string, string>("Ǟ", "A"),
            new KeyValuePair<string, string>("Ǡ", "A"),
            new KeyValuePair<string, string>("Ǥ", "G"),
            new KeyValuePair<string, string>("Ǧ", "G"),
            new KeyValuePair<string, string>("Ǩ", "K"),
            new KeyValuePair<string, string>("Ǭ", "O"),
            new KeyValuePair<string, string>("ǲ", "D"),
            new KeyValuePair<string, string>("Ǵ", "G"),
            new KeyValuePair<string, string>("Ǹ", "N"),
            new KeyValuePair<string, string>("Ǻ", "A"),
            new KeyValuePair<string, string>("Ǿ", "O"),
            new KeyValuePair<string, string>("Ȁ", "A"),
            new KeyValuePair<string, string>("Ȃ", "A"),
            new KeyValuePair<string, string>("Ȅ", "E"),
            new KeyValuePair<string, string>("Ȇ", "E"),
            new KeyValuePair<string, string>("Ȉ", "I"),
            new KeyValuePair<string, string>("Ȋ", "I"),
            new KeyValuePair<string, string>("Ȍ", "O"),
            new KeyValuePair<string, string>("Ȏ", "O"),
            new KeyValuePair<string, string>("Ȑ", "R"),
            new KeyValuePair<string, string>("Ȓ", "R"),
            new KeyValuePair<string, string>("Ȕ", "U"),
            new KeyValuePair<string, string>("Ȗ", "U"),
            new KeyValuePair<string, string>("Ș", "S"),
            new KeyValuePair<string, string>("Ț", "T"),
            new KeyValuePair<string, string>("Ȟ", "H"),
            new KeyValuePair<string, string>("Ƞ", "N"),
            new KeyValuePair<string, string>("Ȥ", "Z"),
            new KeyValuePair<string, string>("Ȧ", "A"),
            new KeyValuePair<string, string>("Ȩ", "E"),
            new KeyValuePair<string, string>("Ȫ", "O"),
            new KeyValuePair<string, string>("Ȭ", "O"),
            new KeyValuePair<string, string>("Ȯ", "O"),
            new KeyValuePair<string, string>("Ȱ", "O"),
            new KeyValuePair<string, string>("Ⱥ", "A"),
            new KeyValuePair<string, string>("Ȼ", "C"),
            new KeyValuePair<string, string>("Ƚ", "L"),
            new KeyValuePair<string, string>("Ⱦ", "T"),
            new KeyValuePair<string, string>("Ⱡ", "L"),
            new KeyValuePair<string, string>("ⱡ", "l"),
            new KeyValuePair<string, string>("Ɫ", "L"),
            new KeyValuePair<string, string>("Ᵽ", "P"),
            new KeyValuePair<string, string>("Ɽ", "R"),
            new KeyValuePair<string, string>("ⱥ", "a"),
            new KeyValuePair<string, string>("ⱦ", "t"),
            new KeyValuePair<string, string>("Ⱨ", "H"),
            new KeyValuePair<string, string>("ⱨ", "h"),
            new KeyValuePair<string, string>("Ⱪ", "K"),
            new KeyValuePair<string, string>("ⱪ", "k"),
            new KeyValuePair<string, string>("Ⱬ", "Z"),
            new KeyValuePair<string, string>("ⱬ", "z"),
            new KeyValuePair<string, string>("ⱴ", "v"),
            new KeyValuePair<string, string>("ớ", "o"),

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#Leaving_foreign_characters_in_DEFAULTSORT
            new KeyValuePair<string, string>("ắ", "a"),
            new KeyValuePair<string, string>("ạ", "a"),
            new KeyValuePair<string, string>("ả", "a"),
            new KeyValuePair<string, string>("ằ", "a"),
            new KeyValuePair<string, string>("ẩ", "a"),
            new KeyValuePair<string, string>("ế", "e"),
            new KeyValuePair<string, string>("ễ", "e"),
            new KeyValuePair<string, string>("ệ", "e"),
            new KeyValuePair<string, string>("ị", "i"),
            new KeyValuePair<string, string>("ỉ", "i"),
            new KeyValuePair<string, string>("ỏ", "o"),
            new KeyValuePair<string, string>("ø", "o"),
            new KeyValuePair<string, string>("ờ", "o"),
            new KeyValuePair<string, string>("ồ", "o"),
            new KeyValuePair<string, string>("ụ", "u"),
            new KeyValuePair<string, string>("ủ", "u"),
            new KeyValuePair<string, string>("ữ", "u"),
            new KeyValuePair<string, string>("ỳ", "y"),
        };
        #endregion

        // Covered by HumanCatKeyTests.RemoveDiacritics()
        /// <summary>
        /// substitutes characters with diacritics with their Latin equivalents
        /// </summary>
        public static string RemoveDiacritics(string s)
        {
            foreach (KeyValuePair<string, string> p in Diacritics)
            {
                s = s.Replace(p.Key, p.Value);
            }
            return s;
        }

        // Covered by HumanCatKeyTests.HasDiacritics
        /// <summary>
        /// Returns whether the given string contains recognised diacritics
        /// </summary>
        public static bool HasDiacritics(string s)
        {
            return s != RemoveDiacritics(s);
        }

        private static readonly Regex BadDsChars = new Regex("[\"]");

        //Covered by HumanCatKeyTests.FixUpDefaultSortTests()
        /// <summary>
        /// Removes recognised diacritics and double quotes, converts to Proper Case per [[WP:CAT]]
        /// </summary>
        public static string FixupDefaultSort(string s)
        {
            s = BadDsChars.Replace(RemoveDiacritics(s), "");

            // convert each word to Proper Case
            // http://en.wikipedia.org/wiki/Wikipedia:Categorization#Using_sort_keys
            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#DEFAULTSORT_capitalization_after_apostrophes
            foreach (Match m in WikiRegexes.RegexWordApostrophes.Matches(s))
            {
                s = s.Remove(m.Index, m.Length);

                s = s.Insert(m.Index, TurnFirstToUpper(m.Value.ToLower()));
            }

            return s;
        }

        /// <summary>
        /// Writes a message to the given file in the specified location.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="file">The name of the file, e.g. "Log.txt".</param>
        /// <param name="append"></param>
        public static void WriteTextFileAbsolutePath(string message, string file, bool append)
        {
            using (StreamWriter writer = new StreamWriter(file, append, Encoding.UTF8))
            {
                writer.Write(message);
                writer.Close();
            }
        }

        /// <summary>
        /// Writes a message to the given file in the specified location.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="file">The name of the file, e.g. "Log.txt".</param>
        /// <param name="append"></param>
        public static void WriteTextFileAbsolutePath(StringBuilder message, string file, bool append)
        {
            WriteTextFileAbsolutePath(message.ToString(), file, append);
        }

        /// <summary>
        /// Writes a message to the given file in the directory of the application.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="file">The name of the file, e.g. "Log.txt".</param>
        /// <param name="append"></param>
        public static void WriteTextFile(string message, string file, bool append)
        {
            if (file.Contains(":")) //If another drive, dont append startup path
                WriteTextFileAbsolutePath(message, file, append);
            else
                WriteTextFileAbsolutePath(message, Application.StartupPath + "\\" + file, append);
        }

        /// <summary>
        /// Writes a message to the given file in the directory of the application.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="file">The name of the file, e.g. "Log.txt".</param>
        /// <param name="append"></param>
        public static void WriteTextFile(StringBuilder message, string file, bool append)
        {
            WriteTextFileAbsolutePath(message.ToString(), Application.StartupPath + "\\" + file, append);
        }

        /// <summary>
        /// Turns an HTML list into a wiki style list using the input bullet style
        /// </summary>
        /// <param name="text">HTML text to convert to list</param>
        /// <param name="bullet">List style to use (# or *)</param>
        public static string HTMLListToWiki(string text, string bullet)
        {
            text = text.Replace("\r\n\r\n", "\r\n");
            text = Regex.Replace(text, "<br ?/?>", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "</?(ol|ul|li)>", "", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "^</?(ol|ul|li)>\r\n", "", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"^(\:|\*|#|\(? ?[0-9]{1,3}\b ?\)|[0-9]{1,3}\b\.?)", "", RegexOptions.Multiline);

            // add bullet to start of each line
            return Regex.Replace(text, "^", bullet, RegexOptions.Multiline);
        }

        private static readonly System.Media.SoundPlayer Sound = new System.Media.SoundPlayer();

        /// <summary>
        /// Beeps
        /// </summary>
        public static void Beep()
        {//public domain sounds from http://www.partnersinrhyme.com/soundfx/PDsoundfx/beep.shtml
            Sound.Stream = Properties.Resources.beep1;
            Sound.Play();
        }

        /// <summary>
        /// Gets or sets value whether debug is enabled
        /// </summary>
        public static bool WriteDebugEnabled;

        /// <summary>
        /// Writes debug log message
        /// </summary>
        public static void WriteDebug(string @object, string text)
        {
            if (!WriteDebugEnabled)
                return;

            try
            {
                WriteTextFile(string.Format(
                    @"object: {0}
Time: {1}
Message: {2}

", @object, DateTime.Now.ToLongTimeString(), text), "Log.txt", true);
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

        // Covered by ToolsTests.StringBetween
        /// <summary>
        /// returns content of a given string that lies between two other strings
        /// where there are multiple matches for one or more of the other strings, the shortest matching portion of the source string is returned
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

        // Covered by ToolsTests.ReplaceOnce()
        /// <summary>
        /// Replaces first occurence of a given text within a StringBuilder
        /// </summary>
        /// <param name="text">Text to be processed</param>
        /// <param name="oldValue">Text to be replaced</param>
        /// <param name="newValue">Replacement text</param>
        /// <returns>Whether the replacement has been made</returns>
        public static bool ReplaceOnce(StringBuilder text, string oldValue, string newValue)
        {
            int index = text.ToString().IndexOf(oldValue);
            if (index < 0)
                return false;

            text.Replace(oldValue, newValue, index, oldValue.Length);
            return true;
        }

        public static bool ReplaceOnce(ref string text, string oldValue, string newValue)
        {
            int index = text.IndexOf(oldValue);
            if (index < 0)
                return false;

            text = text.Remove(index, oldValue.Length);
            text = text.Insert(index, newValue);
            return true;
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
            return (str.Length <= count) ? str : str.Substring(0, count);
        }

        public static string ConvertToLocalLineEndings(string input)
        {
            return input.Replace("\n", Environment.NewLine);
        }

        public static string ConvertFromLocalLineEndings(string input)
        {
            return input.Replace(Environment.NewLine, "\n");
        }

        // TODO: should be replaced with SiteInfo.OpenPageInBrowser() wherever possible
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

        public static string GetENLinkWithSimpleSkinAndLocalLanguage(string article)
        {
            return "http://en.wikipedia.org/w/index.php?title=" + WikiEncode(article) + "&useskin=simple&uselang=" +
                Variables.LangCode;
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

        public static void OpenArticleLogInBrowser(string page)
        {
            OpenURLInBrowser(Variables.URLLong +
                             "index.php?title=Special:Log&type=&user=&page=" + page + "&year=&month=-1&tagfilter=&hide_patrol_log=1");
        }

        /// <summary>
        /// Opens the specified articles edit page
        /// </summary>
        public static void EditArticleInBrowser(string title)
        {
            OpenURLInBrowser(Variables.GetEditURL(title));
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
            return HttpUtility.UrlDecode(title.Replace("+", "%2B")).Replace('_', ' ');
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
            return new Uri(url).Host;
        }

        private static readonly Regex ExpandTemplatesRegex = new Regex(@"<expandtemplates[^\>]*>(.*?)</expandtemplates>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="articleText">The text of the article</param>
        /// <param name="articleTitle">The title of the artlce</param>
        /// <param name="regexes"></param>
        /// <param name="includeComment"></param>
        /// <returns></returns>
        public static string ExpandTemplate(string articleText, string articleTitle, Dictionary<Regex, string> regexes, bool includeComment)
        {
            foreach (KeyValuePair<Regex, string> p in regexes)
            {
                foreach (Match m in p.Key.Matches(articleText))
                {
                    string call = m.Value;

                    string expandUri = Variables.URLApi + "?action=expandtemplates&format=xml&title=" + WikiEncode(articleTitle) + "&text=" + HttpUtility.UrlEncode(call);
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
                    result = new Parsers().Unicodify(result, out skipArticle);
                    if (includeComment)
                        result = result + "<!-- " + call + " -->";

                    articleText = articleText.Replace(call, result);
                }
            }

            return articleText;
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

            return string.IsNullOrEmpty(link) ? null : WikiDecode(link);
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

        private const char ReturnLine = '\r', NewLine = '\n';
        private static readonly char[] Separators = new[] { ReturnLine, NewLine };

        // Covered by ToolsTests.SplitLines()
        /// <summary>
        /// Splits a string of text to separate lines. Supports every line ending possible - CRLF, CR, LF
        /// </summary>
        /// <param name="source">String to split</param>
        /// <returns>Array of lines</returns>
        public static string[] SplitLines(string source)
        {
            List<string> res = new List<string>();

            int pos = 0;
            int sourceLength = source.Length;

            while (pos < sourceLength)
            {
                int eol = source.IndexOfAny(Separators, pos);
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
                    if (ch == ReturnLine && eol < sourceLength)
                    {
                        if (source[eol] == NewLine) eol++;
                    }
                    pos = eol;
                }
                res.Add(s);
            }

            return res.ToArray();
        }

        //Not covered
        /// <summary>
        /// Returns a string containing textual representation of all given values
        /// </summary>
        /// <param name="separator">Separator between values</param>
        /// <param name="list">List of values to be converted to string and joined</param>
        public static string Join(string separator, params object[] list)
        {
            StringBuilder sb = new StringBuilder();

            foreach (object o in list)
            {
                if (sb.Length > 0) sb.Append(separator);
                sb.Append(o.ToString());
            }

            return sb.ToString();
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

        /// <summary>
        /// Turns an article into its associated talk page
        /// </summary>
        public static string ConvertToTalk(string a)
        {
            return ConvertToTalk(new Article(a));
        }

        // Covered by NamespaceFunctions.ConvertToTalk()
        /// <summary>
        /// Turns an article into its associated talk page
        /// </summary>
        /// <param name="a">The Article</param>
        /// <returns>Article Title</returns>
        public static string ConvertToTalk(Article a)
        {
            if (Namespace.IsSpecial(a.NameSpaceKey) || Namespace.IsTalk(a.NameSpaceKey))
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
                newList.Add(a.Equals(s) ? a : new Article(s));
            }
            return newList;
        }

        /// <summary>
        /// Turns a talk page into its associated article
        /// </summary>
        public static string ConvertFromTalk(string a)
        {
            return ConvertFromTalk(new Article(a));
        }

        // Covered by NamespaceFunctions.ConvertFromTalk()
        /// <summary>
        /// Turns a talk page into its associated article
        /// </summary>
        /// <param name="a">The Article</param>
        /// <returns>Article Title</returns>
        public static string ConvertFromTalk(Article a)
        {
            if (Namespace.IsTalk(a.NameSpaceKey))
            {
                if (a.NameSpaceKey == Namespace.Talk)
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
                newList.Add(a.Equals(s) ? a : new Article(s));
            }
            return newList;
        }

        // Covered by ToolsTests.FilterSomeArticles()
        /// <summary>
        /// Filter out articles which we definately do not want to edit and remove duplicates.
        /// (Filters MediaWiki (and talk) NS, Commons, and where NS is less than 0)
        /// </summary>
        /// <param name="unfilteredArticles">Original unfiltered article list</param>
        /// <returns>Filtered article list</returns>
        public static List<Article> FilterSomeArticles(List<Article> unfilteredArticles)
        {
            List<Article> items = new List<Article>();

            foreach (Article a in unfilteredArticles)
            {
                if (a.NameSpaceKey >= Namespace.Article && a.NameSpaceKey != Namespace.MediaWiki &&
                    a.NameSpaceKey != Namespace.MediaWikiTalk && !a.Name.StartsWith("Commons:"))
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

            if (Globals.UnitTestMode) throw new Exception("You shouldn't access Wikipedia from unit tests");

            HttpWebRequest rq = Variables.PrepareWebRequest(url);
            rq.Method = "POST";
            rq.ContentType = "application/x-www-form-urlencoded";

            Stream requestStream = rq.GetRequestStream();
            byte[] data = Encoding.UTF8.GetBytes(BuildPostDataString(postvars));
            requestStream.Write(data, 0, data.Length);
            requestStream.Close();

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
        /// <param name="prompt">String displayed as the message</param>
        /// <param name="title">Title of Input Box</param>
        /// <param name="defaultResponse">Response if no other input provided</param>
        /// <param name="xPos">X Position of input box</param>
        /// <param name="yPos">Y Position of input box</param>
        /// <returns>A string with the contents of the input box</returns>
        public static string VBInputBox(string prompt, string title, string defaultResponse, int xPos, int yPos)
        {
            return Microsoft.VisualBasic.Interaction.InputBox(prompt, title, defaultResponse, xPos, yPos);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="matches"></param>
        /// <returns></returns>
        public static string ReplaceWithSpaces(string input, MatchCollection matches)
        {
            StringBuilder sb = new StringBuilder(input.Length);
            foreach (Match m in matches)
            {
                sb.Append(input, sb.Length, m.Index - sb.Length);
                sb.Append(' ', m.Length);
            }
            sb.Append(input, sb.Length, input.Length - sb.Length);
            return sb.ToString();
        }

        /// <summary>
        /// Replaces all matches of a given regex in a string with space character
        /// </summary>
        /// <param name="input"></param>
        /// <param name="regex"></param>
        /// <returns></returns>
        public static string ReplaceWithSpaces(string input, Regex regex)
        {
            return ReplaceWithSpaces(input, regex.Matches(input));
        }

        // ensure dates returned are English.
        private static readonly System.Globalization.CultureInfo English = new System.Globalization.CultureInfo("en-GB");
        
        /// <summary>
        /// Returns the input ISO date in the requested format (American or International). If another Locale is pasased in the input date is returned. For en-wiki only.
        /// </summary>
        /// <param name="ISODate">string representing ISO date</param>
        /// <param name="locale">Locale of output date required (American or International)</param>
        /// <returns>The English-language (American or International) date</returns>
        public static string ISOToENDate(string ISODate, Parsers.DateLocale locale)
        {
        	if (Variables.LangCode != "en")
        		return ISODate;

        	DateTime dt;
        	
        	try
        	{
        		dt = Convert.ToDateTime(ISODate);
        	}
        	catch
        	{
        		return ISODate;
        	}

        	switch (locale)
        	{
        	    case Parsers.DateLocale.American:
        	        return dt.ToString("MMMM d, yyyy", English);
        	    case Parsers.DateLocale.International:
        	        return dt.ToString("d MMMM yyyy", English);
        	    default:
        	        return ISODate;
        	}
        }
        
        /// <summary>
        /// Appends the input parameter and value to the input template
        /// </summary>
        /// <param name="template">The input template</param>
        /// <param name="parameter">The input parameter name</param>
        /// <param name="value">The input parameter value</param>
        /// <returns>The updated template string</returns>
        public static string AppendParameterToTemplate(string template, string parameter, string value)
        {
            return WikiRegexes.TemplateEnd.Replace(template, @" | " + parameter + "=" + value + @"}}");
        }
        
        /// <summary>
        /// Returns the value of the input parameter in the input template
        /// </summary>
        /// <param name="template">the input template</param>
        /// <param name="parameter">the input parameter to find</param>
        /// <returns>The trimmed parameter value, or a null string if the parameter is not found</returns>
        public static string GetTemplateParameterValue(string template, string parameter)
        {
        	Regex param = new Regex(@"\|\s*" + Regex.Escape(parameter) + @"\s*=\s*((?:(?:\[\[[^{}]+?\|[^{}]+?\]\])?[^{}\|]*?(?:\[\[[^{}]+?\|[^{}]+?\]\])?)*)\s*(?:\||}})");

            return param.Match(template).Groups[1].Value.Trim();
        }
        
        public static string RenameTemplateParameter(string template, string oldparameter, string newparameter)
        {
        	Regex param = new Regex(@"(\|\s*)" + Regex.Escape(oldparameter) + @"(\s*=)", RegexOptions.Compiled);
        	
        	return (param.Replace(template, "$1" + newparameter + "$2"));        	
        }
        
        /// <summary>
        /// Removes the input parameter from all instances of the input template in the article text
        /// </summary>
        /// <param name="articletext"></param>
        /// <param name="templatename"></param>
        /// <param name="parameter"></param>
        /// <returns>The updated article text</returns>
        public static string RemoveTemplateParameter(string articletext, string templatename, string parameter)
        {
            templatename = Regex.Escape(templatename);
            
            Regex oldtemplate = new Regex(@"{{\s*" + Tools.CaseInsensitive(templatename) +@"\s*(\|(?>[^\{\}]+|\{(?<DEPTH>)|\}(?<-DEPTH>))*(?(DEPTH)(?!)))?}}");
            
            foreach(Match m in oldtemplate.Matches(articletext))
            {
                string template = m.Value;
                articletext = articletext.Replace(template, RemoveTemplateParameter(template, parameter));
            }
            
            return articletext;
        }
        
        /// <summary>
        /// Removes the input parameter from the input template
        /// </summary>
        /// <param name="template"></param>
        /// <param name="parameter"></param>
        /// <returns>The updated template</returns>
        public static string RemoveTemplateParameter(string template, string parameter)
        {
            Regex param = new Regex(@"\|\s*" + Regex.Escape(parameter) + @"\s*=\s*(?:(?:(?:\[\[[^{}]+?\|[^{}]+?\]\])?[^{}\|]*?(?:\[\[[^{}]+?\|[^{}]+?\]\])?)*)\s*(?=\||}})");
            
            return(param.Replace(template, ""));
        }
        
        /// <summary>
        /// Renames all matches of the given template name in the input text to the new name given
        /// </summary>
        /// <param name="articletext">the page text</param>
        /// <param name="templatename">the old template name</param>
        /// <param name="newtemplatename">teh new template name</param>
        /// <returns></returns>
        public static string RenameTemplate(string articletext, string templatename, string newtemplatename)
        {
        	// handle underscores instead of spaces
        	templatename = templatename.Replace(" ", @"[_ ]");
        	
        	try
        	{
        		Regex oldtemplate = new Regex(@"({{\s*)" + Tools.CaseInsensitive(templatename) + @"(\s*(?:\||}}))");
        		
        		return oldtemplate.Replace(articletext, "$1" + newtemplatename + "$2");
        	}
        	
        	catch(Exception ex)
        	{
        		return articletext;
        	}
        	
        }

        /// <summary>
        /// returns true if testnode is the same or a subnode of refnode
        /// </summary>
        /// <param name="refnode"></param>
        /// <param name="testnode"></param>
        /// <returns></returns>
        public static bool IsSubnodeOf(TreeNode refnode, TreeNode testnode)
        {
            for (TreeNode t = testnode; t != null; t = t.Parent)
            {
                if (ReferenceEquals(refnode, t))
                    return true;
            }
            return false;
        }

        public static string ListToStringCommaSeparator(List<string> items)
        {
            return string.Join(", ", items.ToArray());
        }
    }
}
