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

namespace AutoWikiBrowser
{
    public partial class Help : WikiFunctions.Controls.Help
    {
        public Help()
        {
            InitializeComponent();
        }

        // TODO: convert hyperlinks to use the simple skin too. Add menu and buttons to base form.
        // TODO: How about bug reporting using this tool, could even send exception data?
        // TODO: After performing an update, display a message in Help browser with link to changelog
        protected override string URL
        {
            // get { return "http://en.wikipedia.org/wiki/Wikipedia:AutoWikiBrowser/User_manual"; } // Met's version
            get { return "http://en.wikipedia.org/w/index.php?title=Wikipedia:AutoWikiBrowser/User_manual" +
                "&useskin=simple&uselang=" + WikiFunctions.Variables.LangCode.ToString(); 
            } // printable=yes looks nice but unfortunately zaps the hyperlinks
        }
    }
}

