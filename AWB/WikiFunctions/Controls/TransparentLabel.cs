/*

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
using System.Drawing;
using System.Windows.Forms;

namespace WikiFunctions.Controls
{
    /// <summary>
    /// A label that can be transparent.
    /// </summary>
    public class TransparentLabel : Control
    {
        /// <summary>
        /// Creates a new <see cref="TransparentLabel"/> instance.
        /// </summary>
        public TransparentLabel()
        {
            TabStop = false;
        }

        /// <summary>
        /// Gets the creation parameters.
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x20;
                return cp;
            }
        }

        /// <summary>
        /// Paints the background.
        /// </summary>
        /// <param name="e">E.</param>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // do nothing
        }

        /// <summary>
        /// Paints the control.
        /// </summary>
        /// <param name="e">E.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            using (SolidBrush brush = new SolidBrush(ForeColor))
            {
                e.Graphics.DrawString(Text, Font, brush, -1, 0);
            }
        }
    }
}