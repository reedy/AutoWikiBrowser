/*

Copyright © Rick Martin (ClickRick)

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

#define DEBUG_OUTPUT_DIALOG
#define SHORT_PLUGIN_MENU

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

// AutoWikiBrowser plugin support:
//using WikiFunctions;
using WikiFunctions.AWBSettings;
//using WikiFunctions.Parse;
//using WikiFunctions.Plugin;

namespace AutoWikiBrowser.Plugins.TheTemplator
{
    /// <summary>
    /// TheTemplator plugin for AutoWikiBrowser. Gives UI-assisted changes to portions of templates within articles.
    /// Current limitations:
    /// 1. Cannot handle positional parameters
    ///    - it will fail to parse the template
    /// 2. Cannot handle empty parameters
    ///    - it will remove a pipe
    /// 3. Cannot handle wikitables used within templates
    ///    - it will stop and prompt the operator
    /// </summary>
    public class TheTemplator : WikiFunctions.Plugin.IAWBPlugin
    {
#if SHORT_PLUGIN_MENU
        private readonly ToolStripMenuItem pluginMenuItem = new ToolStripMenuItem("TheTemplator plugin");
        private readonly ToolStripMenuItem pluginConfigMenuItem = new ToolStripMenuItem("&Configuration...");
        private readonly ToolStripMenuItem aboutMenuItem2 = new ToolStripMenuItem("About TheTemplator plugin...");
#else
		private readonly ToolStripMenuItem pluginMenuItem = new ToolStripMenuItem("TheTemplator plugin");
		private readonly ToolStripMenuItem pluginEnabledMenuItem = new ToolStripMenuItem("&Enabled");
		private readonly ToolStripMenuItem pluginConfigMenuItem = new ToolStripMenuItem("&Configuration...");
		private readonly ToolStripMenuItem aboutMenuItem1 = new ToolStripMenuItem("&About TheTemplator plugin...");
		private readonly ToolStripMenuItem aboutMenuItem2 = new ToolStripMenuItem("About TheTemplator plugin...");
#endif

        #region IAWBPlugin Members
        public void Initialise(WikiFunctions.Plugin.IAutoWikiBrowser sender)
        {
            if (sender == null)
                throw new ArgumentNullException("sender");

            AWB = sender;

            // Menuitem should be checked when CFD plugin is active and unchecked when not, and default to not!
#if SHORT_PLUGIN_MENU
            pluginMenuItem.CheckOnClick = true;
            pluginConfigMenuItem.Click += ShowSettings;
            pluginMenuItem.CheckedChanged += PluginEnabledCheckedChange;
            aboutMenuItem2.Click += AboutMenuItemClicked;
            pluginMenuItem.DropDownItems.Add(pluginConfigMenuItem);
#else
			pluginEnabledMenuItem.CheckOnClick = true;
			pluginConfigMenuItem.Click += ShowSettings;
			pluginEnabledMenuItem.CheckedChanged += PluginEnabledCheckedChange;
			aboutMenuItem1.Click += AboutMenuItemClicked;
			aboutMenuItem2.Click += AboutMenuItemClicked;
			pluginMenuItem.DropDownItems.Add(pluginEnabledMenuItem);
			pluginMenuItem.DropDownItems.Add(pluginConfigMenuItem);
			pluginMenuItem.DropDownItems.Add("-");
			pluginMenuItem.DropDownItems.Add(aboutMenuItem1);
#endif

            sender.PluginsToolStripMenuItem.DropDownItems.Add(pluginMenuItem);
            sender.HelpToolStripMenuItem.DropDownItems.Add(aboutMenuItem2);

            // get defaults for the dialog from the designer
            TemplatorConfig dlg = new TemplatorConfig(defaultSettings.TemplateName,
                                                      defaultSettings.Parameters,
                                                      defaultSettings.Replacements,
                                                      defaultSettings.SkipIfNoTemplates,
                                                      defaultSettings.RemoveExcessPipes);
            defaultSettings.dlgWidth = dlg.Width;
            defaultSettings.dlgHeight = dlg.Height;
            defaultSettings.dlgCol0 = dlg.paramName.Width;
            defaultSettings.dlgCol1 = dlg.paramRegex.Width;
        }
        public void LoadSettings(object[] prefs)
        {
            if (prefs == null)
                return;
            Settings = defaultSettings;
            Settings.Parameters = new Dictionary<string, string>();
            Settings.Replacements = new Dictionary<string, string>();

            foreach (object o in prefs)
            {
                PrefsKeyPair p = o as PrefsKeyPair;
                if (p == null)
                    continue;

                switch (p.Name.ToLower())
                {
                case "enabled":
                    Settings.Enabled = (bool)p.Setting;
#if SHORT_PLUGIN_MENU
                    pluginMenuItem.Checked = Settings.Enabled;
#else
					pluginEnabledMenuItem.Checked = Settings.Enabled;
#endif
                    break;
                case "xspipes":
                    Settings.RemoveExcessPipes = (bool)p.Setting;
                    break;
                case "skip":
                    Settings.SkipIfNoTemplates = (bool)p.Setting;
                    break;
                case "template":
                    Settings.TemplateName = (string)p.Setting;
                    break;
                case "dlgwidth":
                    Settings.dlgWidth = (int)p.Setting;
                    break;
                case "dlgheight":
                    Settings.dlgHeight = (int)p.Setting;
                    break;
                case "dlgcol0":
                    Settings.dlgCol0 = (int)p.Setting;
                    break;
                case "dlgcol1":
                    Settings.dlgCol1 = (int)p.Setting;
                    break;
                default:
                    if (p.Name.StartsWith(":"))
                        Settings.Parameters[p.Name.Substring(1)] = (string)p.Setting;
                    else if (p.Name.StartsWith(";"))
                        Settings.Replacements[p.Name.Substring(1)] = (string)p.Setting;
                    break;
                }
            }
            RegexString = ""; // force the main regex to be evaluated on next use
        }
        public object[] SaveSettings()
        {
            List<PrefsKeyPair> settings = new List<PrefsKeyPair>();
            settings.Add(new PrefsKeyPair("enabled", Settings.Enabled));
            settings.Add(new PrefsKeyPair("xspipes", Settings.RemoveExcessPipes));
            settings.Add(new PrefsKeyPair("skip", Settings.SkipIfNoTemplates));
            settings.Add(new PrefsKeyPair("template", Settings.TemplateName));
            settings.Add(new PrefsKeyPair("dlgwidth", Settings.dlgWidth));
            settings.Add(new PrefsKeyPair("dlgheight", Settings.dlgHeight));
            settings.Add(new PrefsKeyPair("dlgcol0", Settings.dlgCol0));
            settings.Add(new PrefsKeyPair("dlgcol1", Settings.dlgCol1));
            foreach (KeyValuePair<string, string> p in Settings.Parameters)
                settings.Add(new PrefsKeyPair(":" + p.Key, p.Value));
            foreach (KeyValuePair<string, string> p in Settings.Replacements)
                settings.Add(new PrefsKeyPair(";" + p.Key, p.Value));
            return settings.ToArray();
        }
        public string Name
        {
            get { return PluginName; }
        }
        public void Nudge(out bool Cancel)
        {
            Cancel = false;
        }
        public void Nudged(int Nudges)
        {
        }
        public string ProcessArticle(WikiFunctions.Plugin.IAutoWikiBrowser sender, WikiFunctions.Plugin.IProcessArticleEventArgs eventargs)
        {
            string text = eventargs.ArticleText;

            if (!Settings.Enabled)
                return text;

            // Get the set of templates from the article text
            MatchCollection matches = WikiFunctions.Parse.Parsers.GetTemplates(text, Settings.TemplateName);
            if (matches.Count == 0)
            {
                eventargs.Skip = Settings.SkipIfNoTemplates;
                return text;
            }

            // Build our regex from the settings, but only if we need to
            if (RegexString == "")
                BuildRegexes();
            // Nothing to do?  Do nothing.
            if (RegexString == "")
            {
                eventargs.Skip = Settings.SkipIfNoTemplates;
                return text;
            }

            int deltaLength = 0; // used when re-inserting text, to account for differences in previous replacements

            // Now apply our regex to each instance of the template in the article text
            foreach (Match match in matches)
            {
                string matchSegment = match.Value;

                /// 3. Cannot handle wikitables used within templates
                if (matchSegment.Contains("{|"))
                {
                    MessageBox.Show("Infobox contains a wikitable:\r\n\r\n" + text, eventargs.ArticleTitle);
                    eventargs.Skip = Settings.SkipIfNoTemplates;
                    return text;
                }

                if (Settings.RemoveExcessPipes)
                    if (ExcessPipe.IsMatch(matchSegment))
                        matchSegment = ExcessPipe.Replace(matchSegment, RemovePipeReplacement);

                // Matches
                if (!findParametersRegex.IsMatch(matchSegment))
                {
                    MessageBox.Show(string.Format("Bad template matched:\r\n\r\n{0}", matchSegment), eventargs.ArticleTitle);
                    continue;
                }
                Match m = findParametersRegex.Match(matchSegment);
                System.Diagnostics.Debug.Assert(m.Success);

                // Build the segments of the original input string into one composite, in a specific order
                string paramSegments = "";
                foreach (KeyValuePair<string, string> param in Settings.Parameters)
                {
                    string paramSegment = m.Groups["__" + param.Key].ToString();
                    if (paramSegment == "")
                        paramSegments += "|" + param.Key + "=";
                    else
                        paramSegments += paramSegment + m.Groups["_" + param.Key].ToString();
                }
#if DEBUG_OUTPUT_DIALOG
                DebugOutput dlg = new DebugOutput();
                dlg.StartSection("Groups");
                for (int i = 0; i < findParametersRegex.GetGroupNumbers().Length; ++i)
                    dlg.AddSection("Group " + findParametersRegex.GetGroupNames()[i], m.Groups[i].ToString());
                dlg.EndSection();
                dlg.AddSection("paramSegments", paramSegments);
                dlg.AddSection("paramMatcher", paramReplacementRegex.ToString());
                //dlg.AddSection("paramReplacement", paramReplacement);
                dlg.ShowDialog();
#endif
                //TODO: Transform the segments according to the user's replacement rules
#if TODO
				string paramReplacement = paramReplacementRegex.Replace(paramSegments, Settings.Replacements);

				// Check for only whitespace difference between old and new
				if (whiteSpaceRegex.Replace(paramSegments, "") == whiteSpaceRegex.Replace(paramReplacement, ""))
					continue;

				// Remove all occurrences of all of the parameters we're operating on.
				// Note the index of the first occurrence as the insertion point for the replacement
				int firstIndex = match.Length;
				foreach (KeyValuePair<string, string> param in Settings.Parameters)
				{
					Regex segmentRegex = paramRemovalRegexes[param.Key];
					Match segmentMatch = segmentRegex.Match(matchSegment);
					if (segmentMatch.Success)
					{
						matchSegment = segmentRegex.Replace(matchSegment, "");
						if (segmentMatch.Index < firstIndex)
							firstIndex = segmentMatch.Index;
					}
				}

				// Replace the segments in this match with our new version
				matchSegment = matchSegment.Substring(0, firstIndex)
							 + paramReplacement
							 + matchSegment.Substring(firstIndex);

				// Replace this match back into the original text at match.Index+deltaLength
				text = text.Substring(0, match.Index + deltaLength)
					 + matchSegment
					 + text.Substring(match.Index + match.Length + deltaLength);
				deltaLength += matchSegment.Length - match.Length;
#endif // TODO
#if TODO && DEBUG_OUTPUT_DIALOG
                dlg.Clear();
				dlg.AddSection("paramSegments", paramSegments);
				dlg.AddSection("paramMatcher", paramReplacementRegex.ToString());
				dlg.AddSection("paramReplacement", paramReplacement);
				dlg.AddSection("match.Value was", match.Value);
				dlg.AddSection("matchSegment", matchSegment);
				dlg.AddSection("text", text);
				dlg.ShowDialog();
#endif
            }
            return text;
        }

        public void Reset()
        {
            Settings = defaultSettings;
            Settings.Parameters = new Dictionary<string, string>();
            Settings.Replacements = new Dictionary<string, string>();
        }
        public string WikiName
        {
            get { return Name + " Plugin version " + Version; }
        }
        #endregion

        /// <summary>
        /// Build the primary regular expressions - findParametersRegex and paramReplacementRegex - which do the work.
        /// </summary>
        private void BuildRegexes()
        {
            RegexString = "";
            if (Settings.Parameters.Count == 0)
                return;

            // Yes, I know I iterate over Settings.Parameters multiple times. That's hardly the expensive part.

            // findParametersRegex
            foreach (KeyValuePair<string, string> param in Settings.Parameters)
            {
                string name = nonIdentiferCharsRegex.Replace(param.Key, "_");
                // WikiFunctions.WikiRegexes.UnFormattedText:
                //      @"<nowiki>.*?</nowiki>|<pre>.*?</pre>|<math>.*?</math>|<!--.*?-->|<timeline>.*?</timeline>"
                // WikiFunctions.WikiRegexes.NestedTemplates:
                //      @"{{((?>[^\{\}]+|\{(?<DEPTH>)|\}(?<-DEPTH>))*(?(DEPTH)(?!))}})"
                // WikiFunctions.WikiRegexes.SimpleWikiLink
                //      @"\[\[((?>[^\[\]\n]+|\[\[(?<DEPTH>)|\]\](?<-DEPTH>))*(?(DEPTH)(?!)))\]\]"
                // WikiFunctions.WikiRegexes.ExternalLinks
                //      @"(?:[Hh]ttp|[Hh]ttps|[Ff]tp|[Mm]ailto)://[^\ \n<>]*|\[(?:[Hh]ttp|[Hh]ttps|[Ff]tp|[Mm]ailto):.*?\]"
                // BorkedExternalLinks
                //      @"\[.*?\]"
                // Find our template parameter, with its value
                // Match anything that's normal text, possibly text including complete links and complete templates
                // and capture it with a groupname of '_name'
                // Also capture the parameter name "|label=" with a groupname of '__name' for guessing whitespace requirements in the replacements
                string regexGroup
                    // leading white space, the pipe to introduce the parameter, and any more white space
                    = @"(?<__" + name + @">\s*\|\s*"
                    // the parameter name, with any white space, and its equals sign
                    + param.Key + @"\s*=\s*?)"
                    // We keep the whole of parameter value as a named match, but don't keep the components of it:
                    + @"(?<_" + name + ">("
                    // comments, HTML-type tags, etc:
                    + WikiFunctions.WikiRegexes.UnFormattedText
                    + "|"
                    // templates inside this template:
                    + WikiFunctions.WikiRegexes.NestedTemplates
                    + "|"
                    // wikilinks in the parameter value:
                    + WikiFunctions.WikiRegexes.SimpleWikiLink
                    + "|"
                    // external links, or just text in square brackets:
                    + BorkedExternalLinks
                    + "|"
                    // anything else, like regular text and white space
                    + @"[^\[\{\|\}]*"
                    // Repeat the contents of the value
                    + @")*"
                    // Close the named group "_name"
                    + @")"
                    // Terminate at the white space before either a pipe or the closing brace
                    + @"(?=\s*[\|\}])";
                RegexString += regexGroup + "|";
            }
            string rest
                = @"(\s*\|\s*)"
                + @"(([^=\s]*)\s*=\s*?)?"
                + "("
                + WikiFunctions.WikiRegexes.UnFormattedText
                + "|"
                + WikiFunctions.WikiRegexes.NestedTemplates
                + "|"
                + WikiFunctions.WikiRegexes.SimpleWikiLink
                + "|"
                + BorkedExternalLinks
                + "|"
                + @"[^\[\{\|\}]*"
                + @")*(?=\s*[\|\}])";
            RegexString = @"\{\{[^|]*" + ("(" + RegexString + rest + ")*") + @"\}\}";
            findParametersRegex = new Regex(RegexString, RegexOptions.Compiled | RegexOptions.ExplicitCapture);

            // paramRemovalRegexes
            paramRemovalRegexes = new Dictionary<string, Regex>(Settings.Parameters.Count);
            foreach (KeyValuePair<string, string> param in Settings.Parameters)
            {
                string paramRegexString
                        = @"(\s*\|\s*" + param.Key + @"\s*=\s*(("
                        + WikiFunctions.WikiRegexes.UnFormattedText
                        + "|"
                        + WikiFunctions.WikiRegexes.NestedTemplates
                        + "|"
                        + WikiFunctions.WikiRegexes.SimpleWikiLink
                        + "|"
                        + BorkedExternalLinks
                        + "|"
                        + @"[^\[\{\|\}]*"
                        + ")*))";
                paramRemovalRegexes[param.Key] = new Regex(paramRegexString, RegexOptions.Compiled | RegexOptions.ExplicitCapture);
            }

            // paramReplacementRegex
            string paramMatcher = "";
            foreach (KeyValuePair<string, string> param in Settings.Parameters)
                paramMatcher += @"\s*\|\s*" + param.Key + @"\s*=\s*" + param.Value;
            paramReplacementRegex = new Regex(paramMatcher, RegexOptions.Compiled);
        }

        // Menu item handlers:
        void ShowSettings(object o, EventArgs e)
        {
            TemplatorConfig dlg = new TemplatorConfig(Settings.TemplateName,
                                                      Settings.Parameters,
                                                      Settings.Replacements,
                                                      Settings.SkipIfNoTemplates,
                                                      Settings.RemoveExcessPipes);
            dlg.Width = Settings.dlgWidth;
            dlg.Height = Settings.dlgHeight;
            dlg.paramName.Width = Settings.dlgCol0;
            dlg.paramRegex.Width = Settings.dlgCol1;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Settings.TemplateName = dlg.TemplateName;
                Settings.SkipIfNoTemplates = dlg.SkipIfNone;
                Settings.RemoveExcessPipes = dlg.RemoveExcessPipes;
                Settings.Parameters = dlg.Parameters;
                Settings.Replacements = dlg.Replacements;
                Settings.dlgWidth = dlg.Width;
                Settings.dlgHeight = dlg.Height;
                Settings.dlgCol0 = dlg.paramName.Width;
                Settings.dlgCol1 = dlg.paramRegex.Width;
                RegexString = ""; // force the main regex to be evaluated on next use
            }
        }
        void PluginEnabledCheckedChange(object o, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(o is ToolStripMenuItem);
            Settings.Enabled = (o as ToolStripMenuItem).Checked;
        }
        void AboutMenuItemClicked(object o, EventArgs e)
        {
            new AboutBox().ShowDialog();
        }

        public class TemplatorSettings
        {
            public bool Enabled = false;
            public bool SkipIfNoTemplates = false;
            public bool RemoveExcessPipes = false;
            public string TemplateName = "";
            public Dictionary<string, string> Parameters = new Dictionary<string, string>();
            public Dictionary<string, string> Replacements = new Dictionary<string, string>();
            public int dlgWidth;
            public int dlgHeight;
            public int dlgCol0;
            public int dlgCol1;
        }

        // A number of regular expressions are exmployed in this process
        // 1. To parse each whole template for the desired named parameters
        // Note: The string which generates the regex is used to determine whether
        //       the regex has been built for the current set of parameters.
        //       The string is cleared whenever the parameters are loaded or changed through the UI.
        private string RegexString = "";
        private Regex findParametersRegex = null;

        // 2. To extract the values from each parameter in accordance with the user's specifications
        private Regex paramReplacementRegex = null;

        // 3. To transform a template parameter name into a valid regex named group
        internal static Regex nonIdentiferCharsRegex = new Regex("[^a-z0-9_]", RegexOptions.Compiled);

        // 4. To ignore whitespace
        internal static Regex whiteSpaceRegex = new Regex(@"\s*", RegexOptions.Compiled);

        // 5. To remove spare pipes, either two together or a spare at the end
        // CAUTION: this would be acceptable in a wikitable
        internal static Regex ExcessPipe = new Regex(@"\|(?<losepipe>\s*[|}])", RegexOptions.Compiled);
        internal static string RemovePipeReplacement = @" ${losepipe}"; // adds a redundant space so the output length matches the input length

        // 6. To catch external links [http://foo.bar] or [http://foo.bar Baz]
        // but also tolerate borked usage such as [just something in square brackets]
        internal static Regex BorkedExternalLinks = new Regex(@"\[.*?\]", RegexOptions.Compiled);

        // 7. For removing all occurrences of the target parameters from the input text
        private Dictionary<string, Regex> paramRemovalRegexes = null;

        internal static WikiFunctions.Plugin.IAutoWikiBrowser AWB;
        internal static TemplatorSettings Settings = new TemplatorSettings();
        private static TemplatorSettings defaultSettings = new TemplatorSettings();

        static internal string PluginName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
        static internal string Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
    }
}
