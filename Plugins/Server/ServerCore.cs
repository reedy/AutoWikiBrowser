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
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using WikiFunctions;
using WikiFunctions.Plugin;
using WikiFunctions.AWBSettings;

namespace AutoWikiBrowser.Plugins.Server
{
    /// <summary>
    /// AWB Server Plugin main object which implements IAWBPlugin
    /// </summary>
    public sealed class ServerAWBPlugin : IAWBPlugin
    {
        private const string conMe = "Server Plugin";

        // AWB objects:
        internal static IAutoWikiBrowser AWBForm;
        internal static ToolStripStatusLabel StatusText = new ToolStripStatusLabel(conMe + " loaded");

        // Menu items:
        internal static ToolStripMenuItem EnabledMenuItem = new ToolStripMenuItem(conMe);
        internal static ToolStripMenuItem ConfigMenuItem = new ToolStripMenuItem("Configuration");
        internal static ToolStripMenuItem AboutMenuItem = new ToolStripMenuItem("About the AWB " + conMe);

        // Tab page:
        internal static TabPage ServerPluginTabPage = new TabPage("Server");
        private static ServerControl ServerUserControl;

        // Settings:
        internal static Settings ServerSettings = new Settings();

        #region IAWBPlugin Members

        void IAWBPlugin.Initialise(IAutoWikiBrowser sender)
        {
            if (sender == null)
                throw new ArgumentNullException("sender");

            // Store AWB object reference:
            AWBForm = sender;

            // Initialise our settings object:
            ServerUserControl = new ServerControl();

            // Set up our UI objects:
            StatusText.Visible = false;
            StatusText.Margin = new Padding(10, 0, 10, 0);
            StatusText.BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Right;
            StatusText.BorderStyle = Border3DStyle.Etched;
            //AWBForm.StatusStrip.ShowItemToolTips = true; // naughty hack in case somebody turns this off in the designer
            EnabledMenuItem.CheckOnClick = true;
            ServerPluginTabPage.UseVisualStyleBackColor = true;
            ServerPluginTabPage.Controls.Add(ServerUserControl);

            // Event handlers:
            AboutMenuItem.Click += AboutMenuItemClicked;
            EnabledMenuItem.CheckedChanged += PluginEnabledCheckedChange;
            ConfigMenuItem.Click += ShowSettings;
            ServerUserControl.HideButton.Click += HideButton_Click;
            ServerUserControl.SettingsButton.Click += ShowSettings;

            // Add our UI objects to the AWB main form:
            AWBForm.StatusStrip.Items.Insert(2, StatusText);
            EnabledMenuItem.DropDownItems.Add(ConfigMenuItem);
            AWBForm.PluginsToolStripMenuItem.DropDownItems.Add(EnabledMenuItem);
            AWBForm.HelpToolStripMenuItem.DropDownItems.Add(AboutMenuItem);
            AWBForm.HelpToolStripMenuItem.DropDownItems.Add(AboutMenuItem);
        }

        public string Name
        { get { return "AWB " + conMe; } }

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
                PrefsKeyPair pref = prefs[0] as PrefsKeyPair;
                if (pref.Name == "Enabled")
                    PluginEnabled = (bool)pref.Setting;
            }
        }

        object[] IAWBPlugin.SaveSettings()
        { return new object[] { new PrefsKeyPair("Enabled", PluginEnabled) }; }

        void IAWBPlugin.Reset()
        { EnabledMenuItem.Checked = false; }

        void IAWBPlugin.Nudge(out bool Cancel)
        { Cancel = false; }

        void IAWBPlugin.Nudged(int Nudges) {}

        #endregion

        // Properties
        private bool PluginEnabled
        {
            get { return EnabledMenuItem.Checked; }
            set
            {
                if (value != EnabledMenuItem.Checked) // prevent the event from firing unless there's a change
                    EnabledMenuItem.Checked = value;
            }
        }

        // Event handlers
        private static void AboutMenuItemClicked(Object sender, EventArgs e)
        { new ServerAboutBox().Show(); }

        /// <summary>
        /// Show the settings form; if Server Enabled status changes toggle the menu item which will
        /// 'fire' the handler.
        /// </summary>
        private void ShowSettings(Object sender, EventArgs e)
        {
            ServerOptions OptionsForm = new ServerOptions();
            OptionsForm.ServerEnabled = PluginEnabled;
            if (OptionsForm.ShowDialog(AWBForm.Form) == DialogResult.OK)
                PluginEnabled = OptionsForm.ServerEnabled;
        }

        private void PluginEnabledCheckedChange(Object sender, EventArgs e)
        {
            // TODO: Validate settings; start/stop server listening. eg could attach to this event in server object
            StatusText.Visible = PluginEnabled;

            if (PluginEnabled)
            {
                AWBForm.NotifyBalloon(Name + " enabled", ToolTipIcon.Info);
                AWBForm.AddTabPage(ServerPluginTabPage);

                // HACK:
                ServerControl.Init(49155);
            }
            else
            {
                AWBForm.NotifyBalloon(Name + " disabled", ToolTipIcon.Info);
                AWBForm.RemoveTabPage(ServerPluginTabPage);
            }
        }

        private static void HideButton_Click(object sender, EventArgs e)
        { AWBForm.RemoveTabPage(ServerPluginTabPage); }
    }
}
