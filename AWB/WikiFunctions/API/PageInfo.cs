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

        // TODO: adopt for retrieval of information for protection, deletion, etc.
        internal PageInfo(string xml)
        {
            XmlReader xr = XmlReader.Create(new StringReader(xml));

            if (!xr.ReadToFollowing("page"))
                throw new Exception("Cannot find <page> element");

            string normalisedFrom = null, redirectFrom = null;

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            var redirects = doc.GetElementsByTagName("r");

            if (redirects.Count >= 1) //We have redirects
            {
                if (redirects.Count > 1 &&
                    redirects[0].Attributes["from"].Value == redirects[redirects.Count - 1].Attributes["to"].Value)
                {
                    //Redirect loop
                    TitleChangedStatus = PageTitleStatus.RedirectLoop;
                }
                else
                {
                    //Valid redirects
                    TitleChangedStatus = redirects.Count == 1
                                             ? PageTitleStatus.Redirected
                                             : PageTitleStatus.MultipleRedirects;
                }
                redirectFrom = redirects[0].Attributes["from"].Value;
            }
            else
            {
                TitleChangedStatus = PageTitleStatus.NoChange;
            }

            //Normalised before redirect, so would be root. Could still be multiple redirects, or looped
            var normalised = doc.GetElementsByTagName("n");

            if (normalised.Count > 0)
            {
                normalisedFrom = normalised[0].Attributes["from"].Value;

                if (TitleChangedStatus == PageTitleStatus.NoChange)
                    TitleChangedStatus = PageTitleStatus.Normalised;
                else
                    TitleChangedStatus |= PageTitleStatus.Normalised;
            }

            //Normalisation occurs before redirection, so if that exists, that is the title passed to the API
            if (!string.IsNullOrEmpty(normalisedFrom))
            {
                OriginalTitle = normalisedFrom;
            }
            else if (!string.IsNullOrEmpty(redirectFrom))
            {
                OriginalTitle = redirectFrom;
            }

            Exists = (xr.GetAttribute("missing") == null); //if null, page exists
            IsWatched = (xr.GetAttribute("watched") != null);
            EditToken = xr.GetAttribute("edittoken");
            TokenTimestamp = xr.GetAttribute("starttimestamp");

            long revId;
            RevisionID = long.TryParse(xr.GetAttribute("lastrevid"), out revId) ? revId : -1;

            Title = xr.GetAttribute("title");
            NamespaceID = int.Parse(xr.GetAttribute("ns"));

            if (xr.ReadToDescendant("protection") && !xr.IsEmptyElement)
            {
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
        /// Title of the Page
        /// </summary>
        public string Title
        { get; private set; }

        /// <summary>
        /// Original title (before redirects/normalisation) of the Page
        /// </summary>
        public string OriginalTitle
        { get; private set; }

        /// <summary>
        /// Why OriginalTitle differs from Title
        /// </summary>
        public PageTitleStatus TitleChangedStatus 
        { get; private set; }

        /// <summary>
        /// Text of the Page
        /// </summary>
        public string Text
        { get; private set; }

        /// <summary>
        /// Whether the page exists or not
        /// </summary>
        public bool Exists
        { get; private set; }

        /// <summary>
        /// Revision ID, -1 if N/A
        /// </summary>
        public long RevisionID
        { get; private set; }

        /// <summary>
        /// Namespace number
        /// </summary>
        public int NamespaceID
        { get; private set; }

        /// <summary>
        /// Timestamp of the latest revision of the page
        /// </summary>
        public string Timestamp
        { get; private set; }

        /// <summary>
        /// Edit token (http://www.mediawiki.org/wiki/Manual:Edit_token)
        /// </summary>
        public string EditToken
        { get; internal set; }

        /// <summary>
        /// Time when the token was obtained. Used for deletion detection.
        /// </summary>
        public string TokenTimestamp
        { get; private set; }

        /// <summary>
        /// String of any edit protection applied to the page
        /// </summary>
        public string EditProtection
        { get; private set; }

        /// <summary>
        /// String of any move protection applied to the page
        /// </summary>
        public string MoveProtection
        { get; private set; }

        /// <summary>
        /// Whether the current user is watching this page
        /// </summary>
        public bool IsWatched
        { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum PageTitleStatus
    {
        NoChange = 0,
        RedirectLoop = 1,
        MultipleRedirects = 2,
        Redirected = 4,
        Normalised = 5,
    }
}
