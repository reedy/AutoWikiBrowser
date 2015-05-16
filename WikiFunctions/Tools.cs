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
using System.Globalization;
using System.Linq;
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

		// Covered by ToolsTests.IsRedirect()
		/// <summary>
		/// Tests article to see if it is a redirect
		/// </summary>
		/// <param name="articletext">The article text</param>
		public static bool IsRedirect(string articletext)
		{
			return (RedirectTarget(articletext).Length > 0);
		}

		private static readonly Regex SoftRedirect = NestedTemplateRegex(new[] {"Soft redirect", "SoftRedirect", "Soft Redirect", "Softredirect", "Softredir", "Soft link", "Soft redir", "Soft"});

		/// <summary>
		/// Tests article to see if it is a redirect OR a soft redirect using {{soft redirect}}
		/// </summary>
		/// <param name="articletext">The article text</param>
		public static bool IsRedirectOrSoftRedirect(string articletext)
		{
			return (RedirectTarget(articletext).Length > 0 || SoftRedirect.IsMatch(articletext));
		}

		// Covered by ToolsTests.RedirectTarget()
		/// <summary>
		/// Gets the target of the redirect
		/// </summary>
		/// <param name="articleText">The text of the article</param>
		public static string RedirectTarget(string articleText)
		{
			Match m = WikiRegexes.Redirect.Match(WikiRegexes.UnformattedText.Replace(FirstChars(articleText, 512), ""));
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

		private readonly static char[] InvalidChars = { '[', ']', '{', '}', '|', '<', '>', '#' };

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
			articleTitle = RemoveNamespaceString(articleTitle);
			return articleTitle.Length > 0 && !articleTitle.StartsWith(":");
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

		private static readonly Regex PersonOfPlace = new Regex(@"^(?<person>\w+)(?<ordinal> [IXV]+)? of (?<place>\w+)$", RegexOptions.Compiled);
		
		// Covered by HumanCatKeyTests
		/// <summary>
		/// Returns Category key from article name e.g. "David Smith" returns "Smith, David".
		/// special case: "John Doe, Jr." turns into "Doe, Jonn Jr."
		/// https://en.wikipedia.org/wiki/Wikipedia:Categorization_of_people
		/// </summary>
		public static string MakeHumanCatKey(string name, string articletext)
		{
			name = RemoveNamespaceString(Regex.Replace(CleanSortKey(name), @"\(.*?\)$", "").Replace("'", "").Trim()).Trim();

			string origName = name;

			// ukwiki uses "Lastname Firstname Patronymic" convention, nothing more is needed
			// if page has {{Chinese name}} etc. then family name is already first
			if (!name.Contains(" ") || Variables.LangCode.Equals("uk") || WikiRegexes.SurnameClarificationTemplates.IsMatch(articletext))
				return FixupDefaultSort(origName);

			string suffix = "";
			int pos = name.IndexOf(',');

			// ruwiki has "Lastname, Firstname Patronymic" convention
			if (pos >= 0 && Variables.LangCode != "ru")
			{
				suffix = name.Substring(pos + 1).Trim();
				name = name.Substring(0, pos).Trim();
			}

			// https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#Arabic_names
			// Arabic names etc. use "Full Name" format
			// find the most common of these names and use that format for them
			if (Regex.IsMatch(origName, @"(\b(Abd[au]ll?ah?|Ahmed|Mustaq|Merza|Kandah[a-z]*|Mohabet|Nasrat|Nazargul|Yasi[mn]|Husayn|Akram|M[ou]hamm?[ae]d\w*|Abd[eu]l|Razzaq|Adil|Anwar|Fahed|Habi[bdr]|Hafiz|Jawad|Hassan|Ibr[ao]him|Khal[ei]d|Karam|Majid|Mustafa|Rash[ie]d|Yusef|[Bb]in|Nasir|Aziz|Rahim|Kareem|Abu|Aminullah|Fahd|Fawaz|Ahmad|Rahman|Hasan|Nassar|A(?:zz|s)am|Jam[ai]l|Tariqe?|Yussef|Said|Wass?im|Wazir|Tarek|Umran|Mahmoud|Malik|Shoaib|Hizani|Abib|Raza|Salim|Iqbal|Saleh|Hajj|Brahim|Zahir|Wasm|Yo?usef|Yunis|Zakim|Shah|Yasser|Samil|Akh[dk]ar|Haji|Uthman|Khadr|Asiri|Rajab|Shakouri|Ishmurat|Anazi|Nahdi|Zaheed|Ramzi|Rasul|Muktar|Muhassen|Radhi|Rafat|Kadir|Zaman|Karim|Awal|Mahmud|Mohammon|Husein|Airat|Alawi|Ullah|Sayaf|Henali|Ismael|Salih|Mahnut|Faha|Hammad|Hozaifa|Ravil|Jehan|Abdah|Djamel|Sabir|Ruhani|Hisham|Rehman|Mesut|Mehdi|Lakhdar|Mourad|Fazal[a-z]*|Mukit|Jalil|Rustam|Jumm?a|Omar Ali)\b|(?:[bdfmtnrz]ullah|alludin|[hm]atulla|r[ao]llah|harudin|millah)\b|\b(?:Abd[aeu][lr]|Nazur| Al[- ][A-Z]| al-[A-Z]))"))
				return FixupDefaultSort(origName);
			
			// Person of Place --> Person Of Place, WP:NAMESORT
			if(PersonOfPlace.IsMatch(origName))
			{
				origName = PersonOfPlace.Replace(origName,
					m =>
						m.Groups["person"].Value +
						(m.Groups["ordinal"].Length > 0 ? " " + RomanToInt(m.Groups["ordinal"].Value) : "") + " of " +
						m.Groups["place"].Value);
				return FixupDefaultSort(origName);
			}

			int intLast = name.LastIndexOf(" ") + 1;
			string lastName = name.Substring(intLast).Trim();
			if(name.Length > 0)
				name = name.Remove(intLast).Trim();

			if (IsRomanNumber(lastName) || Regex.IsMatch(lastName, @"^[SJsj]n?r\.$"))
			{
				if (name.Contains(" "))
				{
					suffix += lastName;
					intLast = name.LastIndexOf(" ") + 1;
					lastName = name.Substring(intLast);
					name = name.Remove(intLast).Trim();
				}
				else
				{
					// We have something like "Peter" "II" "King of Spain" (first/last/suffix), so return what we started with
					// OR We have "Fred" "II", we don't want to return "II, Fred" so we must return "Fred II"
					return FixupDefaultSort(origName);
				}
			}

			name = (lastName + ", " + (name.Length > 0 ? name + ", " : "") + suffix).Trim(" ,".ToCharArray());

			// set correct casing
			return FixupDefaultSort(name);
		}
		
		/// <summary>
		/// Converts Roman numerals in the range I to CCCCXCIX to Arabic number
		/// </summary>
		/// <param name="Roman">Roman numerals</param>
		/// <returns>Abrabic number as string with leading zero for 1–9</returns>
		public static string RomanToInt(string Roman)
		{
			int converted = 0;

			if(Roman.Contains("IX"))
			{
				converted += 9;
				Roman = Roman.Replace("IX", "");
			}

			if(Roman.Contains("IV"))
			{
				converted += 4;
				Roman = Roman.Replace("IV", "");
			}

            if(Roman.Contains("XL"))
            {
                converted += 40;
                Roman = Roman.Replace("XL", "");
            }
            if(Roman.Contains("XC"))
            {
                converted += 90;
                Roman = Roman.Replace("XC", "");
            }

            converted += (Regex.Matches(Roman, "C").Count * 100);
            converted += (Regex.Matches(Roman, "L").Count * 50);
			converted += (Regex.Matches(Roman, "X").Count * 10);
			converted += (Regex.Matches(Roman, "V").Count * 5);
			converted += Regex.Matches(Roman, "I").Count;
			
			string convertedString = converted.ToString();
			if(converted < 10)
				convertedString = 0 + convertedString;

			return convertedString;
		}

		// Covered by ToolsTests.RemoveNamespaceString()
		/// <summary>
		/// Returns a string with the namespace removed
		/// </summary>
		/// <returns></returns>
		public static string RemoveNamespaceString(string title)
		{
			if (Namespace.Determine(title).Equals(Namespace.Article))
				return title;

			int pos = title.IndexOf(':');
			return pos < 0 ? title : title.Substring(pos + 1).Trim();
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
			if (string.IsNullOrEmpty(s) || s.Length > 9) return false;
			foreach (char c in s)
			{
                if (c != 'I' && c != 'V' && c != 'X' && c != 'L' && c != 'C') return false;
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
		/// <param name="responseURL">The resolved URL of the webpage</param>
		/// <returns>The HTML.</returns>
		public static string GetHTML(string url, out string responseURL)
		{
			return GetHTML(url, Encoding.UTF8, out responseURL);
		}

		/// <summary>
		/// Gets the HTML from the given web address.
		/// </summary>
		/// <param name="url">The URL of the webpage.</param>
		/// <param name="enc">The encoding to use.</param>
		/// <returns>The HTML.</returns>
		public static string GetHTML(string url, Encoding enc)
		{
			string x;
			return GetHTML(url, enc, out x);
		}
		
		/// <summary>
		/// Gets the HTML from the given web address.
		/// </summary>
		/// <param name="url">The URL of the webpage.</param>
		/// <param name="enc">The encoding to use.</param>
		/// <param name="responseURL">The resolved URL of the webpage</param>
		/// <returns>The HTML.</returns>
		public static string GetHTML(string url, Encoding enc, out string responseURL)
		{
			WriteDebug("GetHTML", url);
			if (Globals.UnitTestMode) throw new Exception("You shouldn't access Wikipedia from unit tests");
			CookieContainer cookieJar = new CookieContainer();

			HttpWebRequest rq = Variables.PrepareWebRequest(url); // Uses WikiFunctions' default UserAgent string
			rq.CookieContainer = cookieJar;

			HttpWebResponse response = (HttpWebResponse)rq.GetResponse();
			
			responseURL = response.ResponseUri.ToString();

			Stream stream = response.GetResponseStream();
			StreamReader sr = new StreamReader(stream, enc);

			string text = sr.ReadToEnd();

			sr.Close();
			stream.Close();
			response.Close();

			return text;
		}
		
		private static readonly Regex HTTPWWW = new Regex(@"^https?:?/+(?:www\d*\.)?", RegexOptions.Compiled);
		private static readonly Regex SubDomain = new Regex(@"^[a-z0-9\-]{4,}\.", RegexOptions.Compiled);
		private static readonly Regex DomainEndings = new Regex(@"\.[a-z]{2,3}(?:\.[a-z]{2,3})?$", RegexOptions.Compiled);

		/// <summary>
		/// Returns the domain name from a URL e.g. bbc.co.uk from http://www.bbc.co.uk/1212
		/// </summary>
		/// <param name="url">The source URL</param>
		/// <returns>The domain name for the URL, or a null string if no domain name can be determined</returns>
		public static string GetDomain(string url)
		{
			url=url.ToLower().Trim();

			if(!HTTPWWW.IsMatch(url))
				return "";

			url=HTTPWWW.Replace(url.ToLower().Trim(), "");

			if(url.Contains("/"))
				url=url.Substring(0, url.IndexOf("/"));

			if(DomainEndings.IsMatch(SubDomain.Replace(url, "")) && !SubDomain.Replace(url, "").StartsWith("com."))
				url = SubDomain.Replace(url, "");

			if(DomainEndings.IsMatch(SubDomain.Replace(url, "")) && !SubDomain.Replace(url, "").StartsWith("com."))
				url = SubDomain.Replace(url, "");

			return url;
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
			if (!string.IsNullOrEmpty(input) && char.IsLetter(input[0]) &&
				(char.ToUpper(input[0]) != char.ToLower(input[0])))
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
		
		/// <summary>
		/// Applies the key words "%%title%%" etc.
		/// https://meta.wikimedia.org/wiki/Help:Magic_words
		/// </summary>
		public static string ApplyKeyWords(string title, string text)
		{
			return ApplyKeyWords(title, text, false);
		}

		// Covered by ToolsTests.ApplyKeyWords()
		/// <summary>
		/// Applies the key words "%%title%%" etc.
		/// https://meta.wikimedia.org/wiki/Help:Magic_words
		/// </summary>
		public static string ApplyKeyWords(string title, string text, bool escape)
		{
			if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(title) && text.Contains("%%"))
			{
				string titleEncoded = WikiEncode(title);
				
				text = text.Replace("%%title%%", escape ? Regex.Escape(title) : title);
				text = text.Replace("%%titlee%%", titleEncoded);
				text = text.Replace("%%fullpagename%%", escape ? Regex.Escape(title) : title);
				text = text.Replace("%%fullpagenamee%%", titleEncoded);
				text = text.Replace("%%key%%", MakeHumanCatKey(title, text));

				string titleNoNamespace = RemoveNamespaceString(title);
				string basePageName = BasePageName(title);
				string subPageName = SubPageName(title);
				string theNamespace = GetNamespaceString(title);

				text = text.Replace("%%pagename%%", escape ? Regex.Escape(titleNoNamespace) : titleNoNamespace);
				text = text.Replace("%%pagenamee%%", WikiEncode(titleNoNamespace));

				text = text.Replace("%%basepagename%%", escape ? Regex.Escape(basePageName) : basePageName);
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
		{
			if (!Variables.CapitalizeFirstLetter|| string.IsNullOrEmpty(input))
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

		private static readonly CultureInfo EnglishCulture = new CultureInfo("en-GB");
		private static readonly TextInfo EnglishCultureTextInfo = EnglishCulture.TextInfo;

		/// <summary>
		/// Returns the trimmed input string in Title Case if:
		/// string all upper case
		/// string all lower case
		/// Otherwise returns lower/mixed case words in Title Case and UPPER case in UPPER
		/// </summary>
		/// <param name="text">the input text</param>
		/// <returns>the text in Title Case</returns>
		public static string TitleCaseEN(string text)
		{
			if (text.ToUpper().Equals(text))
				text = text.ToLower();

			return (EnglishCultureTextInfo.ToTitleCase(text.Trim()));
		}
		
		/// <summary>
		/// Prepends a newline to the string, only if string not empty
		/// </summary>
		/// <param name="s">The input string</param>
		/// <returns>The input string with newline prepended</returns>
		public static string Newline(string s)
		{
			return Newline(s, 1);
		}

		/// <summary>
		/// Prepends the specified number of newlines to the string, only if string not empty
		/// </summary>
		/// <param name="s">Input string</param>
		/// <param name="n">Number of newlines to prepend</param>
		/// <returns>(n x newlines) + Input string</returns>
		public static string Newline(string s, int n)
		{
			if (s.Length == 0)
				return s;

			StringBuilder sb = new StringBuilder(s);
			
			for (int i = 0; i < n; i++)
				sb.Insert(0,"\r\n");
			
			return sb.ToString();
		}

		private static readonly Regex RegexWordCountTable = new Regex(@"\{\|.*?\|\}", RegexOptions.Compiled | RegexOptions.Singleline);

		/// <summary>
		/// Returns word count of the string
		/// </summary>
		public static int WordCount(string text)
		{
			return WordCount(text, 999999);
		}
		// Covered by ToolsTests.WordCount()
		/// <summary>
		/// Returns word count of the string
		/// </summary>
		public static int WordCount(string text, int limit)
		{
			text = RegexWordCountTable.Replace(text, "");
			text = WikiRegexes.NestedTemplates.Replace(text, " ");
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
						i++;
					while (i < text.Length && char.IsLetterOrDigit(text[i]));
					
					if(words == limit)
						return words;
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
            // Performance: faster to get all wikilinks and filter on interwiki matches than simply run the regex on the whole article text
            List<string> allWikiLinks = (from Match m in WikiRegexes.WikiLink.Matches(text) where m.Value.Contains(":") select m.Value).ToList();

            return allWikiLinks.Where(s => SiteMatrix.Languages.Contains(WikiRegexes.PossibleInterwikis.Match(s + "]]").Groups[1].Value.ToLower())).Count();
		}
		
		private static readonly Regex FlagIOC = NestedTemplateRegex("flagIOC");

        // Covered by ToolsTests.LinkCountTests
        /// <summary>
        /// Returns the number of mainspace wikilinks i.e. [[links]] in the string, capped at the input limit
        /// </summary>
        public static int LinkCount(string text, int limit)
        {
            int res = FlagIOC.Matches(text).Count;

            if(res >= limit)
                return limit;

            // count only mainspace links, not image/category/interwiki/template links
            foreach(Match m in WikiRegexes.WikiLink.Matches(text))
            {
                if(Namespace.IsMainSpace(m.Groups[1].Value))
                    res++;

                if(res >= limit)
                    return res;
            }
            return res;
        }

        // Covered by ToolsTests.LinkCountTests
        /// <summary>
        /// Returns the number of mainspace wikilinks i.e. [[links]] in the string
        /// </summary>
        public static int LinkCount(string text)
        {
            return LinkCount(text, 9999999);
        }

		/// <summary>
		/// Returns a sorted list of duplicate wikilinks in the input text (links converted to first letter upper) in format: name (count)
		/// </summary>
		public static List<string> DuplicateWikiLinks(string text)
        {
            List<string> allWikiLinks = new List<string>();
            List<string> dupeWikiLinks = new List<string>();

            // ignore links in commented out text etc.
            text = Tools.ReplaceWithSpaces(text, WikiRegexes.UnformattedText.Matches(text));

            // don't count wikilinked dates or targetless links as duplicate links
            // make first character uppercase so that [[proton]] and [[Proton]] are marked as duplicate
            allWikiLinks = (from Match m in WikiRegexes.WikiLink.Matches(text) 
                where m.Groups[1].Value.Length > 0 && !WikiRegexes.Dates.IsMatch(m.Groups[1].Value) && !WikiRegexes.Dates2.IsMatch( m.Groups[1].Value) 
                    select Tools.TurnFirstToUpper(m.Groups[1].Value)).ToList();

            // Take all links found and generate dictionary of link name and count for those with more than one link
            Dictionary<string, int> dupeLinks = allWikiLinks.GroupBy(x => x).Where(g => g.Count() > 1).ToDictionary(x => x.Key, y => y.Count());

            // create list of "Name (count)" from Dictionary
            foreach(KeyValuePair<string, int> kvp in dupeLinks)
                dupeWikiLinks.Add(kvp.Key + @" (" + kvp.Value + @")");

            // ensure list is sorted
            dupeWikiLinks.Sort();
            
            return dupeWikiLinks;
        }

		// Covered by ToolsTests.RemoveSyntax
		/// <summary>
		/// Removes underscores and wiki syntax from links
		/// </summary>
		public static string RemoveSyntax(string text)
		{
			if (string.IsNullOrEmpty(text))
				return text;
			
			text = text.Trim();
			
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

		/// <summary>
		/// Returns the value of an HTML meta tag
		/// </summary>
		/// <param name="pagesource">page source HTML</param>
		/// <param name="metaname">meta content name</param>
		/// <returns>The meta tag value</returns>
		public static string GetMetaContentValue(string pagesource, string metaname)
		{
			if (pagesource.Length == 0 || metaname.Length == 0)
				return "";

			Regex metaContent = new Regex(@"< *meta +(?:xmlns=""http://www\.w3\.org/1999/xhtml"" +)?(?:name|property|itemprop) *= *(?:""|')" + Regex.Escape(metaname) + @"(?:""|')[^<>/]+content *= *(?:""|')([^""<>]+?)(?:""|') */? *>", RegexOptions.IgnoreCase);
			Regex metaContent2 = new Regex(@"< *meta +(?:xmlns=""http://www\.w3\.org/1999/xhtml"" +)?[^<>/]*content *= *(?:""|')([^""<>]+?)(?:""|') *(?:name|property) *= *(?:""|')" + Regex.Escape(metaname) + @"(?:""|') */? *>", RegexOptions.IgnoreCase);

			if(metaContent.IsMatch(pagesource))
				return metaContent.Match(pagesource).Groups[1].Value.Trim();
			
			return metaContent2.Match(pagesource).Groups[1].Value.Trim();
		}

		// Covered by ToolsTests.SplitToSections()
		/// <summary>
		/// Splits wikitext to sections based on any level wiki heading
		/// </summary>
		/// <param name="articleText">Page text</param>
		/// <returns>Array of strings, each represents a section with its heading (if any)</returns>
		public static string[] SplitToSections(string articleText)
		{
			List<string> sections = new List<string>();

            int lastmatchpos = 0;
            foreach(Match m in WikiRegexes.Headings.Matches(articleText))
            {
                if(m.Index > 0) // Don't add empty first section if page starts with heading
                    sections.Add(articleText.Substring(lastmatchpos, m.Index-lastmatchpos));
                lastmatchpos = m.Index;
            }

            // Add text of final section, plus extra newline at end
            sections.Add(articleText.Substring(lastmatchpos) + "\r\n");

			return sections.ToArray();
		}

        private static readonly Regex HeadingStart = new Regex(@"^==", RegexOptions.Multiline);

        /// <summary>
        /// Matches the first section of an article, if the article has sections, else the whole article
        /// </summary>
        public static string GetZerothSection(string articleText)
        {
            Match m = HeadingStart.Match(articleText);

            if(m.Success)
                return articleText.Substring(0, m.Index);

            return articleText;
        }

		/// <summary>
		/// returns how much of the given text starts with only items matched by the given regex, allowing for whitespace only
		/// E.g. whether a portion of text starts only with one or more wiki templates
		/// </summary>
		/// <param name="text">Article text or section to check</param>
		/// <param name="Items">Regex to match one or more items e.g. wiki templates</param>
		/// <param name="allowHeading">Whether to also allow text to start with a heading then only the matched items</param>
		/// <returns>Length</returns>
		public static int HowMuchStartsWith(string text, Regex Items, bool allowHeading)
		{
			int heading = 0;

			if(allowHeading)
			{
				Match m = WikiRegexes.Headings.Match(text);
				
				if(m.Index == 0)
				{
					text = WikiRegexes.Headings.Replace(text, "", 1);
					heading = m.Length;
				}
			}

			MatchCollection mc = Items.Matches(text);

			if(mc.Count == 0)
				return 0;

			string replaced = ReplaceWithSpaces(text, mc).TrimStart();

			return (text.Length-replaced.Length+heading);
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

		public static readonly string[][] Diacritics =
		{
			new[] {"Ɯ", "W"},
			new[] {"ß", "ss"},
			new[] {"Ŀ", "L"},
			new[] {"ŀ", "l"},
			new[] {"Ð", "D"},
			new[] {"ð", "d"},
			new[] {"Þ", "Th"},
			new[] {"þ", "th"},
			new[] {"Œ", "Oe"},
			new[] {"œ", "oe"},
			new[] {"Æ", "Ae"},
			new[] {"æ", "ae"},
			new[] {"Ə", "E"},
			new[] {"ə", "e"},
			new[] {"ĸ", "k"},
			new[] {"ŉ", "n"},
			new[] {"ſ", "s"},
			new[] {"Ё", "E"},
			new[] {"ё", "e"},
			new[] {"б", "b"},
			new[] {"л", "l"},
			new[] {"К", "K"},
			new[] {"к", "k"},
			new[] {"М", "M"},
			new[] {"м", "m"},
			new[] {"Ӳ", "Y"},
			new[] {"ӳ", "y"},
			new[] {"о", "o"},
			new[] {"О", "O"},
			new[] {"а", "a"},
			new[] {"А", "A"},
			new[] {"с", "c"},
			new[] {"С", "C"},
			new[] {"е", "e"},
			new[] {"Е", "E"},
			new[] {"і", "i"},
			new[] {"І", "I"},
			new[] {"х", "x"},
			new[] {"Х", "X"},
			new[] {"Đ", "D"},
			new[] {"đ", "d"},
			new[] {"Ǣ", "Ae"},
			new[] {"ǣ", "ae"},
			new[] {"Ǽ", "Ae"},
			new[] {"ǽ", "ae"},
			new[] {"Ǿ", "O"},
			new[] {"ǿ", "o"},
			new[] {"ı", "i"},
			new[] {"Ⱥ", "A"},
			new[] {"ⱥ", "a"},
			new[] {"Ƀ", "B"},
			new[] {"ƀ", "b"},
			new[] {"Ȼ", "C"},
			new[] {"ȼ", "c"},
			new[] {"Ɇ", "E"},
			new[] {"ɇ", "e"},
			new[] {"Ǥ", "G"},
			new[] {"ǥ", "g"},
			new[] {"Ħ", "H"},
			new[] {"ħ", "h"},
			new[] {"Ɨ", "I"},
			new[] {"ɨ", "i"},
			new[] {"Ɉ", "J"},
			new[] {"ɉ", "j"},
			new[] {"Ł", "L"},
			new[] {"ł", "l"},
			new[] {"Ø", "O"},
			new[] {"ø", "o"},
			new[] {"Ᵽ", "P"},
			new[] {"ᵽ", "p"},
			new[] {"Ɍ", "R"},
			new[] {"ɍ", "r"},
			new[] {"Ŧ", "T"},
			new[] {"ŧ", "t"},
			new[] {"Ʉ", "U"},
			new[] {"ʉ", "u"},
			new[] {"Ɏ", "Y"},
			new[] {"ɏ", "y"},
			new[] {"Ƶ", "Z"},
			new[] {"ƶ", "z"},
			new[] {"Ɓ", "B"},
			new[] {"ɓ", "b"},
			new[] {"Ƈ", "C"},
			new[] {"ƈ", "c"},
			new[] {"Ɗ", "D"},
			new[] {"ɗ", "d"},
			new[] {"Ƒ", "F"},
			new[] {"ƒ", "f"},
			new[] {"Ɠ", "G"},
			new[] {"ɠ", "g"},
			new[] {"ɦ", "h"},
			new[] {"Ƙ", "K"},
			new[] {"ƙ", "k"},
			new[] {"Ɲ", "N"},
			new[] {"ɲ", "n"},
			new[] {"Ƥ", "P"},
			new[] {"ƥ", "p"},
			new[] {"ʠ", "q"},
			new[] {"Ƭ", "T"},
			new[] {"ƭ", "t"},
			new[] {"Ʋ", "V"},
			new[] {"ʋ", "v"},
			new[] {"Ⱳ", "W"},
			new[] {"ⱳ", "w"},
			new[] {"Ƴ", "Y"},
			new[] {"ƴ", "y"},
			new[] {"ᶀ", "b"},
			new[] {"ᶁ", "d"},
			new[] {"ᶂ", "f"},
			new[] {"ᶃ", "g"},
			new[] {"ᶄ", "k"},
			new[] {"ᶅ", "l"},
			new[] {"ᶆ", "m"},
			new[] {"ᶇ", "n"},
			new[] {"ᶈ", "p"},
			new[] {"ᶉ", "r"},
			new[] {"ᶊ", "s"},
			new[] {"ƫ", "t"},
			new[] {"ᶌ", "y"},
			new[] {"ᶍ", "x"},
			new[] {"ᶎ", "z"},
			new[] {"Ʈ", "T"},
			new[] {"ʈ", "t"},
			new[] {"ʐ", "z"},
			new[] {"ƌ", "d"},
			new[] {"ƚ", "l"},
			new[] {"ƞ", "n"},
			new[] {"ȡ", "d"},
			new[] {"ȥ", "z"},
			new[] {"ȴ", "l"},
			new[] {"ȵ", "n"},
			new[] {"ȶ", "t"},
			new[] {"ȿ", "s"},
			new[] {"ɀ", "z"},
			new[] {"Ɖ", "D"},
			new[] {"ɖ", "d"},
			new[] {"Ƌ", "D"},
			new[] {"Ǝ", "E"},
			new[] {"Ɛ", "E"},
			new[] {"Ɵ", "O"},
			new[] {"ɵ", "o"},
			new[] {"ǅ", "Dz"},
			new[] {"ǈ", "Lj"},
			new[] {"ǋ", "Nj"},
			new[] {"ǝ", "e"},
			new[] {"ǲ", "Dz"},
			new[] {"Ƞ", "N"},
			new[] {"Ȥ", "Z"},
			new[] {"Ƚ", "L"},
			new[] {"Ⱦ", "T"},
			new[] {"Ⱡ", "L"},
			new[] {"ⱡ", "l"},
			new[] {"Ɫ", "L"},
			new[] {"Ɽ", "R"},
			new[] {"ⱦ", "t"},
			new[] {"Ⱨ", "H"},
			new[] {"ⱨ", "h"},
			new[] {"Ⱪ", "K"},
			new[] {"ⱪ", "k"},
			new[] {"Ⱬ", "Z"},
			new[] {"ⱬ", "z"},
			new[] {"ⱴ", "v"},
			new[] {"ẚ", "a"},
			new[] {"²", "2"},
			new[] {"Ŋ", "N"},
			new[] {"ŋ", "n"},
			new[] {"ﬂ", "fl"},
			new[] {"ǉ", "lg"},
			new[] {"ǌ", "ng"},
			new[] {"№", "No"}
		};

		public static readonly string[][] SortKeyChars =
		{
			//per WP:SORTKEY "&" needs to change to "and"
			new[] {"&", "and"},
			//per WP:SORTKEY replace / with space
			new[] {"/", " "},
			//per WP:SORTKEY replace multiplication sign with x letter
			new[] {"×", "x"},
			//remove weird "ǀ" character
			new[] {"ǀ", ""},
			//other weird characters
			new[] {"…", "..."},
			new[] {"·", " "},

			new[] {"’", "'"}, // apostrophe
			new[] {"‘", "'"}, // quotation mark
			new[] {"ʻ", "'"}, // okina
			new[] {"`", "'"}, // grave accent
			new[] {"´", "'"}, // acute accent
			new[] {"′", "'"}, // prime
			new[] {"ʹ", "'"}, // greek numeral
		  
			new[] {"“", "'"}, // double quotes (curly)
			new[] {"”", "'"}, // double quotes (curly)
			new[] {"„", "'"}, // double low-9 quotation mark
			new[] {"‟", "'"}, // double reversed comma, quotation mark

			new[] {"–", "-"}, // endash
			new[] {"—", "-"}, // emdash
			new[] {"‐", "-"}, // hyphen
			new[] {"‑", "-"}, // hyphen-minus
			new[] {"‒", "-"}, // figure dash
			new[] {"−", "-"}, // minus
			
			new[] {"¡", ""}, // inverted exclamation mark
			new[] {"¿", ""}, // inverted question mark
			new[] {"ʾ", "'"}, // modifier letter right half ring
			new[] {"ʿ", "'"}, // modifier letter left half ring

			new[] {"̧ ", ""} // cedilla
		};
		#endregion

		// Covered by HumanCatKeyTests.RemoveDiacritics()
		/// <summary>
		/// substitutes characters with diacritics with their Latin equivalents
		/// </summary>
		public static string RemoveDiacritics(string s)
		{
			foreach (var p in Diacritics)
			{
				s = s.Replace(p[0], p[1]);
			}
			s = new string(
				s
					.Normalize(NormalizationForm.FormD)
					.ToCharArray()
					.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
					.ToArray());
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
		
		/// <summary>
		/// Removes recognised diacritics and double quotes, converts to Proper Case per [[WP:CAT]]
		/// </summary>
		public static string FixupDefaultSort(string s)
		{
			return FixupDefaultSort(s, false);
		}

		private static readonly Regex BadDsChars = new Regex(@"[\""º]");

		//Covered by HumanCatKeyTests.FixUpDefaultSortTests()
		/// <summary>
		/// Removes recognised diacritics and double quotes, converts to Proper Case per [[WP:CAT]]
		/// </summary>
		public static string FixupDefaultSort(string s, bool isArticleAboutAPerson)
		{
			s = CleanSortKey(s);
			
			s = BadDsChars.Replace(s, "");
			
			if(isArticleAboutAPerson)
			{
				s = s.Replace("'", "");
				s = Regex.Replace(s, @"(\w) *,(\w)", "$1, $2"); // ensure space after comma between names
			}

			return s.Trim();
		}

		/// <summary>
		/// Cleans sortkeys: removes diacritics, except for ru-wiki
		/// Cleans up/removes specific characters per WP:SORTKEY
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static string CleanSortKey(string s)
		{
			// no diacritic removal in sortkeys on ru-wiki
			if(!Variables.LangCode.Equals("ru"))
				s = RemoveDiacritics(s);

			s = s.Replace("&ndash;", "–");
			s = s.Replace("&mdash;", "—");

			// normalisation - simplify double spaces to a single one
			while(s.Contains("  "))
				s = s.Replace("  ", " ");

			foreach (var p in SortKeyChars)
			{
				s = s.Replace(p[0], p[1]);
			}

			return s;
		}

		/// <summary>
		/// Returns a dediplicated list, using .NET 3.5 Distinct() function if available
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static List<string> DeduplicateList(List<string> input)
		{
			if(Globals.SystemCore3500Available)
				return DeduplicateListHS(input);

			return DeduplicateListLoop(input);
		}

		private static List<string> DeduplicateListHS(List<string> input)
		{
			return input.Distinct().ToList();
		}

		private static List<string> DeduplicateListLoop(List<string> input)
		{
			List<string> output = new List<string>();

			foreach(string s in input)
			{
				if(!output.Contains(s))
					output.Add(s);
			}

			return output;
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
				WriteTextFileAbsolutePath(message, Application.StartupPath + DirectoryDelimiter() + file, append);
		}

		/// <summary>
		/// Writes a message to the given file in the directory of the application.
		/// </summary>
		/// <param name="message">The message to write.</param>
		/// <param name="file">The name of the file, e.g. "Log.txt".</param>
		/// <param name="append"></param>
		public static void WriteTextFile(StringBuilder message, string file, bool append)
		{
			WriteTextFileAbsolutePath(message.ToString(), Application.StartupPath + DirectoryDelimiter() + file, append);
		}

		/// <summary>
		/// Returns directory delimiter: normally \\ but / if running under Mono
		/// </summary>
		/// <returns></returns>
		private static string DirectoryDelimiter()
		{
			return Globals.UsingMono ? "/" : "\\";
		}

		/// <summary>
		/// Turns an HTML list into a wiki style list using the input bullet style
		/// </summary>
		/// <param name="text">HTML text to convert to list</param>
		/// <param name="bullet">List style to use (# or *)</param>
		public static string HTMLListToWiki(string text, string bullet)
		{
			text = text.Replace("\r\n\r\n", "\r\n");
			//   text = text.Replace("\n\n", "\n");
			text = Regex.Replace(text, "<br ?/?>", "", RegexOptions.IgnoreCase);
			text = Regex.Replace(text, "</?(ol|ul|li)>", "", RegexOptions.IgnoreCase);
			text = Regex.Replace(text, "^</?(ol|ul|li)>\r\n", "", RegexOptions.Multiline | RegexOptions.IgnoreCase);
			text = Regex.Replace(text, @"^(\:|\*|#|\(? ?\d{1,3}\b ?\)|\d{1,3}\b\.?)", "", RegexOptions.Multiline);

			// add bullet to start of each line, but not lines with just whitespace
			return Regex.Replace(text, @"^(.*)$(?<!^\s+$)", bullet + "$1", RegexOptions.Multiline);
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
		/// Writes debug log message with timestamp to nearest millisecond
		/// </summary>
		public static void WriteDebug(string @object, string text)
		{
			if (!WriteDebugEnabled)
				return;
			
			for(int a = 0;a < 100;a++)
			{
				try
				{
					WriteTextFile(string.Format(
						@"object: {0}
Time: {1}
Message: {2}

", @object, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture), text), "Log.txt", true);
					break;
				}
				catch
				{
					System.Threading.Thread.Sleep(50); // prevents errors over log file being 'in use by other application'
				}
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

		/// <summary>
		/// Replaces first occurrence of a given text within a string
		/// </summary>
		/// <param name="text">Text to be processed</param>
		/// <param name="oldValue">Text to be replaced</param>
		/// <param name="newValue">Replacement text</param>
		/// <returns>Whether the replacement has been made</returns>
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

		/// <summary>
		/// Converts all line endings are the environment default
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static string ConvertToLocalLineEndings(string input)
		{
			return input.Replace("\n", Environment.NewLine);
		}

		/// <summary>
		/// Converts all line endings to \n
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
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

		private static string WineBrowserPath;
		/// <summary>
		/// Error supressed URL opener in default browser (Windows) or Firefox/Chromium/Konqueror for Wine
		/// </summary>
		public static void OpenURLInBrowser(string url)
		{
			// For Wine use attempt to dynamically determine available browser, caching result
			if(WineBrowserPath == null)
			{
				if(File.Exists("/usr/bin/firefox"))
					WineBrowserPath = "/usr/bin/firefox";
				else if(File.Exists("/usr/bin/chromium-browser"))
					WineBrowserPath = "/usr/bin/chromium-browser";
				else if(File.Exists("/usr/bin/konqueror"))
					WineBrowserPath = "/usr/bin/konqueror";
				else WineBrowserPath = ""; // Windows, or Wine and none of these browsers available
			}
			try
			{
				if(!Globals.UnitTestMode)
				{
					if(WineBrowserPath.Length > 0) // Wine
						System.Diagnostics.Process.Start(WineBrowserPath, url);
					else // Windows
						System.Diagnostics.Process.Start(url);
				}
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
				OpenURLInBrowser("https://en.wikipedia.org/wiki/User:" + WikiEncode(title));
			else
				OpenURLInBrowser("https://en.wikipedia.org/wiki/" + WikiEncode(title));
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
		/// Expands (substitutes) template calls using the API
		/// </summary>
		/// <param name="articleText">The text of the article</param>
		/// <param name="articleTitle">The title of the artlce</param>
		/// <param name="regexes">Dictionary of templates to substitute</param>
		/// <param name="includeComment"></param>
		/// <returns>The updated article text</returns>
		public static string ExpandTemplate(string articleText, string articleTitle, Dictionary<Regex, string> regexes, bool includeComment)
		{
			foreach (KeyValuePair<Regex, string> p in regexes)
			{
				string originalArticleText = "";
				
				while(!originalArticleText.Equals(articleText))
				{
					originalArticleText = articleText;
					// avoid matching on previously commented out calls
					Match m = p.Key.Match(ReplaceWithSpaces(articleText, WikiRegexes.Comments));
					if(!m.Success)
						continue;
					
					string call = m.Value, result;
					
					if(Globals.UnitTestMode)
						result = "Expanded template test return";
					else
					{
						string expandUri = Variables.URLApi + "?action=expandtemplates&format=xml&title=" + WikiEncode(articleTitle) + "&text=" + HttpUtility.UrlEncode(call);
						
						try
						{
							string respStr = GetHTML(expandUri);
							Match m1 = ExpandTemplatesRegex.Match(respStr);
							if (!m1.Success)
							{
								continue;
							}
							result = HttpUtility.HtmlDecode(m1.Groups[1].Value);
						}
						catch
						{
							continue;
						}
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
				foreach (object t in box.SelectedItems)
				{
					builder.AppendLine(t.ToString());
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
		private static readonly char[] Separators = { ReturnLine, NewLine };

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
		/// Returns whether the unformatted text content is the same in the two strings
		/// Rule is: unformatted text must be entirely missing in new string, or present exactly as before
		/// </summary>
		/// <param name="originalArticleText">the first string to search</param>
		/// <param name="articleText">the second string to search</param>
		/// <returns>whether the unformatted text content is the same in the two strings</returns>
		public static bool UnformattedTextNotChanged(string originalArticleText, string articleText)
		{
			if(originalArticleText.Equals(articleText))
				return true;

			List<string> before = new List<string>();
			foreach(Match m in WikiRegexes.UnformattedText.Matches(originalArticleText))
			{
				before.Add(m.Value);
			}
			
			List<string> after = new List<string>();
			foreach(Match m in WikiRegexes.UnformattedText.Matches(articleText))
			{
				after.Add(m.Value);
			}
			
			foreach(string s in before)
			{
				after.Remove(s);
			}
			
			return (after.Count == 0);
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

		/// <summary>
		/// Turns a list of pages into a list of pages in the input namespace
		/// Does not convert Special pages
		/// </summary>
		/// <param name="list">The list of  pages.</param>
		/// <param name="NewNamespace">New namespace number</param>
		/// <returns>The list of articles.</returns>
		public static List<Article> ConvertNamespace(List<Article> list, int NewNamespace)
		{
			List<Article> newList = new List<Article>(list.Count);

			foreach (Article a in list)
			{
				string s = ConvertNamespace(a, NewNamespace);
				newList.Add(a.Equals(s) ? a : new Article(s));
			}
			return newList;
		}

		/// <summary>
		/// Turns a list of pages into a list of pages in the input namespace
		/// Does not convert Special pages
		/// </summary>
		/// <param name="a">The article</param>
		/// <param name="NewNamespace">New namespace number</param>
		/// <returns>The list of articles.</returns>
		public static string ConvertNamespace(Article a, int NewNamespace)
		{
			if (Namespace.IsSpecial(a.NameSpaceKey) || !Variables.Namespaces.ContainsKey(NewNamespace))
				return a.Name;

			return Variables.Namespaces[NewNamespace] + a.NamespacelessName;
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
		/// Performs HTTP post of given variables to given URL
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
		/// Converts a NameValueCollection of parameter names and values into an API command string
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
		/// Shows the user an input box to select a number
		/// </summary>
		/// <param name="edits">Is the thing being counted, user edits?</param>
		/// <param name="max">The maximum value the user can choose</param>
		/// <returns>-1 if cancel clicked, else the number they chose</returns>
		public static int GetNumberFromUser(bool edits, int max)
		{
			using (Controls.LevelNumber num = new Controls.LevelNumber(edits, max))
			{
				if (num.ShowDialog() != DialogResult.OK) return -1;
				return num.Levels;
			}
		}

		/// <summary>
		/// Replaces the values of all matches with spaces
		/// </summary>
		/// <param name="input">The article text to update</param>
		/// <param name="matches">Collection of matches to replace with spaces</param>
		/// <returns>The updated article text</returns>
		public static string ReplaceWithSpaces(string input, MatchCollection matches)
		{
			return ReplaceWith(input, matches, ' ');
		}

		/// <summary>
		/// Replaces the values of all matches with spaces
		/// </summary>
		/// <param name="input">The article text to update</param>
		/// <param name="matches">Collection of matches to replace with spaces</param>
		/// <param name="keepGroup">Regex match group to keep text of in replacement</param>
		/// <returns>The updated article text</returns>
		public static string ReplaceWithSpaces(string input, MatchCollection matches, int keepGroup)
		{
			return ReplaceWith(input, matches, ' ', keepGroup);
		}

		/// <summary>
		/// Replaces the values of all matches with a given character
		/// </summary>
		/// <param name="input">The article text to update</param>
		/// <param name="matches">Collection of matches to replace with spaces</param>
		/// <param name="rwith">The character to use</param>
		/// <returns>The updated article text</returns>
		public static string ReplaceWith(string input, MatchCollection matches, char rwith)
		{
			return ReplaceWith(input, matches, rwith, 0);
		}

		/// <summary>
		/// Replaces the values of all matches with a given character
		/// </summary>
		/// <param name="input">The article text to update</param>
		/// <param name="matches">Collection of matches to replace with spaces</param>
		/// <param name="rwith">The character to use</param>
		/// <param name="keepGroup">Regex match group to keep text of in replacement</param>
		/// <returns>The updated article text</returns>
		public static string ReplaceWith(string input, MatchCollection matches, char rwith, int keepGroup)
		{
			StringBuilder sb = new StringBuilder(input.Length);
			foreach (Match m in matches)
			{
				sb.Append(input, sb.Length, m.Index - sb.Length);
				if(keepGroup < 1 || m.Groups[keepGroup].Value.Length == 0)
					sb.Append(rwith, m.Length);
				else
				{
					sb.Append(rwith, m.Groups[keepGroup].Index - m.Index);
					sb.Append(m.Groups[keepGroup].Value);
					sb.Append(rwith, m.Index + m.Length - m.Groups[keepGroup].Index - m.Groups[keepGroup].Length);
				}
			}
			sb.Append(input, sb.Length, input.Length - sb.Length);
			return sb.ToString();
		}

		/// <summary>
		/// Replaces all matches of a given regex in a string with space characters
		/// such that the length of the string remains the same
		/// </summary>
		/// <param name="input">The article text to update</param>
		/// <param name="regex">The regex to replace all matches of</param>
		/// <returns>The updated article text</returns>
		public static string ReplaceWithSpaces(string input, Regex regex)
		{
			return ReplaceWithSpaces(input, regex.Matches(input));
		}

		/// <summary>
		/// Replaces all matches of a given regex in a string with space characters
		/// such that the length of the string remains the same
		/// </summary>
		/// <param name="input">The article text to update</param>
		/// <param name="regex">The regex to replace all matches of</param>
		/// <param name="keepGroup">Regex match group to keep text of in replacement</param>
		/// <returns>The updated article text</returns>
		public static string ReplaceWithSpaces(string input, Regex regex, int keepGroup)
		{
			return ReplaceWithSpaces(input, regex.Matches(input), keepGroup);
		}

		/// <summary>
		/// Replaces all matches of a given regex in a string with a character
		/// such that the length of the string remains the same
		/// </summary>
		/// <param name="input">The article text to update</param>
		/// <param name="regex">The regex to replace all matches of</param>
		/// <param name="rwith">The character to use</param>
		/// <returns>The updated article text</returns>
		public static string ReplaceWith(string input, Regex regex, char rwith)
		{
			return ReplaceWith(input, regex.Matches(input), rwith);
		}

		/// <summary>
		/// Replaces all matches of a given regex in a string with a character
		/// such that the length of the string remains the same
		/// </summary>
		/// <param name="input">The article text to update</param>
		/// <param name="regex">The regex to replace all matches of</param>
		/// <param name="rwith">The character to use</param>
		/// <param name="keepGroup">Regex match group to keep text of in replacement</param>
		/// <returns>The updated article text</returns>
		public static string ReplaceWith(string input, Regex regex, char rwith, int keepGroup)
		{
			return ReplaceWith(input, regex.Matches(input), rwith, keepGroup);
		}

		// ensure dates returned are English.
		private static readonly CultureInfo BritishEnglish = new CultureInfo("en-GB");
		private static readonly CultureInfo AmericanEnglish = new CultureInfo("en-US");

		private static readonly Regex YearMon = new Regex(@"^\d{4}\-[0-3]\d$", RegexOptions.Compiled);
		private static readonly Regex MonthYear = new Regex(@"^\w+ \d{4}$", RegexOptions.Compiled);
		
		/// <summary>
		/// Returns the input date in the requested format (American or International). If another Locale is pasased in the input date is returned. For en-wiki only.
		/// </summary>
		/// <param name="inputDate">string representing a date, any format that C# can parse</param>
		/// <param name="locale">Locale of output date required (American/International/ISO)</param>
		/// <returns>The English-language (American or International) date</returns>
		public static string ConvertDate(string inputDate, Parsers.DateLocale locale)
		{
			return ConvertDate(inputDate, locale, false);
		}
		
		/// <summary>
		/// Returns the input date in the requested format (American or International). If another Locale is pasased in the input date is returned. For en-wiki only.
		/// </summary>
		/// <param name="inputDate">string representing a date, any format that C# can parse</param>
		/// <param name="locale">Locale of output date required (American/International/ISO)</param>
		/// <param name="AmericanInputDate">Whether the input date is in American MM/DD/YYYY format</param>
		/// <returns>The English-language (American or International) date</returns>
		public static string ConvertDate(string inputDate, Parsers.DateLocale locale, bool AmericanInputDate)
		{
			inputDate = inputDate.Trim();
			if (Variables.LangCode != "en" || YearMon.IsMatch(inputDate) || MonthYear.IsMatch(inputDate))
				return inputDate;

			DateTime dt;

			try
			{
				dt = Convert.ToDateTime(inputDate, AmericanInputDate ? AmericanEnglish : BritishEnglish);
			}
			catch
			{
				return inputDate;
			}

			switch (locale)
			{
				case Parsers.DateLocale.American:
					return dt.ToString("MMMM d, yyy", BritishEnglish);
				case Parsers.DateLocale.International:
					return dt.ToString("d MMMM yyy", BritishEnglish);
				case  Parsers.DateLocale.ISO:
					return dt.ToString("yyy-MM-dd", BritishEnglish);
				default:
					return inputDate;
			}
		}
		
		/// <summary>
		/// Attempts to parse the input string as a date, if parsed returns whether the date is before the system date
		/// Otherwise returns false
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public static bool DateBeforeToday(string date)
		{
			try
			{
				DateTime dt = DateTime.Parse(date);

				if(dt.CompareTo(DateTime.Now) < 0)
					return true;
			}
			catch
			{
				return false;
			}
			
			return false;
		}
		
		/// <summary>
		/// Appends the input parameter and value to the input template
		/// </summary>
		/// <param name="templateCall">The input template call</param>
		/// <param name="parameter">The input parameter name</param>
		/// <param name="newValue">The input parameter value</param>
		/// <returns>The updated template call</returns>
		public static string AppendParameterToTemplate(string templateCall, string parameter, string newValue)
		{
			return AppendParameterToTemplate(templateCall, parameter, newValue, false);
		}
		
		private static readonly Regex Bars = new Regex(@"\|", RegexOptions.Compiled);
		private static readonly Regex AfterSpacedBars = new Regex(@"\| ", RegexOptions.Compiled);
		private static readonly Regex BeforeSpacedBars = new Regex(@" +\|", RegexOptions.Compiled);
		private static readonly Regex Newlines = new Regex("\r\n", RegexOptions.Compiled);

		/// <summary>
		/// Appends the input parameter and value to the input template
		/// Determines whether to put parameter on newline, and whether to put space after pipe based on existing template paremeters' formatting
		/// </summary>
		/// <param name="templateCall">The input template call</param>
		/// <param name="parameter">The input parameter name</param>
		/// <param name="newValue">The input parameter value</param>
		/// <param name="unspaced">Whether to add the parameter value without any excess whitespace</param>
		/// <returns>The updated template call</returns>
		public static string AppendParameterToTemplate(string templateCall, string parameter, string newValue, bool unspaced)
		{
			if (!templateCall.StartsWith(@"{{"))
				return templateCall;

			string separatorBefore = "", separatorAfter = "";
			
			if(!unspaced)
			{
				string templatecopy = templateCall;
				templatecopy = @"{{" + ReplaceWithSpaces(templatecopy.Substring(2), WikiRegexes.NestedTemplates);
				templatecopy = ReplaceWithSpaces(templatecopy, WikiRegexes.SimpleWikiLink);
				templatecopy = ReplaceWithSpaces(templatecopy, WikiRegexes.UnformattedText);

				int bars = Bars.Matches(templatecopy).Count, newlines = Newlines.Matches(templatecopy).Count;

				// determine whether to use newline: use if > 2 newlines and a newline per bar, allowing up to two without
				if (newlines > 2 && newlines >= (bars - 2))
				{
					separatorBefore = "\r\n";
					
					// copy number of spaces used prior to bar
					int spacesBeforeBar = BeforeSpacedBars.Match(templatecopy).Value.Length-1;
					
					if(spacesBeforeBar >= 1)
						separatorBefore += BeforeSpacedBars.Match(templatecopy).Value.TrimEnd('|');
				}
				else
				{
					// are spaces before bar used?
					int beforeSpacedBars = BeforeSpacedBars.Matches(templatecopy).Count;
					
					if(bars > 3 && beforeSpacedBars <= 2)
						separatorBefore = "";
					else
						separatorBefore = " ";
				}

				// are spaces after bar used?
				int afterSpacedBars = AfterSpacedBars.Matches(templatecopy).Count;
				
				if(bars > 3 && afterSpacedBars <= 2)
					separatorAfter = "";
				else
					separatorAfter = " ";
			}

			return WikiRegexes.TemplateEnd.Replace(templateCall, separatorBefore + @"|" + separatorAfter + parameter + "=" + newValue + @"$1}}");
		}

		/// <summary>
		/// Returns the value of the input parameter in the input template
		/// </summary>
		/// <param name="templateCall">the input template call</param>
		/// <param name="parameter">the input parameter to find</param>
		/// <param name="caseInsensitiveParameterName">Whether to match case insensitively on parameter name</param>
		/// <returns>The trimmed parameter value, or a null string if the parameter is not found</returns>
		public static string GetTemplateParameterValue(string templateCall, string parameter, bool caseInsensitiveParameterName)
		{
			RegexOptions ro = RegexOptions.Singleline;
			
			if(caseInsensitiveParameterName)
				ro |= RegexOptions.IgnoreCase;
			
			Regex paramRegex = new Regex(@"\|\s*" + Regex.Escape(parameter) + @"\s*=([^}|]*)(?=\||}}$)", ro);

			string pipecleanedtemplate = PipeCleanedTemplate(templateCall);

			Match m = paramRegex.Match(pipecleanedtemplate);

			if (m.Success)
			{
				Group paramValue = m.Groups[1];

				return templateCall.Substring(paramValue.Index, paramValue.Length).Trim();
			}

			return "";
		}

		private static readonly Regex param = new Regex(@"\|\s*([\w0-9_ -]+?)\s*=\s*([^|}]*)");
		
		/// <summary>
		/// Returns a dictionary of all named parameters used in the template and the value used.
		/// If the parameter is specified with no value, an empty string is returned as the value.
		/// If there are duplicate parameters, the value of the first (may be blank) is reported.
		/// </summary>
		/// <param name="templateCall"></param>
		/// <returns></returns>
		public static Dictionary<string, string> GetTemplateParameterValues(string templateCall)
		{	
			Dictionary<string, string> paramsFound = new Dictionary<string, string>();			

			string pipecleanedtemplate = PipeCleanedTemplate(templateCall);

			foreach(Match m in param.Matches(pipecleanedtemplate))
			{
				if(!paramsFound.ContainsKey(m.Groups[1].Value))
					paramsFound.Add(m.Groups[1].Value, templateCall.Substring(m.Groups[2].Index, m.Groups[2].Length).Trim());
			}

			return paramsFound;
		}
		
		/// <summary>
		/// Returns the value of the input parameter in the input template
		/// </summary>
		/// <param name="templateCall">the input template call</param>
		/// <param name="parameter">the input parameter to find</param>
		/// <returns>The trimmed parameter value, or a null string if the parameter is not found</returns>
		public static string GetTemplateParameterValue(string templateCall, string parameter)
		{
			return GetTemplateParameterValue(templateCall, parameter, false);
		}

		/// <summary>
		/// Returns the values of given parameters for a template call
		/// </summary>
		/// <param name="templateCall">The template call</param>
		/// <param name="parameters">List of parameters requested</param>
		/// <param name="caseInsensitiveParameterNames">Whether to match case insensitively on parameter name</param>
		/// <returns>List of parameter values</returns>
		public static List<string> GetTemplateParametersValues(string templateCall, List<string> parameters, bool caseInsensitiveParameterNames)
		{
			List<string> returnedvalues = new List<string>();

			foreach (string p in parameters)
			{
				returnedvalues.Add(GetTemplateParameterValue(templateCall, p, caseInsensitiveParameterNames));
			}

			return returnedvalues;
		}
		
		/// <summary>
		/// Returns the values of given parameters for a template call
		/// </summary>
		/// <param name="templateCall">The template call</param>
		/// <param name="parameters">List of parameters requested</param>
		/// <returns>List of parameter values</returns>
		public static List<string> GetTemplateParametersValues(string templateCall, List<string> parameters)
		{
			return GetTemplateParametersValues(templateCall, parameters, false);
		}

		/// <summary>
		/// Returns the requested argument from the input template call
		/// </summary>
		/// <param name="templateCall">The template call</param>
		/// <param name="argument">The argument to return</param>
		/// <returns>The argument value (trimmed)</returns>
		public static string GetTemplateArgument(string templateCall, int argument)
		{
			Regex arg = new Regex(@"\|\s*(.*?)\s*(?=\||}}$)", RegexOptions.Singleline);

			string pipecleanedtemplate = PipeCleanedTemplate(templateCall);
			int count = 1;

			foreach (Match m in arg.Matches(pipecleanedtemplate))
			{
				if (count.Equals(argument))
					return templateCall.Substring(m.Groups[1].Index, m.Groups[1].Length);

				count++;
			}

			return "";
		}
		
		/// <summary>
		/// Returns the index of the given argumment from the input template call
		/// </summary>
		/// <param name="templateCall">The template call</param>
		/// <param name="argument">The argument to find</param>
		/// <returns>The index of the argument</returns>
		public static int GetTemplateArgumentIndex(string templateCall, int argument)
		{
			string pipecleanedtemplate = PipeCleanedTemplate(templateCall);
			int count = 1;

			foreach (Match m in TemplateArgument.Matches(pipecleanedtemplate))
			{
				if (count.Equals(argument))
					return m.Index+1;

				count++;
			}

			return -1;
		}

		private static readonly Regex TemplateArgument = new Regex(@"\|\s*([^{}\|]*?)\s*(?=\||}}$)", RegexOptions.Compiled);

		/// <summary>
		/// Returns the number of arguments to the input template call
		/// </summary>
		/// <param name="template">The template call</param>
		/// <param name="populatedparametersonly"> </param>
		/// <returns>The argument count</returns>
		public static int GetTemplateArgumentCount(string template, bool populatedparametersonly)
		{
			string pipecleanedtemplate = PipeCleanedTemplate(template);

			int i=0;
			
			foreach (Match m in (TemplateArgument.Matches(pipecleanedtemplate)))
			{
				if(!populatedparametersonly)
					i++;
				else if(m.Groups[1].Value.Contains("=") && !m.Groups[1].Value.EndsWith("="))
					i++;
			}
			
			return i;
		}
		
		public static int GetTemplateArgumentCount(string template)
		{
			return GetTemplateArgumentCount(template, false);
		}

		/// <summary>
		/// Renames the given template named parameter in the input template call
		/// </summary>
		/// <param name="templateCall">The template call to update</param>
		/// <param name="oldparameter">Existing parameter name</param>
		/// <param name="newparameter">New parameter name</param>
		/// <returns>The updated template call</returns>
		public static string RenameTemplateParameter(string templateCall, string oldparameter, string newparameter)
		{
			Regex paramRegex = new Regex(@"(\|\s*(?:<!--.*?-->)?)" + Regex.Escape(oldparameter) + @"(\s*(?:<!--.*?-->\s*)?=)");

			return (paramRegex.Replace(templateCall, m => RenameTemplateParameterME(m, templateCall, newparameter)));
		}

		/// <summary>
		/// Renames the given template named parameters in the input template
		/// </summary>
		/// <param name="templateCall">The template to update</param>
		/// <param name="oldparameters">List of existing template names</param>
		/// <param name="newparameter">New parameter name</param>
		/// <returns>The updated template call</returns>
		public static string RenameTemplateParameter(string templateCall, List<string> oldparameters, string newparameter)
		{
			string oldparam = "(?:";
			foreach (string oldparameter in oldparameters)
				oldparam += Regex.Escape(oldparameter) + @"|";

			oldparam = oldparam.TrimEnd('|') + @")";
			Regex paramRegex = new Regex(@"(\|\s*(?:<!--.*?-->)?)" + oldparam + @"(\s*(?:<!--.*?-->\s*)?=)");

			return (paramRegex.Replace(templateCall, m => RenameTemplateParameterME(m, templateCall, newparameter)));
		}
		
		/// <summary>
		/// Renames the given template named parameters in the input template
		/// </summary>
		/// <param name="templateCall">The template to update</param>
		/// <param name="Params">Dictionary of old names, new names to apply</param>
		/// <returns>The updated template call</returns>
		public static string RenameTemplateParameter(string templateCall, Dictionary<string, string> Params)
		{
			foreach(KeyValuePair<string, string> kvp in Params)
			{
				templateCall = RenameTemplateParameter(templateCall, kvp.Key, kvp.Value);
			}
			
			return templateCall;
		}
		
		private static string RenameTemplateParameterME(Match m, string templateCall, string newparameter)
		{
			// check for nested templates within the main template to avoid changing their parameter names
			foreach(Match n in WikiRegexes.NestedTemplates.Matches("  " + templateCall.Substring(2)))
			{
				if(n.Index > 0 && m.Index >= n.Index && m.Index <= (n.Index+n.Length))
					return m.Value;
			}
			
			return (m.Groups[1].Value + newparameter + m.Groups[2].Value);
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
			return NestedTemplateRegex(templatename).Replace(articletext, m => RemoveTemplateParameter(m.Value, parameter));
		}
		
		/// <summary>
		/// Removes the input parameter from the input template
		/// </summary>
		/// <param name="templateCall">The template call to update</param>
		/// <param name="parameter">The parameter to remove</param>
		/// <returns>The updated template call</returns>
		public static string RemoveTemplateParameter(string templateCall, string parameter)
		{
			return RemoveTemplateParameter(templateCall, parameter, false);
		}
		
		/// <summary>
		/// Removes the input parameters from the input template
		/// </summary>
		/// <param name="templateCall">The template call to update</param>
		/// <param name="parameters">The parameters to remove</param>
		/// <returns>The updated template call</returns>
		public static string RemoveTemplateParameters(string templateCall, List<string> parameters)
		{
			foreach(string parameter in parameters)
				templateCall = RemoveTemplateParameter(templateCall, parameter, false);
			
			return templateCall;
		}

		/// <summary>
		/// Removes the input parameter from the input template
		/// </summary>
		/// <param name="templateCall"></param>
		/// <param name="parameter"></param>
		/// <param name="removeLastMatch">Whether to remove the last match, rather than the first</param>
		/// <returns>The updated template</returns>
		public static string RemoveTemplateParameter(string templateCall, string parameter, bool removeLastMatch)
		{
			Regex paramRegex = new Regex(@"\|\s*" + Regex.Escape(parameter) + @"\s*=(.*?)(?=\||}}$)", RegexOptions.Singleline);

			string pipecleanedtemplate = PipeCleanedTemplate(templateCall);

			Match m = paramRegex.Match(pipecleanedtemplate);

			if (m.Success)
			{
				if(removeLastMatch)
				{
					foreach(Match y in paramRegex.Matches(pipecleanedtemplate))
						m = y;
				}
				
				int start = m.Index;
				return (templateCall.Substring(0, start) + templateCall.Substring(start + m.Length));
			}

			return templateCall;
		}
		
		private static readonly Regex CiteUrl = new Regex(@"\|\s*url\s*=\s*([^\[\]<>""\s]+)", RegexOptions.Compiled);

		/// <summary>
		/// Removes duplicate (same or null) named parameters from template calls
		/// </summary>
		/// <param name="templatecall">The template call to clean up</param>
		/// <returns>The updated template call</returns>
		public static string RemoveDuplicateTemplateParameters(string templatecall)
		{
			Dictionary<string, string> Params = new Dictionary<string, string>();
			return RemoveDuplicateTemplateParameters(templatecall, Params);
		}

		/// <summary>
		/// Removes duplicate (same or null) named parameters from template calls
		/// </summary>
		/// <param name="templatecall">The template call to clean up</param>
		/// <param name="Params">Dictionary of parameter name and value found in template call</param>
		/// <returns>The updated template call</returns>
		public static string RemoveDuplicateTemplateParameters(string templatecall, Dictionary<string, string> Params)
		{
			string originalTemplateCall = templatecall, updatedTemplateCall = "";

			while(!updatedTemplateCall.Equals(templatecall))
			{
				Params.Clear();
				string pipecleanedtemplate = PipeCleanedTemplate(templatecall);
				updatedTemplateCall = templatecall;
				
				foreach(Match m in anyParam.Matches(pipecleanedtemplate))
				{
					string paramValue = templatecall.Substring(m.Groups[2].Index, m.Groups[2].Length).Trim(),
					paramName = m.Groups[1].Value;
					
					if(!Params.ContainsKey(paramName))
						Params.Add(paramName, paramValue);
					else
					{
                        // do not remove parameter if ends with digit (e.g. last2 parameter), more likely needs renaming (e.g. last3) than removing 
                        if(Regex.IsMatch(paramName, @".+[0-9]$"))
                            continue;

						string earlierParamValue;
						Params.TryGetValue(paramName, out earlierParamValue);
						
						// remove this param if equal value to earlier one, or either value blank, or earlier one contains this one
						if(paramValue.Equals(earlierParamValue) || paramValue.Length == 0 || earlierParamValue.Length == 0
						   || earlierParamValue.Contains(paramValue) || paramValue.Contains(earlierParamValue))
						{
							bool removeLastmatch = (paramValue.Length == 0 || (!paramValue.Equals(earlierParamValue) && earlierParamValue.Contains(paramValue)));
							templatecall = RemoveTemplateParameter(templatecall, paramName, removeLastmatch);
							break;
						}
					}
				}
			}

            // check for URL breakage due to unescaped pipes in URL
			if(!templatecall.Equals(originalTemplateCall))
            {
                string originalURL = CiteUrl.Match(originalTemplateCall).Groups[1].Value.TrimEnd("|".ToCharArray());

                if(originalURL.Length > 0 && !CiteUrl.Match(templatecall).Groups[1].Value.Equals(originalURL))
                    return originalTemplateCall;
            }

			return templatecall;
		}
		
		private static readonly Regex anyParam = new Regex(@"\|\s*([^{}\|<>\r\n]+?)\s*=\s*([^}|]*)(?=\||}}$)", RegexOptions.Singleline);
		
		/// <summary>
		/// Returns duplicate named parameters in a template call
		/// </summary>
		/// <param name="templatecall">The template call to check</param>
		/// <returns>Dictionary of any duplicate parameters: index and length</returns>
		public static Dictionary<int, int> DuplicateTemplateParameters(string templatecall)
		{
			Dictionary<int, int> Dupes = new Dictionary<int, int>();
			
			Dictionary<string, string> Params = new Dictionary<string, string>();
			string pipecleanedtemplate = PipeCleanedTemplate(templatecall);
			
			foreach(Match m in anyParam.Matches(pipecleanedtemplate))
			{
				string paramValue = templatecall.Substring(m.Groups[2].Index, m.Groups[2].Length).Trim(),
				paramName = m.Groups[1].Value;
				
				if(!Params.ContainsKey(paramName))
					Params.Add(paramName, paramValue);
				else
					Dupes.Add(m.Index, m.Length);
			}
			return Dupes;
		}

		private static readonly Regex SpacedPipes = new Regex(@"(\|\s*)(?:\||}})");
		
		/// <summary>
		/// Removes excess pipes from template calls, any where two pipes with no value/only whitespace between
		/// </summary>
		/// <param name="templatecall">The template call to clean up</param>
		/// <returns>The updated template call</returns>
		public static string RemoveExcessTemplatePipes(string templatecall)
		{
            if(SpacedPipes.IsMatch(templatecall))
            {
                string originalURL = CiteUrl.Match(templatecall).Groups[1].Value.TrimEnd("|".ToCharArray()), originalTemplateCall = templatecall;
                string pipecleanedtemplate = PipeCleanedTemplate(templatecall);

                while (SpacedPipes.IsMatch(pipecleanedtemplate))
                {
                    Match m = SpacedPipes.Match(pipecleanedtemplate);

                    // imatch is like "|   |" or "| }}" so remove first | and whitespace
                    templatecall = templatecall.Remove(m.Index, m.Groups[1].Length);

                    pipecleanedtemplate = PipeCleanedTemplate(templatecall);
                }

                // check for URL breakage due to unescaped pipes in URL
                if(originalURL.Length > 0 && !CiteUrl.Match(templatecall).Groups[1].Value.Equals(originalURL))
                    return originalTemplateCall;
            }

			return templatecall;
		}

		/// <summary>
		/// Checks template calls using named parameters for unknown parameters
		/// </summary>
		/// <param name="templatecall">The template call to check</param>
		/// <param name="knownParameters">List of known template parameters</param>
		/// <returns>List of any unknown parameters</returns>
		public static List<string> UnknownTemplateParameters(string templatecall, List<string> knownParameters)
		{
			List<string> Unknowns = new List<string>();

			string pipecleanedtemplate = PipeCleanedTemplate(templatecall);
			
			foreach(Match m in anyParam.Matches(pipecleanedtemplate))
			{
				string paramName = m.Groups[1].Value;
				
				if(!knownParameters.Contains(paramName))
					Unknowns.Add(paramName);
			}
			return Unknowns;
		}

		private static readonly Regex StartWhitespace = new Regex(@"^\s+");

		/// <summary>
		/// Sets the template parameter value to the new value input, only if the template already has the parameter (with or without a value)
		/// </summary>
		/// <param name="templateCall">The template call to update</param>
		/// <param name="parameter">The template parameter</param>
		/// <param name="newvalue">The new value for the parameter</param>
		/// <returns>The updated template call</returns>
		public static string UpdateTemplateParameterValue(string templateCall, string parameter, string newvalue)
		{
			// HACK we are allowing matching on tilde character around parameter name to represent cleaned HTML comment, so may falsely match
			// on stray templates with stray tildes. Will that ever happen?
			Regex paramRegex = new Regex(@"\|[\s~]*" + Regex.Escape(parameter) + @"[\s~]*= *\s*?(.*?)\s*(?=(?:\||}}$))", RegexOptions.Singleline);

			string pipecleanedtemplate = PipeCleanedTemplate(templateCall, true);

			Match m = paramRegex.Match(pipecleanedtemplate);

			if (m.Success)
			{
				int start = m.Groups[1].Index, valuelength = m.Groups[1].Length;

				// retain any newlines at start of parameter value if existing parameter has value
				string startNewline = StartWhitespace.Match(m.Groups[1].Value).Value;
				if(startNewline.Length > 0 && m.Groups[1].Value.Trim().Length > 0)
					newvalue = startNewline + newvalue;

				return (templateCall.Substring(0, start) + newvalue + templateCall.Substring(start + valuelength));
			}

			return templateCall;
		}

		/// <summary>
		/// Merges the values of given parameters to a new one for a template call
		/// </summary>
		/// <param name="templateCall">The template call</param>
		/// <param name="parameters">List of parameters requested</param>
		/// <param name="newparameter">The new value for the parameter</param>
		/// <param name="caseInsensitiveParameterNames">Whether to match case insensitively on parameter name</param>
		/// <returns>List of parameter values</returns>
		public static string MergeTemplateParametersValues(string templateCall, List<string> parameters,  string newparameter, bool caseInsensitiveParameterNames)
		{
			string combined = "";

			foreach (string p in parameters)
			{
				combined +=(GetTemplateParameterValue(templateCall, p) + " ");
				templateCall = RemoveTemplateParameter(templateCall, p, false);
			}

			templateCall = AppendParameterToTemplate(templateCall, newparameter, combined, false);
			return templateCall;
		}

		private const char RepWith = '#';
		/// <summary>
		/// Removes pipes that are not the pipe indicating the end of the parameter's value
		/// </summary>
		/// <param name="templateCall">The template call to clean</param>
		/// <param name="commentsastilde"></param>
		/// <returns>The pipe cleaned template call</returns>
		public static string PipeCleanedTemplate(string templateCall, bool commentsastilde)
		{
			if (templateCall.Length < 5)
				return templateCall;

			string restoftemplate = templateCall.Substring(3);
			// clear out what may contain pipes that are not the pipe indicating the end of the parameter's value
			// Contains checks for performance gain
			if(restoftemplate.Contains(@"{{"))
				restoftemplate = ReplaceWith(restoftemplate, WikiRegexes.NestedTemplates, RepWith);
			if(restoftemplate.Contains(@"[["))
				restoftemplate = ReplaceWith(restoftemplate, WikiRegexes.SimpleWikiLink, RepWith);
			if(restoftemplate.Contains(@"<"))
			{
				restoftemplate = commentsastilde
					? ReplaceWith(restoftemplate, WikiRegexes.Comments, '~')
					: ReplaceWithSpaces(restoftemplate, WikiRegexes.Comments);

				restoftemplate = ReplaceWith(restoftemplate, WikiRegexes.AllTags, RepWith);
			}
			return (templateCall.Substring(0, 3) + restoftemplate);
		}

		private static string PipeCleanedTemplate(string template)
		{
			return PipeCleanedTemplate(template, false);
		}

		/// <summary>
		/// Sets the template parameter value to the new value input: if the template already has the parameter then its value is updated, otherwise the new value is appended
		/// </summary>
		/// <param name="templateCall">The template call to update</param>
		/// <param name="parameter">The template parameter</param>
		/// <param name="newvalue">The new value for the parameter</param>
		/// <param name="prependSpace">Whether to include a space before the new value</param>
		/// <returns>The updated template call</returns>
		public static string SetTemplateParameterValue(string templateCall, string parameter, string newvalue, bool prependSpace)
		{
			if(GetTemplateParameterValue(templateCall, parameter).Equals(newvalue))
				return templateCall;
			
			if(prependSpace)
				newvalue = " " + newvalue;
			
			// first try to update existing field's value
			string updatedtemplate = UpdateTemplateParameterValue(templateCall, parameter, newvalue);

			// if no update then append value in new field
			if (updatedtemplate.Equals(templateCall))
				updatedtemplate = AppendParameterToTemplate(templateCall, parameter, newvalue);

			return updatedtemplate;
		}
		
		/// <summary>
		/// Sets the template parameter value to the new value input: if the template already has the parameter then its value is updated, otherwise the new value is appended
		/// </summary>
		/// <param name="templateCall">The template call to update</param>
		/// <param name="parameter">The template parameter</param>
		/// <param name="newvalue">The new value for the parameter</param>
		/// <returns>The updated template call</returns>
		public static string SetTemplateParameterValue(string templateCall, string parameter, string newvalue)
		{
			return SetTemplateParameterValue(templateCall, parameter, newvalue, false);
		}

		/// <summary>
		/// Returns the name of the input template, excess whitespace removed, underscores replaced with spaces
		/// </summary>
		/// <param name="templateCall">the template call</param>
		/// <returns>the template name</returns>
		public static string GetTemplateName(string templateCall)
		{
			string name = WikiRegexes.TemplateNameRegex.Match(templateCall).Groups[1].Value;
			return Regex.Replace(name, @"[\s_]+", " ");
		}

		public static Regex TemplateNameRegex()
		{
			string TemplateNamespace;

			try
			{
				TemplateNamespace = Variables.NamespacesCaseInsensitive[Namespace.Template];
			}
			catch
			{
				TemplateNamespace = "[Tt]emplate:";
			}

			// allow whitespace before semicolon
			TemplateNamespace = Regex.Replace(TemplateNamespace, @":$", @"[\s_]*:");

			return (new Regex(@"{{\s*(?::?[\s_]*" + TemplateNamespace + @"[\s_]*)?([^\|{}]+?)(?:\s*(?:<!--.*?-->|⌊⌊⌊⌊M?\d+⌋⌋⌋⌋)\s*)?\s*(?:\||}})"));
		}

		/// <summary>
		/// Renames all matches of the given template name in the input text to the new name given
		/// </summary>
		/// <param name="articletext">the page text</param>
		/// <param name="templatename">the old template name</param>
		/// <param name="newtemplatename">the new template name</param>
		/// <returns>The updated article text</returns>
		public static string RenameTemplate(string articletext, string templatename, string newtemplatename)
		{
			return RenameTemplate(articletext, templatename, newtemplatename, true);
		}
		
		/// <summary>
		/// Renames all matches of the given template name in the input text to the new name given
		/// </summary>
		/// <param name="articletext">the page text</param>
		/// <param name="templatename">the old template name</param>
		/// <param name="newtemplatename">the new template name</param>
		/// <param name="keepFirstLetterCase">Whether to keep the first letter casing of the existing template</param>
		/// <returns>The updated article text</returns>
		public static string RenameTemplate(string articletext, string templatename, string newtemplatename, bool keepFirstLetterCase)
		{
			if(templatename.Equals(newtemplatename))
				return articletext;
			
			return NestedTemplateRegex(templatename).Replace(articletext, m => RenameTemplateME(m, newtemplatename, keepFirstLetterCase));
		}

		/// <summary>
		/// Renames the input template to the new name given
		/// </summary>
		/// <param name="templateCall">the template call</param>
		/// <param name="newtemplatename">the new template name</param>
		/// <returns></returns>
		public static string RenameTemplate(string templateCall, string newtemplatename)
		{
			return RenameTemplate(templateCall, newtemplatename, true);
		}
		
		/// <summary>
		/// Renames the input template to the new name given
		/// </summary>
		/// <param name="templateCall">the template call</param>
		/// <param name="newtemplatename">the new template name</param>
		/// <param name="keepFirstLetterCase">Whether to keep the first letter casing of the existing template. If false, casing of new template name is used.</param>
		/// <returns></returns>
		public static string RenameTemplate(string templateCall, string newtemplatename, bool keepFirstLetterCase)
		{
			return NestedTemplateRegex(GetTemplateName(templateCall)).Replace(templateCall, m => RenameTemplateME(m, newtemplatename, keepFirstLetterCase));
		}
		
		private static string RenameTemplateME(Match m, string newTemplateName, bool keepFirstLetterCase)
		{
			string originalTemplateName = m.Groups[2].Value;

			if (keepFirstLetterCase && !newTemplateName.StartsWith("subst:"))
			{
				newTemplateName = TurnFirstToUpper(originalTemplateName).Equals(originalTemplateName)
					? TurnFirstToUpper(newTemplateName)
					: TurnFirstToLower(newTemplateName);
			}

			return (m.Groups[1].Value + newTemplateName + m.Groups[3].Value);
		}

		/// <summary>
		/// Renames the input template to the new name given
		/// </summary>
		/// <param name="articletext">the page text</param>
		/// <param name="templatename">the template call</param>
		/// <param name="newtemplatename">the new template name</param>
		/// <param name="count">The number of templates to rename</param>
		/// <returns></returns>
		public static string RenameTemplate(string articletext, string templatename, string newtemplatename, int count)
		{
			return NestedTemplateRegex(templatename).Replace(articletext, "$1" + newtemplatename + "$3", count);
		}

		private const string NestedTemplateRegexStart = @"({{[\s_]*)(?:";
		private const string NestedTemplateRegexEnd = @"([\s_]*(?:<!--[^>]*?-->\s*|⌊⌊⌊⌊M?\d+⌋⌋⌋⌋\s*)?(\|((?>[^\{\}]+|\{(?<DEPTH>)|\}(?<-DEPTH>))*(?(DEPTH)(?!))))?\}\})";

		/// <summary>
		/// Returns a regex to match the input template
		/// Supports nested templates and comments at end of template call
		/// </summary>
		/// <param name="templatename">The template name</param>
		/// <returns>A Regex matching calls to the template, match group 2 being the template name, group 3 is template arguments from bar to end, group 4 is arguments from bar without final }}</returns>
		public static Regex NestedTemplateRegex(string templatename)
		{
			if (templatename.Length == 0)
				return null;

			return NestedTemplateRegex(new [] { templatename }, false);
		}
		
		/// <summary>
		/// Returns a regex to match the input template
		/// Supports nested templates and comments at end of template call
		/// </summary>
		/// <param name="templatename">The template name</param>
		/// <param name="compiled">Whether to return a compiled regex</param>
		/// <returns>A Regex matching calls to the template, match group 2 being the template name, group 3 is template arguments from bar to end, group 4 is arguments from bar without final }}</returns>
		public static Regex NestedTemplateRegex(string templatename, bool compiled)
		{
			if (templatename.Length == 0)
				return null;

			return NestedTemplateRegex(new [] { templatename }, compiled);
		}

		/// <summary>
		/// Returns a regex to match the input templates
		/// Supports nested templates and comments at end of template call
		/// </summary>
		/// <param name="templatenames">The list of template names</param>
		/// <returns>A Regex matching calls to the template, match group 2 being the template name, group 3 is template arguments from bar to end, group 4 is arguments from bar without final }}</returns>
		public static Regex NestedTemplateRegex(ICollection<string> templatenames)
		{
			return NestedTemplateRegex(templatenames, false);
		}
		
		/// <summary>
		/// Returns a regex to match the input templates
		/// Supports nested templates and comments at end of template call
		/// </summary>
		/// <param name="templatenames">The list of template names</param>
		/// <param name="compiled">Whether to return a compiled regex</param>
		/// <returns>A Regex matching calls to the template, match group 2 being the template name, group 3 is template arguments from bar to end, group 4 is arguments from bar without final }}</returns>
		public static Regex NestedTemplateRegex(ICollection<string> templatenames, bool compiled)
		{
			if (templatenames.Count == 0)
				return null;

			string TemplateNamespace;

			if (!Variables.NamespacesCaseInsensitive.TryGetValue(Namespace.Template, out TemplateNamespace))
				TemplateNamespace = "[Tt]emplate:";

			// support (deprecated) {{msg:Foo}} syntax
			if(string.IsNullOrEmpty(Variables.LangCode) || Variables.LangCode.Equals("en"))
				TemplateNamespace = Regex.Replace(TemplateNamespace, @"(.+?):$", @"(?:$1|[Mm]sg):");
			
			// allow whitespace before semicolon
			TemplateNamespace = Regex.Replace(TemplateNamespace, @":$", @"[\s_]*:");

			StringBuilder theRegex = new StringBuilder(NestedTemplateRegexStart + @":?[\s_]*" + TemplateNamespace + @"[\s_]*\s*)?(");

			foreach (string templatename in templatenames)
			{
				string templatename2 = Regex.Escape(templatename.Trim().Replace('_', ' ')).Replace(@"\ ", @"[_ ]+");
				theRegex.Append(CaseInsensitive(templatename2));
				theRegex.Append("|");
			}

			theRegex[theRegex.Length - 1] = ')';
			theRegex.Append(NestedTemplateRegexEnd);

			if (compiled)
				return new Regex(theRegex.ToString(), RegexOptions.Compiled);

			return new Regex(theRegex.ToString());
		}
		
		/// <summary>
		/// Returns whether template call is a section template or has reason parameter
		/// </summary>
		/// <param name="templateCall"></param>
		/// <returns></returns>
		public static bool IsSectionOrReasonTemplate(string templateCall)
		{
			return IsSectionOrReasonTemplate(templateCall, "");
		}
		
		/// <summary>
		/// Returns whether template call is a section template or has reason parameter
		/// Checks articletext for any {{multiple issues}} section templates
		/// </summary>
		/// <param name="templateCall"></param>
		/// <param name="articletext"></param>
		/// <returns></returns>
		public static bool IsSectionOrReasonTemplate(string templateCall, string articletext)
		{
			return ((WikiRegexes.NestedTemplates.IsMatch(templateCall) && GetTemplateArgument(templateCall, 1).Equals("section"))
					|| GetTemplateParameterValue(WikiRegexes.MultipleIssues.Match(articletext).Value, "section").Equals("y")
					|| GetTemplateParameterValue(templateCall, "reason", true).Length > 0);
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
		
		private static readonly Regex LowerMagicWordTemplates = Tools.NestedTemplateRegex(new[] { "namespace", "numberofarticles", "padleft" });
		/// <summary>
		/// Replaces magic word templates with magic words, ignores templates with no arguments
		/// </summary>
		/// <param name="articleText">The article text</param>
		/// <returns>The updated article text</returns>
		public static string TemplateToMagicWord(string articleText)
		{
			return WikiRegexes.MagicWordTemplates.Replace(articleText, TemplateToMagicWordME);
		}
		
		private static string TemplateToMagicWordME(Match m)
		{
			if(GetTemplateArgumentCount(m.Value) == 0)
				return m.Value;

			// namespace, numberofarticles, padleft must be lowercase, others must be UPPERCASE
			return @"{{" + (LowerMagicWordTemplates.IsMatch(m.Value.ToLower()) ? m.Groups[2].Value.ToLower() : m.Groups[2].Value.ToUpper()) + @":" + m.Groups[3].Value.Trim().TrimStart('|');
		}

		public static string ListToStringCommaSeparator(List<string> items)
		{
			if (Variables.LangCode.Equals("ar") || Variables.LangCode.Equals("arz") || Variables.LangCode.Equals("fa"))
				return string.Join("، ", items.ToArray());
			return string.Join(", ", items.ToArray());
		}

		/// <summary>
		/// Creates a string from a list with the following additions: 
		/// Specify a separator to be used between all elements in the list
		/// Specify a suffix to be added to each element in the list
		/// Specify a lastseparator that will be used between the last two elements in the list
		/// </summary>
		public static string ListToStringWithSeparatorAndWordSuffix(List<string> items, string separator, string suffix, string lastseparator)
		{
			string ret = "";
			for(int i = 0; i < items.Count; i++)
			{
				if(i + 1 == items.Count)
					ret += items[i] + suffix + lastseparator;
				else if(i == items.Count)
					ret += items[i] + suffix;
				else 
					ret += items[i] + suffix + separator;
			}
			return ret;
		}
		
		/// <summary>
		/// Replaces escaped characters in XML with the single character: apotrophe, quote, greater than, less than, ampersand
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static string UnescapeXML(string s)
		{
			if (string.IsNullOrEmpty(s)) return s;

			string returnString = s;
			returnString = returnString.Replace("&apos;", "'");
			returnString = returnString.Replace("&quot;", "\"");
			returnString = returnString.Replace("&gt;", ">");
			returnString = returnString.Replace("&lt;", "<");
			return returnString.Replace("&amp;", "&");
		}
		
		public static string ReAddDiacritics(string WithDiacritics, string WithoutDiacritics)
		{
			foreach(Match m in WikiRegexes.RegexWordApostrophes.Matches(WithDiacritics))
			{
				string withoutWord = RemoveDiacritics(m.Value);

				// don't replace if multiple matches, cannot be sure which word translates
				if(Regex.Matches(WithoutDiacritics, @"\b" + Regex.Escape(withoutWord) + @"\b").Count == 1)
					WithoutDiacritics = Regex.Replace(WithoutDiacritics, @"\b" + Regex.Escape(withoutWord) + @"\b", m.Value);
			}

			return WithoutDiacritics;
		}

		/// <summary>
		/// Calculates md5sum of a string, see http://msdn.microsoft.com/en-us/library/system.security.cryptography.md5%28v=vs.110%29.aspx
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static string GetMd5Sum(string input)
		{
			System.Security.Cryptography.MD5 md5Hasher = System.Security.Cryptography.MD5.Create();

			byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

			StringBuilder res = new StringBuilder();

			// Format each byte as a hexadecimal string.
			for (int i = 0; i < data.Length; i++)
			{
				res.Append(data[i].ToString("x2"));
			}

			return res.ToString();
		}
	}
}
