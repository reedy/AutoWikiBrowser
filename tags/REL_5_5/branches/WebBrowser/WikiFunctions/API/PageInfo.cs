/*
Copyright (C) 2009 Max Semenik

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

using System.Xml;
using System.IO;
using System;
namespace WikiFunctions.API
{
    /// <summary>
    /// This class represents information about the page currently being edited
    /// </summary>
    public sealed class PageInfo
    {
        internal PageInfo()
        {
        }

        internal PageInfo(string xml)
        {
            XmlReader xr = XmlReader.Create(new StringReader(xml));
            if (!xr.ReadToFollowing("page")) throw new Exception("Cannot find <page> element");

            Exists = (xr.GetAttribute("missing") == null); //if null, page exists
            EditToken = xr.GetAttribute("edittoken");

            long revId;
            RevisionID = long.TryParse(xr.GetAttribute("lastrevid"), out revId) ? revId : -1;

            Title = xr.GetAttribute("title");
            NamespaceID = int.Parse(xr.GetAttribute("ns"));

            if (xr.ReadToDescendant("protection") && !xr.IsEmptyElement)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xr.ReadOuterXml());

                foreach (XmlNode xn in doc.GetElementsByTagName("pr"))
                {
                    switch (xn.Attributes["type"].Value)
                    {
                        case "edit":
                            EditProtection = xn.Attributes["level"].Value;
                            break;
                        case "move":
                            MoveProtection = xn.Attributes["level"].Value;
                            break;
                    }
                }
            }
            else
                xr.ReadToFollowing("revisions");

            xr.ReadToDescendant("rev");
            Timestamp = xr.GetAttribute("timestamp");
            Text = Tools.ConvertToLocalLineEndings(xr.ReadString());
        }

        /// <summary>
        /// 
        /// </summary>
        public string Title
        { get; private set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string Text
        { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Exists
        { get; internal set; }

        /// <summary>
        /// Revision ID, -1 if N/A
        /// </summary>
        public long RevisionID
        { get; internal set; }

        /// <summary>
        /// Namespace number
        /// </summary>
        public int NamespaceID
        { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string Timestamp
        { get; private set; }

        /// <summary>
        /// Edit token (http://www.mediawiki.org/wiki/Manual:Edit_token)
        /// </summary>
        public string EditToken { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public string EditProtection { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string MoveProtection { get; private set; }

        //TODO: waiting for https://bugzilla.wikimedia.org/show_bug.cgi?id=19523
        /// <summary>
        /// 
        /// </summary>
        public bool IsWatched
        { get { return false; } }
    }
}
