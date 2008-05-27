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
    /// <summary>
    /// http://developer.yahoo.com/search/
    /// </summary>
    /// <remarks>
    /// http://developer.yahoo.com/search/web/V1/webSearch.html
    /// "The Web Search service is limited to 5,000 queries per IP address per day."
    /// </remarks>
    public class YahooSearchListMakerPlugin : IListMakerPlugin
    {
        internal const string appID = "3mG9u3PV34GC4rnRXJlID0_3aUb0.XVxGZYrbFcYClzQYUqtlkn0u6iXVwYVv9sW1Q--";
        #region IListMakerPlugin Members

        string baseUrl = "http://search.yahooapis.com/WebSearchService/V1/webSearch?appid=" + appID + "&query={0}&results=100&site=" + WikiFunctions.Variables.URL + "&start={1}";

        public List<Article> Search(string[] searchCriteria)
        {
            List<Article> articles = new List<Article>();
            int start = 0;

            foreach (string s in searchCriteria)
            {
                string url = string.Format(baseUrl, s, start.ToString());
                string html, title;
                int resultsReturned = 0, totalResults = 0;

                do
                {
                    html = Tools.GetHTML(url);

                    using (XmlTextReader reader = new XmlTextReader(new StringReader(html)))
                    {
                        while (reader.Read())
                        {
                            if (reader.Name.Equals("ResultSet"))
                            {
                                string val;
                                reader.MoveToAttribute("totalResultsAvailable");
                                val = reader.Value;

                                if (!string.IsNullOrEmpty(val))
                                    totalResults = int.Parse(val);

                                reader.MoveToAttribute("totalResultsReturned");
                                val = reader.Value;

                                if (!string.IsNullOrEmpty(val))
                                    resultsReturned = int.Parse(val);
                            }

                            if (reader.Name.Equals("ClickUrl"))
                            {
                                title = reader.ReadString();

                                if (!title.StartsWith(Variables.URL + "/wiki/")) continue;

                                articles.Add(new WikiFunctions.Article(Tools.GetPageFromURL(title)));
                            }
                        }
                    }

                    if (resultsReturned < 100)
                        break;

                    int totalReturnedResults = (start + resultsReturned - 1);

                    if ((totalReturnedResults < totalResults) && (totalReturnedResults < 900))
                        start += 100;
                    else
                        break;

                    url = string.Format(baseUrl, s, start.ToString());
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
