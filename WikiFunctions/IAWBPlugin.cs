using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using WikiFunctions.Logging;

namespace WikiFunctions.Plugin
{
    public delegate void PluginEventHandler();
    public delegate void PluginSkipEventHandler(string SkipReason);

    /* DO NOT CHANGE without consulting plugin authors. This interface is a contract with external plugins and
     * is of sufficient vintage to be considered non-negotiable. */
    public interface IAWBPlugin
    {
        event PluginEventHandler Start;
        event PluginEventHandler Save;
        event PluginSkipEventHandler Skip;
        event PluginEventHandler Stop;
        event PluginEventHandler Diff;
        event PluginEventHandler Preview;

        void Initialise(IAWBMainForm MainForm);
        string Name { get; }
        string ProcessArticle(IAWBMainForm sender, ProcessArticleEventArgs eventargs);

        void LoadSettings(object[] Prefs);
        object[] SaveSettings();
        void Reset();

        void Nudge(out bool Cancel);
        void Nudged(int Nudges);
    }

    public class ProcessArticleEventArgs
    {
        private string mArticleText, mArticleTitle, mEditSummary="";
        private int mNamespace;
        private IMyTraceListener mAWBLogItem;
        private bool mSkip=false;

        public ProcessArticleEventArgs(string ArticleText, string ArticleTitle, int Namespace, IMyTraceListener AWBLogItem)
        {
            mArticleText = ArticleText;
            mArticleTitle = ArticleTitle;
            mNamespace = Namespace;
            mAWBLogItem = AWBLogItem;
        }
        public string ArticleText { get { return mArticleText; } }
        public string ArticleTitle { get { return mArticleTitle; } }
        public string EditSummary { get { return mEditSummary; } set { mEditSummary = value.Trim(); } }
        public int Namespace { get { return mNamespace; } }
        public IMyTraceListener AWBLogItem { get { return mAWBLogItem; } }
        public bool Skip { get { return mSkip; } set { mSkip = value; } }
    }

    /* This interface allows plugins to manipulate AWB UI elements without (ahem) resorting to hacks.    
    The interface isn't considered "locked" yet so more properties and methods may be added if needed.  */
    public interface IAWBMainForm
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
    }

    public interface IModule
    {
        string ProcessArticle(string ArticleText, string ArticleTitle, int Namespace, out string Summary, out bool Skip);
    }
}
