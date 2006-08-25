using System;
using System.Collections.Generic;
using System.Text;

namespace WikiFunctions
{
    public interface IAWBPlugin
    {
        string Name { get; }
        string EditSummary { get;set; }
        string ProcessArticle(string ArticleText, string ArticleTitle, int Namespace, ref bool Skip);
    }
}
