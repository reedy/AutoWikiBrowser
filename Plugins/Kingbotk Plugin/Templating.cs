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
	/// An object which wraps around a collection of template parameters
	/// </summary>
	/// <remarks></remarks>
	internal sealed class Templating
	{
		internal bool FoundTemplate;
		internal bool BadTemplate;

		internal Dictionary<string, TemplateParametersObject> Parameters = new Dictionary<string, TemplateParametersObject>();
		/// <summary>
		/// Store a parameter from the exploded on-page template into the Parameters collectiom
		/// </summary>
		internal void AddTemplateParmFromExistingTemplate(string ParameterName, string ParameterValue)
		{
			// v2.0: Let's merge duplicates when one or both is empty:
			if (Parameters.ContainsKey(ParameterName)) {
				// This code is very similar to ReplaceTemplateParm(), but that is for programmatic changes (i.e. not
				// from template), needs an Article object, doesn't understand empty new values, and doesn't report
				// bad tags. Turned out to be easier to rewrite here than to modify it.
				if (!(Parameters[ParameterName].Value == ParameterValue)) {
					// existing value is empty, overwrite with new
					if (string.IsNullOrEmpty(Parameters[ParameterName].Value)) {
						Parameters[ParameterName].Value = ParameterValue;
					// new value is empty, keep existing
					} else if (string.IsNullOrEmpty(ParameterValue)) {
					// 2 different non-empty values, template is bad
					} else {
						BadTemplate = true;
					}
				}
				// Else: 2 values the same, do nothing
			} else {
				Parameters.Add(ParameterName, new TemplateParametersObject(ParameterName, ParameterValue));
			}
		}
		internal void NewTemplateParm(string ParameterName, string ParameterValue)
		{
			Parameters.Add(ParameterName, new TemplateParametersObject(ParameterName, ParameterValue));
		}
		internal void NewTemplateParm(string ParameterName, string ParameterValue, bool LogItAndUpdateEditSummary, Article TheArticle, string PluginName, bool MinorEdit = false)
		{
			NewTemplateParm(ParameterName, ParameterValue);
			if (LogItAndUpdateEditSummary)
				TheArticle.ParameterAdded(ParameterName, ParameterValue, PluginName, MinorEdit);
		}
		/// <summary>
		/// Checks for the existence of a parameter and adds it if missing/optionally changes it
		/// </summary>
		/// <returns>True if made a change</returns>
		internal bool NewOrReplaceTemplateParm(string ParameterName, string ParameterValue, Article TheArticle, bool LogItAndUpdateEditSummary, bool ParamHasAlternativeName, bool DontChangeIfSet = false, string ParamAlternativeName = "", string PluginName = "", bool MinorEditOnlyIfAdding = false)
		{
			bool res = false;

			if (Parameters.ContainsKey(ParameterName)) {
				res = ReplaceTemplateParm(ParameterName, ParameterValue, TheArticle, LogItAndUpdateEditSummary, DontChangeIfSet, PluginName);
			} else if (ParamHasAlternativeName && Parameters.ContainsKey(ParamAlternativeName)) {
				res = ReplaceTemplateParm(ParamAlternativeName, ParameterValue, TheArticle, LogItAndUpdateEditSummary, DontChangeIfSet, PluginName);
			// Doesn't contain parameter
			} else {
				NewTemplateParm(ParameterName, ParameterValue, LogItAndUpdateEditSummary, TheArticle, PluginName, MinorEditOnlyIfAdding);

				if (MinorEditOnlyIfAdding)
					TheArticle.ArticleHasAMinorChange();
				else
					TheArticle.ArticleHasAMajorChange();
				return true;
			}

			if (res)
				TheArticle.ArticleHasAMajorChange();
			return res;
		}
		private bool ReplaceTemplateParm(string ParameterName, string ParameterValue, Article TheArticle, bool LogItAndUpdateEditSummary, bool DontChangeIfSet, string PluginName)
		{
			string ExistingValue = WikiRegexes.Comments.Replace(Parameters[ParameterName].Value, "").Trim();
			// trim still needed because altho main regex shouldn't give us spaces at the end of vals any more, the .Replace here might

			// Contains parameter with a different value
			if (!(ExistingValue == ParameterValue)) {
				// Contains parameter with a different value, and _
				if (string.IsNullOrEmpty(ExistingValue) || !DontChangeIfSet) {
					// we want to change it; or contains an empty parameter
					Parameters[ParameterName].Value = ParameterValue;
					TheArticle.ArticleHasAMajorChange();
					if (LogItAndUpdateEditSummary) {
						if (string.IsNullOrEmpty(ExistingValue)) {
							TheArticle.ParameterAdded(ParameterName, ParameterValue, PluginName);
						} else {
							TheArticle.DoneReplacement(ParameterName + "=" + ExistingValue, ParameterValue, true, PluginName);
						}
					}
					return true;
				// Contains param with a different value, and we don't want to change it
				} else {
					return false;
				}
			}
			// Else: Already contains parameter and correct value; no need to change
			return false;
		}

		internal string ParametersToString(string ParameterBreak)
		{
			string res = "";
			foreach (KeyValuePair<string, TemplateParametersObject> o in Parameters) {
				var _with1 = o;
				res += "|" + _with1.Key + "=" + _with1.Value.Value + ParameterBreak;
			}

			res += "}}" + Microsoft.VisualBasic.Constants.vbCrLf;
			return res;
		}
		internal bool HasYesParamLowerOrTitleCase(bool Yes, string ParamName)
		{
			// A little hack to ensure we don't change no to No or yes to Yes as our only edit, and also for checking "yes" values
			if (Parameters.ContainsKey(ParamName)) {
				if (Yes && Parameters[ParamName].Value.ToLower() == "yes") {
					return true;
				} else if (!Yes && Parameters[ParamName].Value.ToLower() == "No") {
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// An object which represents a template parameter
		/// </summary>
		/// <remarks></remarks>
		internal sealed class TemplateParametersObject
		{
			internal string Name;

			internal string Value;
			internal TemplateParametersObject(string ParameterName, string ParameterValue)
			{
				Name = ParameterName;
				Value = ParameterValue;
			}
		}
	}
}
