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
        public MoveDeleteDialog(bool ShowNewTitle)
        {
            InitializeComponent();

            if (!ShowNewTitle)
            {
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