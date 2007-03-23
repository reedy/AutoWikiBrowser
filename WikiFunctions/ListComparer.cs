/*
ListComparer
Copyright (C) 2006 Martin Richards

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace WikiFunctions.Lists
{
    /// <summary>
    /// Provides a form for comparing 2 lists, to find duplicates and/or removing one list from another.
    /// </summary>
    public partial class ListComparer : Form
    {
        public ListComparer()
        {
            InitializeComponent();
            listMaker1.MakeListEnabled = true;
            listMaker2.MakeListEnabled = true;
        }

        private void GetDuplicates()
        {
            foreach (Article article in listMaker1)
            {
                if (listMaker2.Contains(article))
                    lbBoth.Items.Add(article.Name);
                else
                    lbOnly1.Items.Add(article.Name);
            }

            foreach (Article article in listMaker2)
            {
                if (!listMaker1.Contains(article))
                    lbOnly2.Items.Add(article.Name);
            }
        
            updateCounts();
        }

        private string TurnFirstToUpper(string input)
        {
            //turns first character to uppercase
            if (input.Length > 0)
            {
                input = input.Trim();
                string temp = input.Substring(0, 1);
                return temp.ToUpper() + input.Remove(0, 1);
            }
            else
                return "";
        }

        private void SaveList(StringBuilder strList)
        {
            try
            {
                if (saveListDialog.ShowDialog() == DialogResult.OK)
                {
                    StreamWriter sw = new StreamWriter(saveListDialog.FileName, false, Encoding.UTF8);
                    sw.Write(strList);
                    sw.Close();
                }
            }
            catch { }
        }

        //Regex reg = new Regex(" ?\\(.*?\\)$", RegexOptions.Compiled);
        //Regex reg3 = new Regex("^[A-Za-z]* [A-Za-z]*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private void btnGo_Click(object sender, EventArgs e)
        {
            //int i = 0;
            //string s = "";

            //while (i < lbFirst.Items.Count)
            //{
            //    s = lbFirst.Items[i].ToString();

            //    s = reg.Replace(s, "");

            //    if (reg3.IsMatch(s))
            //        lbBoth.Items.Add(lbFirst.Items[i].ToString());

            //    i++;
            //}
            //return;
            lbBoth.Items.Clear();
            lbOnly1.Items.Clear();
            lbOnly2.Items.Clear();
            GetDuplicates();
            lblOnly1.Text = lbOnly1.Items.Count.ToString() + " pages";
            lblOnly2.Text = lbOnly2.Items.Count.ToString() + " pages";
            lblBoth.Text = lbBoth.Items.Count.ToString() + " pages";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int i = 0;
            string s = "";
            StringBuilder strList = new StringBuilder("");

            while (i < lbBoth.Items.Count)
            {
                s = lbBoth.Items[i].ToString();
                strList.AppendLine("# [[" + s + "]]");
                i++;
            }
            SaveList(strList);
        }



        private void btnClear_Click(object sender, EventArgs e)
        {
            lbBoth.Items.Clear();
            lbOnly1.Items.Clear();
            lbOnly2.Items.Clear();

            updateCounts();
        }

        private void updateCounts()
        {
            lblBoth.Text = lbBoth.Items.Count.ToString();
            lblOnly1.Text = lbOnly1.Items.Count.ToString();
            lblOnly2.Text = lbOnly2.Items.Count.ToString();
        }

        private void btnSaveOnly1_Click(object sender, EventArgs e)
        {
            int i = 0;
            string s = "";
            StringBuilder strList = new StringBuilder("");

            while (i < lbOnly1.Items.Count)
            {
                s = lbOnly1.Items[i].ToString();
                strList.AppendLine("# [[" + s + "]]");
                i++;
            }
            SaveList(strList);
        }

        private void btnSaveOnly2_Click(object sender, EventArgs e)
        {
            int i = 0;
            string s = "";
            StringBuilder strList = new StringBuilder("");

            while (i < lbOnly2.Items.Count)
            {
                s = lbOnly2.Items[i].ToString();
                strList.AppendLine("# [[" + s + "]]");
                i++;
            }
            SaveList(strList);
        }


    }
}