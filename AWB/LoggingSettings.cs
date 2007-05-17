using System;
using System.Collections.Generic;
using System.Text;
using WikiFunctions.Logging;

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
    }
}
