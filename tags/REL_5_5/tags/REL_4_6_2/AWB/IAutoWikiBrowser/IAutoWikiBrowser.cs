/*
Copyright (C) 2007 Stephen Kennedy <steve@sdk-software.com>

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
using WikiFunctions.Plugin;
using WikiFunctions.Logging;
using System.Windows.Forms;

namespace AutoWikiBrowser
{
    partial class MainForm
    {
        // Objects:
        TraceManager IAutoWikiBrowser.TraceManager { get { return Program.MyTrace; } }
        WikiFunctions.Logging.Uploader.UploadableLogSettings2 IAutoWikiBrowser.LoggingSettings { get { return Program.MyTrace.LS.Settings; } }

        bool IAutoWikiBrowser.SkipNoChanges { get { return chkSkipNoChanges.Checked; } set { chkSkipNoChanges.Checked = value; } }

        WikiFunctions.Parse.FindandReplace IAutoWikiBrowser.FindandReplace { get { return FindAndReplace; } }
        WikiFunctions.SubstTemplates IAutoWikiBrowser.SubstTemplates { get { return SubstTemplates; } }
        string IAutoWikiBrowser.CustomModule { get { return (CModule.ModuleEnabled && CModule.Module != null) ? CModule.Code : null; } }

        public event GetLogUploadLocationsEvent GetLogUploadLocations;

        /* In the (perhaps unlikely) event we need to know the name of the plugin which calls these subroutines,
         * the code is here and ready to go. */

        // Fire GetLogUploadLocations event
        internal void GetLogUploadLocationsEvent(object sender, List<WikiFunctions.Logging.Uploader.LogEntry> locations)
        {
            if (GetLogUploadLocations != null)
                GetLogUploadLocations(this, locations);
        }

        [ObsoleteAttribute]
        TabControl IAutoWikiBrowser.Tab { get { throw new NotImplementedException("Please use IAutoWikiBrowserTabs"); } }
    }
}
