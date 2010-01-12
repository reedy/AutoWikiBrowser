/*
 * 
(c) Original author?
(c) 2008 Stephen Kennedy

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

/*Problems with the Editor class:
No login-status variable
Minimal or even no error handling
I think it has to load at least one extra page than is optimal
OTOH: It's simple and does the job. If anyone can fix this up to make it better, please do...
*/

using System;
using System.Text;
using System.Net;
using System.Web;
using System.IO;
using System.Text.RegularExpressions;

namespace WikiFunctions
{
    /// <summary>
    /// The main class for editing Wikipedia. This class must be instantiated and persists login data.
    /// </summary>
    [Obsolete("Use ApiEdit or similar")]
    public class Editor
    {
        protected CookieCollection logincookies;
        protected bool LoggedIn;
        protected string m_indexpath = Variables.URLLong;
        const string mUserAgent = "WPAutoEdit/1.02";

        protected virtual string UserAgent
        { get { return mUserAgent; } }

        #region Regexes
        static readonly Regex Edittime = new Regex("<input type='hidden' value=\"([^\"]*)\" name=\"wpEdittime\" />", RegexOptions.Compiled);
        static readonly Regex EditToken = new
            Regex("<input type='hidden' value=\"([^\"]*)\" name=\"wpEditToken\" />", RegexOptions.Compiled);

        #endregion

        #region Editing
        /// <summary>
        /// Gets the wikitext for a specified article.
        /// </summary>
        /// <param name="article">The article to return the wikitext for.</param>
        /// <param name="indexpath">The path to the index page of the wiki to edit.</param>
        /// <returns>The wikitext of the specified article.</returns>
        /// <param name="oldid"
        public static String GetWikiText(string article, string indexpath, int oldid)
        {
            try
            {
                string targetUrl = indexpath + "index.php?title=" + Tools.WikiEncode(article) + "&action=raw";

                if (oldid != 0)
                    targetUrl += "&oldid=" + oldid;

                HttpWebRequest wr = Variables.PrepareWebRequest(targetUrl, mUserAgent);
                HttpWebResponse resps = (HttpWebResponse)wr.GetResponse();
                Stream stream = resps.GetResponseStream();
                StreamReader sr = new StreamReader(stream);

                //wr.Proxy.Credentials = CredentialCache.DefaultCredentials;

                string wikitext = sr.ReadToEnd();

                sr.Close();
                stream.Close();
                resps.Close();

                return wikitext;
            }
            catch (WebException ex)
            {
                if (ex.ToString().Contains("404"))
                    return "";

                throw;
            }
        }

        public struct EditPageRetvals
        {
            public string Article;
            public string responsetext;
            public string difflink;
        }

        readonly Regex permalinkrx = new Regex("<li id=\"t-permalink\"><a href=\"([^\"]*)\">", RegexOptions.Compiled);

        /// <summary>
        /// Edits the specified page.
        /// </summary>
        /// <param name="Article">The article to edit.</param>
        /// <param name="NewText">The new wikitext for the article.</param>
        /// <param name="Summary">The edit summary to use for this edit.</param>
        /// <param name="Minor">Whether or not to flag the edit as minor.</param>
        /// <param name="Watch">Whether article should be added to your watchlist</param>
        /// <returns>An EditPageRetvals object</returns>
        public EditPageRetvals EditPageEx(String Article, String NewText, String Summary, bool Minor, bool Watch)
        {
            HttpWebRequest wr = Variables.PrepareWebRequest(m_indexpath + "index.php?title=" +
                Tools.WikiEncode(Article) + "&action=submit", UserAgent);

            string editpagestr = GetEditPage(Article);

            Match m = Edittime.Match(editpagestr);
            string wpEdittime = m.Groups[1].Value;

            m = EditToken.Match(editpagestr);
            string wpEditkey = HttpUtility.UrlEncode(m.Groups[1].Value);

            wr.CookieContainer = new CookieContainer();

            foreach (Cookie cook in logincookies)
                wr.CookieContainer.Add(cook);

            //Create poststring
            string poststring =
                string.Format(
                    "wpSection=&wpStarttime={0}&wpEdittime={1}&wpScrolltop=&wpTextbox1={2}&wpSummary={3}&wpSave=Save%20Page&wpEditToken={4}",
                    new[]
                        {
                            DateTime.Now.ToUniversalTime().ToString("yyyyMMddHHmmss"), wpEdittime,
                            HttpUtility.UrlEncode(NewText), HttpUtility.UrlEncode(Summary), wpEditkey
                        });

            if (Minor)
                poststring = poststring.Insert(poststring.IndexOf("wpSummary"), "wpMinoredit=1&");

            if (Watch || editpagestr.Contains("type='checkbox' name='wpWatchthis' checked='checked' accesskey=\"w\" id='wpWatchthis'  />"))
                poststring += "&wpWatchthis=1";

            wr.Method = "POST";
            wr.ContentType = "application/x-www-form-urlencoded";

            //poststring = HttpUtility.UrlEncode(poststring);

            byte[] bytedata = Encoding.UTF8.GetBytes(poststring);

            wr.ContentLength = bytedata.Length;

            Stream rs = wr.GetRequestStream();

            rs.Write(bytedata, 0, bytedata.Length);
            rs.Close();

            WebResponse resps = wr.GetResponse();

            StreamReader sr = new StreamReader(resps.GetResponseStream());
            EditPageRetvals retval = new EditPageRetvals();
            retval.Article = Article;

            retval.responsetext = sr.ReadToEnd();

            Match permalinkmatch = permalinkrx.Match(retval.responsetext);

            //From the root directory.
            retval.difflink = m_indexpath.Substring(0, m_indexpath.IndexOf("/", 9)) +
                permalinkmatch.Groups[1].Value + "&diff=prev";

            retval.difflink = retval.difflink.Replace("&amp;", "&");

            // TODO: Check our submission worked and we have a valid difflink; throw an exception if not. Also check for the sorry we could not process because of loss of session data message
            return retval;
        }

