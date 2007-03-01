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
            saveXML.FileName = settingsfilename;
            if (saveXML.ShowDialog() != DialogResult.OK)
                return;

            SavePrefs(saveXML.FileName);
        }

        private void loadSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadSettingsDialog();
        }

        private void loadDefaultSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetSettings();
        }

        private void ResetSettings()
        {
            findAndReplace.Clear();
            replaceSpecial.Clear();
            substTemplates.Clear();

            listMaker1.SelectedSource = 0;
            listMaker1.SourceText = "";

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

            wordWrapToolStripMenuItem1.Checked = true;
            panel2.Show();
            enableToolBar = false;
            bypassRedirectsToolStripMenuItem.Checked = true;
            chkSkipNonExistent.Checked = true;
            doNotAutomaticallyDoAnythingToolStripMenuItem.Checked = false;
            chkSkipNoChanges.Checked = false;
            toolStripComboOnLoad.SelectedIndex = 0;
            markAllAsMinorToolStripMenuItem.Checked = false;
            addAllToWatchlistToolStripMenuItem.Checked = false;
            showTimerToolStripMenuItem.Checked = false;
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
            webBrowserEdit.EnhanceDiffEnabled = true;
            webBrowserEdit.ScrollDown = true;
            webBrowserEdit.DiffFontSize = 150;
            System.Drawing.Font f = new System.Drawing.Font("Courier New", 10F, System.Drawing.FontStyle.Regular);
            txtEdit.Font = f;
            LowThreadPriority = false;
            FlashAndBeep = true;
            Flash = true;
            Beep = true;
            chkLock.Checked = false;


            chkEnableDab.Checked = false;
            txtDabLink.Text = "";
            txtDabVariants.Text = "";
            chkSkipNoDab.Checked = false;

            try
            {
                foreach (KeyValuePair<string, IAWBPlugin> a in AWBPlugins)
                    a.Value.Reset();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problem reseting plugin\r\n\r\n" + ex.Message);
            }

            cModule.ModuleEnabled = false;

            lblStatusText.Text = "Default settings loaded.";
        }

        private void loadSettingsDialog()
        {
            if (openXML.ShowDialog() != DialogResult.OK)
                return;

            LoadPrefs(openXML.FileName);
            settingsfilename = openXML.FileName;
        }

        private string settingsfilename = "settings";

        [Obsolete]
        private void loadSettings(Stream stream)
        {
            try
            {
                findAndReplace.Clear();
                replaceSpecial.Clear();
                substTemplates.Clear();
                cmboEditSummary.Items.Clear();

                using (XmlTextReader reader = new XmlTextReader(stream))
                {
                    reader.WhitespaceHandling = WhitespaceHandling.None;
                    while (reader.Read())
                    {
                        if (reader.Name == "findandreplacesettings" && reader.HasAttributes)
                        {
                            if (reader.MoveToAttribute("enabled"))
                                chkFindandReplace.Checked = bool.Parse(reader.Value);
                            if (reader.MoveToAttribute("ignorenofar"))
                                chkSkipWhenNoFAR.Checked = bool.Parse(reader.Value);
                            if (reader.MoveToAttribute("ignoretext"))
                                findAndReplace.ignoreLinks = bool.Parse(reader.Value);
                            if (reader.MoveToAttribute("appendsummary"))
                                findAndReplace.AppendToSummary = bool.Parse(reader.Value);
                            if (reader.MoveToAttribute("afterotherfixes"))
                                findAndReplace.AfterOtherFixes = bool.Parse(reader.Value);

                            continue;
                        }

                        if (reader.Name == "FindAndReplace" && reader.HasAttributes)
                        {
                            string find = "";
                            string replace = "";
                            bool regex = true;
                            bool casesens = true;
                            bool multi = false;
                            bool single = false;
                            int times = -1;
                            bool enabled = true;

                            if (reader.MoveToAttribute("find"))
                                find = reader.Value;
                            if (reader.MoveToAttribute("replacewith"))
                                replace = reader.Value;

                            if (reader.MoveToAttribute("casesensitive"))
                                casesens = bool.Parse(reader.Value);
                            if (reader.MoveToAttribute("regex"))
                                regex = bool.Parse(reader.Value);
                            if (reader.MoveToAttribute("multi"))
                                multi = bool.Parse(reader.Value);
                            if (reader.MoveToAttribute("single"))
                                single = bool.Parse(reader.Value);
                            if (reader.MoveToAttribute("enabled"))
                                enabled = bool.Parse(reader.Value);
                            if (reader.MoveToAttribute("maxnumber"))
                                times = int.Parse(reader.Value);

                            if (find.Length > 0)
                                findAndReplace.AddNew(find, replace, casesens, regex, multi, single, times, enabled);

                            continue;
                        }

                        if (reader.Name == WikiFunctions.MWB.ReplaceSpecial.XmlName)
                        {
                            bool enabled = false;
                            replaceSpecial.ReadFromXml(reader, ref enabled);
                            continue;
                        }

                        if (reader.Name == "projectlang" && reader.HasAttributes)
                        {
                            string project = "";
                            string language = "";
                            string customproject = "";

                            if (reader.MoveToAttribute("proj"))
                                project = reader.Value;
                            if (reader.MoveToAttribute("lang"))
                                language = reader.Value;
                            if (reader.MoveToAttribute("custom"))
                                customproject = reader.Value;

                            LangCodeEnum l = (LangCodeEnum)Enum.Parse(typeof(LangCodeEnum), language);
                            ProjectEnum p = (ProjectEnum)Enum.Parse(typeof(ProjectEnum), project);

                            SetProject(l, p, customproject);

                            continue;
                        }
                        if (reader.Name == "selectsource" && reader.HasAttributes)
                        {
                            if (reader.MoveToAttribute("index"))
                                listMaker1.SelectedSource = (WikiFunctions.Lists.SourceType)int.Parse(reader.Value);
                            if (reader.MoveToAttribute("text"))
                                listMaker1.SourceText = reader.Value;

                            continue;
                        }
                        if (reader.Name == "general" && reader.HasAttributes)
                        {
                            if (reader.MoveToAttribute("general"))
                                chkGeneralFixes.Checked = bool.Parse(reader.Value);
                            if (reader.MoveToAttribute("tagger"))
                                chkAutoTagger.Checked = bool.Parse(reader.Value);
                            if (reader.MoveToAttribute("unicodifyer"))
                                chkUnicodifyWhole.Checked = bool.Parse(reader.Value);

                            continue;
                        }
                        if (reader.Name == "categorisation" && reader.HasAttributes)
                        {
                            if (reader.MoveToAttribute("index"))
                                cmboCategorise.SelectedIndex = int.Parse(reader.Value);
                            if (reader.MoveToAttribute("text"))
                                txtNewCategory.Text = reader.Value;
                            if (reader.MoveToAttribute("text2"))
                                txtNewCategory2.Text = reader.Value;
                            continue;
                        }
                        if (reader.Name == "skip" && reader.HasAttributes)
                        {
                            if (reader.MoveToAttribute("does"))
                                chkSkipIfContains.Checked = bool.Parse(reader.Value);
                            if (reader.MoveToAttribute("doesnot"))
                                chkSkipIfNotContains.Checked = bool.Parse(reader.Value);
                            if (reader.MoveToAttribute("regex"))
                                chkSkipIsRegex.Checked = bool.Parse(reader.Value);
                            if (reader.MoveToAttribute("casesensitive"))
                                chkSkipCaseSensitive.Checked = bool.Parse(reader.Value);
                            if (reader.MoveToAttribute("doestext"))
                                txtSkipIfContains.Text = reader.Value;
                            if (reader.MoveToAttribute("doesnottext"))
                                txtSkipIfNotContains.Text = reader.Value;
                            if (reader.MoveToAttribute("moreindex"))
                                Skip.SelectedItem = reader.Value;

                            continue;
                        }
                        if (reader.Name == "message" && reader.HasAttributes)
                        {
                            if (reader.MoveToAttribute("enabled"))
                                chkAppend.Checked = bool.Parse(reader.Value);
                            if (reader.MoveToAttribute("text"))
                                txtAppendMessage.Text = reader.Value;
                            if (reader.MoveToAttribute("append"))
                            {
                                rdoAppend.Checked = bool.Parse(reader.Value);
                                rdoPrepend.Checked = !bool.Parse(reader.Value);
                            }

                            continue;
                        }
                        if (reader.Name == "automode" && reader.HasAttributes)
                        {
                            if (reader.MoveToAttribute("delay"))
                                nudBotSpeed.Value = int.Parse(reader.Value);
                            if (reader.MoveToAttribute("quicksave"))
                                chkQuickSave.Checked = bool.Parse(reader.Value);
                            if (reader.MoveToAttribute("suppresstag"))
                                chkSuppressTag.Checked = bool.Parse(reader.Value);

                            continue;
                        }
                        if (reader.Name == "imager" && reader.HasAttributes)
                        {
                            if (reader.MoveToAttribute("index"))
                                cmboImages.SelectedIndex = int.Parse(reader.Value);
                            if (reader.MoveToAttribute("replace"))
                                txtImageReplace.Text = reader.Value;
                            if (reader.MoveToAttribute("with"))
                                txtImageWith.Text = reader.Value;

                            continue;
                        }
                        if (reader.Name == "regextypofixproperties" && reader.HasAttributes)
                        {
                            if (reader.MoveToAttribute("enabled"))
                                chkRegExTypo.Checked = bool.Parse(reader.Value);
                            if (reader.MoveToAttribute("skipnofixed"))
                                chkSkipIfNoRegexTypo.Checked = bool.Parse(reader.Value);

                            continue;
                        }
                        if (reader.Name == "find" && reader.HasAttributes)
                        {
                            if (reader.MoveToAttribute("text"))
                                txtFind.Text = reader.Value;
                            if (reader.MoveToAttribute("regex"))
                                chkFindRegex.Checked = bool.Parse(reader.Value);
                            if (reader.MoveToAttribute("casesensitive"))
                                chkFindCaseSensitive.Checked = bool.Parse(reader.Value);

                            continue;
                        }
                        if (reader.Name == "summary" && reader.HasAttributes)
                        {
                            if (reader.MoveToAttribute("text"))
                                if (!cmboEditSummary.Items.Contains(reader.Value) && reader.Value.Length > 0)
                                   cmboEditSummary.Items.Add(reader.Value);


                            continue;
                        }
                        if (reader.Name == "summaryindex" && reader.HasAttributes)
                        {
                            if (reader.MoveToAttribute("index"))
                                cmboEditSummary.Text = reader.Value;

                            continue;
                        }

                        //menu
                        if (reader.Name == "wordwrap" && reader.HasAttributes)
                        {
                            if (reader.MoveToAttribute("enabled"))
                                wordWrapToolStripMenuItem1.Checked = bool.Parse(reader.Value);

                            continue;
                        }
                        if (reader.Name == "toolbar" && reader.HasAttributes)
                        {
                            if (reader.MoveToAttribute("enabled"))
                                enableToolBar = bool.Parse(reader.Value);

                            continue;
                        }
                        if (reader.Name == "bypass" && reader.HasAttributes)
                        {
                            if (reader.MoveToAttribute("enabled"))
                                bypassRedirectsToolStripMenuItem.Checked = bool.Parse(reader.Value);

                            continue;
                        }
                        if (reader.Name == "ingnorenonexistent" && reader.HasAttributes)
                        {
                            if (reader.MoveToAttribute("enabled"))
                                chkSkipNonExistent.Checked = bool.Parse(reader.Value);

                            continue;
                        }
                        if (reader.Name == "noautochanges" && reader.HasAttributes)
                        {
                            if (reader.MoveToAttribute("enabled"))
                                doNotAutomaticallyDoAnythingToolStripMenuItem.Checked = bool.Parse(reader.Value);

                            continue;
                        }
                        if (reader.Name == "skipnochanges" && reader.HasAttributes)
                        {
                            if (reader.MoveToAttribute("enabled"))
                                chkSkipNoChanges.Checked = bool.Parse(reader.Value);
                        }
                        if (reader.Name == "minor" && reader.HasAttributes)
                        {
                            if (reader.MoveToAttribute("enabled"))
                                markAllAsMinorToolStripMenuItem.Checked = bool.Parse(reader.Value);

                            continue;
                        }
                        if (reader.Name == "watch" && reader.HasAttributes)
                        {
                            if (reader.MoveToAttribute("enabled"))
                                addAllToWatchlistToolStripMenuItem.Checked = bool.Parse(reader.Value);

                            continue;
                        }
                        if (reader.Name == "timer" && reader.HasAttributes)
                        {
                            if (reader.MoveToAttribute("enabled"))
                                showTimerToolStripMenuItem.Checked = bool.Parse(reader.Value);

                            continue;
                        }
                        if (reader.Name == "sortinterwiki" && reader.HasAttributes)
                        {
                            if (reader.MoveToAttribute("enabled"))
                                alphaSortInterwikiLinksToolStripMenuItem.Checked = bool.Parse(reader.Value);

                            continue;
                        }
                        if (reader.Name == "addignoredtolog" && reader.HasAttributes)
                        {
                            if (reader.MoveToAttribute("enabled"))
                                addIgnoredToLogFileToolStripMenuItem.Checked = bool.Parse(reader.Value);
                            btnFalsePositive.Visible = bool.Parse(reader.Value);

                            continue;
                        }
                        if (reader.Name == "pastemore1" && reader.HasAttributes)
                        {
                            if (reader.MoveToAttribute("text"))
                                PasteMore1.Text = reader.Value;

                            continue;
                        }
                        if (reader.Name == "pastemore2" && reader.HasAttributes)
                        {
                            if (reader.MoveToAttribute("text"))
                                PasteMore2.Text = reader.Value;

                            continue;
                        }
                        if (reader.Name == "pastemore3" && reader.HasAttributes)
                        {
                            if (reader.MoveToAttribute("text"))
                                PasteMore3.Text = reader.Value;

                            continue;
                        }
                        if (reader.Name == "pastemore4" && reader.HasAttributes)
                        {
                            if (reader.MoveToAttribute("text"))
                                PasteMore4.Text = reader.Value;

                            continue;
                        }
                        if (reader.Name == "pastemore5" && reader.HasAttributes)
                        {
                            if (reader.MoveToAttribute("text"))
                                PasteMore5.Text = reader.Value;

                            continue;
                        }
                        if (reader.Name == "pastemore6" && reader.HasAttributes)
                        {
                            if (reader.MoveToAttribute("text"))
                                PasteMore6.Text = reader.Value;

                            continue;
                        }
                        if (reader.Name == "pastemore7" && reader.HasAttributes)
                        {
                            if (reader.MoveToAttribute("text"))
                                PasteMore7.Text = reader.Value;

                            continue;
                        }
                        if (reader.Name == "pastemore8" && reader.HasAttributes)
                        {
                            if (reader.MoveToAttribute("text"))
                                PasteMore8.Text = reader.Value;

                            continue;
                        }
                        if (reader.Name == "pastemore9" && reader.HasAttributes)
                        {
                            if (reader.MoveToAttribute("text"))
                                PasteMore9.Text = reader.Value;

                            continue;
                        }
                        if (reader.Name == "pastemore10" && reader.HasAttributes)
                        {
                            if (reader.MoveToAttribute("text"))
                                PasteMore10.Text = reader.Value;

                            continue;
                        }
                        if (reader.Name == "preferencevalues" && reader.HasAttributes)
                        {
                            if (reader.MoveToAttribute("enhancediff"))
                                webBrowserEdit.EnhanceDiffEnabled = bool.Parse(reader.Value);

                            if (reader.MoveToAttribute("scrolldown"))
                                webBrowserEdit.ScrollDown = bool.Parse(reader.Value);

                            if (reader.MoveToAttribute("difffontsize"))
                                webBrowserEdit.DiffFontSize = int.Parse(reader.Value);

                            float s = 10F;
                            string d = "Courier New";
                            if (reader.MoveToAttribute("textboxfontsize"))
                                s = float.Parse(reader.Value);
                            if (reader.MoveToAttribute("textboxfont"))
                                d = reader.Value;
                            System.Drawing.Font f = new System.Drawing.Font(d, s);
                            txtEdit.Font = f;

                            if (reader.MoveToAttribute("lowthreadpriority"))
                                LowThreadPriority = bool.Parse(reader.Value);

                            if (reader.MoveToAttribute("flashandbeep"))
                            {
                               FlashAndBeep = bool.Parse(reader.Value);
                            }

                            if (reader.MoveToAttribute("flash"))
                                Flash = bool.Parse(reader.Value);

                            if (reader.MoveToAttribute("beep"))
                                Beep = bool.Parse(reader.Value);

                            continue;
                        }

                        //foreach (IAWBPlugin a in AWBPlugins)
                        //{
                        //    if (reader.Name == a.Name.Replace(' ', '_') && reader.HasAttributes)
                        //    {
                        //        a.LoadSettings(reader);
                        //        break;
                        //    }
                        //}

                    }
                    stream.Close();
                    findAndReplace.MakeList();
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message, "File error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            MessageBox.Show("Please re-save this settings file to use the new settings format.", "Re-save", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

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
            p.List.ArticleList = listMaker1.GetArticleList();


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

            p.Editprefs.RegexTypoFix = chkRegExTypo.Checked;


            p.SkipOptions.SkipNonexistent = chkSkipNonExistent.Checked;
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
            p.General.Minor = markAllAsMinorToolStripMenuItem.Checked;
            p.General.Watch = addAllToWatchlistToolStripMenuItem.Checked;
            p.General.TimerEnabled = showTimerToolStripMenuItem.Checked;
            p.General.SortInterwikiOrder = sortAlphabeticallyToolStripMenuItem.Checked;
            p.General.AddIgnoredToLog = addIgnoredToLogFileToolStripMenuItem.Checked;

            p.General.EnhancedDiff = webBrowserEdit.EnhanceDiffEnabled;
            p.General.ScrollDown = webBrowserEdit.ScrollDown;
            p.General.DiffFontSize = webBrowserEdit.DiffFontSize;

            p.General.TextBoxFont = txtEdit.Font.Name;
            p.General.TextBoxSize = (int)txtEdit.Font.Size;

            p.General.LowThreadPriority = LowThreadPriority;
            p.General.FlashAndBeep = false;

            p.General.Flash = Flash;
            p.General.Beep = Beep;
            
            p.General.LockSummary = chkLock.Checked;

            p.Disambiguation.Enabled = chkEnableDab.Checked;
            p.Disambiguation.Link = txtDabLink.Text;
            p.Disambiguation.Variants = txtDabVariants.Lines;

            p.Module.Enabled = cModule.ModuleEnabled;
            p.Module.Language = cModule.Language;
            p.Module.Code = cModule.Code;

            foreach (KeyValuePair<string, IAWBPlugin> a in AWBPlugins)
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

            listMaker1.SourceText = p.List.ListSource;
            listMaker1.SelectedSource = p.List.Source;
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
            markAllAsMinorToolStripMenuItem.Checked = p.General.Minor;
            addAllToWatchlistToolStripMenuItem.Checked = p.General.Watch;
            showTimerToolStripMenuItem.Checked = p.General.TimerEnabled;
            sortAlphabeticallyToolStripMenuItem.Checked = p.General.SortInterwikiOrder;
            addIgnoredToLogFileToolStripMenuItem.Checked = p.General.AddIgnoredToLog;

            webBrowserEdit.EnhanceDiffEnabled = p.General.EnhancedDiff;
            webBrowserEdit.ScrollDown = p.General.ScrollDown;
            webBrowserEdit.DiffFontSize = p.General.DiffFontSize;

            System.Drawing.Font f = new System.Drawing.Font(p.General.TextBoxFont, p.General.TextBoxSize);
            txtEdit.Font = f;

            LowThreadPriority = p.General.LowThreadPriority;
            //FlashAndBeep = p.General.FlashAndBeep;

            if (p.General.Flash == p.General.FlashAndBeep)
            {
                Flash = p.General.FlashAndBeep;
            }
            else
            {
                Flash = p.General.Flash;
            }

            if (p.General.Beep == p.General.FlashAndBeep)
            {
                Beep = p.General.FlashAndBeep;
            }
            else
            {
                Beep = p.General.Beep;
            }
            
            chkEnableDab.Checked = p.Disambiguation.Enabled;
            txtDabLink.Text = p.Disambiguation.Link;
            txtDabVariants.Lines = p.Disambiguation.Variants;

            cModule.ModuleEnabled = p.Module.Enabled;
            cModule.Language = p.Module.Language;
            cModule.Code = p.Module.Code.Replace("\n", "\r\n");

            foreach (PluginPrefs pp in p.Plugin)
            {
                if (AWBPlugins.ContainsKey(pp.Name))
                    AWBPlugins[pp.Name].LoadSettings(pp.PluginSettings);
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
                    List<System.Type> types = new List<Type>();

                    try /* Find out what types the plugins are using for their settings so we can 
                         * add them to the Serializer. The plugin author must ensure s(he) is using
                         * serializable types.
                         */
                    {
                        foreach (PluginPrefs pl in P.Plugin)
                        {
                            if ((pl.PluginSettings != null) && (pl.PluginSettings.Length >= 1))
                            {
                                foreach (object pl2 in pl.PluginSettings)
                                {
                                    types.Add(pl2.GetType());
                                }
                            }
                        }
                    }
                    catch {
                    }

                    XmlSerializer xs = new XmlSerializer(typeof(UserPrefs), types.ToArray());
                    xs.Serialize(fStream, P);
                    UpdateRecentList(Path);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error saving settings", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                findAndReplace.Clear();
                replaceSpecial.Clear();
                substTemplates.Clear();

                //test file to see if it is an old AWB file
                StreamReader sr = new StreamReader(new FileStream(Path, FileMode.Open));
                string test = sr.ReadToEnd();
                bool oldFile = false;
                sr.Close();

                if (test.Contains("<projectlang proj="))
                    oldFile = true;

                using (FileStream fStream = new FileStream(Path, FileMode.Open))
                {
                    if (oldFile)
                    {
                        loadSettings(fStream);
                    }
                    else
                    {
                        UserPrefs p;
                        XmlSerializer xs = new XmlSerializer(typeof(UserPrefs));
                        p = (UserPrefs)xs.Deserialize(fStream);
                        LoadPrefs(p);
                    }
                }

                SettingsFile = " - " + Path.Remove(0, Path.LastIndexOf("\\") + 1);
                this.Text = "AutoWikiBrowser" + SettingsFile;
                lblStatusText.Text = "Settings successfully loaded";
                UpdateRecentList(Path);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error loading settings", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
