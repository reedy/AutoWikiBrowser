using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WikiFunctions.Lists
{
    public partial class ListSplitter : Form
    {
        public ListSplitter()
        {
            InitializeComponent();
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
                    string path = Application.StartupPath + "\\" + txtFile.Text + " " + group.Name.ToString() + ".txt";
                    sw = new System.IO.StreamWriter(path, false, Encoding.UTF8);
                    sw.Write(strList);
                    sw.Close();
                }

                MessageBox.Show("Lists Saved");
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

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lvSplit.Items.Clear();
        }
    }
}