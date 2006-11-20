using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WikiFunctions.Controls
{
    public partial class MoveDeleteDialog : Form
    {
        public MoveDeleteDialog(bool IsMove)
        {
            InitializeComponent();

            if (IsMove)
            {
                this.Text = "Move";
                btnOk.Text = "Move";
            }
            else
            {
                this.Text = "Delete";
                btnOk.Text = "Delete";
                this.Size = new Size(this.Width, 115);

                label1.Visible = false;
                txtNewTitle.Visible = false;
            }
        }

        public string NewTitle
        {
            get { return txtNewTitle.Text; }
            set { txtNewTitle.Text = value; }
        }

        public string Summary
        {
            get { return txtSummary.Text; }
            set { txtSummary.Text = value; }
        }
    }
}