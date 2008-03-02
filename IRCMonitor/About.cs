/*

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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using WikiFunctions;
using WikiFunctions.Controls;

namespace IRCM
{
    partial class AboutIrcMon : Form
    {
        public AboutIrcMon()
        {
            InitializeComponent();

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
            Tools.OpenURLInBrowser("http://commons.wikimedia.org/wiki/Image:Pomoc-nik.png");
        }

        private void labelCopyright_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            labelCopyright.LinkVisited = true;
            Tools.OpenURLInBrowser("http://www.gnu.org/licenses/gpl.html");
        }
    }
}
