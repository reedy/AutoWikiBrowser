using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace WikiFunctions.Plugin
{
    public interface IAWBPlugin
    {
        void Initialise(WikiFunctions.Lists.ListMaker list, WikiFunctions.Browser.WebControl web, ToolStripMenuItem tsmi, ContextMenuStrip cms);
        string Name { get; }
        string ProcessArticle(string ArticleText, string ArticleTitle, int Namespace, ref string Summary, ref bool Skip);
    }
}
