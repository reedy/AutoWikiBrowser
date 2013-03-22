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
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WikiFunctions.Background;
using WikiFunctions.API;

namespace WikiFunctions
{
    /// <summary>
    /// This class controls editing process in one wiki
    /// </summary>
    public class Session
    {
        public AsyncApiEdit Editor
        { get; private set; }

        public SiteInfo Site
        { get; private set; }

        public bool IsBot
        { get; private set; }

        public bool IsSysop
        {
            get { return Editor.User.IsSysop; }
        }

        internal string CheckPageText
        { get; private set; }

        public Session(string url)
        {
            Editor = new AsyncApiEdit(url);
            LoadProjectOptions();
            Update();
        }

        private readonly static Regex Message = new Regex("<!--[Mm]essage:(.*?)-->", RegexOptions.Compiled);
        private readonly static Regex VersionMessage = new Regex("<!--VersionMessage:(.*?)\\|\\|\\|\\|(.*?)-->", RegexOptions.Compiled);
        private readonly static Regex Underscores = new Regex("<!--[Uu]nderscores:(.*?)-->", RegexOptions.Compiled);

        private WikiStatusResult Status;

        public WikiStatusResult Update()
        {
            Status = UpdateWikiStatus();
            return Status;
        }

        private static string AWBVersion
        { get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); } }

        /// <summary>
        /// Checks log in status, registered and version.
        /// </summary>
        private WikiStatusResult UpdateWikiStatus()
        {
            try
            {
                string typoPostfix = "";

                IsBot = false;

                //TODO: login?
                Site = new SiteInfo(Editor);

                //load version check page
                BackgroundRequest versionRequest = new BackgroundRequest();
                versionRequest.GetHTML(
                    "http://en.wikipedia.org/w/index.php?title=Wikipedia:AutoWikiBrowser/CheckPage/Version&action=raw");

                //load check page
                string url;
                if (Variables.IsWikia)
                    url = "http://www.wikia.com/wiki/index.php?title=Wikia:AutoWikiBrowser/CheckPage&action=edit";
                else if ((Variables.Project == ProjectEnum.wikipedia) && (Variables.LangCode == LangCodeEnum.ar))
                    url = "http://ar.wikipedia.org/w/index.php?title=%D9%88%D9%8A%D9%83%D9%8A%D8%A8%D9%8A%D8%AF%D9%8A%D8%A7:%D8%A7%D9%84%D8%A3%D9%88%D8%AA%D9%88%D9%88%D9%8A%D9%83%D9%8A_%D8%A8%D8%B1%D8%A7%D9%88%D8%B2%D8%B1/%D9%85%D8%B3%D9%85%D9%88%D8%AD&action=edit";
                else
                    url = Variables.URLIndex + "?title=Project:AutoWikiBrowser/CheckPage&action=edit";

                string strText = Editor.SynchronousEditor.HttpGet(url);

                Variables.RTL = Site.IsRightToLeft;

                if (Variables.IsCustomProject || Variables.IsWikia)
                {
                    try
                    {
                        Variables.LangCode =
                            Variables.ParseLanguage(Site.Language);
                    }
                    catch
                    {
                        // use English if language not recognized
                        Variables.LangCode = LangCodeEnum.en;
                    }
                }

                if (Variables.IsWikia)
                {
                    //this object loads a local checkpage on Wikia
                    //it cannot be used to approve users, but it could be used to set some settings
                    //such as underscores and pages to ignore
                    AsyncApiEdit editWikia = (AsyncApiEdit)Editor.Clone();
                    SiteInfo wikiaInfo = new SiteInfo();
                    string s = editWikia.SynchronousEditor.Open("Project:AutoWikiBrowser/CheckPage");

                    typoPostfix = "-" + Variables.LangCode;

                    // selectively add content of the local checkpage to the global one
                    strText += Message.Match(s).Value
                        /*+ Underscores.Match(s).Value*/
                               + WikiRegexes.NoGeneralFixes.Match(s);

                }

                versionRequest.Wait();
                string strVersionPage = (string)versionRequest.Result;

                //see if this version is enabled
                if (!strVersionPage.Contains(AWBVersion + " enabled"))
                    return WikiStatusResult.OldVersion;

                //TODO:
                // else
                //if (!WeAskedAboutUpdate && strVersionPage.Contains(AWBVersion + " enabled (old)"))
                //{
                //    WeAskedAboutUpdate = true;
                //    if (
                //        MessageBox.Show(
                //            "This version has been superceeded by a new version.  You may continue to use this version or update to the newest version.\r\n\r\nWould you like to automatically upgrade to the newest version?",
                //            "Upgrade?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                //    {
                //        Match version = Regex.Match(strVersionPage, @"<!-- Current version: (.*?) -->");
                //        if (version.Success && version.Groups[1].Value.Length == 4)
                //        {
                //            System.Diagnostics.Process.Start(Path.GetDirectoryName(Application.ExecutablePath) +
                //                                             "\\AWBUpdater.exe");
                //        }
                //        else if (
                //            MessageBox.Show("Error automatically updating AWB. Load the download page instead?",
                //                            "Load download page?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                //        {
                //            Tools.OpenURLInBrowser("http://sourceforge.net/project/showfiles.php?group_id=158332");
                //        }
                //    }
                //}

                CheckPageText = strText;

                // don't run GetInLogInStatus if we don't have the username, we sometimes get 2 error message boxes otherwise
                bool loggedIn = Editor.User.IsRegistered;

                if (!loggedIn)
                    return WikiStatusResult.NotLoggedIn;

                // check if username is globally blacklisted
                foreach (
                    Match m3 in Regex.Matches(strVersionPage, @"badname:\s*(.*)\s*(:?|#.*)$", RegexOptions.IgnoreCase))
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

                //see if we are allowed to use this softare
                strText = Tools.StringBetween(strText, "<!--enabledusersbegins-->", "<!--enabledusersends-->");

                string strBotUsers = Tools.StringBetween(strText, "<!--enabledbots-->", "<!--enabledbotsends-->");
                Regex username = new Regex(@"^\*\s*" + Tools.CaseInsensitive(Editor.User.Name)
                                           + @"\s*$", RegexOptions.Multiline);

                if (IsSysop)
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

                string message = ex is WikiUrlException ? ex.InnerException.Message : ex.Message;
                MessageBox.Show("An error occured while connecting to the server or loading project information from it. " +
                        "Please make sure that your internet connection works and such combination of project/language exist." +
                        "\r\nEnter the URL in the format \"en.wikipedia.org/w/\" (including path where index.php and api.php reside)." +
                        "\r\nError description: " + message,
                        "Error connecting to wiki", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //SetDefaults();
                
                return;
            }
        }
    }
}
