using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using WikiFunctions.Logging;

namespace WikiFunctions.Plugin
{
    /* DO NOT CHANGE without consulting plugin authors. This interface is a contract with external plugins and
     * is of sufficient vintage to be considered non-negotiable. */
    public interface IAWBPlugin
    {
        void Initialise(IAutoWikiBrowser MainForm);
        string Name { get; }
        string ProcessArticle(IAutoWikiBrowser sender, ProcessArticleEventArgs eventargs);

        void LoadSettings(object[] Prefs);
        object[] SaveSettings();
        void Reset();

        void Nudge(out bool Cancel);
        void Nudged(int Nudges);
    }

    /* This interface allows plugins to manipulate AWB UI elements without (ahem) resorting to hacks.    
    The interface isn't considered "locked" yet so more properties and methods may be added if needed.  */
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
        void NotifyBalloon(string Message, ToolTipIcon Icon);
        int Nudges { get; }

        void Start();
        void Save();
        void SkipPage(string reason);
        void Stop();
        void GetDiff();
        void GetPreview();
    }

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
