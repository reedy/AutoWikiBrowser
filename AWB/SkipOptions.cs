using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using WikiFunctions;

namespace AutoWikiBrowser
{
    public partial class SkipOptions : Form
    {
        public SkipOptions()
        {
            InitializeComponent();
        }

        #region Properties

        public bool SkipNoUnicode
        {
            get { return rdoNoUnicode.Checked; }
        }

        public bool SkipNoTag
        {
            get { return rdoNoTag.Checked; }
        }

        public bool SkipNoHeaderError
        {
            get { return rdoNoHeaderError.Checked; }
        }

        public bool SkipNoBoldTitle
        {
            get { return rdoNoBoldTitle.Checked; }
        }

        public bool SkipNoBulletedLink
        {
            get { return rdoNoBulletedLink.Checked; }
        }

        public bool SkipNoBadLink
        {
            get { return rdoNoBadLink.Checked; }
        }

        #endregion

        #region Methods

        public bool skipIf(string articleText)
        {//custom code to skip articles can be added here
            return true;

        }

        public bool SkipIfContains(string ArticleText, string ArticleTitle, string strFind, bool Regexe, bool caseSensitive, bool DoesContain)
        {
            if (strFind.Length > 0)
            {
                RegexOptions RegOptions;

                if (caseSensitive)
                    RegOptions = RegexOptions.None;
                else
                    RegOptions = RegexOptions.IgnoreCase;

                strFind = Tools.ApplyKeyWords(ArticleTitle, strFind);

                if (!Regexe)
                    strFind = Regex.Escape(strFind);

                if (Regex.IsMatch(ArticleText, strFind, RegOptions))
                    return DoesContain;
                else
                    return !DoesContain;
            }
            else
                return false;
        }

        private void SkipOptions_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        public string SelectedItem
        {
            get
            {
                foreach (RadioButton rd in gbOptions.Controls)
                {
                    if (rd.Checked)
                    {
                        return rd.Tag.ToString();
                    }
                }

                return "0";
            }
            set
            {
                foreach (RadioButton rd in gbOptions.Controls)
                {
                    if (rd.Tag.ToString() == value)
                    {
                        rd.Checked = true;
                        return;
                    }
                }
                rdoNone.Checked = true;
            }
        }

        #endregion

    }
}