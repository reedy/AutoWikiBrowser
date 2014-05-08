using System;
using System.Text.RegularExpressions;
using WikiFunctions;

//Copyright © 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/
//Copyright © 2008 Sam Reed (Reedy) http://www.reedyboy.net/

//This program is free software; you can redistribute it and/or modify it under the terms of Version 2 of the GNU General Public License as published by the Free Software Foundation.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

//You should have received a copy of the GNU General Public License Version 2 along with this program; if not, write to the Free Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

namespace AutoWikiBrowser.Plugins.Kingbotk
{
    /// <summary>
    /// An object representing an article which may or may not contain the targetted template
    /// </summary>
    internal sealed class Article
    {
        // Properties:
        private readonly string _fullArticleTitle;

        // Plugin-state:
        private SkipResults _skipResults = SkipResults.NotSet;

        // gets set by ArticleHasAMajorChange/ArticleHasAMinorChange
        private bool _processIt;

        // New:
        internal Article(string articleText, string fullArticleTitle, int vNamespace)
        {
            AlteredArticleText = articleText;
            _fullArticleTitle = fullArticleTitle;
            Namespace = vNamespace;
        }

        // Friend properties:
        internal string AlteredArticleText { get; set; }

        internal string FullArticleTitle
        {
            get { return _fullArticleTitle; }
        }

        internal int Namespace { get; private set; }

        internal string EditSummary { get; set; }

        internal void ArticleHasAMinorChange()
        {
            _processIt = true;
        }

        internal void ArticleHasAMajorChange()
        {
            _processIt = true;
        }

        internal bool ProcessIt
        {
            get { return _processIt; }
        }

        // For calling by plugin:
        internal void PluginCheckTemplateCall(string templateCall, string pluginName)
        {
            // we have "template:"
            if (!string.IsNullOrEmpty(templateCall))
            {
                _processIt = true;
            }
        }

        internal void PluginIHaveFinished(SkipResults result, string pluginName)
        {
            switch (result)
            {
                case SkipResults.SkipBadTag:
                    _skipResults = SkipResults.SkipBadTag;
                    PluginManager.AWBForm.TraceManager.SkippedArticleBadTag(pluginName, _fullArticleTitle, Namespace);
                    break;
                case SkipResults.SkipRegex:
                    if (_skipResults == SkipResults.NotSet)
                        _skipResults = SkipResults.SkipRegex;
                    PluginManager.AWBForm.TraceManager.SkippedArticle(pluginName, "Article text matched skip regex");
                    break;
                case SkipResults.SkipNoChange:
                    PluginManager.AWBForm.TraceManager.SkippedArticle(pluginName, "No change");
                    _skipResults = SkipResults.SkipNoChange;
                    break;
            }
        }

        // For calling by manager:
        internal SkipResults PluginManagerGetSkipResults
        {
            get { return _skipResults; }
        }

        internal void FinaliseEditSummary()
        {
            EditSummary = Regex.Replace(EditSummary, ", $", "");
        }

        // General article writing and manipulation:
        internal void RenamedATemplate(string oldName, string newName)
        {
            DoneReplacement(oldName, newName);
        }

        internal void DoneReplacement(string old, string replacement)
        {
            _processIt = true;
            EditSummary += old + "→" + replacement + ", ";
        }

        internal void TemplateAdded(string template)
        {
            EditSummary += string.Format("Added {{{{[[Template:{0}|{0}]]}}}}, ", template);
            ArticleHasAMajorChange();
        }

        internal void ParameterAdded(string paramName, string paramValue, bool minorEdit = false)
        {
            EditSummary += paramName + "=" + paramValue + ", ";

            if (minorEdit)
                ArticleHasAMinorChange();
            else
                ArticleHasAMajorChange();
        }

        // FIXME: To be replaced
        private readonly Microsoft.VisualBasic.CompilerServices.StaticLocalInitFlag
            static_RestoreTemplateToPlaceholderSpot_strPlaceholder_Init =
                new Microsoft.VisualBasic.CompilerServices.StaticLocalInitFlag();

        // just write one instance of template even if have multiple conTemplatePlaceholder's
        private string static_RestoreTemplateToPlaceholderSpot_strPlaceholder;

        private readonly Microsoft.VisualBasic.CompilerServices.StaticLocalInitFlag
            static_RestoreTemplateToPlaceholderSpot_RestoreTemplateToPlaceholderSpotRegex_Init =
                new Microsoft.VisualBasic.CompilerServices.StaticLocalInitFlag();

        private Regex static_RestoreTemplateToPlaceholderSpot_RestoreTemplateToPlaceholderSpotRegex;

