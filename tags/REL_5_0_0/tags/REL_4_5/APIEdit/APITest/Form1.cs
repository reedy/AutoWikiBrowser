using System;
using System.Windows.Forms;
using WikiFunctions.API;
using Microsoft.Win32;

namespace APITest
{
    public partial class Form1 : Form
    {
        readonly RegistryKey Config = Registry.CurrentUser.CreateSubKey("Wikipedia\\AutoWikiBrowser\\ApiTest");

        public Form1()
        {
            InitializeComponent();
        }

        ApiEdit Editor;

        private void btnLogin_Click(object sender, EventArgs e)
        {
            groupBox2.Enabled = false;
            try
            {
                Editor = new ApiEdit(txtURL.Text);
                Editor.Login(txtUsername.Text, txtPassword.Text);

                groupBox2.Enabled = true;
                txtEdit.Text = "";
                btnSave.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            Editor.Maxlag = 5;
            try
            {
                btnSave.Enabled = false;
                txtEdit.Text = Editor.Open(txtTitle.Text).Replace("\n", "\r\n");
                btnSave.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Editor.Save(txtEdit.Text.Replace("\r\n", "\n"), txtSummary.Text, chkMinor.Checked, chkWatch.Checked);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            Editor.Logout();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            txtURL.Text = (string)Config.GetValue("Wiki", "http://test.wikipedia.org/w/");
            txtUsername.Text = (string)Config.GetValue("User", "");
            txtPassword.Text = (string)Config.GetValue("Password", "");
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                Editor.Delete(txtTitle.Text, txtSummary.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void btnMove_Click(object sender, EventArgs e)
        {
            try
            {
                Editor.MovePage(txtTitle.Text,
                                WikiFunctions.Tools.VBInputBox("New article title?", "New Title", "", 100, 100),
                                WikiFunctions.Tools.VBInputBox("Move Reason?", "Move Reason", "", 100, 100), 
                                true, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void btnProtect_Click(object sender, EventArgs e)
        {
            try
            {
                Editor.Protect(txtTitle.Text,
                               WikiFunctions.Tools.VBInputBox("Protect Reason?", "Protect Reason", "", 100, 100), 
                               "",
                               Protection.Sysop,
                               Protection.Sysop,
                               false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Config.SetValue("Wiki", txtURL.Text);
            Config.SetValue("User", txtUsername.Text);
            Config.SetValue("Password", txtPassword.Text);
        }
    }
}