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
using System.Collections.Generic;
using System.Windows.Forms;

namespace WikiFunctions.Controls
{
    public partial class NamespacesControl : UserControl
    {
        public NamespacesControl()
        {
            InitializeComponent();
            Populate();
        }

        private void chkContents_CheckedChanged(object sender, EventArgs e)
        {
            checkedLBContent.BeginUpdate();

            for (int i = 0; i < checkedLBContent.Items.Count; i++)
            {
                checkedLBContent.SetItemChecked(i, chkContents.Checked);
            }

            checkedLBContent.EndUpdate();
        }

        private void chkTalk_CheckedChanged(object sender, EventArgs e)
        {
            checkedLBTalk.BeginUpdate();

            for (int i = 0; i < checkedLBTalk.Items.Count; i++)
            {
                checkedLBTalk.SetItemChecked(i, chkTalk.Checked);
            }

            checkedLBTalk.EndUpdate();
        }

        public void Populate()
        {
            checkedLBTalk.BeginUpdate();
            checkedLBContent.BeginUpdate();

            foreach (KeyValuePair<int, string> kvp in Variables.Namespaces)
            {
                if (Namespace.IsTalk(kvp.Key))
                    checkedLBTalk.Items.Add(new NSItem(kvp));
                else
                    checkedLBContent.Items.Add(new NSItem(kvp));
            }

            checkedLBTalk.EndUpdate();
            checkedLBContent.EndUpdate();
        }

        public void Reset()
        {
            SetSelectedNamespaces(new List<int>(new[] { 0 }));
        }

        public List<int> GetSelectedNamespaces()
        {
            List<int> ret = new List<int>();
            ret.AddRange(GetSelectedListTags(checkedLBContent));
            ret.AddRange(GetSelectedListTags(checkedLBTalk));
            ret.Sort();
            return ret;
        }

        private List<int> GetSelectedListTags(CheckedListBox clb)
        {
            List<int> ret = new List<int>();
            for (int i = 0; i < clb.Items.Count; i++)
            {
                if (clb.GetItemChecked(i))
                    ret.Add(((NSItem) clb.Items[i]).Key);
            }

            return ret;
        }

        public void SetSelectedNamespaces(List<int> tags)
        {
            SetListTags(checkedLBContent, tags);
            SetListTags(checkedLBTalk, tags);
        }

        private void SetListTags(CheckedListBox clb, ICollection<int> tags)
        {
            for (int i = 0; i < clb.Items.Count; i++)
            {
                clb.SetItemChecked(i, tags.Contains(((NSItem) clb.Items[i]).Key));
            }
        }
    }

    public sealed class NSItem
    {
        private readonly KeyValuePair<int, string> kvp;

        public NSItem(KeyValuePair<int, string> item)
        {
            kvp = item;
        }

        public int Key
        {
            get { return kvp.Key; }
        }

        public string Value
        {
            get { return kvp.Value; }
        }

        public override string ToString()
        {
            return kvp.Value;
        }
    }
}
