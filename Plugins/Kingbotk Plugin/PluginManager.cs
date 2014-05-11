using System.Globalization;
using AutoWikiBrowser.Plugins.Kingbotk.Components;
using AutoWikiBrowser.Plugins.Kingbotk.ManualAssessments;
using AutoWikiBrowser.Plugins.Kingbotk.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using AutoWikiBrowser.Plugins.Kingbotk.WikiProjects;
using WikiFunctions;
using WikiFunctions.Plugin;
//Copyright © 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/
//Copyright © 2008 Sam Reed (Reedy) http://www.reedyboy.net/

//This program is free software; you can redistribute it and/or modify it under the terms of Version 2 of the GNU General Public License as published by the Free Software Foundation.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

//You should have received a copy of the GNU General Public License Version 2 along with this program; if not, write to the Free Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
using WikiFunctions.API;

[assembly: CLSCompliant(true)]

namespace AutoWikiBrowser.Plugins.Kingbotk
{
    /// <summary>
    /// The plugin manager, which interracts with AWB and manages the individual plugins
    /// </summary>
    // Fields here shouldn't need to be Shared, as there will only ever be one instance - created by AWB at startup
    public sealed class PluginManager : IAWBPlugin
    {
        private const string Me = "Kingbotk Plugin Manager";
        // Regular expressions:

        private static readonly Regex ReqPhotoNoParamsRegex =
            new Regex(Constants.TemplatePrefix + "reqphoto\\s*\\}\\}\\s*",
                RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        // Plugins:
        internal static List<PluginBase> ActivePlugins = new List<PluginBase>();
        private static readonly Dictionary<string, PluginBase> Plugins = new Dictionary<string, PluginBase>();

        private Assessments _assessmentsObject;
        //AWB objects:
        internal static IAutoWikiBrowser AWBForm;

        internal static ToolStripStatusLabel StatusText = new ToolStripStatusLabel("Initialising plugin");
        // Menu items:
        private static readonly ToolStripMenuItem AddGenericTemplateMenuItem =
            new ToolStripMenuItem("Add Generic Template");

        // Library state and shared objects:
        private static readonly TabPage KingbotkPluginTab = new TabPage("Plugin");

        private static PluginSettingsControl _pluginSettings;
        // User settings:

        private static bool _showManualAssessmentsInstructions = true;
        // SkipReason:
        private enum SkipReason : byte
        {
            Other,
            BadNamespace,
            BadTag,
            ProcessingMainArticleDoesntExist,
            ProcessingTalkPageArticleDoesntExist,
            NoChange,
            Regex
        }

        // XML:
        private const string ShowManualAssessmentsInstructions = "ShowManualAssessmentsInstructions";
        private const string GenericTemplatesCount = "GenericTemplatesCount";

        private const string GenericTemplate = "GenericTemplate";
        // AWB interface:
        public string Name
        {
            get { return Constants.AWBPluginName; }
        }

        public void Initialise(IAutoWikiBrowser sender)
        {
            // Store AWB object reference:
            AWBForm = sender;

            // Initialise our settings object:
            _pluginSettings = new PluginSettingsControl();


            // Set up our UI objects:

            AWBForm.BotModeCheckbox.EnabledChanged += AWBBotModeCheckboxEnabledChangedHandler;
            AWBForm.BotModeCheckbox.CheckedChanged += AWBBotModeCheckboxCheckedChangeHandler;
            AWBForm.StatusStrip.Items.Insert(2, StatusText);
            StatusText.Margin = new Padding(50, 0, 50, 0);
            StatusText.BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Right;
            StatusText.BorderStyle = Border3DStyle.Etched;
            AWBForm.HelpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[]
            {
                new ToolStripSeparator(),
                _pluginSettings.MenuHelp,
                _pluginSettings.MenuAbout
            });

            // UI - addhandlers for Start/Stop/Diff/Preview/Save/Ignore buttons/form closing:
            AWBForm.Form.FormClosing += AWBClosingEventHandler;

            // Handle over events from AWB:
            AWBForm.StopButton.Click += StopButtonClickEventHandler;
            AWBForm.TheSession.StateChanged += EditorStatusChanged;
            AWBForm.TheSession.Aborted += EditorAborted;

            // Track Manual Assessment checkbox:
            _pluginSettings.ManuallyAssessCheckBox.CheckedChanged += ManuallyAssessCheckBox_CheckChanged;

            // Tabs:
            KingbotkPluginTab.UseVisualStyleBackColor = true;
            KingbotkPluginTab.Controls.Add(_pluginSettings);

            // Add-Generic-Template menu:
            AddGenericTemplateMenuItem.Click += AddGenericTemplateMenuItem_Click;
            AWBForm.PluginsToolStripMenuItem.DropDownItems.Add(AddGenericTemplateMenuItem);

            // Create plugins:
            Plugins.Add("Albums", new WPAlbums());
            Plugins.Add("Australia", new WPAustralia());
            Plugins.Add("India", new WPIndia());
            Plugins.Add("MilHist", new WPMilitaryHistory());
            Plugins.Add("Songs", new WPSongs());
            Plugins.Add("WPNovels", new WPNovels());
            Plugins.Add("Biography", new WPBiography());
            // hopefully if add WPBio last it will ensure that the template gets added to the *top* of pages

            // Initialise plugins:
            foreach (KeyValuePair<string, PluginBase> plugin in Plugins)
            {
                plugin.Value.Initialise();
            }

            // Add our menu items last:
            AWBForm.PluginsToolStripMenuItem.DropDownItems.Add(_pluginSettings.PluginToolStripMenuItem);

            // Reset statusbar text:
            DefaultStatusText();
        }

