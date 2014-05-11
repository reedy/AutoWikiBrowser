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
        protected Article TheArticle;

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
            OurMenuItem.ToolTipText = string.Format("Enable/disable the {0} plugin", PluginShortName);
            OurMenuItem.CheckedChanged += ourmenuitem_CheckedChanged;
            PluginManager.AWBForm.PluginsToolStripMenuItem.DropDownItems.Add(OurMenuItem);
        }

        protected internal abstract void Initialise();

        protected internal abstract void ReadXML(XmlTextReader reader);

        protected internal abstract void Reset();

        protected internal abstract void WriteXML(XmlTextWriter writer);

        protected internal bool ProcessTalkPage(Article article, bool addReqPhotoParm)
        {
            return ProcessTalkPage(article, Classification.Code, Importance.Code, false, false, false,
                ProcessTalkPageMode.Normal, addReqPhotoParm);
        }

        protected internal bool ProcessTalkPage(Article article, Classification classification, Importance importance,
            bool forceNeedsInfobox, bool forceNeedsAttention, bool removeAutoStub,
            ProcessTalkPageMode processTalkPageMode, bool addReqPhotoParm)
        {
            bool badTemplate = false;
            bool res = false;

            TheArticle = article;

            if (SkipIfContains())
            {
                article.PluginIHaveFinished(SkipResults.SkipRegex, PluginShortName);
            }
            else
            {
                // MAIN
                string originalArticleText = article.AlteredArticleText;

                Template = new Templating();
                article.AlteredArticleText = MainRegex.Replace(article.AlteredArticleText, MatchEvaluator);

                if (Template.BadTemplate)
                {
                    badTemplate = true;
                }
                else if (Template.FoundTemplate)
                {
                    // Even if we've found a good template bizarrely the page could still contain a bad template too 
                    if (SecondChanceRegex.IsMatch(article.AlteredArticleText) || TemplateFound())
                    {
                        badTemplate = true;
                    }
                }
                else
                {
                    if (SecondChanceRegex.IsMatch(originalArticleText))
                    {
                        badTemplate = true;
                    }
                    else if (ForceAddition)
                    {
                        TemplateNotFound();
                    }
                }

                // OK, we're in business:
                res = true;
                if (HasReqPhotoParam && addReqPhotoParm)
                {
                    ReqPhoto();
                }

                ProcessArticleFinish();
                if (processTalkPageMode != ProcessTalkPageMode.Normal)
                {
                    ProcessArticleFinishNonStandardMode(classification, importance, forceNeedsInfobox,
                        forceNeedsAttention, removeAutoStub, processTalkPageMode);
                }

                if (TheArticle.ProcessIt)
                {
                    TemplateWritingAndPlacement();
                }
                else
                {
                    article.AlteredArticleText = originalArticleText;
                    article.PluginIHaveFinished(SkipResults.SkipNoChange, PluginShortName);
                }
            }

            if (badTemplate)
            {
                article.PluginIHaveFinished(SkipResults.SkipBadTag, PluginShortName);
                // TODO: We could get the template placeholder here
            }

            TheArticle = null;
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

        protected virtual void ImportanceParameter(Importance importance)
        {
        }

        protected string MatchEvaluator(Match match)
        {
            if (match.Groups["parm"].Captures.Count != match.Groups["val"].Captures.Count)
            {
                Template.BadTemplate = true;
            }
            else
            {
                Template.FoundTemplate = true;
                TheArticle.PluginCheckTemplateCall(match.Groups["tl"].Value, PluginShortName);

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

            return Constants.TemplaterPlaceholder;
        }

        protected void PluginCheckTemplateName(string templateName)
        {
            if (HasAlternateNames)
            {
                if (!PreferredTemplateNameRegex.Match(templateName).Success)
                {
                    TheArticle.RenamedATemplate(templateName, PreferredTemplateName);
                    GotTemplateNotPreferredName(templateName);
                }
            }
        }

        protected virtual void GotTemplateNotPreferredName(string templateName)
        {
        }

        protected virtual void TemplateNotFound()
        {
            TheArticle.ArticleHasAMajorChange();
            if (TheArticle.Namespace == Namespace.Talk)
            {
                Template.NewTemplateParm("class", "");
            }
            TheArticle.TemplateAdded(PreferredTemplateName);
        }

        private void TemplateWritingAndPlacement()
        {
            string templateHeader = WriteTemplateHeader();

            if (Template.FoundTemplate)
            {
                // Write it back where it was
                templateHeader += Template.ParametersToString(ParameterBreak);
                TheArticle.RestoreTemplateToPlaceholderSpot(templateHeader);
            }
            else
            {
                // Our template wasn't found, write it to the top of the page
                TheArticle.PrependTemplate(Template, ParameterBreak, templateHeader);
            }
        }

        protected void AddAndLogNewParamWithAYesValue(string paramName)
        {
            Template.NewOrReplaceTemplateParm(paramName, "yes", TheArticle, true, false, pluginName: PluginShortName);
        }

        protected void AddNewParamWithAYesValue(string paramName)
        {
            Template.NewOrReplaceTemplateParm(paramName, "yes", TheArticle, false, false, pluginName: PluginShortName);
        }

        protected void AddAndLogNewParamWithAYesValue(string paramName, string paramAlternativeName)
        {
            Template.NewOrReplaceTemplateParm(paramName, "yes", TheArticle, true, true,
                paramAlternativeName: paramAlternativeName, pluginName: PluginShortName);
        }

        protected void AddEmptyParam(string paramName)
        {
            if (!Template.Parameters.ContainsKey(paramName))
                Template.NewTemplateParm(paramName, "", false, TheArticle, PluginShortName);
        }


        protected void ProcessArticleFinishNonStandardMode(Classification classification, Importance importance,
            bool forceNeedsInfobox, bool forceNeedsAttention, bool removeAutoStub,
            ProcessTalkPageMode processTalkPageMode)
        {
            if (TheArticle.Namespace == Namespace.Talk && classification != Classification.Unassessed)
            {
                Template.NewOrReplaceTemplateParm("class", classification.ToString(), TheArticle, false, false);
            }

            ImportanceParameter(importance);

            if (forceNeedsInfobox)
            {
                AddAndLogNewParamWithAYesValue("needs-infobox");
            }

            if (forceNeedsAttention)
            {
                AddAndLogNewParamWithAYesValue("attention");
            }

            if (removeAutoStub && Template.Parameters.ContainsKey("auto"))
            {
                Template.Parameters.Remove("auto");
                TheArticle.ArticleHasAMajorChange();
            }
        }

        protected string WriteOutParameterToHeader(string paramName)
        {
            string res = string.Empty;
            if (Template.Parameters.ContainsKey(paramName))
            {
                res = "|" + paramName + "=" + Template.Parameters[paramName].Value + ParameterBreak;
                Template.Parameters.Remove(paramName);
            }
            return res;
        }

        protected void StubClass()
        {
            if (TheArticle.Namespace == Namespace.Talk)
            {
                if (GenericSettings.StubClass)
                {
                    Template.NewOrReplaceTemplateParm("class", "Stub", TheArticle, true, false, pluginName: PluginShortName,
                        dontChangeIfSet: true);
                }

                if (GenericSettings.AutoStub &&
                    Template.NewOrReplaceTemplateParm("class", "Stub", TheArticle, true, false, pluginName: PluginShortName,
                        dontChangeIfSet: true))
                    AddAndLogNewParamWithAYesValue("auto");
                // If add class=Stub (we don't change if set) add auto
            }
        }

        protected void ReplaceATemplateWithAYesParameter(Regex r, string paramName, string templateCall,
            bool replace = true)
        {
            if ((r.Matches(TheArticle.AlteredArticleText).Count > 0))
            {
                if (replace)
                {
                    TheArticle.AlteredArticleText = r.Replace(TheArticle.AlteredArticleText, "");
                }
                TheArticle.DoneReplacement(templateCall, paramName + "=yes");
                Template.NewOrReplaceTemplateParm(paramName, "yes", TheArticle, false, false);
                TheArticle.ArticleHasAMinorChange();
            }
        }

        /// <summary>
        /// Checks if params which have two names (V8, v8) exist under both names
        /// </summary>
        /// <returns>True if BAD TAG</returns>
        protected bool CheckForDoublyNamedParameters(string name1, string name2)
        {
            var parameters = Template.Parameters;
            if (parameters.ContainsKey(name1) && parameters.ContainsKey(name2))
            {
                if (parameters[name1].Value == parameters[name2].Value)
                {
                    parameters.Remove(name2);
                    TheArticle.DoneReplacement(name2, "");
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

        protected internal virtual void BotModeChanged(bool botMode)
        {
            if (botMode && GenericSettings.StubClass)
            {
                GenericSettings.AutoStub = true;
                GenericSettings.StubClass = false;
            }
            GenericSettings.StubClassModeAllowed = !botMode;
        }

        protected internal virtual bool AmGeneric
        {
            get { return false; }
        }

        // User interface:
        protected abstract void ShowHideOurObjects(bool visible);

        // Event handlers:
        private void ourmenuitem_CheckedChanged(object sender, EventArgs e)
        {
            Enabled = OurMenuItem.Checked;
        }

        internal PluginBase(bool iamAGenericTemplate)
        {
            if (!iamAGenericTemplate)
                throw new NotSupportedException();
        }

        // Redirects:
        // SPACES SHOULD BE WRITTEN TO XML AND IN THE GENERIC TL ALTERNATE NAME TEXT BOX AS SPACES ONLY
        // WHEN READ FROM XML, FROM WIKIPEDIA OR FROM THE TEXT BOX AND FED INTO REGEXES CONVERT THEM TO [ _]
        // Should contain spaces not [ _]. We always try to use an up-to-date list from the server, but we can at user's choice fall back to a recent list (generally from XML settings) at user's bidding
        protected string LastKnownGoodRedirects = "";

        internal PluginBase(string defaultRegexpmiddle)
        {
            GotNewAlternateNamesString(defaultRegexpmiddle);
        }

        // Properties and regex construction:

        /// <summary>
        /// Called when we have a new Redirects list (at startup, from Wikipedia, or from the user in the case of generic templates)
        /// </summary>
        /// <param name="alternateNames"></param>
        /// <param name="senderIsGenericTemplateForm"></param>
        protected void GotNewAlternateNamesString(string alternateNames, bool senderIsGenericTemplateForm = false)
        {
            string regexpMiddle;
            // Less efficient to transfer to a new string but makes code easier to understand
            bool mHasAlternateNames;

            // Argument should NOT contain the default name at this stage; should contain spaces not [ _] or _
            if (senderIsGenericTemplateForm)
            {
                alternateNames = alternateNames.Replace("_", " ");
            }

            alternateNames = alternateNames.Trim();

            if (string.IsNullOrEmpty(alternateNames))
            {
                mHasAlternateNames = false;
                regexpMiddle = Regex.Escape(PreferredTemplateName);
            }
            else
            {
                mHasAlternateNames = true;
                LastKnownGoodRedirects = alternateNames;
                regexpMiddle = Regex.Escape(PreferredTemplateName) + "|" + Regex.Escape(alternateNames);
            }
            regexpMiddle = regexpMiddle.Replace(" ", "[ _]");

            MainRegex = new Regex(Constants.RegexpLeft + regexpMiddle + Constants.RegexpRight,
                Constants.RegexpOptions);
            SecondChanceRegex = new Regex(Constants.RegexpLeft + regexpMiddle + Constants.RegexpRightNotStrict,
                Constants.RegexpOptions);

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
        protected internal virtual bool AmReady
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
        /// <param name="target">Template name</param>
        protected static List<WikiFunctions.Article> GetRedirects(string target)
        {
            string message = "Loading redirects for Template:" + target;

            PluginManager.StatusText.Text = message;
            Application.DoEvents();
            // the statusbar text wasn't updating without this; if happens elsewhere may need to write a small subroutine

            try
            {
                return rlp.MakeList(Namespace.Template, new[] {Variables.Namespaces[Namespace.Template] + target});
            }
            finally
            {
                PluginManager.DefaultStatusText();
            }
        }

        protected static string ConvertRedirectsToString(List<WikiFunctions.Article> redirects)
        {
            string res = "";

            foreach (WikiFunctions.Article redirect in redirects)
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
        internal virtual void ReadXMLRedirects(XmlTextReader reader)
        {
            // For compiled template plugins, a Redirect string read in from XML is for backup use only if getting from WP fails
            // Generic templates already support AlternateNames property so will override this
            string redirs = PluginManager.XMLReadString(reader, RedirectsParm, LastKnownGoodRedirects);

            if (!string.IsNullOrEmpty(redirs))
                LastKnownGoodRedirects = redirs;
        }

        internal virtual void WriteXMLRedirects(XmlTextWriter writer)
        {
            if (!string.IsNullOrEmpty(LastKnownGoodRedirects))
                writer.WriteAttributeString(RedirectsParm, LastKnownGoodRedirects);
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
        void ReadXML(XmlTextReader reader);
        void WriteXML(XmlTextWriter writer);
        void XMLReset();
    }
}