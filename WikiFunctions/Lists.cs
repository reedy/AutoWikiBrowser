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
        bool SelectSourceTextBoxEnabled { get;}

        /// <summary>
        /// Called when the ListMaker Provider has been selected in the ComboBox
        /// </summary>
        /// <returns>A boolean indicating whether Select Source text box should be enabled</returns>
        bool Selected();

        /// <summary>
        /// 
        /// </summary>
        bool IsThreaded { get;}
    }

    public class Category : IListMakerProvider
    {
        #region IListMakerProvider Members

        public virtual List<Article> Search(string[] searchCriteria)
        {
            return GetLists.FromCategory(false, searchCriteria);
        }

        public virtual string DisplayText
        {
            get { return "Category"; }
        }

        public string SelectSourceTextBoxText
        {
            get { return Variables.Namespaces[14]; }
        }

        public bool SelectSourceTextBoxEnabled
        { get { return true; } }

        public bool Selected()
        {
            return true;
        }

        public bool IsThreaded
        { get { return true; } }

        #endregion
    }

    public class CategoryRecursive : Category
    {
        public override List<Article> Search(string[] searchCriteria)
        {
            GetLists.QuietMode = true;
            List<Article> ret = GetLists.FromCategory(true, searchCriteria);
            GetLists.QuietMode = false;

            return ret;
        }

        public override string DisplayText
        {
            get { return "Category (recursive)"; }
        }
    }

    public class TextFile : IListMakerProvider
    {
        OpenFileDialog openListDialog;
        public TextFile()
        {
            openListDialog = new OpenFileDialog();
            openListDialog.Filter = "text files|*.txt|All files|*.*";
            openListDialog.Multiselect = true;
        }

        #region IListMakerProvider Members

        public List<Article> Search(string[] searchCriteria)
        {
            List<Article> ret = new List<Article>();
            try
            {
                if (openListDialog.ShowDialog() == DialogResult.OK)
                {
                    ret = GetLists.FromTextFile(openListDialog.FileNames);
                }
                return ret;
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
                return ret;
            }
        }

        public string DisplayText
        {
            get { return "Text File"; }
        }

        public string SelectSourceTextBoxText
        {
            get { return "From file:"; }
        }

        public bool SelectSourceTextBoxEnabled
        { get { return false; } }

        public bool Selected()
        {
            return false;
        }

        public bool IsThreaded
        {
            get { return false; }
        }

        #endregion
    }

    public class WhatLinksHere : IListMakerProvider
    {
        #region IListMakerProvider Members

        public virtual List<Article> Search(string[] searchCriteria)
        {
            return GetLists.FromWhatLinksHere(false, searchCriteria);
        }

        public virtual string DisplayText
        {
            get { return "What links here"; }
        }

        public virtual string SelectSourceTextBoxText
        {
            get { return "What links to"; }
        }

        public bool SelectSourceTextBoxEnabled
        {
            get { return true; }
        }

        public bool Selected()
        {
            return true;
        }

        public bool IsThreaded
        {
            get { return true; }
        }

        #endregion
    }

    public class WhatLinksHereIncludingRedirects : WhatLinksHere
    {
        public override List<Article> Search(string[] searchCriteria)
        {
            return GetLists.FromWhatLinksHere(false, true, searchCriteria);
        }

        public override string DisplayText
        {
            get { return base.DisplayText + " (inc. Redirects)"; }
        }
    }

    public class WhatTranscludesPage : WhatLinksHere
    {
        public override List<Article> Search(string[] searchCriteria)
        {
            return GetLists.FromWhatLinksHere(true, searchCriteria);
        }

        public override string DisplayText
        {
            get { return "What transcludes page"; }
        }

        public override string SelectSourceTextBoxText
        {
            get { return "What embeds"; }
        }
    }

    public class LinksOnPage : IListMakerProvider
    {
        #region IListMakerProvider Members

        public List<Article> Search(string[] searchCriteria)
        {
            return GetLists.FromLinksOnPage(searchCriteria);
        }

        public string DisplayText
        {
            get { return "Links on page"; }
        }

        public string SelectSourceTextBoxText
        {
            get { return "Links on"; }
        }

        public bool SelectSourceTextBoxEnabled
        {
            get { return true; }
        }

        public bool Selected()
        {
            return true;
        }

        public bool IsThreaded
        {
            get { return true; }
        }

        #endregion
    }

    public class ImagesOnPage : IListMakerProvider
    {
        #region IListMakerProvider Members

        public List<Article> Search(string[] searchCriteria)
        {
            return GetLists.FromImagesOnPage(searchCriteria);
        }

        public string DisplayText
        {
            get { return "Images on page"; }
        }

        public string SelectSourceTextBoxText
        {
            get { return "Images on"; }
        }

        public bool SelectSourceTextBoxEnabled
        {
            get { return true; }
        }

        public bool Selected()
        {
            return true;
        }

        public bool IsThreaded
        {
            get { return true; }
        }

        #endregion
    }

    public class TransclusionsOnPage : IListMakerProvider
    {
        #region IListMakerProvider Members

        public List<Article> Search(string[] searchCriteria)
        {
            return GetLists.FromTransclusionsOnPage(searchCriteria);
        }

        public string DisplayText
        {
            get { return "Transclusions on page"; }
        }

        public string SelectSourceTextBoxText
        {
            get { return "Transclusions on"; }
        }

        public bool SelectSourceTextBoxEnabled
        {
            get { return true; }
        }

        public bool Selected()
        {
            return true;
        }

        public bool IsThreaded
        {
            get { return true; }
        }

        #endregion
    }

    public class GoogleSearch : IListMakerProvider
    {
        #region IListMakerProvider Members

        public List<Article> Search(string[] searchCriteria)
        {
            return GetLists.FromGoogleSearch(searchCriteria);
        }

        public string DisplayText
        {
            get { return "Google earch"; }
        }

        public string SelectSourceTextBoxText
        {
            get { return "Google Search:"; }
        }

        public bool SelectSourceTextBoxEnabled
        {
            get { return true; }
        }

        public bool Selected()
        {
            return true;
        }

        public bool IsThreaded
        {
            get { return true; }
        }

        #endregion
    }

    public class UserContribs : IListMakerProvider
    {
        #region IListMakerProvider Members

        public virtual List<Article> Search(string[] searchCriteria)
        {
            return GetLists.FromUserContribs(searchCriteria);
        }

        public virtual string DisplayText
        {
            get { return "User contribs"; }
        }

        public string SelectSourceTextBoxText
        {
            get { return Variables.Namespaces[2]; }
        }

        public bool SelectSourceTextBoxEnabled
        {
            get { return true; }
        }

        public bool Selected()
        {
            return true;
        }

        public bool IsThreaded
        {
            get { return true; }
        }

        #endregion
    }

    public class UserContribsAll : UserContribs
    {

        public override List<Article> Search(string[] searchCriteria)
        {
            return GetLists.FromUserContribs(true, searchCriteria);
        }

        public override string DisplayText
        {
            get { return base.DisplayText + " (all)"; }
        }
    }

    public class SpecialPage : IListMakerProvider
    {
        #region IListMakerProvider Members

        public List<Article> Search(string[] searchCriteria)
        {
            return GetLists.FromSpecialPage(searchCriteria);
        }

        public string DisplayText
        {
            get { return "Special page"; }
        }

        public string SelectSourceTextBoxText
        {
            get { return Variables.Namespaces[-1]; }
        }

        public bool SelectSourceTextBoxEnabled
        {
            get { return true; }
        }

        public bool Selected()
        {
            return true;
        }

        public bool IsThreaded
        {
            get { return true; }
        }

        #endregion
    }

    public class ImageFileLinks : IListMakerProvider
    {
        #region IListMakerProvider Members

        public List<Article> Search(string[] searchCriteria)
        {
            return GetLists.FromImageLinks(searchCriteria);
        }

        public string DisplayText
        {
            get { return "Image file links"; }
        }

        public string SelectSourceTextBoxText
        {
            get { return Variables.Namespaces[6]; }
        }

        public bool SelectSourceTextBoxEnabled
        {
            get { return true; }
        }

        public bool Selected()
        {
            return true;
        }

        public bool IsThreaded
        {
            get { return true; }
        }

        #endregion
    }

    public class WikiSearch : IListMakerProvider
    {
        #region IListMakerProvider Members

        public List<Article> Search(string[] searchCriteria)
        {
            return GetLists.FromWikiSearch(searchCriteria);
        }

        public string DisplayText
        {
            get { return "Wiki search"; }
        }

        public string SelectSourceTextBoxText
        {
            get { return "Wiki search"; }
        }

        public bool SelectSourceTextBoxEnabled
        {
            get { return true; }
        }

        public bool Selected()
        {
            return true;
        }

        public bool IsThreaded
        {
            get { return true; }
        }

        #endregion
    }

    public class Redirects : IListMakerProvider
    {
        #region IListMakerProvider Members

        public List<Article> Search(string[] searchCriteria)
        {
            return GetLists.FromRedirects(searchCriteria);
        }

        public string DisplayText
        {
            get { return "Redirects"; }
        }

        public string SelectSourceTextBoxText
        {
            get { return "Redirects to"; }
        }

        public bool SelectSourceTextBoxEnabled
        {
            get { return true; }
        }

        public bool Selected()
        {
            return true;
        }

        public bool IsThreaded
        {
            get { return true; }
        }

        #endregion
    }

    public class MyWatchlist : IListMakerProvider
    {
        #region IListMakerProvider Members

        public List<Article> Search(string[] searchCriteria)
        {
            return GetLists.FromWatchList();
        }

        public string DisplayText
        {
            get { return "My Watchlist"; }
        }

        public string SelectSourceTextBoxText
        {
            get { return ""; }
        }

        public bool SelectSourceTextBoxEnabled
        {
            get { return false; }
        }

        public bool Selected()
        {
            return false;
        }

        public bool IsThreaded
        {
            get { return true; }
        }

        #endregion
    }

    //public class DatabaseScanner : IListMakerProvider
    //{
    //    #region IListMakerProvider Members

    //    public List<Article> Search(string[] searchCriteria)
    //    {
    //        return new List<Article>();
    //    }

    //    public string DisplayText
    //    {
    //        get { return "Database dump"; }
    //    }

    //    public string SelectSourceTextBoxText
    //    {
    //        get { return ""; }
    //    }

    //    public bool SelectSourceTextBoxEnabled
    //    {
    //        get { return false; }
    //    }

    //    public bool Selected()
    //    {
    //        return false;
    //    }

    //    public bool IsThreaded
    //    {
    //        get { return false; }
    //    }

    //    #endregion
    //}

    // TODO: May well no longer be needed
    public enum SourceType { None = -1, Category, CategoryRecursive, WhatLinksHere, WhatLinksHereIncludingRedirects, WhatTranscludesPage, LinksOnPage, ImagesOnPage, TransclusionsOnPage, TextFile, GoogleWikipedia, UserContribs, AllUserContribs, SpecialPage, ImageFileLinks, DatabaseDump, MyWatchlist, WikiSearch, Redirects, Plugin }
}
