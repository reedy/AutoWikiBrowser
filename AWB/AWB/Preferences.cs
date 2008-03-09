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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WikiFunctions;

namespace AutoWikiBrowser
{
    internal sealed partial class MyPreferences : Form
    {
        public MyPreferences(LangCodeEnum lang, ProjectEnum proj, string customproj,
            Font textFont, bool lowPriority, bool flash, bool beep, bool minimize,
            bool saveArticleList, decimal timeOut, bool autoSaveEditBox,
            string autoSaveEditBoxFile, decimal autoSaveEditBoxPeriod, bool suppressUsingAWB)
        {
            InitializeComponent();

            foreach (LangCodeEnum l in Enum.GetValues(typeof(LangCodeEnum)))
                cmboLang.Items.Add(l.ToString().ToLower());

            foreach (ProjectEnum l in Enum.GetValues(typeof(ProjectEnum)))
                cmboProject.Items.Add(l);

            cmboLang.SelectedItem = lang.ToString().ToLower();
            cmboProject.SelectedItem = proj;

            cmboCustomProject.Items.Clear();
            foreach (string s in Properties.Settings.Default.CustomWikis.Split('|'))
            {
                if (!cmboCustomProject.Items.Contains(s))
                    cmboCustomProject.Items.Add(s);
            }

            cmboCustomProject.Text = customproj;

            TextBoxFont = textFont;
            LowThreadPriority = lowPriority;
            PerfFlash = flash;
            PerfBeep = beep;
            PerfMinimize = minimize;
            PerfSaveArticleList = saveArticleList;
            PerfTimeOutLimit = timeOut;

            PerfAutoSaveEditBoxEnabled = autoSaveEditBox;
            PerfAutoSaveEditBoxFile = autoSaveEditBoxFile;
            PerfAutoSaveEditBoxPeriod = autoSaveEditBoxPeriod;

            chkSupressAWB.Enabled = (cmboProject.Text == "custom" || cmboProject.Text == "wikia");
            if (chkSupressAWB.Enabled)
                chkSupressAWB.Checked = suppressUsingAWB;
            else
                chkSupressAWB.Checked = false;

            cmboProject_SelectedIndexChanged(null, null);

            chkAlwaysConfirmExit.Checked = Properties.Settings.Default.DontAskForTerminate;
            PrivacyCheckBox.Checked = !Properties.Settings.Default.Privacy;
        }

        #region Language and project

        public LangCodeEnum Language
        {
            get
            {
                return Variables.ParseLanguage(cmboLang.SelectedItem.ToString());
            }
        }

        public ProjectEnum Project
        {
            get { return (ProjectEnum)Enum.Parse(typeof(ProjectEnum), cmboProject.SelectedItem.ToString()); }
        }
        public string CustomProject
        {
            get
            {
                string s = cmboCustomProject.Text.Trim();
                if (Project == ProjectEnum.wikia) return s;
                return s.EndsWith("/") ? s : s + "/";
            }
        }

        private void txtCustomProject_Leave(object sender, EventArgs e)
        {
            FixCustomProject();
        }

        private void FixCustomProject()
        {
            cmboCustomProject.Text = Regex.Replace(cmboCustomProject.Text, "^http://", "", RegexOptions.IgnoreCase);
            cmboCustomProject.Text = cmboCustomProject.Text.TrimEnd('/');
            cmboCustomProject.Text = cmboCustomProject.Text + "/";
        }

        private void cmboProject_SelectedIndexChanged(object sender, EventArgs e)
        {
            //disable language selection for single language projects
            cmboLang.Enabled = cmboProject.SelectedIndex <= cmboProject.Items.Count - 6;

            lblPostfix.Text = "";
            ProjectEnum prj = (ProjectEnum)Enum.Parse(typeof(ProjectEnum), cmboProject.SelectedItem.ToString());
            if (prj == ProjectEnum.custom || prj == ProjectEnum.wikia)
            {
                cmboCustomProject.Visible = true;
                cmboLang.Visible = false;
                lblLang.Text = "http://";
                if (prj == ProjectEnum.wikia) lblPostfix.Text = ".wikia.com";
                cmboCustomProjectChanged(null, null);
            }
            else
            {
                cmboCustomProject.Visible = false;
                cmboLang.Visible = true;
                lblLang.Text = "Language:";
                btnOK.Enabled = true;
            }

            chkSupressAWB.Enabled = (prj == ProjectEnum.custom || prj == ProjectEnum.wikia);
        }

