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

namespace AutoWikiBrowser
{
    internal sealed partial class RegexTester : Form
    {
        private Regex NewLineRegex = new Regex("\n");

        public RegexTester()
        {
            InitializeComponent();
        }

        public string ArticleText
        {
            set { Source.Text = value; }
        }

        private void ConditionsChanged(object sender, EventArgs e)
        {
            bool enabled = (!string.IsNullOrEmpty(Find.Text) && !string.IsNullOrEmpty(Source.Text));
            ReplaceBtn.Enabled = (!string.IsNullOrEmpty(Replace.Text) && enabled);
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
                // this was a bit confusing (Multiline box turns on Singleline!), much better to have boxes which correspond directly to the dotnet RegexOptions see e.g. http://www.regexlib.com/RETester.aspx
                RegexOptions res = RegexOptions.None;
                if (Multiline.Checked) res |= RegexOptions.Multiline;
                if (Singleline.Checked) res |= RegexOptions.Singleline; 
                if (Ignorecase.Checked) res |= RegexOptions.IgnoreCase;
                if (explicitcapture.Checked) res |= RegexOptions.ExplicitCapture;

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
                Regex r = new Regex(Find.Text, Options);

                ResultText.Text = r.Replace(Source.Text, Replace.Text.Replace("\\n", "\r\n"));
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

                Regex r = new Regex(Find.Text, Options);
                MatchCollection matches = r.Matches(Source.Text.Replace("\r\n", "\n"));
                foreach (Match m in matches)
                //for(int i = 0; i < matches.Count; i++)
                {
                    //Match m = matches[i];
                    TreeNode n = Captures.Nodes.Add(ReplaceNewLines(m.Value));
                    foreach (Group g in m.Groups)
                    { // TODO: Is there any way to get the name of the group when explicit capture is on?
                        if (g.Captures.Count > 1)
                        {
                            TreeNode nn = n.Nodes.Add("...");
                            foreach (Capture c in g.Captures)
                                nn.Nodes.Add(ReplaceNewLines(c.Value));
                        }
                        else if (g.Captures.Count == 1)
                            n.Nodes.Add(ReplaceNewLines(g.Captures[0].Value));
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

        private void ErrorHandler(Exception ex)
        {
            MessageBox.Show(ex.Message, "Whoops!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}