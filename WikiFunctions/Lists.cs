/*
(c) 2008 Stephen Kennedy

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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;
using WikiFunctions.Lists;

namespace WikiFunctions.Lists
{
    /// <summary>
    /// An interface implemented by objects which attach to the WikiFunctions' ListMaker combo box and return lists of articles
    /// </summary>
    public interface IListMakerProvider
    {
        /// <summary>
        /// Process the search criteria and return a list of articles
        /// </summary>
        /// <param name="searchCriteria"></param>
        /// <returns></returns>
        List<Article> Search(string[] searchCriteria); // TODO: This may need to use param, or accept one string and perform string splitting within the encapsulated code
        
        /// <summary>
        /// The text to display as the combobox list item
        /// </summary>
        string DisplayText { get; }

        /// <summary>
        /// The text to display inside the Select Source text box
        /// </summary>
        string SelectSourceTextBoxText { get; }

        /// <summary>
        /// 
        /// </summary>
        bool SelectSourceEnabled { get; }

        /// <summary>
        /// Called when the ListMaker Provider has been selected in the ComboBox
        /// </summary>
        /// <returns>A boolean indicating whether Select Source text box should be enabled</returns>
        bool Selected();
    }

    // TODO: May well no longer be needed
    public enum SourceType { None = -1, Category, CategoryRecursive, WhatLinksHere, WhatLinksHereIncludingRedirects, WhatTranscludesPage, LinksOnPage, ImagesOnPage, TransclusionsOnPage, TextFile, GoogleWikipedia, UserContribs, AllUserContribs, SpecialPage, ImageFileLinks, DatabaseDump, MyWatchlist, WikiSearch, Redirects, Plugin }
}
