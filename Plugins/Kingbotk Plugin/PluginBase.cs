using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using WikiFunctions;

//Copyright © 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/
//Copyright © 2008 Sam Reed (Reedy) http://www.reedyboy.net/

//This program is free software; you can redistribute it and/or modify it under the terms of Version 2 of the GNU General Public License as published by the Free Software Foundation.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

//You should have received a copy of the GNU General Public License Version 2 along with this program; if not, write to the Free Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
using WikiFunctions.Lists.Providers;

namespace AutoWikiBrowser.Plugins.Kingbotk
{
    /// <summary>
    /// SDK Software's base class for template-manipulating AWB plugins
    /// </summary>
    internal abstract class PluginBase
    {
        // Settings:
        protected internal abstract string PluginShortName { get; }
        // we might want to parameterise this later
        protected const bool ForceAddition = true;

        protected virtual string ParameterBreak
        {
            get { return Environment.NewLine; }
        }

        protected internal abstract IGenericSettings GenericSettings { get; }

        internal abstract bool HasReqPhotoParam { get; }

        internal abstract void ReqPhoto();

        // Objects:
        protected ToolStripMenuItem OurMenuItem;
        protected Article article;

        protected Templating Template;
        // Regular expressions:
        protected Regex MainRegex;
        protected Regex SecondChanceRegex;
        protected Regex PreferredTemplateNameRegex;
        protected abstract string PreferredTemplateName { get; }

        protected static readonly Regex PreferredTemplateNameRegexCreator =
            new Regex("^(?<first>[a-zA-Z0-9]{1})(?<second>.*)", RegexOptions.Compiled);

        protected string PreferredTemplateNameWikiMatchEvaluator(Match match)
        {
            return "^[" + match.Groups["first"].Value.ToUpper() + match.Groups["first"].Value.ToLower() + "]" +
                   match.Groups["second"].Value + "$";
        }

        // AWB pass through:
        protected void InitialiseBase()
        {
            OurMenuItem.CheckOnClick = true;
            OurMenuItem.Checked = false;
            OurMenuItem.ToolTipText = "Enable/disable the " + PluginShortName + " plugin";
            OurMenuItem.CheckedChanged += ourmenuitem_CheckedChanged;
            PluginManager.AWBForm.PluginsToolStripMenuItem.DropDownItems.Add(OurMenuItem);
        }

        protected internal abstract void Initialise();

        protected internal abstract void ReadXML(XmlTextReader Reader);

        protected internal abstract void Reset();

        protected internal abstract void WriteXML(XmlTextWriter Writer);

        protected internal bool ProcessTalkPage(Article A, bool AddReqPhotoParm)
        {
            return ProcessTalkPage(A, Classification.Code, Importance.Code, false, false, false,
                ProcessTalkPageMode.Normal, AddReqPhotoParm);
        }

        protected internal bool ProcessTalkPage(Article A, Classification Classification, Importance Importance,
            bool ForceNeedsInfobox, bool ForceNeedsAttention, bool RemoveAutoStub,
            ProcessTalkPageMode ProcessTalkPageMode, bool AddReqPhotoParm)
        {
            bool BadTemplate = false;
            bool res = false;

            article = A;

            if (SkipIfContains())
            {
                A.PluginIHaveFinished(SkipResults.SkipRegex, PluginShortName);
            }
            else
            {
                // MAIN
                string OriginalArticleText = A.AlteredArticleText;

                Template = new Templating();
                A.AlteredArticleText = MainRegex.Replace(A.AlteredArticleText, MatchEvaluator);

                if (Template.BadTemplate)
                {
                    BadTemplate = true;
                }
                else if (Template.FoundTemplate)
                {
                    // Even if we've found a good template bizarrely the page could still contain a bad template too 
                    if (SecondChanceRegex.IsMatch(A.AlteredArticleText) || TemplateFound())
                    {
                        BadTemplate = true;
                    }
                }
                else
                {
                    if (SecondChanceRegex.IsMatch(OriginalArticleText))
                    {
                        BadTemplate = true;
                    }
                    else if (ForceAddition)
                    {
                        TemplateNotFound();
                    }
                }

                // OK, we're in business:
                res = true;
                if (HasReqPhotoParam && AddReqPhotoParm)
                {
                    ReqPhoto();
                }

                ProcessArticleFinish();
                if (ProcessTalkPageMode != ProcessTalkPageMode.Normal)
                {
                    ProcessArticleFinishNonStandardMode(Classification, Importance, ForceNeedsInfobox,
                        ForceNeedsAttention, RemoveAutoStub, ProcessTalkPageMode);
                }

                if (article.ProcessIt)
                {
                    TemplateWritingAndPlacement();
                }
                else
                {
                    A.AlteredArticleText = OriginalArticleText;
                    A.PluginIHaveFinished(SkipResults.SkipNoChange, PluginShortName);
                }
            }

            if (BadTemplate)
            {
                A.PluginIHaveFinished(SkipResults.SkipBadTag, PluginShortName);
                // TODO: We could get the template placeholder here
            }

            article = null;
            return res;
        }

