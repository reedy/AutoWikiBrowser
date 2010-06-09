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

        public bool IsBusy
        {
            get
            {
                return Editor.IsActive;
            }
        }

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

        private AsyncApiEdit CreateEditor(string url, bool php5)
        {
            AsyncApiEdit edit = new AsyncApiEdit(url, parentControl, php5)
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
            edit.Aborted += OnAborted;

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

        public event AsyncEventHandler Aborted;

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

        void OnAborted(AsyncApiEdit sender)
        {
            if (Aborted != null) Aborted(sender);
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
                Editor = CreateEditor(Variables.URLLong, Variables.PHP5);
            }

            try
            {
                LoadProjectOptions();
                RequireUpdate();
            }
            catch
            {
                Editor = CreateEditor("http://en.wikipedia.org/w/", false);
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
                if (Updater.Result == Updater.AWBEnabledStatus.None)
                    Updater.CheckForUpdates();

                //load check page
                string url;
                if (Variables.IsWikia)
                    url = "http://community.wikia.com/index.php?title=Wikia:AutoWikiBrowser/CheckPage&action=raw";
                else if ((Variables.Project == ProjectEnum.wikipedia) && (Variables.LangCode == "ar"))
                    url =
                        "http://ar.wikipedia.org/w/index.php?title=%D9%88%D9%8A%D9%83%D9%8A%D8%A8%D9%8A%D8%AF%D9%8A%D8%A7:%D8%A7%D9%84%D8%A3%D9%88%D8%AA%D9%88%D9%88%D9%8A%D9%83%D9%8A_%D8%A8%D8%B1%D8%A7%D9%88%D8%B2%D8%B1/%D9%85%D8%B3%D9%85%D9%88%D8%AD&action=raw";
                else
                    url = Variables.URLIndex + "?title=Project:AutoWikiBrowser/CheckPage&action=raw";

                string checkPageText = Editor.SynchronousEditor.HttpGet(url);

                Variables.RTL = Site.IsRightToLeft;
                Variables.CapitalizeFirstLetter = Site.CapitalizeFirstLetter;

                if (Variables.IsCustomProject || Variables.IsWikia)
                {
                    Variables.LangCode = Site.Language;
                }

                string typoPostfix = "", localCheckPage = "";
                if (Variables.IsWikia)
                {
                    typoPostfix = "-" + Variables.LangCode;

                    try
                    {
                        //load a local checkpage on Wikia
                        //it cannot be used to approve users, but it could be used to set some settings
                        //such as underscores and pages to ignore

                        localCheckPage = Editor.SynchronousEditor.Open("Project:AutoWikiBrowser/CheckPage", true);

                        // selectively add content of the local checkpage to the global one
                        checkPageText += Message.Match(localCheckPage).Value
                                         /*+ Underscores.Match(s).Value*/
                                         + WikiRegexes.NoGeneralFixes.Match(localCheckPage);
                    }
                    catch
                    {
                    }
                }

                Updater.WaitForCompletion();
                Updater.AWBEnabledStatus versionStatus = Updater.Result;
                VersionCheckPage = Updater.GlobalVersionPage;

                //see if this version is enabled
                if (versionStatus == Updater.AWBEnabledStatus.Disabled)
                    return WikiStatusResult.OldVersion;

                CheckPageText = checkPageText;

                if (!User.IsLoggedIn)
                    return WikiStatusResult.NotLoggedIn;

                if (!User.HasRight("writeapi"))
                    return WikiStatusResult.NoRights;

                // TODO: assess the impact on servers later
                Editor.Maxlag = /*User.IsBot ? 5 : 20*/ -1;

                // check if username is globally blacklisted
                foreach (Match badName in BadName.Matches(Updater.GlobalVersionPage))
                {
                    if (!string.IsNullOrEmpty(badName.Groups[1].Value.Trim()) &&
                        !string.IsNullOrEmpty(User.Name) &&
                        Regex.IsMatch(User.Name, badName.Groups[1].Value.Trim(),
                                      RegexOptions.IgnoreCase | RegexOptions.Multiline))
                        return WikiStatusResult.NotRegistered;
                }

                //see if there is a message
                Match messages = Message.Match(checkPageText);
                if (messages.Success && messages.Groups[1].Value.Trim().Length > 0)
                {
                    MessageBox.Show(messages.Groups[1].Value, "Automated message", MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }

                //see if there is a version-specific message
                messages = VersionMessage.Match(checkPageText);
                if (messages.Success && messages.Groups[1].Value.Trim().Length > 0 &&
                    messages.Groups[1].Value == AWBVersion)
                {
                    MessageBox.Show(messages.Groups[2].Value, "Automated message", MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }

                bool foundTypoLink = false;
                if (Variables.IsWikia && !string.IsNullOrEmpty(localCheckPage))
                {
                    foundTypoLink = HasTypoLink(localCheckPage, typoPostfix);
                }

                if (!foundTypoLink)
                {
                    HasTypoLink(checkPageText, typoPostfix);
                }

                List<string> us = new List<string>();
                foreach (Match underscore in Underscores.Matches(checkPageText))
                {
                    if (underscore.Success && underscore.Groups[1].Value.Trim().Length > 0)
                        us.Add(underscore.Groups[1].Value.Trim());
                }
                if (us.Count > 0) Variables.LoadUnderscores(us.ToArray());

                //don't require approval if checkpage does not exist.
                if (checkPageText.Length < 1)
                {
                    IsBot = true;
                    return WikiStatusResult.Registered;
                }

                if (checkPageText.Contains("<!--All users enabled-->"))
                {
                    //see if all users enabled
                    IsBot = true;
                    return WikiStatusResult.Registered;
                }

                //see if we are allowed to use this software
                checkPageText = Tools.StringBetween(checkPageText, "<!--enabledusersbegins-->",
                                                    "<!--enabledusersends-->");

                string strBotUsers = Tools.StringBetween(checkPageText, "<!--enabledbots-->", "<!--enabledbotsends-->");
                Regex username = new Regex(@"^\*\s*" + Tools.CaseInsensitive(Regex.Escape(User.Name))
                                           + @"\s*$", RegexOptions.Multiline);

                if (IsSysop && Variables.Project != ProjectEnum.wikia)
                {
                    IsBot = username.IsMatch(strBotUsers);
                    return WikiStatusResult.Registered;
                }

                if (username.IsMatch(checkPageText))
                {
                    //enable bot mode
                    IsBot = username.IsMatch(strBotUsers);

                    return WikiStatusResult.Registered;
                }

                if (Variables.Project != ProjectEnum.custom)
                {
                    string globalUsers = Tools.StringBetween(VersionCheckPage, "<!--globalusers-->",
                                                             "<!--globalusersend-->");

                    if (username.IsMatch(globalUsers))
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

        private static bool HasTypoLink(string text, string typoPostfix)
        {
            Match typoLink = Regex.Match(text, "<!--[Tt]ypos" + typoPostfix + ":(.*?)-->");
            if (typoLink.Success && typoLink.Groups[1].Value.Trim().Length > 0)
            {
                Variables.RetfPath = typoLink.Groups[1].Value.Trim();
                return true;
            }

            return false;
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
