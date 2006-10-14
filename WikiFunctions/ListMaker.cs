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

namespace WikiFunctions.Lists
{
    public enum SourceType { Category, WhatLinksHere, WhatTranscludesHere, LinksOnPage, TextFile, GoogleWikipedia, UserContribs, SpecialPage, ImageFileLinks, DatabaseDump, MyWatchlist, WikiSearch }

    public delegate void ListMakerEventHandler();

    public partial class ListMaker : UserControl, IEnumerable<Article>, ICollection<Article>, IList<Article>
    {
        public event ListMakerEventHandler StatusTextChanged;
        public event ListMakerEventHandler BusyStateChanged;
        public event ListMakerEventHandler NoOfArticlesChanged;
        /// <summary>
        /// Occurs when a list has been created
        /// </summary>
        public event ListMakerEventHandler ListFinished;

        public ListMaker()
        {
            InitializeComponent();
        }

        new public void Refresh()
        { }

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
            get
            {
                return (Article)lbArticles.Items[index];
            }
            set
            {
                lbArticles.Items[index] = value;
            }
        }

        #endregion

        #region Events

        private void cmboSourceSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (SelectedSource)
            {
                case SourceType.Category:
                    lblSourceSelect.Text = Variables.Namespaces[14];
                    txtSelectSource.Enabled = true;
                    chkWLHRedirects.Visible = false;
                    break;
                case SourceType.WhatLinksHere:
                    lblSourceSelect.Text = "What links to";
                    txtSelectSource.Enabled = true;
                    chkWLHRedirects.Visible = false;
                    break;
                case SourceType.WhatTranscludesHere:
                    lblSourceSelect.Text = "What embeds";
                    txtSelectSource.Enabled = true;
                    chkWLHRedirects.Visible = false;
                    break;
                case SourceType.LinksOnPage:
                    lblSourceSelect.Text = "Links on";
                    txtSelectSource.Enabled = true;
                    chkWLHRedirects.Visible = false;
                    break;
                case SourceType.TextFile:
                    lblSourceSelect.Text = "From file";
                    txtSelectSource.Enabled = false;
                    chkWLHRedirects.Visible = false;
                    break;
                case SourceType.GoogleWikipedia:
                    lblSourceSelect.Text = "Google search";
                    txtSelectSource.Enabled = true;
                    chkWLHRedirects.Visible = false;
                    break;
                case SourceType.UserContribs:
                    lblSourceSelect.Text = Variables.Namespaces[2];
                    txtSelectSource.Enabled = true;
                    chkWLHRedirects.Visible = false;
                    break;
                case SourceType.SpecialPage:
                    lblSourceSelect.Text = Variables.Namespaces[-1];
                    txtSelectSource.Enabled = true;
                    chkWLHRedirects.Visible = false;
                    break;
                case SourceType.ImageFileLinks:
                    lblSourceSelect.Text = Variables.Namespaces[6];
                    txtSelectSource.Enabled = true;
                    chkWLHRedirects.Visible = false;
                    break;
                case SourceType.WikiSearch:
                    lblSourceSelect.Text = "Wiki search";
                    txtSelectSource.Enabled = true;
                    chkWLHRedirects.Visible = false;
                    break;
                default:
                    lblSourceSelect.Text = "";
                    txtSelectSource.Enabled = false;
                    chkWLHRedirects.Visible = false;
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
            SourceType ST = SelectedSource;

            txtSelectSource.Text = txtSelectSource.Text.Trim('[', ']');
            if (ST == SourceType.Category)
                txtSelectSource.Text = Regex.Replace(txtSelectSource.Text, "^" + Variables.Namespaces[14], "", RegexOptions.IgnoreCase);
            else if (ST == SourceType.UserContribs)
                txtSelectSource.Text = Regex.Replace(txtSelectSource.Text, "^" + Variables.Namespaces[2], "", RegexOptions.IgnoreCase);
            else if (ST == SourceType.SpecialPage)
                txtSelectSource.Text = Regex.Replace(txtSelectSource.Text, "^" + Variables.Namespaces[-1], "", RegexOptions.IgnoreCase);
            else if (ST == SourceType.ImageFileLinks)
                txtSelectSource.Text = Regex.Replace(txtSelectSource.Text, "^" + Variables.Namespaces[6], "", RegexOptions.IgnoreCase);

            txtSelectSource.Text = Tools.TurnFirstToUpper(txtSelectSource.Text);
            txtSelectSource.AutoCompleteCustomSource.Add(txtSelectSource.Text);

            //make sure there is some text.
            if (txtSelectSource.Text.Length == 0 && txtSelectSource.Enabled)
            {
                MessageBox.Show("Please enter some text", "No text", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string[] s = txtSelectSource.Text.Split('|');

            MakeList(ST, s);
        }

        private void lbArticles_MouseMove(object sender, MouseEventArgs e)
        {
            string strTip = "";

            //Get the item
            int nIdx = lbArticles.IndexFromPoint(e.Location);
            if ((nIdx >= 0) && (nIdx < lbArticles.Items.Count))
                strTip = lbArticles.Items[nIdx].ToString();

            toolTip1.SetToolTip(lbArticles, strTip);
        }

        private void txtNewArticle_MouseMove(object sender, MouseEventArgs e)
        {
            toolTip1.SetToolTip(txtNewArticle, txtNewArticle.Text);
        }

        private void lbArticles_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                btnRemoveArticle.PerformClick();
        }

        private void txtNewArticle_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                e.Handled = true;
                btnAdd.PerformClick();
            }
        }

        private void txtSelectSource_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                e.Handled = true;
                btnMakeList.PerformClick();
            }
        }

        private void mnuListBox_Opening(object sender, CancelEventArgs e)
        {
            bool boolEnabled = lbArticles.Items.Count > 0;

            addSelectedToListToolStripMenuItem.Enabled = lbArticles.SelectedItems.Count > 0;

            removeToolStripMenuItem.Enabled = lbArticles.SelectedItem != null;
            clearToolStripMenuItem1.Enabled = boolEnabled;
            filterOutNonMainSpaceArticlesToolStripMenuItem.Enabled = boolEnabled;
            convertToTalkPagesToolStripMenuItem.Enabled = boolEnabled;
            convertFromTalkPagesToolStripMenuItem.Enabled = boolEnabled;
            sortAlphebeticallyMenuItem.Enabled = boolEnabled;
            saveListToTextFileToolStripMenuItem1.Enabled = boolEnabled;
            specialFilterToolStripMenuItem.Enabled = boolEnabled;

            openInBrowserToolStripMenuItem.Enabled = lbArticles.SelectedItems.Count == 1;
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
                btnFilter.Enabled = value;
                btnRemoveArticle.Enabled = value;
                btnArticlesListClear.Enabled = value;
                btnArticlesListSave.Enabled = value;
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
                    this.StatusTextChanged();
            }
        }

        bool bBusyStatus = false;
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
                    this.BusyStateChanged();
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
            WikiFunctions.DatabaseScanner.DatabaseScanner ds = new WikiFunctions.DatabaseScanner.DatabaseScanner(lbArticles);
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
            s = Tools.TurnFirstToUpper(s);
            Article a = new Article(Tools.RemoveSyntax(s));
            lbArticles.Items.Add(a);
            UpdateNumberOfArticles();
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

            foreach (Article a in l)
            {
                if (!lbArticles.Items.Contains(a))
                    lbArticles.Items.Add(a);
            }

            lbArticles.EndUpdate();

            UpdateNumberOfArticles();
        }

        /// <summary>
        /// Returns the list of articles
        /// </summary>
        private List<Article> ArticleListFromListBox()
        {
            List<Article> list = new List<Article>();

            foreach (Article a in lbArticles)
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
            UpdateNumberOfArticles();

            if (ListFinished != null)
                ListFinished();
        }

        Thread ListerThread = null;
        /// <summary>
        /// Makes a list of pages
        /// </summary>
        /// <param name="ST">The type of list to create</param>
        /// <param name="SourceValues">An array of string values to create the list with, e.g. an array of categories. Use null if not appropriate</param>
        public void MakeList(SourceType ST, string[] SourceValues)
        {
            if (ST == SourceType.DatabaseDump)
            {
                launchDumpSearcher();
                return;
            }
            else if (ST == SourceType.TextFile)
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
                    MessageBox.Show(ex.Message);
                }

                return;
            }
            else if (ST == SourceType.MyWatchlist)
            {
                BusyStatus = true;
                Add(GetLists.FromWatchList());
                BusyStatus = false;
                UpdateNumberOfArticles();
                return;
            }
            else
            {
                Source = ST;
                strSouce = SourceValues;

                ThreadStart thr_Process = new ThreadStart(MakeList2);
                ListerThread = new Thread(thr_Process);
                ListerThread.IsBackground = true;
                ListerThread.Start();
            }
        }

        SourceType Source = SourceType.Category;
        string[] strSouce;

        private void MakeList2()
        {
            Saved = false;
            StartProgressBar();

            try
            {
                switch (Source)
                {
                    case SourceType.Category:
                        Add(GetLists.FromCategory(false, strSouce));
                        break;
                    case SourceType.WhatLinksHere:
                        Add(GetLists.FromWhatLinksHere(false, strSouce));
                        break;
                    case SourceType.WhatTranscludesHere:
                        Add(GetLists.FromWhatLinksHere(true, strSouce));
                        break;
                    case SourceType.LinksOnPage:
                        Add(GetLists.FromLinksOnPage(strSouce));
                        break;
                    //4 from text file
                    case SourceType.GoogleWikipedia:
                        Add(GetLists.FromGoogleSearch(strSouce));
                        break;
                    case SourceType.UserContribs:
                        Add(GetLists.FromUserContribs(strSouce));
                        break;
                    case SourceType.SpecialPage:
                        Add(GetLists.FromSpecialPage(strSouce));
                        break;
                    case SourceType.ImageFileLinks:
                        Add(GetLists.FromImageLinks(strSouce));
                        break;
                    //9 from datadump
                    //10 from watchlist
                    case SourceType.WikiSearch:
                        Add(GetLists.FromWikiSearch(strSouce));
                        break;
                    default:
                        break;
                }
            }
            catch (ThreadAbortException)
            {
            }
            catch (PageDoeNotExistException ex)
            {
                MessageBox.Show(ex.Message, "Page does not exist error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unexpected error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                StopProgressBar();
            }
        }

        private void specialFilter()
        {
            specialFilter SepcialFilter = new specialFilter(lbArticles);
            SepcialFilter.ShowDialog();
            UpdateNumberOfArticles();
        }

        private void RemoveSelectedArticle()
        {
            try
            {
                Saved = false;

                lbArticles.BeginUpdate();
                int i = lbArticles.SelectedIndex;

                if (lbArticles.SelectedItems.Count > 0)
                    txtNewArticle.Text = lbArticles.SelectedItem.ToString();

                while (lbArticles.SelectedItems.Count > 0)
                    lbArticles.Items.Remove(lbArticles.SelectedItem);

                if (lbArticles.Items.Count > i)
                    lbArticles.SelectedIndex = i;
                else
                    lbArticles.SelectedIndex = i - 1;

                lbArticles.EndUpdate();
            }
            catch
            { }

            UpdateNumberOfArticles();
        }

        /// <summary>
        /// Opens the dialog to filter out articles
        /// </summary>
        public void Filter()
        {
            specialFilter SepcialFilter = new specialFilter(lbArticles);
            SepcialFilter.ShowDialog();
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

                foreach (Article a in lbArticles)
                {
                    strList.AppendLine("# [[" + a.Name + "]]");
                }

                if (strListFile.Length > 0)
                    saveListDialog.FileName = strListFile;

                if (saveListDialog.ShowDialog() == DialogResult.OK)
                {
                    strListFile = saveListDialog.FileName;
                    StreamWriter sw = new StreamWriter(strListFile, false, Encoding.UTF8);
                    sw.Write(strList);
                    sw.Close();
                    Saved = true;
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message, "File error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Filters out articles that are not in the main namespace
        /// </summary>
        public void FilterNonMainArticles()
        {
            //filter out non-mainspace articles
            int i = 0;
            string s = "";

            while (i < lbArticles.Items.Count)
            {
                s = lbArticles.Items[i].ToString();

                if (!Tools.IsMainSpace(s))
                    lbArticles.Items.Remove(lbArticles.Items[i]);
                else //move on
                    i++;
            }
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

        private void UpdateNumberOfArticles()
        {
            lblNumberOfArticles.Text = lbArticles.Items.Count.ToString();
            if (NoOfArticlesChanged != null)
                this.NoOfArticlesChanged();
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

        private void convertToTalkPagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<Article> list = ArticleListFromListBox();
            list = GetLists.ConvertToTalk(list);
            lbArticles.Items.Clear();
            Add(list);
        }

        private void convertFromTalkPagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<Article> list = ArticleListFromListBox();
            list = GetLists.ConvertFromTalk(list);
            lbArticles.Items.Clear();
            Add(list);
        }

        private void fromCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] c = new string[lbArticles.SelectedItems.Count];

            int i = 0;
            foreach (object o in lbArticles.SelectedItems)
            {
                c[i] = o.ToString();
                i++;
            }

            MakeList(0, c);
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

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveSelectedArticle();
        }

        private void clearToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void openInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Variables.URL + "index.php?title=" + System.Web.HttpUtility.UrlEncode(lbArticles.SelectedItem.ToString()));
        }

        #endregion

    }
}
