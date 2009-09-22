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

using System.Collections.Generic;
using System.Collections;
using System.Windows.Forms;

namespace WikiFunctions.Controls.Lists
{
    public class ListBox2 : ListBox, IEnumerable<Article>
    {
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
    }
}
