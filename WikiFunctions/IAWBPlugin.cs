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
    }

    public interface IModule
    {
        string ProcessArticle(string ArticleText, string ArticleTitle, int Namespace, out string Summary, out bool Skip);
    }
}
