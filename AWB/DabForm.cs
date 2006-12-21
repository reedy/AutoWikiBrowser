using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WikiFunctions;
using System.Text.RegularExpressions;

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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            tableLayout.Controls.Add(new DabControl());
            tableLayout.Controls.Add(new DabControl());
            tableLayout.Controls.Add(new DabControl());
            tableLayout.Controls.Add(new DabControl());
            Width += 30;
            Width -= 30;
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

            Search = new Regex(@"\[\[\s*(" + Tools.CaseInsensitive(Regex.Escape(dabLink)) + @")\s*(|\|[^\]]*)\]\]");

            Matches = Search.Matches(articleText);

            if (Matches.Count == 0) return articleText;

            if (ShowDialog() == DialogResult.Cancel) return articleText;



            return articleText;
        }

    }
}