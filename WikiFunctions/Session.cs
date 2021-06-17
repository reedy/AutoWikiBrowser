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
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Authentication;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
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

        /// <summary>
        /// Gets the wikitext check page text.
        /// </summary>
        /// <value>The check page text. (may be blank)</value>
        public string CheckPageText
        { get; private set; }

        /// <summary>
        /// Gets the check page JSON Text.
        /// </summary>
        /// <value>The check page JSON Text.</value>
        public string CheckPageJSONText
        { get; private set; }

        /// <summary>
        /// Config Page JSON text
        /// </summary>
        private string ConfigJSONText { get; set; }

        /// <summary>
        /// Gets the JSON of version check page.
        /// </summary>
        /// <value>The JSON of version check page.</value>
        public string VersionCheckPage
        { get; private set; }

        #endregion

        readonly Control parentControl;

        public Session(Control parent)
        {
            parentControl = parent;
            UpdateProject(true);
        }

        private AsyncApiEdit CreateEditor(string url)
        {
            AsyncApiEdit edit = new AsyncApiEdit(url, parentControl)
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

        void OnMaxlagExceeded(AsyncApiEdit sender, double maxlag, int retryAfter)
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

        private static readonly Regex Message = new Regex("<!--[Mm]essage:(.*?)-->", RegexOptions.Compiled);
        private static readonly Regex VersionMessage = new Regex("<!--VersionMessage:(.*?)\\|\\|\\|\\|(.*?)-->", RegexOptions.Compiled);
        private static readonly Regex Underscores = new Regex("<!--[Uu]nderscores:(.*?)-->", RegexOptions.Compiled);

        /// <summary>
        /// Default template of what would exist at Project:AutoWikiBrowser/Config, to be used in case of it not existing
        /// </summary>
        private const string DefaultWikiConfig = "{ 'typolink': '', 'allusersenabled': false, 'allusersenabledusermode': false, 'messages': [], 'underscoretitles': [], 'nogenfixes': [], 'noregextypofix': [] }";

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

        public bool UpdateProject(bool delayLoading)
        {
            // recreate only if project changed, to prevent losing login information
            if (Editor == null || Editor.URL != Variables.URLLong)
            {
                Editor = CreateEditor(Variables.URLLong);
            }

            if (delayLoading)
            {
                return true;
            }
            try
            {
                LoadProjectOptions();
                RequireUpdate();
                return true;
            }
            catch (ReadApiDeniedException)
            {
                throw;
            }
            catch (WebException ex)
            {
                // Check for HTTP 401 error.                
                var resp = (HttpWebResponse)ex.Response;
                if (resp == null) throw;
                switch (resp.StatusCode)
                {
                    case HttpStatusCode.Unauthorized /*401*/:
                        throw;
                }
                return false;
            }
            catch (Exception)
            {                
                Editor = CreateEditor("https://en.wikipedia.org/w/");
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void RequireUpdate()
        {
            status = WikiStatusResult.PendingUpdate;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public WikiStatusResult Update()
        {
            Status = UpdateWikiStatus();
            return Status;
        }

        /// <summary>
        /// 
        /// </summary>
        public static string AWBVersion
        { get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); } }

        private static readonly Regex DirectionMarks = new Regex(@"^(\u200E|\u200F)+", RegexOptions.Multiline);

        /// <summary>
        /// Checks log in status, registered and version.
        /// </summary>
        private WikiStatusResult UpdateWikiStatus()
        {
            try
            {
                IsBot = false;

                Site = new SiteInfo(Editor.SynchronousEditor);

                // load version check page if no status set
                if (Updater.Result == Updater.AWBEnabledStatus.None)
                {
                    Updater.CheckForUpdates();
                }

                Variables.RTL = Site.IsRightToLeft;
                Variables.CapitalizeFirstLetter = Site.CapitalizeFirstLetter;

                Variables.UnicodeCategoryCollation = !Variables.IsCustomProject && Regex.IsMatch(Site.CategoryCollation, "[a-z-]*uca-");

                if (Variables.IsCustomProject || Variables.IsWikia)
                {
                    Variables.LangCode = Site.Language;
                }

                Variables.TagEdits = Site.IsAWBTagDefined;

                Updater.WaitForCompletion();
                Updater.AWBEnabledStatus versionStatus = Updater.Result;
                VersionCheckPage = Updater.GlobalVersionPage;

                // see if this version is enabled
                if ((versionStatus & Updater.AWBEnabledStatus.Disabled) == Updater.AWBEnabledStatus.Disabled)
                {
                    return WikiStatusResult.OldVersion;
                }

                bool usingJSON = false;

                // Attempt to load the JSON CheckPage from the wiki
                string JSONCheckPageText = Editor.SynchronousEditor.HttpGet(Variables.URLIndex + "?title=Project:AutoWikiBrowser/CheckPageJSON&action=raw");

                // HttpGet returns "" for 404
                if (!string.IsNullOrEmpty(JSONCheckPageText))
                {
                    usingJSON = true;
                    CheckPageJSONText = JSONCheckPageText;
                }
                else
                {
                    // load non JSON check page
                    string nonJSONCheckPageText = Editor.SynchronousEditor.HttpGet(Variables.URLIndex + "?title=Project:AutoWikiBrowser/CheckPage&action=raw");

                    // TODO: Some error handling

                    // remove U+200E LEFT-TO-RIGHT MARK, U+200F RIGHT-TO-LEFT MARK as on RTL wikis these can get typed in by accident
                    nonJSONCheckPageText = DirectionMarks.Replace(nonJSONCheckPageText, "");

                    CheckPageText = nonJSONCheckPageText;
                }

                if (!User.IsLoggedIn)
                {
                    return WikiStatusResult.NotLoggedIn;
                }

                if (!User.HasRight("writeapi"))
                {
                    return WikiStatusResult.NoRights;
                }

                // TODO: assess the impact on servers later
                Editor.Maxlag = /*User.IsBot ? 5 : 20*/ -1;

                var versionJson = JObject.Parse(Updater.GlobalVersionPage);

                // check if username is globally blacklisted based on the enwiki version page
                foreach (string badName in versionJson["badnames"])
                {
                    if (!string.IsNullOrEmpty(User.Name) &&
                        Regex.IsMatch(User.Name, badName,
                                      RegexOptions.IgnoreCase | RegexOptions.Multiline))
                    {
                        return WikiStatusResult.NotRegistered;
                    }
                }

                // See if there's any global messages on the enwiki json version page
                JSONMessages(versionJson["messages"]);

                if (usingJSON)
                {
                    string configJSONText = Editor.SynchronousEditor.HttpGet(Variables.URLIndex + "?title=Project:AutoWikiBrowser/Config2&action=raw");

                    ConfigJSONText = !string.IsNullOrEmpty(configJSONText) ? configJSONText : DefaultWikiConfig;

                    var configJson = JObject.Parse(ConfigJSONText);

                    // See if there's any messages on the local wikis config page
                    JSONMessages(configJson["messages"]);

                    // don't update Variables.RetfPath if typolink is empty
                    if(!string.IsNullOrEmpty(configJson["typolink"].ToString()))
                    {
                        Variables.RetfPath = configJson["typolink"].ToString();
                        Tools.WriteDebug("UpdateWikiStatus", "RETF Path set from typolink as " + Variables.RetfPath);
                    }

                    List<string> us = new List<string>();
                    foreach (var underscore in configJson["underscoretitles"])
                    {
                        if (underscore.ToString().Trim().Length > 0)
                        {
                            us.Add(underscore.ToString().Trim());
                        }
                    }
                    if (us.Count > 0)
                    {
                        Variables.LoadUnderscores(us.ToArray());
                    }

                    // don't require approval if CheckPage does not exist
                    // Or it has the special config option...
                    if (CheckPageJSONText.Length < 1 || (bool)configJson["allusersenabled"])
                    {
                        IsBot = true;
                        return WikiStatusResult.Registered;
                    }

                    var checkPageJson = JObject.Parse(CheckPageJSONText);

                    var enabledUsers = checkPageJson["enabledusers"].Select(u => u.ToString()).ToList();
                    var enabledBots = checkPageJson["enabledbots"].Select(u => u.ToString()).ToList();

                    // CheckPage option: 'allusersenabledusermode' will enable all users for user mode,
                    // and enable bots only when in 'enabledbots' section

                    var usernameComparer = new UsernameComparer();
                    // Optimisation, saves running the check multiple times
                    bool isBotEnabled = enabledBots.Contains(User.Name, usernameComparer);

                    if (
                        (bool) configJson["allusersenabledusermode"] ||
                        (IsSysop && Variables.Project != ProjectEnum.wikia) ||
                        isBotEnabled ||
                        enabledUsers.Contains(User.Name, usernameComparer)
                    )
                    {
                        // enable bot mode if in bots section
                        IsBot = isBotEnabled;

                        return WikiStatusResult.Registered;
                    }
                }
                else
                {
                    // THIS IS ALL BASICALLY DEPRECATED...

                    // CheckPage Messages per wiki are still in scary HTML comments.... TBC!
                    // see if there is a message
                    foreach (Match m in Message.Matches(CheckPageText))
                    {
                        if (m.Groups[1].Value.Trim().Length == 0)
                        {
                            continue;
                        }
                        MessageBox.Show(m.Groups[1].Value.Trim(), "Automated message", MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                    }

                    // see if there is a version-specific message
                    foreach (Match m in VersionMessage.Matches(CheckPageText))
                    {
                        if (m.Groups[1].Value.Trim().Length == 0 ||
                            m.Groups[1].Value != AWBVersion)
                        {
                            continue;
                        }
                        MessageBox.Show(m.Groups[2].Value.Trim(), "Automated message", MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                    }

                    HasTypoLink(CheckPageText);

                    List<string> us = new List<string>();
                    foreach (Match underscore in Underscores.Matches(CheckPageText))
                    {
                        if (underscore.Success && underscore.Groups[1].Value.Trim().Length > 0)
                        {
                            us.Add(underscore.Groups[1].Value.Trim());
                        }
                    }
                    if (us.Count > 0)
                    {
                        Variables.LoadUnderscores(us.ToArray());
                    }

                    // don't require approval if checkpage does not exist
                    // Or it has the special text...
                    if (CheckPageText.Length < 1 || CheckPageText.Contains("<!--All users enabled-->"))
                    {
                        IsBot = true;
                        return WikiStatusResult.Registered;
                    }

                    // see if we are allowed to use this software
                    CheckPageText = Tools.StringBetween(CheckPageText, "<!--enabledusersbegins-->",
                                                        "<!--enabledusersends-->");

                    // Checkpage option: <!--All users enabled user mode--> will enable all users for user mode,
                    // and enable bots only when in <!--enabledbots--> section
                    if (CheckPageText.Contains("<!--All users enabled user mode-->")
                        || (IsSysop && Variables.Project != ProjectEnum.wikia)
                        || UserNameInText(User.Name, CheckPageText))
                    {
                        string botUsers = Tools.StringBetween(CheckPageText, "<!--enabledbots-->", "<!--enabledbotsends-->");

                        // enable bot mode if in bots section
                        IsBot = UserNameInText(User.Name, botUsers);

                        return WikiStatusResult.Registered;
                    }
                }

                if (Variables.Project != ProjectEnum.custom)
                {
                    var globalUsers = versionJson["globalusers"];
                    foreach (string s in globalUsers)
                    {
                        if (User.Name == s)
                        {
                            return WikiStatusResult.Registered;
                        }
                    }
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
        /// Gets a list of pages that shouldn't have genfixes run on them
        /// </summary>
        /// <returns>List of pages that shouldn't recieve genfixes</returns>
        public List<string> NoGenfixes()
        {
            // TODO: Bring this upto LoadUnderscores, don't reparse json
            return JObject.Parse(ConfigJSONText)["nogenfixes"]
                .Select(page => page.ToString())
                .Distinct()
                .ToList();
        }

        /// <summary>
        /// Gets a list of pages that shouldn't be processed for typofixing
        /// </summary>
        /// <returns>List of pages that shouldn't recieve typo fixing</returns>
        public List<string> NoRETF()
        {
            // TODO: Bring this upto LoadUnderscores, don't reparse json
            return JObject.Parse(ConfigJSONText)["noregextypofix"]
                .Select(page => page.ToString())
                .Distinct()
                .ToList();
        }

        /// <summary>
        /// Checks text for a username
        /// User ID must be on its own line started with an asterisk (*), any whitespace around it
        /// Matching is first letter case insensitive, and treats underscores and spaces as the same
        /// </summary>
        /// <param name="userText">User text to look for</param>
        /// <param name="text">Text to look for the userText in</param>
        /// <returns>Whether the userText is found within the given text</returns>
        public static bool UserNameInText(string userText, string text)
        {
            return new Regex(
                @"^\*\s*" +
                Tools.FirstLetterCaseInsensitive(
                    Regex.Escape(
                        userText.Replace("_", " ")
                    ).Replace(@"\ ", @"[ _]")
                )
                + @"\s*$",
                RegexOptions.Multiline
            ).IsMatch(text);
        }

        /// <summary>
        /// Extracts the typo link URL from the &lt;!--Typos--&gt; check page comment and updates Variables.RetfPath if found
        /// </summary>
        /// <returns><c>true</c> if has typo link the specified text; otherwise, <c>false</c>.</returns>
        /// <param name="text">Text.</param>
        public static void HasTypoLink(string text)
        {
            Match typoLink = Regex.Match(text, "<!--[Tt]ypos:(.*?)-->");
            if (typoLink.Success && typoLink.Groups[1].Value.Trim().Length > 0)
            {
                Variables.RetfPath = typoLink.Groups[1].Value.Trim();
            }
        }

        private static void JSONMessages(JToken json)
        {
            foreach (var message in json)
            {
                var version = message["version"].ToString();
                // TODO: Semver version checking
                if ((version == "*" || version == AWBVersion) && message["text"] != null)
                {
                    if (message["enabled"] != null && !(bool)message["enabled"])
                    {
                        continue;
                    }
                    // TODO: Stop this depending on MessageBox.Show() add an event/delegate and handle in Main.cs
                    MessageBox.Show(message["text"].ToString().Trim(), "Automated message", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                }
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

                for (int i = 0; i < months.Length; i++)
                {
                    months[i] += "-gen";
                }

                // get localized month names if not en-wiki
                if (!Variables.IsWikipediaEN)
                {
                    Dictionary<string, string> messages = Site.GetMessages(months);

                    if (messages.Count == 12)
                    {
                        for (int i = 0; i < months.Length; i++)
                        {
                            months[i] = messages[months[i]];
                        }
                        Variables.MonthNames = months;
                    }
                }

                Variables.Namespaces = Site.Namespaces;
                Variables.NamespaceAliases = Site.NamespaceAliases;
                Variables.MagicWords = Site.MagicWords;
            }
            catch (ReadApiDeniedException)
            {
                throw;
            }
            catch (WebException ex)
            {
                string message = "";
                
                if (ex.InnerException != null)
                {
                    if (ex.InnerException is AuthenticationException)
                    {

                        if (message.Equals(""))
                        {
                            message = ex.Message;
                        }
                        else
                        {
                            message += " " + ex.Message;
                        }
                    }

                    if (message.Equals(""))
                    {
                        message = ex.InnerException.Message;
                    }
                    else
                    {
                        message += " " + ex.InnerException.Message;
                    }
                }
                else
                {
                    var resp = (HttpWebResponse)ex.Response;
                    if (resp == null) throw;

                    // Check for HTTP 401 error.
                    switch (resp.StatusCode)
                    {
                        case HttpStatusCode.Unauthorized: // 401
                            throw;
                    }
                    
                    message = ex.Message;
                }

                MessageBox.Show(message, "Error connecting to wiki", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                throw;
            }
            catch (Exception ex)
            {
                // TODO:Better error handling

                string message = "";

                if (ex is WikiUrlException)
                {
                    if (ex.InnerException != null)
                    {
                        message = ex.InnerException.Message;
                    }
                }
                else
                {
                    message = ex.Message;
                }

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
