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
            //TODO:Needs to distinguish between which listview the context menu was opened from
            //Sender is context menu
            if (sender == lvIgnored)
            {
                filterByReasonOfSelectedToolStripMenuItem.Enabled = (lvIgnored.SelectedItems.Count == 1);
            }
            else
                filterByReasonOfSelectedToolStripMenuItem.Enabled = false;
        }

        private void addSelectedToArticleListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //TODO:Needs to distinguish between which listview the context menu was opened from
            //(ListView)sender).SelectedItems was tried, but sender is context menu
            foreach (ListViewItem item in ((ListView)sender).SelectedItems)
            {
                listMaker.Add(new Article(item.Text));
            }
        }

        private void filterByReasonOfSelectedToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            //Doesnt need changing
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
    }
}
