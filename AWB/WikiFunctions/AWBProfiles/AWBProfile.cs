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
using System.Text;
using Microsoft.VisualBasic.Devices;
using System.Windows.Forms;

namespace WikiFunctions.Profiles
{
    public class AWBProfile
    {
        public int id;
        public string defaultsettings, notes;

        private string mUsername = "";
        private string mPassword = "";

        public bool useforupload;

        public string Username
        {
            get { return mUsername; }
            set { mUsername = value; }
        }

        public string Password
        {
            get { return mPassword; }
            set { mPassword = value; }
        }
    }

    public static class AWBProfiles
    {
        private const string RegKey = "Software\\Wikipedia\\AutoWikiBrowser\\Profiles";
        private const string PassPhrase = "oi frjweopi 4r390%^($%%^$HJKJNMHJGY 2`';'[#";
        private const string Salt = "SH1ew yuhn gxe$£$%^y HNKLHWEQ JEW`b";
        private const string IV16Chars = "tnf47bgfdwlp9,.q";

        /// <summary>
        /// Encrypts a string using the specified Pass Key and Salt
        /// </summary>
        /// <param name="text">String to be encrypted</param>
        /// <returns>Encrypted String</returns>
        private static string Encrypt(string text)
        {
            try
            {
                if (!string.IsNullOrEmpty(text))
                    return Encryption.RijndaelSimple.Encrypt(text, PassPhrase, Salt, "SHA1", 2, IV16Chars, 256);
                else
                    return text;
            }
            catch { return text; }
        }

        /// <summary>
        /// Decrypts a string
        /// </summary>
        /// <param name="text">String to be decrypted</param>
        /// <returns>Decrypted String</returns>
        private static string Decrypt(string text)
        {
            try
            {
                if (!string.IsNullOrEmpty(text))
                    return Encryption.RijndaelSimple.Decrypt(text, PassPhrase, Salt, "SHA1", 2, IV16Chars, 256);
                else
                    return text;
            }
            catch { return text; }
        }

        /// <summary>
        /// Gets all the Saved Profiles from the Registry
        /// </summary>
        /// <returns>List of Profiles</returns>
        public static List<AWBProfile> GetProfiles()
        {
            List<AWBProfile> profiles = new List<AWBProfile>();
            foreach (int id in GetProfileIDs())
            {
                profiles.Add(GetProfile(id));
            }
            return profiles;
        }

