using System;
using System.Windows.Forms;

namespace WikiFunctions.Controls
{
    public partial class EditProtectControl : UserControl
    {
        public event EventHandler TextBoxIndexChanged;

        public EditProtectControl()
        {
            InitializeComponent();
            lbMove.SelectedIndex = 0;
            lbEdit.SelectedIndex = 0;
        }

        private void chkUnlock_CheckedChanged(object sender, EventArgs e)
        {
            lbMove.Enabled = chkUnlock.Checked;
        }

        private void lbEdit_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!chkUnlock.Checked)
                lbMove.SelectedIndex = lbEdit.SelectedIndex;
        }

        private void BothListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TextBoxIndexChanged != null)
                TextBoxIndexChanged(this, e);
        }

        public bool CascadingEnabled
        {
            get { return ((lbEdit.SelectedIndex == 2) && (lbMove.SelectedIndex == 2)); }
        }

        public int EditProtectionLevel
        {
            get { return lbEdit.SelectedIndex; }
        }

        public int MoveProtectionLevel
        {
            get { return lbMove.SelectedIndex; }
        }

        public bool Visibility
        {
            set { lbEdit.Visible = lbMove.Visible = lblEdit.Visible = lblMove.Visible = chkUnlock.Visible = value; }
        }

        public void Reset()
        {
            lbEdit.SelectedIndex = 0;
            lbMove.SelectedIndex = 0;
            chkUnlock.Checked = false;
        }
    }
}
