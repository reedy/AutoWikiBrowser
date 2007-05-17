using System;
using System.Collections.Generic;
using System.Text;
using WikiFunctions.Logging;

// temp this part of LED control, which should go into WikiFunctions
enum Colour
{
    blue
}

// temp: this should be built into the Logging tab control
namespace AutoWikiBrowser.Logging
{
    internal sealed class LoggingSettings
    {
        internal bool Initialised
        { get { return true; } set { } }

        internal Colour LEDColour
        { get { return Colour.blue; } set { } }
    }
}
