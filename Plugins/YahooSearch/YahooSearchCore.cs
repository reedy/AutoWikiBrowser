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

using System.Collections.Generic;
using System.Xml;
using System.IO;
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
        private const string AppId = "3mG9u3PV34GC4rnRXJlID0_3aUb0.XVxGZYrbFcYClzQYUqtlkn0u6iXVwYVv9sW1Q--";
        #region IListMakerPlugin Members

        private const string BaseUrl =
            "http://search.yahooapis.com/WebSearchService/V1/webSearch?appid=" + AppId +
            "&query={0}&results={2}&site={1}&start={3}";

        private const int TotalResults = 500,
            NoOfResultsPerRequest = 100;

        public List<Article> MakeList(string[] searchCriteria)
        {
            List<Article> articles = new List<Article>();
            int start = 0;

            foreach (string s in searchCriteria)
            {
                do
                {
                    string url = string.Format(BaseUrl, s, Variables.URL, NoOfResultsPerRequest, start);

                    using (XmlTextReader reader = new XmlTextReader(new StringReader(Tools.GetHTML(url))))
                    {
                        while (reader.Read())
                        {
                            if (reader.Name.Equals("Message"))
                            {
                                if (string.Compare(reader.ToString(), "limit exceeded", true) == 0)
                                    Tools.MessageBox("Query limit for Yahoo Exceeded. Please try again later");
                                return articles;
                            }

                            if (reader.Name.Equals("ResultSet"))
                            {
                                reader.MoveToAttribute("totalResultsAvailable");

                                if (!string.IsNullOrEmpty(reader.Value) && int.Parse(reader.Value) > TotalResults)
                                    start += NoOfResultsPerRequest;
                            }

                            if (reader.Name.Equals("ClickUrl"))
                            {
                                string title = Tools.GetTitleFromURL(reader.ReadString());

                                if (!string.IsNullOrEmpty(title))
                                    articles.Add(new Article(title));
                            }
                        }
                    }
                } while (articles.Count < TotalResults);
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
