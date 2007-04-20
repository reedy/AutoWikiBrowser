using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace WikiFunctions.Disambiguation
{
    public partial class DabControl : UserControl
    {
        public DabControl()
        {
            InitializeComponent();
        }

        public DabControl(string articleText, string link, Match match, List<string> variants, int contextChars)
        {
            ArticleText = articleText;
            dabLink = link;
            Match = match;
            Variants = variants;
            ContextChars = contextChars;

            InitializeComponent();
        }

        public event EventHandler Changed;

        // input data
        public string dabLink;
        public string ArticleText;
        public Match Match;
        public List<string> Variants;

        // output data
        public string Surroundings;
        public int SurroundingsStart = 0;
        public string Result
        {
            get
            {
                return txtCorrection.Text;
            }
        }

        //internal
        int ContextChars;
        int posStart;
        int posEnd;
        bool StartOfSentence = false;
        string VisibleLink;
        string CurrentLink;
        int PosInSurroundings;

        static Regex UnpipeRegex = new Regex(@"\[\[([^\|\]]*)\|[^\]]*\]\]", RegexOptions.Compiled);

        public bool CanSave
        {
            get
            {
                return txtCorrection.Text.Trim() != "";
            }
        }

        /// <summary>
        /// most preparations are done here
        /// </summary>
        private void DabControl_Load(object sender, EventArgs e)
        {
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right ;

            // prepare variants
            cmboChoice.Items.Add("[no change]");
            cmboChoice.Items.Add("[unlink]");
            cmboChoice.Items.Add("{{dn}}");

            foreach (string s in Variants) cmboChoice.Items.Add(s);

            //find our paragraph
            for (posStart = Match.Index; posStart > 0; posStart--)
            {
                 if("\n\r".Contains(ArticleText[posStart] + ""))
                 {
                     posStart++;
                     break;
                 }
            }

            posEnd = Match.Index + Match.Value.Length;

            if (Match.Groups[2].Value == "") VisibleLink = Match.Groups[1].Value;
            else VisibleLink = Match.Groups[2].Value;
            VisibleLink = VisibleLink.TrimStart(new char[] { '|' });

            while (posEnd < ArticleText.Length - 1 && !"\n\r".Contains(ArticleText[posEnd] + "")) posEnd++;

            // find surroundings (~ ±ContextChars from link)
            int n = Match.Index - ContextChars;
            if (n < posStart) n = posStart;
            for (; n > posStart; n--)
            {
                if (char.IsSeparator(ArticleText[n]))
                {
                    n++;
                    break;
                }
            }
            SurroundingsStart = n;

            n = Match.Index + Match.Length + ContextChars;
            if (n > posEnd) n = posEnd;
            for (; n < posEnd; n++)
            {
                if (char.IsSeparator(ArticleText[n]))
                {
                    //n--;
                    break;
                }
            }
            Surroundings = ArticleText.Substring(SurroundingsStart, n - SurroundingsStart);
            PosInSurroundings = Surroundings.IndexOf(Match.Value);

            // check if the link is at the beginning of a sentence
            for (n = Match.Index - 1; n > posStart; --n)
            {
                if (ArticleText[n] == '.')
                {
                    StartOfSentence = true;
                    break;
                }
                if (!char.IsWhiteSpace(ArticleText[n])) break;
            }
            if(n == posStart) StartOfSentence = true;

            // prepare text boxes
            txtCorrection.Text = Surroundings;

            txtViewer.Text = ArticleText.Substring(posStart, posEnd - posStart);
            // highlight link to disambiguate
            txtViewer.Select(Match.Index - posStart, Match.Length);
            txtViewer.SelectionFont = new System.Drawing.Font(txtViewer.SelectionFont.FontFamily,
                txtViewer.SelectionFont.Size, System.Drawing.FontStyle.Bold);
            txtViewer.Select(0, 0);

            cmboChoice.SelectedIndex = 0;
            cmboChoice.Select();
        }

        private void ComboBoxChanged(int n)
        {
            if (n == 0) // no change
            {
                txtCorrection.Text = Surroundings;
                CurrentLink = Match.Value;
            }
            else if (n == 1) // unlink
            {
                txtCorrection.Text = Surroundings.Replace(Match.Value, VisibleLink);
                CurrentLink = VisibleLink;
            }
            else if (n == 2) // add {{dn}}
            {
                CurrentLink = Match.Value + "{{dn}}";
                if ((Surroundings.Length > PosInSurroundings + Match.Value.Length) && 
                    (char.IsPunctuation(Surroundings[PosInSurroundings + Match.Value.Length])))
                {
                    txtCorrection.Text = Surroundings.Insert(PosInSurroundings + Match.Value.Length + 1, "{{dn}}");
                }
                else
                    txtCorrection.Text = Surroundings.Replace(Match.Value, CurrentLink);
            }
            else
            {
                CurrentLink = "[[";
                if (StartOfSentence) CurrentLink += Tools.TurnFirstToUpper(Variants[n - 3]);
                else CurrentLink += Variants[n - 3];
                CurrentLink += "|" + VisibleLink + "]]";
                WikiFunctions.Parse.Parsers Parse = new WikiFunctions.Parse.Parsers();
                CurrentLink = Parse.StickyLinks(Parse.SimplifyLinks(CurrentLink));
                txtCorrection.Text = Surroundings.Replace(Match.Value, CurrentLink);
            }

            btnUnpipe.Enabled = CurrentLink.Contains("|");
            if (Changed != null) Changed(this, new EventArgs());
        }

        private void cmboChoice_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxChanged(cmboChoice.SelectedIndex);
        }

        /// <summary>
        /// sets disabmiguation back to 'no change' state
        /// </summary>
        public void Reset()
        {
            // to ensure that handler will be called
            if (cmboChoice.SelectedIndex != 0) cmboChoice.SelectedIndex = 0;
            else ComboBoxChanged(0);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Reset();
        }

        /// <summary>
        /// undoes all manual changes in edit box
        /// </summary>
        public void Undo()
        {
            ComboBoxChanged(cmboChoice.SelectedIndex);
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            Undo();
        }

        private void btnUnpipe_Click(object sender, EventArgs e)
        {
            string newLink = UnpipeRegex.Replace(CurrentLink, "[[$1]]");
            txtCorrection.Text = txtCorrection.Text.Replace(CurrentLink, newLink);
            CurrentLink = newLink;
            if (Changed != null) Changed(this, new EventArgs());
        }

        private void txtCorrection_TextChanged(object sender, EventArgs e)
        {
            if (Changed != null) Changed(this, new EventArgs());
        }
    }
}
