/*
ListComparer
Copyright (C) 2007 Sam Reed

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
using System.Xml;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.IO;

using WikiFunctions.AWBSettings;

namespace WikiFunctions.Lists
{
    public partial class ListSplitter : Form
    {
        UserPrefs P;
        List<System.Type> Types;

        public ListSplitter(UserPrefs Prefs, List<System.Type> Type)
        {
            InitializeComponent();
            P = Prefs;
            Types = Type;   
        }

        public ListSplitter(UserPrefs Prefs, List<System.Type> Type, List<Article> list)
        {
            InitializeComponent();
            P = Prefs;
            Types = Type; 
            listMaker1.Add(list);
        }

        private void ListSplitter_Load(object sender, EventArgs e)
        {
            listMaker1.MakeListEnabled = true;
        }

        private void btnSplitList_Click(object sender, EventArgs e)
        {
            lvSplit.Items.Clear();
            lvSplit.BeginUpdate();

            listMaker1.AlphaSortList();
            int noA = listMaker1.Count;
 
            int roundlimit = Convert.ToInt32(numSplitAmount.Value / 2);

            if ((noA % numSplitAmount.Value) <= roundlimit)
                noA += roundlimit;

            int noGroups = Convert.ToInt32((Math.Round(noA / numSplitAmount.Value) * numSplitAmount.Value) / numSplitAmount.Value);
                        
            for (int i = 0; i < noGroups; i++)
            {
                ListViewGroup group = new ListViewGroup(i.ToString(), i.ToString());
                lvSplit.Groups.Add(group);
                
                for(int j = 0; j < numSplitAmount.Value && listMaker1.Count != 0; j++)
                {
                        ListViewItem item = new ListViewItem(listMaker1.SelectedArticle().ToString());
                        item.Group = lvSplit.Groups[i.ToString()];

                        lvSplit.Items.Add(item);

                        listMaker1.Remove(listMaker1.SelectedArticle());
                }
            }
            lvSplit.EndUpdate();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            saveTXT.FileName = listMaker1.SourceText;
            if (saveTXT.ShowDialog() == DialogResult.OK && saveTXT.FileName != "")
                saveToText(saveTXT.FileName);
        }

        private void saveToText(string Path)
        {
            int No = 1;
            try
            {
                System.IO.StreamWriter sw;
                foreach (ListViewGroup group in lvSplit.Groups)
                {
                    StringBuilder strList = new StringBuilder("");
                    for (int i = 0; i < group.Items.Count; i++)
                    {
                        strList.AppendLine(group.Items[i].Text);
                    }
                    sw = new System.IO.StreamWriter(Path.Replace(".txt", " " + No + ".txt"), false, Encoding.UTF8);
                    sw.Write(strList);
                    sw.Close();

                    No++;
                }

                MessageBox.Show("Lists Saved to Text Files");
            }
            catch (System.IO.IOException ex)
            {
                MessageBox.Show(ex.Message, "File error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXMLSave_Click(object sender, EventArgs e)
        {
            saveXML.FileName = listMaker1.SourceText;
            if (saveXML.ShowDialog() == DialogResult.OK && saveXML.FileName != "")
                saveToXML(saveXML.FileName);
        }

        private void saveToXML(string Path)
        {
            int No = 1;
            try
            {
                foreach (ListViewGroup group in lvSplit.Groups)
                {
                    List<Article> listart = new List<Article>();
                    for (int i = 0; i < group.Items.Count; i++)
                    {
                        listart.Add(new Article(group.Items[i].Text));
                    }
                    
                    P.List.ArticleList = listart;

                    using (FileStream fStream = new FileStream(Path.Replace(".xml", " " + No + ".xml"), FileMode.Create))
                    {
                        XmlSerializer xs = new XmlSerializer(typeof(UserPrefs), Types.ToArray());
                        xs.Serialize(fStream, P);
                    }

                    No += 1;
                }
                MessageBox.Show("Lists Saved to AWB Settings Files");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error saving settings", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lvSplit.Items.Clear();
        }
    }
}