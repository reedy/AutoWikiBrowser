using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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
        public TemplatorConfig(string name,
                               Dictionary<string, string> parameters,
                               Dictionary<string, string> replacements,
                               bool skip,
                               bool removeExcessPipes)
			: this()
		{
			// local vars
			TemplateName = templateName.Text = name;
			Parameters = parameters;
			Replacements = replacements;
			SkipIfNone = skipIfNone.Checked = skip;
            RemoveExcessPipes = removeExcessPipes;

			// Populate the listviews
            foreach (KeyValuePair<string, string> kvp in parameters)
                templateParameters.Items.Add(new ListViewItem(new [] { kvp.Key, kvp.Value }));
            foreach (KeyValuePair<string, string> kvp in replacements)
                replacementParameters.Items.Add(new ListViewItem(new [] { kvp.Key, kvp.Value }));
        }

		private void SetButtonStates()
		{
            editParam.Enabled = templateParameters.SelectedItems.Count == 1;
            delParam.Enabled = templateParameters.SelectedItems.Count > 0;
            editReplacement.Enabled = replacementParameters.SelectedItems.Count == 1;
            delReplacement.Enabled = replacementParameters.SelectedItems.Count > 0;
        }

		private void OnNewParam(object sender, EventArgs e)
		{
			TemplatorParameterDetails dlg = new TemplatorParameterDetails();
			if (dlg.ShowDialog() == DialogResult.OK)
				templateParameters.Items.Add(new ListViewItem(new string[] { dlg.ParamName, dlg.ParamRegex }));
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
        private void OnNewReplacementParam(object sender, EventArgs e)
        {
            TemplatorParameterDetails dlg = new TemplatorParameterDetails();
            if (dlg.ShowDialog() == DialogResult.OK)
                replacementParameters.Items.Add(new ListViewItem(new [] { dlg.ParamName, dlg.ParamRegex }));
        }
        private void OnEditReplacementParam(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(replacementParameters.SelectedItems.Count == 1);
            ListViewItem item = replacementParameters.SelectedItems[0];
            TemplatorParameterDetails dlg = new TemplatorParameterDetails(item.SubItems[0].Text, item.SubItems[1].Text);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                item.SubItems[0].Text = dlg.ParamName;
                item.SubItems[1].Text = dlg.ParamRegex;
            }
        }
        private void OnDelReplacementParam(object sender, EventArgs e)
        {
            foreach (ListViewItem item in replacementParameters.SelectedItems)
                replacementParameters.Items.Remove(item);
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
        private void OnDoubleClickReplacementParameters(object sender, EventArgs e)
        {
            if (replacementParameters.SelectedItems.Count == 1)
                OnEditReplacementParam(sender, e);
        }

		public string TemplateName { get; private set; }
        public Dictionary<string, string> Parameters { get; private set; }
        public Dictionary<string, string> Replacements { get; private set; }
		public string Replacement { get; private set; }
        public bool SkipIfNone { get; private set; }
        public bool RemoveExcessPipes { get; private set; }

		private void OK_Click(object sender, EventArgs e)
		{
            if (templateName.Text == "")
            {
                MessageBox.Show(string.Format("Template name must be specified"), PluginName);
                DialogResult = DialogResult.None;
                templateName.Focus();
                return;
            }
            if (templateParameters.Items.Count == 0)
            {
                MessageBox.Show(string.Format("At least one template parameter must be specified"), PluginName);
                DialogResult = DialogResult.None;
                return;
            }
            TemplateName = templateName.Text;
            SaveParameters(templateParameters, Parameters);
            SaveParameters(replacementParameters, Replacements);
            SkipIfNone = skipIfNone.Checked;
            RemoveExcessPipes = removeExcessPipes.Checked;
		}

        private bool SaveParameters(ListView parameterListView, IDictionary<string, string> parameterList)
        {
            parameterList.Clear();
            List<string> names = new List<string>();
            foreach (ListViewItem lvi in parameterListView.Items)
            {
                // some sanity checking on the parameter names
                string param = lvi.SubItems[0].Text;
                string name = new Regex("[^a-z0-9_]").Replace(param, "");
                if (name == "" || new Regex("[0-9]").IsMatch(name.Substring(0, 1)))
                {
                    MessageBox.Show(string.Format("{0} is not a valid parameter name", param), TemplateName);
                    parameterListView.Focus();
                    parameterListView.SelectedItems.Clear();
                    lvi.Selected = true;
                    DialogResult = DialogResult.None;
                    return false;
                }
                if (names.Contains(name))
                {
                    MessageBox.Show(string.Format("{0}: duplicated parameter", name), TemplateName);
                    parameterListView.Focus();
                    parameterListView.SelectedItems.Clear();
                    lvi.Selected = true;
                    DialogResult = DialogResult.None;
                    return false;
                }
                names.Add(name);
                parameterList.Add(lvi.SubItems[0].Text, lvi.SubItems[1].Text);
            }
            return true;
        }

		static internal string PluginName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

		private void regexHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			regexHelp.LinkVisited = true;
			WikiFunctions.Tools.OpenURLInBrowser("http://msdn.microsoft.com/en-us/library/hs600312.aspx");
		}

        bool InTemplateParameters_ColumnWidthChanging;
        private void templateParameters_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            if (InTemplateParameters_ColumnWidthChanging)
                return;
            InTemplateParameters_ColumnWidthChanging = true;
            replacementParameters.Columns[e.ColumnIndex].Width = e.NewWidth;
            InTemplateParameters_ColumnWidthChanging = false;
        }

        bool InReplacementParameters_ColumnWidthChanging;
        private void replacementParameters_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            if (InReplacementParameters_ColumnWidthChanging)
                return;
            InReplacementParameters_ColumnWidthChanging = true;
            templateParameters.Columns[e.ColumnIndex].Width = e.NewWidth;
            InReplacementParameters_ColumnWidthChanging = false;
        }
    }
}
