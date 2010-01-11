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
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Drawing;
using WikiFunctions.Controls;

namespace WikiFunctions.Parse
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
        string streditsummary = "";

        HideText Remove = new HideText(true, false, true);

        List<Replacement> ReplacementList = new List<Replacement>();
        bool applydefault;
        private bool ApplyDefaultFormatting
        {
            get { return applydefault; }
            set
            {
                applydefault = value;
                dataGridView1.AllowUserToAddRows = value;
            }
        }

        /// <summary>
        /// Returns proper direction arrow depending on locale
        /// Currently returns only LTR arrow due to direction conflict
        /// demonstrated by http://ar.wikipedia.org/w/index.php?diff=1192871
        /// </summary>
        public static string Arrow
        {
            get
            {
                return " → ";//Variables.RTL ? " ← " : " → ";
            }
        }

        private static Replacement RowToReplacement(DataGridViewRow dataGridRow)
        {
            Replacement rep = new Replacement();
            string f = "";
            string r = "";

            rep.Enabled = ((bool)dataGridRow.Cells["enabled"].FormattedValue);

            if (dataGridRow.Cells["replace"].Value == null)
                dataGridRow.Cells["replace"].Value = "";

            f = dataGridRow.Cells["find"].Value.ToString();
            r = dataGridRow.Cells["replace"].Value.ToString();

            f = Encode(f);
            r = Encode(r);

            if (!(bool)dataGridRow.Cells["regex"].FormattedValue)
            {
                f = Regex.Escape(f);
                rep.IsRegex = false;
            }
            else
                rep.IsRegex = true;

            rep.Find = f;
            rep.Replace = r;

            rep.RegularExpressionOptions = RegexOptions.None;
            if (!(bool)dataGridRow.Cells["casesensitive"].FormattedValue)
                rep.RegularExpressionOptions = rep.RegularExpressionOptions | RegexOptions.IgnoreCase;
            if ((bool)dataGridRow.Cells["multi"].FormattedValue)
                rep.RegularExpressionOptions = rep.RegularExpressionOptions | RegexOptions.Multiline;

            if ((bool)dataGridRow.Cells["single"].FormattedValue)
                rep.RegularExpressionOptions = rep.RegularExpressionOptions | RegexOptions.Singleline;

            return rep;
        }

        public void MakeList()
        {
            ReplacementList.Clear();

            foreach (DataGridViewRow dataGridRow in dataGridView1.Rows)
            {
                if (dataGridRow.IsNewRow)
                    continue;

                if (dataGridRow.Cells["find"].Value == null)
                    continue;

                ReplacementList.Add(RowToReplacement(dataGridRow));
            }
        }

        static readonly Regex NewlineRegex = new Regex(@"(?<!\\)\\n", RegexOptions.Compiled);
        static readonly Regex TabulationRegex = new Regex(@"(?<!\\)\\t", RegexOptions.Compiled);
        static string PrepareReplacePart(string replace)
        {
            replace = NewlineRegex.Replace(replace, "\n");
            replace = TabulationRegex.Replace(replace, "\t");

            return replace;
        }


        /// <summary>
        /// Applies a series of defined find and replacements to the supplied article text.
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="ArticleTitle">The title of the article.</param>
        /// <returns>The modified article text.</returns>
        public string MultipleFindAndReplace(string ArticleText, string strTitle, ref string EditSummary)
        {
            streditsummary = "";

            if (chkIgnoreMore.Checked)
                ArticleText = Remove.HideMore(ArticleText);
            else if (chkIgnoreLinks.Checked)
                ArticleText = Remove.Hide(ArticleText);

            foreach (Replacement rep in ReplacementList)
            {
                if (!rep.Enabled)
                    continue;

                ArticleText = PerformFindAndReplace(rep.Find, rep.Replace, ArticleText, strTitle, rep.RegularExpressionOptions);
            }

            if (chkIgnoreMore.Checked)
                ArticleText = Remove.AddBackMore(ArticleText);
            else if (chkIgnoreLinks.Checked)
                ArticleText = Remove.AddBack(ArticleText);

            if (chkAddToSummary.Checked && !string.IsNullOrEmpty(streditsummary))
                EditSummary = ", Replaced: " + summary.Trim();

            return ArticleText;
        }

        Regex findRegex;
        MatchCollection Matches;
        string summary = "";

        private string PerformFindAndReplace(string Find, string Replace, string ArticleText, string ArticleTitle, RegexOptions ROptions)
        {
            Find = Tools.ApplyKeyWords(ArticleTitle, Find);
            Replace = Tools.ApplyKeyWords(ArticleTitle, PrepareReplacePart(Replace));

            findRegex = new Regex(Find, ROptions);
            Matches = findRegex.Matches(ArticleText);

            if (Matches.Count > 0)
            {
                ArticleText = findRegex.Replace(ArticleText, Replace);

                if (Matches[0].Value != Matches[0].Result(Replace))
                {
                    summary = Matches[0].Value + Arrow + Matches[0].Result(Replace);

                    if (Matches.Count > 1)
                        summary += " (" + Matches.Count.ToString() + ")";

                    streditsummary += summary + ", ";
                }
            }

            return ArticleText;
        }

        private void btnDone_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabel2.LinkVisited = true;
            Tools.OpenENArticleInBrowser("Regular_expression", false);
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

        private static string Encode(string Text)
        {
           return Text.Replace("\\r\\n", "\r\n");
        }

        private static string Decode(string Text)
        {
            return Text.Replace("\n", "\\r\\n");
        }

        #region loading/saving

        /// <summary>
        /// Adds a find and replacement task.
        /// </summary>
        /// <param name="Find">The string to find.</param>
        /// <param name="ReplaceWith">The replacement string.</param>
        public void AddNew(string Find, string ReplaceWith, bool CaseSensitive, bool IsRegex, bool Multiline, bool Singleline, bool enabled)
        {
            dataGridView1.Rows.Add(Find, ReplaceWith, CaseSensitive, IsRegex, Multiline, Singleline, enabled);
            if (!enabled)
                dataGridView1.Rows[dataGridView1.Rows.Count - 1].DefaultCellStyle.BackColor = Color.LightGray;

            MakeList();
        }

        public void AddNew(Replacement R)
        {
            bool caseSens = !R.RegularExpressionOptions.ToString().Contains("IgnoreCase");
            bool multiine = R.RegularExpressionOptions.ToString().Contains("Multiline");
            bool singleLine = R.RegularExpressionOptions.ToString().Contains("Singleline");

            if(!R.IsRegex)
                dataGridView1.Rows.Add(Regex.Unescape(Decode(R.Find)), Decode(R.Replace), caseSens, R.IsRegex, multiine, singleLine, R.Enabled);
            else
                dataGridView1.Rows.Add(Decode(R.Find), Decode(R.Replace), caseSens, R.IsRegex, multiine, singleLine, R.Enabled);

            if (!R.Enabled)
                dataGridView1.Rows[dataGridView1.Rows.Count - 1].DefaultCellStyle.BackColor = Color.LightGray;

            ReplacementList.Add(R);
        }

        public void AddNew(List<Replacement> RList)
        {
            foreach (Replacement r in RList)
            {
                AddNew(r);
            }
        }

        /// <summary>
        /// Gets the find and replace settings.
        /// </summary>
        public List<Replacement> GetList()
        {
            return ReplacementList;
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
        /// Gets or sets whether the replacements ignore headings, internal link targets, templates, and refs
        /// </summary>
        public bool ignoreMore
        {
            get { return chkIgnoreMore.Checked; }
            set { chkIgnoreMore.Checked = value; }
        }

        /// <summary>
        /// Gets or sets whether the summary should be used
        /// </summary>
        public bool AppendToSummary
        {
            get { return chkAddToSummary.Checked; }
            set { chkAddToSummary.Checked = value; }
        }

        /// <summary>
        /// Gets or sets whether the replacements are made after or before the general fixes
        /// </summary>
        public bool AfterOtherFixes
        {
            get { return chkAfterOtherFixes.Checked; }
            set { chkAfterOtherFixes.Checked = value; }
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
            deleteRowToolStripMenuItem.Enabled = (dataGridView1.SelectedRows.Count > 0);
            testRegexToolStripMenuItem.Enabled = ((dataGridView1.CurrentRow != null)
                && ((bool)dataGridView1.CurrentRow.Cells["regex"].FormattedValue));
            dataGridView1.EndEdit();
        }

        private void testRegexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = dataGridView1.CurrentRow;

            if (row == null) return;

            using (RegexTester t = new RegexTester(true))
            {
                t.Find = (string)row.Cells["find"].Value;
                t.Replace = (string)row.Cells["replace"].Value;
                t.Multiline = (bool)row.Cells["multi"].FormattedValue;
                t.Singleline = (bool)row.Cells["single"].FormattedValue;
                t.IgnoreCase = !(bool)row.Cells["casesensitive"].FormattedValue;

                if (Variables.MainForm != null && Variables.MainForm.EditBox.Enabled)
                    t.ArticleText = Variables.MainForm.EditBox.Text;

                if (t.ShowDialog(this) == DialogResult.OK)
                {
                    row.Cells["find"].Value = t.Find;
                    row.Cells["replace"].Value = t.Replace;
                    row.Cells["multi"].Value = t.Multiline;
                    row.Cells["single"].Value = t.Singleline;
                    row.Cells["casesensitive"].Value = !t.IgnoreCase;
                }
            }
        }


        #endregion

        private void chkIgnoreMore_CheckedChanged(object sender, EventArgs e)
        {
            if (chkIgnoreMore.Checked)
                chkIgnoreLinks.Checked = true;
        }

        private void chkIgnoreLinks_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkIgnoreLinks.Checked)
                chkIgnoreMore.Checked = false;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            btnSearch.Enabled = txtSearch.Text.Length > 0;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 2; i++)
                for (int j = 0; j < dataGridView1.Rows.Count; j++)
                {
                    DataGridViewCell c = dataGridView1.Rows[j].Cells[i];
                    if (c.Value != null && c.Value.ToString().Contains(txtSearch.Text))
                    {
                        dataGridView1.ClearSelection();
                        dataGridView1.EndEdit();
                        dataGridView1.CurrentCell = c;
                        if (!c.Displayed) dataGridView1.FirstDisplayedScrollingRowIndex = c.RowIndex;
                        dataGridView1.Focus();
                        dataGridView1.BeginEdit(false);
                    }
                }
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                e.Handled = true;
                btnSearch_Click(null, null);
            }
        }

        private void addRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
                dataGridView1.Rows.Insert(dataGridView1.SelectedRows[0].Index, 1);
            else
                dataGridView1.Rows.Add();
        }

        private void moveUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                if (row.Index > 0)
                {
                    int index = row.Index;
                    DataGridViewRow tmp = row;
                    dataGridView1.Rows.Remove(row);
                    dataGridView1.Rows.Insert(index - 1, tmp);
                }
            }
        }

        private void moveDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                if (row.Index != (dataGridView1.Rows.Count - 2))
                {
                    int index = row.Index;
                    DataGridViewRow tmp = row;
                    dataGridView1.Rows.Remove(row);
                    dataGridView1.Rows.Insert(index + 1, tmp);
                }
            }
        }
    }

    public struct Replacement
    {
        public Replacement(string Find, string Replace, bool IsRegex, bool Enabled, RegexOptions RegularExpressionOptions)
        {
            this.Find = Find;
            this.Replace = Replace;
            this.IsRegex = IsRegex;
            this.Enabled = Enabled;
            this.RegularExpressionOptions = RegularExpressionOptions;
        }

        public string Find;
        public string Replace;

        public bool IsRegex;
        public bool Enabled;

        public RegexOptions RegularExpressionOptions;
    }
}
