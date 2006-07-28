using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AutoWikiBrowser
{
    public partial class ProjectSelect : Form
    {
        public ProjectSelect(string lang, string proj)
        {
            InitializeComponent();
            cmboLang.SelectedItem = lang;
            cmboProject.SelectedItem = proj;
        }

        public string Language
        {
            get { return cmboLang.SelectedItem.ToString();}
        }
        public string Project
        {
            get { return cmboProject.SelectedItem.ToString();}
        }
        public bool SetAsDefault
        {
            get { return chkSetAsDefault.Checked; }
        }

        private void cmboProject_SelectedIndexChanged(object sender, EventArgs e)
        {//disable other languages that are not wikipedia.
            if (cmboProject.SelectedIndex >= 3)
            {
                cmboLang.SelectedIndex = 0;
                cmboLang.Enabled = false;
            }
            else
                cmboLang.Enabled = true;
        }
    }
}