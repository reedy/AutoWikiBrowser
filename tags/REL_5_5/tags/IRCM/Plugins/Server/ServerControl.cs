/*
(C) 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com
 With some Net.Sockets help and snippets from http://www.gnu.org/projects/dotgnu/pnetlib-doc/System/Net/Sockets/Socket.html

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
using WikiFunctions.Plugin;

namespace AutoWikiBrowser.Plugins.Server
{
    /// <summary>
    /// User Control which displays server info on a TabPage and which controls our menu items, settings object, server etc
    /// </summary>
    internal sealed partial class ServerControl : UserControl
    {
        // AWB objects:
        internal static IAutoWikiBrowser AWBForm;
        internal static ToolStripStatusLabel StatusText = new ToolStripStatusLabel(ServerAWBPlugin.ME + " loaded");

        // Menu items:
        private static ToolStripMenuItem EnabledMenuItem = new ToolStripMenuItem(ServerAWBPlugin.ME);
        private static ToolStripMenuItem ConfigMenuItem = new ToolStripMenuItem("Configuration");
        private static ToolStripMenuItem AboutMenuItem = new ToolStripMenuItem("About the AWB " + ServerAWBPlugin.ME);
        private static ToolStripMenuItem TabPageMenuItem = new ToolStripMenuItem("Tab Page");

        // Settings object/form:
        internal static ServerOptions Settings = new ServerOptions();

        // Server:
        private static Server serve;

        /// <summary>
        /// Constructor/initialisation, called from IAWBPlugin.Initialise
        /// </summary>
        /// <param name="AWBForm"></param>
        internal ServerControl(IAutoWikiBrowser sender)
        {
            InitializeComponent();

            // Store reference to AWB main form:
            AWBForm = sender;

            // Set up our UI objects:
            StatusText.Visible = false;
            StatusText.Margin = new Padding(10, 0, 10, 0);
            StatusText.BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Right;
            StatusText.BorderStyle = Border3DStyle.Etched;
            //AWBForm.StatusStrip.ShowItemToolTips = true; // naughty hack in case somebody turns this off in the designer
            EnabledMenuItem.CheckOnClick = true;
            TabPageMenuItem.CheckOnClick = true;

            // Event handlers:
            AboutMenuItem.Click += AboutMenuItemClicked;
            EnabledMenuItem.CheckedChanged += PluginEnabled_CheckedChange;
            TabPageMenuItem.CheckedChanged += TabPageMenuItem_CheckedChange;
            ConfigMenuItem.Click += ShowSettings;
            HideButton.Click += HideButton_Click;
            SettingsButton.Click += ShowSettings;

            // Add our UI objects to the AWB main form:
            AWBForm.StatusStrip.Items.Insert(2, StatusText);
            EnabledMenuItem.DropDownItems.Add(ConfigMenuItem);
            EnabledMenuItem.DropDownItems.Add(TabPageMenuItem);
            AWBForm.PluginsToolStripMenuItem.DropDownItems.Add(EnabledMenuItem);
            AWBForm.HelpToolStripMenuItem.DropDownItems.Add(AboutMenuItem);
            AWBForm.HelpToolStripMenuItem.DropDownItems.Add(AboutMenuItem);
        }

        /// <summary>
        /// Show the settings form; if Server Enabled status changes toggle the menu item which will
        /// 'fire' the handler.
        /// </summary>
        private void ShowSettings(Object sender, EventArgs e)
        {
            Settings.ServerEnabled = PluginEnabled;
            if (Settings.ShowDialog(AWBForm.Form) == DialogResult.OK)
                PluginEnabled = Settings.ServerEnabled;
        }

        // Properties
        /// <summary>
        /// Is the plugin enabled?
        /// </summary>
        internal bool PluginEnabled
        {
            get { return EnabledMenuItem.Checked; }
            set
            {  // prevent the event from firing unless there's a change:
                if (value != EnabledMenuItem.Checked)
                    EnabledMenuItem.Checked = value;
            }
        }
        // Event handlers
        private static void AboutMenuItemClicked(Object sender, EventArgs e)
        { new ServerAboutBox().Show(); }

        private void PluginEnabled_CheckedChange(Object sender, EventArgs e)
        {
            // TODO: Validate settings; start/stop server listening. eg could attach to this event in server object
            StatusText.Visible = PluginEnabled;

            if (PluginEnabled)
            {
                AWBForm.NotifyBalloon(Name + " enabled", ToolTipIcon.Info);
                ShowHideTab(true);

                serve = new Server();
                // HACK:
                serve.Init(49155);
                // TODO: Get details from registry in settings control
            }
            else
            {
                AWBForm.NotifyBalloon(Name + " disabled", ToolTipIcon.Info);
                ShowHideTab(false);
            }
        }

        private static void TabPageMenuItem_CheckedChange(Object sender, EventArgs e)
        { ShowHideTab(TabPageMenuItem.Checked); }

        private static void HideButton_Click(object sender, EventArgs e)
        { ShowHideTab(false); }

        private static void ShowHideTab(bool ShowHide)
        {
            if (TabPageMenuItem.Checked != ShowHide)
                TabPageMenuItem.Checked = ShowHide;

            if (ShowHide)
                AWBForm.AddTabPage(ServerAWBPlugin.ServerPluginTabPage);
            else
                AWBForm.RemoveTabPage(ServerAWBPlugin.ServerPluginTabPage);
        }
    }
}
