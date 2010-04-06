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
using System.Windows.Forms;

namespace WikiFunctions.Logging
{
    /// <summary>
    /// A form for displaying when the application is busy uploading
    /// </summary>
    public partial class UploadingPleaseWaitForm : Form
    {
        Cursor OldCursor;

        public UploadingPleaseWaitForm()
        {
            InitializeComponent();
        }

        private void Form_Closing(object sender, FormClosingEventArgs e)
        {
            Cursor = OldCursor;
        }

        private void Form_Shown(object sender, EventArgs e)
        {
            OldCursor = Cursor;
            Cursor = Cursors.WaitCursor;
        }
    }
}