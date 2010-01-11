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
using System.IO;

namespace WikiFunctions.API
{
    /// <summary>
    /// Information about a user
    /// </summary>
    public class UserInfo
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
        /// Whether the user is registered
        /// </summary>
        public bool IsRegistered
        { get { return Id != 0; } }

        /// <summary>
        /// Whether the current user is an administrator
        /// </summary>
        public bool IsSysop
        { get { return Groups.Contains("sysop"); } }

        /// <summary>
        /// Whether the current user is a flagged bot
        /// </summary>
        public bool IsBot
        { get { return Groups.Contains("bot"); } }

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
        /// Creates a UserInfo class from an meta=userinfo XML
        /// </summary>
        /// <param name="xml">XML to parse. Must be already checked for error status by 
        /// ApiEdit.CheckForErrors()</param>
        internal UserInfo(string xml)
        {
            XmlReader xr = XmlReader.Create(new StringReader(xml));
            xr.ReadToFollowing("userinfo");
            Name = xr.GetAttribute("name");
            Id = int.Parse(xr.GetAttribute("id"));
            IsBlocked = xr.GetAttribute("blockedby") != null;
            HasMessages = xr.GetAttribute("messages") != null;

            if (xr.ReadToFollowing("groups") && !xr.IsEmptyElement)
            {
                while (xr.Read() && xr.Name != "groups")
                {
                    if (xr.Name == "g") Groups.Add(xr.ReadString());
                }
            }

            if (xr.ReadToFollowing("rights") && !xr.IsEmptyElement)
            {
                while (xr.Read() && xr.Name != "rights")
                {
                    if (xr.Name == "r") Rights.Add(xr.ReadString());
                }
            }
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
