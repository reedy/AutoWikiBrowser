using System;
using System.Collections.Generic;
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
    /// An object which wraps around a collection of template parameters
    /// </summary>
    /// <remarks></remarks>
    internal sealed class Templating
    {
        internal bool FoundTemplate;
        internal bool BadTemplate;

        internal Dictionary<string, TemplateParametersObject> Parameters =
            new Dictionary<string, TemplateParametersObject>();

        /// <summary>
        /// Store a parameter from the exploded on-page template into the Parameters collectiom
        /// </summary>
        internal void AddTemplateParmFromExistingTemplate(string parameterName, string parameterValue)
        {
            // v2.0: Let's merge duplicates when one or both is empty:
            if (Parameters.ContainsKey(parameterName))
            {
                // This code is very similar to ReplaceTemplateParm(), but that is for programmatic changes (i.e. not
                // from template), needs an Article object, doesn't understand empty new values, and doesn't report
                // bad tags. Turned out to be easier to rewrite here than to modify it.
                if (Parameters[parameterName].Value != parameterValue)
                {
                    // existing value is empty, overwrite with new
                    if (string.IsNullOrEmpty(Parameters[parameterName].Value))
                    {
                        Parameters[parameterName].Value = parameterValue;
                        // new value is empty, keep existing
                    }
                    else if (string.IsNullOrEmpty(parameterValue))
                    {
                        // 2 different non-empty values, template is bad
                    }
                    else
                    {
                        BadTemplate = true;
                    }
                }
                // Else: 2 values the same, do nothing
            }
            else
            {
                Parameters.Add(parameterName, new TemplateParametersObject(parameterName, parameterValue));
            }
        }

        internal void NewTemplateParm(string parameterName, string parameterValue)
        {
            Parameters.Add(parameterName, new TemplateParametersObject(parameterName, parameterValue));
        }

        internal void NewTemplateParm(string parameterName, string parameterValue, bool logItAndUpdateEditSummary,
            Article theArticle, string pluginName, bool minorEdit = false)
        {
            NewTemplateParm(parameterName, parameterValue);
            if (logItAndUpdateEditSummary)
                theArticle.ParameterAdded(parameterName, parameterValue, minorEdit);
        }

        /// <summary>
        /// Checks for the existence of a parameter and adds it if missing/optionally changes it
        /// </summary>
        /// <returns>True if made a change</returns>
        internal bool NewOrReplaceTemplateParm(string parameterName, string parameterValue, Article theArticle,
            bool logItAndUpdateEditSummary, bool paramHasAlternativeName, bool dontChangeIfSet = false,
            string paramAlternativeName = "", string pluginName = "", bool minorEditOnlyIfAdding = false)
        {
            bool res;

            if (Parameters.ContainsKey(parameterName))
            {
                res = ReplaceTemplateParm(parameterName, parameterValue, theArticle, logItAndUpdateEditSummary,
                    dontChangeIfSet);
            }
            else if (paramHasAlternativeName && Parameters.ContainsKey(paramAlternativeName))
            {
                res = ReplaceTemplateParm(paramAlternativeName, parameterValue, theArticle, logItAndUpdateEditSummary,
                    dontChangeIfSet);
                // Doesn't contain parameter
            }
            else
            {
                NewTemplateParm(parameterName, parameterValue, logItAndUpdateEditSummary, theArticle, pluginName,
                    minorEditOnlyIfAdding);

                if (minorEditOnlyIfAdding)
                    theArticle.ArticleHasAMinorChange();
                else
                    theArticle.ArticleHasAMajorChange();
                return true;
            }

            if (res)
                theArticle.ArticleHasAMajorChange();
            return res;
        }

        private bool ReplaceTemplateParm(string parameterName, string parameterValue, Article theArticle,
            bool logItAndUpdateEditSummary, bool dontChangeIfSet)
        {
            string existingValue = WikiRegexes.Comments.Replace(Parameters[parameterName].Value, "").Trim();
            // trim still needed because altho main regex shouldn't give us spaces at the end of vals any more, the .Replace here might

            // Contains parameter with a different value
            if (existingValue != parameterValue)
            {
                // Contains parameter with a different value, and _
                if (string.IsNullOrEmpty(existingValue) || !dontChangeIfSet)
                {
                    // we want to change it; or contains an empty parameter
                    Parameters[parameterName].Value = parameterValue;
                    theArticle.ArticleHasAMajorChange();
                    if (logItAndUpdateEditSummary)
                    {
                        if (string.IsNullOrEmpty(existingValue))
                        {
                            theArticle.ParameterAdded(parameterName, parameterValue);
                        }
                        else
                        {
                            theArticle.DoneReplacement(parameterName + "=" + existingValue, parameterValue);
                        }
                    }
                    return true;
                    // Contains param with a different value, and we don't want to change it
                }
            }
            // Else: Already contains parameter and correct value; no need to change
            return false;
        }

        internal string ParametersToString(string parameterBreak)
        {
            string res = "";
            foreach (KeyValuePair<string, TemplateParametersObject> o in Parameters)
            {
                res += "|" + o.Key + "=" + o.Value.Value + parameterBreak;
            }

            res += "}}" + Environment.NewLine;
            return res;
        }

        internal bool HasYesParamLowerOrTitleCase(bool yes, string paramName)
        {
            // A little hack to ensure we don't change no to No or yes to Yes as our only edit, and also for checking "yes" values
            if (Parameters.ContainsKey(paramName))
            {
                if (yes && Parameters[paramName].Value.ToLower() == "yes" ||
                    !yes && Parameters[paramName].Value.ToLower() == "no")
                {
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
            internal string Name, Value;

            internal TemplateParametersObject(string parameterName, string parameterValue)
            {
                Name = parameterName;
                Value = parameterValue;
            }
        }
    }
}