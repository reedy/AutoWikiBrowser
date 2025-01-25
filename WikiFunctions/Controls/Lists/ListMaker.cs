﻿/*
ListMaker
(C) Martin Richards
(C) Stephen Kennedy, Sam Reed 2009

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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using WikiFunctions.API;
using WikiFunctions.DBScanner;
using WikiFunctions.Lists;
using WikiFunctions.Lists.Providers;
using WikiFunctions.Plugin;

namespace WikiFunctions.Controls.Lists
{
    public delegate void ListMakerEventHandler(object sender, EventArgs e);

    public delegate void ListMakerProviderAdded(IListProvider provider);

    public partial class ListMaker : UserControl, IList<Article>
    {
        private static readonly BindingList<IListProvider> DefaultProviders = new BindingList<IListProvider>();
        private readonly ListFilterForm _specialFilter;

        private readonly BindingList<IListProvider> _listProviders;

        //used to keep easy track of providers for add/remove/(re)use in code
        #region ListProviders

        private static readonly IListProvider RedirectLProvider = new RedirectsListProvider(),
        WhatLinksHereLProvider = new WhatLinksHereListProvider(),
        WhatTranscludesLProvider = new WhatTranscludesPageListProvider(),
        CategoriesOnPageLProvider = new CategoriesOnPageListProvider(),
        NewPagesLProvider = new NewPagesListProvider(),
        RandomPagesLProvider = new RandomPagesSpecialPageProvider(),
        HtmlScraperLProvider = new HTMLPageScraperListProvider(),
        AdvHtmlScraperLProvider = new AdvancedRegexHtmlScraper(),
        CheckWikiLProvider = new CheckWikiListProvider(),
        CheckWikiWithNumberLProvider = new CheckWikiWithNumberListProvider(),
        UserContribLProvider = new UserContribsListProvider(),
        PagesWithProvider = new PagesWithPropListProvider(),
        WikiSearchProvider = new WikiSearchListProvider();
        #endregion

        public event ListMakerEventHandler StatusTextChanged,
        BusyStateChanged, NoOfArticlesChanged;

        public bool FilterNonMainAuto, AutoAlpha, FilterDuplicates;
        /// <summary>
        /// Occurs when a list has been created
        /// </summary>
        public event ListMakerEventHandler ListFinished;

        /// <summary>
        /// 
        /// </summary>
        public static ListMakerProviderAdded ListProviderAdded;

        static ListMaker()
        {
            if (!DefaultProviders.Any())
            {
#if DEBUG
                DefaultProviders.Add(new PagesWithPropJsonListProvider());
#endif
                DefaultProviders.Add(new CategoryListProvider());
                DefaultProviders.Add(new CategoryRecursiveListProvider());
                DefaultProviders.Add(new CategoryRecursiveOneLevelListProvider());
                DefaultProviders.Add(new CategoryRecursiveUserDefinedLevelListProvider());
                DefaultProviders.Add(CategoriesOnPageLProvider);
                DefaultProviders.Add(new CategoriesOnPageOnlyHiddenListProvider());
                DefaultProviders.Add(new CategoriesOnPageNoHiddenListProvider());
                DefaultProviders.Add(WhatLinksHereLProvider);
                DefaultProviders.Add(new WhatLinksHereAllNSListProvider());
                DefaultProviders.Add(new WhatLinksHereAndToRedirectsListProvider());
                DefaultProviders.Add(new WhatLinksHereAndToRedirectsAllNSListProvider());
                DefaultProviders.Add(new WhatLinksHereExcludingPageRedirectsListProvider());
                DefaultProviders.Add(new WhatLinksHereAndPageRedirectsExcludingTheRedirectsListProvider());
                DefaultProviders.Add(WhatTranscludesLProvider);
                DefaultProviders.Add(new WhatTranscludesPageAllNSListProvider());
                DefaultProviders.Add(new LinksOnPageListProvider());
                DefaultProviders.Add(new LinksOnPageOnlyBlueListProvider());
                DefaultProviders.Add(new LinksOnPageOnlyRedListProvider());
                DefaultProviders.Add(new FilesOnPageListProvider());
                DefaultProviders.Add(new TransclusionsOnPageListProvider());
                DefaultProviders.Add(new TextFileListProviderUFT8());
                DefaultProviders.Add(new TextFileListProviderWindows1252());
                DefaultProviders.Add(new GoogleSearchListProvider());
                DefaultProviders.Add(UserContribLProvider);
                DefaultProviders.Add(new UserContribUserDefinedNumberListProvider());
                DefaultProviders.Add(new SpecialPageListProvider(WhatLinksHereLProvider, NewPagesLProvider,
                                                                 CategoriesOnPageLProvider, RandomPagesLProvider,
                                                                 WhatTranscludesLProvider, RedirectLProvider,
                                                                 UserContribLProvider, PagesWithProvider,
                                                                 WikiSearchProvider));
                DefaultProviders.Add(new ImageFileLinksListProvider());
                DefaultProviders.Add(new MyWatchlistListProvider());
                DefaultProviders.Add(WikiSearchProvider);
                DefaultProviders.Add(new WikiSearchAllNSListProvider());
                DefaultProviders.Add(new WikiTitleSearchListProvider());
                DefaultProviders.Add(new WikiTitleSearchAllNSListProvider());
                DefaultProviders.Add(RandomPagesLProvider);
                DefaultProviders.Add(RedirectLProvider);
                DefaultProviders.Add(new RedirectsAllNSListProvider());
                DefaultProviders.Add(NewPagesLProvider);
                DefaultProviders.Add(PagesWithProvider);
            }
        }

        public ListMaker()
        {
            InitializeComponent();

            ListProviderAdded += ProviderAdded;

            _specialFilter = new ListFilterForm(lbArticles);

            foreach (IListProvider prov in DefaultProviders)
            {
                if (!prov.UserInputTextBoxEnabled) continue;

                ToolStripMenuItem addToFromSelectedListFrom = new ToolStripMenuItem(prov.DisplayText) { Tag = prov };

                addToFromSelectedListFrom.Click += AddToFromSelectedListFrom;

                addSelectedToListToolStripMenuItem.DropDownItems.Add(addToFromSelectedListFrom);
            }

            _listProviders = new BindingList<IListProvider>
            {
                new DatabaseScannerListProvider(this),

                //Add these list providers later, we dont really need/want them on the Right click "Add to list from.." menu
                HtmlScraperLProvider,
                CheckWikiLProvider,
                CheckWikiWithNumberLProvider,
                AdvHtmlScraperLProvider
            };

            foreach (IListProvider lvi in DefaultProviders)
            {
                _listProviders.Add(lvi);
            }

            // We'll manage our own collection of list items:
            cmboSourceSelect.DataSource = _listProviders;

            // Mono throws exception in relation to DisplayMember
            if(!Globals.UsingMono)
            {
                // Bind IListProvider.DisplayText to be the displayed text:
                cmboSourceSelect.DisplayMember = "DisplayText";
                cmboSourceSelect.ValueMember = "DisplayText";
            }

            // Use the long-time default, also being quite basic, instead of relying on alphasort
            SelectedProvider = "CategoryListProvider";

            //Dictionary to ComboBox (Maybe change at later date?)
            //http://steve-fair-dev.blogspot.com/2008/04/bind-dictionary-to-winform-combobox.html
        }

        private void AddToFromSelectedListFrom(object sender, EventArgs e)
        {
            AddFromSelectedList((IListProvider)((ToolStripMenuItem)sender).Tag);
        }

        private void ProviderAdded(IListProvider provider)
        {
            if (!_listProviders.Contains(provider))
                _listProviders.Add(provider);
        }

        new public static void Refresh() { }

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
                yield return lbArticles.Items[i];
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
            UpdateNumberOfArticles(false);
        }

        /// <summary>
        /// Returns a value indicating whether the given article is in the list
        /// </summary>
        public bool Contains(Article item)
        {
            return lbArticles.Items.Contains(item);
        }

        /// <summary>
        /// Returns a value indicating whether the given article is in the list
        /// </summary>
        public bool Contains(string title)
        {
            return Contains(new Article(title));
        }

        public void CopyTo(Article[] array, int arrayIndex)
        {
            lbArticles.Items.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns the length of the list
        /// </summary>
        public int Count
        { get { return lbArticles.Items.Count; } }

        public bool IsReadOnly
        { get { return false; } }

        /// <summary>
        /// Removes the given article, by title, from the list
        /// </summary>
        public bool Remove(string title)
        {
            return Remove(new Article(title));
        }

        /// <summary>
        /// Removes the given article from the list
        /// </summary>
        public bool Remove(Article item)
        {
            if (!lbArticles.Items.Contains(item))
                return false;

            // set last used article
            txtPage.Text = item.Name;

            // if one article selected and it's the one to be removed, just RemoveSelected
            if (lbArticles.SelectedItems.Count == 1 && lbArticles.SelectedItem.ToString() == item.Name)
            {
                int currentIndex = lbArticles.SelectedIndex;
                lbArticles.RemoveSelected(FilterDuplicates);
                // Wine fix: does not scroll listbox if selected article moves out of view
                lbArticles.TopIndex = currentIndex;
            }
            else
            {
                // otherwise if article to be removed isn't the single selected one, there may be duplicates of the article
                // so remove first instance and avoid scrolling
                // if replacing the second instance of the article in the list maker avoid jumping selected article to the first
                int intPosition = lbArticles.Items.IndexOf(item);

                while (lbArticles.SelectedItems.Count > 0)
                    lbArticles.SetSelected(lbArticles.SelectedIndex, false);

                lbArticles.Items.Remove(item);

                if (lbArticles.Items.Count == intPosition)
                    intPosition--;

                if (lbArticles.Items.Count > 0)
                {
                    lbArticles.SelectedIndex = intPosition;

                    // Wine fix: does not scroll listbox if selected article moves out of view
                    if (lbArticles.TopIndex > lbArticles.Items.Count-1)
                        lbArticles.TopIndex = intPosition;
                }
            }

            UpdateNumberOfArticles(false);
            return true;
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
            lbArticles.Items.Insert(index, new Article(item));
            UpdateNumberOfArticles();
        }

        /// <summary>
        /// Removes article at the given index
        /// </summary>
        public void RemoveAt(int index)
        {
            lbArticles.Items.RemoveAt(index);
            UpdateNumberOfArticles(false);
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

            if (!UserInputTextBox.AutoCompleteCustomSource.Contains(UserInputTextBox.Text))
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

            if (strTip != tooltip.GetToolTip(lbArticles))
                tooltip.SetToolTip(lbArticles, strTip);
        }

        private void txtNewArticle_MouseMove(object sender, MouseEventArgs e)
        {
            if (txtPage.Text != tooltip.GetToolTip(txtPage))
                tooltip.SetToolTip(txtPage, txtPage.Text);
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
            
            if (e.Modifiers == Keys.Control)
            {
                if (e.KeyCode == Keys.A)
                {
                    UserInputTextBox.SelectAll();
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                }
            }
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
                sortAlphaMenuItem.Enabled = sortReverseAlphaMenuItem.Enabled =
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

        private void txtNewArticle_TextChanged(object sender, EventArgs e)
        {
            btnAdd.Enabled = txtPage.Text.Trim().Length > 0;

            // under Mono assigning txtPage.Text fires another _TextChanged event so we get an infinite loop; so remove and reassign event as workaround 
            if (Globals.UsingMono)
                txtPage.TextChanged -= txtNewArticle_TextChanged;

            // reset any custom formatting of text (if copied from syntax highlighted text in edit box etc.), restoring cursor position
            string a = txtPage.Text; 
            int i = txtPage.SelectionStart;
            txtPage.ResetText();
            txtPage.Text = a;
            txtPage.Select(i,0);

            if (Globals.UsingMono)
                txtPage.TextChanged += txtNewArticle_TextChanged;
        }

        private void lbArticles_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnRemove.Enabled = lbArticles.SelectedItems.Count > 0;
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets the ListBox that holds the list of articles
        /// </summary>
        public ListBox Items
        { get { return lbArticles; } }

        /// <summary>
        /// Gets or sets the selected provider type
        /// </summary>
        public string SelectedProvider
        {
            get { return cmboSourceSelect.SelectedItem.GetType().Name; }
            set
            {
                int index;
                if (int.TryParse(value, out index))
                {
                    // We're opening an old-style config where provider was set as an index
                    if (index < (cmboSourceSelect.Items.Count - 1))
                    {
                        cmboSourceSelect.SelectedIndex = index;
                    }
                }
                else
                {
                    // New settings format that specifies provider type name
                    foreach (var provider in _listProviders)
                    {
                        if (provider.GetType().Name == value)
                        {
                            cmboSourceSelect.SelectedItem = provider;
                            break;
                        }
                    }
                }

                cmboSourceSelect_SelectedIndexChanged(null, null);
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

        string _status = "";
        /// <summary>
        /// The status of the process
        /// </summary>
        public string Status
        {
            get { return _status; }
            private set
            {
                _status = value;
                if (StatusTextChanged != null)
                    StatusTextChanged(null, null);
            }
        }

        bool _busyStatus;
        /// <summary>
        /// Gets a value indicating whether the process is busy
        /// </summary>
        public bool BusyStatus
        {
            get { return _busyStatus; }
            private set
            {
                _busyStatus = value;
                if (BusyStateChanged != null)
                    BusyStateChanged(null, null);
            }
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
        /// <summary>
        /// When using pre-parse mode, selects next article in list, if there is one
        /// </summary>
        public bool NextArticle()
        {
            ((Article)lbArticles.SelectedItem).PreProcessed = true;

            if (lbArticles.Items.Count == lbArticles.SelectedIndex + 1 ||
                (lbArticles.Items.Count == 1 && lbArticles.SelectedIndex == 0))
                return false;

            lbArticles.SelectedIndex++;
            lbArticles.SetSelected(lbArticles.SelectedIndex, false);
 
            // Wine fix: does not scroll listbox if selected article moves out of view
            if ((lbArticles.TopIndex + 15) < lbArticles.SelectedIndex)
                lbArticles.TopIndex = lbArticles.SelectedIndex;

            return true;
        }

        private const string DiffEditURL = @"/w(?:(?:iki)?/index\.php5?\?|/\?)title=(.*?)(?:&(?:action|diff|oldid|pe|offset|curid|redirect|type)=.*|$)";
        /// <summary>
        /// Extracts wiki page title from wiki page URL, including diff and revision history URLs
        /// </summary>
        public string NormalizeTitle(string s)
        {
            // https://en.wikipedia.org/w/index.php?title=...&action=history
            // https://en.wikipedia.org/w/index.php?title=...&diff=
            string originals = s;
            string escaped = Regex.Escape(Variables.URL);

            Regex HistoryDiff = new Regex(Regex.Replace(escaped, @"https?://", @"(?:https?://|//)?") + DiffEditURL);
            s = HistoryDiff.Replace(s, "$1");

            // Assumsuption flaw: that all wikis use /wiki/ as the default path
            string url = Variables.URL + "/wiki/";
            if (Variables.URL.Contains("https:"))
                s = s.Replace("http://", "https://"); // support HTTP and HTTPS links

            s = s.Replace(url, "").Trim();

            // clean section links
            if (s.IndexOf("#", StringComparison.Ordinal) > 0)
                s = s.Substring(0, s.IndexOf("#", StringComparison.Ordinal));

            if (!originals.Equals(s))
                s = Tools.WikiDecode(s);

            //Remove Left-to-right marks from title
            //https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_21#Doesn.27t_skip_pages_with_left-to-right_marks
            s = s.Replace("‎", "").Trim();

            return s;
        }

        private delegate void AddToListDel(string s);
        /// <summary>
        /// Adds the given string to the list, first turning it into an Article
        /// </summary>
        /// <remarks>
        /// Don't use me in a loop. Make a list and pass to the Add(List&lt;<see cref="Article"/>>) overload
        /// </remarks>
        public void Add(string s)
        {
            if (InvokeRequired)
            {
                Invoke(new AddToListDel(Add), s);
                return;
            }

            s = Tools.RemoveSyntax(s);

            if (Variables.CapitalizeFirstLetter)
                s = Tools.TurnFirstToUpper(s);

            List<Article> l = new List<Article> {new Article(s)};

            Add(l);
        }

        private delegate void AddDel(List<Article> l);
        /// <summary>
        /// Adds the article list to the list
        /// </summary>
        public void Add(List<Article> l)
        {
            if (l == null || !l.Any())
                return;

            if (InvokeRequired)
            {
                Invoke(new AddDel(Add), l);
                return;
            }

            if (FilterNonMainAuto)
                l = l.FindAll(a => a.NameSpaceKey == Namespace.Article);

            // if deduplicating, only add items not already in the list, rather than adding all and deduplicating entire list again
            if (FilterDuplicates)
                l = DeDuplicate(l);

            if (l.Any())
            {            
                lbArticles.BeginUpdate();
                lbArticles.Items.AddRange(l.ToArray());
                lbArticles.EndUpdate();

                UpdateNumberOfArticles();
            }
        }

        /// <summary>
        /// Returns only those distinct articles in the input list that do not already exist in the list box
        /// </summary>
        /// <returns>The duplicate.</returns>
        /// <param name="l">L.</param>
        private List<Article> DeDuplicate(List<Article> l)
        {
            List<Article> articles = lbArticles.Items.Cast<Article>().ToList();

            return l.Except(articles).ToList();
        }

        /// <summary>
        /// Returns the list of articles
        /// </summary>
        public List<Article> GetArticleList()
        {
            return lbArticles.Items.Cast<Article>().ToList();
        }

        /// <summary>
        /// Returns the list of selected articles
        /// </summary>
        public List<Article> GetSelectedArticleList()
        {
            return lbArticles.SelectedItems.Cast<Article>().ToList();
        }

        private delegate void StartProgBarDelegate();
        private void StartProgressBar()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new StartProgBarDelegate(StartProgressBar));
                return;
            }

            BusyStatus = true;

            Cursor = Cursors.WaitCursor;
            Status = "Getting list";
            btnGenerate.Enabled = false;
        }

        private delegate void StopProgBarDelegate(int count);
        private void StopProgressBar(int newArticles)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new StopProgBarDelegate(StopProgressBar), newArticles);
                return;
            }

            BusyStatus = false;

            Cursor = Cursors.Default;

            if (newArticles == -1)
                Status = "List creation aborted.";
            else if (newArticles > 0)
                Status = "List complete!";
            else
                Status = "No results.";

            btnGenerate.Enabled = true;

            btnStop.Visible = false;

            if (newArticles > 0)
                UpdateNumberOfArticles();

            if (ListFinished != null)
                ListFinished(null, null);
        }

        private Thread _listerThread;

        /// <summary>
        /// Makes a list of pages from the currently selected item
        /// </summary>
        public void MakeList()
        {
            // Pipe character is a separator for standard searches e.g. foo|bar to search for foo or bar by doing two searches
            // However in a source search like insource:/\|last=Smitherson/ the pipe isn't a search separator, so don't split on | for insource
            if (UserInputTextBox.Text.Contains ("|") && !(UserInputTextBox.Text.StartsWith("insource:", StringComparison.OrdinalIgnoreCase)))
                MakeList((IListProvider)cmboSourceSelect.SelectedItem, UserInputTextBox.Text.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
            else
                MakeList((IListProvider)cmboSourceSelect.SelectedItem, new[] { UserInputTextBox.Text });
        }

        /// <summary>
        /// Makes a list of pages
        /// </summary>
        /// <param name="provider">The <see cref="IListProvider"/> to make the list</param>
        /// <param name="sourceValues">An array of string values to create the list with, e.g. an array of categories. Use null if not appropriate</param>
        public void MakeList(IListProvider provider, string[] sourceValues)
        {
            btnStop.Visible = true;

            _providerToRun = provider;

            if (_providerToRun.StripUrl)
            {
                for (int i = 0; i < sourceValues.Length; i++)
                {
                    sourceValues[i] = NormalizeTitle(sourceValues[i]);
                }
            }

            _source = sourceValues;

            if (_providerToRun.RunOnSeparateThread)
            {
                _listerThread = new Thread(MakeTheListThreaded)
                {
                    IsBackground = true
                };
                _listerThread.SetApartmentState(ApartmentState.STA);
                _listerThread.Start();
            }
            else
                MakeTheList();
        }

        private string[] _source;
        private IListProvider _providerToRun;

        private void MakeTheListThreaded()
        {
            Thread.CurrentThread.Name = "ListMaker (" + _providerToRun.GetType().Name + ": "
                + UserInputTextBox.Text + ")";
            MakeTheList();
        }

        private void MakeTheList()
        {
            StartProgressBar();

            List<Article> articles = null;

            try
            {
                articles = _providerToRun.MakeList(_providerToRun.UserInputTextBoxEnabled ? _source : new string[0]);
                Add(articles);
            }
            catch (ThreadAbortException)
            {
            }
            catch (FeatureDisabledException fde)
            {
                DisabledListProvider(fde);
            }
            catch (LoggedOffException)
            {
                UserLoggedOff();
            }
            catch (ApiErrorException aee)
            {
                if (aee.ErrorCode == "eiinvalidtitle")
                {
                    MessageBox.Show("An invalid title of \"" + aee.GetErrorVariable() + "\" was passed to the API.",
                                    "Invalid Title");
                }
            }
            catch (ArgumentException ae)
            {
                MessageBox.Show(ae.Message, "Invalid Parameter passed to List Maker");
            }
            catch (InterwikiException iwe)
            {
                MessageBox.Show(iwe.Message, "Interwiki title passed to List Maker");
            }
            catch (InvalidTitleException ite)
            {
                MessageBox.Show(ite.Message, "Invalid title passed to List Maker");
            }
            catch (Exception ex)
            {
                ErrorHandler.ListMakerText = UserInputTextBox.Text;
                ErrorHandler.HandleException(ex);
                ErrorHandler.ListMakerText = "";
            }
            finally
            {
                if (FilterNonMainAuto)
                    FilterNonMainArticles();
                if (FilterDuplicates)
                    RemoveListDuplicates();
                StopProgressBar((articles != null) ? articles.Count : 0);
            }
        }

        private void DisabledListProvider(FeatureDisabledException fde)
        {
            MessageBox.Show(
                "Unable to generate lists using " + _providerToRun.DisplayText, fde.ApiErrorMessage);
        }

        private void UserLoggedOff()
        {
            MessageBox.Show(
                "User must be logged in to use \"" + _providerToRun.DisplayText + "\". Please login and try again.",
                "User logged out");
        }

        /// <summary>
        /// 
        /// </summary>
        private void RemoveSelectedArticle()
        {
            lbArticles.RemoveSelected(FilterDuplicates);
            UpdateNumberOfArticles(false);
        }

        /// <summary>
        /// Opens the dialog to filter out articles
        /// </summary>
        public void Filter()
        {
            _specialFilter.ShowDialog(this);
        }

        /// <summary>
        /// Removes all duplicates from the list
        /// </summary>
        public void RemoveListDuplicates()
        {
            if (InvokeRequired)
            {
                Invoke(new GenericDelegate(RemoveListDuplicates));
                return;
            }

            _specialFilter.Clear();
            _specialFilter.RemoveDuplicates();

            UpdateNumberOfArticles(false);
        }

        /// <summary>
        /// Saves the list box of the current <see cref="ListMaker"/> to the specified text file.
        /// </summary>
        public void SaveList()
        {
            lbArticles.SaveList();
        }

        private delegate void GenericDelegate();

        /// <summary>
        /// Filters out articles that are not in the main namespace
        /// </summary>
        public void FilterNonMainArticles()
        {
            if (InvokeRequired)
            {
                Invoke(new GenericDelegate(FilterNonMainArticles));
                return;
            }

            List<Article> articles = new List<Article>(lbArticles);
            List<Article> toberemoved = articles.FindAll(a => a.NameSpaceKey != Namespace.Article);

            if (toberemoved.Any())
            {
                // performance: AddRange performs at about 100 articles per millisecond, Remove takes about 1 millisecond per article
                // so if removing < 1% of articles it's faster to Remove each one, otherwise faster to clear and AddRange the remainder back
                lbArticles.BeginUpdate();
                if (toberemoved.Count < (int)articles.Count/100)
                {
                    foreach(Article a in toberemoved)
                        lbArticles.Items.Remove(a);
                }
                else
                {
                    articles.RemoveAll(a => a.NameSpaceKey != Namespace.Article);
                    lbArticles.Items.Clear();
                    lbArticles.Items.AddRange(articles.ToArray());
                }
                lbArticles.EndUpdate();
                UpdateNumberOfArticles(false);
            }
        }

        /// <summary>
        /// Alphabetically sorts the list
        /// </summary>
        public void AlphaSortList()
        {
            lbArticles.Sort();
        }

        /// <summary>
        /// Reverse Alphabetically sorts the list
        /// </summary>
        public void ReverseAlphaSortList()
        {
            lbArticles.ReverseSort();
        }

        /// <summary>
        /// Replaces one article in the list with another, in the same place
        /// </summary>
        public void ReplaceArticle(Article oldArticle, Article newArticle)
        {
            int intPos;

            // if replacing the second instance of the article in the list maker avoid jumping selected article to the first
            // if the selected article is the oldArticle
            if (lbArticles.SelectedItems.Count == 1 && lbArticles.SelectedItems.Contains(oldArticle))
                intPos = lbArticles.SelectedIndex;
            else
                intPos = lbArticles.Items.IndexOf(oldArticle);

            lbArticles.Items.Remove(oldArticle);
            lbArticles.ClearSelected();
            lbArticles.Items.Insert(intPos, newArticle);

            // set current position by index of new article rather than name in case new entry already exists earlier in list
            lbArticles.SetSelected(intPos, true);
        }

        /// <summary>
        /// Stops the processes
        /// </summary>
        public void Stop()
        {
            if (_listerThread != null)
                _listerThread.Abort();

            StopProgressBar(-1);
        }

        /// <summary>
        /// Updates the Number of Articles, enablement of Remove, Filter buttons. Sorts list if sorting is turned on
        /// </summary>
        public void UpdateNumberOfArticles()
        {
            UpdateNumberOfArticles(true);
        }

        /// <summary>
        /// Updates the Number of Articles, enablement of Remove, Filter buttons. Sorts list if input set
        /// </summary>
        public void UpdateNumberOfArticles(bool sortneeded)
        {
            lblNumOfPages.Text = lbArticles.Items.Count + " page";
            if (lbArticles.Items.Count != 1)
                lblNumOfPages.Text += "s";
            if (NoOfArticlesChanged != null)
                NoOfArticlesChanged(null, null);

            if (sortneeded && AutoAlpha)
                AlphaSortList();

            btnFilter.Enabled = lbArticles.Items.Count > 0;
            btnRemove.Enabled = lbArticles.SelectedItems.Count > 0;
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

        private void sortReverseAlphebeticallyMenuItem_Click(object sender, EventArgs e)
        {
            ReverseAlphaSortList();
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
            RemoveListDuplicates();
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
                object obj = Clipboard.GetDataObject();
                if (obj == null)
                    return;

                string textTba = ((IDataObject)obj).GetData(DataFormats.UnicodeText).ToString();

                List<Article> NewArticles = new List<Article>();

                foreach (string entry in textTba.Split(new[] { "\r\n", "\n", "|" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (!string.IsNullOrEmpty(entry.Trim()))
                    {
                        string s = NormalizeTitle(entry);
                        s = Tools.RemoveSyntax(s);

                        if (Variables.CapitalizeFirstLetter)
                            s = Tools.TurnFirstToUpper(s);
                        NewArticles.Add(new Article(s));
                    }
                }
                lbArticles.BeginUpdate();
                Add(NewArticles);

            }
            catch
            { }
            lbArticles.EndUpdate();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectAll();
        }

        private void invertSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lbArticles.BeginUpdate();

            for (int i = 0; i < lbArticles.Items.Count; i++)
                lbArticles.SetSelected(i, !lbArticles.GetSelected(i));

            lbArticles.EndUpdate();
        }

        private void selectNoneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectNone();
        }
        
        /// <summary>
        /// Desselects any selected items in the list box.
        /// </summary>
        private void SelectNone()
        {
            lbArticles.BeginUpdate();

            lbArticles.SelectedIndex = -1;

            lbArticles.EndUpdate();
        }
        
        /// <summary>
        /// Selects all items in the list box. Uses send of keyboard shortcuts for performance
        /// </summary>
        private void SelectAll()
        {
            lbArticles.BeginUpdate();

            // workaround for Wine issue: use of {HOME} then +{END} leads to 100% CPU and locked application
            // so use slower SetSelected if on Linux
            if (Globals.UsingLinux)
            {
                for(int i = 0; i < lbArticles.Items.Count; i++)
                    lbArticles.SetSelected(i, true);
            }
            else
            {
                SendKeys.SendWait("{HOME}");
                SendKeys.SendWait("+{END}");
            }

            lbArticles.EndUpdate();
        }

        private void AddFromSelectedList(IListProvider provider)
        {
            if (lbArticles.SelectedItems.Count == 0)
                return;

            MakeList(provider, (from Article a in lbArticles.SelectedItems select a.Name).ToArray());
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
                LoadArticlesInBrowser();
        }

        //  Get selected list first, then process. Otherwise looping through listmaker and processing,
        // may take seconds to open multiple browser tabs, could lead to exception if listmaker list changes
        // in the meantime
        private void LoadArticlesInBrowser()
        {
            if (Variables.MainForm.TheSession.Site != null) // TheSession can be null if AWB encounters network problems on startup
            {
                List<Article> articles = GetSelectedArticleList();

                foreach (Article item in articles)
                {
                    Variables.MainForm.TheSession.Site.OpenPageInBrowser(item.Name);
                }
            }
        }

        #endregion

        private void btnStop_Click(object sender, EventArgs e)
        {
            btnStop.Visible = false;
            Stop();
        }

        private void openHistoryInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((lbArticles.SelectedItems.Count < 10) || (MessageBox.Show("Opening " + lbArticles.SelectedItems.Count + " articles in your browser at once could cause your system to run slowly, and even stop responding.\r\nAre you sure you want to continue?", "Continue?", MessageBoxButtons.YesNo) == DialogResult.Yes))
                LoadArticleHistoryInBrowser();
        }

        private void LoadArticleHistoryInBrowser()
        {
            List<Article> sel = GetSelectedArticleList();
            foreach (Article item in sel)
                Tools.OpenArticleHistoryInBrowser(item.Name);
        }

        private void lbArticles_DoubleClick(object sender, EventArgs e)
        {
            if ((lbArticles.SelectedItems.Count < 10) || (MessageBox.Show("Opening " + lbArticles.SelectedItems.Count + " articles in your browser at once could cause your system to run slowly, and even stop responding.\r\nAre you sure you want to continue?", "Continue?", MessageBoxButtons.YesNo) == DialogResult.Yes))
                LoadArticlesInBrowser();
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
        /// Moves the currently selected page(s) in the listbox to the position selected
        /// </summary>
        /// <param name="toIndex">Index to move items to</param>
        private void MoveSelectedItems(int toIndex)
        {
            bool toTop = (toIndex == 0);
            lbArticles.BeginUpdate();

            /* Get the selected articles, reverse order so when re-inserted the original order is maintained and
             * remove the articles by index to ensure the selected pages are removed, rather than any earlier instances
             * of the same page in the list */
            List<Article> articlesToMove = GetSelectedArticleList();
            articlesToMove.Reverse();
            lbArticles.RemoveSelected(FilterDuplicates);

            if (toIndex > lbArticles.Items.Count)
                toIndex = lbArticles.Items.Count;

            foreach (Article a in articlesToMove)
            {
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
            get { return _specialFilter.Settings; }
            set
            {
                if (DesignMode)
                    return;
                _specialFilter.Settings = value;
            }
        }

        /// <summary>
        /// Add a <see cref="IListProvider"/> or a <see cref="IListMakerPlugin"/> to all ListMakers
        /// </summary>
        /// <param name="provider"><see cref="IListProvider"/>/<see cref="IListMakerPlugin"/> to add</param>
        public static void AddProvider(IListProvider provider)
        {
            DefaultProviders.Add(provider);

            if (ListProviderAdded != null)
                ListProviderAdded(provider);
        }

        /// <summary>
        /// Returns a new <see cref="DatabaseScanner"/> tied to an instance of the current Articles List Box
        /// </summary>
        /// <returns></returns>
        public DatabaseScanner DBScanner()
        {
            return new DatabaseScanner(this);
        }

        private static readonly Regex HTMLItalics = new Regex(@"(.*?)<i>(.*?)</i>(.*?(?=<i>|$))");
        private static readonly Regex SpanHide = new Regex(@"< *span +style *= *"" *position *: *absolute *; *top *: *-9999px;? *"" *>.*?< */ *span *>");

        /// <summary>
        /// Overrides default Item Drawing to enable different colour if the article has been pre-processed
        /// Formats text per displaytitle (italics etc.) if option enabled
        /// </summary>
        private void lbArticles_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;

            Article a = (Article)lbArticles.Items[e.Index];

            bool selected = ((e.State & DrawItemState.Selected) == DrawItemState.Selected);

            // bright green background if article pre-processed
            if (!selected)
                e = new DrawItemEventArgs(e.Graphics, e.Font, e.Bounds, e.Index,
                                          e.State,
                                          e.ForeColor, (a.PreProcessed) ? Color.GreenYellow : e.BackColor);

            e.DrawBackground();

            // format display title of article if option enabled, custom text format (not colour)
            if (!formatDisplayTitleToolStripMenuItem.Checked || string.IsNullOrEmpty(a.DisplayTitle))
            {
                e.Graphics.DrawString(a.Name, e.Font, (selected) ? Brushes.White : Brushes.Black, e.Bounds,
                    StringFormat.GenericDefault);
            }
            else
            {
                Font regular = e.Font;
                Font italic = new Font(e.Font, FontStyle.Italic);
                Rectangle r = e.Bounds;

                string displayTitle = a.DisplayTitle;

                // sup and sub not supported by FontStyle so remove
                displayTitle = WikiRegexes.SupSub.Replace(displayTitle, "$1");

                // span of type "position:absolute; top: -9999px" is used to hide text, so do that
                displayTitle = SpanHide.Replace(displayTitle, "");

                // if no display title then standard article, standard format
                if (string.IsNullOrEmpty(displayTitle))
                {
                    e.Graphics.DrawString(a.Name, regular, (selected) ? Brushes.White : Brushes.Black, r,
                        StringFormat.GenericDefault);
                } 
                else
                {
                    displayTitle = displayTitle.Replace("&amp;", "&");

                    // if no HTML, could be first letter lower or underscores in title, format using display title in standard font 
                    if (displayTitle.Replace("_", " ").TrimStart(" _".ToCharArray()).Equals(a.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        e.Graphics.DrawString(displayTitle, regular, (selected) ? Brushes.White : Brushes.Black, r,
                            StringFormat.GenericDefault);
                    }
                    // italics support
                    else if (HTMLItalics.IsMatch(displayTitle))
                    {
                        foreach(Match m in HTMLItalics.Matches(displayTitle))
                        {
                            for (int i = 1; i < 4; i++)
                            {
                                if (m.Groups[i].Value.Length > 0)
                                {
                                    Font f2 = (i == 2 ? italic : regular);
                                    e.Graphics.DrawString(m.Groups[i].Value, f2, (selected) ? Brushes.White : Brushes.Black, r,
                                        StringFormat.GenericDefault);
                                    int w = (int)e.Graphics.MeasureString(m.Groups[i].Value, f2).Width;
                                    r.X += w;
                                    r.Width -= w;
                                }
                            }
                        }
                    }

                    // TODO bold support, sub/sup support, mulitple italics support, span to hide support

                    else // unsupported other formatting in displaytitle, draw as default
                    e.Graphics.DrawString(a.Name, regular, (selected) ? Brushes.White : Brushes.Black, r,
                            StringFormat.GenericDefault);
                }
            }

            e.DrawFocusRectangle();
        }

        /// <summary>
        /// 
        /// </summary>
        public void BeginUpdate()
        {
            lbArticles.BeginUpdate();
        }

        /// <summary>
        /// 
        /// </summary>
        public void EndUpdate()
        {
            lbArticles.EndUpdate();
        }
    }
}
