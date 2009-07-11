using System;
using System.Collections.Generic;
using System.Text;
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

        internal SaveInfo(string xml)
        {
            var xr = XmlReader.Create(new StringReader(xml));
            xr.ReadToFollowing("edit");
            // result="Success" should already be checked before
            Title = xr.GetAttribute("title");
            PageId = int.Parse(xr.GetAttribute("pageid"));
            NewId = int.Parse(xr.GetAttribute("newrevid"));
            int old;
            int.TryParse(xr.GetAttribute("oldrevid"), out old); // will be absent on page creation
            OldId = old;
        }
    }
}
