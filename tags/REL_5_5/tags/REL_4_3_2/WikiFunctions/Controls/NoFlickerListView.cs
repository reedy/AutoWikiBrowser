/*

Copyright (C) 2007 Martin Richards
Copyright (C) 2008 Sam Reed

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
using System.Windows.Forms;

namespace WikiFunctions.Controls
{
    public partial class NoFlickerExtendedListView : ListView
    {
        /// <summary>
        /// 
        /// </summary>
        public NoFlickerExtendedListView()
            : this(false)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sortColumnOnClick"></param>
        public NoFlickerExtendedListView(bool sortColumnOnClick)
            : base()
        {
            if (sortColumnOnClick)
                this.ColumnClick += new ColumnClickEventHandler(ExtendedListView_ColumnClick);

            this.sortColumnsOnClick = sortColumnOnClick;

            this.DoubleBuffered = true;
        }

        bool sortColumnsOnClick;
        /// <summary>
        /// 
        /// </summary>
        public bool SortColumnsOnClick
        {
            set
            {
                if (value && !sortColumnsOnClick)
                    this.ColumnClick += new ColumnClickEventHandler(ExtendedListView_ColumnClick);
                else if (!value && sortColumnsOnClick)
                    this.ColumnClick -= new ColumnClickEventHandler(ExtendedListView_ColumnClick);

                this.sortColumnsOnClick = value;
            }
            get { return this.sortColumnsOnClick; }
        }

        private int sortColumn = -1;

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>From http://msdn2.microsoft.com/en-us/library/ms996467.aspx </remarks>
        void ExtendedListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            try
            {
                this.BeginUpdate();
                // Determine whether the column is the same as the last column clicked.
                if (e.Column != sortColumn)
                {
                    // Set the sort column to the new column.
                    sortColumn = e.Column;
                    // Set the sort order to ascending by default.
                    this.Sorting = SortOrder.Ascending;
                }
                else
                {
                    // Determine what the last sort order was and change it.
                    if (this.Sorting == SortOrder.Ascending)
                        this.Sorting = SortOrder.Descending;
                    else
                        this.Sorting = SortOrder.Ascending;
                }

                // Call the sort method to manually sort.
                this.Sort();
                // Set the lstAccountsItemSorter property to a new lstAccountsItemComparer
                // object.
                this.ListViewItemSorter = new ListViewItemComparer(e.Column, this.Sorting);
                this.EndUpdate();
            }
            catch { }
        }

        /// <summary>
        /// Automatically resize all the coloum in the list view based on whether the text or the title is larger
        /// No BeginUpdate()/EndUpdate called
        /// </summary>
        public void ResizeColumns()
        {
            int width, width2;
            foreach (ColumnHeader head in this.Columns)
            {
                head.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
                width = head.Width;

                head.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                width2 = head.Width;

                if (width2 < width)
                    head.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
            }
        }

        /// <summary>
        /// Automatically resize all the coloum in the list view based on whether the text or the title is larger
        /// </summary>
        /// <param name="useUpdateMethods">Whether to call BeginUpdate() and EndUpdate()</param>
        public void ResizeColumns(bool useUpdateMethods)
        {
            if (useUpdateMethods)
                this.BeginUpdate();

            ResizeColumns();

            if (useUpdateMethods)
                this.EndUpdate();
        }

        /// <summary>
        /// Get/Set if the Control is Double Buffered
        /// </summary>
        protected override bool DoubleBuffered
        {
            get { return base.DoubleBuffered; }
            set { base.DoubleBuffered = value; }
        }

        /// <remarks>From http://msdn2.microsoft.com/en-us/library/ms996467.aspx </remarks>
        sealed class ListViewItemComparer : System.Collections.IComparer
        {
            private int col;
            private SortOrder order;

            public ListViewItemComparer()
            {
                order = SortOrder.Ascending;
            }

            public ListViewItemComparer(int column, SortOrder order)
            {
                col = column;
                this.order = order;
            }

            public int Compare(object x, object y)
            {
                int returnVal;
                // Determine whether the type being compared is a date type.
                try
                {
                    // Parse the two objects passed as a parameter as a DateTime.
                    System.DateTime firstDate =
                            DateTime.Parse(((ListViewItem)x).SubItems[col].Text);
                    System.DateTime secondDate =
                            DateTime.Parse(((ListViewItem)y).SubItems[col].Text);
                    // Compare the two dates.
                    returnVal = DateTime.Compare(firstDate, secondDate);
                }
                // If neither compared object has a valid date format, compare
                // as a string.
                catch
                {
                    // Compare the two items as a string.
                    returnVal = String.Compare(((ListViewItem)x).SubItems[col].Text,
                                ((ListViewItem)y).SubItems[col].Text);
                }
                // Determine whether the sort order is descending.
                if (order == SortOrder.Descending)
                    // Invert the value returned by String.Compare.
                    returnVal *= -1;
                return returnVal;
            }
        }
    }
}
