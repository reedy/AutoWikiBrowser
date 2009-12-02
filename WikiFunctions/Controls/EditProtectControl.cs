/*

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/

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
                if (DesignMode) return;
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
                if (DesignMode) return;
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

        private static string GetProtectionLevel(ListBox lb)
        {
            var prot = lb.SelectedItem as ProtectionLevel;
            return (prot != null) ? prot.Group : "";
        }

        private void EnsureProtectionLevelExists(string group)
        {
            EnsureProtectionLevelExists(group, lbEdit);
            EnsureProtectionLevelExists(group, lbMove);
        }

        private static void EnsureProtectionLevelExists(string group, ListBox lb)
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
            return string.IsNullOrEmpty(Display) ? "" : Display;
        }

        public override bool Equals(object obj)
        {
            if (obj is ProtectionLevel)
                return (obj as ProtectionLevel).Group == Group;
            if (obj is string)
                return Group == (string)obj;
            return false;
        }

        public override int GetHashCode()
        {
            return Group.GetHashCode();
        }

        public static readonly ProtectionLevel[] Levels = new[]
            {
                new ProtectionLevel("", "Unprotected"),
                new ProtectionLevel("autoconfirmed", "Semi-protected"),
                new ProtectionLevel("sysop", "Fully protected")
            };
    }
}
