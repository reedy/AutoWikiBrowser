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

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using WikiFunctions.Logging;

namespace WikiFunctions.Plugin
{
    /* Please DO NOT CHANGE without consulting plugin authors, unless moving to a new AWB major version (v5, v6 etc).
     * This interface is a contract with external plugins. If radical changes are needed, create a new additional i/f. */
    /// <summary>
    /// An interface for plugin components to be recognised by and interract with AWB
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
        /// <param name="eventargs">An ProcessArticleEventArgs object, containing various read-only and read-write data</param>
        /// <returns></returns>
        string ProcessArticle(IAutoWikiBrowser sender, ProcessArticleEventArgs eventargs);

        /// <summary>
        /// Called by AWB when it loads a setting file
        /// </summary>
        /// <param name="Prefs">An array of deserialised setting objects belonging to your plugin</param>
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
        /// <param name="Cancel">True if you want to cancel the "nudging" operation</param>
        void Nudge(out bool Cancel);

        /// <summary>
        ///  AWB performed a nudge
        /// </summary>
        /// <param name="Nudges">How many nudges AWB has performed in this session</param>
        void Nudged(int Nudges);
    }

    public interface IAutoWikiBrowserForm : IAutoWikiBrowserTabs
    {
        Form Form { get; }
        TextBox EditBox { get; }
        TextBox CategoryTextBox { get; }
        CheckBox BotModeCheckbox { get; }
        Button DiffButton { get; }
        Button PreviewButton { get; }
        Button SaveButton { get; }
        Button SkipButton { get; }
        Button StartButton { get; }
        Button StopButton { get; }
        ComboBox EditSummaryComboBox { get; }
        StatusStrip StatusStrip { get; }
        NotifyIcon NotifyIcon { get; }
        ToolStripMenuItem HelpToolStripMenuItem { get; }
        CheckBox SkipNonExistentPagesCheckBox { get;  }
        CheckBox ApplyGeneralFixesCheckBox { get; }
        CheckBox AutoTagCheckBox { get; }
        ToolStripMenuItem PluginsToolStripMenuItem { get; }
        ToolStripMenuItem InsertTagToolStripMenuItem { get; }
        ToolStripMenuItem ToolStripMenuGeneral { get; }
        WikiFunctions.Controls.Lists.ListMaker ListMaker { get; }
        WikiFunctions.Browser.WebControl WebControl { get; }
        ContextMenuStrip EditBoxContextMenu { get; }

        /// <summary>
        /// Display a message balloon above AWB's system tray icon
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="Icon"></param>
        void NotifyBalloon(string Message, ToolTipIcon Icon);
    }

    /// <summary>
    /// Gives access to the tab pages on the main AWB form
    /// </summary>
    public interface IAutoWikiBrowserTabs
    {
        TabPage MoreOptionsTab { get; }
        TabPage OptionsTab { get; }
        TabPage StartTab { get; }
        TabPage SkipTab { get; }
        TabPage DabTab { get; }
        TabPage BotTab { get; }
        TabPage LoggingTab { get; }
        void AddTabPage(TabPage tabp);
        void RemoveTabPage(TabPage tabp);
        void HideAllTabPages();
        void ShowAllTabPages();
        bool ContainsTabPage(TabPage tabp);
    }

    public interface IAutoWikiBrowserInfo
    {
        System.Version AWBVersion { get; }
        System.Version WikiFunctionsVersion { get; }
        string AWBVersionString { get; }
        string WikiFunctionsVersionString { get; }
        string WikiDiffVersionString { get; }
        int NumberOfEdits { get; }
        int NumberOfIgnoredEdits { get; }
        int NumberOfEditsPerMinute { get; }
        int Nudges { get; }
        ProjectEnum Project { get; }
        LangCodeEnum LangCode { get; }
        bool CheckStatus(bool Login);
    }

    public interface IAutoWikiBrowserCommands
    {
        void ShowHelp(string URL);
        void ShowHelpEnWiki(string Article);

        void Start(IAWBPlugin sender);
        void Start(string sender);
        void Stop(IAWBPlugin sender);
        void Stop(string sender);
        void Save(IAWBPlugin sender);
        void Save(string sender);
        /// <summary>
        /// Add an article log entry to the AWB Log tab
        /// </summary>
        /// <param name="Skipped">True if the article was skipped and needs to be added to the Skipped list on the AWB log tab, false if it should be added to the Saved articles list</param>
        /// <param name="LogListener">An AWBLogListener object - basically an object inherited from ListViewItem, which implements IMyTraceListener, and which represents a wiki article for logging purposes.</param>
        void AddLogItem(bool Skipped, AWBLogListener LogListener);

        /// <summary>
        /// Turn off any logging to files
        /// </summary>
        void TurnOffLogging();

        /// <summary>
        /// For the purposes of the logging tab, mark the page as Skipped and provide a reason.
        /// </summary>
        /// <remarks>ProcessArticleEventArgs.Skip needs to be set to True if you want AWB to actually skip the article.
        /// 
        ///  To write to all active log listeners, including logging to file listeners, use methods from IAutoWikiBrowser.TraceManager.</remarks>
        /// <param name="sender">A reference to the calling plugin</param>
        /// <param name="reason">The reason for skipping</param>
        void SkipPage(IAWBPlugin sender, string reason);

        /// <summary>
        /// For the purposes of the logging tab, mark the page as Skipped and provide a reason.
        /// </summary>
        /// <remarks>ProcessArticleEventArgs.Skip needs to be set to True if you want AWB to actually skip the article.
        /// 
        /// To write to all active log listeners, including logging to file listeners, use methods from IAutoWikiBrowser.TraceManager.</remarks>
        /// <param name="sender">The calling plugin's name</param>
        /// <param name="reason">The reason for skipping</param>
        void SkipPage(string sender, string reason);

        void GetDiff(IAWBPlugin sender);
        void GetDiff(string sender);
        void GetPreview(IAWBPlugin sender);
        void GetPreview(string sender);
    }

    /* This interface allows plugins to manipulate AWB UI elements. Members can be added without breaking plugins,
     * since plugins use but don't implement the interface. Removing members is to be avoided if at all possible. */
    public interface IAutoWikiBrowser : IAutoWikiBrowserForm, IAutoWikiBrowserCommands, IAutoWikiBrowserInfo
    {

        /// <summary>
        /// Returns a reference to a WikiFunctions.Logging.TraceManager class which handles AWB's logging. This object also implements the IMyTraceListener interface. Plugin authors can use this reference to write to all active loggers, including the AWB Log tab and logfiles.
        /// </summary>
        TraceManager TraceManager { get; } // implements IMyTraceListener
        WikiFunctions.Logging.Uploader.UploadableLogSettings2 LoggingSettings { get; }
        bool SkipNoChanges { get; set; }
        WikiFunctions.Parse.FindandReplace FindandReplace { get; }
        WikiFunctions.SubstTemplates SubstTemplates { get; }
        string CustomModule { get; }

        event GetLogUploadLocationsEvent GetLogUploadLocations;

        [ObsoleteAttribute]
        TabControl Tab { get; }
    }

    public delegate void GetLogUploadLocationsEvent(IAutoWikiBrowser sender, List<WikiFunctions.Logging.Uploader.LogEntry> locations);

    /* Members may be added to this interface, but not removed unless absolutely necessary. */
    /// <summary>
    /// Sent by AWB to plugins in ProcessArticle()
    /// </summary>
    public interface ProcessArticleEventArgs
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

    public interface IModule
    {
        string ProcessArticle(string ArticleText, string ArticleTitle, int Namespace, out string Summary, 
            out bool Skip);
    }

    public interface ISkipOptions
    {
        bool SkipNoUnicode
        { get; }

        bool SkipNoTag
        { get; }

        bool SkipNoHeaderError
        { get; }

        bool SkipNoBoldTitle
        { get; }

        bool SkipNoBulletedLink
        { get; }

        bool SkipNoBadLink
        { get; }

        bool SkipNoDefaultSortAdded
        { get; }
    }
}
