using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WikiFunctions
{
    public partial class LoginDlg : Form
    {
        public LoginDlg()
        {
            InitializeComponent();
        }

        public string UserName
        {
            get { return txtName.Text; }
            set { txtName.Text = value; }
        }

        public string Password
        {
            get { return txtPassword.Text; }
            set { txtPassword.Text = value; }
        }
        
    }
}