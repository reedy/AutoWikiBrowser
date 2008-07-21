using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WikiFunctions.Controls
{
    public partial class LevelNumber : Form
    {
        public LevelNumber()
        {
            InitializeComponent();
        }

        public int Levels
        {
            get { return (int)numLevels.Value; }
        }
    }
}