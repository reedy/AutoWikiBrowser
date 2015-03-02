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
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using WikiFunctions.Controls;

namespace WikiFunctions.Parse
{
    //TODO: User IArticleComparer derivatives where possible

    /// <summary>
    /// Provides a form and functions for setting and applying multiple find and replacements on a text string.
    /// </summary>
    public partial class FindandReplace : Form
    {
        public FindandReplace()
        {
            InitializeComponent();
        }

        private readonly HideText _remove = new HideText(true, false, true);

        private List<Replacement> _replacementList = new List<Replacement>();
        private List<Replacement> replacementBackup;

        private bool _applyDefault;
        private bool ApplyDefaultFormatting
        {
            get { return _applyDefault; }
            set
            {
                _applyDefault = value;
                dataGridView1.AllowUserToAddRows = value;
            }
        }

        /// <summary>
        /// Returns proper direction arrow depending on locale
        /// Currently returns only LTR arrow due to direction conflict
        /// demonstrated by https://ar.wikipedia.org/w/index.php?diff=1192871
        /// July 2012: from discusison at https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_20#The_arrow_in_.22replaced.22_edit_summaries_points_the_wrong_way_in_right-to-left_languages
        /// Arrow derivation reinstated
        /// </summary>
        public static string Arrow
        {
            get
            {
                return Variables.RTL ? " ← " : " → ";
            }
        }

        private static Replacement RowToReplacement(DataGridViewRow dataGridRow)
        {
            Replacement rep = new Replacement
                                  {
                                      Enabled = ((bool)dataGridRow.Cells["enabled"].FormattedValue),
                                      Minor = ((bool)dataGridRow.Cells["minor"].FormattedValue),
                                      IsRegex = ((bool)dataGridRow.Cells["regex"].FormattedValue),
                                      BeforeOrAfter = ((bool)dataGridRow.Cells["BeforeOrAfter"].FormattedValue)
                                  };

            if (dataGridRow.Cells["replace"].Value == null)
                dataGridRow.Cells["replace"].Value = "";

            string f = Encode(dataGridRow.Cells["find"].Value.ToString());
            string r = Encode(dataGridRow.Cells["replace"].Value.ToString());

            // in F&R newline matching is on \n, so if not a regex ensure this isn't escaped
            if (!rep.IsRegex)
            {
                bool newlines = f.Contains("\\n");
                f = Regex.Escape(f);
                
                if(newlines)
                    f = f.Replace(@"\\n", "\n");
            }

            rep.Find = f;
            rep.Replace = r;

            if (!(bool)dataGridRow.Cells["casesensitive"].FormattedValue)
                rep.RegularExpressionOptions |= RegexOptions.IgnoreCase;

            if ((bool)dataGridRow.Cells["multi"].FormattedValue)
                rep.RegularExpressionOptions |= RegexOptions.Multiline;

            if ((bool)dataGridRow.Cells["single"].FormattedValue)
                rep.RegularExpressionOptions |= RegexOptions.Singleline;

            rep.Comment = (string)dataGridRow.Cells["comment"].FormattedValue ?? "";

            return rep;
        }

        public void MakeList()
        {
            _replacementList.Clear();

            foreach (DataGridViewRow dataGridRow in dataGridView1.Rows)
            {
                if (dataGridRow.IsNewRow || dataGridRow.Cells["find"].Value == null)
                    continue;

                _replacementList.Add(RowToReplacement(dataGridRow));
            }
        }

        private static readonly Regex NewlineRegex = new Regex(@"(?<!\\)\\n", RegexOptions.Compiled),
            TabulationRegex = new Regex(@"(?<!\\)\\t", RegexOptions.Compiled);

        private static string PrepareReplacePart(string replace)
        {
            replace = NewlineRegex.Replace(replace, "\n");
            return TabulationRegex.Replace(replace, "\t");
        }

        /// <summary>
        /// Returns the number of replacements (enabled and disabled)
        /// </summary>
        public int NoOfReplacements { get { return _replacementList.Count; } }

        /// <summary>
        /// Returns whether there are any replacements (enabled and disabled)
        /// </summary>
        public bool HasReplacements { get { return NoOfReplacements != 0; } }

        /// <summary>
        /// Returns whether any of the enabled find & replace entries are specified to be run 'after processing'
        /// </summary>
        public bool HasAfterProcessingReplacements
        {
            get
            {
                return HasProcessingReplacements(true);
            }
        }
        
