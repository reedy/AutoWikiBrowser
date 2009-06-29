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

namespace WikiFunctions.Profiles
{
    public partial class AWBLogUploadProfilesForm : Form
    {
        protected string CurrentSettingsProfile;

        public AWBLogUploadProfilesForm()
        {
            InitializeComponent();
        }

        private void AWBProfiles_Load(object sender, EventArgs e)
        {
            LoadProfiles();
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
                if (!string.IsNullOrEmpty(profile.Password))
                    item.SubItems.Add("Yes");
                else
                    item.SubItems.Add("No");
                item.SubItems.Add(profile.DefaultSettings);
                if (profile.UseForUpload)
                    item.SubItems.Add("Yes");
                else
                    item.SubItems.Add("No");
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
	                                            SetText =
	                                                "Set password for: " +
	                                                lvAccounts.Items[lvAccounts.SelectedIndices[0]].SubItems[1].Text
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
            Edit();
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            Edit();
        }
    }
}
