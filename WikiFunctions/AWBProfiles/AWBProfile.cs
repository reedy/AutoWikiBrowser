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

        public static string Encrypt(string text)
        {
            return Encryption.RijndaelSimple.Encrypt(text, PassPhrase, Salt, "SHA1", 2, IV16Chars, 256);
        }

        public static string Decrypt(string text)
        {
            return Encryption.RijndaelSimple.Decrypt(text, PassPhrase, Salt, "SHA1", 2, IV16Chars, 256);
        }

        public static List<AWBProfile> GetProfiles()
        {
            Computer myComputer = new Computer();
            List<AWBProfile> profiles = new List<AWBProfile>();

            int upper = CountSubKeys();

            for (int i = 1; i <= upper; i++)
            {
                try
                {
                    AWBProfile prof = new AWBProfile();
                    prof.id = i;
                    prof.Username = Decrypt(myComputer.Registry.GetValue("HKEY_CURRENT_USER\\" + RegKey + "\\" + i, "User", "").ToString());
                    prof.Password = Decrypt(myComputer.Registry.GetValue("HKEY_CURRENT_USER\\" + RegKey + "\\" + i, "Pass", "").ToString());
                    prof.defaultsettings = myComputer.Registry.GetValue("HKEY_CURRENT_USER\\" + RegKey + "\\" + i, "Settings", "").ToString();
                    prof.notes = myComputer.Registry.GetValue("HKEY_CURRENT_USER\\" + RegKey + "\\" + i, "Notes", "").ToString();

                    profiles.Add(prof);
                }
                catch { }
            }
            return profiles;
        }

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

        public static string GetPassword(int id)
        {
            return Decrypt(new Computer().Registry.GetValue("HKEY_CURRENT_USER\\" + RegKey + "\\" + id, "Pass", "").ToString());
        }

        public static void SetPassword(int id, string password)
        {
            if (password != "")
                password = Encrypt(password);

            SetProfilePassword(id, password);
        }

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

        public static void AddProfile(AWBProfile profile)
        {
            try
            {
                Microsoft.Win32.RegistryKey key =
                    new Computer().Registry.CurrentUser.CreateSubKey(RegKey + "\\" + (CountSubKeys() + 1));

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

        public static void DeleteProfile(int id)
        {
            try
            { Microsoft.Win32.Registry.CurrentUser.DeleteSubKey(RegKey + "\\" + id.ToString()); }
            catch { }
        }

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
    }
}
