using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
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
            this.Close();
        }
    }
}
