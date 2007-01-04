using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WikiFunctions;
using System.Text.RegularExpressions;
using WikiFunctions.Disambiguation;

namespace AutoWikiBrowser
{
    public partial class DabForm : Form
    {
        public DabForm()
        {
            InitializeComponent();
        }

        List<string> Variants = new List<string>();
        string DabLink;
        string ArticleText;
        string ArticleTitle;
        Regex Search;
        MatchCollection Matches;

        List<DabControl> Dabs = new List<DabControl>();

        static int SavedWidth = 0;
        static int SavedHeight = 0;
        static int SavedLeft = 0;
        static int SavedTop = 0;

        private void btnCancel_Click(object sender, EventArgs e)
        {
        }

        public string Disambiguate(string articleText, string articleTitle, string dabLink,
            string[] dabVariants, out bool Skip)
        {
            Skip = true;

            DabLink = dabLink;
            ArticleText = articleText;
            ArticleTitle = articleTitle;

            foreach (string s in dabVariants)
            {
                if (s.Trim() == "") continue;
                Variants.Add(s.Trim());
            }

            if (Variants.Count == 0) return articleText;

            Search = new Regex(@"\[\[\s*(" + Tools.CaseInsensitive(Regex.Escape(dabLink)) + 
                @")\s*(?:|#[^\|\]]*)(|\|[^\]]*)\]\]");

            Matches = Search.Matches(articleText);

            if (Matches.Count == 0) return articleText;

            foreach (Match m in Matches)
            {
                DabControl c = new DabControl(articleText, dabLink, m, Variants);
                c.Changed += new EventHandler(OnUserInput);
                tableLayout.Controls.Add(c);
                Dabs.Add(c);
            }

            if (ShowDialog(Application.OpenForms[0]) != DialogResult.OK) return articleText;

            int adjust = 0;
            foreach (DabControl d in Dabs)
            {
                ArticleText = ArticleText.Remove(d.SurroundingsStart + adjust, d.Surroundings.Length);
                ArticleText = ArticleText.Insert(d.SurroundingsStart + adjust, d.Result);
                adjust += d.Result.Length - d.Surroundings.Length;
            }

            if (ArticleText != articleText) Skip = false;

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

        private void btnOpenInBrowser_Click(object sender, EventArgs e)
        {
            if (Variables.Project == ProjectEnum.custom)
                System.Diagnostics.Process.Start(Variables.URLLong + "index.php?title=" + ArticleTitle);
            else System.Diagnostics.Process.Start(Variables.URL + "/wiki/" + ArticleTitle);
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
        }

        private void DabForm_Move(object sender, EventArgs e)
        {
            SavedLeft = Left;
            SavedTop = Top;
        }

        private void DabForm_Resize(object sender, EventArgs e)
        {
            SavedWidth = Width;
            SavedHeight = Height;
        }
    }
}