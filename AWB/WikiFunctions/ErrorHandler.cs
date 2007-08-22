using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace WikiFunctions
{
    public partial class ErrorHandler : Form
    {
        public ErrorHandler()
        {
            InitializeComponent();
        }

        private void ErrorHandler_Load(object sender, EventArgs e)
        {
            txtError.BorderStyle = BorderStyle.None;
            Text = Application.ProductName;
        }

        public static void Handle(Exception ex)
        {
            ErrorHandler Handler = new ErrorHandler();

            Handler.txtError.Text = ex.Message;


            Handler.txtDetails.Text = "Exception: " + ex.GetType().Name + "\r\nMessage: " +
                ex.Message + "\r\nCall stack:" + ex.StackTrace;

            /*
            foreach (StackFrame frame in ex.StackTrace.)
            {
                string s = "\r\n    " + frame.GetMethod();
                if (frame.GetFileLineNumber() > 0) s += "(line " + frame.GetFileLineNumber() + ")";
                Handler.txtDetails.Text += s;
            }*/

            Handler.ShowDialog();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.Clear();
            Clipboard.SetText(txtDetails.Text);
        }
    }
}