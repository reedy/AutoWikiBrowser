using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace WikiFunctions.Plugin
{
    public delegate void PluginEventHandler();

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

        void ReadXML(XmlTextReader Reader);
        void WriteXML(XmlTextWriter Writer);
        void Reset();
    }

    public interface IModule
    {
        string ProcessArticle(string ArticleText, string ArticleTitle, int Namespace, out string Summary, out bool Skip);
    }
}
