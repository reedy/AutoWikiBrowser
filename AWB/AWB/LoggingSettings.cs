using System;
using System.Collections.Generic;
using System.Text;
using WikiFunctions.Logging;
using WikiFunctions.Logging.Uploader;

// temp: this should be built into the Logging tab control
namespace AutoWikiBrowser.Logging
{
    internal sealed class LoggingSettings
    {
        // placeholders
        internal bool Initialised
        { get { return true; } set { } }

        internal WikiFunctions.Controls.Colour LEDColour
        { get { return WikiFunctions.Controls.Colour.Blue; } set { } }

        internal sealed class Props : UploadableLogSettings2
        {
            internal Props()
            {
                base.mUploadLocation = "$USER/Logs";
                base.mUploadJobName = "$CATEGORY";
            }

            internal bool Equals(Props Compare)
            {
                return (((((Operators.CompareString(Compare.LogFolder, this.LogFolder, false) == 0) && 
                    (Compare.LogVerbose == this.LogVerbose)) && ((Compare.LogWiki == this.LogWiki) && 
                    (Compare.LogXHTML == this.LogXHTML))) && 
                    (((Compare.UploadAddToWatchlist == this.UploadAddToWatchlist) && 
                    (Operators.CompareString(Compare.UploadJobName, this.UploadJobName, false) == 0)) && 
                    ((Operators.CompareString(Compare.UploadLocation, this.UploadLocation, false) == 0) && 
                    (Compare.UploadMaxLines == this.UploadMaxLines)))) && 
                    (((Compare.UploadOpenInBrowser == this.UploadOpenInBrowser) && 
                    (Compare.UploadToWikiProjects == this.UploadToWikiProjects)) && 
                    ((Compare.UploadYN == this.UploadYN) && 
                    (Operators.CompareString(Compare.Category, this.Category, false) == 0))))               
            }
        }
    }
}
