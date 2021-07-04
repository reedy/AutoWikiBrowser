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

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WikiFunctions.Parse
{

    /// <summary>
    /// Provides functions for editing wiki text, such as formatting and re-categorisation.
    /// </summary>
    public partial class Parsers
    {
        #region config parameters
        private const int MinCleanupTagsToCombine = 2; // article must have at least this many tags to combine to {{multiple issues}}
        #endregion

        #region current style multiple issues
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

            if (hasMI)
            {
                // use cached list of template calls for performance checks
                List<string> alltemplatesD = GetAllTemplateDetail(articleText).FindAll(t => t.Contains(" issues"));

                // Remove multiple issues with zero tags, fix excess newlines
                if(alltemplatesD.Any(t => t != WikiRegexes.MultipleIssues.Replace(t, MultipleIssuesZeroTag)))
                    articleText = WikiRegexes.MultipleIssues.Replace(articleText, MultipleIssuesZeroTag);
            }

            int sectionTags = TemplateCount(alltemplates, WikiRegexes.MultipleIssuesSectionMaintenanceTemplates);

            if (hasMI || TemplateCount(alltemplates, WikiRegexes.MultipleIssuesArticleMaintenanceTemplates) > 1 || 
                sectionTags > 1)
            {
                // get sections
                string[] sections = Tools.SplitToSections(articleText);
                StringBuilder newarticleText = new StringBuilder();

                foreach(string s in sections)
                {
                    if (!s.StartsWith("="))
                    {
                        string newZero = MIZerothSection(s, WikiRegexes.MultipleIssuesArticleMaintenanceTemplates);

                        // if no changes in zeroth section and no section tags, then nothing left to do
                        if(newZero.Equals(s) && sectionTags == 0)
                            return articleText;

                        newarticleText.Append(newZero);
                    }
                    else if(sectionTags > 0)
                        newarticleText.Append(MILaterSection(s, WikiRegexes.MultipleIssuesSectionMaintenanceTemplates).TrimStart());
                    else
                        newarticleText.Append(s);
                }

                return newarticleText.ToString().TrimEnd();
            }

            return articleText;
        }

        /// <summary>
        /// Merges multiple MI templates in zeroth section, if possible
        /// Converts multiple issues with one issue -> single issue tag
        /// Puts other maintenance tags inside existing MI, or adds MI if two or more tags
        /// </summary>
        /// <returns>The zeroth section.</returns>
        /// <param name="zerothsection">Zerothsection.</param>
        /// <param name="Templates">Templates.</param>
        private string MIZerothSection(string zerothsection, Regex Templates)
        {
            // look for maintenance templates
            // cannot support more than one multiple issues template per section
            MatchCollection MIMC = WikiRegexes.MultipleIssues.Matches(zerothsection);
            bool existingMultipleIssues = MIMC.Count > 0;

            if (MIMC.Count > 1)
            {
                zerothsection = MergeMultipleMI(zerothsection);

                MIMC = WikiRegexes.MultipleIssues.Matches(zerothsection);
                existingMultipleIssues = MIMC.Count > 0;
                if (MIMC.Count > 1)
                    return zerothsection;
            }

            string zerothsectionNoMI = Tools.ReplaceWithSpaces(zerothsection, MIMC);

            // count distinct templates
            int totalTemplates = Tools.DeduplicateList((from Match m in Templates.Matches(zerothsectionNoMI) select m.Value).ToList()).Count();

            // multiple issues with one issue -> single issue tag (old style or new style multiple issues)
            if (totalTemplates == 0 && existingMultipleIssues)
            {
                zerothsection = WikiRegexes.MultipleIssues.Replace(zerothsection, MultipleIssuesDeDupe);
                return WikiRegexes.MultipleIssues.Replace(zerothsection, MultipleIssuesSingleTagME);
            }
            
            // if currently no {{Multiple issues}} and less than the min number of maintenance templates, do nothing
            if (!existingMultipleIssues && (totalTemplates < MinCleanupTagsToCombine))
                return zerothsection;

            // if currently has {{Multiple issues}}, add tags to it (new style only), otherwise insert multiple issues with tags.
            // multiple issues with some old style tags would have new style added
            
            if (!existingMultipleIssues)
                zerothsection = @"{{Multiple issues}}" + "\r\n" + zerothsection;
            
            // add each distinct template to MI
            List<string> miTemplates = Tools.DeduplicateList((from Match m in Templates.Matches(zerothsectionNoMI) select m.Value).ToList());
            foreach(string t in miTemplates)
            {
                // remove tag and any trailing spaces
                zerothsection = Regex.Replace(zerothsection, Regex.Escape(t) + " *", "");
                string MI = WikiRegexes.MultipleIssues.Match(zerothsection).Value;
                bool newstyleMI = WikiRegexes.NestedTemplates.IsMatch(MI.Substring(2));
                zerothsection = zerothsection.Replace(MI, Regex.Replace(MI, @"\s*}}$", (newstyleMI ? "" : "|") + "\r\n" + t + "\r\n}}"));
            }

            // clean up again in case of duplicate tags
            zerothsection = WikiRegexes.MultipleIssues.Replace(zerothsection, MultipleIssuesDeDupe);
            return WikiRegexes.MultipleIssues.Replace(zerothsection, MultipleIssuesSingleTagME);
        }

        /// <summary>
        /// Works on non-zeroth section of article
        /// Converts multiple issues with one issue -> single issue tag
        /// Puts other section maintenance tags inside existing MI, or adds MI if two or more section maintenance tags
        /// </summary>
        /// <returns>The  section.</returns>
        /// <param name="section">section.</param>
        /// <param name="Templates">Templates.</param>
        private string MILaterSection(string section, Regex Templates)
        {
            string sectionOriginal = section;
            MatchCollection MIMC = WikiRegexes.MultipleIssues.Matches(section);
            bool existingMultipleIssues = MIMC.Count > 0;
            
            // look for maintenance templates
            // cannot support more than one multiple issues template per section
            if (MIMC.Count > 1)
                return sectionOriginal;
            
            if (existingMultipleIssues)
                section = Tools.ReplaceWithSpaces(section, MIMC);
            
            int templatePortion = 0;

            if (WikiRegexes.NestedTemplates.IsMatch(section))
                templatePortion = Tools.HowMuchStartsWith(section, Templates, true);

            // multiple issues with one issue -> single issue tag (old style or new style multiple issues)
            if (templatePortion == 0)
            {
                if (existingMultipleIssues)
                    return WikiRegexes.MultipleIssues.Replace(sectionOriginal, MultipleIssuesSingleTagME);
                return sectionOriginal;
            }

            string heading = WikiRegexes.Headings.Match(section).Value.Trim(),
            sectionPortion = section.Substring(0, templatePortion),
            sectionPortionOriginal = sectionOriginal.Substring(0, templatePortion),
            sectionRest = sectionOriginal.Substring(templatePortion);
            
            int totalTemplates = Templates.Matches(sectionPortion).Count;
            
            // if currently no {{Multiple issues}} and less than the min number of maintenance templates, do nothing
            if (!existingMultipleIssues && (totalTemplates < MinCleanupTagsToCombine))
                return sectionOriginal;

            // if currently has {{Multiple issues}}, add tags to it (new style only), otherwise insert multiple issues with tags. multiple issues with some old style tags would have new style added
            string newsection;
            
            if (existingMultipleIssues) // add each template to MI
            {
                newsection = WikiRegexes.MultipleIssues.Match(sectionPortionOriginal).Value;

                if (newsection.Length > 0)
                {
                    bool newstyleMI = WikiRegexes.NestedTemplates.IsMatch(Tools.GetTemplateArgument(Tools.RemoveTemplateParameter(newsection, "section"), 1));
                    
                    foreach(Match m in Templates.Matches(sectionPortion))
                    {
                        if (!newsection.Contains(m.Value))
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
                if (tags.Contains("{{") || tags.Equals(""))
                {
                    mi +=tags;
                    return "";
                }

                return m.Value;
            });

            // do nothing if found a MI section template
            if (WikiRegexes.MultipleIssues.IsMatch(articleText))
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
            string res = m.Value;
            string newValue = Tools.RemoveTemplateParameter(Tools.RemoveExcessTemplatePipes(m.Value), "section");

            if (Tools.GetTemplateArgumentCount(newValue) == 1 && WikiRegexes.NestedTemplates.Matches (Tools.GetTemplateArgument (newValue, 1)).Count == 1)
            {
                res = Tools.GetTemplateArgument(newValue, 1);

                // template may have 1= parameter, remove
                if(res.StartsWith("1"))
                    res = Regex.Replace(res, @"^1\s*=\s*", "");
            }
            
            return res;
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

            // only make change if some duplicate templates removed
            if (Regex.Matches(newTags, "{{").Count < Regex.Matches(tags, "{{").Count)
                newValue = newValue.Replace(tags, newTags);

            return newValue;
        }

        /// <summary>
        /// Removes multiple issues with zero tags, fix excess newlines
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
        #endregion
    }
}