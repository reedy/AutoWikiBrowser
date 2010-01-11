/*
AWB Profiles
Copyright (C) 2008 Sam Reed, Stephen Kennedy

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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WikiFunctions.Profiles
{
    public partial class AWBProfilesForm : WikiFunctions.Profiles.AWBLogUploadProfilesForm
    {
        private WikiFunctions.Browser.WebControl webBrowser;
        public event EventHandler LoadProfile;

        public AWBProfilesForm(WikiFunctions.Browser.WebControl Browser)
        {
            InitializeComponent();
            loginAsThisAccountToolStripMenuItem.Visible = true;
            loginAsThisAccountToolStripMenuItem.Click += new System.EventHandler(this.lvAccounts_DoubleClick);
            this.webBrowser = Browser;
        }

        private void browserLogin(string Password)
        {
            browserLogin(AWBProfiles.GetUsername(int.Parse(lvAccounts.Items[lvAccounts.SelectedIndices[0]].Text)), Password);
        }

        private void browserLogin(string Username, string Password)
        {
            webBrowser.Login(Username, Password);
            System.Threading.Thread.Sleep(1000); // HACK: fix this mess
            if (LoadProfile != null)
                LoadProfile(null, null);
            Variables.User.UpdateWikiStatus();
        }
        
        private void loginAsThisAccountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            login();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            login();
        }

        protected override void lvAccounts_DoubleClick(object sender, EventArgs e)
        {
            login();
        }

        /// <summary>
        /// Login based on selected item on the form
        /// </summary>
        private void login()
        {
            try
            {
                if (SelectedItem < 0) return;

                Cursor = Cursors.WaitCursor;
                if (!string.IsNullOrEmpty(lvAccounts.Items[lvAccounts.SelectedIndices[0]].SubItems[3].Text))
                    CurrentSettingsProfile = lvAccounts.Items[lvAccounts.SelectedIndices[0]].SubItems[3].Text;
                else
                    CurrentSettingsProfile = "";

                if (lvAccounts.Items[lvAccounts.SelectedIndices[0]].SubItems[2].Text == "Yes")
                {//Get 'Saved' Password
                    browserLogin(AWBProfiles.GetPassword(int.Parse(lvAccounts.Items[lvAccounts.SelectedIndices[0]].Text)));
                }
                else
                {//Get Password from User
                    UserPassword password = new UserPassword();
                    password.SetText = "Enter password for " + lvAccounts.Items[lvAccounts.SelectedIndices[0]].SubItems[1].Text;

                    if (password.ShowDialog() == DialogResult.OK)
                        browserLogin(password.GetPassword);
                }
                Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }

        /// <summary>
        /// Publically accessible login, to allow calling of login via AWB startup parameters
        /// </summary>
        /// <param name="profileID">Profile ID to login to</param>
        public void login(int profileID)
        {
            if (profileID == -1)
                return;

            try
            {
                AWBProfile startupProfile = AWBProfiles.GetProfile(profileID);

                if (!string.IsNullOrEmpty(startupProfile.Password))
                {//Get 'Saved' Password
                    browserLogin(startupProfile.Username, startupProfile.Password);
                }
                else
                {//Get Password from User
                    UserPassword password = new UserPassword();
                    password.SetText = "Enter password for " + startupProfile.Username;

                    if (password.ShowDialog() == DialogResult.OK)
                        browserLogin(startupProfile.Username, password.GetPassword);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }
    }
}

