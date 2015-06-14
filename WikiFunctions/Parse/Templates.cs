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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using WikiFunctions.Lists.Providers;

namespace WikiFunctions.Parse
{

    /// <summary>
    /// Provides functions for editing wiki text, such as formatting and re-categorisation.
    /// </summary>
    public partial class Parsers
    {

        private static readonly Regex TlOrTlx = Tools.NestedTemplateRegex(new List<string>(new[] { "tl", "tlx" }));
        private static readonly Regex TemplateRedirectsR = new Regex(@"({{ *[Tt]lx? *\|.*}}) *→[ ']*({{ *[Tt]lx? *\| *(.*?) *}})");

        /// <summary>
        /// Processes the text of [[WP:AWB/Template redirects]] into a dictionary of regexes and new template names
        /// Format: {{tl|template 1}}, {{tl|template 2}} → {{tl|actual template}}
        /// </summary>
        public static Dictionary<Regex, string> LoadTemplateRedirects(string text)
        {
            text = WikiRegexes.UnformattedText.Replace(text, "");
            Dictionary<Regex, string> TRs = new Dictionary<Regex, string>();
            List<string> AllRedirectsList = new List<string>();

            foreach (Match m in TemplateRedirectsR.Matches(text))
            {
                string redirects = m.Groups[1].Value, templateName = m.Groups[2].Value;
                templateName = TlOrTlx.Match(templateName).Groups[3].Value.Trim('|').TrimEnd('}').Trim();

                // get all redirects into a list
                List<string> redirectsList = new List<string>();

                foreach (Match r in TlOrTlx.Matches(redirects))
                {
                    redirectsList.Add(Tools.TurnFirstToUpperNoProjectCheck(r.Groups[3].Value.Trim('|').TrimEnd('}').Trim()));
                }

                TRs.Add(Tools.NestedTemplateRegex(redirectsList), templateName);
                AllRedirectsList.AddRange(redirectsList);
            }
            
            WikiRegexes.AllTemplateRedirects = Tools.NestedTemplateRegex(AllRedirectsList);

            // must use separate functions to set value: if HashSets in same function compiler will pre-load them even if not used
            if(Globals.SystemCore3500Available)
                SetAllTemplateRedirectsHashSet(AllRedirectsList);
            else
                SetAllTemplateRedirectsList(AllRedirectsList);

            return TRs;
        }
        
        /// <summary>
        /// Sets the WikiRegexes .AllTemplateRedirects HashSet
        /// </summary>
        /// <param name="RedirectsList"></param>
        private static void SetAllTemplateRedirectsHashSet(List<string> RedirectsList)
        {
            WikiRegexes.AllTemplateRedirectsHS = new HashSet<string>(RedirectsList);
        }
        
        /// <summary>
        /// Sets the WikiRegexes .AllTemplateRedirects List
        /// </summary>
        /// <param name="RedirectsList"></param>
        private static void SetAllTemplateRedirectsList(List<string> RedirectsList)
        {
            WikiRegexes.AllTemplateRedirectsList = RedirectsList;
        }

        /// <summary>
        /// Processes the text of [[WP:AWB/Dated templates]] into a list of template names
        /// Format: * {{tl|Wikify}}
        /// </summary>
        /// <param name="text">The rule page text</param>
        /// <returns>List of templates to match dated templates</returns>
        public static List<string> LoadDatedTemplates(string text)
        {
            text = WikiRegexes.UnformattedText.Replace(text, "");
            return (from Match m in TlOrTlx.Matches(text) select Tools.TurnFirstToUpper(m.Groups[3].Value.Trim('|').TrimEnd('}').Trim())).ToList();
        }

        /// <summary>
        /// Renames templates to bypass template redirects from [[WP:AWB/Template redirects]]
        /// The first letter casing of the existing redirect is kept in the new template name,
        ///  except for acronym templates where first letter uppercase is enforced
        /// Calls TemplateToMagicWord if changes made
        /// </summary>
        /// <param name="articleText">the page text</param>
        /// <param name="TemplateRedirects">Dictionary of redirects and templates</param>
        /// <returns>The updated article text</returns>
        public static string TemplateRedirects(string articleText, Dictionary<Regex, string> TemplateRedirects)
        {
            string newArticleText;
            if(WikiRegexes.AllTemplateRedirects == null)
                return articleText;

            if(Globals.SystemCore3500Available)
                newArticleText = TemplateRedirectsHashSet(articleText, TemplateRedirects);
            else
                newArticleText = TemplateRedirectsList(articleText, TemplateRedirects);

            // call TemplateToMagicWord if changes made
            if (!newArticleText.Equals(articleText))
                return Tools.TemplateToMagicWord(newArticleText);

            return articleText;
        }