        /// <summary>
        /// Returns whether any of the enabled find & replace entries are specified to be run at before/after as input
        /// </summary>
        public bool HasProcessingReplacements(bool after)
        {
            foreach (Replacement rep in _replacementList)
            {
                if (rep.Enabled && ((after && rep.BeforeOrAfter) || (!after && !rep.BeforeOrAfter)))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Applies a series of defined find and replacements to the supplied article text.
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="editSummary"></param>
        /// <param name="strTitle"></param>
        /// <param name="beforeOrAfter">False if "before", true if "after"</param>
        /// <param name="majorChangesMade"></param>
        /// <returns>The modified article text.</returns>
        public string MultipleFindAndReplace(string articleText, string strTitle, bool beforeOrAfter, ref string editSummary, out bool majorChangesMade)
        {
            majorChangesMade = false;

            if (!HasReplacements)
                return articleText;

            ReplacedSummary = "";
            RemovedSummary = "";

            if (chkIgnoreMore.Checked)
            {
                articleText = _remove.HideMore(articleText);
            }
            else if (chkIgnoreLinks.Checked)
            {
                articleText = _remove.Hide(articleText);
            }

            foreach (Replacement rep in _replacementList)
            {
                if (!rep.Enabled || rep.BeforeOrAfter != beforeOrAfter)
                    continue;

                bool changeMade;
                articleText = PerformFindAndReplace(rep, articleText, strTitle, out changeMade);

                if (changeMade && !rep.Minor)
                {
                    majorChangesMade = true;
                }
            }

            if (chkIgnoreMore.Checked)
            {
                // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_24#FormatException_in_HideText.AddBackMore
                // FIXME: Usages of IgnoreMore with number (or M) replacement done in the FindAndReplace can cause corruption
                // e.g. Replacing 2 with "" ⌊⌊⌊⌊M2⌋⌋⌋⌋ becomes ⌊⌊⌊⌊M⌋⌋⌋⌋
                // This cannot then be added back
                articleText = _remove.AddBackMore(articleText);
            }
            else if (chkIgnoreLinks.Checked)
            {
                articleText = _remove.AddBack(articleText);
            }

            if (chkAddToSummary.Checked)
            {
                if (!string.IsNullOrEmpty(ReplacedSummary))
	                if (Variables.LangCode.Equals("ar"))
	                    editSummary = "استبدل: " + ReplacedSummary.Trim();
	                else if (Variables.LangCode.Equals("arz"))
	                    editSummary = "غير: " + ReplacedSummary.Trim();
	                else if (Variables.LangCode.Equals("el"))
	                    editSummary = "αντικατέστησε: " + ReplacedSummary.Trim();
	                else if (Variables.LangCode.Equals("eo"))
	                    editSummary = "anstataŭigis: " + ReplacedSummary.Trim();
	                else if (Variables.LangCode.Equals("fr"))
	                    editSummary = "remplacement: " + ReplacedSummary.Trim();
	                else if (Variables.LangCode.Equals("hy"))
	                    editSummary = "փոխարինվեց: " + ReplacedSummary.Trim();
	                else if (Variables.LangCode.Equals("tr"))
	                    editSummary = "değiştirildi: " + ReplacedSummary.Trim();
                	else
		                editSummary += "replaced: " + ReplacedSummary.Trim();

                if (!string.IsNullOrEmpty(RemovedSummary))
                {
                    if (!string.IsNullOrEmpty(editSummary))
                    {
                    	if (Variables.LangCode.Equals("ar") || Variables.LangCode.Equals("arz") || Variables.LangCode.Equals("fa"))
                        editSummary += "، ";
                    	else
                        editSummary += ", ";                    		
                    }

	                if (Variables.LangCode.Equals("ar"))
	                    editSummary += "أزال: " + RemovedSummary.Trim();
	                else if (Variables.LangCode.Equals("arz"))
	                    editSummary += "شال: " + RemovedSummary.Trim();
	                else if (Variables.LangCode.Equals("el"))
	                    editSummary += "αφαίρεσε: " + RemovedSummary.Trim();
	                else if (Variables.LangCode.Equals("eo"))
	                    editSummary += "forigis: " + RemovedSummary.Trim();
	                else if (Variables.LangCode.Equals("fr"))
	                    editSummary += "retrait: " + RemovedSummary.Trim();
	                else if (Variables.LangCode.Equals("hy"))
	                    editSummary += "ջնջվեց: " + RemovedSummary.Trim();
	                else if (Variables.LangCode.Equals("tr"))
	                    editSummary += "çıkartıldı:" + RemovedSummary.Trim();
	                else
	                    editSummary += "removed: " + RemovedSummary.Trim();
                }
            }

            return articleText;
        }

        public string RemovedSummary { get; private set; }
        public string ReplacedSummary { get; private set; }

        /// <summary>
        /// Executes a single find & replace rule against the article text. First applies keywords to the find and replace portions of the rule, then executes the rule.
        /// Edit summary is generated from the first match of the rule that changes the article text on replacement. Count of changes is replacements affecting article, not total matches
        /// i.e. no-change replacements are not counted in the edit summary
        /// </summary>
        /// <param name="rep">F&amp;R rule to execute</param>
        /// <param name="articleText">The article text</param>
        /// <param name="articleTitle">The article title</param>
        /// <param name="changeMade">Whether the F&amp;R rule caused changes to the article text</param>
        /// <returns>The updated article text</returns>
        public string PerformFindAndReplace(Replacement rep, string articleText, string articleTitle, out bool changeMade)
        {
            if (rep == null) throw new ArgumentNullException("rep");

            string findThis = Tools.ApplyKeyWords(articleTitle, rep.Find, true), replaceWith = Tools.ApplyKeyWords(articleTitle, PrepareReplacePart(rep.Replace));

            string comma = @", ";
            if (Variables.LangCode.Equals("ar") || Variables.LangCode.Equals("arz") || Variables.LangCode.Equals("fa"))
            	comma = @"، ";
            
            Regex findRegex = new Regex(findThis, rep.RegularExpressionOptions);

            int Repcount = 0, Remcount = 0;

            // use first replace that changes article text to generate edit summary
            string res = findRegex.Replace(articleText, m =>  {
                                               string mres = m.Result(replaceWith);

                                               if (!m.Value.Equals(mres))
                                               {
                                                   if (!string.IsNullOrEmpty(mres))
                                                   {
                                                       if(Repcount == 0)
                                                       {
                                                           if (!string.IsNullOrEmpty(ReplacedSummary)) //Add comma before next replaced
                                                               ReplacedSummary += comma;

                                                           ReplacedSummary += m.Value + Arrow + mres;
                                                       }
                                                       Repcount++;
                                                   }
                                                   else
                                                   {
                                                       if(Remcount == 0)
                                                       {
                                                           if (!string.IsNullOrEmpty(RemovedSummary)) //Add comma before next removed
                                                               RemovedSummary += comma;

                                                           RemovedSummary += m.Value;
                                                       }
                                                       Remcount++;
                                                   }
                                               }
                                               
                                               return mres;
                                           } );

            // update summaries with count of changes
            if(Repcount > 1)
                ReplacedSummary += " (" + Repcount + ")";

            if(Remcount > 1)
                RemovedSummary += " (" + Remcount + ")";

            changeMade = (Repcount + Remcount > 0);

            return res;
        }

        private void btnDone_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            lnkWpRE.LinkVisited = true;
            Tools.OpenENArticleInBrowser("Regular_expression", false);
        }

        private void lblMsdn_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            lblMsdn.LinkVisited = true;
            Tools.OpenURLInBrowser("http://msdn.microsoft.com/en-us/library/hs600312.aspx");
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you really want to clear the whole table?", "Really clear?", MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Clear();
            }
        }

