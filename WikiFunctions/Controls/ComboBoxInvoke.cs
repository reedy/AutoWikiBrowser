using System;
using System.Windows.Forms;

namespace WikiFunctions.Controls
{
    public class ComboBoxInvoke : ComboBox
    {
        public override int SelectedIndex
        {
            get
            {
                if (!InvokeRequired)
                {
                    return base.SelectedIndex;
                }

                return (int)Invoke(new Func<int>(() => SelectedIndex));
            }
            set { base.SelectedIndex = value; }
        }
    }
}
