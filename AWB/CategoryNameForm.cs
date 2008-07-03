using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

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
                else
                    return "";
            }
        }

        private void frmCategoryName_Load(object sender, EventArgs e)
        {
            lblCategory.Text = WikiFunctions.Variables.Namespaces[14];

            if (!string.IsNullOrEmpty(txtCategory.Text))
                txtCategory.SelectAll();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
