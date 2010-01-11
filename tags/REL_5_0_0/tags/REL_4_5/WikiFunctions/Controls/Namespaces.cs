using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
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
            chkArticleTalk.Text = Variables.Namespaces[Namespace.Talk];
            chkUser.Text = Variables.Namespaces[Namespace.User];
            chkUserTalk.Text = Variables.Namespaces[Namespace.UserTalk];
            chkWikipedia.Text = Variables.Namespaces[Namespace.Project];
            chkWikipediaTalk.Text = Variables.Namespaces[Namespace.ProjectTalk];
            chkImage.Text = Variables.Namespaces[Namespace.File];
            chkImageTalk.Text = Variables.Namespaces[Namespace.FileTalk];
            chkMediaWiki.Text = Variables.Namespaces[Namespace.MediaWiki];
            chkMediaWikiTalk.Text = Variables.Namespaces[Namespace.MediaWikiTalk];
            chkTemplate.Text = Variables.Namespaces[Namespace.Template];
            chkTemplateTalk.Text = Variables.Namespaces[Namespace.TemplateTalk];
            chkHelp.Text = Variables.Namespaces[Namespace.Help];
            chkHelpTalk.Text = Variables.Namespaces[Namespace.HelpTalk];
            chkCategory.Text = Variables.Namespaces[Namespace.Category];
            chkCategoryTalk.Text = Variables.Namespaces[Namespace.CategoryTalk];

            if (Variables.Namespaces.ContainsKey(Namespace.FirstCustom))
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
            SetSelectedNamespaces(new List<int>(new [] {0}));
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

        private bool doublecol;
        [DefaultValue(false), Category("Layout")]
        [Browsable(true)]
        public bool DoubleColumnFlowLayouts
        {
            get { return doublecol; }
            set
            {
                if (doublecol == value)
                    return;

                doublecol = value;

                if (doublecol)
                {
                    flwContent.Size = new Size(flwContent.Size.Width * 2, flwContent.Size.Height);
                    flwTalk.Size = new Size(flwTalk.Size.Width * 2, flwTalk.Size.Height);

                    Size = new Size(MaximumSize.Width, MinimumSize.Height);
                }
                else
                {
                    flwContent.Size = new Size(flwContent.Size.Width / 2, flwContent.Size.Height);
                    flwTalk.Size = new Size(flwTalk.Size.Width / 2, flwTalk.Size.Height);

                    Size = new Size(MinimumSize.Height, MaximumSize.Width);
                }

                flwTalk.Location = new Point(flwContent.Location.X + flwContent.Size.Width + 6, flwTalk.Location.Y);
                chkTalk.Location = new Point(flwTalk.Location.X, chkTalk.Location.Y);
            }
        }

        [Browsable(false)]
        public override Size MaximumSize
        {
            get
            {
                return base.MaximumSize;
            }
            set
            {
                base.MaximumSize = value;
            }
        }

        [Browsable(false)]
        public override Size MinimumSize
        {
            get
            {
                return base.MinimumSize;
            }
            set
            {
                base.MinimumSize = value;
            }
        }
    }
}
