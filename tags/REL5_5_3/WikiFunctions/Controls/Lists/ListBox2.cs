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

        private class ForwardComparer : IComparer<Article>
        {
        	public int Compare(Article article1, Article article2)
        	{
                return article1.CompareTo(article2);
        	}
        }
        
        private static readonly ForwardComparer ArticleForwardComparer = new ForwardComparer();
        
        /// <summary>
        /// Sorts the article list in alphabetical order, Unicode code point order per https://www.mediawiki.org/wiki/Help:Sorting#Sort_order
        /// </summary>
        public new void Sort()
        {
            SortByDirection(true);
        }

        private class ReverseComparer : IComparer<Article>
        {
        	public int Compare(Article article1, Article article2)
        	{
                return -article1.CompareTo(article2);
        	}
        }

        private static readonly ReverseComparer ArticleReverseComparer = new ReverseComparer();

        /// <summary>
        /// Sorts the article list in reverse alphabetical order
        /// </summary>
        public void ReverseSort()
        {
            SortByDirection(false);
        }

        private void SortByDirection (bool forward)
        {
            BeginUpdate();

            Article[] currentArticles = new Article[Items.Count];

            for (int i = 0; i < Items.Count; i++)
                currentArticles[i] = (Article)Items[i];

            if(forward)
                Array.Sort(currentArticles, ArticleForwardComparer);
            else
                Array.Sort(currentArticles, ArticleReverseComparer);

            Items.Clear();
            Items.AddRange(currentArticles);

            EndUpdate();
        }

        /// <summary>
        /// Removes the currently selected articles from the list
        /// Operates in two modes: a quick (even at 50k volumes) mode when the selected items are in a contiguous block
        /// OR an individual mode for non-contiguous selections, which can be slow on large volumes
        /// </summary>
        public void RemoveSelected()
        {
            int i = SelectedIndex;

            if(i >= 0) // check something selected
            {                
                // is this a contiguous block of selected items: e.g. last selected minus first selected+1 equals the number of selected items
                if(SelectedIndices[SelectedItems.Count-1]-SelectedIndex+1 == SelectedItems.Count)
                {
                    /* Fast block mode: convert all articles in listbox into an article list, remove range from article list by index, then add articles back to 
                       listbox. This means loop through listbox once, a single remove by index operation and a single loop through article list.
                       Alternative of removing selected articles one by one from listbox means each removal requires scan of listbox from beginning up
                       to point of item. e.g. scan 10,000x5 to remove five entries from position 10000 in a list. */

                    List<Article> articles = new List<Article>();
                    
                    for (int j = 0; j < Items.Count; j++)
                        articles.Add((Article)Items[j]);

                    articles.RemoveRange(SelectedIndex, SelectedItems.Count);
                    
                    BeginUpdate();
                    Items.Clear();
                    Items.AddRange(articles.ToArray());
                }
                else // non-contiguous, fall back to loop mode, could be slow on large data volumes.
                {
                    BeginUpdate();

                    /* remove at index rather than removing article, else if list has article twice the first instance of it
                     * will be removed, not necessarily the one the user selected */
                    while (SelectedItems.Count > 0)
                        Items.RemoveAt(SelectedIndex);
                }

                // update selected index: if last deleted article was at end of list then set selected item at end of list
                if (Items.Count > i)
                    SelectedIndex = i;
                else
                    SelectedIndex = Math.Min(i, Items.Count) - 1;

                EndUpdate();
            }
        }

        static string _list = "";

        /// <summary>
        /// 
        /// </summary>
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
                ErrorHandler.HandleException(ex);
            }
        }
    }

    ///// <summary>
    ///// Version of ListBox2 that is defined to take strings
    ///// </summary>
    //public class ListBoxString : ListBox2<string>
    //{
    //}

    /// <summary>
    /// Version of ListBox2 that is defined to take Articles
    /// </summary>
    public class ListBoxArticle : ListBox2<Article>
    {
    }
}
