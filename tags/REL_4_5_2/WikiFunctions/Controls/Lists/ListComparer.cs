/*
ListComparer
Copyright (C) 2007 Martin Richards

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
using System.Text;
using System.Windows.Forms;

namespace WikiFunctions.Controls.Lists
{
    /// <summary>
    /// Provides a form for comparing 2 lists, to find duplicates and/or removing one list from another.
    /// </summary>
    public partial class ListComparer : Form
    {
        public ListComparer()
        {
            InitializeComponent();
            listMaker1.MakeListEnabled = true;
            listMaker2.MakeListEnabled = true;
        }

        public ListComparer(List<Article> list)
            : this()
        {
            listMaker1.Add(list);
        }

        private void GetDuplicates()
        {
            foreach (Article article in listMaker1)
            {
                if (listMaker2.Contains(article))
                    lbBoth.Items.Add(article.Name);
                else
                    lbNo1.Items.Add(article.Name);
            }

            foreach (Article article in listMaker2)
            {
                if (!listMaker1.Contains(article))
                    lbNo2.Items.Add(article.Name);
            }
        
            updateCounts();
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            clear();
            GetDuplicates();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveList(lbBoth);
        }
        
        private void btnClear_Click(object sender, EventArgs e)
        {
            clear();
            updateCounts();
        }

        private void clear()
        {
            lbBoth.Items.Clear();
            lbNo1.Items.Clear();
            lbNo2.Items.Clear();
        }

        private void updateCounts()
        {
            lblNo1.Text = lbNo1.Items.Count + " pages";
            lblNo2.Text = lbNo2.Items.Count + " pages";
            lblNoBoth.Text = lbBoth.Items.Count + " pages";
        }

        private void btnSaveOnly1_Click(object sender, EventArgs e)
        {
            SaveList(lbNo1);
        }

        private void btnSaveOnly2_Click(object sender, EventArgs e)
        {
            SaveList(lbNo2);
        }

        private void SaveList(ListBox lb)
        {
            int i = 0;
            StringBuilder strList = new StringBuilder();

            while (i < lb.Items.Count)
            {
                strList.AppendLine("# [[" + lb.Items[i] + "]]");
                i++;
            }
            SaveList(strList);
        }

        private void SaveList(StringBuilder strList)
        {
            if (saveListDialog.ShowDialog() == DialogResult.OK)
            {
                Tools.WriteTextFileAbsolutePath(strList, saveListDialog.FileName, false);
            }
        }

        private void transferDuplicatesToList1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listMaker1.Clear();
            
            foreach (string str in lbBoth.Items)
                listMaker1.Add(str);
        }

        private void openInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (String article in lbBoth.SelectedItems)
                Tools.OpenArticleInBrowser(article);
        }

        private void openInBrowserToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            foreach (String article in MenuItemOwner(sender).SelectedItems)
                Tools.OpenArticleInBrowser(article);
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tools.Copy(lbBoth);
        }

        private void copySelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tools.Copy(MenuItemOwner(sender));
        }

        private static ListBox MenuItemOwner(object sender)
        {
            try { return ((ListBox)((ContextMenuStrip)sender).SourceControl); }
            catch
            {
                return (ListBox)(((ContextMenuStrip)((ToolStripMenuItem)sender).Owner).SourceControl);
            }
        }

        private void btnMoveOnly1_Click(object sender, EventArgs e)
        {
            foreach (string a in lbNo1.Items)
                listMaker1.Add(a);
        }

        private void btnMoveCommon_Click(object sender, EventArgs e)
        {
            foreach (string a in lbBoth.Items)
                listMaker1.Add(a);
        }

        private void btnMoveOnly2_Click(object sender, EventArgs e)
        {
            foreach (string a in lbNo2.Items)
                listMaker1.Add(a);
        }
    }
}
