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
    public partial class MyPreferences : Form
    {
        public MyPreferences(LangCodeEnum lang, ProjectEnum proj, string customproj, bool EDiff, bool SDown, int DiffSize, Font TextFont, bool LowPriority, bool Flash, bool Beep, bool Minimize)
        {
            InitializeComponent();

            foreach (LangCodeEnum l in Enum.GetValues(typeof(LangCodeEnum)))
                cmboLang.Items.Add(l.ToString().ToLower());

            foreach (ProjectEnum l in Enum.GetValues(typeof(ProjectEnum)))
                cmboProject.Items.Add(l);

            cmboLang.SelectedItem = lang.ToString().ToLower();
            cmboProject.SelectedItem = proj;

            txtCustomProject.Text = customproj;

            EnhanceDiff = EDiff;
            ScrollDown = SDown;
            DiffFontSize = DiffSize;
            TextBoxFont = TextFont;
            LowThreadPriority = LowPriority;
            //FlashAndBeep = FlashBeep;
            perfFlash = Flash;
            perfBeep = Beep;
            perfMinimize = Minimize;

            cmboProject_SelectedIndexChanged(null, null);
        }

        #region Language and project

        public LangCodeEnum Language
        {
            get
            {
                if (cmboLang.SelectedItem.ToString() == "is") return LangCodeEnum.Is;
                LangCodeEnum l = (LangCodeEnum)Enum.Parse(typeof(LangCodeEnum), cmboLang.SelectedItem.ToString());
                return l;
            }
        }
        public ProjectEnum Project
        {
            get
            {
                ProjectEnum p = (ProjectEnum)Enum.Parse(typeof(ProjectEnum), cmboProject.SelectedItem.ToString());
                return p;
            }
        }
        public string CustomProject
        {
            get
            {
                return txtCustomProject.Text;
            }
        }

        private void txtCustomProject_Leave(object sender, EventArgs e)
        {
            txtCustomProject.Text = Regex.Replace(txtCustomProject.Text, "^http://", "", RegexOptions.IgnoreCase);
            txtCustomProject.Text = txtCustomProject.Text.TrimEnd('/');
            txtCustomProject.Text = txtCustomProject.Text + "/";
        }

        private void cmboProject_SelectedIndexChanged(object sender, EventArgs e)
        {
            //disable language selection for single language projects
            cmboLang.Enabled = cmboProject.SelectedIndex <= 6;

            if ((ProjectEnum)Enum.Parse(typeof(ProjectEnum), cmboProject.SelectedItem.ToString()) == ProjectEnum.custom)
            {
                txtCustomProject.Visible = true;
                cmboLang.Visible = false;
                lblLang.Text = "http://";
                edtCustomProject_TextChanged(null, null);
            }
            else
            {
                txtCustomProject.Visible = false;
                cmboLang.Visible = true;
                lblLang.Text = "Language:";
                btnApply.Enabled = true;
            }
        }

        private void edtCustomProject_TextChanged(object sender, EventArgs e)
        {
            btnApply.Enabled = txtCustomProject.Text != "";
        }

        #endregion

        #region Other

        public bool EnhanceDiff
        {
            get { return chkEnhanceDiff.Checked; }
            set { chkEnhanceDiff.Checked = value; }
        }

        public bool ScrollDown
        {
            get { return chkScrollDown.Checked; }
            set { chkScrollDown.Checked = value; }
        }

        public int DiffFontSize
        {
            get { return (int)nudDiffFontSize.Value; }
            set { nudDiffFontSize.Value = value; }
        }

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
            {
                TextBoxFont = fontDialog.Font;
            }
        }

        public bool LowThreadPriority
        {
            get { return chkLowPriority.Checked; }
            set { chkLowPriority.Checked = value; }
        }

        public bool FlashAndBeep
        {
            //get { return chkFlashAndBeep.Checked; }
            set { chkFlash.Checked = value; chkBeep.Checked = value; }
        }

        public bool perfFlash
        {
            get { return chkFlash.Checked; }
            set { chkFlash.Checked = value; }
        }

        public bool perfBeep
        {
            get { return chkBeep.Checked; }
            set { chkBeep.Checked = value; }
        }

        public bool perfMinimize
        {
            get { return chkMinimize.Checked; }
            set { chkMinimize.Checked = value; }
        }

        #endregion
    }
}