/*
Yahoo Search Plugin
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
using System.Text;

using System.Xml;
using System.IO;

using WikiFunctions;
using WikiFunctions.Plugin;

namespace WikiFunctions.Plugins.ListMaker.YahooSearch
{
	//http://developer.yahoo.com/search/
    public class YahooSearchListMakerPlugin : IListMakerPlugin
    {
        internal const string appID = "3mG9u3PV34GC4rnRXJlID0_3aUb0.XVxGZYrbFcYClzQYUqtlkn0u6iXVwYVv9sW1Q--";
        #region IListMakerPlugin Members

        public List<Article> Search(string[] searchCriteria)
        {
            List<Article> articles = new List<Article>();

            foreach (string s in searchCriteria)
            {
                string url = "http://search.yahooapis.com/WebSearchService/V1/webSearch?appid=" + appID + "&query=" + s + "&results=100&site=" + WikiFunctions.Variables.URL;
                string html = Tools.GetHTML(url);
                string title;

                do
                {
                    using (XmlTextReader reader = new XmlTextReader(new StringReader(html)))
                    {
                        while (reader.Read())
                        {
                            if (reader.Name.Equals("ClickUrl"))
                            {
                                title = reader.ReadString();

                                if (!title.StartsWith(Variables.URL + "/wiki/")) continue;

                                articles.Add(new WikiFunctions.Article(Tools.GetPageFromURL(title)));
                            }
                        }
                    }

                    //TODO:More Pages of Results

                    break;
                } while (true);
            }

            return articles;
        }

        public string Name
        {
            get { return "Yahoo Search Plugin"; }
        }

        public string DisplayText
        {
            get { return "Yahoo Search"; }
        }

        public void Initialise(WikiFunctions.Controls.Lists.ListMaker sender)
        {
            //throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
