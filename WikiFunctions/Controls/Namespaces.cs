using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace WikiFunctions.Controls
{
    public partial class Namespaces : UserControl
    {
        public Namespaces()
        {
            InitializeComponent();

            UpdateText();
        }

        private void chkContents_CheckedChanged(object sender, EventArgs e)
        {
            foreach (CheckBox chk in flwContent.Controls)
            {
                chk.Checked = chkContents.Checked;
            }
        }

        private void chkTalk_CheckedChanged(object sender, EventArgs e)
        {
            foreach (CheckBox chk in flwTalk.Controls)
            {
                chk.Checked = chkTalk.Checked;
            }
        }

        public void UpdateText()
        {
            chkArticleTalk.Text = Variables.Namespaces[1];
            chkUser.Text = Variables.Namespaces[2];
            chkUserTalk.Text = Variables.Namespaces[3];
            chkWikipedia.Text = Variables.Namespaces[4];
            chkWikipediaTalk.Text = Variables.Namespaces[5];
            chkImage.Text = Variables.Namespaces[6];
            chkImageTalk.Text = Variables.Namespaces[7];
            chkMediaWiki.Text = Variables.Namespaces[8];
            chkMediaWikiTalk.Text = Variables.Namespaces[9];
            chkTemplate.Text = Variables.Namespaces[10];
            chkTemplateTalk.Text = Variables.Namespaces[11];
            chkHelp.Text = Variables.Namespaces[12];
            chkHelpTalk.Text = Variables.Namespaces[13];
            chkCategory.Text = Variables.Namespaces[14];
            chkCategoryTalk.Text = Variables.Namespaces[15];

            if (Variables.Namespaces.ContainsKey(100))
            {
                chkPortal.Text = Variables.Namespaces[100];
                chkPortalTalk.Text = Variables.Namespaces[101];

                chkPortal.Visible = chkPortalTalk.Visible = true;
            }
            else
                chkPortal.Visible = chkPortalTalk.Visible = false;
        }

        CheckBox tmp;
        public void Reset()
        {
            SetSelectedNamespaces(new List<int>(new int[] {0}));
        }

        public List<int> GetSelectedNamespaces()
        {
            List<int> ret = new List<int>();
            ret.AddRange(GetListTags(Controls));
            return ret;
        }

        private List<int> GetListTags(ControlCollection controls)
        {
            List<int> ret = new List<int>();
            foreach (Control cntrl in controls)
            {
                if (cntrl is FlowLayoutPanel)
                {
                    ret.AddRange(GetListTags(cntrl.Controls));
                    continue;
                }

                tmp = (cntrl as CheckBox);

                if (tmp != null && tmp.Checked && tmp.Tag != null)
                    ret.Add(int.Parse(tmp.Tag.ToString()));
            }

            return ret;
        }

        public void SetSelectedNamespaces(List<int> tags)
        {
            SetListTags(Controls, tags);
        }

        private void SetListTags(ControlCollection controls, List<int> tags)
        {
            foreach (Control cntrl in controls)
            {
                if (cntrl is FlowLayoutPanel)
                {
                    SetListTags(cntrl.Controls, tags);
                    continue;
                }

                tmp = (cntrl as CheckBox);

                if (tmp != null && tmp.Tag != null)
                    tmp.Checked = tags.Contains(int.Parse(tmp.Tag.ToString()));
            }
        }
    }
}
