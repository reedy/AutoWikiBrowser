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
        /// <summary>
        /// Re-organises the Person Data, stub/disambig templates, categories and interwikis
        /// except when a mainspace article has some 'includeonly' tags etc.
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">The article title.</param>
        /// <returns>The re-organised text.</returns>
        public string SortMetaData(string articleText, string articleTitle)
        {
            return SortMetaData(articleText, articleTitle, true);
        }

        /// <summary>
        /// Re-organises the Person Data, stub/disambig templates, categories and interwikis
        /// except when a mainspace article has some 'includeonly' tags etc.
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">The article title.</param>
        /// <param name="fixOptionalWhitespace">Whether to request optional excess whitespace to be fixed</param>
        /// <returns>The re-organised text.</returns>
        public string SortMetaData(string articleText, string articleTitle, bool fixOptionalWhitespace)
        {
            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests/Archive_5#Substituted_templates
            // if article contains some substituted template stuff, sorting the data may mess it up (further)
            if (Namespace.IsMainSpace(articleTitle) && NoIncludeIncludeOnlyProgrammingElement(articleText))
                return articleText;

            return (Variables.Project <= ProjectEnum.species) ? Sorter.Sort(articleText, articleTitle, fixOptionalWhitespace) : articleText;
        }

        /// <summary>
        /// Merges multiple {{portal}} templates into a single one, removing any duplicates. En-wiki only.
        /// Restricted to {{portal}} calls with one argument
        /// Article must have existing {{portal}} and/or a 'see also' section
        /// </summary>
        /// <param name="articleText">The article text</param>
        /// <returns>The updated article text</returns>
        public static string MergePortals(string articleText)
        {
            if (!Variables.LangCode.Equals("en"))
                return articleText;

            string originalArticleText = articleText;
            List<string> Portals = new List<string>();
            int firstPortal = WikiRegexes.PortalTemplate.Match(articleText).Index;

            foreach (Match m in WikiRegexes.PortalTemplate.Matches(articleText))
            {
                string thePortalCall = m.Value, thePortalName = Tools.GetTemplateArgument(m.Value, 1);

                if (!Portals.Contains(thePortalName) && Tools.GetTemplateArgumentCount(thePortalCall) == 1)
                {
                    Portals.Add(thePortalName);
                    articleText = Regex.Replace(articleText, Regex.Escape(thePortalCall) + @"\s*(?:\r\n)?", "");
                }
            }

            if (Portals.Count == 0)
                return articleText;

            // merge in new portal if multiple portals
            if (Portals.Count < 2)
                return originalArticleText;

            // generate portal string
            string portalsToAdd = Portals.Aggregate("", (current, portal) => current + ("|" + portal));

            // first merge to see also section
            if (WikiRegexes.SeeAlso.Matches(articleText).Count == 1)
                return WikiRegexes.SeeAlso.Replace(articleText, "$0" + Tools.Newline(@"{{Portal" + portalsToAdd + @"}}"));

            // otherwise merge to original location if all portals in same section
            if (Summary.ModifiedSection(originalArticleText, articleText).Length > 0)
                return articleText.Insert(firstPortal, @"{{Portal" + portalsToAdd + @"}}" + "\r\n");

            return originalArticleText;
        }

        /// <summary>
        /// Performs some cleanup operations on dablinks
        /// Merges some for & about dablinks
        /// Merges multiple distinguish into one
        /// </summary>
        /// <param name="articleText">The article text</param>
        /// <returns>The updated article text</returns>
        public static string Dablinks(string articleText)
        {
            if (!Variables.LangCode.Equals("en"))
                return articleText;

            string zerothSection = Tools.GetZerothSection(articleText);
            string restOfArticle = articleText.Substring(zerothSection.Length);
            articleText = zerothSection;

            // conversions

            // otheruses4 rename - Wikipedia only
            if (Variables.IsWikipediaEN)
                articleText = Tools.RenameTemplate(articleText, "otheruses4", "about");

            // "{{about|about x..." --> "{{about|x..."
            foreach (Match m in Tools.NestedTemplateRegex("about").Matches(articleText))
            {
                if (m.Groups[3].Value.TrimStart("| ".ToCharArray()).StartsWith("about", StringComparison.OrdinalIgnoreCase))
                    articleText = articleText.Replace(m.Value, m.Groups[1].Value + m.Groups[2].Value + Regex.Replace(m.Groups[3].Value, @"^\|\s*[Aa]bout\s*", "|"));
            }

            // merging

            // multiple same about into one
            string oldArticleText = "";
            while (oldArticleText != articleText)
            {
                oldArticleText = articleText;
                bool doneAboutMerge = false;
                foreach (Match m in Tools.NestedTemplateRegex("about").Matches(articleText))
                {
                    string firstarg = Tools.GetTemplateArgument(m.Value, 1);

                    foreach (Match m2 in Tools.NestedTemplateRegex("about").Matches(articleText))
                    {
                        if (m2.Value == m.Value)
                            continue;

                        // match when reason is the same, not matching on self
                        if (Tools.GetTemplateArgument(m2.Value, 1).Equals(firstarg))
                        {
                            // argument 2 length > 0
                            if (Tools.GetTemplateArgument(m.Value, 2).Length > 0 && Tools.GetTemplateArgument(m2.Value, 2).Length > 0)
                            {
                                articleText = articleText.Replace(m.Value, m.Value.TrimEnd('}') + @"|" + Tools.GetTemplateArgument(m2.Value, 2) + @"|" + Tools.GetTemplateArgument(m2.Value, 3) + @"}}");
                                doneAboutMerge = true;
                            }

                            // argument 2 is null
                            if (Tools.GetTemplateArgument(m.Value, 2).Length == 0 && Tools.GetTemplateArgument(m2.Value, 2).Length == 0)
                            {
                                articleText = articleText.Replace(m.Value, m.Value.TrimEnd('}') + @"|and|" + Tools.GetTemplateArgument(m2.Value, 3) + @"}}");
                                doneAboutMerge = true;
                            }
                        }
                        // match when reason of one is null, the other not
                        else if (Tools.GetTemplateArgument(m2.Value, 1).Length == 0)
                        {
                            // argument 2 length > 0
                            if (Tools.GetTemplateArgument(m.Value, 2).Length > 0 && Tools.GetTemplateArgument(m2.Value, 2).Length > 0)
                            {
                                articleText = articleText.Replace(m.Value, m.Value.TrimEnd('}') + @"|" + Tools.GetTemplateArgument(m2.Value, 2) + @"|" + Tools.GetTemplateArgument(m2.Value, 3) + @"}}");
                                doneAboutMerge = true;
                            }
                        }

                        if (doneAboutMerge)
                        {
                            articleText = articleText.Replace(m2.Value, "");
                            break;
                        }
                    }
                    if (doneAboutMerge)
                        break;
                }
            }

            // multiple for into about: rename a 2-argument for into an about with no reason value
            if (Tools.NestedTemplateRegex("for").Matches(articleText).Count > 1 && Tools.NestedTemplateRegex("about").Matches(articleText).Count == 0)
            {
                foreach (Match m in Tools.NestedTemplateRegex("for").Matches(articleText))
                {
                    if (Tools.GetTemplateArgument(m.Value, 3).Length == 0)
                    {
                        articleText = articleText.Replace(m.Value, Tools.RenameTemplate(m.Value, "about|"));
                        break;
                    }
                }
            }

            // for into existing about, when about has >=2 arguments
            if (Tools.NestedTemplateRegex("about").Matches(articleText).Count == 1 &&
                Tools.GetTemplateArgument(Tools.NestedTemplateRegex("about").Match(articleText).Value, 2).Length > 0)
            {
                foreach (Match m in Tools.NestedTemplateRegex("for").Matches(articleText))
                {
                    string about = Tools.NestedTemplateRegex("about").Match(articleText).Value;
                    
                    // about supports up to 9 arguments
                    if (Tools.GetTemplateArgument(about, 9).Length > 0)
                        continue;
                    
                    string extra = "";

                    // where about has 2 arguments need extra pipe
                    if (Tools.GetTemplateArgument(Tools.NestedTemplateRegex("about").Match(articleText).Value, 3).Length == 0
                        && Tools.GetTemplateArgument(Tools.NestedTemplateRegex("about").Match(articleText).Value, 4).Length == 0)
                        extra = @"|";

                    // append {{for}} value to the {{about}}
                    if (Tools.GetTemplateArgument(m.Value, 3).Length == 0)
                        articleText = articleText.Replace(about, about.TrimEnd('}') + extra + m.Groups[3].Value);
                    else if  (Tools.GetTemplateArgument(m.Value, 4).Length == 0) // where for has 3 arguments need extra and
                        articleText = articleText.Replace(about, about.TrimEnd('}') + extra + m.Groups[3].Value.Insert(m.Groups[3].Value.LastIndexOf('|') + 1, "and|"));
    
                    // if there are 4 arguments do nothing
                    // remove the old {{for}}
                    if  (Tools.GetTemplateArgument(m.Value, 4).Length == 0)
	                    articleText = articleText.Replace(m.Value, "");
                }

                // if for with blank first argument copied over then now need to put "other uses" as the argment
                articleText = Tools.NestedTemplateRegex("about").Replace(articleText, m2 => {
                                                                             string res = m2.Value;
                                                                             if(Tools.GetTemplateArgument(res, 7).Length > 0 && Tools.GetTemplateArgument(res, 6).Length == 0)
                                                                             {
                                                                                 res = res.Insert(Tools.GetTemplateArgumentIndex(res, 6), "other uses");
                                                                             }
                                                                             return res;
                                                                         });
            }

            // non-mainspace links need escaping in {{about}}
            foreach (Match m in Tools.NestedTemplateRegex("about").Matches(articleText))
            {
                string aboutcall = m.Value;
                for (int a = 1; a <= Tools.GetTemplateArgumentCount(m.Value); a++)
                {
                    string arg = Tools.GetTemplateArgument(aboutcall, a);
                    if (arg.Length > 0 && Namespace.Determine(arg) != Namespace.Mainspace)
                        aboutcall = aboutcall.Replace(arg, @":" + arg);
                }

                if (!m.Value.Equals(aboutcall))
                    articleText = articleText.Replace(m.Value, aboutcall);
            }

            // multiple {{distinguish}} into one
            oldArticleText = "";
            while (oldArticleText != articleText)
            {
                oldArticleText = articleText;
                bool doneDistinguishMerge = false;
                foreach (Match m in Tools.NestedTemplateRegex("distinguish").Matches(articleText))
                {
                    foreach (Match m2 in Tools.NestedTemplateRegex("distinguish").Matches(articleText))
                    {
                        if (m2.Value.Equals(m.Value))
                            continue;

                        articleText = articleText.Replace(m.Value, m.Value.TrimEnd('}') + m2.Groups[3].Value);

                        doneDistinguishMerge = true;
                        articleText = articleText.Replace(m2.Value, "");
                        break;
                    }

                    if (doneDistinguishMerge)
                        break;
                }
            }

            return (articleText + restOfArticle);
        }

        private static readonly List<string> SectionMergedTemplates = new List<string>(new[] { "see also", "see also2", "main" });
        private static readonly Regex SectionMergedTemplatesR = Tools.NestedTemplateRegex(SectionMergedTemplates);

        /// <summary>
        /// Merges multiple instances of the same template in the same section
        /// </summary>
        /// <param name="articleText">The article text</param>
        /// <returns>The updated article text</returns>
        public static string MergeTemplatesBySection(string articleText)
        {
            string[] articleTextInSections = Tools.SplitToSections(articleText);
            StringBuilder newArticleText = new StringBuilder();

            foreach(string s in articleTextInSections)
            {
                string sectionText = s;
                foreach (string t in SectionMergedTemplates)
                {
                    if(SectionMergedTemplatesR.Matches(sectionText).Count < 2)
                        break;

                    sectionText = MergeTemplates(sectionText, t);
                }
                newArticleText.Append(sectionText);
            }

            return newArticleText.ToString().TrimEnd();
        }

        /// <summary>
        /// Merges all instances of the given template in the given section of the article, only when templates at top of section
        /// </summary>
        /// <param name="sectionText">The article section text</param>
        /// <param name="templateName">The template to merge</param>
        /// <returns>The updated article section text</returns>
        private static string MergeTemplates(string sectionText, string templateName)
        {
            if (!Variables.LangCode.Equals("en"))
                return sectionText;

            string sectionTextOriginal = sectionText;
            Regex TemplateToMerge = Tools.NestedTemplateRegex(templateName);
            string mergedTemplates = "";

            // unless it's zeroth section must remove heading to have templates at start of section
            string heading = "";
            Match h = WikiRegexes.Headings.Match(sectionText);

            if(h.Success && h.Index == 0)
            {
                heading = h.Value.TrimEnd();
                sectionText = sectionText.Substring(h.Length).TrimStart();
            }

            int merged = 0;

            while(TemplateToMerge.IsMatch(sectionText))
            {
                Match m = TemplateToMerge.Match(sectionText);
                
                // only take templates at very start of section (after heading)
                if(m.Index > 0)
                    break;
                
                // if first template just append, if subsequent then merge in value
                if(mergedTemplates.Length == 0)
                    mergedTemplates = m.Value;
                else
                {
                    mergedTemplates = mergedTemplates.Replace(@"}}", m.Groups[3].Value);
                    merged++;
                }
                
                // reove template from section text
                sectionText = sectionText.Substring(m.Length).TrimStart();
            }

            // recompose section: only if a merge has happened
            if(merged > 0)
                return ((heading.Length > 0 ? heading + "\r\n" : "") + mergedTemplates + "\r\n" + sectionText);

            return sectionTextOriginal;
        }
	}
}