        /// <summary>
        /// Clears the set replacements.
        /// </summary>
        public void Clear()
        {
            _replacementList.Clear();
            dataGridView1.Rows.Clear();
        }

        private static string Encode(string text)
        {
            return text.Replace("\\r\\n", "\r\n");
        }

        private static string Decode(string text)
        {
            return text.Replace("\n", "\\r\\n");
        }

        #region loading/saving

        /// <summary>
        /// Loads a single replacement entry into the find and replace data grid
        /// </summary>
        /// <param name="r"></param>
        public void AddNew(Replacement r)
        {
            bool caseSens = (r.RegularExpressionOptions & RegexOptions.IgnoreCase) != RegexOptions.IgnoreCase;
            bool multiline = (r.RegularExpressionOptions & RegexOptions.Multiline) == RegexOptions.Multiline;
            bool singleLine = (r.RegularExpressionOptions & RegexOptions.Singleline) == RegexOptions.Singleline;

            dataGridView1.Rows.Add(r.IsRegex ? Decode(r.Find) : Regex.Unescape(Decode(r.Find)), Decode(r.Replace),
                                   caseSens, r.IsRegex, multiline, singleLine, r.Minor, r.BeforeOrAfter, r.Enabled, r.Comment);

            _replacementList.Add(r);
        }

