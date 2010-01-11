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

// #define DEBUG_OUTPUT_DIALOG
#define SHORT_PLUGIN_MENU

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text.RegularExpressions;

// AutoWikiBrowser plugin support:
//using WikiFunctions;
using WikiFunctions;
using WikiFunctions.AWBSettings;
//using WikiFunctions.Parse;
//using WikiFunctions.Plugin;

namespace AutoWikiBrowser.Plugins.TheTemplator
{
    /// <summary>
    /// TheTemplator plugin for AutoWikiBrowser. Gives UI-assisted changes to portions of templates within articles.
    /// Current limitations:
    /// 1. Cannot handle wikitables used within templates
    ///    - it will stop and prompt the operator
    /// </summary>
    public class TheTemplator : WikiFunctions.Plugin.IAWBPlugin
    {
        private readonly ToolStripMenuItem pluginMenuItem = new ToolStripMenuItem("TheTemplator plugin");
        private readonly ToolStripMenuItem pluginConfigMenuItem = new ToolStripMenuItem("&Configuration...");
        private readonly ToolStripMenuItem aboutMenuItem2 = new ToolStripMenuItem("About TheTemplator plugin...");

#if !SHORT_PLUGIN_MENU
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

            pluginMenuItem.CheckedChanged += PluginEnabledCheckedChange;
            pluginConfigMenuItem.Click += ShowSettings;
            aboutMenuItem2.Click += AboutMenuItemClicked;

#if SHORT_PLUGIN_MENU
            pluginMenuItem.CheckOnClick = true;
            pluginMenuItem.DropDownItems.Add(pluginConfigMenuItem);
#else
            pluginEnabledMenuItem.CheckOnClick = true;
            pluginConfigMenuItem.Click += ShowSettings;
            pluginEnabledMenuItem.CheckedChanged += PluginEnabledCheckedChange;
            aboutMenuItem1.Click += AboutMenuItemClicked;

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
            List<PrefsKeyPair> settings = new List<PrefsKeyPair>
                                              {
                                                  new PrefsKeyPair("enabled", Settings.Enabled),
                                                  new PrefsKeyPair("xspipes", Settings.RemoveExcessPipes),
                                                  new PrefsKeyPair("skip", Settings.SkipIfNoTemplates),
                                                  new PrefsKeyPair("template", Settings.TemplateName),
                                                  new PrefsKeyPair("dlgwidth", Settings.dlgWidth),
                                                  new PrefsKeyPair("dlgheight", Settings.dlgHeight),
                                                  new PrefsKeyPair("dlgcol0", Settings.dlgCol0),
                                                  new PrefsKeyPair("dlgcol1", Settings.dlgCol1)
                                              };
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
        public void Nudge(out bool cancel)
        {
            cancel = false;
        }
        public void Nudged(int nudges)
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
#if DEBUG_OUTPUT_DIALOG
            DebugOutput dlg = new DebugOutput();
            dlg.Show(AWB.Form);
            dlg.AddSection("text", text);
#endif

