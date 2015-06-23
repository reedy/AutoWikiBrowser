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
using System.Text.RegularExpressions;
using WikiFunctions.Lists.Providers;
using WikiFunctions.Plugin;
using WikiFunctions.Background;
using System.Net;
using System.Threading;

namespace WikiFunctions
{
    public enum ProjectEnum
    {
        wikipedia,
        wiktionary,
        wikisource,
        wikiquote,
        wikiversity,
        wikivoyage,
        wikibooks,
        wikinews,
        species,
        commons,
        meta,
        mediawiki,
        incubator,
        wikia,
        custom
    }

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

            CapitalizeFirstLetter = true;
            IndexPHP = "index.php";
            ApiPHP = "api.php";
            TypoSummaryTag = "typos fixed: ";
            AWBDefaultSummaryTag();
            mSummaryTag = "using ";
            Protocol = "http://";
            NotificationsEnabled = true;
        }

        /// <summary>
        /// Gets the SVN revision of the current build and the date it was committed
        /// </summary>
        public static string Revision
        {
            get // see SvnInfo.template.cs for details
            {
                return (!m_Revision.Contains("$")) ? m_Revision.Replace("/", "-") : "?";
            }
        }

        /// <summary>
        /// Gets the SVN revision number of the current build
        /// </summary>
        public static int RevisionNumber
        {
            get
            {
                return !m_Revision.Contains("$")
                           ? int.Parse(m_Revision.Substring(0, m_Revision.IndexOf(' ')))
                           : 0;
            }
        }

        /// <summary>
        /// Page of RegexTypoFix rules page e.g. Project:AutoWikiBrowser/Typos.
        /// Can be full URL if specified as such on &lt;!--Typos--&gt; check page comment
        /// </summary>
        public static string RetfPath;

        /// <summary>
        /// 
        /// </summary>
        public static IAutoWikiBrowser MainForm { get; set; }

        /// <summary>
        /// Performance profiler
        /// </summary>
        public static Profiler Profiler = new Profiler();

        #region project and language settings

        /// <summary>
        /// Provides access to the en namespace keys e.g. Category:
        /// </summary>
        public static readonly Dictionary<int, string> CanonicalNamespaces = new Dictionary<int, string>(20);

        /// <summary>
        /// Canonical namespace aliases
        /// </summary>
        public static readonly Dictionary<int, List<string>> CanonicalNamespaceAliases;

        /// <summary>
        /// Provides access to the namespace keys e.g. Category:
        /// </summary>
        public static Dictionary<int, string> Namespaces = new Dictionary<int, string>(40);

        /// <summary>
        /// Aliases for current namespaces
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
        /// Gets a URL of the site, e.g. "https://en.wikipedia.org/w/"
        /// </summary>
        public static string URLLong
        {
            get { return URL + URLEnd; }
        }

        /// <summary>
        /// Gets a Index URL of the site, e.g. "https://en.wikipedia.org/w/index.php"
        /// </summary>
        public static string URLIndex
        {
            get { return URLLong + IndexPHP; }
        }

        /// <summary>
        /// Gets a Index URL of the site, e.g. "https://en.wikipedia.org/w/api.php"
        /// </summary>
        public static string URLApi
        {
            get { return URLLong + ApiPHP; }
        }

        public static string HttpAuthUsername { get; set; }

        public static string HttpAuthPassword { get; set; }

        /// <summary>
        /// true if current wiki uses right-to-left writing system
        /// </summary>
        public static bool RTL { get; set; }

        /// <summary>
        /// Whether user notifications (from MediaWiki Echo extension) are available on the wiki
        /// </summary>
        public static bool NotificationsEnabled { get; set; }

        /// <summary>
        /// Whether the wiki capitalizes first letter of page names (usually yes, e.g. English Wikipedia) or not (e.g. Wiktionary)
        /// </summary>
        public static bool CapitalizeFirstLetter { get; set; }

        /// <summary>
        /// localized names of months
        /// </summary>
        public static string[] MonthNames;

        public static readonly string[] ENLangMonthNames =
                                                               {
                                                                   "January", "February", "March", "April", "May",
                                                                   "June",
                                                                   "July", "August", "September", "October", "November",
                                                                   "December"
                                                               };

        private static string URLEnd = "/w/";

        /// <summary>
        /// Gets a URL of the site, e.g. "https://en.wikipedia.org".
        /// </summary>
        public static string URL = "https://en.wikipedia.org";

        public static string Host
        {
            get { return new Uri(URL).Host; }
        }

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
        {
            get { return Project <= ProjectEnum.species; }
        }

        /// <summary>
        /// Returns true if we are currently editing the English Wikipedia
        /// </summary>
        public static bool IsWikipediaEN
        {
            get { return (Project == ProjectEnum.wikipedia && LangCode == "en"); }
        }

        /// <summary>
        /// Returns true if we are currently editing Commons
        /// </summary>
        public static bool IsCommons
        {
            get { return (Project == ProjectEnum.commons); }
        }

        /// <summary>
        /// Returns true if we are currently a monolingual Wikimedia project
        /// </summary>
        public static bool IsWikimediaMonolingualProject
        {
            get
            {
                return (Project == ProjectEnum.commons || Project == ProjectEnum.meta
                        || Project == ProjectEnum.species || Project == ProjectEnum.mediawiki
                        || Project == ProjectEnum.incubator
                        );
            }
        }

        /// <summary>
        /// Returns true if we are currently editing a "custom" wiki
        /// </summary>
        public static bool IsCustomProject
        {
            get { return Project == ProjectEnum.custom; }
        }

        /// <summary>
        /// Returns true if we are currently editing a Wikia site
        /// </summary>
        public static bool IsWikia
        {
            get { return Project == ProjectEnum.wikia; }
        }

        /// <summary>
        /// Gets script path of a custom project or empty string if standard project
        /// </summary>
        public static string CustomProject { get; private set; }

        /// <summary>
        /// Login domain if needed
        /// </summary>
        public static string LoginDomain { get; set; }

        /// <summary>
        /// Protocol, HTTP or HTTPS?
        /// </summary>
        public static string Protocol { get; private set; }

        /// <summary>
        /// index.php appended with "5" if appropriate for the wiki
        /// </summary>
        public static string IndexPHP { get; private set; }

        /// <summary>
        /// api.php appended with "5" if appropriate for the wiki
        /// </summary>
        public static string ApiPHP { get; private set; }

        /// <summary>
        /// Typos wording e.g. "typos fixed: "
        /// Should not start with spaces or commas. Must end with a space
        /// </summary>
        public static string TypoSummaryTag { get; private set; }

        /// <summary>
        /// Localized version of " using " for edit summary tag
        /// Does not need spaces at start or end
        /// </summary>
        private static string mSummaryTag;

        /// <summary>
        /// Gets the tag to add to the edit summary, e.g. " using [[Project:AWB]]".
        /// </summary>
        public static string SummaryTag
        {
            get
            {
                string text = " " + mSummaryTag + " " + WPAWB;
#if DEBUG
                text += " (" + RevisionNumber + ")";
#endif
                return text;
            }
        }

        public static string WPAWB { get; private set; }

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

		/// <summary>
		/// Sets the AWB default summary tag.
		/// </summary>
        private static void AWBDefaultSummaryTag()
        {
            mSummaryTag = "using ";
            WPAWB = "[[Project:AWB|AWB]]";
        }

        #region Delayed load stuff

        /// <summary>
        /// Contains list of pages with underscores in titles, from [[Category:Articles with underscores in the title]] for en wiki
        /// </summary>
        public static readonly List<string> UnderscoredTitles = new List<string>();
        private static readonly List<BackgroundRequest> DelayedRequests = new List<BackgroundRequest>();

        /// <summary>
        /// For unit tests only, method to add entries to UnderscoredTitles
        /// </summary>
        /// <param name="titles"></param>
        public static void AddUnderscoredTitles(List<string> titles)
        {
            if (Globals.UnitTestMode)
                UnderscoredTitles.AddRange(titles);
        }

        private static void CancelBackgroundRequests()
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
                lock (DelayedRequests)
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
                foreach (Article a in (List<Article>) req.Result)
                {
                    UnderscoredTitles.Add(a.Name);
                }
            }
        }

        #endregion

        #region Proxy support

        private static IWebProxy SystemProxy;

        /// <summary>
        /// Creates an HTTP web request. Timeout set to 15 seconds
        /// </summary>
        /// <param name="url"></param>
        /// <param name="userAgent"></param>
        /// <returns></returns>
        public static HttpWebRequest PrepareWebRequest(string url, string userAgent)
        {
            HttpWebRequest r = (HttpWebRequest) WebRequest.Create(url);

            if (SystemProxy != null) r.Proxy = SystemProxy;

            r.UserAgent = string.IsNullOrEmpty(userAgent) ? Tools.DefaultUserAgentString : userAgent;

            r.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

            r.Proxy.Credentials = CredentialCache.DefaultCredentials;
            r.UseDefaultCredentials = true;
            r.Timeout = 15000; // override default timeout of 300,000 (= 5 minutes) down to 15 seconds

            return r;
        }

		/// <summary>
		/// Creates an HTTP web request. Timeout set to 15 seconds
		/// </summary>
		/// <returns>
		/// The web request.
		/// </returns>
		/// <param name='url'>
		/// URL.
		/// </param>
        public static HttpWebRequest PrepareWebRequest(string url)
        {
            return PrepareWebRequest(url, "");
        }

		/// <summary>
		/// Refreshs the system proxy.
		/// </summary>
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
            SetProject(langCode, projectName, "", "https://");
        }

