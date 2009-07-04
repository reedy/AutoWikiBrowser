using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace WikiFunctions.Controls
{
    /// <summary>
    /// Wrapped EditBox to conveniently manage the automatic summary reset conditions
    /// </summary>
    public class ArticleTextBox : RichTextBox
    {
        public ArticleTextBox()
        {
            LanguageOption = RichTextBoxLanguageOptions.DualFont;
            InitializeComponent();
        }

        bool Locked;

        public override string Text
        {
            get { return base.Text.Replace("\n", "\r\n"); }
            set
            {
                Locked = true;
                base.Text = value;
                Locked = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string RawText { get { return base.Text; } }

        protected override void OnTextChanged(EventArgs e)
        {
            // Prohibits triggering the TextChanged event if the text is changed programmatically
            if (!Locked) base.OnTextChanged(e);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            //Bug fix for AutoWordSelection - http://msdn.microsoft.com/en-us/library/system.windows.forms.richtextbox.autowordselection.aspx
            if (!AutoWordSelection)
            {
                AutoWordSelection = true;
                AutoWordSelection = false;
            }
        }

        private Regex RegexObj;
        private Match MatchObj;

        /// <summary>
        /// Resets the Find Objects
        /// </summary>
        public void ResetFind()
        {
            RegexObj = null;
            MatchObj = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strRegex"></param>
        /// <param name="isRegex"></param>
        /// <param name="caseSensitive"></param>
        /// <param name="articleName"></param>
        public void Find(string strRegex, bool isRegex, bool caseSensitive, string articleName)
        {
            string articleText = RawText;

            RegexOptions regOptions = caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;

            strRegex = Tools.ApplyKeyWords(articleName, strRegex);

            if (!isRegex)
                strRegex = Regex.Escape(strRegex);

            if (MatchObj == null || RegexObj == null)
            {
                int findStart = SelectionStart;

                RegexObj = new Regex(strRegex, regOptions);
                MatchObj = RegexObj.Match(articleText, findStart);
                SelectionStart = MatchObj.Index;
                SelectionLength = MatchObj.Length;
            }
            else
            {
                if (MatchObj.NextMatch().Success)
                {
                    MatchObj = MatchObj.NextMatch();
                    SelectionStart = MatchObj.Index;
                    SelectionLength = MatchObj.Length;
                }
                else
                {
                    SelectionStart = 0;
                    SelectionLength = 0;
                    ResetFind();
                }
            }
            Focus();
            ScrollToCaret();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputIndex"></param>
        /// <param name="inputLength"></param>
        public void SetEditBoxSelection(int inputIndex, int inputLength)
        {
            if (inputIndex >= 0 && inputLength > 0 && (inputIndex + inputLength) <= TextLength)
            {
                SelectionStart = inputIndex;
                SelectionLength = inputLength;
            }
            ScrollToCaret();
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            DetectUrls = false;
            ResumeLayout(false);
        }
    }
}
