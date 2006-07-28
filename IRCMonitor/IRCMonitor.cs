/*
WikiFunctions
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

namespace IRCMonitor
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
            cmboEditIP.SelectedIndex = 0;
            cmboEditMinor.SelectedIndex = 0;
            cmboEditNamespace.SelectedIndex = 0;
            cmboNewStuffNamespace.SelectedIndex = 0;
            cmboPageMoveNamespace.SelectedIndex = 0;
            cmboChannel.SelectedIndex = 0;

            btnBack.Image = Resources.GoRtl;
            btnFoward.Image = Resources.GoLtr;
            btnStop.Image = Resources.Stop;
            btnOpenInBrowser.Image = Resources.NewWindow;

            Variables.SetProject("en", "wikipedia");

            btnIPColour.BackColor = ColourIP;
            btnRegisteredUserColour.BackColor = UserColour;
            btnSetWhiteListColour.BackColor = whitelistcolour;
            btnSetBlackListColour.BackColor = BlackListColour;
            btnSetWatchedColour.BackColor = WatchListColour;
            btnSetCheckedColour.BackColor = CheckedColour;

            btnStart.Enabled = false;
        }

        private void IRCMonitor_Load(object sender, EventArgs e)
        {
            ResetStats();
            loadDefaultSettings();
            Start();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            Start();
        }

        WikiIRC IrcObject;

        private void Start()
        {
            Stop();

            Random n = new Random();

            string name = "ircM";
            if (txtNickname.Text == "")
                name += n.Next(1000, 100000).ToString();
            else
                name += txtNickname.Text;

            IrcObject = new WikiIRC(txtServer.Text, int.Parse(txtPort.Text), name, cmboChannel.SelectedItem.ToString());
            WikiIRC.run = true;

            IrcObject.otherMessages += ProcessOtherMessages;
            IrcObject.ConnectEvent += connected;
            IrcObject.DisconnectEvent += disconnected;
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
            IrcObject.Unblock += ProcessUnBlock;


            IrcObject.Start();
        }

        private void Stop()
        {
            if (IrcObject != null)
            {
                WikiIRC.run = false;
                IrcObject.ConnectEvent -= connected;
                IrcObject.DisconnectEvent -= disconnected;
                IrcObject.otherMessages -= ProcessOtherMessages;
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
                IrcObject.Unblock -= ProcessUnBlock;

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
            if (colorDialog.ShowDialog() == DialogResult.OK)
                ColourIP = colorDialog.Color;
        }

        private void btnRegisteredUserColour_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
                UserColour = colorDialog.Color;
        }

        private void btnSetWhiteListColour_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
                WhiteListColour = colorDialog.Color;
        }

        private void btnSetBlackListColour_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
                BlackListColour = colorDialog.Color;
        }

        private void btnSetWatchedColour_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
                WatchListColour = colorDialog.Color;
        }

        private void btnSetCheckedColour_Click(object sender, EventArgs e)
        {
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
            {
                IrcObject.Pause = false;
                btnPause.Text = "Pause";
            }
            else
            {
                IrcObject.Pause = true;
                btnPause.Text = "Resume";
            }
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

        private void connected()
        {
            //  MessageBox.Show("connect");
        }

        private void disconnected()
        {
            if (WikiIRC.run)
            {
                Start();
                textBox1.AppendText("\r\n DISCONNECTED \r\n\r\n");
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            WikiIRC.run = false;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutIRCMon about = new AboutIRCMon();
            about.Show();
        }

        #endregion

        #region Properties

        Color ipcolour = Color.LightSkyBlue;
        public Color ColourIP
        {
            get { return ipcolour; }
            set
            {
                ipcolour = value;
                btnIPColour.BackColor = value;
            }
        }

        Color usercolour = Color.LightGreen;
        public Color UserColour
        {
            get { return usercolour; }
            set
            {
                usercolour = value;
                btnRegisteredUserColour.BackColor = value;
            }
        }

        Color whitelistcolour = Color.Wheat;
        public Color WhiteListColour
        {
            get { return whitelistcolour; }
            set
            {
                whitelistcolour = value;
                btnSetWhiteListColour.BackColor = value;
            }
        }

        Color blacklistcolour = Color.Red;
        public Color BlackListColour
        {
            get { return blacklistcolour; }
            set
            {
                blacklistcolour = value;
                btnSetBlackListColour.BackColor = value;
            }
        }

        Color watchlistcolour = Color.Plum;
        public Color WatchListColour
        {
            get { return watchlistcolour; }
            set
            {
                watchlistcolour = value;
                btnSetWatchedColour.BackColor = value;
            }
        }

        Color checkedcolour = Color.Green;
        public Color CheckedColour
        {
            get { return checkedcolour; }
            set
            {
                checkedcolour = value;
                btnSetCheckedColour.BackColor = value;
            }
        }

        #endregion

        #region Message processing
        readonly Regex isIPAddress = new Regex("^[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}$", RegexOptions.Compiled);
        int MaxNumberOfRows = 500;

        ListViewItem lvItem;
        string[] lviEditArray = { "", "", "", "", "" };
        private void ProcessEdit(string article, string minor, string difflink, string user, int plusminus, string comment)
        {
            bool IPedit = false;
            if (isIPAddress.IsMatch(user))
                IPedit = true;

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
                if (IPedit && cmboEditIP.SelectedIndex == 2)
                    return;
                if (!IPedit && cmboEditIP.SelectedIndex == 1)
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
                watchedItemChanged();
            }
            else if (blacklistedUser)
            {
                lvItem.BackColor = BlackListColour;
                blackListedEdited();
            }
            else if (whiteListedUser)
                lvItem.BackColor = WhiteListColour;
            else if (IPedit)
                lvItem.BackColor = ColourIP;
            else
                lvItem.BackColor = UserColour;

            //add it to the top of the list
            //listViewEdit.Items.Insert(0, lvItem);
            listViewEdit.Items.Insert(0, lvItem);
            if (listViewEdit.Items.Count > MaxNumberOfRows)
                listViewEdit.Items.RemoveAt(MaxNumberOfRows);
            addEdit();
        }

        string[] lviNewUserArray = { "" };
        private void ProcessNewUser(string name)
        {
            lvItem = new ListViewItem(name);

            listviewNewUsers.Items.Insert(0, lvItem);
            if (listviewNewUsers.Items.Count > MaxNumberOfRows)
                listviewNewUsers.Items.RemoveAt(MaxNumberOfRows);
            addNewUser();
        }

        string[] lviNewArticleArray = { "", "", "", "" };
        private void ProcessNewArticles(string article, string user, int plusmin, string comment)
        {
            bool IPedit = false;
            if (isIPAddress.IsMatch(user))
                IPedit = true;

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
                watchedItemChanged();
            }
            else if (blacklistedUser)
            {
                lvItem.BackColor = BlackListColour;
                blackListedEdited();
            }
            else if (whiteListedUser)
                lvItem.BackColor = WhiteListColour;
            else if (IPedit)
                lvItem.BackColor = ColourIP;
            else
                lvItem.BackColor = UserColour;

            listviewNewStuff.Items.Insert(0, lvItem);
            if (listviewNewStuff.Items.Count > MaxNumberOfRows)
                listviewNewStuff.Items.RemoveAt(MaxNumberOfRows);
            addNewArticle();
        }

        string[] lviUploadArray = { "", "", "", "" };
        private void ProcessUpload(string file, string user, string comment)
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
                watchedItemChanged();
            }
            else if (blacklistedUser)
            {
                lvItem.BackColor = BlackListColour;
                blackListedEdited();
            }
            else if (whiteListedUser)
                lvItem.BackColor = WhiteListColour;
            else
                lvItem.BackColor = UserColour;

            listviewNewStuff.Items.Insert(0, lvItem);
            if (listviewNewStuff.Items.Count > MaxNumberOfRows)
                listviewNewStuff.Items.RemoveAt(MaxNumberOfRows);
            addNewUpload();
        }

        string[] lviMoveArray = { "", "", "", "" };
        private void ProcessMove(string oldName, string newName, string user, string comment)
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
                watchedItemChanged();
            }
            else if (blacklistedUser)
            {
                lvItem.BackColor = blacklistcolour;
                blackListedEdited();
            }
            else if (whiteListedUser)
                lvItem.BackColor = WhiteListColour;
            else
                lvItem.BackColor = UserColour;

            listviewPageMoves.Items.Insert(0, lvItem);
            if (listviewPageMoves.Items.Count > MaxNumberOfRows)
                listviewPageMoves.Items.RemoveAt(MaxNumberOfRows);
            addNewPageMove();
        }

        private void ProcessDelete(string admin, string article, string comment)
        {
            ProcessActions(admin, "DELETE", article, comment);
            addNewDeletion();
        }

        private void ProcessRestore(string admin, string article, string comment)
        {
            ProcessActions(admin, "RESTORE", article, comment);
        }

        private void ProcessBlock(string admin, string user, string comment, string time)
        {
            ProcessActions(admin, "BLOCK", user, time + comment);
            addNewBlock();
        }

        private void ProcessUnBlock(string admin, string user, string comment)
        {
            ProcessActions(admin, "UNBLOCK", user, comment);
        }

        private void ProcessProtection(string admin, string article, string comment)
        {
            ProcessActions(admin, "PROTECT", article, comment);
            addNewProtection();
        }

        private void ProcessUnprotection(string admin, string article, string comment)
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
                watchedItemChanged();
            }
            else if (blacklistedUser)
            {
                lvItem.BackColor = BlackListColour;
                blackListedEdited();
            }
            else if (lbWhiteList.Items.Contains(admin))
                lvItem.BackColor = WhiteListColour;
            else
                lvItem.BackColor = UserColour;

            listviewActions.Items.Insert(0, lvItem);
            if (listviewActions.Items.Count > MaxNumberOfRows)
                listviewActions.Items.RemoveAt(MaxNumberOfRows);
        }

        private void ProcessOtherMessages(string msg)
        {
            textBox1.AppendText(msg + "\r\n\r\n");
        }

        #endregion

        #region helper functions


        private void watchedItemChanged()
        {
            if (chkFlashWatchlisted.Checked && !this.ContainsFocus)
                Tools.FlashWindow(this);

            if (chkSoundOnWatchedChanged.Checked)
                Tools.Beep1();
        }

        private void blackListedEdited()
        {
            if (chkFlashBlackListed.Checked)
                Tools.FlashWindow(this);

            if (chkBlackListSound.Checked)
                Tools.Beep2();
        }

        private int intFromMessage(string s)
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

        private bool CheckNameSpace(string Article, int index)
        {
            if (index == 1 && !Tools.IsMainSpace(Article))
                return false;
            if (index == 2 && !Tools.IsNotTalk(Article))
                return false;

            return true;
        }

        private void openInBrowser(string URL)
        {
            if (chkBrowser.Checked)
            {
                webBrowser.Navigate(URL);
                tabControl.SelectedTab = tabPage8;
            }
            else
                System.Diagnostics.Process.Start(URL);
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

        int intNumberofEdits = 0;
        private void addEdit()
        {
            intNumberofEdits++;
            dataGridStatistics.Rows[0].Cells[1].Value = intNumberofEdits;
        }

        int intNumberofNewArticle = 0;
        private void addNewArticle()
        {
            intNumberofNewArticle++;
            dataGridStatistics.Rows[1].Cells[1].Value = intNumberofNewArticle;
        }

        int intNumberofUploads = 0;
        private void addNewUpload()
        {
            intNumberofUploads++;
            dataGridStatistics.Rows[2].Cells[1].Value = intNumberofUploads;
        }

        int intNumberofPageMoves = 0;
        private void addNewPageMove()
        {
            intNumberofPageMoves++;
            dataGridStatistics.Rows[3].Cells[1].Value = intNumberofPageMoves;
        }

        int intNumberofNewUsers = 0;
        private void addNewUser()
        {
            intNumberofNewUsers++;
            dataGridStatistics.Rows[4].Cells[1].Value = intNumberofNewUsers;
        }

        int intNumberofBlocks = 0;
        private void addNewBlock()
        {
            intNumberofBlocks++;
            dataGridStatistics.Rows[5].Cells[1].Value = intNumberofBlocks;
        }

        int intNumberofDeletions = 0;
        private void addNewDeletion()
        {
            intNumberofDeletions++;
            dataGridStatistics.Rows[6].Cells[1].Value = intNumberofDeletions;
        }

        int intNumberofProtection = 0;
        private void addNewProtection()
        {
            intNumberofProtection++;
            dataGridStatistics.Rows[7].Cells[1].Value = intNumberofProtection;
        }

        #endregion

        #region Lists

        private void btnWhiteListAdd_Click(object sender, EventArgs e)
        {
            if (txtList.Text == "")
                return;

            lbWhiteList.Items.Add(Tools.TurnFirstToUpper(txtList.Text));
            txtList.Clear();

            lblWhiteListCount.Text = lbWhiteList.Items.Count.ToString();
        }

        private void btnBlackListAdd_Click(object sender, EventArgs e)
        {
            if (txtList.Text == "")
                return;

            lbBlackList.Items.Add(Tools.TurnFirstToUpper(txtList.Text));
            txtList.Clear();

            lblBlackListCount.Text = lbBlackList.Items.Count.ToString();
        }

        private void btnWhiteListeAddBots_Click(object sender, EventArgs e)
        {
            btnWhiteListAddAdmins.Enabled = false;
            btnWhiteListeAddBots.Enabled = false;

            try
            {
                List<Article> bots = new List<Article>();
                string item = "";
                bots = GetLists.FromCategory("Wikipedia bots");

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
                MessageBox.Show(ex.Message);
            }
            finally
            {
                btnWhiteListAddAdmins.Enabled = true;
                btnWhiteListeAddBots.Enabled = true;
            }
        }

        private void btnWhiteListAddAdmins_Click(object sender, EventArgs e)
        {
            btnWhiteListAddAdmins.Enabled = false;
            btnWhiteListeAddBots.Enabled = false;

            try
            {
                string page = "";
                string item = "";
                page = Tools.GetArticleText("Wikipedia:List of administrators");

                foreach (Match m in Regex.Matches(page, "# \\{\\{admin\\|(.*?)(\\||\\}\\})"))
                {
                    item = m.Groups[1].Value.Replace("_", " ");
                    if (!lbWhiteList.Items.Contains(item))
                        lbWhiteList.Items.Add(item);
                }

                lblWhiteListCount.Text = lbWhiteList.Items.Count.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                btnWhiteListAddAdmins.Enabled = true;
                btnWhiteListeAddBots.Enabled = true;
            }
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
            catch (PageDoeNotExistException ex)
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
            loadSettingsDialog();
        }

        private void saveSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveSettings();
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
            cmboChannel.SelectedIndex = 0;
            txtPort.Text = "6667";
        }

        private void loadSettingsDialog()
        {
            if (openXML.ShowDialog() != DialogResult.OK)
                return;
            loadSettings(openXML.FileName);
        }

        private void loadDefaultSettings()
        {//load Default.xml file if it exists
            try
            {
                string filename = Environment.CurrentDirectory + "\\IRCMonitorDefault.xml";

                if (File.Exists(filename))
                    loadSettings(filename);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void loadSettings(string fileName)
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
                            reader.MoveToAttribute("IPIndex");
                            cmboEditIP.SelectedIndex = int.Parse(reader.Value);
                            reader.MoveToAttribute("minorIndex");
                            cmboEditMinor.SelectedIndex = int.Parse(reader.Value);
                            reader.MoveToAttribute("nameSpaceIndex");
                            cmboEditNamespace.SelectedIndex = int.Parse(reader.Value);
                            continue;
                        }
                        if (reader.Name == "editTitleRegex" && reader.HasAttributes)
                        {
                            reader.MoveToAttribute("enabled");
                            chkEditTitleRegex.Checked = bool.Parse(reader.Value);
                            reader.MoveToAttribute("value");
                            txtEditTitleRegex.Text = reader.Value;
                            continue;
                        }
                        if (reader.Name == "editLessMoreThan" && reader.HasAttributes)
                        {
                            reader.MoveToAttribute("less");
                            nudLessThan.Value = int.Parse(reader.Value);
                            reader.MoveToAttribute("more");
                            nudMoreThan.Value = int.Parse(reader.Value);
                            continue;
                        }
                        if (reader.Name == "newFilter" && reader.HasAttributes)
                        {
                            reader.MoveToAttribute("showNewArticles");
                            chkShowNewArticles.Checked = bool.Parse(reader.Value);
                            reader.MoveToAttribute("showUploads");
                            chkShowUploads.Checked = bool.Parse(reader.Value);
                            reader.MoveToAttribute("namespace");
                            cmboNewStuffNamespace.SelectedIndex = int.Parse(reader.Value);
                            continue;
                        }
                        if (reader.Name == "actions" && reader.HasAttributes)
                        {
                            reader.MoveToAttribute("showBlocks");
                            chkShowBlocks.Checked = bool.Parse(reader.Value);
                            reader.MoveToAttribute("showUnblocks");
                            chkShowUnblocks.Checked = bool.Parse(reader.Value);
                            reader.MoveToAttribute("showProtections");
                            chkShowProtections.Checked = bool.Parse(reader.Value);
                            reader.MoveToAttribute("showUnprotections");
                            chkShowUnprotections.Checked = bool.Parse(reader.Value);
                            reader.MoveToAttribute("showDeletions");
                            chkShowDeletions.Checked = bool.Parse(reader.Value);
                            reader.MoveToAttribute("showRestores");
                            chkShowRestores.Checked = bool.Parse(reader.Value);
                            reader.MoveToAttribute("pageMoveNamespace");
                            cmboPageMoveNamespace.SelectedIndex = int.Parse(reader.Value);

                            continue;
                        }
                        if (reader.Name == "lists" && reader.HasAttributes)
                        {
                            reader.MoveToAttribute("flashOnBlacklisted");
                            chkFlashBlackListed.Checked = bool.Parse(reader.Value);
                            reader.MoveToAttribute("flashWatchlisted");
                            chkFlashWatchlisted.Checked = bool.Parse(reader.Value);
                            reader.MoveToAttribute("soundOnBlacklisted");
                            chkBlackListSound.Checked = bool.Parse(reader.Value);
                            reader.MoveToAttribute("soundOnWatchListed");
                            chkSoundOnWatchedChanged.Checked = bool.Parse(reader.Value);
                            continue;
                        }
                        if (reader.Name == "colours" && reader.HasAttributes)
                        {
                            reader.MoveToAttribute("IPColour");
                            ColourIP = Color.FromArgb(int.Parse(reader.Value));
                            reader.MoveToAttribute("userColour");
                            UserColour = Color.FromArgb(int.Parse(reader.Value));
                            reader.MoveToAttribute("whiteListColour");
                            WhiteListColour = Color.FromArgb(int.Parse(reader.Value));
                            reader.MoveToAttribute("blacklistColour");
                            BlackListColour = Color.FromArgb(int.Parse(reader.Value));
                            reader.MoveToAttribute("watchedColour");
                            WatchListColour = Color.FromArgb(int.Parse(reader.Value));
                            reader.MoveToAttribute("checkedColour");
                            CheckedColour = Color.FromArgb(int.Parse(reader.Value));
                            continue;
                        }
                        if (reader.Name == "preferences" && reader.HasAttributes)
                        {
                            reader.MoveToAttribute("changeCheckedColour");
                            chkChangeCheckedColour.Checked = bool.Parse(reader.Value);
                            reader.MoveToAttribute("ignoreWhiteList");
                            chkIgnoreWhiteList.Checked = bool.Parse(reader.Value);
                            reader.MoveToAttribute("onlyBlackAndWatched");
                            chkOnlyBlackAndWatched.Checked = bool.Parse(reader.Value);
                            reader.MoveToAttribute("userBrowser");
                            chkBrowser.Checked = bool.Parse(reader.Value);
                            continue;
                        }
                        if (reader.Name == "IRCSettings" && reader.HasAttributes)
                        {
                            reader.MoveToAttribute("nickName");
                            txtNickname.Text = reader.Value;
                            reader.MoveToAttribute("server");
                            txtServer.Text = reader.Value;
                            reader.MoveToAttribute("channel");
                            cmboChannel.SelectedIndex = int.Parse(reader.Value);
                            reader.MoveToAttribute("port");
                            txtPort.Text = reader.Value;
                            continue;
                        }

                        if (reader.Name == "whitelist" && reader.HasAttributes)
                        {
                            reader.MoveToAttribute("user");
                            lbWhiteList.Items.Add(reader.Value);
                            continue;
                        }
                        if (reader.Name == "blacklist" && reader.HasAttributes)
                        {
                            reader.MoveToAttribute("item");
                            lbBlackList.Items.Add(reader.Value);
                            continue;
                        }
                        if (reader.Name == "watchlist" && reader.HasAttributes)
                        {
                            reader.MoveToAttribute("item");
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
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void saveSettings()
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
                textWriter.WriteAttributeString("channel", cmboChannel.SelectedIndex.ToString());
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
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            openInBrowser(listViewEdit.SelectedItems[0].Tag.ToString());
            if (chkChangeCheckedColour.Checked)
                listViewEdit.SelectedItems[0].BackColor = CheckedColour;
        }

        private void articleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openInBrowser("http://en.wikipedia.org/wiki/" + listViewEdit.SelectedItems[0].SubItems[0].Text);
            if (chkChangeCheckedColour.Checked)
                listViewEdit.SelectedItems[0].BackColor = CheckedColour;
        }

        private void userToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openInBrowser("http://en.wikipedia.org/wiki/User:" + listViewEdit.SelectedItems[0].SubItems[1].Text);
        }

        private void diffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openInBrowser(listViewEdit.SelectedItems[0].Tag.ToString());
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
            openInBrowser("http://en.wikipedia.org/wiki/User:" + listviewNewUsers.SelectedItems[0].SubItems[0].Text);
            if (chkChangeCheckedColour.Checked)
                listviewNewUsers.SelectedItems[0].BackColor = CheckedColour;
        }
        private void loadUserPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openInBrowser("http://en.wikipedia.org/wiki/User:" + listviewNewUsers.SelectedItems[0].SubItems[0].Text);
            if (chkChangeCheckedColour.Checked)
                listviewNewUsers.SelectedItems[0].BackColor = CheckedColour;
        }
        private void loadBlockPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openInBrowser("http://en.wikipedia.org/wiki/Special:Blockip/" + listviewNewUsers.SelectedItems[0].SubItems[0].Text);
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
            openInBrowser("http://en.wikipedia.org/wiki/" + listviewNewStuff.SelectedItems[0].SubItems[0].Text);
            if (chkChangeCheckedColour.Checked)
                listviewNewStuff.SelectedItems[0].BackColor = CheckedColour;
        }
        private void loadArticlefileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openInBrowser("http://en.wikipedia.org/wiki/" + listviewNewStuff.SelectedItems[0].SubItems[0].Text);
            if (chkChangeCheckedColour.Checked)
                listviewNewStuff.SelectedItems[0].BackColor = CheckedColour;
        }
        private void loadUserPageToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            openInBrowser("http://en.wikipedia.org/wiki/User:" + listviewNewStuff.SelectedItems[0].SubItems[1].Text);
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
            openInBrowser("http://en.wikipedia.org/w/index.php?title=Special%3ALog&type=&user=" + listviewActions.SelectedItems[0].SubItems[0].Text + "&page=");
            if (chkChangeCheckedColour.Checked)
                listviewActions.SelectedItems[0].BackColor = CheckedColour;
        }
        private void loadAdminTalkPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openInBrowser("http://en.wikipedia.org/wiki/User:" + listviewActions.SelectedItems[0].SubItems[0].Text);
            if (chkChangeCheckedColour.Checked)
                listviewActions.SelectedItems[0].BackColor = CheckedColour;
        }
        private void loadAdminsLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openInBrowser("http://en.wikipedia.org/w/index.php?title=Special%3ALog&type=&user=" + listviewActions.SelectedItems[0].SubItems[0].Text + "&page=");
            if (chkChangeCheckedColour.Checked)
                listviewActions.SelectedItems[0].BackColor = CheckedColour;
        }
        private void loadArticleuserPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openInBrowser("http://en.wikipedia.org/wiki/" + listviewActions.SelectedItems[0].SubItems[2].Text);
            if (chkChangeCheckedColour.Checked)
                listviewActions.SelectedItems[0].BackColor = CheckedColour;
        }

        //PAGEMOVES
        private void listviewPageMoves_DoubleClick(object sender, EventArgs e)
        {
            openInBrowser("http://en.wikipedia.org/wiki/" + listviewPageMoves.SelectedItems[0].SubItems[1].Text);
            if (chkChangeCheckedColour.Checked)
                listviewPageMoves.SelectedItems[0].BackColor = CheckedColour;
        }
        private void loadNewPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openInBrowser("http://en.wikipedia.org/wiki/" + listviewPageMoves.SelectedItems[0].SubItems[1].Text);
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

        }

        private void revertToolStripMenuItem_Click(object sender, EventArgs e)
        {

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
            webBrowser.Stop();
        }

        private void btnOpenInBrowser_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(webBrowser.Url.AbsoluteUri);
        }
        private void btnGo_Click(object sender, EventArgs e)
        {
            webBrowser.Navigate(txtURL.Text);
        }

        #endregion
    }
}