        /// <summary>
        /// Most performant version of TemplateRedirects using HashSets
        /// </summary>
        /// <param name="articleText"></param>
        /// <param name="TemplateRedirects"></param>
        /// <returns></returns>
        private static string TemplateRedirectsHashSet(string articleText, Dictionary<Regex, string> TemplateRedirects)
        {
            // performance: first check there's a match between templates used in article and listed template redirects
            // using intersection of HashSet lists of the two
            HashSet<string> TemplatesFound = new HashSet<string>(GetAllTemplates(articleText));
            TemplatesFound.IntersectWith(WikiRegexes.AllTemplateRedirectsHS);

            // run replacements only if matches found
            if(TemplatesFound.Any())
            {
                // performance: secondly filter the TemplateRedirects dictionary down to those rules matching templates used in article
                string all = String.Join(" ", TemplatesFound.Select(s => "{{" + s + "}}").ToArray());
                TemplateRedirects = TemplateRedirects.Where(s => s.Key.IsMatch(all)).ToDictionary(x => x.Key, y => y.Value);

                // performance: thirdly then run replacement for only those matching templates, and only against the matching rules, handle nested templates
                Regex MatchedTemplates = Tools.NestedTemplateRegex(TemplatesFound.ToList());
                while(MatchedTemplates.IsMatch(articleText))
                    articleText = MatchedTemplates.Replace(articleText, m2=>
                                                                              {
                                                                                  string res = m2.Value;
                                                                                  
                                                                                  foreach (KeyValuePair<Regex, string> kvp in TemplateRedirects)
                                                                                  {
                                                                                      res = kvp.Key.Replace(res, m => TemplateRedirectsME(m, kvp.Value));
                                                                                      
                                                                                      // if template name changed and not nested template, done, so break out
                                                                                      if(!res.Equals(m2.Value) && !m2.Groups[3].Value.Contains("{{"))
                                                                                          break;
                                                                                  }
                                                                                  return res;
                                                                              });
            }
            return articleText;
        }

        /// <summary>
        /// (slower) version of TemplateRedirects using List
        /// </summary>
        /// <param name="articleText"></param>
        /// <param name="TemplateRedirects"></param>
        /// <returns></returns>
        private static string TemplateRedirectsList(string articleText, Dictionary<Regex, string> TemplateRedirects)
        {
            List<string> TFH = GetAllTemplates(articleText);

            // if matches found, run replacements
            List<string> TFH2 = new List<string>();
            foreach(string s in TFH)
            {
                if(WikiRegexes.AllTemplateRedirectsList.Contains(s))
                    TFH2.Add(s);
            }

            if(TFH2.Count > 0)
            {
                articleText = Tools.NestedTemplateRegex(TFH2).Replace(articleText, m2=>
                                                                      {
                                                                          string res = m2.Value;
                                                                          
                                                                          foreach (KeyValuePair<Regex, string> kvp in TemplateRedirects)
                                                                          {
                                                                              res = kvp.Key.Replace(res, m => TemplateRedirectsME(m, kvp.Value));
                                                                              
                                                                              // if template name changed and not nested template, done, so break out
                                                                              if(!res.Equals(m2.Value) && !m2.Groups[3].Value.Contains("{{"))
                                                                                  break;
                                                                          }
                                                                          return res;
                                                                      });
            }
            return articleText;
        }

        private static readonly Regex AcronymTemplate = new Regex(@"^[A-Z]{3}");

        private static string TemplateRedirectsME(Match m, string newTemplateName)
        {
            string originalTemplateName = m.Groups[2].Value;

            if (!AcronymTemplate.IsMatch(newTemplateName))
            {
                if (Tools.TurnFirstToUpper(originalTemplateName).Equals(originalTemplateName))
                    newTemplateName = Tools.TurnFirstToUpper(newTemplateName);
                else
                    newTemplateName = Tools.TurnFirstToLower(newTemplateName);
            }

            return (m.Groups[1].Value + newTemplateName + m.Groups[3].Value);
        }

