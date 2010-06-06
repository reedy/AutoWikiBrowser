/*

Copyright (C) 2007 Martin Richards
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
using System.Collections;
using System.Text;
using System.Windows.Forms;

namespace WikiFunctions.Controls.Lists
{
    /// <summary>
    /// Enhanced list box
    /// </summary>
    /// <typeparam name="T">Type of items in the list</typeparam>
    public class ListBox2<T> : ListBox, IEnumerable<T>
    {
        private readonly static SaveFileDialog SaveListDialog;

        static ListBox2()
        {
            SaveListDialog = new SaveFileDialog
                                 {
                                     DefaultExt = "txt",
                                     Filter =
                                         "Text file with wiki markup|*.txt|Plaintext list|*.txt|CSV (Comma Separated Values)|*.txt|CSV with Wikitext|*.txt",
                                     Title = "Save article list"
                                 };
        }


        public IEnumerator<T> GetEnumerator()
        {
            int i = 0;
            while (i < Items.Count)
            {
                yield return (T)Items[i];
                i++;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            int i = 0;
            while (i < Items.Count)
            {
                yield return Items[i];
                i++;
            }
        }

        public new void Sort()
        {
            BeginUpdate();
            Sorted = true;
            Sorted = false;
            EndUpdate();
        }
        
        private class ReverseComparer: IComparer
        {
            public int Compare (Object object1, Object object2)
            {
                return -((IComparable)  object1).CompareTo (object2);
            }
        }
        
        /// <summary>
        /// Sorts the article list in reverse alphabetical order
        /// </summary>
        public void ReverseSort()
        {
            BeginUpdate();
            Sorted = false;

            string[] currentArticles = new string[Items.Count];

            for (int i = 0; i < Items.Count; i++)
                currentArticles[i] = Items[i].ToString();

            Array.Sort(currentArticles, new ReverseComparer());

            Items.Clear();

            foreach (string str in currentArticles)
                Items.Add(new Article(str));

            EndUpdate();
        }

        public void RemoveSelected()
        {
            BeginUpdate();

            int i = SelectedIndex;

            /* remove at index rather than removing article, else if list has article twice the first instance of it
             * will be removed, not necessarily the one the user selected
            */ 
            while (SelectedItems.Count > 0)
                Items.RemoveAt(SelectedIndex);

            if (Items.Count > i)
                SelectedIndex = i;
            else
                SelectedIndex = Math.Min(i, Items.Count) - 1;

            EndUpdate();
        }

        static string _list = "";

        public enum OutputFormat
        {
            WikiText = 1,
            PlainText = 2,
            Csv = 3,
            CsvWikiText = 4
        }

        public void SaveList()
        {
            if (_list.Length > 0) SaveListDialog.FileName = _list;

            if (SaveListDialog.ShowDialog() == DialogResult.OK)
            {
                _list = SaveListDialog.FileName;
                SaveList(_list, (OutputFormat)SaveListDialog.FilterIndex);
            }
        }

        public void SaveList(string filename, OutputFormat format)
        {
            try
            {
                StringBuilder list = new StringBuilder();

                switch (format)
                {
                    case OutputFormat.WikiText:
                        foreach (var a in this)
                            list.AppendLine("# [[:" + a + "]]");
                        break;
                    case OutputFormat.PlainText:
                        foreach (var a in this)
                            list.AppendLine(a.ToString());
                        break;
                    case OutputFormat.Csv:
                        foreach (var a in this)
                            list.Append(a + ", ");
                        list = list.Remove(list.Length - 2, 2);
                        break;
                    case OutputFormat.CsvWikiText:
                        foreach (var a in this)
                            list.Append("[[:" + a + "]], ");
                        list = list.Remove(list.Length - 2, 2);
                        break;
                }

                Tools.WriteTextFileAbsolutePath(list, filename, false);
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }
    }

    /// <summary>
    /// Version of ListBox2 that is defined to take strings
    /// </summary>
    public class ListBoxString : ListBox2<string>
    {
    }

    /// <summary>
    /// Version of ListBox2 that is defined to take Articles
    /// </summary>
    public class ListBoxArticle : ListBox2<Article>
    {
    }
}