        /// <summary>
        /// Edits the specified page.
        /// </summary>
        /// <param name="Article">The article to edit.</param>
        /// <param name="NewText">The new wikitext for the article.</param>
        /// <param name="Summary">The edit summary to use for this edit.</param>
        /// <param name="Minor">Whether or not to flag the edit as minor.</param>
        /// <returns>An EditPageRetvals object</returns>
        public EditPageRetvals EditPageEx(String Article, String NewText, String Summary, bool Minor)
        {
            return EditPageEx(Article, NewText, Summary, Minor, false);
        }

        /// <summary>
        /// Internal function to retrieve the HTML for the "edit" page of an article.
        /// </summary>
        /// <param name="Article">The article to retrieve the edit page for.</param>
        /// <returns>The full HTML source of the edit page for the specified article.</returns>
        public string GetEditPage(String Article)
        {
            HttpWebRequest wr = Variables.PrepareWebRequest(m_indexpath + "index.php?title=" +
                HttpUtility.UrlEncode(Article) + "&action=edit", UserAgent);

            wr.CookieContainer = new CookieContainer();

            foreach (Cookie cook in logincookies)
                wr.CookieContainer.Add(cook);

            HttpWebResponse resps = (HttpWebResponse)wr.GetResponse();

            Stream stream = resps.GetResponseStream();
            StreamReader sr = new StreamReader(stream);

            string wikitext = sr.ReadToEnd();

            sr.Close();
            stream.Close();
            resps.Close();

            return wikitext;
        }

        /// <summary>
        /// Logs this instance in with the specified username and password.
        /// </summary>
        /// <param name="username">The username to log in with.</param>
        /// <param name="password">The password to log in with.</param>
        public void LogIn(string username, string password)
        {
            HttpWebRequest wr = Variables.PrepareWebRequest(m_indexpath + "index.php?title=Special:Userlogin&action=submitlogin&type=login", UserAgent);

            //Create poststring
            string poststring = string.Format("wpName=+{0}&wpPassword={1}&wpRemember=1&wpLoginattempt=Log+in",
                                              new[] { HttpUtility.UrlEncode(username), HttpUtility.UrlEncode(password) });

            wr.Method = "POST";
            wr.ContentType = "application/x-www-form-urlencoded";
            wr.CookieContainer = new CookieContainer();
            wr.AllowAutoRedirect = false;

            //poststring = HttpUtility.UrlEncode(poststring);

            byte[] bytedata = Encoding.UTF8.GetBytes(poststring);

            wr.ContentLength = bytedata.Length;

            Stream rs = wr.GetRequestStream();

            rs.Write(bytedata, 0, bytedata.Length);
            rs.Close();

            HttpWebResponse resps = (HttpWebResponse)wr.GetResponse();

            logincookies = resps.Cookies;
        }

        /// <summary>
        /// Appends the specified text to the specified article.
        /// </summary>
        /// <param name="Article">The article to append text to.</param>
        /// <param name="AppendText">The text to append.</param>
        /// <param name="Summary">The edit summary for this edit.</param>
        /// <param name="Minor">Whether or not to flag this edit as minor.</param>
        /// <returns>An EditPageRetvals object</returns>
        public EditPageRetvals EditPageAppendEx(string Article, string AppendText, string Summary, bool Minor)
        {
            string pagetext = GetWikiText(Article, m_indexpath, 0) + AppendText;

            return EditPageEx(Article, pagetext, Summary, Minor);
        }

        /// <summary>
        /// Gets the raw wiki-text for a specific page.
        /// </summary>
        /// <param name="Article">The page to get the raw wikitext for.</param>
        /// <returns>The raw wikitext for the specified article.</returns>
        public static string GetWikiText(string Article)
        {
            return GetWikiText(Article, "http://en.wikipedia.org/w/", 0);
        }
        #endregion
    }
}
