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
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Reflection;
using System.Web;
using System.IO;
using System.Xml;
using System.Threading;
using System.Text.RegularExpressions;

/// MediaWiki API manual: http://www.mediawiki.org/wiki/API
/// Site prerequisites: MediaWiki 1.13+ with the following settings:
/// * $wgEnableAPI = true; (enabled by default in DefaultSettings.php)
/// * $wgEnableWriteAPI = true;
/// * AssertEdit extension installed (http://www.mediawiki.org/wiki/Extension:Assert_Edit)

namespace WikiFunctions.API
{
    //TODO: refactor XML parsing
    /// <summary>
    /// This class edits MediaWiki sites using api.php
    /// </summary>
    public class ApiEdit : IApiEdit
    {
        private ApiEdit()
        {
        }

        /// <summary>
        /// Creates a new instance of the ApiEdit class
        /// </summary>
        /// <param name="url">Path to scripts on server</param>
        public ApiEdit(string url)
            : this(url, false)
        {
        }

        /// <summary>
        /// Creates a new instance of the ApiEdit class
        /// </summary>
        /// <param name="url">Path to scripts on server</param>
        /// <param name="usePHP5"></param>
        public ApiEdit(string url, bool usePHP5)
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentException("Invalid URL specified", "url");
            if (!url.StartsWith("http://")) throw new NotSupportedException("Only editing via HTTP is currently supported");

            URL = url;
            PHP5 = usePHP5;
            Maxlag = 5;

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
            clone.URL = URL;
            clone.PHP5 = PHP5;
            clone.Maxlag = Maxlag;
            clone.m_Cookies = m_Cookies;
            clone.ProxySettings = ProxySettings;
            clone.m_UserInfo = m_UserInfo;

            return clone;
        }

        #region Properties

        /// <summary>
        /// Path to scripts on server
        /// </summary>
        public string URL { get; private set; }

        private string Server
        {
            get
            { 
                Uri uri = new Uri(URL);
                return "http://" + uri.Host;
            }
        }

        public bool PHP5 { get; private set; }

        /// <summary>
        /// Maxlag parameter of every request (http://www.mediawiki.org/wiki/Manual:Maxlag_parameter)
        /// </summary>
        public int Maxlag
        { get; set; }

        /// <summary>
        /// Action for which we have edit token
        /// </summary>
        public string Action { get; private set; }

        /// <summary>
        /// Name of the page currently being edited
        /// </summary>
        public PageInfo Page
        { get; private set; }

        public string HtmlHeaders
        { get; private set; }

        CookieContainer m_Cookies = new CookieContainer();
        /// <summary>
        /// Cookies stored between requests
        /// </summary>
        public CookieContainer Cookies
        { get { return m_Cookies; } }
        #endregion

