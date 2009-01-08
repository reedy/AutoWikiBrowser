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
using System.Windows.Forms;
using System.Reflection;

namespace WikiFunctions.Controls
{
    public partial class AboutBox : Form
    {
        public AboutBox()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The AboutBox form is being initialised. Override this if you are inheriting and recycling the form.
        /// </summary>
        protected virtual void Initialise()
        {
            lblVersion.Text = "Version " + Tools.VersionString;
            textBoxDescription.Text = GPLNotice;
        }

        protected virtual void okButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected virtual void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabel1.LinkVisited = true;
            Tools.OpenENArticleInBrowser("WP:AWB", false);
        }

        #region Shared
        /// <summary>
        /// Returns a GFDL authors and copyright notice for use within AWB projects
        /// </summary>
        public static string GPLNotice
        { get { return Properties.Resources.GPL; } }

        /// <summary>
        /// Extracts an assembly description (usually created by Visual Studio?)
        /// </summary>
        /// <returns></returns>
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
        { return AssemblyDescription(Ass) + Environment.NewLine + Environment.NewLine + GPLNotice; }
        #endregion

        private void AboutBox_Load(object sender, EventArgs e)
        {
            Initialise();
        }
    }
}
