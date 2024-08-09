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
        /// Gets the check page JSON Text.
        /// </summary>
        /// <value>The check page JSON Text.</value>
        public string CheckPageJSONText
        { get; private set; }

        /// <summary>
        /// Config Page JSON text
        /// </summary>
        public string ConfigJSONText { get; set; }

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
            catch (UriChangedException ex)
            {
                // TODO: We should offer to try changing the protocol to the response Uri scheme and attempt to load again
                MessageBox.Show(
                    ex.Message,
                    ex.Header,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return false;
            }
            catch (WebException ex)
            {
                // Check for HTTP 401 error.                
                var resp = (HttpWebResponse)ex.Response;
                if (resp == null) throw;
                switch (resp.StatusCode)
                {
                    // 401
                    case HttpStatusCode.Unauthorized:
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

        public static string ConfigUrl
        {
            get { return Variables.URLIndex + "?title=Project:AutoWikiBrowser/Config&action=raw"; }
        }

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

                // Attempt to load the JSON CheckPage from the wiki
                CheckPageJSONText = Editor.SynchronousEditor.HttpGet(Variables.URLIndex + "?title=Project:AutoWikiBrowser/CheckPageJSON&action=raw");

                if (!User.IsLoggedIn)
                {
                    return WikiStatusResult.NotLoggedIn;
                }

                // TODO: T294397 removed writeapi userright. T202192 to add some version checking (because of MW LTS at least)
                //if (!User.HasRight("writeapi"))
                //{
                //    return WikiStatusResult.NoRights;
                //}

                // TODO: assess the impact on servers later
                Editor.Maxlag = /*User.IsBot ? 5 : 20*/ -1;

                var versionJson = JObject.Parse(Updater.GlobalVersionPage);

                // check if username is listed globally as a badname, based on the enwiki version page
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

                string configJSONText = Editor.SynchronousEditor.HttpGet(ConfigUrl);

                bool usingDefaultJSONText = false;
                if (!string.IsNullOrEmpty(configJSONText))
                {
                    ConfigJSONText = configJSONText;
                }
                else
                {
                    Tools.WriteDebug("UpdateWikiStatus",
                        "No JSON config page at " + ConfigUrl + "; falling back to default");
                    ConfigJSONText = DefaultWikiConfig;
                    usingDefaultJSONText = true;
                }

                var configJson = JObject.Parse(ConfigJSONText);

                // See if there's any messages on the local wikis config page
                JSONMessages(configJson["messages"]);

                TypoLink(configJson);

                Variables.LoadUnderscores(
                    configJson["underscoretitles"]
                        .Select(underscore => underscore.ToString().Trim())
                        .Where(trimmed => !string.IsNullOrEmpty(trimmed))
                        .ToArray()
                );

                NoGenfixes = configJson["nogenfixes"].DistinctList();

                NoRETF = configJson["noregextypofix"].DistinctList();

                // don't require approval if CheckPage does not exist
                // Or it has the special config option...
                if (CheckPageJSONText.Length < 1 || (bool) configJson["allusersenabled"])
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

                if (Variables.Project != ProjectEnum.custom)
                {
                    foreach (string s in versionJson["globalusers"])
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

        public static void TypoLink(JObject configJson)
        {
            // don't update Variables.RetfPath if typolink is empty
            var typoLink = configJson["typolink"].ToString();
            if (!string.IsNullOrEmpty(typoLink))
            {
                Variables.RetfPath = typoLink;
                Tools.WriteDebug("UpdateWikiStatus", "RETF Path set from typolink as " + Variables.RetfPath);
            }
        }

        /// <summary>
        /// Gets a list of pages that shouldn't have genfixes run on them
        /// </summary>
        /// <returns>List of pages that shouldn't receive genfixes</returns>
        public List<string> NoGenfixes { get; private set; }

        /// <summary>
        /// Gets a list of pages that shouldn't be processed for typofixing
        /// </summary>
        /// <returns>List of pages that shouldn't receive typo fixing</returns>
        public List<string> NoRETF { get; private set; }

        private static void JSONMessages(JToken json)
        {
            foreach (var message in json)
            {
                var version = message["version"];

                // One message could apply to many versions
                if (version is JArray)
                {
                    foreach (var v in version)
                    {
                        JSONMessage(v.ToString(), message);
                    }
                }
                else
                {
                    JSONMessage(version.ToString(), message);
                }
            }
        }

        private static void JSONMessage(string versionString, JToken message)
        {
            // TODO: Proper semver version checking
            if ((versionString != "*" && versionString != AWBVersion) || message["text"] == null)
            {
                return;
            }

            if (message["enabled"] != null && !(bool) message["enabled"])
            {
                return;
            }

            // TODO: Stop this depending on MessageBox.Show() add an event/delegate and handle in Main.cs
            MessageBox.Show(message["text"].ToString().Trim(), "Automated message", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
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
                    var resp = (HttpWebResponse) ex.Response;
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

                MessageBox.Show(
                    "An error occured while connecting to the server or loading project information from it. " +
                    "Please make sure that your internet connection works and such combination of project/language exist." +
                    "\r\nEnter the URL in the format \"en.wikipedia.org/w/\" (including path where index.php and api.php reside)." +
                    "\r\nError description: " + message,
                    "Error connecting to wiki", MessageBoxButtons.OK, MessageBoxIcon.Error);

                throw;
            }
        }
    }
}
