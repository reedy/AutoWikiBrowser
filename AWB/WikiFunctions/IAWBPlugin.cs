using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace WikiFunctions.Plugin
{
    public interface IAWBPlugin
    {
        void Initialise(Form f, ToolStripMenuItem tsmi, ContextMenuStrip cms);
        string Name { get; }
        string EditSummary { get;set; }
        string ProcessArticle(string ArticleText, string ArticleTitle, int Namespace, ref bool Skip);
    }
}
