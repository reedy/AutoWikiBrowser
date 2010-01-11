/*
(C) 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/

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
using System.Windows.Forms;
using WikiFunctions.Plugin;
using WikiFunctions.AWBSettings;

namespace AutoWikiBrowser.Plugins.Server
{
    /// <summary>
    /// AWB Server Plugin main object which implements IAWBPlugin
    /// </summary>
    public sealed class ServerAWBPlugin : IAWBPlugin
    {
        internal const string ME = "Server Plugin";

        // Tab page:
        internal static TabPage ServerPluginTabPage = new TabPage("Server");
        internal static ServerControl ServerUserControl;

        #region IAWBPlugin Members

        void IAWBPlugin.Initialise(IAutoWikiBrowser sender)
        {
            if (sender == null)
                throw new ArgumentNullException("sender");

            // Delegate UI-setup to our user control object:
            ServerUserControl = new ServerControl(sender);

            // Set up the TabPage and attach the user control to it:
            ServerPluginTabPage.UseVisualStyleBackColor = true;
            ServerPluginTabPage.Controls.Add(ServerUserControl);
        }

        public string Name
        { get { return "AWB " + ME; } }

        string IAWBPlugin.WikiName
        { get { return Name; } }

        string IAWBPlugin.ProcessArticle(IAutoWikiBrowser sender, ProcessArticleEventArgs eventargs)
        {
            return eventargs.ArticleText;
        }

        void IAWBPlugin.LoadSettings(object[] prefs)
        {
            if (prefs != null && prefs[0] is PrefsKeyPair)
            {
                PrefsKeyPair pref = (prefs[0] as PrefsKeyPair);
                if ((pref != null) && pref.Name == "Enabled")
                    ServerUserControl.PluginEnabled = (bool)pref.Setting;
            }
        }

        object[] IAWBPlugin.SaveSettings()
        { return new object[] { new PrefsKeyPair("Enabled", ServerUserControl.PluginEnabled) }; }

        void IAWBPlugin.Reset()
        { ServerUserControl.PluginEnabled = false; }

        void IAWBPlugin.Nudge(out bool Cancel)
        { Cancel = false; }

        void IAWBPlugin.Nudged(int Nudges) {}

        #endregion
    }
}
