/*
Copyright (C) 2007 Max Semenik

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
using System.Xml;

namespace WikiFunctions
{
    /// <summary>
    /// This class holds all basic information about a wiki
    /// </summary>
    public class SiteInfo
    {
        private string m_ScriptPath;
        private Dictionary<int, string> m_Namespaces = new Dictionary<int,string>();
        private Dictionary<string, string> m_MessageCache = new Dictionary<string, string>();

        /// <summary>
        /// Creates an instance of the class
        /// </summary>
        /// <param name="scriptPath">URL where index.php and api.php reside</param>
        public SiteInfo(string scriptPath)
        {
            m_ScriptPath = scriptPath;
            if (!m_ScriptPath.EndsWith("/")) m_ScriptPath = m_ScriptPath + "/";

            Load();
        }

        public void Load()
        {
            string output = Tools.GetHTML(m_ScriptPath + "api.php?action=query&meta=siteinfo&siprop=general|namespaces|namespacealiases|statistics&format=xml");

            XmlDocument xd = new XmlDocument();
            xd.LoadXml(output);

            foreach (XmlNode xn in xd.GetElementsByTagName("ns"))
            {
                int id = int.Parse(xn.Attributes["id"].Value);

                m_Namespaces[id] = xn.InnerText;
            }
        }

        public string ScriptPath
        { get { return m_ScriptPath; } }


        public Dictionary<int, string> Namespaces
        {
            get
            {
                return m_Namespaces;
            }
        }

        public Dictionary<string, string> GetMessages(params string[] names)
        {
            string joined = string.Join("|", names);

            string url = m_ScriptPath + "api.php?format=xml&action=query&meta=allmessages&ammessages=" + joined;

            string output = Tools.GetHTML(url);

            XmlDocument xd = new XmlDocument();
            xd.LoadXml(output);

            Dictionary<string, string> result = new Dictionary<string, string>(names.Length);

            foreach (XmlNode xn in xd.GetElementsByTagName("message"))
            {
                result[xn.Attributes["name"].Value] = xn.InnerText;
            }

            return result;
        }

        #region Service functions
        #endregion
    }
}
