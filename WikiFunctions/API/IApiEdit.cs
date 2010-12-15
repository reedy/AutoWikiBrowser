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

        bool PHP5
        { get; }

        /// <summary>
        /// Properties of the currently logged in user
        /// </summary>
        UserInfo User
        { get; }

        /// <summary>
        /// Maxlag parameter of every request (http://www.mediawiki.org/wiki/Manual:Maxlag_parameter)
        /// </summary>
        int Maxlag
        { get; set; }

        /// <summary>
        /// If true, all attempts to edit while the user has unread messages result in NewMessagesException,
        /// otherwise only User.HasMessages is updated.
        /// </summary>
        bool NewMessageThrows
        { get; set; }

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
        /// Retrieves the code of CSS/JS to be used for previews. Forced to default skin for anons.
        /// </summary>
        string HtmlHeaders
        { get; }

        /// <summary>
        /// Creates a new instance of the current class by cloning the current instance
        /// ATTENTION: the clones will share the same cookie container, so logging off or logging under another username
        /// with one instance will automatically make another one do the same
        /// </summary>
        IApiEdit Clone();

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
        /// Opens a page for editing
        /// </summary>
        /// <param name="title">Title of the page to edit</param>
        /// <param name="resolveRedirects">Whether to have the API resolve any redirects</param>
        /// <returns>Page content</returns>
        string Open(string title, bool resolveRedirects);

        /// <summary>
        /// Saves the previously opened page
        /// </summary>
        /// <param name="pageText">New page content.</param>
        /// <param name="summary">Edit summary. Must not be empty.</param>
        /// <param name="minor">Whether the edit should be marked as minor</param>
        /// <param name="watch">Whether the page should be watchlisted</param>
        SaveInfo Save(string pageText, string summary, bool minor, WatchOptions watch);

        /// <summary>
        /// Adds the given page to current user's watchlist
        /// </summary>
        /// <param name="title">Page to add to watchlist</param>
        void Watch(string title);

        /// <summary>
        /// Removes the given page from current user's watchlist
        /// </summary>
        /// <param name="title">Page to remove from watchlist</param>
        void Unwatch(string title);

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
        /// Protects the Page
        /// </summary>
        /// <param name="title">Title of the page to protect</param>
        /// <param name="reason">Reason for protection. Must not be empty.</param>
        /// <param name="expiry"></param>
        /// <param name="edit"></param>
        /// <param name="move"></param>
        void Protect(string title, string reason, string expiry, string edit, string move);

        /// <summary>
        /// Protects the Page
        /// </summary>
        /// <param name="title">Title of the page to protect</param>
        /// <param name="reason">Reason for protection. Must not be empty.</param>
        /// <param name="expiry"></param>
        /// <param name="edit"></param>
        /// <param name="move"></param>
        void Protect(string title, string reason, TimeSpan expiry, string edit, string move);

        /// <summary>
        /// Protects the Page
        /// </summary>
        /// <param name="title">Title of the page to protect</param>
        /// <param name="reason">Reason for protection. Must not be empty.</param>
        /// <param name="expiry"></param>
        /// <param name="edit"></param>
        /// <param name="move"></param>
        /// <param name="cascade"></param>
        /// <param name="watch">Whether to add the page to your watchlist</param>
        void Protect(string title, string reason, string expiry, string edit, string move, bool cascade, bool watch);

        /// <summary>
        /// Protects the Page
        /// </summary>
        /// <param name="title">Title of the page to protect</param>
        /// <param name="reason">Reason for protection. Must not be empty.</param>
        /// <param name="expiry"></param>
        /// <param name="edit"></param>
        /// <param name="move"></param>
        /// <param name="cascade"></param>
        /// <param name="watch">Whether to add the page to your watchlist</param>
        void Protect(string title, string reason, TimeSpan expiry, string edit, string move, bool cascade, bool watch);

        /// <summary>
        /// Moves the page
        /// </summary>
        /// <param name="title">Title of the page to move</param>
        /// <param name="newTitle">Title of the target page</param>
        /// <param name="reason">Reason for move. Must not be empty.</param>
        void Move(string title, string newTitle, string reason);

        /// <summary>
        /// Moves the page
        /// </summary>
        /// <param name="titleid">Page id to move</param>
        /// <param name="newTitle">Title of the target page</param>
        /// <param name="reason">Reason for move. Must not be empty.</param>
        /// <param name="moveTalk">Whether to also move the talk page</param>
        /// <param name="noRedirect">Whether a redirect shoudn't be created</param>
        /// <param name="watch">Whether to add the page to your watchlist</param>
        void Move(int titleid, string newTitle, string reason, bool moveTalk, bool noRedirect, bool watch);

        /// <summary>
        /// Moves the page
        /// </summary>
        /// <param name="title">Title of the page to move</param>
        /// <param name="newTitle">Title of the target page</param>
        /// <param name="reason">Reason for move. Must not be empty.</param>
        /// <param name="moveTalk">Whether to also move the talk page</param>
        /// <param name="noRedirect">Whether a redirect shoudn't be created</param>
        void Move(string title, string newTitle, string reason, bool moveTalk, bool noRedirect);

        /// <summary>
        /// Moves the page
        /// </summary>
        /// <param name="title">Title of the page to move</param>
        /// <param name="newTitle">Title of the target page</param>
        /// <param name="reason">Reason for move. Must not be empty.</param>
        /// <param name="moveTalk">Whether to also move the talk page</param>
        /// <param name="noRedirect">Whether a redirect shoudn't be created</param>
        /// <param name="watch">Whether to add the page to your watchlist</param>
        void Move(string title, string newTitle, string reason, bool moveTalk, bool noRedirect, bool watch);

        /// <summary>
        /// Previews the page
        /// </summary>
        /// <param name="title">Title of the page to preview</param>
        /// <param name="text"></param>
        /// <returns></returns>
        string Preview(string title, string text);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="user"></param>
        void Rollback(string title, string user);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        string ExpandTemplates(string title, string text);

        /// <summary>
        /// Aborts the current operation
        /// </summary>
        void Abort();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryParameters"></param>
        /// <returns></returns>
        string QueryApi(string queryParameters);
    }
}
