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
    /// <summary>
    /// 
    /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        public void Populate()
        {
            checkedLBTalk.BeginUpdate();
            checkedLBContent.BeginUpdate();

            checkedLBTalk.Items.Clear();
            checkedLBContent.Items.Clear();

            checkedLBContent.Items.Add(new NSItem(new KeyValuePair<int,string>(0, "Content")));

            foreach (KeyValuePair<int, string> kvp in Variables.Namespaces)
            {
                if (kvp.Key < 0)
                    continue;

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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<int> GetSelectedNamespaces()
        {
            List<int> ret = new List<int>();
            ret.AddRange(GetSelectedNamespaces(checkedLBContent));
            ret.AddRange(GetSelectedNamespaces(checkedLBTalk));
            ret.Sort();
            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clb"></param>
        /// <returns></returns>
        private static IEnumerable<int> GetSelectedNamespaces(CheckedListBox clb)
        {
            List<int> ret = new List<int>();
            for (int i = 0; i < clb.Items.Count; i++)
            {
                if (clb.GetItemChecked(i))
                    ret.Add(((NSItem) clb.Items[i]).Key);
            }

            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tags"></param>
        public void SetSelectedNamespaces(ICollection<int> tags)
        {
            SetSelectedNamespaces(checkedLBContent, tags);
            SetSelectedNamespaces(checkedLBTalk, tags);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clb"></param>
        /// <param name="tags"></param>
        private static void SetSelectedNamespaces(CheckedListBox clb, ICollection<int> tags)
        {
            for (int i = 0; i < clb.Items.Count; i++)
            {
                clb.SetItemChecked(i, tags.Contains(((NSItem) clb.Items[i]).Key));
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class NSItem
    {
        private readonly KeyValuePair<int, string> kvp;

        public NSItem(KeyValuePair<int, string> item)
        {
            kvp = item;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Key
        {
            get { return kvp.Key; }
        }

        /// <summary>
        /// 
        /// </summary>
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
