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
        private readonly bool JustLoading;

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
            JustLoading = true;

            Editid = profile.ID;

            txtUsername.Text = profile.Username;
            txtPassword.Text = profile.Password;
            txtPath.Text = profile.DefaultSettings;
            chkUseForUpload.Checked = profile.UseForUpload;
            txtNotes.Text = profile.Notes;

            if (!string.IsNullOrEmpty(txtPath.Text))
                chkDefaultSettings.Checked = true;

            if (!string.IsNullOrEmpty(txtPassword.Text))
                chkSavePassword.Checked = true;

            Text = "Edit Profile";
            JustLoading = false;
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
            if (!JustLoading && chkDefaultSettings.Checked && (openDefaultFile.ShowDialog() == DialogResult.OK))
                txtPath.Text = openDefaultFile.FileName;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUsername.Text))
                MessageBox.Show("The Username cannot be blank");
            else
            {
                AWBProfile profile = new AWBProfile {Username = txtUsername.Text};

                if (chkSavePassword.Checked && !string.IsNullOrEmpty(txtPassword.Text))
                    profile.Password = txtPassword.Text;

                profile.DefaultSettings = txtPath.Text;

                int idUpload = AWBProfiles.GetIDOfUploadAccount();

                if (chkUseForUpload.Checked && (idUpload != -1) && (idUpload != Editid))
                    AWBProfiles.SetOtherAccountsAsNotForUpload();

                profile.UseForUpload = chkUseForUpload.Checked;
                profile.Notes = txtNotes.Text;

                profile.ID = Editid;
                AWBProfiles.AddEditProfile(profile);

                DialogResult = DialogResult.Yes;
            }
        }
    }
}