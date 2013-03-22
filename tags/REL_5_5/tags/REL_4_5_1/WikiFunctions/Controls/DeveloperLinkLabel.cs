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

using System.ComponentModel;

using System.Windows.Forms;

namespace WikiFunctions.Controls
{
    public enum Developers { Bluemoose, Kingboyk, Ligulem, Reedy, MaxSem }
    public class DeveloperLinkLabel : LinkLabel
    {
        public DeveloperLinkLabel()
        {
            LinkClicked += DeveloperLinkLabel_LinkClicked;
            WhichDeveloper = dev;
        }

        private void DeveloperLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LinkVisited = true;
            Tools.OpenENArticleInBrowser(Text, true);
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
                Text = dev.ToString();
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
