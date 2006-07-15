//$Header: /cvsroot/autowikibrowser/WikiFunctions/FindandReplace.cs,v 1.8 2006/06/26 16:30:25 wikibluemoose Exp $
/*
Autowikibrowser
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
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace WikiFunctions
{
    /// <summary>
    /// Provides a form and functions for setting and applying multiple find and replacements on a text string.
    /// </summary>
    public partial class FindandReplace : Form
    {
        public FindandReplace()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Applies a series of defined find and replacements to the supplied article text.
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="ArticleTitle">The title of the article.</param>
        /// <returns>The modified article text.</returns>
        public string MultipleFindAndReplce(string ArticleText, string strTitle)
        {
            RegexOptions ROptions = makeOptions();

            int i = 0;
            string f = "";
            string r = "";

            if (chkIgnoreLinks.Checked)
                ArticleText = RemoveLinks(ArticleText);

            while (i < dataGridView1.Rows.Count)
            {
                if (!(dataGridView1.Rows[i].Cells[0].Value == null) && !(dataGridView1.Rows[i].Cells[0].Value.ToString().Length == 0))
                {
                    if (dataGridView1.Rows[i].Cells[1].Value == null)
                        dataGridView1.Rows[i].Cells[1].Value = "";

                    f = dataGridView1.Rows[i].Cells[0].Value.ToString();
                    r = dataGridView1.Rows[i].Cells[1].Value.ToString();

                    ArticleText = PerformFindAndReplace(chkAreRegexes.Checked, f, r, ArticleText, strTitle, ROptions);
                }
                i++;
            }

            if (chkIgnoreLinks.Checked)
                ArticleText = AddLinks(ArticleText);

            return ArticleText;
        }

        private string PerformFindAndReplace(bool Regexe, string strOld, string strNew, string FaRText, string strTitle, RegexOptions ROptions)
        {
            strOld = strOld.Replace("%%title%%", strTitle);
            strNew = strNew.Replace("%%title%%", strTitle);

            if (!Regexe)
                strOld = Regex.Escape(strOld);

            //stop \r\n being interpreted literally
            strNew = strNew.Replace(@"\r", "\r").Replace(@"\n", "\n");
            strOld = strOld.Replace(@"\r", "\r").Replace(@"\n", "\n");

            FaRText = Regex.Replace(FaRText, strOld, strNew, ROptions);

            return FaRText;
        }

        private RegexOptions makeOptions()
        {
            RegexOptions ROptions = RegexOptions.None;

            if (!chkCaseSensitive.Checked)
                ROptions = RegexOptions.IgnoreCase;
            if (chkMultiline.Checked)
                ROptions = ROptions | RegexOptions.Multiline;
            if (chkSingleline.Checked)
                ROptions = ROptions | RegexOptions.Singleline;

            return ROptions;
        }

        Hashtable hashLinks = new Hashtable();
        readonly Regex NoLinksRegex = new Regex("<nowiki>.*?</nowiki>|<math>.*?</math>|<!--.*?-->|[Hh]ttp://[^\\ ]*|\\[[Hh]ttp:.*?\\]|\\[\\[[Ii]mage:.*?\\]\\]", RegexOptions.Singleline | RegexOptions.Compiled);
        private string RemoveLinks(string articleText)
        {
            hashLinks.Clear();

            int i = 0;
            foreach (Match m in NoLinksRegex.Matches(articleText))
            {
                MessageBox.Show(m.Value);
                articleText = articleText.Replace(m.Value, "<" + i.ToString() + ">");
                hashLinks.Add("<" + i.ToString() + ">", m.Value);
                i++;
            }

            return articleText;
        }

        private string AddLinks(string articleText)
        {
            foreach (DictionaryEntry D in hashLinks)
                articleText = articleText.Replace(D.Key.ToString(), D.Value.ToString());

            hashLinks.Clear();
            return articleText;
        }

        private void FindandReplace_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void btnDone_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabel2.LinkVisited = true;
            System.Diagnostics.Process.Start("http://en.wikipedia.org/wiki/Regular_expression");
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you really want to clear the whole table?", "Really clear?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                dataGridView1.Rows.Clear();
        }

        /// <summary>
        /// Clears the set replacements.
        /// </summary>
        public void Clear()
        {
            dataGridView1.Rows.Clear();
        }

        /// <summary>
        /// Returns a dictionary key/vaue list of the set find and replacemets.
        /// </summary>
        public Dictionary<string, string> GetReplacements()
        {
            Dictionary<string, string> r = new Dictionary<string, string>();

            int i = 0;
            while (i < dataGridView1.Rows.Count)
            {
                if (!(dataGridView1.Rows[i].Cells[0].Value == null) && !(dataGridView1.Rows[i].Cells[0].Value.ToString().Length == 0))
                {
                    if (dataGridView1.Rows[i].Cells[1].Value == null)
                        dataGridView1.Rows[i].Cells[1].Value = "";

                    r.Add(dataGridView1.Rows[i].Cells[0].Value.ToString(), dataGridView1.Rows[i].Cells[1].Value.ToString());

                }
                i++;
            }

            return r;
        }

        private void chkAreRegexes_CheckedChanged(object sender, EventArgs e)
        {
            bool regex = chkAreRegexes.Checked;
            chkMultiline.Enabled = regex;
            chkSingleline.Enabled = regex;

            if (!regex)
            {
                chkMultiline.Checked = false;
                chkSingleline.Checked = false;
            }
        }

        #region loading/saving

        /// <summary>
        /// Adds a find and replacement task.
        /// </summary>
        /// <param name="Find">The string to find.</param>
        /// <param name="ReplaceWith">The replacement string.</param>
        public void AddNew(string Find, string ReplaceWith)
        {
            dataGridView1.Rows.Add(Find, ReplaceWith);
        }

        /// <summary>
        /// Writes the find and replace settings to XML.
        /// </summary>
        /// <param name="XMLWriter">The XML writer to write to.</param>
        /// <param name="enabled">Set whether find and replace is enabled.</param>
        public void WriteToXml(XmlTextWriter XMLWriter, bool enabled, bool ignore)
        {
            XMLWriter.WriteStartElement("findandreplace");

            XMLWriter.WriteAttributeString("enabled", enabled.ToString());
            XMLWriter.WriteAttributeString("regex", isRegex.ToString());
            XMLWriter.WriteAttributeString("casesensitive", caseSensitive.ToString());
            XMLWriter.WriteAttributeString("multiline", isMulti.ToString());
            XMLWriter.WriteAttributeString("singleline", isSingle.ToString());
            XMLWriter.WriteAttributeString("ignorenofar", ignore.ToString());
            XMLWriter.WriteAttributeString("ignoretext", ignoreLinks.ToString());

            for (int i = 0; i != dataGridView1.Rows.Count; ++i)
            {
                if (dataGridView1.Rows[i].Cells[0].Value == null)
                    continue;

                if (dataGridView1.Rows[i].Cells[0].Value.ToString() == "")
                    continue;

                if (dataGridView1.Rows[i].Cells[1].Value == null)
                    dataGridView1.Rows[i].Cells[1].Value = "";

                XMLWriter.WriteStartElement("datagridFAR");

                XMLWriter.WriteAttributeString("find", dataGridView1.Rows[i].Cells[0].Value.ToString());
                XMLWriter.WriteAttributeString("replacewith", dataGridView1.Rows[i].Cells[1].Value.ToString());

                XMLWriter.WriteEndElement();
            }

            XMLWriter.WriteEndElement();

        }

        /// <summary>
        /// Gets or sets whether the replacements are regexes.
        /// </summary>
        public bool isRegex
        {
            get { return chkAreRegexes.Checked; }
            set { chkAreRegexes.Checked = value; }
        }

        /// <summary>
        /// Gets or sets whether the replacements are case sensitive.
        /// </summary>
        public bool caseSensitive
        {
            get { return chkCaseSensitive.Checked; }
            set { chkCaseSensitive.Checked = value; }
        }

        /// <summary>
        /// Gets or sets whether the replacements are multiline.
        /// </summary>
        public bool isMulti
        {
            get { return chkMultiline.Checked; }
            set { chkMultiline.Checked = value; }
        }

        /// <summary>
        /// Gets or sets whether the replacements are singleline.
        /// </summary>
        public bool isSingle
        {
            get { return chkSingleline.Checked; }
            set { chkSingleline.Checked = value; }
        }

        /// <summary>
        /// Gets or sets whether the replacements ignore external links and images
        /// </summary>
        public bool ignoreLinks
        {
            get { return chkIgnoreLinks.Checked; }
            set { chkIgnoreLinks.Checked = value; }
        }

        #endregion

    }

}
