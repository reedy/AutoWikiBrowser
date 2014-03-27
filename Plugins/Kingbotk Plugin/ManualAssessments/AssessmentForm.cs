using System;
using System.Windows.Forms;

//Copyright © 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/
//Copyright © 2008 Sam Reed (Reedy) http://www.reedyboy.net/

//This program is free software; you can redistribute it and/or modify it under the terms of Version 2 of the GNU General Public License as published by the Free Software Foundation.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

//You should have received a copy of the GNU General Public License Version 2 along with this program; if not, write to the Free Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

namespace AutoWikiBrowser.Plugins.Kingbotk.ManualAssessments
{
    internal sealed partial class AssessmentForm
    {
        internal static void AllowOnlyOneCheckedItem(object sender, ItemCheckEventArgs e)
        {
            CheckedListBox listBox = (CheckedListBox) sender;

            if (e.NewValue == CheckState.Checked)
            {
                foreach (int i in listBox.CheckedIndices)
                {
                    listBox.SetItemChecked(i, false);
                }
            }
        }

        internal DialogResult ShowDialog(ref Classification classification, ref Importance importance, ref bool infobox,
            ref bool attention, ref bool needsPhoto, string title)
        {
            Text += ": " + title;

            var ret = ShowDialog();
            if (ClassCheckedListBox.SelectedIndices.Count == 0)
            {
                classification = Classification.Unassessed;
            }
            else
            {
                classification = (Classification) ClassCheckedListBox.SelectedIndex;
            }
            if (ImportanceCheckedListBox.SelectedIndices.Count == 0)
            {
                importance = Importance.Unassessed;
            }
            else
            {
                importance = (Importance) ImportanceCheckedListBox.SelectedIndex;
            }
            infobox = (SettingsCheckedListBox.GetItemCheckState(0) == CheckState.Checked);
            attention = (SettingsCheckedListBox.GetItemCheckState(1) == CheckState.Checked);
            needsPhoto = (SettingsCheckedListBox.GetItemCheckState(2) == CheckState.Checked);

            return ret;
        }

        // Button event handlers:
        private void OK_Button_Click(Object sender, EventArgs e)
        {
            Close();
        }

        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            Close();
        }

        public AssessmentForm()
        {
            InitializeComponent();
        }
    }
}