        private static readonly Regex NestedTemplates = new Regex(@"{{\s*([^{}\|]*)((?>[^\{\}]+|\{(?<DEPTH>)|\}(?<-DEPTH>))*(?(DEPTH)(?!)))}}");

        /// <summary>
        /// Extracts a list of all templates used in the input text, supporting any level of template nesting. Template name given in first letter upper
        /// </summary>
        /// <param name="articleText"></param>
        /// <returns>List of all templates in text</returns>
        public static List<string> GetAllTemplates(string articleText)
        {
            if(Globals.SystemCore3500Available)
                return GetAllTemplatesNew(articleText);
            return GetAllTemplatesOld(articleText);
        }

        private static Queue<KeyValuePair<string, List<string>>> GetAllTemplatesNewQueue = new Queue<KeyValuePair<string, List<string>>>();
        private static Queue<KeyValuePair<string, List<string>>> GetAllTemplatesDetailNewQueue = new Queue<KeyValuePair<string, List<string>>>();

        /// <summary>
        /// Extracts a list of all templates used in the input text, supporting any level of template nesting. Template name given in first letter upper. Most performant version using HashSet.
        /// </summary>
        /// <param name="articleText"></param>
        /// <returns></returns>
        private static List<string> GetAllTemplatesNew(string articleText)
        {
            // For peformance, use cached result if available: articletext plus List of template names
            List<string> found = GetAllTemplatesNewQueue.FirstOrDefault(q => q.Key.Equals(articleText)).Value;
            if(found != null)
                return found;
                
            /* performance: process all templates in bulk, extract template contents and reprocess. This is faster than loop applying template match on individual basis. 
            Extract rough template name then get exact template names later, faster to deduplicate then get exact template names */
            // process all templates, handle nested templates to any level of nesting
            List<string> TemplateNames = new List<string>();
            List<string> TemplateDetail = new List<string>();
            HashSet<string> innerTemplateContents = new HashSet<string>();
            string originalarticleText = articleText;

            for(;;)
            {
                List<Match> nt = (from Match m in NestedTemplates.Matches(articleText) select m).ToList();

                if(!nt.Any())
                    break;

                TemplateDetail.AddRange(nt.Select(x => x.Value));

                // add raw template names to list
                TemplateNames.AddRange(nt.Select(m => m.Groups[1].Value).ToList());

                // set text to content of matched templates to process again for any (further) nested templates
                innerTemplateContents = new HashSet<string>(nt.Select(m => m.Groups[2].Value).ToList());
                articleText = String.Join(",", innerTemplateContents.ToArray());
            }

            // now extract exact template names
            List<string> FinalTemplateNames = TemplateNames.Distinct().Select(s => Tools.TurnFirstToUpper(Tools.GetTemplateName(@"{{" + s + @"}}"))).Distinct().ToList();

            TemplateDetail = Tools.DeduplicateList(TemplateDetail);

            // cache new results, then dequeue oldest if cache full
            GetAllTemplatesNewQueue.Enqueue(new KeyValuePair<string, List<string>>(originalarticleText,  FinalTemplateNames));
            if(GetAllTemplatesNewQueue.Count > 10)
                GetAllTemplatesNewQueue.Dequeue();

            GetAllTemplatesDetailNewQueue.Enqueue(new KeyValuePair<string, List<string>>(originalarticleText,  TemplateDetail));
            if(GetAllTemplatesDetailNewQueue.Count > 10)
                GetAllTemplatesDetailNewQueue.Dequeue();

            return FinalTemplateNames;
        }

