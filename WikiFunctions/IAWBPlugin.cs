using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace WikiFunctions.Plugin
{
    public delegate void PluginEventHandler();

    /* DO NOT CHANGE. This interface is a contract with external plugins and is of sufficient vintage to be 
     * considered non-negotiable. Any changes should be made by declaring an additional new interface which
     * next-generation plugins may use. */
    public interface IAWBPlugin
    {
        event PluginEventHandler Start;
        event PluginEventHandler Save;
        event PluginEventHandler Skip;
        event PluginEventHandler Stop;
        event PluginEventHandler Diff;
        event PluginEventHandler Preview;

        void Initialise(WikiFunctions.Lists.ListMaker list, WikiFunctions.Browser.WebControl web, ToolStripMenuItem tsmi, ContextMenuStrip cms, TabControl tab, Form frm, TextBox txt);
        string Name { get; }
        string ProcessArticle(string ArticleText, string ArticleTitle, int Namespace, out string Summary, out bool Skip);

        void LoadSettings(object[] Prefs);
        object[] SaveSettings();
        void Reset();

        void Nudge(out bool Cancel);
        void Nudged(int Nudges);
    }

    /* This interface allows plugins to manipulate AWB UI elements without (ahem) resorting to hacks.
    Plugins are not *required* to implement the interface.
    
    The interface isn't considered "locked" yet so more properties and methods may be added if needed. */
    public interface IAWBMainForm
    {
        TabPage MoreOptionsTab { get; }
        TabPage OptionsTab { get; }
        TabPage StartTab { get; }
        TabPage DabTab { get; }
        TabPage BotTab { get; }
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
        Boolean SkipNonExistentPages { get; set; }
        void NotifyBalloon(string Message, ToolTipIcon Icon);
        int Nudges { get; }
    }

    public interface IModule
    {
        string ProcessArticle(string ArticleText, string ArticleTitle, int Namespace, out string Summary, out bool Skip);
    }
}
