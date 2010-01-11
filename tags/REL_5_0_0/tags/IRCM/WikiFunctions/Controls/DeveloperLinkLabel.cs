/*
DeveloperLinkLabel
Copyright (C) 2008 Sam Reed

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
using System.Diagnostics;
using System.Text;

using System.Windows.Forms;

namespace WikiFunctions.Controls
{
    public enum Developers { Bluemoose, Kingboyk, Ligulem, Reedy, MaxSem }
    public partial class DeveloperLinkLabel : LinkLabel
    {
        public DeveloperLinkLabel()
            : base()
        {
            this.LinkClicked += new LinkLabelLinkClickedEventHandler(DeveloperLinkLabel_LinkClicked);
            this.Text = dev.ToString();
        }

        void DeveloperLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.LinkVisited = true;
            Tools.OpenENArticleInBrowser(this.Text, true);
        }

        Developers dev = Developers.Bluemoose;

        [DefaultValue(Developers.Bluemoose), Category("Appearance")]
        [Browsable(true)]
        public Developers WhichDeveloper
        {
            get { return dev; }
            set
            {
                dev = value;
                this.Text = dev.ToString();
            }
        }

        [Browsable(false)]
        public override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }
    }
}