        // Article processing:
        protected abstract bool SkipIfContains();

        /// <summary>
        /// Send the template to the plugin for preinspection
        /// </summary>
        /// <returns>False if OK, TRUE IF BAD TAG</returns>
        protected abstract bool TemplateFound();

        protected abstract void ProcessArticleFinish();

        protected abstract string WriteTemplateHeader();

        protected abstract void ImportanceParameter(Importance Importance);

        protected string MatchEvaluator(Match match)
        {
            if (match.Groups["parm"].Captures.Count != match.Groups["val"].Captures.Count)
            {
                Template.BadTemplate = true;
            }
            else
            {
                Template.FoundTemplate = true;
                article.PluginCheckTemplateCall(match.Groups["tl"].Value, PluginShortName);

                if (HasAlternateNames)
                    PluginCheckTemplateName(match.Groups["tlname"].Value);
                //.Trim)

                if (match.Groups["parm"].Captures.Count > 0)
                {
                    for (int i = 0; i <= match.Groups["parm"].Captures.Count - 1; i++)
                    {
                        string value = match.Groups["val"].Captures[i].Value;
                        string parm = match.Groups["parm"].Captures[i].Value;

                        Template.AddTemplateParmFromExistingTemplate(parm, value);
                    }
                }
            }

            return Constants.conTemplatePlaceholder;
        }

        protected void PluginCheckTemplateName(string TemplateName)
        {
            if (HasAlternateNames)
            {
                if (!PreferredTemplateNameRegex.Match(TemplateName).Success)
                {
                    article.RenamedATemplate(TemplateName, PreferredTemplateName, PluginShortName);
                    GotTemplateNotPreferredName(TemplateName);
                }
            }
        }

        protected virtual void GotTemplateNotPreferredName(string TemplateName)
        {
        }

        protected virtual void TemplateNotFound()
        {
            article.ArticleHasAMajorChange();
            if (article.Namespace == Namespace.Talk)
            {
                Template.NewTemplateParm("class", "");
            }
            article.TemplateAdded(PreferredTemplateName, PluginShortName);
        }

        private void TemplateWritingAndPlacement()
        {
            string TemplateHeader = WriteTemplateHeader();

            var _with2 = article;
            if (Template.FoundTemplate)
            {
                // Write it back where it was
                TemplateHeader += Template.ParametersToString(ParameterBreak);
                _with2.RestoreTemplateToPlaceholderSpot(TemplateHeader);
            }
            else
            {
                // Our template wasn't found, write it into a shell or to the top of the page
                _with2.PrependTemplateOrWriteIntoShell(Template, ParameterBreak, TemplateHeader);
            }
        }

        protected void AddAndLogNewParamWithAYesValue(string ParamName)
        {
            Template.NewOrReplaceTemplateParm(ParamName, "yes", article, true, false, PluginName: PluginShortName);
        }

        protected void AddNewParamWithAYesValue(string ParamName)
        {
            Template.NewOrReplaceTemplateParm(ParamName, "yes", article, false, false, PluginName: PluginShortName);
        }

        protected void AddAndLogNewParamWithAYesValue(string ParamName, string ParamAlternativeName)
        {
            Template.NewOrReplaceTemplateParm(ParamName, "yes", article, true, true,
                ParamAlternativeName: ParamAlternativeName, PluginName: PluginShortName);
        }

        protected void AddEmptyParam(string ParamName)
        {
            if (!Template.Parameters.ContainsKey(ParamName))
                Template.NewTemplateParm(ParamName, "", false, article, PluginShortName);
        }


        protected void ProcessArticleFinishNonStandardMode(Classification classification, Importance Importance,
            bool ForceNeedsInfobox, bool ForceNeedsAttention, bool RemoveAutoStub,
            ProcessTalkPageMode ProcessTalkPageMode)
        {
            if (article.Namespace == Namespace.Talk && classification == Classification.Unassessed)
            {
                Template.NewOrReplaceTemplateParm("class", classification.ToString(), article, false, false);
            }

            ImportanceParameter(Importance);

            if (ForceNeedsInfobox)
            {
                AddAndLogNewParamWithAYesValue("needs-infobox");
            }

            if (ForceNeedsAttention)
            {
                AddAndLogNewParamWithAYesValue("attention");
            }

            if (RemoveAutoStub)
            {
                var _with3 = article;
                if (Template.Parameters.ContainsKey("auto"))
                {
                    Template.Parameters.Remove("auto");
                    _with3.ArticleHasAMajorChange();
                }
            }
        }

