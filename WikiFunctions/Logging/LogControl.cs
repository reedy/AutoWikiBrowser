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
using System.Collections;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.Linq;

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

        /// <summary>
        /// Adds the item to the saved or skipped lists as appropriate
        /// Unless user has explicit sort on by column clicking, sorts log lists newest first
        /// </summary>
        /// <param name="skipped"></param>
        /// <param name="logListener"></param>
        public void AddLog(bool skipped, AWBLogListener logListener)
        {
            if (skipped)
            {
                logListener.AddAndDateStamp(lvIgnored);
                ResizeListView(lvIgnored);
                
                // sort descending (newest first) unless user has clicked columns to create custom sort order
                if(lvIgnored.Sorting == SortOrder.None)
                {
                    lvIgnored.ListViewItemSorter = new ListViewItemComparer();
                    lvIgnored.Sort();
                }
            }
            else
            {
                logListener.AddAndDateStamp(lvSaved);
                ResizeListView(lvSaved);
                
                 // sort descending (newest first) unless user has clicked columns to create custom sort order
                if(lvSaved.Sorting == SortOrder.None)
                {
                    lvSaved.ListViewItemSorter = new ListViewItemComparer();
                    lvSaved.Sort();
                }
            }

            if (LogAdded != null)
                LogAdded(skipped, logListener);
            RefreshButtonEnablement();
        }
        #endregion

        #region Private/Protected
        private static AWBLogListener SelectedItem(object sender)
        { return ((AWBLogListener)MenuItemOwner(sender).SelectedItems[0]); }

        /// <summary>
        /// Returns the ListView object from which the menu item was clicked
        /// Note: does not work when event triggered by keyboard shortcut as toolstrip not opened by keyboard shortcuts
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

        /// <summary>
        /// Returns the currently selected list view (saved or ignored)
        /// Works from context menu clicks or keyboard shortcuts
        /// </summary>
        /// <returns>The selected list view.</returns>
        private ListView CurrentlySelectedListView()
        {
            // ActiveControl may be null if context menu opened at same time as main form page load finishes
            if(this.ActiveControl != null)
                return (ListView)this.ActiveControl;

            // first fallback
            if(lvIgnored.SelectedItems.Count > 0)
                return lvIgnored;

            // final fallback
            return lvSaved;
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

            addSelectedToArticleListToolStripMenuItem.Enabled = cutToolStripMenuItem.Enabled = copyToolStripMenuItem.Enabled
                = removeToolStripMenuItem.Enabled = openInBrowserToolStripMenuItem.Enabled = openHistoryInBrowserToolStripMenuItem.Enabled
                = filterShowOnlySelectedToolStripMenuItem.Enabled = filterExcludeToolStripMenuItem.Enabled = 
                MenuItemOwner(sender).SelectedItems.Count > 0;

            selectAllToolStripMenuItem.Enabled = selectNoneToolStripMenuItem.Enabled = clearToolStripMenuItem.Enabled =
                MenuItemOwner(sender).Items.Count > 0;
            
            // diff option for saved pages only
            openDiffInBrowserToolStripMenuItem.Enabled = MenuItemOwner(sender) == lvSaved && MenuItemOwner(sender).SelectedItems.Count > 0;
        }

        private void addSelectedToArticleListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<Article> articles = MenuItemOwner(sender).SelectedItems.Cast<ListViewItem>().Select(item => new Article(item.Text)).ToList();

            listMaker.Add(articles);
        }

        private void LogLists_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                ((AWBLogListener)((ListView)sender).FocusedItem).OpenInBrowser();
            }
            catch { }
        }

        /// <summary>
        /// Disables the Clear / Save log / Add all to list buttons when no articles in log, enables when 1 or more articles.
        /// Updates the skipped/saved count labels, refreshes column sizing
        /// </summary>
        private void RefreshButtonEnablement()
        {
            btnClearSaved.Enabled = btnSaveSaved.Enabled = btnAddSuccessToList.Enabled = lvSaved.Items.Count > 0;
            btnClearIgnored.Enabled = btnSaveIgnored.Enabled = btnAddSkippedToList.Enabled = lvIgnored.Items.Count > 0;

            this.label8.Text = "Skipped: " + lvIgnored.Items.Count;
            this.label7.Text = "Successfully saved: " + lvSaved.Items.Count;

            ResizeListView(lvIgnored);
            ResizeListView(lvSaved);
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
            List<Article> articles = lvIgnored.Items.Cast<ListViewItem>().Select(item => new Article(item.Text)).ToList();

            listMaker.Add(articles);
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
            RefreshButtonEnablement();
        }

        private void btnClearIgnored_Click(object sender, EventArgs e)
        {
            lvIgnored.Items.Clear();
            FilteredItems.Clear();
            RefreshButtonEnablement();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tools.Copy(CurrentlySelectedListView());
            RemoveSelected(CurrentlySelectedListView());
            RefreshButtonEnablement();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tools.Copy(CurrentlySelectedListView());
        }

        private static void RemoveSelected(ListView lv)
        {
            foreach (ListViewItem a in lv.SelectedItems)
            {
                a.Remove();
            }
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in CurrentlySelectedListView().Items)
            {
                item.Selected = true;
            }
        }

        private void selectNoneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentlySelectedListView().SelectedItems.Clear();
        }

        private void openInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (AWBLogListener item in CurrentlySelectedListView().SelectedItems)
            {
                item.OpenInBrowser();
            }
        }

        private void openHistoryInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (AWBLogListener item in CurrentlySelectedListView().SelectedItems)
            {
                item.OpenHistoryInBrowser();
            }
        }

        private void openDiffInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (AWBLogListener item in CurrentlySelectedListView().SelectedItems)
            {
                item.OpenDiffInBrowser();
            }
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveSelected(CurrentlySelectedListView());
            RefreshButtonEnablement();
        }

        private void FilterItems(bool compareMatch)
        {
            string filterBy = Filter;

            lvIgnored.BeginUpdate();
            foreach (AWBLogListener item in lvIgnored.Items)
            {
                if ((compareMatch && string.Compare(item.SkipReason, filterBy, true) == 0 ) // match
                    || ( !compareMatch && string.Compare(item.SkipReason, filterBy, true) != 0 ) ) // no match
                {
                    FilteredItems.Add(item);
                    item.Remove();
                }
            }
            lvIgnored.EndUpdate();
            RefreshButtonEnablement();
        }

        private void filterShowOnlySelectedToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            FilterItems(false);
        }

        private void filterExcludeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FilterItems(true);
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
            RefreshButtonEnablement();
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
            RefreshButtonEnablement();
        }
        #endregion

        private void btnAddSuccessToList_Click(object sender, EventArgs e)
        {
            List<Article> articles = lvSaved.Items.Cast<ListViewItem>().Select(item => new Article(item.Text)).ToList();

            listMaker.Add(articles);
        }

        /// <summary>
        /// Dummy comparer class, result is that new item compares as less than others
        /// So we get newest first sorting
        /// </summary>
        public class ListViewItemComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                return String.Compare("", "");
            }
        }
    }
}
