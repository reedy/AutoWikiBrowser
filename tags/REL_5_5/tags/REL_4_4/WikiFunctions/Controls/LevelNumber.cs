using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WikiFunctions.Lists;

namespace WikiFunctions.Controls
{
    public partial class LevelNumber : Form
    {
        public LevelNumber()
        {
            InitializeComponent();
            numLevels.Maximum = CategoryRecursiveListProvider.MaxDepth;
        }

        public int Levels
        {
            get { return (int)numLevels.Value; }
        }
    }
}