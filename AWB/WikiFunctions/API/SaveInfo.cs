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

using System.Xml;
using System.IO;

namespace WikiFunctions.API
{
    /// <summary>
    /// Provides information about a successfully completed page editing operation
    /// </summary>
    public sealed class SaveInfo
    {
        /// <summary>
        /// Title of the edited page
        /// </summary>
        public string Title
        { get; private set; }

        /// <summary>
        /// Database ID of the page
        /// </summary>
        public int PageId
        { get; private set; }

        /// <summary>
        /// Database ID of page's revision before editing
        /// </summary>
        public int OldId
        { get; private set; }

        /// <summary>
        /// Database ID of page's revision after editing
        /// </summary>
        public int NewId
        { get; private set; }

        /// <summary>
        /// Whether the save operation actually didn't change anything
        /// </summary>
        public bool NoChange
        { get; private set; }

        /// <summary>
        /// true if we've just created a page
        /// </summary>
        public bool IsNewPage
        { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public XmlDocument ResponseXml
        { get; private set; }

        internal SaveInfo(XmlDocument doc)
        {
            ResponseXml = doc;

            try 
            {
                var edit = doc["api"]["edit"];

                NoChange = edit.HasAttribute("nochange");
                IsNewPage = edit.HasAttribute("new");
                Title = edit.GetAttribute("title");
                PageId = int.Parse(edit.GetAttribute("pageid"));

                int rev;
                int.TryParse(edit.GetAttribute("newrevid"), out rev); // will be absent on null edits
                NewId = rev;
                int.TryParse(edit.GetAttribute("oldrevid"), out rev); // will be absent on page creation too
                OldId = rev;
            }
            catch
            { }
        }
    }
}
