/*
ListMaker
(C) Martin Richards
(C) Stephen Kennedy, Sam Reed 2008

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
        private static SaveFileDialog saveListDialog;
        private static BindingList<IListProvider> listItems = new BindingList<IListProvider>();
        ListFilterForm SpecialFilter;

        //used to keep easy track of providers for add/remove/use in code
        #region ListProviders
        private static IListProvider redirectLProvider = new RedirectsListProvider();
        private static IListProvider categoryLProvider = new CategoryListProvider();
        private static IListProvider categoryRecursiveLProvider = new CategoryRecursiveListProvider();
        private static IListProvider whatLinksLProvider = new WhatLinksHereListProvider();
        private static IListProvider whatTranscludesLProvider = new WhatTranscludesPageListMakerProvider();
        private static IListProvider linksOnPageLProvider = new LinksOnPageListProvider();
        private static IListProvider imageFileLinksLProvider = new ImageFileLinksListProvider();
        private static IListProvider imagesOnPageLProvider = new ImagesOnPageListProvider();
        private static IListProvider categoriesOnPageLProvider = new CategoriesOnPageListProvider();
        #endregion

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

        static ListMaker()
        {
            saveListDialog = new SaveFileDialog();
            saveListDialog.DefaultExt = "txt";
            saveListDialog.Filter = "Text file with wiki markup|*.txt|Plaintext list|*.txt|CSV (Comma Seperated Values)|*.txt|CSV with Wikitext|*.txt";
            saveListDialog.Title = "Save article list";
        }

        public ListMaker()
        {
            InitializeComponent();

            if (listItems.Count == 0)
            {
                listItems.Add(categoryLProvider);
                listItems.Add(categoryRecursiveLProvider);
                listItems.Add(new CategoryRecursiveOneLevelListProvider());
                listItems.Add(new CategoryRecursiveUserDefinedLevelListProvider());
                listItems.Add(categoriesOnPageLProvider);
                listItems.Add(whatLinksLProvider);
                listItems.Add(new WhatLinksHereIncludingRedirectsListProvider());
                listItems.Add(whatTranscludesLProvider);
                listItems.Add(linksOnPageLProvider);
                listItems.Add(imagesOnPageLProvider);
                listItems.Add(new TransclusionsOnPageListProvider());
                listItems.Add(new TextFileListProvider());
                listItems.Add(new GoogleSearchListProvider());
                listItems.Add(new UserContribsListProvider());
                listItems.Add(new SpecialPageListProvider());
                listItems.Add(imageFileLinksLProvider);
                listItems.Add(new DatabaseScannerListProvider(this));
                listItems.Add(new MyWatchlistListProvider());
                listItems.Add(new WikiSearchListProvider());
                listItems.Add(new RandomPagesListProvider());
                listItems.Add(redirectLProvider);
                listItems.Add(new NewPagesListProvider());
            }

            SpecialFilter = new ListFilterForm(lbArticles);

            // We'll manage our own collection of list items:
            cmboSourceSelect.DataSource = listItems;
            // Bind IListMakerProvider.DisplayText to be the displayed text:
            cmboSourceSelect.DisplayMember = "DisplayText";
            cmboSourceSelect.ValueMember = "DisplayText";
        }

        new public static void Refresh() { }

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
            UpdateNumberOfArticles();
        }

        /// <summary>
        /// Clears the list
        /// </summary>
        public void Clear()
        {
            lbArticles.Items.Clear();
            UpdateNumberOfArticles();
        }

        /// <summary>
        /// Removes/Adds the "Redirects" option from/to the list
        /// </summary>
        public static void AddRemoveRedirects()
        {
            if (Variables.LangCode != LangCodeEnum.en)
                listItems.Remove(redirectLProvider);
            else if (!listItems.Contains(redirectLProvider))
                listItems.Add(redirectLProvider);
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
            return Remove(new Article(Title));
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

                txtPage.Text = item.Name;

                int intPosition = lbArticles.Items.IndexOf(item);

                lbArticles.Items.Remove(item);

                if (lbArticles.Items.Count == intPosition)
                    intPosition--;

                if (lbArticles.Items.Count > 0)
                    lbArticles.SelectedIndex = intPosition;

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
            UpdateNumberOfArticles();
        }

        /// <summary>
        /// Inserts the given article at a specific index
        /// </summary>
        public void Insert(int index, string item)
        {
            Article a = new Article(item);
            lbArticles.Items.Insert(index, a);
            UpdateNumberOfArticles();
        }

        /// <summary>
        /// Removes article at the given index
        /// </summary>
        public void RemoveAt(int index)
        {
            lbArticles.Items.RemoveAt(index);
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

            IListProvider searchItem = (IListProvider)cmboSourceSelect.SelectedItem;

            searchItem.Selected();
            lblUserInput.Text = searchItem.UserInputTextBoxText;
            UserInputTextBox.Enabled = searchItem.UserInputTextBoxEnabled;
            tooltip.SetToolTip(cmboSourceSelect, searchItem.DisplayText);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtPage.Text.Trim().Length == 0)
                return;

            Add(NormalizeTitle(txtPage.Text));
            txtPage.Text = "";
        }

        private void btnRemoveArticle_Click(object sender, EventArgs e)
        {
            RemoveSelectedArticle();
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            Filter();
        }

        private void btnMakeList_Click(object sender, EventArgs e)
        {
            UserInputTextBox.Text = UserInputTextBox.Text.Trim();

            //make sure there is some text.
            if (UserInputTextBox.Enabled && string.IsNullOrEmpty(UserInputTextBox.Text))
            {
                MessageBox.Show("Please enter some text", "No text", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            UserInputTextBox.AutoCompleteCustomSource.Add(UserInputTextBox.Text);

            MakeList();
        }

        private void lbArticles_MouseMove(object sender, MouseEventArgs e)
        {
            string strTip = "";

            //Get the item
            int nIdx = lbArticles.IndexFromPoint(e.Location);
            if ((nIdx >= 0) && (nIdx < lbArticles.Items.Count))
                strTip = lbArticles.Items[nIdx].ToString();
            if (strTip != strlbArticlesTooltip)
                tooltip.SetToolTip(lbArticles, strTip);
            strlbArticlesTooltip = strTip;
        }

        private void txtNewArticle_MouseMove(object sender, MouseEventArgs e)
        {
            if (txtPage.Text != strtxtNewArticleTooltip)
                tooltip.SetToolTip(txtPage, txtPage.Text);
            strtxtNewArticleTooltip = txtPage.Text;
        }

        private void lbArticles_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                btnRemove.PerformClick();
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
                btnGenerate.PerformClick();
        }

        private void mnuListBox_Opening(object sender, CancelEventArgs e)
        {
            // No selected pages
            openInBrowserToolStripMenuItem.Enabled =
            openHistoryInBrowserToolStripMenuItem.Enabled =
            cutToolStripMenuItem.Enabled =
            copyToolStripMenuItem.Enabled =
                //  Remove menu
                selectedToolStripMenuItem.Enabled =
            addSelectedToListToolStripMenuItem.Enabled =
            moveToTopToolStripMenuItem.Enabled =
            moveToBottomToolStripMenuItem.Enabled =
            (lbArticles.SelectedItem != null);

            // Single page
            specialFilterToolStripMenuItem.Enabled =
            sortAlphaMenuItem.Enabled =
            (lbArticles.Items.Count > 1);

            // No pages
            selectMnu.Enabled =
            removeToolStripMenuItem.Enabled =
            convertToTalkPagesToolStripMenuItem.Enabled =
            convertFromTalkPagesToolStripMenuItem.Enabled =
            saveListToFileToolStripMenuItem.Enabled =
            (lbArticles.Items.Count > 0);
        }

        private void txtNewArticle_DoubleClick(object sender, EventArgs e)
        {
            txtPage.SelectAll();
        }

        private void txtSelectSource_DoubleClick(object sender, EventArgs e)
        {
            UserInputTextBox.SelectAll();
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets the ListBox that holds the list of articles
        /// </summary>
        public ListBox Items
        { get { return lbArticles; } }

        /// <summary>
        /// Sets whether the buttons are enabled or not
        /// </summary>
        public bool ButtonsEnabled
        {
            set
            {
                btnFilter.Enabled = btnRemove.Enabled = saveListToFileToolStripMenuItem.Enabled = value;
            }
        }

        /// <summary>
        /// Gets or sets the selected index
        /// </summary>
        public int SelectedSource
        {
            get { return cmboSourceSelect.SelectedIndex; }
            set
            {
                if (value < (cmboSourceSelect.Items.Count - 1))
                {
                    cmboSourceSelect.SelectedIndex = value;
                    cmboSourceSelect_SelectedIndexChanged(null, null);
                }
            }
        }

        /// <summary>
        /// Gets or sets the source text
        /// </summary>
        public string SourceText
        {
            get { return UserInputTextBox.Text; }
            set { UserInputTextBox.Text = value; }
        }

        /// <summary>
        /// Set whether the make button is enabled
        /// </summary>
        public bool MakeListEnabled
        {
            set { btnGenerate.Enabled = value; }
        }

        /// <summary>
        /// Gets the number of articles in the list
        /// </summary>
        public int NumberOfArticles
        {
            get { return lbArticles.Items.Count; }
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

        /// <summary>
        /// The file the list was made from
        /// </summary>
        static string strListFile = "";

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
        /// <summary>
        /// Removes URL and such if applicable
        /// </summary>
        public string NormalizeTitle(string s)
        {
            //// Convert from the escaped UTF-8 byte code into Unicode
            //s = System.Web.HttpUtility.UrlDecode(s);
            //// Convert secure URLs into non-secure equivalents (note the secure system is considered a 'hack')
            //s = Parse.Parsers.ExtToInt1.Replace(s, "http://$2.$1.org/");
            //// Convert http://lang.domain.org/wiki/ into interwiki format
            //s = Parse.Parsers.ExtToInt2.Replace(s, "$2:$1:$3");
            //// Scripts paths (/w/index.php?...) into interwiki format
            //s = Parse.Parsers.ExtToInt3.Replace(s, "$2:$1:$3");
            //// Remove [[brackets]] from link
            //s = Parse.Parsers.ExtToInt4.Replace(s, "$1");
            //// '_' -> ' ' and hard coded home wiki
            //s = Parse.Parsers.ExtToInt5.Replace(s, "$3");
            //// Use short prefix form (wiktionary:en:Wiktionary:Main Page -> wikt:en:Wiktionary:Main Page)
            //return Parse.Parsers.ExtToInt6.Replace(s, "$1$2$3$4$5$6$7$8$9");

            // Assumsuption flaw: that all wikis use /wiki/ as the default path
            string url = Variables.URL + "/wiki/";
            if (Regex.Match(s, url).Success)
                return s.Replace(url, "");
            else
                return s;
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

            lbArticles.Items.Add(new Article(Tools.TurnFirstToUpper(Tools.RemoveSyntax(s))));
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
            if (l == null)
                return;

            if (this.InvokeRequired)
            {
                this.Invoke(new AddDel(Add), l);
                return;
            }

            lbArticles.BeginUpdate();
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
            btnGenerate.Enabled = false;
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
            btnGenerate.Enabled = true;
            Status = "List complete!";
            BusyStatus = false;
            btnStop.Visible = false;
            UpdateNumberOfArticles();

            if (ListFinished != null)
                ListFinished(null, null);
        }

        Thread ListerThread = null;

        public void MakeList()
        {
            MakeList((IListProvider)cmboSourceSelect.SelectedItem, UserInputTextBox.Text.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
        }

        /// <summary>
        /// Makes a list of pages
        /// </summary>
        /// <param name="ST">The type of list to create</param>
        /// <param name="SourceValues">An array of string values to create the list with, e.g. an array of categories. Use null if not appropriate</param>
        public void MakeList(IListProvider provider, string[] sourceValues)
        {
            btnStop.Visible = true;

            providerToRun = provider;

            if (providerToRun.RunOnSeparateThread)
            {
                strSource = sourceValues;
                ThreadStart thr_Process = new ThreadStart(MakeListPlugin);
                ListerThread = new Thread(thr_Process);
                ListerThread.SetApartmentState(ApartmentState.STA);
                ListerThread.IsBackground = true;
                ListerThread.Start();
            }
            else
            {
                BusyStatus = true;

                if (!provider.UserInputTextBoxEnabled)
                    Add(providerToRun.MakeList(new string[0]));
                else
                    Add(providerToRun.MakeList(sourceValues));

                BusyStatus = false;
                UpdateNumberOfArticles();
                btnStop.Visible = false;
            }
        }

        string[] strSource;
        WikiFunctions.Lists.IListProvider providerToRun;

        private void MakeListPlugin()
        {
            Thread.CurrentThread.Name = "ListMaker (" + providerToRun.GetType().Name + ": "
                + UserInputTextBox.Text + ")";
            StartProgressBar();

            try
            {
                Add(providerToRun.MakeList(strSource));
            }
            catch (ThreadAbortException) { }
            catch (PageDoesNotExistException ex)
            {
                MessageBox.Show(ex.Message, "Page does not exist", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                ErrorHandler.ListMakerText = UserInputTextBox.Text;
                ErrorHandler.Handle(ex);
                ErrorHandler.ListMakerText = "";
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
                int i = lbArticles.SelectedIndex;

                if (lbArticles.SelectedItems.Count > 0)
                    txtPage.Text = lbArticles.SelectedItem.ToString();

                while (lbArticles.SelectedItems.Count > 0)
                    lbArticles.Items.Remove(lbArticles.SelectedItem);

                if (lbArticles.Items.Count > i)
                    lbArticles.SelectedIndex = i;
                else
                    lbArticles.SelectedIndex = i - 1;
            }
            catch { }

            lbArticles.EndUpdate();

            UpdateNumberOfArticles();
        }

        /// <summary>
        /// Opens the dialog to filter out articles
        /// </summary>
        public void Filter()
        {
            SpecialFilter.ShowDialog(this);
        }

        /// <summary>
        /// Automatically removes all duplicates from the list
        /// </summary>
        public void removeListDuplicates()
        {
            SpecialFilter.Clear();
            SpecialFilter.RemoveDuplicates();

            UpdateNumberOfArticles();
        }

        /// <summary>
        /// Saves the list box of the current ListMaker to the specified text file.
        /// </summary>
        public void SaveList()
        {
            SaveList(lbArticles);
        }

        /// <summary>
        /// Saves the list from the passed ListBox2 to the specified text file.
        /// </summary>
        public static void SaveList(ListBox2 ArticlesListBox)
        {
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
                            foreach (Article a in ArticlesListBox)
                                strList.AppendLine("# [[:" + a.Name + "]]");
                            break;
                        case 2: //plaintext
                            foreach (Article a in ArticlesListBox)
                                strList.AppendLine(a.Name);
                            break;
                        case 3: //CSV
                            foreach (Article a in ArticlesListBox)
                                strList.Append(a.Name + ", ");
                            strList = strList.Remove(strList.Length - 2, 2);
                            break;
                        case 4: //CSV with wikitext
                            foreach (Article a in ArticlesListBox)
                                strList.Append("[[:" + a.Name + "]], ");
                            strList = strList.Remove(strList.Length - 2, 2);
                            break;
                    }
                    strListFile = saveListDialog.FileName;
                    sw = new StreamWriter(strListFile, false, Encoding.UTF8);
                    sw.Write(strList);
                    sw.Close();
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
            lbArticles.Sort();
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
        /// Updates the Number of Articles
        /// </summary>
        public void UpdateNumberOfArticles()
        {
            lblNumOfPages.Text = lbArticles.Items.Count.ToString() + " page";
            if (lbArticles.Items.Count != 1)
                lblNumOfPages.Text += "s";
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
            lbArticles.Items.Clear();
            Add(Tools.ConvertToTalk(list));
        }

        /// <summary>
        /// Converts the list to equivalent non-talk page
        /// </summary>
        public void ConvertFromTalkPages()
        {
            List<Article> list = GetArticleList();
            lbArticles.Items.Clear();
            Add(Tools.ConvertFromTalk(list));
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
            lbArticles.BeginUpdate();
            try
            {
                string textTba = Clipboard.GetDataObject().GetData(DataFormats.UnicodeText).ToString();

                string[] splitTextTBA = textTba.Split(new string[] { "\r\n", "|" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string entry in splitTextTBA)
                {
                    if (!string.IsNullOrEmpty(entry.Trim()))
                        Add(NormalizeTitle(entry));
                }
            }
            catch { }
            lbArticles.EndUpdate();
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

        private void categoriesOnPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddFromSelectedList(categoriesOnPageLProvider);
        }

        private void fromCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddFromSelectedList(categoryLProvider);
        }

        private void fromCategoryrecursiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddFromSelectedList(categoryRecursiveLProvider);
        }

        private void fromWhatlinkshereToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddFromSelectedList(whatLinksLProvider);
        }

        private void fromTranscludesHereToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddFromSelectedList(whatTranscludesLProvider);
        }

        private void fromLinksOnPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddFromSelectedList(linksOnPageLProvider);
        }

        private void fromImageLinksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddFromSelectedList(imageFileLinksLProvider);
        }

        private void imagesOnPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddFromSelectedList(imagesOnPageLProvider);
        }

        private void fromRedirectsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddFromSelectedList(redirectLProvider);
        }

        private void AddFromSelectedList(IListProvider provider)
        {
            string[] c = new string[lbArticles.SelectedItems.Count];

            int i = 0;
            foreach (object o in lbArticles.SelectedItems)
            {
                c[i] = o.ToString();
                i++;
            }

            if (i > 0)
                MakeList(provider, c);
        }

        private void clearToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (lbArticles.Items.Count <= 100 || (MessageBox.Show(
            "Are you sure you want to clear the large list?", "Clear?", MessageBoxButtons.YesNo)
            == DialogResult.Yes))
                Clear();
        }

        private void openInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((lbArticles.SelectedItems.Count < 10) || (MessageBox.Show("Opening " + lbArticles.SelectedItems.Count + " articles in your browser at once could cause your system to run slowly, and even stop responding.\r\nAre you sure you want to continue?", "Continue?", MessageBoxButtons.YesNo) == DialogResult.Yes))
                loadArticlesInBrowser();
        }

        private void loadArticlesInBrowser()
        {
            try
            {
                foreach (Article item in lbArticles.SelectedItems)
                {
                    try { Tools.OpenArticleInBrowser(item.Name); }
                    catch { }
                }
            }
            catch { }
        }
        #endregion

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

        /// <summary>
        /// Get/Set the Special Filter settings
        /// </summary>
        [Browsable(false)]
        [Localizable(false)]
        public AWBSettings.SpecialFilterPrefs SpecialFilterSettings
        {
            get { return SpecialFilter.Settings; }
            set { SpecialFilter.Settings = value; }
        }

        /// <summary>
        /// Add a IListMakerProvider or a IListMakerPlugin to all ListMakers
        /// </summary>
        /// <param name="provider">IListMakerProvider/IListMakerPlugin to add</param>
        public static void AddProvider(IListProvider provider)
        {
            listItems.Add(provider);
        }

        /// <summary>
        /// Gets the Number of IListMakerPlugins currently loaded
        /// </summary>
        /// <returns>Number of IListmakerPlugins loaded into the ListMaker</returns>
        public static int GetListMakerPluginCount()
        {
            return GetListMakerPlugins().Count;
        }

        /// <summary>
        /// Gets a list of all the IListMakerPlugin Names
        /// </summary>
        /// <returns>List of IListMakerPlugin Names</returns>
        public static List<string> GetListMakerPluginNames()
        {
            List<string> ret = new List<string>();

            foreach (WikiFunctions.Plugin.IListMakerPlugin p in GetListMakerPlugins())
            {
                ret.Add(p.Name);
            }

            return ret;
        }

        /// <summary>
        /// Gets all the IListMakerPlugins from the list of IListMakerProviders
        /// </summary>
        /// <returns>List of IListMakerPlugins currently loaded</returns>
        public static List<WikiFunctions.Plugin.IListMakerPlugin> GetListMakerPlugins()
        {
            List<WikiFunctions.Plugin.IListMakerPlugin> plugins = new List<WikiFunctions.Plugin.IListMakerPlugin>();

            foreach (IListProvider p in listItems)
            {
                WikiFunctions.Plugin.IListMakerPlugin plugin = (p as WikiFunctions.Plugin.IListMakerPlugin);

                if (plugin != null)
                    plugins.Add(plugin);
            }

            return plugins;
        }

        /// <summary>
        /// Returns a new DatabaseScanner tied to an instance of the current Articles List Box
        /// </summary>
        /// <returns></returns>
        public WikiFunctions.DBScanner.DatabaseScanner DBScanner()
        {
            return new WikiFunctions.DBScanner.DatabaseScanner(this);
        }
    }
}

namespace WikiFunctions.Lists
{
    // TODO: Document what I do
    [Serializable]
    public sealed class PageDoesNotExistException : ApplicationException
    {
        public PageDoesNotExistException() { }
        public PageDoesNotExistException(string message)
            : base(message) { }
        public PageDoesNotExistException(string message, System.Exception inner)
            : base(message, inner) { }
        private PageDoesNotExistException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
