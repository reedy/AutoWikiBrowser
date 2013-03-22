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
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections;

namespace WikiFunctions.Controls
{
    /// <summary>
    /// Inreface for creation of custom sorters
    /// </summary>
    public interface IListViewItemComparerFactory
    {
        IComparer CreateComparer(int column, SortOrder order);
    }

    /// <summary>
    /// Advanced ListView
    /// </summary>
    public class NoFlickerExtendedListView : ListView, IListViewItemComparerFactory
    {
        /// <summary>
        /// 
        /// </summary>
        public NoFlickerExtendedListView()
            : this(false, false)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sortColumnOnClick"></param>
        /// <param name="resizeColumnsOnControlResize"></param>
        public NoFlickerExtendedListView(bool sortColumnOnClick, bool resizeColumnsOnControlResize)
        {
            SortColumnsOnClick = sortColumnOnClick;
            ResizeColumsOnControlResize = resizeColumnsOnControlResize;
            sortColumnsOnClick = sortColumnOnClick;
            DoubleBuffered = true;
            comparerFactory = this;
        }

        private void NoFlickerExtendedListView_Resize(object sender, EventArgs e)
        {
            ResizeColumns(true);
        }

        bool resizeColumnsOnControlResize;
        [DefaultValue(false)]
        public bool ResizeColumsOnControlResize
        {
            set
            {
                if (value && !resizeColumnsOnControlResize)
                    this.Resize += new EventHandler(NoFlickerExtendedListView_Resize);
                else if (!value && resizeColumnsOnControlResize)
                    this.Resize -= new EventHandler(NoFlickerExtendedListView_Resize);

                this.resizeColumnsOnControlResize = value;
            }
            get { return this.resizeColumnsOnControlResize; }
        }

        bool sortColumnsOnClick;
        [DefaultValue(false)]
        /// <summary>
        /// Enables or disables sorting upon click on column header
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

        private int sortColumn = 0;

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
                this.ListViewItemSorter = comparerFactory.CreateComparer(e.Column, this.Sorting);
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

        private IListViewItemComparerFactory comparerFactory;

        /// <summary>
        /// Allows to override default column sorting behaviour by providing a factory for custom sorters
        /// </summary>
        [Browsable(false)]
        [Localizable(false)]
        public IListViewItemComparerFactory ComparerFactory
        {
            get { return comparerFactory; }
            set
            {
                comparerFactory = value;
                ListViewItemSorter = comparerFactory.CreateComparer(sortColumn, Sorting);
            }
        }

        public IComparer CreateComparer(int column, SortOrder order)
        {
            return new ListViewItemComparer(column, order);
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

        sealed class ListViewItemComparer : IComparer
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

                string sx = ((ListViewItem)x).SubItems[col].Text;
                string sy = ((ListViewItem)y).SubItems[col].Text;

                DateTime firstDate, secondDate;
                double dblX, dblY;

                // first try to parse as ints
                if (double.TryParse(sx, out dblX) && double.TryParse(sy, out dblY))
                {
                    returnVal = dblX.CompareTo(dblY);
                }
                else
                // Determine whether the type being compared is a DateTime type
                if (DateTime.TryParse(sx, out firstDate) && DateTime.TryParse(sy, out secondDate))
                {
                    // Compare the two dates.
                    returnVal = DateTime.Compare(firstDate, secondDate);
                }
                else
                {
                    // If neither compared object has a valid int or date format, compare
                    // as a string.
                    returnVal = String.Compare(sx, sy);
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
