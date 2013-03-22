using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace WikiFunctions.Controls
{
    /// <summary>
    /// Wrapped EditBox to conveniently manage the automatic summary reset conditions
    /// </summary>
    public class ArticleTextBox : TextBox
    {
        public ArticleTextBox() : base()
        {
        }

        bool Locked = false;

        public override string Text
        {
            get { return base.Text; }
            set
            {
                Locked = true;
                base.Text = value;
                Locked = false;
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            // Prohibits triggering the TextChanged event if the text is changed programmatically
            if (!Locked) base.OnTextChanged(e);
        }
    }
}
