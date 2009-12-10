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
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.IO;

using WikiFunctions.AWBSettings;

namespace WikiFunctions.Controls.Lists
{
    public partial class ListSplitter : Form
    {
        private readonly UserPrefs _p;
        private readonly List<Type> _types;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefs"></param>
        /// <param name="type"></param>
        public ListSplitter(UserPrefs prefs, List<Type> type)
        {
            InitializeComponent();
            _p = prefs;
            _types = type;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefs"></param>
        /// <param name="type"></param>
        /// <param name="list"></param>
        public ListSplitter(UserPrefs prefs, List<Type> type, List<Article> list)
            : this(prefs, type)
        {
            listMaker1.Add(list);
        }

        private void ListSplitter_Load(object sender, EventArgs e)
        {
            listMaker1.MakeListEnabled = true;
        }

        private readonly Regex _characterBlacklist = new Regex(@"[/:*?<>|.]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (listMaker1.Count == 0)
            {
                MessageBox.Show("Nothing to save", "No items in List Maker");
                return;
            }

            saveTXT.FileName = RemoveBadChars();

            if (saveTXT.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(saveTXT.FileName))
                Save(saveTXT.FileName, false);
        }

        private void btnXMLSave_Click(object sender, EventArgs e)
        {
            if (listMaker1.Count == 0)
            {
                MessageBox.Show("Nothing to save", "No items in List Maker");
                return;
            }

            saveTXT.FileName = RemoveBadChars();

            if (saveXML.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(saveXML.FileName))
                Save(saveXML.FileName, true);
        }

        private string RemoveBadChars()
        {
            string text = listMaker1.SourceText;
            foreach (Match m in _characterBlacklist.Matches(listMaker1.SourceText))
            {
                text = text.Replace(m.Value, "");
            }
            return text;
        }

        private void Save(string path, bool xml)
        {
            try
            {
                listMaker1.AlphaSortList();
                int noA = listMaker1.Count;

                int roundlimit = Convert.ToInt32(numSplitAmount.Value / 2);

                if ((noA % numSplitAmount.Value) <= roundlimit)
                    noA += roundlimit;

                int noGroups =
                    Convert.ToInt32((Math.Round(noA / numSplitAmount.Value) * numSplitAmount.Value) / numSplitAmount.Value);

                if (xml)
                {
                    for (int i = 0; i < noGroups; i++)
                    {
                        List<Article> listart = new List<Article>();
                        for (int j = 0; j < numSplitAmount.Value && listMaker1.Count != 0; j++)
                        {
                            listart.Add(listMaker1.SelectedArticle());
                            listMaker1.Remove(listMaker1.SelectedArticle());
                        }

                        _p.List.ArticleList = listart;

                        using (
                            FileStream fStream = new FileStream(path.Replace(".xml", " " + (i + 1) + ".xml"),
                                                                FileMode.Create))
                        {
                            XmlSerializer xs = new XmlSerializer(typeof(UserPrefs), _types.ToArray());
                            xs.Serialize(fStream, _p);
                        }
                    }
                    MessageBox.Show("Lists Saved to AWB Settings Files");
                }
                else
                {
                    for (int i = 0; i < noGroups; i++)
                    {
                        StringBuilder strList = new StringBuilder();

                        for (int j = 0; j < numSplitAmount.Value && listMaker1.Count != 0; j++)
                        {
                            strList.AppendLine(listMaker1.SelectedArticle().ToString());
                            listMaker1.Remove(listMaker1.SelectedArticle());
                        }
                        Tools.WriteTextFileAbsolutePath(strList.ToString(), path.Replace(".txt", " " + (i + 1) + ".txt"),
                                                        false);
                    }
                    MessageBox.Show("Lists saved to text files");
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message, "Save error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }
    }
}