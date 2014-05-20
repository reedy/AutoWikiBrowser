using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;
using WikiFunctions;

//Copyright © 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/
//Copyright © 2008 Sam Reed (Reedy) http://www.reedyboy.net/

//This program is free software; you can redistribute it and/or modify it under the terms of Version 2 of the GNU General Public License as published by the Free Software Foundation.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

//You should have received a copy of the GNU General Public License Version 2 along with this program; if not, write to the Free Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

namespace AutoWikiBrowser.Plugins.Kingbotk.Plugins
{
    partial class GenericWithWorkgroups : IGenericSettings
    {
        public GenericWithWorkgroups()
        {
            InitializeComponent();
        }

        public GenericWithWorkgroups(string template, string prefix, bool autoStubEnabled,
            params TemplateParameters[] @params)
            : this()
        {
            Template = template;
            Prefix = prefix;

            AutoStubCheckBox.Enabled = autoStubEnabled;

            LinkLabel1.Text = "{{" + template + "}}";

            Dictionary<string, ListViewGroup> groupsAndMenus = new Dictionary<string, ListViewGroup>();

            ListView1.BeginUpdate();

            foreach (TemplateParameters prop in @params)
            {
                ListViewItem lvi = new ListViewItem(prop.ParamName) {Name = prop.StorageKey, Tag = prop};

                if (!string.IsNullOrEmpty(prop.Group))
                {
                    string @group = prop.Group.Replace(" ", "");

                    if (!groupsAndMenus.ContainsKey(@group))
                    {
                        ListViewGroup lvGroup = new ListViewGroup {Header = prop.Group};
                        ListView1.Groups.Add(lvGroup);

                        groupsAndMenus.Add(@group, lvGroup);
                        //Cache group and menu item

                        lvi.Group = lvGroup;
                    }
                    else
                    {
                        lvi.Group = groupsAndMenus[@group];
                    }
                }

                ListView1.Items.Add(lvi);
            }
            ListView1.EndUpdate();
        }

        protected string Prefix;

        protected string Template;
        private const string StubClassParm = "StubClass";

        private const string AutoStubParm = "AutoStub";

        #region "XML interface"

        public void ReadXML(XmlTextReader reader)
        {
            foreach (ListViewItem lvi in ListView1.Items)
            {
                TemplateParameters tp = (TemplateParameters) lvi.Tag;
                lvi.Checked = PluginManager.XMLReadBoolean(reader, Prefix + tp.StorageKey, lvi.Checked);
            }

            StubClass = PluginManager.XMLReadBoolean(reader, Prefix + StubClassParm, StubClass);

            if (AutoStubCheckBox.Enabled)
            {
                PluginManager.XMLReadBoolean(reader, Prefix + AutoStubParm, AutoStub);
            }
        }

        public void WriteXML(XmlTextWriter writer)
        {
            foreach (ListViewItem lvi in ListView1.Items)
            {
                TemplateParameters tp = (TemplateParameters) lvi.Tag;
                writer.WriteAttributeString(Prefix + tp.StorageKey, lvi.Checked.ToString());
            }

            writer.WriteAttributeString(Prefix + StubClassParm, StubClass.ToString());

            if (AutoStubCheckBox.Enabled)
            {
                writer.WriteAttributeString(Prefix + AutoStubParm, AutoStub.ToString());
            }
        }

        internal void Reset()
        {
            StubClass = false;
            AutoStub = false;

            foreach (ListViewItem lvi in ListView1.Items)
            {
                lvi.Checked = false;
            }
        }

        void IGenericSettings.XMLReset()
        {
            Reset();
        }

        #endregion

        // Properties:
        public bool StubClass
        {
            get { return StubClassCheckBox.Checked; }
            set { StubClassCheckBox.Checked = value; }
        }

        public bool StubClassModeAllowed
        {
            set { StubClassCheckBox.Enabled = value; }
        }

        public bool AutoStub
        {
            get { return AutoStubCheckBox.Enabled && AutoStubCheckBox.Checked; }
            set { AutoStubCheckBox.Checked = value; }
        }

        // Event handlers:
        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Tools.OpenENArticleInBrowser(Variables.Namespaces[Namespace.Template] + Template, false);
        }

        private void AutoStubCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (AutoStubCheckBox.Checked)
                StubClassCheckBox.Checked = false;
        }

        private void StubClassCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (StubClassCheckBox.Checked)
                AutoStubCheckBox.Checked = false;
        }
    }
}