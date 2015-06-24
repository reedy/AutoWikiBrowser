using System;

namespace WikiFunctions.Controls
{
    /// <summary>
    /// Extends ToolTip to implement custom Style to prevent tooltips changing window focus under Wine
    /// </summary>
    public class AWBToolTip : System.Windows.Forms.ToolTip
    {
        // default constructors
        public AWBToolTip()
        {
        }

        public AWBToolTip(System.ComponentModel.IContainer Cont)
        {
        }

        // For Wine compatibility must set Style to add WS_POPUP
        // and ExStyle to add WS_EX_NOACTIVATE and WS_EX_TOOLWINDOW
        // Otherwise by default a popup changes window focus under Wine (probably wine bug https://bugs.winehq.org/show_bug.cgi?id=9512)
        private const int WS_EX_TOOLWINDOW = 0x00000080, WS_EX_NOACTIVATE = 0x08000000;

        protected override System.Windows.Forms.CreateParams CreateParams
        {
            get
            {
                System.Windows.Forms.CreateParams cp = base.CreateParams;
                unchecked
                {
                    cp.Style |= (int)0x80000000; // WS_POPUP

                    cp.ExStyle |= (WS_EX_NOACTIVATE | WS_EX_TOOLWINDOW);
                }
                return cp;
            }
        }
    }
}
