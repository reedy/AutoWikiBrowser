/*
Copyright (C) 2008 Max Semenik

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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using WikiFunctions;
using System.Net;
using System.Reflection;
using System.Web;
using System.IO;
using System.Xml;

/// Site prerequisites: MediaWiki 1.13+ with the following settings:
/// * $wgEnableAPI = true; (enabled by default in DefaultSettings.php)
/// * $wgEnableWriteAPI = true;
/// * AssertEdit extension installed (http://www.mediawiki.org/wiki/Extension:Assert_Edit)


///////////////////////////////////////////////////////////////////////////////////////////////////////////
//         Currently under rapid development, please don't change it yourself for a while. --MaxSem
///////////////////////////////////////////////////////////////////////////////////////////////////////////

namespace WikiFunctions.API
{
    /// <summary>
    /// This class edits MediaWiki sites using api.php
    /// </summary>
    public class ApiEdit
    {
        /// <summary>
        /// Creates a new instance of the ApiEdit class
        /// </summary>
        /// <param name="url">Path to scripts on server</param>
        public ApiEdit(string url)
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentException("Invalid URL specified", "url");
            if (!url.StartsWith("http://")) throw new NotSupportedException("Only editing via HTTP is currently supported");

            m_URL = url;

            if (ProxyCache.ContainsKey(url))
            {
                ProxySettings = ProxyCache[url];
            }
            else
            {
                ProxySettings = WebRequest.GetSystemWebProxy();
                if (ProxySettings.IsBypassed(new Uri(url)))
                {
                    ProxySettings = null;
                }
                ProxyCache.Add(url, ProxySettings);
            }
        }

        #region Properties
        string m_URL;
        /// <summary>
        /// Path to scripts on server
        /// </summary>
        public string URL
        { get { return m_URL; } }

        int m_Maxlag = 5;
        /// <summary>
        /// Maxlag parameter of every request (http://www.mediawiki.org/wiki/Manual:Maxlag_parameter)
        /// </summary>
        public int Maxlag
        {
            get { return m_Maxlag; }
            set { m_Maxlag = value; }
        }

        string m_Action = "";
        /// <summary>
        /// Action for which we have edit token
        /// </summary>
        public string Action
        { get { return m_Action; } }

        string m_EditToken = "";
        /// <summary>
        /// Edit token (http://www.mediawiki.org/wiki/Manual:Edit_token)
        /// </summary>
        public string EditToken
        { get { return m_EditToken; } }

        string m_Timestamp = "";
        public string Timestamp
        { get { return m_Timestamp; } }

        string m_PageTitle = "";
        /// <summary>
        /// Name of the page currently being edited
        /// </summary>
        public string PageTitle
        { get { return m_PageTitle; } }

        string m_PageText = "";
        /// <summary>
        /// Initial content of the page currently being edited
        /// </summary>
        public string PageText
        { get { return m_PageText; } }
        
        CookieCollection m_Cookies = new CookieCollection();
        /// <summary>
        /// Cookies stored between requests
        /// </summary>
        public CookieCollection Cookies
        { get { return m_Cookies; } }
        #endregion

        /// <summary>
        /// Resets all internal variables, discarding edit tokens, etc
        /// </summary>
        public void Reset()
        {
            m_Action = null;
            m_EditToken = null;
            m_PageText = null;
            m_PageTitle = null;
            m_Timestamp = null;
        }

        #region URL stuff
        protected static string BuildQuery(string[,] request)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i <= request.GetUpperBound(0); i++)
            {
                string s = request[i, 0];
                if (string.IsNullOrEmpty(s)) continue;
                sb.Append('&');
                sb.Append(s);

                s = request[i, 1];
                if (s != null) // empty string is a valid parameter value!
                {
                    sb.Append('=');
                    sb.Append(HttpUtility.UrlEncode(s));
                }
            }

            return sb.ToString();
        }

        protected static string Titles(params string[] titles)
        {
            for (int i = 0; i < titles.Length; i++) titles[i] = Tools.WikiEncode(titles[i]);
            if (titles.Length > 0) return "&titles=" + string.Join("|", titles);
            else return "";
        }

        protected static string NamedTitles(string paramName, params string[] titles)
        {
            for (int i = 0; i < titles.Length; i++) titles[i] = Tools.WikiEncode(titles[i]);
            if (titles.Length > 0) return "&" + paramName+ "=" + string.Join("|", titles);
            else return "";
        }

        protected string BuildUrl(string[,] request, bool autoParams)
        {
            string url = URL + "api.php?format=xml" + BuildQuery(request);
            if (autoParams) url += "&assert=false&maxlag=" + Maxlag;

            return url;
        }

        protected string BuildUrl(string[,] request)
        {
            return BuildUrl(request, true);
        }
        #endregion

        #region Network access
        Dictionary<string, IWebProxy> ProxyCache = new Dictionary<string, IWebProxy>();
        IWebProxy ProxySettings;
        static string UserAgent = string.Format("WikiFunctions/{0} ({1})", Assembly.GetExecutingAssembly().GetName().Version.ToString(),
            Environment.OSVersion.VersionString);

        protected HttpWebRequest CreateRequest(string url)
        {
            HttpWebRequest res = (HttpWebRequest)WebRequest.Create(url);
            if (ProxySettings != null) res.Proxy = ProxySettings;
            res.UserAgent = UserAgent;
            res.CookieContainer = new CookieContainer();
            res.CookieContainer.Add(Cookies);
            res.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

            return res;
        }

        protected void RecycleRequest(HttpWebRequest req)
        {
            if (!req.HaveResponse || !req.RequestUri.ToString().StartsWith(URL)) return; // foolproof

            m_Cookies = req.CookieContainer.GetCookies(req.RequestUri);
        }

        public string HttpPost(string[,] get, string[,] post, bool autoParams)
        {
            string url = BuildUrl(get, autoParams);

            string query = BuildQuery(post);
            byte[] postData = Encoding.UTF8.GetBytes(query);

            HttpWebRequest req = CreateRequest(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = postData.Length;
            using (Stream rs = req.GetRequestStream())
            {
                rs.Write(postData, 0, postData.Length);
            }

            WebResponse resp = req.GetResponse();
            RecycleRequest(req);
            return (new StreamReader(resp.GetResponseStream())).ReadToEnd();
        }

        public string HttpPost(string[,] get, string[,] post)
        {
            return HttpPost(get, post, true);
        }

        public string HttpGet(string[,] request, bool autoParams)
        {
            string url = BuildUrl(request, autoParams);

            return HttpGet(url);
        }

        public string HttpGet(string[,] request)
        {
            return HttpGet(request, true);
        }

        public string HttpGet(string url)
        {
            HttpWebRequest req = CreateRequest(url);

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            RecycleRequest(req);
            using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
            {
                return sr.ReadToEnd();
            }
        }
        #endregion

        #region Login
        public void Login(string username, string password)
        {
            Reset();

            string result = HttpPost(new string[,] { { "action", "login" } },
                new string[,] { { "lgname", username }, { "lgpassword", password } }, false);

            CheckForError(result);

            XmlReader xr = XmlReader.Create(new StringReader(result));
            xr.ReadToFollowing("login");
            string status = xr.GetAttribute("result");
            if (!status.Equals("Success", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ApiLoginException(this, status);
            }
        }

        public void Logout()
        {
            Reset();
            string result = HttpGet(new string[,] { { "action", "logout" } }, false);
        }
        #endregion

        #region Page modification
        public string Open(string pageName)
        {
            if (string.IsNullOrEmpty(pageName)) throw new ArgumentException("Page name required", "pageName");

            Reset();

            // action=query&prop=info|revisions&intoken=edit&titles=Main%20Page&rvprop=timestamp|user|comment|content
            string result = HttpGet(new string[,] { 
                { "action", "query" },
                { "prop", "info|revisions" },
                { "intoken","edit" },
                { "titles", pageName },
                { "rvprop", "content|timestamp" } // timestamp|user|comment|
            });

            CheckForError(result, "query");

            try
            {
                XmlReader xr = XmlReader.Create(new StringReader(result));
                xr.ReadToFollowing("page");
                m_EditToken = xr.GetAttribute("edittoken");
                m_PageTitle = xr.GetAttribute("title");

                xr.ReadToDescendant("revisions");
                xr.ReadToDescendant("rev");
                m_Timestamp = xr.GetAttribute("timestamp");
                m_PageText = xr.ReadString();

                m_Action = "edit";
            }
            catch (Exception ex)
            {
                throw new ApiBrokenXmlException(this, ex);
            }

            return m_PageText;
        }

        public void Save(string pageText, string summary, bool minor, bool watch)
        {
            if (string.IsNullOrEmpty(pageText)) throw new ArgumentException("Can't save empty pages", "pageText");
            if (string.IsNullOrEmpty(summary)) throw new ArgumentException("Edit summary required", "summary");
            if (m_Action != "edit") throw new ApiException(this, "This page is not opened properly for editing");
            if (string.IsNullOrEmpty(m_EditToken)) throw new ApiException(this, "Edit token is needed to edit pages");

            string result = HttpPost(
                new string[,]
                {
                    { "action", "edit" },
                    { "title", m_PageTitle },
                    { minor ? "minor" : null, null },
                    { watch ? "watch" : null, null }
                },
                new string[,]
                {
                    { "token", m_EditToken },
                    { "text", pageText },
                    { "summary", summary },
                    { "timestamp", m_Timestamp }
                });

            CheckForError(result);
        }
        #endregion


        #region Error handling

        /// <summary>
        /// Checks the XML returned by the server for error codes and throws an appropriate exception
        /// </summary>
        /// <param name="xml">Server output</param>
        void CheckForError(string xml)
        {
            CheckForError(xml, null);
        }

        /// <summary>
        /// Checks the XML returned by the server for error codes and throws an appropriate exception
        /// </summary>
        /// <param name="xml">Server output</param>
        /// <param name="action">The action performed</param>
        void CheckForError(string xml, string action)
        {
            if (!xml.Contains("<error") && string.IsNullOrEmpty(action)) return; 

            XmlReader xr = XmlReader.Create(new StringReader(xml));
            if (xml.Contains("<error") && xr.ReadToFollowing("error"))
            {
                throw new ApiErrorException(this, xr.GetAttribute("code"), xr.GetAttribute("info"));
            }

            if (!string.IsNullOrEmpty(action) && xr.ReadToFollowing(action))
            {
                string result = xr.GetAttribute("result");
            }
        }

        #endregion

        #region Helpers

        protected string BoolToParam(bool value)
        {
            if (value) return "1";

            return "0";
        }

        #endregion
    }
}
