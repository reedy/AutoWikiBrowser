using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace AutoWikiBrowser
{
    internal sealed partial class RegexTester : Form
    {
        public RegexTester()
        {
            InitializeComponent();
        }

        private void ConditionsChanged(object sender, EventArgs e)
        {
            Go.Enabled = Find.Text.Trim() != "" && Source.Text.Trim() != "";
        }

        private void Go_Click(object sender, EventArgs e)
        {
            ResultList.Items.Clear();
            ResultText.Text = "";
            Status.Text = "";

            try
            {
                Regex r;
                if (Multiline.Checked && Casesensitive.Checked) r = new Regex(Find.Text, RegexOptions.Singleline);
                else if (!Multiline.Checked && Casesensitive.Checked) r = new Regex(Find.Text);
                else if (Multiline.Checked && !Casesensitive.Checked) r = new Regex(Find.Text, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                else if (!Multiline.Checked && !Casesensitive.Checked) r = new Regex(Find.Text, RegexOptions.IgnoreCase);
                else r = new Regex(Find.Text);

                ResultText.Text = r.Replace(Source.Text, Replace.Text);
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
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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