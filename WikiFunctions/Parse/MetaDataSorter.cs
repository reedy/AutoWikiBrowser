/*

Copyright (C) 2007 Martin Richards, Max Semenik et al.

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
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using WikiFunctions.TalkPages;

namespace WikiFunctions.Parse
{
	/// <summary>
	/// 
	/// </summary>
	public enum InterWikiOrderEnum
	{
		/// <summary>
		/// By order of alphabet, based on local language
		/// </summary>
		LocalLanguageAlpha,

		/// <summary>
		/// By order of alphabet, based on local language (by first word)
		/// </summary>
		LocalLanguageFirstWord,

		/// <summary>
		/// By order of alphabet, based on language code
		/// </summary>
		Alphabetical,

		/// <summary>
		/// English link is first and the rest are sorted alphabetically by language code
		/// </summary>
		AlphabeticalEnFirst
	}

	public class MetaDataSorter
	{
		/// <summary>
		/// 
		/// </summary>
		public List<string> PossibleInterwikis;

		/// <summary>
		/// 
		/// </summary>
		public bool SortInterwikis
		{ get; set; }

		/// <summary>
		/// 
		/// </summary>
		public bool AddCatKey
		{ get; set; }

		public MetaDataSorter()
		{
			SortInterwikis = true;

			if (!LoadInterWikiFromCache())
			{
				LoadInterWikiFromNetwork();
				SaveInterWikiToCache();
			}

			if (InterwikiLocalAlpha == null)
				throw new NullReferenceException("InterwikiLocalAlpha is null");

			//create a comparer
			InterWikiOrder = InterWikiOrderEnum.LocalLanguageAlpha;
		}

		// now will be generated dynamically using Variables.Stub
		private readonly Regex InterLangRegex = new Regex(@"<!--\s*(other languages?|language links?|inter ?(language|wiki)? ?links|inter ?wiki ?language ?links|(?:inter|Other) ?wikis?|The below are interlanguage links\.?|interwiki links to this article in other languages, below)\s*-->", RegexOptions.IgnoreCase);
		private readonly Regex CatCommentRegex = new Regex("<!--+ ?cat(egories)? ?--+>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		private List<string> InterwikiLocalAlpha;
		private List<string> InterwikiLocalFirst;
		private List<string> InterwikiAlpha;
		private List<string> InterwikiAlphaEnFirst;
		//List<Regex> InterWikisList = new List<Regex>();

		private InterWikiComparer Comparer;
		private InterWikiOrderEnum Order = InterWikiOrderEnum.LocalLanguageAlpha;

		/// <summary>
		/// 
		/// </summary>
		public InterWikiOrderEnum InterWikiOrder
		{//orders from https://meta.wikimedia.org/wiki/Interwiki_sorting_order
			set
			{
				Order = value;

				List<string> seq;
				switch (Order)
				{
					case InterWikiOrderEnum.Alphabetical:
						seq = InterwikiAlpha;
						break;
					case InterWikiOrderEnum.AlphabeticalEnFirst:
						seq = InterwikiAlphaEnFirst;
						break;
					case InterWikiOrderEnum.LocalLanguageAlpha:
						seq = InterwikiLocalAlpha;
						break;
					case InterWikiOrderEnum.LocalLanguageFirstWord:
						seq = InterwikiLocalFirst;
						break;
					default:
						throw new ArgumentOutOfRangeException("MetaDataSorter.InterWikiOrder",
						                                      (Exception)null);
				}
				PossibleInterwikis = SiteMatrix.GetProjectLanguages(Variables.Project);
				Comparer = new InterWikiComparer(new List<string>(seq), PossibleInterwikis);
			}
			get
			{
				return Order;
			}
		}

		private bool Loaded = true;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="what"></param>
		/// <returns></returns>
		private List<string> Load(string what)
		{
			var result = (List<string>)ObjectCache.Global.Get<List<string>>(Key(what));
			if (result == null)
			{
				Loaded = false;
				return new List<string>();
			}

			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		private void SaveInterWikiToCache()
		{
			ObjectCache.Global.Set(Key("InterwikiLocalAlpha"), InterwikiLocalAlpha);
			ObjectCache.Global.Set(Key("InterwikiLocalFirst"), InterwikiLocalFirst);
			ObjectCache.Global.Set(Key("InterwikiAlpha"), InterwikiAlpha);
			ObjectCache.Global.Set(Key("InterwikiAlphaEnFirst"), InterwikiAlphaEnFirst);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="what"></param>
		/// <returns></returns>
		private static string Key(string what)
		{
			return "MetaDataSorter::" + what;
		}

		/// <summary>
		/// Loads interwikis from local disk cache if available
		/// </summary>
		/// <returns></returns>
		private bool LoadInterWikiFromCache()
		{
		    if(!Globals.UnitTestMode)
		    {
		        InterwikiLocalAlpha = Load("InterwikiLocalAlpha");
		        InterwikiLocalFirst = Load("InterwikiLocalFirst");
		        InterwikiAlpha = Load("InterwikiAlpha");
		        InterwikiAlphaEnFirst = Load("InterwikiAlphaEnFirst");

		        return Loaded;
		    }
		    List<string> one = new List<string> { "ar", "de", "en", "ru", "sq" };
		    List<string> two = new List<string> { "en", "ar", "de", "ru", "sq" };

		    InterwikiLocalAlpha = one;
		    InterwikiLocalFirst = one;
		    InterwikiAlpha = one;
		    InterwikiAlphaEnFirst = two;

		    return true;
		}

		private static readonly CultureInfo EnUsCulture = new CultureInfo("en-US", true);

		/// <summary>
		/// 
		/// </summary>
		private void LoadInterWikiFromNetwork()
		{
			string text = !Globals.UnitTestMode
				? Tools.GetHTML("https://en.wikipedia.org/w/index.php?title=Wikipedia:AutoWikiBrowser/IW&action=raw")
				: @"<!--InterwikiLocalAlphaBegins-->
ru, sq, en
<!--InterwikiLocalAlphaEnds-->
<!--InterwikiLocalFirstBegins-->
en, sq, ru
<!--InterwikiLocalFirstEnds-->";

			string interwikiLocalAlphaRaw =
				RemExtra(Tools.StringBetween(text, "<!--InterwikiLocalAlphaBegins-->", "<!--InterwikiLocalAlphaEnds-->"));
			string interwikiLocalFirstRaw =
				RemExtra(Tools.StringBetween(text, "<!--InterwikiLocalFirstBegins-->", "<!--InterwikiLocalFirstEnds-->"));

			InterwikiLocalAlpha = new List<string>();

			foreach (string s in interwikiLocalAlphaRaw.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
			        )
			{
				InterwikiLocalAlpha.Add(s.Trim().ToLower());
			}

			InterwikiLocalFirst = new List<string>();

			foreach (string s in interwikiLocalFirstRaw.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
			        )
			{
				InterwikiLocalFirst.Add(s.Trim().ToLower());
			}

			InterwikiAlpha = new List<string>(InterwikiLocalFirst);
			InterwikiAlpha.Sort(StringComparer.Create(EnUsCulture, true));

			InterwikiAlphaEnFirst = new List<string>(InterwikiAlpha);
			InterwikiAlphaEnFirst.Remove("en");
			InterwikiAlphaEnFirst.Insert(0, "en");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="input"></param>
		/// <returns>The updated article text</returns>
		private static string RemExtra(string input)
		{
			return input.Replace("\r\n", "").Replace(">", "").Replace("\n", "");
		}

		private static readonly Regex CommentedOutEnInterwiki = new Regex("<!-- ?\\[\\[en:.*?\\]\\] ?-->");

		/// <summary>
		/// Sorts article meta data, including optional whitespace fixing
		/// </summary>
		/// <param name="articleText">The wiki text of the article.</param>
		/// <param name="articleTitle">Title of the article</param>
		/// <returns>The updated article text</returns>
		internal string Sort(string articleText, string articleTitle)
		{
			return Sort(articleText, articleTitle, true);
		}

		/// <summary>
		/// Sorts article meta data
		/// </summary>
		/// <param name="articleText">The wiki text of the article.</param>
		/// <param name="articleTitle">Title of the article</param>
		/// <param name="fixOptionalWhitespace">Whether to request optional excess whitespace to be fixed</param>
		/// <returns>The updated article text</returns>
		internal string Sort(string articleText, string articleTitle, bool fixOptionalWhitespace)
		{
		    if (Namespace.Determine(articleTitle) == Namespace.Template) // Don't sort on templates
		        return articleText;

            // Performance: get all the templates so "move template" functions below only called when template(s) present in article
            List<string> alltemplates = Parsers.GetAllTemplates(articleText);

		    // short pages monitor check for en-wiki: keep at very end of article if present
		    // See [[Template:Long comment/doc]]
		    // SPM regex quick check for performance on long pages
		    string shortPagesMonitor = "";
		    if(Variables.LangCode.Equals("en") && alltemplates.Contains("Short pages monitor"))
		    {
		        Match spm = WikiRegexes.ShortPagesMonitor.Match(articleText);
		        
		        if(spm.Success)
		        {
		            articleText = WikiRegexes.ShortPagesMonitor.Replace(articleText, "").TrimEnd();
		            shortPagesMonitor = spm.Value.TrimEnd();
		        }
		    }

			articleText = CommentedOutEnInterwiki.Replace(articleText, "");

			string personData = "";
            if(TemplateExists(alltemplates, WikiRegexes.Persondata))
                personData = Tools.Newline(RemovePersonData(ref articleText));

			string disambig = "";
            if(TemplateExists(alltemplates, WikiRegexes.Disambigs))
                disambig = Tools.Newline(RemoveDisambig(ref articleText));

			string categories = Tools.Newline(RemoveCats(ref articleText, articleTitle));

			string interwikis = Tools.Newline(Interwikis(ref articleText, TemplateExists(alltemplates, WikiRegexes.LinkFGAs))); 

			if(Namespace.IsMainSpace(articleTitle))
			{
			    // maintenance templates above infoboxes etc., zeroth section only
                if(TemplateExists(alltemplates, WikiRegexes.MaintenanceTemplates))
			    {
			        string zerothSection = Tools.GetZerothSection(articleText);
                    string restOfArticle = articleText.Substring(zerothSection.Length);
                    zerothSection = MoveMaintenanceTags(zerothSection);

                    if(TemplateExists(alltemplates, WikiRegexes.MultipleIssues))
                        zerothSection = MoveMultipleIssues(zerothSection);

                    articleText = zerothSection + restOfArticle;
			    }

            // deletion/protection templates above maintenance tags, below dablinks per [[WP:LAYOUT]]
            if(TemplateExists(alltemplates, WikiRegexes.DeletionProtectionTags))
                articleText = MoveTemplate(articleText, WikiRegexes.DeletionProtectionTags);

            // Dablinks above maintance tags per [[WP:LAYOUT]]
            if(TemplateExists(alltemplates, WikiRegexes.Dablinks))
                articleText = MoveTemplate(articleText, WikiRegexes.Dablinks);

			    if (Variables.LangCode.Equals("en"))
			    {
                    if(TemplateExists(alltemplates, WikiRegexes.PortalTemplate))
                        articleText = MovePortalTemplates(articleText);

                    if(TemplateExists(alltemplates, WikiRegexes.WikipediaBooks))
                        articleText = MoveTemplateToSeeAlsoSection(articleText, WikiRegexes.WikipediaBooks);

                    if(TemplateExists(alltemplates, WikiRegexes.SisterLinks))
                        articleText = MoveSisterlinks(articleText);

                    if(alltemplates.Contains("Ibid"))
                        articleText = MoveTemplateToReferencesSection(articleText, WikiRegexes.Ibid);

			        articleText = MoveExternalLinks(articleText);

			        articleText = MoveSeeAlso(articleText);
			    }
			}
			// two newlines here per https://en.wikipedia.org/w/index.php?title=Wikipedia_talk:AutoWikiBrowser&oldid=243224092#Blank_lines_before_stubs
			// https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#Two_empty_lines_before_stub-templates
			// ru, sl, ar, arz wikis use only one newline
			string strStub = "";
			
			// Category: can use {{Verylargestub}}/{{popstub}} which is not a stub template, don't do stub sorting
			if(!Namespace.Determine(articleTitle).Equals(Namespace.Category) && TemplateExists(alltemplates, new Regex(Variables.Stub)))
			    strStub = Tools.Newline(RemoveStubs(ref articleText), (Variables.LangCode.Equals("ru") || Variables.LangCode.Equals("sl") || Variables.LangCode.Equals("ar") || Variables.LangCode.Equals("arz")) ? 1 : 2);

			// filter out excess white space and remove "----" from end of article
            if(Namespace.IsMainSpace(articleTitle))
                articleText = articleText.TrimEnd(); // better to trim here than process more slowly in RemoveWhiteSpace where <poem> checks etc. needed
			articleText = Parsers.RemoveWhiteSpace(articleText, fixOptionalWhitespace) + "\r\n";

			articleText += disambig;
            if(TemplateExists(alltemplates, WikiRegexes.MultipleIssues))
                articleText = WikiRegexes.MultipleIssues.Replace(articleText, m=> Regex.Replace(m.Value, "(\r\n)+", "\r\n"));

			switch (Variables.LangCode)
			{
			    case "de":
			    case "sl":
			        articleText += strStub + categories + personData;

			        // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser#Removal_of_blank_lines
			        // on de wiki a blank line is desired between persondata and interwikis
			        if (Variables.LangCode.Equals("de") && personData.Length > 0 && interwikis.Length > 0)
			            articleText += "\r\n";
			        break;

			    case "ar":
			    case "arz":
			    case "cs":
			    case "el":
			    case "pl":
			    case "ru":
			    case "simple":
			        articleText += personData + strStub + categories;
			        break;
			        
			    case "it":
			        if(Variables.Project == ProjectEnum.wikiquote)
			            articleText += personData + strStub + categories;
			        else
			            articleText += personData + categories + strStub;
			        break;
			        
			    default:
			        articleText += personData + categories + strStub;
			        break;
			}
			articleText += interwikis;

			// Only trim start on Category namespace, restore any saved short page monitor text
			return (Namespace.Determine(articleTitle) == Namespace.Category ?  articleText.Trim() : articleText.TrimEnd()) + shortPagesMonitor;
		}
        
        /// <summary>
        /// Returns whether the given regex matches any of the (first name upper) templates in the given list
        /// </summary>
        private static bool TemplateExists(List<string> templatesFound, Regex r)
        {
            return templatesFound.Where(s => r.IsMatch(@"{{" + s + "}}")).Any();
        }
		
		private static readonly Regex LifeTime = Tools.NestedTemplateRegex("Lifetime");
		private static readonly Regex CatsForDeletion = new Regex(@"\[\[Category:(Pages|Categories|Articles) for deletion\]\]");

		/// <summary>
		/// Extracts DEFAULTSORT + categories from the article text; removes duplicate categories, cleans whitespace and underscores
		/// </summary>
		/// <param name="articleText">The wiki text of the article.</param>
		/// <param name="articleTitle">Title of the article</param>
		/// <returns>The cleaned page categories in a single string</returns>
		public string RemoveCats(ref string articleText, string articleTitle)
		{
		    // don't pull category from redirects to a category e.g. page Hello is #REDIRECT[[Category:Hello]]
		    string rt = Tools.RedirectTarget(articleText);
		    if(rt.Length > 0 && WikiRegexes.Category.IsMatch(@"[[" + rt + @"]]"))
				return "";

			List<string> categoryList = new List<string>();
			string originalArticleText = articleText;
			string articleTextNoComments = Tools.ReplaceWithSpaces(articleText, WikiRegexes.Comments.Matches(articleText));

			// don't operate on pages with (incorrectly) multiple defaultsorts
            // ignore commented out DEFAULTSORT – https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_12#Moving_DEFAULTSORT_in_HTML_comments
			MatchCollection mc = WikiRegexes.Defaultsort.Matches(articleTextNoComments);
			if (mc.Count > 1)
				return "";

            string defaultSort = "";
            bool defaultSortRemoved = false;

			// allow comments between categories, and keep them in the same place, only grab any comment after the last category if on same line
			// whitespace: remove all whitespace after, but leave a blank newline before a heading (rare case where category not in last section)
			
			// performance: apply regex on portion of article containing category links rather than whole text
			Match cq = WikiRegexes.CategoryQuick.Match(articleText);

			if(cq.Success)
			{
			    int cutoff = Math.Max(0, cq.Index -500);
			    string cut = articleText.Substring(cutoff);
                cut = WikiRegexes.RemoveCatsAllCats.Replace(cut, m => {
	                                                                   if (!CatsForDeletion.IsMatch(m.Value))
	                                                                       categoryList.Add(m.Value.Trim());
	                                                                   
	                                                                   // if category not at start of line, leave newline, otherwise text on next line moved up
	                                                                   if(m.Index > 2 && !cut.Substring(m.Index-2, 2).Trim().Equals(""))
	                                                                       return "\r\n";

	                                                                   return "";
                                                                      });

                // if category tidying has changed comments/nowikis return with no changes – we've pulled a cat from a comment
                if(!Tools.UnformattedTextNotChanged(originalArticleText.Substring(cutoff), cut))
                {
                    articleText = originalArticleText;
                    return "";
                }

                if (AddCatKey)
                    categoryList = CatKeyer(categoryList, articleTitle);

                // remove defaultsort now if we can, faster to remove from cut than whole articleText
                if(mc.Count > 0 && cut.Contains(mc[0].Value))
                {
                    cut = cut.Replace(mc[0].Value, "");
                    defaultSortRemoved = true;
                }

                articleText = articleText.Substring(0, cutoff) + cut;

                if(CatCommentRegex.IsMatch(cut))
                    articleText = CatCommentRegex.Replace(articleText, m =>
                                                          {
                                                              categoryList.Insert(0, m.Value);
                                                              return "";
                                                          }, 1);

			}

			if(Variables.LangCode.Equals("sl") && LifeTime.IsMatch(articleText))
			{
				defaultSort = LifeTime.Match(articleText).Value;
			}
			else if(mc.Count > 0)
					defaultSort = mc[0].Value;

			if (!string.IsNullOrEmpty(defaultSort))
			{
                // if defaultsort wasn't in the cut area before the categories, remove now
                if(!defaultSortRemoved)
			        articleText = articleText.Replace(defaultSort, "");

			    if (defaultSort.ToUpper().Contains("DEFAULTSORT"))
			        defaultSort = TalkPageFixes.FormatDefaultSort(defaultSort);
			    defaultSort += "\r\n";
			}

			// Extract any {{uncategorized}} template, but not uncat stub templates
            // remove exact duplicates
			string uncat = "";
            if(TemplateExists(Parsers.GetAllTemplates(originalArticleText), WikiRegexes.Uncat) && WikiRegexes.Uncat.IsMatch(articleTextNoComments))
			{
                articleText = WikiRegexes.Uncat.Replace(articleText, uncatm =>
                                                        {
                                                            if(WikiRegexes.PossiblyCommentedStub.IsMatch(uncatm.Value))
                                                                return uncatm.Value;

                                                            // remove exact duplicates
                                                            if(!uncat.Contains(uncatm.Value))
                                                                uncat += uncatm.Value + "\r\n";

                                                            return "";
                                                        });
			}

			return uncat + defaultSort + ListToString(categoryList);
		}

		/// <summary>
		/// Extracts the persondata template from the articleText, along with the persondata comment, if present on the line before
		/// </summary>
		/// <param name="articleText">The wiki text of the article.</param>
		/// <returns></returns>
		public static string RemovePersonData(ref string articleText)
		{
		    string strPersonData = "";

		    articleText = WikiRegexes.Persondata.Replace(articleText, m=>
		                                                 {
		                                                     strPersonData += (strPersonData.Length == 0 ? m.Value : Tools.Newline(m.Value));
		                                                     return "";
		                                                 });

			// https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#Persondata_comments
			// catch the persondata comment the line before it so that the comment and template aren't separated
			if (articleText.Contains(WikiRegexes.PersonDataCommentEN) && Variables.LangCode.Equals("en"))
			{
				articleText = articleText.Replace(WikiRegexes.PersonDataCommentEN, "");
				strPersonData = WikiRegexes.PersonDataCommentEN + strPersonData;
			}

			return strPersonData;
		}

		/// <summary>
		/// Extracts stub templates from the article text
		/// </summary>
		/// <param name="articleText">The wiki text of the article.</param>
		/// <returns></returns>
		public static string RemoveStubs(ref string articleText)
		{
			// Per https://ru.wikipedia.org/wiki/Википедия:Опросы/Использование_служебных_разделов/Этап_2#.D0.A1.D0.BB.D1.83.D0.B6.D0.B5.D0.B1.D0.BD.D1.8B.D0.B5_.D1.88.D0.B0.D0.B1.D0.BB.D0.BE.D0.BD.D1.8B
			// Russian Wikipedia places stubs before navboxes
			if (Variables.LangCode.Equals("ru"))
				return "";

			List<string> stubList = new List<string>();

            articleText = WikiRegexes.PossiblyCommentedStub.Replace(articleText, m => {
                if(!Regex.IsMatch(m.Value, Variables.SectStub))
				{
					stubList.Add(m.Value);
					return "";
				}
            
                return m.Value;
            });

			return (stubList.Count != 0) ? ListToString(stubList) : "";
		}

		/// <summary>
		/// Removes any disambiguation templates from the article text, to be added at bottom later
		/// </summary>
		/// <param name="articleText">The wiki text of the article.</param>
		/// <returns>Article text stripped of disambiguation templates</returns>
		public static string RemoveDisambig(ref string articleText)
		{
			if (Variables.LangCode != "en")
				return "";

			string strDisambig = "";
			
			// Extract up to one disambig (should not be multiple per page), don't pull out of comments
			if (WikiRegexes.Disambigs.IsMatch(WikiRegexes.Comments.Replace(articleText, "")))
			{
			    articleText = WikiRegexes.Disambigs.Replace(articleText, m =>
			                                                {
			                                                    strDisambig = m.Value;
			                                                    return "";
			                                                }, 1);
			}

			return strDisambig;
		}

		/// <summary>
		/// Moves any disambiguation links in the zeroth section to the top of the article (en only)
		/// </summary>
		/// <param name="articleText">The wiki text of the article.</param>
		/// <returns>Article text with disambiguation links at top</returns>
		public static string MoveDablinks(string articleText)
		{
            return MoveTemplate(articleText, WikiRegexes.Dablinks);
		}

        /// <summary>
        /// Moves any templates in the zeroth section to the top of the article (en only)
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="templateRegex">Regex matching the templates to  be moved</param>
        /// <returns>Article text with disambiguation links at top</returns>
        public static string MoveTemplate(string articleText, Regex templateRegex)
        {
            string originalArticletext = articleText;

            // get the zeroth section (text upto first heading)
            string zerothSection = Tools.GetZerothSection(articleText);

            // avoid moving commented out templates
            if (Variables.LangCode != "en" || !templateRegex.IsMatch(WikiRegexes.Comments.Replace(zerothSection, "")))
                return articleText;

            // get the rest of the article including first heading (may be null if article has no headings)
            string restOfArticle = articleText.Substring(zerothSection.Length);

            string strTemplates = "";

            foreach (Match m in templateRegex.Matches(zerothSection))
            {
                strTemplates += m.Value + "\r\n";

                // remove colons before template
                zerothSection = zerothSection.Replace(":" + m.Value + "\r\n", "");

                // additionally, remove whitespace after template
                zerothSection = Regex.Replace(zerothSection, Regex.Escape(m.Value) + @" *(?:\r\n)?", "");
            }

            articleText = strTemplates + zerothSection + restOfArticle;

            // avoid moving commented out templates, round 2
            if(Tools.UnformattedTextNotChanged(originalArticletext, articleText))
                return articleText;

            return originalArticletext;
        }
		
		private static readonly Regex ExternalLinksSection = new Regex(@"(^== *[Ee]xternal +[Ll]inks? *==.*?)(?=^==+[^=][^\r\n]*?[^=]==+(\r\n?|\n)$)", RegexOptions.Multiline | RegexOptions.Singleline);
		private static readonly Regex ExternalLinksToEnd = new Regex(@"(==+) *[Ee]xternal +[Ll]inks? *\1.*", RegexOptions.Singleline);

		/// <summary>
		/// Moves sisterlinks such as {{wiktionary}} to the external links section
		/// </summary>
		/// <param name="articleText">The article text</param>
		/// <returns>The updated article text</returns>
		public static string MoveSisterlinks(string articleText)
		{
		    string originalArticletext = articleText;
		    foreach (Match m in WikiRegexes.SisterLinks.Matches(articleText))
		    {
		        string sisterlinkFound = m.Value;
		        string ExternalLinksSectionString = ExternalLinksSection.Match(articleText).Value;

		        // if ExteralLinksSection didn't match then 'external links' must be last section
		        if (ExternalLinksSectionString.Length == 0)
		            ExternalLinksSectionString = ExternalLinksToEnd.Match(articleText).Value;

		        // need to have an 'external links' section to move the sisterlinks to
		        // check sisterlink NOT currently in 'external links'
		        if (ExternalLinksSectionString.Length > 0 && !ExternalLinksSectionString.Contains(sisterlinkFound.Trim()))
		        {
		            articleText = Regex.Replace(articleText, Regex.Escape(sisterlinkFound) + @"\s*(?:\r\n)?", "");
		            articleText = WikiRegexes.ExternalLinksHeader.Replace(articleText, "$0" + "\r\n" + sisterlinkFound);
		        }
		    }
		    
		    if(Tools.UnformattedTextNotChanged(originalArticletext, articleText))
		        return articleText;

		    return originalArticletext;
		}

		/// <summary>
		/// Moves maintenance tags to the top of the article text.
		/// Does not move tags when only non-infobox templates are above the last tag
		/// For en-wiki apply this to zeroth section of article only
		/// </summary>
		/// <param name="articleText">the article text</param>
		/// <returns>the modified article text</returns>
		public static string MoveMaintenanceTags(string articleText)
		{
			string originalArticleText = articleText;
			bool doMove = false;
			int lastIndex = -1;
			
			// don't pull tags from new-style {{multiple issues}} template
			string articleTextNoMI = Tools.ReplaceWithSpaces(articleText, WikiRegexes.MultipleIssues.Matches(articleText));
			
			// if all templates removed from articletext before last MaintenanceTemplates match are not infoboxes then do not change anything
			foreach(Match m in WikiRegexes.MaintenanceTemplates.Matches(articleTextNoMI))
			{
				lastIndex = m.Index;
			}

			// return if no MaintenanceTemplates to move
			if (lastIndex < 0)
				return articleText;

			string articleTextToCheck = articleText.Substring(0, lastIndex);

			foreach(Match m in WikiRegexes.NestedTemplates.Matches(articleTextToCheck))
			{
				if (Tools.GetTemplateName(m.Value).ToLower().Contains("infobox"))
				{
					doMove = true;
					break;
				}

				articleTextToCheck = articleTextToCheck.Replace(m.Value, "");
			}

			if(articleTextToCheck.Trim().Length > 0)
				doMove = true;

			if(!doMove)
				return articleText;

			string strMaintTags = "";

			foreach (Match m in WikiRegexes.MaintenanceTemplates.Matches(articleText))
			{
				if(!m.Value.Contains("section"))
				{
					strMaintTags = strMaintTags + m.Value + "\r\n";
					articleText = articleText.Replace(m.Value, "");
				}
			}

			articleText = strMaintTags + articleText;
			
			if(!Tools.UnformattedTextNotChanged(originalArticleText, articleText))
				return originalArticleText;

			return strMaintTags.Length > 0 ? articleText.Replace(strMaintTags + "\r\n", strMaintTags) : articleText;
		}

        /// <summary>
        /// Moves multiple issues template to the top of the article text.
        /// Does not move tags when only non-infobox templates are above the last tag
        /// For en-wiki apply this to zeroth section of article only
        /// </summary>
        /// <param name="articleText">the article text</param>
        /// <returns>the modified article text</returns>
        public static string MoveMultipleIssues(string articleText)
        {
            string originalArticleText = articleText;
            int multipleIssuesIndex=-1, infoboxIndex=-1;

            foreach(Match m in WikiRegexes.NestedTemplates.Matches(articleText))
            {
                if (Tools.GetTemplateName(m.Value).ToLower().Contains("infobox"))
                    infoboxIndex = m.Index;
                else if(WikiRegexes.MultipleIssues.IsMatch(m.Value))
                    multipleIssuesIndex = m.Index;
            }

            if(multipleIssuesIndex > infoboxIndex && infoboxIndex > -1)
            {
                string multipleIssues = WikiRegexes.MultipleIssues.Match(articleText).Value;

                articleText = multipleIssues + "\r\n" + articleText.Replace(multipleIssues, "");

                if(!Tools.UnformattedTextNotChanged(originalArticleText, articleText))
                    return originalArticleText;
            }

            return articleText;
        }

		private static readonly Regex SeeAlsoSection = new Regex(@"(^== *[Ss]ee also *==.*?)(?=^==[^=][^\r\n]*?[^=]==(\r\n?|\n)$)", RegexOptions.Multiline | RegexOptions.Singleline);
		private static readonly Regex SeeAlsoToEnd = new Regex(@"(\s*(==+)\s*see\s+also\s*\2 *).*", RegexOptions.IgnoreCase | RegexOptions.Singleline);

		/// <summary>
		/// Moves template calls to the top of the "see also" section of the article
		/// </summary>
		/// <param name="articleText">The article text</param>
		/// <param name="TemplateToMove">The template calls to move</param>
		/// <returns>The updated article text</returns>
		public static string MoveTemplateToSeeAlsoSection(string articleText, Regex TemplateToMove)
		{
            MatchCollection mc = TemplateToMove.Matches(articleText);
			// need to have a 'see also' section to move the template to
			if(mc.Count < 1)
				return articleText;
			
			string originalArticletext = articleText;
            bool templateMoved = false;

			foreach (Match m in mc)
			{
				string TemplateFound = m.Value;
                Match sa = SeeAlsoSection.Match(articleText);
				string seeAlsoSectionString = sa.Value;
				int seeAlsoIndex = sa.Index;

				// if SeeAlsoSection didn't match then 'see also' must be last section
				if (seeAlsoSectionString.Length == 0)
				{
                    Match sae = SeeAlsoToEnd.Match(articleText);
					seeAlsoSectionString = sae.Value;
					seeAlsoIndex = sae.Index;
				}
                
                // if still not found then no "see also" section to move templates to
                if (seeAlsoSectionString.Length == 0)
                    break;

				// only move templates NOT currently in 'see also'
				if (m.Index < seeAlsoIndex || m.Index > (seeAlsoIndex + seeAlsoSectionString.Length))
				{
					// remove template, also remove newline after template if template on its own line
					articleText = Regex.Replace(articleText, @"^" + Regex.Escape(TemplateFound) + @" *(?:\r\n)?", "", RegexOptions.Multiline);

					articleText = articleText.Replace(TemplateFound, "");

                    // place template at top of see also section
					articleText = WikiRegexes.SeeAlso.Replace(articleText, "$0" + Tools.Newline(TemplateFound));
                    templateMoved = true;
				}
			}

			if(templateMoved && Tools.UnformattedTextNotChanged(originalArticletext, articleText))
				return articleText;

			return originalArticletext;
		}
		
		/// <summary>
		/// Moves any {{XX portal}} templates to the 'see also' section, if present (en only), per Template:Portal
		/// </summary>
		/// <param name="articleText">The wiki text of the article.</param>
		/// <returns>Article text with {{XX portal}} template correctly placed</returns>
		// https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Placement_of_portal_template
		public static string MovePortalTemplates(string articleText)
		{
			return MoveTemplateToSeeAlsoSection(articleText, WikiRegexes.PortalTemplate);
		}

		private static readonly Regex ReferencesSectionRegex = new Regex(@"^== *[Rr]eferences *==\s*", RegexOptions.Multiline);
		private static readonly Regex NotesSectionRegex = new Regex(@"^== *[Nn]otes(?: and references)? *==\s*", RegexOptions.Multiline);
		private static readonly Regex FootnotesSectionRegex = new Regex(@"^== *(?:[Ff]ootnotes|Sources) *==\s*", RegexOptions.Multiline);

		/// <summary>
		/// Moves given template to the references section from the zeroth section, if present (en only)
		/// </summary>
		/// <param name="articleText">The wiki text of the article.</param>
		/// <param name="TemplateRegex">A Regex to match the template to move</param>
		/// <param name="onlyfromzerothsection">Whether to check only the zeroth section of the article for the template</param>
		/// <returns>Article text with template correctly placed</returns>
		public static string MoveTemplateToReferencesSection(string articleText, Regex TemplateRegex, bool onlyfromzerothsection)
		{
			// no support for more than one of these templates in the article
			if(TemplateRegex.Matches(articleText).Count != 1 || (onlyfromzerothsection && TemplateRegex.Matches(WikiRegexes.ZerothSection.Match(articleText).Value).Count != 1))
			    return articleText;

			// return if template is already in one the the 'References', 'Notes' or 'Footnotes' sections
			string[] sec = Tools.SplitToSections(articleText);

			foreach(string s in sec)
			{
			    if(TemplateRegex.IsMatch(s))
			    {
			        if(NotesSectionRegex.IsMatch(s) || ReferencesSectionRegex.IsMatch(s)
			           || FootnotesSectionRegex.IsMatch(s))
			            return articleText;
			    }
			}

			// find the template position
			// the template must end up in one of the 'References', 'Notes' or 'Footnotes' section
			int templatePosition = TemplateRegex.Match(articleText).Index, notesSectionPosition = NotesSectionRegex.Match(articleText).Index;

			if (notesSectionPosition > 0 && templatePosition < notesSectionPosition)
				return MoveTemplateToSection(articleText, TemplateRegex, 2);
			
			int referencesSectionPosition = ReferencesSectionRegex.Match(articleText).Index;

			if (referencesSectionPosition > 0 && templatePosition < referencesSectionPosition)
				return MoveTemplateToSection(articleText, TemplateRegex, 1);

			int footnotesSectionPosition = FootnotesSectionRegex.Match(articleText).Index;

			if (footnotesSectionPosition > 0 && templatePosition < footnotesSectionPosition)
				return MoveTemplateToSection(articleText, TemplateRegex, 3);

			return articleText;
		}

		/// <summary>
		/// Moves the given template(s) from anywhere in the article to the references section.
		/// </summary>
		/// <returns>
		/// Updated article text
		/// </returns>
		/// <param name='articleText'>
		/// Article text.
		/// </param>
		/// <param name='templateRegex'>
		/// Regex to match the template(s) to be moved
		/// </param>
		public static string MoveTemplateToReferencesSection(string articleText, Regex templateRegex)
		{
			return MoveTemplateToReferencesSection(articleText, templateRegex, false);
		}

		/// <summary>
		/// Moves the given template(s) to the required section.
		/// </summary>
		/// <returns>
		/// Updated article text
		/// </returns>
		/// <param name='articleText'>
		/// Article text.
		/// </param>
		/// <param name='templateRegex'>
		/// Regex to match the template(s) to be moved
		/// </param>
		/// <param name='section'>
		/// Section (references/notes/footnotes)
		/// </param>
		private static string MoveTemplateToSection(string articleText, Regex templateRegex, int section)
		{
			string extractedTemplate = templateRegex.Match(articleText).Value;
			articleText = templateRegex.Replace(articleText, "");

			switch (section)
			{
				case 1:
					return ReferencesSectionRegex.Replace(articleText, "$0" + extractedTemplate + "\r\n", 1);
				case 2:
					return NotesSectionRegex.Replace(articleText, "$0" + extractedTemplate + "\r\n", 1);
				case 3:
					return FootnotesSectionRegex.Replace(articleText, "$0" + extractedTemplate + "\r\n", 1);
				default:
					return articleText;
			}
		}

		
		private static readonly Regex ReferencesSection = new Regex(@"(^== *[Rr]eferences *==.*?)(?=^==[^=][^\r\n]*?[^=]==(\r\n?|\n)$)", RegexOptions.Multiline | RegexOptions.Singleline);
		private static readonly Regex ReferencesToEnd = new Regex(@"^== *[Rr]eferences *==\s*" + WikiRegexes.ReferencesTemplates + @"\s*(?={{DEFAULTSORT\:|\[\[Category\:)", RegexOptions.Multiline);

		// https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Place_.22External_links.22_section_after_.22References.22
		// TODO: only works when there is another section following the references section
		/// <summary>
		/// Ensures the external links section of an article is after the references section
		/// </summary>
		/// <param name="articleText">The wiki text of the article.</param>
		/// <returns>Article text with external links section below the references section</returns>
		public static string MoveExternalLinks(string articleText)
		{
			string articleTextAtStart = articleText;
			// is external links section above references?
			Match elm = ExternalLinksSection.Match(articleText);
			string externalLinks = elm.Groups[1].Value;

			// validate no <ref> in external links section
			if(!elm.Success || Regex.IsMatch(externalLinks, WikiRegexes.ReferenceEndGR))
			    return articleTextAtStart;
			
			string references = ReferencesSection.Match(articleText).Groups[1].Value;

			// references may be last section
			if (references.Length == 0)
			    references = ReferencesToEnd.Match(articleText).Value;

			if (references.Length > 0 && elm.Index < articleText.IndexOf(references))
			{
			    articleText = articleText.Replace(externalLinks, "");
			    articleText = articleText.Replace(references, references + externalLinks);
			}

			return articleText;
		}

		/// <summary>
		/// Moves the 'see also' section to be above the 'references' section, subject to the limitation that the 'see also' section can't be the last level-2 section.
		/// Does not move section when two or more references sections in the same article
		/// </summary>
		/// <param name="articleText">The wiki text of the article.</param>
		/// <returns></returns>
		public static string MoveSeeAlso(string articleText)
		{
			// is 'see also' section below references?
			Match refSm = ReferencesSection.Match(articleText), seeAm = SeeAlsoSection.Match(articleText);
			string references = refSm.Groups[1].Value, seealso = seeAm.Groups[1].Value;

			if (seeAm.Success && seeAm.Index > refSm.Index && ReferencesSection.Matches(articleText).Count == 1)
			{
				articleText = articleText.Replace(seealso, "");
                articleText = articleText.Replace(references, seealso + "\r\n" + references);
			}
			// newlines are fixed by later logic
			return articleText;
		}

		/// <summary>
		/// Gets a list of Link FA/GA's from the article
		/// </summary>
		/// <param name="articleText">The wiki text of the article.</param>
		/// <returns>The List of {{Link [FG]A}}'s from the article</returns>
		private static List<string> RemoveLinkFGAs(ref string articleText)
		{
			List<string> linkFGAList = new List<string>();

			MatchCollection matches = WikiRegexes.LinkFGAs.Matches(Tools.ReplaceWithSpaces(articleText, WikiRegexes.UnformattedText.Matches(articleText)));

			foreach (Match m in matches)
			{
				linkFGAList.Add(m.Value);
			}
			articleText = Tools.RemoveMatches(articleText, matches);
			return linkFGAList;
		}
        
        /// <summary>
		/// Extracts all of the interwiki featured article and interwiki links from the article text
		/// Ignores interwikis in comments/nowiki tags
		/// </summary>
		/// <param name="articleText">Article text with interwiki and interwiki featured article links removed</param>
		/// <returns>string of interwiki featured article and interwiki links</returns>
		public string Interwikis(ref string articleText)
        {
            return Interwikis(ref articleText, true);
        }

		/// <summary>
		/// Extracts all of the interwiki featured article and interwiki links from the article text
		/// Ignores interwikis in comments/nowiki tags
		/// </summary>
		/// <param name="articleText">Article text with interwiki and interwiki featured article links removed</param>
		/// <returns>string of interwiki featured article and interwiki links</returns>
		public string Interwikis(ref string articleText, bool linkFGAsInText)
		{
		    string interWikiComment = "";
            if(articleText.Contains("<!--"))
                articleText = InterLangRegex.Replace(articleText, m =>
                                                     {
                                                         interWikiComment = m.Value;
                                                         return "";
                                                     }, 1);

            string interWikis = "";

            // Only search for linkFGAs if necessary
            if(linkFGAsInText)
                interWikis = ListToString(RemoveLinkFGAs(ref articleText));

			if(interWikiComment.Length > 0)
				interWikis += interWikiComment + "\r\n";

			interWikis += ListToString(RemoveInterWikis(ref articleText));

			return interWikis;
		}

		/// <summary>
		/// Extracts all of the interwiki links from the article text, handles comments beside interwiki links (not inline comments)
		/// </summary>
		/// <param name="articleText">Article text with interwikis removed</param>
		/// <returns>List of interwikis</returns>
		private List<string> RemoveInterWikis(ref string articleText)
		{
			List<string> interWikiList = new List<string>();

            // Performance: faster to get all wikilinks and filter on interwiki matches than simply run the regex on the whole article text
            List<string> allWikiLinks = (from Match m in WikiRegexes.WikiLink.Matches(articleText) where m.Value.Contains(":") select m.Value + "]]").ToList();

            string allPossibleInterwikis = String.Join(" ", allWikiLinks.ToArray());

            if(!(from Match m in WikiRegexes.PossibleInterwikis.Matches(allPossibleInterwikis) where PossibleInterwikis.Contains(m.Groups[1].Value.Trim().ToLower()) select m.Value).Any())
                return interWikiList;

			// get all unformatted text in article to avoid taking interwikis from comments etc.
		    StringBuilder ut = new StringBuilder();
			foreach(Match u in WikiRegexes.UnformattedText.Matches(articleText))
				ut.Append(u.Value);

			string unformattedText = ut.ToString();

			List<Match> goodMatches = new List<Match>();
            List<string> interWikiListLinksOnly = new List<string>();

			foreach (Match m in WikiRegexes.PossibleInterwikis.Matches(articleText))
			{
				string site = m.Groups[1].Value.Trim().ToLower();
				
				if (!PossibleInterwikis.Contains(site))
					continue;
				
				if(unformattedText.Contains(m.Value))
				{
					Tools.ReplaceOnce(ref unformattedText, m.Value, "");
					continue;
				}
				
				goodMatches.Add(m);
				
				// jbo is only Wikipedia article wiki that's first letter case sensitive
				string IWTarget = site.Equals("jbo") ? m.Groups[2].Value.Trim() : Tools.TurnFirstToUpper(m.Groups[2].Value.Trim());
				string IW = "[[" + site + ":" + IWTarget + "]]";
				
				// drop interwikis to own wiki, but not on commons where language = en and en interwikis go to wikipedia
				if(!(m.Groups[1].Value.Equals(Variables.LangCode) && !Variables.IsWikimediaMonolingualProject) && !interWikiListLinksOnly.Contains(IW))
				{
				    interWikiListLinksOnly.Add(IW);
				    interWikiList.Add(IW + m.Groups[3].Value);
				}
			}

			articleText = Tools.RemoveMatches(articleText, goodMatches);

			if (SortInterwikis)
			{
				// sort twice to result in no reordering of two interwikis to same language project
				interWikiList.Sort(Comparer);
				interWikiList.Sort(Comparer);
			}

			return interWikiList;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="match"></param>
		/// <returns></returns>
		public static string IWMatchEval(Match match)
		{
			string[] textArray = { "[[", match.Groups["site"].ToString().ToLower(), ":", match.Groups["text"].ToString(), "]]" };
			return string.Concat(textArray);
		}

		/// <summary>
		/// Remove duplicates, and return List as string, one item per line
		/// </summary>
		/// <param name="items"></param>
		/// <returns></returns>
		private static string ListToString(ICollection<string> items)
		{
			if (items.Count == 0)
				return "";

			List<string> uniqueItems = new List<string>();

			// remove duplicates: duplicate if an existing list item starts the with string
			// also duplicate when one category is same as another with a sortkey
			// e.g. [[Category:One]] is duplicate of [[Category:One|A]]
			// Or sortkeys vary only by first letter case
			foreach (string s in items)
			{
			    bool addme = true;

			    string s2 =s;
			    // compare based on first letter upper sortkey for categories
			    if(s2.Contains("|") && WikiRegexes.Category.IsMatch(s2))
			        s2 = Regex.Replace(s2, @"(\|\s*)(.+)(\s*\]\]$)", m=> m.Groups[1].Value + Tools.TurnFirstToUpper(m.Groups[2].Value) + m.Groups[3].Value);

			    foreach(string u in uniqueItems)
			    {
			        if(u.StartsWith(s2) || u.StartsWith(s2.TrimEnd(']') + @"|") || u.Equals(s) || u.Equals(s2))
			        {
			            addme = false;
			            break;
			        }
			        if(s2.StartsWith(u)) // e.g. [[Category:A]] already added but [[Category:A]] <!-- comment--> next in list
			        {
			            uniqueItems.Remove(u);
			            break;
			        }

			        // compare on first letter case insensitive for templates
			        if (WikiRegexes.NestedTemplates.IsMatch(s2) && WikiRegexes.NestedTemplates.IsMatch(u))
			        {
			            string s2upper = s2.Substring(1,3).ToUpper() + s2.Substring(3);
			            string uupper = u.Substring(1,3).ToUpper() + u.Substring(3);
			            if(s2upper.Equals(uupper))
			            {
			                addme = false;
			                break;
			            }
			        }
			    }

			    if(addme)
			        uniqueItems.Add(s);
			}

			StringBuilder list = new StringBuilder();
			foreach (string s in uniqueItems)
			{
                list.Append(s + "\r\n"); // Don't just use AppendLine as this may just give \n under Mono
			}

			return list.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="list"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		private static List<string> CatKeyer(IEnumerable<string> list, string name)
		{
			name = Tools.MakeHumanCatKey(name, ""); // make key

			//add key to cats that need it
			List<string> newCats = new List<string>();
			foreach (string s in list)
			{
				string z = s;
				if (!z.Contains("|"))
					z = z.Replace("]]", "|" + name + "]]");

				newCats.Add(z);
			}
			return newCats;
		}
	}
}
