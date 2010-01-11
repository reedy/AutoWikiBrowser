/*
Autowikibrowser
Copyright (C) 2007 Mets501, Stephen Kennedy

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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WikiFunctions;

namespace AutoWikiBrowser
{
    internal sealed partial class Help : WikiFunctions.Controls.Help
    {
        public Help()
        {
            this.Text = "AWB Help";
            InitializeComponent();
        }

        // TODO: convert hyperlinks in the page to use the simple skin too.
        // TODO: How about bug reporting using this tool, could even send exception data?
        // TODO: After performing an update, display a message in Help browser with link to changelog (ShowHelp();)
        protected override string URL
        {
            // get { return "http://en.wikipedia.org/wiki/Wikipedia:AutoWikiBrowser/User_manual"; } // Met's version
            get { return Tools.GetENLinkWithSimpleSkinAndLocalLanguage("Wikipedia:AutoWikiBrowser/User_manual"); 
            } // printable=yes looks nice but unfortunately zaps the hyperlinks
            // Not hardcoding URL allows some of the Wikipedia links to be displayed in user's current language
        }

        public static void ShowHelp(Help h)
        { ShowHelp(h, ""); }

        public static void ShowHelpEN(Help h, string Article)
        { ShowHelp(h, Tools.GetENLinkWithSimpleSkinAndLocalLanguage(Article)); }

        public static void ShowHelp(Help h, string url)
        {
            if (h == null || h.IsDisposed)
                h = new Help();
            h.Show();
            if (string.IsNullOrEmpty(url))
                h.Navigate();
            else
                h.Navigate(url);
        }

        private void Navigate()
        { Navigate(URL); }

        protected override void Help_Load(object sender, EventArgs e)
        { } // do nothing, we'll load URL in ShowHelp()
    }
}

