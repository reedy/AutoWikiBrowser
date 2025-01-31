﻿/*
Copyright � Rick Martin (ClickRick)

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

namespace AutoWikiBrowser.Plugins.TheTemplator
{
    internal sealed class AboutBox : WikiFunctions.Controls.AboutBox
    {
        protected override void Initialise()
        {
            lblVersion.Text = "Version " + 
                System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            textBoxDescription.Text = GPLNotice;
            Text = "TheTemplator Plugin";
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutBox));
            this.SuspendLayout();
            // 
            // linkLabel1
            // 
            this.toolTip1.SetToolTip(this.linkLabel1, resources.GetString("linkLabel1.ToolTip"));
            // 
            // textBoxDescription
            // 
            resources.ApplyResources(this.textBoxDescription, "textBoxDescription");
            // 
            // lblVersion
            // 
            resources.ApplyResources(this.lblVersion, "lblVersion");
            // 
            // AboutBox
            // 
            resources.ApplyResources(this, "$this");
            this.Name = "AboutBox";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