        private void cmboCustomProjectChanged(object sender, EventArgs e)
        {
            btnOK.Enabled = (!string.IsNullOrEmpty(cmboCustomProject.Text));
        }

        #endregion

        #region Other

        Font f;
        public Font TextBoxFont
        {
            get { return f; }
            set { f = value; }
        }

        private void btnTextBoxFont_Click(object sender, EventArgs e)
        {
            fontDialog.Font = TextBoxFont;
            if (fontDialog.ShowDialog() == DialogResult.OK)
                TextBoxFont = fontDialog.Font;
        }

        public bool PerfSupressUsingAWB
        {
            get { return chkSupressAWB.Checked; }
            set { chkSupressAWB.Checked = value; }
        }

        public bool LowThreadPriority
        {
            get { return chkLowPriority.Checked; }
            set { chkLowPriority.Checked = value; }
        }

        public bool PerfFlash
        {
            get { return chkFlash.Checked; }
            set { chkFlash.Checked = value; }
        }

        public bool PerfBeep
        {
            get { return chkBeep.Checked; }
            set { chkBeep.Checked = value; }
        }

        public bool PerfMinimize
        {
            get { return chkMinimize.Checked; }
            set { chkMinimize.Checked = value; }
        }

        public bool PerfSaveArticleList
        {
            get { return chkSaveArticleList.Checked; }
            set { chkSaveArticleList.Checked = value; }
        }

        public decimal PerfTimeOutLimit
        {
            get { return numTimeOutLimit.Value; }
            set { numTimeOutLimit.Value = value; }
        }

        public bool PerfAutoSaveEditBoxEnabled
        {
            get { return chkAutoSaveEdit.Checked; }
            set { chkAutoSaveEdit.Checked = btnSetFile.Enabled = numEditBoxAutosave.Enabled = label9.Enabled = label10.Enabled = value; }
        }

        public decimal PerfAutoSaveEditBoxPeriod
        {
            get { return numEditBoxAutosave.Value; }
            set { numEditBoxAutosave.Value = value; }
        }

        public List<String> PerfCustomWikis
        {
            get
            {
                List<String> Temp = new List<String>();
                Temp.Add(cmboCustomProject.Text);
                foreach (object a in cmboCustomProject.Items)
                    Temp.Add(a.ToString());
                return Temp;
            }
            set
            {
                cmboCustomProject.Items.Clear();
                foreach (string Temp in value)
                    cmboCustomProject.Items.Add(Temp);
            }
        }

        public string PerfAutoSaveEditBoxFile
        {
            get { return txtAutosave.Text; }
            set { txtAutosave.Text = value; }
        }
        #endregion

        private void chkAutoSaveEdit_CheckedChanged(object sender, EventArgs e)
        {
            PerfAutoSaveEditBoxEnabled = chkAutoSaveEdit.Checked;
        }

        private void btnSetFile_Click(object sender, EventArgs e)
        {
            saveFile.InitialDirectory = Application.StartupPath;
            saveFile.ShowDialog();
            if (!string.IsNullOrEmpty(saveFile.FileName))
                txtAutosave.Text = saveFile.FileName;
        }

        private void btnApply_Click(object sender, EventArgs e)
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
            if (tmp.Length == 0)
                Properties.Settings.Default.CustomWikis = "";
            else
                Properties.Settings.Default.CustomWikis = tmp.Substring(0, tmp.LastIndexOf('|'));

            if (Properties.Settings.Default.CustomWikis.Length > 0)
                save = true;

            if (Properties.Settings.Default.DontAskForTerminate != chkAlwaysConfirmExit.Checked)
            {
                Properties.Settings.Default.DontAskForTerminate = chkAlwaysConfirmExit.Checked;
                save = true;
            }
            if (Properties.Settings.Default.Privacy != PrivacyCheckBox.Checked)
            {
                Properties.Settings.Default.Privacy = !PrivacyCheckBox.Checked;
                save = true;
            }

            if (save) Properties.Settings.Default.Save();
        }
    }
}