        /// <summary>
        /// Resets all internal variables, discarding edit tokens and so on,
        /// but does not logs off
        /// </summary>
        public void Reset()
        {
            Action = null;
            Page = new PageInfo();
            m_Aborting = false;
            m_Request = null;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Abort()
        {
            m_Aborting = true;
            m_Request.Abort();
            Thread.Sleep(1);
            m_Aborting = false;
        }

        #region URL stuff
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="titles"></param>
        /// <returns></returns>
        protected static string Titles(params string[] titles)
        {
            for (int i = 0; i < titles.Length; i++) titles[i] = Tools.WikiEncode(titles[i]);
            if (titles.Length > 0) return "&titles=" + string.Join("|", titles);

            return "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="titles"></param>
        /// <returns></returns>
        protected static string NamedTitles(string paramName, params string[] titles)
        {
            for (int i = 0; i < titles.Length; i++) titles[i] = Tools.WikiEncode(titles[i]);
            if (titles.Length > 0) return "&" + paramName + "=" + string.Join("|", titles);
            return "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="autoParams"></param>
        /// <returns></returns>
        protected string BuildUrl(string[,] request, bool autoParams)
        {
            string url = URL + "api.php" + (PHP5 ? "5" : "") + "?format=xml" + BuildQuery(request);
            if (autoParams) url += "&assert=user&maxlag=" + Maxlag;

            return url;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected string BuildUrl(string[,] request)
        {
            return BuildUrl(request, true);
        }
        #endregion

        #region Network access
        static readonly Dictionary<string, IWebProxy> ProxyCache = new Dictionary<string, IWebProxy>();
        IWebProxy ProxySettings;
        static readonly string UserAgent = string.Format("WikiFunctions/{0} ({1})", Assembly.GetExecutingAssembly().GetName().Version,
            Environment.OSVersion.VersionString);

        protected HttpWebRequest CreateRequest(string url)
        {
            if (Globals.UnitTestMode) throw new Exception("You shouldn't access Wikipedia from unit tests");

            ServicePointManager.Expect100Continue = false;
            HttpWebRequest res = (HttpWebRequest)WebRequest.Create(url);
            res.ServicePoint.Expect100Continue = false;
            res.Expect = "";
            if (ProxySettings != null) res.Proxy = ProxySettings;
            res.UserAgent = UserAgent;
            res.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

            // SECURITY: don't send cookies to third-party sites
            if (url.StartsWith(URL)) res.CookieContainer = m_Cookies;

            return res;
        }

        bool m_Aborting;
        HttpWebRequest m_Request;

        protected string GetResponseString(HttpWebRequest req)
        {
            m_Request = req;

            try
            {
                using (WebResponse resp = req.GetResponse())
                {
                    using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
            catch (WebException ex)
            {
                // just reclassifying
                if (ex.Status == WebExceptionStatus.RequestCanceled) throw new ApiAbortedException(this);
                else throw;
            }
            finally
            {
                m_Request = null;
            }
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
            return GetResponseString(req);
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
            return GetResponseString(CreateRequest(url));
        }
        #endregion

        #region Login / user props
        public void Login(string username, string password)
        {
            Reset();

            string result = HttpPost(new[,] { {"action", "login"} },
                                     new[,] { 
                                        { "lgname", username }, 
                                        { "lgpassword", password }
                                     },
                                     false);

            XmlReader xr = XmlReader.Create(new StringReader(result));
            xr.ReadToFollowing("login");
            string status = xr.GetAttribute("result");
            if (!status.Equals("Success", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ApiLoginException(this, status);
            }

            CheckForError(result, "login");

            RefreshUserInfo();
        }

        public void Logout()
        {
            Reset();
            m_UserInfo = new UserInfo();
            string result = HttpGet(new[,] { { "action", "logout" } }, false);
            CheckForError(result, "logout");
        }

        private UserInfo m_UserInfo = new UserInfo();

        public UserInfo User
        {
            get { return m_UserInfo; }
        }

        public void RefreshUserInfo()
        {
            Reset();
            m_UserInfo = new UserInfo();

            string result = HttpPost(new[,] { { "action", "query" } },
                         new[,] {
                            { "meta", "userinfo" },
                            { "uiprop", "blockinfo|hasmsg|groups|rights" }
                         });

            CheckForError(result, "userinfo");

            m_UserInfo = new UserInfo(result);
        }

        #endregion

        #region Page modification

        public string Open(string title)
        {
            if (string.IsNullOrEmpty(title)) throw new ArgumentException("Page name required", "title");

            Reset();

            // action=query&prop=info|revisions&intoken=edit&titles=Main%20Page&rvprop=timestamp|user|comment|content
            string result = HttpGet(new[,] { 
                { "action", "query" },
                { "prop", "info|revisions" },
                { "intoken","edit" },
                { "titles", title },
                { "inprop", "protection" },
                { "rvprop", "content|timestamp" } // timestamp|user|comment|
            });

            CheckForError(result, "query");

            try
            {
                Page = new PageInfo(result);

                Action = "edit";
            }
            catch (Exception ex)
            {
                throw new ApiBrokenXmlException(this, ex);
            }

            return Page.Text;
        }

        public void Save(string pageText, string summary, bool minor, bool watch)
        {
            if (string.IsNullOrEmpty(pageText)) throw new ArgumentException("Can't save empty pages", "pageText");
            if (string.IsNullOrEmpty(summary)) throw new ArgumentException("Edit summary required", "summary");
            if (Action != "edit") throw new ApiException(this, "This page is not opened properly for editing");
            if (string.IsNullOrEmpty(Page.EditToken)) throw new ApiException(this, "Edit token is needed to edit pages");

            string result = HttpPost(
                new[,]
                {
                    { "action", "edit" },
                    { "title", Page.Title },
                    { minor ? "minor" : null, null },
                    { watch ? "watch" : null, null }
                },
                new[,]
                {// order matters here - https://bugzilla.wikimedia.org/show_bug.cgi?id=14210#c4
                    { "md5", MD5(pageText) },
                    { "summary", summary },
                    { "timestamp", Page.Timestamp },
                    { "text", pageText },
                    { "token", Page.EditToken }
                });

            CheckForError(result, "edit");
            Reset();
        }

        public void Delete(string title, string reason)
        {
            Delete(title, reason, false);
        }

        public void Delete(string title, string reason, bool watch)
        {
            if (string.IsNullOrEmpty(title)) throw new ArgumentException("Page name required", "title");
            if (string.IsNullOrEmpty(reason)) throw new ArgumentException("Deletion reason required", "reason");

            Reset();
            Action = "delete";

            string result = HttpGet(
                new[,]
                    {
                        {"action", "query"},
                        {"prop", "info"},
                        {"intoken", "delete"},
                        {"titles", title},
                        {watch ? "watch" : null, null}

                    });

            CheckForError(result);

            try
            {
                XmlReader xr = XmlReader.Create(new StringReader(result));
                if (!xr.ReadToFollowing("page")) throw new Exception("Cannot find <page> element");
                Page.EditToken = xr.GetAttribute("deletetoken");
            }
            catch (Exception ex)
            {
                throw new ApiBrokenXmlException(this, ex);
            }

            if (m_Aborting) throw new ApiAbortedException(this);

            result = HttpPost(
                new[,]
                {
                    { "action", "delete" }
                },
                new[,]
                {
                    { "title", title },
                    { "token", Page.EditToken },
                    { "reason", reason }
                });

            CheckForError(result);

            Reset();
        }

        public void Protect(string title, string reason, TimeSpan expiry, Protection edit, Protection move)
        {
            Protect(title, reason, expiry.ToString(), edit, move, false, false);
        }

        public void Protect(string title, string reason, string expiry, Protection edit, Protection move)
        {
            Protect(title, reason, expiry, edit, move, false, false);
        }

        public void Protect(string title, string reason, TimeSpan expiry, Protection edit, Protection move, bool cascade, bool watch)
        {
            Protect(title, reason, expiry.ToString(), edit, move, cascade, watch);
        }

        public void Protect(string title, string reason, string expiry, Protection edit, Protection move, bool cascade, bool watch)
        {
            if (string.IsNullOrEmpty(title)) throw new ArgumentException("Page name required", "title");
            if (string.IsNullOrEmpty(reason)) throw new ArgumentException("Deletion reason required", "reason");

            Reset();
            Action = "protect";

            string result = HttpGet(
                new[,]
                    {
                        {"action", "query"},
                        {"prop", "info"},
                        {"intoken", "protect"},
                        {"titles", title},

                    });

            CheckForError(result);

            try
            {
                XmlReader xr = XmlReader.Create(new StringReader(result));
                if (!xr.ReadToFollowing("page")) throw new Exception("Cannot find <page> element");
                Page.EditToken = xr.GetAttribute("protecttoken");
            }
            catch (Exception ex)
            {
                throw new ApiBrokenXmlException(this, ex);
            }

            if (m_Aborting) throw new ApiAbortedException(this);
            
            result = HttpPost(
                new[,]
                    {
                        {"action", "protect"}
                    },
                new[,]
                    {
                        {"title", title},
                        {"token", Page.EditToken},
                        {"reason", reason},
                        {"protections", "edit" + edit + "|move=" + move},
                        {"expiry", expiry},
                        {cascade ? "cascade" : null, null},
                        {watch ? "watch" : null, null},
                    });

            CheckForError(result);

            Reset();
        }

        public void MovePage(string title, string newTitle, string reason, bool moveTalk, bool noRedirect)
        {
            MovePage(title, newTitle, reason, moveTalk, noRedirect, false);
        }

        public void MovePage(string title, string newTitle, string reason, bool moveTalk, bool noRedirect, bool watch)
        {
            if (string.IsNullOrEmpty(title)) throw new ArgumentException("Page title required", "title");
            if (string.IsNullOrEmpty(newTitle)) throw new ArgumentException("Target page title required", "newTitle");
            if (string.IsNullOrEmpty(reason)) throw new ArgumentException("Page rename reason required");

            Reset();
            Action = "move";

            string result = HttpGet(
                new[,]
                    {
                        {"action", "query"},
                        {"prop", "info"},
                        {"intoken", "move"},
                        {"titles", title + "|" + newTitle},

                    });

            CheckForError(result);

            try
            {
                XmlReader xr = XmlReader.Create(new StringReader(result));
                if (!xr.ReadToFollowing("page")) throw new Exception("Cannot find <page> element");
                Page.EditToken = xr.GetAttribute("movetoken");
            }
            catch (Exception ex)
            {
                throw new ApiBrokenXmlException(this, ex);
            }

            if (m_Aborting) throw new ApiAbortedException(this);
            
            result = HttpPost(
                new[,]
                    {
                        {"action", "move"}
                    },
                new[,]
                    {
                        {"from", title},
                        {"to", newTitle},
                        {"token", Page.EditToken},
                        {"reason", reason},
                        {"protections", ""},
                        {moveTalk ? "movetalk" : null, null},
                        {noRedirect ? "noredirect" : null, null},
                        {watch ? "watch" : null, null},
                    },
                true);

            CheckForError(result);

            Reset();
        }

        #endregion

        #region Wikitext operations

        private string ExpandRelativeUrls(string html)
        {
            return html.Replace(" href=\"/", " href=\"" + Server + "/")
                .Replace(" src=\"/", " src=\"" + Server + "/");
        }

        private static readonly Regex extractCssAndJs = new Regex(@"("
            + @"<!--\[if .*?-->"
            + @"|<style\b.*?>.*?</style>"
            + @"|<link rel=""stylesheet"".*?/\s?>"
            //+ @"|<script type=""text/javascript"".*?</script>"
            + ")",
            RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Loads wiki's UI HTML and scraps everything we need to make correct previews
        /// </summary>
        private void EnsureHtmlHeadersLoaded()
        {
            if (!string.IsNullOrEmpty(HtmlHeaders)) return;

            string html = HttpGet(URL + "index.php" + (PHP5 ? "5" : ""));
            html = Tools.StringBetween(html, "<head>", "</head>");
            StringBuilder extracted = new StringBuilder(2048);

            foreach (Match m in extractCssAndJs.Matches(html))
            {
                extracted.Append(m.Value);
                extracted.Append("\n");
            }

            //string server = "http://" + new Uri(URL).Host;

            HtmlHeaders = ExpandRelativeUrls(extracted.ToString());
        }

        public string Preview(string pageTitle, string text)
        {
            EnsureHtmlHeadersLoaded();

            string result = HttpPost(
                new[,]
                {
                    { "action", "parse" },
                    { "prop", "text" }
                },
                new[,]
                {
                    { "title", pageTitle },
                    { "text", text }
                });

            CheckForError(result, "parse");
            try
            {
                XmlReader xr = XmlReader.Create(new StringReader(result));
                if (!xr.ReadToFollowing("text")) throw new Exception("Cannot find <text> element");
                return ExpandRelativeUrls(xr.ReadString());
            }
            catch (Exception ex)
            {
                throw new ApiBrokenXmlException(this, ex);
            }
        }

        public string ExpandTemplates(string pageTitle, string text)
        {
            string result = HttpPost(
                new[,]
                {
                    { "action", "expandtemplates" }
                },
                new[,]
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
        private void CheckForError(string xml)
        {
            CheckForError(xml, null);
        }

        /// <summary>
        /// Checks the XML returned by the server for error codes and throws an appropriate exception
        /// </summary>
        /// <param name="xml">Server output</param>
        /// <param name="action">The action performed, null if don't check</param>
        private void CheckForError(string xml, string action)
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
                //string result = xr.GetAttribute("result");
                //if (result != null && result == "Success")
                //{
                string s = xr.GetAttribute("assert");
                if (!string.IsNullOrEmpty(s))
                {
                    throw new ApiAssertionException(this, s);
                }

                s = xr.GetAttribute("spamblacklist");
                if (!string.IsNullOrEmpty(s))
                {
                    throw new ApiSpamlistException(this, s);
                }
                //}
                //else
                //throw new ApiErrorException(this, result, result); //HACK: we need error message
            }

            if (xr.ReadToFollowing("captcha"))
            {
                throw new ApiCaptchaException(this);
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected static string BoolToParam(bool value)
        {
            return value ? "1" : "0";
        }

        /// <summary>
        /// For private use, static to avoid unneeded reinitialisation
        /// </summary>
        private static readonly System.Security.Cryptography.MD5 m_MD5 = System.Security.Cryptography.MD5.Create();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected static string MD5(string input)
        {
            return MD5(Encoding.UTF8.GetBytes(input));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
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

        public bool Asynchronous
        {
            get { return false; }
        }

        public void Wait()
        {
        }
    }
}
