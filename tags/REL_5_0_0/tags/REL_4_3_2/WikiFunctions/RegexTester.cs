/*
Autowikibrowser
Copyright (C) 2008 Max Semenik, Stephen Kennedy

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
using System.Windows.Forms;
using System.Text.RegularExpressions;
using WikiFunctions;

namespace WikiFunctions.Controls
{
    public sealed partial class RegexTester : Form
    {
        private Regex NewLineRegex = new Regex("\n");

        public RegexTester()
        {
            InitializeComponent();
        }

        public RegexTester(bool ask)
            :this()
        {
            AskToApply = ask;
        }

        public static void Test(Form parent, TextBox find, TextBox replace, 
            CheckBox multiline, CheckBox singleline, CheckBox caseSensitive)
        {
            using (RegexTester t = new RegexTester(true))
            {
                t.Find = find.Text;
                if (replace != null) t.Replace = replace.Text;
                t.Multiline = multiline.Checked;
                t.Singleline = singleline.Checked;
                t.IgnoreCase = !caseSensitive.Checked;

                if (Variables.MainForm != null && Variables.MainForm.EditBox.Enabled)
                    t.ArticleText = Variables.MainForm.EditBox.Text;

                if (t.ShowDialog(parent) == DialogResult.OK)
                {
                    find.Text = t.Find;
                    if (replace != null) replace.Text = t.Replace;
                    multiline.Checked = t.Multiline;
                    singleline.Checked = t.Singleline;
                    caseSensitive.Checked = !t.IgnoreCase;
                }
            }
        }


        #region Properties

        public string ArticleText
        {
            set { Source.Text = value; }
        }

        public string Find
        {
            get { return txtFind.Text; }
            set { txtFind.Text = value; }
        }

        public string Replace
        {
            get { return txtReplace.Text; }
            set { txtReplace.Text = value; }
        }

        public RegexOptions RegexOptions
        {
            get
            {
                RegexOptions res = RegexOptions.None;
                if (chkMultiline.Checked) res |= RegexOptions.Multiline;
                if (chkSingleline.Checked) res |= RegexOptions.Singleline;
                if (chkIgnoreCase.Checked) res |= RegexOptions.IgnoreCase;
                if (chkExplicitCapture.Checked) res |= RegexOptions.ExplicitCapture;

                return res;
            }

            set
            {
                chkMultiline.Checked = (value & RegexOptions.Multiline) != 0;
                chkSingleline.Checked = (value & RegexOptions.Singleline) != 0;
                chkIgnoreCase.Checked = (value & RegexOptions.IgnoreCase) != 0;
                chkExplicitCapture.Checked = (value & RegexOptions.ExplicitCapture) != 0;
            }
        }

        public bool Multiline
        {
            get { return chkMultiline.Checked; }
            set { chkMultiline.Checked = value; }
        }

        public bool Singleline
        {
            get { return chkSingleline.Checked; }
            set { chkSingleline.Checked = value; }
        }

        public bool IgnoreCase
        {
            get { return chkIgnoreCase.Checked; }
            set { chkIgnoreCase.Checked = value; }
        }

        public bool ExplicitCapture
        {
            get { return chkExplicitCapture.Checked; }
            set { chkExplicitCapture.Checked = value; }
        }

        public bool AskToApply = false;
        #endregion

        private void ConditionsChanged(object sender, EventArgs e)
        {
            bool enabled = (!string.IsNullOrEmpty(txtFind.Text) && !string.IsNullOrEmpty(Source.Text));
            ReplaceBtn.Enabled = (!string.IsNullOrEmpty(txtReplace.Text) && enabled);
            FindBtn.Enabled = enabled;
        }
        
        private void KeyPressHandler(object sender, KeyPressEventArgs e)
        { // all this to to "select all" <rolls eyes>
            if (e.KeyChar == (char)1) // 1 = CTRL+A
                ((TextBox)sender).SelectAll();
        }

        private RegexOptions Options
        {
            get
            {
                RegexOptions res = RegexOptions.None;
                if (chkMultiline.Checked) res |= RegexOptions.Multiline;
                if (chkSingleline.Checked) res |= RegexOptions.Singleline; 
                if (chkIgnoreCase.Checked) res |= RegexOptions.IgnoreCase;
                if (chkExplicitCapture.Checked) res |= RegexOptions.ExplicitCapture;

                return res;
            }
        }

        private void Replace_Click(object sender, EventArgs e)
        {
            Captures.Nodes.Clear();
            ResultText.Text = "";
            Status.Text = "";
            Source.Text = Source.Text.Replace("\r\n", "\n");

            try
            {
                Regex r = new Regex(txtFind.Text, Options);

                ResultText.Text = r.Replace(Source.Text, txtReplace.Text.Replace("\\n", "\r\n"));
                if (r.Matches(Source.Text).Count != 1)
                    Status.Text = r.Matches(Source.Text).Count.ToString() + " replacements performed";
                else
                    Status.Text = "1 replacement performed";
                Captures.Visible = false;
                ResultText.Visible = true;
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }

            Source.Text = Source.Text.Replace("\n", "\r\n");
            ResultText.Text = ResultText.Text.Replace("\r\n", "\n");
            ResultText.Text = ResultText.Text.Replace("\n", "\r\n");
        }

        private void RegexTester_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 27) // Escape key
            {
                e.Handled = true;
                Close();
            }
        }

        private void FindBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Captures.Nodes.Clear();
                Captures.Visible = true;
                ResultText.Text = "";
                ResultText.Visible = false;
                Status.Text = "";

                Regex r = new Regex(txtFind.Text, Options);
                MatchCollection matches = r.Matches(Source.Text.Replace("\r\n", "\n"));
                foreach (Match m in matches)
                {
                    TreeNode n = Captures.Nodes.Add("{" + ReplaceNewLines(m.Value) + "}");
                    foreach (Group g in m.Groups)
                    { // TODO: Is there any way to get the name of the group when explicit capture is on?
                        if (g.Captures.Count > 1)
                        {
                            TreeNode nn = n.Nodes.Add("...");
                            foreach (Capture c in g.Captures)
                                nn.Nodes.Add("{" + ReplaceNewLines(c.Value) + "}");
                        }
                        else if (g.Captures.Count == 1)
                            n.Nodes.Add(ReplaceNewLines("{" + g.Captures[0].Value) + "}");
                    }
                }
                if (matches.Count == 0)
                    Status.Text = "No matches";
                else if (matches.Count == 1)
                    Status.Text = "1 match found";
                else
                    Status.Text = matches.Count.ToString() + " matches found";


                Captures.ExpandAll();
            }
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }
        }

        private string ReplaceNewLines(string str)
        { // Display line breaks as \n in the results tree so that they're clear
            return NewLineRegex.Replace(str, "\\n");
        }

        private static void ErrorHandler(Exception ex)
        {
            MessageBox.Show(ex.Message, "Whoops!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void RegexTester_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            RegexTester_HelpRequested(sender, null);
        }

        private void RegexTester_HelpRequested(object sender, object hlpevent)
        {
            Tools.OpenURLInBrowser("http://msdn2.microsoft.com/en-us/library/az24scfc.aspx");
        }

        private void RegexTester_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(e.CloseReason != CloseReason.UserClosing || !AskToApply) return;

            switch (MessageBox.Show(this, "Do you want to apply your changes?", Text, MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question))
            {
                case DialogResult.Yes:
                    DialogResult = DialogResult.OK;
                    break;
                case DialogResult.No:
                    DialogResult = DialogResult.Cancel;
                    break;
                case DialogResult.Cancel:
                    e.Cancel = true;
                    break;
            }
        }
    }
}