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

        /// <summary>
        /// Whether we are running from a media that requires special handling
        /// </summary>
        public static bool RunningFromNetworkOrRemovableDrive
        {
            get
            {
                string drive = Path.GetPathRoot(Application.ExecutablePath);
                if (drive.StartsWith("\\")) return true;
                DriveInfo di = new DriveInfo(drive.Substring(0, 1));
                return di.DriveType == DriveType.Removable || di.DriveType == DriveType.Network;
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
