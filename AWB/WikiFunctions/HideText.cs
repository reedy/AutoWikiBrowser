using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace WikiFunctions.Parse
{
    public class HideText
    {
        public HideText() { }

        public HideText(bool HideExternalLinks, bool LeaveMetaHeadings, bool HideImages)
        {
            this.HideExternal = HideExternalLinks;
            this.bHideImages = HideImages;
            this.LeaveMetaHeadings = LeaveMetaHeadings;
        }

        bool LeaveMetaHeadings = false;
        bool bHideImages = false;
        bool HideExternal = false;

        List<HideObject> NoEditList = new List<HideObject>();
        
        readonly Regex NoWikiIgnoreRegex = new Regex("<!-- ?(categories|\\{\\{.*?stub\\}\\}.*?|other languages|language links|inter ?(language|wiki)? ?links|inter ?wiki ?language ?links|inter ?wiki|The below are interlanguage links\\.?) ?-->", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public string Hide(string ArticleText)
        {
            NoEditList.Clear();
            string s = "";

            int i = 0;
            foreach (Match m in WikiRegexes.UnFormattedText.Matches(ArticleText))
            {
                if (LeaveMetaHeadings && NoWikiIgnoreRegex.IsMatch(m.Value))
                    continue;

                s = "⌊⌊⌊⌊" + i.ToString() + "⌋⌋⌋⌋";

                ArticleText = ArticleText.Replace(m.Value, s);
                NoEditList.Add(new HideObject(s, m.Value));
                i++;
            }

            if (bHideImages)
            {
                foreach (Match m in WikiRegexes.Images.Matches(ArticleText))
                {
                    s = "⌊⌊⌊⌊" + i.ToString() + "⌋⌋⌋⌋";

                    ArticleText = ArticleText.Replace(m.Value, s);
                    NoEditList.Add(new HideObject(s, m.Value));
                    i++;
                }
            }

            if (HideExternal)
            {
                foreach (Match m in WikiRegexes.ExternalLinks.Matches(ArticleText))
                {
                    s = "⌊⌊⌊⌊" + i.ToString() + "⌋⌋⌋⌋";

                    ArticleText = ArticleText.Replace(m.Value, s);
                    NoEditList.Add(new HideObject(s, m.Value));
                    i++;
                }
                foreach (Match m in WikiRegexes.InterWikiLinks.Matches(ArticleText))
                {
                    s = "⌊⌊⌊⌊" + i.ToString() + "⌋⌋⌋⌋";

                    ArticleText = ArticleText.Replace(m.Value, s);
                    NoEditList.Add(new HideObject(s, m.Value));
                    i++;
                }
            }

            return ArticleText;
        }

        public string AddBack(string ArticleText)
        {
            NoEditList.Reverse();

            foreach (HideObject k in NoEditList)
                ArticleText = ArticleText.Replace(k.code, k.text);

            NoEditList.Clear();
            return ArticleText;
        }


        List<HideObject> MoreHide = new List<HideObject>(32);

        /// <summary>
        /// Hides images, external links, templates, headings, images
        /// </summary>
        public string HideMore(string ArticleText)
        {
            MoreHide.Clear();
            string s = "";

            int i = 0;
            foreach (Match m in WikiRegexes.Template.Matches(ArticleText))
            {
                s = "⌊⌊⌊⌊M" + i.ToString() + "⌋⌋⌋⌋";

                ArticleText = ArticleText.Replace(m.Value, s);

                MoreHide.Add(new HideObject(s, m.Value));
                i++;
            }
            foreach (Match m in WikiRegexes.TemplateMultiLine.Matches(ArticleText))
            {
                s = "⌊⌊⌊⌊M" + i.ToString() + "⌋⌋⌋⌋";

                ArticleText = ArticleText.Replace(m.Value, s);

                MoreHide.Add(new HideObject(s, m.Value));
                i++;
            }
            foreach (Match m in WikiRegexes.Images.Matches(ArticleText))
            {
                s = "⌊⌊⌊⌊M" + i.ToString() + "⌋⌋⌋⌋";

                ArticleText = ArticleText.Replace(m.Value, s);
                MoreHide.Add(new HideObject(s, m.Value));
                i++;
            }
            foreach (Match m in WikiRegexes.ExternalLinks.Matches(ArticleText))
            {
                s = "⌊⌊⌊⌊M" + i.ToString() + "⌋⌋⌋⌋";

                ArticleText = ArticleText.Replace(m.Value, s);
                MoreHide.Add(new HideObject(s, m.Value));
                i++;
            }
            foreach (Match m in WikiRegexes.Heading.Matches(ArticleText))
            {
                s = "⌊⌊⌊⌊M" + i.ToString() + "⌋⌋⌋⌋";

                ArticleText = ArticleText.Replace(m.Value, s);
                MoreHide.Add(new HideObject(s, m.Value));
                i++;
            }
            foreach (Match m in WikiRegexes.IndentedText.Matches(ArticleText))
            {
                s = "⌊⌊⌊⌊M" + i.ToString() + "⌋⌋⌋⌋";

                ArticleText = ArticleText.Replace(m.Value, s);
                MoreHide.Add(new HideObject(s, m.Value));
                i++;
            }

            return ArticleText;
        }

        /// <summary>
        /// Adds back hidden stuff from HideMore
        /// </summary>
        public string AddBackMore(string ArticleText)
        {
            MoreHide.Reverse();

            foreach (HideObject k in MoreHide)
                ArticleText = ArticleText.Replace(k.code, k.text);

            MoreHide.Clear();
            return ArticleText;
        }
    }

    struct HideObject
    {
        public HideObject(string code, string text)
        {
            this.code = code;
            this.text = text;
        }

        public string code;
        public string text;
    }

}
