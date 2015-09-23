using System;
using System.Windows.Forms;

namespace WikiFunctions.Controls
{
    public class RichTextBoxInvoke : RichTextBox
    {
        public override string Text
        {
            get
            {
                if (!InvokeRequired)
                {
                    return base.Text;
                }

                return (string)Invoke(new Func<string>(() => Text));
            }
            set { base.Text = value; }
        }
    }
}
