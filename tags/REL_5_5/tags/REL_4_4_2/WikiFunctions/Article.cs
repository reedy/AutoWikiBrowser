/*

Copyright (C) 2007 Martin Richards
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
using System.Xml.Serialization;
using WikiFunctions.Logging;
using System.Text.RegularExpressions;
using WikiFunctions.Plugin;
using WikiFunctions.Options;
using WikiFunctions.Parse;

namespace WikiFunctions
{
    public delegate void ArticleRedirected(string oldTitle, string newTitle);

    /// <summary>
    /// A class which represents a wiki article
    /// </summary>
    public class Article : ProcessArticleEventArgs, IArticleSimple, IComparable<Article>
    {
        protected int mNameSpaceKey;
        protected string mName = "";
        protected string mEditSummary = "";
        protected string mSavedSummary = "";
        protected AWBLogListener mAWBLogListener;
        protected string mArticleText = "";
        protected string mOriginalArticleText = "";
        protected string mPluginEditSummary = "";
        protected bool mPluginSkip;

        public virtual IAWBTraceListener Trace
        { get { return mAWBLogListener; } }

        #region Constructors
        public Article()
        {
            EditSummary = "";
        }

        public Article(string mName)
            : this(mName, Tools.CalculateNS(mName))
        { }

        public Article(string mName, int mNameSpaceKey)
            : this()
        {
            if (mName.Contains("#"))
                this.mName = mName.Substring(0, mName.IndexOf('#'));
            else
                this.mName = mName;

            this.mNameSpaceKey = mNameSpaceKey;
        }

        public virtual AWBLogListener InitialiseLogListener()
        {
            InitLog();
            return mAWBLogListener;
        }

        public AWBLogListener InitialiseLogListener(string name, TraceManager TraceManager)
        {
            // Initialise a Log Listener and add it to a TraceManager collection
            InitLog();
            TraceManager.AddListener(name, mAWBLogListener);
            return mAWBLogListener;
        }

        private void InitLog()
        { mAWBLogListener = new AWBLogListener(mName); }
        #endregion

        #region Serialisable properties
        /// <summary>
        /// The full name of the article
        /// </summary>
        public string Name
        { get { return mName; } set { mName = value; } }

        [XmlIgnore]
        public string NamespacelessName
        {
            get
            {
                if (mNameSpaceKey == 0) return mName;

                int pos = mName.IndexOf(':');
                if (pos < 0) return mName;
                return mName.Substring(pos + 1).Trim();
            }
        }

        /// <summary>
        /// The namespace of the article
        /// </summary>
        [XmlAttribute]
        public int NameSpaceKey
        { get { return mNameSpaceKey; } set { mNameSpaceKey = value; } }
        #endregion

        #region Non-serialisable properties
        // Read-write properties should be marked with the [XmlIgnore] attribute

        /// <summary>
        /// AWBLogListener object representing a log entry for the underlying article
        /// </summary>
        [XmlIgnore]
        public AWBLogListener LogListener
        { get { return mAWBLogListener; } } //set { mAWBLogListener = value; } }

        /// <summary>
        /// The name of the article, encoded ready for use in a URL
        /// </summary>
        [XmlIgnore]
        public string URLEncodedName
        { get { return Tools.WikiEncode(mName); } }

        /// <summary>
        /// The text of the article. This is deliberately readonly; set using methods
        /// </summary>
        [XmlIgnore]
        public string ArticleText
        { get { return mArticleText.Trim(); } }

        /// <summary>
        /// Article text before this program manipulated it
        /// </summary>
        [XmlIgnore]
        public string OriginalArticleText
        { get { return mOriginalArticleText.Trim(); } set { mOriginalArticleText = value; mArticleText = value; } }

        /// <summary>
        /// Edit summary proposed for article
        /// </summary>
        [XmlIgnore]
        public string EditSummary
        { get { return mEditSummary; } set { mEditSummary = value; } }

        /// <summary>
        /// Returns true if the article is a stub (a very short article or an article tagged with a "stub template")
        /// </summary>
        [XmlIgnore]
        public bool IsStub { get { return Parsers.IsStub(mArticleText); } }

        /// <summary>
        /// Returns true if the article contains a stub template
        /// </summary>
        [XmlIgnore]
        public bool HasStubTemplate
        { get { return Parsers.HasStubTemplate(mArticleText); } }

        /// <summary>
        /// Returns true if the article contains an infobox
        /// </summary>
        [XmlIgnore]
        public bool HasInfoBox
        { get { return Parsers.HasInfobox(mArticleText); } }

        /// <summary>
        /// Returns true if the article contains a template showing it as "in use"
        /// </summary>
        [XmlIgnore]
        public bool IsInUse
        { get { return Parsers.IsInUse(mArticleText); } }

        /// <summary>
        /// Returns true if the article should be skipped; check after each call to a worker member. See AWB main.cs.
        /// </summary>
        [XmlIgnore]
        public bool SkipArticle
        { get { return mAWBLogListener.Skipped; } private set { mAWBLogListener.Skipped = value; } }

        [XmlIgnore]
        public bool CanDoGeneralFixes
        { get { return (NameSpaceKey == 0 || NameSpaceKey == 14 || Name.Contains("Sandbox")) || Name.Contains("/doc"); } }
        #endregion

        #region AWB worker subroutines
        /// <summary>
        /// AWB skips the article; passed through to the underlying AWBLogListener object
        /// </summary>
        /// <param name="reason">The reason for skipping</param>
        public void AWBSkip(string reason)
        { Trace.AWBSkipped(reason); }

        /// <summary>
        /// Returns whether the only change between the current article text and the original article text is whitespace changes
        /// </summary>
        public bool OnlyWhiteSpaceChanged
        {
            get { return (string.Compare(Regex.Replace(mOriginalArticleText, @"\s+", ""), Regex.Replace(mArticleText, @"\s+", "")) == 0); }
        }

        /// <summary>
        /// Does a case-insensitive comparison of the text, returning true if the same
        /// </summary>
        public bool OnlyCasingChanged
        {
            get { return (string.Compare(mOriginalArticleText, mArticleText, true) == 0); }
        }

        /// <summary>
        /// Returns whether the only change between the current article text and the original article text is whitespace and casing changes
        /// </summary>
        public bool OnlyWhiteSpaceAndCasingChanged
        {
            get { return (string.Compare(Regex.Replace(mOriginalArticleText, @"\s+", ""), Regex.Replace(mArticleText, @"\s+", ""), true) == 0); }
        }

        /// <summary>
        /// Returns whether the only change between the current article text and the original article text was by the general fixes
        /// </summary>
        public bool OnlyGeneralFixesChanged
        {
            get { return (generalFixesCausedChange && (ArticleText == afterGeneralFixesArticleText)); }
        }

        /// <summary>
        /// Returns whether the current article text is the same as the original article text
        /// </summary>
        public bool NoArticleTextChanged
        {
            get { return (string.Compare(mOriginalArticleText, mArticleText) == 0); }
        }

        /// <summary>
        /// Send the article to a plugin for processing
        /// </summary>
        /// <param name="plugin">The plugin</param>
        /// <param name="sender">The AWB instance</param>
        public void SendPageToPlugin(IAWBPlugin plugin, IAutoWikiBrowser sender)
        {
            string strTemp = plugin.ProcessArticle(sender, this);

            if (mPluginSkip)
            {
                if (!SkipArticle)
                    /* plugin has told us to skip but didn't log any info about reason
                    Calling Trace.SkippedArticle() should also result in SkipArticle becoming True
                    and our caller - MainForm.ProcessPage() - can check this value */
                    Trace.SkippedArticle(plugin.Name, "Skipped by plugin");
            }
            else
            {
                mAWBLogListener.Skipped = false;  // a bit of a hack, if plugin says not to skip I'm resetting the LogListener.Skipped value to False
                PluginChangeArticleText(strTemp);
                AppendPluginEditSummary();
            }
        }

        /// <summary>
        /// Convert HTML characters in the article to Unicode
        /// </summary>
        /// <param name="SkipIfNoChange">True if the article should be skipped if no changes are made</param>
        /// <param name="parsers">An initialised Parsers object</param>
        public void Unicodify(bool SkipIfNoChange, Parsers parsers)
        {
            bool noChange;
            string strTemp = parsers.Unicodify(mArticleText, out noChange);

            if (SkipIfNoChange && noChange)
                Trace.AWBSkipped("No Unicodification");
            else if (!noChange)
                AWBChangeArticleText("Page Unicodified", strTemp, false);
        }

        /// <summary>
        /// Remove, replace or comment out a specified image
        /// </summary>
        /// <param name="option">The action to take</param>
        /// <param name="parsers">An initialised Parsers object</param>
        /// <param name="ImageReplaceText">The text (image name) to look for</param>
        /// <param name="ImageWithText">Replacement text (if applicable)</param>
        /// <param name="SkipIfNoChange">True if the article should be skipped if no changes are made</param>
        public void UpdateImages(ImageReplaceOptions option, Parsers parsers,
            string ImageReplaceText, string ImageWithText, bool SkipIfNoChange)
        {
            bool noChange = true; string strTemp = "";

            ImageReplaceText = ImageReplaceText.Trim();
            ImageWithText = ImageWithText.Trim();

            if (ImageReplaceText.Length > 0)
                switch (option)
                {
                    case ImageReplaceOptions.NoAction:
                        return;

                    case ImageReplaceOptions.Replace:
                        if (ImageWithText.Length > 0) strTemp = parsers.ReplaceImage(ImageReplaceText, ImageWithText, mArticleText, out noChange);
                        break;

                    case ImageReplaceOptions.Remove:
                        strTemp = parsers.RemoveImage(ImageReplaceText, mArticleText, false, ImageWithText, out noChange);
                        break;

                    case ImageReplaceOptions.Comment:
                        strTemp = parsers.RemoveImage(ImageReplaceText, mArticleText, true, ImageWithText, out noChange);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException("option");
                }

            if (noChange && SkipIfNoChange)
                Trace.AWBSkipped("No Image Changed");
            else if (!noChange)
                AWBChangeArticleText("Image replacement applied", strTemp, false);
        }

        /// <summary>
        /// Add, remove or replace a specified category
        /// </summary>
        /// <param name="option">The action to take</param>
        /// <param name="parsers">An initialised Parsers object</param>
        /// <param name="SkipIfNoChange">True if the article should be skipped if no changes are made</param>
        /// <param name="CategoryText">The category to add or remove; or, when replacing, the name of the old category</param>
        /// <param name="CategoryText2">The name of the replacement category (recat mode only)</param>
        public void Categorisation(CategorisationOptions option, Parsers parsers,
            bool SkipIfNoChange, string CategoryText, string CategoryText2)
        {
            bool noChange = false; 
            string strTemp, action = "";

            switch (option)
            {
                case CategorisationOptions.NoAction:
                    return;

                case CategorisationOptions.AddCat:
                    if (CategoryText.Length < 1) return;
                    strTemp = Parsers.AddCategory(CategoryText, mArticleText, mName);
                    action = "Added " + CategoryText;
                    break;

                case CategorisationOptions.ReCat:
                    if (CategoryText.Length < 1 || CategoryText2.Length < 1) return;
                    strTemp = parsers.ReCategoriser(CategoryText, CategoryText2, mArticleText, out noChange);
                    break;

                case CategorisationOptions.RemoveCat:
                    if (CategoryText.Length < 1) return;
                    strTemp = parsers.RemoveCategory(CategoryText, mArticleText, out noChange);
                    action = "Removed " + CategoryText;
                    break;

                default:
                    throw new ArgumentOutOfRangeException("option");
            }

            if (noChange && SkipIfNoChange)
                Trace.AWBSkipped("No Category Changed");
            else if (!noChange)
                AWBChangeArticleText(action, strTemp, false);
        }

        /// <summary>
        /// Add or remove a specified category
        /// </summary>
        /// <param name="option">The action to take</param>
        /// <param name="parsers">An initialised Parsers object</param>
        /// <param name="SkipIfNoChange">True if the article should be skipped if no changes are made</param>
        /// <param name="NewCategoryText">The category to add or remove</param>
        public void Categorisation(CategorisationOptions option, Parsers parsers,
            bool SkipIfNoChange, string NewCategoryText)
        {
            if (option == CategorisationOptions.ReCat)
                throw new ArgumentException("This overload has no CategoryText2 argument");
            Categorisation(option, parsers, SkipIfNoChange, NewCategoryText, "");
        }

        /// <summary>
        /// Process a "find and replace"
        /// </summary>
        /// <param name="findAndReplace">A FindandReplace object</param>
        /// <param name="substTemplates">A SubstTemplates object</param>
        /// <param name="replaceSpecial">An MWB ReplaceSpecial object</param>
        /// <param name="SkipIfNoChange">True if the article should be skipped if no changes are made</param>
        public void PerformFindAndReplace(FindandReplace findAndReplace, SubstTemplates substTemplates,
            MWB.ReplaceSpecial replaceSpecial, bool SkipIfNoChange)
        {
            string strTemp = mArticleText.Replace("\r\n", "\n"),
                testText = strTemp, tmpEditSummary = "";

            strTemp = findAndReplace.MultipleFindAndReplace(strTemp, mName, ref tmpEditSummary);
            strTemp = replaceSpecial.ApplyRules(strTemp, mName);
            strTemp = substTemplates.SubstituteTemplates(strTemp, mName); // TODO: Possible bug, this was "articleTitle" not "Name"

            if (SkipIfNoChange && (testText == strTemp)) // NoChange
                Trace.AWBSkipped("No Find And Replace Changes");
            else
            {
                AWBChangeArticleText("Find and replace applied" + tmpEditSummary,
                    strTemp.Replace("\n", "\r\n"), false);
                EditSummary += tmpEditSummary;
            }
        }

        /// <summary>
        /// Fix spelling mistakes
        /// </summary>
        /// <param name="RegexTypos">A RegExTypoFix object</param>
        /// <param name="SkipIfNoChange">True if the article should be skipped if no changes are made</param>
        public void PerformTypoFixes(RegExTypoFix RegexTypos, bool SkipIfNoChange)
        {
            bool noChange;
            string strTemp = RegexTypos.PerformTypoFixes(mArticleText, out noChange, out mPluginEditSummary);

            if (noChange && SkipIfNoChange)
                Trace.AWBSkipped("No typo fixes");
            else if (!noChange)
            {
                AWBChangeArticleText(mPluginEditSummary, strTemp, false);
                AppendPluginEditSummary();
            }
        }

        /// <summary>
        /// "Auto tag" (Adds/removes wikify or stub tags if necessary)
        /// </summary>
        /// <param name="parsers">An initialised Parsers object</param>
        /// <param name="SkipIfNoChange">True if the article should be skipped if no changes are made</param>
        public void AutoTag(Parsers parsers, bool SkipIfNoChange)
        {
            bool noChange; string tmpEditSummary = "";
            string strTemp = parsers.Tagger(mArticleText, mName, out noChange, ref tmpEditSummary);

            if (SkipIfNoChange && noChange)
                Trace.AWBSkipped("No Tag changed");
            else if (!noChange)
            {
                AWBChangeArticleText("Auto tagger changes applied" + tmpEditSummary, strTemp, false);
                EditSummary += tmpEditSummary;
            }
        }

        /// <summary>
        /// Fix header errors
        /// </summary>
        /// <param name="parsers">An initialised Parsers object</param>
        /// <param name="LangCode">The wiki's language code</param>
        /// <param name="SkipIfNoChange">True if the article should be skipped if no changes are made</param>
        public void FixHeaderErrors(Parsers parsers, LangCodeEnum LangCode, bool SkipIfNoChange)
        {
            if (LangCode == LangCodeEnum.en)
            {
                bool noChange;

                string strTemp = parsers.Conversions(mArticleText);

                strTemp = parsers.FixDates(strTemp);
                strTemp = parsers.FixLivingThingsRelatedDates(strTemp);
                strTemp = parsers.LivingPeople(strTemp, out noChange);
                strTemp = parsers.FixHeadings(strTemp, mName, out noChange);

                if (mArticleText == strTemp)
                {
                    if (SkipIfNoChange)
                        Trace.AWBSkipped("No header errors");
                }
                else
                {
                    if (!noChange)
                        AWBChangeArticleText("Fixed header errors", strTemp, true);
                    else
                    {
                        AWBChangeArticleText("Fixed minor formatting issues", strTemp, true);
                        if (SkipIfNoChange) Trace.AWBSkipped("No header errors");
                    }

                }
            }
        }

        /// <summary>
        /// Sets Default Sort on Article if Necessary
        /// </summary>
        /// <param name="parsers">An initialised Parsers object</param>
        /// <param name="LangCode">The wiki's language code</param>
        /// <param name="SkipIfNoChange">True if the article should be skipped if no changes are made</param>
        public void SetDefaultSort(Parsers parsers, LangCodeEnum LangCode, bool SkipIfNoChange)
        {
            if (LangCode == LangCodeEnum.en)
            {
                bool noChange;
                string strTemp = parsers.ChangeToDefaultSort(mArticleText, mName, out noChange);

                if (SkipIfNoChange && noChange)
                    Trace.AWBSkipped("No DefaultSort Added");
                else if (!noChange)
                    AWBChangeArticleText("DefaultSort Added", strTemp, true);
            }
        }

        /// <summary>
        /// Fix link syntax
        /// </summary>
        /// <param name="parsers">An initialised Parsers object</param>
        /// <param name="SkipIfNoChange">True if the article should be skipped if no changes are made</param>
        public void FixLinks(Parsers parsers, bool SkipIfNoChange)
        {
            bool noChange;
            string strTemp = Parsers.FixLinks(mArticleText, out noChange);
            if (noChange && SkipIfNoChange)
                Trace.AWBSkipped("No bad links");
            else if (!noChange)
                AWBChangeArticleText("Fixed links", strTemp, false);
        }

        /// <summary>
        /// Add bulletpoints to external links, if necessary
        /// </summary>
        /// <param name="parsers">An initialised Parsers object</param>
        /// <param name="SkipIfNoChange">True if the article should be skipped if no changes are made</param>
        public void BulletExternalLinks(Parsers parsers, bool SkipIfNoChange)
        {
            bool noChange;
            string strTemp = parsers.BulletExternalLinks(mArticleText, out noChange);
            if (SkipIfNoChange && noChange)
                Trace.AWBSkipped("No missing bulleted links");
            else if (!noChange)
                AWBChangeArticleText("Bulleted external links", strTemp, false);
        }

        /// <summary>
        /// '''Emboldens''' the first occurence of the article title, if not already bold
        /// </summary>
        /// <param name="parsers">An initialised Parsers object</param>
        /// <param name="SkipIfNoChange">True if the article should be skipped if no changes are made</param>
        public void EmboldenTitles(Parsers parsers, bool SkipIfNoChange)
        {
            bool noChange;
            string strTemp = parsers.BoldTitle(mArticleText, mName, out noChange);
            if (SkipIfNoChange && noChange)
                Trace.AWBSkipped("No Titles to embolden");
            else if (!noChange)
                AWBChangeArticleText("Emboldened titles", strTemp, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Module"></param>
        public void SendPageToCustomModule(IModule Module)
        { // TODO: Check this Skips properly if module tells us to. If not, we'll have to set the Skip property directly
            ProcessArticleEventArgs processArticleEventArgs = this;
            string strEditSummary;
            bool skipArticle;

            string strTemp = Module.ProcessArticle(processArticleEventArgs.ArticleText,
                processArticleEventArgs.ArticleTitle, NameSpaceKey, out strEditSummary, out skipArticle);

            if (!skipArticle)
            {
                processArticleEventArgs.EditSummary = strEditSummary;
                processArticleEventArgs.Skip = false;
                AWBChangeArticleText("Custom module", strTemp, true);
                AppendPluginEditSummary();
            }
            else Trace.AWBSkipped("Skipped by custom module");
        }
        #endregion

        #region AWB worker functions
        /// <summary>
        /// Returns true if the article should be skipped based on the text it does or doesn't contain
        /// </summary>
        /// <param name="FindText">The text to find</param>
        /// <param name="regEx">True if FindText contains a regular expression</param>
        /// <param name="caseSensitive">True if the search should be case sensitive</param>
        /// <param name="DoesContain">True if the article should be skipped if it contains the text, false if it should be skipped if it does *not* contain the text</param>
        /// <returns>A boolean value indicating whether or not the article should be skipped</returns>
        public bool SkipIfContains(string FindText, bool regEx, bool caseSensitive, bool DoesContain)
        {
            if (FindText.Length > 0)
            {
                RegexOptions regexOptions = (caseSensitive) ? RegexOptions.None : RegexOptions.IgnoreCase;

                FindText = Tools.ApplyKeyWords(Name, FindText);

                if (!regEx)
                    FindText = Regex.Escape(FindText);

                if (Regex.IsMatch(OriginalArticleText, FindText, regexOptions))
                    return DoesContain;
                
                return !DoesContain;
            }
            
            return false;
        }

        /// <summary>
        /// Disambiguate
        /// </summary>
        /// <returns>True if OK to proceed, false to abort</returns>
        public bool Disambiguate(string dabLinkText, string[] dabVariantsLines, bool botMode, int Context,
            bool skipIfNoChange)
        {
            bool noChange;
            Disambiguation.DabForm df = new Disambiguation.DabForm();
            string strTemp = df.Disambiguate(mArticleText, mName, dabLinkText,
                dabVariantsLines, Context, botMode, out noChange);

            if (df.Abort) return false;

            if (noChange && skipIfNoChange)
                Trace.AWBSkipped("No disambiguation");
            else if (!noChange)
                AWBChangeArticleText("Disambiguated " + dabLinkText, strTemp, false);

            return true;
        }
        #endregion

        #region Article text modifiers
        /// <summary>
        /// Modify the article text, and log the reason
        /// </summary>
        /// <param name="changedBy">Which application or module changed the text</param>
        /// <param name="reason">Why the text was changed</param>
        /// <param name="newText">The new text</param>
        /// <param name="checkIfChanged">Check if the new text does differ from the existing text before logging it; exits silently if this param is true and there was no change</param>
        public void ChangeArticleText(string changedBy, string reason, string newText, bool checkIfChanged)
        {
            if (checkIfChanged && newText == mArticleText) return;

            mArticleText = newText;
            mAWBLogListener.WriteLine(reason, changedBy);
        }

        /// <summary>
        /// A subroutine allowing AWB to modify the article text. Passes through to ChangeArticleText()
        /// </summary>
        public void AWBChangeArticleText(string reason, string newText, bool checkIfChanged)
        {
            ChangeArticleText("AWB", reason, newText, checkIfChanged);
        }

        /// <summary>
        /// Allows plugins to modify the article text. Plugins should set their own log entry using the object passed in ProcessArticle()
        /// </summary>
        /// <param name="newText"></param>
        public void PluginChangeArticleText(string newText)
        {
            mArticleText = newText;
        }
        #endregion

        #region Misc subroutines
        public void AppendPluginEditSummary()
        {
            if (mPluginEditSummary.Length > 0)
            {
                EditSummary += mPluginEditSummary.Trim();
                mPluginEditSummary = "";
            }
        }

        public void HideText(HideText RemoveText)
        { mArticleText = RemoveText.Hide(mArticleText); }

        public void UnHideText(HideText RemoveText)
        { mArticleText = RemoveText.AddBack(mArticleText); }

        public void HideMoreText(HideText RemoveText)
        { mArticleText = RemoveText.HideMore(mArticleText); }

        public void UnHideMoreText(HideText RemoveText)
        { mArticleText = RemoveText.AddBackMore(mArticleText); }

        /// <summary>
        /// returns 
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public static string CanonicalizeTitle(string title)
        {
            // visible parts of links may contain crap we shouldn't modify, such as
            // refs and external links
            if (title.Contains("[") || title.Contains("{")) return title;

            string s = Parsers.CanonicalizeTitle(title);
            if (Variables.UnderscoredTitles.Contains(Tools.TurnFirstToUpper(s)))
            {
                return System.Web.HttpUtility.UrlDecode(title.Replace("+", "%2B"))
                    .Trim(new char[] { '_' });
            }
            return s;
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return mName;
        }

        public override int GetHashCode()
        {
            return mName.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            Article a = obj as Article;
            if (a == null)
            {
                if (obj is string)
                    return mName == obj as string;

                return false;
            }
            return (mName == a.mName);
        }

        public int CompareTo(Article other)
        {
            return string.Compare(mName, other.mName, false, System.Globalization.CultureInfo.InvariantCulture);
        }

        #endregion

        #region Interfaces

        //IMyTraceListener ProcessArticleEventArgs.AWBLogItem
        //{ get { return mAWBLogListener; } }

        string ProcessArticleEventArgs.ArticleTitle
        { get { return mName; } }

        string ProcessArticleEventArgs.EditSummary // this is temp edit summary field, sent from plugin
        { get { return mPluginEditSummary; } set { mPluginEditSummary = value.Trim(); } }

        bool ProcessArticleEventArgs.Skip
        { get { return mPluginSkip; } set { mPluginSkip = value; } }

        // and NamespaceKey

        Article IArticleSimple.Article { get { return this; } }
        #endregion

        public static IArticleSimple GetReadOnlyArticle(string Title)
        {
            // TODO: See Parsers.HasInfobox
            return null;
        }

        #region General fixes
        bool generalFixesCausedChange, textAlreadyChanged;
        string afterGeneralFixesArticleText;

        /// <summary>
        /// Performs numerous minor improvements to the page text
        /// </summary>
        /// <param name="parsers">A parser object</param>
        /// <param name="removeText"></param>
        /// <param name="skip">Skip options</param>
        /// <param name="replaceReferenceTags">If true, <div class="references-small"><references/></div> and so on
        /// will be replaced with {{reflist}}</param>
        public void PerformGeneralFixes(Parsers parsers, HideText removeText, ISkipOptions skip, bool replaceReferenceTags)
        {
            BeforeGeneralFixesTextChanged();

            HideText(removeText);

            Variables.Profiler.Profile("HideText");

            FixHeaderErrors(parsers, Variables.LangCode, skip.SkipNoHeaderError);
            Variables.Profiler.Profile("FixHeaderErrors");
            SetDefaultSort(parsers, Variables.LangCode, skip.SkipNoDefaultSortAdded);
            Variables.Profiler.Profile("SetDefaultSort");

            AWBChangeArticleText("Fix categories", Parsers.FixCategories(ArticleText), true);
            Variables.Profiler.Profile("FixCategories");

            AWBChangeArticleText("Fix images", Parsers.FixImages(ArticleText), true);
            Variables.Profiler.Profile("FixImages");

            AWBChangeArticleText("Fix whitespace in links", Parsers.FixLinkWhitespace(ArticleText), true);
            Variables.Profiler.Profile("FixLinkWhitespace");
            AWBChangeArticleText("Fix syntax", parsers.FixSyntax(ArticleText), true);
            Variables.Profiler.Profile("FixSyntax");

            AWBChangeArticleText("Fix temperatures", parsers.FixTemperatures(ArticleText), true);
            Variables.Profiler.Profile("FixTemperatures");

            AWBChangeArticleText("Fix main article", Parsers.FixMainArticle(ArticleText), true);
            Variables.Profiler.Profile("FixMainArticle");

            if (replaceReferenceTags)
            {
                AWBChangeArticleText("Fix reference tags", parsers.FixReferenceListTags(ArticleText), true);
                Variables.Profiler.Profile("FixReferenceTags");
            }

            AWBChangeArticleText("Fix empty links and templates", Parsers.FixEmptyLinksAndTemplates(ArticleText), true);
            Variables.Profiler.Profile("FixEmptyLinksAndTemplates");

            AWBChangeArticleText("Fix empty references", Parsers.SimplifyReferenceTags(ArticleText), true);
            Variables.Profiler.Profile("FixEmptyReferences");

            //Just a bit broken/Some unwanted fixes (moving of <ref> tags around)
            //AWBChangeArticleText("Fix Footnotes", parsers.FixFootnotes(ArticleText), true);
            //Variables.Profiler.Profile("FixFootnotes");

            FixLinks(parsers, skip.SkipNoBadLink);
            Variables.Profiler.Profile("FixLinks");
            BulletExternalLinks(parsers, skip.SkipNoBulletedLink);
            Variables.Profiler.Profile("BulletExternalLinks");

            AWBChangeArticleText("Remove empty comments", Parsers.RemoveEmptyComments(ArticleText), false);
            Variables.Profiler.Profile("RemoveEmptyComments");

            //if (Variables.IsWikimediaProject)
            //{
            //    AWBChangeArticleText("External to internal links", Parsers.ExternalURLToInternalLink(ArticleText), true);
            //    Variables.Profiler.Profile("ExternalURLToInternalLink");
            //}

            Variables.Profiler.Profile("Links");

            if (!Globals.UnitTestMode) // disable to avoid ssslow network requests
            {
                AWBChangeArticleText("Sort meta data",
                    parsers.SortMetaData(ArticleText, Name), true);

                Variables.Profiler.Profile("Metadata");
            }

            EmboldenTitles(parsers, skip.SkipNoBoldTitle);

            AWBChangeArticleText("Format sticky links",
                Parsers.StickyLinks(Parsers.SimplifyLinks(ArticleText)), true);

            //AWBChangeArticleText("Remove duplicate wikilink", parsers.RemoveDuplicateWikiLinks(ArticleText), true);

            UnHideText(removeText);

            AfterGeneralFixesTextChanged();

            Variables.Profiler.Profile("End of general fixes");
        }
        #endregion

        /// <summary>
        /// Check if the article text has already been changed when the code reaches this point
        /// </summary>
        private void BeforeGeneralFixesTextChanged()
        {
            textAlreadyChanged = (ArticleText != OriginalArticleText);
        }

        /// <summary>
        /// If the text hasnt been changed prior to starting general fixes, see if the general fixes have made a change,
        /// if it has, make a copy of the article text post general fix changes
        /// </summary>
        private void AfterGeneralFixesTextChanged()
        {
            if (!textAlreadyChanged)
            {
                generalFixesCausedChange = (ArticleText != OriginalArticleText);

                if (generalFixesCausedChange)
                    afterGeneralFixesArticleText = ArticleText;
            }
        }

        /// <summary>
        /// Performs the general fixes for user talk pages (ie user talk template substitution)
        /// </summary>
        /// <param name="removeText"></param>
        /// <param name="userTalkTemplatesRegex">Regex of user talk templates to substitute</param>
        /// <param name="SkipIfNoChange"></param>
        public void PerformUserTalkGeneralFixes(HideText removeText, Regex userTalkTemplatesRegex, bool SkipIfNoChange)
        {
            string originalText = ArticleText;
            HideText(removeText);
            Variables.Profiler.Profile("HideText");

            AWBChangeArticleText("Subst user talk warnings",
                Parsers.SubstUserTemplates(ArticleText, Name, userTalkTemplatesRegex), true);

            Variables.Profiler.Profile("SubstUserTemplates");

            UnHideText(removeText);
            Variables.Profiler.Profile("UnHideText");

            if (SkipIfNoChange && (originalText == ArticleText))
            {
                Trace.AWBSkipped("No user talk templates subst'd");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsRedirect
        {
            get { return Tools.IsRedirect(ArticleText); }
        }
    }

    /// <summary>
    /// A simple read-only article interface
    /// </summary>
    // TODO: Primarily for use with IsStub() etc, by plugin
    public interface IArticleSimple
    {
        Article Article { get; }
        string Name { get; }
        int NameSpaceKey { get; }
        string ArticleText { get; }
        bool IsStub { get; }
        bool HasStubTemplate { get; }
        bool HasInfoBox { get; }
    }
}
