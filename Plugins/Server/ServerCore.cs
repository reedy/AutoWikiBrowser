using System;
using System.Collections.Generic;
using System.Text;
using WikiFunctions;
using WikiFunctions.Plugin;

[assembly: CLSCompliant(true)]
namespace AutoWikiBrowser.Plugins.Server
{
    public sealed class ServerCore : IAWBPlugin
    {
        #region IAWBPlugin Members

        void IAWBPlugin.Initialise(IAutoWikiBrowser sender)
        {
        }

        public string Name
        {
            get { return "AWB Server Plugin"; }
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
