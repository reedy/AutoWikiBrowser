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

namespace WikiFunctions
{
    public enum SourceType : byte { Category, WhatLinksHere, WhatTranscludesHere, LinksOnPage, TextFile, GoogleWikipedia, UserContribs, SpecialPage, ImageFileLinks, DatabaseDump, MyWatchlist }

    public delegate void BusyStateChangedDel();
    public delegate void NoOfArticlesChangedDel();

    public partial class ListMaker : UserControl, IEnumerable<Article>
    {
        public event BusyStateChangedDel BusyStateChanged;
        public event NoOfArticlesChangedDel NoOfArticlesChanged;
        
        public ListMaker()
        {
            InitializeComponent();
        }

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

        #region Events

        private void cmboSourceSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmboSourceSelect.SelectedIndex)
            {
                case 0:
                    lblSourceSelect.Text = Variables.Namespaces[14];
                    txtSelectSource.Enabled = true;
                    chkWLHRedirects.Visible = false;
                    return;
                case 1:
                    lblSourceSelect.Text = "What links to";
                    txtSelectSource.Enabled = true;
                    chkWLHRedirects.Visible = false;
                    return;
                case 2:
                    lblSourceSelect.Text = "What embeds";
                    txtSelectSource.Enabled = true;
                    chkWLHRedirects.Visible = false;
                    return;
                case 3:
                    lblSourceSelect.Text = "Links on";
                    txtSelectSource.Enabled = true;
                    chkWLHRedirects.Visible = false;
                    return;
                case 4:
                    lblSourceSelect.Text = "From file";
                    txtSelectSource.Enabled = false;
                    chkWLHRedirects.Visible = false;
                    return;
                case 5:
                    lblSourceSelect.Text = "Google search";
                    txtSelectSource.Enabled = true;
                    chkWLHRedirects.Visible = false;
                    return;
                case 6:
                    lblSourceSelect.Text = Variables.Namespaces[2];
                    txtSelectSource.Enabled = true;
                    chkWLHRedirects.Visible = false;
                    return;
                case 7:
                    lblSourceSelect.Text = Variables.Namespaces[-1];
                    txtSelectSource.Enabled = true;
                    chkWLHRedirects.Visible = false;
                    return;
                case 8:
                    lblSourceSelect.Text = Variables.Namespaces[6];
                    txtSelectSource.Enabled = true;
                    chkWLHRedirects.Visible = false;
                    return;
                default:
                    lblSourceSelect.Text = "";
                    txtSelectSource.Enabled = false;
                    chkWLHRedirects.Visible = false;
                    return;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtNewArticle.Text.Length == 0)
            {
                UpdateButtons();
                return;
            }

            Saved = false;
            AddToList(Tools.TurnFirstToUpper(txtNewArticle.Text));
            txtNewArticle.Text = "";
            UpdateButtons();
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
            txtSelectSource.Text = txtSelectSource.Text.Trim('[', ']');
            if (cmboSourceSelect.SelectedIndex == 0)
                txtSelectSource.Text = Regex.Replace(txtSelectSource.Text, "^" + Variables.Namespaces[14], "", RegexOptions.IgnoreCase);
            else if (cmboSourceSelect.SelectedIndex == 6)
                txtSelectSource.Text = Regex.Replace(txtSelectSource.Text, "^" + Variables.Namespaces[2], "", RegexOptions.IgnoreCase);
            else if (cmboSourceSelect.SelectedIndex == 7)
                txtSelectSource.Text = Regex.Replace(txtSelectSource.Text, "^" + Variables.Namespaces[-1], "", RegexOptions.IgnoreCase);
            else if (cmboSourceSelect.SelectedIndex == 8)
                txtSelectSource.Text = Regex.Replace(txtSelectSource.Text, "^" + Variables.Namespaces[6], "", RegexOptions.IgnoreCase);
            else if (cmboSourceSelect.SelectedIndex == 9)
            {
                launchDumpSearcher();
                return;
            }

            txtSelectSource.Text = Tools.TurnFirstToUpper(txtSelectSource.Text);
            txtSelectSource.AutoCompleteCustomSource.Add(txtSelectSource.Text);

