using System.Windows.Forms;
using WikiFunctions.Lists;

namespace WikiFunctions.Controls
{
    public partial class LevelNumber : Form
    {
        public LevelNumber(bool edits)
        {
            InitializeComponent();

            if (edits)
            {
                label1.Text = "Enter number of user contribs:";
                numLevels.Minimum = 1;
                numLevels.Maximum = 25000;
            }
            else
                numLevels.Maximum = CategoryRecursiveListProvider.MaxDepth;
        }

        public int Levels
        {
            get { return (int)numLevels.Value; }
        }
    }
}