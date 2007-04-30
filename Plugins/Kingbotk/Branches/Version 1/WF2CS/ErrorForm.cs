using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WikiFunctions.Logging.Uploader
{
    public partial class ErrorForm : Form
    {
        public ErrorForm(string errorMessage)
        {
            this.InitializeComponent();
            this.lblError.Text = errorMessage;
        }
    }
}