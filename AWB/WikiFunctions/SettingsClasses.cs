/*

Copyright (C) 2007 Martin Richards, Stephen Kennedy <steve@sdk-software.com>

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

namespace WikiFunctions.AWBSettings
{

    //mother class
    [Serializable, XmlRoot("AutoWikiBrowserPreferences")]
    public class UserPrefs
    {
        // the internal constructors are used during deserialisation or when a blank object is required
        internal UserPrefs()
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
            GeneralPrefs mGeneral, DabPrefs mDisambiguation, ModulePrefs mModule, LoggingPrefs mLogging,
            Dictionary<string, WikiFunctions.Plugin.IAWBPlugin> Plugins)
        {
            LanguageCode = Variables.LangCode;
            Project = Variables.Project;
            CustomProject = Variables.CustomProject;

            FindAndReplace = mFaRPrefs;
            Editprefs = mEditprefs;
            List = mList;
            SkipOptions = mSkipOptions;
            General = mGeneral;
            Disambiguation = mDisambiguation;
            Module = mModule;
            Logging = mLogging;

            foreach (KeyValuePair<string, WikiFunctions.Plugin.IAWBPlugin> a in Plugins)
            {
                PluginPrefs pp = new PluginPrefs();
                pp.Name = a.Key;
                pp.PluginSettings = a.Value.SaveSettings();

                Plugin.Add(pp);
            }
        }

        [XmlAttribute("xml:space")]
        public String SpacePreserve = "preserve";

        [XmlAttribute]
        public string Version = Tools.VersionString;
        public ProjectEnum Project = ProjectEnum.wikipedia;
        public LangCodeEnum LanguageCode = LangCodeEnum.en;
        public string CustomProject = "";

        public ListPrefs List;
        public FaRPrefs FindAndReplace;
        public EditPrefs Editprefs;
        public GeneralPrefs General;
        public SkipPrefs SkipOptions;
        public ModulePrefs Module;
        public DabPrefs Disambiguation;
        public LoggingPrefs Logging;

        public List<PluginPrefs> Plugin = new List<PluginPrefs>();
    }

    //find and replace prefs
    [Serializable]
    public class FaRPrefs
    {
        internal FaRPrefs() { }

        /// <summary>
        /// Fill the object with settings from UI
        /// </summary>
        public FaRPrefs(bool mEnabled, WikiFunctions.Parse.FindandReplace findAndReplace,
            WikiFunctions.MWB.ReplaceSpecial replaceSpecial, string[] mSubstTemplates,
            bool mIncludeComments, bool mExpandRecursively, bool mIgnoreUnformatted)
        {
            Enabled = mEnabled;
            IgnoreSomeText = findAndReplace.ignoreLinks;
            IgnoreMoreText = findAndReplace.ignoreMore;
            AppendSummary = findAndReplace.AppendToSummary;
            AfterOtherFixes = findAndReplace.AfterOtherFixes;
            Replacements = findAndReplace.GetList();
            AdvancedReps = replaceSpecial.GetRules();
            
            SubstTemplates = mSubstTemplates;
            IncludeComments = mIncludeComments;
            ExpandRecursively = mExpandRecursively;
            IgnoreUnformatted = mIgnoreUnformatted;
        }

        public bool Enabled = false;
        public bool IgnoreSomeText = false;
        public bool IgnoreMoreText = false;
        public bool AppendSummary = true;
        public bool AfterOtherFixes = false;
        public List<WikiFunctions.Parse.Replacement> Replacements = new List<WikiFunctions.Parse.Replacement>();

        public List<WikiFunctions.MWB.IRule> AdvancedReps = new List<WikiFunctions.MWB.IRule>();
        
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
            Source = listMaker.SelectedSource;
            if (SaveArticleList)
                ArticleList = listMaker.GetArticleList();
            else
                ArticleList = new List<Article>();
        }

        public string ListSource = "";
        public WikiFunctions.Lists.SourceType Source = WikiFunctions.Lists.SourceType.Category;
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
            bool mSkipIfNoCatChange, bool mSkipIfNoImgChange, bool mAppendText, bool mAppend, string mText,
            int mNewlines, int mAutoDelay, bool mQuickSave, bool mSuppressTag, bool mRegexTypoFix)
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
            SkipIfNoImgChange = mSkipIfNoImgChange;
            AppendText = mAppendText;
            Append = mAppend;
            Text = mText;
            Newlines = mNewlines;
            AutoDelay = mAutoDelay;
            QuickSave = mQuickSave;
            SuppressTag = mSuppressTag;
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
        public bool SkipIfNoImgChange = false;

        public bool AppendText = false;
        public bool Append = true;
        public string Text = "";
        public int Newlines = 2;

        public int AutoDelay = 10;
        public bool QuickSave = false;
        public bool SuppressTag = false;
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
            string mGeneralSkip)
        {
            SkipNonexistent=mSkipNonexistent;
            Skipexistent=mSkipexistent;
            SkipWhenNoChanges =mSkipWhenNoChanges;
            SkipSpamFilterBlocked = mSkipWhenSpamFilterBlocked;
            SkipInuse = mSkipInuse;
            SkipDoes=mSkipDoes;
            SkipDoesNot=mSkipDoesNot;
            SkipDoesText=mSkipDoesText;
            SkipDoesNotText=mSkipDoesNotText;
            Regex=mRegex;
            CaseSensitive=mCaseSensitive;
            SkipNoFindAndReplace=mSkipNoFindAndReplace;
            SkipNoRegexTypoFix=mSkipNoRegexTypoFix;
            SkipNoDisambiguation=mSkipNoDisambiguation;
            GeneralSkip=mGeneralSkip;
        }

        public bool SkipNonexistent = true;
        public bool Skipexistent = false;
        public bool SkipWhenNoChanges = false;
        public bool SkipSpamFilterBlocked = false;
        public bool SkipInuse = false;

        public bool SkipDoes = false;
        public bool SkipDoesNot = false;

        public string SkipDoesText = "";
        public string SkipDoesNotText = "";

        public bool Regex = false;
        public bool CaseSensitive = false;

        public bool SkipNoFindAndReplace = false;
        public bool SkipNoRegexTypoFix = false;
        public bool SkipNoDisambiguation = false;

        public string GeneralSkip = "";
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
            bool mToolBarEnabled, bool mBypassRedirect, bool mNoAutoChanges, int mOnLoadAction, bool mMinor,
            bool mWatch, bool mTimerEnabled, bool mSortInterwikiOrder, bool mAddIgnoredToLog, int mTextBoxSize,
            string mTextBoxFont, bool mLowThreadPriority, bool mBeep, bool mFlash, bool mMinimize,
            decimal mTimeOutLimit, bool autoSaveEditBoxEnabled, decimal autoSaveEditBoxPeriod,
            string autoSaveEditBoxFile, List<string> mCustomWikis, bool mLockSummary, bool mEditToolbarEnabled, bool mSupressUsingAWB, bool mfilterNonMainSpace)
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
            NoAutoChanges = mNoAutoChanges;
            OnLoadAction = mOnLoadAction;
            Minor = mMinor;
            Watch = mWatch;
            TimerEnabled = mTimerEnabled;
            SortInterwikiOrder = mSortInterwikiOrder;
            AddIgnoredToLog = mAddIgnoredToLog;
            TextBoxSize = mTextBoxSize;
            TextBoxFont = mTextBoxFont;
            LowThreadPriority = mLowThreadPriority;
            Beep = mBeep;
            Flash = mFlash;
            Minimize = mMinimize;
            TimeOutLimit = mTimeOutLimit;
            AutoSaveEdit=new EditBoxAutoSavePrefs(autoSaveEditBoxEnabled, autoSaveEditBoxPeriod,
                autoSaveEditBoxFile);
            CustomWikis = mCustomWikis;
            LockSummary = mLockSummary;
            EditToolbarEnabled = mEditToolbarEnabled;
            SupressUsingAWB = mSupressUsingAWB;
            filterNonMainSpace = mfilterNonMainSpace;
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
        public bool NoAutoChanges = false;
        public int OnLoadAction = 0;
        public bool Minor = false;
        public bool Watch = false;
        public bool TimerEnabled = false;
        public bool SortInterwikiOrder = true;
        public bool AddIgnoredToLog = false;
        public bool EditToolbarEnabled = true;
        public bool filterNonMainSpace = false;

        public int TextBoxSize = 10;
        public string TextBoxFont = "Courier New";
        public bool LowThreadPriority = false;
        public bool Beep = false;
        public bool Flash = false;
        public bool Minimize = false;
        public bool LockSummary = false;
        public bool SaveArticleList = true;
        public bool SupressUsingAWB = false;
        public decimal TimeOutLimit = 30;
        public bool IgnoreNoBots = false;

        public List<string> CustomWikis = new List<string>();
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
        public string UploadJobName="";
        public string UploadLocation="";
        public string LogCategoryName="";
    }

    [Serializable]
    public class DabPrefs
    {
        internal DabPrefs() { }
        public DabPrefs(bool mEnabled, string mLink, string[] mVariants, int mContextChars)
        {
            Enabled = mEnabled;
            Link = mLink;
            Variants = mVariants;
            ContextChars = mContextChars;
        }

        public bool Enabled = false;
        public string Link = "";
        public string[] Variants = new string[0];
        public int ContextChars = 20;
    }

    [Serializable]
    public class ModulePrefs
    {
        internal ModulePrefs() { }
        public ModulePrefs(bool mEnabled, int mLanguage, string mCode)
        {
            Enabled = mEnabled;
            Language = mLanguage;
            Code = mCode;
        }

        public bool Enabled = false;
        public int Language = 0;
        public string Code = "";
    }

    [Serializable]
    public class PluginPrefs
    {
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
    }
}
