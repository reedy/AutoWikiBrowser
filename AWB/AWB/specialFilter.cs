using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Text.RegularExpressions;
using WikiFunctions;

namespace AutoWikiBrowser
{
    public partial class specialFilter : Form
    {
        ListBox lb;
        Label lbl;
        public specialFilter(ListBox listbox, Label label)
        {
            InitializeComponent();
            lb = listbox;
            lbl = label;
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            bool does = (chkContains.Checked && txtContains.Text != "");
            bool doesnot = (chkNotContains.Checked && txtDoesNotContain.Text != "");

            if (does || doesnot)
                FilterMatches(does, doesnot);

            FilterNamespace();

            lbl.Text = lb.Items.Count.ToString();
        }

        private void FilterNamespace()
        {
            int j = 0;
            int i = 0;

            while (i < lb.Items.Count)
            {
                j = Tools.CalculateNS(lb.Items[i].ToString());

                if (j == 0)
                {
                    if (chkArticle.Checked)
                    {
                        i++;
                        continue;
                    }
                    else
                        lb.Items.RemoveAt(i);
                }
                else if (j == 1)
                {
                    if (chkArticleTalk.Checked)
                    {
                        i++;
                        continue;
                    }
                    else
                        lb.Items.RemoveAt(i);
                }
                else if (j == 2)
                {
                    if (chkUser.Checked)
                    {
                        i++;
                        continue;
                    }
                    else
                        lb.Items.RemoveAt(i);
                }
                else if (j == 3)
                {
                    if (chkUserTalk.Checked)
                    {
                        i++;
                        continue;
                    }
                    else
                        lb.Items.RemoveAt(i);
                }
                else if (j == 4)
                {
                    if (chkWikipedia.Checked)
                    {
                        i++;
                        continue;
                    }
                    else
                        lb.Items.RemoveAt(i);
                }
                else if (j == 5)
                {
                    if (chkWikipediaTalk.Checked)
                    {
                        i++;
                        continue;
                    }
                    else
                        lb.Items.RemoveAt(i);
                }
                else if (j == 6)
                {
                    if (chkImage.Checked)
                    {
                        i++;
                        continue;
                    }
                    else
                        lb.Items.RemoveAt(i);
                }
                else if (j == 7)
                {
                    if (chkImageTalk.Checked)
                    {
                        i++;
                        continue;
                    }
                    else
                        lb.Items.RemoveAt(i);
                }
                else if (j == 8)
                {
                    if (chkMediaWiki.Checked)
                    {
                        i++;
                        continue;
                    }
                    else
                        lb.Items.RemoveAt(i);
                }
                else if (j == 9)
                {
                    if (chkMediaWikiTalk.Checked)
                    {
                        i++;
                        continue;
                    }
                    else
                        lb.Items.RemoveAt(i);
                }
                else if (j == 10)
                {
                    if (chkTemplate.Checked)
                    {
                        i++;
                        continue;
                    }
                    else
                        lb.Items.RemoveAt(i);
                }
                else if (j == 11)
                {
                    if (chkTemplateTalk.Checked)
                    {
                        i++;
                        continue;
                    }
                    else
                        lb.Items.RemoveAt(i);
                }
                else if (j == 12)
                {
                    if (chkHelp.Checked)
                    {
                        i++;
                        continue;
                    }
                    else
                        lb.Items.RemoveAt(i);
                }
                else if (j == 13)
                {
                    if (chkHelpTalk.Checked)
                    {
                        i++;
                        continue;
                    }
                    else
                        lb.Items.RemoveAt(i);
                }
                else if (j == 14)
                {
                    if (chkCategory.Checked)
                    {
                        i++;
                        continue;
                    }
                    else
                        lb.Items.RemoveAt(i);
                }
                else if (j == 15)
                {
                    if (chkCategoryTalk.Checked)
                    {
                        i++;
                        continue;
                    }
                    else
                        lb.Items.RemoveAt(i);
                }
                else if (j == 100)
                {
                    if (chkPortal.Checked)
                    {
                        i++;
                        continue;
                    }
                    else
                        lb.Items.RemoveAt(i);
                }
                else if (j == 101)
                {
                    if (chkPortalTalk.Checked)
                    {
                        i++;
                        continue;
                    }
                    else
                        lb.Items.RemoveAt(i);
                }
                else
                    i++;
            }
        }

        private void FilterMatches(bool does, bool doesnot)
        {
            string strMatch = txtContains.Text;
            string strNotMatch = txtDoesNotContain.Text;

            if (!chkIsRegex.Checked)
            {
                strMatch = Regex.Escape(strMatch);
                strNotMatch = Regex.Escape(strNotMatch);
            }

            Regex match = new Regex(strMatch);
            Regex notMatch = new Regex(strNotMatch);

            string s = "";
            int i = 0;

            while (i < lb.Items.Count)
            {
                s = lb.Items[i].ToString();
                if (does && match.IsMatch(s))
                    lb.Items.RemoveAt(i);
                else if (doesnot && !notMatch.IsMatch(s))
                    lb.Items.RemoveAt(i);
                else
                    i++;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void chkContains_CheckedChanged(object sender, EventArgs e)
        {
            txtContains.Enabled = chkContains.Checked;
            chkIsRegex.Enabled = chkContains.Checked || chkNotContains.Checked;
        }

        private void chkNotContains_CheckedChanged(object sender, EventArgs e)
        {
            txtDoesNotContain.Enabled = chkNotContains.Checked;
            chkIsRegex.Enabled = chkContains.Checked || chkNotContains.Checked;
        }
    }
}