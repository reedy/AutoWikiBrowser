/*

Copyright (C) 2007 Martin Richards

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
using System.Xml.Serialization;

namespace WikiFunctions.AWBSettings
{

    //mother class
    [Serializable, XmlRoot("AutoWikiBrowserPreferences")]
    public class UserPrefs
    {
        public UserPrefs() { }

        [XmlAttribute("xml:space")]
        public String SpacePreserve = "preserve";

        [XmlAttribute]
        public string Version = Tools.VersionString;
        public ProjectEnum Project = ProjectEnum.wikipedia;
        public LangCodeEnum LanguageCode = LangCodeEnum.en;
        public string CustomProject = "";

        public ListPrefs List = new ListPrefs();
        public FaRPrefs FindAndReplace = new FaRPrefs();
        public EditPrefs Editprefs = new EditPrefs();
        public GeneralPrefs General = new GeneralPrefs();
        public SkipPrefs SkipOptions = new SkipPrefs();
        public ModulePrefs Module = new ModulePrefs();
        public DabPrefs Disambiguation = new DabPrefs();

        public LoggingPrefs Logging = new LoggingPrefs();

        public List<PluginPrefs> Plugin = new List<PluginPrefs>();

        /// <summary>
        /// Fill the object with settings from UI
        /// </summary>
        public void Make()
        {
            LanguageCode = Variables.LangCode;
            Project = Variables.Project;
            CustomProject = Variables.CustomProject;
        }
    }

    //find and replace prefs
    [Serializable]
    public class FaRPrefs
    {
        public bool Enabled = false;
        public bool IgnoreSomeText = false;
        public bool AppendSummary = true;
        public bool AfterOtherFixes = false;
        public List<WikiFunctions.Parse.Replacement> Replacements = new List<WikiFunctions.Parse.Replacement>();

        public List<WikiFunctions.MWB.IRule> AdvancedReps = new List<WikiFunctions.MWB.IRule>();
        public string[] SubstTemplates = new string[0];

        /// <summary>
        /// Fill the object with settings from UI
        /// </summary>
        public void Make(bool mEnabled, WikiFunctions.Parse.FindandReplace findAndReplace,
            WikiFunctions.MWB.ReplaceSpecial replaceSpecial, string[] mSubstTemplates)
        {
            Enabled = mEnabled;
            IgnoreSomeText = findAndReplace.ignoreLinks;
            AppendSummary = findAndReplace.AppendToSummary;
            AfterOtherFixes = findAndReplace.AfterOtherFixes;
            Replacements = findAndReplace.GetList();
            AdvancedReps = replaceSpecial.GetRules();
            SubstTemplates = mSubstTemplates;
        }
    }

    [Serializable]
    public class ListPrefs
    {
        public string ListSource = "";
        public WikiFunctions.Lists.SourceType Source = WikiFunctions.Lists.SourceType.Category;
        public List<Article> ArticleList = new List<Article>();

        /// <summary>
        /// Fill the object with settings from UI
        /// </summary>
        public void Make(WikiFunctions.Controls.Lists.ListMaker listMaker, bool SaveArticleList)
        {
            ListSource = listMaker.SourceText;
            Source = listMaker.SelectedSource;
            if (SaveArticleList)
                ArticleList = listMaker.GetArticleList();
            else
                ArticleList = new List<Article>();
        }
    }

    //the basic settings
    [Serializable]
    public class EditPrefs
    {
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

        public int AutoDelay = 10;
        public bool QuickSave = false;
        public bool SuppressTag = false;
        public bool OverrideWatchlist = false;
        public bool RegexTypoFix = false;

        /// <summary>
        /// Fill the object with settings from UI
        /// </summary>
        public void Make(bool mGeneralFixes, bool mTagger, bool mUnicodify, int mRecategorisation,
            string mNewCategory, string mNewCategory2, int mReImage, string mImageFind, string mReplace,
            bool mSkipIfNoCatChange, bool mSkipIfNoImgChange, bool mAppendText, bool mAppend, string mText,
            int mAutoDelay, bool mQuickSave, bool mSuppressTag, bool mOverrideWatchlist, bool mRegexTypoFix)
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
            AutoDelay = mAutoDelay;
            QuickSave = mQuickSave;
            SuppressTag = mSuppressTag;
            OverrideWatchlist = mOverrideWatchlist;
            RegexTypoFix = mRegexTypoFix;
        }
    }

    //skip options
    [Serializable]
    public class SkipPrefs
    {
        public bool SkipNonexistent = true;
        public bool Skipexistent = false;
        public bool SkipWhenNoChanges = false;

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
        public EditBoxAutoSavePrefs AutoSaveEdit = new EditBoxAutoSavePrefs();

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

        public int TextBoxSize = 10;
        public string TextBoxFont = "Courier New";
        public bool LowThreadPriority = false;
        public bool FlashAndBeep = true;
        public bool Beep = false;
        public bool Flash = false;
        public bool Minimize = false;
        public bool LockSummary = false;
        public bool SaveArticleList = false;
        public decimal TimeOutLimit = 30;
        public bool IgnoreNoBots = false;

        public List<string> CustomWikis = new List<string>();

        /// <summary>
        /// Fill the object with settings from UI
        /// </summary>
        public void Make(bool mSaveArticleList, bool mIgnoreNoBots)
        {
            SaveArticleList = mSaveArticleList;
            IgnoreNoBots = mIgnoreNoBots;
        }
    }

    [Serializable]
    public class EditBoxAutoSavePrefs
    {
        public bool Enabled = false;
        public decimal SavePeriod = 30;
        public string SaveFile = "";
    }

    [Serializable]
    public class LoggingPrefs
    {
        public string LogFolder;
        public bool LogVerbose;
        public bool LogWiki;
        public bool LogXHTML;
        public bool UploadYN;
        public bool UploadAddToWatchlist;
        public string UploadJobName="";
        public string UploadLocation="";
        public string LogCategoryName="";
        public int UploadMaxLines;
        public bool UploadOpenInBrowser;
        public bool UploadToWikiProjects;
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
