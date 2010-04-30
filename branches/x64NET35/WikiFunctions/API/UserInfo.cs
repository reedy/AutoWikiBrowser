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

using System.Collections.Generic;
using System.Xml;

namespace WikiFunctions.API
{
    /// <summary>
    /// Information about a user
    /// </summary>
    public sealed class UserInfo
    {
        /// <summary>
        /// Username
        /// </summary>
        public string Name
        { get; private set; }

        /// <summary>
        /// Database ID
        /// </summary>
        public int Id
        { get; private set; }

        /// <summary>
        /// Whether we are logged in
        /// </summary>
        public bool IsLoggedIn
        { get { return Id != 0; } }

        /// <summary>
        /// Whether the current user is an administrator
        /// </summary>
        public bool IsSysop
        { get { return IsInGroup("sysop"); } }

        /// <summary>
        /// Whether the current user is a flagged bot
        /// </summary>
        public bool IsBot
        { get { return IsInGroup("bot") || HasRight("bot"); } }

        /// <summary>
        /// Whether the current user is blocked from editing
        /// </summary>
        public bool IsBlocked
        { get; private set; }

        /// <summary>
        /// Whether the user has an unread user talk message
        /// </summary>
        public bool HasMessages
        { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public bool IsInGroup(string group)
        {
            return string.IsNullOrEmpty(group) || Groups.Contains(group);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        public bool HasRight(string right)
        {
            return string.IsNullOrEmpty(right) || Rights.Contains(right);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public bool CanEditPage(PageInfo page)
        {
            return (IsInGroup(page.EditProtection) || HasRight(page.EditProtection)) 
                && !(page.NamespaceID == Namespace.MediaWiki && !HasRight("editinterface"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public bool CanMovePage(PageInfo page)
        {
            return page.NamespaceID != Namespace.MediaWiki 
                && (IsInGroup(page.MoveProtection) || HasRight(page.MoveProtection));
        }

        /// <summary>
        /// Creates a UserInfo class from an meta=userinfo XML
        /// </summary>
        /// <param name="xml">XML document to process. Must be already checked for error status by 
        /// ApiEdit.CheckForErrors()</param>
        internal UserInfo(XmlDocument xml)
        {
            var users = xml.GetElementsByTagName("userinfo");
            if (users.Count == 0) throw new BrokenXmlException(null, "XML with <userinfo> element expected");
            var user = users[0];

            Name = user.Attributes["name"].Value;
            Id = int.Parse(user.Attributes["id"].Value);

            var groups = user["groups"];
            if (groups != null)
            {
                foreach(XmlNode g in groups.GetElementsByTagName("g"))
                {
                    Groups.Add(g.InnerText);
                }
            }

            var rights = user["rights"];
            if (rights != null)
            {
                foreach (XmlNode r in rights.GetElementsByTagName("r"))
                {
                    Rights.Add(r.InnerText);
                }
            }

            Update(xml);
        }

        /// <summary>
        /// Updates the information about the current user
        /// </summary>
        /// <param name="xml">XML to process</param>
        internal void Update(XmlDocument xml)
        {
            var users = xml.GetElementsByTagName("userinfo");
            if (users.Count == 0) return;

            HasMessages = users[0].Attributes["messages"] != null;
            IsBlocked = users[0].Attributes["blockedby"] != null;
        }

        /// <summary>
        /// Creates a UserInfo object for an unregistered user
        /// </summary>
        internal UserInfo()
        {
        }

        private readonly List<string> Groups = new List<string>();
        private readonly List<string> Rights = new List<string>();
    }
}
