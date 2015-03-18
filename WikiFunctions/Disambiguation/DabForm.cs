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
using System.Linq;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace WikiFunctions.Disambiguation
{
    public partial class DabForm : Form
    {
        public DabForm(Session session)
        {
            InitializeComponent();
            Session = session;
        }

        /// <summary>
        /// if true, all processing should be immediately halted
        /// </summary>
        public bool Abort;

        private bool BotMode;

        private readonly List<string> Variants = new List<string>();
        private string ArticleTitle;
        private Regex Search;

        private readonly List<DabControl> Dabs = new List<DabControl>();

        private static int SavedWidth, SavedHeight, SavedLeft, SavedTop;
        private bool NoSave = true;

        private readonly Session Session;

        /// <summary>
        /// Matches end of wikilink then {{Disambiguation needed}} template then punctuation
        /// </summary>
        private static readonly Regex DnPunctuationR = new Regex(@"(\]\])({{Disambiguation needed}})([.,'"":;]+)");

        /// <summary>
        /// Displays a form that promts user for disambiguation
        /// if no disambiguation needed, immediately returns
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">Title of the article</param>
        /// <param name="dabLink">link to be disambiguated</param>
        /// <param name="dabVariants">variants of disambiguation</param>
        /// <param name="contextChars">number of chars each side from link in the context box</param>
        /// <param name="botMode">whether AWB saves pages automatically</param>
        /// <param name="skip">returns true when no disambiguation made</param>
        /// <returns></returns>
        public string Disambiguate(string articleText, string articleTitle, string dabLink,
            string[] dabVariants, int contextChars, bool botMode, out bool skip)
        {
            Variants.Clear();
            Dabs.Clear();

            skip = true;

            foreach (string s in dabVariants)
            {
                if (s.Trim().Length > 0)
                    Variants.Add(s.Trim());
            }

            if (Variants.Count == 0) return articleText;

            BotMode = botMode;

            if (dabLink.Contains("|"))
            {
                string sum = dabLink.Split(new[] {'|'})
                    .Where(s => s.Trim().Length != 0)
                    .Aggregate("", (current, s) => current + ("|" + Tools.CaseInsensitive(Regex.Escape(s.Trim()))));
                if (sum.Length > 0 && sum[0] == '|')
                    sum = sum.Remove(0, 1);
                if (sum.Contains("|"))
                    sum = "(?:" + sum + ")";
                dabLink = sum;
            }
            else
                dabLink = Tools.CaseInsensitive(Regex.Escape(dabLink.Trim()));

            string newText = articleText;
            ArticleTitle = articleTitle;

            Search = new Regex(@"\[\[\s*(" + dabLink +
                               @")\s*(?:|#[^\|\]]*)(|\|[^\]]*)\]\]([\p{Ll}\p{Lu}\p{Lt}\p{Pc}\p{Lm}]*)");

            MatchCollection matches = Search.Matches(articleText);

            if (matches.Count == 0)
                return articleText;

            foreach (Match m in matches)
            {
                DabControl c = new DabControl(articleText, m, Variants, contextChars);
                c.Changed += OnUserInput;
                tableLayout.Controls.Add(c);
                Dabs.Add(c);
            }

            switch (ShowDialog(Variables.MainForm as Form))
            {
                case DialogResult.OK:
                    break; // proceed further
                case DialogResult.Abort:
                    Abort = true;
                    goto default;
                default: //DialogResult.Cancel
                    return articleText;
            }

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_22#Errors_disambiguating.2C_possibly_due_to_link_occurring_more_than_once_in_a_line
            // to perform replace perform each replace via regex, take user's chosen link from the matching dab by index
            int a = 0;
            bool dnPunctuation = false;

            newText = Search.Replace(newText, m2 =>
            {
                string res = Dabs[a].NoChange ? m2.Value : Dabs[a].Result;

                if (res.Contains(@"{{Disambiguation needed}}"))
                    dnPunctuation = true;

                a++;
                return res;
            });

            if (!newText.Equals(articleText))
                skip = false;

            // want ''[[link]]''{{Disambiguation needed}} rather than ''[[link]]{{Disambiguation needed}}''
            if (dnPunctuation)
                newText = DnPunctuationR.Replace(newText, "$1$3$2");

            return Parse.Parsers.StickyLinks(newText);
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
            btnDone.Enabled = Dabs.Aggregate(true, (current, d) => current & d.CanSave);
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
            Session.Site.OpenPageInBrowser(ArticleTitle);
        }

        private void editInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tools.EditArticleInBrowser(ArticleTitle);
        }

        private void watchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Session.Editor.Clone().Watch(ArticleTitle);
                MessageBox.Show("Page successfully added to your watchlist");
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex);
            }
        }

        private void unwatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Session.Editor.Clone().Watch(ArticleTitle);
                MessageBox.Show("Page successfully removed from your watchlist");
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex);
            }
        }
    }
}
