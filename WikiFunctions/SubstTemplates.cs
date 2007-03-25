using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Web;
using System.Net;
using System.IO;

namespace WikiFunctions
{
    public partial class SubstTemplates : Form
    {
        public SubstTemplates()
        {
            InitializeComponent();
        }

        public string[] locTemplateList = new string[0];

        public Dictionary<Regex, string> Regexes = new Dictionary<Regex, string>();

        WikiFunctions.Parse.HideText RemoveUnformatted = new WikiFunctions.Parse.HideText(true, false, true);

        public string[] TemplateList
        {
            get
            {
                return locTemplateList;
            }
            set
            {
                textBoxTemplates.Lines = locTemplateList = value;
                textBoxTemplates.Select(0, 0);
                RefreshRegexes();
            }
        }

        public void Clear()
        {
            locTemplateList = new string[0];
            Regexes.Clear();
        }

        private void RefreshRegexes()
        {
            Regexes.Clear();
            string templ = Variables.NamespacesCaseInsensitive[10];
            if (templ[0] == '(')
            {
                templ = templ.Insert(templ.Length - 2, "|[Mm]sg:|");
            }
            else
            {
                templ = @"(?:" + templ + "|[Mm]sg:|)";
            }

            foreach (string s in TemplateList)
            {
                if (s.Trim() == "") continue;
                Regexes.Add(new Regex(@"\{\{\s*" + templ + Tools.CaseInsensitive(s) + @"\s*(\|[^\}]*|)\}\}", 
                    RegexOptions.Singleline | RegexOptions.Compiled), 
                    @"{{subst:"+s+"$1}}");
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            textBoxTemplates.Text = "";
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            TemplateList = textBoxTemplates.Lines;
            Close();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            textBoxTemplates.Lines = TemplateList;
        }

        private string ExpandTemplate(string ArticleText, string ArticleTitle)
        {
            foreach (KeyValuePair<Regex, string> p in Regexes)
            {
                MatchCollection uses = p.Key.Matches(ArticleText);
                foreach (Match m in uses)
                {
                    string Call = m.Value;

                    WebClient wc = new WebClient();
                    Uri expandUri = new Uri("http://en.wikipedia.org/wiki/Special:ExpandTemplates?contexttitle=" + ArticleTitle + "&input=" + Call + "&removecomments=1");
                    wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                    wc.Headers.Add("User-agent", "DotNetWikiBot/1.0");

                    string respStr = wc.DownloadString(expandUri);

                    int resultstart = respStr.IndexOf("readonly=\"readonly\">") + 20;
                    int resultend = respStr.IndexOf("</textarea", resultstart);
                    string result = respStr.Substring(resultstart, resultend - resultstart);
                    WikiFunctions.Parse.Parsers parsers = new WikiFunctions.Parse.Parsers();
                    bool SkipArticle;
                    result = parsers.Unicodify(result, out SkipArticle);

                    ArticleText = ArticleText.Replace(Call, result);

                }
            }

            return ArticleText;
        }

        public string SubstituteTemplates(string ArticleText, string ArticleTitle)
        {
            if (chkIgnoreUnformatted.Checked)
                ArticleText = RemoveUnformatted.HideUnformatted(ArticleText);
            if (!chkUseExpandTemplates.Checked)
            {
                foreach (KeyValuePair<Regex, string> p in Regexes)
                {
                    ArticleText = p.Key.Replace(ArticleText, p.Value);
                }
            }
            else
                ArticleText = ExpandTemplate(ArticleText, ArticleTitle);

            if (chkIgnoreUnformatted.Checked)
                ArticleText = RemoveUnformatted.AddBackUnformatted(ArticleText);
            
            return ArticleText;
        }
    }
}