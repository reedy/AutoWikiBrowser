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
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Collections.Specialized;

namespace WikiFunctions
{
    /// <summary>
    /// The main class for editing Wikipedia. This class must be instantiated and persists login data.
    /// </summary>
    public class Editor
    {
        /// <summary>
        /// Instantiates the Editor class.
        /// </summary>
        public Editor()
        { }

        protected CookieCollection logincookies;
        protected bool LoggedIn;
        protected string m_indexpath = Variables.URLLong;
        private static string mUserAgent = "WPAutoEdit/1.02";

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
        /// <param name="Article">The article to return the wikitext for.</param>
        /// <param name="indexpath">The path to the index page of the wiki to edit.</param>
        /// <returns>The wikitext of the specified article.</returns>
        public static String GetWikiText(String Article, string indexpath, int oldid)
        {
            try
            {
                string targetUrl = indexpath + "index.php?title=" + Tools.WikiEncode(Article) + "&action=raw";

                if (oldid != 0)
                    targetUrl += "&oldid=" + oldid;

                HttpWebRequest wr = Variables.PrepareWebRequest(targetUrl, mUserAgent);
                HttpWebResponse resps;
                Stream stream;
                StreamReader sr;
                string wikitext = "";

                //wr.Proxy.Credentials = CredentialCache.DefaultCredentials;

                resps = (HttpWebResponse)wr.GetResponse();

                stream = resps.GetResponseStream();
                sr = new StreamReader(stream);

                wikitext = sr.ReadToEnd();

                sr.Close();
                stream.Close();
                resps.Close();

                return wikitext;
            }
            catch (WebException ex)
            {
                if (ex.ToString().Contains("404"))
                    return "";
                else
                    throw;
            }
        }

        public struct EditPageRetvals
        {
            public string article;
            public string responsetext;
            public string difflink;
        }

        /// <summary>
        /// Edits the specified page.
        /// </summary>
        /// <param name="Article">The article to edit.</param>
        /// <param name="NewText">The new wikitext for the article.</param>
        /// <param name="Summary">The edit summary to use for this edit.</param>
        /// <param name="Minor">Whether or not to flag the edit as minor.</param>
        /// <param name="Watch">Whether article should be added to your watchlist</param>
        /// <returns>A link to the diff page for the changes made.</returns>
        public string EditPage(String Article, String NewText, String Summary, bool Minor, bool Watch)
        {
            return EditPageEx(Article, NewText, Summary, Minor, Watch).responsetext;
        }

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
            WebResponse resps;
            String poststring;
            String editpagestr;

            editpagestr = GetEditPage(Article);

            Match m = Edittime.Match(editpagestr);
            string wpEdittime = m.Groups[1].Value;

            m = EditToken.Match(editpagestr);
            string wpEditkey = System.Web.HttpUtility.UrlEncode(m.Groups[1].Value);

            wr.CookieContainer = new CookieContainer();

            foreach (Cookie cook in logincookies)
                wr.CookieContainer.Add(cook);

            //Create poststring
            poststring = string.Format("wpSection=&wpStarttime={0}&wpEdittime={1}&wpScrolltop=&wpTextbox1={2}&wpSummary={3}&wpSave=Save%20Page&wpEditToken={4}",
                new string[] { DateTime.Now.ToUniversalTime().ToString("yyyyMMddHHmmss"), wpEdittime, HttpUtility.UrlEncode(NewText), HttpUtility.UrlEncode(Summary), wpEditkey });

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

            resps = wr.GetResponse();

            Regex permalinkrx = new Regex("<li id=\"t-permalink\"><a href=\"([^\"]*)\">");
            Match permalinkmatch;

            StreamReader sr = new StreamReader(resps.GetResponseStream());
            EditPageRetvals retval = new EditPageRetvals();
            retval.article = Article;

            retval.responsetext = sr.ReadToEnd();

            permalinkmatch = permalinkrx.Match(retval.responsetext);
            
            //From the root directory.
            retval.difflink = m_indexpath.Substring(0, m_indexpath.IndexOf("/",9)) + 
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
        /// <returns>A link to the diff page for the changes made.</returns>
        public string EditPage(String Article, String NewText, String Summary, bool Minor)
        {
            return EditPage(Article, NewText, Summary, Minor, false);
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
            HttpWebResponse resps;
            Stream stream;
            StreamReader sr;
            string wikitext = "";

            wr.CookieContainer = new CookieContainer();

            foreach (Cookie cook in logincookies)
                wr.CookieContainer.Add(cook);

            Article = HttpUtility.UrlEncode(Article);

            resps = (HttpWebResponse) wr.GetResponse();

            stream = resps.GetResponseStream();
            sr = new StreamReader(stream);

            wikitext = sr.ReadToEnd();

            sr.Close();
            stream.Close();
            resps.Close();

            return wikitext;
        }

