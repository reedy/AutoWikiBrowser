/*
Autowikibrowser

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
using WikiFunctions;

namespace AutoWikiBrowser
{
    public partial class CategoryNameForm : Form
    {
        public CategoryNameForm()
        {
            InitializeComponent();
        }

        public string CategoryName
        {
            get
            {
                if (!string.IsNullOrEmpty(txtCategory.Text.Trim()))
                    return lblCategory.Text + txtCategory.Text;

                return "";
            }
        }

        private void frmCategoryName_Load(object sender, EventArgs e)
        {
            lblCategory.Text = Variables.Namespaces[Namespace.Category];

            if (!string.IsNullOrEmpty(txtCategory.Text))
                txtCategory.SelectAll();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
