using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using WikiFunctions.Controls;

namespace WikiFunctions
{
    partial class AboutIRCMon : Form
    {
        public AboutIRCMon()
        {
            InitializeComponent();

            //  Initialize the AboutBox to display the product information from the assembly information.
            //  Change assembly information settings for your application through either:
            //  - Project->Properties->Application->Assembly Information
            //  - AssemblyInfo.cs
            this.Text = String.Format("About IRC Monitor");
            labelProductName.Text = "IRC Monitor";
            labelVersion.Text = String.Format("Version {0}", 
                Assembly.GetExecutingAssembly().GetName().Version.ToString());
            labelCopyright.Text = AboutBox.AssemblyCopyright(Assembly.GetExecutingAssembly());
            textBoxDescription.Text = AboutBox.GetDetailedMessage(Assembly.GetExecutingAssembly());
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabel1.LinkVisited = true;
            Tools.OpenENArticleInBrowser("KOTOR", false);
        }

        private void Robot_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://commons.wikimedia.org/wiki/Image:Pomoc-nik.png");
        }

        private void labelCopyright_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            labelCopyright.LinkVisited = true;
            System.Diagnostics.Process.Start("http://www.gnu.org/licenses/gpl.html");
        }

    }
}
