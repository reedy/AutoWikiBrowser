using System;
using System.Collections.Generic;
using System.Text;

using System.Net;
using System.Web;
using System.IO;
using System.Xml;

namespace WikiFunctions
{
    internal static class IRCMTools
    {
        #region From Special:Listusers
        /// <summary>
        /// Gets a list of users with given parameters
        /// </summary>
        /// <param name="group">user group, e.g. "sysop"</param>
        /// <param name="from">username to start from</param>
        /// <param name="limit">limit of users returned (max. 5000) if value <= 0, maximum assumed</param>
        /// <returns>The list of the articles.</returns>
        /// <remarks>Only used by IRCM</remarks>
        public static List<Article> FromListUsers(string group, string from)
        {
            //TODO:api.php?
            int limit = 5000;
            List<Article> list = new List<Article>();

            try
            {
                string url = Variables.URLLong + "index.php?title=Special:Listusers&group=" + group +
                    "&username=" + HttpUtility.UrlEncode(from) + "&limit=" + limit.ToString();

                string search = Tools.GetHTML(url);
                search = Tools.StringBetween(search, "<!-- start content -->", "<!-- end content -->");
                search = "<div>" + search + "</div>";
                StringReader sr = new StringReader(search);
                XmlDocument xml = new XmlDocument();
                xml.Load(sr);

                foreach (XmlNode n in xml.GetElementsByTagName("li"))
                {
                    list.Add(new WikiFunctions.Article(Variables.Namespaces[2] + n.FirstChild.InnerText));
                }
            }
            finally
            { }
            return list;
        }
        #endregion
    }
}