            // Now apply our regex to each instance of the template in the article text
            foreach (Match match in matches)
            {
                string matchSegment = match.Value;

                /// Cannot handle wikitables used within templates
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

                // Determine the pattern for spacing, to minimise the whitespace changes
                string paramPipeStr = "|"; // gets replaced with one surrounded by whitespace if that's what the article uses
                string paramEqualsStr = "="; // gets replaced with one surrounded by whitespace if that's what the article uses
                int paramDesiredLength = 0;
                string trailingWhitespace = ""; // any whitespace after the value
                foreach (KeyValuePair<string, string> param in Settings.Parameters)
                {
                    string paramSegment = m.Groups["__" + param.Key].ToString();
                    if (paramSegment != "")
                    {
                        paramPipeStr = paramPipeRegex.Match(paramSegment).Value;
                        string paramPatternMatch = paramEqualsRegex.Match(paramSegment).Value;
                        paramDesiredLength = paramPatternMatch.Length + param.Key.Length;
                        paramEqualsStr = paramPatternMatch.PadLeft(paramDesiredLength);
                        string valueSegment = m.Groups["_" + param.Key].ToString();
                        if (valueSegment == "")
                            valueSegment = paramSegment;
                        trailingWhitespace = trailingWhiteSpaceRegex.Match(valueSegment).Value;
                        break;
                    }
                }

                // Build the segments of the original input string into one composite, in a specific order
                string paramSegments = "";
                foreach (KeyValuePair<string, string> param in Settings.Parameters)
                {
                    string paramSegment = m.Groups["__" + param.Key].ToString();
                    if (paramSegment == "")
                        paramSegments += "|" + param.Key + "=";
                    else
                        paramSegments += paramSegment + m.Groups["_" + param.Key];
                }
                // add a trailing pipe to assist in capturing all the white space
                paramSegments += "|";

                // Build the segments of the replacement values
                // Try to reproduce the whitespace style of the surrounding parameters if they are on separate lines
                string paramReplacementStr = "";
                foreach (KeyValuePair<string, string> param in Settings.Replacements)
                {
                    string equalsStr;
                    if (trailingWhitespace.Contains("\n"))
                    {
                        int spaceLeft = paramDesiredLength - param.Key.Length;
                        if (spaceLeft <= 0)
                        {
                            // This parameter name is wider than the space to the left of our reference parameter was
                            equalsStr = "=";
                        }
                        else
                        {
                            equalsStr = paramEqualsStr;
                            int indexOfEquals = equalsStr.IndexOf('=');
                            // Trim space from left as far as we can
                            equalsStr = equalsStr.Substring(Math.Min(indexOfEquals, paramDesiredLength - spaceLeft));
                            // Trim space from right if still needed
                            if (equalsStr.Length > spaceLeft)
                                equalsStr = equalsStr.Remove(spaceLeft);
                        }
                    }
                    else
                        equalsStr = paramEqualsStr;
                    paramReplacementStr += paramPipeStr + param.Key + equalsStr + param.Value + trailingWhitespace;
                }

                // Do the replacement
                string paramReplacement = paramReplacementRegex.Replace(paramSegments, paramReplacementStr);
                paramReplacement = paramReplacement.Remove(paramReplacement.Length - 1);

                // Check for only whitespace difference between old and new
                //TODO: re-check this:
                if (WikiRegexes.WhiteSpace.Replace(paramSegments, "") == WikiRegexes.WhiteSpace.Replace(paramReplacement, ""))
                    continue;

#if DEBUG_OUTPUT_DIALOG
                dlg.StartSection(string.Format("Match at {0}", match.Index));
                dlg.StartSection("Groups");
                for (int i = 0; i < findParametersRegex.GetGroupNumbers().Length; ++i)
                    dlg.AddSection("Group " + findParametersRegex.GetGroupNames()[i], m.Groups[i].ToString());
                dlg.EndSection();
                dlg.AddSection("paramSegments", paramSegments);
                dlg.AddSection("paramReplacementStr", paramReplacementStr);
                dlg.AddSection("paramReplacementRegex", paramReplacementRegex.ToString());
                dlg.AddSection("paramReplacement", paramReplacement);
#endif

                // Remove all occurrences of all of the parameters we're operating on.
                // Note the index of the first occurrence as the insertion point for the whole replacement group
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
                if (firstIndex == match.Length)
                {
                    // the parameters don't already exist, so
                    // a) there's no whitespace pattern on which to base our replacement
                    // b) there's no position for placing it
                    continue;
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

#if DEBUG_OUTPUT_DIALOG
                dlg.AddSection("text after substitution", text);

                dlg.EndSection();
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
                    // the pipe to introduce the parameter, and any more white space
                    = @"(?<__" + name + @">\|\s*"
                    // the parameter name, with any white space, and its equals sign
                    + param.Key + @"\s*=\s*)"
                    // assert that we got all the white space
                    + @"(?=\S)"
                    // We keep the whole of parameter value as a named match, but don't keep the components of it:
                    + @"(?<_" + name + ">("
                    // comments, HTML-type tags, etc:
                    + WikiRegexes.UnformattedText
                    + @"|"
                    // templates inside this template:
                    + WikiRegexes.NestedTemplates
                    + @"|"
                    // wikilinks in the parameter value:
                    + WikiRegexes.SimpleWikiLink
                    + @"|"
                    // external links, or just text in square brackets:
                    + BorkedExternalLinks
                    + @"|"
                    // anything else, like regular text and white space
                    + @"[^\[\{\|\}]*"
                    // Repeat the possible bits of the contents of the value
                    + @")*"
                    // Close the named group "_name"
                    + @")"
                    // Terminate at the white space before either a pipe or the closing brace
                    + @"(?=[\|\}])";
                RegexString += regexGroup + "|";
            }
            string rest
                = @"(\|\s*)"
                + @"(([^=\s]*)\s*=\s*(?=\S))?"
                + @"("
                + WikiRegexes.UnformattedText
                + @"|"
                + WikiRegexes.NestedTemplates
                + @"|"
                + WikiRegexes.SimpleWikiLink
                + @"|"
                + BorkedExternalLinks
                + @"|"
                + @"[^\[\{\|\}]*"
                + @")*(?=[\|\}])";
            RegexString = @"\{\{[^|]*" + ("(" + RegexString + rest + ")*") + @"\}\}";
            findParametersRegex = new Regex(RegexString, RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

            // paramRemovalRegexes
            paramRemovalRegexes = new Dictionary<string, Regex>(Settings.Parameters.Count);
            foreach (KeyValuePair<string, string> param in Settings.Parameters)
            {
                string paramRegexString
                        = @"(\|\s*"
                        + param.Key + @"\s*=\s*(?=\S)"
                        + @"("
                        + WikiRegexes.UnformattedText
                        + @"|"
                        + WikiRegexes.NestedTemplates
                        + @"|"
                        + WikiRegexes.SimpleWikiLink
                        + @"|"
                        + BorkedExternalLinks
                        + @"|"
                        + @"[^\[\{\|\}]*"
                        + @")*)(?=[\|\}])";
                paramRemovalRegexes[param.Key] = new Regex(paramRegexString, RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);
            }

            // paramReplacementRegex
            string paramMatcher = "";
            foreach (KeyValuePair<string, string> param in Settings.Parameters)
                paramMatcher += @"\|\s*" + param.Key + @"\s*=\s*(" + param.Value + @"\s*)?(?=\S)";
            paramReplacementRegex = new Regex(paramMatcher, RegexOptions.Compiled);
        }

        // Menu item handlers:
        void ShowSettings(object o, EventArgs e)
        {
            TemplatorConfig dlg = new TemplatorConfig(Settings.TemplateName,
                                                      Settings.Parameters,
                                                      Settings.Replacements,
                                                      Settings.SkipIfNoTemplates,
                                                      Settings.RemoveExcessPipes)
                                      {
                                          Width = Settings.dlgWidth,
                                          Height = Settings.dlgHeight
                                      };
            dlg.paramName.Width = dlg.replacementParamName.Width = Settings.dlgCol0;
            dlg.paramRegex.Width = dlg.replacementExpression.Width = Settings.dlgCol1;
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
        internal static readonly Regex nonIdentiferCharsRegex = new Regex("[^a-z0-9_]", RegexOptions.Compiled);

        // 5. To remove spare pipes, either two together or a spare at the end
        // CAUTION: this would be acceptable in a wikitable
        internal static readonly Regex ExcessPipe = new Regex(@"\|(?<losepipe>\s*[|}])", RegexOptions.Compiled);
        internal static readonly string RemovePipeReplacement = @" ${losepipe}"; // adds a redundant space so the output length matches the input length

        // 6. To catch external links [http://foo.bar] or [http://foo.bar Baz]
        // but also tolerate borked usage such as [just something in square brackets]
        internal static readonly Regex BorkedExternalLinks = new Regex(@"\[.*?\]", RegexOptions.Compiled);

        // 7. For removing all occurrences of the target parameters from the input text
        private Dictionary<string, Regex> paramRemovalRegexes = null;

        // 8. To extract the whitespace pattern for the leading pipe of replacement parameters
        internal static readonly Regex paramPipeRegex = new Regex(@"\s*\|\s*", RegexOptions.Compiled);

        // 9. To extract the whitespace pattern for the leading pipe of replacement parameters
        internal static readonly Regex paramEqualsRegex = new Regex(@"(?<=\w)\s*=\s*", RegexOptions.Compiled);

        // 10. To ignore whitespace
        internal static readonly Regex leadingWhiteSpaceRegex = new Regex(@"^\s*", RegexOptions.Compiled);

        // 11. To extract the whitespace pattern which follows the value
        internal static readonly Regex trailingWhiteSpaceRegex = new Regex(@"(?<=\S)\s*$", RegexOptions.Compiled | RegexOptions.Singleline);

        private static WikiFunctions.Plugin.IAutoWikiBrowser AWB;
        internal static TemplatorSettings Settings = new TemplatorSettings();
        private static readonly TemplatorSettings defaultSettings = new TemplatorSettings();

        private static readonly string PluginName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
        private static readonly string Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
    }
}
