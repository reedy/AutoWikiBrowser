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
using System.Windows.Forms;
using System.IO;

using WikiFunctions.AWBSettings;

namespace WikiFunctions.Controls.Lists
{
    public partial class ListSplitter : Form
    {
        private readonly UserPrefs _p;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefs"></param>
        public ListSplitter(UserPrefs prefs)
        {
            InitializeComponent();
            _p = prefs;
            listMaker1.NoOfArticlesChanged += UpdateButtons;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefs"></param>
        /// <param name="list"></param>
        public ListSplitter(UserPrefs prefs, List<Article> list)
            : this(prefs)
        {
            listMaker1.Add(list);
        }

        private void ListSplitter_Load(object sender, EventArgs e)
        {
            listMaker1.MakeListEnabled = true;
        }

        private readonly Regex _characterBlacklist = new Regex(@"[""/:*?<>|.]",
                                                               RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private void UpdateButtons(object sender, EventArgs e)
        {
            btnSave.Enabled = btnXMLSave.Enabled = listMaker1.Count > 0;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveDialog(saveTXT, false);
        }

        private void btnXMLSave_Click(object sender, EventArgs e)
        {
            SaveDialog(saveXML, true);
        }

        private void SaveDialog(SaveFileDialog sfd, bool xml)
        {
            sfd.FileName = RemoveBadChars();

            if (sfd.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(sfd.FileName))
                Save(sfd.FileName, xml);
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
            listMaker1.AlphaSortList();
            listMaker1.BeginUpdate();
            try
            {
                int noA = listMaker1.Count;

                int roundlimit = Convert.ToInt32(numSplitAmount.Value/2);

                if ((noA%numSplitAmount.Value) <= roundlimit)
                    noA += roundlimit;

                int noGroups =
                    Convert.ToInt32(Math.Round(noA/numSplitAmount.Value));

                int baseIndex = 0;
                int splitValue = (int) numSplitAmount.Value;
                var articles = listMaker1.GetArticleList();
				int minValueCount = Math.Min(splitValue, articles.Count);
                if (xml)
                {
                    string pathPrefix = path.Replace(".xml", " {0}.xml");

                    for (int i = 1; i <= noGroups; i++)
                    {
                        _p.List.ArticleList = articles.GetRange(baseIndex, minValueCount);
                        baseIndex += splitValue;
                        UserPrefs.SavePrefs(_p, string.Format(pathPrefix, i));
                    }
                    MessageBox.Show("Lists Saved to AWB Settings Files");
                }
                else
                {
                    string pathPrefix = path.Replace(".txt", " {0}.txt");
                    for (int i = 1; i <= noGroups; i++)
                    {
                        StringBuilder strList = new StringBuilder();
                        foreach (Article a in articles.GetRange(baseIndex, Math.Min(articles.Count-baseIndex, minValueCount)))
                        {
                            strList.AppendLine(a.ToString());
                        }
                        Tools.WriteTextFileAbsolutePath(strList.ToString().TrimEnd(), string.Format(pathPrefix, i),
                                                        false);
                        baseIndex += splitValue;
                    }
                    MessageBox.Show("Lists saved to text files");
                }

                listMaker1.Clear();
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message, "Save error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex);
            }


            listMaker1.EndUpdate();
        }
    }
}