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
        //Covered by TaggerTests
        /// <summary>
        /// If necessary, adds/removes various cleanup tags such as wikify, stub, ibid
        /// </summary>
        public string Tagger(string articleText, string articleTitle, bool restrictOrphanTagging, out bool noChange, ref string summary)
        {
            string newText = Tagger(articleText, articleTitle, restrictOrphanTagging, ref summary);

            newText = TagUpdater(newText);

            noChange = newText.Equals(articleText);

            return newText;
        }

        private static readonly CategoriesOnPageNoHiddenListProvider CategoryProv = new CategoriesOnPageNoHiddenListProvider
                                                                                    {
                                                                                        Limit = 10
                                                                                    };
        private static readonly LinksOnPageListProvider LinksOnPageProv = new LinksOnPageListProvider();

        private readonly List<string> tagsRemoved = new List<string>();
        private readonly List<string> tagsAdded = new List<string>();
        private static readonly Regex ImproveCategories = Tools.NestedTemplateRegex("improve categories");
        private static readonly Regex ProposedDeletionDatedEndorsed = Tools.NestedTemplateRegex( new [] {"Proposed deletion/dated", "Proposed deletion endorsed", "Prod blp/dated" });
        private static readonly Regex Unreferenced = Tools.NestedTemplateRegex("unreferenced");
        private static readonly Regex Drugbox = Tools.NestedTemplateRegex(new[] { "Drugbox", "Chembox", "PBB", "PBB Summary" });
        private static readonly Regex MinorPlanetListFooter = Tools.NestedTemplateRegex("MinorPlanetListFooter");
        private static readonly Regex BulletedText = new Regex(@"\r\n[\*#: ].*");

        //TODO:Needs re-write
        /// <summary>
        /// If necessary, adds/removes wikify or stub tag
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">The article title.</param>
        /// <param name="restrictOrphanTagging"></param>
        /// <param name="summary"></param>
        /// <returns>The tagged article.</returns>
        public string Tagger(string articleText, string articleTitle, bool restrictOrphanTagging, ref string summary)
        {
            if(!TaggerPermitted(articleText, articleTitle))
                return articleText;

            tagsRemoved.Clear();
            tagsAdded.Clear();
            int tagsrenamed = 0;

            // Performance: get all templates so most template checks can be against this rather than whole article text
            // Due to old-style Multiple issues template need to add full value of any MI templates back in
            string templates = String.Join(" ", GetAllTemplates(articleText).Select(s => "{{" + s + "}}").ToArray());
            
            if(WikiRegexes.MultipleIssues.IsMatch(templates))
            {
                foreach(Match mi in WikiRegexes.MultipleIssues.Matches(articleText))
                    templates += mi.Value;
            }

            string commentsStripped = WikiRegexes.Comments.Replace(articleText, "");
            string commentsCategoriesStripped = WikiRegexes.Category.Replace(commentsStripped, "");
            if(WikiRegexes.Defaultsort.IsMatch(templates))
                commentsCategoriesStripped = WikiRegexes.Defaultsort.Replace(commentsCategoriesStripped, "");
            Sorter.Interwikis(ref commentsStripped, false);

            // remove stub tags from long articles, don't move section stubs
            if(WikiRegexes.Stub.IsMatch(templates) && WikiRegexes.Stub.IsMatch(commentsStripped))
            {
                // bulleted or indented text should weigh less than simple text.
                // for example, actor stubs may contain large filmographies
                string crapStripped = BulletedText.Replace(WikiRegexes.NestedTemplates.Replace(commentsCategoriesStripped, " "), "");
                int words = (Tools.WordCount(commentsCategoriesStripped, 999) + Tools.WordCount(crapStripped, 999)) / 2;
                if(words > StubMaxWordCount)
                {
                    articleText = WikiRegexes.Stub.Replace(articleText, StubChecker).Trim();

                    if(Variables.LangCode.Equals("ar"))
                    {
                        tagsRemoved.Add("بذرة");
                    }
                    else if(Variables.LangCode.Equals("arz"))
                    {
                        tagsRemoved.Add("تقاوى");
                    }
                    else if(Variables.LangCode.Equals("hy"))
                    {
                        tagsRemoved.Add("Անավարտ");
                    }
                    else
                    {
                        tagsRemoved.Add("stub");
                    }
                }
            }

            // refresh
            if(tagsRemoved.Count > 0)
            {
                commentsStripped = WikiRegexes.Comments.Replace(articleText, "");
                commentsCategoriesStripped = WikiRegexes.Category.Replace(commentsStripped, "");
            }

            //remove disambiguation if disambiguation cleanup exists (en-wiki only)
            if (Variables.LangCode.Equals("en") && WikiRegexes.DisambigsCleanup.IsMatch(templates) && WikiRegexes.DisambigsCleanup.IsMatch(commentsStripped))
            {
                articleText = WikiRegexes.DisambigsGeneral.Replace(articleText, "").Trim();
            }

            // do orphan tagging before template analysis for categorisation tags
            articleText = TagOrphans(articleText, articleTitle, restrictOrphanTagging);

            articleText = TagRefsIbid(articleText);

            articleText = TagEmptySection(articleText);

            int totalCategories;
            // ignore commented out wikilinks, and any in {{Proposed deletion/dated}}
            string forLinkCount = commentsStripped;
            if(ProposedDeletionDatedEndorsed.IsMatch(templates))
                forLinkCount = ProposedDeletionDatedEndorsed.Replace(forLinkCount, "");

            // discount persondata, comments, infoboxes and categories from wikify/underlinked and stub evaluation
            string lengthtext = commentsCategoriesStripped;
            if(WikiRegexes.Persondata.IsMatch(templates))
                lengthtext = WikiRegexes.Persondata.Replace(commentsCategoriesStripped, "");
            if(WikiRegexes.InfoBox.IsMatch(templates))
                lengthtext = WikiRegexes.InfoBox.Replace(lengthtext, "");
            if(Drugbox.IsMatch(templates))
                lengthtext = Drugbox.Replace(lengthtext, "");
            if(WikiRegexes.ReferenceList.IsMatch(templates))
                lengthtext = WikiRegexes.ReferenceList.Replace(lengthtext, "");

            int length = lengthtext.Length + 1;
            int linkLimit = (int)(0.0025 * length)+1;
            int wikiLinkCount = Tools.LinkCount(forLinkCount, linkLimit);
            bool underlinked = (wikiLinkCount < 0.0025 * length);

            #if DEBUG || UNITTEST
            if (Globals.UnitTestMode)
            {
                totalCategories = Globals.UnitTestIntValue;
            }
            else
            #endif
            {
                // stubs add non-hidden stub categories, don't count these in categories count
                // also don't count "Proposed deletion..." cats
                // limitation: in the unlikely event that the article has only redlinked cats then it is {{uncat}} but we won't tag it as such
                totalCategories = RegularCategories(commentsStripped, false).Count;

                // templates may add categories to page that are not [[Category...]] links, so use API call for accurate Category count
                if(totalCategories == 0)
                    totalCategories = RegularCategories(CategoryProv.MakeList(new[] { articleTitle })).Count;
            }

            // remove dead end if > 0 explicit wikilinks on page (don't count any links transcluded from templates)
            if (wikiLinkCount > 0 && WikiRegexes.DeadEnd.IsMatch(templates))
            {
                if (Variables.LangCode.Equals("ar") || Variables.LangCode.Equals("arz"))
                    articleText = WikiRegexes.DeadEnd.Replace(articleText, "");
                else
                    articleText = WikiRegexes.DeadEnd.Replace(articleText, m => Tools.IsSectionOrReasonTemplate(m.Value, articleText) ? m.Value : m.Groups[1].Value).TrimStart();

                if (!WikiRegexes.DeadEnd.IsMatch(articleText))
                {
                    if (Variables.LangCode.Equals("ar"))
                    {
                        tagsRemoved.Add("نهاية مسدودة");
                    }
                    else if (Variables.LangCode.Equals("arz"))
                    {
                        tagsRemoved.Add("نهاية مسدودة");
                    }
                    else
                    {
                        tagsRemoved.Add("deadend");
                    }
                }
            }

            if (length <= 300 && !WikiRegexes.Stub.IsMatch(commentsCategoriesStripped) &&
                !WikiRegexes.Disambigs.IsMatch(commentsCategoriesStripped) && !WikiRegexes.SIAs.IsMatch(commentsCategoriesStripped) && !WikiRegexes.NonDeadEndPageTemplates.IsMatch(commentsCategoriesStripped))
            {
                // add stub tag. Exclude pages their title starts with "List of..."
                if (!ListOf.IsMatch(articleTitle) && !WikiRegexes.MeaningsOfMinorPlanetNames.IsMatch(articleTitle))
                {
                    if (Variables.LangCode.Equals("ar"))
                    {
                        articleText += Tools.Newline("{{بذرة}}", 3);
                        tagsAdded.Add("بذرة");
                    }
                    else if (Variables.LangCode.Equals("arz"))
                    {
                        articleText += Tools.Newline("{{تقاوى}}", 3);
                        tagsAdded.Add("تقاوى");
                    }
                    else if (Variables.LangCode.Equals("hy"))
                    {
                        articleText += Tools.Newline("{{Անավարտ}}", 3);
                        tagsAdded.Add("Անավարտ");
                    }
                    else
                    {
                        articleText += Tools.Newline("{{stub}}", 3);
                        tagsAdded.Add("stub");
                    }
                    commentsStripped = WikiRegexes.Comments.Replace(articleText, "");
                }
            }

            // rename existing {{improve categories}} else add uncategorized tag
            if (totalCategories == 0 && ImproveCategories.IsMatch(templates))
            {
                articleText = Tools.RenameTemplate(articleText, "improve categories", "Uncategorized");
                templates = Tools.RenameTemplate(templates, "improve categories", "Uncategorized");
            }

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Archive_19#AWB_problems
            // nl wiki doesn't use {{Uncategorized}} template
            // prevent wictionary redirects from being tagged as uncategorised
            if (totalCategories == 0
                && !WikiRegexes.Uncat.IsMatch(templates)
                && Variables.LangCode != "nl")
            {
                // bulleted or indented text should weigh less than simple text.
                // for example, actor stubs may contain large filmographies
                string crapStripped = BulletedText.Replace(WikiRegexes.NestedTemplates.Replace(commentsCategoriesStripped, " "), "");
                int words = (Tools.WordCount(commentsCategoriesStripped, 10) + Tools.WordCount(crapStripped, 10)) / 2;

                if(words > 6)
                {
                    if (WikiRegexes.Stub.IsMatch(commentsStripped))
                    {
                        // add uncategorized stub tag
                        if (Variables.LangCode.Equals("ar"))
                        {
                            articleText += Tools.Newline("{{بذرة غير مصنفة|", 2) + WikiRegexes.DateYearMonthParameter + @"}}";
                            tagsAdded.Add("[[تصنيف:مقالات غير مصنفة|غير مصنفة]]");
                        }
                        else if (Variables.LangCode.Equals("arz"))
                        {
                            articleText += Tools.Newline("{{تقاوى مش متصنفه|", 2) + WikiRegexes.DateYearMonthParameter + @"}}";
                            tagsAdded.Add("[[قالب:تقاوى مش متصنفه|تقاوى مش متصنفه]]");
                        }
                        else if(Variables.LangCode.Equals("hy")) // same template for uncat and uncat stub
                        {
                            articleText += Tools.Newline("{{Կատեգորիա չկա|", 2) + WikiRegexes.DateYearMonthParameter + @"}}";
                            tagsAdded.Add("Կատեգորիա չկա");
                        }
                        else if(Variables.LangCode.Equals("sv")) // same template for uncat and uncat stub
                        {
                            articleText += Tools.Newline("{{Okategoriserad|", 2) + WikiRegexes.DateYearMonthParameter + @"}}";
                            tagsAdded.Add("[[Mall:Okategoriserad|okategoriserad]]");
                        }
                        else
                        {
                            articleText += Tools.Newline("{{Uncategorized stub|", 2) + WikiRegexes.DateYearMonthParameter + @"}}";
                            tagsAdded.Add("[[CAT:UNCATSTUBS|uncategorised]]");
                        }
                    }
                    else
                    {
                        if (Variables.LangCode.Equals("ar"))
                        {
                            articleText += Tools.Newline("{{غير مصنفة|", 2) + WikiRegexes.DateYearMonthParameter + @"}}";
                            tagsAdded.Add("[[CAT:UNCAT|مقالات غير مصنفة]]");
                        }
                        else if (Variables.LangCode.Equals("arz"))
                        {
                            articleText += Tools.Newline("{{مش متصنفه|", 2) + WikiRegexes.DateYearMonthParameter + @"}}";
                            tagsAdded.Add("[[CAT:UNCAT|مش متصنفه]]");
                        }
                        else if(Variables.LangCode.Equals("el"))
                        {
                            articleText += Tools.Newline("{{Ακατηγοριοποίητο|", 2) + WikiRegexes.DateYearMonthParameter + @"}}";
                            tagsAdded.Add("[[Πρότυπο:Ακατηγοριοποίητο|ακατηγοριοποίητο]]");
                        }
                        else if(Variables.LangCode.Equals("hy"))
                        {
                            articleText += Tools.Newline("{{Կատեգորիա չկա|", 2) + WikiRegexes.DateYearMonthParameter + @"}}";
                            tagsAdded.Add("Կատեգորիա չկա");
                        }
                        else if(Variables.LangCode.Equals("sv"))
                        {
                            articleText += Tools.Newline("{{Okategoriserad|", 2) + WikiRegexes.DateYearMonthParameter + @"}}";
                            tagsAdded.Add("[[Mall:Okategoriserad|okategoriserad]]");
                        }
                        else
                        {
                            articleText += Tools.Newline("{{Uncategorized|", 2) + WikiRegexes.DateYearMonthParameter + @"}}";
                            tagsAdded.Add("[[CAT:UNCAT|uncategorised]]");
                        }
                    }
                }
            }

            // remove {{Uncategorized}} if > 0 real categories (stub categories not counted)
            // rename {{Uncategorized}} to {{Uncategorized stub}} if stub with zero categories (stub categories not counted)
            if (WikiRegexes.Uncat.IsMatch(templates))
            {
                if (totalCategories > 0)
                {
                    articleText = WikiRegexes.Uncat.Replace(articleText, "").TrimStart();
                    if (Variables.LangCode.Equals("ar"))
                        tagsRemoved.Add("غير مصنفة");
                    else if (Variables.LangCode.Equals("arz"))
                        tagsRemoved.Add("مش متصنفه");
                    else
                        tagsRemoved.Add("uncategorised");
                    
                }
                else if (totalCategories == 0 && WikiRegexes.Stub.IsMatch(commentsStripped))
                {
                   // rename uncat to uncat stub if no uncat stub. If uncat and uncat stub, remove uncat.
                    bool uncatstub = false;
                    foreach(Match u in WikiRegexes.Uncat.Matches(articleText))
                    {
                        if(WikiRegexes.Stub.IsMatch(u.Value))
                        {
                            uncatstub = true;
                            break;
                        }
                    }

                    articleText = WikiRegexes.Uncat.Replace(articleText, u2 => {
                                                                if (!uncatstub) // rename
                                                                {
                                                                    tagsrenamed++;
                                                                    if (Variables.LangCode.Equals("ar"))
                                                                        return Tools.RenameTemplate(u2.Value, "بذرة غير مصنفة");
                                                                    if (Variables.LangCode.Equals("arz"))
                                                                        return Tools.RenameTemplate(u2.Value, "تقاوى مش متصنفه");
                                                                    if (Variables.LangCode.Equals("en") || Variables.LangCode.Equals("simple"))
                                                                        return Tools.RenameTemplate(u2.Value, "Uncategorized stub");
                                                                }
                                                                else // already uncat stub so remove plain uncat
                                                                {
                                                                    if(!WikiRegexes.Stub.IsMatch(u2.Value))
                                                                    {
                                                                        if (Variables.LangCode.Equals("ar"))
                                                                            tagsRemoved.Add("غير مصنفة");
                                                                        else if (Variables.LangCode.Equals("arz"))
                                                                            tagsRemoved.Add("مش متصنفه");
                                                                        else
                                                                            tagsRemoved.Add("uncategorised");
                                                                        return "";
                                                                    }
                                                                }
                                                                return u2.Value;
                                                            });
                }
            }
            
            if (wikiLinkCount == 0 &&
                !WikiRegexes.DeadEnd.IsMatch(articleText) &&
                !WikiRegexes.SIAs.IsMatch(templates) &&
                !WikiRegexes.NonDeadEndPageTemplates.IsMatch(templates) &&
                !WikiRegexes.MeaningsOfMinorPlanetNames.IsMatch(articleTitle)                
               )
            {
                int apilinks = 0;

                if (!Globals.UnitTestMode)
                    apilinks = LinksOnPageProv.MakeList(new[] { articleTitle }).Where(l => (l.NameSpaceKey == Namespace.Mainspace)).Count();

                // for dead end addition, use API call to get wikilink count (filter to mainspace links only), so we do count any links transcluded from templates
                if(apilinks == 0)
                {
                    // add dead-end tag
                    // no blank line between dead end and orphan tags for ar/arz
                    if (Variables.LangCode.Equals("ar"))
                    {
                        articleText = "{{نهاية مسدودة|" + WikiRegexes.DateYearMonthParameter + "}}\r\n" + (WikiRegexes.Orphan.IsMatch(articleText) ? "" : "\r\n") + articleText;
                        tagsAdded.Add("[[:تصنيف:مقالات نهاية مسدودة|نهاية مسدودة]]");
                        // if dead end then remove underlinked/wikify
                        if(WikiRegexes.Wikify.IsMatch(articleText))
                        {
                            articleText = WikiRegexes.Wikify.Replace(articleText, "").TrimStart();
                            tagsRemoved.Add("ويكي");
                        }
                    }
                    else if (Variables.LangCode.Equals("arz"))
                    {
                        articleText = "{{نهايه مسدوده|" + WikiRegexes.DateYearMonthParameter + "}}\r\n" + articleText;
                        tagsAdded.Add("[[:قالب:نهايه مسدوده|نهايه مسدوده]]");
                        // if dead end then remove underlinked
                        if(WikiRegexes.Wikify.IsMatch(articleText))
                        {
                            articleText = WikiRegexes.Wikify.Replace(articleText, "").TrimStart();
                            tagsRemoved.Add("ويكى");
                        }
                    }
                    else if (Variables.LangCode != "sv" && !WikiRegexes.Centuryinbox.IsMatch(articleText)
                             && !Regex.IsMatch(WikiRegexes.MultipleIssues.Match(articleText).Value.ToLower(), @"\bdead ?end\b")
                             && !MinorPlanetListFooter.IsMatch(articleText))
                    {
                        // Don't add excess newlines between new tags
                        articleText = "{{Dead end|" + WikiRegexes.DateYearMonthParameter + "}}" + (tagsAdded.Count > 0 ? "\r\n" : "\r\n\r\n") + articleText;
                        tagsAdded.Add("[[CAT:DE|deadend]]");
                        // if dead end then remove underlinked
                        if(articleText.IndexOf("underlinked", StringComparison.OrdinalIgnoreCase) > -1)
                        {
                            articleText = Tools.NestedTemplateRegex("underlinked").Replace(articleText, "").TrimStart();
                            tagsRemoved.Add("underlinked");
                        }
                    }
                }
            }
            // add underlinked/wikify tag, don't add underlinked/wikify if {{dead end}} already present
            // Dont' tag SIA pages, may create wikilinks from templates
            else if (wikiLinkCount < 3 && underlinked && length > 400 && !WikiRegexes.Wikify.IsMatch(articleText)
                     && !WikiRegexes.MultipleIssues.Match(articleText).Value.ToLower().Contains("wikify")
                     && !WikiRegexes.DeadEnd.IsMatch(articleText)
                     && !WikiRegexes.SIAs.IsMatch(articleText)
                     && !WikiRegexes.NonDeadEndPageTemplates.IsMatch(articleText)
                     && !WikiRegexes.MeaningsOfMinorPlanetNames.IsMatch(articleTitle))
            {
                // Avoid excess newlines between templates
                string templateEnd = "}}\r\n" + (articleText.TrimStart().StartsWith(@"{{") ? "" : "\r\n");
                
                if (Variables.LangCode.Equals("ar"))
                {
                    articleText = "{{ويكي|" + WikiRegexes.DateYearMonthParameter + templateEnd + articleText.TrimStart();
                    tagsAdded.Add("[[وب:ويكي|ويكي]]");
                }
                else if (Variables.LangCode.Equals("arz"))
                {
                    articleText = "{{ويكى|" + WikiRegexes.DateYearMonthParameter + templateEnd + articleText;
                    tagsAdded.Add("[[قالب:ويكى|ويكى]]");
                }
                else if (Variables.LangCode.Equals("sv"))
                {
                    articleText = "{{Ickewiki|" + WikiRegexes.DateYearMonthParameter + templateEnd + articleText;
                    tagsAdded.Add("[[WP:PW|ickewiki]]");
                }
                else
                {
                    articleText = "{{Underlinked|" + WikiRegexes.DateYearMonthParameter + templateEnd + articleText;
                    tagsAdded.Add("[[CAT:UL|underlinked]]");
                }
            }
            else if (wikiLinkCount > 3 && !underlinked &&
                     WikiRegexes.Wikify.IsMatch(templates))
            {
                if (Variables.LangCode.Equals("ar") || Variables.LangCode.Equals("arz"))
                    articleText = WikiRegexes.Wikify.Replace(articleText, "");
                else
                    // remove wikify, except section templates or wikify tags with reason parameter specified
                    articleText = WikiRegexes.Wikify.Replace(articleText, m => Tools.IsSectionOrReasonTemplate(m.Value, articleText) ? m.Value : m.Groups[1].Value).TrimStart();

                if (!WikiRegexes.Wikify.IsMatch(articleText))
                {
                    if (Variables.LangCode.Equals("ar"))
                    {
                        tagsRemoved.Add("ويكي");
                    }
                    else if (Variables.LangCode.Equals("arz"))
                    {
                        tagsRemoved.Add("ويكى");
                    }
                    else
                    {
                        tagsRemoved.Add("underlinked");
                    }
                }
            }

            // rename unreferenced --> refimprove if has existing refs, update date
            // if have both unreferenced and refimprove, and have some refs then just remove unreferenced
            if (WikiRegexes.Unreferenced.IsMatch(templates)
                && (TotalRefsNotGrouped(commentsCategoriesStripped) + Tools.NestedTemplateRegex("sfn").Matches(commentsCategoriesStripped).Count) > 0)
            {
                articleText = Unreferenced.Replace(articleText, m2 => 
                                                   {
                                                       if(Tools.NestedTemplateRegex("Refimprove").IsMatch(articleText))
                                                       {
                                                           tagsRemoved.Add("unreferenced");
                                                           return "";
                                                       }

                                                       tagsrenamed++;
                                                       return Tools.UpdateTemplateParameterValue(Tools.RenameTemplate(m2.Value, "refimprove"), "date", "{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}");
                                                   });

            }

            if (tagsAdded.Count > 0 || tagsRemoved.Count > 0 || tagsrenamed > 0)
            {
                Parsers p = new Parsers();
                HideText hider = new HideText();

                articleText = hider.HideUnformatted(articleText);

                articleText = p.MultipleIssues(articleText);
                articleText = Conversions(articleText);
                articleText = hider.AddBackUnformatted(articleText);

                // sort again in case tag removal requires whitespace cleanup
                // Don't sort interwikis, we can't specify the correct InterWikiSortOrder
                p.SortInterwikis = false;
                articleText = p.Sorter.Sort(articleText, articleTitle);
            }

            summary = PrepareTaggerEditSummary();

            return articleText;
        }

        private static string StubChecker(Match m)
        {
            // if stub tag is a section stub tag, don't remove from section in article
            return Variables.SectStubRegex.IsMatch(m.Value) ? m.Value : "";
        }

        /// <summary>
        /// Checks whether Tagger is permitted on article.
        /// Allowed on mainspace for non-redirect pages without {{wi}} template
        /// Also allowed for ar-wiki namespace 104
        /// </summary>
        /// <returns>True if Tagger is permitted on article</returns>
        /// <param name='articleText'>Article text</param>
        /// <param name='articleTitle'>Article title</param>
        public static bool TaggerPermitted(string articleText, string articleTitle)
        {
            if(articleTitle.Equals("Wikipedia:AutoWikiBrowser/Sandbox"))
                return true;
            // don't tag redirects/outside article namespace/no tagging changes
            // allow for ar-wiki 104
            if(Variables.LangCode.Equals("ar") && Namespace.Determine(articleTitle) == 104 && !WikiRegexes.CEHar.IsMatch(articleText))
                return true;
            if (!Namespace.IsMainSpace(articleTitle) || Tools.IsRedirectOrSoftRedirect(articleText) || WikiRegexes.Wi.IsMatch(articleText) || articleTitle=="Main Page")
                return false;

            return true;
        }

        private static readonly Regex TemplateParameter2 = new Regex(@" \{\{\{2\|\}\}\}", RegexOptions.Compiled);

        /// <summary>
        /// Substitutes some user talk templates
        /// </summary>
        /// <param name="talkPageText">The wiki text of the talk page.</param>
        /// <param name="talkPageTitle">The wiki talk page title</param>
        /// <param name="userTalkTemplatesRegex">Dictoinary of regexes matching template calls to substitute</param>
        /// <returns>The updated article text</returns>
        public static string SubstUserTemplates(string talkPageText, string talkPageTitle, Regex userTalkTemplatesRegex)
        {
            if (userTalkTemplatesRegex == null)
                return talkPageText;

            talkPageText = talkPageText.Replace("{{{subst", "REPLACE_THIS_TEXT");
            Dictionary<Regex, string> regexes = new Dictionary<Regex, string> { { userTalkTemplatesRegex, "{{subst:$2}}" } };

            talkPageText = Tools.ExpandTemplate(talkPageText, talkPageTitle, regexes, true);

            talkPageText = TemplateParameter2.Replace(talkPageText, "");
            return talkPageText.Replace("REPLACE_THIS_TEXT", "{{{subst");
        }

        /// <summary>
        /// Returns the categories that are not stub or proposed deletion categories from the input list
        /// </summary>
        /// <param name="AllCategories">List of all categories</param>
        /// <returns>List of regular categories</returns>
        public static List<Article> RegularCategories(List<Article> AllCategories)
        {
            return (from a in AllCategories
                let name = a.NamespacelessName
                where
                    !name.EndsWith(" stubs") &&
                    !a.Name.EndsWith(":Stubs") &&
                    !name.StartsWith("Proposed deletion") &&
                    !name.Contains("proposed for deletion") &&
                    !name.Contains("proposed deletions") &&
                    !name.Equals("Articles created via the Article Wizard")
                select a).ToList();
        }

        /// <summary>
        /// Returns the categories that are not stub or proposed deletion categories from the input article text
        /// </summary>
        /// <param name="articleText">Wiki text</param>
        /// <returns>List of regular categories</returns>
        public static List<Article> RegularCategories(string articleText, bool hideComments)
        {
            // Don't count commented out categories
            if(hideComments)
                articleText = WikiRegexes.MathPreSourceCodeComments.Replace(articleText, "");

            List<Article> Cats = new List<Article>();

            foreach (Match m in WikiRegexes.Category.Matches(articleText))
            {
                Cats.Add(new Article(m.Groups[1].Value.Trim()));
            }

            return RegularCategories(Cats);
        }

        /// <summary>
        /// Returns the categories that are not stub or proposed deletion categories from the input article text
        /// </summary>
        /// <param name="articleText">Wiki text</param>
        /// <returns>List of regular categories</returns>
        public static List<Article> RegularCategories(string articleText)
        {
            return RegularCategories(articleText, true);
        }

        private static readonly WhatLinksHereAndPageRedirectsExcludingTheRedirectsListProvider WlhProv =
            new WhatLinksHereAndPageRedirectsExcludingTheRedirectsListProvider(MinIncomingLinksToBeConsideredAnOrphan)
            {
                ForceQueryLimit = "10",
                Limit = 10
            };

        private const int MinIncomingLinksToBeConsideredAnOrphan = 3;
        private static readonly Regex Rq = Tools.NestedTemplateRegex("Rq");

        /// <summary>
        /// Tags pages with insufficient incoming page links with the orphan template (localised for ru-wiki).
        /// Removes orphan tag from pages with sufficient incoming page links.
        /// Disambig, SIA pages and soft redirects to Wictionary are never tagged as orphan.
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">Title of the article</param>
        /// <param name="restrictOrphanTagging">Whether to restrict the addition of the orphan tag to pages with zero incoming links only.</param>
        /// <returns>The updated article text</returns>
        private string TagOrphans(string articleText, string articleTitle, bool restrictOrphanTagging)
        {
            // check if not orphaned
            bool orphaned, orphaned2;
            int incomingLinks = 0;
            #if DEBUG || UNITTEST
            if (Globals.UnitTestMode)
            {
                orphaned = orphaned2 = Globals.UnitTestBoolValue;
            }
            else
                #endif
            {
                try
                {
                    incomingLinks = WlhProv.MakeList(Namespace.Article, articleTitle).Count;
                    orphaned = (incomingLinks < MinIncomingLinksToBeConsideredAnOrphan);
                    orphaned2 = restrictOrphanTagging
                        ? (incomingLinks == 0)
                        : orphaned;
                }

                catch (Exception ex)
                {
                    // don't mark as orphan in case of exception
                    orphaned = orphaned2 = false;
                    ErrorHandler.CurrentPage = articleTitle;
                    ErrorHandler.HandleException(ex);
                }
            }

            if (Variables.LangCode == "ru" && incomingLinks == 0 && Rq.Matches(articleText).Count == 1)
            {
                string rqText = Rq.Match(articleText).Value;
                if (!rqText.Contains("linkless"))
                    return articleText.Replace(rqText, rqText.Replace(@"}}", @"|linkless}}"));
            }

            // add orphan tag if applicable, and no disambig nor SIA
            if (!Variables.LangCode.Equals("sv") && orphaned2 && !WikiRegexes.Orphan.IsMatch(articleText) && Tools.GetTemplateParameterValue(WikiRegexes.MultipleIssues.Match(articleText).Value, "orphan").Length == 0
                && !WikiRegexes.Disambigs.IsMatch(articleText) && !WikiRegexes.SIAs.IsMatch(articleText) && !WikiRegexes.Wi.IsMatch(articleText) && !articleText.Contains(@"[[Category:Disambiguation pages]]"))
            {
                if (Variables.LangCode.Equals("ar"))
                {
                    articleText = "{{يتيمة|" + WikiRegexes.DateYearMonthParameter + "}}\r\n\r\n" + articleText;
                    tagsAdded.Add("[[تصنيف:يتيمة|يتيمة]]");
                }
                else if (Variables.LangCode.Equals("arz"))
                {
                    articleText = "{{يتيمه|" + WikiRegexes.DateYearMonthParameter + "}}\r\n\r\n" + articleText;
                    tagsAdded.Add("[[قالب:يتيمه|يتيمه]]");
                }
                else if (Variables.LangCode.Equals("el"))
                {
                    articleText = "{{Ορφανό|" + WikiRegexes.DateYearMonthParameter + "}}\r\n\r\n" + articleText;
                    tagsAdded.Add("[[Κατηγορία:Ορφανά λήμματα|ορφανό]]");
                }
                else if (Variables.LangCode.Equals("fa"))
                {
                    articleText = "{{یتیم|" + WikiRegexes.DateYearMonthParameter + "}}\r\n\r\n" + articleText;
                    tagsAdded.Add("[[الگو:یتیم|یتیم]]");
                }
                else if (Variables.LangCode.Equals("hy"))
                {
                    articleText = "{{Որբ|" + WikiRegexes.DateYearMonthParameter + "}}\r\n\r\n" + articleText;
                    tagsAdded.Add("[[Կատեգորիա:«Որբ» հոդվածներ|Որբ]]");
                }
                else if (Variables.LangCode.Equals("tr"))
                {
                    articleText = "{{Öksüz|" + WikiRegexes.DateYearMonthParameter + "}}\r\n\r\n" + articleText;
                    tagsAdded.Add("[[CAT:O|orphan]]");
                }
                else
                {
                    articleText = "{{Orphan|" + WikiRegexes.DateYearMonthParameter + "}}\r\n\r\n" + articleText;
                    tagsAdded.Add("[[CAT:O|orphan]]");
                }
            }
            // otherwise consider orphan tag removal
            // do not remove when "few" parameter specified, human review required then
            else if (!orphaned && WikiRegexes.Orphan.IsMatch(articleText)
                     && Tools.GetTemplateParameterValue(WikiRegexes.Orphan.Match(articleText).Value, "few").Length == 0)
            {
                articleText = WikiRegexes.Orphan.Replace(articleText, m => m.Groups["MI"].Value).TrimStart();
                if (Variables.LangCode.Equals("ar"))
                {
                    tagsRemoved.Add("يتيمة");
                }
                else if (Variables.LangCode.Equals("arz"))
                {
                    tagsRemoved.Add("يتيمه");
                }
                else if (Variables.LangCode.Equals("hy"))
                {
                    tagsRemoved.Add("Որբ");
                }
                else
                {
                    tagsRemoved.Add("orphan");
                }
            }

            return articleText;
        }

        private static readonly Regex IbidOpCitRef = new Regex(@"<\s*ref\b[^<>]*>\s*(ibid\.?|op\.?\s*cit\.?|loc\.?\s*cit\.?)\b", RegexOptions.IgnoreCase);
        /// <summary>
        /// Tags references of 'ibid' with the {{ibid}} cleanup template, en-wiki mainspace only
        /// </summary>
        /// <param name="articleText"></param>
        /// <returns></returns>
        private string TagRefsIbid(string articleText)
        {
            if (Variables.LangCode == "en" && IbidOpCitRef.IsMatch(articleText) && !WikiRegexes.Ibid.IsMatch(articleText))
            {
                tagsAdded.Add("Ibid");
                return @"{{Ibid|" + WikiRegexes.DateYearMonthParameter + @"}}" + articleText;
            }

            return articleText;
        }

        /// <summary>
        /// Tags empty level-2 sections with {{Empty section}}, en-wiki mainspace only
        /// </summary>
        /// <param name="articleText">The article text</param>
        /// <returns>The updated article text</returns>
        private string TagEmptySection(string articleText)
        {
            // Regex check for performance
            if (!Variables.LangCode.Equals("en") || !Regex.IsMatch(articleText, @"==\s+==[^=]"))
                return articleText;

            string originalarticleText = "";
            int tagsadded = 0;

            while (!originalarticleText.Equals(articleText))
            {
                originalarticleText = articleText;

                int lastpos = -1;
                foreach (Match m in WikiRegexes.HeadingLevelTwo.Matches(Tools.ReplaceWith(articleText, WikiRegexes.UnformattedText, 'x')))
                {
                    // empty setion if only whitespace between two level-2 headings
                    if (lastpos > -1 && articleText.Substring(lastpos, (m.Index - lastpos)).Trim().Length == 0)
                    {
                        articleText = articleText.Insert(m.Index, @"{{Empty section|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}" + "\r\n\r\n");
                        tagsadded++;
                        break;
                    }

                    // don't tag single character headings: alpha list where empty section allowed
                    if (m.Groups[1].Length > 1)
                        lastpos = m.Index + m.Length;
                }
            }

            if (tagsadded > 0)
                tagsAdded.Add("Empty section (" + tagsadded + ")");

            return articleText;
        }

        private string PrepareTaggerEditSummary()
        {
            string summary = "";
            string tags;
            if (tagsRemoved.Count > 0)
            {
                // Reverse order of words for arwiki and arzwiki
                if (Variables.LangCode.Equals("ar"))
                {
                    if (tagsRemoved.Count == 1)
                        tags = " وسم ";
                    else if (tagsRemoved.Count == 2)
                        tags = " وسمي ";
                    else 
                        tags = " وسوم ";
                    summary = " أزال" + tags + Tools.ListToStringCommaSeparator(tagsRemoved);
                }
                else if (Variables.LangCode.Equals("arz"))
                {
                    if (tagsRemoved.Count == 1)
                        tags = " وسم ";
                    else if (tagsRemoved.Count == 2)
                        tags = " وسمين ";
                    else 
                        tags = " وسوم ";
                    summary = " شال" + tags + Tools.ListToStringCommaSeparator(tagsRemoved);
                }
                else if (Variables.LangCode.Equals("el"))
                {
                     if(tagsRemoved.Count == 1)
                        summary = "αφαιρέθηκε η ετικέτα:" + Tools.ListToStringCommaSeparator(tagsRemoved);
                    else 
                        summary = "αφαιρέθηκαν οι ετικέτες:" + Tools.ListToStringCommaSeparator(tagsRemoved);
                }
                else if (Variables.LangCode.Equals("eo"))
                    summary = "forigis " + Tools.ListToStringCommaSeparator(tagsRemoved) + " etikedo" +
                    (tagsRemoved.Count == 1 ? "" : "j");
                else if (Variables.LangCode.Equals("fa"))
                    summary = " برچسب" + Tools.ListToStringCommaSeparator(tagsRemoved) + " حذف شد ";
                else if (Variables.LangCode.Equals("fr"))
                    summary = "retrait " + Tools.ListToStringCommaSeparator(tagsRemoved) + " balise" +
                    (tagsRemoved.Count == 1 ? "" : "s");
                else if (Variables.LangCode.Equals("hy"))
                    summary = "ջնջվեց " + Tools.ListToStringCommaSeparator(tagsRemoved) + " կաղապար" +
                    (tagsRemoved.Count == 1 ? "" : "ներ");
                else if (Variables.LangCode.Equals("sv"))
                {
                     if(tagsRemoved.Count == 1)
                        tags = Tools.ListToStringCommaSeparator(tagsRemoved) + "-mall";
                    else 
                        tags = Tools.ListToStringWithSeparatorAndWordSuffix(tagsRemoved, "-", ", ", " och ") + "mallar";
                    summary = "tog bort " + tags;
                }

                else if (Variables.LangCode.Equals("tr"))
                {
                     if(tagsRemoved.Count == 1)
                        summary = "removed " + Tools.ListToStringCommaSeparator(tagsRemoved) + " etiketi";
                    else 
                        summary = "removed " + Tools.ListToStringCommaSeparator(tagsRemoved) + " etiketleri";
                }
                else
                    summary = "removed " + Tools.ListToStringCommaSeparator(tagsRemoved) + " tag" +
                    (tagsRemoved.Count == 1 ? "" : "s");
            }

            if (tagsAdded.Count > 0)
            {
                if (!string.IsNullOrEmpty(summary))
                {
                    if (Variables.LangCode.Equals("ar") || Variables.LangCode.Equals("arz") || Variables.LangCode.Equals("fa"))
                        summary += "، ";
                    else
                        summary += ", ";
                }

                // Reverse order of words for arwiki and arzwiki
                if (Variables.LangCode.Equals("ar"))
                {
                    if (tagsAdded.Count == 1)
                        tags = " وسم ";
                    else if (tagsAdded.Count == 2)
                        tags = " وسمي ";
                    else 
                        tags = " وسوم ";
                    summary += "أضاف " + tags + Tools.ListToStringCommaSeparator(tagsAdded);
                }
                else if (Variables.LangCode.Equals("arz"))
                {
                    if (tagsAdded.Count == 1)
                        tags = " وسم ";
                    else if (tagsAdded.Count == 2)
                        tags = " وسمين ";
                    else 
                        tags = " وسوم ";
                    summary += "زود " + tags + Tools.ListToStringCommaSeparator(tagsAdded);
                }
                else if (Variables.LangCode.Equals("el"))
                {
                    if (tagsAdded.Count == 1)
                        summary += "προστέθηκε η ετικέτα: " + Tools.ListToStringCommaSeparator(tagsAdded);
                    else 
                        summary += "προστέθηκαν οι ετικέτες: " + Tools.ListToStringCommaSeparator(tagsAdded);
                }
                    else if (Variables.LangCode.Equals("eo"))
                    summary += "aldonis " + Tools.ListToStringCommaSeparator(tagsAdded) + " etikedo" +
                    (tagsRemoved.Count == 1 ? "" : "j");
                else if (Variables.LangCode.Equals("fa"))
                    summary += "برچسب " + Tools.ListToStringCommaSeparator(tagsAdded) + " اضافه شد ";
                else if (Variables.LangCode.Equals("fr"))
                    summary += "ajout " + Tools.ListToStringCommaSeparator(tagsAdded) + " balise" +
                    (tagsAdded.Count == 1 ? "" : "s");
                else if (Variables.LangCode.Equals("hy"))
                    summary += "ավելացրել է " + Tools.ListToStringCommaSeparator(tagsAdded) + " կաղապար" +
                    (tagsAdded.Count == 1 ? "" : "ներ");
                else if (Variables.LangCode.Equals("sv"))
                {
                    if (tagsAdded.Count == 1)
                        tags = Tools.ListToStringCommaSeparator(tagsAdded) + "-mall";
                    else 
                        tags = Tools.ListToStringWithSeparatorAndWordSuffix(tagsAdded, "-", ", ", " och ") + "mallar";
                    summary += "lade till " + tags;
                }
                else if (Variables.LangCode.Equals("tk"))
                {
                    if (tagsAdded.Count == 1)
                        summary += "eklendi " + Tools.ListToStringCommaSeparator(tagsAdded) + " etiketi";
                    else 
                        summary += "eklendi " + Tools.ListToStringCommaSeparator(tagsAdded) + " etiketleri";
                }
                else
                    summary += "added " + Tools.ListToStringCommaSeparator(tagsAdded) + " tag" +
                    (tagsAdded.Count == 1 ? "" : "s");
            }
            return summary;
        }

        private static readonly HideText ht = new HideText();

        /// <summary>
        /// Sets the date (month & year) for undated cleanup tags that take a date, from https://en.wikipedia.org/wiki/Wikipedia:AWB/Dated_templates
        /// Avoids changing tags in unformatted text areas (wiki comments etc.)
        /// Note: https://phabricator.wikimedia.org/T4700 means {{subst:}} within ref tags doesn't work, AWB doesn't do anything about it
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The updated article text</returns>
        public static string TagUpdater(string articleText)
        {
            if(WikiRegexes.DatedTemplates.Any())
            {
                List<string> t = GetAllTemplates(articleText), t2 = new List<string>();

                t2.AddRange(t.Where(s => WikiRegexes.DatedTemplates.Contains(s)));

                // only work to do if article has any of the DatedTemplates in it
                if(t2.Any())
                {
                    string originalArticleText = articleText;
                    articleText = Tools.NestedTemplateRegex(t2).Replace(articleText, TagUpdaterME);

                    // Performance: only worth aplying Hide if we made changes to raw articleText
                    if(!originalArticleText.Equals(articleText))
                    {
                        articleText = ht.Hide(originalArticleText);
                        articleText = Tools.NestedTemplateRegex(t2).Replace(articleText, TagUpdaterME);
                        articleText = FixSyntaxSubstRefTags(articleText);
                        articleText = ht.AddBack(articleText);
                    }
                }
            }

            return articleText;
        }

        private static readonly Regex CurlyBraceEnd = new Regex(@"(?:\| *)?}}$", RegexOptions.Compiled);
        private static readonly Regex MonthYear = new Regex(@"^\s*" + WikiRegexes.MonthsNoGroup + @" +20\d\d\s*$", RegexOptions.Compiled);
        private static readonly Regex DateDash = new Regex(@"(\|\s*[Dd]ate\s*)- *=*", RegexOptions.Compiled);

        /// <summary>
        /// Match evaluator for tag updater
        /// Tags undated tags, corrects incorrect template parameter names, removes template namespace in template name
        /// </summary>
        private static string TagUpdaterME(Match m)
        {
            string templatecall = m.Value;

            // rename incorrect template parameter names
            if (Variables.LangCode.Equals("en"))
            {
                templatecall = Tools.RenameTemplateParameter(templatecall, "Date", "date");
                templatecall = Tools.RenameTemplateParameter(templatecall, "dates", "date");
                
                // date- or Date- --> date=
                if(Tools.GetTemplateArgument(templatecall, 1).ToLower().Replace(" ", "").StartsWith("date-"))
                    templatecall = DateDash.Replace(templatecall, m2 => m2.Groups[1].Value.ToLower() + "=");
            }

            // remove template namespace in template name
            templatecall = RemoveTemplateNamespace(templatecall);

            // check if template already dated (date= parameter, localised for some wikis)
            string dateparam = WikiRegexes.DateYearMonthParameter.Substring(0, WikiRegexes.DateYearMonthParameter.IndexOf("="));

            // rename date= if localized
            if(!dateparam.Equals("date"))
                templatecall = Tools.RenameTemplateParameter(templatecall, "date", dateparam);

            // date tag needed?
            if (Tools.GetTemplateParameterValue(templatecall, dateparam).Length == 0)
            {
                // remove empty 'date='
                templatecall = Tools.RemoveTemplateParameter(templatecall, dateparam);

                // find any dates without date= parameter given, add it
                if (Variables.LangCode.Equals("en") && Tools.GetTemplateArgumentCount(templatecall) == 1)
                {
                    string firstArg = Tools.GetTemplateArgument(templatecall, 1);

                    if (MonthYear.IsMatch(firstArg))
                        templatecall = templatecall.Insert(templatecall.IndexOf(firstArg), "date=");
                    else if (firstArg.Equals(dateparam))
                    {
                        templatecall = templatecall.Insert(templatecall.IndexOf(firstArg) + firstArg.Length, "=");
                        templatecall = Tools.RemoveTemplateParameter(templatecall, dateparam);
                    }
                }

                if (Tools.GetTemplateParameterValue(templatecall, dateparam).Length == 0)
                    return (CurlyBraceEnd.Replace(templatecall, "|" + WikiRegexes.DateYearMonthParameter + "}}"));
            }
            else
            {
                string dateFieldValue = Tools.GetTemplateParameterValue(templatecall, dateparam);

                // May, 2010 --> May 2010
                if (dateFieldValue.Contains(",") && !WikiRegexes.AmericanDates.IsMatch(dateFieldValue))
                {
                    templatecall = Tools.SetTemplateParameterValue(templatecall, dateparam, dateFieldValue.Replace(",", ""));
                    dateFieldValue = Tools.GetTemplateParameterValue(templatecall, dateparam);
                }

                // leading zero removed
                if (dateFieldValue.StartsWith("0"))
                {
                    templatecall = Tools.SetTemplateParameterValue(templatecall, dateparam, dateFieldValue.TrimStart('0'));
                    dateFieldValue = Tools.GetTemplateParameterValue(templatecall, dateparam);
                }

                // full International date?
                if (WikiRegexes.InternationalDates.IsMatch(Regex.Replace(dateFieldValue, @"( [a-z])", u => u.Groups[1].Value.ToUpper())))
                {
                    templatecall = Tools.SetTemplateParameterValue(templatecall, dateparam, dateFieldValue.Substring(dateFieldValue.IndexOf(" ")).Trim());
                    dateFieldValue = Tools.GetTemplateParameterValue(templatecall, dateparam);
                }
                else
                    // ISO date?
                    if (WikiRegexes.ISODates.IsMatch(dateFieldValue))
                {
                    DateTime dt = Convert.ToDateTime(dateFieldValue, BritishEnglish);
                    dateFieldValue = dt.ToString("MMMM yyyy", BritishEnglish);

                    templatecall = Tools.SetTemplateParameterValue(templatecall, dateparam, dateFieldValue);
                }

                // date field starts lower case?
                if (!Variables.LangCode.Equals("zh") && !dateFieldValue.Contains(@"CURRENTMONTH") && !dateFieldValue.Equals(Tools.TurnFirstToUpper(dateFieldValue.ToLower())))
                    templatecall = Tools.SetTemplateParameterValue(templatecall, dateparam, Tools.TurnFirstToUpper(dateFieldValue.ToLower()));
            }

            return templatecall;
        }

        private static readonly Regex CommonPunctuation = new Regex(@"[""',\.;:`!\(\)\[\]\?\-–/]", RegexOptions.Compiled);
        /// <summary>
        /// For en-wiki tags redirect pages with one or more of the templates from [[Wikipedia:Template messages/Redirect pages]]
        /// following [[WP:REDCAT]]
        /// </summary>
        /// <param name="articleText">the article text</param>
        /// <param name="articleTitle">the article title</param>
        /// <returns>The updated article text</returns>
        public static string RedirectTagger(string articleText, string articleTitle)
        {
            // only for untagged en-wiki redirects
            if (!Tools.IsRedirect(articleText) || !Variables.IsWikipediaEN || WikiRegexes.Template.IsMatch(articleText))
                return articleText;

            string redirecttarget = Tools.RedirectTarget(articleText);

            // skip self redirects
            if (Tools.TurnFirstToUpperNoProjectCheck(redirecttarget).Equals(Tools.TurnFirstToUpperNoProjectCheck(articleTitle)))
                return articleText;

            // {{R to other namespace}} or more specific template for project/help/portal/category/template/user/talk
            // See https://en.wikipedia.org/wiki/Template:R_to_other_namespace
            if (Namespace.IsMainSpace(articleTitle) && !Namespace.IsMainSpace(redirecttarget) && !WikiRegexes.NestedTemplates.IsMatch(articleText))
            {
                string template;

                switch (Namespace.Determine(redirecttarget))
                {
                    case Namespace.Project:
                        template = "{{R to project namespace}}";
                        break;
                    case Namespace.Help:
                        template = "{{R to help namespace}}";
                        break;
                    case Namespace.FirstCustom:
                        template = "{{R to portal namespace}}";
                        break;
                    case Namespace.Category:
                        template = "{{R to category namespace}}";
                        break;
                    case Namespace.Template:
                        template = "{{R to template namespace}}";
                        break;
                    case Namespace.User:
                        template = "{{R to user namespace}}";
                        break;
                    case Namespace.Talk:
                        template = "{{R to talk namespace}}";
                        break;

                    default:
                        template = "{{R to other namespace}}";
                        break;
                }

                return (articleText + " " + template);
            }

            // {{R from modification}}
            // difference is extra/removed/changed puntuation
            if (!Tools.NestedTemplateRegex(WikiRegexes.RFromModificationList).IsMatch(articleText)
                && !CommonPunctuation.Replace(redirecttarget, "").Equals(redirecttarget) && CommonPunctuation.Replace(redirecttarget, "").Equals(CommonPunctuation.Replace(articleTitle, "")))
                return (articleText + " " + WikiRegexes.RFromModificationString);

            // {{R from title without diacritics}}

            // title and redirect target the same if dacritics removed from redirect target
            if (redirecttarget != Tools.RemoveDiacritics(redirecttarget) && Tools.RemoveDiacritics(redirecttarget).Equals(articleTitle)
                && !Tools.NestedTemplateRegex(WikiRegexes.RFromTitleWithoutDiacriticsList).IsMatch(articleText))
                return (articleText + " " + WikiRegexes.RFromTitleWithoutDiacriticsString);

            // {{R from other capitalisation}}
            if (redirecttarget.Equals(articleTitle, StringComparison.OrdinalIgnoreCase)
                && !Tools.NestedTemplateRegex(WikiRegexes.RFromOtherCapitaliastionList).IsMatch(articleText))
                return (articleText + " " + WikiRegexes.RFromOtherCapitalisationString);

            return articleText;
        }
    }
}