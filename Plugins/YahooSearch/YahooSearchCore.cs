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
    /// Plugin to Search Yahoo
    /// http://developer.yahoo.com/search/
    /// </summary>
    /// <remarks>
    /// http://developer.yahoo.com/search/web/V1/webSearch.html
    /// "The Web Search service is limited to 5,000 queries per IP address per day."
    /// This search will also only return 1000 results per search
    /// </remarks>
    public class YahooSearchListMakerPlugin : IListMakerPlugin
    {
        internal const string appID = "3mG9u3PV34GC4rnRXJlID0_3aUb0.XVxGZYrbFcYClzQYUqtlkn0u6iXVwYVv9sW1Q--";
        #region IListMakerPlugin Members

        string baseUrl = "http://search.yahooapis.com/WebSearchService/V1/webSearch?appid=" + appID + "&query={0}&results={1}&site=" + WikiFunctions.Variables.URL + "&start={2}";

        public List<Article> MakeList(string[] searchCriteria)
        {
            List<Article> articles = new List<Article>();
            int start = 1, noResults = 100;

            foreach (string s in searchCriteria)
            {
                string url = string.Format(baseUrl, s, noResults, start.ToString());
                string html, title;
                int resultsReturned = 0, totalResults = 0;

                do
                {
                    html = Tools.GetHTML(url);
                    //Console.WriteLine(url);

                    using (XmlTextReader reader = new XmlTextReader(new StringReader(html)))
                    {
                        while (reader.Read())
                        {
                            if (reader.Name.Equals("Message"))
                            {
                                if (string.Compare(reader.ToString(), "limit exceeded", true) == 0)
                                {
                                    System.Windows.Forms.MessageBox.Show("Query limit for Yahoo Exceeded. Please try again later");
                                    return articles;
                                }
                            }

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

                            if (reader.Name.Equals("DisplayUrl"))
                            {
                                title = Tools.GetTitleFromURL("http://" + reader.ReadString());

                                if (!string.IsNullOrEmpty(title))
                                    articles.Add(new WikiFunctions.Article(title));
                            }
                        }
                    }

                    if (resultsReturned < 100)
                        break;

                    if ((articles.Count < totalResults) && (articles.Count <= 900))
                        start += noResults;
                    else
                        break;

                    url = string.Format(baseUrl, s, noResults, start.ToString());
                } while (true);
            }

            return articles;
        }

        public string Name
        { get { return "Yahoo Search Plugin"; } }

        public string DisplayText
        { get { return "Yahoo Search"; } }

        public string UserInputTextBoxText
        { get { return "Yahoo Search:"; } }

        public bool UserInputTextBoxEnabled
        { get { return true; } }

        public void Selected() { }

        public bool RunOnSeparateThread
        { get { return true; } }
        #endregion
    }
}
