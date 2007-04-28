using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;

using WikiFunctions.Lists;
using WikiFunctions.Logging;
using WikiFunctions.Parse;
using WikiFunctions.Plugin;
using WikiFunctions;

using FullLog;

namespace FullLog
{
    public partial class FullLogControl : Form
    {
        public FullLogControl()
        {
            InitializeComponent();
        }

        public bool ok;

        public string request
        {
            get { return txtRequest.Text; }
            set { txtRequest.Text = value; }
        }

        private void FullLogControl_Activated(object sender, EventArgs e)
        {
            txtRequest.Focus();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            ok = true;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ok = false;
            Close();
        }

        private void txtRequest_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) btnOK_Click(null, null);
        }
    }

    public class FullLogCore : IAWBPlugin
    {
        IAutoWikiBrowser MainForm;
        NoFlickerListView lvSaved;
        NoFlickerListView lvIgnored;
        string RevisionId;

        public void Initialise(IAutoWikiBrowser Form)
        {
            MainForm = Form;

            SplitContainer splitContainer1 = (SplitContainer)MainForm.Form.Controls[MainForm.Form.Controls.IndexOfKey("splitContainer1")];
            TabControl tabControl2 = (TabControl)splitContainer1.Panel2.Controls[splitContainer1.Panel2.Controls.IndexOfKey("tabControl2")];
            TabPage tpLogs = (TabPage)tabControl2.Controls[tabControl2.Controls.IndexOfKey("tpLogs")];
            LogControl LogControl1 = (LogControl)tpLogs.Controls[tpLogs.Controls.IndexOfKey("LogControl1")];

            Button btnSaveFullLog = new System.Windows.Forms.Button();
            btnSaveFullLog.Location = new System.Drawing.Point(168, 137);
            btnSaveFullLog.Name = "btnSaveFullLog";
            btnSaveFullLog.Size = new System.Drawing.Size(75, 24);
            btnSaveFullLog.TabIndex = 23;
            btnSaveFullLog.Text = "Save full log";
            btnSaveFullLog.UseVisualStyleBackColor = true;
            btnSaveFullLog.Click += new System.EventHandler(SaveFullLog);
            LogControl1.Controls.Add(btnSaveFullLog);

            lvSaved = (NoFlickerListView)LogControl1.Controls[LogControl1.Controls.IndexOfKey("lvSaved")];
            lvIgnored = (NoFlickerListView)LogControl1.Controls[LogControl1.Controls.IndexOfKey("lvIgnored")];

            ColumnHeader colSuccessRevId = new System.Windows.Forms.ColumnHeader();
            colSuccessRevId.Text = "Revision Id";
            lvSaved.Columns.Add(colSuccessRevId);

            MainForm.WebControl.Saved += FLSaved;
        }

        public string Name
        {
            get { return "FullLog"; }
        }

        public string ProcessArticle(IAutoWikiBrowser sender, ProcessArticleEventArgs ProcessArticleEventArgs)
        {
            ProcessArticleEventArgs.EditSummary = "";
            ProcessArticleEventArgs.Skip = false;

            if (sender.WebControl.Document == null)
            {
                RevisionId = "";
            }
            else
            {
                Regex RevIdRegex = new Regex("var wgCurRevisionId = \"(\\d+)\";", RegexOptions.Compiled);
                Match m = RevIdRegex.Match(sender.WebControl.DocumentText);

                if (m.Groups[1].Value == "null")
                {
                    RevisionId = "";
                }
                else
                {
                    RevisionId = m.Groups[1].Value;
                }
            }

            return ProcessArticleEventArgs.ArticleText;
        }

        public void LoadSettings(object[] Prefs) { }
        public object[] SaveSettings() { return null; }
        public void Reset() { }
        public void Nudge(out bool Cancel) { Cancel = false; }
        public void Nudged(int Nudges) { }

        private void FLSaved()
        {
            lvSaved.Items[0].SubItems.Add(RevisionId);
        }

        private void SaveFullLog(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveLogDialog = new System.Windows.Forms.SaveFileDialog();
                saveLogDialog.DefaultExt = "txt";
                saveLogDialog.Filter = "Plain text file|*.txt";
                saveLogDialog.Title = "Save full log";
                if (saveLogDialog.ShowDialog() == DialogResult.OK)
                {
                    string strLogFile = saveLogDialog.FileName;
                    StreamWriter sw = new StreamWriter(strLogFile, false, Encoding.UTF8);

                    StringBuilder strLog = new StringBuilder("");
                    string logLine;

                    string request;
                    FullLogControl ctl = new FullLogControl();
                    ctl.request = MainForm.EditSummary.Text;
                    ctl.ShowDialog();
                    if (ctl.ok)
                        request = ctl.request;
                    else
                        request = MainForm.EditSummary.Text;
                    logLine = "== " + request.Trim() + " ==";
                    strLog.AppendLine(logLine);

                    DateTime dateEdit;
                    DateTime dateStart = new DateTime(2099, 12, 31);
                    DateTime dateEnd = new DateTime(2000, 1, 1);
                    foreach (AWBLogListener log in lvSaved.Items)
                    {
                        dateEdit = Convert.ToDateTime(log.TimeStamp);
                        if (dateEdit.CompareTo(dateStart) < 0)
                            dateStart = dateEdit;
                        if (dateEdit.CompareTo(dateEnd) > 0)
                            dateEnd = dateEdit;
                    }
                    foreach (AWBLogListener log in lvIgnored.Items)
                    {
                        dateEdit = Convert.ToDateTime(log.TimeStamp);
                        if (dateEdit.CompareTo(dateStart) < 0)
                            dateStart = dateEdit;
                        if (dateEdit.CompareTo(dateEnd) > 0)
                            dateEnd = dateEdit;
                    }

                    logLine = "Begin: " + dateStart.ToString() + "<br />\r\n";
                    logLine += "End: " + dateEnd.ToString() + "<br />\r\n";
                    logLine += "History: <span class=\"plainlinks\">[{{fullurl:Special:Contributions"
                        + "|target=" + Variables.User.Name + "&offset="
                        + Regex.Replace(dateEnd.ToString("u"), "\\D", "") + "}} "
                        + "Special:Contributions/" + Variables.User.Name + "]</span><br /><br />";
                    strLog.AppendLine(logLine);

                    logLine = "Mode: ";
                    if (!MainForm.BotModeCheckbox.Checked)
                        logLine += "semi-";
                    logLine += "automatic<br /><br />";
                    strLog.AppendLine(logLine);

                    logLine = "Program used: [[Wikipedia:AutoWikiBrowser|AutoWikiBrowser]] version "
                        + MainForm.Version
                        + " modified by [[:fr:User:TiChou|TiChou]]<br /><br />";
                    strLog.AppendLine(logLine);

                    Control.ControlCollection OptionsTabControls = MainForm.OptionsTab.Controls;

                    GroupBox groupBox6 = (GroupBox)OptionsTabControls[OptionsTabControls.IndexOfKey("groupBox6")];
                    CheckBox chkUnicodifyWhole = (CheckBox)groupBox6.Controls[groupBox6.Controls.IndexOfKey("chkUnicodifyWhole")];
                    GroupBox groupBox13 = (GroupBox)OptionsTabControls[OptionsTabControls.IndexOfKey("groupBox13")];
                    CheckBox chkRegExTypo = (CheckBox)groupBox13.Controls[groupBox13.Controls.IndexOfKey("chkRegExTypo")];
                    if (MainForm.ApplyGeneralFixesCheckBox.Checked || MainForm.AutoTagCheckBox.Checked ||
                        chkUnicodifyWhole.Checked || chkRegExTypo.Checked)
                    {
                        logLine = "Activated AWB options:\r\n";
                        if (MainForm.ApplyGeneralFixesCheckBox.Checked)
                            logLine += "*Apply general fixes\r\n";
                        if (MainForm.AutoTagCheckBox.Checked)
                            logLine += "*Auto tag\r\n";
                        if (chkUnicodifyWhole.Checked)
                            logLine += "*Unicodify whole article\r\n";
                        if (chkRegExTypo.Checked)
                            logLine += "*[[Wikipedia:AutoWikiBrowser/Typos|RegexTypoFix]]\r\n";

                        logLine += "<br />";
                        strLog.AppendLine(logLine);
                    }

                    GroupBox groupBox1 = (GroupBox)OptionsTabControls[OptionsTabControls.IndexOfKey("groupBox1")];
                    CheckBox chkFindandReplace = (CheckBox)groupBox1.Controls[groupBox1.Controls.IndexOfKey("chkFindandReplace")];
                    if (chkFindandReplace.Checked)
                    {
                        List<Replacement> Replacements = MainForm.FindandReplace.GetList();
                        if (Replacements.Count > 0)
                        {
                            logLine = "[[Regular expression|Regex]] used for text modification:\r\n";
                            logLine += "<code>\r\n";

                            foreach (Replacement rep in Replacements)
                            {
                                if (!rep.Enabled)
                                    continue;

                                logLine += "*s/<span style=\"color: #4C6099;\"><nowiki>" + HttpUtility.HtmlEncode(rep.Find) + "</nowiki></span>"
                                        + "/<span style=\"color: #99694C;\"><nowiki>" + HttpUtility.HtmlEncode(rep.Replace) + "</nowiki></span>/";

                                if ((rep.RegularExpressinonOptions & RegexOptions.IgnoreCase) != 0)
                                    logLine += "i";
                                if ((rep.RegularExpressinonOptions & RegexOptions.Multiline) != 0)
                                    logLine += "m";
                                if ((rep.RegularExpressinonOptions & RegexOptions.Singleline) != 0)
                                    logLine += "s";

                                logLine += "\r\n";
                            }

                            logLine += "</code><br />";
                            strLog.AppendLine(logLine);
                        }

                        string[] Templates = MainForm.SubstTemplates.TemplateList;
                        if (Templates.Length > 0)
                        {
                            logLine = "[[Help:Template|Templates]] substituted:\r\n";

                            foreach (string template in Templates)
                                if (template.Trim() != "")
                                    logLine += "*{{m|" + template + "}}\r\n";

                            logLine += "<br />";
                            strLog.AppendLine(logLine);
                        }
                    }

                    Control.ControlCollection MoreOptionsControls = MainForm.MoreOptionsTab.Controls;

                    GroupBox groupBox4 = (GroupBox)MoreOptionsControls[MoreOptionsControls.IndexOfKey("groupBox4")];
                    CheckBox chkAppend = (CheckBox)groupBox4.Controls[groupBox4.Controls.IndexOfKey("chkAppend")];
                    RadioButton rdoAppend = (RadioButton)groupBox4.Controls[groupBox4.Controls.IndexOfKey("rdoAppend")];
                    TextBox txtAppendMessage = (TextBox)groupBox4.Controls[groupBox4.Controls.IndexOfKey("txtAppendMessage")];
                    if (chkAppend.Checked)
                    {
                        if (rdoAppend.Checked)
                            logLine = "Append text:";
                        else
                            logLine = "Prepend text:";
                        logLine += "\r\n<pre>" + HttpUtility.HtmlEncode(txtAppendMessage.Text) + "</pre><br />";
                        strLog.AppendLine(logLine);
                    }

                    GroupBox groupBox5 = (GroupBox)MoreOptionsControls[MoreOptionsControls.IndexOfKey("groupBox5")];
                    ComboBox cmboCategorise = (ComboBox)groupBox5.Controls[groupBox5.Controls.IndexOfKey("cmboCategorise")];
                    TextBox txtNewCategory = (TextBox)groupBox5.Controls[groupBox5.Controls.IndexOfKey("txtNewCategory")];
                    TextBox txtNewCategory2 = (TextBox)groupBox5.Controls[groupBox5.Controls.IndexOfKey("txtNewCategory2")];
                    if (cmboCategorise.SelectedIndex > 0)
                    {
                        switch (cmboCategorise.SelectedIndex)
                        {
                            case 1:
                                logLine = "[[Help:Category|Category]] '''[[:Category:" + txtNewCategory.Text + "|"
                                    + txtNewCategory.Text + "]]''' replaced with category '''[[:Category:"
                                    + txtNewCategory2.Text + "|" + txtNewCategory2.Text + "]]'''";
                                break;
                            case 2:
                                logLine = "[[Help:Category|Category]] '''[[:Category:" + txtNewCategory.Text + "|"
                                    + txtNewCategory.Text + "]]''' added";
                                break;
                            case 3:
                                logLine = "[[Help:Category|Category]] '''[[:Category:" + txtNewCategory.Text + "|"
                                    + txtNewCategory.Text + "]]''' removed";
                                break;
                            default:
                                break;
                        }

                        logLine += "<br /><br />";
                        strLog.AppendLine(logLine);
                    }

                    GroupBox ImageGroupBox = (GroupBox)MoreOptionsControls[MoreOptionsControls.IndexOfKey("ImageGroupBox")];
                    ComboBox cmboImages = (ComboBox)ImageGroupBox.Controls[ImageGroupBox.Controls.IndexOfKey("cmboImages")];
                    TextBox txtImageReplace = (TextBox)ImageGroupBox.Controls[ImageGroupBox.Controls.IndexOfKey("txtImageReplace")];
                    TextBox txtImageWith = (TextBox)ImageGroupBox.Controls[ImageGroupBox.Controls.IndexOfKey("txtImageWith")];
                    if (cmboImages.SelectedIndex > 0)
                    {
                        switch (cmboImages.SelectedIndex)
                        {
                            case 1:
                                logLine = "[[Help:Images and other uploaded files|Image]] '''[[:Image:" + txtImageReplace.Text + "|"
                                    + txtImageReplace.Text + "]]''' replaced with image '''[[:Image:"
                                    + txtImageWith.Text + "|" + txtImageWith.Text + "]]'''";
                                break;
                            case 2:
                                logLine = "[[Help:Images and other uploaded files|Image]] '''[[:Image:" + txtImageReplace.Text + "|"
                                    + txtImageReplace.Text + "]]''' removed";
                                break;
                            case 3:
                                logLine = "[[Help:Images and other uploaded files|Image]] '''[[:Image:" + txtImageReplace.Text + "|"
                                    + txtImageReplace.Text + "]]''' commented out with comment \""
                                    + txtImageWith.Text + "\"";
                                break;
                            default:
                                break;
                        }

                        logLine += "<br /><br />";
                        strLog.AppendLine(logLine);
                    }

                    if (MainForm.CustomModule != null)
                    {
                        logLine = "Module loaded:\r\n<pre>" + HttpUtility.HtmlEncode(MainForm.CustomModule) + "</pre><br />";
                        strLog.AppendLine(logLine);
                    }

                    string title, timestamp, revid, skippedby, reason;
                    logLine = "Articles successfully saved:\r\n";
                    foreach (ListViewItem log in lvSaved.Items)
                    {
                        title = log.SubItems[0].Text;
                        timestamp = log.SubItems[1].Text;
                        revid = log.SubItems[2].Text;
                        logLine += "#" + timestamp + " [[:" + title + "]] "
                            + "<small class=\"plainlinks\">([{{fullurl:" + title
                            + "|diff=next&oldid=" + revid + "}} diff] • "
                            + "[{{fullurl:" + title + "|action=history}} hist])</small>\r\n";
                    }
                    logLine += "<br />";
                    strLog.AppendLine(logLine);

                    logLine = "Articles skipped:\r\n";
                    foreach (ListViewItem log in lvIgnored.Items)
                    {
                        title = log.SubItems[0].Text;
                        timestamp = log.SubItems[1].Text;
                        skippedby = log.SubItems[2].Text;
                        reason = log.SubItems[3].Text;
                        logLine += "#" + timestamp + " [[:" + title + "]] ";
                        logLine += "<small>(" + skippedby + " : " + reason + ")</small>\r\n";
                    }
                    strLog.Append(logLine);

                    sw.Write(strLog);
                    sw.Close();
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message, "File error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}