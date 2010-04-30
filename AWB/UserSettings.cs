/*
Autowikibrowser
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
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using WikiFunctions;
using WikiFunctions.Plugin;
using WikiFunctions.AWBSettings;
using AutoWikiBrowser.Plugins;

namespace AutoWikiBrowser
{
    partial class MainForm
    {
        private void saveAsDefaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to save these settings as the default settings?", "Save as default?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                SavePrefs();
        }

        private void saveSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(SettingsFile))
            {
                if (File.Exists(SettingsFile))
                {
                    if (MessageBox.Show("Replace existing file?", "File exists - " + SettingsFile,
                                        MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                                        MessageBoxDefaultButton.Button1) == DialogResult.No)
                        return;

                    //Make an "old"/backup copy of a file. Old settings are still there if something goes wrong
                    File.Copy(SettingsFile, SettingsFile + ".old", true);
                }

                SavePrefs(SettingsFile);
            }
            else if (
                MessageBox.Show("No settings file currently loaded. Save as Default?",
                                "Save current settings as Default?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                SavePrefs();
            else
            {
                saveSettingsAsToolStripMenuItem_Click(null, null);
            }
        }

        private void loadSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadSettingsDialog();
        }

        private void loadDefaultSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Would you really like to load the original default settings?", "Reset settings to default?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                ResetSettings();
        }

        private void saveSettingsAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SettingsFile != AwbDirs.DefaultSettings)
                saveXML.FileName = SettingsFile;

            if (saveXML.ShowDialog() != DialogResult.OK)
                return;

            SavePrefs(saveXML.FileName);
            SettingsFile = saveXML.FileName;
        }

        /// <summary>
        /// Resets settings to Setting Class defaults
        /// </summary>
        private void ResetSettings()
        {
            try
            {
                LoadPrefs(new UserPrefs());

                try
                {
                    foreach (KeyValuePair<string, IAWBPlugin> a in Plugin.AWBPlugins)
                        a.Value.Reset();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Problem resetting plugin\r\n\r\n" + ex.Message);
                }

                CModule.ModuleEnabled = false;
                Text = Program.Name;
                StatusLabelText = "Default settings loaded.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error loading settings", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadDefaultEditSummaries()
        {
            //cmboEditSummary.Items.Clear();
            cmboEditSummary.Items.Add("clean up");
            cmboEditSummary.Items.Add("re-categorisation per [[WP:CFD|CFD]]");
            cmboEditSummary.Items.Add("clean up and re-categorisation per [[WP:CFD|CFD]]");
            cmboEditSummary.Items.Add("removing category per [[WP:CFD|CFD]]");
            cmboEditSummary.Items.Add("[[Wikipedia:Template substitution|subst:'ing]]");
            cmboEditSummary.Items.Add("[[Wikipedia:WikiProject Stub sorting|stub sorting]]");
            cmboEditSummary.Items.Add("[[WP:AWB/T|Typo fixing]]");
            cmboEditSummary.Items.Add("bad link repair");
            cmboEditSummary.Items.Add("Fixing [[Wikipedia:Disambiguation pages with links|links to disambiguation pages]]");
            cmboEditSummary.Items.Add("Unicodifying");
        }

        private void LoadSettingsDialog()
        {
            if (openXML.ShowDialog() != DialogResult.OK)
                return;

            LoadPrefs(openXML.FileName);
            SettingsFile = openXML.FileName;

            if(removeDuplicatesToolStripMenuItem.Checked)
                listMaker.RemoveListDuplicates();
        }

        private void LoadRecentSettingsList()
        {
            SplashScreen.SetProgress(63);
            try
            {
                UpdateRecentList(RegistryUtils.GetValue("\\RecentList", "").Split('|'));
            }
            catch { return; }
            finally
            {
                SplashScreen.SetProgress(66);
            }
        }

        private void UpdateRecentList(IEnumerable<string> list)
        {
            RecentList.Clear();
            foreach (string s in list)
            {
                if (!string.IsNullOrEmpty(s.Trim())) RecentList.Add(s);
            }
            UpdateRecentSettingsMenu();
        }

        private void UpdateRecentList(string path)
        {
            RecentList.Remove(path);
            RecentList.Insert(0, path);

            UpdateRecentSettingsMenu();
        }

        private void FixupObsoleteRecentSettings()
        {
            RecentList.Remove("Default.xml");
            RecentList.RemoveAll(x => string.Compare(x, "Default.xml", true) == 0
                || string.Compare(x, AwbDirs.DefaultSettings, true) == 0);

            while (RecentList.Count > 5)
                RecentList.RemoveAt(5);
        }

        private void UpdateRecentSettingsMenu()
        {
            FixupObsoleteRecentSettings();

            recentToolStripMenuItem.DropDown.Items.Clear();

            var item = recentToolStripMenuItem.DropDownItems.Add("Default settings");
            item.Click += DefaultSettingsClick;

            if (RecentList.Count > 0) recentToolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());
            foreach (string filename in RecentList)
            {
                item = recentToolStripMenuItem.DropDownItems.Add(filename);
                item.Click += RecentSettingsClick;
            }

            recentToolStripMenuItem.Visible = (RecentList.Count > 0);
        }

        private void SaveRecentSettingsList()
        {
            RegistryUtils.SetValue("", "RecentList", string.Join("|", RecentList.ToArray()));
        }

        private void RecentSettingsClick(object sender, EventArgs e)
        {
            ToolStripItem item = (sender as ToolStripItem);

            if (item != null)
                LoadPrefs(item.Text);
        }

        private void DefaultSettingsClick(object sender, EventArgs e)
        {
            LoadPrefs(AwbDirs.DefaultSettings);
        }

        /// <summary>
        /// Save preferences as default
        /// </summary>
        private void SavePrefs()
        {
            SavePrefs(AwbDirs.DefaultSettings);
        }

        /// <summary>
        /// Save preferences to file
        /// </summary>
        private void SavePrefs(string path)
        {
            try
            {
                UserPrefs.SavePrefs(MakePrefs(), path);

                UpdateRecentList(path);
                SettingsFile = path;

                //Delete temporary/old file if exists when code reaches here
                if (File.Exists(SettingsFile + ".old"))
                    File.Delete(SettingsFile + ".old");
            }
            catch (Exception ex)
            {
                // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#UnauthorizedAccessException_-_Default_settings_should_not_always_save_to_.25SYSTEMROOT.25.5Csystem32_for_UAC_reason
                // if user runs AWB from somewhere they can't write to, saving settings as default will fail, so handle this
                if (ex is UnauthorizedAccessException)
                {
                    MessageBox.Show("Saving settings failed due to insufficient permissions.", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                    ErrorHandler.Handle(ex);

                // don't attempt to write to disk if the error was IOException (disk full etc.)
                if (!(ex is IOException) && File.Exists(SettingsFile + ".old"))
                    File.Copy(SettingsFile + ".old", SettingsFile, true);
            }
        }

        /// <summary>
        /// Make preferences object from current settings
        /// </summary>
        private UserPrefs MakePrefs()
        {
            return new UserPrefs(

                new FaRPrefs(FindAndReplace, RplcSpecial, SubstTemplates)
                    {
                        Enabled = chkFindandReplace.Checked,
                    },

                new EditPrefs
                    {
                        GeneralFixes = chkGeneralFixes.Checked,
                        Tagger = chkAutoTagger.Checked,
                        Unicodify = chkUnicodifyWhole.Checked,
                        Recategorisation = cmboCategorise.SelectedIndex,
                        NewCategory = txtNewCategory.Text,
                        NewCategory2 = txtNewCategory2.Text,
                        ReImage = cmboImages.SelectedIndex,
                        ImageFind = txtImageReplace.Text,
                        Replace = txtImageWith.Text,
                        SkipIfNoCatChange = chkSkipNoCatChange.Checked,
                        RemoveSortKey = chkRemoveSortKey.Checked,
                        SkipIfNoImgChange = chkSkipNoImgChange.Checked,
                        AppendText = chkAppend.Checked,
                        Append = !rdoPrepend.Checked,
                        Text = txtAppendMessage.Text,
                        Newlines = (int) udNewlineChars.Value,
                        AutoDelay = (int) nudBotSpeed.Value,
                        BotMaxEdits = (int) botEditsStop.Value,
                        SupressTag = chkSuppressTag.Checked,
                        RegexTypoFix = chkRegExTypo.Checked
                    },

                new ListPrefs(listMaker, _saveArticleList),

                new SkipPrefs
                    {
                        SkipNonexistent = radSkipNonExistent.Checked,
                        Skipexistent = radSkipExistent.Checked,
                        SkipWhenNoChanges = chkSkipNoChanges.Checked,
                        SkipSpamFilterBlocked = chkSkipSpamFilter.Checked,
                        SkipInuse = chkSkipIfInuse.Checked,
                        SkipDoes = chkSkipIfContains.Checked,
                        SkipDoesNot = chkSkipIfNotContains.Checked,
                        SkipDoesText = txtSkipIfContains.Text,
                        SkipDoesNotText = txtSkipIfNotContains.Text,
                        Regex = chkSkipIsRegex.Checked,
                        CaseSensitive = chkSkipCaseSensitive.Checked,
                        AfterProcessing = chkSkipAfterProcessing.Checked,
                        SkipNoFindAndReplace = chkSkipWhenNoFAR.Checked,
                        SkipMinorFindAndReplace = chkSkipOnlyMinorFaR.Checked,
                        SkipNoRegexTypoFix = chkSkipIfNoRegexTypo.Checked,
                        SkipNoDisambiguation = chkSkipNoDab.Checked,
                        GeneralSkipList = Skip.SelectedItems,
                        SkipWhenOnlyWhitespaceChanged = chkSkipWhitespace.Checked,
                        SkipOnlyCasingChanged = chkSkipCasing.Checked,
                        SkipOnlyGeneralFixChanges = chkSkipGeneralFixes.Checked,
                        SkipOnlyMinorGeneralFixChanges = chkSkipMinorGeneralFixes.Checked,
                        SkipNoLinksOnPage = chkSkipNoPageLinks.Checked,
                        SkipIfRedirect = chkSkipIfRedirect.Checked,
                        SkipIfNoAlerts = chkSkipIfNoAlerts.Checked
                    },

                new GeneralPrefs(cmboEditSummary.Items)
                    {
                        SaveArticleList = _saveArticleList,
                        IgnoreNoBots = IgnoreNoBots,
                        ClearPageListOnProjectChange = ClearPageListOnProjectChange,
                        SelectedSummary = cmboEditSummary.Text,

                        PasteMore = new[]
                                        {
                                            (string) PasteMore1.Tag,
                                            (string) PasteMore2.Tag,
                                            (string) PasteMore3.Tag,
                                            (string) PasteMore4.Tag,
                                            (string) PasteMore5.Tag,
                                            (string) PasteMore6.Tag,
                                            (string) PasteMore7.Tag,
                                            (string) PasteMore8.Tag,
                                            (string) PasteMore9.Tag,
                                            (string) PasteMore10.Tag
                                        },
                        FindText = txtFind.Text,
                        FindRegex = chkFindRegex.Checked,
                        FindCaseSensitive = chkFindCaseSensitive.Checked,
                        WordWrap = wordWrapToolStripMenuItem1.Checked,
                        ToolBarEnabled = EnableToolBar,
                        BypassRedirect = bypassRedirectsToolStripMenuItem.Checked,
                        AutoSaveSettings = autoSaveSettingsToolStripMenuItem.Checked,
                        PreParseMode = preParseModeToolStripMenuItem.Checked,
                        noSectionEditSummary = noSectionEditSummaryToolStripMenuItem.Checked,
                        restrictDefaultsortAddition = restrictDefaultsortChangesToolStripMenuItem.Checked,
                        restrictOrphanTagging = restrictOrphanTaggingToolStripMenuItem.Checked,
                        noMOSComplianceFixes = noMOSComplianceFixesToolStripMenuItem.Checked,
                        syntaxHighlightEditBox = syntaxHighlightEditBoxToolStripMenuItem.Checked,
                        highlightAllFind = highlightAllFindToolStripMenuItem.Checked,
                        NoAutoChanges = !automaticallyDoAnythingToolStripMenuItem.Checked,
                        OnLoadAction = actionOnLoad,
                        DiffInBotMode = doDiffInBotMode,
                        Minor = chkMinor.Checked,
                        AddToWatchlist = addToWatchList.SelectedIndex,
                        TimerEnabled = ShowMovingAverageTimer,
                        SortListAlphabetically = sortAlphabeticallyToolStripMenuItem.Checked,
                        AddIgnoredToLog = Article.AddUsingAWBOnArticleAction,
                        TextBoxSize = (int) txtEdit.Font.Size,
                        TextBoxFont = txtEdit.Font.Name,
                        LowThreadPriority = LowThreadPriority,
                        Beep = _beep,
                        Flash = _flash,
                        Minimize = _minimize,
                        AutoSaveEdit = new EditBoxAutoSavePrefs
                                           {
                                               Enabled = _autoSaveEditBoxEnabled,
                                               SavePeriod = AutoSaveEditBoxPeriod,
                                               SaveFile = _autoSaveEditBoxFile
                                           },
                        LockSummary = chkLock.Checked,
                        EditToolbarEnabled = EditToolBarVisible,
                        SuppressUsingAWB = _suppressUsingAWB,
                        AddUsingAWBToActionSummaries = Article.AddUsingAWBOnArticleAction,
                        filterNonMainSpace = filterOutNonMainSpaceToolStripMenuItem.Checked,
                        AutoFilterDuplicates = removeDuplicatesToolStripMenuItem.Checked,
                        FocusAtEndOfEditBox = focusAtEndOfEditTextBoxToolStripMenuItem.Checked,
                        scrollToUnbalancedBrackets = scrollToUnbalancedBracketsToolStripMenuItem.Checked,

                        SortInterWikiOrder = alphaSortInterwikiLinksToolStripMenuItem.Checked,
                        ReplaceReferenceTags = replaceReferenceTagsToolStripMenuItem.Checked,
                        LoggingEnabled = loggingEnabled
                    },

                new DabPrefs
                    {
                        Enabled = chkEnableDab.Checked,
                        Link = txtDabLink.Text,
                        Variants = txtDabVariants.Lines,
                        ContextChars = (int) udContextChars.Value
                    },

                new ModulePrefs
                    {
                        Enabled = CModule.ModuleEnabled,
                        Language = CModule.Language,
                        Code = CModule.Code
                    },

                ExtProgram.Settings,
                loggingSettings1.SerialisableSettings,
                listMaker.SpecialFilterSettings,

                new ToolsPrefs
                    {
                        ListComparerUseCurrentArticleList = _listComparerUseCurrentArticleList,
                        ListSplitterUseCurrentArticleList = _listSplitterUseCurrentArticleList,
                        DatabaseScannerUseCurrentArticleList = _dbScannerUseCurrentArticleList
                    },

                Plugin.AWBPlugins
                );
        }

        /// <summary>
        /// Load default preferences
        /// </summary>
        private void LoadPrefs()
        {
            SplashScreen.SetProgress(50);

            if (!string.IsNullOrEmpty(SettingsFile))
                LoadPrefs(SettingsFile);
            else
                if (File.Exists(AwbDirs.DefaultSettings))
                    LoadPrefs(AwbDirs.DefaultSettings);
                else
                {
                    LoadPrefs(new UserPrefs());
                    SettingsFile = "";
                }

            SplashScreen.SetProgress(59);
        }

        /// <summary>
        /// Load preferences from file
        /// </summary>
        private void LoadPrefs(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            try
            {
                LoadPrefs(UserPrefs.LoadPrefs(path));

                SettingsFile = path;
                StatusLabelText = "Settings successfully loaded";
                UpdateRecentList(path);
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }

            SettingsFile = path;
            
            if(removeDuplicatesToolStripMenuItem.Checked)
                listMaker.RemoveListDuplicates();
        }

        /// <summary>
        /// Load preferences object
        /// </summary>
        private void LoadPrefs(UserPrefs p)
        {
            SetProject(p.LanguageCode, p.Project, p.CustomProject);

            Variables.PHP5 = p.PHP5;

            FindAndReplace.Clear();
            chkFindandReplace.Checked = p.FindAndReplace.Enabled;
            FindAndReplace.IgnoreLinks = p.FindAndReplace.IgnoreSomeText;
            FindAndReplace.IgnoreMore = p.FindAndReplace.IgnoreMoreText;
            FindAndReplace.AppendToSummary = p.FindAndReplace.AppendSummary;
            FindAndReplace.AfterOtherFixes = p.FindAndReplace.AfterOtherFixes;
            FindAndReplace.AddNew(p.FindAndReplace.Replacements);

            RplcSpecial.Clear();
            RplcSpecial.AddNewRule(p.FindAndReplace.AdvancedReps);

            SubstTemplates.Clear();
            SubstTemplates.TemplateList = p.FindAndReplace.SubstTemplates;
            SubstTemplates.ExpandRecursively = p.FindAndReplace.ExpandRecursively;
            SubstTemplates.IgnoreUnformatted = p.FindAndReplace.IgnoreUnformatted;
            SubstTemplates.IncludeComments = p.FindAndReplace.IncludeComments;

            FindAndReplace.MakeList();

            listMaker.SourceText = p.List.ListSource;
            listMaker.SelectedProvider = p.List.SelectedProvider;

            _saveArticleList = p.General.SaveArticleList;

            IgnoreNoBots = p.General.IgnoreNoBots;
            ClearPageListOnProjectChange = p.General.ClearPageListOnProjectChange;

            listMaker.Add(p.List.ArticleList);

            chkGeneralFixes.Checked = p.Editprefs.GeneralFixes;
            chkAutoTagger.Checked = p.Editprefs.Tagger;
            chkUnicodifyWhole.Checked = p.Editprefs.Unicodify;

            cmboCategorise.SelectedIndex = p.Editprefs.Recategorisation;
            txtNewCategory.Text = p.Editprefs.NewCategory;
            txtNewCategory2.Text = p.Editprefs.NewCategory2;

            cmboImages.SelectedIndex = p.Editprefs.ReImage;
            txtImageReplace.Text = p.Editprefs.ImageFind;
            txtImageWith.Text = p.Editprefs.Replace;

            chkSkipNoCatChange.Checked = p.Editprefs.SkipIfNoCatChange;
            chkRemoveSortKey.Checked = p.Editprefs.RemoveSortKey;
            chkSkipNoImgChange.Checked = p.Editprefs.SkipIfNoImgChange;

            chkAppend.Checked = p.Editprefs.AppendText;
            rdoAppend.Checked = p.Editprefs.Append;
            rdoPrepend.Checked = !p.Editprefs.Append;
            txtAppendMessage.Text = p.Editprefs.Text;
            udNewlineChars.Value = p.Editprefs.Newlines;

            nudBotSpeed.Value = p.Editprefs.AutoDelay;
            botEditsStop.Value = p.Editprefs.BotMaxEdits;
            chkSuppressTag.Checked = p.Editprefs.SupressTag;

            chkRegExTypo.Checked = p.Editprefs.RegexTypoFix;

            radSkipNonExistent.Checked = p.SkipOptions.SkipNonexistent;
            radSkipExistent.Checked = p.SkipOptions.Skipexistent;
            chkSkipNoChanges.Checked = p.SkipOptions.SkipWhenNoChanges;
            chkSkipSpamFilter.Checked = p.SkipOptions.SkipSpamFilterBlocked;
            chkSkipIfInuse.Checked = p.SkipOptions.SkipInuse;
            chkSkipWhitespace.Checked = p.SkipOptions.SkipWhenOnlyWhitespaceChanged;
            chkSkipCasing.Checked = p.SkipOptions.SkipOnlyCasingChanged;
            chkSkipGeneralFixes.Checked = p.SkipOptions.SkipOnlyGeneralFixChanges;
            chkSkipMinorGeneralFixes.Checked = p.SkipOptions.SkipOnlyMinorGeneralFixChanges;
            chkSkipIfRedirect.Checked = p.SkipOptions.SkipIfRedirect;
            chkSkipIfNoAlerts.Checked = p.SkipOptions.SkipIfNoAlerts;

            chkSkipIfContains.Checked = p.SkipOptions.SkipDoes;
            chkSkipIfNotContains.Checked = p.SkipOptions.SkipDoesNot;

            txtSkipIfContains.Text = p.SkipOptions.SkipDoesText;
            txtSkipIfNotContains.Text = p.SkipOptions.SkipDoesNotText;

            chkSkipIsRegex.Checked = p.SkipOptions.Regex;
            chkSkipCaseSensitive.Checked = p.SkipOptions.CaseSensitive;
            chkSkipAfterProcessing.Checked = p.SkipOptions.AfterProcessing;

            chkSkipWhenNoFAR.Checked = p.SkipOptions.SkipNoFindAndReplace;
            chkSkipOnlyMinorFaR.Checked = p.SkipOptions.SkipMinorFindAndReplace;
            chkSkipIfNoRegexTypo.Checked = p.SkipOptions.SkipNoRegexTypoFix;
            Skip.SelectedItems = p.SkipOptions.GeneralSkipList;
            chkSkipNoDab.Checked = p.SkipOptions.SkipNoDisambiguation;
            chkSkipNoPageLinks.Checked = p.SkipOptions.SkipNoLinksOnPage;

            cmboEditSummary.Items.Clear();

            if (p.General.Summaries.Count == 0)
                LoadDefaultEditSummaries();
            else
                foreach (string s in p.General.Summaries)
                    cmboEditSummary.Items.Add(s);

            chkLock.Checked = p.General.LockSummary;
            EditToolBarVisible = p.General.EditToolbarEnabled;

            cmboEditSummary.Text = p.General.SelectedSummary;

            if (chkLock.Checked)
                lblSummary.Text = p.General.SelectedSummary;

            for (int i = 0; i < 10; ++i)
                SetPasteMoreText(i, p.General.PasteMore[i]);

            txtFind.Text = p.General.FindText;
            chkFindRegex.Checked = p.General.FindRegex;
            chkFindCaseSensitive.Checked = p.General.FindCaseSensitive;

            wordWrapToolStripMenuItem1.Checked = p.General.WordWrap;
            EnableToolBar = p.General.ToolBarEnabled;
            bypassRedirectsToolStripMenuItem.Checked = p.General.BypassRedirect;
            autoSaveSettingsToolStripMenuItem.Checked = p.General.AutoSaveSettings;
            preParseModeToolStripMenuItem.Checked = p.General.PreParseMode;
            noSectionEditSummaryToolStripMenuItem.Checked = p.General.noSectionEditSummary;
            restrictDefaultsortChangesToolStripMenuItem.Checked = p.General.restrictDefaultsortAddition;
            restrictOrphanTaggingToolStripMenuItem.Checked = p.General.restrictOrphanTagging;
            noMOSComplianceFixesToolStripMenuItem.Checked = p.General.noMOSComplianceFixes;
            syntaxHighlightEditBoxToolStripMenuItem.Checked = p.General.syntaxHighlightEditBox;
            highlightAllFindToolStripMenuItem.Checked = p.General.highlightAllFind;
            automaticallyDoAnythingToolStripMenuItem.Checked = !p.General.NoAutoChanges;
            actionOnLoad = p.General.OnLoadAction;
            doDiffInBotMode = p.General.DiffInBotMode;
            chkMinor.Checked = p.General.Minor;
            addToWatchList.SelectedIndex = p.General.AddToWatchlist;
            ShowMovingAverageTimer = p.General.TimerEnabled;

            sortAlphabeticallyToolStripMenuItem.Checked = p.General.SortListAlphabetically;
            displayfalsePositivesButtonToolStripMenuItem.Checked = p.General.AddIgnoredToLog;

            _autoSaveEditBoxEnabled = p.General.AutoSaveEdit.Enabled;
            AutoSaveEditBoxPeriod = p.General.AutoSaveEdit.SavePeriod;
            _autoSaveEditBoxFile = p.General.AutoSaveEdit.SaveFile;

            _suppressUsingAWB = p.General.SuppressUsingAWB;
            Article.AddUsingAWBOnArticleAction = p.General.AddUsingAWBToActionSummaries;

            filterOutNonMainSpaceToolStripMenuItem.Checked = p.General.filterNonMainSpace;
            removeDuplicatesToolStripMenuItem.Checked = listMaker.FilterDuplicates = p.General.AutoFilterDuplicates;

            alphaSortInterwikiLinksToolStripMenuItem.Checked = p.General.SortInterWikiOrder;
            replaceReferenceTagsToolStripMenuItem.Checked = p.General.ReplaceReferenceTags;
            focusAtEndOfEditTextBoxToolStripMenuItem.Checked = p.General.FocusAtEndOfEditBox;
            scrollToUnbalancedBracketsToolStripMenuItem.Checked = p.General.scrollToUnbalancedBrackets;

            txtEdit.Font = new System.Drawing.Font(p.General.TextBoxFont, p.General.TextBoxSize);

            loggingEnabled = p.General.LoggingEnabled;

            LowThreadPriority = p.General.LowThreadPriority;
            _flash = p.General.Flash;
            _beep = p.General.Beep;

            _minimize = p.General.Minimize;

            chkEnableDab.Checked = p.Disambiguation.Enabled;
            txtDabLink.Text = p.Disambiguation.Link;
            txtDabVariants.Lines = p.Disambiguation.Variants;
            udContextChars.Value = p.Disambiguation.ContextChars;

            listMaker.SpecialFilterSettings = p.Special;

            loggingSettings1.SerialisableSettings = p.Logging;

            CModule.ModuleEnabled = p.Module.Enabled;
            CModule.Language = p.Module.Language;
            CModule.Code = p.Module.Code.Replace("\n", "\r\n");
            if (CModule.ModuleEnabled)
                CModule.MakeModule();
            else
                CModule.SetModuleNotBuilt();

            ExtProgram.Settings = p.ExternalProgram;

            _listComparerUseCurrentArticleList = p.Tool.ListComparerUseCurrentArticleList;
            _listSplitterUseCurrentArticleList = p.Tool.ListSplitterUseCurrentArticleList;
            _dbScannerUseCurrentArticleList = p.Tool.DatabaseScannerUseCurrentArticleList;

            foreach (PluginPrefs pp in p.Plugin)
            {
                IAWBPlugin plugin;
                if (Plugin.AWBPlugins.TryGetValue(pp.Name, out plugin))
                    plugin.LoadSettings(pp.PluginSettings);
            }
        }

        private void SetPasteMoreText(int item, string s)
        {
            if (item < _pasteMoreItems.Length)
            {
                _pasteMoreItems[item].Tag = s;
                _pasteMoreItems[item].Text = _pasteMoreItemsPrefixes[item] +
                                            (string.IsNullOrEmpty(s) ? "" : s.Replace("&", "&&"));
                _pasteMoreItems[item].Visible = !string.IsNullOrEmpty(s);
            }
        }
    }
}