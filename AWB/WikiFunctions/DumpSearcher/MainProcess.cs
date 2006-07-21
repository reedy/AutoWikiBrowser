/*
DumpSearcher
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
using System.Text;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;

namespace WikiFunctions.DumpSearcher
{
    class MainProcess
    {
        public delegate void FoundDel(string article);
        public event FoundDel FoundArticle;

        Scanners scanners;
        string FileName = "";
        int Limit = 100000;
        Stream stream;
        Regex regComments = new Regex("&lt;!--.*?--&gt;", RegexOptions.Singleline | RegexOptions.Compiled);

        public MainProcess(Scanners scns, string filename, int ResultLimit)
        {
            scanners = scns;
            FileName = filename;
            Limit = ResultLimit;
        }

        public void Start()
        {

        }

        private void Process()
        {
            string articleText = "";
            string articleTitle = "";

            try
            {
                using (XmlTextReader reader = new XmlTextReader(stream))
                {
                    reader.WhitespaceHandling = WhitespaceHandling.None;
                    while (reader.Read() && boolRun)
                    {
                        if (reader.LocalName == "page")
                        {
                            reader.ReadToFollowing("title");
                            articleTitle = reader.ReadInnerXml();
                            reader.ReadToFollowing("text");
                            articleText = reader.ReadInnerXml();

                            if (ignoreCommentsToolStripMenuItem.Checked)
                                articleText = regComments.Replace(articleText, "");

                            if (scanners.Test(articleText, articleTitle))
                            {
                                this.FoundArticle(articleTitle);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (boolMessage)
                    MessageBox.Show("Problem on " + articleTitle + "\r\n\r\n" + ex.Message);
            }
            finally
            {
                StopProgressBar();
            }
        }

        bool boolrun = true;
        public bool Run
        {
            get { return boolrun; }
            set { boolrun = value; }
        }

    }
}
