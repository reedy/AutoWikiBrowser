using System.Windows.Forms;

namespace WikiFunctions.Controls.Lists
{
    public partial class ProtectionLevel : Form
    {
        public ProtectionLevel()
        {
            InitializeComponent();
            lbType.SelectedIndex = 0;
            lbLevel.SelectedIndex = 0;
        }

        public string Type
        {
            get
            {
                switch (lbType.SelectedIndex)
                {
                    case 0:
                        return "edit";
                    case 1:
                        return "move";
                    case 2:
                        return "edit|move";
                    default:
                        return "";
                }
            }
        }

        public string Level
        {
            get
            {
                switch (lbLevel.SelectedIndex)
                {
                    case 0:
                        return "autoconfirmed";
                    case 1:
                        return "sysop";
                    case 2:
                        return "autoconfirmed|sysop";
                    default:
                        return "";
                }
            }
        }

        private void btnOk_Click(object sender, System.EventArgs e)
        {
            Close();
        }
    }
}
