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
        int Counter = 30;
        string sShutdownType;

        public ShutdownNotification()
        {
            InitializeComponent();
        }

        public string ShutdownType
        {
            set {
                sShutdownType = value;
                textBox1.Text = @"AWB has finished processing all articles, and has been requested
to " + value + @". If you would like to stop this, please select cancel.";
                SetShutdownLabel(30);
            }
        }

        private void SetShutdownLabel(int time)
        {
            label1.Text = "Time until " + sShutdownType + ": " + time.ToString();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            Counter--;
            if (Counter != 0)
            {
                SetShutdownLabel(Counter);
                Application.DoEvents();
            }
            else
                this.Close();
        }
    }
}