        /// <summary>
        /// Logs this instance in with the specified username and password.
        /// </summary>
        /// <param name="Username">The username to log in with.</param>
        /// <param name="password">The password to log in with.</param>
        public void LogIn(string Username, string password)
        {
            HttpWebRequest wr = Variables.PrepareWebRequest(m_indexpath + "index.php?title=Special:Userlogin&action=submitlogin&type=login", UserAgent);
            HttpWebResponse resps;
            String poststring;

            //Create poststring
            poststring = string.Format("wpName=+{0}&wpPassword={1}&wpRemember=1&wpLoginattempt=Log+in",
                new string[] { HttpUtility.UrlEncode(Username), HttpUtility.UrlEncode(password) });

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

            resps = (HttpWebResponse)wr.GetResponse();

            logincookies = resps.Cookies;
        }

        /// <summary>
        /// Appends the specified text to the specified article.
        /// </summary>
        /// <param name="Article">The article to append text to.</param>
        /// <param name="AppendText">The text to append.</param>
        /// <param name="Summary">The edit summary for this edit.</param>
        /// <param name="Minor">Whether or not to flag this edit as minor.</param>
        /// <returns>A link to the diff page for the changes made.</returns>
        public string EditPageAppend(string Article, string AppendText, string Summary, bool Minor)
        {
            return EditPageAppendEx(Article, AppendText, Summary, Minor).difflink;
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
            string pagetext;

            pagetext = GetWikiText(Article, m_indexpath, 0);
            pagetext += AppendText;

            return EditPageEx(Article, pagetext, Summary, Minor);
        }

        /// <summary>
        /// Prepends the specified text to the specified article.
        /// </summary>
        /// <param name="Article">The article to prepend text to.</param>
        /// <param name="PrependText">The text to prepend.</param>
        /// <param name="Summary">The edit summary for this edit.</param>
        /// <param name="Minor">Whether or not to flag this edit as minor.</param>
        /// <returns>A link to the diff page for the changes made.</returns>
        public string EditPagePrepend(string Article, string PrependText, string Summary, bool Minor)
        {
            string pagetext;

            pagetext = GetWikiText(Article, m_indexpath, 0);

            pagetext = PrependText + pagetext;

            return EditPage(Article, pagetext, Summary, Minor);
        }