        /// <summary>
        /// Extracts a list of all templates used in the input text. Template name given in first letter upper. .NET 2.0 version not using HashSet
        /// </summary>
        /// <param name="articleText"></param>
        /// <returns></returns>
        private static List<string> GetAllTemplatesOld(string articleText)
        {
            /* performance: faster to process all templates, extract rough template name then get exact template names later
             than to get exact template name for each template found */
            // process all templates, handle nested templates to any level of nesting
            List<string> TFH = new List<string>();
            foreach(Match m in NestedTemplates.Matches(articleText))
            {
                TFH.Add(m.Groups[1].Value);
                string template = m.Value.Substring(2);

                while(NestedTemplates.IsMatch(template))
                {
                    Match m2 = NestedTemplates.Match(template);
                    TFH.Add(m2.Groups[1].Value);
                    template = template.Substring(m2.Index + 2);
                }
            }

            // now extract exact template names
            TFH = Tools.DeduplicateList(TFH);
            List<string> TFH2 = new List<string>();
            foreach(string s in TFH)
            {
                TFH2.Add(Tools.TurnFirstToUpper(Tools.GetTemplateName(@"{{" + s + @"}}")));
            }

            return Tools.DeduplicateList(TFH2);
        }

        /// <summary>
        /// Extracts a list of all template calls in the input text, supporting any level of template nesting.
        /// </summary>
        /// <param name="articleText"></param>
        /// <returns>List of all templates calls in text</returns>
        public static List<string> GetAllTemplateDetail(string articleText)
        {
            GetAllTemplates(articleText);

            List<string> found = GetAllTemplatesDetailNewQueue.FirstOrDefault(q => q.Key.Equals(articleText)).Value;
            if(found == null)
                found = new List<string>();

            return found;
        }

        private static List<string> RenameTemplateParametersOldParams = new List<string>();

        /// <summary>
        /// Renames parameters in template calls.
        /// Does not rename old to new if new paramter already in use with a value
        /// </summary>
        /// <param name="articleText">The wiki text</param>
        /// <param name="RenamedTemplateParameters">List of templates, old parameter, new parameter</param>
        /// <returns>The updated wiki text</returns>
        public static string RenameTemplateParameters(string articleText, List<WikiRegexes.TemplateParameters> RenamedTemplateParameters)
        {
            if (RenamedTemplateParameters.Count == 0)
                return articleText;

            // Performance: now filter templates with parameters to rename against templates used on the page
            // so only templates used on page are looked for
            List<string> templatesToProcess = GetAllTemplates(articleText).Select(t => Tools.TurnFirstToLower(t)).ToList();

            // filter the parameters set down to only those templates used on the page
            RenamedTemplateParameters = RenamedTemplateParameters.Where(t => templatesToProcess.Contains(t.TemplateName)).ToList();

            RenameTemplateParametersOldParams = Tools.DeduplicateList(RenamedTemplateParameters.Select(x => x.OldParameter).ToList());

            templatesToProcess = Tools.DeduplicateList(RenamedTemplateParameters.Select(x => x.TemplateName).ToList());

            if(!templatesToProcess.Any())
                return articleText;

            Regex r = Tools.NestedTemplateRegex(templatesToProcess);

            // Now process distinct templates used in articles using GetAllTemplateDetail
            foreach(string s in GetAllTemplateDetail(articleText))
            {
                string res = r.Replace(s,
                                         m => (Globals.SystemCore3500Available ?
                                               RenameTemplateParametersHashSetME(m, RenamedTemplateParameters)
                                               : RenameTemplateParametersME(m, RenamedTemplateParameters)));
                                                                   
                if(!s.Equals(res))
                    articleText = articleText.Replace(s, res);                
            }

            return articleText;
        }

        /// <summary>
        /// Most performant RenameTemplateParameters MatchEvaluator using HashSets
        /// </summary>
        /// <param name="m"></param>
        /// <param name="RenamedTemplateParameters"></param>
        /// <returns></returns>
        private static string RenameTemplateParametersHashSetME(Match m, List<WikiRegexes.TemplateParameters> RenamedTemplateParameters)
        {
            string newvalue = m.Value;

            // performance: check for intersection of bad parameters and parameters used in template
            // rather than simply looping through all parameters in list
            Dictionary<string, string> pv = Tools.GetTemplateParameterValues(m.Value);
            List<string> oldP = RenameTemplateParametersOldParams.Intersect(pv.Keys.ToArray()).ToList();
            if(oldP.Any())
            {
                string tname = Tools.TurnFirstToLower(Tools.GetTemplateName(@"{{" + m.Groups[2].Value + @"}}"));
                foreach (WikiRegexes.TemplateParameters Params in RenamedTemplateParameters.Where(r => oldP.Contains(r.OldParameter) && r.TemplateName.Equals(tname)))
                {
                    string newp;
                    pv.TryGetValue(Params.NewParameter, out newp);

                    if(string.IsNullOrEmpty(newp))
                        newvalue = Tools.RenameTemplateParameter(newvalue, Params.OldParameter, Params.NewParameter);
                }
            }

            return newvalue;
        }

