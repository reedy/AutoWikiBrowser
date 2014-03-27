using AutoWikiBrowser.Plugins.Kingbotk;
using AutoWikiBrowser.Plugins.Kingbotk.Components;
using AutoWikiBrowser.Plugins.Kingbotk.ManualAssessments;
using AutoWikiBrowser.Plugins.Kingbotk.Plugins;
using My;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
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
	/// <remarks></remarks>
	// Fields here shouldn't need to be Shared, as there will only ever be one instance - created by AWB at startup
	public sealed class PluginManager : IAWBPlugin
	{
		private const string conMe = "Kingbotk Plugin Manager";
		// Regular expressions:

		private static readonly Regex ReqPhotoNoParamsRegex = new Regex(Constants.TemplatePrefix + "reqphoto\\s*\\}\\}\\s*", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.ExplicitCapture);
		// Plugins:
		static internal List<PluginBase> ActivePlugins = new List<PluginBase>();
		private static readonly Dictionary<string, PluginBase> Plugins = new Dictionary<string, PluginBase>();

		private Assessments AssessmentsObject;
		//AWB objects:
		static internal IAutoWikiBrowser AWBForm;

		static internal ToolStripStatusLabel StatusText = new ToolStripStatusLabel("Initialising plugin");
		// Menu items:
		private static ToolStripMenuItem withEventsField_AddGenericTemplateMenuItem = new ToolStripMenuItem("Add Generic Template");
		private static ToolStripMenuItem AddGenericTemplateMenuItem {
			get { return withEventsField_AddGenericTemplateMenuItem; }
		}
		private static ToolStripMenuItem withEventsField_MenuShowSettingsTabs = new ToolStripMenuItem("Show settings tabs");
		private static ToolStripMenuItem MenuShowSettingsTabs {
			get { return withEventsField_MenuShowSettingsTabs; }
			set {
				if (withEventsField_MenuShowSettingsTabs != null) {
					withEventsField_MenuShowSettingsTabs.Click -= MenuShowHide_Click;
				}
				withEventsField_MenuShowSettingsTabs = value;
				if (withEventsField_MenuShowSettingsTabs != null) {
					withEventsField_MenuShowSettingsTabs.Click += MenuShowHide_Click;
				}
			}

		}
		// Library state and shared objects:
		private static readonly TabPage KingbotkPluginTab = new TabPage("Plugin");

		private static PluginSettingsControl PluginSettings;
		// User settings:

		private static bool blnShowManualAssessmentsInstructions = true;
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
		private const string conShowHideTabsParm = "ShowHideTabs";
		private const string conShowManualAssessmentsInstructions = "ShowManualAssessmentsInstructions";
		private const string conGenericTemplatesCount = "GenericTemplatesCount";

		private const string conGenericTemplate = "GenericTemplate";
		// AWB interface:
		public string Name {
            get { return Constants.conAWBPluginName; }
		}
		public void Initialise(IAutoWikiBrowser sender)
		{
			// Store AWB object reference:
			AWBForm = sender;

			// Initialise our settings object:
			PluginSettings = new PluginSettingsControl();

			var _with1 = AWBForm;
			// Set up our UI objects:
			var _with2 = _with1.BotModeCheckbox;
			_with2.EnabledChanged += AWBBotModeCheckboxEnabledChangedHandler;
			_with2.CheckedChanged += AWBBotModeCheckboxCheckedChangeHandler;
			_with1.StatusStrip.Items.Insert(2, StatusText);
			StatusText.Margin = new Padding(50, 0, 50, 0);
			StatusText.BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Right;
			StatusText.BorderStyle = Border3DStyle.Etched;
			_with1.HelpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[]{
				new ToolStripSeparator(),
				PluginSettings.MenuHelp,
				PluginSettings.MenuAbout
			});

			// UI - addhandlers for Start/Stop/Diff/Preview/Save/Ignore buttons/form closing:
			AWBForm.Form.FormClosing += AWBClosingEventHandler;

			// Handle over events from AWB:
			AWBForm.StopButton.Click += StopButtonClickEventHandler;
			AWBForm.TheSession.StateChanged += EditorStatusChanged;
			AWBForm.TheSession.Aborted += EditorAborted;

			// Track Manual Assessment checkbox:
			PluginSettings.ManuallyAssessCheckBox.CheckedChanged += ManuallyAssessCheckBox_CheckChanged;

			// Tabs:
			KingbotkPluginTab.UseVisualStyleBackColor = true;
			KingbotkPluginTab.Controls.Add(PluginSettings);

			// Show/hide tabs menu:
			var _with3 = MenuShowSettingsTabs;
			_with3.CheckOnClick = true;
			_with3.Checked = true;
			AWBForm.ToolStripMenuGeneral.DropDownItems.Add(MenuShowSettingsTabs);

			// Add-Generic-Template menu:
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
			foreach (KeyValuePair<string, PluginBase> plugin in Plugins) {
				plugin.Value.Initialise();
			}

			// Add our menu items last:
			AWBForm.PluginsToolStripMenuItem.DropDownItems.Add(PluginSettings.PluginToolStripMenuItem);

			// Reset statusbar text:
			DefaultStatusText();
		}
		public void LoadSettings(object[] prefs)
		{
			if (prefs.Length > 0) {
				// Check if we're receiving an new type settings block (a serialized string)
				if (object.ReferenceEquals(prefs[0].GetType(), typeof(string))) {
					LoadSettingsNewWay(Convert.ToString(prefs[0]));
				}
			}
		}
		public string ProcessArticle(IAutoWikiBrowser sender, IProcessArticleEventArgs eventargs)
		{
			string res = null;

			var _with4 = eventargs;
			if (ActivePlugins.Count == 0)
				return _with4.ArticleText;

			Article TheArticle = null;
			int Namesp = _with4.NameSpaceKey;

			StatusText.Text = "Processing " + _with4.ArticleTitle;
			AWBForm.TraceManager.ProcessingArticle(_with4.ArticleTitle, Namesp);

			foreach (PluginBase p in ActivePlugins) {
				try {
					if (!p.IAmReady && p.IAmGeneric) {
						MessageBox.Show("The generic template plugin \"" + p.PluginShortName + "\"isn't properly configured.", "Can't start", MessageBoxButtons.OK, MessageBoxIcon.Error);
						StopAWB();
						goto SkipOrStop;
					}
				} catch {
					StopAWB();
					goto SkipOrStop;
				}
			}

			switch (Namesp) {
				case Namespace.Article:
					if (PluginSettings.ManuallyAssess) {
						if (_with4.Exists == Exists.Yes) {
							StatusText.Text += ": Click Preview to read the article; " + "click Save or Ignore to load the assessments form";
							AssessmentsObject.ProcessMainSpaceArticle(_with4.ArticleTitle);
							_with4.EditSummary = "Clean up";
							goto SkipOrStop;
						}
                        //FIXME
					    var es = _with4.EditSummary;
					    var with4Skip = _with4.Skip;
                        res = Skipping(ref es, "", SkipReason.ProcessingMainArticleDoesntExist, _with4.ArticleText, ref with4Skip);
					    _with4.EditSummary = es;
					    _with4.Skip = with4Skip;
					    goto ExitMe;
					}
			        goto SkipBadNamespace;

			        //break;
				case Namespace.Talk:
					AsyncApiEdit editor = AWBForm.TheSession.Editor.Clone();

					editor.Open(Tools.ConvertFromTalk(_with4.ArticleTitle), false);

					editor.Wait();

					if (!editor.Page.Exists)
					{
                        // FIXME
					    var with4ES = _with4.EditSummary;
					    var with4Skip = _with4.Skip;
					    res = Skipping(ref with4ES, "", SkipReason.ProcessingTalkPageArticleDoesntExist, _with4.ArticleText,
					        ref with4Skip, _with4.ArticleTitle);
					    _with4.EditSummary = with4ES;
					    _with4.Skip = with4Skip;
					} else {
						TheArticle = new Article(_with4.ArticleText, _with4.ArticleTitle, Namesp);

						bool ReqPhoto = ReqPhotoParamNeeded(TheArticle);

						if (PluginSettings.ManuallyAssess) {
							// reqphoto byref
							if (!AssessmentsObject.ProcessTalkPage(TheArticle, PluginSettings, ref ReqPhoto)) {
								_with4.Skip = true;
								goto SkipOrStop;
							}
						} else {
							ReqPhoto = ProcessTalkPageAndCheckWeAddedReqPhotoParam(TheArticle, ReqPhoto);
							// We successfully added a reqphoto param
						}

                        // FIXME
					    var with4Skip = _with4.Skip;
					    var with4ES = _with4.EditSummary;
                        res = FinaliseArticleProcessing(TheArticle, ref with4Skip, ref with4ES, _with4.ArticleText,
					        ReqPhoto);
					    _with4.Skip = with4Skip;
					    _with4.EditSummary = with4ES;
					}

					break;
				case Namespace.CategoryTalk:
				case 101:
				case Namespace.ProjectTalk:
				case Namespace.TemplateTalk:
				case Namespace.FileTalk:
					//101 is Portal Talk 
					if (PluginSettings.ManuallyAssess) {
						MessageBox.Show("The plugin has received a non-standard namespace talk page in " + "manual assessment mode. Please remove this item from the list and start again.", "Manual Assessments", MessageBoxButtons.OK, MessageBoxIcon.Error);
						StopAWB();
						goto SkipOrStop;
					} else {
						TheArticle = new Article(_with4.ArticleText, _with4.ArticleTitle, Namesp);

						foreach (PluginBase p in ActivePlugins) {
							p.ProcessTalkPage(TheArticle, Classification.Code, Importance.NA, false, false, false, ProcessTalkPageMode.NonStandardTalk, false);
							if (TheArticle.PluginManagerGetSkipResults == SkipResults.SkipBadTag)
								break; // TODO: might not be correct. Was : Exit For
						}

                        // FIXME
					    var with4Skip = _with4.Skip;
					    var with4ES = _with4.EditSummary;
					    res = FinaliseArticleProcessing(TheArticle, ref with4Skip, ref with4ES, _with4.ArticleText, false);
					    _with4.Skip = with4Skip;
					    _with4.EditSummary = with4ES;
					}

					break;
				default:
					goto SkipBadNamespace;
			}

			if (!_with4.Skip) {
				//TempHackInsteadOfDefaultSettings:
				if (AWBForm.EditSummaryComboBox.Text == "clean up")
					AWBForm.EditSummaryComboBox.Text = "Tagging";
			}
			ExitMe:

			if (!PluginSettings.ManuallyAssess)
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
			blnShowManualAssessmentsInstructions = true;
			var _with5 = PluginSettings;
			_with5.Reset();
			_with5.SkipBadTags = BotMode;
			_with5.SkipWhenNoChange = BotMode;
			foreach (KeyValuePair<string, PluginBase> plugin in Plugins) {
				plugin.Value.Reset();
			}
		}
		public object[] SaveSettings()
		{
			System.IO.StringWriter st = new System.IO.StringWriter();
			XmlTextWriter Writer = new XmlTextWriter(st);

			Writer.WriteStartElement("root");
			WriteXML(Writer);
			Writer.WriteEndElement();
			Writer.Flush();

			return new object[] { st.ToString() };
		}

		// Private routines:
		private static bool ProcessTalkPageAndCheckWeAddedReqPhotoParam(Article TheArticle, bool ReqPhoto)
		{
			bool res = false;
			foreach (PluginBase p in ActivePlugins) {
				if (p.ProcessTalkPage(TheArticle, ReqPhoto) && ReqPhoto && p.HasReqPhotoParam) {
					res = true;
				}

				if (TheArticle.PluginManagerGetSkipResults == SkipResults.SkipBadTag) {
					return false;
				}
			}

			return res;
		}
		private static bool ReqPhotoParamNeeded(Article TheArticle)
		{
			foreach (PluginBase p in ActivePlugins) {
				if (p.HasReqPhotoParam)
				{
				    if (ReqPhotoNoParamsRegex.IsMatch(TheArticle.AlteredArticleText))
						return true;
				}
			}
		    return false;
		}
		private static string FinaliseArticleProcessing(Article TheArticle, ref bool Skip, ref string Summary, string ArticleText, bool ReqPhoto)
		{

			SkipReason SkipReason = SkipReason.Other;

			if (TheArticle.PluginManagerGetSkipResults == SkipResults.NotSet) {
				PluginSettings.PluginStats.Tagged += 1;
			} else {
				var _with6 = PluginSettings.PluginStats;
				switch (TheArticle.PluginManagerGetSkipResults) {
					case SkipResults.SkipBadTag:
						// always skip
						if (PluginSettings.SkipBadTags) {
							_with6.SkippedBadTagIncrement();
							if (PluginSettings.OpenBadInBrowser)
								TheArticle.EditInBrowser();
							Skip = true;
							// always skip
							SkipReason = SkipReason.BadTag;
						} else {
							// the plugin manager stops processing when it gets a bad tag. We know however
							// that one plugin found a bad template and possibly replaced it with
							// conTemplatePlaceholder. We're also not skipping, so we need to remove the placeholder
                            TheArticle.AlteredArticleText = TheArticle.AlteredArticleText.Replace(Constants.conTemplatePlaceholder, "");
							MessageBox.Show("Bad tag. Please fix it manually or click ignore.", "Bad tag", MessageBoxButtons.OK, MessageBoxIcon.Warning);
							PluginSettings.PluginStats.Tagged += 1;
						}
						break;
					case SkipResults.SkipRegex:
					case SkipResults.SkipNoChange:
						if (TheArticle.ProcessIt) {
							PluginSettings.PluginStats.Tagged += 1;
						} else {
							if (TheArticle.PluginManagerGetSkipResults == SkipResults.SkipRegex) {
								_with6.SkippedMiscellaneousIncrement();
								Skip = true;
								SkipReason = SkipReason.Regex;
							// No change:
							} else {
								if (PluginSettings.SkipWhenNoChange) {
									_with6.SkippedNoChangeIncrement();
									SkipReason = SkipReason.NoChange;
									Skip = true;
								} else {
									PluginSettings.PluginStats.Tagged += 1;
									Skip = false;
								}
							}
						}
						break;
				}
			}

			if (Skip) {
				return Skipping(ref Summary, TheArticle.EditSummary, SkipReason, ArticleText, ref Skip);
			}
		    var _with7 = TheArticle;
		    if (ReqPhoto) {
		        _with7.AlteredArticleText = ReqPhotoNoParamsRegex.Replace(_with7.AlteredArticleText, "");
		        _with7.DoneReplacement("{{[[template:reqphoto|reqphoto]]}}", "template param(s)", true, PluginName);
		        _with7.ArticleHasAMajorChange();
		    }

		    _with7.FinaliseEditSummary();
		    Summary = _with7.EditSummary;
		    return _with7.AlteredArticleText;
		}
		private static string Skipping(ref string EditSummary, string DefaultEditSummary, SkipReason SkipReason, string ArticleText, ref bool Skip, string ArticleTitle = null, int NS = Namespace.Talk)
		{

			if (BotMode)
				EditSummary = "This article should have been skipped";
			else
				EditSummary = DefaultEditSummary;

			switch (SkipReason) {
				case SkipReason.BadNamespace:
					PluginSettings.PluginStats.SkippedNamespaceIncrement();
					AWBForm.TraceManager.SkippedArticle(PluginName, "Incorrect namespace");
					break;
				case SkipReason.ProcessingMainArticleDoesntExist:
					PluginSettings.PluginStats.SkippedRedLinkIncrement();
					AWBForm.TraceManager.SkippedArticle(PluginName, "Article doesn't exist");
					break;
				case SkipReason.ProcessingTalkPageArticleDoesntExist:
					PluginSettings.PluginStats.SkippedRedLinkIncrement();
					AWBForm.TraceManager.SkippedArticleRedlink(PluginName, ArticleTitle, NS);
					break;
				case SkipReason.BadTag:
					AWBForm.TraceManager.SkippedArticleBadTag(PluginName, ArticleTitle, NS);
					break;
				case SkipReason.NoChange:
					AWBForm.TraceManager.SkippedArticle(PluginName, "No change");
					break;
				case SkipReason.Regex:
					AWBForm.TraceManager.SkippedArticle(PluginName, "Article text matched a skip-if-found regular expression");
					break;
				case SkipReason.Other:
					AWBForm.TraceManager.SkippedArticle(PluginName, "");
					break;
			}

			Skip = true;
			return ArticleText;
		}
		private static void CreateNewGenericPlugin(string pluginName, string Creator)
		{
			GenericTemplatePlugin plugin = new GenericTemplatePlugin(pluginName);
			Plugins.Add(pluginName, plugin);
			plugin.Initialise();
			plugin.Enabled = true;
			// (adds it to activeplugins)
		}

		// Friend interface exposed to client plugins:
		static internal void ShowHidePluginTab(TabPage tabp, bool Visible)
		{
			if (Visible) {
				//If Not AWBForm.ContainsTabPage(tabp) Then
				bool ContainedMainTab = AWBForm.ContainsTabPage(KingbotkPluginTab);

				if (ContainedMainTab)
					AWBForm.RemoveTabPage(KingbotkPluginTab);
				AWBForm.AddTabPage(tabp);
				if (ContainedMainTab)
					AWBForm.AddTabPage(KingbotkPluginTab);
			//End If
			//If AWBForm.ContainsTabPage(tabp) Then
			} else {
				AWBForm.RemoveTabPage(tabp);
			}
		}
		static internal void PluginEnabledStateChanged(PluginBase Plugin, bool IsEnabled)
		{
			if (IsEnabled) {
				if (!ActivePlugins.Contains(Plugin)) {
					// WPBio must be last in list
					if (Plugin.PluginShortName == "Biography") {
						ActivePlugins.Add(Plugin);
					} else {
						ActivePlugins.Insert(0, Plugin);
					}

					if (ActivePlugins.Count == 1)
						AWBForm.AddTabPage(KingbotkPluginTab);

				}
			} else {
				ActivePlugins.Remove(Plugin);

				if (ActivePlugins.Count == 0)
					AWBForm.RemoveTabPage(KingbotkPluginTab);
			}
			DefaultStatusText();
		}
		static internal void StopAWB()
		{
            AWBForm.Stop(Constants.conAWBPluginName);
		}
		static internal void DeleteGenericPlugin(IGenericTemplatePlugin PG, PluginBase P)
		{
			Plugins.Remove(PG.GenericTemplateKey);
			if (ActivePlugins.Contains(P))
				ActivePlugins.Remove(P);
			if (ActivePlugins.Count == 0)
				AWBForm.RemoveTabPage(KingbotkPluginTab);
			DefaultStatusText();
		}

		// Friend static members:
		static internal bool BotMode {
			get { return AWBForm.BotModeCheckbox.Checked; }
		}
		static internal string PluginName {
			get { return conMe; }
		}
		static internal void EditBoxInsertYesParam(string ParameterName)
		{
			EditBoxInsert("|" + ParameterName + "=yes");
		}
		static internal void EditBoxInsert(string Text)
		{
			AWBForm.EditBox.SelectedText = Text;
		}

		// User interface management:
		private static bool ShowHideTabs {
			get { return MenuShowSettingsTabs.Checked; }
			set {
				if (value) {
					AWBForm.ShowAllTabPages();
					//For Each tabp As TabPage In SettingsTabs ' may not need this now AWB tracks tabs
					//    AWBForm.AddTabPage(tabp)
					//Next
					if (ActivePlugins.Count > 0)
						AWBForm.AddTabPage(KingbotkPluginTab);
				} else {
					AWBForm.HideAllTabPages();
					AWBForm.AddTabPage(KingbotkPluginTab);
				}
				MenuShowSettingsTabs.Checked = value;
			}
		}
		static internal void DefaultStatusText()
		{
			switch (ActivePlugins.Count) {
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
			if (PluginSettings.ManuallyAssess)
				StatusText.Text += " (manual assessments plugin active)";
		}
		static readonly Microsoft.VisualBasic.CompilerServices.StaticLocalInitFlag static_TestSkipNonExistingPages_WeCheckedSkipNonExistingPages_Init = new Microsoft.VisualBasic.CompilerServices.StaticLocalInitFlag();
		static bool static_TestSkipNonExistingPages_WeCheckedSkipNonExistingPages;
		static internal void TestSkipNonExistingPages()
		{
			lock (static_TestSkipNonExistingPages_WeCheckedSkipNonExistingPages_Init) {
				try {
					if (InitStaticVariableHelper(static_TestSkipNonExistingPages_WeCheckedSkipNonExistingPages_Init)) {
						static_TestSkipNonExistingPages_WeCheckedSkipNonExistingPages = false;
					}
				} finally {
					static_TestSkipNonExistingPages_WeCheckedSkipNonExistingPages_Init.State = 1;
				}
			}

			if (!static_TestSkipNonExistingPages_WeCheckedSkipNonExistingPages && ActivePlugins.Count > 0) {
				if (AWBForm.SkipNonExistentPages.Checked) {
					static_TestSkipNonExistingPages_WeCheckedSkipNonExistingPages = true;
					if (MessageBox.Show("The skip non existent pages checkbox is checked. This is not optimal for WikiProject tagging " + "as AWB will skip red-link talk pages. Please note that you will not receive this warning " + "again during this session, even if you load settings which have that box checked." + Microsoft.VisualBasic.Constants.vbCrLf + Microsoft.VisualBasic.Constants.vbCrLf + "Would you like the plugin to change this setting to false?", "Skip Non Existent Pages", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes) {
						AWBForm.SkipNonExistentPages.Checked = false;
					}
				}
			}
		}

		// Event handlers - AWB:
		private static void AWBClosingEventHandler(object sender, FormClosingEventArgs e)
		{
			if (e.Cancel) {
				return;
			}

			var _with8 = AWBForm.TraceManager;
			_with8.Flush();
			_with8.Close();
		}
		private static void AWBBotModeCheckboxCheckedChangeHandler(object sender, EventArgs e)
		{
			foreach (PluginBase p in ActivePlugins) {
				p.BotModeChanged(BotMode);
			}
		}
		private static void AWBBotModeCheckboxEnabledChangedHandler(object sender, EventArgs e)
		{
			if (AWBForm.BotModeCheckbox.Enabled && PluginSettings.ManuallyAssess) {
				AWBForm.BotModeCheckbox.Checked = false;
				AWBForm.BotModeCheckbox.Enabled = false;
			}
		}
		private static void EditorStatusChanged(AsyncApiEdit sender)
		{
			if (AWBForm.TheSession.Editor.IsActive) {
				if (ActivePlugins.Count > 0)
					PluginSettings.AWBProcessingStart(sender);
			} else {
				DefaultStatusText();
				// If AWB has stopped and the list is empty we assume the job is finished, so close the log and upload:
				if (AWBForm.ListMaker.Count == 0) {
					AWBForm.TraceManager.Close();
				}
			}
		}
		private static void EditorAborted(AsyncApiEdit sender)
		{
			PluginSettings.AWBProcessingAborted();
		}
		private void StopButtonClickEventHandler(object sender, EventArgs e)
		{
			DefaultStatusText();
			if ((AssessmentsObject != null))
				AssessmentsObject.Reset();
			PluginSettings.AWBProcessingAborted();
		}
		private static void MenuShowHide_Click(object sender, EventArgs e)
		{
			ShowHideTabs = MenuShowSettingsTabs.Checked;
		}
		private void ManuallyAssessCheckBox_CheckChanged(object sender, EventArgs e)
		{
			if (((CheckBox)sender).Checked) {
				StatusText.Text = "Initialising assessments plugin";

				if (AWBForm.TheSession.Editor.IsActive)
                    AWBForm.Stop(Constants.conAWBPluginName);

				if (blnShowManualAssessmentsInstructions) {
					AssessmentsInstructionsDialog dialog = new AssessmentsInstructionsDialog();

					blnShowManualAssessmentsInstructions = !(dialog.ShowDialog() == DialogResult.Yes);
				}

				AssessmentsObject = new Assessments(PluginSettings);

				DefaultStatusText();
			} else {
				AssessmentsObject.Dispose();
				AssessmentsObject = null;
			}
		}
		private static void AddGenericTemplateMenuItem_Click(object sender, EventArgs e)
		{
			string str = Microsoft.VisualBasic.Interaction.InputBox("Enter the name for this generic plugin").Trim();

			if (!string.IsNullOrEmpty(str)) {
				if (Plugins.ContainsKey(str)) {
					MessageBox.Show("A plugin of this name already exists", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return;
				} else if (Convert.ToBoolean(Microsoft.VisualBasic.Strings.InStr(str, " "))) {
					str = str.Replace(" ", "");
				}

				CreateNewGenericPlugin(str, "User");
			}
		}

		// XML:
		static internal bool XMLReadBoolean(XmlTextReader reader, string param, bool ExistingValue)
		{
			if (reader.MoveToAttribute(param))
				return bool.Parse(reader.Value);
			else
				return ExistingValue;
		}
		static internal string XMLReadString(XmlTextReader reader, string param, string ExistingValue)
		{
			if (reader.MoveToAttribute(param))
				return reader.Value.Trim();
			else
				return ExistingValue;
		}
		static internal int XMLReadInteger(XmlTextReader reader, string param, int ExistingValue)
		{
			if (reader.MoveToAttribute(param))
				return int.Parse(reader.Value);
			else
				return ExistingValue;
		}
		private static void ReadXML(XmlTextReader Reader)
		{
			blnShowManualAssessmentsInstructions = XMLReadBoolean(Reader, conShowManualAssessmentsInstructions, blnShowManualAssessmentsInstructions);
			// must happen BEFORE get ManualAssessment yes/no

			PluginSettings.ReadXML(Reader);

			int Count = XMLReadInteger(Reader, conGenericTemplatesCount, 0);
			if (Count > 0) {
				ReadGenericTemplatesFromXML(Count, Reader);
				// Must set up generic templates
			}
			//before reading in per-template properties, so that the new template receives a ReadXML() of its own

			foreach (KeyValuePair<string, PluginBase> plugin in Plugins) {
				plugin.Value.ReadXML(Reader);
				plugin.Value.ReadXMLRedirects(Reader);
			}

			bool blnNewVal = XMLReadBoolean(Reader, conShowHideTabsParm, ShowHideTabs);
			if (!(blnNewVal == ShowHideTabs))
				ShowHideTabs = blnNewVal;
			// Mustn't set if the same or we get extra tabs; must happen AFTER plugins

			TestSkipNonExistingPages();
		}
		private static void WriteXML(XmlTextWriter Writer)
		{
			System.Collections.Specialized.StringCollection strGenericTemplates = new System.Collections.Specialized.StringCollection();
			int i = 0;

			Writer.WriteAttributeString(conShowHideTabsParm, ShowHideTabs.ToString());
			Writer.WriteAttributeString(conShowManualAssessmentsInstructions, blnShowManualAssessmentsInstructions.ToString());
			PluginSettings.WriteXML(Writer);
			foreach (KeyValuePair<string, PluginBase> plugin in Plugins) {
				plugin.Value.WriteXML(Writer);
				plugin.Value.WriteXMLRedirects(Writer);
				if (plugin.Value.IAmGeneric) {
					strGenericTemplates.Add(((IGenericTemplatePlugin)plugin.Value).GenericTemplateKey);
				}
			}

			Writer.WriteAttributeString(conGenericTemplatesCount, strGenericTemplates.Count.ToString());

			foreach (string str in strGenericTemplates) {
				Writer.WriteAttributeString(conGenericTemplate + i.ToString(), str);
				i += 1;
			}
		}
		private static void LoadSettingsNewWay(string XMLString)
		{
			XMLString = XMLString.Replace("WikiProject Songs", "Songs");
			XMLString = XMLString.Replace("WikiProject Albums", "Albums");
			System.IO.StringReader st = new System.IO.StringReader(XMLString);
			XmlTextReader reader = new XmlTextReader(st);

			while (reader.Read()) {
				if (reader.NodeType == XmlNodeType.Element) {
					ReadXML(reader);
					break; // TODO: might not be correct. Was : Exit While
				}
			}
		}
		private static void ReadGenericTemplatesFromXML(int Count, XmlTextReader Reader)
		{
			string plugin = null;

			for (int i = 0; i <= Count - 1; i++) {
				plugin = XMLReadString(Reader, conGenericTemplate + i.ToString(), "").Trim();
				if (!Plugins.ContainsKey(plugin))
					CreateNewGenericPlugin(plugin, "ReadXML()");
			}
		}

		// AWB nudges:
		public void Nudge(out bool cancel)
		{
		    cancel = false;
			foreach (PluginBase p in ActivePlugins) {
				if (!p.IAmReady) {
					cancel = true;
					break; // TODO: might not be correct. Was : Exit For
				}
			}
		}
		public void Nudged(int nudges)
		{
			PluginSettings.lblAWBNudges.Text = "Nudges: " + nudges.ToString();
		}

		public string WikiName {
            get { return Constants.conWikiPlugin + " version " + AboutBox.Version; }
		}
		static bool InitStaticVariableHelper(Microsoft.VisualBasic.CompilerServices.StaticLocalInitFlag flag)
		{
			if (flag.State == 0) {
				flag.State = 2;
				return true;
			} else if (flag.State == 2) {
				throw new Microsoft.VisualBasic.CompilerServices.IncompleteInitialization();
			} else {
				return false;
			}
		}
	}
}
