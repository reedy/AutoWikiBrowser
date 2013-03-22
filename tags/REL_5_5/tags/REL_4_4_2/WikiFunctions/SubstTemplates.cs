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

        private string[] locTemplateList = new string[0];

        public Dictionary<Regex, string> Regexes = new Dictionary<Regex, string>();

        Parse.HideText RemoveUnformatted = new Parse.HideText(true, false, true);

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

        public bool ExpandRecursively
        {
            get { return chkUseExpandTemplates.Checked; }
            set { chkUseExpandTemplates.Checked = value; }
        }

        public bool IgnoreUnformatted
        {
            get { return chkIgnoreUnformatted.Checked; }
            set { chkIgnoreUnformatted.Checked = value; }
        }

        public bool IncludeComment
        {
            get { return chkIncludeComment.Checked; }
            set { chkIncludeComment.Checked = value; }
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
                templ = templ.Insert(templ.Length - 1, "|[Mm]sg:|");
            else
                templ = @"(?:" + templ + "|[Mm]sg:|)";

            foreach (string s in TemplateList)
            {
                if (string.IsNullOrEmpty(s.Trim())) continue;
                Regexes.Add(new Regex(@"\{\{\s*" + templ + Tools.CaseInsensitive(s) + @"\s*(\|[^\}]*|)\}\}",
                    RegexOptions.Singleline | RegexOptions.Compiled), @"{{subst:" + s + "$1}}");
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

        public string SubstituteTemplates(string ArticleText, string ArticleTitle)
        {
            if (Regexes.Count == 0) return ArticleText; // nothing to substitute

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
                ArticleText = Tools.ExpandTemplate(ArticleText, ArticleTitle, Regexes, chkIncludeComment.Checked);

            if (chkIgnoreUnformatted.Checked)
                ArticleText = RemoveUnformatted.AddBackUnformatted(ArticleText);

            return ArticleText;
        }

        private void chkUseExpandTemplates_CheckedChanged(object sender, EventArgs e)
        {
            chkIncludeComment.Enabled = chkUseExpandTemplates.Checked;
        }
    }
}