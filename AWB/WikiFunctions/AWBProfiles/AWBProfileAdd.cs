/*
AWB Profiles
Copyright (C) 2007 Sam Reed

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

using System.IO;

namespace WikiFunctions.AWBProfiles
{
    public partial class AWBProfileAdd : Form
    {
        //AWBProfile AWBProfile = new AWBProfile();
        private int editid;

        public AWBProfileAdd()
        {
            InitializeComponent();
            editid = -1;

            this.Text = "Add new Profile";
        }

        /// <summary>
        /// Loads details from an AWBProfile object for editing
        /// </summary>
        public AWBProfileAdd(AWBProfile profile)
        {
            InitializeComponent();
            editid = profile.id;

            txtUsername.Text = AWBProfiles.Decrypt(profile.Username);
            txtPassword.Text = AWBProfiles.Decrypt(profile.Password);
            txtPath.Text = profile.defaultsettings;
            txtNotes.Text = profile.notes;

            this.Text = "Edit Profile";
        }

        private void chkSavePassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.Enabled = chkSavePassword.Checked;
        }

        private void AWBProfileAdd_Load(object sender, EventArgs e)
        {
            openDefaultFile.InitialDirectory = Application.StartupPath;
        }

        private void chkDefaultSettings_CheckedChanged(object sender, EventArgs e)
        {
            txtPath.Enabled = chkDefaultSettings.Checked;
            if (chkDefaultSettings.Checked)
            {
                if (openDefaultFile.ShowDialog() == DialogResult.OK)
                    txtPath.Text = openDefaultFile.FileName;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtUsername.Text == "")
                MessageBox.Show("The Username cannot be blank");
            else
            {
                AWBProfile profile = new AWBProfile();

                profile.Username = txtUsername.Text;
                profile.Password = txtPassword.Text;
                profile.defaultsettings = txtPath.Text;
                profile.notes = txtNotes.Text;

                if (editid == -1)
                    AWBProfiles.AddProfile(profile);
                else
                {
                    profile.id = editid;
                    AWBProfiles.EditProfile(profile);
                }

                this.DialogResult = DialogResult.Yes;
            }
        }
    }
}