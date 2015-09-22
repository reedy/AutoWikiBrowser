/*
Copyright (C) 2008 Max Semenik, Sam Reed

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
using System.Web;
using System.Xml;
using System.IO;
using WikiFunctions.API;

namespace WikiFunctions.Lists.Providers
{
    /// <summary>
    /// Parent abstract class for all API-based providers
    /// currently simultaneous call of more than one API generator is not fully supported
    /// </summary>
    public abstract class ApiListProviderBase : IListProvider
    {
        #region Internals
        protected ApiListProviderBase()
        {
            Limit = 25000;
        }

        #endregion

        /// <summary>
        /// Gets the list of XML elements that represent pages,
        /// e.g. &lt;p>, &lt;cm>, &lt;bl> etc
        /// </summary>
        protected abstract ICollection<string> PageElements { get; }

        /// <summary>
        /// 
        /// </summary>
        protected abstract ICollection<string> Actions { get; }

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
            
            // TODO: error handling
            List<Article> list = new List<Article>();
            string postfix = "";

            string newUrl = url;

            ApiEdit editor = Variables.MainForm.TheSession.Editor.SynchronousEditor;

            while (list.Count + haveSoFar < Limit)
            {
                // API continuation needs updating https://phabricator.wikimedia.org/T104684
                string text = editor.QueryApi(newUrl + "&rawcontinue=1" + postfix); // HACK: Hacky hack hack

                XmlTextReader xml = new XmlTextReader(new StringReader(text));
                xml.MoveToContent();
                postfix = "";

                while (xml.Read())
                {
                    if (xml.Name == "query-continue")
                    {
                        XmlReader r = xml.ReadSubtree();

                        r.Read();

                        while (r.Read())
                        {
                            if (!r.IsStartElement()) continue;
                            if (!r.MoveToFirstAttribute())
                                throw new FormatException("Malformed element '" + r.Name + "' in <query-continue>");
                            postfix += "&" + r.Name + "=" + HttpUtility.UrlEncode(r.Value);
                        }
                    }
                    else if (PageElements.Contains(xml.Name) && xml.IsStartElement())
                    {
                        if (!EvaluateXmlElement(xml))
                            continue;

                        int ns;
                        int.TryParse(xml.GetAttribute("ns"), out ns);
                        string name = xml.GetAttribute(WantedAttribute);

                        if (string.IsNullOrEmpty(name))
                        {
                            System.Windows.Forms.MessageBox.Show(xml.ReadInnerXml());
                            break;
                        }

                        list.Add(ns >= 0 ? new Article(name, ns) : new Article(name));
                    }
                }
                if (string.IsNullOrEmpty(postfix)) break;
            }

            return list;
        }

        /// <summary>
        /// Allows for customised evaluation of the Xml element, as it is ok to add this element to the article list
        /// </summary>
        /// <param name="xml">XmlTextReader at which the current element is to be evaluated</param>
        /// <returns>Whether this element can be added</returns>
        protected virtual bool EvaluateXmlElement(XmlTextReader xml)
        {
            return true;
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
}
