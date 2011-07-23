/*

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
using System.IO;
using System.Windows.Forms;

namespace WikiFunctions
{
    /// <summary>
    /// This static class holds paths of directories needed by AWB
    /// </summary>
    public static class AwbDirs
    {
        private static string mAppData;

        /// <summary>
        /// Application data directory common for all users
        /// </summary>
        public static string AppData
        {
            get
            {
                if (mAppData != null) return mAppData;

                mAppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    "AutoWikiBrowser");
                Directory.CreateDirectory(mAppData);
                return mAppData;
            }
        }

        private static string mUserData;

        /// <summary>
        /// Application data directory specific to the current user
        /// </summary>
        public static string UserData
        {
            get
            {
                if (mUserData != null) return mUserData;

                mUserData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "AutoWikiBrowser");
                Directory.CreateDirectory(mUserData);
                return mUserData;
            }
        }

        private static string mDefaultSettings;

        public static string DefaultSettings
        {
            get
            {
                if (mDefaultSettings != null) return mDefaultSettings;

                mDefaultSettings = Path.Combine(UserData, "Default.xml");
                return mDefaultSettings;
            }
        }

        private static bool? mSpecialMedia;

        /// <summary>
        /// Whether we are running from a media that requires special handling
        /// </summary>
        public static bool RunningFromNetworkOrRemovableDrive
        {
            get
            {
                if (mSpecialMedia != null) return (bool)mSpecialMedia;

                string drive = Path.GetPathRoot(Application.ExecutablePath);
                if (drive.StartsWith("\\")) return true;
                DriveInfo di = new DriveInfo(drive.Substring(0, 1));
                mSpecialMedia = (di.DriveType == DriveType.Removable || di.DriveType == DriveType.Network);

                return (bool)mSpecialMedia;
            }
        }

        /// <summary>
        /// Performs silent migration from the previous scheme when we used to 
        /// </summary>
        public static void MigrateDefaultSettings()
        {
            string exeDir = Path.GetDirectoryName(Application.ExecutablePath);
            string defaultXml = Path.Combine(exeDir, "Default.xml");
            if (File.Exists(defaultXml) && !File.Exists(DefaultSettings) && !RunningFromNetworkOrRemovableDrive)
            {
                try
                {
                    File.Copy(defaultXml, DefaultSettings);
                    File.Delete(defaultXml);
                }
                catch
                { } // ignore
            }
        }
    }
}