            //make sure there is some text.
            if (txtSelectSource.Text.Length == 0 && txtSelectSource.Enabled)
            {
                MessageBox.Show("Please enter some text", "No text", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //if (!WikiStatus)
            //    return;

            if (cmboSourceSelect.SelectedIndex == 4)
            {
                try
                {
                    OpenFileDialog openListDialog = new OpenFileDialog();
                    openListDialog.Filter = "text files|*.txt|All files|*.*";
                    openListDialog.Multiselect = true;

                    this.Focus();
                    if (openListDialog.ShowDialog() == DialogResult.OK)
                    {
                        AddArticleListToList(GetLists.FromTextFile(openListDialog.FileNames));
                        ListFile = openListDialog.FileName;
                    }
                    UpdateButtons();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                return;
            }
            else if (cmboSourceSelect.SelectedIndex == 10)
            {
                AddArticleListToList(GetLists.FromWatchList());
                UpdateButtons();
                return;
            }

            string[] s = txtSelectSource.Text.Split('|');

            MakeList(cmboSourceSelect.SelectedIndex, s);
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

            if (lbArticles.SelectedItems.Count == 1)
            {
                if (lbArticles.SelectedItem.ToString().StartsWith(Variables.Namespaces[14]))
                    fromCategoryToolStripMenuItem.Enabled = true;
                else
                    fromCategoryToolStripMenuItem.Enabled = false;

                if (lbArticles.SelectedItem.ToString().StartsWith(Variables.Namespaces[6]))
                    fromImageLinksToolStripMenuItem.Enabled = true;
                else
                    fromImageLinksToolStripMenuItem.Enabled = false;
            }

            addSelectedToListToolStripMenuItem.Enabled = lbArticles.SelectedItems.Count > 0;

            removeToolStripMenuItem.Enabled = lbArticles.SelectedItem != null;
            clearToolStripMenuItem1.Enabled = boolEnabled;
            filterOutNonMainSpaceArticlesToolStripMenuItem.Enabled = boolEnabled;
            convertToTalkPagesToolStripMenuItem.Enabled = boolEnabled;
            convertFromTalkPagesToolStripMenuItem.Enabled = boolEnabled;
            sortAlphebeticallyMenuItem.Enabled = boolEnabled;
            saveListToTextFileToolStripMenuItem1.Enabled = boolEnabled;
            specialFilterToolStripMenuItem.Enabled = boolEnabled;
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

        public int SelectedSourceIndex
        {
            get { return cmboSourceSelect.SelectedIndex; }
            set { cmboSourceSelect.SelectedIndex = value; }
        }

        public string SourceText
        {
            get { return txtSelectSource.Text; }
            set { txtSelectSource.Text = value; }
        }

        public bool MakeListEnabled
        {
            set { btnMakeList.Enabled = value; }
        }

        public Thread ListThread
        {
            get { return ListerThread; }
            set { ListerThread = value; }
        }

        public int NumberOfArticles
        {
            get { return lbArticles.Items.Count; }
        }

        bool bSaved = true;
        public bool Saved
        {
            get { return bSaved; }
            private set { bSaved = value; }
        }

        bool bWikiStatus = true;
        public bool WikiStatus
        {
            get { return bWikiStatus; }
            set { bWikiStatus = value; }
        }

        string strStatus = "";
        public string Status
        {
            get { return strStatus; }
            set
            {
                strStatus = value;
            }
        }

        bool bBusyStatus = false;
        public bool BusyStatus
        {
            get { return bBusyStatus; }
            private set
            {
                bBusyStatus = value;
                this.BusyStateChanged();
            }
        }

        string strListFile = "";
        public string ListFile
        {
            get { return strListFile; }
            set { strListFile = value; }
        }

        public Article SelectedArticle()
        {
            if (lbArticles.SelectedItem == null)
                lbArticles.SelectedIndex = 0;

            return (Article)lbArticles.SelectedItem;
        }

        #endregion

        #region Methods

        public void RemoveEdittingArticle(Article EdittingArticle)
        {
            Saved = false;
            if (lbArticles.Items.Contains(EdittingArticle))
            {
                while (lbArticles.SelectedItems.Count > 0)
                    lbArticles.SetSelected(lbArticles.SelectedIndex, false);

                txtNewArticle.Text = EdittingArticle.Name;

                int intPosition = lbArticles.Items.IndexOf(EdittingArticle);

                lbArticles.Items.Remove(EdittingArticle);

                if (lbArticles.Items.Count == intPosition)
                    intPosition--;

                if (lbArticles.Items.Count > 0)
                    lbArticles.SelectedIndex = intPosition;
            }

            UpdateButtons();
        }        

        private void launchDumpSearcher()
        {
            WikiFunctions.DatabaseScanner.DatabaseScanner ds = new WikiFunctions.DatabaseScanner.DatabaseScanner(lbArticles);
            ds.Show();
            UpdateButtons();
        }

        private delegate void AddToListDel(string s);
        public void AddToList(string s)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new AddToListDel(AddToList), s);
                return;
            }

            Article a = new Article(s);
            lbArticles.Items.Add(a);
            UpdateButtons();
        }