#if DEBUG || UNITTEST
        /// <summary>
        /// Sets the language code of the current project
        /// </summary>
        /// <param name="langCode">The new language code to use</param>
        /// <remarks>Do not use this outside unit tests</remarks>
        public static void SetProjectLangCode(string langCode)
        {
            SetLanguageSpecificValues(langCode, ProjectEnum.wikipedia);
            LangCode = langCode;

            RTL = langCode.Equals("ar");
        }

        /// <summary>
        /// Sets different language variables, such as namespaces. Default is English Wikipedia
        /// </summary>
        /// <param name="langCode">The language code, default is en</param>
        /// <param name="projectName">The project name default is Wikipedia</param>
        /// <remarks>Do not use this outside unit tests</remarks>
        public static void SetProjectSimple(string langCode, ProjectEnum projectName)
        {
            Project = projectName;
            SetProjectLangCode(langCode);
        }
#endif

        public static bool TryLoadingAgainAfterLogin { get; private set; }
        public static ProjectHoldArea ReloadProjectSettings;

        public class ProjectHoldArea
        {
            public ProjectEnum projectName;
            public string langCode;
            public string customProject;
            public string protocol;
        }

        /// <summary>
        /// Sets different language variables, such as namespaces. Default is english Wikipedia
        /// </summary>
        /// <param name="langCode">The language code, default is en</param>
        /// <param name="projectName">The project name default is Wikipedia</param>
        /// <param name="customProject">Script path of a custom project or ""</param>
        /// <param name="protocol"></param>
        public static void SetProject(string langCode, ProjectEnum projectName, string customProject, string protocol)
        {
            TryLoadingAgainAfterLogin = false;
            Namespaces.Clear();
            CancelBackgroundRequests();
            UnderscoredTitles.Clear();
            WikiRegexes.TemplateRedirects.Clear();
            bool typoReloadNeeded = (LangCode != langCode || Project != projectName || customProject != CustomProject);

            Project = projectName;
            LangCode = langCode;
            CustomProject = customProject;
            Protocol = protocol;

            RefreshProxy();

            URLEnd = "/w/";

            Stub = "[^{}|]*?[Ss]tub";

            MonthNames = ENLangMonthNames;

            SectStub = @"\{\{[Ss]ect";
            SectStubRegex = new Regex(SectStub, RegexOptions.Compiled);

            TypoSummaryTag = "typos fixed: ";
            AWBDefaultSummaryTag();
            mSummaryTag = "using";
            NotificationsEnabled = true;

            if (IsCustomProject)
            {
                LangCode = "en";
                var uri = new Uri(Protocol + customProject);
                URLEnd = uri.AbsolutePath;
                URL = protocol + uri.Host;
                if (uri.Port != 80)
                {
                    URL += ":" + uri.Port;
                }
                CustomProject = customProject;
            }
            else
            {
                URL = "https://" + LangCode + "." + Project + ".org";
            }

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
                    //If not set the following will be used:
                    //mSummaryTag="using";
                    //WPAWB = "[[Project:Wikipedia:AutoWikiBrowser|AWB]]";
                    SetLanguageSpecificValues(langCode, projectName);
                    break;
                case ProjectEnum.commons:
                    URL = "https://commons.wikimedia.org";
                    LangCode = "en";
                    break;
                case ProjectEnum.meta:
                    URL = "https://meta.wikimedia.org";
                    LangCode = "en";
                    break;
                case ProjectEnum.mediawiki:
                    URL = "https://www.mediawiki.org";
                    LangCode = "en";
                    break;
                case ProjectEnum.incubator:
                    URL = "https://incubator.wikimedia.org";
                    LangCode = "en";
                    break;
                case ProjectEnum.species:
                    URL = "https://species.wikimedia.org";
                    LangCode = "en";
                    break;
                case ProjectEnum.wikia:
                    URL = "http://" + customProject + ".wikia.com";
                    URLEnd = "/";
                    break;
                case ProjectEnum.custom:
                    break;
            }

            //refresh once more in case project settings were reset due to error with loading
            RefreshProxy();

            //HACK:HACK:HACK:HACK:HACK:
            if (MainForm != null && MainForm.TheSession != null)
            {
                try
                {
                    if (!MainForm.TheSession.UpdateProject(false))
                    {
                        LangCode = "en";
                        Project = ProjectEnum.wikipedia;
                        SetToEnglish();
                    }
                }
                catch (ReadApiDeniedException)
                {
                    TryLoadingAgainAfterLogin = true;
                    ReloadProjectSettings = new ProjectHoldArea
                                                {
                                                    projectName = projectName,
                                                    customProject = customProject,
                                                    langCode = langCode,
                                                    protocol = Protocol
                                                };
                    return;
                }
            }

            RegenerateRegexes();

            RetfPath = Namespaces[Namespace.Project] + "AutoWikiBrowser/Typos";

            if(typoReloadNeeded && MainForm != null)
                MainForm.LoadTypos(true);

            foreach (string s in Namespaces.Values)
            {
                System.Diagnostics.Trace.Assert(s.EndsWith(":"), "Internal error: namespace does not end with ':'.",
                                                "Please contact a developer.");
            }
            System.Diagnostics.Trace.Assert(!Namespaces.ContainsKey(0), "Internal error: key exists for namespace 0.",
                                            "Please contact a developer.");

            if (projectName.Equals(ProjectEnum.wiktionary))
                CapitalizeFirstLetter = false;
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
            Namespaces[118] = "Draft:";
            Namespaces[119] = "Draft talk:";
            Namespaces[446] = "Education Program:";
            Namespaces[447] = "Education Program talk:";
            Namespaces[710] = "TimedText:";
            Namespaces[711] = "TimedText talk:";
            Namespaces[828] = "Module:";
            Namespaces[829] = "Module talk:";

            mSummaryTag = "using";
            WPAWB = "[[Project:AWB|AWB]]";

            NamespaceAliases = CanonicalNamespaceAliases;

            MonthNames = ENLangMonthNames;
            SectStub = @"\{\{[Ss]ect";
            SectStubRegex = new Regex(SectStub, RegexOptions.Compiled);
            Stub = "[^{}|]*?[Ss]tub";

            LangCode = "en";

            RTL = false;
            CapitalizeFirstLetter = true;
        }

		/// <summary>
		/// Sets the language specific values: summary tag, stub regex, AWB project link
		/// </summary>
		/// <param name='langCode'>
		/// Language code of wiki.
		/// </param>
		/// <param name='projectName'>
		/// Project name of wiki.
		/// </param>
        private static void SetLanguageSpecificValues(string langCode, ProjectEnum projectName)
        {
            switch (langCode)
            {
                case "en":
                    if (projectName == ProjectEnum.wikipedia)
                        SetToEnglish();
                    TypoSummaryTag = @"[[WP:AWB/T|typo(s) fixed]]: ";
                    break;

                case "ar":
                    mSummaryTag = "";
                    WPAWB = "باستخدام [[Project:أوب|أوب]]";
                    Stub = @"[^{}|]*?(?:[Ss]tub|بذرة|بذور)[^{}]*?";
                    TypoSummaryTag = "الأخطاء المصححة: ";
                    break;

                case "arz":
                    mSummaryTag = "";
                    WPAWB = "عن طريق [[Project:AWB|اوب]]";
                    Stub = @"[^{}|]*?(?:[Ss]tub|تقاوى|بذرة)[^{}]*?";
                    TypoSummaryTag = "الأخطاء المصححة: ";
                    break;
                    
                case "bg":
                    mSummaryTag = "редактирано с";
                    WPAWB = "AWB";
                    break;

                case "ca":
                    mSummaryTag = "";
                    WPAWB = "[[Viquipèdia:AutoWikiBrowser|AWB]]";
                    break;

                case "cs":
                    mSummaryTag = "za použití";
                    WPAWB = "[[Wikipedie:AutoWikiBrowser|AWB]]";
                    Stub = @"[^{}|]*?([Pp]ahýl)";
                    break;

                case "cy":
                    Stub = @"[^{}|]*?([Ss]tub|[Εe]ginyn[^{}]*?)";
                    break;
                    
                case "da":
                    mSummaryTag = "ved brug af";
                    WPAWB = "[[en:WP:AWB|AWB]]";
                    break;

                case "de":
                    mSummaryTag = "mit";
                    TypoSummaryTag = "Schreibweise: ";
                    break;

                case "el":
                    mSummaryTag = "με τη χρήση";
                    WPAWB = "[[Βικιπαίδεια:AutoWikiBrowser|AWB]]";
                    Stub = @"[^{}|]*?([Ss]tub|[Εε]πέκταση)";
                    SectStub = @"\{\{θέματος";
                    SectStubRegex = new Regex(SectStub, RegexOptions.Compiled);
                    break;

                case "eo":
                    mSummaryTag = "per";
                    WPAWB = "[[Vikipedio:AutoWikiBrowser|AWB]]";
                    TypoSummaryTag = "Skribmaniero: ";
                    break;

                case "fa":
                    mSummaryTag = "";
                    WPAWB = "با استفاده از [[Project:AutoWikiBrowser|AWB]]";
                    break;

                case "fr":
                    mSummaryTag = "avec";
                    WPAWB = "[[Wikipédia:AutoWikiBrowser|AWB]]";
                    break;

                case "he":
                    mSummaryTag = "באמצעות";
                    WPAWB = "[[ויקיפדיה:AutoWikiBrowser|AWB]]";
                    break;

                case "hi":
                    mSummaryTag = "";
                    WPAWB = "[[विकिपीडिया:ऑटोविकिब्राउज़र|AWB]] के साथ";
                    break;

                case "hu":
                    mSummaryTag = "";
                    WPAWB = "[[Wikipédia:AutoWikiBrowser|AWB]]";
                    break;

                case "hy":
                    mSummaryTag = "oգտվելով";
                    WPAWB = "[[Վիքիպեդիա:ԱվտոՎիքիԲրաուզեր|ԱՎԲ]]";
                    Stub = @"[^{}|]*?([Ss]tub|Անավարտ|Զարգացնել[^{}]*?)";
                    break;

                case "ku":
                    mSummaryTag = "";
                    WPAWB = "[[Wîkîpediya:AutoWikiBrowser|AWB]]";
                    break;

                case "ms":
                    mSummaryTag = "menggunakan";
                    break;

                case "ne":
                    mSummaryTag = "";
                    WPAWB = "स्वतःविकी ब्राउजर प्रयोग गर्दै";
                    break;
                    
                case "nl":
                    mSummaryTag = "met";
                    break;
                    
                case "pl":
                    mSummaryTag = "przy użyciu";
                    SectStub = @"\{\{[Ss]ek";
                    SectStubRegex = new Regex(SectStub, RegexOptions.Compiled);
                    break;

                case "pt":
                    mSummaryTag = "utilizando";
                    WPAWB = "[[Project:AutoWikiBrowser|AWB]]";
                    break;

                case "ru":
                    mSummaryTag = "с помощью";
                    Stub = "[^{}]*?(?:[Ss]tub|[Зз]аготовка)";
                    break;

                case "sk":
                    mSummaryTag = "";
                    WPAWB = "[[Wikipédia:AutoWikiBrowser|AWB]]";
                    break;

                case "sl":
                    mSummaryTag = "";
                    WPAWB = "[[Wikipedija:AutoWikiBrowser|AWB]]";
                    Stub = "(?:[^{}]*?[Ss]tub|[Šš]krbina[^{}]*?)";
                    break;

                case "sv":
                    mSummaryTag = "med";
                    TypoSummaryTag = "rättar stavfel: ";
                    Stub = @"(?:[^{}]*?[Ss]tub|[^{}]+?stub(?:[ \-][^{}]+)?)(?<![Ss]tubbmall|[Ss]ubstub|[Ss]tubavsnitt|[Uu]ncategorized stub)";
                    break;

                case "tr":
                    mSummaryTag = "";
                    WPAWB = "[[Vikipedi:AWB|AWB]] ile ";
                    TypoSummaryTag = "yazış şekli: ";
                    break;

                case "uk":
                    Stub = ".*?(?:[Ss]tub|[Дд]оробити)";
                    mSummaryTag = "за допомогою";
                    WPAWB = "[[Вікіпедія:AutoWikiBrowser|AWB]]";
                    break;

                case "zh":
                    mSummaryTag = "由";
                    WPAWB = "[[维基百科:自动维基浏览器|自动维基浏览器]]协助";
                    break;

                case "zh-classical":
                    mSummaryTag = "藉";
                    WPAWB = "[[维基百科:自动维基浏览器|自動維基瀏覽器]]之助";
                    break;

                case "zh-yue":
                    mSummaryTag = "由";
                    WPAWB = "[[维基百科:自动维基浏览器|自動維基瀏覽器]]協助";
                    break;

                    // case "xx:
                    // strsummarytag = " ";
                    // strWPAWB = "";
                    // break;
            }
        }

        #endregion

        #region URL Builders

        /// <summary>
        /// returns full URL to the given page, depends on project settings
        /// </summary>
        public static string NonPrettifiedURL(string title)
        {
            return URLIndex + "?title=" + Tools.WikiEncode(title);
        }

        /// <summary>
        /// returns full URL to the history page of the input title, depends on project settings
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public static string GetArticleHistoryURL(string title)
        {
            return (NonPrettifiedURL(title) + "&action=history");
        }

        /// <summary>
        /// returns full URL to the edit page of the input title, depends on project settings
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public static string GetEditURL(string title)
        {
            return (NonPrettifiedURL(title) + "&action=edit");
        }

        /// <summary>
        /// returns full URL to the user talk page of the input user, depends on project settings
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static string GetUserTalkURL(string username)
        {
            return URLIndex + "?title=User_talk:" + Tools.WikiEncode(username) + "&action=purge";
        }

        /// <summary>
        /// Returns the full URL to the input wiki page using current site settings, specifying &action=raw
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public static string GetPlainTextURL(string title)
        {
            return NonPrettifiedURL(title) + "&action=raw";
        }

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
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

