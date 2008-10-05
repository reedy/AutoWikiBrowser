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
using System.Text;

namespace WikiFunctions.API
{
    public interface IApiEdit
    {
        /// <summary>
        /// Path to scripts on server
        /// </summary>
        string URL
        { get; }

        /// <summary>
        /// Action for which we have edit token
        /// </summary>
        string Action
        { get; }

        /// <summary>
        /// Name of the page currently being edited
        /// </summary>
        string PageTitle
        { get; }

        /// <summary>
        /// Initial content of the page currently being edited
        /// </summary>
        string PageText
        { get; }

        /// <summary>
        /// Resets all internal variables, discarding edit tokens and so on,
        /// but does not logs off
        /// </summary>
        void Reset();

        /// <summary>
        /// Performs a HTTP request
        /// </summary>
        /// <param name="url"></param>
        /// <returns>Text received</returns>
        string HttpGet(string url);

        void Login(string username, string password);

        void Logout();

        /// <summary>
        /// Opens a page for editing
        /// </summary>
        /// <param name="title">Title of the page to edit</param>
        /// <returns>Page content</returns>
        string Open(string title);

        /// <summary>
        /// Saves the previously opened page
        /// </summary>
        /// <param name="pageText">New page content.</param>
        /// <param name="summary">Edit summary. Must not be empty.</param>
        /// <param name="minor">Whether the edit should be marked as minor</param>
        /// <param name="watch">Whether the page should be watchlisted</param>
        void Save(string pageText, string summary, bool minor, bool watch);

        /// <summary>
        /// Deletes the page
        /// </summary>
        /// <param name="title">Title of the page to delete</param>
        /// <param name="reason">Reason for deletion. Must not be empty.</param>
        void Delete(string title, string reason);

        void Protect(string title, string reason, string expiry, Protection edit, Protection move);

        void Protect(string title, string reason, TimeSpan expiry, Protection edit, Protection move);

        void MovePage(string title, string newTitle, string reason, bool moveTalk, bool noRedirect);

        /// <summary>
        /// Aborts the current operation
        /// </summary>
        void Abort();
    }

    /// <summary>
    /// Protection level for a page
    /// </summary>
    public enum Protection
    {
        None,
        Autoconfirmed,
        Sysop
    };
}
