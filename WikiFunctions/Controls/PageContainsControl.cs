using System;
using System.Windows.Forms;

namespace WikiFunctions.Controls
{
    public partial class PageContainsControl : UserControl
    {
        IArticleComparer _comparer;

        public PageContainsControl()
        {
            InitializeComponent();
        }

        private void chkSkipIfContains_CheckedChanged(object sender, EventArgs e)
        {
            txtSkipIfContains.Enabled = chkSkipIfContains.Checked;
        }

        private void Invalidate_CheckedChanged(object sender, EventArgs e)
        {
            _comparer = null;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Enabled
        {
            get { return chkSkipIfContains.Checked; }
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
                _comparer = ArticleComparerFactory.Create(txtSkipIfContains.Text,
                    chkSkipContainsCaseSensitive.Checked,
                    chkSkipContainsIsRegex.Checked,
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
            get { return "Page contains: " + txtSkipIfContains.Text; }
        }
    }
}
