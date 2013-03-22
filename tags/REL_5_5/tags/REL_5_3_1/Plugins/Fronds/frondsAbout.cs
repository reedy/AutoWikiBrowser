using System;
using System.Windows.Forms;

namespace Fronds
{
    public partial class FrondsAbout : Form
    {
        public FrondsAbout()
        {
            InitializeComponent();
        }

        private void btnAboutOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FrondsAbout_Load(object sender, EventArgs e)
        {
            lblAbout.Text = lblAbout.Text.Replace("X.X.X.X", Fronds.CurrentVersion);
        }
    }
}
