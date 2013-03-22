/*
Copyright (C) 2007

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
using System.Windows.Forms;
using System.Text.RegularExpressions;
using WikiFunctions;
using WikiFunctions.Parse;
using WikiFunctions.AWBSettings;

namespace AutoWikiBrowser.Plugins.IFD
{
    public sealed class IfdAWBPlugin : IAWBPlugin
    {
        private readonly ToolStripMenuItem pluginenabledMenuItem = new ToolStripMenuItem("Images For Deletion plugin");
        private readonly ToolStripMenuItem pluginconfigMenuItem = new ToolStripMenuItem("Configuration");
        private readonly ToolStripMenuItem aboutMenuItem = new ToolStripMenuItem("About the IFD plugin");
        internal static IAutoWikiBrowser AWB;
        internal static IfdSettings Settings = new IfdSettings();

        public void Initialise(IAutoWikiBrowser sender)
        {
            if (sender == null)
                throw new ArgumentNullException("sender");

            AWB = sender;

            // Menuitem should be checked when IFD plugin is active and unchecked when not, and default to not!
            pluginenabledMenuItem.CheckOnClick = true;
            PluginEnabled = Settings.Enabled;

            pluginconfigMenuItem.Click += ShowSettings;
            pluginenabledMenuItem.CheckedChanged += PluginEnabledCheckedChange;
            aboutMenuItem.Click += AboutMenuItemClicked;
            pluginenabledMenuItem.DropDownItems.Add(pluginconfigMenuItem);

            sender.PluginsToolStripMenuItem.DropDownItems.Add(pluginenabledMenuItem);
            sender.HelpToolStripMenuItem.DropDownItems.Add(aboutMenuItem);
        }

        public string Name
        { get { return "IFD-Plugin"; } }

        public string WikiName
        {
            get
            {
                return "[[WP:IFD|IFD]] Plugin version " +
            System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            }
        }

        public string ProcessArticle(IAutoWikiBrowser sender, IProcessArticleEventArgs eventargs)
        {
            //If menu item is not checked, then return
            if (!PluginEnabled || Settings.Images.Count == 0)
            {
                eventargs.Skip = false;
                return eventargs.ArticleText;
            }

            eventargs.EditSummary = "";
            string text = eventargs.ArticleText;

            foreach (KeyValuePair<string, string> p in Settings.Images)
            {
                bool noChange;

                if (p.Value.Length == 0)
                {
                    text = Parsers.RemoveImage(p.Key, text, Settings.Comment, "", out noChange);
                    if (!noChange) eventargs.EditSummary += ", removed " + Variables.Namespaces[Namespace.File] + p.Key;
                }
                else
                {
                    text = Parsers.ReplaceImage(p.Key, p.Value, text, out noChange);
                    if (!noChange) eventargs.EditSummary += ", replaced: " + Variables.Namespaces[Namespace.File]
                         + p.Key + FindandReplace.Arrow + Variables.Namespaces[Namespace.File] + p.Value;
                }
                if (!noChange) text = Regex.Replace(text, "<includeonly>[\\s\\r\\n]*\\</includeonly>", "");
            }

            eventargs.Skip = (text == eventargs.ArticleText) && Settings.Skip;

            return text;
        }

        public void LoadSettings(object[] prefs)
        {
            if (prefs == null) return;

            foreach (object o in prefs)
            {
                PrefsKeyPair p = o as PrefsKeyPair;
                if (p == null) continue;

                switch (p.Name.ToLower())
                {
                    case "enabled":
                        PluginEnabled = Settings.Enabled = (bool)p.Setting;
                        break;
                    case "comment":
                        Settings.Comment = (bool)p.Setting;
                        break;
                    case "skip":
                        Settings.Skip = (bool)p.Setting;
                        break;
                }
            }
            //Settings.Images = (Dictionary<string, string>)pkp.Setting;
        }

        public object[] SaveSettings()
        {
            Settings.Enabled = PluginEnabled;

            PrefsKeyPair[] prefs = new PrefsKeyPair[3];
            prefs[0] = new PrefsKeyPair("Enabled", Settings.Enabled);
            prefs[1] = new PrefsKeyPair("Comment", Settings.Comment);
            prefs[2] = new PrefsKeyPair("Skip", Settings.Skip);
            //prefs[3] = new PrefsKeyPair("images", Settings.Images);

            return prefs;
        }

        public void Reset()
        {
            //set default settings
            Settings = new IfdSettings();
            PluginEnabled = Settings.Enabled;
        }

        public void Nudge(out bool Cancel) { Cancel = false; }
        public void Nudged(int Nudges) { }

        private static void ShowSettings(Object sender, EventArgs e)
        { new IfdOptions().Show(); }

        private bool PluginEnabled
        {
            get { return pluginenabledMenuItem.Checked; }
            set { pluginenabledMenuItem.Checked = value; }
        }

        private void PluginEnabledCheckedChange(object sender, EventArgs e)
        {
            Settings.Enabled = PluginEnabled;
            if (PluginEnabled)
                AWB.NotifyBalloon("IFD plugin enabled", ToolTipIcon.Info);
            else
                AWB.NotifyBalloon("IFD plugin disabled", ToolTipIcon.Info);
        }

        private static void AboutMenuItemClicked(object sender, EventArgs e)
        {
            new AboutBox().Show();
        }
    }
}