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
    public class Article : IProcessArticleEventArgs, IArticleSimple, IComparable<Article>
    {
        protected string mName = "";
        protected string mEditSummary = "";
        protected string mSavedSummary = "";
        protected AWBLogListener mAWBLogListener;
        protected string mArticleText = "";
        protected string mOriginalArticleText = "";
        protected string mPluginEditSummary = "";
        protected bool mPluginSkip;

        private bool noChange;

        public virtual IAWBTraceListener Trace
        { get { return mAWBLogListener; } }

        public bool PreProcessed;

        #region Constructors
        public Article()
        {
            EditSummary = "";
        }

        public Article(string name)
            : this(name, Namespace.Determine(name))
        { }

        public Article(string name, int nameSpaceKey)
            : this()
        {
            mName = name.Contains("#") ? name.Substring(0, name.IndexOf('#')) : name;

            NameSpaceKey = nameSpaceKey;
        }

        public virtual AWBLogListener InitialiseLogListener()
        {
            InitLog();
            return mAWBLogListener;
        }

        public AWBLogListener InitialiseLogListener(string name, TraceManager traceManager)
        {
            // Initialise a Log Listener and add it to a TraceManager collection
            InitLog();
            traceManager.AddListener(name, mAWBLogListener);
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
                if (NameSpaceKey == 0) return mName;

                int pos = mName.IndexOf(':');
                return pos < 0 ? mName : mName.Substring(pos + 1).Trim();
            }
        }

        /// <summary>
        /// The namespace of the article
        /// </summary>
        [XmlAttribute]
        public int NameSpaceKey
        { get; set; }
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
        /// returns whether the article's title contains any recognised diacritic(s)
        /// </summary>
        [XmlIgnore]
        public bool HasDiacriticsInTitle
            { get { return (Tools.RemoveDiacritics(Name) != Name);} }

        /// <summary>
        /// returns whether the article is about a person
        /// </summary>
        [XmlIgnore]
        public bool ArticleIsAboutAPerson
        { get { return Parsers.IsArticleAboutAPerson(mArticleText); } }

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
        /// Returns true if the article contains a sic template or bracketed wording, indicating the presence of a deliberate typo
        /// </summary>
        [XmlIgnore]
        public bool HasSicTag
        { get { return Parsers.HasSicTag(mArticleText); } }

        /// <summary>
        /// Returns whether the article contains any {{dead link}} templates
        /// </summary>
        [XmlIgnore]
        public bool HasDeadLinks
        { get { return Parsers.HasDeadLinks(mArticleText); } }

        /// <summary>
        /// Returns true if the article contains a {{nofootnotes}} or {{morefootnotes}} template but has 5+ <ref>...</ref> references
        /// </summary>
        [XmlIgnore]
        public bool HasMorefootnotesAndManyReferences
        { get { return Parsers.HasMorefootnotesAndManyReferences(mArticleText); } }

        /// <summary>
        /// Returns true if the article contains a <ref>...</ref> reference after the {{reflist}} to show them
        /// </summary>
        // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#.28Yet.29_more_reference_related_changes.
        [XmlIgnore]
        public bool HasRefAfterReflist
        { get { return Parsers.HasRefAfterReflist(mArticleText); } }

        /// <summary>
        /// Returns true if the article contains bare references (just the URL link on a line with no description/name)
        /// </summary>
        [XmlIgnore]
        public bool HasBareReferences
        { get { return Parsers.HasBareReferences(mArticleText); } }

        /// <summary>
        /// Returns true if the article should be skipped; check after each call to a worker member. See AWB main.cs.
        /// </summary>
        [XmlIgnore]
        public bool SkipArticle
        { get { return mAWBLogListener.Skipped; } private set { mAWBLogListener.Skipped = value; } }

        [XmlIgnore]
        public bool CanDoGeneralFixes
        {
            get
            {
                return (NameSpaceKey == Namespace.Article
                    || NameSpaceKey == Namespace.Category
                    || Name.Contains("Sandbox"))
                        || Name.Contains("/doc");
            }
        }

        /// <summary>
        /// Returns true if the article uses cite references but has no recognised template to display the references
        /// </summary>
        public bool IsMissingReferencesDisplay
        { get { return Parsers.IsMissingReferencesDisplay(mArticleText); } }
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
            get { return Tools.CaseInsensitiveStringCompare(mOriginalArticleText, mArticleText); }
        }

        /// <summary>
        /// Returns whether the only change between the current article text and the original article text is whitespace and casing changes
        /// </summary>
        public bool OnlyWhiteSpaceAndCasingChanged
        {
            get { return Tools.CaseInsensitiveStringCompare(Regex.Replace(mOriginalArticleText, @"\s+", ""), Regex.Replace(mArticleText, @"\s+", "")); }
        }

        /// <summary>
        /// Returns whether the only change between the current article text and the original article text was by the general fixes
        /// </summary>
        public bool OnlyGeneralFixesChanged
        {
            get { return (GeneralFixesCausedChange && (ArticleText == AfterGeneralFixesArticleText)); }
        }

        /// <summary>
        /// Returns whether the only general fix changes are minor ones
        /// </summary>
        public bool OnlyMinorGeneralFixesChanged
        {
            get { return (OnlyGeneralFixesChanged && !GeneralFixesSignificantChange); }
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
        /// <param name="skipIfNoChange">True if the article should be skipped if no changes are made</param>
        /// <param name="parsers">An initialised Parsers object</param>
        public void Unicodify(bool skipIfNoChange, Parsers parsers)
        {
            string strTemp = parsers.Unicodify(mArticleText, out noChange);

            if (skipIfNoChange && noChange)
                Trace.AWBSkipped("No Unicodification");
            else if (!noChange)
                AWBChangeArticleText("Page Unicodified", strTemp, false);
        }

        /// <summary>
        /// Checks the article text for unbalanced brackets, either square or curly
        /// </summary>
        /// <param name="bracketLength">integer to hold length of unbalanced bracket found</param>
        /// <returns>Index of any unbalanced brackets found</returns>
        public int UnbalancedBrackets(ref int bracketLength)
        {
            return Parsers.UnbalancedBrackets(ArticleText, ref bracketLength);
        }

        /// <summary>
        /// Remove, replace or comment out a specified image
        /// </summary>
        /// <param name="option">The action to take</param>
        /// <param name="imageReplaceText">The text (image name) to look for</param>
        /// <param name="imageWithText">Replacement text (if applicable)</param>
        /// <param name="skipIfNoChange">True if the article should be skipped if no changes are made</param>
        public void UpdateImages(ImageReplaceOptions option,
            string imageReplaceText, string imageWithText, bool skipIfNoChange)
        {
            string strTemp = "";

            imageReplaceText = imageReplaceText.Trim();
            imageWithText = imageWithText.Trim();

            if (imageReplaceText.Length > 0)
                switch (option)
                {
                    case ImageReplaceOptions.NoAction:
                        return;

                    case ImageReplaceOptions.Replace:
                        if (imageWithText.Length > 0) strTemp = Parsers.ReplaceImage(imageReplaceText, imageWithText, mArticleText, out noChange);
                        break;

                    case ImageReplaceOptions.Remove:
                        strTemp = Parsers.RemoveImage(imageReplaceText, mArticleText, false, imageWithText, out noChange);
                        break;

                    case ImageReplaceOptions.Comment:
                        strTemp = Parsers.RemoveImage(imageReplaceText, mArticleText, true, imageWithText, out noChange);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException("option");
                }

            if (noChange && skipIfNoChange)
                Trace.AWBSkipped("No File Changed");
            else if (!noChange)
                AWBChangeArticleText("File replacement applied", strTemp, false);
        }

        /// <summary>
        /// Add, remove or replace a specified category
        /// </summary>
        /// <param name="option">The action to take</param>
        /// <param name="parsers">An initialised Parsers object</param>
        /// <param name="skipIfNoChange">True if the article should be skipped if no changes are made</param>
        /// <param name="categoryText">The category to add or remove; or, when replacing, the name of the old category</param>
        /// <param name="categoryText2">The name of the replacement category (recat mode only)</param>
        public void Categorisation(CategorisationOptions option, Parsers parsers,
            bool skipIfNoChange, string categoryText, string categoryText2, bool removeSortKey)
        {
            string strTemp, action = "";

            switch (option)
            {
                case CategorisationOptions.NoAction:
                    return;

                case CategorisationOptions.AddCat:
                    if (categoryText.Length < 1) return;
                    strTemp = parsers.AddCategory(categoryText, mArticleText, mName, out noChange);
                    action = "Added " + categoryText;
                    break;

                case CategorisationOptions.ReCat:
                    if (categoryText.Length < 1 || categoryText2.Length < 1) return;
                    strTemp = Parsers.ReCategoriser(categoryText, categoryText2, mArticleText, out noChange, removeSortKey);
                    break;

                case CategorisationOptions.RemoveCat:
                    if (categoryText.Length < 1) return;
                    strTemp = Parsers.RemoveCategory(categoryText, mArticleText, out noChange);
                    action = "Removed " + categoryText;
                    break;

                default:
                    throw new ArgumentOutOfRangeException("option");
            }

            if (noChange && skipIfNoChange)
                Trace.AWBSkipped("No Category Changed");
            else if (!noChange)
                AWBChangeArticleText(action, strTemp, false);
        }

        /// <summary>
        /// Add or remove a specified category
        /// </summary>
        /// <param name="option">The action to take</param>
        /// <param name="parsers">An initialised Parsers object</param>
        /// <param name="skipIfNoChange">True if the article should be skipped if no changes are made</param>
        /// <param name="newCategoryText">The category to add or remove</param>
        public void Categorisation(CategorisationOptions option, Parsers parsers,
            bool skipIfNoChange, string newCategoryText)
        {
            if (option == CategorisationOptions.ReCat)
                throw new ArgumentException("This overload has no CategoryText2 argument");
            Categorisation(option, parsers, skipIfNoChange, newCategoryText, "", false);
        }

        /// <summary>
        /// Process a "find and replace"
        /// </summary>
        /// <param name="findAndReplace">A FindandReplace object</param>
        /// <param name="substTemplates">A SubstTemplates object</param>
        /// <param name="replaceSpecial">An MWB ReplaceSpecial object</param>
        /// <param name="skipIfNoChange">True if the article should be skipped if no changes are made</param>
        public void PerformFindAndReplace(FindandReplace findAndReplace, SubstTemplates substTemplates,
            ReplaceSpecial.ReplaceSpecial replaceSpecial, bool skipIfNoChange)
        {
            if (!findAndReplace.HasReplacements && !replaceSpecial.HasRules && !substTemplates.HasSubstitutions)
                return;

            string strTemp = mArticleText.Replace("\r\n", "\n"),
                   testText = strTemp,
                   tmpEditSummary = "";

            strTemp = findAndReplace.MultipleFindAndReplace(strTemp, mName, ref tmpEditSummary);
            strTemp = replaceSpecial.ApplyRules(strTemp, mName);
            strTemp = substTemplates.SubstituteTemplates(strTemp, mName);

            if (testText == strTemp)
            {
                if (skipIfNoChange)
                    Trace.AWBSkipped("No Find And Replace Changes");
                else
                    return;
            }
            else
            {
                AWBChangeArticleText("Find and replace applied" + tmpEditSummary,
                                     strTemp.Replace("\n", "\r\n"), true);
                EditSummary += tmpEditSummary;
            }
        }

        /// <summary>
        /// Fix spelling mistakes
        /// </summary>
        /// <param name="regexTypos">A RegExTypoFix object</param>
        /// <param name="skipIfNoChange">True if the article should be skipped if no changes are made</param>
        public void PerformTypoFixes(RegExTypoFix regexTypos, bool skipIfNoChange)
        {
            string strTemp = regexTypos.PerformTypoFixes(mArticleText, out noChange, out mPluginEditSummary, mName);

            if (noChange && skipIfNoChange)
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
        /// <param name="skipIfNoChange">True if the article should be skipped if no changes are made</param>
        public void AutoTag(Parsers parsers, bool skipIfNoChange)
        {
            string tmpEditSummary = "";
            string strTemp = parsers.Tagger(mArticleText, mName, out noChange, ref tmpEditSummary);

            if (skipIfNoChange && noChange)
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
        /// <param name="parsers"></param>
        /// <param name="langCode">The wiki's language code</param>
        /// <param name="skipIfNoChange">True if the article should be skipped if no changes are made</param>
        protected void FixHeaderErrors(Parsers parsers, LangCodeEnum langCode, bool skipIfNoChange)
        {
            if (langCode == LangCodeEnum.en)
            {
                string strTemp = Parsers.Conversions(mArticleText);

                strTemp = parsers.FixDates(strTemp);
                strTemp = Parsers.FixLivingThingsRelatedDates(strTemp);
                strTemp = Parsers.FixHeadings(strTemp, mName, out noChange);

                if (mArticleText == strTemp)
                {
                    if (skipIfNoChange)
                        Trace.AWBSkipped("No header errors");
                }
                else
                {
                    if (!noChange)
                        AWBChangeArticleText("Fixed header errors", strTemp, true);
                    else
                    {
                        AWBChangeArticleText("Fixed minor formatting issues", strTemp, true);
                        if (skipIfNoChange) Trace.AWBSkipped("No header errors");
                    }

                }
            }
        }

        /// <summary>
        /// Sets Default Sort on Article if Necessary / clean diacritics
        /// </summary>
        /// <param name="langCode">The wiki's language code</param>
        /// <param name="skipIfNoChange">True if the article should be skipped if no changes are made</param>
        /// <param name="restrictDefaultsortAddition"></param>
        public void SetDefaultSort(LangCodeEnum langCode, bool skipIfNoChange, bool restrictDefaultsortAddition)
        {
            if (langCode == LangCodeEnum.en)
            {
                string strTemp = Parsers.ChangeToDefaultSort(mArticleText, mName, out noChange, restrictDefaultsortAddition);

                if (skipIfNoChange && noChange)
                    Trace.AWBSkipped("No DefaultSort Added");
                else if (!noChange)
                    AWBChangeArticleText("DefaultSort Added/Diacritics cleaned", strTemp, true);
            }
        }

         /// <summary>
        /// Sets Default Sort on Article if Necessary / clean diacritics, in restricted addition mode
        /// </summary>
        /// <param name="langCode">The wiki's language code</param>
        /// <param name="skipIfNoChange">True if the article should be skipped if no changes are made</param>
        /// <param name="restrictDefaultsortAddition"></param>
        public void SetDefaultSort(LangCodeEnum langCode, bool skipIfNoChange)
        {
            SetDefaultSort(langCode, skipIfNoChange, true);
        }

        /// <summary>
        /// Corrects common formatting errors in dates in external reference citation templates (doesn't link/delink dates)
        /// </summary>
        /// <param name="parsers"></param>
        /// <param name="skipIfNoChange">True if the article should be skipped if no changes are made</param>
        public void CiteTemplateDates(Parsers parsers, bool skipIfNoChange)
        {
            string strTemp = parsers.CiteTemplateDates(mArticleText, out noChange);

            if (skipIfNoChange && noChange)
                Trace.AWBSkipped("No Citation template dates fixed");
            else if (!noChange)
                AWBChangeArticleText("Citation template dates fixed", strTemp, true);
        }

        /// <summary>
        /// Adds [[Category:XXXX births]], [[Category:XXXX deaths]], [[Category:Living people]] etc. to articles about people where available, for en-wiki only
        /// </summary>
        /// <param name="parsers"></param>
        /// <param name="skipIfNoChange">True if the article should be skipped if no changes are made</param>
        public void FixPeopleCategories(Parsers parsers, bool skipIfNoChange)
        {
            bool noChange2 = false;
            string strTemp = parsers.FixPeopleCategories(mArticleText, out noChange);
            strTemp = Parsers.LivingPeople(strTemp, out noChange2);

            if (!noChange2)
                noChange = false;

            if (skipIfNoChange && noChange)
                Trace.AWBSkipped("No human category changes");
            else if (!noChange)
                AWBChangeArticleText("Human category changes", strTemp, true);
        }

        /// <summary>
        /// Fix link syntax
        /// </summary>
        /// <param name="skipIfNoChange">True if the article should be skipped if no changes are made</param>
        public void FixLinks(bool skipIfNoChange)
        {
            string strTemp = Parsers.FixLinks(mArticleText, Name, out noChange);
            if (noChange && skipIfNoChange)
                Trace.AWBSkipped("No bad links");
            else if (!noChange)
                AWBChangeArticleText("Fixed links", strTemp, false);
        }

        /// <summary>
        /// Add bulletpoints to external links, if necessary
        /// </summary>
        /// <param name="skipIfNoChange">True if the article should be skipped if no changes are made</param>
        public void BulletExternalLinks(bool skipIfNoChange)
        {
            string strTemp = Parsers.BulletExternalLinks(mArticleText, out noChange);
            if (skipIfNoChange && noChange)
                Trace.AWBSkipped("No missing bulleted links");
            else if (!noChange)
                AWBChangeArticleText("Bulleted external links", strTemp, false);
        }

        /// <summary>
        /// '''Emboldens''' the first occurence of the article title, if not already bold
        /// </summary>
        /// <param name="parsers"></param>
        /// <param name="skipIfNoChange">True if the article should be skipped if no changes are made</param>
        public void EmboldenTitles(Parsers parsers, bool skipIfNoChange)
        {
            string strTemp = parsers.BoldTitle(mArticleText, mName, out noChange);
            if (skipIfNoChange && noChange)
                Trace.AWBSkipped("No Titles to embolden");
            else if (!noChange)
                AWBChangeArticleText("Emboldened titles", strTemp, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="module"></param>
        public void SendPageToCustomModule(IModule module)
        {
            IProcessArticleEventArgs processArticleEventArgs = this;
            string strEditSummary;
            bool skipArticle;

            string strTemp = module.ProcessArticle(processArticleEventArgs.ArticleText,
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
        /// <param name="findText">The text to find</param>
        /// <param name="regEx">True if FindText contains a regular expression</param>
        /// <param name="caseSensitive">True if the search should be case sensitive</param>
        /// <param name="doesContain">True if the article should be skipped if it contains the text, false if it should be skipped if it does *not* contain the text</param>
        /// <returns>A boolean value indicating whether or not the article should be skipped</returns>
        public bool SkipIfContains(string findText, bool regEx, bool caseSensitive, bool doesContain)
        {
            if (findText.Length > 0)
            {
                RegexOptions regexOptions = (caseSensitive) ? RegexOptions.None : RegexOptions.IgnoreCase;

                findText = Tools.ApplyKeyWords(Name, findText);

                if (!regEx)
                    findText = Regex.Escape(findText);

                if (Regex.IsMatch(OriginalArticleText, findText, regexOptions))
                    return doesContain;

                return !doesContain;
            }

            return false;
        }

        /// <summary>
        /// Disambiguate
        /// </summary>
        /// <returns>True if OK to proceed, false to abort</returns>
        public bool Disambiguate(string dabLinkText, string[] dabVariantsLines, bool botMode, int context,
            bool skipIfNoChange)
        {
            Disambiguation.DabForm df = new Disambiguation.DabForm();
            string strTemp = df.Disambiguate(mArticleText, mName, dabLinkText,
                dabVariantsLines, context, botMode, out noChange);

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
        /// A subroutine allowing AWB to modify the article text. Passes through to ChangeArticleText()
        /// </summary>
        /// <param name="reason">Why the text was changed</param>
        /// <param name="newText">The new text</param>
        /// <param name="checkIfChanged">Check if the new text does differ from the existing text before logging it; exits silently if this param is true and there was no change</param>
        /// <param name="performsSignificantChanges">indicates whether the general fix function makes 'significant' changes</param>
        public void AWBChangeArticleText(string reason, string newText, bool checkIfChanged, bool performsSignificantChanges)
        {
            if (performsSignificantChanges && !(newText == mArticleText))
                GeneralFixesSignificantChange = true;

            AWBChangeArticleText(reason, newText, checkIfChanged);
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

        public void HideText(HideText removeText)
        { mArticleText = removeText.Hide(mArticleText); }

        public void UnHideText(HideText removeText)
        { mArticleText = removeText.AddBack(mArticleText); }

        public void HideMoreText(HideText removeText)
        { mArticleText = removeText.HideMore(mArticleText); }

        public void UnHideMoreText(HideText removeText)
        { mArticleText = removeText.AddBackMore(mArticleText); }
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

        //IMyTraceListener IProcessArticleEventArgs.AWBLogItem
        //{ get { return mAWBLogListener; } }

        string IProcessArticleEventArgs.ArticleTitle
        { get { return mName; } }

        string IProcessArticleEventArgs.EditSummary // this is temp edit summary field, sent from plugin
        { get { return mPluginEditSummary; } set { mPluginEditSummary = value.Trim(); } }

        bool IProcessArticleEventArgs.Skip
        { get { return mPluginSkip; } set { mPluginSkip = value; } }

        // and NamespaceKey

        Article IArticleSimple.Article { get { return this; } }
        #endregion

        public static IArticleSimple GetReadOnlyArticle(string title)
        {
            // TODO: See Parsers.HasInfobox
            return null;
        }

        #region General fixes
        private bool GeneralFixesCausedChange, TextAlreadyChanged, GeneralFixesSignificantChange;
        private string AfterGeneralFixesArticleText;

        /// <summary>
        /// Performs numerous minor improvements to the page text
        /// </summary>
        /// <param name="parsers">A parser object</param>
        /// <param name="removeText"></param>
        /// <param name="skip">Skip options</param>
        /// <param name="replaceReferenceTags">If true, <div class="references-small"><references/></div> and so on
        /// <param name="restrictDefaultsortAddition"></param>
        /// will be replaced with {{reflist}}</param>
        public void PerformGeneralFixes(Parsers parsers, HideText removeText, ISkipOptions skip, bool replaceReferenceTags, bool restrictDefaultsortAddition, bool noMOSComplianceFixes)
        { //TODO: 2009-01-28 review which of the genfixes below should be labelled 'significant'
            BeforeGeneralFixesTextChanged();

            HideText(removeText);

            Variables.Profiler.Profile("HideText");

            // call this before FixHeaderErrors so that Parsers.Conversions cleans up from ArticleIssues
            AWBChangeArticleText("Fixes for {{article issues}}", parsers.ArticleIssues(ArticleText), true);
            Variables.Profiler.Profile("ArticleIssues");

            FixHeaderErrors(parsers, Variables.LangCode, skip.SkipNoHeaderError);
            Variables.Profiler.Profile("FixHeaderErrors");

            FixPeopleCategories(parsers, skip.SkipNoPeopleCategoriesFixed);
            Variables.Profiler.Profile("FixPeopleCategories");

            SetDefaultSort(Variables.LangCode, skip.SkipNoDefaultSortAdded, restrictDefaultsortAddition);
            Variables.Profiler.Profile("SetDefaultSort");

            AWBChangeArticleText("Fix categories", Parsers.FixCategories(ArticleText, Namespace.IsMainSpace(Name)), true);
            Variables.Profiler.Profile("FixCategories");

            AWBChangeArticleText("Fix images", Parsers.FixImages(ArticleText), true);
            Variables.Profiler.Profile("FixImages");

            AWBChangeArticleText("Fix whitespace in links", Parsers.FixLinkWhitespace(ArticleText, Name), true);
            Variables.Profiler.Profile("FixLinkWhitespace");

            // does significant fixes
            AWBChangeArticleText("Fix syntax", Parsers.FixSyntax(ArticleText), true, true);
            Variables.Profiler.Profile("FixSyntax");

            AWBChangeArticleText("Fix citation templates", Parsers.FixCitationTemplates(ArticleText), true, false);
            Variables.Profiler.Profile("FixCitationTemplates");

            AWBChangeArticleText("Fix temperatures", Parsers.FixTemperatures(ArticleText), true);
            Variables.Profiler.Profile("FixTemperatures");

            AWBChangeArticleText("Fix non-breaking spaces", parsers.FixNonBreakingSpaces(ArticleText), true);
            Variables.Profiler.Profile("FixNonBreakingSpaces");

            AWBChangeArticleText("Fix main article", Parsers.FixMainArticle(ArticleText), true);
            Variables.Profiler.Profile("FixMainArticle");

            if (replaceReferenceTags)
            {
                AWBChangeArticleText("Fix reference tags", Parsers.FixReferenceListTags(ArticleText), true);
                Variables.Profiler.Profile("FixReferenceListTags");
            }

            AWBChangeArticleText("Fix empty links and templates", Parsers.FixEmptyLinksAndTemplates(ArticleText), true);
            Variables.Profiler.Profile("FixEmptyLinksAndTemplates");

            AWBChangeArticleText("FixReferenceTags", Parsers.FixReferenceTags(ArticleText), true);
            Variables.Profiler.Profile("FixReferenceTags");

            AWBChangeArticleText("DuplicateUnnamedReferences", Parsers.DuplicateUnnamedReferences(ArticleText), true);
            Variables.Profiler.Profile("DuplicateUnnamedReferences");

            AWBChangeArticleText("DuplicateNamedReferences", Parsers.DuplicateNamedReferences(ArticleText), true);
            Variables.Profiler.Profile("DuplicateNamedReferences");

            AWBChangeArticleText("ReorderReferences", Parsers.ReorderReferences(ArticleText), true);
            Variables.Profiler.Profile("ReorderReferences");

            AWBChangeArticleText("Fix empty references", Parsers.SimplifyReferenceTags(ArticleText), true);
            Variables.Profiler.Profile("FixEmptyReferences");

            // does significant fixes
            if (IsMissingReferencesDisplay)
            {
                AWBChangeArticleText("Add missing {{reflist}}", Parsers.AddMissingReflist(ArticleText), true, true);
                Variables.Profiler.Profile("AddMissingReflist");
            }

            AWBChangeArticleText("Mdashes", parsers.Mdashes(ArticleText, Name, NameSpaceKey), true);
            Variables.Profiler.Profile("Mdashes");

            CiteTemplateDates(parsers, skip.SkipNoCiteTemplateDatesFixed);
            Variables.Profiler.Profile("CiteTemplateDates");

            //Just a bit broken/Some unwanted fixes (moving of <ref> tags around)
            //AWBChangeArticleText("Fix Footnotes", parsers.FixFootnotes(articleText), true);
            //Variables.Profiler.Profile("FixFootnotes");

            BulletExternalLinks(skip.SkipNoBulletedLink);
            Variables.Profiler.Profile("BulletExternalLinks");

            AWBChangeArticleText("Remove empty comments", Parsers.RemoveEmptyComments(ArticleText), false);
            Variables.Profiler.Profile("RemoveEmptyComments");

            if (!noMOSComplianceFixes)
            {
                AWBChangeArticleText("Fix Date Ordinals/Of", parsers.FixDateOrdinalsAndOf(ArticleText, Name), true, true);
                Variables.Profiler.Profile("FixDateOrdinalsAndOf");
            }

            //if (Variables.IsWikimediaProject)
            //{
            //    AWBChangeArticleText("External to internal links", Parsers.ExternalURLToInternalLink(articleText), true);
            //    Variables.Profiler.Profile("ExternalURLToInternalLink");
            //}

            Variables.Profiler.Profile("Links");

            if (!Globals.UnitTestMode) // disable to avoid ssslow network requests
            {
                // pass unhidden text to MetaDataSorter so that it can allow for comments around persondata, categories etc.
                UnHideText(removeText);
                AWBChangeArticleText("Sort meta data",
                    parsers.SortMetaData(ArticleText, Name), true);
                HideText(removeText);

                Variables.Profiler.Profile("Metadata");
            }

            // must call EmboldenTitles before calling FixLinks
            EmboldenTitles(parsers, skip.SkipNoBoldTitle);

            FixLinks(skip.SkipNoBadLink);
            Variables.Profiler.Profile("FixLinks");

            AWBChangeArticleText("Format sticky links",
                Parsers.StickyLinks(Parsers.SimplifyLinks(ArticleText)), true);

            //AWBChangeArticleText("Remove duplicate wikilink", parsers.RemoveDuplicateWikiLinks(articleText), true);

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
            TextAlreadyChanged = (ArticleText != OriginalArticleText);
        }

        /// <summary>
        /// If the text hasnt been changed prior to starting general fixes, see if the general fixes have made a change,
        /// if it has, make a copy of the article text post general fix changes
        /// </summary>
        private void AfterGeneralFixesTextChanged()
        {
            if (!TextAlreadyChanged)
            {
                GeneralFixesCausedChange = (ArticleText != OriginalArticleText);

                if (GeneralFixesCausedChange)
                    AfterGeneralFixesArticleText = ArticleText;
            }
        }

        /// <summary>
        /// Performs the general fixes for user talk pages (ie user talk template substitution)
        /// </summary>
        /// <param name="removeText"></param>
        /// <param name="userTalkTemplatesRegex">Regex of user talk templates to substitute</param>
        /// <param name="skipIfNoChange"></param>
        public void PerformUserTalkGeneralFixes(HideText removeText, Regex userTalkTemplatesRegex, bool skipIfNoChange)
        {
            string originalText = ArticleText;
            HideText(removeText);
            Variables.Profiler.Profile("HideText");

            AWBChangeArticleText("Subst user talk warnings",
                Parsers.SubstUserTemplates(ArticleText, Name, userTalkTemplatesRegex), true);

            Variables.Profiler.Profile("SubstUserTemplates");

            UnHideText(removeText);
            Variables.Profiler.Profile("UnHideText");

            if (skipIfNoChange && (originalText == ArticleText))
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
