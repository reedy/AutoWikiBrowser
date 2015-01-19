using System.Windows.Forms;

namespace AutoWikiBrowser
{
    public partial class CustomModuleErrors : Form
    {
        public CustomModuleErrors()
        {
            InitializeComponent();
        }

        public string ErrorText
        {
            set { textBox1.Text = value; }
        }
    }
}
