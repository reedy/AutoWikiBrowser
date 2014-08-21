using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using WikiFunctions.Parse;

namespace WikiFunctions.Controls
{
    public partial class TypoStatsControl : NoFlickerExtendedListView
    {
        private Dictionary<string, TypoStat> Data;

        /// <summary>
        /// Whether this controll accumulates statistics for the whole run
        /// </summary>
        [DefaultValue(false)]
        public bool IsOverallStats
        {
            get { return Data != null; }
            set
            {
                if (value)
                {
                    if (Data == null) Data = new Dictionary<string, TypoStat>();
                }
                else
                    Data = null;
            }
        }

        public TypoStatsListViewItem SelectedItem
        {
            get
            {
                if (SelectedItems.Count == 0) return null;

                return (TypoStatsListViewItem) SelectedItems[0];
            }
        }

        public int TotalTypos, SelfMatches, FalsePositives, Saves, Pages;

        public string TyposPerSave
        {
            get
            {
                double fixes = TotalTypos - SelfMatches - FalsePositives;
                // fixed 2 decimal places http://www.csharp-examples.net/string-format-double/
                return string.Format("{0:0.00}", fixes/Saves);
            }
        }

        public void ClearStats()
        {
            if (Data != null) Data.Clear();
            Items.Clear();
            TotalTypos = SelfMatches = FalsePositives = Saves = Pages = 0;
        }

        private void CountStats()
        {
            TotalTypos = SelfMatches = FalsePositives = 0;
            foreach (TypoStat st in Data.Values)
            {
                TotalTypos += st.Total;
                SelfMatches += st.SelfMatches;
                FalsePositives += st.FalsePositives;
            }
        }

        /// <summary>
        /// Updates statistics
        /// </summary>
        /// <param name="stats">Results of typo processing on one page</param>
        /// <param name="skipped">If true, the page was skipped, otherwise skipped</param>
        public void UpdateStats(IEnumerable<TypoStat> stats, bool skipped)
        {
            if (stats == null) return;
            BeginUpdate();
            if (IsOverallStats)
            {
                foreach (TypoStat typo in stats)
                {
                    TypoStat old;
                    if (Data.TryGetValue(typo.Find, out old))
                    {
                        old.Total += typo.Total;
                        old.SelfMatches += typo.SelfMatches;
                        old.FalsePositives += typo.FalsePositives;

                        // if skipped, all changes considered false positives
                        if (skipped) old.FalsePositives += typo.Total - typo.SelfMatches;

                        old.ListViewItem.Refresh();
                    }
                    else
                    {
                        Data.Add(typo.Find, typo);
                        Items.Add(new TypoStatsListViewItem(typo));
                    }
                }
                Pages++;
                if (!skipped) Saves++;
                CountStats();
            }
            else
            {
                Items.Clear();
                foreach (TypoStat typo in stats)
                {
                    Items.Add(new TypoStatsListViewItem(typo));
                }
            }
            EndUpdate();
        }

        private void contextMenu_Opening(object sender, CancelEventArgs e)
        {
            miCopyFind.Enabled = miCopyReplace.Enabled = miTestRegex.Enabled = SelectedItems.Count > 0;
            miClear.Visible = miSaveLog.Visible = IsOverallStats;
        }

        private void miClear_Click(object sender, EventArgs e)
        {
            ClearStats();
        }

        private void miCopyReplace_Click(object sender, EventArgs e)
        {
            TypoStatsListViewItem typo = SelectedItem;
            if (typo == null) return;

            Tools.CopyToClipboard(typo.Typo.Replace);
        }

        private void miCopyFind_Click(object sender, EventArgs e)
        {
            TypoStatsListViewItem typo = SelectedItem;
            if (typo == null) return;

            Tools.CopyToClipboard(typo.Typo.Find);
        }

        private void TestRegex(object sender, EventArgs e)
        {
            TypoStatsListViewItem typo = SelectedItem;
            if (typo == null) return;

            using (RegexTester t = new RegexTester())
            {
                t.Find = typo.Typo.Find;
                t.Replace = typo.Typo.Replace;
                if (Variables.MainForm != null && Variables.MainForm.EditBox.Enabled)
                    t.ArticleText = Variables.MainForm.EditBox.Text;

                t.ShowDialog(FindForm());
            }
        }

        private void miSaveLog_Click(object sender, EventArgs e)
        {
            if ((Saves > 0) && (saveListDialog.ShowDialog() == DialogResult.OK))
            {
                System.Text.StringBuilder strList = new System.Text.StringBuilder();

                strList.AppendLine("Total: " + TotalTypos);
                strList.AppendLine("No change: " + SelfMatches);
                strList.AppendLine("Typo/save: " + TyposPerSave);

                foreach (TypoStatsListViewItem item in Items)
                {
                    strList.AppendLine(item.SubItems[0].Text + ", " + item.SubItems[1].Text + ", " +
                                       item.SubItems[2].Text + ", " + item.SubItems[3].Text);
                }

                Tools.WriteTextFileAbsolutePath(strList.ToString(), saveListDialog.FileName, false);
            }
        }
    }

    public class TypoStatsListViewItem : ListViewItem
    {
        public TypoStat Typo;
        private bool IsYellow;

        public TypoStatsListViewItem(TypoStat stat)
            : base(new[] {stat.Find, stat.Replace, "", ""})
        {
            Typo = stat;
            Typo.ListViewItem = this;
            Refresh();
        }

        public void Refresh()
        {
            SubItems[2].Text = Typo.Total.ToString();
            SubItems[3].Text = Typo.SelfMatches.ToString();

            if ((Typo.Total == Typo.SelfMatches) && !IsYellow)
            {
                BackColor = Color.Yellow;
                IsYellow = true;
            }
            else if (IsYellow)
            {
                BackColor = Color.White;
                IsYellow = false;
            }
        }
    }
}
