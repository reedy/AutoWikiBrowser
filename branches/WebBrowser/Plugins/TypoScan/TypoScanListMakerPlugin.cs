/*
TypoScan ListMakerPlugin
Copyright (C) 2008 Sam Reed

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

using System.Xml;
using System.IO;

using WikiFunctions.Plugin;
using System.Windows.Forms;

namespace WikiFunctions.Plugins.ListMaker.TypoScan
{
    /// <summary>
    /// Gets 100 pages from the TypoScan server
    /// </summary>
    public class TypoScanListMakerPlugin : IListMakerPlugin
    {
        protected int Iterations = 1;

        public virtual string Name
        {
            get { return "TypoScan ListMaker Plugin"; }
        }

        public List<Article> MakeList(params string[] searchCriteria)
        {
            List<Article> articles = new List<Article>();

            // TODO: must support other wikis
            if (Variables.Project != ProjectEnum.wikipedia || Variables.LangCode != LangCodeEnum.en)
            {
                MessageBox.Show("This plugin currently supports only English Wikipedia",
                    "TypoScan", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return articles;
            }

            for (int i = 0; i < Iterations; i++)
            {
                using (
                    XmlTextReader reader =
                        new XmlTextReader(new StringReader(Tools.GetHTML(Common.GetUrlFor("displayarticles")))))
                {
                    while (reader.Read())
                    {
                        if (reader.Name.Equals("article"))
                        {
                            reader.MoveToAttribute("id");
                            int id = int.Parse(reader.Value);
                            string title = reader.ReadString();
                            articles.Add(new Article(title));
                            if (!TypoScanAWBPlugin.PageList.ContainsKey(title))
                                TypoScanAWBPlugin.PageList.Add(title, id);
                        }
                    }
                }
            }
            TypoScanAWBPlugin.CheckoutTime = DateTime.Now;
            return articles;
        }

        public virtual string DisplayText
        { get { return "TypoScan"; } }

        public string UserInputTextBoxText
        { get { return ""; } }

        public bool UserInputTextBoxEnabled
        { get { return false; } }

        public void Selected()
        { }

        public bool RunOnSeparateThread
        { get { return true; } }
    }

    /// <summary>
    /// Gets 500 TypoScan pages
    /// </summary>
    public class TypoScanListMakerPlugin500 : TypoScanListMakerPlugin
    {
        public TypoScanListMakerPlugin500()
        {
            Iterations = 5;
        }

        public override string DisplayText
        { get { return base.DisplayText + " (500 pages)"; } }

        public override string Name
        { get { return base.Name + " 500"; } }
    }
}