        protected string WriteOutParameterToHeader(string ParamName)
        {
            string res = string.Empty;
            var _with4 = Template;
            if (_with4.Parameters.ContainsKey(ParamName))
            {
                res = "|" + ParamName + "=" + _with4.Parameters[ParamName].Value + ParameterBreak;
                _with4.Parameters.Remove(ParamName);
            }
            return res;
        }

        protected void StubClass()
        {
            if (article.Namespace == Namespace.Talk)
            {
                if (GenericSettings.StubClass)
                {
                    Template.NewOrReplaceTemplateParm("class", "Stub", article, true, false, PluginName: PluginShortName,
                        DontChangeIfSet: true);
                }

                if (GenericSettings.AutoStub &&
                    Template.NewOrReplaceTemplateParm("class", "Stub", article, true, false, PluginName: PluginShortName,
                        DontChangeIfSet: true))
                    AddAndLogNewParamWithAYesValue("auto");
                // If add class=Stub (we don't change if set) add auto
            }
        }

        protected void ReplaceATemplateWithAYesParameter(Regex R, string ParamName, string TemplateCall,
            bool Replace = true)
        {
            var _with5 = article;
            if ((R.Matches(_with5.AlteredArticleText).Count > 0))
            {
                if (Replace)
                    _with5.AlteredArticleText = R.Replace(_with5.AlteredArticleText, "");
                _with5.DoneReplacement(TemplateCall, ParamName + "=yes", true, PluginShortName);
                Template.NewOrReplaceTemplateParm(ParamName, "yes", article, false, false);
                _with5.ArticleHasAMinorChange();
            }
        }

