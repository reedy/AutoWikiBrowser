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

[assembly: CLSCompliant(true)]
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
        internal static ToolStripStatusLabel StatusText = new ToolStripStatusLabel(conMe + " Loaded");

        // Menu items:
        internal static ToolStripMenuItem EnabledMenuItem = new ToolStripMenuItem(conMe);
        internal static ToolStripMenuItem ConfigMenuItem = new ToolStripMenuItem("Configuration");
        internal static ToolStripMenuItem AboutMenuItem = new ToolStripMenuItem("About the AWB " + conMe);

        #region IAWBPlugin Members

        void IAWBPlugin.Initialise(IAutoWikiBrowser sender)
        {
            if (sender == null)
                throw new ArgumentNullException("sender");

            // Store AWB object reference:
            AWBForm = sender;

            // Set up our UI objects:
            AWBForm.StatusStrip.Items.Insert(2, StatusText);
            StatusText.Margin = new Padding(50, 0, 50, 0);
            StatusText.BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Right;
            StatusText.BorderStyle = Border3DStyle.Etched;

            AWBForm.HelpToolStripMenuItem.DropDownItems.Add(AboutMenuItem);
            AboutMenuItem.Click += AboutMenuItemClicked;
        }

        public string Name
        {
            get { return "AWB " + conMe; }
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

        #region UI Event Handlers

        private static void AboutMenuItemClicked(Object sender, EventArgs e)
        {
            new ServerAboutBox().Show();
        }

        #endregion
    }
}
