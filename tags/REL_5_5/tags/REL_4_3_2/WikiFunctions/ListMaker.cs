/*
ListMaker

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

namespace WikiFunctions.Controls.Lists
{
    public delegate void ListMakerEventHandler(object sender, EventArgs e);

    public partial class ListMaker : UserControl, IEnumerable<Article>, ICollection<Article>, IList<Article>
    {
        public event ListMakerEventHandler StatusTextChanged;
        public event ListMakerEventHandler BusyStateChanged;
        public event ListMakerEventHandler NoOfArticlesChanged;
        public bool FilterNonMainAuto;
        public bool AutoAlpha;
        public bool FilterDuplicates;
        /// <summary>
        /// Occurs when a list has been created
        /// </summary>
        public event ListMakerEventHandler ListFinished;

        SpecialFilter SpecialFilter = new SpecialFilter();

        public ListMaker()
        {
            InitializeComponent();
            //if (Variables.LangCode == LangCodeEnum.en)
            //    cmboSourceSelect.Items.Add("Redirects");
        }

        new public static void Refresh()
        { }

        public string strlbArticlesTooltip = "";
        public string strtxtNewArticleTooltip = "";

        #region Enumerator
        public IEnumerator<Article> GetEnumerator()
        {
            int i = 0;
            while (i < lbArticles.Items.Count)
            {
                yield return (Article)lbArticles.Items[i];
                i++;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            int i = 0;
            while (i < lbArticles.Items.Count)
            {
                yield return (Article)lbArticles.Items[i];
                i++;
            }
        }

        #endregion

        #region ICollection<Article> Members

        /// <summary>
        /// Adds the article to the list
        /// </summary>
        public void Add(Article item)
        {
            lbArticles.Items.Add(item);
            Saved = false;
            UpdateNumberOfArticles();
        }

        /// <summary>
        /// Clears the list
        /// </summary>
        public void Clear()
        {
            lbArticles.Items.Clear();
            Saved = false;
            UpdateNumberOfArticles();
        }

        /// <summary>
        /// Removes/Adds the "Redirects" option from/to the list
        /// </summary>
        public void AddRemoveRedirects()
        {
            if (Variables.LangCode != LangCodeEnum.en)
                cmboSourceSelect.Items.Remove("Redirects");
            else
                if (!cmboSourceSelect.Items.Contains("Redirects"))
                    cmboSourceSelect.Items.Add("Redirects");
        }

        /// <summary>
        /// Returns a value indicating whether the given article is in the list
        /// </summary>
        public bool Contains(Article item)
        {
            return lbArticles.Items.Contains(item);
        }

        public void CopyTo(Article[] array, int arrayIndex)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Returns the length of the list
        /// </summary>
        public int Count
        {
            get { return lbArticles.Items.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Removes the given article, by title, from the list
        /// </summary>
        public bool Remove(string Title)
        {
            Article a = new Article(Title);
            return Remove(a);
        }

        /// <summary>
        /// Removes the given article from the list
        /// </summary>
        public bool Remove(Article item)
        {
            if (lbArticles.Items.Contains(item))
            {
                while (lbArticles.SelectedItems.Count > 0)
                    lbArticles.SetSelected(lbArticles.SelectedIndex, false);

                txtNewArticle.Text = item.Name;

                int intPosition = lbArticles.Items.IndexOf(item);

                lbArticles.Items.Remove(item);

                if (lbArticles.Items.Count == intPosition)
                    intPosition--;

                if (lbArticles.Items.Count > 0)
                    lbArticles.SelectedIndex = intPosition;

                Saved = false;
                UpdateNumberOfArticles();
                return true;
            }
            else
                return false;
        }

        #endregion

        #region IList<Article> Members

        /// <summary>
        /// Returns the index of the given article
        /// </summary>
        public int IndexOf(Article item)
        {
            return lbArticles.Items.IndexOf(item);
        }

        /// <summary>
        /// Returns the index of the given article title
        /// </summary>
        public int IndexOf(string item)
        {
            return lbArticles.Items.IndexOf(item);
        }

        /// <summary>
        /// Inserts the given article at a specific index
        /// </summary>
        public void Insert(int index, Article item)
        {
            lbArticles.Items.Insert(index, item);
            Saved = false;
            UpdateNumberOfArticles();
        }

        /// <summary>
        /// Inserts the given article at a specific index
        /// </summary>
        public void Insert(int index, string item)
        {
            Article a = new Article(item);
            lbArticles.Items.Insert(index, a);
            Saved = false;
            UpdateNumberOfArticles();
        }

        /// <summary>
        /// Removes article at the given index
        /// </summary>
        public void RemoveAt(int index)
        {
            lbArticles.Items.RemoveAt(index);
            Saved = false;
            UpdateNumberOfArticles();
        }

        public Article this[int index]
        {
            get { return (Article)lbArticles.Items[index]; }
            set { lbArticles.Items[index] = value; }
        }

        #endregion

        #region Events

        private void cmboSourceSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DesignMode) return; // avoid calling Variables constructor

            switch (SelectedSource)
            {
                case SourceType.Category:
                    lblSourceSelect.Text = Variables.Namespaces[14];
                    txtSelectSource.Enabled = true;
                    break;
                case SourceType.WhatLinksHere:
                    lblSourceSelect.Text = "What links to";
                    txtSelectSource.Enabled = true;
                    break;
                case SourceType.WhatTranscludesPage:
                    lblSourceSelect.Text = "What embeds";
                    txtSelectSource.Enabled = true;
                    break;
                case SourceType.LinksOnPage:
                    lblSourceSelect.Text = "Links on";
                    txtSelectSource.Enabled = true;
                    break;
                case SourceType.ImagesOnPage:
                    lblSourceSelect.Text = "Images on";
                    txtSelectSource.Enabled = true;
                    break;
                case SourceType.TransclusionsOnPage:
                    lblSourceSelect.Text = "Transclusions on";
                    txtSelectSource.Enabled = true;
                    break;
                case SourceType.TextFile:
                    lblSourceSelect.Text = "From file:";
                    txtSelectSource.Enabled = false;
                    break;
                case SourceType.GoogleWikipedia:
                    lblSourceSelect.Text = "Google search:";
                    txtSelectSource.Enabled = true;
                    break;
                case SourceType.UserContribs:
                    lblSourceSelect.Text = Variables.Namespaces[2];
                    txtSelectSource.Enabled = true;
                    break;
                case SourceType.AllUserContribs:
                    lblSourceSelect.Text = Variables.Namespaces[2];
                    txtSelectSource.Enabled = true;
                    break;
                case SourceType.SpecialPage:
                    lblSourceSelect.Text = Variables.Namespaces[-1];
                    txtSelectSource.Enabled = true;
                    break;
                case SourceType.ImageFileLinks:
                    lblSourceSelect.Text = Variables.Namespaces[6];
                    txtSelectSource.Enabled = true;
                    break;
                case SourceType.WikiSearch:
                    lblSourceSelect.Text = "Wiki search";
                    txtSelectSource.Enabled = true;
                    break;
                case SourceType.Redirects:
                    lblSourceSelect.Text = "Redirects to:";
                    txtSelectSource.Enabled = true;
                    break;
                case SourceType.CategoryRecursive:
                    lblSourceSelect.Text = Variables.Namespaces[14];
                    txtSelectSource.Enabled = true;
                    break;
                default:
                    lblSourceSelect.Text = "";
                    txtSelectSource.Enabled = false;
                    break;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtNewArticle.Text.Length == 0)
            {
                UpdateNumberOfArticles();
                return;
            }

            Add(txtNewArticle.Text);
            txtNewArticle.Text = "";
        }

        private void btnRemoveArticle_Click(object sender, EventArgs e)
        {
            RemoveSelectedArticle();
        }

        private void btnArticlesListClear_Click(object sender, EventArgs e)
        {
            if (lbArticles.Items.Count <= 1 || (MessageBox.Show(
            "Are you sure you want to clear the article list?", "Clear?", MessageBoxButtons.YesNo)
            == DialogResult.Yes))
                Clear();
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            Filter();
        }

        private void btnArticlesListSave_Click(object sender, EventArgs e)
        {
            SaveList();
        }

        private void btnMakeList_Click(object sender, EventArgs e)
        {
            //make sure there is some text.
            if (txtSelectSource.Text.Length == 0 && txtSelectSource.Enabled)
            {
                MessageBox.Show("Please enter some text", "No text", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SourceType st = SelectedSource;
            if (st != SourceType.WikiSearch && st != SourceType.GoogleWikipedia)
            {
                txtSelectSource.Text = Tools.RemoveHashFromPageTitle(txtSelectSource.Text.Trim('[', ']'));
            }

            if (st == SourceType.Category || st == SourceType.CategoryRecursive)
                txtSelectSource.Text = Regex.Replace(txtSelectSource.Text, "^" + Variables.NamespacesCaseInsensitive[14], "", RegexOptions.IgnoreCase);
            else if (st == SourceType.UserContribs)
                txtSelectSource.Text = Regex.Replace(txtSelectSource.Text, "^" + Variables.Namespaces[2], "", RegexOptions.IgnoreCase);
            else if (st == SourceType.SpecialPage)
                txtSelectSource.Text = Regex.Replace(txtSelectSource.Text, "^" + Variables.Namespaces[-1], "", RegexOptions.IgnoreCase);
            else if (st == SourceType.ImageFileLinks)
                txtSelectSource.Text = Regex.Replace(txtSelectSource.Text, "^" + Variables.Namespaces[6], "", RegexOptions.IgnoreCase);

            if (st != SourceType.GoogleWikipedia && st != SourceType.WikiSearch)
                txtSelectSource.Text = Tools.TurnFirstToUpper(txtSelectSource.Text);
            txtSelectSource.AutoCompleteCustomSource.Add(txtSelectSource.Text);

            txtSelectSource.Text = txtSelectSource.Text.Trim();
            MakeList(st, txtSelectSource.Text.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
        }

        private void lbArticles_MouseMove(object sender, MouseEventArgs e)
        {
            string strTip = "";

            //Get the item
            int nIdx = lbArticles.IndexFromPoint(e.Location);
            if ((nIdx >= 0) && (nIdx < lbArticles.Items.Count))
                strTip = lbArticles.Items[nIdx].ToString();
            if (strTip != strlbArticlesTooltip)
                toolTip1.SetToolTip(lbArticles, strTip);
            strlbArticlesTooltip = strTip;
        }

        private void txtNewArticle_MouseMove(object sender, MouseEventArgs e)
        {
            if (txtNewArticle.Text != strtxtNewArticleTooltip)
                toolTip1.SetToolTip(txtNewArticle, txtNewArticle.Text);
            strtxtNewArticleTooltip = txtNewArticle.Text;
        }

        private void lbArticles_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                btnRemoveArticle.PerformClick();
        }


        private void txtNewArticle_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Enter)
            {
                btnAdd.PerformClick();
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

        private void txtSelectSource_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Enter)
                btnMakeList.PerformClick();
        }


        private void mnuListBox_Opening(object sender, CancelEventArgs e)
        {
            addSelectedToListToolStripMenuItem.Enabled = copyToolStripMenuItem.Enabled = cutToolStripMenuItem.Enabled = (lbArticles.SelectedItems.Count > 0);

            removeToolStripMenuItem.Enabled = lbArticles.SelectedItem != null;
            clearToolStripMenuItem1.Enabled = filterOutNonMainSpaceArticlesToolStripMenuItem.Enabled =
            convertToTalkPagesToolStripMenuItem.Enabled = convertFromTalkPagesToolStripMenuItem.Enabled =
            sortAlphebeticallyMenuItem.Enabled = saveListToTextFileToolStripMenuItem1.Enabled = specialFilterToolStripMenuItem.Enabled =
            selectAllToolStripMenuItem.Enabled = invertSelectionToolStripMenuItem.Enabled = selectNoneToolStripMenuItem.Enabled =
            openInBrowserToolStripMenuItem.Enabled = (lbArticles.Items.Count > 0);
        }

        private void txtNewArticle_DoubleClick(object sender, EventArgs e)
        {
            txtNewArticle.SelectAll();
        }

        private void txtSelectSource_DoubleClick(object sender, EventArgs e)
        {
            txtSelectSource.SelectAll();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Sets whether the buttons are enabled or not
        /// </summary>
        public bool ButtonsEnabled
        {
            set
            {
                btnFilter.Enabled = btnRemoveArticle.Enabled = btnArticlesListClear.Enabled =
                btnArticlesListSave.Enabled = btnRemoveDuplicates.Enabled = value;
            }
        }

        /// <summary>
        /// Gets or sets the selected index
        /// </summary>
        public SourceType SelectedSource
        {
            get { return (SourceType)cmboSourceSelect.SelectedIndex; }
            set { cmboSourceSelect.SelectedIndex = (int)value; }
        }

        /// <summary>
        /// Gets or sets the source text
        /// </summary>
        public string SourceText
        {
            get { return txtSelectSource.Text; }
            set { txtSelectSource.Text = value; }
        }

        /// <summary>
        /// Set whether the make button is enabled
        /// </summary>
        public bool MakeListEnabled
        {
            set { btnMakeList.Enabled = value; }
        }

        /// <summary>
        /// Gets the number of articles in the list
        /// </summary>
        public int NumberOfArticles
        {
            get { return lbArticles.Items.Count; }
        }

        bool bSaved = true;
        /// <summary>
        /// Gets a value showing whether the list has been saved since last changed
        /// </summary>
        public bool Saved
        {
            get { return bSaved; }
            private set { bSaved = value; }
        }

        bool bWikiStatus = true;
        [Browsable(false)]
        public bool WikiStatus
        {
            get { return bWikiStatus; }
            set { bWikiStatus = value; }
        }

        string strStatus = "";
        /// <summary>
        /// The status of the process
        /// </summary>        
        public string Status
        {
            get { return strStatus; }
            private set
            {
                strStatus = value;
                if (StatusTextChanged != null)
                    this.StatusTextChanged(null, null);
            }
        }

        bool bBusyStatus;
        /// <summary>
        /// Gets a value indicating whether the process is busy
        /// </summary>
        public bool BusyStatus
        {
            get { return bBusyStatus; }
            private set
            {
                bBusyStatus = value;
                if (BusyStateChanged != null)
                    this.BusyStateChanged(null, null);
            }
        }

        string strListFile = "";
        /// <summary>
        /// The file the list was made from
        /// </summary>
        public string ListFile
        {
            get { return strListFile; }
            set { strListFile = value; }
        }

        /// <summary>
        /// Returns the selected article
        /// </summary>
        public Article SelectedArticle()
        {
            if (lbArticles.SelectedItem == null)
                lbArticles.SelectedIndex = 0;

            return (Article)lbArticles.SelectedItem;
        }

        #endregion

        #region Methods

        private void launchDumpSearcher()
        {
            WikiFunctions.DBScanner.DatabaseScanner ds = new WikiFunctions.DBScanner.DatabaseScanner(lbArticles);
            ds.Show();
            UpdateNumberOfArticles();
        }

        private delegate void AddToListDel(string s);
        /// <summary>
        /// Adds the given string to the list, first turning it into an Article
        /// </summary>
        public void Add(string s)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new AddToListDel(Add), s);
                return;
            }

            Saved = false;
            if (s.Contains("#"))
                s = s.Substring(0, s.IndexOf('#'));

            Article a = new Article(Tools.RemoveSyntax(Tools.TurnFirstToUpper(s)));
            lbArticles.Items.Add(a);
            UpdateNumberOfArticles();
            if (FilterNonMainAuto)
                FilterNonMainArticles();
            if (FilterDuplicates)
                removeListDuplicates();
        }

        private delegate void AddDel(List<Article> l);
        /// <summary>
        /// Adds the article list to the list
        /// </summary>
        public void Add(List<Article> l)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new AddDel(Add), l);
                return;
            }

            Saved = false;

            lbArticles.BeginUpdate();

            //foreach (Article a in l)
            //{
            //    //if (!lbArticles.Items.Contains(a))
            //    lbArticles.Items.Add(a);
            //}
            lbArticles.Items.AddRange(l.ToArray());

            lbArticles.EndUpdate();

            UpdateNumberOfArticles();
        }

        /// <summary>
        /// Returns the list of articles
        /// </summary>
        public List<Article> GetArticleList()
        {
            List<Article> list = new List<Article>();

            foreach (Article a in lbArticles)
            {
                list.Add(a);
            }

            return list;
        }

        /// <summary>
        /// Returns the list of selected articles
        /// </summary>
        public List<Article> GetSelectedArticleList()
        {
            List<Article> list = new List<Article>();

            foreach (Article a in lbArticles.SelectedItems)
            {
                list.Add(a);
            }

            return list;
        }

        private delegate void StartProgBarDelegate();
        private void StartProgressBar()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new StartProgBarDelegate(StartProgressBar));
                return;
            }

            BusyStatus = true;

            this.Cursor = Cursors.WaitCursor;
            Status = "Getting list";
            btnMakeList.Enabled = false;
        }

        private delegate void SetProgBarDelegate();
        private void StopProgressBar()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new SetProgBarDelegate(StopProgressBar));
                return;
            }

            this.Cursor = Cursors.Default;
            btnMakeList.Enabled = true;
            Status = "List complete!";
            BusyStatus = false;
            btnStop.Visible = false;
            UpdateNumberOfArticles();

            if (ListFinished != null)
                ListFinished(null, null);
        }

        Thread ListerThread = null;

        /// <summary>
        /// Makes a list of pages
        /// </summary>
        /// <param name="ST">The type of list to create</param>
        /// <param name="SourceValues">An array of string values to create the list with, e.g. an array of categories. Use null if not appropriate</param>
        public void MakeList(SourceType st, string[] sourceValues)
        {
            btnStop.Visible = true;
            if (st == SourceType.DatabaseDump)
            {
                launchDumpSearcher();
                return;
            }
            else if (st == SourceType.TextFile)
            {
                try
                {
                    OpenFileDialog openListDialog = new OpenFileDialog();
                    openListDialog.Filter = "text files|*.txt|All files|*.*";
                    openListDialog.Multiselect = true;

                    this.Focus();
                    if (openListDialog.ShowDialog() == DialogResult.OK)
                    {
                        Add(GetLists.FromTextFile(openListDialog.FileNames));
                        ListFile = openListDialog.FileName;
                    }
                    UpdateNumberOfArticles();
                }
                catch (Exception ex)
                {
                    ErrorHandler.Handle(ex);
                }

                return;
            }
            else if (st == SourceType.MyWatchlist)
            {
                try
                {
                    BusyStatus = true;
                    Add(GetLists.FromWatchList());
                    BusyStatus = false;
                    UpdateNumberOfArticles();
                    return;
                }
                catch
                {
                    MessageBox.Show("Please ensure you are logged in", "Log In");
                }
            }
            else
            {
                Source = st;
                strSource = sourceValues;

                ThreadStart thr_Process = new ThreadStart(MakeList2);
                ListerThread = new Thread(thr_Process);
                ListerThread.IsBackground = true;
                ListerThread.Start();
            }
            if (FilterNonMainAuto)
                FilterNonMainArticles();
            if (FilterDuplicates)
                removeListDuplicates();
        }

        SourceType Source = SourceType.Category;
        string[] strSource;

        private void MakeList2()
        {
            Saved = false;
            StartProgressBar();

            try
            {
                switch (Source)
                {
                    case SourceType.Category:
                        Add(GetLists.FromCategory(false, strSource));
                        break;
                    case SourceType.WhatLinksHere:
                        Add(GetLists.FromWhatLinksHere(false, strSource));
                        break;
                    case SourceType.WhatTranscludesPage:
                        Add(GetLists.FromWhatLinksHere(true, strSource));
                        break;
                    case SourceType.LinksOnPage:
                        Add(GetLists.FromLinksOnPage(strSource));
                        break;
                    case SourceType.ImagesOnPage:
                        Add(GetLists.FromImagesOnPage(strSource));
                        break;
                    case SourceType.TransclusionsOnPage:
                        Add(GetLists.FromTransclusionsOnPage(strSource));
                        break;
                    case SourceType.GoogleWikipedia:
                        Add(GetLists.FromGoogleSearch(strSource));
                        break;
                    case SourceType.UserContribs:
                        Add(GetLists.FromUserContribs(strSource));
                        break;
                    case SourceType.AllUserContribs:
                        Add(GetLists.FromUserContribs(true, strSource));
                        break;
                    case SourceType.SpecialPage:
                        Add(GetLists.FromSpecialPage(strSource));
                        break;
                    case SourceType.ImageFileLinks:
                        Add(GetLists.FromImageLinks(strSource));
                        break;
                    case SourceType.WikiSearch:
                        Add(GetLists.FromWikiSearch(strSource));
                        break;
                    case SourceType.Redirects:
                        Add(GetLists.FromRedirects(strSource));
                        break;
                    case SourceType.CategoryRecursive:
                        GetLists.QuietMode = true;
                        Add(GetLists.FromCategory(true, strSource));
                        GetLists.QuietMode = false;
                        break;
                    default:
                        break;
                }

            }
            catch (ThreadAbortException)
            {
            }
            catch (PageDoesNotExistException ex)
            {
                MessageBox.Show(ex.Message, "Page does not exist", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                ErrorHandler.ListMakerText = txtSelectSource.Text;
                ErrorHandler.Handle(ex);
            }
            finally
            {
                if (FilterNonMainAuto)
                    FilterNonMainArticles();
                if (FilterDuplicates)
                    removeListDuplicates();
                StopProgressBar();
            }
        }

        private void RemoveSelectedArticle()
        {
            lbArticles.BeginUpdate();

            try
            {
                Saved = false;
                int i = lbArticles.SelectedIndex;

                if (lbArticles.SelectedItems.Count > 0)
                    txtNewArticle.Text = lbArticles.SelectedItem.ToString();

                while (lbArticles.SelectedItems.Count > 0)
                    lbArticles.Items.Remove(lbArticles.SelectedItem);

                if (lbArticles.Items.Count > i)
                    lbArticles.SelectedIndex = i;
                else
                    lbArticles.SelectedIndex = i - 1;
            }
            catch
            { }

            lbArticles.EndUpdate();

            UpdateNumberOfArticles();
        }

        /// <summary>
        /// Opens the dialog to filter out articles
        /// </summary>
        public void Filter()
        {

            SpecialFilter.lb = lbArticles;
            SpecialFilter.ShowDialog(this);
        }

        /// <summary>
        /// Automatically removes all duplicates from the list
        /// </summary>
        public void removeListDuplicates()
        {
            SpecialFilter.lb = lbArticles;
            SpecialFilter.Clear();
            SpecialFilter.RemoveDuplicates();

            UpdateNumberOfArticles();
        }

        /// <summary>
        /// Saves the list to the specified text file.
        /// </summary>
        public void SaveList()
        {//Save lbArticles list to text file.
            try
            {
                StringBuilder strList = new StringBuilder("");
                StreamWriter sw;

                if (strListFile.Length > 0) saveListDialog.FileName = strListFile;

                if (saveListDialog.ShowDialog() == DialogResult.OK)
                {
                    switch (saveListDialog.FilterIndex)
                    {
                        case 1: //wikitext
                            foreach (Article a in lbArticles)
                                strList.AppendLine("# [[:" + a.Name + "]]");
                            break;
                        case 2: //plaintext
                            foreach (Article a in lbArticles)
                                strList.AppendLine(a.Name);
                            break;
                        case 3: //CSV
                            foreach (Article a in lbArticles)
                                strList.Append(a.Name + ", ");

                            strList = strList.Remove(strList.Length - 2, 2);
                            break;
                    }
                    strListFile = saveListDialog.FileName;
                    sw = new StreamWriter(strListFile, false, Encoding.UTF8);
                    sw.Write(strList);
                    sw.Close();
                    Saved = true;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }

        private delegate void FilterNM();

        /// <summary>
        /// Filters out articles that are not in the main namespace
        /// </summary>
        public void FilterNonMainArticles()
        {
            //filter out non-mainspace articles

            if (this.InvokeRequired)
            {
                this.Invoke(new FilterNM(FilterNonMainArticles));
                return;
            }

            int i = 0;
            string s = "";

            lbArticles.BeginUpdate();
            while (i < lbArticles.Items.Count)
            {
                s = lbArticles.Items[i].ToString();

                if (!Tools.IsMainSpace(s))
                    lbArticles.Items.Remove(lbArticles.Items[i]);
                else //move on
                    i++;
            }
            lbArticles.EndUpdate();
            UpdateNumberOfArticles();
        }

        /// <summary>
        /// Alphabetically sorts the list
        /// </summary>
        public void AlphaSortList()
        {
            lbArticles.Sorted = true;
            lbArticles.Sorted = false;
        }

        /// <summary>
        /// Replaces one article in the list with another, in the same place
        /// </summary>
        public void ReplaceArticle(Article OldArticle, Article NewArticle)
        {
            int intPos = 0;
            intPos = lbArticles.Items.IndexOf(OldArticle);

            Remove(OldArticle);
            lbArticles.Items.Insert(intPos, NewArticle);

            lbArticles.ClearSelected();
            lbArticles.SelectedItem = NewArticle;
        }

        /// <summary>
        /// Stops the processes
        /// </summary>
        public void Stop()
        {
            if (ListerThread != null)
                ListerThread.Abort();

            StopProgressBar();
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdateNumberOfArticles()
        {
            lblNumberOfArticles.Text = lbArticles.Items.Count.ToString() + " page";
            if (lbArticles.Items.Count != 1)
                lblNumberOfArticles.Text += "s";
            if (NoOfArticlesChanged != null)
                this.NoOfArticlesChanged(null, null);

            if (AutoAlpha)
                AlphaSortList();
        }

        /// <summary>
        /// Converts the list to equivalent talk page
        /// </summary>
        public void ConvertToTalkPages()
        {
            List<Article> list = GetArticleList();
            list = GetLists.ConvertToTalk(list);
            lbArticles.Items.Clear();
            Add(list);
        }

        /// <summary>
        /// Converts the list to equivalent non-talk page
        /// </summary>
        public void ConvertFromTalkPages()
        {
            List<Article> list = GetArticleList();
            list = GetLists.ConvertFromTalk(list);
            lbArticles.Items.Clear();
            Add(list);
        }

        #endregion

        #region Context menu

        private void filterOutNonMainSpaceArticlesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FilterNonMainArticles();
        }

        private void specialFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filter();
        }

        private void sortAlphebeticallyMenuItem_Click(object sender, EventArgs e)
        {
            AlphaSortList();
        }

        private void saveListToTextFileToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SaveList();
        }

        private void selectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveSelectedArticle();
        }

        private void duplicatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            removeListDuplicates();
        }

        private void convertToTalkPagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConvertToTalkPages();
        }

        private void convertFromTalkPagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConvertFromTalkPages();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tools.Copy(lbArticles);
            RemoveSelectedArticle();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tools.Copy(lbArticles);
        }


        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string url = Variables.URL + "/wiki/";

                string textTba = Clipboard.GetDataObject().GetData(DataFormats.UnicodeText).ToString();
                string[] splitter = { "\r\n", "|" };

                string[] splitTextTBA = textTba.Split(splitter, StringSplitOptions.RemoveEmptyEntries);

                foreach (string entry in splitTextTBA)
                {
                    if (Regex.Match(entry, url).Success)
                        Add(entry.Replace(url, ""));
                    else if (!string.IsNullOrEmpty(entry.Trim()))
                        Add(entry);
                }
            }
            catch { }
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lbArticles.BeginUpdate();

            for (int i = 0; i != lbArticles.Items.Count; i++)
                lbArticles.SetSelected(i, true);

            lbArticles.EndUpdate();
        }

        private void invertSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lbArticles.BeginUpdate();

            for (int i = 0; i != lbArticles.Items.Count; i++)
                lbArticles.SetSelected(i, !lbArticles.GetSelected(i));

            lbArticles.EndUpdate();
        }

        private void selectNoneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lbArticles.BeginUpdate();

            for (int i = 0; i != lbArticles.Items.Count; i++)
                lbArticles.SetSelected(i, false);

            lbArticles.EndUpdate();
        }

        private void fromCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] c = new string[lbArticles.SelectedItems.Count];

            int i = 0;
            foreach (object o in lbArticles.SelectedItems)
            {
                if (o.ToString().Contains(Variables.Namespaces[14]))
                {
                    c[i] = o.ToString().Substring(Variables.Namespaces[14].Length);
                    i++;
                }
            }
            if (i > 0)
                MakeList(SourceType.Category, c);
        }

        private void fromWhatlinkshereToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] c = new string[lbArticles.SelectedItems.Count];

            int i = 0;
            foreach (object o in lbArticles.SelectedItems)
            {
                c[i] = o.ToString();
                i++;
            }

            MakeList(SourceType.WhatLinksHere, c);
        }

        private void fromTranscludesHereToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] c = new string[lbArticles.SelectedItems.Count];

            int i = 0;
            foreach (object o in lbArticles.SelectedItems)
            {
                c[i] = o.ToString();
                i++;
            }

            MakeList(SourceType.WhatTranscludesPage, c);
        }

        private void fromLinksOnPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] c = new string[lbArticles.SelectedItems.Count];

            int i = 0;
            foreach (object o in lbArticles.SelectedItems)
            {
                c[i] = o.ToString();
                i++;
            }

            MakeList(SourceType.LinksOnPage, c);
        }

        private void fromImageLinksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] c = new string[lbArticles.SelectedItems.Count];

            int i = 0;
            foreach (object o in lbArticles.SelectedItems)
            {
                c[i] = o.ToString();
                i++;
            }

            MakeList(SourceType.ImageFileLinks, c);
        }

        private void fromRedirectsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] c = new string[lbArticles.SelectedItems.Count];

            int i = 0;
            foreach (object o in lbArticles.SelectedItems)
            {
                c[i] = o.ToString();
                i++;
            }

            MakeList(SourceType.Redirects, c);
        }

        private void clearToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void openInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((lbArticles.SelectedItems.Count < 10) || (MessageBox.Show("Opening " + lbArticles.SelectedItems.Count + " articles in your browser at once could cause your system to run slowly, and even stop responding.\r\nAre you sure you want to continue?", "Continue?", MessageBoxButtons.YesNo) == DialogResult.Yes))
                loadArticlesInBrowser();
        }

        private void loadArticlesInBrowser()
        {
            foreach (Article item in lbArticles.SelectedItems)
            {
                try
                {
                    Tools.OpenArticleInBrowser(item.Name);
                }
                catch { }
            }
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveSelectedArticle();
        }
        #endregion

        private void btnRemoveDuplicates_Click(object sender, EventArgs e)
        {
            removeListDuplicates();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            btnStop.Visible = false;
            this.Stop();
        }

        private void openHistoryInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((lbArticles.SelectedItems.Count < 10) || (MessageBox.Show("Opening " + lbArticles.SelectedItems.Count + " articles in your browser at once could cause your system to run slowly, and even stop responding.\r\nAre you sure you want to continue?", "Continue?", MessageBoxButtons.YesNo) == DialogResult.Yes))
                loadArticleHistoryInBrowser();
        }

        private void loadArticleHistoryInBrowser()
        {
            foreach (Article item in lbArticles.SelectedItems)
                Tools.OpenArticleHistoryInBrowser(item.Name);
        }

        private void lbArticles_DoubleClick(object sender, System.EventArgs e)
        {
            loadArticlesInBrowser();
        }

        private void moveToTopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MoveSelectedItems(0);
        }

        private void moveToBottomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MoveSelectedItems(lbArticles.Items.Count - 1);
        }

        /// <summary>
        /// Moves the currently selected items in the listbox to the position selected
        /// </summary>
        /// <param name="toIndex">Index to move items to</param>
        private void MoveSelectedItems(int toIndex)
        {
            bool toTop = (toIndex == 0);
            lbArticles.BeginUpdate();

            foreach (Article a in GetSelectedArticleList())
            {
                lbArticles.Items.Remove(a);
                lbArticles.Items.Insert(toIndex, a);

                if (toTop)
                    toIndex++;
            }
            lbArticles.EndUpdate();
        }

        public AWBSettings.SpecialFilterPrefs SpecialFilterSettings
        {
            get { return SpecialFilter.Settings; }
            set { SpecialFilter.Settings = value; }
        }
    }
}

namespace WikiFunctions.Lists
{
    public enum SourceType { None = -1, Category, CategoryRecursive, WhatLinksHere, WhatTranscludesPage, LinksOnPage, ImagesOnPage, TransclusionsOnPage, TextFile, GoogleWikipedia, UserContribs, AllUserContribs, SpecialPage, ImageFileLinks, DatabaseDump, MyWatchlist, WikiSearch, Redirects }
}