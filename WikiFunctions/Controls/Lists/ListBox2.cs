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
using System.Linq;

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
            Article[] currentArticlesBefore = new Article[Items.Count];

            for (int i = 0; i < Items.Count; i++)
                currentArticles[i] = (Article)Items[i];

            currentArticles.CopyTo(currentArticlesBefore, 0);

            if(forward)
                Array.Sort(currentArticles, ArticleForwardComparer);
            else
                Array.Sort(currentArticles, ArticleReverseComparer);

            // performance of AddRange library method is quite slow
            // so compare arrays and only update articles if sorting changed the order of the articles
            bool same = true;
            for (int i = 0; i < Items.Count; i++)
            {
                if(!currentArticles[i].Equals(currentArticlesBefore[i]))
                {
                    same = false;
                    break;
                }
            }

            if(!same)
            {
                Items.Clear();
                Items.AddRange(currentArticles);
            }

            EndUpdate();
        }

        // Removes the currently selected articles from the list
        public void RemoveSelected(bool filterDuplicates)
        {
            int i = SelectedIndex;

            if (i >= 0) // check something selected
            {
                BeginUpdate();

                // RemoveAt cost product of list size and selected items, cost at least SelectedItems.Count * SelectedIndex
                // So cost low for single/few selected articles, use if less than cost of RemoveSelectedNew, which is ~Items.Count
                if ((SelectedItems.Count*Math.Max(SelectedIndex, 1)) < Items.Count)
                {
                    while (SelectedItems.Count > 0)
                        Items.RemoveAt(SelectedIndex);
                }
                else if (filterDuplicates)
                    RemoveSelected();

                // update selected index: if last deleted article was at end of list then set selected item at end of list
                if (Items.Count > i)
                    SelectedIndex = i;
                else
                    SelectedIndex = Math.Min(i, Items.Count) - 1;

                EndUpdate();
            }
        }

        // Fast implementation to remove the currently selected articles from the list, making use of List.Except method
        // As list is implicitly deduplicated, only use for main listbox if remove duplicates option is on
        private void RemoveSelected()
        {
            List<Article> articles = Items.Cast<Article>().ToList();
            List<Article> Sarticles = SelectedItems.Cast<Article>().ToList();

            Items.Clear();
            Items.AddRange(articles.Except(Sarticles).ToArray());
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
