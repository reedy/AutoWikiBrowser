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

/// MediaWiki API manual: http://www.mediawiki.org/wiki/API
/// Site prerequisites: MediaWiki 1.13+ with the following settings:
/// * $wgEnableAPI = true; (enabled by default in DefaultSettings.php)
/// * $wgEnableWriteAPI = true;
/// * AssertEdit extension installed (http://www.mediawiki.org/wiki/Extension:Assert_Edit)


///////////////////////////////////////////////////////////////////////////////////////////////////////////
//         Currently under rapid development, please don't change it yourself for a while. --MaxSem
///////////////////////////////////////////////////////////////////////////////////////////////////////////

namespace WikiFunctions.API
{
    //TODO: refactor XML parsing
    /// <summary>
    /// This class edits MediaWiki sites using api.php
    /// </summary>
    public class ApiEdit
    {
        private ApiEdit()
        {
        }

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

        /// <summary>
        /// Creates a new instance of the ApiEdit class by cloning the current instance
        /// ATTENTION: the clones will share the same cookie container, so logging off or logging under another username
        /// with one instance will automatically make another one do the same
        /// </summary>
        public ApiEdit Clone()
        {
            ApiEdit clone = new ApiEdit();
            clone.m_URL = m_URL;
            clone.m_Maxlag = m_Maxlag;
            clone.m_Cookies = m_Cookies;
            clone.ProxySettings = ProxySettings;

            return clone;
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

        string m_Action;
        /// <summary>
        /// Action for which we have edit token
        /// </summary>
        public string Action
        { get { return m_Action; } }

        string m_EditToken;
        /// <summary>
        /// Edit token (http://www.mediawiki.org/wiki/Manual:Edit_token)
        /// </summary>
        public string EditToken
        { get { return m_EditToken; } }

        string m_Timestamp;
        public string Timestamp
        { get { return m_Timestamp; } }

        string m_PageTitle;
        /// <summary>
        /// Name of the page currently being edited
        /// </summary>
        public string PageTitle
        { get { return m_PageTitle; } }

        string m_PageText;
        /// <summary>
        /// Initial content of the page currently being edited
        /// </summary>
        public string PageText
        { get { return m_PageText; } }

        CookieContainer m_Cookies = new CookieContainer();
        /// <summary>
        /// Cookies stored between requests
        /// </summary>
        public CookieContainer Cookies
        { get { return m_Cookies; } }

        bool m_Abortable = false;
        /// <summary>
        /// If true, all operations involving network access can be aborted using Abort()
        /// </summary>
        public bool Abortable;
        #endregion

        /// <summary>
        /// Resets all internal variables, discarding edit tokens and so on,
        /// but does not logs off
        /// </summary>
        public void Reset()
        {
            m_Action = null;
            m_EditToken = null;
            m_PageText = null;
            m_PageTitle = null;
            m_Timestamp = null;
        }

        public void Abort()
        {
            throw new NotImplementedException();
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
            if (autoParams) url += "&assert=user&maxlag=" + Maxlag;

            return url;
        }

        protected string BuildUrl(string[,] request)
        {
            return BuildUrl(request, true);
        }
        #endregion

        #region Network access
        static Dictionary<string, IWebProxy> ProxyCache = new Dictionary<string, IWebProxy>();
        IWebProxy ProxySettings;
        static string UserAgent = string.Format("WikiFunctions/{0} ({1})", Assembly.GetExecutingAssembly().GetName().Version.ToString(),
            Environment.OSVersion.VersionString);

        protected HttpWebRequest CreateRequest(string url)
        {
            HttpWebRequest res = (HttpWebRequest)WebRequest.Create(url);
            if (ProxySettings != null) res.Proxy = ProxySettings;
            res.UserAgent = UserAgent;
            res.CookieContainer = m_Cookies;
            res.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

            return res;
        }

        protected string HttpPost(string[,] get, string[,] post, bool autoParams)
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
            return (new StreamReader(resp.GetResponseStream())).ReadToEnd();
        }

        protected string HttpPost(string[,] get, string[,] post)
        {
            return HttpPost(get, post, true);
        }

        protected string HttpGet(string[,] request, bool autoParams)
        {
            string url = BuildUrl(request, autoParams);

            return HttpGet(url);
        }

        protected string HttpGet(string[,] request)
        {
            return HttpGet(request, true);
        }

        /// <summary>
        /// Performs a HTTP request
        /// </summary>
        /// <param name="url"></param>
        /// <returns>Text received</returns>
        public string HttpGet(string url)
        {
            HttpWebRequest req = CreateRequest(url);

            // SECURITY: don't send cookies to third-party sites
            if (!url.StartsWith(URL)) req.CookieContainer = null; 

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
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

            CheckForError(result, "login");

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
            CheckForError(result, "logout");
        }
        #endregion

        #region Page modification