        /// <summary>
        /// Loads list of replacement entries into the find and replace data grid
        /// </summary>
        /// <param name="rList"></param>
        public void AddNew(List<Replacement> rList)
        {
            foreach (Replacement r in rList)
            {
                AddNew(r);
            }
        }

        /// <summary>
        /// Gets the find and replace settings.
        /// </summary>
        public List<Replacement> GetList()
        {
            return _replacementList;
        }

        /// <summary>
        /// Gets or sets whether the replacements ignore external links and images
        /// </summary>
        public bool IgnoreLinks
        {
            get { return chkIgnoreLinks.Checked; }
            set { chkIgnoreLinks.Checked = value; }
        }

        /// <summary>
        /// Gets or sets whether the replacements ignore headings, internal link targets, templates, and refs
        /// </summary>
        public bool IgnoreMore
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

        #endregion

        #region Events

        private void ChangeChecked(string col, bool value)
        {
            dataGridView1.EndEdit();
            foreach (DataGridViewRow r in dataGridView1.Rows)
            {
                if (r.IsNewRow)
                    break;

                r.Cells[col].Value = value;
            }
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (!ApplyDefaultFormatting)
                return;

            dataGridView1.Rows[e.RowIndex].Cells["enabled"].Value = true;
        }

        private void FindandReplace_Shown(object sender, EventArgs e)
        {
            DialogResult = DialogResult.None;
            ApplyDefaultFormatting = true;
        }

        private void FindandReplace_FormClosing(object sender, FormClosingEventArgs e)
        {
            ApplyDefaultFormatting = false;
            Hide();
            if (replacementBackup != null && DialogResult == DialogResult.Cancel)
            {
                dataGridView1.Rows.Clear();
                AddNew(replacementBackup);
                _replacementList.Clear();
                _replacementList = replacementBackup;
                replacementBackup = null;
                return;
            }
            MakeList();
        }

        #endregion

        #region Context menu

