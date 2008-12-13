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
using System.Xml;
using System.Xml.Serialization;

namespace WikiFunctions
{
    /// <summary>
    /// This class holds all basic information about a wiki
    /// </summary>
    [Serializable]
    public class SiteInfo : IXmlSerializable
    {
        private string m_ScriptPath;
        private Dictionary<int, string> m_Namespaces = new Dictionary<int, string>();
        private Dictionary<int, List<string>> m_NamespaceAliases = new Dictionary<int, List<string>>();
        private Dictionary<string, string> m_MessageCache = new Dictionary<string, string>();
        private DateTime m_Time;

        /// <summary>
        /// Creates an instance of the class
        /// </summary>
        /// <param name="scriptPath">URL where index.php and api.php reside</param>
        public SiteInfo(string scriptPath)
        {
            ScriptPath = scriptPath;
            m_Time = DateTime.Now;

            try
            {
                if (!LoadNamespaces())
                    throw new WikiUrlException();
            }
            catch (Exception ex)
            {
                throw new WikiUrlException(ex);
            }

        }

        public SiteInfo(string scriptPath, Dictionary<int, string> namespaces)
        {
            ScriptPath = scriptPath;
            m_Namespaces = namespaces;
            m_Time = DateTime.Now;
        }

        internal SiteInfo()
        { }

        private static void VerifyIntegrity()
        { }

        public static string NormalizeURL(string url)
        {
            if (!url.EndsWith("/")) return url + "/";
            else return url;
        }

        public bool LoadNamespaces()
        {
            string output = Tools.GetHTML(m_ScriptPath + "api.php?action=query&meta=siteinfo&siprop=general|namespaces|namespacealiases|statistics&format=xml");

            XmlDocument xd = new XmlDocument();
            xd.LoadXml(output);

            if (xd.GetElementsByTagName("api").Count != 1)
                return false;

            foreach (XmlNode xn in xd["api"]["query"]["namespaces"].GetElementsByTagName("ns"))
            {
                int id = int.Parse(xn.Attributes["id"].Value);

                if (id != 0) m_Namespaces[id] = xn.InnerText + ":";
            }

            m_NamespaceAliases = Variables.PrepareAliases(m_Namespaces);

            foreach (XmlNode xn in xd["api"]["query"]["namespacealiases"].GetElementsByTagName("ns"))
            {
                int id = int.Parse(xn.Attributes["id"].Value);

                if (id != 0) m_NamespaceAliases[id].Add(xn.InnerText);
            }

            return true;
        }

        [XmlAttribute(AttributeName = "url")]
        public string ScriptPath
        {
            get { return m_ScriptPath; }
            internal set
            {
                m_ScriptPath = NormalizeURL(value);
            }
        }

        [XmlAttribute(AttributeName = "time")]
        public DateTime Time
        {
            get { return m_Time; }
        }

        [XmlIgnore]
        public Dictionary<int, string> Namespaces
        { get { return m_Namespaces; } }

        [XmlIgnore]
        public Dictionary<int, List<string>> NamespaceAliases
        { get { return m_NamespaceAliases; } }

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

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void WriteXml(XmlWriter writer)
        {
            //writer.WriteStartElement("site");
            writer.WriteAttributeString("URL", m_ScriptPath);
            writer.WriteStartAttribute("Time");
            writer.WriteValue(m_Time);
            {
                writer.WriteStartElement("Namespaces");
                {
                    foreach (KeyValuePair<int, string> p in m_Namespaces)
                    {
                        writer.WriteStartElement("Namespace");
                        writer.WriteAttributeString("id", p.Key.ToString());
                        writer.WriteValue(p.Value);
                        writer.WriteEndElement();
                    }
                }
            }
            //writer.WriteEndElement();
        }
        #endregion
    }

    public class WikiUrlException : Exception
    {
        public WikiUrlException()
            : base("Can't connect to given wiki site.")
        { }

        public WikiUrlException(Exception innerException)
            : base("Can't connect to given wiki site.", innerException)
        { }
    }
}