        internal void RestoreTemplateToPlaceholderSpot(string TemplateHeader)
        {
            lock (static_RestoreTemplateToPlaceholderSpot_strPlaceholder_Init)
            {
                try
                {
                    if (InitStaticVariableHelper(static_RestoreTemplateToPlaceholderSpot_strPlaceholder_Init))
                    {
                        static_RestoreTemplateToPlaceholderSpot_strPlaceholder =
                            Regex.Escape(Constants.conTemplatePlaceholder);
                    }
                }
                finally
                {
                    static_RestoreTemplateToPlaceholderSpot_strPlaceholder_Init.State = 1;
                }
            }
            lock (static_RestoreTemplateToPlaceholderSpot_RestoreTemplateToPlaceholderSpotRegex_Init)
            {
                try
                {
                    if (
                        InitStaticVariableHelper(
                            static_RestoreTemplateToPlaceholderSpot_RestoreTemplateToPlaceholderSpotRegex_Init))
                    {
                        static_RestoreTemplateToPlaceholderSpot_RestoreTemplateToPlaceholderSpotRegex =
                            new Regex(static_RestoreTemplateToPlaceholderSpot_strPlaceholder);
                    }
                }
                finally
                {
                    static_RestoreTemplateToPlaceholderSpot_RestoreTemplateToPlaceholderSpotRegex_Init.State = 1;
                }
            }

            AlteredArticleText =
                static_RestoreTemplateToPlaceholderSpot_RestoreTemplateToPlaceholderSpotRegex.Replace(
                    AlteredArticleText, TemplateHeader, 1);
            AlteredArticleText =
                static_RestoreTemplateToPlaceholderSpot_RestoreTemplateToPlaceholderSpotRegex.Replace(
                    AlteredArticleText, "");
        }

        internal void EditInBrowser()
        {
            Tools.EditArticleInBrowser(FullArticleTitle);
        }

        // Enum:
        private enum BannerShellsEnum
        {
            NotChecked,
            NoneFound,
            FoundWikiProjectBannerShell
        }

        //State:
        private BannerShellsEnum _weFoundBannerShells;

        private string _matchEvaluatorString;
        // Regexes:
        // These could probably be simplified significantly (and extra logic doing things like removing linebreaks) if I learnt more of the magic characters

        private static readonly Regex WikiProjectBannerShellRegex =
            new Regex(
                Constants.RegexpLeft + WikiProjectBannerShell +
                ")\\b\\s*(?<start>\\|[^1]*=.*?)*\\s*\\|\\s*1\\s*=\\s*(?<body>.*}}[^{]*?)\\s*(?<end>\\|[^{]*)?\\s*}}",
                RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.ExplicitCapture);

        internal static readonly Regex LineBreakRegex = new Regex("[\\n\\r]*");

        private static readonly Regex DoubleLineBreakRegex = new Regex("[\\n\\r]{2,}");
        // Regex constant strings:
        // IGNORE CASE
        private const string WikiProjectBannerShell =
            "WikiProject ?Banner ?Shell|WP?BS|WPBannerShell|WikiProject ?Banners|WPB";

        // Match evaluators:
        private string WPBSRegexMatchEvaluator(Match match)
        {
            const string templatename = "WikiProjectBannerShell";
            string ending = match.Groups["start"].Value + match.Groups["end"].Value;

            ShellTemplateMatchEvaluatorsCommonTasks(templatename, match);

            if (!string.IsNullOrEmpty(ending))
                ending = Environment.NewLine + ending;

            return
                DoubleLineBreakRegex.Replace(
                    "{{" + templatename + "|1=" + Environment.NewLine + LineBreakRegex.Replace(_matchEvaluatorString, "") +
                    Environment.NewLine + match.Groups["body"].Value + ending + "}}", Environment.NewLine);
        }

        private void ShellTemplateMatchEvaluatorsCommonTasks(string templatename, Match match)
        {
            // Does the shell contain template: ?
            PluginCheckTemplateCall(match.Groups["tl"].Value, templatename);
            // Does the template have it's primary name:
            if (match.Groups["tlname"].Value != templatename)
            {
                RenamedATemplate(match.Groups["tlname"].Value, templatename);
            }
        }

        // Where we (possibly) add our template to an existing shell:
        internal void PrependTemplateOrWriteIntoShell(Templating template, string parameterBreak, string text)
        {
            if (_weFoundBannerShells == BannerShellsEnum.NotChecked)
            {
                _weFoundBannerShells = WikiProjectBannerShellRegex.IsMatch(AlteredArticleText)
                    ? BannerShellsEnum.FoundWikiProjectBannerShell
                    : BannerShellsEnum.NoneFound;
            }

            text += template.ParametersToString(parameterBreak);

            switch (_weFoundBannerShells)
            {
                case BannerShellsEnum.FoundWikiProjectBannerShell:
                    _matchEvaluatorString = text;

                    AlteredArticleText = WikiProjectBannerShellRegex.Replace(AlteredArticleText, WPBSRegexMatchEvaluator,
                        1);
                    _matchEvaluatorString = null;
                    break;
                case BannerShellsEnum.NoneFound:
                    AlteredArticleText = text + AlteredArticleText;
                    break;
            }
        }

        // Misc:
        internal bool PageContainsShellTemplate()
        {
            // Currently only WPBio can possibly call this, so it's ok to just run the regex and not cache the results
            // Later on we want to have dynamic redirects and management of these templates (or it maybe should be in
            // WikiFunctions.TalkPages
            return WikiProjectBannerShellRegex.IsMatch(AlteredArticleText);
        }

        private static bool InitStaticVariableHelper(Microsoft.VisualBasic.CompilerServices.StaticLocalInitFlag flag)
        {
            if (flag.State == 0)
            {
                flag.State = 2;
                return true;
            }
            if (flag.State == 2)
            {
                throw new Microsoft.VisualBasic.CompilerServices.IncompleteInitialization();
            }
            return false;
        }
    }
}