using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

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

        public string SubstituteTemplates(string ArticleText)
        {
            foreach (KeyValuePair<Regex, string> p in Regexes)
            {
                ArticleText = p.Key.Replace(ArticleText, p.Value);
            }

            return ArticleText;
        }
    }
}