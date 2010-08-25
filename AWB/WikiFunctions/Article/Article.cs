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
using System.Text;
using System.Xml.Serialization;
using WikiFunctions.Logging;
using System.Text.RegularExpressions;
using WikiFunctions.Plugin;
using WikiFunctions.Options;
using WikiFunctions.Parse;
using WikiFunctions.Controls;
using System.Windows.Forms;
using WikiFunctions.API;
using System.Collections.Generic;
using WikiFunctions.TalkPages;

namespace WikiFunctions
{
    public delegate void ArticleRedirected(string oldTitle, string newTitle);

    public enum Exists { Yes, No, Unknown }

    public delegate void AddListenerDelegate(string key, IMyTraceListener listener);

    /// <summary>
    /// A class which represents a wiki article
    /// </summary>
    public class Article : IProcessArticleEventArgs, IComparable<Article>
    {
        protected AWBLogListener mAWBLogListener;
        protected string mArticleText = "";
        protected string mOriginalArticleText = "";
        protected string mPluginEditSummary = "";
        protected bool mPluginSkip;

        private readonly PageInfo mPage;

        private bool noChange;

        public virtual IAWBTraceListener Trace
        { get { return mAWBLogListener; } }

        public bool PreProcessed;

        #region Constructors
        public Article()
        {
            Exists = Exists.Unknown;
        }

        public Article(string name)
            : this(name, Namespace.Determine(name))
        {
        }

        public Article(string name, int nameSpaceKey)
            : this()
        {
            Name = name.Contains("#") ? name.Substring(0, name.IndexOf('#')) : name;
            InitialiseLogListener();
            NameSpaceKey = nameSpaceKey;
        }

        public Article(string name, string text)
            : this(name)
        {
            mOriginalArticleText = mArticleText = text;
        }

        public Article(PageInfo page)
            : this(page.Title, page.NamespaceID)
        {
            mPage = page;
            mArticleText = page.Text;
            Exists = page.Exists ? Exists.Yes : Exists.No;
        }

        private static event AddListenerDelegate addListener;
        private static TraceManager currentTraceManager;
        private static string whatName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="del"></param>
        /// <param name="trace"></param>
        /// <param name="what"></param>
        public static void SetAddListener(AddListenerDelegate del, TraceManager trace, string what)
        {
            addListener += del;
            currentTraceManager = trace;
            whatName = what;
        }

        /// <summary>
        /// 
        /// </summary>
        public void InitialiseLogListener()
        {
            if (mAWBLogListener != null)
                return;

            // Initialise a Log Listener and add it to a TraceManager collection
            mAWBLogListener = new AWBLogListener(Name);

            if (currentTraceManager != null)
            {
                currentTraceManager.AddListener(whatName, mAWBLogListener);
            }

            if (addListener != null)
            {
                addListener(whatName, mAWBLogListener);
            }
        }
        #endregion

