/*
Copyright (C) 2009

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
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WikiFunctions.API;

namespace WikiFunctions
{
    /// <summary>
    /// This class controls editing process in one wiki
    /// </summary>
    public class Session
    {
        #region Properties
        public AsyncApiEdit Editor
        { get; private set; }

        public UserInfo User
        { get { return Editor.User; } }

        public PageInfo Page
        { get { return Editor.Page; } }

        public SiteInfo Site
        { get; private set; }

        public bool IsBot
        { get; private set; }

        public bool IsSysop
        {
            get { return Editor.User.IsSysop; }
        }

        public string CheckPageText
        { get; private set; }

        public string VersionCheckPage
        { get; private set; }

        #endregion

        readonly Control parentControl;

        public Session(Control parent)
        {
            parentControl = parent;
            UpdateProject();
            Update();
        }

        private AsyncApiEdit CreateEditor()
        {
            AsyncApiEdit edit = new AsyncApiEdit(Variables.URLLong, parentControl, Variables.PHP5)
                                    {
                                        NewMessageThrows = false
                                    };

            edit.OpenComplete += OnOpenComplete;
            edit.SaveComplete += OnSaveComplete;
            edit.PreviewComplete += OnPreviewComplete;
            edit.ExceptionCaught += OnExceptionCaught;
            edit.MaxlagExceeded += OnMaxlagExceeded;
            edit.LoggedOff += OnLoggedOff;
            edit.StateChanged += OnStateChanged;

            return edit;
        }

        #region Events

        public event AsyncOpenEditHandler OpenComplete;
        public event AsyncSaveEventHandler SaveComplete;
        public event AsyncStringEventHandler PreviewComplete;

        public event AsyncExceptionEventHandler ExceptionCaught;
        public event AsyncMaxlagEventHandler MaxlagExceeded;
        public event AsyncEventHandler LoggedOff;

        public event AsyncEventHandler StateChanged;


        void OnOpenComplete(AsyncApiEdit sender, PageInfo pageInfo)
        {
            if (OpenComplete != null) OpenComplete(sender, pageInfo);
        }

        void OnSaveComplete(AsyncApiEdit sender, SaveInfo saveInfo)
        {
            if (SaveComplete != null) SaveComplete(sender, saveInfo);
        }

        void OnPreviewComplete(AsyncApiEdit sender, string result)
        {
            if (PreviewComplete != null) PreviewComplete(sender, result);
        }

        void OnExceptionCaught(AsyncApiEdit sender, Exception ex)
        {
            if (ExceptionCaught != null) ExceptionCaught(sender, ex);
        }

        void OnMaxlagExceeded(AsyncApiEdit sender, int maxlag, int retryAfter)
        {
            if (MaxlagExceeded != null) MaxlagExceeded(sender, maxlag, retryAfter);
        }

        void OnLoggedOff(AsyncApiEdit sender)
        {
            if (LoggedOff != null) LoggedOff(sender);
        }

        void OnStateChanged(AsyncApiEdit sender)
        {
            if (StateChanged != null) StateChanged(sender);
        }

        #endregion

        private readonly static Regex Message = new Regex("<!--[Mm]essage:(.*?)-->", RegexOptions.Compiled);
        private readonly static Regex VersionMessage = new Regex("<!--VersionMessage:(.*?)\\|\\|\\|\\|(.*?)-->", RegexOptions.Compiled);
        private readonly static Regex Underscores = new Regex("<!--[Uu]nderscores:(.*?)-->", RegexOptions.Compiled);

        WikiStatusResult status;
        public WikiStatusResult Status
        {
            get
            {
                if (status == WikiStatusResult.PendingUpdate)
                    Update();

                return status;
            }
            private set
            {
                status = value;
            }
        }

        public bool UpdateProject()
        {
            // recreate only if project changed, to prevent losing login information
            if (Editor == null || Editor.URL != Variables.URLLong || Editor.PHP5 != Variables.PHP5)
            {
                Editor = CreateEditor();
            }

            try
            {
                LoadProjectOptions();
                RequireUpdate();
            }
            catch
            {
                return false; // Error reporting delayed until UpdateWikiStatus()
            }

            return true;
        }

        public void RequireUpdate()
        {
            status = WikiStatusResult.PendingUpdate;
        }

        public WikiStatusResult Update()
        {
            Status = UpdateWikiStatus();
            return Status;
        }

        public static string AWBVersion
        { get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); } }

        private static readonly Regex BadName = new Regex(@"badname:\s*(.*)\s*(:?|#.*)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Checks log in status, registered and version.
        /// </summary>
        private WikiStatusResult UpdateWikiStatus()
        {
            try
            {
                IsBot = false;

                Site = new SiteInfo(Editor.SynchronousEditor);

                //load version check page if no status set
                if (Updater.Result == Updater.AWBEnabledStatus.None || Updater.Result == Updater.AWBEnabledStatus.Error)
                    Updater.Update();

                //load check page
                string url;
                if (Variables.IsWikia)
                    url = "http://www.wikia.com/index.php?title=Wikia:AutoWikiBrowser/CheckPage&action=raw";
                else if ((Variables.Project == ProjectEnum.wikipedia) && (Variables.LangCode == "ar"))
                    url = "http://ar.wikipedia.org/w/index.php?title=%D9%88%D9%8A%D9%83%D9%8A%D8%A8%D9%8A%D8%AF%D9%8A%D8%A7:%D8%A7%D9%84%D8%A3%D9%88%D8%AA%D9%88%D9%88%D9%8A%D9%83%D9%8A_%D8%A8%D8%B1%D8%A7%D9%88%D8%B2%D8%B1/%D9%85%D8%B3%D9%85%D9%88%D8%AD&action=raw";
                else
                    url = Variables.URLIndex + "?title=Project:AutoWikiBrowser/CheckPage&action=raw";

                string strText = Editor.SynchronousEditor.HttpGet(url);

                Variables.RTL = Site.IsRightToLeft;

                if (Variables.IsCustomProject || Variables.IsWikia)
                {
                    Variables.LangCode = Site.Language;
                }

                string typoPostfix = "";
                if (Variables.IsWikia)
                {
                    typoPostfix = "-" + Variables.LangCode;

                    try
                    {
                        //load a local checkpage on Wikia
                        //it cannot be used to approve users, but it could be used to set some settings
                        //such as underscores and pages to ignore

                        string s = Editor.SynchronousEditor.Open("Project:AutoWikiBrowser/CheckPage");

                        // selectively add content of the local checkpage to the global one
                        strText += Message.Match(s).Value
                            /*+ Underscores.Match(s).Value*/
                                   + WikiRegexes.NoGeneralFixes.Match(s);
                    }
                    catch { }

                }

                Updater.WaitForCompletion();
                Updater.AWBEnabledStatus versionStatus = Updater.Result;
                VersionCheckPage = Updater.GlobalVersionPage;

                //see if this version is enabled
                if (versionStatus == Updater.AWBEnabledStatus.Disabled)
                    return WikiStatusResult.OldVersion;

                CheckPageText = strText;

                if (!Editor.User.IsLoggedIn)
                    return WikiStatusResult.NotLoggedIn;

                Editor.Maxlag = Editor.User.IsBot ? 5 : 20;

                // check if username is globally blacklisted
                foreach (Match m3 in BadName.Matches(Updater.GlobalVersionPage))
                {
                    if (!string.IsNullOrEmpty(m3.Groups[1].Value.Trim()) &&
                        !string.IsNullOrEmpty(Editor.User.Name) &&
                        Regex.IsMatch(Editor.User.Name, m3.Groups[1].Value.Trim(),
                                      RegexOptions.IgnoreCase | RegexOptions.Multiline))
                        return WikiStatusResult.NotRegistered;
                }

                //see if there is a message
                Match m = Message.Match(strText);
                if (m.Success && m.Groups[1].Value.Trim().Length > 0)
                    MessageBox.Show(m.Groups[1].Value, "Automated message", MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);

                //see if there is a version-specific message
                m = VersionMessage.Match(strText);
                if (m.Success && m.Groups[1].Value.Trim().Length > 0 && m.Groups[1].Value == AWBVersion)
                    MessageBox.Show(m.Groups[2].Value, "Automated message", MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);

                m = Regex.Match(strText, "<!--[Tt]ypos" + typoPostfix + ":(.*?)-->");
                if (m.Success && m.Groups[1].Value.Trim().Length > 0)
                    Variables.RetfPath = m.Groups[1].Value.Trim();

                List<string> us = new List<string>();
                foreach (Match m1 in Underscores.Matches(strText))
                {
                    if (m1.Success && m1.Groups[1].Value.Trim().Length > 0)
                        us.Add(m1.Groups[1].Value.Trim());
                }
                if (us.Count > 0) Variables.LoadUnderscores(us.ToArray());

                //don't require approval if checkpage does not exist.
                if (strText.Length < 1)
                {
                    IsBot = true;
                    return WikiStatusResult.Registered;
                }

                if (strText.Contains("<!--All users enabled-->"))
                {
                    //see if all users enabled
                    IsBot = true;
                    return WikiStatusResult.Registered;
                }

                //see if we are allowed to use this software
                strText = Tools.StringBetween(strText, "<!--enabledusersbegins-->", "<!--enabledusersends-->");

                string strBotUsers = Tools.StringBetween(strText, "<!--enabledbots-->", "<!--enabledbotsends-->");
                Regex username = new Regex(@"^\*\s*" + Tools.CaseInsensitive(Regex.Escape(Editor.User.Name))
                                           + @"\s*$", RegexOptions.Multiline);

                if (IsSysop && Variables.Project != ProjectEnum.wikia)
                {
                    IsBot = username.IsMatch(strBotUsers);
                    return WikiStatusResult.Registered;
                }

                if (!string.IsNullOrEmpty(Editor.User.Name) && username.IsMatch(strText))
                {
                    //enable bot mode
                    IsBot = username.IsMatch(strBotUsers);

                    return WikiStatusResult.Registered;
                }

                return WikiStatusResult.NotRegistered;
            }
            catch (Exception ex)
            {
                Tools.WriteDebug(ToString(), ex.Message);
                Tools.WriteDebug(ToString(), ex.StackTrace);
                IsBot = false;
                return WikiStatusResult.Error;
            }
        }

        /// <summary>
        /// Loads namespaces
        /// </summary>
        private void LoadProjectOptions()
        {
            string[] months = (string[])Variables.ENLangMonthNames.Clone();

            try
            {
                Site = new SiteInfo(Editor.SynchronousEditor);

                for (int i = 0; i < months.Length; i++) months[i] += "-gen";
                Dictionary<string, string> messages = Site.GetMessages(months);

                if (messages.Count == 12)
                {
                    for (int i = 0; i < months.Length; i++)
                    {
                        months[i] = messages[months[i]];
                    }
                    Variables.MonthNames = months;
                }

                Variables.Namespaces = Site.Namespaces;
                Variables.NamespaceAliases = Site.NamespaceAliases;
                Variables.MagicWords = Site.MagicWords;
            }
            catch (Exception ex)
            {
                //TODO:Better error handling

                string message = ex is WikiUrlException && ex.InnerException != null 
                    ? ex.InnerException.Message
                    : ex.Message;

                MessageBox.Show("An error occured while connecting to the server or loading project information from it. " +
                        "Please make sure that your internet connection works and such combination of project/language exist." +
                        "\r\nEnter the URL in the format \"en.wikipedia.org/w/\" (including path where index.php and api.php reside)." +
                        "\r\nError description: " + message,
                        "Error connecting to wiki", MessageBoxButtons.OK, MessageBoxIcon.Error);

                throw;
            }
        }
    }
}
