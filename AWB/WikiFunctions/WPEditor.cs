using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

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
        {
        }

        protected CookieCollection logincookies;
        protected bool LoggedIn;
        protected string m_indexpath = Variables.URL;

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
                string TargetURL = indexpath + "index.php?title=" + Article + "&action=raw&ctype=text/plain&dontcountme=s";

                if (oldid != 0)
                    TargetURL += "&oldid=" + oldid;

                HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(TargetURL);
                HttpWebResponse resps;
                Stream stream;
                StreamReader sr;
                string wikitext = "";

                wr.Proxy.Credentials = CredentialCache.DefaultCredentials;
                wr.UserAgent = "WPAutoEdit/1.0";

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
                    throw ex;
            }
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
            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(m_indexpath + "index.php?title=" + 
                HttpUtility.UrlEncode(Article) + "&action=submit");
            WebResponse resps;
            String poststring;
            String editpagestr;

            editpagestr = GetEditPage(Article);

            Regex rx = new Regex("<input type='hidden' value=\"([^\"]*)\" name=\"wpEdittime\" />");
            Match m;
            string wpEdittime = "";
            string wpEditkey = "";

            m = rx.Match(editpagestr);

            wpEdittime = m.Value.Substring(28, m.Value.Substring(28).IndexOf("\""));

            rx = new Regex("<input type='hidden' value=\"([^\"]*)\" name=\"wpEditToken\" />");

            m = rx.Match(editpagestr);

            wpEditkey = m.Value.Substring(28, m.Value.Substring(28).IndexOf("\""));

            wr.Proxy.Credentials = CredentialCache.DefaultCredentials;
            wr.UserAgent = "WPAutoEdit/1.0";

            wr.CookieContainer = new CookieContainer();

            foreach (Cookie Cook in logincookies)
                wr.CookieContainer.Add(Cook);

            //Create poststring
            poststring = string.Format("wpSection=&wpStarttime={0}&wpEdittime={1}&wpScrolltop=&wpTextbox1={2}&wpWatchThis={3}&wpSummary={4}&wpSave=Save%20Page&wpEditToken={5}",
                new string[] { DateTime.Now.ToUniversalTime().ToString("yyyyMMddHHmmss"), wpEdittime, HttpUtility.UrlEncode(NewText), "off", HttpUtility.UrlEncode(Summary), wpEditkey });

            if (Minor)
                poststring = poststring.Insert(poststring.IndexOf("wpSummary"), "wpMinoredit=1&");

            if (Watch) poststring += "&wpWatchthis=1";

            wr.Method = "POST";
            wr.ContentType = "application/x-www-form-urlencoded";

            //poststring = HttpUtility.UrlEncode(poststring);

            byte[] bytedata = Encoding.UTF8.GetBytes(poststring);

            wr.ContentLength = bytedata.Length;

            Stream rs = wr.GetRequestStream();

            rs.Write(bytedata, 0, bytedata.Length);
            rs.Close();

            resps = wr.GetResponse();

            string respstext = "";
            string difflink = "";
            Regex permalinkrx = new Regex("<li id=\"t-permalink\"><a href=\"([^\"]*)\">");
            Match permalinkmatch;

            StreamReader sr = new StreamReader(resps.GetResponseStream());

            respstext = sr.ReadToEnd();

            permalinkmatch = permalinkrx.Match(respstext);
            
            //From the root directory.
            difflink = m_indexpath.Substring(0, m_indexpath.IndexOf("/",9)) + permalinkmatch.Groups[1].Value + "&diff=prev";

            difflink = difflink.Replace("&amp;", "&");

            return difflink;
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
        /// Adds the specified page to current user's watchlist or removes from it
        /// </summary>
        /// <param name="Page">Page to watch/unwatch</param>
        /// <param name="Watch">Whether to add or remove</param>
        /// <returns></returns>
        public bool WatchPage(String Page, bool Watch)
        {
            try
            {
                HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(m_indexpath + "index.php?title=" +
                    HttpUtility.UrlEncode(Page) + "&action=" + (Watch ? "watch" : "unwatch"));
                WebResponse resps;

                wr.Proxy.Credentials = CredentialCache.DefaultCredentials;
                wr.UserAgent = "WPAutoEdit/1.0";
                wr.CookieContainer = new CookieContainer();

                foreach (Cookie cook in logincookies)
                    wr.CookieContainer.Add(cook);

                resps = wr.GetResponse();
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Adds the specified page to current user's watchlist
        /// </summary>
        /// <param name="Page">Page to watch</param>
        /// <returns></returns>
        public bool WatchPage(String Page)
        {
            return WatchPage(Page, true);
        }

        /// <summary>
        /// Removes the specified page from current user's watchlist
        /// </summary>
        /// <param name="Page">Page to unwatch</param>
        /// <returns></returns>
        public bool UnwatchPage(String Page)
        {
            return WatchPage(Page, false);
        }


        /// <summary>
        /// Internal function to retrieve the HTML for the "edit" page of an article.
        /// </summary>
        /// <param name="Article">The article to retrieve the edit page for.</param>
        /// <returns>The full HTML source of the edit page for the specified article.</returns>
        public string GetEditPage(String Article)
        {
            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(m_indexpath + "index.php?title=" + 
                HttpUtility.UrlEncode(Article) + "&action=edit");
            HttpWebResponse resps;
            Stream stream;
            StreamReader sr;
            string wikitext = "";

            wr.Proxy.Credentials = CredentialCache.DefaultCredentials;
            wr.UserAgent = "WPAutoEdit/1.0";

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
            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(m_indexpath + "index.php?title=Special:Userlogin&action=submitlogin&type=login");
            HttpWebResponse resps;
            String poststring;

            wr.Proxy.Credentials = CredentialCache.DefaultCredentials;
            wr.UserAgent = "WPAutoEdit/1.0";

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

            foreach (Cookie cook in resps.Cookies) { }

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
            string pagetext;

            pagetext = GetWikiText(Article, m_indexpath, 0);

            pagetext += AppendText;

            return EditPage(Article, pagetext, Summary, Minor);
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

        /// <summary>
        /// Reverts an article to the specified revision.
        /// </summary>
        /// <param name="Article">The article to revert.</param>
        /// <param name="oldid">The revision number to revert to.</param>
        /// <param name="Summary">The edit summary to use in this reversion.</param>
        /// <param name="Minor">Whether or not to mark this edit as minor.</param>
        public void RevertToRevision(string Article, int oldid, string Summary, bool Minor)
        {
            string Text = GetWikiText(Article, m_indexpath, oldid);

            EditPage(Article, Text, Summary, Minor);
        }

        /// <summary>
        /// Reverts the last edit to an article.
        /// </summary>
        /// <param name="Article">The article to revert.</param>
        /// <param name="Summary">The edit summary to use in your reversion.</param>
        /// <param name="Minor">Whether or not to mark this edit as minor.</param>
        public void Revert(string Article, string Summary, bool Minor)
        {
            List<Revision> History;

            History = GetHistory(Article, 2);

            Summary = Summary.Replace("%u", History[0].User);

            RevertToRevision(Article, History[1].RevisionID, Summary, Minor);

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
            List<Revision> History;

            History = GetHistory(Article, 250);

            int i;
            string HistoryUser = History[0].User;

            if (HistoryUser != User)
                return;

            for (i = 0; i <= 249 && History[i].User == User; i++)
            {
            }

            if (History[i].User != User)
            {

                Summary = Summary.Replace("%v", User);
                Summary = Summary.Replace("%u", History[i].User);

                RevertToRevision(Article, History[i].RevisionID, Summary, Minor);
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
            string TargetURL = m_indexpath + "query.php?format=xml&what=revisions&rvcomments=1" +
                                "&rvlimit=" + Limit + "&titles=" + HttpUtility.UrlEncode(Article);
            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(TargetURL);
            HttpWebResponse resps;
            Stream stream;
            StreamReader sr;
            string pagetext = "";
            List<Revision> History = new List<Revision>();

            wr.UserAgent = "WPAutoEdit/1.0";

            resps =  (HttpWebResponse)wr.GetResponse();

            stream = resps.GetResponseStream();

            sr = new StreamReader(stream);

            pagetext = sr.ReadToEnd();

            //Yay, XML

            XmlDocument Doc;
            XmlElement DocElement;

            Doc = new XmlDocument();

            Doc.LoadXml(pagetext);

            DocElement = Doc.DocumentElement;
            
            foreach (XmlElement rvElement in Doc.GetElementsByTagName("rv"))
            {
                Revision rv = new Revision();

                rv.RevisionID = Convert.ToInt32(rvElement.Attributes["revid"].Value);
                rv.Time = DateTime.Parse(rvElement.Attributes["timestamp"].Value);
                rv.Summary = rvElement.InnerText;
                rv.User = rvElement.Attributes["user"].Value;
                
                if (rvElement.OuterXml.Contains("minor=\""))
                    rv.Minor = true;
                else
                    rv.Minor = false;

                History.Add(rv);
            }

            return History;
        }

        /// <summary>
        /// Provides a class that holds data on a specific revision of an article.
        /// </summary>
        public class Revision
        {
            public int RevisionID = 0;
            public string Summary = "";
            public DateTime Time;
            public bool Minor = false;
            public string User = "";

        }
    }
}
