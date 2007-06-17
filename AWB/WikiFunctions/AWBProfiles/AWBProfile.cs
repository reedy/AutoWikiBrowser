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

namespace WikiFunctions.AWBProfiles
{
    public class AWBProfile
    {
        public int id; // TODO: use properties
        public string defaultsettings, notes;

        protected string mUsername;
        private string mPassword; // or store in RAM encrypted

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
        private const string RegKey = "Software\\AutoWikiBrowser\\Profiles";
        private const string PassPhrase = "oi frjweopi 4r390%^($%%^$HJKJNMHJGY 2`';'[#";
        private const string Salt = "SH1ew yuhn gxe$£$%^y HNKLHWEQ JEW`b";
        private const string IV16Chars = "tnf47bgfdwlp9,.q";

        /// <summary>
        /// Encrypts a string using the specified Pass Key and Salt
        /// </summary>
        /// <param name="text">String to be encrypted</param>
        /// <returns>Encrypted String</returns>
        public static string Encrypt(string text)
        {
            return Encryption.RijndaelSimple.Encrypt(text, PassPhrase, Salt, "SHA1", 2, IV16Chars, 256);
        }

        /// <summary>
        /// Decrypts a string
        /// </summary>
        /// <param name="text">String to be decrypted</param>
        /// <returns>Decrypted String</returns>
        public static string Decrypt(string text)
        {
            return Encryption.RijndaelSimple.Decrypt(text, PassPhrase, Salt, "SHA1", 2, IV16Chars, 256);
        }

        /// <summary>
        /// Gets all the Saved Profiles from the Registry
        /// </summary>
        /// <returns>List of Profiles</returns>
        public static List<AWBProfile> GetProfiles()
        {
            Computer myComputer = new Computer();
            List<AWBProfile> profiles = new List<AWBProfile>();

            List<int> ProfileIDs = GetProfileIDs();
            foreach (int id in ProfileIDs)
            {
                try
                {
                    AWBProfile prof = new AWBProfile();
                    prof.id = id;
                    prof.Username = myComputer.Registry.GetValue("HKEY_CURRENT_USER\\" + RegKey + "\\" + id, "User", "").ToString();
                    if (prof.Username != "")
                        prof.Username = Decrypt(prof.Username);
                    prof.Password = myComputer.Registry.GetValue("HKEY_CURRENT_USER\\" + RegKey + "\\" + id, "Pass", "").ToString();
                    if (prof.Password != "")
                        prof.Password = Decrypt(prof.Password);
                    prof.defaultsettings = myComputer.Registry.GetValue("HKEY_CURRENT_USER\\" + RegKey + "\\" + id, "Settings", "").ToString();
                    prof.notes = myComputer.Registry.GetValue("HKEY_CURRENT_USER\\" + RegKey + "\\" + id, "Notes", "").ToString();

                    profiles.Add(prof);
                }
                catch { }
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
            try
            {
                Computer myComputer = new Computer();

                prof.id = id;
                prof.Username = Decrypt(myComputer.Registry.GetValue("HKEY_CURRENT_USER\\" + RegKey + "\\" + id, "User", "").ToString());
                prof.Password = Decrypt(myComputer.Registry.GetValue("HKEY_CURRENT_USER\\" + RegKey + "\\" + id, "Pass", "").ToString());
                prof.defaultsettings = myComputer.Registry.GetValue("HKEY_CURRENT_USER\\" + RegKey + "\\" + id, "Settings", "").ToString();
                prof.notes = myComputer.Registry.GetValue("HKEY_CURRENT_USER\\" + RegKey + "\\" + id, "Notes", "").ToString();

                return prof;
            }
            catch { return prof; }
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
        /// Set/Change the password of the specified profile
        /// </summary>
        /// <param name="id">Profile ID</param>
        /// <param name="password">Password</param>
        public static void SetPassword(int id, string password)
        {
            if (password != "")
                password = Encrypt(password);

            SetProfilePassword(id, password);
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
                Microsoft.Win32.RegistryKey Key = new Computer().Registry.CurrentUser;
                Key = Key.OpenSubKey(RegKey + "\\" + id, true);
                Key.SetValue("Pass", password);
            }
            catch { }
        }

        /// <summary>
        /// Adds a new Profile to the Registry
        /// </summary>
        /// <param name="profile">Profile Object of User</param>
        public static void AddProfile(AWBProfile profile)
        {
            try
            {
                int id = GetFirstFreeID();

                Microsoft.Win32.RegistryKey key =
                    new Computer().Registry.CurrentUser.CreateSubKey(RegKey + "\\" + id);

                key.SetValue("User", Encrypt(profile.Username));
                if (profile.Password != "")
                    key.SetValue("Pass", Encrypt(profile.Password));
                else
                    key.SetValue("Pass", "");
                key.SetValue("Settings", profile.defaultsettings);
                key.SetValue("Notes", profile.notes);
            }
            catch { }
        }

        /// <summary>
        /// Edits the profile
        /// </summary>
        /// <param name="profile">Profile Object of User</param>
        public static void EditProfile(AWBProfile profile)
        {
            try
            {
                Microsoft.Win32.RegistryKey Key = new Computer().Registry.CurrentUser;
                Key = Key.OpenSubKey(RegKey + "\\" + profile.id, true);

                if (profile.Password != "")
                    profile.Password = Encrypt(profile.Password);

                Key.SetValue("User", profile.Username);
                Key.SetValue("Pass", profile.Password);
                Key.SetValue("Settings", profile.defaultsettings);
                Key.SetValue("Notes", profile.notes);
            }
            catch { }
        }

        /// <summary>
        /// Deletes a specific profile from the registry
        /// </summary>
        /// <param name="id"></param>
        public static void DeleteProfile(int id)
        {
            try
            { Microsoft.Win32.Registry.CurrentUser.DeleteSubKey(RegKey + "\\" + id.ToString()); }
            catch { }
        }

        /// <summary>
        /// Counts the number of Profiles
        /// </summary>
        /// <returns>Number of Profiles</returns>
        private static int CountSubKeys()
        {
            try
            {
                Microsoft.Win32.RegistryKey baseRegistryKey = new Computer().Registry.CurrentUser;
                Microsoft.Win32.RegistryKey key2 = baseRegistryKey.OpenSubKey(RegKey);

                return key2.SubKeyCount;
            }
            catch
            { return 0; }
        }

        /// <summary>
        /// Gets a List of all the Profile IDs
        /// </summary>
        /// <returns>A list of all Profile IDs</returns>
        private static List<int> GetProfileIDs()
        {
            List<int> ProfileIDs = new List<int>();
            try
            {
                Microsoft.Win32.RegistryKey baseRegistryKey = new Computer().Registry.CurrentUser;
                Microsoft.Win32.RegistryKey key2 = baseRegistryKey.OpenSubKey(RegKey);

                string[] profid = key2.GetSubKeyNames();

                foreach (string id in profid)
                {
                    ProfileIDs.Add(int.Parse(id));
                }

                return ProfileIDs;
            }
            catch
            { return ProfileIDs; }
        }

        /// <summary>
        /// Gets the ID number of the first free Profile Slot
        /// </summary>
        /// <returns>ID Number</returns>
        private static int GetFirstFreeID()
        {
            bool FreeIDFound = false;
            List<int> IDs = GetProfileIDs();
            int i = 1;

            do
            {
                if (!IDs.Contains(i))
                    FreeIDFound = true;
                else
                    i++;
            } while (FreeIDFound == false);

            return i;
        }
    }
}
