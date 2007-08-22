/*
Autowikibrowser
Copyright (C) 2007 Martin Richards

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
            Go.Enabled = Find.Text != "" && Source.Text != "";
        }

        private void Go_Click(object sender, EventArgs e)
        {
            ResultList.Items.Clear();
            ResultText.Text = "";
            Status.Text = "";
            Source.Text = Source.Text.Replace("\r\n", "\n");

            try
            {
                Regex r;
                if (Multiline.Checked && Casesensitive.Checked) r = new Regex(Find.Text, RegexOptions.Singleline);
                else if (!Multiline.Checked && Casesensitive.Checked) r = new Regex(Find.Text);
                else if (Multiline.Checked && !Casesensitive.Checked) r = new Regex(Find.Text, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                else if (!Multiline.Checked && !Casesensitive.Checked) r = new Regex(Find.Text, RegexOptions.IgnoreCase);
                else r = new Regex(Find.Text);

                ResultText.Text = r.Replace(Source.Text, Replace.Text.Replace("\\n", "\r\n"));
                if (r.Matches(Source.Text).Count != 1)
                    Status.Text = r.Matches(Source.Text).Count.ToString() + " replacements performed";
                else
                    Status.Text = "1 replacements performed";
                ResultList.Visible = false;
                ResultText.Visible = true;
            }
            catch (Exception ex)
            {
                Status.Text = "Error";
                ErrorHandler.Handle(ex);
            }

            Source.Text = Source.Text.Replace("\n", "\r\n");
            ResultText.Text = ResultText.Text.Replace("\r\n", "\n");
            ResultText.Text = ResultText.Text.Replace("\n", "\r\n");
        }

        private void RegexTester_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 27)
            {
                e.Handled = true;
                Close();
            }
        }
    }
}