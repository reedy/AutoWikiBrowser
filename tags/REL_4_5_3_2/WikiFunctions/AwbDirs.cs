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

                mAppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "AutoWikiBrowser");
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

                mUserData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AutoWikiBrowser");
                Directory.CreateDirectory(mUserData);
                return mUserData;
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
                DriveInfo di = new DriveInfo("" + drive[0]);
                return di.DriveType == DriveType.Removable || di.DriveType == DriveType.Network;
            }
        }

        //public static void MigrateDefaultSettings()
        //{
        //    if (File.Exists("Default.xml") && !RunningFromNetworkOrRemovableDrive)
        //    {
        //        if (MessageBox.Show(""))) ; //
        //    }
        //}
    }
}
