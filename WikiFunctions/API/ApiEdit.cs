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
using System.Security.Cryptography.X509Certificates;

namespace WikiFunctions.API
{
    //TODO: refactor XML parsing
    //TODO: generalise edit token retrieval
    /// <summary>
    /// This class edits MediaWiki sites using api.php
    /// </summary>
    /// <remarks>
    /// MediaWiki API manual: https://www.mediawiki.org/wiki/API
    /// Site prerequisites: MediaWiki 1.13+ with the following settings:
    /// * $wgEnableAPI = true; (enabled by default in DefaultSettings.php)
    /// * $wgEnableWriteAPI = true;
    /// * AssertEdit extension installed (https://www.mediawiki.org/wiki/Extension:Assert_Edit)
    /// </remarks>
    public class ApiEdit : IApiEdit
    {
        /// <summary>
        /// 
        /// </summary>
        private ApiEdit()
        {
            Cookies = new CookieContainer();
            User = new UserInfo();
            NewMessageThrows = true;
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
        /// <param name="usePHP5">Whether a .php5 extension is to be used</param>
        public ApiEdit(string url, bool usePHP5)
            : this()
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentException("Invalid URL specified", "url");
            //if (!url.StartsWith("http://")) throw new NotSupportedException("Only editing via HTTP is currently supported");

            URL = url;
            PHP5 = usePHP5;
            ApiURL = URL + "api.php" + (PHP5 ? "5" : "");
            Maxlag = 5;

            IWebProxy proxy;
            if (ProxyCache.TryGetValue(url, out proxy))
            {
                ProxySettings = proxy;
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

        public IApiEdit Clone()
        {
            return new ApiEdit
            {
                URL = URL,
                ApiURL = ApiURL,
                PHP5 = PHP5,
                Maxlag = Maxlag,
                Cookies = Cookies,
                ProxySettings = ProxySettings,
                User = User
            };
        }

        #region Properties

        /// <summary>
        /// Path to scripts on server e.g. https://en.wikipedia.org/w/ for en-wiki
        /// </summary>
        public string URL { get; private set; }

        /// <summary>
        /// Path to api.php e.g. https://en.wikipedia.org/w/api.php for en-wiki
        /// </summary>
        public string ApiURL { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        private string Server
        {
            get { return "https://" + new Uri(URL).Host; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool PHP5 { get; private set; }

        /// <summary>
        /// Maxlag parameter of every request (https://www.mediawiki.org/wiki/Manual:Maxlag_parameter)
        /// </summary>
        public int Maxlag { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool NewMessageThrows { get; set; }

        /// <summary>
        /// Action for which we have edit token
        /// </summary>
        public string Action { get; private set; }

        /// <summary>
        /// Name of the page currently being edited
        /// </summary>
        public PageInfo Page { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string HtmlHeaders { get; private set; }

        /// <summary>
        /// Cookies stored between requests
        /// </summary>
        public CookieContainer Cookies { get; private set; }

        #endregion

        /// <summary>
        /// Resets all internal variables, discarding edit tokens and so on,
        /// but does not logs off
        /// </summary>
        public void Reset()
        {
            Action = null;
            Page = new PageInfo();
            Aborting = false;
            Request = null;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Abort()
        {
            Aborting = true;
            Request.Abort();
            Thread.Sleep(1);
            Aborting = false;
        }

        /// <summary>
        /// This is a hack required for some multilingual Wikimedia projects,
        /// where CentralAuth returns cookies with a redundant domain restriction.
        /// </summary>
        private void AdjustCookies()
        {
            Uri uri = new Uri(URL);
            string host = uri.Host;
            var newCookies = new CookieContainer();
            var urls = new[] {uri, new Uri(uri.Scheme + Uri.SchemeDelimiter + "fnord." + host)};
            foreach (var u in urls)
            {
                foreach (Cookie c in Cookies.GetCookies(u))
                {
                    c.Domain = host;
                    newCookies.Add(c);
                }
            }

            Cookies = newCookies;
        }

        #region URL stuff

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected static string BuildQuery(Dictionary<string, string> request)
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, string> kvp in request)
            {
                string s = kvp.Key; // TODO: This is probably redundant now
                if (string.IsNullOrEmpty(s))
                {
                    continue;
                }
                sb.Append('&');
                sb.Append(s);
                if (s.Contains("="))
                {
                    Tools.WriteDebug(s, "Api key parameter includes =");
                }
                // Always send a =, so we don't break boolean parameters passed in the POST part of the query
                sb.Append('=');

                if (!string.IsNullOrEmpty(kvp.Value)) // empty string is a valid parameter value!
                {
                    sb.Append(HttpUtility.UrlEncode(kvp.Value));
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
        /// <param name="options"></param>
        protected void AppendOptions(Dictionary<string, string> request, ActionOptions options)
        {
            if ((options & ActionOptions.CheckMaxlag) > 0 && Maxlag > 0)
            {
                request.Add("maxlag", Maxlag.ToString());
            }

            if ((options & ActionOptions.RequireLogin) > 0)
            {
                request.Add("assert", "user");
            }

            if (request.ContainsKey("action") && request["action"] == "query"
                && ((options & ActionOptions.CheckNewMessages) > 0))
            {
                if (request.ContainsKey("meta"))
                {
                    request["meta"] += "|userinfo|notifications";
                }
                else
                {
                    request.Add("meta", "userinfo|notifications");
                }
                request.Add("uiprop", "hasmsg");
                request.Add("notprop", "count");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        protected string BuildUrl(Dictionary<string, string> request, ActionOptions options)
        {
            AppendOptions(request, options);
            return ApiURL + "?format=xml" + BuildQuery(request);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected string BuildUrl(Dictionary<string, string> request)
        {
            return BuildUrl(request, ActionOptions.None);
        }

        #endregion

        #region Network access

        private static readonly Dictionary<string, IWebProxy> ProxyCache = new Dictionary<string, IWebProxy>();
        private IWebProxy ProxySettings;

        private static readonly string UserAgent = string.Format("WikiFunctions ApiEdit/{0} ({1}; .NET CLR {2})",
            Assembly.GetExecutingAssembly().GetName().Version,
            Environment.OSVersion.VersionString,
            Environment.Version);

        private static bool customXertificateValidation(object sender, X509Certificate cert, X509Chain chain,
            System.Net.Security.SslPolicyErrors error)
        {
            // TODO: Implement better validation. JOE 20110722
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        protected HttpWebRequest CreateRequest(string url)
        {
            if (Globals.UnitTestMode) throw new Exception("You shouldn't access Wikipedia from unit tests");

            ServicePointManager.Expect100Continue = false;
            ServicePointManager.ServerCertificateValidationCallback += customXertificateValidation;
            HttpWebRequest res = (HttpWebRequest) WebRequest.Create(url);
            res.KeepAlive = true;
            res.ServicePoint.Expect100Continue = false;
            res.Expect = "";

            if (ProxySettings != null)
            {
                res.Proxy = ProxySettings;
                res.UseDefaultCredentials = true;
            }
            else
            {
                res.Proxy = null;
            }
            res.UserAgent = UserAgent;
            res.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

            // SECURITY: don't send cookies to third-party sites
            if (url.StartsWith(URL)) res.CookieContainer = Cookies;

            return res;
        }

        private bool Aborting;
        private HttpWebRequest Request;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        protected string GetResponseString(HttpWebRequest req)
        {
            Request = req;

            if (!string.IsNullOrEmpty(Variables.HttpAuthUsername) && !string.IsNullOrEmpty(Variables.HttpAuthPassword))
            {
                NetworkCredential login = new NetworkCredential
                {
                    UserName = Variables.HttpAuthUsername,
                    Password = Variables.HttpAuthPassword,
                    // Domain = "",
                };

                CredentialCache myCache = new CredentialCache
                {
                    {new Uri(URL), "Basic", login}
                };
                req.Credentials = myCache;

                req = (HttpWebRequest) SetBasicAuthHeader(req, login.UserName, login.Password);
            }

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
                var resp = (HttpWebResponse) ex.Response;
                if (resp == null) throw;
                switch (resp.StatusCode)
                {
                    case HttpStatusCode.Unauthorized: // 401						
                        break;

                    case HttpStatusCode.NotFound: // 404
                        return ""; // emulate the behaviour of Tools.HttpGet()
                }

                // just reclassifying
                if (ex.Status == WebExceptionStatus.RequestCanceled)
                {
                    throw new AbortedException(this);
                }
                else
                {
                    throw;
                }
            }
            finally
            {
                Request = null;
            }
        }

        private Dictionary<string, string> lastPostParameters;
        private string lastGetUrl;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="req"></param>
        /// <param name="userName"></param>
        /// <param name="userPassword"></param>
        /// <returns></returns>
        /// <remarks>
        /// Source: http://blog.kowalczyk.info/article/Forcing-basic-http-authentication-for-HttpWebReq.html
        /// </remarks>
        protected WebRequest SetBasicAuthHeader(WebRequest req, string userName, string userPassword)
        {
            string authInfo = userName + ":" + userPassword;
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            req.Headers["Authorization"] = "Basic " + authInfo;
            return req;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="get"></param>
        /// <param name="post"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        protected string HttpPost(Dictionary<string, string> get, Dictionary<string, string> post, ActionOptions options)
        {
            string url = BuildUrl(get, options);

            lastGetUrl = url;
            lastPostParameters = post;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="get"></param>
        /// <param name="post"></param>
        /// <returns></returns>
        protected string HttpPost(Dictionary<string, string> get, Dictionary<string, string> post)
        {
            return HttpPost(get, post, ActionOptions.None);
        }

        /// <summary>
        /// Performs a HTTP request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="options"></param>
        /// <returns>Text received</returns>
        protected string HttpGet(Dictionary<string, string> request, ActionOptions options)
        {
            string url = BuildUrl(request, options);
            lastGetUrl = url;

            return HttpGet(url);
        }

        /// <summary>
        /// Performs a HTTP request
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Text received</returns>
        protected string HttpGet(Dictionary<string, string> request)
        {
            return HttpGet(request, ActionOptions.None);
        }

        /// <summary>
        /// Performs a HTTP request
        /// </summary>
        /// <param name="url"></param>
        /// <returns>Text received</returns>
        public string HttpGet(string url)
        {
            Tools.WriteDebug("ApiEdit", url);
            return GetResponseString(CreateRequest(url));
        }

        #endregion

        #region Login / user props

        public void Login(string username, string password)
        {
            Login(username, password, "");
        }

        public void Login(string username, string password, string domain)
        {
            if (string.IsNullOrEmpty(username)) throw new ArgumentException("Username required", "username");
            //if (string.IsNullOrEmpty(password)) throw new ArgumentException("Password required", "password");

            Reset();
            User = new UserInfo(); // we don't know for sure what will be our status in case of exception

            bool domainSet = !string.IsNullOrEmpty(domain);
            var post = new Dictionary<string, string>
            {
                {"lgname", username},
                {"lgpassword", password},
            };
            post.AddIfTrue(domainSet, "lgdomain", domain);

            string result = HttpPost(
                new Dictionary<string, string>
                {
                    {"action", "login"}
                },
                post
                );

            XmlReader xr = CreateXmlReader(result);
            xr.ReadToFollowing("login");

            var attribute = xr.GetAttribute("result");
            if (attribute != null && attribute.Equals("NeedToken", StringComparison.InvariantCultureIgnoreCase))
            {
                AdjustCookies();
                string token = xr.GetAttribute("token");

                post.Add("lgtoken", token);
                result = HttpPost(
                    new Dictionary<string, string> {{"action", "login"}},
                    post
                    );

                xr = CreateXmlReader(result);
                xr.ReadToFollowing("login");
            }

            string status = xr.GetAttribute("result");
            if (status != null && !status.Equals("Success", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new LoginException(this, status);
            }

            CheckForErrors(result, "login");
            AdjustCookies();

            RefreshUserInfo();
        }

        public void Logout()
        {
            Reset();
            User = new UserInfo();
            string result = HttpGet(new Dictionary<string, string> {{"action", "logout"}});
            CheckForErrors(result, "logout");
        }

        public void Watch(string title)
        {
            WatchAction(title, false);
        }

        public void WatchAction(string title, bool unwatch)
        {
            if (string.IsNullOrEmpty(title)) throw new ArgumentException("Page name required", "title");

            // Token needed as of 1.18
            // TODO: Make token getting optional..
            string result = HttpGet(
                new Dictionary<string, string>
                {
                    {"action", "query"},
                    {"prop", "info"},
                    {"intoken", "watch"},
                    {"titles", title}

                },
                ActionOptions.All);

            CheckForErrors(result);

            try
            {
                XmlReader xr = CreateXmlReader(result);
                if (!xr.ReadToFollowing("page")) throw new Exception("Cannot find <page> element");
                Page.WatchToken = xr.GetAttribute("watchtoken");
            }
            catch (Exception ex)
            {
                throw new BrokenXmlException(this, ex);
            }

            if (Aborting) throw new AbortedException(this);

            result = HttpPost(
                new Dictionary<string, string>
                {
                    {"action", "watch"}
                },
                new Dictionary<string, string>
                {
                    {"title", title},
                    {"token", Page.WatchToken}
                },
                ActionOptions.All);
            CheckForErrors(result, "watch");
        }

        public void Unwatch(string title)
        {
            WatchAction(title, true);
        }

        public UserInfo User { get; private set; }

        public void RefreshUserInfo()
        {
            Reset();
            User = new UserInfo();

            string result = HttpPost(new Dictionary<string, string> {{"action", "query"}},
                new Dictionary<string, string>
                {
                    {"meta", "userinfo"},
                    {"uiprop", "blockinfo|hasmsg|groups|rights"}
                });

            var xml = CheckForErrors(result, "userinfo");

            User = new UserInfo(xml);
        }

        #endregion

        #region Page modification

        /// <summary>
        /// Opens the wiki page for editing
        /// </summary>
        /// <param name="title">The wiki page title</param>
        /// <returns>The current content of the wiki page</returns>
        public string Open(string title)
        {
            return Open(title, false);
        }

        /// <summary>
        /// Opens the wiki page for editing
        /// </summary>
        /// <param name="title">The wiki page title</param>
        /// <param name="resolveRedirects"></param>
        /// <returns>The current content of the wiki page</returns>
        public string Open(string title, bool resolveRedirects)
        {
            if (string.IsNullOrEmpty(title))
                throw new ArgumentException("Page name required", "title");

            if (!User.IsLoggedIn)
                throw new LoggedOffException(this);

            Reset();

            // action=query&prop=info|revisions&intoken=edit&titles=Main%20Page&rvprop=timestamp|user|comment|content

            /* converttitles: API doc says "converttitles - Convert titles to other variants if necessary. Only works if the wiki's content language supports variant conversion.
               Languages that support variant conversion include gan, iu, kk, ku, shi, sr, tg, uz, zh"
             * Example with and without converttitles: zh-wiki page 龙门飞甲
             * https://zh.wikipedia.org/w/api.php?action=query&prop=info|revisions&intoken=edit&titles=龙门飞甲&rvprop=timestamp|user|comment|content
             * https://zh.wikipedia.org/w/api.php?action=query&converttitles&prop=info|revisions&intoken=edit&titles=龙门飞甲&rvprop=timestamp|user|comment|content
             If convertitles is not set, API doesn't find the page
             */
            var query = new Dictionary<string, string>
            {
                {"action", "query"},
                {"converttitles", null},
                {"prop", "info|revisions"},
                {"intoken", "edit"},
                {"titles", title},
                {"inprop", "protection|watched|displaytitle"},
                {"rvprop", "content|timestamp"}, // timestamp|user|comment|
            };
            query.AddIfTrue(resolveRedirects, "redirects", null);

            string result = HttpGet(query, ActionOptions.All);

            CheckForErrors(result, "query");

            try
            {
                Page = new PageInfo(result);

                Action = "edit";
            }
            catch (Exception ex)
            {
                throw new BrokenXmlException(this, ex);
            }

            return Page.Text;
        }

        public SaveInfo Save(string pageText, string summary, bool minor, WatchOptions watch)
        {
            if (string.IsNullOrEmpty(pageText) && !Page.Exists)
            {
                throw new ArgumentException("Can't save empty pages", "pageText");
            }
            //if (string.IsNullOrEmpty(summary))
            //{
            //    throw new ArgumentException("Edit summary required", "summary");
            //}
            if (Action != "edit")
            {
                throw new ApiException(this, "This page is not opened properly for editing");
            }
            if (string.IsNullOrEmpty(Page.EditToken))
            {
                throw new ApiException(this, "Edit token is needed to edit pages");
            }

            pageText = Tools.ConvertFromLocalLineEndings(pageText);

            var get = new Dictionary<string, string>
            {
                {"action", "edit"},
                {"title", Page.Title},
                {"watchlist", WatchOptionsToParam(watch)},
            };
            get.AddIfTrue(minor, "minor", null);
            get.AddIfTrue(User.IsBot, "bot", null);

            string result = HttpPost(
                get,
                new Dictionary<string, string>
                {
                    // order matters here - https://bugzilla.wikimedia.org/show_bug.cgi?id=14210#c4
                    {"md5", MD5(pageText)},
                    {"summary", summary},
                    {"basetimestamp", Page.Timestamp},
                    {"text", pageText},
                    {"starttimestamp", Page.TokenTimestamp},
                    {"token", Page.EditToken}
                },
                ActionOptions.All);

            var xml = CheckForErrors(result, "edit");
            Reset();
            return new SaveInfo(xml);
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
                new Dictionary<string, string>
                {
                    {"action", "query"},
                    {"prop", "info"},
                    {"intoken", "delete"},
                    {"titles", title}

                },
                ActionOptions.All);

            CheckForErrors(result);

            try
            {
                XmlReader xr = CreateXmlReader(result);
                if (!xr.ReadToFollowing("page")) throw new Exception("Cannot find <page> element");
                Page.DeleteToken = xr.GetAttribute("deletetoken");
            }
            catch (Exception ex)
            {
                throw new BrokenXmlException(this, ex);
            }

            if (Aborting) throw new AbortedException(this);

            var post = new Dictionary<string, string>
            {
                {"title", title},
                {"token", Page.DeleteToken},
                {"reason", reason},
            };

            //post.AddIfTrue(User.IsBot, "bot", null);
            post.AddIfTrue(watch, "watch", null);
            result = HttpPost(
                new Dictionary<string, string>
                {
                    {"action", "delete"}
                },
                post,
                ActionOptions.All);

            CheckForErrors(result);

            Reset();
        }

        public void Protect(string title, string reason, TimeSpan expiry, string edit, string move)
        {
            Protect(title, reason, expiry.ToString(), edit, move, false, false);
        }

        public void Protect(string title, string reason, string expiry, string edit, string move)
        {
            Protect(title, reason, expiry, edit, move, false, false);
        }

        public void Protect(string title, string reason, TimeSpan expiry, string edit, string move, bool cascade,
            bool watch)
        {
            Protect(title, reason, expiry.ToString(), edit, move, cascade, watch);
        }

        public void Protect(string title, string reason, string expiry, string edit, string move, bool cascade,
            bool watch)
        {
            if (string.IsNullOrEmpty(title)) throw new ArgumentException("Page name required", "title");
            if (string.IsNullOrEmpty(reason)) throw new ArgumentException("Deletion reason required", "reason");

            Reset();
            Action = "protect";

            string result = HttpGet(
                new Dictionary<string, string>
                {
                    {"action", "query"},
                    {"prop", "info"},
                    {"intoken", "protect"},
                    {"titles", title}

                },
                ActionOptions.All);

            CheckForErrors(result);
            Page = new PageInfo(result);

            try
            {
                XmlReader xr = CreateXmlReader(result);
                if (!xr.ReadToFollowing("page")) throw new Exception("Cannot find <page> element");
                Page.ProtectToken = xr.GetAttribute("protecttoken");
            }
            catch (Exception ex)
            {
                throw new BrokenXmlException(this, ex);
            }

            if (Aborting) throw new AbortedException(this);

            // if page does not exist, protection (i.e. salting) requires create protection only
            string protections;

            if (Page.Exists)
                protections = "edit=" + edit + "|move=" + move;
            else
                protections = "create=" + edit;

            string expiryvalue;

            if (string.IsNullOrEmpty(expiry))
                expiryvalue = "";
            else if (Page.Exists)
                expiryvalue = expiry + "|" + expiry;
            else
                expiryvalue = expiry;

            var post = new Dictionary<string, string>
            {
                {"title", title},
                {"token", Page.ProtectToken},
                {"reason", reason},
                {"protections", protections},
            };
            post.AddIfTrue(!string.IsNullOrEmpty(expiry), "expiry", expiryvalue);
            post.AddIfTrue(cascade, "cascade", null);

            //post.AddIfTrue(User.IsBot, "bot", null);
            post.AddIfTrue(watch, "watch", null);

            result = HttpPost(
                new Dictionary<string, string>
                {
                    {"action", "protect"}
                },
                post,
                ActionOptions.All);

            CheckForErrors(result);

            Reset();
        }

        public void Move(string title, string newTitle, string reason)
        {
            Move(title, newTitle, reason, true, false, false);
        }

        public void Move(string title, string newTitle, string reason, bool moveTalk, bool noRedirect)
        {
            Move(title, newTitle, reason, moveTalk, noRedirect, false);
        }

        public void Move(string title, string newTitle, string reason, bool moveTalk, bool noRedirect, bool watch)
        {
            if (string.IsNullOrEmpty(title)) throw new ArgumentException("Page title required", "title");
            if (string.IsNullOrEmpty(newTitle)) throw new ArgumentException("Target page title required", "newTitle");
            if (string.IsNullOrEmpty(reason)) throw new ArgumentException("Page rename reason required", "reason");

            if (title == newTitle) throw new ArgumentException("Page cannot be moved to the same title");

            Reset();
            Action = "move";

            string result = HttpGet(
                new Dictionary<string, string>
                {
                    {"action", "query"},
                    {"prop", "info"},
                    {"intoken", "move"},
                    {"titles", title + "|" + newTitle}

                },
                ActionOptions.All);

            CheckForErrors(result, "query");

            bool invalid;
            try
            {
                XmlReader xr = CreateXmlReader(result);
                if (!xr.ReadToFollowing("page")) throw new Exception("Cannot find <page> element");

                invalid = xr.MoveToAttribute("invalid");

                Page.MoveToken = xr.GetAttribute("movetoken");
            }
            catch (Exception ex)
            {
                throw new BrokenXmlException(this, ex);
            }

            if (Aborting) throw new AbortedException(this);

            if (invalid)
                throw new ApiException(this, "invalidnewtitle", new ArgumentException("Target page invalid", "newTitle"));

            var post = new Dictionary<string, string>
            {
                {"from", title},
                {"to", newTitle},
                {"token", Page.MoveToken},
                {"reason", reason},
                {"protections", ""},
            };

            post.AddIfTrue(moveTalk, "movetalk", null);
            post.AddIfTrue(noRedirect, "noredirect", null);
            //post.AddIfTrue(User.IsBot, "bot", null);
            post.AddIfTrue(watch, "watch", null);

            result = HttpPost(
                new Dictionary<string, string>
                {
                    {"action", "move"}
                },
                post,
                ActionOptions.All);

            CheckForErrors(result, "move");

            Reset();
        }

        #endregion

        #region Query Api

        public string QueryApi(string queryParamters)
        {
            if (string.IsNullOrEmpty(queryParamters))
                throw new ArgumentException("queryParamters cannot be null/empty", "queryParamters");

            string result = HttpGet(ApiURL + "?action=query&format=xml&" + queryParamters);
            //Should we be checking for maxlag?

            CheckForErrors(result, "query");

            return result;
        }

        #endregion

        #region Parse Api

        public string ParseApi(Dictionary<string, string> queryParameters)
        {
            string result = HttpPost(
                new Dictionary<string, string>
                {
                    {"action", "parse"},
                    {"format", "xml"},
                    {"prop", "text|displaytitle|langlinks|categories"}
                },
                queryParameters); //Should we be checking for maxlag?

            CheckForErrors(result, "parse");

            return result;
        }

        #endregion

        #region Wikitext operations

        private string ExpandRelativeUrls(string html)
        {
            // wikilinks
            html = html.Replace(@" href=""/wiki/", @" href=""" + Server + @"/wiki/");

            // relative links (to images, scripts etc.)
            html = html.Replace(@" href=""//", @" href=""https://");
            return html.Replace(@" src=""//", @" src=""https://");
        }

        private static readonly Regex ExtractCssAndJs = new Regex(@"("
                                                                  + @"<!--\[if .*?-->"
                                                                  + @"|<style\b.*?>.*?</style>"
                                                                  + @"|<link rel=""stylesheet"".*?/\s?>"
            //+ @"|<script type=""text/javascript"".*?</script>"
                                                                  + ")",
            RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Loads wiki's UI HTML and scrapes everything we need to make correct previews
        /// </summary>
        private void EnsureHtmlHeadersLoaded()
        {
            if (!string.IsNullOrEmpty(HtmlHeaders)) return;

            string result = HttpGet(
                new Dictionary<string, string>
                {
                    {"action", "parse"},
                    {"prop", "headhtml"},
                    {"title", "a"},
                    {"text", "a"}
                },
                ActionOptions.None
                );

            result = Tools.StringBetween(Tools.UnescapeXML(result), "<head>", "</head>");
            StringBuilder extracted = new StringBuilder(2048);

            foreach (Match m in ExtractCssAndJs.Matches(result))
            {
                extracted.Append(m.Value);
                extracted.Append("\n");
            }

            HtmlHeaders = ExpandRelativeUrls(extracted.ToString());
        }

        public string Preview(string title, string text)
        {
            EnsureHtmlHeadersLoaded();

            string result = HttpPost(
                new Dictionary<string, string>
                {
                    {"action", "parse"},
                    {"prop", "text"}
                },
                new Dictionary<string, string>
                {
                    {"title", title},
                    {"text", text},
                    {"disablepp", null}
                });

            CheckForErrors(result, "parse");
            try
            {
                XmlReader xr = CreateXmlReader(result);
                if (!xr.ReadToFollowing("text")) throw new Exception("Cannot find <text> element");
                return ExpandRelativeUrls(xr.ReadString());
            }
            catch (Exception ex)
            {
                throw new BrokenXmlException(this, ex);
            }
        }

        public void Rollback(string title, string user)
        {
            string result = HttpGet(
                new Dictionary<string, string>
                {
                    {"action", "query"},
                    {"prop", "revisions"},
                    {"rvtoken", "rollback"},
                    {"titles", title}

                },
                ActionOptions.All);

            CheckForErrors(result, "query");

            XmlReader xr = CreateXmlReader(result);
            if (!xr.ReadToFollowing("page")) throw new Exception("Cannot find <page> element");
            string rollbackToken = xr.GetAttribute("rollbacktoken");

            result = HttpPost(
                new Dictionary<string, string>
                {
                    {"action", "rollback"}
                },
                new Dictionary<string, string>
                {
                    {"title", title},
                    {"token", rollbackToken}
                });

            CheckForErrors(result, "rollback");
        }

        public string ExpandTemplates(string title, string text)
        {
            string result = HttpPost(
                new Dictionary<string, string>
                {
                    {"action", "expandtemplates"}
                },
                new Dictionary<string, string>
                {
                    {"title", title},
                    {"text", text}
                });

            CheckForErrors(result, "expandtemplates");
            try
            {
                XmlReader xr = CreateXmlReader(result);
                if (!xr.ReadToFollowing("expandtemplates"))
                    throw new Exception("Cannot find <expandtemplates> element");
                return xr.ReadString();
            }
            catch (Exception ex)
            {
                throw new BrokenXmlException(this, ex);
            }
        }

        #endregion

        #region Error handling

        /// <summary>
        /// Checks the XML returned by the server for error codes and throws an appropriate exception
        /// </summary>
        /// <param name="xml">Server output</param>
        private void CheckForErrors(string xml)
        {
            CheckForErrors(xml, null);
        }

        private static readonly Regex MaxLag = new Regex(@": (\d+) seconds lagged",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Checks the XML returned by the server for error codes and throws an appropriate exception
        /// </summary>
        /// <param name="xml">Server output</param>
        /// <param name="action">The action performed, null if don't check</param>
        private XmlDocument CheckForErrors(string xml, string action)
        {
            if (string.IsNullOrEmpty(xml)) throw new ApiBlankException(this);

            var doc = new XmlDocument();
            try
            {
                doc.Load(new StringReader(xml));
            }
            catch (XmlException xe)
            {
                Tools.WriteDebug("ApiEdit::CheckForErrors", xml);

                string postParams = "";
                if (lastPostParameters != null)
                {
                    if (lastPostParameters.ContainsKey("password"))
                    {
                        lastPostParameters["password"] = "<removed>";
                    }
                    if (lastPostParameters.ContainsKey("token"))
                    {
                        lastPostParameters["token"] = "<removed>";
                    }
                    postParams = BuildQuery(lastPostParameters);
                }
                throw new ApiXmlException(this, xe, lastGetUrl, postParams, xml);
            }

            //TODO: can't figure out the best time for this check
            bool prevMessages = User.HasMessages;
            User.Update(doc);
            if (action != "login"
                && action != "userinfo"
                && NewMessageThrows
                && User.HasMessages
                && !prevMessages)
            {
                throw new NewMessagesException(this);
            }

            var errors = doc.GetElementsByTagName("error");

            if (errors.Count > 0)
            {
                var error = errors[0];
                string errorCode = error.Attributes["code"].Value;
                string errorMessage = error.Attributes["info"].Value;

                switch (errorCode.ToLower())
                {
                    case "maxlag": //guessing
                        int maxlag;
                        int.TryParse(MaxLag.Match(xml).Groups[1].Value, out maxlag);
                        throw new MaxlagException(this, maxlag, 10);
                    case "wrnotloggedin":
                        throw new LoggedOffException(this);
                    case "spamdetected":
                        throw new SpamlistException(this, errorMessage);
                    case "fileexists-sharedrepo-perm":
                        throw new SharedRepoException(this, errorMessage);
                    case "hookaborted":
                        throw new MediaWikiSaysNoException(this, errorMessage);
                    //case "confirmemail":
                    //
                    default:
                        if (errorCode.Contains("disabled"))
                        {
                            throw new FeatureDisabledException(this, errorCode, errorMessage);
                        }

                        throw new ApiErrorException(this, errorCode, errorMessage);
                }
            }

            // look at warnings: are notifications enabled on wiki
            var warnings = doc.GetElementsByTagName("warnings");
            if (warnings.Count > 0)
            {
                var xmlNode = warnings.Item(0);
                if (xmlNode != null &&
                    xmlNode.InnerXml.Contains("Unrecognized value for parameter 'meta': notifications"))
                {
                    Variables.NotificationsEnabled = false;
                }
            }

            if (string.IsNullOrEmpty(action)) return doc; // no action to check

            var api = doc["api"];
            if (api == null) return doc;

            var redirects = api.GetElementsByTagName("r");
            if (action == "query" && redirects.Count >= 1) //We have redirects
            {
                // Workaround for https://bugzilla.wikimedia.org/show_bug.cgi?id=39492
                if (Namespace.IsSpecial(Namespace.Determine(redirects[redirects.Count - 1].Attributes["to"].Value)))
                {
                    throw new RedirectToSpecialPageException(this);
                }
            }

            //FIXME: Awful code is awful
            var page = api.GetElementsByTagName("page");
            if (
                page.Count > 0
                && page[0].Attributes != null
                && page[0].Attributes["invalid"] != null
                && page[0].Attributes["invalid"].Value == ""
                )
            {
                throw new InvalidTitleException(this, page[0].Attributes["title"].Value);
            }

            if (api.GetElementsByTagName("interwiki").Count > 0)
            {
                throw new InterwikiException(this);
            }

            var actionElement = api[action];

            if (actionElement == null)
            {
                return doc; // or shall we explode?
            }

            if (actionElement.HasAttribute("assert"))
            {
                string what = actionElement.GetAttribute("assert");
                if (what == "user")
                    throw new LoggedOffException(this);
                throw new AssertionFailedException(this, what);
            }

            if (actionElement.HasAttribute("spamblacklist"))
            {
                throw new SpamlistException(this, actionElement.GetAttribute("spamblacklist"));
            }

            if (actionElement.GetElementsByTagName("captcha").Count > 0)
            {
                throw new CaptchaException(this);
            }

            string result = actionElement.GetAttribute("result");
            if (!string.IsNullOrEmpty(result) && result != "Success")
            {
                if (actionElement.GetAttribute("code").Contains("abusefilter"))
                    throw new MediaWikiSaysNoException(this, actionElement.GetAttribute("warning"));

                throw new OperationFailedException(this, action, result, xml);
            }

            return doc;
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

        protected static string WatchOptionsToParam(WatchOptions watch)
        {
            switch (watch)
            {
                case WatchOptions.UsePreferences:
                    return "preferences";
                case WatchOptions.Watch:
                    return "watch";
                case WatchOptions.Unwatch:
                    return "unwatch";
                default:
                    return "nochange";
            }
        }

        /// <summary>
        /// For private use, static to avoid unneeded reinitialisation
        /// </summary>
        private static readonly System.Security.Cryptography.MD5 Md5Summer = System.Security.Cryptography.MD5.Create();

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
            byte[] hash = Md5Summer.ComputeHash(input);

            StringBuilder sb = new StringBuilder(20);
            foreach (byte t in hash)
            {
                sb.Append(t.ToString("x2"));
            }

            return sb.ToString();
        }

        protected XmlReader CreateXmlReader(string result)
        {
            return XmlReader.Create(new StringReader(result), new XmlReaderSettings
            {
                ProhibitDtd = false
            });
        }

        #endregion
    }

    public enum WatchOptions
    {
        NoChange,
        UsePreferences,
        Watch,
        Unwatch
    }

    [Flags]
    public enum ActionOptions
    {
        None = 0,
        CheckMaxlag = 1,
        RequireLogin = 2,
        CheckNewMessages = 4,

        All = CheckMaxlag | RequireLogin | CheckNewMessages
    };
}