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

using System;
using System.Collections.Generic;
using System.Net;
using WikiFunctions.Plugin;

namespace WikiFunctions.Plugins.ListMaker.BingSearch
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// https://datamarket.azure.com/dataset/5ba839f1-12ce-4cce-bf57-a49d98d29a44
    /// 
    /// https://datamarket.azure.com/dataset/explore/getproxy/5ba839f1-12ce-4cce-bf57-a49d98d29a44
    /// http://go.microsoft.com/fwlink/?LinkID=248077
    /// http://go.microsoft.com/fwlink/?LinkID=252146
    /// http://go.microsoft.com/fwlink/?LinkID=252151
    /// </remarks>
    public class BingSearchListMakerPlugin : IListMakerPlugin
    {
        private const string AccountKey = "A3wH+TXilV1e0G5T7RF3XuznfxoOMkcgpo3pxKo49xY=";

        public List<Article> MakeList(params string[] searchCriteria)
        {
            List<Article> articles = new List<Article>();
            var bingContainer = new Bing.BingSearchContainer(new Uri("https://api.datamarket.azure.com/Bing/Search/"))
                {Credentials = new NetworkCredential(AccountKey, AccountKey)};

            foreach (string s in searchCriteria)
            {
                var searchQuery = bingContainer.Web(string.Format("{0}({1})", s, Variables.URL), null, null, null, null,
                                                    null, null, null);
                var searchResults = searchQuery.Execute();
                if (searchResults == null)
                {
                    continue;
                }
                foreach (var result in searchResults)
                {
                    articles.Add(new Article(result.Title));
                    Console.WriteLine(result.Title);
                }
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

        public virtual bool StripUrl
        { get { return false; } }
    }
}
