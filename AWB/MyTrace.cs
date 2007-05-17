using System;
using System.Collections.Generic;
using System.Text;
using WikiFunctions.Logging;
using System.Windows.Forms;

namespace AutoWikiBrowser.Logging
{
    internal sealed class MyTrace : TraceManager
    {
        private const string conWiki = "Wiki";

        private bool mIsUploading = false;
        private LoggingSettings LoggingSettings;
        private static string LogFolder = "";

        internal void Initialise()
        {
            LoggingSettings.Initialised = true;


        }

        internal bool HaveOpenFile
        { get { return (Listeners.Count > 0); } }

        protected override string ApplicationName
        {
            get { return "AutoWikiBrowser logging manager"; }
        }

        protected override bool StartingUpload(TraceListenerUploadableBase Sender)
        {
            if (Sender.TraceStatus.LogName != conWiki)
                return false;

            mIsUploading = true;
            LoggingSettings.LEDColour = WikiFunctions.Controls.Colour.Blue;
            Application.DoEvents();
            return true;
        }
    }
}
