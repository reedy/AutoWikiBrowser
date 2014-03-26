using AutoWikiBrowser.Plugins.Kingbotk;
using AutoWikiBrowser.Plugins.Kingbotk.Components;
using AutoWikiBrowser.Plugins.Kingbotk.ManualAssessments;
using AutoWikiBrowser.Plugins.Kingbotk.Plugins;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using WikiFunctions;
using WikiFunctions.Plugin;
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
		private string mArticleText;
		private readonly string mFullArticleTitle;
		private readonly int mNamespace;

		private string mEditSummary = Constants.conWikiPluginBrackets;
		// Plugin-state:
		private SkipResults mSkipResults = SkipResults.NotSet;
			// gets set by ArticleHasAMajorChange/ArticleHasAMinorChange
		private bool mProcessIt;

		// New:
		internal Article(string ArticleText, string vFullArticleTitle, int vNamespace)
		{
			mArticleText = ArticleText;
			mFullArticleTitle = vFullArticleTitle;
			mNamespace = vNamespace;
			//mFullArticleTitle = GetArticleName(mNamespace, mArticleTitle)
		}

		// Friend properties:
		internal string AlteredArticleText {
			get { return mArticleText; }
			set { mArticleText = value; }
		}
		internal string FullArticleTitle {
			get { return mFullArticleTitle; }
		}
		internal int Namespace {
			get { return mNamespace; }
		}
		internal string EditSummary {
			get { return mEditSummary; }
			set { mEditSummary = value; }
		}
		internal void ArticleHasAMinorChange()
		{
			mProcessIt = true;
		}
		internal void ArticleHasAMajorChange()
		{
			mProcessIt = true;
		}
		internal bool ProcessIt {
			get { return mProcessIt; }
		}

		// For calling by plugin:
		internal void PluginCheckTemplateCall(string TemplateCall, string PluginName)
		{
			// we have "template:"
			if (!string.IsNullOrEmpty(TemplateCall)) {
				mProcessIt = true;
			}
		}
		internal void PluginIHaveFinished(SkipResults Result, string PluginName)
		{
			switch (Result) {
				case SkipResults.SkipBadTag:
					mSkipResults = SkipResults.SkipBadTag;
					PluginManager.AWBForm.TraceManager.SkippedArticleBadTag(PluginName, mFullArticleTitle, mNamespace);
					break;
				case SkipResults.SkipRegex:
					if (mSkipResults == SkipResults.NotSet)
						mSkipResults = SkipResults.SkipRegex;
					PluginManager.AWBForm.TraceManager.SkippedArticle(PluginName, "Article text matched skip regex");
					break;
				case SkipResults.SkipNoChange:
					PluginManager.AWBForm.TraceManager.SkippedArticle(PluginName, "No change");
					mSkipResults = SkipResults.SkipNoChange;
					break;
			}
		}

		// For calling by manager:
		internal SkipResults PluginManagerGetSkipResults {
			get { return mSkipResults; }
		}
		internal void FinaliseEditSummary()
		{
			EditSummary = Regex.Replace(EditSummary, ", $", "");
		}

		// General article writing and manipulation:
		internal void RenamedATemplate(string OldName, string NewName, string Caller)
		{
			DoneReplacement(OldName, NewName, false);
		}
		internal void DoneReplacement(string Old, string Replacement, bool LogIt, string PluginName = "")
		{
			mProcessIt = true;
			EditSummary += Old + "→" + Replacement + ", ";
		}
		internal void TemplateAdded(string Template, string PluginName)
		{
			mEditSummary += string.Format("Added {{{{[[Template:{0}|{0}]]}}}}, ", Template);
			ArticleHasAMajorChange();
		}
		internal void ParameterAdded(string ParamName, string ParamValue, string PluginName, bool MinorEdit = false)
		{
			mEditSummary += ParamName + "=" + ParamValue + ", ";

			if (MinorEdit)
				ArticleHasAMinorChange();
			else
				ArticleHasAMajorChange();
		}

        // FIXME: To be replaced
		readonly Microsoft.VisualBasic.CompilerServices.StaticLocalInitFlag static_RestoreTemplateToPlaceholderSpot_strPlaceholder_Init = new Microsoft.VisualBasic.CompilerServices.StaticLocalInitFlag();
		// just write one instance of template even if have multiple conTemplatePlaceholder's
		string static_RestoreTemplateToPlaceholderSpot_strPlaceholder;
		readonly Microsoft.VisualBasic.CompilerServices.StaticLocalInitFlag static_RestoreTemplateToPlaceholderSpot_RestoreTemplateToPlaceholderSpotRegex_Init = new Microsoft.VisualBasic.CompilerServices.StaticLocalInitFlag();
		Regex static_RestoreTemplateToPlaceholderSpot_RestoreTemplateToPlaceholderSpotRegex;
		internal void RestoreTemplateToPlaceholderSpot(string TemplateHeader)
		{
			lock (static_RestoreTemplateToPlaceholderSpot_strPlaceholder_Init) {
				try {
					if (InitStaticVariableHelper(static_RestoreTemplateToPlaceholderSpot_strPlaceholder_Init)) {
                        static_RestoreTemplateToPlaceholderSpot_strPlaceholder = Regex.Escape(Constants.conTemplatePlaceholder);
					}
				} finally {
					static_RestoreTemplateToPlaceholderSpot_strPlaceholder_Init.State = 1;
				}
			}
			lock (static_RestoreTemplateToPlaceholderSpot_RestoreTemplateToPlaceholderSpotRegex_Init) {
				try {
					if (InitStaticVariableHelper(static_RestoreTemplateToPlaceholderSpot_RestoreTemplateToPlaceholderSpotRegex_Init)) {
						static_RestoreTemplateToPlaceholderSpot_RestoreTemplateToPlaceholderSpotRegex = new Regex(static_RestoreTemplateToPlaceholderSpot_strPlaceholder);
					}
				} finally {
					static_RestoreTemplateToPlaceholderSpot_RestoreTemplateToPlaceholderSpotRegex_Init.State = 1;
				}
			}

			AlteredArticleText = static_RestoreTemplateToPlaceholderSpot_RestoreTemplateToPlaceholderSpotRegex.Replace(AlteredArticleText, TemplateHeader, 1);
			AlteredArticleText = static_RestoreTemplateToPlaceholderSpot_RestoreTemplateToPlaceholderSpotRegex.Replace(AlteredArticleText, "");
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
		private BannerShellsEnum WeFoundBannerShells;

		private string MatchEvaluatorString;
		// Regexes:
		// These could probably be simplified significantly (and extra logic doing things like removing linebreaks) if I learnt more of the magic characters

        private static readonly Regex WikiProjectBannerShellRegex = new Regex(Constants.conRegexpLeft + WikiProjectBannerShell + ")\\b\\s*(?<start>\\|[^1]*=.*?)*\\s*\\|\\s*1\\s*=\\s*(?<body>.*}}[^{]*?)\\s*(?<end>\\|[^{]*)?\\s*}}", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.ExplicitCapture);

		static internal readonly Regex LineBreakRegex = new Regex("[\\n\\r]*");

		private static readonly Regex DoubleLineBreakRegex = new Regex("[\\n\\r]{2,}");
		// Regex constant strings:
			// IGNORE CASE
		private const string WikiProjectBannerShell = "WikiProject ?Banner ?Shell|WP?BS|WPBannerShell|WikiProject ?Banners|WPB";

		// Match evaluators:
		private string WPBSRegexMatchEvaluator(Match match)
		{
			const string templatename = "WikiProjectBannerShell";
			string Ending = match.Groups["start"].Value + match.Groups["end"].Value;

			ShellTemplateMatchEvaluatorsCommonTasks(templatename, match);

			if (!string.IsNullOrEmpty(Ending))
				Ending = Environment.NewLine + Ending;

			return DoubleLineBreakRegex.Replace("{{" + templatename + "|1=" + Environment.NewLine + LineBreakRegex.Replace(MatchEvaluatorString, "") + Environment.NewLine + match.Groups["body"].Value + Ending + "}}", Environment.NewLine);
		}

		private void ShellTemplateMatchEvaluatorsCommonTasks(string templatename, Match match)
		{
			// Does the shell contain template: ?
			PluginCheckTemplateCall(match.Groups["tl"].Value, templatename);
			// Does the template have it's primary name:
			if (!(match.Groups["tlname"].Value == templatename)) {
				RenamedATemplate(match.Groups["tlname"].Value, templatename, templatename);
			}
		}

		// Where we (possibly) add our template to an existing shell:
		internal void PrependTemplateOrWriteIntoShell(Templating Template, string ParameterBreak, string Text)
		{
			if (WeFoundBannerShells == BannerShellsEnum.NotChecked) {
				if (WikiProjectBannerShellRegex.IsMatch(AlteredArticleText)) {
					WeFoundBannerShells = BannerShellsEnum.FoundWikiProjectBannerShell;
				} else {
					WeFoundBannerShells = BannerShellsEnum.NoneFound;
				}
			}

			Text += Template.ParametersToString(ParameterBreak);

			switch (WeFoundBannerShells) {
				case BannerShellsEnum.FoundWikiProjectBannerShell:
					MatchEvaluatorString = Text;

					AlteredArticleText = WikiProjectBannerShellRegex.Replace(AlteredArticleText, WPBSRegexMatchEvaluator, 1);
					MatchEvaluatorString = null;
					break;
				case BannerShellsEnum.NoneFound:
					AlteredArticleText = Text + AlteredArticleText;
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
		static bool InitStaticVariableHelper(Microsoft.VisualBasic.CompilerServices.StaticLocalInitFlag flag)
		{
			if (flag.State == 0) {
				flag.State = 2;
				return true;
			} else if (flag.State == 2) {
				throw new Microsoft.VisualBasic.CompilerServices.IncompleteInitialization();
			} else {
				return false;
			}
		}
	}
}
