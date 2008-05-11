using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using WikiFunctions;
using WikiFunctions.Plugin;

[assembly: CLSCompliant(true)]
namespace AutoWikiBrowser.Plugins.Server
{
    /// <summary>
    /// AWB Server Plugin main object which implements IAWBPlugin
    /// </summary>
    public sealed class ServerAWBPlugin : IAWBPlugin
    {
        private const string conMe = "AWB Server Plugin";

        // AWB objects:
        internal static IAutoWikiBrowser AWBForm;
        internal static ToolStripStatusLabel StatusText = new ToolStripStatusLabel("Server Plugin Loaded");

        // Menu item:
        internal static ToolStripMenuItem OurMenuItem = new ToolStripMenuItem("Server Plugin");

        #region IAWBPlugin Members

        void IAWBPlugin.Initialise(IAutoWikiBrowser sender)
        {
        }

        public string Name
        {
            get { return conMe; }
        }

        string IAWBPlugin.WikiName
        {
            get { return Name; }
        }

        string IAWBPlugin.ProcessArticle(IAutoWikiBrowser sender, ProcessArticleEventArgs eventargs)
        {
            return "";
        }

        void IAWBPlugin.LoadSettings(object[] prefs)
        {
        }

        object[] IAWBPlugin.SaveSettings()
        {
            return null;
        }

        void IAWBPlugin.Reset()
        {            
        }

        void IAWBPlugin.Nudge(out bool Cancel)
        {
            Cancel = false;
        }

        void IAWBPlugin.Nudged(int Nudges)
        {            
        }

        #endregion
    }
}
