/*
(C) 2007 Sam Reed, Stephen Kennedy (Kingboyk) http://www.sdk-software.com/

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
using System.Text;
using System.Windows.Forms;

using WikiFunctions.Controls;
using WikiFunctions.Controls.Lists;

namespace WikiFunctions.Logging
{
    public partial class LogControl : UserControl
    {
        protected ListMaker listMaker;
        protected readonly List<AWBLogListener> FilteredItems = new List<AWBLogListener>();

        #region Public
        public LogControl()
        {
            InitializeComponent();
        }

        public void Initialise(ListMaker rlistMaker)
        {
            listMaker = rlistMaker;
            ResizeListView(lvIgnored);
            ResizeListView(lvSaved);
        }

        public delegate void LogAddedToControl(bool skipped, AWBLogListener logListener);
        public event LogAddedToControl LogAdded;

        public void AddLog(bool skipped, AWBLogListener logListener)
        {
            if (skipped)
            {
                logListener.AddAndDateStamp(lvIgnored);
                ResizeListView(lvIgnored);
            }
            else
            {
                logListener.AddAndDateStamp(lvSaved);
                ResizeListView(lvSaved);
            }

            if (LogAdded != null)
                LogAdded(skipped, logListener);
        }
        #endregion

        #region Private/Protected
        private static AWBLogListener SelectedItem(object sender)
        { return ((AWBLogListener)MenuItemOwner(sender).SelectedItems[0]); }

        /// <summary>
        /// Returns the ListView object from which the menu item was clicked
        /// </summary>
        /// 
        private static ListView MenuItemOwner(object sender)
        {
            /* we seem to sometimes be receiving a ToolStripMenuItem, and sometimes a ContextMenuStrip...
             * I've no idea why, but in the meantime this version of the function handles both. */

            if (sender is ContextMenuStrip)
                return ((ListView)((ContextMenuStrip)sender).SourceControl);
            
            if (sender is ToolStripMenuItem)
                return (ListView)(((ContextMenuStrip)((ToolStripMenuItem)sender).Owner).SourceControl);
            throw new ArgumentException("Object of unknown type passed to LogControl.MenuItemOwner()", "sender");
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
                string skipReason = SelectedItem(sender).SkipReason;
                filterShowOnlySelectedToolStripMenuItem.Enabled = true;
                filterShowOnlySelectedToolStripMenuItem.Text = "Filter by skip reason: " + skipReason;
                filterExcludeToolStripMenuItem.Enabled = true;
                filterExcludeToolStripMenuItem.Text = "Filter exclude skip reason: " + skipReason;
            }

            filterShowOnlySelectedToolStripMenuItem.Visible = enabled;
            filterExcludeToolStripMenuItem.Visible = enabled;
            toolStripSeparator1.Visible = enabled;
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

        private static void ResizeListView(NoFlickerExtendedListView lstView)
        {
            lstView.ResizeColumns(true);
        }

        private void SaveListView(ListView listview)
        {
            LogFileType logFileType = GetFilePrefs();
            if (logFileType != 0)
            {
                StringBuilder strList = new StringBuilder();

                foreach (AWBLogListener a in listview.Items)
                {
                    strList.AppendLine(a.Output(logFileType));
                }
                Tools.WriteTextFileAbsolutePath(strList.ToString(), saveListDialog.FileName, false);
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
            FilteredItems.Clear();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tools.Copy(MenuItemOwner(sender));
            RemoveSelected(sender);
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tools.Copy(MenuItemOwner(sender));
        }

        private static void RemoveSelected(object sender)
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
            foreach (AWBLogListener item in MenuItemOwner(sender).SelectedItems)
            {
                item.OpenInBrowser();
            }
        }

        private void openHistoryInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (AWBLogListener item in MenuItemOwner(sender).SelectedItems)
            {
                item.OpenHistoryInBrowser();
            }
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveSelected(sender);
        }

        private void filterShowOnlySelectedToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            string filterBy = Filter;

            foreach (AWBLogListener item in lvIgnored.Items)
            {
                if (string.Compare(item.SkipReason, filterBy, true) != 0) // no match
                {
                    FilteredItems.Add(item);
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
                    FilteredItems.Add(item);
                    item.Remove();
                }
            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MenuItemOwner(sender) == lvIgnored)
            {
                lvIgnored.Items.Clear();
                FilteredItems.Clear();
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
                foreach (AWBLogListener log in FilteredItems)
                { // AddRange accepts only an array or a ListViewItemCollection (the latter is only creatable by passing a listview object to the creator)
                    lvIgnored.Items.Add(log);
                }
                lvIgnored.Sorting = SortOrder.None;
                FilteredItems.Clear();
            }
            else
            {
                lvSaved.Sorting = SortOrder.None;
            }
        }
        #endregion

        private void btnAddSuccessToList_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem article in lvSaved.Items)
                listMaker.Add(new Article(article.Text));
        }
    }
}
