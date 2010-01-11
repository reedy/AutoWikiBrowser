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
using System.Text;
using System.Windows.Forms;

using WikiFunctions.Controls;
using WikiFunctions.Controls.Lists;

namespace WikiFunctions.Logging
{
    public partial class ArticleActionLogControl : UserControl
    {
        private ListMaker _listMaker;

        #region Public
        public ArticleActionLogControl()
        {
            InitializeComponent();
        }

        public void Initialise(ListMaker rlistMaker)
        {
            _listMaker = rlistMaker;
            ResizeListView(lvFailed);
            ResizeListView(lvSuccessful);
        }

        public void LogArticleAction(string page, bool succeeded, ArticleAction action, string message)
        {
            ListViewItem item  = new ListViewItem(page);
            item.SubItems.Add(action.ToString());
            item.SubItems.Add(DateTime.Now.ToString());
            item.SubItems.Add(message);

            if (!succeeded)
            {
                lvFailed.Items.Add(item);
                ResizeListView(lvFailed);
            }
            else
            {
                lvSuccessful.Items.Add(item);
                ResizeListView(lvSuccessful);
            }
        }
        #endregion

        #region Private/Protected
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
        #endregion

        #region Event Handlers

        private void addSelectedToArticleListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in MenuItemOwner(sender).SelectedItems)
            {
                _listMaker.Add(new Article(item.Text));
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

                foreach (ListViewItem lvi in listview.Items)
                {
                    strList.AppendLine(lvi.Text);
                }
                Tools.WriteTextFileAbsolutePath(strList.ToString(), saveListDialog.FileName, false);
            }
        }

        private void btnAddToList_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem article in lvFailed.Items)
                _listMaker.Add(new Article(article.Text));
        }

        private void btnSaveSaved_Click(object sender, EventArgs e)
        {
            SaveListView(lvSuccessful);
        }

        private void btnSaveIgnored_Click(object sender, EventArgs e)
        {
            SaveListView(lvFailed);
        }

        private void btnClearSaved_Click(object sender, EventArgs e)
        {
            lvSuccessful.Items.Clear();
        }

        private void btnClearIgnored_Click(object sender, EventArgs e)
        {
            lvFailed.Items.Clear();
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
            foreach (ListViewItem item in MenuItemOwner(sender).SelectedItems)
                Tools.OpenArticleInBrowser(item.Text);
        }

        private void openHistoryInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in MenuItemOwner(sender).SelectedItems)
                Tools.OpenArticleHistoryInBrowser(item.Text);
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveSelected(sender);
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MenuItemOwner(sender) == lvFailed)
            {
                lvFailed.Items.Clear();
            }
            else
            {
                lvSuccessful.Items.Clear();
            }
        }
        #endregion

        private void openLogInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in MenuItemOwner(sender).SelectedItems)
                Tools.OpenArticleLogInBrowser(item.Text);
        }
    }
}