        /// <summary>
        /// Checks if params which have two names (V8, v8) exist under both names
        /// </summary>
        /// <returns>True if BAD TAG</returns>
        protected bool CheckForDoublyNamedParameters(string Name1, string Name2)
        {
            var _with6 = Template.Parameters;
            if (_with6.ContainsKey(Name1) && _with6.ContainsKey(Name2))
            {
                if (_with6[Name1].Value == _with6[Name2].Value)
                {
                    _with6.Remove(Name2);
                    article.DoneReplacement(Name2, "", true, PluginShortName);
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        // Interraction with manager:
        internal bool Enabled
        {
            get { return OurMenuItem.Checked; }
            set
            {
                OurMenuItem.Checked = value;
                ShowHideOurObjects(value);
                PluginManager.PluginEnabledStateChanged(this, value);
            }
        }

        protected internal virtual void BotModeChanged(bool BotMode)
        {
            if (BotMode && GenericSettings.StubClass)
            {
                GenericSettings.AutoStub = true;
                GenericSettings.StubClass = false;
            }
            GenericSettings.StubClassModeAllowed = !BotMode;
        }

        protected internal virtual bool IAmGeneric
        {
            get { return false; }
        }

        // User interface:
        protected abstract void ShowHideOurObjects(bool Visible);

        // Event handlers:
        private void ourmenuitem_CheckedChanged(object sender, EventArgs e)
        {
            Enabled = OurMenuItem.Checked;
        }

        internal PluginBase(bool IAmAGenericTemplate)
        {
            if (!IAmAGenericTemplate)
                throw new NotSupportedException();
        }

        // Redirects:
        // SPACES SHOULD BE WRITTEN TO XML AND IN THE GENERIC TL ALTERNATE NAME TEXT BOX AS SPACES ONLY
        // WHEN READ FROM XML, FROM WIKIPEDIA OR FROM THE TEXT BOX AND FED INTO REGEXES CONVERT THEM TO [ _]
        // Should contain spaces not [ _]. We always try to use an up-to-date list from the server, but we can at user's choice fall back to a recent list (generally from XML settings) at user's bidding
        protected string mLastKnownGoodRedirects = "";

        internal PluginBase(string DefaultRegexpmiddle)
        {
            GotNewAlternateNamesString(DefaultRegexpmiddle);
        }

        // Properties and regex construction:

        /// <summary>
        /// Called when we have a new Redirects list (at startup, from Wikipedia, or from the user in the case of generic templates)
        /// </summary>
        /// <param name="AlternateNames"></param>
        /// <param name="SenderIsGenericTemplateForm"></param>
        /// <remarks></remarks>
        protected void GotNewAlternateNamesString(string AlternateNames, bool SenderIsGenericTemplateForm = false)
        {
            string RegexpMiddle;
            // Less efficient to transfer to a new string but makes code easier to understand
            bool mHasAlternateNames;

            // Argument should NOT contain the default name at this stage; should contain spaces not [ _] or _
            if (SenderIsGenericTemplateForm)
            {
                AlternateNames = AlternateNames.Replace("_", " ");
            }

            AlternateNames = AlternateNames.Trim();

            if (string.IsNullOrEmpty(AlternateNames))
            {
                mHasAlternateNames = false;
                RegexpMiddle = PreferredTemplateName;
            }
            else
            {
                mHasAlternateNames = true;
                mLastKnownGoodRedirects = AlternateNames;
                RegexpMiddle = PreferredTemplateName + "|" + AlternateNames;
            }
            RegexpMiddle = RegexpMiddle.Replace(" ", "[ _]");

            MainRegex = new Regex(Constants.conRegexpLeft + RegexpMiddle + Constants.conRegexpRight,
                Constants.conRegexpOptions);
            SecondChanceRegex = new Regex(Constants.conRegexpLeft + RegexpMiddle + Constants.conRegexpRightNotStrict,
                Constants.conRegexpOptions);

            PreferredTemplateNameRegex = mHasAlternateNames
                ? new Regex(
                    PreferredTemplateNameRegexCreator.Replace(PreferredTemplateName,
                        PreferredTemplateNameWikiMatchEvaluator),
                    RegexOptions.Compiled)
                : null;
        }

        /// <summary>
        /// Do any redirects point to the template?
        /// </summary>
        /// <returns>True if the template has alternate names</returns>
        protected bool HasAlternateNames
        {
            get { return ((PreferredTemplateNameRegex != null)); }
        }

        /// <summary>
        /// Returns true if the templating plugin is ready to start processing
        /// </summary>
        protected internal virtual bool IAmReady
        {
            // In compiled templates, this is where we check if we've got an up-to-date redirects list from Wikipedia
            // In generic templates, we also check whether the generic template has enough configuration to start tagging
            get { return true; }
        }


        private static readonly RedirectsListProvider rlp = new RedirectsListProvider();
        // Get the redirects from Wikipedia:
        /// <summary>
        /// Load the redirects for a template from Wikipedia
        /// </summary>
        /// <param name="Target">Template name</param>
        protected static List<WikiFunctions.Article> GetRedirects(string Target)
        {
            string message = "Loading redirects for Template:" + Target;

            PluginManager.StatusText.Text = message;
            Application.DoEvents();
            // the statusbar text wasn't updating without this; if happens elsewhere may need to write a small subroutine

            try
            {
                return rlp.MakeList(Namespace.Template, new[] {Variables.Namespaces[Namespace.Template] + Target});
            }
            finally
            {
                PluginManager.DefaultStatusText();
            }
        }

        protected static string ConvertRedirectsToString(List<WikiFunctions.Article> Redirects)
        {
            string res = "";

            foreach (WikiFunctions.Article redirect in Redirects)
            {
                if (redirect.NameSpaceKey == Namespace.Template)
                {
                    res += redirect.Name.Remove(0, 9) + "|";
                }
            }

            return res.Trim(new[] {Convert.ToChar("|")});
            // would .Remove be quicker? or declaring this as static?
        }

        // XML:
        internal virtual void ReadXMLRedirects(XmlTextReader Reader)
        {
            // For compiled template plugins, a Redirect string read in from XML is for backup use only if getting from WP fails
            // Generic templates already support AlternateNames property so will override this
            string Redirs = PluginManager.XMLReadString(Reader, RedirectsParm, mLastKnownGoodRedirects);

            if (!string.IsNullOrEmpty(Redirs))
                mLastKnownGoodRedirects = Redirs;
        }

        internal virtual void WriteXMLRedirects(XmlTextWriter Writer)
        {
            if (!string.IsNullOrEmpty(mLastKnownGoodRedirects))
                Writer.WriteAttributeString(RedirectsParm, mLastKnownGoodRedirects);
        }

        protected string RedirectsParm
        {
            get { return PreferredTemplateName.Replace(" ", "") + "RedirN"; }
        }
    }

    internal interface IGenericSettings
    {
        bool AutoStub { get; set; }
        bool StubClass { get; set; }
        bool StubClassModeAllowed { set; }
        void ReadXML(XmlTextReader Reader);
        void WriteXML(XmlTextWriter Writer);
        void XMLReset();
    }
}