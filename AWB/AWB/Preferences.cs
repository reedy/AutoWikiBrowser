using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WikiFunctions;

namespace AutoWikiBrowser
{
    public partial class MyPreferences : Form
    {
        public MyPreferences(string lang, string proj, bool EDiff, bool SDown, int DiffSize, Font TextFont, bool LowPriority, bool FlashBeep)
        {
            InitializeComponent();

            foreach (LangCodeEnum l in Enum.GetValues(typeof(LangCodeEnum)))
                cmboLang.Items.Add(l.ToString());

            foreach (ProjectEnum l in Enum.GetValues(typeof(ProjectEnum)))
                cmboProject.Items.Add(l.ToString());

            cmboLang.SelectedItem = lang;
            cmboProject.SelectedItem = proj;

            EnhanceDiff = EDiff;
            ScrollDown = SDown;
            DiffFontSize = DiffSize;
            TextBoxFont = TextFont;
            LowThreadPriority = LowPriority;
            FlashAndBeep = FlashBeep;
        }

        #region Language and project

        public LangCodeEnum Language
        {
            get
            {
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

        private void cmboProject_SelectedIndexChanged(object sender, EventArgs e)
        {//disable other languages that are not wikipedia.
            if (cmboProject.SelectedIndex >= 6)
            {
                cmboLang.SelectedIndex = 0;
                cmboLang.Enabled = false;
            }
            else
                cmboLang.Enabled = true;
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
            get { return chkFlashAndBeep.Checked; }
            set { chkFlashAndBeep.Checked = value; }
        }

        #endregion

    }
}