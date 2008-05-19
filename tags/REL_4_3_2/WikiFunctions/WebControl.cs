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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Web;

namespace WikiFunctions.Browser
{
    public delegate void WebControlDel(object sender, EventArgs e);

    public enum enumProcessStage : byte { load, diff, save, delete, protect, none }

    /// <summary>
    /// Provides a WebBrowser component adapted and extended for use with Wikis.
    /// </summary>
    public partial class WebControl : WebBrowser
    {
        #region Constructor etc.

        public WebControl()
        {
            InitializeComponent();
            timer1.Interval = 1000;
            timer1.Enabled = true;

            this.ScriptErrorsSuppressed = true;
            ProcessStage = enumProcessStage.none;
        }

        Timer timer1 = new Timer();

        public static bool Shutdown;

        /// <summary>
        /// Occurs when the edit page has finished loading
        /// </summary>
        public event WebControlDel Loaded;
        /// <summary>
        /// Occurs when the page has finished saving
        /// </summary>
        public event WebControlDel Saved;
        /// <summary>
        /// Occurs when the diff or preview page has finished loading
        /// </summary>
        //public event WebControlDel Diffed;
        /// <summary>
        /// Occurs when the page has finished deleting
        /// </summary>
        public event WebControlDel Deleted;
        /// <summary>
        /// Occurs when the page has finished loading, and it was not a save/load/diff
        /// </summary>
        public event WebControlDel None;
        /// <summary>
        /// Occurs when the page failed to load properly
        /// </summary>
        public event WebControlDel Fault;
        /// <summary>
        /// Occurs when the status changes
        /// </summary>
        public event WebControlDel StatusChanged;
        /// <summary>
        /// Occurs when the Busy state changes
        /// </summary>
        public event WebControlDel BusyChanged;
        /// <summary>
        /// Controls watchlisting, to allow AWB setting to over-ride MW user settings
        /// </summary>
        private bool Watch;

        //private bool CanOverride;

        #endregion

        #region Properties

        enumProcessStage pStage = enumProcessStage.none;
        [Browsable(false)]
        public enumProcessStage ProcessStage
        {
            get { return pStage; }
            set { pStage = value; }
        }

        /// <summary>
        /// Returns title of currently loaded page
        /// </summary>
        [Browsable(false)]
        public string ArticleTitle
        {
            get
            {
                string s = DocumentText;
                s = s.Remove(0, s.IndexOf("var wgPageName = \"") + "var wgPageName = \"".Length);
                return HttpUtility.HtmlDecode(s.Substring(0, s.IndexOf("\""))).Replace("_", " ");
            }
        }

