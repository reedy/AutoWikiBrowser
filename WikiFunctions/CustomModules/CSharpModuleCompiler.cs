using Microsoft.CSharp;
using System.Collections.Generic;

namespace WikiFunctions.CustomModules
{

    public class CSharpCustomModule : CustomModuleCompiler
    {
        public CSharpCustomModule()
        {
            Compiler = new CSharpCodeProvider(new Dictionary<string, string> {{"CompilerVersion", "v3.5"}});
        }

        public override string Name
        {
            get { return "C# 3.5"; }
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
using System.Linq;

namespace AutoWikiBrowser.CustomModules
{
    class CustomModule : WikiFunctions.Plugin.IModule
    {
        WikiFunctions.Plugin.IAutoWikiBrowser awb;

        public CustomModule(WikiFunctions.Plugin.IAutoWikiBrowser _awb)
        {
           awb =  _awb;
        }
";
            }
        }

        public override string CodeEnd
        {
            get { return @"    }
}"; }
        }

        public override string CodeExample
        {
            get
            {
                return
                    @"        public string ProcessArticle(string ArticleText, string ArticleTitle, int wikiNamespace, out string Summary, out bool Skip)
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