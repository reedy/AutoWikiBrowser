using System;
using WikiFunctions;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using WikiFunctions.Plugin;
using WikiFunctions.AWBSettings;

namespace Fronds
{
    public class Fronds : IAWBPlugin
    {
        private readonly ToolStripMenuItem enabledMenuItem = new ToolStripMenuItem("Fronds plugin");
        private readonly ToolStripMenuItem configMenuItem = new ToolStripMenuItem("Configuration");
        private readonly ToolStripMenuItem pluginAboutMenuItem = new ToolStripMenuItem("About");
        private readonly ToolStripMenuItem aboutMenuItem = new ToolStripMenuItem("About Fronds");

        internal static IAutoWikiBrowser AWB;
        internal static FrondsSettings Settings = new FrondsSettings();
        internal static String currentVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

        private static List<String> possibleFronds = new List<String>();
        internal static List<String> possibleFilenames = new List<String>();
        internal static List<String> loadedFinds = new List<String>();
        internal static List<String> loadedReplaces = new List<String>();
        internal static List<Boolean> loadedCases = new List<Boolean>();

        public void Initialise(IAutoWikiBrowser sender)
        {
            if (sender == null)
                throw new ArgumentNullException("sender");
            AWB = sender;

            // Menuitem should be checked when Fronds plugin is active and unchecked when not, and default to not!
            enabledMenuItem.CheckOnClick = true;
            PluginEnabled = Settings.Enabled;

            configMenuItem.Click += ShowSettings;
            enabledMenuItem.CheckedChanged += PluginEnabledCheckedChange;
            aboutMenuItem.Click += AboutMenuItemClicked;
            pluginAboutMenuItem.Click += AboutMenuItemClicked;
            enabledMenuItem.DropDownItems.Add(configMenuItem);
            enabledMenuItem.DropDownItems.Add(pluginAboutMenuItem);


            AWB.PluginsToolStripMenuItem.DropDownItems.Add(enabledMenuItem);
            AWB.HelpToolStripMenuItem.DropDownItems.Add(aboutMenuItem);

            string newVersion = Tools.GetHTML("http://toolserver.org/~jarry/fronds/version.txt").Replace(".","");
            if (Int16.Parse(newVersion) > Int16.Parse(currentVersion.Replace(".", "")))
            {
                DialogResult result = MessageBox.Show(
                    "A newer version of Fronds is available. Downloading it is advisable, as it may contain important bugfixes.\r\n\r\nLoad update page now?",
                    "New version", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (result == DialogResult.Yes)
                {
                    Tools.OpenURLInBrowser("http://en.wikipedia.org/wiki/WP:FRONDS/U");
                }
            }
            string html = Tools.GetHTML("http://toolserver.org/~jarry/fronds/index.txt");
            string[] all = Regex.Split(html, "\r\n");
            foreach (string item in all)
            {
                if (item.Contains("@#@"))
                {
                    string[] parts = Regex.Split(item, "@#@");
                    string filename = parts[0].Trim();
                    string meta = parts[1].Trim();
                    possibleFronds.Add(meta + " (" + filename + ")");
                    possibleFilenames.Add(filename);
                }
            }
        }

        public string ProcessArticle(IAutoWikiBrowser sender, IProcessArticleEventArgs eventargs)
        {
            //If menu item is not checked, then return
            if (!PluginEnabled)
            {
                eventargs.Skip = false;
                return eventargs.ArticleText;
            }

            // Warn if plugin is running, but no fronds have been enabled. A common newbie situation.
            if (Settings.EnabledFilenames.Count == 0)
            {
                DialogResult result = MessageBox.Show(
                    "It looks like you forget to select some fronds to use. You might like to choose some (\"Okay\"), or disable the plugin for now (\"Cancel\").",
                    "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                if (result == DialogResult.OK)
                {
                    configMenuItem.PerformClick();
                }
                else
                {
                    enabledMenuItem.Checked = Settings.Enabled = PluginEnabled = false;
                }
                return eventargs.ArticleText;
            }

            // The inefficiency of this is depressing

            string text = eventargs.ArticleText;
            for (int i = 0; i < loadedFinds.Count; i++)
            {
                text = Regex.Replace(text, loadedFinds[i], loadedReplaces[i],
                                     loadedCases[i] ? RegexOptions.None : RegexOptions.IgnoreCase);
            }
            return text;
        }

        #region Settings and options
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
                    case "enabledfilenames":
                        Settings.EnabledFilenames = (List<string>)p.Setting;
                        break;
                    default:
                        break;
                }
            }
        }

        public object[] SaveSettings()
        {
            Settings.Enabled = PluginEnabled;

            PrefsKeyPair[] prefs = new PrefsKeyPair[2];
            prefs[0] = new PrefsKeyPair("Enabled", Settings.Enabled);
            prefs[1] = new PrefsKeyPair("EnabledFilenames", Settings.EnabledFilenames);

            return prefs;
        }

        private static void ShowSettings(object sender, EventArgs e)
        {
            FrondsOptions frmFrondsOptions = new FrondsOptions();
            foreach (string frond in possibleFronds)
            {
                frmFrondsOptions.AddOption(frond);
            }
            frmFrondsOptions.Show();
        }

        private bool PluginEnabled
        {
            get { return enabledMenuItem.Checked; }
            set { enabledMenuItem.Checked = value; }
        }

        private void PluginEnabledCheckedChange(object sender, EventArgs e)
        {
            Settings.Enabled = PluginEnabled;
        }

        private static void AboutMenuItemClicked(Object sender, EventArgs e)
        {
            new FrondsAbout().Show();
        }

        [Serializable]
        internal sealed class FrondsSettings
        {
            public bool Enabled;
            public List<String> EnabledFilenames = new List<String>();
        }

        #endregion

        #region Other IAWBPlugin members

        public string Name
        {
            get { return "Fronds"; }
        }

        public string WikiName
        {
            get
            {
                return "[[WP:FRONDS|Fronds]] Plugin version " + currentVersion;
            }
        }

        public void Reset()
        {
            //set default settings
            Settings = new FrondsSettings();
            PluginEnabled = false;
        }

        public void Nudge(out bool cancel) { cancel = false; }
        public void Nudged(int nudges) { }
        #endregion

    }
}
