using System;
using System.Collections.Generic;
using System.Text;

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
        /// Whether the user has an unread user talk message
        /// </summary>
        public bool HasMessage
        { get; internal set; }

        /// <summary>
        /// Creates a 
        /// </summary>
        /// <param name="xml"></param>
        internal UserInfo(string xml)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a UserInfo object for an unregistered user
        /// </summary>
        internal UserInfo()
        {
        }

        private List<string> Groups = new List<string>();
        private List<string> Rights = new List<string>();
    }
}
