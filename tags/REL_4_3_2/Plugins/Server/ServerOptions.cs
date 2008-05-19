using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AutoWikiBrowser.Plugins.Server
{
    internal sealed partial class ServerOptions : Form
    {
        // Constructor
        internal ServerOptions()
        {
            InitializeComponent();
            // Let's be green and reuse the icon embedded in WF.dll and recycle some tooltips:
            Icon = WikiFunctions.Properties.Resources.AWBIcon;
            CopyToolTip(LoginIDTextBox, LoginIDLabel);
            CopyToolTip(PortTextBox, PortLabel);
            CopyToolTip(PasswordTextBox, PasswordLabel);
        }

        // Properties
        internal bool ServerEnabled
        {
            get { return EnabledCheckBox.Checked; }
            set { EnabledCheckBox.Checked = value; }
        }

        // Event handlers
        private void PortLabel_Click(object sender, EventArgs e)
        { PortTextBox.Focus(); }

        private void EnabledCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            AuthenticateCheckBox.Enabled = EncryptCheckBox.Enabled = PortTextBox.Enabled =
                EnabledCheckBox.Checked;
            // TODO: Implement RSA public key based encrypting of username/password credentials
            EncryptCheckBox.Enabled = false;
            // remove above line with RSA implemented
            AuthenticateCheckBox_CheckedChanged(sender, e);
        }

        private void AuthenticateCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            LoginIDTextBox.Enabled = PasswordTextBox.Enabled =
                (EnabledCheckBox.Checked && AuthenticateCheckBox.Checked);
        }

        // Helpers
        private void CopyToolTip(Control source, Control destination)
        { toolTip1.SetToolTip(destination, toolTip1.GetToolTip(source)); }
    }
}
