using System;
using System.Windows.Forms;
using System.ComponentModel;

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

            lbEdit.Items.Clear();
            lbMove.Items.Clear();

            foreach (var p in ProtectionLevel.Levels)
            {
                lbEdit.Items.Add(p);
                lbMove.Items.Add(p);
            }
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

        [Browsable(false)]
        public bool CascadingEnabled
        {
            get { return ((lbEdit.SelectedIndex == 2) && (lbMove.SelectedIndex == 2)); }
        }

        [Browsable(false)]
        public string EditProtectionLevel
        {
            get 
            {
                return GetProtectionLevel(lbEdit);
            }
            set
            {
                EnsureProtectionLevelExists(value);
                lbEdit.SelectedItem = value;
            }
        }

        [Browsable(false)]
        public string MoveProtectionLevel
        {
            get
            {
                return GetProtectionLevel(lbMove);
            }
            set
            {
                EnsureProtectionLevelExists(value);
                lbMove.SelectedItem = value;
            }
        }

        [Browsable(false)]
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

        private string GetProtectionLevel(ListBox lb)
        {
            var prot = lb.SelectedItem as ProtectionLevel;
            return prot.Group;
        }

        private void EnsureProtectionLevelExists(string group)
        {
            EnsureProtectionLevelExists(group, lbEdit);
            EnsureProtectionLevelExists(group, lbMove);
        }

        private void EnsureProtectionLevelExists(string group, ListBox lb)
        {
            ProtectionLevel p = new ProtectionLevel(group, group);
            if (!lb.Items.Contains(p)) lb.Items.Add(p);
        }
    }


    internal class ProtectionLevel
    {
        public readonly string Group;
        public readonly string Display;

        public ProtectionLevel(string group, string display)
        {
            Group = group;
            Display = display;
        }

        public override string ToString()
        {
            return Display;
        }

        public override bool Equals(object obj)
        {
            if (obj is ProtectionLevel)
                return (obj as ProtectionLevel).Group == Group;
            else if (obj is string)
                return Group == (string)obj;
            else
                return false;

        }

        public override int GetHashCode()
        {
            return Group.GetHashCode();
        }

        public static ProtectionLevel[] Levels = new[]
            {
                new ProtectionLevel("", "Unprotected"),
                new ProtectionLevel("autoconfirmed", "Semi-protected"),
                new ProtectionLevel("sysop", "Fully protected")
            };
    }
}
