using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;

namespace WikiFunctions.Controls
{
    public partial class AboutBox : Form
    {
        public AboutBox()
        {
            InitializeComponent();

            //  Initialize the AboutBox to display the product information from the assembly information.
            //  Change assembly information settings for your application through either:
            //  - Project->Properties->Application->Assembly Information
            //  - AssemblyInfo.cs

            Initialise();
        }

        protected virtual void Initialise()
        {
            lblVersion.Text = "Version " + Tools.VersionString;
            textBoxDescription.Text = GFDLNotice;  
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabel1.LinkVisited = true;
            Tools.OpenENArticleInBrowser("WP:AWB", false);
        }

        private void lnkDownload_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            lnkDownload.LinkVisited = true;
            System.Diagnostics.Process.Start("http://download.wikimedia.org/enwiki/");

        }

        #region Shared
        public static string GFDLNotice
        { get { return Properties.Resources.GFDL; } }

        public static string AssemblyDescription(Assembly Ass)
        {
            // Get all Description attributes on this assembly
            object[] attributes = Ass.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
            // If there aren't any Description attributes, return an empty string
            if (attributes.Length == 0)
                return "";
            // If there is a Description attribute, return its value
            return ((AssemblyDescriptionAttribute)attributes[0]).Description;
        }

        public static string AssemblyCopyright(Assembly Ass)
        {
            // Get all Copyright attributes on this assembly
            object[] attributes = Ass.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            // If there aren't any Copyright attributes, return an empty string
            if (attributes.Length == 0)
                return "";
            // If there is a Copyright attribute, return its value
            return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
        }

        public static string GetDetailedMessage(Assembly Ass)
        { return AssemblyDescription(Ass) + System.Environment.NewLine + System.Environment.NewLine + GFDLNotice; }
        #endregion
    }
}
