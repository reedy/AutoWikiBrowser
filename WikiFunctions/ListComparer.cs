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
        }
        Regex regexFindPattern = new Regex("\\[\\[(.*?)(\\]\\]|\\|)", RegexOptions.Compiled);

        #region list 1
        private void GetTextFileArticles1()
        {
            string ArticleText = "";
            string FileName = "";
            string x = "";

            try
            {
                if (openListDialog.ShowDialog() == DialogResult.OK)
                {
                    FileName = openListDialog.FileName;

                    StreamReader sr = new StreamReader(FileName, Encoding.UTF8);
                    ArticleText = sr.ReadToEnd();
                    sr.Close();

                   foreach (Match m in regexFindPattern.Matches(ArticleText))
                    {
                         x = m.Groups[1].Value;
                        lbFirst.Items.Add(TurnFirstToUpper(x));
                    }
                }
            }
            catch { }

            updateCounts();
        }

        #endregion

        #region list 2
        private void GetTextFileArticles2()
        {
            string ArticleText = "";
            string FileName = "";
            string x = "";

            try
            {
                if (openListDialog.ShowDialog() == DialogResult.OK)
                {
                    //user chose a file name and pressed OK
                    //get the filename
                    FileName = openListDialog.FileName;

                    StreamReader sr = new StreamReader(FileName, Encoding.UTF8);
                    ArticleText = sr.ReadToEnd();
                    sr.Close();

                    foreach (Match m in regexFindPattern.Matches(ArticleText))
                    {
                        x = m.Groups[1].Value;
                        lbSecond.Items.Add(TurnFirstToUpper(x));
                    }
                }
            }
            catch { }

            updateCounts();
        }

        #endregion


        private void GetDuplicates()
        {
            int i = 0;
            string s = "";

            while (i < lbFirst.Items.Count)
            {
                s = lbFirst.Items[i].ToString();

                if (lbSecond.Items.Contains(s))
                {
                    lbBoth.Items.Add(s);
                    lbFirst.Items.Remove(s);
                    lbSecond.Items.Remove(s);
                }
                else
                    i++;
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

        private void btnOpen1_Click(object sender, EventArgs e)
        {
            GetTextFileArticles1();
        }

        private void btnOpen2_Click(object sender, EventArgs e)
        {
            GetTextFileArticles2();
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

            GetDuplicates();
            lblFirst.Text = lbFirst.Items.Count.ToString() + " only in list 1";
            lblSecond.Text = lbSecond.Items.Count.ToString() + " only in list 2";
            lblBoth.Text = lbBoth.Items.Count.ToString() + " in both lists";
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
            lbFirst.Items.Clear();
            lbSecond.Items.Clear();
        }

        private void updateCounts()
        {
            lblFirst.Text = lbFirst.Items.Count.ToString();
            lblSecond.Text = lbSecond.Items.Count.ToString();
            lblBoth.Text = lbBoth.Items.Count.ToString();
        }

        private void btnSave1_Click(object sender, EventArgs e)
        {
            int i = 0;
            string s = "";
            StringBuilder strList = new StringBuilder("");

            while (i < lbFirst.Items.Count)
            {
                s = lbFirst.Items[i].ToString();
                strList.AppendLine("# [[" + s + "]]");
                i++;
            }
            SaveList(strList);
        }

        private void btnSave2_Click(object sender, EventArgs e)
        {
            int i = 0;
            string s = "";
            StringBuilder strList = new StringBuilder("");

            while (i < lbSecond.Items.Count)
            {
                s = lbSecond.Items[i].ToString();
                strList.AppendLine("# [[" + s + "]]");
                i++;
            }
            SaveList(strList);
        }

    }
}