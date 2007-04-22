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

namespace WikiFunctions.Logging
{
    public partial class LogControl : UserControl
    {
        protected ListMaker listMaker;
        protected int sortColumn = -1;
        protected List<AWBLogListener> mFilteredItems = new List<AWBLogListener>();

        #region Public
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
        #endregion

        #region Private/Protected
            private AWBLogListener SelectedItem(object sender)
            { return ((AWBLogListener)MenuItemOwner(sender).SelectedItems[0]); }

            /// <summary>
            /// Returns the ListView object from which the menu item was clicked
            /// </summary>
            /// 
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
                    { return (ListView)(((ContextMenuStrip)((ToolStripMenuItem)sender).Owner).SourceControl); }
                    catch
                    { throw; }
                }
            }

            private LogFileType GetFilePrefs()
            {
                if (saveListDialog.ShowDialog() != DialogResult.OK)
                    return 0;
                return (LogFileType)saveListDialog.FilterIndex;
            }

            private string Filter
            {
                get { return ((AWBLogListener)lvIgnored.SelectedItems[0]).SkipReason; }
            }
        #endregion

        #region Event Handlers
            private void mnuListView_Opening(object sender, CancelEventArgs e)
            {
                bool enabled = ((MenuItemOwner(sender) == lvIgnored) && lvIgnored.SelectedItems.Count == 1);

                if (enabled)
                {
                    string SkipReason = SelectedItem(sender).SkipReason;
                    filterShowOnlySelectedToolStripMenuItem.Enabled = true;
                    filterShowOnlySelectedToolStripMenuItem.Text = "Filter by skip reason: " + SkipReason;
                    filterExcludeToolStripMenuItem.Enabled = true;
                    filterExcludeToolStripMenuItem.Text = "Filter exclude skip reason: " + SkipReason;
                }

                filterShowOnlySelectedToolStripMenuItem.Visible = enabled;
                filterExcludeToolStripMenuItem.Visible = enabled;
                toolStripSeparator1.Visible = enabled;
                openHistoryInBrowserToolStripMenuItem.Enabled = enabled;
            }
        
            private void addSelectedToArticleListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in MenuItemOwner(sender).SelectedItems)
            {
                listMaker.Add(new Article(item.Text));
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
                lstView.BeginUpdate();
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
                lstView.EndUpdate();
            }

            private void SaveListView(ListView listview)
            {
                try
                {
                    LogFileType LogFileType = GetFilePrefs();
                    if (LogFileType != 0)
                    {
                        StringBuilder strList = new StringBuilder("");
                        StreamWriter sw;
                        string strListFile;
                        foreach (AWBLogListener a in listview.Items)
                        {
                            strList.AppendLine(a.Output(LogFileType));
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
            mFilteredItems.Clear();
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
            try
            {
                listView.BeginUpdate();
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
                listView.EndUpdate();
            }
            catch { }
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
                // I think we only need copy the file list
                /* if (a.SubItems.Count > 0)
                {
                    for (int i = 1; i < a.SubItems.Count; i++)
                    {
                        text += " " + a.SubItems[i].Text;
                    }
                } */
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
            { item.Selected = true; }
        }

        private void selectNoneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in MenuItemOwner(sender).Items)
            { item.Selected = false; }
        }

        private void openInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (AWBLogListener item in MenuItemOwner(sender).SelectedItems) { item.OpenInBrowser(); }
        }

        private void openHistoryInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectedItem(sender).OpenHistoryInBrowser();
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            removeselected(sender);
        }
        
        private void filterShowOnlySelectedToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            string filterBy = Filter;

            foreach (AWBLogListener item in lvIgnored.Items)
            {
                if (string.Compare(item.SkipReason, filterBy, true) != 0) // no match
                {
                    mFilteredItems.Add(item);
                    item.Remove();
                }
            }
        }

        private void filterExcludeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filterBy = Filter;

            foreach (AWBLogListener item in lvIgnored.Items)
            {
                if (string.Compare(item.SkipReason, filterBy, true) == 0) // match
                {
                    mFilteredItems.Add(item);
                    item.Remove();
                }
            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MenuItemOwner(sender) == lvIgnored)
            {
                lvIgnored.Items.Clear();
                mFilteredItems.Clear();
            }
            else
            {
                lvSaved.Items.Clear();
            }
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MenuItemOwner(sender) == lvIgnored)
            {
                foreach (AWBLogListener log in mFilteredItems)
                { // AddRange accepts only an array or a ListViewItemCollection (the latter is only creatable by passing a listview object to the creator)
                    lvIgnored.Items.Add(log);
                }
                lvIgnored.Sorting = SortOrder.None;
                mFilteredItems.Clear();
            }
            else
            {
                lvSaved.Sorting = SortOrder.None;
            }
        }

        #endregion
    }
}
