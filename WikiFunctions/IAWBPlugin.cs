using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace WikiFunctions.Plugin
{
    public interface IAWBPlugin
    {
        void Initialise(WikiFunctions.Lists.ListMaker list, WikiFunctions.Browser.WebControl web, ToolStripMenuItem tsmi, ContextMenuStrip cms);
        string Name { get; }
        string ProcessArticle(string ArticleText, string ArticleTitle, int Namespace, ref string Summary, ref bool Skip);

        void ReadXML(XmlTextReader Reader);
        void WriteXML(XmlTextWriter Writer);
        void Reset();
    }
}
