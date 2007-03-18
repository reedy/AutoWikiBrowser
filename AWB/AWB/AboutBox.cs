/*
    Autowikibrowser
    Copyright (C) 2006 Martin Richards

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;

namespace AutoWikiBrowser
{
    partial class AboutBox : Form
    {
        private const string WikiEN = "http://en.wikipedia.org/wiki";

        public AboutBox() { } //default

        public AboutBox(string IEVersion, TimeSpan time, int intEdits)
        {
            InitializeComponent();

            lblIEVersion.Text = "IE version: " + IEVersion;
            lblAWBVersion.Text = "AWB Version " + AssemblyVersion;
            textBoxDescription.Text = AssemblyDescription;
            lblOSVersion.Text = "Windows version: " + Environment.OSVersion.Version.Major.ToString() + "." + Environment.OSVersion.Version.Minor.ToString();
            lblNETVersion.Text = ".NET Version: " + Environment.Version.ToString();
            lblTimeAndEdits.Text = "You have made " + intEdits.ToString() + " edits in " + time.ToString();
        }

        #region Assembly Attribute Accessors

        
        public static string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public static string AssemblyDescription
        {
            get
            {
                // Get all Description attributes on this assembly
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                // If there aren't any Description attributes, return an empty string
                if (attributes.Length == 0)
                    return "";
                // If there is a Description attribute, return its value
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public static string AssemblyProduct
        {
            get
            {
                // Get all Product attributes on this assembly
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                // If there aren't any Product attributes, return an empty string
                if (attributes.Length == 0)
                    return "";
                // If there is a Product attribute, return its value
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public static string AssemblyCopyright
        {
            get
            {
                // Get all Copyright attributes on this assembly
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                // If there aren't any Copyright attributes, return an empty string
                if (attributes.Length == 0)
                    return "";
                // If there is a Copyright attribute, return its value
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public static string AssemblyCompany
        {
            get
            {
                // Get all Company attributes on this assembly
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                // If there aren't any Company attributes, return an empty string
                if (attributes.Length == 0)
                    return "";
                // If there is a Company attribute, return its value
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion

        private void okButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void linkBluemoose_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkBluemoose.LinkVisited = true;
            System.Diagnostics.Process.Start(WikiEN + "/User:Bluemoose");
        }

        private void linkLigulem_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLigulem.LinkVisited = true;
            System.Diagnostics.Process.Start(WikiEN + "/User:Ligulem");
        }

        private void linkMaxSem_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkMaxSem.LinkVisited = true;
            System.Diagnostics.Process.Start(WikiEN + "/User:MaxSem");
        }

        private void linkMets501_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkMets501.LinkVisited = true;
            System.Diagnostics.Process.Start(WikiEN + "/User:Mets501");
        }

        private void linkReedy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkReedy.LinkVisited = true;
            System.Diagnostics.Process.Start(WikiEN + "/User:Reedy Boy");
        }

        private void linkKingboy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkKingboy.LinkVisited = true;
            System.Diagnostics.Process.Start(WikiEN + "/User:Kingboyk");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkAWBPage.LinkVisited = true;
            System.Diagnostics.Process.Start(WikiEN + "/Wikipedia:AutoWikiBrowser");
        }
        
        private void linkLabel6_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkBugs.LinkVisited = true;
            System.Diagnostics.Process.Start(WikiEN + "/Wikipedia_talk:AutoWikiBrowser/Bugs");
        }

        private void linkLabel7_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkFeatureRequests.LinkVisited = true;
            System.Diagnostics.Process.Start(WikiEN + "/Wikipedia_talk:AutoWikiBrowser/Feature_requests");
        }
    }
}
