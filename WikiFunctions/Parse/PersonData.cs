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
        /// Matches the {{birth date}} family of templates
        /// </summary>
        private static readonly Regex BirthDate = Tools.NestedTemplateRegex(new[] { "birth date", "birth-date", "dob", "bda", "birth date and age", "birthdate and age", "Date of birth and age", "BDA", "Birthdateandage",
                                                                                "Birth Date and age", "birthdate" }, true);

        /// <summary>
        /// Matches the {{death  date}} family of templates
        /// </summary>
        private static readonly Regex DeathDate = Tools.NestedTemplateRegex(new[] { "death date", "death-date", "dda", "death date and age", "deathdateandage", "deathdate" }, true);
        private static List<string> PersonDataLowerCaseParameters = new List<string>(new[] { "name", "alternative names", "short description", "date of birth", "place of birth", "date of death", "place of death" });

        /// <summary>
        /// * Adds the default {{persondata}} template to en-wiki mainspace pages about a person that don't already have {{persondata}}
        /// * Attempts to complete blank {{persondata}} fields based on infobox values
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">Title of the article</param>
        /// <returns></returns>
        public static string PersonData(string articleText, string articleTitle)
        {
            int personDataCount = WikiRegexes.Persondata.Matches(articleText).Count;
            if (!Variables.IsWikipediaEN
                || personDataCount > 1
                || (articleText.Contains("{{Persondata") && personDataCount == 0)) // skip in case of existing persondata with unbalanced brackets
                return articleText;

            string originalPersonData, newPersonData;

            // add default persondata if missing
            if (personDataCount == 0)
            {
                if (IsArticleAboutAPerson(articleText, articleTitle, true))
                {
                    articleText = articleText + Tools.Newline(WikiRegexes.PersonDataDefault);
                    newPersonData = originalPersonData = WikiRegexes.PersonDataDefault;
                }
                else
                    return articleText;
            }
            else
            {
                originalPersonData = WikiRegexes.Persondata.Match(articleText).Value;
                newPersonData = originalPersonData;
                // use uppercase parameters if making changes, rename any lowercase ones
                if(Tools.GetTemplateParameterValues(newPersonData).Any(s => PersonDataLowerCaseParameters.Contains(s.Key)))
                {
                    newPersonData = Tools.RenameTemplateParameter(newPersonData, "name", "NAME");
                    newPersonData = Tools.RenameTemplateParameter(newPersonData, "alternative names", "ALTERNATIVE NAMES");
                    newPersonData = Tools.RenameTemplateParameter(newPersonData, "short description", "SHORT DESCRIPTION");
                    newPersonData = Tools.RenameTemplateParameter(newPersonData, "date of birth", "DATE OF BIRTH");
                    newPersonData = Tools.RenameTemplateParameter(newPersonData, "place of birth", "PLACE OF BIRTH");
                    newPersonData = Tools.RenameTemplateParameter(newPersonData, "date of death", "DATE OF DEATH");
                    newPersonData = Tools.RenameTemplateParameter(newPersonData, "place of death", "PLACE OF DEATH");
                }
            }

            // attempt completion of some persondata fields

            // name
            if (Tools.GetTemplateParameterValue(newPersonData, "NAME", true).Length == 0)
            {
                string name = WikiRegexes.Defaultsort.Match(articleText).Groups["key"].Value;
                if (name.Contains(" ("))
                    name = name.Substring(0, name.IndexOf(" ("));

                if (name.Length == 0 && Tools.WordCount(articleTitle) == 1)
                    name = articleTitle;

                if (name.Length == 0)
                    name = Tools.MakeHumanCatKey(articleTitle, articleText);

                if (name.Length > 0)
                {
                    name = Tools.ReAddDiacritics(articleTitle, name);
                    newPersonData = Tools.SetTemplateParameterValue(newPersonData, "NAME", name, true);
                }
            }

            // date of birth
            if (Tools.GetTemplateParameterValue(newPersonData, "DATE OF BIRTH", true).Length == 0)
            {
                newPersonData = SetPersonDataDate(newPersonData, "DATE OF BIRTH", GetInfoBoxFieldValue(articleText, WikiRegexes.InfoBoxDOBFields), articleText);

                // as fallback use year from category
                if(Tools.GetTemplateParameterValue(newPersonData, "DATE OF BIRTH", true).Length == 0)
                {
                    Match m = WikiRegexes.BirthsCategory.Match(articleText);

                    if (m.Success)
                    {
                        string year = m.Value.Replace(@"[[Category:", "").TrimEnd(']');
                        if (Regex.IsMatch(year, @"^\d{3,4} (?:BC )?births(\|.*)?$"))
                            newPersonData = Tools.SetTemplateParameterValue(newPersonData, "DATE OF BIRTH", year.Substring(0, year.IndexOf(" births")), true);
                    }
                }
            }

            // date of death
            if (Tools.GetTemplateParameterValue(newPersonData, "DATE OF DEATH", true).Length == 0)
            {
                newPersonData = SetPersonDataDate(newPersonData, "DATE OF DEATH", GetInfoBoxFieldValue(articleText, WikiRegexes.InfoBoxDODFields), articleText);

                // as fallback use year from category
                if (Tools.GetTemplateParameterValue(newPersonData, "DATE OF DEATH", true).Length == 0)
                {
                    Match m = WikiRegexes.DeathsOrLivingCategory.Match(articleText);

                    if (m.Success)
                    {
                        string year = m.Value.Replace(@"[[Category:", "").TrimEnd(']');
                        if (Regex.IsMatch(year, @"^\d{3,4} deaths(\|.*)?$"))
                            newPersonData = Tools.SetTemplateParameterValue(newPersonData, "DATE OF DEATH", year.Substring(0, year.IndexOf(" deaths")), true);
                    }
                }
            }

            // place of birth
            string ExistingPOB = Tools.GetTemplateParameterValue(newPersonData, "PLACE OF BIRTH", true);
            if (ExistingPOB.Length == 0)
            {
                string POB = GetInfoBoxFieldValue(articleText, WikiRegexes.InfoBoxPOBFields);

                // as fallback look up cityofbirth/countryofbirth
                if (POB.Length == 0)
                {
                    string ib = WikiRegexes.InfoBox.Match(articleText).Value;
                    POB = (Tools.GetTemplateParameterValue(ib, "cityofbirth") + ", " + Tools.GetTemplateParameterValue(ib, "countryofbirth")).Trim(',');
                }

                POB = WikiRegexes.FileNamespaceLink.Replace(POB, "").Trim();

                POB = WikiRegexes.NestedTemplates.Replace(WikiRegexes.Br.Replace(POB, " "), "").Trim();
                POB = WikiRegexes.Small.Replace(WikiRegexes.Refs.Replace(POB, ""), "$1").TrimEnd(',');
                POB = POB.Replace(@"???", "").Trim();

                newPersonData = Tools.SetTemplateParameterValue(newPersonData, "PLACE OF BIRTH", POB, true);
            }

            // place of death
            string ExistingPOD = Tools.GetTemplateParameterValue(newPersonData, "PLACE OF DEATH", true);
            if (ExistingPOD.Length == 0)
            {
                string POD = GetInfoBoxFieldValue(articleText, WikiRegexes.InfoBoxPODFields);

                // as fallback look up cityofbirth/countryofbirth
                if (POD.Length == 0)
                {
                    string ib = WikiRegexes.InfoBox.Match(articleText).Value;
                    POD = (Tools.GetTemplateParameterValue(ib, "cityofdeath") + ", " + Tools.GetTemplateParameterValue(ib, "countryofdeath")).Trim(',');
                }

                POD = WikiRegexes.FileNamespaceLink.Replace(POD, "").Trim();
                POD = WikiRegexes.NestedTemplates.Replace(WikiRegexes.Br.Replace(POD, " "), "").Trim();
                POD = WikiRegexes.Small.Replace(WikiRegexes.Refs.Replace(POD, ""), "$1").TrimEnd(',');
                POD = POD.Replace(@"???", "").Trim();

                newPersonData = Tools.SetTemplateParameterValue(newPersonData, "PLACE OF DEATH", POD, true);
            }

            // look for full dates matching birth/death categories
            newPersonData = Tools.RemoveDuplicateTemplateParameters(CompletePersonDataDate(newPersonData, articleText));

            // merge changes
            if (!newPersonData.Equals(originalPersonData) && newPersonData.Length > originalPersonData.Length)
                articleText = articleText.Replace(originalPersonData, newPersonData);

            // remove any <small> tags in persondata
            articleText = WikiRegexes.Persondata.Replace(articleText, pd => {
                                                             string res= WikiRegexes.Small.Replace(pd.Value, "$1");
                                                             res = SmallStart.Replace(res, "");
                                                             return SmallEnd.Replace(res, "");
                                                         });

            return articleText;
        }

        private static readonly List<string> DfMf = new List<string>(new[] { "df", "mf" });

        /// <summary>
        /// Completes a persondata call with a date of birth/death.
        /// </summary>
        /// <param name="personData"></param>
        /// <param name="field"></param>
        /// <param name="sourceValue"></param>
        /// <param name="articletext"></param>
        /// <returns>The updated persondata call</returns>
        private static string SetPersonDataDate(string personData, string field, string sourceValue, string articletext)
        {
            string dateFound = "";

            if (field.Equals("DATE OF BIRTH") && BirthDate.IsMatch(articletext))
            {
                sourceValue = Tools.RemoveTemplateParameters(BirthDate.Match(articletext).Value, DfMf);
                dateFound = Tools.GetTemplateArgument(sourceValue, 1);

                // first argument is a year, or a full date
                if (dateFound.Length < 5)
                    dateFound += ("-" + Tools.GetTemplateArgument(sourceValue, 2) + "-" + Tools.GetTemplateArgument(sourceValue, 3));
            }
            else if (field.Equals("DATE OF DEATH") && DeathDate.Matches(articletext).Count == 1)
            {
                sourceValue = Tools.RemoveTemplateParameters(DeathDate.Match(articletext).Value, DfMf);
                dateFound = Tools.GetTemplateArgument(sourceValue, 1);
                if (dateFound.Length < 5)
                    dateFound += ("-" + Tools.GetTemplateArgument(sourceValue, 2) + "-" + Tools.GetTemplateArgument(sourceValue, 3));
            }
            else if (WikiRegexes.AmericanDates.IsMatch(sourceValue))
                dateFound = WikiRegexes.AmericanDates.Match(sourceValue).Value;
            else if (WikiRegexes.InternationalDates.IsMatch(sourceValue))
                dateFound = WikiRegexes.InternationalDates.Match(sourceValue).Value;
            else if (WikiRegexes.ISODates.IsMatch(sourceValue))
                dateFound = WikiRegexes.ISODates.Match(sourceValue).Value;

            // if date not found yet, fall back to year/month/day of brith fields or birth date in {{dda}}
            if (dateFound.Length == 0)
            {
                if (field.Equals("DATE OF BIRTH"))
                {
                    if (GetInfoBoxFieldValue(articletext, "yearofbirth").Length > 0)
                        dateFound = (GetInfoBoxFieldValue(articletext, "yearofbirth") + "-" + GetInfoBoxFieldValue(articletext, "monthofbirth") + "-" + GetInfoBoxFieldValue(articletext, "dayofbirth")).Trim('-');
                    else if (GetInfoBoxFieldValue(articletext, "yob").Length > 0)
                        dateFound = (GetInfoBoxFieldValue(articletext, "yob") + "-" + GetInfoBoxFieldValue(articletext, "mob") + "-" + GetInfoBoxFieldValue(articletext, "dob")).Trim('-');
                    else if (WikiRegexes.DeathDateAndAge.IsMatch(articletext))
                    {
                        string dda = Tools.RemoveTemplateParameters(WikiRegexes.DeathDateAndAge.Match(articletext).Value, DfMf);
                        dateFound = (Tools.GetTemplateArgument(dda, 4) + "-" + Tools.GetTemplateArgument(dda, 5) + "-" + Tools.GetTemplateArgument(dda, 6)).Trim('-');
                    }
                    else if (GetInfoBoxFieldValue(articletext, "birthyear").Length > 0)
                        dateFound = (GetInfoBoxFieldValue(articletext, "birthyear") + "-" + GetInfoBoxFieldValue(articletext, "birthmonth") + "-" + GetInfoBoxFieldValue(articletext, "birthday")).Trim('-');
                }
                else if (field.Equals("DATE OF DEATH"))
                {
                    if (GetInfoBoxFieldValue(articletext, "yearofdeath").Length > 0)
                        dateFound = (GetInfoBoxFieldValue(articletext, "yearofdeath") + "-" + GetInfoBoxFieldValue(articletext, "monthofdeath") + "-" + GetInfoBoxFieldValue(articletext, "dayofdeath")).Trim('-');
                    else if (GetInfoBoxFieldValue(articletext, "deathyear").Length > 0)
                        dateFound = (GetInfoBoxFieldValue(articletext, "deathyear") + "-" + GetInfoBoxFieldValue(articletext, "deathmonth") + "-" + GetInfoBoxFieldValue(articletext, "deathday")).Trim('-');
                    else if (GetInfoBoxFieldValue(articletext, "yod").Length > 0)
                        dateFound = (GetInfoBoxFieldValue(articletext, "yod") + "-" + GetInfoBoxFieldValue(articletext, "mod") + "-" + GetInfoBoxFieldValue(articletext, "dod")).Trim('-');
                }
            }

            // call parser function for futher date fixes
            if(dateFound.Length > 0)
            {
                dateFound = WikiRegexes.Comments.Replace(CiteTemplateDates(@"{{cite web|date=" + dateFound + @"}}").Replace(@"{{cite web|date=", "").Trim('}'), "");

                dateFound = Tools.ConvertDate(dateFound, DeterminePredominantDateLocale(articletext, false)).Trim('-');

                // check ISO dates valid (in case dda used zeros for month/day)
                if (dateFound.Contains("-") && !WikiRegexes.ISODates.IsMatch(dateFound))
                    return personData;

                return Tools.SetTemplateParameterValue(personData, field, dateFound, true);
            }
            
            return personData;
        }

        private static readonly Regex BracketedBirthDeathDate = new Regex(@"\(([^()]+)\)", RegexOptions.Compiled);
        private static readonly Regex FreeFormatDied = new Regex(@"(?:(?:&nbsp;| )(?:-|–|&ndash;)(?:&nbsp;| )|\b[Dd](?:ied\b|\.))", RegexOptions.Compiled);

        /// <summary>
        /// Sets persondata date of birth/death fields based on unformatted info in zeroth section of article, provided dates match existing birth/death categories
        /// </summary>
        /// <param name="personData">Persondata template call</param>
        /// <param name="articletext">The article text</param>
        /// <returns>The updated persondata template call</returns>
        private static string CompletePersonDataDate(string personData, string articletext)
        {
            // get the existing values
            string existingBirthYear = Tools.GetTemplateParameterValue(personData, "DATE OF BIRTH", true);
            string existingDeathYear = Tools.GetTemplateParameterValue(personData, "DATE OF DEATH", true);

            if (existingBirthYear.Length == 4 || existingDeathYear.Length == 4)
            {
                Parsers p = new Parsers();
                string birthDateFound = "", deathDateFound = "";
                string zerothSection = Tools.GetZerothSection(articletext);

                // remove references, wikilinks, templates
                zerothSection = WikiRegexes.Refs.Replace(zerothSection, " ");
                zerothSection = WikiRegexes.SimpleWikiLink.Replace(zerothSection, " ");

                if (WikiRegexes.CircaTemplate.IsMatch(zerothSection))
                    zerothSection = zerothSection.Substring(0, WikiRegexes.CircaTemplate.Match(zerothSection).Index);

                zerothSection = Tools.NestedTemplateRegex("ndash").Replace(zerothSection, " &ndash;");
                zerothSection = WikiRegexes.NestedTemplates.Replace(zerothSection, " ");
                // clean up any format errors in birth/death dates we may want to use
                zerothSection = p.FixDatesAInternal(zerothSection);

                // look for date in bracketed text, check date matches existing value (from categories)
                foreach (Match m in BracketedBirthDeathDate.Matches(zerothSection))
                {
                    string bValue = m.Value;

                    if (!UncertainWordings.IsMatch(bValue) && !ReignedRuledUnsure.IsMatch(bValue) && !FloruitTemplate.IsMatch(bValue))
                    {

                        string bBorn, bDied = "";
                        // split on died/spaced dash
                        if (FreeFormatDied.IsMatch(bValue))
                        {
                            bBorn = bValue.Substring(0, FreeFormatDied.Match(bValue).Index);
                            bDied = bValue.Substring(FreeFormatDied.Match(bValue).Index);
                        }
                        else
                            bBorn = bValue;

                        // born
                        if (existingBirthYear.Length == 4)
                        {
                            if (WikiRegexes.AmericanDates.Matches(bBorn).Count == 1 && WikiRegexes.AmericanDates.Match(bBorn).Value.Contains(existingBirthYear))
                                birthDateFound = WikiRegexes.AmericanDates.Match(bBorn).Value;
                            else if (WikiRegexes.InternationalDates.Matches(bBorn).Count == 1 && WikiRegexes.InternationalDates.Match(bBorn).Value.Contains(existingBirthYear))
                                birthDateFound = WikiRegexes.InternationalDates.Match(bBorn).Value;
                        }

                        // died
                        if (existingDeathYear.Length == 4)
                        {
                            if (WikiRegexes.AmericanDates.Matches(bDied).Count == 1 && WikiRegexes.AmericanDates.Match(bDied).Value.Contains(existingDeathYear))
                                deathDateFound = WikiRegexes.AmericanDates.Match(bDied).Value;
                            else if (WikiRegexes.InternationalDates.Matches(bDied).Count == 1 && WikiRegexes.InternationalDates.Match(bDied).Value.Contains(existingDeathYear))
                                deathDateFound = WikiRegexes.InternationalDates.Match(bDied).Value;
                        }

                        if (birthDateFound.Length > 0 || deathDateFound.Length > 0)
                            break;
                    }
                }

                if (birthDateFound.Length > 4)
                    personData = Tools.SetTemplateParameterValue(personData, "DATE OF BIRTH", Tools.ConvertDate(birthDateFound, DeterminePredominantDateLocale(articletext, true)), false);

                if (deathDateFound.Length > 4)
                    personData = Tools.SetTemplateParameterValue(personData, "DATE OF DEATH", Tools.ConvertDate(deathDateFound, DeterminePredominantDateLocale(articletext, true)), false);
            }

            return personData;
        }
    }
}