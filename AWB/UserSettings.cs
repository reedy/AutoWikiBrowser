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
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using WikiFunctions;
using WikiFunctions.Plugin;
using WikiFunctions.AWBSettings;
using AutoWikiBrowser.Plugins;

namespace AutoWikiBrowser
{
    public partial class MainForm
    {
        private void saveAsDefaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to save these settings as the default settings?", "Save as default?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                SavePrefs();
        }

        private void saveSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((!System.IO.File.Exists(settingsfilename)) || MessageBox.Show("Replace existing file?", "File exists - " + settingsfilename,
            MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)==DialogResult.Yes)
                SavePrefs(settingsfilename);
        }

        private void loadSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadSettingsDialog();
        }

        private void loadDefaultSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((MessageBox.Show("Would you really like to load the original default settings?", "Reset settings to default?", MessageBoxButtons.YesNo)) == DialogResult.Yes)
                ResetSettings();
        }

        private void saveCurrentSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveXML.FileName = settingsfilename;
            if (saveXML.ShowDialog() != DialogResult.OK)
                return;

            SavePrefs(saveXML.FileName);
            settingsfilename = saveXML.FileName;
        }

        private void ResetSettings()
        {
            findAndReplace.Clear();
            replaceSpecial.Clear();
            substTemplates.Clear();

            listMaker1.SelectedSource = 0;
            listMaker1.SourceText = "";
            listMaker1.Clear();

            chkGeneralFixes.Checked = true;
            chkAutoTagger.Checked = true;
            chkUnicodifyWhole.Checked = true;

            chkFindandReplace.Checked = false;
            chkSkipWhenNoFAR.Checked = true;
            findAndReplace.ignoreLinks = false;
            findAndReplace.AppendToSummary = true;
            findAndReplace.AfterOtherFixes = false;

            cmboCategorise.SelectedIndex = 0;
            txtNewCategory.Text = "";
            txtNewCategory2.Text = "";

            chkSkipNoCatChange.Checked = false;
            chkSkipNoImgChange.Checked = false;

            chkSkipIfContains.Checked = false;
            chkSkipIfNotContains.Checked = false;
            chkSkipIsRegex.Checked = false;
            chkSkipCaseSensitive.Checked = false;
            txtSkipIfContains.Text = "";
            txtSkipIfNotContains.Text = "";
            Skip.SelectedItem = "0";

            chkAppend.Checked = false;
            rdoAppend.Checked = true;
            txtAppendMessage.Text = "";

            cmboImages.SelectedIndex = 0;
            txtImageReplace.Text = "";
            txtImageWith.Text = "";

            chkRegExTypo.Checked = false;
            chkSkipIfNoRegexTypo.Checked = false;

            txtFind.Text = "";
            chkFindRegex.Checked = false;
            chkFindCaseSensitive.Checked = false;

            cmboEditSummary.SelectedIndex = 0;
            cmboEditSummary.Items.Clear();
            LoadDefaultEditSummaries();

            wordWrapToolStripMenuItem1.Checked = true;
            splitContainer1.Show();
            enableToolBar = false;
            bypassRedirectsToolStripMenuItem.Checked = true;
            chkSkipNonExistent.Checked = true;
            chkSkipExistent.Checked = false;
            doNotAutomaticallyDoAnythingToolStripMenuItem.Checked = false;
            chkSkipNoChanges.Checked = false;
            toolStripComboOnLoad.SelectedIndex = 0;
            ignoreNoBotsToolStripMenuItem.Checked = false;
            markAllAsMinorToolStripMenuItem.Checked = false;
            addAllToWatchlistToolStripMenuItem.Checked = false;
            showTimerToolStripMenuItem.Checked = false;
            showTimer();
            alphaSortInterwikiLinksToolStripMenuItem.Checked = true;
            addIgnoredToLogFileToolStripMenuItem.Checked = false;

            PasteMore1.Text = "";
            PasteMore2.Text = "";
            PasteMore3.Text = "";
            PasteMore4.Text = "";
            PasteMore5.Text = "";
            PasteMore6.Text = "";
            PasteMore7.Text = "";
            PasteMore8.Text = "";
            PasteMore9.Text = "";
            PasteMore10.Text = "";

            chkAutoMode.Checked = false;
            chkQuickSave.Checked = false;
            nudBotSpeed.Value = 15;

            //preferences
            System.Drawing.Font f = new System.Drawing.Font("Courier New", 10F, System.Drawing.FontStyle.Regular);
            txtEdit.Font = f;
            LowThreadPriority = false;
            FlashAndBeep = true;
            Flash = true;
            Beep = true;
            Minimize = true;
            TimeOut = 30;
            SaveArticleList = true;
            OverrideWatchlist = false;
            chkLock.Checked = false;

            AutoSaveEditBoxEnabled = false;
            AutoSaveEditBoxFile = "Edit Box.txt";
            AutoSaveEditBoxPeriod = 60;
                        
            chkEnableDab.Checked = false;
            txtDabLink.Text = "";
            txtDabVariants.Text = "";
            chkSkipNoDab.Checked = false;
            udContextChars.Value = 20;

            CategoryTextBox.Text = "";
            loggingSettings1.Reset();

            try
            {
                foreach (KeyValuePair<string, IAWBPlugin> a in Plugin.Items)
                    a.Value.Reset();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problem reseting plugin\r\n\r\n" + ex.Message);
            }

            cModule.ModuleEnabled = false;
            this.Text = "AutoWikiBrowser = Default.xml";
            lblStatusText.Text = "Default settings loaded.";
        }

        private void LoadDefaultEditSummaries()
        {
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

        private void loadSettingsDialog()
        {
            if (openXML.ShowDialog() != DialogResult.OK)
                return;

            LoadPrefs(openXML.FileName);
            settingsfilename = openXML.FileName;

            listMaker1.removeListDuplicates();
        }

        private string settingsfilename = "Default.xml";

        public void UpdateRecentList(string[] list)
        {
            RecentList.Clear();
            RecentList.AddRange(list);
            UpdateRecentSettingsMenu();
        }

        public void UpdateRecentList(string s)
        {
            int i = RecentList.IndexOf(s);

            if (i >= 0) RecentList.RemoveAt(i);

            RecentList.Insert(0, s);
            UpdateRecentSettingsMenu();
        }

        public void LoadRecentSettingsList()
        {
            string s;

            try
            {
                Microsoft.Win32.RegistryKey reg = Microsoft.Win32.Registry.CurrentUser.
                    OpenSubKey("Software\\Wikipedia\\AutoWikiBrowser");

                s = reg.GetValue("RecentList", "").ToString();
            }
            catch
            {
                return;
            }
            UpdateRecentList(s.Split('|'));
        }

        private void UpdateRecentSettingsMenu()
        {
            while (RecentList.Count > 5)
                RecentList.RemoveAt(5);

            recentToolStripMenuItem.DropDown.Items.Clear();
            int i = 1;
            foreach (string filename in RecentList)
            {
                if (i != RecentList.Count)
                {
                    i ++;
                    ToolStripItem item = recentToolStripMenuItem.DropDownItems.Add(filename);
                    item.Click += RecentSettingsClick;
                }
            }
        }

        public void SaveRecentSettingsList()
        {
            Microsoft.Win32.RegistryKey reg = Microsoft.Win32.Registry.CurrentUser.
                    CreateSubKey("Software\\Wikipedia\\AutoWikiBrowser");

            string list = "";
            foreach (string s in RecentList)
            {
                if (list != "") list += "|";
                list += s;
            }

            reg.SetValue("RecentList", list);
        }

        private void RecentSettingsClick(object sender, EventArgs e)
        {
            LoadPrefs((sender as ToolStripItem).Text);
            settingsfilename = (sender as ToolStripItem).Text;
            listMaker1.removeListDuplicates();
        }

        //new methods, using serialization

        /// <summary>
        /// Make preferences object from current settings
        /// </summary>
        private UserPrefs MakePrefs()
        {
            UserPrefs p = new UserPrefs();

            p.LanguageCode = Variables.LangCode;
            p.Project = Variables.Project;
            p.CustomProject = Variables.CustomProject;

            p.FindAndReplace.Enabled = chkFindandReplace.Checked;
            p.FindAndReplace.IgnoreSomeText = findAndReplace.ignoreLinks;
            p.FindAndReplace.AppendSummary = findAndReplace.AppendToSummary;
            p.FindAndReplace.AfterOtherFixes = findAndReplace.AfterOtherFixes;
            p.FindAndReplace.Replacements = findAndReplace.GetList();
            p.FindAndReplace.AdvancedReps = replaceSpecial.GetRules();
            p.FindAndReplace.SubstTemplates = substTemplates.TemplateList;

            p.List.ListSource = listMaker1.SourceText;
            p.List.Source = listMaker1.SelectedSource;

            
            p.General.SaveArticleList = SaveArticleList;

            if (p.General.SaveArticleList)
            { p.List.ArticleList = listMaker1.GetArticleList(); }
            else 
            { p.List.ArticleList = new List<Article>(); }

            p.General.IgnoreNoBots = ignoreNoBotsToolStripMenuItem.Checked;
            
            p.Editprefs.GeneralFixes = chkGeneralFixes.Checked;
            p.Editprefs.Tagger = chkAutoTagger.Checked;
            p.Editprefs.Unicodify = chkUnicodifyWhole.Checked;

            p.Editprefs.Recategorisation = cmboCategorise.SelectedIndex;
            p.Editprefs.NewCategory = txtNewCategory.Text;
            p.Editprefs.NewCategory2 = txtNewCategory2.Text;

            p.Editprefs.ReImage = cmboImages.SelectedIndex;
            p.Editprefs.ImageFind = txtImageReplace.Text;
            p.Editprefs.Replace = txtImageWith.Text;

            p.Editprefs.SkipIfNoCatChange = chkSkipNoCatChange.Checked;
            p.Editprefs.SkipIfNoImgChange = chkSkipNoImgChange.Checked;

            p.Editprefs.AppendText = chkAppend.Checked;
            p.Editprefs.Append = rdoAppend.Checked;
            p.Editprefs.Append = !rdoPrepend.Checked;
            p.Editprefs.Text = txtAppendMessage.Text;

            p.Editprefs.AutoDelay = (int)nudBotSpeed.Value;
            p.Editprefs.QuickSave = chkQuickSave.Checked;
            p.Editprefs.SuppressTag = chkSuppressTag.Checked;
            p.Editprefs.OverrideWatchlist = OverrideWatchlist;
            p.Editprefs.RegexTypoFix = chkRegExTypo.Checked;
            
            p.SkipOptions.SkipNonexistent = chkSkipNonExistent.Checked;
            p.SkipOptions.Skipexistent = chkSkipExistent.Checked;
            p.SkipOptions.SkipWhenNoChanges = chkSkipNoChanges.Checked;

            p.SkipOptions.SkipDoes = chkSkipIfContains.Checked;
            p.SkipOptions.SkipDoesNot = chkSkipIfNotContains.Checked;

            p.SkipOptions.SkipDoesText = txtSkipIfContains.Text;
            p.SkipOptions.SkipDoesNotText = txtSkipIfNotContains.Text;

            p.SkipOptions.Regex = chkSkipIsRegex.Checked;
            p.SkipOptions.CaseSensitive = chkSkipCaseSensitive.Checked;

            p.SkipOptions.SkipNoFindAndReplace = chkSkipWhenNoFAR.Checked;
            p.SkipOptions.SkipNoRegexTypoFix = chkSkipIfNoRegexTypo.Checked;
            p.SkipOptions.GeneralSkip = Skip.SelectedItem;
            p.SkipOptions.SkipNoDisambiguation = chkSkipNoDab.Checked;

            foreach (object s in cmboEditSummary.Items)
                p.General.Summaries.Add(s.ToString());

            p.General.SelectedSummary = cmboEditSummary.Text;

            p.General.PasteMore[0] = PasteMore1.Text;
            p.General.PasteMore[1] = PasteMore2.Text;
            p.General.PasteMore[2] = PasteMore3.Text;
            p.General.PasteMore[3] = PasteMore4.Text;
            p.General.PasteMore[4] = PasteMore5.Text;
            p.General.PasteMore[5] = PasteMore6.Text;
            p.General.PasteMore[6] = PasteMore7.Text;
            p.General.PasteMore[7] = PasteMore8.Text;
            p.General.PasteMore[8] = PasteMore9.Text;
            p.General.PasteMore[9] = PasteMore10.Text;

            p.General.FindText = txtFind.Text;
            p.General.FindRegex = chkFindRegex.Checked;
            p.General.FindCaseSensitive = chkFindCaseSensitive.Checked;

            p.General.WordWrap = wordWrapToolStripMenuItem1.Checked;
            p.General.ToolBarEnabled = enableToolBar;
            p.General.BypassRedirect = bypassRedirectsToolStripMenuItem.Checked;
            p.General.NoAutoChanges = doNotAutomaticallyDoAnythingToolStripMenuItem.Checked;
            p.General.OnLoadAction = toolStripComboOnLoad.SelectedIndex;
            p.General.Minor = chkMinor.Checked;
            p.General.Watch = addAllToWatchlistToolStripMenuItem.Checked;
            p.General.TimerEnabled = showTimerToolStripMenuItem.Checked;
            p.General.SortInterwikiOrder = sortAlphabeticallyToolStripMenuItem.Checked;
            p.General.AddIgnoredToLog = addIgnoredToLogFileToolStripMenuItem.Checked;

            p.General.TextBoxFont = txtEdit.Font.Name;
            p.General.TextBoxSize = (int)txtEdit.Font.Size;

            p.General.LowThreadPriority = LowThreadPriority;
            p.General.FlashAndBeep = false;

            p.General.Flash = Flash;
            p.General.Beep = Beep;
            p.General.Minimize = Minimize;
            p.General.TimeOutLimit = TimeOut;

            p.General.AutoSaveEdit.Enabled = AutoSaveEditBoxEnabled;
            p.General.AutoSaveEdit.SavePeriod = AutoSaveEditBoxPeriod;
            p.General.AutoSaveEdit.SaveFile = AutoSaveEditBoxFile;

            p.General.CustomWikis = customWikis;
            
            p.General.LockSummary = chkLock.Checked;

            p.Disambiguation.Enabled = chkEnableDab.Checked;
            p.Disambiguation.Link = txtDabLink.Text;
            p.Disambiguation.Variants = txtDabVariants.Lines;
            p.Disambiguation.ContextChars = (int)udContextChars.Value;

            p.Module.Enabled = cModule.ModuleEnabled;
            p.Module.Language = cModule.Language;
            p.Module.Code = cModule.Code;

            //Code For Saving Logging Settings - Uncomment when placed into AWB
            //p.Logging = LoggingSettings1.Settings;

            foreach (KeyValuePair<string, IAWBPlugin> a in Plugin.Items)
            {
                PluginPrefs pp = new PluginPrefs();
                pp.Name = a.Key;
                pp.PluginSettings = a.Value.SaveSettings();

                p.Plugin.Add(pp);
            }

            return p;
        }

        /// <summary>
        /// Load preferences object
        /// </summary>
        private void LoadPrefs(UserPrefs p)
        {
            SetProject(p.LanguageCode, p.Project, p.CustomProject);

            chkFindandReplace.Checked = p.FindAndReplace.Enabled;
            findAndReplace.ignoreLinks = p.FindAndReplace.IgnoreSomeText;
            findAndReplace.AppendToSummary = p.FindAndReplace.AppendSummary;
            findAndReplace.AfterOtherFixes = p.FindAndReplace.AfterOtherFixes;
            findAndReplace.AddNew(p.FindAndReplace.Replacements);
            replaceSpecial.AddNewRule(p.FindAndReplace.AdvancedReps);
            substTemplates.TemplateList = p.FindAndReplace.SubstTemplates;
            findAndReplace.MakeList();

            OverrideWatchlist = p.Editprefs.OverrideWatchlist;
            listMaker1.SourceText = p.List.ListSource;
            listMaker1.SelectedSource = p.List.Source;

            SaveArticleList = p.General.SaveArticleList;

            ignoreNoBotsToolStripMenuItem.Checked = p.General.IgnoreNoBots;

            listMaker1.Add(p.List.ArticleList);
            
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
            chkSkipNoImgChange.Checked = p.Editprefs.SkipIfNoImgChange;

            chkAppend.Checked = p.Editprefs.AppendText;
            rdoAppend.Checked = p.Editprefs.Append;
            rdoPrepend.Checked = !p.Editprefs.Append;
            txtAppendMessage.Text = p.Editprefs.Text;

            nudBotSpeed.Value = p.Editprefs.AutoDelay;
            chkQuickSave.Checked = p.Editprefs.QuickSave;
            chkSuppressTag.Checked = p.Editprefs.SuppressTag;

            chkRegExTypo.Checked = p.Editprefs.RegexTypoFix;
            
            chkSkipNonExistent.Checked = p.SkipOptions.SkipNonexistent;
            chkSkipExistent.Checked = p.SkipOptions.Skipexistent;
            chkSkipNoChanges.Checked = p.SkipOptions.SkipWhenNoChanges;

            chkSkipIfContains.Checked = p.SkipOptions.SkipDoes;
            chkSkipIfNotContains.Checked = p.SkipOptions.SkipDoesNot;

            txtSkipIfContains.Text = p.SkipOptions.SkipDoesText;
            txtSkipIfNotContains.Text = p.SkipOptions.SkipDoesNotText;

            chkSkipIsRegex.Checked = p.SkipOptions.Regex;
            chkSkipCaseSensitive.Checked = p.SkipOptions.CaseSensitive;

            chkSkipWhenNoFAR.Checked = p.SkipOptions.SkipNoFindAndReplace;
            chkSkipIfNoRegexTypo.Checked = p.SkipOptions.SkipNoRegexTypoFix;
            Skip.SelectedItem = p.SkipOptions.GeneralSkip;
            chkSkipNoDab.Checked = p.SkipOptions.SkipNoDisambiguation;

            cmboEditSummary.Items.Clear();
            foreach (string s in p.General.Summaries)
            {
                cmboEditSummary.Items.Add(s);
            }

            chkLock.Checked = p.General.LockSummary;
            cmboEditSummary.Text = p.General.SelectedSummary;

            if (chkLock.Checked)
                lblSummary.Text = p.General.SelectedSummary;

            PasteMore1.Text = p.General.PasteMore[0];
            PasteMore2.Text = p.General.PasteMore[1];
            PasteMore3.Text = p.General.PasteMore[2];
            PasteMore4.Text = p.General.PasteMore[3];
            PasteMore5.Text = p.General.PasteMore[4];
            PasteMore6.Text = p.General.PasteMore[5];
            PasteMore7.Text = p.General.PasteMore[6];
            PasteMore8.Text = p.General.PasteMore[7];
            PasteMore9.Text = p.General.PasteMore[8];
            PasteMore10.Text = p.General.PasteMore[9];

            txtFind.Text = p.General.FindText;
            chkFindRegex.Checked = p.General.FindRegex;
            chkFindCaseSensitive.Checked = p.General.FindCaseSensitive;

            wordWrapToolStripMenuItem1.Checked = p.General.WordWrap;
            enableToolBar = p.General.ToolBarEnabled;
            bypassRedirectsToolStripMenuItem.Checked = p.General.BypassRedirect;
            doNotAutomaticallyDoAnythingToolStripMenuItem.Checked = p.General.NoAutoChanges;
            toolStripComboOnLoad.SelectedIndex = p.General.OnLoadAction;
            chkMinor.Checked = p.General.Minor;
            addAllToWatchlistToolStripMenuItem.Checked = p.General.Watch;
            showTimerToolStripMenuItem.Checked = p.General.TimerEnabled;
            showTimer();
            sortAlphabeticallyToolStripMenuItem.Checked = p.General.SortInterwikiOrder;
            addIgnoredToLogFileToolStripMenuItem.Checked = p.General.AddIgnoredToLog;

            AutoSaveEditBoxEnabled = p.General.AutoSaveEdit.Enabled;
            AutoSaveEditBoxPeriod = p.General.AutoSaveEdit.SavePeriod;
            AutoSaveEditBoxFile = p.General.AutoSaveEdit.SaveFile;

            customWikis = p.General.CustomWikis;

            System.Drawing.Font f = new System.Drawing.Font(p.General.TextBoxFont, p.General.TextBoxSize);
            txtEdit.Font = f;

            LowThreadPriority = p.General.LowThreadPriority;

            if (p.General.Flash == p.General.FlashAndBeep)
            { Flash = p.General.FlashAndBeep; }
            else
            { Flash = p.General.Flash; }

            if (p.General.Beep == p.General.FlashAndBeep)
            { Beep = p.General.FlashAndBeep; }
            else
            { Beep = p.General.Beep; }

            Minimize = p.General.Minimize;
            TimeOut = p.General.TimeOutLimit;
            webBrowserEdit.TimeoutLimit = int.Parse(TimeOut.ToString());
            
            chkEnableDab.Checked = p.Disambiguation.Enabled;
            txtDabLink.Text = p.Disambiguation.Link;
            txtDabVariants.Lines = p.Disambiguation.Variants;
            udContextChars.Value = p.Disambiguation.ContextChars;

            //Code For Loading Logging Settings - Uncomment when placed into AWB
            //LoggingSettings1.Settings = p.Logging;

            cModule.ModuleEnabled = p.Module.Enabled;
            cModule.Language = p.Module.Language;
            cModule.Code = p.Module.Code.Replace("\n", "\r\n");

            foreach (PluginPrefs pp in p.Plugin)
            {
                if (Plugin.Items.ContainsKey(pp.Name))
                    Plugin.Items[pp.Name].LoadSettings(pp.PluginSettings);
            }
        }

        /// <summary>
        /// Save preferences as default
        /// </summary>
        private void SavePrefs()
        {
            SavePrefs("Default.xml");
        }

        /// <summary>
        /// Save preferences to file
        /// </summary>
        private void SavePrefs(string Path)
        {
            try
            {
                using (FileStream fStream = new FileStream(Path, FileMode.Create))
                {
                    UserPrefs P = MakePrefs();
                    List<System.Type> types = savePluginSettings(P);
                    
                    XmlSerializer xs = new XmlSerializer(typeof(UserPrefs), types.ToArray());
                    xs.Serialize(fStream, P);
                    UpdateRecentList(Path);
                    //LoadPrefs(P); // why is this? it causes problems. commented out. --MaxSem
                    SettingsFile = " - " + Path.Remove(0, Path.LastIndexOf("\\") + 1);
                    updateSettingsFile();
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error saving settings", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<System.Type> savePluginSettings(UserPrefs Prefs)
        {
            List<System.Type> types = new List<Type>();
            /* Find out what types the plugins are using for their settings so we can 
                      * add them to the Serializer. The plugin author must ensure s(he) is using
                      * serializable types.
                      */

            foreach (PluginPrefs pl in Prefs.Plugin)
            {
                if ((pl.PluginSettings != null) && (pl.PluginSettings.Length >= 1))
                {
                    foreach (object pl2 in pl.PluginSettings)
                    {
                        types.Add(pl2.GetType());
                    }
                }
            }
            return types;
        }

        /// <summary>
        /// Load default preferences
        /// </summary>
        private void LoadPrefs()
        {
            if (!File.Exists("Default.xml"))
                return;

            LoadPrefs("Default.xml");
        }

        /// <summary>
        /// Load preferences from file
        /// </summary>
        private void LoadPrefs(string Path)
        {
            try
            {
                if (Path == "")
                    return;

                //test file to see if it is an old AWB file
                StreamReader sr = new StreamReader(new FileStream(Path, FileMode.Open));
                string test = sr.ReadToEnd();
                sr.Close();

                if (test.Contains("<projectlang proj="))
                {
                    MessageBox.Show("This file uses old settings format unsupported by this version of AWB.");
                    return;
                }

                findAndReplace.Clear();
                replaceSpecial.Clear();
                substTemplates.Clear();

                using (FileStream fStream = new FileStream(Path, FileMode.Open))
                {
                    UserPrefs p;
                    XmlSerializer xs = new XmlSerializer(typeof(UserPrefs));
                    p = (UserPrefs)xs.Deserialize(fStream);
                    LoadPrefs(p);
                }

                SettingsFile = " - " + Path.Remove(0, Path.LastIndexOf("\\") + 1);
                updateSettingsFile();
                lblStatusText.Text = "Settings successfully loaded";
                UpdateRecentList(Path);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error loading settings", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void updateSettingsFile()
        {
            this.Text = "AutoWikiBrowser" + SettingsFile;
            ntfyTray.Text = "AutoWikiBrowser" + SettingsFile;
        }
    }
}
