using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace AutoWikiBrowser
{
    public partial class specialFilter : Form
    {
        ArrayList arrayList;
        public specialFilter(ArrayList list)
        {
            InitializeComponent();
            arrayList = list;
        }

        public ArrayList applyFilter()
        {//filter the arraylist
            ArrayList filteredArray = new ArrayList();

            if (rdoFilterIn.Checked)
            {
                if (chkCategory.Checked)
                    foreach (string s in arrayList)
                        if (s.StartsWith("Category:"))
                            filteredArray.Add(s);

                if (chkTemplate.Checked)
                    foreach (string s in arrayList)
                        if (s.StartsWith("Template:"))
                            filteredArray.Add(s);
            }
            else
            {

            }

            return filteredArray;
        }

        private void chkContains_CheckedChanged(object sender, EventArgs e)
        {
            txtContains.Enabled = chkContains.Checked;
        }
    }
}