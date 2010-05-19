using System;
using System.Windows.Forms;

namespace AutoWikiBrowser.Plugins.Delinker
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            Icon = WikiFunctions.Properties.Resources.AWBIcon;
        }

        private void OK_Click(object sender, EventArgs e)
        {
            DelinkerAWBPlugin.Skip = Skip.Checked;
            DelinkerAWBPlugin.Link = Link.Text;
            DelinkerAWBPlugin.RemoveEmptiedSections = RemoveSections.Checked;
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            Link.Text = DelinkerAWBPlugin.Link;
            Skip.Checked = DelinkerAWBPlugin.Skip;
            RemoveSections.Checked = DelinkerAWBPlugin.RemoveEmptiedSections;
        }
    }
}