        public void LoadSettings(object[] prefs)
        {
            if (prefs.Length > 0)
            {
                // Check if we're receiving an new type settings block (a serialized string)
                if (ReferenceEquals(prefs[0].GetType(), typeof (string)))
                {
                    LoadSettingsNewWay(Convert.ToString(prefs[0]));
                }
            }
        }

        public string ProcessArticle(IAutoWikiBrowser sender, IProcessArticleEventArgs eventargs)
        {
            string res;

            if (ActivePlugins.Count == 0)
                return eventargs.ArticleText;

            Article theArticle;

            StatusText.Text = "Processing " + eventargs.ArticleTitle;
            AWBForm.TraceManager.ProcessingArticle(eventargs.ArticleTitle, eventargs.NameSpaceKey);

            foreach (PluginBase p in ActivePlugins)
            {
                try
                {
                    if (!p.AmReady && p.AmGeneric)
                    {
                        MessageBox.Show(
                            "The generic template plugin \"" + p.PluginShortName + "\"isn't properly configured.",
                            "Can't start", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        StopAWB();
                        goto SkipOrStop;
                    }
                }
                catch
                {
                    StopAWB();
                    goto SkipOrStop;
                }
            }

            switch (eventargs.NameSpaceKey)
            {
                case Namespace.Article:
                    if (_pluginSettings.ManuallyAssess)
                    {
                        if (eventargs.Exists == Exists.Yes)
                        {
                            StatusText.Text += ": Click Preview to read the article; " +
                                               "click Save or Ignore to load the assessments form";
                            _assessmentsObject.ProcessMainSpaceArticle(eventargs.ArticleTitle);
                            eventargs.EditSummary = "Clean up";
                            goto SkipOrStop;
                        }
                        //FIXME
                        var eaArticleES = eventargs.EditSummary;
                        var eaArticleSkip = eventargs.Skip;
                        res = Skipping(ref eaArticleES, "", SkipReason.ProcessingMainArticleDoesntExist,
                            eventargs.ArticleText, ref eaArticleSkip);
                        eventargs.EditSummary = eaArticleES;
                        eventargs.Skip = eaArticleSkip;
                        goto ExitMe;
                    }
                    goto SkipBadNamespace;

                    //break;
                case Namespace.Talk:
                    AsyncApiEdit editor = AWBForm.TheSession.Editor.Clone();

                    editor.Open(Tools.ConvertFromTalk(eventargs.ArticleTitle), false);

                    editor.Wait();

                    if (!editor.Page.Exists)
                    {
                        // FIXME
                        var eaNotExistsES = eventargs.EditSummary;
                        var eaNotExistsSkip = eventargs.Skip;
                        res = Skipping(ref eaNotExistsES, "", SkipReason.ProcessingTalkPageArticleDoesntExist,
                            eventargs.ArticleText,
                            ref eaNotExistsSkip, eventargs.ArticleTitle);
                        eventargs.EditSummary = eaNotExistsES;
                        eventargs.Skip = eaNotExistsSkip;
                    }
                    else
                    {
                        theArticle = new Article(eventargs.ArticleText, eventargs.ArticleTitle, eventargs.NameSpaceKey);

                        bool reqPhoto = ReqPhotoParamNeeded(theArticle);

                        if (_pluginSettings.ManuallyAssess)
                        {
                            // reqphoto byref
                            if (!_assessmentsObject.ProcessTalkPage(theArticle, _pluginSettings, ref reqPhoto))
                            {
                                eventargs.Skip = true;
                                goto SkipOrStop;
                            }
                        }
                        else
                        {
                            reqPhoto = ProcessTalkPageAndCheckWeAddedReqPhotoParam(theArticle, reqPhoto);
                            // We successfully added a reqphoto param
                        }

                        // FIXME
                        var eaTalkSkip = eventargs.Skip;
                        var eaTalkES = eventargs.EditSummary;
                        res = FinaliseArticleProcessing(theArticle, ref eaTalkSkip, ref eaTalkES, eventargs.ArticleText,
                            reqPhoto);
                        eventargs.Skip = eaTalkSkip;
                        eventargs.EditSummary = eaTalkES;
                    }

                    break;

                case Namespace.CategoryTalk:
                case 101: //101 is Portal Talk 
                case Namespace.ProjectTalk:
                case Namespace.TemplateTalk:
                case Namespace.FileTalk:
                    if (_pluginSettings.ManuallyAssess)
                    {
                        MessageBox.Show(
                            "The plugin has received a non-standard namespace talk page in " +
                            "manual assessment mode. Please remove this item from the list and start again.",
                            "Manual Assessments", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        StopAWB();
                        goto SkipOrStop;
                    }
                    theArticle = new Article(eventargs.ArticleText, eventargs.ArticleTitle, eventargs.NameSpaceKey);

                    foreach (PluginBase p in ActivePlugins)
                    {
                        p.ProcessTalkPage(theArticle, Classification.Code, Importance.NA, false, false, false,
                            ProcessTalkPageMode.NonStandardTalk, false);
                        if (theArticle.PluginManagerGetSkipResults == SkipResults.SkipBadTag)
                            break; // TODO: might not be correct. Was : Exit For
                    }

                    // FIXME
                    var eaMiscSkip = eventargs.Skip;
                    var eaMiscES = eventargs.EditSummary;
                    res = FinaliseArticleProcessing(theArticle, ref eaMiscSkip, ref eaMiscES, eventargs.ArticleText,
                        false);
                    eventargs.Skip = eaMiscSkip;
                    eventargs.EditSummary = eaMiscES;

                    break;
                default:
                    goto SkipBadNamespace;
            }

            if (!eventargs.Skip)
            {
                //TempHackInsteadOfDefaultSettings:
                if (AWBForm.EditSummaryComboBox.Text == "clean up")
                    AWBForm.EditSummaryComboBox.Text = "Tagging";
            }
            ExitMe:

            if (!_pluginSettings.ManuallyAssess)
                DefaultStatusText();
            AWBForm.TraceManager.Flush();
            return res;
            SkipBadNamespace:

            //FIXME
            var eaES = eventargs.EditSummary;
            var eaSkip = eventargs.Skip;
            res = Skipping(ref eaES, "", SkipReason.BadNamespace, eventargs.ArticleText, ref eaSkip);
            eventargs.EditSummary = eaES;
            eventargs.Skip = eaSkip;
            goto ExitMe;
            SkipOrStop:

            res = eventargs.ArticleText;
            goto ExitMe;
        }

        public void Reset()
        {
            _showManualAssessmentsInstructions = true;
            _pluginSettings.Reset();
            _pluginSettings.SkipBadTags = BotMode;
            _pluginSettings.SkipWhenNoChange = BotMode;
            foreach (KeyValuePair<string, PluginBase> plugin in Plugins)
            {
                plugin.Value.Reset();
            }
        }

        public object[] SaveSettings()
        {
            System.IO.StringWriter st = new System.IO.StringWriter();
            XmlTextWriter writer = new XmlTextWriter(st);

            writer.WriteStartElement("root");
            WriteXML(writer);
            writer.WriteEndElement();
            writer.Flush();

            return new object[] {st.ToString()};
        }

        // Private routines:
        private static bool ProcessTalkPageAndCheckWeAddedReqPhotoParam(Article theArticle, bool reqPhoto)
        {
            bool res = false;
            foreach (PluginBase p in ActivePlugins)
            {
                if (p.ProcessTalkPage(theArticle, reqPhoto) && reqPhoto && p.HasReqPhotoParam)
                {
                    res = true;
                }

                if (theArticle.PluginManagerGetSkipResults == SkipResults.SkipBadTag)
                {
                    return false;
                }
            }

            return res;
        }

        private static bool ReqPhotoParamNeeded(Article theArticle)
        {
            return
                ActivePlugins.Where(p => p.HasReqPhotoParam)
                    .Any(p => ReqPhotoNoParamsRegex.IsMatch(theArticle.AlteredArticleText));
        }

        private static string FinaliseArticleProcessing(Article theArticle, ref bool skip, ref string summary,
            string articleText, bool reqPhoto)
        {
            SkipReason skipReason = SkipReason.Other;

            if (theArticle.PluginManagerGetSkipResults == SkipResults.NotSet)
            {
                _pluginSettings.PluginStats.Tagged += 1;
            }
            else
            {
                switch (theArticle.PluginManagerGetSkipResults)
                {
                    case SkipResults.SkipBadTag:
                        // always skip
                        if (_pluginSettings.SkipBadTags)
                        {
                            _pluginSettings.PluginStats.SkippedBadTagIncrement();
                            if (_pluginSettings.OpenBadInBrowser)
                                theArticle.EditInBrowser();
                            skip = true;
                            // always skip
                            skipReason = SkipReason.BadTag;
                        }
                        else
                        {
                            // the plugin manager stops processing when it gets a bad tag. We know however
                            // that one plugin found a bad template and possibly replaced it with
                            // conTemplatePlaceholder. We're also not skipping, so we need to remove the placeholder
                            theArticle.AlteredArticleText =
                                theArticle.AlteredArticleText.Replace(Constants.TemplaterPlaceholder, "");
                            MessageBox.Show("Bad tag. Please fix it manually or click ignore.", "Bad tag",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            _pluginSettings.PluginStats.Tagged += 1;
                        }
                        break;
                    case SkipResults.SkipRegex:
                    case SkipResults.SkipNoChange:
                        if (theArticle.ProcessIt)
                        {
                            _pluginSettings.PluginStats.Tagged += 1;
                        }
                        else
                        {
                            if (theArticle.PluginManagerGetSkipResults == SkipResults.SkipRegex)
                            {
                                _pluginSettings.PluginStats.SkippedMiscellaneousIncrement();
                                skip = true;
                                skipReason = SkipReason.Regex;
                                // No change:
                            }
                            else
                            {
                                if (_pluginSettings.SkipWhenNoChange)
                                {
                                    _pluginSettings.PluginStats.SkippedNoChangeIncrement();
                                    skipReason = SkipReason.NoChange;
                                    skip = true;
                                }
                                else
                                {
                                    _pluginSettings.PluginStats.Tagged += 1;
                                    skip = false;
                                }
                            }
                        }
                        break;
                }
            }

            if (skip)
            {
                return Skipping(ref summary, theArticle.EditSummary, skipReason, articleText, ref skip);
            }

            if (reqPhoto)
            {
                theArticle.AlteredArticleText = ReqPhotoNoParamsRegex.Replace(theArticle.AlteredArticleText, "");
                theArticle.DoneReplacement("{{reqphoto}}", "template param(s)");
                theArticle.ArticleHasAMajorChange();
            }

            theArticle.FinaliseEditSummary();
            summary = theArticle.EditSummary;
            return theArticle.AlteredArticleText;
        }

        private static string Skipping(ref string editSummary, string defaultEditSummary, SkipReason skipReason,
            string articleText, ref bool Skip, string articleTitle = null, int ns = Namespace.Talk)
        {
            editSummary = BotMode ? "This article should have been skipped" : defaultEditSummary;

            switch (skipReason)
            {
                case SkipReason.BadNamespace:
                    _pluginSettings.PluginStats.SkippedNamespaceIncrement();
                    AWBForm.TraceManager.SkippedArticle(PluginName, "Incorrect namespace");
                    break;
                case SkipReason.ProcessingMainArticleDoesntExist:
                    _pluginSettings.PluginStats.SkippedRedLinkIncrement();
                    AWBForm.TraceManager.SkippedArticle(PluginName, "Article doesn't exist");
                    break;
                case SkipReason.ProcessingTalkPageArticleDoesntExist:
                    _pluginSettings.PluginStats.SkippedRedLinkIncrement();
                    AWBForm.TraceManager.SkippedArticleRedlink(PluginName, articleTitle, ns);
                    break;
                case SkipReason.BadTag:
                    AWBForm.TraceManager.SkippedArticleBadTag(PluginName, articleTitle, ns);
                    break;
                case SkipReason.NoChange:
                    AWBForm.TraceManager.SkippedArticle(PluginName, "No change");
                    break;
                case SkipReason.Regex:
                    AWBForm.TraceManager.SkippedArticle(PluginName,
                        "Article text matched a skip-if-found regular expression");
                    break;
                case SkipReason.Other:
                    AWBForm.TraceManager.SkippedArticle(PluginName, "");
                    break;
            }

            Skip = true;
            return articleText;
        }

        private static void CreateNewGenericPlugin(string pluginName)
        {
            GenericTemplatePlugin plugin = new GenericTemplatePlugin(pluginName);
            Plugins.Add(pluginName, plugin);
            plugin.Initialise();
            plugin.Enabled = true;
            // (adds it to activeplugins)
        }

        // Friend interface exposed to client plugins:
        internal static void ShowHidePluginTab(TabPage tabp, bool visible)
        {
            if (visible)
            {
                //If Not AWBForm.ContainsTabPage(tabp) Then
                bool containedMainTab = AWBForm.ContainsTabPage(KingbotkPluginTab);

                if (containedMainTab)
                    AWBForm.RemoveTabPage(KingbotkPluginTab);
                AWBForm.AddTabPage(tabp);
                if (containedMainTab)
                    AWBForm.AddTabPage(KingbotkPluginTab);
                //End If
                //If AWBForm.ContainsTabPage(tabp) Then
            }
            else
            {
                AWBForm.RemoveTabPage(tabp);
            }
        }

        internal static void PluginEnabledStateChanged(PluginBase plugin, bool isEnabled)
        {
            if (isEnabled)
            {
                if (!ActivePlugins.Contains(plugin))
                {
                    // WPBio must be last in list
                    if (plugin.PluginShortName == "Biography")
                    {
                        ActivePlugins.Add(plugin);
                    }
                    else
                    {
                        ActivePlugins.Insert(0, plugin);
                    }

                    if (ActivePlugins.Count == 1)
                        AWBForm.AddTabPage(KingbotkPluginTab);
                }
            }
            else
            {
                ActivePlugins.Remove(plugin);

                if (ActivePlugins.Count == 0)
                    AWBForm.RemoveTabPage(KingbotkPluginTab);
            }
            DefaultStatusText();
        }

        internal static void StopAWB()
        {
            AWBForm.Stop(Constants.AWBPluginName);
        }

        internal static void DeleteGenericPlugin(IGenericTemplatePlugin pg, PluginBase p)
        {
            Plugins.Remove(pg.GenericTemplateKey);
            if (ActivePlugins.Contains(p))
                ActivePlugins.Remove(p);
            if (ActivePlugins.Count == 0)
                AWBForm.RemoveTabPage(KingbotkPluginTab);

            var plugin = p as GenericTemplatePlugin;
            if (plugin != null)
            {
                plugin.Dispose();
            }
            DefaultStatusText();
        }

        // Friend static members:
        internal static bool BotMode
        {
            get { return AWBForm.BotModeCheckbox.Checked; }
        }

        internal static string PluginName
        {
            get { return Me; }
        }

        internal static void EditBoxInsertYesParam(string parameterName)
        {
            EditBoxInsert("|" + parameterName + "=yes");
        }

        internal static void EditBoxInsert(string text)
        {
            AWBForm.EditBox.SelectedText = text;
        }

        // User interface management:

        internal static void DefaultStatusText()
        {
            switch (ActivePlugins.Count)
            {
                case 0:
                    StatusText.Text = "Kingbotk plugin manager ready";
                    break;
                case 1:
                    StatusText.Text = "Kingbotk plugin ready";
                    break;
                default:
                    StatusText.Text = ActivePlugins.Count.ToString("0 Kingbotk plugins ready");
                    break;
            }
            if (_pluginSettings.ManuallyAssess)
                StatusText.Text += " (manual assessments plugin active)";
        }

        // FIXME: To be removed
        private static readonly Microsoft.VisualBasic.CompilerServices.StaticLocalInitFlag
            static_TestSkipNonExistingPages_WeCheckedSkipNonExistingPages_Init =
                new Microsoft.VisualBasic.CompilerServices.StaticLocalInitFlag();

        private static bool static_TestSkipNonExistingPages_WeCheckedSkipNonExistingPages;

        internal static void TestSkipNonExistingPages()
        {
            lock (static_TestSkipNonExistingPages_WeCheckedSkipNonExistingPages_Init)
            {
                try
                {
                    if (InitStaticVariableHelper(static_TestSkipNonExistingPages_WeCheckedSkipNonExistingPages_Init))
                    {
                        static_TestSkipNonExistingPages_WeCheckedSkipNonExistingPages = false;
                    }
                }
                finally
                {
                    static_TestSkipNonExistingPages_WeCheckedSkipNonExistingPages_Init.State = 1;
                }
            }

            if (!static_TestSkipNonExistingPages_WeCheckedSkipNonExistingPages && ActivePlugins.Count > 0)
            {
                if (AWBForm.SkipNonExistentPages.Checked)
                {
                    static_TestSkipNonExistingPages_WeCheckedSkipNonExistingPages = true;
                    if (
                        MessageBox.Show(
                            "The skip non existent pages checkbox is checked. This is not optimal for WikiProject tagging " +
                            "as AWB will skip red-link talk pages. Please note that you will not receive this warning " +
                            "again during this session, even if you load settings which have that box checked." +
                            Microsoft.VisualBasic.Constants.vbCrLf + Microsoft.VisualBasic.Constants.vbCrLf +
                            "Would you like the plugin to change this setting to false?", "Skip Non Existent Pages",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) ==
                        DialogResult.Yes)
                    {
                        AWBForm.SkipNonExistentPages.Checked = false;
                    }
                }
            }
        }

        // Event handlers - AWB:
        private static void AWBClosingEventHandler(object sender, FormClosingEventArgs e)
        {
            if (e.Cancel)
            {
                return;
            }

            AWBForm.TraceManager.Flush();
            AWBForm.TraceManager.Close();
        }

        private static void AWBBotModeCheckboxCheckedChangeHandler(object sender, EventArgs e)
        {
            foreach (PluginBase p in ActivePlugins)
            {
                p.BotModeChanged(BotMode);
            }
        }

        private static void AWBBotModeCheckboxEnabledChangedHandler(object sender, EventArgs e)
        {
            if (AWBForm.BotModeCheckbox.Enabled && _pluginSettings.ManuallyAssess)
            {
                AWBForm.BotModeCheckbox.Checked = false;
                AWBForm.BotModeCheckbox.Enabled = false;
            }
        }

        private static void EditorStatusChanged(AsyncApiEdit sender)
        {
            if (AWBForm.TheSession.Editor.IsActive)
            {
                if (ActivePlugins.Count > 0)
                    _pluginSettings.AWBProcessingStart(sender);
            }
            else
            {
                DefaultStatusText();
                // If AWB has stopped and the list is empty we assume the job is finished, so close the log and upload:
                if (AWBForm.ListMaker.Count == 0)
                {
                    AWBForm.TraceManager.Close();
                }
            }
        }

        private static void EditorAborted(AsyncApiEdit sender)
        {
            _pluginSettings.AWBProcessingAborted();
        }

        private void StopButtonClickEventHandler(object sender, EventArgs e)
        {
            DefaultStatusText();
            if ((_assessmentsObject != null))
                _assessmentsObject.Reset();
            _pluginSettings.AWBProcessingAborted();
        }

        private void ManuallyAssessCheckBox_CheckChanged(object sender, EventArgs e)
        {
            if (((CheckBox) sender).Checked)
            {
                StatusText.Text = "Initialising assessments plugin";

                if (AWBForm.TheSession.Editor.IsActive)
                    AWBForm.Stop(Constants.AWBPluginName);

                if (_showManualAssessmentsInstructions)
                {
                    AssessmentsInstructionsDialog dialog = new AssessmentsInstructionsDialog();

                    _showManualAssessmentsInstructions = dialog.ShowDialog() != DialogResult.Yes;
                }

                _assessmentsObject = new Assessments(_pluginSettings);

                DefaultStatusText();
            }
            else
            {
                _assessmentsObject.Dispose();
                _assessmentsObject = null;
            }
        }

        private static void AddGenericTemplateMenuItem_Click(object sender, EventArgs e)
        {
            string str = Microsoft.VisualBasic.Interaction.InputBox("Enter the name for this generic plugin").Trim();

            if (!string.IsNullOrEmpty(str))
            {
                if (Plugins.ContainsKey(str))
                {
                    MessageBox.Show("A plugin of this name already exists", "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                    return;
                }
                if (str.Contains(" "))
                {
                    str = str.Replace(" ", "");
                }

                CreateNewGenericPlugin(str);
            }
        }

        // XML:
        internal static bool XMLReadBoolean(XmlTextReader reader, string param, bool existingValue)
        {
            if (reader.MoveToAttribute(param))
                return bool.Parse(reader.Value);
            return existingValue;
        }

        internal static string XMLReadString(XmlTextReader reader, string param, string existingValue)
        {
            if (reader.MoveToAttribute(param))
                return reader.Value.Trim();
            return existingValue;
        }

        internal static int XMLReadInteger(XmlTextReader reader, string param, int existingValue)
        {
            if (reader.MoveToAttribute(param))
                return int.Parse(reader.Value);
            return existingValue;
        }

        private static void ReadXML(XmlTextReader reader)
        {
            _showManualAssessmentsInstructions = XMLReadBoolean(reader, ShowManualAssessmentsInstructions,
                _showManualAssessmentsInstructions);
            // must happen BEFORE get ManualAssessment yes/no

            _pluginSettings.ReadXML(reader);

            int count = XMLReadInteger(reader, GenericTemplatesCount, 0);
            if (count > 0)
            {
                ReadGenericTemplatesFromXML(count, reader);
                // Must set up generic templates
            }
            //before reading in per-template properties, so that the new template receives a ReadXML() of its own

            foreach (KeyValuePair<string, PluginBase> plugin in Plugins)
            {
                plugin.Value.ReadXML(reader);
                plugin.Value.ReadXMLRedirects(reader);
            }

            TestSkipNonExistingPages();
        }

        private static void WriteXML(XmlTextWriter writer)
        {
            System.Collections.Specialized.StringCollection strGenericTemplates =
                new System.Collections.Specialized.StringCollection();
            int i = 0;

            writer.WriteAttributeString(ShowManualAssessmentsInstructions,
                _showManualAssessmentsInstructions.ToString());
            _pluginSettings.WriteXML(writer);
            foreach (KeyValuePair<string, PluginBase> plugin in Plugins)
            {
                plugin.Value.WriteXML(writer);
                plugin.Value.WriteXMLRedirects(writer);
                if (plugin.Value.AmGeneric)
                {
                    strGenericTemplates.Add(((IGenericTemplatePlugin) plugin.Value).GenericTemplateKey);
                }
            }

            writer.WriteAttributeString(GenericTemplatesCount, strGenericTemplates.Count.ToString(CultureInfo.InvariantCulture));

            foreach (string str in strGenericTemplates)
            {
                writer.WriteAttributeString(GenericTemplate + i, str);
                i++;
            }
        }

        private static void LoadSettingsNewWay(string xmlString)
        {
            xmlString = xmlString.Replace("WikiProject Songs", "Songs");
            xmlString = xmlString.Replace("WikiProject Albums", "Albums");
            System.IO.StringReader st = new System.IO.StringReader(xmlString);
            XmlTextReader reader = new XmlTextReader(st);

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    ReadXML(reader);
                    break;
                }
            }
        }

        private static void ReadGenericTemplatesFromXML(int count, XmlTextReader reader)
        {
            for (int i = 0; i <= count - 1; i++)
            {
                string plugin = XMLReadString(reader, GenericTemplate + i, "").Trim();
                if (!Plugins.ContainsKey(plugin))
                    CreateNewGenericPlugin(plugin);
            }
        }

        // AWB nudges:
        public void Nudge(out bool cancel)
        {
            cancel = ActivePlugins.Any(p => !p.AmReady);
        }

        public void Nudged(int nudges)
        {
            _pluginSettings.lblAWBNudges.Text = "Nudges: " + nudges;
        }

        public string WikiName
        {
            get { return Constants.WikiPlugin + " version " + AboutBox.Version; }
        }

        private static bool InitStaticVariableHelper(Microsoft.VisualBasic.CompilerServices.StaticLocalInitFlag flag)
        {
            if (flag.State == 0)
            {
                flag.State = 2;
                return true;
            }
            if (flag.State == 2)
            {
                throw new Microsoft.VisualBasic.CompilerServices.IncompleteInitialization();
            }
            return false;
        }
    }
}