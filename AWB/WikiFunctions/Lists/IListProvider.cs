/*
(C) 2008 Stephen Kennedy, Sam Reed

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

namespace WikiFunctions.Lists
{
    /// <summary>
    /// An interface implemented by objects which attach to the WikiFunctions' ListMaker combo box and return lists of pages
    /// </summary>
    public interface IListProvider
    {
        /// <summary>
        /// Process the user input (if any) and return a list of pages
        /// </summary>
        /// <param name="searchCriteria">User Input/Pages</param>
        List<Article> MakeList(params string[] searchCriteria);

        /// <summary>
        /// The text to display as the combobox list item
        /// </summary>
        string DisplayText { get; }

        /// <summary>
        /// The text to display inside the Select Source text box
        /// </summary>
        string UserInputTextBoxText { get; }

        /// <summary>
        /// Indicates whether the Select Source text box should be enabled
        /// </summary>
        bool UserInputTextBoxEnabled { get; }

        /// <summary>
        /// Called when the ListMaker Provider has been selected in the ComboBox
        /// </summary>
        void Selected();

        /// <summary>
        /// True if the object expects to be started on a seperate thread
        /// </summary>
        bool RunOnSeparateThread { get; }
    }

    /// <summary>
    /// Extension of IListProvider to provide some extra functionality needed for options on Special Pages
    /// </summary>
    interface ISpecialPageProvider : IListProvider
    {
        /// <summary>
        /// Process the user input (if any) and return a list of pages
        /// </summary>
        /// <param name="Namespace">Namespace to enumerate</param>
        /// <param name="searchCriteria">User Input/Pages</param>
        List<Article> MakeList(int Namespace, params string[] searchCriteria);

        /// <summary>
        /// Whether something must be entered in the text box
        /// </summary>
        bool PagesNeeded { get; }

        /// <summary>
        /// Whether Namespace box should be enabled, therefore whether namespace can be used to filter results
        /// </summary>
        bool NamespacesEnabled { get; }
    }
}
