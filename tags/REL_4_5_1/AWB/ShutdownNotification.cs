using System;
using System.Windows.Forms;

namespace AutoWikiBrowser
{
    public partial class ShutdownNotification : Form
    {
        int Counter = 120;  // 2 minutes
        string sShutdownType;

        public ShutdownNotification()
        {
            InitializeComponent();
        }

        public string ShutdownType
        {
            set {
                sShutdownType = value;
                txtPrompt.Text = @"AutoWikiBrowser has finished processing all pages and has been set to " + value + @". If you would like to stop this, press cancel.";
                SetShutdownLabel(Counter);
            }
        }

        private void SetShutdownLabel(int time)
        {
            lblTimer.Text = "Time until " + sShutdownType + ": " + time;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
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
                Close();
        }
    }
}
