using System;
using System.Collections.Generic;
using WikiFunctions;
using WikiFunctions.Plugin;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WikiFunctions.AWBSettings;
using WikiFunctions.Parse;

namespace AutoWikiBrowser.Plugins.Delinker
{
    public class DelinkerAWBPlugin : IAWBPlugin
    {
        private readonly ToolStripMenuItem pluginenabledMenuItem = new ToolStripMenuItem("Delinker plugin");
        private readonly ToolStripMenuItem pluginconfigMenuItem = new ToolStripMenuItem("Configuration");
        //private ToolStripMenuItem aboutMenuItem = new ToolStripMenuItem("About the Delinker plugin");
        private static IAutoWikiBrowser AWB;

        private static bool Enabled;
        internal static bool Skip;
        internal static string Link;
        internal static bool RemoveEmptiedSections = true;

        private Regex r1,
                      r2,
                      r3;

        private readonly Regex RefStartRegex = new Regex(@"< ?ref(|[^>]*?[^/> ])\s*>", RegexOptions.Compiled);
        private readonly Regex RefNameRegex = new Regex(@"name ?= ?""?(\S*)""?", RegexOptions.Compiled);

        private readonly Regex ExternalLinksSectionRegex =
            new Regex(@"^={2,3}\s*((external )?links?|web ?links?|(внешн(€€|ие) )?ссылк[аи])\s*={2,3}\s*\r?\n",
                      RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline),
                               r4 = new Regex(@"< ?ref(|[^>]*?[^/> ])\s*>(.*?)< ?/ ?ref ?>",
                                              RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline |
                                              RegexOptions.IgnoreCase);

        private string LinkRegexed;

        private readonly List<string> RefNames = new List<string>();

        private readonly Parsers parser = new Parsers();

        public void Initialise(IAutoWikiBrowser sender)
        {
            if (sender == null)
                throw new ArgumentNullException("sender");

            AWB = sender;

            // Menuitem should be checked when plugin is active and unchecked when not, and default to not!
            pluginenabledMenuItem.CheckOnClick = true;

            pluginconfigMenuItem.Click += ShowSettings;
            pluginenabledMenuItem.CheckedChanged += PluginEnabledCheckedChange;
            //aboutMenuItem.Click += AboutMenuItemClicked;
            pluginenabledMenuItem.DropDownItems.Add(pluginconfigMenuItem);

            sender.PluginsToolStripMenuItem.DropDownItems.Add(pluginenabledMenuItem);
            //sender.HelpToolStripMenuItem.DropDownItems.Add(aboutMenuItem);

            Reset();
        }

        void ShowSettings(object sender, EventArgs e)
        {
            if (new SettingsForm().ShowDialog(AWB.Form) == DialogResult.OK)
                GenerateRegexes();
        }

        void PluginEnabledCheckedChange(object sender, EventArgs e)
        {
            Enabled = pluginenabledMenuItem.Checked;
        }

        public string Name
        {
            get { return "Delinker"; }
        }

        public string WikiName
        {
            get { return "Delinker plugin"; }
        }

        private string RemoveSection(string articleText)
        {
            //string text = WikiRegexes.Comments.Replace(ArticleText, "");

            string[] sections = Tools.SplitToSections(articleText);

            for (int i = 0; i < sections.Length; i++)
            {
                if (ExternalLinksSectionRegex.IsMatch(sections[i]))
                {
                    bool lastSection = (i == (sections.Length - 1));
                    if (WikiRegexes.ExternalLinks.IsMatch(sections[i])) return articleText;

                    string el = ExternalLinksSectionRegex.Replace(sections[i], "").Trim();

                    if (lastSection)
                    {
                        parser.Sorter.RemoveCats(ref el, "");
                        parser.Sorter.Interwikis(ref el);
                        MetaDataSorter.RemoveDisambig(ref articleText);
                        MetaDataSorter.RemovePersonData(ref articleText);
                        MetaDataSorter.RemoveStubs(ref articleText);
                    }
                    el = Parsers.RemoveAllWhiteSpace(articleText).Trim();

                    if (el.Length == 0)
                        return ExternalLinksSectionRegex.Replace(articleText, "");

                    break;
                }
            }

            return articleText;
        }

        string R4Evaluator(Match match)
        {
            if (RefStartRegex.IsMatch(match.Groups[2].Value)) return match.Value;

            if (Regex.IsMatch(match.Value, LinkRegexed))
            {
                string name = RefNameRegex.Match(match.Groups[1].Value).Groups[1].Value;
                if (name.Length > 0 && !RefNames.Contains(name)) RefNames.Add(name);

                return "";
            }

            return match.Value;
        }

        public string ProcessArticle(IAutoWikiBrowser sender, IProcessArticleEventArgs eventargs)
        {
            if (!Enabled) return eventargs.ArticleText;

            string articleText = eventargs.ArticleText;
            RefNames.Clear();

            articleText = r4.Replace(articleText, R4Evaluator);
            articleText = r1.Replace(articleText, "");
            articleText = r2.Replace(articleText, "");
            articleText = r3.Replace(articleText, "");

            if (RefNames.Count > 0)
                foreach (string name in RefNames)
                {
                    articleText = Regex.Replace(articleText, @"< ?ref\b[^>]*?name ?= ?""?" + name + "[\" ]? ?/ ?>", "");
                }

            if (articleText == eventargs.ArticleText)
            {
                eventargs.Skip = Skip;
            }
            else
            {
                if (RemoveEmptiedSections && (Variables.LangCode == "en" ||
                    Variables.LangCode == "de" || Variables.LangCode == "ru"))
                    articleText = RemoveSection(articleText);
            }

            return articleText;
        }

        public void LoadSettings(object[] prefs)
        {
            Reset();
            if (prefs == null) return;

            foreach (object o in prefs)
            {
                PrefsKeyPair p = o as PrefsKeyPair;
                if (p == null) continue;

                switch (p.Name.ToLower())
                {
                    case "enabled":
                        Enabled = (bool)p.Setting;
                        break;
                    case "link":
                        Link = (string)p.Setting;
                        break;
                    case "skip":
                        Skip = (bool)p.Setting;
                        break;
                    case "removeemptiedsections":
                        RemoveEmptiedSections = (bool)p.Setting;
                        break;
                }

                GenerateRegexes();
            }
        }

        public object[] SaveSettings()
        {
            return new object[]
            {
                new PrefsKeyPair("Enabled", Enabled),
                new PrefsKeyPair("Link", Link),
                new PrefsKeyPair("Skip", Skip),
                new PrefsKeyPair("RemoveEmptiedSections", RemoveEmptiedSections)
            };
        }

        public void Reset()
        {
            Enabled = false;
            Skip = false;
            Link = "";

            GenerateRegexes();
        }

        private void GenerateRegexes()
        {
            if (string.IsNullOrEmpty(Link)) return;

            LinkRegexed = @"http://(\S*?\.|)" + Link + @"/?(|[^\]\s]*?)";

            r1 = new Regex(@"^[\*#]\s*\[" + LinkRegexed + @"(|\s+[^\]]*?)\].{0,100}\n",
                RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);

            r2 = new Regex(@"^[\*#]\s*" + LinkRegexed + @"\s+.{0,100}\n",
                RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);

            r3 = new Regex(@"\[" + LinkRegexed + @"\]",
                RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
        }

        public void Nudge(out bool Cancel)
        {
            Cancel = false;
        }

        public void Nudged(int Nudges)
        {
        }
    }
}