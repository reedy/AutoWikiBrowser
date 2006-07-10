using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Text.RegularExpressions;

namespace AutoWikiBrowser
{
    public partial class specialFilter : Form
    {
        ListBox lb;
        public specialFilter(ListBox l)
        {
            InitializeComponent();
            lb = l;
        }

        //ArrayList arrayList;
        //public specialFilter(ArrayList list)
        //{
        //    InitializeComponent();
        //    arrayList = list;
        //}

        //public ArrayList applyFilter()
        //{//filter the arraylist
        //    ArrayList filteredArray = new ArrayList();

        //    if (rdoFilterIn.Checked)
        //    {
        //        if (chkCategory.Checked)
        //            foreach (string s in arrayList)
        //                if (s.StartsWith("Category:"))
        //                    filteredArray.Add(s);

        //        if (chkTemplate.Checked)
        //            foreach (string s in arrayList)
        //                if (s.StartsWith("Template:"))
        //                    filteredArray.Add(s);
        //    }
        //    else
        //    {

        //    }

        //    return filteredArray;
        //}

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (chkContains.Checked || chkNotContains.Checked)
                FilterMatches();
        }

        private void FilterMatches()
        {
            string strMatch = txtContains.Text;
            string strNotMatch = txtDoesNotContain.Text;

            if (!chkIsRegex.Checked)
            {
                strMatch = Regex.Escape(strMatch);
                strNotMatch = Regex.Escape(strNotMatch);
            }

            Regex match = new Regex(strMatch);
            Regex notMatch = new Regex(strNotMatch);

            string s;
            int i = 0;

            while (i < lb.Items.Count)
            {
                s = lb.Items[i].ToString();
                if (chkContains.Checked && match.IsMatch(s))
                    lb.Items.RemoveAt(i);
                else if(chkNotContains.Checked && !notMatch.IsMatch(s))
                    lb.Items.RemoveAt(i);
                else
                    i++;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void chkContains_CheckedChanged(object sender, EventArgs e)
        {
            txtContains.Enabled = chkContains.Checked;
            chkIsRegex.Enabled = chkContains.Checked || chkNotContains.Checked;
        }

        private void chkNotContains_CheckedChanged(object sender, EventArgs e)
        {
            txtDoesNotContain.Enabled = chkNotContains.Checked;
            chkIsRegex.Enabled = chkContains.Checked || chkNotContains.Checked;
        }
    }
}