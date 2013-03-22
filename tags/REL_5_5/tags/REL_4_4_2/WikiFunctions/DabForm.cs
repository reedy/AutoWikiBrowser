/*
Copyright (C) 2007 Max Semenik

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
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace WikiFunctions.Disambiguation
{
    public partial class DabForm : Form
    {
        public DabForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// if true, all processing should be immediately halted
        /// </summary>
        public bool Abort;
        bool BotMode;

        List<string> Variants = new List<string>();
        string ArticleText;
        string ArticleTitle;
        Regex Search;
        MatchCollection Matches;

        List<DabControl> Dabs = new List<DabControl>();

        static int SavedWidth;
        static int SavedHeight;
        static int SavedLeft;
        static int SavedTop;
        bool NoSave = true;

        /// <summary>
        /// displays form that promts user for disambiguation
        /// if no disambihuation needed, immediately returns
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle"></param>
        /// <param name="dabLink">link to be disambiguated</param>
        /// <param name="dabVariants">variants of disambiguation</param>
        /// <param name="contextChars">number of chars each side from link in the context box</param>
        /// <param name="botMode">whether AWB saves pages automatically</param>
        /// <param name="skip">returns true when no disambiguation made</param>
        /// <returns></returns>
        public string Disambiguate(string articleText, string articleTitle, string dabLink,
            string[] dabVariants, int contextChars, bool botMode, out bool skip)
        {
            skip = true;

            BotMode = botMode;

            if (dabLink.Contains("|"))
            {
                string sum = "";
                foreach (string s in dabLink.Split(new char[] { '|' }))
                {
                    if (s.Trim().Length == 0) continue;
                    sum += "|" + Tools.CaseInsensitive(Regex.Escape(s.Trim()));
                }
                if (sum.Length > 0 && sum[0] == '|') sum = sum.Remove(0, 1);
                if (sum.Contains("|")) sum = "(?:" + sum + ")";
                dabLink = sum;
            }
            else dabLink = Tools.CaseInsensitive(Regex.Escape(dabLink.Trim()));
            ArticleText = articleText;
            ArticleTitle = articleTitle;

            foreach (string s in dabVariants)
            {
                if (s.Trim().Length == 0) continue;
                Variants.Add(s.Trim());
            }

            if (Variants.Count == 0) return articleText;

            Search = new Regex(@"\[\[\s*(" + dabLink +
                @")\s*(?:|#[^\|\]]*)(|\|[^\]]*)\]\]([\p{Ll}\p{Lu}\p{Lt}\p{Pc}\p{Lm}]*)");

            Matches = Search.Matches(articleText);

            if (Matches.Count == 0) return articleText;

            foreach (Match m in Matches)
            {
                DabControl c = new DabControl(articleText, dabLink, m, Variants, contextChars);
                c.Changed += new EventHandler(OnUserInput);
                tableLayout.Controls.Add(c);
                Dabs.Add(c);
            }

            DialogResult r = ShowDialog(Variables.MainForm as Form);

            switch (r)
            {
                case DialogResult.OK:
                    break; // proceed further
                case DialogResult.Abort:
                    Abort = true;
                    return articleText;
                case DialogResult.Cancel:
                    skip = true;
                    return articleText; 
                    //break;
                default:
                    return articleText;
            }

            int adjust = 0;
            foreach (DabControl d in Dabs)
            {
                int start, end1, end2;
                for (start = 0; (start < Math.Min(d.Surroundings.Length, d.Result.Length)) && (d.Result[start] == d.Surroundings[start]); start++);

                end1 = d.Surroundings.Length - 1;
                end2 = d.Result.Length - 1;

                while ((end1 > start) && (end2 > start) && (d.Result[end2] == d.Surroundings[end1]))
                {
                    end1--;
                    end2--;
                }

                ArticleText = Tools.ReplacePartOfString(ArticleText, d.SurroundingsStart + start + adjust,
                    end1 - start + 1, d.Result.Substring(start, end2 - start + 1));
                adjust += d.Result.Length - d.Surroundings.Length;

                //ArticleText = Tools.ReplacePartOfString(ArticleText, d.SurroundingsStart + adjust, 
                //    d.Surroundings.Length, d.Result);
                //adjust += d.Result.Length - d.Surroundings.Length;
            }

            if (ArticleText != articleText) skip = false;

            return ArticleText;
        }

        private void btnResetAll_Click(object sender, EventArgs e)
        {
            foreach (DabControl d in Dabs)
            {
                d.Reset();
            }
        }

        private void btnUndoAll_Click(object sender, EventArgs e)
        {
            foreach (DabControl d in Dabs)
            {
                d.Undo();
            }
        }

        private void OnUserInput(object sender, EventArgs e)
        {
            bool l = true;
            foreach (DabControl d in Dabs)
            {
                l &= d.CanSave;
            }
            btnDone.Enabled = l;
        }

        private void DabForm_Load(object sender, EventArgs e)
        {
            Text += " — " + ArticleTitle;
            if (SavedWidth != 0)
            {
                Width = SavedWidth;
                Height = SavedHeight;
            }
            if (SavedLeft != 0)
            {
                
                Left = SavedLeft;
                Top = SavedTop;
            }
            NoSave = false;
            Dabs[0].Select();
        }

        private void DabForm_Move(object sender, EventArgs e)
        {
            if (!NoSave)
            {
                SavedLeft = Left;
                SavedTop = Top;
            }
        }

        private void DabForm_Resize(object sender, EventArgs e)
        {
            if (!NoSave)
            {
                SavedWidth = Width;
                SavedHeight = Height;
            }
        }

        private void DabForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 27)
            {
                e.Handled = true;
                DialogResult = BotMode ? DialogResult.Abort : DialogResult.Cancel;
            }
        }

        private void btnArticle_Click(object sender, EventArgs e)
        {
            contextMenuStripOther.Show(this, new Point(btnArticle.Left, btnArticle.Top + btnArticle.Height),
                                       ToolStripDropDownDirection.BelowRight);
        }

        private void openInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tools.OpenArticleInBrowser(ArticleTitle);
        }

        private void editInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tools.EditArticleInBrowser(ArticleTitle);
        }

        private void watchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                WikiFunctions.Browser.WebControl browser = new WikiFunctions.Browser.WebControl();
                browser.Navigate(Variables.URLLong + "index.php?title=" + Tools.WikiEncode(ArticleTitle) + "&action=watch");
                browser.Wait();
                MessageBox.Show("Page successfully added to your watchlist");
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }

        private void unwatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Browser.WebControl browser = new Browser.WebControl();
                browser.Navigate(Variables.URLLong + "index.php?title=" + Tools.WikiEncode(ArticleTitle) + "&action=unwatch");
                browser.Wait();
                MessageBox.Show("Page successfully removed from your watchlist");
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }

        private void watchWithDefaultBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tools.WatchArticleInBrowser(ArticleTitle);
        }

        private void unwatchWithDefaultBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tools.UnwatchArticleInBrowser(ArticleTitle);
        }
    }
}