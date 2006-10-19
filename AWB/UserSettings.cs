using System;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using WikiFunctions;
using WikiFunctions.Plugin;

namespace AutoWikiBrowser
{
    public partial class MainForm
    {
        private void saveAsDefaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //   saveSettings(Application.StartupPath + "\\Default.xml");
        }

        private void saveSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveXML.ShowDialog() != DialogResult.OK)
                return;

            //  saveSettings(saveXML.FileName);
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
            previewInsteadOfDiffToolStripMenuItem.Checked = false;
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

            try
            {
                foreach (IAWBPlugin a in AWBPlugins)
                    a.Reset();
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
            loadSettings(openXML.FileName);
        }

        private void loadDefaultSettings()
        {//load Default.xml file if it exists
            try
            {
                string filename = Application.StartupPath + "\\Default.xml";

                loadSettings(filename);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void loadSettings(string filename)
        {
            try
            {
                if (!File.Exists(filename))
                    throw new FileNotFoundException("Settings file not found.");

                strSettingsFile = " - " + filename.Remove(0, filename.LastIndexOf("\\") + 1);
                this.Text = "AutoWikiBrowser" + strSettingsFile;

                Stream stream = new FileStream(filename, FileMode.Open);
                findAndReplace.Clear();
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
                                rdoAppend.Checked = bool.Parse(reader.Value);
                            rdoPrepend.Checked = !bool.Parse(reader.Value);

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
                        if (reader.Name == "preview" && reader.HasAttributes)
                        {
                            if (reader.MoveToAttribute("enabled"))
                                previewInsteadOfDiffToolStripMenuItem.Checked = bool.Parse(reader.Value);

                            continue;
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
                                FlashAndBeep = bool.Parse(reader.Value);

                            continue;
                        }

                        foreach (IAWBPlugin a in AWBPlugins)
                        {
                            if (reader.Name == a.Name.Replace(' ', '_') && reader.HasAttributes)
                            {
                                a.ReadXML(reader);
                                break;
                            }
                        }

                    }
                    stream.Close();
                    findAndReplace.MakeList();
                    lblStatusText.Text = "Settings successfully loaded";
                    UpdateRecentList(filename);
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
            foreach (string filename in RecentList)
            {
                ToolStripItem item = recentToolStripMenuItem.DropDownItems.Add(filename);
                item.Click += RecentSettingsClick;
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
            loadSettings((sender as ToolStripItem).Text);
        }

        //new methods, using serialization

        private UserPrefs MakePrefs()
        {
            UserPrefs p = new UserPrefs();

            p.LanguageCode = Variables.LangCode;
            p.Project = Variables.Project;
            p.CustomProject = Variables.CustomProject;

            p.FindAndReplace.Enabled = chkFindandReplace.Checked;
            p.FindAndReplace.IgnoreSomeText = findAndReplace.ignoreLinks;
            p.FindAndReplace.AppendSummary = findAndReplace.AppendToSummary;
            p.FindAndReplace.Replacements = findAndReplace.GetList();


            p.List.ListSource = listMaker1.SourceText;
            p.List.Source = listMaker1.SelectedSource;
            p.List.ArticleList = listMaker1.GetArticleList();


            /*
            chkGeneralFixes.Checked = p.Editprefs.GeneralFixes;
            chkAutoTagger.Checked = p.Editprefs.Tagger;
            chkUnicodifyWhole.Checked = p.Editprefs.Unicodify;

            cmboCategorise.SelectedIndex = p.Editprefs.Recategorisation;
            txtNewCategory.Text = p.Editprefs.NewCategory;

            cmboImages.SelectedIndex = p.Editprefs.ReImage;
            txtImageReplace.Text = p.Editprefs.ImageFind;
            txtImageWith.Text = p.Editprefs.Replace;

            chkAppend.Checked = p.Editprefs.AppendText;
            rdoAppend.Checked = p.Editprefs.Append;
            rdoPrepend.Checked = !p.Editprefs.Append;
            txtAppendMessage.Text = p.Editprefs.Text;

            nudBotSpeed.Value = (decimal)p.Editprefs.AutoDelay;
            chkQuickSave.Checked = p.Editprefs.QuickSave;
            chkSuppressTag.Checked = p.Editprefs.SuppressTag;

            chkRegExTypo.Checked = p.Editprefs.RegexTypoFix;


            chkSkipNonExistent.Checked = p.Skipoptions.SkipNonexistent;
            chkSkipNoChanges.Checked = p.Skipoptions.SkipWhenNoChanges;

            chkSkipIfContains.Checked = p.Skipoptions.SkipDoes;
            chkSkipIfNotContains.Checked = p.Skipoptions.SkipDoesNot;

            txtSkipIfContains.Text = p.Skipoptions.SkipDoesText;
            txtSkipIfNotContains.Text = p.Skipoptions.SkipDoesNotText;

            chkSkipIsRegex.Checked = p.Skipoptions.Regex;
            chkSkipCaseSensitive.Checked = p.Skipoptions.CaseSensitive;

            chkSkipWhenNoFAR.Checked = p.Skipoptions.SkipNoFindAndReplace;
            chkSkipIfNoRegexTypo.Checked = p.Skipoptions.SkipNoRegexTypoFix;
            Skip.SelectedItem = p.Skipoptions.GeneralSkip;


            foreach (string s in p.General.Summaries)
                cmboEditSummary.Items.Add(s);

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
            enableTheToolbarToolStripMenuItem.Checked = p.General.ToolBarEnabled;
            bypassRedirectsToolStripMenuItem.Checked = p.General.BypassRedirect;
            doNotAutomaticallyDoAnythingToolStripMenuItem.Checked = p.General.NoAutoChanges;
            previewInsteadOfDiffToolStripMenuItem.Checked = p.General.Preview;
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
            FlashAndBeep = p.General.FlashAndBeep;


            cModule.ModuleEnabled = p.Module.Enabled;
            cModule.Language = p.Module.Language;
            cModule.Code = p.Module.Code;

             */

            return p;
        }

        private void LoadPrefs(UserPrefs p)
        {
            SetProject(p.LanguageCode, p.Project, p.CustomProject);

            chkFindandReplace.Checked = p.FindAndReplace.Enabled;
            findAndReplace.ignoreLinks = p.FindAndReplace.IgnoreSomeText;
            findAndReplace.AppendToSummary = p.FindAndReplace.AppendSummary;
            findAndReplace.AddNew(p.FindAndReplace.Replacements);


            listMaker1.SourceText = p.List.ListSource;
            listMaker1.SelectedSource = p.List.Source;
            listMaker1.Add(p.List.ArticleList);


            chkGeneralFixes.Checked = p.Editprefs.GeneralFixes;
            chkAutoTagger.Checked = p.Editprefs.Tagger;
            chkUnicodifyWhole.Checked = p.Editprefs.Unicodify;

            cmboCategorise.SelectedIndex = p.Editprefs.Recategorisation;
            txtNewCategory.Text = p.Editprefs.NewCategory;

            cmboImages.SelectedIndex = p.Editprefs.ReImage;
            txtImageReplace.Text = p.Editprefs.ImageFind;
            txtImageWith.Text = p.Editprefs.Replace;

            chkAppend.Checked = p.Editprefs.AppendText;
            rdoAppend.Checked = p.Editprefs.Append;
            rdoPrepend.Checked = !p.Editprefs.Append;
            txtAppendMessage.Text = p.Editprefs.Text;

            nudBotSpeed.Value = (decimal)p.Editprefs.AutoDelay;
            chkQuickSave.Checked = p.Editprefs.QuickSave;
            chkSuppressTag.Checked = p.Editprefs.SuppressTag;

            chkRegExTypo.Checked = p.Editprefs.RegexTypoFix;


            chkSkipNonExistent.Checked = p.Skipoptions.SkipNonexistent;
            chkSkipNoChanges.Checked = p.Skipoptions.SkipWhenNoChanges;

            chkSkipIfContains.Checked = p.Skipoptions.SkipDoes;
            chkSkipIfNotContains.Checked = p.Skipoptions.SkipDoesNot;

            txtSkipIfContains.Text = p.Skipoptions.SkipDoesText;
            txtSkipIfNotContains.Text = p.Skipoptions.SkipDoesNotText;

            chkSkipIsRegex.Checked = p.Skipoptions.Regex;
            chkSkipCaseSensitive.Checked = p.Skipoptions.CaseSensitive;

            chkSkipWhenNoFAR.Checked = p.Skipoptions.SkipNoFindAndReplace;
            chkSkipIfNoRegexTypo.Checked = p.Skipoptions.SkipNoRegexTypoFix;
            Skip.SelectedItem = p.Skipoptions.GeneralSkip;

            foreach (string s in p.General.Summaries)
                cmboEditSummary.Items.Add(s);

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
            enableTheToolbarToolStripMenuItem.Checked = p.General.ToolBarEnabled;
            bypassRedirectsToolStripMenuItem.Checked = p.General.BypassRedirect;
            doNotAutomaticallyDoAnythingToolStripMenuItem.Checked = p.General.NoAutoChanges;
            previewInsteadOfDiffToolStripMenuItem.Checked = p.General.Preview;
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
            FlashAndBeep = p.General.FlashAndBeep;


            cModule.ModuleEnabled = p.Module.Enabled;
            cModule.Language = p.Module.Language;
            cModule.Code = p.Module.Code;
        }

        private void SavePrefs(UserPrefs p)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(UserPrefs));
                FileStream fStream = new FileStream("AWBPrefs.xml", FileMode.Create, FileAccess.Write, FileShare.None);

                xs.Serialize(fStream, p);
                fStream.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadPrefs(string path)
        {
            try
            {
                UserPrefs p;

                XmlSerializer xs = new XmlSerializer(typeof(UserPrefs));
                FileStream fStream = new FileStream("AWBPrefs.xml", FileMode.Open, FileAccess.Read, FileShare.None);

                p = (UserPrefs)xs.Deserialize(fStream);

                fStream.Close();

                LoadPrefs(p);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }

    //mother class
    [Serializable]
    public class UserPrefs
    {
        public UserPrefs() { }
        public ProjectEnum Project = ProjectEnum.wikipedia;
        public LangCodeEnum LanguageCode = LangCodeEnum.en;
        public string CustomProject = "";

        public ListPrefs List = new ListPrefs();
        public FaRPrefs FindAndReplace = new FaRPrefs();
        public EditPrefs Editprefs = new EditPrefs();
        public GeneralPrefs General = new GeneralPrefs();
        public SkipPrefs Skipoptions = new SkipPrefs();
        public ModulePrefs Module = new ModulePrefs();
    }

    //find and replace prefs
    [Serializable]
    public class FaRPrefs
    {
        public bool Enabled = false;
        public bool IgnoreSomeText = false;
        public bool AppendSummary = true;
        public List<WikiFunctions.Parse.Replacement> Replacements = new List<WikiFunctions.Parse.Replacement>();

        //need to save "Advanced find and replace" settings.
    }

    [Serializable]
    public class ListPrefs
    {
        public string ListSource = "";
        public WikiFunctions.Lists.SourceType Source = WikiFunctions.Lists.SourceType.Category;
        public List<Article> ArticleList = new List<Article>();
    }

    //the basic settings
    [Serializable]
    public class EditPrefs
    {
        public bool GeneralFixes = true;
        public bool Tagger = true;
        public bool Unicodify = true;

        public int Recategorisation = 0;
        public string NewCategory = "";

        public int ReImage = 0;
        public string ImageFind = "";
        public string Replace = "";

        public bool AppendText = false;
        public bool Append = true;
        public string Text = "";

        public int AutoDelay = 10;
        public bool QuickSave = false;
        public bool SuppressTag = false;

        public bool RegexTypoFix = false;
    }

    //skip options
    [Serializable]
    public class SkipPrefs
    {
        public bool SkipNonexistent = true;
        public bool SkipWhenNoChanges = false;

        public bool SkipDoes = false;
        public bool SkipDoesNot = false;

        public string SkipDoesText = "";
        public string SkipDoesNotText = "";

        public bool Regex = false;
        public bool CaseSensitive = false;

        public bool SkipNoFindAndReplace = false;
        public bool SkipNoRegexTypoFix = false;

        public string GeneralSkip = "";
    }

    [Serializable]
    public class GeneralPrefs
    {
        public List<string> Summaries = new List<string>();

        public string[] PasteMore = new string[10] { "", "" , "" , "" , "" , "" , "" , "" , "" , "" };

        public string FindText = "";
        public bool FindRegex = false;
        public bool FindCaseSensitive = false;

        public bool WordWrap = true;
        public bool ToolBarEnabled = false;
        public bool BypassRedirect = true;
        public bool NoAutoChanges = false;
        public bool Preview = false;
        public bool Minor = false;
        public bool Watch = false;
        public bool TimerEnabled = false;
        public bool SortInterwikiOrder = true;
        public bool AddIgnoredToLog = false;

        public bool EnhancedDiff = true;
        public bool ScrollDown = true;
        public int DiffFontSize = 150;
        public int TextBoxSize = 10;
        public string TextBoxFont = "Courier New";
        public bool LowThreadPriority = false;
        public bool FlashAndBeep = true;
    }

    [Serializable]
    public class ModulePrefs
    {
        public bool Enabled = false;
        public int Language = 0;
        public string Code = "";
    }
}
