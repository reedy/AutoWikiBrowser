using System;
using System.Windows.Forms;

namespace AutoWikiBrowser
{
    public partial class ShutdownNotification : Form
    {
        int Counter = 120;  // 2 minutes
        string SType;

        public ShutdownNotification()
        {
            InitializeComponent();
        }

        public string ShutdownType
        {
            set {
                SType = value;
                txtPrompt.Text = string.Format(txtPrompt.Text, value);
                SetShutdownLabel(Counter);
            }
        }

        private void SetShutdownLabel(int time)
        {
            lblTimer.Text = "Time until " + SType + ": " + time;
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
