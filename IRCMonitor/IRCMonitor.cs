/*
Copyright (C) 2007 Martin Richards

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
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WikiFunctions.IRC;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml;
using WikiFunctions;
using WikiFunctions.Lists;
using System.Web;
using System.Net;
using WikiFunctions.Browser;
using System.Reflection;

namespace IRCM
{
    /// <summary>
    /// This form demonstates usage of the WikiIRC framework.
    /// </summary>
    public partial class IRCMonitor : Form
    {
        #region Main events and constructor
        /// <summary>
        /// This form demonstates usage of the WikiIRC framework.
        /// </summary>
        public IRCMonitor()
        {
            InitializeComponent();
            LoadIrcChannels();
            cmboEditIP.SelectedIndex = 0;
            cmboEditMinor.SelectedIndex = 0;
            cmboEditNamespace.SelectedIndex = 0;
            cmboNewStuffNamespace.SelectedIndex = 0;
            cmboPageMoveNamespace.SelectedIndex = 0;

            btnBack.Image = WikiFunctions.Properties.Resources.LeftArrow;
            btnFoward.Image = WikiFunctions.Properties.Resources.RightArrow;
            btnStop.Image = WikiFunctions.Properties.Resources.Stop;
            btnOpenInBrowser.Image = WikiFunctions.Properties.Resources.NewWindow;

            Variables.SetProject(LangCodeEnum.en, ProjectEnum.wikipedia);

            btnIPColour.BackColor = ColourIP;
            btnRegisteredUserColour.BackColor = UserColour;
            btnSetWhiteListColour.BackColor = whitelistcolour;
            btnSetBlackListColour.BackColor = BlackListColour;
            btnSetWatchedColour.BackColor = WatchListColour;
            btnSetCheckedColour.BackColor = CheckedColour;

            foreach (LangCodeEnum l in Enum.GetValues(typeof(LangCodeEnum)))
                cmboLang.Items.Add(l.ToString().ToLower());

            foreach (ProjectEnum l in Enum.GetValues(typeof(ProjectEnum)))
                cmboProject.Items.Add(l);

            webBrowser.Saved += new WikiFunctions.Browser.WebControlDel(WebBrowserSaved);

            Variables.User.UserNameChanged += UpdateUserName;
            Variables.User.BotStatusChanged += UpdateBotStatus;
            Variables.User.AdminStatusChanged += UpdateAdminStatus;
            Variables.User.WikiStatusChanged += UpdateWikiStatus;
        }

        private ProjectSettings Project = new ENWikipediaSettings();

        public ProjectSettings Project1
        {
            get
            {
                return Project;
            }
            set
            {
                Project = value;
            }
        }

        private string VandalName;

        public string VandalName1
        {
            get
            {
                return VandalName;
            }
            set
            {
                VandalName = value;
            }
        }
        private string VandalizedPage;

        public string VandalizedPage1
        {
            get
            {
                return VandalizedPage;
            }
            set
            {
                VandalizedPage = value;
            }
        }
        public string ReplacementText;
        public enum NextTaskType { None, Warn, Report, Contribs, Blacklist };
        NextTaskType NextTask = NextTaskType.None;
        //string[] IRCChannels = new String[] { "#en.wikibooks", "#en.wikinews ", "#en.wikipedia", "#en.wikiquote", "#en.wikisource", "#meta" };

        void WebBrowserSaved(object sender, EventArgs e)
        {
            webBrowser.AllowNavigation = true;
            switch (NextTask)
            {
                case NextTaskType.Warn:
                    NextTask = NextTaskType.None;
                    webBrowser.Navigate(Variables.URL + "/wiki/User talk:" + HttpUtility.UrlEncode(VandalName));
                    break;
                case NextTaskType.Report:
                    NextTask = NextTaskType.None;
                    ReportVandal(VandalName);
                    break;
                case NextTaskType.Blacklist:
                    AddToBlacklist(VandalName);
                    break;
                default:
                    NextTask = NextTaskType.None;
                    break;
            }
        }

        private void ReportVandal(string username)
        {
            webBrowser.Navigate(Project.ReportUrl);
            webBrowser.Wait();
            NextTask = NextTaskType.None;
            if (webBrowser.GetArticleText().Contains(username + "}}"))
            {
                MessageBox.Show("This user is already reported", "AIV", MessageBoxButtons.OK, MessageBoxIcon.Information);
                webBrowser.GoBack();
                return;
            }
            webBrowser.SetArticleText(webBrowser.GetArticleText() + "\r\n*{{" + (Tools.IsIP(username) ? Project.ReportAnonTemplate : Project.ReportRegisteredTemplate) + "|" + username + "}} ~~~~");
            webBrowser.SetSummary(Project.ReportSummary.Replace("%v", username) + Project.Using);
            //webBrowser.Save();
        }

        public static void MakeMenu(string[] templates, ToolStripItemCollection items, EventHandler handler)
        {
            Stack<ToolStripItemCollection> st = new Stack<ToolStripItemCollection>();
            st.Push(items);
            foreach (string s in templates)
            {
                string title = s.TrimStart('*');
                int level = s.Length - title.Length;
                int tag;
                if (string.IsNullOrEmpty(title)) continue;
                if (int.TryParse(Tools.StringBetween(title, "[", "]"), out tag)) tag = -1;
                if (title[0] != '{' && title.Contains("{")) title = title.Remove(0, title.IndexOf('{'));
                ToolStripMenuItem ts = new ToolStripMenuItem(title);
                ts.Tag = tag;

                if (level > st.Count - 1)
                {
                    ToolStripMenuItem parent = (ToolStripMenuItem)st.Peek()[st.Peek().Count - 1];
                    st.Push(parent.DropDownItems);
                }
                else if (level < st.Count - 1)
                {
                    for (; level < st.Count - 1; level++) st.Pop();
                }
                if (title.StartsWith("{{")) ts.Click += handler;
                st.Peek().Add(ts);
            }
        }

        private void IRCMonitor_Load(object sender, EventArgs e)
        {
            ResetStats();
            LoadDefaultSettings();

            Updater.Update();

            btnWarn.DropDownItems.Clear();
            MakeMenu(Project.WarningTemplates, btnWarn.DropDownItems, new EventHandler(WarnUserClick));
            //MakeMenu(Project.StubTypes, addStubToolStripMenuItem.DropDownItems, new EventHandler(AddStubClick));
            MakeMenu(Project.PageTags, tagWithToolStripMenuItem.DropDownItems, new EventHandler(AddTagClick));
        }

        private void LoadIrcChannels()
        {
            cmboLang.Text = "en";
            cmboProject.Text = "wikipedia";
        }

        private bool CheckStatus(bool login)
        {
            StatusLabelText = "Loading page to check if we are logged in.";
            WikiStatusResult result = Variables.User.UpdateWikiStatus();

            bool b = false;
            string label = "Software disabled";

            switch (result)
            {
                case WikiStatusResult.Error:
                    lblUserName.BackColor = Color.Red;
                    MessageBox.Show("Check page failed to load.\r\n\r\nCheck your Internet Explorer is working and that the Wikipedia servers are online, also try clearing Internet Explorer cache.", "Problem", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;

                case WikiStatusResult.NotLoggedIn:
                    lblUserName.BackColor = Color.Red;
                    if (!login)
                        MessageBox.Show("You are not logged in. The log in screen will now load, enter your name and password, click \"Log in\", wait for it to complete, then start the process again.\r\n\r\nIn the future you can make sure this won't happen by logging in to Wikipedia using Microsoft Internet Explorer.", "Not logged in", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    webBrowser.LoadLogInPage();
                    tabControl.SelectedTab = BrowserTab;
                    break;

                case WikiStatusResult.NotRegistered:
                    lblUserName.BackColor = Color.Red;
                    MessageBox.Show(Variables.User.Name + " is not enabled to use this.", "Problem", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    Tools.OpenArticleInBrowser("/wiki/Project:AutoWikiBrowser/CheckPage");
                    break;

                case WikiStatusResult.OldVersion:
                    OldVersion();
                    break;

                case WikiStatusResult.Registered:
                    b = true;
                    label = string.Format("Logged in, user and software enabled. Bot = {0}, Admin = {1}", Variables.User.IsBot, Variables.User.IsAdmin);
                    lblUserName.BackColor = Color.LightGreen;

                    break;
            }

            txtNickname.Text = Variables.User.Name + "-IRCM";
            lblUserName.Text = Variables.User.Name;
            StatusLabelText = label;

            return b;
        }

        private void OldVersion()
        {
            if (!WebControl.Shutdown)
            {
                lblUserName.BackColor = Color.Red;

                DialogResult yesnocancel = MessageBox.Show("This version is not enabled, please download the newest version. If you have the newest version, check that Wikipedia is online.\r\n\r\nPlease press \"Yes\" to run the AutoUpdater, \"No\" to load the download page and update manually, or \"Cancel\" to not update (but you will not be able to edit).", "Problem", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                if (yesnocancel == DialogResult.Yes)
                    RunUpdater();

                if (yesnocancel == DialogResult.No)
                {
Tools.OpenURLInBrowser("http://sourceforge.net/project/showfiles.php?group_id=158332");
                }
            }
        }

        private void UpdateWikiStatus(object sender, EventArgs e)
        { }

        private void UpdateAdminStatus(object sender, EventArgs e)
        { }

        private void UpdateBotStatus(object sender, EventArgs e)
        { }

        private void UpdateUserName(object sender, EventArgs e)
        { }

        private static void RunUpdater()
        {
            System.Diagnostics.Process.Start(Path.GetDirectoryName(Application.ExecutablePath) + "\\AWBUpdater.exe");

            if (MessageBox.Show("IRCM needs to be closed. To do this now, click 'yes'. If you need to save your settings, do this now, the updater will not complete until AWB is closed.", "Close AWB?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
                Application.Exit();
        }

        private void UpdateButtons()
        {
            btnRevert.Enabled = webBrowser.Url.ToString().Contains("&diff=");
            btnWarn.Enabled = Variables.User.WikiStatus && webBrowser.IsUserSpace;
            btnUser.Enabled = Variables.User.WikiStatus && webBrowser.IsUserSpace;
            btnPage.Enabled = Variables.User.WikiStatus;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (btnStart.Text == "Connect")
            {
                if (OkToConnect())
                {
                    btnStart.Text = "Disconnect";
                    CheckStatus(false);
                    Start();
                }
                else
                    MessageBox.Show("The server, port, nickname, language and project cannot be blank. Please check and try again");
            }
            else
            {
                btnStart.Text = "Connect";
                WikiIRC.Run = false;
            }
        }

        private bool OkToConnect()
        {
            if (!string.IsNullOrEmpty(txtServer.Text) && !string.IsNullOrEmpty(txtPort.Text) && !string.IsNullOrEmpty(txtNickname.Text))
                return ((cmboProject.Text == "meta" || cmboProject.Text == "commons") || (!string.IsNullOrEmpty(cmboProject.Text) && !string.IsNullOrEmpty(cmboLang.Text)));
            else
                return false;
        }

        private string GetIrcChannel()
        {
            if (cmboProject.Text == "meta" || cmboProject.Text == "commons")
                return "#" + cmboProject.SelectedItem.ToString() + ".wikimedia";
            else
                return "#" + cmboLang.SelectedItem.ToString() + "." + cmboProject.SelectedItem.ToString();
        }

        WikiIRC IrcObject;

        private void Start()
        {
            Stop();

            StatusLabelText = "Connecting";
            Random n = new Random();

            string name = "ircM";
            if (string.IsNullOrEmpty(txtNickname.Text))
                name += n.Next(1000, 100000).ToString();
            else
                name += txtNickname.Text;

            IrcObject = new WikiIRC(txtServer.Text, int.Parse(txtPort.Text), name, GetIrcChannel());
            WikiIRC.Run = true;

            IrcObject.OtherMessages += ProcessOtherMessages;
            IrcObject.ConnectEvent += Connected;
            IrcObject.DisconnectEvent += Disconnected;
            IrcObject.Edit += ProcessEdit;
            IrcObject.NewArticle += ProcessNewArticles;
            IrcObject.NewUser += ProcessNewUser;
            IrcObject.PageMove += ProcessMove;
            IrcObject.Upload += ProcessUpload;
            IrcObject.Delete += ProcessDelete;
            IrcObject.Restore += ProcessRestore;
            IrcObject.Protect += ProcessProtection;
            IrcObject.Unprotect += ProcessUnprotection;
            IrcObject.Block += ProcessBlock;
            IrcObject.Unblock += ProcessUNBlock;

            IrcObject.Start();
        }

        private void Stop()
        {
            if (IrcObject != null)
            {
                WikiIRC.Run = false;
                IrcObject.ConnectEvent -= Connected;
                IrcObject.DisconnectEvent -= Disconnected;
                IrcObject.OtherMessages -= ProcessOtherMessages;
                IrcObject.Edit -= ProcessEdit;
                IrcObject.NewArticle -= ProcessNewArticles;
                IrcObject.NewUser -= ProcessNewUser;
                IrcObject.PageMove -= ProcessMove;
                IrcObject.Upload -= ProcessUpload;
                IrcObject.Delete -= ProcessDelete;
                IrcObject.Restore -= ProcessRestore;
                IrcObject.Protect -= ProcessProtection;
                IrcObject.Unprotect -= ProcessUnprotection;
                IrcObject.Block -= ProcessBlock;
                IrcObject.Unblock -= ProcessUNBlock;

                IrcObject = null;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.SelectAll();
            textBox1.Copy();
        }

        private void btnIPColour_Click(object sender, EventArgs e)
        {
            colorDialog.Color = ColourIP;
            if (colorDialog.ShowDialog() == DialogResult.OK)
                ColourIP = colorDialog.Color;
        }

        private void btnRegisteredUserColour_Click(object sender, EventArgs e)
        {
            colorDialog.Color = UserColour;
            if (colorDialog.ShowDialog() == DialogResult.OK)
                UserColour = colorDialog.Color;
        }

        private void btnSetWhiteListColour_Click(object sender, EventArgs e)
        {
            colorDialog.Color = WhiteListColour;
            if (colorDialog.ShowDialog() == DialogResult.OK)
                WhiteListColour = colorDialog.Color;
        }

        private void btnSetBlackListColour_Click(object sender, EventArgs e)
        {
            colorDialog.Color = BlackListColour;
            if (colorDialog.ShowDialog() == DialogResult.OK)
                BlackListColour = colorDialog.Color;
        }

        private void btnSetWatchedColour_Click(object sender, EventArgs e)
        {
            colorDialog.Color = WatchListColour;
            if (colorDialog.ShowDialog() == DialogResult.OK)
                WatchListColour = colorDialog.Color;
        }

        private void btnSetCheckedColour_Click(object sender, EventArgs e)
        {
            colorDialog.Color = CheckedColour;
            if (colorDialog.ShowDialog() == DialogResult.OK)
                CheckedColour = colorDialog.Color;
        }

        private void chkChangeCheckedColour_CheckedChanged(object sender, EventArgs e)
        {
            btnSetCheckedColour.Enabled = chkChangeCheckedColour.Checked;
        }

        private void IRCMonitor_FormClosing(object sender, FormClosingEventArgs e)
        {
            Stop();
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            if (IrcObject.Pause)
                btnPause.Text = "Pause";
            else
                btnPause.Text = "Resume";
            IrcObject.Pause = !IrcObject.Pause;
        }

        private void chkEditTitleRegex_CheckedChanged(object sender, EventArgs e)
        {
            txtEditTitleRegex.Enabled = chkEditTitleRegex.Checked;
        }

        private void chkBlackListSound_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBlackListSound.Checked)
                Tools.Beep2();
        }

        private void chkSoundOnWatchedChanged_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSoundOnWatchedChanged.Checked)
                Tools.Beep1();
        }

        private void Connected(object sender, EventArgs e)
        {
            StatusLabelText = "Connected";
        }

        private void Disconnected(object sender, EventArgs e)
        {
            StatusLabelText = "Disconnected";
            if (WikiIRC.Run)
            {
                Start();
                textBox1.AppendText("\r\n DISCONNECTED \r\n\r\n");
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            WikiIRC.Run = false;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutIrcMon about = new AboutIrcMon();
            about.Show();
        }

        #endregion

        #region Properties

        private Color ipcolour = Color.LightSkyBlue;
        public Color ColourIP
        {
            get { return ipcolour; }
            set { btnIPColour.BackColor = ipcolour = value; }
        }

        private Color usercolour = Color.LightGreen;
        public Color UserColour
        {
            get { return usercolour; }
            set { usercolour = btnRegisteredUserColour.BackColor = value; }
        }

        private Color whitelistcolour = Color.Wheat;
        public Color WhiteListColour
        {
            get { return whitelistcolour; }
            set { whitelistcolour = btnSetWhiteListColour.BackColor = value; }
        }

        private Color blacklistcolour = Color.Red;
        public Color BlackListColour
        {
            get { return blacklistcolour; }
            set { blacklistcolour = btnSetBlackListColour.BackColor = value; }
        }

        private Color watchlistcolour = Color.Plum;
        public Color WatchListColour
        {
            get { return watchlistcolour; }
            set { watchlistcolour = btnSetWatchedColour.BackColor = value; }
        }

        private Color checkedcolour = Color.Green;
        public Color CheckedColour
        {
            get { return checkedcolour; }
            set { checkedcolour = btnSetCheckedColour.BackColor = value; }
        }

        private string StatusLabelText
        {
            get { return lblStatusText.Text; }
            set { lblStatusText.Text = value; }
        }

        #endregion

        #region Message processing
        int MaxNumberOfRows = 500;

        ListViewItem lvItem;
        string[] lviEditArray = { "", "", "", "", "" };
        private void ProcessEdit(object sender, EventArgs e, string article, string minor, string difflink, string user, int plusminus, string comment)
        {
            bool iPedit = false;
            if (WikiRegexes.IPAddress.IsMatch(user))
                iPedit = true;

            bool blacklistedUser = lbBlackList.Items.Contains(user);
            bool watchedArticle = lbWatchList.Items.Contains(article);
            bool whiteListedUser = lbWhiteList.Items.Contains(user);

            if (!blacklistedUser && !watchedArticle)
            {//filter out if not black listed or on watchlist
                if (chkOnlyBlackAndWatched.Checked)
                    return;
                if (minor == "M" && cmboEditMinor.SelectedIndex == 2)
                    return;
                if (minor != "M" && cmboEditMinor.SelectedIndex == 1)
                    return;
                if (!CheckNameSpace(article, cmboEditNamespace.SelectedIndex))
                    return;
                if (chkIgnoreWhiteList.Checked && whiteListedUser)
                    return;
                if (iPedit && cmboEditIP.SelectedIndex == 2)
                    return;
                if (!iPedit && cmboEditIP.SelectedIndex == 1)
                    return;
                try
                {
                    if (chkEditTitleRegex.Checked && !Regex.IsMatch(article, txtEditTitleRegex.Text))
                        return;
                }
                catch { }
                if ((plusminus > (int)nudLessThan.Value) && (plusminus < (int)nudMoreThan.Value))
                    return;
            }

            //make the list item
            lviEditArray[0] = article;
            lviEditArray[1] = user;
            lviEditArray[2] = minor;
            lviEditArray[3] = plusminus.ToString();
            lviEditArray[4] = comment;
            lvItem = new ListViewItem(lviEditArray);
            lvItem.Tag = difflink;

            //set the colour
            if (watchedArticle)
            {
                lvItem.BackColor = WatchListColour;
                WatchedItemChanged();
            }
            else if (blacklistedUser)
            {
                lvItem.BackColor = BlackListColour;
                BlackListedEdited();
            }
            else if (whiteListedUser)
                lvItem.BackColor = WhiteListColour;
            else if (iPedit)
                lvItem.BackColor = ColourIP;
            else
                lvItem.BackColor = UserColour;

            //add it to the top of the list
            //listViewEdit.Items.Insert(0, lvItem);
            listViewEdit.Items.Insert(0, lvItem);
            if (listViewEdit.Items.Count > MaxNumberOfRows)
                listViewEdit.Items.RemoveAt(MaxNumberOfRows);
            AddEdit();
        }

        string[] lviNewUserArray = { "" };
        private void ProcessNewUser(object sender, EventArgs e, string name)
        {
            lvItem = new ListViewItem(name);

            listviewNewUsers.Items.Insert(0, lvItem);
            if (listviewNewUsers.Items.Count > MaxNumberOfRows)
                listviewNewUsers.Items.RemoveAt(MaxNumberOfRows);
            AddNewUser();
        }

        string[] lviNewArticleArray = { "", "", "", "" };
        private void ProcessNewArticles(object sender, EventArgs e, string article, string user, int plusmin, string comment)
        {
            bool iPedit = false;
            if (WikiRegexes.IPAddress.IsMatch(user))
                iPedit = true;

            bool whiteListedUser = lbWhiteList.Items.Contains(user);
            bool blacklistedUser = lbBlackList.Items.Contains(user);
            bool watchedArticle = lbWatchList.Items.Contains(article);

            if (!blacklistedUser && !watchedArticle)
            {//filter out if not black listed or on watchlist
                if (chkIgnoreWhiteList.Checked && whiteListedUser)
                    return;
                if (chkOnlyBlackAndWatched.Checked)
                    return;
                if (!CheckNameSpace(article, cmboNewStuffNamespace.SelectedIndex))
                    return;
            }

            lviNewArticleArray[0] = article;
            lviNewArticleArray[1] = user;
            lviNewArticleArray[2] = plusmin.ToString();
            lviNewArticleArray[3] = comment;
            lvItem = new ListViewItem(lviNewArticleArray);

            //set the colour
            if (watchedArticle)
            {
                lvItem.BackColor = WatchListColour;
                WatchedItemChanged();
            }
            else if (blacklistedUser)
            {
                lvItem.BackColor = BlackListColour;
                BlackListedEdited();
            }
            else if (whiteListedUser)
                lvItem.BackColor = WhiteListColour;
            else if (iPedit)
                lvItem.BackColor = ColourIP;
            else
                lvItem.BackColor = UserColour;

            listviewNewStuff.Items.Insert(0, lvItem);
            if (listviewNewStuff.Items.Count > MaxNumberOfRows)
                listviewNewStuff.Items.RemoveAt(MaxNumberOfRows);
            AddNewArticle();
        }

        string[] lviUploadArray = { "", "", "", "" };
        private void ProcessUpload(object sender, EventArgs e, string file, string user, string comment)
        {
            bool whiteListedUser = lbWhiteList.Items.Contains(user);
            bool blacklistedUser = lbBlackList.Items.Contains(user);
            bool watchedArticle = lbWatchList.Items.Contains(file);

            if (!blacklistedUser && !watchedArticle)
            {//filter out if not black listed or on watchlist
                if (chkIgnoreWhiteList.Checked && whiteListedUser)
                    return;
                if (chkOnlyBlackAndWatched.Checked)
                    return;
            }

            lviUploadArray[0] = file;
            lviUploadArray[1] = user;
            lviUploadArray[2] = "";
            lviUploadArray[3] = comment;
            lvItem = new ListViewItem(lviUploadArray);

            //set the colour
            if (watchedArticle)
            {
                lvItem.BackColor = WatchListColour;
                WatchedItemChanged();
            }
            else if (blacklistedUser)
            {
                lvItem.BackColor = BlackListColour;
                BlackListedEdited();
            }
            else if (whiteListedUser)
                lvItem.BackColor = WhiteListColour;
            else
                lvItem.BackColor = UserColour;

            listviewNewStuff.Items.Insert(0, lvItem);
            if (listviewNewStuff.Items.Count > MaxNumberOfRows)
                listviewNewStuff.Items.RemoveAt(MaxNumberOfRows);
            AddNewUpload();
        }

        string[] lviMoveArray = { "", "", "", "" };
        private void ProcessMove(object sender, EventArgs e, string oldName, string newName, string user, string comment)
        {
            bool whiteListedUser = lbWhiteList.Items.Contains(user);
            bool blacklistedUser = lbBlackList.Items.Contains(user);
            bool watchedArticle = lbWatchList.Items.Contains(oldName);

            if (!blacklistedUser && !watchedArticle)
            {//filter out if not black listed or on watchlist
                if (chkIgnoreWhiteList.Checked && whiteListedUser)
                    return;
                if (chkOnlyBlackAndWatched.Checked)
                    return;
                if (!CheckNameSpace(oldName, cmboPageMoveNamespace.SelectedIndex))
                    return;
            }

            lviMoveArray[2] = user;
            lviMoveArray[0] = oldName;
            lviMoveArray[1] = newName;
            lviMoveArray[3] = comment;
            lvItem = new ListViewItem(lviMoveArray);

            if (watchedArticle)
            {
                lvItem.BackColor = WatchListColour;
                WatchedItemChanged();
            }
            else if (blacklistedUser)
            {
                lvItem.BackColor = blacklistcolour;
                BlackListedEdited();
            }
            else if (whiteListedUser)
                lvItem.BackColor = WhiteListColour;
            else
                lvItem.BackColor = UserColour;

            listviewPageMoves.Items.Insert(0, lvItem);
            if (listviewPageMoves.Items.Count > MaxNumberOfRows)
                listviewPageMoves.Items.RemoveAt(MaxNumberOfRows);
            AddNewPageMove();
        }

        private void ProcessDelete(object sender, EventArgs e, string admin, string article, string comment)
        {
            ProcessActions(admin, "DELETE", article, comment);
            AddNewDeletion();
        }

        private void ProcessRestore(object sender, EventArgs e, string admin, string article, string comment)
        {
            ProcessActions(admin, "RESTORE", article, comment);
        }

        private void ProcessBlock(object sender, EventArgs e, string admin, string user, string comment, string time)
        {
            ProcessActions(admin, "BLOCK", user, time + comment);
            AddNewBlock();
        }

        private void ProcessUNBlock(object sender, EventArgs e, string admin, string user, string comment)
        {
            ProcessActions(admin, "UNBLOCK", user, comment);
        }

        private void ProcessProtection(object sender, EventArgs e, string admin, string article, string comment)
        {
            ProcessActions(admin, "PROTECT", article, comment);
            AddNewProtection();
        }

        private void ProcessUnprotection(object sender, EventArgs e, string admin, string article, string comment)
        {
            ProcessActions(admin, "UNPROTECT", article, comment);
        }

        string[] lviActionArray = { "", "", "", "" };
        private void ProcessActions(string admin, string action, string article, string comment)
        {
            bool blacklistedUser = lbBlackList.Items.Contains(admin);
            bool watchedArticle = lbWatchList.Items.Contains(article);

            if (!blacklistedUser && !watchedArticle)
            {//filter out if not black listed or on watchlist
                if (chkOnlyBlackAndWatched.Checked)
                    return;
            }

            lviActionArray[0] = admin;
            lviActionArray[1] = action;
            lviActionArray[2] = article;
            lviActionArray[3] = IrcObject.removeSyntax(comment);
            lvItem = new ListViewItem(lviActionArray);

            if (watchedArticle)
            {
                lvItem.BackColor = WatchListColour;
                WatchedItemChanged();
            }
            else if (blacklistedUser)
            {
                lvItem.BackColor = BlackListColour;
                BlackListedEdited();
            }
            else if (lbWhiteList.Items.Contains(admin))
                lvItem.BackColor = WhiteListColour;
            else
                lvItem.BackColor = UserColour;

            listviewActions.Items.Insert(0, lvItem);
            if (listviewActions.Items.Count > MaxNumberOfRows)
                listviewActions.Items.RemoveAt(MaxNumberOfRows);
        }

        private void ProcessOtherMessages(object sender, EventArgs e, string msg)
        {
            textBox1.AppendText(msg + "\r\n\r\n");
        }

        #endregion

        #region helper functions
        private void WatchedItemChanged()
        {
            if (chkFlashWatchlisted.Checked && !this.ContainsFocus)
                Tools.FlashWindow(this);

            if (chkSoundOnWatchedChanged.Checked)
                Tools.Beep1();
        }

        private void BlackListedEdited()
        {
            if (chkFlashBlackListed.Checked)
                Tools.FlashWindow(this);

            if (chkBlackListSound.Checked)
                Tools.Beep2();
        }

        private static int IntFromMessage(string s)
        {
            string z = s;
            int i = 0;

            s = s.Trim('');
            s = s.TrimStart('-', '+');
            i = Int32.Parse(s);

            if (z.StartsWith("-"))
                i = i * -1;

            return i;
        }

        private static bool CheckNameSpace(string article, int index)
        {
            if (index == 1 && !Tools.IsMainSpace(article))
                return false;
            if (index == 2 && Tools.IsTalkPage(article))
                return false;

            return true;
        }

        private void OpenInBrowser(string url)
        {
            if (chkBrowser.Checked)
            {
                webBrowser.AllowNavigation = true;
                webBrowser.Navigate(url);
                tabControl.SelectedTab = BrowserTab;
            }
            else
            {
                Tools.OpenURLInBrowser(url);
            }
        }

        private void btnResetStats_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to reset the statistics back to 0?", "Sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                ResetStats();
        }

        private void ResetStats()
        {
            dataGridStatistics.Rows.Clear();

            dataGridStatistics.Rows.Add(8);
            dataGridStatistics.Rows[0].Cells[0].Value = "Edits";
            dataGridStatistics.Rows[1].Cells[0].Value = "New articles";
            dataGridStatistics.Rows[2].Cells[0].Value = "Uploads";
            dataGridStatistics.Rows[3].Cells[0].Value = "Page moves";
            dataGridStatistics.Rows[4].Cells[0].Value = "New users";
            dataGridStatistics.Rows[5].Cells[0].Value = "Blocks";
            dataGridStatistics.Rows[6].Cells[0].Value = "Deletions";
            dataGridStatistics.Rows[7].Cells[0].Value = "Protections";

            dataGridStatistics.Rows[0].Cells[1].Value = 0;
            dataGridStatistics.Rows[1].Cells[1].Value = 0;
            dataGridStatistics.Rows[2].Cells[1].Value = 0;
            dataGridStatistics.Rows[3].Cells[1].Value = 0;
            dataGridStatistics.Rows[4].Cells[1].Value = 0;
            dataGridStatistics.Rows[5].Cells[1].Value = 0;
            dataGridStatistics.Rows[6].Cells[1].Value = 0;
            dataGridStatistics.Rows[7].Cells[1].Value = 0;
        }

        int intNumberofEdits;
        private void AddEdit()
        {
            intNumberofEdits++;
            dataGridStatistics.Rows[0].Cells[1].Value = intNumberofEdits;
        }

        int intNumberofNewArticle;
        private void AddNewArticle()
        {
            intNumberofNewArticle++;
            dataGridStatistics.Rows[1].Cells[1].Value = intNumberofNewArticle;
        }

        int intNumberofUploads;
        private void AddNewUpload()
        {
            intNumberofUploads++;
            dataGridStatistics.Rows[2].Cells[1].Value = intNumberofUploads;
        }

        int intNumberofPageMoves;
        private void AddNewPageMove()
        {
            intNumberofPageMoves++;
            dataGridStatistics.Rows[3].Cells[1].Value = intNumberofPageMoves;
        }

        int intNumberofNewUsers;
        private void AddNewUser()
        {
            intNumberofNewUsers++;
            dataGridStatistics.Rows[4].Cells[1].Value = intNumberofNewUsers;
        }

        int intNumberofBlocks;
        private void AddNewBlock()
        {
            intNumberofBlocks++;
            dataGridStatistics.Rows[5].Cells[1].Value = intNumberofBlocks;
        }

        int intNumberofDeletions;
        private void AddNewDeletion()
        {
            intNumberofDeletions++;
            dataGridStatistics.Rows[6].Cells[1].Value = intNumberofDeletions;
        }

        int intNumberofProtection;
        private void AddNewProtection()
        {
            intNumberofProtection++;
            dataGridStatistics.Rows[7].Cells[1].Value = intNumberofProtection;
        }

        #endregion

        #region Lists

        public void AddToBlacklist(string username)
        {
            username = Tools.TurnFirstToUpper(username);
            if (lbBlackList.Items.Contains(username)) return;
            lbBlackList.Items.Add(username);
            lblBlackListCount.Text = lbBlackList.Items.Count.ToString();
        }

        private void btnWhiteListAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtList.Text))
                return;

            lbWhiteList.Items.Add(Tools.TurnFirstToUpper(txtList.Text));
            txtList.Clear();

            lblWhiteListCount.Text = lbWhiteList.Items.Count.ToString();
        }

        private void btnBlackListAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtList.Text))
                return;

            AddToBlacklist(txtList.Text);
            txtList.Clear();
        }

        private void btnWhiteListeAddBots_Click(object sender, EventArgs e)
        {
            WhitelistEnabled(false);

            try
            {
                List<Article> bots = new List<Article>();
                string item = "";
                //HACK:
                bots = GetLists.FromListUsers("bot", "", 0);//FromCategory(false, "Wikipedia bots");

                foreach (Article a in bots)
                {
                    if (a.NameSpaceKey == 2)
                    {
                        item = a.Name.Replace("User:", "");
                        if (!lbWhiteList.Items.Contains(item))
                            lbWhiteList.Items.Add(item);
                    }
                }

                lblWhiteListCount.Text = lbWhiteList.Items.Count.ToString();
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
            finally
            {
                WhitelistEnabled(true);
            }
        }

        private void btnWhiteListAddAdmins_Click(object sender, EventArgs e)
        {
            WhitelistEnabled(false);

            try
            {
                foreach (Article a in GetLists.FromListUsers("sysop", "", 0))
                {
                    string name = a.Name.Replace(Variables.Namespaces[2], "");
                    if (!lbWhiteList.Items.Contains(name))
                        lbWhiteList.Items.Add(name);
                }

                lblWhiteListCount.Text = lbWhiteList.Items.Count.ToString();
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
            finally
            {
                WhitelistEnabled(true);
            }
        }

        private void WhitelistEnabled(bool enabled)
        {
            btnWhiteListAddAdmins.Enabled = btnWhiteListeAddBots.Enabled = enabled;
        }

        private void btnImportWatchList_Click(object sender, EventArgs e)
        {
            try
            {
                List<Article> list = new List<Article>();
                list = GetLists.FromWatchList();

                foreach (Article a in list)
                {
                    if (!lbWatchList.Items.Contains(a))
                        lbWatchList.Items.Add(a);
                }

                lblWatchListCount.Text = lbWatchList.Items.Count.ToString();
            }
            catch (PageDoesNotExistException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void lbWhiteList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                lbWhiteList.BeginUpdate();
                while (lbWhiteList.SelectedItems.Count > 0)
                    lbWhiteList.Items.Remove(lbWhiteList.SelectedItem);
                lbWhiteList.EndUpdate();
            }

            lblWhiteListCount.Text = lbWhiteList.Items.Count.ToString();
        }

        private void lbBlackList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                lbBlackList.BeginUpdate();
                while (lbBlackList.SelectedItems.Count > 0)
                    lbBlackList.Items.Remove(lbBlackList.SelectedItem);
                lbBlackList.EndUpdate();
            }

            lblBlackListCount.Text = lbBlackList.Items.Count.ToString();
        }

        private void lbWatchList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                lbWatchList.BeginUpdate();
                while (lbWatchList.SelectedItems.Count > 0)
                    lbWatchList.Items.Remove(lbWatchList.SelectedItem);
                lbWatchList.EndUpdate();
            }

            lblWatchListCount.Text = lbWatchList.Items.Count.ToString();
        }

        #endregion

        #region load/save settings

        private void loadSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadSettingsDialog();
        }

        private void saveSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void loadDefaultSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetSettings();
        }

        private void ResetSettings()
        {
            //edit
            cmboEditIP.SelectedIndex = 0;
            cmboEditMinor.SelectedIndex = 0;
            cmboEditNamespace.SelectedIndex = 0;
            chkEditTitleRegex.Checked = false;
            txtEditTitleRegex.Text = "";
            nudLessThan.Value = 0;
            nudMoreThan.Value = 0;

            //new
            chkShowNewArticles.Checked = true;
            chkShowUploads.Checked = true;
            cmboNewStuffNamespace.SelectedIndex = 0;

            //actions
            chkShowBlocks.Checked = true;
            chkShowUnblocks.Checked = true;
            chkShowProtections.Checked = true;
            chkShowUnprotections.Checked = true;
            chkShowDeletions.Checked = true;
            chkShowRestores.Checked = true;
            cmboPageMoveNamespace.SelectedIndex = 0;

            //lists
            chkFlashBlackListed.Checked = true;
            chkFlashWatchlisted.Checked = true;
            chkBlackListSound.Checked = true;
            lbBlackList.Items.Clear();
            lbWhiteList.Items.Clear();
            lbWatchList.Items.Clear();
            chkSoundOnWatchedChanged.Checked = true;

            //options
            ColourIP = Color.LightSkyBlue;
            UserColour = Color.LightGreen;
            WhiteListColour = Color.Wheat;
            BlackListColour = Color.Red;
            WatchListColour = Color.Plum;
            CheckedColour = Color.Green;

            chkChangeCheckedColour.Checked = true;
            chkIgnoreWhiteList.Checked = false;
            chkOnlyBlackAndWatched.Checked = false;
            chkBrowser.Checked = true;

            txtNickname.Text = "";
            txtServer.Text = "irc.wikimedia.org";
            LoadIrcChannels();
            txtPort.Text = "6667";
        }

        private void LoadSettingsDialog()
        {
            if (openXML.ShowDialog() != DialogResult.OK)
                return;
            LoadSettings(openXML.FileName);
        }

        private void LoadDefaultSettings()
        {//load Default.xml file if it exists
            try
            {
                string filename = Environment.CurrentDirectory + "\\IRCMonitorDefault.xml";

                if (File.Exists(filename))
                    LoadSettings(filename);
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }

        //TODO:Use AWB Style Serialisation
        private void LoadSettings(string fileName)
        {
            try
            {
                //clear old settings
                ResetSettings();

                Stream stream = new FileStream(fileName, FileMode.Open);

                using (XmlTextReader reader = new XmlTextReader(stream))
                {
                    reader.WhitespaceHandling = WhitespaceHandling.None;
                    while (reader.Read())
                    {
                        if (reader.Name == "editFilter" && reader.HasAttributes)
                        {
                            if (reader.MoveToAttribute("IPIndex"))
                                cmboEditIP.SelectedIndex = int.Parse(reader.Value);
                            if (reader.MoveToAttribute("minorIndex"))
                                cmboEditMinor.SelectedIndex = int.Parse(reader.Value);
                            if (reader.MoveToAttribute("nameSpaceIndex"))
                                cmboEditNamespace.SelectedIndex = int.Parse(reader.Value);
                            continue;
                        }
                        if (reader.Name == "editTitleRegex" && reader.HasAttributes)
                        {
                            if (reader.MoveToAttribute("enabled"))
                                chkEditTitleRegex.Checked = bool.Parse(reader.Value);
                            if (reader.MoveToAttribute("value"))
                                txtEditTitleRegex.Text = reader.Value;
                            continue;
                        }
                        if (reader.Name == "editLessMoreThan" && reader.HasAttributes)
                        {
                            if (reader.MoveToAttribute("less"))
                                nudLessThan.Value = int.Parse(reader.Value);
                            if (reader.MoveToAttribute("more"))
                                nudMoreThan.Value = int.Parse(reader.Value);
                            continue;
                        }
                        if (reader.Name == "newFilter" && reader.HasAttributes)
                        {
                            if (reader.MoveToAttribute("showNewArticles"))
                                chkShowNewArticles.Checked = bool.Parse(reader.Value);
                            if (reader.MoveToAttribute("showUploads"))
                                chkShowUploads.Checked = bool.Parse(reader.Value);
                            if (reader.MoveToAttribute("namespace"))
                                cmboNewStuffNamespace.SelectedIndex = int.Parse(reader.Value);
                            continue;
                        }
                        if (reader.Name == "actions" && reader.HasAttributes)
                        {
                            if (reader.MoveToAttribute("showBlocks"))
                                chkShowBlocks.Checked = bool.Parse(reader.Value);
                            if (reader.MoveToAttribute("showUnblocks"))
                                chkShowUnblocks.Checked = bool.Parse(reader.Value);
                            if (reader.MoveToAttribute("showProtections"))
                                chkShowProtections.Checked = bool.Parse(reader.Value);
                            if (reader.MoveToAttribute("showUnprotections"))
                                chkShowUnprotections.Checked = bool.Parse(reader.Value);
                            if (reader.MoveToAttribute("showDeletions"))
                                chkShowDeletions.Checked = bool.Parse(reader.Value);
                            if (reader.MoveToAttribute("showRestores"))
                                chkShowRestores.Checked = bool.Parse(reader.Value);
                            if (reader.MoveToAttribute("pageMoveNamespace"))
                                cmboPageMoveNamespace.SelectedIndex = int.Parse(reader.Value);

                            continue;
                        }
                        if (reader.Name == "lists" && reader.HasAttributes)
                        {
                            if (reader.MoveToAttribute("flashOnBlacklisted"))
                                chkFlashBlackListed.Checked = bool.Parse(reader.Value);
                            if (reader.MoveToAttribute("flashWatchlisted"))
                                chkFlashWatchlisted.Checked = bool.Parse(reader.Value);
                            if (reader.MoveToAttribute("soundOnBlacklisted"))
                                chkBlackListSound.Checked = bool.Parse(reader.Value);
                            if (reader.MoveToAttribute("soundOnWatchListed"))
                                chkSoundOnWatchedChanged.Checked = bool.Parse(reader.Value);
                            continue;
                        }
                        if (reader.Name == "colours" && reader.HasAttributes)
                        {
                            if (reader.MoveToAttribute("IPColour"))
                                ColourIP = Color.FromArgb(int.Parse(reader.Value));
                            if (reader.MoveToAttribute("userColour"))
                                UserColour = Color.FromArgb(int.Parse(reader.Value));
                            if (reader.MoveToAttribute("whiteListColour"))
                                WhiteListColour = Color.FromArgb(int.Parse(reader.Value));
                            if (reader.MoveToAttribute("blacklistColour"))
                                BlackListColour = Color.FromArgb(int.Parse(reader.Value));
                            if (reader.MoveToAttribute("watchedColour"))
                                WatchListColour = Color.FromArgb(int.Parse(reader.Value));
                            if (reader.MoveToAttribute("checkedColour"))
                                CheckedColour = Color.FromArgb(int.Parse(reader.Value));
                            continue;
                        }
                        if (reader.Name == "preferences" && reader.HasAttributes)
                        {
                            if (reader.MoveToAttribute("changeCheckedColour"))
                                chkChangeCheckedColour.Checked = bool.Parse(reader.Value);
                            if (reader.MoveToAttribute("ignoreWhiteList"))
                                chkIgnoreWhiteList.Checked = bool.Parse(reader.Value);
                            if (reader.MoveToAttribute("onlyBlackAndWatched"))
                                chkOnlyBlackAndWatched.Checked = bool.Parse(reader.Value);
                            if (reader.MoveToAttribute("userBrowser"))
                                chkBrowser.Checked = bool.Parse(reader.Value);
                            continue;
                        }
                        if (reader.Name == "IRCSettings" && reader.HasAttributes)
                        {
                            if (reader.MoveToAttribute("nickName"))
                                txtNickname.Text = reader.Value;
                            if (reader.MoveToAttribute("server"))
                                txtServer.Text = reader.Value;
                            if (reader.MoveToAttribute("channel"))
                            {
                                Match channel = Regex.Match(reader.Value, "#([a-zA-z]{2}).(wiki(news|quote|books|species|source|versity|pedia)|wiktionary)");

                                if (channel.Success)
                                {
                                    cmboLang.Text = channel.Groups[1].Value;
                                    cmboProject.Text = channel.Groups[2].Value;
                                }
                            }
                            if (reader.MoveToAttribute("port"))
                                txtPort.Text = reader.Value;
                            continue;
                        }

                        if (reader.Name == "whitelist" && reader.HasAttributes)
                        {
                            if (reader.MoveToAttribute("user"))
                                lbWhiteList.Items.Add(reader.Value);
                            continue;
                        }
                        if (reader.Name == "blacklist" && reader.HasAttributes)
                        {
                            if (reader.MoveToAttribute("item"))
                                lbBlackList.Items.Add(reader.Value);
                            continue;
                        }
                        if (reader.Name == "watchlist" && reader.HasAttributes)
                        {
                            if (reader.MoveToAttribute("item"))
                                lbWatchList.Items.Add(reader.Value);
                            continue;
                        }
                    }
                    stream.Close();

                    lblBlackListCount.Text = lbBlackList.Items.Count.ToString();
                    lblWhiteListCount.Text = lbWhiteList.Items.Count.ToString();
                    lblWatchListCount.Text = lbWatchList.Items.Count.ToString();
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message, "File error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }

        //TODO:Use AWB Style Serialisation
        private void SaveSettings()
        {
            try
            {
                if (saveXML.ShowDialog() != DialogResult.OK)
                    return;

                XmlTextWriter textWriter = new XmlTextWriter(saveXML.FileName, UTF8Encoding.UTF8);
                // Opens the document
                textWriter.Formatting = Formatting.Indented;
                textWriter.WriteStartDocument();

                // Write first element
                textWriter.WriteStartElement("Settings");
                textWriter.WriteAttributeString("Program", "IRCMonitor");
                textWriter.WriteAttributeString("Schema", "1");

                textWriter.WriteStartElement("Options");

                textWriter.WriteStartElement("editFilter");
                textWriter.WriteAttributeString("IPIndex", cmboEditIP.SelectedIndex.ToString());
                textWriter.WriteAttributeString("minorIndex", cmboEditMinor.SelectedIndex.ToString());
                textWriter.WriteAttributeString("namespaceIndex", cmboEditNamespace.SelectedIndex.ToString());
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("editTitleRegex");
                textWriter.WriteAttributeString("enabled", chkEditTitleRegex.Checked.ToString());
                textWriter.WriteAttributeString("value", txtEditTitleRegex.Text);
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("editLessMoreThan");
                textWriter.WriteAttributeString("less", nudLessThan.Value.ToString());
                textWriter.WriteAttributeString("more", nudMoreThan.Value.ToString());
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("newFilter");
                textWriter.WriteAttributeString("showNewArticles", chkShowNewArticles.Checked.ToString());
                textWriter.WriteAttributeString("showUploads", chkShowUploads.Checked.ToString());
                textWriter.WriteAttributeString("namespace", cmboNewStuffNamespace.SelectedIndex.ToString());
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("actions");
                textWriter.WriteAttributeString("showBlocks", chkShowBlocks.Checked.ToString());
                textWriter.WriteAttributeString("showUnblocks", chkShowUnblocks.Checked.ToString());
                textWriter.WriteAttributeString("showProtections", chkShowProtections.Checked.ToString());
                textWriter.WriteAttributeString("showUnprotections", chkShowUnprotections.Checked.ToString());
                textWriter.WriteAttributeString("showDeletions", chkShowDeletions.Checked.ToString());
                textWriter.WriteAttributeString("showRestores", chkShowRestores.Checked.ToString());
                textWriter.WriteAttributeString("pageMoveNamespace", cmboPageMoveNamespace.SelectedIndex.ToString());
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("lists");
                textWriter.WriteAttributeString("flashOnBlacklisted", chkFlashBlackListed.Checked.ToString());
                textWriter.WriteAttributeString("flashWatchlisted", chkFlashWatchlisted.Checked.ToString());
                textWriter.WriteAttributeString("soundOnBlacklisted", chkBlackListSound.Checked.ToString());
                textWriter.WriteAttributeString("soundOnWatchListed", chkSoundOnWatchedChanged.Checked.ToString());
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("colours");
                textWriter.WriteAttributeString("IPColour", ColourIP.ToArgb().ToString());
                textWriter.WriteAttributeString("userColour", UserColour.ToArgb().ToString());
                textWriter.WriteAttributeString("whiteListColour", WhiteListColour.ToArgb().ToString());
                textWriter.WriteAttributeString("blacklistColour", BlackListColour.ToArgb().ToString());
                textWriter.WriteAttributeString("watchedColour", WatchListColour.ToArgb().ToString());
                textWriter.WriteAttributeString("checkedColour", CheckedColour.ToArgb().ToString());
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("preferences");
                textWriter.WriteAttributeString("changeCheckedColour", chkChangeCheckedColour.Checked.ToString());
                textWriter.WriteAttributeString("ignoreWhiteList", chkIgnoreWhiteList.Checked.ToString());
                textWriter.WriteAttributeString("onlyBlackAndWatched", chkOnlyBlackAndWatched.Checked.ToString());
                textWriter.WriteAttributeString("userBrowser", chkBrowser.Checked.ToString());
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("IRCSettings");
                textWriter.WriteAttributeString("nickName", txtNickname.Text);
                textWriter.WriteAttributeString("server", txtServer.Text);
                textWriter.WriteAttributeString("channel", GetIrcChannel());
                textWriter.WriteAttributeString("port", txtPort.Text);
                textWriter.WriteEndElement();

                textWriter.WriteEndElement();//endoptions

                textWriter.WriteStartElement("Lists");
                int i = 0;
                while (i < lbWhiteList.Items.Count)
                {
                    textWriter.WriteStartElement("whitelist");
                    textWriter.WriteAttributeString("user", lbWhiteList.Items[i].ToString());
                    textWriter.WriteEndElement();
                    i++;
                }
                i = 0;
                while (i < lbBlackList.Items.Count)
                {
                    textWriter.WriteStartElement("blacklist");
                    textWriter.WriteAttributeString("item", lbBlackList.Items[i].ToString());
                    textWriter.WriteEndElement();
                    i++;
                }
                i = 0;
                while (i < lbWatchList.Items.Count)
                {
                    textWriter.WriteStartElement("watchlist");
                    textWriter.WriteAttributeString("item", lbWatchList.Items[i].ToString());
                    textWriter.WriteEndElement();
                    i++;
                }

                textWriter.WriteEndElement();//endlists

                textWriter.WriteEndElement();//endsettings
                // Ends the document.
                textWriter.WriteEndDocument();
                // close writer
                textWriter.Close();
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message, "File error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }
        #endregion

        #region table click events
        //Context menus
        private void listViewEdit_MouseUp_1(object sender, MouseEventArgs e)
        {
            if (listViewEdit.SelectedItems.Count == 0)
                return;

            contextMenuEdit.Show(MousePosition);
        }
        private void listviewNewUsers_MouseUp(object sender, MouseEventArgs e)
        {
            if (listviewNewUsers.SelectedItems.Count == 0)
                return;

            contextMenuNewUser.Show(MousePosition);
        }
        private void listviewNewStuff_MouseUp(object sender, MouseEventArgs e)
        {
            if (listviewNewStuff.SelectedItems.Count == 0)
                return;

            contextMenuNewStuff.Show(MousePosition);
        }
        private void listviewActions_MouseUp(object sender, MouseEventArgs e)
        {
            if (listviewActions.SelectedItems.Count == 0)
                return;

            contextMenuActions.Show(MousePosition);
        }
        private void listviewPageMoves_MouseUp(object sender, MouseEventArgs e)
        {
            if (listviewPageMoves.SelectedItems.Count == 0)
                return;

            contextMenuPageMoves.Show(MousePosition);
        }

        //context menu click events        
        private void listViewEdit_DoubleClick_1(object sender, EventArgs e)
        {
            if (listViewEdit.SelectedItems.Count == 0)
                return;

            OpenInBrowser(listViewEdit.SelectedItems[0].Tag.ToString());
            if (chkChangeCheckedColour.Checked)
                listViewEdit.SelectedItems[0].BackColor = CheckedColour;
        }

        private void articleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenInBrowser(Variables.URL + "/wiki/" + listViewEdit.SelectedItems[0].SubItems[0].Text);
            if (chkChangeCheckedColour.Checked)
                listViewEdit.SelectedItems[0].BackColor = CheckedColour;
        }

        private void userToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string username = listViewEdit.SelectedItems[0].SubItems[1].Text;

            // for anons, display user talk instead of userpage
            if (Tools.IsIP(username)) OpenInBrowser(Variables.URL + "/wiki/User talk:" + username);
            else OpenInBrowser(Variables.URL + "/wiki/User:" + username);
        }

        private void diffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenInBrowser(listViewEdit.SelectedItems[0].Tag.ToString());
            if (chkChangeCheckedColour.Checked)
                listViewEdit.SelectedItems[0].BackColor = CheckedColour;
        }

        private void whitelistUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lbWhiteList.Items.Add(listViewEdit.SelectedItems[0].SubItems[1].Text);
        }

        private void blacklistUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lbBlackList.Items.Add(listViewEdit.SelectedItems[0].SubItems[1].Text);
        }

        private void unwhitelistUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lbWhiteList.Items.Remove(listViewEdit.SelectedItems[0].SubItems[1].Text);
        }

        private void unblacklistUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lbBlackList.Items.Remove(listViewEdit.SelectedItems[0].SubItems[1].Text);
        }

        //NEW USERS
        private void listviewNewUsers_DoubleClick(object sender, EventArgs e)
        {
            OpenInBrowser(Variables.URL + "/wiki/User:" + listviewNewUsers.SelectedItems[0].SubItems[0].Text);
            if (chkChangeCheckedColour.Checked)
                listviewNewUsers.SelectedItems[0].BackColor = CheckedColour;
        }
        private void loadUserPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenInBrowser(Variables.URL + "/wiki/User:" + listviewNewUsers.SelectedItems[0].SubItems[0].Text);
            if (chkChangeCheckedColour.Checked)
                listviewNewUsers.SelectedItems[0].BackColor = CheckedColour;
        }
        private void loadBlockPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenInBrowser(Variables.URL + "/wiki/Special:Blockip/" + listviewNewUsers.SelectedItems[0].SubItems[0].Text);
            if (chkChangeCheckedColour.Checked)
                listviewNewUsers.SelectedItems[0].BackColor = CheckedColour;
        }
        private void addUserToBlackListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lbBlackList.Items.Add(listviewNewUsers.SelectedItems[0].SubItems[0].Text);
        }

        //NEW STUFF
        private void listviewNewStuff_DoubleClick(object sender, EventArgs e)
        {
            OpenInBrowser(Variables.URL + "/wiki/" + listviewNewStuff.SelectedItems[0].SubItems[0].Text);
            if (chkChangeCheckedColour.Checked)
                listviewNewStuff.SelectedItems[0].BackColor = CheckedColour;
        }
        private void loadArticlefileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenInBrowser(Variables.URL + "/wiki/" + listviewNewStuff.SelectedItems[0].SubItems[0].Text);
            if (chkChangeCheckedColour.Checked)
                listviewNewStuff.SelectedItems[0].BackColor = CheckedColour;
        }
        private void loadUserPageToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenInBrowser(Variables.URL + "/wiki/User:" + listviewNewStuff.SelectedItems[0].SubItems[1].Text);
        }
        private void addUserToBlacklistToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            lbBlackList.Items.Add(listviewNewStuff.SelectedItems[0].SubItems[1].Text);
        }
        private void addArticlefileToWatchlistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lbWatchList.Items.Add(listviewNewStuff.SelectedItems[0].SubItems[0].Text);
        }

        //ACTIONS
        private void listviewActions_DoubleClick(object sender, EventArgs e)
        {
            OpenInBrowser(Variables.URLLong + "index.php?title=Special%3ALog&type=&user=" + listviewActions.SelectedItems[0].SubItems[0].Text + "&page=");
            if (chkChangeCheckedColour.Checked)
                listviewActions.SelectedItems[0].BackColor = CheckedColour;
        }
        private void loadAdminTalkPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenInBrowser(Variables.URL + "/wiki/User:" + listviewActions.SelectedItems[0].SubItems[0].Text);
            if (chkChangeCheckedColour.Checked)
                listviewActions.SelectedItems[0].BackColor = CheckedColour;
        }
        private void loadAdminsLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenInBrowser(Variables.URLLong + "index.php?title=Special%3ALog&type=&user=" + listviewActions.SelectedItems[0].SubItems[0].Text + "&page=");
            if (chkChangeCheckedColour.Checked)
                listviewActions.SelectedItems[0].BackColor = CheckedColour;
        }
        private void loadArticleuserPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenInBrowser(Variables.URL + "/wiki/" + listviewActions.SelectedItems[0].SubItems[2].Text);
            if (chkChangeCheckedColour.Checked)
                listviewActions.SelectedItems[0].BackColor = CheckedColour;
        }

        //PAGEMOVES
        private void listviewPageMoves_DoubleClick(object sender, EventArgs e)
        {
            OpenInBrowser(Variables.URL + "/wiki/" + listviewPageMoves.SelectedItems[0].SubItems[1].Text);
            if (chkChangeCheckedColour.Checked)
                listviewPageMoves.SelectedItems[0].BackColor = CheckedColour;
        }
        private void loadNewPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenInBrowser(Variables.URL + "/wiki/" + listviewPageMoves.SelectedItems[0].SubItems[1].Text);
            if (chkChangeCheckedColour.Checked)
                listviewPageMoves.SelectedItems[0].BackColor = CheckedColour;
        }
        private void addUseToBlacklistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lbBlackList.Items.Add(listviewPageMoves.SelectedItems[0].SubItems[2].Text);
        }
        private void addarticletoWatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lbWatchList.Items.Add(listviewPageMoves.SelectedItems[0].SubItems[1].Text);
        }

        #endregion

        #region Browser

        private void webBrowser_DocumentCompleted_1(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            pbBrowserProgess.Style = ProgressBarStyle.Continuous;
            pbBrowserProgess.MarqueeAnimationSpeed = 0;
            txtURL.Text = webBrowser.Url.AbsoluteUri;
            btnBack.Enabled = webBrowser.CanGoBack;
            btnFoward.Enabled = webBrowser.CanGoForward;
            btnStop.Enabled = false;

            string loggedInUser = webBrowser.UserName;

            if (!WikiFunctions.Variables.User.WikiStatus || !webBrowser.Url.ToString().StartsWith(Variables.URL) ||
                WikiFunctions.Variables.User.Name != loggedInUser)
            {
                WikiFunctions.Variables.User.WikiStatus = false;
                UpdateButtons();
                WikiFunctions.Variables.User.UpdateWikiStatus();
            }
            else
            {
                UpdateButtons();
            }
        }

        private void webBrowser_Navigating_1(object sender, WebBrowserNavigatingEventArgs e)
        {
            pbBrowserProgess.Style = ProgressBarStyle.Marquee;
            pbBrowserProgess.MarqueeAnimationSpeed = 100;
            btnBack.Enabled = webBrowser.CanGoBack;
            btnFoward.Enabled = webBrowser.CanGoForward;
            btnStop.Enabled = true;
        }

        private void revertAndWarnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NextTask = NextTaskType.Warn;
            VandalizedPage = webBrowser.ArticleTitle;
            Revert(Project.RevertSummary, webBrowser.Revid, out VandalName);
        }

        private void revertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NextTask = NextTaskType.None;
            VandalizedPage = webBrowser.ArticleTitle;
            Revert(Project.RevertSummary, webBrowser.Revid, out VandalName);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            webBrowser.GoBack();
        }

        private void btnFoward_Click(object sender, EventArgs e)
        {
            webBrowser.GoForward();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            webBrowser.Stop2();
        }

        private void btnOpenInBrowser_Click(object sender, EventArgs e)
        {
            Tools.OpenURLInBrowser(webBrowser.Url.AbsoluteUri);
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            webBrowser.Navigate(txtURL.Text);
        }

        #endregion

        #region Vandalfighting
        void Revert(string summary, int badrev, out string username)
        {
            int i;
            Editor e = new Editor();

            username = null;
            List<WikiFunctions.Editor.Revision> hist = e.GetHistory(webBrowser.ArticleTitle, 50);
            foreach (WikiFunctions.Editor.Revision r in hist)
            {
                if (r.RevisionID == badrev)
                {
                    username = r.User;
                    break;
                }
            }

            if (hist[0].User != username)
            {
                webBrowser.Navigate(Variables.URLLong + "index.php?title=" + HttpUtility.UrlEncode(webBrowser.ArticleTitle) + "&action=history");
                MessageBox.Show("Another user has edited this article. Please look at its history and check if there is vandalism", "Cannot revert", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            for (i = 0; i <= hist.Count && hist[i].User == username; i++)
            {
            }

            if (hist[i].User != username)
            {
                summary = summary.Replace("%v", username);
                summary = summary.Replace("%u", hist[i].User);
                summary += Project.Using;

                webBrowser.LoadEditPage(webBrowser.ArticleTitle, hist[i].RevisionID);//, hist[i].RevisionID);
                webBrowser.Wait();
                ReplacementText = webBrowser.GetArticleText();


                if (string.IsNullOrEmpty(webBrowser.GetArticleText().Trim()))
                {
                    MessageBox.Show("Cannot revert to empty revision. Probably, there is some error.");
                    NextTask = NextTaskType.None;
                    return;
                }

                webBrowser.LoadEditPage(webBrowser.ArticleTitle);
                webBrowser.SetSummary(summary);
                webBrowser.SetArticleText(ReplacementText);
                webBrowser.SetMinor(true);
                webBrowser.Save();
                webBrowser.Wait();
            }
        }

        private void WarnUserClick(object sender, EventArgs e)
        {
            string username = webBrowser.ArticleTitle;
            username = username.Remove(0, username.IndexOf(':') + 1);
            string template = (sender as ToolStripMenuItem).Text;
            template = template.Substring(0, template.IndexOf("}}") + 2);
            WarnUser(username, template, VandalizedPage);
        }

        private void AddStubClick(object sender, EventArgs e)
        {
            string tag = (sender as ToolStripMenuItem).Text;
            tag = tag.Substring(0, tag.LastIndexOf("}}") + 2);

            NextTask = NextTaskType.None;
            webBrowser.LoadEditPage(webBrowser.ArticleTitle);
            webBrowser.Wait();
            string summary;
            webBrowser.SetArticleText(Project.AppendTag(webBrowser.GetArticleText(), tag, out summary));
            webBrowser.SetSummary(summary + Project.Using);
            webBrowser.Save();
        }

        private void AddTagClick(object sender, EventArgs e)
        {
            string tag = (sender as ToolStripMenuItem).Text;
            tag = tag.Substring(0, tag.LastIndexOf("}}") + 2);

            NextTask = NextTaskType.None;
            webBrowser.LoadEditPage(webBrowser.ArticleTitle);
            webBrowser.Wait();
            string summary;
            webBrowser.SetArticleText(Project.PrependTag(webBrowser.GetArticleText(), tag, out summary));
            webBrowser.SetSummary(summary + Project.Using);
            webBrowser.Save();
        }

        public void WarnUser(string user, string template, string pageConcerned)
        {
            string warning = template.Insert(2, "subst:") + " —~~~~";

            webBrowser.LoadEditPage("User talk:" + user);
            webBrowser.Wait();

            webBrowser.AllowNavigation = true;
            webBrowser.SetArticleText(webBrowser.GetArticleText() + "\r\n\r\n" + warning);
            webBrowser.SetSummary(Project.WarningSummary.Replace("%t", template) + Project.Using);
            VandalName = user;
            NextTask = NextTaskType.Blacklist;
            webBrowser.ProcessStage = WikiFunctions.Browser.enumProcessStage.save;
        }

        private void revertAndReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NextTask = NextTaskType.Report;
            VandalizedPage = webBrowser.ArticleTitle;
            Revert(Project.RevertSummary, webBrowser.Revid, out VandalName);
        }
        #endregion

        private void timer1_Tick(object sender, EventArgs e)
        {
            UTCtime.Text = DateTime.UtcNow.ToShortDateString() + " " + DateTime.UtcNow.ToLongTimeString();
        }

        private void reportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NextTask = NextTaskType.None;
            ReportVandal(webBrowser.ArticleTitle.Remove(0, webBrowser.ArticleTitle.IndexOf(':') + 1));
        }

        private void contribsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NextTask = NextTaskType.None;
            webBrowser.Navigate(Variables.URL + "/wiki/Special:Contributions/" +
                webBrowser.ArticleTitle.Remove(0, webBrowser.ArticleTitle.IndexOf(':') + 1));
        }

        private void webBrowser_StatusChanged(object sender, EventArgs e)
        {
            StatusLabelText = webBrowser.StatusText;
        }

        private void logsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NextTask = NextTaskType.None;
            webBrowser.Navigate(Variables.URLLong + "index.php?title=Special:Log&user=" +
                webBrowser.ArticleTitle.Remove(0, webBrowser.ArticleTitle.IndexOf(':') + 1));
        }

        private void blockLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NextTask = NextTaskType.None;
            webBrowser.Navigate(Variables.URLLong + "index.php?title=Special:Log&type=block&page=User:" +
                webBrowser.ArticleTitle.Remove(0, webBrowser.ArticleTitle.IndexOf(':') + 1));
        }

        private void txtURL_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case '\r': //ENTER:
                    OpenInBrowser(txtURL.Text);
                    e.Handled = true;
                    break;
            }
        }

        private void loginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckStatus(true);
        }

        private void iRCMonitorPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tools.OpenENArticleInBrowser("Wikipedia:IRCMonitor", false);
        }

        private void blacklistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddToBlacklist(webBrowser.ArticleTitle.Remove(0, webBrowser.ArticleTitle.IndexOf(':') + 1));
        }

        private void profilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WikiFunctions.Profiles.AWBProfilesForm profiles = new WikiFunctions.Profiles.AWBProfilesForm(webBrowser);
            profiles.LoadProfile += LoadProfileSettings;
            profiles.Show(this);
        }

        private void LoadProfileSettings(object sender, EventArgs e)
        { }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
        }

        private void cmboLang_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboChange();
        }

        private void cmboProject_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmboLang.Enabled = !(cmboProject.Text == "meta" || cmboProject.Text == "commons");
            if (!cmboLang.Enabled) cmboLang.SelectedIndex = -1;
            ComboChange();
        }

        private void ComboChange()
        {
            if (!string.IsNullOrEmpty(cmboLang.Text) && !string.IsNullOrEmpty(cmboProject.Text))
                Variables.SetProject((LangCodeEnum)Enum.Parse(typeof(LangCodeEnum), cmboLang.SelectedItem.ToString()),
                   (ProjectEnum)Enum.Parse(typeof(ProjectEnum), cmboProject.SelectedItem.ToString()));
        }
    }
}
