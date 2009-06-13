using System;
using WikiFunctions;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using WikiFunctions.Plugin;
using WikiFunctions.AWBSettings;

namespace Fronds
{
    public class Fronds : WikiFunctions.Plugin.IAWBPlugin
    {

        private readonly ToolStripMenuItem enabledMenuItem = new ToolStripMenuItem("Fronds plugin");
        private readonly ToolStripMenuItem configMenuItem = new ToolStripMenuItem("Configuration");
        private readonly ToolStripMenuItem aboutMenuItem = new ToolStripMenuItem("About Fronds");
        internal static IAutoWikiBrowser AWB;
        internal static FrondsSettings Settings = new FrondsSettings();

        private static List<String> possibleFronds = new List<String>();
        private static List<String> possibleFilenames = new List<String>();
        private static List<String> loadedFinds = new List<String>();
        private static List<String> loadedReplaces = new List<String>();
        private static List<Boolean> loadedCases = new List<Boolean>();
        Dictionary<Regex, string> cases = new Dictionary<Regex, string>();

        public void Initialise(WikiFunctions.Plugin.IAutoWikiBrowser sender)
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
            enabledMenuItem.DropDownItems.Add(configMenuItem);
            enabledMenuItem.DropDownItems.Add(aboutMenuItem);

            AWB.PluginsToolStripMenuItem.DropDownItems.Add(enabledMenuItem);
            AWB.HelpToolStripMenuItem.DropDownItems.Add(aboutMenuItem);

            try
            {
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
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public string ProcessArticle(WikiFunctions.Plugin.IAutoWikiBrowser sender, WikiFunctions.Plugin.IProcessArticleEventArgs eventargs)
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
                MessageBox.Show("The Fronds plugin is running, but no individual fronds have been enabled. Either enable some or disable the plugin to stop getting this message.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return eventargs.ArticleText;
            }

            // The inefficiency of this is depressing
            // I was kinda hoping C# could match PHP's functionality -
            // where you can pass arrays of regexes

            string text = eventargs.ArticleText;
            for (int i = 0; i < loadedFinds.Count; i++)
            {
                   text = Regex.Replace(text, loadedFinds[i], loadedReplaces[i], loadedCases[i] ? RegexOptions.None : RegexOptions.IgnoreCase);
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
                frmFrondsOptions.addOption(frond);
            }
            frmFrondsOptions.Show();
            frmFrondsOptions.ButtonClicked += new OptionsOKClickedEventHandler(btnOptionsOK_Clicked);
        }

        private static void btnOptionsOK_Clicked(object sender, OptionsOKClickedEventArgs e)
        {
            loadedFinds = new List<String>();
            loadedReplaces = new List<String>();
            loadedCases = new List<Boolean>();

            foreach (int frond in e.EnabledIndices)
            {
                string html = Tools.GetHTML(("http://toolserver.org/~jarry/fronds/" + possibleFilenames[frond].ToString()));
                string[] parts = Regex.Split(html, "@#@");
                foreach (string chunk in parts)
                {
                    if (chunk.Contains("Find:"))
                    {
                        loadedFinds.Add(chunk.Substring(5));
                    }
                    else if (chunk.Contains("Replace:"))
                    {
                        loadedReplaces.Add(chunk.Substring(8));
                    }
                    else if (chunk.Contains("CaseSensitive:"))
                    {
                        string myCase = chunk.Trim().Substring(14);
                        switch (myCase.ToLower())
                        {
                            case "yes":
                                loadedCases.Add(true);
                                break;
                            case "no":
                                loadedCases.Add(false);
                                break;
                        }
                    }
                }
            }
        }

        public delegate void OptionsOKClickedEventHandler(object sender, OptionsOKClickedEventArgs e);

        private bool PluginEnabled
        {
            get { return enabledMenuItem.Checked; }
            set { enabledMenuItem.Checked = value; }
        }

        private void PluginEnabledCheckedChange(object sender, EventArgs e)
        {
            Settings.Enabled = PluginEnabled;
            if (PluginEnabled)
                AWB.NotifyBalloon("Fronds enabled", ToolTipIcon.Info);
            else
                AWB.NotifyBalloon("Fronds disabled", ToolTipIcon.Info);
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
                return "[[WP:FRONDS|Fronds]] Plugin version " +
                    System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            }
        }

        public void Reset()
        {
            //set default settings
            Settings = new FrondsSettings();
            PluginEnabled = false;
        }

        public void Nudge(out bool Cancel) { Cancel = false; }
        public void Nudged(int Nudges) { }
        #endregion

    }
}
