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
        private const int MinCleanupTagsToCombine = 2; // article must have at least this many tags to combine to {{multiple issues}}
        private static readonly Regex ExpertSubject = Tools.NestedTemplateRegex("expert-subject");

        /// <summary>
        /// Combines multiple cleanup tags into {{multiple issues}} template, ensures parameters have correct case, removes date parameter where not needed
        /// only for English-language wikis
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public string MultipleIssuesOld(string articleText)
        {
            if (!Variables.LangCode.Equals("en"))
                return articleText;

            // convert title case parameters within {{Multiple issues}} to lower case
            foreach (Match m in WikiRegexes.MultipleIssuesInTitleCase.Matches(articleText))
            {
                string firstPart = m.Groups[1].Value;
                string parameterFirstChar = m.Groups[2].Value.ToLower();
                string lastPart = m.Groups[3].Value;

                articleText = articleText.Replace(m.Value, firstPart + parameterFirstChar + lastPart);
            }

            // remove any date field within  {{Multiple issues}} if no 'expert=subject' field using it
            string MICall = WikiRegexes.MultipleIssues.Match(articleText).Value;
            if (MICall.Length > 10 && (Tools.GetTemplateParameterValue(MICall, "expert").Length == 0 ||
                                       MonthYear.IsMatch(Tools.GetTemplateParameterValue(MICall, "expert"))))
                articleText = articleText.Replace(MICall, Tools.RemoveTemplateParameter(MICall, "date"));

            // get the zeroth section (text upto first heading)
            string zerothSection = Tools.GetZerothSection(articleText);

            // get the rest of the article including first heading (may be null if entire article falls in zeroth section)
            string restOfArticle = articleText.Substring(zerothSection.Length);
            string ESDate = "";

            if (ExpertSubject.IsMatch(zerothSection))
            {
                ESDate = Tools.GetTemplateParameterValue(ExpertSubject.Match(zerothSection).Value, "date");
                zerothSection = Tools.RemoveTemplateParameter(zerothSection, "expert-subject", "date");
            }

            int tagsToAdd = WikiRegexes.MultipleIssuesTemplates.Matches(zerothSection).Count;

            // if currently no {{Multiple issues}} and less than the min number of cleanup templates, do nothing
            if (!WikiRegexes.MultipleIssues.IsMatch(zerothSection) && WikiRegexes.MultipleIssuesTemplates.Matches(zerothSection).Count < MinCleanupTagsToCombine)
            {
                // article issues with one issue -> single issue tag (e.g. {{multiple issues|cleanup=January 2008}} to {{cleanup|date=January 2008}} etc.)
                articleText = WikiRegexes.MultipleIssues.Replace(articleText, MultipleIssuesOldSingleTagME);

                return MultipleIssuesBLPUnreferenced(articleText);
            }

            // only add tags to multiple issues if new tags + existing >= MinCleanupTagsToCombine
            MICall = Tools.RenameTemplateParameter(WikiRegexes.MultipleIssues.Match(zerothSection).Value, "OR", "original research");

            if ((WikiRegexes.MultipleIssuesTemplateNameRegex.Matches(MICall).Count + tagsToAdd) < MinCleanupTagsToCombine || tagsToAdd == 0)
            {
                // article issues with one issue -> single issue tag (e.g. {{multiple issues|cleanup=January 2008}} to {{cleanup|date=January 2008}} etc.)
                articleText = WikiRegexes.MultipleIssues.Replace(articleText, (MultipleIssuesOldSingleTagME));

                return MultipleIssuesBLPUnreferenced(articleText);
            }

            string newTags = "";

            foreach (Match m in WikiRegexes.MultipleIssuesTemplates.Matches(zerothSection))
            {
                // all fields except COI, OR, POV and ones with BLP should be lower case
                string singleTag = m.Groups[1].Value;
                string tagValue = m.Groups[2].Value;
                if (!WikiRegexes.CoiOrPovBlp.IsMatch(singleTag))
                    singleTag = singleTag.ToLower();

                string singleTagLower = singleTag.ToLower();

                // tag renaming
                if (singleTagLower.Equals("cleanup-rewrite"))
                    singleTag = "rewrite";
                else if (singleTagLower.Equals("cleanup-laundry"))
                    singleTag = "laundrylists";
                else if (singleTagLower.Equals("cleanup-jargon"))
                    singleTag = "jargon";
                else if (singleTagLower.Equals("primary sources"))
                    singleTag = "primarysources";
                else if (singleTagLower.Equals("news release"))
                    singleTag = "newsrelease";
                else if (singleTagLower.Equals("game guide"))
                    singleTag = "gameguide";
                else if (singleTagLower.Equals("travel guide"))
                    singleTag = "travelguide";
                else if (singleTagLower.Equals("very long"))
                    singleTag = "verylong";
                else if (singleTagLower.Equals("cleanup-reorganise"))
                    singleTag = "restructure";
                else if (singleTagLower.Equals("cleanup-reorganize"))
                    singleTag = "restructure";
                else if (singleTagLower.Equals("cleanup-spam"))
                    singleTag = "spam";
                else if (singleTagLower.Equals("criticism section"))
                    singleTag = "criticisms";
                else if (singleTagLower.Equals("pov-check"))
                    singleTag = "pov-check";
                else if (singleTagLower.Equals("expert-subject"))
                    singleTag = "expert";

                // copy edit|for=grammar --> grammar
                if (singleTag.Replace(" ", "").Equals("copyedit") && Tools.GetTemplateParameterValue(m.Value, "for").Equals("grammar"))
                {
                    singleTag = "grammar";
                    tagValue = Regex.Replace(tagValue, @"for\s*=\s*grammar\s*\|?", "");
                }

                // expert must have a parameter
                if (singleTag == "expert" && tagValue.Trim().Length == 0)
                    continue;

                // for tags with a parameter, that parameter must be the date
                if ((tagValue.Contains("=") && Regex.IsMatch(tagValue, @"(?i)date")) || tagValue.Length == 0 || singleTag == "expert")
                {
                    tagValue = Regex.Replace(tagValue, @"^[Dd]ate\s*=\s*", "= ");

                    // every tag except expert needs a date
                    if (!singleTag.Equals("expert") && tagValue.Length == 0)
                        tagValue = @"= {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}";
                    else if (!tagValue.Contains(@"="))
                        tagValue = @"= " + tagValue;

                    // don't add duplicate tags
                    if (MICall.Length == 0 || Tools.GetTemplateParameterValue(MICall, singleTag).Length == 0)
                        newTags += @"|" + singleTag + @" " + tagValue;
                }
                else
                    continue;

                newTags = newTags.Trim();

                // remove the single template
                zerothSection = zerothSection.Replace(m.Value, "");
            }

            if (ESDate.Length > 0)
                newTags += ("|date = " + ESDate);

            // if article currently has {{Multiple issues}}, add tags to it
            string ai = WikiRegexes.MultipleIssues.Match(zerothSection).Value;
            if (ai.Length > 0)
                zerothSection = zerothSection.Replace(ai, ai.Substring(0, ai.Length - 2) + newTags + @"}}");

            else // add {{Multiple issues}} to top of article, metaDataSorter will arrange correctly later
                zerothSection = @"{{Multiple issues" + newTags + "}}\r\n" + zerothSection;

            articleText = zerothSection + restOfArticle;

            // Conversions() will add any missing dates and correct ...|wikify date=May 2008|...
            return MultipleIssuesBLPUnreferenced(articleText);
        }

        /// <summary>
        /// Converts old-style multiple issues with one issue to stand alone tag
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private string MultipleIssuesOldSingleTagME(Match m)
        {
            // Performance: nothing to do if no named parameters
            if(Tools.GetTemplateParameterValues(m.Value).Count == 0)
                return m.Value;

            string newValue = Conversions(Tools.RemoveTemplateParameter(m.Value, "section"));

            if (Tools.GetTemplateArgumentCount(newValue) == 1 && !WikiRegexes.NestedTemplates.IsMatch(Tools.GetTemplateArgument(newValue, 1)))
            {
                string single = Tools.GetTemplateArgument(newValue, 1);

                if (single.Contains(@"="))
                    newValue = @"{{" + single.Substring(0, single.IndexOf("=")).Trim() + @"|date=" + single.Substring(single.IndexOf("=") + 1).Trim() + @"}}";
            }
            else return m.Value;

            return newValue;
        }

        private static readonly Regex MultipleIssuesDate = new Regex(@"(?<={{\s*(?:[Aa]rticle|[Mm]ultiple) ?issues\s*(?:\|[^{}]*?)?(?:{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}[^{}]*?){0,4}\|[^{}\|]{3,}?)\b(?i)date(?<!.*out of.*)", RegexOptions.Compiled);

        /// <summary>
        /// In the {{Multiple issues}} template renames unref to BLPunref for living person bio articles
        /// </summary>
        /// <param name="articleText">The page text</param>
        /// <returns>The updated page text</returns>
        private string MultipleIssuesBLPUnreferenced(string articleText)
        {
            articleText = MultipleIssuesDate.Replace(articleText, "");

            if (WikiRegexes.MultipleIssues.IsMatch(articleText))
            {
                string aiat = WikiRegexes.MultipleIssues.Match(articleText).Value;

                // unref to BLPunref for living person bio articles
                if (Tools.GetTemplateParameterValue(aiat, "unreferenced").Length > 0 && articleText.Contains(CategoryLivingPeople))
                    articleText = articleText.Replace(aiat, Tools.RenameTemplateParameter(aiat, "unreferenced", "BLP unsourced"));
                else if (Tools.GetTemplateParameterValue(aiat, "unref").Length > 0 && articleText.Contains(CategoryLivingPeople))
                    articleText = articleText.Replace(aiat, Tools.RenameTemplateParameter(aiat, "unref", "BLP unsourced"));

                articleText = MetaDataSorter.MoveMaintenanceTags(articleText);
            }

            return articleText;
        }

        /// <summary>
        /// Performs cleanup on old-style multiple issues templates:
        /// convert title case parameters within {{Multiple issues}} to lower case
        /// remove any date field within  {{Multiple issues}} if no 'expert=subject' field using it
        /// </summary>
        /// <param name="articleText"></param>
        /// <returns></returns>
        private static string MultipleIssuesOldCleanup(string articleText)
        {
            // old-format cleanup: convert title case parameters within {{Multiple issues}} to lower case
            foreach (Match m in WikiRegexes.MultipleIssuesInTitleCase.Matches(articleText))
            {
                string firstPart = m.Groups[1].Value;
                string parameterFirstChar = m.Groups[2].Value.ToLower();
                string lastPart = m.Groups[3].Value;

                articleText = articleText.Replace(m.Value, firstPart + parameterFirstChar + lastPart);
            }

            // old-format cleanup: remove any date field within  {{Multiple issues}} if no 'expert=subject' field using it
            string MICall = WikiRegexes.MultipleIssues.Match(articleText).Value;
            if (MICall.Length > 10 && (Tools.GetTemplateParameterValue(MICall, "expert").Length == 0 ||
                                       MonthYear.IsMatch(Tools.GetTemplateParameterValue(MICall, "expert"))))
                articleText = articleText.Replace(MICall, Tools.RemoveTemplateParameter(MICall, "date"));

            return articleText;
        }

        /// <summary>
        /// Combines maintenance tags into {{multiple issues}} template, for en-wiki only
        /// Operates on a section by section basis through article text
        /// </summary>
        /// <param name="articleText"></param>
        /// <returns></returns>
        public string MultipleIssues(string articleText)
        {
            // en wiki only
            if (!Variables.LangCode.Equals("en"))
                return articleText;

            // Performance: get all the templates, only apply multiple issues functions if relevant templates found
            List<string> alltemplates = GetAllTemplates(articleText);
            bool hasMI = TemplateExists(alltemplates, WikiRegexes.MultipleIssues);

            if(hasMI)
            {
                articleText = MultipleIssuesOldCleanup(articleText);

                // Remove multiple issues with zero tags, fix excess newlines
                articleText = WikiRegexes.MultipleIssues.Replace(articleText, MultipleIssuesZeroTag);
            }

            if(hasMI || TemplateCount(alltemplates, WikiRegexes.MultipleIssuesArticleMaintenanceTemplates) > 1 || 
                TemplateCount(alltemplates, WikiRegexes.MultipleIssuesSectionMaintenanceTemplates) > 1)
            {
                // get sections
                string[] sections = Tools.SplitToSections(articleText);
                StringBuilder newarticleText = new StringBuilder();

                foreach(string s in sections)
                {
                    if(!s.StartsWith("="))
                        newarticleText.Append(MIZerothSection(s, WikiRegexes.MultipleIssuesArticleMaintenanceTemplates));
                    else
                        newarticleText.Append(MILaterSection(s, WikiRegexes.MultipleIssuesSectionMaintenanceTemplates).TrimStart());
                }

                return newarticleText.ToString().TrimEnd();
            }

            return articleText;
        }

        private string MIZerothSection(string zerothsection, Regex Templates)
        {
            // look for maintenance templates
            // cannot support more than one multiple issues template per section
            MatchCollection MIMC = WikiRegexes.MultipleIssues.Matches(zerothsection);
            bool existingMultipleIssues = MIMC.Count > 0;

            if(MIMC.Count > 1)
            {
                zerothsection = MergeMultipleMI(zerothsection);

                MIMC = WikiRegexes.MultipleIssues.Matches(zerothsection);
                existingMultipleIssues = MIMC.Count > 0;
                if(MIMC.Count > 1)
                    return zerothsection;
            }

            string zerothsectionNoMI = Tools.ReplaceWithSpaces(zerothsection, MIMC);

            // count distinct templates
            int totalTemplates = Tools.DeduplicateList((from Match m in Templates.Matches(zerothsectionNoMI) select m.Value).ToList()).Count();

            // multiple issues with one issue -> single issue tag (old style or new style multiple issues)
            if(totalTemplates == 0 && existingMultipleIssues)
            {
                zerothsection = WikiRegexes.MultipleIssues.Replace(zerothsection, MultipleIssuesOldSingleTagME);
                zerothsection = WikiRegexes.MultipleIssues.Replace(zerothsection, MultipleIssuesDeDupe);
                return WikiRegexes.MultipleIssues.Replace(zerothsection, MultipleIssuesSingleTagME);
            }
            
            // if currently no {{Multiple issues}} and less than the min number of maintenance templates, do nothing
            if(!existingMultipleIssues && (totalTemplates < MinCleanupTagsToCombine))
                return zerothsection;

            // if currently has {{Multiple issues}}, add tags to it (new style only), otherwise insert multiple issues with tags.
            // multiple issues with some old style tags would have new style added
            
            if(!existingMultipleIssues)
                zerothsection = @"{{Multiple issues}}" + "\r\n" + zerothsection;
            
            // add each template to MI
            foreach(Match m in Templates.Matches(zerothsectionNoMI))
            {
                zerothsection = zerothsection.Replace(m.Value, "");
                string MI = WikiRegexes.MultipleIssues.Match(zerothsection).Value;
                bool newstyleMI = WikiRegexes.NestedTemplates.IsMatch(MI.Substring(2));
                zerothsection = zerothsection.Replace(MI, Regex.Replace(MI, @"\s*}}$", (newstyleMI ? "" : "|") + "\r\n" + m.Value + "\r\n}}"));
            }

            // clean up again in case of duplicate tags
            zerothsection = WikiRegexes.MultipleIssues.Replace(zerothsection, MultipleIssuesDeDupe);
            return WikiRegexes.MultipleIssues.Replace(zerothsection, MultipleIssuesSingleTagME);
        }

        private string MILaterSection(string section, Regex Templates)
        {
            string sectionOriginal = section;
            MatchCollection MIMC = WikiRegexes.MultipleIssues.Matches(section);
            bool existingMultipleIssues = MIMC.Count > 0;
            
            // look for maintenance templates
            // cannot support more than one multiple issues template per section
            if(MIMC.Count > 1)
                return sectionOriginal;
            
            if(existingMultipleIssues)
                section = Tools.ReplaceWithSpaces(section, MIMC);
            
            int templatePortion = 0;

            if(WikiRegexes.NestedTemplates.IsMatch(section))
                templatePortion = Tools.HowMuchStartsWith(section, Templates, true);

            // multiple issues with one issue -> single issue tag (old style or new style multiple issues)
            if(templatePortion == 0)
            {
                if(existingMultipleIssues)
                {
                    sectionOriginal = WikiRegexes.MultipleIssues.Replace(sectionOriginal, MultipleIssuesOldSingleTagME);
                    return WikiRegexes.MultipleIssues.Replace(sectionOriginal, MultipleIssuesSingleTagME);
                }
                return sectionOriginal;
            }

            string heading = WikiRegexes.Headings.Match(section).Value.Trim(),
            sectionPortion = section.Substring(0, templatePortion),
            sectionPortionOriginal = sectionOriginal.Substring(0, templatePortion),
            sectionRest = sectionOriginal.Substring(templatePortion);
            
            int totalTemplates = Templates.Matches(sectionPortion).Count;
            
            // if currently no {{Multiple issues}} and less than the min number of maintenance templates, do nothing
            if(!existingMultipleIssues && (totalTemplates < MinCleanupTagsToCombine))
                return sectionOriginal;

            // if currently has {{Multiple issues}}, add tags to it (new style only), otherwise insert multiple issues with tags. multiple issues with some old style tags would have new style added
            string newsection;
            
            if(existingMultipleIssues) // add each template to MI
            {
                newsection = WikiRegexes.MultipleIssues.Match(sectionPortionOriginal).Value;

                if(newsection.Length > 0)
                {
                    bool newstyleMI = WikiRegexes.NestedTemplates.IsMatch(Tools.GetTemplateArgument(Tools.RemoveTemplateParameter(newsection, "section"), 1));
                    
                    foreach(Match m in Templates.Matches(sectionPortion))
                    {
                        if(!newsection.Contains(m.Value))
                            newsection = newsection.Replace(newsection, Regex.Replace(newsection, @"\s*}}$", (newstyleMI ? "" : "|") + "\r\n" + m.Value + "\r\n}}"));
                    }
                }
                else // MI template later in section, other text in between, cannot do anything with this
                    return sectionOriginal;
            }
            else // create new MI and add each template
            {
                newsection = "{{Multiple issues|section=yes|\r\n";
                
                foreach(Match m in Templates.Matches(sectionPortion))
                    newsection += (m.Value + "\r\n");
                
                newsection += "}}";
            }
            
            return heading + "\r\n" + newsection + "\r\n" + sectionRest;
        }

        /// <summary>
        /// Merge multiple {{multiple issues}} templates in zeroth section into one
        /// </summary>
        /// <returns>The M.</returns>
        /// <param name="articleText">Article text.</param>
        private string MergeMultipleMI(string articleText)
        {
            string originalArticleText = articleText, mi = "";

            articleText = WikiRegexes.MultipleIssues.Replace(articleText, m =>
            {
                string tags = Tools.GetTemplateArgument(m.Value, 1);

                // do not process if a MI section template
                if(tags.Contains("{{") || tags.Equals(""))
                {
                    mi +=tags;
                    return "";
                }

                return m.Value;
            });

            // do nothing if found a MI section template
            if(WikiRegexes.MultipleIssues.IsMatch(articleText))
                return originalArticleText;

            // extract and de-duplicate tags
            List<string> miTags = Parsers.DeduplicateMaintenanceTags((from Match m in WikiRegexes.NestedTemplates.Matches(mi)
                                                                  select m.Value).ToList());

            mi = string.Join("\r\n", miTags.ToArray());

            return @"{{Multiple issues|" + "\r\n" + mi + "\r\n" + @"}}" + articleText;
        }
        
        /// <summary>
        /// Converts new-style multiple issues template with one issue to standalone tag
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private string MultipleIssuesSingleTagME(Match m)
        {
            string newValue = Tools.RemoveTemplateParameter(Tools.RemoveExcessTemplatePipes(m.Value), "section");

            if (Tools.GetTemplateArgumentCount(newValue) == 1 && WikiRegexes.NestedTemplates.Matches(Tools.GetTemplateArgument(newValue, 1)).Count == 1)
                return Tools.GetTemplateArgument(newValue, 1);
            
            return m.Value;
        }

        /// <summary>
        /// Deduplicates tags in multipleissues template calls (not section templates)
        /// </summary>
        /// <returns>The revised multipleissues template call </returns>
        /// <param name="m"></param>
        private string MultipleIssuesDeDupe(Match m)
        {
            string newValue = m.Value;

            string tags = Tools.GetTemplateArgument(newValue, 1);

            List<string> tagValues = Parsers.DeduplicateMaintenanceTags((from Match n in WikiRegexes.NestedTemplates.Matches(tags)
                select n.Value).ToList());

            string newTags = string.Join("\r\n", tagValues.ToArray());

            if(Regex.Matches(newTags, "{{").Count < Regex.Matches(tags, "{{").Count)
                newValue = newValue.Replace(tags, newTags);

            return newValue;
        }

        /// <summary>
        /// Removes multiple issues template with zero tags
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private string MultipleIssuesZeroTag(Match m)
        {
            string newValue = Tools.RemoveTemplateParameter(Tools.RemoveExcessTemplatePipes(m.Value), "section");

            if (Tools.GetTemplateArgumentCount(newValue) == 0)
                return "";

            // clean excess newlines
            return Regex.Replace(m.Value, "(\r\n)+", "\r\n");
        }
    }
}