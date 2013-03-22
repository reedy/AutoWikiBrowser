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
        protected int Count = 100;

        public virtual string Name
        {
            get { return "TypoScan ListMaker Plugin"; }
        }

        public List<Article> MakeList(params string[] searchCriteria)
        {
            List<Article> articles = new List<Article>();

            using (
                XmlTextReader reader =
                    new XmlTextReader(new StringReader(Tools.GetHTML(Common.GetUrlFor("displayarticles") + "&count=" + Count))))
            {
                while (reader.Read())
                {
                    if (reader.Name.Equals("site"))
                    {
                        reader.MoveToAttribute("address");
                        string site = reader.Value;

                        if (site != Common.GetSite())
                            //Probably shouldnt get this as the wanted site was sent to the server
                        {
                            MessageBox.Show("Wrong Site");
                            return articles;
                        }
                    }
                    else if (reader.Name.Equals("article"))
                    {
                        reader.MoveToAttribute("id");
                        int id = int.Parse(reader.Value);
                        string title = reader.ReadString();
                        articles.Add(new Article(title));
                        if (!TypoScanBasePlugin.PageList.ContainsKey(title))
                            TypoScanBasePlugin.PageList.Add(title, id);
                    }
                }
            }
            TypoScanBasePlugin.CheckoutTime = DateTime.Now;
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

        public virtual bool StripUrl
        { get { return false; } }
    }

    /// <summary>
    /// Gets 500 TypoScan pages
    /// </summary>
    public class TypoScanListMakerPlugin500 : TypoScanListMakerPlugin
    {
        public TypoScanListMakerPlugin500()
        {
            Count = 500;
        }

        public override string DisplayText
        { get { return base.DisplayText + " (500 pages)"; } }

        public override string Name
        { get { return base.Name + " 500"; } }
    }
}
