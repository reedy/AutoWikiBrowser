using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WikiFunctions.AWBProfiles
{
    public partial class UserPassword : Form
    {
        public UserPassword()
        {
            InitializeComponent();
        }

        public string GetPassword
        {
            get { return txtPassword.Text; }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}