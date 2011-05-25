/*

Copyright (C) 2007 Martin Richards
(C) 2007 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/

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

namespace AutoWikiBrowser.Plugins.CFD
{
    public sealed class CfdAWBPlugin : IAWBPlugin
    {
        private readonly ToolStripMenuItem pluginenabledMenuItem = new ToolStripMenuItem("Categories For Deletion plugin");
        private readonly ToolStripMenuItem pluginconfigMenuItem = new ToolStripMenuItem("Configuration");
        private readonly ToolStripMenuItem aboutMenuItem = new ToolStripMenuItem("About the CFD plugin");
        internal static IAutoWikiBrowser AWB;
        internal static CfdSettings Settings = new CfdSettings();

        public void Initialise(IAutoWikiBrowser sender)
        {
            if (sender == null)
                throw new ArgumentNullException("sender");

            AWB = sender;

            // Menuitem should be checked when CFD plugin is active and unchecked when not, and default to not!
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
        { get { return "CFD-Plugin"; } }

        public string WikiName
        {
            get
            {
                return "[[WP:CFD|CFD]] Plugin version " +
                    System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            }
        }

        public string ProcessArticle(IAutoWikiBrowser sender, IProcessArticleEventArgs eventargs)
        {
            //If menu item is not checked, then return
            if (!PluginEnabled || Settings.Categories.Count == 0)
            {
                eventargs.Skip = false;
                return eventargs.ArticleText;
            }

            string text = eventargs.ArticleText;
            string removed = "", replaced = "";

            foreach (KeyValuePair<string, string> p in Settings.Categories)
            {
                bool noChange;

                if (p.Value.Length == 0)
                {
                    text = Parsers.RemoveCategory(p.Key, text, out noChange);
                    if (!noChange)
                    {
                        if (!string.IsNullOrEmpty(removed))
                        {
                            removed += ", ";
                        }

                        removed += Variables.Namespaces[Namespace.Category] + p.Key;
                    }
                }
                else
                {
                    text = Parsers.ReCategoriser(p.Key, p.Value, text, out noChange);
                    if (!noChange)
                    {
                        if (!string.IsNullOrEmpty(replaced))
                        {
                            replaced += ", ";
                        }

                        replaced += Variables.Namespaces[Namespace.Category]
                         + p.Key + FindandReplace.Arrow + Variables.Namespaces[Namespace.Category] + p.Value;
                    }
                }
                if (!noChange)
                {
                    text = Regex.Replace(text, "<includeonly>[\\s\\r\\n]*\\</includeonly>", "");
                }
            }

            string editSummary = "";
            if (Settings.AppendToEditSummary)
            {
                if (!string.IsNullOrEmpty(replaced))
                    editSummary = "replaced: " + replaced.Trim();

                if (!string.IsNullOrEmpty(removed))
                {
                    if (!string.IsNullOrEmpty(editSummary))
                        editSummary += ", ";

                    editSummary += "removed: " + removed.Trim();
                }
            }
            eventargs.EditSummary = editSummary;

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
                    case "skip":
                        Settings.Skip = (bool)p.Setting;
                        break;
                    case "appendtoeditsummary":
                        Settings.AppendToEditSummary = (bool)p.Setting;
                        break;
                }
                //Settings.Categories = (Dictionary<string, string>)pkp.Setting;
            }
        }

        public object[] SaveSettings()
        {
            Settings.Enabled = PluginEnabled;
            
            return new PrefsKeyPair[] {
            new PrefsKeyPair("Enabled", Settings.Enabled),
            new PrefsKeyPair("Skip", Settings.Skip),
            new PrefsKeyPair("AppendToEditSummary", Settings.AppendToEditSummary) };
        }

        public void Reset()
        {
            //set default settings
            Settings = new CfdSettings();
            PluginEnabled = false;
        }

        public void Nudge(out bool Cancel) { Cancel = false; }
        public void Nudged(int Nudges) { }

        private static void ShowSettings(object sender, EventArgs e)
        { new CfdOptions().Show(); }

        private bool PluginEnabled
        {
            get { return pluginenabledMenuItem.Checked; }
            set { pluginenabledMenuItem.Checked = value; }
        }

        private void PluginEnabledCheckedChange(object sender, EventArgs e)
        {
            Settings.Enabled = PluginEnabled;
            if (PluginEnabled)
                AWB.NotifyBalloon("CFD plugin enabled", ToolTipIcon.Info);
            else
                AWB.NotifyBalloon("CFD plugin disabled", ToolTipIcon.Info);
        }

        private static void AboutMenuItemClicked(Object sender, EventArgs e)
        {
            new AboutBox().Show();
        }
    }

    [Serializable]
    internal sealed class CfdSettings
    {
        public bool Enabled;
        public Dictionary<string, string> Categories = new Dictionary<string, string>();
        public bool Skip = true;
        public bool AppendToEditSummary = true;
    }
}


