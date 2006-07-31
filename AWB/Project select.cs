using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WikiFunctions;

namespace AutoWikiBrowser
{
    public partial class ProjectSelect : Form
    {
        public ProjectSelect(string lang, string proj)
        {
            InitializeComponent();           

            foreach (LangCodeEnum l in Enum.GetValues(typeof(LangCodeEnum)))
                cmboLang.Items.Add(l.ToString());

            foreach (ProjectEnum l in Enum.GetValues(typeof(ProjectEnum)))
                cmboProject.Items.Add(l.ToString());

            cmboLang.SelectedItem = lang;
            cmboProject.SelectedItem = proj;                
        }

        public LangCodeEnum Language
        {
            get
            {
                LangCodeEnum l = (LangCodeEnum)Enum.Parse(typeof(LangCodeEnum), cmboLang.SelectedItem.ToString());
                return l;

            }  
        }
        public ProjectEnum Project
        {
            get
            {
                ProjectEnum p = (ProjectEnum)Enum.Parse(typeof(ProjectEnum), cmboProject.SelectedItem.ToString());
                return p;
            }
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