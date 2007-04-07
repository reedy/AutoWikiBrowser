using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.IO;

namespace WikiFunctions
{
    public partial class AWBProfileAdd : Form
    {
        public AWBProfileAdd()
        {
            InitializeComponent();
        }

        private void chkSavePassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.Enabled = chkSavePassword.Checked;
        }

        private void AWBProfileAdd_Load(object sender, EventArgs e)
        {
            openDefaultFile.InitialDirectory = Application.StartupPath;
        }

        private void chkDefaultSettings_CheckedChanged(object sender, EventArgs e)
        {
            if (openDefaultFile.ShowDialog() == DialogResult.OK)
                txtPath.Text = openDefaultFile.FileName;
        }
    }
}