        /// <summary>
        /// Gets contents of currently loaded page
        /// </summary>
        /// <returns>HTML text</returns>
        public override string ToString()
        {
            try
            {
                return DocumentText;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Returns revision ID of currently loaded page
        /// </summary>
        [Browsable(false)]
        public int Revid
        {
            get
            {
                int rev;
                Regex r;
                try
                {
                    r = new Regex("&diff=\\d+");
                    Match m = r.Match(Url.ToString());
                    if (m.Success && int.TryParse(m.Value.Remove(0, 6), out rev)) return rev;
                }
                finally
                {
                    r = new Regex("&oldid=\\d+");
                    rev = int.Parse(r.Match(Url.ToString()).Value.Remove(0, 7));
                }
                return rev;
            }
        }
        /// <summary>
        /// Gets a value indicating whether the page can be saved
        /// </summary>
        [Browsable(false)]
        public bool CanSave
        {
            get { return (this.Document != null && this.Document.GetElementById("wpSave") != null); }
        }

        /// <summary>
        /// Gets a value indicating whether the page can be previewed
        /// </summary>
        [Browsable(false)]
        public bool CanPreview
        {
            get { return (this.Document != null && this.Document.GetElementById("wpPreview") != null); }
        }

        /// <summary>
        /// Gets a value indicating whether the page can be deleted
        /// </summary>
        [Browsable(false)]
        public bool CanDelete
        {
            get { return (this.Document != null && this.Document.GetElementById("wpConfirmB") != null); }
        }

        /// <summary>
        /// Gets a value indicating whether the page can be protected
        /// </summary>
        [Browsable(false)]
        public bool CanProtect
        {
            get { return (this.Document != null && this.Document.GetElementById("mw-Protect-submit") != null); }
        }

        /// <summary>
        /// Gets a value indicating whether the user is logged in
        /// </summary>
        public bool GetLogInStatus()
        {
            if (this.Document == null)
                return false;

            try
            {
                return !(UserName.Length == 0);
            }
            catch (Exception ex)
            {
                throw new WebBrowserOperationsException("Error getting log-in status", ex);
            }
        }

        /// <summary>
        /// Gets the user name if logged in
        /// </summary>
        [Browsable(false)]
        public string UserName
        {
            get
            {
                string s = GetScriptingVar("wgUserName");
                if (s == "null") return "";
                else return s;
            }
        }

        /// <summary>
        /// Login Function for use in AWB Profiles
        /// Allows username and password to be passed and then the user logged in
        /// </summary>
        public void Login(string username, string password)
        {
            this.LoadLogInPage();

            this.Wait();
            this.Document.GetElementById("wpName1").InnerText = username;
            this.Document.GetElementById("wpPassword1").InnerText = password;
            this.Document.GetElementById("wpRemember").SetAttribute("value", "1");
            this.Document.GetElementById("wpLoginattempt").InvokeMember("click");

            this.Wait();
        }

        public UserInfo GetUserInfo()
        {
            Navigate(Variables.URLLong + "api.php?action=query&meta=userinfo&uiprop=groups|rights");
            Wait();

            string s = Document.Body.InnerText;
            s = s.Remove(0, s.IndexOf("<api>"));
            s = s.Remove(s.IndexOf("</api>") + 6, s.Length - s.IndexOf("</api>") - 6);
            return new UserInfo(s);
        }

        static readonly Regex NewMessagesRegex = new Regex("(?<!<!-- start content -->.*)(<div class=['\"]usermessage['\"])", 
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Gets a value indicating whether there is a new message
        /// </summary>
        [Browsable(false)]
        public bool NewMessage
        {
            get
            {
                return NewMessagesRegex.IsMatch(DocumentText);
            }
        }

        static readonly Regex wpTextbox1 = new Regex(@"<textarea [^>]*?name=[""']wpTextbox1[""']", RegexOptions.Compiled);

        /// <summary>
        /// Gets a value indicating whether the textbox is present
        /// </summary>
        [Browsable(false)]
        public bool HasArticleTextBox
        {
            get { return wpTextbox1.IsMatch(DocumentText); }
        }

        string strStatus = "";
        /// <summary>
        /// Gets a string indicating the current status
        /// </summary>
        [Browsable(false)]
        public string Status
        {
            get
            { return strStatus; }
            private set
            {
                strStatus = value;

                if (this.StatusChanged != null)
                    this.StatusChanged(null, null);
            }
        }

        bool boolBusy;

        /// <summary>
        /// Gets a value indicating whether articles are still being processed
        /// </summary>
        [Browsable(false)]
        public bool Busy
        {
            get { return boolBusy; }
            set
            {
                bool b = boolBusy;
                boolBusy = value;
                if (b != boolBusy && BusyChanged != null)
                    BusyChanged(null, null);
            }
        }

        /// <summary>
        /// Gets a bool indicating if the current page is a diff
        /// </summary>
        [Browsable(false)]
        public bool IsDiff
        {
            get { return (this.Document.Body.InnerHtml.Contains("<DIV id=wikiDiff>")); }
        }

        [Browsable(false)]
        public bool IsUserPage
        {
            get { return ArticleTitle.StartsWith(Variables.Namespaces[2]); }
        }

        /// <summary>
        /// returns true if current page is a userpage
        /// </summary>
        [Browsable(false)]
        public bool IsUserTalk
        {
            get
            { return ArticleTitle.StartsWith(Variables.Namespaces[3]); }
        }

        [Browsable(false)]
        public bool IsUserSpace
        {
            get
            {
                string s = ArticleTitle;
                return s.StartsWith(Variables.Namespaces[2]) || s.StartsWith(Variables.Namespaces[3]);
            }
        }

        bool boolTalkExists = true;
        /// <summary>
        /// Gets a value indicating if the associated talk page exists
        /// </summary>
        [Browsable(false)]
        public bool TalkPageExists
        {
            get { return boolTalkExists; }
            private set { boolTalkExists = value; }

        }

        bool boolArticlePageExists = true;
        /// <summary>
        /// Gets a value indicating if the associated article page exists
        /// </summary>
        [Browsable(false)]
        public bool ArticlePageExists
        {
            get { return boolArticlePageExists; }
            private set { boolArticlePageExists = value; }
        }

        #endregion

        #region Methods

        public void Stop2()
        {
            StopTimer();
            ProcessStage = enumProcessStage.none;
            Busy = false;
            this.Stop();
        }

        /// <summary>
        /// Gets the text from the textbox
        /// </summary>
        public string GetArticleText()
        {
            if (HasArticleTextBox)
            {
                string txt = HttpUtility.HtmlDecode(this.Document.GetElementById("wpTextbox1").InnerHtml);
                if (txt == null)
                    return "";
                else
                    return txt;
            }
            else
                return "";
        }

        /// <summary>
        /// returns head of the currently loaded document or empty string of none
        /// </summary>
        public string GetHead()
        {
            if (Document == null) return "";
            try { return Tools.StringBetween(DocumentText, "<head>", "</head>"); }
            catch { return ""; }
        }

        /// <summary>
        /// returns value of one of JavaScript scripting variables set by MediaWiki
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetScriptingVar(string name)
        {
            if (this.Document == null)
                return "";

            try
            {
                Regex r = new Regex("var " + name + " = (.*?);\n");
                Match m = r.Match(GetHead());

                if (!m.Groups[1].Success)
                    return "";

                string s = m.Groups[1].Value.Trim('"');
                s = s.Replace("\\\"", "\"").Replace("\\'", "'");

                return s;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Sets the article text
        /// </summary>
        public void SetArticleText(string ArticleText)
        {
            if (HasArticleTextBox)
            {
                this.Document.GetElementById("wpTextbox1").Enabled = true;
                this.Document.GetElementById("wpTextbox1").InnerText = ArticleText.Trim();
            }
        }

        /// <summary>
        /// Sets the minor checkbox
        /// </summary>
        public void SetMinor(bool IsMinor)
        {
            if (this.Document == null || !this.Document.Body.InnerHtml.Contains("wpMinoredit"))
                return;

            if (IsMinor)
                this.Document.GetElementById("wpMinoredit").SetAttribute("checked", "checked");
            else
                this.Document.GetElementById("wpMinoredit").SetAttribute("checked", "");
        }

        /// <summary>
        /// Sets the watch checkbox
        /// </summary>
        public void SetWatch(bool watch1)
        {
            if (this.Document == null || !this.Document.Body.InnerHtml.Contains("wpWatchthis"))
                return;

            if (watch1)
                this.Document.GetElementById("wpWatchthis").SetAttribute("checked", "checked");

            Watch = watch1;
        }


        /// <summary>
        /// Sets the edit summary text
        /// </summary>
        public void SetSummary(string Summary)
        {
            if (this.Document == null || !this.Document.Body.InnerHtml.Contains("wpSummary"))
                return;

            this.Document.GetElementById("wpSummary").InnerText = Summary;
        }

        /// <summary>
        /// Sets the reason given for deletion or move, returns true if successful
        /// </summary>
        public bool SetReason(string Reason)
        {
            if (this.Document == null || !this.Document.Body.InnerHtml.Contains("wpReason"))
                return false;

            this.Document.GetElementById("wpReason").InnerText = Reason;
            return true;
        }

        /// <summary>
        /// Sets the reason given for protection, returns true if successful
        /// </summary>
        public bool SetReasonAndExpiry(string Reason, string Expiry)
        {
            if (this.Document == null || !this.Document.Body.InnerHtml.Contains("mwProtect-reason") || !this.Document.Body.InnerHtml.Contains("mwProtect-expiry"))
                return false;

            this.Document.GetElementById("mwProtect-reason").InnerText = Reason;
            this.Document.GetElementById("mwProtect-expiry").InnerText = Expiry;
            return true;
        }

        /// <summary>
        /// Gets a value indicating whether the minor checkbox is checked
        /// </summary>
        public bool IsMinor()
        {
            return (!(this.Document == null || this.Document.GetElementById("wpMinoredit").GetAttribute("checked") != "True"));
        }

        /// <summary>
        /// Gets a value indicating whether the watch checkbox is checked
        /// </summary>
        public bool IsWatched()
        {
            return (!(this.Document == null || this.Document.GetElementById("wpWatchthis").GetAttribute("checked") != "True"));
        }

        /// <summary>
        /// Gets the entered edit summary
        /// </summary>
        public string GetSummary()
        {
            if (this.Document == null || !this.Document.Body.InnerHtml.Contains("wpSummary"))
                return "";

            return this.Document.GetElementById("wpSummary").InnerText;
        }

        string startMark = "<!-- start content -->";
        string endMark = "<!-- end content -->";

        /// <summary>
        /// Gets the HTML within the <!-- start content --> and <!-- end content --> tags
        /// </summary>
        public string PageHTMLSubstring(string text)
        {
            if (this.Document.Body.InnerHtml.Contains(startMark) && this.Document.Body.InnerHtml.Contains(endMark))
                return text.Substring(text.IndexOf(startMark), text.IndexOf(endMark) - text.IndexOf(startMark));
            else
                return text;
        }

        /// <summary>
        /// wait for current operation to complete
        /// </summary>
        public void Wait()
        {
            while (ReadyState != WebBrowserReadyState.Complete && !Shutdown) Application.DoEvents();
        }

        #endregion

        #region Save/load/diff methods and events

        /// <summary>
        /// Invokes the Save button
        /// </summary>
        public void Save()
        {
            if (CanSave)
            {
                this.AllowNavigation = true;
                ProcessStage = enumProcessStage.save;
                Status = "Saving";
                this.Document.GetElementById("wpSave").InvokeMember("click");
            }
        }

        /// <summary>
        /// Invokes the Preview button
        /// </summary>
        public void ShowPreview()
        {
            if (CanPreview)
            {
                AllowNavigation = true;
                ProcessStage = enumProcessStage.diff;
                Status = "Loading preview";
                Document.GetElementById("wpPreview").InvokeMember("click");

                this.Wait();
            }
        }

        /// <summary>
        /// Invokes the Delete button
        /// </summary>
        public void Delete()
        {
            if (CanDelete)
            {
                this.AllowNavigation = true;
                ProcessStage = enumProcessStage.delete;
                Status = "Deleting page";
                this.Document.GetElementById("wpConfirmB").InvokeMember("click");

                Deleted(null, null);
            }
        }

        /// <summary>
        /// Loads the delete page of the given article
        /// </summary>
        public void LoadDeletePage(string Article)
        {
            try
            {
                this.AllowNavigation = true;
                ProcessStage = enumProcessStage.delete;
                Status = "Loading delete page";
                Navigate(Variables.URLLong + "index.php?title=" + HttpUtility.UrlEncode(Article) + "&action=delete");
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }

        /// <summary>
        /// Invokes the Protect button
        /// </summary>
        public void Protect(int EditProtectionLevel, int MoveProtectionLevel)
        {
            if (CanProtect)
            {
                this.AllowNavigation = true;
                ProcessStage = enumProcessStage.protect;
                Status = "Protecting page";

                HtmlElement editProtElement = this.Document.GetElementById("mwProtect-level-edit");
                if (EditProtectionLevel != 0)
                {
                    editProtElement.Children[0].SetAttribute("selected", "");
                    switch (EditProtectionLevel)
                    {
                        case 1:
                            editProtElement.Children[1].SetAttribute("selected", "selected");
                            editProtElement.Children[2].SetAttribute("selected", "");
                            break;
                        case 2:
                            editProtElement.Children[1].SetAttribute("selected", "");
                            editProtElement.Children[2].SetAttribute("selected", "selected");
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    editProtElement.Children[0].SetAttribute("selected", "selected");
                    editProtElement.Children[1].SetAttribute("selected", "");
                    editProtElement.Children[2].SetAttribute("selected", "");
                }

                HtmlElement moveProtElement = this.Document.GetElementById("mwProtect-level-move");
                if (MoveProtectionLevel != 0)
                {
                    moveProtElement.Children[0].SetAttribute("selected", "");
                    switch (MoveProtectionLevel)
                    {
                        case 1:
                            moveProtElement.Children[1].SetAttribute("selected", "selected");
                            moveProtElement.Children[2].SetAttribute("selected", "");
                            break;
                        case 2:
                            moveProtElement.Children[1].SetAttribute("selected", "");
                            moveProtElement.Children[2].SetAttribute("selected", "selected");
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    moveProtElement.Children[0].SetAttribute("selected", "selected");
                    moveProtElement.Children[1].SetAttribute("selected", "");
                    moveProtElement.Children[2].SetAttribute("selected", "");
                }

                this.Document.GetElementById("mw-Protect-submit").InvokeMember("click");
            }
        }

        /// <summary>
        /// Loads the protect page of the given article
        /// </summary>
        public void LoadProtectPage(string Article)
        {
            try
            {
                this.AllowNavigation = true;
                ProcessStage = enumProcessStage.protect;
                Status = "Loading protect page";
                Navigate(Variables.URLLong + "index.php?title=" + HttpUtility.UrlEncode(Article) + "&action=protect");
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }

        /// <summary>
        /// Loads the edit page of the given article
        /// </summary>
        public void LoadEditPage(string Article)
        {
            try
            {
                this.AllowNavigation = true;
                ProcessStage = enumProcessStage.load;
                Status = "Loading page";
                this.Navigate(Variables.URLLong + "index.php?title=" + HttpUtility.UrlEncode(Article) + "&action=edit&useskin=myskin");
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }

        /// <summary>
        /// Loads the edit page of the given article
        /// </summary>
        /// <param name="Article">Article title</param>
        /// <param name="Revision">Revision</param>
        public void LoadEditPage(string Article, int Revision)
        {
            LoadEditPage(Article, Revision.ToString());
        }

        /// <summary>
        /// Loads the edit page of the given article
        /// </summary>
        /// <param name="Article">Article title</param>
        /// <param name="Revision">Revision</param>
        public void LoadEditPage(string Article, string Revision)
        {
            try
            {
                this.AllowNavigation = true;
                ProcessStage = enumProcessStage.load;
                Status = "Loading page";
                this.Navigate(Variables.URLLong + "index.php?title=" + HttpUtility.UrlEncode(Article) + "&action=edit&oldid="
                    + Revision);
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }

        /// <summary>
        /// Loads the edit page of the given article
        /// </summary>
        /// <param name="Article">Article title</param>
        /// <param name="Section">Section name</param>
        public void LoadEditPageSection(string Article, string Section)
        {
            try
            {
                this.AllowNavigation = true;
                ProcessStage = enumProcessStage.load;
                Status = "Loading page";
                string url = Variables.URLLong + "index.php?title=" + HttpUtility.UrlEncode(Article) + "&action=edit&section=" + Section;
                this.Navigate(url);
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }

        /// <summary>
        /// Loads the log in page
        /// </summary>
        public void LoadLogInPage()
        {
            this.AllowNavigation = true;
            ProcessStage = enumProcessStage.none;
            Status = "Loading log in page";
            this.Navigate(Variables.URLLong + "index.php?title=Special:Userlogin&returnto=Main_Page");
            Busy = false;
        }

        /// <summary>
        /// Allows user to logout
        /// </summary>
        public void LoadLogOut()
        {
            this.AllowNavigation = true;
            ProcessStage = enumProcessStage.none;
            Status = "Logging Out";
            this.Navigate(Variables.URLLong + "index.php?title=Special:Userlogout");
            Busy = false;
        }

        Regex RegexArticleExists = new Regex("<LI (class=new|class=\"selected new\") id=ca-nstab", RegexOptions.Compiled);
        Regex RegexArticleTalkExists = new Regex("<LI (class=new|class=\"selected new\") id=ca-talk", RegexOptions.Compiled);

        protected override void OnDocumentCompleted(WebBrowserDocumentCompletedEventArgs e)
        {
            base.OnDocumentCompleted(e);
            StopTimer();

            if (!this.Document.Body.InnerHtml.Contains("id=siteSub"))
            {
                ProcessStage = enumProcessStage.none;
                if (Fault != null)
                    this.Fault(null, null);
                return;
            }

            if (this.Url.AbsolutePath.Contains("Userlogout"))
                this.AllowNavigation = false;
            else if (ProcessStage == enumProcessStage.load)
            {
                TalkPageExists = !RegexArticleTalkExists.IsMatch(this.Document.Body.InnerHtml);

                ArticlePageExists = !RegexArticleExists.IsMatch(this.Document.Body.InnerHtml);

                this.AllowNavigation = false;
                ProcessStage = enumProcessStage.none;

                Status = "Ready to save";

                if (Loaded != null)
                    this.Loaded(null, null);

                this.Document.GetElementById("wpTextbox1").Enabled = false;
            }
            else if (ProcessStage == enumProcessStage.diff)
            {
                this.AllowNavigation = false;
                ProcessStage = enumProcessStage.none;
                Status = "Ready to save";
            }
            else if (ProcessStage == enumProcessStage.none)
            {
                if (None != null)
                    this.None(null, null);
            }
        }

        protected override void OnProgressChanged(WebBrowserProgressChangedEventArgs e)
        {
            if (this.ReadyState == WebBrowserReadyState.Interactive && ProcessStage == enumProcessStage.save)
            {
                StopTimer();
                this.OnDocumentCompleted(null);
                this.AllowNavigation = false;
                ProcessStage = enumProcessStage.none;
                this.Stop();
                if (this.Saved != null)
                    this.Saved(null, null);
            }
            base.OnProgressChanged(e);
        }

        /// <summary>
        /// Moves an article, returns true if successful
        /// </summary>
        public bool MovePage(string OldTitle, string NewTitle, string Summary)
        {
            AllowNavigation = true;

            Navigate(Variables.URL + "/wiki/Special:Movepage/" + OldTitle);
            Status = "Loading move page";
            Wait();

            if (this.Document == null || !this.Document.Body.InnerHtml.Contains("wpNewTitle"))
            {
                AllowNavigation = false;
                return false;
            }

            Document.GetElementById("wpNewTitle").InnerText = NewTitle;

            if (!SetReason(Summary))
            {
                AllowNavigation = false;
                return false;
            }

            foreach (HtmlElement e in Document.GetElementById("movepage").GetElementsByTagName("input"))
            {
                if (e.GetAttribute("name") == "wpMove")
                {
                    Status = "Moving";
                    e.InvokeMember("click");
                    Wait();

                    AllowNavigation = false;
                    if (e.Document.GetElementById("movepage") != null) return false;

                    Status = "Moved";
                    return true;
                }
            }

            AllowNavigation = false;
            return false;
        }

        /// <summary>
        /// Sets unwatched pages to watched, and watched pages to unwatched
        /// </summary>
        public void WatchUnwatch()
        {
            if (this.Document == null)
                return;

            if (IsWatched())
                this.Document.GetElementById("ca-watch").InvokeMember("click");
            else
                this.Document.GetElementById("ca-un").InvokeMember("click");

            Wait();
            AllowNavigation = false;
        }

        /// <summary>
        /// Deletes an article, returns true if successful
        /// </summary>
        public bool DeletePage(string Article, string Summary)
        {
            LoadDeletePage(Article);
            Wait();

            if (this.Document == null)
            {
                AllowNavigation = false;
                return false;
            }

            if (!SetReason(Summary))
            {
                AllowNavigation = false;
                return false;
            }

            Delete();
            Wait();
            AllowNavigation = false;

            Status = "Protected";
            return true;
        }

        /// <summary>
        /// Protects an article, returns true if successful
        /// </summary>
        public bool ProtectPage(string Article, string Summary, int EditProtectionLevel, int MoveProtectionLevel, string ProtectExpiry)
        {
            LoadProtectPage(Article);
            Wait();

            if (this.Document == null)
            {
                AllowNavigation = false;
                return false;
            }

            if (!SetReasonAndExpiry(Summary, ProtectExpiry))
            {
                AllowNavigation = false;
                return false;
            }

            Protect(EditProtectionLevel, MoveProtectionLevel);
            Wait();
            AllowNavigation = false;

            Status = "Protected";
            return true;
        }

        #endregion

        #region IRCMonitor-related
        [Browsable(false)]
        public string AdminRollbackUrl
        {
            get
            {
                string url = "";
                foreach (HtmlElement h in Document.Links)
                {
                    string s = h.GetAttribute("href").ToString();
                    if (s.Contains("action=rollback")) url = s;
                }
                return url;
            }
        }

        #endregion

        protected override void OnNavigating(WebBrowserNavigatingEventArgs e)
        {
            base.OnNavigating(e);
            StartTimer();
        }

        int LoadTime;
        private void StartTimer()
        {
            StopTimer();
            timer1.Tick += IncrementTime;
        }

        private void StopTimer()
        {
            timer1.Tick -= IncrementTime;
            LoadTime = 0;
        }

        private void IncrementTime(object sender, EventArgs e)
        {
            LoadTime++;

            if (LoadTime == timeout)
            {
                StopTimer();
                Stop2();
                Status = "Timed out";
                if (this.Fault != null)
                    this.Fault(null, null);
            }
        }

        int timeout = 30;
        public int TimeoutLimit
        {
            get { return timeout; }
            set { timeout = value; }
        }
    }

    [Serializable]
    public class WebBrowserOperationsException : ApplicationException
    {
        public WebBrowserOperationsException()
            : base("Web browser operations exception") { }

        public WebBrowserOperationsException(string message)
            : base(message) { }

        public WebBrowserOperationsException(string message, Exception inner)
            : base("Web browser operations exception: " + message, inner) { }

        protected WebBrowserOperationsException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
