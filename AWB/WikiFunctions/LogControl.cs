/*
(C) 2007 Reedy Boy
(C) 2007 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/

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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.IO;
using System.Windows.Forms;

using WikiFunctions;
using WikiFunctions.Lists;
using WikiFunctions.Logging;

namespace WikiFunctions
{
    public partial class LogControl : UserControl
    {
        ListMaker listMaker;
        int sortColumn = -1;

        public LogControl()
        {
            InitializeComponent();
        }

        public void Initialise(ListMaker rlistMaker)
        {
            listMaker = rlistMaker;
            resizeListView(lvIgnored);
            resizeListView(lvSaved);
        }

        #region lvMenu
        private void mnuListView_Opening(object sender, CancelEventArgs e)
        {
            bool enabled = (MenuItemOwner(sender).SelectedItems.Count == 1);

            filterByReasonOfSelectedToolStripMenuItem.Enabled = ((lvIgnored.SelectedItems.Count == 1) && enabled);
            filterByReasonOfSelectedToolStripMenuItem.Visible = enabled;
            toolStripSeparator1.Visible = enabled;

            openInBrowserToolStripMenuItem.Enabled = enabled;
            openHistoryInBrowserToolStripMenuItem.Enabled = enabled;
        }

        private void addSelectedToArticleListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in MenuItemOwner(sender).SelectedItems)
            {
                listMaker.Add(new Article(item.Text));
            }
        }

        /// <summary>
        /// Returns the ListView object from which the menu item was clicked
        /// </summary>
        private ListView MenuItemOwner(object sender)
        {
            /* we seem to sometimes be receiving a ToolStripMenuItem, and sometimes a ContextMenuStrip...
             * I've no idea why, but in the meantime this version of the function handles both. */
            try
            {
                return ((ListView)((ContextMenuStrip)sender).SourceControl);
            }
            catch
            {
                try
                {
                    return (ListView)(((ContextMenuStrip)((ToolStripMenuItem)sender).Owner).SourceControl);
                }
                catch
                {
                    throw;
                }
            }
        }

        private void filterByReasonOfSelectedToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            /* Sam, I think it would be better if the filter menu item only appeared when the reason is
             * clicked, and showed the actual text... is that possible? */

            string filterBy = lvIgnored.SelectedItems[0].SubItems[3].Text;

            foreach (ListViewItem item in lvIgnored.Items)
            {
                if (string.CompareOrdinal(item.SubItems[3].Text, filterBy) != 0)
                    item.Remove();
            }
        }
        #endregion

        public void AddLog(bool Skipped, AWBLogListener LogListener)
        {
            if (Skipped)
            {
                LogListener.AddAndDateStamp(lvIgnored);
                resizeListView(lvIgnored);
            }
            else
            {
                LogListener.AddAndDateStamp(lvSaved);
                resizeListView(lvSaved);
            }
        }

        private void LogLists_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                ((AWBLogListener)((ListView)sender).FocusedItem).OpenInBrowser();
            }
            catch { }
        }

        private void resizeIgnored()
        {
            resizeListView(lvIgnored);
        }

        private void resizeSaved()
        {
            resizeListView(lvSaved);
        }

        private void resizeListView(ListView lstView)
        {
            int width; int width2;
            foreach (ColumnHeader head in lstView.Columns)
            {
                head.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
                width = head.Width;

                head.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                width2 = head.Width;

                if (width2 < width)
                    head.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
            }
        }

        private void SaveListView(ListView listview)
        {
            try
            {
                StringBuilder strList = new StringBuilder("");
                StreamWriter sw;
                string strListFile;
                if (saveListDialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (ListViewItem a in listview.Items)
                    {
                        string text = a.Text;
                        if (a.SubItems.Count > 0)
                        {
                            for (int i = 1; i < a.SubItems.Count; i++)
                            {
                                text += " " + a.SubItems[i].Text;
                            }
                        }
                        strList.AppendLine(text);
                    }
                    strListFile = saveListDialog.FileName;
                    sw = new StreamWriter(strListFile, false, Encoding.UTF8);
                    sw.Write(strList);
                    sw.Close();
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message, "File error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAddToList_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem article in lvIgnored.Items)
                listMaker.Add(new Article(article.Text));
        }

        private void btnSaveSaved_Click(object sender, EventArgs e)
        {
            SaveListView(lvSaved);
        }

        private void btnSaveIgnored_Click(object sender, EventArgs e)
        {
            SaveListView(lvIgnored);
        }

        private void btnClearSaved_Click(object sender, EventArgs e)
        {
            lvSaved.Items.Clear();
        }

        private void btnClearIgnored_Click(object sender, EventArgs e)
        {
            lvIgnored.Items.Clear();
        }

        #region ColumnSort
        private void lvSavedColumnSort(object sender, System.Windows.Forms.ColumnClickEventArgs e)
        {
            lvColumnSort(lvSaved, e);
        }

        private void lvIgnoredColumnSort(object sender, System.Windows.Forms.ColumnClickEventArgs e)
        {
            lvColumnSort(lvIgnored, e);
        }

        private void lvColumnSort(ListView listView, System.Windows.Forms.ColumnClickEventArgs e)
        {
            // Determine whether the column is the same as the last column clicked.
            if (e.Column != sortColumn)
            {
                // Set the sort column to the new column.
                sortColumn = e.Column;
                // Set the sort order to ascending by default.
                listView.Sorting = SortOrder.Ascending;
            }
            else
            {
                // Determine what the last sort order was and change it.
                if (listView.Sorting == SortOrder.Ascending)
                    listView.Sorting = SortOrder.Descending;
                else
                    listView.Sorting = SortOrder.Ascending;
            }

            // Call the sort method to manually sort.
            listView.Sort();
            // Set the ListViewItemSorter property to a new ListViewItemComparer
            // object.
            listView.ListViewItemSorter = new ListViewItemComparer(e.Column,
                                                              listView.Sorting);
        }
        #endregion

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ctrl_c(sender);
            removeselected(sender);
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ctrl_c(sender);
        }

        private void ctrl_c(object sender)
        {
            string ClipboardData = "";

            foreach (ListViewItem a in MenuItemOwner(sender).SelectedItems)
            {
                string text = a.Text;
                if (a.SubItems.Count > 0)
                {
                    for (int i = 1; i < a.SubItems.Count; i++)
                    {
                        text += " " + a.SubItems[i].Text;
                    }
                }
                if (ClipboardData != "") ClipboardData += "\r\n";
                
                ClipboardData += text;
            }

            Clipboard.SetDataObject(ClipboardData, true);
        }

        private void removeselected(object sender)
        {
            foreach (ListViewItem a in MenuItemOwner(sender).SelectedItems)
            {
                a.Remove();
            }
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in MenuItemOwner(sender).Items)
            {
                item.Selected = true;
            }
        }

        private void selectNoneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in MenuItemOwner(sender).Items)
            {
                item.Selected = false;
            }
        }

        private void openInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Variables.URL + "/wiki/" + MenuItemOwner(sender).SelectedItems[0].Text);
        }

        private void openHistoryInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Variables.URL + "/wiki/" + MenuItemOwner(sender).SelectedItems[0].Text + "&action=history"); 
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            removeselected(sender);
        }
    }
}