        /// <summary>
        /// Edits an article, replacing items matching a specific .NET Regular Expression with another expression.
        /// Use brackets around a section in the regular expression to remember this area.
        /// Each of these can be recalled in the replacing expression using $N, where N is the 1-based index of the stored expression.
        /// </summary>
        /// <param name="Article">The article to edit.</param>
        /// <param name="FindRegEx">The Regular Expression to find.</param>
        /// <param name="ReplaceRegEx">The expression to replace all found instances with.</param>
        /// <param name="Summary">The edit summary for this edit.</param>
        /// <param name="Minor">Whether or not to flag this edit as minor.</param>
        /// <returns>A link to the diff page for the changes made.</returns>
        public string EditPageReplace(string Article, string findregex, string replaceregex, string Summary, bool Minor)
        {
            string pagetext = "";
            Regex rgex = new Regex(findregex, RegexOptions.IgnoreCase);

            pagetext = GetWikiText(Article, m_indexpath, 0);
            pagetext = rgex.Replace(pagetext, replaceregex);

            return EditPage(Article, pagetext, Summary, Minor);
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

        /// <summary>
        /// The path to the script directory of the wiki to edit.
        /// </summary>
        public string IndexPath
        {
            get { return m_indexpath; }
            set { m_indexpath = value; }
        }
        #endregion

        #region Revisions and reverts
        /// <summary>
        /// Reverts an article to the specified revision.
        /// </summary>
        /// <param name="Article">The article to revert.</param>
        /// <param name="oldid">The revision number to revert to.</param>
        /// <param name="Summary">The edit summary to use in this reversion.</param>
        /// <param name="Minor">Whether or not to mark this edit as minor.</param>
        public void RevertToRevision(string Article, int oldid, string Summary, bool Minor)
        {
            string text = GetWikiText(Article, m_indexpath, oldid);

            EditPage(Article, text, Summary, Minor);
        }

        /// <summary>
        /// Reverts the last edit to an article.
        /// </summary>
        /// <param name="Article">The article to revert.</param>
        /// <param name="Summary">The edit summary to use in your reversion.</param>
        /// <param name="Minor">Whether or not to mark this edit as minor.</param>
        public void Revert(string Article, string Summary, bool Minor)
        {
            List<Revision> history;

            history = GetHistory(Article, 2);
            Summary = Summary.Replace("%u", history[0].User);

            RevertToRevision(Article, history[1].RevisionID, Summary, Minor);

        }

        /// <summary>
        /// Rolls back an article to the most recent revision that is not by the specified user.
        /// </summary>
        /// <param name="Article">The article to roll back.</param>
        /// <param name="User">The user whose edits need to be reverted.</param>
        /// <param name="Summary">The edit summary for this rollback.</param>
        /// <param name="Minor">Whether or not to mark this edit as minor.</param>
        public void Rollback(string Article, string User, string Summary, bool Minor)
        {
            List<Revision> history;

            history = GetHistory(Article, 250);

            int i;
            string historyUser = history[0].User;

            if (historyUser != User)
                return;

            for (i = 0; i <= 249 && history[i].User == User; i++)
            {
            }

            if (history[i].User != User)
            {
                Summary = Summary.Replace("%v", User);
                Summary = Summary.Replace("%u", history[i].User);

                RevertToRevision(Article, history[i].RevisionID, Summary, Minor);
            }
        }

        /// <summary>
        /// Retrieves the history for an article.
        /// </summary>
        /// <param name="Article">The article that history is required for.</param>
        /// <param name="Limit">The maximum number of revisions to return.</param>
        /// <returns>The history, as a generic list of Revision objects.</returns>
        public List<Revision> GetHistory(string Article, int Limit)
        {
            string targetUrl = m_indexpath + "api.php?action=query&prop=revisions&titles=" + HttpUtility.UrlEncode(Article) +
                "&rvlimit=" + Limit;

            HttpWebRequest wr = Variables.PrepareWebRequest(targetUrl, UserAgent);
            HttpWebResponse resps;
            Stream stream;
            StreamReader sr;
            string pagetext = "";
            List<Revision> history = new List<Revision>();

            resps = (HttpWebResponse)wr.GetResponse();
            stream = resps.GetResponseStream();
            sr = new StreamReader(stream);

            pagetext = sr.ReadToEnd();

            XmlDocument doc = new XmlDocument();

            doc.LoadXml(pagetext);

            foreach (XmlElement rvElement in doc.GetElementsByTagName("rev"))
            {
                Revision rv = new Revision();

                rv.RevisionID = Convert.ToInt32(rvElement.Attributes["revid"].Value);
                rv.Time = DateTime.Parse(rvElement.Attributes["timestamp"].Value);
                rv.Summary = rvElement.InnerText;
                rv.User = rvElement.Attributes["user"].Value;

                rv.Minor = (rvElement.OuterXml.Contains("minor=\""));

                history.Add(rv);
            }

            return history;
        }

        /// <summary>
        /// Provides a class that holds data on a specific revision of an article.
        /// </summary>
        public class Revision
        {
            public int RevisionID;
            public string Summary = "";
            public DateTime Time;
            public bool Minor;
            public string User = "";
        }

        #endregion

        #region Watchlist-related

        /// <summary>
        /// Adds the specified page to current user's watchlist or removes from it
        /// </summary>
        /// <param name="Page">Page to watch/unwatch</param>
        /// <param name="Watch">Whether to add or remove</param>
        /// <returns>true if successful</returns>
        public bool WatchPage(String Page, bool Watch)
        {
            try
            {
                HttpWebRequest wr = Variables.PrepareWebRequest(m_indexpath + "index.php?title=" +
                    HttpUtility.UrlEncode(Page) + "&action=" + (Watch ? "watch" : "unwatch"), UserAgent);
                WebResponse resps;

                wr.CookieContainer = new CookieContainer();

                foreach (Cookie cook in logincookies)
                    wr.CookieContainer.Add(cook);

                resps = wr.GetResponse();
                resps.Close();
            }
            catch { return false; }
            return true;
        }

        /// <summary>
        /// Adds the specified page to current user's watchlist
        /// </summary>
        /// <param name="Page">Page to watch</param>
        /// <returns>true if successful</returns>
        public bool WatchPage(String Page)
        {
            return WatchPage(Page, true);
        }

        /// <summary>
        /// Removes the specified page from current user's watchlist
        /// </summary>
        /// <param name="Page">Page to unwatch</param>
        /// <returns>true if successful</returns>
        public bool UnwatchPage(String Page)
        {
            return WatchPage(Page, false);
        }

        /// <summary>
        /// Removes all items from watchlist
        /// </summary>
        /// <returns>true if successful</returns>
        public bool ClearWatchlist()
        {   // doesn't work so far
            try
            {
                Regex rx = new Regex("<input name=\"token\" type=\"hidden\" value=\"([^\"]*)\" />");
                Match m;

                HttpWebRequest wr = Variables.PrepareWebRequest(m_indexpath +
                    "index.php?title=Special:Watchlist/clear", UserAgent);
                WebResponse resps;

                wr.CookieContainer = new CookieContainer();

                foreach (Cookie cook in logincookies)
                    wr.CookieContainer.Add(cook);

                resps = wr.GetResponse();

                Stream stream;
                StreamReader sr;

                stream = resps.GetResponseStream();
                sr = new StreamReader(stream);

                string html = sr.ReadToEnd();
                resps.Close();
                sr.Close();

                m = rx.Match(html);

                int index = m.Value.IndexOf("value=\"") + 7;
                string token = m.Value.Substring(index, m.Value.Substring(index).IndexOf("\""));

                wr = Variables.PrepareWebRequest(m_indexpath +
                    "index.php?title=Special:Watchlist&amp;action=clear", UserAgent);

                wr.CookieContainer = new CookieContainer();

                foreach (Cookie cook in logincookies)
                    wr.CookieContainer.Add(cook);

                wr.Method = "POST";
                wr.ContentType = "application/x-www-form-urlencoded";

                byte[] bytedata = Encoding.UTF8.GetBytes("submit=Submit&token=" + token);

                wr.ContentLength = bytedata.Length;

                Stream rs = wr.GetRequestStream();

                rs.Write(bytedata, 0, bytedata.Length);
                rs.Close();

                resps = wr.GetResponse();
                stream = resps.GetResponseStream();
                sr = new StreamReader(stream);

                html = sr.ReadToEnd();
            }
            catch { return false; }
            return true;
        }

        /// <summary>
        /// Returns all pages in user's watchlist
        /// </summary>
        /// <returns>StringCollection with page titles</returns>
        public StringCollection GetWatchlist()
        {
            HttpWebRequest wr = Variables.PrepareWebRequest(m_indexpath +
               "index.php?title=Special:Watchlist/edit", UserAgent);
            WebResponse resps;
     
            wr.CookieContainer = new CookieContainer();
            foreach (Cookie cook in logincookies)
                wr.CookieContainer.Add(cook);

            resps = wr.GetResponse();

            Stream stream;
            StreamReader sr;

            stream = resps.GetResponseStream();
            sr = new StreamReader(stream);

            string html = sr.ReadToEnd();
            resps.Close();
            sr.Close();

            StringCollection list = new StringCollection();
            Regex r = new Regex("<input type=\"checkbox\" name=\"id\\[\\]\" value=(.*?) />");
            foreach (Match m in r.Matches(html))
            {
                string title = m.Groups[1].Value.Trim('"');
                title = title.Replace("&amp;", "&").Replace("&quot;", "\"");
                list.Add(title);
            }

            return list;
        }

        #endregion
    }

    /// <summary>
    /// Class that stores information about user and his rights
    /// </summary>
    public class UserInfo
    {
        StringCollection Groups = new StringCollection();
        StringCollection Rights = new StringCollection();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xml"></param>
        public UserInfo(string xml)
        {
            StringReader sr = new StringReader(xml);
            XmlTextReader xr = new XmlTextReader(sr);

            xr.MoveToContent();

            while (xr.Read())
            {
                if (xr.Name == "g")
                {
                    Groups.Add(xr.ReadString());
                }
                if (xr.Name == "r")
                {
                    Rights.Add(xr.ReadString());
                }
            }
            Name = Regex.Match(xml, @"<userinfo name=""(.*?)"">").Groups[1].Value;
        }

        /// <summary>
        /// Returns if the user is an Admin/Sysop
        /// </summary>
        public bool IsSysop
        {
            get { return Groups.Contains("sysop"); }
        }

        /// <summary>
        /// Returns if the user is a Bot
        /// </summary>
        public bool IsBot
        {
            get { return Groups.Contains("bot"); }
        }

        /// <summary>
        /// Returns if the user is anonamous
        /// </summary>
        public bool IsAnon
        {
            get { return Tools.IsIP(Name); }
        }

        public string Name;
    }
}
