/*
AWB Profiles
Copyright (C) 2008 Sam Reed

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


namespace WikiFunctions.Profiles
{
    public partial class AWBProfileAdd : Form
    {
        private readonly int Editid;

        public AWBProfileAdd()
        {
            InitializeComponent();
            Editid = -1;

            Text = "Add new Profile";
        }

        /// <summary>
        /// Loads details from an AWBProfile object for editing
        /// </summary>
        public AWBProfileAdd(AWBProfile profile)
        {
            InitializeComponent();

            Editid = profile.ID;

            txtUsername.Text = profile.Username;
            txtPassword.Text = profile.Password;
            txtPath.Text = profile.DefaultSettings;
            txtNotes.Text = profile.Notes;

            if (!string.IsNullOrEmpty(txtPath.Text))
                chkDefaultSettings.Checked = true;

            if (!string.IsNullOrEmpty(txtPassword.Text))
                chkSavePassword.Checked = true;

            Text = "Edit Profile";
        }

        private void chkSavePassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.Enabled = chkSavePassword.Checked;
        }

        private void AWBProfileAdd_Load(object sender, EventArgs e)
        {
            openDefaultFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        private void chkDefaultSettings_CheckedChanged(object sender, EventArgs e)
        {
            txtPath.Enabled = chkDefaultSettings.Checked;
            btnBrowse.Enabled = chkDefaultSettings.Checked;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (chkDefaultSettings.Checked && (openDefaultFile.ShowDialog() == DialogResult.OK))
                txtPath.Text = openDefaultFile.FileName;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUsername.Text))
                MessageBox.Show("The Username cannot be blank");
            else
            {
            	// warn user if profile for entered user ID already exists
                if (Editid == -1 && AWBProfiles.GetProfile(txtUsername.Text) != null)
                {
                	if (MessageBox.Show("Username \"" +txtUsername.Text + "\" is already used in another profile. Are you sure you want to use this username again?", 
                	                    "Username already used in another profile", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                	return;
                }

                AWBProfile profile = new AWBProfile {Username = txtUsername.Text};

                if (chkSavePassword.Checked && !string.IsNullOrEmpty(txtPassword.Text))
                    profile.Password = txtPassword.Text;
                else
                	 profile.Password ="";

                profile.DefaultSettings = txtPath.Text;
                profile.Notes = txtNotes.Text;

                profile.ID = Editid;
                AWBProfiles.AddEditProfile(profile);

                DialogResult = DialogResult.Yes;
            }
        }
    }
}