        private void allCaseSensitiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeChecked("casesensitive", true);
        }

        private void uncheckAllCaseSensitiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeChecked("casesensitive", false);
        }

        private void checkAllRegularExpressionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeChecked("regex", true);
        }

        private void uncheckAllRegularExpressionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeChecked("regex", false);
        }

        private void checkAllMultlineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeChecked("multi", true);
        }

        private void uncheckAllMultilineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeChecked("multi", false);
        }

        private void enableAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeChecked("enabled", true);
        }

        private void disableAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeChecked("enabled", false);
        }

        private void checkAllSinglelineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeChecked("single", true);
        }

        private void uncheckAllSinglelineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeChecked("single", false);
        }

        private void checkAllBeforeOrAfterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeChecked("beforeorafter", true);
        }

        private void uncheckAllBeforeOrAfterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeChecked("beforeorafter", false);
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
            testRegexToolStripMenuItem.Enabled = createRetfRuleToolStripMenuItem.Enabled =
                ((dataGridView1.CurrentRow != null)
                && ((bool)dataGridView1.CurrentRow.Cells["regex"].FormattedValue));
        }

        private void testRegexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = dataGridView1.CurrentRow;

            if (row == null) return;

            using (RegexTester t = new RegexTester(true))
            {
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
                t.Find = (string)row.Cells["find"].Value;
                t.Replace = (string)row.Cells["replace"].Value;
                t.Multiline = (bool)row.Cells["multi"].FormattedValue;
                t.Singleline = (bool)row.Cells["single"].FormattedValue;
                t.IgnoreCase = !(bool)row.Cells["casesensitive"].FormattedValue;

                if (Variables.MainForm != null && Variables.MainForm.EditBox.Enabled)
                    t.ArticleText = Variables.MainForm.EditBox.Text;

                if (t.ShowDialog(this) != DialogResult.OK) return;
                row.Cells["find"].Value = t.Find;
                row.Cells["replace"].Value = t.Replace;
                row.Cells["multi"].Value = t.Multiline;
                row.Cells["single"].Value = t.Singleline;
                row.Cells["casesensitive"].Value = !t.IgnoreCase;
                dataGridView1.RefreshEdit();
            }
        }
        #endregion

        private void chkIgnoreMore_CheckedChanged(object sender, EventArgs e)
        {
            if (chkIgnoreMore.Checked)
            {
                chkIgnoreLinks.Checked = true;
                chkIgnoreLinks.Enabled = false;
            }
            else
                 chkIgnoreLinks.Enabled = true;
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
                        if (!c.Displayed)
                        {
                            dataGridView1.FirstDisplayedScrollingRowIndex = c.RowIndex;
                        }
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
                dataGridView1.Rows.Insert(dataGridView1.SelectedRows[0].Index);
            else
                dataGridView1.Rows.Add();
        }

        private void moveUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                if (row.Index > 0 && row.Index < (dataGridView1.Rows.Count - 1))
                {
                    int index = row.Index;
                    DataGridViewRow tmp = row;
                    dataGridView1.Rows.Remove(row);
                    ApplyDefaultFormatting = false;
                    dataGridView1.Rows.Insert(index - 1, tmp);
                    ApplyDefaultFormatting = true;
                    SelectRowAndFocusColumn(index-1);
                }
            }
        }

        private void moveDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                if (row.Index < (dataGridView1.Rows.Count - 2))
                {
                    int index = row.Index;
                    DataGridViewRow tmp = row;
                    dataGridView1.Rows.Remove(row);
                    ApplyDefaultFormatting = false;
                    dataGridView1.Rows.Insert(index + 1, tmp);
                    ApplyDefaultFormatting = true;
                    SelectRowAndFocusColumn(index+1);
                }
            }
        }

        private void moveToTopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                if (row.Index > 0 && row.Index < (dataGridView1.Rows.Count - 1))
                {
                    DataGridViewRow tmp = row;
                    dataGridView1.Rows.Remove(row);
                    ApplyDefaultFormatting = false;
                    dataGridView1.Rows.Insert(0, tmp);
                    ApplyDefaultFormatting = true;
                    SelectRowAndFocusColumn(0);
                }
            }
        }

        private void moveToBottomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                if (row.Index < (dataGridView1.Rows.Count - 2))
                {
                    DataGridViewRow tmp = row;
                    dataGridView1.Rows.Remove(row);
                    ApplyDefaultFormatting = false;
                    dataGridView1.Rows.Add(tmp);
                    ApplyDefaultFormatting = true;
                    SelectRowAndFocusColumn(dataGridView1.Rows.Count-2); // zero based row and always one blank at end
                }
            }
        }

        /// <summary>
        /// After a user has just moved a row: selects the moved row and returns focus to the previously selected column
        /// </summary>
        /// <param name='row'>
        /// Moved row number
        /// </param>
        private void SelectRowAndFocusColumn(int row)
        {
            dataGridView1.Rows[row].Selected = true;
            dataGridView1.CurrentCell = dataGridView1[dataGridView1.CurrentCell.ColumnIndex, row]; // focus on column that currently has focus, but in the moved row
        }

        private void createRetfRuleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = dataGridView1.CurrentRow;

            if (row == null)
                return;

            string typoName = (string)row.Cells["Comment"].Value;
            if (string.IsNullOrEmpty(typoName))
                typoName = "<enter a name>";

            Tools.CopyToClipboard(RegExTypoFix.CreateRule((string)row.Cells["find"].Value,
                                                          (string)row.Cells["replace"].Value, typoName));
        }

        private void checkAllMinorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeChecked("minor", true);
        }

        private void uncheckAllMinorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeChecked("minor", false);
        }

        private void FindandReplace_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                replacementBackup = _replacementList.ConvertAll(repl => new Replacement(repl));
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Replacement
    {
        public Replacement()
        {
            RegularExpressionOptions = RegexOptions.None;
        }

        public Replacement(string find, string replace, bool isRegex, bool enabled, bool minor, bool beforeOrAfter,
                           RegexOptions regularExpressionOptions, string comment)
        {
            Find = find;
            Replace = replace;
            IsRegex = isRegex;
            Enabled = enabled;
            Minor = minor;
            BeforeOrAfter = beforeOrAfter;
            RegularExpressionOptions = regularExpressionOptions;
            Comment = comment;
        }

        public Replacement(Replacement repl)
            : this(
                repl.Find, repl.Replace, repl.IsRegex, repl.Enabled, repl.Minor,
                repl.BeforeOrAfter, repl.RegularExpressionOptions, repl.Comment)
        {
        }

        public string Find,
            Replace, Comment;

        public bool IsRegex,
            Enabled, Minor, BeforeOrAfter;

        public RegexOptions RegularExpressionOptions;
    }
}
