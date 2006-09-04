/*
WikiFunctions
Copyright (C) 2006 Martin Richards

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
using System.Text;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Collections;
using System.Web;

[assembly: CLSCompliant(true)]
namespace WikiFunctions.Parse
{
    /// <summary>
    /// Provides functions for editting wiki text, such as formatting and re-categorisation.
    /// </summary>
    public class Parsers
    {
        #region constructor etc.
        public Parsers()
        {//default constructor
            metaDataSorter = new MetaDataSorter(this);
            MakeRegexes();
        }

        /// <summary>
        /// Re-organises the Person Data, stub/disambig templates, categories and interwikis
        /// </summary>
        /// <param name="StubWordCount">The number of maximum number of words for a stub.</param>
        public Parsers(int StubWordCount, bool AddHumanKey)
        {
            metaDataSorter = new MetaDataSorter(this);
            StubMaxWordCount = StubWordCount;
            addCatKey = AddHumanKey;
            MakeRegexes();
        }

        private void MakeRegexes()
        {
            //look bad if changed
            RegexUnicode.Add(new Regex("&(ndash|mdash|minus|times|lt|gt|#160|nbsp|thinsp|shy|[Pp]rime);", RegexOptions.Compiled), "&amp;$1;");
            //IE6 does like these
            RegexUnicode.Add(new Regex("&#(705|803|596|620|699|700|8652|9408|9848);", RegexOptions.Compiled), "&amp;#$1;");
            //Phoenician alphabet
            RegexUnicode.Add(new Regex("&#(x109[0-9A-Z]{2});", RegexOptions.Compiled), "&amp;#$1;");
            //Blackboard bold and more
            RegexUnicode.Add(new Regex("&#((?:277|119|84|x1D|x100)[A-Z0-9a-z]{2,3});", RegexOptions.Compiled), "&amp;#$1;");
            //Cuneiform script
            RegexUnicode.Add(new Regex("&#(x12[A-Za-z0-9]{3});", RegexOptions.Compiled), "&amp;#$1;");
            //interfere with wiki syntax
            RegexUnicode.Add(new Regex("&#(126|x5D|x5B|x7c|0?9[13]|0?12[345]|0?0?3[92]);", RegexOptions.Compiled), "&amp;#$1;");
            //not entity, but still wrong
            RegexUnicode.Add(new Regex("(cm| m|mm|km|mi)<sup>2</sup>", RegexOptions.Compiled), "$1²");
            RegexUnicode.Add(new Regex("(cm| m|mm|km|mi)<sup>3</sup>", RegexOptions.Compiled), "$1³");

            RegexTagger.Add(new Regex("\\{\\{(template:)?(wikify|wikify-date|wfy|wiki)\\}\\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Wikify|{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");
            RegexTagger.Add(new Regex("\\{\\{(template:)?(Clean ?up|CU|Clean|Tidy)\\}\\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Cleanup|{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");
            RegexTagger.Add(new Regex("\\{\\{(template:)?Linkless\\}\\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Linkless|{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");
            RegexTagger.Add(new Regex("\\{\\{(template:)?(Uncategori[sz]ed|Uncat|Classify|Category needed|Catneeded|categori[zs]e|nocats?)\\}\\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Uncategorized|{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");

            RegexConversion.Add(new Regex("\\{\\{(Dab|Disamb|Disambiguation)\\}\\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Disambig}}");
            RegexConversion.Add(new Regex("\\{\\{(2cc|2LAdisambig|2LCdisambig|2LC)\\}\\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{2CC}}");
            RegexConversion.Add(new Regex("\\{\\{(3cc|3LW|Tla-dab|TLA-disambig|TLAdisambig|3LC)\\}\\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{3CC}}");
            RegexConversion.Add(new Regex("\\{\\{(4cc|4LW|4LA|4LC)\\}\\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{4CC}}");
            RegexConversion.Add(new Regex("\\{\\{(Prettytable|Prettytable100|Pt)\\}\\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{subst:Prettytable}}");
            RegexConversion.Add(new Regex("\\{\\{(?:[Tt]emplate:)?(PAGENAMEE?\\}\\}|[Ll]ived\\||[Bb]io-cats\\|)", RegexOptions.Compiled), "{{subst:$1");

            RegexConversion.Add(new Regex(@"\{\{[Ll]ife(?:time|span)\|([0-9]{4})\|([0-9]{4})\|(.*?)\}\}", RegexOptions.Compiled), "[[Category:$1 births|$3]]\r\n[[Category:$2 deaths|$3]]");
            RegexConversion.Add(new Regex(@"\{\{[Ll]ife(?:time|span)\|\|([0-9]{4})\|(.*?)\}\}", RegexOptions.Compiled), "[[Category:Year of birth unknown|$2]]\r\n[[Category:$1 deaths|$2]]");
            RegexConversion.Add(new Regex(@"\{\{[Ll]ife(?:time|span)\|([0-9]{4})\|\|(.*?)\}\}", RegexOptions.Compiled), "[[Category:$1 births|$2]]\r\n[[Category:Year of death unknown|$2]]");
        }

        Dictionary<Regex, string> RegexUnicode = new Dictionary<Regex, string>();
        Dictionary<Regex, string> RegexConversion = new Dictionary<Regex, string>();
        Dictionary<Regex, string> RegexTagger = new Dictionary<Regex, string>();

        MetaDataSorter metaDataSorter;
        string testText = "";
        int StubMaxWordCount = 500;

        /// <summary>
        /// Sort interwiki link order
        /// </summary>
        public bool sortInterwikiOrder
        {
            get { return boolInterwikiOrder; }
            set { boolInterwikiOrder = value; }
        }
        private bool boolInterwikiOrder = true;

        /// <summary>
        /// The interwiki link order to use
        /// </summary>
        public InterWikiOrderEnum InterWikiOrder
        {
            set { metaDataSorter.InterWikiOrder = value; }
            get { return metaDataSorter.InterWikiOrder; }
        }

        /// <summary>
        /// When set to true, adds key to categories (for people only) when parsed
        /// </summary>
        public bool addCatKey
        {
            get { return boolAddCatKey; }
            set { boolAddCatKey = value; }
        }
        private bool boolAddCatKey = false;

        #endregion

        #region General Parse

        /// <summary>
        /// Re-organises the Person Data, stub/disambig templates, categories and interwikis
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="ArticleTitle">The article title.</param>
        /// <param name="sortWikis">True, sort interwiki order per pywiki bots, false keep current order.</param>
        /// <returns>The re-organised text.</returns>
        public string SortMetaData(string ArticleText, string ArticleTitle)
        {
            return metaDataSorter.Sort(ArticleText, ArticleTitle);
        }

        readonly Regex regexHeadings0 = new Regex("(== ?)(see also:?|related topics:?|related articles:?|internal links:?|also see:?)( ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        readonly Regex regexHeadings1 = new Regex("(== ?)(external links:?|external sites:?|outside links|web ?links:?|exterior links:?)( ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        readonly Regex regexHeadings2 = new Regex("(== ?)(external link:?|external site:?|web ?link:?|exterior link:?)( ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        readonly Regex regexHeadings3 = new Regex("(== ?)(reference:?)(s? ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        readonly Regex regexHeadings4 = new Regex("(== ?)(source:?)(s? ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        readonly Regex regexHeadings5 = new Regex("(== ?)(further readings?:?)( ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        readonly Regex regexHeadings6 = new Regex("(== ?)(Early|Personal|Adult) Life( ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        readonly Regex regexHeadings7 = new Regex("(== ?)Early Career( ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Fix ==See also== and similar section common errors.
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The modified article text.</returns>
        public string FixHeadings(string ArticleText, ref bool NoChange)
        {
            testText = ArticleText;
            ArticleText = FixHeadings(ArticleText);

            if (testText == ArticleText)
                NoChange = true;
            else
                NoChange = false;

            return ArticleText;
        }

        /// <summary>
        /// Fix ==See also== and similar section common errors.
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public string FixHeadings(string ArticleText)
        {
            if (!Regex.IsMatch(ArticleText, "= ?See also ?="))
                ArticleText = regexHeadings0.Replace(ArticleText, "$1See also$3");

            ArticleText = regexHeadings1.Replace(ArticleText, "$1External links$3");
            ArticleText = regexHeadings2.Replace(ArticleText, "$1External link$3");
            ArticleText = regexHeadings3.Replace(ArticleText, "$1Reference$3");
            ArticleText = regexHeadings4.Replace(ArticleText, "$1Source$3");
            ArticleText = regexHeadings5.Replace(ArticleText, "$1Further reading$3");
            ArticleText = regexHeadings6.Replace(ArticleText, "$1$2 life$3");
            ArticleText = regexHeadings7.Replace(ArticleText, "$1Early career$2");

            return ArticleText;
        }

        /// <summary>
        /// Applies removes some excess whitespace from the article
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string RemoveWhiteSpace(string ArticleText)
        {
            ArticleText = Regex.Replace(ArticleText, "\r\n(\r\n)+", "\r\n\r\n");

            ArticleText = Regex.Replace(ArticleText, "== ? ?\r\n\r\n==", "==\r\n==");
            ArticleText = ArticleText.Replace("\r\n\r\n(* ?\\[?http)", "\r\n$1");

            ArticleText = Regex.Replace(ArticleText.Trim(), "----+$", "");
            ArticleText = Regex.Replace(ArticleText.Trim(), "<br ?/?>$", "", RegexOptions.IgnoreCase);

            return ArticleText.Trim();
        }

        /// <summary>
        /// Applies removes all excess whitespace from the article
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public string RemoveAllWhiteSpace(string ArticleText)
        {//removes all whitespace
            ArticleText = ArticleText.Replace("\t", " ");
            ArticleText = RemoveWhiteSpace(ArticleText);

            ArticleText = ArticleText.Replace("\r\n\r\n*", "\r\n*");

            ArticleText = Regex.Replace(ArticleText, "  +", " ");
            ArticleText = Regex.Replace(ArticleText, " \r\n", "\r\n");

            ArticleText = Regex.Replace(ArticleText, "==\r\n\r\n", "==\r\n");

            //fix bullet points
            ArticleText = Regex.Replace(ArticleText, "^([\\*#]+) ", "$1", RegexOptions.Multiline);
            ArticleText = Regex.Replace(ArticleText, "^([\\*#]+)", "$1 ", RegexOptions.Multiline);

            //fix heading space
            ArticleText = Regex.Replace(ArticleText, "^(={1,4}) ?(.*?) ?(={1,4})$", "$1$2$3", RegexOptions.Multiline);

            //fix dash spacing
            ArticleText = Regex.Replace(ArticleText, " ?(–|—|&#15[01];|&[nm]dash;|&#821[12];|&#x201[34];) ?", "$1");
            ArticleText = Regex.Replace(ArticleText, "(—|&#151;|&mdash;|&#8212;|&#x2014;|–|&#150;|&ndash;|&#8211;|&#x2013;)", " $1 ");

            return ArticleText.Trim();
        }

        /// <summary>
        /// Fixes and improves syntax (such as html markup)
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The modified article text.</returns>
        public string FixSyntax(string ArticleText, ref bool NoChange)
        {
            testText = ArticleText;
            ArticleText = FixSyntax(ArticleText);

            if (testText == ArticleText)
                NoChange = true;
            else
                NoChange = false;

            return ArticleText;
        }

        readonly Regex SyntaxRegex1 = new Regex("\\[\\[http:\\/\\/([^][]*?)\\]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        readonly Regex SyntaxRegex2 = new Regex("\\[http:\\/\\/([^][]*?)\\]\\]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        readonly Regex SyntaxRegex3 = new Regex("\\[\\[http:\\/\\/(.*?)\\]\\]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        readonly Regex SyntaxRegex4 = new Regex("\\[\\[([^][]*?)\\]([^][][^\\]])", RegexOptions.Compiled);
        readonly Regex SyntaxRegex5 = new Regex("([^][])\\[([^][]*?)\\]\\]([^\\]])", RegexOptions.Compiled);

        readonly Regex SyntaxRegex6 = new Regex("\\[?\\[image:(http:\\/\\/.*?)\\]\\]?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        readonly Regex SyntaxRegex7 = new Regex("\\[\\[ (.*)?\\]\\]", RegexOptions.Compiled);
        readonly Regex SyntaxRegex8 = new Regex("\\[\\[([A-Za-z]*) \\]\\]", RegexOptions.Compiled);
        readonly Regex SyntaxRegex9 = new Regex("\\[\\[(.*)?_#(.*)\\]\\]", RegexOptions.Compiled);

        readonly Regex SyntaxRegex10 = new Regex("(\\{\\{[\\s]*)[Tt]emplate:(.*?\\}\\})", RegexOptions.Singleline | RegexOptions.Compiled);
        readonly Regex SyntaxRegex11 = new Regex("^((#|\\*).*?)<br ?/?>\r\n", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        readonly Regex SyntaxRegex12 = new Regex("<i>(.*?)</i>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        readonly Regex SyntaxRegex13 = new Regex("<b>(.*?)</b>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Fixes and improves syntax (such as html markup)
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public string FixSyntax(string ArticleText)
        {
            //replace html with wiki syntax
            if (!Regex.IsMatch(ArticleText, "'</?[ib]>|</?[ib]>'", RegexOptions.IgnoreCase))
            {
                ArticleText = SyntaxRegex12.Replace(ArticleText, "''$1''");
                ArticleText = SyntaxRegex13.Replace(ArticleText, "'''$1'''");
            }
            ArticleText = Regex.Replace(ArticleText, "^<hr>|^----+", "----", RegexOptions.Multiline);

            //remove appearance of double line break
            ArticleText = Regex.Replace(ArticleText, "(^==?[^=]*==?)\r\n(\r\n)?----+", "$1", RegexOptions.Multiline);

            //remove unnecessary namespace
            ArticleText = SyntaxRegex10.Replace(ArticleText, "$1$2");

            //remove <br> from lists
            ArticleText = SyntaxRegex11.Replace(ArticleText, "$1\r\n");

            //can cause problems
            //ArticleText = Regex.Replace(ArticleText, "^<[Hh]2>(.*?)</[Hh]2>", "==$1==", RegexOptions.Multiline);
            //ArticleText = Regex.Replace(ArticleText, "^<[Hh]3>(.*?)</[Hh]3>", "===$1===", RegexOptions.Multiline);
            //ArticleText = Regex.Replace(ArticleText, "^<[Hh]4>(.*?)</[Hh]4>", "====$1====", RegexOptions.Multiline);

            //fix uneven bracketing on links
            if (!Regex.IsMatch(ArticleText, "\\[\\[[Ii]mage:[^]]*http"))
            {
                ArticleText = SyntaxRegex1.Replace(ArticleText, "[http://$1]");
                ArticleText = SyntaxRegex2.Replace(ArticleText, "[http://$1]");
                ArticleText = SyntaxRegex3.Replace(ArticleText, "[http://$1]");
                ArticleText = SyntaxRegex4.Replace(ArticleText, "[[$1]]$2");
                ArticleText = SyntaxRegex5.Replace(ArticleText, "$1[[$2]]$3");
            }

            //repair bad external links
            ArticleText = SyntaxRegex6.Replace(ArticleText, "[$1]");

            //repair bad internal links
            ArticleText = SyntaxRegex7.Replace(ArticleText, "[[$1]]");
            ArticleText = SyntaxRegex8.Replace(ArticleText, "[[$1]]");
            ArticleText = SyntaxRegex9.Replace(ArticleText, "[[$1#$2]]");

            ArticleText = Regex.Replace(ArticleText, "ISBN: ?([0-9])", "ISBN $1");

            return ArticleText.Trim();
        }

        /// <summary>
        /// Fixes link syntax
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The modified article text.</returns>
        public string FixLinks(string ArticleText, ref bool NoChange)
        {
            testText = ArticleText;
            ArticleText = FixLinks(ArticleText);

            if (testText == ArticleText)
                NoChange = true;
            else
                NoChange = false;

            return ArticleText;
        }

        readonly Regex RegexLink = new Regex("\\[\\[.*?\\]\\]", RegexOptions.Compiled);

        /// <summary>
        /// Fixes link syntax
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public string FixLinks(string ArticleText)
        {
            string x = "";
            string y = "";

            string cat = "[[" + Variables.Namespaces[14];

            foreach (Match m in RegexLink.Matches(ArticleText))
            {
                x = m.Value;
                y = "";

                if (!x.StartsWith("[[Image:") && !x.StartsWith("[[image:") && !x.StartsWith("[[_") && !x.Contains("|_"))
                    y = x.Replace("_", " ");
                else
                    y = x;

                y = y.Replace("+", "%2B");
                y = HttpUtility.UrlDecode(y);

                if (y.StartsWith(cat))
                    y = y.Replace("|]]", "| ]]");
                else
                    y = Regex.Replace(y, " ?\\| ?", "|");

                ArticleText = ArticleText.Replace(x, y);
            }

            return ArticleText;
        }

        /// <summary>
        /// Adds bullet points to external links after "external links" header
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The modified article text.</returns>
        public string BulletExternalLinks(string ArticleText, ref bool NoChange)
        {
            testText = ArticleText;
            ArticleText = BulletExternalLinks(ArticleText);

            if (testText == ArticleText)
                NoChange = true;
            else
                NoChange = false;

            return ArticleText;
        }

        /// <summary>
        /// Adds bullet points to external links after "external links" header
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public string BulletExternalLinks(string ArticleText)
        {
            int intStart = 0;
            string ArticleTextSubstring = "";

            Match m = Regex.Match(ArticleText, "= ? ?external links? ? ?=", RegexOptions.IgnoreCase | RegexOptions.RightToLeft);

            if (!m.Success)
                return ArticleText;

            intStart = m.Index;

            ArticleTextSubstring = ArticleText.Substring(intStart);
            ArticleText = ArticleText.Substring(0, intStart);
            ArticleTextSubstring = Regex.Replace(ArticleTextSubstring, "(\r\n)?(\r\n)(\\[?http)", "$2* $3");
            ArticleText += ArticleTextSubstring;

            return ArticleText;
        }

        public string FixCategories(string ArticleText)
        {//Fix common spacing/capitalisation errors in categories

            Regex catregex = new Regex("\\[\\[ ?" + Variables.NamespacesCaseInsensitive[14].Replace(":", " ?:") + " ?(.*?)\\]\\]");
            string cat = "[[" + Variables.Namespaces[14];
            string x = "";

            foreach (Match m in catregex.Matches(ArticleText))
            {
                x = cat + m.Groups[1].Value.Replace("_", " ") + "]]";
                ArticleText = ArticleText.Replace(m.Value, x);
            }

            return ArticleText;
        }

        #endregion

        #region other functions

        /// <summary>
        /// Converts HTML entities to unicode, with some deliberate exceptions
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The modified article text.</returns>
        public string Unicodify(string ArticleText, ref bool NoChange)
        {
            testText = ArticleText;
            ArticleText = Unicodify(ArticleText);

            if (testText == ArticleText)
                NoChange = true;
            else
                NoChange = false;

            return ArticleText;
        }

        /// <summary>
        /// Converts HTML entities to unicode, with some deliberate exceptions
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public string Unicodify(string ArticleText)
        {            
            ArticleText = Regex.Replace(ArticleText, "&#150;|&#8211;|&#x2013;", "&ndash;");
            ArticleText = Regex.Replace(ArticleText, "&#151;|&#8212;|&#x2014;", "&mdash;");
            ArticleText = ArticleText.Replace(" &amp; ", " & ");
            ArticleText = ArticleText.Replace("&amp;", "&amp;amp;");

            foreach (KeyValuePair<Regex, string> k in RegexUnicode)
            {
                ArticleText = k.Key.Replace(ArticleText, k.Value);
            }
            try
            {
                ArticleText = HttpUtility.HtmlDecode(ArticleText);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }

            return ArticleText;
        }

        /// <summary>
        /// '''Emboldens''' the first occurence of the title, if it isnt already
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="ArticleTitle">The title of the article.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The modified article text.</returns>
        public string BoldTitle(string ArticleText, string ArticleTitle, ref bool NoChange)
        {
            testText = ArticleText;
            ArticleText = BoldTitle(ArticleText, ArticleTitle);

            if (testText == ArticleText)
                NoChange = true;
            else
                NoChange = false;

            return ArticleText;
        }

        /// <summary>
        /// '''Emboldens''' the first occurence of the title, if it isnt already
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="ArticleTitle">The title of the article.</param>
        /// <returns>The modified article text.</returns>
        public string BoldTitle(string ArticleText, string ArticleTitle)
        {
            //ignore date articles
            if (Regex.IsMatch(ArticleTitle, "^(January|February|March|April|May|June|July|August|September|October|November|December) [0-9]{1,2}$"))
                return ArticleText;

            string escTitle = Regex.Escape(ArticleTitle);

            //remove self links first
            Regex tregex = new Regex("\\[\\[(" + Tools.caseInsensitive(escTitle) + ")\\]\\]");
            if (!ArticleText.Contains("'''"))
            {
                ArticleText = tregex.Replace(ArticleText, "'''$1'''", 1);
            }
            else
            {
                ArticleText = ArticleText.Replace("[[" + ArticleTitle + "]]", ArticleTitle);
                ArticleText = ArticleText.Replace("[[" + Tools.TurnFirstToLower(ArticleTitle) + "]]", Tools.TurnFirstToLower(ArticleTitle));
            }

            escTitle = Regex.Replace(ArticleTitle, " \\(.*?\\)$", "");
            escTitle = Regex.Escape(escTitle);

            if (Regex.IsMatch(ArticleText, "^(\\[\\[|\\{|\\*|:)") || Regex.IsMatch(ArticleText, "''' ?" + escTitle + " ?'''", RegexOptions.IgnoreCase))
                return ArticleText;

            Regex regexBold = new Regex("([^\\[]|^)(" + escTitle + ")([ ,.:;])", RegexOptions.IgnoreCase);

            string strSecondHalf = "";
            if (ArticleText.Length > 80)
            {
                strSecondHalf = ArticleText.Substring(80);
                ArticleText = ArticleText.Substring(0, 80);
            }

            if (ArticleText.Contains("'''"))
                return ArticleText + strSecondHalf;

            ArticleText = regexBold.Replace(ArticleText, "$1'''$2'''$3", 1);

            return ArticleText + strSecondHalf;
        }

        /// <summary>
        /// Replaces an iamge in the article.
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="OldImage">The old image to replace.</param>
        /// <param name="NewImage">The new image.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The new article text.</returns>
        public string ReImager(string OldImage, string NewImage, string ArticleText, ref bool NoChange)
        {
            testText = ArticleText;
            ArticleText = ReplaceImage(OldImage, NewImage, ArticleText);

            if (testText == ArticleText)
                NoChange = true;
            else
                NoChange = false;

            return ArticleText;
        }

        /// <summary>
        /// Replaces an iamge in the article.
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="OldImage">The old image to replace.</param>
        /// <param name="NewImage">The new image.</param>
        /// <returns>The new article text.</returns>
        public string ReplaceImage(string OldImage, string NewImage, string ArticleText)
        {
            //remove image prefix
            OldImage = Regex.Replace(OldImage, "^" + Variables.Namespaces[6], "", RegexOptions.IgnoreCase).Replace("_", " ");
            NewImage = Regex.Replace(NewImage, "^" + Variables.Namespaces[6], "", RegexOptions.IgnoreCase).Replace("_", " ");

            OldImage = Regex.Escape(OldImage).Replace("\\ ", "[ _]");

            OldImage = Variables.NamespacesCaseInsensitive[6] + Tools.caseInsensitive(OldImage);
            NewImage = Variables.Namespaces[6] + NewImage;

            ArticleText = Regex.Replace(ArticleText, OldImage, NewImage);

            return ArticleText;
        }

        /// <summary>
        /// Removes an iamge in the article.
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="Image">The image to remove.</param>
        /// <returns>The new article text.</returns>
        public string RemoveImage(string Image, string ArticleText, bool CommentOut, string Comment)
        {
            //remove image prefix
            Image = Regex.Replace(Image, "^" + Variables.Namespaces[6], "", RegexOptions.IgnoreCase).Replace("_", " ");
            Image = Regex.Escape(Image).Replace("\\ ", "[ _]");
            Image = Tools.caseInsensitive(Image);

            Regex r = new Regex("\\[\\[" + Variables.NamespacesCaseInsensitive[6] + Image + ".*\\]\\]");
            MatchCollection n = r.Matches(ArticleText);

            if (n.Count > 0)
            {
                foreach (Match m in n)
                {
                    string match = m.Value;

                    int i = 0;
                    int j = 0;

                    foreach (char c in match)
                    {
                        if (c == '[')
                            j++;
                        else if (c == ']')
                            j--;

                        i++;

                        if (j == 0)
                        {
                            if (match.Length > i)
                                match = match.Remove(i);

                            Regex t = new Regex(Regex.Escape(match));

                            if (CommentOut)
                                ArticleText = t.Replace(ArticleText, "<!-- " + Comment + " " + match + " -->", 1, m.Index);
                            else
                                ArticleText = t.Replace(ArticleText, "", 1);

                            break;
                        }

                    }
                }
            }
            else
            {
                r = new Regex("(" + Variables.NamespacesCaseInsensitive[6] + ")?" + Image);
                n = r.Matches(ArticleText);

                foreach (Match m in n)
                {
                    Regex t = new Regex(Regex.Escape(m.Value));

                    if (CommentOut)
                        ArticleText = t.Replace(ArticleText, "<!-- " + Comment + " $0 -->", 1, m.Index);
                    else
                        ArticleText = t.Replace(ArticleText, "", 1, m.Index);
                }
            }

            return ArticleText;
        }

        /// <summary>
        /// Removes an iamge in the article.
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="OldImage">The image to remove.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The new article text.</returns>
        public string RemoveImage(string Image, string ArticleText, bool CommentOut, string Comment, ref bool NoChange)
        {
            testText = ArticleText;
            ArticleText = RemoveImage(Image, ArticleText, CommentOut, Comment);

            if (testText == ArticleText)
                NoChange = true;
            else
                NoChange = false;

            return ArticleText;
        }

        /// <summary>
        /// Adds the category to the article.
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="NewCategory">The new category.</param>
        /// <returns>The article text.</returns>
        public string AddCategory(string NewCategory, string ArticleText, string ArticleTitle, ref bool NoChange)
        {
            testText = ArticleText;
            ArticleText = AddCategory(NewCategory, ArticleText, ArticleTitle);

            if (testText == ArticleText)
                NoChange = true;
            else
                NoChange = false;

            return ArticleText;
        }

        /// <summary>
        /// Adds the category to the article.
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="NewCategory">The new category.</param>
        /// <returns>The article text.</returns>
        public string AddCategory(string NewCategory, string ArticleText, string ArticleTitle)
        {
            if (Regex.IsMatch(ArticleText, "\\[\\[ ?[Cc]ategory ?: ?" + Regex.Escape(NewCategory)))
                return ArticleText;

            string cat = "\r\n[[" + Variables.Namespaces[14] + NewCategory + "]]";
            cat = Tools.ApplyKeyWords(ArticleTitle, cat);

            if (ArticleTitle.StartsWith(Variables.Namespaces[10]))
                ArticleText += "<noinclude>" + cat + "\r\n</noinclude>";
            else
                ArticleText += cat;

            return ArticleText;
        }

        /// <summary>
        /// Re-categorises the article.
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="OldCategory">The old category to replace.</param>
        /// <param name="NewCategory">The new category.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The re-categorised article text.</returns>
        public string ReCategoriser(string OldCategory, string NewCategory, string ArticleText, ref bool NoChange)
        {
            //remove category prefix
            OldCategory = Regex.Replace(OldCategory, "^" + Variables.Namespaces[14], "", RegexOptions.IgnoreCase);
            NewCategory = Regex.Replace(NewCategory, "^" + Variables.Namespaces[14], "", RegexOptions.IgnoreCase);

            //format categories properly
            ArticleText = FixCategories(ArticleText);

            testText = ArticleText;

            if (Regex.IsMatch(ArticleText, "\\[\\[" + Variables.NamespacesCaseInsensitive[14] + Tools.caseInsensitive(Regex.Escape(NewCategory)) + "( ?\\|| ?\\]\\])"))
            {
                ArticleText = RemoveCategory(OldCategory, ArticleText);
            }
            else
            {
                OldCategory = Regex.Escape(OldCategory);
                OldCategory = Tools.caseInsensitive(OldCategory);                

                OldCategory = Variables.Namespaces[14] + OldCategory + "( ?\\|| ?\\]\\])";
                NewCategory = Variables.Namespaces[14] + NewCategory + "$1";

                ArticleText = Regex.Replace(ArticleText, OldCategory, NewCategory);
            }

            if (testText == ArticleText)
                NoChange = true;
            else
                NoChange = false;

            return ArticleText;
        }

        /// <summary>
        /// Removes a category from an article.
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="strOldCat">The old category to remove.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The article text without the old category.</returns>
        public string RemoveCategory(string strOldCat, string ArticleText, ref bool NoChange)
        {
            testText = ArticleText;
            ArticleText = RemoveCategory(strOldCat, ArticleText);

            if (testText == ArticleText)
                NoChange = true;
            else
                NoChange = false;

            return ArticleText;
        }

        /// <summary>
        /// Removes a category from an article.
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="strOldCat">The old category to remove.</param>
        /// <returns>The article text without the old category.</returns>
        public string RemoveCategory(string strOldCat, string ArticleText)
        {
            //format categories properly
            ArticleText = FixCategories(ArticleText);

            strOldCat = Regex.Escape(strOldCat);
            strOldCat = Tools.caseInsensitive(strOldCat);

            strOldCat = "\\[\\[" + Variables.NamespacesCaseInsensitive[14] + " ?" + strOldCat + "( ?\\]\\]| ?\\|[^\\|]*?\\]\\])(\r\n)?";
            ArticleText = Regex.Replace(ArticleText, strOldCat, "");

            return ArticleText;
        }

        /// <summary>
        /// Simplifies some links in article wiki text such as changing [[Dog|Dogs]] to [[Dog]]s
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The simplified article text.</returns>
        public string LinkSimplifier(string ArticleText, ref bool NoChange)
        {
            testText = ArticleText;
            ArticleText = LinkSimplifier(ArticleText);

            if (testText == ArticleText)
                NoChange = true;
            else
                NoChange = false;

            return ArticleText;
        }

        readonly Regex LinkSimplierRegex = new Regex("\\[\\[([^[]*?)\\|([^[]*?)\\]\\]", RegexOptions.Compiled);

        /// <summary>
        /// Simplifies some links in article wiki text such as changing [[Dog|Dogs]] to [[Dog]]s
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The simplified article text.</returns>
        public string LinkSimplifier(string ArticleText)
        {
            string n = "";
            string a = "";
            string b = "";
            string k = "";

            foreach (Match m in LinkSimplierRegex.Matches(ArticleText))
            {
                n = m.Value;
                a = m.Groups[1].Value;
                b = m.Groups[2].Value;

                if (a == b || Tools.TurnFirstToLower(a) == b)
                {
                    k = LinkSimplierRegex.Replace(n, "[[$2]]");
                    ArticleText = ArticleText.Replace(n, k);
                }
                else if (a + "s" == b || Tools.TurnFirstToLower(a) + "s" == b)
                {
                    k = LinkSimplierRegex.Replace(n, "$2");
                    k = "[[" + k.Substring(0, k.Length - 1) + "]]s";
                    ArticleText = ArticleText.Replace(n, k);
                }
            }

            return ArticleText;
        }

        public string LivingPeople(string ArticleText, ref bool NoChange)
        {
            NoChange = true;
            testText = ArticleText;

            if (Regex.IsMatch(ArticleText, "\\[\\[ ?Category ?:[ _]?([0-9]{1,2}[ _]century[ _]deaths|[0-9s]{4,5}[ _]deaths|Disappeared[ _]people|Living[ _]people|Year[ _]of[ _]death[ _]missing|Possibly[ _]living[ _]people)", RegexOptions.IgnoreCase))
                return ArticleText;

            Match m = Regex.Match(ArticleText, "\\[\\[ ?Category ?:[ _]?([0-9]{4})[ _]births(\\|.*?)?\\]\\]", RegexOptions.IgnoreCase);

            if (!m.Success)
                return ArticleText;

            string birthCat = m.Value;
            int birthYear = int.Parse(m.Groups[1].Value);
            string catKey = "";

            if (birthYear < 1910)
                return ArticleText;

            if (birthCat.Contains("|"))
                catKey = Regex.Match(birthCat, "\\|.*?\\]\\]").Value;
            else
                catKey = "]]";

            ArticleText += "[[Category:Living people" + catKey;

            if (testText == ArticleText)
                NoChange = true;
            else
                NoChange = false;

            return ArticleText;
        }

        /// <summary>
        /// Converts/subst'd some deprecated templates
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The new article text.</returns>
        public string Conversions(string ArticleText, ref bool NoChange)
        {
            testText = ArticleText;
            ArticleText = Conversions(ArticleText);

            if (testText == ArticleText)
                NoChange = true;
            else
                NoChange = false;

            return ArticleText;
        }

        /// <summary>
        /// Converts/subst'd some deprecated templates
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The new article text.</returns>
        public string Conversions(string ArticleText)
        {
            //Use proper codes
            ArticleText = ArticleText.Replace("[[zh-tw:", "[[zh:");
            ArticleText = ArticleText.Replace("[[nb:", "[[no:");
            ArticleText = ArticleText.Replace("[[dk:", "[[da:");

            ArticleText = ArticleText.Replace("{{msg:", "{{");

            foreach (KeyValuePair<Regex, string> k in RegexConversion)
            {
                ArticleText = k.Key.Replace(ArticleText, k.Value);
            }

            return ArticleText;
        }

        /// <summary>
        /// Removes unnecessary introductory headers 
        /// </summary>
        public string RemoveBadHeaders(string ArticleText, string ArticleTitle, ref bool NoChange)
        {
            testText = ArticleText;
            ArticleText = RemoveBadHeaders(ArticleText, ArticleTitle);

            if (testText == ArticleText)
                NoChange = true;
            else
                NoChange = false;

            return ArticleText;
        }

        Regex RegexBadHeader = new Regex("^(={1,4} ?(about|description|overview|definition|general information|background|intro|introduction|summary|bio|biography) ?={1,4})", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        /// <summary>
        /// Removes unnecessary introductory headers 
        /// </summary>
        public string RemoveBadHeaders(string ArticleText, string ArticleTitle)
        {
            ArticleText = Regex.Replace(ArticleText, "^={1,4} ?" + Regex.Escape(ArticleTitle) + " ?={1,4}", "", RegexOptions.IgnoreCase);
            ArticleText = RegexBadHeader.Replace(ArticleText, "");

            return ArticleText.Trim();
        }

        /// <summary>
        /// Subst'd some user talk templates
        /// </summary>
        /// <param name="TalPageText">The wiki text of the talk page.</param>
        /// <returns>The new text.</returns>
        public string SubstUserTemplates(string TalkPageText)
        {
            TalkPageText = Regex.Replace(TalkPageText, "\\{\\{(template:)?(test[n0-6]?[ab]?)\\}\\}", "{{subst:$2}}", RegexOptions.IgnoreCase);
            TalkPageText = Regex.Replace(TalkPageText, "\\{\\{(template:)?(test[n0-6]?[ab]?-n\\|.*?)\\}\\}", "{{subst:$2}}", RegexOptions.IgnoreCase);

            TalkPageText = Regex.Replace(TalkPageText, "\\{\\{(template:)?(3RR[0-5]?)\\}\\}", "{{subst:$2}}", RegexOptions.IgnoreCase);

            TalkPageText = Regex.Replace(TalkPageText, "\\{\\{(template:)?(spam[0-5][ab]?)\\}\\}", "{{subst:$2}}", RegexOptions.IgnoreCase);
            TalkPageText = Regex.Replace(TalkPageText, "\\{\\{(template:)?(spam[0-5]?-n\\|.*?)\\}\\}", "{{subst:$2}}", RegexOptions.IgnoreCase);

            TalkPageText = Regex.Replace(TalkPageText, "\\{\\{(template:)?(welcome[0-6]|welcomeip|anon|welcome-anon)\\}\\}", "{{subst:$2}}", RegexOptions.IgnoreCase);

            return TalkPageText;
        }

        readonly Regex RegexStub = new Regex("\\{\\{.*?[Ss]tub\\}\\}", RegexOptions.Compiled);

        /// <summary>
        /// If necessary, adds/removes wikify or stub tag
        /// </summary>
        public string Tagger(string ArticleText, string ArticleTitle, ref bool NoChange, ref string Summary)
        {
            testText = ArticleText;
            ArticleText = Tagger(ArticleText, ArticleTitle, ref Summary);

            if (testText == ArticleText)
                NoChange = true;
            else
                NoChange = false;

            return ArticleText;
        }

        /// <summary>
        /// adds/removes
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="ArticleTitlet">The old category to remove.</param>
        /// <returns>The article text without the old category.</returns>
        public string Tagger(string ArticleText, string ArticleTitle, ref string Summary)
        {
            if (Tools.IsRedirect(ArticleText))
                return ArticleText;

            double Length = ArticleText.Length + 1;
            int words = Tools.WordCount(ArticleText);
            double LinkCount = 1;
            double Ratio = 0;

            //update by-date tags
            foreach (KeyValuePair<Regex, string> k in RegexTagger)
            {
                ArticleText = k.Key.Replace(ArticleText, k.Value);
            }

            //remove stub tags from long articles
            if (words > StubMaxWordCount && RegexStub.IsMatch(ArticleText))
            {
                MatchEvaluator stubEvaluator = new MatchEvaluator(stubChecker);
                ArticleText = RegexStub.Replace(ArticleText, stubEvaluator);

                return ArticleText.Trim();
            }            

            if (Regex.IsMatch(ArticleText, @"\{\{.*?\}\}"))
            {
                return ArticleText;
            }

            LinkCount = Tools.LinkCount(ArticleText);
            Ratio = LinkCount / Length;

            if (words > 8 && !Regex.IsMatch(ArticleText, @"\[\[ ?category", RegexOptions.IgnoreCase))
            {
                ArticleText += "\r\n\r\n{{Uncategorized|{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}";
                Summary += ", added uncategorised tag";
            }
            else if (LinkCount < 3 && (Ratio < 0.0025))
            {
                ArticleText = "{{Wikify|{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}\r\n\r\n" + ArticleText;
                Summary += ", added wikify tag";
            }
            else if (ArticleText.Length <= 300 && !RegexStub.IsMatch(ArticleText))
            {
                ArticleText = ArticleText + "\r\n\r\n\r\n{{stub}}";
                Summary += ", added stub tag";
            }            

            return ArticleText;
        }

        private string stubChecker(Match m)
        {// Replace each Regex cc match with the number of the occurrence.
            if (Regex.IsMatch(m.Value, "\\{\\{[Ss]ect"))
                return m.Value;
            else
                return "";
        }

        #endregion

        #region unused

        /// <summary>
        /// Fixes minor problems, such as abbreviations and miscapitalisations
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The new article text.</returns>
        public string MinorThings(string ArticleText)
        {
            ArticleText = Regex.Replace(ArticleText, "[Aa]\\.[Kk]\\.[Aa]\\.?", "also known as");

            ArticleText = ArticleText.Replace("e.g.", "for example");
            ArticleText = ArticleText.Replace("i.e.", "that is");

            MatchCollection ma = Regex.Matches(ArticleText, "(monday|tuesday|wednesday|thursday|friday|saturday|sunday|january|february|april|june|july|august|september|october|november|december)");
            if (ma.Count > 0)
            {
                foreach (Match m in ma)
                    ArticleText = ArticleText.Replace(m.Groups[1].Value, Tools.TurnFirstToUpper(m.Groups[1].Value));
            }

            return ArticleText;
        }

        //[http://en.wikipedia.org/wiki/Dog] to [[Dog]]
        //private string ExtToInternalLinks(string ArticleText)
        //{
        //    foreach (Match m in Regex.Matches(ArticleText, "\\[http://en\\.wikipedia\\.org/wiki/.*?\\]"))
        //    {
        //        string a = HttpUtility.UrlDecode(m.ToString());

        //        if (a.Contains(" "))
        //        {
        //            int intP;
        //            //string a = n;
        //            intP = a.IndexOf(" ");

        //            string b = a.Substring(intP);
        //            a = a.Remove(intP);
        //            b = b.TrimStart();
        //            a = a.Replace("_", " ");

        //            ArticleText = ArticleText.Replace(m.ToString(), a);
        //        }
        //    }

        //    ArticleText = Regex.Replace(ArticleText, "\\[http://en\\.wikipedia\\.org/wiki/(.*?)\\]", "[[$1]]");
        //    return ArticleText;
        //}

        #endregion
    }
}
