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
namespace WikiFunctions
{
    /// <summary>
    /// Provides functions for editting wiki text, such as formatting and re-categorisation.
    /// </summary>
    public class Parsers
    {
        #region constructor etc.

        MetaDataSorter metaDataSorter;
        public Parsers()
        {//default constructor
            metaDataSorter = new MetaDataSorter(this);
        }

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

        /// <summary>
        /// Used to append extra text to the edit summary.
        /// </summary>
        public string EditSummary
        {
            get { return editsummary; }
            set { editsummary = value; }
        }
        string editsummary = "";

        #endregion

        #region General Parse

        /// <summary>
        /// Re-organises the Person Data, stub/disambig templates, categories and interwikis
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">The article title.</param>
        /// <param name="sortWikis">True, sort interwiki order per pywiki bots, false keep current order.</param>
        /// <returns>The re-organised text.</returns>
        public string SortMetaData(string articleText, string articleTitle)
        {
            return metaDataSorter.Sort(articleText, articleTitle);
        }

        Hashtable hashNowiki = new Hashtable();
        readonly Regex NoWikiRegex = new Regex("<nowiki>.*?</nowiki>|<math>.*?</math>|<!--.*?-->|\\[\\[[Ii]mage:.*?\\]\\]", RegexOptions.Singleline | RegexOptions.Compiled);
        public string RemoveNowiki(string articleText)
        {
            hashNowiki.Clear();
            string s = "";

            int i = 0;
            foreach (Match m in NoWikiRegex.Matches(articleText))
            {
                if (Regex.IsMatch(m.Value, "<!-- ?(categories|\\{\\{.*?stub\\}\\}.*?|other languages|language links|inter ?(language|wiki)? ?links|inter ?wiki ?language ?links|inter ?wiki|The below are interlanguage links\\.?) ?-->", RegexOptions.IgnoreCase))
                    continue;

                s = "<%%<" + i.ToString() + ">%%>";
                articleText = articleText.Replace(m.Value, s);
                hashNowiki.Add(s, m.Value);
                i++;
            }

            return articleText;
        }

        public string AddNowiki(string articleText)
        {
            foreach (DictionaryEntry D in hashNowiki)
                articleText = articleText.Replace(D.Key.ToString(), D.Value.ToString());

            hashNowiki.Clear();
            return articleText;
        }

