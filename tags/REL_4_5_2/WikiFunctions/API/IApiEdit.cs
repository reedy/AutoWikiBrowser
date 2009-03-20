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
        /// true if the editor is asynchronous
        /// Whether all operations should return immediately or wait for completition
        /// </summary>
        bool Asynchronous
        { get; }

        /// <summary>
        /// Properties of the currently logged in user
        /// </summary>
        UserInfo User
        { get; }

        /// <summary>
        /// Updates the User property
        /// </summary>
        void RefreshUserInfo();

        /// <summary>
        /// Action for which we have edit token
        /// </summary>
        string Action
        { get; }

        /// <summary>
        /// Information about the page currently being modified
        /// </summary>
        PageInfo Page
        { get; }

        /// <summary>
        /// Retrieves the code of CSS/JS to be used for previews
        /// </summary>
        string HtmlHeaders
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        void Login(string username, string password);

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// Deletes the page
        /// </summary>
        /// <param name="title">Title of the page to delete</param>
        /// <param name="reason">Reason for deletion. Must not be empty.</param>
        /// <param name="watch">Whether to add the page to your watchlist</param>
        void Delete(string title, string reason, bool watch);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="reason"></param>
        /// <param name="expiry"></param>
        /// <param name="edit"></param>
        /// <param name="move"></param>
        void Protect(string title, string reason, string expiry, Protection edit, Protection move);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="reason"></param>
        /// <param name="expiry"></param>
        /// <param name="edit"></param>
        /// <param name="move"></param>
        void Protect(string title, string reason, TimeSpan expiry, Protection edit, Protection move);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="reason"></param>
        /// <param name="expiry"></param>
        /// <param name="edit"></param>
        /// <param name="move"></param>
        /// <param name="cascade"></param>
        /// <param name="watch"></param>
        void Protect(string title, string reason, string expiry, Protection edit, Protection move, bool cascade, bool watch);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="reason"></param>
        /// <param name="expiry"></param>
        /// <param name="edit"></param>
        /// <param name="move"></param>
        /// <param name="cascade"></param>
        /// <param name="watch"></param>
        void Protect(string title, string reason, TimeSpan expiry, Protection edit, Protection move, bool cascade, bool watch);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="newTitle"></param>
        /// <param name="reason"></param>
        /// <param name="moveTalk"></param>
        /// <param name="noRedirect"></param>
        void MovePage(string title, string newTitle, string reason, bool moveTalk, bool noRedirect);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="newTitle"></param>
        /// <param name="reason"></param>
        /// <param name="moveTalk"></param>
        /// <param name="noRedirect"></param>
        /// <param name="watch"></param>
        void MovePage(string title, string newTitle, string reason, bool moveTalk, bool noRedirect, bool watch);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageTitle"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        string Preview(string pageTitle, string text);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageTitle"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        string ExpandTemplates(string pageTitle, string text);

        /// <summary>
        /// Aborts the current operation
        /// </summary>
        void Abort();

        /// <summary>
        /// Waits for current operation to complete. If the editor is not asynchronous,
        /// returns immediately.
        /// </summary>
        void Wait();
    }
}