        private delegate void AddArticleListDel(List<Article> l);
        private void AddArticleListToList(List<Article> l)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new AddArticleListDel(AddArticleListToList), l);
                return;
            }

            lbArticles.BeginUpdate();

            foreach (Article a in l)
            {
                if (!lbArticles.Items.Contains(a))
                    lbArticles.Items.Add(a);
            }

            lbArticles.EndUpdate();

            UpdateButtons();
        }

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
            UpdateButtons();
        }

        Thread ListerThread = null;
        private void MakeList(int Source, string[] SourceValues)
        {
            intSourceIndex = Source;
            strSouce = SourceValues;

            ThreadStart thr_Process = new ThreadStart(MakeList2);
            ListerThread = new Thread(thr_Process);
            ListerThread.IsBackground = true;
            ListerThread.Start();
        }

        int intSourceIndex = 0;
        string[] strSouce;
        private void MakeList2()
        {
            Saved = false;
            StartProgressBar();

            try
            {
                switch (intSourceIndex)
                {
                    case 0:
                        AddArticleListToList(GetLists.FromCategory(false, strSouce));
                        break;
                    case 1:
                        AddArticleListToList(GetLists.FromWhatLinksHere(false, strSouce));
                        break;
                    case 2:
                        AddArticleListToList(GetLists.FromWhatLinksHere(true, strSouce));
                        break;
                    case 3:
                        AddArticleListToList(GetLists.FromLinksOnPage(strSouce));
                        break;
                    //4 from text file
                    case 5:
                        AddArticleListToList(GetLists.FromGoogleSearch(strSouce));
                        break;
                    case 6:
                        AddArticleListToList(GetLists.FromUserContribs(strSouce));
                        break;
                    case 7:
                        AddArticleListToList(GetLists.FromSpecialPage(strSouce));
                        break;
                    case 8:
                        AddArticleListToList(GetLists.FromImageLinks(strSouce));
                        break;
                    //9 from datadump
                    //10 from watchlist
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
            UpdateButtons();
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

            UpdateButtons();
        }

        private void Clear()
        {
            Saved = false;
            lbArticles.Items.Clear();

            UpdateButtons();
        }

        public void Filter()
        {
            specialFilter SepcialFilter = new specialFilter(lbArticles);
            SepcialFilter.ShowDialog();
            UpdateButtons();
        }       

        public void SaveList()
        {//Save lbArticles list to text file.
            try
            {
                StringBuilder strList = new StringBuilder("");

                foreach (Article a in lbArticles)
                {
                    strList.Append("# [[" + a.Name + "]]\r\n");
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
            UpdateButtons();
        }

        public void AlphaSortList()
        {
            lbArticles.Sorted = true;
            lbArticles.Sorted = false;
        }

        public void ReplaceArticle(Article OldArticle, Article NewArticle)
        {
            lbArticles.ClearSelected();

            int intPos = 0;
            intPos = lbArticles.Items.IndexOf(OldArticle);

            RemoveEdittingArticle(OldArticle);
            lbArticles.Items.Insert(intPos, NewArticle);

            lbArticles.SelectedItem = NewArticle;
        }

        public void Stop()
        {
            if (ListerThread != null)
                ListerThread.Abort();

            StopProgressBar();
        }

        private void UpdateButtons()
        {
            lblNumberOfArticles.Text = lbArticles.Items.Count.ToString();
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
            AddArticleListToList(list);
        }

        private void convertFromTalkPagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<Article> list = ArticleListFromListBox();
            list = GetLists.ConvertFromTalk(list);
            lbArticles.Items.Clear();
            AddArticleListToList(list);
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

            MakeList(1, c);
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

            MakeList(3, c);
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

            MakeList(8, c);
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveSelectedArticle();
        }

        private void clearToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Clear();
        }
        #endregion
    }
}
