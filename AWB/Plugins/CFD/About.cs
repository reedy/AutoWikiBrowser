/*

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
using System.Text;

namespace AutoWikiBrowser.Plugins.CFD
{
    internal sealed class AboutBox : WikiFunctions.Controls.AboutBox
    {
        protected override void Initialise()
        {
            lblVersion.Text = "Version " + 
                System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            textBoxDescription.Text = GFDLNotice;
            lnkDownload.Visible = false;
            Text = "CFD Plugin";
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutBox));
            this.SuspendLayout();
            // 
            // linkLabel1
            // 
            this.toolTip1.SetToolTip(this.linkLabel1, "The AWB Team: Bluemoose (retired), MaxSem, Mets501, Reedy Boy, Kingboyk, Martinp2" +
                    "3, and others");
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Text = resources.GetString("textBoxDescription.Text");
            // 
            // AboutBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(262, 207);
            this.Name = "AboutBox";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
