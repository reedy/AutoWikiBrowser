/*
AWB Profiles
Copyright (C) 2007 Sam Reed, Stephen Kennedy

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
    public partial class AWBProfilesForm : Form
    {
        protected string CurrentSettingsProfile;
        private readonly Session TheSession;
        public event EventHandler LoggedIn;

        public AWBProfilesForm(Session session)
        {
            InitializeComponent();
            loginAsThisAccountToolStripMenuItem.Visible = true;
            loginAsThisAccountToolStripMenuItem.Click += lvAccounts_DoubleClick;
            btnLogin.Visible = true;
            TheSession = session;
            UsernameOrPasswordChanged(this, null);
        }

        private void AWBProfiles_Load(object sender, EventArgs e)
        {
            if (!DesignMode)
                LoadProfiles();

            string lua = AWBProfiles.LastUsedAccount;

            if (!string.IsNullOrEmpty(lua))
            {
                int id;
                int.TryParse(lua, out id);

                AWBProfile p = AWBProfiles.GetProfile(id);

                if (p == null)
                {
                    txtUsername.Text = lua;
                    return;
                }

                txtUsername.Text = (id > 0) ? p.Username : lua;
            }
        }

        /// <summary>
        /// Gets the integer of the first selected item
        /// </summary>
        protected int SelectedItem
        {
            get
            {
                try
                {
                    if (lvAccounts.SelectedIndices.Count == 0) 
                        return -1;
                    
                    return int.Parse(lvAccounts.Items[lvAccounts.SelectedIndices[0]].Text);
                }
                catch { return -1; }
            }
        }

        private void UpdateUI()
        {
            btnLogin.Enabled = btnDelete.Enabled = BtnEdit.Enabled = loginAsThisAccountToolStripMenuItem.Enabled =
                editThisAccountToolStripMenuItem.Enabled = changePasswordToolStripMenuItem.Enabled =
                deleteThisAccountToolStripMenuItem.Enabled = (lvAccounts.SelectedItems.Count > 0);
        }

        /// <summary>
        /// Loads all the profiles onto the form
        /// </summary>
        private void LoadProfiles()
        {
            lvAccounts.BeginUpdate();
            lvAccounts.Items.Clear();

            foreach (AWBProfile profile in AWBProfiles.GetProfiles())
            {
                ListViewItem item = new ListViewItem(profile.ID.ToString());
                item.SubItems.Add(profile.Username);

                item.SubItems.Add(!string.IsNullOrEmpty(profile.Password) ? "Yes" : "No");

                item.SubItems.Add(profile.DefaultSettings);
                item.SubItems.Add(profile.Notes);

                lvAccounts.Items.Add(item);
            }

            UpdateUI();
            lvAccounts.ResizeColumns();
            lvAccounts.EndUpdate();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            Add();
        }

        private void addNewAccountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Add();
        }

        private void Add()
        {
            AWBProfileAdd add = new AWBProfileAdd();
            if (add.ShowDialog() == DialogResult.Yes)
                LoadProfiles();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            Delete();
        }

        private void deleteThisSavedAccountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Delete();
        }

        private void Delete()
        {
            try
            {
                if (SelectedItem < 0) return;
                AWBProfiles.DeleteProfile(SelectedItem);
                LoadProfiles();
            }
            finally
            {
                UpdateUI();
            }
        }

        private void changePasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
			try
			{
	            UserPassword password = new UserPassword
	                                        {
	                                            Username = lvAccounts.Items[lvAccounts.SelectedIndices[0]].SubItems[1].Text
	                                        };

			    if (password.ShowDialog() == DialogResult.OK)
	                AWBProfiles.SetPassword(int.Parse(lvAccounts.Items[lvAccounts.SelectedIndices[0]].Text), password.GetPassword);
			}
			finally
			{
				LoadProfiles();
			}
        }

        private void editThisAccountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Edit();
        }

        private void Edit()
        {
            try
            {
                AWBProfileAdd add = new AWBProfileAdd(AWBProfiles.GetProfile(int.Parse(lvAccounts.Items[lvAccounts.SelectedIndices[0]].Text)));
                if (add.ShowDialog() == DialogResult.Yes)
                    LoadProfiles();
            }
            catch { }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        public string SettingsToLoad
        {
            get { return CurrentSettingsProfile; }
        }

        private void lvAccounts_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateUI();
        }

        protected virtual void lvAccounts_DoubleClick(object sender, EventArgs e)
        {
            Login();
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            Edit();
        }

        private void PerformLogin(string password)
        {
            PerformLogin(AWBProfiles.GetUsername(int.Parse(lvAccounts.Items[lvAccounts.SelectedIndices[0]].Text)), password);
        }

        private void PerformLogin(string username, string password)
        {
            if (TheSession.IsBusy)
            {
                MessageBox.Show("Cannot log in, session is busy.\r\n\r\nPlease wait for currently saving pages to complete.", "Session busy", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool needsUpdate = TheSession.User.IsLoggedIn;
            try
            {
                TheSession.Editor.SynchronousEditor.Login(username, password, Variables.LoginDomain);
                needsUpdate = true;
            }
            catch (LoginException ex)
            {
                MessageBox.Show(this, ex.Message, "Login failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }

            if (LoggedIn != null && needsUpdate)
                LoggedIn(null, null);

            if (TheSession.User.IsLoggedIn) Close();
        }

        private void btnLogin_Click(object sender, EventArgs e)
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
                {
                    //Get 'Saved' Password
                    PerformLogin(AWBProfiles.GetPassword(int.Parse(lvAccounts.Items[lvAccounts.SelectedIndices[0]].Text)));
                }
                else
                {
                    //Get Password from User
                    UserPassword password = new UserPassword
                    {
                        Username = lvAccounts.Items[lvAccounts.SelectedIndices[0]].SubItems[1].Text
                    };

                    if (password.ShowDialog(this) == DialogResult.OK)
                        PerformLogin(password.GetPassword);
                }

                AWBProfiles.LastUsedAccount = lvAccounts.Items[lvAccounts.SelectedIndices[0]].Text;

                Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
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
                        Username = startupProfile.Username
                    };

                    if (password.ShowDialog(this) == DialogResult.OK)
                        PerformLogin(startupProfile.Username, password.GetPassword);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }

        private void UsernameOrPasswordChanged(object sender, EventArgs e)
        {
            btnQuickLogin.Enabled = txtPassword.Text.Length > 0 && txtUsername.Text.Length > 0;
        }

        private void txtPassword_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Return || e.KeyCode == Keys.Enter) && btnQuickLogin.Enabled)
                btnQuickLogin.PerformClick();
        }

        private void btnQuickLogin_Click(object sender, EventArgs e)
        {
            string user = txtUsername.Text;
            string password = txtPassword.Text;

            if (chkSaveProfile.Checked)
            {
                if (AWBProfiles.GetProfile(txtUsername.Text) != null)
                {
                    MessageBox.Show("Username \"" + txtUsername.Text + "\" already exists.", "Username exists");
                    return;
                }

                var profile = new AWBProfile { Username = user };
                if (chkSavePassword.Checked) profile.Password = password;
                AWBProfiles.AddEditProfile(profile);
            }

            AWBProfiles.LastUsedAccount = user;
            PerformLogin(user, password);
        }

        private void chkSaveProfile_CheckedChanged(object sender, EventArgs e)
        {
            chkSavePassword.Enabled = chkSaveProfile.Checked;
        }

    }
}
