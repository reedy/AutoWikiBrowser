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
        private const string InlineCitationCleanupTemplatesRp = @"(?:Author incomplete|Author missing|Citation broken|Citation not found|Clarify|Date missing|Episode|ISBN missing|Page needed|Publisher missing|Season needed|Time needed|Title incomplete|Title missing|Volume needed|Year missing|rp)";
        private const string OutofOrderRefs = @"(<ref\s+name\s*=\s*(?:""|')?([^<>""=]+?)(?:""|')?\s*(?:\/\s*|>[^<>]+</ref)>)(\s*{{\s*" + InlineCitationCleanupTemplatesRp + @"\s*\|[^{}]+}})?(\s*)(<ref\s+name\s*=\s*(?:""|')?([^<>""=]+?)(?:""|')?\s*(?:\/\s*|>[^<>]+</ref)>)(\s*{{\s*" + InlineCitationCleanupTemplatesRp + @"\s*\|[^{}]+}})?";
        private static readonly Regex OutofOrderRefs1 = new Regex(@"(<ref>[^<>]+</ref>)(\s*)(<ref\s+name\s*=\s*(?:""|')?([^<>""=]+?)(?:""|')?\s*(?:\/\s*|>[^<>]+</ref)>)(\s*{{\s*" + InlineCitationCleanupTemplatesRp + @"\s*\|[^{}]+}})?", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private static readonly Regex OutofOrderRefs2 = new Regex(OutofOrderRefs, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        // regex below ensures a forced match on second and third of consecutive references
        private static readonly Regex OutofOrderRefs3 = new Regex(@"(?<=\s*(?:\/\s*|>[^<>]+</ref)>\s*(?:{{\s*" + InlineCitationCleanupTemplatesRp + @"\s*\|[^{}]+}})?)" + OutofOrderRefs, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private static readonly Regex RefsTag = new Regex(@"<\s*references");

        /// <summary>
        /// Reorders references so that they appear in numerical order, allows for use of {{rp}}, doesn't modify grouped references [[WP:REFGROUP]]
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string ReorderReferences(string articleText)
        {
            // do not reorder stuff in the {{refs}} or <references>...</references> section
            int referencestags = RefsTemplateIndex(articleText);

            // build dictionary of all named references and index of first occurrence
            // No need to go collect refs in named refs section: must be used in article
            Dictionary<string, int> NamedRefsIndexes = new Dictionary<string, int>();
            
            foreach (Match n in GetNamedRefs(articleText).Where(m => m.Index <= referencestags))
            {
                if(!NamedRefsIndexes.ContainsKey(n.Groups[2].Value))
                    NamedRefsIndexes.Add(n.Groups[2].Value, n.Index);
            }

            for (int i = 0; i < 9; i++) // allows for up to 9 consecutive references
            {
                string articleTextBefore = articleText;

                foreach (Match m in OutofOrderRefs1.Matches(articleText))
                {
                    string ref1 = m.Groups[1].Value;
                    int ref1Index;
                    NamedRefsIndexes.TryGetValue(m.Groups[4].Value, out ref1Index);
                    int ref2Index = m.Index;

                    if (ref1Index < ref2Index && ref1Index > 0 && m.Groups[3].Index < referencestags)
                    {
                        string whitespace = m.Groups[2].Value;
                        string rptemplate = m.Groups[5].Value;
                        string ref2 = m.Groups[3].Value;
                        articleText = articleText.Replace(ref1 + whitespace + ref2 + rptemplate, ref2 + rptemplate + whitespace + ref1);
                    }
                }

                articleText = ReorderRefs(articleText, OutofOrderRefs2, referencestags, NamedRefsIndexes);
                articleText = ReorderRefs(articleText, OutofOrderRefs3, referencestags, NamedRefsIndexes);

                if (articleTextBefore.Equals(articleText))
                    break;
            }

            return articleText;
        }

        /// <summary>
        /// Returns the index of the {{reflist}} or {{refs}} or &lt;references&gt; tags, or the articleText length if no tags found
        /// </summary>
        /// <param name="articleText"></param>
        /// <returns></returns>
        private static int RefsTemplateIndex(string articleText)
        {
            Match r1 = WikiRegexes.ReferenceList.Match(articleText);
            if(r1.Success)
                return r1.Index;

            Match r2 = RefsTag.Match(articleText);

            if(r2.Success)
                return r2.Index;

            return articleText.Length;
        }

        private const string RefsPunctuation = @"([,\.;:\?\!])";
        private const string NoPunctuation = @"([^,\.:;\?\!])";
        private static readonly Regex RefsBeforePunctuationR = new Regex(@" *" + WikiRegexes.Refs + @" *" + RefsPunctuation + @"([^,\.:;\?\!]|$)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private static readonly Regex RefsBeforePunctuationQuick = new Regex(@"(?<=(?:/|ref) *)> *" + RefsPunctuation);
        private static readonly Regex RefsAfterDupePunctuation = new Regex(NoPunctuation + RefsPunctuation.Replace(@"\!", "") + @"\2 *" + WikiRegexes.Refs, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private static readonly Regex RefsAfterDupePunctuationQuick = new Regex(@"(?<![,\.:;\?\!])" + RefsPunctuation.Replace(@"\!", "") + @"\1 *<");
        private static readonly Regex Footnote = Tools.NestedTemplateRegex(new[] {"Efn", "Efn-ua", "Efn-lr", "Sfn", "Shortened footnote", "Shortened footnote template", "Sfnb", "Sfnp", "Sfnm", "SfnRef", "Rp"});
        private static readonly Regex PunctuationAfterFootnote = new Regex(@"(?<sfn>" + Footnote + @")(?<punc>[,\.;:\?\!])");
        private static readonly Regex FootnoteAfterDupePunctuation = new Regex(NoPunctuation + RefsPunctuation + @"\2 *(?<sfn>" + Footnote + @")");
        private static readonly Regex FootnoteAfterDupePunctuationQuick = new Regex(RefsPunctuation + @"\1 *" + Footnote);

        /// <summary>
        /// Puts &lt;ref&gt; and {{sfn}} references after punctuation (comma, full stop) per WP:REFPUNC
        /// Applies to en/el wiki only
        /// </summary>
        /// <param name="articleText">The article text</param>
        /// <returns>The updated article text</returns>
        public static string RefsAfterPunctuation(string articleText)
        {
            if (!Variables.LangCode.Equals("en") && !Variables.LangCode.Equals("simple"))
                return articleText;

            string articleTextOriginal = articleText;
            bool hasFootnote = TemplateExists(GetAllTemplates(articleText), Footnote);

            articleText = RefsBeforePunctuation(articleText);

            if(hasFootnote)
            {
                while(PunctuationAfterFootnote.IsMatch(articleText))
                {
                    articleText = PunctuationAfterFootnote.Replace(articleText, "${punc}${sfn}");
                    articleText = RefsBeforePunctuation(articleText);
                }
            }

            // clean duplicate punctuation before ref, not for !!, could be part of wiki table
            if(RefsAfterDupePunctuationQuick.IsMatch(articleText))
                articleText = RefsAfterDupePunctuation.Replace(articleText, "$1$2$3");
            if(hasFootnote && FootnoteAfterDupePunctuationQuick.IsMatch(articleText))
                articleText = FootnoteAfterDupePunctuation.Replace(articleText, "$1$2${sfn}");

            // if there have been changes need to call FixReferenceTags in case punctation moved didn't have witespace after it  
            if(!articleTextOriginal.Equals(articleText))
                articleText = FixReferenceTags(articleText);

            return articleText;
        }

        private static string RefsBeforePunctuation(string articleText)
        {
            // 'quick' regexes are used for runtime performance saving
            if(RefsBeforePunctuationQuick.IsMatch(articleText))
            {
                while(RefsBeforePunctuationR.IsMatch(articleText))
                {
                    articleText = RefsBeforePunctuationR.Replace(articleText, "$2$1$3");
                    articleText = RefsAfterDupePunctuation.Replace(articleText, "$1$2$3");
                }
            }
            return articleText;
        }

        /// <summary>
        /// reorders references within the article text based on the input regular expression providing matches for references that are out of numerical order
        /// </summary>
        /// <param name="articleText">the wiki text of the article</param>
        /// <param name="outofOrderRegex">a regular expression representing two references that are out of numerical order</param>
        /// <param name="referencestagindex"></param>
        /// <param name="NamedRefsIndexes"></param>
        /// <returns>the modified article text</returns>
        private static string ReorderRefs(string articleText, Regex outofOrderRegex, int referencestagindex, Dictionary<string, int> NamedRefsIndexes)
        {
            foreach (Match m in outofOrderRegex.Matches(articleText))
            {
                if(m.Groups[5].Index < referencestagindex)
                {
                    int ref1Index, ref2Index; 
                    NamedRefsIndexes.TryGetValue(m.Groups[2].Value, out ref1Index);
                    NamedRefsIndexes.TryGetValue(m.Groups[6].Value, out ref2Index);

                    if (ref1Index > ref2Index && ref1Index > 0)
                    {
                        string ref1 = m.Groups[1].Value;
                        string ref2 = m.Groups[5].Value;
                        string whitespace = m.Groups[4].Value;
                        string rptemplate1 = m.Groups[3].Value;
                        string rptemplate2 = m.Groups[7].Value;

                        articleText = articleText.Replace(ref1 + rptemplate1 + whitespace + ref2 + rptemplate2, ref2 + rptemplate2 + whitespace + ref1 + rptemplate1);
                    }
                }
                else break; // all further matches beyond refs template
            }

            return articleText;
        }

        private static readonly Regex LongNamedReferences = new Regex(@"(<\s*ref\s+name\s*=\s*(?:""|')?([^<>=\r\n]+?)(?:""|')?\s*>\s*([^<>]{30,}?)\s*<\s*/\s*ref>)", RegexOptions.Compiled);

        // Covered by: DuplicateNamedReferencesTests()
        /// <summary>
        /// Where an unnamed reference is a duplicate of another named reference, set the unnamed one to use the named ref
        /// Checks for instances of named references with same ref name having different values, does not modify article text in this case
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string DuplicateNamedReferences(string articleText)
        {
            string articleTextOriginal = articleText;
            Dictionary<string, string> NamedRefs = new Dictionary<string, string>();

            for (; ; )
            {
                bool reparse = false;
                NamedRefs.Clear();
                int reflistIndex = RefsTemplateIndex(articleText);

                foreach (Match m in GetNamedRefs(articleText).Where(m => m.Groups[3].Value.Length > 0))
                {
                    string refName = m.Groups[2].Value, namedRefValue = m.Groups[3].Value;

                    if (!NamedRefs.ContainsKey(namedRefValue))
                    {
                        // check for instances of named references with same ref name having different values: requires manual correction of article
                        if(NamedRefs.ContainsValue(refName))
                            return articleTextOriginal;
                        
                        NamedRefs.Add(namedRefValue, refName);
                    }
                    else
                    {
                        // we've already seen this reference, can condense later ones
                        string name2;
                        NamedRefs.TryGetValue(namedRefValue, out name2);

                        if (!string.IsNullOrEmpty(name2) && name2.Equals(refName) && namedRefValue.Length >= 25)
                        {
                            // don't condense refs in {{reflist...}}
                            if (reflistIndex > 0 && m.Index > reflistIndex)
                                continue;

                            if (m.Index > articleText.Length)
                            {
                                reparse = true;
                                break;
                            }

                            // duplicate citation fixer (both named): <ref name="Fred">(...)</ref>....<ref name="Fred">\2</ref> --> ..<ref name="Fred"/>, minimum 25 characters to avoid short refs
                            string texttomatch = articleText.Substring(0, m.Index);
                            string textaftermatch = articleText.Substring(m.Index);

                            if (textaftermatch.Contains(m.Value))
                                articleText = texttomatch + textaftermatch.Replace(m.Value, @"<ref name=""" + refName + @"""/>");
                            else
                            {
                                reparse = true;
                                break;
                            }
                        }
                    }
                }

                // duplicate citation fixer (first named): <ref name="Fred">(...)</ref>....<ref>\2</ref> --> ..<ref name="Fred"/>
                // duplicate citation fixer (second named): <ref>(...)</ref>....<ref name="Fred">\2</ref> --> ..<ref name="Fred"/>
                List<Match> unr = (from Match m in GetUnnamedRefs(articleText) select m).ToList();

                unr = unr.Where(m => NamedRefs.ContainsKey(m.Groups[1].Value.Trim())).ToList();

                foreach(Match m in unr)
                {
                    string newname;
                    if(NamedRefs.TryGetValue(m.Groups[1].Value.Trim(), out newname))
                        articleText = articleText.Replace(m.Value, @"<ref name=""" + newname + @"""/>");
                }

                if (!reparse)
                    break;
            }

            return articleText;
        }

        private const string RefName = @"(?si)<\s*ref\s+name\s*=\s*(?:""|')?";

        /// <summary>
        /// Checks for named references
        /// </summary>
        /// <param name="articleText">The article text</param>
        /// <returns>Whether the text contains named references</returns>
        public static bool HasNamedReferences(string articleText)
        {
            return WikiRegexes.NamedReferences.IsMatch(WikiRegexes.Comments.Replace(articleText, ""));
        }

        /// <summary>
        /// Derives and sets a reference name per [[WP:REFNAME]] for duplicate &lt;ref&gt;s
        /// </summary>
        /// <param name="articleText">the text of the article</param>
        /// <returns>the modified article text</returns>
        public static string DuplicateUnnamedReferences(string articleText)
        {
            /* On en-wiki AWB is asked not to add named references to an article if there are none currently, as some users feel
             * this is a change of citation style, so is against the [[WP:CITE]] "don't change established style" guidelines */
            if (Variables.LangCode.Equals("en") && !HasNamedReferences(articleText))
                return articleText;

            // get list of all unnamed refs, then filter to only those with duplicate ref content
            List<Match> allRefs = (from Match m in GetUnnamedRefs(articleText) select m).ToList();

            allRefs = allRefs.GroupBy(m => m.Groups[1].Value.Trim()).Where(g => g.Count() > 1).SelectMany(m => m).ToList();

            // do not apply to refs with ibid/loc cit etc.
            allRefs = allRefs.Where(m => !WikiRegexes.IbidLocCitation.IsMatch(m.Value)).ToList();

            // now process the duplicate refs, add ref name to first and condense the later ones
            Dictionary<string, string> refNameContent = new Dictionary<string, string>();

            foreach(Match m in allRefs)
            {
                string innerText = m.Groups[1].Value.Trim(), friendlyName;
                if(!refNameContent.TryGetValue(innerText, out friendlyName))
                {
                    friendlyName = DeriveReferenceName(articleText, innerText);

                    // check reference name was derived
                    if(friendlyName.Length <= 3)
                        continue;

                    Tools.ReplaceOnce(ref articleText, m.Value, @"<ref name=""" + friendlyName + @""">" + innerText + "</ref>");
                    refNameContent.Add(innerText, friendlyName);
                } 
                else
                {
                    articleText = articleText.Replace(m.Value, @"<ref name=""" + friendlyName + @"""/>");
                }
            }
         
            return articleText;
        }

        private static readonly Regex PageRef = new Regex(@"\s*(?:(?:[Pp]ages?|[Pp][pg]?[:\.]?)|^)\s*[XVI\d]", RegexOptions.Compiled);
        private static readonly Regex RefNameFromGroup = new Regex(@"<\s*ref\s+name\s*=\s*(?<nm>[^<>]*?)\s*group|group\s*=\s*[^<>]*?\s*name\s*=\s*(?<nm>[^<>]*?)\s*/?\s*>");

        /// <summary>
        /// Corrects named references where the reference text is the same but the reference name is different
        /// </summary>
        /// <param name="articleText">the wiki text of the page</param>
        /// <returns>the updated wiki text</returns>
        public static string SameRefDifferentName(string articleText)
        {
            string articleTextOriginal = articleText;
            // refs with same name, but one is very short, so just change to <ref name=foo/> notation
            articleText = SameNamedRefShortText(articleText);

            // get named refs, convert to keyvaluepair list of ref name and ref content
            List<KeyValuePair<string, string>> namedRefsList = GetNamedRefs(articleText).Where(m => m.Groups[3].Value.Length > 0).Select(m => new KeyValuePair<string, string>(m.Groups[2].Value, m.Groups[3].Value)).ToList();

            // filter list to those where ref content occurs more than once
            namedRefsList = namedRefsList.GroupBy(a => a.Value).Where(g => g.Count() > 1).SelectMany(a => a).ToList();

            if(!namedRefsList.Any())
                return articleText;

            // get list of all ref names used in group refs, cannot change these
            List<string> RefsInGroupRef = (from Match m in WikiRegexes.RefsGrouped.Matches(articleText) 
                select RefNameFromGroup.Match(m.Value).Groups["nm"].Value.Trim(@"'""".ToCharArray())).ToList();

            Dictionary<string, string> NamedRefs = new Dictionary<string, string>();

            foreach(KeyValuePair<string, string> kvp in namedRefsList)
            {
                string refname = kvp.Key, refvalue = kvp.Value, existingname;

                if (!NamedRefs.ContainsKey(refvalue))
                {
                    NamedRefs.Add(refvalue, refname);
                    continue;
                }

                NamedRefs.TryGetValue(refvalue, out existingname);

                // don't apply to ibid short ref, don't change if ref name used in a group ref
                if (existingname.Length > 0 && !existingname.Equals(refname) && !WikiRegexes.IbidLocCitation.IsMatch(refvalue) && !RefsInGroupRef.Contains(existingname))
                {
                    string newRefName = refname, oldRefName = existingname;

                    // use longest ref name as the one to keep
                    if ((existingname.Length > refname.Length && !existingname.Contains("autogenerated")
                         && !existingname.Contains("ReferenceA")) || (refname.Contains("autogenerated")
                                                                     || refname.Contains("ReferenceA")))
                    {
                        newRefName = existingname;
                        oldRefName = refname;
                    }

                    // rename the named ref and any short named refs of same name (format <ref name="Foo" />)
                    Regex a = new Regex(@"<\s*ref\s+name\s*=\s*(?:""|')?" + Regex.Escape(oldRefName) + @"(?:""|')?\s*(?=/\s*>|>\s*" + Regex.Escape(refvalue) + @"\s*</ref>)");

                    articleText = a.Replace(articleText, @"<ref name=""" + newRefName + @"""");
                }
            }

            // performance: only call if changes made
            if(!articleTextOriginal.Equals(articleText))
                articleText = DuplicateNamedReferences(articleText);

            return articleText;
        }

        private static readonly Regex ShortNameReference = new Regex(@"(<\s*ref\s+name\s*=\s*(?:""|')?([^<>=\r\n/]+?)(?:""|')?\s*>\s*([^<>]{1,9}?|\[?[Ss]ee above\]?|{{\s*[Cc]ite *\w+\s*}})\s*<\s*/\s*ref>)");

        /// <summary>
        /// refs with same name, but one is very short, so just change to &lt;ref name=foo/&gt; notation
        /// </summary>
        /// <param name="articleText">the wiki text of the page</param>
        /// <returns>the update wiki text</returns>
        private static string SameNamedRefShortText(string articleText)
        {
            // Peformance: get a list of all the short named refs that could be condensed
            // then only attempt replacement if some found and matching long named refs found
            string justNamedRefs = string.Join("", GetNamedRefs(articleText).Select(m => m.Value).ToArray());
            List<string> ShortNamed = (from Match m in ShortNameReference.Matches(justNamedRefs) select m.Groups[2].Value).ToList();

            if(ShortNamed.Any())
            {
                foreach (Match m in LongNamedReferences.Matches(articleText))
                {
                    string refname = m.Groups[2].Value;

                   // don't apply if short ref is a page ref
                   if(ShortNamed.Contains(refname) && m.Groups[3].Value.Length > 30)
                      articleText = Regex.Replace(articleText, @"(<\s*ref\s+name\s*=\s*(?:""|')?(" + Regex.Escape(refname) + @")(?:""|')?\s*>\s*([^<>]{1,9}?|\[?[Ss]ee above\]?|{{\s*[Cc]ite *\w+\s*}})\s*<\s*/\s*ref>)",
                                                    m2=> PageRef.IsMatch(m2.Groups[3].Value) ? m2.Value : @"<ref name=""" + refname + @"""/>");
                }
            }

            return articleText;
        }

        /// <summary>
        /// Extracts strings from an input string using the input regex to derive a name for a reference
        /// </summary>
        /// <param name="reference">value of the reference needing a name</param>
        /// <param name="referenceNameMask">regular expression to apply</param>
        /// <param name="components">number of groups to extract</param>
        /// <returns>the derived reference name</returns>
        private static string ExtractReferenceNameComponents(string reference, Regex referenceNameMask, int components)
        {
            string referenceName = "";

            if (referenceNameMask.Matches(reference).Count > 0)
            {
                Match m = referenceNameMask.Match(reference);

                referenceName = m.Groups[1].Value;

                if (components > 1)
                    referenceName += " " + m.Groups[2].Value;

                if (components > 2)
                    referenceName += " " + m.Groups[3].Value;
            }

            return CleanDerivedReferenceName(referenceName);
        }

        private const string CharsToTrim = @".;: {}[]|`?\/$’‘-_–=+,";
        // U230A is Floor Left; U230B is Floor Right
        private static readonly Regex CommentOrFloorNumber = new Regex(@"(\<\!--.*?--\>|" + "\u230A" + @"{3,}\d+" + "\u230B" + "{3,})", RegexOptions.Compiled);
        private static readonly Regex SequenceOfQuotesInDerivedName = new Regex(@"(''+|[“‘”""\[\]\(\)\<\>" + "\u230A\u230B" + "])", RegexOptions.Compiled);
        private static readonly Regex WhitespaceInDerivedName = new Regex(@"(\s{2,}|&nbsp;|\t|\n)", RegexOptions.Compiled);
        private static readonly Regex DateRetrievedOrAccessed = new Regex(@"(?im)(\s*(date\s+)?(retrieved|accessed)\b|^\d+$)", RegexOptions.Compiled);

        /// <summary>
        /// Removes various unwanted punctuation and comment characters from a derived reference name
        /// </summary>
        /// <param name="derivedName">the input reference name</param>
        /// <returns>the cleaned reference name</returns>
        private static string CleanDerivedReferenceName(string derivedName)
        {
            derivedName = WikiRegexes.PipedWikiLink.Replace(derivedName, "$2"); // piped wikilinks -> text value

            derivedName = CommentOrFloorNumber.Replace(derivedName, "");
            // rm comments from ref name, might be masked
            derivedName = derivedName.Trim(CharsToTrim.ToCharArray());
            derivedName = SequenceOfQuotesInDerivedName.Replace(derivedName, ""); // remove chars
            derivedName = WhitespaceInDerivedName.Replace(derivedName, " "); // spacing fixes
            derivedName = derivedName.Replace(@"&ndash;", "–");

            Parsers p = new Parsers();
            derivedName = p.FixDatesA(derivedName);
            derivedName = p.FixDatesB(derivedName, false, false);

            return DateRetrievedOrAccessed.IsMatch(derivedName) ? "" : derivedName;
        }

        private const string NameMask = @"(?-i)\s*(?:sir)?\s*((?:[A-Z]+\.?){0,3}\s*[A-Z][\w-']{2,}[,\.]?\s*(?:\s+\w\.?|\b(?:[A-Z]+\.?){0,3})?(?:\s+[A-Z][\w-']{2,}){0,3}(?:\s+\w(?:\.?|\b)){0,2})\s*(?:[,\.'&;:\[\(“`]|et\s+al)(?i)[^{}<>\n]*?";
        private const string YearMask = @"(\([12]\d{3}\)|\b[12]\d{3}[,\.\)])";
        private const string PageMask = @"('*(?:p+g?|pages?)'*\.?'*(?:&nbsp;)?\s*(?:\d{1,3}|(?-i)[XVICM]+(?i))\.?(?:\s*[-/&\.,]\s*(?:\d{1,3}|(?-i)[XVICM]+(?i)))?\b)";

        private static readonly Regex CitationCiteBook = new Regex(@"{{[Cc]it[ae]((?>[^\{\}]+|\{(?<DEPTH>)|\}(?<-DEPTH>))*(?(DEPTH)(?!))}})", RegexOptions.Compiled);
        private static readonly Regex CiteTemplatePagesParameter = new Regex(@"(?<=\s*pages?\s*=\s*)([^{}\|<>]+?)(?=\s*(?:\||}}))", RegexOptions.Compiled);
        private static readonly Regex UrlShortDescription = new Regex(@"\s*[^{}<>\n]*?\s*\[*(?:https?://www\.|https?://|www\.)[^\[\]<>""\s]+?\s+([^{}<>\[\]]{4,35}?)\s*(?:\]|<!--|\u230A\u230A\u230A\u230A)", RegexOptions.Compiled);
        private static readonly Regex UrlDomain = new Regex(@"\s*\w*?[^{}<>]{0,4}?\s*(?:\[?|\{\{\s*cit[^{}<>]*\|\s*url\s*=\s*)\s*(?:https?://www\.|https?://|www\.)([^\[\]<>""\s\/:]+)", RegexOptions.Compiled);
        private static readonly Regex HarvnbTemplate = new Regex(@"\s*{{ *(?:[Hh]arv(?:(?:col)?(?:nb|txt)|ard citation no brackets)?|[Ss]fn)\s*\|\s*([^{}\|]+?)\s*\|(?:[^{}]*?\|)?\s*(\d{4})\s*(?:\|\s*(?:pp?\s*=\s*)?([^{}\|]+?)\s*)?}}\s*", RegexOptions.Compiled);
        private static readonly Regex WholeShortReference = new Regex(@"\s*([^<>{}]{4,35})\s*", RegexOptions.Compiled);
        private static readonly Regex CiteTemplateUrl = new Regex(@"\s*\{\{\s*cit[^{}<>]*\|\s*url\s*=\s*([^\/<>{}\|]{4,35})", RegexOptions.Compiled);
        private static readonly Regex NameYearPage = new Regex(NameMask + YearMask + @"[^{}<>\n]*?" + PageMask + @"\s*", RegexOptions.Compiled);
        private static readonly Regex NamePage = new Regex(NameMask + PageMask + @"\s*", RegexOptions.Compiled);
        private static readonly Regex NameYear = new Regex(NameMask + YearMask + @"\s*", RegexOptions.Compiled);
        private static readonly Regex CiteDOIPMID = Tools.NestedTemplateRegex(new[] { "cite doi", "cite pmid" });

        /// <summary>
        /// Derives a name for a reference by searching for author names and dates, or website base URL etc.
        /// </summary>
        /// <param name="articleText">text of article, to check the derived name is not already used for some other reference</param>
        /// <param name="reference">the value of the reference a name is needed for</param>
        /// <returns>the derived reference name, or null if none could be determined</returns>
        public static string DeriveReferenceName(string articleText, string reference)
        {
            string derivedReferenceName = "";

            // try parameters from a citation: lastname, year and page
            string citationTemplate = CitationCiteBook.Match(reference).Value;

            if (citationTemplate.Length > 10)
            {
                string last = Tools.GetTemplateParameterValue(reference, "last");

                if (last.Length < 1)
                {
                    last = Tools.GetTemplateParameterValue(reference, "author");
                }

                if (last.Length > 1)
                {
                    derivedReferenceName = last;
                    string year = Tools.GetTemplateParameterValue(reference, "year");

                    string pages = CiteTemplatePagesParameter.Match(reference).Value.Trim();

                    if (year.Length > 3)
                        derivedReferenceName += " " + year;
                    else
                    {
                        string date = YearOnly.Match(Tools.GetTemplateParameterValue(reference, "date")).Value;

                        if (date.Length > 3)
                            derivedReferenceName += " " + date;
                    }

                    if (pages.Length > 0)
                        derivedReferenceName += " " + pages;

                    derivedReferenceName = CleanDerivedReferenceName(derivedReferenceName);
                }
                // otherwise try title
                else
                {
                    string title = Tools.GetTemplateParameterValue(reference, "title");

                    if (title.Length > 3 && title.Length < 35)
                        derivedReferenceName = title;
                    derivedReferenceName = CleanDerivedReferenceName(derivedReferenceName);

                    // try publisher
                    if (derivedReferenceName.Length < 4)
                    {
                        title = Tools.GetTemplateParameterValue(reference, "publisher");

                        if (title.Length > 3 && title.Length < 35)
                            derivedReferenceName = title;
                        derivedReferenceName = CleanDerivedReferenceName(derivedReferenceName);
                    }

                    // try work
                    if (derivedReferenceName.Length < 4)
                    {
                        title = Tools.GetTemplateParameterValue(reference, "work");

                        if (title.Length > 3 && title.Length < 35)
                            derivedReferenceName = title;
                        derivedReferenceName = CleanDerivedReferenceName(derivedReferenceName);
                    }
                }
            }

            if (ReferenceNameValid(articleText, derivedReferenceName))
                return derivedReferenceName;

            // try description of a simple external link
            derivedReferenceName = ExtractReferenceNameComponents(reference, UrlShortDescription, 1);

            if (ReferenceNameValid(articleText, derivedReferenceName))
                return derivedReferenceName;

            // website URL first, allowing a name before link
            derivedReferenceName = ExtractReferenceNameComponents(reference, UrlDomain, 1);

            if (ReferenceNameValid(articleText, derivedReferenceName))
                return derivedReferenceName;

            // Harvnb template {{Harvnb|Young|1852|p=50}}
            derivedReferenceName = ExtractReferenceNameComponents(reference, HarvnbTemplate, 3);

            if (ReferenceNameValid(articleText, derivedReferenceName))
                return derivedReferenceName;

            // cite pmid / cite doi
            derivedReferenceName = Regex.Replace(ExtractReferenceNameComponents(reference, CiteDOIPMID, 3), @"[Cc]ite (pmid|doi)\s*\|\s*", "$1");

            if (ReferenceNameValid(articleText, derivedReferenceName))
                return derivedReferenceName;

            // now just try to use the whole reference if it's short (<35 characters)
            if (reference.Length < 35)
                derivedReferenceName = ExtractReferenceNameComponents(reference, WholeShortReference, 1);

            if (ReferenceNameValid(articleText, derivedReferenceName))
                return derivedReferenceName;

            //now try title of a citation
            derivedReferenceName = ExtractReferenceNameComponents(reference, CiteTemplateUrl, 1);

            if (ReferenceNameValid(articleText, derivedReferenceName))
                return derivedReferenceName;

            // name...year...page. Regex checks to avoid excessive backtracking
            if(Regex.IsMatch(reference, YearMask) && NameYear.IsMatch(reference))
                derivedReferenceName = ExtractReferenceNameComponents(reference, NameYearPage, 3);

            if (ReferenceNameValid(articleText, derivedReferenceName))
                return derivedReferenceName;

            // name...page
            if(Regex.IsMatch(reference, PageMask))
                derivedReferenceName = ExtractReferenceNameComponents(reference, NamePage, 2);

            if (ReferenceNameValid(articleText, derivedReferenceName))
                return derivedReferenceName;

            // name...year
            if(Regex.IsMatch(reference, YearMask))
                derivedReferenceName = ExtractReferenceNameComponents(reference, NameYear, 2);

            if (ReferenceNameValid(articleText, derivedReferenceName))
                return derivedReferenceName;

            // generic ReferenceA
            derivedReferenceName = @"ReferenceA";

            if (ReferenceNameValid(articleText, derivedReferenceName))
                return derivedReferenceName;

            // generic ReferenceB
            derivedReferenceName = @"ReferenceB";

            if (ReferenceNameValid(articleText, derivedReferenceName))
                return derivedReferenceName;

            // generic ReferenceC
            derivedReferenceName = @"ReferenceC";

            return ReferenceNameValid(articleText, derivedReferenceName) ? derivedReferenceName : "";
        }

        /// <summary>
        /// Checks the validity of a new reference name:
        /// Name at least 3 characters and not already used in article, not just 'http'
        /// </summary>
        /// <param name="articleText">The article text</param>
        /// <param name="derivedReferenceName">The reference name</param>
        /// <returns>Whether the article does not already have a reference of that name</returns>
        private static bool ReferenceNameValid(string articleText, string derivedReferenceName)
        {
            return !Regex.IsMatch(articleText, RefName + Regex.Escape(derivedReferenceName) + @"(?:""|')?\s*/?\s*>") && derivedReferenceName.Length >= 3
                && !derivedReferenceName.Equals("http");
        }

        // Covered by: FootnotesTests.TestFixReferenceListTags()
        private static string ReflistMatchEvaluator(Match m)
        {
            // don't change anything if div tags mismatch
            if(DivStart.Matches(m.Value).Count != DivEnd.Matches(m.Value).Count)
                return m.Value;

            // {{reflist}} template not used on sv-wiki
            if(Variables.LangCode == "sv")
                return "<references/>";

            if(m.Value.Contains("references-2column") || m.Value.Contains("column-count:2"))
                return "{{Reflist|2}}";

            return "{{Reflist}}";
        }

        /// <summary>
        /// Main regex for {{Reflist}} converter
        /// </summary>
        private static readonly Regex ReferenceListTags = new Regex(@"(<(span|div)( class=""(references-small|small|references-2column)|)?""(?:\s*style\s*=\s*""[^<>""]+?""\s*)?>[\r\n\s]*){1,2}[\r\n\s]*<references[\s]?/>([\r\n\s]*</(span|div)>){1,2}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex ReferenceListSmallTags = new Regex(@"(<small>[\r\n\s]*){1,2}[\r\n\s]*<references[\s]?/>([\r\n\s]*</small>){1,2}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex DivStart = new Regex(@"<div\b.*?>", RegexOptions.Compiled);
        private static readonly Regex DivEnd = new Regex(@"< ?/ ?div\b.*?>", RegexOptions.Compiled);

        // Covered by: FootnotesTests.TestFixReferenceListTags()
        /// <summary>
        /// Replaces various old reference tag formats, with the new {{Reflist}}, or &lt;references/&gt; for sv-wiki
        /// </summary>
        /// <param name="articleText">The wiki text of the article</param>
        /// <returns>The updated article text</returns>
        public static string FixReferenceListTags(string articleText)
        {
            // check for performance
            if(articleText.IndexOf(@"<references", StringComparison.OrdinalIgnoreCase) < 0)
                return articleText;

            articleText = ReferenceListSmallTags.Replace(articleText, ReflistMatchEvaluator);
            return ReferenceListTags.Replace(articleText, ReflistMatchEvaluator);
        }

        private static readonly Regex EmptyReferences = new Regex(@"(<ref\s+name\s*=\s*[^<>=\r\n]+?)\s*(?:/\s*)?>\s*<\s*/\s*ref\s*>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Covered by: FootnotesTests.TestSimplifyReferenceTags()
        /// <summary>
        /// Replaces reference tags in the form &lt;ref name="blah">&lt;/ref> with &lt;ref name="blah" />
        /// Removes some of the MW errors that occur from the prior
        /// </summary>
        /// <param name="articleText">The wiki text of the article</param>
        /// <returns>The updated article text</returns>
        public static string SimplifyReferenceTags(string articleText)
        {
            return EmptyReferences.Replace(articleText, @"$1 />");
        }

        private static readonly Regex LinksHeading = new Regex(@"(?sim)(==+\s*)Links(\s*==+\s*(?:^(?:\*|\d\.?)?\s*\[?\s*http://))", RegexOptions.Compiled);
        private static readonly Regex ReferencesHeadingLevel2 = new Regex(@"(?i)==\s*'*\s*References?\s*'*\s*==", RegexOptions.Compiled);
        private static readonly Regex ReferencesHeadingLevelLower = new Regex(@"(?i)(==+\s*'*\s*References?\s*'*\s*==+)", RegexOptions.Compiled);
        private static readonly Regex ExternalLinksHeading = new Regex(@"(?im)(^\s*=+\s*(?:External\s+link|Source|Web\s*link)s?\s*=)", RegexOptions.Compiled);
        private static readonly Regex ExternalLinksToReferences = new Regex(@"(?sim)(^\s*=+\s*(?:External\s+link|Source|Web\s*link)s?\s*=+.*?)(\r\n==+References==+\r\n{{Reflist}})", RegexOptions.Compiled);
        private static readonly Regex Category = new Regex(@"(?im)(^\s*\[\[\s*Category\s*:)", RegexOptions.Compiled);
        private static readonly Regex CategoryToReferences = new Regex(@"(?sim)((?:^\{\{(?!(?:[Tt]racklist|[Ss]\-end)\b)[^{}]+?\}\}\s*)*)(^\s*\[\[\s*Category\s*:.*?)(\r\n==+References==+\r\n{{Reflist}})", RegexOptions.Compiled);

        private static readonly Regex ReferencesMissingSlash = new Regex(@"<\s*[Rr]eferences\s*>", RegexOptions.Compiled);

        /// <summary>
        /// First checks for a &lt;references&lt; missing '/' to correct, otherwise:
        /// if the article uses cite references but has no recognised template to display the references, add {{Reflist}} in the appropriate place
        /// </summary>
        /// <param name="articleText">The wiki text of the article</param>
        /// <returns>The updated article text</returns>
        public static string AddMissingReflist(string articleText)
        {
            if (!IsMissingReferencesDisplay(articleText) || !Variables.LangCode.Equals("en"))
                return articleText;

            if (ReferencesMissingSlash.IsMatch(articleText))
                return ReferencesMissingSlash.Replace(articleText, @"<references/>");

            // Rename ==Links== to ==External links==
            articleText = LinksHeading.Replace(articleText, "$1External links$2");

            // add to any existing references section if present
            if (ReferencesHeadingLevel2.IsMatch(articleText))
                articleText = ReferencesHeadingLevelLower.Replace(articleText, "$1\r\n{{Reflist}}");
            else
            {
                articleText += "\r\n==References==\r\n{{Reflist}}";

                // now sort metadata in case Category at top of article
                Parsers p = new Parsers();
                articleText = p.SortMetaData(articleText, "A", false);

                // try to move just above external links
                if (ExternalLinksHeading.IsMatch(articleText))
                    articleText = ExternalLinksToReferences.Replace(articleText, "$2\r\n$1");
                else if (Category.IsMatch(articleText))
                    // try to move just above categories
                    articleText = CategoryToReferences.Replace(articleText, "$3\r\n$1$2");
                else // not moved, so extra blank line required before heading
                    articleText = articleText.Replace("\r\n==References==", "\r\n\r\n==References==");
            }

            return articleText;
        }

        private static readonly RegexReplacement[] RefSimple = {
            new RegexReplacement(new Regex(@"<\s*(?:\s+ref\s*|\s*ref\s+)>",  RegexOptions.Singleline), "<ref>"),
            // <ref name="Fred" /ref> --> <ref name="Fred"/>
            new RegexReplacement(new Regex(@"(<\s*ref\s+name\s*=\s*""[^<>=""\/]+?"")\s*/\s*(?:ref|/)\s*>",  RegexOptions.Singleline | RegexOptions.IgnoreCase), "$1/>"),

            // <ref name="Fred""> --> <ref name="Fred">
            new RegexReplacement(new Regex(@"(<\s*ref\s+name\s*=\s*""[^<>=""\/]+?"")["">]\s*(/?)>",  RegexOptions.Singleline | RegexOptions.IgnoreCase), "$1$2>"),

            // <ref name = ”Fred”> --> <ref name="Fred">
            new RegexReplacement(new Regex(@"(<\s*ref\s+name\s*=\s*)(?:[“‘”’]+(?<val>[^<>=""\/]+?)[“‘”’]*|[“‘”’]*(?<val>[^<>=""\/]+?)[“‘”’]+)(\s*/?>)",  RegexOptions.Singleline | RegexOptions.IgnoreCase), @"$1""${val}""$2"),

            // <ref name = ''Fred'> --> <ref name="Fred"> (two apostrophes) or <ref name = 'Fred''> --> <ref name="Fred"> (two apostrophes)
            new RegexReplacement(new Regex(@"(<\s*ref\s+name\s*=\s*)(?:''+(?<val>[^<>=""\/]+?)'+|'+(?<val>[^<>=""\/]+?)''+)(\s*/?>)",  RegexOptions.Singleline | RegexOptions.IgnoreCase), @"$1""${val}""$2"),

            // <ref name=foo bar> --> <ref name="foo bar">, match when spaces
            new RegexReplacement(new Regex(@"(<\s*ref\s+name\s*=\s*)([^<>=""'\/]+?\s+[^<>=""'\/\s]+?)(\s*/?>)",  RegexOptions.Singleline | RegexOptions.IgnoreCase), @"$1""$2""$3"),

            // <ref name=foo bar> --> <ref name="foo bar">, match when non-ASCII characters ([\x00-\xff]*)
            new RegexReplacement(new Regex(@"(<\s*ref\s+name\s*=\s*)([^<>=""'\/]*?[^\x00-\xff]+?[^<>=""'\/]*?)(\s*/?>)",  RegexOptions.Singleline | RegexOptions.IgnoreCase), @"$1""$2""$3"),

            // <ref name=foo bar"> --> <ref name="foo bar"> or <ref name="foo bar> --> <ref name="foo bar">
            new RegexReplacement(new Regex(@"(<\s*ref\s+name\s*=\s*)(?:['`”]*(?<val>[^<>=""\/]+?)""|""(?<val>[^<>=""\/]+?)['`”]*)(\s*/?>)",  RegexOptions.Singleline | RegexOptions.IgnoreCase), @"$1""${val}""$2"),

            // <ref name "foo bar"> --> <ref name="foo bar">
            new RegexReplacement(new Regex(@"(<\s*ref\s+name\s*)[\+\-]?(\s*""[^<>=""\/]+?""\s*/?>)",  RegexOptions.Singleline | RegexOptions.IgnoreCase), @"$1=$2"),

            // <ref "foo bar"> --> <ref name="foo bar">
            new RegexReplacement(new Regex(@"(<\s*ref\s+)=?\s*(""[^<>=""\/]+?""\s*/?>)",  RegexOptions.Singleline | RegexOptions.IgnoreCase), "$1name=$2"),

            // ref name typos
            new RegexReplacement(new Regex(@"(<\s*ref\s+n)(me\s*=)",  RegexOptions.IgnoreCase), "$1a$2"),
            
            // <ref name="Fred" Smith> --> <ref name="Fred Smith">
            new RegexReplacement(new Regex(@"(<\s*ref\s+name\s*=\s*""[^<>=""\/]+?)""([^<>=""\/]{2,}?)(?<!\s+)(?=\s*/?>)",  RegexOptions.IgnoreCase), @"$1$2"""),
            
            // <ref name-"Fred"> --> <ref name="Fred">
            new RegexReplacement(new Regex(@"(<\s*ref\s+name\s*)-"), "$1="),
            
            // <ref NAME= --> <ref name=
            // <refname= --> <ref name=
            new RegexReplacement(new Regex(@"<\s*ref(?:\s+NAME|name)(\s*=)"), "<ref name$1"),
            
            // empty ref name: <ref name=>
            new RegexReplacement(new Regex(@"<\s*ref\s+name\s*=\s*>"), "<ref>")
        };

        // Matches possibly bad ref tags, but not the most common valid formats
        private static readonly Regex PossiblyBadRefTags = new Regex(@"<\s*[Rr][Ee][Ff][^<>]*>(?<!(?:<ref name *= *[\w0-9\-.]+( ?/)?>|<ref>|<ref name *= *""[^{}""<>]+""( ?/)?>))");
        // <ref>...<ref/> or <ref>...</red> --> <ref>...</ref>
        private static readonly Regex RedRef = new Regex(@"(<\s*ref(?:\s+name\s*=[^<>]*?)?\s*>[^<>""]+?)<\s*(?:/\s*red|ref\s*/)\s*>", RegexOptions.IgnoreCase);
        private static readonly Regex AllTagsSpace = new Regex(@"<[^<>]+> *");

        // Covered by TestFixReferenceTags
        /// <summary>
        /// Various fixes to the formatting of &lt;ref&gt; reference tags including case conversion and trimming excess whitespace
        /// </summary>
        /// <param name="articleText">The wiki text of the article</param>
        /// <returns>The modified article text.</returns>
        public static string FixReferenceTags(string articleText)
        {
            // Performance strategy: get all tags in article, filter down to tags that look like ref tags, ignore valid tags, only apply regexes where relevant tags are found
            List<string> AllTagsList = Tools.DeduplicateList((from Match m in AllTagsSpace.Matches(articleText)
                where m.Value.IndexOf("re", StringComparison.OrdinalIgnoreCase) > 0 
                && !m.Value.Equals("<ref>") && !m.Value.Equals("</ref>") && !m.Value.StartsWith(@"<references") && !m.Value.Contains(" group")
                                                                          select m.Value).ToList());

            // <REF>, </REF> and <Ref> to lowercase ref
            if(AllTagsList.Any(s => Regex.IsMatch(s, @"R[Ee][Ff]|r[Ee]F")))
                articleText = Regex.Replace(articleText, @"(<\s*\/?\s*)(?:R[Ee][Ff]|r[Ee]F)(\s*(?:>|name\s*=))", "$1ref$2");

            // remove any spaces between consecutive references -- WP:REFPUNC
            if(AllTagsList.Any(s => s.EndsWith(" ")))
                articleText = Regex.Replace(articleText, @"(</ref>|<ref\s*name\s*=[^{}<>]+?\s*\/\s*>) +(?=<ref(?:\s*name\s*=[^{}<>]+?\s*\/?\s*)?>)", "$1");

            // ensure a space between a reference and text (reference within a paragraph) -- WP:REFPUNC
            articleText = Regex.Replace(articleText, @"(</ref>|<ref\s*name\s*=[^{}<>]+?\s*\/\s*>)(\w)", "$1 $2");

            // remove spaces between punctuation and references -- WP:REFPUNC
            if(articleText.Contains(" <ref"))
                articleText = Regex.Replace(articleText, @"(?<=[,\.:;]) +(<ref(?:\s*name\s*=[^{}<>]+?\s*\/?\s*)?>)", "$1");

            // empty <ref>...</ref> tags
            articleText = Regex.Replace(articleText, @"<ref>\s*</ref>", "");

            // Trailing spaces at the beginning of a reference, within the reference
            if(AllTagsList.Any(s => s.EndsWith(" ")))
                articleText = Regex.Replace(articleText, @"(<ref[^<>\{\}\/]*>) +", "$1");

            // whitespace cleaning of </ref>
            if(AllTagsList.Any(s => Regex.IsMatch(s, @"<(?:\s*/(?:\s+ref\s*|\s*ref\s+)|\s+/\s*ref\s*)>")))
                articleText = Regex.Replace(articleText, @"<(?:\s*/(?:\s+ref\s*|\s*ref\s+)|\s+/\s*ref\s*)>", "</ref>");

            // trim trailing spaces at the end of a reference, within the reference
            if(articleText.Contains(@" </ref>"))
                articleText = Regex.Replace(articleText, @" +</ref>", "</ref>");

            if(AllTagsList.Any(s => s.StartsWith("<ref/>") || s.StartsWith("</red>")))
                articleText = RedRef.Replace(articleText, "$1</ref>");

            // Chinese do not use spaces to separate sentences
            if (Variables.LangCode.Equals("zh"))
                articleText = Regex.Replace(articleText, @"(</ref>|<ref\s*name\s*=[^{}<>]+?\s*\/\s*>) +", "$1");

            // Performance: apply ref tag fixes only to ref tags that might be invalid
            if(AllTagsList.Any(s => !Regex.IsMatch(s, @"(?:<ref name *= *[\w0-9\-.]+( ?/)?>|<ref name *= *""[^{}""<>]+""( ?/)?>)|</ref>")))
                articleText = PossiblyBadRefTags.Replace(articleText, FixReferenceTagsME);

            return articleText;
        }

        private static string FixReferenceTagsME(Match m)
        {
            return RefSimple.Aggregate(m.Value, (current, rr) => rr.Regex.Replace(current, rr.Replacement));
        }


        private static Queue<KeyValuePair<string, List<Match>>> GetUnnamedRefsQueue = new Queue<KeyValuePair<string, List<Match>>>();

        /// <summary>
        /// Extracts a list of all unnamed refs used in the input text
        /// </summary>
        /// <param name="articleText"></param>
        private static List<Match> GetUnnamedRefs(string articleText)
        {
            // For peformance, use cached result if available: articletext plus List matches
            List<Match> refsList = GetUnnamedRefsQueue.FirstOrDefault(q => q.Key.Equals(articleText)).Value;
            if(refsList != null)
                return refsList;

            refsList = (from Match m in WikiRegexes.UnnamedReferences.Matches(articleText) select m).ToList();

            // cache new results, then dequeue oldest if cache full
            GetUnnamedRefsQueue.Enqueue(new KeyValuePair<string, List<Match>>(articleText,  refsList));
            if(GetUnnamedRefsQueue.Count > 10)
                GetUnnamedRefsQueue.Dequeue();

            return refsList;
        }

        private static Queue<KeyValuePair<string, List<Match>>> GetNamedRefsQueue = new Queue<KeyValuePair<string, List<Match>>>();

        /// <summary>
        /// Extracts a list of all named refs, including condensed used in the input text
        /// </summary>
        /// <param name="articleText"></param>
        private static List<Match> GetNamedRefs(string articleText)
        {
            // For peformance, use cached result if available: articletext plus List matches
            List<Match> refsList = GetNamedRefsQueue.FirstOrDefault(q => q.Key.Equals(articleText)).Value;
            if(refsList != null)
                return refsList;

            refsList = (from Match m in WikiRegexes.NamedReferencesIncludingCondensed.Matches(articleText) select m).ToList();

            // cache new results, then dequeue oldest if cache full
            GetNamedRefsQueue.Enqueue(new KeyValuePair<string, List<Match>>(articleText,  refsList));
            if(GetNamedRefsQueue.Count > 10)
                GetNamedRefsQueue.Dequeue();

            return refsList;
        }

        /// <summary>
        /// Returns the number of &lt;ref&gt; references in the input text, excluding grouped refs
        /// </summary>
        /// <param name="arcticleText"></param>
        /// <returns></returns>
        private static int TotalRefsNotGrouped(string arcticleText)
        {
            return WikiRegexes.Refs.Matches(arcticleText).Count - WikiRegexes.RefsGrouped.Matches(arcticleText).Count;
        }

	}
}
