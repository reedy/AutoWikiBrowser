using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AutoWikiBrowser.Plugins.Delinker
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void OK_Click(object sender, EventArgs e)
        {
            DelinkerPlugin.Skip = Skip.Checked;
            DelinkerPlugin.Link = Link.Text;
            DelinkerPlugin.RemoveEmptiedSections = RemoveSections.Checked;
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            Link.Text = DelinkerPlugin.Link;
            Skip.Checked = DelinkerPlugin.Skip;
            RemoveSections.Checked = DelinkerPlugin.RemoveEmptiedSections;
        }
    }
}