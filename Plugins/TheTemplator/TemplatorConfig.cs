using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace AutoWikiBrowser.Plugins.TheTemplator
{
	public partial class TemplatorConfig : Form
	{
		public TemplatorConfig()
		{
			InitializeComponent();
			SetButtonStates();
		}

		public TemplatorConfig(string name, Dictionary<string, string> parameters, string replacement, bool skip)
			: this()
		{
			// local vars
			TemplateName = templateName.Text = name;
			Parameters = parameters;
			Replacement = replacementText.Text = replacement;
			SkipIfNone = skipIfNone.Checked = skip;

			// Populate the listview
			foreach (KeyValuePair<string, string> kvp in parameters)
				templateParameters.Items.Add(new ListViewItem(new [] { kvp.Key, kvp.Value }));
		}

		private void SetButtonStates()
		{
			editParam.Enabled = templateParameters.SelectedItems.Count == 1;
			delParam.Enabled = templateParameters.SelectedItems.Count > 0;
		}

		private void OnAddParam(object sender, EventArgs e)
		{
			TemplatorParameterDetails dlg = new TemplatorParameterDetails();
			if (dlg.ShowDialog() == DialogResult.OK)
				templateParameters.Items.Add(new ListViewItem(new [] { dlg.ParamName, dlg.ParamRegex }));
		}

		private void OnEditParam(object sender, EventArgs e)
		{
			System.Diagnostics.Debug.Assert(templateParameters.SelectedItems.Count == 1);
			ListViewItem item = templateParameters.SelectedItems[0];
			TemplatorParameterDetails dlg = new TemplatorParameterDetails(item.SubItems[0].Text, item.SubItems[1].Text);
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				item.SubItems[0].Text = dlg.ParamName;
				item.SubItems[1].Text = dlg.ParamRegex;
			}
		}

		private void OnDelParam(object sender, EventArgs e)
		{
			foreach (ListViewItem item in templateParameters.SelectedItems)
				templateParameters.Items.Remove(item);
		}

		private void OnSelectedIndexChangedTemplateParameters(object sender, EventArgs e)
		{
			SetButtonStates();
		}

		private void OnDoubleClickTemplateParameters(object sender, EventArgs e)
		{
			if (templateParameters.SelectedItems.Count == 1)
				OnEditParam(sender, e);
		}

		public string TemplateName { get; private set; }
		public Dictionary<string, string> Parameters { get; private set; }
		public string Replacement { get; private set; }
		public bool SkipIfNone { get; private set; }

		private void OK_Click(object sender, EventArgs e)
		{
			if (templateName.Text == "")
			{
				MessageBox.Show(string.Format("Template name must be specified"), PluginName);
				DialogResult = DialogResult.None;
				templateName.Focus();
				return;
			}
			TemplateName = templateName.Text;
			Replacement = replacementText.Text;
			Parameters.Clear();
			List<string> names = new List<string>();
			foreach (ListViewItem lvi in templateParameters.Items)
			{
				// some sanity checking on the parameter names
				string param = lvi.SubItems[0].Text;
				string name = new Regex("[^a-z0-9_]").Replace(param, "");
				if (name == "" || new Regex("[0-9]").IsMatch(name.Substring(0, 1)))
				{
					MessageBox.Show(string.Format("{0} is not a valid parameter name", param), PluginName);
					templateParameters.Focus();
					templateParameters.SelectedItems.Clear();
					lvi.Selected = true;
					DialogResult = DialogResult.None;
					return;
				}
				if (names.Contains(name))
				{
					MessageBox.Show(string.Format("{0}: duplicated parameter", name), PluginName);
					templateParameters.Focus();
					templateParameters.SelectedItems.Clear();
					lvi.Selected = true;
					DialogResult = DialogResult.None;
					return;
				}
				names.Add(name);
				Parameters.Add(lvi.SubItems[0].Text, lvi.SubItems[1].Text);
			}
			SkipIfNone = skipIfNone.Checked;
		}
		static internal string PluginName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

		private void regexHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			regexHelp.LinkVisited = true;
			WikiFunctions.Tools.OpenURLInBrowser("http://msdn.microsoft.com/en-us/library/hs600312.aspx");
		}
	}
}