        /// <summary>
        /// Gets a Specified Profile from the Registry
        /// </summary>
        /// <param name="id">Profile ID to get</param>
        /// <returns>Specified Profile</returns>
        public static AWBProfile GetProfile(int id)
        {
            AWBProfile prof = new AWBProfile();
            Computer myComputer = new Computer();

            prof.id = id;
            try { prof.Username = Decrypt(myComputer.Registry.GetValue("HKEY_CURRENT_USER\\" + RegKey + "\\" + id, "User", "").ToString()); }
            catch
            {
                if (MessageBox.Show("Profile corrupt. Would you like to delete this profile?", "Delete corrupt profile?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    DeleteProfile(id);

                throw new Exception("");
            }

            // one try...catch without a resume has the effect that all remaining code in the try block is skipped
            // WHY are we just ignoring these errors anyway? There should be a wrapper around Registry.GetValue perhaps?
            try { prof.Password = Decrypt(myComputer.Registry.GetValue("HKEY_CURRENT_USER\\" + RegKey + "\\" + id, "Pass", "").ToString()); }
            catch { prof.Password = ""; }
            finally
            {
                prof.defaultsettings = myComputer.Registry.GetValue("HKEY_CURRENT_USER\\" + RegKey + "\\" + id, "Settings", "").ToString();
                try { prof.useforupload = bool.Parse(myComputer.Registry.GetValue("HKEY_CURRENT_USER\\" + RegKey + "\\" + id, "UseForUpload", "").ToString()); }
                catch { prof.useforupload = false; }
                prof.notes = myComputer.Registry.GetValue("HKEY_CURRENT_USER\\" + RegKey + "\\" + id, "Notes", "").ToString();
            }
            return prof;
        }

        /// <summary>
        /// Return (or create and return) an AWBProfile for the account used for log uploading
        /// </summary>
        /// <returns>The Profile. Throw an error or return null if the user declines to create a profile?</returns>
        public static AWBProfile GetProfileForLogUploading(IWin32Window owner)
        {
            int idOfUploadAccount = GetIDOfUploadAccount();
            AWBProfile retval;
            AWBLogUploadProfilesForm profiles;

            if (idOfUploadAccount == -1)
            {
                if (MessageBox.Show("Please select or add a Profile to use for log uploading",
                    "Log uploading", MessageBoxButtons.OKCancel, MessageBoxIcon.Information)
                    == DialogResult.OK)
                {
                    profiles = new AWBLogUploadProfilesForm();
                    profiles.ShowDialog(owner);
                    retval = GetProfileForLogUploading(owner);
                }
                else
                    throw new System.Configuration.ConfigurationErrorsException("Log upload profile: User cancelled");
            }
            else
                retval = GetProfile(idOfUploadAccount);

            if (string.IsNullOrEmpty(retval.Password) && string.IsNullOrEmpty(TempPassword))
            {
                WikiFunctions.Profiles.UserPassword password = new WikiFunctions.Profiles.UserPassword();
                password.SetText = "Enter password for " + retval.Username;
                if (password.ShowDialog() == DialogResult.OK)
                {
                    retval.Password = TempPassword = password.GetPassword;
                }
            }
            else if (!string.IsNullOrEmpty(TempPassword))
                retval.Password = TempPassword;
            return retval;
        }

        /// <summary>
        /// Returns the ID of the account set to be used to upload logs
        /// </summary>
        /// <returns>-1 if no Upload Profile found</returns>
        public static int GetIDOfUploadAccount()
        {
            foreach (AWBProfile prof in GetProfiles())
                if (prof.useforupload)
                    return prof.id;
            return -1;
        }

        /// <summary>
        /// Sets all current accounts as not for upload, so the new account can be the upload account
        /// </summary>
        internal static void SetOtherAccountsAsNotForUpload()
        {
            try
            {
                AWBProfiles.ResetTempPassword();
                foreach (int id in GetProfileIDs())
                {
                    Microsoft.Win32.RegistryKey key = new Computer().Registry.CurrentUser.OpenSubKey(RegKey + "\\" + id, true);
                    key.SetValue("UseForUpload", false);
                }
            }
            catch { }
        }

        /// <summary>
        /// Gets the decrypted password of a specified profile
        /// </summary>
        /// <param name="id">Profile ID</param>
        /// <returns>Decrypted password</returns>
        public static string GetPassword(int id)
        {
            return Decrypt(new Computer().Registry.GetValue("HKEY_CURRENT_USER\\" + RegKey + "\\" + id, "Pass", "").ToString());
        }

        /// <summary>
        /// Gets the decrypted username of a specified profile
        /// </summary>
        /// <param name="id">Profile ID</param>
        /// <returns>Decrypted password</returns>
        public static string GetUsername(int id)
        {
            return Decrypt(new Computer().Registry.GetValue("HKEY_CURRENT_USER\\" + RegKey + "\\" + id, "User", "").ToString());
        }

        /// <summary>
        /// Set/Change the password of the specified profile
        /// </summary>
        /// <param name="id">Profile ID</param>
        /// <param name="password">Password</param>
        public static void SetPassword(int id, string password)
        {
            SetProfilePassword(id, Encrypt(password));
        }

        /// <summary>
        /// Sets the Profile Password in the Registry
        /// </summary>
        /// <param name="id">Profile ID</param>
        /// <param name="password">Password</param>
        private static void SetProfilePassword(int id, string password)
        {
            try
            {
                Microsoft.Win32.RegistryKey key = new Computer().Registry.CurrentUser.OpenSubKey(RegKey + "\\" + id, true);
                key.SetValue("Pass", password);
            }
            catch { }
        }

        /// <summary>
        /// Adds a new Profile to the Registry
        /// </summary>
        /// <param name="profile">Profile Object of User</param>
        public static void AddProfile(AWBProfile profile)
        {
            try { AddEditProfile(profile, new Computer().Registry.CurrentUser.CreateSubKey(RegKey + "\\" + GetFirstFreeID())); }
            catch { }
        }

        /// <summary>
        /// Edits the profile
        /// </summary>
        /// <param name="profile">Profile Object of User</param>
        public static void EditProfile(AWBProfile profile)
        {
            try { AddEditProfile(profile, new Computer().Registry.CurrentUser.OpenSubKey(RegKey + "\\" + profile.id, true)); }
            catch { }
        }

        /// <summary>
        /// Does the registry writing for add & edit profiles
        /// </summary>
        /// <param name="profile">Profile Object of User</param>
        /// <param name="Key">Registry Key for Adding/Editing</param>
        private static void AddEditProfile(AWBProfile profile, Microsoft.Win32.RegistryKey Key)
        {
            Key.SetValue("User", Encrypt(profile.Username));
            Key.SetValue("Pass", Encrypt(profile.Password));
            Key.SetValue("Settings", profile.defaultsettings);
            Key.SetValue("UseForUpload", profile.useforupload);
            Key.SetValue("Notes", profile.notes);
        }

        /// <summary>
        /// Gets/Returns the temporary password set by the user
        /// </summary>
        private static string TempPassword
        {
            get
            {
                try { return Decrypt(new Computer().Registry.GetValue("HKEY_CURRENT_USER\\" + RegKey, "TempPassword", "").ToString()); }
                catch { return ""; }
            }

            set
            {
                try
                {
                    Microsoft.Win32.RegistryKey key = new Computer().Registry.CurrentUser.OpenSubKey(RegKey, true);
                    if (key != null) key.SetValue("TempPassword", Encrypt(value));
                }
                catch { }
            }
        }

        /// <summary>
        /// Sets the temporary password to ""
        /// </summary>
        public static void ResetTempPassword()
        {
            TempPassword = "";
        }

        /// <summary>
        /// Deletes a specific profile from the registry
        /// </summary>
        /// <param name="id"></param>
        public static void DeleteProfile(int id)
        {
            try { Microsoft.Win32.Registry.CurrentUser.DeleteSubKey(RegKey + "\\" + id.ToString()); }
            catch { }
        }

        /// <summary>
        /// Counts the number of Profiles
        /// </summary>
        /// <returns>Number of Profiles</returns>
        private static int CountSubKeys()
        {
            try { return new Computer().Registry.CurrentUser.OpenSubKey(RegKey).SubKeyCount; }
            catch { return 0; }
        }

        /// <summary>
        /// Gets a List of all the Profile IDs
        /// </summary>
        /// <returns>A list of all Profile IDs</returns>
        private static List<int> GetProfileIDs()
        {
            List<int> profileIds = new List<int>();
            try
            {
                foreach (string id in new Computer().Registry.CurrentUser.OpenSubKey(RegKey).GetSubKeyNames())
                    profileIds.Add(int.Parse(id));

                return profileIds;
            }
            catch
            { return profileIds; }
        }

        /// <summary>
        /// Gets the ID number of the first free Profile Slot
        /// </summary>
        /// <returns>ID Number</returns>
        private static int GetFirstFreeID()
        {
            bool freeIdFound = false;
            List<int> ids = GetProfileIDs();
            int i = 1;

            do
            {
                if (!ids.Contains(i))
                    freeIdFound = true;
                else
                    i++;
            } while (!freeIdFound);

            return i;
        }
    }
}
