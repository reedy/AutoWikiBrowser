using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WikiFunctions.DatabaseScanner
{
    public partial class Results : Form
    {
        public Results(string result)
        {
            InitializeComponent();

            textBox1.Text = result;
        }
    }
}