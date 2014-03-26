using AutoWikiBrowser.Plugins.Kingbotk;
using AutoWikiBrowser.Plugins.Kingbotk.Components;
using AutoWikiBrowser.Plugins.Kingbotk.ManualAssessments;
using AutoWikiBrowser.Plugins.Kingbotk.Plugins;
using My;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using WikiFunctions;
using WikiFunctions.Logging.Uploader;
using WikiFunctions.Plugin;
//Copyright © 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/
//Copyright © 2008 Sam Reed (Reedy) http://www.reedyboy.net/

//This program is free software; you can redistribute it and/or modify it under the terms of Version 2 of the GNU General Public License as published by the Free Software Foundation.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

//You should have received a copy of the GNU General Public License Version 2 along with this program; if not, write to the Free Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

namespace AutoWikiBrowser.Plugins.Kingbotk.Plugins
{
	internal partial class GenericWithWorkgroups : IGenericSettings
	{

		public GenericWithWorkgroups(string template, string prefix, bool autoStubEnabled, params TemplateParameters[] @params)
		{
			// This call is required by the designer.
			InitializeComponent();

			this.Template = template;
			this.Prefix = prefix;

			AutoStubCheckBox.Enabled = autoStubEnabled;

			LinkLabel1.Text = "{{" + template + "}}";

			Dictionary<string, ListViewGroup> groupsAndMenus = new Dictionary<string, ListViewGroup>();

			ListView1.BeginUpdate();

			foreach (TemplateParameters prop in @params) {
				ListViewItem lvi = new ListViewItem(prop.ParamName);
				lvi.Name = prop.StorageKey;
				lvi.Tag = prop;

				if (!string.IsNullOrEmpty(prop.Group)) {
					string @group = prop.Group.Replace(" ", "");

					if (!groupsAndMenus.ContainsKey(@group)) {
						ListViewGroup lvGroup = new ListViewGroup { Header = prop.Group };
						ListView1.Groups.Add(lvGroup);

						groupsAndMenus.Add(@group, lvGroup);
						//Cache group and menu item

						lvi.Group = lvGroup;
					} else {
						lvi.Group = groupsAndMenus[@group];
					}
				}

				ListView1.Items.Add(lvi);
			}
			ListView1.EndUpdate();
		}

		protected string Prefix;

		protected string Template;
		private const string conStubClassParm = "StubClass";

		private const string conAutoStubParm = "AutoStub";
		#region "XML interface"
		internal void ReadXML(XmlTextReader Reader)
		{
			foreach (ListViewItem lvi in ListView1.Items) {
				TemplateParameters tp = (TemplateParameters)lvi.Tag;
				lvi.Checked = PluginManager.XMLReadBoolean(Reader, Prefix + tp.StorageKey, lvi.Checked);
			}

			StubClass = PluginManager.XMLReadBoolean(Reader, Prefix + conStubClassParm, StubClass);

			if (AutoStubCheckBox.Enabled) {
				PluginManager.XMLReadBoolean(Reader, Prefix + conAutoStubParm, AutoStub);
			}
		}

		internal void WriteXML(XmlTextWriter Writer)
		{
			var _with1 = Writer;
			foreach (ListViewItem lvi in ListView1.Items) {
				TemplateParameters tp = (TemplateParameters)lvi.Tag;
				_with1.WriteAttributeString(Prefix + tp.StorageKey, lvi.Checked.ToString());
			}

			_with1.WriteAttributeString(Prefix + conStubClassParm, StubClass.ToString());

			if (AutoStubCheckBox.Enabled) {
				_with1.WriteAttributeString(Prefix + conAutoStubParm, AutoStub.ToString());
			}
		}

		internal void Reset()
		{
			StubClass = false;
			AutoStub = false;

			foreach (ListViewItem lvi in ListView1.Items) {
				lvi.Checked = false;
			}
		}
		void IGenericSettings.XMLReset()
		{
			Reset();
		}
		#endregion
		// Properties:
		internal bool StubClass {
			get { return StubClassCheckBox.Checked; }
			set { StubClassCheckBox.Checked = value; }
		}

		public bool StubClassModeAllowed {
			set { StubClassCheckBox.Enabled = value; }
		}

		internal bool AutoStub {
			get { return AutoStubCheckBox.Enabled && AutoStubCheckBox.Checked; }
			set { AutoStubCheckBox.Checked = value; }
		}
		// Event handlers:
		private void LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Tools.OpenENArticleInBrowser(Variables.Namespaces[WikiFunctions.Namespace.Template] + Template, false);
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
