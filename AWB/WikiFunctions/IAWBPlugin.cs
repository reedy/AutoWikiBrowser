using System;
using System.Collections.Generic;
using System.Text;

namespace WikiFunctions
{
    public interface IAWBPlugin
    {
        string Name { get; }

        string ProcessArticle(string ArticleText, string ArticleTitle, ref bool Skip);

    }
}
