using System.Collections.Generic;
using System.Windows.Forms;

namespace AWBUpdater
{
    public partial class VersionChooser : Form
    {
        public VersionChooser(List<Enabledversion> versions)
        {
            InitializeComponent();

            foreach (Enabledversion v in versions)
            {
                listView1.Items.Add(new ListViewItem(new[] {v.version, v.releasedate, v.dotnetversion}));
            }
            listView1.Items[0].Selected = true;
            listView1.Select();
        }

        /// <summary>
        /// Gets the selected version to upgrade to...
        /// </summary>
        public string SelectedVersion
        {
            get { return listView1.SelectedItems[0] != null ? listView1.SelectedItems[0].SubItems[0].Text : ""; }
        }

        private void cancelButton_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void okButton_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
