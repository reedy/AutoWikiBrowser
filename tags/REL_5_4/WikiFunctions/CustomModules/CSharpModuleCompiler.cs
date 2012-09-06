using Microsoft.CSharp;

namespace WikiFunctions.CustomModules
{

    public class CSharpCustomModule : CustomModuleCompiler
    {
        public CSharpCustomModule()
        {
            Compiler = new CSharpCodeProvider();
        }

        public override string Name
        {
            get { return "C# 2.0"; }
        }

        public override string CodeStart
        {
            get
            {
                return @"using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using WikiFunctions;

namespace AutoWikiBrowser.CustomModules
{
    class CustomModule : WikiFunctions.Plugin.IModule
    {
        WikiFunctions.Plugin.IAutoWikiBrowser awb;

        public CustomModule(WikiFunctions.Plugin.IAutoWikiBrowser mAWB)
        {
           awb =  mAWB;
        }
";
            }
        }

        public override string CodeEnd
        {
            get
            {
                return @"    }
}";
            }
        }

        public override string CodeExample
        {
            get
            {
                return @"        public string ProcessArticle(string ArticleText, string ArticleTitle, int wikiNamespace, out string Summary, out bool Skip)
        {
            Skip = false;
            Summary = ""test"";

            ArticleText = ""test \r\n\r\n"" + ArticleText;

            return ArticleText;
        }";
            }
        }
    }
}