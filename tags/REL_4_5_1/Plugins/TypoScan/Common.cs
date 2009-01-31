/*
TypoScan AWBPlugin
Copyright (C) 2008 Sam Reed, Max Semenik

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

                return r.ReadString();
            }
        }
    }
}