        #region Serialisable properties
        /// <summary>
        /// The full name of the article
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlIgnore]
        public string NamespacelessName
        {
            get
            {
                if (NameSpaceKey == Namespace.Article) return Name;

                int pos = Name.IndexOf(':');
                return pos < 0 ? Name : Name.Substring(pos + 1).Trim();
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
        { get { return Tools.WikiEncode(Name); } }

        /// <summary>
        /// The text of the article. This is deliberately readonly; set using methods
        /// </summary>
        [XmlIgnore]
        public string ArticleText
        { get { return mArticleText; } }

        /// <summary>
        /// Article text before this program manipulated it
        /// </summary>
        [XmlIgnore]
        public string OriginalArticleText
        {
            get { return mPage == null ? mOriginalArticleText : mPage.Text; }
        }

        /// <summary>
        /// Edit summary proposed for article
        /// </summary>
        [XmlIgnore]
        public string EditSummary
        { get { return summary.ToString(); } }

        public void ResetEditSummary()
        {
            summary.Length = 0;
        }

        private readonly StringBuilder summary = new StringBuilder();

        private void AppendToSummary(string newText)
        {
            if (string.IsNullOrEmpty(newText.Trim()))
                return;

            if (summary.Length > 0)
                summary.Append(", " + newText);
            else
                summary.Append(newText);
        }

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
        { get { return (Tools.RemoveDiacritics(Name) != Name); } }

        /// <summary>
        /// returns whether the article is about a person
        /// </summary>
        [XmlIgnore]
        public bool ArticleIsAboutAPerson
        {
            get
            {
                return Variables.Project == ProjectEnum.wikipedia
                    && Variables.LangCode == "en"
                    && Parsers.IsArticleAboutAPerson(mArticleText, Name, true);
            }
        }

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
        /// Returns whether the article is a disambiguation page (en only)
        /// </summary>
        [XmlIgnore]
        public bool IsDisambiguationPage
        { get { return Variables.LangCode == "en" && NameSpaceKey == Namespace.Mainspace && WikiRegexes.Disambigs.IsMatch(mArticleText); } }

        /// <summary>
        /// Returns whether the article is a SIA page (en only)
        /// </summary>
        [XmlIgnore]
        public bool IsSIAPage
        { get { return Variables.LangCode == "en" && NameSpaceKey == Namespace.Mainspace && WikiRegexes.SIAs.IsMatch(mArticleText); } }

        /// <summary>
        /// Returns whether the article is a disambiguation page has references
        /// </summary>
        [XmlIgnore]
        public bool IsDisambiguationPageWithRefs
        { get { return IsDisambiguationPage && WikiRegexes.Refs.IsMatch(mArticleText); } }

        /// <summary>
        /// Returns true if the article contains a <ref>...</ref> reference after the {{reflist}} to show them
        /// </summary>
        // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#.28Yet.29_more_reference_related_changes.
        [XmlIgnore]
        public bool HasRefAfterReflist
        { get { return Parsers.HasRefAfterReflist(mArticleText); } }

        /// <summary>
        /// Returns true if the article uses named references ([[WP:REFNAME]])
        /// </summary>
        [XmlIgnore]
        public bool HasNamedReferences
        { get { return Parsers.HasNamedReferences(mArticleText); } }

        /// <summary>
        /// Returns true if the article contains bare references (just the URL link on a line with no description/name)
        /// </summary>
        [XmlIgnore]
        public bool HasBareReferences
        { get { return Parsers.HasBareReferences(mArticleText); } }

        /// <summary>
        /// Returns true if the article contains ambiguous dates within a citation template
        /// </summary>
        [XmlIgnore]
        public bool HasAmbiguousCiteTemplateDates
        { get { return Parsers.AmbiguousCiteTemplateDates(mArticleText); } }
        
        /// <summary>
        /// Returns true if the article contains ambiguous dates within a citation template
        /// </summary>
        [XmlIgnore]
        public bool HasSeeAlsoAfterNotesReferencesOrExternalLinks
        { get { return (NameSpaceKey == Namespace.Article && Parsers.HasSeeAlsoAfterNotesReferencesOrExternalLinks(mArticleText)); } }

        /// <summary>
        /// Returns true if the article should be skipped; check after each call to a worker member. See AWB main.cs.
        /// </summary>
        [XmlIgnore]
        public bool SkipArticle
        { get { return mAWBLogListener.Skipped; } private set { mAWBLogListener.Skipped = value; } }

        /// <summary>
        /// Returns true of article general fixes can be applied to the page: article or category namespace, sandbox, template documnetation page or Anexo namespace on es-wiki
        /// </summary>
        [XmlIgnore]
        public bool CanDoGeneralFixes
        {
            get
            {
                return (NameSpaceKey == Namespace.Article
                        || NameSpaceKey == Namespace.Category
                        || Name.Contains("Sandbox")
                        || Name.Contains("/doc")
                        || (Variables.LangCode.Equals("es") && NameSpaceKey == 104 /* Anexo */));
            }
        }

        [XmlIgnore]
        public bool CanDoTalkGeneralFixes
        {
            get
            {
                return (NameSpaceKey == Namespace.Talk);
            }
        }

        /// <summary>
        /// Returns true if the article uses cite references but has no recognised template to display the references
        /// </summary>
        [XmlIgnore]
        public bool IsMissingReferencesDisplay
        { get { return Parsers.IsMissingReferencesDisplay(mArticleText); } }

        /// <summary>
        /// Returns the predominant date locale of the article (may be Undetermined)
        /// </summary>
        [XmlIgnore]
        public Parsers.DateLocale DateLocale
        { get { return Parsers.DeterminePredominantDateLocale(mArticleText); } }
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
        [XmlIgnore]
        public bool OnlyWhiteSpaceChanged
        {
            get
            {
                return
                    (string.Compare(WikiRegexes.WhiteSpace.Replace(OriginalArticleText, ""),
                                    WikiRegexes.WhiteSpace.Replace(mArticleText, "")) == 0);
            }
        }

        /// <summary>
        /// Does a case-insensitive comparison of the text, returning true if the same
        /// </summary>
        [XmlIgnore]
        public bool OnlyCasingChanged
        {
            get { return Tools.CaseInsensitiveStringCompare(OriginalArticleText, mArticleText); }
        }

        /// <summary>
        /// Returns whether the only change between the current article text and the original article text is whitespace and casing changes
        /// </summary>
        [XmlIgnore]
        public bool OnlyWhiteSpaceAndCasingChanged
        {
            get
            {
                return Tools.CaseInsensitiveStringCompare(WikiRegexes.WhiteSpace.Replace(OriginalArticleText, ""),
                                                          WikiRegexes.WhiteSpace.Replace(mArticleText, ""));
            }
        }

        /// <summary>
        /// Returns whether the only change between the current article text and the original article text was by the general fixes
        /// </summary>
        [XmlIgnore]
        public bool OnlyGeneralFixesChanged
        {
            get { return (_generalFixesCausedChange && (ArticleText == _afterGeneralFixesArticleText)); }
        }

        /// <summary>
        /// Returns whether the only general fix changes are minor ones
        /// </summary>
        [XmlIgnore]
        public bool OnlyMinorGeneralFixesChanged
        {
            get { return (OnlyGeneralFixesChanged && !_generalFixesSignificantChange); }
        }

        /// <summary>
        /// Returns whether the current article text is the same as the original article text
        /// </summary>
        [XmlIgnore]
        public bool NoArticleTextChanged
        {
            get { return (string.Compare(OriginalArticleText, mArticleText) == 0); }
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
        /// <returns>Dictionary of any unbalanced brackets found</returns>
        public Dictionary<int, int> UnbalancedBrackets()
        {
			Dictionary<int, int> UnB = new Dictionary<int, int>();
			int bracketLength = 0;
			int bracketIndex = Parsers.UnbalancedBrackets(ArticleText, ref bracketLength);
			
			if(bracketIndex > -1)
				UnB.Add(bracketIndex, bracketLength);
			
			return UnB;
        }

        /// <summary>
        /// Returns a dictionary of the index and length of any invalid or unknown citation parameters within a citation template
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, int> BadCiteParameters()
        {
            return Parsers.BadCiteParameters(ArticleText);
        }
        
        /// <summary>
        /// Returns a dictionary of the index and length of any duplicated parameters in any WikiProjectBannerShell template
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, int> DuplicateWikiProjectBannerShellParameters()
        {
            Dictionary<int, int> Dupes = new Dictionary<int, int>();
            
            if(NameSpaceKey.Equals(Namespace.Talk))
                Dupes = Tools.DuplicateTemplateParameters(WikiRegexes.WikiProjectBannerShellTemplate.Match(ArticleText).Value);
            
            return Dupes;
        }
        
        /// <summary>
        /// Returns a list of any unknown parameters in any WikiProjectBannerShell template
        /// </summary>
        /// <returns></returns>
        public List<string> UnknownWikiProjectBannerShellParameters()
        {
            List<string> Unknowns = new List<string>();
            List<string> Knowns = new List<string>(new[] { "blp", "blp", "activepol", "collapsed", "banner collapsed", "1"});
            
            if(NameSpaceKey.Equals(Namespace.Talk))
                Unknowns = Tools.UnknownTemplateParameters(WikiRegexes.WikiProjectBannerShellTemplate.Match(ArticleText).Value, Knowns);
            return Unknowns;
        }        
        
		/// <summary>
        /// Returns a list of any unknown parameters in any Multiple issues template
        /// </summary>
        /// <returns>List of unknown parameters</returns>
        public List<string> UnknownMultipleIssuesParameters()
        {
            List<string> Unknowns = new List<string>();
            List<string> Knowns = new List<string>(new[] { "advert", "autobiography", "biased", "blpdispute", "citations missing", "citation style", 
			"citecheck", "cleanup", "COI", "coi", "colloquial", "confusing", "context", "contradict", 
			"copyedit", "criticisms", "crystal", "deadend", "disputed", "essay", "examplefarm", "expert", "external links", 
			"fancruft", "fansite", "fiction", "globalize", "grammar", "histinfo", "hoax", "howto", "inappropriate person", "incomplete", 
			"lead missing", "lead rewrite", "lead too long", "lead too short", "in-universe", "jargon", "laundrylists", "laundry", "likeresume", 
			"long", "newsrelease", "notable", "notability", "onesource", "OR", "or", "original research", "orphan", "out of date", "peacock", 
			"plot", "POV", "NPOV", "pov", "npov", "pov-check", "primarysources", "prose", "proseline", "quotefarm", "recentism", "refimprove", 
			"refimprove BLP", "restructure", "reorganisation", "organize", "review", "rewrite", "sections", "self-published", 
			"spam", "story", "synthesis", "inappropriate tone", "tone", "travelguide", "trivia", "unbalanced", "unencyclopedic", "unreferenced", 
			"unreferencedBLP", "update", "weasel", "wikify"});
            
            if(NameSpaceKey.Equals(Namespace.Mainspace))
                Unknowns = Tools.UnknownTemplateParameters(WikiRegexes.MultipleIssues.Match(ArticleText).Value, Knowns);
            return Unknowns;
        }
        
        /// <summary>
        /// Returns a dictionary of the index and length of any unclosed &lt;math&gt;, &lt;source&gt;, &lt;code&gt;, &lt;nowiki&gt; or &lt;pre&gt; tags
        /// </summary>
        public Dictionary<int, int> UnclosedTags()
        { 
            return Parsers.UnclosedTags(ArticleText); 
        }

        /// <summary>
        /// Returns a dictionary of the index and length of any {{dead link}}s found
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, int> DeadLinks()
        {
            return Parsers.DeadLinks(ArticleText);
        }

        /// <summary>
        /// Returns a dictionary of ambiguously formatted dates within citation templates
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, int> AmbiguousCiteTemplateDates()
        {
            return Parsers.AmbigCiteTemplateDates(ArticleText);
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
        /// <param name="removeSortKey"></param>
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
                    strTemp = parsers.AddCategory(categoryText, mArticleText, Name, out noChange);
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
        /// Process a "find and replace"
        /// </summary>
        /// <param name="findAndReplace">A FindandReplace object</param>
        /// <param name="substTemplates">A SubstTemplates object</param>
        /// <param name="replaceSpecial">An MWB ReplaceSpecial object</param>
        /// <param name="skipIfNoChange">True if the article should be skipped if no changes are made</param>
        /// <param name="skipIfOnlyMinorChange"></param>
        public void PerformFindAndReplace(FindandReplace findAndReplace, SubstTemplates substTemplates,
                                          ReplaceSpecial.ReplaceSpecial replaceSpecial, bool skipIfNoChange, bool skipIfOnlyMinorChange)
        {
            if (!findAndReplace.HasReplacements && !replaceSpecial.HasRules && !substTemplates.HasSubstitutions)
                return;

            string strTemp = Tools.ConvertFromLocalLineEndings(mArticleText),
            testText = strTemp,
            tmpEditSummary = "";

            bool majorChangesMade;
            strTemp = findAndReplace.MultipleFindAndReplace(strTemp, Name, ref tmpEditSummary, out majorChangesMade);

            bool farMadeMajorChanges = (testText != strTemp && majorChangesMade);

            string strTemp2 = replaceSpecial.ApplyRules(strTemp, Name);

            strTemp2 = substTemplates.SubstituteTemplates(strTemp2, Name);

            if (!farMadeMajorChanges && strTemp2 != strTemp) //override minor changes if other replcements did something
                farMadeMajorChanges = true;

            if (testText == strTemp2)
            {
                if (skipIfNoChange)
                    Trace.AWBSkipped("No Find And Replace Changes");
                else
                    return; //No changes, so nothing to change in article text (but we're not skipping either)
            }
            else if (!farMadeMajorChanges && skipIfOnlyMinorChange)
            {
                Trace.AWBSkipped("Only minor Find And Replace Changes");
            }
            else
            {
                AWBChangeArticleText("Find and replace applied" + tmpEditSummary,
                                     Tools.ConvertToLocalLineEndings(strTemp2), true);
                AppendToSummary(tmpEditSummary);
            }
        }

        /// <summary>
        /// Fix spelling mistakes
        /// </summary>
        /// <param name="regexTypos">A RegExTypoFix object</param>
        /// <param name="skipIfNoChange">True if the article should be skipped if no changes are made</param>
        public void PerformTypoFixes(RegExTypoFix regexTypos, bool skipIfNoChange)
        {
            string tmpEditSummary;
            string strTemp = regexTypos.PerformTypoFixes(mArticleText, out noChange, out tmpEditSummary, Name);

            if (noChange && skipIfNoChange)
                Trace.AWBSkipped("No typo fixes");
            else if (!noChange)
            {
                AWBChangeArticleText(tmpEditSummary, strTemp, false);
                AppendToSummary(tmpEditSummary);
            }
        }

        /// <summary>
        /// "Auto tag" (Adds/removes wikify or stub tags if necessary)
        /// </summary>
        /// <param name="parsers">An initialised Parsers object</param>
        /// <param name="skipIfNoChange">True if the article should be skipped if no changes are made</param>
        /// <param name="restrictOrphanTagging"></param>
        public void AutoTag(Parsers parsers, bool skipIfNoChange, bool restrictOrphanTagging)
        {
            string tmpEditSummary = "";
            string strTemp = parsers.Tagger(mArticleText, Name, restrictOrphanTagging, out noChange, ref tmpEditSummary);

            if (skipIfNoChange && noChange)
                Trace.AWBSkipped("No Tag changed");
            else if (!noChange)
            {
                AWBChangeArticleText("Auto tagger changes applied" + tmpEditSummary, strTemp, false);
                AppendToSummary(tmpEditSummary);
            }
        }

        /// <summary>
        /// Fixes some crap
        /// </summary>
        /// <param name="langCode">The wiki's language code</param>
        /// <param name="skipIfNoChange">True if the article should be skipped if no changes are made</param>
        protected void MinorFixes(string langCode, bool skipIfNoChange)
        {
            AWBChangeArticleText("Fixed interwikis", Parsers.InterwikiConversions(mArticleText, out noChange), true);

            if (langCode == "en")
            {
                string strTemp = mArticleText;

                // do not subst on Template documentation pages
                if (!(Namespace.Determine(Name).Equals(Namespace.Template) && Name.EndsWith(@"/doc")))
                    strTemp = Parsers.Conversions(mArticleText);

                strTemp = Parsers.FixLivingThingsRelatedDates(strTemp);
                strTemp = Parsers.FixHeadings(strTemp, Name, out noChange);

                if (mArticleText == strTemp && skipIfNoChange)
                {
                    Trace.AWBSkipped("No header errors");
                }
                else if (!noChange)
                    AWBChangeArticleText("Fixed header errors", strTemp, true);
                else
                {
                    AWBChangeArticleText("Fixed minor formatting issues", strTemp, true);
                    if (skipIfNoChange) Trace.AWBSkipped("No header errors");
                }
            }
        }

        /// <summary>
        /// Sets Default Sort on Article if Necessary / clean diacritics
        /// </summary>
        /// <param name="langCode">The wiki's language code</param>
        /// <param name="skipIfNoChange">True if the article should be skipped if no changes are made</param>
        /// <param name="restrictDefaultsortAddition"></param>
        public void SetDefaultSort(string langCode, bool skipIfNoChange, bool restrictDefaultsortAddition)
        {
            if (langCode == "en" && Variables.IsWikimediaProject && !Variables.IsWikimediaMonolingualProject)
            {
                string strTemp = Parsers.ChangeToDefaultSort(mArticleText, Name, out noChange, restrictDefaultsortAddition);

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
        public void SetDefaultSort(string langCode, bool skipIfNoChange)
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
            bool noChange2;
            string strTemp = parsers.FixPeopleCategories(mArticleText, Name, true, out noChange);
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
            string strTemp = parsers.BoldTitle(mArticleText, Name, out noChange);
            if (skipIfNoChange && noChange)
                Trace.AWBSkipped("No Titles to embolden");
            else if (!noChange)
                AWBChangeArticleText("Emboldened titles", strTemp, false);
        }

        /// <summary>
        /// Invokes the Custom Module code
        /// </summary>
        /// <param name="module"></param>
        public void SendPageToCustomModule(IModule module)
        {
            IProcessArticleEventArgs processArticleEventArgs = this;
            string strEditSummary;
            bool skipArticle;

            string strTemp = module.ProcessArticle(processArticleEventArgs.ArticleText,
                                                   processArticleEventArgs.ArticleTitle, NameSpaceKey, out strEditSummary, out skipArticle);

            // take updated article text even if skip true, so that in re-parse mode updates are taken
            AWBChangeArticleText("Custom module", strTemp, true);
            
            if (!skipArticle)
            {
                processArticleEventArgs.EditSummary = strEditSummary;
                processArticleEventArgs.Skip = false;
                AppendPluginEditSummary();
            }
            else
                Trace.AWBSkipped("Skipped by custom module");
        }
        #endregion

        #region AWB worker functions
        /// <summary>
        /// Disambiguate
        /// </summary>
        /// <returns>True if OK to proceed, false to abort</returns>
        public bool Disambiguate(Session session, string dabLinkText, string[] dabVariantsLines, bool botMode, int context,
                                 bool skipIfNoChange)
        {
            Disambiguation.DabForm df = new Disambiguation.DabForm(session);
            string strTemp = df.Disambiguate(mArticleText, Name, dabLinkText,
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
            if (performsSignificantChanges && (newText != mArticleText))
                _generalFixesSignificantChange = true;

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
                AppendToSummary(mPluginEditSummary.Trim());
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
            return Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            Article a = obj as Article;
            if (a == null)
            {
                if (obj is string)
                    return Name == obj as string;

                return false;
            }
            return (this == a);
        }

        public bool Equals(Article a)
        {
            return (this == a);
        }

        public int CompareTo(Article other)
        {
            return string.Compare(Name, other.Name, false, System.Globalization.CultureInfo.InvariantCulture);
        }

        #endregion

        public static bool operator ==(Article a, Article b)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(a, b))
                return true;

            if ((object)a == null || (object)b == null)
                return false;

            return (a.Name == b.Name);
        }

        public static bool operator !=(Article a, Article b)
        {
            return !(a == b);
        }

        #region Interfaces

        //IMyTraceListener IProcessArticleEventArgs.AWBLogItem
        //{ get { return mAWBLogListener; } }

        string IProcessArticleEventArgs.ArticleTitle
        { get { return Name; } }

        string IProcessArticleEventArgs.EditSummary // this is temp edit summary field, sent from plugin
        { get { return mPluginEditSummary; } set { mPluginEditSummary = value.Trim(); } }

        bool IProcessArticleEventArgs.Skip
        { get { return mPluginSkip; } set { mPluginSkip = value; } }

        [XmlIgnore]
        public Exists Exists { get; protected set; }
        #endregion

        #region General fixes
        private bool _generalFixesCausedChange, _textAlreadyChanged, _generalFixesSignificantChange;
        private string _afterGeneralFixesArticleText;

        /// <summary>
        /// Performs numerous minor improvements to the page text
        /// </summary>
        /// <param name="parsers">A parser object</param>
        /// <param name="removeText"></param>
        /// <param name="skip">Skip options</param>
        /// <param name="replaceReferenceTags">If true, &lt;div class="references-small">&lt;references/>&lt;/div> and so on
        /// <param name="restrictDefaultsortAddition"></param>
        /// <param name="noMOSComplianceFixes"></param>
        /// will be replaced with {{reflist}}</param>
        /// <param name="restrictDefaultsortAddition"></param>
        /// <param name="noMOSComplianceFixes"></param>
        public void PerformGeneralFixes(Parsers parsers, HideText removeText, ISkipOptions skip, bool replaceReferenceTags, bool restrictDefaultsortAddition, bool noMOSComplianceFixes)
        { //TODO: 2009-01-28 review which of the genfixes below should be labelled 'significant'
            BeforeGeneralFixesTextChanged();

            // FixDates does its own hiding
            AWBChangeArticleText("Fix dates", parsers.FixDates(ArticleText), false);
            Variables.Profiler.Profile("FixDates");

            HideText(removeText);

            Variables.Profiler.Profile("HideText");

            // call this before MinorFixes so that Parsers.Conversions cleans up from ArticleIssues
            AWBChangeArticleText("Fixes for {{multiple issues}}", parsers.ArticleIssues(ArticleText), true);
            Variables.Profiler.Profile("ArticleIssues");

            MinorFixes(Variables.LangCode, skip.SkipNoHeaderError);
            Variables.Profiler.Profile("MinorFixes");

            FixPeopleCategories(parsers, skip.SkipNoPeopleCategoriesFixed);
            Variables.Profiler.Profile("FixPeopleCategories");

            SetDefaultSort(Variables.LangCode, skip.SkipNoDefaultSortAdded, restrictDefaultsortAddition);
            Variables.Profiler.Profile("SetDefaultSort");

            AWBChangeArticleText("Fix categories", Parsers.FixCategories(ArticleText), true);
            Variables.Profiler.Profile("FixCategories");

            AWBChangeArticleText("Fix images", Parsers.FixImages(ArticleText), true);
            Variables.Profiler.Profile("FixImages");

            AWBChangeArticleText("Fix whitespace in links", Parsers.FixLinkWhitespace(ArticleText, Name), true);
            Variables.Profiler.Profile("FixLinkWhitespace");

            // does significant fixes
            AWBChangeArticleText("Fix syntax", Parsers.FixSyntax(ArticleText), true, true);
            Variables.Profiler.Profile("FixSyntax");

            AWBChangeArticleText("Fix citation templates", Parsers.FixCitationTemplates(ArticleText), true, true);
            Variables.Profiler.Profile("FixCitationTemplates");

            AWBChangeArticleText("Fix temperatures", Parsers.FixTemperatures(ArticleText), true);
            Variables.Profiler.Profile("FixTemperatures");

            if (!noMOSComplianceFixes)
            {
                AWBChangeArticleText("Fix non-breaking spaces", parsers.FixNonBreakingSpaces(ArticleText), true);
                Variables.Profiler.Profile("FixNonBreakingSpaces");
            }

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

            AWBChangeArticleText("SameRefDifferentName", Parsers.SameRefDifferentName(ArticleText), true);
            Variables.Profiler.Profile("SameRefDifferentName");

            AWBChangeArticleText("ReorderReferences", Parsers.ReorderReferences(ArticleText), true);
            Variables.Profiler.Profile("ReorderReferences");

            AWBChangeArticleText("Fix empty references", Parsers.SimplifyReferenceTags(ArticleText), true);
            Variables.Profiler.Profile("FixEmptyReferences");
            
            AWBChangeArticleText("Refs after punctuation", Parsers.RefsAfterPunctuation(ArticleText), true);
            Variables.Profiler.Profile("RefsAfterPunctuation");

            // does significant fixes
            AWBChangeArticleText("Add missing {{reflist}}", Parsers.AddMissingReflist(ArticleText), true, true);
            Variables.Profiler.Profile("AddMissingReflist");

            CiteTemplateDates(parsers, skip.SkipNoCiteTemplateDatesFixed);
            Variables.Profiler.Profile("CiteTemplateDates");

            AWBChangeArticleText("Redirect tagger", Parsers.RedirectTagger(ArticleText, Name), false);
            Variables.Profiler.Profile("RedirectTagger");

            BulletExternalLinks(skip.SkipNoBulletedLink);
            Variables.Profiler.Profile("BulletExternalLinks");

            AWBChangeArticleText("Remove empty comments", Parsers.RemoveEmptyComments(ArticleText), false);
            Variables.Profiler.Profile("RemoveEmptyComments");

            if (!noMOSComplianceFixes)
            {
                AWBChangeArticleText("Mdashes", parsers.Mdashes(ArticleText, Name), true);
                Variables.Profiler.Profile("Mdashes");

                AWBChangeArticleText("Fix Date Ordinals/Of", parsers.FixDateOrdinalsAndOf(ArticleText, Name), true, true);
                Variables.Profiler.Profile("FixDateOrdinalsAndOf");
            }

            AWBChangeArticleText("PersonData", Parsers.PersonData(ArticleText, Name), false);
            Variables.Profiler.Profile("PersonData");
            
            // must call EmboldenTitles before calling FixLinks
            EmboldenTitles(parsers, skip.SkipNoBoldTitle);

            FixLinks(skip.SkipNoBadLink);
            Variables.Profiler.Profile("FixLinks");

            if (!Globals.UnitTestMode) // disable to avoid ssslow network requests
            {
                // pass unhidden text to MetaDataSorter so that it can allow for comments around persondata, categories etc.
                UnHideText(removeText);
                AWBChangeArticleText("Sort meta data",
                                     parsers.SortMetaData(ArticleText, Name), true);
                HideText(removeText);

                Variables.Profiler.Profile("Metadata");
            }

            AWBChangeArticleText("Simplify links", Parsers.SimplifyLinks(ArticleText), true);
            Variables.Profiler.Profile("SimplifyLinks");

            //AWBChangeArticleText("Remove duplicate wikilink", parsers.RemoveDuplicateWikiLinks(articleText), true);

            UnHideText(removeText);

            AfterGeneralFixesTextChanged();

            Variables.Profiler.Profile("End of general fixes");
        }
        
        /// <summary>
        /// Preforms general fixes suitable for all pages: trims whitespace
        /// </summary>
        public void PerformUniversalGeneralFixes()
        {
            mArticleText = mArticleText.Trim();
        }

        /// <summary>
        /// Calls the MetaDataSorter on the article if it is on mainspace
        /// </summary>
        /// <param name="parsers"></param>
        public void PerformMetaDataSort(Parsers parsers)
        {
            if (!Globals.UnitTestMode && NameSpaceKey == Namespace.Mainspace)
                mArticleText = parsers.SortMetaData(ArticleText, Name);
        }
        #endregion

        /// <summary>
        /// Check if the article text has already been changed when the code reaches this point
        /// </summary>
        private void BeforeGeneralFixesTextChanged()
        {
            _textAlreadyChanged = (ArticleText != OriginalArticleText);
        }

        /// <summary>
        /// If the text hasnt been changed prior to starting general fixes, see if the general fixes have made a change,
        /// if it has, make a copy of the article text post general fix changes
        /// </summary>
        private void AfterGeneralFixesTextChanged()
        {
            if (!_textAlreadyChanged)
            {
                _generalFixesCausedChange = (ArticleText != OriginalArticleText);

                if (_generalFixesCausedChange)
                    _afterGeneralFixesArticleText = ArticleText;
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
        /// Executes general fixes specific to article talk pages
        /// </summary>
        public void PerformTalkGeneralFixes()
        {
            BeforeGeneralFixesTextChanged();

            string articleText = ArticleText;
            TalkPageHeaders.ProcessTalkPage(ref articleText, DEFAULTSORT.NoChange);

            if (articleText != ArticleText)
            {
                AWBChangeArticleText("Talk Page general fixes", articleText, false);
            }

            AfterGeneralFixesTextChanged();
        }

        /// <summary>
        /// Returns <see langword="true"/> if the article is a redirect page
        /// </summary>
        [XmlIgnore]
        public bool IsRedirect
        {
            get { return Tools.IsRedirect(ArticleText); }
        }


        private static string _lastMove = "", _lastDelete = "", _lastProtect = "";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="newTitle"></param>
        /// <returns></returns>
        public bool Move(Session session, out string newTitle)
        {
            using (ArticleActionDialog dlgArticleAction = new ArticleActionDialog(ArticleAction.Move))
            {
                dlgArticleAction.NewTitle = Name;
                dlgArticleAction.Summary = _lastMove;

                if (dlgArticleAction.ShowDialog() == DialogResult.OK
                    && Name != dlgArticleAction.NewTitle)
                {
                    _lastMove = dlgArticleAction.Summary;
                    session.Editor.SynchronousEditor.Move(Name, dlgArticleAction.NewTitle,
                                                          ArticleActionSummary(dlgArticleAction), true /* probably wants dealing with on dialog*/,
                                                          dlgArticleAction.NoRedirect, dlgArticleAction.Watch);

                    newTitle = dlgArticleAction.NewTitle;

                    return true;
                }
                newTitle = "";
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public bool Delete(Session session)
        {
            using (ArticleActionDialog dlgArticleAction = new ArticleActionDialog(ArticleAction.Delete))
            {
                dlgArticleAction.Summary = _lastDelete;

                if (dlgArticleAction.ShowDialog() == DialogResult.OK)
                {
                    _lastDelete = dlgArticleAction.Summary;
                    session.Editor.SynchronousEditor.Delete(Name, ArticleActionSummary(dlgArticleAction), dlgArticleAction.Watch);

                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public bool Protect(Session session)
        {
            using (ArticleActionDialog dlgArticleAction = new ArticleActionDialog(ArticleAction.Protect))
            {
                dlgArticleAction.Summary = _lastProtect;

                dlgArticleAction.EditProtectionLevel = session.Page.EditProtection;
                dlgArticleAction.MoveProtectionLevel = session.Page.MoveProtection;

                if (dlgArticleAction.ShowDialog() == DialogResult.OK)
                {
                    _lastProtect = dlgArticleAction.Summary;
                    session.Editor.SynchronousEditor.Protect(Name,
                                                             ArticleActionSummary(dlgArticleAction),
                                                             dlgArticleAction.ProtectExpiry,
                                                             dlgArticleAction.EditProtectionLevel,
                                                             dlgArticleAction.MoveProtectionLevel,
                                                             dlgArticleAction.CascadingProtection,
                                                             dlgArticleAction.Watch);

                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Setting: Add "Using AWB" to the summary when deleting or protecting an article?
        /// </summary>
        public static bool AddUsingAWBOnArticleAction;

        private static string ArticleActionSummary(ArticleActionDialog dlgArticleAction)
        {
            return AddUsingAWBOnArticleAction
                ? dlgArticleAction.Summary + " (" + Variables.SummaryTag.Trim() + ")"
                : dlgArticleAction.Summary;
        }
    }
}