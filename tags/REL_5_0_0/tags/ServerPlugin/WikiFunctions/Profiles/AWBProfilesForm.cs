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
using System.Windows.Forms;
using WikiFunctions.API;

namespace WikiFunctions.Profiles
{
    public partial class AWBProfilesForm : AWBLogUploadProfilesForm
    {
        private readonly IApiEdit Editor;
        public event EventHandler LoggedIn;

        public AWBProfilesForm(IApiEdit editor)
        {
            InitializeComponent();
            loginAsThisAccountToolStripMenuItem.Visible = true;
            loginAsThisAccountToolStripMenuItem.Click += lvAccounts_DoubleClick;
            Editor = editor;
        }

        private void PerformLogin(string password)
        {
            PerformLogin(AWBProfiles.GetUsername(int.Parse(lvAccounts.Items[lvAccounts.SelectedIndices[0]].Text)), password);
        }

        private void PerformLogin(string username, string password)
        {
            Editor.Login(username, password);
            if (LoggedIn != null)
                LoggedIn(null, null);
        }
        
        private void btnLogin_Click(object sender, EventArgs e)
        {
            Login();
        }

        protected override void lvAccounts_DoubleClick(object sender, EventArgs e)
        {
            Login();
        }

        /// <summary>
        /// Login based on selected item on the form
        /// </summary>
        private void Login()
        {
            try
            {
                if (SelectedItem < 0) return;

                Cursor = Cursors.WaitCursor;
                CurrentSettingsProfile =
                    string.IsNullOrEmpty(lvAccounts.Items[lvAccounts.SelectedIndices[0]].SubItems[3].Text)
                        ? ""
                        : lvAccounts.Items[lvAccounts.SelectedIndices[0]].SubItems[3].Text;

                if (lvAccounts.Items[lvAccounts.SelectedIndices[0]].SubItems[2].Text == "Yes")
                {//Get 'Saved' Password
                    PerformLogin(AWBProfiles.GetPassword(int.Parse(lvAccounts.Items[lvAccounts.SelectedIndices[0]].Text)));
                }
                else
                {//Get Password from User
                    UserPassword password = new UserPassword
                                                {
                                                    SetText =
                                                        "Enter password for " +
                                                        lvAccounts.Items[lvAccounts.SelectedIndices[0]].SubItems[1].Text
                                                };

                    if (password.ShowDialog() == DialogResult.OK)
                        PerformLogin(password.GetPassword);
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
        /// <param name="profileIdOrName">Profile ID to login to</param>
        public void Login(string profileIdOrName)
        {
            if (profileIdOrName.Length == 0)
                return;

            try
            {
                int profileID;
                AWBProfile startupProfile = int.TryParse(profileIdOrName, out profileID) ? AWBProfiles.GetProfile(profileID) : AWBProfiles.GetProfile(profileIdOrName);

                if (startupProfile == null)
                {
                    MessageBox.Show(Parent, "Can't find user '" + profileIdOrName + "'.", "Command line error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (!string.IsNullOrEmpty(startupProfile.Password))
                {//Get 'Saved' Password
                    PerformLogin(startupProfile.Username, startupProfile.Password);
                }
                else
                {//Get Password from User
                    UserPassword password = new UserPassword
                                                {
                                                    SetText = "Enter password for " + startupProfile.Username
                                                };

                    if (password.ShowDialog() == DialogResult.OK)
                        PerformLogin(startupProfile.Username, password.GetPassword);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }
    }
}

