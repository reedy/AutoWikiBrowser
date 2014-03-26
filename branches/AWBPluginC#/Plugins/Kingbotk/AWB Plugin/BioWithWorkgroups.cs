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

using WikiFunctions.Plugin;
namespace AutoWikiBrowser.Plugins.Kingbotk.Plugins
{
	internal class BioWithWorkgroups : GenericWithWorkgroups
	{

		public BioWithWorkgroups(string template, string prefix, bool autoStubEnabled, params TemplateParameters[] @params) : base(template, prefix, autoStubEnabled, @params)
		{
		}


		private void ListView1_ItemChecked(object sender, ItemCheckedEventArgs e)
		{
			//HACK:For some reason, during the setup phases, the items are there, but aren't assigned. And are null
			if (ListView1.Items[ListView1.Items.Count - 1] != null) {
				if (e.Item.Text == "Living" && e.Item.Checked) {
					ListViewItem lvi = ListView1.FindItemWithText("Not Living");

					if (lvi != null && lvi.Checked) {
						lvi.Checked = false;
					}
				} else if (e.Item.Text == "Not Living" && e.Item.Checked) {
					ListViewItem lvi = ListView1.FindItemWithText("Living");

					if (lvi != null && lvi.Checked) {
						lvi.Checked = false;
					}
				}
			}
		}
	}
}
