using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AutoWikiBrowser
{
    public partial class MyPreferences : Form
    {
        public MyPreferences(bool EDiff, bool SDown, int DiffSize, Font TextFont)
        {
            InitializeComponent();

            EnhanceDiff = EDiff;
            ScrollDown = SDown;
            DiffFontSize = DiffSize;
            TextBoxFont = TextFont;
        }

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
            get{ return (int)nudDiffFontSize.Value;}
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
    }
}