        /// <summary>
        /// Opens a page for editing
        /// </summary>
        /// <param name="title">Title of the page to edit</param>
        /// <returns>Page content</returns>
        public string Open(string title)
        {
            if (string.IsNullOrEmpty(title)) throw new ArgumentException("Page name required", "title");

            Reset();

            // action=query&prop=info|revisions&intoken=edit&titles=Main%20Page&rvprop=timestamp|user|comment|content
            string result = HttpGet(new string[,] { 
                { "action", "query" },
                { "prop", "info|revisions" },
                { "intoken","edit" },
                { "titles", title },
                { "rvprop", "content|timestamp" } // timestamp|user|comment|
            });

            CheckForError(result, "query");

            try
            {
                XmlReader xr = XmlReader.Create(new StringReader(result));
                if (!xr.ReadToFollowing("page")) throw new Exception("Cannot find <page> element");
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

        /// <summary>
        /// Saves the previously opened page
        /// </summary>
        /// <param name="pageText">New page content. Must not be empty.</param>
        /// <param name="summary">Edit summary. Must not be empty.</param>
        /// <param name="minor">Whether the edit should be marked as minor</param>
        /// <param name="watch">Whether the page should be watchlisted</param>
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
                {// order matters here - https://bugzilla.wikimedia.org/show_bug.cgi?id=14210#c4
                    { "md5", MD5(pageText) },
                    { "summary", summary },
                    { "timestamp", m_Timestamp },
                    { "text", pageText },
                    { "token", m_EditToken }
                });

            CheckForError(result, "edit");
            Reset();
        }

        public void Delete(string title, string reason)
        {
            if (string.IsNullOrEmpty(title)) throw new ArgumentException("Page name required", "title");
            if (string.IsNullOrEmpty(reason)) throw new ArgumentException("Deletion reason required", "reason");

            Reset();
            m_Action = "delete";

            string result = HttpGet(
                new string[,]
                {
                    { "action", "query" },
                    { "prop", "info" },
                    { "intoken", "delete" },
                    { "titles", title },

                });

            CheckForError(result);

            try
            {
                XmlReader xr = XmlReader.Create(new StringReader(result));
                if (!xr.ReadToFollowing("page")) throw new Exception("Cannot find <page> element");
                m_EditToken = xr.GetAttribute("deletetoken");
            }
            catch (Exception ex)
            {
                throw new ApiBrokenXmlException(this, ex);
            }

            result = HttpPost(
                new string[,]
                {
                    { "action", "delete" }
                },
                new string[,]
                {
                    { "title", title },
                    { "token", m_EditToken },
                    { "reason", reason }
                });

            CheckForError(result);

            Reset();
        }
        #endregion


        #region Wikitext operations
        public string Preview(string pageTitle, string text)
        {
            string result = HttpPost(
                new string[,]
                {
                    { "action", "parse" },
                    { "prop", "text" }
                },
                new string[,]
                {
                    { "title", pageTitle },
                    { "text", text }
                });

            CheckForError(result, "parse");
            try
            {
                XmlReader xr = XmlReader.Create(new StringReader(result));
                if (!xr.ReadToFollowing("text")) throw new Exception("Cannot find <text> element");
                return xr.ReadString();
            }
            catch (Exception ex)
            {
                throw new ApiBrokenXmlException(this, ex);
            }
        }

        public string ExpandTemplates(string pageTitle, string text)
        {
            string result = HttpPost(
                new string[,]
                {
                    { "action", "expandtemplates" }
                },
                new string[,]
                {
                    { "title", pageTitle },
                    { "text", text }
                });

            CheckForError(result, "expandtemplates");
            try
            {
                XmlReader xr = XmlReader.Create(new StringReader(result));
                if (!xr.ReadToFollowing("expandtemplates")) throw new Exception("Cannot find <expandtemplates> element");
                return xr.ReadString();
            }
            catch (Exception ex)
            {
                throw new ApiBrokenXmlException(this, ex);
            }
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
        /// <param name="action">The action performed, null if don't check</param>
        void CheckForError(string xml, string action)
        {
            if (string.IsNullOrEmpty(xml)) throw new ApiBlankException(this);

            if (!xml.Contains("<error") && string.IsNullOrEmpty(action)) return; 

            XmlReader xr = XmlReader.Create(new StringReader(xml));
            if (xml.Contains("<error") && xr.ReadToFollowing("error"))
            {
                string errorCode = xr.GetAttribute("code");
                string errorMessage = xr.GetAttribute("info");

                switch (errorCode.ToLower())
                {
                    case "maxlag":
                        throw new ApiMaxlagException(this, errorMessage);
                    default:
                        throw new ApiErrorException(this, errorCode, errorMessage);
                }
            }

            if (!string.IsNullOrEmpty(action) && xr.ReadToFollowing(action))
            {
                string result = xr.GetAttribute("result");
                if (result != null && result != "Success")
                {
                    if (action == "edit")
                    {
                        string assert = xr.GetAttribute("assert");
                        if (!string.IsNullOrEmpty(assert))
                        {
                            throw new ApiAssertionException(this, assert);
                        }
                        if (xr.ReadToFollowing("captcha"))
                        {
                            throw new ApiCaptchaException(this);
                        }
                    }
                    else
                        throw new ApiErrorException(this, result, result); //HACK: we need error message
                }
            }
        }

        #endregion

        #region Helpers

        protected string BoolToParam(bool value)
        {
            if (value) return "1";

            return "0";
        }

        /// <summary>
        /// For private use, static to avoid unneeded reinitialisation
        /// </summary>
        private static System.Security.Cryptography.MD5 m_MD5 = System.Security.Cryptography.MD5.Create();

        protected static string MD5(string input)
        {
            return MD5(Encoding.UTF8.GetBytes(input));
        }

        protected static string MD5(byte[] input)
        {
            byte[] hash = m_MD5.ComputeHash(input);

            StringBuilder sb = new StringBuilder(20);
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }

            return sb.ToString();
        }

        #endregion
    }
}
