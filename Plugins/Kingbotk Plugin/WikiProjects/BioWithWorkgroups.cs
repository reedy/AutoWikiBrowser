/*
Copyright (C) 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/
Copyright (C) 2008 Sam Reed (Reedy) http://www.reedyboy.net/

This program is free software; you can redistribute it and/or modify it under the terms of Version 2 of the GNU General Public License as published by the Free Software Foundation.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License Version 2 along with this program; if not, write to the Free Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/

using System.Windows.Forms;

namespace AutoWikiBrowser.Plugins.Kingbotk.WikiProjects
{
    internal class BioWithWorkgroups : GenericWithWorkgroups
    {
        public BioWithWorkgroups(string template, string prefix, bool autoStubEnabled,
            params TemplateParameters[] @params)
            : base(template, prefix, autoStubEnabled, @params)
        {
            ListView1.ItemChecked += ListView1_ItemChecked;
        }

        private void ListView1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            // HACK:For some reason, during the setup phases, the items are there, but aren't assigned. And are null
            if (ListView1.Items[ListView1.Items.Count - 1] != null)
            {
                if (e.Item.Text == "Living" && e.Item.Checked)
                {
                    ListViewItem lvi = ListView1.FindItemWithText("Not Living");

                    if (lvi != null && lvi.Checked)
                    {
                        lvi.Checked = false;
                    }
                }
                else if (e.Item.Text == "Not Living" && e.Item.Checked)
                {
                    ListViewItem lvi = ListView1.FindItemWithText("Living");

                    if (lvi != null && lvi.Checked)
                    {
                        lvi.Checked = false;
                    }
                }
            }
        }
    }
}