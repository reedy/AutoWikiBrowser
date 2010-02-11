/*
Copyright (C) 2007 Martin Richards, 2008 Stephen Kennedy

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
using System.Windows.Forms;
using System.Text.RegularExpressions;
using WikiFunctions.Lists.Providers;
using WikiFunctions.Plugin;
using WikiFunctions.Background;
using System.Net;
using System.Threading;

namespace WikiFunctions
{
    public enum ProjectEnum { wikipedia, wiktionary, wikisource, wikiquote, wikiversity, wikibooks, wikinews, species, commons, meta, mediawiki, wikia, custom }

    /// <summary>
    /// Holds static variables, to allow functionality on different wikis.
    /// </summary>
    public static partial class Variables
    {
        static Variables()
        {
            CanonicalNamespaces[-2] = "Media:";
            CanonicalNamespaces[-1] = "Special:";
            CanonicalNamespaces[1] = "Talk:";
            CanonicalNamespaces[2] = "User:";
            CanonicalNamespaces[3] = "User talk:";
            CanonicalNamespaces[4] = "Project:";
            CanonicalNamespaces[5] = "Project talk:";
            CanonicalNamespaces[6] = "File:";
            CanonicalNamespaces[7] = "File talk:";
            CanonicalNamespaces[8] = "MediaWiki:";
            CanonicalNamespaces[9] = "MediaWiki talk:";
            CanonicalNamespaces[10] = "Template:";
            CanonicalNamespaces[11] = "Template talk:";
            CanonicalNamespaces[12] = "Help:";
            CanonicalNamespaces[13] = "Help talk:";
            CanonicalNamespaces[14] = "Category:";
            CanonicalNamespaces[15] = "Category talk:";

            CanonicalNamespaceAliases = PrepareAliases(CanonicalNamespaces);

            CanonicalNamespaceAliases[6].Add("Image:");
            CanonicalNamespaceAliases[7].Add("Image talk:");

            if (!Globals.UnitTestMode)
                SetProject("en", ProjectEnum.wikipedia); //Shouldn't have to load en defaults...
            else
            {
                SetToEnglish();
                RegenerateRegexes();
            }

            PHP5 = false;
            TypoSummaryTag = "typos fixed: ";
            AWBDefaultSummaryTag();
        }

        /// <summary>
        /// Gets the SVN revision of the current build and the date it was committed
        /// </summary>
        public static string Revision
        {
            get // see SvnInfo.template.cs for details
            {
                return (!m_Revision.Contains("$")) ? m_Revision.Replace("/", "-") : "?"; //fallback in case of failed revision extraction
            }
        }

        public static string RetfPath;

        public static IAutoWikiBrowser MainForm
        { get; set; }

        public static Profiler Profiler = new Profiler();


        #region project and language settings

        /// <summary>
        /// Provides access to the en namespace keys
        /// </summary>
        public static readonly Dictionary<int, string> CanonicalNamespaces = new Dictionary<int, string>(20);

        /// <summary>
        /// Canonical namespace aliases
        /// </summary>
        public static readonly Dictionary<int, List<string>> CanonicalNamespaceAliases;

        /// <summary>
        /// Provides access to the namespace keys
        /// </summary>
        public static Dictionary<int, string> Namespaces = new Dictionary<int, string>(40);

        /// <summary>
        /// Aliases for current namspaces
        /// </summary>
        public static Dictionary<int, List<string>> NamespaceAliases;

        /// <summary>
        /// Provides access to the namespace keys in a form so the first letter is case insensitive e.g. [Ww]ikipedia:
        /// </summary>
        public static readonly Dictionary<int, string> NamespacesCaseInsensitive = new Dictionary<int, string>(24);

        /// <summary>
        /// 
        /// </summary>
        public static Dictionary<string, List<string>> MagicWords = new Dictionary<string, List<string>>();

        /// <summary>
        /// Gets a URL of the site, e.g. "http://en.wikipedia.org/w/"
        /// </summary>
        public static string URLLong
        { get { return URL + URLEnd; } }

        /// <summary>
        /// Gets a Index URL of the site, e.g. "http://en.wikipedia.org/w/index.php"
        /// </summary>
        public static string URLIndex
        { get { return URLLong + IndexPHP; } }

        /// <summary>
        /// Gets a Index URL of the site, e.g. "http://en.wikipedia.org/w/api.php"
        /// </summary>
        public static string URLApi
        { get { return URLLong + ApiPHP; } }

        /// <summary>
        /// true if current wiki uses right-to-left writing system
        /// </summary>
        public static bool RTL
        { get; set; }

        /// <summary>
        /// localized names of months
        /// </summary>
        public static string[] MonthNames;

        public static readonly string[] ENLangMonthNames = new[]{"January", "February", "March", "April", "May", "June",
                "July", "August", "September", "October", "November", "December"};

        private static string URLEnd = "/w/";

        /// <summary>
        /// Gets a URL of the site, e.g. "http://en.wikipedia.org".
        /// </summary>
        public static string URL = "http://en.wikipedia.org";

        public static string Host { get { return new Uri(URL).Host; } }

        /// <summary>
        /// Returns the script path of the site, e.g. /w
        /// </summary>
        public static string ScriptPath
        {
            get { return URLEnd.Substring(0, URLEnd.LastIndexOf('/')); }
        }

        /// <summary>
        /// Gets a name of the project, e.g. "wikipedia".
        /// </summary>
        public static ProjectEnum Project { get; private set; }

        /// <summary>
        /// Gets the language code, e.g. "en".
        /// </summary>
        public static string LangCode { get; internal set; }

        /// <summary>
        /// Returns true if we are currently editing a WMF wiki
        /// </summary>
        public static bool IsWikimediaProject
        { get { return Project <= ProjectEnum.species; } }

        /// <summary>
        /// Returns true if we are currently editing the English Wikipedia
        /// </summary>
        public static bool IsWikipediaEN
        { get { return (Project == ProjectEnum.wikipedia && LangCode == "en"); } }

        /// <summary>
        /// Returns true if we are currently a monolingual Wikimedia project
        /// </summary>
        public static bool IsWikimediaMonolingualProject
        {
            get
            {
                return (Project == ProjectEnum.commons || Project == ProjectEnum.meta
                  || Project == ProjectEnum.species || Project == ProjectEnum.mediawiki);
            }
        }

        /// <summary>
        /// Returns true if we are currently editing a "custom" wiki
        /// </summary>
        public static bool IsCustomProject
        { get { return Project == ProjectEnum.custom; } }

        /// <summary>
        /// Returns true if we are currently editing a Wikia site
        /// </summary>
        public static bool IsWikia
        { get { return Project == ProjectEnum.wikia; } }

        /// <summary>
        /// Gets script path of a custom project or empty string if standard project
        /// </summary>
        public static string CustomProject
        { get; private set; }

        /// <summary>
        /// index.php appended with "5" if appropriate for the wiki
        /// </summary>
        public static string IndexPHP { get; private set; }

        /// <summary>
        /// api.php appended with "5" if appropriate for the wiki
        /// </summary>
        public static string ApiPHP { get; private set; }

        private static bool usePHP5;
        /// <summary>
        /// Whether the current wiki uses the .php5 extension
        /// </summary>
        public static bool PHP5
        {
            get { return usePHP5; }
            set
            {
                usePHP5 = value;
                IndexPHP = value ? "index.php5" : "index.php";
                ApiPHP = value ? "api.php5" : "api.php";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string TypoSummaryTag
        { get; private set; }

        private static string mSummaryTag = " using ";

        /// <summary>
        /// Gets the tag to add to the edit summary, e.g. " using [[Project:AWB]]".
        /// </summary>
        public static string SummaryTag
        { get { return mSummaryTag + WPAWB; } }

        public static string WPAWB
        { get; private set; }

        internal static Dictionary<int, List<string>> PrepareAliases(Dictionary<int, string> namespaces)
        {
            Dictionary<int, List<string>> ret = new Dictionary<int, List<string>>(namespaces.Count);

            // fill aliases with empty lists, to avoid KeyNotFoundException
            foreach (int n in namespaces.Keys)
            {
                ret[n] = new List<string>();
            }

            return ret;
        }

        private static void AWBDefaultSummaryTag()
        {
            mSummaryTag = " using ";
            WPAWB = "[[Project:AWB|AWB]]";
        }

        #region Delayed load stuff
        public static readonly List<string> UnderscoredTitles = new List<string>();
        private static readonly List<BackgroundRequest> DelayedRequests = new List<BackgroundRequest>();

        static void CancelBackgroundRequests()
        {
            lock (DelayedRequests)
            {
                foreach (BackgroundRequest b in DelayedRequests) b.Abort();
                DelayedRequests.Clear();
            }
        }

        /// <summary>
        /// Waits for all background stuff to be loaded
        /// </summary>
        public static void WaitForDelayedRequests()
        {
            do
            {
                lock(DelayedRequests)
                {
                    if (DelayedRequests.Count == 0) return;
                }
                Thread.Sleep(100);
            } while (true);
        }

        internal static void LoadUnderscores(params string[] cats)
        {
            BackgroundRequest r = new BackgroundRequest(UnderscoresLoaded) {HasUI = false};
            DelayedRequests.Add(r);
            r.GetList(new CategoryListProvider(), cats);
        }

        private static void UnderscoresLoaded(BackgroundRequest req)
        {
            lock (DelayedRequests)
            {
                DelayedRequests.Remove(req);
                UnderscoredTitles.Clear();
                foreach (Article a in (List<Article>)req.Result)
                {
                    UnderscoredTitles.Add(a.Name);
                }
            }
        }

        #endregion

        #region Proxy support
        static IWebProxy SystemProxy;

        public static HttpWebRequest PrepareWebRequest(string url, string userAgent)
        {
            HttpWebRequest r = (HttpWebRequest)WebRequest.Create(url);

            if (SystemProxy != null) r.Proxy = SystemProxy;

            r.UserAgent = string.IsNullOrEmpty(userAgent) ? Tools.DefaultUserAgentString : userAgent;

            r.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

            r.Proxy.Credentials = CredentialCache.DefaultCredentials;

            return r;
        }

        public static HttpWebRequest PrepareWebRequest(string url)
        { return PrepareWebRequest(url, ""); }

        public static void RefreshProxy()
        {
            SystemProxy = WebRequest.GetSystemWebProxy();
            if (SystemProxy.IsBypassed(new Uri(URL)))
            {
                SystemProxy = null;
            }
        }
        #endregion

        // for logging, these will probably need internationalising
        public static string AWBVersionString(string version)
        {
            return "*" + WPAWB + " version " + version + Environment.NewLine;
        }

        public static string Stub;
        public static string SectStub;
        public static Regex SectStubRegex;

        /// <summary>
        /// Sets different language variables, such as namespaces. Default is english Wikipedia
        /// </summary>
        /// <param name="langCode">The language code, default is en</param>
        /// <param name="projectName">The project name default is Wikipedia</param>
        public static void SetProject(string langCode, ProjectEnum projectName)
        {
            SetProject(langCode, projectName, "");
        }


        static readonly string[] AttackSites = new[]
            {
                "encyclopediadramatica",
                "conservapedia.com",
                "traditio.",
                "volgota.com",
                "wikireality.ru"
            };
        
        /// <summary>
        /// Sets the language code of the current project
        /// </summary>
        /// <param name="langCode">The new language code to use</param>
        public static void SetProjectLangCode(string langCode)
        {
            #if DEBUG
            LangCode = langCode;
            #endif
        }

        /// <summary>
        /// Sets different language variables, such as namespaces. Default is english Wikipedia
        /// </summary>
        /// <param name="langCode">The language code, default is en</param>
        /// <param name="projectName">The project name default is Wikipedia</param>
        /// <param name="customProject">Script path of a custom project or ""</param>
        public static void SetProject(string langCode, ProjectEnum projectName, string customProject)
        {
            Namespaces.Clear();
            CancelBackgroundRequests();
            UnderscoredTitles.Clear();

            foreach(var s in AttackSites)
                if (customProject.Contains(s))
                {
                    MessageBox.Show("This software does not work on attack sites.");
                    Application.ExitThread();
                }

            Project = projectName;
            LangCode = langCode;
            CustomProject = customProject;

            RefreshProxy();

            URLEnd = "/w/";

            AWBDefaultSummaryTag();
            Stub = "[Ss]tub";

            MonthNames = ENLangMonthNames;

            SectStub = @"\{\{[Ss]ect";
            SectStubRegex = new Regex(SectStub, RegexOptions.Compiled);

            if (IsCustomProject)
            {
                LangCode = "en";
                int x = customProject.IndexOf('/');

                if (x > 0)
                {
                    URLEnd = customProject.Substring(x, customProject.Length - x);
                    customProject = customProject.Substring(0, x);
                }

                URL = "http://" + CustomProject;
            }
            else
                URL = "http://" + LangCode + "." + Project + ".org";

            // HACK:
            switch (projectName)
            {
                case ProjectEnum.wikipedia:
                case ProjectEnum.wikinews:
                case ProjectEnum.wikisource:
                case ProjectEnum.wikibooks:
                case ProjectEnum.wikiquote:
                case ProjectEnum.wiktionary:
                case ProjectEnum.wikiversity:
                    switch (langCode)
                    {
                        case "en":
                            if (projectName == ProjectEnum.wikipedia)
                                SetToEnglish();
                            break;

                        case "ar":
                            mSummaryTag = " ";
                            WPAWB = "باستخدام [[ويكيبيديا:أوب|الأوتوويكي براوزر]]";
                            TypoSummaryTag = ".الأخطاء المصححة: ";
                            break;

                        case "bg":
                            mSummaryTag = " редактирано с ";
                            WPAWB = "AWB";
                            break;

                        case "ca":
                            mSummaryTag = " ";
                            WPAWB = "[[Viquipèdia:AutoWikiBrowser|AWB]]";
                            break;

                        case "da":
                            mSummaryTag = " ved brug af ";
                            WPAWB = "[[en:Wikipedia:AutoWikiBrowser|AWB]]";
                            break;

                        case "de":
                            mSummaryTag = " mit ";
                            TypoSummaryTag = ", Schreibweise:";
                            break;
						
						case "el":
							mSummaryTag = " με τη χρήση ";
                            WPAWB = "[[Βικιπαίδεια:AutoWikiBrowser|AWB]]";
							break;

                        case "eo":
                            mSummaryTag = " ";
                            WPAWB = "[[Vikipedio:AutoWikiBrowser|AWB]]";
                            break;

                        case "hu":
                            mSummaryTag = " ";
                            WPAWB = "[[Wikipédia:AutoWikiBrowser|AWB]]";
                            break;

                        case "ku":
                            mSummaryTag = " ";
                            WPAWB = "[[Wîkîpediya:AutoWikiBrowser|AWB]]";
                            break;

                        case "nl":
                            mSummaryTag = " met ";
                            break;

                        case "pl":
                            SectStub = @"\{\{[Ss]ek";
                            SectStubRegex = new Regex(SectStub, RegexOptions.Compiled);
                            break;

                        case "pt":
                            mSummaryTag = " utilizando ";
                            break;

                        case "ru":
                            mSummaryTag = " с помощью ";
                            Stub = "(?:[Ss]tub|[Зз]аготовка)";
                            break;

                        case "sk":
                            mSummaryTag = " ";
                            WPAWB = "[[Wikipédia:AutoWikiBrowser|AWB]]";
                            break;

                        case "sl":
                            mSummaryTag = " ";
                            WPAWB = "[[Wikipedija:AutoWikiBrowser|AWB]]";
                            Stub = "(?:[Ss]tub|[Šš]krbina)";
                            break;

                        case "uk":
                            Stub = "(?:[Ss]tub|[Дд]оробити)";
                            mSummaryTag = " за допомогою ";
                            WPAWB = "[[Вікіпедія:AutoWikiBrowser|AWB]]";
                            break;

                        // case "xx:
                        // strsummarytag = " ";
                        // strWPAWB = "";
                        // break;

                        default:
                            break;
                    }
                    break;
                case ProjectEnum.commons:
                    URL = "http://commons.wikimedia.org";
                    LangCode = "en";
                    break;
                case ProjectEnum.meta:
                    URL = "http://meta.wikimedia.org";
                    LangCode = "en";
                    break;
                case ProjectEnum.mediawiki:
                    URL = "http://www.mediawiki.org";
                    LangCode = "en";
                    break;
                case ProjectEnum.species:
                    URL = "http://species.wikimedia.org";
                    LangCode = "en";
                    break;
                case ProjectEnum.wikia:
                    URL = "http://" + customProject + ".wikia.com";
                    URLEnd = "/";
                    break;
                case ProjectEnum.custom:
                    URLEnd = "";
                    break;
            }

            //refresh once more in case project settings were reset due to error with loading
            RefreshProxy();

            //HACK:HACK:HACK:HACK:HACK:
            if (MainForm != null && MainForm.TheSession != null)
            {
                if (!MainForm.TheSession.UpdateProject())
                {
                    LangCode = "en";
                    Project = ProjectEnum.wikipedia;
                    SetToEnglish();
                }
            }

            RegenerateRegexes();

            RetfPath = Namespaces[Namespace.Project] + "AutoWikiBrowser/Typos";

            foreach (string s in Namespaces.Values)
            {
                System.Diagnostics.Trace.Assert(s.EndsWith(":"), "Internal error: namespace does not end with ':'.",
                                                "Please contact a developer.");
            }
            System.Diagnostics.Trace.Assert(!Namespaces.ContainsKey(0), "Internal error: key exists for namespace 0.",
                                            "Please contact a developer.");
        }

        /// <summary>
        /// 
        /// </summary>
        private static void RegenerateRegexes()
        {
            NamespacesCaseInsensitive.Clear();

            foreach (int ns in Namespaces.Keys)
            {
                NamespacesCaseInsensitive.Add(ns, "(?i:"
                    + WikiRegexes.GenerateNamespaceRegex(ns) + @")\s*:");
            }

            WikiRegexes.MakeLangSpecificRegexes();
        }

        /// <summary>
        /// 
        /// </summary>
        private static void SetToEnglish()
        {
            foreach (var i in CanonicalNamespaces.Keys)
                Namespaces[i] = CanonicalNamespaces[i];

            Namespaces[4] = "Wikipedia:";
            Namespaces[5] = "Wikipedia talk:";
            Namespaces[100] = "Portal:";
            Namespaces[101] = "Portal talk:";
            Namespaces[108] = "Book:";
            Namespaces[109] = "Book talk:";

            mSummaryTag = " using ";

            NamespaceAliases = CanonicalNamespaceAliases;

            MonthNames = ENLangMonthNames;
            SectStub = @"\{\{[Ss]ect";
            SectStubRegex = new Regex(SectStub, RegexOptions.Compiled);
            Stub = "[Ss]tub";

            LangCode = "en";

            RTL = false;
        }
        #endregion

        #region URL Builders
        /// <summary>
        /// returns URL to the given page, depends on project settings
        /// </summary>
        public static string NonPrettifiedURL(string title)
        {
            return URLIndex + "?title=" + Tools.WikiEncode(title);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public static string GetArticleHistoryURL(string title)
        {
            return (NonPrettifiedURL(title) + "&action=history");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public static string GetEditURL(string title)
        {
            return (NonPrettifiedURL(title) + "&action=edit");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public static string GetAddToWatchlistURL(string title)
        {
            return (NonPrettifiedURL(title) + "&action=watch");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public static string GetRemoveFromWatchlistURL(string title)
        {
            return (NonPrettifiedURL(title) + "&action=unwatch");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static string GetUserTalkURL(string username)
        {
            return URLIndex + "?title=User_talk:" + Tools.WikiEncode(username) + "&action=purge";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public static string GetPlainTextURL(string title)
        {
            return NonPrettifiedURL(title) + "&action=raw";
        }
        #endregion
    }

    public enum WikiStatusResult
    {
        Error, 
        NotLoggedIn,
        NotRegistered,
        OldVersion,
        NoRights,
        Registered,
        PendingUpdate
    }
}

