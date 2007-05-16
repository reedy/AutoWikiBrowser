using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace WikiFunctions.Controls
{
    public partial class NoFlickerListView : ListView
    {
        public NoFlickerListView()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }

        protected override bool DoubleBuffered
        {
            get
            {
                return base.DoubleBuffered;
            }
            set
            {
                base.DoubleBuffered = value;
            }
        }
    }
}
