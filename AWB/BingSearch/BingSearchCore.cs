/*
Bing Search Plugin
Copyright (C) 2009 Sam Reed

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
using System.IO;
using System.Xml;
using WikiFunctions.Plugin;

namespace WikiFunctions.Plugins.ListMaker.BingSearch
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// http://msdn.microsoft.com/en-us/library/dd251056.aspx
    /// http://msdn.microsoft.com/en-us/library/dd251042.aspx - Error Handling
    /// </remarks>
    public class BingSearchListMakerPlugin : IListMakerPlugin
    {
        private const string AppId = "56218EEF0B712AA9E56CEBA0FFE1B79E55E92FFA";

        private const string BaseUrl = "http://api.search.live.net/xml.aspx?Appid=" + AppId + 
            "&query={0}(site:{1})&sources=web&web.count=50&web.offset={2}";

        private const int TotalResults = 500;

        public List<Article> MakeList(params string[] searchCriteria)
        {
            List<Article> articles = new List<Article>();

            foreach (string s in searchCriteria)
            {
                int start = 0;
                do
                {
                    string url = string.Format(BaseUrl, s, Variables.URL, start);

                    using (XmlTextReader reader = new XmlTextReader(new StringReader(Tools.GetHTML(url))))
                    {
                        while (reader.Read())
                        {
                            if (reader.Name.Equals("Error"))
                            {
                                reader.ReadToFollowing("Code");
                                if (string.Compare(reader.ReadString(), "2002", true) == 0)
                                    Tools.MessageBox("Query limit for Bing Exceeded. Please try again later");

                                return articles;
                            }

                            if (reader.Name.Equals("web:Total"))
                            {
                                if (int.Parse(reader.ReadString()) > TotalResults)
                                    start += 50;
                            }

                            if (reader.Name.Equals("web:Url"))
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

        public string DisplayText
        {
            get { return "Bing Search"; }
        }

        public string UserInputTextBoxText
        {
            get { return "Bing Search:"; }
        }

        public bool UserInputTextBoxEnabled
        { get { return true; } }

        public void Selected() { }

        public bool RunOnSeparateThread
        { get { return true; } }

        public string Name
        {
            get { return "Bing Search Plugin"; }
        }
    }
}
