/*

Copyright (C) 2007 Martin Richards, Stephen Kennedy

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

/*
How to enable a new setting:
 * Add a public serialisable field to the relevant settings class in this code file
 * Add a parameter to the public constructor, so that the UI-value can be stored in the object when saving settings
 * Add code to the object creation line in MakePrefs() in UserSettings.cs - this is where the UI passes it's
   settings state to new SettingsClass objects prior to saving to XML
 * Add code to read the deserialised value in UserSettings.cs:LoadPrefs
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

using System.IO;

namespace WikiFunctions.AWBSettings
{
    //mother class
    [Serializable, XmlRoot("AutoWikiBrowserPreferences")]
    public class UserPrefs
    {
        // the internal constructors are used during deserialisation or when a blank object is required
        public UserPrefs()
        {
            FindAndReplace = new FaRPrefs();
            Editprefs = new EditPrefs();
            List = new ListPrefs();
            SkipOptions = new SkipPrefs();
            General = new GeneralPrefs();
            Disambiguation = new DabPrefs();
            Module = new ModulePrefs();
            Logging = new LoggingPrefs();
        }

        // the public constructors are used to create an object with settings from the UI
        public UserPrefs(FaRPrefs mFaRPrefs, EditPrefs mEditprefs, ListPrefs mList, SkipPrefs mSkipOptions,
            GeneralPrefs mGeneral, DabPrefs mDisambiguation, ModulePrefs mModule, ExternalProgramPrefs mExternalProgram, LoggingPrefs mLogging, SpecialFilterPrefs mSpecial,
            Dictionary<string, Plugin.IAWBPlugin> plugins)
        {
            LanguageCode = Variables.LangCode;
            Project = Variables.Project;
            CustomProject = Variables.CustomProject;
            PHP5 = Variables.PHP5;

            FindAndReplace = mFaRPrefs;
            Editprefs = mEditprefs;
            List = mList;
            SkipOptions = mSkipOptions;
            General = mGeneral;
            Disambiguation = mDisambiguation;
            Module = mModule;
            ExternalProgram = mExternalProgram;
            Logging = mLogging;
            Special = mSpecial;

            foreach (KeyValuePair<string, Plugin.IAWBPlugin> a in plugins)
            {
                Plugin.Add(new PluginPrefs(a.Key, a.Value.SaveSettings()));
            }
        }

        [XmlAttribute("xml:space")]
        public String SpacePreserve = "preserve";

        [XmlAttribute]
        public string Version = Tools.VersionString;
        public ProjectEnum Project = ProjectEnum.wikipedia;
        public LangCodeEnum LanguageCode = LangCodeEnum.en;
        public string CustomProject = "";
        public bool PHP5;

        public ListPrefs List;
        public FaRPrefs FindAndReplace;
        public EditPrefs Editprefs;
        public GeneralPrefs General;
        public SkipPrefs SkipOptions;
        public ModulePrefs Module;
        public ExternalProgramPrefs ExternalProgram = new ExternalProgramPrefs();
        public DabPrefs Disambiguation;
        public LoggingPrefs Logging;
        public SpecialFilterPrefs Special = new SpecialFilterPrefs();

        public List<PluginPrefs> Plugin = new List<PluginPrefs>();

        /// <summary>
        /// Loads the UserPrefs from the specified file
        /// </summary>
        /// <param name="file">File to load from</param>
        /// <returns>Loaded UserPrefs</returns>
        public static UserPrefs LoadPrefs(string file)
        {
            string settings;
            using (StreamReader f = new StreamReader(file, Encoding.UTF8))
            {
                settings = f.ReadToEnd();
            }

            //test to see if it is an old AWB file
            if (settings.Contains("<projectlang proj="))
                throw new Exception("This file uses old settings format unsupported by this version of AWB.");

            XmlSerializer xs = new XmlSerializer(typeof(UserPrefs), new [] { typeof(PrefsKeyPair) });
            return (UserPrefs)xs.Deserialize(new StringReader(settings));
        }

        /// <summary>
        /// Saves the UserPrefs to the specified file
        /// </summary>
        /// <param name="prefs">UserPrefs object to save</param>
        /// <param name="file">File to save to</param>
        public static void SavePrefs(UserPrefs prefs, string file)
        {
            try
            {
                using (FileStream fStream = new FileStream(file, FileMode.Create))
                {
                    List<Type> types = SavePluginSettings(prefs);

                    XmlSerializer xs = new XmlSerializer(typeof(UserPrefs), types.ToArray());
                    xs.Serialize(fStream, prefs);
                }
            }
            catch (Exception ex) { ErrorHandler.Handle(ex); }
        }

        /// <summary>
        /// Saves the plugin Settings
        /// </summary>
        /// <param name="prefs">UserPrefs object</param>
        /// <returns>A list of the plugin types</returns>
        public static List<Type> SavePluginSettings(UserPrefs prefs)
        {
            List<Type> types = new List<Type>();
            /* Find out what types the plugins are using for their settings so we can 
               add them to the Serializer. The plugin author must ensure s(he) is using
               serializable types.*/

            foreach (PluginPrefs pl in prefs.Plugin)
            {
                if ((pl.PluginSettings != null) && (pl.PluginSettings.Length >= 1))
                {
                    foreach (object pl2 in pl.PluginSettings)
                    {
                        types.Add(pl2.GetType());
                    }
                }
            }
            return types;
        }
    }

    //find and replace prefs
    [Serializable]
    public class FaRPrefs
    {
        internal FaRPrefs() { }

        /// <summary>
        /// Fill the object with settings from UI
        /// </summary>
        public FaRPrefs(Parse.FindandReplace findAndReplace,
            ReplaceSpecial.ReplaceSpecial replaceSpecial, SubstTemplates substTemplates)
        {
            IgnoreSomeText = findAndReplace.IgnoreLinks;
            IgnoreMoreText = findAndReplace.IgnoreMore;
            AppendSummary = findAndReplace.AppendToSummary;
            AfterOtherFixes = findAndReplace.AfterOtherFixes;
            Replacements = findAndReplace.GetList();
            AdvancedReps = replaceSpecial.GetRules();

            SubstTemplates = substTemplates.TemplateList;
            IncludeComments = substTemplates.IncludeComments;
            ExpandRecursively = substTemplates.ExpandRecursively;
            IgnoreUnformatted = substTemplates.IgnoreUnformatted;
        }

        public bool Enabled = false;
        public bool IgnoreSomeText = false;
        public bool IgnoreMoreText = false;
        public bool AppendSummary = true;
        public bool AfterOtherFixes = false;
        public List<Parse.Replacement> Replacements = new List<Parse.Replacement>();

        public List<ReplaceSpecial.IRule> AdvancedReps = new List<ReplaceSpecial.IRule>();

        public string[] SubstTemplates = new string[0];
        public bool IncludeComments = false;
        public bool ExpandRecursively = true;
        public bool IgnoreUnformatted = false;
    }

    [Serializable]
    public class ListPrefs
    {
        internal ListPrefs() { }

        /// <summary>
        /// Fill the object with settings from UI
        /// </summary>
        public ListPrefs(WikiFunctions.Controls.Lists.ListMaker listMaker, bool SaveArticleList)
        {
            ListSource = listMaker.SourceText;
            SourceIndex = listMaker.SelectedSource;
            if (SaveArticleList)
                ArticleList = listMaker.GetArticleList();
            else
                ArticleList = new List<Article>();
        }

        public string ListSource = "";
        public int SourceIndex = 0;
        public List<Article> ArticleList = new List<Article>();
    }

    //the basic settings
    [Serializable]
    public class EditPrefs
    {
        internal EditPrefs() { }

        /// <summary>
        /// Fill the object with settings from UI
        /// </summary>
        public EditPrefs(bool mGeneralFixes, bool mTagger, bool mUnicodify, int mRecategorisation,
            string mNewCategory, string mNewCategory2, int mReImage, string mImageFind, string mReplace,
            bool mSkipIfNoCatChange, bool mRemoveSortKey, bool mSkipIfNoImgChange, bool mAppendText, bool mAppend, string mText,
            int mNewlines, int mAutoDelay, bool mSuppressTag, bool mRegexTypoFix)
        {
            GeneralFixes = mGeneralFixes;
            Tagger = mTagger;
            Unicodify = mUnicodify;
            Recategorisation = mRecategorisation;
            NewCategory = mNewCategory;
            NewCategory2 = mNewCategory2;
            ReImage = mReImage;
            ImageFind = mImageFind;
            Replace = mReplace;
            SkipIfNoCatChange = mSkipIfNoCatChange;
            RemoveSortKey = mRemoveSortKey;
            SkipIfNoImgChange = mSkipIfNoImgChange;
            AppendText = mAppendText;
            Append = mAppend;
            Text = mText;
            Newlines = mNewlines;
            AutoDelay = mAutoDelay;
            SupressTag = mSuppressTag;
            RegexTypoFix = mRegexTypoFix;
        }

        public bool GeneralFixes = true;
        public bool Tagger = true;
        public bool Unicodify = true;

        public int Recategorisation = 0;
        public string NewCategory = "";
        public string NewCategory2 = "";

        public int ReImage = 0;
        public string ImageFind = "";
        public string Replace = "";

        public bool SkipIfNoCatChange = false;
        public bool RemoveSortKey = false;
        public bool SkipIfNoImgChange = false;

        public bool AppendText = false;
        public bool Append = true;
        public string Text = "";
        public int Newlines = 2;

        public int AutoDelay = 10;
        public bool SupressTag = false;
        public bool OverrideWatchlist = false;
        public bool RegexTypoFix = false;
    }

    //skip options
    [Serializable]
    public class SkipPrefs
    {
        internal SkipPrefs() { }
        public SkipPrefs(bool mSkipNonexistent, bool mSkipexistent, bool mSkipWhenNoChanges, bool mSkipWhenSpamFilterBlocked, bool mSkipInuse, bool mSkipDoes,
            bool mSkipDoesNot, string mSkipDoesText, string mSkipDoesNotText, bool mRegex, bool mCaseSensitive,
            bool mSkipNoFindAndReplace, bool mSkipNoRegexTypoFix, bool mSkipNoDisambiguation,
            bool mSkipWhitespaceOnly, bool mSkipCasingOnly, bool mSkipOnlyGeneralFixChanges, bool mSkipOnlyMinorGeneralFixChanges, bool mSkipNoLinksOnPage,
            List<int> mGeneralSkipList, bool mSkipIfRedirect, bool mSkipIfNoAlerts)
        {
            SkipNonexistent = mSkipNonexistent;
            Skipexistent = mSkipexistent;
            SkipWhenNoChanges = mSkipWhenNoChanges;
            SkipSpamFilterBlocked = mSkipWhenSpamFilterBlocked;
            SkipInuse = mSkipInuse;
            SkipDoes = mSkipDoes;
            SkipDoesNot = mSkipDoesNot;
            SkipDoesText = mSkipDoesText;
            SkipDoesNotText = mSkipDoesNotText;
            Regex = mRegex;
            CaseSensitive = mCaseSensitive;
            SkipNoFindAndReplace = mSkipNoFindAndReplace;
            SkipNoRegexTypoFix = mSkipNoRegexTypoFix;
            SkipNoDisambiguation = mSkipNoDisambiguation;
            GeneralSkipList = mGeneralSkipList;
            SkipWhenOnlyWhitespaceChanged = mSkipWhitespaceOnly;
            SkipOnlyCasingChanged = mSkipCasingOnly;
            SkipOnlyGeneralFixChanges = mSkipOnlyGeneralFixChanges;
            SkipOnlyMinorGeneralFixChanges = mSkipOnlyMinorGeneralFixChanges;
            SkipNoLinksOnPage = mSkipNoLinksOnPage;
            SkipIfRedirect = mSkipIfRedirect;
            SkipIfNoAlerts = mSkipIfNoAlerts;
        }

        public bool SkipNonexistent = true;
        public bool Skipexistent = false;
        public bool SkipWhenNoChanges = false;
        public bool SkipSpamFilterBlocked = false;
        public bool SkipInuse = false;
        public bool SkipWhenOnlyWhitespaceChanged = false;
        public bool SkipOnlyGeneralFixChanges = true;
        public bool SkipOnlyMinorGeneralFixChanges = false;
        public bool SkipOnlyCasingChanged = false;
        public bool SkipIfRedirect = false;
        public bool SkipIfNoAlerts = false;

        public bool SkipDoes = false;
        public bool SkipDoesNot = false;

        public string SkipDoesText = "";
        public string SkipDoesNotText = "";

        public bool Regex = false;
        public bool CaseSensitive = false;

        public bool SkipNoFindAndReplace = false;
        public bool SkipNoRegexTypoFix = false;
        public bool SkipNoDisambiguation = false;
        public bool SkipNoLinksOnPage = false;

        public List<int> GeneralSkipList = new List<int>();
        public string GeneralSkip = ""; //to be removed at later date
    }

    [Serializable]
    public class GeneralPrefs
    {
        internal GeneralPrefs()
        {
            AutoSaveEdit = new EditBoxAutoSavePrefs();
        }

        /// <summary>
        /// Fill the object with settings from UI
        /// </summary>
        public GeneralPrefs(bool mSaveArticleList, bool mIgnoreNoBots,
            System.Windows.Forms.ComboBox.ObjectCollection mSummaries, string mSelectedSummary,
            string[] mPasteMore, string mFindText, bool mFindRegex, bool mFindCaseSensitive, bool mWordWrap,
            bool mToolBarEnabled, bool mBypassRedirect, bool mAutoSaveSettings, bool mPreParseMode, bool mNoSectionEditSummary, bool mRestrictDefaultsortAddition, bool mNoMOSComplianceFixes, bool mSyntaxHighlightEditBox, bool mNoAutoChanges, int mOnLoadAction, bool mMinor,
            bool mWatch, bool mDoNotWatch, bool mTimerEnabled, bool mSortListAlphabetically, bool mAddIgnoredToLog, int mTextBoxSize,
            string mTextBoxFont, bool mLowThreadPriority, bool mBeep, bool mFlash, bool mMinimize,
            decimal mTimeOutLimit, bool autoSaveEditBoxEnabled, decimal autoSaveEditBoxPeriod,
            string autoSaveEditBoxFile, bool mLockSummary, bool mEditToolbarEnabled, bool mSuppressUsingAWB,
            bool mAddUsingAWBToActionSummaries, bool mfilterNonMainSpace, bool mAutoFilterDupes, bool mSortInterWikiOrder,
            bool mReplaceReferenceTags, bool mFocusAtEndOfEditBox, bool mscrollToUnbalancedBrackets)
        {
            SaveArticleList = mSaveArticleList;
            IgnoreNoBots = mIgnoreNoBots;

            foreach (object s in mSummaries)
                Summaries.Add(s.ToString());

            SelectedSummary = mSelectedSummary;
            PasteMore = mPasteMore;
            FindText = mFindText;
            FindRegex = mFindRegex;
            FindCaseSensitive = mFindCaseSensitive;
            WordWrap = mWordWrap;
            ToolBarEnabled = mToolBarEnabled;
            BypassRedirect = mBypassRedirect;
            AutoSaveSettings = mAutoSaveSettings;
            PreParseMode = mPreParseMode;
            noSectionEditSummary = mNoSectionEditSummary;
            restrictDefaultsortAddition = mRestrictDefaultsortAddition;
            noMOSComplianceFixes = mNoMOSComplianceFixes;
            syntaxHighlightEditBox = mSyntaxHighlightEditBox;
            NoAutoChanges = mNoAutoChanges;
            OnLoadAction = mOnLoadAction;
            Minor = mMinor;
            Watch = mWatch;
            DoNotWatch = mDoNotWatch;
            TimerEnabled = mTimerEnabled;
            SortListAlphabetically = mSortListAlphabetically;
            AddIgnoredToLog = mAddIgnoredToLog;
            TextBoxSize = mTextBoxSize;
            TextBoxFont = mTextBoxFont;
            LowThreadPriority = mLowThreadPriority;
            Beep = mBeep;
            Flash = mFlash;
            Minimize = mMinimize;
            TimeOutLimit = mTimeOutLimit;
            AutoSaveEdit = new EditBoxAutoSavePrefs(autoSaveEditBoxEnabled, autoSaveEditBoxPeriod,
                autoSaveEditBoxFile);
            LockSummary = mLockSummary;
            EditToolbarEnabled = mEditToolbarEnabled;
            SuppressUsingAWB = mSuppressUsingAWB;
            AddUsingAWBToActionSummaries = mAddUsingAWBToActionSummaries;
            filterNonMainSpace = mfilterNonMainSpace;
            AutoFilterDuplicates = mAutoFilterDupes;
            FocusAtEndOfEditBox = mFocusAtEndOfEditBox;
            scrollToUnbalancedBrackets = mscrollToUnbalancedBrackets;

            SortInterWikiOrder = mSortInterWikiOrder;
            ReplaceReferenceTags = mReplaceReferenceTags;
        }

        public EditBoxAutoSavePrefs AutoSaveEdit;

        public string SelectedSummary = "Clean up";
        public List<string> Summaries = new List<string>();

        public string[] PasteMore = new string[10] { "", "", "", "", "", "", "", "", "", "" };

        public string FindText = "";
        public bool FindRegex = false;
        public bool FindCaseSensitive = false;

        public bool WordWrap = true;
        public bool ToolBarEnabled = false;
        public bool BypassRedirect = true;
        public bool AutoSaveSettings = false;
        public bool noSectionEditSummary = false;
        public bool restrictDefaultsortAddition = false;
        public bool noMOSComplianceFixes = false;
        public bool syntaxHighlightEditBox = false;
        public bool PreParseMode = false;
        public bool NoAutoChanges = false;
        public int OnLoadAction = 0;
        public bool DiffInBotMode = false;
        public bool Minor = false;
        public bool Watch = false;
        public bool DoNotWatch = false;
        public bool TimerEnabled = false;
        public bool SortListAlphabetically = false;
        public bool AddIgnoredToLog = false;
        public bool EditToolbarEnabled = true;
        public bool filterNonMainSpace = false;
        public bool AutoFilterDuplicates = false;
        public bool FocusAtEndOfEditBox = false;
        public bool scrollToUnbalancedBrackets = false;

        public int TextBoxSize = 10;
        public string TextBoxFont = "Courier New";
        public bool LowThreadPriority = false;
        public bool Beep = false;
        public bool Flash = false;
        public bool Minimize = false;
        public bool LockSummary = false;
        public bool SaveArticleList = true;
        public bool SuppressUsingAWB = false;
        public bool AddUsingAWBToActionSummaries = false;
        public decimal TimeOutLimit = 30;
        public bool IgnoreNoBots = false;

        public bool SortInterWikiOrder = true;
        public bool ReplaceReferenceTags = true;
    }

    [Serializable]
    public class EditBoxAutoSavePrefs
    {
        internal EditBoxAutoSavePrefs()
        {
            SavePeriod = 30;
        }
        internal EditBoxAutoSavePrefs(bool mEnabled, decimal mPeriod, string mFile)
        {
            Enabled = mEnabled;
            SavePeriod = mPeriod;
            SaveFile = mFile;
        }

        public bool Enabled = false;
        public decimal SavePeriod;
        public string SaveFile = "";
    }

    [Serializable]
    public class LoggingPrefs
    {
        // initialised/handled in LoggingSettings.SerialisableSettings
        public bool LogVerbose;
        public bool LogWiki;
        public bool LogXHTML;
        public bool UploadYN;
        public bool UploadAddToWatchlist;
        public bool UploadOpenInBrowser;
        public bool UploadToWikiProjects;
        public bool DebugUploading;
        public int UploadMaxLines = 1000;
        public string LogFolder = "";
        public string UploadJobName = "";
        public string UploadLocation = "";
        public string LogCategoryName = "";
    }

    [Serializable]
    public class DabPrefs
    {
        public bool Enabled = false;
        public string Link = "";
        public string[] Variants = new string[0];
        public int ContextChars = 20;
    }

    [Serializable]
    public class ModulePrefs
    {
        public bool Enabled = false;
        public int Language = 0;
        public string Code = @"        public string ProcessArticle(string ArticleText, string ArticleTitle, int wikiNamespace, out string Summary, out bool Skip)
        {
            Skip = false;
            Summary = ""test"";

            ArticleText = ""test \r\n\r\n"" + ArticleText;

            return ArticleText;
        }";
    }

    [Serializable]
    public class ExternalProgramPrefs
    {
        public bool Enabled = false;
        public bool Skip = false;

        public string WorkingDir = "";
        public string Program = "";
        public string Parameters = "";

        public bool PassAsFile = true;
        public string OutputFile = "";
    }

    [Serializable]
    public class SpecialFilterPrefs
    {
        internal SpecialFilterPrefs() { } // Use default values
        public SpecialFilterPrefs(List<int> nameValues, bool RemoveDupes, bool SortListAZ,
            bool FilterTitlesThatContain, bool FilterTitlesThatDontContain, bool AreRegex,
            int OpType, List<string> Remove)
        {
            // Create object and initialise values
            namespaceValues = nameValues;
            remDupes = RemoveDupes;
            sortAZ = SortListAZ;
            filterTitlesThatContain = FilterTitlesThatContain;
            filterTitlesThatDontContain = FilterTitlesThatDontContain;
            areRegex = AreRegex;
            opType = OpType;
            remove = Remove;
        }

        public List<int> namespaceValues;

        public bool remDupes = true;
        public bool sortAZ = true;

        public bool filterTitlesThatContain = false;
        public string filterTitlesThatContainText = "";
        public bool filterTitlesThatDontContain = false;
        public string filterTitlesThatDontContainText = "";
        public bool areRegex = false;

        public int opType = -1;
        public List<string> remove = new List<string>();
    }

    [Serializable,]
    public class PluginPrefs
    {
        internal PluginPrefs() { }

        public PluginPrefs(string aName, object[] aPluginSettings)
        {
            Name = aName;
            PluginSettings = aPluginSettings;
        }

        public string Name = "";
        public object[] PluginSettings = null;
    }

    /// <summary>
    /// A generic serialisable settings object for plugins to use
    /// </summary>
    [Serializable]
    public class PrefsKeyPair
    {
        public string Name = "";
        public object Setting = null;

        internal PrefsKeyPair() { }

        public PrefsKeyPair(string aName, object aSetting)
        {
            Name = aName;
            Setting = aSetting;
        }
    }
}
