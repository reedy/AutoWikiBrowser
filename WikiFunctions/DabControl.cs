using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace WikiFunctions
{
    public partial class DabControl : UserControl
    {
        public DabControl()
        {
            InitializeComponent();
        }

        public DabControl(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
