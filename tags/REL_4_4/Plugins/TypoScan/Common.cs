using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;

namespace WikiFunctions.Plugins.ListMaker.TypoScan
{
    static class Common
    {
        /// <summary>
        /// The URL, script and action=
        /// </summary>
        private const string Url = "http://toolserver.org/~maxsem/typoscan/index.php?action=";

        internal static string GetUrlFor(string action)
        {
            string wiki = Regex.Replace(Variables.URLLong, @"^https?://([-a-z0-9\.]+).*$", "$1", RegexOptions.IgnoreCase);

            return Url + action + "&wiki=" + wiki;
        }

        public static string CheckOperation(string xml)
        {
            using (XmlTextReader r = new XmlTextReader(new StringReader(xml)))
            {
                if (!r.ReadToFollowing("operation"))
                    return "xml";

                if (r.GetAttribute("status") == "success")
                    return null;

                string s = r.GetAttribute("error");
                if (!string.IsNullOrEmpty(s))
                    return s;
                else
                    return r.ReadString();
            }
        }
    }
}
