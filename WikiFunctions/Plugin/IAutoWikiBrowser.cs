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
using System.Windows.Forms;
using WikiFunctions.Logging;

namespace WikiFunctions.Plugin
{
    public interface IAutoWikiBrowserForm : IAutoWikiBrowserTabs
    {
        Form Form { get; }
        TextBoxBase EditBox { get; }
        TextBox CategoryTextBox { get; }
        CheckBox BotModeCheckbox { get; }
        CheckBox SkipNoChangesCheckBox { get; }
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
        RadioButton SkipNonExistentPages { get; }
        CheckBox ApplyGeneralFixesCheckBox { get; }
        CheckBox AutoTagCheckBox { get; }
        CheckBox RegexTypoFix { get; }
        ToolStripMenuItem PluginsToolStripMenuItem { get; }
        ToolStripMenuItem InsertTagToolStripMenuItem { get; }
        ToolStripMenuItem ToolStripMenuGeneral { get; }
        Controls.Lists.ListMaker ListMaker { get; }
        ContextMenuStrip EditBoxContextMenu { get; }
        LogControl LogControl { get; }
        Session TheSession { get; }

        /// <summary>
        /// Display a message balloon above AWB's system tray icon
        /// </summary>
        /// <param name="message"></param>
        /// <param name="icon"></param>
        void NotifyBalloon(string message, ToolTipIcon icon);

        string StatusLabelText { get; set; }
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
        Version AWBVersion { get; }
        Version WikiFunctionsVersion { get; }
        string AWBVersionString { get; }
        string WikiFunctionsVersionString { get; }
        string WikiDiffVersionString { get; }
        int NumberOfEdits { get; }
        int NumberOfIgnoredEdits { get; }
        int NumberOfEditsPerMinute { get; }
        int NumberOfPagesPerMinute { get; }
        int Nudges { get; }
        ProjectEnum Project { get; }
        string LangCode { get; }
        bool CheckStatus(bool login);
        bool Privacy { get; }
        bool Shutdown { get; }
    }

    /// <summary>
    ///  Exposes various commands a plugin or module may send to AWB
    /// </summary>
    public interface IAutoWikiBrowserCommands
    {
        void ShowHelp(string url);
        void ShowHelpEnWiki(string article);

        void Start(IAWBPlugin sender);
        void Start(string sender);
        void Stop(IAWBPlugin sender);
        void Stop(string sender);
        void Save(IAWBPlugin sender);
        void Save(string sender);
        /// <summary>
        /// Add an article log entry to the AWB Log tab
        /// </summary>
        /// <param name="skipped">True if the article was skipped and needs to be added to the Skipped list on the AWB log tab, false if it should be added to the Saved articles list</param>
        /// <param name="logListener">An AWBLogListener object - basically an object inherited from ListViewItem, which implements IMyTraceListener, and which represents a wiki article for logging purposes.</param>
        void AddLogItem(bool skipped, AWBLogListener logListener);

        /// <summary>
        /// Turn off any logging to files
        /// </summary>
        void TurnOffLogging();

        /// <summary>
        /// For the purposes of the logging tab, mark the page as Skipped and provide a reason.
        /// </summary>
        /// <remarks>IProcessArticleEventArgs.Skip needs to be set to True if you want AWB to actually skip the article.
        /// 
        ///  To write to all active log listeners, including logging to file listeners, use methods from IAutoWikiBrowser.TraceManager.</remarks>
        /// <param name="sender">A reference to the calling plugin</param>
        /// <param name="reason">The reason for skipping</param>
        void SkipPage(IAWBPlugin sender, string reason);

        /// <summary>
        /// For the purposes of the logging tab, mark the page as Skipped and provide a reason.
        /// </summary>
        /// <remarks>IProcessArticleEventArgs.Skip needs to be set to True if you want AWB to actually skip the article.
        /// 
        /// To write to all active log listeners, including logging to file listeners, use methods from IAutoWikiBrowser.TraceManager.</remarks>
        /// <param name="sender">The calling plugin's name</param>
        /// <param name="reason">The reason for skipping</param>
        void SkipPage(string sender, string reason);

        void GetDiff(IAWBPlugin sender);
        void GetDiff(string sender);
        void GetPreview(IAWBPlugin sender);
        void GetPreview(string sender);

        /// <summary>
        /// To add another FormClosingEventHandler onto that of the Main form Closing event.
        /// Pass an reference to the handler, and it will be +='d onto the FormClosingEvent
        /// </summary>
        /// <param name="handler">FormClosingEventHandler to add onto Main Form FormClosing Event</param>
        void AddMainFormClosingEventHandler(FormClosingEventHandler handler);

        void StartProgressBar();
        void StopProgressBar();

        void AddArticleRedirectedEventHandler(ArticleRedirected handler);
    }

    /* This interface allows plugins to manipulate AWB UI elements. Members can be added without breaking plugins,
     * since plugins use but don't implement the interface. Removing members is to be avoided if at all possible. */
    public interface IAutoWikiBrowser : IAutoWikiBrowserForm, IAutoWikiBrowserCommands, IAutoWikiBrowserInfo
    {
        /// <summary>
        /// Returns a reference to a WikiFunctions.Logging.TraceManager class which handles AWB's logging. This object also implements the IMyTraceListener interface. Plugin authors can use this reference to write to all active loggers, including the AWB Log tab and logfiles.
        /// </summary>
        TraceManager TraceManager { get; } // implements IMyTraceListener
        Logging.Uploader.UploadableLogSettings2 LoggingSettings { get; }
        bool SkipNoChanges { get; set; }
        Parse.FindandReplace FindandReplace { get; }
        SubstTemplates SubstTemplates { get; }
        string CustomModule { get; }

        event GetLogUploadLocationsEvent GetLogUploadLocations;

        [ObsoleteAttribute]
        TabControl Tab { get; }
    }
}
