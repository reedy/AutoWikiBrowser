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
using System.Drawing;

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

        HideText RemoveLinks = new HideText(true, false);

        List<Replacements> ReplacementList = new List<Replacements>();
        bool applydefault = false;
        private bool ApplyDefaultFormatting
        {
            get { return applydefault; }
            set
            {
                applydefault = value;
                dataGridView1.AllowUserToAddRows = value;
            }
        }

        public void MakeList()
        {
            ReplacementList.Clear();

            string f = "";
            string r = "";

            foreach (DataGridViewRow dataGridRow in dataGridView1.Rows)
            {
                if (dataGridRow.IsNewRow)
                    continue;

                if (!(bool)dataGridRow.Cells["enabled"].FormattedValue || dataGridRow.Cells["find"].Value == null)
                    continue;

                if (dataGridRow.Cells["replace"].Value == null)
                    dataGridRow.Cells["replace"].Value = "";

                Replacements rep = new Replacements();

                f = dataGridRow.Cells["find"].Value.ToString();
                r = dataGridRow.Cells["replace"].Value.ToString();

                f = f.Replace(@"\r", "\r").Replace(@"\n", "\n");
                r = r.Replace(@"\r", "\r").Replace(@"\n", "\n");

                if (!(bool)dataGridRow.Cells["regex"].FormattedValue)
                    f = Regex.Escape(f);

                rep.Find = f;
                rep.Replace = r;

                rep.RegularExpressinonOptions = RegexOptions.None;
                if (!(bool)dataGridRow.Cells["casesensitive"].FormattedValue)
                    rep.RegularExpressinonOptions = rep.RegularExpressinonOptions | RegexOptions.IgnoreCase;
                if ((bool)dataGridRow.Cells["multi"].FormattedValue)
                    rep.RegularExpressinonOptions = rep.RegularExpressinonOptions | RegexOptions.Multiline;
                if ((bool)dataGridRow.Cells["single"].FormattedValue)
                    rep.RegularExpressinonOptions = rep.RegularExpressinonOptions | RegexOptions.Singleline;

                ReplacementList.Add(rep);
            }
        }

        /// <summary>
        /// Applies a series of defined find and replacements to the supplied article text.
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="ArticleTitle">The title of the article.</param>
        /// <returns>The modified article text.</returns>
        public string MultipleFindAndReplce(string ArticleText, string strTitle)
        {
            EditSummary = "";

            if (chkIgnoreLinks.Checked)
                ArticleText = RemoveLinks.Hide(ArticleText);

            foreach (Replacements rep in ReplacementList)
            {
                ArticleText = PerformFindAndReplace(rep.Find, rep.Replace, ArticleText, strTitle, rep.RegularExpressinonOptions);
            }

            if (chkIgnoreLinks.Checked)
                ArticleText = RemoveLinks.AddBack(ArticleText);

            if (EditSummary != "")
                EditSummary = ", Replaced: " + EditSummary.Trim();

            return ArticleText;
        }

        Regex findRegex;
        MatchCollection Matches;
        string summary = "";

        private string PerformFindAndReplace(string Find, string Replace, string ArticleText, string ArticleTitle, RegexOptions ROptions)
        {
            Find = Tools.ApplyKeyWords(ArticleTitle, Find);
            Replace = Tools.ApplyKeyWords(ArticleTitle, Replace);

            findRegex = new Regex(Find, ROptions);
            Matches = findRegex.Matches(ArticleText);

            if (Matches.Count > 0)
            {                
                ArticleText = findRegex.Replace(ArticleText, Replace);

                if (Matches[0].Value != Matches[0].Result(Replace))
                {
                    summary = Matches[0].Value + " → " + Matches[0].Result(Replace);

                    if (Matches.Count > 1)
                        summary += " (" + Matches.Count.ToString() + ")";

                    EditSummary += summary + ", ";
                }
            }

            return ArticleText;
        }             

        private void btnDone_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabel2.LinkVisited = true;
            System.Diagnostics.Process.Start("http://en.wikipedia.org/wiki/Regular_expression");
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you really want to clear the whole table?", "Really clear?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                Clear();
        }

        /// <summary>
        /// Clears the set replacements.
        /// </summary>
        public void Clear()
        {
            ReplacementList.Clear();
            dataGridView1.Rows.Clear();
        }

        /// <summary>
        /// Returns a list of the set find and replacemets.
        /// </summary>
        public List<Replacements> GetReplacements()
        {
            return ReplacementList;
        }

        #region loading/saving

        /// <summary>
        /// Adds a find and replacement task.
        /// </summary>
        /// <param name="Find">The string to find.</param>
        /// <param name="ReplaceWith">The replacement string.</param>
        public void AddNew(string Find, string ReplaceWith, bool CaseSensitive, bool Regex, bool MultiLine, bool SingleLine, int Times, bool enabled)
        {
            dataGridView1.Rows.Add(Find, ReplaceWith, CaseSensitive, Regex, MultiLine, SingleLine, enabled);
            if (!enabled)
                dataGridView1.Rows[dataGridView1.Rows.Count - 1].DefaultCellStyle.BackColor = Color.LightGray;
        }

        /// <summary>
        /// Writes the find and replace settings to XML.
        /// </summary>
        /// <param name="XMLWriter">The XML writer to write to.</param>
        /// <param name="enabled">Set whether find and replace is enabled.</param>
        public void WriteToXml(XmlTextWriter XMLWriter, bool enabled, bool ignore)
        {
            XMLWriter.WriteStartElement("findandreplacesettings");

            XMLWriter.WriteAttributeString("enabled", enabled.ToString());
            XMLWriter.WriteAttributeString("ignorenofar", ignore.ToString());
            XMLWriter.WriteAttributeString("ignoretext", ignoreLinks.ToString());
            XMLWriter.WriteAttributeString("appendsummary", AppendToSummary.ToString());

            foreach (DataGridViewRow dataGridRow in dataGridView1.Rows)
            {
                if (dataGridRow.IsNewRow || dataGridRow.Cells["find"].Value == null)
                    continue;

                if (dataGridRow.Cells["replace"].Value == null)
                    dataGridRow.Cells["replace"].Value = "";

                XMLWriter.WriteStartElement("FindAndReplace");

                XMLWriter.WriteAttributeString("find", dataGridRow.Cells["find"].Value.ToString());
                XMLWriter.WriteAttributeString("replacewith", dataGridRow.Cells["replace"].Value.ToString());
                XMLWriter.WriteAttributeString("casesensitive", dataGridRow.Cells["casesensitive"].FormattedValue.ToString());
                XMLWriter.WriteAttributeString("regex", dataGridRow.Cells["regex"].FormattedValue.ToString());
                XMLWriter.WriteAttributeString("multi", dataGridRow.Cells["multi"].FormattedValue.ToString());
                XMLWriter.WriteAttributeString("single", dataGridRow.Cells["single"].FormattedValue.ToString());
                XMLWriter.WriteAttributeString("enabled", dataGridRow.Cells["enabled"].FormattedValue.ToString());
                XMLWriter.WriteAttributeString("maxnumber", "-1");

                XMLWriter.WriteEndElement();
            }

            XMLWriter.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets whether the replacements ignore external links and images
        /// </summary>
        public bool ignoreLinks
        {
            get { return chkIgnoreLinks.Checked; }
            set { chkIgnoreLinks.Checked = value; }
        }

        /// <summary>
        /// Gets or sets whether the summary should be used
        /// </summary>
        public bool AppendToSummary
        {
            get { return chkAddToSummary.Checked; }
            set { chkAddToSummary.Checked = value; }
        }

        string streditsummary = "";
        public string EditSummary
        {
            get { return streditsummary; }
            set { streditsummary = value; }
        }

        #endregion

        #region Events

        private void ChangeChecked(string col, int value)
        {
            dataGridView1.EndEdit();
            foreach (DataGridViewRow r in dataGridView1.Rows)
            {
                if (r.IsNewRow)
                    break;

                r.Cells[col].Value = value;
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            dataGridView1.EndEdit();

            if (!(bool)dataGridView1.Rows[e.RowIndex].Cells["enabled"].FormattedValue)
                dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGray;
            else
                dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
            dataGridView1.EndEdit();

        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (!ApplyDefaultFormatting)
                return;

            dataGridView1.Rows[e.RowIndex].Cells["enabled"].Value = 1;
        }

        private void FindandReplace_Shown(object sender, EventArgs e)
        {
            ApplyDefaultFormatting = true;
        }

        private void FindandReplace_FormClosing(object sender, FormClosingEventArgs e)
        {
            ApplyDefaultFormatting = false;
            MakeList();
            this.Hide();
        }

        #endregion

        #region Context menu

        private void allCaseSensitiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeChecked("casesensitive", 1);
        }

        private void uncheckAllCaseSensitiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeChecked("casesensitive", 0);
        }

        private void checkAllRegularExpressionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeChecked("regex", 1);
        }

        private void uncheckAllRegularExpressionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeChecked("regex", 0);
        }

        private void checkAllMultlineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeChecked("multi", 1);
        }

        private void uncheckAllMultilineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeChecked("multi", 0);
        }

        private void enableAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeChecked("enabled", 1);
        }

        private void disableAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeChecked("enabled", 0);
        }

        private void checkAllSinglelineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeChecked("single", 1);
        }

        private void uncheckAllSinglelineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeChecked("single", 0);
        }

        private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            while (dataGridView1.SelectedRows.Count > 0)
            {
                if (dataGridView1.Rows[dataGridView1.SelectedRows[0].Index].IsNewRow)
                {
                    dataGridView1.SelectedRows[0].Selected = false;
                    continue;
                }

                dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);
            }
        }

        private void FindAndReplaceContextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            deleteRowToolStripMenuItem.Enabled = dataGridView1.SelectedRows.Count > 0;
        }

        #endregion
    }

    public struct Replacements
    {
        public string Find;
        public string Replace;
        public int NumberOfTimes;
        public RegexOptions RegularExpressinonOptions;
    }
}