        readonly Regex regexHeadings0 = new Regex("(== ?)(see also:?|related topics:?|related articles:?|internal links:?|also see:?)( ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        readonly Regex regexHeadings1 = new Regex("(== ?)(external links:?|external sites:?|outside links|web ?links:?|exterior links:?)( ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        readonly Regex regexHeadings2 = new Regex("(== ?)(external link:?|external site:?|web ?link:?|exterior link:?)( ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        readonly Regex regexHeadings3 = new Regex("(== ?)(reference:?)(s? ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        readonly Regex regexHeadings4 = new Regex("(== ?)(source:?)(s? ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        readonly Regex regexHeadings5 = new Regex("(== ?)(further readings?:?)( ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Fix ==See also== and similar section common errors.
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The modified article text.</returns>
        public string FixHeadings(string articleText, ref bool NoChange)
        {
            string testText = articleText;
            articleText = FixHeadings(articleText);

            if (testText == articleText)
                NoChange = true;
            else
                NoChange = false;

            return articleText;
        }

        /// <summary>
        /// Fix ==See also== and similar section common errors.
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public string FixHeadings(string articleText)
        {
            if (!Regex.IsMatch(articleText, "= ?See also ?="))
                articleText = regexHeadings0.Replace(articleText, "$1See also$3");

            articleText = regexHeadings1.Replace(articleText, "$1External links$3");
            articleText = regexHeadings2.Replace(articleText, "$1External link$3");
            articleText = regexHeadings3.Replace(articleText, "$1Reference$3");
            articleText = regexHeadings4.Replace(articleText, "$1Source$3");
            articleText = regexHeadings5.Replace(articleText, "$1Further reading$3");

            return articleText;
        }

        /// <summary>
        /// Applies removes some excess whitespace from the article
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string RemoveWhiteSpace(string articleText)
        {
            //Replace lines that only contain one or two spaces with only a newline
            articleText = Regex.Replace(articleText, "^ ? ? \r\n", "\r\n", RegexOptions.Multiline);

            while (articleText.IndexOf("\r\n\r\n\r\n") > 0)
            {
                articleText = articleText.Replace("\r\n\r\n\r\n", "\r\n\r\n");
            }
            articleText = Regex.Replace(articleText, "== ? ?\r\n\r\n==", "==\r\n==");
            articleText = articleText.Replace("\r\n\r\n(* ?\\[?http)", "\r\n$1");

            articleText = Regex.Replace(articleText.Trim(), "----+$", "");
                     
            return articleText.Trim();
        }

        /// <summary>
        /// Applies removes all excess whitespace from the article
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public string RemoveAllWhiteSpace(string articleText)
        {//removes all whitespace
            articleText = articleText.Replace("\t", " ");
            articleText = RemoveWhiteSpace(articleText);

            articleText = articleText.Replace("\r\n\r\n*", "\r\n*");

            articleText = Regex.Replace(articleText, "  +", " ");
            articleText = Regex.Replace(articleText, " \r\n", "\r\n");

            articleText = Regex.Replace(articleText, "==\r\n\r\n", "==\r\n");

            //fix bullet points
            articleText = Regex.Replace(articleText, "^([\\*#]+) ", "$1", RegexOptions.Multiline);
            articleText = Regex.Replace(articleText, "^([\\*#]+)", "$1 ", RegexOptions.Multiline);

            //fix heading space
            articleText = Regex.Replace(articleText, "^(={1,4}) ?(.*?) ?(={1,4})$", "$1$2$3", RegexOptions.Multiline);

            //fix dash spacing
            articleText = Regex.Replace(articleText, " ?(–|&#150;|&ndash;|&#8211;|&#x2013;) ?", "$1");
            articleText = Regex.Replace(articleText, " ?(—|&#151;|&mdash;|&#8212;|&#x2014;) ?", "$1");
            articleText = Regex.Replace(articleText, "(—|&#151;|&mdash;|&#8212;|&#x2014;|–|&#150;|&ndash;|&#8211;|&#x2013;)", " $1 ");

            return articleText.Trim();
        }

        /// <summary>
        /// Fixes and improves syntax (such as html markup)
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The modified article text.</returns>
        public string SyntaxFixer(string articleText, ref bool NoChange)
        {
            string testText = articleText;
            articleText = SyntaxFixer(articleText);

            if (testText == articleText)
                NoChange = true;
            else
                NoChange = false;

            return articleText;
        }

        /// <summary>
        /// Fixes and improves syntax (such as html markup)
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public string SyntaxFixer(string articleText)
        {
            //replace html with wiki syntax
            if (!Regex.IsMatch(articleText, "'</?[ib]>|</?[ib]>'", RegexOptions.IgnoreCase))
            {
                articleText = Regex.Replace(articleText, "<i>(.*?)</i>", "''$1''", RegexOptions.IgnoreCase);
                articleText = Regex.Replace(articleText, "<b>(.*?)</b>", "'''$1'''", RegexOptions.IgnoreCase);
            }

            //remove unnecessary namespace
            articleText = Regex.Replace(articleText, "(\\{\\{[\\s]*)[Tt]emplate:(.*?\\}\\})", "$1$2", RegexOptions.Singleline);

            //remove <br> from lists, correct xhtml syntax
            articleText = Regex.Replace(articleText, "^((#|\\*).*?)<br ?/?>\r\n", "$1\r\n", RegexOptions.Multiline | RegexOptions.IgnoreCase);

            //can cause problems
            //articleText = Regex.Replace(articleText, "^<[Hh]2>(.*?)</[Hh]2>", "==$1==", RegexOptions.Multiline);
            //articleText = Regex.Replace(articleText, "^<[Hh]3>(.*?)</[Hh]3>", "===$1===", RegexOptions.Multiline);
            //articleText = Regex.Replace(articleText, "^<[Hh]4>(.*?)</[Hh]4>", "====$1====", RegexOptions.Multiline);

            //fix uneven bracketing on links
            if (!Regex.IsMatch(articleText, "\\[\\[[Ii]mage:[^]]*http"))
            {
                articleText = Regex.Replace(articleText, "\\[\\[http:\\/\\/([^][]*?)\\]", "[http://$1]", RegexOptions.IgnoreCase);
                articleText = Regex.Replace(articleText, "\\[http:\\/\\/([^][]*?)\\]\\]", "[http://$1]", RegexOptions.IgnoreCase);
                articleText = Regex.Replace(articleText, "\\[\\[http:\\/\\/(.*?)\\]\\]", "[http://$1]", RegexOptions.IgnoreCase);
                articleText = Regex.Replace(articleText, "\\[\\[([^][]*?)\\]([^][][^\\]])", "[[$1]]$2");
                articleText = Regex.Replace(articleText, "([^][])\\[([^][]*?)\\]\\]([^\\]])", "$1[[$2]]$3");
            }

            //repair bad external links
            articleText = Regex.Replace(articleText, "\\[?\\[image:(http:\\/\\/.*?)\\]\\]?", "[$1]", RegexOptions.IgnoreCase);

            //repair bad internal links
            articleText = Regex.Replace(articleText, "\\[\\[ (.*)?\\]\\]", "[[$1]]");
            articleText = Regex.Replace(articleText, "\\[\\[([A-Za-z]*) \\]\\]", "[[$1]]");
            articleText = Regex.Replace(articleText, "\\[\\[(.*)?_#(.*)\\]\\]", "[[$1#$2]]");

            return articleText.Trim();
        }

        /// <summary>
        /// Fixes link syntax
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The modified article text.</returns>
        public string LinkFixer(string articleText, ref bool NoChange)
        {
            string testText = articleText;
            articleText = LinkFixer(articleText);

            if (testText == articleText)
                NoChange = true;
            else
                NoChange = false;

            return articleText;
        }

        /// <summary>
        /// Fixes link syntax
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public string LinkFixer(string articleText)
        {
            string x = "";
            string y = "";

            foreach (Match m in Regex.Matches(articleText, "\\[\\[.*?\\]\\]"))
            {
                x = m.Value;
                y = "";

                if (!x.StartsWith("[[Image:") && !x.StartsWith("[[image:") && !x.StartsWith("[[_") && !x.Contains("|_"))
                    y = x.Replace("_", " ");
                else
                    y = x;

                y = noUnicoify(y);

                y = y.Replace("+", "%2B");
                y = HttpUtility.HtmlDecode(y);
                y = HttpUtility.UrlDecode(y);

                if (y.StartsWith("[[Category:"))
                    y = y.Replace("|]]", "| ]]");
                else
                    y = Regex.Replace(y, " ?\\| ?", "|");

                articleText = articleText.Replace(x, y);
            }

            return articleText;
        }

        /// <summary>
        /// Adds bullet points to external links after "external links" header
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The modified article text.</returns>
        public string BulletExternalLinks(string articleText, ref bool NoChange)
        {
            string testText = articleText;
            articleText = BulletExternalLinks(articleText);

            if (testText == articleText)
                NoChange = true;
            else
                NoChange = false;

            return articleText;
        }

        /// <summary>
        /// Adds bullet points to external links after "external links" header
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public string BulletExternalLinks(string articleText)
        {
            int intStart = 0;
            string articleTextSubstring = "";
            string strExtLink = Regex.Match(articleText, "= ? ?external links? ? ?=", RegexOptions.IgnoreCase).ToString();
            intStart = articleText.LastIndexOf(strExtLink);

            articleTextSubstring = articleText.Substring(intStart);
            articleText = articleText.Substring(0, intStart);
            articleTextSubstring = Regex.Replace(articleTextSubstring, "(\r\n)?(\r\n)(\\[?http)", "$2* $3");
            articleText += articleTextSubstring;

            return articleText;
        }

        public string FixCats(string articleText)
        {//Fix common spacing/capitalisation errors in categories
            articleText = Regex.Replace(articleText, "\\[\\[ ?" + caseInsensitive(Variables.Namespaces[14].Replace(":", " ?:")) + " ?", "[[" + Variables.Namespaces[14]);

            articleText = Regex.Replace(articleText, "\\]\\] ?\\[\\[Category:", "]]\r\n[[Category:");

            return articleText;
        }

        #endregion

        #region other functions

        /// <summary>
        /// Converts HTML entities to unicode, with some deliberate exceptions
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The modified article text.</returns>
        public string Unicodify(string articleText, ref bool NoChange)
        {
            string testText = articleText;
            articleText = Unicodify(articleText);

            if (testText == articleText)
                NoChange = true;
            else
                NoChange = false;

            return articleText;
        }

        /// <summary>
        /// Converts HTML entities to unicode, with some deliberate exceptions
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public string Unicodify(string articleText)
        {
            articleText = noUnicoify(articleText);
            articleText = Regex.Replace(articleText, "&(ndash|mdash);", "&amp;$1;");
            articleText = Regex.Replace(articleText, "(cm| m|km|mi)<sup>2</sup>", "$1²");

            articleText = HttpUtility.HtmlDecode(articleText);

            return articleText;
        }

        private static string noUnicoify(string articleText)
        {//entities that should never be unicoded, and convert some to more readable form
            articleText = Regex.Replace(articleText, "&#150;|&#8211;|&#x2013;", "&ndash;");
            articleText = Regex.Replace(articleText, "&#151;|&#8212;|&#x2014;", "&mdash;");

            articleText = articleText.Replace(" &amp; ", " & ");
            articleText = articleText.Replace("&amp;", "&amp;amp;");

            articleText = articleText.Replace("&minus;", "&amp;minus;").Replace("&times;", "&amp;times;");

            //IE6 does like these 
            articleText = articleText.Replace("&#705;", "&amp;#705;");
            articleText = articleText.Replace("&#803;", "&amp;#803;");
            articleText = articleText.Replace("&#596;", "&amp;#596;");
            articleText = articleText.Replace("&#620;", "&amp;#620;");
            articleText = articleText.Replace("&#699;", "&amp;#699;");
            articleText = articleText.Replace("&#700;", "&amp;#700;");
            articleText = articleText.Replace("&#8652;", "&amp;#8652;");
            articleText = articleText.Replace("&#9408;", "&amp;#9408;");
            articleText = articleText.Replace("&#9848;", "&amp;#9848;");

            articleText = articleText.Replace("&#x5B;", "&amp;#x5B;").Replace("&#x5D;", "&amp;#x5D;");
            articleText = articleText.Replace("&#126;", "&amp;#126;");
            articleText = articleText.Replace("&lt;", "&amp;lt;").Replace("&gt;", "&amp;gt;");
            articleText = articleText.Replace("&#160;", "&nbsp;");
            articleText = articleText.Replace("&nbsp;", "&amp;nbsp;").Replace("&thinsp;", "&amp;thinsp;").Replace("&shy;", "&amp;shy;");
            articleText = articleText.Replace("&prime;", "&amp;prime;").Replace("&Prime;", "&amp;Prime;");
            articleText = Regex.Replace(articleText, "&(#0?9[13];)", "&amp;$1");
            articleText = Regex.Replace(articleText, "&(#0?12[345];)", "&amp;$1");
            articleText = Regex.Replace(articleText, "&(#0?0?3[92];)", "&amp;$1");

            return articleText;
        }

        /// <summary>
        /// '''Emboldens''' the first occurence of the title, if it isnt already
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">The title of the article.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The modified article text.</returns>
        public string BoldTitle(string articleText, string articleTitle, ref bool NoChange)
        {
            string testText = articleText;
            articleText = BoldTitle(articleText, articleTitle);

            if (testText == articleText)
                NoChange = true;
            else
                NoChange = false;

            return articleText;
        }

        /// <summary>
        /// '''Emboldens''' the first occurence of the title, if it isnt already
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">The title of the article.</param>
        /// <returns>The modified article text.</returns>
        public string BoldTitle(string articleText, string articleTitle)
        {
            //ignore date articles
            if (Regex.IsMatch(articleTitle, "^(January|February|March|April|May|June|July|August|September|October|November|December) [0-9]{1,2}$"))
                return articleText;

            string escTitle = articleTitle;
            escTitle = Regex.Escape(escTitle);

            //remove self links first
            Regex tregex = new Regex("\\[\\[" + caseInsensitive(escTitle) + "\\]\\]");
            if (!articleText.Contains("'''"))
            {
                articleText = tregex.Replace(articleText, "'''$1$2'''", 1);
            }
            else
            {
                articleText = articleText.Replace("[[" + articleTitle + "]]", articleTitle);
                articleText = articleText.Replace("[[" + TurnFirstToLower(articleTitle) + "]]", TurnFirstToLower(articleTitle));
            }

            escTitle = Regex.Replace(articleTitle, " \\(.*?\\)$", "");
            escTitle = Regex.Escape(escTitle);

            if (Regex.IsMatch(articleText, "^(\\[\\[|\\{|\\*|:|<)") || Regex.IsMatch(articleText, "''' ?" + escTitle + " ?'''", RegexOptions.IgnoreCase))
                return articleText;

            Regex regexBold = new Regex("([^\\[]|^)(" + escTitle + ")([ ,.:;])", RegexOptions.IgnoreCase);

            string strSecondHalf = "";
            if (articleText.Length > 80)
            {
                strSecondHalf = articleText.Substring(80);
                articleText = articleText.Substring(0, 80);
            }

            if (articleText.Contains("'''"))
                return articleText + strSecondHalf;

            articleText = regexBold.Replace(articleText, "$1'''$2'''$3", 1);

            return articleText + strSecondHalf;
        }

        /// <summary>
        /// Replaces an iamge in the article.
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="OldImage">The old image to replace.</param>
        /// <param name="NewImage">The new image.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The new article text.</returns>
        public string ReImager(string OldImage, string NewImage, string articleText, ref bool NoChange)
        {
            string testText = articleText;
            articleText = ReImager(OldImage, NewImage, articleText);

            if (testText == articleText)
                NoChange = true;
            else
                NoChange = false;

            return articleText;
        }

        /// <summary>
        /// Replaces an iamge in the article.
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="OldImage">The old image to replace.</param>
        /// <param name="NewImage">The new image.</param>
        /// <returns>The new article text.</returns>
        public string ReImager(string OldImage, string NewImage, string articleText)
        {
            //remove image prefix
            OldImage = Regex.Replace(OldImage, "^" + Variables.Namespaces[6], "", RegexOptions.IgnoreCase).Replace("_", " ");
            NewImage = Regex.Replace(NewImage, "^" + Variables.Namespaces[6], "", RegexOptions.IgnoreCase).Replace("_", " ");

            OldImage = Regex.Escape(OldImage).Replace("\\ ", "[ _]");

            OldImage = "\\[\\[" + caseInsensitive(Variables.Namespaces[6]) + caseInsensitive(OldImage);
            NewImage = "[[" + Variables.Namespaces[6] + NewImage;

            articleText = Regex.Replace(articleText, OldImage, NewImage);

            return articleText;
        }

        /// <summary>
        /// Removes an iamge in the article.
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="OldImage">The image to remove.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The new article text.</returns>
        public string RemoveImage(string OldImage, string articleText, ref bool NoChange)
        {
            string testText = articleText;
            articleText = RemoveImage(OldImage, articleText);

            if (testText == articleText)
                NoChange = true;
            else
                NoChange = false;

            return articleText;
        }

        /// <summary>
        /// Removes an iamge in the article.
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="OldImage">The image to remove.</param>
        /// <returns>The new article text.</returns>
        public string RemoveImage(string OldImage, string articleText)
        {
            //remove image prefix
            OldImage = Regex.Replace(OldImage, "^" + Variables.Namespaces[6], "", RegexOptions.IgnoreCase).Replace("_", " ");

            OldImage = Regex.Escape(OldImage).Replace("\\ ", "[ _]");

            OldImage = "(\r\n)?\\[\\[" + caseInsensitive(Variables.Namespaces[6]) + caseInsensitive(OldImage) + ".*\\]\\]";

            articleText = Regex.Replace(articleText, OldImage, "");

            return articleText.Trim();
        }

        /// <summary>
        /// Re-categorises the article.
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="OldCategory">The old category to replace.</param>
        /// <param name="NewCategory">The new category.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The re-categorised article text.</returns>
        public string ReCategoriser(string OldCategory, string NewCategory, string articleText, ref bool NoChange)
        {
            string testText = articleText;
            articleText = ReCategoriser(OldCategory, NewCategory, articleText);

            if (testText == articleText)
                NoChange = true;
            else
                NoChange = false;

            return articleText;
        }

        /// <summary>
        /// Re-categorises the article.
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="OldCategory">The old category to replace.</param>
        /// <param name="NewCategory">The new category.</param>
        /// <returns>The re-categorised article text.</returns>
        public string ReCategoriser(string OldCategory, string NewCategory, string articleText)
        {
            //remove category prefix
            OldCategory = Regex.Replace(OldCategory, "^" + Variables.Namespaces[14], "", RegexOptions.IgnoreCase);
            NewCategory = Regex.Replace(NewCategory, "^" + Variables.Namespaces[14], "", RegexOptions.IgnoreCase);

            //format categories properly
            articleText = FixCats(articleText);

            OldCategory = Regex.Escape(OldCategory).Replace("\\ ", "[ _]");

            OldCategory = Variables.Namespaces[14] + OldCategory + "( ?\\|| ?\\]\\])";
            NewCategory = Variables.Namespaces[14] + NewCategory + "$1";

            articleText = Regex.Replace(articleText, OldCategory, NewCategory, RegexOptions.IgnoreCase);

            return articleText;
        }

        /// <summary>
        /// Removes a category from an article.
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="strOldCat">The old category to remove.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The article text without the old category.</returns>
        public string RemoveCategory(string strOldCat, string articleText, ref bool NoChange)
        {
            string testText = articleText;
            articleText = RemoveCategory(strOldCat, articleText);

            if (testText == articleText)
                NoChange = true;
            else
                NoChange = false;

            return articleText;
        }

        /// <summary>
        /// Removes a category from an article.
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="strOldCat">The old category to remove.</param>
        /// <returns>The article text without the old category.</returns>
        public string RemoveCategory(string strOldCat, string articleText)
        {
            //format categories properly
            articleText = FixCats(articleText);

            string strFirst = strOldCat.Substring(0, 1);
            string strFirstLower = strFirst.ToLower() + "]";

            strOldCat = strOldCat.Substring(1, strOldCat.Length - 1);
            strOldCat = Regex.Escape(strOldCat).Replace("\\ ", "[ _]");
            strOldCat = strFirst + strFirstLower + strOldCat;

            strOldCat = "\\[\\[" + caseInsensitive(Variables.Namespaces[14]) + " ?[" + strOldCat + "( ?\\]\\]| ?\\|[^\\|]*?\\]\\])(\r\n)?";
            articleText = Regex.Replace(articleText, strOldCat, "");

            return articleText;
        }

        /// <summary>
        /// Simplifies some links in article wiki text such as changing [[Dog|Dogs]] to [[Dog]]s
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The simplified article text.</returns>
        public string LinkSimplifier(string articleText, ref bool NoChange)
        {
            string testText = articleText;
            articleText = LinkSimplifier(articleText);

            if (testText == articleText)
                NoChange = true;
            else
                NoChange = false;

            return articleText;
        }

        /// <summary>
        /// Simplifies some links in article wiki text such as changing [[Dog|Dogs]] to [[Dog]]s
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The simplified article text.</returns>
        public string LinkSimplifier(string articleText)
        {
            string n = "";
            string a = "";
            string b = "";
            string k = "";

            foreach (Match m in Regex.Matches(articleText, "\\[\\[([^[]*?)\\|([^[]*?)\\]\\]"))
            {
                n = m.Value;
                a = m.Groups[1].Value;
                b = m.Groups[2].Value;

                if (a == b || TurnFirstToLower(a) == b)
                {
                    k = Regex.Replace(n, "\\[\\[(.*?)\\|(.*?)\\]\\]", "[[$2]]");
                    articleText = articleText.Replace(n, k);
                }
                else if (a + "s" == b || TurnFirstToLower(a) + "s" == b)
                {
                    k = Regex.Replace(n, "\\[\\[(.*?)\\|(.*?)\\]\\]", "$2");
                    k = "[[" + k.Substring(0, k.Length - 1) + "]]s";
                    articleText = articleText.Replace(n, k);
                }
            }
            return articleText;
        }
        
        public string LivingPeople(string articleText)
        {
            string strNot = "\\[\\[ ?Category ?:[ _]?([0-9]{1,2}[ _]century[ _]deaths|[0-9s]{4,5}[ _]deaths|Disappeared[ _]people|Living[ _]people|Year[ _]of[ _]death[ _]missing|Possibly[ _]living[ _]people)";

            if (!Regex.IsMatch(articleText, "\\[\\[ ?Category ?:[ _]?[0-9]{4}[ _]births", RegexOptions.IgnoreCase) || Regex.IsMatch(articleText, strNot, RegexOptions.IgnoreCase))
                return articleText;

            string birthCat = Regex.Match(articleText, "\\[\\[ ?Category ?:[ _]?[0-9]{4}[ _]births(\\|.*?)?\\]\\]", RegexOptions.IgnoreCase).ToString();
            int birthYear = int.Parse(Regex.Match(birthCat, "[0-9]{4}").ToString());
            string catKey = "";

            if (birthYear < 1910)
                return articleText;

            if (birthCat.Contains("|"))
                catKey = Regex.Match(birthCat, "\\|.*?\\]\\]").ToString();
            else
                catKey = "]]";

            articleText += "[[Category:Living people" + catKey;

            return articleText;
        }

        /// <summary>
        /// Converts/subst'd some deprecated templates
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The new article text.</returns>
        public string Conversions(string articleText, ref bool NoChange)
        {
            string testText = articleText;
            articleText = Conversions(articleText);

            if (testText == articleText)
                NoChange = true;
            else
                NoChange = false;

            return articleText;
        }

        /// <summary>
        /// Converts/subst'd some deprecated templates
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The new article text.</returns>
        public string Conversions(string articleText)
        {
            articleText = Regex.Replace(articleText, "\\{\\{(template:)?(wikify|wfy|wiki)\\}\\}", "{{Wikify-date|{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}", RegexOptions.IgnoreCase);
            articleText = Regex.Replace(articleText, "\\{\\{(template:)?(Clean ?up|Clean|Tidy)\\}\\}", "{{Cleanup-date|{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}", RegexOptions.IgnoreCase);
            articleText = Regex.Replace(articleText, "\\{\\{(template:)?Linkless\\}\\}", "{{Linkless-date|{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}", RegexOptions.IgnoreCase);

            articleText = Regex.Replace(articleText, "\\{\\{(Dab|Disamb|Disambiguation)\\}\\}", "{{Disambig}}", RegexOptions.IgnoreCase);
            articleText = Regex.Replace(articleText, "\\{\\{(2cc|2LAdisambig|2LCdisambig|2LC)\\}\\}", "{{2CC}}", RegexOptions.IgnoreCase);
            articleText = Regex.Replace(articleText, "\\{\\{(3cc|3LW|Tla-dab|TLA-disambig|TLAdisambig|3LC)\\}\\}", "{{3CC}}", RegexOptions.IgnoreCase);
            articleText = Regex.Replace(articleText, "\\{\\{(4cc|4LW|4LA|4LC)\\}\\}", "{{4CC}}", RegexOptions.IgnoreCase);

            articleText = Regex.Replace(articleText, "\\{\\{(Unsourced|Cite source|Unref|No references?|References|Not referenced|Needs? references|Sources|Cite-sources|Cleanup-sources?)\\}\\}", "{{Unreferenced}}", RegexOptions.IgnoreCase);

            articleText = Regex.Replace(articleText, "\\{\\{(Prettytable|Prettytable100|Pt)\\}\\}", "{{subst:Prettytable}}", RegexOptions.IgnoreCase);

            articleText = Regex.Replace(articleText, "\\{\\{(PAGENAME)\\}\\}", "{{subst:$1}}", RegexOptions.IgnoreCase);

            //articleText = Regex.Replace(articleText, "\\{\\{(Citation required|Citationneeded|Cite[- ]?needed)\\}\\}", "{{Citation needed}}", RegexOptions.IgnoreCase);

            return articleText;
        }

        Regex RegexBadHeader = new Regex("^(={1,4} ?(about|overview|definition|general information|background|intro|introduction|summary|bio|biography) ?={1,4})", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        /// <summary>
        /// Removes unnecessary introductory headers 
        /// </summary>
        public string RemoveBadHeaders(string articleText, string articleTitle)
        {
            articleText = Regex.Replace(articleText, "^={1,4} ?" + articleTitle + " ?={1,4}", "", RegexOptions.IgnoreCase);
            articleText = RegexBadHeader.Replace(articleText, "");

            return articleText.Trim();
        }

        /// <summary>
        /// Subst'd some user talk templates
        /// </summary>
        /// <param name="TalPageText">The wiki text of the talk page.</param>
        /// <returns>The new text.</returns>
        public string SubstUserTemplates(string TalkPageText)
        {
            TalkPageText = RemoveNowiki(TalkPageText);

            TalkPageText = Regex.Replace(TalkPageText, "\\{\\{(template:)?(test[n0-6]?[ab]?)\\}\\}", "{{subst:$2}}", RegexOptions.IgnoreCase);
            TalkPageText = Regex.Replace(TalkPageText, "\\{\\{(template:)?(test[n0-6]?[ab]?-n\\|.*?)\\}\\}", "{{subst:$2}}", RegexOptions.IgnoreCase);
            
            TalkPageText = Regex.Replace(TalkPageText, "\\{\\{(template:)?(3RR[0-5]?)\\}\\}", "{{subst:$2}}", RegexOptions.IgnoreCase);

            TalkPageText = Regex.Replace(TalkPageText, "\\{\\{(template:)?(spam[0-5][ab]?)\\}\\}", "{{subst:$2}}", RegexOptions.IgnoreCase);
            TalkPageText = Regex.Replace(TalkPageText, "\\{\\{(template:)?(spam[0-5]?-n\\|.*?)\\}\\}", "{{subst:$2}}", RegexOptions.IgnoreCase);

            TalkPageText = Regex.Replace(TalkPageText, "\\{\\{(template:)?(welcome[0-6]|welcomeip|anon|welcome-anon)\\}\\}", "{{subst:$2}}", RegexOptions.IgnoreCase);

            TalkPageText = AddNowiki(TalkPageText);
            return TalkPageText;
        }

        /// <summary>
        /// If necessary, adds wikify or stub tag
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitlet">The old category to remove.</param>
        /// <returns>The article text without the old category.</returns>
        public string Tagger(string articleText, string articleTitle)
        {
            if (articleText.Contains("}}"))
                return articleText;

            double intLength = articleText.Length + 1;
            double intLinkCount = 1;
            double intRatio = 0;

            foreach (Match m in Regex.Matches(articleText, "\\[\\["))
                intLinkCount++;

            intRatio = intLinkCount / intLength;

            if (Tools.IsMainSpace(articleTitle) && !Regex.IsMatch(articleText, "^#redirect", RegexOptions.IgnoreCase))
            {
                if (intLinkCount < 4 && (intRatio < 0.0025))
                {
                    articleText = "{{Wikify-date|{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}\r\n\r\n" + articleText;
                    EditSummary += " and added wikify tag";
                }

                if (intLength <= 380 && (!(Regex.IsMatch(articleText, "((S|s)tub)"))))
                {
                    articleText = articleText + "\r\n\r\n\r\n{{stub}}";
                    EditSummary += " and added stub tag";
                }
            }
            return articleText;
        }

        #endregion

        #region helper functions

        private string caseInsensitive(string txt)
        {//gets a string e.g. "Category" and returns "[Cc]ategory
            if (txt != "")
            {
                txt = txt.Trim();
                string temp = txt.Substring(0, 1);
                return "([" + temp.ToUpper() + temp.ToLower() + "])(" + txt.Remove(0, 1) + ")";
            }
            else
                return "";
        }

        private string TurnFirstToLower(string input)
        {
            //turns first character to lowercase
            if (input.Length == 0)
                return "";

            input = char.ToLower(input[0]) + input.Substring(1, input.Length - 1);

            return input;
        }

        #endregion

        #region unused

        /// <summary>
        /// Applies all the formatting functions
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">The title of the article.</param>
        /// <returns>The modified article text.</returns>
        public string Parse(string articleText, string articleTitle)
        {
            //remove stuff in <nowiki> and <math> tags
            articleText = RemoveNowiki(articleText);

            //General parsing, such as [[category => [[Category, interwiki order standardisation,
            articleText = Conversions(articleText);
            articleText = LivingPeople(articleText);
            articleText = FixHeadings(articleText);
            articleText = SyntaxFixer(articleText);
            articleText = FixCats(articleText);
            articleText = LinkFixer(articleText);
            articleText = BulletExternalLinks(articleText);
            articleText = SortMetaData(articleText, articleTitle);
            articleText = BoldTitle(articleText, articleTitle);
            articleText = LinkSimplifier(articleText);

            //add stuff in nowiki tags back
            articleText = AddNowiki(articleText);

            return articleText.Trim();
        }

        //[http://en.wikipedia.org/wiki/Dog] to [[Dog]]
        //private string ExtToInternalLinks(string articleText)
        //{
        //    foreach (Match m in Regex.Matches(articleText, "\\[http://en\\.wikipedia\\.org/wiki/.*?\\]"))
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

        //            articleText = articleText.Replace(m.ToString(), a);
        //        }
        //    }

        //    articleText = Regex.Replace(articleText, "\\[http://en\\.wikipedia\\.org/wiki/(.*?)\\]", "[[$1]]");
        //    return articleText;
        //}

        #endregion
    }
}
