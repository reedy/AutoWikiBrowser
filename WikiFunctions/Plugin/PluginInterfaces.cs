/*
Copyright (C) 2007 Martin Richards
(C) 2007 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/

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

namespace WikiFunctions.Plugin
{
    /* Please DO NOT CHANGE without consulting plugin authors, unless moving to a new AWB major version (v5, v6 etc).
     * This interface is a contract with external plugins. If radical changes are needed, create a new additional i/f. */
    /// <summary>
    /// An interface for plugin components to be recognised by and interact with AWB
    /// </summary>
    public interface IAWBPlugin
    {
        /// <summary>
        /// AWB calls this method when it is ready to initialise your plugin
        /// </summary>
        /// <param name="sender">A reference to an active IAutoWikiBrowser object owned by AWB. This object may be used by the plugin to access all sorts of functionality in AWB.</param>
        void Initialise(IAutoWikiBrowser sender);

        /// <summary>
        /// The name of your plugin
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The name of your plugin in Mediawiki syntax
        /// </summary>
        string WikiName { get; }

        /// <summary>
        /// When AWB has an article to process, it calls this function in your plugin
        /// </summary>
        /// <param name="sender">A reference to an active IAutoWikiBrowser object owned by AWB</param>
        /// <param name="eventargs">An IProcessArticleEventArgs object, containing various read-only and read-write data</param>
        /// <returns></returns>
        string ProcessArticle(IAutoWikiBrowser sender, IProcessArticleEventArgs eventargs);

        /// <summary>
        /// Called by AWB when it loads a setting file
        /// </summary>
        /// <param name="prefs">An array of deserialised setting objects belonging to your plugin</param>
        void LoadSettings(object[] prefs);

        /// <summary>
        /// Called by AWB when it is saving settings
        /// </summary>
        /// <returns>An array of deserialised setting objects belonging to your plugin</returns>
        /// <remarks>Plugin authors have at least 4 ways of saving their settings, by returning an array of:
        /// 1. Simple, serializable types such as Strings
        /// 2. AWBSettings.PrefsKeyPair objects (used by the CFD/IFD plugins amongst others)
        /// 3. Custom public classes with each field marked as Serializable
        /// 4. An XML block converted to a String (used by the Kingbotk plugin)</remarks>
        object[] SaveSettings();

        /// <summary>
        /// Called by AWB when the user has requested to return to default settings. When this is called you should reset your plugin to it's default state.
        /// </summary>
        void Reset();

        /// <summary>
        /// AWB has got stuck and wants to "nudge" (stop and restart processing)
        /// </summary>
        /// <param name="cancel">True if you want to cancel the "nudging" operation</param>
        void Nudge(out bool cancel);

        /// <summary>
        ///  AWB performed a nudge
        /// </summary>
        /// <param name="nudges">How many nudges AWB has performed in this session</param>
        void Nudged(int nudges);
    }

    public delegate void GetLogUploadLocationsEvent(IAutoWikiBrowser sender, List<Logging.Uploader.LogEntry> locations);

    /* Members may be added to this interface, but not removed unless absolutely necessary. */
    /// <summary>
    /// Sent by AWB to plugins in ProcessArticle()
    /// </summary>
    public interface IProcessArticleEventArgs
    {
        /// <summary>
        /// The article text
        /// </summary>
        /// <remarks>Read only. The plugin should return the processed article text as the return value of ProcessArticle()</remarks>
        string ArticleText { get; }

        /// <summary>
        /// The article title
        /// </summary>
        /// <remarks>Read only.</remarks>
        string ArticleTitle { get; }

        /// <summary>
        /// Edit summary
        /// </summary>
        /// <remarks>Read/write.</remarks>
        string EditSummary { get; set; }

        /// <summary>
        /// Article's namespace
        /// </summary>
        /// <remarks>Read only.</remarks>
        int NameSpaceKey { get; }
        //IMyTraceListener AWBLogItem { get; }

        /// <summary>
        /// Set to True if AWB should skip this article
        /// </summary>
        bool Skip { get; set; }
    }

    /// <summary>
    /// An Interface for plugins to interact with the ListMaker for creating lists
    /// </summary>
    /// <remarks>Ideally we would pass an IAutoWikiBrowser at init() time, but that might not be thread-safe
    /// and also the Listmaker control is not AWB specific.</remarks>
    public interface IListMakerPlugin : Lists.IListProvider
    {
        /// <summary>
        /// The name of the plugin
        /// </summary>
        string Name { get; }
    }
}
