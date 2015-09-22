/*
Copyright (C) 2015 Sam Reed

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
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;
using WikiFunctions.API;

namespace WikiFunctions.Lists.Providers
{
    public abstract class ApiJsonListProviderBase : IListProvider
    {
        protected ApiJsonListProviderBase()
        {
            Limit = 1000;
        }

        /// <summary>
        /// Upper limit for number of pages returned, could be a bit exceeded by number of pages in the last request
        /// </summary>
        public int Limit  { get; set; }

        protected string WantedAttribute = "title";

        /// <summary>
        /// Main function that retrieves the list from API, including paging
        /// </summary>
        /// <param name="url">URL of API request</param>
        /// <param name="haveSoFar">Number of pages already retrieved, for upper limit control</param>
        /// <returns>List of pages</returns>
        public List<Article> ApiMakeList(string url, int haveSoFar)
        {
            if (Globals.UnitTestMode) throw new Exception("You shouldn't access Wikipedia from unit tests");

            ApiEdit editor = Variables.MainForm.TheSession.Editor.SynchronousEditor;

            var json = JObject.Parse(editor.QueryApiJson(url));

            var titles = from t in json["query"]["pageswithprop"] select (string)t["title"];

            return titles.Select(title => new Article(title)).ToList();
        }

        public virtual bool StripUrl
        {
            get { return false; }
        }

        #region To be overridden

        public abstract List<Article> MakeList(params string[] searchCriteria);

        public abstract string DisplayText { get; }

        public abstract string UserInputTextBoxText { get; }

        public abstract bool UserInputTextBoxEnabled { get; }

        public abstract void Selected();

        public virtual bool RunOnSeparateThread
        { get { return true; } }

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Temporary, for debugging
    /// </remarks>
    public class PagesWithPropJsonListProvider : ApiJsonListProviderBase
    {
        #region Tags: <pageswithprop>/<page>
        //protected override ICollection<string> PageElements
        //{
        //    get { return new[] { "page" }; }
        //}

        //protected override ICollection<string> Actions
        //{
        //    get { return new[] { "pageswithprop" }; }
        //}
        #endregion

        public override List<Article> MakeList(params string[] searchCriteria)
        {
            List<Article> list = new List<Article>();

            foreach (string prop in searchCriteria)
            {
                string url = "list=pageswithprop&pwppropname="
                             + HttpUtility.UrlEncode(prop) + "&pwplimit=max";

                list.AddRange(ApiMakeList(url, list.Count));
            }
            return list;
        }

        public List<Article> MakeList(int @namespace, params string[] searchCriteria)
        {
            return MakeList(searchCriteria);
        }

        #region ListMaker properties
        public override string DisplayText
        {
            get { return "(JSON)Pages with a page property"; }
        }

        public override string UserInputTextBoxText
        {
            get { return "Property name:"; }
        }

        public override bool UserInputTextBoxEnabled
        {
            get { return true; }
        }

        public override void Selected() { }
        #endregion
    }
}
