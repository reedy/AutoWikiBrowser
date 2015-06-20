/*
Autowikibrowser
Copyright (C) 2007 Martin Richards
(C) 2007 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/

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
using System.Windows.Forms;
using System.Reflection;
using WikiFunctions;

namespace AutoWikiBrowser
{
    internal sealed partial class AboutBox : Form
    {
        public AboutBox(string ieVersion)
        {
            InitializeComponent();

            lblAWBVersion.Text = "Version " + Program.VersionString;
            lblRevision.Text = "SVN " + Variables.Revision;
            txtWarning.Text = WikiFunctions.Controls.AboutBox.GetDetailedMessage(Assembly.GetExecutingAssembly());

            txtVersions.Text = string.Format(@"Internet Explorer version: {0}
.NET version: {1}
Windows version: {2}",
                     ieVersion,
                     Environment.Version,
                     Environment.OSVersion.Version.Major + "." + Environment.OSVersion.Version.Minor
                );
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void linkAWBPage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkAWBPage.LinkVisited = true;
            Tools.OpenENArticleInBrowser("Wikipedia:AutoWikiBrowser", false);
        }

       private void UsageStatsLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            UsageStatsLabel.LinkVisited = true;
            UsageStats.OpenUsageStatsURL();
        }

        private void linkPhabricator_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkPhabricator.LinkVisited = true;
            Tools.OpenURLInBrowser("https://phabricator.wikimedia.org/maniphest/task/create/?projects=AutoWikiBrowser");
        }
    }
}
