using System.Windows.Forms;

namespace AutoWikiBrowser.Plugins.Kingbotk.Plugins
{
	internal class BioWithWorkgroups : GenericWithWorkgroups
	{
        public BioWithWorkgroups(string template, string prefix, bool autoStubEnabled, params TemplateParameters[] @params)
            : base(template, prefix, autoStubEnabled, @params)
        {
            ListView1.ItemChecked += ListView1_ItemChecked;
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
