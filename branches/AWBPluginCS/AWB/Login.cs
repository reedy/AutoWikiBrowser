using System;
using System.Windows.Forms;
using WikiFunctions;

namespace AutoWikiBrowser
{
    internal sealed partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
		}

		private void FormOnKeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter) {
				CloseForm();
			}
		}

		private void btnLogin_Click(object sender, EventArgs e)
		{
			CloseForm();
		}

		private void CloseForm() {
			Variables.HttpAuthUsername = txtUsername.Text;
			Variables.HttpAuthPassword = txtPassword.Text;
			Close();
		}
    }
}
