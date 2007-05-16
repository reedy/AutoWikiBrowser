using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AutoWikiBrowser
{
    internal sealed partial class ExitQuestion : Form
    {
        public ExitQuestion(TimeSpan time, int intEdits, string msg)
        {
            InitializeComponent();

            lblMessage.Text = msg + "Are you sure you want to exit?";

            lblTimeAndEdits.Text = "You made " + intEdits.ToString() + " edits in " + time.ToString();
        }

        public bool checkBoxDontAskAgain
        {
            get { return chkDontAskAgain.Checked; }
        }
    }
}