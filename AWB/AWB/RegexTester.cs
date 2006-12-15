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
    public partial class RegexTester : Form
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
                if(Multiline.Checked) r = new Regex(Find.Text, RegexOptions.Singleline);
                else r = new Regex(Find.Text);

                if (Replace.Text.Trim() == "")
                {
                    int n = 0;
                    foreach (Match m in r.Matches(Source.Text))
                    {
                        ResultList.Items.Add(m.Value);
                        n++;
                    }
                    Status.Text = n.ToString() + " matches found";
                    ResultList.Visible = true;
                    ResultText.Visible = false;
                }
                else
                {
                    ResultText.Text = r.Replace(Source.Text, Replace.Text);
                    Status.Text = "Replacement(s) performed";
                    ResultList.Visible = false;
                    ResultText.Visible = true;
                }
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