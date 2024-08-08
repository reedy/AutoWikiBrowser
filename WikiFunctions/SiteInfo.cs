﻿/*
Copyright (C) 2009 Max Semenik

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
using System.Xml.Serialization;
using System.Net;
using Newtonsoft.Json.Linq;
using WikiFunctions.API;
using System.Globalization;

namespace WikiFunctions
{
    /// <summary>
    /// This class holds all basic information about a wiki
    /// </summary>
    [Serializable]
    public class SiteInfo : IXmlSerializable
    {
        private readonly IApiEdit Editor;

        // path in format https://en.wikipedia.org/w/
        private string scriptPath;
        private readonly Dictionary<int, string> namespaces = new Dictionary<int, string>();
        private Dictionary<int, List<string>> namespaceAliases = new Dictionary<int, List<string>>();
        // private Dictionary<string, string> messageCache = new Dictionary<string, string>();
        private readonly Dictionary<string, List<string>> magicWords = new Dictionary<string, List<string>>();

        private string siteinfoOutput;

        private readonly Uri uri;
        internal SiteInfo()
        { }

        /// <summary>
        /// Creates an instance of the class
        /// </summary>
        public SiteInfo(IApiEdit editor)
        {
            Editor = editor;
            ScriptPath = editor.URL;
            uri = new Uri(ScriptPath);

            try
            {
                if (!LoadSiteInfo())
                {
                    var ret = ParseErrorFromSiteInfoOutput();
                    if (ret is bool && !(bool) ret)
                    {
                        throw new WikiUrlException();
                    }

                    var ex = ret as Exception;
                    if (ex != null)
                    {
                        throw ex;
                    }
                }
            }
            catch (WikiException)
            {
                throw;
            }
            catch (WebException)
            {
                throw;
            }
            catch (UriChangedException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new WikiUrlException(ex);
            }
        }

        /// <summary>
        /// For object caching support
        /// </summary>
        private static string Key(string scriptPath)
        {
            return "SiteInfo(" + scriptPath + ")@";
        }

        public static SiteInfo CreateOrLoad(IApiEdit editor)
        {
            SiteInfo si = (SiteInfo)ObjectCache.Global.Get<SiteInfo>(Key(editor.URL));
            if (si != null
                && Namespace.VerifyNamespaces(si.Namespaces))
            {
                return si;
            }

            si = new SiteInfo(editor);
            ObjectCache.Global[Key(editor.URL)] = si;

            return si;
        }

        /// <summary>
        /// Gets the API path in format https://en.wikipedia.org/w/api.php or https://en.wikipedia.org/w/api.php5
        /// </summary>
        /// <value>The API path.</value>
        private string ApiPath
        {
            get { return scriptPath + "api.php" + (Editor.PHP5 ? "5" : ""); }
        }

        /// <summary>
        /// Ensures URL ends with /
        /// </summary>
        /// <returns>The updated URL</returns>
        /// <param name="url">URL.</param>
        public static string NormalizeURL(string url)
        {
            return !url.EndsWith("/") ? url + "/" : url;
        }

        /// <summary>
        /// Loads siteinfo XML from Global cache on disk if available
        /// </summary>
        /// <returns><c>true</c>, if loaded from cache successfully, <c>false</c> otherwise.</returns>
        private bool LoadFromCache()
        {
            var cacheResult = ObjectCache.Global.Get<string>("SiteInfo:" + scriptPath);

            // simple (string) cast of line above fails under Mono so do more verbosely
            siteinfoOutput = (cacheResult == null ? "" : cacheResult.ToString());

            return !string.IsNullOrEmpty(siteinfoOutput);
        }

        /// <summary>
        /// Loads siteinfo XML from network via API call
        /// </summary>
        /// <returns><c>true</c>, if loaded from network successfully, <c>false</c> otherwise.</returns>
        private bool LoadFromNetwork()
        {
            siteinfoOutput = Editor.HttpGet(ApiPath + "?action=query&meta=siteinfo&siprop=general|namespaces|namespacealiases|statistics|magicwords&format=xml");

            // readapidenied API error check for private wikis that require login for any query
            if (string.IsNullOrEmpty(siteinfoOutput) || siteinfoOutput.Contains("readapidenied"))
                return false;

            // cache successful result
            ObjectCache.Global.Set("SiteInfo:" + scriptPath, siteinfoOutput);

            return true;
        }

        /// <summary>
        /// Loads SiteInfo from local cache or API call, processes data returned
        /// </summary>
        /// <returns></returns>
        public bool LoadSiteInfo()
        {
            if (!LoadFromCache())
                LoadFromNetwork();

            XmlDocument xd = new XmlDocument();
            xd.LoadXml(siteinfoOutput);

            var api = xd["api"];
            if (api == null) return false;

            var query = api["query"];
            if (query == null) return false;

            var general = query["general"];
            if (general == null) return false;

            ArticleUrl = Host + general.GetAttribute("articlepath");
            Language = general.GetAttribute("lang");
            IsRightToLeft = general.HasAttribute("rtl");
            CapitalizeFirstLetter = general.GetAttribute("case") == "first-letter";
            MediaWikiVersion = general.GetAttribute("generator").Replace("MediaWiki ", "");

            CategoryCollation = general.GetAttribute("categorycollation");

            if (query["namespaces"] == null || query["namespacealiases"] == null)
                return false;

            foreach (XmlNode xn in query["namespaces"].GetElementsByTagName("ns"))
            {
                int id = int.Parse(xn.Attributes["id"].Value, CultureInfo.InvariantCulture);

                if (id != 0) namespaces[id] = xn.InnerText + ":";
            }

            if (!Namespace.VerifyNamespaces(namespaces))
                throw new Exception("Error loading namespaces from " + ApiPath);

            namespaceAliases = Variables.PrepareAliases(namespaces);

            foreach (XmlNode xn in query["namespacealiases"].GetElementsByTagName("ns"))
            {
                int id = int.Parse(xn.Attributes["id"].Value, CultureInfo.InvariantCulture);

                if (id != 0) namespaceAliases[id].Add(xn.InnerText);
            }

            if (query["magicwords"] == null)
                return false;

            foreach (XmlNode xn in query["magicwords"].GetElementsByTagName("magicword"))
            {
                List<string> alias = new List<string>();

                foreach (XmlNode xin in xn["aliases"].GetElementsByTagName("alias"))
                {
                    alias.Add(xin.InnerText);
                }

                magicWords.Add(xn.Attributes["name"].Value, alias);
            }

            LoadAWBTag();

            return true;
        }

        /// <summary>
        /// Looks to see if a tag called AWB has been defined in Special:Tags
        /// </summary>
        private void LoadAWBTag()
        {
            var awbTagDefined = (bool?) ObjectCache.Global.Get<bool>("AWBTagDefined:" + scriptPath);

            // If it's false, we should look again incase it's been defined...
            if (awbTagDefined == null || awbTagDefined == false)
            {
                var obj = JObject.Parse(
                    Editor.HttpGet(ApiPath + "?format=json&action=query&list=tags&tgprop=active&tglimit=max")
                );

                if (obj["error"] != null)
                {
                    // We probably got "code": "readapidenied", due to a "private" wiki
                    //if (obj["error"]["code"].ToString() == "readapidenied")
                    //{
                    //    Variables.TryLoadingAgainAfterLogin ???
                    //    throw new ReadApiDeniedException();
                    //}
                    return;
                }

                awbTagDefined = obj["query"]["tags"].Any(t => (string) t["name"] == "AWB" && t["active"] != null);

                ObjectCache.Global.Set("AWBTagDefined:" + scriptPath, awbTagDefined);
            }

            IsAWBTagDefined = (bool)awbTagDefined;
        }

        public object ParseErrorFromSiteInfoOutput()
        {
            if (string.IsNullOrEmpty(siteinfoOutput))
                return false;

            XmlDocument xd = new XmlDocument();
            xd.LoadXml(siteinfoOutput);

            var api = xd["api"];
            if (api == null) return false;

            var error = api["error"];
            if (error == null) return false;

            var errorCode = error.GetAttribute("code");
            if (!string.IsNullOrEmpty(errorCode))
            {
                switch (errorCode)
                {
                    case "readapidenied":
                        return new ReadApiDeniedException();
                    default:
                        return false;
                }
            }
            return true;
        }

        // [XmlAttribute(AttributeName = "url")]
        public string ScriptPath
        {
            get { return scriptPath; }
            set // Must stay public otherwise Serialiser for ObjectCache isn't happy =(
            {
                scriptPath = NormalizeURL(value);
            }
        }

        public string Host
        {
            get
            {
                return uri.Scheme + Uri.SchemeDelimiter + uri.Host;
            }
        }

        /// <summary>
        /// Contains namespaces for this wiki mapped by their IDs
        /// </summary>
        public Dictionary<int, string> Namespaces
        { get { return namespaces; } }

        /// <summary>
        /// Alternative names of namespaces
        /// </summary>
        public Dictionary<int, List<string>> NamespaceAliases
        { get { return namespaceAliases; } }

        /// <summary>
        /// Magic words used by parser, with alternative variants
        /// </summary>
        public Dictionary<string, List<string>> MagicWords
        { get { return magicWords; } }

        /// <summary>
        /// Prettified URL of pages on server, $1 should be replaced with page title
        /// </summary>
        public string ArticleUrl
        { get; private set; }

        /// <summary>
        /// Version of MediaWiki this site is running on
        /// </summary>
        public string MediaWikiVersion { get; private set; }

        /// <summary>
        /// ISO code of current language
        /// </summary>
        public string Language
        { get; private set; }

        /// <summary>
        /// Is the wiki RTL?
        /// </summary>
        public bool IsRightToLeft
        { get; private set; }

        public bool CapitalizeFirstLetter
        { get; private set; }

        /// <summary>
        /// Category Collation ($wgCategoryCollation) of the wiki
        /// </summary>
        public string CategoryCollation { get; private set; }

        /// <summary>
        /// Returns whether an AWB tag has been defined on Special:Tags
        /// </summary>
        public bool IsAWBTagDefined { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        /// <remarks>Only called if language != en</remarks>
        public Dictionary<string, string> GetMessages(params string[] names)
        {
            var json = JObject.Parse(
                Editor.HttpGet(ApiPath + "?format=json&action=query&meta=allmessages&continue=&ammessages=" +
                    string.Join("|", names)));

            if (json["error"] != null)
            {
                // We probably got "code": "readapidenied", due to a "private" wiki
                //if (obj["error"]["code"].ToString() == "readapidenied")
                //{
                //    Variables.TryLoadingAgainAfterLogin ???
                //    throw new ReadApiDeniedException();
                //}
                // HACK
                return new Dictionary<string, string>();
            }

            return json["query"]["allmessages"].ToDictionary(
                    k => k.Value<string>("name"),
                    v => v.Value<string>("*"));
        }

        #region Helpers
        public void OpenPageInBrowser(string title)
        {
            Tools.OpenArticleInBrowser(title);
        }

        public void OpenPageHistoryInBrowser(string title)
        {
            Tools.OpenArticleHistoryInBrowser(title);
        }

        #endregion

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void WriteXml(XmlWriter writer)
        {
            // writer.WriteStartElement("site");
            writer.WriteAttributeString("url", scriptPath);
            writer.WriteAttributeString("php5", Editor.PHP5 ? "1" : "0");
            {
                writer.WriteStartElement("Namespaces");
                {
                    foreach (KeyValuePair<int, string> p in namespaces)
                    {
                        writer.WriteStartElement("Namespace");
                        writer.WriteAttributeString("id", p.Key.ToString());
                        writer.WriteValue(p.Value);
                        writer.WriteEndElement();
                    }
                }
            }
            // writer.WriteEndElement();
        }
        #endregion
    }

    public abstract class WikiException : Exception
    {
        protected WikiException(string text)
            : base(text)
        { }

        protected WikiException(string text, Exception innerException)
            : base(text, innerException)
        { }
    }

    public class WikiUrlException : WikiException
    {
        private const string ExceptionMessage = "Can't connect to given wiki site.";
        public WikiUrlException()
            : base(ExceptionMessage)
        { }

        public WikiUrlException(Exception innerException)
            : base(ExceptionMessage, innerException)
        { }
    }

    public class ReadApiDeniedException : WikiException
    {
        private const string ExceptionMessage = "You need read permission to use this module";

        public ReadApiDeniedException()
            : base(ExceptionMessage)
        { }

        public ReadApiDeniedException(Exception innerException)
            : base(ExceptionMessage, innerException)
        { }
    }
}
