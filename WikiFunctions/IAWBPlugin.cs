using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using WikiFunctions.Logging;

namespace WikiFunctions.Plugin
{
    /* Please DO NOT CHANGE without consulting plugin authors, unless moving to a new AWB major version (v5, v6 etc).
     * This interface is a contract with external plugins. */
    public interface IAWBPlugin
    {
        void Initialise(IAutoWikiBrowser sender);
        string Name { get; }
        string ProcessArticle(IAutoWikiBrowser sender, ProcessArticleEventArgs eventargs);

        void LoadSettings(object[] Prefs);
        object[] SaveSettings();
        void Reset();

        void Nudge(out bool Cancel);
        void Nudged(int Nudges);
    }

    /* This interface allows plugins to manipulate AWB UI elements. Members can be added without breaking plugins,
     * since plugins use but don't implement the interface. Removing members is to be avoided if at all possible. */
    public interface IAutoWikiBrowser
    {
        Form Form { get; }
        TabPage MoreOptionsTab { get; }
        TabPage OptionsTab { get; }
        TabPage StartTab { get; }
        TabPage DabTab { get; }
        TabPage BotTab { get; }
        TextBox EditBox { get; }
        CheckBox BotModeCheckbox { get; }
        Button DiffButton { get; }
        Button PreviewButton { get; }
        Button SaveButton { get; }
        Button SkipButton { get; }
        Button StartButton { get; }
        Button StopButton { get; }
        ComboBox EditSummary { get; }
        StatusStrip StatusStrip { get; }
        NotifyIcon NotifyIcon { get; }
        ToolStripMenuItem HelpToolStripMenuItem { get; }
        CheckBox SkipNonExistentPagesCheckBox { get;  }
        CheckBox ApplyGeneralFixesCheckBox { get; }
        CheckBox AutoTagCheckBox { get; }
        ToolStripMenuItem PluginsToolStripMenuItem { get; }
        WikiFunctions.Lists.ListMaker ListMaker { get; }
        WikiFunctions.Browser.WebControl WebControl { get; }
        ContextMenuStrip EditBoxContextMenu { get; }
        TabControl Tab { get; }
        WikiFunctions.Parse.FindandReplace FindandReplace { get; }
        WikiFunctions.SubstTemplates SubstTemplates { get; }
        string CustomModule { get; }
        System.Version AWBVersion { get; }
        System.Version WikiFunctionsVersion { get; }
        void NotifyBalloon(string Message, ToolTipIcon Icon);
        int Nudges { get; }
        void AddLogItem(bool Skipped, AWBLogListener LogListener);

        void Start(IAWBPlugin sender);
        void Save(IAWBPlugin sender);
        void SkipPage(IAWBPlugin sender, string reason);
        void Stop(IAWBPlugin sender);
        void GetDiff(IAWBPlugin sender);
        void GetPreview(IAWBPlugin sender);

        void Start(string sender);
        void Save(string sender);
        void SkipPage(string sender, string reason);
        void Stop(string sender);
        void GetDiff(string sender);
        void GetPreview(string sender);
    }

    /* Members may be added to this interface, but not removed unless absolutely necessary. */
    public interface ProcessArticleEventArgs
    {
        string ArticleText { get; }
        string ArticleTitle { get; }
        string EditSummary { get; set; }
        int NameSpaceKey { get; }
        IMyTraceListener AWBLogItem { get; }
        bool Skip { get; set; }
    }

    public interface IModule
    {
        string ProcessArticle(string ArticleText, string ArticleTitle, int Namespace, out string Summary, out bool Skip);
    }
}
