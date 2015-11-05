using System;
using System.Windows.Forms;

namespace WikiFunctions.Controls
{
    public partial class PageContainsControl : UserControl
    {
        private IArticleComparer _comparer;

        public PageContainsControl()
        {
            InitializeComponent();
        }

        private void chkSkipIfContains_CheckedChanged(object sender, EventArgs e)
        {
            txtContains.Enabled = chkContains.Checked;
        }

        private void txtContains_TextChanged(object sender, EventArgs e)
        {
            txtContains.ResetFormatting();
        }

        private void InvalidateComparer(object sender, EventArgs e)
        {
            _comparer = null;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool SkipEnabled
        {
            get { return chkContains.Checked; }
            set { chkContains.Checked = value; }
        }

        public string SkipText
        {
            get { return txtContains.Text; }
            set { txtContains.Text = value; }
        }

        public bool IsRegex
        {
            get { return chkIsRegex.Checked; }
            set { chkIsRegex.Checked = value; }
        }

        public bool IsCaseSensitive
        {
            get { return chkCaseSensitive.Checked; }
            set { chkCaseSensitive.Checked = value; }
        }

        public bool After
        {
            get { return chkAfterProcessing.Checked; }
            set { chkAfterProcessing.Checked = value;  }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="article"></param>
        /// <returns></returns>
        public bool Matches(Article article)
        {
            if (_comparer == null)
            {
                _comparer = ArticleComparerFactory.Create(txtContains.Text,
                    chkCaseSensitive.Checked,
                    chkIsRegex.Checked,
                    false, // singleline
                    false // multiline
                    );
            }
            return _comparer.Matches(article);
        }

        /// <summary>
        /// 
        /// </summary>
        public string SkipReason
        {
            get { return "Page contains: " + txtContains.Text; }
        }


    }
}
