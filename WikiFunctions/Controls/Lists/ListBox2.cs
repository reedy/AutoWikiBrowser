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
    public class ListBox2 : ListBox, IEnumerable<Article>
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

        public IEnumerator<Article> GetEnumerator()
        {
            int i = 0;
            while (i < Items.Count)
            {
                yield return (Article)Items[i];
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

        public void RemoveSelected()
        {
            BeginUpdate();

            int i = SelectedIndex;

            if (SelectedItems.Count == 1)
                Items.Remove(SelectedItem);
            else
                while (SelectedItems.Count > 0)
                    Items.Remove(SelectedItem);

            if (Items.Count > i)
                SelectedIndex = i;
            else
                SelectedIndex = System.Math.Min(i, Items.Count) - 1;

            EndUpdate();
        }

        static string _list = "";

        public enum OutputFormat
        {
            WikiText = 1,
            PlainText = 2,
            CSV = 3,
            CSVWikiText = 4
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
                        foreach (Article a in this)
                            list.AppendLine("# [[:" + a.Name + "]]");
                        break;
                    case OutputFormat.PlainText:
                        foreach (Article a in this)
                            list.AppendLine(a.Name);
                        break;
                    case OutputFormat.CSV:
                        foreach (Article a in this)
                            list.Append(a.Name + ", ");
                        list = list.Remove(list.Length - 2, 2);
                        break;
                    case OutputFormat.CSVWikiText:
                        foreach (Article a in this)
                            list.Append("[[:" + a.Name + "]], ");
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
}
