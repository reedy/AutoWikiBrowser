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
            txtContains.TextChanged += txtContains_TextChanged;
        }

        private void chkSkipIfContains_CheckedChanged(object sender, EventArgs e)
        {
            // TODO: This feels weird
            CheckEnabled = chkContains.Checked;
        }

        private void txtContains_TextChanged(object sender, EventArgs e)
        {
            // disable TextChanged temporarily under Mono otherwise get infinite loop
            if(Globals.UsingMono)
                txtContains.TextChanged -= txtContains_TextChanged;

            txtContains.ResetFormatting();

            if(Globals.UsingMono)
                txtContains.TextChanged += txtContains_TextChanged;
        }

        private void InvalidateComparer(object sender, EventArgs e)
        {
            _comparer = null;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool CheckEnabled
        {
            get { return chkContains.Checked; }
            set
            {
                txtContains.Enabled = value;
                chkIsRegex.Enabled = value;
                chkCaseSensitive.Enabled = value;
                chkAfterProcessing.Enabled = value;
                chkContains.Checked = value;
            }
        }

        public string CheckText
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

        /// <summary>
        /// Whether to check before or after processing
        /// </summary>
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
        public virtual bool Matches(Article article)
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
        public virtual string SkipReason
        {
            get { return "Page contains: " + txtContains.Text; }
        }
    }
}
