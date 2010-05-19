using System;
using System.Xml;
using WikiFunctions;
using System.Windows.Forms;
using System.Collections.Generic;
using WikiFunctions.Plugin;
using WikiFunctions.AWBSettings;

namespace Fronds
{
    public class Fronds : IAWBPlugin
    {
        private readonly ToolStripMenuItem EnabledMenuItem = new ToolStripMenuItem("Fronds plugin");
        private readonly ToolStripMenuItem ConfigMenuItem = new ToolStripMenuItem("Configuration");
        private readonly ToolStripMenuItem PluginAboutMenuItem = new ToolStripMenuItem("About");
        private readonly ToolStripMenuItem AboutMenuItem = new ToolStripMenuItem("About Fronds");

        private static IAutoWikiBrowser AWB;
        internal static FrondsSettings Settings = new FrondsSettings();
        internal static readonly string CurrentVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        internal static readonly List<string> PossibleFilenames = new List<string>();

        private static readonly List<string> PossibleFronds = new List<string>();

        internal static readonly List<Frond> Replacements = new List<Frond>();

        internal const string BaseURL = "http://toolserver.org/~jarry/fronds/";

        public void Initialise(IAutoWikiBrowser sender)
        {
            if (sender == null)
                throw new ArgumentNullException("sender");
            AWB = sender;

            // Menuitem should be checked when Fronds plugin is active and unchecked when not, and default to not!
            EnabledMenuItem.CheckOnClick = true;
            PluginEnabled = Settings.Enabled;

            ConfigMenuItem.Click += ShowSettings;
            EnabledMenuItem.CheckedChanged += PluginEnabledCheckedChange;
            AboutMenuItem.Click += AboutMenuItemClicked;
            PluginAboutMenuItem.Click += AboutMenuItemClicked;

            EnabledMenuItem.DropDownItems.AddRange(new[] {ConfigMenuItem, PluginAboutMenuItem});
            AWB.PluginsToolStripMenuItem.DropDownItems.Add(EnabledMenuItem);
            AWB.HelpToolStripMenuItem.DropDownItems.Add(AboutMenuItem);
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
                    ConfigMenuItem.PerformClick();
                }
                else
                {
                    EnabledMenuItem.Checked = Settings.Enabled = PluginEnabled = false;
                }
                return eventargs.ArticleText;
            }

            string text = eventargs.ArticleText;
            foreach (Frond f in Replacements)
            {
                text = f.Preform(text);
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
            new FrondsOptions(PossibleFronds).Show(); //TODO: We can probably reuse the instance and just update the enabled state
        }

        private bool PluginEnabled
        {
            get { return EnabledMenuItem.Checked; }
            set
            {
                EnabledMenuItem.Checked = value;

                if (value && PossibleFilenames.Count == 0)
                    LoadFronds();
            }
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
        { get { return "Fronds"; } }

        public string WikiName
        { get { return "[[WP:FRONDS|Fronds]] Plugin version " + CurrentVersion; } }

        public void Reset()
        {
            //set default settings
            Settings = new FrondsSettings();
            PluginEnabled = false;
        }

        public void Nudge(out bool cancel) { cancel = false; }
        public void Nudged(int nudges) { }
        #endregion

        private static void LoadFronds()
        {
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(Tools.GetHTML(BaseURL + "index.xml"));

            if (xd["fronds"] == null)
                return;

            foreach (XmlNode xn in xd["fronds"].GetElementsByTagName("frond"))
            {
                if (xn.ChildNodes.Count != 2)
                    continue;

                PossibleFilenames.Add(xn.ChildNodes[0].InnerText);
                PossibleFronds.Add(xn.ChildNodes[1].InnerText + " (" + xn.ChildNodes[0].InnerText + ")");
            }
        }
    }
}