        /// <summary>
        /// Less performant RenameTemplateParameters not using HashSets
        /// </summary>
        /// <param name="m"></param>
        /// <param name="RenamedTemplateParameters"></param>
        /// <returns></returns>
        private static string RenameTemplateParametersME(Match m, List<WikiRegexes.TemplateParameters> RenamedTemplateParameters)
        {
            string templatename = Tools.TurnFirstToLower(m.Groups[2].Value), newvalue = m.Value;

            foreach (WikiRegexes.TemplateParameters Params in RenamedTemplateParameters)
            {
                if (Params.TemplateName.Equals(templatename) && newvalue.Contains(Params.OldParameter)
                    && Tools.GetTemplateParameterValue(m.Value, Params.NewParameter).Length == 0)
                    newvalue = Tools.RenameTemplateParameter(newvalue, Params.OldParameter, Params.NewParameter);
            }

            return newvalue;
        }

        /// <summary>
        /// Loads List of templates (first letter lower), old parameter, new parameter from within {{AWB rename template parameter}}
        /// </summary>
        /// <param name="text">Source page of {{AWB rename template parameter}} rules</param>
        /// <returns>List of templates (first letter lower), old parameter, new parameter</returns>
        public static List<WikiRegexes.TemplateParameters> LoadRenamedTemplateParameters(string text)
        {
            text = WikiRegexes.UnformattedText.Replace(text, "");
            List<WikiRegexes.TemplateParameters> TPs = new List<WikiRegexes.TemplateParameters>();

            foreach (Match m in Tools.NestedTemplateRegex("AWB rename template parameter").Matches(text))
            {
                string templatename = Tools.TurnFirstToLower(Tools.GetTemplateArgument(m.Value, 1)), oldparam = Tools.GetTemplateArgument(m.Value, 2),
                newparam = Tools.GetTemplateArgument(m.Value, 3);

                WikiRegexes.TemplateParameters Params;
                Params.TemplateName = templatename;
                Params.OldParameter = oldparam;
                Params.NewParameter = newparam;

                TPs.Add(Params);
            }

            return TPs;
        }

        /// <summary>
        /// Extracts template using the given match.
        /// </summary>
        private static string ExtractTemplate(string articleText, Match m)
        {
            Regex theTemplate = new Regex(Regex.Escape(m.Groups[1].Value) + @"(?>[^\{\}]+|\{(?<DEPTH>)|\}(?<-DEPTH>))*(?(DEPTH)(?!))}}");
            string res = "";
            foreach (Match n in theTemplate.Matches(articleText))
            {
                if (n.Index == m.Index)
                {
                    res= theTemplate.Match(articleText).Value;
                    break;
                }
            }

            return res;
        }

        /// <summary>
        /// Finds first occurrence of a given template in article text.
        /// Handles nested templates correctly.
        /// </summary>
        /// <param name="articleText">Source text</param>
        /// <param name="template">Name of template, can be regex without a group capture</param>
        /// <returns>Template with all params, enclosed in curly brackets</returns>
        public static string GetTemplate(string articleText, string template)
        {
            Regex search = new Regex(@"(\{\{\s*" + Tools.CaseInsensitive(template) + @"\s*)(?:\||\}|<)", RegexOptions.Singleline);

            // remove commented out templates etc. before searching
            string articleTextCleaned = WikiRegexes.UnformattedText.Replace(articleText, "");

            if (search.IsMatch(articleTextCleaned))
            {
                // extract from original article text
                Match m = search.Match(articleText);

                return m.Success ? ExtractTemplate(articleText, m) : "";
            }

            return "";
        }

        /// <summary>
        /// Finds every occurrence of a given template in article text, excludes commented out/nowiki'd templates
        /// Handles nested templates and templates with embedded HTML comments correctly.
        /// </summary>
        /// <param name="articleText">Source text</param>
        /// <param name="template">Name of template</param>
        /// <returns>List of matches found</returns>
        public static List<Match> GetTemplates(string articleText, string template)
        {
            return GetTemplates(articleText, Tools.NestedTemplateRegex(template));
        }

