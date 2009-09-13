using System;
using System.Collections.Generic;
using System.Text;

namespace WikiFunctions
{
    /// <summary>
    /// Holds some deepest-level things to be initialised prior to most other static classes,
    /// including Variables
    /// </summary>
    public static class Globals
    {
        /// <summary>
        /// Whether we are running under Windows
        /// </summary>
        public static bool RunningOnWindows
        { get { return windows; } }

        static readonly bool windows = Environment.OSVersion.VersionString.Contains("Windows");

        #region Unit tests support
        /// <summary>
        /// Set this to true in unit tests, to disable checkpage loading and other slow stuff.
        /// This disables some functions, however.
        /// </summary>
        public static bool UnitTestMode;

        /// <summary>
        /// 
        /// </summary>
        public static int UnitTestIntValue;

        /// <summary>
        /// 
        /// </summary>
        public static bool UnitTestBoolValue;
        #endregion
    }
}
