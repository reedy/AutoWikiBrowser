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

// From WikiFunctions2.dll. Converted from VB to C#

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WikiFunctions.Logging
{
    /// <summary>
    /// A form for displaying when the application is busy uploading
    /// </summary>
    public partial class UploadingPleaseWaitForm : Form
    {
        Cursor oldCursor;

        public UploadingPleaseWaitForm()
        {
            base.Shown += new EventHandler(this.Form_Shown);
            base.FormClosing += new FormClosingEventHandler(this.Form_Closing);
            InitializeComponent();
        }

        private void Form_Closing(object sender, FormClosingEventArgs e)
        {
            this.Cursor = this.oldCursor;
        }

        private void Form_Shown(object sender, EventArgs e)
        {
            this.oldCursor = this.Cursor;
            this.Cursor = Cursors.WaitCursor;
        }
    }
}