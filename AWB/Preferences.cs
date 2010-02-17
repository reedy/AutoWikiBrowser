/*
Autowikibrowser
Copyright (C) 2007 Martin Richards
(C) 2008 Stephen Kennedy

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
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using WikiFunctions;
using WikiFunctions.Parse;

namespace AutoWikiBrowser
{
    internal sealed partial class MyPreferences : Form
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lang"></param>
        /// <param name="proj"></param>
        /// <param name="customproj"></param>
        /// <param name="usePHP5"></param>
        public MyPreferences(string lang, ProjectEnum proj, string customproj, bool usePHP5)
        {
            InitializeComponent();

            foreach (ProjectEnum l in Enum.GetValues(typeof(ProjectEnum)))
                cmboProject.Items.Add(l);

            cmboProject.SelectedItem = proj;

            cmboProject_SelectedIndexChanged(null, null);

            cmboLang.SelectedItem = lang.ToLower();

            cmboCustomProject.Items.Clear();
            foreach (string s in Properties.Settings.Default.CustomWikis.Split('|'))
            {
                if (!cmboCustomProject.Items.Contains(s))
                    cmboCustomProject.Items.Add(s);
            }

            cmboCustomProject.Text = customproj;

            PrefPHP5 = usePHP5;

            chkSupressAWB.Enabled = (cmboProject.Text == "custom" || cmboProject.Text == "wikia");

            chkAlwaysConfirmExit.Checked = Properties.Settings.Default.AskForTerminate;
            chkPrivacy.Checked = !Properties.Settings.Default.Privacy;

            if (Globals.UsingMono)
            {
                chkFlash.Enabled = false;
                chkFlash.Checked = false;
            }
        }

        #region Language and project

        public string Language
        {
            get { return cmboLang.SelectedItem.ToString(); }
        }

        public ProjectEnum Project
        {
            get { return (ProjectEnum)Enum.Parse(typeof(ProjectEnum), cmboProject.SelectedItem.ToString()); }
        }

        public string CustomProject
        {
            get
            {
                FixCustomProject();
                return cmboCustomProject.Text;
            }
        }

        private void txtCustomProject_Leave(object sender, EventArgs e)
        {
            FixCustomProject();
        }

        private static readonly Regex CustomProjectRegex = new Regex(@"^.*?://(?:([\w/\.-]+?)/(?:index|api).php|([\w/\.-]+)).*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private void FixCustomProject()
        {
            string proj = CustomProjectRegex.Replace(cmboCustomProject.Text.Trim(), "$1$2");

            proj = proj.TrimEnd('/');
            if (Project == ProjectEnum.custom) // we shouldn't screw up Wikia
            {
                proj += "/";
            }

            cmboCustomProject.Text = proj;
        }

        private void cmboProject_SelectedIndexChanged(object sender, EventArgs e)
        {
            ProjectEnum prj = Project;

            //disable language selection for single language projects
            cmboLang.Enabled = (prj < ProjectEnum.species);

            string temp = (cmboLang.SelectedItem != null) ? cmboLang.SelectedItem.ToString() : "";

            cmboLang.Items.Clear();
            List<string> langs;

            switch (prj)
            {
                case ProjectEnum.wikipedia:
                    langs = SiteMatrix.WikipediaLanguages;
                    break;

                case ProjectEnum.wiktionary:
                    langs = SiteMatrix.WiktionaryLanguages;
                    break;

                case ProjectEnum.wikibooks:
                    langs = SiteMatrix.WikibooksLanguages;
                    break;

                case ProjectEnum.wikinews:
                    langs = SiteMatrix.WikinewsLanguages;
                    break;

                case ProjectEnum.wikiquote:
                    langs = SiteMatrix.WikiquoteLanguages;
                    break;

                case ProjectEnum.wikisource:
                    langs = SiteMatrix.WikisourceLanguages;
                    break;

                case ProjectEnum.wikiversity:
                    langs = SiteMatrix.WikiversityLanguages;
                    break;

                default:
                    langs = SiteMatrix.Languages;
                    break;
            }
            cmboLang.Items.AddRange(langs.ToArray());

            if (!string.IsNullOrEmpty(temp))
            {
                cmboLang.SelectedIndex = cmboLang.Items.IndexOf(temp);
            }

            if (prj == ProjectEnum.custom || prj == ProjectEnum.wikia)
            {
                cmboCustomProject.Visible = true;
                cmboLang.Visible = false;
                lblLang.Text = "http://";
                lblPostfix.Text = prj == ProjectEnum.wikia ? ".wikia.com" : "";
                cmboCustomProjectChanged(null, null);

                chkSupressAWB.Enabled = true;

                chkPHP5Ext.Enabled = (prj == ProjectEnum.custom);

                return;
            }

            lblPostfix.Text = "";
            cmboCustomProject.Visible = false;
            cmboLang.Visible = true;
            lblLang.Text = "Language:";
            btnOK.Enabled = true;
            chkSupressAWB.Enabled = false;
        }

        private void cmboCustomProjectChanged(object sender, EventArgs e)
        {
            ProjectEnum prj = (ProjectEnum) Enum.Parse(typeof (ProjectEnum), cmboProject.SelectedItem.ToString());
            if (prj == ProjectEnum.custom || prj == ProjectEnum.wikia)
                btnOK.Enabled = (!string.IsNullOrEmpty(cmboCustomProject.Text));
            else
                btnOK.Enabled = true;
        }

        #endregion

        #region Other

        public Font TextBoxFont;

        private void btnTextBoxFont_Click(object sender, EventArgs e)
        {
            fontDialog.Font = TextBoxFont;
            if (fontDialog.ShowDialog() == DialogResult.OK)
                TextBoxFont = fontDialog.Font;
        }

        public bool PrefSuppressUsingAWB
        {
            get { return chkSupressAWB.Checked; }
            set { chkSupressAWB.Checked = chkSupressAWB.Enabled && value; }
        }

        public bool PrefAddUsingAWBOnArticleAction
        {
            get { return chkAddUsingAWBToActionSummaries.Checked; }
            set { chkAddUsingAWBToActionSummaries.Checked = value; }
        }

        public bool LowThreadPriority
        {
            get { return chkLowPriority.Checked; }
            set { chkLowPriority.Checked = value; }
        }

        public bool PrefFlash
        {
            get { return chkFlash.Checked; }
            set { chkFlash.Checked = value; }
        }

        public bool PrefBeep
        {
            get { return chkBeep.Checked; }
            set { chkBeep.Checked = value; }
        }

        public bool PrefMinimize
        {
            get { return chkMinimize.Checked; }
            set { chkMinimize.Checked = value; }
        }

        public bool PrefSaveArticleList
        {
            get { return chkSaveArticleList.Checked; }
            set { chkSaveArticleList.Checked = value; }
        }

        public bool PrefAutoSaveEditBoxEnabled
        {
            get { return chkAutoSaveEdit.Checked; }
            set { chkAutoSaveEdit.Checked = btnSetFile.Enabled = nudEditBoxAutosave.Enabled = txtAutosave.Enabled = lblAutosaveFile.Enabled = value; }
        }

        public decimal PrefAutoSaveEditBoxPeriod
        {
            get { return nudEditBoxAutosave.Value; }
            set { nudEditBoxAutosave.Value = value; }
        }

        public string PrefAutoSaveEditBoxFile
        {
            get { return txtAutosave.Text; }
            set { txtAutosave.Text = value; }
        }

        public bool EnableLogging
        {
            get { return chkEnableLogging.Checked; }
            set { chkEnableLogging.Checked = value; }
        }

        //TODO:Reinstate/Use?
        public List<string> PrefCustomWikis
        {
            get
            {
                List<String> temp = new List<String> {cmboCustomProject.Text};
                foreach (object a in cmboCustomProject.Items)
                    temp.Add(a.ToString());
                return temp;
            }
            set
            {
                cmboCustomProject.Items.Clear();
                foreach (string temp in value)
                    cmboCustomProject.Items.Add(temp);
            }
        }

        public bool PrefPHP5
        {
            get { return chkPHP5Ext.Checked; }
            set { chkPHP5Ext.Checked = value; }
        }

        public bool PrefIgnoreNoBots
        {
            get { return chkIgnoreNoBots.Checked; }
            set { chkIgnoreNoBots.Checked = value; }
        }

        public bool PrefShowTimer
        {
            get { return chkShowTimer.Checked; }
            set { chkShowTimer.Checked = value; }
        }

        public int PrefListComparerUseCurrentArticleList
        {
            get { return cmboListComparer.SelectedIndex; }
            set { cmboListComparer.SelectedIndex = value; }
        }

        public int PrefListSplitterUseCurrentArticleList
        {
            get { return cmboListSplitter.SelectedIndex; }
            set { cmboListSplitter.SelectedIndex = value; }
        }

        public int PrefDBScannerUseCurrentArticleList
        {
            get { return cmboDBScanner.SelectedIndex; }
            set { cmboDBScanner.SelectedIndex = value; }
        }

        public int PrefOnLoad
        {
            get { return cmboOnLoad.SelectedIndex; }   
            set { cmboOnLoad.SelectedIndex = value;}
        }

        public bool PrefDiffInBotMode
        {
            get { return chkDiffInBotMode.Checked; }
            set { chkDiffInBotMode.Checked = value; }
        }

        public bool PrefClearPageListOnProjectChange
        {
            get { return chkEmptyOnProjectChange.Checked; }
            set { chkEmptyOnProjectChange.Checked = value; }
        }
        #endregion

        private void chkAutoSaveEdit_CheckedChanged(object sender, EventArgs e)
        {
            PrefAutoSaveEditBoxEnabled = chkAutoSaveEdit.Checked;
        }

        private void btnSetFile_Click(object sender, EventArgs e)
        {
            saveFile.InitialDirectory = Application.StartupPath;
            saveFile.ShowDialog();
            if (!string.IsNullOrEmpty(saveFile.FileName))
                txtAutosave.Text = saveFile.FileName;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            bool save = false;

            if (chkAutoSaveEdit.Checked && string.IsNullOrEmpty(txtAutosave.Text))
                chkAutoSaveEdit.Checked = false;

            if (cmboProject.Text == "custom" && !string.IsNullOrEmpty(cmboCustomProject.Text))
            {
                FixCustomProject();
                cmboCustomProject.Items.Add(cmboCustomProject.Text);
            }

            StringBuilder customs = new StringBuilder();
            foreach (string s in cmboCustomProject.Items)
            {
                if (!string.IsNullOrEmpty(s.Trim()))
                    customs.Append(s + "|");
            }

            string tmp = customs.ToString();
            Properties.Settings.Default.CustomWikis = (tmp.Length == 0) ? "" : tmp.Substring(0, tmp.LastIndexOf('|'));

            if (Properties.Settings.Default.CustomWikis.Length > 0)
                save = true;

            if (Properties.Settings.Default.AskForTerminate != chkAlwaysConfirmExit.Checked)
            {
                Properties.Settings.Default.AskForTerminate = chkAlwaysConfirmExit.Checked;
                save = true;
            }
            if (Properties.Settings.Default.Privacy == chkPrivacy.Checked)
            {
                Properties.Settings.Default.Privacy = !chkPrivacy.Checked;
                save = true;
            }

            if (save) Properties.Settings.Default.Save();
        }

        private void cmboOnLoad_SelectedIndexChanged(object sender, EventArgs e)
        {
            chkDiffInBotMode.Enabled = (cmboOnLoad.SelectedIndex == 0);
        }
    }
}