        /// <summary>
        /// Finds all templates in article text excluding commented out/nowiki'd templates.
        /// Handles nested templates and templates with embedded HTML comments correctly.
        /// </summary>
        /// <param name="articleText">Source text</param>
        /// <returns>List of matches found</returns>
        public static List<Match> GetTemplates(string articleText)
        {
            return GetTemplates(articleText, WikiRegexes.NestedTemplates);
        }

        /// <summary>
        /// Finds all templates in article text excluding commented out/nowiki'd templates.
        /// Handles nested templates and templates with embedded HTML comments correctly.
        /// </summary>
        /// <param name="articleText">Source text</param>
        /// <param name="search">nested template regex to use</param>
        /// <returns>List of matches found</returns>
        private static List<Match> GetTemplates(string articleText, Regex search)
        {
            List<Match> templateMatches = new List<Match>();
            string articleTextAtStart = articleText;

            // replace with spaces any commented out templates etc., this means index of real matches remains the same as in actual article text
            articleText = Tools.ReplaceWithSpaces(articleText, WikiRegexes.UnformattedText);

            // return matches found in article text at start, provided they exist in cleaned text
            // i.e. don't include matches for commented out/nowiki'd templates
            foreach (Match m in search.Matches(articleText))
            {
                foreach (Match m2 in search.Matches(articleTextAtStart))
                {
                    if (m2.Index.Equals(m.Index))
                    {
                        templateMatches.Add(m2);
                        break;
                    }
                }
            }

            return templateMatches;
        }

        /// <summary>
        /// Returns whether the given regex matches any of the (first name upper) templates in the given list
        /// </summary>
        private static bool TemplateExists(List<string> templatesFound, Regex r)
        {
            return templatesFound.Any(s => r.IsMatch(@"{{" + s + "|}}"));
        }

        /// <summary>
        /// Returns the value of the given fields from the page's infobox, where available
        /// Returns a null string if the input article has no infobox, or the input field regex doesn't match on the infobox found
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="fields">List of infobox fields to search</param>
        /// <returns>Field value</returns>
        public static string GetInfoBoxFieldValue(string articleText, List<string> fields)
        {
            string infoBox = WikiRegexes.InfoBox.Match(articleText).Value;

            // clean out references and comments
            infoBox = WikiRegexes.Comments.Replace(infoBox, "");
            infoBox = WikiRegexes.Refs.Replace(infoBox, "");

            List<string> FieldsBack = Tools.GetTemplateParametersValues(infoBox, fields, true);

            foreach (string f in FieldsBack)
            {
                if (f.Length > 0)
                    return f;
            }

            return "";
        }

        /// <summary>
        /// Returns the value of the given field from the page's infobox, where available
        /// Returns a null string if the input article has no infobox, or the input field regex doesn't match on the infobox found
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="field">infobox field to search</param>
        /// <returns>Field value</returns>
        public static string GetInfoBoxFieldValue(string articleText, string field)
        {
            return GetInfoBoxFieldValue(articleText, new List<string>(new[] { field }));
        }

        /// <summary>
        /// Removes Template: (or equivalent translation) from start of template calls, canonicalizes template names
        /// </summary>
        /// <param name="articleText">The wiki article text</param>
        /// <returns>The updated article text</returns>
        public static string RemoveTemplateNamespace(string articleText)
        {
            Regex SyntaxRegexTemplate = new Regex(@"(\{\{\s*)" + Variables.NamespacesCaseInsensitive[Namespace.Template] + @"([^\|]*?)(\s*(?:\}\}|\|))", RegexOptions.Singleline);
            
            return (SyntaxRegexTemplate.Replace(articleText, m => m.Groups[1].Value + CanonicalizeTitle(m.Groups[2].Value) + m.Groups[3].Value));
        }

        /// <summary>
        /// Returns the count of matches for the given regex against the (first name upper) templates in the given list
        /// </summary>
        private static int TemplateCount(List<string> templatesFound, Regex r)
        {
            return templatesFound.Where(s => r.IsMatch(@"{{" + s + "|}}")).Count();
        }

	}
}
