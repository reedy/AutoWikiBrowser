/*
Autowikibrowser
Copyright (C) 2006 Martin Richards

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

namespace WikiFunctions
{
    public delegate void LoadDel();
    public delegate void SaveDel();
    public delegate void DiffDel();
    public delegate void DeleteDel();
    public delegate void NoneDel();
    public delegate void FaultDel();
    public delegate void StatusDel();

    public enum enumProcessStage : byte { load, diff, save, delete, none }

    /// <summary>
    /// Provides a webBrowser component adapted speciailly to work with Wikis.
    /// </summary>
    public partial class WebControl : WebBrowser
    {
        #region Constructor etc.

        public WebControl()
        {
            InitializeComponent();
            this.ScriptErrorsSuppressed = true;
            ProcessStage = enumProcessStage.none;
        }

        public enumProcessStage ProcessStage = new enumProcessStage();
        /// <summary>
        /// Occurs when the edit page has finished loading
        /// </summary>
        public event LoadDel Loaded;
        /// <summary>
        /// Occurs when the page has finished saving
        /// </summary>
        public event SaveDel Saved;
        /// <summary>
        /// Occurs when the diff or preview page has finished loading
        /// </summary>
        public event DiffDel Diffed;
        /// <summary>
        /// Occurs when the page has finished deleting
        /// </summary>
        public event DeleteDel Deleted;
        /// <summary>
        /// Occurs when the page has finished loading, and it was not a save/load/diff
        /// </summary>
        public event DiffDel None;
        /// <summary>
        /// Occurs when the page failed to load properly
        /// </summary>
        public event FaultDel Fault;
        /// <summary>
        /// Occurs when the status changes
        /// </summary>
        public event StatusDel StatusChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether the page can be saved
        /// </summary>
        public bool CanSave
        {
            get
            {
                if (this.Document != null && this.Document.GetElementById("wpSave") != null)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the page can be diffed
        /// </summary>
        public bool CanDiff
        {
            get
            {
                if (this.Document != null && this.Document.GetElementById("wpDiff") != null)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the page can be previewed
        /// </summary>
        public bool CanPreview
        {
            get
            {
                if (this.Document != null && this.Document.GetElementById("wpPreview") != null)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the page can be deleted
        /// </summary>
        public bool CanDelete
        {
            get
            {
                if (this.Document != null && this.Document.GetElementById("wpConfirmB") != null)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the user is logged in
        /// </summary>
        public bool LoggedIn
        {
            get
            {
                if (this.Document != null && this.Document.GetElementById("pt-logout") != null)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the textbox is present
        /// </summary>
        public bool HasArticleTextBox
        {
            get
            {
                if (this.Document != null && this.Document.GetElementById("wpTextbox1") != null)
                    return true;
                else
                    return false;
            }
        }
        
        string strStatus = "";
        /// <summary>
        /// Gets a string indicating the current status
        /// </summary>
        public string Status
        {
            get
            {
                return strStatus;
            }
            private set
            {
                strStatus = value;
                this.StatusChanged();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the text from the textbox
        /// </summary>
        public string GetArticleText()
        {
            if (HasArticleTextBox)
                return this.Document.GetElementById("wpTextbox1").InnerText;
            else
                return "";
        }

        /// <summary>
        /// Sets the article text
        /// </summary>
        public void SetArticleText(string ArticleText)
        {
            if (HasArticleTextBox)
            {
                this.Document.GetElementById("wpTextbox1").Enabled = true;
                this.Document.GetElementById("wpTextbox1").InnerText = ArticleText;
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
        public void SetWatch(bool Watch)
        {
            if (this.Document == null || !this.Document.Body.InnerHtml.Contains("wpWatchthis"))
                return;

            if (Watch)
                this.Document.GetElementById("wpWatchthis").SetAttribute("checked", "checked");
            else
                this.Document.GetElementById("wpWatchthis").SetAttribute("checked", "");
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
        /// Sets the reason given for deletion
        /// </summary>
        public void SetDeleteReason(string Reason)
        {
            if (this.Document == null || !this.Document.Body.InnerHtml.Contains("wpReason"))
                return;

            this.Document.GetElementById("wpReason").InnerText = Reason;
        }

        /// <summary>
        /// Gets a value indicating whether the minor checkbox is checked
        /// </summary>
        public bool IsMinor()
        {
            if (this.Document == null || this.Document.GetElementById("wpMinoredit").GetAttribute("checked") != "True")
                return false;
            else
                return true;
        }

        /// <summary>
        /// Gets a value indicating whether the watch checkbox is checked
        /// </summary>
        public bool IsWatched()
        {
            if (this.Document == null || this.Document.GetElementById("wpWatchthis").GetAttribute("checked") != "True")
                return false;
            else
                return true;
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
        public string PageHTMLSubstring
        {
            get
            {
                string text = this.Document.Body.InnerHtml;
                if (this.Document.Body.InnerHtml.Contains(startMark) && this.Document.Body.InnerHtml.Contains(endMark))
                    return text.Substring(text.IndexOf(startMark), text.IndexOf(endMark) - text.IndexOf(startMark));
                else
                    return text;
            }
        }

        /// <summary>
        /// If logged in, gets the user name from the cookie
        /// </summary>
        public string GetUserName()
        {
            if (this.Document.Cookie == null)
                return "";

            string cookieText = this.Document.Cookie;
            Match m = Regex.Match(cookieText, "enwikiUserName=(.*?); ");
            string UserName = m.Groups[1].Value;
            UserName = UserName.Replace("+", " ");
            UserName = HttpUtility.UrlDecode(UserName);

            return UserName;
        }

        ///// <summary>
        ///// Removes excess HTML from the document
        ///// </summary>
        //public void RemoveHTML()
        //{
        //    //string html = this.Document.Body.InnerHtml;

        //    //html = html.Substring(html.IndexOf("<TABLE"), html.IndexOf("</TABLE>") - html.IndexOf("<TABLE"));

        //    //this.Document.Body.InnerHtml = html;

        //    this.Document.Body.InnerHtml = PageHTMLSubstring;
        //}

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
        /// Invokes the Show changes button
        /// </summary>
        public void ShowDiff()
        {
            if (CanDiff)
            {
                this.AllowNavigation = true;
                ProcessStage = enumProcessStage.diff;
                Status = "Loading changes";
                this.Document.GetElementById("wpDiff").InvokeMember("click");
            }
        }

        /// <summary>
        /// Invokes the Preview button
        /// </summary>
        public void ShowPreview()
        {
            if (CanPreview)
            {
                this.AllowNavigation = true;
                ProcessStage = enumProcessStage.diff;
                Status = "Loading preview";
                this.Document.GetElementById("wpPreview").InvokeMember("click");
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
                this.Navigate(Variables.URL + "index.php?title=" + Article + "&action=edit");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
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
            this.Navigate(Variables.URL + "index.php?title=Special:Userlogin&returnto=Main_Page");
        }

        protected override void OnDocumentCompleted(WebBrowserDocumentCompletedEventArgs e)
        {
            base.OnDocumentCompleted(e);

            if (!this.Document.Body.InnerHtml.Contains("id=siteSub"))
            {
                ProcessStage = enumProcessStage.none;
                this.Fault();
                return;
            }

            if (ProcessStage == enumProcessStage.load)
            {
                this.AllowNavigation = false;
                ProcessStage = enumProcessStage.none;
                this.Loaded();
            }
            else if (ProcessStage == enumProcessStage.delete)
            {
                this.AllowNavigation = false;
                ProcessStage = enumProcessStage.none;
                Status = "Deleted";
                this.Deleted();
            }
            else if (ProcessStage == enumProcessStage.diff)
            {
                this.AllowNavigation = false;
                ProcessStage = enumProcessStage.none;
                Status = "Ready to save";
                this.Diffed();
                //RemoveHTML();
                this.Document.GetElementById("wpTextbox1").Enabled = false;
            }
            else if (ProcessStage == enumProcessStage.none)
            {
                this.None();
            }
        }

        protected override void OnProgressChanged(WebBrowserProgressChangedEventArgs e)
        {
            if (this.ReadyState == WebBrowserReadyState.Interactive && ProcessStage == enumProcessStage.save)
            {
                this.OnDocumentCompleted(null);
                this.AllowNavigation = false;
                this.Saved();
            }
            base.OnProgressChanged(e);
        }

        #endregion
               
    }
}
