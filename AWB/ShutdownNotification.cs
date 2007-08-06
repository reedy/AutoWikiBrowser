using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AutoWikiBrowser
{
    public partial class ShutdownNotification : Form
    {
        public ShutdownNotification()
        {
            InitializeComponent();
        }

        public string ShutdownType
        {
            set { label1.Text = @"AWB has finished proccessing all articles, and has been requested
to" + value + @". If you would like to stop this, please select cancel"; }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}