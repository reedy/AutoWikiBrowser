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
            saveSettings(Application.StartupPath + "\\Default.xml");
        }

        private void saveSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveXML.ShowDialog() != DialogResult.OK)
                return;

            saveSettings(saveXML.FileName);
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

        private void saveSettings(string FileName)
        {
            try
            {
                strSettingsFile = " - " + FileName.Remove(0, FileName.LastIndexOf("\\") + 1);
                this.Text = "AutoWikiBrowser" + strSettingsFile;

                XmlTextWriter textWriter = new XmlTextWriter(FileName, UTF8Encoding.UTF8);
                // Opens the document
                textWriter.Formatting = Formatting.Indented;
                textWriter.WriteStartDocument();

                // Write first element
                textWriter.WriteStartElement("Settings");
                textWriter.WriteAttributeString("program", "AWB");
                textWriter.WriteAttributeString("schema", "2");

                textWriter.WriteStartElement("Project");
                textWriter.WriteStartElement("projectlang");
                textWriter.WriteAttributeString("proj", Variables.Project);
                textWriter.WriteAttributeString("lang", Variables.LangCode);
                textWriter.WriteAttributeString("custom", Variables.CustomProject);
                textWriter.WriteEndElement();
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("Options");

                textWriter.WriteStartElement("selectsource");
                int x = (int)listMaker1.SelectedSource;
                textWriter.WriteAttributeString("index", x.ToString());
                textWriter.WriteAttributeString("text", listMaker1.SourceText);
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("general");
                textWriter.WriteAttributeString("general", chkGeneralFixes.Checked.ToString());
                textWriter.WriteAttributeString("tagger", chkAutoTagger.Checked.ToString());
                textWriter.WriteAttributeString("unicodifyer", chkUnicodifyWhole.Checked.ToString());
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("categorisation");
                textWriter.WriteAttributeString("index", cmboCategorise.SelectedIndex.ToString());
                textWriter.WriteAttributeString("text", txtNewCategory.Text);
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("skip");
                textWriter.WriteAttributeString("does", chkSkipIfContains.Checked.ToString());
                textWriter.WriteAttributeString("doesnot", chkSkipIfNotContains.Checked.ToString());
                textWriter.WriteAttributeString("regex", chkSkipIsRegex.Checked.ToString());
                textWriter.WriteAttributeString("casesensitive", chkSkipCaseSensitive.Checked.ToString());
                textWriter.WriteAttributeString("doestext", txtSkipIfContains.Text);
                textWriter.WriteAttributeString("doesnottext", txtSkipIfNotContains.Text);
                textWriter.WriteAttributeString("moreindex", Skip.SelectedItem.ToString());
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("message");
                textWriter.WriteAttributeString("enabled", chkAppend.Checked.ToString());
                textWriter.WriteAttributeString("text", txtAppendMessage.Text);
                textWriter.WriteAttributeString("append", rdoAppend.Checked.ToString());
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("automode");
                textWriter.WriteAttributeString("delay", nudBotSpeed.Value.ToString());
                textWriter.WriteAttributeString("quicksave", chkQuickSave.Checked.ToString());
                textWriter.WriteAttributeString("suppresstag", chkSuppressTag.Checked.ToString());
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("imager");
                textWriter.WriteAttributeString("index", cmboImages.SelectedIndex.ToString());
                textWriter.WriteAttributeString("replace", txtImageReplace.Text);
                textWriter.WriteAttributeString("with", txtImageWith.Text);
                textWriter.WriteEndElement();
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("regextypofix");
                textWriter.WriteStartElement("regextypofixproperties");
                textWriter.WriteAttributeString("enabled", chkRegExTypo.Checked.ToString());
                textWriter.WriteAttributeString("skipnofixed", chkSkipIfNoRegexTypo.Checked.ToString());
                textWriter.WriteEndElement();
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("FindAndReplaceSettings");
                findAndReplace.WriteToXml(textWriter, chkFindandReplace.Checked, chkSkipWhenNoFAR.Checked);
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("FindAndReplace");
                replaceSpecial.WriteToXml(textWriter, chkFindandReplace.Checked);
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("startoptions");
                int j = 0;
                while (j < cmboEditSummary.Items.Count)
                {
                    textWriter.WriteStartElement("summary");
                    textWriter.WriteAttributeString("text", cmboEditSummary.Items[j].ToString());
                    textWriter.WriteEndElement();
                    j++;
                }

                if (!cmboEditSummary.Items.Contains(cmboEditSummary.Text))
                {
                    textWriter.WriteStartElement("summary");
                    textWriter.WriteAttributeString("text", cmboEditSummary.Text);
                    textWriter.WriteEndElement();
                }

                textWriter.WriteStartElement("summaryindex");
                textWriter.WriteAttributeString("index", cmboEditSummary.Text);
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("find");
                textWriter.WriteAttributeString("text", txtFind.Text);
                textWriter.WriteAttributeString("regex", chkFindRegex.Checked.ToString());
                textWriter.WriteAttributeString("casesensitive", chkFindCaseSensitive.Checked.ToString());
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("menu");

                textWriter.WriteStartElement("wordwrap");
                textWriter.WriteAttributeString("enabled", wordWrapToolStripMenuItem1.Checked.ToString());
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("toolbar");
                textWriter.WriteAttributeString("enabled", enableToolBar.ToString());
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("bypass");
                textWriter.WriteAttributeString("enabled", bypassRedirectsToolStripMenuItem.Checked.ToString());
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("ingnorenonexistent");
                textWriter.WriteAttributeString("enabled", chkSkipNonExistent.Checked.ToString());
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("noautochanges");
                textWriter.WriteAttributeString("enabled", doNotAutomaticallyDoAnythingToolStripMenuItem.Checked.ToString());
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("skipnochanges");
                textWriter.WriteAttributeString("enabled", chkSkipNoChanges.Checked.ToString());
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("preview");
                textWriter.WriteAttributeString("enabled", previewInsteadOfDiffToolStripMenuItem.Checked.ToString());
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("minor");
                textWriter.WriteAttributeString("enabled", markAllAsMinorToolStripMenuItem.Checked.ToString());
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("watch");
                textWriter.WriteAttributeString("enabled", addAllToWatchlistToolStripMenuItem.Checked.ToString());
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("timer");
                textWriter.WriteAttributeString("enabled", showTimerToolStripMenuItem.Checked.ToString());
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("sortinterwiki");
                textWriter.WriteAttributeString("enabled", alphaSortInterwikiLinksToolStripMenuItem.Checked.ToString());
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("addignoredtolog");
                textWriter.WriteAttributeString("enabled", addIgnoredToLogFileToolStripMenuItem.Checked.ToString());
                textWriter.WriteEndElement();

                textWriter.WriteEndElement();

                //write plugin settings
                try
                {
                    textWriter.WriteStartElement("plugins");
                    foreach (IAWBPlugin a in AWBPlugins)
                    {
                        textWriter.WriteStartElement(a.Name.Replace(' ', '_'));
                        a.WriteXML(textWriter);
                        textWriter.WriteEndElement();
                    }
                    textWriter.WriteEndElement();
                }
                catch (Exception ex)
                {
                    throw new Exception("Problem writing plugin settings\r\n" + ex.Message);
                }

                textWriter.WriteEndElement();

                textWriter.WriteStartElement("pastemore");

                textWriter.WriteStartElement("pastemore1");
                textWriter.WriteAttributeString("text", PasteMore1.Text);
                textWriter.WriteEndElement();
                textWriter.WriteStartElement("pastemore2");
                textWriter.WriteAttributeString("text", PasteMore2.Text);
                textWriter.WriteEndElement();
                textWriter.WriteStartElement("pastemore3");
                textWriter.WriteAttributeString("text", PasteMore3.Text);
                textWriter.WriteEndElement();
                textWriter.WriteStartElement("pastemore4");
                textWriter.WriteAttributeString("text", PasteMore4.Text);
                textWriter.WriteEndElement();
                textWriter.WriteStartElement("pastemore5");
                textWriter.WriteAttributeString("text", PasteMore5.Text);
                textWriter.WriteEndElement();
                textWriter.WriteStartElement("pastemore6");
                textWriter.WriteAttributeString("text", PasteMore6.Text);
                textWriter.WriteEndElement();
                textWriter.WriteStartElement("pastemore7");
                textWriter.WriteAttributeString("text", PasteMore7.Text);
                textWriter.WriteEndElement();
                textWriter.WriteStartElement("pastemore8");
                textWriter.WriteAttributeString("text", PasteMore8.Text);
                textWriter.WriteEndElement();
                textWriter.WriteStartElement("pastemore9");
                textWriter.WriteAttributeString("text", PasteMore9.Text);
                textWriter.WriteEndElement();
                textWriter.WriteStartElement("pastemore10");
                textWriter.WriteAttributeString("text", PasteMore10.Text);
                textWriter.WriteEndElement();

                textWriter.WriteEndElement();

                textWriter.WriteStartElement("preferences");
                textWriter.WriteStartElement("preferencevalues");
                textWriter.WriteAttributeString("enhancediff", webBrowserEdit.EnhanceDiffEnabled.ToString());
                textWriter.WriteAttributeString("scrolldown", webBrowserEdit.ScrollDown.ToString());
                textWriter.WriteAttributeString("difffontsize", webBrowserEdit.DiffFontSize.ToString());
                textWriter.WriteAttributeString("textboxfontsize", txtEdit.Font.Size.ToString());
                textWriter.WriteAttributeString("textboxfont", txtEdit.Font.Name);
                textWriter.WriteAttributeString("lowthreadpriority", LowThreadPriority.ToString());
                textWriter.WriteAttributeString("flashandbeep", FlashAndBeep.ToString());
                textWriter.WriteEndElement();
                textWriter.WriteEndElement();

                // Ends the document.
                textWriter.WriteEndDocument();
                // close writer
                textWriter.Close();
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message, "File error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            lblStatusText.Text = "Settings successfully saved";
            UpdateRecentList(FileName);
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

            p.Project = ProjectEnum.wikipedia;
            p.LanguageCode = LangCodeEnum.en;
            p.CustomProject = "";


            p.FindAndReplace.Enabled = false;
            p.FindAndReplace.IgnoreSomeText = false;
            p.FindAndReplace.AppendSummary = true;
            p.FindAndReplace.Replacements = new List<WikiFunctions.Parse.Replacement>();


            p.List.ListSource = "";
            p.List.Source = WikiFunctions.Lists.SourceType.Category;
            p.List.ArticleList = new List<Article>();


            p.Editprefs.GeneralFixes = true;
            p.Editprefs.Tagger = true;
            p.Editprefs.Unicodify = true;

            p.Editprefs.Recategorisation = 0;
            p.Editprefs.NewCategory = "";

            p.Editprefs.ReImage = 0;
            p.Editprefs.ImageFind = "";
            p.Editprefs.Replace = "";

            p.Editprefs.AppendText = false;
            p.Editprefs.Append = true;
            p.Editprefs.Text = "";

            p.Editprefs.AutoDelay = 10;
            p.Editprefs.QuickSave = false;
            p.Editprefs.SuppressTag = false;

            p.Editprefs.RegexTypoFix = false;


            p.Skipoptions.SkipNonexistent = true;
            p.Skipoptions.SkipWhenNoChanges = false;

            p.Skipoptions.SkipDoes = false;
            p.Skipoptions.SkipDoesNot = false;

            p.Skipoptions.SkipDoesText = "";
            p.Skipoptions.SkipDoesNotText = "";

            p.Skipoptions.Regex = false;
            p.Skipoptions.CaseSensitive = false;

            p.Skipoptions.SkipNoFindAndReplace = false;
            p.Skipoptions.SkipNoRegexTypoFix = false;


            p.General.Summaries = new List<string>();

            p.General.PasteMore = new string[10];

            p.General.FindText = "";
            p.General.FindRegex = false;
            p.General.FindCaseSensitive = false;

            p.General.WordWrap = true;
            p.General.ToolBarEnabled = false;
            p.General.BypassRedirect = true;
            p.General.NoAutoChanges = false;
            p.General.Preview = false;
            p.General.Minor = false;
            p.General.Watch = false;
            p.General.TimerEnabled = false;
            p.General.SortInterwikiOrder = true;
            p.General.AddIgnoredToLog = false;

            p.General.EnhancedDiff = true;
            p.General.ScrollDown = true;
            p.General.DiffFontSize = 150;
            p.General.TextBoxSize = 10;
            p.General.TextBoxFont = "Courier New";
            p.General.LowThreadPriority = false;
            p.General.FlashAndBeep = true;


            p.Module.Enabled = false;
            p.Module.Language = 0;
            p.Module.Code = "";

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
    }

    [Serializable]
    public class GeneralPrefs
    {
        public List <string > Summaries = new List< string> () ;

        public string[] PasteMore = new string[10] { "", "", "", "", "", "", "", "", "", "" };

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
